using Core.Contracts;
using Core.Entities;
using Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebApi.Mappings;
using static WebApi.Dtos.SubCategoryDto;

namespace WebApi.Controllers
{


    [Route("api/[controller]/[action]")]
    //Kein if (ModelState.IsValid) mehr nötig, bei ApiController passiert das automatisch, (Fehlerausgabe ProblemDetails) 
    [ApiController]
    public class SubCategoriesController (IUnitOfWork uow) : ControllerBase
    {
        // Bei Fehlern wird Middleware einspringen

        private readonly IUnitOfWork _uow = uow;



        [HttpGet]
        [ProducesResponseType(typeof(SubCategory[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByAll()
        {
            ICollection<SubCategory> categories = await _uow.SubCategoryRepository.GetAllAsync();

            return Ok(categories);
        }



        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SubCategory), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByAll(int id)
        {
            SubCategory? subCategory = await _uow.SubCategoryRepository.GetByIdAsync(id);

            if (subCategory is null)
            {
                return NotFound();
            }
            return Ok(subCategory);
        }



        [HttpPost]
        [Authorize(Roles = nameof(Roles.Admin))]
        [ProducesResponseType(typeof(SubCategory), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostByAdmin([FromBody] SubCategoryPostDto subCategoryDto)
        {
            Category? category = await _uow.CategoryRepository.GetByIdAsync(subCategoryDto.CategoryId);
            if (category is null)
            {
                return NotFound();
            }

            SubCategory subCategoryToPost = subCategoryDto.ToEntity();

            _uow.SubCategoryRepository.Insert(subCategoryToPost);
            await _uow.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByAll), new { id = subCategoryToPost.Id }, subCategoryToPost);
        }




        [HttpPut("{id}")]
        [Authorize(Roles = nameof(Roles.Admin))]
        [ProducesResponseType(typeof(SubCategory), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> PutByAdmin(int id, [FromBody] SubCategoryPutDto subCategoryDto)
        {
            if (id != subCategoryDto.Id)
                return BadRequest();

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
        [Authorize(Roles = nameof(Roles.Admin))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteByAdmin(int id)
        {
            SubCategory? subCategoryToRemove = await _uow.SubCategoryRepository.GetByIdAsync(id);
            if (subCategoryToRemove is null)
            {
                return NotFound();
            }

            //_uow.SubCategoryRepository.Delete(subCategoryToRemove);

            _uow.SubCategoryRepository.SoftDelete(id);

            await _uow.SaveChangesAsync();
            return NoContent();
        }


    }
}
