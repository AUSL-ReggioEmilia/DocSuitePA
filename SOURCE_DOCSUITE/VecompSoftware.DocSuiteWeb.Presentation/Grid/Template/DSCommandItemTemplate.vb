Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers
Imports System.Web.UI.HtmlControls

Public Class DSCommandItemTemplate
    Implements ITemplate

#Region " Fields "

    Private _pageCount As Integer = 0
    'Private _pageIndex As Integer = 0
    Private _enableExport As Boolean = True
    Private _enableFilter As Boolean = True
    Private _enablePaging As Boolean = True
    Private _enableCustomExport As Boolean = False
    Private _customExportText As String = String.Empty
    Private _customExportTooltip As String = String.Empty

    Protected firstImage As RadButton

    Protected prevImage As RadButton

    Protected nextImage As RadButton

    Protected lastImage As RadButton

    Protected txtCurrentPage As RadTextBox
    Protected rangeValidator As RangeValidator
    Protected currentButton As RadButton

    Protected clearFilterButton As RadButton

    Protected excelButton As RadButton
    Protected excelFButton As RadButton
    Protected wordButton As RadButton
    Protected wordFButton As RadButton
    Protected pdfButton As RadButton
    Protected excelCButton As RadButton

    Dim _grid As BaseGrid

#End Region

#Region " Properties "

    Public Property EnablePagingButtons() As Boolean
        Get
            Return _enablePaging
        End Get
        Set(ByVal value As Boolean)
            _enablePaging = value
        End Set
    End Property

    Public Property EnableExportButtons() As Boolean
        Get
            Return _enableExport
        End Get
        Set(ByVal value As Boolean)
            _enableExport = value
        End Set
    End Property

    Public Property EnableFilterButtons() As Boolean
        Get
            Return _enableFilter
        End Get
        Set(ByVal value As Boolean)
            _enableFilter = value
        End Set
    End Property

    Public Property EnableCustomExportButtons() As Boolean
        Get
            Return _enableCustomExport
        End Get
        Set(ByVal value As Boolean)
            _enableCustomExport = value
        End Set
    End Property

    Public Property CustomExportButtonText() As String
        Get
            Return _customExportText
        End Get
        Set(ByVal value As String)
            _customExportText = value
        End Set
    End Property

    Public Property CustomExportButtonTooltip() As String
        Get
            Return _customExportTooltip
        End Get
        Set(ByVal value As String)
            _customExportTooltip = value
        End Set
    End Property

#End Region

#Region " Constructors "

    Public Sub New(ByVal enablePaging As Boolean, ByVal enableExport As Boolean, ByVal enableFilter As Boolean, ByVal enableCustomExport As Boolean, ByRef grid As BaseGrid)
        EnablePagingButtons = enablePaging
        EnableExportButtons = enableExport
        EnableFilterButtons = enableFilter
        EnableCustomExportButtons = enableCustomExport
        _grid = grid
    End Sub

#End Region

#Region " Methods "

    Private Sub EnableCustomPageButton(ByVal enable As Boolean)
        txtCurrentPage.ReadOnly = Not enable
        currentButton.Visible = enable
        rangeValidator.Enabled = enable
    End Sub

    ''' <summary> Crea controlli di paginazione </summary>
    ''' <param name="container">TD.rgCommandCell</param>
    Public Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn

        'Creazione tabella paginazione
        Dim tablePaging As New DSTable(False)
        tablePaging.CSSClass = "commandItemPaging"
        'Creazione riga della tabella paginazione
        tablePaging.CreateEmptyRow(False)
        If EnablePagingButtons Then
            'carica informazioni paginazione
            _pageCount = _grid.PageCount
            '_pageIndex = _grid.YetAnotherPageIndex
            'Cella Prima Pagina
            CreateFirstPageCell(tablePaging)
            'Cella Pagina Precedente
            CreatePrevPageCell(tablePaging)
            'Cella Vai a Pagina
            CreateTextBoxPageCell(tablePaging, container)
            'Cella Pagina Successiva
            CreateNextPageCell(tablePaging)
            'Cella Ultima Pagina
            CreateLastPageCell(tablePaging)
            'Abilita/disabilita pulsanti
            SetStatePageButtons()
        End If

        'Creazione tabella contatore risultati
        Dim tableCount As New DSTable(False)
        tableCount.CSSClass = "commandItemCount"

        'verifica che il numero di elementi sia minore del conteggio totale
        If _grid.DataSource IsNot Nothing AndAlso ReflectionHelper.HasProperty(_grid.DataSource, "Count") AndAlso _grid.DataSource.Count <= _grid.VirtualItemCount Then
            Dim lit As New Label()
            Dim current As Integer = _grid.DataSource.Count
            If (current >= (_grid.PageSize * (_grid.CustomPageIndex + 1))) Then
                current = (_grid.PageSize * (_grid.CustomPageIndex + 1))
            End If
            lit.Text = String.Format("Visualizzati {0} su {1} risultati", current, _grid.VirtualItemCount)
            lit.Font.Name = "Verdana"
            lit.Font.Bold = True

            tableCount.CreateEmptyRow(False)
            tableCount.CurrentRow.CreateEmpytCell(False)
            tableCount.CurrentRow.CurrentCell.AddCellControl(lit)
        End If

        'creazione tabella pulsanti
        Dim rightButtons As New HtmlGenericControl("div")
        rightButtons.Attributes("class") = "commandItemButtons"
        rightButtons.Style("float") = "right"
        If EnableExportButtons Then
            'Cella Pulsante azzeramento filtri
            If FilteringByColumnIsActive() Then
                clearFilterButton = New RadButton()
                clearFilterButton.ID = "clearFilter"
                clearFilterButton.ButtonType = RadButtonType.LinkButton
                clearFilterButton.Icon.SecondaryIconUrl = "~/App_Themes/DocSuite2008/imgset16/filter_delete.png"
                clearFilterButton.CommandName = "ClearFilters"
                clearFilterButton.Text = "Pulisci"
                clearFilterButton.ToolTip = "Pulisci Filtri"
                clearFilterButton.CausesValidation = False
            End If

            ' Pulsanti esportazione
            excelButton = New RadButton()
            excelButton.ID = "excelButton"
            excelButton.ButtonType = RadButtonType.LinkButton
            excelButton.Icon.SecondaryIconUrl = "~/App_Themes/DocSuite2008/imgset16/file_extension_xls.png"
            excelButton.CommandName = "Export"
            excelButton.CommandArgument = "Excel"
            excelButton.Text = "Esporta pagina"
            excelButton.ToolTip = "Esporta la pagina corrente della ricerca in Excel"
            excelButton.CausesValidation = False

            excelFButton = New RadButton()
            excelFButton.ID = "excelFButton"
            excelFButton.ButtonType = RadButtonType.LinkButton
            excelFButton.Icon.SecondaryIconUrl = "~/App_Themes/DocSuite2008/imgset16/file_extension_xls.png"
            excelFButton.CommandName = "ExportFull"
            excelFButton.CommandArgument = "Excel"
            excelFButton.Text = "Esporta tutto"
            excelFButton.ToolTip = "Esporta tutto il risultato della ricerca in Excel"
            excelFButton.CausesValidation = False
            excelFButton.Style("margin-right") = "10px"

            wordButton = New RadButton()
            wordButton.ID = "wordButton"
            wordButton.ButtonType = RadButtonType.LinkButton
            wordButton.Icon.SecondaryIconUrl = "~/App_Themes/DocSuite2008/imgset16/file_extension_doc.png"
            wordButton.CommandName = "Export"
            wordButton.CommandArgument = "Word"
            wordButton.Text = "Esporta pagina"
            wordButton.ToolTip = "Esporta la pagina corrente della ricerca in Word"
            wordButton.CausesValidation = False

            wordFButton = New RadButton()
            wordFButton.ID = "wordFButton"
            wordFButton.ButtonType = RadButtonType.LinkButton
            wordFButton.Icon.SecondaryIconUrl = "~/App_Themes/DocSuite2008/imgset16/file_extension_doc.png"
            wordFButton.CommandName = "ExportFull"
            wordFButton.CommandArgument = "Word"
            wordFButton.Text = "Esporta tutto"
            wordFButton.ToolTip = "Esporta tutto il risultato della ricerca in Word"
            wordFButton.CausesValidation = False

            If EnableCustomExportButtons Then
                excelCButton = New RadButton()
                excelCButton.ID = "excelCButton"
                excelCButton.ButtonType = RadButtonType.LinkButton
                excelCButton.Icon.SecondaryIconUrl = "~/App_Themes/DocSuite2008/imgset16/file_extension_xls.png"
                excelCButton.CommandName = "CustomExport"
                excelCButton.CommandArgument = "Excel"
                excelCButton.Text = CustomExportButtonText
                excelCButton.ToolTip = CustomExportButtonTooltip
                excelCButton.CausesValidation = False
                excelCButton.Style("margin-left") = "10px"
            End If

            If clearFilterButton IsNot Nothing Then
                rightButtons.Controls.Add(clearFilterButton)
            End If
            If excelCButton IsNot Nothing Then
                rightButtons.Controls.Add(excelCButton)
            End If
            rightButtons.Controls.Add(excelButton)
            rightButtons.Controls.Add(excelFButton)
            rightButtons.Controls.Add(wordButton)
            rightButtons.Controls.Add(wordFButton)
        End If

        ' Assemblo la tabella padre
        'Dim tableContainer As New DSTable(False)
        Dim tableContainer As New HtmlGenericControl("div")
        tableContainer.Attributes("class") = "commandItemTable"
        tableContainer.Controls.Add(tablePaging.Table)
        tableContainer.Controls.Add(tableCount.Table)
        tableContainer.Controls.Add(rightButtons)

        'Aggiunge tabella al controllo
        container.Controls.Add(tableContainer)
    End Sub

    Private Sub SetStatePageButtons()
        If _pageCount = 1 Then
            firstImage.Enabled = False
            prevImage.Enabled = False
            lastImage.Enabled = False
            nextImage.Enabled = False
            EnableCustomPageButton(True)
            Exit Sub
        End If

        Select Case _grid.YetAnotherPageIndex
            Case 1
                firstImage.Enabled = False
                prevImage.Enabled = False
                lastImage.Enabled = True
                nextImage.Enabled = True
                EnableCustomPageButton(True)
            Case _pageCount
                firstImage.Enabled = True
                prevImage.Enabled = True
                lastImage.Enabled = False
                nextImage.Enabled = False
                EnableCustomPageButton(True)
            Case Else
                firstImage.Enabled = True
                prevImage.Enabled = True
                lastImage.Enabled = True
                nextImage.Enabled = True
                EnableCustomPageButton(True)
        End Select
    End Sub

    ''' <summary> Crea la cella con i controlli per andare alla prima pagina della griglia. </summary>
    ''' <param name="table">tabella a cui agganciare i controlli</param>
    Private Sub CreateFirstPageCell(ByRef table As DSTable)
        firstImage = New RadButton()
        firstImage.Icon.PrimaryIconCssClass = "firstPage"
        firstImage.ButtonType = RadButtonType.LinkButton
        firstImage.ID = "firstImage"
        firstImage.CommandName = "Page"
        firstImage.CommandArgument = "First"
        firstImage.CausesValidation = False
        firstImage.Text = "Prima"
        firstImage.ToolTip = "Vai alla prima pagina"

        table.CurrentRow.CreateEmpytCell(False)
        table.CurrentRow.CurrentCell.Width = Unit.Pixel(80)
        table.CurrentRow.CurrentCell.AddCellControl(firstImage)
    End Sub

    ''' <summary> Crea la cella con i controlli per andare alla precedente pagina della griglia. </summary>
    ''' <param name="table">tabella a cui agganciare i controlli</param>
    Private Sub CreatePrevPageCell(ByRef table As DSTable)
        prevImage = New RadButton()
        prevImage.ID = "prevImage"
        prevImage.Icon.PrimaryIconCssClass = "prevPage"
        prevImage.ButtonType = RadButtonType.LinkButton
        prevImage.CommandName = "Page"
        prevImage.CommandArgument = "Prev"
        prevImage.CausesValidation = False
        prevImage.Text = "Precedente"
        prevImage.ToolTip = "Vai alla pagina precedente"

        table.CurrentRow.CreateEmpytCell(False)
        table.CurrentRow.CurrentCell.Width = Unit.Pixel(100)
        table.CurrentRow.CurrentCell.AddCellControl(prevImage)
    End Sub

    ''' <summary>
    ''' Crea la cella con i controlli per andare alla successiva pagina della griglia 
    ''' </summary>
    ''' <param name="table">tabella a cui agganciare i controlli</param>
    Private Sub CreateNextPageCell(ByRef table As DSTable)
        nextImage = New RadButton()
        nextImage.ID = "nextImage"
        nextImage.Icon.SecondaryIconCssClass = "nextPage"
        nextImage.ButtonType = RadButtonType.LinkButton
        nextImage.CommandName = "Page"
        nextImage.CommandArgument = "Next"
        nextImage.CausesValidation = False
        nextImage.Text = "Successiva"
        nextImage.ToolTip = "Vai alla pagina successiva"

        table.CurrentRow.CreateEmpytCell(False)
        table.CurrentRow.CurrentCell.Width = Unit.Pixel(100)
        table.CurrentRow.CurrentCell.AddCellControl(nextImage)
    End Sub

    ''' <summary> Crea la cella con i controlli per andare all'ultima pagina della griglia. </summary>
    ''' <param name="table">tabella a cui agganciare i controlli</param>
    Private Sub CreateLastPageCell(ByRef table As DSTable)
        lastImage = New RadButton()
        lastImage.ID = "lastImage"
        lastImage.Icon.SecondaryIconCssClass = "lastPage"
        lastImage.ButtonType = RadButtonType.LinkButton
        lastImage.CommandName = "Page"
        lastImage.CommandArgument = "Last"
        lastImage.CausesValidation = False
        lastImage.Text = "Ultima"
        lastImage.ToolTip = "Vai all'ultima pagina"

        table.CurrentRow.CreateEmpytCell(False)
        table.CurrentRow.CurrentCell.Width = Unit.Pixel(80)
        table.CurrentRow.CurrentCell.AddCellControl(lastImage)
    End Sub

    ''' <summary> Crea la cella con la textbox per andare ad una pagina specificata </summary>
    ''' <param name="table">tabella a cui agganciare i controlli</param>
    Private Sub CreateTextBoxPageCell(ByRef table As DSTable, ByRef container As Control)
        txtCurrentPage = New RadTextBox()
        txtCurrentPage.ID = "txtCurrentPage"
        txtCurrentPage.Width = Unit.Pixel(50)
        txtCurrentPage.Text = _grid.YetAnotherPageIndex
        txtCurrentPage.Style.Add("text-align", "center")

        rangeValidator = New RangeValidator()
        rangeValidator.ID = "rangeValidator1"
        rangeValidator.ControlToValidate = txtCurrentPage.ID
        rangeValidator.EnableClientScript = True
        rangeValidator.MinimumValue = 1
        rangeValidator.MaximumValue = _pageCount
        rangeValidator.Display = ValidatorDisplay.Dynamic
        rangeValidator.Type = ValidationDataType.Integer
        rangeValidator.ErrorMessage = "Valore non compreso tra 1 - " + rangeValidator.MaximumValue

        currentButton = New RadButton()
        currentButton.ID = "currentButton"
        currentButton.Text = "Vai"
        currentButton.CausesValidation = False
        currentButton.CommandName = "CustomChangePage"

        Dim panel As New Panel()
        panel.Controls.Add(New LiteralControl("Pagina: "))
        panel.Controls.Add(txtCurrentPage)
        panel.Controls.Add(New LiteralControl(" di " & _pageCount & " &nbsp;"))
        panel.Controls.Add(rangeValidator)
        panel.Controls.Add(currentButton)
        panel.DefaultButton = currentButton.ID
        panel.Wrap = False

        container.Page.Session.Add("PageID", container.UniqueID.Substring(0, container.UniqueID.Length - 5) + txtCurrentPage.UniqueID)
        container.Page.Session.Add("TargetID", container.UniqueID.Substring(0, container.UniqueID.Length - 5) + currentButton.UniqueID)

        table.CurrentRow.CreateEmpytCell(False)
        table.CurrentRow.CurrentCell.Width = Unit.Pixel(200)
        table.CurrentRow.CurrentCell.HorizontalAlignment = HorizontalAlign.Center
        table.CurrentRow.CurrentCell.AddCellControl(panel)
    End Sub

    ''' <summary> Controlla se è abilitata la funzione di filtro per i campi. </summary>
    ''' <remarks> Esegue il controllo a livello di <see cref="RadGrid"/>, di <see cref=" GridTableView"/> e di singole colonne. </remarks>
    Private Function FilteringByColumnIsActive() As Boolean
        If Not _grid.AllowFilteringByColumn AndAlso Not _grid.MasterTableView.AllowFilteringByColumn Then
            Return False
        End If
        ' Se abilitato cerco se c'è almeno una colonna con filtraggio abilitato
        For Each obj As Object In _grid.Columns
            Dim column As GridEditableColumn = TryCast(obj, GridEditableColumn)
            If column IsNot Nothing AndAlso column.SupportsFiltering() Then
                Return True
            End If
        Next
        Return False
    End Function

#End Region

End Class
