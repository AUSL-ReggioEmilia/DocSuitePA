Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.Helpers.UDS
Imports VecompSoftware.Helpers.WebAPI
Imports VecompSoftware.DocSuiteWeb.Facade.Common.UDS
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Facade.ProtocolParerFacade

Partial Public Class uscProtocollo
    Inherits DocSuite2008BaseControl

    Public Event RefreshContact(ByVal sender As Object, ByVal e As EventArgs)
    Public Event MittenteAdded(ByVal sender As Object, ByVal e As EventArgs)
    Public Event DestinatarioAdded(ByVal sender As Object, ByVal e As EventArgs)

#Region " Fields "

    Private _contactLoaded As Boolean = False
    Private _currentProtocol As Protocol
    Private _currentProtocolRights As ProtocolRights
    Private _linkablePec As Boolean?
    Public Const UDS_ADDRESS_NAME As String = "API-UDSAddress"
    Private Const ODATA_EQUAL_UDSID As String = "$filter=UDSId eq {0}"
    Private _currentUDSFacade As UDSFacade
    Private _webAPIHelper As IWebAPIHelper
#End Region

#Region " Properties "

    Public ReadOnly Property ButtonViewUDS As RadButton
        Get
            Return btnViewUDS
        End Get
    End Property

    Public ReadOnly Property CurrentUDSFacade As UDSFacade
        Get
            If _currentUDSFacade Is Nothing Then
                _currentUDSFacade = New UDSFacade()
            End If
            Return _currentUDSFacade
        End Get
    End Property

    Public Property CurrentProtocol() As Protocol
        Get
            If _currentProtocol Is Nothing Then
                If TypeOf Page Is ProtBasePage Then
                    _currentProtocol = DirectCast(Page, ProtBasePage).CurrentProtocol
                End If
            End If
            Return _currentProtocol
        End Get
        Set(ByVal value As Protocol)
            _currentProtocol = value
            ' TODO: rimuovere questi abomini
            If uscDestinatari IsNot Nothing Then
                uscDestinatari.CurrentProtocol = value
            End If
            If uscMittenti IsNot Nothing Then
                uscMittenti.CurrentProtocol = value
            End If
        End Set
    End Property

    Public ReadOnly Property CurrentProtocolRights As ProtocolRights
        Get
            If _currentProtocolRights Is Nothing Then
                _currentProtocolRights = New ProtocolRights(CurrentProtocol, CurrentProtocol.IdStatus.GetValueOrDefault(ProtocolStatusId.Attivo) = ProtocolStatusId.Annullato)
            End If

            Return _currentProtocolRights
        End Get
    End Property

    Public Property VisibleAssegnatario() As Boolean
        Get
            Return rowAssegnatario.Visible
        End Get
        Set(ByVal value As Boolean)
            rowAssegnatario.Visible = value
        End Set
    End Property

    Public Property VisibleProponente() As Boolean
        Get
            Return rowProponente.Visible
        End Get
        Set(ByVal value As Boolean)
            rowProponente.Visible = value
        End Set
    End Property

    Public Property VisibleProtocollo() As Boolean
        Get
            Return tblProtocollo.Visible
        End Get
        Set(ByVal value As Boolean)
            tblProtocollo.Visible = value
        End Set
    End Property

    Public Property VisibleTipoDocumento() As Boolean
        Get
            Return tblTipoDocumento.Visible
        End Get
        Set(ByVal value As Boolean)
            tblTipoDocumento.Visible = value
        End Set
    End Property

    Public Property VisibleFatturazione() As Boolean
        Get
            Return tblInvoice.Visible
        End Get
        Set(ByVal value As Boolean)
            tblInvoice.Visible = value
        End Set
    End Property

    Public Property VisibleStatoProtocollo() As Boolean
        Get
            Return tblStatusProt.Visible
        End Get
        Set(ByVal value As Boolean)
            tblStatusProt.Visible = value
        End Set
    End Property

    Public Property VisibleOggetto() As Boolean
        Get
            Return tblOggetto.Visible
        End Get
        Set(ByVal value As Boolean)
            tblOggetto.Visible = value
        End Set
    End Property

    Public Property VisibleProtocolloMittente() As Boolean
        Get
            Return tblSenderProt.Visible
        End Get
        Set(ByVal value As Boolean)
            tblSenderProt.Visible = value
        End Set
    End Property

    Public Property VisibleMittentiDestinatari() As Boolean
        Get
            Return tblMittentiDestinatari.Visible
        End Get
        Set(ByVal value As Boolean)
            tblMittentiDestinatari.Visible = value
            ' TODO: rifattorizzare, non si può caricare un controllo impostando una proprietà di visualizzazione
            If (value = True) Then
                LoadContacts(True)
            End If
        End Set
    End Property

    Public Property VisibleMittentiButtonDelete() As Boolean
        Get
            Return uscMittenti.ButtonDeleteVisible
        End Get
        Set(ByVal value As Boolean)
            uscMittenti.ButtonDeleteVisible = value
        End Set
    End Property

    Public Property VisibleDestinatariButtonDelete() As Boolean
        Get
            Return uscDestinatari.ButtonDeleteVisible
        End Get
        Set(ByVal value As Boolean)
            uscDestinatari.ButtonDeleteVisible = value
        End Set
    End Property

    Public Property VisibleClassificazione() As Boolean
        Get
            Return tblClassificazione.Visible
        End Get
        Set(ByVal value As Boolean)
            tblClassificazione.Visible = value
        End Set
    End Property

    Public Property VisibleAnnullamento() As Boolean
        Get
            Return tblAnnullamento.Visible
        End Get
        Set(ByVal value As Boolean)
            tblAnnullamento.Visible = value
        End Set
    End Property

    Public Property VisibleSettori() As Boolean
        Get
            Return uscSettori.Visible
        End Get
        Set(ByVal value As Boolean)
            uscSettori.Visible = value
            If (value = True) Then
                LoadRoles()
            End If
        End Set
    End Property

    Public Property VisibleFascicolo() As Boolean
        Get
            Return tblFascicoli.Visible
        End Get
        Set(ByVal value As Boolean)
            tblFascicoli.Visible = value
        End Set
    End Property

    Public Property VisibleAltri() As Boolean
        Get
            Return tblAltri.Visible
        End Get
        Set(ByVal value As Boolean)
            tblAltri.Visible = value
        End Set
    End Property

    ''' <summary> Hide/Show del Pannello assegnatario di protocollo </summary>
    ''' <remarks> enav </remarks>
    Public Property VisibleHandler() As Boolean
        Get
            Return tblAssegnatario.Visible
        End Get
        Set(ByVal value As Boolean)
            tblAssegnatario.Visible = value
        End Set
    End Property

    Public Property VisibleScatolone() As Boolean
        Get
            Return tblScatoloni.Visible
        End Get
        Set(ByVal value As Boolean)
            tblScatoloni.Visible = value
        End Set
    End Property

    Public Property VisibleClaim() As Boolean
        Get
            Return cbClaim.Visible
        End Get
        Set(ByVal value As Boolean)
            cbClaim.Visible = value
        End Set
    End Property

    Public Property VisibleNote As Boolean
        Get
            Return trNote.Visible
        End Get
        Set(value As Boolean)
            trNote.Visible = value
        End Set
    End Property

    Public Property VisibleRefusedTreeView() As Boolean
        Get
            Return tbltRefusedAuthorizations.Visible
        End Get
        Set(ByVal value As Boolean)
            tbltRefusedAuthorizations.Visible = value
        End Set
    End Property

    Public Property VisibleSourceCollaboration As Boolean
        Get
            Return trSourceCollaboration.Visible
        End Get
        Set(value As Boolean)
            trSourceCollaboration.Visible = value AndAlso DocSuiteContext.Current.ProtocolEnv.CollaborationSourceProtocolEnabled
        End Set
    End Property
    Public Property VisibleMulticlassification As Boolean
        Get
            Return uscMulticlassificationRest.Visible
        End Get
        Set(ByVal value As Boolean)
            uscMulticlassificationRest.Visible = value
        End Set
    End Property

    Protected Function WindowWidth() As Integer
        Return ProtocolEnv.PECWindowWidth
    End Function

    Protected Function WindowHeight() As Integer
        Return ProtocolEnv.PECWindowHeight
    End Function

    Protected Function WindowBehaviors() As String
        Return ProtocolEnv.PECWindowBehaviors
    End Function

    Protected Function WindowPosition() As String
        Return ProtocolEnv.PECWindowPosition
    End Function

    Public ReadOnly Property ControlSenders() As uscContattiSel
        Get
            Return uscMittenti
        End Get
    End Property

    Public ReadOnly Property ControlRecipients() As uscContattiSel
        Get
            Return uscDestinatari
        End Get
    End Property

    Public Property ContactModifyEnable() As Boolean
        Get
            If ViewState("_contactModify") Is Nothing Then
                Return False
            End If
            Return CType(ViewState("_contactModify"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            ViewState("_contactModify") = value
            If value = True Then
                VisibleMittentiDestinatari = True
            End If
        End Set
    End Property

    Public Property ContactMittenteModifyEnable() As Boolean
        Get
            If ViewState("_contactMittModify") Is Nothing Then
                Return False
            End If
            Return CType(ViewState("_contactMittModify"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            ViewState("_contactMittModify") = value
            If value = True Then
                VisibleMittentiDestinatari = True
            End If
        End Set
    End Property

    Public Property ContactDestinatariModifyEnable() As Boolean
        Get
            If ViewState("_contactDestModify") Is Nothing Then
                Return False
            End If
            Return CType(ViewState("_contactDestModify"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            ViewState("_contactDestModify") = value
            If value = True Then
                VisibleMittentiDestinatari = True
            End If
        End Set
    End Property

    Public Property ClaimModifyEnable() As Boolean
        Get
            Return cbClaim.Enabled
        End Get
        Set(ByVal value As Boolean)
            cbClaim.Enabled = value
        End Set
    End Property

    Public Property IsClaim() As Boolean
        Get
            Return cbClaim.Checked
        End Get
        Set(ByVal value As Boolean)
            cbClaim.Checked = value
        End Set
    End Property

    ''' <summary> Indica se l'utente può visualizzare la pec </summary>
    ''' <remarks>
    ''' Verifico che l'utente corrente abbia i permessi per visualizzare i documenti e di conseguenza abilito il link o meno ai documenti
    ''' </remarks>
    Private ReadOnly Property LinkablePec As Boolean
        Get
            If Not _linkablePec.HasValue Then
                _linkablePec = CurrentProtocolRights.IsDocumentReadable
            End If
            Return _linkablePec.Value
        End Get
    End Property

    Public Property ViewUDSSource As Boolean
        Get
            If ViewState("viewUDSSource") Is Nothing Then
                Return False
            End If
            Return CType(ViewState("viewUDSSource"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            ViewState("viewUDSSource") = value
        End Set
    End Property
    Public Property VisibleParer As Boolean
        Get
            Return tblParer.Visible
        End Get
        Set(value As Boolean)
            tblParer.Visible = value
        End Set
    End Property

    Public Property VisibleInvoicePA As Boolean
        Get
            Return tblInvoicePA.Visible
        End Get
        Set(value As Boolean)
            tblInvoicePA.Visible = value
        End Set
    End Property

    Public Property VisiblePosteWeb As Boolean
        Get
            Return tblPosteWeb.Visible
        End Get
        Set(value As Boolean)
            tblPosteWeb.Visible = value
        End Set
    End Property

    Public Property CurrentRelatedUDS As Entity.UDS.UDSDocumentUnit

    Public Property VisibleUDReferences As Boolean
        Get
            Return rowDocumentUnitReference.Visible
        End Get
        Set(ByVal value As Boolean)
            rowDocumentUnitReference.Visible = value
        End Set
    End Property
#End Region

#Region " Events "

    Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
        ' TODO: perchè inizializzare così? dangerous!
        CurrentProtocol = Nothing
        VisibleAltri = False
        VisibleAnnullamento = False
        VisibleAssegnatario = False
        VisibleClassificazione = False
        VisibleFascicolo = False
        VisibleFatturazione = False
        VisibleMittentiDestinatari = False
        VisibleOggetto = False
        VisibleProponente = False
        VisibleProtocollo = False
        VisibleProtocolloMittente = False
        VisibleSettori = False
        VisibleStatoProtocollo = False
        VisibleTipoDocumento = False
        VisibleScatolone = False
        ClaimModifyEnable = False
        lblTitle.Text = "Protocollo del Documento"


    End Sub

    Private Sub PageLoad(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()

        If (ProtocolEnv.RoleContactHistoricizing) Then
            uscMittenti.HistoricizeDate = CurrentProtocol.RegistrationDate.ToLocalTime.DateTime
            uscDestinatari.HistoricizeDate = CurrentProtocol.RegistrationDate.ToLocalTime.DateTime

        End If

        RedundantLegacyLoader()

        If Not Page.IsPostBack Then
            Initialize()
        End If
    End Sub
    Private Sub uscDestinatari_ShowList(ByVal sender As Object, ByVal e As EventArgs) Handles uscDestinatari.ShowContactList
        RaiseEvent RefreshContact(uscDestinatari, e)
    End Sub

    Private Sub uscMittenti_ShowList(ByVal sender As Object, ByVal e As EventArgs) Handles uscMittenti.ShowContactList
        RaiseEvent RefreshContact(uscMittenti, e)
    End Sub

    Protected Sub uscMittenti_ContactAdded(ByVal sender As Object, ByVal e As EventArgs) Handles uscMittenti.ContactAdded
        RaiseEvent MittenteAdded(sender, e)
    End Sub

    Protected Sub uscMittenti_ManualContactAdded(ByVal sender As Object, ByVal e As EventArgs) Handles uscMittenti.ManualContactAdded
        RaiseEvent MittenteAdded(sender, e)
    End Sub

    Protected Sub uscDestinatari_ContactAdded(ByVal sender As Object, ByVal e As EventArgs) Handles uscDestinatari.ContactAdded
        RaiseEvent DestinatarioAdded(sender, e)
    End Sub

    Protected Sub uscDestinatari_ManualContactAdded(ByVal sender As Object, ByVal e As EventArgs) Handles uscDestinatari.ManualContactAdded
        RaiseEvent DestinatarioAdded(sender, e)
    End Sub

    Private Sub uscProtocolRoleUser_OnRoleUserViewManagersChanged(sender As Object, e As EventArgs) Handles uscProtocolRoleUser.OnRoleUserViewManagersChanged
        BindProtocolRoleUsers(CType(uscProtocolRoleUser.CurrentRoleUserViewMode, uscSettori.RoleUserViewMode))
    End Sub

    Private Sub uscProtocolRoleUser_OnRoleUserViewModeChanged(ByVal sender As Object, ByVal e As EventArgs) Handles uscProtocolRoleUser.OnRoleUserViewModeChanged
        If uscProtocolRoleUser.CurrentRoleUserViewMode = CByte(uscSettori.RoleUserViewMode.RoleUsers) Then
            BindProtocolRoleUsers(uscSettori.RoleUserViewMode.Roles)
        Else
            BindProtocolRoleUsers(uscSettori.RoleUserViewMode.RoleUsers)
        End If
    End Sub

    Protected Sub dgPosteRequestContact_OnItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles dgPosteRequestContact.ItemDataBound
        If e.Item.ItemType <> GridItemType.Item AndAlso e.Item.ItemType <> GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim polRequest As POLRequestRecipientHeader = DirectCast(e.Item.DataItem, POLRequestRecipientHeader)

        Dim imgType As Image = DirectCast(e.Item.FindControl("imgType"), Image)
        imgType.ImageUrl = Prot.PosteWeb.Ricerca.GetPolRequestTypeImage(polRequest.RequestType)
        imgType.AlternateText = Prot.PosteWeb.Ricerca.GetPolRequestTypeText(polRequest.RequestType)

        Dim lblCost As Label = DirectCast(e.Item.FindControl("lblCost"), Label)
        lblCost.Text = If(polRequest.Costo.HasValue, polRequest.Costo.Value.ToString("0.00"), "-")

    End Sub

    Protected Sub UDSDynamicControls_OnNeedDynamicsSource(sender As Object, e As EventArgs) Handles udsDynamicControls.OnNeedDynamicsSource
        If CurrentRelatedUDS Is Nothing OrElse Not ViewUDSSource Then
            Exit Sub
        End If

        Try
            Dim currentUDSDocumentUnit As Entity.UDS.UDSDocumentUnit = CurrentRelatedUDS
            udsDynamicControls.ResetState()
            Dim repository As UDSRepository = GetCurrentUDSRepository()
            Dim udsSource As UDSDto = GetCurrentUDSSource(repository)

            btnViewUDS.Text = String.Format("{0} {1}/{2:0000000}", repository.Name, udsSource.Year, udsSource.Number)
            Dim schemaRepositoryModel As UDSModel = GetCurrentSchemaRepositoryModel(repository)
            udsDynamicControls.LoadDynamicControls(schemaRepositoryModel.Model, False)
            udsDynamicControls.SetUDSValues(udsSource.UDSModel.Model)
        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            tblUds.SetDisplay(False)
        End Try
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(udsDynamicControls, udsDynamicControls)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscProtocolRoleUser, uscProtocolRoleUser)
    End Sub

    Private Sub Initialize()

        If ProtocolEnv.RoleContactHistoricizing Then
            uscMittenti.HistoricizeDate = CurrentProtocol.RegistrationDate.ToLocalTime().DateTime
            uscDestinatari.HistoricizeDate = CurrentProtocol.RegistrationDate.ToLocalTime().DateTime
        End If

        InitializeSourceCollaboation()
        InitializeUDSDynamicsControl()

        uscMulticlassificationRest.IdDocumentUnit = CurrentProtocol.Id.ToString()
        uscMulticlassificationRest.Visible = ProtocolEnv.MulticlassificationEnabled
        Dim showCheckBox As Boolean = checkPECSendMessages()

        uscDocumentUnitReferences.IdDocumentUnit = CurrentProtocol.Id.ToString()
        uscDocumentUnitReferences.DocumentUnitYear = CurrentProtocol.Year.ToString()
        uscDocumentUnitReferences.DocumentUnitNumber = CurrentProtocol.Number.ToString()
        uscDocumentUnitReferences.ShowArchiveRelationLinks = ProtocolEnv.UDSEnabled
        uscDocumentUnitReferences.ShowActiveWorkflowActivities = ProtocolEnv.WorkflowManagerEnabled AndAlso ProtocolEnv.WorkflowStateSummaryEnabled
        uscDocumentUnitReferences.ShowDoneWorkflowActivities = ProtocolEnv.WorkflowManagerEnabled AndAlso ProtocolEnv.WorkflowStateSummaryEnabled
        uscDocumentUnitReferences.ShowTNotice = ProtocolEnv.TNoticeEnabled
        uscDocumentUnitReferences.ShowPECIncoming = False
        uscDocumentUnitReferences.ShowPECOutgoing = False

        If ProtocolEnv.IsPECEnabled Then
            If CurrentProtocol.Type.ShortDescription.Eq("U") Then
                uscDocumentUnitReferences.ShowPECOutgoing = True
                uscDocumentUnitReferences.ShowPECIncoming = False
            End If
            If Not CurrentProtocol.Type.ShortDescription.Eq("U") Then
                uscDocumentUnitReferences.ShowPECOutgoing = True
                uscDocumentUnitReferences.ShowPECIncoming = True
            End If

        End If

    End Sub
    Private Function GetCurrentUDSRepository() As UDSRepository
        Dim repository As UDSRepository = CurrentUDSRepositoryFacade.GetById(CurrentRelatedUDS.Repository.UniqueId)
        If repository Is Nothing Then
            Throw New DocSuiteException(String.Format("Nessun repository configurato con ID {0}", CurrentRelatedUDS.Repository.UniqueId))
        End If

        Return repository
    End Function

    Private Function GetCurrentUDSSource(repository As UDSRepository) As UDSDto
        Dim udsSource As UDSDto = CurrentUDSFacade.GetUDSSource(repository, String.Format(ODATA_EQUAL_UDSID, CurrentRelatedUDS.IdUDS))
        If udsSource Is Nothing Then
            Throw New DocSuiteException(String.Format("Nessun archivio trovato con ID {0}", CurrentRelatedUDS.IdUDS))
        End If

        Return udsSource
    End Function

    Private Function GetCurrentSchemaRepositoryModel(repository As UDSRepository) As UDSModel
        Dim schemaRepositoryModel As UDSModel = UDSModel.LoadXml(repository.ModuleXML)

        Return schemaRepositoryModel
    End Function

    Private Sub InitializeUDSDynamicsControl()
        tblUds.SetDisplay(False)
        If CurrentRelatedUDS Is Nothing OrElse Not ViewUDSSource Then
            Exit Sub
        End If

        Dim udsRepository As UDSRepository = GetCurrentUDSRepository()
        Dim schemaRepositoryModel As UDSModel = GetCurrentSchemaRepositoryModel(udsRepository)
        Dim showArchiveProtocol As Boolean = schemaRepositoryModel.Model.ShowArchiveInProtocolSummaryEnabled

        tblUds.SetDisplay(showArchiveProtocol)

        If Not showArchiveProtocol Then
            Exit Sub
        End If

        InitializeUDSSource()
    End Sub

    ''' <summary> Caricamento dello stato per i controlli legacy </summary>
    ''' <remarks>
    ''' Fa schifo vero? 
    ''' Appalesamento dell'orrendo passato dell'user control. Qui vanno caricati tutti i controlli indipendenti dallo SHOW (or ora sono nell'aspx se non te ne sei accorto).
    ''' Far evolvere questo metodo SOLO a favore di <see cref="Initialize"/> che fa il binding unicamente in base al <see cref="CurrentProtocol"/> impostato dalla pagina chiamante, dopo aver tolto il caricamento da querystring.
    ''' </remarks>
    Private Sub RedundantLegacyLoader()
        ' pannello rigetto
        tblReject.Visible = False
        If CurrentProtocolRights.IsRejected Then
            tblReject.Visible = True
            lblReject.Text = If(String.IsNullOrEmpty(CurrentProtocol.LastChangedReason), "Nessun motivo esplicito.", CurrentProtocol.LastChangedReason)
            imgReject.ImageUrl = ImagePath.SmallReject
        End If

        ' pannello Tipologia spedizione
        tblTipoDocumento.Visible = False
        If ProtocolEnv.IsTableDocTypeEnabled AndAlso CurrentProtocol.DocumentType IsNot Nothing Then
            tblTipoDocumento.Visible = True
            TipoDocumentoDescription.Text = CurrentProtocol.DocumentType.Description
        End If
    End Sub

    ''' <summary> Caricamento programmatico </summary>
    ''' <remarks>
    ''' No, non pensarci a manutenere questa cosa, guarda <see cref="RedundantLegacyLoader" />
    ''' </remarks>
    Public Sub Show()
        If ProtocolEnv.ProtParzialeEnabled AndAlso CurrentProtocol.IdStatus = ProtocolStatusId.Incompleto Then
            lblTitle.Text = "Protocollo Incompleto"
        End If

        ' Carico INVOICE
        ' Nascosto di default
        tblInvoice.Visible = False
        ' Visualizza pannello invoices
        If ProtocolEnv.IsInvoiceEnabled AndAlso CurrentProtocol.AccountingNumber.HasValue AndAlso Not String.IsNullOrEmpty(CurrentProtocol.InvoiceNumber) Then
            tblInvoice.Visible = True
            LoadInvoice()
        End If

        ' Visualizza status del protocollo
        tblStatusProt.Visible = ProtocolEnv.IsStatusEnabled

        'scatoloni
        If VisibleScatolone Then
            VisibleScatolone = ProtocolEnv.IsPackageEnabled
        End If

        ' Dati protocollo ingresso
        tblSenderProt.Visible = tblSenderProt.Visible AndAlso (CurrentProtocol.Type.Id = -1)

        ' Contatti
        If Not _contactLoaded Then
            LoadContacts(True)
        End If

        If ProtocolEnv.IsClaimEnabled Then
            cbClaim.Visible = True
            If CurrentProtocol.IsClaim.HasValue Then
                cbClaim.Checked = CurrentProtocol.IsClaim.Value
            Else
                cbClaim.Checked = False
            End If
        End If

        'settori
        If ProtocolEnv.IsAuthorizFullEnabled Then
            LoadRoles()
        End If

        ' Fascicoli
        If ProtocolEnv.IsIssueEnabled Then
            tblFascicoli.Visible = True

            Dim contacts As List(Of ContactDTO) = Me.CurrentProtocol.ContactIssues _
               .Select(Function(c) FacadeFactory.Instance.ContactFacade.CreateDTO(c)) _
               .Where(Function(d) d IsNot Nothing) _
               .ToList()

            uscFascicoli.DataSource = contacts
            uscFascicoli.DataBind()
        Else
            tblFascicoli.Visible = False
        End If

        ' Visualizza riga assegnatario/proponente
        If CurrentProtocol.Type.Id = -1 Then
            rowAssegnatario.Visible = True
            rowProponente.Visible = False
        ElseIf CurrentProtocol.Type.Id = 1 Then
            rowAssegnatario.Visible = False
            rowProponente.Visible = True
        End If

        'Pubblicazione su internet
        lblPubblication.Visible = ProtocolEnv.IsPublicationEnabled
        lblCheckPublication.Visible = ProtocolEnv.IsPublicationEnabled
        lblCheckPublication.Text = If(CurrentProtocol.CheckPublication, "Pubblicato on-line", "")

        ' Motivo di annullamento
        If CurrentProtocol.IdStatus.HasValue AndAlso CurrentProtocol.IdStatus.Value = ProtocolStatusId.Annullato Then
            tblAnnullamento.Visible = True
        Else
            tblAnnullamento.Visible = False
        End If

        If VisiblePosteWeb Then
            LoadPosteOnLine()
        End If

        'Motivazione modifica oggetto
        trObjectChangeReason.Visible = ProtocolEnv.IsChangeObjectEnable And Not String.IsNullOrEmpty(CurrentProtocol.ObjectChangeReason)

        'Contenitore e Location
        If Not IsNothing(CurrentProtocol.Location) Then
            LocationName.Text = CurrentProtocol.Location.Name
        End If
        If Not IsNothing(CurrentProtocol.Container) Then
            ContainerName.Text = CurrentProtocol.Container.Name
        End If

        ' PARER
        If ProtocolEnv.ParerEnabled AndAlso VisibleParer Then
            LoadParer()
        Else
            VisibleParer = False
        End If

        If ProtocolEnv.ProtocolKindEnabled AndAlso ProtocolEnv.IsInvoiceEnabled AndAlso ProtocolEnv.InvoicePAEnabled AndAlso VisibleInvoicePA AndAlso CurrentProtocol.IdProtocolKind.Equals(ProtocolKind.FatturePA) Then
            LoadInvoicePA()
        Else
            VisibleInvoicePA = False
        End If

        'Altri
        If ProtocolEnv.DomainLookUpEnabled Then
            lblProtocolRegistrationUser.Text = String.Format("{0} {1:dd/MM/yyyy}", CommonAD.GetDisplayName(CurrentProtocol.RegistrationUser), CurrentProtocol.RegistrationDate.ToLocalTime)
            lblProtocolLastChangedUser.Text = CommonAD.GetDisplayName(CurrentProtocol.LastChangedUser)
        Else
            lblProtocolRegistrationUser.Text = String.Format("{0} {1:dd/MM/yyyy}", CurrentProtocol.RegistrationUser, CurrentProtocol.RegistrationDate.ToLocalTime)
            lblProtocolLastChangedUser.Text = CurrentProtocol.LastChangedUser
        End If

        If Not ProtocolEnv.DomainLookUpEnabled AndAlso CurrentProtocol.LastChangedDate.HasValue Then
            lblProtocolLastChangedUser.Text = String.Format("{0} {1:dd/MM/yyyy}", CurrentProtocol.LastChangedUser, CurrentProtocol.LastChangedDate.Value.ToLocalTime())
        ElseIf (CurrentProtocol.LastChangedDate.HasValue) Then
            lblProtocolLastChangedUser.Text = String.Format("{0} {1:dd/MM/yyyy}", CommonAD.GetDisplayName(CurrentProtocol.LastChangedUser), CurrentProtocol.LastChangedDate.Value.ToLocalTime())
        End If

        'Collegamento Protocolli
        If CurrentProtocol.ProtocolLinks IsNot Nothing AndAlso CurrentProtocol.ProtocolLinks.Count > 0 Then
            lblProtocolLink.Text = CurrentProtocol.ProtocolLinks.Count.ToString()
        End If

        If VisibleRefusedTreeView Then
            For Each rejectedRole As ProtocolRejectedRole In CurrentProtocol.RejectedRoles.Where(Function(r) r.Status = ProtocolRoleStatus.Refused)
                Dim node As New RadTreeNode()
                node.Text = String.Format("{0} - {1} ( {2} )", rejectedRole.Role.Name,
                                          String.Format(DocSuiteContext.Current.ProtocolEnv.ProtRegistrationDateFormat, rejectedRole.RegistrationDate.ToLocalTime.DateTime),
                                          rejectedRole.Note)
                node.ImageUrl = ImagePath.SmallSubRole
                TreeViewRefused.Nodes.Add(node)
            Next
        End If
    End Sub

    Private Function checkPECSendMessages() As Boolean
        Dim outgoingMails As IList(Of PECMail)

        outgoingMails = CurrentProtocol.OutgoingPecMailsProcessingError

        If outgoingMails.Count > 0 Then
            Return True
        End If
        Return False
    End Function

    Public Sub LoadPosteOnLine()
        VisiblePosteWeb = False
        If Not ProtocolEnv.IsPosteWebEnabled Then
            Exit Sub
        End If

        Dim lst As IList(Of POLRequestRecipientHeader) = Facade.PosteOnLineRequestFacade.GetRecipientByProtocol(CurrentProtocol.Id)
        If lst.Count <= 0 Then
            Exit Sub
        End If

        VisiblePosteWeb = True
        dgPosteRequestContact.DataSource = lst
        dgPosteRequestContact.DataBind()
    End Sub

    Public Sub LoadParer()
        ' Controllo se soggetto alla conservazione sostitutiva
        If Not Facade.ProtocolParerFacade.Exists(CurrentProtocol) Then
            parerInfo.Visible = False
            parerIcon.ImageUrl = "../Comm/images/parer/lightgray.png"
            parerLabel.Text = "Non soggetto alla conservazione anticipata."
            Exit Sub
        End If

        parerInfo.ImageUrl = "../Comm/images/info.png"
        parerInfo.OnClientClick = String.Format("return OpenParerDetail('{0}');", CurrentProtocol.Id)
        Dim status As ProtocolParerConservationStatus = Facade.ProtocolParerFacade.GetConservationStatus(CurrentProtocol)
        parerIcon.ImageUrl = uscProtGrid.GetParerStatusIcon(status)
        parerLabel.Text = ConservationsStatus(status)
    End Sub

    Public Sub LoadInvoicePA()
        Select Case CurrentProtocol.IdStatus
            Case ProtocolStatusId.Attivo
                imgInvoicePAStatus.ImageUrl = "../Comm/images/parer/lightgray.png"
                lblInvoicePAStatus.Text = "Fattura non ancora inviata a SDI."
            Case ProtocolStatusId.PAInvoiceSent
                imgInvoicePAStatus.ImageUrl = "../Comm/images/parer/yellow.png"
                lblInvoicePAStatus.Text = "Fattura inviata a SDI."
            Case ProtocolStatusId.PAInvoiceNotified
                imgInvoicePAStatus.ImageUrl = "../Comm/images/parer/yellow.png"
                lblInvoicePAStatus.Text = "Fattura inviata a SDI e notificata al destinatario"
            Case ProtocolStatusId.PAInvoiceAccepted
                imgInvoicePAStatus.ImageUrl = "../Comm/images/parer/green.png"
                lblInvoicePAStatus.Text = "Fattura accettata dal destinatario."
            Case ProtocolStatusId.PAInvoiceSdiRefused
                imgInvoicePAStatus.ImageUrl = "../Comm/images/parer/red.png"
                lblInvoicePAStatus.Text = "Fattura rifiutata da SDI."
            Case ProtocolStatusId.PAInvoiceRefused
                imgInvoicePAStatus.ImageUrl = "../Comm/images/parer/red.png"
                lblInvoicePAStatus.Text = "Fattura rifiutata dal destinatario"
            Case Else
                imgInvoicePAStatus.ImageUrl = "../Comm/images/parer/red.png"
                lblInvoicePAStatus.Text = "Status non riconosciuto."
        End Select
    End Sub

    Public Sub LoadContacts(ByVal checkMaxItems As Boolean)
        If tblMittentiDestinatari.Visible Then
            'abilita modifica contatti se flag attivo
            If ContactModifyEnable Then
                Select Case CurrentProtocol.Type.Id
                    Case -1
                        uscMittenti.IsRequired = True
                    Case 1
                        uscDestinatari.IsRequired = True
                End Select
                uscMittenti.ReadOnly = False
                uscMittenti.EnableCompression = False
                uscMittenti.MultiSelect = True
                uscMittenti.ButtonSelectVisible = True
                uscMittenti.ButtonSelectDomainVisible = DocSuiteContext.Current.ProtocolEnv.AbilitazioneRubricaDomain
                uscMittenti.ButtonSelectOChartVisible = True
                uscMittenti.ButtonDeleteVisible = True
                uscMittenti.ButtonManualVisible = True
                uscMittenti.ButtonPropertiesVisible = True
                uscMittenti.ButtonImportVisible = ProtocolEnv.IsImportContactEnabled
                uscMittenti.ButtonIPAVisible = ProtocolEnv.IsIPAAUSEnabled

                uscDestinatari.ReadOnly = False
                uscDestinatari.EnableCompression = False
                uscDestinatari.MultiSelect = True
                uscDestinatari.ButtonSelectVisible = True
                uscDestinatari.ButtonSelectDomainVisible = DocSuiteContext.Current.ProtocolEnv.AbilitazioneRubricaDomain
                uscDestinatari.ButtonSelectOChartVisible = True
                uscDestinatari.ButtonDeleteVisible = True
                uscDestinatari.ButtonManualVisible = True
                uscDestinatari.ButtonPropertiesVisible = True
                uscDestinatari.ButtonImportVisible = ProtocolEnv.IsImportContactEnabled
                uscDestinatari.ButtonIPAVisible = ProtocolEnv.IsIPAAUSEnabled
            Else
                If ContactMittenteModifyEnable Then
                    uscMittenti.IsRequired = True
                    uscMittenti.ReadOnly = False
                    uscMittenti.EnableCompression = False
                    uscMittenti.MultiSelect = False ' non supporta la multiselezione, eventualmente prevedere una proprietà ContactMittentiModifyEnable
                    uscMittenti.ButtonSelectVisible = True
                    uscMittenti.ButtonSelectDomainVisible = DocSuiteContext.Current.ProtocolEnv.AbilitazioneRubricaDomain
                    uscMittenti.ButtonSelectOChartVisible = True
                    uscMittenti.ButtonDeleteVisible = True
                    uscMittenti.ButtonManualVisible = True
                    uscMittenti.ButtonPropertiesVisible = True
                    uscMittenti.ButtonImportVisible = ProtocolEnv.IsImportContactEnabled
                    uscMittenti.ButtonIPAVisible = ProtocolEnv.IsIPAAUSEnabled
                End If

                If ContactDestinatariModifyEnable Then
                    uscDestinatari.IsRequired = True
                    uscDestinatari.ReadOnly = False
                    uscDestinatari.EnableCompression = False
                    uscDestinatari.MultiSelect = True
                    uscDestinatari.ButtonSelectVisible = True
                    uscDestinatari.ButtonSelectDomainVisible = DocSuiteContext.Current.ProtocolEnv.AbilitazioneRubricaDomain
                    uscDestinatari.ButtonSelectOChartVisible = True
                    uscDestinatari.ButtonDeleteVisible = True
                    uscDestinatari.ButtonManualVisible = True
                    uscDestinatari.ButtonPropertiesVisible = True
                    uscDestinatari.ButtonImportVisible = ProtocolEnv.IsImportContactEnabled
                    uscDestinatari.ButtonIPAVisible = ProtocolEnv.IsIPAAUSEnabled
                End If
            End If

            'recupera il numero di contatti da rubrica
            Dim countMittContacts As Integer = 0
            Dim countDestContacts As Integer = 0
            If (checkMaxItems) Then
                countMittContacts = Facade.ProtocolContactFacade.GetCountByProtocol(CurrentProtocol, "M")
                countMittContacts += Facade.ProtocolContactManualFacade.GetCountByProtocol(CurrentProtocol, "M")
                'recupera il numero di contatti manuali
                countDestContacts = Facade.ProtocolContactFacade.GetCountByProtocol(CurrentProtocol, "D")
                countDestContacts += Facade.ProtocolContactManualFacade.GetCountByProtocol(CurrentProtocol, "D")
            End If


            uscMittenti.DataSource = Facade.ProtocolFacade.GetSenders(CurrentProtocol)
            uscMittenti.DataBind(countMittContacts)

            uscDestinatari.DataSource = Facade.ProtocolFacade.GetRecipients(CurrentProtocol)
            uscDestinatari.DataBind(countDestContacts)
            'memorizza di aver già eseguito l'operazione
            _contactLoaded = True
        End If
    End Sub

    Private Sub LoadRoles()
        If CurrentProtocol.Roles.Count > 0 OrElse (DocSuiteContext.Current.SimplifiedPrivacyEnabled AndAlso CurrentProtocol.Users.Where(Function(u) u.Type = ProtocolUserType.Authorization).Count > 0) _
            OrElse (ProtocolEnv.ProtocolHighlightEnabled AndAlso CurrentProtocol.Users.Where(Function(u) u.Type = ProtocolUserType.Highlight).Count > 0) Then
            uscSettori.Visible = True
            uscSettori.Caption = "Settori con Autorizzazione"
            uscSettori.CurrentProtocol = CurrentProtocol
            uscSettori.DataBindProtocolRoles(CurrentProtocol.Roles, True, CurrentProtocol.Users.Where(Function(u) u.Type = ProtocolUserType.Authorization OrElse u.Type = ProtocolUserType.Highlight).ToList())
        Else
            uscSettori.Visible = False
        End If

        BindProtocolRoleUsers(uscSettori.RoleUserViewMode.RoleUsers)
    End Sub

    Public Sub RefreshProtocolRolesTree()
        uscSettori.TreeViewControl.Nodes().Clear()
        LoadRoles()
    End Sub

    Public Function GetProtocolStatusDescription() As String
        If CurrentProtocol.Status IsNot Nothing AndAlso CurrentProtocol.IdStatus.HasValue Then
            Return CurrentProtocol.Status.Description
        End If

        Return String.Empty
    End Function

    ''' <summary> Carica i dati di INVOICE, verifica se attivo e se deve essere visualizzato. </summary>
    Private Sub LoadInvoice()
        'Invoice Number
        If Not String.IsNullOrEmpty(CurrentProtocol.InvoiceNumber) Then
            trInvoiceNumber.Visible = True
            tdInvoiceNumber.InnerText = String.Format("{0} del {1:dd/MM/yyyy}", CurrentProtocol.InvoiceNumber, CurrentProtocol.InvoiceDate)
        Else
            trInvoiceNumber.Visible = False
        End If

        'AccountingSectional
        If Not CurrentProtocol.AccountingNumber.HasValue Then
            Exit Sub
        End If

        trAccountingSectional.Visible = True
        Dim txtAccounting As String = String.Format("{0}{1}/{2}",
                                      If(String.IsNullOrEmpty(CurrentProtocol.AccountingSectional), "", CurrentProtocol.AccountingSectional & " "),
                                      CurrentProtocol.AccountingYear.ToString(),
                                      CurrentProtocol.AccountingNumber.ToString())

        If CurrentProtocol.AccountingDate.HasValue Then
            txtAccounting &= " del " & CurrentProtocol.AccountingDate.DefaultString()
        End If
        tdAccountingSectional.InnerText = txtAccounting
    End Sub

    Protected Function ProtocolParerDetailUrl() As String
        Return ResolveUrl("~/PARER/ParerDetail.aspx")
    End Function

    Protected Function GetContenutoRequest(ByVal rq As POLRequestRecipientHeader) As String
        Select Case rq.RequestType
            Case POLRequestType.Lettera
                Return rq.LOLRequestDocumentName
            Case POLRequestType.Raccomandata
                Return rq.ROLRequestDocumentName
            Case POLRequestType.Telegramma
                Return rq.TOLRequestTesto
            Case POLRequestType.Serc
                Return rq.SOLRequestDocumentName
            Case Else
                Return String.Empty
        End Select
    End Function

    Private Sub BindProtocolRoleUsers(ByVal roleUserViewMode As uscSettori.RoleUserViewMode)
        If Not ProtocolEnv.IsDistributionEnabled OrElse (Not CurrentProtocolRights.IsDistributable AndAlso Not CurrentProtocolRights.IsDistributionAssigned) Then
            uscProtocolRoleUser.Visible = False
            Exit Sub
        End If

        uscProtocolRoleUser.Visible = True
        uscProtocolRoleUser.Caption = "Autorizzazioni Responsabile Settore"

        ' Creo un List(Of Role) per compatibilità con DataSource di uscSettori.
        Dim rolesToDisplay As New List(Of Role)
        If CurrentProtocol.Roles.Count > 0 Then
            For Each protocolRole As ProtocolRole In CurrentProtocol.Roles
                rolesToDisplay.Add(protocolRole.Role)
            Next
        End If
        Dim isManagerWithRights As Boolean = CurrentProtocolRights.IsDistributable AndAlso ProtocolEnv.ProtocolDistributionTypologies.Contains(CurrentProtocol.Type.Id) AndAlso rolesToDisplay.Any(Function(x) x.RoleGroups.Any(Function(x1) x1.ProtocolRights.IsRoleManager))

        uscProtocolRoleUser.CurrentProtocol = CurrentProtocol
        uscProtocolRoleUser.CurrentRoleUserViewMode = roleUserViewMode
        uscProtocolRoleUser.ProtocolRoleUsersOnly = True
        uscProtocolRoleUser.SourceRoles = rolesToDisplay
        uscProtocolRoleUser.ViewDistributableManager = CurrentProtocolRights.IsDistributionAssigned OrElse isManagerWithRights
        uscProtocolRoleUser.ToolBarVisible = isManagerWithRights
        uscProtocolRoleUser.DataBindForRoleUser(Nothing, Nothing, True)

    End Sub

    Public Shared Sub RepeaterExpander(button As ImageButton, repeater As Repeater)
        If repeater.Visible Then
            button.ImageUrl = ImagePath.SmallExpand
            button.ToolTip = "Espandi"
            repeater.Visible = False
        Else
            button.ImageUrl = ImagePath.SmallShrink
            button.ToolTip = "Comprimi"
            repeater.Visible = True
        End If
    End Sub

    Private Sub InitializeUDSSource()
        udsDynamicControls.IsReadOnly = True
        If CurrentRelatedUDS Is Nothing Then
            Exit Sub
        End If

        Dim currentUDSDocumentUnit As Entity.UDS.UDSDocumentUnit = CurrentRelatedUDS
        tblUds.SetDisplay(ViewUDSSource)
        btnViewUDS.Text = "Unità Documentaria"
        btnViewUDS.Icon.PrimaryIconUrl = ImagePath.SmallDocumentSeries
        btnViewUDS.Icon.PrimaryIconHeight = Unit.Pixel(16)
        btnViewUDS.Icon.PrimaryIconWidth = Unit.Pixel(16)
        btnViewUDS.NavigateUrl = String.Format("../UDS/UDSView.aspx?Type=UDS&IdUDS={0}&IdUDSRepository={1}", currentUDSDocumentUnit.IdUDS, currentUDSDocumentUnit.Repository.UniqueId)
    End Sub

    Private Sub InitializeSourceCollaboation()
        If Not DocSuiteContext.Current.ProtocolEnv.CollaborationSourceProtocolEnabled Then
            Me.VisibleSourceCollaboration = False
            Return
        End If

        Dim source As Collaboration = FacadeFactory.Instance.CollaborationFacade.GetByProtocol(Me.CurrentProtocol)
        If source Is Nothing Then
            Me.VisibleSourceCollaboration = False
            Return
        End If

        Me.VisibleSourceCollaboration = True
        Me.cmdSourceCollaboration.Text = String.Format("N. {0} del {1:dd/MM/yyyy}", source.Id, source.RegistrationDate.ToLocalTime())
        Dim queryString As String = "Type=Prot&Titolo=Inserimento&Action=Prt&idCollaboration={0}&Action2=CP&Title2=Protocollati/Gestiti"
        queryString = String.Format(queryString, source.Id)
        queryString = CommonShared.AppendSecurityCheck(queryString)
        Dim url As String = "../User/UserCollGestione.aspx?" & queryString
        Me.cmdSourceCollaboration.Icon.PrimaryIconUrl = "../App_Themes/DocSuite2008/imgset16/collaboration.png"
        Me.cmdSourceCollaboration.Icon.PrimaryIconHeight = Unit.Pixel(16)
        Me.cmdSourceCollaboration.Icon.PrimaryIconWidth = Unit.Pixel(16)
        Me.cmdSourceCollaboration.NavigateUrl = String.Format(url, source.Id)
    End Sub

#End Region

End Class
