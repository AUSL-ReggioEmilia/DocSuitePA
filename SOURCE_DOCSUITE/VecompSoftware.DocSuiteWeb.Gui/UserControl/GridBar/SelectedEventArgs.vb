Public Class SelectedEventArgs
    Inherits EventArgs

    Private _selected As Boolean
    Private _errorMessage As String

    Public Property Selected() As Boolean
        Get
            Return _selected
        End Get
        Set(ByVal value As Boolean)
            _selected = value
        End Set
    End Property

    Public ReadOnly Property ErrorMessage() As String
        Get
            Return _errorMessage
        End Get
    End Property

    Public Sub New(ByVal selected As Boolean)
        _selected = selected
    End Sub

    Public Sub New(ByVal selected As Boolean, ByVal errorMessage As String)
        _selected = selected
        _errorMessage = errorMessage
    End Sub
End Class