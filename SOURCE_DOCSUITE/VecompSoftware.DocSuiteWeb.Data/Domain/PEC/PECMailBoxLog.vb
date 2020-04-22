<Serializable()>
Public Class PECMailBoxLog
    Inherits DomainObject(Of Int32)

#Region " Fields "

#End Region

#Region " Constructors "
    Public Sub New()

    End Sub
#End Region

#Region " Properties "

    Public Overridable Property MailBox As PECMailBox

    Public Overridable ReadOnly Property IDMailBox As Short
    Public Overridable Property Description As String

    Public Overridable Property Type As String

    Public Overridable Property [Date] As Date

    Public Overridable Property SystemComputer As String

    Public Overridable Property SystemUser As String

#End Region

End Class
