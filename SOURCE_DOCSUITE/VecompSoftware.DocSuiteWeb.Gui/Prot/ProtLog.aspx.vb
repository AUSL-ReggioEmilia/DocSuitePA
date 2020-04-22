Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade

Partial Public Class ProtLog
    Inherits ProtBasePage

#Region " Properties "

    Private ReadOnly Property Year() As Short
        Get
            Return Request.QueryString.GetValue(Of Short)("Year")
        End Get
    End Property

    Private ReadOnly Property Number() As Integer
        Get
            Return Request.QueryString.GetValue(Of Integer)("Number")
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Title = "Protocollo - Log " & ProtocolFacade.ProtocolFullNumber(Year, Number)
        InitializeAjaxSettings()
        If Not Page.IsPostBack Then
            Initialize()
        End If

    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(GridProt, GridProt, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub Initialize()
        Dim protLogFinder As NHibernateProtocolLogFinder = Facade.ProtocolLogFinder

        protLogFinder.SystemUser = String.Empty

        '' Verifico se devo mascherare i log non propri
        '' Se non è stato chiesto esplicitamente un utente
        '' e il parametro è attivo
        '' e l'utente corrente non è amministratore
        If String.IsNullOrEmpty(protLogFinder.SystemUser) AndAlso ProtocolEnv.ProtocolLogShowOnlyCurrentIfNotAdmin AndAlso Not CommonUtil.HasGroupAdministratorRight Then
            '' limito la visibilità ai propri log
            protLogFinder.SystemUser = DocSuiteContext.Current.User.FullUserName
        End If

        protLogFinder.ProtocolYear = Year
        protLogFinder.ProtocolNumber = Number
        protLogFinder.PageSize = ProtocolEnv.SearchMaxRecords

        GridProt.Finder = protLogFinder
        GridProt.PageSize = protLogFinder.PageSize
        GridProt.MasterTableView.SortExpressions.AddSortExpression("Id DESC")
        GridProt.DataBindFinder()
    End Sub

#End Region

End Class