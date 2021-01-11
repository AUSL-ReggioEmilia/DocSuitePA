Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Public Class ReslParerGrid
    Inherits CommonBasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Initialize()
        InitializeAjaxSettings()
        If Not IsPostBack Then
            DoSearch()
        End If
    End Sub

    Private Sub Initialize()
        MasterDocSuite.TitleVisible = False
        AddHandler uscReslGrid.Grid.DataBound, AddressOf DataSourceChanged
        uscReslGrid.DisableColumn(uscReslGrid.COLUMN_DOCUMENT_SIGN)
        uscReslGrid.DisableColumn(uscReslGrid.COLUMN_CLIENT_SELECT)

        ' Visualizzo le colonne PARER
        uscReslGrid.ColumnParerDescriptionVisible = True
        uscReslGrid.ColumnParerIconVisible = True


        uscReslGrid.ColumnAttachSelectVisible = False 'Colonna dei documenti allegati
        uscReslGrid.ColumnOCVisible = False
        uscReslGrid.ColumnControllerStatusVisibile = False
    End Sub

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(uscReslGrid.Grid, uscReslGrid.Grid, MasterDocSuite.AjaxLoadingPanelSearch)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscReslGrid.Grid, lblHeader)
    End Sub

    Public Function GetStatusIcon(id As Integer) As String
        Dim resl As Resolution = Facade.ResolutionFacade.GetById(id, False)
        If Facade.ResolutionParerFacade.Exists(resl) Then
            Select Case Facade.ResolutionParerFacade.GetConservationStatus(resl)
                Case ResolutionParerFacade.ResolutionParerConservationStatus.Correct
                    Return "../Comm/images/parer/green.png"
                Case ResolutionParerFacade.ResolutionParerConservationStatus.Warning
                    Return "../Comm/images/parer/yellow.png"
                Case ResolutionParerFacade.ResolutionParerConservationStatus.Error
                    Return "../Comm/images/parer/red.png"
                Case ResolutionParerFacade.ResolutionParerConservationStatus.Undefined
                    Return "../Comm/images/parer/lightgray.png"
                Case ResolutionParerFacade.ResolutionParerConservationStatus.NotNeeded
                    Return "../Comm/images/parer/lightgray.png"
            End Select
        Else
            ' Non soggetto alla conservazione sostitutiva
            Return "../Comm/images/parer/lightgray.png"
        End If
        Return String.Empty
    End Function

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
        Try
            Dim gvResolutions As BindGrid = uscReslGrid.Grid
            Dim finder As NHibernateResolutionFinder = SessionSearchController.LoadSessionFinder(SessionSearchController.SessionFinderType.ResolutionParerFinderType)
            gvResolutions.PageSize = finder.PageSize
            gvResolutions.Finder = finder
            gvResolutions.DataBindFinder()
            gvResolutions.Visible = True

        Catch ex As Exception
            Throw New DocSuiteException("Errore nella ricerca", ex)
        End Try
    End Sub

    Protected Function ParerDetailUrl() As String
        Return ResolveUrl("~/Prot/ProtParerDetail.aspx")
    End Function


End Class