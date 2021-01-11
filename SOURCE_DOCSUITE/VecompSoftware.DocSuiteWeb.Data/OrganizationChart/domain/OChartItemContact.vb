<Serializable>
Public Class OChartItemContact
    Inherits DomainObject(Of Guid)

#Region " Constructors "

    Public Sub New()
        RegistrationDate = DateTimeOffset.UtcNow
    End Sub
    Public Sub New(source As OChartItemContact)
        Me.New()
        Contact = source.Contact
    End Sub

#End Region

#Region " Properties "
    Public Overridable Property Item As OChartItem
    Public Overridable Property Contact As Contact
    Public Overridable Property RegistrationDate As DateTimeOffset
#End Region

End Class
