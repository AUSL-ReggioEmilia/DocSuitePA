<Serializable()> _
Public Class ResolutionRoleCompositeKey

#Region "Public Properties"

    Public Property IdResolution As Integer
    Public Property IdRole As Integer
    Public Property IdResolutionRoleType As Integer

#End Region

    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        Dim compareTo As ResolutionRoleCompositeKey = DirectCast(obj, ResolutionRoleCompositeKey)
        Return IdResolution = compareTo.IdResolution And Me.IdRole = compareTo.IdRole() And IdResolutionRoleType = compareTo.IdResolutionRoleType
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return ToString().GetHashCode()
    End Function

    Public Overrides Function ToString() As String
        Return IdRole.ToString + "/" + IdResolution.ToString() + "/" + IdResolutionRoleType.ToString()
    End Function


End Class