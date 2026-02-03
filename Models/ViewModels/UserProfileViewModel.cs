using System.ComponentModel.DataAnnotations;

namespace LocalServicesBooking.Models.ViewModels
{
    public class UserProfileViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Profile Image URL")]
        [Url]
        public string? ProfileImageUrl { get; set; }
        
        // Read-only logic for display if needed
        [Display(Name = "Email Address")]
        public string Email { get; set; }
        
        public string UserType { get; set; }
    }
}
