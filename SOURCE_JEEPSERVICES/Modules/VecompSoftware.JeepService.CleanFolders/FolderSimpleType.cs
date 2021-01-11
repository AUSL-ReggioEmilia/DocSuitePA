using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.JeepService.CleanFolders
{
    public class FolderSimpleType : FolderBaseType
    {
        #region [ Variables ]
        private Func<bool> _cancelFunc;
        private string _loggerName;
        #endregion

        #region [ Constructor ]
        public FolderSimpleType(Func<bool> cancelFunc, Action<string> sendMessage, string loggerName, string path, CleanFoldersParameters parameters)
            :base(path, sendMessage, parameters)
        {
            _cancelFunc = cancelFunc;
            _loggerName = loggerName;
        }
        #endregion

        #region [ Methods ]
        private IEnumerable<FileInfo> GetFilesByParam(string path)
        {
            var files =
                Parameters.Filters.Split('|')
                    .SelectMany(filter => new DirectoryInfo(path).GetFiles(filter, SearchOption.TopDirectoryOnly));

            if(Parameters.MaxNumberDaysToKeep > 0)
                return files.Where(x => x.CreationTime < DateTime.Now.Subtract(TimeSpan.FromDays(Parameters.MaxNumberDaysToKeep)));

            return files;
        }

        public void Process()
        {
            var listOfFiles = GetFilesByParam(FolderPath);
            var fileInfos = listOfFiles as IList<FileInfo> ?? listOfFiles.ToList();
            var counter = fileInfos.Count();
            FileLogger.Debug(_loggerName, String.Format("[FolderSimpleType - Process]. Inizio processo {0} file.", counter));
            foreach (var file in fileInfos)
            {
                if (_cancelFunc())
                {
                    FileLogger.Info(_loggerName, "Chiusura modulo invocata dall'utente.");
                    return;
                }
                FileLogger.Debug(_loggerName, String.Format("[FolderSimpleType - Process]. Procedo alla cancellazione del file {0} dalla directory {1}.", file.Name, FolderPath));
                DeleteFile(file.FullName);
            }
            FileLogger.Debug(_loggerName, String.Format("[FolderSimpleType - Process]. Fine processo {0} file.", counter));
        }

        public override void DeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    FileLogger.Debug(_loggerName,
                        String.Format(
                            "[FolderSimpleType - DeleteFile]. Cancellazione del file {0} avvenuta correttamente.",
                            Path.GetFileName(filePath)));
                }
                else
                {
                    throw new FileNotFoundException("File non trovato", filePath);
                }
            }
            catch (Exception ex)
            {
                FileLogger.Error(_loggerName, String.Format(
                    "Errore in [FolderSimpleType - DeleteFile]. E' avvenuto un errore nella cancellazione del file {0}. {1}{2}",
                    filePath, Environment.NewLine, ex.Message));
                UpdateCleanFoldersException(String.Format(
                    "Errore in [FolderSimpleType - DeleteFile]. E' avvenuto un errore nella cancellazione del file {0}.",
                    filePath), FolderType.Simple);
            }
        }
        #endregion
    }
}
