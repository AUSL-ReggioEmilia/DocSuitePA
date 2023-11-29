Imports System.Web
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade

Public Class TbltWorkflowEvaluationPropertyGes
    Inherits CommonBasePage
    Public ReadOnly Property WorkflowEnv As String
        Get
            Return HttpContext.Current.Request.QueryString("WorkflowEnv")
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False
        If Not CommonShared.HasGroupAdministratorRight Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If


    End Sub

End Class