Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data


Partial Public Class ReslRisultati
    Inherits ReslBasePage

#Region " Fields "

    Private _gridBarController As ResolutionGridBarController

#End Region

#Region " Properties "

    Protected Property GridBarController() As ResolutionGridBarController
        Get
            Return _gridBarController
        End Get
        Set(ByVal value As ResolutionGridBarController)
            _gridBarController = value
        End Set
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Action.Eq("CopyReslDocuments") Then
            MasterDocSuite.TitleVisible = False
        End If
        AddHandler uscReslGrid.Grid.DataBound, AddressOf DataSourceChanged
        uscReslGrid.DisableColumn(uscReslGrid.COLUMN_DOCUMENT_SIGN)
        uscReslGrid.ColumnAttachSelectVisible = False 'Colonna dei documenti allegati
        uscReslGrid.ColumnOcVisible = False

        InitializeAjaxSettings()

        uscReslGridBar.AjaxEnabled = True
        uscReslGridBar.AjaxLoadingPanel = MasterDocSuite.AjaxDefaultLoadingPanel
        GridBarController = New ResolutionGridBarController(uscReslGridBar)
        GridBarController.LoadConfiguration(ResolutionEnv.Configuration)
        GridBarController.BindGrid(uscReslGrid.Grid)

        Select Case Action
            Case "Docm"
                GridBarController.Hide()
            Case "Resl"
                GridBarController.Hide()
            Case "Fasc"
                GridBarController.Hide()
            Case Else
                GridBarController.Show()
        End Select
    End Sub

    Protected Sub ReslRisultatiAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arguments As String() = Split(e.Argument.Replace("~", "'"), "|", 2)
        Select Case arguments(0)
            Case "InitialPageLoad"
                DoSearch()
        End Select
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjaxSettings()
        AjaxManager.EnablePageHeadUpdate = True
        AddHandler AjaxManager.AjaxRequest, AddressOf ReslRisultatiAjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscReslGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, MasterDocSuite.TitleContainer)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscReslGrid.Grid, uscReslGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscReslGrid.Grid, MasterDocSuite.TitleContainer)
    End Sub

    Protected Sub DataSourceChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim gvResolution As BindGrid = uscReslGrid.Grid
        If gvResolution IsNot Nothing Then
            Title = String.Format("{0} - Risultati ({1}/{2})", Facade.TabMasterFacade.TreeViewCaption, gvResolution.DataSource.Count, gvResolution.VirtualItemCount)
        Else
            Title = Facade.TabMasterFacade.TreeViewCaption & " - Nessun Risultato"
        End If
        MasterDocSuite.Title = Title
        MasterDocSuite.HistoryTitle = Title
        AjaxManager.ResponseScripts.Add("if(GetRadWindow()) {GetRadWindow().set_title(""" & Title & """);}")
    End Sub

    Private Sub DoSearch()
        If uscReslGrid.Grid.Finder Is Nothing Then
            uscReslGrid.Grid.Finder = CType(SessionSearchController.LoadSessionFinder(SessionSearchController.SessionFinderType.ReslFinderType), NHibernateResolutionFinder)
        End If
        uscReslGrid.Grid.PageSize = uscReslGrid.Grid.Finder.PageSize
        uscReslGrid.Grid.DataBindFinder()
        uscReslGrid.Grid.Visible = True
    End Sub

#End Region

End Class