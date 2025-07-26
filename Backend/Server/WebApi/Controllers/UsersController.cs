using Core.Contracts;
using Core.Entities;
using Core.Enums;
using Core.Validations;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebApi.Mappings;

namespace WebApi.Controllers
{

    public record UserPostDto(
        [Required(AllowEmptyStrings = false, ErrorMessage = "Benutzername muss eingegeben werden")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Benutzername muss zwischen 2 und 50 Zeichen lang sein")]
        string Username,

        [Required(AllowEmptyStrings = false, ErrorMessage = "Passwort muss eingegeben werden")]
        string PasswordHash,

        [Required(AllowEmptyStrings = false, ErrorMessage = "E-Mail muss eingegeben werden")]
        [StringLength(100, ErrorMessage = "E-Mail darf maximal 100 Zeichen lang sein")]
        [EmailAddress(ErrorMessage = "E-Mail ist nicht gültig")]
        string Email,

        [Required(AllowEmptyStrings = false, ErrorMessage = "Vorname muss eingegeben werden")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Vorname muss zwischen 2 und 50 Zeichen lang sein")]
        string FirstName,

        [Required(AllowEmptyStrings = false, ErrorMessage = "Nachname muss eingegeben werden")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Nachname muss zwischen 2 und 50 Zeichen lang sein")]
        string LastName,

        [DateNotMinValue(nameof(BirthDate))] // 1.
        [DateNotInFuture(nameof(BirthDate))] // 2.
        DateTime BirthDate,

        Roles Role,

        [StringLength(100, ErrorMessage = "Land darf maximal 100 Zeichen lang sein")]
        string Country,

        [StringLength(20, ErrorMessage = "Postleitzahl darf maximal 20 Zeichen lang sein")]
        string PostalCode,

        [StringLength(100, ErrorMessage = "Ort darf maximal 100 Zeichen lang sein")]
        string Place,

        [StringLength(200, ErrorMessage = "Adresse darf maximal 200 Zeichen lang sein")]
        string Address,

        [OptionalPhone]
        string PhoneNumber
    );
    public record UserPutDto(
        int Id,
        byte[]? RowVersion,

        [Required(AllowEmptyStrings = false, ErrorMessage = "Benutzername muss eingegeben werden")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Benutzername muss zwischen 2 und 50 Zeichen lang sein")]
        string Username,

        [Required(AllowEmptyStrings = false, ErrorMessage = "Passwort muss eingegeben werden")]
        string PasswordHash,

        [Required(AllowEmptyStrings = false, ErrorMessage = "E-Mail muss eingegeben werden")]
        [StringLength(100, ErrorMessage = "E-Mail darf maximal 100 Zeichen lang sein")]
        [EmailAddress(ErrorMessage = "E-Mail ist nicht gültig")]
        string Email,

        [Required(AllowEmptyStrings = false, ErrorMessage = "Vorname muss eingegeben werden")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Vorname muss zwischen 2 und 50 Zeichen lang sein")]
        string FirstName,

        [Required(AllowEmptyStrings = false, ErrorMessage = "Nachname muss eingegeben werden")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Nachname muss zwischen 2 und 50 Zeichen lang sein")]
        string LastName,

        [DateNotMinValue(nameof(BirthDate))] // 1.
        [DateNotInFuture(nameof(BirthDate))] // 2.
        DateTime BirthDate,

        Roles Role,

        [StringLength(100, ErrorMessage = "Land darf maximal 100 Zeichen lang sein")]
        string Country,

        [StringLength(20, ErrorMessage = "Postleitzahl darf maximal 20 Zeichen lang sein")]
        string PostalCode,

        [StringLength(100, ErrorMessage = "Ort darf maximal 100 Zeichen lang sein")]
        string Place,

        [StringLength(200, ErrorMessage = "Adresse darf maximal 200 Zeichen lang sein")]
        string Address,

        [OptionalPhone]
        string PhoneNumber
    );


    [Route("api/[controller]/[action]")]
    //Kein if (ModelState.IsValid) mehr nötig, bei ApiController passiert das automatisch, (Fehlerausgabe ProblemDetails) 
    [ApiController]
    public class UsersController(IUnitOfWork uow) : ControllerBase
    {
        private readonly IUnitOfWork _uow = uow;

        // Bei Fehlern wird Middleware einspringen

        [HttpGet]
        [ProducesResponseType(typeof(User[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            ICollection<User> users = await _uow.UserRepository.GetAllAsync();
            return Ok(users);
        }



        [HttpGet("{id}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            User? user = await _uow.UserRepository.GetByIdAsync(id);

            if (user is null)
            {
                return NotFound();
            }
            return Ok(user);
        }



        [HttpPost]
        [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] UserPostDto userDto)
        {
            if (userDto == null)
            {
                return BadRequest();
            }

            User userToPost = userDto.ToEntity();

            _uow.UserRepository.Insert(userToPost);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = userToPost.Id }, userToPost);
        }



        [HttpPut("{id}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(int id, [FromBody] UserPutDto userDto)
        {
            if (userDto == null)
            {
                return BadRequest();
            }

            if (userDto.Id != id)
                return BadRequest("ID im Body stimmt nicht mit ID in URL überein.");

            User? userToPut = await _uow.UserRepository.GetByIdAsync(id);
            if (userToPut == null)
            {
                return NotFound();
            }

            userDto.UpdateEntity(userToPut);

            _uow.UserRepository.Update(userToPut);
            await _uow.SaveChangesAsync();
            return Ok(userToPut);
        }



        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id)
        {
            User? userToRemove = await _uow.UserRepository.GetByIdAsync(id);
            if (userToRemove is null)
            {
                return NotFound();
            }
            //_uow.UserRepository.Delete(userToRemove);

            _uow.UserRepository.SoftDelete(id);

            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
