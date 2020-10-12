Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade

Partial Public Class ProtRolesLog
    Inherits ProtBasePage

#Region " Properties "

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjaxSettings()
        If Not Page.IsPostBack Then
            Title = $"Protocollo - Movimentazioni {CurrentProtocol.Id}"
            Initialize()
        End If

    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(GridProt, GridProt, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub Initialize()
        Dim protLogFinder As NHibernateProtocolLogFinder = Facade.ProtocolLogFinder

        protLogFinder.SystemUser = String.Empty
        protLogFinder.UniqueIdProtocol = CurrentProtocol.Id
        protLogFinder.PageSize = ProtocolEnv.SearchMaxRecords
        protLogFinder.LogType = ProtocolLogEvent.AR.ToString()

        GridProt.Finder = protLogFinder
        GridProt.PageSize = protLogFinder.PageSize
        GridProt.MasterTableView.SortExpressions.AddSortExpression("Year DESC")
        GridProt.MasterTableView.SortExpressions.AddSortExpression("Number DESC")
        GridProt.DataBindFinder()
    End Sub

#End Region

End Class