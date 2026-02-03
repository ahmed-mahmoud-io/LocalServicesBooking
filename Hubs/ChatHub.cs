using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using LocalServicesBooking.Data;
using LocalServicesBooking.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocalServicesBooking.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private static int _onlineCount = 0;
        private static readonly object _lock = new object();

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        // For booking-specific conversations
        public async Task SendMessage(int bookingId, string user, string message)
        {
            await Clients.Group($"Booking_{bookingId}")
                .SendAsync("ReceiveMessage", user, message, DateTime.UtcNow.ToString("t"));
        }
        
        public async Task JoinBookingConversation(int bookingId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Booking_{bookingId}");
        }
        
        public async Task LeaveBookingConversation(int bookingId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Booking_{bookingId}");
        }

        // Community Chat
        public async Task SendCommunityMessage(string displayName, string message, string? imageUrl = null)
        {
            var userIdStr = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                await Clients.Caller.SendAsync("Error", "You must be logged in to send messages.");
                return;
            }

            // Save to database
            var chatMessage = new ChatMessage
            {
                UserId = userId,
                Text = message,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.UtcNow
            };

            _context.ChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync();

            // Get user info for avatar
            var user = await _context.Users.FindAsync(userId);
            var avatarUrl = user?.ProfileImageUrl;
            if (string.IsNullOrEmpty(avatarUrl))
            {
                avatarUrl = user?.Gender == "Male" ? "/images/defaults/default_male.png" :
                           user?.Gender == "Female" ? "/images/defaults/default_female.png" :
                           "/images/defaults/default_user.png";
            }

            await Clients.All.SendAsync("ReceiveCommunityMessage", 
                chatMessage.MessageId,
                userId.ToString(), 
                displayName, 
                message, 
                imageUrl,
                avatarUrl,
                DateTime.UtcNow.ToString("g"));
        }

        public async Task JoinCommunityChat()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "CommunityChat");
            
            lock (_lock)
            {
                _onlineCount++;
            }
            
            await Clients.All.SendAsync("UpdateOnlineCount", _onlineCount);
        }

        public async Task LeaveCommunityChat()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "CommunityChat");
            
            lock (_lock)
            {
                _onlineCount = Math.Max(0, _onlineCount - 1);
            }
            
            await Clients.All.SendAsync("UpdateOnlineCount", _onlineCount);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            lock (_lock)
            {
                _onlineCount = Math.Max(0, _onlineCount - 1);
            }
            
            await Clients.All.SendAsync("UpdateOnlineCount", _onlineCount);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task<List<object>> GetRecentMessages(int count = 50)
        {
            var messages = await _context.ChatMessages
                .Include(m => m.User)
                .Where(m => !m.IsDeleted)
                .OrderByDescending(m => m.CreatedAt)
                .Take(count)
                .Select(m => new
                {
                    m.MessageId,
                    UserId = m.UserId.ToString(),
                    DisplayName = m.User != null ? $"{m.User.FirstName} {m.User.LastName}" : "Unknown",
                    m.Text,
                    m.ImageUrl,
                    AvatarUrl = m.User != null ? 
                        (!string.IsNullOrEmpty(m.User.ProfileImageUrl) ? m.User.ProfileImageUrl :
                         m.User.Gender == "Male" ? "/images/defaults/default_male.png" :
                         m.User.Gender == "Female" ? "/images/defaults/default_female.png" :
                         "/images/defaults/default_user.png") : "/images/defaults/default_user.png",
                    Time = m.CreatedAt.ToString("g")
                })
                .ToListAsync();

            return messages.Cast<object>().Reverse().ToList();
        }
    }
}
