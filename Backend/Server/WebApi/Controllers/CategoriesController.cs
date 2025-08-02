using Core.Contracts;
using Core.Entities;
using Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebApi.Dtos;
using WebApi.Mappings;
using static WebApi.Dtos.CategoryDto;


namespace WebApi.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]

    public class CategoriesController(IUnitOfWork uow, ILogger<ItemsController> logger) : BaseController(uow, logger)
    {


        [HttpGet]
        [ProducesResponseType(typeof(Category[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            ICollection<Category> categories = await _uow.CategoryRepository.GetAllAsync();
            return Ok(categories);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Category), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            Category? category = await _uow.CategoryRepository.GetByIdAsync(id);

            if (category == null)
                return NotFound(new { error = $"Keine Kategorie mit der ID {id} gefunden." });

            return Ok(category);
        }



        [HttpPost]
        [Authorize(Roles = nameof(Roles.Admin))]
        [ProducesResponseType(typeof(Category), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostByAdmin([FromBody] CategoryPostDto categoryDto)
        {
            Category categoryToPost = categoryDto.ToEntity();

            _uow.CategoryRepository.Insert(categoryToPost);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = categoryToPost.Id }, categoryToPost);
        }



        [HttpPut("{id}")]
        [Authorize(Roles = nameof(Roles.Admin))]
        [ProducesResponseType(typeof(Category), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutByAdmin(int id, [FromBody] CategoryPutDto categoryDto)
        {
            if (id != categoryDto.Id)
                return BadRequest(new { error = "Die ID in der URL stimmt nicht mit der ID im Body überein." });

            Category? categoryToPut = await _uow.CategoryRepository.GetByIdAsync(id);
            if (categoryToPut == null)
                return NotFound(new { error = $"Keine Kategorie mit der ID {categoryDto.Id} gefunden." });

            categoryDto.UpdateEntity(categoryToPut);

            _uow.CategoryRepository.Update(categoryToPut);
            await _uow.SaveChangesAsync();
            return Ok(categoryToPut);
        }




        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(Roles.Admin))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteByAdmin(int id)
        {
            Category? categoryToRemove = await _uow.CategoryRepository.GetByIdAsync(id);
            if (categoryToRemove == null)
                return NotFound(new { error = $"Keine Kategorie mit der ID {id} gefunden." });

            _uow.CategoryRepository.SoftDelete(id);
            await _uow.SaveChangesAsync(); 
            return NoContent();
        }




    }
}
