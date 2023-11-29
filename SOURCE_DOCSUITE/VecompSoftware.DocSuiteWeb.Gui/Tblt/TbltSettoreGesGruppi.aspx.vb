Imports System.Collections.Generic
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Partial Public Class TbltSettoreGesGruppi
    Inherits CommonBasePage

#Region " Fields "

    Private _role As Role

#End Region

#Region " Properties "
    Private ReadOnly Property CurrentIdRole() As Integer
        Get
            Return Request.QueryString.GetValue(Of Integer)("IdRole")
        End Get
    End Property

    Public ReadOnly Property CurrentRole() As Role
        Get
            If _role Is Nothing Then

                If CurrentIdRole <> 0 Then
                    _role = FacadeFactory.Instance.RoleFacade.GetById(CurrentIdRole, False)
                Else
                    _role = New Role()
                End If
            End If

            Return _role
        End Get
    End Property

    Private ReadOnly Property CurrentGroupName() As String
        Get
            Return Request.QueryString.GetValueOrDefault(Of String)("GroupName", String.Empty)
        End Get
    End Property

    Private Property _NewCheckedRolesDictionary As Dictionary(Of String, List(Of String))

    Public Property SessionCheckedRolesDictionary As Dictionary(Of String, List(Of String))
        Get
            If Not Session.Item("RolesDictionary") Is Nothing Then
                Return Session.Item("RolesDictionary")
            Else
                Return New Dictionary(Of String, List(Of String))
            End If
        End Get
        Set(value As Dictionary(Of String, List(Of String)))
            If value Is Nothing Then
                Session.Remove("RolesDictionary")
            Else
                Session.Item("RolesDictionary") = value
            End If
        End Set
    End Property
    Protected ReadOnly Property ShowReadonlySecurityGroups As Boolean
        Get
            Dim _readOnlyMode As Boolean = Request.QueryString.GetValueOrDefault("ReadonlySecurityGroups", False)
            Return _readOnlyMode
        End Get
    End Property

#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupTblRoleAdminRight) Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If

        MasterDocSuite.TitleVisible = False
        InitializeAjax()
        InitializeUserControlGroups()
        If Not Page.IsPostBack Then
            _NewCheckedRolesDictionary = New Dictionary(Of String, List(Of String))()
            SessionCheckedRolesDictionary = _NewCheckedRolesDictionary

            Initialize()
        End If
    End Sub

    Private Sub btnConfermaDiritti_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConfermaDiritti.Click
        Save()
        SessionCheckedRolesDictionary = Nothing
        AjaxManager.ResponseScripts.Add("GetRadWindow().close();")
    End Sub

    Private Sub UscGruppiNodeClick(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadTreeNodeEventArgs) Handles uscGruppi.NodeClick
        Dim group As RoleGroup = CType(uscGruppi.GetGroup(e.Node.Text), RoleGroup)
        If group IsNot Nothing Then
            InitializeGroups(group.Name)
            Return
        End If

        InitializeGroups(String.Empty)
        Save()
    End Sub

    Private Sub UscGruppiNodeRemove(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadTreeNodeEventArgs) Handles uscGruppi.NodeRemove
        InitializeRightsPanel(e.Node)
        _NewCheckedRolesDictionary = SessionCheckedRolesDictionary
        If _NewCheckedRolesDictionary.ContainsKey(e.Node.Value) Then
            _NewCheckedRolesDictionary.Remove(e.Node.Value)
        End If
        SessionCheckedRolesDictionary = _NewCheckedRolesDictionary
        Facade.TableLogFacade.Insert("RoleGroup", LogEvent.DL, String.Format("Eliminato Gruppo {0}", e.Node.Text), CurrentRole.UniqueId)
        AjaxManager.ResponseScripts.Add("UpdateGroups();")
    End Sub
#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(uscGruppi, pnlDiritti, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlDiritti)
    End Sub

    Private Sub Initialize()
        lblSeries.Text = ProtocolEnv.DocumentSeriesName
        chbSeries.Text = ProtocolEnv.DocumentSeriesName
        ' Dossier
        If DocSuiteContext.Current.ProtocolEnv.DossierEnabled Then
            lblDocm.Visible = True
            ckbDocm.Visible = True
            cblDocm.Visible = True
            cblDocm.Items.Clear()
        End If

        ' Protocollo
        If DocSuiteContext.Current.IsProtocolEnabled Then
            lblProt.Visible = True
            ckbProt.Visible = True

            If Not DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled AndAlso Not DocSuiteContext.Current.ProtocolEnv.IsPECEnabled Then
                ckbProt.AutoPostBack = False
                cblProt.Visible = False
                cblProt.Items.Clear()
                Exit Sub
            End If

            ckbProt.AutoPostBack = True
            cblProt.Visible = True
            cblProt.Items.Clear()

            If DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled Then
                AddProtocolRoleRightItem(ProtocolRoleRightPositions.Manager)
            End If
            If DocSuiteContext.Current.ProtocolEnv.IsPECEnabled AndAlso DocSuiteContext.Current.ProtocolEnv.RoleGroupPECRightEnabled Then
                AddProtocolRoleRightItem(ProtocolRoleRightPositions.PEC)
            End If
            If DocSuiteContext.Current.ProtocolEnv.RoleGroupProcotolMailBoxRightEnabled Then
                AddProtocolRoleRightItem(ProtocolRoleRightPositions.ProtocolMailBox)

            End If
        End If

        ' Atti
        If CommonInstance.ReslEnabled Then
            lblResl.Visible = True
            ckbResl.Visible = True
            lblResl.Text = FacadeFactory.Instance.TabMasterFacade.TreeViewCaption
            ckbResl.Text = FacadeFactory.Instance.TabMasterFacade.TreeViewCaption
        End If

        InitializeGroups(CurrentGroupName)
    End Sub

    Private Sub SetReadonlyCheckboxes()
        ckbDocm.Enabled = False
        For Each listItem As ListItem In cblDocm.Items
            listItem.Enabled = False
        Next

        ckbProt.Enabled = False
        For Each listItem As ListItem In cblProt.Items
            listItem.Enabled = False
        Next
        ckbResl.Enabled = False
        chbSeries.Enabled = False
    End Sub

    Private Sub InitializeUserControlGroups()
        uscGruppi.CurrentGroups = CurrentRole.RoleGroups
        uscGruppi.ContainerName = CurrentRole.Name
        uscGruppi.CurrentGroupFacade = Facade.RoleGroupFacade
        uscGruppi.GroupName = CurrentGroupName
        If (CurrentRole.Father Is Nothing) Then
            uscGruppi.ContainerImageURL = ImagePath.SmallRole
        Else
            uscGruppi.ContainerImageURL = ImagePath.SmallSubRole
        End If

        uscGruppi.Visible = Not ShowReadonlySecurityGroups
        btnConfermaDiritti.Visible = Not ShowReadonlySecurityGroups
    End Sub

    Protected Sub onCheck(ByVal sender As Object, ByVal e As EventArgs)

        Dim protRights As String = GroupRights.EmptyRights
        Dim docmRights As String = GroupRights.EmptyRights
        Dim reslRights As String = GroupRights.EmptyRights
        Dim seriesRights As String = GroupRights.EmptyRights

        GetRoleRights(protRights, docmRights, reslRights, seriesRights)

        Dim node As RadTreeNode = uscGruppi.SelectedNode
        If node Is Nothing Then
            AjaxAlert("Selezionare un gruppo per l'attività")
            Return
        End If

        Dim GroupRoleList As List(Of String) = New List(Of String)

        GroupRoleList.Add(docmRights)
        GroupRoleList.Add(protRights)
        GroupRoleList.Add(reslRights)
        GroupRoleList.Add(seriesRights)

        _NewCheckedRolesDictionary = SessionCheckedRolesDictionary

        If Not _NewCheckedRolesDictionary.ContainsKey(node.Value) Then
            _NewCheckedRolesDictionary.Add(node.Value, GroupRoleList)
        Else
            _NewCheckedRolesDictionary.Item(node.Value) = GroupRoleList
        End If

        SessionCheckedRolesDictionary = _NewCheckedRolesDictionary

    End Sub

    Private Sub InitializeGroups(ByVal groupName As String)

        InitializeRightsPanel(uscGruppi.SelectedNode)
        If uscGruppi.SelectedNode.ParentNode Is Nothing Then
            Return
        End If

        If String.IsNullOrEmpty(groupName) Then
            ckbDocm.Checked = False
            ckbProt.Checked = False
            ckbResl.Checked = False
            chbSeries.Checked = False

            If DocSuiteContext.Current.IsProtocolEnabled Then
                For Each li As ListItem In cblProt.Items
                    li.Selected = False
                Next li

                If DocSuiteContext.Current.ProtocolEnv.IsPECEnabled AndAlso DocSuiteContext.Current.ProtocolEnv.RoleGroupPECRightEnabled Then
                    cblProt.Items.FindByValue(ProtocolRoleRightPositions.PEC.ToString("D")).Selected = DocSuiteContext.Current.ProtocolEnv.PECRoleRightDefaultValue
                End If
            End If

            If DocSuiteContext.Current.IsDocumentEnabled Then
                For Each li As ListItem In cblDocm.Items
                    li.Selected = False
                Next li
            End If

            Return
        End If

        'Ricerca un determinato gruppo all'interno di un settore
        Dim roleGroup As RoleGroup = CType(uscGruppi.GetGroup(groupName), RoleGroup)
        If roleGroup Is Nothing Then
            Throw New DocSuiteException("Ruoli", "Errore in ricerca")
        End If

        Dim GroupRoleList As List(Of String) = New List(Of String)

        GroupRoleList.Add(roleGroup.DocumentRights)
        GroupRoleList.Add(roleGroup.ProtocolRightsString)
        GroupRoleList.Add(roleGroup.ResolutionRights)
        GroupRoleList.Add(roleGroup.DocumentSeriesRights)

        _NewCheckedRolesDictionary = SessionCheckedRolesDictionary

        If Not _NewCheckedRolesDictionary.ContainsKey(roleGroup.Id.ToString()) Then
            _NewCheckedRolesDictionary.Add(roleGroup.Id.ToString(), GroupRoleList)
        End If

        SessionCheckedRolesDictionary = _NewCheckedRolesDictionary

        PopulateRoleRights(roleGroup.Id.ToString())

        If ShowReadonlySecurityGroups Then
            SetReadonlyCheckboxes()
        End If

    End Sub

    Private Sub PopulateRoleRights(ByVal roleGroupUniqueId As String)
        _NewCheckedRolesDictionary = SessionCheckedRolesDictionary

        Dim roleGroupRights As List(Of String) = _NewCheckedRolesDictionary.Item(roleGroupUniqueId)

        '-- Pratiche
        If ckbDocm.Visible Then
            ckbDocm.Checked = GetDiritti(roleGroupRights.Item(0), DossierRoleRightPositions.Enabled)
        End If

        '-- Protocollo
        If ckbProt.Visible Then
            ckbProt.Checked = GetDiritti(roleGroupRights.Item(1), ProtocolRoleRightPositions.Enabled) ' roleGroup.ProtocolRights.IsRoleEnabled

            If DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled Then
                cblProt.Items.FindByValue(ProtocolRoleRightPositions.Manager.ToString("D")).Selected = GetDiritti(roleGroupRights.Item(1), ProtocolRoleRightPositions.Manager)
            End If
            If DocSuiteContext.Current.ProtocolEnv.IsPECEnabled AndAlso DocSuiteContext.Current.ProtocolEnv.RoleGroupPECRightEnabled Then
                cblProt.Items.FindByValue(ProtocolRoleRightPositions.PEC.ToString("D")).Selected = GetDiritti(roleGroupRights.Item(1), ProtocolRoleRightPositions.PEC)
            End If
            If DocSuiteContext.Current.ProtocolEnv.ProtocolBoxEnabled AndAlso DocSuiteContext.Current.ProtocolEnv.RoleGroupProcotolMailBoxRightEnabled Then
                cblProt.Items.FindByValue(ProtocolRoleRightPositions.ProtocolMailBox.ToString("D")).Selected = GetDiritti(roleGroupRights.Item(1), ProtocolRoleRightPositions.ProtocolMailBox)
            End If
        End If
        '-- Atti
        If ckbResl.Visible Then
            ckbResl.Checked = GetDiritti(roleGroupRights.Item(2), ResolutionRoleRightPositions.Enabled)
        End If
        ' Document Series
        If chbSeries.Visible Then
            chbSeries.Checked = GetDiritti(roleGroupRights.Item(3), DocumentSeriesRoleRightPositions.Enabled)
        End If

    End Sub
    Private Sub GetRoleRights(ByRef protRights As String, ByRef docmRights As String, ByRef reslRights As String, ByRef seriesRights As String)

        '-- Protocollo
        If ckbProt.Visible Then
            CommonUtil.GetInstance.SetGroupRight(protRights, ProtocolRoleRightPositions.Enabled, ckbProt.Checked)

            If ProtocolEnv.IsDistributionEnabled Then
                Dim managerSelected As Boolean? = GetProtocolRoleRightItem(ProtocolRoleRightPositions.Manager)
                If managerSelected.HasValue Then
                    CommonUtil.GetInstance.SetGroupRight(protRights, ProtocolRoleRightPositions.Manager, managerSelected.Value)
                End If
            End If
            If DocSuiteContext.Current.ProtocolEnv.IsPECEnabled AndAlso DocSuiteContext.Current.ProtocolEnv.RoleGroupPECRightEnabled Then
                Dim pecSelected As Boolean? = GetProtocolRoleRightItem(ProtocolRoleRightPositions.PEC)
                If pecSelected.HasValue Then
                    CommonUtil.GetInstance.SetGroupRight(protRights, ProtocolRoleRightPositions.PEC, pecSelected.Value)
                End If
            End If
            If DocSuiteContext.Current.ProtocolEnv.RoleGroupProcotolMailBoxRightEnabled Then
                Dim protocolMailBoxSelected As Boolean? = GetProtocolRoleRightItem(ProtocolRoleRightPositions.ProtocolMailBox)
                If protocolMailBoxSelected.HasValue Then
                    CommonUtil.GetInstance.SetGroupRight(protRights, ProtocolRoleRightPositions.ProtocolMailBox, protocolMailBoxSelected.Value)
                End If
            End If
        End If

        '-- Pratiche
        If ckbDocm.Visible Then
            CommonUtil.GetInstance.SetGroupRight(docmRights, DossierRoleRightPositions.Enabled, ckbDocm.Checked)
        End If

        '-- Atti
        If ckbResl.Visible Then
            CommonUtil.GetInstance.SetGroupRight(reslRights, ResolutionRightPositions.Insert, ckbResl.Checked)
        End If

        '-- Document Series
        If chbSeries.Visible Then
            CommonUtil.GetInstance.SetGroupRight(seriesRights, DocumentSeriesContainerRightPositions.Insert, chbSeries.Checked)
        End If
    End Sub

    Private Sub InitializeRightsPanel(ByVal node As RadTreeNode)
        pnlDiritti.Visible = ShowReadonlySecurityGroups OrElse (node IsNot Nothing AndAlso node.ParentNode IsNot Nothing)
    End Sub

    Private Sub AddItem(ByVal cbl As CheckBoxList, ByVal testo As String, ByVal selected As Boolean)
        Dim li As New ListItem
        li.Text = testo
        li.Selected = selected
        cbl.Items.Add(li)
    End Sub

    Private Sub AddProtocolRoleRightItem(position As ProtocolRoleRightPositions)
        Dim item As New ListItem()
        item.Text = position.GetDescription().ToString()
        item.Value = Convert.ChangeType(position, GetType(Integer)).ToString()

        cblProt.Items.Add(item)
    End Sub

    Private Function GetProtocolRoleRightItem(position As ProtocolRoleRightPositions) As Boolean?
        Dim value As String = Convert.ChangeType(position, GetType(Integer)).ToString()
        Dim found As ListItem = cblProt.Items.FindByValue(value)

        If found IsNot Nothing Then
            Return found.Selected
        End If
        Return Nothing
    End Function

    Private Function GetDiritti(ByVal field As String, ByVal right As Integer) As Boolean
        Dim b As Boolean
        Select Case Mid$(field, right, 1)
            Case "1"
                b = True
            Case Else
                b = False
        End Select
        Return b
    End Function

    Private Sub Save()
        Dim node As RadTreeNode = uscGruppi.SelectedNode

        Try
            If String.IsNullOrEmpty(node.Value) Then
                ' Nuovi diritti di un gruppo sul settore
                Dim rg As New RoleGroup()
                rg.Name = node.Text
                rg.Role = CurrentRole
                rg.DocumentRights = GroupRights.EmptyRights
                rg.DocumentSeriesRights = GroupRights.EmptyRights
                Dim protRigths As String = GroupRights.EmptyRights
                rg.ProtocolRights = New RoleProtocolRights(protRigths)
                rg.ResolutionRights = GroupRights.EmptyRights
                rg.SecurityGroup = FacadeFactory.Instance.SecurityGroupsFacade.GetGroupByName(node.Text)
                If rg.SecurityGroup Is Nothing Then
                    AjaxAlert("Impossibile salvare la modifica.{0}Il gruppo non esiste.", Environment.NewLine)
                End If
                FacadeFactory.Instance.RoleGroupFacade.Save(rg)
                ' se non viene ricaricato genera una lazy exception uscGruppi usa la reflection 
                uscGruppi.CurrentGroups = FacadeFactory.Instance.RoleFacade.GetById(CurrentIdRole, False).RoleGroups
                uscGruppi.Refresh()
            Else
                ' Modifica dei diritti del gruppo sul settore
                _NewCheckedRolesDictionary = SessionCheckedRolesDictionary

                For Each groupId As String In _NewCheckedRolesDictionary.Keys
                    Dim roleGroupRights As List(Of String) = _NewCheckedRolesDictionary(groupId)
                    Dim treeNode As RadTreeView = uscGruppi.GetTreeView
                    Dim nodeToSave As RadTreeNode = treeNode.FindNodeByValue(groupId)
                    Dim rg As RoleGroup = CType(uscGruppi.GetGroup(nodeToSave.Text), RoleGroup)
                    If rg IsNot Nothing Then

                        FacadeFactory.Instance.TableLogFacade.Insert("RoleGroup", LogEvent.UP, String.Format("PreModifica Gruppo {0}: DocumentRights {1}, ProtocolRights {2}, ResolutionRights {3}, DocumentSeriesRights {4}", rg.Name, rg.DocumentRights, rg.ProtocolRights, rg.ResolutionRights, rg.DocumentSeriesRights), rg.Role.UniqueId)
                        rg.DocumentRights = _NewCheckedRolesDictionary.Item(groupId).Item(0)
                        Dim protRigths As String = _NewCheckedRolesDictionary.Item(groupId).Item(1)
                        rg.ProtocolRights = New RoleProtocolRights(protRigths)
                        rg.ResolutionRights = _NewCheckedRolesDictionary.Item(groupId).Item(2)
                        rg.DocumentSeriesRights = _NewCheckedRolesDictionary.Item(groupId).Item(3)
                        FacadeFactory.Instance.RoleGroupFacade.UpdateOnly(rg)
                        uscGruppi.Refresh()
                    End If
                Next

            End If
        Catch ex As Exception
            Throw New DocSuiteException("Errore conferma diritti", "Errore non previsto, contattare assistenza.", ex)
        End Try

        AjaxManager.ResponseScripts.Add("UpdateGroups();")
    End Sub

#End Region

End Class