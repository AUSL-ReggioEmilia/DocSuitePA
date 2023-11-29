Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Report
Imports VecompSoftware.DocSuiteWeb.Report
Imports VecompSoftware.Helpers
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models

Namespace Series

    Public Class LogSummary
        Inherits CommonBasePage

#Region "Properties"
        Private ReadOnly Property IdArchive As Integer?
            Get
                Dim archive As Integer? = Nothing
                If Not String.IsNullOrEmpty(ddlContainerArchive.SelectedValue) Then
                    archive = Integer.Parse(ddlContainerArchive.SelectedValue)
                End If
                Return archive
            End Get
        End Property
#End Region
#Region " Events "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            InitializeAjax()

            If Not IsPostBack() Then
                Title = "Statistiche"
                dtpLogDateFrom.SelectedDate = DateTime.Today.AddMonths(-1)

                InitializeCommands()
                BindGrid()
            End If
        End Sub
        Private Sub cmdRefresh_Click(sender As Object, e As EventArgs) Handles cmdRefresh.Click
            BindGrid()
        End Sub
        Private Sub cmdClearFilters_Click(sender As Object, e As EventArgs) Handles cmdClearFilters.Click
            dtpLogDateFrom.SelectedDate = Nothing
            dtpLogDateTo.SelectedDate = Nothing
            ddlContainerArchive.ClearSelection()

            BindGrid()
        End Sub

        Private Sub ddlContainerArchive_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlContainerArchive.SelectedIndexChanged
            Dim archive As ContainerArchive = Nothing
            Dim idArchive As Integer
            If Integer.TryParse(ddlContainerArchive.SelectedValue, idArchive) Then
                archive = Facade.ContainerArchiveFacade.GetById(idArchive)
            End If

            BindGrid()
        End Sub

        Private Sub cmdExportToExcel_Click(sender As Object, e As EventArgs) Handles cmdExportToExcel.Click
            Dim columns As IEnumerable(Of Column) = grdLogSummary.Columns.Cast(Of GridColumn).Select(Function(c) New Column(c))
            If ReportFacade.TenantModel Is Nothing Then
                ReportFacade.TenantModel = DocSuiteContext.Current.CurrentTenant
            End If
            Dim report As IReport(Of RadGrid) = ReportFacade.GenerateReport(Of RadGrid)(New List(Of RadGrid) From {grdLogSummary}, columns.ToList())
            Dim doc As DocumentInfo = report.ExportExcel("DocumentSeriesLogSummary.xls")

            Dim file As FileInfo = BiblosFacade.SaveUniqueToTemp(doc)
            Dim jsWindowOpen As String = "window.open('{0}/Temp/{1}');"
            jsWindowOpen = String.Format(jsWindowOpen, DocSuiteContext.Current.CurrentTenant.DSWUrl, file.Name)
            AjaxManager.ResponseScripts.Add(jsWindowOpen)

            ' Rieseguo BindGrid per via di un bug nel metodo ReportFacade.GenerateReport che invalida il viewstate di grdLogSummary sbiancandone la visualizzazione.
            BindGrid()
        End Sub

#End Region

#Region " Methods "

        Private Sub InitializeAjax()
            AjaxManager.AjaxSettings.AddAjaxSetting(grdLogSummary, grdLogSummary, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdRefresh, grdLogSummary, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdClearFilters, grdLogSummary, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdClearFilters, dtpLogDateFrom)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdClearFilters, dtpLogDateTo)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdClearFilters, ddlContainerArchive)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdExportToExcel, grdLogSummary, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlContainerArchive, grdLogSummary, MasterDocSuite.AjaxDefaultLoadingPanel)
        End Sub
        Private Sub InitializeCommands()
            ddlContainerArchive.DataValueField = "Id"
            ddlContainerArchive.DataTextField = "Name"
            ddlContainerArchive.DataSource = Facade.ContainerArchiveFacade.GetAllOrdered("Name ASC")
            ddlContainerArchive.DataBind()
            ddlContainerArchive.Items.Insert(0, "")

            cmdExportToExcel.Icon.SecondaryIconUrl = ImagePath.SmallExcel
        End Sub
        Private Sub BindGrid()
            Dim finder As New DocumentSeriesItemFinder(False, Nothing) With {.EnablePaging = False}
            finder.LogDate = New Range(Of DateTime)(dtpLogDateFrom.SelectedDate, dtpLogDateTo.SelectedDate)
            If IdArchive.HasValue Then
                finder.IdArchive = IdArchive.Value
            End If
            Dim summaries As IList(Of DocumentSeriesLogSummaryDTO) = finder.DoSearchLogSummary()

            Dim fillers As IList(Of DocumentSeriesLogSummaryDTO) = FacadeFactory.Instance.DocumentSeriesFacade.GetEmptyLogSummaries(IdArchive)
            Dim missing As IEnumerable(Of DocumentSeriesLogSummaryDTO) = fillers.Where(Function(f) Not summaries.Any(Function(s) s.Id = f.Id))
            summaries = summaries.Concat(missing) _
                .OrderBy(Function(s) s.Name).ThenBy(Function(s) s.Id) _
                .ToList()

            grdLogSummary.DataSource = summaries
            grdLogSummary.DataBind()
        End Sub

#End Region

    End Class

End Namespace