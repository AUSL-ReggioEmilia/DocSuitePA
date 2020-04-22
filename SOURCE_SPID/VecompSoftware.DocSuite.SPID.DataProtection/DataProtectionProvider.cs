using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using VecompSoftware.DocSuite.SPID.Common.Helpers.Core;
using VecompSoftware.DocSuite.SPID.Model.Common;

namespace VecompSoftware.DocSuite.SPID.DataProtection
{
    public static class DataProtectionProvider
    {
        public static void AddProtectionData(this IServiceCollection services, IConfiguration configuration)
        {
            IConfigurationSection dataProtectionSettings = configuration.GetSection(nameof(DataProtectionConfiguration));
            string secretStoragePath = dataProtectionSettings.GetValue<string>(nameof(DataProtectionConfiguration.SecretKeyStoragePath));
            TokenKeysProtectionType protectionType = dataProtectionSettings.GetValue<TokenKeysProtectionType>(nameof(DataProtectionConfiguration.ProtectionType));

            if (protectionType == TokenKeysProtectionType.WindowsDpapi)
            {
                services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo(secretStoragePath))
                    .ProtectKeysWithDpapi(protectToLocalMachine: true)
                    .SetApplicationName(DataProtectionHelper.SPID_DATA_PROTECTION_SHARED_NAME);
            }
            else
            {
                string x509CertificateThumbprint = dataProtectionSettings.GetValue<string>(nameof(DataProtectionConfiguration.X509CertificateThumbprint));
                string cleanedx509CertificateThumbprint = Regex.Replace(x509CertificateThumbprint, @"\s|\W", "").ToUpper();
                using (X509Store x509Store = new X509Store(StoreName.My, StoreLocation.CurrentUser, OpenFlags.ReadOnly))
                {
                    X509Certificate2Collection x509Certificates = x509Store.Certificates.Find(X509FindType.FindByThumbprint, cleanedx509CertificateThumbprint, false);
                    services.AddDataProtection()
                        .PersistKeysToFileSystem(new DirectoryInfo(secretStoragePath))
                        .ProtectKeysWithCertificate(x509Certificates[0])
                        .SetApplicationName(DataProtectionHelper.SPID_DATA_PROTECTION_SHARED_NAME);
                }
            }

            services.AddSingleton<IDataProtectionService, DataProtectionService>();
        }
    }
}
