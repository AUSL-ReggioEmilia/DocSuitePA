Imports System.Collections.Generic
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Public Class ReslDisposPrint
    Inherits BasePrint

#Region "Fields"
#End Region

#Region "Properties"
#End Region

#Region "Create Rows"

    Private Sub CreateDataRow(ByRef tbl As DSTable, ByVal [date] As DateTime)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        'stile cella
        cellStyle.Width = Unit.Percentage(100)
        cellStyle.Font.Bold = True
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.ColumnSpan = 9
        cellStyle.LineBox = True

        'crea riga contenitori
        tbl.CreateEmptyRow("Prnt-Tabella")
        'crea cella
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = "Data adozione: " & String.Format("{0:dd/MM/yyyy}", [date])
        'stile cella
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
    End Sub

    Private Sub CreateResolutionHeader(ByRef tbl As DSTable)
        'crea riga
        tbl.CreateEmptyRow()

        'crea cella Tipologia
        tbl.CurrentRow.CreateEmpytCell()
        CreateTipologiaCellStyle(tbl.CurrentRow.CurrentCell, True)
        tbl.CurrentRow.CurrentCell.Text = Facade.ResolutionTypeFacade.DeterminaCaption & " n."

        'crea cella Oggetto
        tbl.CurrentRow.CreateEmpytCell()
        CreateOggettoCellStyle(tbl.CurrentRow.CurrentCell, True)
        tbl.CurrentRow.CurrentCell.Text = "Oggetto"

        'crea cella Contenitore
        tbl.CurrentRow.CreateEmpytCell()
        CreateContenitoreCellStyle(tbl.CurrentRow.CurrentCell, True)
        tbl.CurrentRow.CurrentCell.Text = "Contenitore"

        'crea cella Destinatari
        tbl.CurrentRow.CreateEmpytCell()
        CreateDestinatariCellStyle(tbl.CurrentRow.CurrentCell, True)
        tbl.CurrentRow.CurrentCell.Text = "Destinatari"
    End Sub

    Private Sub CreateResolutionRow(ByRef tbl As DSTable, ByVal res As Resolution)
        'crea riga
        tbl.CreateEmptyRow()

        'crea cella Tipologia
        tbl.CurrentRow.CreateEmpytCell()
        CreateTipologiaCellStyle(tbl.CurrentRow.CurrentCell, False)
        Dim s As String = "", s1 As String = ""
        Facade.ResolutionFacade.ReslFullNumber(res, res.Type.Id, s, s1)
        tbl.CurrentRow.CurrentCell.Text = If(s1 = "", s, s1) & "<BR>" & res.AdoptionDate.Value

        'crea cella Oggetto
        tbl.CurrentRow.CreateEmpytCell()
        CreateOggettoCellStyle(tbl.CurrentRow.CurrentCell, False)
        tbl.CurrentRow.CurrentCell.Text = StringHelper.ReplaceCrLf(res.ResolutionObject)

        'crea cella Contenitore
        tbl.CurrentRow.CreateEmpytCell()
        CreateContenitoreCellStyle(tbl.CurrentRow.CurrentCell, False)
        tbl.CurrentRow.CurrentCell.Text = res.Container.Name

        'crea cella Destinatari
        tbl.CurrentRow.CreateEmpytCell()
        CreateDestinatariCellStyle(tbl.CurrentRow.CurrentCell, False)
        Dim des As String = String.Empty
        GetContacts(res.ResolutionContacts, "D", des)
        If Not String.IsNullOrEmpty(res.AlternativeRecipient) Then
            des &= If(Not String.IsNullOrEmpty(des), "<BR>", "") & res.AlternativeRecipient
        End If
        tbl.CurrentRow.CurrentCell.Text = des
    End Sub
#End Region

#Region "Cell Styles"
    'Tipologia
    Private Sub CreateTipologiaCellStyle(ByRef cell As DSTableCell, ByVal bold As Boolean)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(10)
        cellStyle.Font.Bold = bold
        cellStyle.HorizontalAlignment = HorizontalAlign.Center
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    'Oggetto
    Private Sub CreateOggettoCellStyle(ByRef cell As DSTableCell, ByVal bold As Boolean)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(40)
        cellStyle.Font.Bold = bold
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.LineBox = True
        cellStyle.Wrap = True
        cell.ApplyStyle(cellStyle)
    End Sub

    'Contenitore
    Private Sub CreateContenitoreCellStyle(ByRef cell As DSTableCell, ByVal bold As Boolean)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(20)
        cellStyle.Font.Bold = bold
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    'Destinatari
    Private Sub CreateDestinatariCellStyle(ByRef cell As DSTableCell, ByVal bold As Boolean)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(30)
        cellStyle.Font.Bold = bold
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub
#End Region

#Region "IPrint Implementation"
    Public Overrides Sub DoPrint()
        CreatePrint()
    End Sub
#End Region

#Region "Private Functions"
    Private Sub CreatePrint()
        Dim lastDate As Date = Nothing
        Dim resls As IList(Of Resolution)
        Finder.EagerLog = False
        resls = Finder.DoSearch()

        If (resls.Count > 0) Then
            For Each res As Resolution In resls
                If (res.AdoptionDate <> lastDate) Then
                    'Data
                    CreateDataRow(TablePrint, res.AdoptionDate.Value)
                    'Intestazione
                    CreateResolutionHeader(TablePrint)
                End If
                CreateResolutionRow(TablePrint, res)
                lastDate = res.AdoptionDate
            Next
        End If
    End Sub

    Private Sub GetContacts(ByVal contacts As Object, ByVal communicationType As String, ByRef s As String)
        ContactFacade.FormatContacts(contacts, communicationType, s)

        Dim sRet As String = String.Empty
        Dim arr As String() = s.Split("#")
        For i As Integer = 0 To arr.Length - 1
            If arr(i) <> "§§§" Then
                If Not String.IsNullOrEmpty(arr(i)) Then sRet &= arr(i) & "<BR>"
                For j As Integer = i + 1 To arr.Length - 1
                    If arr(i) = arr(j) Then
                        arr(j) = "§§§"
                    End If
                Next
            End If
        Next i

        If sRet.Contains("<BR>") Then
            s = sRet.Substring(0, sRet.Length - 4)
        Else
            s = sRet
        End If

    End Sub
#End Region

End Class
