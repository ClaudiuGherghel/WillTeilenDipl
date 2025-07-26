using Core.Contracts;
using Core.Entities;
using Core.Enums;
using Core.Validations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Mappings;

namespace WebApi.Controllers
{
    public record ItemPostDto(
        [Required(AllowEmptyStrings = false, ErrorMessage = "Gegenstandsname muss eingegeben werden")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Gegenstandsname muss zwischen 2 und 100 Zeichen lang sein")]
        string Name,

        [StringLength(1000, ErrorMessage = "Beschreibung darf maximal 1000 Zeichen lang sein")]
        string Description,

        bool IsAvailable,

        [Required(AllowEmptyStrings = false, ErrorMessage = "Land muss eingegeben werden")]
        [StringLength(50, ErrorMessage = "Land darf maximal 50 Zeichen lang sein")]
        string Country,

        [Required(AllowEmptyStrings = false, ErrorMessage = "Bundesland muss eingegeben werden")]
        [StringLength(50, ErrorMessage = "Bundesland darf maximal 50 Zeichen lang sein")]
        string State,

        [Required(AllowEmptyStrings = false, ErrorMessage = "Postleitzahl muss eingegeben werden")]
        [StringLength(20, ErrorMessage = "Postleitzahl darf maximal 20 Zeichen lang sein")]
        string PostalCode,

        [Required(AllowEmptyStrings = false, ErrorMessage = "Ort muss eingegeben werden")]
        [StringLength(100, ErrorMessage = "Ort darf maximal 100 Zeichen lang sein")]
        string Place,

        [StringLength(200, ErrorMessage = "Adresse darf maximal 200 Zeichen lang sein")]
        string Address,

        [Required(ErrorMessage = "Miete muss angegeben werden")]
        [Range(0, double.MaxValue, ErrorMessage = "Miete muss 0 oder höher sein")]
        decimal Price,

        [Range(0, int.MaxValue, ErrorMessage = "Stock muss 0 oder höher sein")]
        int Stock,

        [Required(ErrorMessage = "Kaution muss angegeben werden")]
        [Range(0, double.MaxValue, ErrorMessage = "Kaution muss 0 oder höher sein")]
        decimal Deposit,

        [NotUnknownEnum(RentalType.Unknown, ErrorMessage = "Miettyp darf nicht Unknown sein.")]
         RentalType RentalType, // Required funktioniert nur mit null, deshalb Custom Validator

        ItemCondition ItemCondition,

        // Foreign Keys
        [Range(1, int.MaxValue, ErrorMessage = "SubCategoryId muss größer als 0 sein.")]
        int SubCategoryId,
        [Range(1, int.MaxValue, ErrorMessage = "OwnerId (UserId) muss größer als 0 sein.")]
        int OwnerId
    );

    public record ItemPutDto(
        int Id,
        byte[]? RowVersion,

        [Required(AllowEmptyStrings = false, ErrorMessage = "Gegenstandsname muss eingegeben werden")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Gegenstandsname muss zwischen 2 und 100 Zeichen lang sein")]
        string Name,

        [StringLength(1000, ErrorMessage = "Beschreibung darf maximal 1000 Zeichen lang sein")]
        string Description,

        bool IsAvailable,

        [Required(AllowEmptyStrings = false, ErrorMessage = "Land muss eingegeben werden")]
        [StringLength(50, ErrorMessage = "Land darf maximal 50 Zeichen lang sein")]
        string Country,

        [Required(AllowEmptyStrings = false, ErrorMessage = "Bundesland muss eingegeben werden")]
        [StringLength(50, ErrorMessage = "Bundesland darf maximal 50 Zeichen lang sein")]
        string State,

        [Required(AllowEmptyStrings = false, ErrorMessage = "Postleitzahl muss eingegeben werden")]
        [StringLength(20, ErrorMessage = "Postleitzahl darf maximal 20 Zeichen lang sein")]
        string PostalCode,

        [Required(AllowEmptyStrings = false, ErrorMessage = "Ort muss eingegeben werden")]
        [StringLength(100, ErrorMessage = "Ort darf maximal 100 Zeichen lang sein")]
        string Place,

        [StringLength(200, ErrorMessage = "Adresse darf maximal 200 Zeichen lang sein")]
        string Address,

        [Required(ErrorMessage = "Miete muss angegeben werden")]
        [Range(0, double.MaxValue, ErrorMessage = "Miete muss 0 oder höher sein")]
        decimal Price,

        [Range(0, int.MaxValue, ErrorMessage = "Stock muss 0 oder höher sein")]
        int Stock,

        [Required(ErrorMessage = "Kaution muss angegeben werden")]
        [Range(0, double.MaxValue, ErrorMessage = "Kaution muss 0 oder höher sein")]
        decimal Deposit,

        [NotUnknownEnum(RentalType.Unknown, ErrorMessage = "Miettyp darf nicht Unknown sein.")]
         RentalType RentalType, // Required funktioniert nur mit null, deshalb Custom Validator

        ItemCondition ItemCondition,

        // Foreign Keys
        [Range(1, int.MaxValue, ErrorMessage = "SubCategoryId muss größer als 0 sein.")]
        int SubCategoryId,
        [Range(1, int.MaxValue, ErrorMessage = "OwnerId (UserId) muss größer als 0 sein.")]
        int OwnerId
    );


    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ItemsController(IUnitOfWork uow) : ControllerBase
    {
        private readonly IUnitOfWork _uow = uow;



        [HttpGet]
        [ProducesResponseType(typeof(Item[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            ICollection<Item> items = await _uow.ItemRepository.GetAllAsync();

            return Ok(items);
        }



        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Item), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
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
        public async Task<IActionResult> GetByFilter([FromQuery] string filter)
        {
            ICollection<Item> items = await _uow.ItemRepository.GetItemsByFilterAsync(filter);
            return Ok(items);
        }





        [HttpPost]
        [ProducesResponseType(typeof(Item), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Post([FromBody] ItemPostDto itemDto)
        {
            if(itemDto is null)
            {
                return BadRequest();
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

            return CreatedAtAction(nameof(Get), new { id = itemToPost.Id }, itemToPost);
        }




        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Item), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(int id, [FromBody] ItemPutDto itemDto)
        {
            if (itemDto is null)
            {
                return BadRequest();
            }

            if (itemDto.Id != id)
                return BadRequest("ID im Body stimmt nicht mit ID in URL überein.");

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
