using VecompSoftware.BPM.Integrations.Modules.SM.ACATA.DocumentService;

namespace VecompSoftware.BPM.Integrations.Modules.SM.ACATA.Models
{
    public class ContentInfo
    {
        /// <summary>
        /// The name of the byblos document
        /// </summary>
        public string DocumentName { get; set; }

        /// <summary>
        /// The extension of the byblos document
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// The content as byte array of the byblos document
        /// </summary>
        public byte[] Content { get; set; }

        /// <summary>
        /// The final media type
        /// </summary>
        public enumMimeTypeType ActaMediaType { get; set; }
    }
}
