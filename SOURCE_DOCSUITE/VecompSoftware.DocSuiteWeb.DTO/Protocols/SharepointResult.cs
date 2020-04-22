using System;

namespace VecompSoftware.DocSuiteWeb.DTO.Protocols
{
    public class SharepointResult
    {
        /// <summary>
        /// Costruttore vuoto
        /// </summary>
        public SharepointResult() : base()
        { }

        public Guid FileId { get; set; }

        public string FileName { get; set; }

        public DateTime? Modified { get; set; }

        public string FilePath { get; set; }

        public string FileTipology { get; set; }


    }
}

