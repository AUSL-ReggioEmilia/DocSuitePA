Public Class ProtocolRoleUser
    Inherits AuditableDomainObject(Of ProtocolRoleUserKey)

    Private _role As Role
    Private _protocol As Protocol

    Public Overridable Property Role() As Role
        Get
            Return _role
        End Get
        Set(ByVal value As Role)
            _role = value
            Id.IdRole = value.Id
        End Set
    End Property

    Public Overridable Property Account As String

    Public Overridable Property IsActive As Short

    Public Overridable Property Protocol() As Protocol
        Get
            Return _protocol
        End Get
        Set(ByVal value As Protocol)
            _protocol = value
            Id.Year = value.Year
            Id.Number = value.Number
        End Set
    End Property

    Public Overridable Property UniqueIdProtocol As Guid
    Public Overridable Property Status As Short


#Region "Ctor/init"
    Public Sub New()
        Id = New ProtocolRoleUserKey()
        UniqueId = Guid.NewGuid()
        RegistrationDate = DateTimeOffset.UtcNow
        RegistrationUser = DocSuiteContext.Current.User.FullUserName
    End Sub
#End Region

End Class