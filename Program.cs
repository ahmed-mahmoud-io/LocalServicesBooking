using Microsoft.EntityFrameworkCore;
using LocalServicesBooking.Data;
using LocalServicesBooking.Models.Entities;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<User, IdentityRole<int>>(options => 
    {
        options.SignIn.RequireConfirmedAccount = true;
        
        // Password settings - Easier requirements
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false; // No special characters required
        options.Password.RequiredLength = 6;
        options.Password.RequiredUniqueChars = 1;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, LocalServicesBooking.Services.EmailSender>();

// Initialize Firebase Admin SDK (Optional - only if service account key exists)
var serviceAccountPath = builder.Configuration["Firebase:ServiceAccountKeyPath"] ?? "serviceAccountKey.json";
try
{
    if (File.Exists(serviceAccountPath))
    {
        FirebaseAdmin.FirebaseApp.Create(new FirebaseAdmin.AppOptions()
        {
            Credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile(serviceAccountPath)
        });
        Console.WriteLine("Firebase initialized successfully");
    }
    else
    {
        Console.WriteLine($"Firebase ServiceAccountKey not found at {serviceAccountPath} - Firebase features will be disabled");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Failed to initialize Firebase: {ex.Message} - Firebase features will be disabled");
}

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "google-client-id";
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "google-client-secret";
    })
    .AddApple(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Apple:ClientId"] ?? "apple-client-id";
        options.KeyId = builder.Configuration["Authentication:Apple:KeyId"] ?? "apple-key-id";
        options.TeamId = builder.Configuration["Authentication:Apple:TeamId"] ?? "apple-team-id";
        options.ClientSecret = "placeholder-secret"; // Required for validation if private key is not set
        // Need to load PrivateKey properly in real scenario, this is a placeholder
        // options.UsePrivateKey(
        //     (keyId) =>  /* load key */ 
        // );
    });

// Service Layer Registration
builder.Services.AddScoped<LocalServicesBooking.Services.Interfaces.IProviderService, LocalServicesBooking.Services.ProviderManager>();
builder.Services.AddScoped<LocalServicesBooking.Services.Interfaces.IBookingService, LocalServicesBooking.Services.BookingService>();
builder.Services.AddScoped<LocalServicesBooking.Services.Interfaces.IMessagingService, LocalServicesBooking.Services.MessagingService>();

builder.Services.AddSignalR();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Apply pending migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate(); // Apply any pending migrations
        LocalServicesBooking.Data.DbSeeder.SeedAsync(app).Wait();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

// Configure the HTTP request pipeline.
// if (!app.Environment.IsDevelopment())
// {
    // app.UseExceptionHandler("/Home/Error");
    // app.UseHsts();
// }
app.UseDeveloperExceptionPage();

// app.UseHttpsRedirection(); // Commented: MonsterASP handles SSL termination
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapHub<LocalServicesBooking.Hubs.ChatHub>("/chatHub");

app.Run();
