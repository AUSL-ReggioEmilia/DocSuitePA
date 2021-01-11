
<Serializable()> _
Public Class TaskLock
    Inherits DomainObject(Of TaskLockCompositeKey)

#Region "Properties"

    Private _state As Char = "O"c
    Overridable Property State() As Char
        Get
            Return _state
        End Get
        Set(ByVal value As Char)
            _state = value
        End Set
    End Property

    Private _message As String = ""
    Overridable Property Message() As String
        Get
            Return _message
        End Get
        Set(ByVal value As String)
            _message = value
        End Set
    End Property

    Private _created As DateTime = DateTime.Now
    Overridable Property Created() As DateTime
        Get
            Return _created
        End Get
        Set(ByVal value As DateTime)
            _created = value
        End Set
    End Property

    Private _data As String
    Overridable Property Data() As String
        Get
            Return _data
        End Get
        Set(ByVal value As String)
            _data = value
        End Set
    End Property

#End Region

End Class
