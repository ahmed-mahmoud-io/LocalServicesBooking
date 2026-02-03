using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalServicesBooking.Models.Entities
{
    public class ProviderPhoto
    {
        [Key]
        public int PhotoId { get; set; }

        public int ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public Provider? Provider { get; set; }

        public string PhotoUrl { get; set; } = string.Empty;
        
        public string? Caption { get; set; }
        public int DisplayOrder { get; set; }
    }
}
