

using Core.Contracts.Validations;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Persistence.Validations.ValidationRules
{
    public class UserUniqueUserNameRule(ApplicationDbContext dbContext) : IEntityValidationRule
    {
        public ApplicationDbContext DbContext { get; set; } = dbContext;

        public async Task ValidateAsync(object entity, bool checkMemory = false)
        {
            if (entity is not User user) return;

            bool usernameExistsInMemory = false;

            if (checkMemory)
            {
                usernameExistsInMemory = DbContext.ChangeTracker
                    .Entries<User>()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                    .Select(e => e.Entity)
                    .Any(u => u != user && string.Equals(u.Username, user.Username, StringComparison.OrdinalIgnoreCase));
            }

            bool usernameExistsInDb = await DbContext.Users
                    .AnyAsync(u => u != user && EF.Functions.Like(u.Username, user.Username));


            if (usernameExistsInMemory || usernameExistsInDb)
            {
                throw new ValidationException(
                    new ValidationResult("Benutzername existiert bereits", [nameof(User.Username)]), null, user);
            }

        }
        
    }
}
