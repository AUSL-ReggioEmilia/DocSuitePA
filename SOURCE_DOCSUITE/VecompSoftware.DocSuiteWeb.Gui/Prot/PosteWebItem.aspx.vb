Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports System.Linq
Imports System.Security.Cryptography
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports System.IO
Imports Telerik.Web.UI
Imports System.Text

Namespace Prot.PosteWeb
    Partial Class Item
        Inherits ProtBasePage

#Region " Fields "
        Public Const TypeLettera As String = "LT"
        Public Const TypeRaccomandata As String = "RC"
        Public Const TypeTelegramma As String = "TG"
        Public Const TypeTNotice As String = "SE"
        Public Const ExtendedInformationSercKey As String = "SercInformation"
        Private Const ViewStatePolAccDenKey As String = "polAccount[{0}].Denominazionni"
        Private Const PROTOCOL_MERGED_DOCUMENT_NAME As String = "Documento principale.pdf"
#End Region

#Region " Properties "
        Public Property ValidationResult() As SegnaturaValidationResult
            Get
                If ViewState("ValidationResult") IsNot Nothing Then
                    Return DirectCast(ViewState("ValidationResult"), SegnaturaValidationResult)
                End If

                Return New SegnaturaValidationResult()
            End Get
            Set(ByVal value As SegnaturaValidationResult)
                ViewState("ValidationResult") = value
            End Set
        End Property

        Private ReadOnly Property CurrentType As String
            Get
                Return Request.QueryString("PType")
            End Get
        End Property

        Private ReadOnly Property HasSelectedPOLAccount As Boolean
            Get
                Return ddlPolAccount.SelectedItem IsNot Nothing AndAlso ((ddlPolAccount.Items.Count > 1 AndAlso ddlPolAccount.SelectedIndex > 0) OrElse (ddlPolAccount.Items.Count = 1 AndAlso ddlPolAccount.SelectedIndex = 0))
            End Get
        End Property

        Private ReadOnly Property SelectedSenders As IList(Of ContactDTO)
            Get
                Return uscProtocollo.ControlSenders.GetSelectedContacts.Where(Function(f) f.Contact IsNot Nothing AndAlso f.Contact.Address IsNot Nothing AndAlso f.Contact.Address.IsValidAddress).ToList()
            End Get
        End Property

        Private ReadOnly Property SelectedRecipients As IList(Of ContactDTO)
            Get
                Return uscProtocollo.ControlRecipients.GetSelectedContacts().Where(Function(f) f.Contact IsNot Nothing AndAlso f.Contact.Address IsNot Nothing AndAlso Not String.IsNullOrEmpty(f.Contact.Address.Address)).ToList()
            End Get
        End Property

#End Region

#Region " Events "
        Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            InitializeAjax()

            If Not IsPostBack Then
                Initialize()
                InitializeProtocolControl()

                If CurrentType.Eq(TypeTNotice) Then
                    'this will also save pol account names in viewstate
                    InitializeTNoticeAccount()
                    'this nees the pol account names from the viewstate, so it has to be called after
                    CheckSelectedPolAccountState()
                    Me.uscProtocollo.ControlSenders.InnerPanelButtons.Visible = False
                Else
                    InitializePOLAccounts()
                End If

            End If
        End Sub

        Protected Sub ddlPolAccount_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlPolAccount.SelectedIndexChanged
            CheckSelectedPolAccountState()
        End Sub

        Private Sub CheckSelectedPolAccountState()
            cmdSend.Enabled = HasSelectedPOLAccount
            uscProtocollo.CurrentProtocol = CurrentProtocol

            If CurrentType.Eq(TypeTNotice) Then
                If (ddlPolAccount.SelectedIndex > 0 AndAlso CInt(ddlPolAccount.SelectedValue) > -1) Then
                    'get the PollACcount Denominazioni from the view stast
                    Dim polAccDenominazioni As String = CStr(ViewState.Item(String.Format(ViewStatePolAccDenKey, CInt(ddlPolAccount.SelectedValue))))
                    _sender_denominazioni.Text = polAccDenominazioni
                Else
                    _sender_denominazioni.Text = "Nessun Account Selezionata"
                End If
            End If
            'THe selected value is the id of a PolAcc if it's greated then -1
        End Sub

        Protected Sub uscProtocollo_MittenteAdded(ByVal sender As Object, ByVal e As EventArgs) Handles uscProtocollo.MittenteAdded
            uscProtocollo.ControlRecipients.EnableCheck = True
            uscProtocollo.ControlRecipients.CheckAll()
            uscProtocollo.ControlSenders.EnableCheck = True
            uscProtocollo.ControlSenders.CheckAll()
        End Sub

        Private Sub cmdSend_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSend.Click
            Try
                If (CurrentType.Eq(TypeTNotice)) Then
                    SaveTNotice()
                Else
                    SavePosteWeb()
                End If
                Response.Redirect($"ProtVisualizza.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={CurrentProtocol.Id}&Type=Prot")}")
            Catch ex As Exception
                FileLogger.Error(LoggerName, "Errore durante la fase di invio", ex)
                AjaxAlert(String.Format("Errore durante la fase di invio: {0}", ex.Message))
                Exit Sub
            End Try
        End Sub

        Protected Sub chkMultipleTNotice_CheckedChanged(sender As Object, e As EventArgs) Handles chkMultipleTNotice.CheckedChanged
            uscProtocollo.ControlRecipients.SingleCheck = Not chkMultipleTNotice.Checked
        End Sub
#End Region

#Region " Methods "
        Private Sub Initialize()
            uscProtocollo.CurrentProtocol = CurrentProtocol
            uscProtocollo.ControlRecipients.EnableCompression = False
            uscProtAttachs.EnableCheckedDocumentSelection = Not CurrentType.Eq(TypeTelegramma)

            tblMessagge.Visible = CurrentType.Eq(TypeTelegramma)

            pnlTNoticeMessage.Visible = CurrentType.Eq(TypeTNotice)
            pnlTNoticeObject.Visible = CurrentType.Eq(TypeTNotice)
            pnlTNoticeOptions.Visible = CurrentType.Eq(TypeTNotice)
            txtTNoticeObject.Text = CurrentProtocol.ProtocolObject.Truncate(100) '100 is the limitation from TNotice

            'documenti
            uscProtDocument.LoadDocumentInfo(ProtocolFacade.GetDocument(CurrentProtocol), False, False, False, False)
            'allegati
            If CurrentProtocol.IdAttachments.HasValue AndAlso CurrentProtocol.IdAttachments > 0 Then
                uscProtAttachs.LoadDocumentInfo(ProtocolFacade.GetAttachments(CurrentProtocol), True, False, False, False)
                If uscProtAttachs.TreeViewControl.Nodes(0).Nodes.Count > 0 Then
                    For Each node As RadTreeNode In uscProtAttachs.TreeViewControl.Nodes(0).Nodes
                        node.Checked = ProtocolEnv.PosteWEBAttachmentsChecked
                    Next
                End If
            End If
        End Sub

        Private Sub InitializeAjax()
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdSend, pnlContent, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdSend, cmdSend, MasterDocSuite.AjaxFlatLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlPolAccount, cmdSend, MasterDocSuite.AjaxFlatLoadingPanel)
            If (CurrentType.Eq(TypeTNotice)) Then
                AjaxManager.AjaxSettings.AddAjaxSetting(chkMultipleTNotice, uscProtocollo.ControlRecipients.TreeViewControl)
            End If
        End Sub

        Private Sub InitializeProtocolControl()

            'verifica Protocollo
            If (CurrentType.Eq(TypeTNotice) AndAlso Not CurrentProtocolRights.IsTNoticeSendable) OrElse (Not CurrentType.Eq(TypeTNotice) AndAlso Not CurrentProtocolRights.IsPECSendable) Then
                Throw New DocSuiteException($"Protocollo n. {CurrentProtocol.FullNumber}", "Mancano diritti di Autorizzazione sul protocollo", Request.Url.ToString(), DocSuiteContext.Current.User.FullUserName)
            End If

            ' i protocolli in ingresso non sono spedibili via interoperabilità
            If CurrentProtocol.Type.Id = -1 Then
                Throw New DocSuiteException($"Protocollo n. {CurrentProtocol.FullNumber}", "I protocolli in ingresso non possono essere inviati attraverso l'interoperabilità", Request.Url.ToString(), DocSuiteContext.Current.User.FullUserName)
            End If

            If Not DocSuiteContext.Current.ProtocolEnv.IsInteropEnabled Then
                Throw New DocSuiteException($"Protocollo n. {CurrentProtocol.FullNumber}", "Interoperabilità non abilitata", Request.Url.ToString(), DocSuiteContext.Current.User.FullUserName)
            End If

            lblTNoticeSender.Visible = False
            lblTNoticeSenderDescription.Visible = False
            If CurrentType.Eq(TypeTNotice) Then
                Page.Title = "Protocollo - Gestione invio TNotice"
                lblAccountName.Text = "Account TNotice"
                lblTNoticeSender.Visible = True
                lblTNoticeSenderDescription.Visible = True
            End If

            uscProtocollo.VisibleProtocollo = True
            uscProtocollo.VisibleMittentiDestinatari = True
            uscProtocollo.VisibleMittentiButtonDelete = True
            uscProtocollo.VisibleDestinatariButtonDelete = True

            Dim senders As IList(Of ContactDTO) = CurrentProtocol.GetSenders()
            uscProtocollo.ContactMittenteModifyEnable = True
            uscProtocollo.ControlSenders.IsRequired = False
            If Not senders.IsNullOrEmpty() Then
                uscProtocollo.ContactMittenteModifyEnable = False
                uscProtocollo.ControlSenders.EnableCheck = True
            End If
            uscProtocollo.ControlRecipients.EnableCheck = True

            If CurrentType.Eq(TypeTNotice) Then
                Dim recipients As ICollection(Of ContactDTO) = CurrentProtocol.GetRecipients()
                chkMultipleTNotice.Checked = recipients.Count > 1
                uscProtocollo.ControlRecipients.SingleCheck = Not chkMultipleTNotice.Checked
            Else
                uscProtocollo.ControlRecipients.CheckAll()
            End If

            uscProtocollo.VisibleOggetto = True
            ' uscProtocollo.VisiblePEC = False
            uscProtocollo.VisibleProtocolloMittente = False
            uscProtocollo.VisibleAltri = False
            uscProtocollo.VisibleClassificazione = False
            uscProtocollo.VisibleFascicolo = False
            uscProtocollo.VisibleStatoProtocollo = False
            uscProtocollo.VisibleTipoDocumento = False
            uscProtocollo.VisibleFatturazione = False
            uscProtocollo.VisibleAssegnatario = False
            uscProtocollo.VisibleScatolone = False
            uscProtocollo.VisibleHandler = False
        End Sub

        Private Sub InitializePOLAccounts()
            Dim accounts As IList(Of POLAccount) = Facade.PosteOnLineAccountFacade.GetUserAccounts(CurrentTenant.TenantAOO.UniqueId)
            ddlPolAccount.DataSource = accounts
            ddlPolAccount.DataBind()
            cmdSend.Enabled = accounts.Any()
            If accounts.Count > 1 Then
                cmdSend.Enabled = False
                ddlPolAccount.Items.Insert(0, New ListItem("Selezionare account PosteWeb", ""))
            End If
        End Sub

        Private Sub InitializeTNoticeAccount()
            If CurrentType.Eq(TypeTNotice) Then
                cmdSend.Enabled = False
                Dim accounts As IList(Of POLAccount) = Facade.PosteOnLineAccountFacade.GetUserAccounts(CurrentTenant.TenantAOO.UniqueId)
                Dim filtered As IList(Of POLAccount) = New List(Of POLAccount)()

                For Each polAcc As POLAccount In accounts
                    Dim ep As POLAccountExtendedProperties = polAcc.TryGetExtendedProperties()
                    If ep IsNot Nothing Then
                        ViewState.Add(String.Format(ViewStatePolAccDenKey, polAcc.Id), ep.SercExtra.Mitente.Denominazione)
                        filtered.Add(polAcc)
                    End If
                Next

                ddlPolAccount.DataSource = filtered
                ddlPolAccount.DataBind()
                cmdSend.Enabled = filtered.Any()

                If accounts.Count > 1 Then
                    ddlPolAccount.Items.Insert(0, New ListItem("Selezionare Account TNotice", ""))
                End If
            End If
        End Sub

        Private Function InitializeSercExtraProperties(ByRef request As POLRequest) As Boolean
            Dim extendedProperties As New POLRequestExtendedProperties
            extendedProperties.MetaData = New POLRequestMetaData
            extendedProperties.MetaData.PolAccountName = request.Account.Name

            Dim selPolAccId As Integer = CInt(ddlPolAccount.SelectedValue)

            If (selPolAccId = 0) Then
                AjaxAlert("Nessun Account Selezionato")
                Return False
            End If

            Dim polAccSenderNameValue As String = CStr(ViewState.Item(String.Format(ViewStatePolAccDenKey, selPolAccId)))
            extendedProperties.MetaData.PolRequestContactName = polAccSenderNameValue

            request.TrySetExtendedProperties(extendedProperties)
            Return True
        End Function

        ''' <summary> Aggiorna il protocollo corrente con il mittente specificato. </summary>
        ''' TODO: Siamo sicuri che sia corretto eliminare il mittente di un Protocollo già inserito? Si potrebbe rimuovere il metodo.
        Private Sub UpdateSingleSender(sender As ContactDTO)
            ' Elimino tutti i mittenti (sia da rubrica che manuali) abbinati al protocollo corrente.
            Dim currentProtocolContacts As New List(Of ProtocolContact)(CurrentProtocol.Contacts)
            For Each pc As ProtocolContact In currentProtocolContacts
                If pc.ComunicationType.Eq(ProtocolContactCommunicationType.Sender) Then
                    CurrentProtocol.Contacts.Remove(pc)
                End If
            Next
            Dim currentProtocolManualContacts As New List(Of ProtocolContactManual)(CurrentProtocol.ManualContacts)
            For Each pcm As ProtocolContactManual In currentProtocolManualContacts
                If pcm.ComunicationType.Eq(ProtocolContactCommunicationType.Sender) Then
                    CurrentProtocol.ManualContacts.Remove(pcm)
                End If
            Next
            Facade.ProtocolFacade.UpdateOnly(CurrentProtocol)

            ' Aggiungo il nuovo mittente.
            Select Case sender.Type
                Case ContactDTO.ContactType.Address
                    CurrentProtocol.AddSender(sender.Contact, sender.IsCopiaConoscenza)
                Case ContactDTO.ContactType.Manual
                    CurrentProtocol.AddSenderManual(sender.Contact, sender.IsCopiaConoscenza)
            End Select
            Facade.ProtocolFacade.UpdateOnly(CurrentProtocol)
        End Sub

        Private Function GetPOLRequestByType() As POLRequest
            Dim request As POLRequest = Nothing
            Dim requestStatusDescrition As String = "Poste Online"
            Select Case CurrentType
                Case TypeLettera
                    request = New LOLRequest()
                Case TypeRaccomandata
                    request = New ROLRequest()
                Case TypeTelegramma
                    request = New TOLRequest()
                Case TypeTNotice
                    request = New SOLRequest()
                    requestStatusDescrition = "TNotice"
                Case Else
                    Throw New DocSuiteException(String.Format("POLRequest {0} non valido.", CurrentType))
            End Select

            request.Id = Guid.NewGuid()
            request.Status = POLRequestStatusEnum.RequestQueued
            request.StatusDescrition = $"In attesa di invio a {requestStatusDescrition}"
            request.DocumentUnit = Facade.DocumentUnitFacade.GetById(CurrentProtocol.Id)
            request.Account = Facade.PosteOnLineAccountFacade.GetById(Integer.Parse(ddlPolAccount.SelectedValue))
            request.RegistrationDate = Date.UtcNow
            request.RegistrationUser = DocSuiteContext.Current.User.FullUserName

            Return request
        End Function

        Private Sub SetPOLRequestSender(ByRef request As POLRequest, sender As ContactDTO)
            request.Sender = New POLRequestSender()
            request.Sender.Id = Guid.NewGuid()
            request.Sender.Name = sender.Contact.DescriptionFormatByContactType
            Facade.PosteOnLineContactFacade.RecursiveSetSenderAddress(request.Sender, sender.Contact)
            request.Sender.PhoneNumber = String.Format("{0}", sender.Contact.TelephoneNumber).Trim()
            request.Sender.RegistrationDate = DateTimeOffset.UtcNow
            request.Sender.Request = request
            request.Sender.RegistrationUser = DocSuiteContext.Current.User.FullUserName
        End Sub

        Private Sub SetPOLRequestRecipient(ByRef request As POLRequest, recipient As ContactDTO)

            Dim polRecipient As POLRequestRecipient = New POLRequestRecipient()
            polRecipient.Id = Guid.NewGuid()
            polRecipient.Name = recipient.Contact.DescriptionFormatByContactType
            Facade.PosteOnLineContactFacade.TryRecursiveSetRecipientAddress(polRecipient, recipient.Contact)
            polRecipient.PhoneNumber = String.Format("{0}", recipient.Contact.TelephoneNumber).Trim()
            polRecipient.RegistrationDate = DateTimeOffset.UtcNow
            polRecipient.RegistrationUser = DocSuiteContext.Current.User.FullUserName
            polRecipient.Status = POLMessageContactEnum.Created
            polRecipient.StatusDescrition = "In Attesa di Invio"

            polRecipient.Request = request
            If (CurrentType.Eq(TypeTNotice)) Then
                polRecipient = PopulateRecipientInformationForSercService(recipient, polRecipient)
            End If

            request.Recipients.Add(polRecipient)
        End Sub

        Private Function PopulateRecipientInformationForSercService(senderDto As ContactDTO, receiver As POLRequestRecipient) As POLRequestRecipient
            Dim extendedSercProperties As New POLRequestContactSercExtendedProperties
            extendedSercProperties.Denominazione = senderDto.Contact?.DescriptionFormatByContactType
            extendedSercProperties.Nome = String.Empty ' campo vuoto per Tnotice referente aziendale
            extendedSercProperties.Cognome = String.Empty '  campo vuoto  per Tnotice referente aziendale
            extendedSercProperties.Telefono = senderDto.Contact?.TelephoneNumber
            extendedSercProperties.Email = senderDto.Contact?.EmailAddress
            extendedSercProperties.Sede = New PRCESede
            extendedSercProperties.Sede.Nazione = senderDto.Contact?.Address?.Nationality
            extendedSercProperties.Sede.Civico = senderDto.Contact?.Address?.CivicNumber
            extendedSercProperties.Sede.CAP = senderDto.Contact?.Address?.ZipCode
            extendedSercProperties.Sede.Comune = senderDto.Contact?.Address?.City
            extendedSercProperties.Sede.Indirizzio = senderDto.Contact?.Address?.Address
            extendedSercProperties.Sede.Provincia = senderDto.Contact?.Address?.CityCode

            extendedSercProperties.ProtocolInfo = New PRCEProtocolInformation
            extendedSercProperties.ProtocolInfo.Subject = txtTNoticeObject.Text
            extendedSercProperties.ProtocolInfo.Note = txtTNoticeMessage.GetHtml(EditorStripHtmlOptions.None)

            Dim extendedProperties As New POLRequestContactExtendedProperties
            extendedProperties.SercExtra = extendedSercProperties

            receiver.TrySetExtendedProperties(extendedProperties)
            receiver.LastChangedDate = DateTimeOffset.UtcNow

            Return receiver
        End Function

        Private Sub SaveTNotice()
            If (String.IsNullOrEmpty(ddlPolAccount.SelectedValue)) Then
                Throw New DocSuiteException("Specificare un account TNotice per l'invio")
            End If

            Dim selectedRecipients As ICollection(Of ContactDTO) = uscProtocollo.ControlRecipients.GetSelectedContacts()
            If (selectedRecipients.Count = 0) Then
                Throw New DocSuiteException("Specificare almeno un destinatario per l'invio")
            End If

            If (Not chkMultipleTNotice.Checked AndAlso selectedRecipients.Count > 1) Then
                Throw New DocSuiteException("Non è stato specificato l'invio a più destinatari tra le opzioni di invio, è necessario quindi selezionare un unico destinatario")
            End If

            Dim posteWebLocation As Location = Facade.LocationFacade.GetById(ProtocolEnv.PosteWebRequestLocation)
            If (posteWebLocation Is Nothing) Then
                Throw New DocSuiteException("Attenzione! Nessuna Location definita per la gestione delle PosteWebOnline")
            End If

            Dim mergedDocument As DocumentInfo = CreateProtocolMergedDocument()
            Dim mergedDocumentMD5 As String = String.Empty
            Using md5Service As New MD5CryptoServiceProvider() 'Due to collision problems with MD5/SHA1, Microsoft recommends SHA256 or SHA512. Consider using the SHA256 class or the SHA512 class instead of the MD5 class.
                mergedDocumentMD5 = BitConverter.ToString(md5Service.ComputeHash(mergedDocument.Stream)).Replace("-", String.Empty)
            End Using

            If (selectedRecipients.Any(Function(x) x.Contact Is Nothing)) Then
                Throw New DocSuiteException("Errore nella verifica dei destinatari selezionati, alcuni elementi non hanno informazioni di contatto associate.")
            End If

            Dim request As SOLRequest = Nothing
            Dim recipientValidationErrors As String = Nothing
            Dim chain As BiblosChainInfo = Nothing
            For Each recipient As ContactDTO In selectedRecipients
                If (Not ValidateTNoticeRecipient(recipient, recipientValidationErrors)) Then
                    Throw New DocSuiteException(recipientValidationErrors)
                End If

                request = DirectCast(GetPOLRequestByType(), SOLRequest)
                request.DocumentUnit = Facade.DocumentUnitFacade.GetById(CurrentProtocol.Id)

                SetPOLRequestRecipient(request, recipient)

                chain = New BiblosChainInfo(New List(Of DocumentInfo) From {mergedDocument})
                request.IdArchiveChain = chain.ArchiveInBiblos(posteWebLocation.ProtBiblosDSDB)
                request.DocumentMD5 = mergedDocumentMD5
                request.DocumentName = PROTOCOL_MERGED_DOCUMENT_NAME

                If Not InitializeSercExtraProperties(request) Then
                    Throw New DocSuiteException("E' avvenuto un errore nell'inizializzazione delle proprietà per la richiesta TNotice.")
                End If

                Facade.PosteOnLineRequestFacade.SaveAll(request, True)
            Next
        End Sub

        Private Sub SavePosteWeb()
            If (String.IsNullOrEmpty(ddlPolAccount.SelectedValue)) Then
                Throw New DocSuiteException("Specificare un account per l'invio.")
            End If

            Dim selectedRecipients As ICollection(Of ContactDTO) = uscProtocollo.ControlRecipients.GetSelectedContacts()
            If (selectedRecipients.Count = 0) Then
                Throw New DocSuiteException("Specificare almeno un destinatario per l'invio")
            End If

            Dim posteWebLocation As Location = Facade.LocationFacade.GetById(ProtocolEnv.PosteWebRequestLocation)
            If (posteWebLocation Is Nothing) Then
                Throw New DocSuiteException("Attenzione! Nessuna Location definita per la gestione delle PosteWebOnline")
            End If

            'Inizializzo una nuova POLRequest
            Dim request As POLRequest = GetPOLRequestByType()
            request.DocumentUnit = Facade.DocumentUnitFacade.GetById(CurrentProtocol.Id)

            Dim selectedSenders As ICollection(Of ContactDTO) = uscProtocollo.ControlSenders.GetSelectedContacts()
            If (selectedSenders.IsNullOrEmpty() AndAlso (request.Account Is Nothing _
                    OrElse request.Account.DefaultContact Is Nothing _
                    OrElse request.Account.DefaultContact.Address Is Nothing _
                    OrElse Not request.Account.DefaultContact.Address.IsValidAddress)) Then

                Throw New DocSuiteException("Specificare un mittente valido per l'invio.")
            End If

            If (selectedSenders.Count > 1 AndAlso Me.SelectedSenders.Count > 1) Then
                Throw New DocSuiteException("Specificare un singolo mittente valido per l'invio.")
            End If

            Dim selectedSender As ContactDTO = CreatePosteWebSenderDTO(request, selectedSenders)
            SetPOLRequestSender(request, selectedSender)

            If uscProtocollo.ControlRecipients.TreeViewControl.Nodes(0).Nodes.Count > 0 AndAlso Not uscProtocollo.ControlRecipients.TreeViewControl.Nodes(0).Nodes(0).Checkable Then
                uscProtocollo.ControlRecipients.TreeViewControl.Nodes(0).Nodes(0).Checked = False
            End If

            Dim recipientValidationErrors As String = Nothing
            If (Not ValidatePosteWebRecipients(selectedRecipients, recipientValidationErrors)) Then
                Throw New DocSuiteException(recipientValidationErrors)
            End If

            For Each recipient As ContactDTO In selectedRecipients
                SetPOLRequestRecipient(request, recipient)
            Next

            If CurrentType.Eq(TypeLettera) OrElse CurrentType.Eq(TypeRaccomandata) Then
                Dim rq As LOLRequest = DirectCast(request, LOLRequest)
                Dim mergedDocument As DocumentInfo = CreateProtocolMergedDocument()
                Dim mergedDocumentMD5 As String = String.Empty
                Using md5Service As New MD5CryptoServiceProvider() 'Due to collision problems with MD5/SHA1, Microsoft recommends SHA256 or SHA512. Consider using the SHA256 class or the SHA512 class instead of the MD5 class.
                    mergedDocumentMD5 = BitConverter.ToString(md5Service.ComputeHash(mergedDocument.Stream)).Replace("-", String.Empty)
                End Using
                Dim chain As BiblosChainInfo = New BiblosChainInfo(New List(Of DocumentInfo) From {mergedDocument})
                rq.IdArchiveChain = chain.ArchiveInBiblos(posteWebLocation.ProtBiblosDSDB)
                rq.DocumentMD5 = mergedDocumentMD5
                rq.DocumentName = PROTOCOL_MERGED_DOCUMENT_NAME
            ElseIf CurrentType.Eq(TypeTelegramma) Then
                Dim rq As TOLRequest = DirectCast(request, TOLRequest)
                rq.Testo = txtMessage.Text
            End If

            Facade.PosteOnLineRequestFacade.SaveAll(request)
        End Sub

        Private Function CreateProtocolMergedDocument() As DocumentInfo
            Dim mergedDocuments As MergeDocumentResult = Facade.ProtocolFacade.GetMergedDocuments(CurrentProtocol, uscProtAttachs.CheckedDocumentInfos.Cast(Of BiblosDocumentInfo).ToList(), New List(Of BiblosDocumentInfo))
            If mergedDocuments.HasErrors Then
                Dim errorBuilder As StringBuilder = New StringBuilder()
                errorBuilder.AppendLine("Sono avvenuti degli errori nella gestione dei documenti da spedire:")
                For Each errorMessage As String In mergedDocuments.Errors
                    errorBuilder.AppendLine(errorMessage)
                Next
                Throw New DocSuiteException(errorBuilder.ToString())
            End If

            Return mergedDocuments.MergedDocument
        End Function

        Private Function ValidatePosteWebRecipients(selectedRecipients As ICollection(Of ContactDTO), ByRef errorMessage As String) As Boolean
            Dim errorBuilder As StringBuilder = New StringBuilder("E' avvenuto un errore nella validazione dei destinatari:").AppendLine()
            Dim hasError As Boolean = False
            For Each recipient As ContactDTO In selectedRecipients
                If (recipient.Contact Is Nothing OrElse recipient.Contact.Address Is Nothing OrElse Not recipient.Contact.Address.IsValidAddress) Then
                    errorBuilder.AppendLine(recipient.Contact.Description)
                    hasError = True
                End If
            Next

            If (hasError) Then
                errorMessage = errorBuilder.ToString()
            End If
            Return Not hasError
        End Function

        Private Function ValidateTNoticeRecipient(recipient As ContactDTO, ByRef errorMessage As String) As Boolean
            If recipient.Contact Is Nothing Then
                errorMessage = "Errore nella validazione del destinatario. Dati di contatto non trovati."
                Return False
            End If

            'determining if the contactType is a person or company
            Dim hasDescription As Boolean = Not recipient.Contact.Description.IsNullOrEmpty()
            Dim isPerson As Boolean = (hasDescription AndAlso recipient.Contact.Description.Contains("|"c) AndAlso recipient.Contact.ContactType.Equals(ContactType.Person)) OrElse Not hasDescription


            '2.8.5.1 Denominazione - Se compilato indica l'intestazione/ragione sociale del destinatario Obbligatorio se non è compilato il campo 2.8.5.3 e 2.8.5.4.
            '2.8.5.3 Nome - Se compilato è il Nome del destinatario (obbligatorio se non è compilato il campo 2.8.5.1). Se è compilato il campo 2.8.5.1 indica il Nome del referente.
            '2.8.5.4 Cognome - Se compilato è il Cognome del destinatario (obbligatorio se non è compilato il campo 2.8.5.1). Se è compilato il campo 2.8.5.1 indica il Cognome del referente.

            Dim errorBuilder As StringBuilder = New StringBuilder($"Errore nella validazione del destinatario {recipient.Contact.Description}:").AppendLine()
            Dim hasError As Boolean = False
            If (isPerson) Then
                If recipient.Contact.FirstName.IsNullOrEmpty() Then
                    errorBuilder.AppendLine("-- Il campo nome è richiesto")
                    hasError = True
                End If

                If recipient.Contact.LastName.IsNullOrEmpty() Then
                    errorBuilder.AppendLine("-- Il campo cognome è richiesto")
                    hasError = True
                End If
            Else
                If recipient.Contact.Description.IsNullOrEmpty() Then
                    errorBuilder.AppendLine("-- Il campo denominazione è richiesto")
                    hasError = True
                End If
            End If

            If (recipient.Contact.EmailAddress.IsNullOrEmpty()) Then
                errorBuilder.AppendLine("-- Il campo e-mail è richiesto")
                hasError = True
            End If

            If (hasError) Then
                errorMessage = errorBuilder.ToString()
            End If
            Return Not hasError
        End Function

        Private Function CreatePosteWebSenderDTO(request As POLRequest, selectedSenders As ICollection(Of ContactDTO)) As ContactDTO
            Select Case selectedSenders.Count
                Case 0
                    Return New ContactDTO(request.Account.DefaultContact, DirectCast(request.Account.DefaultContact.Id, ContactDTO.ContactType))
                Case > 1
                    Return Me.SelectedSenders.Single()
                Case Else
                    Return uscProtocollo.ControlSenders.GetSelectedContacts.Single()
            End Select
        End Function
#End Region

    End Class
End Namespace
