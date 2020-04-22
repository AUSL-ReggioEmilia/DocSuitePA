Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports Telerik.Web.UI

Public Class DSPagerTemplate
    Implements ITemplate

#Region "Controls"
    Protected firstImage As RadButton

    Protected prevImage As RadButton

    Protected nextImage As RadButton

    Protected lastImage As RadButton

    Protected txtCurrentPage As TextBox
    Protected rangeValidator As RangeValidator
    Protected currentButton As LinkButton

    Protected excelButton As ImageButton
    Protected wordButton As ImageButton
    Protected pdfButton As ImageButton

    Protected clearFilterButton As ImageButton
#End Region

    Public Sub InstantiateIn(ByVal container As System.Web.UI.Control) Implements System.Web.UI.ITemplate.InstantiateIn
        Dim table As New DSTable
        table.CSSClass = "CustomGridPager"
        table.Width = Unit.Percentage(100)
        table.Table.Height = Unit.Pixel(20)

        'Creazione riga della tabella
        table.CreateEmptyRow()

        'Cella Prima Pagina
        CreateFirstPageCell(table)
        'Cella Pagina Precedente
        CreatePrevPageCell(table)
        'Cella Vai a Pagina
        CreateTextBoxPageCell(table, container)
        'Cella Pagina Successiva
        CreateNextPageCell(table)
        'Cella Ultima Pagina
        CreateLastPageCell(table)
        'Cella Pulsante azzeramento filtri
        CreateClearFiltersCell(table)
        'Cella Pulsanti esportazione
        CreateExportCell(table)

        'Aggiunge tabella al controllo
        container.Controls.Add(table.Table)
    End Sub

#Region "Create Paging Cell"
    ''' <summary>Crea la cella con i controlli per andare alla prima pagina della griglia </summary>
    ''' <param name="table">tabella a cui agganciare i controlli</param>
    Private Sub CreateFirstPageCell(ByRef table As DSTable)
        firstImage = New RadButton()
        firstImage.Icon.PrimaryIconCssClass = "firstPage"
        firstImage.ID = "firstImage"
        firstImage.CommandName = "Page"
        firstImage.CommandArgument = "First"
        firstImage.CausesValidation = False
        'firstImage.TabIndex = -1
        firstImage.Text = "Prima"
        firstImage.ToolTip = "Vai alla prima pagina"

        table.CurrentRow.CreateEmpytCell()
        table.CurrentRow.CurrentCell.Width = Unit.Percentage(8)
        table.CurrentRow.CurrentCell.TableCell.Style.Add("padding-left", "10px")
        table.CurrentRow.CurrentCell.AddCellControl(firstImage)
    End Sub

    ''' <summary> Crea la cella con i controlli per andare alla precedente pagina della griglia </summary>
    ''' <param name="table">tabella a cui agganciare i controlli</param>
    Private Sub CreatePrevPageCell(ByRef table As DSTable)
        prevImage = New RadButton()
        prevImage.ID = "prevImage"
        prevImage.Icon.PrimaryIconCssClass = "prevPage"
        prevImage.CommandName = "Page"
        prevImage.CommandArgument = "Prev"
        prevImage.CausesValidation = False
        'prevImage.TabIndex = -1
        prevImage.Text = "Precedente"
        prevImage.ToolTip = "Vai alla pagina precedente"

        table.CurrentRow.CreateEmpytCell()
        table.CurrentRow.CurrentCell.Width = Unit.Percentage(8)
        table.CurrentRow.CurrentCell.AddCellControl(prevImage)
    End Sub

    ''' <summary> Crea la cella con i controlli per andare alla successiva pagina della griglia </summary>
    ''' <param name="table">tabella a cui agganciare i controlli</param>
    Private Sub CreateNextPageCell(ByRef table As DSTable)
        nextImage = New RadButton()
        nextImage.ID = "nextImage"
        nextImage.Icon.SecondaryIconCssClass = "nextPage"
        nextImage.CommandName = "Page"
        nextImage.CommandArgument = "Next"
        nextImage.CausesValidation = False
        'nextImage.TabIndex = -1
        nextImage.Text = "Successiva"
        nextImage.ToolTip = "Vai alla pagina successiva"

        table.CurrentRow.CreateEmpytCell()
        table.CurrentRow.CurrentCell.Width = Unit.Percentage(8)
        table.CurrentRow.CurrentCell.AddCellControl(nextImage)
    End Sub

    ''' <summary> Crea la cella con i controlli per andare all'ultima pagina della griglia </summary>
    ''' <param name="table">tabella a cui agganciare i controlli</param>
    Private Sub CreateLastPageCell(ByRef table As DSTable)
        lastImage = New RadButton()
        lastImage.ID = "lastImage"
        lastImage.Icon.SecondaryIconCssClass = "lastPage"
        lastImage.CommandName = "Page"
        lastImage.CommandArgument = "Last"
        lastImage.CausesValidation = False
        'lastImage.TabIndex = -1
        lastImage.Text = "Ultima"
        lastImage.ToolTip = "Vai all'ultima pagina"

        table.CurrentRow.CreateEmpytCell()
        table.CurrentRow.CurrentCell.Width = Unit.Percentage(8)
        table.CurrentRow.CurrentCell.AddCellControl(lastImage)
    End Sub

    ''' <summary> Crea la cella con la textbox per andare ad una pagina specificata </summary>
    ''' <param name="table">tabella a cui agganciare i controlli</param>
    Private Sub CreateTextBoxPageCell(ByRef table As DSTable, ByRef container As Control)
        Dim pageCount As Integer = CType(container.NamingContainer, Telerik.Web.UI.GridPagerItem).OwnerTableView.PageCount
        Dim pageIndex As Integer = CType(container.NamingContainer, Telerik.Web.UI.GridPagerItem).OwnerTableView.CurrentPageIndex

        txtCurrentPage = New TextBox()
        txtCurrentPage.ID = "txtCurrentPage"
        txtCurrentPage.Columns = "5"
        txtCurrentPage.Text = pageIndex + 1
        txtCurrentPage.TabIndex = -1

        rangeValidator = New RangeValidator()
        rangeValidator.ID = "rangeValidator1"
        rangeValidator.ControlToValidate = txtCurrentPage.ID
        rangeValidator.EnableClientScript = True
        rangeValidator.MinimumValue = 1
        rangeValidator.MaximumValue = pageCount
        rangeValidator.Display = ValidatorDisplay.Dynamic
        rangeValidator.Type = ValidationDataType.Integer
        rangeValidator.ErrorMessage = "Il valore non compreso tra 1 - " + rangeValidator.MaximumValue
        rangeValidator.Style.Add("margin-left", "2px")
        rangeValidator.Style.Add("margin-right", "2px")
        rangeValidator.TabIndex = -1

        currentButton = New LinkButton()
        currentButton.ID = "currentButton"
        currentButton.Text = "Vai"
        currentButton.CommandName = "CustomChangePage"
        currentButton.Style.Add("margin-left", "5px")
        currentButton.TabIndex = -1

        table.CurrentRow.CreateEmpytCell()
        table.CurrentRow.CurrentCell.Width = Unit.Percentage(18)
        table.CurrentRow.CurrentCell.HorizontalAlignment = HorizontalAlign.Center
        table.CurrentRow.CurrentCell.AddCellControl(New LiteralControl("Pagina: "))
        table.CurrentRow.CurrentCell.AddCellControl(txtCurrentPage)
        table.CurrentRow.CurrentCell.AddCellControl(New LiteralControl(" di " & pageCount))
        table.CurrentRow.CurrentCell.AddCellControl(rangeValidator)
        table.CurrentRow.CurrentCell.AddCellControl(currentButton)
    End Sub

    ''' <summary> Crea la cella con il pulsante per pulire i filtri </summary>
    ''' <param name="table">tabella a cui agganciare i controlli</param>
    Private Sub CreateClearFiltersCell(ByVal table)
        clearFilterButton = New ImageButton()
        clearFilterButton.ID = "clearFilter"
        clearFilterButton.CssClass = "clearFilter"
        clearFilterButton.CommandName = "ClearFilters"
        clearFilterButton.ToolTip = "Pulisci Filtri"
        clearFilterButton.CausesValidation = False

        table.CurrentRow.CreateEmpytCell()
        table.CurrentRow.CurrentCell.HorizontalAlignment = HorizontalAlign.Right
        table.CurrentRow.CurrentCell.AddCellControl(clearFilterButton)
    End Sub

    ''' <summary> Crea la cella con i pulsanti di esportazione </summary>
    ''' <param name="table">tabella a cui agganciare i controlli</param>
    Private Sub CreateExportCell(ByRef table As DSTable)
        excelButton = New ImageButton()
        excelButton.ID = "excelButton"
        excelButton.CssClass = "exportExcel"
        excelButton.CommandName = "Export"
        excelButton.CommandArgument = "Excel"
        excelButton.ToolTip = "Esporta in Excel"
        excelButton.CausesValidation = False
        excelButton.TabIndex = -1

        wordButton = New ImageButton()
        wordButton.ID = "wordButton"
        wordButton.CssClass = "exportWord"
        wordButton.CommandName = "Export"
        wordButton.CommandArgument = "Word"
        wordButton.ToolTip = "Esporta in Word"
        wordButton.CausesValidation = False
        wordButton.TabIndex = -1

        table.CurrentRow.CreateEmpytCell()
        table.CurrentRow.CurrentCell.Width = Unit.Percentage(50)
        table.CurrentRow.CurrentCell.HorizontalAlignment = HorizontalAlign.Right
        table.CurrentRow.CurrentCell.AddCellControl(excelButton)
        table.CurrentRow.CurrentCell.AddCellControl(wordButton)
    End Sub

#End Region

End Class
