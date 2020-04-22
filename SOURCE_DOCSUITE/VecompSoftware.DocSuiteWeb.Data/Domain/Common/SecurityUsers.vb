<Serializable()> _
Public Class SecurityUsers
    Inherits AuditableDomainObject(Of Integer)
    Implements IAuditable

#Region " Properties "

    Public Overridable Property UserDomain As String
    Public Overridable Property Account As String
    Public Overridable Property Description As String
    Public Overridable Property Group As SecurityGroups

    Public Overridable ReadOnly Property DisplayName As String
        Get
            Return String.Format("{0}\{1}", UserDomain, Account)
        End Get
    End Property


#End Region

#Region " Constructor "
    Public Sub New()
        UniqueId = Guid.NewGuid()
    End Sub
#End Region

End Class

