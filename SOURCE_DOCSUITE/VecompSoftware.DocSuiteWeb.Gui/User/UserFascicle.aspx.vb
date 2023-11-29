Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Fascicles
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class UserFascicle
    Inherits UserBasePage

#Region " Fields "
    Dim _actionType As String = String.Empty
    Public Const ACTIONTYPE_FOR As String = "FOR"
    Public Const ACTIONTYPE_FAU As String = "FAU"

#End Region

#Region " Properties "

    Protected ReadOnly Property ActionType As String
        Get
            If String.IsNullOrEmpty(_actionType) Then
                _actionType = Me.Request.QueryString.GetValueOrDefault("Action", String.Empty)
            End If
            Return _actionType
        End Get
    End Property

    Public ReadOnly Property SelectableFaciclesThreshold As Integer
        Get
            Return ProtocolEnv.SelectableProtocolThreshold
        End Get
    End Property
#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Title = Request.QueryString("Title")
            InitializeDateRanges()
            InitializeResultsAjax()
            uscFascicleGrid.Grid.DiscardFinder()
            Search()
        End If

    End Sub

    Private Sub InitializeDateRanges()
        rdpDateFrom.SelectedDate = Date.Today.AddDays(-1 * ProtocolEnv.DesktopDayDiff)
        rdpDateTo.SelectedDate = Date.Today
    End Sub

    Private Sub InitializeResultsAjax()
        AddHandler uscFascicleGrid.Grid.NeedImpersonation, AddressOf ImpersonationFinderDelegate
        AddHandler uscFascicleGrid.Grid.DataBound, AddressOf DataSourceChanged

        AjaxManager.AjaxSettings.AddAjaxSetting(uscFascicleGrid.Grid, uscFascicleGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Protected Sub DataSourceChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim gvFascicles As BindGrid = uscFascicleGrid.Grid
        If gvFascicles.DataSource IsNot Nothing Then
            Title = String.Format("{0} - Risultati ({1}/{2})", Request.QueryString("Title"), gvFascicles.DataSource.Count, gvFascicles.VirtualItemCount)
        Else
            Title = Request.QueryString("Title")
        End If
        MasterDocSuite.HistoryTitle = Title
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        Search()
    End Sub

    Private Sub Search()
        Dim fascicleFinder As New FascicleFinder(DocSuiteContext.Current.CurrentTenant)

        If ProtocolEnv.SearchMaxRecords <> 0 Then
            fascicleFinder.PageSize = ProtocolEnv.SearchMaxRecords
        Else
            'TODO: Investigate where to have a default value for ProtocolEnv.SearchMaxRecords
            fascicleFinder.PageSize = 100
        End If
        fascicleFinder.FascicleFinderModel = New Model.Entities.Fascicles.FascicleFinderModel()
        If rdpDateFrom.SelectedDate.HasValue Then
            fascicleFinder.FascicleFinderModel.StartDateFrom = rdpDateFrom.SelectedDate.Value
        End If
        If rdpDateTo.SelectedDate.HasValue Then
            fascicleFinder.FascicleFinderModel.StartDateTo = rdpDateTo.SelectedDate.Value
        End If
        fascicleFinder.ExpandProperties = False
        fascicleFinder.PageIndex = 0
        fascicleFinder.FromPostMethod = True

        If Me.ActionType = ACTIONTYPE_FOR Then
            ' FOR - Aperti
            fascicleFinder.FascicleFinderModel.ApplySecurity = True
            fascicleFinder.FascicleFinderModel.ViewOnlyClosable = True
        Else
            'FAU - Authorized
            fascicleFinder.FascicleFinderModel.ApplySecurity = True
        End If

        DoSearch(fascicleFinder)
    End Sub

    Private Sub DoSearch(finder As IFinder)
        uscFascicleGrid.Grid.Finder = finder
        uscFascicleGrid.Grid.Finder.SortExpressions("Entity.StartDate") = "desc"
        uscFascicleGrid.Grid.DataBindFinder()
        uscFascicleGrid.Grid.Visible = True
    End Sub

    Private Sub ImpersonationFinderDelegate(ByVal source As Object, ByVal e As EventArgs)
        uscFascicleGrid.Grid.SetImpersonationAction(AddressOf ImpersonateGridCallback)
        uscFascicleGrid.Grid.SetImpersonationCounterAction(AddressOf ImpersonateGridCallback)
    End Sub

    Private Function ImpersonateGridCallback(Of TResult)(finder As IFinder, callback As Func(Of TResult)) As TResult
        Return WebAPIImpersonatorFacade.ImpersonateFinder(Of FascicleFinder, TResult)(finder,
                        Function(impersonationType, wfinder)
                            Return callback()
                        End Function)
    End Function

End Class