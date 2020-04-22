Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Partial Public Class uscMultiSelGroups
    Inherits CommonBasePage

#Region " Fields "

#End Region

#Region " Properties "

#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False
        InitializeAjax()
        If Not Page.IsPostBack Then
            btnConfirm.Enabled = False
        End If
    End Sub

    Private Sub BtnSearchClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearch.Click
        Initialize(txtSearch.Text)
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, RadTreeGroups, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, btnConfirm)
    End Sub

    Private Sub Initialize(ByVal search As String)
        btnConfirm.Enabled = True
        Dim root As New RadTreeNode()

        For Each group As SecurityGroups In Facade.SecurityGroupsFacade.GetGroupsFlat(search)
            root.Nodes.Add(CreateNode(group.GroupName))
        Next

        root.Text = String.Format("Gruppi ({0})", root.Nodes.Count)
        root.Value = String.Empty
        root.Expanded = True

        RadTreeGroups.Nodes.Clear()
        RadTreeGroups.Nodes.Add(root)

    End Sub

    Private Shared Function CreateNode(groupName As String) As RadTreeNode
        Dim node As New RadTreeNode
        node.Text = groupName
        node.Value = groupName
        node.ImageUrl = "../App_Themes/DocSuite2008/imgset16/GroupMembers.png"
        Return node
    End Function

#End Region

End Class