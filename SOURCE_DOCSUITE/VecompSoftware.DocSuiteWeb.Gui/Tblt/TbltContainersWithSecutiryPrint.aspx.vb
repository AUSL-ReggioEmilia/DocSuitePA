Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Partial Class PrintContainersWithSecurity
    Inherits CommonBasePage

#Region "Properties"
    Private ReadOnly Property IsSecurity() As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean)("IsSecurity", False)
        End Get
    End Property

#End Region
#Region " Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If Not (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupTblStampeSecurityRight OrElse CommonShared.HasGroupTblStampeRight) Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If
        InitializeAjaxSettings()
        InitializeControls()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub cmdStampa_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdStampa.Click
        If Not RadTreeContainer.CheckedNodes().Count > 0 Then
            AjaxAlert("Selezione Contenitore obbligatoria")
            Exit Sub
        End If
        If IsSecurity Then
            StampaContainersWithSecurity()
        Else
            StampaContenitore()
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
        WebUtils.ExpandOnClientNodeAttachEvent(RadTreeContainer)
    End Sub

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSelectAll, RadTreeContainer)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnDeselectAll, RadTreeContainer)
    End Sub

    Private Sub Initialize()
        Dim finder As New ContainerFinder()
        Dim containers As IList(Of Container) = finder.List()
        If containers.Count <= 0 Then
            Exit Sub
        End If

        RadTreeContainer.Nodes(0).Nodes.Clear()
        For Each container As Container In containers
            Dim node As RadTreeNode = CreateNode(container)
            RadTreeContainer.Nodes(0).Nodes.Add(node)
        Next
    End Sub

    Private Shared Function CreateNode(ByVal container As Container) As RadTreeNode
        Dim node As New RadTreeNode
        node.Text = container.Name()
        node.Value = container.Id.ToString()
        node.Checkable = True
        node.Expanded = True
        node.ImageUrl = ImagePath.SmallBoxOpen
        Return node
    End Function

    Private Sub StampaContenitore()
        Dim print As New ContainerPrint()
        For Each node As RadTreeNode In RadTreeContainer.CheckedNodes
            print.ContainersID.Add(Integer.Parse(node.Value))
        Next
        Session("Printer") = print
        Response.Redirect("..\Comm\CommPrint.aspx?Type=Comm&PrintName=ContainerPrint")
    End Sub

    Private Sub StampaContainersWithSecurity()
        Dim print As New ContainerSecurityPrint()
        For Each node As RadTreeNode In RadTreeContainer.CheckedNodes
            print.ContainersID.Add(Integer.Parse(node.Value))
        Next
        Session("Printer") = print
        Response.Redirect("..\Comm\CommPrint.aspx?Type=Comm&PrintName=ContainerSecurityPrint")
    End Sub

    Private Sub SelectOrDeselectAll(ByVal Selected As Boolean)
        For Each node As RadTreeNode In RadTreeContainer.Nodes(0).Nodes
            node.Checked = Selected
        Next
    End Sub

#End Region

End Class


