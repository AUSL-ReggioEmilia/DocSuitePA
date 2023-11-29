Imports System.Collections.Generic
Imports Microsoft.VisualBasic.CompilerServices
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI

Public Class ProtGestioneFatturePA
    Inherits ProtBasePage

#Region "Fields"
    Private _title As String
#End Region

#Region "Properties"
    Private Const DefaultDayToAdd As Double = -30

    Public Const ConsegnateSDI As String = "CSDI"
    Public Const Rifiutate As String = "REFUSED"
    Public Const DaInviare As String = "TOSEND"
    Public Const Consegnate As String = "DELIVERED"


    Public ReadOnly Property PageTitle() As String
        Get
            If String.IsNullOrEmpty(_title) Then
                _title = Request.QueryString("Title")
            End If
            Return _title
        End Get
    End Property

#End Region

#Region "Event"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub InvoicePA_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arg As String = e.Argument
        Select Case arg
            Case "loadData"
                LoadFinder()
            Case Else
                Throw New ArgumentException()
        End Select
    End Sub
#End Region

#Region "Methods"
    Private Sub Initialize()
        Page.Title = String.Format("Gestione Fatture PA - {0}", PageTitle)

        filterTable.Visible = False
        Select Case Action
            Case Consegnate
                filterTable.Visible = True
                dateFrom.SelectedDate = Today.AddDays(DefaultDayToAdd)
                dateTo.SelectedDate = Today
        End Select
    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscProtocolGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscProtocolGrid.Grid, uscProtocolGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnUpdateGrid, uscProtocolGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AddHandler AjaxManager.AjaxRequest, AddressOf InvoicePA_AjaxRequest
    End Sub

    Private Sub LoadFinder()
        Dim finder As NHibernateProtocolFinder
        Select Case Action
            Case ConsegnateSDI
                Dim status As New List(Of Integer)
                status.Add(ProtocolStatusId.PAInvoiceSent)
                finder = Facade.ProtocolFacade.GetInvoicePaProtocolsFinder(status)
            Case Consegnate
                Dim status As New List(Of Integer)
                status.Add(ProtocolStatusId.PAInvoiceAccepted)
                finder = Facade.ProtocolFacade.GetInvoicePaProtocolsFinder(status, dateFrom.SelectedDate, dateTo.SelectedDate)
            Case Rifiutate
                Dim status As New List(Of Integer)
                status.Add(ProtocolStatusId.PAInvoiceRefused)
                status.Add(ProtocolStatusId.PAInvoiceSdiRefused)
                finder = Facade.ProtocolFacade.GetInvoicePaProtocolsFinder(status)
            Case DaInviare
                Dim status As New List(Of Integer)
                status.Add(ProtocolStatusId.Attivo)
                status.Add(ProtocolStatusId.PAInvoiceRefused)
                status.Add(ProtocolStatusId.PAInvoiceSdiRefused)
                finder = Facade.ProtocolFacade.GetInvoicePaProtocolsFinder(status)
            Case Else
                Throw New Exception("Tipologia di ricerca non riconosciuta.")
        End Select

        CommonInstance.ApplyProtocolFinderSecurity(finder, SecurityType.Read, CurrentTenant.TenantAOO.UniqueId, True)

        uscProtocolGrid.Grid.PageSize = finder.PageSize
        uscProtocolGrid.Grid.MasterTableView.SortExpressions.Clear()
        uscProtocolGrid.Grid.Finder = finder
        uscProtocolGrid.Grid.DataBindFinder()
        uscProtocolGrid.Grid.Visible = True
        uscProtocolGrid.Grid.MasterTableView.AllowMultiColumnSorting = False
    End Sub
#End Region

End Class