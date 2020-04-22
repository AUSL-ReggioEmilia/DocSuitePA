Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq
Imports System.Xml
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Tenants
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Tenants
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.Analytics
Imports VecompSoftware.Helpers.Analytics.Models.AdaptiveSearches
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.NHibernate
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Logging

Partial Public Class ProtRicerca
    Inherits ProtBasePage

#Region " Fields "

    Dim _finder As ProtocolTaskHeaderFinder
    Dim _contacts As IList(Of ContactDTO)
    Dim _contactsAss As New List(Of String)
    Private _adaptiveShowControlsAction As IDictionary(Of String, Action)
    Private _defaultAdaptiveSearch As AdaptiveSearchModel
    Private _currentUserLog As UserLog
    Private _adaptiveSearchAnalysis As AdaptiveSearchAnalysis
    Private _currentTenantFacade As TenantFacade
    Private _currentTenantFinder As TenantFinder

#End Region

#Region " Properties "
    Private ReadOnly Property MinDate As DateTime?
        Get
            Dim d As DateTime

            If Request.QueryString("MinDate") Is Nothing OrElse Not DateTime.TryParseExact(Request.QueryString("MinDate"), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, d) Then
                Return Nothing
            End If

            Return d
        End Get
    End Property

    Private ReadOnly Property IdCategory As Integer?
        Get
            Dim i As Integer
            If Request.QueryString("IdCategory") Is Nothing OrElse Not Integer.TryParse(Request.QueryString("IdCategory"), i) Then
                Return Nothing
            End If
            Return i
        End Get
    End Property

    Private ReadOnly Property AdaptiveShowControlsAction As IDictionary(Of String, Action)
        Get
            If _adaptiveShowControlsAction Is Nothing Then
                _adaptiveShowControlsAction = CreateAdaptiveShowControlsAction(Nothing, Me.searchTable.Controls)
            End If
            Return _adaptiveShowControlsAction
        End Get
    End Property

    Private ReadOnly Property DefaultAdaptiveSearch As AdaptiveSearchModel
        Get
            If _defaultAdaptiveSearch Is Nothing Then
                If Not String.IsNullOrEmpty(CurrentUserLog.DefaultAdaptiveSearchControls) Then
                    Dim model As IDictionary(Of String, AdaptiveSearchModel) = JsonConvert.DeserializeObject(Of IDictionary(Of String, AdaptiveSearchModel))(CurrentUserLog.DefaultAdaptiveSearchControls)
                    If model.ContainsKey(UserLogFacade.PROTOCOL_ADAPTIVE_SEARCH_KEY) Then
                        _defaultAdaptiveSearch = model(UserLogFacade.PROTOCOL_ADAPTIVE_SEARCH_KEY)
                        Return _defaultAdaptiveSearch
                    End If
                End If
                _defaultAdaptiveSearch = ProtocolEnv.ProtocolDefaultAdaptiveSearch
            End If
            Return _defaultAdaptiveSearch
        End Get
    End Property

    Private ReadOnly Property CurrentUserLog As UserLog
        Get
            If _currentUserLog Is Nothing Then
                _currentUserLog = Facade.UserLogFacade.GetByUser(DocSuiteContext.Current.User.UserName, DocSuiteContext.Current.User.Domain)
            End If
            Return _currentUserLog
        End Get
    End Property

    Private Property IsSearchExpanded As Boolean
        Get
            If ViewState("IsSearchExpanded") Is Nothing Then
                Return False
            End If
            Return Boolean.Parse(ViewState("IsSearchExpanded").ToString())
        End Get
        Set(value As Boolean)
            ViewState("IsSearchExpanded") = value
        End Set
    End Property
#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        SetResponseNoCache()
        InitializeAjax()

        ' Quando viene aperto in una modale nascondo il titolo ridondante
        If Action.Eq("Fasc") OrElse Action.Eq("CopyProtocolDocuments") Then
            MasterDocSuite.TitleVisible = False
        End If

        If ProtocolEnv.AutocompleteContainer Then
            rcbContainer.Style.Remove("display")
        Else
            ddlContainer.Style.Remove("display")
        End If

        If Not Page.IsPostBack Then
            trLocazione.Visible = ProtocolEnv.ProtocolSearchLocationEnabled
            sett.SetDisplay(ProtocolEnv.RolesUserProfileEnabled)

            '' Se è visibile il componente ComboBox allora esco dalla routine in quanto il caricamento avviene in modo dinamico
            If String.IsNullOrEmpty(ddlContainer.Style("display")) Then
                BindContainers(ddlContainer)
            Else
                SearchContainer(rcbContainer, String.Empty)
            End If

            If ProtocolEnv.ProtocolSearchAdaptiveEnabled Then
                SetControlsVisibilityRecursive(False, Me.searchTable.Controls)
                InitializeAdaptiveSearch()
                btnExpandSearch.Visible = True
                Title = String.Format("{0} {1}", Title, "Semplice")
            Else
                btnExpandSearch.Visible = False
            End If

            ddlLocation.DataValueField = "Id"
            ddlLocation.DataTextField = "Name"
            idDocType.DataValueField = "Id"
            idDocType.DataTextField = "Description"
            cmbTitoliStudio.DataValueField = "Id"
            cmbTitoliStudio.DataTextField = "Description"
            ProtocolStatus.DataValueField = "Id"
            ProtocolStatus.DataTextField = "Description"

            UscClassificatore1.OnlyActive = False

            If ProtocolEnv.ProtParzialeEnabled Then
                rowIncomplete.Style.Add("display", "")
            Else
                rowIncomplete.Style.Add("display", "none")
            End If

            Me.InitializeSearchDefault()

            If ProtocolEnv.CorporateAcronym.Contains("ENPACL") Then
                chbCategoryChild.Checked = True
            End If
            cbOnlyMyProt.Checked = False

            If Not ProtocolEnv.ProtocolSearchAdaptiveEnabled Then
                SetSpecificFilterVisibility()
            End If

            If MinDate.HasValue Then
                txtRegistrationDateFrom.SelectedDate = MinDate.Value
            End If

            If IdCategory.HasValue Then
                UscClassificatore1.DataSource.Add(Facade.CategoryFacade.GetById(IdCategory.Value))
                UscClassificatore1.DataBind()
                chbCategoryChild.Checked = True
            End If

            LoadFinderFromSession()

            Page.Form.DefaultButton = btnSearch.UniqueID
            btnSearch.Focus()

            uscSettore.RoleRestictions = RoleRestrictions.OnlyMine
            If (CommonShared.HasGroupsWithSearchProtocolRoleRestrictionNoneRight) Then
                uscSettore.RoleRestictions = RoleRestrictions.None
            End If
            rowAssegnatario.Visible = False
            If ProtocolEnv.IsDistributionEnabled Then
                Dim canManageableRoles As Boolean = Facade.RoleFacade.GetUserRolesCount(DSWEnvironment.Protocol, ProtocolRoleRightPositions.Manager, True) > 0
                Dim canManagebleContainers As Boolean = CommonShared.UserProtocolCheckRight(ProtocolContainerRightPositions.DocDistribution)
                If canManageableRoles OrElse canManagebleContainers Then
                    uscSettore.RoleRestictions = RoleRestrictions.None
                    rowAssegnatario.Visible = True
                End If
            End If
        End If

    End Sub

    Protected Sub OnContactAdded(ByVal sender As Object, ByVal e As EventArgs) Handles UscContattiSel1.ContactAdded
        rowInteropExtended.Style.Add("display", "")
    End Sub

    Protected Sub OnContactRemoved(ByVal sender As Object, ByVal e As EventArgs) Handles UscContattiSel1.ContactRemoved
        If UscContattiSel1.TreeViewControl.Nodes(0).Nodes.Count = 0 Then
            rowInteropExtended.Style.Add("display", "none")
            chbCategoryChild.Visible = False
        End If
    End Sub

    Protected Sub CategoryCheck(ByVal sender As Object, ByVal e As EventArgs) Handles UscClassificatore1.CategoryRemoved, UscClassificatore1.CategoryAdded
        chbCategoryChild.Visible = UscClassificatore1.HasSelectedCategories
    End Sub

    Protected Sub RoleCheck(ByVal sender As Object, ByVal e As EventArgs) Handles uscSettore.RoleRemoved, uscSettore.RoleAdded
        chbRoleChild.Visible = ProtocolEnv.IsDistributionEnabled AndAlso uscSettore.HasSelectedRole
    End Sub

    Protected Sub RcbContainer_ItemsRequested(o As Object, e As Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs)
        Dim combo As RadComboBox = DirectCast(o, RadComboBox)
        SearchContainer(combo, e.Text)
    End Sub

    Protected Sub Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearch.Click
        If Not Page.IsValid Then
            Return
        End If
        ' Cancella variabile in sessione relativa all'ultima pagina caricata
        CommonShared.SetContextValue("ProtRisultati.PageIndex", Nothing)

        Try
            LoadFinder()

            If ProtocolEnv.IsDistributionEnabled = True AndAlso IsProtocolDistribution.Checked = True Then
                CommonUtil.GetInstance().ApplyProtocolFinderSecurity(_finder, SecurityType.Distribute, True)
                _finder.NotDistributed = True
                _finder.IdTypes = ProtocolEnv.ProtocolDistributionTypologies
            End If

            If _finder.ContactDescriptionSearchBehaviour IsNot Nothing AndAlso Not BehaviourValidation(_finder.ContactDescriptionSearchBehaviour) Then
                Throw New DocSuiteException("Errore di superamento soglia", "Il filtro Mittente/Destinatario supera il massimo grado di complessità prevista, riprovare con meno parole.")
            End If

            If CommonInstance.ApplyProtocolFinderSecurity(_finder, SecurityType.Read, True) AndAlso ProtocolEnv.SearchMaxRecords <> 0 Then
                _finder.PageSize = ProtocolEnv.SearchMaxRecords
            End If

            If ProtocolEnv.CorporateAcronym.Contains("ENPACL") Then
                Dim config As New XmlDocument
                config.Load(CommonUtil.GetInstance().AppPath + "\Config\ProtocolFilterRequired.xml")
                Dim strMessage As String = ""
                If Not ProtocolFilterRequired.CheckFilter(_finder, config, strMessage, ProtocolEnv.IsTableDocTypeEnabled, ProtocolEnv.IsInteropEnabled, ProtocolEnv.IsPackageEnabled, ProtocolEnv.IsStatusEnabled) Then
                    Response.Redirect("../Prot/ProtRisultati.aspx?Type=Prot&Action=RequiredField&Message=" & strMessage, True)
                    Exit Sub
                End If
            End If

            _finder.RestrictionOnlyRoles = False
            ' Settore autorizzato
            If Not uscSettore.RoleListAdded.IsNullOrEmpty() Then
                If ProtocolEnv.IsDistributionEnabled Then
                    _finder.DistributionRestrictionRoles = uscSettore.RoleListAdded
                    _finder.IncludeChildRoles = chbRoleChild.Checked
                Else
                    _finder.SecurityRoles = String.Join(",", uscSettore.RoleListAdded.Select(Function(f) f.ToString()).ToArray())
                End If
                _finder.RestrictionOnlyRoles = ProtocolEnv.RolesUserProfileEnabled
            End If

            If ProtocolEnv.ProtocolSearchAdaptiveEnabled Then
                Try
                    'Aggiorno le statistiche
                    Dim finderControls As IDictionary(Of String, String) = LoadFinderControls(Nothing, searchTable.Controls)
                    Facade.UserLogFacade.UpdateProtocolSearchStatistics(finderControls, CurrentUserLog)
                Catch ex As Exception
                    FileLogger.Error(LoggerName, ex.Message, ex)
                End Try
            End If

            SessionSearchController.SaveSessionFinder(_finder, SessionSearchController.SessionFinderType.ProtFinderType)
            ClearSessions(Of ProtRisultati)()

            Response.Redirect("../Prot/ProtRisultati.aspx?Type=Prot&Action=" & Action)
        Catch ex As DocSuiteException
            AjaxAlert(ex)
        End Try
    End Sub

    Private Sub cvContactDescription_ServerValidate(source As Object, args As ServerValidateEventArgs) Handles cvContactDescription.ServerValidate
        args.IsValid = BehaviourValidation()
    End Sub

    Private Sub cmdClearFilters_Click(sender As Object, e As EventArgs) Handles cmdClearFilters.Click
        Me.ClearFilters(Me.searchTable.Controls)
        Me.ClearSessions(Of ProtRisultati)()
        Me.InitializeSearchDefault()
        If Not ProtocolEnv.ProtocolSearchAdaptiveEnabled Then
            SetSpecificFilterVisibility()
        End If
    End Sub

    Protected Sub uscClassificatore_CategoryAdding(sender As Object, args As EventArgs) Handles UscClassificatore1.CategoryAdding
        UscClassificatore1.Year = Nothing
        If txtYear.Value.HasValue Then
            UscClassificatore1.Year = Convert.ToInt32(txtYear.Value)
        End If
        UscClassificatore1.FromDate = txtRegistrationDateFrom.SelectedDate
        UscClassificatore1.ToDate = txtRegistrationDateTo.SelectedDate
    End Sub

    Protected Sub ExpandSearch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnExpandSearch.Click
        If IsSearchExpanded Then
            SimpleSearchFiltersVisibility()
            MasterDocSuite.TitleContainer.InnerText = String.Format("{0} {1}", Title, "Semplice")
            btnExpandSearch.Text = "Ricerca Completa"
            IsSearchExpanded = False
        Else
            ExpandSearchFiltersVisibility()
            MasterDocSuite.TitleContainer.InnerText = String.Format("{0} {1}", Title, "Completa")
            btnExpandSearch.Text = "Ricerca Semplice"
            IsSearchExpanded = True
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, chbCategoryChild)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, chbRoleChild)
        AjaxManager.AjaxSettings.AddAjaxSetting(rcbContainer, searchTable)
        Me.AjaxManager.AjaxSettings.AddAjaxSetting(Me.cmdClearFilters, Me.searchTable)
        If ProtocolEnv.ProtocolSearchAdaptiveEnabled Then
            AjaxManager.AjaxSettings.AddAjaxSetting(btnExpandSearch, searchTable)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnExpandSearch, MasterDocSuite.TitleContainer)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnExpandSearch, btnExpandSearch)
        End If
    End Sub

    Private Sub InitializeSearchDefault()
        txtYear.Focus()
        If DocSuiteContext.Current.ProtocolEnv.IsEnvSearchDefaultEnabled Then
            txtYear.Text = DateTime.Today.Year.ToString()
            txtNumber.Focus()
        End If
        If DocSuiteContext.Current.ProtocolEnv.ProtocolDefaultAllStatusSearchEnabled Then
            ddlStatusCancel.SelectedValue = String.Empty
        End If
    End Sub

    Private Sub LoadFinder()
        If _finder Is Nothing Then
            _finder = New ProtocolTaskHeaderFinder()
        End If

        If Not String.IsNullOrWhiteSpace(txtYear.Text) Then
            _finder.Year = Short.Parse(txtYear.Text)
        End If
        If Not String.IsNullOrWhiteSpace(txtNumber.Text) Then
            _finder.Number = Integer.Parse(txtNumber.Text)
        End If

        _finder.RegistrationDateFrom = txtRegistrationDateFrom.SelectedDate
        _finder.RegistrationDateTo = txtRegistrationDateTo.SelectedDate
        _finder.ProtocolNotReaded = chbNoRead.Checked
        If Not String.IsNullOrEmpty(ddlType.SelectedValue) Then
            _finder.IdTypes = New List(Of Integer) From {Convert.ToInt32(ddlType.SelectedValue)}
        End If
        _finder.IdLocation = ddlLocation.SelectedValue

        If ProtocolEnv.AutocompleteContainer Then
            _finder.IdContainer = rcbContainer.SelectedValue
        Else
            _finder.IdContainer = ddlContainer.SelectedValue
        End If

        _finder.DocumentDateFrom = txtDocumentDateFrom.SelectedDate
        _finder.DocumentDateTo = txtDocumentDateTo.SelectedDate
        _finder.DocumentProtocol = txtDocumentNumber.Text
        _finder.DocumentName = txtDocumentName.Text.Trim()
        _finder.ProtocolObject = txtObjectProtocol.Text.Trim()
        Select Case rblClausola.SelectedValue
            Case "AND"
                _finder.ProtocolObjectSearch = NHibernateProtocolFinder.ObjectSearchType.AllWords
            Case "OR"
                _finder.ProtocolObjectSearch = NHibernateProtocolFinder.ObjectSearchType.AtLeastWord
        End Select
        _finder.Note = txtNote.Text.Trim()


        Select Case DocSuiteContext.Current.ProtocolEnv.ProtocolContactTextSearchMode
            Case 0
                _finder.Recipient = txtRecipient.Text.Trim()
                _finder.EnableRecipientContains = chkRecipientContains.Checked
            Case 1
                _finder.ContactDescription = txtContactDescription.Text
                _finder.ContactDescriptionSearchBehaviour = GetContactDescriptionSearchBehaviour()

            Case 2
                _finder.ContactDescription = txtRecipient.Text.Trim()
                _finder.ContactDescriptionSearchBehaviour = GetContactDescriptionSearchByBehaviour()
        End Select


        _finder.Subject = txtSubject.Text.Trim()
        _finder.ServiceCategory = txtServiceCategory.Text.Trim()

        ' Categoria
        If UscClassificatore1.HasSelectedCategories Then
            _finder.Classifications = UscClassificatore1.SelectedCategories.First().FullIncrementalPath
            If chbCategoryChild.Visible Then
                _finder.IncludeChildClassifications = chbCategoryChild.Checked
            End If
        End If
        _finder.ProtocolStatusCancel = NHibernateProtocolFinder.StatusSearchType.OnlyStatusActive

        Select Case ddlStatusCancel.SelectedValue
            Case "AND"
                _finder.ProtocolStatusCancel = NHibernateProtocolFinder.StatusSearchType.EvenStatusCancel
            Case "OR"
                _finder.ProtocolStatusCancel = NHibernateProtocolFinder.StatusSearchType.OnlyStatusCancel
        End Select

        _finder.IncludeIncomplete = cbIncomplete.Checked
        _finder.ProtocolNoRoles = chbNoRoles.Checked

        If ProtocolEnv.FascicleEnabled Then
            _finder.IsFascicolated = cbFascicolated.Checked
        End If

        If cbOnlyMyProt.Checked Then
            _finder.RegistrationUser = DocSuiteContext.Current.User.FullUserName
        End If

        ' Ricerca opzionale
        If ProtocolEnv.IsTableDocTypeEnabled Then
            _finder.IdDocType = idDocType.SelectedValue
            If Convert.ToInt32(rblClaim.SelectedValue) < 2 Then
                _finder.IsClaim = Convert.ToInt32(rblClaim.SelectedValue) = 0
            End If
        End If

        ' Invoice
        If ProtocolEnv.IsInteropEnabled Then
            _finder.EnableInvoiceSearch = True
            If String.IsNullOrEmpty(InvoiceNumber.Text) Then
                _finder.InvoiceNumber = Nothing
            Else
                _finder.InvoiceNumber = InvoiceNumber.Text.Trim
            End If
            _finder.InvoiceDateFrom = InvoiceDateFrom.SelectedDate
            _finder.InvoiceDateTo = InvoiceDateTo.SelectedDate
            _finder.AccountingSectional = AccountingSectional.Text.Trim()
            If String.IsNullOrEmpty(AccountingYear.Text) Then
                _finder.AccountingYear = Nothing
            Else
                _finder.AccountingYear = Int16.Parse(AccountingYear.Text)
            End If
            If String.IsNullOrEmpty(AccountingNumber.Text) Then
                _finder.AccountingNumber = Nothing
            Else
                _finder.AccountingNumber = Int32.Parse(AccountingNumber.Text)
            End If
        End If
        ' Interop
        If ProtocolEnv.IsInteropEnabled Then
            _contacts = UscContattiSel1.GetContacts(False)

            If _contacts.Count > 0 Then
                _finder.Contacts = _contacts(0).Contact.FullIncrementalPath
                _finder.IncludeChildContacts = chbContactChild.Checked
            End If
        End If

        If ProtocolEnv.IsDistributionEnabled Then
            _contacts = uscContactAss.GetContacts(False)
            If _contacts.Count > 0 Then
                _contactsAss.Add(_contacts.First.Contact.Code)
                _finder.ContactsAssignee = _contactsAss
            End If
        End If

        ' Package
        If ProtocolEnv.IsPackageEnabled Then
            _finder.EnablePackageSearch = True
            _finder.PackageOrigin = Origin.Text.Trim()
            _finder.Package = Package.Text.Trim()
            _finder.PackageLot = Lot.Text.Trim()
            _finder.PackageIncremental = Incremental.Text.Trim()
        End If

        If ProtocolEnv.IsStatusEnabled Then
            _finder.AdvancedStatus = ProtocolStatus.SelectedValue
        End If

        ' Pec
        If ProtocolEnv.IsPECEnabled Then
            _finder.HasIngoingPecMails = hasIngoingPec.Checked
        End If

        If ProtocolEnv.ProtocolHighlightEnabled Then
            _finder.ProtocolHighlightToMe = cbProtocolHighlight.Checked
        End If

        If ProtocolEnv.IsDistributionEnabled Then
            Select Case ddlAssignToMe.SelectedValue
                Case "assignedtome"
                    _finder.RoleCC = False
                    _finder.RoleUser = DocSuiteContext.Current.User.FullUserName
                    _finder.RoleDistributionType = Explicit
                    _finder.AssignedToMe = True
                Case "assignedtomecc"
                    _finder.RoleCC = True
                    _finder.RoleUser = DocSuiteContext.Current.User.FullUserName
                    _finder.RoleDistributionType = Explicit
                    _finder.AssignedToMeCC = True
            End Select
        End If




    End Sub

    Private Sub LoadFinderFromSession()
        _finder = CType(SessionSearchController.LoadSessionFinder(SessionSearchController.SessionFinderType.ProtFinderType), ProtocolTaskHeaderFinder)
        If _finder Is Nothing Then
            Exit Sub
        End If

        ''il finder è valorizzato
        If _finder.Year.HasValue Then
            txtYear.Text = _finder.Year.ToString()
        End If

        If _finder.Number.HasValue Then
            txtNumber.Text = _finder.Number.ToString()
        End If

        txtRegistrationDateFrom.SelectedDate = _finder.RegistrationDateFrom
        txtRegistrationDateTo.SelectedDate = _finder.RegistrationDateTo
        chbNoRead.Checked = _finder.ProtocolNotReaded
        If Not _finder.IdTypes.IsNullOrEmpty() Then
            ddlType.SelectedValue = _finder.IdTypes.First().ToString()
        End If
        ddlLocation.SelectedValue = _finder.IdLocation


        If ProtocolEnv.AutocompleteContainer Then
            rcbContainerValue.Value = _finder.IdContainer
        Else
            ddlContainer.SelectedValue = _finder.IdContainer
        End If

        txtDocumentDateFrom.SelectedDate = _finder.DocumentDateFrom
        txtDocumentDateTo.SelectedDate = _finder.DocumentDateTo
        txtDocumentNumber.Text = _finder.DocumentProtocol
        txtDocumentName.Text = _finder.DocumentName
        txtObjectProtocol.Text = _finder.ProtocolObject
        Select Case _finder.ProtocolObjectSearch
            Case NHibernateProtocolFinder.ObjectSearchType.AllWords
                rblClausola.SelectedValue = "AND"
            Case NHibernateProtocolFinder.ObjectSearchType.AtLeastWord
                rblClausola.SelectedValue = "OR"
        End Select
        txtNote.Text = _finder.Note

        Select Case DocSuiteContext.Current.ProtocolEnv.ProtocolContactTextSearchMode
            Case 0
                txtRecipient.Text = _finder.Recipient
                chkRecipientContains.Checked = _finder.EnableRecipientContains
            Case 1
                txtContactDescription.Text = _finder.ContactDescription
                If _finder.ContactDescriptionSearchBehaviour.AtLeastOne Then
                    rblAtLeastOne.SelectedValue = "1"
                End If
                rblTextMatchMode.SelectedValue = GetTextMatchModeFromBehaviour(_finder.ContactDescriptionSearchBehaviour)
            Case 2
                txtRecipient.Text = _finder.ContactDescription
        End Select
        txtSubject.Text = _finder.Subject
        txtServiceCategory.Text = _finder.ServiceCategory
        If Not String.IsNullOrEmpty(_finder.Classifications) Then
            Dim cat As Category = Facade.CategoryFacade.GetCategoryByFullIncrementalPath(_finder.Classifications, True)
            If cat IsNot Nothing Then
                UscClassificatore1.DataSource = New List(Of Category) From {cat}
                UscClassificatore1.DataBind()
            End If
        End If
        If chbCategoryChild.Visible Then
            chbCategoryChild.Checked = _finder.IncludeChildClassifications
        End If
        Select Case _finder.ProtocolStatusCancel
            Case NHibernateProtocolFinder.StatusSearchType.EvenStatusCancel
                ddlStatusCancel.SelectedValue = "AND"
            Case NHibernateProtocolFinder.StatusSearchType.OnlyStatusCancel
                ddlStatusCancel.SelectedValue = "OR"
        End Select
        cbIncomplete.Checked = _finder.IncludeIncomplete
        chbNoRoles.Checked = _finder.ProtocolNoRoles
        cbProtocolHighlight.Checked = _finder.ProtocolHighlightToMe

        cbOnlyMyProt.Checked = Not String.IsNullOrEmpty(_finder.RegistrationUser)

        ' Ricerca opzionale
        If ProtocolEnv.IsTableDocTypeEnabled Then
            idDocType.SelectedValue = _finder.IdDocType
            If _finder.IsClaim Then
                rblClaim.SelectedValue = "0"
            End If
        End If

        ' Invoice
        If ProtocolEnv.IsInteropEnabled Then
            If Not String.IsNullOrEmpty(_finder.InvoiceNumber) Then
                InvoiceNumber.Text = _finder.InvoiceNumber
            End If

            InvoiceDateFrom.SelectedDate = _finder.InvoiceDateFrom
            InvoiceDateTo.SelectedDate = _finder.InvoiceDateTo
            AccountingSectional.Text = _finder.AccountingSectional

            If _finder.AccountingYear.HasValue Then
                AccountingYear.Text = _finder.AccountingYear.Value.ToString()
            End If
            If _finder.AccountingNumber.HasValue Then
                AccountingNumber.Text = _finder.AccountingNumber.Value.ToString()
            End If
        End If

        ' Interop
        If ProtocolEnv.IsInteropEnabled AndAlso _finder.Contacts IsNot Nothing AndAlso _finder.Contacts.Any() Then
            Dim ct As List(Of Contact) = Facade.ContactFacade.GetContactByFullPath(_finder.Contacts).ToList()
            If ct.Any() Then
                UscContattiSel1.DataSource = ct.Select(Function(s) New ContactDTO(s, ContactDTO.ContactType.Address)).ToList()
                UscContattiSel1.DataBind()
                chbContactChild.Checked = _finder.IncludeChildContacts
            End If
        End If

        If _finder.ContactsAssignee IsNot Nothing AndAlso _finder.ContactsAssignee.Any() Then
            Dim ct As New List(Of ContactDTO)
            Dim adUser As AccountModel = CommonAD.GetAccount(_finder.ContactsAssignee.First)
            Dim contact As Contact = New Contact With {
                .ContactType = New ContactType(ContactType.Mistery),
                .Code = adUser.Account,
                .Description = String.Concat(adUser.DisplayName)
                }

            Dim contactDTO As ContactDTO = New ContactDTO
            contactDTO.Contact = contact

            ct.Add(contactDTO)
            If ct.Any() Then
                uscContactAss.DataSource = ct
                uscContactAss.DataBind()
            End If
        End If
        ' Package
        If ProtocolEnv.IsPackageEnabled Then
            Origin.Text = _finder.PackageOrigin
            Package.Text = _finder.Package
            Lot.Text = _finder.PackageLot
            Incremental.Text = _finder.PackageIncremental
        End If

        If ProtocolEnv.IsStatusEnabled Then
            ProtocolStatus.SelectedValue = _finder.AdvancedStatus
        End If

        ' Pec
        If ProtocolEnv.IsPECEnabled Then
            hasIngoingPec.Checked = _finder.HasIngoingPecMails.HasValue AndAlso _finder.HasIngoingPecMails.Value
        End If

        If _finder.AssignedToMe Then
            ddlAssignToMe.SelectedValue = "assignedtome"
        End If
        If _finder.AssignedToMeCC Then
            ddlAssignToMe.SelectedValue = "assignedtomecc"
        End If

        If _finder.IsFascicolated.HasValue Then
            cbFascicolated.Checked = _finder.IsFascicolated.Value
        End If
    End Sub

    Protected Overridable Sub BindContainers(ByRef comboBox As DropDownList)
        Dim containers As ICollection(Of Container) = Facade.ContainerFacade.GetAllRightsDistinct("Prot", Nothing)
        If ProtocolEnv.MultiTenantEnabled Then
            If CurrentTenant IsNot Nothing Then
                Dim tenantContainers As ICollection(Of Entity.Commons.Container) = CurrentTenant.Containers
                containers = containers.Where(Function(x) tenantContainers.Any(Function(xx) xx.EntityShortId = x.Id)).ToList()
            Else
                containers = New List(Of Container)
            End If
        End If

        If Not containers.IsNullOrEmpty() Then
            '' Tengo separati i contenitori disabilitati
            Dim disabledContainers As New List(Of ListItem)
            For Each container As Container In containers
                Dim li As New ListItem(container.Name, container.Id.ToString())
                If container.IsActive = 1 AndAlso container.IsActiveRange() Then
                    '' Se è attivo e valido lo aggiungo direttamente al controllo
                    comboBox.Items.Add(li)
                Else
                    '' Altrimenti lo disabilito e lo tengo da parte per aggiungerlo alla fine
                    li.Attributes.Add("style", "color:grey;")
                    disabledContainers.Add(li)
                End If
            Next
            '' Aggiungo tutti i contenitori disabilitati
            comboBox.Items.AddRange(disabledContainers.ToArray())
        End If
    End Sub

    Protected Overridable Sub SearchContainer(ByRef comboBox As RadComboBox, ByVal textToSearch As String)
        Dim containers As ICollection(Of Container) = Facade.ContainerFacade.GetAllRightsDistinct("Prot", Nothing)
        If ProtocolEnv.MultiTenantEnabled Then
            If CurrentTenant IsNot Nothing Then
                Dim tenantContainers As ICollection(Of Entity.Commons.Container) = CurrentTenant.Containers
                containers = containers.Where(Function(x) tenantContainers.Any(Function(xx) xx.EntityShortId = x.Id)).ToList()
            Else
                containers = New List(Of Container)
            End If
        End If
        Dim filtered As IList(Of Container) = Facade.ContainerFacade.FilterContainers(containers, textToSearch)
        If Not filtered.IsNullOrEmpty() Then
            '' Tengo separati i contenitori disabilitati
            Dim disabledContainers As New List(Of RadComboBoxItem)
            comboBox.Items.Clear()
            comboBox.Items.Add(New RadComboBoxItem(String.Empty, String.Empty))
            For Each container As Container In filtered
                Dim item As New RadComboBoxItem(container.Name, container.Id.ToString())
                If container.IsActive = 1 AndAlso container.IsActiveRange() Then
                    '' Se è attivo e valido lo aggiungo direttamente al controllo
                    comboBox.Items.Add(item)
                Else
                    '' Altrimenti lo disabilito e lo tengo da parte per aggiungerlo alla fine
                    item.Enabled = False
                    disabledContainers.Add(item)
                End If
            Next
            '' Aggiungo tutti i contenitori disabilitati
            comboBox.Items.AddRange(disabledContainers.ToArray())
        End If
    End Sub

    Private Sub InitializeProtocolContactTextSearch()
        Dim rblDefaultValue As IDictionary(Of String, String) = DocSuiteContext.Current.ProtocolEnv.SearchProtocolRblDefaultValue
        If Not rblDefaultValue Is Nothing Then
            For Each rblValue As KeyValuePair(Of String, String) In rblDefaultValue
                Dim ctrl As Control = Me.searchTable.FindControl(rblValue.Key)
                SetControlsValue(ctrl, rblValue.Value)
            Next
        End If
        Select Case DocSuiteContext.Current.ProtocolEnv.ProtocolContactTextSearchMode
            Case 0

                pnlDescriptionSearchBehaviour.Visible = False
                pnlLegacyDescriptionSearchBehaviour.Visible = True
                chkRecipientContains.Visible = True
            Case 1
                pnlLegacyDescriptionSearchBehaviour.Visible = False

                pnlDescriptionSearchBehaviour.Visible = True
                If pnlDescriptionSearchBehaviour.Visible Then
                    rblTextMatchMode.SelectedValue = DocSuiteContext.Current.ProtocolEnv.DefaultTextMatchMode
                End If
            Case 2
                pnlDescriptionSearchBehaviour.Visible = False
                pnlLegacyDescriptionSearchBehaviour.Visible = True
                chkRecipientContains.Visible = False
        End Select


    End Sub
    Private Function GetContactDescriptionSearchByBehaviour() As TextSearchBehaviour

        Dim behaviour As New TextSearchBehaviour()
        behaviour.MatchMode = TextSearchBehaviour.TextMatchMode.Contains
        behaviour.AtLeastOne = 0

        Return behaviour
    End Function
    Private Function GetContactDescriptionSearchBehaviour() As TextSearchBehaviour
        Dim behaviour As New TextSearchBehaviour()
        Select Case rblTextMatchMode.SelectedValue
            Case "Equals"
                behaviour.MatchMode = TextSearchBehaviour.TextMatchMode.Equals
            Case "StartsWith"
                behaviour.MatchMode = TextSearchBehaviour.TextMatchMode.StartsWith
            Case "EndsWith"
                behaviour.MatchMode = TextSearchBehaviour.TextMatchMode.EndsWith
            Case "Contains"
                behaviour.MatchMode = TextSearchBehaviour.TextMatchMode.Contains
            Case "Anywhere"
                behaviour.MatchMode = TextSearchBehaviour.TextMatchMode.Anywhere
            Case Else
                behaviour.MatchMode = TextSearchBehaviour.TextMatchMode.Equals
        End Select
        behaviour.AtLeastOne = rblAtLeastOne.SelectedValue.Equals("1")

        Return behaviour
    End Function

    Private Function GetTextMatchModeFromBehaviour(ByVal behaviour As TextSearchBehaviour) As String
        Dim tor As String = DocSuiteContext.Current.ProtocolEnv.DefaultTextMatchMode
        Select Case behaviour.MatchMode
            Case TextSearchBehaviour.TextMatchMode.StartsWith
                tor = "StartsWith"
            Case TextSearchBehaviour.TextMatchMode.EndsWith
                tor = "EndsWith"
            Case TextSearchBehaviour.TextMatchMode.Contains
                tor = "Contains"
            Case TextSearchBehaviour.TextMatchMode.Anywhere
                tor = "Anywhere"
        End Select

        Return tor
    End Function

    Public Function BehaviourValidation(behaviour As TextSearchBehaviour) As Boolean
        Select Case True
            Case TextSearchBehaviour.TextMatchMode.Anywhere
            Case TextSearchBehaviour.TextMatchMode.Contains AndAlso behaviour.AtLeastOne
            Case Else
                Return True
        End Select

        Dim words As Integer = txtContactDescription.Text.PurgeWildcards().ToWords().Count()
        Return words <= DocSuiteContext.Current.ProtocolEnv.TextSearchComplexityThreshold
    End Function

    Public Function BehaviourValidation() As Boolean
        Dim behaviour As TextSearchBehaviour = GetContactDescriptionSearchBehaviour()
        Return BehaviourValidation(behaviour)
    End Function

    Private Sub ClearFilters(controls As ControlCollection)
        For Each item As Control In controls
            If item.Controls IsNot Nothing AndAlso item.Controls.Count > 0 Then
                Me.ClearFilters(item.Controls)
            End If

            Select Case True
                Case TypeOf item Is TextBox
                    Dim casted As TextBox = DirectCast(item, TextBox)
                    If casted.Enabled AndAlso casted.Visible Then
                        casted.Text = String.Empty
                    End If
                    Continue For

                Case TypeOf item Is CheckBox
                    Dim casted As CheckBox = DirectCast(item, CheckBox)
                    If casted.Enabled AndAlso casted.Visible Then
                        casted.Checked = False
                    End If
                    Continue For

                Case TypeOf item Is DropDownList
                    Dim casted As DropDownList = DirectCast(item, DropDownList)
                    If casted.Enabled AndAlso casted.Visible Then
                        casted.ClearSelection()
                    End If
                    Continue For

                Case TypeOf item Is RadioButtonList
                    Dim casted As RadioButtonList = DirectCast(item, RadioButtonList)
                    If casted.Enabled AndAlso casted.Visible Then
                        casted.ClearSelection()
                    End If
                    Continue For

                Case TypeOf item Is RadNumericTextBox
                    Dim casted As RadNumericTextBox = DirectCast(item, RadNumericTextBox)
                    If casted.Enabled AndAlso casted.Visible Then
                        casted.Text = String.Empty
                    End If
                    Continue For

                Case TypeOf item Is RadDropDownList
                    Dim casted As RadDropDownList = DirectCast(item, RadDropDownList)
                    If casted.Enabled AndAlso casted.Visible Then
                        casted.ClearSelection()
                    End If
                    Continue For

                Case TypeOf item Is uscContattiSel
                    Dim casted As uscContattiSel = DirectCast(item, uscContattiSel)
                    If Not casted.ReadOnly AndAlso casted.Visible Then
                        casted.DataSource.Clear()
                        casted.DataBind()
                    End If
                    Continue For

                Case TypeOf item Is uscClassificatore
                    Dim casted As uscClassificatore = DirectCast(item, uscClassificatore)
                    If Not casted.ReadOnly AndAlso casted.Visible Then
                        casted.Clear()
                        casted.DataBind()
                    End If
                    Continue For

                Case TypeOf item Is RadDatePicker
                    Dim casted As RadDatePicker = DirectCast(item, RadDatePicker)
                    If casted.Enabled AndAlso casted.Visible Then
                        casted.Clear()
                    End If
                    Continue For

                Case TypeOf item Is RadComboBox
                    Dim casted As RadComboBox = DirectCast(item, RadComboBox)
                    If casted.Enabled AndAlso casted.Visible Then
                        casted.ClearSelection()
                    End If
                    Continue For

                Case Else
            End Select
        Next
    End Sub

    Private Sub SimpleSearchFiltersVisibility()
        trLocazione.Visible = ProtocolEnv.ProtocolSearchLocationEnabled
        sett.SetDisplay(ProtocolEnv.RolesUserProfileEnabled)
        SetControlsVisibilityRecursive(False, Me.searchTable.Controls)
        InitializeAdaptiveSearch()
    End Sub

    Private Sub ExpandSearchFiltersVisibility()
        SetControlsVisibilityRecursive(True, Me.searchTable.Controls)
        trLocazione.Visible = ProtocolEnv.ProtocolSearchLocationEnabled
        sett.SetDisplay(ProtocolEnv.RolesUserProfileEnabled)

        SetSpecificFilterVisibility()
    End Sub

    Private Sub SetSpecificFilterVisibility()

        rowClaim.Visible = ProtocolEnv.IsClaimEnabled
        rowInterop.SetDisplay(ProtocolEnv.IsInteropEnabled)
        rowInteropExtended.Style.Add("display", "none")
        chbCategoryChild.Visible = False
        chbRoleChild.Visible = False

        rowIdDocType.Visible = ProtocolEnv.IsTableDocTypeEnabled
        rowStatusSearch.Visible = ProtocolEnv.IsStatusEnabled
        rowPackage.Visible = ProtocolEnv.IsPackageEnabled

        rowInvoice.Visible = ProtocolEnv.IsInvoiceEnabled
        rowInvoice2.Visible = ProtocolEnv.IsInvoiceEnabled
        rowInvoice3.Visible = ProtocolEnv.IsInvoiceEnabled

        rowPerson.Visible = ProtocolEnv.IsProtSearchTitleEnabled
        rowPerson2.Visible = ProtocolEnv.IsProtSearchTitleEnabled
        rowPerson3.Visible = ProtocolEnv.IsProtSearchTitleEnabled

        rowLogType.Visible = ProtocolEnv.IsLogStatusEnabled

        rowPec.Visible = ProtocolEnv.IsPECEnabled
        rowDistribution.Visible = ProtocolEnv.IsDistributionEnabled
        rowHighlight.Visible = ProtocolEnv.ProtocolHighlightEnabled
        rowFascicle.Visible = ProtocolEnv.FascicleEnabled


        rowAssignToMe.Visible = ProtocolEnv.IsDistributionEnabled

        InitializeProtocolContactTextSearch()
    End Sub

    Private Function GetShowControlsAction(item As Control) As Action
        Select Case item.ID
            Case txtRecipient.ID,
                 txtContactDescription.ID
                Return Sub()
                           rowLegacyDescriptionSearchBehaviour.Visible = True
                           Select Case DocSuiteContext.Current.ProtocolEnv.ProtocolContactTextSearchMode
                               Case 0
                                   pnlDescriptionSearchBehaviour.Visible = False
                                   pnlLegacyDescriptionSearchBehaviour.Visible = True
                                   txtRecipient.Visible = True
                                   chkRecipientContains.Visible = True
                               Case 1
                                   pnlLegacyDescriptionSearchBehaviour.Visible = False
                                   pnlDescriptionSearchBehaviour.Visible = True
                                   txtContactDescription.Visible = True
                                   rblAtLeastOne.Visible = True
                                   rblTextMatchMode.Visible = True
                               Case 2
                                   pnlDescriptionSearchBehaviour.Visible = False
                                   pnlLegacyDescriptionSearchBehaviour.Visible = True
                                   txtRecipient.Visible = True
                                   chkRecipientContains.Visible = False
                           End Select
                       End Sub

            Case ddlLocation.ID
                Return Sub()
                           trLocazione.Visible = ProtocolEnv.ProtocolSearchLocationEnabled
                       End Sub

            Case txtRegistrationDateFrom.ID,
                 txtRegistrationDateTo.ID
                Return Sub()
                           rowRegistrationDate.Visible = True
                           txtRegistrationDateFrom.Visible = True
                           txtRegistrationDateTo.Visible = True
                       End Sub

            Case txtDocumentDateFrom.ID,
             txtDocumentDateTo.ID
                Return Sub()
                           rowDocumentDate.Visible = True
                           txtDocumentDateFrom.Visible = True
                           txtDocumentDateTo.Visible = True
                       End Sub

            Case InvoiceDateFrom.ID,
                InvoiceDateTo.ID
                Return Sub()
                           rowInvoice2.Visible = True
                           InvoiceDateFrom.Visible = True
                           InvoiceDateTo.Visible = True
                       End Sub


            Case rblAtLeastOne.ID,
                 rblTextMatchMode.ID,
                 rblClausola.ID
                'Non eseguo attività
                Return Sub()

                       End Sub

            Case Package.ID
                Return Sub()
                           rowPackage.Visible = True
                           item.Visible = True
                           lblPackage.Visible = True
                       End Sub

            Case Lot.ID
                Return Sub()
                           rowPackage.Visible = True
                           item.Visible = True
                           lblLot.Visible = True
                       End Sub

            Case Incremental.ID
                Return Sub()
                           rowPackage.Visible = True
                           item.Visible = True
                           lblIncremental.Visible = True
                       End Sub

            Case AccountingYear.ID
                Return Sub()
                           rowInvoice3.Visible = True
                           item.Visible = True
                           lblAccountingYear.Visible = True
                       End Sub

            Case AccountingNumber.ID
                Return Sub()
                           rowInvoice3.Visible = True
                           item.Visible = True
                           lblAccountingNumber.Visible = True
                       End Sub

            Case rcbContainer.ID,
                 ddlContainer.ID
                Return Sub()
                           rowContainer.Visible = True
                           rcbContainer.Visible = ProtocolEnv.AutocompleteContainer
                           ddlContainer.Visible = Not ProtocolEnv.AutocompleteContainer
                       End Sub

            Case txtObjectProtocol.ID
                Return Sub()
                           rowObjectProtocol.Visible = True
                           rowClausola.Visible = True
                           txtObjectProtocol.Visible = True
                           rblClausola.Visible = True
                       End Sub

            Case UscContattiSel1.ID
                Return Sub()
                           rowInterop.SetDisplay(True)
                       End Sub

            Case uscSettore.ID
                Return Sub()
                           sett.SetDisplay(ProtocolEnv.RolesUserProfileEnabled)
                       End Sub

            Case UscClassificatore1.ID
                Return Sub()
                           rowUscClassificatore1.SetDisplay(True)
                       End Sub

            Case chbCategoryChild.ID
                Return Sub()
                           If UscClassificatore1.HasSelectedCategories Then
                               item.Visible = True
                               rowUscClassificatore1.SetDisplay(True)
                           End If
                       End Sub
            Case Else
                'Casisitica di default, controllo definito in una cella di una riga di tabella
                Return Sub()
                           item.Parent.Parent.Visible = True
                           item.Visible = True
                       End Sub
        End Select
    End Function

    Private Function CreateAdaptiveShowControlsAction(showControlsAction As IDictionary(Of String, Action), controls As ControlCollection) As IDictionary(Of String, Action)
        If showControlsAction Is Nothing Then
            showControlsAction = New Dictionary(Of String, Action)
        End If

        For Each item As Control In controls
            If item.Controls IsNot Nothing AndAlso item.Controls.Count > 0 Then
                Me.CreateAdaptiveShowControlsAction(showControlsAction, item.Controls)
            End If

            Select Case True
                Case TypeOf item Is TextBox,
                     TypeOf item Is CheckBox,
                     TypeOf item Is DropDownList,
                     TypeOf item Is RadioButtonList,
                     TypeOf item Is RadNumericTextBox,
                     TypeOf item Is RadDropDownList,
                     TypeOf item Is uscContattiSel,
                     TypeOf item Is uscClassificatore,
                     TypeOf item Is uscSettori,
                     TypeOf item Is RadDatePicker,
                     TypeOf item Is RadComboBox
                    If Not showControlsAction.ContainsKey(item.ID) Then
                        showControlsAction.Add(item.ID, GetShowControlsAction(item))
                    End If
            End Select
        Next

        Return showControlsAction
    End Function

    Private Sub SetControlsVisibilityRecursive(visible As Boolean, controls As ControlCollection)
        For Each item As Control In controls
            Dim isUserControl As Boolean = TypeOf item Is uscSettori OrElse TypeOf item Is uscClassificatore OrElse TypeOf item Is uscContattiSel
            If item.Controls IsNot Nothing AndAlso item.Controls.Count > 0 AndAlso Not isUserControl Then
                Me.SetControlsVisibilityRecursive(visible, item.Controls)
            End If

            Select Case True
                Case TypeOf item Is TextBox,
                     TypeOf item Is CheckBox,
                     TypeOf item Is DropDownList,
                     TypeOf item Is RadioButtonList,
                     TypeOf item Is RadNumericTextBox,
                     TypeOf item Is RadDropDownList,
                     TypeOf item Is RadDatePicker,
                     TypeOf item Is RadComboBox,
                     TypeOf item Is TableRow,
                     TypeOf item Is Label
                    item.Visible = visible

                Case TypeOf item Is HtmlTableRow
                    If item.ID.Eq(sett.ID) OrElse item.ID.Eq(rowInterop.ID) OrElse item.ID.Eq(rowUscClassificatore1.ID) Then
                        DirectCast(item, HtmlTableRow).SetDisplay(visible)
                    Else
                        item.Visible = visible
                    End If

            End Select
        Next
    End Sub

    Private Sub SetControlsValue(item As Control, value As String)
        If item Is Nothing Then
            Exit Sub
        End If

        If String.IsNullOrEmpty(value) Then
            Exit Sub
        End If

        Select Case True
            Case TypeOf item Is TextBox
                Dim casted As TextBox = DirectCast(item, TextBox)
                If casted.Enabled Then
                    casted.Text = value
                End If

            Case TypeOf item Is CheckBox
                Dim casted As CheckBox = DirectCast(item, CheckBox)
                If casted.Enabled Then
                    Dim checked As Boolean = False
                    If Boolean.TryParse(value, checked) Then
                        casted.Checked = checked
                    End If
                End If

            Case TypeOf item Is DropDownList
                Dim casted As DropDownList = DirectCast(item, DropDownList)
                If casted.Enabled Then
                    casted.SelectedValue = value
                End If

            Case TypeOf item Is RadioButtonList
                Dim casted As RadioButtonList = DirectCast(item, RadioButtonList)
                If casted.Enabled Then
                    casted.SelectedValue = value
                End If

            Case TypeOf item Is RadNumericTextBox
                Dim casted As RadNumericTextBox = DirectCast(item, RadNumericTextBox)
                If casted.Enabled Then
                    Dim val As Double = 0
                    If Double.TryParse(value, val) Then
                        casted.Value = val
                    End If
                End If

            Case TypeOf item Is RadDropDownList
                Dim casted As RadDropDownList = DirectCast(item, RadDropDownList)
                If casted.Enabled Then
                    casted.SelectedValue = value
                End If

            Case TypeOf item Is uscContattiSel
                Dim casted As uscContattiSel = DirectCast(item, uscContattiSel)
                Dim contactId As Integer = 0
                If Integer.TryParse(value, contactId) Then
                    Dim contact As Contact = Facade.ContactFacade.GetById(contactId)
                    If contact IsNot Nothing Then
                        casted.DataSource = New List(Of ContactDTO) From {New ContactDTO(contact, ContactDTO.ContactType.Address)}
                        casted.DataBind()
                    End If
                End If

            Case TypeOf item Is uscClassificatore
                Dim casted As uscClassificatore = DirectCast(item, uscClassificatore)
                Dim categoryId As Integer = 0
                If Integer.TryParse(value, categoryId) Then
                    Dim category As Category = Facade.CategoryFacade.GetById(categoryId)
                    If category IsNot Nothing Then
                        casted.DataSource = New List(Of Category) From {category}
                        casted.DataBind()
                    End If
                End If

            Case TypeOf item Is uscSettori
                Dim casted As uscSettori = DirectCast(item, uscSettori)
                Dim roleId As Integer = 0
                If Integer.TryParse(value, roleId) Then
                    Dim role As Role = Facade.RoleFacade.GetById(roleId)
                    If role IsNot Nothing Then
                        casted.AddRole(role, True, False, False, True)
                    End If
                End If

            Case TypeOf item Is RadDatePicker
                Dim casted As RadDatePicker = DirectCast(item, RadDatePicker)
                If casted.Enabled Then
                    Dim val As DateTime
                    If Date.TryParse(value, val) Then
                        casted.SelectedDate = val
                    End If
                End If

            Case TypeOf item Is RadComboBox
                Dim casted As RadComboBox = DirectCast(item, RadComboBox)
                If casted.Enabled Then
                    casted.SelectedValue = value
                End If
        End Select
    End Sub

    Public Sub InitializeAdaptiveSearch()
        Dim searchModel As UserSearchModel = Facade.UserLogFacade.GetProtocolUserSearchControls(CurrentUserLog)
        For Each control As KeyValuePair(Of String, String) In searchModel.SearchControls
            If AdaptiveShowControlsAction.ContainsKey(control.Key) Then
                AdaptiveShowControlsAction(control.Key)()
                If Not String.IsNullOrEmpty(control.Value) Then
                    Try
                        Dim ctrl As Control = Me.searchTable.FindControl(control.Key)
                        SetControlsValue(ctrl, control.Value)
                    Catch ex As Exception
                        FileLogger.Error(LoggerName, ex.Message, ex)
                    End Try
                End If
            End If
        Next
    End Sub

    Public Function LoadFinderControls(finderControls As IDictionary(Of String, String), controls As ControlCollection) As IDictionary(Of String, String)
        If finderControls Is Nothing Then
            finderControls = New Dictionary(Of String, String)
        End If

        For Each item As Control In controls
            If item.Controls IsNot Nothing AndAlso item.Controls.Count > 0 Then
                Me.LoadFinderControls(finderControls, item.Controls)
            End If

            Select Case True
                Case TypeOf item Is TextBox
                    Dim casted As TextBox = DirectCast(item, TextBox)
                    If casted.Enabled AndAlso casted.Visible AndAlso Not String.IsNullOrEmpty(casted.Text) Then
                        finderControls.Add(item.ID, casted.Text)
                    End If

                Case TypeOf item Is CheckBox
                    Dim casted As CheckBox = DirectCast(item, CheckBox)
                    If casted.Enabled AndAlso casted.Visible AndAlso casted.Checked Then
                        If casted.ID.Eq(chkRecipientContains.ID) AndAlso String.IsNullOrEmpty(txtRecipient.Text) Then
                            Continue For
                        End If
                        finderControls.Add(item.ID, casted.Checked.ToString())
                    End If

                Case TypeOf item Is DropDownList
                    Dim casted As DropDownList = DirectCast(item, DropDownList)
                    If casted.Enabled AndAlso casted.Visible AndAlso Not String.IsNullOrEmpty(casted.SelectedValue) Then
                        finderControls.Add(item.ID, casted.SelectedValue)
                    End If

                Case TypeOf item Is RadioButtonList
                    Dim casted As RadioButtonList = DirectCast(item, RadioButtonList)
                    If casted.Enabled AndAlso casted.Visible AndAlso Not String.IsNullOrEmpty(casted.SelectedValue) Then
                        Select Case casted.ID
                            Case rblClausola.ID
                                If String.IsNullOrEmpty(txtObjectProtocol.Text) Then
                                    Continue For
                                End If
                            Case rblTextMatchMode.ID,
                                 rblAtLeastOne.ID
                                If String.IsNullOrEmpty(txtContactDescription.Text) Then
                                    Continue For
                                End If
                        End Select
                        finderControls.Add(item.ID, casted.SelectedValue)
                    End If

                Case TypeOf item Is RadNumericTextBox
                    Dim casted As RadNumericTextBox = DirectCast(item, RadNumericTextBox)
                    If casted.Enabled AndAlso casted.Visible AndAlso casted.Value.HasValue Then
                        finderControls.Add(item.ID, casted.Value.Value.ToString())
                    End If

                Case TypeOf item Is RadDropDownList
                    Dim casted As RadDropDownList = DirectCast(item, RadDropDownList)
                    If casted.Enabled AndAlso casted.Visible AndAlso Not String.IsNullOrEmpty(casted.SelectedValue) Then
                        finderControls.Add(item.ID, casted.SelectedValue)
                    End If

                Case TypeOf item Is uscContattiSel
                    Dim casted As uscContattiSel = DirectCast(item, uscContattiSel)
                    If casted.Visible AndAlso Not casted.GetContacts(False).IsNullOrEmpty() Then
                        finderControls.Add(item.ID, casted.GetContacts(False).First().Contact.Id.ToString())
                    End If

                Case TypeOf item Is uscClassificatore
                    Dim casted As uscClassificatore = DirectCast(item, uscClassificatore)
                    If casted.Visible AndAlso casted.HasSelectedCategories Then
                        finderControls.Add(item.ID, casted.SelectedCategories.First().Id.ToString())
                    End If

                Case TypeOf item Is uscSettori
                    Dim casted As uscSettori = DirectCast(item, uscSettori)
                    If casted.Visible AndAlso Not casted.RoleListAdded.IsNullOrEmpty() Then
                        finderControls.Add(item.ID, casted.RoleListAdded.First().ToString())
                    End If

                Case TypeOf item Is RadDatePicker
                    Dim casted As RadDatePicker = DirectCast(item, RadDatePicker)
                    If casted.Enabled AndAlso casted.Visible AndAlso casted.SelectedDate.HasValue Then
                        finderControls.Add(item.ID, casted.SelectedDate.ToString())
                    End If

                Case TypeOf item Is RadComboBox
                    Dim casted As RadComboBox = DirectCast(item, RadComboBox)
                    If casted.Enabled AndAlso casted.Visible AndAlso Not String.IsNullOrEmpty(casted.SelectedValue) Then
                        finderControls.Add(item.ID, casted.SelectedValue.ToString())
                    End If
            End Select
        Next
        Return finderControls
    End Function
#End Region

End Class