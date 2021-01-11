Public NotInheritable Class HistoryEventArgs
    Inherits EventArgs

    Private _entryName As String

    Friend Sub New(ByVal entryName As String)
        _entryName = entryName
    End Sub

    Public ReadOnly Property EntryName() As String
        Get
            Return _entryName
        End Get
    End Property
End Class

Public Delegate Sub HistoryEventHandler(ByVal sender As Object, ByVal e As HistoryEventArgs)