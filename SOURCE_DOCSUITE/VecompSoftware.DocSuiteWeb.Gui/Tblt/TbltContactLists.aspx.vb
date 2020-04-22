Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Commons
Imports VecompSoftware.Services.Logging

Partial Public Class TbltContactLists
    Inherits CommonBasePage

    Public Event ContactAdded(ByVal sender As Object, ByVal e As EventArgs)
    Public Event RefreshContact(ByVal sender As Object, ByVal e As EventArgs)

#Region "Fields"

    Private _currentContactListFacase As ContactListFacade
    Private Const SEARCH_OPTION As String = "btnListSearch"
    Private Const CREATE_OPTION As String = "create"
    Private Const MODIFY_OPTION As String = "modify"
    Private Const DELETE_OPTION As String = "delete"

#End Region

#Region "Properties"
    Private ReadOnly Property CurrentContactListFacade As ContactListFacade
        Get
            If _currentContactListFacase Is Nothing Then
                _currentContactListFacase = New ContactListFacade()
            End If
            Return _currentContactListFacase
        End Get
    End Property
#End Region

#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()
        If Not Page.IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub TbltContactLists_AjaxRequest(sender As Object, e As AjaxRequestEventArgs)
        Dim arguments As String() = e.Argument.Split("|"c)
        Select Case arguments(0)
            Case "LoadContactLists"
                pnlDetails.Visible = False
                LoadContactLists(SearchListTextBox.Text)
            Case "LoadContacts"
                Try
                    If arguments.Length > 1 Then
                        Dim contactIds As String() = arguments(1).Split(","c)
                        Dim contactList As ContactList = CurrentContactListFacade.GetById(Guid.Parse(RadTreeViewLists.SelectedValue))

                        For Each id As Integer In contactIds
                            Dim contactToAdd As Contact = Facade.ContactFacade.GetById(id)
                            contactList.Contacts.Add(contactToAdd)
                        Next
                        CurrentContactListFacade.Update(contactList)
                    End If
                    LoadContacts()
                Catch ex As Exception
                    FileLogger.Error(LoggerName, String.Format("Errore in aggiunta di contatto in lista di contatti: {0}", ex))
                    AjaxAlert("Si è verificato un errore durante l'aggiunta di un contatto in una lista di contatti.")
                    Return
                End Try

        End Select
    End Sub


    Protected Sub RadTreeViewLists_NodeClick(sender As Object, e As RadTreeNodeEventArgs) Handles RadTreeViewLists.NodeClick
        LoadContacts()
    End Sub

    Public ReadOnly Property SearchListTextBox As RadTextBox
        Get
            Dim toolBarItem As RadToolBarItem = ToolBarSearch.FindItemByValue("searchList")
            Return DirectCast(toolBarItem.FindControl("txtListSearch"), RadTextBox)
        End Get
    End Property

    Protected Sub cmdSearch_Click(sender As Object, e As EventArgs) Handles ToolBarSearch.ButtonClick
        Dim filter As String = String.Empty
        If Not String.IsNullOrEmpty(SearchListTextBox.Text) Then
            filter = SearchListTextBox.Text
        End If
        pnlDetails.Visible = False
        LoadContactLists(filter)
    End Sub

    Protected Sub btnDeleteContact_Click(sender As Object, e As EventArgs) Handles btnDeleteContact.Click
        Try
            Dim contactId As Integer = Convert.ToInt32(uscContacts.TreeViewControl.SelectedValue)
            Dim contactToRemove As Contact = Facade.ContactFacade.GetById(contactId)
            Dim contactList As ContactList = CurrentContactListFacade.GetById(Guid.Parse(RadTreeViewLists.SelectedValue))
            contactList.Contacts.Remove(contactToRemove)
            CurrentContactListFacade.Update(contactList)
            LoadContacts()
        Catch ex As Exception
            FileLogger.Error(LoggerName, String.Format("Errore in cancellazione di contatto da lista di contatti: {0}", ex))
            AjaxAlert("Si è verificato un errore durante la cancellazione di un contatto da una lista di contatti.")
            Return
        End Try

    End Sub

    Protected Sub uscContacts_ContactAdded(ByVal sender As Object, ByVal e As EventArgs) Handles uscContacts.ContactAdded
        RaiseEvent ContactAdded(sender, e)
    End Sub

    Private Sub uscContacts_ShowList(ByVal sender As Object, ByVal e As EventArgs) Handles uscContacts.ShowContactList
        RaiseEvent RefreshContact(uscContacts, e)
    End Sub

    Protected Sub FolderToolBar_ButtonClick(sender As Object, e As RadToolBarEventArgs) Handles FolderToolBar.ButtonClick
        Select Case e.Item.Value
            Case CREATE_OPTION
                AjaxManager.ResponseScripts.Add("OpenEditWindow('windowAddList', 'Add');")
            Case DELETE_OPTION
                AjaxManager.ResponseScripts.Add("OpenEditWindow('windowDeleteList', 'Delete');")
            Case MODIFY_OPTION
                AjaxManager.ResponseScripts.Add("OpenEditWindow('windowModifyList', 'Edit');")
        End Select
    End Sub
#End Region

#Region "Methods"

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf TbltContactLists_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, RadTreeViewLists, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlDetails, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ToolBarSearch.FindItemByValue(SEARCH_OPTION), RadTreeViewLists, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(RadTreeViewLists, pnlDetails, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnDeleteContact, pnlDetails, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ToolBarSearch.FindItemByValue(SEARCH_OPTION), pnlDetails, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub Initialize()
        LoadContactLists(String.Empty)
    End Sub

    Public Sub LoadContactLists(name As String)
        Dim contactLists As IList(Of ContactList) = New List(Of ContactList)

        If String.IsNullOrEmpty(name) Then
            contactLists = CurrentContactListFacade.GetAll()
        Else
            contactLists = CurrentContactListFacade.GetByName(name)
        End If

        PopulateTreeView(contactLists)
    End Sub

    Private Sub PopulateTreeView(contactLists As IList(Of ContactList))
        RadTreeViewLists.Nodes(0).Nodes.Clear()
        For Each item As ContactList In contactLists
            Dim node As New RadTreeNode()
            node.Text = item.Name
            node.Value = item.Id.ToString()
            node.Font.Bold = True
            RadTreeViewLists.Nodes(0).Nodes.Add(node)
        Next
    End Sub

    Private Sub LoadContacts()
        Dim selectedNode As RadTreeNode = RadTreeViewLists.SelectedNode()
        If selectedNode IsNot Nothing Then
            pnlDetails.Visible = True
            lblListName.Text = selectedNode.Text
            Dim contactList As ContactList = CurrentContactListFacade.GetById(Guid.Parse(RadTreeViewLists.SelectedValue))
            If contactList IsNot Nothing Then
                Dim contactsDto As IList(Of ContactDTO) = New List(Of ContactDTO)
                For Each contact As Contact In contactList.Contacts
                    Dim contactDto As ContactDTO = New ContactDTO
                    contactDto.Contact = contact
                    contactDto.Type = ContactDTO.ContactType.Address
                    contactsDto.Add(contactDto)
                Next

                pnlContacts.Text = String.Format("{0} ({1})", "Contatti", contactsDto.Count)
                uscContacts.InnerPanelButtons.Visible = False
                uscContacts.DataSource = contactsDto
                uscContacts.DataBind()
            End If
        End If
    End Sub

#End Region


End Class