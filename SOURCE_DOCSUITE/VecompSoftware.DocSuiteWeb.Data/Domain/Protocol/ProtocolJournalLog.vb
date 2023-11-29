<Serializable()> _
Public Class ProtocolJournalLog
    Inherits DomainObject(Of Integer)
    Implements IAuditable
    Implements IFormattable
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

    Public Overridable Property RegistrationUser As String Implements IAuditable.RegistrationUser

    Public Overridable Property RegistrationDate() As DateTimeOffset Implements IAuditable.RegistrationDate

    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser

    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate
#End Region

#Region " Constructor "
    Public Sub New()
        UniqueId = Guid.NewGuid()
    End Sub
#End Region

#Region " Methods "
    Public Overrides Function ToString() As String
        Return ToString("g", Nothing)
    End Function

    Public Overridable Overloads Function ToString(format As String, formatProvider As IFormatProvider) As String Implements IFormattable.ToString
        Return String.Format("ProtocolJournal  {0} del {1} ({2})", Id.ToString(), RegistrationDate.ToString)
    End Function


#End Region
End Class
