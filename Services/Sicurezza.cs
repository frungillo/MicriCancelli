using System;
using System.Security.Cryptography;
using System.Text;

namespace MicriCancelli.Services
{
    public static class Sicurezza
    {
        public static string Sha256Hex(string testo)
        {
            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(testo ?? ""));
                var sb = new StringBuilder(hash.Length * 2);
                foreach (var b in hash) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        public static bool VerificaPassword(string tentativo, string hashAtteso) =>
            !string.IsNullOrEmpty(hashAtteso) &&
            string.Equals(Sha256Hex(tentativo), hashAtteso.Trim(), StringComparison.OrdinalIgnoreCase);

        /// <summary>Numero casuale crittografico di <paramref name="cifre"/> cifre, prima cifra diversa da zero.</summary>
        public static int NumeroCasuale(int cifre)
        {
            if (cifre < 1 || cifre > 9) throw new ArgumentOutOfRangeException(nameof(cifre));
            int min = (int)Math.Pow(10, cifre - 1);
            int max = (int)Math.Pow(10, cifre) - 1;
            using (var rng = RandomNumberGenerator.Create())
            {
                var buffer = new byte[4];
                rng.GetBytes(buffer);
                uint v = BitConverter.ToUInt32(buffer, 0);
                return min + (int)(v % (uint)(max - min + 1));
            }
        }
    }
}
