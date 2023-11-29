Imports System.Text
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Facade.ExtensionMethods
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade.PEC
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Logging
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Gui.Viewers
Imports VecompSoftware.Helpers.Web
Imports VecompSoftware.Helpers
Imports VecompSoftware.Services.Biblos.Models

Public Class PECView
    Inherits PECBasePage
    Implements IHaveViewerLight
    Implements ISendMail
    Implements IHavePecMail

#Region " Fields "

    Public Const SessionSeed As String = "pmvSeed"

#End Region

#Region " Properties "

    Private ReadOnly Property WindowedMode() As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean)("vo", False)
        End Get
    End Property

    Public ReadOnly Property CheckedDocuments() As List(Of DocumentInfo) Implements IHaveViewerLight.CheckedDocuments
        Get
            Return viewerLight.CheckedDocuments
        End Get
    End Property

    Public ReadOnly Property SelectedDocument() As DocumentInfo Implements IHaveViewerLight.SelectedDocument
        Get
            Return viewerLight.SelectedDocument
        End Get
    End Property

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
            Dim docs As IList(Of DocumentInfo) = viewerLight.CheckedDocuments
            If docs.Count = 0 Then
                docs = Facade.PECMailFacade.GetDocumentList(CurrentPecMailWrapper)
            End If
            Return docs
        End Get
    End Property

    Public ReadOnly Property Subject As String Implements ISendMail.Subject
        Get
            Return String.Empty
        End Get
    End Property

    Public ReadOnly Property Body As String Implements ISendMail.Body
        Get
            Return String.Empty
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()

        ' Aggiunge le signature ai documenti se abilitate da configurazione
        CreateSignature()

        viewerLight.EnabledCheckIsSignedButton = Not ProtocolEnv.CheckSignedEvaluateStream
        If WindowedMode Then
            MasterDocSuite.TitleVisible = False
        End If
        If Not IsPostBack AndAlso Not Page.IsCallback Then
            SetResponseNoCache()
            If CurrentPecMail.MailBox.PecHandleEnabled() AndAlso _
                CommonShared.UserConnectedBelongsTo(ProtocolEnv.GroupPecAutoHandle, True) AndAlso _
                CurrentPecMail.Direction = PECMailDirection.Ingoing AndAlso _
                String.IsNullOrEmpty(CurrentPecMail.Handler) AndAlso _
                CurrentPecMailRights.CanHandle AndAlso _
                Not PecUnhandled Then
                ' Prendi in carico la PEC
                CurrentPecMail.Handler = DocSuiteContext.Current.User.FullUserName

                CleanCurrentPecMailRights()

                Facade.PECMailFacade.Update(CurrentPecMail)
                '' Aggiungo il Log della presa in carico automatica
                Facade.PECMailLogFacade.Handle(CurrentPecMail, CurrentPecMail.Handler, True)
            End If
            InitializeButtons()

            Title = String.Format("{0:dddd dd/MM/yyyy} - {1} - [{2}]", If(CurrentPecMail.MailDate.HasValue, CurrentPecMail.MailDate.Value, CurrentPecMail.RegistrationDate.ToLocalTime().Date), CurrentPecMail.MailSubject, Facade.PECMailFacade.GetCalculatedSize(CurrentPecMail, New Action(Sub() DelegateElsaInitialize(CurrentPecMail))))
            viewerLight.CheckBoxes = True
            Facade.PECMailLogFacade.Read(CurrentPecMail)

            If ProtocolEnv.IsPECDestinationEnabled AndAlso CurrentPecMail.MailBox.IsDestinationEnabled.GetValueOrDefault(False) AndAlso Not String.IsNullOrEmpty(CurrentPecMail.DestinationNote) Then
                lblDestination.Text = String.Concat("NOTE DI DESTINAZIONE: ", CurrentPecMail.DestinationNote)
                pnlDestination.CssClass = "warningArea"
            End If

            InitializeLog()

            BindViewerLight()
        End If
    End Sub

    Private Sub CreateSignature()
        ' Decorazione della signature da mettere nella testata dei documenti della mail in caso di Accettazione delle PEC inviate
        If ProtocolEnv.EnableSignaturePECAccepted = True Then
            Dim currentPecMailOutgoing As PECMail = Facade.PECMailFacade.GetOutgoingMailHistoryByStatus(CurrentPecMail, EnumHelper.GetDescription(PECMailReceiptType.AvvenutaConsegna), PECMailType.Receipt)
            If Not currentPecMailOutgoing Is Nothing AndAlso Not ProtocolBoxEnabled Then
                viewerLight.AddSignature = String.Format(ProtocolEnv.SignaturePECAccepted, If(String.IsNullOrEmpty(currentPecMailOutgoing.MailType), String.Empty, currentPecMailOutgoing.MailType.Replace("-", " ")), CurrentPecMail.MailDate)
            End If
        End If

        ' Decorazione della signature da mettere nella testata dei documenti delle PEC in caso di RICEVUTA
        If ProtocolEnv.EnableSignaturePECReceived = True Then
            If CurrentPecMail.PECType = PECMailType.PEC AndAlso CurrentPecMail.Direction = PECMailDirection.Ingoing AndAlso Not ProtocolBoxEnabled Then
                viewerLight.AddSignature = String.Format(ProtocolEnv.SignaturePECReceived, "ricevuta", CurrentPecMail.MailDate)
            End If
        End If

        ' Decorazione della signature da mettere nella testata dei documenti della mail in caso di RICEVUTA
        If ProtocolEnv.EnableSignatureMailReceived = True Then
            If (CurrentPecMail.PECType = PECMailType.Receipt Or CurrentPecMail.PECType = PECMailType.Anomalia) _
                AndAlso CurrentPecMail.Direction = PECMailDirection.Ingoing Then

                viewerLight.AddSignature = String.Format(ProtocolEnv.SignatureMailReceived, "ricevuta", CurrentPecMail.MailDate)
            End If
        End If
    End Sub

    Private Sub BtnHandleClick(sender As Object, e As EventArgs) Handles btnHandle.Click, btnUnhandle.Click
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
                        Dim text As New StringBuilder(ProtocolEnv.PECHandlerNotificationTemplate)
                        text.Replace("%USER%", CurrentPecMail.Handler)
                        text.Replace("%USEREMAIL%", CurrentPecMail.Handler)
                        text.Replace("%ID%", CurrentPecMail.Id.ToString())
                        text.Replace("%SUBJECT%", CurrentPecMail.MailSubject)
                        text.Replace("%DATE%", CurrentPecMail.MailDate.Value.ToString())

                        Facade.MessageFacade.NotifyToPreviousHandler(CurrentPecMail.Handler, oldHandler, text.ToString(), ProtocolEnv.DispositionNotification)
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
        InitializeButtons()
    End Sub

    Private Sub cmdPECView_Click(sender As Object, e As EventArgs) Handles cmdPECView.Click
        Response.Redirect(String.Format("~/PEC/PECSummary.aspx?Type=PEC&PECId={0}&ProtocolBox={1}", CurrentPecMail.Id, ProtocolBoxEnabled))
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnPECToDocumentUnit, viewerLight, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnAttachPec, viewerLight, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnReply, viewerLight, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnMovePEC, viewerLight, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdPECView, viewerLight, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnDelete, viewerLight, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRestore, viewerLight, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnForward, viewerLight, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnReceipt, viewerLight, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnViewLog, viewerLight, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnHandle, viewerLight, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnUnhandle, viewerLight, MasterDocSuite.AjaxDefaultLoadingPanel)


        AjaxManager.AjaxSettings.AddAjaxSetting(btnPECToDocumentUnit, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnAttachPec, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnReply, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnMovePEC, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdPECView, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnDelete, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRestore, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnForward, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnReceipt, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnViewLog, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnHandle, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnUnhandle, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)

        If Not viewerLight.Initialized Then
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, viewerLight)
        End If
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, WarningPanel)
    End Sub

    Private Sub InitializeLog()
        ' AREA LOG
        ' Fisso una stringa di default per segnalare i warning.
        Dim fixedStringWarn As String = ProtocolEnv.PECWarningMessage

        ' Visualizzo solamente il messaggio per le mail in stato di Warning.
        If CurrentPecMail.LogEntries.Where(Function(o) o.Type.Eq(PECMailLogType.Warning.ToString())).Count() > 0 Then
            WarningLabel.Text = fixedStringWarn
            WarningPanel.CssClass = "warningAreaLow"
        End If
    End Sub

    Private Sub BindViewerLight()
        Dim viewList As IList(Of PECMailView) = Nothing

        'Definisco se devo utilizzare la vista di default per la Casella di protocollazione
        If ProtocolBoxEnabled Then
            Dim protocolBoxPecMailViewDefault As PECMailView = Facade.PECMailViewFacade.GetById(ProtocolEnv.ProtocolBoxPecMailViewDefault)
            If protocolBoxPecMailViewDefault IsNot Nothing Then
                viewList = New List(Of PECMailView)({protocolBoxPecMailViewDefault})
            End If
        Else
            'Altrimenti carico tutte le viste ammesse
            viewList = Facade.PECMailViewFacade.GetByRightAndPageType()
        End If

        ' Verifico se è presente una vista di default a livello di applicazione
        If viewList Is Nothing OrElse viewList.Count = 0 Then
            Dim systemDefaultPecMailView As PECMailView = Facade.PECMailViewFacade.GetById(ProtocolEnv.PecMailViewDefault)
            If systemDefaultPecMailView IsNot Nothing Then
                viewList = New List(Of PECMailView)({systemDefaultPecMailView})
            End If
        End If

        ' Genero una lista di FolderInfo per caricare il datasource
        Dim dataSourceList As New List(Of DocumentInfo)
        ' Se non ho viste da utilizzare, carico il binding classico con valore Nothing.
        If viewList Is Nothing OrElse viewList.Count = 0 Then
            dataSourceList.Add(Facade.PECMailFacade.GetDocumentInfo(CurrentPecMailWrapper, Nothing))
            viewerLight.MultiViewEnable = False
        Else
            ' Altrimenti carico il valore di default e procedo a 
            Dim defaultPecMailView As PECMailView = Facade.PECMailViewFacade.GetDefault(viewList)
            viewerLight.MultiViewEnable = True
            ' Se disponibile carico una vista di default
            If defaultPecMailView IsNot Nothing Then
                viewerLight.MultiViewDefaultId = defaultPecMailView.Id
            End If

            ' Ricreo la struttura per ogni vista definita
            For Each pecMailView As PECMailView In viewList
                ' Creo un nodo padre aggiuntivo contenente id della view e title
                Dim pecMailViewFolderInfo As New FolderInfo(pecMailView.Id.ToString(), pecMailView.Title)
                ' Inizializzo nuovamente l'oggetto
                Dim tempPecMail As BiblosPecMailWrapper = New BiblosPecMailWrapper(CurrentPecMail, ProtocolEnv.CheckSignedEvaluateStream)
                Dim temp As DocumentInfo = Facade.PECMailFacade.GetDocumentInfo(tempPecMail, pecMailView)
                temp.Parent = pecMailViewFolderInfo
                dataSourceList.Add(pecMailViewFolderInfo)
            Next
        End If

        viewerLight.DataSource = dataSourceList
    End Sub

    Private Sub InitializeButtons()
        If Not IsPostBack Then
            btnMovePEC.PostBackUrl = String.Format("{0}?Type=PEC&ids={1}&ProtocolBox={2}", btnMovePEC.PostBackUrl, CurrentPecMail.Id, ProtocolBoxEnabled)
            btnDestinate.PostBackUrl = String.Format("{0}?Type=PEC&id={1}&ProtocolBox={2}", btnDestinate.PostBackUrl, CurrentPecMail.Id, ProtocolBoxEnabled)
            btnPECToDocumentUnit.PostBackUrl = String.Format("{0}?Type=PEC&id={1}&ProtocolBox={2}", btnPECToDocumentUnit.PostBackUrl, CurrentPecMail.Id, ProtocolBoxEnabled)
            btnAttachPec.PostBackUrl = String.Format("{0}?Type=PEC&id={1}&ProtocolBox={2}", btnAttachPec.PostBackUrl, CurrentPecMail.Id, ProtocolBoxEnabled)
            btnLinkToProtocol.PostBackUrl = String.Format("{0}?Type=PEC&id={1}&ProtocolBox={2}", btnLinkToProtocol.PostBackUrl, CurrentPecMail.Id, ProtocolBoxEnabled)
            btnForward.PostBackUrl = String.Format("{0}?Type=PEC&PECIds={1}&ProtocolBox={2}&ForwardPECMode=True&SimpleMode=True", btnForward.PostBackUrl, CurrentPecMail.Id, ProtocolBoxEnabled)
            btnReply.PostBackUrl = String.Format("{0}?Type=PEC&PECId={1}&ProtocolBox={2}&SimpleMode={3}", btnReply.PostBackUrl, CurrentPecMail.Id, ProtocolBoxEnabled, DocSuiteContext.Current.ProtocolEnv.PECSimpleMode)
            btnViewLog.PostBackUrl = String.Format("{0}?Type=PEC&PECId={1}&ProtocolBox={2}", btnViewLog.PostBackUrl, CurrentPecMail.Id, ProtocolBoxEnabled)
            btnMail.PostBackUrl = String.Format("{0}?Type=PEC&PECId={1}&ProtocolBox={2}", btnMail.PostBackUrl, CurrentPecMail.Id, ProtocolBoxEnabled)
            btnReply.Text = String.Format("Rispondi {0}", PecLabel)
            btnForward.Text = String.Format("Inoltra {0}", PecLabel)
        End If

        If Not IsReadOnly Then
            ' Gestione presa in carico
            If CurrentPecMail.MailBox.PecHandleEnabled() AndAlso Not CurrentPecMail.Direction.Equals(PECMailDirection.Outgoing) Then
                btnHandle.Visible = CurrentPecMailRights.CanHandle
                btnUnhandle.Visible = (Not btnHandle.Visible) AndAlso CurrentPecMailRights.IsHandler
            End If

            btnDelete.Enabled = CurrentPecMailRights.IsDeletable AndAlso ProtocolEnv.PECDeleteButtonEnabled

            btnPECToDocumentUnit.Visible = (CurrentUserHasUDSsRights OrElse CurrentUserHasProtocolContainersRights)
            btnPECToDocumentUnit.Enabled = If(btnPECToDocumentUnit.Visible, CurrentPecMailRights.IsProtocollable, False)


            btnAttachPec.Enabled = CurrentPecMailRights.IsProtocollable

                btnLinkToProtocol.Visible = False
                If ProtocolEnv.EnableLinkToProtocol Then
                    btnLinkToProtocol.Visible = True
                    btnLinkToProtocol.Enabled = CurrentPecMailRights.IsAttachable
                End If

                If CurrentPecMailRights.IsActive Then
                    btnMovePEC.Enabled = True
                    btnReply.Enabled = True

                    btnForward.Enabled = CurrentPecMailRights.IsForwardable
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
            btnMovePEC.Enabled = CurrentPecMailRights.IsHandler
        Else
            btnMovePEC.Visible = False
        End If

        ' Rispondi PEC se è abilitato la risposta e la PEC non è in uscita
        If ProtocolEnv.PECReplyEnabled AndAlso Not CurrentPecMail.Direction.Equals(PECMailDirection.Outgoing) Then
            btnReply.Visible = True
            btnReply.Text = String.Format("Rispondi {0}", PecLabel)
        Else
            btnReply.Visible = False
        End If

        btnReceipt.Enabled = CurrentPecMailRights.IsReceiptViewable

    End Sub

#End Region

End Class