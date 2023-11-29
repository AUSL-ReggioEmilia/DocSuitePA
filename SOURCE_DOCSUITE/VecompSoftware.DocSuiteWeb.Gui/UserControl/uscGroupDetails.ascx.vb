Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Desks
Imports Newtonsoft.Json

Partial Public Class uscGroupDetails
    Inherits DocSuite2008BaseControl

#Region "Fields"
    Private Const ADD_USER As String = "addUser"
    Private Const DELETE_USER As String = "deleteUser"
    Private Const IMPORT_USER As String = "importUser"
#End Region

#Region "Properties"
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
    Public ReadOnly Property Button_AddUser As RadToolBarItem
        Get
            Return ActionsToolbar.FindItemByValue(uscGroupDetails.ADD_USER)
        End Get
    End Property
    Public ReadOnly Property Button_DeleteUser As RadToolBarItem
        Get
            Return ActionsToolbar.FindItemByValue(uscGroupDetails.DELETE_USER)
        End Get
    End Property
    Public ReadOnly Property Button_ImportUser As RadToolBarItem
        Get
            Return ActionsToolbar.FindItemByValue(uscGroupDetails.IMPORT_USER)
        End Get
    End Property
    Private ReadOnly Property SelectedGroup As SecurityGroups
        Get
            Return Facade.SecurityGroupsFacade.GetById(SelectedGroupId)
        End Get
    End Property

    Public Property GroupFacade As IGroupFacade(Of GroupRights)

    Public ContextMenuUserEnabled As Boolean = True

    Public ReadOnly Property SelectedUsers As IList(Of SecurityUsers)
        Get
            Dim selectedUsersIds As SecurityUsers = Nothing
            Dim selectedUsersList As IList(Of SecurityUsers) = New List(Of SecurityUsers)
            Dim usersSelected As IList(Of RadTreeNode) = RadTreeViewUsers.CheckedNodes()
            If (usersSelected Is Nothing OrElse usersSelected.Count() = 0) AndAlso SelectedIdsRole.Count = 0 AndAlso SelectedIdsContainer.Count = 0 Then
                Return Nothing
            End If
            If usersSelected IsNot Nothing OrElse usersSelected.Count() <> 0 Then
                Dim idsUser As IList(Of Integer) = usersSelected.Select(Function(x) Integer.Parse(x.Value)).ToList()
                For Each idUser As Integer In idsUser
                    selectedUsersIds = Facade.SecurityUsersFacade.GetById(idUser)
                    selectedUsersList.Add(selectedUsersIds)
                Next
            End If
            Return selectedUsersList
        End Get
    End Property

    Public ReadOnly Property SelectedIdsRole As IList(Of Integer)
        Get
            Dim selectedRoleIds As IList(Of Integer) = New List(Of Integer)
            For Each selectedItem As GridDataItem In grdRoles.MasterTableView.GetSelectedItems()
                Dim roleId As Integer = Integer.Parse(selectedItem.Attributes.Item("IdRole"))
                If selectedItem Is Nothing OrElse grdRoles.MasterTableView.GetSelectedItems().Count() = 0 Then
                    Return Nothing
                End If
                selectedRoleIds.Add(roleId)
            Next
            Return selectedRoleIds
        End Get
    End Property

    Public ReadOnly Property SelectedIdsContainer As IList(Of Integer)
        Get
            Dim selectedContainerIds As IList(Of Integer) = New List(Of Integer)
            For Each selectedItem As GridDataItem In grdContainers.MasterTableView.GetSelectedItems()
                Dim idContainer As Integer = Integer.Parse(selectedItem.Attributes.Item("IdContainer"))
                If selectedItem Is Nothing OrElse grdContainers.MasterTableView.GetSelectedItems().Count() = 0 Then
                    Return Nothing
                End If
                selectedContainerIds.Add(idContainer)
            Next
            Return selectedContainerIds
        End Get
    End Property

#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Me.RadTreeViewContextUsers.Visible = ContextMenuUserEnabled
        End If
    End Sub

    Protected Sub grdRoles_DetailTableDataBind(source As Object, e As GridDetailTableDataBindEventArgs) Handles grdRoles.DetailTableDataBind
        Dim dataItem As GridDataItem = DirectCast(e.DetailTableView.ParentItem, GridDataItem)
        Dim roleId As Integer = Integer.Parse(dataItem.Attributes.Item("IdRole"))
        Dim currentRole As Role = Facade.RoleFacade.GetById(roleId)
        Dim defaultTitle As String = String.Concat(currentRole.Name, " (", currentRole.Id, ")")

        Dim dt As DataTable = New DataTable()
        dt.Columns.Add("DocumentType")
        dt.Columns.Add("Rights")

        If String.IsNullOrEmpty(SelectedGroup.GroupName) Then
            Dim lbl As String = If(ProtocolEnv.TNoticeEnabled, "TNotice", "Poste Web")
            If ProtocolEnv.IsPosteWebEnabled() OrElse ProtocolEnv.TNoticeEnabled Then
                Dim accounts As IList(Of POLAccount) = Facade.PosteOnLineAccountFacade.GetByRoles(New List(Of Role) From {currentRole})
                If accounts.Any() Then
                    For Each account As POLAccount In accounts
                        dt.Rows.Add(lbl, account.Name)
                    Next
                End If
            End If
            'Tabella email            
            If Not String.IsNullOrEmpty(currentRole.EMailAddress) Then dt.Rows.Add(String.Format("Email", currentRole.EMailAddress))
            If currentRole.Mailboxes IsNot Nothing Then
                Dim mailboxes As IList(Of PECMailBox) = currentRole.Mailboxes.Where(Function(pmb) Not pmb.IsProtocolBox.GetValueOrDefault(False)).ToList()
                If ProtocolEnv.IsPECEnabled AndAlso mailboxes.Any() Then
                    For Each mailBox As PECMailBox In mailboxes
                        Dim isMailBoxDefault As Boolean = mailBox.MailBoxRoles.Any(Function(x) x.Id.RoleId.Equals(currentRole.Id) AndAlso x.Priority)
                        Dim cellText As String = String.Format("{0}{1}", mailBox.MailBoxName, If(isMailBoxDefault, " [casella predefinita]", String.Empty))
                        dt.Rows.Add("Posta Certificata ", cellText)
                    Next
                End If
            End If
            If currentRole.Mailboxes IsNot Nothing AndAlso currentRole.Mailboxes.Any() Then
                Dim protocolMailboxes As IList(Of PECMailBox) = currentRole.Mailboxes.Where(Function(pmb) pmb.IsProtocolBox.GetValueOrDefault(False)).ToList()
                If ProtocolEnv.ProtocolBoxEnabled AndAlso protocolMailboxes.Any() Then
                    For Each mailBox As PECMailBox In protocolMailboxes
                        Dim isMailBoxDefault As Boolean = mailBox.MailBoxRoles.Any(Function(x) x.Id.RoleId.Equals(currentRole.Id) AndAlso x.Priority)
                        Dim cellText As String = String.Format("{0}{1}", mailBox.MailBoxName, If(isMailBoxDefault, " [casella predefinita]", String.Empty))
                        dt.Rows.Add("Casella di protocollazione", cellText)
                    Next
                End If
            End If
        Else
            Dim roleGroup As RoleGroup = currentRole.GetRoleGroup(SelectedGroup.GroupName)
            If roleGroup Is Nothing Then
                Exit Sub
            End If
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
                dt.Rows.Add(docType, docRights)
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

                dt.Rows.Add(docType, docRights)
            End If

            If CommonInstance.ReslEnabled Then
                docType = Facade.TabMasterFacade.TreeViewCaption
                docRights = String.Empty
                If Diritti(roleGroup.ResolutionRights, ResolutionRightPositions.Insert) Then
                    docRights = "Abilitato"
                End If
                dt.Rows.Add(docType, docRights)
            End If

            If ProtocolEnv.DocumentSeriesEnabled Then
                docType = ProtocolEnv.DocumentSeriesName
                docRights = String.Empty
                If Diritti(roleGroup.DocumentSeriesRights, DocumentSeriesRoleRightPositions.Enabled) Then
                    docRights = "Abilitato"
                End If
                dt.Rows.Add(docType, docRights)
            End If
        End If

        e.DetailTableView.DataSource = dt
    End Sub

    Protected Sub grdContainers_DetailTableDataBind(source As Object, e As GridDetailTableDataBindEventArgs) Handles grdContainers.DetailTableDataBind
        Dim dataItem As GridDataItem = DirectCast(e.DetailTableView.ParentItem, GridDataItem)
        Dim containerId As Integer = Integer.Parse(dataItem.Attributes.Item("IdContainer"))
        Dim currentContainerGroup As ContainerGroup = Facade.ContainerGroupFacade.GetByContainerAndName(containerId, SelectedGroup.GroupName)

        Dim dt As DataTable = New DataTable()
        dt.Columns.Add("DocumentType")
        dt.Columns.Add("Rights")

        If CommonInstance.DocmEnabled AndAlso (currentContainerGroup.Container.DocmLocation IsNot Nothing) Then
            BindDocmRightsDetailFor(currentContainerGroup, dt)
        End If

        If CommonInstance.ProtEnabled AndAlso (currentContainerGroup.Container.ProtLocation IsNot Nothing) Then
            BindProtocolRightsDetailFor(currentContainerGroup, dt)
        End If

        If CommonInstance.ReslEnabled AndAlso (currentContainerGroup.Container.ReslLocation IsNot Nothing) Then
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

        If currentContainerGroup.Container.UDSLocation IsNot Nothing Then
            BindUDSDetailFor(currentContainerGroup, dt)
        End If
        e.DetailTableView.DataSource = dt
    End Sub

    Private Sub grdRoles_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles grdRoles.ItemDataBound, grdRoles.ItemEvent

        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim item As RoleGroup = TryCast(e.Item.DataItem, RoleGroup)
        If item Is Nothing Then
            Exit Sub
        End If

        Dim row As GridDataItem = DirectCast(e.Item, GridDataItem)
        row.Attributes.Add("IdRole", item.Role.Id.ToString())
        With DirectCast(e.Item.FindControl("RoleName"), Label)
            If Not String.IsNullOrEmpty(item.Role.Name) Then
                row.Item("RoleName").Text = item.Role.Name
            End If
            If Not item.Role.IsActive Then
                row.Item("RoleName").ForeColor = Drawing.Color.LightSlateGray
                row.Item("RoleName").ToolTip = "Settore disabilitato"
                Dim documentImage As Image = DirectCast(e.Item.FindControl("roleDisabled"), Image)
                documentImage.ImageUrl = "~/App_Themes/DocSuite2008/imgset16/brickEnabled.png"
            End If
        End With

    End Sub

    Private Sub grdContainers_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles grdContainers.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If
        Dim item As ContainerGroup = TryCast(e.Item.DataItem, ContainerGroup)
        If item Is Nothing Then
            Exit Sub
        End If

        Dim row As GridDataItem = DirectCast(e.Item, GridDataItem)
        row.Attributes.Add("IdContainer", item.Container.Id.ToString())
        With DirectCast(e.Item.FindControl("ContainerName"), Label)
            If Not String.IsNullOrEmpty(item.Container.Name) Then
                row.Item("ContainerName").Text = item.Container.Name
            End If
        End With

    End Sub

    Protected Sub uscGroupDetails_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arguments As String() = Split(e.Argument, "|", 3)
        Select Case arguments(0).ToLower()
            Case "importfrom"
                Dim listGroups As String() = JsonConvert.DeserializeObject(Of String())(arguments(1))
                For Each nameGroup As String In listGroups
                    Dim selGroup As SecurityGroups = Facade.SecurityGroupsFacade.GetGroupByName(nameGroup)
                    Dim currentGroup As SecurityGroups = Me.SelectedGroup
                    If selGroup IsNot Nothing Then
                        For Each user As SecurityUsers In selGroup.SecurityUsers.Where(Function(f) Not Facade.SecurityUsersFacade.IsUserInGroup(currentGroup, f.DisplayName))
                            Facade.SecurityUsersFacade.Insert(user.UserDomain, user.Account, user.Description, currentGroup)
                        Next
                    End If
                Next

            Case "users"
                AddUser(arguments(1))
            Case "deleteFromGroup"
                DeleteFromGroup()
        End Select
        pnlDetail.Style.Add("visibility", "visible")
        LoadUsers(SelectedGroup)
    End Sub
#End Region

#Region " Methods "
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf uscGroupDetails_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, RadTreeViewUsers, BasePage.MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, grdRoles, BasePage.MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, grdContainers, BasePage.MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub
    Public Sub LoadDetails()
        pnlDetail.Style.Add("visibility", "visible")
        LoadUsers(SelectedGroup)
        LoadGridRoles(SelectedGroup)
        LoadGridContainers(SelectedGroup)
    End Sub

    Public Sub LoadOnlyUsers()
        pnlDetail.Style.Add("visibility", "visible")
        Dim usersPanelItem As RadPanelItem = rpbGroups.FindItemByValue("rpiUsers")
        usersPanelItem.Visible = True
        LoadUsers(SelectedGroup)
    End Sub

    Public Sub LoadUsers(ByVal group As SecurityGroups)
        RadTreeViewUsers.Nodes.Clear()

        If group Is Nothing Then
            Exit Sub
        End If

        Dim item As RadPanelItem = rpbGroups.FindItemByValue("rpiUsers")
        item.Visible = True
        item.Text = $"Utenti ({group.Id} - {group.UniqueId})"

        If (group.HasAllUsers) Then
            RadTreeViewUsers.Nodes.Add(New RadTreeNode(SecurityGroups.DEFAULT_ALL_USER))
            Return
        End If

        If group.SecurityUsers.Count = 0 Then
            RadTreeViewUsers.Nodes.Add(New RadTreeNode("Nessun utente definito"))
            RadTreeViewUsers.Nodes(0).Checkable = False
        End If

        For Each securityUsers As SecurityUsers In group.SecurityUsers.OrderBy(Function(f) f.Description)
            AddUserNode(RadTreeViewUsers, securityUsers)
        Next
    End Sub

    Private Function AddUserNode(ByRef node As RadTreeView, ByRef user As SecurityUsers) As RadTreeNode
        Dim newNode As RadTreeNode = New RadTreeNode()
        newNode.Value = user.Id.ToString()
        newNode.Text = user.Account & If(Not String.IsNullOrEmpty(user.Description), String.Format(" ({0})", user.Description), "")
        newNode.Font.Bold = True
        newNode.Expanded = True

        If DocSuiteContext.Current.ProtocolEnv.MultiDomainEnabled Then
            Dim userDomain As String = user.UserDomain
            If String.IsNullOrEmpty(userDomain) Then
                userDomain = DocSuiteContext.Current.CurrentDomainName
            End If
            Dim userDescription As String = user.Account
            If Not String.IsNullOrEmpty(user.Description) Then
                userDescription = String.Concat(user.Account, " (", user.Description, ")")
            End If

            newNode.Text = String.Format("{0}\{1}", userDomain, userDescription)
        End If

        node.Nodes.Add(newNode)

        Return newNode
    End Function

    Private Sub LoadGridRoles(ByVal group As SecurityGroups)
        If group Is Nothing Then
            Exit Sub
        End If

        Dim rolesPanelItem As RadPanelItem = rpbGroups.FindItemByValue("rpiRoles")
        rolesPanelItem.Visible = True

        grdRoles.DataSource = Facade.RoleGroupFacade.GetBySecurityGroup(group, True)
        grdRoles.DataBind()
    End Sub

    Private Sub LoadGridContainers(ByVal group As SecurityGroups)

        If group Is Nothing Then
            Exit Sub
        End If

        Dim containersPanelItem As RadPanelItem = rpbGroups.FindItemByValue("rpiContainers")
        containersPanelItem.Visible = True

        grdContainers.DataSource = Facade.ContainerGroupFacade.GetBySecurityGroup(group, True)
        grdContainers.DataBind()
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
        dt.Rows.Add(docType, docRights)
    End Sub

    Private Function Diritti(ByVal field As String, ByVal right As Integer) As Boolean
        If String.IsNullOrEmpty(field) Then
            Return False
        End If

        Return field.Substring(right - 1, 1).Eq("1"c)
    End Function

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
        dt.Rows.Add(docType, docRights)
    End Sub

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
        dt.Rows.Add(docType, docRights)
    End Sub

    Public Sub ClearRadTreeViewUsers()
        RadTreeViewUsers.Nodes.Clear()
        If SelectedIdsRole.Count > 0 Then
            LoadGridRoles(SelectedGroup)
        End If
        If SelectedIdsContainer.Count > 0 Then
            LoadGridContainers(SelectedGroup)
        End If
    End Sub

    Public Sub DeleteFromGroup()
        Try
            ' cancello dal db
            Dim userDisplayName As String = String.Empty
            Dim usersChosen As IList(Of SecurityUsers) = SelectedUsers
            If usersChosen IsNot Nothing AndAlso usersChosen.Count() > 0 Then
                For Each userSelected As SecurityUsers In usersChosen
                    userDisplayName = userSelected.DisplayName
                    Facade.SecurityUsersFacade.Delete(userSelected)
                Next
            End If
            ' Aggiorno utenti e gruppo
            LoadUsers(SelectedGroup)

            Dim roleIdsChosen As IList(Of Integer) = SelectedIdsRole
            Dim containerIdsChosen As IList(Of Integer) = SelectedIdsContainer

            If roleIdsChosen.Count > 0 Then
                Dim groupSelected As IList(Of RoleGroup) = Facade.RoleGroupFacade.GetByRolesAndGroup(SelectedIdsRole.ToArray(), SelectedGroup.GroupName)
                For Each id As Integer In roleIdsChosen
                    For Each roleSelected As RoleGroup In groupSelected.Where(Function(x) x.Role.Id = id)
                        Facade.RoleGroupFacade.DeleteGroup(roleSelected)
                    Next
                Next
                LoadGridRoles(SelectedGroup)
            End If

            If containerIdsChosen.Count > 0 Then
                Dim idsFromContainerGroup As IList(Of ContainerGroup) = Facade.ContainerGroupFacade.GetByContainersAndGroup(containerIdsChosen.ToArray(), SelectedGroup.GroupName)
                For Each idContainer As Integer In containerIdsChosen
                    For Each containerChosen As ContainerGroup In idsFromContainerGroup.Where(Function(c) c.Container.Id = idContainer)
                        Facade.ContainerGroupFacade.DeleteGroup(containerChosen)
                    Next
                Next
                LoadGridContainers(SelectedGroup)
            End If


        Catch ex As Exception

        End Try
    End Sub

    Public Sub AddUser(user As String)
        Dim adUser As AccountModel = JsonConvert.DeserializeObject(Of AccountModel)(user)
        Dim group As SecurityGroups = SelectedGroup

        If Facade.SecurityUsersFacade.IsUserInGroup(group, adUser.GetFullUserName()) Then
            Exit Sub
        End If
        Facade.SecurityUsersFacade.Insert(adUser.Domain, adUser.Account, adUser.Name, group)
    End Sub
#End Region

End Class