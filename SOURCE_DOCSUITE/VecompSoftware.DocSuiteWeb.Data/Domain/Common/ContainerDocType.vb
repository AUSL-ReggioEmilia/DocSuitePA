<Serializable()> _
Public Class ContainerDocType
    Inherits DomainObject(Of Int32)

    Private _idContainerDocType As Integer
    Private _isAllowed As Boolean
    Private _container As Container
    Private _documentType As DocumentType

    Public Sub New()
        _idContainerDocType = New Int32
    End Sub

    Public Overridable Property IdContainerDocType As Integer
        Get
            Return _idContainerDocType
        End Get
        Set(ByVal value As Integer)
            _idContainerDocType = value
        End Set
    End Property

    Public Overridable Property IsAllowed As Boolean
        Get
            Return _isAllowed
        End Get
        Set(ByVal value As Boolean)
            If value = Nothing Then value = False
            _isAllowed = value
        End Set
    End Property

    Public Overridable Property Container As Container
        Get
            Return _container
        End Get
        Set(ByVal value As Container)
            _container = value
        End Set
    End Property

    Public Overridable Property DocumentTypes As DocumentType
        Get
            Return _documentType
        End Get
        Set(ByVal value As DocumentType)
            _documentType = value
        End Set
    End Property

End Class
