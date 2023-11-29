Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Web
Imports System.Xml
Imports Limilabs.Mail
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.WorkflowsElsa
Imports VecompSoftware.DocSuiteWeb.Entity.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.Entity.Workflows
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.UDS
Imports VecompSoftware.DocSuiteWeb.Facade.WebAPI
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Commons
Imports VecompSoftware.DocSuiteWeb.Model.Workflow
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.Compress
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.LimilabsMail
Imports VecompSoftware.Helpers.Signer.Security
Imports VecompSoftware.Helpers.UDS
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers.WebAPI
Imports VecompSoftware.Helpers.Workflow
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging

Public Class PECInsert
    Inherits PECBasePage

#Region " Fields "

    Private _mailboxes As List(Of PECMailBox)
    Private _allmailboxes As List(Of PECMailBox)
    Private _interopMailBoxes As IList(Of PECMailBox)
    Private _originalMail As PECMail
    Private _selectedMailBoxId As Nullable(Of Short)
    Private _currentMailBox As PECMailBox
    Private _currentProtocol As Protocol
    Private _currentProtocolRights As ProtocolRights
    Private _currentUDS As UDSDto
    Private _UDSRepositoryFacade As UDSRepositoryFacade
    Private _signalRServerAddress As String
    Private Const NoMailContactsSeed As String = "PecInsert_NoMailContactsSeed"
    Private Const SendPecCommand As String = "SendPec"
    Private Const UpdatePecAddressCommand As String = "UpdatedPecAddress"
    Private Const SendWithStandardMailCommand As String = "SendWithStandardMail"
    Private Const DeleteExceedingAttachmentsCommand As String = "DeleteExceedingAttachments"
    Private Const SplitAttachmentsCommand As String = "SplitAttachments"
    Private Const ODATA_EQUAL_UDSID As String = "$filter=UDSId eq {0}"
    Private Const UDS_SUMMARY_PATH As String = "~/UDS/UDSView.aspx?Type=UDS&IdUDS={0}&IdUDSRepository={1}"
    Public Const NOTIFICATION_ERROR_ICON As String = "delete"
    Public Const NOTIFICATION_SUCCESS_ICON As String = "ok"
    Private Const ON_ERROR_FUNCTION As String = "onError('{0}')"
    Public Const COMMAND_SUCCESS As String = "Attendere il termine dell'attività di {0}."
    Private Const VALID_MAILBOX_IMGURL As String = "../App_Themes/DocSuite2008/images/green-dot-document.png"
    Private Const INTEROP_IMGURL As String = "../App_Themes/DocSuite2008/imgset16/user.png"
    Private _passwordGenerator As PasswordGenerator


#End Region

#Region " Properties "

    Public ReadOnly Property CurrentProtocol As Protocol
        Get
            If _currentProtocol Is Nothing AndAlso Not IdUDS.HasValue Then
                Dim uniqueIdProtocol As Guid? = Request.QueryString.GetValueOrDefault(Of Guid?)("UniqueIdProtocol", Nothing)
                If uniqueIdProtocol.HasValue Then
                    _currentProtocol = Facade.ProtocolFacade.GetById(uniqueIdProtocol.Value, False)
                End If
            End If
            Return _currentProtocol
        End Get
    End Property

    Public ReadOnly Property CurrentUDS As UDSDto
        Get
            If _currentUDS Is Nothing AndAlso IdUDS.HasValue AndAlso IdUDSRepository.HasValue Then
                Dim repository As UDSRepository = CurrentUDSRepositoryFacade.GetById(IdUDSRepository.Value)
                _currentUDS = CurrentUDSFacade.GetUDSSource(repository, String.Format(ODATA_EQUAL_UDSID, IdUDS.Value))
            End If
            Return _currentUDS
        End Get
    End Property

    Public ReadOnly Property IdUDS As Guid?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Guid?)("IdUDS", Nothing)
        End Get
    End Property

    Public ReadOnly Property IdUDSRepository As Guid?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Guid?)("IdUDSRepository", Nothing)
        End Get
    End Property

    Public ReadOnly Property CurrentProtocolRights As ProtocolRights
        Get
            If _currentProtocolRights Is Nothing Then
                _currentProtocolRights = New ProtocolRights(CurrentProtocol)
            End If
            Return _currentProtocolRights
        End Get
    End Property

    ''' <summary> Tutte le caselle di posta (anche interoperabili) </summary>
    Public ReadOnly Property MailBoxes As List(Of PECMailBox)
        Get
            If _mailboxes Is Nothing Then
                _mailboxes = New List(Of PECMailBox)
                For Each mailbox As PECMailBox In Facade.PECMailboxFacade.GetVisibleSendingMailBoxes()
                    If Facade.PECMailboxFacade.IsRealPecMailBox(mailbox) Then
                        _mailboxes.Add(mailbox)
                    End If
                Next
            End If
            Return _mailboxes
        End Get
    End Property

    Public ReadOnly Property AllMailBoxes As List(Of PECMailBox)
        Get
            If _allmailboxes Is Nothing Then
                _allmailboxes = New List(Of PECMailBox)
            End If

            If _allmailboxes.Count = 0 Then
                If ProtocolBoxEnabled Then
                    _allmailboxes = Facade.PECMailboxFacade.GetVisibleProtocolMailBoxes()
                Else
                    _allmailboxes = Facade.PECMailboxFacade.GetVisibleMailBoxes()
                End If
            End If

            Return _allmailboxes
        End Get
    End Property

    ''' <summary> Caselle di posta interoperabili </summary>
    Private ReadOnly Property InteropMailBoxes As IList(Of PECMailBox)
        Get
            If _interopMailBoxes Is Nothing Then
                _interopMailBoxes = New List(Of PECMailBox)
                For Each mailbox As PECMailBox In Facade.PECMailboxFacade.GetVisibleSendingMailBoxes()
                    If Facade.PECMailboxFacade.IsRealPecMailBox(mailbox) AndAlso mailbox.IsForInterop Then
                        _interopMailBoxes.Add(mailbox)
                    End If
                Next
            End If
            Return _interopMailBoxes
        End Get
    End Property

    Private ReadOnly Property OriginalMailToReply As PECMail
        Get
            If _originalMail Is Nothing Then
                _originalMail = CurrentPecMail
            End If
            Return _originalMail
        End Get
    End Property

    Private ReadOnly Property SimpleMode As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("SimpleMode", False)
        End Get
    End Property

    Protected ReadOnly Property RedirectTo As String
        Get
            Return Request.QueryString.GetValueOrDefault("RedirectTo", PreviousPageUrl)
        End Get
    End Property

    ''' <summary>Indica se attivare la modalità "Inoltra PEC"</summary>
    Private ReadOnly Property ForwardPecMode As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("ForwardPECMode", False)
        End Get
    End Property

    Private ReadOnly Property ReplyAll As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("ReplyAll", False)
        End Get
    End Property

    ''' <summary>Mailbox di invio</summary>
    Private Property SelectedMailboxId As Short?
        Get
            If Not _selectedMailBoxId.HasValue Then
                _selectedMailBoxId = Request.QueryString.GetValueOrDefault(Of Short?)("SelectedMailboxId", Nothing)
                'Provo a caricare la casella PEC dalla sessione
                If Not _selectedMailBoxId.HasValue Then
                    _selectedMailBoxId = CommonShared.SelectedPecMailBoxId
                End If
            End If
            Return _selectedMailBoxId
        End Get
        Set(value As Short?)
            _selectedMailBoxId = value
        End Set
    End Property

    Private ReadOnly Property CurrentMailBox As PECMailBox
        Get
            If _currentMailBox Is Nothing Then
                If SelectedMailboxId.HasValue Then
                    _currentMailBox = Facade.PECMailboxFacade.GetById(SelectedMailboxId.Value)
                End If
                If _currentMailBox Is Nothing AndAlso ddlMailFrom.SelectedItem IsNot Nothing AndAlso Not String.IsNullOrEmpty(ddlMailFrom.SelectedItem.Value) Then
                    _currentMailBox = Facade.PECMailboxFacade.GetById(Short.Parse(ddlMailFrom.SelectedItem.Value))
                End If
            End If
            Return _currentMailBox
        End Get
    End Property

    Private ReadOnly Property CurrentAttachments As IList(Of DocumentInfo)
        Get
            Dim tor As New List(Of DocumentInfo)

            '' Se previsto l'utilizzo carico i documenti dalla griglia
            If ProtocolEnv.EnablePecAttachmentListFromProtocol Then
                tor.AddRange(uscAttachmentList.GetSelectedDocuments())
            End If

            '' aggiungo poi i documenti standard
            tor.AddRange(uscAttachment.DocumentInfos)
            Return tor
        End Get
    End Property

    Public Property CurrentSignature As String
        Get
            If ViewState("Segnatura") IsNot Nothing Then
                Return ViewState("Segnatura").ToString()
            End If

            Return String.Empty
        End Get
        Set(ByVal value As String)
            ViewState("Segnatura") = value
        End Set
    End Property

    Public Property ValidationResult As SegnaturaValidationResult
        Get
            If ViewState("ValidationResult") IsNot Nothing Then
                Return CType(ViewState("ValidationResult"), SegnaturaValidationResult)
            End If

            Return New SegnaturaValidationResult()
        End Get
        Set(ByVal value As SegnaturaValidationResult)
            ViewState("ValidationResult") = value
        End Set
    End Property

    ''' <summary>Mittenti</summary>
    Public Property Senders As List(Of ContactDTO)
        Get
            If ViewState("Senders") Is Nothing Then
                ViewState("Senders") = New List(Of ContactDTO)
            End If

            Return DirectCast(ViewState("Senders"), List(Of ContactDTO))
        End Get
        Set(ByVal value As List(Of ContactDTO))
            ViewState("Senders") = value
        End Set
    End Property

    ''' <summary>Destinatari</summary>
    Public Property Recipients As List(Of ContactDTO)
        Get
            If ViewState("Recipients") Is Nothing Then
                ViewState("Recipients") = New List(Of ContactDTO)
            End If

            Return DirectCast(ViewState("Recipients"), List(Of ContactDTO))
        End Get
        Set(ByVal value As List(Of ContactDTO))
            ViewState("Recipients") = value
        End Set
    End Property

    Private ReadOnly Property IsAttachmentListEnabled As Boolean
        Get
            Return (CurrentProtocol IsNot Nothing AndAlso ProtocolEnv.EnablePecAttachmentListFromProtocol) _
                        OrElse (CurrentUDS IsNot Nothing) _
                        OrElse (CurrentPecMailList IsNot Nothing AndAlso ForwardPecMode AndAlso ProtocolEnv.EnablePecAttachmentListFromPec) _
                        OrElse (SendFromViewer AndAlso ProtocolEnv.EnablePecAttachmentListFromProtocol) _
                        OrElse (OriginalMailToReply IsNot Nothing AndAlso Not ForwardPecMode AndAlso chkAddOriginalAttachments.Checked)
        End Get
    End Property

    Protected ReadOnly Property SignalRServerAddress As String
        Get
            If String.IsNullOrEmpty(_signalRServerAddress) Then
                _signalRServerAddress = DocSuiteContext.SignalRServerAddress
            End If
            Return _signalRServerAddress
        End Get
    End Property

    Private ReadOnly Property SendFromViewer As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("FromViewer", False)
        End Get
    End Property

    Private ReadOnly Property RedirectToProtocolSummary As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("RedirectToProtocolSummary", False)
        End Get
    End Property

    Public ReadOnly Property PasswordGenerator As PasswordGenerator
        Get
            If _passwordGenerator Is Nothing Then
                _passwordGenerator = New PasswordGenerator()
            End If
            Return _passwordGenerator
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If ddlMailFrom.SelectedItem IsNot Nothing AndAlso Not String.IsNullOrEmpty(ddlMailFrom.SelectedItem.Value) Then
            SelectedMailboxId = Short.Parse(ddlMailFrom.SelectedItem.Value)
        End If

        InitializeAjax()

        uscDestinatari.SimpleMode = SimpleMode
        uscDestinatari.ButtonIPAVisible = ProtocolEnv.IsIPAAUSEnabled
        If ProtocolEnv.SelectCheckRecipientEnabled Then
            uscDestinatari.EnableCheck = True
        End If
        uscAttachment.AllowZipDocument = ProtocolEnv.ZipUploadEnabled

        If ProtocolEnv.InsertDefaultBodyButtonEnabled Then
            cmdInsertSign.Visible = True
        End If

        If Not ProtocolEnv.EnableCc Then
            tblCc.Visible = False
            uscDestinatariCc.Visible = False
        Else
            uscDestinatariCc.SimpleMode = SimpleMode
            uscDestinatariCc.ButtonIPAVisible = ProtocolEnv.IsIPAAUSEnabled
        End If

        uscAttachmentList.DocumentSelectionEnabled = ProtocolEnv.SendPECDocumentEnabled

        If Not IsPostBack Then
            SetResponseNoCache()
            chkMultiPec.Text = String.Format("Crea una {0} per ogni destinatario", PecLabel.ToLower)
            PreviousPageUrl = Request.UrlReferrer.ToString()

            If SuperAdminAuthored Then
                txtMailBody.EditModes = EditModes.All
            End If

            chkAddOriginalAttachments.Visible = False
            txtMailSubject.Style.Add("padding-left", "3px")
            chkSetPecInteroperable.Visible = False
            chkMultiPec.Enabled = uscDestinatari.GetContacts(False).Count > 0
            ' Carico le mailbox e seleziono se possibile quella in querystring
            LoadMailboxes()
            UpdatePECMailBoxInputColor()

            Dim previous As ISendMail = Nothing
            If SendFromViewer Then
                previous = TryCast(PreviousPage, ISendMail)
                If previous IsNot Nothing Then
                    uscAttachmentList.Caption = "Documenti del protocollo"
                    uscAttachmentList.LoadDocumentInfos(previous.Documents)
                    txtMailSubject.Text = previous.Subject
                    txtMailBody.Content = previous.Body
                    uscAttachmentList.DataBind()
                End If
            End If

            If Not SendFromViewer OrElse previous Is Nothing Then
                Select Case True
                    Case CurrentProtocol IsNot Nothing
                        uscAttachmentList.Caption = "Documenti del protocollo"
                        ' Se presente un protocollo
                        Title = String.Format("{0} - Invia Protocollo {1}", PecLabel, CurrentProtocol.FullNumber)
                        chkSetPecInteroperable.Visible = True
                        InitializeFromProtocol(CurrentProtocol)
                    Case CurrentUDS IsNot Nothing
                        uscAttachmentList.Caption = String.Format("Documenti di {0}", CurrentUDS.UDSModel.Model.Title)
                        ' Se presente un protocollo
                        Title = String.Format("{0} - Invia {1} {2}/{3:0000000}", PecLabel, CurrentUDS.UDSModel.Model.Title, CurrentUDS.Year, CurrentUDS.Number)
                        chkSetPecInteroperable.Visible = False
                        InitializeFromUDS(CurrentUDS.UDSModel)
                    Case OriginalMailToReply IsNot Nothing AndAlso Not ForwardPecMode
                        ' Se voglio fare una risposta
                        Title = String.Format("{0} - Rispondi", PecLabel)
                        GenerateAnswer(OriginalMailToReply)
                    Case CurrentPecMailList IsNot Nothing AndAlso ForwardPecMode
                        ' Se voglio fare un inoltro
                        Title = String.Format("{0} - Inoltra", PecLabel)

                        uscAttachmentList.Caption = "Allegati della PEC di origine"
                        GenerateForward(CurrentPecMailList)
                    Case Else
                        Title = String.Format("{0} - Inserimento", PecLabel)
                End Select
            End If

            uscAttachment.ButtonCopyProtocol.Visible = ProtocolEnv.CopyProtocolDocumentsEnabled
            uscAttachment.ButtonCopySeries.Visible = ProtocolEnv.CopyFromSeries
            uscAttachment.ButtonCopyUDS.Visible = ProtocolEnv.UDSEnabled
            If (DocSuiteContext.Current.IsResolutionEnabled) Then
                uscAttachment.ButtonCopyResl.Visible = ResolutionEnv.CopyReslDocumentsEnabled
            End If

            If ProtocolEnv.ShowPECManualMultiVisibleButton Then
                uscDestinatari.ButtonManualMultiVisible = ProtocolEnv.ShowPECManualMultiVisibleButton
            End If

            uscAttachmentList.Visible = IsAttachmentListEnabled
            uscAttachmentList.ShowTotalSize = ProtocolEnv.PECInsertDocumentSizeEvaluationEnabled
            uscAttachment.ShowTotalSize = ProtocolEnv.PECInsertDocumentSizeEvaluationEnabled
            chkZip.Visible = ProtocolEnv.PECAttachmentsZipEnabled
            If chkZip.Visible Then
                chkZip.Enabled = uscAttachmentList.DocumentsCount > 0 OrElse uscAttachment.DocumentsAddedCount > 0
            End If
            chkZipPassword.Visible = ProtocolEnv.PECPasswordZipEnabled
            If chkZipPassword.Visible Then
                chkZipPassword.Enabled = uscAttachmentList.DocumentsCount > 0 OrElse uscAttachment.DocumentsAddedCount > 0
            End If

            If Not ProtocolEnv.PECInsertDocumentSizeEvaluationEnabled Then
                Dim pecSize As Long = uscAttachment.TotalSize
                If ProtocolEnv.EnablePecAttachmentListFromProtocol Then
                    pecSize += uscAttachmentList.TotalSize
                End If
                uscAttachmentList.SetDocument(String.Concat("La dimensione del messaggio (massima di ", pecSize.ToByteFormattedString(0), ") può essere influenzata dalla generazione delle copie pdf."))
            End If

            If ProtocolEnv.PECMailInsertAuthorizationEnabled AndAlso (Not Facade.DomainUserFacade.HasCurrentRight(CurrentDomainUser, DSWEnvironmentType.Protocol, DomainUserFacade.HasPECSendableRight) OrElse ddlMailFrom.Items.Count() = 0) Then
                cmdSend.Enabled = False
                cmdSend.ToolTip = "Non autorizzato a eseguire questa azione"
                cmdInsertSign.Enabled = False
                cmdInsertSign.ToolTip = "Non autorizzato a eseguire questa azione"
            End If
        End If
    End Sub

    Private Sub UpdatePECMailBoxInputColor()
        Dim selectedItem As RadComboBoxItem = ddlMailFrom.SelectedItem

        If selectedItem.ForeColor = Drawing.Color.Red Then
            ddlMailFrom.InputCssClass = "text-red"
        End If
    End Sub

    Protected Sub ManagerAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arg As String = e.Argument.Replace("~", "'")
        Dim arguments As Object() = arg.Split("|"c)
        If arguments.Length = 0 Then
            Exit Sub
        End If

        Select Case arguments(0).ToString()
            Case UpdatePecAddressCommand
                SendPecFunction(String.Empty)
            Case SendWithStandardMailCommand, DeleteExceedingAttachmentsCommand, SplitAttachmentsCommand
                SendPecFunction(e.Argument)
            Case SendPecCommand
                Dim pecId As Integer = -1
                If arguments.Length > 1 AndAlso Integer.TryParse(arguments(1).ToString(), pecId) AndAlso pecId > 0 Then
                    Try
                        Dim pec As PECMail = Facade.PECMailFacade.GetById(pecId)
                        If pec IsNot Nothing Then
                            SendPec(pec)
                            RedirectToSource()
                        End If
                    Catch ex As Exception
                        FileLogger.Error(LoggerName, ex.Message, ex)
                        Throw ex
                    End Try
                End If
        End Select
    End Sub

    Private Sub CmdSendClick(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSend.Click
        SendPecFunction(String.Empty)
    End Sub

    Private Sub cmdInsertSignClick(ByVal sender As Object, ByVal e As EventArgs) Handles cmdInsertSign.Click
        AppendSignToContent()
    End Sub

    Private Sub uscDestinari_contactsAdded(ByVal sender As Object, ByVal e As EventArgs) Handles uscDestinatari.ContactAdded, uscDestinatari.ContactRemoved, uscDestinatari.ManualContactAdded, uscDestinatari.ImportExcelContactAdded
        Dim contacts As IList(Of ContactDTO) = uscDestinatari.GetContacts(True)
        chkMultiPec.Enabled = contacts.Count > 1
    End Sub


    Private Sub uscAttachment_documentsAdded(ByVal sender As Object, ByVal e As EventArgs) Handles uscAttachment.DocumentUploaded, uscAttachment.DocumentRemoved
        chkZip.Enabled = ProtocolEnv.PECAttachmentsZipEnabled AndAlso uscAttachment.DocumentsAddedCount > 0
        chkZipPassword.Enabled = uscAttachment.DocumentsAddedCount > 0
    End Sub


    Private Sub ChkSetPecInteroperableCheckedChanged(sender As Object, e As EventArgs) Handles chkSetPecInteroperable.CheckedChanged
        SetInteropMailboxes()
    End Sub

    Private Sub ddlMailbox_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlMailFrom.SelectedIndexChanged
        If CurrentMailBox IsNot Nothing AndAlso CurrentMailBox.LoginError Then
            AjaxAlert("La casella PEC ha un problema di configurazione. Avvisare il responsabile per la corretta configurazione")
        End If

        ddlMailFrom.InputCssClass = If(CurrentMailBox IsNot Nothing AndAlso CurrentMailBox.LoginError, "text-red", "text-black")

        Dim selectedId As Short
        If Short.TryParse(ddlMailFrom.SelectedValue, selectedId) Then
            CommonShared.SelectedPecMailBoxId = selectedId
        Else
            CommonShared.SelectedPecMailBoxId = Nothing
        End If
    End Sub

    Private Sub chkAddOriginalAttachments_CheckedChanged(sender As Object, e As EventArgs) Handles chkAddOriginalAttachments.CheckedChanged
        If ProtocolEnv.EnablePecAttachmentListFromProtocol Then
            uscAttachmentList.Visible = chkAddOriginalAttachments.Checked
            If chkAddOriginalAttachments.Checked AndAlso OriginalMailToReply IsNot Nothing Then
                uscAttachmentList.Caption = "Allegati della PEC di origine"
                uscAttachmentList.UncheckAll = True
                uscAttachmentList.LoadDocumentInfos(OriginalMailToReply.Attachments.ToDocumentInfoList(AddressOf DelegateElsaInitialize))
                ' Carico i documenti
                uscAttachmentList.DataBind()
            End If
        Else
            ' Documenti in ORIGINALE
            Dim docs As IList(Of DocumentInfo) = OriginalMailToReply.Attachments.ToDocumentInfoList(AddressOf DelegateElsaInitialize)
            uscAttachment.LoadDocumentInfo(docs, False, True, True, True, True)
            ' Documenti in Copia conforme
            Dim attachsPdf As List(Of BiblosPdfDocumentInfo) = (From att In docs Select New BiblosPdfDocumentInfo(CType(att, BiblosDocumentInfo))).ToList()
            uscAttachment.LoadDocumentInfo(attachsPdf.Cast(Of DocumentInfo).ToList(), False, True, True, True, True)
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeContacts(protocol As Protocol)
        For Each item As ProtocolContact In protocol.Contacts
            If item.ComunicationType.Eq(ProtocolContactCommunicationType.Sender) Then
                Senders.Add(New ContactDTO(item.Contact, ContactDTO.ContactType.Address))
            ElseIf item.ComunicationType.Eq(ProtocolContactCommunicationType.Recipient) Then
                ' FG20140519: Verifico sia configurato il nodo radice di rubrica interna
                ' - E - l'esclusione dai destinatari per l'inoltro dei nodi di rubrica interna.
                If DocSuiteContext.Current.ProtocolEnv.ExcludeInnerContact AndAlso
                    DocSuiteContext.Current.ProtocolEnv.InnerContactRoot.HasValue AndAlso
                    item.Contact.FullIncrementalPath.Split("|"c).Contains(DocSuiteContext.Current.ProtocolEnv.InnerContactRoot.Value.ToString()) Then
                    ' Se è nodo figlio di rubrica interno lo ignoro.
                    Continue For
                End If
                Recipients.Add(New ContactDTO(item.Contact, ContactDTO.ContactType.Address))
            End If
        Next

        For Each item As ProtocolContactManual In protocol.ManualContacts
            If item.ComunicationType.Eq(ProtocolContactCommunicationType.Sender) Then
                Senders.Add(New ContactDTO(item.Contact, item.Id))
            ElseIf item.ComunicationType.Eq(ProtocolContactCommunicationType.Recipient) Then
                Recipients.Add(New ContactDTO(item.Contact, item.Id))
            End If
        Next
    End Sub

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf ManagerAjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, cmdSend, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscAttachment)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscAttachmentList)
        AjaxManager.AjaxSettings.AddAjaxSetting(chkSetPecInteroperable, pnlContent)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscDestinatari, uscDestinatari)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscDestinatari, uscDestinatari.TreeViewControl)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdSend, pnlContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdSend, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(chkAddOriginalAttachments, uscAttachmentList)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdInsertSign, txtMailBody, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(chkZip, pnlContent)
        AjaxManager.AjaxSettings.AddAjaxSetting(chkZipPassword, pnlContent)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscDestinatari, chkMultiPec)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscAttachment, chkZipPassword)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscAttachment, chkZipPassword)


        If ProtocolEnv.EnableCc Then
            AjaxManager.AjaxSettings.AddAjaxSetting(uscDestinatariCc, uscDestinatariCc)
            AjaxManager.AjaxSettings.AddAjaxSetting(uscDestinatariCc, uscDestinatariCc.TreeViewControl)
        End If
    End Sub

    Private Sub LoadMailboxes()
        Dim mailboxId As Short? = Nothing
        Dim mailBoxIdToSelect As String = String.Empty
        Dim editableMailbox As Boolean = True

        If CurrentProtocol Is Nothing Then
            'Non si tratta di un invio da protocollo
            If OriginalMailToReply Is Nothing Then
                ' Invio singolo
                ' Abilito caricamento e modifica completi, seleziono di default 
                editableMailbox = True

                Dim defaultMailbox As PECMailBox = FacadeFactory.Instance.PECMailboxFacade.GetDefault(MailBoxes)
                If defaultMailbox IsNot Nothing Then
                    mailBoxIdToSelect = defaultMailbox.Id.ToString()
                End If

                If SelectedMailboxId.HasValue Then
                    mailBoxIdToSelect = SelectedMailboxId.Value.ToString
                End If
            ElseIf ForwardPecMode Then
                ' Invio come inoltro
                ' Abilito il caricamento di tutte le mailbox con diritti e lascio la modifica attiva
                editableMailbox = True
                ' Seleziono la mailbox del destinatario della mail originale
                If SelectedMailboxId.HasValue Then
                    mailBoxIdToSelect = SelectedMailboxId.Value.ToString()
                ElseIf OriginalMailToReply.MailBox IsNot Nothing Then
                    mailBoxIdToSelect = OriginalMailToReply.MailBox.Id.ToString()
                End If
            ElseIf ProtocolEnv.ReplyFromOriginalBox Then
                ' Invio come risposta 
                ' Abilito solo la PEC che ha ricevuto la mail originaria (se compatibile con i diritti) e blocco la modifica
                mailboxId = FacadeFactory.Instance.PECMailFacade.GetOriginalMailboxId(OriginalMailToReply)
                editableMailbox = False
            ElseIf OriginalMailToReply IsNot Nothing AndAlso Not ForwardPecMode Then
                'da rispondi pec
                Dim originalPecMailBox As PECMailBox = FacadeFactory.Instance.PECMailboxFacade.GetByRecipient(OriginalMailToReply.OriginalRecipient)
                If originalPecMailBox IsNot Nothing Then
                    mailBoxIdToSelect = originalPecMailBox.Id.ToString()
                Else
                    ' Imposto allora la casella di Default
                    Dim defaultMailbox As PECMailBox = FacadeFactory.Instance.PECMailboxFacade.GetDefault(MailBoxes)
                    If defaultMailbox IsNot Nothing Then
                        mailBoxIdToSelect = defaultMailbox.Id.ToString()
                    End If
                End If

            End If
        Else
            ' Invio da protocollo
            ' Consento le mailbox interoperabili in quanto potrei utilizzare una pec interoperabile per invio normale
            editableMailbox = True

            ' Imposto allora la casella di Default
            Dim defaultMailbox As PECMailBox = FacadeFactory.Instance.PECMailboxFacade.GetDefault(MailBoxes)
            If defaultMailbox IsNot Nothing Then
                mailBoxIdToSelect = defaultMailbox.Id.ToString()
            End If
        End If

        ' Effettuo la ricerca
        If mailboxId.HasValue Then
            '' Se è definita una specifica casella allora la uso
            ddlMailFrom.Items.Clear()

            ' Devo impostare una singola casella
            Dim required As PECMailBox = FacadeFactory.Instance.PECMailboxFacade.GetById(mailboxId.Value)

            ' Qui mancava un Not!
            ' Se l'id cercato non è presente tra quelli autorizzati
            ' allora significa che mancano i diritti
            If Not MailBoxes.Any(Function(m) m.Id.Equals(required.Id)) Then
                Dim label As String = FacadeFactory.Instance.PECMailboxFacade.MailBoxRecipientLabel(required)
                ddlMailFrom.ToolTip = String.Format("L'utente corrente non può rispondere in quanto non ha i diritti sulla casella {0}", label)
            Else
                ' La casella e valida e la posso inserire
                Dim mailbox As PECMailBox = MailBoxes.First(Function(m) m.Id.Equals(required.Id))
                ddlMailFrom.Items.Add(CreateRadComboboxItem(mailbox))
            End If
        ElseIf ProtocolBoxEnabled Then
            If Not MailBoxes.IsNullOrEmpty() Then
                ddlMailFrom.Items.Clear()

                For Each item As PECMailBox In AllMailBoxes
                    Dim label As String = FacadeFactory.Instance.PECMailboxFacade.MailBoxRecipientLabel(item)
                    ddlMailFrom.Items.Add(CreateRadComboboxItem(item))
                Next
            End If

        Else
            '' Altrimenti le carico tutte (quelle autorizzate)
            If Not MailBoxes.IsNullOrEmpty() Then
                ddlMailFrom.Items.Clear()

                For Each item As PECMailBox In MailBoxes
                    Dim label As String = FacadeFactory.Instance.PECMailboxFacade.MailBoxRecipientLabel(item)
                    ddlMailFrom.Items.Add(CreateRadComboboxItem(item))
                Next
            End If
        End If

        ' Disabilito la modifica se necessario
        ddlMailFrom.Enabled = editableMailbox
        If Not String.IsNullOrEmpty(mailBoxIdToSelect) Then
            ' Devo selezionare una mailbox
            If ddlMailFrom.Items.FindItemByValue(mailBoxIdToSelect) IsNot Nothing Then
                ddlMailFrom.SelectedValue = mailBoxIdToSelect
            Else
                ' in caso di errori permetto la modifica
                ddlMailFrom.Enabled = True
            End If
        End If

        If DocSuiteContext.Current.ProtocolEnv.PECMailboxSelectEnabled AndAlso ddlMailFrom.Items.Count() > 1 Then
            ddlMailFrom.Items.Insert(0, New RadComboBoxItem(String.Empty, String.Empty))
            ddlMailFrom.SelectedIndex = 0
        End If

        If CurrentMailBox IsNot Nothing AndAlso CurrentMailBox.LoginError Then
            AjaxAlert("La casella PEC ha un problema di configurazione. Avvisare il responsabile per la corretta configurazione")
        End If

        ' Attivo il bottone di invio solo se ci sono caselle effettivamente utilizzabili
        cmdSend.Enabled = ddlMailFrom.Items.Count > 0
    End Sub


    Private Sub AppendSignToContent()
        If txtMailBody.Content.Contains(ProtocolEnv.DefaultPECBodyContent) Then
            Exit Sub
        End If

        Dim isHtml As Boolean = txtMailBody.Content.Contains("</body>")
        Dim currentEditorContent As String = txtMailBody.Content
        Dim contentWithSignature As String = String.Empty
        If isHtml Then
            contentWithSignature = currentEditorContent.Replace("</body>", $"<br /><br />{ProtocolEnv.DefaultPECBodyContent}</body>")
        Else
            contentWithSignature = $"{currentEditorContent}<br /><br />{ProtocolEnv.DefaultPECBodyContent}"
        End If

        txtMailBody.Content = contentWithSignature
    End Sub

    Private Sub SetInteropMailboxes()
        ' caricamento dropdown
        ddlMailFrom.Items.Clear()

        ' Carico le mailbox solo se disponibili
        If Not chkSetPecInteroperable.Checked Then
            ' Ho rimosso il check e ripristino status base
            LoadMailboxes()
            Exit Sub
        End If

        If CurrentProtocolRights.IsInteroperable Then
            ' Il protocollo può essere spedito come interoperabile
            For Each pecMailBox As PECMailBox In InteropMailBoxes
                ddlMailFrom.Items.Add(CreateRadComboboxItem(pecMailBox))
            Next
            'Attivo il bottone di invio solo se ci sono caselle effettivamente utilizzabili
            cmdSend.Enabled = ddlMailFrom.Items.Count > 0
        Else
            ' se non ho diritto blocco tutto
            cmdSend.Enabled = False
        End If
    End Sub

    Private Sub GenerateAnswer(ByVal pecToAnswer As PECMail)
        txtMailBody.Content = GetAnswerTextContent(pecToAnswer)

        ' Impostazione per allegati
        uscAttachment.SignButtonEnabled = False

        ' Evidenzio nell'oggetto che si tratta di una risposta
        txtMailSubject.Text = String.Concat("RE: ", StringHelper.Clean(StringHelper.ReplaceCrLf(pecToAnswer.MailSubject), DocSuiteContext.Current.ProtocolEnv.RegexOnPasteString))
        ' E' possibile selezionare solo i mittenti
        uscDestinatari.ReadOnly = False
        If Not ProtocolEnv.AddPECRecipientsEnabled Then
            uscDestinatari.ReadOnly = True
        End If

        ' Permetto l'aggiunta dell'allegato
        chkAddOriginalAttachments.Visible = True

        Dim sourceRecipients As String = ""
        If RegexHelper.IsValidEmail(pecToAnswer.MailSenders) Then
            sourceRecipients = RegexHelper.MatchEmail(pecToAnswer.MailSenders)
        End If

        If String.IsNullOrEmpty(sourceRecipients) Then
            AjaxAlert("Errore in recupero indirizzo mittente: " & pecToAnswer.MailSenders)
            Exit Sub
        End If

        If ReplyAll Then
            If ProtocolEnv.EnableCc Then
                sourceRecipients = String.Concat(sourceRecipients, ",", pecToAnswer.MailRecipients, ",", pecToAnswer.MailRecipientsCc)
            Else
                sourceRecipients = String.Concat(sourceRecipients, ",", pecToAnswer.MailRecipients)
            End If
        End If

        sourceRecipients = sourceRecipients.Replace(",", ";")
        Dim availableRecipients As String() = sourceRecipients.Split(";"c)


        For Each emailAddress As String In From emailAddress1 In availableRecipients Where Not String.IsNullOrEmpty(emailAddress1)
            Dim dto As New ContactDTO
            dto.Contact = New Contact
            dto.Contact.ContactType = New Data.ContactType(Data.ContactType.Person)
            dto.Contact.Description = emailAddress
            dto.Contact.EmailAddress = emailAddress

            uscDestinatari.DataSource.Add(dto)
        Next
        uscDestinatari.DataBind()
    End Sub

    Private Function GetAnswerTextContent(pecToAnswer As PECMail) As String
        Dim pecWrapper As BiblosPecMailWrapper = New BiblosPecMailWrapper(pecToAnswer, False)
        Dim mailContent As DocumentInfo = pecWrapper.PostaCert
        If mailContent Is Nothing Then
            mailContent = pecWrapper.MailContent
        End If
        If mailContent Is Nothing Then
            FileLogger.Warn(LoggerName, String.Concat("Nessun mail content trovato per la PEC ", pecToAnswer.Id))
            Return String.Empty
        End If

        Dim replyMailBuilder As LimilabsReplyMailBuilder = New LimilabsReplyMailBuilder(mailContent.Stream, mailContent.Name)
        replyMailBuilder.AddSendedDate(pecToAnswer.MailDate)
        Dim toReplyMail As IMail = replyMailBuilder.BuildReplyMail()

        Dim replyMailContent As String = toReplyMail.GetBodyAsText()
        If toReplyMail.IsHtml Then
            replyMailContent = toReplyMail.GetBodyAsHtml()
        End If
        Return replyMailContent
    End Function

    Private Sub GenerateForward(ByVal pecsToForward As IList(Of PECMail))
        ' Evidenzio nell'oggetto che si tratta di un inoltro
        If pecsToForward.HasSingle Then
            txtMailSubject.Text = String.Concat("I: ", StringHelper.Clean(StringHelper.ReplaceCrLf(pecsToForward.First().MailSubject), DocSuiteContext.Current.ProtocolEnv.RegexOnPasteString))
        End If
        ' Impostazione per allegati
        uscAttachment.SignButtonEnabled = False
        ' Permetto di caricare il destinatario desiderato
        uscDestinatari.ReadOnly = False

        Try
            For Each pec As PECMail In pecsToForward
                ' Aggiungo la mail originale come allegato.
                Dim original As DocumentInfo = FacadeFactory.Instance.PECMailFacade.GetOriginalEml(pec, New Action(Sub() DelegateElsaInitialize(pec)))
                If original IsNot Nothing Then
                    original.Name = EmlFileNameFormat(pec.MailSubject)
                    uscAttachment.LoadDocumentInfo(original, False, True, True, True, True)
                End If

                If ProtocolEnv.EnablePecAttachmentListFromPec Then
                    '' Aggiungo gli allegati della PEC originaria
                    uscAttachmentList.UncheckAll = True
                    uscAttachmentList.LoadDocumentInfos(pec.Attachments.ToDocumentInfoList(AddressOf DelegateElsaInitialize))
                    ' Carico i documenti
                    uscAttachmentList.DataBind()
                End If
            Next
        Catch ex As Exception
            Throw New DocSuiteException("Inizializza Forward PEC Mail", "Si è verificato un errore in fase di generazione dell'allegato", ex)
        End Try
    End Sub

    Private Sub InitializeWorkflowWizard()
        Dim insertPECStep As RadWizardStep = New RadWizardStep()
        insertPECStep.ID = "InsertPEC"
        insertPECStep.Title = "Invia una nuova PEC"
        insertPECStep.ToolTip = "Invia una nuova PEC"
        insertPECStep.Active = True
        MasterDocSuite.WorkflowWizardControl.WizardSteps.Add(insertPECStep)

        Dim sendCompleteStep As RadWizardStep = New RadWizardStep()
        sendCompleteStep.ID = "SendComplete"
        sendCompleteStep.Title = "Concludi attività"
        sendCompleteStep.ToolTip = "Concludi attività"
        sendCompleteStep.Enabled = False
        MasterDocSuite.WorkflowWizardControl.WizardSteps.Add(sendCompleteStep)
    End Sub

    Private Sub InitializeFromUDS(uds As UDSModel)
        If uds Is Nothing Then
            Exit Sub
        End If

        txtMailSubject.Text = uds.Model.Subject.Value
        If uds.Model.Contacts IsNot Nothing Then
            For Each contact As Contacts In uds.Model.Contacts
                If contact.ContactInstances IsNot Nothing Then
                    For Each instance As ContactInstance In contact.ContactInstances
                        Dim dto As ContactDTO = New ContactDTO()
                        dto.Id = Int32.Parse(instance.IdContact)
                        dto.Contact = Facade.ContactFacade.GetById(dto.Id)
                        dto.Type = ContactDTO.ContactType.Address
                        uscDestinatari.DataSource.Add(dto)
                    Next
                End If
                If contact.ContactManualInstances IsNot Nothing Then
                    For Each manualInstance As ContactManualInstance In contact.ContactManualInstances
                        Dim dto As ContactDTO = New ContactDTO()
                        dto = JsonConvert.DeserializeObject(Of ContactDTO)(manualInstance.ContactDescription)
                        uscDestinatari.DataSource.Add(dto)
                    Next
                End If
            Next
            uscDestinatari.DataBind()
        End If

        If uds.Model.Documents IsNot Nothing Then
            SetDocumentControl(uscAttachmentList, uds.Model.Documents.Document)
            SetDocumentControl(uscAttachmentList, uds.Model.Documents.DocumentAttachment)
            SetDocumentControl(uscAttachmentList, uds.Model.Documents.DocumentAnnexed)
            uscAttachmentList.DataBind()
        End If

        chkSetPecInteroperable.Style.Add("visible", "none")
    End Sub

    Private Sub InitializeFromProtocol(prot As Protocol)
        If prot Is Nothing Then
            Exit Sub
        End If

        If IsWorkflowOperation Then
            MasterDocSuite.WorkflowWizardRow.Visible = True
            InitializeWorkflowWizard()
        End If
        If DocSuiteContext.Current.ProtocolEnv.CustomPECMailSubjectEnabled Then
            txtMailSubject.Text = String.Format(DocSuiteContext.Current.ProtocolEnv.CustomPECMailSubjectFormat, prot.Year, prot.Number, prot.RegistrationDate, prot.ProtocolObject)
        Else
            txtMailSubject.Text = prot.ProtocolObject
        End If

        ' Calcolo i destinatari
        'Carico i contatti direttamente dal protocollo
        If DocSuiteContext.Current.ProtocolEnv.ExcludeInnerContact Then
            InitializeContacts(prot)
        Else
            prot.GetContacts(Senders, Recipients)
            prot.GetManualContacts(Senders, Recipients)
        End If

        If Recipients.Count > 0 Then
            For Each contact As ContactDTO In Recipients
                uscDestinatari.DataSource.Add(contact)
            Next
            uscDestinatari.DataBind()
        End If

        ' Calcolo degli allegati della PEC
        ' Documento principale
        Dim doc As BiblosDocumentInfo = ProtocolFacade.GetDocumentWithExternalKey(prot)
        ' Allegati
        Dim attachs As IList(Of BiblosDocumentInfo) = ProtocolFacade.GetAttachmentsWithExternalKey(prot)
        ' Annessi
        Dim annexes As IList(Of BiblosDocumentInfo) = ProtocolFacade.GetAnnexesWithExternalKey(prot)

        '' In base al tipo di gestione carico un pannello piuttosto che l'altro
        If ProtocolEnv.EnablePecAttachmentListFromProtocol Then
            '' Se utilizzo il pannello, ci carico i documenti in alternativa al modulo di upload standard
            ' Documento principale
            uscAttachmentList.LoadDocumentInfo(doc,
                                                Sub(options)
                                                    options.Selected = Not ProtocolEnv.ProtocolSendPecChainTypesExcluded.Any(Function(x) x = ChainType.MainChain)
                                                End Sub)
            ' Allegati
            uscAttachmentList.LoadDocumentInfos(CType(attachs, IList(Of DocumentInfo)),
                                                Sub(options)
                                                    options.Selected = Not ProtocolEnv.ProtocolSendPecChainTypesExcluded.Any(Function(x) x = ChainType.AttachmentsChain)
                                                End Sub)
            ' Annessi
            uscAttachmentList.LoadDocumentInfos(CType(annexes, IList(Of DocumentInfo)),
                                                Sub(options)
                                                    options.Selected = Not ProtocolEnv.ProtocolSendPecChainTypesExcluded.Any(Function(x) x = ChainType.AnnexedChain)
                                                End Sub)
            ' Carico i documenti
            uscAttachmentList.DataBind()
        Else
            '' Altrimenti carico sul modulo di upload standard
            ' Documento principale - ORIGINALE
            uscAttachment.LoadDocumentInfo(doc, False, True, True, True, True)
            ' Documento principale - Copia conforme
            uscAttachment.LoadDocumentInfo(New BiblosPdfDocumentInfo(doc), False, True, True, True, True)
            ' Allegati - ORIGINALE

            Dim results As IEnumerable(Of DocumentInfo) = attachs.OfType(Of DocumentInfo)()
            If results IsNot Nothing AndAlso results.Any() Then
                uscAttachment.LoadDocumentInfo(results.ToList(), False, True, True, True, True)
            End If

            ' Allegati - Copia conforme
            Dim attachsPdf As List(Of BiblosPdfDocumentInfo) = (From att In attachs Select New BiblosPdfDocumentInfo(att)).ToList()
            uscAttachment.LoadDocumentInfo(attachsPdf.Cast(Of DocumentInfo).ToList(), False, True, True, True, True)
            ' Annessi - ORIGINALE
            results = Nothing
            results = annexes.OfType(Of DocumentInfo)()
            If results IsNot Nothing AndAlso results.Any() Then
                uscAttachment.LoadDocumentInfo(results.ToList(), False, True, True, True, True)
            End If
            ' Annessi - Copia conforme
            Dim annexesPdf As List(Of BiblosPdfDocumentInfo) = (From att In annexes Select New BiblosPdfDocumentInfo(att)).ToList()
            uscAttachment.LoadDocumentInfo(annexesPdf.Cast(Of DocumentInfo).ToList(), False, True, True, True, True)
        End If

        'Verifico se il protocollo di riferimento è di tipo Fatture PA
        If ProtocolEnv.ProtocolKindEnabled Then
            If ProtocolEnv.InvoicePAEnabled AndAlso ProtocolEnv.IsInvoiceEnabled Then
                Dim idPA As Boolean = ProtocolKind.IsDefined(GetType(ProtocolKind), prot.IdProtocolKind)
                If idPA Then
                    Dim item As ProtocolKind = CType(prot.IdProtocolKind, ProtocolKind)
                    If item.Equals(ProtocolKind.FatturePA) Then
                        Dim sdi As ContactDTO = ProtocolContactFacade.GetSdiContactDto(CurrentProtocol)
                        If sdi Is Nothing Then
                            BindWarningPanel("Non è possibile spedire la PEC con tipologia Fattura PA. Non è presente un indirizzo valido per il Sistema di Interscambio. Verificare il parametro InvoicePAContactSDI")
                        Else
                            sdi.IsLocked = True
                            Recipients.Add(sdi)
                            uscDestinatari.DataSource.Insert(0, sdi)
                            uscDestinatari.DataBind()
                            Exit Sub
                        End If
                    End If
                End If
            End If
        End If

        'Abilito la possibilità di inserire il contatto del Sistema di interscambio se non provengo da protocollo tipo Fatture PA
        If ProtocolEnv.ProtocolKindEnabled Then
            If ProtocolEnv.InvoicePAEnabled AndAlso ProtocolEnv.IsInvoiceEnabled Then
                uscDestinatari.ButtonSdiContactVisible = True
            End If
        End If

        'Verifico se la abilitare l'interoperabilità
        If Not ProtocolEnv.IsInteropEnabled Then
            'Se non è stata attivata l'interoperabilità disattivo tutto il pannello
            chkSetPecInteroperable.Style.Add("visible", "none")
            Exit Sub
        End If

        'Il contenitore del protocollo deve consentire l'invio per PEC interoperabile
        If Not CurrentProtocolRights.IsInteroperable Then
            chkSetPecInteroperable.ToolTip = String.Format("Il protocollo corrente non è abilitato all'utilizzo della funzionalità di interoperabilità. Controllare i diritti sul contenitore ""{0}"".", CurrentProtocol.Container.Name)
            Exit Sub
        End If

        'L'utente deve avere diritto su almeno una PEC interoperabile
        If InteropMailBoxes.Count = 0 Then
            chkSetPecInteroperable.ToolTip = "Non esistono caselle PEC interoperabili disponibili per l'utente corrente."
            Exit Sub
        End If

        'Il protocollo di riferimento deve possedere esattamente 1 mittente
        If Senders.Count <> 1 Then
            ' Se c'è meno di un mittente oppure più di un mittente
            If ProtocolEnv.PecIntDefaultContactSenderId = -1 Then
                chkSetPecInteroperable.Enabled = False
                chkSetPecInteroperable.ToolTip = "Per attivare la funzionalità il mittente deve essere esattamente 1 oppure deve essere definito un contatto di rubrica di default da utilizzare."
                Exit Sub
            Else
                Dim contactSender As Contact = Facade.ContactFacade.GetById(ProtocolEnv.PecIntDefaultContactSenderId)
                If (contactSender IsNot Nothing) Then
                    chkSetPecInteroperable.Text &= String.Format(" ==> VERRA' UTILIZZATO IL CONTATTO {0}", Replace(contactSender.Description, "|", " "))
                Else
                    chkSetPecInteroperable.Enabled = False
                    chkSetPecInteroperable.ToolTip = "Il contatto di default definito non è stato trovato in rubrica."
                    Exit Sub
                End If
            End If
        End If

        ' Se sono arrivato qui significa che i controlli precedenti sono andati a buon fine
        chkSetPecInteroperable.Enabled = True
        chkSetPecInteroperable.Visible = True

        ' Opzione aggiuntiva nel caso in cui non avessi altre mailbox
        If (Not cmdSend.Enabled) Then
            'Significa che non esistono caselle standard da cui inviare
            'potrebbero tuttavia esisterne di interoperabili
            chkSetPecInteroperable.Checked = True
            SetInteropMailboxes()
        End If
    End Sub

    ''' <summary>Crea una stringa di indirizzi email a partire dai contatti selezionati nell'usercontrol.</summary>
    Private Function GetSelectedRecipients(controlToCheck As uscContattiSel) As String
        Dim temp As New List(Of String)()

        For Each c As ContactDTO In controlToCheck.GetContacts(True)
            If String.IsNullOrEmpty(c.Contact.CertifiedMail) AndAlso String.IsNullOrEmpty(c.Contact.EmailAddress) Then
                Throw New DocSuiteException(String.Format("{0} non ha un indirizzo email valido.", c.Contact.Description))
            End If

            '' Imposto preferenzialmente la PEC certificata
            Dim address As String = c.Contact.CertifiedMail

            '' se non è presente allora imposto la mail normale
            If String.IsNullOrEmpty(address) Then
                address = c.Contact.EmailAddress
            End If

            address = RegexHelper.MatchEmail(address)

            If Not RegexHelper.IsValidEmail(address) Then
                Throw New DocSuiteException(String.Format("{0} non ha un indirizzo email valido.", c.Contact.Description))
            End If

            Dim item As String = String.Format("{0} <{1}>", c.Contact.DescriptionFormatByContactType, address)
            temp.Add(item.TrimStart.TrimEnd)
        Next

        Return String.Join("; ", temp)
    End Function

    ''' <summary>Crea la PEC da spedire</summary>
    Private Function CreatePec(mailRecipients As String, mailRecipientsCc As String, multiPecType As PecMailMultipleTypeEnum) As PECMail
        Dim pec As New PECMail()
        pec.Multiple = multiPecType <> PecMailMultipleTypeEnum.NoSplit
        pec.MultipleType = multiPecType

        Select Case True
            Case CurrentProtocol IsNot Nothing
                'Significa che la mail che viene creata proviene da un protocollo
                pec.Year = CurrentProtocol.Year
                pec.Number = CurrentProtocol.Number
                pec.DocumentUnit = Facade.DocumentUnitFacade.GetById(CurrentProtocol.Id)
            Case CurrentUDS IsNot Nothing
                'Significa che la mail che viene creata proviene da una UDS
                pec.Year = Convert.ToInt16(CurrentUDS.Year)
                pec.Number = CurrentUDS.Number
                pec.DocumentUnit = Facade.DocumentUnitFacade.GetById(CurrentUDS.Id)
        End Select

        pec.Direction = PECMailDirection.Outgoing
        pec.MailSubject = If(String.IsNullOrEmpty(txtMailSubject.Text), String.Empty, StringHelper.Clean(StringHelper.ReplaceCrLf(txtMailSubject.Text), DocSuiteContext.Current.ProtocolEnv.RegexOnPasteString))
        pec.MailType = PECMailTypes.Invio
        pec.Segnatura = String.Empty
        pec.MailPriority = Convert.ToInt16(ddlPriority.SelectedValue)
        pec.MailBody = Helpers.StringHelper.EncodingAccentChar(txtMailBody.Text)
        'pec.MailBody = txtMailBody.GetHtml(EditorStripHtmlOptions.None)
        ' Genero la PEC come nascosta per evitare che venga vista dagli utenti e che venga gestita dal JeepService
        pec.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Disabled)
        pec.IsValidForInterop = False
        pec.Attachments = New List(Of PECMailAttachment)

        ' Imposto mittente e mailbox
        Dim selectedMailBox As PECMailBox = CurrentMailBox

        pec.MailSenders = selectedMailBox.MailBoxName
        pec.MailBox = selectedMailBox
        pec.Location = selectedMailBox.Location
        pec.MailRecipients = mailRecipients 'SelectedDestinatari()
        pec.MailRecipientsCc = mailRecipientsCc 'SelectedDestinatariCc()
        Facade.PECMailFacade.Save(pec)

        'Gestione Interoperabilità
        If (chkSetPecInteroperable.Checked) Then
            'Gestione contatto automatico
            Dim sendersList As IList(Of ContactDTO) = CurrentProtocol.GetSenders()
            If sendersList.Count = 0 AndAlso chkSetPecInteroperable.Checked = True AndAlso ProtocolEnv.PecIntDefaultContactSenderId <> -1 Then
                sendersList.Add(New ContactDTO(Facade.ContactFacade.GetById(ProtocolEnv.PecIntDefaultContactSenderId), ContactDTO.ContactType.Address))
            End If

            GenerateSegnatura(sendersList, CurrentProtocol.GetRecipients())
            pec.IsValidForInterop = True
            pec.Segnatura = GetSignatura()
        End If

        '' Salvo gli allegati su DB/Biblos
        If Not StoreAttachments(pec) Then
            Return Nothing
        End If
        Return pec
    End Function

    Private Function StoreAttachments(ByRef pec As PECMail) As Boolean
        'Gestione Errori
        Dim errors As New List(Of String)
        Dim pecDocuments As List(Of DocumentInfo) = New List(Of DocumentInfo)()

        If Not chkZip.Checked AndAlso Not chkZipPassword.Checked Then
            '' Gestione allegati specifica per UD
            If IsAttachmentListEnabled Then
                pecDocuments.AddRange(uscAttachmentList.GetSelectedDocuments())
            End If

            'Gestione allegati PEC standard
            Dim standardAttachments As IList(Of DocumentInfo) = uscAttachment.DocumentInfos
            pecDocuments.AddRange(standardAttachments)
        End If

        If ProtocolEnv.PECAttachmentsZipEnabled AndAlso chkZip.Checked AndAlso Not chkZipPassword.Checked Then
            pecDocuments.AddRange(CompressDocuments())
        End If

        If ProtocolEnv.PECPasswordZipEnabled AndAlso chkZipPassword.Checked AndAlso Not chkZip.Checked Then
            Dim password As String = PasswordGenerator.GenerateAlphanumericPassword(8)
            Dim zipDocs As IList(Of DocumentInfo) = CompressDocuments(password:=password, zipFileName:="File protetti.zip")
            pecDocuments.AddRange(zipDocs)
            Try
                SendPecWithPassword(pec, password)
            Catch ex As Exception
                FileLogger.Error(LoggerName, ex.Message, ex)
                AjaxAlert("Errore durante l'inserimento nella coda di invio del messaggio contenente la password")
                Return False
            End Try
        End If
        ArchiveAttachments(pec, pecDocuments, errors)

        'Aggiorno la PEC
        Facade.PECMailFacade.UpdateNoLastChange(pec)

        'Check errori
        If errors.Count > 0 Then
            Dim attachmentsWarningMessage As New StringBuilder()
            attachmentsWarningMessage.Append("Impossibile gestire i seguenti allegati:")
            attachmentsWarningMessage.Append(Environment.NewLine)
            attachmentsWarningMessage.Append(String.Join(Environment.NewLine, errors))
            attachmentsWarningMessage.Append(Environment.NewLine)
            attachmentsWarningMessage.Append("Rimuoverli e procedere ugualmente con l'invio?")
            Dim confirmFunction As String = String.Format("ExecuteAjaxRequest('{0}|{1}')", SendPecCommand, pec.Id)
            AjaxAlertConfirm(attachmentsWarningMessage.ToString(), confirmFunction, "", True)
            Return False
        End If

        Return True
    End Function

    Private Sub ArchiveAttachments(ByRef pec As PECMail, ByRef docs As List(Of DocumentInfo), ByRef errors As List(Of String))
        For Each attachment As DocumentInfo In docs
            Try
                If TypeOf attachment Is DocumentProxyDocumentInfo Then
                    Dim newTempDoc As FileInfo = attachment.SaveUniqueToTemp()
                    Facade.PECMailFacade.ArchiveAttachment(pec, New TempFileDocumentInfo(attachment.Name, newTempDoc), False)
                Else
                    Facade.PECMailFacade.ArchiveAttachment(pec, attachment, False)
                End If
            Catch ex As Exception
                errors.Add(String.Format("{0} -> [{1}]", attachment.Name, ex.Message))
                FileLogger.Error(LoggerName, ex.Message, ex)
                FileLogger.Warn(LoggerName, String.Format("Errore in fase di caricamento allegato {0}", attachment.ToString()))
            End Try
        Next
    End Sub

    Private Sub SendPec(ByRef pecMail As PECMail)
        Try
            ' Solo alla fine rendo attiva l'email creata
            pecMail.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Active)
            Facade.PECMailFacade.Update(pecMail)

            Facade.PECMailFacade.SendInsertPECMailCommand(pecMail)

            'Registro che il protocollo è stato inviato per PEC
            If CurrentProtocol IsNot Nothing Then
                Dim logMessage As String = String.Format("Protocollo inviato tramite PEC {0} a {1}", If(Not String.IsNullOrEmpty(pecMail.Segnatura), "Interoperabile", ""), pecMail.MailRecipients)
                If logMessage.Length > 254 Then
                    logMessage = logMessage.Substring(0, 254)
                End If
                Facade.ProtocolLogFacade.Insert(CurrentProtocol, ProtocolLogEvent.PO, logMessage)
                PushWorkflowNotify(pecMail)
            End If
        Catch ex As Exception
            Throw New DocSuiteException("Invio PEC", ex) With {.Descrizione = String.Format("Errore durante l'inserimento nella coda di invio del messaggio: {0}.", ex.Message), .User = DocSuiteContext.Current.User.FullUserName}
        End Try

        Try
            ' LOG inserimento nuova pec
            Facade.PECMailLogFacade.Created(pecMail)

            If OriginalMailToReply IsNot Nothing AndAlso Not ForwardPecMode Then
                ' LOG link tra le due mail
                Facade.PECMailLogFacade.Replied(OriginalMailToReply, pecMail)
            ElseIf CurrentPecMailList IsNot Nothing AndAlso ForwardPecMode Then
                For Each mail As PECMail In CurrentPecMailList
                    Facade.PECMailLogFacade.Forwarded(mail, pecMail)
                Next
            End If
        Catch ex As Exception
            FileLogger.Warn(LoggerName, ex.Message, ex)
        End Try
    End Sub

    Private Shared Function EmlFileNameFormat(source As String) As String
        Dim tmp As String = source
        If tmp.Length > 251 Then
            tmp = tmp.Substring(0, 251)
        End If
        tmp = Path.GetInvalidFileNameChars.Aggregate(tmp, Function(current, invalid) current.Replace(invalid, "_"))
        If String.IsNullOrEmpty(tmp) Then
            tmp = "undefined"
        End If
        Return tmp & FileHelper.EML
    End Function

    Private Sub GenerateSegnatura(senderList As IList(Of ContactDTO), recipientsList As IList(Of ContactDTO))
        If CurrentSignature.Length <> 0 Then
            Exit Sub
        End If

        Try
            ' creazione segnatura
            Dim segnatura As New Segnatura(CurrentProtocol, DocSuiteContext.PecSegnature)
            If senderList.Count <= 0 OrElse recipientsList.Count <= 0 Then
                Exit Sub
            End If

            segnatura.LoadContactList(senderList, ContactDirection.Sender)
            segnatura.LoadContactList(recipientsList, ContactDirection.Recipient)

            Dim segnaturaXml As XmlDocument = segnatura.GetXmlSignature()

            ' validazione segnatura
            ValidationResult = segnatura.IsInteropSignatureValid()
            If ValidationResult.IsValid Then
                CurrentSignature = segnaturaXml.OuterXml
                Exit Sub
            End If

            Dim validationException As String = ""
            If segnatura.ValidationException Is Nothing Then
                CurrentSignature = segnaturaXml.OuterXml
            Else
                validationException = segnatura.ValidationException.Message
            End If

            AjaxAlert(String.Format("{0} - {1}", ValidationResult.NotValidMessage, validationException))

        Catch ex As Exception
            FileLogger.Error(LoggerName, "Errore in generazione segnatura.", ex)
            AjaxAlert(ex.Message)
        End Try
    End Sub

    Private Function GetSignatura() As String
        If Not chkSetPecInteroperable.Checked Then
            Return String.Empty
        End If

        If Not ValidationResult.IsValid Then
            Throw New DocSuiteException(String.Format("Signature non valida: {0}.", ValidationResult.NotValidMessage))
        End If

        Dim retval As String
        Try
            Dim sign As New Segnatura(CurrentSignature, DocSuiteContext.PecSegnature)
            Dim xmlDoc As XmlDocument = sign.GetXmlSignature()
            If Not String.IsNullOrWhiteSpace(txtMailBody.Content) Then
                Dim xmlNote As XmlElement = xmlDoc.CreateElement("Note")
                xmlNote.InnerText = txtMailBody.Content
                xmlDoc.SelectSingleNode("//Descrizione").AppendChild(xmlNote)
            End If

            retval = xmlDoc.OuterXml
        Catch ex As Exception
            Throw New DocSuiteException("Recupero Signatura", String.Format("Errore in fase di generazione della Signature: {0}.", ValidationResult.NotValidMessage), ex)
        End Try
        Return retval
    End Function

    ''' <summary>
    ''' Verifica se ci sono contatti privi di e-mail ed avvisa l'utente
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CheckNoMailContacts(controlToCheck As uscContattiSel, ByRef notCertifiedMailList As IList(Of KeyValuePair(Of String, String))) As Boolean
        Dim noMailContactIds As String = ""
        Dim noMailContacts As String = ""
        If Not ProtocolEnv.CheckEmptyPecAddressFromSummary Then
            Return True
        End If

        If CurrentUDS IsNot Nothing AndAlso controlToCheck.GetContacts(False).Any(Function(x) String.IsNullOrEmpty(x.Contact.EmailAddress) AndAlso String.IsNullOrEmpty(x.Contact.CertifiedMail)) Then
            AjaxManager.ResponseScripts.Add(String.Format(ON_ERROR_FUNCTION, HttpUtility.JavaScriptStringEncode(String.Format("Impossibile inviare il messaggio PEC, alcuni {0} non possiedono un indirizzo e-mail.", controlToCheck.TreeViewCaption.ToLower()))))
            Return False
        End If

        For Each contactDto As ContactDTO In controlToCheck.GetContacts(False)
            If String.IsNullOrEmpty(contactDto.Contact.CertifiedMail) Then
                '' Se non ho indirizzo certificato
                If String.IsNullOrEmpty(contactDto.Contact.EmailAddress) Then
                    '' Se non ho neanche indirizzo standard
                    '' allora tengo traccia per chiedere all'utente la valorizzazione
                    If contactDto.Type = 0 Then
                        noMailContactIds &= contactDto.IdManualContact.ToString() & "$"
                    Else
                        noMailContactIds &= contactDto.Contact.Id.ToString() & "$"
                    End If
                    noMailContacts &= Replace(contactDto.Contact.Description, "|", " ") & "$"
                Else
                    '' Altrimenti so già che questo indirizzo verrà utilizzato al posto di quello certificato
                    notCertifiedMailList.Add(New KeyValuePair(Of String, String)(contactDto.Contact.Description.Replace("|", " "), contactDto.Contact.EmailAddress))
                End If
            End If
        Next

        If noMailContacts.Length <= 0 Then
            Return True
        End If

        noMailContactIds = noMailContactIds.TrimEnd("$"c)
        noMailContacts = noMailContacts.TrimEnd("$"c)

        Session(NoMailContactsSeed) = New KeyValuePair(Of String, String)(noMailContactIds.ToString, noMailContacts.Replace("'", String.Empty).ToString)
        Dim script As String = String.Format("OpenPecMailAddressWindow('{0}','{1}');", NoMailContactsSeed, String.Format("Impossibile inviare il messaggio PEC, alcuni {0} non possiedono un indirizzo e-mail. E’ stato riportato l’indirizzo PEC del contatto padre.", controlToCheck.TreeViewCaption.ToLower()))
        ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "InsertEmails", script, True)
        Return False
    End Function

    Private Function CheckNotCertifiedContacts(ByRef notCertifiedMailList As IList(Of KeyValuePair(Of String, String))) As Boolean
        '' Se non ho da verificare nulla allora ritorno subito True
        If notCertifiedMailList Is Nothing OrElse
            notCertifiedMailList.Count = 0 OrElse
            Not ProtocolEnv.PECAddressDestinationCheck Then
            Return True
        End If

        '' Altrimenti preparo il messaggio di verifica
        Dim emailAddresses As New StringBuilder()
        For Each address As KeyValuePair(Of String, String) In notCertifiedMailList
            emailAddresses.AppendFormat("{0}""{1}"" <{2}>", emailAddresses, address.Key, address.Value)
            emailAddresses.Append(Environment.NewLine)
        Next
        Dim message As String = String.Format("I seguenti contatti sono stati aggiunti tramite indirizzo mail standard:{0}{1}{0}Procedere?", Environment.NewLine, emailAddresses)
        AjaxAlertConfirm(message, String.Format("ExecuteAjaxRequest('{0}');", SendWithStandardMailCommand), String.Empty, True)
        Return False
    End Function

    Private Function CheckPecSize() As Boolean
        '' Verifico la grandezza complessiva
        Dim pecSize As Long = uscAttachment.TotalSize
        If ProtocolEnv.EnablePecAttachmentListFromProtocol Then
            pecSize += uscAttachmentList.TotalSize
        End If
        Dim maxSize As Long = Facade.PECMailboxFacade.GetMaxSendSize(CurrentMailBox)
        If pecSize < maxSize Then
            '' Se la grandezza complessiva è sotto il limite allora posso procedere senza warning
            Return True
        End If

        '' La PEC sfora la grandezza massima
        '' Calcolo dimensione reale della Pec 
        Dim realPecSize As Long = CalculateRealPecSendSize(pecSize)

        Dim attachmentsWarningMessage As New StringBuilder()
        attachmentsWarningMessage.AppendFormat("La PEC corrente ha dimensione [{0}] superiore ai limiti impostati dal server [{1}].", realPecSize.ToByteFormattedString(), CurrentMailBox.Configuration.MaxSendByteSize.ToByteFormattedString())
        attachmentsWarningMessage.Append(Environment.NewLine)


        '' Se esistano 1 o più allegati che da soli superano la grandezza massima consentita
        Dim confirmFunction As String
        If CheckExceedingDocument(attachmentsWarningMessage, maxSize) Then
            attachmentsWarningMessage.Append(Environment.NewLine)
            attachmentsWarningMessage.AppendFormat("Rimuovere gli allegati di dimensione eccessiva e procedere?")
            confirmFunction = String.Format("ExecuteAjaxRequest('{0}')", DeleteExceedingAttachmentsCommand)
        Else
            attachmentsWarningMessage.AppendFormat("Suddividere gli allegati su più {0}?", PecLabel)
            confirmFunction = String.Format("ExecuteAjaxRequest('{0}')", SplitAttachmentsCommand)
        End If

        AjaxAlertConfirm(attachmentsWarningMessage.ToString(), confirmFunction, "", True)

        Return False
    End Function

    Public Function CalculateRealPecSendSize(pecSize As Long) As Long
        Dim effectivePecSize As Double = pecSize
        Try
            Dim margine As String = DocSuiteContext.Current.ProtocolEnv.PecSendMaximumSizeMargin
            If margine.EndsWith("%") Then
                '' E' un valore percentuale
                Dim factor As Double = Double.Parse(margine.Split("%"c)(0))
                factor = (factor / 100) + 1
                effectivePecSize = pecSize * factor
            ElseIf margine.EndsWith("bytes") Then
                '' E' un valore da aggiungere alla PecSize
                Dim marginBytes As Double = Double.Parse(margine.Substring(0, margine.IndexOf("bytes", StringComparison.Ordinal)))
                effectivePecSize = pecSize + marginBytes
            End If
        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Impossibile calcolare la dimensione della PEC", ex)
        End Try

        Return Convert.ToInt64(effectivePecSize)
    End Function

    Private Function CheckExceedingDocument(ByRef attachmentsWarningMessage As StringBuilder, maxSize As Long) As Boolean
        Dim documentsWarningMessage As New StringBuilder()
        For Each documentInfo As DocumentInfo In From documentInfo1 In CurrentAttachments Where documentInfo1.Size > maxSize
            documentsWarningMessage.AppendFormat("-> {0} ({1})", documentInfo.Name, documentInfo.Size.ToByteFormattedString())
            documentsWarningMessage.Append(Environment.NewLine)
        Next

        If documentsWarningMessage.Length > 0 Then
            attachmentsWarningMessage.Append(Environment.NewLine)
            attachmentsWarningMessage.AppendFormat("I seguenti allegati superano singolarmente la grandezza massima consentita:")
            attachmentsWarningMessage.Append(Environment.NewLine)
            attachmentsWarningMessage.Append(documentsWarningMessage)
            Return True
        End If

        Return False
    End Function

    Private Sub SendPecFunction(ByVal ajaxRequestCommand As String)
        Try
            If CurrentMailBox IsNot Nothing AndAlso CurrentMailBox.LoginError Then
                AjaxAlert("La casella PEC selezionata presenta un problema di configurazione. Informare la persona responsabile della corretta configurazione")
            End If

            If (ddlMailFrom.SelectedItem Is Nothing OrElse String.IsNullOrEmpty(ddlMailFrom.SelectedValue)) Then
                AjaxAlert("E' necessario specificare un mittente")
                Exit Sub
            End If
            '' Verifiche sui contatti
            If String.IsNullOrEmpty(ajaxRequestCommand) AndAlso Not CheckContacts() Then
                Exit Sub
            End If

            'txtMailBody.EditModes = EditModes.Html

            '' Istanzio multipleType a "NoSplit: casistica standard" oppure a "SplitByRecipients: se il checkbox è selezionato"
            '' ManageAttachments si occupa di definire se sia necessario uno split specifico per allegati e/o contatti
            Dim multipleType As PecMailMultipleTypeEnum = If(chkMultiPec.Checked, PecMailMultipleTypeEnum.SplitByRecipients, PecMailMultipleTypeEnum.NoSplit)
            If Not ManageAttachments(ajaxRequestCommand, multipleType) Then
                Exit Sub
            End If

            '' Genero e invio la PEC
            If Not ElaboratePec(multipleType) Then
                Exit Sub
            End If

            '' Ritorno la grafica
            RedirectToSource()
        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore invio PEC", ex)
            If CurrentUDS IsNot Nothing Then
                AjaxManager.ResponseScripts.Add(String.Format(ON_ERROR_FUNCTION, HttpUtility.JavaScriptStringEncode(ex.Message)))
            Else
                AjaxAlert("Errore nell'invio mail: {0}", ex.Message)
            End If
        End Try
    End Sub

    Private Sub RedirectToSource()
        If RedirectToProtocolSummary Then
            Response.Redirect($"~/Prot/ProtVisualizza.aspx?{CommonShared.AppendSecurityCheck($"Type=Prot&UniqueId={CurrentProtocol.Id}")}")
        End If

        If IsWorkflowOperation Then
            Response.Redirect(String.Format("~/Prot/ProtVisualizza.aspx?UniqueId={0}&Type=Prot&Action=FromPEC&IsWorkflowOperation=True&IdWorkflowActivity={1}", CurrentProtocol.Id, CurrentIdWorkflowActivity))
        End If

        If CurrentUDS IsNot Nothing Then
            Response.Redirect(String.Format(UDS_SUMMARY_PATH, CurrentUDS.Id, CurrentUDS.UDSRepository.UniqueId))
        End If

        If OriginalMailToReply IsNot Nothing Then
            Response.Redirect(String.Format("PECView.aspx?Type=Pec&PECId={0}{1}", OriginalMailToReply.Id, If(ProtocolBoxEnabled, "&ProtocolBox=True", String.Empty)))
        Else
            Dim param As String
            param = String.Format("{0}{1}SelectedMailBoxId={2}", RedirectTo, If(RedirectTo.Contains("?"), "&", "?"), SelectedMailboxId)
            Response.Redirect(param)
        End If
    End Sub

    Private Function CheckContacts() As Boolean
        '' Verifica dei contatti senza indirizzi utilizzabili
        Dim notCertifiedMailList As IList(Of KeyValuePair(Of String, String)) = New List(Of KeyValuePair(Of String, String))
        '' Destinatari normali
        If Not CheckNoMailContacts(uscDestinatari, notCertifiedMailList) Then
            Return False
        End If
        '' Destinatari CC
        If Not CheckNoMailContacts(uscDestinatariCc, notCertifiedMailList) Then
            Return False
        End If

        If Not CheckNotCertifiedContacts(notCertifiedMailList) Then
            Return False
        End If

        Return True
    End Function

    Private Function ManageAttachments(ByVal ajaxRequestCommand As String, ByRef multipleType As PecMailMultipleTypeEnum) As Boolean
        Dim doCheckSize As Boolean
        Select Case ajaxRequestCommand
            Case DeleteExceedingAttachmentsCommand
                '' Ricerco tra gli allegati tutti quelli che non possono
                '' essere inviati neanche singolarmente
                '' e li rimuovo
                Dim documentsToAnalyzeFromGrid As IList(Of DocumentInfo) = uscAttachmentList.GetSelectedDocuments()
                For Each documentInfo As DocumentInfo In From documentInfo1 In documentsToAnalyzeFromGrid Where documentInfo1.Size > CurrentMailBox.Configuration.MaxSendByteSize
                    uscAttachmentList.RemoveDocumentInfo(documentInfo)
                Next
                Dim documentsToAnalyzeFromUpload As IList(Of DocumentInfo) = uscAttachment.DocumentInfos.ToList()
                For Each documentInfo As DocumentInfo In From documentInfo1 In documentsToAnalyzeFromUpload Where documentInfo1.Size > CurrentMailBox.Configuration.MaxSendByteSize
                    uscAttachment.RemoveDocumentInfo(documentInfo)
                Next
                '' Faccio ugualmente un altro controllo sugli allegati per verificare se
                '' anche dopo l'eliminazione degli allegati eccessivi devo splittare
                doCheckSize = True
            Case SplitAttachmentsCommand
                If multipleType = PecMailMultipleTypeEnum.SplitByRecipients Then
                    '' Se richiesto split per recipients
                    '' aggiungo lo split per Size
                    multipleType = PecMailMultipleTypeEnum.SplitBySizeAndRecipients
                Else
                    '' Altrimenti imposto direttamente lo split per Size
                    multipleType = PecMailMultipleTypeEnum.SplitBySize
                End If
            Case Else
                doCheckSize = True
        End Select

        '' Verifiche sulla grandezza della PEC
        '' solo se non è stata richiesta la gestione automatica degli allegati
        If doCheckSize AndAlso Not CheckPecSize() Then
            Return False
        End If

        '' Se tutti i controlli precedenti sono andati a buon fine allora posso procedere
        Return True
    End Function

    Private Function ElaboratePec(multipleType As PecMailMultipleTypeEnum) As Boolean
        Try

            'Carico tutti i destinatari e i documenti una volta sola.
            'In caso di PEC multipla sarà cura del JeepService DSWPec effettuare la duplicazione
            Dim pecRecipients As String = GetSelectedRecipients(uscDestinatari)
            Dim pecRecipientsCc As String = GetSelectedRecipients(uscDestinatariCc)
            Dim pec As PECMail = CreatePec(pecRecipients, pecRecipientsCc, multipleType)
            If pec Is Nothing Then
                Return False
            End If
            SendPec(pec)
            Return True
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Private Sub BindWarningPanel(ByVal errorMessage As String)
        If Not String.IsNullOrEmpty(errorMessage) Then
            WarningLabel.Text = errorMessage
            WarningPanel.CssClass = "warningAreaLow"
        End If
    End Sub

    Private Sub PushWorkflowNotify(pecMail As PECMail)
        If IsWorkflowOperation AndAlso CurrentIdWorkflowActivity.HasValue Then
            Dim worklowAcyivity As WorkflowActivity = GetWorkflowActivity(CurrentIdWorkflowActivity.Value)
            Dim workflowNotify As WorkflowNotify = New WorkflowNotify(CurrentIdWorkflowActivity.Value) With {
                    .WorkflowName = worklowAcyivity?.WorkflowInstance?.WorkflowRepository?.Name}
            workflowNotify.OutputArguments.Add(WorkflowPropertyHelper.DSW_FIELD_PEC_ID, New WorkflowArgument() With {
                                                   .Name = WorkflowPropertyHelper.DSW_FIELD_PEC_ID,
                                                   .PropertyType = ArgumentType.PropertyInt,
                                                   .ValueInt = pecMail.Id})

            Dim webApiHelper As WebAPIHelper = New WebAPIHelper()
            If Not WebAPIImpersonatorFacade.ImpersonateSendRequest(webApiHelper, workflowNotify, DocSuiteContext.Current.CurrentTenant.WebApiClientConfig, DocSuiteContext.Current.CurrentTenant.OriginalConfiguration) Then
                Throw New Exception("Settaggio proprietà workflow non riuscita.")
            End If
        End If
    End Sub

    Private Sub SetDocumentControl(control As uscDocumentList, document As Helpers.UDS.Document)
        If document Is Nothing OrElse document.Instances Is Nothing Then
            Exit Sub
        End If
        Dim docInfos As IList(Of BiblosDocumentInfo) = New List(Of BiblosDocumentInfo)
        For Each instance As DocumentInstance In document.Instances
            Dim bibDocs As IList(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(Guid.Parse(instance.StoredChainId))
            'Verifico se è stato passato l'ID document invece dell'ID chain
            If bibDocs.Count = 0 Then
                bibDocs = BiblosDocumentInfo.GetDocumentInfo(Guid.Parse(instance.StoredChainId), Nothing, True)
            End If
            For Each doc As BiblosDocumentInfo In bibDocs
                docInfos.Add(doc)
            Next
        Next
        control.LoadDocumentInfos(docInfos.Cast(Of DocumentInfo).ToList())
    End Sub

    Private Function CompressDocuments(Optional password As String = "", Optional zipFileName As String = "File compressi.zip") As IList(Of DocumentInfo)
        Dim results As IList(Of DocumentInfo) = New List(Of DocumentInfo)()
        Dim CompressManager As ICompress = New ZipCompress()
        Dim content As IEnumerable(Of KeyValuePair(Of String, Byte())) = CurrentAttachments.Select(Function(d) New KeyValuePair(Of String, Byte())(String.Concat(String.Join(String.Empty, Path.GetFileNameWithoutExtension(d.Name).Take(240)), Path.GetExtension(d.Name)), d.Stream))
        If content.Any() Then
            Dim zipFolderCompressedStream As Byte() = CompressManager.InMemoryCompress(content.ToList(), True, password)
            results.Add(New MemoryDocumentInfo(zipFolderCompressedStream, zipFileName))
        End If
        Return results
    End Function

    Private Sub SendPecWithPassword(pecMail As PECMail, message As String)
        Dim protectedPec As PECMail = New PECMail()
        protectedPec.Direction = PECMailDirection.Outgoing
        protectedPec.Year = pecMail.Year
        protectedPec.Number = pecMail.Number
        protectedPec.MailSubject = pecMail.MailSubject
        protectedPec.MailSenders = pecMail.MailSenders
        protectedPec.MailRecipients = pecMail.MailRecipients
        protectedPec.MailBody = String.Concat("La password per visualizzare i documenti della PEC è la seguente: ", message)
        protectedPec.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Active)
        protectedPec.MailBox = pecMail.MailBox
        protectedPec.Location = pecMail.Location
        protectedPec.IsValidForInterop = pecMail.IsValidForInterop
        protectedPec.MultipleType = pecMail.MultipleType
        protectedPec.RegistrationUser = DocSuiteContext.Current.User.FullUserName
        protectedPec.RegistrationDate = DateTimeOffset.UtcNow

        If ProtocolEnv.SendPecWithPasswordToMailRecipientsCCEnabled Then
            protectedPec.MailRecipientsCc = pecMail.MailRecipientsCc
        End If

        Facade.PECMailFacade.Save(protectedPec)
        Facade.PECMailFacade.SendInsertPECMailCommand(protectedPec)
        FileLogger.Info(LoggerName, String.Concat("PECInsert - SendPECWithPassword - PEC con password inserita in coda di invio"))
    End Sub

    Private Function CreateRadComboboxItem(pecMailBox As PECMailBox) As RadComboBoxItem
        Dim comboboxItem As RadComboBoxItem = New RadComboBoxItem()
        comboboxItem.Text = pecMailBox.MailBoxName
        comboboxItem.Value = pecMailBox.Id.ToString()
        comboboxItem.ImageUrl = If(pecMailBox.IsForInterop, INTEROP_IMGURL, VALID_MAILBOX_IMGURL)
        If pecMailBox.LoginError Then
            comboboxItem.AddAttribute("hasLoginError", pecMailBox.LoginError.ToString())
            comboboxItem.ForeColor = Drawing.Color.Red
        End If

        Return comboboxItem
    End Function
#End Region

End Class
