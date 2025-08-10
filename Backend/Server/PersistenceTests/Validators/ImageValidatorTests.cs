using Core.Entities;
using Persistence.Validations.ValidationRules;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistenceTests.Validators
{
    public class ImageValidatorTests : TestBase
    {
        [Fact]
        public async Task ValidateAsync_Throws_When_ImageUrl_ExistsInDb()
        {
            // Arrange
            using var context = CreateDbContext();
            context.Images.Add(new Image { Id = 1, ImageUrl = "https://example.com/image1.jpg" });
            await context.SaveChangesAsync();

            var rule = new ImageUniqueImageUrlRule(context);
            var duplicate = new Image { Id = 2, ImageUrl = "https://example.com/image1.jpg" };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                rule.ValidateAsync(duplicate)
            );
        }

        [Fact]
        public async Task ValidateAsync_Throws_When_ImageUrl_ExistsInMemory()
        {
            // Arrange
            using var context = CreateDbContext();

            var img1 = new Image { Id = 1, ImageUrl = "https://test.com/img.jpg" };
            var img2 = new Image { Id = 2, ImageUrl = "https://test.com/img.jpg" };

            context.Images.Add(img1);
            context.Images.Add(img2); // Noch nicht gespeichert

            var rule = new ImageUniqueImageUrlRule(context);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                rule.ValidateAsync(img2, checkMemory: true)
            );
        }

        [Fact]
        public async Task ValidateAsync_Allows_Unique_ImageUrl()
        {
            // Arrange
            using var context = CreateDbContext();

            var image = new Image { Id = 3, ImageUrl = "https://unique.com/image.jpg" };

            var rule = new ImageUniqueImageUrlRule(context);

            // Act
            var ex = await Record.ExceptionAsync(() => rule.ValidateAsync(image));

            // Assert
            Assert.Null(ex);
        }
    }
}
