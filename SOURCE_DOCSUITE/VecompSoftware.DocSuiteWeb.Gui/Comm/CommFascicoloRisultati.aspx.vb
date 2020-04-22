Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic

Partial Public Class CommFascicoloRisultati
    Inherits ProtBasePage

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeProtocolGrid()
        If CommonInstance.DocmEnabled Then
            InitializeDocumentGrid()
        End If
        InitializeAjax()
        If Not IsPostBack Then
            DoSearch()
        End If
        MasterDocSuite.TitleVisible = False
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(uscProtocolGrid.Grid, uscProtocolGrid.Grid)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscProtocolGrid.Grid, divTitolo)
        If CommonInstance.DocmEnabled Then
            AjaxManager.AjaxSettings.AddAjaxSetting(uscDocumentGrid.Grid, uscDocumentGrid.Grid)
            AjaxManager.AjaxSettings.AddAjaxSetting(uscDocumentGrid.Grid, divTitolo)
        End If
    End Sub

    Private Sub InitializeProtocolGrid()
        uscProtocolGrid.SetDataField(uscProtGrid.COLUMN_CATEGORY_NAME, "SubCategoryFullDescrition")
        uscProtocolGrid.ColumnProtocolContactVisible = False
        uscProtocolGrid.ColumnProtocolStatusVisible = False
        If ProtocolEnv.SmartClientMultiChain Then
            uscProtocolGridBar.Grid = uscProtocolGrid.Grid
            uscProtocolGridBar.AjaxEnabled = True
            uscProtocolGridBar.Visible = True
            uscProtocolGridBar.ShowDocumentButton()
            uscProtocolGridBar.Show()
            uscProtocolGridBar.AjaxLoadingPanel = MasterDocSuite.AjaxDefaultLoadingPanel
        Else
            uscProtocolGrid.ColumnClientSelectVisible = False
            uscProtocolGridBar.Visible = False
        End If
        AddHandler uscProtocolGrid.Grid.DataBound, AddressOf DataSourceChanged
    End Sub

    Private Sub InitializeDocumentGrid()
        uscDocumentGrid.SetDataFieldColumnSafe(uscDocmGrid.COLUMN_CATEGORY, "SubCategoryFullDescrition")
        uscDocumentGrid.ColumnClientSelectVisible = False
        uscDocumentGrid.DisableColumn(uscDocmGrid.COLUMN_FOLDER_NAME)
        uscDocumentGrid.DisableColumn(uscDocmGrid.COLUMN_FOLDER_EXPIRYDATE)
        uscDocumentGrid.DisableColumn(uscDocmGrid.COLUMN_FOLDER_EXPIRY_DESCRIPTION)
        AddHandler uscDocumentGrid.Grid.DataBound, AddressOf DataSourceChanged
    End Sub

    Protected Sub DataSourceChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim gvProtocols As BindGrid = uscProtocolGrid.Grid
        If gvProtocols.DataSource IsNot Nothing Then
            ' se sto aggiornando i protocolli
            lblProt.Text = String.Format("Protocolli [{0}]", gvProtocols.VirtualItemCount)
        End If

        If CommonInstance.DocmEnabled Then
            lblDocm.Visible = True
            Dim gvDocuments As BindGrid = uscDocumentGrid.Grid
            If gvDocuments.DataSource IsNot Nothing Then
                ' se sto aggiornando le pratiche
                lblDocm.Text = String.Format("Pratiche [{0}]", gvDocuments.VirtualItemCount)
            End If
        Else
            lblDocm.Visible = False
        End If
    End Sub

    Private Sub DoSearch()
        ' ricerca per i protocolli
        Dim countProtocols As Integer = ExecuteProtocolSearch()
        ' ricerca per le pratiche
        Dim countDocuments As Integer? = Nothing
        If CommonInstance.DocmEnabled Then
            countDocuments = ExecuteDocumentSearch()
        End If
        ' imposta pagina
        If countProtocols = 0 AndAlso countDocuments.GetValueOrDefault(0) = 0 Then
            Throw New InformationException("Ricerca fascicoli", "Nessun risultato")
        End If
        rowBottom.Visible = (countDocuments.GetValueOrDefault(0) > 0)
        splitterBar.Visible = (countDocuments.GetValueOrDefault(0) > 0)
        rowTop.Visible = (countProtocols > 0)
    End Sub

    Private Function ExecuteProtocolSearch() As Integer
        Dim protFinder As NHibernateProtocolFinder = DirectCast(SessionSearchController.LoadSessionFinder(SessionSearchController.SessionFinderType.CommProtFascFinderType), NHibernateProtocolFinder)

        Dim grid As BindGrid = uscProtocolGrid.Grid
        grid.PageSize = protFinder.PageSize
        grid.Finder = protFinder
        ' Per la distribuzione ENPACL l'ordinamento di default è la data di registrazione
        If ProtocolEnv.CorporateAcronym.Contains("ENPACL") Then
            grid.MasterTableView.SortExpressions.AddSortExpression("RegistrationDate ASC")
            grid.MasterTableView.AllowMultiColumnSorting = True
        End If
        grid.MasterTableView.SortExpressions.AddSortExpression(GetSortExpression("P"))
        grid.DataBindFinder()
        grid.MasterTableView.AllowMultiColumnSorting = False
        Return grid.VirtualItemCount
    End Function

    Private Function ExecuteDocumentSearch() As Integer
        Dim docmFinder As NHibernateDocumentFinder = DirectCast(SessionSearchController.LoadSessionFinder(SessionSearchController.SessionFinderType.CommDocmFascFinderType), NHibernateDocumentFinder)

        Dim grid As BindGrid = uscDocumentGrid.Grid
        grid.PageSize = docmFinder.PageSize
        grid.Finder = docmFinder
        grid.MasterTableView.SortExpressions.AddSortExpression(GetSortExpression("D"))
        grid.DataBindFinder()

        Return grid.VirtualItemCount
    End Function

    Private Function GetSortExpression(dbType As String) As String
        ' TODO: Accrocchio retrocompatibilità da eliminare
        Dim sortBy As String = Request.QueryString.GetValue(Of String)("SortBy")
        Dim sortDirection As String = Request.QueryString.GetValue(Of String)("SortDirection")
        If String.IsNullOrEmpty(sortBy) OrElse String.IsNullOrEmpty(sortDirection) Then
            Return String.Empty
        End If

        Dim _field As String = Nothing
        Select Case sortBy
            Case "C"
                _field = "Category.Name"

            Case "D"
                Select Case dbType
                    Case "D"
                        _field = "StartDate"
                    Case "P"
                        _field = "RegistrationDate"
                End Select

            Case "N"
                _field = "Id"

        End Select
        Return String.Format("{0} {1}", _field, sortDirection)
    End Function

#End Region

End Class