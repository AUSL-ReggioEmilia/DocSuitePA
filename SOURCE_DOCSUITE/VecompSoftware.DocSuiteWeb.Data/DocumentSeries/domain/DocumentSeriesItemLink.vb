Imports VecompSoftware.Helpers.ExtensionMethods

<Serializable()> _
Public Class DocumentSeriesItemLink
    Inherits DomainObject(Of Int32)
    Implements IAuditable

#Region " Properties "
    Public Overridable Property DocumentSeriesItem() As DocumentSeriesItem
    Public Overridable Property Resolution As Resolution
    Public Overridable Property LinkType As DocumentSeriesItemLinkType
    Public Overridable Property RegistrationUser As String Implements IAuditable.RegistrationUser
    Public Overridable Property RegistrationDate As DateTimeOffset Implements IAuditable.RegistrationDate
    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser
    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate
    Public Overridable Property UniqueIdDocumentSeriesItem As Guid
    Public Overridable Property UniqueIdResolution As Guid
#End Region

#Region " Constructor "

    Public Sub New()
        UniqueId = Guid.NewGuid()
    End Sub

#End Region
End Class