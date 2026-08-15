using System.Security.Cryptography;
using System.Text;

namespace FastKart.Helpers
{

    public static class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100000;

        private static readonly Random random = new();
        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;

        public static string CreatePasswordHash(string password)
        {
            if (password == null)
                throw new ArgumentNullException("password");

            if (password == string.Empty)
                throw new ArgumentException("password is empty");

            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

            return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
        }

        public static bool VerifyPassword(string password, string PasswordHash)
        {
            if (password == null)
                throw new ArgumentNullException("password");

            if (password == string.Empty)
                throw new ArgumentException("password is empty");

            if (PasswordHash == null)
                throw new ArgumentNullException("PasswordHash");

            if (PasswordHash == string.Empty)
                throw new ArgumentException("PasswordHash is empty");

            string[] parts = PasswordHash.Split('-');
            if (parts.Length != 2)
            {
                return false;
            }

            byte[] hash = Convert.FromHexString(parts[0]);
            byte[] salt = Convert.FromHexString(parts[1]);

            byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

            return CryptographicOperations.FixedTimeEquals(hash, inputHash);
        }

        public static string GeneratePassword(int length = 12)
        {
            if (length < 8)
                throw new ArgumentException("Password length must be at least 8 characters.");

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Production")
                return "Sadeem1234@";

            const string letters = "abcdefghijklmnopqrstuvwxyz";
            const string capitalLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string allChars = letters + capitalLetters + digits;

            StringBuilder password = new();

            // Ensure at least one letter
            password.Append(letters[random.Next(letters.Length)]);

            // Ensure at least one capital letter
            password.Append(capitalLetters[random.Next(capitalLetters.Length)]);

            // Ensure at least one digit
            password.Append(digits[random.Next(digits.Length)]);

            // Fill the rest with random characters, excluding whitespace
            while (password.Length < length)
            {
                char nextChar = allChars[random.Next(allChars.Length)];
                password.Append(nextChar);
            }

            // Shuffle the password to avoid predictable patterns
            return ShuffleString(password.ToString());
        }

        private static string ShuffleString(string input)
        {
            char[] array = input.ToCharArray();
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }
            return new string(array);
        }
    }
}