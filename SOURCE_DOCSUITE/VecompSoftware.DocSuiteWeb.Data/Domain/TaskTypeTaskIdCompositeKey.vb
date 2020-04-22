Imports VecompSoftware.Helpers.NHibernate

<Serializable()> _
Public Class TaskLockCompositeKey
    Inherits Quadruplet(Of String, String, Byte, Guid)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal taskType As String, ByVal taskId As String, ByVal [step] As Byte, ByVal session As Guid)
        MyBase.New(taskType, taskId, [step], session)
    End Sub

    Public Overridable Property TaskType() As String
        Get
            Return First
        End Get
        Set(ByVal value As String)
            First = value
        End Set
    End Property

    Public Overridable Property TaskId() As String
        Get
            Return Second()
        End Get
        Set(ByVal value As String)
            Second = value
        End Set
    End Property

    Overridable Property [Step]() As Byte
        Get
            Return Third
        End Get
        Set(ByVal value As Byte)
            Third = value
        End Set
    End Property

    Overridable Property Session() As Guid
        Get
            Return Forth
        End Get
        Set(ByVal value As Guid)
            Forth = value
        End Set
    End Property


    Public Overrides Function ToString() As String
        Return String.Format("{0}|{1}|{2}|{3}", TaskType, TaskId, [Step], Session)
    End Function

    Public Overrides Function GetHashCode() As Int32
        Return Me.ToString().GetHashCode()
        'Return MyBase.GetHashCode()
    End Function

End Class
