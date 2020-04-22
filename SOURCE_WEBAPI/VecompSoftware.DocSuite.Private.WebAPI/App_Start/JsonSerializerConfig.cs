using Newtonsoft.Json;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace VecompSoftware.DocSuite.Private.WebAPI
{
    public static class JsonSerializerConfig
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
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            //Configure Web API
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter()
            {
                SerializerSettings = SerializerSettings
            });

            //Configure SignalR serialization
            JsonSerializer jsonSerializer = JsonSerializer.Create(settings);
            Microsoft.AspNet.SignalR.GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => jsonSerializer);
        }
    }
}
