Public Class ResolutionDocumentSeriesItem
    Inherits DomainObject(Of Integer)
    Implements IAuditable

#Region " Properties "
    Public Overridable Property Resolution As Resolution
    Public Overridable Property IdDocumentSeriesItem As Integer?
    Public Overridable Property RegistrationUser As String Implements IAuditable.RegistrationUser
    Public Overridable Property RegistratrionDate As DateTimeOffset Implements IAuditable.RegistrationDate
    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser
    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate
    Public Overridable Property UniqueIdResolution As Guid
    Public Overridable Property UniqueIdDocumentSeriesItem As Guid
#End Region

#Region " Constructor "

    Public Sub New()
        UniqueId = Guid.NewGuid()
    End Sub

#End Region
End Class
