Public Class Designer
    Inherits CommonBasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not (IsPostBack) Then
            dematerialisationEnabled.Value = ProtocolEnv.DematerialisationEnabled.ToString()
            workflowManager.Value = ProtocolEnv.WorkflowManagerEnabled.ToString()
        End If
    End Sub

End Class