using System;

namespace BiblosDS.Library.Common.Preservation.ObjectsXml
{
    public class XmlBasic
    {
        protected static string CreaNodoNullo(Type type)
        {
            return string.Format("<{0}></{0}>", type.Name);
        }
    }
}