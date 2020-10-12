Imports VecompSoftware.DocSuiteWeb.Facade

Public Class TbltWorkflowStepGes
    Inherits CommonBasePage

#Region " Properties "
    Public ReadOnly Property PageAction As String
        Get
            Return GetKeyValue(Of String, TbltWorkflowStepGes)("Action")
        End Get
    End Property
#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False
        If Not CommonShared.HasGroupAdministratorRight Then
            AjaxAlert("Sono necessari diritti amministrativi per vedere la pagina.")
            AjaxManager.ResponseScripts.Add("CloseWindow();")
            Exit Sub
        End If

    End Sub
#End Region

End Class