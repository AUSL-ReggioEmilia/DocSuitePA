using System;
using System.IO;
using System.Linq;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.JeepService.CleanFolders
{
    public class FolderBaseType
    {
        #region [ Properties ]
        public string FolderPath { get; set; }
        public string Name { get; set; }
        public int FileNumber { get; set; }

        public CleanFoldersParameters Parameters { get; private set; }
        protected Action<string> SendMessage { get; set; }        
        #endregion

        #region [ Constructor ]
        public FolderBaseType(string path, Action<string> sendMessage, CleanFoldersParameters parameters)
        {
            var di = new DirectoryInfo(path);
            FolderPath = path;
            Name = di.Name;
            FileNumber = di.EnumerateFiles().Count();
            SendMessage = sendMessage;
            Parameters = parameters;
        }
        #endregion

        #region [ Methods ]
        protected String[] GetAll()
        {
            if (!Directory.Exists(FolderPath))
                return new string[] { };

            var fileList = Directory.GetFiles(FolderPath);
            return fileList;
        }

        public virtual void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }

        protected void UpdateCleanFoldersException(string text, FolderType type)
        {
            // Aggiungo la riga al file
            var fileNameFormat = String.Format("{0}_CleanFoldersExceptions_{1}.txt", DateTime.Today.ToString("yyyy_MM_dd"), type);
            var path = Path.Combine(Parameters.TempFolder, fileNameFormat);
            // Se il file non esiste allora lo creo
            if (!File.Exists(path)) File.Create(path).Dispose();
            // Apro il file e ci scrivo
            using (var sw = File.AppendText(path))
            {
                sw.WriteLine("{0} -> {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), text);
            }
        }

        public void MailErrorMessage()
        {
            try
            {
                var filesToSend = Directory.GetFiles(Parameters.TempFolder, "*_CleanFoldersExceptions_*.txt");
                if(!filesToSend.Any())
                    return;
                // Se esistono i file li spedisco
                foreach (var file in filesToSend)
                {
                    SendMessage(String.Format(
                       "Riepilogo errori di cancellazione del giorno {0}{1}{2}",
                       DateTime.Today.ToString("dd/MM/yyyy"), Environment.NewLine, File.ReadAllText(file)));
                    File.Delete(file);   
                }                
            }
            catch (Exception ex)
            {
                FileLogger.Error(Name, "Errore in [MailMessage].", ex);
                SendMessage(String.Format("Errore in [MailMessage]. \nErrore: {0} \nStacktrace: {1}", ex.Message, ex.StackTrace));
            }
        }
        #endregion
    }
}
