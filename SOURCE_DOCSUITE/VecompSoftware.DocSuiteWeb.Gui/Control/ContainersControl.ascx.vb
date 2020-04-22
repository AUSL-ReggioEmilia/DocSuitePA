Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data

Public Class ContainersControl
    Inherits ItemTreeControl(Of Container)
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

    End Sub

    Protected Overrides Function GetSelectedNode() As RadTreeNode
        Return Tree.SelectedNode
    End Function

    Public Function SelectedNode() As RadTreeNode
        Return Tree.SelectedNode
    End Function

    Protected Overrides Function GetItemFromId(ByVal idString As String) As Container
        Dim idContact As Integer = Integer.Parse(idString)
        Return FacadeFactory.Instance.ContainerFacade.GetById(idContact)
    End Function

    Public Overrides Function GetItemNodeValue(ByVal item As Container, ByVal group As String) As String
        Return String.Format(NodeValueTemplate, item.Id, group)
    End Function

    Public Overrides Function GetNodeFromValue(ByVal value As String) As RadTreeNode
        Return Tree.FindNodeByValue(value)
    End Function

    Public Overrides Function GetItemId(ByVal item As Container) As String
        Return item.Id.ToString()
    End Function
    Public Overrides Function GetItemDescription(ByVal item As Container) As String
        Return item.Name
    End Function

    Public Overrides Function GetItemIcon(ByVal item As Container) As String
        Return ImagePath.SmallBoxOpen
    End Function
    
    Public Overrides Function GetItemParent(ByVal item As Container) As Container
        Return Nothing
    End Function

    Public Overrides Function GetItems() As IList(Of Container)
        Dim values As List(Of String) = Tree.GetAllNodes().Select(Function(x) GetNodeId(x)).ToList()
        Dim ids As New List(Of Integer)
        For Each idValue As String In values
            Dim value As Integer
            If Integer.TryParse(idValue, value) Then
                ids.Add(value)
            End If
        Next
        Return FacadeFactory.Instance.ContainerFacade.GetListByIds(ids)
    End Function

    Public Overrides Sub AddNodeToTree(ByVal node As RadTreeNode)
        Tree.Nodes.Add(node)
    End Sub

    Public Overrides Sub AddGroupNodeToTree(ByVal node As RadTreeNode)
        Tree.Nodes.Insert(0, node)
    End Sub

    Public Overrides Sub Clear()
        Tree.Nodes.Clear()
    End Sub

    Public Overrides Sub NodeCustomization(ByVal node As RadTreeNode, ByVal item As Container, ByVal explicit As Boolean)
        ' noop
    End Sub

End Class
