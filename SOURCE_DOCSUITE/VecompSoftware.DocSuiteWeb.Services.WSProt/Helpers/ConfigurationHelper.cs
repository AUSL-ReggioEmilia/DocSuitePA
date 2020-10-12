using System;
using System.Configuration;

namespace VecompSoftware.DocSuiteWeb.Services.WSProt.Helpers
{
    public class ConfigurationHelper
    {
        #region [ Fields ]
        private static Guid? _currentTenantAOOId;
        #endregion

        #region [ Properties ]        
        public static Guid CurrentTenantAOOId
        {
            get
            {
                if (!_currentTenantAOOId.HasValue)
                {
                    _currentTenantAOOId = Guid.Parse(ConfigurationManager.AppSettings["TenantAOOId"]);
                }
                return _currentTenantAOOId.Value;
            }
        }
        #endregion
    }
}