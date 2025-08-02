

using Core.Contracts.Validations;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Persistence.Validations.ValidationRules
{
    public class SubCategoryUniqueNameRule(ApplicationDbContext dbContext) : IEntityValidationRule
    {
        public ApplicationDbContext DbContext { get; set; } = dbContext;

        public async Task ValidateAsync(object entity, bool checkMemory = false)
        {
            if (entity is not SubCategory subCategory) return;

            bool subCategoryExistsInMemory = false;

            if (checkMemory)
            {
                subCategoryExistsInMemory = DbContext.ChangeTracker
                    .Entries<SubCategory>()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                    .Select(e => e.Entity)
                    .Any(sc => sc != subCategory && string.Equals(sc.Name, subCategory.Name, StringComparison.OrdinalIgnoreCase));

            }

            bool subCategoryExistsInDb = await DbContext.SubCategories
                    .AnyAsync(sc => sc.Id != subCategory.Id &&
                                    EF.Functions.Like(sc.Name, subCategory.Name));

            if (subCategoryExistsInMemory || subCategoryExistsInDb)
            {
                throw new ValidationException(
                    new ValidationResult("SubCategory mit gleichem Namen existiert bereits", [nameof(SubCategory.Name)]), null, subCategory);
            }

        }
    }
}
