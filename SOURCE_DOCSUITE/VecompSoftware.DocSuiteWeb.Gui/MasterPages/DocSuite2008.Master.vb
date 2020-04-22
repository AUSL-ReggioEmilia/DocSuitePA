Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Linq
Imports System.Reflection
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Extensions
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.UDS
Imports VecompSoftware.DocSuiteWeb.Model.WebAPI.Client
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Logging

Partial Public Class DocSuite2008
    Inherits MasterPage


#Region " Fields "

    Private Shared _webAPIModels As String
#End Region

#Region " Properties "
    Public Event OnWorkflowConfirmed As EventHandler

    Public ReadOnly Property CompleteWorkflowActivityButton As RadButton
        Get
            Return btnCompleteWorkflow
        End Get
    End Property

    Public ReadOnly Property ScriptManager As RadScriptManager
        Get
            Return scmMasterPage
        End Get
    End Property

    Public ReadOnly Property AjaxManager() As RadAjaxManager
        Get
            Return masterAjaxManager
        End Get
    End Property

    Public ReadOnly Property DefaultWindowManager As RadWindowManager
        Get
            Return alertManager
        End Get
    End Property


    Public ReadOnly Property AjaxDefaultLoadingPanel() As RadAjaxLoadingPanel
        Get
            Return DefaultLoadingPanel
        End Get
    End Property


    Public ReadOnly Property AjaxFlatLoadingPanel() As RadAjaxLoadingPanel
        Get
            Return FlatLoadingPanel
        End Get
    End Property

    <Obsolete("Loading Panel dismesso, utilizzare AjaxDefaultLoadingPanel")>
    Public ReadOnly Property AjaxLoadingPanel() As RadAjaxLoadingPanel
        Get
            Return LoadingPanel
        End Get
    End Property

    <Obsolete("Loading Panel dismesso, utilizzare AjaxDefaultLoadingPanel")>
    Public ReadOnly Property AjaxLoadingPanelSearch() As RadAjaxLoadingPanel
        Get
            Return LoadingPanelSearch
        End Get
    End Property

    <Obsolete("Loading Panel dismesso, utilizzare AjaxDefaultLoadingPanel")>
    Public ReadOnly Property AjaxLoadingPanelGrid() As RadAjaxLoadingPanel
        Get
            Return LoadingPanelGrid
        End Get
    End Property

    Public Property HistoryTitle() As String
        Get
            Return CType(ViewState("_historyTitle"), String)
        End Get
        Set(ByVal value As String)
            ViewState("_historyTitle") = value
        End Set
    End Property

    Public Property Title() As String
        Get
            Return lblHeader.Text
        End Get
        Set(ByVal value As String)
            lblHeader.Text = value
        End Set
    End Property

    Public Property TitleVisible() As Boolean
        Get
            Return divTitolo.Visible
        End Get
        Set(ByVal value As Boolean)
            divTitolo.Visible = value
        End Set
    End Property

    Public Property TitleContainer As HtmlGenericControl
        Get
            Return divTitolo
        End Get
        Set(value As HtmlGenericControl)
            divTitolo = value
        End Set
    End Property

    Public ReadOnly Property Header() As ContentPlaceHolder
        Get
            Return cphHeader
        End Get
    End Property

    Public ReadOnly Property Content() As ContentPlaceHolder
        Get
            Return cphContent
        End Get
    End Property

    Public ReadOnly Property Footer() As ContentPlaceHolder
        Get
            Return cphFooter
        End Get
    End Property

    <Obsolete("Evitare se possibile.")>
    Public ReadOnly Property HtmlBody() As HtmlGenericControl
        Get
            Return body
        End Get
    End Property

    Public ReadOnly Property WorkflowWizardControl As RadWizard
        Get
            Return workflowWizard
        End Get
    End Property

    Public ReadOnly Property WorkflowWizardRow As LayoutRow
        Get
            Return rowWizard
        End Get
    End Property

    Public ReadOnly Property WizardActionColumn As LayoutColumn
        Get
            Return colWizardAction
        End Get
    End Property

    Protected ReadOnly Property DSWVersion As String
        Get
            Return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion
        End Get
    End Property

    Protected Shared ReadOnly Property WebAPIModels As String
        Get
            If _webAPIModels Is Nothing Then
                Dim webApiServiceModel As ICollection(Of WebAPIServiceDto) = DocSuiteContext.Current.CurrentTenant.GetServiceDto()
                Dim udsRepositories As IList(Of UDSRepository) = New UDSRepositoryFacade(DocSuiteContext.Current.User.FullUserName).GetActiveRepositories(String.Empty)
                Dim address As IBaseAddress = DocSuiteContext.Current.CurrentTenant.WebApiClientConfig.Addresses.Single(Function(s) s.AddressName.Eq("API-UDSAddress"))
                Dim baseUrl As String = address.Address.AbsoluteUri

                If Not baseUrl.EndsWith("/") Then
                    baseUrl = String.Concat(baseUrl, "/")
                End If
                Dim url As String = baseUrl
                Dim controllerName As String
                For Each udsRepository As UDSRepository In udsRepositories
                    controllerName = Helpers.UDS.Utils.GetWebAPIControllerName(udsRepository.Name)
                    FileLogger.Debug(LogName.FileLog, $"setting WebAPIModels {controllerName} udsRepository")
                    url = String.Concat(baseUrl, If(Char.IsDigit(controllerName.ElementAt(0)), $"_{controllerName}", controllerName))
                    webApiServiceModel.Add(New WebAPIServiceDto() With {
                                           .Name = udsRepository.Name,
                                           .ODATAUrl = url,
                                           .WebAPIUrl = url})
                Next
                _webAPIModels = JsonConvert.SerializeObject(webApiServiceModel)
            End If
            Return _webAPIModels
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        Title = Page.Title
        Dim guiVersion As String = Assembly.GetExecutingAssembly().GetName().Version.ToString(4)
        Dim scriptUrl As String = String.Concat("?v=", guiVersion)
        scmMasterPage.Scripts.Add(New ScriptReference(String.Concat("~/js/console.js", scriptUrl)))
        scmMasterPage.Scripts.Add(New ScriptReference(String.Concat("~/js/common.js", scriptUrl)))
        ' Imposto i valori della generic window
        alertManager.Width = DocSuiteContext.Current.ProtocolEnv.ModalWidth
        alertManager.Height = DocSuiteContext.Current.ProtocolEnv.ModalHeight
        alertManager.MinWidth = DocSuiteContext.Current.ProtocolEnv.ModalWidth
        alertManager.MinHeight = DocSuiteContext.Current.ProtocolEnv.ModalHeight

        alertManager.Behaviors = WindowBehaviors.Close
        alertManager.Modal = True
        alertManager.DestroyOnClose = True
        alertManager.VisibleStatusbar = False
        alertManager.CenterIfModal = True

        'Imposto etichetta History di default (titolo pagina)
        If Not String.IsNullOrEmpty(HistoryTitle) Then
            Page.Title = HistoryTitle
        End If

        'se nessun controllo nel content
        If cphContent.Controls.Count = 0 Then
            cphContent.Visible = False
        End If

        'se nessun controllo nel footer
        If cphFooter.Controls.Count = 0 Then
            cphFooter.Visible = False
        End If

        'per default utilizzato la classe Prot
        If Not String.IsNullOrEmpty(Request.QueryString("Type")) Then
            body.Attributes("class") = Request.QueryString("Type").ToLower()
        Else
            body.Attributes("class") = ProtBasePage.DefaultType.ToLower()
        End If
    End Sub

    Protected Sub RadScriptManager_PostBackError(sender As Object, e As AsyncPostBackErrorEventArgs)
        If (e.Exception IsNot Nothing) Then
            FileLogger.Error(LogName.FileLog, e.Exception.Message, e.Exception)
        End If

    End Sub

    Private Sub BtnExportClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnExport.Click
        Dim cookieExportGrid As HttpCookie = Request.Cookies("ExportGridID")
        Dim cookieExportType As HttpCookie = Request.Cookies("ExportGridType")
        Dim grid As BindGrid
        If cookieExportGrid IsNot Nothing Then
            grid = CType(Page.FindControl(cookieExportGrid.Value), BindGrid)
            If grid IsNot Nothing AndAlso cookieExportType IsNot Nothing Then
                Dim exportType As String = cookieExportType.Value
                Request.Cookies.Remove("ExportGridID")
                Request.Cookies.Remove("ExportGridType")
                Select Case exportType
                    Case "Excel"
                        grid.MasterTableView.ExportToExcel()
                    Case "Word"
                        grid.MasterTableView.ExportToWord()
                    Case "Pdf"
                        grid.MasterTableView.ExportToPdf()
                End Select
            End If
        End If
    End Sub

    Protected Sub NotificationWorkflowBtnClick(sender As Object, e As EventArgs) Handles btnCompleteWorkflow.Click
        RaiseEvent OnWorkflowConfirmed(Me, e)
    End Sub

#End Region

#Region "Methods"
    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnCompleteWorkflow, btnCompleteWorkflow, AjaxFlatLoadingPanel)
    End Sub
#End Region

End Class