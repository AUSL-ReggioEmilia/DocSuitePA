Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.DocSuiteWeb.Facade

Public Class UDSDuplica
    Inherits UDSBasePage

#Region "Events"
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        MasterDocSuite.TitleVisible = False
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, pnlButton, MasterDocSuite.AjaxDefaultLoadingPanel)
        If Not Me.IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub btnConferma_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConferma.Click
        Dim s As String = "0000000000"
        SetCheck(s, CShort(UDSCopyOption.Object), cbOggetto.Checked)
        If pnlContacts.Visible Then
            SetCheck(s, CShort(UDSCopyOption.Contacts), cbContacts.Checked)
        End If
        SetCheck(s, CShort(UDSCopyOption.Category), cbClassificazione.Checked)
        SetCheck(s, CShort(UDSCopyOption.Metadata), cbMetadata.Checked)
        If pnlRoles.Visible Then
            SetCheck(s, CShort(UDSCopyOption.Roles), cbRoles.Checked)
        End If
        Me.MasterDocSuite.AjaxManager.ResponseScripts.Add(String.Concat("CloseWindow('", s, "');"))

    End Sub
#End Region

#Region "Methods"
    Private Sub Initialize()
        Dim uds As UDSDto = GetSource()
        pnlContacts.Visible = False
        pnlRoles.Visible = False
        metadatapnl.Visible = True
        If uds.UDSModel.Model.Contacts IsNot Nothing AndAlso uds.UDSModel.Model.Contacts.Any() AndAlso uds.Contacts.Count > 0 Then
            pnlContacts.Visible = True
        End If

        If uds.UDSModel.Model.Authorizations IsNot Nothing AndAlso uds.UDSModel.Model.Authorizations.Instances IsNot Nothing AndAlso uds.UDSModel.Model.Authorizations.Instances.Length > 0 AndAlso uds.Authorizations.Count > 0 Then
            pnlRoles.Visible = True
        End If
    End Sub


    Private Sub SetCheck(ByRef field As String, ByVal right As Short, ByVal value As Boolean)
        If value Then
            Mid$(field, right, 1) = "1"
        Else
            Mid$(field, right, 1) = "0"
        End If
    End Sub
#End Region
End Class