using System;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.ReceivableInvoice.Models
{
    public class ZipEntryProcessSummary
    {
        public string ZipMetadataName { get; set; }
        /// <summary>
        /// The metadata full name
        /// </summary>
        public string InvoiceFileName { get; set; }

        /// <summary>
        /// If an exception was captured
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// If it was completed succesfully
        /// </summary>
        public bool Processed { get; set; } = false;
    }
}
