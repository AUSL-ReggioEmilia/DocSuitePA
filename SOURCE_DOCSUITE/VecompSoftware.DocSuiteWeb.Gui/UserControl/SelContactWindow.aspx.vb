Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Entity.Tenants

Partial Public Class SelContactWindow
    Inherits Page

    Dim myContactFacade As ContactFacade

    Public Property CurrentTenant As Tenant
        Get
            If Session(CommonShared.USER_CURRENT_TENANT) IsNot Nothing Then
                Return DirectCast(Session(CommonShared.USER_CURRENT_TENANT), Tenant)
            End If
            Return Nothing
        End Get
        Set(value As Tenant)
            Session(CommonShared.USER_CURRENT_TENANT) = value
        End Set
    End Property

#Region "Page Load"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        myContactFacade = New ContactFacade()

        If Not IsPostBack And Not IsCallback Then
            rtvContact_NodeExpand(Nothing, New RadTreeNodeEventArgs(rtvContact.Nodes(0)))
        End If

    End Sub

#End Region

    Protected Sub rtvContact_NodeExpand(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs) Handles rtvContact.NodeExpand

        Dim _ContactsList As IList(Of Contact)

        If String.IsNullOrEmpty(e.Node.Value) Then
            _ContactsList = myContactFacade.GetRootContact(False, False, False, CurrentTenant.TenantAOO.UniqueId)
        Else
            _ContactsList = myContactFacade.GetContactByParentId(Integer.Parse(e.Node.Value), False)
        End If

        For Each contact As Contact In _ContactsList
            Dim childNode As New RadTreeNode()
            childNode.Text = contact.Description
            childNode.Value = contact.Id.ToString()
            childNode.ImageUrl = Page.ResolveUrl(ImagePath.ContactTypeIcon(contact.ContactType.Id, contact.isLocked.HasValue AndAlso contact.isLocked.Value = 1))
            childNode.ExpandMode = TreeNodeExpandMode.ServerSideCallBack
            e.Node.Nodes.Add(childNode)
        Next contact

    End Sub

End Class