Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Web
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.IP4D
Imports VecompSoftware.DocSuiteWeb.Model.ExternalModels
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.Compress
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Signer.Security
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging

Namespace MailSenders
    Public Class MailSenderControl
        Inherits DocSuite2008BaseControl
        Private _currentOriginalAttachments As IList(Of DocumentInfo) = Nothing
        Private _currentPdfAttachments As IList(Of DocumentInfo) = Nothing
        Private _currentIP4DFacade As IP4DFacade
        Private _passwordGenerator As PasswordGenerator
        Private _currentContactRecipientPosition As MessageContact.ContactPositionEnum = MessageContact.ContactPositionEnum.Recipient
        Public Event MailSent(ByVal sender As Object, ByVal e As MailSentEventArgs)
        Public Event IP4DSent(ByVal sender As Object, ByVal e As IP4DSentEventArgs)
        Public Event CancelByUser(ByVal sender As Object, ByVal e As EventArgs)
        Public Event ConfirmByUser(ByVal sender As Object, ByVal e As EventArgs)
        Public Event MailError(ByVal sender As Object, ByVal e As MailErrorEventArgs)


#Region " Properties "
        Private ReadOnly Property CurrentOriginalAttachments As IList(Of DocumentInfo)
            Get
                If _currentOriginalAttachments Is Nothing Then
                    Dim tor As List(Of DocumentInfo) = New List(Of DocumentInfo)

                    For Each item As GridDataItem In DocumentListGrid.MasterTableView.Items()
                        Dim document As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(CType(item.GetDataKeyValue("Serialized"), String)))
                        If DirectCast(item.FindControl("chkOriginal"), CheckBox).Checked Then
                            tor.Add(document)
                        End If
                    Next

                    '' aggiungo poi i documenti standard
                    tor.AddRange(uscAttachment.DocumentInfos)
                    _currentOriginalAttachments = tor
                End If
                Return _currentOriginalAttachments
            End Get
        End Property

        Private ReadOnly Property CurrentPdfAttachments As IList(Of DocumentInfo)
            Get
                If _currentPdfAttachments Is Nothing Then
                    Dim tor As New List(Of DocumentInfo)

                    For Each item As GridDataItem In DocumentListGrid.MasterTableView.Items()
                        Dim document As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(CType(item.GetDataKeyValue("Serialized"), String)))
                        If DirectCast(item.FindControl("chkPdf"), CheckBox).Checked Then
                            tor.Add(document)
                        End If
                    Next
                    _currentPdfAttachments = tor
                End If
                Return _currentPdfAttachments
            End Get
        End Property

        Private ReadOnly Property ChkNeedDisposition As RadButton
            Get
                Dim controlContainer As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("needDisposition"), RadToolBarButton)
                Return DirectCast(controlContainer.FindControl("chkNeedDisposition"), RadButton)
            End Get
        End Property

        Public ReadOnly Property ChkSecuredDocuments As RadButton
            Get
                Dim controlContainer As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("securedDocuments"), RadToolBarButton)
                Return DirectCast(controlContainer.FindControl("chkSecuredDocuments"), RadButton)
            End Get
        End Property

        Public ReadOnly Property ChkCompressedDocuments As RadButton
            Get
                Dim controlContainer As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("compressedDocuments"), RadToolBarButton)
                Return DirectCast(controlContainer.FindControl("chkCompressedDocuments"), RadButton)
            End Get
        End Property

        Public Property SenderDescriptionValue As String
            Get
                Return SenderDescription.Text
            End Get
            Set(value As String)
                SenderDescription.Text = value
            End Set
        End Property

        Public Property CurrentContactRecipientPosition As MessageContact.ContactPositionEnum
            Get
                If String.IsNullOrEmpty(hf_MessageRecipientPosition.Value) Then
                    Return MessageContact.ContactPositionEnum.Recipient
                End If
                Return EnumHelper.ParseDescriptionToEnum(Of MessageContact.ContactPositionEnum)(hf_MessageRecipientPosition.Value)
            End Get
            Set(value As MessageContact.ContactPositionEnum)
                _currentContactRecipientPosition = value
                hf_MessageRecipientPosition.Value = _currentContactRecipientPosition.ToString()
            End Set
        End Property

        Public Property SenderEmailValue As String
            Get
                Return SenderEmail.Text
            End Get
            Set(value As String)
                If ProtocolEnv.CurrentUserEMailSenderEnabled = True Then
                    SenderEmail.Text = If(String.IsNullOrEmpty(value), String.Empty, value.Trim())
                End If
            End Set
        End Property

        Public ReadOnly Property SubjectMaxLength As Integer
            Get
                If ProtocolEnv.MessageObjectMaxLength > 0 Then
                    Return ProtocolEnv.MessageObjectMaxLength
                End If
                Return 500
            End Get
        End Property

        Public Property SubjectValue As String
            Get
                Return Subject.Text
            End Get
            Set(value As String)
                Subject.Text = value
            End Set
        End Property
        Public Property EnableAttachment As Boolean

        Public Property BodyValue As String
            Get
                Return Body.Content
            End Get
            Set(value As String)
                Body.Content = value
            End Set
        End Property

        Public Property Recipients As IList(Of ContactDTO)

        Public Property Documents As IList(Of DocumentInfo)

        Public Property EnableCheckBoxRecipients As Boolean = False

        Public Property AuthorizationsVisibility As Boolean = True

        Public Property IsIP4DEnabled As Boolean

        Public Property UDYear As Short?
            Get
                If ViewState("UDYear") IsNot Nothing Then
                    Return DirectCast(ViewState("UDYear"), Short)
                End If
                Return Nothing
            End Get
            Set(value As Short?)
                ViewState("UDYear") = value
            End Set
        End Property

        Public Property MailWithPasswordBody As String
            Get
                If ViewState("MailWithPasswordBody") IsNot Nothing Then
                    Return ViewState("MailWithPasswordBody").ToString()
                End If
                Return String.Empty
            End Get
            Set(value As String)
                ViewState("MailWithPasswordBody") = value
            End Set
        End Property

        Public Property UDNumber As Integer?
            Get
                If ViewState("UDNumber") IsNot Nothing Then
                    Return DirectCast(ViewState("UDNumber"), Integer)
                End If
                Return Nothing
            End Get
            Set(value As Integer?)
                ViewState("UDNumber") = value
            End Set
        End Property

        Public Property UDId As Guid?
            Get
                If ViewState("UDId") IsNot Nothing Then
                    Return DirectCast(ViewState("UDId"), Guid)
                End If
                Return Nothing
            End Get
            Set(value As Guid?)
                ViewState("UDId") = value
            End Set
        End Property

        Public Property UDRegistrationUser As String
            Get
                If ViewState("UDRegistrationUser") IsNot Nothing Then
                    Return ViewState("UDRegistrationUser").ToString()
                End If
                Return Nothing
            End Get
            Set(value As String)
                ViewState("UDRegistrationUser") = value
            End Set
        End Property

        Public Property UDRegistrationDate As DateTimeOffset?
            Get
                If ViewState("UDRegistrationDate") IsNot Nothing Then
                    Return DateTimeOffset.Parse(ViewState("UDRegistrationDate").ToString())
                End If
                Return Nothing
            End Get
            Set(value As DateTimeOffset?)
                ViewState("UDRegistrationDate") = value
            End Set
        End Property

        Public ReadOnly Property MessageTablePanel As Panel
            Get
                Return messageTable
            End Get
        End Property

        Public ReadOnly Property ButtonToolBar As RadToolBar
            Get
                Return ToolBar
            End Get
        End Property

        Private ReadOnly Property SendButton As RadToolBarButton
            Get
                Return DirectCast(ToolBar.FindItemByValue("MailSender_Send"), RadToolBarButton)
            End Get
        End Property

        Private ReadOnly Property CurrentIP4DFacade As IP4DFacade
            Get
                If _currentIP4DFacade Is Nothing Then
                    _currentIP4DFacade = New IP4DFacade()
                End If
                Return _currentIP4DFacade
            End Get
        End Property

        Public Property ShowCustomError As Boolean

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
            InitializeAjax()
            uscAttachment.AllowZipDocument = ProtocolEnv.ZipUploadEnabled
            If Not IsPostBack Then
                Initialize()
            End If

        End Sub

        Protected Sub RadAjaxManagerAjaxRequest(sender As Object, e As AjaxRequestEventArgs)
            Dim arguments As String() = Split(e.Argument, "|", 2)
            Select Case arguments(0)
                Case "MailSender_Confirm"
                    RaiseEvent ConfirmByUser(Me, New EventArgs())
                Case "Send_Mail"
                    Dim items As String() = arguments(1).Split(";"c, ","c)
                    If items IsNot Nothing AndAlso items.Length > 0 Then
                        Dim ids As IList(Of Integer) = items.Select(Function(x) Convert.ToInt32(x)).ToList()
                        Dim roles As IList(Of Role) = Facade.RoleFacade.GetByIds(ids.ToList())
                        Recipients = RoleFacade.GetValidContacts(roles)
                        DataBind()
                    End If
                Case "MailSender_Send_External"
                    SendMail()
            End Select

        End Sub

        Protected Sub DocumentListGridItemDataBound(ByVal sender As System.Object, ByVal e As GridItemEventArgs) Handles DocumentListGrid.ItemDataBound
            If e.Item.ItemType <> GridItemType.Item AndAlso e.Item.ItemType <> GridItemType.AlternatingItem Then
                Exit Sub
            End If

            Dim item As DocumentInfo = DirectCast(e.Item.DataItem, DocumentInfo)

            Dim documentType As ImageButton = DirectCast(e.Item.FindControl("documentType"), ImageButton)
            documentType.ImageUrl = ImagePath.FromDocumentInfo(item)

            Dim lblFileName As Label = DirectCast(e.Item.FindControl("lblFileName"), Label)
            lblFileName.Text = item.Name

            Dim chkPdf As CheckBox = DirectCast(e.Item.FindControl("chkPdf"), CheckBox)
            chkPdf.Checked = If(FileHelper.MatchFileName(item.Name, FileHelper.BUSTA), False, True)

            Dim chkOriginal As CheckBox = DirectCast(e.Item.FindControl("chkOriginal"), CheckBox)
            If FileHelper.MatchExtension(item.Name, FileHelper.P7M) Then
                chkOriginal.Checked = True
            End If
        End Sub

        Protected Sub DocumentListGridCommand(ByVal sender As System.Object, ByVal e As GridCommandEventArgs) Handles DocumentListGrid.ItemCommand
            If Not e.CommandName.Eq("preview") Then
                Exit Sub
            End If

            Dim document As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(DirectCast(e.Item, GridDataItem).GetDataKeyValue("Serialized").ToString()))
            AjaxManager.ResponseScripts.Add(String.Format("OpenGenericWindow('../Viewers/DocumentInfoViewer.aspx?{0}')", document.ToQueryString().AsEncodedQueryString()))
        End Sub

        Private Sub ToolBarButtonClick(sender As Object, e As RadToolBarEventArgs) Handles ToolBar.ButtonClick
            Select Case e.Item.Value
                Case "MailSender_SendIP4D"
                    SendIP4D()
                Case "MailSender_Send"
                    SendMail()
                Case "MailSender_Cancel"
                    RaiseEvent CancelByUser(Me, New EventArgs())
            End Select
        End Sub

        Public Sub chkSecuredDocumentsClick(sender As Object, e As EventArgs)
            SecuredDocumentsChecked()
        End Sub
#End Region

#Region " Methods "

        Private Sub InitializeAjax()
            AddHandler AjaxManager.AjaxRequest, AddressOf RadAjaxManagerAjaxRequest

            AjaxManager.AjaxSettings.AddAjaxSetting(ToolBar, ToolBar, BasePage.MasterDocSuite.AjaxFlatLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(ToolBar, messageTable, BasePage.MasterDocSuite.AjaxDefaultLoadingPanel)

            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, ToolBar, BasePage.MasterDocSuite.AjaxFlatLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, messageTable, BasePage.MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscAttachment, BasePage.MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(uscSettori, Me.panelSettori)
            AjaxManager.AjaxSettings.AddAjaxSetting(uscSettori, uscSettori)
        End Sub

        Private Sub Initialize()
            If ProtocolEnv.CurrentUserEMailSenderEnabled = False Then
                SenderEmail.Text = ProtocolEnv.ProtPecSendSender
                SenderDescription.Text = ProtocolEnv.ProtPecSendSender
            End If
            ' abilitazione flag per la visualizzione della sezione di "richiedi conferma per recapito di lettura"
            ChkNeedDisposition.Visible = ProtocolEnv.ShowDispositionNotification
            ChkNeedDisposition.Checked = ProtocolEnv.DispositionNotification
            ChkCompressedDocuments.Visible = ProtocolEnv.EmailCompressEnabled AndAlso (Not Documents.IsNullOrEmpty() OrElse EnableAttachment)
            ChkSecuredDocuments.Visible = Not Documents.IsNullOrEmpty() OrElse EnableAttachment
            ' abilitazione flag per la visualizzione della sezione di "invio per conto di un settore"
            If ProtocolEnv.ShowSendBySectorNotification AndAlso AuthorizationsVisibility Then
                panelSettori.Visible = True
                uscSettori.Visible = True
                ' Caricamento settori
                BindAutorizzazioniCc()
            Else
                panelSettori.Visible = False
                uscSettori.Visible = False
            End If

            'Gestisco la cancellazione multipla dei destinatari di una mail
            MessageRecipients.EnableCheck = EnableCheckBoxRecipients

            If IsIP4DEnabled Then
                SendButton.Text = "Invia IP4D"
                SendButton.Value = "MailSender_SendIP4D"
                MessageRecipients.ButtonAddMyselfVisible = False
                MessageRecipients.ButtonImportManualVisible = False
                MessageRecipients.ButtonImportVisible = False
                MessageRecipients.ButtonIPAVisible = False
                MessageRecipients.ButtonManualMultiVisible = False
                MessageRecipients.ButtonManualVisible = False
                MessageRecipients.ButtonRoleVisible = False
                MessageRecipients.ButtonSdiContactVisible = False
                MessageRecipients.ButtonSelectOChartVisible = False
                MessageRecipients.ButtonSelectVisible = False
                MessageRecipients.IP4DRestriction = True
            End If

            pnlAttachment.Visible = EnableAttachment
            uscAttachment.ButtonCopyProtocol.Visible = ProtocolEnv.CopyProtocolDocumentsEnabled
            uscAttachment.ButtonCopySeries.Visible = ProtocolEnv.CopyFromSeries
            If (DocSuiteContext.Current.IsResolutionEnabled) Then
                uscAttachment.ButtonCopyResl.Visible = ResolutionEnv.CopyReslDocumentsEnabled
            End If
            maxChars.Text = SubjectMaxLength.ToString()
            CheckSenderEmail()
        End Sub

        Private Sub SendIP4D()
            Try
                If String.IsNullOrEmpty(SenderEmailValue) Then
                    Throw New ApplicationException("Non è presente un indirizzo mail valido per il mittente")
                End If

                If String.IsNullOrEmpty(ProtocolEnv.ExternalViewerProtocolLink) Then
                    Throw New ApplicationException("Parametro ExternalViewerProtocolLink non configurato")
                End If

                If Not UDYear.HasValue OrElse Not UDNumber.HasValue OrElse Not UDId.HasValue Then
                    Throw New ApplicationException("Non sono stati definiti alcuni parametri del documento")
                End If

                Dim ip4dModel As ExternalViewerModel = New ExternalViewerModel() With {
                    .Year = UDYear.Value,
                    .Number = UDNumber.Value,
                    .UniqueId = UDId.Value,
                    .Body = BodyValue,
                    .Subject = SubjectValue,
                    .RegistrationUser = UDRegistrationUser,
                    .RegistrationDate = UDRegistrationDate,
                    .Url = String.Format(ProtocolEnv.ExternalViewerProtocolLink, UDYear, UDNumber, UDId),
                    .Sender = New ExternalViewerContactModel() With {.Name = SenderDescriptionValue, .Email = SenderEmailValue}
                }

                ' Aggiungo i destinatari
                Dim recipientsSelected As IList(Of ContactDTO) = MessageRecipients.GetContacts(False)
                If recipientsSelected.Count = 0 Then
                    Throw New ApplicationException("Selezionare almeno un destinatario.")
                End If

                ip4dModel.Recipients = New List(Of ExternalViewerContactModel)
                For Each contact As ContactDTO In recipientsSelected
                    Dim contactMail As String = RegexHelper.MatchEmail(contact.Contact.EmailAddress)
                    If String.IsNullOrEmpty(contactMail) Then
                        Throw New ApplicationException(String.Format("Il destinatario ({0}) non ha un indirizzo email.", contact.Contact.Description))
                    End If
                    ip4dModel.Recipients.Add(New ExternalViewerContactModel() With
                                             {.Name = contact.Contact.Description,
                                             .Email = contact.Contact.EmailAddress,
                                             .Account = contact.Contact.SearchCode})
                Next

                'Invia il comando alle WebAPI
                CurrentIP4DFacade.SendIP4D(ip4dModel)
                RaiseEvent IP4DSent(Me, New IP4DSentEventArgs(ip4dModel))
                RaiseEvent ConfirmByUser(Me, New EventArgs())
            Catch argEx As ApplicationException
                BasePage.AjaxAlert("Impossibile invio IP4D: {0}.", argEx.Message)
            Catch ex As Exception
                FileLogger.Error(LoggerName, ex.Message, ex)
                BasePage.AjaxAlert("Errore impossibile invio IP4D. Contattare l'assistenza")
            End Try
        End Sub

        Private Sub SendMail()
            Try
                'Dovrebbe essere già verificato dal controllo nella pagina ma eseguo lo stesso un controllo lato server
                If SubjectValue.Length > SubjectMaxLength Then
                    Throw New DocSuiteException("Errore invio", "Il valore del campo oggetto supera la dimensione massima consentita")
                End If

                If String.IsNullOrEmpty(SenderEmailValue) Then
                    Throw New DocSuiteException("Errore invio", "Non è presente un indirizzo mail valido per il mittente")
                End If

                Dim contacts As List(Of MessageContactEmail) = New List(Of MessageContactEmail)
                contacts.Add(Facade.MessageContactEmailFacade.CreateEmailContact(SenderDescription.Text, DocSuiteContext.Current.User.FullUserName, SenderEmailValue, MessageContact.ContactPositionEnum.Sender))

                ' Aggiungo i destinatari
                Dim recipientsSelected As IList(Of ContactDTO) = MessageRecipients.GetContacts(False)
                If recipientsSelected.Count = 0 Then
                    Throw New DocSuiteException("Errore invio", "Selezionare almeno un destinatario.")
                End If

                If Not CheckAttachmentsSize() Then
                    Exit Sub
                End If

                For Each contact As ContactDTO In recipientsSelected
                    Dim contactMail As String = RegexHelper.MatchEmail(contact.Contact.EmailAddress)
                    If String.IsNullOrEmpty(contactMail) Then
                        contactMail = RegexHelper.MatchEmail(contact.Contact.CertifiedMail)
                    End If
                    If String.IsNullOrEmpty(contactMail) Then
                        Throw New DocSuiteException("Errore invio", String.Format("Il destinatario ({0}) non ha un indirizzo email.", contact.Contact.DescriptionFormatByContactType))
                    End If
                    Dim contactDesc As String = contact.Contact.DescriptionFormatByContactType
                    If String.IsNullOrEmpty(contactDesc) Then
                        contactDesc = contactMail
                    End If

                    If contact.Contact.ContactType.Id.Equals(ContactType.Role) Then
                        contacts.Add(Facade.MessageContactEmailFacade.CreateEmailContact(contactDesc, DocSuiteContext.Current.User.FullUserName, contactMail, CurrentContactRecipientPosition, MessageContact.ContactTypeEnum.Role))
                    Else
                        contacts.Add(Facade.MessageContactEmailFacade.CreateEmailContact(contactDesc, DocSuiteContext.Current.User.FullUserName, contactMail, CurrentContactRecipientPosition))
                    End If
                Next

                Dim email As MessageEmail
                Dim settori As String = String.Empty

                If ProtocolEnv.ShowSendBySectorNotification = True Then
                    Dim stringaSettori As String = ConcatSelectedRoles()
                    ' Creo la stringa di invio settori solamente se è stato selezionato almento un settore
                    If Not String.IsNullOrEmpty(stringaSettori) Then
                        settori = String.Format(ProtocolEnv.SendBySectorFormat, SenderEmailValue, ConcatSelectedRoles())
                    End If
                End If

                email = Facade.MessageEmailFacade.CreateEmailMessage(contacts, Subject.Text, Body.Content, ChkNeedDisposition.Checked, settori)

                If CurrentOriginalAttachments.Count > 0 OrElse CurrentPdfAttachments.Count > 0 Then
                    If (ChkCompressedDocuments.Visible OrElse ChkSecuredDocuments.Visible) AndAlso (ChkCompressedDocuments.Checked OrElse ChkSecuredDocuments.Checked) Then
                        Dim password As String = String.Empty
                        If ChkSecuredDocuments.Visible AndAlso ChkSecuredDocuments.Checked Then
                            password = PasswordGenerator.GenerateAlphanumericPassword(8)
                            SendMailWithPassword(contacts.Select(Function(x) x.Clone()).Cast(Of MessageContactEmail).ToList(),
                                             Subject.Text, String.Concat(MailWithPasswordBody, password), settori)
                        End If
                        If ChkSecuredDocuments.Checked OrElse ChkCompressedDocuments.Checked Then
                            Dim zipFolderDocumentInfo As MemoryDocumentInfo = CompressDocuments(password)
                            Facade.MessageFacade.AddAttachment(email.Message, zipFolderDocumentInfo, False)
                        End If
                    Else
                        ' Allego i documenti
                        For Each originalAttachment As DocumentInfo In CurrentOriginalAttachments
                            Facade.MessageFacade.AddAttachment(email.Message, originalAttachment, False)
                        Next

                        For Each pdfAttachment As DocumentInfo In CurrentPdfAttachments
                            Facade.MessageFacade.AddAttachment(email.Message, pdfAttachment, True)
                        Next
                    End If
                End If

                Dim idMessage As Integer = FacadeFactory.Instance.MessageEmailFacade.SendEmailMessage(email)
                FileLogger.Info(LoggerName, String.Concat("MailSenderControl - SendEmail - Mail inserita in coda di invio [id ", idMessage, " ]"))

                Dim args As New MailSentEventArgs(email)
                RaiseEvent MailSent(Me, args)

                If args.ShowConfirm Then
                    AjaxManager.ResponseScripts.Add("openConfirmWindow();")
                Else
                    RaiseEvent ConfirmByUser(Me, New EventArgs())
                End If

            Catch dswx As DocSuiteException
                RaiseEvent MailError(Me, New MailErrorEventArgs() With {.Message = String.Format("Impossibile recapitare l'email: {0}.", dswx.Message)})
                If Not ShowCustomError Then
                    BasePage.AjaxAlert("Impossibile recapitare l'email: {0}.", dswx.Message)
                End If
            Catch ex As Exception
                FileLogger.Warn(LoggerName, ex.Message, ex)
                RaiseEvent MailError(Me, New MailErrorEventArgs() With {.Message = "Errore impossibile inviare Email. Contattare l'assistenza"})
                If Not ShowCustomError Then
                    BasePage.AjaxAlert("Errore impossibile inviare Email. Contattare l'assistenza")
                End If
            End Try
        End Sub

        Public Overloads Sub DataBind()
            If Not Documents.IsNullOrEmpty() Then
                DocumentListGrid.DataSource = Documents
                DocumentListGrid.DataBind()
            Else
                DocumentsPanel.Visible = False
            End If

            MessageRecipients.DataSource = Recipients
            MessageRecipients.DataBind()
        End Sub

        Private Sub CheckSenderEmail()
            If String.IsNullOrEmpty(SenderEmailValue) Then
                If DocSuiteContext.Current.ProtocolEnv.UserLogEmail Then
                    SenderEmailValue = Facade.UserLogFacade.EmailOfUser(DocSuiteContext.Current.User.UserName, DocSuiteContext.Current.User.Domain, True)
                    Exit Sub
                End If
                BindWarningPanel("Non è presente un indirizzo email valido per il mittente.")
            End If
        End Sub

        Private Sub BindWarningPanel(errorMessage As String)
            If Not String.IsNullOrEmpty(errorMessage) Then
                WarningLabel.Text = errorMessage
                WarningPanel.CssClass = "warningAreaLow"
            End If
        End Sub

        Private Overloads Sub BindAutorizzazioniCc()
            BindAutorizzazioniCc(uscSettori.GetRoles(), uscSettori.CurrentRoleUserViewMode)
        End Sub

        Private Overloads Sub BindAutorizzazioniCc(ByVal roleUserViewMode As uscSettori.RoleUserViewMode)
            BindAutorizzazioniCc(Me.uscSettori.GetRoles(), roleUserViewMode)
        End Sub
        Private Overloads Sub BindAutorizzazioniCc(ByVal roles As IEnumerable(Of Role), ByVal roleUserViewMode As uscSettori.RoleUserViewMode?)
            If Not ProtocolEnv.IsDistributionEnabled Then
                Exit Sub
            End If

            uscSettori.Checkable = True
            uscSettori.TreeViewControl.CheckBoxes = True
            uscSettori.Required = False

            uscSettori.CurrentRoleUserViewMode = roleUserViewMode

            uscSettori.SourceRoles = CType(roles, List(Of Role))
        End Sub

        ' Concatena i settori selezionati in una unica stringa con il separatore
        Private Function ConcatSelectedRoles() As String
            Dim settori As System.Text.StringBuilder = New Text.StringBuilder()
            Dim roles As IList(Of Role) = Me.uscSettori.GetRoles()
            If roles.IsNullOrEmpty() Then
                Return String.Empty
            End If

            ' Inserimento del settore per il quale invio la mail
            For Each item As Role In roles
                settori.Append(item.Name)
                If item.Id <> roles(roles.Count - 1).Id Then
                    settori.Append(", ")
                End If
            Next

            Return settori.ToString()
        End Function

        Private Function CheckAttachmentsSize() As Boolean
            Dim attachmentsSize As Long = CurrentOriginalAttachments.Sum(Function(s) s.Size)
            attachmentsSize += CurrentPdfAttachments.Sum(Function(s) s.Size)

            Dim maxSize As Long = ProtocolEnv.MessageMaxSize
            If attachmentsSize < maxSize OrElse Not maxSize > 0 Then
                'La dimensione degli allegati è inferiore a quella impostata in ParamEnv
                'oppure in ParamEnv è stato impostato 0
                Return True
            End If

            'La dimensione degli allegati da spedire supera la dimensione massima
            BasePage.AjaxAlert(ProtocolEnv.MessageMaxSizeAlertMessage)
            Return False
        End Function

        Private Function CompressDocuments(password As String) As MemoryDocumentInfo
            Dim CompressManager As ICompress = New ZipCompress()
            Dim content As List(Of KeyValuePair(Of String, Byte())) = New List(Of KeyValuePair(Of String, Byte()))
            If CurrentOriginalAttachments.Any() Then
                content.AddRange(CurrentOriginalAttachments.Select(Function(d) New KeyValuePair(Of String, Byte())(String.Concat(String.Join(String.Empty, Path.GetFileNameWithoutExtension(d.Name).Take(240)), Path.GetExtension(d.Name)), d.Stream)))
            End If
            If CurrentPdfAttachments.Any() Then
                content.AddRange(CurrentPdfAttachments.Select(Function(d) New KeyValuePair(Of String, Byte())(String.Concat(String.Join(String.Empty, Path.GetFileNameWithoutExtension(d.PDFName).Take(240)), Path.GetExtension(d.PDFName)), d.GetPdfStream())))
            End If
            Dim zipFolderCompressedStream As Byte() = CompressManager.InMemoryCompress(content, True, password)
            Return New MemoryDocumentInfo(zipFolderCompressedStream, "File protetti.zip")
        End Function

        Private Sub SendMailWithPassword(protectedContacts As List(Of MessageContactEmail), text As String, body As String, Optional signatureBelow As String = "")
            Dim emailWithPassword As MessageEmail = Facade.MessageEmailFacade.CreateEmailMessage(protectedContacts, text, body, False, signatureBelow)
            Dim idmessage As Integer = Facade.MessageEmailFacade.SendEmailMessage(emailWithPassword)
            FileLogger.Info(LoggerName, String.Concat("MailSenderControl - SendMailWithPassword - Mail inserita in coda di invio [id ", idmessage, " ]"))
        End Sub

        Private Sub SecuredDocumentsChecked()
            If Not ChkCompressedDocuments.Visible Then
                Return
            End If
            If ChkSecuredDocuments.Checked Then
                ChkCompressedDocuments.Checked = True
            Else
                ChkCompressedDocuments.Checked = False
            End If
        End Sub

#End Region

    End Class
End Namespace