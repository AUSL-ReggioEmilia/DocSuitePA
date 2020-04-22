Imports VecompSoftware.Helpers.NHibernate

<Serializable()>
Partial Public Class TemplateProtocolContactManual
    Inherits DomainObject(Of TemplateProtocolContactManualCompositeKey)

#Region "Fields"
    Private _contact As Contact
    Private _templateProtocol As TemplateProtocol
    Private _comunicationType As String
    Private _type As String
    Private _idAD As String
#End Region

#Region "Properties"
    Public Overridable Property ComunicationType As String
        Get
            Return _comunicationType
        End Get
        Set(ByVal value As String)
            _comunicationType = value
        End Set
    End Property

    Public Overridable Property IdAD As String
        Get
            Return _idAD
        End Get
        Set(ByVal value As String)
            _idAD = value
        End Set
    End Property

    Public Overridable Property Type As String
        Get
            Return _type
        End Get
        Set(ByVal value As String)
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

    Public Overridable Property Contact As Contact
        Get
            Return _contact
        End Get
        Set(ByVal value As Contact)
            _contact = value
        End Set
    End Property
#End Region

#Region "Constructor"
    Public Sub New()
        Id = New TemplateProtocolContactManualCompositeKey()
    End Sub

    Public Sub New(key As TemplateProtocolContactManualCompositeKey)
        Id = key
    End Sub
#End Region

End Class
