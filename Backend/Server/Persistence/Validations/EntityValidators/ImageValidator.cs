
using Core.Contracts.Validations;
using Persistence.Validations.ValidationRules;

namespace Persistence.Validations.EntityValidators
{
    public class ImageValidator(ApplicationDbContext dbContext) : IEntityValidator
    {
        private readonly List<IEntityValidationRule> _rules =
        [
            new ImageUniqueImageUrlRule(dbContext)
            // weitere Regeln hier ergänzen
        ];

        public async Task ValidateAsync(object entity, bool checkMemory = false)
        {
            foreach (var rule in _rules)
            {
                await rule.ValidateAsync(entity, checkMemory);
            }
        }
    }
}
