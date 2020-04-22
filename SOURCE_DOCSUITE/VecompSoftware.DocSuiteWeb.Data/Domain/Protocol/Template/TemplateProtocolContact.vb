Imports VecompSoftware.Helpers.NHibernate

<Serializable()>
Partial Public Class TemplateProtocolContact
    Inherits DomainObject(Of TemplateProtocolContactCompositeKey)

#Region "Fields"
    Private _contact As Contact
    Private _templateProtocol As TemplateProtocol
    Private _type As String
#End Region

#Region "Properties"
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
    Public Overridable Property Contact As Contact
        Get
            Return _contact
        End Get
        Set(value As Contact)
            _contact = value
            Id.IdContact = value.Id
        End Set
    End Property

    Public Overridable Property TemplateProtocol As TemplateProtocol
        Get
            Return _templateProtocol
        End Get
        Set(value As TemplateProtocol)
            _templateProtocol = value
            Id.IdTemplateProtocol = value.Id
        End Set
    End Property
#End Region

#Region " Constructor "

    Public Sub New()
        Id = New TemplateProtocolContactCompositeKey()
    End Sub

    Public Sub New(ByVal key As TemplateProtocolContactCompositeKey)
        Id = key
    End Sub

#End Region

End Class
