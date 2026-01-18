using Saistones.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Saistones.Infrastructure.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int KeySize = 32;
        private const int Iterations = 100_000;

        public void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using var hmac = new HMACSHA256();
            salt = hmac.Key;

            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256
            );

            hash = pbkdf2.GetBytes(KeySize);
        }

        public bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            storedSalt,
            Iterations,
            HashAlgorithmName.SHA256
        );

            var computedHash = pbkdf2.GetBytes(KeySize);

            return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
        }
    }
}
