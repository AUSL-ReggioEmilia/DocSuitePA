Imports VecompSoftware.DocSuiteWeb.Data

<Serializable()>
Public Class CollaborationDraft
    Inherits AuditableDomainObject(Of Guid)

#Region " Fields "
#End Region

#Region " Properties "
    Public Overridable Property Incremental As Integer

    Public Overridable Property IsActive As Short

    Public Overridable Property Description As String

    Public Overridable Property Data As String

    Public Overridable Property DraftType As Integer

    Public Overridable Property Collaboration As Collaboration
    Public Overridable Property IdDocumentUnit As Guid?
#End Region

#Region "Constructor"
    Public Sub New()

    End Sub
#End Region

End Class
