using Core.Entities;
using Core.Enums;
using Core.Validations.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class ItemForSearchQueryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public decimal Deposit { get; set; }

        public RentalType RentalType { get; set; } = RentalType.Unknown; // Required funktioniert nur mit null, deshalb Custom Validator

        public ItemCondition ItemCondition { get; set; } = ItemCondition.Unknown;

        // Navigationen
        public GeoPostal GeoPostal { get; set; } = null!;

        // Main Image (kann null sein)
        public ImageForSearchQueryDto? MainImage { get; set; }
    }
}
