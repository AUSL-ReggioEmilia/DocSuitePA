Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class PECMailBoxLogFacade
    Inherits BaseProtocolFacade(Of PECMailBoxLog, Integer, NHibernatePECMailBoxLogDao)

    Public Enum PecMailBoxLogType
        Incoming = 0
        Imported = 1
        ServerRemoved = 2
        Info = 3
        Warn = 4
        ImportError = 5
        Sent = 6
        SentError = 7
        TimeEval = 8
        ErrorEval = 9
        PECErrorEval = 10
        PECReadedEval = 11
        PECDoneEval = 12
    End Enum

#Region " Constructors "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

    Public Sub IncomingMails(ByRef mailBox As PECMailBox, ByVal count As Integer)
        If count > 0 Then
            Dim desc As String = String.Format("Ci sono {0} PecMail da controllare.", count.ToString())
            InsertLog(mailBox, desc, PECMailBoxLogType.Incoming)
        End If
    End Sub

    Public Sub ImportedMail(ByRef pecMail As PECMail)
        Dim desc As String = String.Format("PEC Importata {0} con successo.", pecMail.MailUID)
        InsertLog(pecMail.MailBox, desc, PECMailBoxLogType.Imported)
    End Sub

    Public Sub RemovedFromServer(ByRef pecMail As PECMail, ByVal serverName As String)
        Dim desc As String = String.Format("PEC {0} rimossa dal server {1} con successo.", pecMail.MailUID, serverName)
        InsertLog(pecMail.MailBox, desc, PecMailBoxLogType.ServerRemoved)
    End Sub

    Public Sub SentMail(ByRef pecMail As PECMail)
        Dim desc As String = String.Format("PEC [{0}] inviata con successo.", pecMail.MailUID)
        InsertLog(pecMail.MailBox, desc, PECMailBoxLogType.Sent)
    End Sub

    Public Sub SentErrorMail(ByRef pecMail As PECMail, ByVal exception As Exception)
        Dim desc As String = String.Format("Impossibile inviare la PEC [{0}]. Errore: {1}", pecMail.MailUID, exception.Message)
        InsertLog(pecMail.MailBox, desc, PECMailBoxLogType.SentError)
    End Sub

    ''' <summary> Ritorna la linea di log con la quale la PEC è stata importata. </summary>
    Public Function GetImportItem(ByVal pecMail As PECMail) As PECMailBoxLog
        Dim desc As String = String.Format("PEC Importata {0} con successo.", pecMail.MailUID)

        Dim pecMailBoxLog As IList(Of PECMailBoxLog) = _dao.GetLogItem(desc, PECMailBoxLogType.Imported.ToString())
        If pecMailBoxLog.IsNullOrEmpty() Then
            Return Nothing
        End If
        Return pecMailBoxLog(0)
    End Function

    Public Function GetLastRecord(ByVal pecMailBoxId As Short) As Date?
        Return _dao.GetLastRecord(pecMailBoxId)
    End Function

    Public Function GetLastRecords() As IList(Of PECMailBoxReportDateDto)
        Return _dao.GetLastRecords()
    End Function

    Public Function GetLastRecordWithoutError(ByVal pecMailBoxId As Short) As Date?
        Return _dao.GetLastRecordWithoutError(pecMailBoxId)
    End Function

    Public Function GetLastRecordsWithoutError() As IList(Of PECMailBoxReportDateDto)
        Return _dao.GetLastRecordsWithoutError()
    End Function

    Public Sub Info(ByRef mailBox As PECMailBox, ByVal info As String)
        InsertLog(mailBox, info, PECMailBoxLogType.Info)
    End Sub

    Public Sub Warn(ByRef mailBox As PECMailBox, ByVal warn As String)
        InsertLog(mailBox, warn, PECMailBoxLogType.Warn)
    End Sub

    Public Sub InsertLog(ByRef mailBox As PECMailBox, ByVal info As String, ByVal type As PECMailBoxLogType)
        Dim log As New PECMailBoxLog()
        log.Date = Date.Now
        log.Description = info
        log.MailBox = mailBox
        log.SystemComputer = DocSuiteContext.Current.UserComputer
        log.SystemUser = DocSuiteContext.Current.User.FullUserName
        log.Type = type.ToString()

        Save(log)
    End Sub

End Class
