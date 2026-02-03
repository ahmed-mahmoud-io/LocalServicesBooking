using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalServicesBooking.Models.Entities
{
    public class Quote
    {
        [Key]
        public int QuoteId { get; set; }

        public int BookingId { get; set; }
        [ForeignKey("BookingId")]
        public Booking Booking { get; set; }

        public int ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public Provider Provider { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal QuoteAmount { get; set; }
        
        public string? Description { get; set; }
        public string Status { get; set; } = "Pending"; // 'Pending', 'Accepted', 'Rejected'
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }
    }
}
