using Core.Validations.Helper;
using System.ComponentModel.DataAnnotations;


namespace Core.Validations.Annotations
{
    public class OptionalPhoneAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var phone = value as string;
            return ValidationHelper.ValidateOptionalPhone(phone, validationContext.DisplayName);
        }

    }
}
