Public Class ProtocolRoleUser
    Inherits AuditableDomainObject(Of Guid)

#Region " Fields "
    Private _protocol As Protocol
#End Region

#Region " Properties "
    Public Overridable Property Year As Short

    Public Overridable Property Number As Integer

    Public Overridable Property GroupName As String

    Public Overridable Property UserName As String

    Public Overridable Property Account As String

    Public Overridable Property IsActive As Short

    Public Overridable Property Status As Short

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

    Public Overridable Property Role As Role

    Public Overridable Property ProtocolRole As ProtocolRole
#End Region

#Region " Constructor "
    Public Sub New()

    End Sub
#End Region

#Region " Methods "
    Public Overrides Function ToString() As String
        Return $"{Year}/{Number:0000000}/{Role.Id}/{GroupName}/{UserName}"
    End Function
#End Region

End Class