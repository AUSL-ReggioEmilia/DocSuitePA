namespace VecompSoftware.DocSuiteWeb.Entity.PECMails
{
    public enum InvoiceStatus : short
    {
        None = 0,
        PAInvoiceSent = 1,
        PAInvoiceNotified = 2,
        PAInvoiceAccepted = 3,
        PAInvoiceSdiRefused = 4,
        PAInvoiceRefused = 5,
        InvoiceWorkflowStarted = 6,
        InvoiceWorkflowCompleted = 7,
        InvoiceWorkflowError = 8,
        InvoiceSignRequired = 9,
        InvoiceLookingMetadata = 10,
        InvoiceAccounted = 11
    }
}
