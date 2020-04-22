<Serializable()>
Partial Public Class TemplateProtocol
    Inherits DomainObject(Of Int32)
    Implements IAuditable
    Implements IFormattable

#Region "Fields"
    Private _templateName As String
    Private _isDefault As Boolean?
    Private _idTemplateStatus As Short
    Private _container As Container
    Private _category As Category
    Private _docType As DocumentType
    Private _object As String
    Private _type As ProtocolType
    Private _idProtocolKind As Short?
    Private _templateAdvancedProtocol As TemplateAdvancedProtocol
    Private _lastChangedDate As DateTimeOffset?
    Private _lastChangedUser As String
    Private _registrationDate As DateTimeOffset
    Private _registrationUser As String


    Private _contacts As IList(Of TemplateProtocolContact)
    Private _contactsManual As IList(Of TemplateProtocolContactManual)
    Private _roles As IList(Of TemplateProtocolRole)
    Private _roleUsers As IList(Of TemplateProtocolRoleUser)

#End Region

#Region "Properties"
    Public Overridable Property TemplateName As String
        Get
            Return _templateName
        End Get
        Set(value As String)
            _templateName = value
        End Set
    End Property

    Public Overridable Property IsDefault As Boolean?
        Get
            Return _isDefault
        End Get
        Set(value As Boolean?)
            _isDefault = value
        End Set
    End Property

    Public Overridable Property IdTemplateStatus As Short
        Get
            Return _idTemplateStatus
        End Get
        Set(value As Short)
            _idTemplateStatus = value
        End Set
    End Property

    Public Overridable Property ProtocolObject As String
        Get
            Return _object
        End Get
        Set(value As String)
            _object = value
        End Set
    End Property

    Public Overridable Property IdProtocolKind As Short?
        Get
            Return _idProtocolKind
        End Get
        Set(value As Short?)
            _idProtocolKind = value
        End Set
    End Property

    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate
        Get
            Return _lastChangedDate
        End Get
        Set(value As DateTimeOffset?)
            _lastChangedDate = value
        End Set
    End Property

    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser
        Get
            Return _lastChangedUser
        End Get
        Set(value As String)
            _lastChangedUser = value
        End Set
    End Property

    Public Overridable Property RegistrationDate As DateTimeOffset Implements IAuditable.RegistrationDate
        Get
            Return _registrationDate
        End Get
        Set(value As DateTimeOffset)
            _registrationDate = value
        End Set
    End Property

    Public Overridable Property RegistrationUser As String Implements IAuditable.RegistrationUser
        Get
            Return _registrationUser
        End Get
        Set(value As String)
            _registrationUser = value
        End Set
    End Property
#End Region

#Region "Navigation Properties"
    Public Overridable Property TemplateAdvancedProtocol As TemplateAdvancedProtocol
        Get
            Return _templateAdvancedProtocol
        End Get
        Set(value As TemplateAdvancedProtocol)
            _templateAdvancedProtocol = value
        End Set
    End Property

    Public Overridable Property Container As Container
        Get
            Return _container
        End Get
        Set(value As Container)
            _container = value
        End Set
    End Property

    Public Overridable Property Category As Category
        Get
            Return _category
        End Get
        Set(value As Category)
            _category = value
        End Set
    End Property

    Public Overridable Property DocType As DocumentType
        Get
            Return _docType
        End Get
        Set(value As DocumentType)
            _docType = value
        End Set
    End Property

    Public Overridable Property Type As ProtocolType
        Get
            Return _type
        End Get
        Set(value As ProtocolType)
            _type = value
        End Set
    End Property

    Public Overridable Property Contacts As IList(Of TemplateProtocolContact)
        Get
            Return _contacts
        End Get
        Set(value As IList(Of TemplateProtocolContact))
            _contacts = value
        End Set
    End Property

    Public Overridable Property ContactsManual As IList(Of TemplateProtocolContactManual)
        Get
            Return _contactsManual
        End Get
        Set(value As IList(Of TemplateProtocolContactManual))
            _contactsManual = value
        End Set
    End Property

    Public Overridable Property Roles As IList(Of TemplateProtocolRole)
        Get
            Return _roles
        End Get
        Set(value As IList(Of TemplateProtocolRole))
            _roles = value
        End Set
    End Property

    Public Overridable Property RoleUsers As IList(Of TemplateProtocolRoleUser)
        Get
            Return _roleUsers
        End Get
        Set(value As IList(Of TemplateProtocolRoleUser))
            _roleUsers = value
        End Set
    End Property

    Public Overridable ReadOnly Property HasContacts As Boolean
        Get
            Return Contacts.Count > 0 OrElse ContactsManual.Count > 0
        End Get
    End Property

#End Region

#Region "Constructor"
    Public Sub New()
        Contacts = LoadContacts()
        ContactsManual = LoadContactsManual()
        Roles = LoadRoles()
        RoleUsers = LoadRoleUsers()
    End Sub
#End Region

#Region "Methods"
    Private Function LoadContacts() As List(Of TemplateProtocolContact)
        Return New List(Of TemplateProtocolContact)()
    End Function

    Private Function LoadContactsManual() As List(Of TemplateProtocolContactManual)
        Return New List(Of TemplateProtocolContactManual)()
    End Function

    Private Function LoadRoles() As List(Of TemplateProtocolRole)
        Return New List(Of TemplateProtocolRole)()
    End Function

    Private Function LoadRoleUsers() As List(Of TemplateProtocolRoleUser)
        Return New List(Of TemplateProtocolRoleUser)()
    End Function

    Public Overridable Sub AddAdvancedProtocol(ByRef advancedProtocol As TemplateAdvancedProtocol)
        advancedProtocol.TemplateProtocol = Me
        TemplateAdvancedProtocol = advancedProtocol
    End Sub

    Public Overridable Sub AddRecipient(ByRef contact As Contact, ByVal copiaConoscenza As Boolean)
        Dim tpc As New TemplateProtocolContact()
        tpc.Contact = contact
        tpc.Id.ComunicationType = ProtocolContactCommunicationType.Recipient
        If copiaConoscenza Then
            tpc.Type = "CC"
        End If
        tpc.TemplateProtocol = Me

        Contacts.Add(tpc)
    End Sub

    Public Overridable Sub AddRecipientManual(ByRef contact As Contact, ByVal copiaConoscenza As Boolean)
        Dim tpcm As New TemplateProtocolContactManual()
        tpcm.ComunicationType = ProtocolContactCommunicationType.Recipient
        If copiaConoscenza Then
            tpcm.Type = "CC"
        End If
        tpcm.Contact = contact
        tpcm.TemplateProtocol = Me
        ContactsManual.Add(tpcm)
        tpcm.Id.Incremental = ContactsManual.Count
        tpcm.Contact.FullIncrementalPath = tpcm.Id.Incremental.ToString()
    End Sub

    Public Overridable Sub AddSender(ByRef contact As Contact, ByVal copiaConoscenza As Boolean)
        Dim tpc As New TemplateProtocolContact()
        tpc.Contact = contact
        tpc.Id.ComunicationType = ProtocolContactCommunicationType.Sender
        If copiaConoscenza Then
            tpc.Type = "CC"
        End If
        tpc.TemplateProtocol = Me
        Contacts.Add(tpc)
    End Sub

    Public Overridable Sub AddSenderManual(ByRef contact As Contact, ByVal copiaConoscenza As Boolean)
        Dim tpcm As New TemplateProtocolContactManual()
        tpcm.ComunicationType = ProtocolContactCommunicationType.Sender
        If copiaConoscenza Then
            tpcm.Type = "CC"
        End If
        tpcm.Contact = contact
        tpcm.TemplateProtocol = Me
        ContactsManual.Add(tpcm)
        tpcm.Id.Incremental = ContactsManual.Count
        tpcm.Contact.FullIncrementalPath = tpcm.Id.Incremental.ToString()
    End Sub

    Public Overridable Overloads Sub AddRole(ByRef role As Role, ByVal userName As String, ByVal regDate As DateTime?, ByVal distributionType As String)
        Dim pr As New TemplateProtocolRole()

        pr.Role = role

        If DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled Then
            pr.DistributionType = distributionType
        End If

        pr.TemplateProtocol = Me
        Roles.Add(pr)
    End Sub

    Public Overrides Function ToString() As String
        Return ToString("g", Nothing)
    End Function

    Public Overridable Overloads Function ToString(format As String, formatProvider As IFormatProvider) As String Implements IFormattable.ToString
        Return String.Format("Template {0} del {1} ({2})", Id.ToString(), RegistrationDate.ToString, ProtocolObject)
    End Function
#End Region

    End Class
