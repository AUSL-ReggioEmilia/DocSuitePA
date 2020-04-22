Imports System.Web.UI.WebControls
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports System.Web

''' <summary> Vecchie utility da rimuovere. </summary>
''' <remarks>
''' TODO: rimuovere come tutta la presentation.
''' </remarks>
Public Class WebUtils

    Public Shared Sub ExpandOnClientNodeAttachEvent(ByRef tree As RadTreeView)
        Tree.OnClientNodeClicked = "OnClientNodeClickedExpand"
    End Sub

    ''' <summary> Imposta lo style display:none per l'oggetto passato come parametro </summary>
    ''' <param name="obj">l'oggetto da nascondere</param>
    Public Shared Sub ObjAttDisplayNone(ByVal obj As Object)
        obj.Attributes.Add("style", "display:none;")
    End Sub

    ''' <summary>Metodo osceno per la creazione delle tabelle</summary>
    Public Shared Function ObjTableAdd(ByVal type As String,
                                       ByVal table As Table,
                                       ByVal rowConfiguration As String,
                                       ByVal columnsConfiguration As String,
                                       ByVal lineBox As Boolean,
                                       ByVal cssClass As String,
                                       ByVal rowIndex As Integer?) As Boolean

        ' TODO: sostituire pian piano con TableRowCollectionEx.AddRaw

        Try
            Dim rowArguments As String() = Split(rowConfiguration, "|")
            Dim columnsArguments As String() = Split(columnsConfiguration, "|")

            If rowArguments.Length > columnsArguments.Length Then
                FileLogger.Warn(LogName.FileLog, String.Format("Errore creazione riga [{0}] e [{1}] non compatibili.", rowConfiguration, columnsConfiguration))
            End If

            Dim row As New TableRow
            For i As Integer = 0 To rowArguments.Length - 1
                Dim cell As New TableCell
                Select Case True
                    Case rowArguments(i) = ""
                        cell.Text = "&nbsp;"
                    Case rowArguments(i) = "True"
                        Dim chkbox As New CheckBox
                        chkbox.Checked = True
                        chkbox.Enabled = False
                        cell.Controls.Add(chkbox)
                    Case rowArguments(i) = "False"
                        Dim chkbox As New CheckBox
                        chkbox.Checked = False
                        chkbox.Enabled = False
                        cell.Controls.Add(chkbox)
                    Case Left(rowArguments(i), 3) = "../"
                        Dim s() As String = Split(rowArguments(i), "§")
                        Dim immagine As New Image
                        immagine.ImageUrl = s(0)
                        Select Case UBound(s)
                            Case 1
                                immagine.ToolTip = s(1)
                            Case 2
                                immagine.ToolTip = s(1)
                                If InStr(s(2), "document.location") > 0 Then
                                    immagine.Attributes.Add("onclick", s(2) & "; return false;")
                                    immagine.Attributes.Add("style", "cursor:hand;")
                                Else
                                    ' Nessuna parte del codice dovrebbe portare qui, lascio errore per correttezza
                                    ' se si presentasse il problema aprire la pagina con le telerik
                                    Throw New NotImplementedException("Impossibile aprire la pagina " & s(2))
                                End If
                        End Select
                        cell.Controls.Add(immagine)
                    Case Else
                        cell.Text = rowArguments(i)
                End Select

                If UBound(rowArguments) = 0 Then
                    cell.ColumnSpan = UBound(columnsArguments) + 1
                End If

                If Not String.IsNullOrEmpty(cssClass) Then
                    cell.CssClass = cssClass
                End If

                Dim cellArguments As String() = Split(columnsArguments(i), "-")
                cell.Width = Unit.Percentage(cellArguments(0))
                Select Case cellArguments(1)
                    Case "B"
                        cell.Font.Bold = True
                    Case "X"
                        cell.CssClass = "label"
                End Select

                Select Case cellArguments(2)
                    Case "L", ""
                        cell.HorizontalAlign = HorizontalAlign.Left
                    Case "R"
                        cell.HorizontalAlign = HorizontalAlign.Right
                    Case "C"
                        cell.HorizontalAlign = HorizontalAlign.Center
                End Select
                If cellArguments.Length - 1 > 2 Then
                    If IsNumeric(cellArguments(3)) Then
                        cell.ColumnSpan = cellArguments(3)
                    Else
                        Select Case cellArguments(3)
                            Case "T" : cell.VerticalAlign = VerticalAlign.Top
                            Case "M" : cell.VerticalAlign = VerticalAlign.Middle
                            Case "B" : cell.VerticalAlign = VerticalAlign.Bottom
                            Case "W" : cell.Wrap = False
                        End Select
                    End If
                End If

                If lineBox Then
                    cell.BorderStyle = BorderStyle.Solid
                    cell.BorderWidth = Unit.Pixel(1)
                Else
                    cell.BorderStyle = BorderStyle.None
                    cell.BorderWidth = Unit.Pixel(0)
                End If
                row.Cells.Add(cell)
            Next (i)

            If Not String.IsNullOrEmpty(cssClass) Then
                row.CssClass = cssClass
            Else
                row.CssClass = If((table.Rows.Count Mod 2) = 0, "Chiaro", "Scuro")
            End If

            If rowIndex.HasValue Then
                table.Rows.AddAt(rowIndex.Value, row)
            Else
                table.Rows.Add(row)
            End If

        Catch ex As Exception
            FileLogger.Error(LogName.FileLog, String.Format("Errore creazione riga [{0}] e [{1}].", rowConfiguration, columnsConfiguration))
            Return False
        End Try
        Return True
    End Function

    Public Shared Function ObjTreeViewExistNode(ByVal tvw As RadTreeView, ByVal id As String, ByVal testo As String, Optional ByRef refNode As RadTreeNode = Nothing) As Boolean
        Dim tn As RadTreeNode
        Dim b As Boolean
        ObjTreeViewExistNode = False
        If id = "" And testo = "" Then Exit Function
        For Each tn In tvw.Nodes
            ObjTreeViewExistNodeM(tn, id, testo, b, RefNode)
            If b Then Exit For
        Next tn
        ObjTreeViewExistNode = b
    End Function

    Public Shared Sub ObjTreeViewExistNodeM(ByVal tn As RadTreeNode, ByVal id As String, ByVal testo As String, ByRef b As Boolean, ByRef refNode As RadTreeNode)
        Dim tn2 As RadTreeNode
        Select Case True
            Case Id <> ""
                If tn.Value = Id Then b = True
            Case testo <> ""
                If UCase(tn.Text) = UCase(testo) Then b = True
        End Select
        If b Then
            refNode = tn
            Exit Sub
        Else
            For Each tn2 In tn.Nodes
                ObjTreeViewExistNodeM(tn2, Id, testo, b, refNode)
                If b Then Exit For
            Next tn2
        End If
    End Sub

    Public Shared Function ObjDropDownListAdd(ByVal ddl As DropDownList, ByVal text As String, ByVal value As String, Optional ByVal selected As Boolean = False) As Boolean
        Dim itm As New ListItem
        itm.Text = text
        itm.Value = value
        itm.Selected = selected
        ddl.Items.Add(itm)
    End Function

    Public Shared Function ObjTreeViewRoleAdd(ByRef tree As RadTreeView, ByVal nodoFiglio As RadTreeNode, _
              ByRef role As Role, _
              ByVal existRootNode As Boolean, _
              ByVal nodeExpanded As Boolean, _
              ByVal nodeBold As Boolean, _
               ByVal nodeColor As String, _
              ByVal checkBox As Boolean, _
              ByVal checkValue As Boolean) As Boolean

        Dim nodoToAdd As New RadTreeNode
        Dim nodoExist As New RadTreeNode

        nodoToAdd.Checkable = False

        If Not IsNothing(role) Then
            nodoToAdd.Text = role.Name.ToString()
            nodoToAdd.Value = role.Id
            If Not ObjTreeViewExistNode(tree, nodoToAdd.Value, "", nodoExist) Then
                If IsNothing(role.Father) Then
                    If existRootNode Then
                        tree.Nodes(0).Nodes.Add(nodoToAdd)
                    Else
                        tree.Nodes.Add(nodoToAdd)
                    End If
                    ' TODO: rimuovere o trasformare in stile
                    nodoToAdd.ImageUrl = VirtualPathUtility.ToAbsolute("~/App_Themes/DocSuite2008/imgset16/bricks.png")
                Else
                    ObjTreeViewRoleAdd(tree, nodoToAdd, role.Father, existRootNode, nodeExpanded, True, "", False, False)
                    ' TODO: rimuovere o trasformare in stile
                    nodoToAdd.ImageUrl = VirtualPathUtility.ToAbsolute("~/App_Themes/DocSuite2008/imgset16/brick.png")
                End If
                If role.IsActive <> 1 Then
                    nodoToAdd.Style.Add("color", "gray")
                Else
                    nodoToAdd.Style.Remove("color")
                End If
            Else
                nodoToAdd = nodoExist
            End If
            nodoToAdd.Expanded = nodeExpanded
        End If
        If Not IsNothing(nodoFiglio) Then
            nodoToAdd.Nodes.Add(nodoFiglio)
        Else
            If nodeBold Then nodoToAdd.Style.Add("font-weight", "bold")
            If nodeColor <> "" Then nodoToAdd.Style.Add("color", nodeColor)
            If checkBox Then
                nodoToAdd.Checkable = True
                nodoToAdd.Checked = checkValue
            Else
                nodoToAdd.Checkable = False
            End If
        End If

    End Function

End Class
