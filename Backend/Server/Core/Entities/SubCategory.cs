using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class SubCategory: EntityObject
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "Subkategoriename muss eingegeben werden")]
        public string Name { get; set; } = string.Empty;


        //Foreign Key
        [ForeignKey(nameof(CategoryId))]
        public int CategoryId { get; set; }


        //Navigation Properties
        public Category Category { get; set; } = null!;
        public ICollection<Item> Items { get; set; } = [];
    }
}
