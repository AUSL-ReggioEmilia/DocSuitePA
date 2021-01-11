using Newtonsoft.Json;

namespace VecompSoftware.DocSuiteWeb.API
{
    public static class IAPISerializableEx
    {
        public static string Serialize(this IAPISerializable source)
        {
            var json = JsonConvert.SerializeObject(source);
            var base64 = json.ToBase64();
            return base64;
        }

        public static T Deserialize<T>(this string source) where T : IAPISerializable
        {
            var json = source.FromBase64();
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
