using Core.Entities;
using WebApi.Controllers;
using static WebApi.Dtos.ImageDto;

namespace WebApi.Mappings
{
    public static class ImageMapper
    {
        public static Image ToEntity(this ImagePostDto imageDto)
        {
            return new Image
            {
                CreatedAt = DateTime.UtcNow,
                ImageUrl = imageDto.ImageUrl,
                AltText = imageDto.AltText ?? string.Empty,
                DisplayOrder = imageDto.DisplayOrder,
                IsMainImage = imageDto.IsMainImage,
                ItemId = imageDto.ItemId,
            };
        }


        public static void UpdateEntity(this ImagePutDto imageDto, Image imageToPut)
        {
            imageToPut.UpdatedAt = DateTime.UtcNow;
            imageToPut.RowVersion = imageDto.RowVersion;
            imageToPut.ImageUrl = imageDto.ImageUrl ?? string.Empty;
            imageToPut.AltText = imageDto.AltText ?? string.Empty;
            imageToPut.DisplayOrder = imageDto.DisplayOrder;
            imageToPut.IsMainImage = imageDto.IsMainImage;
            imageToPut.ItemId = imageDto.ItemId;
        }
    }
}
