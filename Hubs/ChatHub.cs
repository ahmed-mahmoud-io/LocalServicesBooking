using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace LocalServicesBooking.Hubs
{
    public class ChatHub : Hub
    {
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

        // For Community Chat
        public async Task SendCommunityMessage(string displayName, string message)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            await Clients.All.SendAsync("ReceiveCommunityMessage", userId, displayName, message, DateTime.UtcNow.ToString("g"));
        }

        public async Task JoinCommunityChat()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "CommunityChat");
        }

        public async Task LeaveCommunityChat()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "CommunityChat");
        }
    }
}
