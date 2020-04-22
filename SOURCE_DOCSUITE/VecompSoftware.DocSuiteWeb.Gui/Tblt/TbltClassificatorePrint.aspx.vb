Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI

Partial Class PrintClassificatore
    Inherits CommonBasePage

#Region " Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        InitializeAjaxSettings()
        InitializeControls()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub cmdStampa_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdStampa.Click
        If Not RadTreeCategory.CheckedNodes().Count > 0 Then
            AjaxAlert("Selezione Classificatore obbligatoria")
            Exit Sub
        End If

        StampaClassificazione()
    End Sub

    Private Sub btnSelectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectAll.Click
        SelectOrDeselectAll(True)
    End Sub

    Private Sub btnDeselectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDeselectAll.Click
        SelectOrDeselectAll(False)
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeControls()
        WebUtils.ExpandOnClientNodeAttachEvent(RadTreeCategory)
    End Sub

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSelectAll, RadTreeCategory)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnDeselectAll, RadTreeCategory)
    End Sub

    Private Sub Initialize()
        Dim categories As IList(Of Category) = Facade.CategoryFacade.GetRootCategory(True)
        If categories.Count <= 0 Then
            Exit Sub
        End If

        RadTreeCategory.Nodes(0).Nodes.Clear()
        For Each category As Category In categories
            Dim node As RadTreeNode = CreateNode(category)
            RadTreeCategory.Nodes(0).Nodes.Add(node)
        Next
    End Sub

    Private Shared Function CreateNode(ByVal category As Category) As RadTreeNode
        Dim node As New RadTreeNode
        node.Text = category.GetFullName()
        node.Value = category.Id.ToString()
        node.Checkable = True
        node.Expanded = True
        If category.HasChildren Then
            node.ImageUrl = "../Comm/images/folderopen16.gif"
        Else
            node.ImageUrl = "../Comm/images/Classificatore.gif"
        End If
        Return node
    End Function

    Private Sub StampaClassificazione()
        Dim print As New CategoryPrint()
        For Each node As RadTreeNode In RadTreeCategory.CheckedNodes
            print.CategoriesID.Add(Integer.Parse(node.Value))
        Next
        Session("Printer") = print
        Response.Redirect("..\Comm\CommPrint.aspx?Type=Comm&PrintName=CategoryPrint")
    End Sub

    Private Sub SelectOrDeselectAll(ByVal Selected As Boolean)
        For Each node As RadTreeNode In RadTreeCategory.Nodes(0).Nodes
            node.Checked = Selected
        Next
    End Sub

#End Region

End Class


