using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalServicesBooking.Models.Entities
{
    public class ProviderAvailability
    {
        [Key]
        public int AvailabilityId { get; set; }

        public int ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public Provider? Provider { get; set; }

        public DayOfWeek DayOfWeek { get; set; }
        
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        
        public bool IsAvailable { get; set; } = true;
    }
}
