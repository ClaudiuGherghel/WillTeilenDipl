using Core.Contracts;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebApi.Mappings;

namespace WebApi.Controllers
{

    public record SubCategoryPostDto(
        [Required(AllowEmptyStrings = false, ErrorMessage = "Unterkategoriename muss eingegeben werden")] 
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Subkategoriename muss zwischen 2 und 100 Zeichen lang sein")]
        string Name,
        [Range(1, int.MaxValue, ErrorMessage = "CategoryId muss größer als 0 sein.")] 
        int CategoryId
    );
    public record SubCategoryPutDto(
        int Id,
        byte[]? RowVersion,
        [Required(AllowEmptyStrings = false, ErrorMessage = "Unterkategoriename muss eingegeben werden")] 
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Subkategoriename muss zwischen 2 und 100 Zeichen lang sein")]
        string Name,
        [Range(1, int.MaxValue, ErrorMessage = "CategoryId muss größer als 0 sein.")] 
        int CategoryId
    );


    [Route("api/[controller]/[action]")]
    //Kein if (ModelState.IsValid) mehr nötig, bei ApiController passiert das automatisch, (Fehlerausgabe ProblemDetails) 
    [ApiController]
    public class SubCategoriesController (IUnitOfWork uow) : ControllerBase
    {
        // Bei Fehlern wird Middleware einspringen

        private readonly IUnitOfWork _uow = uow;

        [HttpGet]
        [ProducesResponseType(typeof(SubCategory[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            ICollection<SubCategory> categories = await _uow.SubCategoryRepository.GetAllAsync();

            return Ok(categories);
        }



        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SubCategory), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            SubCategory? category = await _uow.SubCategoryRepository.GetByIdAsync(id);

            if (category is null)
                return NotFound();
            else
                return Ok(category);
        }



        [HttpPost]
        [ProducesResponseType(typeof(SubCategory), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] SubCategoryPostDto subCategoryDto)
        {

            if (subCategoryDto == null)
            {
                return BadRequest();
            }

            Category? category = await _uow.CategoryRepository.GetByIdAsync(subCategoryDto.CategoryId);
            if (category is null)
            {
                return NotFound();
            }

            SubCategory subCategoryToPost = subCategoryDto.ToEntity();

            _uow.SubCategoryRepository.Insert(subCategoryToPost);
            await _uow.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = subCategoryToPost.Id }, subCategoryToPost);
        }




        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SubCategory), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Put(int id, [FromBody] SubCategoryPutDto subCategoryDto)
        {

            if (subCategoryDto.Id != id)
                return BadRequest("ID im Body stimmt nicht mit ID in URL überein.");

            SubCategory? subCategoryToPut = await _uow.SubCategoryRepository.GetByIdAsync(id);

            if (subCategoryToPut == null)
            {
                return NotFound();
            }

            Category? category = await _uow.CategoryRepository.GetByIdAsync(subCategoryDto.CategoryId);
            if(category == null)
            {
                return NotFound();
            }

            subCategoryDto.UpdateEntity(subCategoryToPut);
            _uow.SubCategoryRepository.Update(subCategoryToPut);
            await _uow.SaveChangesAsync();
            return Ok(subCategoryToPut);
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id)
        {
            SubCategory? subCategoryToRemove = await _uow.SubCategoryRepository.GetByIdAsync(id);
            if (subCategoryToRemove is null)
            {
                return NotFound();
            }

            _uow.SubCategoryRepository.Delete(subCategoryToRemove);
            await _uow.SaveChangesAsync();
            return NoContent();
        }


    }
}
