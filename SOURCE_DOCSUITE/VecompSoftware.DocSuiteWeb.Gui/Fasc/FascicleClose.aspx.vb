Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Fascicles
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles

Public Class FascicleClose
    Inherits FascBasePage

#Region " Fields "
    Private _finder As FascicleFinder = New FascicleFinder(DocSuiteContext.Current.Tenants)
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjaxSettings()

        If Not IsPostBack Then
            Initialize()
            DoSearch()
        End If
    End Sub

    Protected Sub DataSourceChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim gvFascicles As BindGrid = uscFascicleGrid.Grid
        If gvFascicles.DataSource IsNot Nothing Then
            lblHeader.Text = String.Format("Fascicoli da chiudere ({0}/{1})", gvFascicles.DataSource.Count, gvFascicles.VirtualItemCount)
        Else
            lblHeader.Text = "Fascicoli da chiudere - Nessun Risultato"
        End If
        MasterDocSuite.HistoryTitle = lblHeader.Text
    End Sub

    Private Sub ImpersonationFinderDelegate(ByVal source As Object, ByVal e As EventArgs)
        uscFascicleGrid.Grid.SetImpersonationAction(AddressOf ImpersonateGridCallback)
        uscFascicleGrid.Grid.SetImpersonationCounterAction(AddressOf ImpersonateGridCallback)
    End Sub
#End Region

#Region " Methods "
    Private Sub Initialize()
        MasterDocSuite.TitleVisible = False
    End Sub

    Private Sub InitializeAjaxSettings()
        AddHandler uscFascicleGrid.Grid.NeedImpersonation, AddressOf ImpersonationFinderDelegate
        AddHandler uscFascicleGrid.Grid.DataBound, AddressOf DataSourceChanged
        AjaxManager.AjaxSettings.AddAjaxSetting(uscFascicleGrid.Grid, uscFascicleGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub DoSearch()
        Dim finderModel As FascicleFinderModel = New FascicleFinderModel()
        finderModel.ViewOnlyClosable = True
        finderModel.ApplySecurity = True
        _finder.FascicleFinderModel = finderModel
        _finder.FromPostMethod = True

        Dim setSortExpression As Boolean = False

        If ProtocolEnv.SearchMaxRecords <> 0 Then
            _finder.PageSize = ProtocolEnv.SearchMaxRecords
        End If

        If uscFascicleGrid.Grid.Finder Is Nothing Then
            uscFascicleGrid.Grid.Finder = _finder
            setSortExpression = True
        End If
        If setSortExpression Then
            Dim order As String = If(DocSuiteContext.Current.ProtocolEnv.ForceDescendingOrderElements, "desc", "asc")
            uscFascicleGrid.Grid.Finder.SortExpressions.Add("Entity.StartDate", order)
        End If
        uscFascicleGrid.ColumnLastChangedDateVisible = True
        uscFascicleGrid.Grid.DataBindFinder()
        uscFascicleGrid.Grid.Visible = True
    End Sub

    Private Function ImpersonateGridCallback(Of TResult)(finder As IFinder, callback As Func(Of TResult)) As TResult
        Return WebAPIImpersonatorFacade.ImpersonateFinder(Of FascicleFinder, TResult)(finder,
                        Function(impersonationType, wfinder)
                            Return callback()
                        End Function)
    End Function
#End Region
End Class