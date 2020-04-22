using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.DocSuiteWeb.Services.WSDocm.Dto;
using VecompSoftware.Helpers;
using VecompSoftware.Helpers.ExtensionMethods;

namespace VecompSoftware.DocSuiteWeb.Services.WSDocm
{
    public static class WsDocmUtil
    {
        public static IDictionary<string, string> GetDictionaryFromXml(string path, string keyProperty,
            string valueProperty)
        {
            var xdoc = XDocument.Load(path);
            Dictionary<string, string> dictionary = xdoc.Descendants("property").ToDictionary(d => (string)d.Attribute(keyProperty), d => (string)d.Attribute(valueProperty));

            return dictionary;
        }

        public static List<Pratica> GetDataFromFinder(IList<Filtro> filters, IEnumerable<string> metadata, int maxResults)
        {
            var results = new List<Pratica>();
            IList<DocumentHeader> documents = FacadeFactory.Instance.DocumentFacade.GetDataForWs(filters.ToDictionary(f => f.Chiave, f => f.Valore), maxResults);

            var mappingProperties = GetDictionaryFromXml(HttpContext.Current.Server.MapPath("Mapping/Pratica.xml"), "name", "field");
            foreach (var document in documents)
            {
                var pratica = new Pratica();
                foreach (var entry in mappingProperties)
                {
                    var propInfo = document.GetType().GetProperty(entry.Value);
                    if(propInfo != null && (metadata.Any(x => x.Eq(entry.Key)) || metadata.Contains("ALL")))
                        ReflectionHelper.SetProperty(pratica, entry.Key, propInfo.GetValue(document, null));
                }
                results.Add(pratica);
            }
            return results;
        }
    }
}