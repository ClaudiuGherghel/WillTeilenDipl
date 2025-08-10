using Core.Enums;
using Core.Validations.Annotations;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Dtos
{
    public class UserDto
    {
        public record UserPostDto(
            [Required(AllowEmptyStrings = false, ErrorMessage = "Benutzername muss eingegeben werden")]
            [StringLength(50, MinimumLength = 2, ErrorMessage = "Benutzername muss zwischen 2 und 50 Zeichen lang sein")]
            string UserName,

            [Required(AllowEmptyStrings = false, ErrorMessage = "Passwort muss eingegeben werden")]
            string Password,

            [Required(AllowEmptyStrings = false, ErrorMessage = "E-Mail muss eingegeben werden")]
            [StringLength(100, ErrorMessage = "E-Mail darf maximal 100 Zeichen lang sein")]
            [EmailAddress(ErrorMessage = "E-Mail ist nicht gültig")]
            string Email,

            [Required(AllowEmptyStrings = false, ErrorMessage = "Vorname muss eingegeben werden")]
            [StringLength(50, MinimumLength = 2, ErrorMessage = "Vorname muss zwischen 2 und 50 Zeichen lang sein")]
            string FirstName,

            [Required(AllowEmptyStrings = false, ErrorMessage = "Nachname muss eingegeben werden")]
            [StringLength(50, MinimumLength = 2, ErrorMessage = "Nachname muss zwischen 2 und 50 Zeichen lang sein")]
            string LastName,

            [DateNotMinValueAttribute(nameof(BirthDate))] // 1.
            [DateNotInFutureAttribute(nameof(BirthDate))] // 2.
            DateTime BirthDate,

            Roles Role,

            [StringLength(100, ErrorMessage = "Land darf maximal 100 Zeichen lang sein")]
            string Country,

            [StringLength(20, ErrorMessage = "Postleitzahl darf maximal 20 Zeichen lang sein")]
            string PostalCode,

            [StringLength(100, ErrorMessage = "Ort darf maximal 100 Zeichen lang sein")]
            string Place,

            [StringLength(200, ErrorMessage = "Adresse darf maximal 200 Zeichen lang sein")]
            string Address,

            [OptionalPhoneAttribute]
            string PhoneNumber
        );



        public record UserPutDto(
            int Id,
            byte[]? RowVersion,

            [Required(AllowEmptyStrings = false, ErrorMessage = "Benutzername muss eingegeben werden")]
            [StringLength(50, MinimumLength = 2, ErrorMessage = "Benutzername muss zwischen 2 und 50 Zeichen lang sein")]
            string UserName,

            [Required(AllowEmptyStrings = false, ErrorMessage = "E-Mail muss eingegeben werden")]
            [StringLength(100, ErrorMessage = "E-Mail darf maximal 100 Zeichen lang sein")]
            [EmailAddress(ErrorMessage = "E-Mail ist nicht gültig")]
            string Email,

            [Required(AllowEmptyStrings = false, ErrorMessage = "Vorname muss eingegeben werden")]
            [StringLength(50, MinimumLength = 2, ErrorMessage = "Vorname muss zwischen 2 und 50 Zeichen lang sein")]
            string FirstName,

            [Required(AllowEmptyStrings = false, ErrorMessage = "Nachname muss eingegeben werden")]
            [StringLength(50, MinimumLength = 2, ErrorMessage = "Nachname muss zwischen 2 und 50 Zeichen lang sein")]
            string LastName,

            [DateNotMinValueAttribute(nameof(BirthDate))] // 1.
            [DateNotInFutureAttribute(nameof(BirthDate))] // 2.
            DateTime BirthDate,

            Roles Role,

            [StringLength(100, ErrorMessage = "Land darf maximal 100 Zeichen lang sein")]
            string Country,

            [StringLength(20, ErrorMessage = "Postleitzahl darf maximal 20 Zeichen lang sein")]
            string PostalCode,

            [StringLength(100, ErrorMessage = "Ort darf maximal 100 Zeichen lang sein")]
            string Place,

            [StringLength(200, ErrorMessage = "Adresse darf maximal 200 Zeichen lang sein")]
            string Address,

            [OptionalPhoneAttribute]
            string PhoneNumber
        );

        public record UserChangePasswordDto(
            int Id,
            byte[]? RowVersion,

            [Required(AllowEmptyStrings = false, ErrorMessage = "Passwort muss eingegeben werden")]
            string NewPassword
        );

        public record LoginRequest(
            [Required(AllowEmptyStrings = false, ErrorMessage = "Benutzername muss eingegeben werden")]
            [StringLength(50, MinimumLength = 2, ErrorMessage = "Benutzername muss zwischen 2 und 50 Zeichen lang sein")]
            string Username,

            [Required(AllowEmptyStrings = false, ErrorMessage = "Passwort muss eingegeben werden")]
            string Password
        );

        public record AuthResponse(string Token);
    }
}
