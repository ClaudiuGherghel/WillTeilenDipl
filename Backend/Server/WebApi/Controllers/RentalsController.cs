using Core.Contracts;
using Core.Entities;
using Core.Enums;
using Core.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using WebApi.Dtos;
using WebApi.Mappings;
using static WebApi.Dtos.RentalDto;

namespace WebApi.Controllers
{


    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RentalsController(IUnitOfWork uow, ILogger<RentalsController> logger) : BaseController<RentalsController>(uow, logger)
    {

        [HttpGet]
        [ProducesResponseType(typeof(Rental[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            ICollection<Rental> rentals = await _uow.RentalRepository.GetAllAsync();

            return Ok(rentals);
        }



        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Rental), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            Rental? rental = await _uow.RentalRepository.GetByIdAsync(id);

            if (rental is null)
                return NotFound(new { error = $"Kein Termin mit der ID {id} gefunden." });

            return Ok(rental);
        }



        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(Rental), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostByUser([FromBody] RentalPostDto rentalDto)
        {
            User? user = await _uow.UserRepository.GetByIdAsync(rentalDto.RenterId);
            if (user is null)
                return NotFound(new { error = $"Kein Benutzer mit der ID {rentalDto.RenterId} gefunden." });

            Item? item = await _uow.ItemRepository.GetByIdAsync(rentalDto.ItemId);
            if (item is null)
                return NotFound(new { error = $"Kein Item mit der ID {rentalDto.ItemId} gefunden." });


            int? userId = GetUserIdFromClaims();
            if (userId == null)
                return Unauthorized(new { error = "Benutzer nicht angemeldet." });

            if (userId != item.OwnerId && !User.IsInRole(nameof(Roles.Admin)))
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "Nur Bentuzer oder Admin darf buchen." });


            Rental rentalToPost = rentalDto.ToEntity();
            _uow.RentalRepository.Insert(rentalToPost);
            await _uow.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = rentalToPost.Id }, rentalToPost);
        }




        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(Rental), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutByUser(int id, [FromBody] RentalPutDto rentalDto)
        {
            if (id != rentalDto.Id)
                return BadRequest(new { error = "Die ID in der URL stimmt nicht mit der ID im Body überein." });

            Rental? rentalToPut = await _uow.RentalRepository.GetByIdAsync(id);
            if (rentalToPut == null)
                return NotFound(new { error = $"Kein Termin mit der ID {rentalDto.Id} gefunden." });

            User? user = await _uow.UserRepository.GetByIdAsync(rentalDto.RenterId);
            if (user is null)
                return NotFound(new { error = $"Kein Benutzer mit der ID {rentalDto.RenterId} gefunden." });

            Item? item = await _uow.ItemRepository.GetByIdAsync(rentalDto.ItemId);
            if (item is null)
                return NotFound(new { error = $"Kein Item mit der ID {rentalDto.ItemId} gefunden." });

            int? userId = GetUserIdFromClaims();
            if (userId == null)
                return Unauthorized(new { error = "Benutzer nicht angemeldet." });

            if (userId != item.OwnerId && !User.IsInRole(nameof(Roles.Admin)))
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "Nur der Benutzer oder ein Admin darf den Termin bearbeiten." });

            rentalDto.UpdateEntity(rentalToPut);
            _uow.RentalRepository.Update(rentalToPut);
            await _uow.SaveChangesAsync();
            return Ok(rentalToPut);
        }


        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteByUser(int id)
        {
            Rental? rentalToPut = await _uow.RentalRepository.GetByIdAsync(id);
            if (rentalToPut == null)
                return NotFound(new { error = $"Kein Termin mit der ID {id} gefunden." });

            int? userId = GetUserIdFromClaims();
            if (userId == null)
                return Unauthorized(new { error = "Benutzer nicht angemeldet." });

            if (userId != rentalToPut.RenterId && !User.IsInRole(nameof(Roles.Admin)))
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "Nur der Benutzer oder ein Admin darf den Termin löschen." });

            _uow.RentalRepository.SoftDelete(id);

            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
