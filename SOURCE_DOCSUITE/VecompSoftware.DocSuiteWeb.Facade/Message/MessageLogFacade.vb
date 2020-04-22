
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class MessageLogFacade
    Inherits BaseProtocolFacade(Of MessageLog, Integer, NHibernateMessageLogDAO)


    Public Sub InsertLog(ByVal message As DSWMessage, ByVal info As String, ByVal type As MessageLog.MessageLogType)
        Dim log As New MessageLog()
        log.LogDate = Date.Now
        log.Description = info
        log.Message = message
        log.SystemComputer = DocSuiteContext.Current.UserComputer
        log.SystemUser = DocSuiteContext.Current.User.FullUserName
        log.Type = type

        Save(log)
    End Sub

End Class
