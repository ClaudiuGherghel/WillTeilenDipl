using Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Validations;

namespace Core.Entities
{
    public class User: EntityObject
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "Benutzername muss eingegeben werden")]
        public string Username { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Passwort muss eingegeben werden")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "E-Mail muss eingegeben werden")]
        [EmailAddress(ErrorMessage = "E-Mail ist nicht gültig")]
        public string Email { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Vorname muss eingegeben werden")]
        public string FirstName { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Nachname muss eingegeben werden")]
        public string LastName { get; set; } = string.Empty;

        [DataType(DataType.Date)] // macht keine Validierung, für API kein nutzen
        [DateNotMinValue(nameof(BirthDate))] // 1.
        [DateNotInFuture(nameof(BirthDate))] // 2.
        public DateTime BirthDate { get; set; }

        public Roles Role { get; set; } = Roles.User; // Standardrolle
        public string Country { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Place { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        [Phone(ErrorMessage = "Telefonnummer ist nicht gültig")]
        public string PhoneNumber { get; set; } = string.Empty;


        // Navigation Properties
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

Leere Zeichenfolge, wenn kein [Required] gesetzt ist: → gilt als gültig, da [Phone] nicht prüft, ob etwas überhaupt eingegeben wurde.
*/