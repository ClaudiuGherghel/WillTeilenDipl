using Core.Contracts;
using Core.Entities;
using Core.Enums;
using Core.Validations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Mappings;

namespace WebApi.Controllers
{

    public record RentalPostDto(
        [DateNotMinValue(nameof(From))] // 1.
        [DateNotInFuture(nameof(From))] // 2.
        DateTime From,

        [DateNotMinValue(nameof(To))] // 3.
        [DateNotInFuture(nameof(To))] // 4.
        DateTime To,

        [StringLength(1000, ErrorMessage = "Notiz darf maximal 1000 Zeichen lang sein")]
        string Note,

        RentalStatus Status,

        //Foreign Keys
        [Range(1, int.MaxValue, ErrorMessage = "RenterId (UserId) muss größer als 0 sein.")]
        int RenterId,

        [Range(1, int.MaxValue, ErrorMessage = "ItemId muss größer als 0 sein.")]
        int ItemId
    );

    public record RentalPutDto(
        int Id,
        byte[]? RowVersion,

        [DateNotMinValue(nameof(From))] // 1.
        [DateNotInFuture(nameof(From))] // 2.
        DateTime From,

        [DateNotMinValue(nameof(To))] // 3.
        [DateNotInFuture(nameof(To))] // 4.
        DateTime To,

        [StringLength(1000, ErrorMessage = "Notiz darf maximal 1000 Zeichen lang sein")]
        string Note,

        RentalStatus Status,

        //Foreign Keys
        [Range(1, int.MaxValue, ErrorMessage = "RenterId (UserId) muss größer als 0 sein.")]
        int RenterId,

        [Range(1, int.MaxValue, ErrorMessage = "ItemId muss größer als 0 sein.")]
        int ItemId
    );



    [Route("api/[controller]")]
    [ApiController]
    public class RentalsController(IUnitOfWork uow) : ControllerBase
    {
        // Bei Fehlern wird Middleware einspringen

        private readonly IUnitOfWork _uow = uow;



        [HttpGet]
        [ProducesResponseType(typeof(Rental[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            ICollection<Rental> rentals = await _uow.RentalRepository.GetAllAsync();

            return Ok(rentals);
        }



        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Rental), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            Rental? rental = await _uow.RentalRepository.GetByIdAsync(id);

            if (rental is null)
            {
                return NotFound();
            }
            return Ok(rental);
        }



        [HttpPost]
        [ProducesResponseType(typeof(Rental), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Post([FromBody] RentalPostDto rentalDto)
        {

            if (rentalDto == null)
            {
                return BadRequest();
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

            return CreatedAtAction(nameof(Get), new { id = rentalToPost.Id }, rentalToPost);
        }




        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Rental), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Put(int id, [FromBody] RentalPutDto rentalDto)
        {
            if (rentalDto == null)
            {
                return BadRequest();
            }

            if (rentalDto.Id != id)
                return BadRequest("ID im Body stimmt nicht mit ID in URL überein.");

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
