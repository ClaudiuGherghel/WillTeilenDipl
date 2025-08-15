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

            [StringLength(200, ErrorMessage = "Adresse darf maximal 200 Zeichen lang sein")]
            string Address,

            [OptionalPhoneAttribute]
            string PhoneNumber,

            [Range(1, int.MaxValue, ErrorMessage = "GeoPostalId muss größer als 0 sein.")]
            int GeoPostaldId

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

            [StringLength(200, ErrorMessage = "Adresse darf maximal 200 Zeichen lang sein")]
            string Address,

            [OptionalPhoneAttribute]
            string PhoneNumber,
            
            [Range(1, int.MaxValue, ErrorMessage = "GeoPostalId muss größer als 0 sein.")]
            int GeoPostaldId
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
