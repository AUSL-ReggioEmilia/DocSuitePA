Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class UserRoles
    Inherits UserBasePage

#Region " Properties "

    Private ReadOnly Property Account() As String
        Get
            Return Request.QueryString("account")
        End Get
    End Property

    Private ReadOnly Property Environment() As CollaborationDocumentType?
        Get
            If Not String.IsNullOrEmpty(Request.QueryString("Environment")) Then
                Return DirectCast([Enum].Parse(GetType(CollaborationDocumentType), Request.QueryString("Environment")), CollaborationDocumentType)
            End If
            Return Nothing
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False
        
        'configuro chiamate Ajax
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlRoles, uscUserRoles, MasterDocSuite.AjaxDefaultLoadingPanel)

        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub btnConfirm_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConfirm.Click
        If (uscUserRoles.UpdateCheckedRoles()) Then
            AjaxManager.ResponseScripts.Add("Close();")
        Else
            AjaxAlert("Errore in Aggiornamento Dati")
        End If
    End Sub

    Private Sub ddlRoles_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlRoles.SelectedIndexChanged
        LoadUserRoles()
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        ' Carico i settori dove l'utente è direttore
        Dim roles As IList(Of RoleUser) = New List(Of RoleUser)()
        If Not String.IsNullOrEmpty(Account) Then
            roles = Facade.RoleUserFacade.GetByUserType(RoleUserType.D, DocSuiteContext.Current.User.FullUserName, False, Nothing)
        End If
        If roles.IsNullOrEmpty() Then
            AjaxAlert("Nessun Settore disponibile.")
            AjaxManager.ResponseScripts.Add("Close();")
            Exit Sub
        End If
        ddlRoles.Enabled = roles.Count > 1

        For Each roleUser As RoleUser In roles
            If ddlRoles.Items.FindByValue(roleUser.Role.Id.ToString()) Is Nothing Then
                ddlRoles.Items.Add(New ListItem(roleUser.Role.Name, roleUser.Role.Id.ToString()))
            End If
        Next

        uscUserRoles.EditMode = False
        LoadUserRoles()
    End Sub

    Private Sub LoadUserRoles()
        uscUserRoles.RoleId = Integer.Parse(ddlRoles.SelectedValue)
        uscUserRoles.FromCollaboration = True
        uscUserRoles.Environment = Environment
        uscUserRoles.LoadUserRoles(True)
    End Sub


#End Region

End Class