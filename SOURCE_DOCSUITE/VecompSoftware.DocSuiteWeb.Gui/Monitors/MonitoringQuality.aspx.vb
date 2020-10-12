Imports System.Collections.Generic
Imports System.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.DocumentSeries
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.WebAPI
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Monitors
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class MonitoringQuality
    Inherits CommonBasePage

#Region " Fields "

#End Region

#Region " Properties "

    Private ReadOnly Property DocumentSeriesId As Integer?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Integer?)("idDocumentSeries", Nothing)
        End Get
    End Property

    Private ReadOnly Property RoleName As String
        Get
            Return Request.QueryString.GetValueOrDefault("RoleName", String.Empty)
        End Get
    End Property

    Private ReadOnly Property SeriesName As String
        Get
            Return Request.QueryString.GetValueOrDefault("SeriesName", String.Empty)
        End Get
    End Property

    Private ReadOnly Property DefaultDateFrom As Date?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Date?)("dateFrom", Date.Today.AddDays(-30))
        End Get
    End Property

    Private ReadOnly Property DefaultDateTo As Date?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Date?)("dateTo", Date.Today)
        End Get
    End Property
#End Region

#Region " Events "
    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            cmdExportToExcel.Icon.SecondaryIconUrl = ImagePath.SmallExcel
            If Not CType(Session("dtpDateFrom"), Date) = Nothing OrElse Not CType(Session("dtpDateTo"), Date) = Nothing Then
                dtpDateFrom.SelectedDate = CType(Session("dtpDateFrom"), Date)
                dtpDateTo.SelectedDate = CType(Session("dtpDateTo"), Date)
            ElseIf DefaultDateFrom.HasValue AndAlso DefaultDateTo.HasValue Then
                dtpDateFrom.SelectedDate = DefaultDateFrom
                dtpDateTo.SelectedDate = DefaultDateTo
            End If
        End If
    End Sub

    Private Sub monitoringQualityGrid_NeedDataSource(sender As Object, e As EventArgs) Handles monitoringQualityGrid.NeedDataSource
        LoadMonitoringQualitySummary()
    End Sub

    Private Sub monitoringQualityGrid_PreRender(sender As Object, e As EventArgs) Handles monitoringQualityGrid.PreRender
        If Not IsPostBack Then
            ExpandeGridRecursive(monitoringQualityGrid.MasterTableView)
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        If (Not dtpDateFrom.SelectedDate.HasValue OrElse Not dtpDateTo.SelectedDate.HasValue) Then
            AjaxAlert("E' obbligatorio selezionare un range di date per la ricerca")
            Exit Sub
        End If
        monitoringQualityGrid.Rebind()
    End Sub

    Protected Sub btnClean_Click(sender As Object, e As EventArgs)
        dtpDateFrom.SelectedDate = Date.Today.AddDays(-30)
        dtpDateTo.SelectedDate = Date.Today
    End Sub

    Protected Sub monitoringQualityGrid_DetailTableDataBind(source As Object, e As Telerik.Web.UI.GridDetailTableDataBindEventArgs)
        Dim dataItem As GridDataItem = DirectCast(e.DetailTableView.ParentItem, GridDataItem)
        FillDataTable(e.DetailTableView, dataItem)
    End Sub

    Private Sub monitoringQualityGrid_ItemCommand(sender As Object, e As GridCommandEventArgs) Handles monitoringQualityGrid.ItemCommand
        Select Case e.CommandName
            Case "ViewDocumentSeriesSummary"
                Dim parameters As String = String.Format("IdDocumentSeriesItem={0}&Action={1}&Type=Series", e.CommandArgument, DocumentSeriesAction.View)
                Session("dtpDateFrom") = dtpDateFrom.SelectedDate
                Session("dtpDateTo") = dtpDateTo.SelectedDate
                Response.Redirect("~/Series/Item.aspx?" & CommonShared.AppendSecurityCheck(parameters))
        End Select
    End Sub

    Protected Sub cmdExportToExcel_Click(sender As Object, e As EventArgs) Handles cmdExportToExcel.Click
        Dim radGrid As RadGrid = monitoringQualityGrid
        radGrid.ExportSettings.ExportOnlyData = True
        radGrid.ExportSettings.OpenInNewWindow = True
        radGrid.ExportSettings.FileName = "registro.xls"
        radGrid.MasterTableView.ExportToExcel()
    End Sub
#End Region

#Region " Methods "
    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnClean, dtpDateFrom)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnClean, dtpDateTo)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, monitoringQualityGrid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(monitoringQualityGrid, monitoringQualityGrid, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub LoadMonitoringQualitySummary()
        Dim elements As ICollection(Of WebAPIDto(Of MonitoringQualitySummaryModel)) = WebAPIImpersonatorFacade.ImpersonateFinder(New MonitoringQualitySummaryFinder(DocSuiteContext.Current.CurrentTenant),
                    Function(impersonationType, finder)
                        finder.ResetDecoration()
                        finder.DateFrom = New DateTimeOffset(dtpDateFrom.SelectedDate.Value)
                        finder.DateTo = New DateTimeOffset(dtpDateTo.SelectedDate.Value).AddDays(1)
                        finder.EnablePaging = False
                        finder.EnableTopOdata = False

                        If DocumentSeriesId.HasValue Then
                            finder.IdDocumentSeries = DocumentSeriesId.Value.ToString()
                        End If
                        If Not String.IsNullOrEmpty(RoleName) Then
                            finder.RoleName = RoleName
                        End If
                        If Not String.IsNullOrEmpty(SeriesName) Then
                            finder.DocumentSeries = SeriesName
                        End If
                        Return finder.DoSearchHeader()
                    End Function)

        Dim gridElements As List(Of MonitoringQualitySummaryModel) = elements.Select(Function(x) x.Entity).ToList()
        Dim gridElementsGrouped As List(Of MonitoringQualitySummaryModel) = (From row In gridElements
                                                                             Group row By IdDocumentSeries = row.IdDocumentSeries.Value Into MonitoringQualityGroup = Group
                                                                             Select New MonitoringQualitySummaryModel With {
                                                                        .IdDocumentSeries = IdDocumentSeries,
                                                                        .DocumentSeries = MonitoringQualityGroup.FirstOrDefault(Function(x) x.IdDocumentSeries.Value = IdDocumentSeries).DocumentSeries,
                                                                        .IdRole = 0,
                                                                        .Role = "",
                                                                        .FromProtocol = MonitoringQualityGroup.Sum(Function(r) r.FromProtocol),
                                                                        .FromResolution = MonitoringQualityGroup.Sum(Function(r) r.FromResolution),
                                                                        .Published = MonitoringQualityGroup.Sum(Function(r) r.Published),
                                                                        .WithoutDocument = MonitoringQualityGroup.Sum(Function(r) r.WithoutDocument),
                                                                        .WithoutLink = MonitoringQualityGroup.Sum(Function(r) r.WithoutLink)
                                                                   }).OrderBy(Function(f) f.DocumentSeries).ToList()
        monitoringQualityGrid.DataSource = gridElementsGrouped
    End Sub

    Private Sub LoadMonitoringQualitySummary(IdDocumentSeries As String, detailTableView As GridTableView)
        Dim elements As ICollection(Of WebAPIDto(Of MonitoringQualitySummaryModel)) = WebAPIImpersonatorFacade.ImpersonateFinder(New MonitoringQualitySummaryFinder(DocSuiteContext.Current.CurrentTenant),
                    Function(impersonationType, finder)
                        finder.ResetDecoration()
                        finder.DateFrom = New DateTimeOffset(dtpDateFrom.SelectedDate.Value)
                        finder.DateTo = New DateTimeOffset(dtpDateTo.SelectedDate.Value).AddDays(1)
                        finder.IdDocumentSeries = IdDocumentSeries
                        finder.EnablePaging = False
                        finder.EnableTopOdata = False

                        If Not String.IsNullOrEmpty(RoleName) Then
                            finder.RoleName = RoleName
                        End If
                        If Not String.IsNullOrEmpty(SeriesName) Then
                            finder.DocumentSeries = SeriesName
                        End If
                        Return finder.DoSearchHeader()
                    End Function)

        Dim detailTableViewElements As List(Of MonitoringQualitySummaryModel) = elements.Select(Function(x) x.Entity).OrderBy(Function(f) f.Role).ToList()
        detailTableView.DataSource = detailTableViewElements
    End Sub

    Private Sub LoadMonitoringQualityDetails(IdDocumentSeries As String, IdRole As String, detailTableView As GridTableView)
        Dim elements As ICollection(Of WebAPIDto(Of MonitoringQualityDetailsModel)) = WebAPIImpersonatorFacade.ImpersonateFinder(New MonitoringQualityDetailsFinder(DocSuiteContext.Current.CurrentTenant),
                    Function(impersonationType, finder)
                        finder.ResetDecoration()
                        finder.DateFrom = New DateTimeOffset(dtpDateFrom.SelectedDate.Value)
                        finder.DateTo = New DateTimeOffset(dtpDateTo.SelectedDate.Value).AddDays(1)
                        finder.IdDocumentSeries = IdDocumentSeries
                        finder.IdRole = IdRole
                        finder.EnablePaging = False
                        finder.EnableTopOdata = False
                        Return finder.DoSearchHeader()
                    End Function)

        Dim detailTableViewElements As List(Of MonitoringQualityDetailsModel) = elements.Select(Function(x) x.Entity).OrderBy(Function(f) f.Year.Value).OrderBy(Function(f) f.Number.Value).ToList()
        detailTableView.DataSource = detailTableViewElements
    End Sub
    Private Sub FillDataTable(detailTableView As GridTableView, dataItem As GridDataItem)
        Select Case detailTableView.Name
            Case "MonitoringQualityRole"
                Dim IdDocumentSeries As String = dataItem.GetDataKeyValue("IdDocumentSeries").ToString()
                LoadMonitoringQualitySummary(IdDocumentSeries, detailTableView)
                Exit Select

            Case "MonitoringQualityDetails"
                Dim keyRole As Object = dataItem.GetDataKeyValue("IdRole")
                Dim IdRole As String = If(keyRole Is Nothing, "null", keyRole.ToString())
                Dim IdDocumentSeries As String = dataItem.GetDataKeyValue("IdDocumentSeries").ToString()
                LoadMonitoringQualityDetails(IdDocumentSeries, IdRole, detailTableView)
                Exit Select
        End Select
    End Sub

    Private Sub ExpandeGridRecursive(tableView As GridTableView)
        If DocumentSeriesId.HasValue OrElse Not String.IsNullOrEmpty(SeriesName) Then
            Dim nestedViewItems As GridItem() = tableView.GetItems(GridItemType.NestedView)
            For Each nestedViewItem As GridNestedViewItem In nestedViewItems
                For Each nestedView As GridTableView In nestedViewItem.NestedTableViews
                    nestedView.ParentItem.Expanded = True
                    If Not String.IsNullOrEmpty(RoleName) Then
                        ExpandeGridRecursive(nestedView)
                    End If
                Next
            Next
        End If

    End Sub
#End Region

End Class