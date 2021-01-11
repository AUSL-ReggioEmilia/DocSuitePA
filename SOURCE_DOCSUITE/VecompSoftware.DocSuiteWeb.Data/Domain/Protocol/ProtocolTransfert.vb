<Serializable()> _
Public Class ProtocolTransfert
    Inherits DomainObject(Of Guid)
    Implements IAuditable

#Region " Fields "
    Private _protocol As Protocol
#End Region

#Region " Properties "

    Public Overridable Property Year As Short

    Public Overridable Property Number As Integer

    Public Overridable Property Container As Container

    Public Overridable Property Category As Category

    Public Overridable Property ProtocolObject As String

    Public Overridable Property Type As ProtocolType

    Public Overridable Property Note As String

    Public Overridable Property Request As String

    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate

    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser

    Public Overridable Property RegistrationUser() As String Implements IAuditable.RegistrationUser

    Public Overridable Property RegistrationDate() As DateTimeOffset Implements IAuditable.RegistrationDate

    Public Overridable Property Protocol As Protocol
        Get
            Return _protocol
        End Get
        Set(value As Protocol)
            _protocol = value
            Year = value.Year
            Number = value.Number
        End Set
    End Property

#End Region


#Region " Constructor "
    Public Sub New()

    End Sub
#End Region

End Class
