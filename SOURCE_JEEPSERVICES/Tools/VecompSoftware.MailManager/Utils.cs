using System;
using System.Globalization;
using System.IO;

namespace VecompSoftware.MailManager
{
    public static class Utils
    {
        public static int PrefixLen = 6;

        public static void GetDropFilenames(string dropFolder, out string outpathEml, out string outpathXml)
        {
            while (true)
            {
                var tempName = GetTempName();
                outpathEml = Path.Combine(dropFolder, String.Format("{0}_{1}", tempName, "mail.eml"));
                outpathXml = Path.Combine(dropFolder, String.Format("{0}_{1}", tempName, "info.xml"));
                if (!File.Exists(outpathEml) && !File.Exists(outpathXml))
                    break;
            }
        }


        public static string SafeFileName(string filename, int maxLength = 0)
        {
            // Rimuovo i caratteri non validi
            Array.ForEach(Path.GetInvalidFileNameChars(),
                  c => filename = filename.Replace(c.ToString(CultureInfo.InvariantCulture), String.Empty));

            // Verifico la lunghezza
            if (maxLength > 0)
                filename = TrimFileName(filename, maxLength);

            return filename;
        }

        public static string TrimFileName(string filename, int allowedMaxLength)
        {
            var extension = Path.GetExtension(filename) ?? String.Empty;
            var maxLength = allowedMaxLength - extension.Length;

            //prende gli ultimi maxLenght caratteri del nome del file
            if (maxLength > 0 &&  Path.GetFileNameWithoutExtension(filename).Length > maxLength)
            {
                // Rimuovo l'estensione
                filename = Path.GetFileNameWithoutExtension(filename);
                // Prendo i primi maxLength caratteri
                filename = String.Concat(filename.Substring(0, maxLength - 1), extension);
            }

            return filename;
        }


        public static string CreateTempName(string folder, string filename, int biblosMaxLength)
        {
            filename = SafeFileName(filename);
            while (true)
            {
                var tempName = GetTempName();
                filename = TrimFileName(filename, biblosMaxLength - folder.Length - tempName.Length);

                var outpath = Path.Combine(folder, String.Format("{0}_{1}", tempName, filename));
                if (!File.Exists(outpath))
                    return outpath;
            }
        }


        public static string CreateTempFolder(string path)
        {
            string outpath;
            while (true)
            {

                outpath = Path.Combine(path, GetTempName());
                if (!Directory.Exists(outpath))
                {
                    Directory.CreateDirectory(outpath);
                    break;
                }
            }
            return outpath;
        }

        public static string GetTempName()
        {
            var tmp = Path.GetRandomFileName().Replace(".", String.Empty).ToUpper();

            if (tmp.Length > PrefixLen)
                tmp = tmp.Substring(0, PrefixLen);

            if (tmp.Length < PrefixLen)
                tmp = tmp.PadLeft(PrefixLen, '0');

            return tmp;
        }


        public static void CopyDirectory(string source, string destination)
        {
            if (!destination.EndsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture)))
            {
                destination += Path.DirectorySeparatorChar;
            }

            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            var entries = Directory.GetFileSystemEntries(source);

            foreach (var item in entries)
            {
                if (Directory.Exists(item))
                {
                    CopyDirectory(item, destination + Path.GetFileName(item));
                }
                else
                {
                    File.Copy(item, destination + Path.GetFileName(item), true);
                }
            }
        }

        public static string FullStacktrace(Exception ex)
        {
            var currentException = ex;
            var fullStracktrace = String.Empty;
            while (currentException != null)
            {
                fullStracktrace += String.Format("{1}:{0}{0}{2}{0}{0}",Environment.NewLine, currentException.Message, currentException.StackTrace);
                currentException = currentException.InnerException;
            }
            return fullStracktrace;
        }
    }
}
