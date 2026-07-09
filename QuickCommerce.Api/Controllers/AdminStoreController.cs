using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.DTOs.Store;
using QuickCommerce.Core.Interfaces;
using System.Threading.Tasks;

namespace QuickCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/admin/stores")]
    public class AdminStoreController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public AdminStoreController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        // =========================
        // CREATE STORE
        // =========================
        [Authorize(Policy = "STORE.CREATE")]
        [HttpPost]
        public async Task<IActionResult> CreateStore([FromBody] CreateStoreDto dto)
        {
            var result = await _storeService.CreateStoreAsync(dto);
            return Ok(result);
        }

        // =========================
        // GET ALL STORES
        // =========================
        [Authorize(Policy = "STORE.VIEW")]
        [HttpGet]
        public async Task<IActionResult> GetAllStores()
        {
            var result = await _storeService.GetAllStoresAsync();
            return Ok(result);
        }

        // =========================
        // GET STORE BY ID
        // =========================
        [Authorize(Policy = "STORE.VIEW")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStoreById(int id)
        {
            var result = await _storeService.GetStoreByIdAsync(id);

            if (result == null)
                return NotFound("Store not found.");

            return Ok(result);
        }

        // =========================
        // UPDATE STORE
        // =========================
        [Authorize(Policy = "STORE.UPDATE")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStore(int id, [FromBody] UpdateStoreDto dto)
        {
            var result = await _storeService.UpdateStoreAsync(id, dto);

            if (result == null)
                return NotFound("Store not found.");

            return Ok(result);
        }

        // =========================
        // SUSPEND STORE
        // =========================
        [Authorize(Policy = "STORE.UPDATE")]
        [HttpPatch("{id}/suspend")]
        public async Task<IActionResult> SuspendStore(int id)
        {
            await _storeService.SuspendStoreAsync(id);

            return Ok(new
            {
                message = "Store suspended successfully"
            });
        }
        // =========================
        // BLOCK STORE
        // =========================
        [Authorize(Policy = "STORE.UPDATE")]
        [HttpPatch("{id}/block")]
        public async Task<IActionResult> BlockStore(int id)
        {
            await _storeService.BlockStoreAsync(id);

            return Ok(new
            {
                message = "Store blocked successfully"
            });
        }


        // =========================
        // CLOSE STORE
        // =========================
        [Authorize(Policy = "STORE.UPDATE")]
        [HttpPatch("{id}/close")]
        public async Task<IActionResult> CloseStore(int id)
        {
            await _storeService.CloseStoreAsync(id);

            return Ok(new
            {
                message = "Store closed permanently"
            });
        }
        // =========================
        // ACTIVATE STORE
        // =========================
        [Authorize(Policy = "STORE.UPDATE")]
        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> ActivateStore(int id)
        {
            await _storeService.ActivateStoreAsync(id);

            return Ok(new
            {
                message = "Store activated successfully"
            });
        }
        // =========================
        // STORE ONLINE
        // =========================
        [Authorize(Policy = "STORE.UPDATE")]
        [HttpPatch("{id}/online")]
        public async Task<IActionResult> SetStoreOnline(int id)
        {
            await _storeService.SetStoreOnlineAsync(id);

            return Ok(new
            {
                message = "Store is now online"
            });
        }

        // =========================
        // STORE OFFLINE
        // =========================
        [Authorize(Policy = "STORE.UPDATE")]
        [HttpPatch("{id}/offline")]
        public async Task<IActionResult> SetStoreOffline(int id)
        {
            await _storeService.SetStoreOfflineAsync(id);

            return Ok(new
            {
                message = "Store is now offline"
            });
        }

        // =========================
        // VERIFY STORE
        // =========================
        [Authorize(Policy = "STORE.UPDATE")]
        [HttpPatch("{id}/verify")]
        public async Task<IActionResult> VerifyStore(int id)
        {
            await _storeService.VerifyStoreAsync(id);

            return Ok(new
            {
                message = "Store verified successfully"
            });
        }

        // =========================
        // UNVERIFY STORE
        // =========================
        [Authorize(Policy = "STORE.UPDATE")]
        [HttpPatch("{id}/unverify")]
        public async Task<IActionResult> UnverifyStore(int id)
        {
            await _storeService.UnverifyStoreAsync(id);
            
            return Ok(new
            {
                message = "Store verification removed"
            });
        }
        [Authorize(Policy = "STORE.VIEW")]
        [HttpGet("{id}/dashboard")]
        public async Task<IActionResult> GetStoreDashboard(int id)
        {
            var result = await _storeService.GetStoreDashboardAsync(id);

            return Ok(result);
        }
        [Authorize(Policy = "STORE.VIEW")]
        [HttpGet("{id}/inventory-monitor")]
        public async Task<IActionResult> GetInventoryMonitor(int id)
        {
            var result = await _storeService.GetStoreInventoryMonitorAsync(id);

            return Ok(result);
        }
    }
}