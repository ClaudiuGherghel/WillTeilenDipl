using Core.Contracts;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebApi.Mappings;

namespace WebApi.Controllers
{

    public record ImagePostDto(
        [Required(AllowEmptyStrings = false, ErrorMessage = "Url muss eingegeben werden")]
        [StringLength(300, ErrorMessage = "Die Bild-URL darf maximal 300 Zeichen lang sein.")]
        [Url] //Fehlermeldung auch bei ""
        string Url,
        [StringLength(150, ErrorMessage = "Der Alternativtext darf maximal 150 Zeichen lang sein.")]
        string AltText,
        [StringLength(100, ErrorMessage = "Der MIME-Typ darf maximal 100 Zeichen lang sein.")]
        string MimeType, // z. B. "image/jpeg"
        [Range(1, int.MaxValue, ErrorMessage = "ItemId muss größer als 0 sein.")]
        int ItemId
    );
    public record ImagePutDto(
        int Id,
        byte[]? RowVersion,
        [Required(AllowEmptyStrings = false, ErrorMessage = "Url muss eingegeben werden")]
        [StringLength(300, ErrorMessage = "Die Bild-URL darf maximal 300 Zeichen lang sein.")]
        [Url] //Fehlermeldung auch bei ""
        string Url,
        [StringLength(150, ErrorMessage = "Der Alternativtext darf maximal 150 Zeichen lang sein.")]
        string AltText,
        [StringLength(100, ErrorMessage = "Der MIME-Typ darf maximal 100 Zeichen lang sein.")]
        string MimeType, // z. B. "image/jpeg"
        [Range(1, int.MaxValue, ErrorMessage = "ItemId muss größer als 0 sein.")]
        int ItemId
    );




    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ImagesController(IUnitOfWork uow, IWebHostEnvironment env) : ControllerBase
    {
        private readonly IUnitOfWork _uow = uow;
        private readonly IWebHostEnvironment _env = env;


        // Bei Fehlern wird Middleware einspringen



        [HttpGet]
        [ProducesResponseType(typeof(Image[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            ICollection<Image> images = await _uow.ImageRepository.GetAllAsync();
            return Ok(images);
        }



        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Image), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            Image? image = await _uow.ImageRepository.GetByIdAsync(id);

            if (image is null)
            {
                return NotFound();
            }
            return Ok(image);
        }



        [HttpPost]
        [ProducesResponseType(typeof(Image), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> Post([FromBody] ImagePostDto imageDto)
        {

            if (imageDto == null)
            {
                return BadRequest();
            }

            Item? item = await _uow.ItemRepository.GetByIdAsync(imageDto.ItemId);
            if (item is null)
            {
                return NotFound();
            }

            Image imageToPost = imageDto.ToEntity();

            _uow.ImageRepository.Insert(imageToPost);
            await _uow.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = imageToPost.Id }, imageToPost);
        }




        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Image), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(int id, [FromBody] ImagePutDto imageDto)
        {
            if (imageDto.Id != id)
                return BadRequest("ID im Body stimmt nicht mit ID in URL überein.");

            Image? imageToPut = await _uow.ImageRepository.GetByIdAsync(id);
            if (imageToPut == null)
            {
                return NotFound();
            }

            Item? item = await _uow.ItemRepository.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            imageDto.UpdateEntity(imageToPut);
            _uow.ImageRepository.Update(imageToPut);
            await _uow.SaveChangesAsync();
            return Ok(imageDto);
        }




        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            Image? imageToRemove = await _uow.ImageRepository.GetByIdAsync(id);
            if (imageToRemove is null)
            {
                return NotFound();
            }
            _uow.ImageRepository.Delete(imageToRemove);
            await _uow.SaveChangesAsync();
            return NoContent();
        }


        [HttpPost("upload")]
        [RequestSizeLimit(5_000_000)] // z. B. 5 MB Limit
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Datei ist leer.");

            // Nur Bilder erlauben
            if (!file.ContentType.StartsWith("image/"))
                return BadRequest("Nur Bilddateien sind erlaubt.");

            string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder); // Falls Ordner noch nicht existiert

            string extension = Path.GetExtension(file.FileName);
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            string url = $"{Request.Scheme}://{Request.Host}/uploads/{uniqueFileName}";

            return Ok(new
            {
                imageUrl = url,
                mimeType = file.ContentType,
                fileSize = file.Length
            });
        }

    }
}
