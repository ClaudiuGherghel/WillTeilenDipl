using Core.Entities;
using System;
using WebApi.Mappings;
using static WebApi.Dtos.SubCategoryDto;

namespace WebApiTests.Mappings
{
    public class SubCategoryMapperTests
    {
        [Fact]
        public void SubCategory_ToEntity_MapsCorrectly()
        {
            // Arrange
            var dto = new SubCategoryPostDto("Test", 1);

            // Act
            var entity = dto.ToEntity();

            // Assert
            Assert.Equal(dto.Name, entity.Name);
            Assert.Equal(dto.CategoryId, entity.CategoryId);
            Assert.True((DateTime.UtcNow - entity.CreatedAt).TotalSeconds < 2);
        }

        [Fact]
        public void SubCategory_ToEntity_SetsEmptyStrings_WhenFieldsAreNull()
        {
            // Arrange
            var dto = new SubCategoryPostDto(null!, 1);

            // Act
            var entity = dto.ToEntity();

            // Assert
            Assert.Equal(string.Empty, entity.Name);
        }

        [Fact]
        public void SubCategory_UpdateEntity_MapsCorrectly()
        {
            // Arrange
            var entity = new SubCategory();
            var dto = new SubCategoryPutDto(1, [1, 2], "Updated", 2);

            // Act
            dto.UpdateEntity(entity);

            // Assert
            Assert.Equal(dto.Name, entity.Name);
            Assert.Equal(dto.CategoryId, entity.CategoryId);
            Assert.Equal(dto.RowVersion, entity.RowVersion);
            Assert.NotNull(entity.UpdatedAt);
            Assert.True((DateTime.UtcNow - entity.UpdatedAt!.Value).TotalSeconds < 2);
        }

        [Fact]
        public void SubCategory_UpdateEntity_SetsEmptyStrings_WhenFieldsAreNull()
        {
            // Arrange
            var entity = new SubCategory
            {
                Name = "Old"
            };

            var dto = new SubCategoryPutDto(1, [1, 2, 3], null!, 2);

            // Act
            dto.UpdateEntity(entity);

            // Assert
            Assert.Equal(string.Empty, entity.Name);
        }
    }
}
