using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.ServiceHosting.ServiceRuntime;
using System.Globalization;

namespace BiblosDS.Library.Storage.Azure
{
    /// <summary>
    /// Objects of this class encapsulate information about a storage account and endpoint configuration. 
    /// Associated with a storage account is the account name, the base URI of the account and a shared key. 
    /// </summary>
    public class StorageAccountInfo
    {

        /// <summary>
        /// The default configuration string in configuration files for setting the queue storage endpoint.
        /// </summary>
        public static readonly string DefaultQueueStorageEndpointConfigurationString = "QueueStorageEndpoint";

        /// <summary>
        /// The default configuration string in configuration files for setting the blob storage endpoint.
        /// </summary>
        public static readonly string DefaultBlobStorageEndpointConfigurationString = "BlobStorageEndpoint";

        /// <summary>
        /// The default configuration string in configuration files for setting the table storage endpoint.
        /// </summary>
        public static readonly string DefaultTableStorageEndpointConfigurationString = "TableStorageEndpoint";

        /// <summary>
        /// The default configuration string in configuration files for setting the storage account name.
        /// </summary>
        public static readonly string DefaultAccountNameConfigurationString = "AccountName";

        /// <summary>
        /// The default configuration string in configuration files for setting the shared key associated with a storage account.
        /// </summary>
        public static readonly string DefaultAccountSharedKeyConfigurationString = "AccountSharedKey";

        /// <summary>
        /// The default configuration string in configuration files for setting the UsePathStyleUris option.
        /// </summary>
        public static readonly string DefaultUsePathStyleUrisConfigurationString = "UsePathStyleUris";

        /// <summary>
        /// The default prefix string in application config and Web.config files to indicate that this setting should be looked up 
        /// in the fabric's configuration system.
        /// </summary>
        public static readonly string CSConfigStringPrefix = "CSConfigName";

        private bool? _usePathStyleUris;

        /// <summary>
        /// Constructor for creating account info objects.
        /// </summary>
        /// <param name="baseUri">The account's base URI.</param>
        /// <param name="usePathStyleUris">If true, path-style URIs (http://baseuri/accountname/containername/objectname) are used.
        /// If false host-style URIs (http://accountname.baseuri/containername/objectname) are used,
        /// where baseuri is the URI of the service..
        /// If null, the choice is made automatically: path-style URIs if host name part of base URI is an 
        /// IP addres, host-style otherwise.</param>
        /// <param name="accountName">The account name.</param>
        /// <param name="base64Key">The account's shared key.</param>
        public StorageAccountInfo(Uri baseUri, bool? usePathStyleUris, string accountName, string base64Key)
            : this(baseUri, usePathStyleUris, accountName, base64Key, false)
        {
        }

        /// <summary>
        /// Constructor for creating account info objects.
        /// </summary>
        /// <param name="baseUri">The account's base URI.</param>
        /// <param name="usePathStyleUris">If true, path-style URIs (http://baseuri/accountname/containername/objectname) are used.
        /// If false host-style URIs (http://accountname.baseuri/containername/objectname) are used,
        /// where baseuri is the URI of the service.
        /// If null, the choice is made automatically: path-style URIs if host name part of base URI is an 
        /// IP addres, host-style otherwise.</param>
        /// <param name="accountName">The account name.</param>
        /// <param name="base64Key">The account's shared key.</param>
        /// <param name="allowIncompleteSettings">true if it shall be allowed to only set parts of the StorageAccountInfo properties.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        public StorageAccountInfo(Uri baseUri, bool? usePathStyleUris, string accountName, string base64Key, bool allowIncompleteSettings)
        {
            if (baseUri == null && !allowIncompleteSettings)
            {
                throw new ArgumentNullException("baseUri");
            }
            if (string.IsNullOrEmpty(base64Key) && !allowIncompleteSettings)
            {
                throw new ArgumentNullException("base64Key");
            }
            if (baseUri != null)
            {
                string newAccountName = null;
                Uri newBaseUri = null;
                if (IsStandardStorageEndpoint(baseUri, out newAccountName, out newBaseUri))
                {
                    if (!string.IsNullOrEmpty(newAccountName) &&
                        !string.IsNullOrEmpty(accountName) &&
                        string.Compare(accountName, newAccountName, StringComparison.Ordinal) != 0)
                    {
                        throw new ArgumentException("The configured base URI " + baseUri.AbsoluteUri + " for the storage service is incorrect. " +
                                                    "The configured account name " + accountName + " must match the account name " + newAccountName +
                                                    " as specified in the storage service base URI." +
                                                     GeneralAccountConfigurationExceptionString);
                    }
                    Debug.Assert((newBaseUri == null && newAccountName == null) || (newBaseUri != null && newAccountName != null));
                    if (newAccountName != null && newBaseUri != null)
                    {
                        accountName = newAccountName;
                        baseUri = newBaseUri;
                    }
                }
            }
            if (string.IsNullOrEmpty(accountName) && !allowIncompleteSettings)
            {
                throw new ArgumentNullException("accountName");
            }
            if (!string.IsNullOrEmpty(accountName) && accountName.ToLowerInvariant() != accountName)
            {
                throw new ArgumentException("The account name must not contain upper-case letters. " +
                                "The account name is the first part of the URL for accessing the storage services as presented to you by the portal or " +
                                "the predefined storage account name when using the development storage tool. " +
                                GeneralAccountConfigurationExceptionString);
            }

            BaseUri = baseUri;
            AccountName = accountName;
            Base64Key = base64Key;
            if (usePathStyleUris == null && baseUri == null && !allowIncompleteSettings)
            {
                throw new ArgumentException("Cannot determine setting from empty URI.");
            }
            else if (usePathStyleUris == null)
            {
                if (baseUri != null)
                {
                    _usePathStyleUris = Utilities.StringIsIPAddress(baseUri.Host);
                }
                else
                {
                    _usePathStyleUris = null;
                }
            }
            else
            {
                UsePathStyleUris = usePathStyleUris.Value;
            }
        }

        /// <summary>
        /// The base URI of the account.
        /// </summary>
        public Uri BaseUri
        {
            get;
            set;
        }

        /// <summary>
        /// The account name.
        /// </summary>
        public string AccountName
        {
            get;
            set;
        }

        /// <summary>
        /// The account's key.
        /// </summary>
        public string Base64Key
        {
            get;
            set;
        }

        /// <summary>
        /// If set, returns the UsePathStyleUris properties. If the property has not been explicitly set, 
        /// the implementation tries to derive the correct value from the base URI.
        /// </summary>
        public bool UsePathStyleUris
        {
            get
            {
                if (_usePathStyleUris == null)
                {
                    if (BaseUri == null)
                    {
                        return false;
                    }
                    else
                    {
                        return Utilities.StringIsIPAddress(BaseUri.Host);
                    }
                }
                else
                {
                    return _usePathStyleUris.Value;
                }
            }
            set
            {
                _usePathStyleUris = value;
            }
        }    

        /// <summary>
        /// Retrieves account information settings from configuration settings. First, the implementation checks for 
        /// settings in an application config section of an app.config or Web.config file. These values are overwritten 
        /// if the same settings appear in a .csdef file.
        /// The implementation also supports indirect settings. In this case, indirect settings overwrite all other settings.
        /// </summary>        
        /// <param name="accountNameConfiguration">Configuration string for the account name.</param>
        /// <param name="accountSharedKeyConfiguration">Configuration string for the key.</param>
        /// <param name="endpointConfiguration">Configuration string for the endpoint.</param>
        /// <param name="usePathStyleUrisConfiguration">Configuration string for the path style.</param>
        /// <param name="allowIncompleteSettings">If false, an exception is thrown if not all settings are available.</param>
        /// <returns>StorageAccountInfo object containing the retrieved settings.</returns>        
        public static StorageAccountInfo GetAccountInfoFromConfiguration(
            string StorageName, 
            string MainPath,
            string StorageKey,
            bool allowIncompleteSettings)
        {            
            string endpoint = MainPath;
            string name = StorageName;
            string key = StorageKey;
            string pathStyle = null;                 

            if (string.IsNullOrEmpty(key) && !allowIncompleteSettings)
            {
                throw new ArgumentException("No account key specified!");
            }
            if (string.IsNullOrEmpty(endpoint) && !allowIncompleteSettings)
            {
                throw new ArgumentException("No endpoint specified!");
            }
            if (string.IsNullOrEmpty(name))
            {
                // in this case let's try to derive the account name from the Uri
                string newAccountName = null;
                Uri newBaseUri = null;
                if (IsStandardStorageEndpoint(new Uri(endpoint), out newAccountName, out newBaseUri))
                {
                    Debug.Assert((newAccountName != null && newBaseUri != null) || (newAccountName == null && newBaseUri == null));
                    if (newAccountName != null && newBaseUri != null)
                    {
                        //endpoint = newBaseUri.AbsoluteUri;
                        name = newAccountName;
                    }
                }
                if (string.IsNullOrEmpty(name) && !allowIncompleteSettings)
                {
                    throw new ArgumentException("No account name specified.");
                }
            }

            bool? usePathStyleUris = null;
            if (!string.IsNullOrEmpty(pathStyle))
            {
                bool b;
                if (!bool.TryParse(pathStyle, out b))
                {
                    throw new Exception("Cannot parse value of setting UsePathStyleUris as a boolean");
                }
                usePathStyleUris = b;
            }
            Uri tmpBaseUri = null;
            if (!string.IsNullOrEmpty(endpoint))
            {
                tmpBaseUri = new Uri(endpoint);
            }
            return new StorageAccountInfo(tmpBaseUri, usePathStyleUris, name, key, allowIncompleteSettings);
        }


        /// <summary>
        /// Checks whether all essential properties of this object are set. Only then, the account info object 
        /// should be used in ohter APIs of this library.
        /// </summary>
        /// <returns></returns>
        public bool IsCompleteSetting()
        {
            return BaseUri != null && Base64Key != null && AccountName != null;
        }

        /// <summary>
        /// Checks whether this StorageAccountInfo object is complete in the sense that all properties are set.
        /// </summary>
        public void CheckComplete()
        {
            if (!IsCompleteSetting())
            {
                throw new Exception("Account information incomplete!");
            }
        }

        #region Private methods

        private static string TryGetConfigurationSetting(string configName)
        {
            string ret = null;
            try
            {
                ret = RoleManager.GetConfigurationSetting(configName);
            }
            catch (RoleException)
            {
                return null;
            }
            return ret;
        }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
        //                                                  Justification = "Make sure that nothing prevents us to read from the fabric's configuration envrionment.")]
        //private static string TryGetAppSetting(string configName)
        //{
        //    string ret = null;
        //    try
        //    {
        //        ret = ConfigurationSettings.AppSettings[configName];
        //    }
        //    // some exception happened when accessing the app settings section
        //    // most likely this is because there is no app setting file
        //    // we assume that this is because there is no app settings file; this is not an error
        //    // and explicitly all exceptions are captured here
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //    return ret;
        //}

        private static string GeneralAccountConfigurationExceptionString
        {
            get
            {
                return "If the portal defines http://test.blob.core.windows.net as your blob storage endpoint, the string \"test\" " +
                       "is your account name, and you can specify http://blob.core.windows.net as the BlobStorageEndpoint in your " +
                       "service's configuration file(s).";
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        private static bool IsStandardStorageEndpoint(Uri baseUri, out string newAccountName, out Uri newBaseUri)
        {
            if (baseUri == null)
            {
                throw new ArgumentNullException("baseUri");
            }

            newAccountName = null;
            newBaseUri = null;

            string host = baseUri.Host;
            if (string.IsNullOrEmpty(host))
            {
                throw new ArgumentException("The host part of the Uri " + baseUri.AbsoluteUri + " must not be null or empty.");
            }
            if (host != host.ToLowerInvariant())
            {
                throw new ArgumentException("The specified host string " + host + " must not contain upper-case letters.");
            }

            string suffix = null;
            if (host.EndsWith(StandardPortalEndpoints.TableStorageEndpoint, StringComparison.Ordinal))
            {
                suffix = StandardPortalEndpoints.TableStorageEndpoint;
            }
            if (host.EndsWith(StandardPortalEndpoints.BlobStorageEndpoint, StringComparison.Ordinal))
            {
                suffix = StandardPortalEndpoints.BlobStorageEndpoint;
            }
            if (host.EndsWith(StandardPortalEndpoints.QueueStorageEndpoint, StringComparison.Ordinal))
            {
                suffix = StandardPortalEndpoints.QueueStorageEndpoint;
            }
            // a URL as presented on the portal was specified, lets find out whether it is in the correct format
            if (suffix != null)
            {
                int index = host.IndexOf(suffix, StringComparison.Ordinal);
                Debug.Assert(index != -1);
                if (index > 0)
                {
                    string first = host.Substring(0, index);
                    Debug.Assert(!string.IsNullOrEmpty(first));
                    if (first[first.Length - 1] != ConstChars.Dot[0])
                    {
                        return false;
                    }
                    first = first.Substring(0, first.Length - 1);
                    if (string.IsNullOrEmpty(first))
                    {
                        throw new ArgumentException("The configured base URI " + baseUri.AbsoluteUri + " for the storage service is incorrect. " +
                                                     GeneralAccountConfigurationExceptionString);
                    }
                    if (first.Contains(ConstChars.Dot))
                    {
                        throw new ArgumentException("The configured base URI " + baseUri.AbsoluteUri + " for the storage service is incorrect. " +
                                                     GeneralAccountConfigurationExceptionString);
                    }
                    newAccountName = first;
                    newBaseUri = new Uri(baseUri.Scheme + Uri.SchemeDelimiter + suffix + baseUri.PathAndQuery);
                }
                return true;
            }
            return false;
        }


        #endregion

    }
}
