Imports System.Runtime.CompilerServices

Public Module DataTableExtensions

    <Extension()>
    Public Sub ExportToExcel(dataTable As DataTable, currentPage As Page, fileName As String)
        Dim table As Table = New Table With {
            .BackColor = Drawing.Color.Transparent
        }

        Dim tableRow As TableRow = New TableRow() With {
            .BackColor = Drawing.Color.Transparent
        }
        Dim columnCell As TableCell
        For Each column As DataColumn In dataTable.Columns
            columnCell = New TableCell With {
                .Text = column.ColumnName,
                .BackColor = Drawing.Color.Transparent,
                .BorderStyle = BorderStyle.Solid
            }
            tableRow.Cells.Add(columnCell)
        Next
        table.Rows.Add(tableRow)

        Dim rowCell As TableCell
        For Each row As DataRow In dataTable.Rows
            tableRow = New TableRow()
            For Each column As DataColumn In dataTable.Columns
                rowCell = New TableCell With {
                    .Text = row.Item(column.ColumnName).ToString(),
                    .BackColor = Drawing.Color.Transparent,
                    .BorderStyle = BorderStyle.Solid
                }
                tableRow.Cells.Add(rowCell)
            Next
            table.Rows.Add(tableRow)
        Next

        ExportHelper.ExportToExcel(table, currentPage, fileName)
    End Sub

End Module
