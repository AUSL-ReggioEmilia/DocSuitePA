
Imports VecompSoftware.DocSuiteWeb.Data

Public Interface ITaskExecutionManager
    Function OpenProcess(ByVal processType As String) As Boolean
    Function SaveFailedTask(ByVal task As TaskLock) As Boolean
    Function GetFailedTasksByTaskType(ByVal taskType As String) As IList(Of TaskLock)
    Function GetLastFailedStep2Task(ByVal taskType As String, ByVal taskId As String) As TaskLock
    Function GetCurrentFailedTasks() As IList(Of TaskLock)
    Function DeleteFailedTasksByTaskType(ByVal taskType As String, Optional ByRef failedTasks As IList(Of TaskLock) = Nothing) As Boolean
    Sub DeleteFailedTask(ByVal task As TaskLock)
    Function CloseProcess(ByVal processType As String) As Boolean

    ReadOnly Property SessionId() As Guid
End Interface
