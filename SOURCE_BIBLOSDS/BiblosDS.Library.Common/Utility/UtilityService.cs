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
using System.Globalization;

namespace BiblosDS.Library.Common.Utility
{
    public class UtilityService
    {
        public static string GetStringFromBob(byte[] Blob)
        {
            return System.Convert.ToBase64String(Blob);
        }

        public static byte[] GetBlobFromString(string Base64Blob)
        {
            return System.Convert.FromBase64String(Base64Blob);
        }     

        public static string Format(string Value, string Validazione, string Formato, Type AttributeType, out object AttributeTypeValue)
        {
            AttributeTypeValue = null;
            try
            {
                Regex r = new Regex(Validazione);
                if (Formato.Length == 0)
                    return Value;

                if (Formato.IndexOf("|") < 0)
                {
                    string sHlp = r.Replace(Value, Formato);
                    return sHlp;
                }
                string[] arg = Formato.Split('|');
                object[] argo = new object[arg.Length];
                int[] yyyyMMdd = new int[3];
                for (int i = 1; i < arg.Length; i++)
                {
                    string Valore = r.Replace(Value, arg[i].Substring(1));
                    switch (arg[i].Substring(0, 1))
                    {
                        case "d":
                            argo[i] = int.Parse(Valore);
                            break;
                        case "n":
                            argo[i] = decimal.Parse(Valore);
                            break;
                        default:
                        case "s":
                            argo[i] = Valore;
                            break;
                    }                    
                    if (AttributeType.Equals(typeof(DateTime)))
                    {
                        switch (arg[i].Substring(3, arg[i].Length-4))
                        {
                            case "day":
                                yyyyMMdd[2] = int.Parse(Valore);
                                break;
                            case "month":
                                yyyyMMdd[1] = int.Parse(Valore);
                                break;
                            case "year":
                                yyyyMMdd[0] = int.Parse(Valore);
                                break;
                            default:
                                throw new Exception("Date not in correct Formatting");
                        }
                    }
                }
                if (AttributeType.Equals(typeof(DateTime)))
                    AttributeTypeValue = new DateTime(yyyyMMdd[0], yyyyMMdd[1], yyyyMMdd[2]);
                else
                {
                    if (arg.Length <= 2)
                        AttributeTypeValue = argo[0];
                    else
                        throw new Exception("Attribute Type "+AttributeType+" not implemented.");
                }
                return string.Format(new System.Globalization.CultureInfo("it-IT"), arg[0], argo);
            }
            catch (Exception ex)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                    "Format",
                    ex.ToString(),
                    BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                    BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                return "";
            }

            // sintassi  {1,00}/{2,00}/{3}|n${day}|n${month}|n${year}
            // La prima parte è la sintassi di Formattazione standard di .Net seguita dai parametri in sequenza.
            // I parametri iniziano con il tipo in cui vengono convertiti (n=number, d=decimal... secondo lo standard .Net), in un secondo tempo si potrebbe specificare tra [] la nazionalità della decodifica, es [en] per l'Inghlterra da passare al parser. Attualmente opera come formato italiano->italiano
        }

        public static string[] Roles(WindowsIdentity identity)
        {
            // Parameters check
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }
            if (identity.Name.Length < 1)
            {
                return new string[0];
            }

            // Get roles
            string[] roles = (string[])CallPrivateMethod(identity, "GetRoles");
            return roles;
        }

        /// <summary>
        /// 
        /// </summary>
        private static readonly char[] hexDigits = {
                                                       '0', '1', '2', '3', '4', '5', '6', '7',
                                                       '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'
                                                   };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileContent"></param>
        /// <returns></returns>
        public static string GetHash(byte[] fileContent, bool sha256mark = false)
        {
            string ret = string.Empty;

            if (fileContent != null)
            {
                if (sha256mark)
                    ret = ToHexString(new SHA256CryptoServiceProvider().ComputeHash(fileContent), sha256mark);
                else
                    ret = ToHexString(new SHA1CryptoServiceProvider().ComputeHash(fileContent), sha256mark);
            }

            return ret;
        }

        /// <summary>
        /// Torna un HASH SHA1 o SHA256 a partire da un file.
        /// </summary>
        /// <param name="sFile"></param>
        /// <returns></returns>
        public static string GetHash(string sFile, bool sha256mark = false)
        {
            try
            {
              using (var sr = new StreamReader(sFile))
              {
                byte[] result;

                result = (sha256mark) ? new SHA256CryptoServiceProvider().ComputeHash(sr.BaseStream) : new SHA1CryptoServiceProvider().ComputeHash(sr.BaseStream);

                return ToHexString(result, sha256mark);
              }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToHexString(byte[] bytes, bool sha256mark = false)
        {
            var chars = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                int b = bytes[i];
                chars[i * 2] = hexDigits[b >> 4];
                chars[i * 2 + 1] = hexDigits[b & 0xF];
            }
            return new string(chars);
        }

        [ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
        private static object CallPrivateMethod(object o, string methodName)
        {
            Type t = o.GetType();
            MethodInfo mi = t.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (mi == null)
            {
                throw new System.Reflection.ReflectionTypeLoadException(null, null, String.Format("{0}.{1} method wasn't found. The runtimeimplementation may have changed!", t.FullName, methodName));
            }
            return mi.Invoke(o, null);
        }

        public static string SafeSQLName(string str)
        {
            str = str.Trim().ToLower();

            //separatori -> spazi
            str = Regex.Replace(str, @"[-._:]", " ");

            //conversione accentate
            str = Regex.Replace(str, @"[áàäâå]", "a");
            str = Regex.Replace(str, @"[éèëê]", "e");
            str = Regex.Replace(str, @"[íìïî]", "i");
            str = Regex.Replace(str, @"[óòöô]", "o");
            str = Regex.Replace(str, @"[úùüû]", "u");

            //solamente alfa-numerici
            str = Regex.Replace(str, @"[^a-zA-Z0-9 ]+", "");

            //parole in Title case
            TextInfo textInfo = new CultureInfo("it-IT", false).TextInfo;
            str = textInfo.ToTitleCase(str);

            //rimozione degli spazi
            str = str.Replace(" ", string.Empty);
            return str;
        }

        public static string GetSafeFileName(string name, char replace = '_')
        {
            char[] invalids = Path.GetInvalidFileNameChars();
            return new string(name.Select(c => invalids.Contains(c) ? replace : c).ToArray());
        }
    }
}
