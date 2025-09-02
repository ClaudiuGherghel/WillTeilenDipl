using Core.Contracts;
using Core.Dtos;
using Core.Entities;
using Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebApi.Dtos;
using WebApi.Mappings;
using static WebApi.Dtos.SubCategoryDto;

namespace WebApi.Controllers
{


    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SubCategoriesController(IUnitOfWork uow, ILogger<SubCategoriesController> logger) : BaseController<SubCategoriesController>(uow, logger)
        {



        [HttpGet]
        [ProducesResponseType(typeof(SubCategory[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            ICollection<SubCategory> categories = await _uow.SubCategoryRepository.GetAllAsync();

            return Ok(categories);
        }



        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SubCategory), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            SubCategory? subCategory = await _uow.SubCategoryRepository.GetByIdAsync(id);

            if (subCategory is null)
                return NotFound(new { error = $"Keine Unterkategorie mit der ID {id} gefunden." });

            return Ok(subCategory);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SubCategoryWithMainImageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetWithMainImage(int id)
        {
            SubCategoryWithMainImageDto? subCategoryDto = await _uow.SubCategoryRepository.GetWithMainImageByIdAsync(id);

            if (subCategoryDto is null)
                return NotFound(new { error = $"Keine Unterkategorie mit der ID {id} gefunden." });

            return Ok(subCategoryDto);
        }




        [HttpPost]
        [Authorize(Roles = nameof(Roles.Admin))]
        [ProducesResponseType(typeof(SubCategory), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostByAdmin([FromBody] SubCategoryPostDto subCategoryDto)
        {
            Category? category = await _uow.CategoryRepository.GetByIdAsync(subCategoryDto.CategoryId);
            if (category is null)
                return NotFound(new { error = $"Keine Kategorie mit der ID {subCategoryDto.CategoryId} gefunden." });

            SubCategory subCategoryToPost = subCategoryDto.ToEntity();

            _uow.SubCategoryRepository.Insert(subCategoryToPost);
            await _uow.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = subCategoryToPost.Id }, subCategoryToPost);
        }




        [HttpPut("{id}")]
        [Authorize(Roles = nameof(Roles.Admin))]
        [ProducesResponseType(typeof(SubCategory), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutByAdmin(int id, [FromBody] SubCategoryPutDto subCategoryDto)
        {
            if (id != subCategoryDto.Id)
                return BadRequest(new { error = "Die ID in der URL stimmt nicht mit der ID im Body überein." });

            SubCategory? subCategoryToPut = await _uow.SubCategoryRepository.GetByIdAsync(id);
            if (subCategoryToPut == null)
                return NotFound(new { error = $"Keine Unterkategorie mit der ID {subCategoryDto.Id} gefunden." });

            Category? category = await _uow.CategoryRepository.GetByIdAsync(subCategoryDto.CategoryId);
            if(category == null)
                return NotFound(new { error = $"Keine Kategorie mit der ID {subCategoryDto.CategoryId} gefunden." });

            subCategoryDto.UpdateEntity(subCategoryToPut);
            _uow.SubCategoryRepository.Update(subCategoryToPut);
            await _uow.SaveChangesAsync();
            return Ok(subCategoryToPut);
        }



        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(Roles.Admin))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteByAdmin(int id)
        {
            SubCategory? subCategoryToRemove = await _uow.SubCategoryRepository.GetByIdAsync(id);
            if (subCategoryToRemove is null)
                return NotFound(new { error = $"Keine Unterkategorie mit der ID {id} gefunden." });

            _uow.SubCategoryRepository.SoftDelete(id);
            await _uow.SaveChangesAsync();
            return NoContent();
        }


    }
}
