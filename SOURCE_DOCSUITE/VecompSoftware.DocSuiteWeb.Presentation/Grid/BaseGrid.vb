Imports System.Web.UI.WebControls
Imports VecompSoftware.Helpers
Imports Telerik.Web.UI
Imports System.Web

''' <summary> Estensione della RadGrid che registra lo script per la chiusura del menu. </summary>
Public Class BaseGrid
    Inherits RadGrid

#Region " Fields "
    Protected Const FILTERID As String = "FILTERID"
    Protected Const IMAGE_COLUMN_WIDTH As Integer = 24

    Private _enableCloseMenuOnClientClick As Boolean = True
    Private _autofitTextBoxFilter As Boolean = True
    Private _enableClearFilterButton As Boolean = True
    Private _enableScrolling As Boolean = True
#End Region

#Region " Properties "

    ''' <summary> Abilita la chiusura automatica del FilterMenu al click del mouse </summary>
    ''' <remarks> True di default </remarks>
    Public Property CloseFilterMenuOnClientClick() As Boolean
        Get
            Return _enableCloseMenuOnClientClick
        End Get
        Set(ByVal value As Boolean)
            If value = True Then
                FilterMenu.OnClientItemClicked = "ItemMenuClickedHandler"
            End If
            _enableCloseMenuOnClientClick = value
        End Set
    End Property

    ''' <summary> Abilita l'autofit sui campi di testo dei filtri </summary>
    ''' <remarks> True di default </remarks>
    Public Property AllowAutofitTextBoxFilter() As Boolean
        Get
            Return _autofitTextBoxFilter
        End Get
        Set(ByVal value As Boolean)
            _autofitTextBoxFilter = value
        End Set
    End Property

    ''' <summary> Numero di pagina attuale della griglia </summary>
    Public Overridable Property CustomPageIndex() As Integer
        Get
            Return CurrentPageIndex
        End Get
        Set(ByVal value As Integer)
            CurrentPageIndex = value
        End Set
    End Property

    ''' <summary> Abilita/Disabilita il pulsante di Pulizia Filtri </summary>
    Public Overridable Property EnableClearFilterButton() As Boolean
        Get
            Return _enableClearFilterButton
        End Get
        Set(ByVal value As Boolean)
            _enableClearFilterButton = value
        End Set
    End Property

    Public ReadOnly Property YetAnotherPageIndex
        Get
            Return CustomPageIndex + 1
        End Get
    End Property

    ''' <summary> Indica l'utente vuole vedere il pannello di raggruppamento </summary>
    Private Property HasShownGroupPanel() As Boolean
        Get
            If ViewState("HasShownGroupPanel") Is Nothing Then
                ViewState("HasShownGroupPanel") = False
            End If
            Return ViewState("HasShownGroupPanel")
        End Get
        Set(value As Boolean)
            ViewState("HasShownGroupPanel") = value
        End Set
    End Property

    Public Overridable Property EnableScrolling As Boolean
        Get
            Return _enableScrolling
        End Get
        Set(value As Boolean)
            _enableScrolling = value
        End Set
    End Property

    Public Overridable Property ImpersonateCurrentUser As Boolean
#End Region

#Region " Constructors "
    Public Sub New()
        MyBase.New()

        _enableCloseMenuOnClientClick = True
        _autofitTextBoxFilter = True
    End Sub
#End Region

#Region " Events "

    Protected Overrides Sub OnInit(ByVal e As EventArgs)
        MyBase.OnInit(e)
        ConfigureFilterMenu()

        ClientSettings.AllowGroupExpandCollapse = True
        ClientSettings.AllowDragToGroup = True

        ConfigureSorting()
        ConfigurePaging()
        ConfigureExport()
        ConfigureStyle()
        ConfigureResizing()
        If EnableScrolling Then ConfiguringScrolling()
    End Sub

    Private Sub ConfiguringScrolling()
        ClientSettings.Scrolling.UseStaticHeaders = True
        ClientSettings.Scrolling.AllowScroll = True
        ClientSettings.Scrolling.ScrollHeight = Unit.Percentage(100)
        ClientSettings.Scrolling.SaveScrollPosition = True
        Width = Unit.Percentage(100)
        Height = Unit.Percentage(100)
        MasterTableView.Width = Unit.Percentage(100)
    End Sub

    ''' <summary> Registra la risorsa javascript per la chiusura automatica del menu </summary>
    Protected Overrides Sub OnPreRender(ByVal e As EventArgs)
        MyBase.OnPreRender(e)

        'intercetta le chiamate ajax. In caso di esportazione annulla la chiamata facendo il postback.
        RadAjaxManager.GetCurrent(Page).ClientEvents.OnRequestStart = "mngRequestStarted"

        Dim dg As RadGrid = CType(Me, RadGrid)
        ' impedisco che le colonne template con immagine vengano sovrapposte
        ' impostando una larghezza minima
        For Each column As GridColumn In dg.Columns
            If (TypeOf (column) Is GridTemplateColumn) Then
                If Not String.IsNullOrEmpty(column.HeaderImageUrl) Then
                    column.HeaderStyle.Width = Unit.Pixel(IMAGE_COLUMN_WIDTH)
                    column.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
                    column.ItemStyle.HorizontalAlign = HorizontalAlign.Center
                End If
            ElseIf (TypeOf (column) Is GridClientSelectColumn) Then
                column.HeaderStyle.Width = Unit.Pixel(IMAGE_COLUMN_WIDTH)
                column.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
                column.ItemStyle.HorizontalAlign = HorizontalAlign.Center
            End If
        Next
    End Sub

    Protected Overrides Sub OnItemCreated(ByVal e As GridItemEventArgs)
        MyBase.OnItemCreated(e)

        If TypeOf e.Item Is GridFilteringItem Then
            CreateFilterControls(e)
        End If
    End Sub

    Private Sub BindGrid_DataBound(ByVal sender As Object, ByVal e As EventArgs) Handles Me.DataBound
        ShowHeader = True
        ShowFooter = False
        ' Mostra il pannello raggruppamento solo se ci sono elementi
        If ShowGroupPanel Then
            HasShownGroupPanel = True
        End If
        ShowGroupPanel = HasShownGroupPanel AndAlso (MasterTableView.Items.Count > 0)
    End Sub

#End Region

#Region " Methods "

    Private Sub ConfigureFilterMenu()
        CloseFilterMenuOnClientClick = True
        ReduceFilterMenu()
        LocalizeFilterMenu()
    End Sub

    Private Sub ConfigureExport()
        ExportSettings.Excel.Format = GridExcelExportFormat.ExcelML
        ExportSettings.Pdf.AllowPrinting = True
        ExportSettings.Pdf.FontType = Telerik.Web.Apoc.Render.Pdf.FontType.Subset
        ExportSettings.Pdf.PaperSize = GridPaperSize.A4
        ExportSettings.FileName = HttpUtility.UrlEncode("Esportazione")
        ExportSettings.ExportOnlyData = False
    End Sub

    Private Sub ConfigureSorting()
        SortingSettings.SortedAscToolTip = "Ordine Crescente"
        SortingSettings.SortedDescToolTip = "Ordine Decrescente"
        SortingSettings.SortToolTip = "Ordina"
        MasterTableView.AllowMultiColumnSorting = False
        MasterTableView.AllowCustomSorting = True
        AllowSorting = True
    End Sub

    Private Sub ConfigurePaging()
        MasterTableView.PagerStyle.Visible = False
        MasterTableView.PagerStyle.Position = GridPagerPosition.Top
        AllowPaging = True
        MasterTableView.AllowCustomPaging = True
        MasterTableView.CommandItemTemplate = New DSCommandItemTemplate(True, True, EnableClearFilterButton, Me)
        MasterTableView.CommandItemDisplay = GridCommandItemDisplay.Top
    End Sub

    Private Sub ConfigureStyle()
        MasterTableView.ItemStyle.CssClass = "Scuro"
        MasterTableView.ItemStyle.Font.Name = "Verdana"
        MasterTableView.AlternatingItemStyle.CssClass = "Chiaro"
        MasterTableView.AlternatingItemStyle.Font.Name = "Verdana"
    End Sub

    Private Sub ConfigureResizing()
        ClientSettings.Resizing.AllowColumnResize = True
        ClientSettings.Resizing.EnableRealTimeResize = False
        ClientSettings.Resizing.ClipCellContentOnResize = False
        ClientSettings.Resizing.ResizeGridOnColumnResize = True
        ClientSettings.ClientMessages.DragToResize = "Ridimensiona"
    End Sub

    Private Sub ReduceFilterMenu()
        Dim menu As GridFilterMenu = FilterMenu
        Dim i As Integer = 0
        While i < menu.Items.Count
            If menu.Items(i).Text = "NoFilter" OrElse _
               menu.Items(i).Text = "Contains" OrElse _
               menu.Items(i).Text = "EqualTo" OrElse _
               menu.Items(i).Text = "GreaterThan" OrElse _
               menu.Items(i).Text = "GreaterThanOrEqualTo" OrElse _
               menu.Items(i).Text = "LessThan" OrElse _
               menu.Items(i).Text = "LessThanOrEqualTo" Then
                i = i + 1
            Else
                menu.Items.RemoveAt(i)
            End If
        End While
    End Sub

    Private Sub LocalizeFilterMenu()
        Dim menu As GridFilterMenu = FilterMenu
        Dim item As RadMenuItem
        For Each item In menu.Items
            item.Attributes(FILTERID) = item.Text
            Select Case item.Text
                Case "NoFilter"
                    item.Text = "Nessun Filtro"
                Case "Contains"
                    item.Text = "Contiene"
                Case "EqualTo"
                    item.Text = "Uguale a"
                Case "GreaterThan"
                    item.Text = "Maggiore di"
                Case "GreaterThanOrEqualTo"
                    item.Text = "Maggiore e uguale a"
                Case "LessThan"
                    item.Text = "Minore di"
                Case "LessThanOrEqualTo"
                    item.Text = "Minore e uguale a"
            End Select
        Next
    End Sub

    Protected Sub CreateFilterControls(ByVal e As GridItemEventArgs)
        'controllo se autofit dei filtri è impostato
        If Not _autofitTextBoxFilter Then
            Exit Sub
        End If

        Dim filteringItem As GridFilteringItem = DirectCast(e.Item, GridFilteringItem)

        For Each column As GridColumn In filteringItem.OwnerTableView.Columns
            Dim cell As TableCell = filteringItem(column.UniqueName)
            If cell.Controls.Count <= 0 Then
                Continue For
            End If

            If TypeOf cell.Controls(0) Is TextBox Then
                Dim filterBox As TextBox = cell.Controls(0)
                filterBox.CssClass = "filterBox"

                filterBox.Attributes.Add("onkeypress", String.Format("return DoFilter(this, event, '{0}', '{1}', '{2}');",
                                                                     ClientID, column.UniqueName, column.CurrentFilterFunction.ToString()))

                'Salvo il valore nella sessione
                If Not Page.IsPostBack Then
                    FilterHelper.SetSessionFilterClient(Page.Session, column.UniqueName, New FilterClient(column.UniqueName, filterBox.UniqueID, filterBox.Text))
                End If

                Dim table As DSTable = FilterHelper.CreateTable(filterBox, Nothing)
                cell.Controls.Clear()
                cell.Controls.Add(table.Table)

            ElseIf TypeOf cell.Controls(0) Is RadDatePicker Then
                'column.HeaderStyle.Width = Unit.Pixel(135)
                'column.ItemStyle.Width = Unit.Pixel(135)
                DirectCast(cell.Controls(2), Button).ToolTip = "Seleziona Filtro"

                Dim datePicker As New RadDatePicker()
                datePicker.Width = Unit.Pixel(96)

                Dim table As DSTable = FilterHelper.CreateTable(datePicker, cell.Controls(2))
                cell.Controls.Clear()
                cell.Controls.Add(table.Table)

                'Salvo il valore nella sessione
                If Not Page.IsPostBack Then
                    FilterHelper.SetSessionFilterClient(Page.Session, column.UniqueName, New FilterClient(column.UniqueName, datePicker.Controls(0).ClientID + "_text", ""))
                End If

            End If

        Next
    End Sub

    Public Function GetRedirectParentPageScript(ByVal url As String) As String
        Return String.Format("return RedirectParent('{0}');", url)
    End Function

    Public Sub DoExport(ByVal e As GridCommandEventArgs, ByRef gridColumns As GridColumnCollection, ByRef gridItems As GridDataItemCollection)
        ExportSettings.ExportOnlyData = True
        ExportSettings.OpenInNewWindow = True
        ExportSettings.Excel.Format = GridExcelExportFormat.Html
        ExportSettings.Pdf.PaperSize = GridPaperSize.A4
        ExportSettings.Pdf.PageHeight = Unit.Parse("210mm")
        ExportSettings.Pdf.PageWidth = Unit.Parse("297mm")
        ExportSettings.Pdf.AllowPrinting = True
        Select Case e.CommandArgument
            Case "Excel"
                Dim table As Table = ExportHelper.TableFromGrid(gridColumns, gridItems)
                ExportHelper.ExportToExcel(table, Page, ExportSettings.FileName & FileHelper.XLS)
            Case "Word"
                Dim table As Table = ExportHelper.TableFromGrid(gridColumns, gridItems)
                ExportHelper.ExportToWord(table, Page, ExportSettings.FileName & FileHelper.DOC)
            Case "Pdf"
                MasterTableView.ExportToPdf()
        End Select
    End Sub

#End Region

End Class
