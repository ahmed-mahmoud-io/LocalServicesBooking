using System.ComponentModel.DataAnnotations;

namespace LocalServicesBooking.Models.Entities
{
    public class ServiceCategory
    {
        [Key]
        public int CategoryId { get; set; }
        
        [Required]
        public string CategoryName { get; set; }
        public string? IconName { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<ProviderService> Services { get; set; }
    }
}
