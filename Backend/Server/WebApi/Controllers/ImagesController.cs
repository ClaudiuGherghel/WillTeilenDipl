using Core.Contracts;
using Core.Entities;
using Core.Enums;
using Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebApi.Mappings;
using static WebApi.Dtos.ImageDto;

namespace WebApi.Controllers
{


    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ImagesController(IUnitOfWork uow, ILogger<ImagesController> logger, IWebHostEnvironment env) : BaseController<ImagesController>(uow, logger)
    {
        private readonly IWebHostEnvironment _env = env;


        [HttpGet]
        [ProducesResponseType(typeof(Image[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            ICollection<Image> images = await _uow.ImageRepository.GetAllAsync();
            return Ok(images);
        }



        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Image), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            Image? image = await _uow.ImageRepository.GetByIdAsync(id);
            if (image is null)
                return NotFound(new { error = $"Kein Bild mit der ID {id} gefunden." });

            return Ok(image);
        }




        [HttpPost]
        [Authorize]
        [RequestSizeLimit(5_000_000)] // Max. 5 MB
        [ProducesResponseType(typeof(Image), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostByUser(IFormFile file, [FromBody] ImagePostDto imageDto)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "Die hochgeladene Datei ist leer oder fehlt." });

            if (!file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { error = "Nur Bilddateien sind erlaubt (image/*)." });

            int? userId = GetUserIdFromClaims();
            if (userId is null)
                return Unauthorized(new { error = "Benutzer nicht angemeldet." });

            Item? item = await _uow.ItemRepository.GetByIdAsync(imageDto.ItemId);
            if (item is null)
                return NotFound(new { error = $"Kein Item mit der ID {imageDto.ItemId} gefunden." });

            if (userId != item.OwnerId && !User.IsInRole(nameof(Roles.Admin)))
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "Nur Eigentümer oder Admin darf Bilder hochladen." });

            // Pfad aufbauen: /wwwroot/uploads/user42/
            string userFolder = Path.Combine(_env.WebRootPath, "uploads", $"user{userId}");
            Directory.CreateDirectory(userFolder); // falls noch nicht vorhanden

            string extension = Path.GetExtension(file.FileName);
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(userFolder, uniqueFileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            // URL erzeugen: /uploads/user42/filename.jpg
            string relativeUrl = $"/uploads/user{userId}/{uniqueFileName}";
            string absoluteUrl = $"{Request.Scheme}://{Request.Host}{relativeUrl}";

            Image imageToPost = imageDto.ToEntity();
            imageToPost.ImageUrl = absoluteUrl;

            _uow.ImageRepository.Insert(imageToPost);
            await _uow.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = imageToPost.Id }, imageToPost);
        }





        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(Image), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutByUser(int id, [FromBody] ImagePutDto imageDto)
        {
            if (id != imageDto.Id)
                return BadRequest(new { error = "Die ID in der URL stimmt nicht mit der ID im Body überein." });

            Image? imageToPut = await _uow.ImageRepository.GetByIdAsync(id);
            if (imageToPut is null)
                return NotFound(new { error = $"Kein Bild mit der ID {id} gefunden." });

            Item? item = await _uow.ItemRepository.GetByIdAsync(imageDto.ItemId);
            if (item is null)
                return NotFound(new { error = $"Kein Item mit der ID {imageDto.ItemId} gefunden." });

            int? userId = GetUserIdFromClaims();
            if (userId == null)
                return Unauthorized(new { error = "Benutzer nicht angemeldet." });

            if (userId != item.OwnerId && !User.IsInRole(nameof(Roles.Admin)))
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "Nur der Eigentümer oder ein Admin darf das Bild bearbeiten." });

            imageDto.UpdateEntity(imageToPut);
            _uow.ImageRepository.Update(imageToPut);
            await _uow.SaveChangesAsync();

            return Ok(imageToPut);
        }




        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteByUser(int id)
        {
            Image? imageToRemove = await _uow.ImageRepository.GetByIdAsync(id);
            if (imageToRemove is null)
                return NotFound(new { error = $"Kein Bild mit der ID {id} gefunden." });

            Item? item = await _uow.ItemRepository.GetByIdAsync(imageToRemove.ItemId);
            if (item is null)
                return NotFound(new { error = $"Kein zugehöriges Item mit der ID {imageToRemove.ItemId} gefunden." });

            int? userId = GetUserIdFromClaims();
            if (userId == null)
                return Unauthorized(new { error = "Benutzer nicht angemeldet." });

            // Admin darf auch löschen
            bool isOwnerOrAdmin = (userId == item.OwnerId || User.IsInRole(nameof(Roles.Admin)));
            if (!isOwnerOrAdmin)
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "Nur der Eigentümer oder ein Admin darf das Bild löschen." });

            // Datei löschen – Besitzer-ID nutzen, auch bei Admins
            if (!string.IsNullOrWhiteSpace(imageToRemove.ImageUrl))
            {
                string fileName = Path.GetFileName(imageToRemove.ImageUrl);
                string userFolder = Path.Combine(_env.WebRootPath, "uploads", $"user{item.OwnerId}");
                string fullPath = Path.Combine(userFolder, fileName);

                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);
            }

            _uow.ImageRepository.SoftDelete(id);
            await _uow.SaveChangesAsync();

            return NoContent();
        }







    }
}
