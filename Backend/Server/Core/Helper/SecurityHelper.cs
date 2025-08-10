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

        /// <summary>
        /// Einfache SHA256-Hashfunktion ohne Salt (nicht empfohlen für echte Passwörter).
        /// </summary>
        public static string HashPasswordSimple(string password)
        {
            return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
        }

        /// <summary>
        /// Erzeugt einen sicheren salted PBKDF2-Hash für das Passwort.
        /// </summary>
        public static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(KeySize);

            byte[] hashBytes = new byte[SaltSize + KeySize];
            Buffer.BlockCopy(salt, 0, hashBytes, 0, SaltSize);
            Buffer.BlockCopy(hash, 0, hashBytes, SaltSize, KeySize);

            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Prüft, ob das eingegebene Passwort zum gespeicherten salted PBKDF2-Hash passt.
        /// </summary>
        public static bool VerifyPassword(string password, string storedHash)
        {
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            byte[] salt = new byte[SaltSize];
            Buffer.BlockCopy(hashBytes, 0, salt, 0, SaltSize);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(KeySize);

            for (int i = 0; i < KeySize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Erzeugt einen 256-Bit-Key für JWT HS256, als Base64-String gespeichert.
        /// </summary>
        public static string GenerateJwtSecretBase64()
        {
            byte[] keyBytes = RandomNumberGenerator.GetBytes(KeySize); // 32 Bytes = 256 bit
            return Convert.ToBase64String(keyBytes);
        }

        /// <summary>
        /// Erzeugt einen 256-Bit-Key für JWT HS256 als reiner UTF8-String (mindestens 32 Zeichen).
        /// </summary>
        public static string GenerateJwtSecretPlain()
        {
            // 32 zufällige Bytes → Base32-ähnlicher String
            var keyBytes = RandomNumberGenerator.GetBytes(KeySize);
            return Convert.ToHexString(keyBytes); // gibt 64 Zeichen (256-bit Hex)
        }
    }
}
