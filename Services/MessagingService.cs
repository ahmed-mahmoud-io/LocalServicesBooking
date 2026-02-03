using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using LocalServicesBooking.Data;
using LocalServicesBooking.Hubs;
using LocalServicesBooking.Models.Entities;
using LocalServicesBooking.Models.ViewModels;
using LocalServicesBooking.Services.Interfaces;

namespace LocalServicesBooking.Services
{
    public class MessagingService : IMessagingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessagingService(ApplicationDbContext context, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<List<ConversationListViewModel>> GetConversationsAsync(int userId)
        {
            var bookings = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Provider).ThenInclude(p => p.User)
                .Include(b => b.Service)
                .Include(b => b.Messages)
                .Where(b => b.CustomerId == userId || (b.Provider != null && b.Provider.UserId == userId))
                .OrderByDescending(b => b.UpdatedAt)
                .ToListAsync();

            return bookings.Select(b => new ConversationListViewModel
            {
                BookingId = b.BookingId,
                ServiceName = b.Service?.ServiceName ?? "Unknown Service",
                OtherPartyName = b.CustomerId == userId 
                    ? (b.Provider?.BusinessName ?? $"{b.Provider?.User?.FirstName} {b.Provider?.User?.LastName}")
                    : $"{b.Customer?.FirstName} {b.Customer?.LastName}",
                OtherPartyImage = b.CustomerId == userId
                    ? b.Provider?.User?.ProfileImageUrl
                    : b.Customer?.ProfileImageUrl,
                LastMessage = b.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault()?.MessageText ?? "No messages yet",
                LastMessageDate = b.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault()?.SentAt ?? b.CreatedAt
            }).ToList();
        }

        public async Task<ChatViewModel?> GetConversationAsync(int bookingId, int currentUserId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Messages)
                .Include(b => b.Customer)
                .Include(b => b.Provider).ThenInclude(p => p.User)
                .Include(b => b.Service)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null) return null;
            if (booking.Customer == null || booking.Provider == null) return null; // Safety check
            
            if (booking.CustomerId != currentUserId && booking.Provider.UserId != currentUserId) return null;

            var otherPartyName = booking.CustomerId == currentUserId
                ? (booking.Provider.BusinessName ?? $"{booking.Provider.User?.FirstName} {booking.Provider.User?.LastName}")
                : $"{booking.Customer.FirstName} {booking.Customer.LastName}";

            var otherPartyId = booking.CustomerId == currentUserId 
                ? booking.Provider.UserId 
                : booking.CustomerId;

            return new ChatViewModel
            {
                BookingId = booking.BookingId,
                OtherPartyName = otherPartyName,
                OtherPartyId = otherPartyId,
                ServiceName = booking.Service?.ServiceName ?? "Service",
                Messages = booking.Messages.OrderBy(m => m.SentAt).Select(m => new MessageViewModel
                {
                    MessageId = m.MessageId,
                    Text = m.MessageText,
                    SenderId = m.SenderId,
                    SentAt = m.SentAt,
                    IsMine = m.SenderId == currentUserId,
                    SenderName = m.SenderId == currentUserId ? "Me" : otherPartyName
                }).ToList()
            };
        }

        public async Task SendMessageAsync(int bookingId, int senderId, string messageText)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null) return;
            
            var fullBooking = await _context.Bookings
                .Include(b => b.Provider)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
                
            if(fullBooking == null) return;
            if (fullBooking.Provider == null) return; // Cannot rely on provider if null

            var receiverId = fullBooking.CustomerId == senderId ? fullBooking.Provider.UserId : fullBooking.CustomerId;

            var message = new Message
            {
                BookingId = bookingId,
                SenderId = senderId,
                ReceiverId = receiverId,
                MessageText = messageText,
                SentAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            var senderUser = await _context.Users.FindAsync(senderId);
            var senderName = senderUser?.FirstName ?? "User";

            await _hubContext.Clients.Group($"Booking_{bookingId}")
                 .SendAsync("ReceiveMessage", senderName, messageText, DateTime.UtcNow.ToString("t"));
        }
    }
}
