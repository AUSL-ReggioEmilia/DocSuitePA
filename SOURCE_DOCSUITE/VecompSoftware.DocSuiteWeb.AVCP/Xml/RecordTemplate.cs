using System;
using System.Xml.Serialization;

namespace VecompSoftware.DocSuiteWeb.AVCP.Import
{
    [XmlRootAttribute(ElementName = "record", IsNullable = false)]
  public class RecordTemplate : XmlFile<RecordTemplate>
  {
    [XmlElementAttribute("field")]
    public RecordTemplateField[] fields;

    public static string ParseDefaultValue(RecordTemplateField field)
    {
      if (field.type.ToLower() == "date" && field.defaultValue.ToLower() == "today")
        return DateTime.Today.ToShortDateString();

      return field.defaultValue;
    }
  }

  
}
