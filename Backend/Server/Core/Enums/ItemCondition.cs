using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums
{
    public enum ItemCondition
    {
        // <summary>
        // Standartwert
        // </summary>
        Unknown,

        // <summary>
        // (Wie neu) – kaum benutzt, in ausgezeichnetem Zustand
        // </summary>
        LikeNew,

        // <summary>
        // (Gut) – leichte Gebrauchsspuren, voll funktionsfähig
        // </summary>
        Good,

         //<summary>
         //(Gebraucht) – sichtbare Abnutzung, aber geeignet für den Einsatz
         //</summary>
        Used
    }
}
