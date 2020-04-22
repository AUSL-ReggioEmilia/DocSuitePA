Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Logging

<ComponentModel.DataObject()>
Public Class CollaborationLogFacade
    Inherits BaseProtocolFacade(Of CollaborationLog, Integer, NHibernateCollaborationLogDao)

#Region " Constructors "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "



    ''' <summary> Inserimento per modifiche semplici. </summary>
    Public Sub Insert(ByVal coll As Collaboration, ByVal description As String)
        Insert(coll, Nothing, Nothing, Nothing, CollaborationLogType.MS, description)
    End Sub

    Public Sub Insert(ByVal coll As Collaboration, ByVal collaborationIncremental As Integer?, ByVal incremental As Short?, ByVal chainId As Integer?, ByVal logType As CollaborationLogType, ByVal description As String)
        Try
            Dim log As New CollaborationLog
            log.Collaboration = coll
            log.IdCollaboration = coll.Id
            log.CollaborationIncremental = collaborationIncremental
            log.Incremental = incremental
            log.IdChain = chainId
            log.LogType = logType.ToString()
            log.LogDescription = description
            log.LogDate = _dao.GetServerDate()
            log.SystemComputer = DocSuiteContext.Current.UserComputer
            log.SystemUser = DocSuiteContext.Current.User.FullUserName
            log.Program = DocSuiteContext.Program
            log.SessionId = CommonUtil.UserSessionId

            Save(log)
        Catch ex As Exception
            Factory.UserErrorFacade.Insert("SqlLogInsert", String.Concat("Errore in inserimento registrazione ", ex.Message))
            FileLogger.Error(LoggerName, "Errore in inserimento registrazione", ex)
        End Try
    End Sub

    Public Overrides Sub Save(ByRef obj As CollaborationLog)
        MyBase.Save(obj)
    End Sub

    Public Function IsInsertedByCurrentProgram(collaboration As Collaboration) As Boolean
        Return Me._dao.IsInsertedByCurrentProgram(collaboration)
    End Function


#End Region

End Class