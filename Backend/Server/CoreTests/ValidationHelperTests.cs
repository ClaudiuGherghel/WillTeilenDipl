using Core.Entities;
using Core.Enums;
using Core.Validations.Helper;
using System.ComponentModel.DataAnnotations;


namespace CoreTests
{
    public class ValidationHelperTests
    {

        [Fact]
        public void ValidateNotMinValue_Should_ReturnError_IfDateIsMinValue()
        {
            var result = ValidationHelper.ValidateNotMinValue(DateTime.MinValue, "Start");

            Assert.NotNull(result);
            Assert.Contains("Start muss eingegeben werden", result!.ErrorMessage);
        }

        [Fact]
        public void ValidateNotInFuture_Should_ReturnError_IfDateIsInFuture()
        {
            var future = DateTime.UtcNow.AddDays(1);
            var result = ValidationHelper.ValidateNotInFuture(future, "Datum");

            Assert.NotNull(result);
            Assert.Contains("Datum darf nicht in der Zukunft liegen", result!.ErrorMessage);
        }

        [Fact]
        public void ValidateFromBeforeTo_Should_ReturnError_IfFromIsAfterTo()
        {
            var from = DateTime.UtcNow.AddDays(1);
            var to = DateTime.UtcNow;

            var result = ValidationHelper.ValidateFromBeforeTo(from, to, "Start", "Ende");

            Assert.NotNull(result);
            Assert.Contains("Start darf nicht nach Ende liegen", result!.ErrorMessage);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ValidateOptionalPhone_Should_Pass_IfEmpty(string? phone)
        {
            var result = ValidationHelper.ValidateOptionalPhone(phone);

            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void ValidateOptionalPhone_Should_ReturnError_IfInvalid()
        {
            var result = ValidationHelper.ValidateOptionalPhone("123abc");

            Assert.NotNull(result);
            Assert.Contains("ist nicht gültig", result!.ErrorMessage);
        }


        [Fact]
        public void ValidateNotUnknownEnum_Should_ReturnError_IfValueIsUnknown()
        {
            var value = RentalType.Unknown;
            var unknown = RentalType.Unknown;
            var field = "RentalType";

            var result = ValidationHelper.ValidateNotUnknownEnum(value, unknown, field);

            Assert.NotNull(result);
            Assert.Equal($"{field} darf nicht den Wert '{unknown}' haben.", result!.ErrorMessage);
        }

        [Fact]
        public void ValidateNotUnknownEnum_Should_ReturnSuccess_IfValueIsValid()
        {
            var value = RentalType.Privat; // <- gültiger Wert
            var unknown = RentalType.Unknown;
            var field = "RentalType";

            var result = ValidationHelper.ValidateNotUnknownEnum(value, unknown, field);

            Assert.Equal(ValidationResult.Success, result);
        }
    }
}
