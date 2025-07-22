using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Contracts
{
    public interface IEntityObject
    {
        int Id { get; set; }
        byte[]? RowVersion { get; set; }
    }
}
