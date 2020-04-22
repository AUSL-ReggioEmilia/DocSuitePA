<Serializable()>
Public Class ProtocolDraft
    Inherits AuditableDomainObject(Of Integer)

    Public Overridable Property IsActive As Short

    Public Overridable Property Description As String

    Public Overridable Property Data As String

    Public Overridable Property DraftType As Integer

    Public Overridable Property UniqueIdProtocol As Guid

    Public Overridable Property Protocol As Protocol

    Public Overridable Property Collaboration As Collaboration

#Region "Constructor"
    Public Sub New()
        UniqueId = Guid.NewGuid()
    End Sub
#End Region

End Class
