Imports System.Collections.Generic
Imports System.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.DocumentSeries
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Monitors

Public Class MonitoringSeriesSection
    Inherits CommonBasePage

#Region " Fields "

#End Region

#Region " Properties "

#End Region

#Region " Events "
    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            cmdExportToExcel.Icon.SecondaryIconUrl = ImagePath.SmallExcel
            If Not CType(Session("dtpDateFrom"), Date) = Nothing OrElse Not CType(Session("dtpDateTo"), Date) = Nothing Then
                dtpDateFrom.SelectedDate = CType(Session("dtpDateFrom"), Date)
                dtpDateTo.SelectedDate = CType(Session("dtpDateTo"), Date)
            Else
                dtpDateFrom.SelectedDate = Date.Today.AddDays(-30)
                dtpDateTo.SelectedDate = Date.Today
            End If
            LoadMonitoringSeriesSectionFamilies()
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        If (Not dtpDateFrom.SelectedDate.HasValue OrElse Not dtpDateTo.SelectedDate.HasValue) Then
            AjaxAlert("E' obbligatorio selezionare un range di date per la ricerca")
            Exit Sub
        End If
        LoadMonitoringSeriesSectionFamilies()
    End Sub

    Protected Sub btnClean_Click(sender As Object, e As EventArgs)
        dtpDateFrom.SelectedDate = Date.Today.AddDays(-30)
        dtpDateTo.SelectedDate = Date.Today
    End Sub

    Protected Sub monitoringSeriesSectionGrid_ItemCreated(sender As Object, e As GridItemEventArgs) Handles monitoringSeriesSectionGrid.ItemCreated
        If TypeOf e.Item Is GridNoRecordsItem Then
            e.Item.OwnerTableView.Visible = False
        End If
    End Sub

    Protected Sub monitoringSeriesSectionGrid_DetailTableDataBind(source As Object, e As GridDetailTableDataBindEventArgs)
        Dim dataItem As GridDataItem = e.DetailTableView.ParentItem
        Select Case e.DetailTableView.Name
            Case "MonitoringSection"
                Dim family As String = dataItem.GetDataKeyValue("Family").ToString()
                LoadMonitoringSeriesSectionSummary(family)
                Exit Select

            Case "MonitoringSectionDetails"
                Dim series As String = dataItem.GetDataKeyValue("Series").ToString()
                Dim family As String = dataItem.GetDataKeyValue("Family").ToString()
                LoadMonitoringSeriesSectionDetails(family, series)
                Exit Select
        End Select
    End Sub

    Private Sub monitoringSeriesRoleGrid_ItemCommand(sender As Object, e As GridCommandEventArgs) Handles monitoringSeriesSectionGrid.ItemCommand
        Select Case e.CommandName
            Case "ViewMonitoringQuality"
                Dim args As String = e.CommandArgument.ToString()
                Dim parameterString As String = String.Format("type=Series&dateFrom={0:dd/MM/yyyy}&dateTo={1:dd/MM/yyyy}&SeriesName={2}", dtpDateFrom.SelectedDate, dtpDateTo.SelectedDate, args)
                Session("dtpDateFrom") = dtpDateFrom.SelectedDate
                Session("dtpDateTo") = dtpDateTo.SelectedDate
                Response.Redirect(String.Concat("~/Monitors/MonitoringQuality.aspx?", parameterString))
        End Select
    End Sub

    Private Sub cmdExportToExcel_Click(sender As Object, e As EventArgs) Handles cmdExportToExcel.Click
        Dim radGrid As RadGrid = monitoringSeriesSectionGrid
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
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, monitoringSeriesSectionGrid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(monitoringSeriesSectionGrid, monitoringSeriesSectionGrid, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub LoadMonitoringSeriesSectionFamilies()
        Dim finder As MonitoringSeriesSectionFinder = New MonitoringSeriesSectionFinder(DocSuiteContext.Current.Tenants) With {
            .DateFrom = New DateTimeOffset(dtpDateFrom.SelectedDate.Value),
            .DateTo = New DateTimeOffset(dtpDateTo.SelectedDate.Value).AddDays(1),
            .EnablePaging = False,
            .EnableTopOdata = False
        }
        Dim elements As ICollection(Of WebAPIDto(Of MonitoringSeriesSectionModel)) = finder.DoSearchHeader()
        Dim gridElements As List(Of MonitoringSeriesSectionModel) = elements.Select(Function(x) x.Entity).ToList()
        Dim gridElementsGrouped As List(Of MonitoringSeriesSectionModel) = (From row In gridElements
                                                                            Group row By Family = row.Family Into MonitoringSeriesSectionGroup = Group
                                                                            Select New MonitoringSeriesSectionModel With {
                                                                        .Family = Family,
                                                                        .ActivePublished = MonitoringSeriesSectionGroup.Sum(Function(r) r.ActivePublished),
                                                                        .Inserted = MonitoringSeriesSectionGroup.Sum(Function(r) r.Inserted),
                                                                        .Published = MonitoringSeriesSectionGroup.Sum(Function(r) r.Published),
                                                                        .Updated = MonitoringSeriesSectionGroup.Sum(Function(r) r.Updated),
                                                                        .Canceled = MonitoringSeriesSectionGroup.Sum(Function(r) r.Canceled),
                                                                        .Retired = MonitoringSeriesSectionGroup.Sum(Function(r) r.Retired),
                                                                        .LastUpdated = MonitoringSeriesSectionGroup.Max(Function(x) x.LastUpdated)
                                                                   }).OrderBy(Function(f) f.Family).OrderBy(Function(f) f.Series).ToList()

        monitoringSeriesSectionGrid.DataSource = gridElementsGrouped
        monitoringSeriesSectionGrid.DataBind()
    End Sub

    Private Sub LoadMonitoringSeriesSectionSummary(familyName As String)
        Dim finder As MonitoringSeriesSectionFinder = New MonitoringSeriesSectionFinder(DocSuiteContext.Current.Tenants) With {
            .DateFrom = New DateTimeOffset(dtpDateFrom.SelectedDate.Value),
            .DateTo = New DateTimeOffset(dtpDateTo.SelectedDate.Value).AddDays(1),
            .FamilyName = familyName.Replace("'"c, "''"),
            .EnablePaging = False,
            .EnableTopOdata = False
        }
        Dim elements As ICollection(Of WebAPIDto(Of MonitoringSeriesSectionModel)) = finder.DoSearchHeader()
        Dim gridElements As List(Of MonitoringSeriesSectionModel) = elements.Select(Function(x) x.Entity).ToList()

        Dim gridElementsGrouped As List(Of MonitoringSeriesSectionModel) = (From row In gridElements
                                                                            Group row By Series = row.Series Into MonitoringSeriesSectionGroup = Group
                                                                            Select New MonitoringSeriesSectionModel With {
                                                                        .Family = MonitoringSeriesSectionGroup.First(Function(x) x.Series = Series).Family,
                                                                        .Series = Series,
                                                                        .ActivePublished = MonitoringSeriesSectionGroup.Sum(Function(r) r.ActivePublished),
                                                                        .Inserted = MonitoringSeriesSectionGroup.Sum(Function(r) r.Inserted),
                                                                        .Published = MonitoringSeriesSectionGroup.Sum(Function(r) r.Published),
                                                                        .Updated = MonitoringSeriesSectionGroup.Sum(Function(r) r.Updated),
                                                                        .Canceled = MonitoringSeriesSectionGroup.Sum(Function(r) r.Canceled),
                                                                        .Retired = MonitoringSeriesSectionGroup.Sum(Function(r) r.Retired),
                                                                        .LastUpdated = MonitoringSeriesSectionGroup.Max(Function(x) x.LastUpdated)
                                                                   }).OrderBy(Function(f) f.Family).OrderBy(Function(f) f.Series).ToList()

        monitoringSeriesSectionGrid.DataSource = gridElementsGrouped
    End Sub

    Private Sub LoadMonitoringSeriesSectionDetails(familyName As String, seriesName As String)
        Dim finder As MonitoringSeriesSectionFinder = New MonitoringSeriesSectionFinder(DocSuiteContext.Current.Tenants) With {
            .DateFrom = New DateTimeOffset(dtpDateFrom.SelectedDate.Value),
            .DateTo = New DateTimeOffset(dtpDateTo.SelectedDate.Value).AddDays(1),
            .FamilyName = familyName.Replace("'"c, "''"),
            .SeriesName = seriesName.Replace("'"c, "''"),
            .EnablePaging = False,
            .EnableTopOdata = False
        }
        Dim elements As ICollection(Of WebAPIDto(Of MonitoringSeriesSectionModel)) = finder.DoSearchHeader()
        Dim gridElements As List(Of MonitoringSeriesSectionModel) = elements.Select(Function(x) x.Entity).ToList()
        monitoringSeriesSectionGrid.Visible = True
        If gridElements.Count <= 0 OrElse (gridElements.Count = 1 AndAlso String.IsNullOrEmpty(gridElements.First().SubSection)) Then
            monitoringSeriesSectionGrid.DataSource = String.Empty
            Return
        End If
        monitoringSeriesSectionGrid.DataSource = gridElements
    End Sub
#End Region

End Class