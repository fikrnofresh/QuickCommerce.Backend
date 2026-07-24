using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.DTOs.Commission;
using QuickCommerce.Core.Entities;
using QuickCommerce.Core.Interfaces;
using QuickCommerce.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Services
{
    public class CommissionService : ICommissionService
    {
        private readonly ApplicationDbContext _context;

        public CommissionService(ApplicationDbContext context)
        {
            _context = context;
        }

        /* =========================================================
           CREATE COMMISSION RULE
        ========================================================== */

        public async Task<CategoryCommissionDto> CreateRuleAsync(CreateCategoryCommissionDto dto)
        {
            // Validate category exists
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == dto.CategoryId);

            if (!categoryExists)
                throw new System.Exception("Invalid category.");

            // Prevent duplicate rule
            var existing = await _context.CategoryCommissionRules
                .FirstOrDefaultAsync(c => c.CategoryId == dto.CategoryId);

            if (existing != null)
                throw new System.Exception("Commission rule already exists for this category.");

            var rule = new CategoryCommissionRule
            {
                CategoryId = dto.CategoryId,
                CommissionPercent = dto.CommissionPercent,
                MinimumCommissionPerOrder = dto.MinimumCommissionPerOrder,
                IsActive = true
            };

            _context.CategoryCommissionRules.Add(rule);

            await _context.SaveChangesAsync();

            return new CategoryCommissionDto
            {
                Id = rule.Id,
                CategoryId = rule.CategoryId,
                CommissionPercent = rule.CommissionPercent,
                MinimumCommissionPerOrder = rule.MinimumCommissionPerOrder,
                IsActive = rule.IsActive
            };
        }

        /* =========================================================
           GET ALL COMMISSION RULES
        ========================================================== */

        public async Task<IEnumerable<CategoryCommissionDto>> GetAllRulesAsync()
        {
            var rules = await _context.CategoryCommissionRules
                .AsNoTracking()
                .Select(r => new CategoryCommissionDto
                {
                    Id = r.Id,
                    CategoryId = r.CategoryId,
                    CommissionPercent = r.CommissionPercent,
                    MinimumCommissionPerOrder = r.MinimumCommissionPerOrder,
                    IsActive = r.IsActive
                })
                .ToListAsync();

            return rules;
        }

        /* =========================================================
           UPDATE COMMISSION RULE
        ========================================================== */

        public async Task<CategoryCommissionDto?> UpdateRuleAsync(int id, CreateCategoryCommissionDto dto)
        {
            var rule = await _context.CategoryCommissionRules
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rule == null)
                return null;

            // Update fields
            rule.CommissionPercent = dto.CommissionPercent;
            rule.MinimumCommissionPerOrder = dto.MinimumCommissionPerOrder;

            await _context.SaveChangesAsync();

            return new CategoryCommissionDto
            {
                Id = rule.Id,
                CategoryId = rule.CategoryId,
                CommissionPercent = rule.CommissionPercent,
                MinimumCommissionPerOrder = rule.MinimumCommissionPerOrder,
                IsActive = rule.IsActive
            };
        }
    }
}