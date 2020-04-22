<Serializable()>
Public Class ResolutionActivity
    Inherits AuditableDomainObject(Of Guid)

#Region " Properties "

    Public Overridable Property Description As String
    Public Overridable Property Status As ResolutionActivityStatus
    Public Overridable Property ActivityDate As DateTimeOffset
    Public Overridable Property ActivityType As ResolutionActivityType
    Public Overridable Property WorkflowType As String
    Public Overridable Property Resolution As Resolution
    Public Overridable Property JsonDocuments As String
    Public Overridable Property UniqueIdResolution As Guid

#End Region

End Class
