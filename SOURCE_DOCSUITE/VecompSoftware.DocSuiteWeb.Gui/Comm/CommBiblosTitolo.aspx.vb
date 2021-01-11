Partial Class CommBiblosTitolo
    Inherits CommBasePage

#Region "Properties"
    Public ReadOnly Property Titolo() As String
        Get
            Return Request.QueryString("Titolo")
        End Get
    End Property

    Public ReadOnly Property Segnatura() As String
        Get
            Return Request.QueryString("Segnatura")
        End Get
    End Property

    Public ReadOnly Property PageType() As String
        Get
            Return Type
        End Get
    End Property
#End Region

End Class


