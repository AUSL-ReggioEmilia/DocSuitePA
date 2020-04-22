using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Preservation.IpdaDoc
{
  public class IpdaUtil
  {
    public static string GetCloseFile(string preservationPath)
    {
      return GetFile(preservationPath, "chiusura*.txt");
    }

    public static string GetIpdaXmlFile(string preservationPath)
    {
      return GetFile(preservationPath, "ipda*.xml");
    }

    public static string GetIpdaTsdFile(string preservationPath)
    {
      return GetFile(preservationPath, "ipda*.tsd");
    }

    public static string Close2IpdaFilename(string closeFile)
    {
      string res = closeFile.Replace("CHIUSURA", "IPDA");
      return Path.Combine(Path.GetDirectoryName(closeFile), Path.GetFileNameWithoutExtension(res) + ".xml");
    }
        
    private static string GetFile(string path, string searchFile)
    {
      if (String.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
        return "";

      string[] files = Directory.GetFiles(path, searchFile);
      if (files.Count() != 1)
        return "";

      return files[0];
    }

  }
}
