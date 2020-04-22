Public Class ProtocolDocumentSeriesItem
    Inherits DomainObject(Of Guid)
    Implements IAuditable

    Public Overridable Property Protocol As Protocol
    Public Overridable Property DocumentSeriesItem As DocumentSeriesItem

    Public Overridable Property UniqueIdProtocol As Guid
    Public Overridable Property UniqueIdDocumentSeriesItem As Guid

    Public Overridable Property RegistrationUser As String Implements IAuditable.RegistrationUser
    Public Overridable Property RegistratrionDate As DateTimeOffset Implements IAuditable.RegistrationDate
    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser
    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate

End Class
