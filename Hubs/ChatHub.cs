using Microsoft.AspNetCore.SignalR;

namespace LocalServicesBooking.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(int bookingId, string user, string message)
        {
            // In a real scenario, you'd save to DB here via a Service or Controller call,
            // or just broadcast and let the client handle UX.
            // For simple implementation, we assume the API call saved it and this just notifies.
            
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
    }
}
