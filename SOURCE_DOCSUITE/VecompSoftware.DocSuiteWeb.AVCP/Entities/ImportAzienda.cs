
using System;

namespace VecompSoftware.DocSuiteWeb.AVCP.Import
{
    public class ImportAzienda
  {
    public string Codice { get; set; }
    public string CodiceEstero { get; set; }
    public string RagioneSociale { get; set; }

    public ImportAzienda()
    {
      this.Codice = String.Empty;
      this.CodiceEstero = String.Empty;
      this.RagioneSociale = String.Empty;
    }
  }

}
