using Core.Enums;
using Core.Validations.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Core.Entities
{
    public class User: EntityObject
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "Benutzername muss eingegeben werden")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Benutzername muss zwischen 2 und 50 Zeichen lang sein")]
        public string UserName { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Passwort muss eingegeben werden")]
        public string PasswordHash { get; set; } = string.Empty;
        public Roles Role { get; set; } = Roles.User; // Standardrolle

        [Required(AllowEmptyStrings = false, ErrorMessage = "E-Mail muss eingegeben werden")]
        [StringLength(100, ErrorMessage = "E-Mail darf maximal 100 Zeichen lang sein")]
        [EmailAddress(ErrorMessage = "E-Mail ist nicht gültig")]
        public string Email { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Vorname muss eingegeben werden")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Vorname muss zwischen 2 und 50 Zeichen lang sein")]
        public string FirstName { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Nachname muss eingegeben werden")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Nachname muss zwischen 2 und 50 Zeichen lang sein")]
        public string LastName { get; set; } = string.Empty;

        [DataType(DataType.Date)] // macht keine Validierung, für API kein nutzen
        [DateNotMinValueAttribute(nameof(BirthDate))] // 1.
        [DateNotInFutureAttribute(nameof(BirthDate))] // 2.
        public DateTime BirthDate { get; set; }

        [StringLength(200, ErrorMessage = "Adresse darf maximal 200 Zeichen lang sein")]
        public string Address { get; set; } = string.Empty;

        // [Phone] Fehlermeldung bei leerer Eingabe aber nicht bei null
        [OptionalPhoneAttribute]
        public string PhoneNumber { get; set; } = string.Empty;


        //Foreign Key

        [ForeignKey(nameof(GeoPostalId))]
        public int GeoPostalId { get; set; }

        // Navigation Properties
        public GeoPostal? GeoPostal { get; set; }
        public ICollection<Rental> Rentals { get; set; } = [];
        public ICollection<Item> OwnedItems { get; set; } = [];
    }
}

/*Obwohl DateTime ein Werttyp ist (nicht nullable), 
 * und somit nie null sein kann, 
 * wird beim Model-Binding aus JSON der Standardwert DateTime.MinValue (01.01.0001) gesetzt, 
 * wenn kein Datum übermittelt wurde.
 * 
 *  Empfehlung
Wenn du automatische Validierung willst:
➡️ DateTime? + [Required]

Wenn du lieber einen nicht-nullbaren Typ behalten willst:
➡️ DateTime + manuelle Prüfung auf .MinValue (if..)
 * */


/* Als gültig zählt:
Nur Ziffern: "1234567890"
Mit Leerzeichen, Bindestrichen, Klammern oder Plus:
"(0123) 456-7890"
"+49 160 1234567"
"0049-89-123456"

Als ungültig zählt z. B.:
Buchstaben enthalten: "abc123456" ❌
Sonderzeichen wie @, !, #, etc.: "123@456" ❌
Unvollständig wie nur "+" oder "()": ❌
*/