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
    public class SubCategoryValidatorTests : TestBase
    {
        [Fact]
        public async Task ValidateAsync_Throws_When_SubCategoryNameExistsInDb()
        {
            // Arrange
            using var context = CreateDbContext();
            context.SubCategories.Add(new SubCategory { Id = 1, Name = "Notebooks" });
            await context.SaveChangesAsync();

            var rule = new SubCategoryUniqueNameRule(context);
            var newSubCategory = new SubCategory { Id = 2, Name = "Notebooks" };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                rule.ValidateAsync(newSubCategory)
            );
        }

        [Fact]
        public async Task ValidateAsync_Throws_When_SubCategoryNameExistsInMemory()
        {
            // Arrange
            using var context = CreateDbContext();

            var sc1 = new SubCategory { Id = 1, Name = "Küchengeräte" };
            var sc2 = new SubCategory { Id = 2, Name = "Küchengeräte" };

            context.SubCategories.Add(sc1);
            context.SubCategories.Add(sc2); // Noch nicht gespeichert

            var rule = new SubCategoryUniqueNameRule(context);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                rule.ValidateAsync(sc2, checkMemory: true)
            );
        }

        [Fact]
        public async Task ValidateAsync_Allows_Unique_SubCategoryName()
        {
            // Arrange
            using var context = CreateDbContext();

            var subCategory = new SubCategory { Id = 3, Name = "Werkzeuge" };

            var rule = new SubCategoryUniqueNameRule(context);

            // Act
            var ex = await Record.ExceptionAsync(() => rule.ValidateAsync(subCategory));

            // Assert
            Assert.Null(ex);
        }
    }
}
