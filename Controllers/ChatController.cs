using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalServicesBooking.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ChatController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest(new { error = "No image provided" });

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                return BadRequest(new { error = "Invalid file type. Only JPG, PNG, and GIF are allowed." });

            // Validate file size (max 5MB)
            if (image.Length > 5 * 1024 * 1024)
                return BadRequest(new { error = "File size exceeds 5MB limit." });

            try
            {
                var uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "chat");
                if (!Directory.Exists(uploadsDir))
                    Directory.CreateDirectory(uploadsDir);

                var fileName = $"{Guid.NewGuid():N}{extension}";
                var filePath = Path.Combine(uploadsDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                var imageUrl = $"/uploads/chat/{fileName}";
                return Ok(new { url = imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to upload image", details = ex.Message });
            }
        }
    }
}
