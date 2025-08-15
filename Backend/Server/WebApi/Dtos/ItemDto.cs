using Core.Enums;
using Core.Validations.Annotations;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Dtos
{
    public class ItemDto
    {
        public record ItemPostDto(
            [Required(AllowEmptyStrings = false, ErrorMessage = "Gegenstandsname muss eingegeben werden")]
            [StringLength(100, MinimumLength = 2, ErrorMessage = "Gegenstandsname muss zwischen 2 und 100 Zeichen lang sein")]
            string Name,

            [StringLength(1000, ErrorMessage = "Beschreibung darf maximal 1000 Zeichen lang sein")]
            string Description,

            bool IsAvailable,

            [StringLength(200, ErrorMessage = "Adresse darf maximal 200 Zeichen lang sein")]
            string Address,

            [Required(ErrorMessage = "Miete muss angegeben werden")]
            [Range(0, double.MaxValue, ErrorMessage = "Miete muss 0 oder höher sein")]
            decimal Price,

            [Range(0, int.MaxValue, ErrorMessage = "Stock muss 0 oder höher sein")]
            int Stock,

            [Required(ErrorMessage = "Kaution muss angegeben werden")]
            [Range(0, double.MaxValue, ErrorMessage = "Kaution muss 0 oder höher sein")]
            decimal Deposit,

            [NotUnknownEnumAttribute(RentalType.Unknown, ErrorMessage = "Miettyp darf nicht Unknown sein.")]
            RentalType RentalType, // Required funktioniert nur mit null, deshalb Custom Validator

            ItemCondition ItemCondition,

            // Foreign Keys
            [Range(1, int.MaxValue, ErrorMessage = "SubCategoryId muss größer als 0 sein.")]
            int SubCategoryId,
            [Range(1, int.MaxValue, ErrorMessage = "OwnerId (UserId) muss größer als 0 sein.")]
            int OwnerId,
            [Range(1, int.MaxValue, ErrorMessage = "GeoPostalId muss größer als 0 sein.")]
            int GeoPostaldId
        );

        public record ItemPutDto(
            int Id,
            byte[]? RowVersion,

            [Required(AllowEmptyStrings = false, ErrorMessage = "Gegenstandsname muss eingegeben werden")]
            [StringLength(100, MinimumLength = 2, ErrorMessage = "Gegenstandsname muss zwischen 2 und 100 Zeichen lang sein")]
            string Name,

            [StringLength(1000, ErrorMessage = "Beschreibung darf maximal 1000 Zeichen lang sein")]
            string Description,

            bool IsAvailable,

            [StringLength(200, ErrorMessage = "Adresse darf maximal 200 Zeichen lang sein")]
            string Address,

            [Required(ErrorMessage = "Miete muss angegeben werden")]
            [Range(0, double.MaxValue, ErrorMessage = "Miete muss 0 oder höher sein")]
            decimal Price,

            [Range(0, int.MaxValue, ErrorMessage = "Stock muss 0 oder höher sein")]
            int Stock,

            [Required(ErrorMessage = "Kaution muss angegeben werden")]
            [Range(0, double.MaxValue, ErrorMessage = "Kaution muss 0 oder höher sein")]
            decimal Deposit,

            [NotUnknownEnumAttribute(RentalType.Unknown, ErrorMessage = "Miettyp darf nicht Unknown sein.")]
            RentalType RentalType, // Required funktioniert nur mit null, deshalb Custom Validator

            ItemCondition ItemCondition,

            // Foreign Keys
            [Range(1, int.MaxValue, ErrorMessage = "SubCategoryId muss größer als 0 sein.")]
            int SubCategoryId,
            [Range(1, int.MaxValue, ErrorMessage = "OwnerId (UserId) muss größer als 0 sein.")]
            int OwnerId,
            [Range(1, int.MaxValue, ErrorMessage = "GeoPostalId muss größer als 0 sein.")]
            int GeoPostaldId
        );
    }
}
