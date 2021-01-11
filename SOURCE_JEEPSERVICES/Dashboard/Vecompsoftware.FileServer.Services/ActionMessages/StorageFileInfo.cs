using System;

namespace Vecompsoftware.FileServer.Services.ActionMessages
{
	[Serializable]
	public class StorageFileInfo
	{
        public enum Type
        {
            Directory = 0,
            File = 1
        }

        /// <summary>
        /// Nome del file
        /// </summary>
        public string Name { get; set; }

		/// <summary>
		/// Gets or sets the virtual path to the file
		/// </summary>
		public string VirtualPath { get; set; }

		/// <summary>
		/// Gets or sets the size of the file (in bytes)
		/// </summary>
		public long Size { get; set; }

        /// <summary>
        /// Definisce se il percorso identifica una cartella oppure un file
        /// </summary>
        public Type StorageFileType { get; set; }
	}
}
