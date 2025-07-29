using System.ComponentModel.DataAnnotations;

namespace WebApi.Dtos
{
    public class CategoryDto
    {
        public record CategoryPostDto(
            [Required(AllowEmptyStrings = false, ErrorMessage = "Kategoriename muss eingegeben werden")]
            [StringLength(100, MinimumLength = 2, ErrorMessage = "Kategoriename muss zwischen 2 und 100 Zeichen lang sein")]
            string Name
        );
        public record CategoryPutDto(
            int Id,
            byte[]? RowVersion,

            [Required(AllowEmptyStrings = false, ErrorMessage = "Kategoriename muss eingegeben werden")]
            [StringLength(100, MinimumLength = 2, ErrorMessage = "Kategoriename muss zwischen 2 und 100 Zeichen lang sein")]
            string Name
        );
    }
}
