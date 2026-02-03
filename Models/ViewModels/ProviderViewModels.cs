using System.ComponentModel.DataAnnotations;
using LocalServicesBooking.Models.Entities;

namespace LocalServicesBooking.Models.ViewModels
{
    public class ProviderSetupViewModel
    {
        [Required]
        [Display(Name = "Business Name")]
        public string BusinessName { get; set; }
        
        [Required]
        [Display(Name = "Professional Bio")]
        public string Bio { get; set; }
        
        [Required]
        [Range(0, 50)]
        [Display(Name = "Years of Experience")]
        public int YearsOfExperience { get; set; }
        
        [Display(Name = "Are you pet friendly?")]
        public bool IsPetFriendly { get; set; }
        
        [Display(Name = "Non-smoker?")]
        public bool IsNonSmoker { get; set; }
        
        [Display(Name = "Do you have your own supplies?")]
        public bool HasOwnSupplies { get; set; }
    }

    public class ManageServicesViewModel
    {
        public int ProviderId { get; set; }
        public List<ProviderService> Services { get; set; }
        public List<ServiceCategory> Categories { get; set; }
    }

    public class AddServiceViewModel
    {
        public int ProviderId { get; set; }
        
        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        
        [Required]
        [Display(Name = "Service Name")]
        public string ServiceName { get; set; }
        
        [Display(Name = "Description")]
        public string? Description { get; set; }
        
        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Base Price")]
        public decimal BasePrice { get; set; }
        
        [Required]
        [Display(Name = "Price Unit (e.g., hour, job)")]
        public string PriceUnit { get; set; }
        
        [Display(Name = "Estimated Duration (minutes)")]
        public int? DurationMinutes { get; set; }
    }
}
