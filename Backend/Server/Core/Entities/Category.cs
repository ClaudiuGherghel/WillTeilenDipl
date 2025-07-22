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
        [Required(ErrorMessage = "Kategoriename muss eingegeben werden")]
        public string Name { get; set; } = string.Empty;

        //Navigation Property
        public ICollection<SubCategory> SubCategories { get; set; } = [];
    }
}
