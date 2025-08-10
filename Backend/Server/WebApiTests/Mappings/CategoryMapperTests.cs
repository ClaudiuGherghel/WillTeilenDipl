using Core.Entities;
using WebApi.Mappings;
using static WebApi.Dtos.CategoryDto;

namespace WebApiTests.Mappings
{
    public class CategoryMapperTests
    {
        [Fact]
        public void Category_ToEntity_MapsCorrectly()
        {
            // Arrange
            var dto = new CategoryPostDto("TestKategorie");

            // Act
            var entity = dto.ToEntity();

            // Assert
            Assert.NotNull(entity);
            Assert.Equal(dto.Name, entity.Name);
            Assert.True((DateTime.UtcNow - entity.CreatedAt).TotalSeconds < 2);
        }

        [Fact]
        public void Category_ToEntity_SetsEmptyString_WhenNameIsNull()
        {
            // Arrange
            var dto = new CategoryPostDto(null!); // null explizit gesetzt

            // Act
            var entity = dto.ToEntity();

            // Assert
            Assert.NotNull(entity);
            Assert.Equal(string.Empty, entity.Name);
        }

        [Fact]
        public void Category_UpdateEntity_MapsCorrectly()
        {
            // Arrange
            var entity = new Category
            {
                Id = 1,
                Name = "OldName",
                RowVersion = [1, 2, 3]
            };

            byte[] newRowVersion = [9, 9, 9];
            var dto = new CategoryPutDto(1, newRowVersion, "UpdatedName");

            // Act
            dto.UpdateEntity(entity);

            // Assert
            Assert.Equal(dto.Name, entity.Name);
            Assert.Equal(dto.RowVersion, entity.RowVersion);
            Assert.True((DateTime.UtcNow - entity.UpdatedAt!.Value).TotalSeconds < 2);
        }

        [Fact]
        public void Category_UpdateEntity_SetsEmptyString_WhenNameIsNull()
        {
            // Arrange
            var entity = new Category
            {
                Id = 1,
                Name = "OldName",
                RowVersion = [1, 2, 3]
            };

            var dto = new CategoryPutDto(1, null, null!); // Name null gesetzt

            // Act
            dto.UpdateEntity(entity);

            // Assert
            Assert.Equal(string.Empty, entity.Name);
        }
    }
}
