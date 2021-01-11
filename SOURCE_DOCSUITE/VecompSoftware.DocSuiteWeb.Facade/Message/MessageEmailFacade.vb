Imports VecompSoftware.DocSuiteWeb.API
Imports VecompSoftware.DocSuiteWeb.Data
Imports StringHelper = VecompSoftware.Helpers.StringHelper
Imports VecompSoftware.Services.Biblos.Models
Imports System.Linq

<ComponentModel.DataObject()>
Public Class MessageEmailFacade
    Inherits BaseProtocolFacade(Of MessageEmail, Integer, NHibernateMessageEmailDAO)

    Public Function GetByMessage(message As DSWMessage) As MessageEmail
        Return _dao.GetByMessage(message)
    End Function

    Public Function CreateEmailMessage(contacts As IList(Of MessageContactEmail), subject As String, body As String,
                                       isDispositionNotification As Boolean, Optional ByVal signatureBelow As String = "") As MessageEmail

        Return CreateEmailMessage(contacts, New List(Of MessageAttachment), subject, body, isDispositionNotification, signatureBelow)
    End Function

    Public Function CreateEmailMessage(contacts As IList(Of MessageContactEmail), attachments As IList(Of MessageAttachment), subject As String, body As String,
                                       isDispositionNotification As Boolean, Optional ByVal signatureBelow As String = "") As MessageEmail
        Dim message As DSWMessage = Factory.MessageFacade.CreateMessage(DSWMessage.MessageTypeEnum.Email)
        ' Aggiunta della firma in calce alla mail
        If Not String.IsNullOrEmpty(signatureBelow) Then
            body = String.Concat(body, Environment.NewLine, signatureBelow)
        End If

        subject = If(String.IsNullOrEmpty(subject), String.Empty, StringHelper.ReplaceCrLf(subject))
        Dim email As New MessageEmail(message, subject, body, isDispositionNotification)
        Save(email)

        ' Aggiorno contatti
        For Each contact As MessageContactEmail In contacts
            contact.MessageContact.Message = message

            Factory.MessageContactFacade.Save(contact.MessageContact)
            Factory.MessageContactEmailFacade.Save(contact)
        Next

        If attachments.Count() > 0 Then
            For Each attachment As MessageAttachment In attachments
                attachment.Message = message

                Factory.MessageAttachmentFacade.Save(attachment)
            Next
        End If

        Return email
    End Function

    ''' <summary> Attiva l'invio del messaggio e ne restituisce l'id </summary>
    Public Function SendEmailMessage(email As MessageEmail) As Integer
        email.Message.Status = DSWMessage.MessageStatusEnum.Active ' Messaggio da spedire

        Factory.MessageFacade.Update(email.Message)
        Factory.MessageLogFacade.InsertLog(email.Message, "Messaggio abilitato per la spedizione", MessageLog.MessageLogType.Edited)

        Return email.Id
    End Function

    Public Function GetDocuments(msg As MessageEmail) As IList(Of DocumentInfo)
        '' Creo l'albero principale
        Dim mainFolder As New FolderInfo() With {.Name = "E-mail"}

        '' Aggiungo il documento principale (se calcolabile)
        If msg.EmlDocumentId.HasValue AndAlso msg.Message.Location IsNot Nothing Then
            Dim mainMessage As New FolderInfo() With {.Name = "Messaggio"}
            Dim docInfo As DocumentInfo
            Dim mainDocument As BiblosDocumentInfo = BiblosDocumentInfo.GetDocumentInfo(msg.EmlDocumentId.Value, Nothing, True, True).FirstOrDefault()
            docInfo = mainDocument
            If mainDocument.IsRemoved Then
                docInfo = New BiblosDeletedDocumentInfo(mainDocument.DocumentId)
            End If
            mainMessage.AddChild(docInfo)
            mainFolder.AddChild(mainMessage)
        End If

        '' Aggiungo gli allegati
        Dim attachmentFolder As New FolderInfo() With {.Name = "Allegati"}
        attachmentFolder.AddChildren(Factory.MessageAttachmentFacade.GetByMessageAsDocumentInfoList(msg.Message))

        '' Aggancio gli allegati solo se effettivamente ne sono stati calcolati
        If attachmentFolder.HasChildren Then
            mainFolder.AddChild(attachmentFolder)
        End If

        '' Aggancio l'albero principale all'oggetto di ritorno
        Dim tor As New List(Of DocumentInfo)
        tor.Add(mainFolder)
        Return tor
    End Function

    Public Function CreateEmailMessage(ByVal dto As MailDTO, toPdf As Boolean) As MailDTO
        Dim listContacts As New List(Of MessageContactEmail)()
        Dim listAttachments As New List(Of MessageAttachment)()
        Dim sender As MessageContactEmail = Factory.MessageContactEmailFacade.CreateEmailContact(dto.Sender.EmailAddress, DocSuiteContext.Current.User.FullUserName, dto.Sender.EmailAddress, MessageContact.ContactPositionEnum.Sender)
        Dim locationFacade As New LocationFacade()
        Dim location As Location = locationFacade.GetById(DocSuiteContext.Current.ProtocolEnv.MessageLocation)
        Dim savedDocument As BiblosDocumentInfo
        Dim document As MessageAttachment

        listContacts.Add(sender)
        For Each contact As IContactDTO In dto.GetAllRecipients()
            Dim recipient As MessageContactEmail = Factory.MessageContactEmailFacade.CreateEmailContact(contact.EmailAddress, DocSuiteContext.Current.User.FullUserName, contact.EmailAddress, MessageContact.ContactPositionEnum.Recipient)
            listContacts.Add(recipient)
        Next

        If dto.HasAttachments() Then
            Dim attachments As List(Of DocumentInfo) = dto.Attachments().SelectMany(Function(d) d.ToDocumentInfos()).ToList()
            If toPdf Then
                attachments = attachments.Select(Function(d) Me.GetBiblosPdfOrDefault(d)).ToList()
            End If

            For Each attachment As DocumentInfo In attachments
                savedDocument = attachment.ArchiveInBiblos(location.ProtBiblosDSDB)
                document = New MessageAttachment(savedDocument.ArchiveName, savedDocument.BiblosChainId, 0, Nothing)
                listAttachments.Add(document)
            Next
        End If

        Dim email As MessageEmail = CreateEmailMessage(listContacts, listAttachments, dto.Subject, dto.Body, False)

        Dim result As New MailDTO()
        result.Id = Factory.MessageEmailFacade.SendEmailMessage(email).ToString()
        Return result
    End Function

    Private Function GetBiblosPdfOrDefault(document As DocumentInfo) As DocumentInfo
        If TypeOf document Is BiblosDocumentInfo Then
            Return New BiblosPdfDocumentInfo(document)
        End If

        Return document
    End Function
End Class
