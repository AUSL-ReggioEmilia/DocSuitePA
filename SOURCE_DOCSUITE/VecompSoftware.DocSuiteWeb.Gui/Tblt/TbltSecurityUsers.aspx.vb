Imports System.Linq
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Desks
Imports Newtonsoft.Json

Public Class TbltSecurityUsers
    Inherits CommonBasePage


#Region " Fields "
    Private Const USERS_INITIALIZE_DETAILS_CALLBACK As String = "tbltSecurityUsers.initializeDetailsCallback('{0}');"
    Private Const INITIALIZE_SPECIAL_TOOLBAR_ACTION As String = "tbltSecurityUsers.initializeSpecialToolbarAction();"
#End Region

#Region " Properties "

    Public ReadOnly Property SearchAccountTextBox As RadTextBox
        Get
            Dim toolBarItem As RadToolBarItem = ToolBarSearch.FindItemByValue("searchAccount")
            Return DirectCast(toolBarItem.FindControl("txtAccount"), RadTextBox)
        End Get
    End Property

    Public ReadOnly Property DomainOptionsCombobox As RadDropDownList
        Get
            Dim toolBarItem As RadToolBarItem = ToolBarSearch.FindItemByValue("domainOptions")
            Return DirectCast(toolBarItem.FindControl("rcbDomain"), RadDropDownList)
        End Get
    End Property

    Public ReadOnly Property SelectedUser As RadTreeNode
        Get
            Return rtvUsers.SelectedNode()
        End Get
    End Property

    Private ReadOnly Property UserGroups As IList(Of SecurityGroups)
        Get
            Return Facade.SecurityGroupsFacade.GetByUser(SelectedUser.Attributes("Account"), SelectedUser.Attributes("Domain"))
        End Get
    End Property


#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub ToolBarSearch_ButtonClick(ByVal sender As Object, ByVal e As RadToolBarEventArgs) Handles ToolBarSearch.ButtonClick
        Dim btn As RadToolBarButton = TryCast(e.Item, RadToolBarButton)

        If btn.Value = "search" Then
            LoadUsers()
        Else
            LoadUsersFromUserlog()
        End If
    End Sub

    Protected Sub TbltSecurityUsers_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arguments As String() = Split(e.Argument, "|")
        Select Case arguments(0).ToLower()
            Case "copyfromuser"
                If Not String.IsNullOrEmpty(arguments(1)) Then
                    Dim asUser As AccountModel = JsonConvert.DeserializeObject(Of AccountModel)(arguments(1))
                    If asUser IsNot Nothing Then
                        For Each item As SecurityGroups In Facade.SecurityUsersFacade.GetGroupsByAccount(asUser.GetFullUserName())
                            If Not Facade.SecurityUsersFacade.IsUserInGroup(item, SelectedUser.Value) Then
                                Facade.SecurityUsersFacade.Insert(SelectedUser.Attributes("Domain"), SelectedUser.Attributes("Account"), SelectedUser.Attributes("DisplayName"), item)
                            End If
                        Next
                        RefreshDetails()
                    End If
                End If
                AjaxManager.ResponseScripts.Add(String.Format(USERS_INITIALIZE_DETAILS_CALLBACK, String.Empty))
            Case "groups"
                If Not String.IsNullOrEmpty(arguments(1)) Then
                    Dim errorGroups As List(Of String) = New List(Of String)()
                    Dim groupNames As String() = arguments(1).Split(";"c)
                    If SelectedUser Is Nothing Then
                        AjaxAlert("Utente non selezionato.")
                        Exit Sub
                    End If

                    For Each item As String In groupNames
                        If Not String.IsNullOrEmpty(item) Then
                            Dim group As SecurityGroups = Facade.SecurityGroupsFacade.GetGroupByName(item)
                            If group IsNot Nothing AndAlso Facade.SecurityUsersFacade.IsUserInGroup(group, SelectedUser.Value) Then
                                errorGroups.Add(group.GroupName)
                                Continue For
                            End If
                            Facade.SecurityUsersFacade.Insert(SelectedUser.Attributes("Domain"), SelectedUser.Attributes("Account"), SelectedUser.Attributes("DisplayName"), group)
                        End If
                    Next
                    Dim errorMessage As String
                    If errorGroups.Count > 0 Then
                        errorMessage = String.Format("L'utente {0} è già presente nei gruppi: ", SelectedUser.Value)
                        For Each item As String In errorGroups
                            errorMessage = String.Concat(vbCrLf, errorMessage, item)
                        Next
                        AjaxAlert(errorMessage)
                        RefreshDetails()
                        Exit Sub
                    End If
                    RefreshDetails()
                End If
                AjaxManager.ResponseScripts.Add(String.Format(USERS_INITIALIZE_DETAILS_CALLBACK, String.Empty))
            Case "delete"
                If rtvGroups.CheckedNodes() Is Nothing OrElse rtvGroups.CheckedNodes().Count() = 0 Then
                    Exit Sub
                End If

                Dim selectedGroupIds As IList(Of Integer) = rtvGroups.CheckedNodes().Select(Function(s) Convert.ToInt32(s.Value)).ToList()
                If selectedGroupIds.Count() > 0 Then
                    RemoveUserFromGroups(selectedGroupIds)
                    RefreshDetails()
                End If

                AjaxManager.ResponseScripts.Add(String.Format(USERS_INITIALIZE_DETAILS_CALLBACK, String.Empty))
            Case "deleteuser"
                Dim allUserGroups As IList(Of SecurityGroups) = Facade.SecurityGroupsFacade.GetByUser(SelectedUser.Attributes("Account"), SelectedUser.Attributes("Domain"))

                If allUserGroups.Count() > 0 Then
                    RemoveUserFromGroups(allUserGroups.Select(Function(x) x.Id).ToList())
                    LoadUsers()
                    SetDetailsPanelVisibility(False)
                End If
            Case "users"
                Dim asUser As AccountModel = JsonConvert.DeserializeObject(Of AccountModel)(arguments(1))
                If Facade.SecurityUsersFacade.GetSecurityUsersCount(asUser.Account, asUser.Domain) > 0 Then
                    AjaxAlert("L'utente è già stato configurato.")
                    Exit Sub
                End If
                AjaxManager.ResponseScripts.Add(String.Format(USERS_INITIALIZE_DETAILS_CALLBACK, String.Empty))
                Response.Redirect(String.Format("~/Tblt/TbltSecurityGroupWizard.aspx?Type=Comm&DomainAD={0}&AccountAD={1}", asUser.Domain, asUser.Account))
        End Select
    End Sub


    Protected Sub RadTreeViewRolesNodeClick(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs) Handles rtvUsers.NodeClick
        If e.Node.Attributes("NodeType").Eq("Root") Then
            SetDetailsPanelVisibility(False)
            AjaxManager.ResponseScripts.Add(String.Format(USERS_INITIALIZE_DETAILS_CALLBACK, "Root"))
            Exit Sub
        End If

        RefreshDetails()
        AjaxManager.ResponseScripts.Add(String.Format(USERS_INITIALIZE_DETAILS_CALLBACK, String.Empty))
    End Sub

    Private Sub GrdContainers_DetailTableDataBind(ByVal source As Object, ByVal e As GridDetailTableDataBindEventArgs) Handles grdContainers.DetailTableDataBind
        Dim dataItem As GridDataItem = e.DetailTableView.ParentItem
        Dim containerRights As IList(Of ContainerGroupRight)

        Select Case e.DetailTableView.Name
            Case "Groups"
                Dim idContainer As Integer = Convert.ToInt32(dataItem.GetDataKeyValue("Id"))
                Dim containerGroups As IList(Of ContainerGroup) = Facade.ContainerGroupFacade.GetByContainerAndGroups(idContainer, UserGroups.Select(Function(g) g.GroupName).ToArray())
                containerRights = New List(Of ContainerGroupRight)()
                Dim dto As ContainerGroupRight
                For Each item As ContainerGroup In containerGroups
                    dto = New ContainerGroupRight()
                    dto.GroupName = item.Name
                    containerRights.Add(dto)
                Next
                e.DetailTableView.DataSource = containerRights
            Case "Rights"
                Dim dt As DataTable = New DataTable()
                Dim parentItem As GridDataItem = e.DetailTableView.ParentItem.OwnerTableView.ParentItem
                Dim groupName As String = dataItem.GetDataKeyValue("GroupName").ToString()
                Dim idContainer As Integer = Convert.ToInt32(parentItem.GetDataKeyValue("Id"))
                Dim currentContainerGroup As ContainerGroup = Facade.ContainerGroupFacade.GetByContainerAndName(idContainer, groupName)

                dt.Columns.Add("DocumentType")
                dt.Columns.Add("Rights")

                If CommonInstance.DocmEnabled AndAlso currentContainerGroup.Container.DocmLocation IsNot Nothing Then
                    BindDocmRightsDetailFor(currentContainerGroup, dt)
                End If

                If CommonInstance.ProtEnabled AndAlso currentContainerGroup.Container.ProtLocation IsNot Nothing Then
                    BindProtocolRightsDetailFor(currentContainerGroup, dt)
                End If

                If CommonInstance.ReslEnabled AndAlso currentContainerGroup.Container.ReslLocation IsNot Nothing Then
                    BindReslRightsDetailFor(currentContainerGroup, dt)
                End If

                If CommonInstance.ProtEnabled AndAlso
                    (currentContainerGroup.Container.DocumentSeriesLocation IsNot Nothing OrElse
                    currentContainerGroup.Container.DocumentSeriesAnnexedLocation IsNot Nothing OrElse
                    currentContainerGroup.Container.DocumentSeriesUnpublishedAnnexedLocation IsNot Nothing) AndAlso
                    currentContainerGroup.DocumentSeriesRights IsNot Nothing Then
                    BindDocumentSeriesDetailFor(currentContainerGroup, dt)
                End If

                If ProtocolEnv.DeskEnable AndAlso currentContainerGroup.Container.DeskLocation IsNot Nothing Then
                    BindDeskDetailFor(currentContainerGroup, dt)
                End If

                If currentContainerGroup.Container.UDSLocation IsNot Nothing AndAlso currentContainerGroup.Container.UDSLocation IsNot Nothing Then
                    BindUDSDetailFor(currentContainerGroup, dt)
                End If
                e.DetailTableView.DataSource = dt
        End Select

    End Sub

    Private Sub GrdRoles_DetailTableDataBind(ByVal source As Object, ByVal e As GridDetailTableDataBindEventArgs) Handles grdRoles.DetailTableDataBind
        Dim dataItem As GridDataItem = e.DetailTableView.ParentItem

        Dim roleRights As IList(Of RoleGroupRight)

        Select Case e.DetailTableView.Name
            Case "Groups"
                Dim idRole As Integer = Convert.ToInt32(dataItem.GetDataKeyValue("Id"))
                Dim roleGroups As IList(Of RoleGroup) = Facade.RoleGroupFacade.GetByRoleAndGroups(idRole, UserGroups.Select(Function(g) g.GroupName).ToArray())
                roleRights = New List(Of RoleGroupRight)()
                Dim dto As RoleGroupRight
                For Each item As RoleGroup In roleGroups
                    dto = New RoleGroupRight()
                    dto.GroupName = item.Name
                    roleRights.Add(dto)
                Next
                e.DetailTableView.DataSource = roleRights
            Case "Rights"
                Dim dt As DataTable = New DataTable()
                Dim parentItem As GridDataItem = e.DetailTableView.ParentItem.OwnerTableView.ParentItem
                Dim idRole As Integer = Convert.ToInt32(parentItem.GetDataKeyValue("Id"))
                Dim groupName As String = dataItem.GetDataKeyValue("GroupName").ToString()
                Dim roleGroup As RoleGroup = Facade.RoleGroupFacade.GetByRoleAndGroups(idRole, {groupName}).FirstOrDefault()

                If roleGroup Is Nothing Then
                    Exit Sub
                End If

                dt.Columns.Add("DocumentType")
                dt.Columns.Add("Rights")
                PrepareRoleGroupRights(roleGroup, dt)
                e.DetailTableView.DataSource = dt
        End Select

    End Sub

    Private Sub grdRoles_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles grdRoles.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If
        Dim item As Role = TryCast(e.Item.DataItem, Role)
        If item Is Nothing Then
            Exit Sub
        End If
        Dim row As GridDataItem = DirectCast(e.Item, GridDataItem)
        With DirectCast(e.Item.FindControl("Name"), Label)
            If item.IsActive <> 1 Then
                row.Item("Name").ForeColor = Drawing.Color.LightSlateGray
                row.Item("Name").ToolTip = "Settore disabilitato"
                Dim documentImage As Image = DirectCast(e.Item.FindControl("roleImage"), Image)
                documentImage.ImageUrl = "~/App_Themes/DocSuite2008/imgset16/brickEnabled.png"
            End If
        End With
    End Sub

#End Region

#Region " Methods "
    Private Sub Initialize()
        DomainOptionsCombobox.Items.Add(New DropDownListItem("Tutti i domini", ""))
        For Each domain As String In DocSuiteContext.Current.Tenants.Select(Function(t) t.DomainName)
            DomainOptionsCombobox.Items.Add(New DropDownListItem(domain, domain))
        Next
        btnPrivacy.Text = String.Concat("Assegna ", PRIVACY_LABEL)
        SetDetailsPanelVisibility(False)
    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(ToolBarSearch, rtvUsers, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(rtvUsers, pnlDetails, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, rtvUsers)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlDetails, MasterDocSuite.AjaxDefaultLoadingPanel)
        AddHandler AjaxManager.AjaxRequest, AddressOf TbltSecurityUsers_AjaxRequest
    End Sub

    Private Sub LoadUsers()
        rtvUsers.Nodes(0).Nodes.Clear()
        Dim users As IList(Of SecurityUsers) = Facade.SecurityUsersFacade _
        .GetUsersByAccountOrDescription(SearchAccountTextBox.Text, DomainOptionsCombobox.SelectedValue) _
        .ToList()
        For Each user As SecurityUsers In users
            Dim fullUserName As String = String.Concat(user.UserDomain, "\", user.Account)
            Dim newNode As RadTreeNode = CreateNode(String.Concat(fullUserName, " (", user.Description, ")"), fullUserName, "~/App_Themes/DocSuite2008/imgset16/user.png")
            newNode.Attributes.Add("Account", user.Account)
            newNode.Attributes.Add("Domain", user.UserDomain)
            newNode.Attributes.Add("DisplayName", user.Description)
            rtvUsers.Nodes(0).Nodes.Add(newNode)
        Next
    End Sub

    Private Sub LoadUsersFromUserlog()
        rtvUsers.Nodes(0).Nodes.Clear()
        Dim users As IList(Of UserLog) = Facade.UserLogFacade.GetUnconfiguredUsers().Distinct().ToList()
        For Each user As UserLog In users
            Dim fullUserName As String = user.Id
            Dim newNode As RadTreeNode = CreateNode(fullUserName, "~/App_Themes/DocSuite2008/imgset16/user.png", String.Empty)
            Dim token As String() = fullUserName.Split("\"c)
            newNode.Attributes.Add("Account", token.Last())
            newNode.Attributes.Add("Domain", token.First())
            newNode.Attributes.Add("DisplayName", fullUserName)
            rtvUsers.Nodes(0).Nodes.Add(newNode)
        Next
    End Sub

    Private Sub RefreshDetails()
        Dim groups As IList(Of SecurityGroups) = UserGroups
        LoadGroups(groups)
        LoadContainersGrid(groups)
        LoadRolesGrid(groups)
    End Sub

    Private Sub LoadGroups(groups As IList(Of SecurityGroups))
        rtvGroups.Nodes.Clear()
        If groups.Count = 0 Then
            rtvGroups.Nodes.Add(New RadTreeNode("Nessun gruppo definito"))
            Exit Sub
        End If

        SetDetailsPanelVisibility(True)
        For Each group As SecurityGroups In groups
            Dim newNode As RadTreeNode = CreateNode(group.GroupName, group.Id.ToString, String.Empty)
            rtvGroups.Nodes.Add(newNode)
        Next
    End Sub

    Private Function CreateNode(text As String, value As String, imageUrl As String) As RadTreeNode
        Dim newNode As RadTreeNode = New RadTreeNode()
        newNode.Text = text
        newNode.Value = value
        newNode.ImageUrl = imageUrl
        Return newNode
    End Function

    Private Sub LoadContainersGrid(groups As IList(Of SecurityGroups))
        grdContainers.DataSource = Facade.ContainerFacade.GetContainersBySecurity(groups)
        grdContainers.DataBind()
    End Sub

    Private Sub LoadRolesGrid(groups As IList(Of SecurityGroups))
        grdRoles.DataSource = Facade.RoleFacade.GetRolesBySecurityGroups(groups)
        grdRoles.DataBind()
    End Sub

    Private Sub RemoveUserFromGroups(groupsId As IList(Of Integer))
        Dim filteredGroupIds As IList(Of Integer) = New List(Of Integer)
        Dim adminGroupIds As IList(Of Integer) = DocSuiteContext.Current.ProtocolEnv.EnvGroupAdministrator.Split("|"c).Select(Function(f) Convert.ToInt32((f))).ToList()

        If SelectedUser.Value = DocSuiteContext.Current.User.FullUserName Then
            For Each groupId As Integer In groupsId
                If Not adminGroupIds.Contains(groupId) Then
                    filteredGroupIds.Add(groupId)
                End If
            Next
        Else
            filteredGroupIds = groupsId
        End If

        If filteredGroupIds.Count > 0 Then
            Dim users As IList(Of SecurityUsers) = Facade.SecurityUsersFacade.GetUsersByAccountAndGroups(SelectedUser.Attributes("Account"), SelectedUser.Attributes("Domain"), filteredGroupIds.ToArray()).ToList()
            For Each item As SecurityUsers In users
                Facade.SecurityUsersFacade.Delete(item)
            Next
        End If
    End Sub

    Private Sub SetDetailsPanelVisibility(visible As Boolean)
        pnlDetails.Visible = visible
    End Sub

    Private Function Diritti(ByVal field As String, ByVal right As Integer) As Boolean
        If String.IsNullOrEmpty(field) Then
            Return False
        End If

        Return field.Substring(right - 1, 1).Eq("1"c)
    End Function

    Private Function PrepareRoleGroupRights(roleGroup As RoleGroup, dt As DataTable) As DataTable
        Dim docType As String = String.Empty
        Dim docRights As String = String.Empty
        If CommonInstance.DocmEnabled Then
            docType = DocSuiteContext.Current.DossierAndPraticheLabel
            If (Diritti(roleGroup.DocumentRights, DossierRoleRightPositions.Enabled)) Then
                docRights = String.Concat(docRights, "Abilitato")
            End If
            If (Diritti(roleGroup.DocumentRights, DossierRoleRightPositions.Workflow)) Then
                docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Workflow", ", Workflow"))
            End If
            If (Diritti(roleGroup.DocumentRights, DossierRoleRightPositions.Manager)) Then
                docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Manager", ", Manager"))
            End If
            If Not String.IsNullOrEmpty(docRights) Then
                dt.Rows.Add(docType, docRights)
            End If
        End If

        If CommonInstance.ProtEnabled Then
            docType = "Protocollo"
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
            If Not String.IsNullOrEmpty(docRights) Then
                dt.Rows.Add(docType, docRights)
            End If
        End If

        If CommonInstance.ReslEnabled Then
            docType = Facade.TabMasterFacade.TreeViewCaption
            docRights = String.Empty
            If Diritti(roleGroup.ResolutionRights, ResolutionRightPositions.Insert) Then
                docRights = "Abilitato"
            End If
            If Not String.IsNullOrEmpty(docRights) Then
                dt.Rows.Add(docType, docRights)
            End If
        End If

        If ProtocolEnv.DocumentSeriesEnabled Then
            docType = ProtocolEnv.DocumentSeriesName
            docRights = String.Empty
            If Diritti(roleGroup.DocumentSeriesRights, DocumentSeriesRoleRightPositions.Enabled) Then
                docRights = "Abilitato"
            End If
            dt.Rows.Add(docType, docRights)
        End If
        Return dt
    End Function

    Private Sub BindDocmRightsDetailFor(currentContainerGroup As ContainerGroup, dt As DataTable)
        Dim docType As String = DocSuiteContext.Current.DossierAndPraticheLabel
        Dim docRights As String = String.Empty

        If Diritti(currentContainerGroup.DocumentRights, DocumentContainerRightPositions.Insert) Then
            docRights = String.Concat(docRights, "Inserimento")
        End If
        If Diritti(currentContainerGroup.DocumentRights, DocumentContainerRightPositions.Modify) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Modifica", ", Modifica"))
        End If
        If Diritti(currentContainerGroup.DocumentRights, DocumentContainerRightPositions.View) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Visualizzazione", ", Visualizzazione"))
        End If
        If Diritti(currentContainerGroup.DocumentRights, DocumentContainerRightPositions.Preview) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Sommario", ", Sommario"))
        End If
        If Diritti(currentContainerGroup.DocumentRights, DocumentContainerRightPositions.Cancel) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Cancellazione", ", Cancellazione"))
        End If
        If Diritti(currentContainerGroup.DocumentRights, DocumentContainerRightPositions.Workflow) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Workflow", ", Workflow"))
        End If
        AddPrivacyLevel(docRights, currentContainerGroup)
        dt.Rows.Add(docType, docRights)
    End Sub

    Private Sub BindProtocolRightsDetailFor(currentContainerGroup As ContainerGroup, ByRef dt As DataTable)
        Dim docType As String = "Protocollo"
        Dim docRights As String = String.Empty
        If Diritti(currentContainerGroup.ProtocolRightsString, ProtocolContainerRightPositions.Insert) Then
            docRights = String.Concat(docRights, "Inserimento")
        End If
        If Diritti(currentContainerGroup.ProtocolRightsString, ProtocolContainerRightPositions.Modify) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Modifica", ", Modifica"))
        End If
        If Diritti(currentContainerGroup.ProtocolRightsString, ProtocolContainerRightPositions.View) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Visualizzazione", ", Visualizzazione"))
        End If
        If Diritti(currentContainerGroup.ProtocolRightsString, ProtocolContainerRightPositions.Preview) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Sommario", ", Sommario"))
        End If
        If Diritti(currentContainerGroup.ProtocolRightsString, ProtocolContainerRightPositions.Cancel) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Cancellazione", ", Cancellazione"))
        End If
        If Diritti(currentContainerGroup.ProtocolRightsString, ProtocolContainerRightPositions.InteropIn) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Interop. Ingresso", ", Interop. Ingresso"))
        End If
        If Diritti(currentContainerGroup.ProtocolRightsString, ProtocolContainerRightPositions.InteropOut) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Interop. Uscita", ", Interop. Uscita"))
        End If
        If Diritti(currentContainerGroup.ProtocolRightsString, ProtocolContainerRightPositions.PECIn) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "PEC Ingresso", ", PEC Ingresso"))
        End If
        If Diritti(currentContainerGroup.ProtocolRightsString, ProtocolContainerRightPositions.PECOut) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "PEC Uscita", ", PEC Uscita"))
        End If
        If ProtocolEnv.IsDistributionEnabled AndAlso Diritti(currentContainerGroup.ProtocolRightsString, ProtocolContainerRightPositions.DocDistribution) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Distribuzione Doc.", ", Distribuzione Doc."))
        End If
        If Diritti(currentContainerGroup.ProtocolRightsString, ProtocolContainerRightPositions.Privacy) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Privacy", ", Privacy"))
        End If
        AddPrivacyLevel(docRights, currentContainerGroup)
        dt.Rows.Add(docType, docRights)
    End Sub

    Private Sub BindDocumentSeriesDetailFor(currentContainerGroup As ContainerGroup, ByRef dt As DataTable)
        Dim docType As String = ProtocolEnv.DocumentSeriesName
        Dim docRights As String = String.Empty
        If Diritti(currentContainerGroup.DocumentSeriesRights, DocumentSeriesContainerRightPositions.Insert) Then
            docRights = String.Concat(docRights, "Inserimento")
        End If
        If Diritti(currentContainerGroup.DocumentSeriesRights, DocumentSeriesContainerRightPositions.Modify) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Modifica", ", Modifica"))
        End If
        If Diritti(currentContainerGroup.DocumentSeriesRights, DocumentSeriesContainerRightPositions.View) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Visualizzazione", ", Visualizzazione"))
        End If
        If Diritti(currentContainerGroup.DocumentSeriesRights, DocumentSeriesContainerRightPositions.Preview) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Sommario", ", Sommario"))
        End If
        If Diritti(currentContainerGroup.DocumentSeriesRights, DocumentSeriesContainerRightPositions.Cancel) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Annullamento", ", Annullamento"))
        End If
        If Diritti(currentContainerGroup.DocumentSeriesRights, DocumentSeriesContainerRightPositions.Draft) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Bozze", ", Bozze"))
        End If
        If Diritti(currentContainerGroup.DocumentSeriesRights, DocumentSeriesContainerRightPositions.ViewCanceled) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Vis. annullate", ", Vis. annullate"))
        End If
        If Diritti(currentContainerGroup.DocumentSeriesRights, DocumentSeriesContainerRightPositions.Admin) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Amministrazione", ", Amministrazione"))
        End If
        AddPrivacyLevel(docRights, currentContainerGroup)
        dt.Rows.Add(docType, docRights)
    End Sub

    Private Sub BindUDSDetailFor(currentContainerGroup As ContainerGroup, dt As DataTable)
        Dim docType As String = "Archivi"
        Dim docRights As String = String.Empty
        If Diritti(currentContainerGroup.UDSRights, UDSRightPositions.Insert) Then
            docRights = String.Concat(docRights, "Inserimento")
        End If
        If Diritti(currentContainerGroup.UDSRights, UDSRightPositions.Modify) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Modifica", ", Modifica"))
        End If
        If Diritti(currentContainerGroup.UDSRights, UDSRightPositions.ViewDocuments) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Visualizzazione", ", Visualizzazione"))
        End If
        If Diritti(currentContainerGroup.UDSRights, UDSRightPositions.Read) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Sommario", ", Sommario"))
        End If
        If Diritti(currentContainerGroup.UDSRights, UDSRightPositions.Protocol) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Protocollazione", ", Protocollazione"))
        End If
        If Diritti(currentContainerGroup.UDSRights, UDSRightPositions.PECIngoing) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "PEC Ingresso", ", PEC Ingresso"))
        End If
        If Diritti(currentContainerGroup.UDSRights, UDSRightPositions.PECOutgoing) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "PEC Uscita", ", PEC Uscita"))
        End If
        If Diritti(currentContainerGroup.UDSRights, UDSRightPositions.Cancel) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Annullamento", ", Annullamento"))
        End If
        AddPrivacyLevel(docRights, currentContainerGroup)
        dt.Rows.Add(docType, docRights)
    End Sub

    Private Sub BindDeskDetailFor(currentContainerGroup As ContainerGroup, ByRef dt As DataTable)
        Dim docType As String = "Tavoli"
        Dim docRights As String = String.Empty
        If Diritti(currentContainerGroup.DeskRights, DeskRightPositions.Insert) Then
            docRights = String.Concat(docRights, "Inserimento")
        End If
        If Diritti(currentContainerGroup.DeskRights, DeskRightPositions.Modify) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Modifica", ", Modifica"))
        End If
        If Diritti(currentContainerGroup.DeskRights, DeskRightPositions.ViewDocuments) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Visualizzazione", ", Visualizzazione"))
        End If
        If Diritti(currentContainerGroup.DeskRights, DeskRightPositions.Read) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Sommario", ", Sommario"))
        End If
        If Diritti(currentContainerGroup.DeskRights, DeskRightPositions.Delete) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Annullamento", ", Annullamento"))
        End If
        If Diritti(currentContainerGroup.DeskRights, DeskRightPositions.Close) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Chiusura", ", Chiusura"))
        End If
        If Diritti(currentContainerGroup.DeskRights, DeskRightPositions.Collaboration) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Collaborazione", ", Collaborazione"))
        End If
        AddPrivacyLevel(docRights, currentContainerGroup)
        dt.Rows.Add(docType, docRights)
    End Sub

    Private Sub BindReslRightsDetailFor(currentContainerGroup As ContainerGroup, ByRef dt As DataTable)
        Dim docType As String = Facade.TabMasterFacade.TreeViewCaption
        Dim docRights As String = String.Empty
        If Diritti(currentContainerGroup.ResolutionRights, ResolutionRightPositions.Insert) Then
            docRights = String.Concat(docRights, "Proposta")
        End If
        If Diritti(currentContainerGroup.ResolutionRights, ResolutionRightPositions.Executive) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Uff. Dirigenziale", ", Uff. Dirigenziale"))
        End If
        If Diritti(currentContainerGroup.ResolutionRights, ResolutionRightPositions.View) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Visualizzazione", ", Visualizzazione"))
        End If
        If Diritti(currentContainerGroup.ResolutionRights, ResolutionRightPositions.Preview) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Sommario", ", Sommario"))
        End If
        If Diritti(currentContainerGroup.ResolutionRights, ResolutionRightPositions.Cancel) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Cancellazione", ", Cancellazione"))
        End If
        If Diritti(currentContainerGroup.ResolutionRights, ResolutionRightPositions.Administration) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Amministrazione", ", Amministrazione"))
        End If
        If Diritti(currentContainerGroup.ResolutionRights, ResolutionRightPositions.Adoption) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Adozione", ", Adozione"))
        End If
        If Diritti(currentContainerGroup.ResolutionRights, ResolutionRightPositions.PrivacyAttachments) Then
            docRights = String.Concat(docRights, If(String.IsNullOrEmpty(docRights), "Allegati Riservati", ", Allegati Riservati"))
        End If
        AddPrivacyLevel(docRights, currentContainerGroup)
        dt.Rows.Add(docType, docRights)
    End Sub

    Private Sub AddPrivacyLevel(ByRef rights As String, containerGroup As ContainerGroup)
        If DocSuiteContext.Current.PrivacyLevelsEnabled AndAlso Not String.IsNullOrEmpty(rights) Then
            Dim privacyLevel As String = containerGroup.PrivacyLevel.ToString()
            rights = String.Concat(rights, If(String.IsNullOrEmpty(rights), String.Concat("Livello ", privacyLevel), String.Concat(", Livello ", privacyLevel)))
        End If
    End Sub

#End Region

End Class