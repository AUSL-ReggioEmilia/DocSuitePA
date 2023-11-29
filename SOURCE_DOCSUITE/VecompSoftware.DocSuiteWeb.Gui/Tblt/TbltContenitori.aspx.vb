Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Desks
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.UDS
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.DTO.Fascicles

Partial Class TbltContenitori
    Inherits CommonBasePage

#Region "Fields"
    Private _udsRepositoryFacade As UDSRepositoryFacade
    Private _hasContainerAdminRights As Boolean?
    Private Const DOCUMENT_OPTION As String = "DocumentOption"
    Private Const DOSSIER_OPTION As String = "DossierOption"
    Private Const DOCUMENTSERIES_CONSTRAINT_OPTION As String = "ConstraintOption"
    Private Const CREATE_OPTION As String = "create"
    Private Const MODIFY_OPTION As String = "modify"
    Private Const DELETE_OPTION As String = "delete"
    Private Const PRINT_OPTION As String = "print"
    Private Const LOG_OPTION As String = "log"
    Private Const RECOVER_OPTION As String = "recover"
    Private Const MODIFICA_OPTION As String = "modifica"
    Private Const PROPERTIES_OPTION As String = "properties"
    Private Const OPTION_OPTION As String = "option"
    Private Const SEARCH_OPTION As String = "search"
#End Region

#Region "Properties"

    Public ReadOnly Property SearchCategoryTextBox As RadTextBox
        Get
            Dim toolBarItem As RadToolBarItem = ToolBar.FindItemByValue("searchDescription")
            Return DirectCast(toolBarItem.FindControl("txtSearchContainer"), RadTextBox)
        End Get
    End Property

    Private ReadOnly Property EnvironmentsComboBox As RadComboBox
        Get
            Dim toolBarItem As RadToolBarItem = ToolBar.FindItemByValue("btnEnvironments")
            Return DirectCast(toolBarItem.FindControl("ddlEnvironments"), RadComboBox)
        End Get
    End Property

    Public ReadOnly Property SelectedEnvironment As String
        Get
            Return EnvironmentsComboBox.SelectedValue.ToString()
        End Get
    End Property

    Public ReadOnly Property SearchActive As Boolean
        Get
            Dim toolBarItem As RadToolBarItem = ToolBar.FindItemByValue("searchActive")
            Return DirectCast(toolBarItem, RadToolBarButton).Checked
        End Get
    End Property
    Private Property ContainerRightModels As IList(Of ContainerGroupRight)
        Get
            If ViewState("ContainerRightModels") IsNot Nothing Then
                Return DirectCast(ViewState("ContainerRightModels"), IList(Of ContainerGroupRight))
            End If
            Return New List(Of ContainerGroupRight)
        End Get
        Set(value As IList(Of ContainerGroupRight))
            ViewState("ContainerRightModels") = value
        End Set
    End Property

    Private ReadOnly Property HasContainerAdminRights As Boolean
        Get
            If Not _hasContainerAdminRights.HasValue Then
                _hasContainerAdminRights = (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupTblContainerAdminRight)
            End If
            Return _hasContainerAdminRights.Value
        End Get
    End Property

#End Region

#Region " Events "
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        If Not (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupTblContainerRight OrElse CommonShared.HasGroupTblContainerAdminRight) Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If

        If Not Page.IsPostBack Then
            Initialize()
        End If

        InitializeAjax()
    End Sub

    Protected Sub TbltContenitori_AjaxRequest(ByVal seneder As Object, ByVal e As AjaxRequestEventArgs)
        ' converto il valore di ritorno del javascript preparato dalla tbltContenitoriGes.aspx
        Dim currentArguments As String() = e.Argument.Split("|"c)

        Dim node As New RadTreeNode
        Select Case currentArguments(0).ToLowerInvariant()
            Case "add"
                LoadContainers()
                node = rtvContainers.FindNodeByValue(currentArguments(2))
                If node IsNot Nothing Then
                    node.Selected = True
                    LoadDetails()
                End If
            Case "delete"
                node = rtvContainers.FindNodeByValue(currentArguments(2))
                If node IsNot Nothing Then
                    node.CssClass = "notActive"
                    LoadDetails()
                End If
            Case "recovery"
                node = rtvContainers.FindNodeByValue(currentArguments(2))
                If node IsNot Nothing Then
                    node.CssClass = ""
                    LoadDetails()
                End If
            Case "rename", "update"
                node = rtvContainers.SelectedNode
                If node IsNot Nothing Then
                    node.Selected = True
                    If currentArguments.Length > 1 Then
                        node.Text = currentArguments(1)
                    End If
                    LoadDetails()
                    ContainerRightModels = GetContainerGroupsRight(Integer.Parse(rtvContainers.SelectedNode.Value))
                    grdGroups.DataSource = ContainerRightModels
                    grdGroups.DataBind()
                End If
            Case "adduser"
                Dim asUser As AccountModel = JsonConvert.DeserializeObject(Of AccountModel)(currentArguments(1))
                Dim group As SecurityGroups = Facade.SecurityGroupsFacade.GetGroupByName(asUser.RelatedGroupName)
                If group Is Nothing OrElse Facade.SecurityUsersFacade.IsUserInGroup(group, asUser.GetFullUserName()) Then
                    AjaxAlert("L'utente {0} è già presente nel gruppo {1} o il gruppo non esiste", asUser.Account, asUser.RelatedGroupName)
                    Exit Select
                End If
                Facade.SecurityUsersFacade.Insert(asUser.Domain, asUser.Account, asUser.Name, group)
                ContainerRightModels = GetContainerGroupsRight(Integer.Parse(rtvContainers.SelectedNode.Value))
                grdGroups.DataSource = ContainerRightModels
                grdGroups.MasterTableView.HierarchyDefaultExpanded = True
                grdGroups.DataBind()
        End Select
    End Sub

    Private Sub rtvContainers_NodeClick(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadTreeNodeEventArgs) Handles rtvContainers.NodeClick
        e.Node.ExpandChildNodes()
        If (String.IsNullOrEmpty(e.Node.Value)) Then
            SetDefaultToolbarButtonsState()
            Exit Sub
        End If
        LoadDetails()
        ContainerRightModels = GetContainerGroupsRight(Integer.Parse(e.Node.Value))
        grdGroups.DataSource = ContainerRightModels
        grdGroups.DataBind()

    End Sub

    Protected Sub ToolBarSearch_ButtonClick(sender As Object, e As RadToolBarEventArgs) Handles ToolBar.ButtonClick
        Select Case e.Item.Value
            Case SEARCH_OPTION
                rtvContainers.Nodes(0).Nodes.Clear()
                pnlDetails.Visible = False
                pnlOptions.SetDisplay(False)
                LoadContainers()
        End Select
    End Sub

    Protected Sub ToolBarStatus_ButtonClick(sender As Object, e As EventArgs)
        rtvContainers.Nodes(0).Nodes.Clear()
        pnlDetails.Visible = False
        pnlOptions.SetDisplay(False)
        LoadContainers()
    End Sub

    Protected Sub btnSearchUser_OnClick(sender As Object, e As EventArgs) Handles btnSearchUser.Click
        ContainerRightModels = GetContainerGroupsRight(Integer.Parse(rtvContainers.SelectedNode.Value))
        If Not String.IsNullOrEmpty(txtSearchUser.Text) Then
            ContainerRightModels = ContainerRightModels.Where(Function(x) x.Users.Any()).ToList()
        End If
        grdGroups.DataSource = ContainerRightModels
        grdGroups.MasterTableView.HierarchyDefaultExpanded = Not String.IsNullOrEmpty(txtSearchUser.Text)
        grdGroups.Rebind()
        grdGroups.Visible = True
    End Sub

    Protected Sub ddlEnvironments_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs)
        rtvContainers.Nodes(0).Nodes.Clear()
        pnlDetails.Visible = False
        pnlOptions.SetDisplay(False)
        LoadContainers()
    End Sub

    Protected Sub rdlOptions_OnSelectedIndexChanged(sender As Object, e As DropDownListEventArgs) Handles rdlOptions.SelectedIndexChanged
        Select Case e.Value
            Case DOCUMENT_OPTION
                rpvContainerExtGes.Selected = True
                uscContainerExtGes.IDContainer = Integer.Parse(rtvContainers.SelectedValue)
                uscContainerExtGes.KeyType = ContainerExtensionType.FL
                uscContainerExtGes.Initialize()

            Case DOSSIER_OPTION
                rpvContainerDossier.Selected = True
                uscContainerDossierOptions.LoadFolders(Integer.Parse(rtvContainers.SelectedValue))

            Case DOCUMENTSERIES_CONSTRAINT_OPTION
                rpvContainerConstraint.Selected = True
                uscContainerConstraintOptions.LoadConstraints(Integer.Parse(rtvContainers.SelectedValue))
        End Select
    End Sub
#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf TbltContenitori_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, rtvContainers, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, rtvContainers)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlDetails)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlOptions)
        AjaxManager.AjaxSettings.AddAjaxSetting(rtvContainers, rtvContainers, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(EnvironmentsComboBox, splPage, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(grdGroups, grdGroups, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearchUser, pnlDetails, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ToolBar, splPage, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ToolBar, splPage, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(rtvContainers, pnlDetails, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(rtvContainers, FolderToolBar, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(FolderToolBar, pnlDetails, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(FolderToolBar, pnlOptions, MasterDocSuite.AjaxDefaultLoadingPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(rtvContainers, pnlOptions)
        AjaxManager.AjaxSettings.AddAjaxSetting(rdlOptions, optionMultiPage)
        AjaxManager.AjaxSettings.AddAjaxSetting(rdlOptions, pnlOptionActions, MasterDocSuite.AjaxDefaultLoadingPanel)

    End Sub

    Private Sub Initialize()
        FolderToolBar.Visible = HasContainerAdminRights
        FolderToolBar.FindItemByValue(CREATE_OPTION).Visible = HasContainerAdminRights
        FolderToolBar.FindItemByValue(MODIFY_OPTION).Visible = HasContainerAdminRights
        FolderToolBar.FindItemByValue(DELETE_OPTION).Visible = HasContainerAdminRights
        FolderToolBar.FindItemByValue(PRINT_OPTION).Visible = HasContainerAdminRights
        FolderToolBar.FindItemByValue(MODIFICA_OPTION).Visible = HasContainerAdminRights
        FolderToolBar.FindItemByValue(LOG_OPTION).Visible = HasContainerAdminRights
        FolderToolBar.FindItemByValue(OPTION_OPTION).Visible = HasContainerAdminRights
        FolderToolBar.FindItemByValue(PROPERTIES_OPTION).Visible = SuperAdminAuthored
        uscContainerExtGes.StandardRoleRenderMode = RenderMode.Lightweight
        windowEditContainers.IconUrl = ImagePath.SmallBoxOpen
        pnlDetails.Visible = False
        optionMultiPage.SelectedIndex = -1
        pnlOptions.SetDisplay(False)
        LoadEnvironments()
    End Sub

    Protected Sub LoadEnvironments()
        Dim repositories As IList(Of RadComboBoxItem) = GetAvailableDSWEnvironments()
        Dim toolBarItem As RadToolBarItem = ToolBar.FindItemByValue("btnEnvironments")
        Dim combo As RadComboBox = DirectCast(toolBarItem.FindControl("ddlEnvironments"), RadComboBox)
        combo.Items.AddRange(repositories.OrderBy(Function(f) f.Text))
        combo.SelectedValue = String.Empty
    End Sub

    Private Function GetAvailableDSWEnvironments() As IList(Of RadComboBoxItem)
        Dim result As IList(Of RadComboBoxItem) = New List(Of RadComboBoxItem)

        Dim defaultItem As RadComboBoxItem = New RadComboBoxItem(String.Empty, String.Empty)
        result.Add(defaultItem)

        If DocSuiteContext.Current.IsProtocolEnabled Then
            Dim item As RadComboBoxItem = New RadComboBoxItem("Protocolli", DirectCast(LocationTypeEnum.ProtLocation, Integer).ToString())
            item.ImageUrl = "~/Comm/Images/DocSuite/Protocollo16.png"
            result.Add(item)
        End If
        If DocSuiteContext.Current.IsDocumentEnabled Then
            Dim item As RadComboBoxItem = New RadComboBoxItem(DocSuiteContext.Current.DossierAndPraticheLabel, DirectCast(LocationTypeEnum.DocmLocation, Integer).ToString())
            item.ImageUrl = "~/Comm/Images/DocSuite/Dossier_16.png"
            result.Add(item)
        End If
        If DocSuiteContext.Current.IsResolutionEnabled Then
            Dim item As RadComboBoxItem = New RadComboBoxItem(FacadeFactory.Instance.TabMasterFacade.TreeViewCaption, DirectCast(LocationTypeEnum.ReslLocation, Integer).ToString())
            item.ImageUrl = "~/Comm/Images/DocSuite/Atti16.png"
            result.Add(item)
        End If
        If DocSuiteContext.Current.ProtocolEnv.DocumentSeriesEnabled Then
            Dim item As RadComboBoxItem = New RadComboBoxItem(DocSuiteContext.Current.ProtocolEnv.DocumentSeriesName, DirectCast(LocationTypeEnum.DocumentSeriesLocation, Integer).ToString())
            item.ImageUrl = ImagePath.SmallDocumentSeries
            result.Add(item)
        End If
        If DocSuiteContext.Current.ProtocolEnv.UDSEnabled Then
            Dim item As RadComboBoxItem = New RadComboBoxItem("Archivi", DirectCast(LocationTypeEnum.UDSLocation, Integer).ToString())
            item.ImageUrl = ImagePath.SmallDocumentSeries
            result.Add(item)
        End If
        If DocSuiteContext.Current.ProtocolEnv.DeskEnable Then
            Dim item As RadComboBoxItem = New RadComboBoxItem("Tavoli", DirectCast(LocationTypeEnum.DeskLocation, Integer).ToString())
            item.ImageUrl = "~/App_Themes/DocSuite2008/images/desk/Desk.png"
            result.Add(item)
        End If
        Return result
    End Function

    Private Sub LoadContainers()
        Dim finder As New ContainerFinder()
        finder.Name = SearchCategoryTextBox.Text

        If Not String.IsNullOrEmpty(EnvironmentsComboBox.SelectedValue) Then
            finder.LocationTypeIn = {DirectCast([Enum].Parse(GetType(LocationTypeEnum), EnvironmentsComboBox.SelectedValue, True), LocationTypeEnum)}
        End If

        Dim containers As ICollection(Of Container) = finder.List()
        For Each item As Container In containers

            If active.Checked = True AndAlso item.IsActive Then
                AddNode(item)
            End If
            If inActive.Checked = True AndAlso Not item.IsActive Then
                AddNode(item)
            End If

        Next

        If active.Checked = True Then
            For Each item As Container In containers
                If item.IsActive Then
                    AddNode(item)
                End If
            Next
        End If

        If active.Checked = False Then
            For Each item As Container In containers

                If Not item.IsActive Then
                    AddNode(item)
                End If

            Next
        End If
    End Sub

    Private Function AddNode(ByVal container As Container) As RadTreeNode
        Dim currentNode As RadTreeNode = rtvContainers.FindNodeByValue(container.Id.ToString())
        If currentNode IsNot Nothing Then
            Return Nothing
        End If

        Dim nodeToAdd As RadTreeNode = CreateNode(container)
        rtvContainers.Nodes(0).Nodes.Add(nodeToAdd)
        Return nodeToAdd
    End Function

    Private Function CreateNode(ByVal container As Container) As RadTreeNode
        Dim vNode As New RadTreeNode()
        vNode.Text = container.Name
        vNode.Value = container.Id.ToString()
        If container.DocmLocation Is Nothing AndAlso container.DocumentSeriesLocation Is Nothing AndAlso container.ProtLocation Is Nothing AndAlso
            container.ReslLocation Is Nothing AndAlso container.UDSLocation Is Nothing Then
            vNode.Text = String.Concat(vNode.Text, " (*)")
            vNode.ToolTip = "Nessun deposito documentale abilitato"
        End If

        vNode.ImageUrl = ImagePath.SmallBoxOpen
        If Not Convert.ToBoolean(container.IsActive) Then
            vNode.CssClass = "notActive"
        End If
        If container.ContainerGroups.Any(Function(x) x.SecurityGroup Is Nothing) Then
            vNode.ToolTip = "A questo nodo non è associato un gruppo di sicurezza"
        End If

        vNode.Attributes.Add("UniqueId", container.UniqueId.ToString())
        Return vNode
    End Function

    Private Sub LoadDetails()
        pnlOptions.SetDisplay(False)
        If String.IsNullOrEmpty(rtvContainers.SelectedValue) Then
            pnlDetails.Visible = False
            Exit Sub
        End If

        Dim selectedContainer As Container = Facade.ContainerFacade.GetById(Integer.Parse(rtvContainers.SelectedValue))
        pnlDetails.Visible = True
        pnlPrivacy.Visible = DocSuiteContext.Current.PrivacyLevelsEnabled AndAlso CommonShared.HasGroupAdministratorRight
        If pnlPrivacy.Visible Then
            lblIsPrivacy.Text = If(selectedContainer.PrivacyEnabled, "Attiva", "Non Attiva")
            pnlPrivacyLevel.Visible = lblIsPrivacy.Text.Equals("Attiva") AndAlso Not String.IsNullOrEmpty(selectedContainer.PrivacyLevel.ToString)
            If pnlPrivacyLevel.Visible Then
                lblLevel.Text = selectedContainer.PrivacyLevel.ToString
            End If
        End If

        lblContainerNote.Text = selectedContainer.Note
        lblContainerNome.Text = $"{selectedContainer.Name} ({selectedContainer.Id} - {selectedContainer.UniqueId})"
        Dim containerLocations As IList(Of String) = GetContainerLocations(selectedContainer)
        If containerLocations.Count = 0 Then
            containerLocations.Add("Nessun deposito documentale abilitato")
        End If
        locationRepeater.DataSource = containerLocations
        locationRepeater.DataBind()


        ' pulsanti
        If Not HasContainerAdminRights Then
            FolderToolBar.Visible = True
            FolderToolBar.FindItemByValue(CREATE_OPTION).Enabled = True
            FolderToolBar.FindItemByValue(DELETE_OPTION).Enabled = False
            FolderToolBar.FindItemByValue(MODIFY_OPTION).Enabled = False
            FolderToolBar.FindItemByValue(PRINT_OPTION).Enabled = False
            FolderToolBar.FindItemByValue(MODIFICA_OPTION).Enabled = False
            FolderToolBar.FindItemByValue(LOG_OPTION).Enabled = False
            FolderToolBar.FindItemByValue(OPTION_OPTION).Enabled = False
        Else
            FolderToolBar.FindItemByValue(CREATE_OPTION).Enabled = True
            FolderToolBar.FindItemByValue(DELETE_OPTION).Enabled = True
            If selectedContainer IsNot Nothing AndAlso Not selectedContainer.IsActive Then
                FolderToolBar.FindItemByValue(RECOVER_OPTION).Visible = True
                FolderToolBar.FindItemByValue(RECOVER_OPTION).Enabled = True
            End If
            FolderToolBar.FindItemByValue(MODIFY_OPTION).Enabled = selectedContainer IsNot Nothing
            FolderToolBar.FindItemByValue(PRINT_OPTION).Enabled = selectedContainer IsNot Nothing
            FolderToolBar.FindItemByValue(MODIFICA_OPTION).Enabled = selectedContainer IsNot Nothing
            FolderToolBar.FindItemByValue(LOG_OPTION).Enabled = selectedContainer IsNot Nothing

            Dim series As DocumentSeries = Facade.DocumentSeriesFacade.GetDocumentSeries(selectedContainer)
            FolderToolBar.FindItemByValue(OPTION_OPTION).Enabled = selectedContainer IsNot Nothing _
                AndAlso ((DocSuiteContext.Current.IsDocumentEnabled AndAlso selectedContainer.DocmLocation IsNot Nothing AndAlso Not ProtocolEnv.DossierEnabled) _
                OrElse (selectedContainer.DocmLocation IsNot Nothing AndAlso ProtocolEnv.DossierEnabled) _
                OrElse (selectedContainer.DocumentSeriesLocation IsNot Nothing AndAlso ProtocolEnv.DocumentSeriesEnabled AndAlso series IsNot Nothing))
        End If

    End Sub

    Protected Sub FolderToolBar_ButtonClick(sender As Object, e As RadToolBarEventArgs) Handles FolderToolBar.ButtonClick
        Select Case e.Item.Value
            Case CREATE_OPTION
                AjaxManager.ResponseScripts.Add("OpenEditWindow('windowEditContainers','Add');")
            Case DELETE_OPTION
                AjaxManager.ResponseScripts.Add("OpenEditWindow('windowEditContainers','Delete');")
            Case MODIFY_OPTION
                AjaxManager.ResponseScripts.Add("OpenEditWindow('windowEditContainers','Rename');")
            Case PRINT_OPTION
                AjaxManager.ResponseScripts.Add("OpenPrintWindow('windowPrintContainers');")
            Case LOG_OPTION
                AjaxManager.ResponseScripts.Add("OpenLogWindow('windowLogContainers');")
            Case RECOVER_OPTION
                AjaxManager.ResponseScripts.Add("OpenEditWindow('windowEditContainers','Recovery');")
            Case MODIFICA_OPTION
                AjaxManager.ResponseScripts.Add($"OpenGroupsWindow('windowGroupContainers', {If(String.IsNullOrEmpty(SelectedEnvironment), "null", SelectedEnvironment)} );")
            Case PROPERTIES_OPTION
                AjaxManager.ResponseScripts.Add("OpenPropertiesWindow('windowPropertiesContainers');")
            Case OPTION_OPTION
                If rtvContainers.SelectedNode Is Nothing OrElse String.IsNullOrEmpty(rtvContainers.SelectedValue) Then
                    AjaxAlert("Selezionare un contenitore.")
                    Exit Sub
                End If

                pnlDetails.Visible = False
                pnlOptions.SetDisplay(True)
                ResetOptions()
        End Select
    End Sub

    ''' <summary> Visualizza il dettaglio del <see>Location</see> richiesto. </summary>
    Private Function GetContainerLocations(ByRef container As Container) As IList(Of String)
        Dim results As IList(Of String) = New List(Of String)
        Dim docmLocationfacade As LocationFacade = New LocationFacade("DocmDB")
        Dim protLocationFacade As LocationFacade = New LocationFacade("ProtDB")
        Dim reslLocationFacade As LocationFacade = New LocationFacade("ReslDB")

        If CommonInstance.DocmEnabled AndAlso container.DocmLocation IsNot Nothing Then
            Dim docmLocation As Location = docmLocationfacade.GetById(container.DocmLocation.Id)
            If docmLocation IsNot Nothing Then
                results.Add(String.Concat(DocSuiteContext.Current.DossierAndPraticheLabel, ": ", docmLocation.Name))
            End If
        End If

        If CommonInstance.ProtEnabled AndAlso container.ProtLocation IsNot Nothing Then
            Dim protLocation As Location = protLocationFacade.GetById(container.ProtLocation.Id)
            If protLocation IsNot Nothing Then
                results.Add(String.Concat("Protocollo: ", protLocation.Name))
            End If
        End If

        If DocSuiteContext.Current.ProtocolEnv.IsProtocolAttachLocationEnabled AndAlso container.ProtAttachLocation IsNot Nothing Then
            Dim protAttachLocation As Location = protLocationFacade.GetById(container.ProtAttachLocation.Id)
            If protAttachLocation IsNot Nothing Then
                results.Add(String.Concat("Allegati Protocollo: ", protAttachLocation.Name))
            End If
        End If

        If CommonInstance.ReslEnabled AndAlso container.ReslLocation IsNot Nothing Then
            Dim reslLocation As Location = reslLocationFacade.GetById(container.ReslLocation.Id)
            If reslLocation IsNot Nothing Then
                results.Add(String.Concat(Facade.TabMasterFacade.TreeViewCaption, ": ", reslLocation.Name))
            End If
        End If

        If container.DocumentSeriesLocation IsNot Nothing Then
            Dim docmsLocation As Location = protLocationFacade.GetById(container.DocumentSeriesLocation.Id)
            If docmsLocation IsNot Nothing Then
                results.Add(String.Concat(ProtocolEnv.DocumentSeriesName, ": ", docmsLocation.Name))
            End If
        End If

        If container.DocumentSeriesAnnexedLocation IsNot Nothing Then
            Dim docmsAnnexedLocation As Location = protLocationFacade.GetById(container.DocumentSeriesAnnexedLocation.Id)
            If docmsAnnexedLocation IsNot Nothing Then
                results.Add(String.Concat(ProtocolEnv.DocumentSeriesName & " Annessi (non parte integrante): ", docmsAnnexedLocation.Name))
            End If
        End If

        If container.DocumentSeriesUnpublishedAnnexedLocation IsNot Nothing Then
            Dim docmsUnpublishedAnnexedLocation As Location = protLocationFacade.GetById(container.DocumentSeriesUnpublishedAnnexedLocation.Id)
            If docmsUnpublishedAnnexedLocation IsNot Nothing Then
                results.Add(String.Concat(ProtocolEnv.DocumentSeriesName & " Annessi non pubblicabili: ", docmsUnpublishedAnnexedLocation.Name))
            End If
        End If

        If ProtocolEnv.DeskEnable AndAlso container.DeskLocation IsNot Nothing Then
            Dim deskLocation As Location = protLocationFacade.GetById(container.DeskLocation.Id)
            If deskLocation IsNot Nothing Then
                results.Add(String.Concat("Tavoli: ", deskLocation.Name))
            End If
        End If

        If container.UDSLocation IsNot Nothing Then
            Dim udsLocation As Location = protLocationFacade.GetById(container.UDSLocation.Id)
            If udsLocation IsNot Nothing Then
                results.Add(String.Concat("Archivi: ", udsLocation.Name))
            End If
        End If
        Return results
    End Function

    Protected Sub grdGroups_DataBind(sender As Object, e As GridItemEventArgs) Handles grdGroups.ItemDataBound
        If TypeOf e.Item Is GridGroupHeaderItem Then
            Dim item As GridGroupHeaderItem = DirectCast(e.Item, GridGroupHeaderItem)
            Dim myArr As String() = item.DataCell.Text.Split(":"c)
            item.DataCell.Text = myArr(1).Trim()
        End If

        If (TypeOf e.Item Is GridDataItem AndAlso e.Item.OwnerTableView.Name = "UsersGrid") Then
            Dim boundHeader As ContainerUserRight = DirectCast(e.Item.DataItem, ContainerUserRight)
            Dim imageUserControl As Image = DirectCast(e.Item.FindControl("imgUser"), Image)
            Dim buttonAddUserControl As ImageButton = DirectCast(e.Item.FindControl("btnAddUser"), ImageButton)
            buttonAddUserControl.Visible = False
            If HasContainerAdminRights AndAlso String.IsNullOrEmpty(boundHeader.Name) Then
                buttonAddUserControl.Visible = True
                imageUserControl.Visible = False
                buttonAddUserControl.OnClientClick = String.Concat("OpenEditWindowUsers('windowAddUsers','", boundHeader.GroupName, "');")
            End If
        End If
    End Sub

    Protected Sub grdGroups_DetailTableDataBind(source As Object, e As GridDetailTableDataBindEventArgs) Handles grdGroups.DetailTableDataBind
        Dim dataItem As GridDataItem = DirectCast(e.DetailTableView.ParentItem, GridDataItem)
        Dim groupName As String = dataItem.GetDataKeyValue("GroupName").ToString()
        Dim users As IList(Of ContainerUserRight) = New List(Of ContainerUserRight)
        Dim filteredModel As ContainerGroupRight = ContainerRightModels.Where(Function(x) x.GroupName.Eq(groupName)).FirstOrDefault()
        If filteredModel IsNot Nothing Then
            users = filteredModel.Users
        End If
        e.DetailTableView.DataSource = users
    End Sub

    Public Function CreateCointainerGroupRights(containerGroup As ContainerGroup, idContainer As Integer, location As Integer) As ContainerGroupRight
        Dim dto As ContainerGroupRight = New ContainerGroupRight(idContainer)
        dto.GroupName = containerGroup.Name
        Dim docmRights As String = String.Empty
        Dim userRights As IList(Of ContainerUserRight) = New List(Of ContainerUserRight)
        Dim userList As List(Of String) = New List(Of String)

        If containerGroup.SecurityGroup Is Nothing Then
            userList.Add("Il gruppo non è collegato ad un SecurityGroup.")
        Else
            If (containerGroup.SecurityGroup.HasAllUsers) Then
                userList.Add(SecurityGroups.DEFAULT_ALL_USER)
            End If
            userList.AddRange(containerGroup.SecurityGroup.SecurityUsers.OrderBy(Function(x) x.Description).Select(Function(f) GetLabel(f)))
        End If

        If Not String.IsNullOrEmpty(txtSearchUser.Text) Then
            userList = userList.Where(Function(x) x.ContainsIgnoreCase(txtSearchUser.Text)).ToList()
        End If
        Dim model As ContainerUserRight
        For Each contGr As String In userList
            model = New ContainerUserRight()
            model.Name = contGr
            model.GroupName = containerGroup.Name
            userRights.Add(model)
        Next
        userRights.Add(New ContainerUserRight() With {.Name = String.Empty, .GroupName = containerGroup.Name})
        If location = 1 Then
            dto.Location = "Documenti"

            If CommonInstance.DocmEnabled Then
                docmRights = GetDocmAuthorizationLabel(containerGroup)
            End If
            If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                docmRights = String.Concat(docmRights, If(String.IsNullOrEmpty(docmRights), String.Concat("Livello ", containerGroup.PrivacyLevel), String.Concat(" , Livello ", containerGroup.PrivacyLevel)))
            End If
        End If

        If location = 2 Then
            dto.Location = "Atti"
            If CommonInstance.ReslEnabled AndAlso (containerGroup.Container.ReslLocation IsNot Nothing) Then
                docmRights = GetResolutionAuthorizationLabel(containerGroup)
            End If
            If DocSuiteContext.Current.PrivacyLevelsEnabled AndAlso containerGroup.PrivacyLevel >= 0 Then
                docmRights = String.Concat(docmRights, If(String.IsNullOrEmpty(docmRights), String.Concat("Livello ", containerGroup.PrivacyLevel), String.Concat(" , Livello ", containerGroup.PrivacyLevel)))
            End If
        End If

        If location = 3 Then
            dto.Location = "Protocolli"
            If CommonInstance.ProtEnabled AndAlso (containerGroup.Container.ProtLocation IsNot Nothing) Then
                docmRights = GetProtocolAuthorizationLabel(containerGroup)
            End If
            If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                docmRights = String.Concat(docmRights, If(String.IsNullOrEmpty(docmRights), String.Concat("Livello ", containerGroup.PrivacyLevel), String.Concat(" , Livello ", containerGroup.PrivacyLevel)))
            End If
        End If

        If location = 4 Then
            dto.Location = "Serie Documentali"
            If CommonInstance.ProtEnabled AndAlso
                (containerGroup.Container.DocumentSeriesLocation IsNot Nothing) Then
                docmRights = GetDocumentSeriesAuthorizationLabel(containerGroup)
            End If
            If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                docmRights = String.Concat(docmRights, If(String.IsNullOrEmpty(docmRights), String.Concat("Livello ", containerGroup.PrivacyLevel), String.Concat(" , Livello ", containerGroup.PrivacyLevel)))
            End If
        End If

        If location = 5 Then
            dto.Location = "Tavoli"
            If ProtocolEnv.DeskEnable AndAlso
                (containerGroup.Container.DeskLocation IsNot Nothing) Then
                docmRights = GetDeskAuthorizationLabel(containerGroup)
            End If
            If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                docmRights = String.Concat(docmRights, If(String.IsNullOrEmpty(docmRights), String.Concat("Livello ", containerGroup.PrivacyLevel), String.Concat(" , Livello ", containerGroup.PrivacyLevel)))
            End If
        End If

        If location = 6 Then
            dto.Location = "Archivi"
            If ProtocolEnv.UDSEnabled AndAlso (containerGroup.Container.UDSLocation IsNot Nothing) Then
                docmRights = GetUDSAuthorizationLabel(containerGroup)
            End If
            If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                docmRights = String.Concat(docmRights, If(String.IsNullOrEmpty(docmRights), String.Concat("Livello ", containerGroup.PrivacyLevel), String.Concat(" , Livello ", containerGroup.PrivacyLevel)))
            End If
        End If

        If location = 7 Then
            dto.Location = "Fascicoli"
            If ProtocolEnv.FascicleContainerEnabled Then
                docmRights = GetFascicleAuthorizationLabel(containerGroup)
            End If

            If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                docmRights = String.Concat(docmRights, If(String.IsNullOrEmpty(docmRights), String.Concat("Livello ", containerGroup.PrivacyLevel), String.Concat(" , Livello ", containerGroup.PrivacyLevel)))
            End If
        End If
        dto.Users = userRights
        dto.Authorization = docmRights
        Return dto
    End Function

    Private Function GetAuthorizationLabel(Of TRights As Structure)(rights As String) As String
        Dim authLabels As ICollection(Of String) = New List(Of String)
        Dim activeAuthorizations As ICollection(Of [Enum]) = [Enum].GetValues(GetType(TRights)).OfType(Of [Enum]).
                                                                                   Where(Function(x) Diritti(rights, Convert.ToInt32(x))).
                                                                                   Select(Function(s) s).ToList()
        For Each right As [Enum] In activeAuthorizations
            authLabels.Add(right.GetDescription())
        Next
        Return String.Join(", ", authLabels)
    End Function

    Private Function GetDocmAuthorizationLabel(containerGroup As ContainerGroup) As String
        Dim authLabels As String = GetAuthorizationLabel(Of DocumentContainerRightPositions)(containerGroup.DocumentRights)
        Return authLabels
    End Function

    Private Function GetResolutionAuthorizationLabel(containerGroup As ContainerGroup) As String
        Dim authLabels As String = GetAuthorizationLabel(Of ResolutionRightPositions)(containerGroup.ResolutionRights)
        Return authLabels
    End Function

    Private Function GetProtocolAuthorizationLabel(containerGroup As ContainerGroup) As String
        Dim authLabels As String = GetAuthorizationLabel(Of ProtocolContainerRightPositions)(containerGroup.ProtocolRightsString)
        Return authLabels
    End Function

    Private Function GetDocumentSeriesAuthorizationLabel(containerGroup As ContainerGroup) As String
        Dim authLabels As String = GetAuthorizationLabel(Of DocumentSeriesContainerRightPositions)(containerGroup.DocumentSeriesRights)
        Return authLabels
    End Function

    Private Function GetDeskAuthorizationLabel(containerGroup As ContainerGroup) As String
        Dim authLabels As String = GetAuthorizationLabel(Of DeskRightPositions)(containerGroup.DeskRights)
        Return authLabels
    End Function

    Private Function GetUDSAuthorizationLabel(containerGroup As ContainerGroup) As String
        Dim authLabels As String = GetAuthorizationLabel(Of UDSRightPositions)(containerGroup.UDSRights)
        Return authLabels
    End Function

    Private Function GetFascicleAuthorizationLabel(containerGroup As ContainerGroup) As String
        Dim authLabels As String = GetAuthorizationLabel(Of FascicleRightPosition)(containerGroup.FascicleRights)
        Return authLabels
    End Function

    Public Enum Authorizations
        Document = 1
        Resolutions = 2
        Protocol = 3
        DocumentSeries = 4
        Desk = 5
        Archive = 6
        Fascicle = 7
    End Enum
    Public Function GetContainerGroupsRight(idContainer As Integer) As IList(Of ContainerGroupRight)
        Dim containerSelected As Container = Facade.ContainerFacade.GetById(idContainer)
        Dim models As IList(Of ContainerGroupRight) = New List(Of ContainerGroupRight)
        Dim temp As Array
        Dim containerGr As ContainerGroupRight
        For Each containerGroup As ContainerGroup In containerSelected.ContainerGroups
            temp = System.Enum.GetValues(GetType(Authorizations))
            For Each item As Integer In temp
                containerGr = CreateCointainerGroupRights(containerGroup, idContainer, item)
                If Not (containerGr.Authorization.IsNullOrEmpty) Then
                    models.Add(containerGr)
                End If
            Next
        Next
        Return models
    End Function

    ''' <summary> Visualizza il dettaglio del <see>ContainerGroup</see> richiesto. </summary>
    ''' 
    Public Function GetUserGroups(account As String) As IList(Of String)
        Dim groups As IList(Of String) = New List(Of String)
        Dim securityGroups As IList(Of SecurityGroups) = FacadeFactory.Instance.SecurityUsersFacade.GetGroupsByAccount(account)
        groups = securityGroups.Select(Function(s) s.GroupName).ToList()
        Return groups
    End Function

    Private Function Diritti(ByVal field As String, ByVal right As Integer) As Boolean
        If String.IsNullOrEmpty(field) Then
            Return False
        End If

        Return field.Substring(right - 1, 1).Eq("1"c)
    End Function

    Private Function EvaluteLocation(ByVal protLocation As Location) As String
        If protLocation Is Nothing Then
            Return String.Empty
        Else
            Return protLocation.Id.ToString()
        End If
    End Function

    Private Sub ResetOptions()
        ReloadOptionsSelection()
        optionMultiPage.SelectedIndex = -1
    End Sub

    Private Sub ReloadOptionsSelection()
        rdlOptions.ClearSelection()
        rdlOptions.Items.Clear()
        Dim selectedContainer As Container = Facade.ContainerFacade.GetById(Integer.Parse(rtvContainers.SelectedValue))
        If (DocSuiteContext.Current.IsDocumentEnabled AndAlso Not ProtocolEnv.DossierEnabled AndAlso selectedContainer.DocmLocation IsNot Nothing) Then
            rdlOptions.Items.Add(New DropDownListItem("Pratiche", DOCUMENT_OPTION))
        End If

        If (ProtocolEnv.DossierEnabled AndAlso selectedContainer.DocmLocation IsNot Nothing) Then
            rdlOptions.Items.Add(New DropDownListItem("Dossier", DOSSIER_OPTION))
        End If

        Dim series As DocumentSeries = Facade.DocumentSeriesFacade.GetDocumentSeries(selectedContainer)
        If (ProtocolEnv.DocumentSeriesEnabled AndAlso selectedContainer.DocumentSeriesLocation IsNot Nothing AndAlso series IsNot Nothing) Then
            rdlOptions.Items.Add(New DropDownListItem("Obblighi trasparenza", DOCUMENTSERIES_CONSTRAINT_OPTION))
        End If
    End Sub

    Private Sub SetDefaultToolbarButtonsState()
        FolderToolBar.FindItemByValue(MODIFY_OPTION).Enabled = False
        FolderToolBar.FindItemByValue(DELETE_OPTION).Enabled = False
        FolderToolBar.FindItemByValue(OPTION_OPTION).Enabled = False
        FolderToolBar.FindItemByValue(RECOVER_OPTION).Enabled = False
        FolderToolBar.FindItemByValue(PRINT_OPTION).Enabled = False
        FolderToolBar.FindItemByValue(MODIFICA_OPTION).Enabled = False
    End Sub
#End Region

End Class

