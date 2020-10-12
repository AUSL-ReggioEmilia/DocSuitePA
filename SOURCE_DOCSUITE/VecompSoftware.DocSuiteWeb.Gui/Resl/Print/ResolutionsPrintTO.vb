Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Linq
Public Class ResolutionsPrintTO
    Inherits ResolutionsPrint

#Region "Create Rows"
    Protected Overrides Sub CreateRowHeader(ByRef tbl As DSTable, ByVal textType As String)
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

    Protected Overrides Sub CreaIntestazione(ByRef tbl As DSTable, ByVal numero As Boolean)
        'crea riga
        tbl.CreateEmptyRow()

        'crea cella Numerazione
        tbl.CurrentRow.CreateEmpytCell()
        CreateNumerazioneCellStyle(tbl.CurrentRow.CurrentCell, True)
        tbl.CurrentRow.CurrentCell.Text = "Numerazione"

        'crea cella Contenitore
        tbl.CurrentRow.CreateEmpytCell()
        CreateContenitoreCellStyle(tbl.CurrentRow.CurrentCell, True, HorizontalAlign.Center)
        tbl.CurrentRow.CurrentCell.Text = "Tipologia Atti"

        'crea cella Controllo
        tbl.CurrentRow.CreateEmpytCell()
        CreateControlloCellStyle(tbl.CurrentRow.CurrentCell, True, HorizontalAlign.Center)
        tbl.CurrentRow.CurrentCell.Text = "Controllo"

        tbl.CurrentRow.CreateEmpytCell()
        CreateAdoPubEseCellStyle(tbl.CurrentRow.CurrentCell, True)
        tbl.CurrentRow.CurrentCell.Text = "Prop."

        'crea cella Oggetto
        tbl.CurrentRow.CreateEmpytCell()
        CreateOggettoCellStyle(tbl.CurrentRow.CurrentCell, True, HorizontalAlign.Center)
        tbl.CurrentRow.CurrentCell.Text = "Oggetto"

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
        tbl.CurrentRow.CurrentCell.Text = "Pubblicaz. dal"

        'crea cella Esecuzione
        tbl.CurrentRow.CreateEmpytCell()
        CreateAdoPubEseCellStyle(tbl.CurrentRow.CurrentCell, True)
        tbl.CurrentRow.CurrentCell.Text = "Esecuzione"

    End Sub

    Protected Overrides Sub CreateRow(ByRef tbl As DSTable, ByRef resl As Resolution, ByVal numero As Boolean)
        'crea riga
        tbl.CreateEmptyRow()

        'crea cella Numerazione
        tbl.CurrentRow.CreateEmpytCell()
        CreateNumerazioneCellStyle(tbl.CurrentRow.CurrentCell, False)
        Dim s As String = "", s1 As String = ""
        Facade.ResolutionFacade.ReslFullNumber(resl, resl.Type.Id, s, s1, False)
        tbl.CurrentRow.CurrentCell.Text = If(s1 = "", "Reg: " & s, s1)

        'crea cella Contenitore
        tbl.CurrentRow.CreateEmpytCell()
        CreateContenitoreCellStyle(tbl.CurrentRow.CurrentCell, False, HorizontalAlign.Left)
        tbl.CurrentRow.CurrentCell.Text = resl.Container.Name

        'crea cella Controllo
        tbl.CurrentRow.CreateEmpytCell()
        Dim sControllo As String = ""
        If (resl.OCSupervisoryBoard.HasValue AndAlso resl.OCSupervisoryBoard.Value = True) Then sControllo = "art.14"

        If (resl.OCRegion.HasValue AndAlso resl.OCRegion.Value = True) Then
            If sControllo <> "" Then sControllo &= "<BR>"
            sControllo &= "Regione"
        End If

        If (resl.OCCorteConti.HasValue AndAlso resl.OCCorteConti.Value = True) Then
            If sControllo <> "" Then sControllo &= "<BR>"
            sControllo &= "CorteConti"
        End If

        If (resl.OCOther.HasValue AndAlso resl.OCOther.Value = True) Then
            If sControllo <> "" Then sControllo &= "<BR>"
            sControllo &= resl.OtherDescription
        End If

        CreateControlloCellStyle(tbl.CurrentRow.CurrentCell, False, HorizontalAlign.Left)
        tbl.CurrentRow.CurrentCell.Text = sControllo

        tbl.CurrentRow.CreateEmpytCell()
        CreateProposerCellStyle(tbl.CurrentRow.CurrentCell, False)
        If resl.ResolutionContactProposers IsNot Nothing AndAlso resl.ResolutionContactProposers.Count > 0 Then
            tbl.CurrentRow.CurrentCell.Text = String.Join(", ", resl.ResolutionContactProposers.Select(Function(x) x.Contact.Code))
        End If

        'crea cella Oggetto
        tbl.CurrentRow.CreateEmpytCell()
        CreateOggettoCellStyle(tbl.CurrentRow.CurrentCell, False, HorizontalAlign.Left)
        tbl.CurrentRow.CurrentCell.Text = resl.ResolutionObject

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
    End Sub
#End Region

#Region "Cell Styles"
    'Numerazione
    Private Sub CreateNumerazioneCellStyle(ByRef cell As DSTableCell, ByVal bold As Boolean)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(10)
        cellStyle.Font.Bold = bold
        cellStyle.HorizontalAlignment = HorizontalAlign.Center
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    'Contenitore
    Private Sub CreateContenitoreCellStyle(ByRef cell As DSTableCell, ByVal bold As Boolean, ByVal align As HorizontalAlign)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(10)
        cellStyle.Font.Bold = bold
        cellStyle.HorizontalAlignment = align
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    'Registrazione
    Private Sub CreateRegistrazioneCellStyle(ByRef cell As DSTableCell, ByVal bold As Boolean)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(10)
        cellStyle.Font.Bold = bold
        cellStyle.HorizontalAlignment = HorizontalAlign.Center
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    'Tipologia
    Private Sub CreateTipologiaCellStyle(ByRef cell As DSTableCell, ByVal bold As Boolean)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(10)
        cellStyle.Font.Bold = bold
        cellStyle.HorizontalAlignment = HorizontalAlign.Center
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    'Proposer
    Private Sub CreateProposerCellStyle(ByRef cell As DSTableCell, ByVal bold As Boolean)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(4)
        cellStyle.Font.Bold = bold
        cellStyle.HorizontalAlignment = HorizontalAlign.Center
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    'Adozione|Pubblicazione|Esecuzione
    Private Sub CreateAdoPubEseCellStyle(ByRef cell As DSTableCell, ByVal bold As Boolean)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(10)
        cellStyle.Font.Bold = bold
        cellStyle.HorizontalAlignment = HorizontalAlign.Center
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    'Oggetto
    Private Sub CreateOggettoCellStyle(ByRef cell As DSTableCell, ByVal bold As Boolean, ByVal align As HorizontalAlign)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(30)
        cellStyle.Font.Bold = bold
        cellStyle.HorizontalAlignment = align
        cellStyle.LineBox = True
        cellStyle.Wrap = True
        cell.ApplyStyle(cellStyle)
    End Sub

    'Controllo
    Private Sub CreateControlloCellStyle(ByRef cell As DSTableCell, ByVal bold As Boolean, ByVal align As HorizontalAlign)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(10)
        cellStyle.Font.Bold = bold
        cellStyle.HorizontalAlignment = align
        cellStyle.LineBox = True
        cellStyle.Wrap = True
        cell.ApplyStyle(cellStyle)
    End Sub
#End Region

#Region "IPrint Implementation"
    Public Overrides Sub DoPrint()
        MyBase.DoPrint()

        'Setto il titolo della stampa
        TitlePrint = "ELENCO ATTI SELEZIONATI"
    End Sub
#End Region

#Region "Private Function"
    Protected Overrides Sub CreateHeader()
        'MUST DO NOTHING!!!!!
    End Sub
#End Region

End Class
