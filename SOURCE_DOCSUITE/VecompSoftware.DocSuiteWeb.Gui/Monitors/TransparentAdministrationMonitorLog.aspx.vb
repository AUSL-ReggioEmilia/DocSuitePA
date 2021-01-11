Imports System.Linq
Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.DTO.DocumentSeries
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Monitors

Public Class TransparentAdministrationMonitorLog
    Inherits CommonBasePage
#Region " Fields "

#End Region

#Region " Properties "
    Public ReadOnly Property TransparentAdministrationMonitorLogItems As String
        Get
            Return Request.Form("TransparentAdministrationMonitorlogItems")
        End Get
    End Property

#End Region

#Region " Events "
    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            cmdExportToExcel.Icon.SecondaryIconUrl = ImagePath.SmallExcel
            dtpDateFrom.SelectedDate = Date.Today.AddDays(-30)
            dtpDateTo.SelectedDate = Date.Today
        End If
    End Sub
    Private Sub cmdExportToExcel_Click(sender As Object, e As EventArgs) Handles cmdExportToExcel.Click
        Dim radGrid As RadGrid = CType(uscAmmTraspMonitorLogGrid.FindControl("ammTraspMonitorLogGrid"), RadGrid)
        Dim dataSource As List(Of TransparentAdministrationMonitorLogModel) = JsonConvert.DeserializeObject(Of List(Of TransparentAdministrationMonitorLogModel))(TransparentAdministrationMonitorLogItems, New JsonSerializerSettings() With {
                                                                                                       .NullValueHandling = NullValueHandling.Ignore,
                                                                                                       .TypeNameHandling = TypeNameHandling.Objects,
                                                                                                       .ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                                                                                       .PreserveReferencesHandling = PreserveReferencesHandling.All,
                                                                                                       .DateFormatString = "dd/MM/yyyy"
                                                                                                       })
        Dim results As IEnumerable(Of TransparentAdministrationMonitorLogViewModel) = dataSource.Select(Function(f) New TransparentAdministrationMonitorLogViewModel() _
        With {
                .[Date] = f.Date,
                .DocumentUnitName = f.DocumentUnitName,
                .DocumentUnitSummaryUrl = f.DocumentUnitTitle,
                .DocumentUnitTitle = f.DocumentUnitTitle,
                .IdDocumentUnit = f.IdDocumentUnit,
                .IdRole = f.IdRole,
                .Note = f.Note,
                .Rating = f.Rating,
                .RegistrationDate = f.RegistrationDate,
                .RegistrationUser = f.RegistrationUser,
                .RoleName = f.RoleName,
                .UniqueId = f.UniqueId
        })
        radGrid.DataSource = results
        radGrid.DataBind()
        radGrid.ExportSettings.ExportOnlyData = True
        radGrid.ExportSettings.OpenInNewWindow = True
        radGrid.ExportSettings.FileName = "registro.xls"
        radGrid.MasterTableView.ExportToExcel()
    End Sub

#End Region


#Region " Methods "
    Private Sub InitializeAjax()

    End Sub

#End Region

End Class