<Serializable>
Public Class OChartItemContainer
    Inherits DomainObject(Of Guid)

#Region " Constructors "

    Public Sub New()
        RegistrationDate = DateTimeOffset.UtcNow
    End Sub
    Public Sub New(source As OChartItemContainer)
        Me.New()
        Container = source.Container
    End Sub

#End Region

#Region " Properties "

    Public Overridable Property Item As OChartItem
    Public Overridable Property Container As Container

    Public Overridable Property Master As Boolean?
    Public Overridable Property Rejection As Boolean?

    Public Overridable Property RegistrationDate As DateTimeOffset

    Public Overridable ReadOnly Property IsMaster As Boolean
        Get
            Return Master.GetValueOrDefault(False)
        End Get
    End Property
    Public Overridable ReadOnly Property IsRejection As Boolean
        Get
            Return Rejection.GetValueOrDefault(False)
        End Get
    End Property

#End Region

End Class
