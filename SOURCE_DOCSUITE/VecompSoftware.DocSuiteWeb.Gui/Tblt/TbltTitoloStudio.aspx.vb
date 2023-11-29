Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports Telerik.Web.UI

Partial Public Class TbltTitoloStudio
    Inherits CommonBasePage

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        If Not CommonShared.HasGroupAdministratorRight Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If

        InitializeAjaxSettings()
        InitializeControls()
        If Not IsPostBack Then
            Initialize()
            RadTreeView1.Nodes(0).Selected = True
            InitializeButtons()
        End If
    End Sub

    Protected Sub TbltTitoloStudio_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        If e.Argument.Eq("Update") Then
            Dim selectedNode As RadTreeNode = RadTreeView1.SelectedNode
            LoadTitoliStudio()
            selectedNode = RadTreeView1.FindNodeByValue(selectedNode.Value)
            If selectedNode IsNot Nothing AndAlso selectedNode.Value <> "" Then
                selectedNode.Selected = True
            Else
                RadTreeView1.Nodes(0).Selected = True
            End If
            InitializeButtons()
        End If
    End Sub

    Private Sub RadTreeView1_NodeClick(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs) Handles RadTreeView1.NodeClick
        InitializeButtons()
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeControls()
        WebUtils.ObjAttDisplayNone(btnRefresh)
        WebUtils.ExpandOnClientNodeAttachEvent(RadTreeView1)
    End Sub

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, RadTreeView1)
        AjaxManager.AjaxSettings.AddAjaxSetting(RadTreeView1, pnlButtons)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlButtons)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, btnElimina)
        AddHandler AjaxManager.AjaxRequest, AddressOf TbltTitoloStudio_AjaxRequest
    End Sub

    Private Sub Initialize()
        LoadTitoliStudio()

        'Abilitazione
        Dim GesTblContact As Boolean = CommonShared.HasGroupAdministratorRight() OrElse CommonShared.HasGroupTblContactRight
        If GesTblContact Then
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
            btnAggiungi.Enabled = True
            btnElimina.Enabled = False
            btnRinomina.Enabled = False
        Else
            btnAggiungi.Enabled = True
            btnElimina.Enabled = True
            btnRinomina.Enabled = True
        End If
        SetRecoveryButton()
    End Sub

    Private Sub SetRecoveryButton()
        Dim tn As RadTreeNode = RadTreeView1.SelectedNode

        If tn.Attributes("isActive").Eq("false") Then
            btnElimina.Text = "Recupera"
            btnElimina.OnClientClick = "OpenEditWindow('windowEdit','Recovery');return false;"
        Else
            btnElimina.Text = "Elimina"
            btnElimina.OnClientClick = "OpenEditWindow('windowEdit','Delete');return false;"
        End If
    End Sub

    Private Sub CreateContextMenu(ByRef tree As RadTreeView)
        Dim menu As New RadTreeViewContextMenu()
        menu.Items.Add(TreeViewUtils.CreateMenuItem("Aggiungi", "Add", Unit.Pixel(200)))
        menu.Items.Add(TreeViewUtils.CreateMenuItem("Modifica", "Rename", Unit.Pixel(200)))
        menu.Items.Add(TreeViewUtils.CreateMenuItem("Elimina", "Delete", Unit.Pixel(200)))
        menu.Items.Add(TreeViewUtils.CreateMenuItem("Recupera", "Recovery", Unit.Pixel(200)))

        tree.ContextMenus.Add(menu)
        tree.OnClientContextMenuItemClicked = "ContextMenuItemClicked"
        tree.OnClientContextMenuShowing = "ContextMenuShowing"
    End Sub

    Private Sub LoadTitoliStudio()
        Dim contactstitle As IList(Of ContactTitle) = Facade.ContactTitleFacade.GetAll()
        If contactstitle.Count <= 0 Then
            Exit Sub
        End If

        RadTreeView1.Nodes(0).Nodes.Clear()
        For Each contacttitle As ContactTitle In contactstitle
            Dim tn As New RadTreeNode
            tn.Value = contacttitle.Id.ToString()
            tn.Text = contacttitle.Description

            If Not contacttitle.IsActive Then
                'tn.Attributes.Add("Color", "#808080")
                tn.ForeColor = Drawing.Color.Gray
            End If

            If Not String.IsNullOrEmpty(contacttitle.Code) Then
                tn.Text = String.Format("({0}) {1}", contacttitle.Code, tn.Text)
            End If
            tn.ImageUrl = "../Comm/images/Oggetto.gif"
            tn.Attributes.Add("isActive", contacttitle.IsActive.ToString())
            RadTreeView1.Nodes(0).Nodes.Add(tn)
        Next
        InitializeButtons()
    End Sub

#End Region

End Class