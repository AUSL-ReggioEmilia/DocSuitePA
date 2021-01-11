using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms.Design;
using VecompSoftware.JeepService.Common;

namespace VecompSoftware.JeepService
{
    public class BDSImporterParameters : JeepParametersBase
    {

        #region [ Constants ]

        public const string InputFolderName = "Input";
        public const string OutputFolderName = "Output";
        public const string XsdsFolderName = "Xsds";
        public const string RejectedFolderName = "Scarto";

        public const string defaultWorkingFolderName = "BSDImporterWorkingFolder";

        #endregion

        #region [ Fields ]

        private string _localWorkingFolder;
        private string _inputFolder;
        private string _outputFolder;
        private string _xsdsFolder;
        private string _rejectedFolder;

        #endregion

        #region [ Properties ]

        private string LocalWorkingFolder
        {
            get
            {
                if (!string.IsNullOrEmpty(WorkingFolder))
                    return WorkingFolder;
                if (string.IsNullOrEmpty(_localWorkingFolder))
                    _localWorkingFolder = Path.Combine(Environment.CurrentDirectory, defaultWorkingFolderName);
                return _localWorkingFolder;
            }
        }

        // Configuration
        [Category("Configuration")]
        [Description("Cartella nella quale il modulo eseguirà i cicli di lavoro.")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string WorkingFolder { get; set; }

        [Description("Cartella di Input figlia di WorkingFolder.")]
        [ReadOnly(true)]
        public string InputFolder
        {
            get
            {
                if (!string.IsNullOrEmpty(LocalWorkingFolder) && string.IsNullOrEmpty(_inputFolder))
                    _inputFolder = Path.Combine(LocalWorkingFolder, InputFolderName);

                return _inputFolder;
            }
        }
        [Description("Cartella di Output figlia di WorkingFolder.")]
        [ReadOnly(true)]
        public string OutputFolder
        {
            get
            {
                if (!string.IsNullOrEmpty(LocalWorkingFolder) && string.IsNullOrEmpty(_outputFolder))
                    _outputFolder = Path.Combine(LocalWorkingFolder, OutputFolderName);

                return _outputFolder;
            }
        }
        [Description("Cartella contenente gli xsd figlia di WorkingFolder.")]
        [ReadOnly(true)]
        public string XsdsFolder
        {
            get
            {
                if (!string.IsNullOrEmpty(LocalWorkingFolder) && string.IsNullOrEmpty(_xsdsFolder))
                    _xsdsFolder = Path.Combine(LocalWorkingFolder, XsdsFolderName);

                return _xsdsFolder;
            }
        }

        [Description("Cartella contenente i documenti con validazione errata della firma figlia di WorkingFolder.")]
        [ReadOnly(true)]
        public string RejectedFolder
        {
            get
            {
                if (!string.IsNullOrEmpty(LocalWorkingFolder) && string.IsNullOrEmpty(_rejectedFolder))
                    _rejectedFolder = Path.Combine(LocalWorkingFolder, RejectedFolderName);

                return _rejectedFolder;
            }
        }

        [Category("Configuration")]
        [DefaultValue("SRVBIBLOS")]
        [Description("Nome del server Biblos destinatario dell'archiviazione.")]
        public string BiblosServerName { get; set; }

        [Category("Configuration")]
#if DEBUG
        [DefaultValue("BDSImporter")]
#endif
        [Description("Archivio di Biblos.")]
        public string BiblosArchive { get; set; }

        [Category("Configuration")]
        [DefaultValue("{0:yyyy-MM-dd HH:mm:ss}")]
        [Description("Formato data/ora di Biblos.")]
        public string BiblosDateTimeFormat { get; set; }

        [Category("Configuration")]
        [DefaultValue(true)]
        [Description("Abilita la conservazione della copia locale dei documenti processati.")]
        public bool LocalCopyEnabled { get; set; }

        // Thresholds
        [Category("Thresholds")]
        [DefaultValue(100)]
        [Description("Numero massimo di archiviazioni per ciclo.")]
        public int MaxIterationPerCycle { get; set; }

        [Category("Thresholds")]
        [Description("Numero di giorni per cui verrà conservata la copia locale dei documenti processati.")]
        [DefaultValue(10)]
        public int LocalCopyExpires { get; set; }

        [Category("Thresholds")]
        [DefaultValue(2)]
        [Description("Numero di giorni per cui verrà atteso il documento mancante prima di essere processato come anomalia.")]
        public int PostponeMissingExpires { get; set; }

        [Category("Thresholds")]
        [DefaultValue(".pdf.p7m")]
        [Description("Lista di estensioni consentite.")]
        public string ExtensionWhiteList { get; set; }

        [Category("Configuration")]
        [DefaultValue(false)]
        [Description("Abilita la gestione degli elementi da scartare per errata validazione firma")]
        public bool SignatureValidationEnabled { get; set; }

        [Category("Configuration")]
        [DefaultValue("https://dss.agid.gov.it/")]
        [Description("Url del servizio REST DSS di validazione firma documenti")]
        public string SignatureValidationBaseUrl { get; set; }
        #endregion

    }
}
