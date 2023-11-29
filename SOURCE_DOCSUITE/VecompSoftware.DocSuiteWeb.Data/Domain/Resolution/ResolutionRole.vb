<Serializable()> _
Public Class ResolutionRole
    Inherits DomainObject(Of ResolutionRoleCompositeKey)
    Implements IAuditable

#Region "Properties"
    Public Overridable Property RegistrationUser As String Implements IAuditable.RegistrationUser
    Public Overridable Property RegistrationDate As DateTimeOffset Implements IAuditable.RegistrationDate
    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser
    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate

    Public Overridable Property Resolution As Resolution
    Public Overridable Property Role As Role
    Public Overridable Property ResolutionRoleType As ResolutionRoleType
    Public Overridable Property UniqueIdResolution As Guid
#End Region

#Region "Ctor/init"
    Public Sub New()
        Id = New ResolutionRoleCompositeKey
        UniqueId = Guid.NewGuid()
    End Sub
#End Region

End Class

