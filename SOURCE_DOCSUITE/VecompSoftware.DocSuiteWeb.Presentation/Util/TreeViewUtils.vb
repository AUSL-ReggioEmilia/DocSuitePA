Imports VecompSoftware.Helpers
Imports Telerik.Web.UI

Public Class TreeViewUtils
    Public Shared Sub PerformDragAndDrop(ByRef dropPosition As RadTreeViewDropPosition, ByRef sourceNode As RadTreeNode, ByRef destNode As RadTreeNode)
        If (sourceNode.Equals(destNode) OrElse sourceNode.IsAncestorOf(destNode)) Then
            Return
        End If
        sourceNode.Owner.Nodes.Remove(sourceNode)
        Select Case (dropPosition)
            Case RadTreeViewDropPosition.Over
                ' child
                If Not sourceNode.IsAncestorOf(destNode) Then
                    destNode.Nodes.Add(sourceNode)
                End If
            Case RadTreeViewDropPosition.Above
                ' sibling - above                    
                destNode.InsertBefore(sourceNode)
            Case RadTreeViewDropPosition.Below
                ' sibling - below
                destNode.InsertAfter(sourceNode)
        End Select
    End Sub

    Public Shared Function CreateMenuItem(ByVal text As String, ByVal value As String, ByVal width As System.Web.UI.WebControls.Unit, Optional ByVal imageUrl As String = "") As RadMenuItem
        Dim item As New RadMenuItem()
        item.Text = text
        item.Value = value
        item.Width = width
        item.ImageUrl = imageUrl

        Return item
    End Function

    Public Shared Sub ChangeNodesForeColor(ByRef rootNode As RadTreeNode, ByVal color As System.Drawing.Color)
        rootNode.ForeColor = color
        For Each child As RadTreeNode In rootNode.Nodes
            ChangeNodesForeColor(child, color)
        Next
    End Sub

    'SortNodes is a recursive method enumerating and sorting all node levels 
    Public Shared Sub SortNodes(ByVal collection As RadTreeNodeCollection, Optional ByVal orderClause As String = "Text ASC")
        Sort(collection, orderClause)
        For Each node As RadTreeNode In collection
            If node.Nodes.Count > 0 Then
                SortNodes(node.Nodes)
            End If
        Next
    End Sub

    'The Sort method is called for each node level sorting the child nodes 
    Private Shared Sub Sort(ByVal collection As RadTreeNodeCollection, Optional ByVal orderClause As String = "Text ASC")
        Dim nodes As RadTreeNode() = New RadTreeNode(collection.Count - 1) {}
        collection.CopyTo(nodes, 0)
        Dim comparer As DynamicComparer(Of RadTreeNode) = New DynamicComparer(Of RadTreeNode)(orderClause)
        Array.Sort(nodes, comparer)
        collection.Clear()
        collection.AddRange(nodes)
    End Sub

End Class
