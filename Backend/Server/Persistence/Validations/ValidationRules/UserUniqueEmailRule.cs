using Core.Contracts.Validations;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace Persistence.Validations.ValidationRules
{
    public class UserUniqueEmailRule(ApplicationDbContext dbContext) : IEntityValidationRule
    {
        public ApplicationDbContext DbContext { get; set; } = dbContext;

        public async Task ValidateAsync(object entity, bool checkMemory = false)
        {
            if (entity is not User user) return;

            bool emailExistsInMemory = false;
            List<User> list = [];
            if (checkMemory)
            {
                emailExistsInMemory = DbContext.ChangeTracker
                    .Entries<User>()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                    .Select(e => e.Entity)
                    .Any(u => u != user && string.Equals(u.Email, user.Email, StringComparison.OrdinalIgnoreCase));

                list = DbContext.ChangeTracker
                   .Entries<User>()
                   .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                   .Select(e => e.Entity)
                   .ToList();
            }

            bool emailExistsInDb = await DbContext.Users
                .AnyAsync(u => u != user && EF.Functions.Like(u.Email, user.Email));

            if (emailExistsInMemory || emailExistsInDb)
            {
                throw new ValidationException(
                    new ValidationResult($"E-Mail {user.Email} existiert bereits", [nameof(User.Email)]), null, user);
            }
        }
    }
}
