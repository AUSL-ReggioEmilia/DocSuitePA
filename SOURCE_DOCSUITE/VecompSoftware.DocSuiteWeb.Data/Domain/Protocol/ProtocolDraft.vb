Imports VecompSoftware.DocSuiteWeb.Data

<Serializable()>
Public Class ProtocolDraft
    Inherits AuditableDomainObject(Of Guid)

#Region " Fields "
    Private _protocol As Protocol
#End Region

#Region " Properties "
    Public Overridable Property Incremental As Integer

    Public Overridable Property Year As Short?

    Public Overridable Property Number As Integer?

    Public Overridable Property IsActive As Short

    Public Overridable Property Description As String

    Public Overridable Property Data As String

    Public Overridable Property DraftType As Integer

    Public Overridable Property Protocol As Protocol
        Get
            Return _protocol
        End Get
        Set(ByVal value As Protocol)
            _protocol = value
            If value IsNot Nothing Then
                Year = value.Year
                Number = value.Number
            End If
        End Set
    End Property

    Public Overridable Property Collaboration As Collaboration
#End Region

#Region "Constructor"
    Public Sub New()

    End Sub
#End Region

End Class
