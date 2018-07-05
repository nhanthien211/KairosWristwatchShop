using System;
using System.Security.Cryptography;

namespace ProjectKairos.Utilities
{
    public class EncryptPasswordUtil
    {
        private const int Iteration = 1000;  //recommend iteration 
        private const int SaltByteSize = 24; //recommended size
        private const int HashByteSize = 24; //recommended size

        public static string EncryptPassword(string password, out string saltKey)
        {
            byte[] salt = new byte[SaltByteSize];

            RNGCryptoServiceProvider cryptoProvider = new RNGCryptoServiceProvider();
            cryptoProvider.GetBytes(salt);

            saltKey = Convert.ToBase64String(salt);

            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iteration);
            //this will create a derived key from password and salt

            return Convert.ToBase64String((pbkdf2.GetBytes(HashByteSize)));
        }

        public static string EncryptPassword(string password, string saltKey)
        {
            byte[] salt = Convert.FromBase64String(saltKey);

            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iteration);
            //this will create a derived key from password and salt

            return Convert.ToBase64String((pbkdf2.GetBytes(HashByteSize)));
        }


    }
}