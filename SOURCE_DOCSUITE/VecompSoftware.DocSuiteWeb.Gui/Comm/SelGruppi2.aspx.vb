Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data

Partial Class SelGruppi2
    Inherits CommBasePage

#Region " Events "
    Private Sub BtnCercaClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnCerca.Click
        Dim filteredNodes As ICollection(Of RadTreeNode) = GetFilteredNodes(txtCerca.Text)
        BindGroupNodes(filteredNodes)
    End Sub

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        MasterDocSuite.TitleVisible = False
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub
#End Region

#Region " Methods "
    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnCerca, RadTreeGroups, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub Initialize()
        txtCerca.Focus()
    End Sub

    Private Function GetFilteredNodes(filter As String) As ICollection(Of RadTreeNode)
        Dim foundNodes As ICollection(Of RadTreeNode) = New List(Of RadTreeNode)()
        For Each group As SecurityGroups In Facade.SecurityGroupsFacade.GetGroupsFlat(filter)
            foundNodes.Add(CreateNode(group.GroupName))
        Next
        Return foundNodes
    End Function

    Private Sub BindGroupNodes(groupNodes As ICollection(Of RadTreeNode))
        Dim rootNode As RadTreeNode = New RadTreeNode With {
            .Checkable = False,
            .Value = String.Empty,
            .Expanded = True,
            .Text = $"Gruppi ({groupNodes.Count})"
        }

        rootNode.Nodes.AddRange(groupNodes)

        RadTreeGroups.Nodes.Clear()
        RadTreeGroups.Nodes.Add(rootNode)
    End Sub

    Private Function CreateNode(ByVal roleName As String) As RadTreeNode
        Dim node As RadTreeNode = New RadTreeNode With {
            .Text = roleName,
            .Value = roleName,
            .ImageUrl = "../App_Themes/DocSuite2008/imgset16/GroupMembers.png"
        }
        Return node
    End Function

#End Region

End Class


