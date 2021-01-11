Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Commons

Partial Class TbltPrivacyLevel
    Inherits CommonBasePage

#Region "Fields"
#End Region

#Region "Properties"
#End Region

#Region "Events"
    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        If Not CommonShared.HasGroupAdministratorRight Then
            AjaxAlert("Sono necessari diritti amministrativi per vedere la pagina.")
            Exit Sub
        End If

        InitializeAjax()
        If Not IsPostBack Then
            Title = String.Concat("Livelli di ", PRIVACY_LABEL)
        End If
    End Sub

    Protected Sub TbltPrivacyLevelAjaxRequest(sender As Object, e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch
        End Try

        If ajaxModel IsNot Nothing AndAlso String.Equals(ajaxModel.ActionName, "Refresh") Then
            Dim privacyLevelFacade As PrivacyLevelFacade = New PrivacyLevelFacade()
            DocSuiteContext.Current.RefreshPrivacyLevel(privacyLevelFacade.GetCurrentPrivacyLevels())
        End If
    End Sub
#End Region

#Region "Methods"
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf TbltPrivacyLevelAjaxRequest
    End Sub
#End Region

End Class

