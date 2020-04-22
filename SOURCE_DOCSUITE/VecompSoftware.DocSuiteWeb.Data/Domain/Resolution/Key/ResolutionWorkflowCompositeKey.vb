<Serializable()> _
Public Class ResolutionWorkflowCompositeKey

#Region "Private Fields"
    Private _idResolution As Integer
    Private _incremental As Short
#End Region

#Region "Public Properties"
    Public Overridable Property IdResolution() As Integer
        Get
            Return _idResolution
        End Get
        Set(ByVal value As Integer)
            _idResolution = value
        End Set
    End Property

    Public Overridable Property Incremental() As Short
        Get
            Return _incremental
        End Get
        Set(ByVal value As Short)
            _incremental = value
        End Set
    End Property
#End Region

    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        Dim compareTo As ResolutionWorkflowCompositeKey = DirectCast(obj, ResolutionWorkflowCompositeKey)
        Return Me.IdResolution = compareTo.IdResolution And Me.Incremental = compareTo.Incremental

    End Function

    Public Overrides Function GetHashCode() As Integer
        Return Me.ToString().GetHashCode()
    End Function

    Public Overrides Function ToString() As String
        Return IdResolution.ToString() + "/" + Incremental.ToString()
    End Function


End Class