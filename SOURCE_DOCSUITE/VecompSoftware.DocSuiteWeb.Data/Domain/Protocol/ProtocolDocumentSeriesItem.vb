Imports VecompSoftware.DocSuiteWeb.Data

Public Class ProtocolDocumentSeriesItem
    Inherits DomainObject(Of Guid)
    Implements IAuditable

#Region " Fields "
    Private _protocol As Protocol
#End Region

#Region " Constructor "
    Public Sub New()

    End Sub
#End Region

#Region " Properties "
    Public Overridable Property Year As Short

    Public Overridable Property Number As Integer

    Public Overridable Property UniqueIdDocumentSeriesItem As Guid

    Public Overridable Property RegistrationUser As String Implements IAuditable.RegistrationUser

    Public Overridable Property RegistratrionDate As DateTimeOffset Implements IAuditable.RegistrationDate

    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser

    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate

    Public Overridable Property Protocol As Protocol
        Get
            Return _protocol
        End Get
        Set(ByVal value As Protocol)
            _protocol = value
            Year = value.Year
            Number = value.Number
        End Set
    End Property

    Public Overridable Property DocumentSeriesItem As DocumentSeriesItem
#End Region

End Class
