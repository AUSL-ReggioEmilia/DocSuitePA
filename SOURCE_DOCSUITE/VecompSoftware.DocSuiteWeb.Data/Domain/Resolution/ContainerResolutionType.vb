<Serializable()> _
Public Class ContainerResolutionType
    Inherits DomainObject(Of ContainerResolutionTypeCompositeKey)

#Region "Private Fields"
    Private _resolutionType As ResolutionType
    Private _container As Container
#End Region

#Region "Properties"
    Public Overridable Property resolutionType() As ResolutionType
        Get
            Return _resolutionType
        End Get
        Set(ByVal value As ResolutionType)
            _resolutionType = value
        End Set
    End Property
    Public Overridable Property container() As Container
        Get
            Return _container
        End Get
        Set(ByVal value As Container)
            _container = value
        End Set
    End Property
#End Region

    Public Sub New()
        Id = New ContainerResolutionTypeCompositeKey()
    End Sub
End Class
