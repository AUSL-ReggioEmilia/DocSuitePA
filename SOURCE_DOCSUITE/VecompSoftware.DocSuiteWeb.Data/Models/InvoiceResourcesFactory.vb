Imports VecompSoftware.DocSuiteWeb.Model.DocumentGenerator

Public Class InvoiceResourcesFactory
    Private ReadOnly _invoicePAStyles As IDictionary(Of String, String) = New Dictionary(Of String, String) From
    {
        {InvoiceStylesheetNames.ASSO_SOFTWARE, My.Resources.XSL_AssoSoftware_1_2},
        {InvoiceStylesheetNames.AGID, My.Resources.XSL_AGID_1_2}
    }

    Private ReadOnly _invoiceSMStyles As IDictionary(Of String, String) = New Dictionary(Of String, String) From
    {
        {InvoiceStylesheetNames.ASSO_SOFTWARE, My.Resources.XSL_AssoSoftware_1_2}
    }

    Public Function CreateInvoicePA10() As InvoiceResources
        Return New InvoiceResources() With
        {
            .InvoiceKind = XMLModelKind.InvoicePA_V10,
            .Stylesheets = _invoicePAStyles
        }
    End Function

    Public Function CreateInvoicePA11() As InvoiceResources
        Return New InvoiceResources() With
        {
            .InvoiceKind = XMLModelKind.InvoicePA_V11,
            .Stylesheets = _invoicePAStyles
        }
    End Function

    Public Function CreateInvoicePA12() As InvoiceResources
        Return New InvoiceResources() With
        {
            .InvoiceKind = XMLModelKind.InvoicePA_V12,
            .Stylesheets = _invoicePAStyles
        }
    End Function

    Public Function CreateInvoicePR12() As InvoiceResources
        Return New InvoiceResources() With
        {
            .InvoiceKind = XMLModelKind.InvoicePR_V12,
            .Stylesheets = _invoicePAStyles
        }
    End Function
End Class
