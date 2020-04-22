Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI

Partial Public Class TbltTipoDoc
    Inherits CommonBasePage

#Region "Page Load"

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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
        btnRefresh.Style.Add("display", "none")
        WebUtils.ExpandOnClientNodeAttachEvent(RadTreeView1)
    End Sub

    Private Sub InitializeAjaxSettings()
        Me.AjaxManager.AjaxSettings.AddAjaxSetting(Me.AjaxManager, RadTreeView1)
        Me.AjaxManager.AjaxSettings.AddAjaxSetting(RadTreeView1, pnlButtons)
        Me.AjaxManager.AjaxSettings.AddAjaxSetting(Me.AjaxManager, pnlButtons)
        AddHandler Me.AjaxManager.AjaxRequest, AddressOf TbltTipoDoc_AjaxRequest
    End Sub

    Private Sub Initialize()
        LoadTableDocType()
        'Abilitazione
        If CommonUtil.HasGroupTblCategoryRight Then
            pnlButtons.Visible = True
            InitializeButtons()
            CreateContextMenu(RadTreeView1)
        Else
            pnlButtons.Visible = False
        End If
    End Sub

    Private Sub InitializeButtons()

        btnAggiungi.Enabled = True
        btnElimina.Enabled = True
        btnRinomina.Enabled = True

        If RadTreeView1.SelectedNode Is Nothing Then
            RadTreeView1.Nodes(0).Selected = True
        End If

        If RadTreeView1.SelectedNode.Equals(RadTreeView1.Nodes(0)) Then
            btnRinomina.Enabled = False
            btnElimina.Enabled = False
            btnRecupera.Visible = False
            btnLog.Enabled = False
        Else
            btnLog.Enabled = True
            If RadTreeView1.SelectedNode.Style.Item("color") = "gray" Then
                btnRecupera.Visible = True
                btnElimina.Visible = False
            Else
                btnElimina.Visible = True
                btnRecupera.Visible = False
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
                If tabledoc.IsActive <> 1 Then
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