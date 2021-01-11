Imports VecompSoftware.DocSuiteWeb.Facade

Public MustInherit Class ReslMemoPrint
    Inherits BasePrint

#Region "Properties"
    Protected ReadOnly Property ReslFacade() As ResolutionFacade
        Get
            Return Facade.ResolutionFacade
        End Get
    End Property
#End Region

#Region "Create Rows"
    Protected Sub CreateDivindingLine(ByRef tbl As DSTable, Optional ByVal size As String = "1")
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        'stile cella
        cellStyle.Width = Unit.Percentage(100)
        cellStyle.ColumnSpan = 3
        'crea riga
        tbl.CreateEmptyRow()
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.AddDividingLineControl("#000000", size)
        'stile cella
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
    End Sub

    Protected Sub CreateTitle(ByRef tbl As DSTable, ByVal title As String)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        'stile cella
        cellStyle.Width = Unit.Percentage(100)
        cellStyle.Font.Bold = True
        cellStyle.HorizontalAlignment = HorizontalAlign.Center
        cellStyle.ColumnSpan = 3

        'crea riga contenitori
        tbl.CreateEmptyRow()
        'crea cella
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = title
        'stile cella
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
    End Sub

    Protected Sub CreateSectionTitle(ByRef tbl As DSTable, ByVal title As String)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        'stile cella
        cellStyle.Width = Unit.Percentage(100)
        cellStyle.Font.Bold = False
        cellStyle.HorizontalAlignment = HorizontalAlign.Center
        cellStyle.ColumnSpan = 3

        'crea riga
        tbl.CreateEmptyRow()
        'crea cella
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = title
        'stile cella
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
    End Sub

    Protected Sub CreateDataRow(ByRef tbl As DSTable, ByVal protocolLink As String, ByVal reslFull As String, ByVal container As String, ByVal bold As Boolean)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        'stile cella
        cellStyle.Width = Unit.Percentage(20)
        cellStyle.Font.Bold = bold
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        'crea riga
        tbl.CreateEmptyRow()
        'crea cella collegamento Protocollo
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = protocolLink
        'stile cella
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)

        'crea cella atto
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = reslFull
        'stile cella
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)

        'crea cella contenitore
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = container
        'stile cella
        cellStyle.Width = Unit.Percentage(60)
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
    End Sub
#End Region

#Region "IPrint Implementation"
    Public Overrides Sub DoPrint()
        CreatePrint()
    End Sub
#End Region

#Region "Virtual Sub"
    Protected MustOverride Sub CreatePrint()
#End Region

End Class
