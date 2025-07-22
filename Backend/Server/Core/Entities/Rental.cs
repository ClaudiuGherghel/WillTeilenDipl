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
    public class Rental: EntityObject
    {
        [Required]
        //[ReservationDateValidation]
        public DateTime StartTime { get; set; }

        // Ende der Reservierung
        [Required]
        public DateTime EndTime { get; set; }

        // Optionale Notiz zur Reservierung
        public string Note { get; set; } = string.Empty;

        public RentalStatus Status { get; set; } = RentalStatus.Active;


        //Foreign Keys
        [ForeignKey(nameof(RenterId))]
        public int RenterId { get; set; }

        [ForeignKey(nameof(ItemId))]
        public int ItemId { get; set; }

        // Navigation Properties
        public User? Renter { get; set; }
        public Item? Item { get; set; }
    }
}

