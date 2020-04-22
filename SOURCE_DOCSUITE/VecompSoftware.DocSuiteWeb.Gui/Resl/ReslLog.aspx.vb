Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class ReslLog
    Inherits ReslBasePage

#Region " Fields "

    Private reslLogFinder As NHibernateResolutionLogFinder

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()
        Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RL, "ATTI.LOG: Visualizzato pannello LOG")
        If Not IsPostBack Then
            Initialize()
        End If

    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(Dgr, Dgr, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub Initialize()
        Title = Facade.ResolutionTypeFacade.GetDescription(CurrentResolution.Type)

        reslLogFinder = Facade.ResolutionLogFinder
        reslLogFinder.PageSize = ResolutionEnv.SearchMaxRecords
        reslLogFinder.Id = CInt(IdResolution)

        Dgr.Finder = reslLogFinder
        Dgr.PageSize = reslLogFinder.PageSize
        Dgr.MasterTableView.SortExpressions.AddSortExpression("Id DESC")
        Dgr.DataBindFinder()
    End Sub

#End Region

End Class