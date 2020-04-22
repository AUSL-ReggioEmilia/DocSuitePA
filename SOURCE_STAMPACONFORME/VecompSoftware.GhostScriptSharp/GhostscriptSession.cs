using System;
using System.IO;
using GhostscriptSharp;
using VecompSoftware.Commons.BiblosInterfaces;

namespace VecompSoftware.GhostscriptSharp
{
    public class GhostscriptSession : IDisposable
    {

        #region [ Constructors ]

        public GhostscriptSession(string id)
        {
            Id = id;
            Folder = Path.Combine(GhostscriptSharpHelper.GetWorkingFolder(), id);

            var pattern = string.Format("{0}_{1}", Id, PAGEPLACEHOLDER);
            OutputPattern = Path.Combine(Folder, pattern);
        }

        #endregion

        #region [ Constants ]

        private const string SESSIONSEARCHPATTERN = "{0}*.*";
        private const string PAGEPLACEHOLDER = "%03d";

        #endregion

        #region [ Properties ]

        public string Id { get; private set; }
        public string Folder { get; private set; }
        public string OutputPattern { get; private set; }
        public bool HasEnvironment
        {
            get { return Directory.Exists(Folder); }
        }

        #endregion

        #region [ Methods ]

        public void Dispose()
        {
            if (HasEnvironment)
                Directory.Delete(Folder, true);
        }

        public string[] GetFiles()
        {
            if (!HasEnvironment)
                return null;

            var searchPattern = string.Format(SESSIONSEARCHPATTERN, Id);
            return Directory.GetFiles(Folder, searchPattern);
        }

        public GhostscriptSession CreateEnvironment()
        {
            if (!HasEnvironment)
                Directory.CreateDirectory(Folder);
            return this;
        }

        public GhostscriptSession GenerateOutput(IPdfContent pdfInfo)
        {
            CreateEnvironment();

            var inputFilename = Id + ".pdf";
            var inputPath = Path.Combine(Folder, inputFilename);
            pdfInfo.WriteAllBytes(inputPath);

            var settings = GhostscriptSettingsFactory.Default();
            GhostscriptWrapper.GenerateOutput(inputPath, OutputPattern + settings.GetDeviceExtension(), settings);
            return this;
        }

        public GhostscriptSession GenerateForTesseract(string inputPath)
        {
            CreateEnvironment();

            var settings = GhostscriptSettingsFactory.Tesseract();
            GhostscriptWrapper.GenerateOutput(inputPath, OutputPattern + settings.GetDeviceExtension(), settings);
            return this;
        }
        public GhostscriptSession GenerateForTesseract(byte[] pdfContent)
        {
            var inputFilename = Id + ".pdf";
            var inputPath = Path.Combine(Folder, inputFilename);
            File.WriteAllBytes(inputPath, pdfContent);
            return GenerateForTesseract(inputPath);
        }

        public GhostscriptSession GenerateForThumbnail(string inputPath)
        {
            if (!HasEnvironment)
                CreateEnvironment();
            var settings = GhostscriptSettingsFactory.Thumbnail();
            GhostscriptWrapper.GenerateOutput(inputPath, OutputPattern + settings.GetDeviceExtension(), settings);
            return this;
        }
        public GhostscriptSession GenerateForThumbnail(byte[] pdfContent)
        {
            var inputFilename = Id + ".pdf";
            var inputPath = Path.Combine(Folder, inputFilename);
            File.WriteAllBytes(inputPath, pdfContent);
            return GenerateForThumbnail(inputPath);
        }

        #endregion

    }
}
