using Core.Contracts;
using Core.Entities;
using Core.Enums;
using Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebApi.Dtos;
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

        [HttpGet("{itemId}")]
        [ProducesResponseType(typeof(Image), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByItemId(int itemId)
        {
            ICollection<Image> images = await _uow.ImageRepository.GetByItemIdAsync(itemId);

            return Ok(images);
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
        [Consumes("multipart/form-data")]

        public async Task<IActionResult> PostByUser([FromForm] ImageClassPostDto imageDto)
        {
            if (imageDto == null || imageDto.File.Length == 0)
                return BadRequest(new { error = "Die hochgeladene Datei ist leer oder fehlt." });

            if (!imageDto.File.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
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
            string webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
            string userFolder = Path.Combine(webRoot, "uploads", $"user{userId}");
            Directory.CreateDirectory(userFolder);

            string extension = Path.GetExtension(imageDto.File.FileName);
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(userFolder, uniqueFileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await imageDto.File.CopyToAsync(stream);

            // URL erzeugen: /uploads/user42/filename.jpg
            string relativeUrl = $"/uploads/user{userId}/{uniqueFileName}";
            string absoluteUrl = $"{Request.Scheme}://{Request.Host}{relativeUrl}";


            int cntImages = await _uow.ImageRepository.CountAsync(imageDto.ItemId);
            bool isMainImage = false;

            if(cntImages == 0)
             isMainImage = true;   


            string altText = imageDto.AltText;
            if (string.IsNullOrWhiteSpace(imageDto.AltText))
            {
                altText = Path.GetFileNameWithoutExtension(imageDto.File.FileName);
            }

            var imageToPost = new Image
            {
                ItemId = imageDto.ItemId,
                ImageUrl = absoluteUrl,
                AltText = imageDto.AltText,
                IsMainImage = false,
            };

            //var imageToPost = imageDto.ToEntity();
            imageToPost.IsMainImage = isMainImage;
            imageToPost.AltText = altText;

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

            Console.WriteLine(Convert.ToBase64String(imageToPut.RowVersion));
            Console.WriteLine(Convert.ToBase64String(imageDto.RowVersion));
            Console.WriteLine(Convert.ToBase64String(imageToPut.RowVersion));
            imageDto.UpdateEntity(imageToPut);
            _uow.ImageRepository.Update(imageToPut);
            await _uow.SaveChangesAsync();

            if (imageToPut.IsMainImage)
            {
                Image? otherMainImage = await _uow.ImageRepository.GetOtherMainImageAsync(id);
                if(otherMainImage == null)
                {
                    return Ok(imageToPut);
                }
                otherMainImage.IsMainImage = false;
                _uow.ImageRepository.Update(otherMainImage);
                await _uow.SaveChangesAsync();
            }
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

            bool isMainImageToChange = false;
            if (imageToRemove.IsMainImage)
            {
                int cntImages = 0;
                cntImages = await _uow.ImageRepository.CountAsync(item.Id);
                if (cntImages > 1)
                {
                    isMainImageToChange = true;
                }
            }
            
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

            if (isMainImageToChange)
            {
                ICollection<Image> list = await _uow.ImageRepository.GetAllAsync();
                list = [..list.OrderBy(x => x.Id)];
                Image imageToChange = list.FirstOrDefault() ?? throw new ArgumentNullException("Es sollte zumindest ein Image existieren");

                imageToChange.UpdatedAt = DateTime.UtcNow;
                imageToChange.IsMainImage = true;
                _uow.ImageRepository.Update(imageToChange);
            }

            await _uow.SaveChangesAsync();

            return NoContent();
        }







    }
}
