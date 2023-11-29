Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data

Partial Class ReslRicerca
    Inherits ReslBasePage

#Region " Fields "

    Private _finderController As BaseResolutionFinderController
#End Region

#Region " Properties "

    Protected Property FinderController() As BaseResolutionFinderController
        Get
            Return _finderController
        End Get
        Set(ByVal value As BaseResolutionFinderController)
            _finderController = value
        End Set
    End Property

#End Region

#Region " Events "

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Dim manager As ScriptManager = ScriptManager.GetCurrent(Page)
        manager.EnableHistory = True
    End Sub

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        If Action.Eq("CopyReslDocuments") Then
            MasterDocSuite.TitleVisible = False
        End If
        Title = Facade.TabMasterFacade.TreeViewCaption & " - Ricerca"
        btnSearch.Focus()
        FinderController = ControllerFactory.CreateResolutionFinderController(uscResolutionFinder)
        FinderController.BindControls()
        If Not Page.IsPostBack Then
            FinderController.Initialize()
            uscResolutionFinder.EnableDateFromValue = False
            uscResolutionFinder.EnableDateToValue = False
            uscResolutionFinder.ActiveStepSelected = ResolutionEnv.ActiveStepFilterSelectedEnabled
            Page.Form.DefaultButton = btnSearch.UniqueID
            btnSearch.Focus()
        End If

    End Sub

    Private Sub Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearch.Click
        Dim finder As NHibernateResolutionFinder = FinderController.LoadFinder()
        If CommonInstance.ApplyResolutionFinderSecurity(Me, finder, CurrentTenant.TenantAOO.UniqueId) AndAlso ResolutionEnv.SearchMaxRecords <> 0 Then
            finder.PageSize = ResolutionEnv.SearchMaxRecords
        End If
        SessionSearchController.SaveSessionFinder(finder, SessionSearchController.SessionFinderType.ReslFinderType)
        ClearSessions(Of ReslRisultati)()
        Response.Redirect("../Resl/ReslRisultati.aspx?Type=Resl&Action=" & Action)
    End Sub
#End Region

End Class
