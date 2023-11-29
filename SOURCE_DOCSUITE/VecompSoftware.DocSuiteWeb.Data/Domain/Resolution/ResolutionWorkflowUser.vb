Public Class ResolutionWorkflowUser
    Inherits AuditableDomainObject(Of Guid)

#Region " Properties "
    Public Overridable Property Account As String
    Public Overridable Property AuthorizationType As AuthorizationRoleType
#End Region

#Region " Navigation Properties "
    Public Overridable Property ResolutionWorkflow As ResolutionWorkflow
#End Region

#Region " Constructor "
    Public Sub New()
        Id = Guid.NewGuid()
    End Sub
#End Region

End Class
