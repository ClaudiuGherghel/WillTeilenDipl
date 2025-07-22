using Core.Contracts;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Controllers
{

    public record SubCategoryPostDto(
    [Required(AllowEmptyStrings = false, ErrorMessage = "Kategoriename muss eingegeben werden")] string Name,
    int CategoryId
    );
    public record SubCategoryPutDto(
        int Id,
        //byte[]? RowVersion,
        [Required(AllowEmptyStrings = false, ErrorMessage = "Kategoriename muss eingegeben werden")] string Name,
        int CategoryId
        );


    [Route("api/[controller]")]
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

            SubCategory newSubCategory = new()
            {
                CategoryId = category.Id,
                Name = subCategoryDto.Name,
            };

            _uow.SubCategoryRepository.Insert(newSubCategory);
            await _uow.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = newSubCategory.Id }, newSubCategory);
        }




        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SubCategory), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Put(int id, [FromBody] SubCategoryPutDto subCategoryDto)
        {

            SubCategory? subCategory = await _uow.SubCategoryRepository.GetByIdAsync(id);

            if (subCategory == null)
            {
                return NotFound();
            }

            Category? category = await _uow.CategoryRepository.GetByIdAsync(subCategoryDto.CategoryId);
            if(category == null)
            {
                return NotFound();
            }

            subCategory.Name = subCategoryDto.Name;
            subCategory.CategoryId = subCategoryDto.CategoryId;

            _uow.SubCategoryRepository.Update(subCategory);
            await _uow.SaveChangesAsync();
            return Ok(subCategory);
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id)
        {
            SubCategory? subCategory = await _uow.SubCategoryRepository.GetByIdAsync(id);
            if (subCategory is null)
            {
                return NotFound();
            }

            _uow.SubCategoryRepository.Delete(subCategory);
            await _uow.SaveChangesAsync();
            return NoContent();
        }


    }
}
