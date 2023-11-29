<Serializable()>
Public Class ResolutionKind
    Inherits DomainObject(Of Guid)
    Implements IAuditable

#Region "Fields"

    Private _resolutionKindDocumentSeries As ICollection(Of ResolutionKindDocumentSeries)


#End Region

#Region "Constructor"

    Public Sub New()
        Me.ResolutionKindDocumentSeries = New HashSet(Of ResolutionKindDocumentSeries)()
    End Sub

#End Region

#Region "Properties"

    Public Overridable Property Name As String

    Public Overridable Property IsActive As Boolean

    Public Overridable Property AmountEnabled As Boolean

    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate

    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser

    Public Overridable Property RegistrationDate As DateTimeOffset Implements IAuditable.RegistrationDate

    Public Overridable Property RegistrationUser As String Implements IAuditable.RegistrationUser

#End Region

#Region "Navigation Properties"

    Public Overridable Property ResolutionKindDocumentSeries() As ICollection(Of ResolutionKindDocumentSeries)
        Get
            Return _resolutionKindDocumentSeries
        End Get
        Set(value As ICollection(Of ResolutionKindDocumentSeries))
            _resolutionKindDocumentSeries = value
        End Set
    End Property

#End Region

End Class
