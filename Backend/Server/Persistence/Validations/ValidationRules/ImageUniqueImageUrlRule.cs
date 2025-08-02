

using Core.Contracts.Validations;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Persistence.Validations.ValidationRules
{
    public class ImageUniqueImageUrlRule(ApplicationDbContext dbContext) : IEntityValidationRule 
    {
        public ApplicationDbContext DbContext { get; set; } = dbContext;

        public async Task ValidateAsync(object entity, bool checkMemory = false)
        {
            if (entity is not Image image) return;

            bool imageExistsInMemory = false;


            if (checkMemory)
            {
                imageExistsInMemory = DbContext.ChangeTracker
                    .Entries<Image>()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                    .Select(e => e.Entity)
                    .Any(i => i != image && string.Equals(i.ImageUrl, image.ImageUrl, StringComparison.OrdinalIgnoreCase));
            }

            bool imageExistsInDb = await DbContext.Images
                    .AnyAsync(img => img.Id != image.Id &&
                                     EF.Functions.Like(img.ImageUrl, image.ImageUrl));

                if (imageExistsInMemory || imageExistsInDb)
                {
                    throw new ValidationException(
                        new ValidationResult("Image mit der gleichen URL existiert bereits", [nameof(Image.ImageUrl)]), null, image);
                }
            
        }
    }
}
