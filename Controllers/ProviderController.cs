using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LocalServicesBooking.Models.Entities;
using LocalServicesBooking.Models.ViewModels;
using LocalServicesBooking.Services.Interfaces;

namespace LocalServicesBooking.Controllers
{
    [Authorize]
    public class ProviderController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IProviderService _providerService;

        public ProviderController(UserManager<User> userManager, IProviderService providerService)
        {
            _userManager = userManager;
            _providerService = providerService;
        }

        [HttpGet]
        public async Task<IActionResult> Setup()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var existingProvider = await _providerService.GetProviderByUserIdAsync(user.Id);
            if (existingProvider != null)
            {
                return RedirectToAction("Provider", "Dashboard"); 
            }

            return View(new ProviderSetupViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Setup(ProviderSetupViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound();

                var provider = new Provider
                {
                    UserId = user.Id,
                    BusinessName = model.BusinessName,
                    Bio = model.Bio,
                    YearsOfExperience = model.YearsOfExperience,
                    IsPetFriendly = model.IsPetFriendly,
                    IsNonSmoker = model.IsNonSmoker,
                    HasOwnSupplies = model.HasOwnSupplies,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                await _providerService.CreateProviderProfileAsync(provider);
                return RedirectToAction("ManageServices");
            }
            return View(model);
        }

        public async Task<IActionResult> ManageServices()
        {
            var user = await _userManager.GetUserAsync(User);
            var provider = await _providerService.GetProviderByUserIdAsync(user.Id);
            
            if (provider == null) return RedirectToAction("Setup");

            var services = await _providerService.GetProviderServicesAsync(provider.ProviderId);
            
            var model = new ManageServicesViewModel
            {
                Services = services
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AddService()
        {
            var categories = await _providerService.GetCategoriesAsync();
            ViewBag.Categories = categories;
            return View(new AddServiceViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> AddService(AddServiceViewModel model)
        {
             var user = await _userManager.GetUserAsync(User);
             var provider = await _providerService.GetProviderByUserIdAsync(user.Id);
             if (provider == null) return RedirectToAction("Setup");

             if (ModelState.IsValid)
             {
                 var service = new ProviderService
                 {
                     ProviderId = provider.ProviderId,
                     CategoryId = model.CategoryId,
                     ServiceName = model.ServiceName,
                     Description = model.Description,
                     BasePrice = model.BasePrice,
                     PriceUnit = model.PriceUnit,
                     DurationMinutes = model.DurationMinutes,
                     IsActive = true
                 };

                 await _providerService.AddProviderServiceAsync(service);
                 return RedirectToAction("ManageServices");
             }
             
             ViewBag.Categories = await _providerService.GetCategoriesAsync();
             return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Availability()
        {
            var user = await _userManager.GetUserAsync(User);
            var provider = await _providerService.GetProviderByUserIdAsync(user.Id);
            if (provider == null) return RedirectToAction("Setup");

            var availabilities = await _providerService.GetAvailabilityAsync(provider.ProviderId);

            // If no availability set, initialize default M-F 9-5
            if (availabilities == null || !availabilities.Any())
            {
                var defaults = new List<ProviderAvailabilityViewModel>();
                foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
                {
                    defaults.Add(new ProviderAvailabilityViewModel
                    {
                        DayOfWeek = day,
                        IsAvailable = day != DayOfWeek.Saturday && day != DayOfWeek.Sunday,
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(17, 0, 0)
                    });
                }
                
                // Sort starting from Monday (business standard)
                defaults = defaults.OrderBy(d => d.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)d.DayOfWeek).ToList();

                return View(new ManageAvailabilityViewModel { Availabilities = defaults });
            }

            // Map Entity to ViewModel
            var modelList = availabilities.Select(a => new ProviderAvailabilityViewModel
            {
                DayOfWeek = a.DayOfWeek,
                IsAvailable = a.IsAvailable,
                StartTime = a.StartTime,
                EndTime = a.EndTime
            }).OrderBy(d => d.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)d.DayOfWeek).ToList();

            return View(new ManageAvailabilityViewModel { Availabilities = modelList });
        }

        [HttpPost]
        public async Task<IActionResult> Availability(ManageAvailabilityViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var provider = await _providerService.GetProviderByUserIdAsync(user.Id);
            if (provider == null) return RedirectToAction("Setup");

            if (ModelState.IsValid)
            {
                var entities = model.Availabilities.Select(vm => new ProviderAvailability
                {
                    ProviderId = provider.ProviderId,
                    DayOfWeek = vm.DayOfWeek,
                    IsAvailable = vm.IsAvailable,
                    StartTime = vm.StartTime,
                    EndTime = vm.EndTime
                }).ToList();

                await _providerService.UpdateAvailabilityAsync(provider.ProviderId, entities);
                TempData["SuccessMessage"] = "Availability updated successfully!";
                return RedirectToAction("Availability");
            }

            return View(model);
        }
    }
}
