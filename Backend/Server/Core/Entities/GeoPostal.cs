using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class GeoPostal: EntityObject
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Land muss eingegeben werden")]
        [StringLength(50, ErrorMessage = "Land darf maximal 50 Zeichen lang sein")]
        public string Country { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Bundesland muss eingegeben werden")]
        [StringLength(50, ErrorMessage = "Bundesland darf maximal 50 Zeichen lang sein")]
        public string State { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Postleitzahl muss eingegeben werden")]
        [StringLength(10, ErrorMessage = "Postleitzahl darf maximal 10 Zeichen lang sein")]
        public string PostalCode { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Ort muss eingegeben werden")]
        [StringLength(100, ErrorMessage = "Ort darf maximal 100 Zeichen lang sein")]
        public string Place { get; set; } = string.Empty;
    }
}
