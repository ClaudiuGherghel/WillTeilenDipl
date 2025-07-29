using Core.Contracts;
using Core.Entities;
using Core.Enums;
using Core.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using WebApi.Mappings;
using WebApi.Dtos;
using static WebApi.Dtos.ItemDto;

namespace WebApi.Controllers
{



    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ItemsController(IUnitOfWork uow) : ControllerBase
    {
        private readonly IUnitOfWork _uow = uow;



        [HttpGet]
        [ProducesResponseType(typeof(Item[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByAll()
        {
            ICollection<Item> items = await _uow.ItemRepository.GetAllAsync();

            return Ok(items);
        }



        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Item), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByAll(int id)
        {
            Item? item = await _uow.ItemRepository.GetByIdAsync(id);

            if (item is null)
            {
                return NotFound();
            }
            return Ok(item);

        }


        [HttpGet]
        [ProducesResponseType(typeof(Item[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByFilterByAll([FromQuery] string filter)
        {
            ICollection<Item> items = await _uow.ItemRepository.GetItemsByFilterAsync(filter);
            return Ok(items);
        }



        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(Item), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Post([FromBody] ItemPostDto itemDto)
        {
            // Nur Eigentümer oder Admin darf hinzufügen
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                return Unauthorized();
            }

            if (userId != itemDto.OwnerId && !User.IsInRole(nameof(Roles.Admin)))
            {
                return Forbid();
            }

            SubCategory? subCategory = await _uow.SubCategoryRepository.GetByIdAsync(itemDto.SubCategoryId);
            if (subCategory is null)
            {
                return NotFound();
            }

            User? user = await _uow.UserRepository.GetByIdAsync(itemDto.OwnerId);
            if(user is null)
            {
                return NotFound();
            }

            Item itemToPost = itemDto.ToEntity();

            _uow.ItemRepository.Insert(itemToPost);
            await _uow.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByAll), new { id = itemToPost.Id }, itemToPost);
        }




        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(Item), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(int id, [FromBody] ItemPutDto itemDto)
        {
            if (id != itemDto.Id)
                return BadRequest();

            // Nur Eigentümer oder Admin darf änder
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                return Unauthorized();
            }

            if (userId != itemDto.OwnerId && !User.IsInRole(nameof(Roles.Admin)))
            {
                return Forbid();
            }

            Item? itemToPut = await _uow.ItemRepository.GetByIdAsync(id);

            if (itemToPut == null)
            {
                return NotFound();
            }

            SubCategory? subCategory = await _uow.SubCategoryRepository.GetByIdAsync(itemDto.SubCategoryId);
            if (subCategory is null)
            {
                return NotFound();
            }

            User? user = await _uow.UserRepository.GetByIdAsync(itemDto.OwnerId);
            if (user is null)
            {
                return NotFound();
            }

            _uow.ItemRepository.Update(itemToPut);
            await _uow.SaveChangesAsync();
            return Ok(itemDto);
        }




        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            Item? itemToRemove = await _uow.ItemRepository.GetByIdAsync(id);
            if (itemToRemove is null)
            {
                return NotFound();
            }
            //_uow.ItemRepository.Delete(itemToRemove);

            _uow.ItemRepository.SoftDelete(id);

            await _uow.SaveChangesAsync();
            return NoContent();
        }

    }
}
