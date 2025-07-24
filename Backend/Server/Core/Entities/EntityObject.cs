using Core.Contracts;
using Core.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    [DateFromBeforeTo(nameof(CreatedAt), nameof(UpdatedAt))] // 5.
    public class EntityObject : IEntityObject
    {
        [Key]
        public int Id { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }


        [DataType(DataType.Date)]
        [DateNotMinValue(nameof(CreatedAt))] // 1.
        [DateNotInFuture(nameof(CreatedAt))] // 2.
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [DateNotMinValue(nameof(UpdatedAt))] // 3.
        [DateNotInFuture(nameof(UpdatedAt))] // 4.
        public DateTime UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false; // Soft-Delete

    }
}

        // Beim ersten Speichern wird eine RowVersion generiert.
        // Bei jedem erfolgreichen SaveChanges() wird RowVersion auf einen neuen Wert gesetzt.
        // Der Eintrag wird gelöscht, die RowVersion wird nicht mehr relevant.