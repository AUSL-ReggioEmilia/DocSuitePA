
<Serializable()> _
Public Class DocumentSeries
    Inherits DomainObject(Of Int32)
    Implements IAuditable

    Public Overridable Property Name As String
    Public Overridable Property Container() As Container
    Public Overridable Property Family() As DocumentSeriesFamily
    Public Overridable Property PublicationEnabled() As Boolean?
    Public Overridable Property SubsectionEnabled() As Boolean?
    Public Overridable Property RoleEnabled() As Boolean?
    Public Overridable Property AllowNoDocument() As Boolean?
    Public Overridable Property AllowAddDocument() As Boolean?
    Public Overridable Property AttributeSorting() As String
    Public Overridable Property AttributeCache() As String
    Public Overridable Property RegistrationUser() As String Implements IAuditable.RegistrationUser
    Public Overridable Property RegistrationDate() As DateTimeOffset Implements IAuditable.RegistrationDate
    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser
    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate
    Public Overridable Property SortOrder() As Integer?
End Class