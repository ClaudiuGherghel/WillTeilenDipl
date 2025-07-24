using Core.Entities;
using WebApi.Controllers;

namespace WebApi.Mappings
{
    public static class ImageMapper
    {
        public static Image ToEntity(this ImagePostDto imageDto)
        {
            return new Image
            {
                AltText = imageDto.AltText ?? string.Empty,
                ImageUrl = imageDto.Url ?? string.Empty,
            };
        }


        public static void UpdateEntity(this ImagePutDto imageDto, Image imageToPut)
        {
            imageToPut.ImageUrl = imageDto.Url ?? string.Empty;
            imageToPut.AltText = imageDto.AltText ?? string.Empty;
            imageToPut.ItemId = imageDto.ItemId;
            imageToPut.RowVersion = imageDto.RowVersion;
        }
    }
}
