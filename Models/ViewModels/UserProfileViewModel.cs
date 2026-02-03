using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

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
        public string? ProfileImageUrl { get; set; }
        
        [Display(Name = "Profile Picture")]
        public IFormFile? ProfileImage { get; set; }
        
        [Display(Name = "Gender")]
        public string? Gender { get; set; } // "Male" or "Female"
        
        // Read-only logic for display if needed
        [Display(Name = "Email Address")]
        public string Email { get; set; }
        
        public string UserType { get; set; }
    }
}
