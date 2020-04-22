Imports VecompSoftware.Helpers.NHibernate

<Serializable()>
Partial Public Class TemplateProtocolRole
    Inherits DomainObject(Of TemplateProtocolRoleCompositeKey)

#Region "Fields"
    Private _templateProtocol As TemplateProtocol
    Private _role As Role
    Private _rights As String
    Private _note As String
    Private _distributionType As String
    Private _type As String
#End Region

#Region "Properties"
    Public Overridable Property Rights As String
        Get
            Return _rights
        End Get
        Set(value As String)
            _rights = value
        End Set
    End Property

    Public Overridable Property Note As String
        Get
            Return _note
        End Get
        Set(value As String)
            _note = value
        End Set
    End Property

    Public Overridable Property DistributionType As String
        Get
            Return _distributionType
        End Get
        Set(value As String)
            _distributionType = value
        End Set
    End Property

    Public Overridable Property Type As String
        Get
            Return _type
        End Get
        Set(value As String)
            _type = value
        End Set
    End Property
#End Region

#Region "Navigation Properties"
    Public Overridable Property TemplateProtocol As TemplateProtocol
        Get
            Return _templateProtocol
        End Get
        Set(value As TemplateProtocol)
            _templateProtocol = value
            Id.IdTemplateProtocol = value.Id
        End Set
    End Property

    Public Overridable Property Role As Role
        Get
            Return _role
        End Get
        Set(value As Role)
            _role = value
            Id.IdRole = value.Id
        End Set
    End Property
#End Region

#Region "Constructor"
    Public Sub New()
        Id = New TemplateProtocolRoleCompositeKey()
    End Sub

    Public Sub New(key As TemplateProtocolRoleCompositeKey)
        Id = key
    End Sub
#End Region

End Class
