using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCommerce.Core.Entities;
using QuickCommerce.Core.Interfaces;

namespace QuickCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            var created = await _categoryRepository.AddAsync(category);
            return Ok(created);
        }

        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Category updated)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
                return NotFound();

            category.Name = updated.Name;
            category.Description = updated.Description;
            category.ImageUrl = updated.ImageUrl;
            category.DisplayOrder = updated.DisplayOrder;
            category.IsActive = updated.IsActive;

            await _categoryRepository.UpdateAsync(category);

            return Ok(category);
        }

        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> Activate(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            category.IsActive = true;

            await _categoryRepository.UpdateAsync(category);

            return Ok(category);
        }

        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            category.IsActive = false;

            await _categoryRepository.UpdateAsync(category);

            return Ok(category);
        }

        [Authorize(Roles = "SUPER_ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
                return NotFound();

            await _categoryRepository.DeleteAsync(category);

            return Ok("Category deleted");
        }
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [HttpPatch("{id}/reorder")]
        public async Task<IActionResult> Reorder(int id, int displayOrder)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
                return NotFound();

            category.DisplayOrder = displayOrder;

            await _categoryRepository.UpdateAsync(category);

            return Ok(category);
        }
    }

}