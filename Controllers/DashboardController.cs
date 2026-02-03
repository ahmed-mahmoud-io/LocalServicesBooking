using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LocalServicesBooking.Data;
using LocalServicesBooking.Models.Entities;
using LocalServicesBooking.Models.ViewModels;

namespace LocalServicesBooking.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.UserType == "Provider")
            {
                return RedirectToAction("Provider");
            }
            return RedirectToAction("Customer");
        }

        [Authorize(Roles = "Provider")] // Or check UserType manually if Roles not fully used
        public async Task<IActionResult> Provider()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.UserType != "Provider") return RedirectToAction("Customer");

            var provider = await _context.Providers
                .Include(p => p.Bookings)
                    .ThenInclude(b => b.Customer)
                .Include(p => p.Bookings)
                    .ThenInclude(b => b.Service)
                .FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (provider == null) return RedirectToAction("Setup", "Provider");

            var bookings = provider.Bookings ?? new List<Booking>();

            var model = new ProviderDashboardViewModel
            {
                UpcomingBookingsCount = bookings.Count(b => b.Status == "Confirmed" && b.BookingDate >= DateTime.Today),
                TotalEarnings = bookings.Where(b => b.Status == "Completed").Sum(b => b.FinalCost ?? 0),
                NewRequestsCount = bookings.Count(b => b.Status == "Pending"),
                ActiveJobs = bookings.Where(b => b.Status == "Pending" || b.Status == "Confirmed" || b.Status == "InProgress")
                    .OrderBy(b => b.BookingDate).ToList()
            };

            return View(model);
        }

        public async Task<IActionResult> Customer()
        {
            var user = await _userManager.GetUserAsync(User);
            var bookings = await _context.Bookings
                .Include(b => b.Provider).ThenInclude(p => p.User)
                .Include(b => b.Service)
                .Where(b => b.CustomerId == user.Id)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            var model = new CustomerDashboardViewModel
            {
                UpcomingBookings = bookings.Where(b => b.BookingDate >= DateTime.Today && b.Status != "Completed" && b.Status != "Cancelled").ToList(),
                PastBookings = bookings.Where(b => b.BookingDate < DateTime.Today || b.Status == "Completed" || b.Status == "Cancelled").ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int bookingId, string status)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking != null)
            {
                booking.Status = status;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
