using System.IO;
using System.Xml.Serialization;

namespace VecompSoftware.DocSuiteWeb.Common.Helpers
{
    public static class XmlSerializerHelper
    {
        public static string SerializeString<TModel>(TModel model)
        {
            using (StringWriter stringWriter = new StringWriter())
            {
                XmlSerializer xm = new XmlSerializer(typeof(TModel));
                xm.Serialize(stringWriter, model);
                return stringWriter.GetStringBuilder().ToString();
            }
        }
    }
}
