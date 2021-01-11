Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class DocmRisultati
    Inherits DocmBasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Initialize()
        InitializeAjaxSettings()
        InitializeGridBar()

        'Esegue la ricerca
        If Not String.IsNullOrEmpty(Request.QueryString("ProtYear")) And _
            Not String.IsNullOrEmpty(Request.QueryString("ProtNumber")) Then
            Me.uscDocumentGrid.ProtocolYear = Short.Parse(Request.QueryString("ProtYear"))
            Me.uscDocumentGrid.ProtocolNumber = Integer.Parse(Request.QueryString("ProtNumber"))
        End If

        If Not IsNothing(Request.QueryString("ReslId")) And _
            Request.QueryString("ReslId") <> "" Then
            Me.uscDocumentGrid.ResolutionId = Short.Parse(Request.QueryString("ReslId"))
        End If

        If Not Me.IsPostBack Then
            DoSearch()
        End If
    End Sub

    Private Sub Initialize()
        Me.MasterDocSuite.TitleVisible = False
        AddHandler uscDocumentGrid.Grid.DataBound, AddressOf DataSourceChanged
    End Sub

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(uscDocumentGrid.Grid, uscDocumentGrid.Grid, MasterDocSuite.AjaxLoadingPanelSearch)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscDocumentGrid.Grid, lblHeader)
    End Sub

    Private Sub InitializeGridBar()
        uscDocumentGrid.DisableColumn(uscDocmGrid.COLUMN_FOLDER_NAME)
        uscDocumentGrid.DisableColumn(uscDocmGrid.COLUMN_FOLDER_EXPIRYDATE)
        uscDocumentGrid.DisableColumn(uscDocmGrid.COLUMN_FOLDER_EXPIRY_DESCRIPTION)
        uscDocumentGridBar.Grid = uscDocumentGrid.Grid
        uscDocumentGridBar.AjaxEnabled = True
        uscDocumentGridBar.AjaxLoadingPanel = MasterDocSuite.AjaxLoadingPanelSearch
    End Sub

    Protected Sub DataSourceChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim gvDocuments As BindGrid = uscDocumentGrid.Grid
        If gvDocuments.DataSource IsNot Nothing Then
            lblHeader.Text = String.Concat("Pratiche - Risultati (", gvDocuments.DataSource.Count, "/", gvDocuments.VirtualItemCount, ")")
        Else
            lblHeader.Text = "Pratiche - Nessun Risultato"
        End If
        MasterDocSuite.HistoryTitle = lblHeader.Text
    End Sub

    Private Sub DoSearch()
        If uscDocumentGrid.Grid.Finder Is Nothing Then
            Dim finder As NHibernateDocumentFinder = CType(SessionSearchController.LoadSessionFinder(SessionSearchController.SessionFinderType.DocmFinderType), NHibernateDocumentFinder)

            uscDocumentGrid.Grid.Finder = finder
        End If
        
        uscDocumentGrid.Grid.DataBindFinder()
        uscDocumentGrid.Grid.Visible = True

        ' Imposta colonna nome documento
        uscDocumentGrid.ColumnDocDescriptionVisible = DirectCast(uscDocumentGrid.Grid.Finder, NHibernateDocumentFinder).UsedDocumentObject

        'visualizza barra pulsanti
        uscDocumentGridBar.Show()
    End Sub

End Class