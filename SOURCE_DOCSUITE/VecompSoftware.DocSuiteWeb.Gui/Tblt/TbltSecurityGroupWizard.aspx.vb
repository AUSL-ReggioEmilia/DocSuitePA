Imports System.Collections.Generic
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class TbltSecurityGroupWizard
    Inherits CommonBasePage
    Private Enum ContainerTreeNodeType
        ContainerRoot
        RoleRoot
        Container
        Role
        SecurityGroup
    End Enum

#Region "Fields"
    Private _currentFinder As SecurityGroupFinder
    Private _currentUser As AccountModel
#End Region

#Region "Properties"
    Protected ReadOnly Property CurrentFinder As SecurityGroupFinder
        Get
            If _currentFinder Is Nothing Then
                _currentFinder = New SecurityGroupFinder()
            End If
            Return _currentFinder
        End Get
    End Property
    Private ReadOnly Property CurrentUser As AccountModel
        Get
            If _currentUser Is Nothing Then
                _currentUser = CommonAD.GetAccount(String.Format("{0}\{1}", Request.QueryString("DomainAD").ToString(), Request.QueryString("AccountAD").ToString()))
            End If
            Return _currentUser
        End Get
    End Property
    Private ReadOnly Property SearchTextBox() As RadTextBox
        Get
            Return DirectCast(SearchToolbar.Items.FindItemByValue("searchBtn").FindControl("searchInput"), RadTextBox)
        End Get
    End Property
    Private Property RoleRootNode() As RadTreeNode
    Private Property ContainerRootNode() As RadTreeNode
    Private Const ATTRNAME_NODETYPE As String = "NodeType"
    Private Const ATTRNAME_DEFAULTCHECKED As String = "DefaultChecked"
    Private Property SGDetailsPageUrlTemplateDictionary As Dictionary(Of String, String)

#End Region

#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not CommonShared.HasGroupAdministratorRight Then
            AjaxAlert("Sono necessari diritti amministrativi per vedere la pagina.")
            Exit Sub
        End If

        InitializeAjax()
        InitializeDetailPagesUrlTemplatesDictionary()
        If Not IsPostBack Then
            Title = String.Format("Configurazione Utente - {0}\{1}", Request.QueryString("DomainAD").ToString(), Request.QueryString("AccountAD").ToString())
            btnSave.Enabled = True
            groupDetails.ContextMenuUserEnabled = False
            InitializeTreeView()
        End If

        RoleRootNode = RadTreeGroups.Nodes().FindNode(Function(treeNode) treeNode.Attributes(ATTRNAME_NODETYPE) = ContainerTreeNodeType.RoleRoot.ToString())
        ContainerRootNode = RadTreeGroups.Nodes().FindNode(Function(treeNode) treeNode.Attributes(ATTRNAME_NODETYPE) = ContainerTreeNodeType.ContainerRoot.ToString())
    End Sub
    Private Sub InitializeDetailPagesUrlTemplatesDictionary()
        SGDetailsPageUrlTemplateDictionary = New Dictionary(Of String, String)
        SGDetailsPageUrlTemplateDictionary.Add(ContainerTreeNodeType.Role.ToString(), "../Tblt/TbltSettoreGesGruppi.aspx?Type=Comm&IdRole={0}&GroupName={1}&ReadonlySecurityGroups={2}")
        SGDetailsPageUrlTemplateDictionary.Add(ContainerTreeNodeType.Container.ToString(), "../Tblt/TbltContenitoriGesGruppi.aspx?Type=Comm&idContainer={0}&GroupName={1}&ReadonlySecurityGroups={2}")
    End Sub
    Private Sub InitializeTreeView()
        RadTreeGroups.Nodes.Clear()

        Dim contenitoriRootNode As RadTreeNode = New RadTreeNode()
        contenitoriRootNode.Text = "Contenitori"
        contenitoriRootNode.Attributes.Add(ATTRNAME_NODETYPE, ContainerTreeNodeType.ContainerRoot.ToString())
        contenitoriRootNode.Checkable = False
        contenitoriRootNode.ExpandMode = TreeNodeExpandMode.ServerSideCallBack

        Dim settoriRootNode As RadTreeNode = New RadTreeNode()
        settoriRootNode.Text = "Settori"
        settoriRootNode.Attributes.Add(ATTRNAME_NODETYPE, ContainerTreeNodeType.RoleRoot.ToString())
        settoriRootNode.Checkable = False
        settoriRootNode.ExpandMode = TreeNodeExpandMode.ServerSideCallBack

        RadTreeGroups.Nodes.Add(contenitoriRootNode)
        RadTreeGroups.Nodes.Add(settoriRootNode)
    End Sub
    Private Sub AppendEmptyNode(parentNode As RadTreeNode)
        Dim emptyNode As RadTreeNode = New RadTreeNode()
        emptyNode.Text = "Nessun elemento trovato"
        emptyNode.Checkable = False

        parentNode.Nodes.Add(emptyNode)
    End Sub

    Private Sub RadTreeGroups_NodeClick(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs) Handles RadTreeGroups.NodeClick
        Dim selectedNode As RadTreeNode = e.Node

        If Not String.IsNullOrEmpty(selectedNode.Value) Then
            groupDetails.SelectedGroupId = CInt(selectedNode.Value)
            groupDetails.LoadOnlyUsers()

            pnlGroupDetails.Visible = True

            Dim securityGroupDetailsPageUrl As String = GetSecurityGroupDetailsPageUrl(selectedNode)
            groupDetailsPane.ContentUrl = securityGroupDetailsPageUrl
        End If
    End Sub

    Private Function GetSecurityGroupDetailsPageUrl(selectedGroupNode As RadTreeNode) As String
        Dim groupName As String = selectedGroupNode.Text

        Dim selectedNodeParent As RadTreeNode = selectedGroupNode.ParentNode
        Dim parentId As String = selectedNodeParent.Value
        Dim parentNodeType As String = selectedNodeParent.Attributes(ATTRNAME_NODETYPE)

        Dim detailsPageUrlTemplate As String = SGDetailsPageUrlTemplateDictionary(parentNodeType)
        Dim detailsPageUrl As String = String.Format(detailsPageUrlTemplate, parentId, groupName, True)

        Return detailsPageUrl
    End Function

    Private Sub RadTreeGroups_NodeExpand(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs) Handles RadTreeGroups.NodeExpand
        Dim expandedNode As RadTreeNode = e.Node
        Dim expandedNodeType As String = expandedNode.Attributes(ATTRNAME_NODETYPE)
        expandedNode.Nodes.Clear()

        Select Case expandedNodeType
            Case ContainerTreeNodeType.ContainerRoot.ToString()
                LoadContainers(expandedNode)
            Case ContainerTreeNodeType.RoleRoot.ToString()
                LoadRootRoles(expandedNode)
            Case ContainerTreeNodeType.Role.ToString()
                LoadSecurityGroups(expandedNode)
                LoadRoleChildren(expandedNode)
            Case ContainerTreeNodeType.Container.ToString()
                LoadSecurityGroups(expandedNode)
        End Select

        If expandedNode.Nodes.Count = 0 Then
            AppendEmptyNode(expandedNode)
        End If
    End Sub

    Private Sub LoadSecurityGroups(parentNode As RadTreeNode)
        CurrentFinder.ClearFinder()

        If DecorateFinder(parentNode) Then
            Exit Sub
        End If

        Dim sGroups As IList(Of SecurityGroups) = CurrentFinder.DoSearch()
        Dim sGroupNodeImageUrl As String = "../App_Themes/DocSuite2008/imgset16/GroupMembers.png"

        For Each group As SecurityGroups In sGroups
            Dim securityGroupNode As RadTreeNode = CreateTreeNode(group.GroupName, group.Id.ToString(), sGroupNodeImageUrl, checkable:=True, expandable:=False)
            Dim currentUserIsInGroup As Boolean = CheckIfCurrentUserIsInGroup(group)
            securityGroupNode.Attributes.Add(ATTRNAME_NODETYPE, ContainerTreeNodeType.SecurityGroup.ToString())
            securityGroupNode.Checked = currentUserIsInGroup

            If currentUserIsInGroup Then
                securityGroupNode.Attributes.Add(ATTRNAME_DEFAULTCHECKED, True.ToString())
            End If

            parentNode.Nodes.Add(securityGroupNode)
        Next
    End Sub

    Private Sub AddRolesToTreeView(roles As IList(Of Role), roleParentNode As RadTreeNode)
        Dim nodeImageUrl As String = Nothing
        For Each roleModel As Role In roles
            nodeImageUrl = If(roleModel.Father Is Nothing, "../App_Themes/DocSuite2008/imgset16/bricks.png", "../App_Themes/DocSuite2008/imgset16/brick.png")
            Dim roleTreeNode As RadTreeNode = CreateTreeNode(roleModel.FullDescription, roleModel.Id.ToString(), nodeImageUrl)
            roleTreeNode.Attributes.Add(ATTRNAME_NODETYPE, ContainerTreeNodeType.Role.ToString())
            roleParentNode.Nodes.Add(roleTreeNode)
        Next
    End Sub
    Private Sub LoadRootRoles(parentNode As RadTreeNode)
        parentNode.Nodes().Clear()
        Dim roles As IList(Of Role) = FindRoles(loadOnlyRoots:=True)
        AddRolesToTreeView(roles, parentNode)
    End Sub
    Private Sub LoadRoleChildren(expandedNode As RadTreeNode)
        Dim parentRole As Role = New Role()
        parentRole.Id = CInt(expandedNode.Value)

        Dim roles As IList(Of Role) = FindRoles(parentRole)
        AddRolesToTreeView(roles, expandedNode)
    End Sub
    Private Sub LoadContainers(parentNode As RadTreeNode)
        parentNode.Nodes.Clear()
        Dim containers As IList(Of Container) = Facade.ContainerFacade.GetAllOrdered("Name ASC")
        Dim nodeImageUrl As String = "../App_Themes/DocSuite2008/imgset16/box_open.png"

        For Each container As Container In containers
            Dim containerTreeNode As RadTreeNode = CreateTreeNode(container.Name, container.Id.ToString(), nodeImageUrl)
            containerTreeNode.Attributes.Add(ATTRNAME_NODETYPE, ContainerTreeNodeType.Container.ToString())
            parentNode.Nodes.Add(containerTreeNode)
        Next
    End Sub
    Private Function FindRoles(Optional parentRole As Role = Nothing, Optional loadOnlyRoots As Boolean = False, Optional roleNameFilter As String = Nothing) As IList(Of Role)
        Dim roles As IList(Of Role) = Facade.RoleFacade.GetRoles(DSWEnvironment.Any, Nothing, True, roleNameFilter, loadOnlyRoots, parentRole, TenantId)
        Return roles
    End Function
    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As ButtonClickEventArgs) Handles btnSave.Click
        Dim errorGroups As List(Of String) = New List(Of String)()
        Dim allSelectedGroups As List(Of RadTreeNode) = CType(RadTreeGroups.CheckedNodes, List(Of RadTreeNode))
        Dim newSelectedGroups As List(Of RadTreeNode) = allSelectedGroups.FindAll(Function(node) node.Attributes(ATTRNAME_DEFAULTCHECKED) Is Nothing)
        If newSelectedGroups.Count = 0 Then
            AjaxAlert("Spuntare almeno un gruppo")
            AjaxManager.ResponseScripts.Add("closeLoadingPanel();")
            Exit Sub
        End If
        For Each groupId As RadTreeNode In newSelectedGroups
            Dim group As SecurityGroups = Facade.SecurityGroupsFacade.GetById(Integer.Parse(groupId.Value))
            If group IsNot Nothing AndAlso Facade.SecurityUsersFacade.IsUserInGroup(group, CurrentUser.GetFullUserName) Then
                errorGroups.Add(group.GroupName)
                Continue For
            End If
            Facade.SecurityUsersFacade.Insert(CurrentUser.Domain, CurrentUser.Account, CurrentUser.Name, group)
        Next
        Dim errorMessage As String = "Utente inserito con successo"
        If errorGroups.Count > 0 Then
            errorMessage = String.Format("L'utente {0} è già presente nei gruppi: " & vbCrLf, CurrentUser.GetFullUserName)
            For Each item As String In errorGroups
                errorMessage = String.Concat(errorMessage, item & vbCrLf)
            Next
        End If
        AjaxAlert(errorMessage)
        AjaxManager.ResponseScripts.Add("closeLoadingPanel();")

        MakeNodesCheckboxReadonly(newSelectedGroups)

        If pnlGroupDetails.Visible Then
            RefreshGroupUsersPanel()
        End If
    End Sub
    Private Sub btnBack_Click(ByVal sender As Object, ByVal e As ButtonClickEventArgs) Handles btnBack.Click
        Dim param As String = "Type=Comm"
        Response.Redirect(String.Format("../Tblt/TbltSecurityUsers.aspx?", CommonShared.AppendSecurityCheck(param)))
    End Sub
    Private Sub SearchToolbar_ButtonClick(ByVal sender As Object, ByVal e As RadToolBarEventArgs) Handles SearchToolbar.ButtonClick
        Dim searchString As String = SearchTextBox.Text
        FilterTreeView(searchString)
    End Sub
#End Region

#Region "Methods"
    Private Sub FilterTreeView(filterKeyword As String)

        If String.IsNullOrEmpty(filterKeyword) Then
            ContainerRootNode.Expanded = False
            RoleRootNode.Expanded = False

            LoadContainers(ContainerRootNode)
            LoadRootRoles(RoleRootNode)
            Exit Sub
        End If

        LoadFilteredRoles(filterKeyword)
        LoadFilteredContainers(filterKeyword)
    End Sub
    Private Sub LoadFilteredContainers(searchKeyword As String)
        Dim filteredContainerList As IList(Of Container) = Facade.ContainerFacade.GetOrderedContainersByName("Name ASC", searchKeyword)
        ContainerRootNode.Nodes().Clear()

        Dim nodeImageUrl As String = "../App_Themes/DocSuite2008/imgset16/box_open.png"
        For Each container As Container In filteredContainerList
            Dim containerTreeNode As RadTreeNode = CreateTreeNode(container.Name, container.Id.ToString(), nodeImageUrl, expandable:=False)
            containerTreeNode.Attributes.Add(ATTRNAME_NODETYPE, ContainerTreeNodeType.Container.ToString())
            containerTreeNode.Font.Bold = True
            ContainerRootNode.Nodes().Add(containerTreeNode)

            LoadSecurityGroups(containerTreeNode)

            If containerTreeNode.Nodes().Count > 0 Then
                containerTreeNode.Expanded = True
            End If
        Next

        ContainerRootNode.Expanded = True
    End Sub
    Private Sub LoadFilteredRoles(searchKeyword As String)
        Dim filteredRoleList As IList(Of Role) = FindRoles(roleNameFilter:=searchKeyword)
        RoleRootNode.Nodes().Clear()

        For Each role As Role In filteredRoleList
            Dim insertedRoleTree As RadTreeNode = RenderRoleRecursive(role)
            insertedRoleTree.Font.Bold = True
            insertedRoleTree.Attributes.Add(ATTRNAME_NODETYPE, ContainerTreeNodeType.Role.ToString())
            LoadSecurityGroups(insertedRoleTree)

            If insertedRoleTree.Nodes().Count > 0 Then
                insertedRoleTree.Expanded = True
            End If
        Next
        RoleRootNode.Expanded = True
    End Sub
    Private Function RenderRoleRecursive(role As Role) As RadTreeNode
        Dim currentRoleFather As Role = role.Father
        Dim nodeImageUrl As String = If(currentRoleFather Is Nothing, "../App_Themes/DocSuite2008/imgset16/bricks.png", "../App_Themes/DocSuite2008/imgset16/brick.png")
        Dim currentRoleTreeNode As RadTreeNode = CreateTreeNode(role.FullDescription, role.Id.ToString(), nodeImageUrl, expandable:=False)

        If currentRoleFather IsNot Nothing Then
            Dim allRolesNodes As List(Of RadTreeNode) = CType(RoleRootNode.GetAllNodes(), List(Of RadTreeNode))
            Dim parentNodeFromTree As RadTreeNode = allRolesNodes.Find(Function(node) node.Value = currentRoleFather.Id.ToString())

            If parentNodeFromTree IsNot Nothing Then
                parentNodeFromTree.Nodes().Add(currentRoleTreeNode)
                parentNodeFromTree.Expanded = True
            Else
                Dim lastInsertedParent As RadTreeNode = RenderRoleRecursive(currentRoleFather)
                lastInsertedParent.Expanded = True
                lastInsertedParent.Nodes().Add(currentRoleTreeNode)
            End If
        Else
            RoleRootNode.Nodes().Add(currentRoleTreeNode)
        End If

        Return currentRoleTreeNode
    End Function
    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(RadTreeGroups, pnlGroupDetails, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(SearchToolbar, RadTreeGroups, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub
    Private Function CreateTreeNode(nodeText As String, nodeValue As String, Optional nodeImage As String = "", Optional checkable As Boolean = False, Optional expandable As Boolean = True) As RadTreeNode
        Dim node As New RadTreeNode
        node.Text = nodeText
        node.Value = nodeValue
        node.Checkable = checkable

        If expandable Then
            node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack
        End If

        If Not String.IsNullOrEmpty(nodeImage) Then
            node.ImageUrl = nodeImage
        End If

        Return node
    End Function
    Private Function CheckIfCurrentUserIsInGroup(group As SecurityGroups) As Boolean
        For Each user As SecurityUsers In group.SecurityUsers
            If user.DisplayName = CurrentUser.GetFullUserName Then
                Return True
            End If
        Next
        Return False
    End Function
    Private Sub RefreshGroupUsersPanel()
        Dim selectedNode As RadTreeNode = RadTreeGroups.SelectedNode

        groupDetails.SelectedGroupId = CInt(selectedNode.Value)
        groupDetails.LoadOnlyUsers()
    End Sub
    Private Sub MakeNodesCheckboxReadonly(radTreeNodes As List(Of RadTreeNode))
        For Each node As RadTreeNode In radTreeNodes
            node.Attributes.Add(ATTRNAME_DEFAULTCHECKED, True.ToString())
        Next
    End Sub
    Private Function DecorateFinder(expandedNode As RadTreeNode) As Boolean
        Dim expandedNodeValue As Integer = CInt(expandedNode.Value)
        Dim expandedNodeType As String = expandedNode.Attributes(ATTRNAME_NODETYPE)

        Select Case expandedNodeType
            Case ContainerTreeNodeType.Container.ToString()
                CurrentFinder.OnContainerEnabled = expandedNodeValue
            Case ContainerTreeNodeType.Role.ToString()
                CurrentFinder.OnRoleEnabled = expandedNodeValue
        End Select
        Return False
    End Function

#End Region

End Class