<Serializable()> _
Public Class ProtocolContactIssue
    Inherits DomainObject(Of Guid)
    Implements IAuditable

#Region " Fields "
    Private _protocol As Protocol
#End Region

#Region " Properties "

    Public Overridable Property Year As Short

    Public Overridable Property Number As Integer

    Public Overridable Property Incremental As Integer

    Public Overridable Property Contact As Contact

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

    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate

    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser

    Public Overridable Property RegistrationUser As String Implements IAuditable.RegistrationUser

    Public Overridable Property RegistrationDate As DateTimeOffset Implements IAuditable.RegistrationDate

#End Region

#Region "Ctor/init"
    Public Sub New()

    End Sub
#End Region

End Class



