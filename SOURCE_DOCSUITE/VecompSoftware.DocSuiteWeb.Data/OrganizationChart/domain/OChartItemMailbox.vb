<Serializable>
Public Class OChartItemMailbox
    Inherits DomainObject(Of Guid)

#Region " Constructors "

    Public Sub New()
        RegistrationDate = DateTimeOffset.UtcNow
    End Sub
    Public Sub New(source As OChartItemMailbox)
        Me.New()
        Mailbox = source.Mailbox
    End Sub

#End Region

#Region " Properties "
    Public Overridable Property Item As OChartItem
    Public Overridable Property Mailbox As PECMailBox
    Public Overridable Property RegistrationDate As DateTimeOffset
#End Region

End Class
