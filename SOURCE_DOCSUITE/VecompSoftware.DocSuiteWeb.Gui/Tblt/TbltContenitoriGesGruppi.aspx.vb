Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Desks
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.Model.Parameters
Imports NHibernate.Util
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Commons
Imports APIEntity = VecompSoftware.DocSuiteWeb.Entity.Commons
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.DTO.Fascicles
Imports VecompSoftware.DocSuiteWeb.Facade.Common.WebAPI

Partial Public Class TbltContenitoriGesGruppi
    Inherits CommonBasePage

#Region " Fields "

    Private _currentContainer As Container
    Private _reslEnable As Boolean
    Private _privacyLevelFinder As PrivacyLevelFinder
    Private _maxValue As Integer?
    Private _allowedLevels As ICollection(Of WebAPIDto(Of APIEntity.PrivacyLevel))
#End Region

#Region " Properties "

    Private ReadOnly Property CurrentPrivacyLevelFinder As PrivacyLevelFinder
        Get
            If _privacyLevelFinder Is Nothing Then
                _privacyLevelFinder = New PrivacyLevelFinder(DocSuiteContext.Current.CurrentTenant)
            End If
            Return _privacyLevelFinder
        End Get
    End Property

    Private ReadOnly Property GroupName() As String
        Get
            Return Request.QueryString("GroupName")
        End Get
    End Property

    Private ReadOnly Property ResolutionEnable() As Boolean
        Get
            Return _reslEnable
        End Get
    End Property

    Public ReadOnly Property CurrentContainer() As Container
        Get
            If _currentContainer Is Nothing Then
                Dim temp As Integer = Request.QueryString.GetValueOrDefault(Of Integer)("IdContainer", 0)
                If temp <> 0 Then
                    _currentContainer = Facade.ContainerFacade.GetById(temp, False)
                Else
                    _currentContainer = New Container()
                End If
            End If
            Return _currentContainer
        End Get
    End Property

    Public ReadOnly Property Active As Boolean
        Get
            Return GetKeyValue(Of Boolean)("Active")
        End Get
    End Property

    Public ReadOnly Property Environment As String
        Get
            Return Request.QueryString.GetValueOrDefault(Of String)("Environment", String.Empty)
        End Get
    End Property

    Public ReadOnly Property CurrentProtocolRightsSelected As String
        Get
            Dim protRights As String = GroupRights.EmptyRights
            Dim rights As IEnumerable(Of ListItem) = cblProt.Items.OfType(Of ListItem)()
            For Each item As ListItem In rights
                CommonUtil.GetInstance.SetGroupRight(protRights, Convert.ToInt32(item.Value), item.Selected)
            Next
            Return protRights
        End Get
    End Property

    Public ReadOnly Property CurrentResolutionRightsSelected As String
        Get
            Dim reslRights As String = GroupRights.EmptyRights
            Dim rights As IEnumerable(Of ListItem) = cblResl.Items.OfType(Of ListItem)()
            For Each item As ListItem In rights
                CommonUtil.GetInstance.SetGroupRight(reslRights, Convert.ToInt32(item.Value), item.Selected)
            Next
            Return reslRights
        End Get
    End Property

    Public ReadOnly Property CurrentSeriesRightsSelected As String
        Get
            Dim seriesRights As String = GroupRights.EmptyRights
            Dim rights As IEnumerable(Of ListItem) = cblSeries.Items.OfType(Of ListItem)()
            For Each item As ListItem In rights
                CommonUtil.GetInstance.SetGroupRight(seriesRights, Convert.ToInt32(item.Value), item.Selected)
            Next
            Return seriesRights
        End Get
    End Property

    Public ReadOnly Property CurrentDocmRightsSelected As String
        Get
            Dim docmRights As String = GroupRights.EmptyRights
            Dim rights As IEnumerable(Of ListItem) = cblDocm.Items.OfType(Of ListItem)()
            For Each item As ListItem In rights
                CommonUtil.GetInstance.SetGroupRight(docmRights, Convert.ToInt32(item.Value), item.Selected)
            Next
            Return docmRights
        End Get
    End Property

    Public ReadOnly Property CurrentDeskRightsSelected As String
        Get
            Dim deskRights As String = GroupRights.EmptyRights
            Dim rights As IEnumerable(Of ListItem) = cblDesks.Items.OfType(Of ListItem)()
            For Each item As ListItem In rights
                CommonUtil.GetInstance.SetGroupRight(deskRights, Convert.ToInt32(item.Value), item.Selected)
            Next
            Return deskRights
        End Get
    End Property

    Public ReadOnly Property CurrentUDSRightsSelected As String
        Get
            Dim udsRights As String = GroupRights.EmptyRights
            Dim rights As IEnumerable(Of ListItem) = cblUDS.Items.OfType(Of ListItem)()
            For Each item As ListItem In rights
                CommonUtil.GetInstance.SetGroupRight(udsRights, Convert.ToInt32(item.Value), item.Selected)
            Next
            Return udsRights
        End Get
    End Property

    Public ReadOnly Property CurrentFascicleRightsSelected As String
        Get
            Dim fascicleRights As String = GroupRights.EmptyRights
            Dim rights As IEnumerable(Of ListItem) = cblFascicles.Items.OfType(Of ListItem)()
            For Each item As ListItem In rights
                CommonUtil.GetInstance.SetGroupRight(fascicleRights, Convert.ToInt32(item.Value), item.Selected)
            Next
            Return fascicleRights
        End Get
    End Property

    Private ReadOnly Property MaxValue As Integer
        Get
            If _maxValue Is Nothing Then
                _maxValue = AllowedLevels.Max(Function(x) x.Entity.Level)
            End If
            Return _maxValue.Value
        End Get
    End Property

    Private ReadOnly Property AllowedLevels As ICollection(Of WebAPIDto(Of APIEntity.PrivacyLevel))
        Get
            If _allowedLevels Is Nothing Then
                _allowedLevels = WebAPIImpersonatorFacade.ImpersonateFinder(CurrentPrivacyLevelFinder,
                    Function(impersonationType, finder)
                        finder.ResetDecoration()
                        finder.MinimumLevel = CurrentContainer.PrivacyLevel
                        finder.EnablePaging = False
                        Return finder.DoSearch()
                    End Function)
            End If
            Return _allowedLevels
        End Get
    End Property
    Protected ReadOnly Property ShowReadonlySecurityGroups As Boolean
        Get
            Dim _readOnlyMode As Boolean = Request.QueryString.GetValueOrDefault("ReadonlySecurityGroups", False)
            Return _readOnlyMode
        End Get
    End Property


#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupTblContainerAdminRight) Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If

        InitializeAjax()
        InitializeControls()

        If Not Page.IsPostBack Then
            Dim menuJson As IDictionary(Of String, MenuNodeModel) = DocSuiteContext.Current.DocSuiteMenuConfiguration

            lblSeries.Text = String.Empty
            lblUDS.Text = String.Empty
            If menuJson.Keys.Contains("Menu6") AndAlso menuJson("Menu6").Nodes.Keys.Contains("FirstNode1") Then
                lblSeries.Text = menuJson("Menu6").Nodes("FirstNode1").Name
            End If
            If menuJson.Keys.Contains("Menu6") AndAlso menuJson("Menu6").Nodes.Keys.Contains("FirstNode2") Then
                lblUDS.Text = menuJson("Menu6").Nodes("FirstNode2").Name
            End If

            InitializePrivacy()
            InitializeLocations()
            InitializeRights()
            InitializeGroups(GroupName)

        End If

        uscGruppi.Visible = Not ShowReadonlySecurityGroups
        btnConfermaDiritti.Visible = Not ShowReadonlySecurityGroups
    End Sub

    Protected Sub TbltContenitoriGesGruppi_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim currentArguments As String() = e.Argument.Split("|"c)

        Dim node As New RadTreeNode
        Select Case currentArguments(0).ToLowerInvariant()
            Case "copy"
                CopyContainerGroups(Integer.Parse(currentArguments(1)))
        End Select
    End Sub

    Private Sub CopyContainerGroups(ByVal containerId As Integer)
        Dim selectedContainerToCopy As Container = Facade.ContainerFacade.GetById(containerId, False)
        Dim containerGroup As ContainerGroup

        Dim containersToCopy As IList(Of ContainerGroup) = selectedContainerToCopy.ContainerGroups.Where(Function(x) Not CurrentContainer.ContainerGroups.Any(Function(y) y.Name = x.Name)).ToList()

        For Each container As ContainerGroup In containersToCopy
            uscGruppi.AddGroup(container.Name)
            If String.IsNullOrEmpty(uscGruppi.SelectedNode.Value) Then
                ' Nuovi diritti di un gruppo sul contenitore
                containerGroup = New ContainerGroup()
                containerGroup = PrepareContainer(containerGroup)

                containerGroup.ProtocolRightsString = container.ProtocolRightsString
                containerGroup.DocumentRights = container.DocumentRights
                containerGroup.ResolutionRights = container.ResolutionRights
                containerGroup.DocumentSeriesRights = container.DocumentSeriesRights
                containerGroup.DeskRights = container.DeskRights
                containerGroup.UDSRights = container.UDSRights
                containerGroup.FascicleRights = container.FascicleRights

                containerGroup.Container = CurrentContainer

                containerGroup.SecurityGroup = Facade.SecurityGroupsFacade.GetGroupByName(uscGruppi.SelectedNode.Text)
                If containerGroup.SecurityGroup Is Nothing Then
                    AjaxAlert("Impossibile salvare la modifica. Il gruppo non esiste.")
                End If

                Facade.ContainerGroupFacade.Save(containerGroup)
                CurrentContainer.ContainerGroups.Add(containerGroup)
                InitializeGroups(uscGruppi.SelectedNode.Text)

            End If
            Dim grpName As String = GroupName
            uscGruppi.GroupName = container.Name
            uscGruppi.Refresh()
        Next

    End Sub
    Private Sub LoadContainers()

        Dim finder As New Facade.ContainerFinder()
        finder.Name = String.Empty

        If Not String.IsNullOrEmpty(Environment) Then
            finder.LocationTypeIn = {DirectCast([Enum].Parse(GetType(LocationTypeEnum), Environment, True), LocationTypeEnum)}
        End If

        Dim containers As ICollection(Of Container) = finder.List()
        For Each item As Container In containers

            If Active = True AndAlso item.IsActive Then
                AddNode(item)
            End If
            If Active = False AndAlso Not item.IsActive Then
                AddNode(item)
            End If

        Next

        If Active = True Then
            For Each item As Container In containers
                If item.IsActive Then
                    AddNode(item)
                End If
            Next
        End If

        If Active = False Then
            For Each item As Container In containers

                If Not item.IsActive Then
                    AddNode(item)
                End If

            Next
        End If

    End Sub

    Private Sub AddNode(ByVal container As Container)
        Dim currentNode As RadTreeNode = rtvContainersCopy.FindNodeByValue(container.Id.ToString())
        If currentNode IsNot Nothing Then
            Exit Sub
        End If

        Dim nodeToAdd As RadTreeNode = CreateNode(container)
        If nodeToAdd.Value = CurrentContainer.Id.ToString Then
            nodeToAdd.Enabled = False
        End If
        rtvContainersCopy.Nodes(0).Nodes.Add(nodeToAdd)
    End Sub

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
    Private Sub BtnConfermaDirittiClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnConfermaDiritti.Click

        If uscGruppi.SelectedNode Is Nothing Then
            AjaxAlert("Selezionare un gruppo.")
            Exit Sub
        End If

        If Not CheckPageHasRightsSelected() Then 'non ho impostato nessun diritto
            AjaxAlert("Impossibile salvare la modifica. Impostare almeno un diritto oppure eliminare il gruppo.")
            Exit Sub
        End If



        Dim containerGroup As ContainerGroup
        If String.IsNullOrEmpty(uscGruppi.SelectedNode.Value) Then
            ' Nuovi diritti di un gruppo sul contenitore
            containerGroup = New ContainerGroup()

            containerGroup = PrepareContainer(containerGroup)
            containerGroup.Container = CurrentContainer

            containerGroup.SecurityGroup = Facade.SecurityGroupsFacade.GetGroupByName(uscGruppi.SelectedNode.Text)
            If containerGroup.SecurityGroup Is Nothing Then
                AjaxAlert("Impossibile salvare la modifica. Il gruppo non esiste.")
            End If

            If DocSuiteContext.Current.PrivacyLevelsEnabled AndAlso CurrentContainer.PrivacyEnabled AndAlso containerGroup IsNot Nothing AndAlso MaxValue < containerGroup.PrivacyLevel Then
                AjaxAlert(String.Concat("Attenzione: errore nelle configurazioni dei livelli di", PRIVACY_LABEL, ". Il livello del gruppo e' superiore a quelli permessi."))
                Exit Sub
            End If

            Facade.ContainerGroupFacade.Save(containerGroup)
            InitializeGroups(uscGruppi.SelectedNode.Text)
        Else
            ' Modifica dei diritti del gruppo sul contenitore
            containerGroup = CType(uscGruppi.GetGroup(uscGruppi.SelectedNode.Text), ContainerGroup)
            If containerGroup Is Nothing Then
                Throw New DocSuiteException("Errore in fase di recupero ContainerGroup " + uscGruppi.SelectedNode.Text)
            End If

            Facade.TableLogFacade.Insert("ContainerGroup", LogEvent.UP, String.Format("PreModifica Gruppo {0}: Rights {1}, ResolutionRights {2}, DocumentRights {3}, DocumentSeriesRights {4}, DeskRights {5},  UDSRights {6}", containerGroup.Name, containerGroup.ProtocolRightsString, containerGroup.ResolutionRights, containerGroup.DocumentRights, containerGroup.DocumentSeriesRights, containerGroup.DeskRights, containerGroup.UDSRights), containerGroup.Container.UniqueId)

            If Not String.Equals(containerGroup.PrivacyLevel.ToString, ddlPrivacy.SelectedValue) Then
                FacadeFactory.Instance.TableLogFacade.Insert("ContainerGroup", LogEvent.PR, String.Format("Modificato il livello {0} del Gruppo {1} con il livello {1}", PRIVACY_LABEL, containerGroup.UniqueId, containerGroup.PrivacyLevel), containerGroup.Id)
            End If

            If DocSuiteContext.Current.PrivacyLevelsEnabled AndAlso CurrentContainer.PrivacyEnabled AndAlso containerGroup IsNot Nothing AndAlso MaxValue < containerGroup.PrivacyLevel Then
                AjaxAlert(String.Concat("Attenzione: errore nelle configurazioni dei livelli di", PRIVACY_LABEL, ". Il livello del gruppo e' superiore a quelli permessi."))
                Exit Sub
            End If

            containerGroup = PrepareContainer(containerGroup)
            Facade.ContainerGroupFacade.UpdateOnly(containerGroup)



            InitializeGroups(uscGruppi.SelectedNode.Text)
        End If

        uscGruppi.Refresh()

        AjaxManager.ResponseScripts.Add("UpdateGroups();")

    End Sub

    Private Sub btnCopiaClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnCopia.Click
        Dim containerGroup As Object = uscGruppi.CurrentGroups
        Dim currentContainerGroup As Object = CurrentContainer.ContainerGroups
        LoadContainers()
        AjaxManager.ResponseScripts.Add("OpenContenitoriCopyWindow();")
    End Sub

    Private Sub UscGruppiNodeClick(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadTreeNodeEventArgs) Handles uscGruppi.NodeClick
        Dim group As ContainerGroup = CType(uscGruppi.GetGroup(e.Node.Text), ContainerGroup)
        If (group Is Nothing) Then
            InitializeGroups("")
        Else
            InitializeGroups(group.Name)
        End If
    End Sub

    Private Sub UscGruppiNodeRemove(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadTreeNodeEventArgs) Handles uscGruppi.NodeRemove
        InitializeRightsPanel(e.Node)
        Facade.TableLogFacade.Insert("ContainerGroup", LogEvent.DL, String.Format("Eliminato Gruppo {0}", e.Node.Text), CurrentContainer.UniqueId)
        AjaxManager.ResponseScripts.Add("UpdateGroups();")
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeControls()
        MasterDocSuite.TitleVisible = False

        uscGruppi.CurrentGroups = CurrentContainer.ContainerGroups
        uscGruppi.ContainerName = CurrentContainer.Name
        uscGruppi.CurrentGroupFacade = Facade.ContainerGroupFacade
        uscGruppi.GroupName = GroupName
        uscGruppi.ContainerImageURL = ImagePath.SmallBoxOpen
    End Sub

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf TbltContenitoriGesGruppi_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(uscGruppi, pnlDiritti)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlDiritti)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscGruppi, pnlPrivacy)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlPrivacy)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscGruppi, btnConfermaDiritti)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, btnConfermaDiritti)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConfermaDiritti, uscGruppi)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConfirmCopyContainer, windowContainersCopy)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnCopia, rtvContainersCopy)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConfirmCopyContainer, rtvContainersCopy)
        AjaxManager.AjaxSettings.AddAjaxSetting(windowContainersCopy, rtvContainersCopy)

    End Sub

    Private Sub InitializeLocations()
        'Recupero della locazione
        If DocSuiteContext.Current.IsResolutionEnabled Then
            _reslEnable = Not (CurrentContainer.ReslLocation Is Nothing)
            lblResl.Text = Facade.TabMasterFacade.TreeViewCaption
        End If
    End Sub

    Private Sub InitializeRights()

        InitializeProtocolRight(DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled)


        'Abilitazione Pannello diritti Pratiche
        lblDocm.Visible = DocSuiteContext.Current.IsDocumentEnabled
        cblDocm.Visible = DocSuiteContext.Current.IsDocumentEnabled
        If DocSuiteContext.Current.IsDocumentEnabled Then
            InitializeDocumentRight()
        End If

        'Abilitazione Pannello diritti Atti
        lblResl.Visible = ResolutionEnable
        cblResl.Visible = ResolutionEnable
        If ResolutionEnable Then
            InitializeResolutionRight()
        End If

        'Abilitazione Pannello diritti Atti
        lblSeries.Visible = ProtocolEnv.DocumentSeriesEnabled
        cblSeries.Visible = ProtocolEnv.DocumentSeriesEnabled
        If ProtocolEnv.DocumentSeriesEnabled Then
            InitializeSeriesRight()
        End If

        'Abilitazione Pannello diritti Tavoli
        lblDesks.Visible = ProtocolEnv.DeskEnable AndAlso CurrentContainer.DeskLocation IsNot Nothing
        cblDesks.Visible = ProtocolEnv.DeskEnable AndAlso CurrentContainer.DeskLocation IsNot Nothing
        If ProtocolEnv.DeskEnable AndAlso CurrentContainer.DeskLocation IsNot Nothing Then
            InitializeDeskRight()
        End If

        lblUDS.Visible = CurrentContainer.UDSLocation IsNot Nothing
        cblUDS.Visible = CurrentContainer.UDSLocation IsNot Nothing
        If CurrentContainer.UDSLocation IsNot Nothing Then
            InitializeUDSRight()
        End If

        Dim fascicleRightsManageable As Boolean = ProtocolEnv.FascicleContainerEnabled
        lblFascicles.Visible = fascicleRightsManageable
        cblFascicles.Visible = fascicleRightsManageable
        If fascicleRightsManageable Then
            InitializeFascicleRight()
        End If

        pnlDiritti.Visible = False
        pnlPrivacy.Visible = False
        btnConfermaDiritti.Visible = False

        If ShowReadonlySecurityGroups Then
            SetReadonlyCheckboxes()
        End If

    End Sub
    Private Sub SetReadonlyCheckboxes()
        For Each listItem As ListItem In cblProt.Items
            listItem.Enabled = False
        Next

        For Each listItem As ListItem In cblDocm.Items
            listItem.Enabled = False
        Next

        For Each listItem As ListItem In cblResl.Items
            listItem.Enabled = False
        Next

        For Each listItem As ListItem In cblSeries.Items
            listItem.Enabled = False
        Next

        For Each listItem As ListItem In cblDesks.Items
            listItem.Enabled = False
        Next

        For Each listItem As ListItem In cblUDS.Items
            listItem.Enabled = False
        Next

        For Each listItem As ListItem In cblFascicles.Items
            listItem.Enabled = False
        Next
    End Sub

    Private Sub InitializeProtocolRight(distributionEnabled As Boolean)
        cblProt.Items.Clear()
        Dim text As String
        For Each val As ProtocolContainerRightPositions In [Enum].GetValues(GetType(ProtocolContainerRightPositions))
            If (val = ProtocolContainerRightPositions.DocDistribution AndAlso Not distributionEnabled) OrElse
               (val = ProtocolContainerRightPositions.Privacy AndAlso Not DocSuiteContext.Current.SimplifiedPrivacyEnabled) Then
                Continue For
            End If
            If Not ProtocolEnv.IsInteropEnabled AndAlso (val = ProtocolContainerRightPositions.InteropIn OrElse val = ProtocolContainerRightPositions.InteropOut) Then
                Continue For
            End If
            If Not ProtocolEnv.IsPECEnabled AndAlso val = ProtocolContainerRightPositions.PECIn Then
                Continue For
            End If
            If val = ProtocolContainerRightPositions.PECOut AndAlso Not ProtocolEnv.IsPECEnabled AndAlso Not ProtocolEnv.TNoticeEnabled Then
                Continue For
            End If
            text = val.GetDescription()
            If val = ProtocolContainerRightPositions.PECOut AndAlso ProtocolEnv.TNoticeEnabled Then
                text = If(ProtocolEnv.IsPECEnabled, $"{val.GetDescription()} & Invio TNotice", "Invio TNotice")
            End If
            AddItem(cblProt, text, val, False)
        Next
    End Sub

    Private Sub InitializeDocumentRight()
        cblDocm.Items.Clear()
        For Each val As DocumentRightPositions In [Enum].GetValues(GetType(DocumentRightPositions))
            AddItem(cblDocm, val.GetDescription(), val, False)
        Next
    End Sub

    Private Sub InitializeResolutionRight()
        cblResl.Items.Clear()
        For Each val As ResolutionRightPositions In [Enum].GetValues(GetType(ResolutionRightPositions))
            AddItem(cblResl, val.GetDescription(), val, False)
        Next
    End Sub

    Private Sub InitializeSeriesRight()
        cblSeries.Items.Clear()
        For Each val As DocumentSeriesContainerRightPositions In [Enum].GetValues(GetType(DocumentSeriesContainerRightPositions))
            AddItem(cblSeries, val.GetDescription(), val, False)
        Next
    End Sub

    Private Sub InitializeUDSRight()
        cblUDS.Items.Clear()
        For Each val As UDSRightPositions In [Enum].GetValues(GetType(UDSRightPositions))
            AddItem(cblUDS, val.GetDescription(), val, False)
        Next
    End Sub

    Private Sub InitializeDeskRight()
        cblDesks.Items.Clear()

        AddItem(cblDesks, DeskRightPositions.Insert.GetDescription(), DeskRightPositions.Insert, False)

        Dim results As New List(Of DeskRightPositions)()
        results.Add(DeskRightPositions.Modify)
        results.Add(DeskRightPositions.ViewDocuments)
        results.Add(DeskRightPositions.Read)
        results.Add(DeskRightPositions.Delete)
        results.Add(DeskRightPositions.Close)
        results.Add(DeskRightPositions.Collaboration)

        For Each right As DeskRightPositions In results
            AddItem(cblDesks, right.GetDescription(), right, False, False)
        Next

    End Sub

    Private Sub InitializeFascicleRight()
        cblFascicles.Items.Clear()
        For Each val As FascicleRightPosition In [Enum].GetValues(GetType(FascicleRightPosition))
            AddItem(cblFascicles, val.GetDescription(), val, False)
        Next
    End Sub

    Private Sub InitializeGroups(ByVal name As String)
        InitializeRightsPanel(uscGruppi.SelectedNode)

        If CurrentContainer.Id = 0 Then
            Exit Sub
        End If

        ClearRightsPanel()
        If String.IsNullOrEmpty(name) Then
            Exit Sub
        End If

        _currentContainer = Nothing
        uscGruppi.CurrentGroups = CurrentContainer.ContainerGroups
        Dim containerGroup As ContainerGroup = CType(uscGruppi.GetGroup(name), ContainerGroup)
        If (containerGroup Is Nothing) Then
            Throw New DocSuiteException("Contenitori", "Errore in ricerca", Request.Url.ToString(), DocSuiteContext.Current.User.FullUserName)
        End If

        If CommonInstance.DocmEnabled And (CurrentContainer.DocmLocation IsNot Nothing) Then
            InitializeRightValues(cblDocm, containerGroup.DocumentRights)
        End If

        If CommonInstance.ProtEnabled And (CurrentContainer.ProtLocation IsNot Nothing) Then
            InitializeRightValues(cblProt, containerGroup.ProtocolRightsString)
        End If

        If CommonInstance.ReslEnabled And (CurrentContainer.ReslLocation IsNot Nothing) Then
            InitializeRightValues(cblResl, containerGroup.ResolutionRights)
        End If

        If CommonInstance.ProtEnabled And (CurrentContainer.DocumentSeriesLocation IsNot Nothing OrElse CurrentContainer.DocumentSeriesAnnexedLocation IsNot Nothing OrElse CurrentContainer.DocumentSeriesUnpublishedAnnexedLocation IsNot Nothing) Then
            InitializeRightValues(cblSeries, containerGroup.DocumentSeriesRights)
        End If

        If CommonInstance.ProtEnabled AndAlso CurrentContainer.DeskLocation IsNot Nothing Then
            InitializeRightValues(cblDesks, containerGroup.DeskRights)
        End If

        If CommonInstance.ProtEnabled AndAlso CurrentContainer.UDSLocation IsNot Nothing Then
            InitializeRightValues(cblUDS, containerGroup.UDSRights)
        End If

        If CommonInstance.ProtEnabled AndAlso ProtocolEnv.FascicleContainerEnabled Then
            InitializeRightValues(cblFascicles, containerGroup.FascicleRights)
        End If

        If CurrentContainer.PrivacyEnabled Then
            ddlPrivacy.SelectedValue = CurrentContainer.PrivacyLevel.ToString()
            If CurrentContainer.PrivacyLevel <= containerGroup.PrivacyLevel Then
                ddlPrivacy.SelectedValue = containerGroup.PrivacyLevel.ToString()
            End If
        End If
    End Sub

    Private Shared Sub InitializeRightValues(checkBoxList As CheckBoxList, rights As String)
        For Each item As ListItem In checkBoxList.Items
            item.Selected = GetDiritti(rights, CType(item.Value, Integer))
        Next
    End Sub

    Private Shared Function GetDiritti(ByVal field As String, ByVal right As Integer) As Boolean
        Return Mid$(field, right, 1).Eq("1")
    End Function

    ''' <summary> Imposta la visibilità del pannello dei diritti </summary>
    Private Sub InitializeRightsPanel(ByVal node As RadTreeNode)
        Dim nodeVisibility As Boolean = Not node Is Nothing AndAlso Not node.ParentNode Is Nothing
        pnlDiritti.Visible = nodeVisibility
        btnConfermaDiritti.Visible = nodeVisibility AndAlso Not ShowReadonlySecurityGroups
        If nodeVisibility AndAlso DocSuiteContext.Current.PrivacyLevelsEnabled AndAlso CurrentContainer.PrivacyEnabled Then
            pnlPrivacy.Visible = nodeVisibility
        End If
    End Sub

    Private Shared Sub AddItem(ByVal cbl As CheckBoxList, ByVal text As String, value As Integer, ByVal selected As Boolean, Optional enabled As Boolean = True)
        Dim li As New ListItem
        li.Value = value.ToString()
        li.Text = text
        li.Selected = selected
        li.Enabled = enabled
        cbl.Items.Add(li)
    End Sub

    Private Sub ClearRightsPanel()
        Dim showPanel As Boolean

        'Inizializzazione pannello Pratiche
        showPanel = (CurrentContainer.DocmLocation IsNot Nothing)
        lblDocm.Visible = showPanel
        cblDocm.Visible = showPanel
        For Each li As ListItem In cblDocm.Items
            li.Selected = False
        Next li

        'Inizializzazione pannello Protocollo
        showPanel = (CurrentContainer.ProtLocation IsNot Nothing)
        lblProt.Visible = showPanel
        cblProt.Visible = showPanel
        For Each li As ListItem In cblProt.Items
            li.Selected = False
        Next li

        'Inizializzazione pannello Atti
        showPanel = (CurrentContainer.ReslLocation IsNot Nothing)
        lblResl.Visible = showPanel
        cblResl.Visible = showPanel
        For Each li As ListItem In cblResl.Items
            li.Selected = False
        Next li

        'Inizializzazione pannello Serie Documentali
        showPanel = (CurrentContainer.DocumentSeriesLocation IsNot Nothing OrElse
                     CurrentContainer.DocumentSeriesAnnexedLocation IsNot Nothing OrElse
                     CurrentContainer.DocumentSeriesUnpublishedAnnexedLocation IsNot Nothing)
        lblSeries.Visible = showPanel
        cblSeries.Visible = showPanel
        For Each li As ListItem In cblSeries.Items
            li.Selected = False
        Next li

        'Inizializzazione pannello Archivi
        showPanel = (CurrentContainer.UDSLocation) IsNot Nothing
        lblUDS.Visible = showPanel
        cblUDS.Visible = showPanel
        For Each li As ListItem In cblUDS.Items
            li.Selected = False
        Next li

        showPanel = (CurrentContainer.DeskLocation) IsNot Nothing
        lblDesks.Visible = showPanel
        cblDesks.Visible = showPanel
        For Each li As ListItem In cblDesks.Items
            li.Selected = False
        Next li

    End Sub

    Private Function CheckPageHasRightsSelected() As Boolean
        Dim protHasValue As Boolean = cblProt.Items.OfType(Of ListItem)().Any(Function(x) x.Selected)
        Dim reslHasValue As Boolean = cblResl.Items.OfType(Of ListItem)().Any(Function(x) x.Selected)
        Dim seriesHasValue As Boolean = cblSeries.Items.OfType(Of ListItem)().Any(Function(x) x.Selected)
        Dim docmHasValue As Boolean = cblDocm.Items.OfType(Of ListItem)().Any(Function(x) x.Selected)
        Dim deskHasValue As Boolean = cblDesks.Items.OfType(Of ListItem)().Any(Function(x) x.Selected)
        Dim udsHasValue As Boolean = cblUDS.Items.OfType(Of ListItem)().Any(Function(x) x.Selected)
        Dim fascicleHasValue As Boolean = cblFascicles.Items.OfType(Of ListItem)().Any(Function(x) x.Selected)

        If protHasValue OrElse reslHasValue OrElse seriesHasValue OrElse docmHasValue OrElse deskHasValue OrElse udsHasValue OrElse fascicleHasValue Then
            Return True
        End If

        Return False
    End Function

    Private Function PrepareContainer(containerGroup As ContainerGroup) As ContainerGroup
        containerGroup.ProtocolRightsString = CurrentProtocolRightsSelected
        containerGroup.DocumentRights = CurrentDocmRightsSelected
        containerGroup.ResolutionRights = CurrentResolutionRightsSelected
        containerGroup.DocumentSeriesRights = CurrentSeriesRightsSelected
        containerGroup.DeskRights = CurrentDeskRightsSelected
        containerGroup.UDSRights = CurrentUDSRightsSelected
        containerGroup.FascicleRights = CurrentFascicleRightsSelected
        containerGroup.Name = uscGruppi.SelectedNode.Text
        containerGroup.PrivacyLevel = Int16.Parse(ddlPrivacy.SelectedValue)
        Return containerGroup
    End Function

    Private Sub InitializePrivacy()
        lblPrivacy.Text = String.Concat("Livello di ", PRIVACY_LABEL, ": ")

        Dim privacyLevels As IList(Of APIEntity.PrivacyLevel) = New List(Of APIEntity.PrivacyLevel)

        If AllowedLevels.Count() = 0 Then
            AjaxAlert(String.Concat("Attenzione: errore nelle configurazioni dei livelli di ", PRIVACY_LABEL, ". Il livello del contenitore e' superiore a quelli permessi."))
            Exit Sub
        End If

        For Each PrivacyLevelDTO As WebAPIDto(Of APIEntity.PrivacyLevel) In AllowedLevels
            privacyLevels.Add(PrivacyLevelDTO.Entity)
        Next
        Dim item As ListItem
        For Each level As APIEntity.PrivacyLevel In privacyLevels
            item = New ListItem()
            item.Text = level.Description.ToString()
            item.Value = level.Level.ToString()
            ddlPrivacy.Items.Add(item)
        Next
    End Sub
#End Region

End Class