using System.IO;

namespace Vecompsoftware.FileServer.Services
{
	public static class StreamExtensions
	{
		/// <summary>
		/// Copies data from one stream to another.
		/// </summary>
		/// <param name="input">The input stream</param>
		/// <param name="output">The output stream</param>
		public static void CopyTo(this Stream input, Stream output)
		{
			const int bufferSize = 2048;
			var buffer = new byte[bufferSize];
			int bytes;

			while ((bytes = input.Read(buffer, 0, bufferSize)) > 0)
			{
				output.Write(buffer, 0, bytes);
			}
		}
	}
}