using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalServicesBooking.Models.Entities
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }

        public int SenderId { get; set; }
        [ForeignKey("SenderId")]
        public User Sender { get; set; }

        public int ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public User Receiver { get; set; }

        public int BookingId { get; set; }
        [ForeignKey("BookingId")]
        public Booking Booking { get; set; }

        public string? MessageText { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsRead { get; set; } = false;
        
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
