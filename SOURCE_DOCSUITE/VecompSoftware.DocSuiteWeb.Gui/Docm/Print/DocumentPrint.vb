Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Public Class DocumentPrint
    Inherits BasePrint

#Region "Fields"
    Private _listid As New List(Of YearNumberCompositeKey)
#End Region

#Region "properties"

    Public Property ListId() As List(Of YearNumberCompositeKey)
        Get
            Return _listid
        End Get
        Set(ByVal value As List(Of YearNumberCompositeKey))
            _listid = value
        End Set
    End Property


#End Region

#Region "Create Rows"
    Private Sub CreateRowHeader(ByRef tbl As DSTable, ByVal textType As String)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        'stile cella
        cellStyle.Width = Unit.Percentage(100)
        cellStyle.Font.Bold = True
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.ColumnSpan = 9

        'crea riga Tipo ordinamento
        tbl.CreateEmptyRow("Prnt-Tabella")
        'crea cella
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = textType
        'stile cella
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
    End Sub

    Private Sub CreaIntestazionePratiche(ByRef tbl As DSTable)
        'crea riga
        tbl.CreateEmptyRow()

        'crea cella Numero Pratica
        tbl.CurrentRow.CreateEmpytCell()
        CreateDocumentCellStyle(tbl.CurrentRow.CurrentCell)
        tbl.CurrentRow.CurrentCell.Font.Bold = True
        tbl.CurrentRow.CurrentCell.Text = "P. Numero"

        'crea cella Data
        tbl.CurrentRow.CreateEmpytCell()
        CreateDataCellStyle(tbl.CurrentRow.CurrentCell)
        tbl.CurrentRow.CurrentCell.Font.Bold = True
        tbl.CurrentRow.CurrentCell.Text = "Data"

        'crea cella Settore
        tbl.CurrentRow.CreateEmpytCell()
        CreateSettoreCellStyle(tbl.CurrentRow.CurrentCell)
        tbl.CurrentRow.CurrentCell.Font.Bold = True
        tbl.CurrentRow.CurrentCell.Text = "Settore Apert."

        'crea cella Data di scadenza
        tbl.CurrentRow.CreateEmpytCell()
        CreateScadenzaCellStyle(tbl.CurrentRow.CurrentCell)
        tbl.CurrentRow.CurrentCell.Font.Bold = True
        tbl.CurrentRow.CurrentCell.Text = "Scadenza"

        'crea cella Numero Servizio
        tbl.CurrentRow.CreateEmpytCell()
        CreateNumeroServizioCellStyle(tbl.CurrentRow.CurrentCell)
        tbl.CurrentRow.CurrentCell.Font.Bold = True
        tbl.CurrentRow.CurrentCell.Text = "N. Servizio"

        'crea cella Nome
        tbl.CurrentRow.CreateEmpytCell()
        CreateNomeCellStyle(tbl.CurrentRow.CurrentCell)
        tbl.CurrentRow.CurrentCell.Font.Bold = True
        tbl.CurrentRow.CurrentCell.Text = "Nome"

        'crea cella Oggetto
        tbl.CurrentRow.CreateEmpytCell()
        CreateOggettoCellStyle(tbl.CurrentRow.CurrentCell)
        tbl.CurrentRow.CurrentCell.Font.Bold = True
        tbl.CurrentRow.CurrentCell.Text = "Oggetto"

        'crea cella Categoria
        tbl.CurrentRow.CreateEmpytCell()
        CreateCategoriaCellStyle(tbl.CurrentRow.CurrentCell)
        tbl.CurrentRow.CurrentCell.Font.Bold = True
        tbl.CurrentRow.CurrentCell.Text = "Classificatore"

        'crea cella Responsabile
        tbl.CurrentRow.CreateEmpytCell()
        CreateResponsabileCellStyle(tbl.CurrentRow.CurrentCell)
        tbl.CurrentRow.CurrentCell.Font.Bold = True
        tbl.CurrentRow.CurrentCell.Text = "Responsabile"
    End Sub

    Private Sub CreateRowDocument(ByRef tbl As DSTable, ByRef docm As Document)
        'crea riga
        tbl.CreateEmptyRow()

        'crea cella Documento
        tbl.CurrentRow.CreateEmpytCell()
        CreateDocumentCellStyle(tbl.CurrentRow.CurrentCell)
        tbl.CurrentRow.CurrentCell.Text = DocumentUtil.DocmFull(docm.Year, docm.Number, " ")

        'crea cella Data
        tbl.CurrentRow.CreateEmpytCell()
        CreateDataCellStyle(tbl.CurrentRow.CurrentCell)
        tbl.CurrentRow.CurrentCell.Text = docm.StartDate.Value.ToString()

        'crea cella Settore
        tbl.CurrentRow.CreateEmpytCell()
        CreateSettoreCellStyle(tbl.CurrentRow.CurrentCell)
        tbl.CurrentRow.CurrentCell.Text = docm.Role.Name

        'crea cella Scadenza
        tbl.CurrentRow.CreateEmpytCell()
        CreateScadenzaCellStyle(tbl.CurrentRow.CurrentCell)
        Dim scadenza As String = String.Empty
        If docm.ExpiryDate.HasValue Then
            scadenza = docm.ExpiryDate.Value.ToString()
        End If
        tbl.CurrentRow.CurrentCell.Text = scadenza

        'crea cella Numero Servizio
        tbl.CurrentRow.CreateEmpytCell()
        CreateNumeroServizioCellStyle(tbl.CurrentRow.CurrentCell)
        tbl.CurrentRow.CurrentCell.Text = docm.ServiceNumber

        'crea cella Numero Servizio
        tbl.CurrentRow.CreateEmpytCell()
        CreateNomeCellStyle(tbl.CurrentRow.CurrentCell)
        tbl.CurrentRow.CurrentCell.Text = docm.Name

        'crea cella Oggetto
        tbl.CurrentRow.CreateEmpytCell()
        CreateOggettoCellStyle(tbl.CurrentRow.CurrentCell)
        tbl.CurrentRow.CurrentCell.Text = docm.DocumentObject

        'crea cella Categoria
        tbl.CurrentRow.CreateEmpytCell()
        CreateCategoriaCellStyle(tbl.CurrentRow.CurrentCell)
        If docm.Category IsNot Nothing Then
            tbl.CurrentRow.CurrentCell.Text = docm.Category.Name
        End If

        'crea cella Responsabile
        tbl.CurrentRow.CreateEmpytCell()
        CreateResponsabileCellStyle(tbl.CurrentRow.CurrentCell)
        tbl.CurrentRow.CurrentCell.Text = docm.Manager
    End Sub

#End Region

#Region "Contact Cell Styles"
    'Numero pratica
    Private Sub CreateDocumentCellStyle(ByRef cell As DSTableCell)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(10)
        cellStyle.Font.Bold = False
        cellStyle.HorizontalAlignment = HorizontalAlign.Center
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    'data
    Private Sub CreateDataCellStyle(ByRef cell As DSTableCell)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(8)
        cellStyle.Font.Bold = False
        cellStyle.HorizontalAlignment = HorizontalAlign.Center
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    'Settore
    Private Sub CreateSettoreCellStyle(ByRef cell As DSTableCell)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(10)
        cellStyle.Font.Bold = False
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    'Scadenza
    Private Sub CreateScadenzaCellStyle(ByRef cell As DSTableCell)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(8)
        cellStyle.Font.Bold = False
        cellStyle.HorizontalAlignment = HorizontalAlign.Center
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    'Numero Servizio
    Private Sub CreateNumeroServizioCellStyle(ByRef cell As DSTableCell)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(10)
        cellStyle.Font.Bold = False
        cellStyle.HorizontalAlignment = HorizontalAlign.Left

        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    'Nome
    Private Sub CreateNomeCellStyle(ByRef cell As DSTableCell)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(10)
        cellStyle.Font.Bold = False
        cellStyle.HorizontalAlignment = HorizontalAlign.Left

        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    'Oggetto
    Private Sub CreateOggettoCellStyle(ByRef cell As DSTableCell)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(24)
        cellStyle.Font.Bold = False
        cellStyle.HorizontalAlignment = HorizontalAlign.Left

        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    'Categoria
    Private Sub CreateCategoriaCellStyle(ByRef cell As DSTableCell)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(10)
        cellStyle.Font.Bold = False
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.Wrap = True
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    'Responsabile
    Private Sub CreateResponsabileCellStyle(ByRef cell As DSTableCell)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(10)
        cellStyle.Font.Bold = False
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.Wrap = True
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub
#End Region

#Region "IPrint Implementation"
    Public Overrides Sub DoPrint()
        'Setto il titolo della stampa
        TitlePrint = "Stampa Elenco Selezionati"

        Dim list As IList(Of Document) = New List(Of Document)
        Dim docmFacade As DocumentFacade = Facade.DocumentFacade()

        list = docmFacade.GetDocuments(Me.ListId)
        If list.Count > 0 Then
            CreateHeader()
            CreatePrint(list)
        End If
    End Sub
#End Region

#Region "Private Function"
    Private Sub CreateHeader()
        Dim textType As String = "Stampa delle Pratiche"
        CreateRowHeader(TablePrint, textType)
    End Sub

    Private Sub CreatePrint(ByRef list As IList(Of Document))
        CreaIntestazionePratiche(TablePrint)
        For Each docm As Document In list
            CreateRowDocument(TablePrint, docm)
        Next
    End Sub
#End Region

End Class
