using System;
using System.IO;
using System.Security.Cryptography;

namespace VecompSoftware.Common
{
    public class ContentInfo : IContent
    {

        public ContentInfo(string fullName, byte[] content)
        {
            FullName = fullName;
            _content = content;
        }
        public ContentInfo(byte[] content) : this(null, content) { }
        public ContentInfo(string fullName, bool eager)
        {
            FullName = fullName;
            if (eager)
                ReadContent();
        }
        public ContentInfo(string fullName) : this(fullName, false) { }

        public ContentInfo(IContent content) : this(content.FullName, content.Content) { }


        #region [ Fields ]

        private byte[] _content;
        private string _fileName;
        private string _extension;

        #endregion

        #region [ Properties ]

        public string FullName { get; private set; }
        public byte[] Content
        {
            get
            {
                if (_content.IsNullOrEmpty())
                    ReadContent();
                return _content;
            }
        }

        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(_fileName))
                    _fileName = FullName.ToPath().GetFileName();
                return _fileName;
            }
        }
        public string Extension
        {
            get
            {
                if (string.IsNullOrEmpty(_extension))
                    _extension = FileName.ToPath().GetExtension();
                return _extension;
            }
        }
        public bool IsDirty
        {
            get { return Exists() && _content.IsNullOrEmpty(); }
        }

        #endregion

        #region [ Methods ]

        private void ReadContent()
        {
            if (_content.IsNullOrEmpty())
                _content = File.ReadAllBytes(FullName);
        }

        public bool Exists()
        {
            return File.Exists(FullName);
        }

        public void WriteAllBytes(string destination)
        {
            if (IsDirty)
            {
                File.Copy(FullName, destination);
                return;
            }

            File.WriteAllBytes(destination, Content);
        }

        #endregion

    }
}
