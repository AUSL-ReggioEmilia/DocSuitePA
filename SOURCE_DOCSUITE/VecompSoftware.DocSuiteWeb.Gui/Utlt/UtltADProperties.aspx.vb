Imports VecompSoftware.DocSuiteWeb.Facade
Imports System.Collections.Generic

Public Class UtltADProperties
    Inherits UtltBasePage

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False
    End Sub

#End Region

#Region " Methods "

    Private Sub LoadProperties(ByVal userAccount As String)
        'Creo la tabella che conterrà i dati
        Dim table As New DSTable(False)
        table.CSSClass = "datatable"
        table.Width = Unit.Percentage(100)

        'Creo l'intestazione della tabella
        CreateRow(table, "Proprietà", "Valore", "tabella")

        'Visualizzo tutte le property
        Dim foundAdUser As AccountModel = CommonAD.GetAccount(userAccount)
        If foundAdUser IsNot Nothing AndAlso foundAdUser.RawProperties IsNot Nothing Then
            For Each prop As KeyValuePair(Of String, String) In foundAdUser.RawProperties
                CreateRow(table, prop.Key, prop.Value, "chiaro")
            Next

            phProperty.Controls.Add(table.Table)
        End If
    End Sub

    Private Sub CreateRow(ByRef tbl As DSTable, ByVal prop As String, ByVal value As String, Optional ByVal classname As String = "")

        'crea riga
        If String.IsNullOrEmpty(classname) Then
            tbl.CreateEmptyRow()
        Else
            tbl.CreateEmptyRow(classname)
        End If

        'crea cella Proprietà
        tbl.CurrentRow.CreateEmpytCell()
        CreatePropertyCellStyle(tbl.CurrentRow.CurrentCell)
        tbl.CurrentRow.CurrentCell.Text = prop
        tbl.CurrentRow.CurrentCell.TableCell.Style.Add("padding", "3xp 3px 3px 3px")

        'crea cella Valore
        tbl.CurrentRow.CreateEmpytCell()
        CreateValueCellStyle(tbl.CurrentRow.CurrentCell)
        tbl.CurrentRow.CurrentCell.Text = value
        tbl.CurrentRow.CurrentCell.TableCell.Wrap = True
        tbl.CurrentRow.CurrentCell.TableCell.Style.Add("padding", "3xp 3px 3px 3px")
    End Sub

    ''' <summary> Proprietà </summary>
    Private Sub CreatePropertyCellStyle(ByRef cell As DSTableCell)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(30)
        cellStyle.Font.Bold = True
        cellStyle.HorizontalAlignment = HorizontalAlign.Right
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

    ''' <summary> Valore </summary>
    Private Sub CreateValueCellStyle(ByRef cell As DSTableCell)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(70)
        cellStyle.Font.Bold = True
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.LineBox = True
        cell.ApplyStyle(cellStyle)
    End Sub

#End Region

    Private Sub BtnSearchClick(sender As Object, e As EventArgs) Handles btnSearch.Click
        LoadProperties(txtUserToSearch.Text)
    End Sub
End Class