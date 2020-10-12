Imports System.Collections.Generic
Imports System.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Extensions

Partial Public Class ProtMDRisultati
    Inherits ProtBasePage

    Private Property SelectedDateFrom As Date?
        Get
            If Session("DateFrom") IsNot Nothing Then
                Return Convert.ToDateTime(Session("DateFrom"))
            End If
            Return Nothing
        End Get
        Set(value As Date?)
            Session("DateFrom") = value
        End Set
    End Property

    Private Property SelectedDateTo As Date?
        Get
            If Session("DateTo") IsNot Nothing Then
                Return Convert.ToDateTime(Session("DateTo"))
            End If
            Return Nothing
        End Get
        Set(value As Date?)
            Session("DateTo") = value
        End Set
    End Property


#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjaxSettings()
        If Not Page.IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        LoadProtocols()
    End Sub

    Protected Sub ProtMDRisultatiAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)

    End Sub

    Private Sub ImpersonationFinderDelegate(ByVal source As Object, ByVal e As EventArgs)
        uscProtocolGrid.Grid.SetImpersonationAction(AddressOf ImpersonateGridCallback)
        uscProtocolGrid.Grid.SetImpersonationCounterAction(AddressOf ImpersonateGridCallback)
    End Sub
#End Region

#Region "Methods"

    Public Sub Initialize()
        MasterDocSuite.Title = Title

        rdpDateFrom.SelectedDate = Date.Today.AddDays(-7)
        rdpDateTo.SelectedDate = Date.Today
        If SelectedDateFrom.HasValue Then
            rdpDateFrom.SelectedDate = SelectedDateFrom.Value
        End If

        If SelectedDateTo.HasValue Then
            rdpDateTo.SelectedDate = SelectedDateTo.Value
        End If
        Title = "I miei documenti autorizzati"

        uscProtocolGrid.Grid.DiscardFinder()
    End Sub

    Private Sub LoadProtocols()
        Dim finder As DocumentUnitModelFinder = New DocumentUnitModelFinder(DocSuiteContext.Current.Tenants.GetActualTenants(Of Entity.DocumentUnits.DocumentUnit).ToList())
        finder.PageSize = 50
        finder.DocumentUnitFinderAction = DocumentUnitFinderActionType.AuthorizedUD
        finder.DateFrom = New DateTimeOffset(rdpDateFrom.SelectedDate.Value)
        finder.DateTo = New DateTimeOffset(rdpDateTo.SelectedDate.Value).AddDays(1)
        finder.IdTenantAOO = CurrentTenant.TenantAOO.UniqueId
        finder.SortExpressions.Add("Entity.Year", "DESC")
        finder.SortExpressions.Add("Entity.Number", "DESC")
        uscProtocolGrid.Grid.Finder = finder
        uscProtocolGrid.Grid.DataBindFinder()

        SelectedDateFrom = rdpDateFrom.SelectedDate.Value
        SelectedDateTo = rdpDateTo.SelectedDate.Value

        If uscProtocolGrid.Grid.DataSource IsNot Nothing Then
            MasterDocSuite.Title = String.Format("I miei documenti autorizzati ({0}/{1})", uscProtocolGrid.Grid.DataSource.Count, uscProtocolGrid.Grid.VirtualItemCount)
        Else
            MasterDocSuite.Title = "I miei documenti autorizzati - Nessun Risultato"
        End If
    End Sub

    Private Sub InitializeAjaxSettings()
        AddHandler AjaxManager.AjaxRequest, AddressOf ProtMDRisultatiAjaxRequest
        AddHandler uscProtocolGrid.Grid.NeedImpersonation, AddressOf ImpersonationFinderDelegate
        AjaxManager.AjaxSettings.AddAjaxSetting(btnUpdate, pnlHeader, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnUpdate, MasterDocSuite.TitleContainer)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscProtocolGrid.Grid, MasterDocSuite.TitleContainer)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscProtocolGrid.Grid, pnlHeader, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscProtocolGrid.Grid, uscProtocolGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnUpdate, uscProtocolGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Function ImpersonateGridCallback(Of TResult)(finder As IFinder, callback As Func(Of TResult)) As TResult
        Return WebAPIImpersonatorFacade.ImpersonateFinder(Of DocumentUnitModelFinder, TResult)(finder,
                        Function(impersonationType, wfinder)
                            Return callback()
                        End Function)
    End Function
#End Region

End Class