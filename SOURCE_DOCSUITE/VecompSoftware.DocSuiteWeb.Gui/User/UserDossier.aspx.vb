Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Dossiers
Imports VecompSoftware.DocSuiteWeb.Entity.Dossiers
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class UserDossier
    Inherits UserBasePage

#Region " Fields "
    Dim _actionType As String = String.Empty

#End Region

#Region " Properties "

    Protected ReadOnly Property ActionType As String
        Get
            If String.IsNullOrEmpty(_actionType) Then
                _actionType = Me.Request.QueryString.GetValueOrDefault("Action", String.Empty)
            End If
            Return _actionType
        End Get
    End Property

#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Title = Request.QueryString("Title")
    End Sub

End Class