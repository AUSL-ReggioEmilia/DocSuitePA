Imports System.Collections.Generic
Imports System.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade

Partial Public Class uscAuthorizations
    Inherits DocSuite2008BaseControl

#Region " Fields "

#End Region

#Region " Properties "

    Public ReadOnly Property PageContentDiv As RadPageLayout
        Get
            Return pageContent
        End Get
    End Property

    Public Property MasterRoles As ICollection(Of Role)
    Public Property ResponsibleRoles As ICollection(Of Role)
    Public Property AccountedRoles As ICollection(Of Role)
    Public Property ResponsibleContacts As ICollection(Of Entity.Commons.Contact)
    Public Property AccountedRoleCaption As String
    Public Property WorkflowRole As ICollection(Of Role)
    Public Property WorkflowHandler As String
#End Region

#Region " Events "

    Private Sub uscAuthorizations_Load(sender As Object, e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            ClearControls()
            rowWorkflowHandler.Visible = False
            responsibleUser.Visible = False
            rowResponsibleRole.Visible = False
            rowWorkflowHandler.Visible = False
            rowMasterRole.Visible = False
        End If
    End Sub

    Private Sub ClearControls()
        rtvAuthorizedRoles.Nodes.Clear()
        rtvResponsibleRole.Nodes.Clear()
        rtvResponsibleUser.Nodes.Clear()
        rtvWorkflowHandler.Nodes.Clear()
        rtvMasterRole.Nodes.Clear()
    End Sub

    Private Sub BtnExpandFascInfo_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnExpandFascInfo.Click
        If btnExpandFascInfo.CssClass.Contains("dsw-arrow-down") Then
            fascInfo.Visible = False
            btnExpandFascInfo.CssClass = btnExpandFascInfo.CssClass.Replace("dsw-arrow-down", "dsw-arrow-up")
        Else
            fascInfo.Visible = True
            btnExpandFascInfo.CssClass = btnExpandFascInfo.CssClass.Replace("dsw-arrow-up", "dsw-arrow-down")
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, rtvAuthorizedRoles)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, rtvResponsibleRole)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, rtvMasterRole)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, rtvResponsibleUser)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, rtvWorkflowHandler)
    End Sub

    Public Sub BindData()
        ClearControls()
        If WorkflowRole IsNot Nothing Then
            rowMasterRole.Visible = True
            LoadRolesTree(WorkflowRole, rtvMasterRole, "Settore con presa in carico")
        End If
        If ResponsibleRoles IsNot Nothing Then
            rowResponsibleRole.Visible = True
            LoadRolesTree(ResponsibleRoles, rtvResponsibleRole, "Settore Responsabile/Competente")
        End If
        If AccountedRoles IsNot Nothing Then
            authorizedRoles.Visible = True
            LoadRolesTree(AccountedRoles, rtvAuthorizedRoles, AccountedRoleCaption)
        End If
        If ResponsibleContacts IsNot Nothing AndAlso ResponsibleContacts.Count > 0 Then
            responsibleUser.Visible = True
            LoadContacts(ResponsibleContacts, rtvResponsibleUser, DocSuiteContext.Current.ProtocolEnv.FascicleRoleRPLabel)
        End If

        If Not String.IsNullOrEmpty(WorkflowHandler) Then
            rowWorkflowHandler.Visible = True
            LoadWorkflowHandler(WorkflowHandler)
        End If
    End Sub

    Private Sub LoadWorkflowHandler(workflowHandler As String)
        Dim node As RadTreeNode = New RadTreeNode("In carico a", "Root")
        node.Font.Bold = True
        node.ToolTip = "Espandibile"
        rtvWorkflowHandler.Nodes.Add(node)
        rtvWorkflowHandler.Nodes(0).Expanded = True

        Dim handler As RadTreeNode = New RadTreeNode()
        handler.Text = workflowHandler
        handler.ImageUrl = ImagePath.ContactTypeIcon(ContactType.Person, False)
        rtvWorkflowHandler.Nodes(0).Nodes.Add(handler)
    End Sub

    Private Sub LoadRolesTree(fascRoles As ICollection(Of Role), tree As RadTreeView, caption As String)
        If fascRoles.Any() Then
            Dim node As RadTreeNode = New RadTreeNode(caption, "Root")
            node.Font.Bold = True
            node.ToolTip = "Espandibile"
            tree.Nodes.Add(node)
            tree.Nodes(0).Expanded = True

            LoadRoles(fascRoles, tree.Nodes(0), caption)
        End If
    End Sub

    Private Sub LoadRoles(roles As ICollection(Of Role), ByRef tree As RadTreeNode, treeCaption As String)
        Dim reeView As RadTreeNode = tree
        Dim node As RadTreeNode
        For Each role As Role In roles
            node = SeekAndImplementNode(Nothing, role, tree, role.TenantId.ToString())
            node.Font.Bold = True
        Next
    End Sub

    Private Function SeekAndImplementNode(ByRef node As RadTreeNode, ByVal role As Role, ByRef treeView As RadTreeNode, tenantId As String) As RadTreeNode
        Dim existingNode As RadTreeNode = treeView.Nodes.FindNodeByValue(role.IdRoleTenant.ToString())
        If existingNode IsNot Nothing Then
            Return existingNode
        End If

        Dim nodeToAdd As New RadTreeNode()
        nodeToAdd.Checkable = False
        If role IsNot Nothing Then
            nodeToAdd.Text = role.Name
            nodeToAdd.Value = role.IdRoleTenant.ToString()
            If role.Father Is Nothing Then 'Primo Livello
                nodeToAdd.ImageUrl = ImagePath.SmallRole
                treeView.Nodes.Add(nodeToAdd)
            Else
                nodeToAdd.ImageUrl = ImagePath.SmallSubRole
                Dim newNode As RadTreeNode = treeView.Nodes.FindNodeByValue(role.Father.IdRoleTenant.ToString())
                If (newNode Is Nothing) Then
                    SeekAndImplementNode(nodeToAdd, role.Father, treeView, tenantId)
                Else
                    newNode.Nodes.Add(nodeToAdd)
                End If

            End If
            nodeToAdd.Expanded = True
        End If
        If node IsNot Nothing Then
            nodeToAdd.Nodes.Add(node)
        End If

        Return nodeToAdd
    End Function

    Private Sub LoadContacts(contacts As ICollection(Of Entity.Commons.Contact), ByRef tree As RadTreeView, treeCaption As String)
        Dim node As RadTreeNode = New RadTreeNode(String.Format("{0} ({1})", treeCaption, contacts.Count), "Root")
        node.Font.Bold = True
        node.ToolTip = "Espandibile"
        tree.Nodes.Add(node)
        tree.Nodes(0).Expanded = True
        Dim contact As Data.Contact
        For Each fascicleContact As Entity.Commons.Contact In contacts
            contact = Facade.ContactFacade.GetById(fascicleContact.EntityId)
            AddContactNode(Nothing, contact)
        Next
    End Sub

    Private Sub AddContactNode(ByRef node As RadTreeNode, ByVal contact As Contact)
        Dim nodeToAdd As New RadTreeNode()
        If rtvResponsibleUser.FindNodeByValue(contact.Id.ToString()) IsNot Nothing Then
            Return
        End If

        nodeToAdd.Text = Replace(contact.Description, "|", " ")
        If Not String.IsNullOrEmpty(contact.FiscalCode) AndAlso ProtocolEnv.ShowFiscalCodeInFascicleSummary Then
            nodeToAdd.Text = String.Format("{1} ({0})", contact.FiscalCode, nodeToAdd.Text)
        End If
        nodeToAdd.Value = contact.Id.ToString()
        nodeToAdd.ImageUrl = ImagePath.ContactTypeIcon(contact.ContactType.Id, contact.isLocked.HasValue AndAlso contact.isLocked.Value = 1)
        nodeToAdd.Expanded = True

        If contact IsNot Nothing Then
            rtvResponsibleUser.Nodes(0).Nodes.Add(nodeToAdd)
        End If
        If node IsNot Nothing Then
            nodeToAdd.Nodes.Add(node)
        End If
        nodeToAdd.Checkable = False
    End Sub
#End Region

End Class