using System.ComponentModel.DataAnnotations;

namespace LocalServicesBooking.Models.ViewModels
{
    public class ConversationListViewModel
    {
        public int BookingId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string OtherPartyName { get; set; } = string.Empty;
        public string? OtherPartyImage { get; set; }
        public string LastMessage { get; set; } = string.Empty;
        public DateTime LastMessageDate { get; set; }
    }

    public class ChatViewModel
    {
        public int BookingId { get; set; }
        public string OtherPartyName { get; set; } = string.Empty;
        public int OtherPartyId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public List<MessageViewModel> Messages { get; set; } = new List<MessageViewModel>();
    }

    public class MessageViewModel
    {
        public int MessageId { get; set; }
        public string Text { get; set; } = string.Empty;
        public int SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsMine { get; set; }
    }

    public class SendMessageDto
    {
        public int BookingId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
