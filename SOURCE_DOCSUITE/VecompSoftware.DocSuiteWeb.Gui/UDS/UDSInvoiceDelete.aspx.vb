Public Class UDSInvoiceDelete
    Inherits UDSBasePage

#Region " Fields "

#End Region
#Region " Properties "

#End Region
#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
#End Region

#Region " Methods "
    Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlContentInvoice, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
    End Sub
#End Region
End Class