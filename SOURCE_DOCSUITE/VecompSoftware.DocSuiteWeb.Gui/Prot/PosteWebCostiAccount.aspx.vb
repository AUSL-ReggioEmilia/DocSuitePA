Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Report
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Report
Imports VecompSoftware.Services.Biblos.Models

Namespace Prot.PosteWeb
    Public Class CostiAccount
        Inherits ProtBasePage

#Region " Events "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            InitializeAjax()
            If Not IsPostBack Then
                Title = "Costi PosteWeb"
                ' Pulsanti esportazione
                btnExcel.Icon.SecondaryIconUrl = ImagePath.SmallExcel
                btnExcel.CommandName = "Export"
                btnExcel.CommandArgument = "Excel"
                btnExcel.ToolTip = "Esporta in Excel"
                btnExcel.CausesValidation = False
            End If
            BindGrid()
        End Sub

        Protected Sub BtnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
            'BindGrid()
        End Sub

        Protected Sub BtnExcel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnExcel.Click
            If ReportFacade.TenantModel Is Nothing Then
                ReportFacade.TenantModel = DocSuiteContext.Current.CurrentTenant
            End If
            Dim report As IReport(Of PolDtoCosti) = ReportFacade.GenerateReport(Of PolDtoCosti)(CType(dgPosteRequestContact.DataSource, IList(Of PolDtoCosti)), (From column As GridColumn In dgPosteRequestContact.Columns Select New Column(column)).ToList())
            Dim doc As DocumentInfo = report.ExportExcel("Costi_PosteOnLine.xls")

            Dim file As FileInfo = BiblosFacade.SaveUniqueToTemp(doc)
            Dim script As String = String.Concat("window.open('", DocSuiteContext.Current.CurrentTenant.DSWUrl, "/Temp/", file.Name & "');")
            AjaxManager.ResponseScripts.Add(script)
        End Sub
#End Region

#Region " Methods "

        Private Sub InitializeAjax()
            AjaxManager.AjaxSettings.AddAjaxSetting(dgPosteRequestContact, dgPosteRequestContact, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnExcel, dgPosteRequestContact, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnRefresh, dgPosteRequestContact, MasterDocSuite.AjaxDefaultLoadingPanel)
        End Sub

        Private Sub BindGrid()
            dgPosteRequestContact.DataSource = Facade.PosteOnLineRequestFacade.CostiAccount(dtpRegistrationDateFrom.SelectedDate, dtpRegistrationDateTo.SelectedDate)
            dgPosteRequestContact.DataBind()
        End Sub

#End Region

    End Class
End Namespace