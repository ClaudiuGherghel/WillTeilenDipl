using System.ComponentModel.DataAnnotations;

namespace WebApi.Dtos
{
    public class ImageDto
    {
        public record ImagePostDto(
            [Required(AllowEmptyStrings = false, ErrorMessage = "Url muss eingegeben werden")]
            [StringLength(300, ErrorMessage = "Die Bild-URL darf maximal 300 Zeichen lang sein.")]
            [Url] //Fehlermeldung auch bei ""
            string ImageUrl,

            [StringLength(150, ErrorMessage = "Der Alternativtext darf maximal 150 Zeichen lang sein.")]
            string AltText,

            [StringLength(100, ErrorMessage = "Der MIME-Typ darf maximal 100 Zeichen lang sein.")]
            string MimeType, // z. B. "image/jpeg"

            int DisplayOrder,
            bool IsMainImage,
            [Range(1, int.MaxValue, ErrorMessage = "ItemId muss größer als 0 sein.")]
            int ItemId
        );

        public record ImagePutDto(
            int Id,
            byte[]? RowVersion,

            [Required(AllowEmptyStrings = false, ErrorMessage = "Url muss eingegeben werden")]
            [StringLength(300, ErrorMessage = "Die Bild-URL darf maximal 300 Zeichen lang sein.")]
            [Url] //Fehlermeldung auch bei ""
            string ImageUrl,

            [StringLength(150, ErrorMessage = "Der Alternativtext darf maximal 150 Zeichen lang sein.")]
            string AltText,

            [StringLength(100, ErrorMessage = "Der MIME-Typ darf maximal 100 Zeichen lang sein.")]
            string MimeType, // z. B. "image/jpeg"

            int DisplayOrder,
            bool IsMainImage,
            [Range(1, int.MaxValue, ErrorMessage = "ItemId muss größer als 0 sein.")]
            int ItemId
        );
    }
}
