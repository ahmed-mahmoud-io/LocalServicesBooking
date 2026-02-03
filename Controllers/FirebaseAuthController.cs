using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LocalServicesBooking.Models.Entities;
using LocalServicesBooking.Models;
using System.Security.Claims;

namespace LocalServicesBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FirebaseAuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public FirebaseAuthController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyToken([FromBody] FirebaseLoginRequest request)
        {
            try
            {
                // Verify the ID token via Firebase Admin SDK
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(request.IdToken);
                var uid = decodedToken.Uid;
                var email = decodedToken.Claims.ContainsKey("email") ? decodedToken.Claims["email"].ToString() : null;
                var name = decodedToken.Claims.ContainsKey("name") ? decodedToken.Claims["name"].ToString() : "Firebase User";

                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("Email is required.");
                }

                // Check if user exists in our DB
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    // Create new user (Auto-registration)
                    var nameParts = name.Split(' ', 2);
                    user = new User
                    {
                        UserName = email,
                        Email = email,
                        FirstName = nameParts[0],
                        LastName = nameParts.Length > 1 ? nameParts[1] : "",
                        EmailConfirmed = true,
                        CreatedAt = DateTime.UtcNow,
                        UserType = "Customer" // Default to Customer
                    };

                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        return BadRequest(result.Errors);
                    }
                }

                // Sign in the user to ASP.NET Core Identity
                await _signInManager.SignInAsync(user, isPersistent: true);

                return Ok(new { message = "Login successful", username = user.UserName });
            }
            catch (FirebaseAuthException ex)
            {
                return Unauthorized($"Invalid Token: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Error: {ex.Message}");
            }
        }
    }
}
