using Core.Enums;
using Core.Validations.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Validations.Annotations
{
    public class NotUnknownEnum(object unknownValue) : ValidationAttribute
    {
        private readonly object _unknownValue = unknownValue;


        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return new ValidationResult($"{validationContext.DisplayName} darf nicht leer sein.");

            var enumType = value.GetType();
            if (!enumType.IsEnum)
            {
                return new ValidationResult($"{validationContext.DisplayName} ist kein Enum-Typ.");
            }

            // Rufe Helper-Methode auf, cast zu generischem Enum via reflection
            var method = typeof(ValidationHelper).GetMethod(nameof(ValidationHelper.ValidateNotUnknownEnum))!;
            var genericMethod = method.MakeGenericMethod(enumType);

            var result = (ValidationResult?)genericMethod.Invoke(null, [value, _unknownValue, validationContext.DisplayName])!;

            return result;
        }
    }
}
