using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.DTOs.Customer;
using QuickCommerce.Core.Interfaces;
using QuickCommerce.Infrastructure.Data;
using QuickCommerce.Infrastructure.Services;
using System.Security.Claims;
using QuickCommerce.Core.Entities;


namespace QuickCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/customer")]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _context;
        private readonly CustomerService _customerService;

        public CustomerController(
    IAuthService authService,
    ApplicationDbContext context,
    CustomerService customerService)
        {
            _authService = authService;
            _context = context;
            _customerService = customerService;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst("userId")?.Value!);
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetUserId();

            var profile = await _authService.GetCustomerProfileAsync(userId);

            if (profile == null)
                return NotFound();

            return Ok(profile);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(UpdateCustomerProfileDto dto)
        {
            var userId = GetUserId();

            var result = await _authService.UpdateCustomerProfileAsync(userId, dto);

            if (!result)
                return NotFound();

            return Ok(new { message = "Profile updated successfully" });
        }
        [HttpPost("activity")]
        public async Task<IActionResult> TrackActivity(TrackActivityDto dto)
        {
            var userId = int.Parse(User.FindFirst("userId")!.Value);
            
            await _authService.TrackCustomerActivityAsync(userId, dto);

            return Ok(new { message = "Activity tracked" });
        }

        [HttpGet("segment")]
        public async Task<IActionResult> GetSegment()
        {
            var userId = int.Parse(User.FindFirst("userId")!.Value);

            var segment = await _authService.GetCustomerSegmentAsync(userId);

            return Ok(new { segment });
        }
        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = int.Parse(User.FindFirst("userId")!.Value);

            var notifications = await _context.NotificationRecipients
                .Where(nr => nr.UserId == userId)
                .Select(nr => new
                {
                    nr.NotificationId,
                    nr.IsRead,
                    nr.Notification.Title,
                    nr.Notification.Message,
                    nr.Notification.Type,
                    nr.Notification.CreatedAt
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Ok(notifications);
        }
        [HttpPatch("notifications/{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = int.Parse(User.FindFirst("userId")!.Value);

            var record = await _context.NotificationRecipients
                .FirstOrDefaultAsync(x => x.NotificationId == id && x.UserId == userId);

            if (record == null)
                return NotFound();

            record.IsRead = true;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Marked as read" });
        }
        [HttpGet("wallet")]
        public async Task<IActionResult> GetWallet()
        {
            var userId = int.Parse(User.FindFirst("userId")!.Value);

            var wallet = await _customerService.GetWalletAsync(userId);

            return Ok(wallet);
        }
        [HttpGet("wallet-transactions")]
        public async Task<IActionResult> GetWalletTransactions()
        {
            var userId = int.Parse(User.FindFirst("userId")!.Value);

            var transactions = await _context.WalletTransactions
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Ok(transactions);
        }
        [HttpPost("support")]
        public async Task<IActionResult> CreateSupport([FromBody] SupportTicket request)
        {
            var userId = int.Parse(User.FindFirst("userId")!.Value);

            var result = await _customerService.CreateTicketAsync(
                userId,
                request.Subject,
                request.Message
            );

            return Ok(result);
        }
        [HttpGet("support")]
        public async Task<IActionResult> GetSupport()
        {
            var userId = int.Parse(User.FindFirst("userId")!.Value);

            var tickets = await _customerService.GetTicketsAsync(userId);

            return Ok(tickets);
        }
        [HttpGet("recommendations")]
        public async Task<IActionResult> GetRecommendations()
        {
            var userId = int.Parse(User.FindFirst("userId")!.Value);

            var result = await _customerService.GetRecommendationsAsync(userId);

            return Ok(result);
        }
        [HttpGet("reorder-recommendations")]
        public async Task<IActionResult> Reorder()
        {
            var userId = int.Parse(User.FindFirst("userId").Value);
            return Ok(await _customerService.GetReorderRecommendations(userId));
        }

        [HttpGet("bundle-recommendations/{productId}")]
        public async Task<IActionResult> Bundle(int productId)
        {
            return Ok(await _customerService.GetFrequentlyBoughtTogether(productId));
        }

        [HttpGet("search-suggestions")]
        public async Task<IActionResult> Search(string q)
        {
            var userId = int.Parse(User.FindFirst("userId").Value);
            return Ok(await _customerService.GetSearchSuggestions(q, userId));
        }

        [HttpGet("smart-recommendations")]
        public async Task<IActionResult> Smart()
        {
            var userId = int.Parse(User.FindFirst("userId").Value);
            return Ok(await _customerService.GetSmartRecommendations(userId));
        }
    }
}