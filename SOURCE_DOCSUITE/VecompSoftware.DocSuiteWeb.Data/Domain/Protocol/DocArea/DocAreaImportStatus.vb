<Serializable()> _
Public Class DocAreaImportStatus

    Private _code As String
    Private _description As String

    '0 è andato a buon fine....
    Public Property Code() As String
        Get
            Return _code
        End Get
        Set(ByVal value As String)
            _code = value
        End Set
    End Property

    Public Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

End Class
