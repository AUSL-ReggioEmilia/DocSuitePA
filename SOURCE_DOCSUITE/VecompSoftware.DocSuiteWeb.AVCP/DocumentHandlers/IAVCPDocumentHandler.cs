using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.AVCP.Import
{
    public delegate List<ImportRowGara> RowGaraConverter(List<DocumentRow> rows);

  public delegate string GetDocumentKeyXml(string documentKey);
  public delegate Dictionary<string,string> ResolveDocumentKey(string[] cigs);

  public interface IAVCPDocumentHandler
  {
    List<DocumentData> BuildDocuments(List<DocumentRow> rows, out List<Notification> errors);
    List<ImportRowPagamento> AggregatePagamenti(List<DocumentRow> rows);
    List<DocumentData> UpdateDocuments(List<DocumentRow> rows, ResolveDocumentKey resolveHandler, GetDocumentKeyXml getHandler, out List<Notification> errors);
  }

}
