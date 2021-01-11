Imports System.Web
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class DossierRisultati
    Inherits CommonBasePage
#Region " Properties "
    Public ReadOnly Property IsWindowPopupEnable As Boolean
        Get
            Return HttpContext.Current.Request.QueryString.GetValueOrDefault("IsWindowPopupEnable", False)
        End Get
    End Property
#End Region

#Region " Events "
    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            uscDossierGrid.IsWindowPopupEnable = IsWindowPopupEnable
        End If
    End Sub
#End Region

End Class