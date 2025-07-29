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
    //Kein if (ModelState.IsValid) mehr nötig, bei ApiController passiert das automatisch, (Fehlerausgabe ProblemDetails) 
    [ApiController]
    public class UsersController(IUnitOfWork uow, IConfiguration config) : ControllerBase
    {
        private readonly IUnitOfWork _uow = uow;
        private readonly IConfiguration _configuration = config;

        // Bei Fehlern wird Middleware einspringen

        [HttpGet]
        [Authorize(Roles = nameof(Roles.Admin))]
        [ProducesResponseType(typeof(User[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByAdmin()
        {
            ICollection<User> users = await _uow.UserRepository.GetAllAsync();
            return Ok(users);
        }



        [HttpGet("{id}")]
        [Authorize(Roles = nameof(Roles.Admin))]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByAdmin(int id)
        {
            User? user = await _uow.UserRepository.GetByIdAsync(id);

            if (user is null)
            {
                return NotFound();
            }
            return Ok(user);
        }


        [HttpPost]
        [Authorize(Roles = nameof(Roles.Admin))]
        [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        public async Task<IActionResult> Register([FromBody] UserPostDto userDto)
        {

            User userToPost = userDto.ToEntity();

            _uow.UserRepository.Insert(userToPost);
            await _uow.SaveChangesAsync();
            var token = GenerateJwtToken(userToPost);
            return Ok(new AuthResponse(token));
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _uow.UserRepository.AuthenticateAsync(request.Username, request.Password);
            if (user == null)
                return Unauthorized("Invalid credentials");
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
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
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
        public async Task<IActionResult> Update(int id, [FromBody] UserPutDto userDto)
        {
            // Benutzer darf sich nur selbst bearbeiten - außer Admin

            if (id != userDto.Id)
                return BadRequest();

            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return Unauthorized();

            if (userId != id && !User.IsInRole(nameof(Roles.Admin)))
                return Forbid();

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


        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] UserChangePasswordDto userDto)
        {
            // Benutzer darf nur eigenes Passwort ändern - außer Admin
            if (id != userDto.Id)
                return BadRequest();

            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return Unauthorized();

            if (userId != id && !User.IsInRole(nameof(Roles.Admin)))
                return Forbid();

            var userToPut = await _uow.UserRepository.GetByIdAsync(id);
            if (userToPut == null)
            {
                return NotFound();
            }

            userDto.ChangePassword(userToPut);

            _uow.UserRepository.Update(userToPut);
            await _uow.SaveChangesAsync();

            return NoContent();
        }



        
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id)
        {

            // Benutzer darf nur sich selbst löschen – außer Admin
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return Unauthorized();

            if (userId != id && !User.IsInRole(nameof(Roles.Admin)))
                return Forbid();

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
