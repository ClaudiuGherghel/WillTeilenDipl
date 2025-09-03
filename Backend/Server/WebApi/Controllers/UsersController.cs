using Azure.Core;
using Core.Contracts;
using Core.Entities;
using Core.Enums;
using Core.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Persistence;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Dtos;
using WebApi.Mappings;
using static WebApi.Dtos.UserDto;

namespace WebApi.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]

    public class UsersController(IUnitOfWork uow, ILogger<UsersController> logger, IConfiguration config, IWebHostEnvironment env) : BaseController<UsersController>(uow, logger)
    {
        private readonly IConfiguration _configuration = config;
        private readonly IWebHostEnvironment _env = env;


        [HttpGet]
        [Authorize(Roles = nameof(Roles.Admin))]
        [ProducesResponseType(typeof(User[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByAdmin()
        {
            ICollection<User> users = await _uow.UserRepository.GetAllAsync();
            return Ok(users);
        }



        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            User? user = await _uow.UserRepository.GetByIdAsync(id);

            if (user is null)
                return NotFound(new { error = $"Kein Benutzer mit der ID {id} gefunden." });

            return Ok(user);
        }


        [HttpPost]
        [Authorize(Roles = nameof(Roles.Admin))]
        [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostByAdmin([FromBody] UserPostDto userDto)
        {
            User userToPost = userDto.ToEntity();

            _uow.UserRepository.Insert(userToPost);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(GetByAdmin), new { id = userToPost.Id }, userToPost);
        }


        [HttpPost]
        [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserPostDto userDto)
        {
            User userToPost = userDto.ToEntity();

            _uow.UserRepository.Insert(userToPost);
            await _uow.SaveChangesAsync();
            var token = GenerateJwtToken(userToPost);
            return Ok(new AuthResponse(token));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _uow.UserRepository.AuthenticateAsync(request.Username, request.Password);
            if (user == null)
                return Unauthorized("Benutzerdaten sind ungültig");
            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(User user)
        {
            string? secret = _configuration["JwtSettings:Secret"] ?? throw new InvalidOperationException("JwtSettings:Secret is not configured");
            var key = Encoding.UTF8.GetBytes(secret);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };
            var token = new JwtSecurityToken(

                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:ExpirationMinutes"])),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(token);
        }





        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateByUser(int id, [FromBody] UserPutDto userDto)
        {

            if (id != userDto.Id)
                return BadRequest("Die übergebene ID stimmt nicht mit der Item-ID überein.");

            User? userToPut = await _uow.UserRepository.GetWithoutReferencesByIdAsync(id);
            if (userToPut == null)
                return NotFound(new { error = $"Kein Benutzer mit der ID {userDto.Id} gefunden." });

            int? userId = GetUserIdFromClaims();
            if (userId == null)
                return Unauthorized(new { error = "Benutzer nicht angemeldet." });

            if (userId != userDto.Id && !User.IsInRole(nameof(Roles.Admin)))
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "Nur Eigentümer oder Admin darf Benutzer bearbeiten." });
 
            userDto.UpdateEntity(userToPut);
            _uow.UserRepository.Update(userToPut);
            await _uow.SaveChangesAsync();
            return Ok(userToPut);
        }


        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangePasswordByUser(int id, [FromBody] UserChangePasswordDto userDto)
        {
            if (id != userDto.Id)
                return BadRequest("Die übergebene ID stimmt nicht mit der Item-ID überein.");

            User? userToPut = await _uow.UserRepository.GetWithoutReferencesByIdAsync(id);
            if (userToPut == null)
                return NotFound(new { error = $"Kein Benutzer mit der ID {userDto.Id} gefunden." });

        int? userId = GetUserIdFromClaims();
            if (userId == null)
                return Unauthorized(new { error = "Benutzer nicht angemeldet." });

            if (userId != userDto.Id && !User.IsInRole(nameof(Roles.Admin)))
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "Nur Eigentümer oder Admin darf das Passwort ändern." });

            var user = await _uow.UserRepository.AuthenticateAsync(userToPut.UserName, userDto.CurrentPassword);
            if (user == null)
                return Unauthorized("Benutzerdaten sind ungültig");

            userDto.ChangePassword(userToPut);

            _uow.UserRepository.Update(userToPut);
            await _uow.SaveChangesAsync();

            return Ok(userToPut);
        }



        
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteByUser(int id)
        {
            User? userToRemove = await _uow.UserRepository.GetWithoutReferencesByIdAsync(id);
            if (userToRemove is null)
                return NotFound(new { error = $"Kein Benutzer mit der ID {id} gefunden." });

            int? userId = GetUserIdFromClaims();
            if (userId is null)
                return Unauthorized(new { error = "Benutzer nicht angemeldet." });

            if (userToRemove.Id != userId && !User.IsInRole(nameof(Roles.Admin)))
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "Nur der Eigentümer oder ein Admin darf den Benutzer löschen." });
            
            _uow.UserRepository.SoftDelete(id);
            await _uow.SaveChangesAsync();
            return NoContent();
        }


    }
}
