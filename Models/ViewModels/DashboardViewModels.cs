using LocalServicesBooking.Models.Entities;

namespace LocalServicesBooking.Models.ViewModels
{
    public class ProviderDashboardViewModel
    {
        public int UpcomingBookingsCount { get; set; }
        public decimal TotalEarnings { get; set; }
        public int NewRequestsCount { get; set; }
        public List<Booking> ActiveJobs { get; set; }
    }

    public class CustomerDashboardViewModel
    {
        public List<Booking> UpcomingBookings { get; set; }
        public List<Booking> PastBookings { get; set; }
    }
}
