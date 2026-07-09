using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.DTOs;
using QuickCommerce.Core.Entities;
using System.Security.Claims;

[ApiController]
[Route("api/v1/inventory")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryRepository _inventoryRepository;

    public InventoryController(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    [HttpGet("store/{storeId}")]
    public async Task<IActionResult> GetStoreInventory(int storeId)
    {
        var data = await _inventoryRepository.GetStoreInventoryAsync(storeId);
        return Ok(data);
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStock()
    {
        var data = await _inventoryRepository.GetLowStockAsync();
        return Ok(data);
    }

    [HttpGet("out-of-stock")]
    public async Task<IActionResult> GetOutOfStock()
    {
        var data = await _inventoryRepository.GetOutOfStockAsync();
        return Ok(data);
    }

    [HttpGet("history/{storeProductId}")]
    public async Task<IActionResult> GetHistory(int storeProductId)
    {
        var data = await _inventoryRepository.GetHistoryAsync(storeProductId);
        return Ok(data);
    }

    [HttpPost("adjustment")]
    public async Task<IActionResult> AdjustStock([FromBody] InventoryAdjustmentDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _inventoryRepository.AdjustStockAsync(
            dto.StoreProductId,
            dto.QuantityChange,
            dto.Reason,
            userId
        );

        return Ok("Stock adjusted");
    }
}