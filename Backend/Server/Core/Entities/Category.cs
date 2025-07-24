using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Category: EntityObject
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Kategoriename muss eingegeben werden")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Kategoriename muss zwischen 2 und 100 Zeichen lang sein")]
        public string Name { get; set; } = string.Empty;


        //Navigation Property
        public ICollection<SubCategory> SubCategories { get; set; } = [];
    }
}
