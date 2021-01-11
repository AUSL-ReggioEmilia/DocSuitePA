Imports System.Threading
Imports System.Collections.Generic
Imports System.Web

Public Class MultiStepLongRunningTask

#Region "Fields"

    Delegate Sub TaskToExec(ByVal currentStep As Int32)
    Delegate Sub ListTaskToExecDelegate(ByVal item As Object)
    Delegate Function SetCurrentFileNameDelegate(ByVal currentStep As Int32) As String
    Delegate Sub UpdateControlDelegate()
    Delegate Sub TaskCompleteDelegate()
    Delegate Sub TaskSuccessfullyCompletedDelegate()
    Delegate Sub AllTasksCompletedDelegate()
    Delegate Function ListToIterateDelegate() As IList

    Private _task As MultiStepLongRunningTask.TaskToExec
    Private _setCurrent As MultiStepLongRunningTask.SetCurrentFileNameDelegate
    Private _updateControl As MultiStepLongRunningTask.UpdateControlDelegate
    Private _taskCompleteHandler As MultiStepLongRunningTask.TaskCompleteDelegate
    Private _taskSuccessfullyCompletedHandler As MultiStepLongRunningTask.TaskSuccessfullyCompletedDelegate
    Private _allTasksCompletedHander As MultiStepLongRunningTask.AllTasksCompletedDelegate
    Private _listTaskToExecHandler As MultiStepLongRunningTask.ListTaskToExecDelegate

    Private _running As Boolean = False
    Private _lastTaskSuccess As Boolean = True
    Private _exceptionOccured As Exception = Nothing
    Private _lastStartTime As DateTime = DateTime.MinValue
    Private _lastFinishTime As DateTime = DateTime.MinValue
    Private _currentStep As Integer
    Private _currentFileName As String
    Private _stepsCount As Integer
    Private _taskUser As String = String.Empty
    Private _taskName As String = String.Empty

    Private _shouldStop As Boolean = False
    Private _taskInterrupt As Boolean = False
#End Region

#Region "Properties"
    Public ReadOnly Property TaskInterrupt() As Boolean
        Get
            Return _taskInterrupt
        End Get
    End Property

    Public Property TaskToExecute() As MultiStepLongRunningTask.TaskToExec
        Get
            Return _task
        End Get
        Set(ByVal Value As MultiStepLongRunningTask.TaskToExec)
            _task = Value
        End Set
    End Property

    Public Property TaskCompleteHandler() As MultiStepLongRunningTask.TaskCompleteDelegate
        Get
            Return _taskCompleteHandler
        End Get
        Set(ByVal Value As MultiStepLongRunningTask.TaskCompleteDelegate)
            _taskCompleteHandler = Value
        End Set
    End Property

    Public Property ListTaskToExecuteHandler() As MultiStepLongRunningTask.ListTaskToExecDelegate
        Get
            Return _listTaskToExecHandler
        End Get
        Set(ByVal Value As MultiStepLongRunningTask.ListTaskToExecDelegate)
            _listTaskToExecHandler = Value
        End Set
    End Property

    Public Property TaskSuccessfullyCompletedHandler() As MultiStepLongRunningTask.TaskSuccessfullyCompletedDelegate
        Get
            Return _taskSuccessfullyCompletedHandler
        End Get
        Set(ByVal Value As MultiStepLongRunningTask.TaskSuccessfullyCompletedDelegate)
            _taskSuccessfullyCompletedHandler = Value
        End Set
    End Property

    Public Property AllTasksCompletedHandler() As MultiStepLongRunningTask.AllTasksCompletedDelegate
        Get
            Return _allTasksCompletedHander
        End Get
        Set(ByVal Value As MultiStepLongRunningTask.AllTasksCompletedDelegate)
            _allTasksCompletedHander = Value
        End Set
    End Property

    Public ReadOnly Property Running() As Boolean
        Get
            Return _running
        End Get
    End Property

    Public ReadOnly Property LastTaskSuccess() As Boolean
        Get
            Return _lastTaskSuccess
        End Get
    End Property

    Public ReadOnly Property ExceptionOccured() As Exception
        Get
            Return _exceptionOccured
        End Get
    End Property

    Public ReadOnly Property LastStartTime() As DateTime
        Get
            If _lastStartTime = DateTime.MinValue Then
                Throw New InvalidOperationException("The task has never started.")
            End If
            Return _lastStartTime
        End Get
    End Property

    Public ReadOnly Property LastFinishTime() As DateTime
        Get
            If _lastFinishTime = DateTime.MinValue Then
                Throw New InvalidOperationException("The task has never completed.")
            End If
            Return _lastFinishTime
        End Get
    End Property

    Public ReadOnly Property CurrentStep() As Integer
        Get
            Return _currentStep
        End Get
    End Property

    ' TODO: Sarebbe da rendere readonly e passare il parametro nel costruttore, possibili problemi di race condition, analisi di impatto
    Public Property StepsCount() As Integer
        Get
            Return _stepsCount
        End Get
        Set(ByVal Value As Integer)
            _stepsCount = Value
        End Set
    End Property

    Public Property TaskUser() As String
        Get
            Return _taskUser
        End Get
        Set(ByVal Value As String)
            _taskUser = Value
        End Set
    End Property

    Public Property TaskName() As String
        Get
            Return _taskName
        End Get
        Set(ByVal Value As String)
            _taskName = Value
        End Set
    End Property

    Public Property SetCurrentFileName() As SetCurrentFileNameDelegate
        Get
            Return _setCurrent
        End Get
        Set(ByVal value As SetCurrentFileNameDelegate)
            _setCurrent = value
        End Set
    End Property

    Public ReadOnly Property CurrentFileName() As String
        Get
            If _running Then
                Return _currentFileName
            Else
                Return String.Empty
            End If
        End Get
    End Property

    Public Property UpdateControl() As UpdateControlDelegate
        Get
            Return _updateControl
        End Get
        Set(ByVal value As UpdateControlDelegate)
            _updateControl = value
        End Set
    End Property

#End Region

    Public Sub RequestStop()
        _shouldStop = True
    End Sub

    Public Sub RunTask()
        SyncLock Me
            If Not _running Then
                _running = True
                _lastStartTime = DateTime.Now

                Dim ctx As HttpContext = HttpContext.Current
                Dim newThread As New Thread(New ThreadStart(Sub()
                                                                HttpContext.Current = ctx
                                                                DoWork()
                                                            End Sub))

                newThread.Start()
                _currentStep = 0
            Else
                Throw New InvalidOperationException("The task is already running!")
            End If
        End SyncLock
    End Sub

    Public Sub RunTasks(ByVal lists As MultiStepLongRunningTask.ListToIterateDelegate())
        SyncLock Me
            If Not _running Then
                _stepsCount = 0
                _currentStep = 0

                If lists IsNot Nothing AndAlso lists.Length > 0 Then
                    _running = True
                    _lastStartTime = DateTime.Now

                    Dim index As Short
                    Dim events As New List(Of AutoResetEvent)

                    For Each listHandler As MultiStepLongRunningTask.ListToIterateDelegate In lists
                        Dim resetEvent As New AutoResetEvent(False)
                        Dim state As New TaskStateObject(index, listHandler, resetEvent) 'oggetto di stato passato al worker thread comprendente identificativo, delegate per ottenere la lista degli elementi su cui lavorare, evento di completamento
                        Dim newThread As New Thread(New ParameterizedThreadStart(AddressOf GetListAndDoWork))
                        newThread.Start(state)

                        events.Add(resetEvent)
                    Next

                    ' thread di verifica completamento dei tasks
                    Dim completeThread As New Thread(New ParameterizedThreadStart(AddressOf AllTasksCompleted))
                    completeThread.Start(events)
                End If
            Else
                Throw New InvalidOperationException("The task is already running!")
            End If
        End SyncLock
    End Sub

    Private Sub DoWork()

        Try
            Dim i As Int32
            For i = 0 To StepsCount - 1
                If (_shouldStop) Then
                    _shouldStop = False
                    _taskInterrupt = True
                    Exit For
                End If

                If Not _setCurrent Is Nothing Then
                    _currentFileName = _setCurrent(i)
                End If

                If Not _task Is Nothing Then
                    _task(i)
                End If

                _currentStep = _currentStep + 1

                If Not _updateControl Is Nothing Then
                    _updateControl()
                End If

            Next

            _lastTaskSuccess = True

        Catch e As Exception
            _lastTaskSuccess = False
            _exceptionOccured = e
        Finally
            _running = False
            _lastFinishTime = DateTime.Now
        End Try

    End Sub

    Private Sub GetListAndDoWork(ByVal state As Object)

        Dim thrdState As TaskStateObject
        Try

            If state IsNot Nothing AndAlso TypeOf state Is TaskStateObject Then

                thrdState = DirectCast(state, TaskStateObject)
                Dim listHandler As MultiStepLongRunningTask.ListToIterateDelegate = thrdState.ListHandler
                Dim list As IList = listHandler() 'richiedo la lista di elementi da passare al delegate di esecuzione del task

                If list IsNot Nothing AndAlso list.Count > 0 Then

                    SyncLock Me
                        Me.StepsCount += list.Count
                    End SyncLock

                    For Each item As Object In list

                        If _listTaskToExecHandler IsNot Nothing Then
                            _listTaskToExecHandler(item) 'chiamata al delegate di esecuzione del task

                            SyncLock Me
                                _currentStep = _currentStep + 1
                            End SyncLock

                            If _updateControl IsNot Nothing Then
                                _updateControl()
                            End If

                            If _taskSuccessfullyCompletedHandler IsNot Nothing Then
                                _taskSuccessfullyCompletedHandler()
                            End If

                            SyncLock Me
                                _lastTaskSuccess = True
                            End SyncLock

                        End If
                    Next

                End If

            End If

        Catch e As Exception
            SyncLock Me
                _lastTaskSuccess = False
                _exceptionOccured = e
            End SyncLock
        Finally
            If thrdState IsNot Nothing Then thrdState.ResetEvent.Set()

            If Not _taskCompleteHandler Is Nothing Then
                _taskCompleteHandler()
            End If
        End Try

    End Sub

    Public Sub AllTasksCompleted(ByVal state As Object)
        If state IsNot Nothing AndAlso TypeOf state Is List(Of AutoResetEvent) Then
            Dim events As List(Of AutoResetEvent) = DirectCast(state, List(Of AutoResetEvent))
            If events.Count > 0 AndAlso WaitHandle.WaitAll(events.ToArray()) Then 'aspetto tutti i worker threads
                _running = False
                _lastFinishTime = DateTime.Now

                If _allTasksCompletedHander IsNot Nothing Then
                    _allTasksCompletedHander()
                End If
            End If
        End If
    End Sub

End Class