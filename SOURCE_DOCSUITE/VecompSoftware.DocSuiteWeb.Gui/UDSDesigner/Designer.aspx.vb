Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade

Public Class Designer
    Inherits CommonBasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not CommonShared.HasGroupAdministratorRight Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If
        If Not (IsPostBack) Then
            workflowManager.Value = ProtocolEnv.WorkflowManagerEnabled.ToString()
            conservationManager.Value = ProtocolEnv.ConservationEnabled.ToString()
            odataUrl.Value = DocSuiteContext.Current.CurrentTenant.ODATAUrl
            restUrl.Value = DocSuiteContext.Current.CurrentTenant.ODATAUrl.Replace("ODATA", "API/ENTITY") 'leave please!
        End If
    End Sub

End Class