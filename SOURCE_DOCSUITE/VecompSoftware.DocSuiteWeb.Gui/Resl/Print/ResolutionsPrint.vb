Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data

Public Class ResolutionsPrint
    Inherits BasePrint

#Region " Fields "

    Private _listid As New List(Of Integer)

#End Region

#Region " Properties "

    Public Property ListId() As List(Of Integer)
        Get
            Return _listid
        End Get
        Set(ByVal value As List(Of Integer))
            _listid = value
        End Set
    End Property

#End Region

#Region " Methods "

    Protected Overridable Sub CreateRowHeader(ByRef tbl As DSTable, ByVal textType As String)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        'stile cella
        cellStyle.Width = Unit.Percentage(100)
        cellStyle.Font.Bold = True
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.ColumnSpan = 7

        'crea riga Tipo ordinamento
        tbl.CreateEmptyRow("Prnt-Tabella")
        'crea cella
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = textType
        'stile cella
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
    End Sub

    Protected Overridable Sub CreaIntestazione(ByRef tbl As DSTable, ByVal numero As Boolean)
        'crea riga
        tbl.CreateEmptyRow()

        'crea cella Numerazione
        tbl.CurrentRow.CreateEmpytCell()
        CreateNumerazioneCellStyle(tbl.CurrentRow.CurrentCell, True)
        tbl.CurrentRow.CurrentCell.Text = "Numerazione"

        'crea cella Contenitore
        tbl.CurrentRow.CreateEmpytCell()
        CreateContenitoreCellStyle(tbl.CurrentRow.CurrentCell, True)
        tbl.CurrentRow.CurrentCell.Text = "Contenitore"

        If numero Then
            'crea cella Registrazione
            tbl.CurrentRow.CreateEmpytCell()
            CreateRegistrazioneCellStyle(tbl.CurrentRow.CurrentCell, True)
            tbl.CurrentRow.CurrentCell.Text = "Registrazione"
        Else
            'crea cella Tipologia
            tbl.CurrentRow.CreateEmpytCell()
            CreateTipologiaCellStyle(tbl.CurrentRow.CurrentCell, True)
            tbl.CurrentRow.CurrentCell.Text = "Tipologia"
        End If

        'crea cella Adozione
        tbl.CurrentRow.CreateEmpytCell()
        CreateAdoPubEseCellStyle(tbl.CurrentRow.CurrentCell, True)
        tbl.CurrentRow.CurrentCell.Text = "Adozione"

        'crea cella Pubblicazione
        tbl.CurrentRow.CreateEmpytCell()
        CreateAdoPubEseCellStyle(tbl.CurrentRow.CurrentCell, True)
        tbl.CurrentRow.CurrentCell.Text = "Pubblicazione"

        'crea cella Esecuzione
        tbl.CurrentRow.CreateEmpytCell()
        CreateAdoPubEseCellStyle(tbl.CurrentRow.CurrentCell, True)
        tbl.CurrentRow.CurrentCell.Text = "Esecuzione"

        'crea cella Oggetto
        tbl.CurrentRow.CreateEmpytCell()
        CreateOggettoCellStyle(tbl.CurrentRow.CurrentCell, True, If(numero, 37, 34))
        tbl.CurrentRow.CurrentCell.Text = "Oggetto"
    End Sub

    Protected Overridable Sub CreateRow(ByRef tbl As DSTable, ByRef resl As Resolution, ByVal numero As Boolean)
        'crea riga
        tbl.CreateEmptyRow()

        'crea cella Numerazione
        tbl.CurrentRow.CreateEmpytCell()
        CreateNumerazioneCellStyle(tbl.CurrentRow.CurrentCell, False)
        Dim s As String = "", s1 As String = ""
        Facade.ResolutionFacade.ReslFullNumber(resl, resl.Type.Id, s, s1)
        tbl.CurrentRow.CurrentCell.Text = If(s1 = "", "Reg: " & s, s1)

        'crea cella Contenitore
        tbl.CurrentRow.CreateEmpytCell()
        CreateContenitoreCellStyle(tbl.CurrentRow.CurrentCell, False)
        tbl.CurrentRow.CurrentCell.Text = resl.Container.Name

        If numero Then
            'crea cella Registrazione
            tbl.CurrentRow.CreateEmpytCell()
            CreateRegistrazioneCellStyle(tbl.CurrentRow.CurrentCell, False)
            tbl.CurrentRow.CurrentCell.Text = resl.ProposeDate
        Else
            'crea cella Tipologia
            tbl.CurrentRow.CreateEmpytCell()
            CreateTipologiaCellStyle(tbl.CurrentRow.CurrentCell, False)
            tbl.CurrentRow.CurrentCell.Text = Facade.ResolutionTypeFacade.GetDescription(resl.Type)
        End If

        'crea cella Adozione
        tbl.CurrentRow.CreateEmpytCell()
        CreateAdoPubEseCellStyle(tbl.CurrentRow.CurrentCell, False)
        If resl.AdoptionDate.HasValue Then
            tbl.CurrentRow.CurrentCell.Text = resl.AdoptionDate
        End If

        'crea cella Pubblicazione
        tbl.CurrentRow.CreateEmpytCell()
        CreateAdoPubEseCellStyle(tbl.CurrentRow.CurrentCell, False)
        If resl.PublishingDate.HasValue Then
            tbl.CurrentRow.CurrentCell.Text = resl.PublishingDate
        End If

        'crea cella Esecuzione
        tbl.CurrentRow.CreateEmpytCell()
        CreateAdoPubEseCellStyle(tbl.CurrentRow.CurrentCell, False)
        If resl.EffectivenessDate.HasValue Then
            tbl.CurrentRow.CurrentCell.Text = resl.EffectivenessDate
        End If

        'crea cella Oggetto
        tbl.CurrentRow.CreateEmpytCell()
        CreateOggettoCellStyle(tbl.CurrentRow.CurrentCell, False, If(numero, 37, 34))
        tbl.CurrentRow.CurrentCell.Text = resl.ResolutionObject
    End Sub

    Private Sub CreateNumerazioneCellStyle(ByRef cell As DSTableCell, ByVal bold As Boolean)
        'Numerazione
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(15)
        cellStyle.Font.Bold = bold
        cellStyle.HorizontalAlignment = HorizontalAlign.Center
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    Private Sub CreateContenitoreCellStyle(ByRef cell As DSTableCell, ByVal bold As Boolean)
        'Contenitore
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(20)
        cellStyle.Font.Bold = bold
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    Private Sub CreateRegistrazioneCellStyle(ByRef cell As DSTableCell, ByVal bold As Boolean)
        'Registrazione
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(7)
        cellStyle.Font.Bold = bold
        cellStyle.HorizontalAlignment = HorizontalAlign.Center
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    Private Sub CreateTipologiaCellStyle(ByRef cell As DSTableCell, ByVal bold As Boolean)
        'Tipologia
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(10)
        cellStyle.Font.Bold = bold
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    Private Sub CreateAdoPubEseCellStyle(ByRef cell As DSTableCell, ByVal bold As Boolean)
        'Adozione|Pubblicazione|Esecuzione
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(7)
        cellStyle.Font.Bold = bold
        cellStyle.HorizontalAlignment = HorizontalAlign.Center
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    Private Sub CreateOggettoCellStyle(ByRef cell As DSTableCell, ByVal bold As Boolean, ByVal percentage As Integer)
        'Oggetto
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(percentage)
        cellStyle.Font.Bold = bold
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.LineBox = True
        cellStyle.Wrap = True
        cell.ApplyStyle(cellStyle)
    End Sub

    Public Overrides Sub DoPrint()
        'Setto il titolo della stampa
        TitlePrint = "Stampa elenco selezionati"

        Dim numero As Boolean
        Select Case True
            Case Facade.ResolutionFacade.IsManagedProperty("ServiceNumber", ResolutionType.IdentifierDetermina)
                numero = False
            Case Facade.ResolutionFacade.IsManagedProperty("Number", ResolutionType.IdentifierDetermina)
                numero = True
        End Select

        'Di default imposto l'ordine su 'Number' [comportamento legacy].
        'Se esplicitamente richiesto allora mantengo l'ordine ricevuto
        Dim order As String = Nothing
        If MantainResultsOrder = False Then
            order = "Number"
        End If
        Dim list As IList(Of Resolution) = Facade.ResolutionFacade.GetResolutions(ListId, order, True)
        If list.Count > 0 Then
            CreateHeader()
            CreatePrint(list, numero)
        End If
    End Sub

    Protected Overridable Sub CreateHeader()
        Dim textType As String = "Dettaglio"
        CreateRowHeader(TablePrint, textType)
    End Sub

    Private Sub CreatePrint(ByRef list As IList(Of Resolution), ByVal numero As Boolean)
        CreaIntestazione(TablePrint, numero)
        For Each resl As Resolution In list
            CreateRow(TablePrint, resl, numero)
        Next
    End Sub

#End Region

End Class
