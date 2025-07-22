using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Image: EntityObject
    {
        [Required]
        public string ImageUrl { get; set; } = string.Empty;  // Pfad oder URL des Bildes
        public string Description { get; set; } = string.Empty;  // Optional: Eine Beschreibung des Bildes


        // Foreign Key
        [ForeignKey(nameof(ItemId))]
        public int ItemId { get; set; }

        // Navigation Property
        public Item? Item { get; set; }
    }
}
