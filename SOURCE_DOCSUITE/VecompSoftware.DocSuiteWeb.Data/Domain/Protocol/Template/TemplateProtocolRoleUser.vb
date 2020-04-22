Imports VecompSoftware.Helpers.NHibernate

Partial Public Class TemplateProtocolRoleUser
    Inherits DomainObject(Of TemplateProtocolRoleUserCompositeKey)

#Region "Fields"
    Private _account As String
    Private _templateProtocol As TemplateProtocol
    Private _role As Role
    Private _isActive As Short
#End Region

#Region "Properties"    
    Public Overridable Property Account As String
        Get
            Return _account
        End Get
        Set(value As String)
            _account = value
        End Set
    End Property

    Public Overridable Property IsActive As Short
        Get
            Return _isActive
        End Get
        Set(value As Short)
            _isActive = value
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
        Id = New TemplateProtocolRoleUserCompositeKey()
    End Sub

    Public Sub New(key As TemplateProtocolRoleUserCompositeKey)
        Id = key
    End Sub
#End Region

End Class
