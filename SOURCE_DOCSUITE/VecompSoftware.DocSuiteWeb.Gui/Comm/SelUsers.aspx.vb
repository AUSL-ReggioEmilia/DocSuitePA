Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports Newtonsoft.Json
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Model.Parameters
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Partial Public Class SelUsers
    Inherits CommonBasePage

#Region " Fields "
    Private _relatedGroupName As String
#End Region

#Region " Properties "
    Private ReadOnly Property RelatedGroupName As String
        Get
            If String.IsNullOrEmpty(_relatedGroupName) Then
                _relatedGroupName = Request.QueryString.GetValueOrDefault(Of String)("GroupName", String.Empty)
            End If
            Return _relatedGroupName
        End Get
    End Property
#End Region
#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()

        If Not IsPostBack Then
            MasterDocSuite.TitleVisible = False
            txtFilter.Focus()
            Initialize()
        End If
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnSearch.Click
        SearchUsers(txtFilter.Text)
    End Sub

#End Region

#Region " Methods "
    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, RadTreeUsers, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub SearchUsers(ByVal searchText As String)

        Dim filter As String = txtFilter.Text.Trim()

        If filter.Length < 2 Then
            AjaxAlert("Il filtro di ricerca deve essere di almeno 2 caratteri.", False)
            Exit Sub
        End If

        filter = Helpers.StringHelper.ReplaceBackSlash(filter)
        Dim currentDomain As String = String.Empty
        If DocSuiteContext.Current.ProtocolEnv.MultiDomainEnabled AndAlso lbMultiDomain.SelectedItem IsNot Nothing AndAlso Not lbMultiDomain.SelectedItem.Value.Eq("-") Then
            currentDomain = lbMultiDomain.SelectedItem.Text
        End If
        Dim found As IList(Of AccountModel) = CommonAD.FindADUsers(filter, currentDomain)
        If found.IsNullOrEmpty() Then
            AjaxAlert("Nessun risultato disponibile.", False)
            Exit Sub
        End If

        Dim root As New RadTreeNode()
        root.Text = String.Format("Utenti ({0})", found.Count)
        root.Value = String.Empty
        root.Expanded = True
        RadTreeUsers.Nodes.Clear()
        RadTreeUsers.Nodes.Add(root)
        Dim node As RadTreeNode = New RadTreeNode
        For Each user As AccountModel In found.OrderBy(Function(f) f.Name)
            user.RelatedGroupName = RelatedGroupName
            node = New RadTreeNode()
            node.Text = user.GetLabel()
            node.Value = JsonConvert.SerializeObject(user)
            node.ImageUrl = "../App_Themes/DocSuite2008/imgset16/GroupMembers.png"
            RadTreeUsers.Nodes(0).Nodes.Add(node)
        Next

    End Sub

    Private Sub Initialize()

        If DocSuiteContext.Current.ProtocolEnv.MultiDomainEnabled Then
            lbMultiDomain.Visible = True
            Dim tenantModels As IList(Of TenantModel) = DocSuiteContext.Current.Tenants
            lbMultiDomain.Items.Add(New ListItem("Tutti domini", "-"))
            For Each tenatnModel As TenantModel In tenantModels
                lbMultiDomain.Items.Add(New ListItem(tenatnModel.DomainName, tenatnModel.DomainAddress))
            Next
            lbMultiDomain.Items.FindByValue(DocSuiteContext.Current.CurrentTenant.DomainAddress).Selected = True
        End If
    End Sub

#End Region

End Class