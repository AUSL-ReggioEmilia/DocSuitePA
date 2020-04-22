Public Class ResolutionActivityHeader

    Public Property Id As Guid

    Public Property Description As String

    Public Property ActivityDate As DateTimeOffset

    Public Property Status As ResolutionActivityStatus

    Public Property Type As ResolutionActivityType

    Public Property ResolutionId As Integer

    Public Property ResolutionUniqueId As Guid

    Public Property ResolutionObject As String

    Public Property ResolutionStatusId As Short

    Public Property ResolutionTypeId As Short

    Public Property ResolutionInclusiveNumber As String

    Public Property ResolutionEffectivenessDate As DateTime?

    Public Property ResolutionOCSupervisoryBoard As Boolean

    Public Property ResolutionOCRegion As Boolean

    Public Property ResolutionOCManagement As Boolean

    Public Property ResolutionOCCorteConti As Boolean

    Public Property ResolutionOCOther As Boolean

    Public Property ResolutionExecutiveRight As Integer

    Public ReadOnly Property HasResolutionExecutiveRight As Boolean
        Get
            Return ResolutionExecutiveRight > 0
        End Get
    End Property



End Class
