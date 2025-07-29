using System.ComponentModel.DataAnnotations;

namespace WebApi.Dtos
{
    public class SubCategoryDto
    {
        public record SubCategoryPostDto(
            [Required(AllowEmptyStrings = false, ErrorMessage = "Unterkategoriename muss eingegeben werden")]
            [StringLength(100, MinimumLength = 2, ErrorMessage = "Subkategoriename muss zwischen 2 und 100 Zeichen lang sein")]
            string Name,

            [Range(1, int.MaxValue, ErrorMessage = "CategoryId muss größer als 0 sein.")]
            int CategoryId
        );
        public record SubCategoryPutDto(
            int Id,
            byte[]? RowVersion,

            [Required(AllowEmptyStrings = false, ErrorMessage = "Unterkategoriename muss eingegeben werden")]
            [StringLength(100, MinimumLength = 2, ErrorMessage = "Subkategoriename muss zwischen 2 und 100 Zeichen lang sein")]
            string Name,

            //Foreign Keys
            [Range(1, int.MaxValue, ErrorMessage = "CategoryId muss größer als 0 sein.")]
            int CategoryId
        );
    }
}
