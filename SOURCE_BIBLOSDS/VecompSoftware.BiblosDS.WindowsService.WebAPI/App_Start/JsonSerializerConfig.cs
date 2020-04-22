using Newtonsoft.Json;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace VecompSoftware.BiblosDS.WindowsService.WebAPI
{
    public class JsonSerializerConfig
    {
        internal static JsonSerializerSettings SerializerSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Include,
            TypeNameHandling = TypeNameHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.All
        };

        public static void RegisterConfigure(HttpConfiguration config)
        {
            //Configure Web API
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter()
            {
                SerializerSettings = SerializerSettings
            });
        }
    }
}
