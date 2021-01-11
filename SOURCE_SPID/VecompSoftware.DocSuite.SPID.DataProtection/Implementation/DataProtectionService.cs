using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Text;
using VecompSoftware.DocSuite.SPID.Common.Helpers.Core;

namespace VecompSoftware.DocSuite.SPID.DataProtection
{
    public class DataProtectionService : IDataProtectionService
    {
        #region [ Fields ]
        private readonly IKeyManager _keyManager;
        private readonly IDataProtector _dataProtector;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public DataProtectionService(IKeyManager keyManager, IDataProtectionProvider dataProtectionProvider)
        {
            _keyManager = keyManager;
            _dataProtector = dataProtectionProvider.CreateProtector(DataProtectionHelper.SPID_DATA_PROTECTION_SHARED_NAME);
            Init();
        }
        #endregion

        #region [ Methods ]
        public SymmetricSecurityKey GetSigningKey()
        {
            if (_keyManager == null)
            {
                throw new ArgumentNullException("keyManager", "GetSymmetricSecurityKey -> KeyManager provider is null");
            }

            IKey dataProviderKey = _keyManager.GetAllKeys().FirstOrDefault(x => x.IsRevoked == false);
            if (dataProviderKey == null)
            {
                throw new Exception("GetSymmetricSecurityKey -> data provider key not found");
            }

            string secretKey = dataProviderKey.Descriptor.ExportToXml().SerializedDescriptorElement.Value;
            SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            return signingKey;
        }

        public string Protect(string content)
        {
            return _dataProtector.Protect(content);
        }

        public string Unprotect(string protectedContent)
        {
            return _dataProtector.Unprotect(protectedContent);
        }

        private void Init()
        {
            _dataProtector.Protect(string.Empty);
        }
        #endregion        
    }
}
