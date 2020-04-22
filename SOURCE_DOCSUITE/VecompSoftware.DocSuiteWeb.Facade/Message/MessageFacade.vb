Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Text
Imports VecompSoftware.Services.Biblos.Models

<ComponentModel.DataObject()> _
Public Class MessageFacade
    Inherits BaseProtocolFacade(Of DSWMessage, Integer, NHibernateMessageDAO)

    Private _facade As FacadeFactory
    Private ReadOnly Property Facade() As FacadeFactory
        Get
            If _facade Is Nothing Then
                _facade = New FacadeFactory("ProtDB")
            End If
            Return _facade
        End Get
    End Property

    Public Function GetContacts(message As DSWMessage) As IList(Of MessageContact)
        Return Facade.MessageContactFacade.GetByMessage(message)
    End Function

    Public Function GetAttachments(message As DSWMessage) As IList(Of MessageAttachment)
        Return Facade.MessageAttachmentFacade.GetByMessage(message)
    End Function

    Public Function GetEmail(message As DSWMessage) As MessageEmail
        Return Facade.MessageEmailFacade.GetByMessage(message)
    End Function

    Public Function GetMessagesToSend(type As DSWMessage.MessageTypeEnum, status As DSWMessage.MessageStatusEnum) As IList(Of DSWMessage)
        Return _dao.GetActiveMessages(type, status)
    End Function

    Public Sub AddAttachment(message As DSWMessage, document As DocumentInfo, convertToPdf As Boolean)
        Dim fl As New LocationFacade()
        Dim l As Location = fl.GetById(DocSuiteContext.Current.ProtocolEnv.MessageLocation)

        If (l IsNot Nothing) Then
            Dim saved As BiblosDocumentInfo = document.ArchiveInBiblos(l.DocumentServer, l.ProtBiblosDSDB)
            Dim ext As String = If(convertToPdf, "pdf", Nothing)
            Dim indexPosition As Integer = BiblosDocumentInfo.GetInChainPosition(l.DocumentServer, saved.DocumentParentId.Value, saved.DocumentId)
            Dim temp As New MessageAttachment(saved.Server, saved.ArchiveName, saved.BiblosChainId, indexPosition, ext)
            AddAttachment(message, temp)
        Else
            Throw New Exception("Location per l'invio e-mail non definita. Verificare il parametro ""MessageLocation""")
        End If
    End Sub

    Public Sub AddAttachment(message As DSWMessage, attachment As MessageAttachment)
        attachment.Message = message
        Facade.MessageAttachmentFacade.Save(attachment)

        Facade.MessageLogFacade.InsertLog(message, "Allegato aggiunto", MessageLog.MessageLogType.Edited)
    End Sub

    Public Function CreateMessage(type As DSWMessage.MessageTypeEnum) As DSWMessage
        Dim item As New DSWMessage(type)
        item.Location = Facade.LocationFacade.GetById(DocSuiteContext.Current.ProtocolEnv.MessageLocation)
        item.Status = DSWMessage.MessageStatusEnum.Draft
        Save(item)

        Factory.MessageLogFacade.InsertLog(item, String.Format("Creata [{0}]", DSWMessage.MessageStatusEnum.Draft.GetDescription()), MessageLog.MessageLogType.Created)
        Return item
    End Function

    Public Sub NotifyToPreviousHandler(sender As String, receiver As String, subject As String, isDispositionNotification As Boolean)
        NotifyToPreviousHandler(sender, receiver, subject, String.Empty, isDispositionNotification)
    End Sub

    ''' <summary>
    ''' Notifica via email, la presa in carico del protocollo da parte di un altro utente
    ''' </summary>
    Public Sub NotifyToPreviousHandler(sender As String, receiver As String, subject As String, body As String, isDispositionNotification As Boolean)
        Try
            ' Recupero l'email del mittente.
            Dim senderAddress As String = Facade.UserLogFacade.EmailOfUser(sender, True)

            ' Recupero l'email del destinatario.
            Dim recipientAddress As String = Facade.UserLogFacade.EmailOfUser(receiver, True)

            ' Recupero da UserLog
            If String.IsNullOrEmpty(recipientAddress) Then
                Dim item As UserLog = Facade.UserLogFacade.GetByUser(receiver, String.Empty)
                If item IsNot Nothing AndAlso Not String.IsNullOrEmpty(item.UserMail) Then
                    recipientAddress = item.UserMail
                End If
            End If

            ' Utilizzo come body l'oggetto della mail nel caso non fosse valorizzato 
            If String.IsNullOrEmpty(body) Then
                body = subject
            End If

            ' Se non trovo l'email del destinatario 
            If String.IsNullOrEmpty(recipientAddress) Then
                Throw New DocSuiteException(String.Format("Indirizzo email operatore {0} mancante o non specificato.", receiver))
            End If

#If DEBUG Then
            recipientAddress = recipientAddress.Replace("@vecompsoftware.it", "@vecompsw.emea.microsoftonline.com")
#End If

            ' Invio la mail di notifica al precedente gestore
            Select Case DocSuiteContext.Current.ProtocolEnv.NotificationHandlerType
                Case NotificationHandlerType.Smtp
                    CommonUtil.SendMail(senderAddress, recipientAddress, subject, body)
                Case NotificationHandlerType.JeepService
                    Dim contacts As New List(Of MessageContactEmail)
                    contacts.Add(Facade.MessageContactEmailFacade.CreateEmailContact(senderAddress, DocSuiteContext.Current.User.FullUserName, senderAddress, MessageContact.ContactPositionEnum.Sender))
                    contacts.Add(Facade.MessageContactEmailFacade.CreateEmailContact(recipientAddress, DocSuiteContext.Current.User.FullUserName, recipientAddress, MessageContact.ContactPositionEnum.Recipient))
                    Dim email As MessageEmail = Facade.MessageEmailFacade.CreateEmailMessage(contacts, subject, body, isDispositionNotification)
                    Facade.MessageEmailFacade.SendEmailMessage(email)
            End Select

        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore invio mail di notifica presa in carico.", ex)
            Throw New DocSuiteException("Notifica presa in carico", "Si è verificato un errore nell'invio mail di notifica presa in carico: ", ex)
        End Try
    End Sub

End Class
