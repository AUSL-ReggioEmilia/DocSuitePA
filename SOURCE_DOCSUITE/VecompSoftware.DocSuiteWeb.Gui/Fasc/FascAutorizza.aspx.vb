Imports Telerik.Web.UI
Imports Newtonsoft.Json
Imports System.Linq
Imports System.Collections.Generic

Partial Public Class FascAutorizza
    Inherits FascBasePage

#Region " Fields "
#End Region

#Region " Properties "

#End Region

#Region " Events "
    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()

        If Not IsPostBack Then
            uscFascicolo.UscDocumentReference.Visible = False
            If Not IdFascicle = Guid.Empty Then
                uscFascicolo.CurrentFascicleId = IdFascicle
            End If
        End If
    End Sub
#End Region

#Region " Methods "
    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConfirm, uscFascicolo)
    End Sub
#End Region

End Class


