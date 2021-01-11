Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods

Public Class ExportHelper
    Private Shared Sub ExportToContentType(ByRef control As Control, ByRef page As Page, ByVal fileName As String, ByVal contentType As String)
        page.Response.Clear()
        page.Response.Buffer = True
        page.Response.ContentType = contentType
        page.Response.Charset = "ISO-8859-1" '"UTF-8"
        If fileName <> "" Then
            page.Response.AddHeader("content-disposition", String.Format("attachment;filename={0}", fileName))
        End If
        page.Response.Charset = ""
        page.EnableViewState = False
        Dim oStringWriter As New System.IO.StringWriter
        Dim oHtmlTextWriter As New System.Web.UI.HtmlTextWriter(oStringWriter)
        control.RenderControl(oHtmlTextWriter)
        page.Response.Write(oStringWriter.ToString())
        page.Response.End()
    End Sub

    Public Shared Sub ExportToExcel(ByRef control As Control, ByRef page As Page, Optional ByVal fileName As String = "")
        'export to excel
        Dim contentType As String = "application/msexcel"
        ExportToContentType(control, page, fileName, contentType)
    End Sub

    Public Shared Sub ExportToWord(ByRef control As Control, ByRef page As Page, Optional ByVal fileName As String = "")
        'export to excel
        Dim contentType As String = "application/msword"
        ExportToContentType(control, page, fileName, contentType)
    End Sub

    Public Shared Function TableFromGrid(gridColumns As GridColumnCollection, gridItems As GridDataItemCollection, Optional border As Boolean = True) As Table
        Dim table As DSTable = New DSTable()
        table.Table.Style.Add("border-collapse", "collapse")

        Dim index As Integer = -1
        Dim manageableColumns As IDictionary(Of Integer, GridColumn) = gridColumns.Cast(Of GridColumn) _
            .ToDictionary(Function(k)
                              index += 1
                              Return index
                          End Function, Function(k) k).Where(Function(c) c.Value.Visible AndAlso c.Value.Display AndAlso Not c.Value.UniqueName.Eq("ClientSelectColumn")).ToDictionary(Function(i) i.Key, Function(c) c.Value)
        Dim maxTableWidth As Double = manageableColumns.Sum(Function(c) c.Value.HeaderStyle.Width.Value)

        'Creo l'intestazione
        table.CreateEmptyRow()
        Dim cellHeaderStyle As DSTableCellStyle
        Dim cellPercentageWidth As Double = 0
        For Each column As GridColumn In manageableColumns.Values
            cellHeaderStyle = New DSTableCellStyle()
            cellPercentageWidth = (column.HeaderStyle.Width.Value / maxTableWidth) * 100
            cellHeaderStyle.Width = Unit.Percentage(cellPercentageWidth)
            cellHeaderStyle.Font.Bold = True
            cellHeaderStyle.LineBox = border

            table.CurrentRow.CreateEmpytCell()
            table.CurrentRow.CurrentCell.Text = column.HeaderText
            table.CurrentRow.CurrentCell.ApplyStyle(cellHeaderStyle)
        Next

        Dim cellItemStyle As DSTableCellStyle
        For Each gridDataItem As GridDataItem In gridItems
            table.CreateEmptyRow()
            For Each column As KeyValuePair(Of Integer, GridColumn) In manageableColumns
                If column.Key <= gridDataItem.Cells.Count Then
                    cellItemStyle = New DSTableCellStyle()
                    cellItemStyle.LineBox = border

                    table.CurrentRow.CreateEmpytCell()
                    ' Più offsetColumn per evitare le colonne inziali
                    table.CurrentRow.CurrentCell.Text = CellToString(column.Value, gridDataItem, gridDataItem.Cells(column.Key + 2))
                    table.CurrentRow.CurrentCell.ApplyStyle(cellItemStyle)
                End If
            Next
        Next

        Return table.Table
    End Function

    Private Shared Function CellToString(column As GridColumn, dataItem As GridDataItem, ByRef cell As TableCell) As String
        Dim retval As String = String.Empty

        Select Case column.ColumnType
            Case "CompositeTemplateExportableColumn"
                Dim exportableColumn As IExportableColumn = DirectCast(column, IExportableColumn)
                retval = exportableColumn.GetExportableValue(dataItem)
            Case Else
                If cell.Controls.Count > 1 Then
                    Select Case cell.Controls(1).GetType.ToString
                        Case "System.Web.UI.WebControls.Image", "System.Web.UI.WebControls.ImageButton"
                            retval = ImageToString(ReflectionHelper.GetProperty(cell.Controls(1), "ImageUrl").ToString())
                        Case "Telerik.Web.UI.RadButton"
                            Dim radButton As RadButton = DirectCast(cell.Controls(1), RadButton)
                            If Not String.IsNullOrEmpty(radButton.Image.ImageUrl) Then
                                retval = ImageToString(radButton.Image.ImageUrl)
                            End If
                        Case Else
                            retval = ReflectionHelper.GetProperty(cell.Controls(1), "Text").ToString()
                    End Select
                ElseIf cell.Controls IsNot Nothing _
                AndAlso cell.Controls.Count > 0 _
                AndAlso String.Compare(cell.Controls(0).GetType().ToString(), "System.Web.UI.DataBoundLiteralControl", True).Equals(0) Then
                    retval = DirectCast(cell.Controls.Item(0), DataBoundLiteralControl).Text
                Else
                    retval = cell.Text
                End If
        End Select

        Return retval
    End Function

    Private Shared Function ImageToString(ByVal path As String) As String
        Dim sRet As String = String.Empty
        Dim filename As String = System.IO.Path.GetFileNameWithoutExtension(path)

        Select Case filename.ToUpper
            Case "MAIL16_U", "MAIL32_U"
                sRet = "Uscita"
            Case "MAIL16_I", "MAIL32_I"
                sRet = "Ingresso"
            Case "MAIL16_IU", "MAIL32_IU"
                sRet = "Ingresso/Uscita"
            Case "DOCMANNULLA"
                sRet = "Pratica Annullata"
            Case "DOCMARCHIVIA"
                sRet = "Pratica Archiviata"
            Case "DOCMCHIUSURA"
                sRet = "Pratica Chiusura"
            Case "DOCMRIAPERTURA"
                sRet = "Pratica Riapertura"
            Case "ATTOANNULLATO"
                sRet = "Atto Annullato"
            Case "DELIBERAANNULLATA"
                sRet = "Delibera Annullata"
                'Cartella DocSuite
            Case "ATTO16", "ATTO32", "RESOLUTION16"
                sRet = "Atto"
            Case "COLLEGAMENTO16", "COLLEGAMENTO32"
                sRet = "Collegamento"
            Case "DELIBERA16", "DELIBERA32"
                sRet = "Delibera"
            Case "FASCICOLI16", "FASCICOLI32", "FASCICLE_OPEN"
                sRet = "Fascicoli"
            Case "FASCICOLOCLOSED16", "FASCICOLOCLOSED32"
                sRet = "Fascicolo Chiuso"
            Case "PRATICA16", "PRATICA32"
                sRet = "Pratica"
            Case "PRATICHE16", "PRATICHE32"
                sRet = "Pratiche"
                'Cartella File
            Case "ACROBAT16", "ACROBAT32"
                sRet = "PDF"
            Case "ALLEGATI16", "ALLEGATI32"
                sRet = "Allegati"
            Case "ATTI16", "ATTI32"
                sRet = "Atti"
            Case "CHECKOUT16", "CHECKOUT32"
                sRet = "CheckOut"
            Case "DIGITALSIGN16", "DIGITALSIGN32"
                sRet = "Firmato"
            Case "EXCEL16", "EXCEL32"
                sRet = "Excel"
            Case "EXPLORERHTM16", "EXPLORERHTM32"
                sRet = "HTML"
            Case "EXPLORERMHT16", "EXPLORERMHT32"
                sRet = "MHT"
            Case "FASCICOLO16", "FASCICOLO32"
                sRet = "Fascicolo"
            Case "IMAGE16", "IMAGE32"
                sRet = "Immagine"
            Case "MAIL16", "MAIL32"
                sRet = "Mail"
            Case "NONE16", "NONE32"
                sRet = String.Empty
            Case "NOTEPAD16", "NOTEPAD32"
                sRet = "Testo/ASCII"
            Case "POWERPOINT16", "POWERPOINT32"
                sRet = "PowerPoint"
            Case "PROTOCOLLO16", "PROTOCOLLO32"
                sRet = "Protocollo"
            Case "VERSION16", "VERSION32"
                sRet = "Versione"
            Case "WINWORD16", "WINWORD32"
                sRet = "Word"
            Case "XML16", "XML32"
                sRet = "XML"
            Case Else
                sRet = filename
        End Select

        Return sRet
    End Function

End Class
