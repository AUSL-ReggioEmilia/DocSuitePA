using System;
using System.Security.Cryptography;
using System.Text;

namespace VecompSoftware.Helpers.Signer.Security
{
    public class HashGenerator
    {
        public static string GenerateHash(string password)
        {
            try
            {
                SHA256CryptoServiceProvider criptographyPassword = new SHA256CryptoServiceProvider();
                byte[] hashValue = criptographyPassword.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder hashCode = new StringBuilder();
                for (int i = 0; i < hashValue.Length; i++)
                {
                    hashCode.Append(hashValue[i].ToString("X2"));
                }
                return hashCode.ToString();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Errors in generating the hash code", ex);
            }

        }

    }
}
