using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LocalServicesBooking.Models.Entities;

namespace LocalServicesBooking.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Core Tables
        public DbSet<Provider> Providers { get; set; }
        public DbSet<ServiceCategory> ServiceCategories { get; set; }
        public DbSet<ProviderService> ProviderServices { get; set; }

        // Operational
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewPhoto> ReviewPhotos { get; set; }
        public DbSet<ProviderAvailability> ProviderAvailabilities { get; set; }
        public DbSet<ProviderPhoto> ProviderPhotos { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // Rename Identity Tables to match desired schema if needed, or keep defaults (AspNetUsers, etc.)
            // For now, let's keep defaults but ensure relationships are clean.
            
            builder.Entity<User>().ToTable("Users");
            builder.Entity<IdentityRole<int>>().ToTable("Roles");
            builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
            builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");

            // Provider -> User relationship
            builder.Entity<Provider>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking Relationships
            builder.Entity<Booking>()
                .HasOne(b => b.Customer)
                .WithMany()
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Booking>()
                .HasOne(b => b.Provider)
                .WithMany(p => p.Bookings)
                .HasForeignKey(b => b.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Booking>()
                .HasOne(b => b.Service)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Message Relationships
            builder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<Message>()
                .HasOne(m => m.Booking)
                .WithMany(b => b.Messages)
                .HasForeignKey(m => m.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            // Review Relationships
            builder.Entity<Review>()
                .HasOne(r => r.Booking)
                .WithMany(b => b.Reviews)
                .HasForeignKey(r => r.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Review>()
                .HasOne(r => r.Customer)
                .WithMany()
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Review>()
                .HasOne(r => r.Provider)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Quote Relationships
            builder.Entity<Quote>()
                .HasOne(q => q.Booking)
                .WithMany(b => b.Quotes)
                .HasForeignKey(q => q.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Quote>()
                .HasOne(q => q.Provider)
                .WithMany()
                .HasForeignKey(q => q.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
