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
using System.IdentityModel.Tokens.Jwt;

namespace WebApi.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ItemsController(IUnitOfWork uow, ILogger<ItemsController> logger) : BaseController<ItemsController>(uow, logger)
    {

        [HttpGet]
        [ProducesResponseType(typeof(Item[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            ICollection<Item> items = await _uow.ItemRepository.GetAllAsync();

            return Ok(items);
        }



        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Item), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            Item? item = await _uow.ItemRepository.GetByIdAsync(id);

            if (item is null)
                return NotFound(new { error = $"Kein Item mit der ID {id} gefunden." });

            return Ok(item);
        }


        [HttpGet]
        [ProducesResponseType(typeof(Item[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByFilter([FromQuery] string filter)
        {
            ICollection<Item> items = await _uow.ItemRepository.GetItemsByFilterAsync(filter);
            return Ok(items);
        }




        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(Item), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostByUser([FromBody] ItemPostDto itemDto)
        {
            SubCategory? subCategory = await _uow.SubCategoryRepository.GetByIdAsync(itemDto.SubCategoryId);
            if (subCategory == null)
                return NotFound(new { error = $"Keine Unterkategorie mit der ID {itemDto.SubCategoryId} gefunden." });

            User? user = await _uow.UserRepository.GetByIdAsync(itemDto.OwnerId);
            if (user == null)
                return NotFound(new { error = $"Kein Benutzer mit der ID {itemDto.OwnerId} gefunden." });

            int? userId = GetUserIdFromClaims();
            if (userId == null)
                return Unauthorized(new { error = "Benutzer nicht angemeldet." });

            if (userId != itemDto.OwnerId && !User.IsInRole(nameof(Roles.Admin)))
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "Nur Eigentümer oder Admin darf Item hinzufügen." });

            Item itemToPost = itemDto.ToEntity();
            _uow.ItemRepository.Insert(itemToPost);
            await _uow.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = itemToPost.Id }, itemToPost);
        }




        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(Item), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutByUser(int id, [FromBody] ItemPutDto itemDto)
        {
            if (id != itemDto.Id)
                return BadRequest("Die übergebene ID stimmt nicht mit der Item-ID überein.");

            Item? itemToPut = await _uow.ItemRepository.GetByIdAsync(id);

            if (itemToPut == null)
                return NotFound(new { error = $"Kein Item mit ID {itemDto.Id} gefunden." });

            SubCategory? subCategory = await _uow.SubCategoryRepository.GetByIdAsync(itemDto.SubCategoryId);
            if (subCategory is null)
                return NotFound(new { error = $"Keine Unterkategorie mit der ID {itemDto.SubCategoryId} gefunden." });

            User? user = await _uow.UserRepository.GetByIdAsync(itemDto.OwnerId);
            if (user is null)
                return NotFound(new { error = $"Kein Benutzer mit der ID {itemDto.OwnerId} gefunden." });

            int? userId = GetUserIdFromClaims();
            if (userId == null)
                return Unauthorized(new { error = "Benutzer nicht angemeldet." });

            if (userId != itemDto.OwnerId && !User.IsInRole(nameof(Roles.Admin)))
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "Nur Eigentümer oder Admin darf Item hinzufügen." });

            _uow.ItemRepository.Update(itemToPut);
            await _uow.SaveChangesAsync();
            return Ok(itemDto);
        }




        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteByUser(int id)
        {
            Item? itemToRemove = await _uow.ItemRepository.GetByIdAsync(id);
            if (itemToRemove is null)
                return NotFound(new { error = $"Kein Item mit ID {id} gefunden." });

            int? userId = GetUserIdFromClaims();
            if (userId is null)
                return Unauthorized(new { error = "Benutzer nicht angemeldet." });

            if (itemToRemove.OwnerId != userId && !User.IsInRole(nameof(Roles.Admin)))
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "Nur der Eigentümer oder ein Admin darf das Item löschen." });

            _uow.ItemRepository.SoftDelete(id);
            await _uow.SaveChangesAsync();
            _logger.LogInformation("User {UserId} deleted Item {ItemId}", userId, id);
            return NoContent();
        }

    }
}
