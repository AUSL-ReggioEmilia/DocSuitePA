''' <summary>
''' Eccezione generica d'applicazione
''' </summary>
Public Class PubblicationException
    Inherits Exception

    Public Sub New(ByVal message As String)
        MyBase.New(message)

    End Sub

    Public Sub New(ByVal message As String, ByVal inner As Exception)
        MyBase.New(message, inner)

    End Sub

End Class
