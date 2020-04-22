using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web.Configuration;

namespace VecompSoftware.Commons.Configuration
{
    public static class ConfigurationHelper
    {

        private static void DeclareSettingsIfNotExists(string key)
        {
            if (WebConfigurationManager.AppSettings.AllKeys.Contains(key))
                return;

            WebConfigurationManager.AppSettings.Add(key, string.Empty);
            var config = WebConfigurationManager.OpenWebConfiguration("~/");
            config.AppSettings.Settings.Add(key, string.Empty);
            config.Save(ConfigurationSaveMode.Modified);

            ConfigurationManager.RefreshSection("appSettings");
        }

        public static string GetValue(string key)
        {
            DeclareSettingsIfNotExists(key);

            var result = WebConfigurationManager.AppSettings[key];
            result = result.Trim();
            return result;
        }
        public static T GetValue<T>(string key)
        {
            var value = GetValue(key);
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public static T GetValueOrDefault<T>(string key, T defaultValue)
        {
            try
            {
                var value = GetValue(key);
                if (string.IsNullOrEmpty(value))
                    return defaultValue;

                return GetValue<T>(key);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
            return defaultValue;
        }
        public static T GetValueOrDefault<T>(string key)
        {
            return GetValueOrDefault<T>(key, default(T));
        }

    }
}
