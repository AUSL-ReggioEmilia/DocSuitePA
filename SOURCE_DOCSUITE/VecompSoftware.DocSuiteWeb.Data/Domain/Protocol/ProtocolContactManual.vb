Imports VecompSoftware.DocSuiteWeb.Data

<Serializable()>
Public Class ProtocolContactManual
    Inherits AuditableDomainObject(Of Guid)

#Region " Fields "
    Private _protocol As Protocol
#End Region

#Region " Properties "
    Public Overridable Property Year As Short

    Public Overridable Property Number As Integer

    Public Overridable Property Incremental As Integer

    Public Overridable Property ComunicationType As String

    Public Overridable Property IdAD As String

    Public Overridable Property Type As String

    Public Overridable Property SDIIdentification As String

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

End Class


