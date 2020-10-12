Imports VecompSoftware.DocSuiteWeb.Facade

Public Class TbltPECMailBox
    Inherits CommonBasePage

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not CommonShared.HasGroupAdministratorRight Then
            AjaxAlert("Sono necessari diritti amministrativi per vedere la pagina.")
            Exit Sub
        End If

    End Sub

End Class