
Public Class ContBasePage
    Inherits CommonBasePage

#Region " Properties "

    Public ReadOnly Property DocType() As String
        Get
            Return Request.QueryString("DocType")
        End Get
    End Property

#End Region

End Class
