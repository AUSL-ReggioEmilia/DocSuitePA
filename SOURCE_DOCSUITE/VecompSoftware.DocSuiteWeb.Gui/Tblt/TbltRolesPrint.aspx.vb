Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Partial Class PrintRoles
    Inherits CommonBasePage

#Region "Properties"
    Private ReadOnly Property IsSecurity() As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("IsSecurity", False)
        End Get
    End Property
#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        InitializeAjaxSettings()
        InitializeControls()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub cmdStampa_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdStampa.Click
        If Not RadTreeRole.CheckedNodes().Count > 0 Then
            AjaxAlert("Selezione Settore obbligatoria")
            Exit Sub
        End If

        If IsSecurity Then
            StampaSettoreWithSecurity()
        Else
            StampaSettore()
        End If
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
        WebUtils.ExpandOnClientNodeAttachEvent(RadTreeRole)
    End Sub

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSelectAll, RadTreeRole)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnDeselectAll, RadTreeRole)
    End Sub

    Private Sub Initialize()

        Dim roles As IList(Of Role) = Facade.RoleFacade.GetRootItems(withPECMailbox:=False)
        If roles.Count <= 0 Then
            Exit Sub
        End If

        RadTreeRole.Nodes(0).Nodes.Clear()
        For Each role As Role In roles
            Dim node As RadTreeNode = CreateNode(role)
            RadTreeRole.Nodes(0).Nodes.Add(node)
        Next
    End Sub

    Private Shared Function CreateNode(ByVal role As Role) As RadTreeNode
        Dim node As New RadTreeNode
        node.Text = role.Name()
        node.Value = role.Id.ToString()
        node.Checkable = True
        node.Expanded = True
        If role.Father Is Nothing Then
            node.ImageUrl = ImagePath.SmallRole
        Else
            node.ImageUrl = ImagePath.SmallSubRole
        End If
        Return node
    End Function

    Private Sub StampaSettore()
        Dim print As New RolePrint()
        For Each node As RadTreeNode In RadTreeRole.CheckedNodes
            print.RolesID.Add(Integer.Parse(node.Value))
        Next
        Session("Printer") = print
        Response.Redirect("..\Comm\CommPrint.aspx?Type=Comm&PrintName=RolePrint")
    End Sub

    Private Sub StampaSettoreWithSecurity()
        Dim print As New RoleSecurityPrint()
        For Each node As RadTreeNode In RadTreeRole.CheckedNodes
            print.RolesID.Add(Integer.Parse(node.Value))
        Next
        Session("Printer") = print
        Response.Redirect("..\Comm\CommPrint.aspx?Type=Comm&PrintName=RoleSecurityPrint")
    End Sub

    Private Sub SelectOrDeselectAll(ByVal Selected As Boolean)
        For Each node As RadTreeNode In RadTreeRole.Nodes(0).Nodes
            node.Checked = Selected
        Next
    End Sub

#End Region

End Class


