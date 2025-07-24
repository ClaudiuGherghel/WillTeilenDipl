using Core.Enums;
using Core.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Core.Entities
{
    [DateFromBeforeTo(nameof(From), nameof(To))] // 5.
    public class Rental: EntityObject
    {

        [DataType(DataType.Date)] // macht keine Validierung, für API kein nutzen
        [DateNotMinValue(nameof(From))] // 1.
        [DateNotInFuture(nameof(From))] // 2.
        public DateTime From { get; set; }

        [DataType(DataType.Date)] 
        [DateNotMinValue(nameof(To))] // 3.
        [DateNotInFuture(nameof(To))] // 4.
        public DateTime To { get; set; }

        [StringLength(1000, ErrorMessage = "Notiz darf maximal 1000 Zeichen lang sein")]
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


/* Reihenfolge der Validierungen
 * 1. Property-Level-Validierung
 * 2. Class-Level-Validierung
 */