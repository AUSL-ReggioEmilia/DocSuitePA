namespace VecompSoftware.DocSuiteWeb.AVCP.Import
{
    public class DocumentData
  {
    public int Anno { get; set; }
    public string CodiceServizio { get; set; }
    public int Numero { get; set; }
    public string[] CigList { get; set; }

    public string DocumentKey { get; set; }
    public AVCP.pubblicazione Pubblicazione { get; set; }
  }
}
