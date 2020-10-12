using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using VecompSoftware.BPM.Integrations.Modules.SM.ACATA.Models;

namespace VecompSoftware.BPM.Integrations.Modules.SM.ACATA.Configuration
{
    public static class ModuleConfigurationHelper
    {
        #region [ Fields ]
        public const string MODULE_NAME = "VecompSoftware.BPM.Integrations.Modules.SM.ACATA";
        private const string MODULE_CONFIGURATION_NAME = "module.configuration.json";

        #endregion

        #region [ Properties ]
        public static string CurrentModulePath { get; } = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Execution)).Location);
        public static JsonSerializerSettings JsonSerializerSettings { get; } = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.All
        };

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
