Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports Telerik.Web.UI

Public MustInherit Class ItemTreeControl(Of T)
    Inherits BaseControl

    Public Event ItemsAdded(sender As Object, args As ItemControlEventArgs(Of T))
    Public Event ItemsRemoved(sender As Object, args As ItemControlEventArgs(Of T))
    Public Event ItemSelected(sender As Object, args As ItemControlEventArgs(Of T))
    Public Event ItemsAdding(sender As Object, args As ItemControlEventArgs(Of T))
    Public Event ItemsRemoving(sender As Object, args As ItemControlEventArgs(Of T))
    Public Event NodeCreated(sender As Object, args As ItemNodeEventArgs(Of T))

#Region " Fields "

    Protected Const NodeValueTemplate As String = "IdItem[{0}]Group[{1}]"
    Protected Const NodeGroupValueTemplate As String = "Group[{0}]"
    Protected Const AttributeIdItem As String = "IdItem"
    Protected Const AttributeExplicit As String = "Explicit"
    Protected Const AttributeValueExplicitTrue As String = "1"
    Protected Const AttributeGroup As String = "Group"

#End Region

#Region " Properties "

    Public Property DefaultGroup() As String
        Get
            Return DirectCast(ViewState("DefaultGroup"), String)
        End Get
        Set(value As String)
            ViewState("DefaultGroup") = value
        End Set
    End Property

#End Region

#Region " Methods "

    Protected MustOverride Function GetSelectedNode() As RadTreeNode
    Protected MustOverride Function GetItemFromId(idString As String) As T
    Public MustOverride Function GetItemNodeValue(item As T, group As String) As String
    Public MustOverride Function GetNodeFromValue(value As String) As RadTreeNode
    Public MustOverride Function GetItemId(item As T) As String
    Public MustOverride Function GetItemDescription(item As T) As String
    Public MustOverride Function GetItemIcon(item As T) As String
    Public MustOverride Function GetItemParent(item As T) As T
    Public MustOverride Function GetItems() As IList(Of T)
    Public MustOverride Sub AddNodeToTree(node As RadTreeNode)
    Public MustOverride Sub AddGroupNodeToTree(node As RadTreeNode)
    Public MustOverride Sub Clear()
    Public MustOverride Sub NodeCustomization(node As RadTreeNode, item As T, explicit As Boolean)

    Public Function GetSelectedItem() As T
        Dim node As RadTreeNode = GetSelectedNode()
        If node Is Nothing Then
            Return Nothing
        End If
        Dim idValue As String = GetNodeId(GetSelectedNode())
        If String.IsNullOrEmpty(idValue) Then
            Return Nothing
        End If
        Dim item As T = GetItemFromId(idValue)
        Return item
    End Function

    Public Function GetNodeId(ByVal node As RadTreeNode) As String
        Return node.Attributes(AttributeIdItem)
    End Function

    Public Overridable Function CreateNode(item As T, group As String, explicit As Boolean, disabled As Boolean) As RadTreeNode

        Dim node As New RadTreeNode()
        node.ImageUrl = GetItemIcon(item)
        node.Attributes.Add(AttributeIdItem, GetItemId(item))
        node.Attributes.Add(AttributeGroup, group)
        If explicit Then
            node.Attributes.Add(AttributeExplicit, AttributeValueExplicitTrue)
        End If
        If disabled Then
            node.CssClass = "notActive"
        End If
        node.Expanded = True
        node.Checkable = False
        node.Text = GetItemDescription(item)
        node.Value = GetItemNodeValue(item, group)

        NodeCustomization(node, item, explicit)


        Return node
    End Function

    Public Overridable Sub PosizionaNodo(node As RadTreeNode, item As T, group As String, disabled As Boolean)

        Dim parentItem As T = GetItemParent(item)

        If parentItem Is Nothing Then
            If String.IsNullOrEmpty(group) Then
                AddNodeToTree(node)
            Else
                GetGroupNode(group).Nodes.Add(node)
            End If
        Else
            ' Verifico se c'è il padre
            Dim parentValue As String = GetItemNodeValue(parentItem, group)
            Dim parentNode As RadTreeNode = GetNodeFromValue(parentValue)
            If parentNode Is Nothing Then
                parentNode = CreateNode(parentItem, group, False, disabled)
                PosizionaNodo(parentNode, parentItem, group, disabled)
            End If
            parentNode.Nodes.Add(node)
        End If
    End Sub

    Private Function GetGroupNode(group As String) As RadTreeNode
        Dim nodeValue As String = String.Format(NodeGroupValueTemplate, group)
        Dim nodeGroup As RadTreeNode = GetNodeFromValue(nodeValue)
        If nodeGroup Is Nothing Then
            nodeGroup = New RadTreeNode()
            nodeGroup.Expanded = True
            nodeGroup.Text = group
            nodeGroup.Value = nodeValue
            AddGroupNodeToTree(nodeGroup)
        End If
        Return nodeGroup
    End Function

    Protected Sub OnItemAdding(args As ItemControlEventArgs(Of T))
        RaiseEvent ItemsAdding(Me, args)
    End Sub
    Protected Sub OnItemAdded(args As ItemControlEventArgs(Of T))
        RaiseEvent ItemsAdded(Me, args)
    End Sub

    Protected Sub OnItemRemoving(args As ItemControlEventArgs(Of T))
        RaiseEvent ItemsRemoving(Me, args)
    End Sub
    Protected Sub OnItemRemoved(args As ItemControlEventArgs(Of T))
        RaiseEvent ItemsRemoved(Me, args)
    End Sub

    Public Function IsExplicit(item As T) As Boolean
        Return IsExplicit(item, DefaultGroup)
    End Function

    Public Function IsExplicit(item As T, group As String) As Boolean
        Dim val As String = GetItemNodeValue(item, group)
        Dim node As RadTreeNode = GetNodeFromValue(val)
        Return IsExplicit(node)
    End Function
    Public Function IsExplicit(node As RadTreeNode) As Boolean
        Dim av As String = node.Attributes(AttributeExplicit)
        Return Not String.IsNullOrEmpty(av) AndAlso av.Eq(AttributeValueExplicitTrue)
    End Function

    Public Sub RemoveItem(item As T)
        RemoveItem(item, DefaultGroup)
    End Sub

    Public Sub RemoveItem(item As T, group As String)
        Dim val As String = GetItemNodeValue(item, group)
        Dim node As RadTreeNode = GetNodeFromValue(val)
        If node Is Nothing Then
            Return
        End If
        ' Non posso cancellare nodi non espliciti
        If Not IsExplicit(item) Then
            Return
        End If

        Dim args As New ItemControlEventArgs(Of T)(item, group)
        OnItemRemoving(args)

        If args.Cancel Then
            Return
        End If
        Dim parentNode As RadTreeNode = node.ParentNode
        node.Remove()

        ClearImplicitNodes(parentNode)

        OnItemRemoved(New ItemControlEventArgs(Of T)(item, group))
    End Sub

    Public Sub ClearImplicitNodes(node As RadTreeNode)
        If node Is Nothing Then
            Return
        End If
        Dim parentNode As RadTreeNode = node.ParentNode

        If Not IsExplicit(node) AndAlso node.Nodes.Count = 0 Then
            node.Remove()
            ClearImplicitNodes(parentNode)
        End If
    End Sub

    Public Sub AddItem(item As T, disabled As Boolean)
        AddItem(item, DefaultGroup, disabled)
    End Sub

    Public Sub AddItems(items As IList(Of T), IsDisabled As Func(Of T, Boolean))
        AddItems(items, DefaultGroup, IsDisabled)
    End Sub

    Public Sub AddItems(items As IList(Of T), group As String, IsDisabled As Func(Of T, Boolean))
        For Each item As T In items
            AddItem(item, group, IsDisabled(item))
        Next
    End Sub

    Public Sub AddItem(item As T, group As String, disabled As Boolean)
        Dim argsAdding As New ItemControlEventArgs(Of T)(item, group)
        OnItemAdding(argsAdding)
        If argsAdding.Cancel Then
            Return
        End If

        OnItemAdded(New ItemControlEventArgs(Of T)(item, group))

        LoadItem(item, group, disabled)
    End Sub

    Public Sub LoadItems(items As IEnumerable(Of T), IsDisabled As Func(Of T, Boolean))
        LoadItems(items, DefaultGroup, IsDisabled)
    End Sub

    Public Sub LoadItems(items As IEnumerable(Of T), group As String, IsDisabled As Func(Of T, Boolean))
        For Each item As T In items
            LoadItem(item, group, IsDisabled(item))
        Next
    End Sub

    Public Sub LoadItem(item As T, group As String, disabled As Boolean)
        Dim node As RadTreeNode = CreateNode(item, group, True, disabled)

        Dim args As New ItemNodeEventArgs(Of T)(item, node, group)
        RaiseEvent NodeCreated(Me, args)

        PosizionaNodo(node, item, args.Group, disabled)
    End Sub

    Public Function Contains(item As T, group As String) As Boolean
        Dim val As String = GetItemNodeValue(item, group)
        Return GetNodeFromValue(val) IsNot Nothing
    End Function

#End Region

End Class