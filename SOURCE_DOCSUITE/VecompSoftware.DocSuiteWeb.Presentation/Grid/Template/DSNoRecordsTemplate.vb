Imports System.Web.UI
Imports System.Web.UI.WebControls

<Obsolete("Eliminare questa classe il prima possibile")>
Public Class DSNoRecordsTemplate
    Implements ITemplate

#Region " Fields "

    Protected lblNoRecords As Label

    Private _text As String = ""

#End Region

    Public Sub New(ByVal text As String)
        _text = text
    End Sub

#Region " Methods "

    Public Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn
        'Cella Prima Pagina
        lblNoRecords = New Label()
        lblNoRecords.ID = "lblNoRecords"
        lblNoRecords.Text = _text
        lblNoRecords.Font.Bold = True

        Dim table As New DSTable
        table.Width = Unit.Percentage(100)
        table.Table.Height = Unit.Pixel(20)
        table.CreateEmptyRow()
        table.CurrentRow.CreateEmpytCell()
        table.CurrentRow.CurrentCell.TableCell.Style.Add("border", "0px !important")
        table.CurrentRow.CurrentCell.AddCellControl(lblNoRecords)

        container.Controls.Add(table.Table)
    End Sub

#End Region

End Class
