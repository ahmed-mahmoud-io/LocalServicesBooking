using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LocalServicesBooking.Models.Entities;
using LocalServicesBooking.Models.ViewModels;
using LocalServicesBooking.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using LocalServicesBooking.Data;

namespace LocalServicesBooking.Controllers
{
    [Authorize]
    public class MessagingController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IMessagingService _messagingService;
        private readonly ApplicationDbContext _context; // Direct access needed for now until Service is improved?
        // Wait, original code didn't use _context directly in Actions except via Service. 
        // But the previous implementation (MessagingService) does fetch data.
        // My fix for ConversationListViewModel construction relied on `bookings` variable which came from `_messagingService.GetConversationsAsync`?
        // NO. `GetConversationsAsync` returns `List<ConversationListViewModel>`.
        // The failed replacement was trying to put logic INTO the controller that belongs in the SERVICE.
        // My previous `replace_file_content` attempt was trying to replace logic in `GetConversationsAsync` INSIDE `MessagingService.cs`?
        // NO, it was targeting `MessagingController.cs`. 
        // Wait, `MessagingController.cs` calls `_messagingService.GetConversationsAsync`.
        // The error `Dereference of a possibly null reference` was likely in `MessagingService.cs` or `MessagingController.cs` (if it did mapping).
        // Let's look at `MessagingController.cs` content again.
        // It calls `_messagingService.GetConversationsAsync(user.Id)`.
        // So the NullReference is in `MessagingService.cs`!
        // I was trying to fix the wrong file with the wrong content!
        // The `replace_file_content` failed because the code I was trying to match (mapping logic) IS NOT IN `MessagingController.cs`.
        // It is in `MessagingService.cs`.
        
        public MessagingController(UserManager<User> userManager, IMessagingService messagingService)
        {
            _userManager = userManager;
            _messagingService = messagingService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var conversations = await _messagingService.GetConversationsAsync(user.Id);
            return View(conversations);
        }

        public async Task<IActionResult> Conversation(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var conversation = await _messagingService.GetConversationAsync(id, user.Id);
            if (conversation == null) return NotFound(); 

            return View(conversation);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            await _messagingService.SendMessageAsync(dto.BookingId, user.Id, dto.Message);
            return Ok();
        }
    }
}
