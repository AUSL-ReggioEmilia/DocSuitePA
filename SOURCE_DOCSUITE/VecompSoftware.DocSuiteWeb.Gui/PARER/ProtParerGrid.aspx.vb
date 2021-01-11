Imports VecompSoftware.DocSuiteWeb.Data

Public Class ProtParerGrid
    Inherits CommonBasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Initialize()
        InitializeAjaxSettings()
        If Not IsPostBack Then
            DoSearch()
        End If
    End Sub

    Private Sub Initialize()
        MasterDocSuite.TitleVisible = False
        AddHandler uscProtocolGrid.Grid.DataBound, AddressOf DataSourceChanged

        uscProtocolGrid.ColumnPARERIconVisible = True
        uscProtocolGrid.ColumnPARERDescriptionVisible = True
        uscProtocolGrid.ColumnClientSelectVisible = False
        uscProtocolGrid.ColumnHasReadVisible = False
        uscProtocolGrid.ColumnViewLinksVisible = False
    End Sub

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(uscProtocolGrid.Grid, uscProtocolGrid.Grid, MasterDocSuite.AjaxLoadingPanelSearch)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscProtocolGrid.Grid, lblHeader)
    End Sub

    Protected Sub DataSourceChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim gvProtocols As BindGrid = uscProtocolGrid.Grid
        If gvProtocols.DataSource IsNot Nothing Then
            lblHeader.Text = "Protocollo - Risultati (" & gvProtocols.DataSource.Count & "/" & gvProtocols.VirtualItemCount & ")"
        Else
            lblHeader.Text = "Protocollo - Nessun Risultato"
        End If
        MasterDocSuite.HistoryTitle = lblHeader.Text
    End Sub

    Private Sub DoSearch()
        Dim gvProtocols As BindGrid = uscProtocolGrid.Grid
        Dim finder As NHibernateProtocolFinder = SessionSearchController.LoadSessionFinder(SessionSearchController.SessionFinderType.ProtocolParerFinderType)

        gvProtocols.PageSize = finder.PageSize
        gvProtocols.MasterTableView.SortExpressions.Clear()
        gvProtocols.Finder = finder
        gvProtocols.DataBindFinder()
        gvProtocols.Visible = True
        gvProtocols.MasterTableView.AllowMultiColumnSorting = False

    End Sub




End Class