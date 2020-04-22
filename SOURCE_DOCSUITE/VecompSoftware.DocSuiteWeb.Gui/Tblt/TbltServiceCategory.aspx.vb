Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI

Partial Class TbltServiceCategory
    Inherits CommonBasePage

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
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

#End Region

#Region " Methods "

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, RadTreeViewServiceCategories)
        AjaxManager.AjaxSettings.AddAjaxSetting(RadTreeViewServiceCategories, pnlButtons)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlButtons)
        AddHandler AjaxManager.AjaxRequest, AddressOf TbltServiceCategoryAjaxRequest
    End Sub


    Private Sub InitializeButtons()
        btnAggiungi.Enabled = True
        btnElimina.Enabled = True
        btnRinomina.Enabled = True
        If Not RadTreeViewServiceCategories.SelectedNode Is Nothing Then
            If RadTreeViewServiceCategories.SelectedNode.Equals(RadTreeViewServiceCategories.Nodes(0)) Then
                btnAggiungi.Enabled = True
                btnElimina.Enabled = False
                btnRinomina.Enabled = False
            Else
                btnAggiungi.Enabled = False
                btnElimina.Enabled = True
                btnRinomina.Enabled = True
            End If
            RadTreeViewServiceCategories.ContextMenus(0).Enabled = True
        Else
            btnAggiungi.Enabled = False
            btnElimina.Enabled = False
            btnRinomina.Enabled = False
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
        pnlButtons.Visible = True
        RadTreeViewServiceCategories.ContextMenus(0).Visible = True
        RadTreeViewServiceCategories.OnClientContextMenuItemClicked = "ContextMenuItemClicked"
        RadTreeViewServiceCategories.OnClientContextMenuShowing = "ContextMenuShowing"
        InitializeButtons()
    End Sub

    Private Sub DisableModifyMode()
        pnlButtons.Visible = False
        RadTreeViewServiceCategories.ContextMenus(0).Visible = False
        RadTreeViewServiceCategories.OnClientContextMenuItemClicked = ""
        RadTreeViewServiceCategories.OnClientContextMenuShowing = ""
        RadTreeViewServiceCategories.Enabled = False
        RadTreeViewServiceCategories.SelectedNode.Selected = False
    End Sub

#End Region

End Class







