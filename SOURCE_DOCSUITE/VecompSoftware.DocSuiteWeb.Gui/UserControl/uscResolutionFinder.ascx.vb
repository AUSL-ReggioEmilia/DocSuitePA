Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports Telerik.Web.UI

Partial Public Class uscResolutionFinder
    Inherits DocSuite2008BaseControl

    Public Delegate Sub BindComboBoxDelegate(ByRef combobox As DropDownList)
    Public BindContainersDelegate As BindComboBoxDelegate
    Public BindControllerStatusDelegate As BindComboBoxDelegate

#Region " Fields "

    ReadOnly _finder As NHibernateResolutionFinder = New NHibernateResolutionFinder("ReslDB")
    Dim _contacts As IList(Of ContactDTO)
    Private _dateAdoptionToValue As Date?
    Private _dateAdoptionFromValue As Date?
    Private _dateProposerToValue As Date?
    Private _dateProposerFromValue As Date?

#End Region

#Region "Properties"

    Public ReadOnly Property Count() As Long
        Get
            Return _finder.Count()
        End Get
    End Property

    Public Property PageIndex() As Integer
        Get
            Return _finder.PageIndex
        End Get
        Set(ByVal value As Integer)
            _finder.PageIndex = value
        End Set
    End Property

    Public Property PageSize() As Integer
        Get
            Return _finder.PageSize
        End Get
        Set(ByVal value As Integer)
            _finder.PageSize = value
        End Set
    End Property

    Public ReadOnly Property Finder() As NHibernateBaseFinder(Of Resolution, ResolutionHeader)
        Get
            BindData()
            Return _finder
        End Get
    End Property

    ''' <summary>
    ''' In caso di back dell'utente la RadDatePicker non riesce a leggere la data poichè la considera invalida.
    ''' Viene fixato il bug con la seguente proprietà
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property DateAdoptionFromValue As Date?
        Get
            If DateAdoptionFrom.SelectedDate.HasValue Then
                _dateAdoptionFromValue = DateAdoptionFrom.SelectedDate.Value
            Else
                Dim result As Date
                If Date.TryParse(DateAdoptionFrom.DateInput.InvalidTextBoxValue, result) Then
                    _dateAdoptionFromValue = result
                End If
            End If

            Return _dateAdoptionFromValue
        End Get
    End Property

    ''' <summary>
    ''' In caso di back dell'utente la RadDatePicker non riesce a leggere la data poichè la considera invalida.
    ''' Viene fixato il bug con la seguente proprietà
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property DateAdoptionToValue As Date?
        Get
            If DateAdoptionTo.SelectedDate.HasValue Then
                _dateAdoptionToValue = DateAdoptionTo.SelectedDate.Value
            Else
                Dim result As Date
                If Date.TryParse(DateAdoptionTo.DateInput.InvalidTextBoxValue, result) Then
                    _dateAdoptionToValue = result
                End If
            End If

            Return _dateAdoptionToValue
        End Get
    End Property

    ''' <summary>
    ''' In caso di back dell'utente la RadDatePicker non riesce a leggere la data poichè la considera invalida.
    ''' Viene fixato il bug con la seguente proprietà
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property DateProposerFromValue As Date?
        Get
            If DateProposerFrom.SelectedDate.HasValue Then
                _dateProposerFromValue = DateProposerFrom.SelectedDate.Value
            Else
                Dim result As Date
                If Date.TryParse(DateProposerFrom.DateInput.InvalidTextBoxValue, result) Then
                    _dateProposerFromValue = result
                End If
            End If

            Return _dateProposerFromValue
        End Get
    End Property

    ''' <summary>
    ''' In caso di back dell'utente la RadDatePicker non riesce a leggere la data poichè la considera invalida.
    ''' Viene fixato il bug con la seguente proprietà
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property DateProposerToValue As Date?
        Get
            If DateProposerTo.SelectedDate.HasValue Then
                _dateProposerToValue = DateProposerTo.SelectedDate.Value
            Else
                Dim result As Date
                If Date.TryParse(DateProposerTo.DateInput.InvalidTextBoxValue, result) Then
                    _dateProposerToValue = result
                End If
            End If

            Return _dateProposerToValue
        End Get
    End Property

    Public Property EnableDateFromValue As Boolean
        Get
            Return DateFrom.Enabled
        End Get
        Set(ByVal value As Boolean)
            DateFrom.Enabled = value
        End Set

    End Property

    Public Property EnableDateToValue As Boolean
        Get
            Return DateTo.Enabled
        End Get
        Set(ByVal value As Boolean)
            DateTo.Enabled = value
        End Set

    End Property

#End Region

#Region "Properties: Panels"

    Public Property VisibleAdoptionYear() As Boolean
        Get
            Return trAdoptionYear.Visible
        End Get
        Set(ByVal value As Boolean)
            trAdoptionYear.Visible = value
        End Set
    End Property

    Public Property VisibleServiceNumber() As Boolean
        Get
            Return trServiceNumber.Visible
        End Get
        Set(ByVal value As Boolean)
            trServiceNumber.Visible = value
        End Set
    End Property

    Public Property VisibleAUSLPCNumber() As Boolean
        Get
            Return trAUSLPCNumber.Visible()
        End Get
        Set(ByVal value As Boolean)
            trAUSLPCNumber.Visible = value
        End Set
    End Property

    Public Property VisibleNumber() As Boolean
        Get
            Return trNumber.Visible
        End Get
        Set(ByVal value As Boolean)
            trNumber.Visible = value
        End Set
    End Property

    Public Property VisibleIdResolution() As Boolean
        Get
            Return trIdResolution.Visible
        End Get
        Set(ByVal value As Boolean)
            trIdResolution.Visible = value
        End Set
    End Property

    Public Property VisibleWorkflowStep() As Boolean
        Get
            Return trWorkflow.Visible
        End Get
        Set(ByVal value As Boolean)
            trWorkflow.Visible = value
            trActiveStep.Visible = value
        End Set
    End Property

    Public Property VisibleOC() As Boolean
        Get
            Return trOC.Visible
        End Get
        Set(ByVal value As Boolean)
            trOC.Visible = value
        End Set
    End Property

    Public Property VisibleDateStep() As Boolean
        Get
            Return trStepDate.Visible
        End Get
        Set(ByVal value As Boolean)
            trStepDate.Visible = value
        End Set
    End Property

    Public Property VisibleContainer() As Boolean
        Get
            Return trContainer.Visible
        End Get
        Set(ByVal value As Boolean)
            trContainer.Visible = value
        End Set
    End Property

    Public Property VisibleObject() As Boolean
        Get
            Return trObject.Visible
        End Get
        Set(ByVal value As Boolean)
            trObject.Visible = value
            trObjectExt.Visible = value
        End Set
    End Property

    Public Property VisibleNote() As Boolean
        Get
            Return trNote.Visible
        End Get
        Set(ByVal value As Boolean)
            trNote.Visible = value
        End Set
    End Property

    Public Property VisibleRecipientContact() As Boolean
        Get
            Return trDestContact.Visible
        End Get
        Set(ByVal value As Boolean)
            trDestContact.Visible = value
        End Set
    End Property

    Public Property VisibleProposerContact() As Boolean
        Get
            Return trPropContact.Visible
        End Get
        Set(ByVal value As Boolean)
            trPropContact.Visible = value
        End Set
    End Property

    Public Property VisibleAssigneeContact() As Boolean
        Get
            Return trAssContact.Visible
        End Get
        Set(ByVal value As Boolean)
            trAssContact.Visible = value
        End Set
    End Property

    Public Property VisibleManagerContact() As Boolean
        Get
            Return trMgrContact.Visible
        End Get
        Set(ByVal value As Boolean)
            trMgrContact.Visible = value
        End Set
    End Property

    Public Property VisibleOCComment() As Boolean
        Get
            Return trOCComment.Visible
        End Get
        Set(ByVal value As Boolean)
            trOCComment.Visible = value
        End Set
    End Property

    Public Property VisibleOCOpinion() As Boolean
        Get
            Return trOCOpinion.Visible
        End Get
        Set(ByVal value As Boolean)
            trOCOpinion.Visible = value
        End Set
    End Property

    Public Property VisibleCategory() As Boolean
        Get
            Return trCategory.Visible
        End Get
        Set(ByVal value As Boolean)
            trCategory.Visible = value
        End Set
    End Property

#End Region

#Region "Properties: Panels ASL-TO2"
    Public Property VisibleImmediatelyExecutive() As Boolean
        Get
            Return trImmediatelyExecutive.Visible
        End Get
        Set(ByVal value As Boolean)
            trImmediatelyExecutive.Visible = value
        End Set
    End Property

    Public Property VisibleProposerDate() As Boolean
        Get
            Return trProposerDate.Visible
        End Get
        Set(ByVal value As Boolean)
            trProposerDate.Visible = value
        End Set
    End Property

    Public Property VisibleAdoptionDate() As Boolean
        Get
            Return trAdoptionDate.Visible
        End Get
        Set(ByVal value As Boolean)
            trAdoptionDate.Visible = value
        End Set
    End Property

    Public Property VisibleOCList() As Boolean
        Get
            Return trOCList.Visible
        End Get
        Set(ByVal value As Boolean)
            trOCList.Visible = value
        End Set
    End Property

    Public Property VisibileOC_SupervisoryBoard() As Boolean
        Get
            Return pnlSupervisoryBoard.Visible
        End Get
        Set(value As Boolean)
            pnlSupervisoryBoard.Visible = value
        End Set

    End Property

    Public Property VisibileOC_ConfSind() As Boolean
        Get
            Return pnlConfSind.Visible
        End Get
        Set(value As Boolean)
            pnlConfSind.Visible = value
        End Set

    End Property

    Public Property VisibileOC_Region() As Boolean
        Get
            Return pnlRegion.Visible
        End Get
        Set(value As Boolean)
            pnlRegion.Visible = value
        End Set
    End Property

    Public Property VisibileOC_Management() As Boolean
        Get
            Return pnlManagement.Visible
        End Get
        Set(value As Boolean)
            pnlManagement.Visible = value
        End Set
    End Property

    Public Property VisibileOC_CorteConti() As Boolean
        Get
            Return pnlCorteConti.Visible
        End Get
        Set(value As Boolean)
            pnlCorteConti.Visible = value
        End Set
    End Property

    Public Property VisibileOC_Other() As Boolean
        Get
            Return pnlOther.Visible
        End Get
        Set(value As Boolean)
            pnlOther.Visible = value
        End Set
    End Property

    Public Property VisibleStatusCancel() As Boolean
        Get
            Return trStatusCancel.Visible
        End Get
        Set(ByVal value As Boolean)
            trStatusCancel.Visible = value
        End Set
    End Property

    Public Property VisibleOnlytatusCancel() As Boolean
        Get
            Return trOnlyStatusCancel.Visible
        End Get
        Set(ByVal value As Boolean)
            trOnlyStatusCancel.Visible = value
        End Set
    End Property

    Public Property VisibilePrivacyPublication() As Boolean
        Get
            Return trPrivacyPublication.Visible
        End Get
        Set(value As Boolean)
            trPrivacyPublication.Visible = value
        End Set
    End Property
#End Region

#Region "Properties: Controls"

#Region "Contacts"
    Public ReadOnly Property ControlRecipientInterop() As uscContattiSel
        Get
            Return uscDestInterop
        End Get
    End Property

    Public ReadOnly Property ControlProposerInterop() As uscContattiSel
        Get
            Return uscPropInterop
        End Get
    End Property

    Public ReadOnly Property ControlManagerInterop() As uscContattiSel
        Get
            Return uscRespInterop
        End Get
    End Property

    Public ReadOnly Property ControlAssigneeInterop() As uscContattiSel
        Get
            Return uscAssiInterop
        End Get
    End Property

    Public ReadOnly Property ControlRecipient() As uscContattiSelText
        Get
            Return Recipient
        End Get
    End Property

    Public ReadOnly Property ControlProposer() As uscContattiSelText
        Get
            Return Proposer
        End Get
    End Property

    Public ReadOnly Property ControlManager() As uscContattiSelText
        Get
            Return Manager
        End Get
    End Property

    Public ReadOnly Property ControlAssignee() As uscContattiSelText
        Get
            Return Assignee
        End Get
    End Property
#End Region

#Region "Category"
    Public ReadOnly Property ControlCategory() As uscClassificatore
        Get
            Return uscCategory
        End Get
    End Property
#End Region

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            If String.IsNullOrEmpty(txtYear.Text) Then
                txtYear.Text = Date.Now.Year.ToString()
            End If
            If DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("AUSL-PC") Then ' Disattivo l'anno automatico per praticità di ricerca atti anche in proposta
                AUSLPCNumber.Focus()
            End If
        End If
        trCategoryExt.Attributes.Add("style", "display:none")

        If ResolutionEnv.ControlCheckResolutionEnabled Then
            Delibera.AutoPostBack = True
            pnChecked.Visible = True
            AddHandler Delibera.CheckedChanged, AddressOf Delibera_CheckedChanged
        End If


        If ResolutionEnv.CheckOCValidations Then
            Delibera.AutoPostBack = True
            Determina.AutoPostBack = True
            AddHandler Delibera.CheckedChanged, AddressOf Delibera_CheckedChanged
            AjaxManager.AjaxSettings.AddAjaxSetting(Delibera, pnlSupervisoryBoard)
            AjaxManager.AjaxSettings.AddAjaxSetting(Delibera, pnlConfSind)
            AjaxManager.AjaxSettings.AddAjaxSetting(Delibera, pnlRegion)
            AddHandler Determina.CheckedChanged, AddressOf Determina_CheckedChangedEvent
            AjaxManager.AjaxSettings.AddAjaxSetting(Determina, pnlSupervisoryBoard)
            AjaxManager.AjaxSettings.AddAjaxSetting(Determina, pnlConfSind)
            AjaxManager.AjaxSettings.AddAjaxSetting(Determina, pnlRegion)
        End If

    End Sub

    Private Sub uscCategory_CategoryAdded(ByVal sender As Object, ByVal e As EventArgs) Handles uscCategory.CategoryAdded
        chbCategoryChild.Checked = True
        AjaxManager.ResponseScripts.Add("VisibleCategorySearch()")
    End Sub

    Private Sub uscCategory_CategoryRemoved(ByVal sender As Object, ByVal e As EventArgs) Handles uscCategory.CategoryRemoved
        If Not uscCategory.HasSelectedCategories Then
            chbCategoryChild.Checked = False
            AjaxManager.ResponseScripts.Add("HideCategorySearch()")
        End If
    End Sub

    Private Sub Delibera_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
        If ResolutionEnv.ControlCheckResolutionEnabled Then
            pnChecked.Visible = trType.Visible And Delibera.Checked
        End If

        If ResolutionEnv.CheckOCValidations Then
            CheckOCVisibility()
        End If
    End Sub

    Private Sub Determina_CheckedChangedEvent(ByVal sender As Object, ByVal e As EventArgs)
        If ResolutionEnv.CheckOCValidations Then
            CheckOCVisibility()
        End If
    End Sub
    Protected Sub Proposta_CheckedChanged(sender As Object, E As EventArgs) Handles Proposta.CheckedChanged
        If (Proposta.Checked) Then
            EnableRangeDate()
        Else DisableRangeDate()
        End If
    End Sub

    Protected Sub Adottata_CheckedChanged(sender As Object, e As EventArgs) Handles Adottata.CheckedChanged
        If (Adottata.Checked) Then
            EnableRangeDate()
        Else DisableRangeDate()
        End If
    End Sub
    Protected Sub Pubblicata_CheckedChanged(sender As Object, e As EventArgs) Handles Pubblicata.CheckedChanged
        If (Pubblicata.Checked) Then
            EnableRangeDate()
        Else DisableRangeDate()
        End If

    End Sub
    Protected Sub Esecutiva_CheckedChanged(sender As Object, e As EventArgs) Handles Esecutiva.CheckedChanged
        If (Esecutiva.Checked) Then
            EnableRangeDate()
        Else DisableRangeDate()
        End If
    End Sub

    Protected Sub Spedizione_CheckedChanged(sender As Object, e As EventArgs) Handles Spedizione.CheckedChanged
        If (Spedizione.Checked) Then
            EnableRangeDate()
        Else DisableRangeDate()
        End If
    End Sub
    Protected Sub Ricezione_CheckedChanged(sender As Object, e As EventArgs) Handles Ricezione.CheckedChanged
        If (Ricezione.Checked) Then
            EnableRangeDate()
        Else DisableRangeDate()
        End If
    End Sub
    Protected Sub Scadenza_CheckedChanged(sender As Object, e As EventArgs) Handles Scadenza.CheckedChanged
        If (Scadenza.Checked) Then
            EnableRangeDate()
        Else DisableRangeDate()
        End If
    End Sub
    Protected Sub Risposta_CheckedChanged(sender As Object, e As EventArgs) Handles Risposta.CheckedChanged
        If (Risposta.Checked) Then
            EnableRangeDate()
        Else DisableRangeDate()
        End If
    End Sub

    Protected Sub uscCategory_CategoryAdding(sender As Object, args As EventArgs) Handles uscCategory.CategoryAdding
        uscCategory.Year = Nothing
        If Not String.IsNullOrEmpty(txtYear.Text) Then
            uscCategory.Year = Convert.ToInt32(txtYear.Text)
        End If
        uscCategory.FromDate = DateFrom.SelectedDate
        uscCategory.ToDate = DateTo.SelectedDate
    End Sub

#End Region

#Region " Methods "

    Private Sub BindData()
        _finder.Delibera = Delibera.Checked
        _finder.Determina = Determina.Checked
        _finder.Year = txtYear.Text.Trim()
        Dim num As Integer
        If Integer.TryParse(Number.Text, num) Then
            _finder.Number = num
        End If
        _finder.AuslPcNumber = AUSLPCNumber.Text.Trim()
        _finder.ServiceNumber = ServiceNumber.Text.Trim()
        _finder.IdResolution = idResolution.Text.Trim()

        _finder.Spedizione = Spedizione.Checked
        _finder.Ricezione = Ricezione.Checked
        _finder.Scadenza = Scadenza.Checked
        _finder.Risposta = Risposta.Checked

        _finder.Proposta = Proposta.Checked
        _finder.Adottata = Adottata.Checked
        _finder.Pubblicata = Pubblicata.Checked
        _finder.Esecutiva = Esecutiva.Checked
        _finder.StepAttivo = StepAttivo.Checked

        _finder.DateFrom = DateFrom.SelectedDate
        _finder.DateTo = DateTo.SelectedDate
        _finder.ContainerIds = String.Empty

        If ddlContainer.SelectedItem IsNot Nothing Then
            _finder.ContainerIds = ddlContainer.SelectedItem.Value
        End If

        _finder.ViewAllExecutive = ResolutionEnv.ViewAllExecutiveEnabled
        _finder.ResolutionObject = txtOggetto.Text.Trim()
        Select Case rblClausola.SelectedValue
            Case "AND"
                _finder.ResolutionObjectSearch = NHibernateProtocolFinder.ObjectSearchType.AllWords
            Case "OR"
                _finder.ResolutionObjectSearch = NHibernateProtocolFinder.ObjectSearchType.AtLeastWord
        End Select

        _finder.Note = Note.Text.Trim()

        _finder.Recipient = Recipient.GetContactText.Trim()
        _finder.Proposer = Proposer.GetContactText.Trim()
        _finder.Assignee = Assignee.GetContactText.Trim()
        _finder.Manager = Manager.GetContactText.Trim()

        If VisibleRecipientContact Then
            _contacts = uscDestInterop.GetContacts(False)

            If _contacts.Count > 0 Then
                _finder.InteropRecipients = _contacts(0).Contact.FullIncrementalPath
            End If
        End If

        If VisibleProposerContact Then
            _contacts = uscPropInterop.GetContacts(False)

            If _contacts.Count > 0 Then
                _finder.InteropProposers = _contacts(0).Contact.FullIncrementalPath
            End If
        End If

        If VisibleAssigneeContact Then
            _contacts = uscAssiInterop.GetContacts(False)

            If _contacts.Count > 0 Then
                _finder.InteropAssignees = _contacts(0).Contact.FullIncrementalPath
            End If
        End If

        If VisibleManagerContact Then
            _contacts = uscRespInterop.GetContacts(False)

            If _contacts.Count > 0 Then
                _finder.InteropManagers = _contacts(0).Contact.FullIncrementalPath
            End If
        End If

        _finder.ControllerOpinion = ControllerOpinion.Text.Trim()
        _finder.IdControllerStatus = ddlControllerStatus.SelectedItem.Value

        If uscCategory.HasSelectedCategories Then
            _finder.Categories = uscCategory.SelectedCategories.First().FullIncrementalPath
        End If
        _finder.IncludeChildCategories = chbCategoryChild.Checked

        _finder.ImmediatelyExecutive = chkImmediatelyExecutive.Checked
        _finder.ProposerDateFrom = DateProposerFromValue
        _finder.ProposerDateTo = DateProposerToValue
        _finder.AdoptionDateFrom = DateAdoptionFromValue
        _finder.AdoptionDateTo = DateAdoptionToValue
        _finder.OCSupervisoryBoard = chkSupervisoryBoard.Checked
        _finder.OCRegion = chkRegion.Checked
        _finder.OCManagement = chkManagement.Checked
        If (pnlConfSind.Visible) Then 'EF 20120126 Per compatibilità con tutti gli altri comportamenti, carico su Management il valore della conferenza dei sindaci
            _finder.OCManagement = chkConfSind.Checked
        End If
        _finder.OCCorteConti = chkCorteConti.Checked
        _finder.OCOther = chkOther.Checked
        _finder.StatusCancel = chkStatusCancel.Checked
        _finder.OnlyStatusCancel = chkOnlyStatusCancel.Checked

        If ResolutionEnv.ControlCheckResolutionEnabled AndAlso (Convert.ToInt32(rblChecked.SelectedValue) < 2) Then
            _finder.IsChecked = (Convert.ToInt32(rblChecked.SelectedValue) = 1)
        End If

        _finder.HasServiceNumber = Facade.ResolutionFacade.IsManagedProperty("ServiceNumber", ResolutionType.IdentifierDelibera)
        _finder.HasNumber = Facade.ResolutionFacade.IsManagedProperty("Number", ResolutionType.IdentifierDelibera)
        _finder.Configuration = DocSuiteContext.Current.ResolutionEnv.Configuration

        If DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("AUSL-PC") Then
            If Not _finder.SortExpressions.Keys.Any(Function(f) f.Eq("R.InclusiveNumber")) Then
                _finder.SortExpressions.Add("R.InclusiveNumber", "ASC")
            End If
        End If

        If trPrivacyPublication.Visible Then
            _finder.PrivacyPublication = rblPrivacyPublication.SelectedValue
        End If
    End Sub

    Protected Sub EnableRangeDate()
        DateFrom.Enabled = True
        DateTo.Enabled = True
    End Sub

    Protected Sub DisableRangeDate()
        If Not (Proposta.Checked OrElse Adottata.Checked OrElse Pubblicata.Checked OrElse Esecutiva.Checked OrElse Spedizione.Checked OrElse Ricezione.Checked OrElse Scadenza.Checked OrElse Risposta.Checked) Then
            DateFrom.Enabled = False
            DateTo.Enabled = False
        End If
    End Sub
    Public Sub BindControls()
        'Tipologia
        Delibera.Text = Facade.ResolutionTypeFacade.DeliberaCaption()
        Determina.Text = Facade.ResolutionTypeFacade.DeterminaCaption()
        'Containers
        BindContainersDelegate.Invoke(ddlContainer)
        'ControllerStatus
        BindControllerStatusDelegate.Invoke(ddlControllerStatus)
    End Sub

    Private Sub CheckOCVisibility()
        If (Delibera.Checked Xor Determina.Checked) Then
            VisibileOC_SupervisoryBoard = (Delibera.Checked And Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDelibera, "CS")) Or (Determina.Checked And Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDetermina, "CS"))
            VisibileOC_ConfSind = (Delibera.Checked And Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDelibera, "CONFSIND")) Or (Determina.Checked And Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDetermina, "CONFSIND"))
            VisibileOC_CorteConti = (Delibera.Checked And Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDelibera, "CC")) Or (Determina.Checked And Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDetermina, "CC"))
            VisibileOC_Management = (Delibera.Checked And Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDelibera, "GEST")) Or (Determina.Checked And Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDetermina, "GEST"))
            VisibileOC_Other = (Delibera.Checked And Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDelibera, "ALTRO")) Or (Determina.Checked And Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDetermina, "ALTRO"))
            VisibileOC_Region = (Delibera.Checked And Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDelibera, "REG")) Or (Determina.Checked And Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDetermina, "REG"))
        Else
            VisibileOC_SupervisoryBoard = Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDelibera, "CS") Or Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDetermina, "CS")
            VisibileOC_ConfSind = Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDelibera, "CONFSIND") Or Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDetermina, "CONFSIND")
            VisibileOC_CorteConti = Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDelibera, "CC") Or Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDetermina, "CC")
            VisibileOC_Management = Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDelibera, "GEST") Or Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDetermina, "GEST")
            VisibileOC_Other = Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDelibera, "ALTRO") Or Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDetermina, "ALTRO")
            VisibileOC_Region = Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDelibera, "REG") Or Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDetermina, "REG")
        End If
    End Sub

#End Region

End Class