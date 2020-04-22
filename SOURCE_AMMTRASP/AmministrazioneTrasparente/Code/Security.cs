using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel.Security;
using System.Text;
using System.Web;

namespace AmministrazioneTrasparente.Code
{
    public class Security
    {
        public static string Username
        {
            get { return HttpContext.Current.User.Identity.Name; }    
        }

        public static string GetMd5Hash(string source)
        {
            using (var md5Hash = MD5.Create())
            {
                var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(source));
                var sBuilder = new StringBuilder();

                foreach (var t in data)
                {
                    sBuilder.Append(t.ToString("x2"));
                }

                return sBuilder.ToString();
            }
        }
    }
}