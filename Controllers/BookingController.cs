using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LocalServicesBooking.Models.Entities;
using LocalServicesBooking.Models.ViewModels;
using LocalServicesBooking.Services.Interfaces;

namespace LocalServicesBooking.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IBookingService _bookingService;
        private readonly IProviderService _providerService;

        public BookingController(UserManager<User> userManager, IBookingService bookingService, IProviderService providerService)
        {
            _userManager = userManager;
            _bookingService = bookingService;
            _providerService = providerService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int serviceId)
        {
            var service = await _providerService.GetProviderServiceByIdAsync(serviceId);
            if (service == null) return NotFound();

            var model = new CreateBookingViewModel
            {
                ServiceId = service.ServiceId,
                ServiceName = service.ServiceName,
                ProviderName = service.Provider.BusinessName ?? $"{service.Provider.User.FirstName} {service.Provider.User.LastName}",
                BasePrice = service.BasePrice,
                PriceUnit = service.PriceUnit,
                BookingDate = DateTime.Today.AddDays(1)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                 if (user == null) return NotFound();
                 
                var service = await _providerService.GetProviderServiceByIdAsync(model.ServiceId);
                if (service == null) return NotFound();

                var booking = new Booking
                {
                    CustomerId = user.Id,
                    ProviderId = service.ProviderId,
                    ServiceId = service.ServiceId,
                    BookingDate = model.BookingDate,
                    BookingTime = model.BookingTime,
                    Status = "Pending",
                    LocationAddress = model.LocationAddress,
                    LocationCity = model.LocationCity,
                    LocationZipCode = model.LocationZipCode,
                    EstimatedCost = service.BasePrice, // Simple logic
                    Notes = model.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                await _bookingService.CreateBookingAsync(booking);
                
                return RedirectToAction("Index", "Dashboard");
            }
            return View(model);
        }
        
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var booking = await _bookingService.GetBookingByIdAsync(id);
            
            if (booking == null) return NotFound();
            
            // Security check
            if (booking.CustomerId != user.Id && booking.Provider.UserId != user.Id)
                return Forbid();
                
            return View(booking);
        }
    }
}
