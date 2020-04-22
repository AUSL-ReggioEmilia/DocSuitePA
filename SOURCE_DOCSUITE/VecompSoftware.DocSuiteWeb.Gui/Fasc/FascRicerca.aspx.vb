Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.ExtensionMethods

Partial Public Class FascRicerca
    Inherits FascBasePage

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Action.Eq("SearchFascicles") Then
            MasterDocSuite.TitleVisible = False
        End If
        If Not IsPostBack Then
            btnSearch.Focus()
        End If
    End Sub

    Protected Sub Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearch.Click
        If ProtocolEnv.SearchMaxRecords <> 0 Then
            uscFascicleFinder.PageSize = ProtocolEnv.SearchMaxRecords
        End If
        SessionSearchController.SaveSessionFinder(uscFascicleFinder.Finder, SessionSearchController.SessionFinderType.FascFinderType)
        ClearSessions(Of FascRisultati)()
        Response.Redirect(String.Concat("~/Fasc/FascRisultati.aspx?Type=Fasc&Action=", Action))
    End Sub

#End Region

#Region "Methods"

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, pageContent, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub


#End Region

End Class
