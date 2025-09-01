using System.ComponentModel.DataAnnotations;

namespace WebApi.Dtos
{
    public class ImageDto
    {

        //to do: Weglöschen
        public record ImagePostDto(
            IFormFile File,
            string ImageUrl,
            string AltText,
            int DisplayOrder,
            bool IsMainImage,
            [Range(1, int.MaxValue, ErrorMessage = "ItemId muss größer als 0 sein.")]
            int ItemId
        );

        public class ImageClassPostDto
        {
            public IFormFile File { get; set; } = null!;

            public string ImageUrl { get; set; } = string.Empty;

            //[StringLength(150, ErrorMessage = "Der Alternativtext darf maximal 150 Zeichen lang sein.")]
            public string AltText { get; set; } = string.Empty;

            public int DisplayOrder { get; set; } = 0;
            public bool IsMainImage { get; set; } = false;

            [Range(1, int.MaxValue, ErrorMessage = "ItemId muss größer als 0 sein.")]
            public int ItemId { get; set; }
        }

        public record ImagePutDto(
            int Id,
            byte[]? RowVersion,

            [Required(AllowEmptyStrings = false, ErrorMessage = "Url muss eingegeben werden")]
            [StringLength(300, ErrorMessage = "Die Bild-URL darf maximal 300 Zeichen lang sein.")]
            [Url] //Fehlermeldung auch bei ""
            string ImageUrl,

            [StringLength(150, ErrorMessage = "Der Alternativtext darf maximal 150 Zeichen lang sein.")]
            string AltText,

            int DisplayOrder,
            bool IsMainImage,
            [Range(1, int.MaxValue, ErrorMessage = "ItemId muss größer als 0 sein.")]
            int ItemId
        );
    }
}
