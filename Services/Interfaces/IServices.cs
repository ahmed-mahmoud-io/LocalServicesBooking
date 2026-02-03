using LocalServicesBooking.Models.Entities;
using LocalServicesBooking.Models.ViewModels;

namespace LocalServicesBooking.Services.Interfaces
{
    public interface IProviderService
    {
        Task<Provider?> GetProviderByUserIdAsync(int userId);
        Task<Provider?> GetProviderByIdAsync(int providerId);
        Task CreateProviderProfileAsync(Provider provider);
        Task<List<LocalServicesBooking.Models.Entities.ProviderService>> GetProviderServicesAsync(int providerId);
        Task<LocalServicesBooking.Models.Entities.ProviderService?> GetProviderServiceByIdAsync(int serviceId);
        Task AddProviderServiceAsync(LocalServicesBooking.Models.Entities.ProviderService service);
        Task<List<ProviderCardViewModel>> SearchProvidersAsync(string query, string location, decimal? minPrice, decimal? maxPrice, decimal? minRating, string sortBy);
        Task<List<ServiceCategory>> GetCategoriesAsync();
        
        Task<List<ProviderAvailability>> GetAvailabilityAsync(int providerId);
        Task UpdateAvailabilityAsync(int providerId, List<ProviderAvailability> availabilities);
    }

    public interface IBookingService
    {
        Task<Booking?> GetBookingByIdAsync(int bookingId);
        Task<List<Booking>> GetCustomerBookingsAsync(int customerId);
        Task<List<Booking>> GetProviderBookingsAsync(int providerId);
        Task CreateBookingAsync(Booking booking);
        Task UpdateBookingStatusAsync(int bookingId, string status);
        Task<bool> IsProviderAvailableAsync(int providerId, DateTime date, TimeSpan time);
    }

    public interface IMessagingService
    {
        Task<List<ConversationListViewModel>> GetConversationsAsync(int userId);
        Task<ChatViewModel?> GetConversationAsync(int bookingId, int currentUserId);
        Task SendMessageAsync(int bookingId, int senderId, string messageText);
    }
}
