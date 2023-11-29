Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade

Partial Class TbltTenant
    Inherits CommonBasePage

#Region " Fields "

#End Region

#Region " Properties "

#End Region

#Region " Events "
    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not CommonShared.HasGroupAdministratorRight Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If

    End Sub
#End Region

#Region " Methods "

#End Region

End Class


