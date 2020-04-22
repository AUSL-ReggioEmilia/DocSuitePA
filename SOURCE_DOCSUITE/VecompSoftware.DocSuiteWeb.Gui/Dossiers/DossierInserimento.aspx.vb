Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging

Public Class DossierInserimento
    Inherits CommonBasePage

#Region " Fields "
    Private Const DOSSIER_INSERT_CALLBACK As String = "dossierInserimento.insertCallback('{0}','{1}');"
#End Region

#Region " Properties "

#End Region

#Region "Events"

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()

        If Not IsPostBack() Then
            Initialize()
        End If
    End Sub


    Protected Sub btnConfirm_OnClick(sender As Object, e As EventArgs) Handles btnInserimento.Click

        'PARTE CONTATTO
        Dim contacts As IList(Of ContactDTO) = uscContattiSel.GetContacts(False)
        Dim contactJson As String = String.Empty
        If contacts IsNot Nothing AndAlso contacts.Any() Then
            Dim contactIds As Integer() = contacts.Select(Function(x) x.Contact.Id).ToArray()
            contactJson = JsonConvert.SerializeObject(contactIds)
        End If

        'PARTE SETTORE 
        Dim roles As IList(Of Role) = uscSettori.GetRoles()
        Dim roleJson As String = String.Empty
        If roles IsNot Nothing AndAlso roles.Any() Then
            Dim roleIds As Integer() = roles.Select(Function(x) x.Id).ToArray()
            roleJson = JsonConvert.SerializeObject(roleIds)
        End If
        AjaxManager.ResponseScripts.Add(String.Format(DOSSIER_INSERT_CALLBACK, contactJson, roleJson))

    End Sub

#End Region

#Region " Methods "
    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(uscContattiSel, uscContattiSel)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscSettori, uscSettori)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnInserimento, btnInserimento)
    End Sub

    Private Sub Initialize()
        uscSettori.RoleRestictions = RoleRestrictions.OnlyMine
        uscSettori.Environment = DSWEnvironment.Document
    End Sub

#End Region

End Class