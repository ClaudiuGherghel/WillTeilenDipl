using Core.Entities;
using Core.Enums;
using WebApi.Controllers;
using static WebApi.Dtos.RentalDto;

namespace WebApi.Mappings
{
    public static class RentalMapper
    {
        public static Rental ToEntity(this RentalPostDto rentalDto)
        {
            return new Rental
            {
                CreatedAt = DateTime.UtcNow,
                From = rentalDto.From,
                To = rentalDto.To,
                Note = rentalDto.Note ?? string.Empty,
                Status = rentalDto.Status,
                RenterId = rentalDto.RenterId,
                ItemId = rentalDto.ItemId,
                OwnerId = rentalDto.OwnerId
            };
        }

        public static void UpdateEntity(this RentalPutDto rentalDto, Rental rentalToPut)
        {
            rentalToPut.UpdatedAt = DateTime.UtcNow;
            rentalToPut.RowVersion = rentalDto.RowVersion;
            rentalToPut.From = rentalDto.From;
            rentalToPut.To = rentalDto.To;
            rentalToPut.Note = rentalDto.Note??string.Empty;
            rentalToPut.Status = rentalDto.Status;
            rentalToPut.RenterId = rentalDto.RenterId;
            rentalToPut.ItemId = rentalDto.ItemId;
            // OwnderId sollte nicht verändert werden können
        }
    }
}
