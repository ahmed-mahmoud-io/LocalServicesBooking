using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LocalServicesBooking.Models.Entities
{
    public class User : IdentityUser<int>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        // PhoneNumber, Email, PasswordHash are inherited from IdentityUser
        
        [Required]
        public string UserType { get; set; } // 'Customer' or 'Provider'
        
        public string? ProfileImageUrl { get; set; }
        
        public string? Gender { get; set; } // "Male", "Female", or null
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
        
        [StringLength(6)]
        public string? EmailVerificationCode { get; set; }
        public int EmailVerificationCodeAttempts { get; set; }
        public DateTime? EmailVerificationCodeSentAt { get; set; }
        
        // IsEmailVerified is inhereted as EmailConfirmed
    }
}
