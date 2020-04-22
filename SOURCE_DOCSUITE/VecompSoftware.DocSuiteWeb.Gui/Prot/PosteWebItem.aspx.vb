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
        Public Const TypeSerc As String = "SE"
        Public Const ExtendedInformationSercKey = "SercInformation"
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
                InitializePOLAccounts()
            End If
        End Sub

        Protected Sub ddlPolAccount_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlPolAccount.SelectedIndexChanged
            cmdSend.Enabled = HasSelectedPOLAccount
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

                If uscProtocollo.ControlRecipients.TreeViewControl.Nodes(0).Nodes.Count > 0 AndAlso Not uscProtocollo.ControlRecipients.TreeViewControl.Nodes(0).Nodes(0).Checkable Then
                    uscProtocollo.ControlRecipients.TreeViewControl.Nodes(0).Nodes(0).Checked = False
                End If

                ' Imposto i destinatari.
                Select Case uscProtocollo.ControlRecipients.GetSelectedContacts.Count
                    Case 0
                        AjaxAlert("Specificare almeno un destinatario per l'invio")
                        Exit Sub
                        Exit Select

                    Case > 0
                        Dim invalidAddressRecipient As String = ""
                        For Each recipient As ContactDTO In uscProtocollo.ControlRecipients.GetSelectedContacts()
                            If Not (recipient.Contact IsNot Nothing AndAlso recipient.Contact.Address.IsValidAddress) Then
                                invalidAddressRecipient += ControlChars.CrLf + recipient.Contact.Description.ToString + "; "
                            End If

                            If (invalidAddressRecipient.IsNullOrEmpty) Then
                                SetPOLRequestRecipient(request, recipient)
                            End If
                        Next

                        If Not (invalidAddressRecipient.IsNullOrEmpty) Then
                            AjaxAlert("Errore indirizzo destinatario presente in: " + invalidAddressRecipient)
                            Exit Sub
                        End If
                        Exit Select

                End Select

                If CurrentType.Eq(TypeLettera) OrElse CurrentType.Eq(TypeRaccomandata) OrElse CurrentType.Eq(TypeSerc) Then
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
                    rq.IdArchiveChain = chain.ArchiveInBiblos(posteWebLocation.DocumentServer, posteWebLocation.ProtBiblosDSDB)

                    Dim md5Service As MD5CryptoServiceProvider = New MD5CryptoServiceProvider()
                    rq.DocumentMD5 = BitConverter.ToString(md5Service.ComputeHash(mergedDocuments.MergedDocument.Stream)).Replace("-", String.Empty)
                    rq.DocumentName = "Documento principale.pdf"
                ElseIf CurrentType.Eq(TypeTelegramma) Then
                    Dim rq As TOLRequest = DirectCast(request, TOLRequest)
                    rq.Testo = txtMessage.Text
                End If

                Facade.PosteOnLineRequestFacade.SaveAll(request)
                Response.Redirect("ProtVisualizza.aspx?" & CommonShared.AppendSecurityCheck(String.Format("Year={0}&Number={1}", CurrentProtocol.Year, CurrentProtocol.Number)))
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
            If Not CurrentProtocolRights.IsPecSendable Then
                Throw New DocSuiteException("Protocollo n. " & CurrentProtocol.FullNumber, "Mancano diritti di Autorizzazione sul protocollo", Request.Url.ToString(), DocSuiteContext.Current.User.FullUserName)
            End If

            ' i protocolli in ingresso non sono spedibili via interoperabilità
            If CurrentProtocol.Type.Id = -1 Then
                Throw New DocSuiteException("Protocollo n. " & CurrentProtocol.FullNumber, "I protocolli in ingresso non possono essere inviati attraverso l'interoperabilità", Request.Url.ToString(), DocSuiteContext.Current.User.FullUserName)
            End If

            If Not DocSuiteContext.Current.ProtocolEnv.IsInteropEnabled Then
                Throw New DocSuiteException("Protocollo n. " & CurrentProtocol.FullNumber, "Interoperabilità non abilitata", Request.Url.ToString(), DocSuiteContext.Current.User.FullUserName)
            End If

            uscProtocollo.VisibleProtocollo = True
            uscProtocollo.VisibleMittentiDestinatari = True
            uscProtocollo.VisibleMittentiButtonDelete = True
            uscProtocollo.VisibleDestinatariButtonDelete = True

            Dim senders As IList(Of ContactDTO) = CurrentProtocol.GetSenders()
            If Not senders.IsNullOrEmpty() Then
                uscProtocollo.ContactMittenteModifyEnable = False
                uscProtocollo.ControlSenders.EnableCheck = True
            Else
                uscProtocollo.ContactMittenteModifyEnable = True
                uscProtocollo.ControlSenders.IsRequired = False
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
        End Sub

        Private Sub InitializePOLAccounts()
            Dim accounts As IList(Of POLAccount) = Facade.PosteOnLineAccountFacade.GetUserAccounts()
            ddlPolAccount.DataSource = accounts
            ddlPolAccount.DataBind()
            cmdSend.Enabled = True
            If accounts.Count > 1 Then
                cmdSend.Enabled = False
                ddlPolAccount.Items.Insert(0, New ListItem("Selezionare Account PosteWeb", ""))
            End If
        End Sub

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
            Select Case CurrentType
                Case TypeLettera
                    request = New LOLRequest()
                Case TypeRaccomandata
                    request = New ROLRequest()
                Case TypeTelegramma
                    request = New TOLRequest()
                Case TypeSerc
                    request = New SOLRequest()
                Case Else
                    Throw New DocSuiteException(String.Format("POLRequest {0} non valido.", CurrentType))
            End Select

            request.Id = Guid.NewGuid()
            request.Status = POLRequestStatusEnum.RequestQueued
            request.StatusDescrition = "In attesa di Invio a Poste Online"
            request.ProtocolYear = CurrentProtocol.Year
            request.ProtocolNumber = CurrentProtocol.Number
            request.Account = Facade.PosteOnLineAccountFacade.GetById(Integer.Parse(ddlPolAccount.SelectedValue))
            request.RegistrationDate = Date.Now
            request.RegistrationUser = DocSuiteContext.Current.User.FullUserName

            Return request
        End Function

        Private Sub SetPOLRequestSender(ByRef request As POLRequest, sender As ContactDTO)
            request.Sender = New POLRequestSender()

            If (CurrentType.Eq(TypeSerc)) Then
                request.Sender = PopulateRecipientInformationForSercService(sender, request.Sender)
            End If

            request.Sender.Id = Guid.NewGuid()
            request.Sender.Name = sender.Contact.DescriptionFormatByContactType
            Facade.PosteOnLineContactFacade.RecursiveSetSenderAddress(request.Sender, sender.Contact)
            request.Sender.PhoneNumber = String.Format("{0}", sender.Contact.TelephoneNumber).Trim()
            request.Sender.RegistrationDate = Date.Now
            request.Sender.Request = request
            request.Sender.RegistrationUser = DocSuiteContext.Current.User.FullUserName
        End Sub

        Private Sub SetPOLRequestRecipient(ByRef request As POLRequest, recipient As ContactDTO)
            Dim polRecipient As POLRequestRecipient = New POLRequestRecipient()
            polRecipient.Id = Guid.NewGuid()
            polRecipient.Name = recipient.Contact.DescriptionFormatByContactType
            Facade.PosteOnLineContactFacade.RecursiveSetRecipientAddress(polRecipient, recipient.Contact)
            polRecipient.PhoneNumber = String.Format("{0}", recipient.Contact.TelephoneNumber).Trim()
            polRecipient.RegistrationDate = Date.Now
            polRecipient.RegistrationUser = DocSuiteContext.Current.User.FullUserName
            polRecipient.Status = POLMessageContactEnum.Created
            polRecipient.StatusDescrition = "In Attesa di Invio"
            polRecipient.Request = request

            request.Recipients.Add(polRecipient)
        End Sub

        Private Function PopulateRecipientInformationForSercService(senderDto As ContactDTO, polSender As POLRequestSender) As POLRequestSender
            Dim extendedSercProperties As New POLRequestContactSercExtendedProperties
            extendedSercProperties.Denominazione = senderDto.Contact?.ContactType?.Description
            extendedSercProperties.Cognome = senderDto.Contact?.FirstName
            extendedSercProperties.Nome = senderDto.Contact?.LastName
            extendedSercProperties.Telefono = senderDto.Contact?.TelephoneNumber

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

            polSender.TrySetExtendedProperties(extendedProperties)
            polSender.LastChangedDate = DateTime.Now
            Return polSender
        End Function
#End Region

    End Class
End Namespace
