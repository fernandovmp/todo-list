using System;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using TodoList.WebApi.Configurations;

namespace TodoList.WebApi.Services
{

    public class PasswordHasherService : IPasswordHasherService
    {

        private const int SaltSize = 16;
        private const int KeySize = 32;
        private readonly HasherOptions _options;

        public PasswordHasherService(IOptions<HasherOptions> options)
        {
            _options = options.Value;
        }

        public string Hash(string password)
        {
            using var algorithm = new Rfc2898DeriveBytes(
                password, SaltSize, _options.Iterations, HashAlgorithmName.SHA256);

            string key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
            string salt = Convert.ToBase64String(algorithm.Salt);

            return $"{_options.Iterations}.{salt}.{key}";
        }

        public bool VerifyPassword(string hash, string password)
        {
            string[] hashParts = hash.Split('.');

            int iterations = int.Parse(hashParts[0]);
            byte[] salt = Convert.FromBase64String(hashParts[1]);
            byte[] key = Convert.FromBase64String(hashParts[2]);

            using var algorithm = new Rfc2898DeriveBytes(
                password, salt, iterations, HashAlgorithmName.SHA256);

            byte[] keyToCheck = algorithm.GetBytes(KeySize);
            return keyToCheck.SequenceEqual(key);
        }
    }
}
