Imports Telerik.Web.UI

Public Class uscDossierFolders
    Inherits DocSuite2008BaseControl

#Region " Fields "

#End Region

#Region " Properties "
    Public ReadOnly Property PageContentDiv As RadPageLayout
        Get
            Return pageContent
        End Get
    End Property

    Public Property PersistanceDisabled As Boolean
    Public Property HideFascicleAssociateButton As Boolean
    Public Property HideStatusToolbar As Boolean
    Public Property IsWindowPopupEnable As Boolean
    Public Property FascicleModifyButtonEnable As Boolean = True
#End Region

#Region " Events"

    Protected Sub uscDossierFolders_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()
        btnSelectDossierFolder.Visible = IsWindowPopupEnable
    End Sub

    Protected Sub uscDossierFoldersAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)

    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf uscDossierFoldersAjaxRequest
    End Sub

#End Region

End Class