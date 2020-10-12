<Serializable()> _
Public Class ProtocolContact
    Inherits AuditableDomainObject(Of Guid)

#Region " Fields "

    Private _protocol As Protocol

#End Region

#Region " Properties "

    Public Overridable Property Year As Short

    Public Overridable Property Number As Integer

    Public Overridable Property ComunicationType As String

    Public Overridable Property Type As String

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
#End Region

#Region " Constructor "

    Public Sub New()

    End Sub

#End Region

#Region " Methods "
    Public Overrides Function ToString() As String
        Return $"{Year}/{Number:0000000}-{Contact.Id}-{ComunicationType}"
    End Function
#End Region

End Class

