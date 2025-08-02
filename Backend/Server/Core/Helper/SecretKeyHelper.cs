using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helper
{
    public static class SecretKeyHelper
    {

        /*  PowerShell Secret Key
            $bytes = New-Object byte[] 32
            [System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
            [Convert]::ToBase64String($bytes)
        */

        /// <summary>
        /// Generiert einen zufälligen 256-Bit (32 Byte) Secret Key für JWT, gibt ihn als Base64-String aus und liefert ihn als Byte-Array zurück.
        /// </summary>
        public static byte[] GenerateSecretKey()
        {
            byte[] key = RandomNumberGenerator.GetBytes(32); // 256 Bit
            string base64Key = Convert.ToBase64String(key);
            Console.WriteLine("Generated Base64 Secret Key:\n" + base64Key);
            return key;
        }
    }
}
