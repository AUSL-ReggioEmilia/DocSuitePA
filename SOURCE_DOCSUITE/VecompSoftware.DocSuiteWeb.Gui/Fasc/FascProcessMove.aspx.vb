Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class FascMove
    Inherits CommBasePage

#Region "Properties"
    Protected ReadOnly Property FascicleId As String
        Get
            Dim _fascicleId As String = Request.QueryString.GetValueOrDefault("FascicleId", String.Empty)
            Return _fascicleId
        End Get
    End Property
    Protected ReadOnly Property ParentFolderId As String
        Get
            Dim _fascicleParentId As String = Request.QueryString.GetValueOrDefault("ParentFolderId", String.Empty)
            Return _fascicleParentId
        End Get
    End Property
#End Region
#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False
        If Not Page.IsPostBack AndAlso Not Page.IsCallback Then
            If Not CommonShared.HasGroupAdministratorRight AndAlso Not CommonShared.HasGroupProcessesViewsManageableRight Then
                Throw New DocSuiteException("Sposta fascicolo", "Diritti di visualizzazione mancanti.")
            End If
        End If
    End Sub
#End Region

End Class