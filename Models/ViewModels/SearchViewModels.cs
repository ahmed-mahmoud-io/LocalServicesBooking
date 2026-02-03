using LocalServicesBooking.Models.Entities;

namespace LocalServicesBooking.Models.ViewModels
{
    public class SearchViewModel
    {
        public string? ServiceQuery { get; set; }
        public string? Location { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? MinRating { get; set; }
        public string? SortBy { get; set; } // "Recommended", "Price", "Rating"
        
        public List<ProviderCardViewModel> Results { get; set; } = new List<ProviderCardViewModel>();
        public int TotalResults { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class ProviderCardViewModel
    {
        public int ProviderId { get; set; }
        public string BusinessName { get; set; }
        public string ProviderName { get; set; }
        public string ProfileImageUrl { get; set; }
        public bool IsVerified { get; set; }
        public decimal Rating { get; set; }
        public int ReviewCount { get; set; }
        public decimal StartingPrice { get; set; }
        public string PriceUnit { get; set; }
        public string ServiceName { get; set; }
        public bool IsAvailableToday { get; set; }
    }
}
