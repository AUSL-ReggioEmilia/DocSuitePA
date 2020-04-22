Imports Telerik.Web.UI
Imports Newtonsoft.Json
Imports System.Linq
Imports System.Collections.Generic

Partial Public Class FascAutorizza
    Inherits FascBasePage

#Region " Fields "
    Private Const FASCICLE_INSERT_CALLBACK As String = "fascAutorizza.insertCallback('{0}','{1}');"
#End Region

#Region " Properties "

#End Region

#Region " Events "
    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()

        If Not IsPostBack Then
            If Not IdFascicle = Guid.Empty Then
                uscFascicolo.CurrentFascicleId = IdFascicle
            End If
        End If
    End Sub
    Protected Sub FascAutorizzaAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Select Case e.Argument
            Case "Authorized"
                Dim added As ICollection(Of Integer) = uscFascicolo.RolesAccountedAdded
                Dim removed As ICollection(Of Integer) = uscFascicolo.RolesAccountedRemoved
                Dim roleAddedJson As String = String.Empty
                Dim roleRemovedJson As String = String.Empty
                If added IsNot Nothing AndAlso added.Any() Then
                    roleAddedJson = JsonConvert.SerializeObject(added)
                End If
                If (removed IsNot Nothing AndAlso removed.Any()) Then
                    roleRemovedJson = JsonConvert.SerializeObject(removed)
                End If
                AjaxManager.ResponseScripts.Add(String.Format(FASCICLE_INSERT_CALLBACK, roleAddedJson, roleRemovedJson))
                Exit Select
        End Select
    End Sub
#End Region

#Region " Methods "
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf FascAutorizzaAjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConfirm, uscFascicolo)
    End Sub
#End Region

End Class


