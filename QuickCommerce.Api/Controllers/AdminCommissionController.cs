using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.DTOs.Commission;
using QuickCommerce.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/admin/commission")]
    [Authorize]
    public class AdminCommissionController : ControllerBase
    {
        private readonly ICommissionService _commissionService;

        public AdminCommissionController(ICommissionService commissionService)
        {
            _commissionService = commissionService;
        }

        // =============================================
        // CREATE COMMISSION RULE
        // =============================================

        [HttpPost]
        public async Task<IActionResult> CreateRule([FromBody] CreateCategoryCommissionDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _commissionService.CreateRuleAsync(dto);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error creating commission rule",
                    error = ex.Message
                });
            }
        }

        // =============================================
        // GET ALL COMMISSION RULES
        // =============================================

        [HttpGet]
        public async Task<IActionResult> GetAllRules()
        {
            try
            {
                var result = await _commissionService.GetAllRulesAsync();

                if (result == null)
                    return Ok(new List<CategoryCommissionDto>());

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error loading commission rules",
                    error = ex.Message
                });
            }
        }

        // =============================================
        // UPDATE COMMISSION RULE
        // =============================================

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRule(int id, [FromBody] CreateCategoryCommissionDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _commissionService.UpdateRuleAsync(id, dto);

                if (result == null)
                    return NotFound(new { message = "Commission rule not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error updating commission rule",
                    error = ex.Message
                });
            }
        }
    }
}