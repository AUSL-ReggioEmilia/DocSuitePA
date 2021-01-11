Imports System.Threading

Public Class TaskStateObject
    Private _id As Short
    Private _listHandler As Object
    Private _resetEvent As AutoResetEvent

    Public Sub New(ByVal id As Short, ByVal listHandler As MultiStepLongRunningTask.ListToIterateDelegate, ByVal resetEvent As AutoResetEvent)
        _id = id
        _listHandler = listHandler
        _resetEvent = resetEvent
    End Sub

    Public ReadOnly Property Id() As Short
        Get
            Return _id
        End Get
    End Property

    Public ReadOnly Property ListHandler() As MultiStepLongRunningTask.ListToIterateDelegate
        Get
            Return _listHandler
        End Get
    End Property

    Public ReadOnly Property ResetEvent() As AutoResetEvent
        Get
            Return _resetEvent
        End Get
    End Property
End Class