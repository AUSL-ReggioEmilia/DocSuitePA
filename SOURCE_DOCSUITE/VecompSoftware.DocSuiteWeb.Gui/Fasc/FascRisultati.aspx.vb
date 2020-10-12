Imports System.Collections.Generic
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Fascicles
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.ExtensionMethods

Partial Public Class FascRisultati
    Inherits FascBasePage


#Region "Properties"

    Public ReadOnly Property SelectableFaciclesThreshold As Integer
        Get
            Return ProtocolEnv.SelectableProtocolThreshold
        End Get
    End Property

    Public ReadOnly Property ChoiseFolderEnabled() As String
        Get
            Return Request.QueryString.Get("ChoiseFolderEnabled")
        End Get
    End Property

    Public ReadOnly Property SelectedFascicleFolderId() As String
        Get
            Return Request.QueryString.Get("SelectedFascicleFolderId")
        End Get
    End Property

    Public ReadOnly Property CurrentFascicleId() As String
        Get
            Return Request.QueryString.Get("CurrentFascicleId")
        End Get
    End Property

    Public ReadOnly Property BackButtonEnabled() As Boolean
        Get
            Return GetKeyValueOrDefault("BackButtonEnabled", False)
        End Get
    End Property

    Public ReadOnly Property EnableSessionFilterLoading() As Boolean
        Get
            Return GetKeyValueOrDefault("EnableSessionFilterLoading", False)
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjaxSettings()
        If Action.Eq("SearchFascicles") Then
            MasterDocSuite.TitleVisible = False
            uscFascicleGrid.ColumnClientSelectVisible = False
            btnDeselectAll.Visible = False
            btnDocuments.Visible = False
            btnSelectAll.Visible = False
            backBtn.Visible = BackButtonEnabled
        End If
        If Not IsPostBack Then
            Initialize()
            DoSearch()
        End If
    End Sub

    Protected Sub DataSourceChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim gvFascicles As BindGrid = uscFascicleGrid.Grid
        If gvFascicles.DataSource IsNot Nothing Then
            lblHeader.Text = String.Format("Fascicoli - Risultati ({0}/{1})", gvFascicles.DataSource.Count, gvFascicles.VirtualItemCount)
        Else
            lblHeader.Text = "Fascicoli - Nessun Risultato"
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
        AjaxManager.AjaxSettings.AddAjaxSetting(uscFascicleGrid.Grid, lblHeader)
    End Sub

    Private Sub DoSearch()
        Dim setSortExpression As Boolean = False
        If uscFascicleGrid.Grid.Finder Is Nothing OrElse EnableSessionFilterLoading Then
            uscFascicleGrid.Grid.Finder = CType(SessionSearchController.LoadSessionFinder(SessionSearchController.SessionFinderType.FascFinderType), FascicleFinder)
            setSortExpression = True
        End If
        If setSortExpression Then
            Dim order As String = If(DocSuiteContext.Current.ProtocolEnv.ForceDescendingOrderElements, "desc", "asc")
            uscFascicleGrid.Grid.Finder.SortExpressions("Entity.StartDate") = order
        End If
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