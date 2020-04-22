<Serializable()> _
Public Class DocumentTabToken
    Inherits DomainObject(Of String)


#Region "private data"
    Private _description As String
    Private _sendMail As Short

#End Region

#Region "Properties"
    Public Overridable Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property
    Public Overridable Property SendMail() As Short
        Get
            Return _sendMail
        End Get
        Set(ByVal value As Short)
            _sendMail = value
        End Set
    End Property

#End Region

#Region "Ctor/init"
    Public Sub New()

    End Sub
#End Region

End Class

