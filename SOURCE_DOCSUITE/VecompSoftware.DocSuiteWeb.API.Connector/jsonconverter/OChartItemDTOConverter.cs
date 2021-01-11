using System;
using Newtonsoft.Json;

namespace VecompSoftware.DocSuiteWeb.API
{
    internal class OChartItemDTOConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(IOChartItemDTO));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType.IsArray)
                return serializer.Deserialize<OChartItemDTO[]>(reader);
            return serializer.Deserialize<OChartItemDTO>(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}