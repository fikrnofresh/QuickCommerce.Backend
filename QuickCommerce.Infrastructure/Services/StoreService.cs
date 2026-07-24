using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.DTOs.Store;
using QuickCommerce.Core.Entities;
using QuickCommerce.Core.Enums;
using QuickCommerce.Core.Interfaces;
using QuickCommerce.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Services
{
    public class StoreService : IStoreService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StoreService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // =========================
        // 🔐 Scope Helpers
        // =========================

        private string? GetRoleScope()
        {
            return _httpContextAccessor.HttpContext?
                .User?
                .FindFirst("roleScope")?
                .Value;
        }

        private int? GetTokenStoreId()
        {
            var claim = _httpContextAccessor.HttpContext?
                .User?
                .FindFirst("storeId")?
                .Value;

            if (int.TryParse(claim, out var storeId))
                return storeId;

            return null;
        }

        private void EnforceStoreScope(int targetStoreId)
        {
            var roleScope = GetRoleScope();

            if (roleScope == "PLATFORM")
                return;

            var tokenStoreId = GetTokenStoreId();

            if (tokenStoreId == null || tokenStoreId != targetStoreId)
                throw new UnauthorizedAccessException(
                    "You are not allowed to access this store.");
        }

        // =========================
        // CREATE STORE
        // =========================
        public async Task<StoreDto> CreateStoreAsync(CreateStoreDto dto)
        {
            if (GetRoleScope() != "PLATFORM")
                throw new UnauthorizedAccessException(
                    "Only platform admin can create stores.");

            if (await _context.Stores.AnyAsync(s => s.Code == dto.Code))
                throw new Exception("Store code already exists.");

            var store = new Store
            {
                Name = dto.Name,
                Code = dto.Code,
                City = dto.City,
                Area = dto.Area,
                State = dto.State,
                Pincode = dto.Pincode,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                OpeningTime = dto.OpeningTime,
                ClosingTime = dto.ClosingTime,
                IsActive = dto.IsActive,
                IsOnline = dto.IsOnline,
                Status = StoreStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            _context.Stores.Add(store);
            await _context.SaveChangesAsync();

            return MapToDto(store);
        }

        // =========================
        // GET BY ID
        // =========================
        public async Task<StoreDto?> GetStoreByIdAsync(int id)
        {
            EnforceStoreScope(id);

            var store = await _context.Stores
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            return store == null ? null : MapToDto(store);
        }

        // =========================
        // GET ALL
        // =========================
        public async Task<IEnumerable<StoreDto>> GetAllStoresAsync()
        {
            var roleScope = GetRoleScope();

            if (roleScope == "PLATFORM")
            {
                var allStores = await _context.Stores
                    .AsNoTracking()
                    .OrderByDescending(s => s.CreatedAt)
                    .ToListAsync();

                return allStores.Select(MapToDto);
            }

            var tokenStoreId = GetTokenStoreId();

            if (tokenStoreId == null)
                throw new UnauthorizedAccessException(
                    "Store mapping not found.");

            var store = await _context.Stores
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == tokenStoreId);

            if (store == null)
                return Enumerable.Empty<StoreDto>();

            return new List<StoreDto> { MapToDto(store) };
        }

        // =========================
        // UPDATE
        // =========================
        public async Task<StoreDto> UpdateStoreAsync(int id, UpdateStoreDto dto)
        {
            EnforceStoreScope(id);

            var store = await _context.Stores.FindAsync(id);
            if (store == null)
                throw new Exception("Store not found.");

            store.Name = dto.Name;
            store.City = dto.City;
            store.Area = dto.Area;
            store.State = dto.State;
            store.Pincode = dto.Pincode;
            store.Latitude = dto.Latitude;
            store.Longitude = dto.Longitude;
            store.PhoneNumber = dto.PhoneNumber;
            store.Email = dto.Email;
            store.OpeningTime = dto.OpeningTime;
            store.ClosingTime = dto.ClosingTime;
            store.IsActive = dto.IsActive;
            store.IsOnline = dto.IsOnline;
            store.IsVerified = dto.IsVerified;
            store.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToDto(store);
        }

        // =========================
        // SUSPEND STORE
        // =========================
        public async Task SuspendStoreAsync(int storeId)
        {
            if (GetRoleScope() != "PLATFORM")
                throw new UnauthorizedAccessException(
                    "Only platform admin can suspend stores.");

            var store = await _context.Stores.FindAsync(storeId);

            if (store == null)
                throw new Exception("Store not found.");

            if (store.Status == StoreStatus.Suspended)
                throw new Exception("Store already suspended.");

            if (store.Status == StoreStatus.Closed)
                throw new Exception("Closed store cannot be suspended.");

            store.Status = StoreStatus.Suspended;
            store.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        // =========================
        // BLOCK STORE
        // =========================
        public async Task BlockStoreAsync(int storeId)
        {
            if (GetRoleScope() != "PLATFORM")
                throw new UnauthorizedAccessException(
                    "Only platform admin can block stores.");

            var store = await _context.Stores.FindAsync(storeId);

            if (store == null)
                throw new Exception("Store not found.");

            if (store.Status == StoreStatus.Blocked)
                throw new Exception("Store already blocked.");

            if (store.Status == StoreStatus.Closed)
                throw new Exception("Closed store cannot be blocked.");

            store.Status = StoreStatus.Blocked;
            store.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }


        // =========================
        // CLOSE STORE
        // =========================
        public async Task CloseStoreAsync(int storeId)
        {
            if (GetRoleScope() != "PLATFORM")
                throw new UnauthorizedAccessException(
                    "Only platform admin can close stores.");

            var store = await _context.Stores.FindAsync(storeId);

            if (store == null)
                throw new Exception("Store not found.");

            if (store.Status == StoreStatus.Closed)
                throw new Exception("Store already closed.");

            store.Status = StoreStatus.Closed;
            store.IsActive = false;
            store.IsOnline = false;
            store.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        // =========================
        // ACTIVATE STORE
        // =========================
        public async Task ActivateStoreAsync(int storeId)
        {
            if (GetRoleScope() != "PLATFORM")
                throw new UnauthorizedAccessException(
                    "Only platform admin can activate stores.");

            var store = await _context.Stores.FindAsync(storeId);

            if (store == null)
                throw new Exception("Store not found.");

            // Closed store cannot be reactivated
            if (store.Status == StoreStatus.Closed)
                throw new Exception("Closed stores cannot be reactivated. Create a new store instead.");

            if (store.Status == StoreStatus.Active)
                throw new Exception("Store is already active.");

            store.Status = StoreStatus.Active;
            store.IsActive = true;
            store.IsOnline = true;
            store.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        // =========================
        // DEACTIVATE STORE
        // =========================
        public async Task DeactivateStoreAsync(int storeId)
        {
            if (GetRoleScope() != "PLATFORM")
                throw new UnauthorizedAccessException(
                    "Only platform admin can deactivate stores.");

            var store = await _context.Stores.FindAsync(storeId);

            if (store == null)
                throw new Exception("Store not found.");

            if (!store.IsActive)
                throw new Exception("Store is already inactive.");

            store.IsActive = false;
            store.IsOnline = false;

            if (store.Status == StoreStatus.Active)
                store.Status = StoreStatus.Suspended;

            store.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
        // =========================
        // STORE ONLINE
        // =========================
        public async Task SetStoreOnlineAsync(int storeId)
        {
            var store = await _context.Stores.FindAsync(storeId);

            if (store == null)
                throw new Exception("Store not found.");

            if (store.Status != StoreStatus.Active)
                throw new Exception("Only active stores can go online.");

            if (!store.IsVerified)
                throw new Exception("Store must be verified before going online.");

            store.IsOnline = true;
            store.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        // =========================
        // STORE OFFLINE
        // =========================
        public async Task SetStoreOfflineAsync(int storeId)
        {
            var store = await _context.Stores.FindAsync(storeId);

            if (store == null)
                throw new Exception("Store not found.");

            store.IsOnline = false;
            store.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        // =========================
        // VERIFY STORE
        // =========================
        public async Task VerifyStoreAsync(int storeId)
        {
            if (GetRoleScope() != "PLATFORM")
                throw new UnauthorizedAccessException("Only platform admin can verify stores.");

            var store = await _context.Stores.FindAsync(storeId);

            if (store == null)
                throw new Exception("Store not found.");

            store.IsVerified = true;
            store.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        // =========================
        // UNVERIFY STORE
        // =========================
        public async Task UnverifyStoreAsync(int storeId)
        {
            if (GetRoleScope() != "PLATFORM")
                throw new UnauthorizedAccessException("Only platform admin can unverify stores.");

            var store = await _context.Stores.FindAsync(storeId);

            if (store == null)
                throw new Exception("Store not found.");

            store.IsVerified = false;
            store.IsOnline = false;
            store.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        // =========================
        // MAPPING
        // =========================
        private static StoreDto MapToDto(Store store)
        {
            return new StoreDto
            {
                Id = store.Id,
                Name = store.Name,
                Code = store.Code,
                City = store.City,
                Area = store.Area,
                State = store.State,
                Pincode = store.Pincode,
                Latitude = store.Latitude,
                Longitude = store.Longitude,
                PhoneNumber = store.PhoneNumber,
                Email = store.Email,
                OpeningTime = store.OpeningTime,
                ClosingTime = store.ClosingTime,
                IsActive = store.IsActive,
                IsOnline = store.IsOnline,
                IsVerified = store.IsVerified,
                CreatedAt = store.CreatedAt
            };
        }
        public async Task<StoreDashboardDto> GetStoreDashboardAsync(int storeId)
        {
            EnforceStoreScope(storeId);

            var today = DateTime.UtcNow.Date;

            var ordersToday = await _context.Orders
                .CountAsync(o => o.StoreId == storeId && o.CreatedAt.Date == today);

            var revenueToday = await _context.Orders
                .Where(o => o.StoreId == storeId && o.CreatedAt.Date == today)
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

            var ordersInProgress = await _context.Orders
                .CountAsync(o =>
                    o.StoreId == storeId &&
                    (o.Status == "PENDING" ||
                     o.Status == "CONFIRMED" ||
                     o.Status == "PREPARING"));

            var pendingOrders = await _context.Orders
                .CountAsync(o => o.StoreId == storeId && o.Status == "PENDING");

            var preparingOrders = await _context.Orders
                .CountAsync(o => o.StoreId == storeId && o.Status == "PREPARING");

            var readyOrders = await _context.Orders
                .CountAsync(o => o.StoreId == storeId && o.Status == "READY_FOR_PICKUP");

            var lowStockProducts = await _context.StoreProducts
                .CountAsync(sp =>
                    sp.StoreId == storeId &&
                    sp.StockQuantity > 0 &&
                    sp.StockQuantity <= sp.LowStockThreshold);

            var outOfStockProducts = await _context.StoreProducts
                .CountAsync(sp =>
                    sp.StoreId == storeId &&
                    sp.StockQuantity == 0);

            return new StoreDashboardDto
            {
                StoreId = storeId,
                RevenueToday = revenueToday,
                OrdersToday = ordersToday,
                OrdersInProgress = ordersInProgress,
                PendingOrders = pendingOrders,
                PreparingOrders = preparingOrders,
                ReadyForPickupOrders = readyOrders,
                LowStockProducts = lowStockProducts,
                OutOfStockProducts = outOfStockProducts
            };
        }
        public async Task<List<StoreInventoryMonitorDto>> GetStoreInventoryMonitorAsync(int storeId)
        {
            EnforceStoreScope(storeId);

            return await _context.StoreProducts
                .Include(sp => sp.Product)
                .Where(sp => sp.StoreId == storeId)
                .OrderBy(sp => sp.StockQuantity)
                .Select(sp => new StoreInventoryMonitorDto
                {
                    StoreId = storeId,
                    ProductId = sp.ProductId,
                    ProductName = sp.Product.Name,
                    StockQuantity = sp.StockQuantity,
                    LowStockThreshold = sp.LowStockThreshold,
                    IsOutOfStock = sp.StockQuantity == 0
                })
                .Take(200)
                .ToListAsync();
        }
    }
}