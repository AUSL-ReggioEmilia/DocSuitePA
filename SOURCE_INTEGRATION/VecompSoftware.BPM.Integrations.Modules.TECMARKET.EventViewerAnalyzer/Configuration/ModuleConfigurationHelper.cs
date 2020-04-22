using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using VecompSoftware.BPM.Integrations.Modules.TECMARKET.EventViewerAnalyzer.Models;

namespace VecompSoftware.BPM.Integrations.Modules.TECMARKET.EventViewerAnalyzer.Configuration
{
    public static class ModuleConfigurationHelper
    {
        #region [ Fields ]
        public const string MODULE_NAME = "VecompSoftware.BPM.Integrations.Modules.TECMARKET.EventViewerAnalyzer";
        private const string MODULE_CONFIGURATION_NAME = "module.configuration.json";
        private static readonly string _modulePath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Execution)).Location);
        private static string _queryPath;
        private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.All
        };
        #endregion

        #region [ Properties ]
        public static string CurrentModulePath => _modulePath;
        public static JsonSerializerSettings JsonSerializerSettings => _serializerSettings;
        public static string QueryStringPath => _queryPath;

        #endregion

        #region [ Constructor ]

        #endregion

        #region [ Methods ]
        public static ModuleConfigurationModel GetModuleConfiguration()
        {
            ModuleConfigurationModel ModelConfiguration = JsonConvert.DeserializeObject<ModuleConfigurationModel>(File.ReadAllText(Path.Combine(CurrentModulePath, MODULE_CONFIGURATION_NAME)));
            _queryPath = Path.Combine(CurrentModulePath, ModelConfiguration.PathQueryXpath);
            return ModelConfiguration;
        }
        #endregion
    }
}
