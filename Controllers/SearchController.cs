using Microsoft.AspNetCore.Mvc;
using LocalServicesBooking.Models.ViewModels;
using LocalServicesBooking.Services.Interfaces;

namespace LocalServicesBooking.Controllers
{
    public class SearchController : Controller
    {
        private readonly IProviderService _providerService;

        public SearchController(IProviderService providerService)
        {
            _providerService = providerService;
        }

        public async Task<IActionResult> Index(string query, string location, 
            decimal? minPrice, decimal? maxPrice, decimal? minRating, 
            string sortBy, int page = 1)
        {
            // Note: Pagination logic inside Service or here? 
            // The service method we built returns all matching list. 
            // Ideally service should support Skip/Take or return IQueryable (bad practice for full decoupling).
            // For MVP refactor, we fetch list then paginate in memory or update service.
            // Let's update service later if performance issue.
            
            var allResults = await _providerService.SearchProvidersAsync(query, location, minPrice, maxPrice, minRating, sortBy);
            
            int pageSize = 10;
            int totalResults = allResults.Count;
            var pagedResults = allResults
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new SearchViewModel
            {
                ServiceQuery = query,
                Location = location,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                MinRating = minRating,
                SortBy = sortBy,
                Results = pagedResults,
                TotalResults = totalResults,
                CurrentPage = page
            };

            return View(model);
        }
    }
}
