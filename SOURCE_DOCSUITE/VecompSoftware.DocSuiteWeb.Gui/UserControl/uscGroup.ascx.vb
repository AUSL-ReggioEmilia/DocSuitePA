Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports Newtonsoft.Json

Partial Public Class uscGroup
    Inherits DocSuite2008BaseControl

#Region "Delegates"
    Public Delegate Sub OnNodeClick(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs)
    Public Delegate Sub OnNodeAdd(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs)
    Public Delegate Sub OnNodeRemove(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs)
#End Region

#Region "UserControl Events"
    Public Event NodeClick As OnNodeClick
    Public Event NodeAdd As OnNodeAdd
    Public Event NodeRemove As OnNodeRemove
#End Region

#Region "Fields"

    Private Const SCROLL_TREE As String = "setTimeout('ScrollToSelectedNode()', 20);"
    Private _addFromRoleVisible As Boolean = False
#End Region

#Region "Properties"

    Public Property AddFromRoleVisible As Boolean
        Get
            Return _addFromRoleVisible
        End Get
        Set(value As Boolean)
            _addFromRoleVisible = value
        End Set
    End Property

    Public Property ContainerName As String

    Public Property CurrentGroupFacade As Object

    Public Property ContainerImageURL As String

    Public Property CurrentGroups As IEnumerable(Of GroupRights)

    Public Property GroupName As String

    Public ReadOnly Property SelectedNode As RadTreeNode
        Get
            Return RadTreeViewGroups.SelectedNode
        End Get
    End Property

    Public ReadOnly Property GetTreeView As RadTreeView
        Get
            Return RadTreeViewGroups
        End Get
    End Property
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjaxSettings()
        If Not IsPostBack Then
            btnAddFromRole.Visible = AddFromRoleVisible
            InitializeTreeContainer()
            LoadGroups()
        End If

        If Me.Visible Then
            AjaxManager.ResponseScripts.Add(SCROLL_TREE)
        End If
    End Sub

    Private Sub btnEliminaGruppo_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnEliminaGruppo.Click
        Dim currentSelectedNode As RadTreeNode = SelectedNode
        If Not IsRootNode(currentSelectedNode) Then
            RemoveGroupFromContext(currentSelectedNode)
            currentSelectedNode.Attributes.Add("OtherGroupsRemaining", (CurrentGroups.Count > 0).ToJavaScriptString())
            RemoveGroupFromTreeView(currentSelectedNode)
        End If
    End Sub

    Protected Sub uscGroup_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch
        End Try

        If ajaxModel Is Nothing Then
            Return
        End If

        Select Case ajaxModel.ActionName
            Case "AddRoleGroup"
                Dim idRoles As IList(Of Integer) = JsonConvert.DeserializeObject(Of IList(Of Integer))(ajaxModel.Value(0))
                For Each idRole As Integer In idRoles
                    CreateNodeFromRole(idRole)
                Next
            Case "AddGroup"
                Dim groupNames As IList(Of String) = JsonConvert.DeserializeObject(Of IList(Of String))(ajaxModel.Value(0))
                For Each groupName As String In groupNames
                    CreateNode(groupName)
                    Dim node As RadTreeNode = RadTreeViewGroups.FindNodeByText(groupName)
                    RaiseEvent NodeClick(sender, New RadTreeNodeEventArgs(node))
                Next
                If groupNames.Count > 0 Then
                    Dim rootNode As RadTreeNode = RadTreeViewGroups.FindNodeByText(groupNames.First())
                    rootNode.Selected = True
                    rootNode.Focus()
                    RaiseEvent NodeClick(sender, New RadTreeNodeEventArgs(rootNode))
                End If
        End Select

        AjaxManager.ResponseScripts.Add("HideLoadingPanel();")
    End Sub

    Public Sub AddGroup(ByVal groupName As String)

        CreateNode(groupName)
        Dim node As RadTreeNode = RadTreeViewGroups.FindNodeByText(groupName)
        RaiseEvent NodeClick(New Object, New RadTreeNodeEventArgs(node))

    End Sub

    Private Sub RadTreeViewGroups_NodeClick(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs) Handles RadTreeViewGroups.NodeClick
        btnEliminaGruppo.Enabled = Not IsRootNode(e.Node)
        RaiseEvent NodeClick(sender, e)
    End Sub
#End Region

#Region " Methods "
    Private Sub InitializeAjaxSettings()
        AddHandler AjaxManager.AjaxRequest, AddressOf uscGroup_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, RadTreeViewGroups)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlButtons)
        AjaxManager.AjaxSettings.AddAjaxSetting(RadTreeViewGroups, pnlButtons)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnEliminaGruppo, RadTreeViewGroups, BasePage.MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub InitializeTreeContainer()
        Dim node As RadTreeNode = New RadTreeNode With {
            .Text = ContainerName,
            .ImageUrl = ContainerImageURL
        }
        RadTreeViewSelectedRole.Nodes().Clear()
        RadTreeViewSelectedRole.Nodes().Add(node)
    End Sub

    Private Sub LoadGroups()
        If CurrentGroups Is Nothing OrElse CurrentGroups.Count = 0 Then
            Exit Sub
        End If

        For Each group As GroupRights In CurrentGroups
            Dim node As RadTreeNode = CreateGroupNode(group.Name, group.Id.ToString())
            RadTreeViewGroups.Nodes(0).Nodes.Add(node)
            If GroupName.Eq(node.Text) Then
                node.Selected = True
                RaiseEvent NodeClick(Me, New RadTreeNodeEventArgs(node))
            End If
        Next
    End Sub

    Private Function CreateGroupNode(name As String, value As String) As RadTreeNode
        Dim node As RadTreeNode = New RadTreeNode With {
            .Text = name,
            .Value = value,
            .ImageUrl = "../App_Themes/DocSuite2008/imgset16/GroupMembers.png"
        }
        Return node
    End Function

    Public Function IsRootNode(node As RadTreeNode) As Boolean
        Return (node Is Nothing OrElse node.Value.Eq("Root"))
    End Function

    Private Sub RemoveGroupFromContext(group As RadTreeNode)
        Dim groupRight As GroupRights = CurrentGroups.SingleOrDefault(Function(s) s.Name.Eq(group.Text))
        If groupRight IsNot Nothing Then
            CurrentGroupFacade.DeleteGroup(groupRight)
            CurrentGroups.ToList().Remove(groupRight)
        End If
    End Sub

    Private Sub RemoveGroupFromTreeView(group As RadTreeNode)
        If (RadTreeViewGroups.Nodes(0).Nodes.Count > 0) Then
            RadTreeViewGroups.Nodes(0).Nodes.Remove(group)
            RadTreeViewGroups.Nodes(0).Selected = True
            btnEliminaGruppo.Enabled = False
            RaiseEvent NodeRemove(Me, New RadTreeNodeEventArgs(group))
        End If
    End Sub

    Public Function GetGroup(name As String) As Object
        Return CurrentGroups.SingleOrDefault(Function(s) s.Name.Eq(name))
    End Function

    Public Sub Refresh()
        RadTreeViewGroups.Nodes(0).Nodes.Clear()
        LoadGroups()
    End Sub

    Private Sub CreateNodeFromRole(idRole As Integer)
        Dim role As Role = Facade.RoleFacade.GetById(idRole)
        If role Is Nothing Then
            Exit Sub
        End If

        For Each roleGroup As RoleGroup In role.RoleGroups
            If Not CheckGroupExist(roleGroup.Name) Then
                Continue For
            End If

            CreateNode(roleGroup.Name)
            Dim node As RadTreeNode = RadTreeViewGroups.FindNodeByText(roleGroup.Name)
            RaiseEvent NodeClick(Me, New RadTreeNodeEventArgs(node))
        Next
    End Sub

    Private Sub CreateNode(name As String)
        Dim rootNode As RadTreeNode = RadTreeViewGroups.Nodes(0)
        Dim newNode As RadTreeNode = RadTreeViewGroups.FindNodeByText(name)
        If newNode Is Nothing Then
            newNode = CreateGroupNode(name, String.Empty)
            rootNode.Nodes.Add(newNode)
        End If
        newNode.Selected = True
        newNode.Focus()
        TreeViewUtils.SortNodes(RadTreeViewGroups.Nodes(0).Nodes)
        btnEliminaGruppo.Enabled = True
    End Sub

    Private Function CheckGroupExist(groupName As String) As Boolean
        Dim securityGroup As SecurityGroups = Facade.SecurityGroupsFacade.GetGroupByName(groupName)
        Return securityGroup IsNot Nothing
    End Function
#End Region

End Class