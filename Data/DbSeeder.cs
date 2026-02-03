using LocalServicesBooking.Data;
using LocalServicesBooking.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace LocalServicesBooking.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Microsoft.AspNetCore.Identity.IdentityRole<int>>>();

            if (!await roleManager.RoleExistsAsync("Provider")) await roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole<int>("Provider"));
            if (!await roleManager.RoleExistsAsync("Customer")) await roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole<int>("Customer"));

            // Ensure categories exist or update them
            var categories = new List<ServiceCategory>
            {
                new ServiceCategory { CategoryName = "Plumber", IconName = "bi-wrench", DisplayOrder = 1 },
                new ServiceCategory { CategoryName = "Electrician", IconName = "bi-lightning-charge", DisplayOrder = 2 },
                new ServiceCategory { CategoryName = "Cleaner", IconName = "bi-stars", DisplayOrder = 3 },
                new ServiceCategory { CategoryName = "Tutor", IconName = "bi-book", DisplayOrder = 4 },
                new ServiceCategory { CategoryName = "Photographer", IconName = "bi-camera", DisplayOrder = 5 },
                new ServiceCategory { CategoryName = "Mover", IconName = "bi-truck", DisplayOrder = 6 }
            };

            foreach (var cat in categories)
            {
                var existing = await context.ServiceCategories.FirstOrDefaultAsync(c => c.CategoryName == cat.CategoryName);
                if (existing == null)
                {
                    context.ServiceCategories.Add(cat);
                }
                else
                {
                    // Update icon if changed
                    existing.IconName = cat.IconName;
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
