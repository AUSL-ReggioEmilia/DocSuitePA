using System;
using Newtonsoft.Json;

namespace VecompSoftware.DocSuiteWeb.API
{
    internal class APIArgumentConverter<T, I> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType is I);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType.IsArray)
                return serializer.Deserialize<T[]>(reader);
            return serializer.Deserialize<T>(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}