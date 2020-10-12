Public Class WorkflowInstances
    Inherits CommonBasePage

#Region " Fields "

#End Region

#Region " Properties "
    Protected ReadOnly Property WorkflowRepositoryName As String
        Get
            Return GetKeyValueOrDefault("WorkflowRepositoryName", String.Empty)
        End Get
    End Property
#End Region

#Region "Events"
    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        If Not IsPostBack Then
            dtpWorkflowRepositoryActiveFrom.SelectedDate = DateTime.Today.AddDays(-ProtocolEnv.DesktopDayDiff).Date
            dtpWorkflowRepositoryActiveTo.SelectedDate = DateTime.Today.Date.AddDays(1).AddMilliseconds(-1)
        End If
    End Sub
#End Region

#Region "Methods"
#End Region

End Class