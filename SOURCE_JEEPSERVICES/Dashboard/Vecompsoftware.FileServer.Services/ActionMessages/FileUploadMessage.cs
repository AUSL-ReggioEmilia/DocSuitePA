using System.ServiceModel;
using System.IO;

namespace Vecompsoftware.FileServer.Services.ActionMessages
{
	[MessageContract]
	public class FileUploadMessage
	{
		[MessageHeader(MustUnderstand=true)]
		public string VirtualPath { get; set; }

		[MessageBodyMember(Order=1)]
		public Stream DataStream { get; set; }
	}
}
