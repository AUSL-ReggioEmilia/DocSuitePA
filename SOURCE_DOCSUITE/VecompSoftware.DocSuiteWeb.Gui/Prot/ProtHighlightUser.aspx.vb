Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods
Imports Telerik.Web.UI
Imports System.Linq
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Partial Public Class ProtHighlightUser
    Inherits CommonBasePage

#Region " Fields "
    Dim _currentProtocol As Protocol
#End Region

#Region " Properties "
    Public ReadOnly Property NoteTextBox As RadTextBox
        Get
            Return rtbProtocolNotes
        End Get
    End Property

    Public ReadOnly Property UniqueIdProtocol As Guid
        Get
            Return Request.QueryString.GetValueOrDefault(Of Guid)("UniqueIdProtocol", Nothing)
        End Get
    End Property

    Public ReadOnly Property CurrentProtocol As Protocol
        Get
            If _currentProtocol Is Nothing Then
                _currentProtocol = Facade.ProtocolFacade.GetByUniqueId(UniqueIdProtocol)
            End If
            Return _currentProtocol
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

        Dim alreadyHighlightedToMe As Boolean = CurrentProtocol.Users.Any(Function(x) x.Account.Eq(DocSuiteContext.Current.User.FullUserName) AndAlso x.Type = ProtocolUserType.Highlight)
        btnRemoveHighlight.Visible = alreadyHighlightedToMe
        btnHighlightToMe.Visible = Not alreadyHighlightedToMe
    End Sub
    Protected Sub uscSeach_NodeClicked(ByVal sender As Object, ByVal e As EventArgs) Handles uscUserSearch.NodeClicked
        Dim protocolUser As ProtocolUser = CurrentProtocol.Users.FirstOrDefault(Function(x) x.Account.ToLower() = uscUserSearch.selectedNode.Text.Split(" - ")(0).ToLower())
        If protocolUser IsNot Nothing Then
            AjaxManager.ResponseScripts.Add("populateNote('" + protocolUser.Note + "');")
        Else
            AjaxManager.ResponseScripts.Add("populateNote('');")
        End If
    End Sub
    Private Sub btnConfirm_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConfirm.Click
        If uscUserSearch.TreeViewControl.SelectedNode Is Nothing OrElse (uscUserSearch.TreeViewControl.SelectedNode.Index = 0 AndAlso uscUserSearch.TreeViewControl.SelectedNode.Text.Eq("Contatti")) Then
            AjaxAlert("Selezionare un contatto valido.")
            Return
        End If

        Facade.ProtocolUserFacade.SetHighlightUser(CurrentProtocol, uscUserSearch.TreeViewControl.SelectedNode.Attributes.Item("DisplayName"), NoteTextBox.Text)

        AjaxManager.ResponseScripts.Add("CloseWindow();")
    End Sub


    Private Sub btnHighlightToMe_Click(sender As Object, e As EventArgs) Handles btnHighlightToMe.Click
        Facade.ProtocolUserFacade.SetHighlightUser(CurrentProtocol, DocSuiteContext.Current.User.FullUserName, String.Empty)
        AjaxManager.ResponseScripts.Add("CloseWindow();")
    End Sub

    Private Sub btnRemoveHighlight_Click(sender As Object, e As EventArgs) Handles btnRemoveHighlight.Click
        Facade.ProtocolUserFacade.RemoveHighlightUser(CurrentProtocol, DocSuiteContext.Current.User.FullUserName)
        AjaxManager.ResponseScripts.Add("CloseWindow();")
    End Sub

#End Region


End Class