using System.ComponentModel.DataAnnotations;

namespace LocalServicesBooking.Models.ViewModels
{
    public class CreateBookingViewModel
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string ProviderName { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public string PriceUnit { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; }
        
        [Required]
        [Display(Name = "Time")]
        [DataType(DataType.Time)]
        public TimeSpan BookingTime { get; set; }
        
        [Required]
        [Display(Name = "Address")]
        public string LocationAddress { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "City")]
        public string LocationCity { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Zip Code")]
        public string LocationZipCode { get; set; } = string.Empty;
        
        [Display(Name = "Additional Notes")]
        public string? Notes { get; set; }
    }
}
