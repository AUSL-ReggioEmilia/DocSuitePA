Imports VecompSoftware.DocSuiteWeb.Data

<Serializable()>
Public Class ProtocolLink
    Inherits AuditableDomainObject(Of Guid)

#Region " Fields "
    Private _protocol As Protocol
    Private _protocolSon As Protocol
#End Region

#Region "Properties"
    Public Overridable Property Year As Short

    Public Overridable Property Number As Integer

    Public Overridable Property YearSon As Short

    Public Overridable Property NumberSon As Integer

    Public Overridable Property LinkType As Integer

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

    Public Overridable Property ProtocolLinked As Protocol
        Get
            Return _protocolSon
        End Get
        Set(ByVal value As Protocol)
            _protocolSon = value
            YearSon = value.Year
            NumberSon = value.Number
        End Set
    End Property

#End Region

#Region " Constructor "
    Public Sub New()

    End Sub
#End Region

#Region " Methods "
    Public Overrides Function ToString() As String
        Return $"{Year}/{Number:0000000}-{YearSon}/{NumberSon:0000000}"
    End Function
#End Region

End Class





