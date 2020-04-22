Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.API
Imports VecompSoftware.Services.Logging

Public Class TaskHeaderFacade
    Inherits FacadeNHibernateBase(Of TaskHeader, Integer, NHibernateTaskHeaderDao)

    Public Function GetByTypeAndStatus(taskType As TaskTypeEnum, status As TaskStatusEnum) As IList(Of TaskHeader)
        Return _dao.GetByTypeAndStatus(taskType, status)
    End Function

    Public Sub UpdateHeader(header As TaskHeader)
        _dao.UpdateHeader(header)
    End Sub

    Public Function CreateHeader(dto As TaskDTO) As TaskHeader
        Dim header As New TaskHeader()
        header.Code = dto.Code
        header.Title = dto.Title
        header.Description = dto.Description
        header.TaskType = DirectCast(dto.TaskType.Value, TaskTypeEnum)
        header.Status = TaskStatusEnum.Queued

        Me.Save(header)
        Return header
    End Function

    Public Function GetRecentFastMergeCodes(maxResults As Integer) As IList(Of String)
        Return Me._dao.GetRecentFastMergeCodes(maxResults)
    End Function

    Public Function Reset(taskHeaderId As Integer) As Boolean
        If taskHeaderId <= 0 Then
            Throw New ArgumentException("Task non trovato")
        End If
        Dim taskHeader As TaskHeader = _dao.GetById(taskHeaderId, True)
        Return Reset(taskHeader)
    End Function

    Public Function Reset(taskHeader As TaskHeader) As Boolean
        If taskHeader Is Nothing Then
            Throw New ArgumentException("Task non trovato")
        End If
        taskHeader.Status = TaskStatusEnum.Queued
        Update(taskHeader)
        Return True
    End Function

    Public Function GetProtocolsKey(ids As IEnumerable(Of Integer)) As IList(Of YearNumberCompositeKey)
        Return Me._dao.GetProtocolsKey(ids)
    End Function

    Public Sub ActivatePECTaskProcess(pecMail As PECMail)
        Dim taskHeader As TaskHeader = GetByPEC(pecMail)
        If taskHeader Is Nothing Then
            FileLogger.Info(LoggerName, $"Nessun task trovato per la richiesta pec {pecMail.Id}")
            Return
        End If
        ActivateTaskProcess(taskHeader)
    End Sub

    Public Sub ActivatePOLTaskProcess(request As POLRequest)
        Dim taskHeader As TaskHeader = GetByPOL(request)
        If taskHeader Is Nothing Then
            FileLogger.Info(LoggerName, $"Nessun task trovato per la richiesta posteweb {request.Id}")
            Return
        End If
        ActivateTaskProcess(taskHeader)
    End Sub

    Private Sub ActivateTaskProcess(taskHeader As TaskHeader)
        If taskHeader Is Nothing Then
            Return
        End If

        If taskHeader.SendingProcessStatus = TaskHeaderSendingProcessStatus.Todo Then
            FileLogger.Info(LoggerName, $"Attivazione task {taskHeader.Id}")
            taskHeader.SendingProcessStatus = TaskHeaderSendingProcessStatus.InProgress
            Factory.TaskHeaderFacade.Update(taskHeader)
        End If
    End Sub

    Public Sub CompleteTaskProcess(taskHeader As TaskHeader)
        If taskHeader Is Nothing Then
            Return
        End If

        If HasTaskSendingElementsToComplete(taskHeader) Then
            FileLogger.Warn(LoggerName, $"Sono ancora presenti elementi da gestire per il task {taskHeader.Id}")
            Return
        End If

        taskHeader.SendingProcessStatus = TaskHeaderSendingProcessStatus.Complete
        If taskHeader.SendedStatus Is Nothing Then
            taskHeader.SendedStatus = TaskHeaderSendedStatus.Successfully
        End If
        Factory.TaskHeaderFacade.Update(taskHeader)
    End Sub

    Public Function HasTaskSendingElementsToComplete(taskHeader As TaskHeader) As Boolean
        Return _dao.HasTaskSendingElementsToComplete(taskHeader)
    End Function

    Public Function GetByPOL(request As POLRequest) As TaskHeader
        Return _dao.GetByPOL(request)
    End Function

    Public Function GetByPEC(pecMail As PECMail) As TaskHeader
        Return _dao.GetByPEC(pecMail)
    End Function

End Class
