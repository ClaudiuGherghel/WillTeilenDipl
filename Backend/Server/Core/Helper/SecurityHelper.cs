using Core.Entities;
using System.Security.Cryptography;
using System.Text;

namespace Core.Helper
{
    public static class SecurityHelper
    {
        private const int SaltSize = 16; // 128 bit
        private const int KeySize = 32;  // 256 bit
        private const int Iterations = 100_000;

        public static string HashPasswordSimple(string password)
        {
            return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
        }

        public static string HashPassword(string password)
        {
            // Salt erzeugen
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            // Hash mit Salt berechnen
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(KeySize);

            // Salt + Hash kombinieren und als Base64 zurückgeben
            byte[] hashBytes = new byte[SaltSize + KeySize];
            Buffer.BlockCopy(salt, 0, hashBytes, 0, SaltSize);
            Buffer.BlockCopy(hash, 0, hashBytes, SaltSize, KeySize);

            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            // Salt extrahieren
            byte[] salt = new byte[SaltSize];
            Buffer.BlockCopy(hashBytes, 0, salt, 0, SaltSize);

            // Hash des eingegebenen Passworts mit dem gespeicherten Salt erzeugen
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(KeySize);

            // Vergleich mit gespeichertem Hash
            for (int i = 0; i < KeySize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                    return false;
            }
            return true;
        }


        /*
            1. Salt verhindert Rainbow-Table-Angriffe
            HashPasswordSimple (SHA256) erzeugt für gleiche Passwörter denselben Hash → sehr anfällig!
            Mit Salt wird jedem Passwort ein zufälliger Wert vorangestellt → jeder Hash ist einzigartig, selbst bei gleichem Passwort.
             Beispiel:
            "Passwort123" → zwei User → zwei verschiedene Hashes dank Salt!

            2. PBKDF2 macht Brute-Force langsam
            SHA256 ist sehr schnell → gut für Datenverarbeitung, schlecht für Passwortschutz.
            PBKDF2 mit z. B. 100.000 Iterationen ist absichtlich langsam → bremst Angreifer massiv bei Brute-Force-Versuchen.

            3. Standardkonform und bewährt
            PBKDF2 ist ein etablierter Standard (RFC 8018), in vielen Frameworks und Bibliotheken weltweit genutzt (z. B. ASP.NET Identity).
            Von Sicherheitsforschern geprüft und empfohlen.
        */


    }
}
