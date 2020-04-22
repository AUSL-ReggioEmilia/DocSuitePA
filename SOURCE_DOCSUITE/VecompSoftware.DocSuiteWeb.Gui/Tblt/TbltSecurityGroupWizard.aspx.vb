Imports System.Collections.Generic
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class TbltSecurityGroupWizard
    Inherits CommonBasePage

#Region "Fields"
    Private _currentFinder As SecurityGroupFinder
    Private _currentUser As AccountModel
#End Region

#Region "Properties"
    Protected ReadOnly Property CurrentFinder As SecurityGroupFinder
        Get
            If _currentFinder Is Nothing Then
                _currentFinder = New SecurityGroupFinder()
            End If
            Return _currentFinder
        End Get
    End Property

    Private ReadOnly Property CurrentUser As AccountModel
        Get
            If _currentUser Is Nothing Then
                _currentUser = CommonAD.GetAccount(String.Format("{0}\{1}", Request.QueryString("DomainAD").ToString(), Request.QueryString("AccountAD").ToString()))
            End If
            Return _currentUser
        End Get
    End Property

    Private ReadOnly Property rcbConfigType As RadComboBox
        Get
            Dim toolBarItem As RadToolBarItem = ToolBarSearchConfig.FindItemByValue("btnConfigs")
            Return DirectCast(toolBarItem.FindControl("rcbConfigType"), RadComboBox)
        End Get
    End Property

    Private ReadOnly Property rcbContainer As RadComboBox
        Get
            Dim toolBarItem As RadToolBarItem = ToolBarSearchContainer.FindItemByValue("btnContainers")
            Return DirectCast(toolBarItem.FindControl("rcbContainer"), RadComboBox)
        End Get
    End Property

    Private ReadOnly Property uscRole As RadComboBox
        Get
            Dim toolBarItem As RadToolBarItem = ToolBarSearchRole.FindItemByValue("btnRolesUsc")
            Return DirectCast(toolBarItem.FindControl("rcbRole"), RadComboBox)
        End Get
    End Property
#End Region

#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()
        If RadTreeGroups.SelectedNode IsNot Nothing AndAlso Not String.IsNullOrEmpty(RadTreeGroups.SelectedNode.Value) Then
            groupDetails.SelectedGroupId = CInt(RadTreeGroups.SelectedNode.Value)
            groupDetails.LoadDetails()
        End If
        If Not IsPostBack Then
            Title = String.Format("Configurazione Utente - {0}\{1}", Request.QueryString("DomainAD").ToString(), Request.QueryString("AccountAD").ToString())
            btnSave.Enabled = False
            ToolBarSearchContainer.Visible = False
            ToolBarSearchRole.SetDisplay(False)
            LoadConfigType()
            groupDetails.ContextMenuUserEnabled = False
        End If
    End Sub

    Protected Sub ToolBarSearch_ButtonClick(ByVal sender As Object, ByVal e As RadToolBarEventArgs)
        Search()
    End Sub

    Protected Sub rcbConfigType_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs)
        Select Case rcbConfigType.SelectedValue
            Case "container"
                ToolBarSearchContainer.Visible = True
                ToolBarSearchRole.SetDisplay(False)
                rcbContainer.DataSource = Facade.ContainerFacade.GetAllOrdered("Name ASC")
                rcbContainer.DataBind()
            Case "role"
                ToolBarSearchContainer.Visible = False
                ToolBarSearchRole.SetDisplay(True)
                uscRole.DataSource = Facade.RoleFacade.GetRoles(DSWEnvironment.Any, Nothing, True, String.Empty, False, Nothing, TenantId)
                uscRole.DataBind()
            Case Else
                ToolBarSearchContainer.Visible = False
                ToolBarSearchRole.SetDisplay(False)
        End Select
    End Sub

    Private Sub RadTreeGroups_NodeClick(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs) Handles RadTreeGroups.NodeClick
        If Not String.IsNullOrEmpty(e.Node.Value) Then
            groupDetails.SelectedGroupId = CInt(e.Node.Value)
            groupDetails.LoadDetails()
        End If
    End Sub

    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As ButtonClickEventArgs) Handles btnSave.Click
        Dim errorGroups As List(Of String) = New List(Of String)()
        Dim groupIds As IList(Of RadTreeNode) = RadTreeGroups.CheckedNodes
        If groupIds.Count = 0 Then
            AjaxAlert("Spuntare almeno un gruppo")
            AjaxManager.ResponseScripts.Add("closeLoadingPanel();")
            Exit Sub
        End If
        For Each groupId As RadTreeNode In groupIds
            Dim group As SecurityGroups = Facade.SecurityGroupsFacade.GetById(Integer.Parse(groupId.Value))
            If group IsNot Nothing AndAlso Facade.SecurityUsersFacade.IsUserInGroup(group, CurrentUser.GetFullUserName) Then
                errorGroups.Add(group.GroupName)
                Continue For
            End If
            Facade.SecurityUsersFacade.Insert(CurrentUser.Domain, CurrentUser.Account, CurrentUser.Name, group)
        Next
        Dim errorMessage As String = "Utente inserito con successo"
        If errorGroups.Count > 0 Then
            errorMessage = String.Format("L'utente {0} è già presente nei gruppi: " & vbCrLf, CurrentUser.GetFullUserName)
            For Each item As String In errorGroups
                errorMessage = String.Concat(errorMessage, item & vbCrLf)
            Next
        End If
        AjaxAlert(errorMessage)
        AjaxManager.ResponseScripts.Add("closeLoadingPanel();")
    End Sub

    Private Sub btnBack_Click(ByVal sender As Object, ByVal e As ButtonClickEventArgs) Handles btnBack.Click
        Dim param As String = "Type=Comm"
        Response.Redirect(String.Format("../Tblt/TbltSecurityUsers.aspx?", CommonShared.AppendSecurityCheck(param)))
    End Sub
#End Region

#Region "Methods"
    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(ToolBarSearchConfig, RadTreeGroups, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ToolBarSearchConfig, btnSave)
        AjaxManager.AjaxSettings.AddAjaxSetting(rcbConfigType, ToolBarSearchContainer)
        AjaxManager.AjaxSettings.AddAjaxSetting(rcbConfigType, ToolBarSearchRole)
        AjaxManager.AjaxSettings.AddAjaxSetting(RadTreeGroups, pnlGroupDetails, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub LoadConfigType()
        rcbConfigType.Items.Add(New RadComboBoxItem(String.Empty, String.Empty))
        rcbConfigType.Items.Add(New RadComboBoxItem("Contenitori", "container"))
        rcbConfigType.Items.Add(New RadComboBoxItem("Settori", "role"))
    End Sub

    Private Sub Search()
        btnSave.Enabled = True
        Dim root As New RadTreeNode()

        CurrentFinder.ClearFinder()

        If DecorateFinder() Then
            Exit Sub
        End If

        Dim sGroups As IList(Of SecurityGroups) = CurrentFinder.DoSearch()

        For Each g As SecurityGroups In sGroups
            root.Nodes.Add(CreateNode(g.GroupName, g.Id))
        Next

        root.Text = String.Format("Gruppi ({0})", root.Nodes.Count)
        root.Value = String.Empty
        root.Checkable = False
        root.Expanded = True

        RadTreeGroups.Nodes.Clear()
        RadTreeGroups.Nodes.Add(root)
    End Sub

    Private Shared Function CreateNode(groupName As String, idGroup As Integer) As RadTreeNode
        Dim node As New RadTreeNode
        node.Text = groupName
        node.Value = idGroup.ToString()
        node.ImageUrl = "../App_Themes/DocSuite2008/imgset16/GroupMembers.png"
        Return node
    End Function

    Private Function DecorateFinder() As Boolean
        'restrizioni sulla ricerca del gruppo già configurato
        'su classificatori, contenitori e/o settori        
        Select Case rcbConfigType.SelectedValue
            Case "container"
                If String.IsNullOrEmpty(rcbContainer.SelectedValue) Then
                    AjaxAlert("Selezionare almeno un contenitore")
                    Return True
                End If
                CurrentFinder.OnContainerEnabled = CInt(rcbContainer.SelectedValue)
            Case "role"
                If String.IsNullOrEmpty(uscRole.SelectedValue) Then
                    AjaxAlert("Selezionare almeno un settore")
                    Return True
                End If
                CurrentFinder.OnRoleEnabled = CInt(uscRole.SelectedValue)
        End Select
        Return False
    End Function

#End Region

End Class