Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Newtonsoft.Json

Public Class TemplateProtocolInsert
    Inherits ProtBasePage

#Region "Fields"
    Private _currentTemplate As TemplateProtocol
    Private _idTemplate As Integer?
#End Region

#Region "Properties"
    Private Overloads ReadOnly Property CurrentContainerControl As ContainerControl
        Get
            Return New ContainerControl(cboIdContainer, rcbContainer)
        End Get
    End Property

    Private ReadOnly Property CurrentTemplateProtocol As TemplateProtocol
        Get
            If _currentTemplate IsNot Nothing Then
                Return _currentTemplate
            End If
            Select Case Action
                Case "add"
                    _currentTemplate = New TemplateProtocol()
                Case "modify"
                    If Not IdTemplate.HasValue Then
                        Throw New DocSuiteException("Nessun id template passato per la modifica.")
                    End If
                    _currentTemplate = Facade.TemplateProtocolFacade.GetById(IdTemplate.Value)
                Case "precompiler"
                    _currentTemplate = New TemplateProtocol()
                    If Not Session("TemplateProtocolPrecopiler") Is Nothing Then
                        _currentTemplate = DirectCast(Session("TemplateProtocolPrecopiler"), TemplateProtocol)
                    End If
            End Select
            Return _currentTemplate
        End Get
    End Property

    Private ReadOnly Property IdTemplate As Integer?
        Get
            If _idTemplate IsNot Nothing Then
                Return _idTemplate
            End If
            Dim param As String = Request.QueryString.GetValueOrDefault("idtemplate", String.Empty)
            If Not String.IsNullOrEmpty(param) Then
                _idTemplate = Integer.Parse(param)
                Return _idTemplate
            End If
            Return Nothing
        End Get
    End Property
#End Region

#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()

        'Abilitazione autorizzazioni
        If ProtocolEnv.IsAuthorizInsertEnabled Then
            pnlAutorizzazioni.Visible = True
            If ProtocolEnv.IsAuthorizInsertRolesEnabled Then
                AddHandler uscMittenti.ContactRemoved, AddressOf uscContact_RoleUpdate
                AddHandler uscMittenti.ContactAdded, AddressOf uscContact_RoleUpdate
                AddHandler uscDestinatari.ContactRemoved, AddressOf uscContact_RoleUpdate
                AddHandler uscDestinatari.ContactAdded, AddressOf uscContact_RoleUpdate
            End If
        End If

        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub uscContact_RoleUpdate(ByVal sender As Object, ByVal e As EventArgs)
        Dim contactList As New List(Of ContactDTO)
        contactList.AddRange(uscMittenti.GetContacts(False))
        contactList.AddRange(uscDestinatari.GetContacts(False))
        uscAutorizzazioni.LoadRoleContacts(contactList, False)
    End Sub

    Private Sub rblTipoProtocollo_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles rblTipoProtocollo.SelectedIndexChanged
        UpdateTipoProtocollo(False)
        pnlInvoice.Visible = False
        ContainerControlSelectionChanged()
        ResetProtocolKind()
    End Sub

    Private Sub Container_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cboIdContainer.SelectedIndexChanged, rcbContainer.SelectedIndexChanged
        ContainerControlSelectionChanged()
        UpdateTipoProtocollo()
        BindProtocolKind()
    End Sub

    Private Sub btnClose_Click(ByVal sender As Object, ByVal e As EventArgs)
        Response.Redirect("../Tblt/TbltTemplateProtocolManager.aspx")
    End Sub

    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As EventArgs)
        SaveTemplate()
    End Sub

    Protected Sub TemplateProtInserimento_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arg As String = e.Argument
        Select Case arg
            Case "save"
                btnSave_Click(sender, e)
            Case "saveprecopiler"
                btnSave_Click(sender, e)
            Case "close"
                btnClose_Click(sender, e)
        End Select
    End Sub
#End Region

#Region "Methods"

    Private Overloads Sub BindDocType(idContainer As Integer?)
        Dim availableDocTypes As IList(Of DocumentType)
        If idContainer.HasValue Then
            availableDocTypes = Facade.ContainerDocTypeFacade.ContainerDocTypeSearch(idContainer, True)
        Else
            availableDocTypes = Facade.DocumentTypeFacade.DocTypeSearch(0, True, ProtocolEnv.IsPackageEnabled, "")
        End If
        cboIdDocType.Items.Clear()
        For Each dt As DocumentType In availableDocTypes
            Dim currentItem As New ListItem(dt.Description, dt.Id)
            If ProtocolEnv.IsPackageEnabled AndAlso dt.NeedPackage Then
                If String.IsNullOrEmpty(dt.CommonUser) Then
                    currentItem.Text &= " (*)"
                Else
                    currentItem.Text &= " (#)"
                End If
            End If
            cboIdDocType.Items.Add(currentItem)
        Next
        If cboIdDocType.Items.Count > 1 Then
            cboIdDocType.Items.Insert(0, New ListItem(String.Empty, String.Empty))
            cboIdDocType.SelectedIndex = 0
        End If
    End Sub

    Private Sub ContainerControlSelectionChanged()
        If ProtocolEnv.IsTableDocTypeEnabled Then
            'Carico l'ultima impostazione della Tipologia spedizione fatta
            Dim prevSelectedDocType As String = cboIdDocType.SelectedValue

            If Not String.IsNullOrEmpty(CurrentContainerControl.SelectedValue) Then
                BindDocType(Integer.Parse(CurrentContainerControl.SelectedValue))
            Else
                BindDocType(Nothing)
            End If

            If Not String.IsNullOrEmpty(prevSelectedDocType) Then
                If cboIdDocType.Items.FindByValue(prevSelectedDocType) IsNot Nothing Then
                    cboIdDocType.ClearSelection()
                    cboIdDocType.Items.FindByValue(prevSelectedDocType).Selected = True
                End If
            End If
        End If

        If Not ProtocolEnv.IsInvoiceEnabled OrElse String.IsNullOrEmpty(CurrentContainerControl.SelectedValue) Then
            Exit Sub
        End If

        Dim containerExtFacade As New ContainerExtensionFacade("ProtDB")
        rblTipoProtocollo.Enabled = True
        Dim containerExtensions As IList(Of ContainerExtension) = containerExtFacade.GetByContainerAndKey(Integer.Parse(CurrentContainerControl.SelectedValue), ContainerExtensionType.FT)
        If containerExtensions.Count > 0 AndAlso containerExtensions(0).KeyValue = "1" Then
            Dim containerExtensionsSearch As IList(Of ContainerExtension) = containerExtFacade.GetByContainerAndKey(Integer.Parse(CurrentContainerControl.SelectedValue), ContainerExtensionType.SC)
            If Not containerExtensionsSearch Is Nothing Then
                ddlAccountingSectional.Items.Clear()
                For Each cntExt As ContainerExtension In containerExtensionsSearch
                    ddlAccountingSectional.Items.Add("" & cntExt.KeyValue)
                Next
                pnlSectionalType.Visible = True
            Else
                pnlSectionalType.Visible = False
            End If
            Dim containerExtensionsTest As IList(Of ContainerExtension) = containerExtFacade.GetByContainerAndKey(Integer.Parse(CurrentContainerControl.SelectedValue), ContainerExtensionType.TP)
            Select Case containerExtensionsTest(0).KeyValue
                Case "A"
                    rblTipoProtocollo.SelectedValue = "1"
                    rblTipoProtocollo.Enabled = False
                Case "P"
                    rblTipoProtocollo.SelectedValue = "-1"
                    rblTipoProtocollo.Enabled = False
                Case Else
                    AjaxAlert("Tipo Contenitore non gestito, Tipologie valide A/P")
            End Select

            pnlInvoice.Visible = True
            uscDestinatari.IsFiscalCodeRequired = True
            uscMittenti.IsFiscalCodeRequired = True
        Else
            pnlInvoice.Visible = False
            uscDestinatari.IsFiscalCodeRequired = False
            uscMittenti.IsFiscalCodeRequired = False
            If String.IsNullOrWhiteSpace(rblTipoProtocollo.SelectedValue) Then
                rblTipoProtocollo.SelectedValue = "-1"
            End If
        End If
    End Sub

    Private Sub Initialize()
        If Not CommonShared.UserConnectedBelongsTo(ProtocolEnv.TemplateGroups) Then
            Throw New DocSuiteException("Utente non abilitato alla gestione del template di Protocollo")
        End If

        ' Lunghezza oggetto
        uscOggetto.MaxLength = ProtocolEnv.ObjectMaxLength

        'Abilitazione Tipologia Documento
        pnlIdDocType.Visible = ProtocolEnv.IsTableDocTypeEnabled

        'Inizializzo il modello di Protocollo
        ddlProtKindList.Items.Add(New ListItem(ProtocolKind.Standard.GetDescription(), CType(ProtocolKind.Standard, Short).ToString()))
        ddlProtKindList.DataBind()
        pnlProtocolKind.Visible = False

        'fattura
        pnlInvoice.Visible = False

        'imposta la pagina in base alla tipologia selezionata
        rblTipoProtocollo.DataSource = Facade.ProtocolTypeFacade().GetTypes()
        rblTipoProtocollo.DataBind()

        Me.ContainerControlSelectionChanged()

        rblTipoProtocollo.SelectedValue = "-1"

        Me.UpdateTipoProtocollo()

        cbClaim.Visible = ProtocolEnv.IsClaimEnabled

        uscMittenti.ButtonIPAVisible = ProtocolEnv.IsIPAAUSEnabled
        uscDestinatari.ButtonIPAVisible = ProtocolEnv.IsIPAAUSEnabled

        'Sezione Classificatore
        uscClassificatori.Multiple = False

        uscDestinatari.ButtonImportVisible = ProtocolEnv.IsImportContactEnabled
        uscMittenti.ButtonImportVisible = ProtocolEnv.IsImportContactEnabled
        uscMittenti.ButtonManualMultiVisible = ProtocolEnv.EnableContactAndDistributionGroup
        uscDestinatari.ButtonManualMultiVisible = ProtocolEnv.EnableContactAndDistributionGroup

        'Impostazione Titolo Pagina
        btnSave.Visible = False
        Select Case Action
            Case "add"
                Title = "Inserimento nuovo Template di Protocollo"
            Case "modify"
                Title = "Modifica Template di Protocollo"
            Case "precompiler"
                Title = "Bozza di Protocollo"
                pnlTemplateName.Visible = False
                templateName.Text = "precompiler"
                pnlUscOggetto.Visible = False
                btnConfirm.Visible = False
                btnAnnulla.Visible = False
                btnSave.Visible = True
            Case Else
                Title = String.Empty
        End Select


        rblTipoProtocollo.DataSource = Facade.ProtocolTypeFacade().GetTypes()
        rblTipoProtocollo.DataBind()
        BindContainerControl()

        'Pulizia directory temporanea
        CommonInstance.UserDeleteTemp(TempType.I)

        pnlProtocolStatus.Visible = False
        If ProtocolEnv.IsStatusEnabled Then
            pnlProtocolStatus.Visible = True
            cboProtocolStatus.DataSource = Facade.ProtocolStatusFacade().GetByProtocolStatusExclusion("P")
            cboProtocolStatus.DataBind()
        End If

        BindPageFromTemplate()
    End Sub

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf TemplateProtInserimento_AjaxRequest

        AjaxManager.ClientEvents.OnRequestStart = "OnRequestStart"
        AjaxManager.ClientEvents.OnResponseEnd = "OnResponseEnd"

        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, MasterDocSuite.AjaxDefaultLoadingPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(rblTipoProtocollo, lblAssegnatario)

        AjaxManager.AjaxSettings.AddAjaxSetting(rblTipoProtocollo, uscMittenti, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblTipoProtocollo, uscDestinatari, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblTipoProtocollo, UpdatePanelProtocollo)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblTipoProtocollo, pnlContenitore)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblTipoProtocollo, pnlIdDocType)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblTipoProtocollo, pnlInvoice)

        If ProtocolEnv.IsAuthorizInsertEnabled Then
            AjaxManager.AjaxSettings.AddAjaxSetting(rblTipoProtocollo, uscAutorizzazioni, MasterDocSuite.AjaxDefaultLoadingPanel)
        End If

        ' inizializzo il container control
        CurrentContainerControl.AutoPostBack = False
        Dim ajaxified As New List(Of Control)
        ajaxified.AddRange({divLblNote, divTxtNote, divLblServiceCategory, divSelServiceCategory})

        If ProtocolEnv.ContainerBehaviourEnabled Then
            CurrentContainerControl.AutoPostBack = True
        End If

        If ProtocolEnv.IsInvoiceEnabled Then
            CurrentContainerControl.AutoPostBack = True
            ajaxified.AddRange({pnlInvoice, pnlSectionalType, lblAssegnatario, uscMittenti.TreeViewControl, uscDestinatari.TreeViewControl, uscMittenti.Header, uscDestinatari.Header, Me.rblTipoProtocollo})
        End If

        If ProtocolEnv.IsTableDocTypeEnabled Then
            CurrentContainerControl.AutoPostBack = True
            ajaxified.Add(pnlIdDocType)
        End If

        If ProtocolEnv.OChartEnabled Then
            CurrentContainerControl.AutoPostBack = True
            ajaxified.Add(uscMittenti)
            ajaxified.Add(uscDestinatari)
            If ProtocolEnv.IsAuthorizInsertEnabled Then
                ajaxified.Add(uscAutorizzazioni)
            End If
        End If

        'Inizializzo il modello di Protocollo
        If ProtocolEnv.ProtocolKindEnabled Then
            AjaxManager.AjaxSettings.AddAjaxSetting(rblTipoProtocollo, pnlProtocolKind)
            AjaxManager.AjaxSettings.AddAjaxSetting(CurrentContainerControl.ActiveControl, pnlProtocolKind)
        End If

        'Behaviour da scelta contenitore --> nel caso di implementazione più generica espandere questa modalità
        ajaxified.Add(uscClassificatori)
        ajaxified.Add(pnlUscOggetto)
        CurrentContainerControl.AddAjaxSettings(AjaxManager, ajaxified)

        ' inizializzo altro
        AjaxManager.AjaxSettings.AddAjaxSetting(cboIdDocType, divLblNote)
        AjaxManager.AjaxSettings.AddAjaxSetting(cboIdDocType, divTxtNote)
        AjaxManager.AjaxSettings.AddAjaxSetting(cboIdDocType, divLblServiceCategory)
        AjaxManager.AjaxSettings.AddAjaxSetting(cboIdDocType, divSelServiceCategory)

        'Chiamate Ajax pannelli autorizzazioni
        If ProtocolEnv.IsAuthorizInsertEnabled Then
            AjaxManager.AjaxSettings.AddAjaxSetting(uscAutorizzazioni, uscAutorizzazioni)
            AjaxManager.AjaxSettings.AddAjaxSetting(uscMittenti, uscAutorizzazioni)
            AjaxManager.AjaxSettings.AddAjaxSetting(uscDestinatari, uscAutorizzazioni)
            AjaxManager.AjaxSettings.AddAjaxSetting(uscAutorizzazioni, uscAutorizzazioni)
        End If

    End Sub

    Private Sub BindContainerControl()
        Dim containers As IList(Of Container) = Facade.ContainerFacade.GetAll("ProtDB")
        If Not containers.Any() Then
            Exit Sub
        End If

        CurrentContainerControl.Enabled = True
        CurrentContainerControl.ClearItems()
        CurrentContainerControl.AddItem(String.Empty, String.Empty)
        For Each container As Container In containers
            CurrentContainerControl.AddItem(container.Name, container.Id.ToString())
        Next
        If containers.Count.Equals(1) Then
            CurrentContainerControl.SelectedValue = containers.Single().Id.ToString()
            CurrentContainerControl.Enabled = False
            ContainerControlSelectionChanged()
            UpdateTipoProtocollo()
        End If
    End Sub

    Private Sub UpdateTipoProtocollo(Optional needBindContainer As Boolean = False)
        Select Case rblTipoProtocollo.SelectedValue
            Case "-1" ' Ingresso
                InitializeIngoingProtocolType()

            Case "1" ' Uscita
                InitializeOutgoingProtocolType()

            Case "0" ' Tra uffici
                InitializeBetweenOfficesProtocolType()

        End Select

        If (needBindContainer) Then
            BindContainerControl()
        End If
        uscMittenti.UpdateButtons()
        uscDestinatari.UpdateButtons()
    End Sub

    'Inizializzazione per tipo protocollo in Ingresso
    Private Sub InitializeIngoingProtocolType()
        lblAssegnatario.Text = "Assegnatario:"
        rblTipoProtocollo.Width = Unit.Pixel(5)

        uscMittenti.IsRequired = False
        uscMittenti.APIDefaultProvider = False
        uscMittenti.Enable()

        uscDestinatari.IsRequired = False
        uscDestinatari.APIDefaultProvider = True
        uscDestinatari.Disable()

        If ProtocolEnv.InnerContactRoot.HasValue Then
            uscMittenti.AddExcludeContact(ProtocolEnv.InnerContactRoot.Value)
            uscMittenti.ContactRoot = Nothing
            uscDestinatari.RemoveExcludeContact(ProtocolEnv.InnerContactRoot.Value)
            uscDestinatari.ContactRoot = ProtocolEnv.InnerContactRoot.Value
        End If

        ' Swap dei contatti da destinatari a mittenti
        If uscDestinatari.Count > 0 Then
            uscMittenti.DataSource = uscDestinatari.GetManualContacts()
            uscMittenti.DataBind()
            uscDestinatari.DataSource.Clear()
            uscDestinatari.DataBind()
        End If
    End Sub

    'Inizializzazione per tipo protocollo in Uscita
    Private Sub InitializeOutgoingProtocolType()
        lblAssegnatario.Text = "Proponente:"

        uscMittenti.APIDefaultProvider = True
        uscMittenti.Disable()

        uscDestinatari.APIDefaultProvider = False
        uscDestinatari.Enable()

        If ProtocolEnv.InnerContactRoot.HasValue Then
            uscMittenti.RemoveExcludeContact(ProtocolEnv.InnerContactRoot.Value)
            uscMittenti.ContactRoot = ProtocolEnv.InnerContactRoot.Value
            uscDestinatari.ContactRoot = Nothing
            If Not ProtocolEnv.IncludeInnerContactRootToRecipients Then
                uscDestinatari.AddExcludeContact(ProtocolEnv.InnerContactRoot.Value)
            End If
        End If

        ' Swap dei contatti dei mittenti a destinatari
        If uscMittenti.Count > 0 Then
            uscDestinatari.DataSource = uscMittenti.GetManualContacts()
            uscDestinatari.DataBind()
            uscMittenti.DataSource.Clear()
            uscMittenti.DataBind()
        End If
    End Sub

    Private Sub InitializeBetweenOfficesProtocolType()
        lblAssegnatario.Text = "Interno:"

        uscMittenti.APIDefaultProvider = True
        uscMittenti.Enable()

        uscDestinatari.APIDefaultProvider = True
        uscDestinatari.Enable()

        uscMittenti.ExcludeContacts = Nothing
        uscMittenti.ContactRoot = ProtocolEnv.InnerContactRoot
        uscDestinatari.ExcludeContacts = Nothing
        uscDestinatari.ContactRoot = ProtocolEnv.InnerContactRoot

        uscMittenti.Enable()
        uscDestinatari.Enable()
    End Sub

    Private Sub InitPanelProtocolKind()
        If ProtocolEnv.ProtocolKindEnabled Then
            If ddlProtKindList.Items.Count > 1 Then
                pnlProtocolKind.Visible = True
            Else
                pnlProtocolKind.Visible = False
            End If
        Else
            pnlProtocolKind.Visible = False
        End If
    End Sub

    Private Function GetSelectedProtocolKindId() As Short
        Dim kindId As Short
        Short.TryParse(ddlProtKindList.SelectedValue, kindId)
        Return kindId
    End Function

    Private Sub ResetProtocolKind()
        ddlProtKindList.SelectedValue = "0"
        ddlProtKindList.DataBind()
        pnlProtocolKind.Visible = False
    End Sub

    ''' <summary>
    ''' Eseguo il Bind della nuova gestione di Modello Protocollo (Generico per tutti i futuri modelli)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub BindProtocolKind()
        If Not ProtocolEnv.ProtocolKindEnabled Then
            Exit Sub
        End If

        If String.IsNullOrEmpty(CurrentContainerControl.SelectedValue) Then
            ResetProtocolKind()
            Exit Sub
        End If

        Dim idContainerSelected As Integer = CType(CurrentContainerControl.SelectedValue, Integer)
        Dim availableKinds As IList(Of ProtocolKind) = Facade.ProtocolFacade.AvailableProtocolKinds(idContainerSelected)

        ddlProtKindList.Items.Clear()
        'Abilitazione Tipologia di Protocollo
        Dim kinds As Array = [Enum].GetValues(GetType(ProtocolKind))
        For Each kind As ProtocolKind In kinds
            Dim index As Integer = kind
            If Not availableKinds.Any(Function(x) x.Equals(kind)) Then
                Continue For
            End If

            Dim item As ListItem = New ListItem(kind.GetDescription(), index.ToString())
            ddlProtKindList.Items.Add(item)
        Next
        ddlProtKindList.DataBind()

        InitPanelProtocolKind()
    End Sub

    Private Sub SaveTemplate()
        If String.IsNullOrEmpty(templateName.Text) Then
            AjaxAlert("Il campo Nome Template è obbligatorio")
            Exit Sub
        End If

        BindDataFromPage(CurrentTemplateProtocol)


        Select Case Action
            Case "add"
                CurrentTemplateProtocol.IdTemplateStatus = TemplateStatus.Fault
                'Salvo il protocollo
                Facade.TemplateProtocolFacade.Save(CurrentTemplateProtocol)
                'Aggiungo i settori
                BindRolesFromPage(CurrentTemplateProtocol)
                'Aggiorno Advanced
                Dim advanced As New TemplateAdvancedProtocol()
                CurrentTemplateProtocol.AddAdvancedProtocol(advanced)
                BindAdvancedTemplate(CurrentTemplateProtocol)
                'Aggiorno contatti
                BindContactFromPage(CurrentTemplateProtocol)
                'Aggiorno il protocollo
                CurrentTemplateProtocol.IdTemplateStatus = TemplateStatus.Active
                Facade.TemplateProtocolFacade.UpdateNoLastChange(CurrentTemplateProtocol)
            Case "modify"
                'Aggiungo i settori
                BindRolesFromPage(CurrentTemplateProtocol)
                'Aggiorno Advanced
                BindAdvancedTemplate(CurrentTemplateProtocol)
                'Aggiorno contatti
                BindContactFromPage(CurrentTemplateProtocol)
                'Aggiorno il protocollo
                Facade.TemplateProtocolFacade.UpdateOnly(CurrentTemplateProtocol)
            Case "precompiler"
                CurrentTemplateProtocol.IdTemplateStatus = TemplateStatus.Fault
                'Aggiungo i settori
                BindRolesFromPage(CurrentTemplateProtocol)
                'Aggiorno Advanced
                Dim advanced As New TemplateAdvancedProtocol()
                CurrentTemplateProtocol.AddAdvancedProtocol(advanced)
                BindAdvancedTemplate(CurrentTemplateProtocol)
                'Aggiorno contatti
                BindContactFromPage(CurrentTemplateProtocol, False)
                Session("TemplateProtocolPrecopiler") = CurrentTemplateProtocol

                Dim protocolXML As ProtocolXML = FromTemplateProtocolToProtocolXML()
                Dim serialized As String = StringHelper.EncodeJS(JsonConvert.SerializeObject(protocolXML))
                Dim jsScript As String = String.Format("ReturnValuesJSon('{0}','{1}');", "DOCUMENTUNITDRAFT", serialized)
                AjaxManager.ResponseScripts.Add(jsScript)

                Exit Sub
            Case Else
                AjaxAlert("L'azione della pagina non risulta corretta. Il template non verrà pertanto salvato.")
                Exit Sub
        End Select

        Response.Redirect("../Tblt/TbltTemplateProtocolManager.aspx")
    End Sub

    Private Function FromTemplateProtocolToProtocolXML() As ProtocolXML
        Dim protocolDraft As New ProtocolXML
        protocolDraft.Object = CurrentTemplateProtocol.ProtocolObject
        protocolDraft.Notes = CurrentTemplateProtocol.TemplateAdvancedProtocol.Note
        If Not CurrentTemplateProtocol.Container Is Nothing Then
            protocolDraft.Container = CurrentTemplateProtocol.Container.Id
        End If
        protocolDraft.Assignee = CurrentTemplateProtocol.TemplateAdvancedProtocol.Subject
        protocolDraft.ServiceCode = CurrentTemplateProtocol.TemplateAdvancedProtocol.ServiceCategory
        If Not CurrentTemplateProtocol.Category Is Nothing Then
            protocolDraft.Category = CurrentTemplateProtocol.Category.Id
        End If
        protocolDraft.Type = CurrentTemplateProtocol.Type.Id

        Dim listAuth As New List(Of Integer)
        For Each role As TemplateProtocolRole In CurrentTemplateProtocol.Roles
            listAuth.Add(role.Role.Id)
        Next
        protocolDraft.Authorizations = listAuth

        Dim listSender As New List(Of ContactBag)
        Dim listRecipients As New List(Of ContactBag)

        Dim contactSendersXML As List(Of ContactXML)
        Dim contactRecipientsXML As List(Of ContactXML)

        contactSendersXML = New List(Of ContactXML)
        contactRecipientsXML = New List(Of ContactXML)
        For Each templateProtocolContact As TemplateProtocolContact In CurrentTemplateProtocol.Contacts
            Dim contactXML As New ContactXML
            contactXML.Id = templateProtocolContact.Contact.Id
            contactXML.Type = templateProtocolContact.Contact.ContactType.Id.ToString()
            contactXML.Cc = templateProtocolContact.Type.Eq("CC")
            If templateProtocolContact.Id.ComunicationType.Eq(ProtocolContactCommunicationType.Sender) Then
                contactSendersXML.Add(contactXML)
            Else
                contactRecipientsXML.Add(contactXML)
            End If
        Next

        Dim contactBagSenderRubrica As New ContactBag
        contactBagSenderRubrica.Contacts = contactSendersXML
        contactBagSenderRubrica.SourceType = ContactTypeEnum.AddressBook
        listSender.Add(contactBagSenderRubrica)

        Dim contactBagRecipientsRubrica As New ContactBag
        contactBagRecipientsRubrica.Contacts = contactRecipientsXML
        contactBagRecipientsRubrica.SourceType = ContactTypeEnum.AddressBook
        listRecipients.Add(contactBagRecipientsRubrica)

        contactSendersXML = New List(Of ContactXML)
        contactRecipientsXML = New List(Of ContactXML)
        For Each item As TemplateProtocolContactManual In CurrentTemplateProtocol.ContactsManual
            Dim contactXML As New ContactXML
            If item.Contact.Description.Contains("|") Then
                Dim array As String() = item.Contact.Description.Split("|"c)
                If array.Count > 1 Then
                    contactXML.Surname = array(0)
                    contactXML.Name = array(1)
                Else
                    contactXML.Surname = array(0)
                End If
            Else
                contactXML.Description = item.Contact.Description
            End If
            contactXML.Type = item.Contact.ContactType.Id.ToString()
            contactXML.Cc = item.Type.Eq("CC")
            contactXML.StandardMail = item.Contact.EmailAddress
            contactXML.CertifiedMail = item.Contact.CertifiedMail
            contactXML.FiscalCode = item.Contact.FiscalCode

            ' Address
            If item.Contact.Address IsNot Nothing Then
                contactXML.Address = New AddressXML()
                contactXML.Address.Cap = item.Contact.Address.ZipCode
                contactXML.Address.City = item.Contact.Address.City
                contactXML.Address.Name = item.Contact.Address.Address
                contactXML.Address.Prov = item.Contact.Address.CityCode
                If item.Contact.Address.PlaceName IsNot Nothing Then
                    contactXML.Address.Type = item.Contact.Address.PlaceName.Description
                End If
            End If

            contactXML.Telephone = item.Contact.TelephoneNumber
            contactXML.Fax = item.Contact.FaxNumber
            contactXML.Notes = item.Contact.Note

            If item.Contact.BirthDate.HasValue Then
                contactXML.BirthDate = item.Contact.BirthDate.Value.ToString()
            Else
                contactXML.BirthDate = Nothing
            End If
            If item.ComunicationType.Eq(ProtocolContactCommunicationType.Sender) Then
                contactSendersXML.Add(contactXML)
            Else
                contactRecipientsXML.Add(contactXML)
            End If
        Next

        Dim contactBagManualeSenderRubrica As New ContactBag
        contactBagManualeSenderRubrica.Contacts = contactSendersXML
        contactBagManualeSenderRubrica.SourceType = ContactTypeEnum.Manual
        listSender.Add(contactBagManualeSenderRubrica)

        Dim contactBagManualeRecipientsRubrica As New ContactBag
        contactBagManualeRecipientsRubrica.Contacts = contactRecipientsXML
        contactBagManualeRecipientsRubrica.SourceType = ContactTypeEnum.Manual
        listRecipients.Add(contactBagManualeRecipientsRubrica)

        protocolDraft.Senders = listSender
        protocolDraft.Recipients = listRecipients

        Return protocolDraft
    End Function


    Private Sub BindPageFromTemplate()
        If (Action.Eq("modify") OrElse (Action.Eq("precompiler") AndAlso Not Session("TemplateProtocolPrecopiler") Is Nothing)) Then

            templateName.Text = CurrentTemplateProtocol.TemplateName

            If Not String.IsNullOrEmpty(CurrentTemplateProtocol.ProtocolObject) Then
                uscOggetto.Text = CurrentTemplateProtocol.ProtocolObject
            End If

            'Note
            If Not String.IsNullOrEmpty(CurrentTemplateProtocol.TemplateAdvancedProtocol.Note) Then
                txtNote.Text = CurrentTemplateProtocol.TemplateAdvancedProtocol.Note
            End If

            'Assegnatari/Proponente
            If Not String.IsNullOrEmpty(CurrentTemplateProtocol.TemplateAdvancedProtocol.Subject) Then
                uscContactAssegnatario.DataSource = CurrentTemplateProtocol.TemplateAdvancedProtocol.Subject
            End If

            'Categoria di servizio
            If Not String.IsNullOrEmpty(CurrentTemplateProtocol.TemplateAdvancedProtocol.ServiceCategory) Then
                SelServiceCategory.CategoryText = CurrentTemplateProtocol.TemplateAdvancedProtocol.ServiceCategory
            End If

            'Contenitore
            If CurrentTemplateProtocol.Container IsNot Nothing Then
                Dim exist As Boolean = CurrentContainerControl.HasItemWithValue(CurrentTemplateProtocol.Container.Id.ToString())
                If exist Then
                    CurrentContainerControl.SelectedValue = CurrentTemplateProtocol.Container.Id.ToString()
                    ContainerControlSelectionChanged()

                    'Accounting sectional
                    If Not String.IsNullOrEmpty(CurrentTemplateProtocol.TemplateAdvancedProtocol.AccountingSectional) Then
                        ddlAccountingSectional.SelectedValue = CurrentTemplateProtocol.TemplateAdvancedProtocol.AccountingSectional
                    End If

                    'ProtocolKind
                    If CurrentTemplateProtocol.IdProtocolKind.HasValue Then
                        Dim match As ListItem = ddlProtKindList.Items.FindByValue(CurrentTemplateProtocol.IdProtocolKind.ToString())
                        If match IsNot Nothing Then
                            ddlProtKindList.SelectedValue = CurrentTemplateProtocol.IdProtocolKind.ToString()
                            BindProtocolKind()
                        End If
                    End If
                End If
            End If

            'Tipologia protocollo
            If CurrentTemplateProtocol.Type IsNot Nothing Then
                rblTipoProtocollo.SelectedValue = CurrentTemplateProtocol.Type.Id.ToString()
                UpdateTipoProtocollo()
            End If

            'Settori
            For Each templateProtocolRole As TemplateProtocolRole In CurrentTemplateProtocol.Roles
                uscAutorizzazioni.SourceRoles.Add(templateProtocolRole.Role)
            Next
            uscAutorizzazioni.DataBind()
            'Classificatore
            If Not Session("TemplateProtocolPrecopiler") Is Nothing Then
                Dim category As New Category
                If CurrentTemplateProtocol.TemplateAdvancedProtocol.SubCategory IsNot Nothing Then
                    category = Facade.CategoryFacade.GetById(CurrentTemplateProtocol.TemplateAdvancedProtocol.SubCategory.Id)
                Else
                    category = Facade.CategoryFacade.GetById(CurrentTemplateProtocol.Category.Id)
                End If
                If category IsNot Nothing Then
                    uscClassificatori.DataSource.Add(category)
                End If
            Else
                If CurrentTemplateProtocol.TemplateAdvancedProtocol.SubCategory IsNot Nothing Then
                    uscClassificatori.DataSource.Add(CurrentTemplateProtocol.TemplateAdvancedProtocol.SubCategory)
                Else
                    uscClassificatori.DataSource.Add(CurrentTemplateProtocol.Category)
                End If
            End If
            uscClassificatori.DataBind()

            'Contatti
            For Each templateProtocolContact As TemplateProtocolContact In CurrentTemplateProtocol.Contacts
                Dim dto As New ContactDTO
                dto.Contact = templateProtocolContact.Contact
                dto.IsCopiaConoscenza = templateProtocolContact.Type.Eq("CC")
                dto.Type = ContactDTO.ContactType.Address
                If templateProtocolContact.Id.ComunicationType.Eq(ProtocolContactCommunicationType.Sender) Then
                    uscMittenti.DataSource.Add(dto)
                Else
                    uscDestinatari.DataSource.Add(dto)
                End If
            Next

            'Contatti manuali
            For Each templateProtocolContactManual As TemplateProtocolContactManual In CurrentTemplateProtocol.ContactsManual
                Dim dto As New ContactDTO
                dto.Contact = templateProtocolContactManual.Contact
                dto.IsCopiaConoscenza = templateProtocolContactManual.Type.Eq("CC")
                dto.Type = ContactDTO.ContactType.Manual
                If templateProtocolContactManual.ComunicationType.Eq(ProtocolContactCommunicationType.Sender) Then
                    uscMittenti.DataSource.Add(dto)
                Else
                    uscDestinatari.DataSource.Add(dto)
                End If
            Next

            uscMittenti.DataBind()
            uscDestinatari.DataBind()

            'Status
            If CurrentTemplateProtocol.TemplateAdvancedProtocol.Status IsNot Nothing Then
                cboProtocolStatus.SelectedValue = CurrentTemplateProtocol.TemplateAdvancedProtocol.Status.Id.ToString()
            End If

            If Not (CurrentTemplateProtocol.DocType Is Nothing) Then
                cboIdDocType.SelectedValue = CurrentTemplateProtocol.DocType.Id.ToString()
            End If
        End If
    End Sub

    Private Sub BindDataFromPage(ByRef template As TemplateProtocol)
        template.TemplateName = templateName.Text

        'Verifico tipo protocollo
        Dim tipoProtocollo As String = rblTipoProtocollo.SelectedValue
        If Not String.IsNullOrEmpty(tipoProtocollo) Then
            template.Type = GetSelectedProtocolType()
        End If

        'Verifico il contenitore
        template.Container = Nothing
        Dim container As String = CurrentContainerControl.SelectedValue
        If Not String.IsNullOrEmpty(container) Then
            template.Container = CurrentContainerControl.SelectedContainer("ProtDB")
        End If

        'Verifico protocolKind
        template.IdProtocolKind = Nothing
        If ProtocolEnv.IsInvoiceEnabled AndAlso ProtocolEnv.ProtocolKindEnabled Then
            Dim kind As String = ddlProtKindList.SelectedValue
            If Not String.IsNullOrEmpty(kind) Then
                template.IdProtocolKind = GetSelectedProtocolKindId()
            End If
        End If

        'Verifico il tipo di documento
        template.DocType = Nothing
        If ProtocolEnv.IsTableDocTypeEnabled Then
            Dim docType As String = cboIdDocType.SelectedValue
            If Not String.IsNullOrEmpty(docType) Then
                template.DocType = GetSelectedDocType()
            End If
        End If

        'Inserimento oggetto
        template.ProtocolObject = Nothing
        If Not String.IsNullOrEmpty(uscOggetto.Text) Then
            template.ProtocolObject = StringHelper.Clean(uscOggetto.Text, DocSuiteContext.Current.ProtocolEnv.RegexOnPasteString)
        End If
    End Sub

    Private Function GetSelectedProtocolTypeId() As Integer?
        Dim parsed As Integer
        If Integer.TryParse(rblTipoProtocollo.SelectedValue, parsed) Then
            Return parsed
        End If
        Return Nothing
    End Function

    Private Function GetSelectedProtocolType() As ProtocolType
        Dim selectedId As Integer? = GetSelectedProtocolTypeId()
        If selectedId.HasValue Then
            Return FacadeFactory.Instance.ProtocolTypeFacade.GetById(selectedId.Value)
        End If
        Return Nothing
    End Function

    Private Function GetSelectedDocType() As DocumentType
        Dim idDoc As Integer
        Dim docType As New DocumentType()
        If Not String.IsNullOrEmpty(cboIdDocType.SelectedValue) Then
            If Integer.TryParse(cboIdDocType.SelectedValue, idDoc) Then
                docType = Facade.DocumentTypeFacade.GetById(idDoc)
            End If
        End If
        Return docType
    End Function

    Private Sub BindRolesFromPage(ByRef template As TemplateProtocol)
        If ProtocolEnv.IsAuthorizInsertEnabled Then
            'Rimuovo i settori eliminati
            Dim removedRoles As ICollection(Of Integer) = uscAutorizzazioni.RoleListRemoved
            For Each roleId As Integer In removedRoles
                Dim templateToRemove As TemplateProtocolRole = template.Roles.SingleOrDefault(Function(s) s.Role.Id.Equals(roleId))
                If templateToRemove IsNot Nothing Then
                    template.Roles.Remove(templateToRemove)
                End If
            Next

            'Inserimento settori
            Dim rolesAdded As ICollection(Of Integer) = uscAutorizzazioni.RoleListAdded
            For Each roleId As Integer In rolesAdded
                Dim roleToAdd As Role = Facade.RoleFacade.GetById(roleId)
                template.AddRole(roleToAdd, DocSuiteContext.Current.User.FullUserName, Date.Now, If(DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled, "E", Nothing))
            Next
        End If
    End Sub

    Private Sub BindAdvancedTemplate(ByRef template As TemplateProtocol)
        'Verifico accounting sectional
        template.TemplateAdvancedProtocol.AccountingSectional = Nothing
        If ProtocolEnv.IsInvoiceEnabled Then
            Dim accSectional As String = ddlAccountingSectional.SelectedValue
            If Not String.IsNullOrEmpty(accSectional) Then
                template.TemplateAdvancedProtocol.AccountingSectional = accSectional
            End If
        End If

        'Inserimento status
        template.TemplateAdvancedProtocol.Status = Nothing
        If ProtocolEnv.IsStatusEnabled Then
            Dim status As String = cboProtocolStatus.SelectedValue
            If Not String.IsNullOrEmpty(status) Then
                template.TemplateAdvancedProtocol.Status = Facade.ProtocolStatusFacade.GetById(cboProtocolStatus.SelectedValue)
            End If
        End If

        'Inserimento classificatore
        template.Category = Nothing
        If uscClassificatori.SelectedCategories.Any() Then
            Dim selectedCategory As Category = uscClassificatori.SelectedCategories.First()
            Dim root As Category = selectedCategory.Root
            If selectedCategory.Equals(root) Then
                template.Category = selectedCategory
            Else
                template.Category = root
                template.TemplateAdvancedProtocol.SubCategory = selectedCategory
            End If
        End If

        'Note
        template.TemplateAdvancedProtocol.Note = Nothing
        If Not String.IsNullOrEmpty(txtNote.Text) Then
            template.TemplateAdvancedProtocol.Note = StringHelper.Clean(txtNote.Text, DocSuiteContext.Current.ProtocolEnv.RegexOnPasteString)
        End If

        'Assegnatari/Proponente
        template.TemplateAdvancedProtocol.Subject = Nothing
        If Not String.IsNullOrEmpty(uscContactAssegnatario.GetContactText()) Then
            template.TemplateAdvancedProtocol.Subject = uscContactAssegnatario.GetContactText()
        End If

        'Categoria di servizio
        template.TemplateAdvancedProtocol.ServiceCategory = Nothing
        If Not String.IsNullOrEmpty(SelServiceCategory.CategoryText) Then
            template.TemplateAdvancedProtocol.ServiceCategory = SelServiceCategory.CategoryText
        End If
    End Sub

    Private Sub BindContactFromPage(ByRef template As TemplateProtocol, Optional LastChange As Boolean = True)
        template.Contacts.Clear()
        template.ContactsManual.Clear()
        If LastChange Then
            Facade.TemplateProtocolFacade.UpdateNoLastChange(template)
        End If


        ' Inserimento Mittenti
        Dim mittenti As IList(Of ContactDTO) = uscMittenti.GetContacts(True)
        If mittenti.Any() Then
            For Each contact As ContactDTO In uscMittenti.GetContacts(True)
                Select Case contact.ContactPart
                    Case ContactTypeEnum.AddressBook
                        template.AddSender(contact.Contact, contact.IsCopiaConoscenza)
                    Case Else
                        template.AddSenderManual(contact.Contact, contact.IsCopiaConoscenza)
                End Select
            Next
        End If

        ' Inserimento Destinatari
        For Each contact As ContactDTO In uscDestinatari.GetContacts(True)
            Select Case contact.ContactPart
                Case ContactTypeEnum.AddressBook
                    template.AddRecipient(contact.Contact, contact.IsCopiaConoscenza)
                Case Else
                    template.AddRecipientManual(contact.Contact, contact.IsCopiaConoscenza)
            End Select
        Next
    End Sub
#End Region

End Class