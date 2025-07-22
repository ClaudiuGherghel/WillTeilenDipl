using Core.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class EntityObject : IEntityObject
    {
        [Key]
        public int Id { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        // Beim ersten Speichern wird eine RowVersion generiert.
        // Bei jedem erfolgreichen SaveChanges() wird RowVersion auf einen neuen Wert gesetzt.
        // Der Eintrag wird gelöscht, die RowVersion wird nicht mehr relevant.
    }
}
