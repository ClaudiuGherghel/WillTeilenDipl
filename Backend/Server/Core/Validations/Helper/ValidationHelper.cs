using System.ComponentModel.DataAnnotations;


namespace Core.Validations.Helper
{
    public static class ValidationHelper
    {
        public static ValidationResult? ValidateNotMinValue(DateTime date, string fieldDisplayName)
        {
            //Angular Date-MinValue
            DateTime angularMinDate = new(1970, 1, 1);


            if (date == DateTime.MinValue || date == angularMinDate)
            {
                return new ValidationResult($"{fieldDisplayName} muss eingegeben werden.");
            }
            return ValidationResult.Success;
        }

        public static ValidationResult? ValidateNotInFuture(DateTime date, string fieldDisplayName)
        {
            if (date > DateTime.UtcNow)
            {
                return new ValidationResult($"{fieldDisplayName} darf nicht in der Zukunft liegen.");
            }
            return ValidationResult.Success;
        }

        public static ValidationResult? ValidateFromBeforeTo(DateTime fromDate, DateTime? toDate, string fromFieldName, string toFieldName)
        {
            if (toDate!= null && fromDate > toDate)
            {
                return new ValidationResult($"{fromFieldName} darf nicht nach {toFieldName} liegen.");
            }
            return ValidationResult.Success;
        }

        public static ValidationResult? ValidateOptionalPhone(string? phone, string fieldDisplayName = "Telefonnummer")
        {
            if (string.IsNullOrWhiteSpace(phone))
                return ValidationResult.Success;

            var phoneAttribute = new PhoneAttribute();
            if (phoneAttribute.IsValid(phone))
                return ValidationResult.Success;

            return new ValidationResult($"{fieldDisplayName} ist nicht gültig.");
        }



        /*Die Einschränkung where TEnum : struct, Enum sagt: TEnum muss ein Werttyp (struct) sein, und es muss ein Enum sein.
         *  TEnum value — der aktuelle Wert, den du validieren willst.
            TEnum unknownValue — der "ungültige" Wert, z.B. RentalType.Unknown.
            string fieldDisplayName — der Name des Feldes (für die Fehlermeldung).
        */
        public static ValidationResult? ValidateNotUnknownEnum<TEnum>(TEnum value, TEnum unknownValue, string fieldDisplayName)
        where TEnum : struct, Enum
        {
            if (value.Equals(unknownValue))
            {
                return new ValidationResult($"{fieldDisplayName} darf nicht den Wert '{unknownValue}' haben.");
            }
            return ValidationResult.Success;
        }
    }

}
