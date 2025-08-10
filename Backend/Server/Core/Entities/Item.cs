using Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Validations;
using Core.Validations.Annotations;

namespace Core.Entities
{
    public class Item: EntityObject
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Gegenstandsname muss eingegeben werden")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Gegenstandsname muss zwischen 2 und 100 Zeichen lang sein")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Beschreibung darf maximal 1000 Zeichen lang sein")]
        public string Description { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = false;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Land muss eingegeben werden")]
        [StringLength(50, ErrorMessage = "Land darf maximal 50 Zeichen lang sein")]
        public string Country { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Bundesland muss eingegeben werden")]
        [StringLength(50, ErrorMessage = "Bundesland darf maximal 50 Zeichen lang sein")]
        public string State { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Postleitzahl muss eingegeben werden")]
        [StringLength(20, ErrorMessage = "Postleitzahl darf maximal 20 Zeichen lang sein")]
        public string PostalCode { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Ort muss eingegeben werden")]
        [StringLength(100, ErrorMessage = "Ort darf maximal 100 Zeichen lang sein")]
        public string Place { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Adresse darf maximal 200 Zeichen lang sein")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Miete muss angegeben werden")]
        [Column(TypeName = "decimal(6,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Miete muss 0 oder höher sein")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock muss 0 oder höher sein")]
        public int Stock { get; set; } = 0;

        [Required(ErrorMessage = "Kaution muss angegeben werden")]
        [Column(TypeName = "decimal(6,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Kaution muss 0 oder höher sein")]
        public decimal Deposit { get; set; }

        [NotUnknownEnumAttribute(RentalType.Unknown, ErrorMessage = "Miettyp darf nicht Unknown sein.")]
        public RentalType RentalType { get; set; } = RentalType.Unknown; // Required funktioniert nur mit null, deshalb Custom Validator

        public ItemCondition ItemCondition { get; set; } = ItemCondition.Unknown;


        // Foreign Keys
        [ForeignKey(nameof(SubCategoryId))]
        public int SubCategoryId { get; set; }

        [ForeignKey(nameof(OwnerId))]
        public int OwnerId { get; set; }


        // Navigation Properties
        public SubCategory? SubCategory { get; set; }
        public User? Owner { get; set; }
        public ICollection<Rental> Rentals { get; set; } = [];
        public ICollection<Image> Images { get; set; } = [];
    }
}
