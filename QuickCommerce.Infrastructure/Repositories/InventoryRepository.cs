using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.Entities;
using QuickCommerce.Core.Interfaces;
using QuickCommerce.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class InventoryRepository : IInventoryRepository
{
    private readonly ApplicationDbContext _context;

    public InventoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<StoreProduct>> GetStoreInventoryAsync(int storeId)
    {
        return await _context.StoreProducts
            .Include(p => p.Product)
            .Where(p => p.StoreId == storeId)
            .ToListAsync();
    }

    public async Task<IEnumerable<StoreProduct>> GetLowStockAsync()
    {
        return await _context.StoreProducts
            .Where(p => p.StockQuantity <= p.LowStockThreshold)
            .ToListAsync();
    }

    public async Task<IEnumerable<StoreProduct>> GetOutOfStockAsync()
    {
        return await _context.StoreProducts
            .Where(p => p.StockQuantity == 0)
            .ToListAsync();
    }

    public async Task<IEnumerable<InventoryMovement>> GetHistoryAsync(int storeProductId)
    {
        return await _context.InventoryMovements
            .Where(m => m.StoreProductId == storeProductId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task AdjustStockAsync(int storeProductId, int quantityChange, string reason, int userId)
    {
        var storeProduct = await _context.StoreProducts.FindAsync(storeProductId);

        if (storeProduct == null)
            throw new Exception("StoreProduct not found");

        storeProduct.StockQuantity += quantityChange;

        var movement = new InventoryMovement
        {
            StoreProductId = storeProductId,
            StoreId = storeProduct.StoreId,
            ProductId = storeProduct.ProductId,
            QuantityChanged = quantityChange,
            Reason = reason,
            PerformedByUserId = userId
        };

        _context.InventoryMovements.Add(movement);

        await _context.SaveChangesAsync();
    }
}