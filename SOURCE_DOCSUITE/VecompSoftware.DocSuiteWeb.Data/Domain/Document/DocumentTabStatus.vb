<Serializable()> _
Public Class DocumentTabStatus
    Inherits DomainObject(Of String)


#Region "private data"
    Private _description As String

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

#End Region

#Region "Ctor/init"
    Public Sub New()

    End Sub
#End Region

End Class

