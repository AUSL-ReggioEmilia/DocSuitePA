Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Fascicles
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class UserUDFascicle
    Inherits UserBasePage

#Region " Fields "

    Dim _titolo As String = String.Empty
    Dim _actionType As String = String.Empty

#End Region

#Region " Properties "

    Protected ReadOnly Property ActionType As String
        Get
            If String.IsNullOrEmpty(_actionType) Then
                _actionType = Me.Request.QueryString.GetValueOrDefault("Action", String.Empty)
            End If
            Return _actionType
        End Get
    End Property


#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        _titolo = Request.QueryString("Title")
        Title = String.Format("Fascicoli - {0}", _titolo)
        AddHandler uscUDFascicleGrid.Grid.DataBound, AddressOf DataSourceChanged
        InitializeAjaxSettings()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        LoadDocumentUnits()
    End Sub

    Protected Sub UserFascicleAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        If e.Argument.Eq("loadDocumentUnits") Then
            LoadDocumentUnits()
        End If
    End Sub
#End Region

#Region " Methods "
    Public Sub Initialize()
        MasterDocSuite.Title = Title

        rdpDateFrom.SelectedDate = Date.Today.AddDays(-30)
        rdpDateTo.SelectedDate = Date.Today

        uscUDFascicleGrid.Grid.DiscardFinder()
    End Sub

    Private Sub LoadDocumentUnits()
        Dim finder As DocumentUnitModelFinder = New DocumentUnitModelFinder(DocSuiteContext.Current.CurrentTenant)
        finder.PageSize = 50
        finder.DocumentUnitFinderAction = DocumentUnitFinderActionType.FascicolableUD
        finder.UserName = DocSuiteContext.Current.User.UserName
        finder.Domain = DocSuiteContext.Current.User.Domain
        finder.DateFrom = New DateTimeOffset(rdpDateFrom.SelectedDate.Value)
        finder.DateTo = New DateTimeOffset(rdpDateTo.SelectedDate.Value).AddDays(1)
        finder.IsSecurityUserEnabled = True
        finder.IncludeThreshold = chkRedThreshold.Checked
        finder.ExcludeLinked = chkExcludeLinked.Checked
        finder.FascicolableThresholdDate = Convert.ToDateTime(DocSuiteContext.Current.ProtocolEnv.FascicolableThresholdDate)
        finder.IdTenantAOO = CurrentTenant.TenantAOO.UniqueId
        finder.SortExpressions.Add("Entity.RegistrationDate", "ASC")
        finder.SortExpressions.Add("Entity.Number", "ASC")
        uscUDFascicleGrid.Grid.Finder = finder
        uscUDFascicleGrid.Grid.SetImpersonationAction(AddressOf ImpersonateGridCallback)
        uscUDFascicleGrid.Grid.SetImpersonationCounterAction(AddressOf ImpersonateGridCallback)
        uscUDFascicleGrid.Grid.DataBindFinder()

        If uscUDFascicleGrid.Grid.DataSource IsNot Nothing Then
            MasterDocSuite.Title = String.Format("Fascicoli - {0} ({1}/{2})", _titolo, uscUDFascicleGrid.Grid.DataSource.Count, uscUDFascicleGrid.Grid.VirtualItemCount)
        Else
            MasterDocSuite.Title = "Fascicoli - Nessun risultato"
        End If
    End Sub

    Private Function ImpersonateGridCallback(Of TResult)(finder As IFinder, callback As Func(Of TResult)) As TResult
        Return WebAPIImpersonatorFacade.ImpersonateFinder(Of DocumentUnitModelFinder, TResult)(finder,
                        Function(impersonationType, wfinder)
                            Return callback()
                        End Function)
    End Function

    Protected Sub DataSourceChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim FascicleGrid As BindGrid = uscUDFascicleGrid.Grid
        If FascicleGrid.DataSource IsNot Nothing Then
            Title = String.Format("Fascicoli - {0} ({1}/{2})", _titolo, FascicleGrid.DataSource.Count, FascicleGrid.VirtualItemCount)
        Else
            Title = "Fascicoli - Nessun risultato"
        End If
        MasterDocSuite.Title = Title
    End Sub

    Private Sub InitializeAjaxSettings()
        AddHandler AjaxManager.AjaxRequest, AddressOf UserFascicleAjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscUDFascicleGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, MasterDocSuite.TitleContainer)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnUpdate, MasterDocSuite.TitleContainer)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscUDFascicleGrid, MasterDocSuite.TitleContainer)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscUDFascicleGrid.Grid, MasterDocSuite.TitleContainer)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscUDFascicleGrid.Grid, uscUDFascicleGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnUpdate, uscUDFascicleGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub
#End Region

End Class
