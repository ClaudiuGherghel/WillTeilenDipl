using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Core.Contracts;
using Persistence;
using Core.Entities;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;


namespace WebApi.Controllers
{

    public record CategoryPostDto(
        [Required(AllowEmptyStrings = false, ErrorMessage = "Kategoriename muss eingegeben werden")] string Name
        );
    public record CategoryPutDto(
        int Id,
        byte[]? RowVersion,
        [Required(AllowEmptyStrings = false, ErrorMessage = "Kategoriename muss eingegeben werden")] string Name
        );


    [Route("api/[controller]/[action]")]
    //Kein if (ModelState.IsValid) mehr nötig, bei ApiController passiert das automatisch, (Fehlerausgabe ProblemDetails) 
    [ApiController]

    public class CategoriesController(IUnitOfWork uow) : ControllerBase
    {
    // Bei Fehlern wird Middleware einspringen

        private readonly IUnitOfWork _uow = uow;


        [HttpGet]
        [ProducesResponseType(typeof(Category[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            ICollection<Category> categories = await _uow.CategoryRepository.GetAllAsync();
            return Ok(categories.ToArray());
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Category), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            Category? category = await _uow.CategoryRepository.GetByIdAsync(id);

            if (category != null)
                return Ok(category);
            else
                return NotFound();
        }



        [HttpPost]
        [ProducesResponseType(typeof(Category), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> Post([FromBody] CategoryPostDto categoryDto)
        {
            if (categoryDto == null)
            {
                return BadRequest();
            }

            Category newCategory = new()
            {
                Name = categoryDto.Name,
            };

            _uow.CategoryRepository.Insert(newCategory);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = newCategory.Id }, newCategory);
        }





        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Category), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(int id, [FromBody] CategoryPutDto categoryDto)
        {

            Category? category = await _uow.CategoryRepository.GetByIdAsync(id);
            if (category == null)
                return NotFound();
            
            category.Name = categoryDto.Name;

            _uow.CategoryRepository.Update(category);
            await _uow.SaveChangesAsync();
            return Ok(category);
        }




        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Category? category = await _uow.CategoryRepository.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            _uow.CategoryRepository.Delete(category);
            await _uow.SaveChangesAsync(); 
            return NoContent();
        }




    }
}
