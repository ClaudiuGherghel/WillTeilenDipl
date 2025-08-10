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
    public class CategoryValidatorTests : TestBase
    {
        [Fact]
        public async Task ValidateAsync_Throws_When_CategoryNameExistsInDb()
        {
            // Arrange
            using var context = CreateDbContext();
            context.Categories.Add(new Category { Id = 1, Name = "Elektronik" });
            await context.SaveChangesAsync();

            var rule = new CategoryUniqueNameRule(context);
            var newCategory = new Category { Id = 2, Name = "Elektronik" };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                rule.ValidateAsync(newCategory)
            );
        }

        [Fact]
        public async Task ValidateAsync_Throws_When_CategoryNameExistsInMemory()
        {
            // Arrange
            using var context = CreateDbContext();

            var cat1 = new Category { Id = 1, Name = "Haushalt" };
            var cat2 = new Category { Id = 2, Name = "Haushalt" };

            context.Categories.Add(cat1);
            context.Categories.Add(cat2); // Noch nicht gespeichert

            var rule = new CategoryUniqueNameRule(context);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                rule.ValidateAsync(cat2, checkMemory: true)
            );
        }

        [Fact]
        public async Task ValidateAsync_Allows_Unique_CategoryName()
        {
            // Arrange
            using var context = CreateDbContext();

            var category = new Category { Id = 3, Name = "Sport" };

            var rule = new CategoryUniqueNameRule(context);

            // Act
            var ex = await Record.ExceptionAsync(() => rule.ValidateAsync(category));

            // Assert
            Assert.Null(ex);
        }
    }
}
