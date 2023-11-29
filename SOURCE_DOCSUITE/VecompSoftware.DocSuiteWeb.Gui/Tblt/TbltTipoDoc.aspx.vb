Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI

Partial Public Class TbltTipoDoc
    Inherits CommonBasePage

#Region "Fields"
    Private Const CREATE_OPTION As String = "create"
    Private Const MODIFY_OPTION As String = "modify"
    Private Const DELETE_OPTION As String = "delete"
    Private Const REFRESH_OPTION As String = "refresh"
    Private Const RECOVER_OPTION As String = "recover"
    Private Const LOG_OPTION As String = "log"
#End Region
#Region "Page Load"

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If Not CommonShared.HasGroupAdministratorRight Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If


        InitializeAjaxSettings()
        InitializeControls()
        If Not Me.IsPostBack Then
            Initialize()
            RadTreeView1.Nodes(0).Selected = True
            InitializeButtons()
        End If
    End Sub

#End Region

#Region "Initialize"
    Private Sub InitializeControls()
        FolderToolBar.FindItemByValue(REFRESH_OPTION).Style.Add("display", "none")
        WebUtils.ExpandOnClientNodeAttachEvent(RadTreeView1)
    End Sub

    Private Sub InitializeAjaxSettings()
        Me.AjaxManager.AjaxSettings.AddAjaxSetting(Me.AjaxManager, RadTreeView1)
        Me.AjaxManager.AjaxSettings.AddAjaxSetting(RadTreeView1, FolderToolBar)
        Me.AjaxManager.AjaxSettings.AddAjaxSetting(Me.AjaxManager, FolderToolBar)
        AddHandler Me.AjaxManager.AjaxRequest, AddressOf TbltTipoDoc_AjaxRequest
    End Sub

    Private Sub Initialize()
        LoadTableDocType()
        'Abilitazione
        If CommonUtil.HasGroupTblCategoryRight Then
            FolderToolBar.Visible = True
            InitializeButtons()
            CreateContextMenu(RadTreeView1)
        Else
            FolderToolBar.Visible = False
        End If
    End Sub

    Private Sub InitializeButtons()

        FolderToolBar.FindItemByValue(CREATE_OPTION).Enabled = True
        FolderToolBar.FindItemByValue(DELETE_OPTION).Enabled = True
        FolderToolBar.FindItemByValue(MODIFY_OPTION).Enabled = True

        If RadTreeView1.SelectedNode Is Nothing Then
            RadTreeView1.Nodes(0).Selected = True
        End If

        If RadTreeView1.SelectedNode.Equals(RadTreeView1.Nodes(0)) Then
            FolderToolBar.FindItemByValue(MODIFY_OPTION).Enabled = False
            FolderToolBar.FindItemByValue(DELETE_OPTION).Enabled = False
            FolderToolBar.FindItemByValue(RECOVER_OPTION).Visible = False
            FolderToolBar.FindItemByValue(LOG_OPTION).Enabled = False
        Else
            FolderToolBar.FindItemByValue(LOG_OPTION).Enabled = True
            If RadTreeView1.SelectedNode.Style.Item("color") = "gray" Then
                FolderToolBar.FindItemByValue(RECOVER_OPTION).Visible = True
                FolderToolBar.FindItemByValue(DELETE_OPTION).Visible = False
            Else
                FolderToolBar.FindItemByValue(DELETE_OPTION).Visible = True
                FolderToolBar.FindItemByValue(RECOVER_OPTION).Visible = False
            End If
        End If
    End Sub

#End Region

#Region "ContextMenu"
    Private Sub CreateContextMenu(ByRef tree As RadTreeView)
        Dim item As RadMenuItem = Nothing
        Dim menu As New RadTreeViewContextMenu()
        menu.Items.Add(TreeViewUtils.CreateMenuItem("Aggiungi", "Add", Unit.Pixel(200)))
        menu.Items.Add(TreeViewUtils.CreateMenuItem("Rinomina", "Rename", Unit.Pixel(200)))
        menu.Items.Add(TreeViewUtils.CreateMenuItem("Elimina", "Delete", Unit.Pixel(200)))
        menu.Items.Add(TreeViewUtils.CreateMenuItem("Recupera", "Recovery", Unit.Pixel(200)))
        menu.Items.Add(TreeViewUtils.CreateMenuItem("Log", "Log", Unit.Pixel(200)))

        tree.ContextMenus.Add(menu)
        tree.OnClientContextMenuItemClicked = "ContextMenuItemClicked"
        tree.OnClientContextMenuShowing = "ContextMenuShowing"
    End Sub
#End Region

#Region "Ajax Request"
    Protected Sub TbltTipoDoc_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Select Case e.Argument
            Case "Update"
                Dim node As RadTreeNode = RadTreeView1.SelectedNode
                LoadTableDocType()
                node = RadTreeView1.FindNodeByValue(node.Value)
                If node Is Nothing Then
                    node = RadTreeView1.Nodes(0)
                End If
                node.Selected = True
                RadTreeView1_NodeClick(RadTreeView1, New RadTreeNodeEventArgs(node))
        End Select
    End Sub
#End Region

#Region "Events"
    Private Sub RadTreeView1_NodeClick(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadTreeNodeEventArgs) Handles RadTreeView1.NodeClick
        InitializeButtons()
    End Sub

    Protected Sub FolderToolBar_ButtonClick(sender As Object, e As RadToolBarEventArgs) Handles FolderToolBar.ButtonClick
        Select Case e.Item.Value
            Case CREATE_OPTION
                AjaxManager.ResponseScripts.Add("OpenEditWindow('windowEdit','Add');")
            Case DELETE_OPTION
                AjaxManager.ResponseScripts.Add("OpenEditWindow('windowEdit','Delete');")
            Case MODIFY_OPTION
                AjaxManager.ResponseScripts.Add("OpenEditWindow('windowEdit','Rename');")
            Case RECOVER_OPTION
                AjaxManager.ResponseScripts.Add("OpenEditWindow('windowEdit','Recovery');")
            Case LOG_OPTION
                AjaxManager.ResponseScripts.Add("OpenEditWindow('windowEdit','Log');")
        End Select
    End Sub
#End Region

#Region "Private Methods"
    Private Sub LoadTableDocType()
        Dim tn As RadTreeNode = Nothing
        Dim tabledocs As IList(Of DocumentType)

        tabledocs = Facade.TableDocTypeFacade.GetAll()
        RadTreeView1.Nodes(0).Nodes.Clear()
        If tabledocs.Count > 0 Then
            For Each tabledoc As DocumentType In tabledocs
                tn = New RadTreeNode
                tn.Value = tabledoc.Id
                tn.Text = tabledoc.Description
                If ("" & tabledoc.Code) <> "" Then tn.Text = "(" & tabledoc.Code & ") " & tn.Text
                tn.ImageUrl = "../Comm/images/Oggetto.gif"
                If Not tabledoc.IsActive Then
                    tn.Style.Add("color", "gray")
                End If
                tn.Attributes.Add("IsActive", tabledoc.IsActive.ToString())
                RadTreeView1.Nodes(0).Nodes.Add(tn)
            Next
        End If
        InitializeButtons()
    End Sub
#End Region

#Region "Enum"
    Enum OrderType
        DESC = 0
        ASC = 1
    End Enum
#End Region


End Class