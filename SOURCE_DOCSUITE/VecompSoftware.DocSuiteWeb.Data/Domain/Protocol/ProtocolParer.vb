
Imports VecompSoftware.DocSuiteWeb.Data

<Serializable()>
Public Class ProtocolParer
    Inherits AuditableDomainObject(Of Guid)


#Region " Fields "
    Private _protocol As Protocol
#End Region

#Region " Properties "

    Public Overridable Property Year As Short

    Public Overridable Property Number As Integer

    Public Overridable Property ArchivedDate As DateTime?

    Public Overridable Property ParerUri As String

    Public Overridable Property IsForArchive As Boolean

    Public Overridable Property HasError As Boolean

    Public Overridable Property LastError As String

    Public Overridable Property LastSendDate As DateTime?

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
