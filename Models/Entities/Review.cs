using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalServicesBooking.Models.Entities
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        public int BookingId { get; set; }
        [ForeignKey("BookingId")]
        public Booking Booking { get; set; }

        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public User Customer { get; set; }

        public int ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public Provider Provider { get; set; }

        [Range(1, 5)]
        public int OverallRating { get; set; }
        
        [Range(1, 5)]
        public int CommunicationRating { get; set; }
        
        [Range(1, 5)]
        public int TimelinessRating { get; set; }
        
        [Range(1, 5)]
        public int ValueRating { get; set; }
        
        public string? ReviewText { get; set; }
        public bool IsPublic { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ReviewPhoto> Photos { get; set; }
    }

    public class ReviewPhoto
    {
        [Key]
        public int PhotoId { get; set; }

        public int ReviewId { get; set; }
        [ForeignKey("ReviewId")]
        public Review Review { get; set; }

        [Required]
        public string PhotoUrl { get; set; }
        
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
