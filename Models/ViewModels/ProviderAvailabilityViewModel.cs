using System.ComponentModel.DataAnnotations;

namespace LocalServicesBooking.Models.ViewModels
{
    public class ProviderAvailabilityViewModel
    {
        public DayOfWeek DayOfWeek { get; set; }
        
        public bool IsAvailable { get; set; }
        
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; } = new TimeSpan(9, 0, 0); // Default 9 AM
        
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; } = new TimeSpan(17, 0, 0); // Default 5 PM
    }

    public class ManageAvailabilityViewModel
    {
        public List<ProviderAvailabilityViewModel> Availabilities { get; set; } = new List<ProviderAvailabilityViewModel>();
    }
}
