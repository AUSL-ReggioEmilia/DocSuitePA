Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data

Public Class PECMailBoxControl
    Inherits ItemTreeControl(Of PECMailBox)

    Protected Overrides Function GetSelectedNode() As RadTreeNode
        Return Tree.SelectedNode
    End Function

    Protected Overrides Function GetItemFromId(ByVal idString As String) As PECMailBox
        Dim idRole As Short = Short.Parse(idString)
        Return FacadeFactory.Instance.PECMailboxFacade.GetById(idRole)
    End Function

    Public Overrides Function GetItemNodeValue(ByVal item As PECMailBox, ByVal group As String) As String
        Return String.Format(NodeValueTemplate, item.Id, group)
    End Function

    Public Overrides Function GetNodeFromValue(ByVal value As String) As RadTreeNode
        Return Tree.FindNodeByValue(value)
    End Function

    Public Overrides Function GetItemId(ByVal item As PECMailBox) As String
        Return item.Id.ToString()
    End Function

    Public Overrides Function GetItemDescription(ByVal item As PECMailBox) As String
        Return item.MailBoxName
    End Function

    Public Overrides Function GetItemIcon(ByVal item As PECMailBox) As String
        Return ImagePath.SmallMailBox
    End Function

    Public Overrides Function GetItemParent(ByVal item As PECMailBox) As PECMailBox
        Return Nothing
    End Function

    Public Overrides Function GetItems() As IList(Of PECMailBox)
        Dim values As List(Of String) = Tree.GetAllNodes().Select(Function(x) GetNodeId(x)).ToList()
        Dim ids As New List(Of Short)
        For Each idValue As String In values
            Dim value As Short
            If Short.TryParse(idValue, value) Then
                ids.Add(value)
            End If
        Next
        Return FacadeFactory.Instance.PECMailboxFacade.GetListByIds(ids)
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

    Public Overrides Sub NodeCustomization(ByVal node As RadTreeNode, ByVal item As PECMailBox, ByVal explicit As Boolean)
        ' noop
    End Sub
End Class
