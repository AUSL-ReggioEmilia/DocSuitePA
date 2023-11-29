Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI

Public Class ContactControl
    Inherits ItemTreeControl(Of Contact)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

    End Sub

    Protected Overrides Function GetSelectedNode() As RadTreeNode
        Return Tree.SelectedNode
    End Function

    Protected Overrides Function GetItemFromId(ByVal idString As String) As Contact
        Dim idContact As Integer = Integer.Parse(idString)
        Return FacadeFactory.Instance.ContactFacade.GetById(idContact)
    End Function

    Public Overrides Function GetItemNodeValue(ByVal item As Contact, ByVal group As String) As String
        Return String.Format(NodeValueTemplate, item.Id, group)
    End Function

    Public Overrides Function GetNodeFromValue(ByVal value As String) As RadTreeNode
        Return Tree.FindNodeByValue(value)
    End Function

    Public Overrides Function GetItemId(ByVal item As Contact) As String
        Return item.Id.ToString()
    End Function

    Public Overrides Function GetItemDescription(ByVal item As Contact) As String
        Return ContactFacade.FormatContact(item)
    End Function

    Public Overrides Function GetItemIcon(ByVal item As Contact) As String
        Return ImagePath.ContactTypeIcon(item.ContactType.Id, item.isLocked.HasValue AndAlso item.isLocked.Value = 1)
    End Function

    Public Overrides Function GetItemParent(ByVal item As Contact) As Contact
        If item.Parent Is Nothing Then
            Return Nothing
        End If
        Return FacadeFactory.Instance.ContactFacade.GetById(item.Parent.Id)
    End Function

    Public Overrides Function GetItems() As IList(Of Contact)
        Dim values As List(Of String) = Tree.GetAllNodes().Select(Function(x) GetNodeId(x)).ToList()
        Dim ids As New List(Of Integer)
        For Each idValue As String In values
            Dim value As Integer
            If Integer.TryParse(idValue, value) Then
                ids.Add(value)
            End If
        Next
        Return FacadeFactory.Instance.ContactFacade.GetListByIds(ids)
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

    Public Overrides Sub NodeCustomization(ByVal node As RadTreeNode, ByVal item As Contact, ByVal explicit As Boolean)
        If explicit Then
            node.Font.Bold = True
        End If
    End Sub
    
    
End Class