<Serializable()> _
Public Class UserLog
    Inherits AuditableDomainObject(Of String)

#Region "private data"

#End Region

#Region "Properties"
    Public Overridable Property SystemServer As String
    Public Overridable Property SystemComputer As String
    Public Overridable Property LastOperationDate As DateTimeOffset?
    Public Overridable Property AccessNumber As Integer
    Public Overridable Property PrevOperationDate As DateTimeOffset?
    Public Overridable Property SessionId As String
    Public Overridable Property AdvancedScanner As Integer
    Public Overridable Property AdvancedViewer As Integer
    Public Overridable Property CurrentTenantId As Guid
    Public Overridable Property UserMail As String
    Public Overridable Property MobilePhone As String
    Public Overridable Property DefaultAdaptiveSearchControls As String
    Public Overridable Property AdaptiveSearchStatistics As String
    Public Overridable Property AdaptiveSearchEvaluated As String
    Public Overridable Property PrivacyLevel As Integer
    Public Overridable Property UserProfile As String
    Public Overridable Property UserPrincipalName As String

#End Region

#Region "Ctor/init"
    Public Sub New()
        UniqueId = Guid.NewGuid()
    End Sub
#End Region
End Class


