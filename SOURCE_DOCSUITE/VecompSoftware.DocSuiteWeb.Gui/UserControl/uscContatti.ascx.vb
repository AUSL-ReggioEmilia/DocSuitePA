Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Entity.Tenants
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Commons
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Partial Public Class uscContatti
    Inherits DocSuite2008BaseControl

#Region " Fields "

    Private _showAll As Boolean
    Private _currentContactListFacade As ContactListFacade
    Private _categoryFascicleRightFacade As CategoryFascicleRightFacade
    Private _currentCategoryFascicleRightRoles As IList(Of Integer)
    Private _roleContactIds As List(Of Integer)
    Private _excluded As List(Of Integer)
    Private Const RoleEnabledContactTag As String = "IsRoleContact"

#End Region

#Region " Properties "

    ''' <summary> Contatto da precaricare nel dettaglio. </summary>
    ''' <remarks> 
    ''' TODO: Refactoring! non si accede alla querystring da un controllo 
    ''' </remarks>
    Private ReadOnly Property IdContact As String
        Get
            Return Request.QueryString("idContact")
        End Get
    End Property

    ''' <summary>  </summary>
    ''' <remarks> 
    ''' TODO: Refactoring! non si accede alla querystring da un controllo 
    ''' </remarks>
    Private ReadOnly Property DefaultDescription As String
        Get
            Return Request.QueryString("DefaultDescr")
        End Get
    End Property

    ''' <summary> Radice dei contatti da caricare </summary>
    ''' <remarks> 
    ''' TODO: Refactoring! non si accede alla querystring da un controllo 
    ''' </remarks>
    Private ReadOnly Property ContactRoot() As String
        Get
            Return Request.QueryString("ContactRoot")
        End Get
    End Property

    ''' <summary> Contatti da escludere nel caricamento </summary>
    Public Property ExcludeContact() As List(Of Integer)
        Get
            Dim exclContact As List(Of Integer) = CType(ViewState("ExcludeContact"), List(Of Integer))
            Return If(exclContact.IsNullOrEmpty(), New List(Of Integer), exclContact)
        End Get
        Set(ByVal value As List(Of Integer))
            If value IsNot Nothing AndAlso value.Count() > 0 Then
                ViewState("ExcludeContact") = value
            Else
                ViewState("ExcludeContact") = New List(Of Integer)
            End If
        End Set
    End Property
    '''<summary> Nodi di settore da escludere nella ricerca </summary>
    Public Property ExcludeRoleRoot() As Boolean?
        Get
            If ViewState("ExcludeRoleRoot") Is Nothing Then
                ViewState("ExcludeRoleRoot") = False
            End If
            Return CType(ViewState("ExcludeRoleRoot"), Boolean?)
        End Get
        Set(ByVal value As Boolean?)
            ViewState("ExcludeRoleRoot") = value
        End Set
    End Property

    ''' <summary> Imposta la visualizzazione dell'albero dei contatti manuali. </summary>
    Public Property ShowAddressBook() As Boolean
        Get
            If ViewState("ShowAddressBook") Is Nothing Then
                ViewState("ShowAddressBook") = True
            End If
            Return CType(ViewState("ShowAddressBook"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            ViewState("ShowAddressBook") = value
        End Set
    End Property

    ''' <summary> Attiva la visualizzazione dei dettagli. </summary>
    Public Property ShowDetails() As Boolean
        Get
            If ViewState("ShowDetails") Is Nothing Then
                ViewState("ShowDetails") = True
            End If
            Return CType(ViewState("ShowDetails"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            ViewState("ShowDetails") = value
        End Set
    End Property

    ''' <summary> Attiva la modifica della rubrica. </summary>
    Public Property EditMode() As Boolean
        Get
            If ViewState("EditMode") Is Nothing Then
                ViewState("EditMode") = False
            End If
            Return CType(ViewState("EditMode"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            ViewState("EditMode") = value
        End Set
    End Property

    ''' <summary> Mostra o meno la sezione di ricerca. </summary>
    Public Property SearchMode() As Boolean
        Get
            If ViewState("SearchMode") Is Nothing Then
                ViewState("SearchMode") = False
            End If
            Return CType(ViewState("SearchMode"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            ViewState("SearchMode") = value
        End Set
    End Property
    ''' <summary> Mostra o meno la sezione di ricerca in Rubrica. </summary>
    Public Property SearchinRubrica() As Boolean
        Get
            If ViewState("SearchinRubrica") Is Nothing Then
                ViewState("SearchinRubrica") = False
            End If
            Return CType(ViewState("SearchinRubrica"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            ViewState("SearchinRubrica") = value
        End Set
    End Property

    Public Property ShowAll() As Boolean
        Get
            Return _showAll
        End Get
        Set(value As Boolean)
            _showAll = value
        End Set
    End Property

    ''' <summary> Attiva la selezione multipla dei ruoli. </summary>
    Public Property MultiSelect() As Boolean
        Get
            If ViewState("MultiSelect") Is Nothing Then
                ViewState("MultiSelect") = False
            End If
            Return CType(ViewState("MultiSelect"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            ViewState("MultiSelect") = value
        End Set
    End Property

    Public Property ConfermaSelezioneVisible() As Boolean
        Get
            If ViewState("ConfermaSelezioneVisible") Is Nothing Then
                ViewState("ConfermaSelezioneVisible") = True
            End If
            Return CType(ViewState("ConfermaSelezioneVisible"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            ViewState("ConfermaSelezioneVisible") = value
        End Set
    End Property

    Public Property AVCPBusinessContactEnabled As Boolean

    Public Property FascicleContactEnabled As Boolean

    Public ReadOnly Property ExcludeParentId As List(Of Integer)
        Get
            If (ProtocolEnv.AVCPIdBusinessContact > 0 AndAlso Not AVCPBusinessContactEnabled) Then
                Dim parents As List(Of Integer) = New List(Of Integer) From {ProtocolEnv.AVCPIdBusinessContact}
                Return parents
            End If
            Return New List(Of Integer)()
        End Get
    End Property

    Public ReadOnly Property OnlyParentId As Integer?
        Get
            If (ProtocolEnv.AVCPIdBusinessContact > 0 AndAlso AVCPBusinessContactEnabled) Then
                Return ProtocolEnv.AVCPIdBusinessContact
            ElseIf (ProtocolEnv.FascicleContactId > 0 AndAlso FascicleContactEnabled) Then
                Return ProtocolEnv.FascicleContactId
            End If
        End Get
    End Property
    Private ReadOnly Property CurrentCategoryFascicleRightFacade As CategoryFascicleRightFacade
        Get
            If _categoryFascicleRightFacade Is Nothing Then
                _categoryFascicleRightFacade = New CategoryFascicleRightFacade()
            End If
            Return _categoryFascicleRightFacade
        End Get
    End Property

    Private ReadOnly Property CurrentCategoryFascicleRightRoles As IList(Of Integer)
        Get
            If _currentCategoryFascicleRightRoles Is Nothing Then
                If SearchInCategoryContacts.HasValue Then
                    _currentCategoryFascicleRightRoles = CurrentCategoryFascicleRightFacade.GetByIdCategoryRoleRestricted(SearchInCategoryContacts.Value).Select(Function(f) f.Role.Id).ToList()
                End If
                If SearchInRoleContacts.HasValue AndAlso _currentCategoryFascicleRightRoles Is Nothing Then
                    _currentCategoryFascicleRightRoles = New List(Of Integer)
                End If

                If SearchInRoleContacts.HasValue AndAlso Not _currentCategoryFascicleRightRoles.Contains(SearchInRoleContacts.Value) Then
                    _currentCategoryFascicleRightRoles.Add(SearchInRoleContacts.Value)
                End If
            End If
            Return _currentCategoryFascicleRightRoles
        End Get
    End Property

    Public ReadOnly Property CurrentContactListFacade As ContactListFacade
        Get
            If _currentContactListFacade Is Nothing Then
                _currentContactListFacade = New ContactListFacade()
            End If
            Return _currentContactListFacade
        End Get
    End Property

    Public Property EnableFlagGroupChild As Boolean = False

    Public ReadOnly Property RoleContactIds As List(Of Integer)
        Get
            If _roleContactIds Is Nothing Then
                _roleContactIds = FacadeFactory.Instance.ContactFacade.GetContactMyRoles()
            End If
            Return _roleContactIds
        End Get
    End Property

    Public ReadOnly Property ExcludedContacts() As List(Of Integer)
        Get
            If _excluded IsNot Nothing AndAlso _excluded.Count > 0 Then
                Return _excluded
            End If
            _excluded = ExcludeContact
            _excluded.AddRange(ExcludeParentId)
            Return _excluded
        End Get
    End Property

    Public Property SearchInCategoryContacts As Integer?

    Public Property CategoryContactsProcedureType As String

    Public Property SearchInRoleContacts As Integer?

    Public Property RoleContactsProcedureType As String

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()

        If Not IsPostBack AndAlso Not Page.IsCallback Then
            Initialize()
            AVCPBusinessContactEnabled = False
        End If
    End Sub

    Protected Sub uscContatti_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arguments As String() = e.Argument.Split("|"c)
        If Not arguments(0).Eq(ClientID) Then
            Exit Sub
        End If

        Select Case arguments(1)
            Case "Add"
                If String.IsNullOrEmpty(arguments(2)) Then
                    Exit Sub
                End If

                Dim contact As Contact = Facade.ContactFacade.GetById(Integer.Parse(arguments(2)), False)
                AddContact(contact)
                SetButtons(contactTree.SelectedNode)

            Case "Rename"
                Dim tn As RadTreeNode = contactTree.SelectedNode
                If (tn Is Nothing) Then
                    Exit Sub
                End If

                Dim contact As Contact = Facade.ContactFacade.GetById(Integer.Parse(tn.Value), False)

                contactTree.SelectedNode.Text = contact.FullDescription
                contactTree.SelectedNode.ImageUrl = ImagePath.ContactTypeIcon(contact.ContactType.Id, contact.isLocked.HasValue AndAlso contact.isLocked.Value = 1)

                InitializeDetails(contact)
                SetButtons(contactTree.SelectedNode)
            Case "Move"
                Dim tn As RadTreeNode = contactTree.SelectedNode
                Dim destIdx As Integer
                If (tn Is Nothing OrElse String.IsNullOrEmpty(arguments(2)) OrElse Not Integer.TryParse(arguments(2), destIdx)) Then
                    Exit Sub
                End If

                Dim contact As Contact = Facade.ContactFacade.GetById(Integer.Parse(tn.Value))
                Dim destination As Contact = Facade.ContactFacade.GetById(destIdx)
                Facade.ContactFacade.Move(contact, destination)
                Search()
            Case "Del"
                Dim tn As RadTreeNode = contactTree.SelectedNode
                If (tn Is Nothing) Then
                    Exit Sub
                End If

                tn.Style.Add("color", "gray")
                tn.Attributes("Recovery") = "true"
                tn.Nodes.Clear()
                SelectNode(tn)

            Case "Recovery"
                Dim tn As RadTreeNode = contactTree.SelectedNode
                If tn Is Nothing Then
                    Exit Sub
                End If

                Dim contact As Contact = Facade.ContactFacade.GetById(Integer.Parse(tn.Value))
                tn.Style.Remove("color")
                tn.Attributes("Recovery") = "false"
                If contact.IsChanged.Equals(Convert.ToInt16(True)) Then
                    tn.Style.Add("color", "green")
                    tn.Attributes.Add("Changed", "true")
                End If
                tn.Nodes.Clear()
                SelectNode(tn)

            Case "reload", "clone"
                txtCerca.Focus()
                txtCerca.Text = ""
                txtSearchCode.Text = ""
                Search()

            Case "Modify"
                BasePage.AjaxAlert("Caso non previsto, contattare l'assitenza.")

        End Select

    End Sub

    Protected Sub BtnCercaClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearch.Click
        Search()
    End Sub
    Protected Sub BtnCercaClickRubrica(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearchTbltRubrica.Click
        Search()
    End Sub


    Protected Sub BtnSearchCodeClick(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnSearchCode.Click
        If Not String.IsNullOrEmpty(txtSearchCode.Text) Then
            If txtSearchCode.Text.Split("."c).Length <= 0 Then
                BasePage.AjaxAlert("Codice non valido.")
                Exit Sub
            End If
        Else
            contactTree.Nodes(0).Nodes.Clear()
            SelectNode(contactTree.Nodes(0))
        End If

        Dim contactListId As Guid? = Nothing
        If ProtocolEnv.ContactListsEnabled AndAlso pnlContactListFilter.Visible AndAlso Not String.IsNullOrEmpty(ddlContactLists.SelectedValue) AndAlso Not Guid.Parse(ddlContactLists.SelectedValue).Equals(Guid.Empty) Then
            contactListId = Guid.Parse(ddlContactLists.SelectedValue)
        End If

        ' Get current user's tenant
        Dim currentUserTenant As Tenant = GetCurrentTenant()

        Dim contactList As IList(Of Contact) = Facade.ContactFacade.GetContactBySearchCode(txtSearchCode.Text, 1S,
                                                                                           categoryFascicleRightRoles:=CurrentCategoryFascicleRightRoles,
                                                                                           excludeParentIds:=ExcludedContacts, onlyParentId:=OnlyParentId, contactListId:=contactListId,
                                                                                           procedureType:=CategoryContactsProcedureType, idRole:=SearchInRoleContacts, roleType:=RoleContactsProcedureType, currentTenantId:=currentUserTenant?.UniqueId)
        If (contactList.Count = 0) Then
            BasePage.AjaxAlert("Codice inesistente.")
            txtSearchCode.Focus()
            txtSearchCode.Attributes.Add("onfocusin", " select();")

        ElseIf (contactList.Count > 1) Then
            BasePage.AjaxAlert("Il Codice non è univoco.")
            txtSearchCode.Focus()
            txtSearchCode.Attributes.Add("onfocusin", " select();")

        Else
            AjaxManager.ResponseScripts.Add(String.Format("ReturnCodeValue('{0}');", contactList(0).Id.ToString()))

        End If
    End Sub

    Protected Sub ContactTreeNodeExpand(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs) Handles contactTree.NodeExpand
        SelectNode(e.Node)
    End Sub

    Protected Sub ContactTreeNodeClick(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs) Handles contactTree.NodeClick
        SelectNode(e.Node)
    End Sub

    Protected Sub ContactTreeContextMenuItemClick(ByVal sender As Object, ByVal e As RadTreeViewContextMenuEventArgs) Handles contactTree.ContextMenuItemClick
        InitializeDetails(e.Node.Value)
    End Sub

    Private Sub BtnGestioneClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnGestione.Click
        EditMode = Not EditMode
        For Each nd As RadTreeNode In contactTree.GetAllNodes
            nd.EnableContextMenu = EditMode
        Next

        SetButtons(contactTree.SelectedNode)
    End Sub

    Private Sub BtnSbloccaClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnSblocca.Click
        contactTree.Nodes(0).Nodes.Clear()
        LoadContacts(contactTree.Nodes(0))
    End Sub

    Private Sub ToolBar_ButtonClick(sender As Object, e As RadToolBarEventArgs) Handles ToolBar.ButtonClick
        Dim btnToolbar As RadToolBarButton = DirectCast(e.Item, RadToolBarButton)
        If btnToolbar.Group.Eq("selection") Then
            Dim nodes As RadTreeNodeCollection = If(contactTree.SelectedNode IsNot Nothing, contactTree.SelectedNode.Nodes, contactTree.Nodes(0).Nodes)
            CheckNodes(nodes, btnToolbar.CommandName.Eq("checkAllContacts"))
        End If
    End Sub

    Private Sub GroupNodeChecked(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs) Handles contactTree.NodeCheck
        If MultiSelect AndAlso EnableFlagGroupChild Then
            Dim tmpId As Integer
            If Not Integer.TryParse(e.Node.Value, tmpId) Then
                Exit Sub
            End If

            Dim contact As Contact = Facade.ContactFacade.GetById(tmpId, False)
            If contact Is Nothing Then
                Exit Sub
            End If

            If (contact.ContactType.Id = ContactType.Group) Then
                SelectNode(e.Node)
                If (e.Node.Nodes.Count > 0) Then
                    e.Node.CheckChildNodes()
                    CheckChildren(e.Node)
                End If
            End If
        End If
    End Sub

    Private Sub ddlContactLists_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlContactLists.SelectedIndexChanged
        LoadContactsFromList()
    End Sub
#End Region

#Region " Methods "

    Private Sub InitializeAjax()

        AjaxManager.AjaxSettings.AddAjaxSetting(ToolBar, contactTree)

        AjaxManager.AjaxSettings.AddAjaxSetting(contactTree, ToolBar)
        AjaxManager.AjaxSettings.AddAjaxSetting(contactTree, contactTree)
        AjaxManager.AjaxSettings.AddAjaxSetting(contactTree, btnGestione)

        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, contactSplitter)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearchTbltRubrica, contactSplitter)

        AjaxManager.AjaxSettings.AddAjaxSetting(btnGestione, ToolBar)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnGestione, contactTree)

        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearchCode, pnlSearchCode)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearchCode, ToolBar)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearchCode, contactTree)

        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlSearch)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlSearchCode)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, contactTree)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, ToolBar)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, btnGestione)
        AjaxManager.AjaxSettings.AddAjaxSetting(contactTree, AjaxManager)

        AjaxManager.AjaxSettings.AddAjaxSetting(btnSblocca, contactTree)

        If ShowDetails Then
            AjaxManager.AjaxSettings.AddAjaxSetting(contactTree, contactDetailAjaxPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnSearchCode, contactDetailAjaxPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, contactDetailAjaxPanel)
        End If

        If ProtocolEnv.ContactListsEnabled AndAlso SearchMode Then
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlSearchList)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, ddlContactLists)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlContactLists, contactSplitter)
        End If

        AddHandler AjaxManager.AjaxRequest, AddressOf uscContatti_AjaxRequest
    End Sub

    Private Sub Initialize()

        If SearchinRubrica Then
            pnlSearchDescription.Visible = True
        Else
            pnlSearchDescription.Visible = False
        End If

        If Not SearchMode Then
            footerPane.Collapsed = True
            headerPane.Collapsed = True
        ElseIf Not String.IsNullOrEmpty(DefaultDescription) Then
            txtCerca.Focus()
            txtCerca.Text = DefaultDescription
            Search()
        End If

        ' Visualizzo solo albero dei contatti
        If Not ShowDetails Then
            contactDetailPane.Visible = False
            contactSplitBar.Visible = False
        End If

        ' Visualizzo solo dettagli
        If Not ShowAddressBook Then
            contactTreePane.Visible = False
            contactSplitBar.Visible = False
            InitializeDetails(IdContact)
        End If

        headerPane.Height = Unit.Pixel(55)
        mainPane.Height = Unit.Percentage(100)
        If ProtocolEnv.ContactListsEnabled AndAlso SearchMode Then
            pnlContactListFilter.Visible = True
            Dim contactLists As IList(Of ContactList) = CurrentContactListFacade.GetAll()

            ddlContactLists.DataSource = contactLists
            ddlContactLists.DataBind()
            ddlContactLists.Items.Insert(0, New ListItem(String.Empty, String.Empty))

            headerPane.Height = Unit.Pixel(80)
        End If

        chbContiene.Checked = DocSuiteContext.Current.ProtocolEnv.ContattiDefaultContiene


        If String.IsNullOrEmpty(ContactRoot) Then
            LoadContacts(contactTree.Nodes(0))
            WebUtils.ObjAttDisplayNone(btnSblocca)
        Else
            PopulateTreeViewContacts(Facade.ContactFacade.GetContactByParentId(Integer.Parse(ContactRoot), EditMode,
                                                                               categoryFascicleRightRoles:=CurrentCategoryFascicleRightRoles, excludeParentIds:=ExcludedContacts,
                                                                               onlyParentId:=OnlyParentId, procedureType:=CategoryContactsProcedureType,
                                                                               idRole:=SearchInRoleContacts, roleType:=RoleContactsProcedureType).ToList())
            If Not CommonShared.HasGroupProposerFullRight Then
                WebUtils.ObjAttDisplayNone(btnSblocca)
            End If
        End If

        btnConfermaNuovo.Visible = MultiSelect

        If Not MultiSelect Then
            contactTree.CheckBoxes = False
            contactTree.MultipleSelect = False
            btnConferma.OnClientClicked = "btnConferma_OnClick"
        Else
            contactTree.CheckBoxes = True
            btnConferma.OnClientClicked = "btnConfermaMulti_OnClick"
        End If
        btnConferma.Visible = ConfermaSelezioneVisible

        SetButtons(contactTree.SelectedNode)

        DirectCast(ToolBar.FindButtonByCommandName("edit"), RadToolBarButton).ImageUrl = ImagePath.SmallEdit
        DirectCast(ToolBar.FindButtonByCommandName("move"), RadToolBarButton).ImageUrl = ImagePath.SmallMoveToFolder
        DirectCast(ToolBar.FindButtonByCommandName("delete"), RadToolBarButton).ImageUrl = ImagePath.SmallRemove
        DirectCast(ToolBar.FindButtonByCommandName("recovery"), RadToolBarButton).ImageUrl = ImagePath.SmallRecycle
        If Not ShowDetails Then
            With DirectCast(ToolBar.FindButtonByCommandName("detail"), RadToolBarButton)
                .Visible = True
                .ImageUrl = ImagePath.SmallInfo
            End With
        End If
        DirectCast(ToolBar.FindButtonByCommandName("log"), RadToolBarButton).ImageUrl = ImagePath.SmallLog
        DirectCast(ToolBar.FindButtonByCommandName("print"), RadToolBarButton).ImageUrl = ImagePath.SmallPrinter
        DirectCast(ToolBar.FindButtonByCommandName("legenda"), RadToolBarButton).ImageUrl = ImagePath.SmallLegend
        DirectCast(ToolBar.FindButtonByCommandName("clone"), RadToolBarButton).ImageUrl = ImagePath.SmallDocumentCopies
    End Sub

    ''' <summary> Impostazioni dei pulsanti che possono cambiare </summary>
    Private Sub SetButtons(ByVal node As RadTreeNode)
        For Each btn As RadToolBarButton In ToolBar.GetGroupButtons("selection")
            btn.Visible = MultiSelect
        Next

        Dim isRoot As Boolean = node Is Nothing OrElse node.ParentNode Is Nothing
        If MultiSelect Then
            ToolBar.FindButtonByCommandName("checkAllContacts").Text = If(isRoot, "Seleziona tutti", "Seleziona Figli")
            ToolBar.FindButtonByCommandName("uncheckAllContacts").Text = If(isRoot, "Deseleziona tutti", "Deseleziona Figli")
        End If

        ' Pulisco menu contestuale
        contactTree.ContextMenus.Clear()

        Dim isRoleEnabledContact As Boolean
        Boolean.TryParse(node.Attributes(RoleEnabledContactTag), isRoleEnabledContact)
        If Not CommonShared.HasGroupAdministratorRight AndAlso Not CommonShared.HasGroupTblContactRight AndAlso (Not ProtocolEnv.RoleContactEnabled OrElse (ProtocolEnv.RoleContactEnabled AndAlso Not isRoleEnabledContact)) Then
            ' Edit impossibile se non sono amministratore e non sono neppure editor di contatti della rubrica di settore in un contatto di settore
            For Each btn As RadToolBarButton In ToolBar.GetGroupButtons("edit")
                btn.Visible = False
            Next
            For Each btn As RadToolBarButton In ToolBar.GetGroupButtons("add")
                btn.Visible = False
            Next
            For Each btn As RadToolBarButton In ToolBar.GetGroupButtons("move")
                btn.Visible = False
            Next
            For Each btn As RadToolBarButton In ToolBar.GetGroupButtons("history")
                btn.Visible = False
            Next
            For Each btn As RadToolBarButton In ToolBar.GetGroupButtons("clone")
                btn.Visible = False
            Next
            btnGestione.Visible = False
            Exit Sub
        End If

        btnGestione.Visible = True

        ' Default quando l'edit è possibile
        For Each btn As RadToolBarButton In ToolBar.GetGroupButtons("edit")
            btn.Visible = True
            btn.Enabled = False
        Next
        For Each btn As RadToolBarButton In ToolBar.GetGroupButtons("move")
            btn.Visible = True
            btn.Enabled = False
        Next
        For Each btn As RadToolBarButton In ToolBar.GetGroupButtons("add")
            btn.Visible = False
        Next
        For Each btn As RadToolBarButton In ToolBar.GetGroupButtons("history")
            btn.Visible = False
        Next
        For Each btn As RadToolBarButton In ToolBar.GetGroupButtons("clone")
            btn.Visible = False
        Next

        If Not EditMode Then
            Exit Sub
        End If

        Dim menu As New RadTreeViewContextMenu()

        Dim nodeContactType As String = If(node Is Nothing, "", node.Attributes("ContactType"))


        If ProtocolEnv.RoleContactHistoricizing Then

            Dim showHistory As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("showHistory"), RadToolBarButton)
            If Not String.IsNullOrEmpty(node.Value) Then
                If CheckHistoryDetails(Convert.ToInt32(node.Value)) Then
                    showHistory.Enabled = CheckHistoryDetails(Convert.ToInt32(node.Value))
                    showHistory.Visible = CheckHistoryDetails(Convert.ToInt32(node.Value))
                End If
                showHistory.ImageUrl = ImagePath.SmallInfo
                showHistory.CommandArgument = GetWindowParameters("Add", "H")
                menu.Items.Add(New RadMenuItem("Storicizzazione Contatto") With {.Value = showHistory.CommandName, .ImageUrl = showHistory.ImageUrl})
            Else
                showHistory.Visible = False
            End If

        End If

        Dim addAmministrazione As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("addAmministrazione"), RadToolBarButton)
        If isRoot OrElse nodeContactType.Eq(ContactType.Sector) Then
            addAmministrazione.Visible = True
            addAmministrazione.ImageUrl = ImagePath.ContactTypeIcon(ContactType.Administration)
            addAmministrazione.CommandArgument = GetWindowParameters("Add", "M")
            menu.Items.Add(New RadMenuItem("Nuova Amministrazione") With {.Value = addAmministrazione.CommandName, .ImageUrl = addAmministrazione.ImageUrl})
        Else
            addAmministrazione.Visible = False
        End If

        Dim addGruppo As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("addGruppo"), RadToolBarButton)
        If isRoot OrElse nodeContactType.Eq(ContactType.Sector) Then
            addGruppo.Visible = True
            addGruppo.ImageUrl = ImagePath.ContactTypeIcon(ContactType.Group)
            addGruppo.CommandArgument = GetWindowParameters("Add", "G")
            menu.Items.Add(New RadMenuItem("Nuovo gruppo") With {.Value = addGruppo.CommandName, .ImageUrl = addGruppo.ImageUrl})
        Else
            addGruppo.Visible = False
        End If

        Dim addSettore As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("addSettore"), RadToolBarButton)
        If isRoot Then
            addSettore.Visible = True
            addSettore.ImageUrl = ImagePath.ContactTypeIcon(ContactType.Sector)
            addSettore.CommandArgument = GetWindowParameters("Add", "S")
            menu.Items.Add(New RadMenuItem("Nuovo Settore") With {.Value = addSettore.CommandName, .ImageUrl = addSettore.ImageUrl})
        Else
            addSettore.Visible = False
        End If

        Dim addAOO As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("addAOO"), RadToolBarButton)
        If nodeContactType.Eq(ContactType.Administration) OrElse nodeContactType.Eq(ContactType.Group) Then
            addAOO.Visible = True
            addAOO.ImageUrl = ImagePath.ContactTypeIcon(ContactType.Aoo)
            addAOO.CommandArgument = GetWindowParameters("Add", "A")
            menu.Items.Add(New RadMenuItem("Nuova Area Organizzativa Omogenea") With {.Value = addAOO.CommandName, .ImageUrl = addAOO.ImageUrl})
        Else
            addAOO.Visible = False
        End If

        Dim addUO As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("addUO"), RadToolBarButton)
        If nodeContactType.Eq(ContactType.OrganizationUnit) OrElse nodeContactType.Eq(ContactType.Aoo) OrElse
            nodeContactType.Eq(ContactType.Administration) OrElse nodeContactType.Eq(ContactType.Group) Then
            addUO.Visible = True
            addUO.ImageUrl = ImagePath.ContactTypeIcon(ContactType.OrganizationUnit)
            addUO.CommandArgument = GetWindowParameters("Add", "U")
            menu.Items.Add(New RadMenuItem("Nuova Unità Organizzativa") With {.Value = addUO.CommandName, .ImageUrl = addUO.ImageUrl})
        Else
            addUO.Visible = False
        End If

        Dim addRuolo As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("addRuolo"), RadToolBarButton)
        If nodeContactType.Eq(ContactType.OrganizationUnit) OrElse nodeContactType.Eq(ContactType.Role) OrElse
            nodeContactType.Eq(ContactType.Aoo) OrElse nodeContactType.Eq(ContactType.Administration) OrElse nodeContactType.Eq(ContactType.Group) Then
            addRuolo.Visible = True
            addRuolo.ImageUrl = ImagePath.ContactTypeIcon(ContactType.Role)
            addRuolo.CommandArgument = GetWindowParameters("Add", "R")
            menu.Items.Add(New RadMenuItem("Nuovo Ruolo") With {.Value = addRuolo.CommandName, .ImageUrl = addRuolo.ImageUrl})
        Else
            addRuolo.Visible = False
        End If

        Dim addPersona As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("addPersona"), RadToolBarButton)
        If nodeContactType.Eq(ContactType.Role) OrElse nodeContactType.Eq(ContactType.OrganizationUnit) OrElse nodeContactType.Eq(ContactType.Aoo) OrElse nodeContactType.Eq(ContactType.Administration) OrElse nodeContactType.Eq(ContactType.Group) Then
            addPersona.Visible = True
            addPersona.ImageUrl = ImagePath.ContactTypeIcon(ContactType.Person)
            addPersona.CommandArgument = GetWindowParameters("Add", "P")
            menu.Items.Add(New RadMenuItem("Nuova Persona") With {.Value = addPersona.CommandName, .ImageUrl = addPersona.ImageUrl})
        Else
            addPersona.Visible = False
        End If


        Dim addPersoneExcel As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("addPersoneExcel"), RadToolBarButton)
        addPersoneExcel.Visible = True
        addPersoneExcel.ImageUrl = ImagePath.SmallExcel
        addPersoneExcel.CommandArgument = GetWindowParameters("Add", "P")
        menu.Items.Add(New RadMenuItem("Nuovi Contatti aziende tramite excel") With {.Value = addPersoneExcel.CommandName, .ImageUrl = addPersoneExcel.ImageUrl})

        Dim imgEdit As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("edit"), RadToolBarButton)
        imgEdit.Visible = True
        If Not isRoot Then
            imgEdit.Enabled = True
            imgEdit.CommandArgument = GetWindowParameters("Rename", "")
            menu.Items.Add(New RadMenuItem("Modifica") With {.Value = imgEdit.CommandName, .ImageUrl = imgEdit.ImageUrl})
        Else
            imgEdit.Enabled = False
        End If

        Dim imgMove As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("move"), RadToolBarButton)
        imgMove.Visible = True
        If Not isRoot Then
            imgMove.Enabled = True
            imgMove.CommandArgument = GetWindowParameters("Move", "")
            menu.Items.Add(New RadMenuItem("Sposta") With {.Value = imgMove.CommandName, .ImageUrl = imgMove.ImageUrl})
        Else
            imgMove.Enabled = False
        End If

        Dim imgClone As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("clone"), RadToolBarButton)
        imgClone.Visible = True
        If Not isRoot Then
            imgClone.Enabled = True
            imgClone.CommandArgument = GetWindowParameters("Clone", String.Empty)
            menu.Items.Add(New RadMenuItem("Clona") With {.Value = imgClone.CommandName, .ImageUrl = imgClone.ImageUrl})
        Else
            imgClone.Enabled = False
        End If

        Dim recovery As Boolean
        Boolean.TryParse(node.Attributes("Recovery"), recovery)
        Dim hasChildren As Boolean
        Boolean.TryParse(node.Attributes("HasChildren"), hasChildren)

        Dim imgDelete As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("delete"), RadToolBarButton)
        Dim imgRecovery As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("recovery"), RadToolBarButton)
        imgDelete.Visible = True
        imgDelete.Enabled = Not isRoot AndAlso Not recovery
        imgRecovery.Visible = True
        imgRecovery.Enabled = Not isRoot AndAlso recovery
        If recovery Then
            imgRecovery.CommandArgument = GetWindowParameters("Recovery", "")
            menu.Items.Add(New RadMenuItem("Ripristina") With {.Value = imgRecovery.CommandName, .ImageUrl = imgRecovery.ImageUrl})
        Else
            imgDelete.CommandArgument = GetWindowParameters("Del", "")
            menu.Items.Add(New RadMenuItem("Elimina") With {.Value = imgDelete.CommandName, .ImageUrl = imgDelete.ImageUrl})
        End If

        Dim imgLog As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("log"), RadToolBarButton)
        imgLog.Visible = True
        If Not isRoot AndAlso DocSuiteContext.Current.ProtocolEnv.IsTableLogEnabled Then
            imgLog.Enabled = True
            menu.Items.Add(New RadMenuItem("Log") With {.Value = imgLog.CommandName, .ImageUrl = imgLog.ImageUrl})
        Else
            imgLog.Enabled = False
        End If

        Dim imgPrint As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("print"), RadToolBarButton)
        imgPrint.Visible = True
        If Not isRoot Then
            imgPrint.Enabled = True
            menu.Items.Add(New RadMenuItem("Stampa") With {.Value = imgPrint.CommandName, .ImageUrl = imgPrint.ImageUrl})
        Else
            imgPrint.Enabled = False
        End If
    End Sub

    Private Sub Search()
        ' Verifica se specificato un contatto root
        Dim search As String
        search = txtCerca.Text
        Dim containChecked As Boolean
        containChecked = chbContiene.Checked
        If SearchinRubrica Then
            search = txtCercaTbltRubrica.Text
            containChecked = chbContieneTbltRubrica.Checked
        End If
        Dim contactId As Integer = 0
        Dim contactFullIncrementalPath As String = String.Empty
        If Not String.IsNullOrEmpty(ContactRoot) Then
            contactId = Integer.Parse(ContactRoot)
            contactFullIncrementalPath = Facade.ContactFacade.GetById(contactId).FullIncrementalPath
        End If

        Dim contactListId As Guid? = Nothing
        If ProtocolEnv.ContactListsEnabled AndAlso pnlContactListFilter.Visible AndAlso Not String.IsNullOrEmpty(ddlContactLists.SelectedValue) AndAlso Not Guid.Parse(ddlContactLists.SelectedValue).Equals(Guid.Empty) Then
            contactListId = Guid.Parse(ddlContactLists.SelectedValue)
        End If

        If String.IsNullOrEmpty(search) Then
            contactTree.Nodes(0).Nodes.Clear()
            Select Case contactId
                Case 0
                    If ProtocolEnv.ContactListsEnabled AndAlso contactListId.HasValue Then
                        LoadContactsFromList()
                    Else
                        LoadContacts(contactTree.Nodes(0))
                    End If
                Case Else
                    Dim contacts As List(Of Contact) = Facade.ContactFacade.GetContactByParentId(Integer.Parse(ContactRoot), EditMode OrElse ShowAll,
                                                                                                 categoryFascicleRightRoles:=CurrentCategoryFascicleRightRoles, excludeParentIds:=ExcludedContacts,
                                                                                                 onlyParentId:=OnlyParentId, contactListId:=contactListId, procedureType:=CategoryContactsProcedureType,
                                                                                                 idRole:=SearchInRoleContacts, roleType:=RoleContactsProcedureType).ToList()
                    PopulateTreeViewContacts(contacts)
            End Select

            Exit Sub
        End If

        If search.Length < 2 Then
            BasePage.AjaxAlert("Inserire almeno due caratteri.")
            Exit Sub
        End If

        'recupera i contatti per descrizione
        Dim currentUserTenant As Tenant = GetCurrentTenant()

        Dim contactList As IList(Of Contact)
        If containChecked Then
            contactList = Facade.ContactFacade.GetContactByDescription(search, NHibernateContactDao.DescriptionSearchType.Contains, ShowAll, RoleContactIds, categoryFascicleRightRoles:=CurrentCategoryFascicleRightRoles,
                                                                       rootFullIncrementalPath:=contactFullIncrementalPath, excludeParentId:=ExcludedContacts, onlyParentId:=OnlyParentId, contactListId:=contactListId,
                                                                       procedureType:=CategoryContactsProcedureType, idRole:=SearchInRoleContacts, roleType:=RoleContactsProcedureType, currentTenant:=currentUserTenant)
        Else
            contactList = Facade.ContactFacade.GetContactByDescription(search, NHibernateContactDao.DescriptionSearchType.Equal, ShowAll, RoleContactIds, categoryFascicleRightRoles:=CurrentCategoryFascicleRightRoles,
                                                                       rootFullIncrementalPath:=contactFullIncrementalPath, excludeParentId:=ExcludedContacts, onlyParentId:=OnlyParentId, contactListId:=contactListId,
                                                                       procedureType:=CategoryContactsProcedureType, idRole:=SearchInRoleContacts, roleType:=RoleContactsProcedureType, currentTenant:=currentUserTenant)
        End If

        PopulateTreeViewContacts(contactList.ToList())
    End Sub

    Protected Sub LoadContacts(ByRef selNode As RadTreeNode)
        Dim contactsList As IList(Of Contact)

        ' Get current user's tenant
        Dim currentUserTenant As Tenant = GetCurrentTenant()

        If String.IsNullOrEmpty(selNode.Value) Then
            contactsList = Facade.ContactFacade.GetRootContact(EditMode OrElse ShowAll, Not CommonShared.HasGroupTblContactRight(), EditMode,
                                                               categoryFascicleRightRoles:=CurrentCategoryFascicleRightRoles, excludeParentId:=ExcludedContacts,
                                                               onlyParentId:=OnlyParentId, excludeRoleRoot:=ExcludeRoleRoot, procedureType:=CategoryContactsProcedureType,
                                                               idRole:=SearchInRoleContacts, roleType:=RoleContactsProcedureType, currentTenant:=currentUserTenant
                                                               )
        Else
            contactsList = Facade.ContactFacade.GetContactByParentId(Integer.Parse(selNode.Value), EditMode OrElse ShowAll,
                                                                     categoryFascicleRightRoles:=CurrentCategoryFascicleRightRoles, excludeParentIds:=ExcludedContacts,
                                                                     onlyParentId:=OnlyParentId, procedureType:=CategoryContactsProcedureType, idRole:=SearchInRoleContacts,
                                                                     roleType:=RoleContactsProcedureType, currentTenant:=currentUserTenant)
        End If

        For Each contact As Contact In contactsList
            If ExcludedContacts IsNot Nothing AndAlso ExcludedContacts.Contains(contact.Id) Then
                Continue For
            End If

            Dim node As RadTreeNode = CreateContactNode(contact)
            selNode.Nodes.Add(node)
            If contact.RoleRootContact IsNot Nothing OrElse selNode.Attributes(RoleEnabledContactTag) IsNot Nothing Then
                node.Attributes.Add(RoleEnabledContactTag, "true")
            End If
            If contact.IsChanged.Equals(1S) AndAlso contact.IsActive.Equals(Convert.ToInt16(True)) Then
                node.Style.Add("color", "green")
                node.Attributes.Add("Changed", "true")
            End If
            If contact.Parent Is Nothing Then
                node.Font.Bold = True
            End If
        Next
    End Sub

    Private Sub LoadContactsFromList()
        If Not String.IsNullOrEmpty(ddlContactLists.SelectedValue) AndAlso Not Guid.Parse(ddlContactLists.SelectedValue).Equals(Guid.Empty) Then
            Dim contactListId As Guid = Guid.Parse(ddlContactLists.SelectedValue)
            Dim contacts As List(Of Contact) = Facade.ContactFacade.GetByList(contactListId, ShowAll, RoleContactIds, ExcludedContacts, OnlyParentId).ToList()
            PopulateTreeViewContacts(contacts)
        Else
            LoadContacts(contactTree.Nodes(0))
        End If
    End Sub

    Public Sub CheckNodes(ByRef nodes As RadTreeNodeCollection, ByVal check As Boolean)
        If nodes.Count = 0 Then
            Exit Sub
        End If
        For Each nd As RadTreeNode In nodes
            nd.Checked = check
            CheckNodes(nd.Nodes, check)
        Next
    End Sub

    ''' <summary>
    '''     If ProtocolEnv.MultiTenantEnabled returns the current users's Tenant
    '''     otherwise returns Nothing
    ''' </summary>
    Private Function GetCurrentTenant() As Tenant
        Dim currentUserTenant As Tenant = Nothing
        If DocSuiteContext.Current.ProtocolEnv.MultiTenantEnabled Then
            currentUserTenant = CType(Session("CurrentTenant"), Tenant)
        End If

        Return currentUserTenant
    End Function

    Private Function CreateContactNode(ByRef contact As Contact, Optional ByVal checkChildren As Boolean = True) As RadTreeNode
        Dim node As New RadTreeNode()
        node.Text = contact.FullDescription
        node.Value = contact.Id.ToString()
        node.ImageUrl = ImagePath.ContactTypeIcon(contact.ContactType.Id, contact.isLocked.HasValue AndAlso contact.isLocked.Value = 1)

        If contact.isNotExpandable.HasValue AndAlso contact.isNotExpandable.Value = 1 Then
            node.Attributes.Add("IsExpandable", "false")
        Else
            If checkChildren AndAlso contact.HasChildren Then
                node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack
                node.Attributes.Add("HasChildren", "true")
            End If
            node.Attributes.Add("IsExpandable", "true")
        End If

        node.Attributes.Add("ContactType", contact.ContactType.Id)
        ' Imposto il colore del nodo
        If (contact.IsActive <> 1 OrElse Not contact.IsActiveRange()) Then
            node.Style.Add("color", "gray")
        Else
            node.Style.Remove("color")
        End If

        If ShowAll Then
            ' Se sono in fase di ricerca devo poter selezionare anche quelli non attivi
            node.Attributes.Add("Recovery", "false")
        Else
            ' negli altri casi disabilito la selezione
            If contact.IsActive <> 1 OrElse Not contact.IsActiveRange() Then
                node.Checkable = False
            End If
            node.Attributes.Add("Recovery", If(contact.IsActive <> 1, "true", "false"))
        End If
        node.EnableContextMenu = EditMode

        Return node
    End Function

    Private Sub AddContact(ByVal contact As Contact)
        Dim tn As New RadTreeNode()
        If contact IsNot Nothing Then
            tn = CreateContactNode(contact)
        End If

        contactTree.SelectedNode.Nodes.Add(tn)
        If contactTree.SelectedNode.Attributes("HasChildren") Is Nothing Then
            contactTree.SelectedNode.Attributes.Add("HasChildren", "true")
        End If
        If contactTree.SelectedNode.Attributes(RoleEnabledContactTag) IsNot Nothing Then
            tn.Attributes.Add(RoleEnabledContactTag, contactTree.SelectedNode.Attributes(RoleEnabledContactTag))
        End If
        'ordinamento nodi post inserimento
        TreeViewUtils.SortNodes(contactTree.SelectedNode.Nodes, "Text ASC")
        'espansione padre
        contactTree.SelectedNode.Expanded = True
        'selezione nodo inserito
        tn.Selected = True
        'visualizzazione dettagli
        InitializeDetails(contact)
    End Sub

    Protected Function GetWindowParameters(ByVal action As String, ByVal actionType As String) As String
        Dim parameters As New StringBuilder()
        parameters.AppendFormat("Type={0}", If(Not String.IsNullOrEmpty(Type), Type, CommBasePage.DefaultType))

        If Not String.IsNullOrEmpty(action) Then
            parameters.AppendFormat("&Action={0}", action)
        End If

        If Not String.IsNullOrEmpty(actionType) Then
            parameters.AppendFormat("&ActionType={0}", actionType)
        End If
        If "Move".Eq(action) AndAlso ProtocolEnv.ManageDisableItemsEnabled Then
            parameters.Append("&ShowAll=True")
        End If
        Return parameters.ToString()
    End Function

    Private Sub PopulateTreeViewContacts(ByRef contactList As List(Of Contact))
        'pulisce la treeview
        contactTree.Nodes(0).Nodes.Clear()

        'Data la lista di contatti ottimizza la ricerca dei nodi padre per 
        'la visualizzazione gerarchica
        Dim hierarchy As List(Of List(Of Contact)) = Facade.ContactFacade.GetHierarchyOfContacts(contactList)
        For i As Integer = 0 To hierarchy.Count - 1
            For Each contact As Contact In hierarchy(i)
                If contact Is Nothing OrElse contactTree.FindNodeByValue(contact.Id.ToString()) IsNot Nothing OrElse (ExcludedContacts IsNot Nothing AndAlso ExcludedContacts.Contains(contact.Id)) Then
                    Continue For
                End If

                Dim node As RadTreeNode = CreateContactNode(contact, False)
                node.ExpandMode = TreeNodeExpandMode.ClientSide
                node.Expanded = True

                Dim parentNode As RadTreeNode = Nothing
                If contact.Parent IsNot Nothing Then
                    parentNode = contactTree.FindNodeByValue(contact.Parent.Id.ToString())
                End If

                If parentNode Is Nothing Then
                    contactTree.Nodes(0).Nodes.Add(node)

                    If ProtocolEnv.RoleContactEnabled AndAlso (contact.RoleRootContact IsNot Nothing) Then
                        node.Attributes.Add(RoleEnabledContactTag, "true")
                    End If

                Else
                    If parentNode.Attributes(RoleEnabledContactTag) IsNot Nothing Then
                        node.Attributes.Add(RoleEnabledContactTag, parentNode.Attributes(RoleEnabledContactTag))
                    End If

                    parentNode.Nodes.Add(node)

                End If

                Dim predicate As New ContactPredicate(contact)
                Dim isSelected As Boolean = contactList.Exists(New Predicate(Of Contact)(AddressOf predicate.CompareId))
                node.Font.Bold = isSelected
                If ProtocolEnv.ContactListsEnabled AndAlso pnlContactListFilter.Visible AndAlso Not String.IsNullOrEmpty(ddlContactLists.SelectedValue) AndAlso Not Guid.Parse(ddlContactLists.SelectedValue).Equals(Guid.Empty) Then
                    node.Checked = isSelected
                End If
            Next
        Next
    End Sub

    Private Sub SelectNode(ByRef node As RadTreeNode)
        If node.Attributes("IsExpandable").Eq("false") Then
            BasePage.AjaxAlert("Il Gruppo [{0}] non può essere aperto.{1}Utilizzare le funzioni di Ricerca", node.Text, Environment.NewLine)
        Else

            If (node.Nodes.Count = 0) AndAlso (Not String.IsNullOrEmpty(node.Value)) Then
                LoadContacts(node)
                node.ExpandMode = TreeNodeExpandMode.ClientSide
                node.Expanded = True
            End If
        End If

        'CheckHistoryDetails(node.Value)

        InitializeDetails(node.Value)

        SetButtons(node)

        node.ExpandMode = TreeNodeExpandMode.ClientSide

    End Sub
    Private Function CheckHistoryDetails(ByVal roleId As Integer) As Boolean
        Dim currentContact As Contact = Facade.ContactFacade.GetById(roleId)
        If currentContact Is Nothing OrElse currentContact.IsChanged.Equals(0S) Then
            Return False
        End If
        Return True
    End Function
    Private Sub InitializeDetails(ByVal contactId As String)
        Dim tmpId As Integer
        If Not Integer.TryParse(contactId, tmpId) Then
            Exit Sub
        End If

        Dim contact As Contact = Facade.ContactFacade.GetById(tmpId, False)
        If contact Is Nothing Then
            Exit Sub
        End If
        InitializeDetails(contact)
    End Sub

    Private Sub InitializeDetails(ByRef contact As Contact)
        If Not ShowDetails Then
            Exit Sub
        End If

        ' Indico perchè non è valido
        If Not contact.IsActiveRange() Then
            ' si è brutto, ma così si confonde meglio
            Dim activeWarn As New HtmlGenericControl("div")
            activeWarn.Attributes.Add("class", "warningArea")
            activeWarn.InnerText = String.Format("Non abilitato: Valido da {0} a {1}", contact.ActiveFrom.DefaultString, contact.ActiveTo.DefaultString)
            contactDetailAjaxPanel.Controls.AddAt(0, activeWarn)
        End If

        If Not contact.RoleRootContact Is Nothing Then
            contactDetailTable.Rows.AddRaw(0, Nothing, Nothing, {30, 70}, {New LiteralControl("Settore Rubrica"), New LiteralControl(contact.RoleRootContact.Name)}, {"label"})
        End If
        If Not contact.Role Is Nothing Then
            contactDetailTable.Rows.AddRaw(0, Nothing, Nothing, {30, 70}, {New LiteralControl("Settore Autorizzato"), New LiteralControl(contact.Role.Name)}, {"label"})
        End If
        If Not String.IsNullOrEmpty(contact.Note) Then
            contactDetailTable.Rows.AddRaw(0, Nothing, Nothing, {30, 70}, {New LiteralControl("Note"), New LiteralControl(contact.Note)}, {"label"})
        End If
        If Not String.IsNullOrEmpty(contact.EmailAddress) Then
            contactDetailTable.Rows.AddRaw(0, Nothing, Nothing, {30, 70}, {New LiteralControl("eMail"), New LiteralControl(contact.EmailAddress)}, {"label"})
        End If
        If Not String.IsNullOrEmpty(contact.FaxNumber) Then
            contactDetailTable.Rows.AddRaw(0, Nothing, Nothing, {30, 70}, {New LiteralControl("Fax"), New LiteralControl(contact.FaxNumber)}, {"label"})
        End If
        If Not String.IsNullOrEmpty(contact.TelephoneNumber) Then
            contactDetailTable.Rows.AddRaw(0, Nothing, Nothing, {30, 70}, {New LiteralControl("Telefono"), New LiteralControl(contact.TelephoneNumber)}, {"label"})
        End If
        If Not String.IsNullOrEmpty(contact.SDIIdentification) Then
            contactDetailTable.Rows.AddRaw(0, Nothing, Nothing, {30, 70}, {New LiteralControl("Identificazione"), New LiteralControl(contact.SDIIdentification)}, {"label"})
        End If

        If Not contact.Address Is Nothing Then
            Dim indirizzo As New StringBuilder()
            If Not contact.Address.PlaceName Is Nothing Then
                indirizzo.AppendFormat("{0} ", contact.Address.PlaceName.Description)
            End If
            indirizzo.AppendFormat("{0} {1}<BR>{2} {3} {4}", contact.Address.Address, contact.Address.CivicNumber, contact.Address.ZipCode, contact.Address.City, contact.Address.CityCode)
            Dim result As String = indirizzo.ToString()
            If Not result.Trim().Eq("<BR>") Then
                contactDetailTable.Rows.AddRaw(0, Nothing, Nothing, {30, 70}, {New LiteralControl("Indirizzo"), New LiteralControl(result)}, {"label"})
            End If
        End If

        If Not String.IsNullOrEmpty(contact.FiscalCode) Then
            contactDetailTable.Rows.AddRaw(0, Nothing, Nothing, {30, 70}, {New LiteralControl("Cod. Fisc./P. IVA"), New LiteralControl(contact.FiscalCode)}, {"label"})
        End If

        If Not String.IsNullOrEmpty(contact.SearchCode) Then
            contactDetailTable.Rows.AddRaw(0, Nothing, Nothing, {30, 70}, {New LiteralControl("Codice Ricerca"), New LiteralControl(contact.SearchCode)}, {"label"})
        End If
        If Not String.IsNullOrEmpty(contact.Code) Then
            contactDetailTable.Rows.AddRaw(0, Nothing, Nothing, {30, 70}, {New LiteralControl("Codice AOO"), New LiteralControl(contact.Code)}, {"label"})
        End If

        If contact.StudyTitle IsNot Nothing AndAlso Not String.IsNullOrEmpty(contact.StudyTitle.Description) Then
            contactDetailTable.Rows.AddRaw(0, Nothing, Nothing, {30, 70}, {New LiteralControl("Titolo di studio"), New LiteralControl(contact.StudyTitle.Description)}, {"label"})
        End If

        If contact.BirthDate.HasValue Then
            contactDetailTable.Rows.AddRaw(0, Nothing, Nothing, {30, 70}, {New LiteralControl("Data di nascita"), New LiteralControl(contact.BirthDate.DefaultString())}, {"label"})
        End If

        If contact.BirthPlace IsNot Nothing AndAlso DocSuiteContext.Current.ProtocolEnv.SpidEnabled Then
            contactDetailTable.Rows.AddRaw(0, Nothing, Nothing, {30, 70}, {New LiteralControl("Luogo di nascita"), New LiteralControl(contact.BirthPlace)}, {"label"})
        End If

        If contact.isLocked.HasValue AndAlso contact.isLocked.Value = 1 Then
            contactDetailTable.Rows.AddRaw(0, Nothing, Nothing, {30, 70}, {New LiteralControl("Bloccato"), New LiteralControl("True")}, {"label"})
        End If

        If Not String.IsNullOrEmpty(contact.CertifiedMail) Then
            contactDetailTable.Rows.AddRaw(0, Nothing, Nothing, {30, 70}, {New LiteralControl("Posta certificata"), New LiteralControl(contact.CertifiedMail)}, {"label"})
        End If

        If contact.ContactType IsNot Nothing Then
            Dim image As New Image()
            image.ImageUrl = ImagePath.ContactTypeIcon(contact.ContactType.Id, contact.isLocked.HasValue AndAlso contact.isLocked.Value = 1)
            Dim text As New LiteralControl(Replace(contact.Description, "|", " "))
            contactDetailTable.Rows.AddRaw(0, Nothing, Nothing, {30, 70}, {image, text}, {"head", "head"})
        End If

        contactDetailTable.CssClass = "datatable"

        If contact.Parent IsNot Nothing Then
            InitializeDetails(contact.Parent)
        End If
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Private Sub CheckChildren(node As RadTreeNode)
        For Each child As RadTreeNode In node.Nodes
            SelectNode(child)
            If (node.Nodes.Count > 0) Then
                node.CheckChildNodes()
                CheckChildren(child)
            End If
        Next
    End Sub
#End Region

End Class