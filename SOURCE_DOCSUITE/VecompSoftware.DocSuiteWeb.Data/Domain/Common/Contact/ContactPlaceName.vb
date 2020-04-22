<Serializable()> _
Public Class ContactPlaceName
    Inherits DomainObject(Of Int32)


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

#Region " Constructor "
    Public Sub New()
        _description = ""
        UniqueId = Guid.NewGuid()
    End Sub
#End Region

End Class

