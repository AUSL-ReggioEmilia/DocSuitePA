using System;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.ReceivableInvoice.Exceptions
{
    public class AlreadyExistsInvoiceException : Exception
    {
        #region [ Fields ]
        #endregion

        #region [ Properties ]
        public string InvoiceMessage { get; }
        public string InvoiceNumber { get; }
        public string InvoiceFilename { get; }
        public string UDSRepositoryName { get; }
        public string UDSId { get; }
        public string TenantName { get; }
        #endregion

        #region [ Constructor ]
        public AlreadyExistsInvoiceException(string invoiceNumber, string invoiceFilename, string udsRepositoryName, string udsId, string tenantName)
            : base($"Invoice {invoiceFilename}/{invoiceNumber} already inserted in UDS archive {udsRepositoryName} with UDSId {udsId}")
        {
            InvoiceNumber = invoiceNumber;
            InvoiceFilename = invoiceFilename;
            UDSRepositoryName = udsRepositoryName;
            UDSId = udsId;
            TenantName = tenantName;
            InvoiceMessage = $"Fattura {invoiceFilename}/{invoiceNumber} è gia presente nell'archivio {udsRepositoryName}";
        }

        #endregion

    }
}