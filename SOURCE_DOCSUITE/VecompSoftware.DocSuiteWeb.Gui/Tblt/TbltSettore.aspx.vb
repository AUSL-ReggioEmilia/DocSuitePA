Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Entity.Tenants

Partial Class TbltSettore
    Inherits CommonBasePage

#Region "Fields"
    Private Const CREATE_OPTION As String = "create"
    Private Const MODIFY_OPTION As String = "modify"
    Private Const DELETE_OPTION As String = "delete"
    Private Const MOVE_OPTION As String = "move"
    Private Const CLONE_OPTION As String = "clone"
    Private Const PRINT_OPTION As String = "print"
    Private Const GROUPS_OPTION As String = "groups"
    Private Const LOG_OPTION As String = "log"
    Private Const FUNCTION_OPTION As String = "function"
    Private Const PROPAGATION_OPTION As String = "propagation"
#End Region

#Region " Properties "

    Public Property SelectedGroupId As Integer
        Get
            If ViewState("SelectedGroupId") IsNot Nothing Then
                Return DirectCast(ViewState("SelectedGroupId"), Integer)
            End If
            Return Nothing
        End Get
        Set(value As Integer)
            ViewState("SelectedGroupId") = value
        End Set
    End Property

    Private ReadOnly Property SelectedGroup As SecurityGroups
        Get
            Return Facade.SecurityGroupsFacade.GetById(SelectedGroupId)
        End Get
    End Property
    Public ReadOnly Property RoleDescriptionTextBox As RadTextBox
        Get
            Dim toolBarItem As RadToolBarItem = ToolBarSearch.FindItemByValue("searchDescription")
            Return DirectCast(toolBarItem.FindControl("txtRoleDescription"), RadTextBox)
        End Get
    End Property

    Public ReadOnly Property SearchDisabled As Boolean
        Get
            Dim toolBarItem As RadToolBarItem = ToolBarSearch.FindItemByValue("searchDisabled")
            Return DirectCast(toolBarItem, RadToolBarButton).Checked
        End Get
    End Property

    Public ReadOnly Property SearchActive As Boolean
        Get
            Dim toolBarItem As RadToolBarItem = ToolBarSearch.FindItemByValue("searchActive")
            Return DirectCast(toolBarItem, RadToolBarButton).Checked
        End Get
    End Property

    Public ReadOnly Property CurrentSearchActiveValue As Boolean?
        Get
            Dim isActive As Boolean? = Nothing
            If (SearchActive Xor SearchDisabled) Then
                isActive = SearchActive AndAlso Not SearchDisabled
            End If
            Return isActive
        End Get
    End Property

    Private Property RoleRightModel As IList(Of RoleGroupRight)
        Get
            If ViewState("RoleRightModels") IsNot Nothing Then
                Return DirectCast(ViewState("RoleRightModels"), IList(Of RoleGroupRight))
            End If
            Return New List(Of RoleGroupRight)
        End Get
        Set(value As IList(Of RoleGroupRight))
            ViewState("RoleRightModels") = value
        End Set
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        If Not (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupTblRoleRight OrElse CommonShared.HasGroupTblRoleAdminRight) Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If

        InitializeAjax()
            InitializeControls()

            If Not IsPostBack Then
                RadTreeViewRoles.Nodes(0).Selected = True
                InitializeButtons()
                pnlDetail.Visible = False
                pnlCollaboration.Visible = False
            End If
    End Sub

    Protected Sub ToolBarSearch_ButtonClick(sender As Object, e As RadToolBarEventArgs)
        LoadRoles()
    End Sub

    Protected Sub PageAjaxRequest(ByVal seneder As Object, ByVal e As AjaxRequestEventArgs)
        Dim currentArguments As String() = e.Argument.Split("|"c)

        If currentArguments(0).Eq("move") Then
            Dim destinationId As Short? = Nothing
            Dim tmpId As Short = Nothing
            If Short.TryParse(currentArguments(1), tmpId) Then
                destinationId = tmpId
            End If
            MoveRole(destinationId)
            LoadRoles()
            pnlDetail.Visible = False
            pnlCollaboration.Visible = False
            AjaxManager.ResponseScripts.Add("tbltSettore.disableButtons();")
            Exit Sub
        End If

        If currentArguments(0).Eq("AddUser") Then
            Dim adUser As AccountModel = JsonConvert.DeserializeObject(Of AccountModel)(currentArguments(1))
            Dim group As SecurityGroups = Facade.SecurityGroupsFacade.GetGroupByName(adUser.RelatedGroupName)
            If group Is Nothing OrElse Facade.SecurityUsersFacade.IsUserInGroup(group, adUser.GetFullUserName()) Then
                AjaxAlert("L'utente {0} è già presente nel gruppo {1} o il gruppo non esiste", adUser.Account, adUser.RelatedGroupName)
            End If
            Facade.SecurityUsersFacade.Insert(adUser.Domain, adUser.Account, adUser.Name, group)

            If Not String.IsNullOrEmpty(RadTreeViewRoles.SelectedNode.Value) Then
                RoleRightModel = GetGroupRolesRight(Integer.Parse(RadTreeViewRoles.SelectedNode.Value))
                grdGroups.DataSource = RoleRightModel
                grdGroups.DataBind()
                For Each item As GridItem In grdGroups.MasterTableView.Items
                    item.Expanded = True
                Next
            End If
        End If

        If currentArguments(0).Eq("showcollaboration") Then
            Me.pnlDetail.Visible = False
            'Me.gridInformations.Visible = False
            Me.pnlCollaboration.Visible = True

            Dim idRole As Integer
            If Integer.TryParse(RadTreeViewRoles.SelectedNode.Value, idRole) Then
                UscCollRoles1.RoleId = idRole
                UscCollRoles1.BindDdlDSWEnvironment()
                UscCollRoles1.LoadUserRoles()
            End If
        End If

        If String.Compare(currentArguments(0), "update", StringComparison.OrdinalIgnoreCase) <> 0 Then
            Exit Sub
        End If


        Dim commandType As String = "update"
        If currentArguments.Length > 1 Then
            commandType = currentArguments(1).ToLowerInvariant
        End If

        Dim node As New RadTreeNode
        Select Case commandType
            Case "add"
                Dim role As Role = Facade.RoleFacade.GetById(Integer.Parse(currentArguments(2)))
                node = RecursiveAddFather(Nothing, role)
            Case "delete"
                node = RadTreeViewRoles.FindNodeByValue(currentArguments(2))
                Dim searchDisabledButton As RadToolBarButton = DirectCast(ToolBarSearch.FindItemByValue("searchDisabled"), RadToolBarButton)
                Dim searchActiveButton As RadToolBarButton = DirectCast(ToolBarSearch.FindItemByValue("searchActive"), RadToolBarButton)
                If searchDisabledButton.Checked Then
                    RecursiveMarkChildren(node)
                Else
                    node.Remove()
                End If
                node.Attributes("Recovery") = "true"
                node = Nothing
            Case "update", "recovery", "rename"
                node = RadTreeViewRoles.SelectedNode
                Dim role As Role = Facade.RoleFacade.GetById(Integer.Parse(node.Value))
                lblName.Text = role.Name
                lblMailAddress.Text = role.EMailAddress
                lblPecMailAddress.Text = String.Empty
                If role.Mailboxes IsNot Nothing AndAlso role.Mailboxes.Count > 0 Then
                    For Each mailbox As PECMailBox In role.Mailboxes
                        Dim isMailBoxDefault As Boolean = mailbox.MailBoxRoles.Any(Function(s) s.Id.RoleId.Equals(role.Id) AndAlso s.Priority)
                        Dim cellText As String = String.Format("{0}{1}", mailbox.MailBoxName, If(isMailBoxDefault, "[casella predefinita]", String.Empty))
                        lblPecMailAddress.Text = String.Concat(lblPecMailAddress.Text, If(String.IsNullOrEmpty(lblPecMailAddress.Text), cellText, String.Concat("<br />", cellText)))
                    Next
                End If
                LoadRoles()
                If commandType = "recovery" Then
                    RecursiveMarkFather(node)
                End If
                AjaxManager.ResponseScripts.Add("tbltSettore.hideLoadingSpinner();")

                AjaxManager.ResponseScripts.Add(String.Format("tbltSettore.selectNode(""{0}"");", node.Value))
        End Select

        If node Is Nothing Then
            node = RadTreeViewRoles.Nodes(0)
            pnlDetail.Visible = False
            pnlCollaboration.Visible = False
        End If

        node.Selected = True
        SelectNode(node)

    End Sub

    Private Sub RadTreeViewRolesNodeClick(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadTreeNodeEventArgs) Handles RadTreeViewRoles.NodeClick
        If e.Node.Attributes("NodeType").Eq("Root") Then
            pnlDetail.Visible = False
            pnlCollaboration.Visible = False
            InitializeButtons()
            Exit Sub
        End If

        If e.Node.Nodes.Count = 0 Then
            SelectNode(e.Node)
        End If

        pnlDetail.Visible = True
        pnlCollaboration.Visible = False

        Dim roleSelected As Role = Facade.RoleFacade.GetById(Integer.Parse(e.Node.Value))
        lblMailAddress.Text = roleSelected.EMailAddress

        lblPecMailAddress.Text = String.Empty

        If roleSelected.Mailboxes IsNot Nothing AndAlso roleSelected.Mailboxes.Count > 0 Then
            For Each mailbox As PECMailBox In roleSelected.Mailboxes
                Dim isMailBoxDefault As Boolean = mailbox.MailBoxRoles.Any(Function(s) s.Id.RoleId.Equals(roleSelected.Id) AndAlso s.Priority)
                Dim cellText As String = String.Format("{0}{1}", mailbox.MailBoxName, If(isMailBoxDefault, "[casella predefinita]", String.Empty))
                lblPecMailAddress.Text = String.Concat(lblPecMailAddress.Text, If(String.IsNullOrEmpty(lblPecMailAddress.Text), cellText, String.Concat("<br />", cellText)))
            Next
        End If

        InitializeButtons()
        If Not roleSelected.IsActive Then
            HideButtons()
        End If

        lblName.Text = $"{roleSelected.Name} ({roleSelected.Id} - {roleSelected.UniqueId})"

        RoleRightModel = GetGroupRolesRight(Integer.Parse(e.Node.Value))
        grdGroups.DataSource = RoleRightModel
        grdGroups.DataBind()

        uscRoleUsers.RoleId = Integer.Parse(e.Node.Value)
        uscRoleUsers.BindDdlDSWEnvironment()
        uscRoleUsers.LoadUserRoles()

        FolderToolBar.FindItemByValue("delete").Enabled = True
    End Sub

    Protected Sub grdGroups_DataBind(sender As Object, e As GridItemEventArgs) Handles grdGroups.ItemDataBound
        If TypeOf e.Item Is GridGroupHeaderItem Then
            Dim item As GridGroupHeaderItem = DirectCast(e.Item, GridGroupHeaderItem)
            Dim myArr As String() = item.DataCell.Text.Split(":"c)
            item.DataCell.Text = myArr(1).Trim()
        End If

        If (TypeOf e.Item Is GridDataItem AndAlso e.Item.OwnerTableView.Name = "UsersGrid") Then
            Dim boundHeader As RoleUserRight = DirectCast(e.Item.DataItem, RoleUserRight)
            Dim imageUserControl As Image = DirectCast(e.Item.FindControl("imgUser"), Image)
            Dim buttonAddUserControl As ImageButton = DirectCast(e.Item.FindControl("btnAddUser"), ImageButton)
            buttonAddUserControl.Visible = False
            If String.IsNullOrEmpty(boundHeader.Name) Then
                buttonAddUserControl.Visible = True
                imageUserControl.Visible = False
                buttonAddUserControl.OnClientClick = String.Concat("OpenEditWindowUsers('windowAddUsers','", boundHeader.GroupName, "');")
            End If
        End If
    End Sub

    Protected Sub grdGroups_DetailTableDataBind(source As Object, e As GridDetailTableDataBindEventArgs) Handles grdGroups.DetailTableDataBind
        Dim dataItem As GridDataItem = DirectCast(e.DetailTableView.ParentItem, GridDataItem)
        Dim groupName As String = dataItem.GetDataKeyValue("GroupName").ToString()
        Dim users As IList(Of RoleUserRight) = New List(Of RoleUserRight)
        Dim filteredModel As RoleGroupRight = RoleRightModel.Where(Function(x) x.GroupName.Eq(groupName)).FirstOrDefault()
        If filteredModel IsNot Nothing Then
            users = filteredModel.Users
        End If
        e.DetailTableView.DataSource = users
    End Sub
#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler MasterDocSuite.AjaxManager.AjaxRequest, AddressOf PageAjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(RadTreeViewRoles, FolderToolBar)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, FolderToolBar)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, RadTreeViewRoles)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlDetail)
        AjaxManager.AjaxSettings.AddAjaxSetting(pnlCollaboration, pnlCollaboration)
        AjaxManager.AjaxSettings.AddAjaxSetting(RadTreeViewRoles, FolderToolBar, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(RadTreeViewRoles, FolderToolBar, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(grdGroups, grdGroups, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearchUser, pnlDetail, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(FolderToolBar, pnlDetail, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ToolBarSearch, RadTreeViewRoles, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ToolBarSearch, RadTreeViewRoles, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(RadTreeViewRoles, pnlDetail, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(RadTreeViewRoles, pnlCollaboration, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(FolderToolBar, pnlCollaboration, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub InitializeControls()
        ' Menu contestuale
        If Not CommonShared.HasGroupAdministratorRight AndAlso Not CommonShared.HasGroupTblRoleAdminRight Then
            Exit Sub
        End If
        RadTreeViewRoles.OnClientContextMenuItemClicked = "OnContextMenuItemClicked"
        RadTreeViewRoles.OnClientContextMenuShowing = "OnContextMenuShowing"
    End Sub

    Private Sub InitializeButtons()
        If Not CommonShared.HasGroupAdministratorRight AndAlso Not CommonShared.HasGroupTblRoleAdminRight Then
            FolderToolBar.FindItemByValue(CREATE_OPTION).Visible = False
            FolderToolBar.FindItemByValue(DELETE_OPTION).Visible = False
            FolderToolBar.FindItemByValue(MODIFY_OPTION).Visible = False
            FolderToolBar.FindItemByValue(MOVE_OPTION).Visible = False
            FolderToolBar.FindItemByValue(PRINT_OPTION).Visible = False
            FolderToolBar.FindItemByValue(GROUPS_OPTION).Visible = False
            FolderToolBar.FindItemByValue(LOG_OPTION).Visible = False
            FolderToolBar.FindItemByValue(FUNCTION_OPTION).Visible = False
            FolderToolBar.FindItemByValue(PROPAGATION_OPTION).Visible = False
            FolderToolBar.FindItemByValue(CLONE_OPTION).Visible = False
            Exit Sub
        End If

        FolderToolBar.FindItemByValue(LOG_OPTION).Visible = DocSuiteContext.Current.ProtocolEnv.IsTableLogEnabled AndAlso (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupTblRoleAdminRight)
        FolderToolBar.FindItemByValue(FUNCTION_OPTION).Visible = True
        FolderToolBar.FindItemByValue(PROPAGATION_OPTION).Visible = DocSuiteContext.Current.ProtocolEnv.HierarchicalCollaboration AndAlso DocSuiteContext.Current.ProtocolEnv.MassivePropageCollaborationEnabled
        FolderToolBar.FindItemByValue(CREATE_OPTION).Enabled = False
        FolderToolBar.FindItemByValue(DELETE_OPTION).Enabled = False
        FolderToolBar.FindItemByValue(MODIFY_OPTION).Enabled = False
        FolderToolBar.FindItemByValue(MOVE_OPTION).Enabled = False
        FolderToolBar.FindItemByValue(PRINT_OPTION).Enabled = False
        FolderToolBar.FindItemByValue(GROUPS_OPTION).Enabled = False
        FolderToolBar.FindItemByValue(LOG_OPTION).Enabled = False
        FolderToolBar.FindItemByValue(CLONE_OPTION).Enabled = False

        Dim node As RadTreeNode = RadTreeViewRoles.SelectedNode
        If node Is Nothing Then
            Exit Sub
        End If

        'Root
        If node.Equals(RadTreeViewRoles.Nodes(0)) Then
            FolderToolBar.FindItemByValue(CREATE_OPTION).Enabled = True
            FolderToolBar.FindItemByValue(MODIFY_OPTION).Enabled = False
            FolderToolBar.FindItemByValue(DELETE_OPTION).Enabled = False
            FolderToolBar.FindItemByValue(MOVE_OPTION).Enabled = False
            FolderToolBar.FindItemByValue(PRINT_OPTION).Enabled = False
            FolderToolBar.FindItemByValue(GROUPS_OPTION).Enabled = False
            FolderToolBar.FindItemByValue(LOG_OPTION).Enabled = False
            FolderToolBar.FindItemByValue(FUNCTION_OPTION).Enabled = False
            FolderToolBar.FindItemByValue(CLONE_OPTION).Enabled = False
            Exit Sub
        End If

        Dim recovery As String = node.Attributes("Recovery")
        Select Case recovery
            Case "true"
                FolderToolBar.FindItemByValue(DELETE_OPTION).Text = "Recupera"
            Case Else
                FolderToolBar.FindItemByValue(DELETE_OPTION).Text = "Elimina"
        End Select
        Select Case node.Attributes("NodeType").ToUpper()
            Case "ROLE", "SUBROLE"
                FolderToolBar.FindItemByValue(CREATE_OPTION).Enabled = True
                FolderToolBar.FindItemByValue(MODIFY_OPTION).Enabled = True
                FolderToolBar.FindItemByValue(DELETE_OPTION).Enabled = True
                FolderToolBar.FindItemByValue(MOVE_OPTION).Enabled = True
                FolderToolBar.FindItemByValue(PRINT_OPTION).Enabled = True
                FolderToolBar.FindItemByValue(LOG_OPTION).Enabled = True
                FolderToolBar.FindItemByValue(GROUPS_OPTION).Enabled = True
                FolderToolBar.FindItemByValue(FUNCTION_OPTION).Enabled = True
                FolderToolBar.FindItemByValue(CLONE_OPTION).Enabled = True

            Case "GROUP"
                FolderToolBar.FindItemByValue(CREATE_OPTION).Enabled = False
                FolderToolBar.FindItemByValue(MODIFY_OPTION).Enabled = False
                FolderToolBar.FindItemByValue(DELETE_OPTION).Enabled = False
                FolderToolBar.FindItemByValue(MOVE_OPTION).Enabled = False
                FolderToolBar.FindItemByValue(PRINT_OPTION).Enabled = False
                FolderToolBar.FindItemByValue(LOG_OPTION).Enabled = True
                FolderToolBar.FindItemByValue(GROUPS_OPTION).Enabled = False
                FolderToolBar.FindItemByValue(FUNCTION_OPTION).Enabled = False
                FolderToolBar.FindItemByValue(CLONE_OPTION).Enabled = False
        End Select
    End Sub

    Private Sub HideButtons()
        FolderToolBar.FindItemByValue(CREATE_OPTION).Enabled = False
        FolderToolBar.FindItemByValue(MODIFY_OPTION).Enabled = ProtocolEnv.ManageDisableItemsEnabled
        FolderToolBar.FindItemByValue(DELETE_OPTION).Enabled = True
        FolderToolBar.FindItemByValue(MOVE_OPTION).Enabled = ProtocolEnv.ManageDisableItemsEnabled
        FolderToolBar.FindItemByValue(PRINT_OPTION).Enabled = False
        FolderToolBar.FindItemByValue(GROUPS_OPTION).Enabled = False
        FolderToolBar.FindItemByValue(LOG_OPTION).Enabled = False
        FolderToolBar.FindItemByValue(FUNCTION_OPTION).Enabled = False
        FolderToolBar.FindItemByValue(CLONE_OPTION).Enabled = False
        Exit Sub
    End Sub

    Protected Sub FolderToolBar_ButtonClick(sender As Object, e As RadToolBarEventArgs) Handles FolderToolBar.ButtonClick
        Select Case e.Item.Value
            Case CREATE_OPTION
                AjaxManager.ResponseScripts.Add("OpenEditWindow('windowEditRoles','Add');")
            Case DELETE_OPTION
                AjaxManager.ResponseScripts.Add("OpenEditWindow('windowEditRoles','Delete');")
            Case MODIFY_OPTION
                AjaxManager.ResponseScripts.Add("OpenEditWindow('windowEditRoles','Rename');")
            Case MOVE_OPTION
                AjaxManager.ResponseScripts.Add("OpenRolesWindow();")
            Case CLONE_OPTION
                AjaxManager.ResponseScripts.Add("OpenEditWindow('windowEditRoles','Clone');")
            Case PRINT_OPTION
                AjaxManager.ResponseScripts.Add("OpenPrintWindow('windowPrintRoles');")
            Case GROUPS_OPTION
                AjaxManager.ResponseScripts.Add("OpenGroupsWindow();")
            Case LOG_OPTION
                AjaxManager.ResponseScripts.Add("OpenLogWindow('windowLogRoles');")
            Case FUNCTION_OPTION
                Me.pnlDetail.Visible = False
                'Me.gridInformations.Visible = False
                Me.pnlCollaboration.Visible = True

                Dim idRole As Integer
                If Integer.TryParse(RadTreeViewRoles.SelectedNode.Value, idRole) Then
                    UscCollRoles1.RoleId = idRole
                    UscCollRoles1.BindDdlDSWEnvironment()
                    UscCollRoles1.LoadUserRoles()
                End If
            Case PROPAGATION_OPTION
                AjaxManager.ResponseScripts.Add("OpenPropagationWindow('windowPropagation');")
        End Select
    End Sub

    Public Enum AuthorizationEnum
        Protocol = 1
        Document = 2
        Repository = 3
        DocumentSeries = 4
    End Enum

    Protected Sub btnSearchUser_OnClick(sender As Object, e As EventArgs) Handles btnSearchUser.Click
        RoleRightModel = GetGroupRolesRight(Integer.Parse(RadTreeViewRoles.SelectedNode.Value))
        If Not String.IsNullOrEmpty(txtSearchUser.Text) Then
            RoleRightModel = RoleRightModel.Where(Function(x) x.Users.Any()).ToList()
        End If
        grdGroups.DataSource = RoleRightModel
        grdGroups.MasterTableView.HierarchyDefaultExpanded = Not String.IsNullOrEmpty(txtSearchUser.Text)
        grdGroups.Rebind()
    End Sub

    Private Function GetGroupRolesRight(roleId As Integer) As IList(Of RoleGroupRight)
        Dim models As IList(Of RoleGroupRight) = New List(Of RoleGroupRight)
        Dim roleSelected As Role = Facade.RoleFacade.GetById(roleId)
        Dim modelToAdd As RoleGroupRight
        Dim Items As Array
        For Each roleGroup As RoleGroup In roleSelected.RoleGroups
            Items = System.Enum.GetValues(GetType(AuthorizationEnum))
            For Each location As Integer In Items
                modelToAdd = CreateRoleGroupRight(roleGroup, roleGroup.Role.Id, location)

                If Not String.IsNullOrEmpty(modelToAdd.Authorization) Then
                    models.Add(modelToAdd)
                Else
                    If location = 1 And modelToAdd.Authorization = String.Empty And Not models.Any(Function(model) model.GroupName = modelToAdd.GroupName) Then
                        models.Add(modelToAdd)
                    End If
                End If
                If models.Any(Function(model) model.GroupName = modelToAdd.GroupName And model.Authorization = String.Empty And Not modelToAdd.Location = "Protocollo") Then
                    Dim modelToDelete As RoleGroupRight = models.Where(Function(model) model.GroupName = modelToAdd.GroupName And model.Authorization = String.Empty).First()
                    models.Remove(modelToDelete)
                End If
            Next
        Next
        Return models
    End Function

    Private Function CreateRoleGroupRight(roleGroup As RoleGroup, idRole As Integer, locations As Integer) As RoleGroupRight
        Dim dto As RoleGroupRight = New RoleGroupRight(idRole)
        dto.IdRole = idRole
        dto.GroupName = roleGroup.Name
        Dim userRights As IList(Of RoleUserRight) = New List(Of RoleUserRight)
        Dim users As IList(Of String) = New List(Of String)

        users = roleGroup.SecurityGroup.SecurityUsers.OrderBy(Function(x) x.Description) _
                .Select(Function(f) GetLabel(f)).ToList()

        If Not String.IsNullOrEmpty(txtSearchUser.Text) Then
            users = users.Where(Function(x) x.ContainsIgnoreCase(txtSearchUser.Text)).ToList()
        End If

        For Each user As String In users
            Dim model As RoleUserRight = New RoleUserRight()
            model.Name = user
            model.GroupName = roleGroup.Name
            userRights.Add(model)
        Next

        userRights.Add(New RoleUserRight() With {.Name = String.Empty, .GroupName = roleGroup.Name})

        Dim docRights As String = String.Empty

        If locations = 1 Then
            dto.Location = "Protocollo"
            If CommonInstance.ProtEnabled Then
                docRights = String.Empty
                If roleGroup.ProtocolRights.IsRoleEnabled Then
                    docRights = String.Concat(docRights, "Abilitato")
                End If
                If ProtocolEnv.IsDistributionEnabled AndAlso roleGroup.ProtocolRights.IsRoleManager Then
                    docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Manager", ", Manager"))
                End If
                If ProtocolEnv.IsPECEnabled AndAlso roleGroup.ProtocolRights.IsRolePEC Then
                    docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "PEC", ", PEC"))
                End If
                If ProtocolEnv.ProtocolBoxEnabled AndAlso ProtocolEnv.RoleGroupProcotolMailBoxRightEnabled AndAlso roleGroup.ProtocolRights.IsRoleProtocolMail Then
                    docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Casella di protocollazione", ", Casella di protocollazione"))
                End If
            End If
        End If
        If locations = 2 Then
            dto.Location = DocSuiteContext.Current.DossierAndPraticheLabel
            If CommonInstance.DocmEnabled Then
                If Diritti(roleGroup.DocumentRights, DossierRoleRightPositions.Enabled) Then
                    docRights = String.Concat(docRights, "Abilitato")
                End If
                If Diritti(roleGroup.DocumentRights, DossierRoleRightPositions.Workflow) Then
                    docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Workflow", ", Workflow"))
                End If
                If Diritti(roleGroup.DocumentRights, DossierRoleRightPositions.Manager) Then
                    docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Manager", ", Manager"))
                End If
            End If
        End If
        If locations = 3 Then
            dto.Location = "Atti"
            If CommonInstance.ReslEnabled Then
                docRights = String.Empty
                If Diritti(roleGroup.ResolutionRights, ResolutionRightPositions.Insert) Then
                    docRights = "Abilitato"
                End If
            End If
        End If
        If locations = 4 Then
            dto.Location = "SerieDocumentali"
            If ProtocolEnv.DocumentSeriesEnabled Then
                docRights = String.Empty
                If Diritti(roleGroup.DocumentSeriesRights, DocumentSeriesRoleRightPositions.Enabled) Then
                    docRights = "Abilitato"
                End If
            End If
        End If
        If docRights = String.Empty Then
            dto.Location = "Protocollo"
        End If
        dto.Authorization = docRights
        dto.Users = userRights
        Return dto
    End Function

    Private Function Diritti(ByVal field As String, ByVal right As Integer) As Boolean
        If String.IsNullOrEmpty(field) Then
            Return False
        End If
        Return field.Substring(right - 1, 1).Eq("1"c)
    End Function

    Private Function CheckDeletable(ByRef node As RadTreeNode) As Boolean
        For Each child As RadTreeNode In node.Nodes
            If child.Attributes("NodeType").Eq("SUBROLE") AndAlso child.Attributes("Recovery").Eq("false") Then
                Return False
            End If
        Next
        Return True
    End Function

    Private Sub SelectNode(ByRef node As RadTreeNode)
        Dim nodeType As String = node.Attributes("NodeType")
        InitializeButtons()
        Dim idRole As Integer
        If Not Integer.TryParse(node.Value, idRole) Then
            Exit Sub
        End If
    End Sub

    ''' <summary> Cancella l'albero corrente e lo ridisegna a seconda del filtro impostato. </summary>
    Private Sub LoadRoles()

        RadTreeViewRoles.Nodes(0).Nodes.Clear()
        Dim roleLists As IList(Of Role)
        If String.IsNullOrEmpty(RoleDescriptionTextBox.Text) Then
            roleLists = Facade.RoleFacade.GetRootItems(CurrentTenant.TenantAOO.UniqueId, isActive:=CurrentSearchActiveValue, withPECMailbox:=False)
            For Each role As Role In roleLists
                RecursiveAddChildren(Nothing, role)
            Next
        Else
            roleLists = Facade.RoleFacade.GetNoSecurityRoles(Env, RoleDescriptionTextBox.Text, CurrentTenant.TenantAOO.UniqueId, isActive:=CurrentSearchActiveValue)
            For Each role As Role In roleLists
                RecursiveAddFather(Nothing, role)
            Next
        End If
    End Sub

    ''' <summary> Crea un nodo, aggiunge settori e gruppi, lo imposta nel padre e continua finchè ci sono sottosettori </summary>
    ''' <param name="node">Nodo padre, se nullo lo mette al primo posto</param>
    Private Sub RecursiveAddChildren(ByRef node As RadTreeNode, ByRef role As Role)
        Dim nodeToAdd As New RadTreeNode()
        SetRoleNode(nodeToAdd, role, False)

        Dim existingNode As RadTreeNode = RadTreeViewRoles.FindNodeByValue(nodeToAdd.Value)
        If existingNode Is Nothing Then
            If node Is Nothing Then
                RadTreeViewRoles.Nodes(0).Nodes.Add(nodeToAdd)
            Else
                node.Nodes.Add(nodeToAdd)
            End If
        Else
            nodeToAdd = existingNode
        End If

        Dim children As IList(Of Role) = Facade.RoleFacade.GetItemsByParentId(role.Id, isActive:=CurrentSearchActiveValue)
        If children.Count > 0 Then
            For Each child As Role In children
                RecursiveAddChildren(nodeToAdd, child)
            Next
            nodeToAdd.Attributes.Add("HasChildren", "True")
        Else
            nodeToAdd.Attributes.Add("HasChildren", "False")
        End If
    End Sub

    Private Sub RecursiveMarkChildren(node As RadTreeNode)
        Dim children As IList(Of RadTreeNode) = node.GetAllNodes()
        If children.Count > 0 Then
            For Each child As RadTreeNode In children
                RecursiveMarkChildren(child)
            Next
        End If
        Dim nodeRole As Role = Facade.RoleFacade.GetById(Integer.Parse(node.Value))
        Facade.RoleFacade.DisableRole(nodeRole)
        node.Style.Add("color", "gray")
        node.Attributes.Add("Recovery", "true")
    End Sub

    Private Sub RecursiveMarkFather(node As RadTreeNode)
        Dim allNodes As IList(Of RadTreeNode) = RadTreeViewRoles.Nodes(0).GetAllNodes()
        For Each treeNode As RadTreeNode In allNodes
            If treeNode.Value = node.ParentNode.Value Then
                RecursiveMarkFather(treeNode)
                Dim fatherRole As Role = Facade.RoleFacade.GetById(Integer.Parse(treeNode.Value))
                Facade.RoleFacade.ActivateRole(fatherRole)
                treeNode.Style.Add("color", "black")
                treeNode.Attributes.Add("Recovery", "false")
            End If
        Next
    End Sub

    ''' <summary>
    ''' Se il settore non è presente nell'albero lo crea, aggiunge settori e gruppi, e lo associa al padre o lo mette in root
    ''' </summary>
    ''' <param name="node">Se non è nullo lo associa come nodo figlio del nodo creato</param>
    Private Function RecursiveAddFather(ByRef node As RadTreeNode, ByRef role As Role) As RadTreeNode
        Dim nodeToAdd As New RadTreeNode()
        If (RadTreeViewRoles.FindNodeByValue(role.Id.ToString()) Is Nothing) Then
            If role IsNot Nothing Then
                SetRoleNode(nodeToAdd, role, False)
                If role.Father Is Nothing Then 'Primo Livello
                    RadTreeViewRoles.Nodes(0).Nodes.Add(nodeToAdd)
                Else
                    Dim fatherNode As RadTreeNode = RadTreeViewRoles.FindNodeByValue(role.Father.Id.ToString())
                    If (fatherNode Is Nothing) Then
                        RecursiveAddFather(nodeToAdd, role.Father)
                    Else
                        fatherNode.Nodes.Add(nodeToAdd)
                    End If
                End If
                nodeToAdd.Expanded = True
            End If
            If node IsNot Nothing Then
                nodeToAdd.Nodes.Add(node)
            End If
        End If

        Dim children As IList(Of Role) = Facade.RoleFacade.GetItemsByParentId(role.Id)
        If children.Count > 0 Then
            nodeToAdd.Attributes.Add("HasChildren", "True")
        Else
            nodeToAdd.Attributes.Add("HasChildren", "False")
        End If

        Return nodeToAdd
    End Function

    ''' <summary> Valorizza un nodo col settore richiesto. </summary>
    Private Sub SetRoleNode(ByRef node As RadTreeNode, ByVal role As Role, ByVal expandCallBack As Boolean)
        ' Valori di base
        node.Text = role.FullDescription
        node.Value = role.Id.ToString()
        node.Attributes.Add("ID", role.Id.ToString())
        node.Attributes.Add("UniqueId", role.UniqueId.ToString())
        ' Imposto immagine di settore o sottosettore
        If (role.Father Is Nothing) Then
            If (expandCallBack) Then
                node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack
            End If
            node.ImageUrl = ImagePath.SmallRole
            node.Attributes.Add("NodeType", "Role")
        Else
            node.ImageUrl = ImagePath.SmallSubRole
            node.Attributes.Add("NodeType", "SubRole")
        End If
        ' Imposto il colore se è attivo
        If Not role.IsActive Then
            node.Style.Add("color", "gray")
            node.Attributes.Add("Recovery", "true")
        Else
            node.Style.Add("color", "black")
            node.Attributes.Add("Recovery", "false")
        End If
        node.Font.Bold = True
        node.Expanded = True
    End Sub

    Private Sub MoveRole(destinationId As Short?)
        Dim idRole As Integer
        If Integer.TryParse(RadTreeViewRoles.SelectedNode.Value, idRole) Then
            Dim role As Role = Facade.RoleFacade.GetById(idRole)
            Dim destination As Role = Nothing
            If destinationId.HasValue Then
                destination = Facade.RoleFacade.GetById(destinationId.Value)
            End If
            If destination Is Nothing OrElse (Not destination.FullIncrementalPath.StartsWith(role.FullIncrementalPath) AndAlso destination.Id <> role.Id) Then
                Facade.RoleFacade.Move(role, destination)
            End If
        End If
    End Sub

#End Region

End Class


