using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalServicesBooking.Models.Entities
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public User? Customer { get; set; }

        public int ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public Provider? Provider { get; set; }

        public int ServiceId { get; set; }
        [ForeignKey("ServiceId")]
        public ProviderService? Service { get; set; }

        public DateTime BookingDate { get; set; }
        public TimeSpan BookingTime { get; set; }
        
        [Required]
        public string Status { get; set; } = string.Empty; // 'Pending', 'Confirmed', 'InProgress', 'Completed', 'Cancelled'
        
        public string? LocationAddress { get; set; }
        public string? LocationCity { get; set; }
        public string? LocationState { get; set; }
        public string? LocationZipCode { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal? EstimatedCost { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal? FinalCost { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal? ServiceFee { get; set; }
        
        public string? Notes { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public ICollection<Quote> Quotes { get; set; } = new List<Quote>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
