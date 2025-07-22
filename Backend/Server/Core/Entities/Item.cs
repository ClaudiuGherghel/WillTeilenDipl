using Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Item: EntityObject
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = false;
        [Required]
        public string Country { get; set; } = string.Empty;
        [Required]
        public string State { get; set; } = string.Empty;
        [Required]
        public string PostalCode { get; set; } = string.Empty;
        [Required]
        public string Place { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        [Column(TypeName = "decimal(6,2)"), Range(0, double.MaxValue, ErrorMessage = "Miete muss 0 oder höher sein")]
        [Required]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock muss 0 oder höher sein")]
        public int Stock { get; set; } = 0;

        [Column(TypeName = "decimal(6,2)"), Range(0, double.MaxValue, ErrorMessage = "Deposit muss 0 oder höher sein")]
        public decimal Deposit { get; set; }

        [Required]
        public RentalType RentalType { get; set; }

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
