using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace VecompSoftware.DocSuite.Public.Helpers.Json
{
    public class DocSuiteContractResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty prop = base.CreateProperty(member, memberSerialization);

            if (!prop.Writable)
            {
                PropertyInfo property = member as PropertyInfo;
                if (property != null)
                {
                    bool hasPrivateSetter = property.GetSetMethod(true) != null;
                    prop.Writable = hasPrivateSetter;
                }
            }

            return prop;
        }
    }
}