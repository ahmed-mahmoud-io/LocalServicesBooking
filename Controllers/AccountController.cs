using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LocalServicesBooking.Models.Entities;
using LocalServicesBooking.Models.ViewModels;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace LocalServicesBooking.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User 
                { 
                    UserName = model.Email, 
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserType = model.UserType,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.UserType);

                    // Generate Verification Code
                    var code = new Random().Next(100000, 999999).ToString();
                    user.EmailVerificationCode = code;
                    user.EmailVerificationCodeSentAt = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);

                    await _emailSender.SendEmailAsync(model.Email, "Confirm your email", $"Your verification code is: {code}");
                    
                    return RedirectToAction("VerifyEmail", new { email = model.Email });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult VerifyEmail(string email)
        {
             if (string.IsNullOrEmpty(email)) return RedirectToAction("Register");
             return View(new VerifyEmailViewModel { Email = email });
        }

        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) 
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            if (user.EmailConfirmed) return RedirectToAction("Login");

            if (user.EmailVerificationCode == model.Code)
            {
                user.EmailConfirmed = true;
                user.EmailVerificationCode = null;
                await _userManager.UpdateAsync(user);
                
                await _signInManager.SignInAsync(user, isPersistent: false);
                
                if (user.UserType == "Provider") return RedirectToAction("Setup", "Provider");
                return RedirectToAction("Index", "Home");
            }
            
            ModelState.AddModelError("", "Invalid verification code.");
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Login");
            }

            try
            {
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    ModelState.AddModelError(string.Empty, "Error loading external login information.");
                    return View("Login");
                }

                // Sign in the user with this external login provider if the user already has a login.
                var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }
                if (result.IsLockedOut)
                {
                    return RedirectToAction("Lockout");
                }
                else
                {
                    // If the user does not have an account, then ask the user to create an account.
                    var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                    if (email != null)
                    {
                        var user = await _userManager.FindByEmailAsync(email);
                        if (user == null)
                        {
                            user = new User 
                            { 
                                UserName = email, 
                                Email = email,
                                FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
                                LastName = info.Principal.FindFirstValue(ClaimTypes.Surname),
                                UserType = "Customer", // Default behavior
                                EmailConfirmed = true,
                                ProfileImageUrl = info.Principal.FindFirstValue("picture") // Try to get profile picture
                            };
                            var createResult = await _userManager.CreateAsync(user);
                            if (createResult.Succeeded)
                            {
                                createResult = await _userManager.AddLoginAsync(user, info);
                                if (createResult.Succeeded)
                                {
                                    await _signInManager.SignInAsync(user, isPersistent: false);
                                    return LocalRedirect(returnUrl);
                                }
                            }
                        }
                        else
                        {
                            var linkResult = await _userManager.AddLoginAsync(user, info);
                            if (linkResult.Succeeded)
                            {
                                await _signInManager.SignInAsync(user, isPersistent: false);
                                return LocalRedirect(returnUrl);
                            }
                        }
                    }

                    // If we got here, something failed
                    ModelState.AddModelError(string.Empty, "Could not create an account for you.");
                    return View("Login");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Internal Error during Google Login: {ex.Message} --- {ex.InnerException?.Message}");
                return View("Login");
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new UserProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                ProfileImageUrl = user.ProfileImageUrl,
                Email = user.Email,
                UserType = user.UserType
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(UserProfileViewModel model)
        {
             if (!ModelState.IsValid) return View(model);
             
             var user = await _userManager.GetUserAsync(User);
             if (user == null) return NotFound();

             user.FirstName = model.FirstName;
             user.LastName = model.LastName;
             user.PhoneNumber = model.PhoneNumber;
             user.ProfileImageUrl = model.ProfileImageUrl; // In a real app, handle file upload
             user.UpdatedAt = DateTime.UtcNow;

             var result = await _userManager.UpdateAsync(user);
             if (result.Succeeded)
             {
                 TempData["SuccessMessage"] = "Profile updated successfully!";
                 return RedirectToAction("Profile");
             }

             foreach (var error in result.Errors)
             {
                 ModelState.AddModelError(string.Empty, error.Description);
             }

             return View(model);
        }
    }
}
