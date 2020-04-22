<Serializable()> _
Public Class TabWorkflowCompositeKey

#Region "Private Fields"
    Private _workflowType As String
    Private _step As Short
#End Region

#Region "Public Properties"
    Public Overridable Property WorkflowType() As String
        Get
            Return _workflowType
        End Get
        Set(ByVal value As String)
            _workflowType = value
        End Set
    End Property

    Public Overridable Property ResStep() As Short
        Get
            Return _step
        End Get
        Set(ByVal value As Short)
            _step = value
        End Set
    End Property
#End Region


    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        Dim compareTo As TabWorkflowCompositeKey = DirectCast(obj, TabWorkflowCompositeKey)
        Return Me.WorkflowType = compareTo.WorkflowType And Me.ResStep = compareTo.ResStep()
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return Me.ToString().GetHashCode()
    End Function

    Public Overrides Function ToString() As String
        Return WorkflowType.ToString + "/" + ResStep.ToString()
    End Function


End Class