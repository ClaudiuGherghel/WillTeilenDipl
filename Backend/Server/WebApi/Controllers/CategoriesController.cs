using Core.Contracts;
using Core.Entities;
using Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebApi.Mappings;
using static WebApi.Dtos.CategoryDto;


namespace WebApi.Controllers
{


    [Route("api/[controller]/[action]")]
    //Kein if (ModelState.IsValid) mehr nötig, bei ApiController passiert das automatisch, (Fehlerausgabe ProblemDetails) 
    [ApiController]

    public class CategoriesController(IUnitOfWork uow) : ControllerBase
    {
    // Bei Fehlern wird Middleware einspringen

        private readonly IUnitOfWork _uow = uow;


        [HttpGet]
        [ProducesResponseType(typeof(Category[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByAll()
        {
            ICollection<Category> categories = await _uow.CategoryRepository.GetAllAsync();
            return Ok(categories);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Category), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByAll(int id)
        {
            Category? category = await _uow.CategoryRepository.GetByIdAsync(id);

            if (category != null)
                return Ok(category);
            else
                return NotFound();
        }



        [HttpPost]
        [Authorize(Roles = nameof(Roles.Admin))]
        [ProducesResponseType(typeof(Category), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> PostByAdmin([FromBody] CategoryPostDto categoryDto)
        {
            Category categoryToPost = categoryDto.ToEntity();

            _uow.CategoryRepository.Insert(categoryToPost);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(GetByAll), new { id = categoryToPost.Id }, categoryToPost);
        }





        [HttpPut("{id}")]
        [Authorize(Roles = nameof(Roles.Admin))]
        [ProducesResponseType(typeof(Category), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutByAdmin(int id, [FromBody] CategoryPutDto categoryDto)
        {
            if (id != categoryDto.Id)
                return BadRequest();

            Category? categoryToPut = await _uow.CategoryRepository.GetByIdAsync(id);
            if (categoryToPut == null)
                return NotFound();

            categoryDto.UpdateEntity(categoryToPut);

            _uow.CategoryRepository.Update(categoryToPut);
            await _uow.SaveChangesAsync();
            return Ok(categoryToPut);
        }




        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> DeleteByAdmin(int id)
        {
            Category? categoryToRemove = await _uow.CategoryRepository.GetByIdAsync(id);
            if (categoryToRemove == null)
                return NotFound();

            //_uow.CategoryRepository.Delete(categoryToRemove);

            _uow.CategoryRepository.SoftDelete(id);
            await _uow.SaveChangesAsync(); 
            return NoContent();
        }




    }
}
