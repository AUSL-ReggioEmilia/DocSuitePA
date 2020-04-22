Imports VecompSoftware.Helpers.ExtensionMethods
<Serializable()>
Public Class TableLog
    Inherits AuditableDomainObject(Of Guid)

#Region "Properties"
    Public Overridable Property SystemComputer As String
    Public Overridable Property SystemUser As String
    Public Overridable Property TableName As String
    Public Overridable Property EntityId As Integer
    Public Overridable Property EntityUniqueId As Guid
    Public Overridable Property LogType As LogEvent
    Public Overridable Property LogDescription As String
    Public Overridable Property Hash As String
#End Region

#Region "Ctor/init"
    Public Sub New()
        Id = Guid.NewGuid()
    End Sub
#End Region
End Class


