Imports System.Collections.Generic
Imports System.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.DocumentSeries
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Monitors

Public Class MonitoringSeriesRole
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
            LoadMonitoringSeriesRole()
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        If (Not dtpDateFrom.SelectedDate.HasValue OrElse Not dtpDateTo.SelectedDate.HasValue) Then
            AjaxAlert("E' obbligatorio selezionare un range di date per la ricerca")
            Exit Sub
        End If
        LoadMonitoringSeriesRole()
    End Sub

    Protected Sub btnClean_Click(sender As Object, e As EventArgs)
        dtpDateFrom.SelectedDate = Date.Today.AddDays(-30)
        dtpDateTo.SelectedDate = Date.Today
    End Sub

    Protected Sub monitoringSeriesRoleGrid_DetailTableDataBind(source As Object, e As GridDetailTableDataBindEventArgs)
        Dim dataItem As GridDataItem = DirectCast(e.DetailTableView.ParentItem, GridDataItem)
        Select Case e.DetailTableView.Name
            Case "MonitoringSeriesRoleDetails"
                Dim Role As String = dataItem.GetDataKeyValue("Role").ToString()
                e.DetailTableView.DataSource = LoadMonitoringSeriesRole(Role)
        End Select
    End Sub

    Private Sub monitoringSeriesRoleGrid_ItemCommand(sender As Object, e As GridCommandEventArgs) Handles monitoringSeriesRoleGrid.ItemCommand
        Select Case e.CommandName
            Case "ViewMonitoringQuality"
                Dim args As String() = e.CommandArgument.ToString().Split("|"c)
                Dim parameterString As String = String.Format("type=Series&dateFrom={0:dd/MM/yyyy}&dateTo={1:dd/MM/yyyy}&idDocumentSeries={2}&RoleName={3}", dtpDateFrom.SelectedDate, dtpDateTo.SelectedDate, args.First(), args.Last())
                Session("dtpDateFrom") = dtpDateFrom.SelectedDate
                Session("dtpDateTo") = dtpDateTo.SelectedDate
                Response.Redirect(String.Concat("~/Monitors/MonitoringQuality.aspx?", parameterString))
        End Select
    End Sub
    Private Sub cmdExportToExcel_Click(sender As Object, e As EventArgs) Handles cmdExportToExcel.Click
        Dim radGrid As RadGrid = monitoringSeriesRoleGrid
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
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, monitoringSeriesRoleGrid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(monitoringSeriesRoleGrid, monitoringSeriesRoleGrid, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub LoadMonitoringSeriesRole()
        Dim elements As ICollection(Of WebAPIDto(Of MonitoringSeriesRoleModel)) = WebAPIImpersonatorFacade.ImpersonateFinder(New MonitoringSeriesRoleFinder(DocSuiteContext.Current.CurrentTenant),
                    Function(impersonationType, finder)
                        finder.ResetDecoration()
                        finder.DateFrom = New DateTimeOffset(dtpDateFrom.SelectedDate.Value)
                        finder.DateTo = New DateTimeOffset(dtpDateTo.SelectedDate.Value).AddDays(1)
                        finder.EnablePaging = False
                        finder.EnableTopOdata = False
                        Return finder.DoSearchHeader()
                    End Function)

        Dim gridElements As List(Of MonitoringSeriesRoleModel) = elements.Select(Function(x) x.Entity).ToList()
        Dim gridElementsGrouped As List(Of MonitoringSeriesRoleModel) = (From row In gridElements
                                                                         Group row By Role = row.Role Into MonitoringSeriesRoleGroup = Group
                                                                         Select New MonitoringSeriesRoleModel With {
                                                                        .Role = Role,
                                                                        .DocumentSeries = "",
                                                                        .IdDocumentSeries = 0,
                                                                        .ActivePublished = MonitoringSeriesRoleGroup.Sum(Function(r) r.ActivePublished),
                                                                        .Inserted = MonitoringSeriesRoleGroup.Sum(Function(r) r.Inserted),
                                                                        .Published = MonitoringSeriesRoleGroup.Sum(Function(r) r.Published),
                                                                        .Updated = MonitoringSeriesRoleGroup.Sum(Function(r) r.Updated),
                                                                        .Cancelled = MonitoringSeriesRoleGroup.Sum(Function(r) r.Cancelled),
                                                                        .Retired = MonitoringSeriesRoleGroup.Sum(Function(r) r.Retired),
                                                                        .LastUpdated = MonitoringSeriesRoleGroup.Max(Function(r) r.LastUpdated)
                                                                   }).OrderBy(Function(f) f.Role).ToList()
        monitoringSeriesRoleGrid.DataSource = gridElementsGrouped
        monitoringSeriesRoleGrid.DataBind()
    End Sub

    Private Function LoadMonitoringSeriesRole(Role As String) As IList(Of MonitoringSeriesRoleModel)
        Dim elements As ICollection(Of WebAPIDto(Of MonitoringSeriesRoleModel)) = WebAPIImpersonatorFacade.ImpersonateFinder(New MonitoringSeriesRoleFinder(DocSuiteContext.Current.CurrentTenant),
                    Function(impersonationType, finder)
                        finder.ResetDecoration()
                        finder.DateFrom = New DateTimeOffset(dtpDateFrom.SelectedDate.Value)
                        finder.DateTo = New DateTimeOffset(dtpDateTo.SelectedDate.Value).AddDays(1)
                        finder.EnablePaging = False
                        finder.EnableTopOdata = False
                        Return finder.DoSearchHeader()
                    End Function)

        Dim detailTableViewElements As List(Of MonitoringSeriesRoleModel) = elements.Select(Function(x) x.Entity).Where(Function(x) x.Role = Role).OrderBy(Function(f) f.DocumentSeries).ToList()
        Return detailTableViewElements
    End Function
#End Region

End Class