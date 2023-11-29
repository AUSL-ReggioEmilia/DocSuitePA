Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI

Partial Class TbltServiceCategory
    Inherits CommonBasePage

#Region "Fields"
    Private Const CREATE_OPTION As String = "create"
    Private Const MODIFY_OPTION As String = "modify"
    Private Const DELETE_OPTION As String = "delete"
#End Region
#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        If Not CommonShared.HasGroupAdministratorRight Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If

        InitializeAjaxSettings()

        If Not Page.IsPostBack Then
            InitializeButtons()
            LoadObjects("ProtDB")
        End If
    End Sub

    Protected Sub TbltServiceCategoryAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Select Case e.Argument
            Case "Update"
                LoadObjects("ProtDB")
        End Select
    End Sub

    Protected Sub RadTreeViewServiceCategories_NodeClick(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadTreeNodeEventArgs) Handles RadTreeViewServiceCategories.NodeClick
        InitializeButtons()
    End Sub

    Protected Sub FolderToolBar_ButtonClick(sender As Object, e As RadToolBarEventArgs) Handles FolderToolBar.ButtonClick
        Select Case e.Item.Value
            Case CREATE_OPTION
                AjaxManager.ResponseScripts.Add("OpenEditWindow('wdEditServiceCategory','Add');")
            Case DELETE_OPTION
                AjaxManager.ResponseScripts.Add("OpenEditWindow('wdEditServiceCategory','Delete');")
            Case MODIFY_OPTION
                AjaxManager.ResponseScripts.Add("OpenEditWindow('wdEditServiceCategory','Rename');")
        End Select
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, RadTreeViewServiceCategories)
        AjaxManager.AjaxSettings.AddAjaxSetting(RadTreeViewServiceCategories, FolderToolBar)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, FolderToolBar)
        AddHandler AjaxManager.AjaxRequest, AddressOf TbltServiceCategoryAjaxRequest
    End Sub


    Private Sub InitializeButtons()
        FolderToolBar.FindItemByValue(CREATE_OPTION).Enabled = True
        FolderToolBar.FindItemByValue(DELETE_OPTION).Enabled = True
        FolderToolBar.FindItemByValue(MODIFY_OPTION).Enabled = True
        If Not RadTreeViewServiceCategories.SelectedNode Is Nothing Then
            If RadTreeViewServiceCategories.SelectedNode.Equals(RadTreeViewServiceCategories.Nodes(0)) Then
                FolderToolBar.FindItemByValue(CREATE_OPTION).Enabled = True
                FolderToolBar.FindItemByValue(DELETE_OPTION).Enabled = False
                FolderToolBar.FindItemByValue(MODIFY_OPTION).Enabled = False
            Else
                FolderToolBar.FindItemByValue(CREATE_OPTION).Enabled = False
                FolderToolBar.FindItemByValue(DELETE_OPTION).Enabled = True
                FolderToolBar.FindItemByValue(MODIFY_OPTION).Enabled = True
            End If
            RadTreeViewServiceCategories.ContextMenus(0).Enabled = True
        Else
            FolderToolBar.FindItemByValue(CREATE_OPTION).Enabled = False
            FolderToolBar.FindItemByValue(DELETE_OPTION).Enabled = False
            FolderToolBar.FindItemByValue(MODIFY_OPTION).Enabled = False
            RadTreeViewServiceCategories.ContextMenus(0).Enabled = False
        End If
    End Sub

    Private Sub SetNode(ByRef node As RadTreeNode, ByVal obj As ServiceCategory)
        node.Value = obj.Id.ToString()
        node.Text = "(" & obj.Code & ") " & obj.Description
        node.ImageUrl = "../Comm/images/Oggetto.gif"
        RadTreeViewServiceCategories.Nodes(0).Nodes.Add(node)
        node.Expanded = True
    End Sub

    Private Sub LoadObjects(ByVal objType As String)
        Dim Modify As Boolean

        Dim objectFacade As New ServiceCategoryFacade()
        Dim objects As IList(Of ServiceCategory)

        Select Case UCase(objType)
            Case "DOCMDB"
                Modify = CommonShared.UserDocumentCheckRight(DocumentContainerRightPositions.Insert)
            Case "PROTDB"
                Modify = CommonShared.UserProtocolCheckRight(ProtocolContainerRightPositions.Insert)
            Case "RESLDB"
                Modify = CommonShared.UserResolutionCheckRight(ResolutionRightPositions.Insert)
        End Select


        RadTreeViewServiceCategories.Nodes(0).Selected = True

        objects = objectFacade.GetAll(objType)
        Dim selnode As RadTreeNode = RadTreeViewServiceCategories.SelectedNode
        RadTreeViewServiceCategories.Nodes(0).Nodes.Clear()
        For Each obj As ServiceCategory In objects
            Dim node As New RadTreeNode
            SetNode(node, obj)
        Next
        If selnode IsNot Nothing Then
            selnode = RadTreeViewServiceCategories.FindNodeByValue(selnode.Value)
            If selnode IsNot Nothing Then
                selnode.Selected = True
            Else
                RadTreeViewServiceCategories.Nodes(0).Selected = True
            End If
        End If
        'Abilitazione
        If Modify Then
            EnableModifyMode()
        Else
            DisableModifyMode()
        End If
    End Sub

    Private Sub EnableModifyMode()
        FolderToolBar.Visible = True
        RadTreeViewServiceCategories.ContextMenus(0).Visible = True
        RadTreeViewServiceCategories.OnClientContextMenuItemClicked = "ContextMenuItemClicked"
        RadTreeViewServiceCategories.OnClientContextMenuShowing = "ContextMenuShowing"
        InitializeButtons()
    End Sub

    Private Sub DisableModifyMode()
        FolderToolBar.Visible = False
        RadTreeViewServiceCategories.ContextMenus(0).Visible = False
        RadTreeViewServiceCategories.OnClientContextMenuItemClicked = ""
        RadTreeViewServiceCategories.OnClientContextMenuShowing = ""
        RadTreeViewServiceCategories.Enabled = False
        RadTreeViewServiceCategories.SelectedNode.Selected = False
    End Sub

#End Region

End Class







