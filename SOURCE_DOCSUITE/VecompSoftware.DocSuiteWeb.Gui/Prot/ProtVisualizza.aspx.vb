Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Entity.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.Entity.Workflows
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Interfaces
Imports VecompSoftware.DocSuiteWeb.Facade.Report
Imports VecompSoftware.DocSuiteWeb.Model.Workflow
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers.WebAPI
Imports VecompSoftware.Helpers.Workflow
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging

Public Class ProtVisualizza
    Inherits ProtBasePage
    Implements ISendMail

#Region " Fields "

    Private Const NoMailContactsSeed As String = "ProtVisualizza_NoMailContactsSeed"
    Private Const REFRESH_COMMAND As String = "Refresh"
    Private Const SENDNEWPEC_COMMAND As String = "SendNewPec"
    Private Const HIGHLIGHTUSER_WINDOW_COMMAND As String = "WindowCommandEvent"

    Private _isCyclable As Lazy(Of Boolean)
    Private _currentManageableRoleId As Integer?
    Private _uniqueIdList As List(Of Guid)
    Private _udsDocumentUnitFinder As UDSDocumentUnitFinder
    Private _currentRelatedUDS As Entity.UDS.UDSDocumentUnit

#End Region

#Region " Properties "

    Public Property VisibleDefaultButtonBar() As Boolean
        Get
            Return btnAutorizza.Visible
        End Get
        Set(ByVal value As Boolean)
            btnAutorizza.Visible = value
            btnMail.Visible = value
            btnMailSettori.Visible = value
            btnLink.Visible = value
            btnStampa.Visible = value
            btnDuplica.Visible = value
            btnPrintDocumentLabel.Visible = value
            btnPrintAttachmentLabel.Visible = value
            btnWorkflow.Visible = value
            btnToSeries.Visible = value
        End Set
    End Property

    Public Property VisibleExtraButtonBar() As Boolean
        Get
            Return btnPratica.Visible
        End Get
        Set(ByVal value As Boolean)
            btnPratica.Visible = value
            btnFascicle.Visible = value
            btnRiferimenti.Visible = value
            btnNoteSettore.Visible = value
            btnModifica.Visible = value
            btnAnnulla.Visible = value
            btnReject.Visible = value
            btnCorrection.Visible = value
            btnReassignRejected.Visible = value
            btnRispondiDaPEC.Visible = value AndAlso CurrentHasContainerRight
            btnInterop.Visible = value
            btnLog.Visible = value
            btnRolesLog.Visible = value
        End Set
    End Property

    Private ReadOnly Property GetPecMailUrl As String
        Get
            Return String.Format("../PEC/PECInsert.aspx?Type=Pec&SimpleMode={0}&UniqueIdProtocol={1}", DocSuiteContext.Current.ProtocolEnv.PECSimpleMode.ToString(), CurrentProtocol.Id)
        End Get
    End Property

    Public ReadOnly Property NewPecWindowWidth As String
        Get
            Return ProtocolEnv.PECWindowWidth.ToString()
        End Get
    End Property

    Public ReadOnly Property NewPecWindowHeight As String
        Get
            Return ProtocolEnv.PECWindowHeight.ToString()
        End Get
    End Property

    Public ReadOnly Property SenderDescription() As String Implements ISendMail.SenderDescription
        Get
            Return CommonInstance.UserDescription
        End Get
    End Property

    Public ReadOnly Property SenderEmail() As String Implements ISendMail.SenderEmail
        Get
            Return CommonInstance.UserMail
        End Get
    End Property

    Public ReadOnly Property Recipients() As IList(Of ContactDTO) Implements ISendMail.Recipients
        Get
            Return RoleFacade.GetValidContacts(CurrentProtocol.GetRoles())
        End Get
    End Property

    Public ReadOnly Property Documents() As IList(Of DocumentInfo) Implements ISendMail.Documents
        Get
            ' Nessun documento
            Return New List(Of DocumentInfo)()
        End Get
    End Property

    Public ReadOnly Property Subject() As String Implements ISendMail.Subject
        Get
            Return MailFacade.GetProtocolSubject(CurrentProtocol)
        End Get
    End Property

    Public ReadOnly Property Body() As String Implements ISendMail.Body
        Get
            Return MailFacade.GetProtocolBody(CurrentProtocol)
        End Get
    End Property

    Public ReadOnly Property IsCyclable As Boolean
        Get
            Return Me._isCyclable.Value
        End Get
    End Property

    Private ReadOnly Property CurrentManageableRoleId As Integer?
        Get
            If Not _currentManageableRoleId.HasValue AndAlso Not String.IsNullOrEmpty(CommonUtil.GroupProtocolManagerSelected) Then
                If CommonUtil.GroupProtocolManagerSelected IsNot Nothing AndAlso CommonUtil.GroupProtocolManagerSelected.Replace("|", "").Split(","c).Length = 1 Then
                    _currentManageableRoleId = CInt(CommonUtil.GroupProtocolManagerSelected.Replace("|", ""))
                End If
            End If
            Return _currentManageableRoleId
        End Get
    End Property

    Public ReadOnly Property IdDocumentChain() As Guid?
        Get
            If CurrentProtocol.IdDocument.GetValueOrDefault(0).Equals(0) OrElse CurrentDocumentUnitChains Is Nothing Then
                Return Nothing
            End If
            Dim documentUnitChain As Entity.DocumentUnits.DocumentUnitChain = CurrentDocumentUnitChains.SingleOrDefault(Function(f) f.ChainType = Entity.DocumentUnits.ChainType.MainChain)
            If documentUnitChain IsNot Nothing Then
                Return documentUnitChain.IdArchiveChain
            End If
            Return Nothing
        End Get

    End Property

    Public ReadOnly Property IdAttachmentsChain() As Guid?
        Get
            If CurrentProtocol.IdAttachments.GetValueOrDefault(0).Equals(0) OrElse CurrentDocumentUnitChains Is Nothing Then
                Return Nothing
            End If
            Dim documentUnitChain As Entity.DocumentUnits.DocumentUnitChain = CurrentDocumentUnitChains.SingleOrDefault(Function(f) f.ChainType = Entity.DocumentUnits.ChainType.AttachmentsChain)
            If documentUnitChain IsNot Nothing Then
                Return documentUnitChain.IdArchiveChain
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property DocumentUnitIds() As String
        Get
            If CurrentProtocol.Id.Equals(Guid.Empty) Then
                Return String.Empty
            End If
            _uniqueIdList = New List(Of Guid)
            _uniqueIdList.Add(CurrentProtocol.Id)
            Return JsonConvert.SerializeObject(_uniqueIdList)
        End Get
    End Property

    Private ReadOnly Property UDSDocumentUnitFinder As UDSDocumentUnitFinder
        Get
            If _udsDocumentUnitFinder Is Nothing Then
                _udsDocumentUnitFinder = New UDSDocumentUnitFinder(DocSuiteContext.Current.Tenants)
            End If
            Return _udsDocumentUnitFinder
        End Get
    End Property

    Public ReadOnly Property CurrentRelatedUDS As Entity.UDS.UDSDocumentUnit
        Get
            If _currentRelatedUDS Is Nothing Then
                Dim result As ICollection(Of WebAPIDto(Of Entity.UDS.UDSDocumentUnit)) = WebAPIImpersonatorFacade.ImpersonateFinder(UDSDocumentUnitFinder,
                    Function(impersonationType, finder)
                        finder.ResetDecoration()
                        finder.EnablePaging = False
                        finder.ExpandRepository = True
                        finder.IdDocumentUnit = CurrentProtocol.Id
                        Return finder.DoSearch()
                    End Function)

                If result Is Nothing Then
                    Return Nothing
                End If

                _currentRelatedUDS = result.Select(Function(s) s.Entity).SingleOrDefault()
            End If
            Return _currentRelatedUDS
        End Get
    End Property
#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        SetResponseNoCache()
        InitializeAjax()
        If Not IsPostBack Then
            If CurrentProtocol Is Nothing Then
                Throw New DocSuiteException($"Protocollo ID {CurrentProtocolId}", "Protocollo Inesistente")
            End If

            Dim sb As New StringBuilder
            With sb
                .Append("Titolo=Selezione Pratica")
                .AppendFormat("&NomeCampoID={0}", SelPratica.ClientID)
                .AppendFormat("&AddButton={0}", btnSelPratica.ClientID)
                .Append("&Type=LP")
                .AppendFormat("&Link={0}|", ProtocolFacade.ProtocolFullNumber(CurrentProtocol.Year, CurrentProtocol.Number, "|"))

            End With

            btnPratica.OnClientClick = String.Format("return {0}_OpenWindow('../Docm/DocmSelezione.aspx', 'windowDocmSceltaPratica', 600 ,400, '{1}');", ID, sb.ToString())

            If CurrentProtocolRights.IsPECAnswerable Then
                btnDuplica.Enabled = False ' disabilito il pulsante "Duplica".
                btnRispondiDaPEC.Enabled = True ' abilito il pulsante "Rispondi Da PEC".
                btnRispondiDaPEC.Style.Remove("display")
            Else
                btnDuplica.Enabled = True
                btnRispondiDaPEC.Enabled = False
                btnRispondiDaPEC.Style.Add("display", "none")
            End If

            sb = New StringBuilder
            With sb
                .AppendFormat("document.all('{0}').style.display = 'none';", tblButton.ClientID)
                .Append("var hgt = document.all('divContent').style.height;")
                .Append("var wdt = document.all('divContent').style.width;")
                .Append("document.all('divContent').style.height = '100%';")
                .Append("document.all('divContent').style.width = '100%';")
                .Append("window.print();")
                .AppendFormat("document.all('{0}').style.display = 'inline';", tblButton.ClientID)
                .Append("document.all('divContent').style.height = hgt;")
                .Append("document.all('divContent').style.width = wdt;")
                .Append("return false;")
            End With

            btnStampa.Attributes.Add("onclick", sb.ToString())

            If Action.Eq("MailSettori") Then
                ClientScript.RegisterStartupScript(Me.GetType(), "MailSettori", String.Format("<script language='javascript'>document.getElementById('{0}').click();</script>", btnMailSettori.ClientID))
            End If

            InitializeProtocolControl()
            InitializeHandler()
            Initialize()
            Facade.ProtocolLogFacade.Insert(CurrentProtocol, ProtocolLogEvent.PS, "")
        End If

        AddHandler MasterDocSuite.OnWorkflowConfirmed, AddressOf WorkflowConfirmed
        InitReportButtons()

        If Not String.IsNullOrEmpty(Request.QueryString("OpenDocument")) AndAlso Request.QueryString("OpenDocument").Eq("yes") Then
            Dim parameters As String = String.Format("{0}/viewers/ProtocolViewer.aspx?UniqueId={1}&Type=Prot", DocSuiteContext.Current.CurrentTenant.DSWUrl, CurrentProtocol.Id)
            Response.Redirect(parameters)
        End If
    End Sub

    Protected Sub ManagerAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim commandName As String = e.Argument

        Select Case commandName
            Case REFRESH_COMMAND
                uscProtocollo.Show()
            Case SENDNEWPEC_COMMAND
                Response.Redirect(GetPecMailUrl)
            Case HIGHLIGHTUSER_WINDOW_COMMAND
                RefreshProtocolRolesTree()
                btnRemoveHighlight.Visible = CurrentProtocolRights.HasHighlightRights
        End Select
    End Sub

    Private Sub UscProtocolloRefreshContact(ByVal sender As Object, ByVal e As EventArgs) Handles uscProtocollo.RefreshContact
        uscProtocollo.CurrentProtocol = CurrentProtocol
        uscProtocollo.LoadContacts(False)
    End Sub

    Private Sub BtnLinkClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnLink.Click
        Response.Redirect($"ProtCollegamenti.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={CurrentProtocol.Id}&Type=Prot")}")
    End Sub

    Private Sub BtnLogClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnLog.Click
        Response.Redirect($"~/Prot/ProtLog.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={CurrentProtocol.Id}&Type=Prot")}")
    End Sub

    Private Sub BtnRolesLogClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnRolesLog.Click
        Response.Redirect($"~/Prot/ProtRolesLog.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={CurrentProtocol.Id}&Type=Prot")}")
    End Sub

    Private Sub BtnModificaClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnModifica.Click
        Response.Redirect($"~/Prot/ProtModifica.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={CurrentProtocol.Id}&Type=Prot")}")
    End Sub

    Private Sub BtnReassignRejectedClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnReassignRejected.Click
        Response.Redirect($"~/Prot/ProtReassignment.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={CurrentProtocol.Id}&Type=Prot")}")
    End Sub

    Private Sub BtnCorrectionClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnCorrection.Click
        Response.Redirect($"~/Prot/ProtCorrection.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={CurrentProtocol.Id}&Type=Prot")}")
    End Sub

    Private Sub BtnInteropClick(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnInterop.Click
        Response.Redirect($"~/Prot/ProtInterop.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={CurrentProtocol.Id}&Type=Prot&im=1")}")
    End Sub

    Private Sub BtnSelPraticaClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnSelPratica.Click
        Dim s As String = SelPratica.Text
        If Left(s, 5) = "Docm:" Then
            s = Mid$(s, 6)
            Dim v As Array = Split(s, "/")
            s = "../Docm/DocmVisualizza.aspx?" & CommonShared.AppendSecurityCheck(String.Format("Year={0}&Number={1}&Type=Docm", v(0), v(1)))
            SelPratica.Text = ""
            Response.Redirect(s)
        End If
    End Sub

    Private Sub BtnNoteSettoreClick(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnNoteSettore.Click
        Response.Redirect($"~/Prot/ProtNoteGes.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={CurrentProtocol.Id}&Type=Prot")}")
    End Sub

    Private Sub BtnAnnullaClick(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnAnnulla.Click
        Response.Redirect($"~/Prot/ProtAnnulla.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={CurrentProtocol.Id}&Type=Prot")}")
    End Sub

    Private Sub BtnRejectOkClick(ByVal sender As System.Object, ByVal e As EventArgs) Handles cmdRejectOk.Click
        Facade.ProtocolFacade.Reject(CurrentProtocol, txtRejectMotivation.Text)
        InformationsMessage = New InformationsMessage() With {
            .InformationTitle = "Protocollo rigettato correttamente",
            .InformationMessage = String.Format("Il protocollo {0} è stato rigettato nel contenitore {1} con motivazione {2}.", CurrentProtocol.ToString(), Facade.ProtocolFacade.RejectionContainer.Name, txtRejectMotivation.Text),
            .RedirectUriAddress = New Uri(DocSuiteContext.Current.CurrentTenant.DSWUrl),
            .RedirectUriLatency = 15}
    End Sub

    Private Sub BtnFascicleClick(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnFascicle.Click
        Dim params As String = CommonShared.AppendSecurityCheck(String.Format("UniqueId={0}&CategoryId={1}&UDType={2}&FolderSelectionEnabled=True&Type=Fasc", CurrentProtocol.Id, CurrentProtocol.Category.Id, Convert.ToInt32(DSWEnvironment.Protocol)))
        Response.Redirect(String.Concat("~/Fasc/FascUDManager.aspx?", params))
    End Sub

    Private Sub BtnNuovoClick(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnCallbackDuplica.Click
        Response.Redirect($"~/Prot/ProtInserimento.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={CurrentProtocol.Id}&Type=Prot&Action=Duplicate&Check={txtCheckSel.Text}")}")
    End Sub

    Private Sub BtnRispondiDaPecClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnRispondiDaPEC.Click
        Response.Redirect($"~/Prot/ProtInserimento.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={CurrentProtocol.Id}&Type=Prot&Action=RispondiDaPEC&Check={txtCheckSel.Text}")}")
    End Sub

    Private Sub BtnRiferimentiClick(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnRiferimenti.Click
        Response.Redirect($"~/Prot/ProtRiferimento.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={CurrentProtocol.Id}&Type=Prot")}")
    End Sub

    Private Sub BtnAutorizzaClick(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnAutorizza.Click
        Response.Redirect($"~/Prot/ProtAutorizza.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={CurrentProtocol.Id}&Type=Prot&Action={Action}")}")
    End Sub

    Protected Sub BtnAddToPraticaClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddToPratica.Click
        Response.Redirect($"~/Docm/DocmRicerca.aspx?{CommonShared.AppendSecurityCheck($"ProtYear={CurrentProtocol.Year}&ProtNumber={CurrentProtocol.Number}&Type=Prot")}")
    End Sub

    Protected Sub BtnLetteraClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnLettera.Click
        Response.Redirect($"~/Prot/PosteWebItem.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={CurrentProtocol.Id}&Type=Prot&PType={Prot.PosteWeb.Item.TypeLettera}")}")
    End Sub

    Protected Sub BtnRaccomandataClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnRaccomandata.Click
        Response.Redirect($"~/Prot/PosteWebItem.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={CurrentProtocol.Id}&Type=Prot&PType={Prot.PosteWeb.Item.TypeRaccomandata}")}")
    End Sub

    Protected Sub BtnSercClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnTNotice.Click
        Response.Redirect($"~/Prot/PosteWebItem.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={CurrentProtocol.Id}&Type=Prot&PType={Prot.PosteWeb.Item.TypeTNotice}")}")
    End Sub

    Protected Sub BtnTelegrammaClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnTelegramma.Click
        Response.Redirect($"~/Prot/PosteWebItem.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={CurrentProtocol.Id}&Type=Prot&PType={Prot.PosteWeb.Item.TypeTelegramma}")}")
    End Sub

    Private Sub BtnForzaBiblosClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnForzaBiblos.Click
        Try
            Service.DetachDocument(CurrentProtocol.Location.ProtBiblosDSDB, CurrentProtocol.IdDocument.Value)
            AjaxAlert("Forzatura Biblos eseguita con successo")
        Catch ex As Exception
            Throw New DocSuiteException("Errore in fase Detach del Documento, impossibile eseguire l'annullamento del protocollo: " & ex.Message, ex)
        End Try
    End Sub
    Private Sub BtnAutoAssegnaClick(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnAutoAssign.Click
        If CurrentManageableRoleId.HasValue Then
            Dim protocolRoleGroup As ProtocolRole = Facade.ProtocolFacade.GetDistributionProtocolRole(CurrentProtocol)
            Dim groupName As String = Facade.ProtocolFacade.GetDistributionGroupName(CurrentProtocol)
            Dim adUser As AccountModel = CommonAD.GetAccount(DocSuiteContext.Current.User.FullUserName)
            Facade.ProtocolFacade.AddRoleUserAuthorization(CurrentProtocol, protocolRoleGroup.Role.Id, groupName, adUser.DisplayName, DocSuiteContext.Current.User.FullUserName)

            InitializeProtocolControl()
            InitializeHandler()
            InitializeProtocolBar()
            InitAutoAssegnoButton()
        Else
            AjaxAlert("E' necessario abilitare un solo settore da Utente>Settori abilitati.")
        End If
    End Sub

    ''' <summary> Prendi in carico il protocolo. </summary>
    Private Sub BtnHandleClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnHandle.Click
        Dim oldHandler As String = CurrentProtocol.Subject
        Try
            CurrentProtocol.Subject = DocSuiteContext.Current.User.FullUserName
            CurrentProtocol.HandlerDate = Date.Now
            Facade.ProtocolFacade.UpdateOnly(CurrentProtocol)

            If ProtocolEnv.ProtHandlerNotificationEnabled AndAlso Not String.IsNullOrEmpty(oldHandler) Then
                ' Preparo il subject della mail
                Dim subjectText As New StringBuilder(DocSuiteContext.Current.ProtocolEnv.ProtHandlerNotificationTemplate)
                subjectText.Replace("%USER%", CurrentProtocol.Subject)
                subjectText.Replace("%USEREMAIL%", CurrentProtocol.Subject)
                subjectText.Replace("%ID%", String.Format("{0}/{1}", CurrentProtocol.Year, CurrentProtocol.Number))
                subjectText.Replace("%SUBJECT%", CurrentProtocol.Subject)

                subjectText.Replace("%DATE%", CurrentProtocol.RegistrationDate.ToLocalTime().ToString())
                Facade.MessageFacade.NotifyToPreviousHandler(CurrentProtocol.Subject, oldHandler, subjectText.ToString(), ProtocolEnv.DispositionNotification)
                Facade.ProtocolLogFacade.Handle(CurrentProtocol, CurrentProtocol.Subject, True)


            End If
        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore nella presa in carico del protocollo: " & ex.Message, ex)
            AjaxAlert("Errore nella presa in carico del protocollo: " & ex.Message)
        End Try

        InitializeHandler()
    End Sub

    Private Sub BtnRicevutaClick(sender As Object, e As EventArgs)
        Dim btn As Button = DirectCast(sender, Button)
        Dim reportName As String = btn.CommandArgument

        Dim report As New ProtocolReport()
        report.AddRange(New List(Of Protocol)({CurrentProtocol}))

        Dim fullPath As String = If(Directory.Exists(ProtocolEnv.ReportLibraryPath),
                                    ProtocolEnv.ReportLibraryPath,
                                    Server.MapPath(ProtocolEnv.ReportLibraryPath))
        report.RdlcPrint = Path.Combine(fullPath, reportName)

        'TODO
        'Parametro richiesto dal Report --> ma cosa ci deve andare dentro??
        report.AddParameter("HeaderDescription", ProtocolEnv.ReportRicevutaHeaderDescription)
        report.AddParameter("ParerEnabled", ProtocolEnv.ParerEnabled.ToString)

        Dim doc As DocumentInfo = report.DoPrint(String.Empty)

        Dim file As FileInfo = BiblosFacade.SaveUniqueToTemp(doc)
        Dim temp As New TempFileDocumentInfo(file) With {.Caption = doc.Caption, .Name = doc.Caption}

        Dim url As String = ResolveUrl("~/Viewers/DocumentInfoViewer.aspx?" & CommonShared.AppendSecurityCheck(temp.ToQueryString().AsEncodedQueryString()))

        Facade.ProtocolLogFacade.Insert(CurrentProtocol, ProtocolLogEvent.PD, String.Format("Generata stampa ""{0}""", btn.Text))
        Response.Redirect(url, False)
    End Sub

    Private Sub btnInsertCollaborationConfirm_Click(sender As Object, e As EventArgs) Handles btnInsertCollaborationConfirm.Click
        Me.Response.Redirect(Me.rblInsertCollaboration.SelectedValue)
    End Sub

    Private Sub btnCycle_Click(sender As Object, e As EventArgs) Handles btnCycle.Click
        If Me.Session(UserScrivaniaD.MultiDistribuzioneSessionName) Is Nothing Then
            Return
        End If

        Dim cyclable As List(Of Guid) = CType(Me.Session(UserScrivaniaD.MultiDistribuzioneSessionName), List(Of Guid))
        cyclable.Remove(CurrentProtocol.Id)
        Me.Session(UserScrivaniaD.MultiDistribuzioneSessionName) = cyclable

        Dim nextKey As Guid = cyclable.First()
        Me.Response.Redirect($"~/Prot/ProtVisualizza.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={nextKey}&Type=Prot")}")
    End Sub

    Private Sub btnFlushAnnexed_Click(sender As Object, e As EventArgs) Handles btnFlushAnnexed.Click
        Facade.ProtocolFacade.FlushAnnexed(CurrentProtocol)
        btnFlushAnnexed.Visible = False
        MyBase.ReloadCurrentProtocolState()
        AjaxAlert("Catena annessi svuotata correttamente")
    End Sub

    Private Sub btnRemoveHighlight_Click(sender As Object, e As EventArgs) Handles btnRemoveHighlight.Click
        Facade.ProtocolUserFacade.RemoveHighlightUser(CurrentProtocol, DocSuiteContext.Current.User.FullUserName)
        btnRemoveHighlight.Visible = False
        RefreshProtocolRolesTree()
        AjaxManager.ResponseScripts.Add("HideLoadingPanel();")
    End Sub

    Private Sub RefreshProtocolRolesTree()
        uscProtocollo.CurrentProtocol = CurrentProtocol
        uscProtocollo.RefreshProtocolRolesTree()
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf ManagerAjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlMainContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRemoveHighlight, uscProtocollo, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnReject, RejectMotivation, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnHandle, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnHandle, pnlMainContent, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdRejectOk, txtRejectMotivation, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnNewPecMail, RejectMotivation, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnAutoAssign, pnlButtons, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnAutoAssign, pnlMainContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        If ProtocolEnv.EnableFlushAnnexed Then
            AjaxManager.AjaxSettings.AddAjaxSetting(btnFlushAnnexed, btnFlushAnnexed)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnFlushAnnexed, pnlMainContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        End If
    End Sub

    Private Sub InitializeProtocolControl()
        btnAddToPratica.Visible = ProtocolEnv.PraticheEnabled
        ' disabilito il pulsante "Aggiungi a Pratica" qualora il protocollo sia annullato.
        btnAddToPratica.Enabled = Not (CurrentProtocol.IdStatus.HasValue AndAlso CurrentProtocol.IdStatus.Value = ProtocolStatusId.Annullato)

        ' Visualizza protocollo
        uscProtocollo.CurrentProtocol = CurrentProtocol
        uscProtocollo.VisibleAltri = True
        uscProtocollo.VisibleClassificazione = True
        uscProtocollo.VisibleFascicolo = False
        uscProtocollo.VisibleMittentiDestinatari = True
        uscProtocollo.VisibleOggetto = True
        uscProtocollo.VisibleProtocollo = True
        uscProtocollo.VisibleSettori = True
        uscProtocollo.VisibleStatoProtocollo = True
        uscProtocollo.VisibleTipoDocumento = True
        ' Abilito il pannello assegnatario di protocollo enav solo per i protocollo in ingresso
        uscProtocollo.VisibleHandler = ProtocolEnv.ProtHandlerEnabled AndAlso CurrentProtocol.Type.Id = -1
        uscProtocollo.VisibleRefusedTreeView = ProtocolEnv.RefusedProtocolAuthorizationEnabled AndAlso
                                               CommonShared.HasRefusedProtocolGroupsRight AndAlso
                                               CurrentProtocol.RejectedRoles.Any(Function(r) r.Status = ProtocolRoleStatus.Refused)
        uscProtocollo.VisibleParer = ProtocolEnv.ParerEnabled
        uscProtocollo.VisibleProtocolloMittente = True
        uscProtocollo.VisibleInvoicePA = ProtocolEnv.ProtocolKindEnabled AndAlso ProtocolEnv.IsInvoiceEnabled AndAlso ProtocolEnv.InvoicePAEnabled AndAlso CurrentProtocol.IdProtocolKind.Equals(ProtocolKind.FatturePA)
        uscProtocollo.VisiblePosteWeb = ProtocolEnv.IsPosteWebEnabled
        uscProtocollo.VisibleScatolone = ProtocolEnv.IsPackageEnabled
        uscProtocollo.VisibleUDReferences = True
        uscProtocollo.Show()
    End Sub

    ''' <summary> Inizializza le funzioni per assegnatario. </summary>
    ''' <remarks> enav </remarks>
    Private Sub InitializeHandler()
        btnHandle.Visible = ProtocolEnv.ProtHandlerEnabled And Not CurrentProtocolRights.IsHighilightViewable
        btnHandle.Enabled = (String.IsNullOrEmpty(CurrentProtocol.Subject) OrElse Not DocSuiteContext.Current.User.FullUserName.Eq(CurrentProtocol.Subject)) AndAlso CurrentProtocol.Type.Id = -1
    End Sub

    ''' <summary> Pulsanti poste on line </summary>
    Private Sub InitPolButtons()
        If Not ProtocolEnv.IsLetteraEnabled AndAlso Not ProtocolEnv.IsRaccomandataEnabled AndAlso Not ProtocolEnv.IsTelgrammaEnabled AndAlso Not ProtocolEnv.TNoticeEnabled Then
            ' Se da ParameterEnv non ci sono pulsanti abilitabili evito tutte le verifiche
            Exit Sub
        End If

        ' Di default sono tutto nascosti
        btnLettera.Visible = False
        btnRaccomandata.Visible = False
        btnTelegramma.Visible = False
        btnTNotice.Visible = False

        ' Se c'è almeno un account disponibile ed il Protocollo è in Uscita
        Dim accounts As IList(Of POLAccount) = Facade.PosteOnLineAccountFacade.GetUserAccounts()
        If accounts.Count <= 0 OrElse Not CurrentProtocol.Type.ShortDescription.Eq("U") Then
            Exit Sub
        End If

        ' Se l'operatore ha diritti sul protocollo di PEC in Uscita
        If CurrentProtocolRights.IsPECSendable AndAlso Not CurrentProtocolRights.IsHighilightViewable Then
            btnLettera.Visible = ProtocolEnv.IsLetteraEnabled
            btnRaccomandata.Visible = ProtocolEnv.IsRaccomandataEnabled
            btnTelegramma.Visible = ProtocolEnv.IsTelgrammaEnabled
        End If

        btnTNotice.Visible = CurrentProtocolRights.IsTNoticeSendable
    End Sub
    Private Sub InitSendToUsers()
        btnSendToUsers.Visible = DocSuiteContext.Current.SimplifiedPrivacyEnabled
        If DocSuiteContext.Current.SimplifiedPrivacyEnabled Then
            btnSendToUsers.Visible = CurrentProtocolRights.IsAuthorizable AndAlso CurrentProtocol.Users.Any(Function(r) r.Type = ProtocolUserType.Authorization)
            Dim querystring As String = String.Format("UniqueId={0}&SendToUsers=True", CurrentProtocol.Id)
            btnSendToUsers.PostBackUrl = String.Concat("~/MailSenders/ProtocolMailSender.aspx?", CommonShared.AppendSecurityCheck(querystring))
        End If
    End Sub
    Private Sub InitNoteSettore()
        btnNoteSettore.Visible = True
        If Not btnNoteSettore.Visible Then
            Exit Sub
        End If
        ' diritti da controllare
        Dim rights As String = "1"
        If ProtocolEnv.IsDistributionEnabled Then
            rights = "11"
        End If

        If ProtocolEnv.RefusedProtocolAuthorizationEnabled Then
            btnNoteSettore.Text = "Accettazione e note"
        End If

        btnNoteSettore.Visible = Facade.ProtocolFacade.SecurityGroupsUserRole(CurrentProtocol, Facade.SecurityUsersFacade.GetGroupsByAccount(DocSuiteContext.Current.User.UserName), rights)
    End Sub

    Private Sub InitPecButtons()

        'Attivo la nuova gestione della PEC
        btnNewPecMail.Visible = CurrentProtocolRights.IsPECSendable AndAlso ProtocolEnv.ProtNewPECMailEnabled AndAlso (Not CurrentProtocol.IdStatus.HasValue OrElse CurrentProtocol.IdStatus.Value <> CType(ProtocolStatusId.Incompleto, Integer))

        If Not ProtocolEnv.ConsentiDuplicaProtDaPEC AndAlso CurrentProtocol.PecMails IsNot Nothing AndAlso CurrentProtocol.PecMails.Count > 0 Then
            btnDuplica.Enabled = False
        Else
            btnDuplica.Enabled = True
        End If
    End Sub

    ''' <summary> Pulsanti stampante Zebra </summary>
    Private Sub InitZebraButtons()
        If Not ProtocolEnv.ZebraPrinterEnabled OrElse Not CurrentProtocolRights.IsReadable OrElse (Facade.ComputerLogFacade.GetCurrent Is Nothing) OrElse (Facade.ComputerLogFacade.GetCurrent.ZebraPrinter Is Nothing) Then
            btnPrintDocumentLabel.Visible = False
            btnPrintAttachmentLabel.Visible = False
            Exit Sub
        End If

        CommonShared.ZebraPrintData = Nothing
        CommonShared.ZebraPrintData = New List(Of Guid) From {CurrentProtocol.Id}

        btnPrintDocumentLabel.Visible = CurrentProtocol.IdDocument.GetValueOrDefault(0) <> 0
        btnPrintDocumentLabel.OnClientClick =
            String.Format("return {0}_OpenWindow('../Prot/ProtZebraLabel.aspx','windowPrintLabel',350,150,'{1}');",
                          ID,
                          CommonShared.AppendSecurityCheck(String.Format("ChainType={0}", ProtZebraLabel.ChainType.Document)))

        btnPrintAttachmentLabel.Visible = CurrentProtocol.IdAttachments.GetValueOrDefault(0) <> 0
        btnPrintAttachmentLabel.OnClientClick =
            String.Format("return {0}_OpenWindow('../Prot/ProtZebraLabel.aspx','windowPrintLabel',350,150,'{1}');",
                          ID,
                          CommonShared.AppendSecurityCheck(String.Format("ChainType={0}", ProtZebraLabel.ChainType.Attachment)))
    End Sub

    Private Sub InitProtocolloAnnullato()
        VisibleDefaultButtonBar = False
        VisibleExtraButtonBar = False
        btnAutoAssign.Visible = False
        btnHighlight.Visible = False
        btnToSeries.Visible = False
        btnLog.Visible = CurrentProtocolRights.EnableViewLog
        btnSendToUsers.Visible = False
        btnAddToPratica.Visible = False
        btnRemoveHighlight.Visible = False

        If Not CommonUtil.HasGroupAdministratorRight() Then
            btnDocument.Visible = False
        End If

        If Not DocSuiteContext.Current.ProtocolEnv.IsInvoiceEnabled Then
            Exit Sub
        End If

        Dim contExtFacade As New ContainerExtensionFacade("ProtDB")
        Dim containerExtensions As IList(Of ContainerExtension) = contExtFacade.GetByContainerAndKey(CurrentProtocol.Container.Id, ContainerExtensionType.FT)
        If containerExtensions.Count > 0 AndAlso (CurrentProtocol.IdDocument.GetValueOrDefault(0) <> 0 AndAlso containerExtensions(0).KeyValue = "1") AndAlso Not CurrentProtocolRights.IsHighilightViewable Then
            btnForzaBiblos.Visible = True
        End If
    End Sub

    Private Sub InitFascicleButtons()
        btnFascicle.Visible = ProtocolEnv.FascicleEnabled AndAlso CurrentProtocolRightsStatusCancel.IsDocumentReadable
    End Sub

    Private Sub InitAuthorizeButton()
        btnAutorizza.Visible = CurrentProtocolRights.IsAuthorizable
        If ProtocolEnv.IsDistributionEnabled Then
            btnAutorizza.Visible = CurrentProtocolRights.IsDistributable
        End If
    End Sub

    Private Sub InitLogButton()
        btnLog.Visible = CurrentProtocolRights.EnableViewLog
        btnRolesLog.Visible = ProtocolEnv.RefusedProtocolAuthorizationEnabled
    End Sub

    Private Sub InitDocumentButtons()
        btnDocument.Enabled = CurrentProtocolRightsStatusCancel.IsDocumentReadable
        btnMail.Enabled = CurrentProtocolRightsStatusCancel.IsReadable
        btnMailSettori.Enabled = CurrentProtocolRightsStatusCancel.IsReadable

        If Not btnDocument.Enabled Then
            Exit Sub
        End If

        Dim querystring As String = String.Format("DataSourceType=prot&UniqueId={0}", CurrentProtocol.Id)
        btnDocument.PostBackUrl = String.Concat(ResolveUrl("~/viewers/ProtocolViewer.aspx?"), CommonShared.AppendSecurityCheck(querystring))
    End Sub

    Private Sub InitButtons()

        ' Pulsanti modifica
        btnModifica.Visible = CurrentProtocolRights.IsEditable OrElse CurrentProtocolRights.IsEditableAttachment.GetValueOrDefault(False)
        If ProtocolEnv.DocumentSeriesEnabled AndAlso DocSuiteContext.Current.ProtocolEnv.AmministrazioneTrasparenteProtocolEnabled Then
            If CurrentProtocolRightsStatusCancel.IsReadable OrElse CurrentProtocolRights.IsHighilightViewable AndAlso
                CommonShared.UserDocumentSeriesCheckRight(DocumentSeriesContainerRightPositions.Insert) Then
                btnToSeries.Visible = ProtocolEnv.ProtocolDocumentSeriesButtonEnable
                btnToSeries.Text = ProtocolEnv.ButtonSeriesTitle
                btnToSeries.PostBackUrl = ResolveUrl($"~/Prot/ProtToSeries.aspx?Type=Prot&UniqueId={CurrentProtocol.Id}")
            End If
        End If

        btnUDS.Visible = ProtocolEnv.UDSEnabled AndAlso CurrentRelatedUDS Is Nothing AndAlso CurrentProtocolRights.IsArchivable AndAlso CurrentUDSRepositoryFacade.HasProtocollableRepositories()
        btnUDS.PostBackUrl = ResolveUrl(String.Concat("~/UDS/ProtocolToUDS.aspx?Type=UDS&ProtocolUniqueId=", CurrentProtocol.Id))

        btnFlushAnnexed.Visible = False
        If ProtocolEnv.EnableFlushAnnexed Then
            btnFlushAnnexed.Visible = CurrentProtocolRights.IsFlushAnnexedEnable AndAlso Not CurrentProtocolRights.IsHighilightViewable
        End If
        btnReassignRejected.Visible = False
        If CurrentProtocolRights.IsRejected AndAlso CurrentProtocol.Subject = DocSuiteContext.Current.User.FullUserName Then
            btnReassignRejected.Visible = True
        End If

        ' Pulsanti dei documenti
        InitDocumentButtons()

        ' Pulsanti di log
        InitLogButton()

        ' Protocollo annullato
        If CurrentProtocol.IdStatus.HasValue AndAlso CurrentProtocol.IdStatus.Value = ProtocolStatusId.Annullato Then
            InitProtocolloAnnullato()
            Exit Sub
        End If

        btnMail.PostBackUrl = String.Format("~/MailSenders/ProtocolMailSender.aspx?recipients=false&UniqueId={0}&Type=Prot", CurrentProtocol.Id)
        btnMailSettori.PostBackUrl = String.Format("~/MailSenders/ProtocolMailSender.aspx?UniqueId={0}&SendToRoles=True&Type=Prot", CurrentProtocol.Id)

        ' Verifico se esiste almeno un Settore collegato al protocollo
        btnMailSettori.Visible = CurrentProtocol.Roles.Any(Function(prRole) Not prRole.Role Is Nothing) AndAlso Not CurrentProtocolRights.IsHighilightViewable

        InitFascicleButtons()
        InitZebraButtons()
        InitAuthorizeButton()
        InitAutoAssegnoButton()
        InitPecButtons()
        InitPolButtons()

        InitNoteSettore()
        InitSendToUsers()

        ' Button Interoperabilità
        btnInterop.Visible = CurrentProtocolRights.IsInteroperable AndAlso Not ProtocolEnv.ProtNewPECMailEnabled
        btnDuplica.Visible = CommonShared.UserProtocolCheckRight(ProtocolContainerRightPositions.Insert)

        btnWorkflow.Visible = ProtocolEnv.WorkflowManagerEnabled AndAlso CurrentProtocolRightsStatusCancel.IsDocumentReadable
        btnAnnulla.Visible = CurrentProtocolRights.IsCancelable
        btnReject.Visible = CurrentProtocolRights.IsEditable AndAlso CurrentProtocolRights.IsRejectable
        btnCorrection.Visible = CommonShared.HasGroupProtocolCorrectionRight

        ' Button riferimenti
        btnRiferimenti.Visible = False

        Dim rightAddressResult As Boolean = True
        ' Bottoni pec
        If ProtocolEnv.CheckEmptyPecAddressFromSummary Then
            Dim noMailContactIds As New StringBuilder()
            Dim noMailContacts As New StringBuilder()

            For Each contactDto As ContactDTO In uscProtocollo.ControlRecipients.GetContacts(False)
                If Not String.IsNullOrEmpty(contactDto.Contact.EmailAddress) OrElse Not String.IsNullOrEmpty(contactDto.Contact.CertifiedMail) Then
                    Continue For
                End If

                If contactDto.Type = 0 Then
                    noMailContactIds.Append(contactDto.IdManualContact.ToString())
                Else
                    noMailContactIds.Append(contactDto.Contact.Id.ToString())
                End If
                noMailContactIds.Append("$")

                noMailContacts.Append(Replace(contactDto.Contact.Description, "|", " ")).Append("$")
            Next

            If noMailContacts.Length > 0 Then
                ' rimuovo l'ultima occorrenza di "$"
                noMailContactIds = noMailContactIds.Remove(noMailContactIds.Length - 1, 1)
                noMailContacts = noMailContacts.Remove(noMailContacts.Length - 1, 1)

                Session(NoMailContactsSeed) = New KeyValuePair(Of String, String)(noMailContactIds.ToString, noMailContacts.Replace("'", String.Empty).ToString)
                btnNewPecMail.OnClientClick = String.Format("OpenPecMailAddressWindow('{0}','{1}'); return false;", NoMailContactsSeed, "Impossibile inviare il messaggio PEC, alcuni destinatari non possiedono un indirizzo e-mail.")
                rightAddressResult = False
            End If
        End If

        '' Se non ho valorizzato la proprietà "OnClientClick" significa che non ho motivo di aprire la finestra dei contatti e quindi apro la finestra standard
        If rightAddressResult Then
            btnNewPecMail.PostBackUrl = GetPecMailUrl
        End If

        Me.InitializeCollaborationSourceProtocol()
        If IsWorkflowOperation AndAlso IsCurrentWorkflowActivityManualComplete Then
            If CurrentWorkflowActivity IsNot Nothing AndAlso CurrentWorkflowActivity.Status <> WorkflowStatus.Done Then
                btnAddToPratica.Enabled = False
                btnAnnulla.Enabled = False
                btnAutorizza.Enabled = False
                btnCallbackDuplica.Enabled = False
                btnCorrection.Enabled = False
                btnCycle.Enabled = False
                btnDocument.Enabled = False
                btnDuplica.Enabled = False
                btnFascicle.Enabled = False
                btnFlushAnnexed.Enabled = False
                btnForzaBiblos.Enabled = False
                btnHandle.Enabled = False
                btnInsertCollaboration.Enabled = False
                btnInsertCollaborationConfirm.Enabled = False
                btnInterop.Enabled = False
                btnLettera.Enabled = False
                btnLink.Enabled = False
                btnLog.Enabled = False
                btnRolesLog.Enabled = False
                btnMail.Enabled = False
                btnMailSettori.Enabled = False
                btnModifica.Enabled = False
                btnNewPecMail.Enabled = False
                btnNoteSettore.Enabled = False
                btnPratica.Enabled = False
                btnPrintAttachmentLabel.Enabled = False
                btnPrintDocumentLabel.Enabled = False
                btnProtocollo.Enabled = False
                btnRaccomandata.Enabled = False
                btnReassignRejected.Enabled = False
                btnReject.Enabled = False
                btnRiferimenti.Enabled = False
                btnRispondiDaPEC.Enabled = False
                btnSelPratica.Enabled = False
                btnStampa.Enabled = False
                btnTelegramma.Enabled = False
                uscProtocollo.ButtonViewUDS.Enabled = False
                btnUDS.Enabled = False
                btnToSeries.Enabled = False
                btnHighlight.Enabled = False
                btnRemoveHighlight.Enabled = False
                btnWorkflow.Enabled = False
                btnTNotice.Enabled = False
                btnToSeries.Enabled = False
            End If
        End If

        btnHighlight.Visible = False
        btnRemoveHighlight.Visible = False
        If ProtocolEnv.ProtocolHighlightEnabled AndAlso CurrentProtocol.IdStatus.HasValue AndAlso (CurrentProtocol.IdStatus.Value >= 0 OrElse CurrentProtocol.IdStatus.Value = -20) Then
            If ProtocolEnv.IsDistributionEnabled Then
                btnHighlight.Visible = CurrentProtocolRights.IsPreviewable
            Else
                btnHighlight.Visible = CurrentProtocolRights.IsAuthorizable
            End If

            btnRemoveHighlight.Visible = CurrentProtocolRights.HasHighlightRights
        End If

        If CurrentProtocolRights.IsHighilightViewable Then
            VisibleDefaultButtonBar = False
            VisibleExtraButtonBar = False
        End If
    End Sub

    Private Sub InitReportButtons()
        If Not ProtocolEnv.ReportLibraryEnabled OrElse CurrentProtocolRights.IsHighilightViewable Then
            Exit Sub
        End If

        Dim fullPath As String = If(Directory.Exists(ProtocolEnv.ReportLibraryPath), ProtocolEnv.ReportLibraryPath, Server.MapPath(ProtocolEnv.ReportLibraryPath))

        ' Carico i pulsanti
        For Each report As String In Directory.GetFiles(fullPath, "REPORT_*.rdlc")
            Dim name As String = Path.GetFileNameWithoutExtension(report)
            Dim btn As New Button
            btn.OnClientClick = "ShowLoadingPanel();"
            btn.Text = name.Split("_".ToCharArray())(1)
            btn.CommandArgument = Path.GetFileName(report)
            btn.Width = New Unit(150, UnitType.Pixel)
            ReportButtons.Controls.Add(btn)
            ReportButtons.Controls.Add(New LiteralControl(WebHelper.Space))
            If btn.Text.Eq("Ricevuta") Then
                btn.Visible = CommonShared.UserProtocolCheckRight(ProtocolContainerRightPositions.Insert)
            End If
            AddHandler btn.Click, AddressOf BtnRicevutaClick
        Next

        'Se c'è almeno 1 bottone di ricevuta nascondo il bottone di stampa retrò
        If ReportButtons.Controls.Count > 0 Then btnStampa.Visible = False
    End Sub

    Private Sub InitializeWorkflowWizard()

        Dim prevStep As RadWizardStep = New RadWizardStep()
        Select Case Action
            Case "FromCollaboration"
                prevStep.ID = "InsertCollaborationProtocol"
                prevStep.Title = "Protocolla Collaborazione"
                prevStep.ToolTip = "Protocolla Collaborazione"
            Case "FromPEC"
                prevStep.ID = "InsertPEC"
                prevStep.Title = "Invia una nuova PEC"
                prevStep.ToolTip = "Invia una nuova PEC"
            Case "FromUDS"
                prevStep.ID = "InsertUDSProtocol"
                prevStep.Title = "Protocolla UDS"
                prevStep.ToolTip = "Protocolla UDS"
            Case Else
                prevStep.ID = "InsertProtocol"
                prevStep.Title = "Inserisci nuovo protocollo"
                prevStep.ToolTip = "Inserisci nuovo protocollo"
        End Select
        prevStep.Enabled = False
        MasterDocSuite.WorkflowWizardControl.WizardSteps.Add(prevStep)

        Dim sendCompleteStep As RadWizardStep = New RadWizardStep()
        sendCompleteStep.ID = "SendComplete"
        sendCompleteStep.Title = "Concludi attività"
        sendCompleteStep.ToolTip = "Concludi attività"
        sendCompleteStep.Active = True
        MasterDocSuite.CompleteWorkflowActivityButton.Enabled = False
        MasterDocSuite.WorkflowWizardControl.WizardSteps.Add(sendCompleteStep)
        If CurrentWorkflowActivity IsNot Nothing AndAlso (CurrentWorkflowActivity.Status = WorkflowStatus.Todo OrElse CurrentWorkflowActivity.Status = WorkflowStatus.Progress) Then
            MasterDocSuite.CompleteWorkflowActivityButton.Enabled = True
        End If

    End Sub

    Private Sub Initialize()
        If IsWorkflowOperation AndAlso IsCurrentWorkflowActivityManualComplete Then
            MasterDocSuite.WorkflowWizardRow.Visible = True
            MasterDocSuite.WizardActionColumn.Visible = True
            InitializeWorkflowWizard()
        End If
        ' Verifica autorizzazione di accesso al protocollo
        If (Not CurrentProtocolRightsStatusCancel.IsPreviewable AndAlso Not CurrentProtocolRights.IsHighilightViewable AndAlso Not CurrentProtocolRights.IsUserAuthorized) Then
            Facade.ProtocolLogFacade.Log(CurrentProtocol, ProtocolLogEvent.PE, String.Format("Protocollo n. {0} - Non autorizzato alla visualizzazione.", CurrentProtocol.FullNumber))
            Dim errorMessage As String
            If CurrentProtocol.IdStatus = ProtocolStatusId.Errato AndAlso CurrentProtocol.IdDocument = 0 Then
                errorMessage = "Il protocollo richiesto non è disponibile. Contattare il supporto IT."
            ElseIf ProtocolEnv.ProtocolRejectionEnabled AndAlso CurrentProtocol.IdStatus = ProtocolStatusId.Rejected AndAlso CurrentProtocol.Container.Id = ProtocolEnv.ProtocolRejectionContainerId Then
                errorMessage = String.Format("Non è possibile visualizzare il Protocollo richiesto in quanto rigettato nel contenitore [{0}].{1}Per visualizzare il protocollo è necessario possedere specifica autorizzazione.", Facade.ProtocolFacade.RejectionContainer.Name, WebHelper.Br)
            ElseIf CurrentProtocol.IdStatus = ProtocolStatusId.Incompleto OrElse CurrentProtocol.IdStatus = ProtocolStatusId.Sospeso Then
                Dim strEnum As String = If(CurrentProtocol.IdStatus = ProtocolStatusId.Incompleto, "Incompleto", "Sospeso")

                errorMessage = String.Concat("Non è possibile visualizzare il Protocollo richiesto se è nello stato di '", strEnum, "'. Verificare di aver inserito il documento principale durante la fase di 'recupero protocollo'.")
            Else
                errorMessage = "Non è possibile visualizzare il Protocollo richiesto. Verificare se si dispone di sufficienti autorizzazioni."
            End If
            Throw New DocSuiteException("Protocollo n. " & CurrentProtocol.FullNumber, errorMessage)
        End If

        Me._isCyclable = New Lazy(Of Boolean)(Function() Me.GetIsCyclable())
        Me.btnCycle.Visible = Me.IsCyclable

        MasterDocSuite.HistoryTitle = $"Protocollo - Visualizza {CurrentProtocol.FullNumber}"
        uscProtocollo.CurrentRelatedUDS = CurrentRelatedUDS
        Try
            InitializeProtocolBar()

        Catch ex As Exception
            Throw New DocSuiteException("Inizializzazione Protocollo", "Errore non previsto in inizializzazione del protocollo", ex)
        End Try

        InitButtons()
    End Sub

    Private Sub InitializeProtocolBar()
        Dim c As TableCell = tblIcons.Rows(0).Cells(0)
        ' Icona rigetto
        If CurrentProtocolRights.IsRejected Then
            AddIcon(c, ImagePath.BigReject, "Protocollo rigettato")
        End If
        ' Icona protocollo annullato
        If CurrentProtocol.IdStatus.HasValue AndAlso CurrentProtocol.IdStatus.Value = ProtocolStatusId.Annullato Then
            AddIcon(c, "../comm/images/remove32.gif", "Protocollo Annullato")
        End If
        ' Icona tipo protocollo
        Select Case CurrentProtocol.Type.Id
            Case -1
                AddIcon(c, "images/mail32_i.gif", "Protocollo in Ingresso")
                Title = "Protocollo in Ingresso - Visualizzazione"
            Case 1
                AddIcon(c, "images/mail32_u.gif", "Protocollo in Uscita")
                Title = "Protocollo in Uscita - Visualizzazione"
            Case 0
                AddIcon(c, "images/mail32_iu.gif", "Protocollo Tra Uffici")
                Title = "Protocollo Tra Uffici - Visualizzazione"
        End Select
        ' Icona tipo documento di base
        Dim tooltip As String = "Tipologia spedizione"
        Dim s As String
        If CurrentProtocol.IdStatus = ProtocolStatusId.Incompleto Then
            s = ImagePath.BigEmptyDocument
            tooltip = "Documento mancante"
        ElseIf String.IsNullOrWhiteSpace(CurrentProtocol.DocumentCode) OrElse CurrentProtocol.IdDocument.GetValueOrDefault(0) = 0 Then
            s = ImagePath.BigPageError
            tooltip = "Documento non inserito"
        Else
            s = ImagePath.FromFileNoSignature(CurrentProtocol.DocumentCode, False)
        End If
        AddIcon(c, s, tooltip)

        ' Icona di firma
        If FileHelper.MatchExtension(CurrentProtocol.DocumentCode, FileHelper.P7M) OrElse
               FileHelper.MatchExtension(CurrentProtocol.DocumentCode, FileHelper.P7X) OrElse
               FileHelper.MatchExtension(CurrentProtocol.DocumentCode, FileHelper.M7M) Then
            AddIcon(c, ImagePath.BigSigned, "Documento con Firma Digitale Qualificata")
        End If

        ' Icona allegati
        If CurrentProtocol.IdAttachments.GetValueOrDefault(0) <> 0 Then
            AddIcon(c, "../Comm/images/file/Allegati32.gif", "Protocollo con allegati")
        End If
        ' Icona collegamenti
        If CurrentProtocol.ProtocolLinks.Count > 0 Then
            AddIcon(c, "../Comm/Images/DocSuite/Link32.png", "Protocollo con collegamenti")
        End If
        ' Icona fascicoli ProtocolEnv.IsFascicleEnabled
        If DocSuiteContext.Current.ProtocolEnv.FascicleEnabled Then
            If CurrentFascicleDocumentUnits.Any(Function(x) x.ReferenceType = ReferenceType.Fascicle) Then
                Dim close As Boolean = CurrentFascicleDocumentUnits.First(Function(x) x.ReferenceType = ReferenceType.Fascicle).Fascicle.EndDate.HasValue
                AddIcon(c, If(close, "~/App_Themes/DocSuite2008/imgset32/fascicle_close.png", "~/App_Themes/DocSuite2008/imgset32/fascicle_open.png"), "Protocollo fascicolato")
            End If
            If CurrentFascicleDocumentUnits.Any(Function(x) x.ReferenceType = ReferenceType.Reference) Then
                AddIcon(c, "~/App_Themes/DocSuite2008/imgset32/fascicle_link.png", "Protocollo collegato a dei fascicoli")

            End If
        Else
            btnFascicle.Visible = False
        End If

        btnAddToPratica.Visible = ProtocolEnv.PraticheEnabled AndAlso Not CurrentProtocolRights.IsHighilightViewable
        If DocSuiteContext.Current.IsDocumentEnabled Then
            Select Case Facade.ProtocolFacade.GetLinkedDocumentCount(CurrentProtocol.Year, CurrentProtocol.Number)
                Case 0
                    btnPratica.Visible = False
                Case 1
                    btnPratica.Visible = True
                    AddIcon(c, "../Comm/images/docsuite/pratica32.gif", "Protocollo in pratica")
                Case Is > 1
                    btnPratica.Visible = True
                    AddIcon(c, "../Comm/images/docsuite/pratiche32.gif", "Protocollo in pratiche")
                Case Else
                    btnPratica.Visible = False
            End Select
        End If

        If ProtocolEnv.IsDistributionEnabled Then
            If CurrentProtocolRights.IsDistributionAssignedToMe Then
                'Operatore effettivo vince su manager
                If CurrentProtocolRights.IsCurrentUserDistributionCc() Then
                    AddIcon(c, "../Comm/images/users/yellow.gif", "Operatore in CC")
                Else
                    AddIcon(c, "../Comm/images/users/red.gif", "Operatore competente")
                End If
            Else
                If CurrentProtocolRights.IsCurrentUserDistributionManager() Then
                    ' Manager
                    If CurrentProtocolRights.IsCurrentUserDistributionCc() Then
                        AddIcon(c, "../Comm/images/users/cyan.gif", "Manager in CC")
                    Else
                        AddIcon(c, "../Comm/images/users/blue.gif", "Manager")
                    End If
                Else
                    If CurrentProtocolRights.IsCurrentUserDistributionCc() Then
                        AddIcon(c, "../Comm/images/users/yellow.gif", "Operatore in CC")
                    Else
                        AddIcon(c, "../Comm/images/users/red.gif", "Operatore competente")
                    End If
                End If
            End If
        End If

        ' Aggiungo le icone delle coccarde relative alle sole PEC valide
        If ProtocolEnv.IsPECEnabled Then
            Dim kvp As KeyValuePair(Of String, String) = CoccardaManager.GetImage(CurrentProtocol, ProtocolEnv.CoccardaProtocolEnabled)
            If Not String.IsNullOrEmpty(kvp.Key) Then
                AddIcon(c, kvp.Key, kvp.Value)
            End If
        End If

        If ProtocolEnv.ProtocolHighlightEnabled AndAlso CurrentProtocol.Users.Any(Function(x) x.Account.Eq(DocSuiteContext.Current.User.FullUserName) AndAlso x.Type = ProtocolUserType.Highlight) Then
            AddIcon(c, "~/App_Themes/DocSuite2008/imgset32/watch.png", "Protocollo in evidenza")
        End If
    End Sub

    Private Sub AddIcon(ByVal cell As TableCell, ByVal imageUrl As String, ByVal toolTip As String)
        Dim a As New Image
        a.ImageUrl = imageUrl
        a.ToolTip = toolTip
        a.Style("margin-bottom") = "5px"
        cell.Controls.Add(a)
        cell.Controls.Add(WebHelper.BrControl)
    End Sub

    Private Sub InitializeCollaborationSourceProtocol()
        If Not DocSuiteContext.Current.ProtocolEnv.CollaborationSourceProtocolEnabled Then
            Return
        End If

        If Not Me.CurrentProtocol.Type.Id.Equals(-1) Then
            ' Questa funzionalità è disponibile solo per i protocolli in ingresso.
            Return
        End If

        If Not (CollaborationRights.GetInserimentoAllaVisioneFirmaEnabled() OrElse CollaborationRights.GetInserimentoAlProtocolloSegreteriaEnabled()) Then
            Return
        End If

        Dim queryString As String = "Type=Prot&Titolo=Inserimento&Document=P&Title2={0}&Action={1}&Action2={2}&SourceUniqueIdProtocol={3}"
        Dim postBackUrl As String = "../User/UserCollGestione.aspx?"
        If CollaborationRights.GetInserimentoAllaVisioneFirmaEnabled() Then
            Dim formatted As String = String.Format(queryString, "Alla Visione/Firma", "Add", "CI", Me.CurrentProtocol.Id)
            Dim allaVisioneFirmaUrl As String = postBackUrl & CommonShared.AppendSecurityCheck(formatted)
            Me.rblInsertCollaboration.Items.Add(New ListItem("Inserimento Alla Visione/Firma", allaVisioneFirmaUrl))
        End If

        If CollaborationRights.GetInserimentoAlProtocolloSegreteriaEnabled() Then
            Dim formatted As String = String.Format(queryString, "Al Protocollo/Segreteria", "Apt", "CA", Me.CurrentProtocol.Id)
            Dim alProtocolloSegreteriaUrl As String = postBackUrl & CommonShared.AppendSecurityCheck(formatted)
            Me.rblInsertCollaboration.Items.Add(New ListItem("Inserimento Al Protocollo/Segreteria", alProtocolloSegreteriaUrl))
        End If

        If Me.rblInsertCollaboration.Items Is Nothing _
            OrElse Not Me.rblInsertCollaboration.Items.Count > 0 Then
            Return
        End If

        Me.btnInsertCollaboration.Visible = Not CurrentProtocolRights.IsHighilightViewable

        If String.IsNullOrEmpty(Me.rblInsertCollaboration.SelectedValue) Then
            Me.rblInsertCollaboration.Items.Cast(Of ListItem).Last().Selected = True
        End If

        If Me.rblInsertCollaboration.Items.Count.Equals(1) Then
            ' Se ho disponibile una sola modalità vado direttamente a quella tipologia di inserimento.
            Me.btnInsertCollaboration.PostBackUrl = Me.rblInsertCollaboration.SelectedValue
            Return
        End If

        ' Altrimenti permetto all'utente di scegliere.
        Me.btnInsertCollaboration.OnClientClick = String.Format("return OpenWindow(""{0}"");", Me.wndInsertCollaboration.ClientID)
    End Sub

    Private Function GetIsCyclable() As Boolean
        Dim cyclable As List(Of Guid) = CType(Me.Session(UserScrivaniaD.MultiDistribuzioneSessionName), List(Of Guid))
        If cyclable.IsNullOrEmpty() Then
            ' Non sto ciclando nessun protocollo.
            Return False
        End If

        If Not cyclable.Any(Function(k) Me.CurrentProtocol.Id.Equals(k)) Then
            ' Il protocollo che sto visualizzando non è stato selezionato nell'elenco di quelli che intendo ciclare.
            Return False
        End If

        If cyclable.Count = 1 Then
            ' Non ho nessun protocollo successivo.
            Return False
        End If

        Return True
    End Function

    Protected Sub WorkflowConfirmed(sender As Object, e As EventArgs)
        If CurrentWorkflowActivity IsNot Nothing AndAlso (CurrentWorkflowActivity.Status = WorkflowStatus.Todo OrElse CurrentWorkflowActivity.Status = WorkflowStatus.Progress) Then
            Dim workflowNotify As WorkflowNotify = New WorkflowNotify(CurrentWorkflowActivity.UniqueId) With {
                .WorkflowName = CurrentWorkflowActivity?.WorkflowInstance?.WorkflowRepository?.Name
            }
            workflowNotify.OutputArguments.Add(WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_NUMBER, New WorkflowArgument() With {
                                      .Name = WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_NUMBER,
                                      .PropertyType = ArgumentType.PropertyInt,
                                      .ValueInt = CurrentProtocol.Number})
            workflowNotify.OutputArguments.Add(WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_YEAR, New WorkflowArgument() With {
                                      .Name = WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_YEAR,
                                      .PropertyType = ArgumentType.PropertyInt,
                                      .ValueInt = CurrentProtocol.Year})
            workflowNotify.OutputArguments.Add(WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_UNIQUEID, New WorkflowArgument() With {
                                      .Name = WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_UNIQUEID,
                                      .PropertyType = ArgumentType.PropertyGuid,
                                      .ValueGuid = CurrentProtocol.Id})
            workflowNotify.OutputArguments.Add(WorkflowPropertyHelper.DSW_ACTION_ACTIVITY_MANUAL_COMPLETE, New WorkflowArgument() With {
                                      .Name = WorkflowPropertyHelper.DSW_ACTION_ACTIVITY_MANUAL_COMPLETE,
                                      .PropertyType = ArgumentType.PropertyBoolean,
                                      .ValueBoolean = True})
            If CurrentDocumentUnitChains IsNot Nothing AndAlso CurrentDocumentUnitChains.Any() Then
                workflowNotify.OutputArguments.Add(WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_CHAINID_MAIN, New WorkflowArgument() With {
                                      .Name = WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_CHAINID_MAIN,
                                      .PropertyType = ArgumentType.PropertyGuid,
                                      .ValueGuid = CurrentDocumentUnitChains.Single(Function(f) f.ChainType = Entity.DocumentUnits.ChainType.MainChain).IdArchiveChain})
            End If
            Dim webApiHelper As WebAPIHelper = New WebAPIHelper()
            If Not WebAPIImpersonatorFacade.ImpersonateSendRequest(webApiHelper, workflowNotify, DocSuiteContext.Current.CurrentTenant.WebApiClientConfig, DocSuiteContext.Current.CurrentTenant.OriginalConfiguration) Then
                FileLogger.Warn(LoggerName, "ProtocolWorkflowConfirmed is not correctly evaluated from WebAPI. See specific error in WebAPI logger")
            End If
        End If
        Response.Redirect("../User/UserWorkflow.aspx?Type=Comm")
    End Sub

    Private Sub InitAutoAssegnoButton()
        btnAutoAssign.Visible = False
        btnAutoAssign.Visible = ProtocolEnv.IsDistributionEnabled AndAlso CurrentProtocolRights.IsCurrentUserDistributionManager.Value AndAlso Not CurrentProtocolRights.IsCurrentUserDistributionCc.Value AndAlso Not CurrentProtocolRights.IsDistributionAssigned AndAlso Not CurrentProtocolRights.IsHighilightViewable
    End Sub
#End Region

End Class