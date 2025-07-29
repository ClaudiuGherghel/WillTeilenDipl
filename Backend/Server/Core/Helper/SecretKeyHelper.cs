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

        public static byte[]? GenerateSecretKeyGenerator()
        {
            var key = new byte[32]; // 256 Bit
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(key);
            string base64Key = Convert.ToBase64String(key);
            Console.WriteLine(base64Key);
            return key;
        }
    }
}
