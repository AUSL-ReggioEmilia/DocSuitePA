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
                'Inizializzo una nuova POLRequest
                Dim request As POLRequest = GetPOLRequestByType()
                request.DocumentUnit = Facade.DocumentUnitFacade.GetById(CurrentProtocol.Id)
                'Sending TNotice does involve getting the sender from the table PosteOnLineAccount. This is added as a drop down list option
                'to the user. The sender (chosen in the first page when inserting a protocol) is not valid for TNotice

                If CurrentType.Eq(TypeTNotice) Then

                    Dim selPolAccId As Integer = CInt(ddlPolAccount.SelectedValue)

                    If (selPolAccId = 0) Then
                        AjaxAlert("Specificare un account TNotice per l'invio")
                    End If
                Else
                    Dim selectedSender As ContactDTO = Nothing

                    'Imposto i mittenti.
                    Select Case uscProtocollo.ControlSenders.GetSelectedContacts.Count
                        Case 0
                            If (request.Account IsNot Nothing AndAlso request.Account.DefaultContact IsNot Nothing AndAlso request.Account.DefaultContact.Address IsNot Nothing AndAlso request.Account.DefaultContact.Address.IsValidAddress) Then
                                selectedSender = New ContactDTO(request.Account.DefaultContact, request.Account.DefaultContact.Id)

                            Else AjaxAlert("Specificare un mittente per l'invio")
                                Exit Sub

                            End If
                            Exit Select
                        Case 1
                            selectedSender = uscProtocollo.ControlSenders.GetSelectedContacts.Single()
                            Exit Select
                        Case > 1
                            If SelectedSenders.Count = 1 Then
                                selectedSender = SelectedSenders.Single()
                            Else
                                AjaxAlert("Specifica un solo mittente")
                                Exit Sub
                            End If
                            Exit Select
                    End Select

                    Try
                        SetPOLRequestSender(request, selectedSender)
                    Catch ex As Exception
                        FileLogger.Warn(LoggerName, "Errore indirizzo mittente", ex)
                        AjaxAlert(ex.Message)
                        Exit Sub
                    End Try
                End If

                If uscProtocollo.ControlRecipients.TreeViewControl.Nodes(0).Nodes.Count > 0 AndAlso Not uscProtocollo.ControlRecipients.TreeViewControl.Nodes(0).Nodes(0).Checkable Then
                    uscProtocollo.ControlRecipients.TreeViewControl.Nodes(0).Nodes(0).Checked = False
                End If

                ' Imposto i destinatari.
                ' For TNotice the required fields are Name, Surname and Email
                Select Case uscProtocollo.ControlRecipients.GetSelectedContacts.Count
                    Case 0
                        AjaxAlert("Specificare almeno un destinatario per l'invio")
                        Exit Sub
                        Exit Select

                    Case > 0
                        If CurrentType.Eq(TypeTNotice) Then

                            Dim tnoticeSenderErrors As New List(Of String)
                            For Each recipient As ContactDTO In uscProtocollo.ControlRecipients.GetSelectedContacts()

                                If recipient.Contact Is Nothing Then
                                    AjaxAlert("dati di contatto del mittente non trovati")
                                    Exit Sub
                                End If

                                'determining if the contactType is a person or company
                                Dim hasDescription As Boolean = Not recipient.Contact.Description.IsNullOrEmpty()

                                Dim isPerson As Boolean = (hasDescription AndAlso recipient.Contact.Description.Contains("|"c) AndAlso recipient.Contact.ContactType.Equals(ContactType.Person)) OrElse Not hasDescription


                                '2.8.5.1 Denominazione - Se compilato indica l'intestazione/ragione sociale del destinatario Obbligatorio se non è compilato il campo 2.8.5.3 e 2.8.5.4.
                                '2.8.5.3 Nome - Se compilato è il Nome del destinatario (obbligatorio se non è compilato il campo 2.8.5.1). Se è compilato il campo 2.8.5.1 indica il Nome del referente.
                                '2.8.5.4 Cognome - Se compilato è il Cognome del destinatario (obbligatorio se non è compilato il campo 2.8.5.1). Se è compilato il campo 2.8.5.1 indica il Cognome del referente.

                                If recipient.Contact IsNot Nothing Then
                                    If (isPerson) Then
                                        If recipient.Contact.FirstName.IsNullOrEmpty() Then
                                            tnoticeSenderErrors.Add("il nome")
                                        End If

                                        If recipient.Contact.LastName.IsNullOrEmpty() Then
                                            tnoticeSenderErrors.Add("il cognome")
                                        End If
                                    Else
                                        If recipient.Contact.Description.IsNullOrEmpty() Then
                                            tnoticeSenderErrors.Add("la denominazione")
                                        End If
                                    End If

                                    If (recipient.Contact.EmailAddress.IsNullOrEmpty()) Then
                                        tnoticeSenderErrors.Add("l'e-mail")
                                    End If

                                    If tnoticeSenderErrors.Count > 0 Then
                                        AjaxAlert($"Per il destinatario {recipient.Contact.Description} mancano i seguenti campi: {String.Join(", ", tnoticeSenderErrors)}")
                                        Exit Sub
                                    End If

                                    If (tnoticeSenderErrors.IsNullOrEmpty) Then
                                        SetPOLRequestRecipient(request, recipient)
                                    End If
                                End If

                            Next

                        Else
                            Dim invalidAddressRecipient As String = ""
                            For Each recipient As ContactDTO In uscProtocollo.ControlRecipients.GetSelectedContacts()
                                If Not (recipient.Contact IsNot Nothing AndAlso recipient.Contact.Address IsNot Nothing AndAlso recipient.Contact.Address.IsValidAddress) Then
                                    invalidAddressRecipient += ControlChars.CrLf + recipient.Contact.Description.ToString + "; "
                                End If
                            Next

                            If Not (invalidAddressRecipient.IsNullOrEmpty) Then
                                AjaxAlert("Errore indirizzo destinatario presente in: " + invalidAddressRecipient)
                                Exit Sub
                            End If
                        End If

                        Exit Select

                End Select

                If CurrentType.Eq(TypeLettera) OrElse CurrentType.Eq(TypeRaccomandata) OrElse CurrentType.Eq(TypeTNotice) Then
                    Dim posteWebLocation As Location = Facade.LocationFacade.GetById(ProtocolEnv.PosteWebRequestLocation)
                    If posteWebLocation Is Nothing Then
                        AjaxAlert("Attenzione! Nessuna Location definita per la gestione delle PosteWebOnline")
                        FileLogger.Warn(LoggerName, "Nessuna Location definita per la gestione delle PosteWebOnline")
                        Exit Sub
                    End If
                    Dim rq As LOLRequest = DirectCast(request, LOLRequest)
                    Dim mergedDocuments As MergeDocumentResult = Facade.ProtocolFacade.GetMergedDocuments(CurrentProtocol, uscProtAttachs.CheckedDocumentInfos.Cast(Of BiblosDocumentInfo).ToList(), New List(Of BiblosDocumentInfo))
                    If mergedDocuments.HasErrors Then
                        Dim errorMessage As String = String.Concat("Sono avvenuti degli errori nella gestione dei documenti da spedire:", Environment.NewLine)
                        errorMessage += String.Join(Environment.NewLine, mergedDocuments.Errors.Select(Function(s) s))
                        AjaxAlert(errorMessage)
                        Exit Sub
                    End If

                    Dim protocolDocuments As List(Of DocumentInfo) = New List(Of DocumentInfo)
                    protocolDocuments.Add(mergedDocuments.MergedDocument)

                    Dim chain As BiblosChainInfo = New BiblosChainInfo(protocolDocuments)
                    rq.IdArchiveChain = chain.ArchiveInBiblos(posteWebLocation.ProtBiblosDSDB)

                    Dim md5Service As MD5CryptoServiceProvider = New MD5CryptoServiceProvider()
                    rq.DocumentMD5 = BitConverter.ToString(md5Service.ComputeHash(mergedDocuments.MergedDocument.Stream)).Replace("-", String.Empty)
                    rq.DocumentName = "Documento principale.pdf"
                ElseIf CurrentType.Eq(TypeTelegramma) Then
                    Dim rq As TOLRequest = DirectCast(request, TOLRequest)
                    rq.Testo = txtMessage.Text
                End If

                If CurrentType.Eq(TypeTNotice) Then
                    If Not InitializeSercExtraProperties(request) Then
                        Return
                    End If
                End If

                If CurrentType.Eq(TypeTNotice) Then
                    'The true flag excludes saving the sender which is by default included in the request
                    'in TNOtice context sender has null entries because it's not loaded 
                    Facade.PosteOnLineRequestFacade.SaveAll(request, True)
                Else
                    Facade.PosteOnLineRequestFacade.SaveAll(request)
                End If

                Response.Redirect($"ProtVisualizza.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={CurrentProtocol.Id}&Type=Prot")}")
            Catch ex As Exception
                FileLogger.Warn(LoggerName, "Errore durante la fase di invio", ex)
                AjaxAlert(String.Format("Errore durante la fase di invio: {0}", ex.Message))
                Exit Sub
            End Try
        End Sub
#End Region

#Region " Methods "
        Private Sub Initialize()
            uscProtocollo.CurrentProtocol = CurrentProtocol
            uscProtocollo.ControlRecipients.EnableCompression = False
            uscProtAttachs.EnableCheckedDocumentSelection = Not CurrentType.Eq(TypeTelegramma)

            tblMessagge.Visible = CurrentType.Eq(TypeTelegramma)

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
            uscProtocollo.ControlRecipients.CheckAll()

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
            uscProtocollo.VisibleMulticlassification = False
        End Sub

        Private Sub InitializePOLAccounts()
            Dim accounts As IList(Of POLAccount) = Facade.PosteOnLineAccountFacade.GetUserAccounts()
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
                Dim accounts As IList(Of POLAccount) = Facade.PosteOnLineAccountFacade.GetUserAccounts()
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
            request.RegistrationDate = Date.Now
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
            extendedSercProperties.ProtocolInfo.Subject = CurrentProtocol.ProtocolObject

            Dim extendedProperties As New POLRequestContactExtendedProperties
            extendedProperties.SercExtra = extendedSercProperties

            receiver.TrySetExtendedProperties(extendedProperties)
            receiver.LastChangedDate = DateTimeOffset.UtcNow

            Return receiver
        End Function
#End Region

    End Class
End Namespace
