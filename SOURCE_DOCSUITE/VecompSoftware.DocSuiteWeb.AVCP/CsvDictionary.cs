using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VecompSoftware.DocSuiteWeb.AVCP.Import
{
    public class CsvDictionary
  {
    /// <summary>
    ///Mantiene solo parole di almeno 3 caratteri eliminando tutti i simboli numeri spazi e caratteri speciali.
    ///Converte tutte le lettere accentate in caratteri normali
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string NormalizeString(string str)
    {
      str = Regex.Replace(str, @"\b\w{1,3}\b", "");
      str = Regex.Replace(str, @"[^\w]", "");
      str = Regex.Replace(str, @"[0-9]", "");

      return RemoveAccents(str).ToUpper();
    }


    private static string RemoveAccents(string input)
    {
      string normalized = input.Normalize(NormalizationForm.FormKD);
      Encoding removal = Encoding.GetEncoding(Encoding.ASCII.CodePage,
                                              new EncoderReplacementFallback(""),
                                              new DecoderReplacementFallback(""));
      byte[] bytes = removal.GetBytes(normalized);
      return Encoding.ASCII.GetString(bytes);
    }

    /// <summary>
    /// Esegue parsing di un file in formato CSV con righe in formato key;value
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static Dictionary<string, string> Get(string filename, bool normalize = true)
    {
      try
      {
        string[] lines = File.ReadAllLines(filename);
        var query = from line in lines
                    let data = line.Trim().Split(';')
                    where data.Length == 2
                    select new
                    {
                      Key = normalize == true ? NormalizeString(data[0]) : data[0].Trim().ToUpper(),
                      Value = data[1].Trim()
                    };

        return query.ToDictionary(p => p.Key, p => p.Value);
      }
      catch (Exception ex)
      {
        throw new ApplicationException(string.Format("Errore lettura dizionario '{0}'. {1}", filename, ex));
      }
    }
  }
}
