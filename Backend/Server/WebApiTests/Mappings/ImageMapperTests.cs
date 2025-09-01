using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Mappings;
using static WebApi.Dtos.ImageDto;

namespace WebApiTests.Mappings
{
    public class ImageMapperTests
    {
        [Fact]
        public void Image_ToEntity_MapsCorrectly()
        {
            // Arrange
            var dto = new ImagePostDto(null!, null!, "alt", 1, true, 99);
            
            // Act
            var entity = dto.ToEntity();

            // Assert
            Assert.Equal(dto.AltText, entity.AltText);
            Assert.Equal(dto.DisplayOrder, entity.DisplayOrder);
            Assert.Equal(dto.IsMainImage, entity.IsMainImage);
            Assert.Equal(dto.ItemId, entity.ItemId);
            Assert.True((DateTime.UtcNow - entity.CreatedAt).TotalSeconds < 2);
        }

        [Fact]
        public void Image_ToEntity_SetsEmptyStrings_WhenFieldsAreNull()
        {
            // Arrange
            var dto = new ImagePostDto(null!,"", null!, 1, true, 99);

            // Act
            var entity = dto.ToEntity();

            // Assert
            Assert.Equal(string.Empty, entity.ImageUrl);
            Assert.Equal(string.Empty, entity.AltText);
        }


        [Fact]
        public void Image_UpdateEntity_MapsCorrectly()
        {
            //Arrange
            var entity = new Image();
            var dto = new ImagePutDto(1, [1, 2], "url", "alt", 1, true, 99);
            
            // Act
            dto.UpdateEntity(entity);

            // Assert
            Assert.Equal(dto.ImageUrl, entity.ImageUrl);
            Assert.Equal(dto.AltText, entity.AltText);
            Assert.Equal(dto.DisplayOrder, entity.DisplayOrder);
            Assert.Equal(dto.IsMainImage, entity.IsMainImage);
            Assert.Equal(dto.ItemId, entity.ItemId);
            Assert.Equal(dto.RowVersion, entity.RowVersion);
            Assert.NotNull(entity.UpdatedAt);
            Assert.True((DateTime.UtcNow - entity.UpdatedAt!.Value).TotalSeconds < 2);
        }

        [Fact]
        public void Image_UpdateEntity_SetsEmptyStrings_WhenFieldsAreNull()
        {
            // Arrange
            var entity = new Image
            {
                ImageUrl = "old",
                AltText = "old",
            };

            var dto = new ImagePutDto(1, [1, 2, 3], null!, null!, 1, true, 99);

            // Act
            dto.UpdateEntity(entity);

            // Assert
            Assert.Equal(string.Empty, entity.ImageUrl);
            Assert.Equal(string.Empty, entity.AltText);
        }


    }
}
