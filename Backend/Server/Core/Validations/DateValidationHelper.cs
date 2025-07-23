using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Validations
{
    public static class DateValidationHelper
    {
        public static ValidationResult? ValidateNotMinValue(DateTime date, string fieldDisplayName)
        {
            if (date == DateTime.MinValue)
            {
                return new ValidationResult($"{fieldDisplayName} muss eingegeben werden.");
            }
            return ValidationResult.Success;
        }

        public static ValidationResult? ValidateNotInFuture(DateTime date, string fieldDisplayName)
        {
            if (date > DateTime.Today)
            {
                return new ValidationResult($"{fieldDisplayName} darf nicht in der Zukunft liegen.");
            }
            return ValidationResult.Success;
        }

        public static ValidationResult? ValidateFromBeforeTo(DateTime fromDate, DateTime toDate, string fromFieldName, string toFieldName)
        {
            if (fromDate > toDate)
            {
                return new ValidationResult($"{fromFieldName} darf nicht nach {toFieldName} liegen.");
            }
            return ValidationResult.Success;
        }
    }

}
