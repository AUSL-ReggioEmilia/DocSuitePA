using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using VecompSoftware.BPM.Integrations.Modules.DGROOVE.ImportCedolini.Models;

namespace VecompSoftware.BPM.Integrations.Modules.DGROOVE.ImportCedolini
{
    public static class ModuleConfigurationHelper
    {
        #region [ Fields ]
        public const string MODULE_NAME = "VecompSoftware.BPM.Integrations.Modules.DGROOVE.ImportCedolini";
        private const string MODULE_CONFIGURATION_NAME = "module.configuration.json";
        private static readonly string _modulePath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Execution)).Location);
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

        #endregion

        #region [ Constructor ]

        #endregion

        #region [ Methods ]
        public static ModuleConfigurationModel GetModuleConfiguration()
        {
            return JsonConvert.DeserializeObject<ModuleConfigurationModel>(File.ReadAllText(Path.Combine(CurrentModulePath, MODULE_CONFIGURATION_NAME)));
        }
        #endregion
    }
}
