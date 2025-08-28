using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Mappings;
using static WebApi.Dtos.ItemDto;

namespace WebApiTests.Mappings
{
    public class ItemMapperTests
    {
        [Fact]
        public void Item_ToEntity_MapsCorrectly()
        {
            // Arrange
            var dto = new ItemPostDto("Item", "Desc", true,"Street", 100, 10, 50, 0, 0, 1, 2,1);
            
            // Act
            var entity = dto.ToEntity();

            // Assert
            Assert.Equal(dto.Name, entity.Name);
            Assert.Equal(dto.Description, entity.Description);
            Assert.Equal(dto.IsAvailable, entity.IsAvailable);
            Assert.Equal(dto.Address, entity.Address);
            Assert.Equal(dto.Price, entity.Price);
            Assert.Equal(dto.Stock, entity.Stock);
            Assert.Equal(dto.Deposit, entity.Deposit);
            Assert.Equal(dto.RentalType, entity.RentalType);
            Assert.Equal(dto.ItemCondition, entity.ItemCondition);
            Assert.Equal(dto.SubCategoryId, entity.SubCategoryId);
            Assert.Equal(dto.OwnerId, entity.OwnerId);
            Assert.Equal(dto.GeoPostalId, entity.GeoPostalId);
            Assert.True((DateTime.UtcNow - entity.CreatedAt).TotalSeconds < 2);
        }

        [Fact]
        public void Item_ToEntity_SetsEmptyStrings_WhenFieldsAreNull()
        {
            // Arrange
            var dto = new ItemPostDto(
                null!, // Name
                null!, // Description
                true,
                null!, // Address
                100,
                10,
                50,
                0,
                0,
                1,
                2,
                1
            );

            // Act
            var entity = dto.ToEntity();

            // Assert
            Assert.Equal(string.Empty, entity.Name);
            Assert.Equal(string.Empty, entity.Description);
            Assert.Equal(string.Empty, entity.Address);
        }



        [Fact]
        public void Item_UpdateEntity_MapsCorrectly()
        {
            // Arrange
            var entity = new Item();
            var dto = new ItemPutDto(1, [1, 2], "Item", "Desc", true, "Street", 100, 10, 50, 0, 0, 1, 2, 1);
            
            // Act
            dto.UpdateEntity(entity);

            // Assert
            Assert.Equal(dto.Name, entity.Name);
            Assert.Equal(dto.Description, entity.Description);
            Assert.Equal(dto.IsAvailable, entity.IsAvailable);
            Assert.Equal(dto.Address, entity.Address);
            Assert.Equal(dto.Price, entity.Price);
            Assert.Equal(dto.Stock, entity.Stock);
            Assert.Equal(dto.Deposit, entity.Deposit);
            Assert.Equal(dto.RentalType, entity.RentalType);
            Assert.Equal(dto.ItemCondition, entity.ItemCondition);
            Assert.Equal(dto.SubCategoryId, entity.SubCategoryId);
            Assert.Equal(dto.OwnerId, entity.OwnerId);
            Assert.Equal(dto.RowVersion, entity.RowVersion);
            Assert.NotNull(entity.UpdatedAt);
            Assert.True((DateTime.UtcNow - entity.UpdatedAt!.Value).TotalSeconds < 2);
            Assert.Equal(dto.GeoPostalId, entity.GeoPostalId);
        }



        [Fact]
        public void Item_UpdateEntity_SetsEmptyStrings_WhenFieldsAreNull()
        {
            // Arrange
            var entity = new Item
            {
                Name = "Old",
                Description = "Old",
                Address = "Old"
            };

            var dto = new ItemPutDto(
                1,
                [1, 2, 3],
                null!, // Name
                null!, // Description
                true,
                null!, // Address
                100,
                10,
                50,
                0,
                0,
                1,
                2,
                1
            );

            // Act
            dto.UpdateEntity(entity);

            // Assert
            Assert.Equal(string.Empty, entity.Name);
            Assert.Equal(string.Empty, entity.Description);
            Assert.Equal(string.Empty, entity.Address);
        }


    }
}
