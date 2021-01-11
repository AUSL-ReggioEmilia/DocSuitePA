''' <summary>
''' Eccezione specifica per la store procedure
''' </summary>
Public Class StoreProcedureException
    Inherits Exception

    Public Sub New(ByVal message As String)
        MyBase.New(message)

    End Sub

    Public Sub New(ByVal message As String, ByVal inner As Exception)
        MyBase.New(message, inner)

    End Sub

End Class