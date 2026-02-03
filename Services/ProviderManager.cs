using Microsoft.EntityFrameworkCore;
using LocalServicesBooking.Data;
using LocalServicesBooking.Models.Entities;
using LocalServicesBooking.Models.ViewModels;
using LocalServicesBooking.Services.Interfaces;

namespace LocalServicesBooking.Services
{
    public class ProviderManager : IProviderService
    {
        private readonly ApplicationDbContext _context;

        public ProviderManager(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Provider?> GetProviderByUserIdAsync(int userId)
        {
            return await _context.Providers.FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<Provider?> GetProviderByIdAsync(int providerId)
        {
            return await _context.Providers
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.ProviderId == providerId);
        }

        public async Task CreateProviderProfileAsync(Provider provider)
        {
            _context.Providers.Add(provider);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Models.Entities.ProviderService>> GetProviderServicesAsync(int providerId)
        {
            return await _context.ProviderServices
                .Include(s => s.ServiceCategory)
                .Where(s => s.ProviderId == providerId)
                .ToListAsync();
        }

        public async Task<Models.Entities.ProviderService?> GetProviderServiceByIdAsync(int serviceId)
        {
            return await _context.ProviderServices
                .Include(s => s.Provider)
                .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(s => s.ServiceId == serviceId);
        }

        public async Task AddProviderServiceAsync(Models.Entities.ProviderService service)
        {
            _context.ProviderServices.Add(service);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ServiceCategory>> GetCategoriesAsync()
        {
             return await _context.ServiceCategories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
        }

        public async Task<List<ProviderCardViewModel>> SearchProvidersAsync(string query, string location, decimal? minPrice, decimal? maxPrice, decimal? minRating, string sortBy)
        {
            var servicesQuery = _context.ProviderServices
                .Include(s => s.Provider)
                .ThenInclude(p => p.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                servicesQuery = servicesQuery.Where(s => 
                    s.ServiceName.Contains(query) || 
                    s.Description.Contains(query) ||
                    s.Provider.BusinessName.Contains(query));
            }

            if (minPrice.HasValue)
            {
                servicesQuery = servicesQuery.Where(s => s.BasePrice >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                servicesQuery = servicesQuery.Where(s => s.BasePrice <= maxPrice.Value);
            }

            if (minRating.HasValue)
            {
                servicesQuery = servicesQuery.Where(s => s.Provider.AverageRating >= minRating.Value);
            }

            // Sorting logic
            servicesQuery = sortBy switch
            {
                "Price" => servicesQuery.OrderBy(s => s.BasePrice),
                "Rating" => servicesQuery.OrderByDescending(s => s.Provider.AverageRating),
                _ => servicesQuery.OrderByDescending(s => s.Provider.IsVerified).ThenByDescending(s => s.Provider.AverageRating)
            };

            return await servicesQuery
                .Select(s => new ProviderCardViewModel
                {
                    ProviderId = s.ProviderId,
                    BusinessName = s.Provider.BusinessName,
                    ProviderName = (s.Provider.User.FirstName ?? "") + " " + (s.Provider.User.LastName ?? ""),
                    ProfileImageUrl = s.Provider.User.ProfileImageUrl,
                    IsVerified = s.Provider.IsVerified,
                    Rating = s.Provider.AverageRating,
                    ReviewCount = s.Provider.TotalReviews,
                    StartingPrice = s.BasePrice,
                    PriceUnit = s.PriceUnit,
                    ServiceName = s.ServiceName,
                    IsAvailableToday = true
                })
                .ToListAsync();
        }

        public async Task<List<ProviderAvailability>> GetAvailabilityAsync(int providerId)
        {
            return await _context.ProviderAvailabilities
                .Where(a => a.ProviderId == providerId)
                .OrderBy(a => a.DayOfWeek)
                .ToListAsync();
        }

        public async Task UpdateAvailabilityAsync(int providerId, List<ProviderAvailability> availabilities)
        {
            var existing = await _context.ProviderAvailabilities
                .Where(a => a.ProviderId == providerId)
                .ToListAsync();

            _context.ProviderAvailabilities.RemoveRange(existing);
            
            foreach(var item in availabilities)
            {
                item.ProviderId = providerId; // Ensure ID is correct
            }
            
            _context.ProviderAvailabilities.AddRange(availabilities);
            await _context.SaveChangesAsync();
        }
    }
}
