
<Serializable()>
Public Class ContainerProperty
    Inherits DomainObject(Of Guid)
    Implements IAuditable

#Region " Constructors "
    Public Sub New()
        Id = Guid.NewGuid()
    End Sub

#End Region

#Region " Properties "

    Public Overridable Property Name() As String

    Public Overridable Property ContainerType() As ContainerPropertyType

    Public Overridable Property ValueInt() As Long?

    Public Overridable Property ValueDate() As DateTime?

    Public Overridable Property ValueDouble() As Double?

    Public Overridable Property ValueString() As String

    Public Overridable Property ValueGuid() As Guid?

    Public Overridable Property ValueBoolean() As Boolean?

    Public Overridable Property RegistrationUser() As String Implements IAuditable.RegistrationUser

    Public Overridable Property RegistrationDate() As DateTimeOffset Implements IAuditable.RegistrationDate

    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser

    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate

    Public Overridable Property Container() As Container

#End Region


End Class
