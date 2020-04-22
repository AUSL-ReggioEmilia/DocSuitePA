using System.Collections.Generic;
using System.Dynamic;

namespace VecompSoftware.DocSuiteWeb.AVCP.Import
{
    public class DocumentRow
  {
    public int RowIndex { get; set; }
    public dynamic Values { get; set; }

    private IDictionary<string, object> valuesDict
    {
      get { return (IDictionary<string, object>)this.Values; }
    }

    public DocumentRow()
    {
      this.RowIndex = 0;
      this.Values = new ExpandoObject();
    }

    public void SetValue(string fieldName, object value)
    {
      valuesDict[fieldName] = value;
    }
  }
}
