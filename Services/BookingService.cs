using Microsoft.EntityFrameworkCore;
using LocalServicesBooking.Data;
using LocalServicesBooking.Models.Entities;
using LocalServicesBooking.Services.Interfaces;

namespace LocalServicesBooking.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;

        public BookingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Booking?> GetBookingByIdAsync(int bookingId)
        {
            return await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Provider).ThenInclude(p => p.User)
                .Include(b => b.Service)
                .Include(b => b.Messages)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
        }

        public async Task<List<Booking>> GetCustomerBookingsAsync(int customerId)
        {
            return await _context.Bookings
                .Include(b => b.Provider).ThenInclude(p => p.User)
                .Include(b => b.Service)
                .Where(b => b.CustomerId == customerId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetProviderBookingsAsync(int providerId)
        {
             return await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Service)
                .Where(b => b.ProviderId == providerId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }

        public async Task CreateBookingAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBookingStatusAsync(int bookingId, string status)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking != null)
            {
                booking.Status = status;
                booking.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsProviderAvailableAsync(int providerId, DateTime date, TimeSpan time)
        {
            // 1. Check if the provider has working hours for this day
            var availability = await _context.ProviderAvailabilities
                .FirstOrDefaultAsync(a => a.ProviderId == providerId && a.DayOfWeek == date.DayOfWeek);

            // If no availability record exists, assume unavailable (or available by default depending on business logic - assuming Unavailable for explicit setup)
            if (availability == null || !availability.IsAvailable)
            {
                // For MVP: If no schedule set, we might assume 9-5 MF? Or just return false.
                // Let's assume false to force them to set schedule.
                return false; 
            }

            // 2. Check if time is within working hours
            if (time < availability.StartTime || time >= availability.EndTime)
            {
                return false;
            }

            // 3. Check for overlapping bookings
            // Booking duration isn't strictly defined per slot, assuming 1 hour slots for now.
            var exists = await _context.Bookings.AnyAsync(b => 
                b.ProviderId == providerId && 
                b.BookingDate.Date == date.Date &&
                b.BookingTime.Hours == time.Hours &&
                b.Status != "Cancelled" && b.Status != "Rejected");
            
            return !exists;
        }
    }
}
