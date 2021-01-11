<Serializable()>
Partial Public Class TemplateAdvancedProtocol
    Inherits DomainObject(Of Int32)

#Region "Fields"
    Private _isClaim As Boolean?
    Private _subCategory As Category
    Private _status As ProtocolStatus
    Private _note As String
    Private _subject As String
    Private _serviceCategory As String
    Private _accountingSectional As String
    Private _templateProtocol As TemplateProtocol
#End Region

#Region "Properties"
    Public Overridable Property IdTemplateProtocol As Integer
        Get
            Return Id
        End Get
        Set(value As Integer)
            Id = value
        End Set
    End Property

    Public Overridable Property IsClaim As Boolean?
        Get
            Return _isClaim
        End Get
        Set(value As Boolean?)
            _isClaim = True
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

    Public Overridable Property Subject As String
        Get
            Return _subject
        End Get
        Set(value As String)
            _subject = value
        End Set
    End Property

    Public Overridable Property ServiceCategory As String
        Get
            Return _serviceCategory
        End Get
        Set(value As String)
            _serviceCategory = value
        End Set
    End Property

    Public Overridable Property AccountingSectional As String
        Get
            Return _accountingSectional
        End Get
        Set(value As String)
            _accountingSectional = value
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
            IdTemplateProtocol = value.Id
        End Set
    End Property

    Public Overridable Property SubCategory As Category
        Get
            Return _subCategory
        End Get
        Set(value As Category)
            _subCategory = value
        End Set
    End Property

    Public Overridable Property Status As ProtocolStatus
        Get
            Return _status
        End Get
        Set(value As ProtocolStatus)
            _status = value
        End Set
    End Property
#End Region

End Class
