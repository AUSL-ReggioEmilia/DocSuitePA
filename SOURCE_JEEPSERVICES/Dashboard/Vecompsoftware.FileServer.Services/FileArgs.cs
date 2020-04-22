using System;

namespace Vecompsoftware.FileServer.Services
{
	public delegate void FileEventHandler(object sender, FileEventArgs e);
    public delegate void FileWatcherEventHandler(object sender, FileWatcherEventArgs e);

	public class FileEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the virtual path.
		/// </summary>
		public string VirtualPath
		{
			get { return _virtualPath; }
		}

	    readonly string _virtualPath;

		/// <summary>
		/// Initializes a new instance of the <see cref="FileEventArgs"/> class.
		/// </summary>
		/// <param name="vPath">The v path.</param>
		public FileEventArgs(string vPath)
		{
			_virtualPath = vPath;
		}
	}

    public class FileWatcherEventArgs : FileEventArgs
    {
        public bool EnableRaisingEvents
        {
            get { return _enableRaisingEvents; }
        }

        public string Filter
        {
            get { return _filter; }
        }

        private readonly bool _enableRaisingEvents;
        private readonly string _filter;

        public FileWatcherEventArgs(bool enableRaisingEvents, string path, string filter) : base(path)
        {
            _enableRaisingEvents = enableRaisingEvents;
            _filter = filter;
        }

        public FileWatcherEventArgs(bool enableRaisingEvents) : base(String.Empty)
        {
            _enableRaisingEvents = enableRaisingEvents;
            _filter = String.Empty;
        }
    }
}
