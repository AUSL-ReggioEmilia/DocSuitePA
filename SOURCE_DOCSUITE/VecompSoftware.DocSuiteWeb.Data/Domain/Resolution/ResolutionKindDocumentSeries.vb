<Serializable()>
Public Class ResolutionKindDocumentSeries
    Inherits AuditableDomainObject(Of Guid)

#Region "Fields"

    Private _resolutionKind As ResolutionKind
    Private _documentSeries As DocumentSeries
    Private _documentRequired As Boolean
    Private _isActive As Integer

#End Region

#Region "Properties"

    Public Overridable Property ResolutionKind() As ResolutionKind
        Get
            Return _resolutionKind
        End Get
        Set(value As ResolutionKind)
            _resolutionKind = value
        End Set
    End Property

    Public Overridable Property DocumentSeries() As DocumentSeries
        Get
            Return _documentSeries
        End Get
        Set(value As DocumentSeries)
            _documentSeries = value
        End Set
    End Property

    Public Overridable Property DocumentRequired As Boolean
        Get
            Return _documentRequired
        End Get
        Set(value As Boolean)
            _documentRequired = value
        End Set
    End Property

    Public Overridable Property IdDocumentSeriesConstraint As Guid?
#End Region

End Class
