using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vecompsoftware.FileServer.Services;

namespace VecompSoftware.JeepDashboard.Code
{
    static class LogsHelper
    {
        /// <summary>
        /// Definisce se fisicamente il servizio esiste
        /// </summary>
        /// <param name="filter"> </param>
        /// <param name="client"></param>
        /// <param name="logFolderPath"> </param>
        /// <returns></returns>
        public static string[] GetFiles(string logFolderPath, string filter, IFileRepositoryService client)
        {
            if (!string.IsNullOrEmpty(logFolderPath) && client != null)
            {
                var fileList = client.ListFiles(logFolderPath, filter);
                return fileList.Select(storageFileInfo => storageFileInfo.Name).ToArray();
            }
            return new DirectoryInfo(logFolderPath).GetFiles(filter).Select(fileInfo => fileInfo.Name).ToArray();
        }

        /// <summary>
        /// Carica uno specifico file di Log
        /// </summary>
        /// <param name="logFilePath"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static Stream GetFile(FileInfo logFilePath, IFileRepositoryService client)
        {
            return client != null ? client.GetFile(logFilePath.FullName) : File.Open(logFilePath.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
    }
}
