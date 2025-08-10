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
            var dto = new ItemPostDto("Item", "Desc", true, "DE", "BY", "12345", "Munich", "Street", 100, 10, 50, 0, 0, 1, 2);
            
            // Act
            var entity = dto.ToEntity();

            // Assert
            Assert.Equal(dto.Name, entity.Name);
            Assert.Equal(dto.Description, entity.Description);
            Assert.Equal(dto.IsAvailable, entity.IsAvailable);
            Assert.Equal(dto.Country, entity.Country);
            Assert.Equal(dto.State, entity.State);
            Assert.Equal(dto.PostalCode, entity.PostalCode);
            Assert.Equal(dto.Place, entity.Place);
            Assert.Equal(dto.Address, entity.Address);
            Assert.Equal(dto.Price, entity.Price);
            Assert.Equal(dto.Stock, entity.Stock);
            Assert.Equal(dto.Deposit, entity.Deposit);
            Assert.Equal(dto.RentalType, entity.RentalType);
            Assert.Equal(dto.ItemCondition, entity.ItemCondition);
            Assert.Equal(dto.SubCategoryId, entity.SubCategoryId);
            Assert.Equal(dto.OwnerId, entity.OwnerId);
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
                null!, // Country
                null!, // State
                null!, // PostalCode
                null!, // Place
                null!, // Address
                100,
                10,
                50,
                0,
                0,
                1,
                2
            );

            // Act
            var entity = dto.ToEntity();

            // Assert
            Assert.Equal(string.Empty, entity.Name);
            Assert.Equal(string.Empty, entity.Description);
            Assert.Equal(string.Empty, entity.Country);
            Assert.Equal(string.Empty, entity.State);
            Assert.Equal(string.Empty, entity.PostalCode);
            Assert.Equal(string.Empty, entity.Place);
            Assert.Equal(string.Empty, entity.Address);
        }



        [Fact]
        public void Item_UpdateEntity_MapsCorrectly()
        {
            // Arrange
            var entity = new Item();
            var dto = new ItemPutDto(1, [1, 2], "Item", "Desc", true, "DE", "BY", "12345", "Munich", "Street", 100, 10, 50, 0, 0, 1, 2);
            
            // Act
            dto.UpdateEntity(entity);

            // Assert
            Assert.Equal(dto.Name, entity.Name);
            Assert.Equal(dto.Description, entity.Description);
            Assert.Equal(dto.IsAvailable, entity.IsAvailable);
            Assert.Equal(dto.Country, entity.Country);
            Assert.Equal(dto.State, entity.State);
            Assert.Equal(dto.PostalCode, entity.PostalCode);
            Assert.Equal(dto.Place, entity.Place);
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
        }



        [Fact]
        public void Item_UpdateEntity_SetsEmptyStrings_WhenFieldsAreNull()
        {
            // Arrange
            var entity = new Item
            {
                Name = "Old",
                Description = "Old",
                Country = "Old",
                State = "Old",
                PostalCode = "Old",
                Place = "Old",
                Address = "Old"
            };

            var dto = new ItemPutDto(
                1,
                [1, 2, 3],
                null!, // Name
                null!, // Description
                true,
                null!, // Country
                null!, // State
                null!, // PostalCode
                null!, // Place
                null!, // Address
                100,
                10,
                50,
                0,
                0,
                1,
                2
            );

            // Act
            dto.UpdateEntity(entity);

            // Assert
            Assert.Equal(string.Empty, entity.Name);
            Assert.Equal(string.Empty, entity.Description);
            Assert.Equal(string.Empty, entity.Country);
            Assert.Equal(string.Empty, entity.State);
            Assert.Equal(string.Empty, entity.PostalCode);
            Assert.Equal(string.Empty, entity.Place);
            Assert.Equal(string.Empty, entity.Address);
        }


    }
}
