Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.Commons
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Commons

Partial Public Class SelCategoryWindow
    Inherits CommBasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If (Not Page.IsPostBack And Not Page.IsCallback) Then
            LoadRootNodes()
            Session("treeViewState") = RadTreeCategory.GetXml()
        End If
    End Sub

    Private Sub LoadRootNodes()
        Dim categoryFinder As CategoryFinder = New CategoryFinder(New MapperCategoryModel(), DocSuiteContext.Current.User.FullUserName)
        categoryFinder.EnablePaging = False
        Dim categoryList As ICollection(Of Category) = categoryFinder.DoSearch()

        For Each category As Category In categoryList
            Dim node As New RadTreeNode()
            node.Text = category.Name
            node.Value = category.Id.ToString()
            node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack
            RadTreeCategory.Nodes.Add(node)
        Next category
    End Sub

    Private Sub AddChildNodes(ByVal node As RadTreeNode)
        Dim containerFacade As New CategoryFacade()

        Dim categoryList As IList(Of Category) = containerFacade.GetCategoryByParentId(node.Value)

        For Each category As Category In categoryList
            Dim childNode As New RadTreeNode()
            childNode.Text = category.Name
            childNode.Value = category.Id.ToString()
            childNode.ExpandMode = TreeNodeExpandMode.ServerSideCallBack
            node.Nodes.Add(childNode)
        Next category
    End Sub

    Private Sub RadTreeCategory_NodeExpand(ByVal o As Object, ByVal e As RadTreeNodeEventArgs) Handles RadTreeCategory.NodeExpand
        AddChildNodes(e.Node)

        Dim treeViewState As String = CStr(Session("treeViewState"))
        Dim cachedTreeView As New RadTreeView()
        cachedTreeView.LoadXmlString(treeViewState)

        Dim cachedNodeClicked As RadTreeNode = cachedTreeView.FindNodeByValue(e.Node.Value)
        AddChildNodes(cachedNodeClicked)
        cachedNodeClicked.ExpandMode = TreeNodeExpandMode.ClientSide
        cachedNodeClicked.Expanded = True

        Session("treeViewState") = cachedTreeView.GetXml()
    End Sub

End Class