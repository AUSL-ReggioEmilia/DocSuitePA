<Serializable()>
Public Class TenantContact
    Inherits DomainObject(Of Guid)
#Region " Fields "

#End Region

#Region " Properties "
    Public Overridable Property RegistrationDate() As DateTimeOffset
    Public Overridable Property IdTenant() As Guid
    Public Overridable Property EntityId() As Int32
#End Region

#Region " Constructors "
    Public Sub New()
        UniqueId = Guid.NewGuid()
    End Sub
#End Region
End Class
