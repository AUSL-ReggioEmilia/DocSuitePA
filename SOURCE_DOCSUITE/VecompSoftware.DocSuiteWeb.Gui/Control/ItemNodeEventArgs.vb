Imports Telerik.Web.UI

Public Class ItemNodeEventArgs(Of T)
    Inherits EventArgs

    Public Property Item As T
    Public Property Node As RadTreeNode
    Public Property Group As String

    Public Sub New(item As T, node As RadTreeNode, group As String)
        Me.Item = item
        Me.Node = node
        Me.Group = group
    End Sub

End Class