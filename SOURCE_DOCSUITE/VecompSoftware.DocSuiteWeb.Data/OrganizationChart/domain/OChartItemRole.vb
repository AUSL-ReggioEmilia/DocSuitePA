<Serializable>
Public Class OChartItemRole
    Inherits DomainObject(Of Guid)

#Region " Constructors "

    Public Sub New()
        RegistrationDate = DateTimeOffset.UtcNow
    End Sub
    Public Sub New(source As OChartItemRole)
        Me.New()
        Role = source.Role
    End Sub

#End Region

#Region " Properties "
    Public Overridable Property Item As OChartItem
    Public Overridable Property Role As Role
    Public Overridable Property RegistrationDate As DateTimeOffset
#End Region

End Class
