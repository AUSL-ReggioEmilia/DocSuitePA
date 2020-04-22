Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade

Public Class APIProviderTestPage
    Inherits ProtBasePage

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()
    End Sub

    Private Sub ButtonRenew_Click(sender As Object, e As EventArgs) Handles ButtonRenew.Click
        Try
            FacadeFactory.Instance.APIProviderFacade.Renew()
            AjaxAlert("Allineamento completato")
        Catch ex As Exception
            AjaxAlert(ex.Message)
        End Try
    End Sub
    Private Sub ButtonCreateNew_Click(sender As Object, e As EventArgs) Handles ButtonCreateNew.Click
        Dim placeholder As String = Guid.NewGuid().ToString("N")
        Dim provider As New APIProvider() With {
            .Enabled = False,
            .Code = placeholder,
            .Title = placeholder,
            .Description = placeholder,
            .Address = DocSuiteContext.Current.ProtocolEnv.APIDefaultProvider
        }
        FacadeFactory.Instance.APIProviderFacade.Save(provider)
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(ButtonRenew, ButtonRenew, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ButtonCreateNew, ButtonCreateNew, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

#End Region


End Class