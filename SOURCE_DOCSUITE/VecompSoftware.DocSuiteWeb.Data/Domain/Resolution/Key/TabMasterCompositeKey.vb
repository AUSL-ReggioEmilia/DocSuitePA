<Serializable()> _
Public Class TabMasterCompositeKey

#Region "Private Fields"
    Private _configuration As String
    Private _resolutionType As Short
#End Region

#Region "Public Properties"
    Public Overridable Property Configuration() As String
        Get
            Return _configuration
        End Get
        Set(ByVal value As String)
            _configuration = value
        End Set
    End Property
    Public Overridable Property ResolutionType() As Short
        Get
            Return _resolutionType
        End Get
        Set(ByVal value As Short)
            _resolutionType = value
        End Set
    End Property
#End Region

    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        Dim compareTo As TabMasterCompositeKey = DirectCast(obj, TabMasterCompositeKey)
        Return Me.Configuration = compareTo.Configuration And Me.ResolutionType = compareTo.ResolutionType()
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return Me.ToString().GetHashCode()
    End Function

    Public Overrides Function ToString() As String
        Return Configuration.ToString + "/" + ResolutionType.ToString()
    End Function


End Class