<Serializable()> _
Public MustInherit Class AuditableDomainObject(Of IdT)
    Inherits DomainObject(Of IdT)
    Implements IAuditable

    Public Overridable Property RegistrationUser() As String Implements IAuditable.RegistrationUser

    Public Overridable Property RegistrationDate() As DateTimeOffset Implements IAuditable.RegistrationDate

    Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser

    Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate

End Class
