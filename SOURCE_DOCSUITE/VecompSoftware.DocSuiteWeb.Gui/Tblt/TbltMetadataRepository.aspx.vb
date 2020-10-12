Imports VecompSoftware.DocSuiteWeb.Facade

Public Class TbltMetadataRepository
    Inherits CommonBasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not CommonShared.HasGroupAdministratorRight Then
            AjaxAlert("Sono necessari diritti amministrativi per vedere la pagina.")
            Exit Sub
        End If

    End Sub

End Class