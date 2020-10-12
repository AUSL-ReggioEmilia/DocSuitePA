Imports System.ComponentModel
Imports System.IO
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Facade.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade.PEC.Util
Imports VecompSoftware.Helpers.ExtensionMethods
Imports itextsharp.text
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Text
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.API
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.Compress
Imports VecompSoftware.Services.Command.CQRS.Commands.Entities.PECMails
Imports VecompSoftware.Services.Command.CQRS.Commands
Imports VecompSoftware.Core.Command
Imports VecompSoftware.DocSuiteWeb.EntityMapper.PECMails
Imports APIPECMail = VecompSoftware.DocSuiteWeb.Entity.PECMails
Imports VecompSoftware.Core.Command.CQRS.Commands.Entities.PECMails
Imports VecompSoftware.Core.Command.CQRS.Events.Entities.PECMails
Imports VecompSoftware.Services.Command.CQRS.Events
Imports VecompSoftware.Services.Command.CQRS
Imports VecompSoftware.Services.Command.CQRS.Events.Entities.PECMails
Imports VecompSoftware.Helpers.WebAPI
Imports VecompSoftware.DocSuiteWeb.Entity.Tenants

<DataObject()>
Public Class PECMailFacade
    Inherits BaseProtocolFacade(Of PECMail, Integer, NHibernatePECMailDao)

#Region " Fields "

    ''' <summary> Nuova riga </summary>
    Private Shared ReadOnly NewLine As New Paragraph(" ")
    Private _commandInsertFacade As CommandFacade(Of ICommandCreatePECMail)
    Private _mapperPECMailEntity As MapperPECMailEntity
    Private _webAPIHelper As WebAPIHelper

#End Region

#Region " Constructor "
    Public Sub New()
        MyBase.New()
    End Sub
#End Region

#Region " Properties "
    Private ReadOnly Property CommandInsertFacade As CommandFacade(Of ICommandCreatePECMail)
        Get
            If _commandInsertFacade Is Nothing Then
                _commandInsertFacade = New CommandFacade(Of ICommandCreatePECMail)
            End If
            Return _commandInsertFacade
        End Get
    End Property

    Private ReadOnly Property MapperPECMailEntity As MapperPECMailEntity
        Get
            If _mapperPECMailEntity Is Nothing Then
                _mapperPECMailEntity = New MapperPECMailEntity()
            End If
            Return _mapperPECMailEntity
        End Get
    End Property

    Private ReadOnly Property WebAPIHelper As WebAPIHelper
        Get
            If _webAPIHelper Is Nothing Then
                _webAPIHelper = New WebAPIHelper()
            End If
            Return _webAPIHelper
        End Get
    End Property
#End Region

    ''' <summary> Istanzia una PecMail in uscita. </summary>
    ''' <remarks> Include le logiche di creazione PEC. </remarks>
    Public Function InstantiateOutgoing(senders As String, recipients As String, subject As String, body As String, mailbox As PECMailBox) As PECMail
        Dim pecMail As New PECMail
        With pecMail
            .Direction = PECMailDirection.Outgoing
            .MailType = PECMailTypes.Invio
            .PECType = PECMailType.PEC
            .Segnatura = ""
            .MailPriority = CShort(PECMailPriority.Normale)
            .IsValidForInterop = False

            .MailSenders = senders
            .MailRecipients = recipients
            .MailSubject = subject
            .MailBody = body

            .MailBox = mailbox
            .Location = mailbox.Location
            .IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Delete)
        End With
        Return pecMail
    End Function

    Public Sub ArchiveInConservation(ByRef pec As PECMail, data() As Byte, name As String)
        Dim doc As New MemoryDocumentInfo(data, name)
        ArchiveInConservation(pec, doc, needTransaction:=False)
        doc.Dispose()
    End Sub

    Public Function Resend(ByRef pec As PECMail) As PECMail
        Try
            Dim parentPecMail As PECMail = Nothing
            If pec.SplittedFrom > 0 Then
                parentPecMail = Me.GetById(pec.SplittedFrom, False)
            End If
            Dim clonedPec As PECMail = pec.Clone(True, False, parentPecMail)

            clonedPec.MailDate = Nothing
            clonedPec.XRiferimentoMessageID = Nothing
            clonedPec.IDPostacert = Guid.Empty

            clonedPec.Direction = PECMailDirection.Outgoing

            clonedPec.Year = pec.Year
            clonedPec.Number = pec.Number
            clonedPec.DocumentUnit = pec.DocumentUnit
            clonedPec.TaskHeader = Nothing

            clonedPec.Receipts = Nothing
            Dim protocol As Protocol = FacadeFactory.Instance.ProtocolFacade.GetById(pec.DocumentUnit.Id)
            Dim contacts As List(Of Contact) = protocol.Contacts _
                .Where(Function(f) f.ComunicationType.Eq(ProtocolContactCommunicationType.Recipient)) _
                .Select(Function(f) f.Contact).ToList()
            Dim manualContacts As List(Of Contact) = protocol.ManualContacts _
                .Where(Function(f) f.ComunicationType.Eq(ProtocolContactCommunicationType.Recipient)) _
                .Select(Function(f) f.Contact).ToList()
            contacts.AddRange(manualContacts)
            clonedPec.MailRecipients = String.Join(";", contacts.Select(Function(r) Me.GetEmailAddress(r)))
            clonedPec.LogEntries = Nothing
            clonedPec.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Active)

            Me.Save(clonedPec)

            FacadeFactory.Instance.PECMailLogFacade.Created(clonedPec)
            FacadeFactory.Instance.PECMailLogFacade.Resend(pec, clonedPec)

            Return clonedPec
        Catch ex As Exception
            FileLogger.Error(LoggerName,
                String.Format("Si è verificato un errore in fase di clonazione della PEC MAIL con ID {0}", IIf(pec Is Nothing, -1, pec.Id)), ex)
            Throw ex
        End Try
    End Function

    Public Sub Duplicate(ByRef pec As PECMail, Optional ByVal resetProtocol As Boolean = False)
        Try
            If pec Is Nothing Then
                Throw New ArgumentException("PEC Mail inesistente")
            End If
            If pec.Direction = PECMailDirection.Outgoing Then
                Throw New ArgumentException("Si possono duplicare solo PEC in ingresso")
            End If

            Dim parentPecMail As PECMail = Nothing
            If pec.SplittedFrom > 0 Then
                parentPecMail = Me.GetById(pec.SplittedFrom, False)
            End If
            Dim clonedPec As PECMail = pec.Clone(True, True, parentPecMail)
            clonedPec.LogEntries = Nothing
            clonedPec.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Active)
            If resetProtocol Then
                clonedPec.Number = Nothing
                clonedPec.Year = Nothing
                clonedPec.DocumentUnit = Nothing
            End If

            Me.Save(clonedPec)

            FacadeFactory.Instance.PECMailLogFacade.Created(clonedPec)
        Catch ex As Exception
            FileLogger.Error(LoggerName,
                String.Format("Si è verificato un errore in fase di clonazione della PEC MAIL con ID {0}", IIf(pec Is Nothing, -1, pec.Id)), ex)
            Throw ex
        End Try
    End Sub

    Public Sub ArchiveInConservation(ByRef pec As PECMail, doc As DocumentInfo, Optional needTransaction As Boolean = True)
        Dim chain As New BiblosChainInfo(New List(Of DocumentInfo) From {doc})

        doc.AddAttribute("Anno", pec.RegistrationDate.Year.ToString())
        doc.AddAttribute("Data", pec.RegistrationDate.DefaultString())
        doc.AddAttribute("Direction", If(pec.Direction = PECMailDirection.Ingoing, "Ingresso", "Uscita"))
        doc.AddAttribute("Mitt_Dest", String.Concat("Mittenti : ", pec.MailSenders, ". Destinatari : ", pec.MailRecipients, "."))
        doc.AddAttribute("Numero", pec.Id.ToString())
        doc.AddAttribute("Oggetto", pec.MailSubject)
        If pec.MailDate.HasValue Then
            doc.AddAttribute("MailDate", pec.MailDate.DefaultString())
        End If

        Dim guidArchiviedChain As Guid = chain.ArchiveInBiblos(pec.Location.ConsBiblosDSDB)

        If Not guidArchiviedChain = Guid.Empty Then
            pec.IDMailContent = guidArchiviedChain
            pec.Size = doc.Size
        End If

        If needTransaction Then
            Factory.PECMailFacade.Update(pec)
        Else
            Factory.PECMailFacade.UpdateWithoutTransaction(pec)
        End If
    End Sub
    Public Function GetProtocol(pec As PECMail) As Protocol
        If pec.DocumentUnit Is Nothing OrElse pec.DocumentUnit.Environment <> DSWEnvironment.Protocol Then
            Return Nothing
        End If

        Return _dao.GetProtocol(pec)
    End Function

    Public Function GetPecMailContent(pec As PECMail) As BiblosDocumentInfo
        If pec.IDMailContent.Equals(Guid.Empty) Then
            Return Nothing
        End If

        Dim documents As List(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(pec.IDMailContent)
        If Not documents.HasSingle() Then
            Throw New DocSuiteException("Errore in fase di recupero Busta PEC.")
        End If

        Return documents.First()
    End Function

    Private Shared Function ArchiveAttachedDocument(ByRef pec As PECMail, ByRef document As DocumentInfo) As Guid
        Dim doc As BiblosDocumentInfo = document.ArchiveInBiblos(pec.Location.ProtBiblosDSDB, pec.IDAttachments)
        pec.IDAttachments = doc.ChainId
        Return doc.DocumentId
    End Function

    ''' <summary>
    ''' Archivia un documento nella catena degli allegati e restituisce l'identificativo del documento salvato.
    ''' </summary>
    ''' <param name="pec">PEC a cui associare l'allegato</param>
    ''' <param name="data">Stream del documento</param>
    ''' <param name="name">Nome del documento</param>
    ''' <returns>Restituisce l'identificativo del documento inserito.</returns>
    ''' <remarks>Il documento viene salvato nella catena degli allegati, se la catena non esiste viene creata e salvata nella PEC.</remarks>
    Public Function ArchiveAttachedDocument(ByRef pec As PECMail, data() As Byte, name As String) As Guid
        Dim tor As Guid
        Dim doc As DocumentInfo = New MemoryDocumentInfo(data, name)
        tor = ArchiveAttachedDocument(pec, doc)
        doc.Dispose()
        Return tor
    End Function

    Public Sub ArchivePostacert(ByRef pec As PECMail, data() As Byte, name As String)
        pec.IDPostacert = ArchiveAttachedDocument(pec, data, name)
        pec.Size = data.LongLength
    End Sub

    Public Sub ArchiveDaticert(ByRef pec As PECMail, data() As Byte, name As String)
        pec.IDDaticert = ArchiveAttachedDocument(pec, data, name)
    End Sub

    Public Sub LogicDelete(ByRef pecs As ICollection(Of PECMail))
        _dao.LogicDelete(pecs.Select(Function(pec) pec.Id).ToList())
    End Sub

    Public Sub ArchiveSmime(ByRef pec As PECMail, data() As Byte, name As String)
        pec.IDSmime = ArchiveAttachedDocument(pec, data, name)
    End Sub

    Public Function GetSmime(ByRef pec As PECMail) As BiblosDocumentInfo
        If pec.IDSmime = Guid.Empty Then
            Return Nothing
        End If

        Return New BiblosDocumentInfo(pec.IDSmime)
    End Function

    Public Sub ArchiveEnvelope(ByRef pec As PECMail, data() As Byte, name As String)
        pec.IDEnvelope = ArchiveAttachedDocument(pec, data, name)
    End Sub

    Public Sub ArchiveSegnatura(ByRef pec As PECMail, data() As Byte, name As String)
        pec.IDSegnatura = ArchiveAttachedDocument(pec, data, name)
    End Sub

    Public Function ArchiveAttachments(ByRef pec As PECMail, docs As IList(Of DocumentInfo)) As IList(Of PECMailAttachment)
        Dim tor As New List(Of PECMailAttachment)
        For Each docToAttach As DocumentInfo In docs
            tor.Add(ArchiveAttachment(pec, docToAttach, False))
        Next
        Return tor
    End Function

    Public Function ArchiveAttachment(ByRef pec As PECMail, doc As DocumentInfo, main As Boolean) As PECMailAttachment
        Return ArchiveAttachment(pec, doc, doc.Name, main)
    End Function

    Public Function ArchiveAttachment(ByRef pec As PECMail, doc As DocumentInfo, name As String, main As Boolean) As PECMailAttachment
        Return ArchiveAttachment(pec, doc, name, main, Nothing)
    End Function

    Public Function ArchiveAttachment(ByRef pec As PECMail, data() As Byte, name As String, main As Boolean) As PECMailAttachment
        Return ArchiveAttachment(pec, data, name, main, Nothing)
    End Function

    ''' <summary>
    ''' Metodo di retrocompatibilità per consentire l'inserimento da stream di byte
    ''' </summary>
    ''' <param name="pec"></param>
    ''' <param name="data"></param>
    ''' <param name="name"></param>
    ''' <param name="main"></param>
    ''' <param name="parent"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ArchiveAttachment(ByRef pec As PECMail, data() As Byte, name As String, main As Boolean, parent As PECMailAttachment) As PECMailAttachment
        Dim doc As DocumentInfo = New MemoryDocumentInfo(data, name)
        Dim pecAttachment As PECMailAttachment = ArchiveAttachment(pec, doc, name, main, parent)
        doc.Dispose()
        Return pecAttachment
    End Function

    ''' <summary>
    ''' Inserimento principale degli allegati in PEC
    ''' </summary>
    ''' <param name="pec"></param>
    ''' <param name="doc"></param>
    ''' <param name="name"></param>
    ''' <param name="main"></param>
    ''' <param name="parent"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ArchiveAttachment(ByRef pec As PECMail, doc As DocumentInfo, name As String, main As Boolean, parent As PECMailAttachment) As PECMailAttachment
        Dim item As New PECMailAttachment()
        item.AttachmentName = name
        item.IsMain = main
        item.Mail = pec
        item.Parent = parent

        item.IDDocument = ArchiveAttachedDocument(pec, doc)
        item.Size = doc.Size

        pec.Attachments.Add(item)
        Factory.PECMailAttachmentFacade.Save(item)

        Return item
    End Function

    Public Shared Function GetAttachments(ByRef pec As PECMail) As BiblosDocumentInfo()
        If pec.IDAttachments = Guid.Empty Then
            Return New BiblosDocumentInfo() {}
        End If
        Return BiblosDocumentInfo.GetDocuments(pec.IDAttachments).ToArray()
    End Function

    Public Function GetMailStatusByXRiferimentoMessageId(ByVal xRiferimentoMessageId As String) As String
        Return _dao.GetMailStatusByXRiferimentoMessageId(xRiferimentoMessageId)
    End Function

    ''' <summary> Carica le PEC da inviare di tipologia standard (ovvero singole) </summary>
    ''' <param name="idPecMailBox">Casella in cui cercare</param>
    Public Function GetOutgoingMails(ByVal idPecMailBox As Short, maxResults As Integer, Optional useStatusProcessing As Boolean = False) As IList(Of Integer)
        Return _dao.GetOutgoingMails(idPecMailBox, False, maxResults, useStatusProcessing)
    End Function

    ''' <summary> Carica le PEC che devono essere duplicate perchè impostate come Multiple </summary>
    ''' <param name="idPecMailBox">Casella in cui cercare</param>
    Public Function GetOutgoingMailsToDuplicate(ByVal idPecMailBox As Short, maxResults As Integer) As IList(Of Integer)
        Return _dao.GetOutgoingMails(idPecMailBox, True, maxResults)
    End Function

    Public Function GetOutgoingMailByXRiferimentoMessageId(ByVal xRiferimentoMessageId As String) As PECMail
        Return _dao.GetOutgoingMailByXRiferimentoMessageId(xRiferimentoMessageId)
    End Function

    ''' <summary> Permette di ottenere tutte le pec in entrata con il medesimo xRiferimentoMessageID </summary>
    ''' <param name="xRiferimentoMessageId"></param>
    ''' <returns>lista di pec con il medesimo xRiferimentoMessageID</returns>
    Public Function GetIncomingMailByXRiferimentoMessageId(ByVal xRiferimentoMessageId As String) As IList(Of PECMail)
        Return _dao.GetIncomingMailByXRiferimentoMessageID(xRiferimentoMessageId)
    End Function

    Public Function GetMailByXRiferimentoMessageId(ByVal xRiferimentoMessageId As String) As PECMail
        Return _dao.GetMailByXRiferimentoMessageId(xRiferimentoMessageId)
    End Function

    Public Function Exists(ByVal uid As String, ByVal box As PECMailBox) As Boolean
        Return _dao.Exists(uid, box)
    End Function

    Public Function ChecksumExists(ByVal checksum As String, ByVal originalRecipient As String, Optional includeOnError As Boolean = False) As Boolean
        Return _dao.ChecksumExists(checksum, originalRecipient, includeOnError)
    End Function

    Public Function HeaderChecksumExists(ByVal headerChecksum As String, ByVal originalRecipient As String, Optional includeOnError As Boolean = False) As Boolean
        Return _dao.HeaderChecksumExists(headerChecksum, originalRecipient, includeOnError)
    End Function

    Public Function GetByChecksum(checksum As String, originalRecipient As String, ByVal isActiveIn As List(Of Integer)) As IList(Of PECMail)
        Return _dao.GetByChecksum(checksum, originalRecipient, isActiveIn)
    End Function

    Public Function GetMailByUid(ByVal uid As String) As PECMail
        Return _dao.GetMailByUid(uid)
    End Function
    Public Function GetOriginalPECFromReferenceToSDIIdentification(ByVal referenceToPECMessageId As String) As PECMail
        Return _dao.GetOriginalPECFromReferenceToSDIIdentification(referenceToPECMessageId)
    End Function
    Public Function GetOriginalPECFromPAAttachmentFileName(attachmentFileName As String) As PECMail
        Return _dao.GetOriginalPECFromPAAttachmentFileName(attachmentFileName)
    End Function

    Public Function GetMailsToForward(ByVal idPecMailBox As Short) As IList(Of PECMail)
        Return _dao.GetMailsToForward(idPecMailBox)
    End Function
    ''' <summary>
    ''' Ritorno un oggetto mail incluso nelle History di OutGoing di una PEC
    ''' </summary>
    ''' <param name="mail">mail PEC in cui verificare la History</param>
    ''' <param name="status">stato della mail Da leggere negli stati di History</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetOutgoingMailHistoryByStatus(ByVal mail As PECMail, ByVal status As String, pecType As PECMailType) As PECMail
        Dim result As PECMail = Nothing
        For Each currentPecMailOutgoing As PECMail In _dao.GetOutgoingMailHistory(mail.Id)
            If currentPecMailOutgoing.MailType = status AndAlso currentPecMailOutgoing.PECType = pecType Then
                result = currentPecMailOutgoing
            End If
        Next
        Return result
    End Function

    Public Function GetOutgoingMailHistory(ByVal mail As PECMail) As IList(Of PECMail)
        Return GetOutgoingMailHistory(mail.Id)
    End Function

    Public Function GetOutgoingMailHistory(ByVal idPecMail As Integer) As IList(Of PECMail)
        Return _dao.GetOutgoingMailHistory(idPecMail)
    End Function

    Public Function GetEmptyMails() As IList(Of PECMail)
        Return _dao.GetEmptyMails()
    End Function

    <Obsolete("Non usata")>
    Public Overrides Function IsUsed(ByRef obj As PECMail) As Boolean
        Return True
    End Function

#Region " Ricevuta PEC "


#End Region

    Public Sub FlushSession()
        _dao.FlushSession()
    End Sub

    Public Function CanChangeHandler(pec As PECMail) As Boolean
        If String.IsNullOrEmpty(pec.Handler) Then
            Return True
        End If
        If DocSuiteContext.Current.ProtocolEnv.PECHandlerTimeout <= 0 Then
            Return False
        End If
        Return Not pec.LastChangedDate.HasValue OrElse pec.LastChangedDate.Value.DateTime.AddSeconds(DocSuiteContext.Current.ProtocolEnv.PECHandlerTimeout) < DateTime.Now
    End Function

    ''' <summary> Genera una mail di notifica spostamento. </summary>
    ''' <param name="mail">Mail spostata</param>
    ''' <param name="sourceMailbox">Mailbox di origine</param>
    ''' <param name="destinationMailbox">Mailbox di destinazione</param>
    Private Function CreateMoveNotification(mail As PECMail, sourceMailbox As PECMailBox, destinationMailbox As PECMailBox) As PECMail
        Dim retval As New PECMail()
        retval.XTrasporto = "errore" ' mail.XTrasporto
        retval.Direction = mail.Direction
        Dim mailSubject As String = "NOTIFICA SPOSTAMENTO: {0}"
        mailSubject = String.Format(mailSubject, mail.MailSubject)
        retval.MailSubject = mailSubject
        retval.MailSenders = sourceMailbox.MailBoxName
        retval.MailRecipients = destinationMailbox.MailBoxName
        retval.MailDate = DateTime.Now
        retval.MailBox = destinationMailbox
        retval.PECType = PECMailType.Notifica
        Dim sb As New StringBuilder()
        sb.AppendLine("Questa è una notifica avvenuto spostamento di una mail in questa mailbox.")
        sb.Append(vbCrLf)
        sb.AppendFormat("Identificativo PEC: {0}{1}", mail.Id, vbCrLf)
        sb.AppendFormat("Casella PEC di origine: {0}{1}", sourceMailbox.MailBoxName, vbCrLf)
        Dim mailDate As String = "Data ricezione PEC: non ancora spedita."
        If mail.MailDate.HasValue Then
            mailDate = String.Format("Data ricezione PEC: {0}{1}", mail.MailDate, vbCrLf)
        End If
        sb.AppendLine(mailDate)
        sb.AppendFormat("Mittente: {0}{1}", mail.MailSenders, vbCrLf)
        sb.AppendFormat("Destinatari: {0}{1}", mail.MailRecipients, vbCrLf)
        sb.AppendFormat("Oggetto: {0}", mail.MailSubject)
        retval.MailBody = sb.ToString()
        retval.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Active)

        Return retval
    End Function

    ''' <summary> Invia una mail di notifica spostamento.  </summary>
    ''' <param name="mail">Mail spostata</param>
    ''' <param name="sourceMailbox">Mailbox di origine</param>
    ''' <param name="destinationMailbox">Mailbox di destinazione</param>
    ''' <param name="recipients">Destinatari per la mail</param>
    ''' <param name="notificationType">Tipo di notifica se via PEC o via Mail</param>
    Public Sub SendMoveNotification(mail As PECMail, sourceMailbox As PECMailBox, destinationMailbox As PECMailBox, recipients As List(Of String), notificationType As PECMoveNotificationType)

        If notificationType = PECMoveNotificationType.Message Then
            sendMailNotification(mail, recipients)
        End If

        If notificationType = PECMoveNotificationType.Pec Then
            sendPECNotification(mail, sourceMailbox, destinationMailbox)
        End If

    End Sub

    Private Sub sendPECNotification(mail As PECMail, sourceMailbox As PECMailBox, destinationMailbox As PECMailBox)

        Dim moveNotification As PECMail = CreateMoveNotification(mail, sourceMailbox, destinationMailbox)
        Save(moveNotification)
        Factory.PECMailLogFacade.Created(moveNotification)
        Factory.PECMailLogFacade.MoveNotified(mail, moveNotification)

    End Sub




    Private Sub sendMailNotification(currentPecMail As PECMail, recipients As List(Of String))
        For Each recipient As String In recipients
            Try
                If DocSuiteContext.Current.ProtocolEnv.PECMoveNotificationEnabled Then
                    Dim mailSubject As New StringBuilder(DocSuiteContext.Current.ProtocolEnv.PECHandlerMoveNotificationTemplate)
                    mailSubject.Replace("%USER%", currentPecMail.Handler)
                    mailSubject.Replace("%USEREMAIL%", currentPecMail.Handler)
                    mailSubject.Replace("%ID%", currentPecMail.Id.ToString())
                    mailSubject.Replace("%SUBJECT%", currentPecMail.MailSubject)
                    mailSubject.Replace("%DATE%", currentPecMail.MailDate.Value.ToString())

                    FacadeFactory.Instance.MessageFacade.NotifyToPreviousHandler(currentPecMail.Handler, recipient, mailSubject.ToString(), DocSuiteContext.Current.ProtocolEnv.DispositionNotification)

                End If
            Catch ex As Exception
                FileLogger.Warn(LoggerName, "Errore nella presa in carico PEC", ex)
            End Try
        Next
    End Sub

    Public Sub MoveToNewInBoxWithNotification(ByVal mailBoxId As Short, ByVal pecMails As IList(Of PECMail), motivation As String)
        Dim unitOfWork As New NHibernateUnitOfWork("ProtDB")
        Try
            Dim mailbox As PECMailBox = Factory.PECMailboxFacade.GetById(mailBoxId)

            Dim recipients As New List(Of String)

            For Each emailAddress As PECMailBoxRole In mailbox.MailBoxRoles
                If Not String.IsNullOrEmpty(emailAddress.Role.EMailAddress) Then
                    recipients.Add(emailAddress.Role.EMailAddress)
                End If
            Next

            For Each mail As PECMail In pecMails
                Factory.PECMailLogFacade.Moved(mail, mail.MailBox, mailbox, motivation)
                ' Invio una mail di notifica avvenuto spostamento.
                If DocSuiteContext.Current.ProtocolEnv.PECMoveNotificationEnabled Then
                    SendMoveNotification(mail, mail.MailBox, mailbox, recipients, DocSuiteContext.Current.ProtocolEnv.PECMoveNotificationType)
                End If

                mail.MailBox = mailbox

                If DocSuiteContext.Current.ProtocolEnv.PECHandlerEnabled Then
                    If DocSuiteContext.Current.ProtocolEnv.PECUnhandleOnMoveBehaviour.Equals(2) Then
                        ' Rilascio la pec sullo spostamento se la mailbox di destinazione.
                        If Not String.IsNullOrEmpty(mail.Handler) AndAlso Not Factory.PECMailboxFacade.IsVisibleMailBox(mail.MailBox) Then
                            Factory.PECMailLogFacade.Handle(mail, mail.Handler, False)
                            mail.Handler = String.Empty
                        End If
                    ElseIf DocSuiteContext.Current.ProtocolEnv.PECUnhandleOnMoveBehaviour.Equals(1) OrElse DocSuiteContext.Current.ProtocolEnv.PECUnhandleOnMove Then
                        ' Rilascio la pec sullo spostamento.
                        'If Not String.IsNullOrEmpty(mail.Handler) Then
                        Factory.PECMailLogFacade.Handle(mail, mail.Handler, False)
                        mail.Handler = String.Empty
                        'End If
                    End If
                End If
                'mailbox.Mails.Add(mail) ' di dubbia utilità...
                Factory.PECMailFacade.Update(mail)

                If mail.Direction = PECMailDirection.Ingoing AndAlso mail.PECType = PECMailType.PEC AndAlso Not mail.HasDocumentUnit() Then
                    SendPECMailCreatedEvent(mail)
                End If
            Next

            'Commit Transaction
            unitOfWork.Commit()
        Catch ex As Exception
            'if there is an error Rollback
            unitOfWork.Rollback()
            Throw New DocSuiteException("Errore spostamento", "Impossibile spostare gli elementi selezionati nella nuova casella di posta. Contattare l'assistenza", ex)
        End Try
    End Sub

    Public Function IsPECMailInvoice(pecMail As PECMail) As Boolean
        Return pecMail.MailSenders.StartsWith("sdi", StringComparison.InvariantCultureIgnoreCase) AndAlso pecMail.MailSenders.Contains("fatturapa.it")
    End Function

    Public Sub FinalizeToProtocol(id As Integer, protocol As Protocol)
        FinalizeToProtocol(GetById(id), protocol)
    End Sub

    Public Sub FinalizeToProtocol(mail As PECMail, protocol As Protocol)
        mail.Year = protocol.Year
        mail.Number = protocol.Number
        mail.DocumentUnit = FacadeFactory.Instance.DocumentUnitFacade.GetById(protocol.Id)
        mail.RecordedInDocSuite = CType(1, Short)
        mail.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Active)
        Update(mail)
    End Sub

    ''' <summary>
    ''' Meotodo utilizzato in JeepService per attivare la PEC
    ''' </summary>
    ''' <param name="pec"></param>
    ''' <remarks></remarks>
    Public Sub ActivatePec(pec As PECMail)
        pec.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Active)
        UpdateNoLastChange(pec)
    End Sub

    Public Sub ErrorPec(pec As PECMail)
        pec.IsActive = ActiveType.PECMailActiveType.Error
        UpdateNoLastChange(pec)
    End Sub

    ''' <summary> Data una pecMail verifica se è certificata. </summary>
    ''' <returns>True se è certificata, False in caso contrario</returns>
    Public Function IsCertified(ByVal pecMail As PECMail) As Boolean
        Return pecMail.XTrasporto.Eq("posta-certificata") OrElse pecMail.Direction = PECMailDirection.Outgoing
    End Function

    ''' <summary> Estrae tutte le mail che non sono ancora state convertite alla DSW8. </summary>
    Public Function GetDsw7StoredMail(ByVal idPecMailBox As Short, ByVal maxResults As Integer, ByVal startDate As Date?, ByVal endDate As Date?) As IList(Of PECMail)
        Return _dao.GetDsw7StoredMail(idPecMailBox, maxResults, startDate, endDate)
    End Function

    Public Sub CheckPecMailBoxHashes(ByVal idPecMailBox As Short)
        'Carico tutte le PEC della casella che non hanno un hash (a regime dovrebbero essere 0)
        Dim pecMailContentFacade As New PECMailContentFacade()
        Dim notHashedMails As IList(Of PECMail) = _dao.GetNotHashedMails(idPecMailBox)
        While (notHashedMails.Count > 0)
            For Each pecMail As PECMail In notHashedMails
                ' Utilizzo primariamente il MailContent storico
                Dim contentToHash As String = pecMail.MailContent

                ' Se è vuoto utilizzo il nuovo PecMailContent
                If String.IsNullOrEmpty(contentToHash) Then
                    Dim pecMailContent As PECMailContent = pecMailContentFacade.GetByMail(pecMail)
                    If (pecMailContent Is Nothing) Then
                        '' Se non esiste anche il PECMailContent allora utilizzo il MailBody
                        contentToHash = pecMail.MailBody
                    Else
                        '' Altrimenti utilizzo correttamente il PECMailContenta
                        contentToHash = pecMailContent.MailContent
                    End If
                End If


                If (Not String.IsNullOrEmpty(contentToHash)) Then
                    ' Calcolo l'SHA256 e aggiorno il campo
                    pecMail.Checksum = contentToHash.ComputeSHA256Hash()
                Else
                    pecMail.Checksum = "NO_CONTENT_TO_HASH"
                End If

                Update(pecMail)
            Next

            'Libero la memoria di NHibernate e ricalcolo
            NHibernateSessionManager.Instance.CloseTransactionAndSessions()
            notHashedMails = _dao.GetNotHashedMails(idPecMailBox)
        End While
    End Sub
    ''' <summary>
    ''' Genera la lista degli allegati univoci partendo da una lista di DocumentInfo
    ''' </summary>
    Public Function GetDocumentList(documents As List(Of DocumentInfo)) As List(Of DocumentInfo)
        Dim dct As Dictionary(Of String, DocumentInfo) = Nothing
        FillDocsRecursive(dct, documents, False)
        Return dct.Values.ToList()
    End Function
    ''' <summary> Genera la lista piatta di allegati ad una mail. </summary>
    Public Function GetDocumentList(pecmail As BiblosPecMailWrapper) As List(Of DocumentInfo)
        '' Prendo il valore di default per l'invio
        Dim defaultSendMailView As PECMailView = FacadeFactory.Instance.PECMailViewFacade.GetById(DocSuiteContext.Current.ProtocolEnv.PecMailViewDefaultToSend)
        '' Se non è presente prendo il default dell'utente corrente
        If defaultSendMailView Is Nothing Then
            defaultSendMailView = FacadeFactory.Instance.PECMailViewFacade.GetDefault()
        End If

        Dim node As DocumentInfo = GetDocumentInfo(pecmail, defaultSendMailView)
        Dim documents As Dictionary(Of String, DocumentInfo) = Nothing
        FillDocsRecursive(documents, node, True)
        Return documents.Values.ToList()
    End Function

    ''' <summary> Genera un albero di allegati ad una mail. </summary>
    Public Function GetDocumentInfo(pecmail As BiblosPecMailWrapper, currentPecMailView As PECMailView) As DocumentInfo
        ''Gestione della View - Imposto i valori di default
        Dim documentoPrincipaleLabel As String = "Messaggio"
        Dim corpoDelMessaggioLabel As String = "postacert.eml"
        Dim allegatiLabel As String = "Allegati"
        Dim allegatiTecniciLabel As String = "Allegati tecnici"
        Dim ricevuteLabel As String = "Ricevute"

        Dim foldersToHide As String() = {}
        Dim filesToHide As String() = {}
        Dim extensionsToHide As String() = {}
        Dim hideExtensions As Boolean
        Dim flatAttachments As Boolean

        Dim tor As New FolderInfo

        ''Se ho ricevuto una lista provvedo a ricalcolare (se specificate) le etichette e i parametri. 
        ''Altrimenti restano valide quelle di default già definite
        If currentPecMailView IsNot Nothing Then
            If Not String.IsNullOrEmpty(currentPecMailView.DocumentoPrincipaleLabel) Then
                documentoPrincipaleLabel = currentPecMailView.DocumentoPrincipaleLabel
            End If
            If Not String.IsNullOrEmpty(currentPecMailView.CorpoDelMessaggioLabel) Then
                corpoDelMessaggioLabel = currentPecMailView.CorpoDelMessaggioLabel
            End If
            If Not String.IsNullOrEmpty(currentPecMailView.AllegatiLabel) Then
                allegatiLabel = currentPecMailView.AllegatiLabel
            End If
            If Not String.IsNullOrEmpty(currentPecMailView.AllegatiTecniciLabel) Then
                allegatiTecniciLabel = currentPecMailView.AllegatiTecniciLabel
            End If
            If Not String.IsNullOrEmpty(currentPecMailView.RicevuteLabel) Then
                ricevuteLabel = currentPecMailView.RicevuteLabel
            End If
            If currentPecMailView.FoldersToHide IsNot Nothing Then
                foldersToHide = currentPecMailView.FoldersToHide.Split("|"c)
            End If
            If currentPecMailView.FilesToHide IsNot Nothing Then
                filesToHide = currentPecMailView.FilesToHide.Split("|"c)
            End If
            If currentPecMailView.ExtensionsToHide IsNot Nothing Then
                extensionsToHide = currentPecMailView.ExtensionsToHide.Split("|"c)
            End If
            hideExtensions = currentPecMailView.HideExtensions
            flatAttachments = currentPecMailView.FlatAttachments

            tor.Name = currentPecMailView.RootNodeName
        End If

        tor.ID = pecmail.Id
        If String.IsNullOrEmpty(tor.Name) Then
            tor.Name = If(pecmail.MailBox.IsProtocolBox.HasValue AndAlso pecmail.MailBox.IsProtocolBox.Value, "E-mail", "PEC")
        End If

        ' Aggiungo il documento principale
        If pecmail.PostaCert IsNot Nothing Then
            Dim postaCert As DocumentInfo = pecmail.PostaCert
            Dim parentFolder As New FolderInfo() With {.Name = documentoPrincipaleLabel, .Parent = tor}
            postaCert.Parent = parentFolder
            postaCert.Caption = corpoDelMessaggioLabel
            If extensionsToHide.Any(Function(ex) ex.Eq(Path.GetExtension(postaCert.Caption))) OrElse hideExtensions Then
                postaCert.Caption = Path.GetFileNameWithoutExtension(postaCert.Caption)
            End If
        ElseIf pecmail.MailContent IsNot Nothing Then
            Dim mailContent As BiblosDocumentInfo = pecmail.MailContent
            Dim parentFolder As New FolderInfo() With {.Name = documentoPrincipaleLabel, .Parent = tor}
            mailContent.Parent = parentFolder
            mailContent.Caption = pecmail.MailContent.Caption
            If extensionsToHide.Any(Function(ex) ex.Eq(Path.GetExtension(mailContent.Caption))) OrElse hideExtensions Then
                mailContent.Caption = Path.GetFileNameWithoutExtension(mailContent.Caption)
            End If
        ElseIf pecmail.MailBody IsNot Nothing Then
            Dim mailBody As FileDocumentInfo = pecmail.MailBody
            Dim parentFolder As New FolderInfo() With {.Name = documentoPrincipaleLabel, .Parent = tor}
            mailBody.Parent = parentFolder
            mailBody.Caption = pecmail.MailBody.Caption
            If extensionsToHide.Any(Function(ex) ex.Eq(Path.GetExtension(mailBody.Caption))) OrElse hideExtensions Then
                mailBody.Caption = Path.GetFileNameWithoutExtension(mailBody.Caption)
            End If
        End If

        '' Se la cartella (con il nome preferito) non è stata citata allora la inserisco
        If Not foldersToHide.Any(Function(x) x.Eq(allegatiLabel)) Then
            Dim attachments As New FolderInfo() With {.Name = allegatiLabel}
            For Each att As BiblosPecMailAttachmentWrapper In From att1 In pecmail.Attachments Where IsNothing(att1.Parent)
                AddAttachment(attachments, att, filesToHide, extensionsToHide, hideExtensions, flatAttachments)
            Next
            If attachments.Children.Count > 0 Then
                attachments.Parent = tor
            End If
        End If

        '' Se la cartella (con il nome preferito) non è stata citata allora la inserisco
        If Not foldersToHide.Any(Function(x) x.Eq(allegatiTecniciLabel)) Then
            Dim attachmentsTec As New FolderInfo() With {.Name = allegatiTecniciLabel}
            Dim busta As DocumentInfo = pecmail.Envelope
            '' Aggiungo la busta solo se non è stata esclusa
            If busta IsNot Nothing AndAlso Not filesToHide.Any(Function(x) x.Eq(busta.Caption)) Then
                If extensionsToHide.Any(Function(ex) ex.Eq(Path.GetExtension(busta.Caption))) OrElse hideExtensions Then
                    busta.Caption = Path.GetFileNameWithoutExtension(busta.Caption)
                End If
                attachmentsTec.AddChild(busta)
            End If
            Dim segnatura As DocumentInfo = pecmail.SegnaturaInteroperabilita
            '' Aggiungo la segnatura solo se non è stata esclusa
            If segnatura IsNot Nothing AndAlso Not filesToHide.Any(Function(x) x = segnatura.Caption) Then
                If extensionsToHide.Any(Function(ex) ex.Eq(Path.GetExtension(segnatura.Caption))) OrElse hideExtensions Then
                    segnatura.Caption = Path.GetFileNameWithoutExtension(segnatura.Caption)
                End If
                attachmentsTec.AddChild(segnatura)
            End If
            Dim daticert As DocumentInfo = pecmail.DatiCert
            '' Aggiungo daticert solo se non è stato escluso
            If daticert IsNot Nothing AndAlso Not filesToHide.Any(Function(x) x = daticert.Caption) Then
                If extensionsToHide.Any(Function(ex) ex.Eq(Path.GetExtension(daticert.Caption))) OrElse hideExtensions Then
                    daticert.Caption = Path.GetFileNameWithoutExtension(daticert.Caption)
                End If
                attachmentsTec.AddChild(daticert)
            End If
            Dim oChartCommunicationData As DocumentInfo = pecmail.OChartCommunicationData
            '' Aggiungo OChartCommunicationData.xml solo se non è stato escluso
            If oChartCommunicationData IsNot Nothing AndAlso Not filesToHide.Any(Function(x) x = oChartCommunicationData.Caption) Then
                If extensionsToHide.Any(Function(ex) ex.Eq(Path.GetExtension(oChartCommunicationData.Caption))) OrElse hideExtensions Then
                    oChartCommunicationData.Caption = Path.GetFileNameWithoutExtension(oChartCommunicationData.Caption)
                End If
                attachmentsTec.AddChild(oChartCommunicationData)
            End If
            If attachmentsTec.Children.Count > 0 Then
                tor.AddChild(attachmentsTec)
            End If
        End If

        '' Se la cartella (con il nome preferito) non è stata citata allora la inserisco
        If Not foldersToHide.Any(Function(x) x = ricevuteLabel) Then
            Dim receipts As New FolderInfo() With {.Name = ricevuteLabel}
            For Each receipt As BiblosPecMailReceiptWrapper In pecmail.Receipts
                Dim temp As BiblosDocumentInfo = receipt.Parent.Envelope
                If Not temp Is Nothing AndAlso Not filesToHide.Any(Function(x) x = temp.Caption) Then
                    temp.Caption = String.Format("{0}.eml", receipt.ReceiptType)
                    temp.Parent = receipts
                    '' Rimuovo l'estensione dalle ricevute nel caso in cui non sia da considerare
                    If extensionsToHide.Any(Function(ex) ex.Eq(".eml")) OrElse hideExtensions Then
                        temp.Caption = receipt.ReceiptType
                    End If
                End If
            Next
            If receipts.Children.Count > 0 Then
                tor.AddChild(receipts)
            End If
        End If
        ''Ritorno il FolderInfo generato
        Return tor
    End Function

    Private Sub FillDocsRecursive(ByRef docs As Dictionary(Of String, DocumentInfo), ByVal root As DocumentInfo, compressExtract As Boolean)
        If root.Children Is Nothing Then
            docs = New Dictionary(Of String, DocumentInfo)
            Exit Sub
        End If
        FillDocsRecursive(docs, root.Children, compressExtract)
        If Not compressExtract Then
            Exit Sub
        End If
        If (FileHelper.MatchExtension(root.Name, FileHelper.ZIP) OrElse FileHelper.MatchExtension(root.Name, FileHelper.RAR)) AndAlso DocSuiteContext.Current.ProtocolEnv.ViewerUnzip AndAlso Not root.HasChildren Then
            Try
                Dim compressManager As ICompress = New ZipCompress()
                If FileHelper.MatchExtension(root.Name, FileHelper.RAR) Then
                    compressManager = New RarCompress()
                End If
                Using memoryStream As New MemoryStream(root.Stream)
                    For Each item As CompressItem In compressManager.InMemoryExtract(memoryStream)
                        Dim memoryInfo As New MemoryDocumentInfo(item.Data, item.Filename)
                        Dim file As New TempFileDocumentInfo(BiblosFacade.SaveUniqueToTemp(memoryInfo))
                        file.Name = item.Filename
                        file.Parent = root
                        docs.Add(file.Name, file)
                    Next
                End Using
            Catch ex As ExtractException
                FileLogger.Warn(LogName.FileLog, ex.Message, ex)
            End Try
        End If
    End Sub

    ''' <summary> Smonta un <see cref="DocumentInfo"/> generato da <see cref="GetDocumentInfo"/> mettendolo in una lista. </summary>
    ''' <remarks> Tutto questo 'annida e disfa' è malato, trovare un modo sano per fare la lista piatta degli allegati di una mail. </remarks>
    Private Sub FillDocsRecursive(ByRef docs As Dictionary(Of String, DocumentInfo), ByVal docInfos As List(Of DocumentInfo), compressExtract As Boolean)
        If docs Is Nothing Then
            docs = New Dictionary(Of String, DocumentInfo)
        End If
        If docInfos.Any() Then
            For Each doc As DocumentInfo In docInfos
                If TryCast(doc, FolderInfo) Is Nothing Then
                    Dim docBiblos As DocumentInfo = doc
                    If (docBiblos IsNot Nothing) Then
                        If Not docs.ContainsKey(docBiblos.Serialized) Then
                            docs.Add(docBiblos.Serialized, doc)
                        End If
                    End If
                End If
                FillDocsRecursive(docs, doc, compressExtract)
            Next
        End If
    End Sub

    Private Sub AddAttachment(dest As DocumentInfo, att As BiblosPecMailAttachmentWrapper, filesToHide As String(), extensionsToHide As String(), hideExtensions As Boolean, useFlatAttachments As Boolean)
        If Not att.IsGenericAttachment() Then
            Return ' Allegato da non visualizzare
        End If

        Dim doc As DocumentInfo = CreateValidDocument(dest, att, filesToHide, extensionsToHide, hideExtensions)

        ''Se attivo gli allegati Flat carico sempre tutto sul primo nodo passato alla funzione
        If useFlatAttachments Then
            doc = dest
        End If

        '' anche se l'estensione non viene aggiunta, proseguo ugualmente su eventuali allegati figli
        If att.Children.Count > 0 Then
            For Each child As BiblosPecMailAttachmentWrapper In att.Children
                AddAttachment(doc, child, filesToHide, extensionsToHide, hideExtensions, useFlatAttachments)
            Next
        End If
    End Sub

    Private Function CreateValidDocument(dest As DocumentInfo, att As BiblosPecMailAttachmentWrapper, filesToHide As IEnumerable(Of String), extensionsToHide As IEnumerable(Of String), hideExtensions As Boolean) As DocumentInfo
        '' Rigenero il documento
        Dim doc As DocumentInfo = att.Document

        '' Se il nome del file è vietato restituisco il destinatario
        If filesToHide.Any(Function(x) x.Eq(doc.Name)) Then
            Return dest
        End If

        '' Rimuovo le estensioni se richiesto
        If extensionsToHide.Any(Function(x) x.Eq(doc.Extension)) OrElse hideExtensions Then
            doc.Caption = Path.GetFileNameWithoutExtension(doc.Caption)
        End If

        '' Se il nome contiene il DocumentId allora lo passo negli attributes --> Gestione "Allegati OChart"
        Dim splittedName As String() = doc.Name.Split("#"c)
        If splittedName IsNot Nothing AndAlso splittedName.Length > 1 Then
            doc.Caption = splittedName(1)
            doc.Name = splittedName(1)
            doc.ExternalKey = splittedName(0)
        End If

        '' Aggiungo il nodo al destinatario
        dest.AddChild(doc)

        '' Ritorno il nuovo nodo
        Return doc
    End Function

    ''' <summary> Ritorna l'id della mailbox all'interno della quale è stata originariamente scaricata la PEC </summary>
    ''' <param name="mail">PecMail da verificare</param>
    Public Function GetOriginalMailboxId(ByVal mail As PECMail) As Short?
        '' Se esiste la nuova gestione la uso (che è più performante)
        If Not String.IsNullOrEmpty(mail.OriginalRecipient) Then
            Dim mailBox As PECMailBox = FacadeFactory.Instance.PECMailboxFacade.GetByRecipient(mail.OriginalRecipient)
            If mailBox Is Nothing Then
                Return Nothing
            End If
            Return mailBox.Id
        End If

        '' Altrimenti uso la vecchia gestione e recupero dai log
        Dim importedLog As PECMailBoxLog = FacadeFactory.Instance.PECMailboxLogFacade.GetImportItem(mail)
        If importedLog Is Nothing Then
            Return Nothing
        End If

        Return importedLog.IDMailBox
    End Function

    Public Sub Reset(ByVal pec As PECMail)
        pec.RecordedInDocSuite = Nothing
        pec.Year = Nothing
        pec.Number = Nothing
        pec.DocumentUnit = Nothing
        pec.Handler = Nothing
        Update(pec)
    End Sub

    ''' <summary> Genera le PEC duplicate a partire da una PEC multipla di origine </summary>
    ''' <remarks> Utilizzato nel JeepService </remarks>
    Public Sub SplitMultiPec(ByVal sourcePecMail As PECMail)
        If Not sourcePecMail.Multiple Then
            Throw New Exception(String.Format("Impossibile duplicare la PEC {0} - casella {1} in quanto non marcata per la duplicazione.", sourcePecMail.Id, sourcePecMail.MailBox))
        End If

        Dim pecList As New List(Of PECMail)()
        Select Case sourcePecMail.MultipleType
            Case PecMailMultipleTypeEnum.NoSplit
                Return
            Case PecMailMultipleTypeEnum.SplitByRecipients
                pecList.AddRange(PecMailDuplicator.SplitMultiPecByRecipients(sourcePecMail))
            Case PecMailMultipleTypeEnum.SplitBySize
                pecList.AddRange(PecMailDuplicator.SplitMultiPecBySize(sourcePecMail))
            Case PecMailMultipleTypeEnum.SplitBySizeAndRecipients
                Dim partialList As IList(Of PECMail) = PecMailDuplicator.SplitMultiPecBySize(sourcePecMail)
                For Each pecMail As PECMail In partialList
                    pecList.AddRange(PecMailDuplicator.SplitMultiPecByRecipients(pecMail))
                Next
        End Select

        '' Imposto la PEC di origine a status 4 per indicare che è già stata processata
        '' (Scelta abbastanza singolare che andrebbe inserita all'interno di un contesto più ampio)
        sourcePecMail.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Processed)
        Save(sourcePecMail)

        '' Attivo tutte le PEC generate
        For Each pecMail As PECMail In pecList
            ActivatePec(pecMail)
        Next

        FileLogger.Info(LoggerName, String.Format("Conclusa duplicazione PEC {0}.", sourcePecMail.Id))
    End Sub

    ''' <summary> Ritorna il file originale della mail. </summary>
    ''' <remarks> In caso di errore ritorna Nothing </remarks>
    Public Function GetOriginalEml(ByVal pec As PECMail) As DocumentInfo
        If pec.Direction = PECMailDirection.Ingoing Then
            If pec.Location Is Nothing Then
                Return Nothing
            End If
            Return GetPecMailContent(pec)
        End If

        If pec.Direction <> PECMailDirection.Outgoing Then
            Return Nothing
        End If

        '' altrimenti carico solo l'eml effettivamente inviato
        If pec.IDPostacert <> Guid.Empty Then
            Return New BiblosDocumentInfo(pec.IDPostacert)
        End If

        ' Salvo in TEMP un file di Testo con il corpo della MAIL
        Dim corpoDellaMail As New MemoryDocumentInfo(If(String.IsNullOrEmpty(pec.MailBody), String.Empty, pec.MailBody).ToBytes(), "Corpo della mail.txt")
        Return New FileDocumentInfo(corpoDellaMail.SaveUniqueToTemp(DocSuiteContext.Current.User.UserName))
    End Function

    ''' <summary> Dimensione della PECMail originale </summary>
    ''' <remarks> Se non è mai stato calcolato aggiorna il campo </remarks>
    ''' <returns> Stringa con dimensione dinamica ("byte", "KB"...). </returns>
    Public Function GetCalculatedSize(ByVal pec As PECMail) As String
        ' TODO: necessario refactoring per separare ritiro, aggiornamento e visualizzazione
        ''leggo il size corrente e se non ha valore lo valorizzo
        If Not pec.Size.HasValue Then
            Dim originalEml As DocumentInfo = GetOriginalEml(pec)
            If originalEml IsNot Nothing Then
                '' Memorizzo la grandezza del documentInfo
                pec.Size = originalEml.Size
            Else
                '' Se non riesco ad ottenere il documento originale allora memorizzo una grandezza pari a 0
                pec.Size = 0
            End If
            UpdateNoLastChange(pec)
        End If
        Return pec.Size.ToByteFormattedString(2)
    End Function

    Public Function GetElementsWithoutOriginalRecipient(ByVal idMailBox As Short) As IList(Of PecMailHeader)
        Dim pecFinder As New NHibernatePECMailFinder()
        ' tutte le pec
        pecFinder.EnablePaging = False
        ' della casella corrente
        pecFinder.MailboxIds = {idMailBox}
        ' che sono attive
        pecFinder.Actives = True
        ' che sono state scaricate
        pecFinder.Direction = PECMailDirection.Ingoing
        ' che hanno il checksum calcolato
        pecFinder.WithChecksum = True
        ' che non hanno l'OriginalRecipient
        pecFinder.WithOriginalRecipient = False

        Return CType(pecFinder.DoSearchHeader(), IList(Of PecMailHeader))
    End Function

    Public Sub CalculateMissingOriginalRecipient(ByVal idMailBox As Short)
        Dim pecsToProcess As IList(Of PecMailHeader) = GetElementsWithoutOriginalRecipient(idMailBox)

        For Each pecMailHeader As PecMailHeader In pecsToProcess
            Dim currentPec As PECMail = GetById(pecMailHeader.Id.Value)
            If Not pecMailHeader.HasMove.HasValue OrElse Not pecMailHeader.HasMove.Value Then
                ''se la PEC non è mai stata spostata posso aggiornarla senza problemi con il valore della sua casella corrente
                currentPec.OriginalRecipient = currentPec.MailBox.MailBoxName
            Else
                ''altrimenti devo cercare il valore del log di download
                Dim pmbLog As PECMailBoxLog = Factory.PECMailboxLogFacade.GetImportItem(currentPec)
                If pmbLog IsNot Nothing Then
                    Dim originalMailBox As PECMailBox = Factory.PECMailboxFacade.GetById(pmbLog.IDMailBox)
                    currentPec.OriginalRecipient = originalMailBox.MailBoxName
                End If
            End If
            '' aggiorno subito
            UpdateNoLastChange(currentPec)
        Next
    End Sub

#Region " Methods "

    Public Overrides Sub Save(ByRef mail As PECMail)
        mail.MailSenders = If(String.IsNullOrEmpty(mail.MailSenders), String.Empty, mail.MailSenders.Trim())
        mail.MailRecipients = If(String.IsNullOrEmpty(mail.MailRecipients), String.Empty, mail.MailRecipients.Trim())
        mail.MailRecipientsCc = If(String.IsNullOrEmpty(mail.MailRecipientsCc), String.Empty, mail.MailRecipientsCc.Trim())
        MyBase.Save(mail)
    End Sub

    Public Function SendMail(dto As MailDTO, Optional toPdf As Boolean = True) As Integer
        Dim mail As New PECMail()

        mail.Direction = PECMailDirection.Outgoing
        mail.MailType = PECMailTypes.Invio
        mail.MailSenders = dto.Sender.EmailAddress

        If dto.HasRecipients() Then
            Dim recipients As List(Of Contact) = dto.Recipients.Select(Function(c) FacadeFactory.Instance.ProtocolFacade.GetContact(c)).ToList()
            Dim stringifiedRecipients As String = String.Join(";", recipients.Select(Function(r) Me.GetEmailAddress(r)))
            mail.MailRecipients = stringifiedRecipients
        End If

        If dto.HasRecipientsCc() Then
            Dim recipientsCc As List(Of Contact) = dto.RecipientsCc.Select(Function(c) FacadeFactory.Instance.ProtocolFacade.GetContact(c)).ToList()
            Dim stringifiedRecipientsCc As String = String.Join(";", recipientsCc.Select(Function(r) Me.GetEmailAddress(r)))
            mail.MailRecipientsCc = stringifiedRecipientsCc
        End If

        mail.MailSubject = dto.Subject
        mail.MailBody = dto.Body

        mail.MailBox = FacadeFactory.Instance.PECMailboxFacade.GetById(dto.Mailbox.Id)
        mail.Location = mail.MailBox.Location

        mail.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Disabled)
        Me.Save(mail)
        FacadeFactory.Instance.PECMailLogFacade.Created(mail)

        If dto.HasAttachments() Then
            Dim attachments As List(Of DocumentInfo) = dto.Attachments().SelectMany(Function(d) d.ToDocumentInfos()).ToList()
            If toPdf Then
                attachments = attachments.Select(Function(d) Me.GetBiblosPdfOrDefault(d)).ToList()
            End If

            For Each item As DocumentInfo In attachments
                Dim s_attr As KeyValuePair(Of String, String)? = item.Attributes.SingleOrDefault(Function(f) f.Key.Eq("Signature"))
                Dim f_attr As KeyValuePair(Of String, String)? = item.Attributes.SingleOrDefault(Function(f) f.Key.Eq("Filename"))
                item.ClearAttributes()
                If (s_attr.HasValue) Then
                    item.AddAttribute(s_attr.Value.Key, s_attr.Value.Value)
                End If
                If (f_attr.HasValue) Then
                    item.AddAttribute(f_attr.Value.Key, f_attr.Value.Value)
                End If

                FacadeFactory.Instance.PECMailFacade.ArchiveAttachment(mail, item, item.Name, False)
            Next
        End If

        mail.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Active)
        Me.UpdateNoLastChange(mail)

        Return mail.Id
    End Function

    Public Function PairToProtocol(mail As PECMail, protocol As Protocol) As PECMail
        If mail.Year.HasValue AndAlso mail.Number.HasValue Then
            Dim message As String = "Questa mail ha già un protocollo associato ({0})."
            message = String.Format(message, protocol.FullNumber)
            Throw New InvalidOperationException(message)
        End If

        mail.Year = protocol.Year
        mail.Number = protocol.Number
        mail.DocumentUnit = FacadeFactory.Instance.DocumentUnitFacade.GetById(protocol.Id)

        Me.Update(mail)
        Return mail
    End Function

    Public Function PairToProtocol(mailDTO As API.MailDTO, protocolDTO As API.ProtocolDTO) As Integer
        If Not DirectCast(mailDTO.Mailbox, MailboxDTO).IsPEC() Then
            Throw New InvalidOperationException("MailDTO specificato non è di tipo ""PEC"".")
        End If

        Dim pecId As Integer = CInt(mailDTO.Id)
        Dim mail As PECMail = FacadeFactory.Instance.PECMailFacade.GetById(pecId)
        Dim protocol As Protocol = FacadeFactory.Instance.ProtocolFacade.GetById(protocolDTO.UniqueId.Value)
        Dim updated As PECMail = Me.PairToProtocol(mail, protocol)
        Return updated.Id
    End Function

    Private Function GetEmailAddress(contact As Contact) As String
        If Not String.IsNullOrWhiteSpace(contact.CertifiedMail) Then
            Return contact.CertifiedMail
        End If

        If Not String.IsNullOrWhiteSpace(contact.EmailAddress) Then
            Return contact.EmailAddress
        End If

        Dim message As String = "{0} non ha un indirizzo email valido."
        message = String.Format(message, contact.Description)
        Throw New DocSuiteException(message)
    End Function

    Private Function GetBiblosPdfOrDefault(document As DocumentInfo) As DocumentInfo
        If TypeOf document Is BiblosDocumentInfo Then
            Return New BiblosPdfDocumentInfo(document)
        End If

        Return document
    End Function

    Public Sub LinkToProtocol(ByVal pecMail As PECMail, ByVal protocol As Protocol)
        If pecMail.HasDocumentUnit() Then
            Dim message As String = String.Format("Questa mail ha già una unità documentale associata ({0}).", protocol.FullNumber)
            Throw New InvalidOperationException(message)
        End If

        pecMail.Year = protocol.Year
        pecMail.Number = protocol.Number
        pecMail.DocumentUnit = FacadeFactory.Instance.DocumentUnitFacade.GetById(protocol.Id)
        pecMail.RecordedInDocSuite = Convert.ToInt16(1)

        Me.Update(pecMail)

        FacadeFactory.Instance.PECMailLogFacade.InsertLog(pecMail, String.Format("[Collegamento] Pec collegata al protocollo {0}", protocol.Id.ToString()), PECMailLogType.Linked)
        FacadeFactory.Instance.ProtocolLogFacade.Insert(protocol, ProtocolLogEvent.PM, String.Format("[Collegamento] Collegata PEC n.{0} del {1} con oggetto ""{2}""", pecMail.Id, pecMail.RegistrationDate.ToLocalTime().DateTime.ToShortDateString(), pecMail.MailSubject))
    End Sub

    Public Function SendInsertPECMailCommand(pecMail As PECMail) As Guid?
        Dim protocol As Protocol = FacadeFactory.Instance.PECMailFacade.GetProtocol(pecMail)
        Dim collaboration As Collaboration = Nothing
        If protocol IsNot Nothing Then
            collaboration = FacadeFactory.Instance.CollaborationFacade.GetByProtocol(protocol)
        End If
        Return SendInsertPECMailCommand(pecMail, protocol, collaboration)
        Return Nothing
    End Function

    Public Function SendInsertPECMailCommand(pecMail As PECMail, protocol As Protocol, collaboration As Collaboration) As Guid?
        Try
            Dim commandInsert As ICommandCreatePECMail = PreparePECCommand(Of ICommandCreatePECMail)(pecMail, protocol, collaboration, Function(tenantName, tenantId, tenantAOOId, collaborationUniqueId, collaboraitonId, collaborationTemplateName, protocolUniqueId, protocolYear, protocolNumber, identity, apiPECMail)
                                                                                                                                           Return New CommandCreatePECMail(tenantName, tenantId, tenantAOOId, collaborationUniqueId, collaboraitonId, collaborationTemplateName, protocolUniqueId, protocolYear, protocolNumber, False, identity, apiPECMail)
                                                                                                                                       End Function)
            CommandInsertFacade.Push(commandInsert)
            Return commandInsert.Id
        Catch ex As Exception
            FileLogger.Error(LoggerName, String.Concat("SendInsertPECMailCommand => ", ex.Message), ex)
        End Try

        Return Nothing
    End Function

    Public Sub SendPECMailCreatedEvent(pecMail As PECMail)

        Dim protocol As Protocol = Nothing
        Dim collaboration As Collaboration = Nothing
        If pecMail.HasDocumentUnit() AndAlso pecMail.DocumentUnit.Environment = DSWEnvironment.Protocol Then
            protocol = FacadeFactory.Instance.ProtocolFacade.GetById(pecMail.DocumentUnit.Id)
            If protocol IsNot Nothing Then
                collaboration = FacadeFactory.Instance.CollaborationFacade.GetByProtocol(protocol)
            End If
        End If
        SendPECMailCreatedEvent(pecMail, protocol, collaboration)

    End Sub

    Public Sub SendPECMailCreatedEvent(pecMail As PECMail, protocol As Protocol, collaboration As Collaboration)
        Try
            Dim eventCreatePECMail As IEventCreatePECMail = PreparePECEvent(Of IEventCreatePECMail)(pecMail, protocol, collaboration, Function(tenantName, tenantId, tenantAOOId, collaborationUniqueId, collaboraitonId, collaborationTemplateName, protocolUniqueId, protocolYear, protocolNumber, isInvoice, identity, apiPECMail)
                                                                                                                                          Return New EventCreatePECMail(tenantName, tenantId, tenantAOOId, collaborationUniqueId, collaboraitonId, collaborationTemplateName, protocolUniqueId, protocolYear, protocolNumber, isInvoice, identity, apiPECMail, Nothing)
                                                                                                                                      End Function)

            WebAPIImpersonatorFacade.ImpersonateSendRequest(WebAPIHelper, eventCreatePECMail, DocSuiteContext.Current.CurrentTenant.WebApiClientConfig, DocSuiteContext.Current.CurrentTenant.OriginalConfiguration)
        Catch ex As Exception
            FileLogger.Error(LoggerName, String.Concat("SendPECMailCreatedEvent => ", ex.Message), ex)
        End Try

    End Sub

    Private Function PreparePECMessage(Of T As {IMessage})(pecMail As PECMail, protocol As Protocol, collaboration As Collaboration,
                                                          initializeFunc As Func(Of String, Guid, Guid, Guid?, Integer?, String, Guid?, Short?, Integer?, IdentityContext, APIPECMail.PECMail, T)) As T
        Dim apiPECMail As APIPECMail.PECMail = MapperPECMailEntity.MappingDTO(pecMail)
        Dim identity As IdentityContext = New IdentityContext(DocSuiteContext.Current.User.FullUserName)
        Dim tenantName As String = CurrentTenant.TenantName
        Dim tenantId As Guid = CurrentTenant.UniqueId
        Dim tenantAOOId As Guid = CurrentTenant.TenantAOO.UniqueId
        Dim collaborationId As Integer? = Nothing
        Dim colalborationTemplateName As String = String.Empty
        Dim collaborationUniqueId As Guid?
        Dim protocolUniqueId As Guid?
        Dim protocolYear As Short? = Nothing
        Dim protocolNumber As Integer? = Nothing

        If collaboration IsNot Nothing Then
            collaborationId = collaboration.Id
            colalborationTemplateName = collaboration.TemplateName
            collaborationUniqueId = collaboration.UniqueId
        End If

        If protocol IsNot Nothing Then
            protocolUniqueId = protocol.UniqueId
            protocolYear = protocol.Year
            protocolNumber = protocol.Number
        End If
        Return initializeFunc(tenantName, tenantId, tenantAOOId, collaborationUniqueId, collaborationId, colalborationTemplateName, protocolUniqueId, protocolYear, protocolNumber, identity, apiPECMail)
    End Function

    Private Function PreparePECCommand(Of T As {ICommand})(pecMail As PECMail, protocol As Protocol, collaboration As Collaboration,
                                                          commandInitializeFunc As Func(Of String, Guid, Guid, Guid?, Integer?, String, Guid?, Short?, Integer?, IdentityContext, APIPECMail.PECMail, T)) As T

        Return PreparePECMessage(Of T)(pecMail, protocol, collaboration, commandInitializeFunc)
    End Function

    Private Function PreparePECEvent(Of T As {IEvent})(pecMail As PECMail, protocol As Protocol, collaboration As Collaboration,
                                                          eventInitializeFunc As Func(Of String, Guid, Guid, Guid?, Integer?, String, Guid?, Short?, Integer?, Boolean, IdentityContext, APIPECMail.PECMail, T)) As T

        Dim isInvoice As Boolean = IsPECMailInvoice(pecMail)
        Return PreparePECMessage(Of T)(pecMail, protocol, collaboration, Function(tenantName, tenantId, tenantAOOId, collaborationUniqueId, collaboraitonId, collaborationTemplateName, protocolUniqueId, protocolYear, protocolNumber, identity, apiPECMail)
                                                                             Return eventInitializeFunc(tenantName, tenantId, tenantAOOId, collaborationUniqueId, collaboraitonId, collaborationTemplateName, protocolUniqueId, protocolYear, protocolNumber, isInvoice, identity, apiPECMail)
                                                                         End Function)
    End Function
#End Region

End Class