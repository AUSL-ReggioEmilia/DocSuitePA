Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Entity.Workflows

Partial Class TbltWorkflowRepository
    Inherits CommonBasePage

#Region "Fields"
#End Region

#Region "Properties"

#End Region

#Region "Events"
    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub
#End Region

#Region "Methods"
    Private Sub InitializeAjax()

    End Sub

    Private Sub Initialize()
        rgvWorkflowRoleMappings.DataSource = New List(Of WorkflowRoleMapping)
        rgvXamlWorkflowRoleMappings.DataSource = New List(Of Object)
        rgvWorkflowStartUp.DataSource = New List(Of Object)
        rgvStepInputProperties.DataSource = New List(Of Object)
        rgvStepEvaluationProperties.DataSource = New List(Of Object)
        rgvStepOutputProperties.DataSource = New List(Of Object)
        mappingDataSource.DataSource.WebServiceDataSourceSettings.BaseUrl = DocSuiteContext.Current.CurrentTenant.ODATAUrl
    End Sub
#End Region

End Class

