using Newtonsoft.Json;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Parameters
{
    public abstract class ParameterJsonModelMapper<TModel> : BaseModelMapper<string, TModel>, IParameterJsonModelMapper<TModel>
        where TModel : class, new()
    {

        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public override TModel Map(string value, TModel result)
        {
            TModel deserialized = JsonConvert.DeserializeObject<TModel>(value, _jsonSerializerSettings);
            return deserialized;
        }

    }
}
