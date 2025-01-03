﻿Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Text.RegularExpressions
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Conservations
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.DTO.WorkflowsElsa
Imports VecompSoftware.DocSuiteWeb.Entity.Conservations
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade.PEC
Imports VecompSoftware.DocSuiteWeb.Gui.Viewers.Extensions
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Commons
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Facade.WebAPI

Public Class PECSummary
    Inherits PECBasePage
    Implements ISendMail
    Implements IHavePecMail

#Region " Fields "
    Private _currentConservation As Conservation
#End Region

#Region " Properties "

    Public ReadOnly Property PecMails() As IEnumerable(Of PECMail) Implements IHavePecMail.PecMails
        Get
            Return {CurrentPecMail}
        End Get
    End Property

    Private ReadOnly Property IsReadOnly() As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean)("readonly", False)
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
            Return New List(Of ContactDTO)()
        End Get
    End Property

    Public ReadOnly Property Documents As IList(Of DocumentInfo) Implements ISendMail.Documents
        Get
            Return Facade.PECMailFacade.GetDocumentList(CurrentPecMailWrapper)
        End Get
    End Property

    Public ReadOnly Property Subject() As String Implements ISendMail.Subject
        Get
            Return String.Empty
        End Get
    End Property

    Public ReadOnly Property Body() As String Implements ISendMail.Body
        Get
            Return String.Empty
        End Get
    End Property

    Public ReadOnly Property CurrentConservation As Conservation
        Get
            If _currentConservation Is Nothing Then
                Dim conservation As Conservation = WebAPIImpersonatorFacade.ImpersonateFinder(New ConservationFinder(DocSuiteContext.Current.CurrentTenant),
                    Function(impersonationType, finder)
                        finder.UniqueId = CurrentPecMail.UniqueId
                        finder.EnablePaging = False
                        Return finder.DoSearch().Select(Function(x) x.Entity).FirstOrDefault()
                    End Function)
                _currentConservation = conservation
            End If
            Return _currentConservation
        End Get
    End Property


#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        InitIcons()
        LoadConservation()

        If Not IsPostBack AndAlso Not Page.IsCallback Then
            SetResponseNoCache()
            If CurrentPecMail.ProcessStatus = PECMailProcessStatus.StoredInDocumentManager AndAlso Not String.IsNullOrEmpty(CurrentPecMail.MailContent) Then
                Dim results As String = New FacadeElsaWebAPI(ProtocolEnv.DocSuiteNextElsaBaseURL).StartPreparePECMailDocumentsWorkflow(CurrentPecMail.UniqueId, JsonConvert.DeserializeObject(Of List(Of DocumentInfoModel))(CurrentPecMail.MailContent))
                If CurrentPecMail.Receipts IsNot Nothing AndAlso CurrentPecMail.Receipts.Count > 0 Then
                    For Each pecMailReceipt As PECMailReceipt In CurrentPecMail.Receipts.Where(Function(x) x.Parent.ProcessStatus = PECMailProcessStatus.StoredInDocumentManager AndAlso Not String.IsNullOrWhiteSpace(x.Parent.MailContent))
                        results = New FacadeElsaWebAPI(ProtocolEnv.DocSuiteNextElsaBaseURL).StartPreparePECMailDocumentsWorkflow(pecMailReceipt.Parent.UniqueId, JsonConvert.DeserializeObject(Of List(Of DocumentInfoModel))(pecMailReceipt.Parent.MailContent))
                    Next
                End If
            End If

            If Not String.IsNullOrEmpty(CurrentPecMail.Segnatura) Then
                uscInteropInfo.Visible = True
                uscInteropInfo.Source = CurrentPecMail.Segnatura
            End If
            If CurrentPecMail.MailBox.PecHandleEnabled() AndAlso
                CommonShared.UserConnectedBelongsTo(ProtocolEnv.GroupPecAutoHandle, True) AndAlso
                CurrentPecMail.Direction = PECMailDirection.Ingoing AndAlso
                String.IsNullOrEmpty(CurrentPecMail.Handler) AndAlso
                CurrentPecMailRights.CanHandle AndAlso Not PecUnhandled Then
                ' Prendi in carico la PEC

                CurrentPecMail.Handler = DocSuiteContext.Current.User.FullUserName
                If Not String.IsNullOrEmpty(Request.QueryString("setHandler")) Then
                    If Request.QueryString("setHandler").Equals("True") Then
                        CurrentPecMail.Handler = String.Empty
                    End If
                End If

                CleanCurrentPecMailRights()

                Facade.PECMailFacade.Update(CurrentPecMail)
                '' Aggiungo il Log della presa in carico automatica
                Facade.PECMailLogFacade.Handle(CurrentPecMail, CurrentPecMail.Handler, True)
            End If
            '' Gestione del download dell'eml originale - Se l'utente è amministratore o è presente nel gruppo
            If CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupOriginalEmlRight Then
                If CurrentPecMail.IDMailContent <> Guid.Empty Then
                    If CurrentPecMail.ProcessStatus <> PECMailProcessStatus.StoredInDocumentManager AndAlso Not New BiblosChainInfo(CurrentPecMail.IDMailContent).HasActiveDocuments() Then
                        btnOriginalEml.Visible = True
                        btnOriginalEml.Enabled = False
                        btnOriginalEml.Text = String.Format("Scarica {0} originale", PecLabel)
                        btnOriginalEml.ToolTip = "La PEC originale è stata rimossa in quanto storica e non più di interesse."
                    Else
                        Dim original As DocumentInfo = Facade.PECMailFacade.GetOriginalEml(CurrentPecMail, New Action(Sub() DelegateElsaInitialize(CurrentPecMail)))
                        If original IsNot Nothing Then
                            btnOriginalEml.Text = String.Format("Scarica {0} originale", PecLabel)
                            btnOriginalEml.Visible = True
                            btnOriginalEml.Enabled = True
                        End If
                    End If
                End If

                If cmdPECView.Visible AndAlso CurrentPecMail.IDAttachments <> Guid.Empty Then
                    If CurrentPecMail.ProcessStatus = PECMailProcessStatus.StoredInDocumentManager OrElse
                        (CurrentPecMail.ProcessStatus <> PECMailProcessStatus.StoredInDocumentManager AndAlso New BiblosChainInfo(CurrentPecMail.IDAttachments).HasActiveDocuments()) Then
                        cmdPECView.Enabled = True
                    Else
                        cmdPECView.Enabled = False
                        cmdPECView.ToolTip = "La PEC è stata gestita/protocollata ed è possibile consultare i documenti tramite il link al protocollo."
                    End If
                End If
            End If
            InitializeButtons()

            Title = String.Format("{0} - Sommario", PecLabel)
            Facade.PECMailLogFacade.Read(CurrentPecMail)

            InitializePec()
        End If
    End Sub

    Protected Sub PECSummary_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)

        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch
            Exit Sub
        End Try

        If ajaxModel IsNot Nothing Then
            Select Case ajaxModel.ActionName
                Case "FisicalRemovePEC"
                    FisicalRemovePEC()
            End Select
        End If
    End Sub

    Private Sub handlers_Click(sender As Object, e As EventArgs) Handles btnHandle.Click, btnUnhandle.Click
        Dim btn As Button = CType(sender, Button)
        Select Case btn.CommandArgument
            Case "True"
                Dim oldHandler As String = CurrentPecMail.Handler
                Try
                    CurrentPecMail.Handler = DocSuiteContext.Current.User.FullUserName
                    Facade.PECMailFacade.Update(CurrentPecMail)
                    Facade.PECMailLogFacade.Handle(CurrentPecMail, CurrentPecMail.Handler, True)

                    ' Se la mail era precedentemente gestita da qualcuno gli invio una mail di notifica.
                    If ProtocolEnv.PECHandlerNotificationEnabled AndAlso Not String.IsNullOrEmpty(oldHandler) Then
                        Dim mailSubject As New StringBuilder(ProtocolEnv.PECHandlerNotificationTemplate)
                        mailSubject.Replace("%USER%", CurrentPecMail.Handler)
                        mailSubject.Replace("%USEREMAIL%", CurrentPecMail.Handler)
                        mailSubject.Replace("%ID%", CurrentPecMail.Id.ToString())
                        mailSubject.Replace("%SUBJECT%", CurrentPecMail.MailSubject)
                        mailSubject.Replace("%DATE%", CurrentPecMail.MailDate.Value.ToString())

                        Facade.MessageFacade.NotifyToPreviousHandler(CurrentPecMail.Handler, oldHandler, mailSubject.ToString(), ProtocolEnv.DispositionNotification)
                    End If
                Catch ex As Exception
                    FileLogger.Warn(LoggerName, "Errore nella presa in carico PEC", ex)
                End Try
            Case "False"
                Dim previousHandler As String = CurrentPecMail.Handler
                CurrentPecMail.Handler = String.Empty
                Facade.PECMailFacade.Update(CurrentPecMail)
                Facade.PECMailLogFacade.Handle(CurrentPecMail, previousHandler, False)
                PecUnhandled = True
        End Select
        InitializeHandler()
        InitializeButtons()
    End Sub

    Protected Sub btnRaccomandata_Click(sender As Object, e As EventArgs) Handles btnRaccomandata.Click
        If Not Me.CurrentPecMailRights.HasError Then
            Me.AjaxAlert("La consegna della PEC non ha o non ha ancora avuto esito negativo, impossibile procedere.")
            Return
        End If

        If Not CurrentPecMail.HasDocumentUnit() Then
            Me.AjaxAlert("La PEC non risulta essere gestita, impossibile procedere.")
            Return
        End If

        Response.Redirect($"~/Prot/PosteWebItem.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={CurrentPecMail.DocumentUnit.Id}&PType={Prot.PosteWeb.Item.TypeRaccomandata}")}")
    End Sub

    Private Sub cmdPECView_Click(sender As Object, e As EventArgs) Handles cmdPECView.Click
        Dim url As String = "~/PEC/PECView.aspx?Type=PEC&PECId={0}&ProtocolBox={1}"
        url = String.Format(url, Me.CurrentPecMail.Id, Me.ProtocolBoxEnabled)
        Response.Redirect(url)
    End Sub

    Private Sub btnOriginalEml_Click(sender As Object, e As EventArgs) Handles btnOriginalEml.Click
        Dim original As DocumentInfo = Facade.PECMailFacade.GetOriginalEml(CurrentPecMail, New Action(Sub() DelegateElsaInitialize(CurrentPecMail)))
        Dim url As String = original.DownloadLink()
        url = String.Format(url, Me.CurrentPecMail.Id, Me.ProtocolBoxEnabled)
        AjaxManager.ResponseScripts.Add(String.Format("StartDownload('{0}');", url))
    End Sub


    Private Sub cmdResend_Click(sender As Object, e As EventArgs) Handles cmdResend.Click
        Dim resended As PECMail = FacadeFactory.Instance.PECMailFacade.Resend(Me.CurrentPecMail)
        Dim qs As String = "Type=PEC&PECId={0}"
        qs = String.Format(qs, resended.Id)
        qs = CommonShared.AppendSecurityCheck(qs)
        Response.Redirect("~/PEC/PECSummary.aspx?" & qs)
    End Sub

    Private Sub btnFisicalRemovePEC_Click(sender As Object, e As EventArgs) Handles btnFisicalRemovePEC.Click
        windowManager.RadConfirm("Sei sicuro di voler eliminare il PEC?", "confirmFisicalRemovePEC", 400, 150, Nothing, "Elimina Fisica PEC")
    End Sub

    Private Sub FisicalRemovePEC()
        FacadeFactory.Instance.TableLogFacade.Insert("PECMailBox", LogEvent.INS, String.Format("Il PECMail è stato eliminata fisicamente: {0}, {1}, {2}", CurrentPecMail.Id, CurrentPecMail.MailSubject, CurrentPecMail.MailDate), CurrentPecMail.Id)

        'Errore [HibernateException: The length of the string value exceeds the length configured in the mapping/parameter.

        'Dim serializeSettings As JsonSerializerSettings = New JsonSerializerSettings()
        'serializeSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        'Dim pecMailJson As String = JsonConvert.SerializeObject(CurrentPecMail, Formatting.Indented, serializeSettings)
        ''FacadeFactory.Instance.TableLogFacade.Insert("PECMailBox", LogEvent.INS, pecMailJson, CurrentPecMail.Id)

        DetachDocuments()

        Facade.PECMailFacade.Delete(CurrentPecMail)

        Dim outgoing As Boolean = CurrentPecMail.Direction.Equals(PECMailDirection.Outgoing)
        If outgoing Then
            Response.Redirect(String.Format("PECOutgoingMails.aspx?Type=Pec&ProtocolBox={0}", ProtocolBoxEnabled))
            Return
        End If
        Response.Redirect(String.Format("PECIncomingMails.aspx?Type=Pec&ProtocolBox={0}", ProtocolBoxEnabled))

    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf PECSummary_AjaxRequest

        AjaxManager.AjaxSettings.AddAjaxSetting(btnPECToDocumentUnit, pnlMainContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnAttachPec, pnlMainContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnReply, pnlMainContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnMovePEC, pnlMainContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnMail, pnlMainContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdPECView, pnlMainContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnDelete, pnlMainContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnFisicalRemovePEC, pnlMainContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRestore, pnlMainContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnForward, pnlMainContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnReceipt, pnlMainContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnViewLog, pnlMainContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnHandle, pnlMainContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnUnhandle, pnlMainContent, MasterDocSuite.AjaxDefaultLoadingPanel)


        AjaxManager.AjaxSettings.AddAjaxSetting(btnPECToDocumentUnit, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnAttachPec, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnReply, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnMovePEC, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnMail, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdPECView, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnDelete, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnFisicalRemovePEC, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRestore, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnForward, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnReceipt, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnViewLog, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnHandle, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnUnhandle, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnOriginalEml, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRaccomandata, pnlMainContent, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdResend, pnlMainContent, MasterDocSuite.AjaxFlatLoadingPanel)
    End Sub

    Private Sub InitializeButtons()
        If Not IsPostBack Then
            btnMovePEC.PostBackUrl = String.Format("{0}?Type=PEC&ids={1}&ProtocolBox={2}", btnMovePEC.PostBackUrl, CurrentPecMail.Id, ProtocolBoxEnabled)
            btnDestinate.PostBackUrl = String.Format("{0}?Type=PEC&id={1}&ProtocolBox={2}", btnDestinate.PostBackUrl, CurrentPecMail.Id, ProtocolBoxEnabled)
            btnPECToDocumentUnit.PostBackUrl = String.Format("{0}?Type=PEC&id={1}&ProtocolBox={2}", btnPECToDocumentUnit.PostBackUrl, CurrentPecMail.Id, ProtocolBoxEnabled)
            btnAttachPec.PostBackUrl = String.Format("{0}?Type=PEC&id={1}&ProtocolBox={2}", btnAttachPec.PostBackUrl, CurrentPecMail.Id, ProtocolBoxEnabled)
            btnLinkToProtocol.PostBackUrl = String.Format("{0}?Type=PEC&id={1}&ProtocolBox={2}", btnLinkToProtocol.PostBackUrl, CurrentPecMail.Id, ProtocolBoxEnabled)
            btnForward.PostBackUrl = String.Format("{0}?Type=PEC&PECIds={1}&ProtocolBox={2}&SimpleMode={3}&ForwardPECMode=True", btnForward.PostBackUrl, CurrentPecMail.Id, ProtocolBoxEnabled, DocSuiteContext.Current.ProtocolEnv.PECSimpleMode)
            btnReply.PostBackUrl = String.Format("{0}?Type=PEC&PECId={1}&ProtocolBox={2}&SimpleMode={3}", btnReply.PostBackUrl, CurrentPecMail.Id, ProtocolBoxEnabled, DocSuiteContext.Current.ProtocolEnv.PECSimpleMode)
            btnReplyAll.PostBackUrl = String.Format("{0}?Type=PEC&PECId={1}&ProtocolBox={2}&SimpleMode={3}&ReplyAll={4}", btnReplyAll.PostBackUrl, CurrentPecMail.Id, ProtocolBoxEnabled, DocSuiteContext.Current.ProtocolEnv.PECSimpleMode, DocSuiteContext.Current.ProtocolEnv.PECReplyAllEnabled)
            btnMail.PostBackUrl = String.Format("{0}?Type=PEC&PECId={1}", btnMail.PostBackUrl, CurrentPecMail.Id)
            btnViewLog.PostBackUrl = String.Format("{0}?Type=PEC&PECId={1}&ProtocolBox={2}", btnViewLog.PostBackUrl, CurrentPecMail.Id, ProtocolBoxEnabled)
            btnFix.PostBackUrl = String.Format("{0}?Type=PEC&PECId={1}", btnFix.PostBackUrl, CurrentPecMail.Id)
            btnReply.Text = String.Format("Rispondi {0}", PecLabel)
            btnForward.Text = String.Format("Inoltra {0}", PecLabel)
            btnViewLog.Visible = CommonShared.HasGroupPecMailLogViewRight
            btnFix.Visible = CurrentPecMail.Direction = PECMailDirection.Outgoing AndAlso CurrentPecMail.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Error) AndAlso CommonShared.HasGroupPECFixedRight
            If ProtocolEnv.PECReplyAllEnabled Then
                btnReplyAll.Text = "Rispondi a tutti"
            End If

            Me.btnRaccomandata.Visible = DocSuiteContext.Current.ProtocolEnv.FastProtocolSenderEnabled _
                AndAlso DocSuiteContext.Current.ProtocolEnv.IsRaccomandataEnabled
            Me.cmdResend.Visible = DocSuiteContext.Current.ProtocolEnv.FastProtocolSenderEnabled

        End If

        If Not IsReadOnly Then
            ' Gestione presa in carico
            If CurrentPecMail.MailBox.PecHandleEnabled() AndAlso Not CurrentPecMail.Direction.Equals(PECMailDirection.Outgoing) Then
                btnHandle.Visible = CurrentPecMailRights.CanHandle
                btnUnhandle.Visible = (Not btnHandle.Visible) AndAlso CurrentPecMailRights.IsHandler
            End If

            If DocSuiteContext.Current.ProtocolEnv.EnableButtonAllega Then
                btnAttachPec.Enabled = True
                btnAttachPec.Visible = True
            End If

            btnDelete.Enabled = CurrentPecMailRights.IsDeletable AndAlso ProtocolEnv.PECDeleteButtonEnabled

            btnPECToDocumentUnit.Visible = (CurrentUserHasUDSsRights OrElse CurrentUserHasProtocolContainersRights)
            btnPECToDocumentUnit.Enabled = If(btnPECToDocumentUnit.Visible, CurrentPecMailRights.IsProtocollable, False)

            btnAttachPec.Enabled = CurrentPecMailRights.IsAttachable

            btnLinkToProtocol.Visible = False
            If ProtocolEnv.EnableLinkToProtocol Then
                btnLinkToProtocol.Visible = True
                btnLinkToProtocol.Enabled = CurrentPecMailRights.IsAttachable
            End If

            btnMail.Enabled = CurrentPecMailRights.IsSendable
            ' Abilito il reinvio tramite PEC/POL se e solo se la PEC è abbinata ad un protocollo del quale ho il permesso di spedizione.
            btnRaccomandata.Enabled = Me.CurrentPecMailRights.IsResendable
            cmdResend.Enabled = Me.CurrentPecMailRights.IsResendable

            btnReply.Enabled = CurrentPecMailRights.IsForwardable
            btnReplyAll.Enabled = CurrentPecMailRights.IsForwardable
            btnForward.Enabled = CurrentPecMailRights.IsForwardable

            If CurrentPecMailRights.IsActive Then
                btnMovePEC.Enabled = True
            End If

            btnDestinate.Visible = ProtocolEnv.IsPECDestinationEnabled
            btnDestinate.Enabled = CurrentPecMailRights.IsDestinable
        End If

        If CurrentPecMailRights.IsRestorable Then
            btnRestore.Visible = True
            btnRestore.Enabled = Not IsReadOnly
            btnDelete.Visible = False
        Else
            btnRestore.Visible = False
            btnDelete.Enabled = Not IsReadOnly AndAlso CurrentPecMailRights.IsDeletable AndAlso ProtocolEnv.PECDeleteButtonEnabled
            btnDelete.Visible = ProtocolEnv.PECDeleteButtonEnabled
        End If

        ' Sposta PEC se è abilitato lo spostamento e la PEC non è in uscita
        If ProtocolEnv.PECMoveEnabled AndAlso Not CurrentPecMail.Direction.Equals(PECMailDirection.Outgoing) Then
            btnMovePEC.Visible = True
            btnMovePEC.Enabled = CurrentPecMailRights.IsHandler AndAlso Not CurrentPecMailRights.HasAnomalies
        Else
            btnMovePEC.Visible = False
        End If

        ' Rispondi PEC se è abilitato la risposta e la PEC non è in uscita
        If ProtocolEnv.PECReplyEnabled AndAlso Not CurrentPecMail.Direction.Equals(PECMailDirection.Outgoing) Then
            btnReply.Visible = True
            btnReply.Text = String.Format("Rispondi {0}", PecLabel)
            If CurrentPecMail.MailRecipients.Count > 0 And ProtocolEnv.PECReplyAllEnabled Then
                btnReplyAll.Visible = True
                btnReplyAll.Text = "Rispondi a tutti"
            End If
        Else
            btnReply.Visible = False
        End If

        btnReceipt.Enabled = CurrentPecMailRights.IsReceiptViewable

        If CommonShared.HasGroupPECFisicalDeleteRight AndAlso ProtocolEnv.RemovePECEnabledInPECMailBoxes.Contains(CurrentPecMail.MailBox.Id) Then
            btnFisicalRemovePEC.Enabled = True
        End If

        If CurrentConservation IsNot Nothing AndAlso CurrentConservation.Status = ConservationStatus.Conservated Then
            btnDelete.Visible = False
        End If

        If ProtocolEnv.PECMailInsertAuthorizationEnabled AndAlso Not Facade.DomainUserFacade.HasCurrentRight(CurrentDomainUser, DSWEnvironmentType.Protocol, DomainUserFacade.HasPECSendableRight) Then
            btnReply.Visible = False
            btnReplyAll.Visible = False
        End If

        If ProtocolEnv.WorkflowManagerEnabled Then
            btnWorkflow.Enabled = True
            btnWorkflow.Visible = True
        End If
    End Sub

    Private Sub InitializePec()

        If CurrentPecMail.HasDocumentUnit() AndAlso CurrentPecMail.DocumentUnit IsNot Nothing AndAlso CurrentPecMail.DocumentUnit.Environment = DSWEnvironment.Protocol Then
            Dim protocol As Protocol = Facade.ProtocolFacade.GetById(CurrentPecMail.DocumentUnit.Id)
            uscProtocolPreview.CurrentProtocol = protocol
        Else
            uscProtocolPreview.Visible = False
        End If

        lblFrom.Text = Server.HtmlEncode(CurrentPecMail.MailSenders)
        lblTo.Text = Server.HtmlEncode(CurrentPecMail.MailRecipients)
        If ProtocolEnv.EnableCc Then
            trCc.Visible = True
            lblCc.Text = Server.HtmlEncode(CurrentPecMail.MailRecipientsCc)
        End If
        lblSubject.Text = Server.HtmlEncode(CurrentPecMail.MailSubject)

        tblBody.Visible = False
        If ProtocolEnv.PecShowBody Then
            tblBody.Visible = True
            lblBody.Content = If(String.IsNullOrWhiteSpace(CurrentPecMail.MailBody), "Nessun messaggio presente", CurrentPecMail.MailBody)
            If (lblBody.Content.Contains("data:image/jpeg;base64") AndAlso Not lblBody.Content.EndsWith("' />")) Then
                lblBody.Content = String.Concat(lblBody.Content, "' />")
            End If
        End If

        If CurrentPecMail IsNot Nothing AndAlso CurrentPecMail.Direction = PECMailDirection.Outgoing Then
            If CurrentPecMail.MailDate.HasValue Then
                lblMainData.Text = String.Format("Inviata {0:dddd dd/MM/yyyy HH:mm}, Casella [{1}]", CurrentPecMail.MailDate.Value, Facade.PECMailboxFacade.MailBoxRecipientLabel(CurrentPecMail.MailBox))
            Else
                lblMainData.Text = String.Format("NON ANCORA SPEDITA, Casella [{0}]", Facade.PECMailboxFacade.MailBoxRecipientLabel(CurrentPecMail.MailBox))
            End If
        Else
            lblMainData.Text = String.Format("Ricevuta {0:dddd dd/MM/yyyy HH:mm}, Casella [{1}]", CurrentPecMail.MailDate.Value, Facade.PECMailboxFacade.MailBoxRecipientLabel(CurrentPecMail.MailBox))
        End If

        If Not CurrentPecMail.MailBox.PecHandleEnabled() OrElse CurrentPecMail.HasDocumentUnit() OrElse Not CurrentPecMail.Direction.Equals(PECMailDirection.Ingoing) Then
            lblHandler.Visible = False
        Else
            InitializeHandler()
        End If

        If ProtocolEnv.IsPECDestinationEnabled AndAlso CurrentPecMail.MailBox.IsDestinationEnabled.GetValueOrDefault(False) AndAlso Not String.IsNullOrEmpty(CurrentPecMail.DestinationNote) Then
            lblDestination.Text = String.Concat("NOTE DI DESTINAZIONE: ", CurrentPecMail.DestinationNote)
            pnlDestination.CssClass = "warningArea"
        End If

        ''Gestione Size
        lblPecSize.Text = Facade.PECMailFacade.GetCalculatedSize(CurrentPecMail, New Action(Sub() DelegateElsaInitialize(CurrentPecMail)))

        lblStatus.Text = ActiveType.Message(CurrentPecMail.IsActive)

        If CurrentPecMail.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Processing) Then
            DirectCast(lblStatus.Parent, HtmlControl).Attributes.CssStyle("background-color") = "Yellow"
        End If
        If CurrentPecMail.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Error) Then
            DirectCast(lblStatus.Parent, HtmlControl).Attributes.CssStyle("background-color") = "Red"
        End If
        Dim log As PECMailLog

        tblAnnullamento.Visible = False
        lblInfo.Text = "-"
        If CurrentPecMail IsNot Nothing Then
            log = Facade.PECMailLogFacade.GetLogTypeByPEC(CurrentPecMail, PECMailLogType.Move)
            If log IsNot Nothing Then
                lblInfo.Text = $"{log.Description} in data {log.Date.ToShortDateString()}"
            End If
            If CurrentPecMail.IsActive <> 1 Then
                log = Facade.PECMailLogFacade.GetLogTypeByPEC(CurrentPecMail, PECMailLogType.Delete)
                If log IsNot Nothing Then
                    lblDeleteInfo.Text = $"{log.Description} in data {log.Date.ToShortDateString()}"
                    tblAnnullamento.Visible = True
                End If
            End If
        End If

        If CurrentPecMail.Direction = PECMailDirection.Outgoing Then
            uscPecHistory.PecHistory = Facade.PECMailFacade.GetOutgoingMailHistory(CurrentPecMail.Id)
        End If
        uscPecHistory.BindData()
    End Sub

    Private Sub DetachDocuments()
        If CurrentPecMail.IDSmime <> Guid.Empty Then
            Try
                Service.DetachDocument(CurrentPecMail.IDSmime)
            Catch ex As Exception
                Throw New DocSuiteException("Annullamento PEC", String.Format("Errore in fase Detach del documento con IDSmine = {0}, impossible eseguire.", CurrentPecMail.IDSmime), ex)
            End Try
        End If

        If CurrentPecMail.IDSegnatura <> Guid.Empty Then
            Try
                Service.DetachDocument(CurrentPecMail.IDSegnatura)
            Catch ex As Exception
                Throw New DocSuiteException("Annullamento PEC", String.Format("Errore in fase Detach del documento con IDSegnatura = {0}, impossible eseguire.", CurrentPecMail.IDSegnatura), ex)
            End Try
        End If
        If CurrentPecMail.IDPostacert <> Guid.Empty Then
            Try
                Service.DetachDocument(CurrentPecMail.IDPostacert)
            Catch ex As Exception
                Throw New DocSuiteException("Annullamento PEC", String.Format("Errore in fase Detach del documento con IDPostacert = {0}, impossible eseguire.", CurrentPecMail.IDPostacert), ex)
            End Try
        End If
        If CurrentPecMail.IDMailContent <> Guid.Empty Then
            Try
                Service.DetachDocument(CurrentPecMail.IDMailContent)
            Catch ex As Exception
                Throw New DocSuiteException("Annullamento PEC", String.Format("Errore in fase Detach del documento con IDMailContent = {0}, impossible eseguire.", CurrentPecMail.IDMailContent), ex)
            End Try
        End If
        If CurrentPecMail.IDEnvelope <> Guid.Empty Then
            Try
                Service.DetachDocument(CurrentPecMail.IDEnvelope)
            Catch ex As Exception
                Throw New DocSuiteException("Annullamento PEC", String.Format("Errore in fase Detach del documento con IDEnvelope = {0}, impossible eseguire.", CurrentPecMail.IDEnvelope), ex)
            End Try
        End If
        If CurrentPecMail.IDDaticert <> Guid.Empty Then
            Try
                Service.DetachDocument(CurrentPecMail.IDDaticert)
            Catch ex As Exception
                Throw New DocSuiteException("Annullamento PEC", String.Format("Errore in fase Detach del documento con IDDaticert = {0}, impossible eseguire.", CurrentPecMail.IDDaticert), ex)
            End Try
        End If
        If CurrentPecMail.IDAttachments <> Guid.Empty Then
            Try
                Service.DetachDocument(CurrentPecMail.IDAttachments)
            Catch ex As Exception
                Throw New DocSuiteException("Annullamento PEC", String.Format("Errore in fase Detach del documento con IDAttachments = {0}, impossible eseguire.", CurrentPecMail.IDAttachments), ex)
            End Try
        End If

    End Sub

    Private Sub InitIcons()
        Dim cell As TableCell = CType(TblButtons.FindControl("IconsCell"), TableCell)

        If Not CurrentPecMail.Attachments.IsNullOrEmpty() Then
            AggiungiIcona(cell, ImagePath.BigEmailDocumentsAttach, String.Format("Allegati: {0}", CurrentPecMail.Attachments.Count))
        End If

        If Not String.IsNullOrEmpty(CurrentPecMail.Segnatura) Then
            If CurrentPecMail.IsValidForInterop Then
                AggiungiIcona(cell, ImagePath.BigCodeGreen, "Valida per Interoperabilità")
            Else
                AggiungiIcona(cell, ImagePath.BigCodeGray, "Interoperabilità non convalidata")
            End If
        End If

        ' Aggiungo le icone delle coccarde di PEC
        Dim kvp As KeyValuePair(Of String, String) = CoccardaManager.GetImage(CurrentPecMail, ProtocolEnv.CoccardaProtocolEnabled, True)
        If Not String.IsNullOrEmpty(kvp.Key) Then
            AggiungiIcona(cell, kvp.Key, kvp.Value)
        End If


        If ProtocolEnv.ConservationEnabled AndAlso CurrentConservation IsNot Nothing Then
            AggiungiIcona(cell, ConservationHelper.StatusBigIcon(CurrentConservation.Status), ConservationHelper.StatusDescription(CurrentConservation.Status))
        End If
    End Sub

    Private Sub AggiungiIcona(ByVal c As TableCell, ByVal imageUrl As String, ByVal toolTip As String)
        Dim a As New Image
        a.ImageUrl = imageUrl
        a.ToolTip = toolTip
        a.Style("margin-bottom") = "5px"
        c.Controls.Add(a)
        c.Controls.Add(WebHelper.BrControl)
    End Sub

    Private Sub InitializeHandler()
        If Not CurrentPecMail.MailBox.PecHandleEnabled() OrElse
                CurrentPecMail.Direction.Equals(PECMailDirection.Outgoing) Then
            Exit Sub
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("PECHandler")) Then
            If Request.QueryString("PECHandler").Equals("True") Then
                CurrentPecMail.Handler = String.Empty
            End If
        End If



        Dim handling As Boolean = (Not String.IsNullOrEmpty(CurrentPecMail.Handler) AndAlso CurrentPecMail.Handler.Eq(DocSuiteContext.Current.User.FullUserName))


        If CurrentPecMail.Direction <> PECMailDirection.Ingoing Then
            Exit Sub
        End If

        If handling Then
            lblHandler.Text = String.Format("{0} in carico all'utente corrente", PecLabel)
            Exit Sub
        End If

        If String.IsNullOrEmpty(CurrentPecMail.Handler) Then
            lblHandler.Text = String.Format("{0} non in carico", PecLabel)
            Exit Sub
        End If

        Dim handlerName As String = CommonAD.GetDisplayName(CurrentPecMail.Handler)
        AjaxAlert("ATTENZIONE!{0}{2} in gestione all'utente {1}.", Environment.NewLine, handlerName, PecLabel)
        Dim temp As String
        If CurrentPecMail.LastChangedDate.HasValue Then
            temp = String.Format("{2} in carico all'utente {0} dal {1}", handlerName, CurrentPecMail.LastChangedDate.Value.ToUniversalTime().ToString(), PecLabel)
        Else
            temp = String.Format("{0} in carico all'utente {1}", PecLabel, handlerName)
        End If
        lblHandler.Text = temp
    End Sub

    Public Sub LoadConservation()
        trConservationStatus.Visible = False

        If ProtocolEnv.ConservationEnabled AndAlso CurrentConservation IsNot Nothing Then
            trConservationStatus.Visible = True
            conservationIcon.ImageUrl = ConservationHelper.StatusSmallIcon(CurrentConservation.Status)
            lblConservationStatus.Text = ConservationHelper.StatusDescription(CurrentConservation.Status)

            If String.IsNullOrEmpty(CurrentConservation.Uri) Then
                Exit Sub
            End If

            If Not Regex.IsMatch(CurrentConservation.Uri, ProtocolEnv.ConservationURIValidationRegex) Then
                lblConservationUri.Visible = True
                conservationUriLabel.Visible = True

                lblConservationStatus.ToolTip = "Url non compatibile con il portale ingestor"
                lblConservationUri.Text = CurrentConservation.Uri
                Exit Sub
            End If
            If Not String.IsNullOrWhiteSpace(ProtocolEnv.IngestorBaseURL) Then
                lblConservationStatus.NavigateUrl = $"{ProtocolEnv.IngestorBaseURL}/{CurrentConservation.Uri}"
            End If
        End If
    End Sub
#End Region



End Class