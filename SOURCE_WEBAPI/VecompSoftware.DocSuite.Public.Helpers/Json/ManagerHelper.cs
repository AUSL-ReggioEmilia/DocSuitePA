using Newtonsoft.Json;

namespace VecompSoftware.DocSuite.Public.Helpers.Json
{
    public static class ManagerHelper
    {
        #region [ Fields ]

        private static readonly JsonSerializerSettings _defaultSerializerSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new DocSuiteContractResolver(),
            MaxDepth = 10,
        };

        #endregion

        #region [ Properties ]
        /// <summary>
        /// Ritorna la classe coi parametri di default che la DocSuite usa per serializzare e deserializzare correttamente i modelli.
        /// L'uso 
        /// </summary>
        public static JsonSerializerSettings DocSuiteJsonSerializerSettings => _defaultSerializerSettings;

        #endregion

        /// <summary>
        /// Metodo che permette di serializzare correttamente i modelli della DocSuite
        /// Incapsula correttamente nella stringa risultante, le varie sezioni $type. 
        /// Usando questo motodo standard il fornitore non necessita di ulteriore documentazione
        /// su come determinare i parametri di default da usare durante la serializzazione Json.
        /// 
        /// Eventualmente se si intende procedere manualmente, usare <paramref name="DocSuiteJsonSerializerSettings"/>
        /// </summary>
        /// <typeparam name="TModel">Modello in ingresso, Consultare la libreria <seealso cref="VecompSoftware.DocSuite.Public.Core.Models"/></typeparam>
        /// <param name="model">Modello da serializzare</param>
        /// <returns>json del modello serializzato</returns>
        public static string SerializeModel<TModel>(TModel model)
            where TModel : class
        {
            return JsonConvert.SerializeObject(model, DocSuiteJsonSerializerSettings);
        }

        /// <summary>
        /// Metodo che permette di deserializzare i modelli della DocSuite
        /// Decodifica correttamente il Json passato come parametro, instanziando correttamente 
        /// le classi dalle interfacce generiche. 
        /// Usando questo motodo standard il fornitore non necessita di ulteriore documentazione
        /// su come determinare i parametri di default da usare durante la deserializzazione Json.
        /// 
        /// Eventualmente se si intende procedere manualmente, usare <paramref name="DocSuiteJsonSerializerSettings"/>
        /// </summary>
        /// <typeparam name="TModel">Modello in ingresso, Consultare la libreria <seealso cref="VecompSoftware.DocSuite.Public.Core.Models"/></typeparam>
        /// <param name="modelSerialized">stringa contenente il Json da deserializzare</param>
        /// <returns>Modello deserializzato</returns>
        public static TModel DeserializeModel<TModel>(string modelSerialized)
            where TModel : class
        {
            return JsonConvert.DeserializeObject<TModel>(modelSerialized, DocSuiteJsonSerializerSettings);
        }
    }
}
