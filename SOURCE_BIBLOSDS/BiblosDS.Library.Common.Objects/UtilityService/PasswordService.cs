using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Principal;
using System.Security.Permissions;
using System.Reflection;
using System.Security.Cryptography;
using System.IO;

namespace BiblosDS.Library.Common.Objects.UtilityService
{
    public class PasswordService
    {
        public static string GenerateHash(string value)
        {
            try
            {
                SHA256CryptoServiceProvider criptographyPassword = new SHA256CryptoServiceProvider();
                byte[] hashValue = criptographyPassword.ComputeHash(Encoding.UTF8.GetBytes(value));
                StringBuilder hashCode = new StringBuilder();
                for (int i = 0; i < hashValue.Length; i++)
                {
                    hashCode.Append(hashValue[i].ToString("X2"));
                }
                return hashCode.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Errori nella generazione dell'hash", ex);
            }
        }
    }
}
