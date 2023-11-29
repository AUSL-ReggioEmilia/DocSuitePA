Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data

Public Class ReslWorkflowPrint
    Inherits ResolutionsPrint

#Region "Create Rows"
    Protected Overrides Sub CreateRowHeader(ByRef tbl As DSTable, ByVal textType As String)
        'NOTHING TO DO!
    End Sub

    Protected Overrides Sub CreaIntestazione(ByRef tbl As DSTable, ByVal numero As Boolean)
        'crea riga
        tbl.CreateEmptyRow()

        'crea cella Numerazione
        tbl.CurrentRow.CreateEmpytCell()
        CreateNumerazioneCellStyle(tbl.CurrentRow.CurrentCell, True)
        tbl.CurrentRow.CurrentCell.Text = "Numerazione"

        'crea cella Tipologia
        tbl.CurrentRow.CreateEmpytCell()
        CreateTipologiaCellStyle(tbl.CurrentRow.CurrentCell, True, HorizontalAlign.Center)
        tbl.CurrentRow.CurrentCell.Text = "Tipologia Atti"

        'crea cella Controllo
        tbl.CurrentRow.CreateEmpytCell()
        CreateControlloCellStyle(tbl.CurrentRow.CurrentCell, True, HorizontalAlign.Center)
        tbl.CurrentRow.CurrentCell.Text = "Controllo"

        'crea cella Oggetto
        tbl.CurrentRow.CreateEmpytCell()
        CreateOggettoCellStyle(tbl.CurrentRow.CurrentCell, True, HorizontalAlign.Center)
        tbl.CurrentRow.CurrentCell.Text = "Oggetto"

        'crea cella Registrazione
        tbl.CurrentRow.CreateEmpytCell()
        CreateRegAdoPubEseCellStyle(tbl.CurrentRow.CurrentCell, True)
        tbl.CurrentRow.CurrentCell.Text = If(numero, "Registrazione", "Tipologia")

        'crea cella Adozione
        tbl.CurrentRow.CreateEmpytCell()
        CreateRegAdoPubEseCellStyle(tbl.CurrentRow.CurrentCell, True)
        tbl.CurrentRow.CurrentCell.Text = "Adozione"

        'crea cella Pubblicazione
        tbl.CurrentRow.CreateEmpytCell()
        CreateRegAdoPubEseCellStyle(tbl.CurrentRow.CurrentCell, True)
        tbl.CurrentRow.CurrentCell.Text = "Pubblicaz. dal"

        'crea cella Esecuzione
        tbl.CurrentRow.CreateEmpytCell()
        CreateRegAdoPubEseCellStyle(tbl.CurrentRow.CurrentCell, True)
        tbl.CurrentRow.CurrentCell.Text = "Esecuzione"
    End Sub

    Protected Overrides Sub CreateRow(ByRef tbl As DSTable, ByRef resl As Resolution, ByVal numero As Boolean)
        'crea riga
        tbl.CreateEmptyRow()

        'crea cella Numerazione
        tbl.CurrentRow.CreateEmpytCell()
        CreateNumerazioneCellStyle(tbl.CurrentRow.CurrentCell, False)
        Dim s As String = "", s1 As String = ""
        Facade.ResolutionFacade.ReslFullNumber(resl, resl.Type.Id, s, s1)
        tbl.CurrentRow.CurrentCell.Text = If(s1 = "", "Reg: " & s, s1)

        'crea cella Tipologia 
        tbl.CurrentRow.CreateEmpytCell()
        CreateTipologiaCellStyle(tbl.CurrentRow.CurrentCell, False, HorizontalAlign.Left)
        tbl.CurrentRow.CurrentCell.Text = resl.Container.Name

        'crea cella Controllo
        tbl.CurrentRow.CreateEmpytCell()
        CreateControlloCellStyle(tbl.CurrentRow.CurrentCell, False, HorizontalAlign.Left)
        tbl.CurrentRow.CurrentCell.Text = GetControllo(resl)

        'crea cella Oggetto
        tbl.CurrentRow.CreateEmpytCell()
        CreateOggettoCellStyle(tbl.CurrentRow.CurrentCell, False, HorizontalAlign.Left)
        tbl.CurrentRow.CurrentCell.Text = resl.ResolutionObject

        'crea cella Tipologia
        tbl.CurrentRow.CreateEmpytCell()
        CreateRegAdoPubEseCellStyle(tbl.CurrentRow.CurrentCell, False)
        tbl.CurrentRow.CurrentCell.Text = If(numero, resl.ProposeDate.DefaultString(), Facade.ResolutionTypeFacade.GetDescription(resl.Type))

        'crea cella Adozione
        tbl.CurrentRow.CreateEmpytCell()
        CreateRegAdoPubEseCellStyle(tbl.CurrentRow.CurrentCell, False)
        If resl.AdoptionDate.HasValue Then
            tbl.CurrentRow.CurrentCell.Text = resl.AdoptionDate
        End If

        'crea cella Pubblicazione
        tbl.CurrentRow.CreateEmpytCell()
        CreateRegAdoPubEseCellStyle(tbl.CurrentRow.CurrentCell, False)
        If resl.PublishingDate.HasValue Then
            tbl.CurrentRow.CurrentCell.Text = resl.PublishingDate
        End If

        'crea cella Esecuzione
        tbl.CurrentRow.CreateEmpytCell()
        CreateRegAdoPubEseCellStyle(tbl.CurrentRow.CurrentCell, False)
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

    'Tipologia
    Private Sub CreateTipologiaCellStyle(ByRef cell As DSTableCell, ByVal bold As Boolean, ByVal align As HorizontalAlign)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(10)
        cellStyle.Font.Bold = bold
        cellStyle.HorizontalAlignment = align
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    'Controllo
    Private Sub CreateControlloCellStyle(ByRef cell As DSTableCell, ByVal bold As Boolean, ByVal align As HorizontalAlign)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(10)
        cellStyle.Font.Bold = bold
        cellStyle.HorizontalAlignment = align
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

    'Registrazione|Adozione|Pubblicazione|Esecuzione
    Private Sub CreateRegAdoPubEseCellStyle(ByRef cell As DSTableCell, ByVal bold As Boolean)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(10)
        cellStyle.Font.Bold = bold
        cellStyle.HorizontalAlignment = HorizontalAlign.Center
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub
#End Region

    Public Overrides Sub DoPrint()
        MyBase.DoPrint()
    End Sub

#Region "Private Functions"
    Private Function GetControllo(ByVal resl As Resolution) As String
        Dim sControllo As String = ""
        If (resl.OCSupervisoryBoard.GetValueOrDefault(False) = True) Then sControllo = "art.14"
        If (resl.OCRegion.GetValueOrDefault(False) = True) Then
            If sControllo <> "" Then sControllo &= "<BR>"
            sControllo &= "Regione"
        End If
        If (resl.OCManagement.GetValueOrDefault(False) = True) Then
            If sControllo <> "" Then sControllo &= "<BR>"
            sControllo &= "Controllo gestione"
        End If
        If (resl.OCOther.GetValueOrDefault(False) = True) Then
            If sControllo <> "" Then sControllo &= "<BR>"
            sControllo &= resl.OtherDescription
        End If
        If (resl.OCCorteConti.GetValueOrDefault(False) = True) Then
            If sControllo <> "" Then sControllo &= "<BR>"
            sControllo &= "Corte dei conti"
        End If
        Return sControllo
    End Function
#End Region
End Class
