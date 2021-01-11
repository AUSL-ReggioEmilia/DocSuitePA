using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VecompSoftware.JeepService.DocSeriesExporter.CsvHelper
{
    public class ExporterHelper
    {
        public const string EnumTipoProcedimento = "ENUM_TipoProcedimento";

        private static readonly Hashtable HashTipoProcedimento = new Hashtable
        {
            { "0", "Attivati d'ufficio dall'azienda" },
            { "1", "Attivati ad istanza di parte" }
        };

        public static string GetTipoProcedimento(string value)
        {
            if(HashTipoProcedimento.ContainsKey(value))
            {
                return HashTipoProcedimento[value].ToString();
            }
            return value;
        }
    }
}
