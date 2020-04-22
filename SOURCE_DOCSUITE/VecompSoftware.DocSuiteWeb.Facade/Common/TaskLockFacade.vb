
Imports System
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class TaskLockFacade
    Inherits CommonFacade(Of TaskLock, TaskLockCompositeKey, NHibernateTaskLockDao)
    Implements ITaskExecutionManager

    Private _sessionId As Guid = Guid.NewGuid()

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal sessionId As Guid)
        MyBase.New()
        Me._sessionId = sessionId
    End Sub

    Public Sub New(ByVal DbName As String)
        MyBase.New(DbName)
    End Sub

    Public ReadOnly Property SessionId() As Guid Implements ITaskExecutionManager.SessionId
        Get
            Return _sessionId
        End Get
    End Property

    'Public Function GetContactByParentId(ByVal ParentId As Integer) As IList(Of Contact)
    '    Return _dao.GetContactByParentId(ParentId)
    'End Function

    'Protected Function GetContactHierarchy(ByRef contactList As IList(Of Contact), ByRef hierarchy As ArrayList) As Integer

    '    Dim lst As IList(Of Contact) = _dao.GetParentsForContactList(ConvertsContactToIds(contactList))
    '    If lst.Count > 0 Then
    '        hierarchy.Insert(0, lst)
    '        GetContactHierarchy(lst, hierarchy)
    '    End If

    '    Return hierarchy.Count
    'End Function
    Public Function OpenProcess(ByVal processType As String) As Boolean Implements ITaskExecutionManager.OpenProcess
        If _dao.GetTasksByType(processType).Count = 0 Then
            ' insert
            Dim tl As New TaskLock()
            tl.Id = New TaskLockCompositeKey(processType, "", 0, Me._sessionId)
            tl.State = "O"c

            _dao.Save(tl)
            Return True
        End If

        Return False
    End Function

    Public Function SaveFailedTask(ByVal task As TaskLock) As Boolean Implements ITaskExecutionManager.SaveFailedTask
        task.Id.Session = Me._sessionId
        _dao.Save(task)
    End Function

    Public Function GetFailedTasksByTaskType(ByVal taskType As String) As IList(Of TaskLock) Implements ITaskExecutionManager.GetFailedTasksByTaskType
        Return _dao.GetTasksByType(taskType)
    End Function

    Public Function GetLastFailedStep2Task(ByVal taskType As String, ByVal taskId As String) As TaskLock Implements ITaskExecutionManager.GetLastFailedStep2Task
        Return _dao.GetLastTaskWithDataByTaskId(taskType, taskId)
    End Function

    Public Function GetCurrentFailedTasks() As IList(Of TaskLock) Implements ITaskExecutionManager.GetCurrentFailedTasks
        Return _dao.GetTasksBySession(Me._sessionId)
    End Function

    Public Function DeleteFailedTasksByTaskType(ByVal taskType As String, Optional ByRef failedTasks As IList(Of TaskLock) = Nothing) As Boolean Implements ITaskExecutionManager.DeleteFailedTasksByTaskType
        Dim failTasks As IList(Of TaskLock) = _dao.DeleteTasksByType(taskType)

        If failedTasks IsNot Nothing Then failedTasks = failTasks

        Return (failTasks Is Nothing)
    End Function

    Public Sub DeleteFailedTask(ByVal task As TaskLock) Implements ITaskExecutionManager.DeleteFailedTask
        _dao.Delete(task)
    End Sub

    Public Function CloseProcess(ByVal processType As String) As Boolean Implements ITaskExecutionManager.CloseProcess

        ' cancello tutti i task appartenenti alle sessioni vecchie
        _dao.DeleteOlderTasks(Me._sessionId)

        _dao.DeleteTasksByType(processType)
    End Function
End Class
