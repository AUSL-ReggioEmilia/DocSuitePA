<Serializable()> _
Public Class RoleUser
    Inherits DomainObject(Of Integer)
    Implements IAuditable

#Region " Properties "
    Public Overridable Property Role() As Role
    ''' <summary> Tipologia del ruolo (Dirigenti, Vice e Segreterie) </summary>
    ''' <returns> Vedere <see cref="RoleUserType"/> </returns>
    Public Overridable Property Type As String
    Public Overridable Property Description As String
    Public Overridable Property Account As String
    Public Overridable Property Enabled As Boolean?
    Public Overridable Property Email As String
    Public Overridable Property RegistrationUser As String Implements IAuditable.RegistrationUser
    Public Overridable Property RegistrationDate As DateTimeOffset Implements IAuditable.RegistrationDate
    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser
    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate
    Public Overridable Property IsMainRole As Boolean?
    Public Overridable Property DSWEnvironment As DSWEnvironment
    Public Overridable Property IdUDSRepository As Guid?
#End Region

#Region " Constructor "

    Public Sub New()
        UniqueId = Guid.NewGuid()
    End Sub

#End Region

End Class
