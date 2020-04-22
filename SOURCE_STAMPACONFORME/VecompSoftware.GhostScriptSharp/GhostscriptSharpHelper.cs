using System;
using System.IO;
using System.Linq;

namespace VecompSoftware.GhostscriptSharp
{
    public static class GhostscriptSharpHelper
    {

        #region [ Constants ]

        public const string WORKINGFOLDERNAME = @"VecompSoftware\GhostScriptSharp";

        #endregion

        #region [ Methods ]

        public static string GetWorkingFolder()
        {
            var folder = Path.Combine(Path.GetTempPath(), WORKINGFOLDERNAME);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            return folder;
        }

        private static bool IsExpired(DateTime time, int threshold)
        {
            return (DateTime.Now - time).TotalHours > Math.Abs(threshold);
        }
        private static bool IsDirectoryExpired(string path)
        {
            var creationDate = Directory.GetCreationTime(path);
            return IsExpired(creationDate, 1);
        }

        public static void PurgeWorkingFolder()
        {
            var workingFolder = GetWorkingFolder();
            var directories = Directory.GetDirectories(workingFolder);
            var expired = directories.Where(IsDirectoryExpired);
            expired.ToList().ForEach(e => Directory.Delete(e, true));
        }
        public static GhostscriptSession GetSession(string id)
        {
            return new GhostscriptSession(id);
        }
        public static GhostscriptSession GetSession()
        {
            var id = Guid.NewGuid().ToString("N");
            return GetSession(id);
        }

        #endregion

    }
}
