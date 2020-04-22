Partial Class UtltUsers
    Inherits CommonBasePage

#Region "Properties"
    Dim _tablePrint As DSTable
    Private ReadOnly Property TablePrint() As DSTable
        Get
            If _tablePrint Is Nothing Then
                _tablePrint = New DSTable()
                _tablePrint.Table.CellPadding = 1
                _tablePrint.Table.Width = Unit.Percentage(100)
            End If
            Return _tablePrint
        End Get
    End Property
#End Region

#Region "Creazione Righe"
    Private Sub CreateRow(ByRef tbl As DSTable, ByVal utente As String, ByVal computer As String, ByVal data As String, ByVal sessione As String, ByRef cssClass As String)
        Dim cellStyle As New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(25)
        cellStyle.Font.Bold = True
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.LineBox = False
        'crea riga
        tbl.CreateEmptyRow(cssClass)
        'crea cella Utente
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = utente
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        'crea cella Computer
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = computer
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        'crea cella Data
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = data
        cellStyle.HorizontalAlignment = HorizontalAlign.Center
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        'crea cella sessione
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = sessione
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
    End Sub
#End Region

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

        Do While Application.Count > 50
            Application.RemoveAt(0)
        Loop
        CreateRow(TablePrint, "Utente", "Computer", "Data", "Sessione", "tabella")
        For Each ic As String In Application.AllKeys
            Dim chiave As String() = Split(ic, "|")
            CreateRow(TablePrint, chiave(0), chiave(1), chiave(2), chiave(3), "Chiaro")
        Next ic
        phUsers.Controls.Add(TablePrint.Table)
    End Sub

End Class
