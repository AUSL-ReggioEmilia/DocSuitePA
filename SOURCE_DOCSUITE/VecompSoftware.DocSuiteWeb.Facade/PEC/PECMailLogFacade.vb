Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Linq
Imports VecompSoftware.Helpers.Signer.Security
Imports VecompSoftware.Services.Logging

<ComponentModel.DataObject()>
Public Class PECMailLogFacade
    Inherits BaseProtocolFacade(Of PECMailLog, Integer, NHibernatePECMailLogDao)

#Region " Fields "
    Private _hashHelper As HashGenerator
#End Region

#Region " Properties "
    Private ReadOnly Property HashHelper As Helpers.Signer.Security.HashGenerator
        Get
            If _hashHelper Is Nothing Then
                _hashHelper = New Helpers.Signer.Security.HashGenerator()
            End If
            Return _hashHelper
        End Get
    End Property
#End Region

#Region " Methods "


    Public Overrides Sub SaveWithoutTransaction(ByRef obj As PECMailLog)
        obj.Hash = HashHelper.GenerateHash(String.Concat(obj.SystemUser, "|", obj.Type, "|", obj.Description, "|", obj.UniqueId, "|", obj.Mail.Id, "|", obj.Date.ToString("yyyyMMddHHmmss")))
        MyBase.SaveWithoutTransaction(obj)
    End Sub

    Public Overrides Sub Save(ByRef obj As PECMailLog)
        obj.Hash = HashHelper.GenerateHash(String.Concat(obj.SystemUser, "|", obj.Type, "|", obj.Description, "|", obj.UniqueId, "|", obj.Mail.Id, "|", obj.Date.ToString("yyyyMMddHHmmss")))
        MyBase.Save(obj)
    End Sub

    Private Sub BuildLinkLog(ByRef source As PECMail, ByRef destination As PECMail, ByRef type As PECMailLogType,
                             ByRef sourceMessage As String, ByRef destinationMessage As String)
        Dim sourceLog As PECMailLog = BuildPECMailLog(source)

        If sourceLog Is Nothing Then
            FileLogger.Warn(LogName.FileLog, "BuildLinkLog has skip sourceLog to undefined PECMail record definition")
            Return
        End If

        sourceLog = sourceLog.SetLogType(type)
        sourceLog.DestinationMail = destination
        sourceLog.Description = String.Concat(sourceMessage, String.Format(" {0}", destination.MailRecipients))

        Save(sourceLog)

        Dim destinationLog As PECMailLog = BuildPECMailLog(destination)
        If destinationLog Is Nothing Then
            FileLogger.Warn(LogName.FileLog, "BuildLinkLog has skip destinationLog to undefined PECMail record definition")
            Return
        End If
        destinationLog = destinationLog.SetLogType(PECMailLogType.Linked)
        destinationLog.DestinationMail = source
        destinationLog.Description = String.Concat(destinationMessage, String.Format(" {0}", source.MailSenders))
        Save(destinationLog)
    End Sub

    Public Sub Replied(ByRef source As PECMail, ByRef reply As PECMail)
        BuildLinkLog(source, reply, PECMailLogType.Replied, "Inviata risposta a", "PEC inviata come risposta della PEC")
    End Sub

    Public Sub Forwarded(ByVal source As PECMail, ByVal forward As PECMail)
        BuildLinkLog(source, forward, PECMailLogType.Forwarded, "PEC inoltrata a", "PEC inoltrata dalla PEC originaria")
    End Sub

    Public Sub Resend(ByRef source As PECMail, ByVal cloned As PECMail)
        BuildLinkLog(source, cloned, PECMailLogType.Resend, "PEC reinviata a", "PEC reinviata dalla PEC originaria")
    End Sub

    Public Sub Sended(ByVal source As PECMail, ByVal sended As MessageEmail, ByVal contactsMail As IList(Of MessageContactEmail))
        Dim sendedLog As PECMailLog = BuildPECMailLog(source)
        If sendedLog Is Nothing Then
            FileLogger.Warn(LogName.FileLog, "Sended has skip log to undefined PECMail record definition")
            Return
        End If
        sendedLog = sendedLog.SetLogType(PECMailLogType.Sended)
        Dim mailRecipients As String = String.Join(", ", contactsMail.Select(Function(e) e.ToString()).ToArray())
        sendedLog.Description = String.Format("PEC inviata a ""{0}"" con ID {1}", mailRecipients, sended.Id)
        Save(sendedLog)
    End Sub

    Private Shared Function BuildPECMailLog(ByRef pecMail As PECMail) As PECMailLog
        If pecMail Is Nothing OrElse pecMail.Id <= 0 Then
            FileLogger.Warn(LogName.FileLog, "BuildPECMailLog has skip log to undefined PECMail record definition")
            Return Nothing
        End If
        Dim log As PECMailLog = New PECMailLog()
        log.Date = DateTime.Now
        log.Mail = pecMail
        log.SystemComputer = DocSuiteContext.Current.UserComputer
        log.SystemUser = DocSuiteContext.Current.User.FullUserName
        Return log
    End Function

#End Region

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub Created(ByRef pecMail As PECMail, Optional needTransaction As Boolean = True)
        Dim mailRecipients As String = String.Concat(New String(pecMail.MailRecipients.Take(3500).ToArray()), If(pecMail.MailRecipients.Length > 3500, " ...", String.Empty))
        InsertLog(pecMail, String.Format("PEC creata da {0} e inviata a {1}", pecMail.RegistrationUser, mailRecipients), PECMailLogType.Create, needTransaction:=needTransaction)
    End Sub

    Public Sub [Error](ByRef pecMail As PECMail, message As String)
        InsertLog(pecMail, message, PECMailLogType.Error)
    End Sub

    Public Sub Drafted(ByRef pecMail As PECMail, destinationMailBoxId As Short)
        InsertLog(pecMail, destinationMailBoxId.ToString(), PECMailLogType.Draft)
    End Sub

    Public Sub Moved(ByRef pecMail As PECMail, mbDa As PECMailBox, mbA As PECMailBox, motivation As String)
        InsertLog(pecMail, String.Format("Spostata da {0} a {1}. [{2}] ", mbDa.MailBoxName, mbA.MailBoxName, motivation), PECMailLogType.Move)
    End Sub

    Public Sub MoveNotified(ByRef original As PECMail, ByVal notification As PECMail)
        InsertLog(original, notification.Id.ToString(), PECMailLogType.MoveNotify)
        InsertLog(notification, original.Id.ToString(), PECMailLogType.Linked)
    End Sub

    Public Sub Handle(ByRef pecMail As PECMail, handler As String, status As Boolean)
        If status Then
            InsertLog(pecMail, String.Concat("Mail presa in carico da: " + handler), PECMailLogType.Modified)
        Else
            InsertLog(pecMail, String.Concat("Mail rilasciata da: " + handler), PECMailLogType.Modified)
        End If
    End Sub

    Public Sub Warning(ByRef pecMail As PECMail, message As String)
        InsertLog(pecMail, message, PECMailLogType.Warning)
    End Sub

    Public Sub ErrorLog(ByRef pecMail As PECMail, message As String)
        InsertLog(pecMail, message, PECMailLogType.Error)
    End Sub

    Public Sub Deleted(ByRef pecMail As PECMail, motivation As String)
        InsertLog(pecMail, String.Format("Mail eliminata. [{0}]", motivation), PECMailLogType.Delete)
    End Sub

    Public Sub Restored(ByRef pecMail As PECMail)
        InsertLog(pecMail, "Mail ripristinata", PECMailLogType.Restore)
    End Sub

    Public Sub AttachmentRenamed(ByRef pecMail As PECMail, idAttachment As Integer, oldName As String, newName As String)
        Dim message As String = String.Format("IdAttachment: {0}, Allegato ""{1}"" rinominato in ""{2}"".", idAttachment, oldName, newName)
        InsertLog(pecMail, message, PECMailLogType.Modified)
    End Sub

    Public Sub Read(ByRef pecMail As PECMail)
        InsertLog(pecMail, "Lettura", PECMailLogType.Read)
    End Sub

    Public Sub InsertLog(ByRef pecMail As PECMail, ByVal info As String, ByVal type As PECMailLogType, Optional needTransaction As Boolean = True)
        Dim log As PECMailLog = BuildPECMailLog(pecMail)
        If log Is Nothing Then
            FileLogger.Warn(LogName.FileLog, "InsertLog has skip log to undefined PECMail record definition")
            Return
        End If

        log.Description = info
        log.Type = type.ToString() 'Che brutto
        If (needTransaction) Then
            Save(log)
        Else
            SaveWithoutTransaction(log)
        End If

    End Sub

    Public Function GetByPec(pec As PECMail) As IList(Of PECMailLog)
        Return _dao.GetByPEC(pec)
    End Function

    Public Function GetLogTypeByPEC(pec As PECMail, logType As PECMailLogType) As PECMailLog
        Return _dao.GetLogTypeByPEC(pec, logType)
    End Function

End Class
