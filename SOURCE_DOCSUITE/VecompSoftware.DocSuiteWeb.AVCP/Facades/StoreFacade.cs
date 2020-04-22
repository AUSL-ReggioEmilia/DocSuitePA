using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.AVCP.Import
{
    public class StoreFacade
    {
        public static string GetDocumentKeyXml(string documentKey)
        {
            using (System.IO.StreamReader sr = new System.IO.StreamReader(@"torinoTest.xml"))
            {
                return sr.ReadToEnd();
            }
        }

        public static Dictionary<string, string> ResolveDocumentKey(string[] cigs)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var cig in cigs)
            {
                dict.Add(cig, "1/2/3");
            }
            return dict;
        }

        public static void SaveXml(int anno, string codiceServizio, int numerof, string xml)
        {
            Console.WriteLine(xml);
        }
    }
}
