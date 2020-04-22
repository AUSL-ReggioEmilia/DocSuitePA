Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data

Public Class RoleControl
    Inherits ItemTreeControl(Of Role)
    Public Property HistoricizeDate As Date?

    Protected Overrides Function GetSelectedNode() As RadTreeNode
        Return Tree.SelectedNode
    End Function

    Protected Overrides Function GetItemFromId(ByVal idString As String) As Role
        Dim idRole As Integer = Integer.Parse(idString)
        Dim role As Role = FacadeFactory.Instance.RoleFacade.GetById(idRole)
        Return role
    End Function

    Public Overrides Function GetItemNodeValue(ByVal item As Role, ByVal group As String) As String
        Return String.Format(NodeValueTemplate, item.Id, group)
    End Function

    Public Overrides Function GetNodeFromValue(ByVal value As String) As RadTreeNode
        Return Tree.FindNodeByValue(value)
    End Function

    Public Overrides Function GetItemId(ByVal item As Role) As String
        Return item.Id.ToString()
    End Function

    Public Overrides Function GetItemDescription(ByVal item As Role) As String
        Dim name As String = item.Name
        If (HistoricizeDate.HasValue AndAlso item.RoleNames IsNot Nothing AndAlso item.RoleNames.Any()) Then
            name = item.RoleNames.SingleOrDefault(Function(f) f.FromDate <= HistoricizeDate.Value AndAlso (Not f.ToDate.HasValue OrElse (f.ToDate.HasValue AndAlso HistoricizeDate.Value <= f.ToDate.Value))).Name
        End If
        Return name
    End Function

    Public Overrides Function GetItemIcon(ByVal item As Role) As String
        Return ImagePath.SmallSubRole
    End Function

    Public Overrides Function GetItemParent(ByVal item As Role) As Role
        If item.Father Is Nothing Then
            Return Nothing
        End If
        Return FacadeFactory.Instance.RoleFacade.GetById(item.Father.Id)
    End Function

    Public Overrides Function GetItems() As IList(Of Role)
        Dim values As List(Of String) = Tree.GetAllNodes().Where(Function(n) n.Font.Bold).Select(Function(x) GetNodeId(x)).ToList()
        Dim ids As New List(Of Integer)
        For Each idValue As String In values
            Dim value As Integer
            If Integer.TryParse(idValue, value) Then
                ids.Add(value)
            End If
        Next
        Return FacadeFactory.Instance.RoleFacade.GetByIds(ids)
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

    Public Overrides Sub NodeCustomization(ByVal node As RadTreeNode, ByVal item As Role, ByVal explicit As Boolean)
        If explicit Then
            node.Font.Bold = True
        End If
    End Sub

End Class