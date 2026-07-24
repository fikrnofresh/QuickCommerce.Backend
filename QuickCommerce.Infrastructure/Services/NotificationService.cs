using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.Entities;
using QuickCommerce.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Services
{
    public class NotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateNotificationAsync(
            string title,
            string message,
            string type,
            List<int> userIds)
        {
            var notification = new Notification
            {
                Title = title,
                Message = message,
                Type = type,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            foreach (var userId in userIds)
            {
                var recipient = new NotificationRecipient
                {
                    NotificationId = notification.Id,
                    UserId = userId
                };

                _context.NotificationRecipients.Add(recipient);
            }

            await _context.SaveChangesAsync();

            return notification.Id;
        }
    }
}