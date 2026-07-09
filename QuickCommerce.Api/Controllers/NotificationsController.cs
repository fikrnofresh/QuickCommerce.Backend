using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using QuickCommerce.Api.Hubs;
using QuickCommerce.Infrastructure.Data;
using QuickCommerce.Core.Entities;

namespace QuickCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hub;

        public NotificationsController(
            ApplicationDbContext context,
            IHubContext<NotificationHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        // ==================================
        // GET USER NOTIFICATIONS
        // ==================================
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserNotifications(int userId)
        {
            var notifications = await _context.NotificationRecipients
                .Include(x => x.Notification)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Notification.CreatedAt)
                .ToListAsync();

            return Ok(notifications);
        }

        // ==================================
        // MARK NOTIFICATION AS READ
        // ==================================
        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.NotificationRecipients.FindAsync(id);

            if (notification == null)
                return NotFound();

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok();
        }

        // ==================================
        // SEND TEST NOTIFICATION (REALTIME)
        // ==================================
        [HttpPost("test/{userId}")]
        public async Task<IActionResult> SendTestNotification(int userId)
        {
            var notification = new Notification
            {
                Title = "Test Notification",
                Message = "This is a realtime test notification",
                Type = "TEST",
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            var recipient = new NotificationRecipient
            {
                NotificationId = notification.Id,
                UserId = userId
            };

            _context.NotificationRecipients.Add(recipient);
            await _context.SaveChangesAsync();

            // REALTIME PUSH USING SIGNALR
            await _hub.Clients
                .Group($"user_{userId}")
                .SendAsync("ReceiveNotification", new
                {
                    title = notification.Title,
                    message = notification.Message,
                    type = notification.Type
                });

            return Ok(new
            {
                message = "Notification sent successfully"
            });
        }
    }
}