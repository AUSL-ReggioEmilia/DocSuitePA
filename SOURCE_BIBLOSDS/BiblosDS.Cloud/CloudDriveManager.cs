using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.IO;
using System.Diagnostics;
using log4net;


namespace BiblosDS.Cloud
{
    public class CloudDriveManager
    {
        public const string STORAGE_ACCOUNT_SETTING = "BiblosVhdStorage";
        CloudStorageAccount _account;
        CloudDrive _cloudDrive = null;
        string _vhdName;
        string _vhdUrl;
        char _driveLetter;
        int _cacheSize;
        LocalResource _cache;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(CloudDriveManager));

        public const int DISK_SIZE = 4096;
        public int DELTA_SIZE = 40;

        public string VHD_Url
        {
            get
            {
                return _vhdUrl;
            }
        }

        public CloudDriveManager(CloudStorageAccount account, string vhdPathAndName, LocalResource cache)
        {
            _account = account;
            _vhdName = vhdPathAndName;
            _cacheSize = cache.MaximumSizeInMegabytes;
            _cache = cache;
            try
            {
                DELTA_SIZE = int.Parse(RoleEnvironment.GetConfigurationSettingValue("BiblosVhdStorageDeltaSizeMB"));
                if (DELTA_SIZE < 40)
                    DELTA_SIZE = 40;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public static CloudDriveManager MountAllDrives(string DRIVE_SETTINGS, string DCACHE_NAME, bool delete)
        {
            CloudDriveManager _cloudDriveManager = null;
            try
            {
                _logger.Info("MountAllDrives");
                var driveSettings = RoleEnvironment.GetConfigurationSettingValue(DRIVE_SETTINGS);
                CloudStorageAccount account = CloudStorageAccount.FromConfigurationSetting(STORAGE_ACCOUNT_SETTING);
                string dCacheName = RoleEnvironment.GetConfigurationSettingValue(DCACHE_NAME);
                LocalResource cache = RoleEnvironment.GetLocalResource(dCacheName);
                int cacheSize = cache.MaximumSizeInMegabytes / 2;
                string[] settings = driveSettings.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in settings)
                {
                    _logger.InfoFormat("Mount {0}", item);
                    _cloudDriveManager = new CloudDriveManager(account, settings[0], cache);
                    _cloudDriveManager.UnmountAll();
                    _cloudDriveManager.CreateDrive(DISK_SIZE, delete);
                    _cloudDriveManager.Mount();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return _cloudDriveManager;
        }

        public CloudDriveManager()
        {
            _account = CloudStorageAccount.FromConfigurationSetting(STORAGE_ACCOUNT_SETTING);            
        }

        public IDictionary<string, Uri> GetMountedDrive()
        {
            return CloudDrive.GetMountedDrives();            
        }

        public bool Unmount()
        {
            if (_cloudDrive == null)
                return false;
            try
            {                
                _logger.InfoFormat("Unmount {0}", _cloudDrive.Uri);
                _cloudDrive.Unmount();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }

        public void UnmountAll()
        {
            foreach (var drive in CloudDrive.GetMountedDrives())
            {
                try
                {
                    _logger.InfoFormat("Unmount {0}", drive.Value);
                    var mountedDrive = _account.CreateCloudDrive(drive.Value.PathAndQuery);                    
                    mountedDrive.Unmount();
                }
                catch (Exception ex)
                {
                    _logger.Info(ex);                    
                }                
            }           
        }

        public string Mount()
        {
            if (_cloudDrive == null)
                throw new Exception("_cloudDrive non definito"); 
            var driveLetter = _cloudDrive.Mount(_cacheSize, DriveMountOptions.Force);
            _logger.DebugFormat("mounted drive letter {0}", driveLetter);
            return driveLetter;
        }

        public void CreateDrive(string container, string vhdName, int totalSize, bool delete)
        {
            try
            {                
                totalSize = Math.Max(totalSize/(1024*1024), 16);                
                if (totalSize > 16)
                    totalSize = (totalSize / 5) + totalSize;
                totalSize = totalSize + DELTA_SIZE;
                CloudBlobClient blobClient = _account.CreateCloudBlobClient();
                CloudBlobContainer drives = blobClient.GetContainerReference(container);
                try
                {
                    drives.CreateIfNotExist();
                }
                catch (Exception ex) { _logger.Warn("Arror on CreateIfNotExist", ex); }
                var containerDisk = blobClient.GetContainerReference(container);
                _vhdUrl = containerDisk.GetPageBlobReference(vhdName).Uri.ToString();
                containerDisk.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Container });
                _logger.Info("CreateDrive " + _vhdUrl);
                _cloudDrive = _account.CreateCloudDrive(_vhdUrl);                
                _logger.Info("using account " + _account.BlobEndpoint + " to create " + vhdName);
                var created = _cloudDrive.CreateIfNotExist(totalSize);
                if (delete && !created)
                {
                    _logger.Info("using account " + _account.BlobEndpoint + " exists " + vhdName);
                    _logger.Info("using account " + _account.BlobEndpoint + " delete " + vhdName);
                    _cloudDrive.Delete();
                    _logger.Info("using account " + _account.BlobEndpoint + " recreate " + vhdName);
                    _cloudDrive.CreateIfNotExist(totalSize);

                }
            }
            catch (StorageClientException ex)
            {
                if (ex.ErrorCode == StorageErrorCode.ContainerAlreadyExists)
                {
                    try
                    {
                        if (delete)
                        {
                            _cloudDrive.Delete();
                            _cloudDrive.CreateIfNotExist(totalSize);
                        }
                    }
                    catch (Exception ex1)
                    {
                        _logger.Info("error on CreateDrive: " + ex1.ToString());
                    }                      
                }
            }
            catch (Exception ex)
            {
                _logger.Info("error on CreateDrive: " + ex.ToString());
            }
        }

        public void CreateDriveFromUrl(string url)
        {
            _vhdUrl = url;
            _cloudDrive = _account.CreateCloudDrive(_vhdUrl);              
            _cloudDrive.CreateIfNotExist(DISK_SIZE);              
        }

        public void CreateDrive(int totalSize, bool delete)
        {
            try
            {
                CloudBlobClient blobClient = _account.CreateCloudBlobClient();
                CloudBlobContainer drives = blobClient.GetContainerReference("drives");
                try 
                { 
                    drives.CreateIfNotExist(); 
                }
                catch { }
                var vhdUrl = blobClient.GetContainerReference("drives").GetPageBlobReference(_vhdName).Uri.ToString();
                _logger.Info("CreateDrive " + vhdUrl);
                _cloudDrive = _account.CreateCloudDrive(vhdUrl);
                _logger.Info("using account " + _account.BlobEndpoint + " to create " + _vhdName);
                if (_cloudDrive.CreateIfNotExist(totalSize))
                {
                    if (delete)
                        _cloudDrive.Delete();
                }
            }
            catch (Exception ex)
            {
                _logger.Info("error on CreateDrive: " + ex.ToString());
            }
        }        
    }
}
