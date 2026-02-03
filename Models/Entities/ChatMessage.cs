using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalServicesBooking.Models.Entities
{
    public class ChatMessage
    {
        [Key]
        public int MessageId { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Text { get; set; } = "";

        public string? ImageUrl { get; set; }

        public int? ReplyToMessageId { get; set; }

        [ForeignKey("ReplyToMessageId")]
        public ChatMessage? ReplyToMessage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;
    }
}
