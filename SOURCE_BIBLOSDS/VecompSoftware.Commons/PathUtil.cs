using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VecompSoftware.Common
{
    public class PathUtil
    {

        #region [ Constructors ]

        public PathUtil(string source)
        {
            Input = source;
        }

        #endregion

        #region [ Constants ]

        public const string UNDEFINEDFILENAME = "undefined";

        public const string EXTENSIONPDF = ".pdf";
        public const string EXTENSIONMSG = ".msg";
        public const string EXTENSIONP7M = ".p7m";

        #endregion

        #region [ Properties ]

        public string Input { get; private set; }

        #endregion

        #region [ Methods ]

        public string GetFileName()
        {
            if (Input.IsNullOrWhiteSpace())
                return UNDEFINEDFILENAME;

            var fileName = Path.GetFileNameWithoutExtension(Input);
            var extension = Path.GetExtension(Input);
            if (fileName.IsNullOrWhiteSpace())
                return Path.ChangeExtension(UNDEFINEDFILENAME, extension);

            return Input;
        }
        public string GetFileNameWithoutExtension()
        {
            var fileName = GetFileName();
            return Path.GetFileNameWithoutExtension(fileName);
        }
        public string GetExtension()
        {
            var fileName = GetFileName();
            return Path.GetExtension(fileName).ToLowerInvariant();
        }
        public bool HasExtension()
        {
            return !Path.GetExtension(Input).IsNullOrWhiteSpace();
        }

        public string ChangeExtension(string extension)
        {
            var fileName = GetFileName();
            return Path.ChangeExtension(fileName, extension.ToLowerInvariant());
        }
        public string GetUniqueFileName()
        {
            var fileName = GetFileNameWithoutExtension();
            var extension = GetExtension();
            return string.Format("{0}_{1}{2}", fileName, Guid.NewGuid().ToString("N"), extension);
        }
        public string GetFileNameResolveChain()
        {
            var splitted = GetFileName().SplitNoEmpty('.');
            var extension = GetExtension().Replace(".", string.Empty);
            var result = splitted.Reverse().SkipWhile(e => e.EqualsIgnoreCase(extension));
            return string.Join(".", result.Reverse());
        }
        public bool IsUndefinedFileName()
        {
            return GetFileName().Equals(UNDEFINEDFILENAME);
        }

        #endregion

    }
}
