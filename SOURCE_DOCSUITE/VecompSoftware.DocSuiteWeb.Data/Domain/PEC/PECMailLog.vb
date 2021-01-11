
<Serializable()>
Public Class PECMailLog
    Inherits DomainObject(Of Int32)

#Region " Fields "

#End Region

#Region " Constructor "

    Public Sub New()
        UniqueId = Guid.NewGuid()
    End Sub

#End Region

#Region " Properties "

    Public Overridable Property Mail As PECMail

    Public Overridable Property Description As String

    Public Overridable Property Type As String

    Public Overridable Property [Date] As DateTime

    Public Overridable Property SystemComputer As String

    Public Overridable Property SystemUser As String

    Public Overridable Property DestinationMail As PECMail

    Public Overridable Property Severity As Short?

    Public Overridable Property Hash As String


#End Region

#Region " Methods "

    Public Overridable Function SetLogType(logType As PECMailLogType) As PECMailLog
        Me.Type = logType.ToString()
        Return Me
    End Function

#End Region

End Class
