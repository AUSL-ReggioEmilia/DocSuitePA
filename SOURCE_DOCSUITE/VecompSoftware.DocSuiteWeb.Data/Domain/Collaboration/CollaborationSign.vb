<Serializable()>
Public Class CollaborationSign
    Inherits AuditableDomainObject(Of Guid)
    Implements ISupportBooleanLogicDelete

#Region " Fields "

#End Region

#Region " Properties "

    Public Overridable Property IdCollaboration As Integer

    Public Overridable Property Incremental As Short

    Public Overridable Property IsActive As Boolean Implements ISupportBooleanLogicDelete.IsActive

    Public Overridable Property IdStatus As String

    Public Overridable Property SignUser As String

    Public Overridable Property SignName As String

    Public Overridable Property SignEMail As String

    Public Overridable Property SignDate As Date?

    Public Overridable Property Collaboration As Collaboration

    Public Overridable Property IsRequired As Boolean?

    Public Overridable Property IsAbsent As Boolean?


#End Region

#Region " Constructor "

    Public Sub New()
    End Sub

#End Region


End Class


