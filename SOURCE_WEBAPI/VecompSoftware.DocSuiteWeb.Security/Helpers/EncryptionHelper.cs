using SecurityEncryptionHelper = VecompSoftware.Helpers.Security;

namespace VecompSoftware.DocSuiteWeb.Security
{
    public static class EncryptionHelper
    {
        #region [ Methods ]
        public static string EncryptString(string plaintext, string key) 
        {
            return SecurityEncryptionHelper.EncryptionHelper.EncryptString(plaintext, key);
        }
        public static string DecryptString(string plaintext, string key)
        {
            return SecurityEncryptionHelper.EncryptionHelper.DecryptString(plaintext, key);
        }
        #endregion
    }
}
