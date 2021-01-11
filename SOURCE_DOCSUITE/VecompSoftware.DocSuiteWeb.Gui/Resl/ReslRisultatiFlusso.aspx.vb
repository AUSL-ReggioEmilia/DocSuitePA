Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Gui.ResolutionGridBarController

Partial Public Class ReslRisultatiFlusso
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

    Private ReadOnly Property MyStep As BarWorkflowStep
        Get
            Return DirectCast(Session("CurrentStep"), BarWorkflowStep)
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Inizializzazioni
        Initialize()
        InitializeAjaxSettings()
        InitializeController()
        If Not Me.IsPostBack Then
            DoSearch()
        End If
    End Sub

    Protected Sub ReslRisultatiFlusso_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Select Case e.Argument
            Case "UPDATE"
                DoSearch()
        End Select
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        Me.MasterDocSuite.TitleVisible = False
        AddHandler uscReslGrid.Grid.DataBound, AddressOf DataSourceChanged
        AddHandler AjaxManager.AjaxRequest, AddressOf ReslRisultatiFlusso_AjaxRequest
        uscReslGrid.DisableColumn(uscReslGrid.COLUMN_DOCUMENT_SIGN)
        'Colonna dei documenti allegati
        uscReslGrid.ColumnAttachSelectVisible = ResolutionEnv.AutomaticActivityStepEnabled AndAlso MyStep = BarWorkflowStep.Pubblicazione
        uscReslGrid.ColumnReturnFromCollaborationVisible = ProtocolEnv.CheckResolutionCollaborationOriginEnabled AndAlso MyStep = BarWorkflowStep.DaAffariGenerali
        uscReslGrid.ColumnLastLogVisible = ProtocolEnv.CheckResolutionCollaborationOriginEnabled AndAlso MyStep = BarWorkflowStep.DaAffariGenerali
        CurrentResolutionDocuments = Nothing
    End Sub

    Private Sub InitializeController()
        InitializeGridBar()
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

    Private Sub InitializeGridBar()
        uscReslGridBar.AjaxEnabled = True
        uscReslGridBar.AjaxLoadingPanel = Me.MasterDocSuite.AjaxLoadingPanelSearch
    End Sub

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(uscReslGrid.Grid, uscReslGrid.Grid, MasterDocSuite.AjaxLoadingPanelSearch)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscReslGrid.Grid, MasterDocSuite.AjaxLoadingPanelSearch)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscReslGrid.Grid, lblHeader)
    End Sub

    Protected Sub DataSourceChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim gvResolutions As BindGrid = uscReslGrid.Grid
        If gvResolutions.DataSource IsNot Nothing Then
            lblHeader.Text = Facade.TabMasterFacade.TreeViewCaption & " - Risultati (" & gvResolutions.DataSource.Count & "/" & gvResolutions.VirtualItemCount & ")"
        Else
            lblHeader.Text = Facade.TabMasterFacade.TreeViewCaption & " - Nessun Risultato"
        End If
        MasterDocSuite.HistoryTitle = lblHeader.Text
    End Sub

    Private Sub DoSearch()
        If uscReslGrid.Grid.Finder Is Nothing Then
            Dim finder As NHibernateResolutionFinder = SessionSearchController.LoadSessionFinder(SessionSearchController.SessionFinderType.ReslFlussoFinderType)
            uscReslGrid.Grid.Finder = finder
        End If

        uscReslGrid.Grid.PageSize = uscReslGrid.Grid.Finder.PageSize
        uscReslGrid.Grid.DataBindFinder()
        uscReslGrid.Grid.Visible = True

        Dim bScript As String = ""
        If Session("DocumentToOpen") IsNot Nothing Then
            If Not String.IsNullOrEmpty(Session("DocumentToOpen").ToString()) Then bScript = String.Format("window.open('{0}');", Session("DocumentToOpen"))
            Session.Remove("DocumentToOpen")
        End If

        If uscReslGrid.Grid.VirtualItemCount = 0 Then
            GridBarController.Hide()
        End If
        AjaxManager.ResponseScripts.Add(bScript)
        uscReslGridBar.SetButtonLabel()
        DataGridRegionInBold(uscReslGrid.Grid)
        uscReslGrid.Grid.Visible = True
    End Sub

    Public Sub DataGridRegionInBold(ByVal grid As Telerik.Web.UI.RadGrid)
        For Each item As Telerik.Web.UI.GridDataItem In grid.Items
            Dim chk As CheckBox = DirectCast(item("Regione").Controls(0), CheckBox)
            If chk.Checked Then
                item.Font.Bold = True
            End If
        Next item
    End Sub

#End Region

End Class