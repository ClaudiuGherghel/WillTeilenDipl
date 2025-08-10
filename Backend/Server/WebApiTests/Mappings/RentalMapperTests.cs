using Core.Entities;
using System;
using WebApi.Mappings;
using static WebApi.Dtos.RentalDto;

namespace WebApiTests.Mappings
{
    public class RentalMapperTests
    {
        [Fact]
        public void Rental_ToEntity_MapsCorrectly()
        {
            // Arrange
            var from = DateTime.UtcNow;
            var to = from.AddDays(3);
            var dto = new RentalPostDto(from, to, "note", 0, 1, 2, 3);

            // Act
            var entity = dto.ToEntity();

            // Assert
            Assert.Equal(dto.Note, entity.Note);
            Assert.Equal(dto.From, entity.From);
            Assert.Equal(dto.To, entity.To);
            Assert.Equal(dto.Status, entity.Status);
            Assert.Equal(dto.ItemId, entity.ItemId);
            Assert.Equal(dto.RenterId, entity.RenterId);
            Assert.Equal(dto.OwnerId, entity.OwnerId);
            Assert.True((DateTime.UtcNow - entity.CreatedAt).TotalSeconds < 2);
        }

        [Fact]
        public void Rental_ToEntity_SetsEmptyStrings_WhenFieldsAreNull()
        {
            // Arrange
            var dto = new RentalPostDto(DateTime.UtcNow, DateTime.UtcNow.AddDays(1), null!, 0, 1, 2, 3);

            // Act
            var entity = dto.ToEntity();

            // Assert
            Assert.Equal(string.Empty, entity.Note);
        }

        [Fact]
        public void Rental_UpdateEntity_MapsCorrectly()
        {
            // Arrange
            var entity = new Rental();
            var from = DateTime.UtcNow;
            var to = from.AddDays(1);
            var dto = new RentalPutDto(1, [1, 2], from, to, "Updated", 0, 1, 2, 3);

            // Act
            dto.UpdateEntity(entity);

            // Assert
            Assert.Equal(dto.Note, entity.Note);
            Assert.Equal(dto.From, entity.From);
            Assert.Equal(dto.To, entity.To);
            Assert.Equal(dto.Status, entity.Status);
            Assert.Equal(dto.ItemId, entity.ItemId);
            Assert.Equal(dto.RenterId, entity.RenterId);
            // OwnderId sollte nicht verändert werden können
            Assert.NotEqual(dto.OwnerId, entity.OwnerId);
            Assert.Equal(dto.RowVersion, entity.RowVersion);
            Assert.NotNull(entity.UpdatedAt);
            Assert.True((DateTime.UtcNow - entity.UpdatedAt!.Value).TotalSeconds < 2);
        }

        [Fact]
        public void Rental_UpdateEntity_SetsEmptyStrings_WhenFieldsAreNull()
        {
            // Arrange
            var entity = new Rental
            {
                Note = "Old"
            };

            var dto = new RentalPutDto(1, [1, 2, 3], DateTime.UtcNow, DateTime.UtcNow.AddDays(1), null!, 0, 1, 2, 3);

            // Act
            dto.UpdateEntity(entity);

            // Assert
            Assert.Equal(string.Empty, entity.Note);
        }
    }
}
