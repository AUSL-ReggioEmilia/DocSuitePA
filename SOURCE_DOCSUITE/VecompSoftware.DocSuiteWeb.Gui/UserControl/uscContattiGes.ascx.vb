Imports System.Drawing
Imports System.Linq
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports System.Text
Imports VecompSoftware.Services.Logging
Imports System.Web
Imports System.Text.RegularExpressions
Imports VecompSoftware.DocSuiteWeb.Entity.Tenants
Imports VecompSoftware.DocSuiteWeb.Facade.Common.WebAPI
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Commons
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate

Partial Public Class uscContattiGes
    Inherits DocSuite2008BaseControl

#Region " Fields "

    Private _saveToDb As Boolean
    Private _manualContactMode As Boolean
    Private _contact As Contact
    Private _contactManual As ProtocolContactManual
    Private _simpleMode As Boolean

    Dim _description As String = ""
    Dim _pType As Boolean = False
    Protected WithEvents lblForm As System.Web.UI.WebControls.Label
    Protected WithEvents Textbox1 As System.Web.UI.WebControls.TextBox

#End Region

#Region " Properties "

    Protected ReadOnly Property OnlyManualDetail() As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean)("OnlyManualDetail", False)
        End Get
    End Property

    Protected ReadOnly Property IdContact() As Integer?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Integer?)("idContact", Nothing)
        End Get
    End Property

    Protected ReadOnly Property ActionType() As String
        Get
            Return Request.QueryString("ActionType")
        End Get
    End Property

    Protected ReadOnly Property Year() As Short
        Get
            Return CType(Request.QueryString("Year"), Short)
        End Get
    End Property

    Protected ReadOnly Property Number() As Integer
        Get
            Return CType(Request.QueryString("Number"), Integer)
        End Get
    End Property

    Protected ReadOnly Property Source() As String
        Get
            Return Request.QueryString("Source")
        End Get
    End Property

    Protected ReadOnly Property JsonContact() As String
        Get
            Return Request.QueryString("JsonContact")
        End Get
    End Property

    Protected ReadOnly Property ComunicationType() As String
        Get
            Return Request.QueryString("ComunicationType")
        End Get
    End Property

    Protected ReadOnly Property Record() As String
        Get
            Return Request.QueryString("Record")
        End Get
    End Property

    Public Property SimpleMode As Boolean
        Get
            Return _simpleMode
        End Get
        Set(value As Boolean)
            _simpleMode = DocSuiteContext.Current.ProtocolEnv.PECSimpleMode AndAlso value
        End Set
    End Property

    ''' <summary> Decide se lo user control effettua il salvataggio su DB. </summary>
    Public Property SaveToDb() As Boolean
        Get
            Return _saveToDb And Not _manualContactMode
        End Get
        Set(ByVal value As Boolean)
            _saveToDb = value
        End Set
    End Property

    ''' <summary> Imposta la modalità contatti manuali (true)/rubrica(false). </summary>
    Public Property ManualContactMode() As Boolean
        Get
            Return _manualContactMode
        End Get
        Set(ByVal value As Boolean)
            _manualContactMode = value
        End Set
    End Property

    ''' <summary> Indica se visualizzare il pannello dei settori </summary>
    ''' <remarks> TODO: non ha senso impedire a parametro solo la modifica settore, capire come eliminare questo parametro </remarks>
    Public Property ProtType() As Boolean
        Get
            Return _pType
        End Get
        Set(ByVal value As Boolean)
            _pType = value
        End Set
    End Property

    ''' <summary> Controllo in sola visualizzazione dettaglio </summary>
    ''' <remarks> Risitemare, basta usare la <seealso cref="uscContatti"/> in versione dettaglio per visualizzare questa cosa che esiste solo per l'IPA </remarks>
    Public Property IsReadOnly As Boolean
        Get
            If ViewState("ReadOnly") Is Nothing Then
                ViewState("ReadOnly") = False
            End If
            Return CType(ViewState("ReadOnly"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            ViewState("ReadOnly") = value
        End Set
    End Property

    Public Property IsFiscalCodeRequired() As Boolean
        Get
            Return rfvFiscalCode.Enabled
        End Get
        Set(ByVal value As Boolean)
            rfvFiscalCode.Enabled = value
        End Set
    End Property

    Private Property CurrentType As Char
        Get
            Return CType(ViewState("currenttype"), Char)
        End Get
        Set(value As Char)
            ViewState("currenttype") = value
        End Set
    End Property

    Public ReadOnly Property AjaxDefaultLoadingPanel() As RadAjaxLoadingPanel
        Get
            Return DefaultLoadingPanel
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

    Private Sub btnConferma_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConferma.Click
        ConfirmAction(True, DirectCast(sender, RadButton).CommandArgument)
    End Sub

    Private Sub btnConfermaNuovo_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConfermaNuovo.Click
        ConfirmAction(False, DirectCast(sender, RadButton).CommandArgument)
    End Sub

    Private Sub ToolBar_ButtonClick(sender As Object, e As RadToolBarEventArgs) Handles ToolBar.ButtonClick
        SetTipologia(e.Item.Value)
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma)
        AjaxManager.AjaxSettings.AddAjaxSetting(ToolBar, pnlAggiungi)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConfermaNuovo, pnlAggiungi)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, pnlAggiungi, DefaultLoadingPanel)

    End Sub

    Private Sub Initialize()
        pnlNationality.Visible = DocSuiteContext.Current.ProtocolEnv.ContactNationalityEnabled
        ddlTitoliStudio.DataValueField = "Id"
        ddlTitoliStudio.DataTextField = "Description"
        ddlPlaceName.DataValueField = "Id"
        ddlPlaceName.DataTextField = "Description"

        For Each lang As LanguageType In [Enum].GetValues(GetType(LanguageType))
            Dim val As Integer = lang
            ddlLanguageType.Items.Add(New ListItem(lang.GetDescription(), val.ToString()))
        Next

        lblNote.Text = "Note"
        txtNote.Enabled = True
        If Not ProtocolEnv.AVCPAddSelContactEnabled AndAlso IdContact.HasValue Then
            If Facade.ContactFacade.IsChildContact(ProtocolEnv.AVCPIdBusinessContact, IdContact.Value) Then
                rfvFiscalCode.Enabled = True
            End If
        ElseIf (ProtocolEnv.FascicleEnabled AndAlso ProtocolEnv.FascicleContactId > 0 AndAlso IdContact.HasValue) Then
            If Facade.ContactFacade.IsChildContact(ProtocolEnv.FascicleContactId, IdContact.Value) Then
                rfvFiscalCode.Enabled = True
            End If
        End If


        ''Check sulla PEC (unico per tutti)
        rfvCertifiedMail.Enabled = SimpleMode

        If DocSuiteContext.Current.IsResolutionEnabled AndAlso DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("ASL3-TO") Then
            If CommonUtil.HasGroupAdministratorRight Then
                lblNote.Text = "Gruppi"
            Else
                txtNote.Enabled = False
            End If
        End If

        If DocSuiteContext.Current.ProtocolEnv.SpidEnabled Then
            pnlBirthPlace.Visible = True
        End If

        If IdContact.HasValue Then
            _contact = Facade.ContactFacade.GetById(IdContact.Value, False)

            If _contact IsNot Nothing Then
                _description = Replace(_contact.Description, "|", " ")
                CurrentType = _contact.ContactType.Id
            End If
        Else
            _description = "Rubrica"
        End If

        If ManualContactMode Then
            Select Case Source
                Case "MM"
                    BasePage.Title = "Mittente"
                Case "DM"
                    BasePage.Title = "Destinatario"
            End Select
        End If

        Select Case BasePage.Action
            Case "Ins" 'Inserimento Contatto Manuale
                SetTipologia(ContactType.Aoo)
                If SimpleMode Then
                    txtCertifiedMail.Focus()
                Else
                    txtDescription.Focus()
                End If
                rfvDescription.Enabled = Not SimpleMode
                btnConfermaNuovo.Visible = True

            Case "Mod" 'Modifica Contatto Manuale: Form riempita lato client
                btnConfermaNuovo.Visible = False

                Dim v As String() = Split(Record, "^")
                SetTipologia(v(0))

                Select Case v(0)
                    Case ContactType.Aoo
                        txtDescription.Text = v(1)
                    Case ContactType.AdAmPerson
                        txtDescription.Text = v(1)
                    Case ContactType.Person
                        txtLastName.Text = v(2)
                        txtFirstName.Text = v(3)
                End Select

                If v(0) = ContactType.AdAmPerson Then
                    pnlEditType.Visible = False
                    btnConferma.Visible = False

                    ' Questo modo di programmare è orribile
                    ' todo: reimplementare questa funzione per disabilitare tutti i testi
                    For Each obj As Object In Page.Controls(0).Controls
                        Dim txt As TextBox = CType(obj, TextBox)
                        If txt IsNot Nothing AndAlso txt.ID.StartsWith("TXT", StringComparison.InvariantCultureIgnoreCase) Then
                            txt.Enabled = False
                        End If
                    Next

                    ddlPlaceName.Enabled = False
                End If

            Case "Add" 'Inserimento da Rubrica
                BasePage.Title = "Rubrica - Aggiungi "
                CurrentType = ActionType

                If CurrentType = ContactType.Person Then
                    txtLastName.Focus()
                Else
                    txtDescription.Focus()
                End If


                lblChildren.Visible = False
                chkChildren.Visible = False

            Case "Rename", "Vis"  'Visualizzazione di un contatto

                If ManualContactMode Then
                    Dim manualContact As String = JsonContact
                    If Not (String.IsNullOrEmpty(manualContact)) Then
                        _contact = JsonConvert.DeserializeObject(Of Contact)(manualContact, DocSuiteContext.DefaultJsonSerializerSettings)
                        _contact = Contact.EscapingJSON(_contact, Function(x) Server.UrlDecode(x))
                        _contact = Contact.EscapingJSON(_contact, Function(x) HttpUtility.HtmlDecode(x))
                    End If

                    If (_contact IsNot Nothing) AndAlso (_contact.ContactType IsNot Nothing) Then
                        SetTipologia(_contact.ContactType.Id)
                    End If

                    Select Case ComunicationType
                        Case "M"
                            Page.Title = "Mittente"
                        Case "D"
                            Page.Title = "Destinatario"
                    End Select

                    'disabilita i pulsanti di gestione e i campi per un contatto manuale
                    If OnlyManualDetail Then
                        ReadOnlyForm()
                        btnConferma.Visible = False
                        btnConfermaNuovo.Visible = False
                        pnlEditType.Visible = False
                    End If
                Else
                    BasePage.Title = "Rubrica - Modifica "
                End If

                If _contact IsNot Nothing Then
                    InizializeForm(_contact)
                    CurrentType = _contact.ContactType.Id
                End If

                If CurrentType = ContactType.Person Then
                    txtLastName.Focus()
                Else
                    txtDescription.Focus()
                End If

                lblChildren.Visible = CurrentType <> ContactType.Person
                chkChildren.Visible = CurrentType <> ContactType.Person

            Case "Del", "Recovery" 'Eliminazione\Recovery di un contatto
                If BasePage.Action.Eq("Del") Then
                    BasePage.Title = "Rubrica - Elimina "
                Else
                    BasePage.Title = "Rubrica - Recupera "
                End If

                If _contact IsNot Nothing Then
                    InizializeForm(_contact)
                End If

                'disabilita tutti i campi della form
                ReadOnlyForm()

                lblChildren.Visible = CurrentType <> ContactType.Person
                chkChildren.Visible = CurrentType <> ContactType.Person

            Case "Clone"
                BasePage.Title = "Rubrica - Clona "
                If _contact IsNot Nothing Then
                    InizializeForm(_contact)
                    CurrentType = _contact.ContactType.Id
                End If

        End Select

        If Not ManualContactMode Then
            BasePage.Title &= ContactTypeFacade.LegacyDescription(CurrentType)
        End If

        pnlDescrizione.Visible = CurrentType <> ContactType.Person
        pnlPersona.Visible = (CurrentType = ContactType.Person)
        pnlTitoliStudio.Visible = (CurrentType = ContactType.Person)

        trSettoreRubrica.Visible = ProtocolEnv.RoleContactEnabled AndAlso (CurrentType = ContactType.Sector) AndAlso CommonShared.HasGroupTblContactRight

        lblCertifiedMail.Text = "Posta certificata:"
        If SimpleMode Then
            lblCertifiedMail.Text = "Indirizzo di invio:"
        End If
        pnlCertifiedMail.Visible = DocSuiteContext.Current.ProtocolEnv.PECContactTypeEnabled.Contains(CurrentType)
        pnlFiscalCode.Visible = Not SimpleMode AndAlso ((CurrentType = ContactType.Aoo) OrElse (CurrentType = ContactType.Person))

        ' Inizializzo la treeview
        Tvw.Nodes.Clear()
        Dim tn As New RadTreeNode()
        tn.Text = _description
        If _contact IsNot Nothing AndAlso _contact.ContactType IsNot Nothing Then
            tn.ImageUrl = ImagePath.ContactTypeIcon(_contact.ContactType.Id)
        End If
        tn.Expanded = True
        Tvw.Nodes.Add(tn)

        trTrvContatto.Visible = Not ManualContactMode

        If OnlyManualDetail Then
            pnlEditType.Visible = False
            btnConfermaNuovo.Visible = False
        Else
            btnConfermaNuovo.Visible = ManualContactMode AndAlso Not (BasePage.Action.Eq("Rename") OrElse BasePage.Action.Eq("Vis") OrElse BasePage.Action.Eq("Clone"))
            If Not SimpleMode AndAlso ManualContactMode Then
                pnlEditType.Visible = True
                Dim defaultType As Char = If(_contact IsNot Nothing AndAlso _contact.ContactType IsNot Nothing, _contact.ContactType.Id, ContactType.Aoo)
                For Each coType As Char In {ContactType.Aoo, ContactType.Person}
                    Dim toolbarButton As New RadToolBarButton
                    toolbarButton.ToolTip = ContactTypeFacade.LegacyDescription(coType)
                    toolbarButton.ImageUrl = ImagePath.ContactTypeIcon(coType)
                    toolbarButton.Value = coType
                    toolbarButton.CheckOnClick = True
                    toolbarButton.Checked = (coType = defaultType)
                    toolbarButton.CausesValidation = False
                    ToolBar.Items.Add(toolbarButton)
                Next

            Else
                pnlEditType.Visible = False
            End If
        End If

        pnlContactRubrica.Visible = Not SimpleMode AndAlso Not ManualContactMode

        If ProtType Then
            pnlRole.Visible = False
        End If

        pnlSimpleMode.Visible = Not SimpleMode

        If BasePage.Action.Eq("clone") Then
            pnlCertifiedMail.Visible = False
            pnlContactRubrica.Visible = False
            pnlFiscalCode.Visible = False
            pnlSimpleMode.Visible = False
            pnlRole.Visible = False
            trBirthDay.Visible = False
            pnlBirthPlace.Visible = False
            pnlTitoliStudio.Visible = False
            pnlSDIIdentification.Visible = False
        End If

        If IsReadOnly Then
            pnlEditType.Visible = False
            ReadOnlyForm()
            pnlFooter.Visible = False
        End If
    End Sub

    ''' <summary> Valida gli indirizzi email solo se presenti. </summary>
    ''' <param name="contact"> Contatto da verificare. </param>
    Private Sub ValidateEmailAdresses(contact As Contact)
        Dim sb As New StringBuilder()
        Dim mailPattern As String = ProtocolEnv.ValidateEmailAdressesPattern

        If Not String.IsNullOrEmpty(contact.CertifiedMail) AndAlso Not Regex.IsMatch(contact.CertifiedMail, mailPattern, RegexOptions.IgnoreCase) Then
            Dim fieldName As String = lblCertifiedMail.Text.Replace(":", "")
            sb.AppendFormat("Campo - {0} - non valido.{1}", fieldName, Environment.NewLine)
        End If
        If Not String.IsNullOrEmpty(contact.EmailAddress) AndAlso Not Regex.IsMatch(contact.EmailAddress, mailPattern, RegexOptions.IgnoreCase) Then
            sb.AppendLine("Campo - Posta elettronica - non valido.")
        End If

        If sb.Length > 0 Then
            Throw New DocSuiteException("Validazione indirizzo email", sb.ToString())
        End If
    End Sub

    Private Sub PropagateRoleContact(contact As Contact, role As Role)
        If contact Is Nothing OrElse contact.Children.IsNullOrEmpty() OrElse role Is Nothing Then
            Return
        End If
        For Each child As Contact In contact.Children
            child.RoleRootContact = _contact.RoleRootContact
            PropagateRoleContact(child, role)
        Next
    End Sub
    Private Sub PropagateRoleContactUpdate(contact As Contact, role As Role)
        If contact Is Nothing OrElse contact.Children.IsNullOrEmpty() OrElse role Is Nothing Then
            Return
        End If

        For Each child As Contact In contact.Children
            child.RoleRootContact = _contact.RoleRootContact
            PropagateRoleContactUpdate(child, role)
            FacadeFactory.Instance.ContactFacade.UpdateOnly(child)
        Next

    End Sub

    ''' <summary> Crea un oggetto contatto recuperando i campi dalla form. </summary>
    ''' <param name="newContact">Se True l'oggetto creato è nuovo</param>
    Private Sub FillContactByForm(ByVal newContact As Boolean)

        If newContact Then
            _contact = New Contact()
            _contact.IsActive = True
            If IdContact.HasValue Then
                _contact.Parent = Facade.ContactFacade.GetById(IdContact.Value, False)
            End If
        Else
            _contact = Facade.ContactFacade.GetById(IdContact.Value, True)
        End If

        _contact.ContactType = New ContactType(CurrentType)

        If Not String.IsNullOrEmpty(ddlTitoliStudio.SelectedValue) Then
            _contact.StudyTitle = New ContactTitle()
            _contact.StudyTitle.Id = Integer.Parse(ddlTitoliStudio.SelectedValue)
        Else
            _contact.StudyTitle = Nothing
        End If

        _contact.Description = StringHelper.ConvertToNothing(If((CurrentType = ContactType.Person), txtLastName.Text.Trim() & "|" & txtFirstName.Text.Trim(), txtDescription.Text))
        _contact.BirthDate = txtBirthDate.SelectedDate
        _contact.BirthPlace = StringHelper.ConvertToNothing(txtBirthPlace.Text)
        _contact.Code = StringHelper.ConvertToNothing(txtCode.Text)
        _contact.SearchCode = StringHelper.ConvertToNothing(txtSearchCode.Text)
        _contact.FiscalCode = StringHelper.ConvertToNothing(txtFiscalCode.Text)

        _contact.Address = New Address()
        _contact.Address.Address = StringHelper.ConvertToNothing(txtAddress.Text)
        _contact.Address.CivicNumber = StringHelper.ConvertToNothing(txtCivicNumber.Text)
        _contact.Address.ZipCode = StringHelper.ConvertToNothing(txtZipCode.Text)
        _contact.Address.City = StringHelper.ConvertToNothing(txtCity.Text)
        _contact.Address.CityCode = StringHelper.ConvertToNothing(txtCityCode.Text)
        _contact.Address.Nationality = StringHelper.ConvertToNothing(txtNationality.Text)

        If Not String.IsNullOrEmpty(ddlLanguageType.SelectedValue) Then
            _contact.Address.Language = ddlLanguageType.SelectedValue
        Else
            _contact.Address.Language = Nothing
        End If


        If Not String.IsNullOrEmpty(ddlPlaceName.SelectedValue) Then
            _contact.Address.PlaceName = New ContactPlaceName()
            _contact.Address.PlaceName.Id = ddlPlaceName.SelectedValue
        Else
            _contact.Address.PlaceName = Nothing
        End If

        _contact.TelephoneNumber = StringHelper.ConvertToNothing(txtTelephoneNumber.Text)
        _contact.FaxNumber = StringHelper.ConvertToNothing(txtFaxNumber.Text)
        _contact.EmailAddress = StringHelper.ConvertToNothing(txtEMailAddress.Text)
        _contact.CertifiedMail = StringHelper.ConvertToNothing(txtCertifiedMail.Text)
        _contact.Note = StringHelper.ConvertToNothing(txtNote.Text)
        _contact.LastChangedDate = DateTime.Now
        _contact.isLocked = If(chkLocked.Checked, 1S, 0S)

        Dim roles As IList(Of Role) = uscAutorizza.GetRoles()
        If roles.IsNullOrEmpty() Then
            _contact.Role = Nothing
        Else
            _contact.Role = roles.First()
        End If

        ' Gestione rubrica di settore abilitata
        If ProtocolEnv.RoleContactEnabled AndAlso (CurrentType = ContactType.Sector) AndAlso CommonShared.HasGroupTblContactRight Then
            Dim roleContact As IList(Of Role) = uscSettoreRubrica.GetRoles()
            If roleContact.IsNullOrEmpty() Then
                _contact.RoleRootContact = Nothing
            Else
                _contact.RoleRootContact = roleContact.First()
            End If
        End If

        If ProtocolEnv.RoleContactEnabled AndAlso newContact AndAlso _contact.Parent IsNot Nothing AndAlso (_contact.Parent.ContactType.Id).Equals(ContactType.Sector) AndAlso _contact.Parent.RoleRootContact IsNot Nothing Then
            _contact.RoleRootContact = _contact.Parent.RoleRootContact
        End If

        If ProtocolEnv.RoleContactEnabled AndAlso Not newContact AndAlso (CurrentType = ContactType.Sector) Then
            'prima di aggiornare (problema di chiusura delle sessioni di nhibernate, è necessario leggere tutta 
            'l'alberatura, caricandola in memoria, e poi aggiornare. La doppia ricorsione risovle questo problema
            PropagateRoleContact(_contact, _contact.RoleRootContact)
            PropagateRoleContactUpdate(_contact, _contact.RoleRootContact)
        End If
    End Sub

    Dim escaping As Func(Of String, String) = Function(s) HttpUtility.HtmlEncode(s)

    ''' <summary> Conferma di inserimento di un contatto. </summary>
    Private Sub ConfirmAction(ByVal closeAfterAction As Boolean, Optional actionArgument As String = "")
        Dim contactJSon As String = String.Empty

        Try
            Select Case BasePage.Action
                Case "Ins", "Mod", "Vis" ' Inserimento Contatto Manuale
                    FillContactByForm(True)
                    ValidateEmailAdresses(_contact)
                    _contact = Contact.EscapingJSON(_contact, escaping)
                    contactJSon = JsonConvert.SerializeObject(_contact).Replace("'", "\'").Replace("\", "")


                Case "Add" ' Inserimento Contatto Rubrica
                    If Not String.IsNullOrEmpty(txtSearchCode.Text) Then
                        Dim contacts As IList(Of Contact) = Facade.ContactFacade.GetContactBySearchCode(txtSearchCode.Text, False)
                        If Not contacts.IsNullOrEmpty() Then
                            Dim code As String = contacts.First().SearchCode
                            Dim description As String = If(String.IsNullOrEmpty(contacts.First().Description), "", contacts.First().Description.Replace("|"c, " "c))
                            Throw New DocSuiteException("Inserimento contatto", String.Format("Codice [{0}] già utilizzato per utente [{1}]", code, description))
                        End If
                    End If

                    FillContactByForm(True)
                    If (ProtocolEnv.FascicleEnabled AndAlso ProtocolEnv.FascicleContactId > 0 AndAlso IdContact.HasValue) Then
                        If Facade.ContactFacade.IsChildContact(ProtocolEnv.FascicleContactId, IdContact.Value) Then
                            If String.IsNullOrEmpty(_contact.EmailAddress) AndAlso String.IsNullOrEmpty(_contact.CertifiedMail) Then
                                BasePage.AjaxAlert("Inserire almeno un valore nel campo Posta Elettronica o Posta certificata")
                                Exit Sub
                            End If
                        End If
                    End If
                    ValidateEmailAdresses(_contact)
                    Facade.ContactFacade.DuplicationCheck(_contact, CurrentTenant.TenantAOO.UniqueId)
                    Facade.ContactFacade.Save(_contact)

                Case "Rename" ' Modifica Contatto Rubrica

                    'Questo è il case in cui esegue la modifica del contatto
                    'Mantengo la modifica dell'oggetto _contact aggiorno il nome in Contact e inserisco il nuovo nome nella contactName col campo from date a adesso
                    ''
                    FillContactByForm(False)
                    If (ProtocolEnv.FascicleEnabled AndAlso ProtocolEnv.FascicleContactId > 0 AndAlso IdContact.HasValue) Then
                        If Facade.ContactFacade.IsChildContact(ProtocolEnv.FascicleContactId, IdContact.Value) Then
                            If String.IsNullOrEmpty(_contact.EmailAddress) AndAlso String.IsNullOrEmpty(_contact.CertifiedMail) Then
                                BasePage.AjaxAlert("Inserire almeno un valore nel campo Posta Elettronica o Posta certificata")
                                Exit Sub
                            End If
                        End If
                    End If
                    ValidateEmailAdresses(_contact)
                    Facade.ContactFacade.Update(_contact)

                Case "Del" ' Elimina Contatto Rubrica
                    FillContactByForm(False)
                    Dim disableAllChildren As Boolean = actionArgument.Eq("disableAllChildren")
                    Facade.ContactFacade.DisableContact(_contact, disableAllChildren)

                Case "Recovery"
                    FillContactByForm(False)
                    Dim activateAllChildren As Boolean = actionArgument.Eq("activateAllChildren")
                    Facade.ContactFacade.ActivateContact(_contact, activateAllChildren)

                Case "Clone"
                    Dim persistedContact As Contact = Facade.ContactFacade.GetById(IdContact.Value, False)
                    Dim newName As String = StringHelper.ConvertToNothing(Of String)(If(CurrentType.Equals(ContactType.Person), String.Concat(txtLastName.Text.Trim(), "|", txtFirstName.Text.Trim()), txtDescription.Text))
                    Facade.ContactFacade.Clone(persistedContact, newName)
                    _contact = persistedContact
            End Select

            AjaxManager.ResponseScripts.Add(String.Format("ReturnValuesJSon('{0}','{1}','{2}','{3}');", BasePage.Action, _contact.Id, HttpUtility.HtmlEncode(contactJSon), closeAfterAction))
            If Not closeAfterAction Then
                ClearForm()
            End If

        Catch ex As DocSuiteException
            FileLogger.Warn(LoggerName, "Errore in conferma azione", ex)
            BasePage.AjaxAlert(ex)
        Catch ex As Exception
            Throw New DocSuiteException("Conferma operazione sui contatti", String.Format("Errore in fase di [{0}].", BasePage.Action), ex)
        End Try
    End Sub

    ''' <summary>Inizializza la form con i campi di un contatto</summary>
    Private Sub InizializeForm(ByRef contact As Contact)
        If CurrentType = ContactType.Person Then
            ' La seguente gestione è stata introdotta per evitare l'overflow dell'indice qualora in "contact.Description" non fosse presente il separatore "Cognome|Nome".
            If contact.Description.Contains("|") Then
                txtLastName.Text = contact.Description.Split("|"c).GetValue(0)
                txtFirstName.Text = contact.Description.Split("|"c).GetValue(1)
            Else
                txtLastName.Text = contact.Description
                txtFirstName.Text = ""
            End If

            If Not contact.StudyTitle Is Nothing Then
                ddlTitoliStudio.SelectedValue = contact.StudyTitle.Id.ToString()
            End If
        Else
            txtDescription.Text = contact.Description
        End If
        txtBirthDate.SelectedDate = contact.BirthDate
        txtBirthPlace.Text = contact.BirthPlace
        txtCode.Text = contact.Code
        txtSearchCode.Text = contact.SearchCode
        txtFiscalCode.Text = contact.FiscalCode
        txtCertifiedMail.Text = If(SimpleMode AndAlso String.IsNullOrEmpty(contact.CertifiedMail), contact.EmailAddress, contact.CertifiedMail)
        txtSDIIdentification.Text = contact.SDIIdentification
        If contact.Address IsNot Nothing Then
            If contact.Address.PlaceName IsNot Nothing Then
                ddlPlaceName.SelectedValue = contact.Address.PlaceName.Id
            End If
            If contact.Address.Language IsNot Nothing Then
                ddlLanguageType.SelectedValue = contact.Address.Language
            End If

            txtAddress.Text = contact.Address.Address
            txtCivicNumber.Text = contact.Address.CivicNumber
            txtZipCode.Text = contact.Address.ZipCode
            txtCity.Text = contact.Address.City
            txtCityCode.Text = contact.Address.CityCode
            txtNationality.Text = contact.Address.Nationality
        End If
        txtTelephoneNumber.Text = contact.TelephoneNumber
        txtFaxNumber.Text = contact.FaxNumber
        txtEMailAddress.Text = contact.EmailAddress
        txtNote.Text = contact.Note

        If Not ProtType Then
            Dim authRoles As New List(Of Role)
            If _contact.Role IsNot Nothing Then
                authRoles.Add(_contact.Role)
            End If
            uscAutorizza.SourceRoles = authRoles
            uscAutorizza.DataBind()

            If ProtocolEnv.RoleContactEnabled AndAlso (CurrentType = ContactType.Sector) AndAlso CommonShared.HasGroupTblContactRight Then
                Dim rootRoles As New List(Of Role)
                If _contact.RoleRootContact IsNot Nothing Then
                    rootRoles.Add(_contact.RoleRootContact)
                End If
                uscSettoreRubrica.SourceRoles = rootRoles
                uscSettoreRubrica.DataBind()
            End If
        End If

        If contact.isLocked.HasValue AndAlso contact.isLocked.Value = 1 Then
            chkLocked.Checked = True
        Else
            chkLocked.Checked = False
        End If
    End Sub

    Private Sub ClearForm()
        txtDescription.Text = String.Empty
        txtLastName.Text = String.Empty
        txtFirstName.Text = String.Empty
        ddlTitoliStudio.SelectedIndex = 0
        txtBirthDate.SelectedDate = Nothing
        txtBirthPlace.Text = String.Empty
        txtCode.Text = String.Empty
        txtSearchCode.Text = String.Empty
        txtFiscalCode.Text = String.Empty
        txtCertifiedMail.Text = String.Empty
        ddlPlaceName.SelectedIndex = 0
        txtAddress.Text = String.Empty
        txtCivicNumber.Text = String.Empty
        txtZipCode.Text = String.Empty
        txtCity.Text = String.Empty
        txtCityCode.Text = String.Empty
        txtNationality.Text = String.Empty
        txtTelephoneNumber.Text = String.Empty
        txtFaxNumber.Text = String.Empty
        txtEMailAddress.Text = String.Empty
        txtNote.Text = String.Empty
        chkLocked.Checked = False

        If Not ProtType Then
            uscAutorizza.SourceRoles.Clear()
            uscAutorizza.DataBind()

            uscSettoreRubrica.SourceRoles.Clear()
            uscSettoreRubrica.DataBind()
        End If
    End Sub

    Private Sub ReadOnlyForm()
        rfvCertifiedMail.Enabled = False
        txtDescription.Enabled = False
        txtLastName.Enabled = False
        txtFirstName.Enabled = False
        ddlTitoliStudio.Enabled = False
        txtBirthDate.DateInput.DisabledStyle.BackColor = Color.White
        txtBirthDate.DateInput.Enabled = False
        txtBirthDate.DatePopupButton.Visible = False
        txtBirthPlace.Enabled = False
        txtCode.Enabled = False
        txtSearchCode.Enabled = False
        txtFiscalCode.Enabled = False
        txtCertifiedMail.Enabled = False
        ddlPlaceName.Enabled = False
        txtAddress.Enabled = False
        txtCivicNumber.Enabled = False
        txtZipCode.Enabled = False
        txtCity.Enabled = False
        txtCityCode.Enabled = False
        txtNationality.Enabled = False
        txtTelephoneNumber.Enabled = False
        txtFaxNumber.Enabled = False
        txtEMailAddress.Enabled = False
        txtNote.Enabled = False
        chkLocked.Enabled = False
        chkChildren.Enabled = False
        txtSDIIdentification.Enabled = False

        If Not ProtType Then
            uscAutorizza.ReadOnly = True

            uscSettoreRubrica.ReadOnly = True
        End If
    End Sub

    ''' <summary> Imposta la tipologia di contatto da visualizzare in pagina. </summary>
    Private Sub SetTipologia(ByVal tipo As Char)
        'tipologia inserimento contatto manuale protocollo

        'Per evitare di rifare le stesse operazioni se clicco sul medesimo bottone
        If tipo = CurrentType Then
            Exit Sub
        End If

        If ProtType Then
            pnlRole.Visible = False
            pnlCertifiedMail.Visible = False
        End If

        Select Case tipo
            Case ContactType.Person
                pnlDescrizione.Visible = False
                pnlPersona.Visible = True
                pnlTitoliStudio.Visible = True
                pnlCertifiedMail.Visible = True

            Case ContactType.Aoo
                pnlDescrizione.Visible = True
                pnlPersona.Visible = False
                pnlTitoliStudio.Visible = False
                pnlCertifiedMail.Visible = True

            Case ContactType.Ipa
                pnlCertifiedMail.Visible = True
            Case ContactType.AdAmPerson
                pnlDescrizione.Visible = True
                pnlPersona.Visible = False
                txtDescription.Enabled = False
                pnlTitoliStudio.Visible = False

            Case Else
                pnlCertifiedMail.Visible = False
        End Select

        CurrentType = tipo
    End Sub

#End Region

End Class