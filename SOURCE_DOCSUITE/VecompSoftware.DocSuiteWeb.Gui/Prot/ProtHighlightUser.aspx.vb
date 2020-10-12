Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods
Imports Telerik.Web.UI
Imports System.Linq

Partial Public Class ProtHighlightUser
    Inherits ProtBasePage

#Region " Fields "
    Private Const CLOSE_WINDOW As String = "CloseWindow('{0}');"
    Private Const HIGHLIGHTUSER_WINDOW_COMMAND As String = "WindowCommandEvent"
#End Region

#Region " Properties "
    Public ReadOnly Property NoteTextBox As RadTextBox
        Get
            Return rtbProtocolNotes
        End Get
    End Property

#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If ProtocolEnv.ProtocolHighlightEnabled Then
            note.Style.Add("display", "table-row")
        End If

        If Not IsPostBack() Then
            MasterDocSuite.TitleVisible = False
            Initialize()
        End If
    End Sub

    Private Sub Initialize()
        If ProtocolEnv.ProtocolHighlightEnabled AndAlso ProtocolEnv.HighlightProtocolGroups.IsActive Then
            Dim groups As Dictionary(Of String, String) = ProtocolEnv.HighlightProtocolGroups.GroupList
            uscUserSearch.ADRestrictionGroups = groups.Select(Function(s) s.Value).ToList()
        End If

        Dim alreadyHighlightedToMe As Boolean = CurrentProtocolRights.HasHighlightRights
        btnRemoveHighlight.Visible = alreadyHighlightedToMe
        btnHighlightToMe.Visible = Not alreadyHighlightedToMe
        btnConfirmAndRemoveHighlight.Visible = alreadyHighlightedToMe
    End Sub
    Protected Sub uscSeach_NodeClicked(ByVal sender As Object, ByVal e As EventArgs) Handles uscUserSearch.NodeClicked
        Dim selectedUser As String = uscUserSearch.selectedNode.Attributes("DisplayName")
        Dim protocolUser As ProtocolUser = CurrentProtocol.Users.FirstOrDefault(Function(x) x.Account.Eq(selectedUser))
        If protocolUser IsNot Nothing Then
            AjaxManager.ResponseScripts.Add($"populateNote('{protocolUser.Note}',{If(protocolUser.RegistrationUser.Eq(DocSuiteContext.Current.User.FullUserName), "false", "true")});")
        Else
            AjaxManager.ResponseScripts.Add("populateNote('',false);")
        End If
    End Sub
    Private Sub btnConfirmAndRemoveHighlight_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConfirmAndRemoveHighlight.Click
        AddSelectedContactAsProtocolHighlight(removeCurrentUserHighlight:=True)
    End Sub

    Private Sub btnConfirm_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConfirm.Click
        AddSelectedContactAsProtocolHighlight()
    End Sub

    Private Sub btnHighlightToMe_Click(sender As Object, e As EventArgs) Handles btnHighlightToMe.Click
        Facade.ProtocolUserFacade.SetHighlightUser(CurrentProtocol, DocSuiteContext.Current.User.FullUserName, String.Empty)

        AjaxManager.ResponseScripts.Add(String.Format(CLOSE_WINDOW, HIGHLIGHTUSER_WINDOW_COMMAND))
    End Sub

    Private Sub btnRemoveHighlight_Click(sender As Object, e As EventArgs) Handles btnRemoveHighlight.Click
        Facade.ProtocolUserFacade.RemoveHighlightUser(CurrentProtocol, DocSuiteContext.Current.User.FullUserName)

        AjaxManager.ResponseScripts.Add(String.Format(CLOSE_WINDOW, HIGHLIGHTUSER_WINDOW_COMMAND))
    End Sub

    Private Sub AddSelectedContactAsProtocolHighlight(Optional removeCurrentUserHighlight As Boolean = False)
        If uscUserSearch.TreeViewControl.SelectedNode Is Nothing OrElse (uscUserSearch.TreeViewControl.SelectedNode.Index = 0 AndAlso uscUserSearch.TreeViewControl.SelectedNode.Text.Eq("Contatti")) Then
            AjaxAlert("Selezionare un contatto valido.")
            Return
        End If

        Dim selectedContactDisplayName As String = uscUserSearch.TreeViewControl.SelectedNode.Attributes.Item("DisplayName")
        Dim selectedContactAlreadyHighlighted As Boolean = CurrentProtocol.Users.Any(Function(x) x.Account.Eq(selectedContactDisplayName) AndAlso x.Type = ProtocolUserType.Highlight)

        If selectedContactAlreadyHighlighted Then
            AjaxAlert("Il contatto selezionato è già evidenziato.")
            Return
        End If

        If removeCurrentUserHighlight Then
            Facade.ProtocolUserFacade.RemoveHighlightUser(CurrentProtocol, DocSuiteContext.Current.User.FullUserName)
        End If

        Facade.ProtocolUserFacade.SetHighlightUser(CurrentProtocol, selectedContactDisplayName, NoteTextBox.Text)

        AjaxManager.ResponseScripts.Add(String.Format(CLOSE_WINDOW, HIGHLIGHTUSER_WINDOW_COMMAND))
    End Sub

#End Region


End Class