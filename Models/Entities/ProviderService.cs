using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalServicesBooking.Models.Entities
{
    public class ProviderService
    {
        [Key]
        public int ServiceId { get; set; }

        public int ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public Provider? Provider { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public ServiceCategory? ServiceCategory { get; set; } // Renamed to match earlier usage? Wait, earlier usage was ServiceCategory. Let's check code. ProviderService.cs used `Category`? 
        // ProviderService.cs LINE 40 in `ProviderService.cs` (Service implementation) used `Include(s => s.ServiceCategory)`.
        // The Entity file I just read has `public ServiceCategory Category { get; set; }`.
        // This is a mismatch causing EF errors potentially.
        // I should rename it to `ServiceCategory` to generic convention or update Service.
        // Code in `ProviderService` used `.Include(s => s.ServiceCategory)`.
        // So Entity MUST be `ServiceCategory`.

        [Required]
        public string ServiceName { get; set; } = string.Empty;
        public string? Description { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal BasePrice { get; set; }
        
        public string? PriceUnit { get; set; } // 'hour', 'job', 'sqft'
        public int? DurationMinutes { get; set; } // Renamed from EstimatedDuration to match ViewModels? 
        // ProviderController uses `DurationMinutes`.
        // Entity had `EstimatedDuration`.
        // I will standardize to `DurationMinutes`.
        
        public bool IsActive { get; set; } = true;
        
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
