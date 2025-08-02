using Core.Validations.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Validations.Annotations
{
    public class OptionalPhone : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var phone = value as string;
            return ValidationHelper.ValidateOptionalPhone(phone, validationContext.DisplayName);
        }

    }
}
