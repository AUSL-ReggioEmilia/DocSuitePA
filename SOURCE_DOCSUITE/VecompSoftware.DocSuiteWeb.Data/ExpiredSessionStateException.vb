''' <summary> Eccezione per gestire la Sessione Scaduta </summary>
''' <remarks> gestita a livello Suite dal web.config e Global.asax </remarks>
Public Class ExpiredSessionStateException
    Inherits DocSuiteException

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal msg As String)
        MyBase.New(msg)
    End Sub

End Class
