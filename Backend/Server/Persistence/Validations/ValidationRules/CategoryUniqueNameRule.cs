using Core.Contracts.Validations;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace Persistence.Validations.ValidationRules
{
    public class CategoryUniqueNameRule(ApplicationDbContext dbContext) : IEntityValidationRule
    {
        public ApplicationDbContext DbContext { get; set; } = dbContext;

        public async Task ValidateAsync(object entity, bool checkMemory = false)
        {
            if (entity is not Category category) return;


            bool categoryExistsInMemory = false;

            if (checkMemory)
            {
                categoryExistsInMemory = DbContext.ChangeTracker
                    .Entries<Category>()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                    .Select(e => e.Entity)
                    .Any(c => c != category && string.Equals(c.Name, category.Name, StringComparison.OrdinalIgnoreCase));
            }

            bool categoryExistsInDb = await DbContext.Categories
                    .AnyAsync(c => c.Id != category.Id &&
                                   EF.Functions.Like(c.Name, category.Name));

                if (categoryExistsInMemory || categoryExistsInDb)
                {
                    throw new ValidationException(
                        new ValidationResult("Category mit gleichem Namen existiert bereits", [nameof(Category.Name)]), null, category);
                }
            
        }
    }
}
