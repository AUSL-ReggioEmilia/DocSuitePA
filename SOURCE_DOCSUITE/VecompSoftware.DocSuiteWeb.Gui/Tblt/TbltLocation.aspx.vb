Imports VecompSoftware.DocSuiteWeb.Facade

Public Class TbltLocation
    Inherits CommBasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not CommonShared.HasGroupAdministratorRight AndAlso Not CommonShared.HasGroupTblContainerAdminRight Then
            AjaxAlert("Sono necessari diritti amministrativi per vedere la pagina.")
            Exit Sub
        End If

    End Sub

End Class