using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.DTOs.Onboarding;
using QuickCommerce.Core.Entities;
using QuickCommerce.Infrastructure.Data;
using QuickCommerce.Infrastructure.Services; // ✅ ADDED

[ApiController]
[Route("api/v1/onboarding")]
public class OnboardingController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly AuditLogService _auditLogService; // ✅ ADDED

    public OnboardingController(
        ApplicationDbContext context,
        AuditLogService auditLogService) // ✅ UPDATED
    {
        _context = context;
        _auditLogService = auditLogService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOnboardingRequestDto dto)
    {
        var request = new OnboardingRequest
        {
            Type = dto.Type,
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            StoreId = dto.StoreId,
            RequestedRole = dto.RequestedRole,
            RequestedPermissions = dto.RequestedPermissions,

            Status = "PENDING",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.OnboardingRequests.AddAsync(request);
        await _context.SaveChangesAsync();

        // 🔥 AUDIT LOG
        await _auditLogService.LogAsync(
            userId: null,
            module: "ONBOARDING",
            action: "CREATE",
            entityType: "OnboardingRequest",
            entityId: request.Id,
            description: $"Onboarding request created for {request.FullName}"
        );

        return Ok(new
        {
            message = "Onboarding request created",
            id = request.Id
        });
    }

    [HttpPost("approve")]
    public async Task<IActionResult> Approve(ApproveOnboardingDto dto)
    {
        var request = await _context.OnboardingRequests
            .FirstOrDefaultAsync(x => x.Id == dto.RequestId);

        if (request == null)
            return NotFound("Request not found");

        if (request.Status != "PENDING")
            return BadRequest("Already processed");

        var user = new User
        {
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        if (request.StoreId.HasValue)
        {
            await _context.UserStores.AddAsync(new UserStore
            {
                UserId = user.Id,
                StoreId = request.StoreId.Value,
                IsPrimary = true
            });
        }

        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == dto.RoleName);

        if (role == null)
            return BadRequest("Role not found");

        await _context.UserRoles.AddAsync(new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id
        });

        if (!string.IsNullOrEmpty(dto.PermissionsJson))
        {
            var permissions = System.Text.Json.JsonSerializer
                .Deserialize<List<string>>(dto.PermissionsJson);

            if (permissions != null)
            {
                foreach (var permName in permissions)
                {
                    var perm = await _context.Permissions
                        .FirstOrDefaultAsync(p => p.Name == permName);

                    if (perm != null)
                    {
                        await _context.RolePermissions.AddAsync(new RolePermission
                        {
                            RoleId = role.Id,
                            PermissionId = perm.Id
                        });
                    }
                }
            }
        }

        request.Status = "APPROVED";
        request.ApprovedBy = dto.ApprovedBy;
        request.ApprovedAt = DateTime.UtcNow;
        request.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // 🔥 AUDIT LOG
        await _auditLogService.LogAsync(
            userId: null,
            module: "ONBOARDING",
            action: "APPROVE",
            entityType: "User",
            entityId: user.Id,
            description: $"Onboarding approved for {user.FullName}"
        );

        return Ok(new
        {
            message = "Approved successfully",
            userId = user.Id
        });
    }

    [HttpPost("reject")]
    public async Task<IActionResult> Reject(RejectOnboardingDto dto)
    {
        var request = await _context.OnboardingRequests
            .FirstOrDefaultAsync(x => x.Id == dto.RequestId);

        if (request == null)
            return NotFound();

        if (request.Status != "PENDING")
            return BadRequest("Already processed");

        request.Status = "REJECTED";
        request.RejectionReason = dto.Reason;
        request.ApprovedBy = dto.RejectedBy;
        request.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // 🔥 AUDIT LOG
        await _auditLogService.LogAsync(
            userId: null,
            module: "ONBOARDING",
            action: "REJECT",
            entityType: "OnboardingRequest",
            entityId: request.Id,
            description: $"Onboarding rejected: {dto.Reason}"
        );

        return Ok(new { message = "Rejected successfully" });
    }

    // =========================
    // GET ALL (UNCHANGED)
    // =========================
    [HttpGet]
    public async Task<IActionResult> GetAll(
        string? status,
        string? type,
        int? storeId,
        string? search)
    {
        var query = _context.OnboardingRequests.AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(x => x.Status == status);

        if (!string.IsNullOrEmpty(type))
            query = query.Where(x => x.Type == type);

        if (storeId.HasValue)
            query = query.Where(x => x.StoreId == storeId);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(x =>
                x.FullName.Contains(search) ||
                x.PhoneNumber.Contains(search));

        var result = await query
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new
            {
                x.Id,
                x.Type,
                x.FullName,
                x.PhoneNumber,
                x.StoreId,
                x.Status,
                x.CreatedAt,
                x.ApprovedAt
            })
            .ToListAsync();

        return Ok(result);
    }

    // =========================
    // GET BY ID (UNCHANGED)
    // =========================
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var data = await _context.OnboardingRequests
            .Where(x => x.Id == id)
            .Select(x => new
            {
                x.Id,
                x.Type,
                x.Status,
                x.FullName,
                x.PhoneNumber,
                x.Email,
                x.AddressLine1,
                x.City,
                x.State,
                x.AadharNumber,
                x.DrivingLicenseNumber,
                x.VehicleType,
                x.VehicleNumber,
                x.RequestedRole,
                x.RequestedPermissions,
                x.ApprovedBy,
                x.ApprovedAt,
                x.RejectionReason
            })
            .FirstOrDefaultAsync();

        if (data == null)
            return NotFound();

        return Ok(data);
    }
}