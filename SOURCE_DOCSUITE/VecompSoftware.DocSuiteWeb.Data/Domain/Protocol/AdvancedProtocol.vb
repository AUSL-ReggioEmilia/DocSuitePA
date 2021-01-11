<Serializable()>
Public Class AdvancedProtocol
    Inherits AuditableDomainObject(Of Guid)

#Region " Fields "

    Private _protocol As Protocol

#End Region

#Region "Properties"

    Public Overridable Property Number As Integer

    Public Overridable Property Year As Short

    Public Overridable Property ServiceCategory As String

    Public Overridable Property Subject As String

    Public Overridable Property ServiceField As String

    Public Overridable Property AccountingDate As Date?

    Public Overridable Property AccountingYear As Short?

    Public Overridable Property AccountingNumber As Integer?

    Public Overridable Property AccountingSectional As String

    Public Overridable Property AccountingSectionalNumber As Integer?

    Public Overridable Property Note As String

    Public Overridable Property InvoiceDate As Date?

    Public Overridable Property InvoiceNumber As String

    Public Overridable Property InvoiceTotal As Decimal?

    Public Overridable Property Package As Integer?

    Public Overridable Property PackageOrigin As Char

    Public Overridable Property PackageIncremental As Integer?

    Public Overridable Property PackageLot As Integer?

    Public Overridable Property Status As ProtocolStatus

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

    Public Overridable Property IsClaim As Boolean?

    Public Overridable Property IdentificationSDI As String

    Public Overridable Property InvoiceYear As Integer?

#End Region

#Region " Constructor "
    Public Sub New()

    End Sub
#End Region

End Class