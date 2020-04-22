
Partial Public Class Protocol
    <Serializable()> _
    Public Class AdvancedProtocol
        Inherits AuditableDomainObject(Of YearNumberCompositeKey)

#Region "private data"

        Private _serviceCategory As String
        Private _subject As String
        Private _serviceField As String
        Private _note As String

        'INVOICE
        Private _accountingDate As Date?
        Private _accountingNumber As Integer?
        Private _accountingYear As Nullable(Of Short)
        Private _accountingSectional As String
        Private _accountingSectionalNumber As Integer?
        Private _invoiceDate As Date?
        Private _invoiceNumber As String
        Private _invoiceTotal As Nullable(Of Decimal)
        Private _status As ProtocolStatus

        'PACKAGE
        Private _package As Integer?
        Private _packageOrigin As Char
        Private _packageLot As Integer?
        Private _packageIncremental As Integer?

        'ENPACL
        Private _isClaim As Nullable(Of Boolean)

        'PROTOCOL
        Private _protocol As Protocol

        Private _identificationSDI As String
#End Region

#Region "Properties"

        Public Overridable Property Number() As Integer
            Get
                Return Id.Number.Value
            End Get
            Set(ByVal value As Integer)
                Id.Number = value
            End Set
        End Property

        Public Overridable Property Year() As Short
            Get
                Return Id.Year.Value
            End Get
            Set(ByVal value As Short)
                Id.Year = value
            End Set
        End Property

        Public Overridable Property ServiceCategory() As String
            Get
                Return _serviceCategory
            End Get
            Set(ByVal value As String)
                _serviceCategory = value
            End Set
        End Property

        Public Overridable Property Subject() As String
            Get
                Return _subject
            End Get
            Set(ByVal value As String)
                _subject = value
            End Set
        End Property

        Public Overridable Property ServiceField() As String
            Get
                Return _serviceField
            End Get
            Set(ByVal value As String)
                _serviceField = value
            End Set
        End Property

        Public Overridable Property AccountingDate() As Date?
            Get
                Return _accountingDate
            End Get
            Set(ByVal value As Date?)
                _accountingDate = value
            End Set
        End Property

        Public Overridable Property AccountingYear() As Nullable(Of Short)
            Get
                Return _accountingYear
            End Get
            Set(ByVal value As Nullable(Of Short))
                _accountingYear = value
            End Set
        End Property

        Public Overridable Property AccountingNumber() As Integer?
            Get
                Return _accountingNumber
            End Get
            Set(ByVal value As Integer?)
                _accountingNumber = value
            End Set
        End Property

        Public Overridable Property AccountingSectional() As String
            Get
                Return _accountingSectional
            End Get
            Set(ByVal value As String)
                _accountingSectional = value
            End Set
        End Property

        Public Overridable Property AccountingSectionalNumber As Integer?
            Get
                Return _accountingSectionalNumber
            End Get
            Set(value As Integer?)
                _accountingSectionalNumber = value
            End Set
        End Property

        Public Overridable Property Note() As String
            Get
                Return _note
            End Get
            Set(ByVal value As String)
                _note = value
            End Set
        End Property

        Public Overridable Property InvoiceDate() As Date?
            Get
                Return _invoiceDate
            End Get
            Set(ByVal value As Date?)
                _invoiceDate = value
            End Set
        End Property

        Public Overridable Property InvoiceNumber() As String
            Get
                Return _invoiceNumber
            End Get
            Set(ByVal value As String)
                _invoiceNumber = value
            End Set
        End Property

        Public Overridable Property InvoiceTotal() As Nullable(Of Decimal)
            Get
                Return _invoiceTotal
            End Get
            Set(ByVal value As Nullable(Of Decimal))
                _invoiceTotal = value
            End Set
        End Property

        Public Overridable Property Package() As Integer?
            Get
                Return _package
            End Get
            Set(ByVal value As Integer?)
                _package = value
            End Set
        End Property

        Public Overridable Property PackageOrigin() As Char
            Get
                Return _packageOrigin
            End Get
            Set(ByVal value As Char)
                _packageOrigin = value
            End Set
        End Property

        Public Overridable Property PackageIncremental() As Integer?
            Get
                Return _packageIncremental
            End Get
            Set(ByVal value As Integer?)
                _packageIncremental = value
            End Set
        End Property

        Public Overridable Property PackageLot() As Integer?
            Get
                Return _packageLot
            End Get
            Set(ByVal value As Integer?)
                _packageLot = value
            End Set
        End Property

        Public Overridable Property Status() As ProtocolStatus
            Get
                Return _status
            End Get
            Set(ByVal value As ProtocolStatus)
                _status = value
            End Set
        End Property

        Public Overridable Property Protocol() As Protocol
            Get
                Return _protocol
            End Get
            Set(ByVal value As Protocol)
                _protocol = value
            End Set
        End Property

        Public Overridable Property IsClaim() As Nullable(Of Boolean)
            Get
                Return _isClaim
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                _isClaim = value
            End Set
        End Property

        Public Overridable Property IdentificationSDI As String
            Get
                Return _identificationSDI
            End Get
            Set(value As String)
                _identificationSDI = value
            End Set
        End Property

        Public Overridable Property InvoiceYear() As Integer?

        Public Overridable Property UniqueIdProtocol As Guid

#End Region

#Region "Ctor/init"
        Public Sub New()
            Id = New YearNumberCompositeKey()
            UniqueId = Guid.NewGuid()
        End Sub
#End Region

    End Class

End Class
