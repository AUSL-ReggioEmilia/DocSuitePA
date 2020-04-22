using System;
using System.IO;

namespace VecompSoftware.Common
{
    public class TempSession : IDisposable
    {

        public TempSession()
        {
            Id = Guid.NewGuid().ToString("N");
            TempPath = Path.Combine(Path.GetTempPath(), Id);

            Directory.CreateDirectory(TempPath);
        }

        #region [ Properties ]

        public string Id { get; private set; }
        public string TempPath { get; private set; }
        
        #endregion

        #region [ Methods ]

        public void Dispose()
        {
            if (Directory.Exists(TempPath))
                Directory.Delete(TempPath, true);
        }

        public string Write(IContent contentInfo)
        {
            var tempFileName = Path.Combine(TempPath, contentInfo.FileName.ToPath().GetUniqueFileName());
            File.WriteAllBytes(tempFileName, contentInfo.Content);
            return tempFileName;
        }
        public string Write(string path, byte[] content)
        {
            var contentInfo = new ContentInfo(path, content);
            return Write(contentInfo);
        }
        public string Write(string path)
        {
            var contentInfo = new ContentInfo(path);
            return Write(contentInfo);
        }

        #endregion

    }
}
