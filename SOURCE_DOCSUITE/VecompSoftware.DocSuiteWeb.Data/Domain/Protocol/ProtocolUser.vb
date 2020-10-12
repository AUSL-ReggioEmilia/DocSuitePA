Public Class ProtocolUser
    Inherits AuditableDomainObject(Of Guid)

    Private _protocol As Protocol

#Region " Constructor "
    Public Sub New()

    End Sub

#End Region

#Region " Properties "
    Public Overridable Property Year As Short

    Public Overridable Property Number As Integer

    Public Overridable Property Account As String

    Public Overridable Property Note As String

    Public Overridable Property Type As ProtocolUserType

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
#End Region

End Class