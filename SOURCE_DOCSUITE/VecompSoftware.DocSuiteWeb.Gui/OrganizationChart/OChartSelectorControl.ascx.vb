Imports System.Collections.Generic
Imports System.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.API
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.ExtensionMethods


Public Class OChartSelectorControl
    Inherits DocSuite2008BaseControl

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            BindDropDownProviders()
        End If
    End Sub

    Private Sub ButtonSearch_Click(sender As Object, e As EventArgs) Handles ButtonSearch.Click
        Dim filter As String = TextBoxFilter.Text.Trim()
        BindTreeViewItems(filter)
    End Sub
    Private Sub ButtonConfirm_Click(sender As Object, e As EventArgs) Handles ButtonConfirm.Click

    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(ToolBar, TreeViewSelectedItems)
        AjaxManager.AjaxSettings.AddAjaxSetting(ToolBar, WindowSelector)
        AjaxManager.AjaxSettings.AddAjaxSetting(ButtonSearch, TreeViewItems)
        AjaxManager.AjaxSettings.AddAjaxSetting(ButtonConfirm, TreeViewSelectedItems)
        AjaxManager.AjaxSettings.AddAjaxSetting(ButtonConfirm, WindowSelector)
    End Sub
    Private Sub BindDropDownProviders()
        Dim providers As IEnumerable(Of APIProvider) = FacadeFactory.Instance.APIProviderFacade.GetAll().Where(Function(p) p.IsEnabled).OrderBy(Function(p) p.Title)

        DropDownProviders.DataSource = providers
        DropDownProviders.DataTextField = "Title"
        DropDownProviders.DataValueField = "Address"
        DropDownProviders.DataBind()
    End Sub

    Private Function GetNodeText(item As IOChartItemDTO) As String
        If String.IsNullOrEmpty(item.Mailboxes) Then
            Return String.Format("{0} - {1}", item.Code, item.Description)
        End If
        Return String.Format("{0} - {1} ({2})", item.Code, item.Description, item.Mailboxes)
    End Function
    Private Function GetNode(item As IOChartItemDTO) As RadTreeNode
        Dim node As New RadTreeNode() With {.Text = GetNodeText(item), .Value = item.Code}
        AddAttribute(node, "Mailboxes", item.Mailboxes)
        AddAttribute(node, "FullCode", item.FullCode)
        Return node
    End Function
    Private Sub AddNode(nodes As RadTreeNodeCollection, item As IOChartItemDTO)
        Dim current As RadTreeNode = GetNode(item)
        If nodes.FindNodeByValue(current.Value, True) IsNot Nothing Then
            Return
        End If
        If item.Parent IsNot Nothing Then
            AddNode(nodes, item.Parent)
        End If
        nodes.Add(current)
        If item.Items.IsNullOrEmpty() Then
            item.Items.ToList().ForEach(Sub(i) AddNode(current.Nodes, i))
        End If
    End Sub

    Private Sub BindTreeViewItems(filter As String)
        Dim items As IOChartItemDTO() = OChartConnector.For(DropDownProviders.SelectedValue).GetEffective()
        Dim found As IEnumerable(Of IOChartItemDTO) = items.Where(Function(i) String.Format("{0}|{1}|{2}", i.Title, i.Description, i.Mailboxes).ContainsIgnoreCase(filter))
        found.ToList().ForEach(Sub(i) AddNode(TreeViewItems.Nodes, i))
    End Sub

#End Region

#Region " Extension Methods (?) "

    ' FG20131129: Sarebbe bello averlo come extension method in un progetto VecompSoftware.Telerik.Web
    Private Sub AddAttribute(node As RadTreeNode, key As String, value As String)
        If String.IsNullOrEmpty(value) Then
            Return
        End If
        node.Attributes.Add(key, value)
    End Sub

#End Region

End Class