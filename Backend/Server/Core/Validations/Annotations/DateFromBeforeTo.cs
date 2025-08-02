using Core.Validations.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Validations.Annotations
{
    // Das Attribut darf nur auf Klassen angewendet werden (nicht auf Methoden, Properties usw.).
    // Man darf das Attribut mehrmals auf derselben Klasse anwenden (z. B. für mehrere From/To-Paare).
    /* Bsp.:    [DateFromBeforeTo("StartDate", "EndDate")]
                [DateFromBeforeTo("Anmeldung", "Abmeldung")]
    */

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)] //optional, dient als Einstränkung um Fehler zu reduzieren
    public class DateFromBeforeToAttribute(string fromPropertyName, string toPropertyName) : ValidationAttribute
    {
        public string FromPropertyName { get; } = fromPropertyName;
        public string ToPropertyName { get; } = toPropertyName;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var type = value.GetType();
            var fromProp = type.GetProperty(FromPropertyName);
            var toProp = type.GetProperty(ToPropertyName);

            if (fromProp == null)
                return new ValidationResult($"Property '{FromPropertyName}' nicht gefunden.");
            if (toProp == null)
                return new ValidationResult($"Property '{ToPropertyName}' nicht gefunden.");

            if (fromProp.PropertyType != typeof(DateTime) ||
                toProp.PropertyType != typeof(DateTime) && toProp.PropertyType != typeof(DateTime?))
                return new ValidationResult("Beide Properties müssen vom Typ DateTime sein.");

            var fromValue = (DateTime)fromProp.GetValue(value)!;
            var toValue = (DateTime?)toProp.GetValue(value);

            var result = ValidationHelper.ValidateFromBeforeTo(fromValue, toValue, FromPropertyName, ToPropertyName);
            if (result != ValidationResult.Success)
            {
                return result;
            }

            return ValidationResult.Success;
        }

    }

}
