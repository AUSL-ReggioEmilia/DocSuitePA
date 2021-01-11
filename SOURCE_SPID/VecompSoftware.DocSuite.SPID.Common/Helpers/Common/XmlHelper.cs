using System.IO;
using System.Xml.Serialization;

namespace VecompSoftware.DocSuite.SPID.Common.Helpers.Common
{
    public class XmlHelper
    {
        #region [ Methods ]        
        public static T Deserialize<T>(string serialized)
        {
            using (TextReader sr = new StringReader(serialized))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(sr);
            }
        }
        #endregion
    }
}
