
Imports VecompSoftware.DocSuiteWeb.Facade

Public Class UtltImpersonator
    Inherits SuperAdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Private Sub cmdImpersonate_Click(sender As Object, e As System.EventArgs) Handles cmdImpersonate.Click
        If txtPwd.Text.GetHashCode() = "764522407" Then
            'inizializza utente
            CommonUtil.GetInstance.InitializeUser()
            AjaxAlert("OK!")
        Else
            ' Password errata
            AjaxAlert("Password errata!")
        End If
    End Sub
End Class