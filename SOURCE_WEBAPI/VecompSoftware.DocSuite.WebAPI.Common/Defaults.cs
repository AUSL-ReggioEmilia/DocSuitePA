using Newtonsoft.Json;
using VecompSoftware.DocSuite.WebAPI.Common.Json;

namespace VecompSoftware.DocSuite.WebAPI.Common
{
    public class Defaults
    {
        #region [ Fields ]
        private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new WebAPIContractResolver(),
            MaxDepth = 10,
        };

        #endregion

        #region [ Properties ]

        public static JsonSerializerSettings DefaultJsonSerializer => _serializerSettings;
        #endregion

        private Defaults()
        {

        }
    }
}
