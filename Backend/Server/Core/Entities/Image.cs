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

        [Required(AllowEmptyStrings = false, ErrorMessage = "Url muss eingegeben werden")]
        [StringLength(300, ErrorMessage = "Die Bild-URL darf maximal 300 Zeichen lang sein.")]
        [Url] //Fehlermeldung auch bei ""
        public string ImageUrl { get; set; } = string.Empty;  // Pfad oder URL des Bildes

        [StringLength(150, ErrorMessage = "Der Alternativtext darf maximal 150 Zeichen lang sein.")]
        public string AltText { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Der MIME-Typ darf maximal 100 Zeichen lang sein.")]
        public string MimeType { get; set; } = string.Empty; // z. B. "image/jpeg"


        //Image-Sortierung
        public int DisplayOrder { get; set; } = 0;
        public bool IsMainImage { get; set; } = false;

        // Foreign Key
        [ForeignKey(nameof(ItemId))]
        public int ItemId { get; set; }

        // Navigation Property
        public Item Item { get; set; } = null!;
    }
}

//Die Navigation-Property Item ist nullable, was bei ORM-Modellen üblich ist, weil das verknüpfte Objekt z. B. nicht immer mitgeladen wird.
