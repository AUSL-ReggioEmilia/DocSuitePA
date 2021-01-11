
<Serializable>
Public Class OChartItemWorkflow
    Inherits DomainObject(Of Guid)
    Implements IAuditable

#Region " Constructors "

    Public Sub New()
    End Sub

    Public Sub New(source As OChartItemWorkflow)
        Me.RoleUsers = New HashSet(Of RoleUser)
        IdWorkflowRepository = source.IdWorkflowRepository
    End Sub

#End Region

#Region " Properties "

    Public Overridable Property OChartItem As OChartItem
    Public Overridable Property IdWorkflowRepository As Guid
    Public Overridable Property RegistrationUser As String Implements IAuditable.RegistrationUser
    Public Overridable Property RegistrationDate As DateTimeOffset Implements IAuditable.RegistrationDate
    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser
    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate

#End Region

#Region " Navigation Properties "

    Public Overridable Property RoleUsers As ICollection(Of RoleUser)

#End Region

End Class
