''' <summary> Finta eccezione per mostrare informazioni </summary>
Public Class InformationException
    Inherits DocSuiteException

    ' TODO: eccezioni per mostrare informazioni? trovare un modo per eliminare questa brutta idea

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal msg As String)
        MyBase.New(msg)
    End Sub

    Public Sub New(ByVal title As String, ByVal msg As String)
        MyBase.New(title, msg)
    End Sub

    Public Sub New(ByVal title As String, ByVal msg As String, ByVal redirectUriAddress As Uri, ByVal redirectUriLatency As Integer)
        MyBase.New(title, msg, redirectUriAddress, redirectUriLatency)
    End Sub

End Class
