Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Fascicles
Imports VecompSoftware.Helpers.ExtensionMethods

Partial Public Class FascRisultati
    Inherits FascBasePage


#Region "Properties"

    Public ReadOnly Property SelectableFaciclesThreshold As Integer
        Get
            Return ProtocolEnv.SelectableProtocolThreshold
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

#End Region

#Region " Methods "

    Private Sub Initialize()
        MasterDocSuite.TitleVisible = False
        AddHandler uscFascicleGrid.Grid.DataBound, AddressOf DataSourceChanged
    End Sub

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(uscFascicleGrid.Grid, uscFascicleGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscFascicleGrid.Grid, lblHeader)
    End Sub

    Private Sub DoSearch()
        If uscFascicleGrid.Grid.Finder Is Nothing Then
            uscFascicleGrid.Grid.Finder = CType(SessionSearchController.LoadSessionFinder(SessionSearchController.SessionFinderType.FascFinderType), FascicleFinder)
        End If
        If DocSuiteContext.Current.ProtocolEnv.ForceDescendingOrderElements Then
            uscFascicleGrid.Grid.Finder.SortExpressions.Add("Entity.RegistrationDate", "desc")
        Else
            uscFascicleGrid.Grid.Finder.SortExpressions.Add("Entity.RegistrationDate", "asc")
        End If
        uscFascicleGrid.Grid.DataBindFinder()
        uscFascicleGrid.Grid.Visible = True
    End Sub
#End Region

End Class