using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalServicesBooking.Models.Entities
{
    public class Provider
    {
        [Key]
        public int ProviderId { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public string? BusinessName { get; set; }
        public string? Bio { get; set; }
        public int YearsOfExperience { get; set; }
        public bool IsVerified { get; set; } = false;
        public bool IsBackgroundChecked { get; set; } = false;
        
        [Column(TypeName = "decimal(3,2)")]
        public decimal AverageRating { get; set; } = 0;
        
        public int TotalReviews { get; set; } = 0;
        public bool IsPetFriendly { get; set; } = false;
        public bool IsNonSmoker { get; set; } = false;
        public bool HasOwnSupplies { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public int? AverageResponseTimeMinutes { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<ProviderService> Services { get; set; } = new List<ProviderService>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<ProviderAvailability> Availabilities { get; set; } = new List<ProviderAvailability>();
        public ICollection<ProviderPhoto> Photos { get; set; } = new List<ProviderPhoto>();
    }
}
