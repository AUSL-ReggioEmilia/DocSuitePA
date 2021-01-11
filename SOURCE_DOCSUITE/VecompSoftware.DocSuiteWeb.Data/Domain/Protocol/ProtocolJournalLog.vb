<Serializable()> _
Public Class ProtocolJournalLog
    Inherits DomainObject(Of Integer)
    Implements ISupportTenant

#Region " Fields "

#End Region

#Region " Properties "
    Public Overridable Property LogDate As Date?

    Public Overridable Property ProtocolJournalDate As Date?

    Public Overridable Property SystemComputer As String

    Public Overridable Property SystemUser As String

    Public Overridable Property StartDate As Date?

    Public Overridable Property EndDate As Date?

    Public Overridable Property ProtocolTotal As Integer?

    Public Overridable Property ProtocolRegister As Integer?

    Public Overridable Property ProtocolError As Integer?

    Public Overridable Property ProtocolCancelled As Integer?

    Public Overridable Property ProtocolActive As Integer?

    Public Overridable Property ProtocolOthers As Integer?

    Public Overridable Property IdDocument As Integer?

    Public Overridable Property Location As Location

    Public Overridable Property LogDescription As String

    Public Overridable Property IdTenantAOO As Guid? Implements ISupportTenant.IdTenantAOO
#End Region

#Region " Constructor "
    Public Sub New()
    End Sub
#End Region


End Class
