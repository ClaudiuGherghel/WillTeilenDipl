using Core.Validations.Helper;
using System.ComponentModel.DataAnnotations;

namespace Core.Validations.Annotations
{

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)] //optional, dient als Einstränkung um Fehler zu reduzieren
    public class DateNotMinValueAttribute(string propertyName) : ValidationAttribute
    {
        public string PropertyName { get; } = propertyName;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            ArgumentNullException.ThrowIfNull(validationContext);

            var instance = validationContext.ObjectInstance;
            var property = instance.GetType().GetProperty(PropertyName);

            if (property == null || property.PropertyType != typeof(DateTime))
            {
                return new ValidationResult($"Property '{PropertyName}' ist ungültig.");
            }

            var dateValue = (DateTime)property.GetValue(instance)!;

            var result = ValidationHelper.ValidateNotMinValue(dateValue, PropertyName);
            if (result != ValidationResult.Success)
            {
                return result;
            }
            return ValidationResult.Success;
        }
    }
}
