using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.DTOs.Customer;
using QuickCommerce.Core.Interfaces.Services;

namespace QuickCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/customer/addresses")]
    [Authorize]
    public class CustomerAddressController : ControllerBase
    {
        private readonly ICustomerAddressService _service;

        public CustomerAddressController(ICustomerAddressService service)
        {
            _service = service;
        }

        private int GetUserId()
        {
            var claim = User.FindFirst("userId");

            if (claim == null)
                throw new UnauthorizedAccessException("UserId claim missing");

            return int.Parse(claim.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetAddresses()
        {
            var userId = GetUserId();

            var result = await _service.GetAddresses(userId);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CustomerAddressDto dto)
        {
            var userId = GetUserId();

            var result = await _service.CreateAddress(userId, dto);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            
            await _service.DeleteAddress(id, userId);

            return Ok();
        }
    }
}