Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Services.Logging

Partial Public Class TbltGruppi
    Inherits CommonBasePage

#Region " Properties "
    ''' <summary>
    ''' Torna il gruppo selezionato
    ''' </summary>
    ''' <value>SecurityGroup</value>
    ''' <returns>Nothing se non è selezionato o se è il nodo root</returns>
    Private ReadOnly Property SelectedGroup() As SecurityGroups
        Get
            ' Se nessun nodo è selezionato o se è il nodo root
            If RadTreeViewGroups.SelectedNode Is Nothing Or String.IsNullOrEmpty(RadTreeViewGroups.SelectedNode.Value) Then
                Return Nothing
            End If

            Dim idGroup As Integer
            If Integer.TryParse(RadTreeViewGroups.SelectedNode.Value, idGroup) Then
                Dim sel As SecurityGroups = Facade.SecurityGroupsFacade.GetById(idGroup)
                groupDetails.SelectedGroupId = idGroup
                Return sel
            Else
                Throw New InvalidCastException("IdGroup non corretto: " & RadTreeViewGroups.SelectedNode.Value)
            End If
        End Get
    End Property

    Private ReadOnly Property SelectedUser() As IList(Of SecurityUsers)
        Get
            Return groupDetails.SelectedUsers
        End Get
    End Property
    Public ReadOnly Property SearchGroupTextBox As RadTextBox
        Get
            Dim toolBarItem As RadToolBarItem = ToolBarSearch.FindItemByValue("searchGroup")
            Return DirectCast(toolBarItem.FindControl("txtGroup"), RadTextBox)
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjaxSettings()
        If Not IsPostBack Then
            InitializeControls()
        End If
    End Sub

    Protected Sub TbltGruppi_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arguments As String() = Split(e.Argument, "|", 3)
        Select Case arguments(0).ToLower()
            Case "groups"
                Dim group As SecurityGroups = Facade.SecurityGroupsFacade.GetById(Integer.Parse(arguments(2)))
                Select Case arguments(1)
                    Case "Add"
                        Dim father As RadTreeNode = RadTreeViewGroups.SelectedNode
                        ' creo il nodo e lo seleziono
                        Dim newNode As RadTreeNode = AddRecursiveNode(father, group)
                        newNode.Selected = True
                        groupDetails.SelectedGroupId = Integer.Parse(arguments(2))
                        groupDetails.LoadDetails()
                    Case "Rename"
                        ' Aggiorno il testo del nodo
                        Dim node As RadTreeNode = RadTreeViewGroups.SelectedNode
                        groupDetails.SelectedGroupId = Integer.Parse(arguments(2))
                        groupDetails.LoadDetails()
                        SetNode(node, group)
                    Case "Delete"
                        RadTreeViewGroups.SelectedNode.Remove()
                        groupDetails.ClearRadTreeViewUsers()
                End Select

            Case "importfrom"
                Dim seletedGroup As SecurityGroups = Facade.SecurityGroupsFacade.GetGroupByName(arguments(1))
                If seletedGroup IsNot Nothing Then
                    SetNode(RadTreeViewGroups.SelectedNode, SelectedGroup)
                End If

            Case "users"
                Dim adUser As AccountModel = JsonConvert.DeserializeObject(Of AccountModel)(arguments(1))
                Dim group As SecurityGroups = SelectedGroup

                If Facade.SecurityUsersFacade.IsUserInGroup(group, adUser.GetFullUserName()) Then
                    AjaxAlert("L'utente {0} è già presente nel gruppo {1}", adUser.Account, group.GroupName)
                    Exit Select
                End If
                ' aggiorno il nodo del gruppo per visualizzare l'icona corretta
                SetNode(RadTreeViewGroups.SelectedNode, SelectedGroup)
            Case "deleteuser"
                DeleteUser()
        End Select
    End Sub

    Private Sub cmdSearch_Click(sender As Object, e As EventArgs) Handles ToolBarSearch.ButtonClick
        ' Eseguo il popolamento dell'albero filtrando per nome del gruppo       
        LoadGroups()
    End Sub

    Private Sub RadTreeViewGroups_NodeClick(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs) Handles RadTreeViewGroups.NodeClick
        groupDetails.Button_AddUser.Enabled = False
        groupDetails.Button_ImportUser.Enabled = False
        groupDetails.Button_DeleteUser.Enabled = False
        If SelectedGroup IsNot Nothing Then
            groupDetails.Button_AddUser.Enabled = Not SelectedGroup.HasAllUsers AndAlso (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasSecurityGroupAdminRight OrElse CommonShared.HasSecurityGroupPowerUserRight)
            groupDetails.Button_ImportUser.Enabled = Not SelectedGroup.HasAllUsers AndAlso (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasSecurityGroupAdminRight OrElse CommonShared.HasSecurityGroupPowerUserRight)
            groupDetails.Button_DeleteUser.Enabled = Not SelectedGroup.HasAllUsers AndAlso (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasSecurityGroupAdminRight OrElse CommonShared.HasSecurityGroupPowerUserRight)

            groupDetails.LoadDetails()
        End If
    End Sub

    Private Sub DeleteUser()
        Try
            If SelectedUser Is Nothing Then
                AjaxAlert("Nessun nodo selezionato")
                Exit Sub
            End If
            groupDetails.DeleteFromGroup()
            SetNode(RadTreeViewGroups.SelectedNode, SelectedGroup)
            groupDetails.LoadDetails()
        Catch ex As DocSuiteException
            AjaxAlert(ex.Descrizione)
            FileLogger.Warn(LoggerName, "Errore cancellazione gruppo.", ex)
        End Try
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(ToolBarSearch, RadTreeViewGroups, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, RadTreeViewGroups, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(RadTreeViewGroups, ActionsToolbar)
        AddHandler AjaxManager.AjaxRequest, AddressOf TbltGruppi_AjaxRequest

        AjaxManager.AjaxSettings.AddAjaxSetting(RadTreeViewGroups, pnlGroupDetails, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(groupDetails, groupDetails, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub InitializeControls()
        btnAddGroup.Visible = CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasSecurityGroupAdminRight
        btnRenameGroup.Visible = CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasSecurityGroupAdminRight
        btnDeleteGroup.Visible = CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasSecurityGroupAdminRight
        btnLogGroup.Visible = CommonShared.HasGroupAdministratorRight
    End Sub

    Private Sub LoadGroups()
        Dim groupList As IList(Of SecurityGroups) = Facade.SecurityGroupsFacade.GetRootGroups(SearchGroupTextBox.Text)
        If (Not CommonShared.HasGroupAdministratorRight AndAlso (CommonShared.HasSecurityGroupPowerUserRight OrElse CommonShared.HasSecurityGroupAdminRight)) Then
            groupList = groupList.Where(Function(f) Not CommonShared.HasHiddenSecurityGroupForNotAdminsRight.Any(Function(x) f.Id.ToString() = x)).ToList()
        End If
        RadTreeViewGroups.Nodes(0).Nodes.Clear()
        For Each group As SecurityGroups In groupList
            AddRecursiveNode(RadTreeViewGroups.Nodes(0), group)
        Next

    End Sub

    ''' <summary> Crea il nodo padre, lo valorizza e lo associa ai figli ricorsivamente </summary>
    Private Function AddRecursiveNode(ByRef node As RadTreeNode, ByRef group As SecurityGroups) As RadTreeNode
        Dim newNode As RadTreeNode = AddGroupNode(node, group)

        Dim groups As IList(Of SecurityGroups) = group.Children
        If (groups IsNot Nothing) AndAlso (groups.Count > 0) Then
            For Each child As SecurityGroups In groups
                AddRecursiveNode(newNode, child)
            Next
        End If
        Return newNode
    End Function

    ''' <summary> Crea un nodo col gruppo e lo associa al nodo padre </summary>
    Private Function AddGroupNode(ByRef father As RadTreeNode, ByRef group As SecurityGroups) As RadTreeNode
        Dim newNode As RadTreeNode = New RadTreeNode()

        SetNode(newNode, group)

        father.Nodes.Add(newNode)

        Return newNode
    End Function

    ''' <summary> Imposta il nodo con le proprietà del gruppo. </summary>
    ''' <remarks>Non aggiunge figli</remarks>
    Private Shared Sub SetNode(ByRef nodeToSet As RadTreeNode, ByRef group As SecurityGroups)
        nodeToSet.Value = group.Id.ToString()
        nodeToSet.Text = group.GroupName
        If group.SecurityUsers.Count > 0 Then
            nodeToSet.ImageUrl = "../App_Themes/DocSuite2008/imgset16/GroupMembers.png"
        Else
            nodeToSet.ImageUrl = "../App_Themes/DocSuite2008/imgset16/GroupEmpty.png"
        End If
        nodeToSet.Attributes.Add("UsersCount", group.SecurityUsers.Count.ToString())
        nodeToSet.Attributes.Add("UniqueId", group.UniqueId.ToString())
        nodeToSet.Font.Bold = True
        nodeToSet.Expanded = True
    End Sub
#End Region

End Class