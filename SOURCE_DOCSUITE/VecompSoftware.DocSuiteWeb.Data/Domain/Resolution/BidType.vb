<Serializable()> _
Public Class BidType
    Inherits DomainObject(Of Int32)

#Region "private data"
    Private _acronym As String
    Private _description As String
#End Region

#Region "Properties"

    Public Overridable Property Acronym() As String
        Get
            Return _acronym
        End Get
        Set(ByVal value As String)
            _acronym = value
        End Set
    End Property
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

