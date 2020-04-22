using System;

namespace VecompSoftware.DocSuiteWeb.AVCP.Import
{
    public class ImportRowPagamento
      {
        public string CIG { get; set; }
        public decimal ImportoLiquidato { get; set; }

        //chiave di aggregazione
        public string DocumentKey { get; set; }
        public DateTime DataAggiornamento { get; set; }
      }

}
