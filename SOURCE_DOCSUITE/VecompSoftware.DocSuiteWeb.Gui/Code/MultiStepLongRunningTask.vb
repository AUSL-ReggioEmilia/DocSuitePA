Imports System.Threading
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

    Private _running As Boolean = False
    Private _lastTaskSuccess As Boolean = True
    Private _exceptionOccured As Exception = Nothing
    Private _lastStartTime As DateTime = DateTime.MinValue
    Private _currentStep As Integer
    Private _currentFileName As String
    Private _stepsCount As Integer
    Private _taskUser As String = String.Empty
    Private _taskName As String = String.Empty

    Private _shouldStop As Boolean = False
#End Region

#Region "Properties"

    Public Property TaskToExecute() As MultiStepLongRunningTask.TaskToExec
        Get
            Return _task
        End Get
        Set(ByVal Value As MultiStepLongRunningTask.TaskToExec)
            _task = Value
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


    Private Sub DoWork()

        Try
            Dim i As Int32
            For i = 0 To StepsCount - 1
                If (_shouldStop) Then
                    _shouldStop = False
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
        End Try

    End Sub

End Class