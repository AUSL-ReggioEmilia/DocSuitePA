Public Class UDSInvoiceDelete
    Inherits UDSBasePage

#Region " Fields "

#End Region

#Region " Properties "
    Public ReadOnly Property CurrentUserTenantName As String
        Get
            If CurrentTenant IsNot Nothing Then
                Return CurrentTenant.TenantName
            End If
            Return String.Empty
        End Get
    End Property

    Public ReadOnly Property CurrentUserTenantId As Guid
        Get
            If CurrentTenant IsNot Nothing Then
                Return CurrentTenant.UniqueId
            End If
            Return Guid.Empty
        End Get
    End Property

    Public ReadOnly Property CurrentUserTenantAOOId As Guid
        Get
            If CurrentTenant IsNot Nothing Then
                Return CurrentTenant.TenantAOO.UniqueId
            End If
            Return Guid.Empty
        End Get
    End Property
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