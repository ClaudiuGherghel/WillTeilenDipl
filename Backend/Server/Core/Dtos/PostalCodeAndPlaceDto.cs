using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class PostalCodeAndPlaceDto
    {

        public string PostalCode { get; set; } = string.Empty;

        public string Place { get; set; } = string.Empty;
    }
}
