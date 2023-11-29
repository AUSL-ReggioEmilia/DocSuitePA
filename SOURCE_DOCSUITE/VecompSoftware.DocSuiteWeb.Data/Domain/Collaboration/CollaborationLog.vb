<Serializable()> _
Public Class CollaborationLog
    Inherits DomainObject(Of Integer)
    Implements ILog

#Region " Fields "

#End Region

#Region " Properties "

    Public Overrides Property Id As Integer Implements ILog.Id

    Public Overridable Property IdCollaboration As Integer

    Public Overridable Property CollaborationIncremental As Integer?

    Public Overridable Property Incremental As Short?

    Public Overridable Property IdChain As Integer?

    Public Overridable Property LogDate As Date Implements ILog.LogDate

    Public Overridable Property SystemComputer As String Implements ILog.SystemComputer

    Public Overridable Property SystemUser As String Implements ILog.SystemUser

    Public Overridable Property Program As String Implements ILog.Program

    Public Overridable Property LogType As String Implements ILog.LogType

    Public Overridable Property LogDescription As String Implements ILog.LogDescription

    Public Overridable Property Collaboration As Collaboration

    Public Overridable Property Number As Integer Implements ILog.Number

    Public Overridable Property Year As Short Implements ILog.Year

    Public Overridable Property Severity As Short? Implements ILog.Severity

#End Region

End Class
