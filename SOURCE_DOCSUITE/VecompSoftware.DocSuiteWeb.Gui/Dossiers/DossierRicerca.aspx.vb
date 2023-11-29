Imports System.Web
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class DossierRicerca
    Inherits DossierBasePage
#Region " Properties "
    Public ReadOnly Property IsWindowPopupEnable As Boolean
        Get
            Return HttpContext.Current.Request.QueryString.GetValueOrDefault("IsWindowPopupEnable", False)
        End Get
    End Property
    Public ReadOnly Property DossierStatusEnabled As Boolean
        Get
            Return HttpContext.Current.Request.QueryString.GetValueOrDefault("DossierStatusEnabled", True)
        End Get
    End Property
#End Region

#Region " Events "
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        uscMetadataRepositorySel.SetiContactVisibilityButton = False
        dossierStatusRow.Visible = DossierStatusEnabled
    End Sub
#End Region

End Class