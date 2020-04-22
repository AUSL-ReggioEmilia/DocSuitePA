Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports System.Linq
Imports Telerik.Web.UI
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Model.Parameters
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.DTO.Collaborations

Partial Public Class UserAbsentManagers
    Inherits UserBasePage

#Region " Fields "
    Private Const ATTRIBUTE_MANAGER_NODE As String = "manager_node"
    Private Const ATTRIBUTE_ORIGINAL_NAME As String = "originalName"
    Private Const ATTRIBUTE_STATUS As String = "status"
#End Region

#Region " Properties "

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        MasterDocSuite.TitleVisible = False

        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub btnConfirm_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConfirm.Click
        Dim checkedManagers As AbsentManager() = rtvManagers.Nodes(0).GetAllNodes().Where(Function(t) t.Checked).Select(Function(t) New AbsentManager() With {.Manager = t.Value, .Substitution = t.Attributes(ATTRIBUTE_STATUS)}).ToArray()

        Dim serialized As String = StringHelper.EncodeJS(JsonConvert.SerializeObject(checkedManagers))
        AjaxManager.ResponseScripts.Add(String.Format("return Close('{0}');", serialized))
    End Sub

    Private Sub rtvManagers_NodeCheck(sender As Object, e As RadTreeNodeEventArgs) Handles rtvManagers.NodeCheck
        If e.Node IsNot Nothing Then
            If e.Node.Checked Then
                Dim label As Label = New Label()
                label.ID = String.Concat("lbl_", e.Node.Value.Replace(" ", String.Empty).Replace("-", String.Empty))
                label.Text = String.Concat("Il direttore ", e.Node.Text, " e' stato dichiarato assente, si vuole vicariarlo?")
                Dim dyn As RadDropDownList = New RadDropDownList()
                dyn.ID = String.Concat("dyn_", e.Node.Value.Replace(" ", String.Empty).Replace("-", String.Empty))
                dyn.Items.Add(New DropDownListItem("No, Assente", String.Empty))
                dyn.OnClientItemSelected = "OnClientItemSelected"
                dyn.Attributes.Add(ATTRIBUTE_MANAGER_NODE, e.Node.Value)
                Dim managers As IEnumerable(Of CollaborationManagerModel) = DocSuiteContext.Current.ProtocolEnv.AbsentManagersCertificates.Managers _
                    .Select(Function(f) f.Value) _
                    .Where(Function(f) f.IsAbsenceManaged AndAlso Not String.Equals(e.Node.Value, f.Account, StringComparison.InvariantCultureIgnoreCase) AndAlso Not rtvManagers.Nodes(0).GetAllNodes().Any(Function(t) t.Checked AndAlso String.Equals(t.Value, f.Account, StringComparison.InvariantCultureIgnoreCase)))
                For Each manager As CollaborationManagerModel In managers
                    dyn.Items.Add(New DropDownListItem(manager.SignUser, manager.Account))
                Next
                e.Node.Controls.Add(label)
                e.Node.Controls.Add(dyn)
            Else
                Dim controls As List(Of Control) = New List(Of Control)(New Control() {
                                                                       e.Node.FindControl(String.Concat("lbl_", e.Node.Value.Replace(" ", "").Replace("-", String.Empty))),
                                                                       e.Node.FindControl(String.Concat("dyn_", e.Node.Value.Replace(" ", "").Replace("-", String.Empty)))})
                For Each item As Control In controls.Where(Function(f) f IsNot Nothing)
                    e.Node.Controls.Remove(item)
                Next
                For Each item As RadTreeNode In rtvManagers.Nodes(0).GetAllNodes().Where(Function(f) f.Attributes(ATTRIBUTE_STATUS) = e.Node.Value)
                    item.Attributes(ATTRIBUTE_STATUS) = String.Empty
                    item.Text = item.Attributes(ATTRIBUTE_ORIGINAL_NAME)
                Next
                e.Node.Text = e.Node.Attributes(ATTRIBUTE_ORIGINAL_NAME)
            End If

        End If
    End Sub

    Private Sub UserAbsentManagers_AjaxRequest(sender As Object, e As AjaxRequestEventArgs)
        Dim arguments As String() = e.Argument.Split("~"c)
        If arguments.Any() Then
            Select Case arguments(0)
                Case "manageManager"
                    Dim manager As String = arguments(1)
                    Dim value As String = arguments(2)
                    Dim description As String = arguments(3)
                    Dim node As RadTreeNode = rtvManagers.Nodes(0).GetAllNodes().SingleOrDefault(Function(f) f.Value = manager)
                    If node IsNot Nothing Then
                        node.Attributes(ATTRIBUTE_STATUS) = value
                        If Not String.IsNullOrEmpty(value) Then
                            node.Text = String.Concat(node.Attributes(ATTRIBUTE_ORIGINAL_NAME), " assente e vicariato da ", description)
                        End If
                    End If
                    For Each item As RadTreeNode In rtvManagers.Nodes(0).GetAllNodes().Where(Function(f) f.Attributes(ATTRIBUTE_STATUS) = manager)
                        item.Attributes(ATTRIBUTE_STATUS) = String.Empty
                        item.Text = item.Attributes(ATTRIBUTE_ORIGINAL_NAME)
                    Next

            End Select
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler MasterDocSuite.AjaxManager.AjaxRequest, AddressOf UserAbsentManagers_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, rtvManagers, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(rtvManagers, rtvManagers, MasterDocSuite.AjaxFlatLoadingPanel)
    End Sub

    Private Sub Initialize()
        Dim node As RadTreeNode
        Dim collaborationManagers As IList(Of CollaborationManagerDTO) = New List(Of CollaborationManagerDTO)
        Dim collaborationManager As CollaborationManagerDTO
        Dim user As AccountModel
        For Each manager As CollaborationManagerModel In ProtocolEnv.AbsentManagersCertificates.Managers.Values.Where(Function(m) m.IsAbsenceManaged)
            user = CommonAD.GetAccount(manager.Account)
            If (user IsNot Nothing) Then
                collaborationManager = New CollaborationManagerDTO With {
                    .UserName = user.Account,
                    .Domain = user.Domain,
                    .FullAccount = manager.Account,
                    .RoleId = manager.RoleId,
                    .Description = String.Concat(user.DisplayName, " - ", manager.Type)
                }
                collaborationManagers.Add(collaborationManager)
            End If
        Next

        If CurrentCollaboration IsNot Nothing Then
            collaborationManagers = collaborationManagers.Where(Function(t) CurrentCollaboration.CollaborationSigns.Any(Function(s) s.SignUser.Eq(t.FullAccount) AndAlso (Not s.IsAbsent.HasValue OrElse Not s.IsAbsent.Value) AndAlso s.Incremental >= CurrentCollaborationSign.Incremental)).ToList()
        End If

        For Each item As CollaborationManagerDTO In collaborationManagers
            node = New RadTreeNode With {
                .Text = item.Description,
                .Value = item.FullAccount
            }
            node.Attributes(ATTRIBUTE_ORIGINAL_NAME) = item.Description
            node.Attributes(ATTRIBUTE_STATUS) = String.Empty
            rtvManagers.Nodes(0).Nodes.Add(node)
        Next
    End Sub

#End Region

End Class