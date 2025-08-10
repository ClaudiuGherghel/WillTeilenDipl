using Core.Enums;
using Core.Validations.Annotations;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Dtos
{
    public class RentalDto
    {
        public record RentalPostDto(
            [DateNotMinValueAttribute(nameof(From))] // 1.
            [DateNotInFutureAttribute(nameof(From))] // 2.
            DateTime From,

            [DateNotMinValueAttribute(nameof(To))] // 3.
            [DateNotInFutureAttribute(nameof(To))] // 4.
            DateTime To,

            [StringLength(1000, ErrorMessage = "Notiz darf maximal 1000 Zeichen lang sein")]
            string Note,

            RentalStatus Status,

            //Foreign Keys
            [Range(1, int.MaxValue, ErrorMessage = "RenterId (UserId) muss größer als 0 sein.")]
            int RenterId,

            [Range(1, int.MaxValue, ErrorMessage = "OwnerId (UserId) muss größer als 0 sein.")]
            int OwnerId,

            [Range(1, int.MaxValue, ErrorMessage = "ItemId muss größer als 0 sein.")]
            int ItemId
        );

        public record RentalPutDto(
            int Id,
            byte[]? RowVersion,

            [DateNotMinValueAttribute(nameof(From))] // 1.
            [DateNotInFutureAttribute(nameof(From))] // 2.
            DateTime From,

            [DateNotMinValueAttribute(nameof(To))] // 3.
            [DateNotInFutureAttribute(nameof(To))] // 4.
            DateTime To,

            [StringLength(1000, ErrorMessage = "Notiz darf maximal 1000 Zeichen lang sein")]
            string Note,

            RentalStatus Status,

            //Foreign Keys
            [Range(1, int.MaxValue, ErrorMessage = "RenterId (UserId) muss größer als 0 sein.")]
            int RenterId,

            [Range(1, int.MaxValue, ErrorMessage = "OwnerId (UserId) muss größer als 0 sein.")]
            int OwnerId,

            [Range(1, int.MaxValue, ErrorMessage = "ItemId muss größer als 0 sein.")]
            int ItemId
        );

    }
}
