using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.AVCP.Import
{
    public interface IAVCPDataReader
  {
    bool Fetch(out List<DocumentRow> rows, out List<Notification> err);
  }
}
