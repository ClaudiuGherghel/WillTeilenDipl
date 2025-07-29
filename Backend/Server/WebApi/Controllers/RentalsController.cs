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


    [Route("api/[controller]")]
    [ApiController]
    public class RentalsController(IUnitOfWork uow) : ControllerBase
    {
        // Bei Fehlern wird Middleware einspringen

        private readonly IUnitOfWork _uow = uow;



        [HttpGet]
        [ProducesResponseType(typeof(Rental[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByAll()
        {
            ICollection<Rental> rentals = await _uow.RentalRepository.GetAllAsync();

            return Ok(rentals);
        }



        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Rental), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByAll(int id)
        {
            Rental? rental = await _uow.RentalRepository.GetByIdAsync(id);

            if (rental is null)
            {
                return NotFound();
            }
            return Ok(rental);
        }



        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(Rental), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Post([FromBody] RentalPostDto rentalDto)
        {
            // Nur Eigentümer oder Admin darf änder
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                return Unauthorized();
            }

            if (userId != rentalDto.RenterId && !User.IsInRole(nameof(Roles.Admin)))
            {
                return Forbid();
            }

            User? user = await _uow.UserRepository.GetByIdAsync(rentalDto.RenterId);
            if (user is null)
            {
                return NotFound();
            }

            Item? item = await _uow.ItemRepository.GetByIdAsync(rentalDto.ItemId);
            if (item is null)
            {
                return NotFound();
            }

            Rental rentalToPost = rentalDto.ToEntity();

            _uow.RentalRepository.Insert(rentalToPost);
            await _uow.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByAll), new { id = rentalToPost.Id }, rentalToPost);
        }




        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(Rental), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Put(int id, [FromBody] RentalPutDto rentalDto)
        {
            if (id != rentalDto.Id)
                return BadRequest();

            // Nur Eigentümer oder Admin darf änder
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                return Unauthorized();
            }

            if (userId != rentalDto.RenterId && !User.IsInRole(nameof(Roles.Admin)))
            {
                return Forbid();
            }

            Rental? rentalToPut = await _uow.RentalRepository.GetByIdAsync(id);
            if (rentalToPut == null)
            {
                return NotFound();
            }

            User? user = await _uow.UserRepository.GetByIdAsync(rentalDto.RenterId);
            if (user is null)
            {
                return NotFound();
            }

            Item? item = await _uow.ItemRepository.GetByIdAsync(rentalDto.ItemId);
            if (item is null)
            {
                return NotFound();
            }

            rentalDto.UpdateEntity(rentalToPut);
            _uow.RentalRepository.Update(rentalToPut);
            await _uow.SaveChangesAsync();
            return Ok(rentalToPut);
        }


        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id)
        {
            Rental? rentalToRemove = await _uow.RentalRepository.GetByIdAsync(id);
            if (rentalToRemove is null)
            {
                return NotFound();
            }

            //_uow.RentalRepository.Delete(rentalToRemove);

            _uow.RentalRepository.SoftDelete(id);

            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
