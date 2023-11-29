Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Entity.Tenants
Imports VecompSoftware.DocSuiteWeb.Facade.Common.WebAPI

Partial Class TbltSettoreGes
    Inherits CommonBasePage

#Region "Fields"
    Private _parentRole As Role = Nothing
    Private _role As Role = Nothing
    Private Enum SecurityGroupType
        Dossier
        Protocol
        ProtocolMananger
        Archives
    End Enum

    Private securityGroupTypes As IDictionary(Of SecurityGroupType, String) = New Dictionary(Of SecurityGroupType, String) From {
        {SecurityGroupType.Dossier, "Ds-Set-{0}-Dossier"},
        {SecurityGroupType.Protocol, "Ds-Set-{0}-Protocol"},
        {SecurityGroupType.ProtocolMananger, "Ds-Set-{0}-Protocol-Mananger"},
        {SecurityGroupType.Archives, "Ds-Set-{0}-Archives"}
    }

    Private _contactParent As Contact
    Private _contactRoot As Contact = If(ProtocolEnv.InnerContactRoot.HasValue, Facade.ContactFacade.GetById(ProtocolEnv.InnerContactRoot.Value), Nothing)
#End Region

#Region "Properties"

    Public ReadOnly Property PecEnabled As Boolean
        Get
            Return DocSuiteContext.Current.ProtocolEnv.IsPECEnabled
        End Get
    End Property

    Public ReadOnly Property ProtocolBoxEnabled As Boolean
        Get
            Return DocSuiteContext.Current.ProtocolEnv.ProtocolBoxEnabled
        End Get
    End Property

    Public ReadOnly Property ParentRole As Role
        Get
            If _parentRole Is Nothing Then
                Dim temp As Integer
                If Integer.TryParse(Request.QueryString("ParentRoleID"), temp) Then
                    _parentRole = Facade.RoleFacade.GetById(temp)
                End If
            End If
            Return _parentRole
        End Get
    End Property

    Public ReadOnly Property MainRole As Role
        Get
            If _role Is Nothing Then
                Dim idRole As Integer
                If Integer.TryParse(Request.QueryString("RoleID"), idRole) AndAlso idRole <> 0 Then
                    _role = Facade.RoleFacade.GetById(idRole, False)
                Else
                    _role = New Role()
                End If
            End If

            Return _role
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        If Not (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupTblRoleAdminRight) Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If

        MasterDocSuite.TitleVisible = False

        InitializeAjax()
        If Not IsPostBack Then
            InitializePage()
        End If
    End Sub

    Private Shared Sub grdMailboxes_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles grdMailboxes.ItemDataBound, grdMailboxesNew.ItemDataBound, grdProtocolBoxes.ItemDataBound, grdProtocolBoxesNew.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Return
        End If

        Dim bound As PECMailBox = DirectCast(e.Item.DataItem, PECMailBox)
        Dim lblMailboxName As Label = DirectCast(e.Item.FindControl("lblMailboxName"), Label)
        lblMailboxName.Text = bound.MailBoxName
    End Sub

    Protected Sub btnConferma_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnConferma.Click
        'Verifica che il nome Settore sia sempre impostato
        Select Case Action.ToLower()
            Case "add"
                'Add: If selected node have children with name=input at first level of children, do not add the node
                'Move (sposta): If parent node of node to move have children with name eq text of node to move, do not move the node
                'Modify (modifica): If selected node have brothers with the same name, do not modify
                'Delete (active=0): No rule
                'Restore (active=1): If restored node have brothers with the same name, do not restore

                'OR: do not allow duplicate name for all actions, active or inactive

                If String.IsNullOrEmpty(txtName.Text) Then
                    AjaxAlert("Campo nome obbligatorio")
                    Exit Sub
                End If

                If Facade.RoleFacade.AlreadyExists(txtName.Text, CurrentTenant.TenantAOO.UniqueId) Then
                    AjaxAlert("Esiste già un settore con lo stesso nome")
                    Exit Sub
                End If

                MainRole.Name = txtName.Text
                MainRole.EMailAddress = txtEmail.Text
                MainRole.ServiceCode = txtServiceCod.Text
                MainRole.UriSharepoint = txtUriSharepoint.Text

                'Se ha una role padre allora la imposto
                MainRole.Father = ParentRole
                MainRole.IdTenantAOO = CurrentTenant.TenantAOO.UniqueId
                MainRole.Id = MainRole.Id
                MainRole.Collapsed = cbNewCollapsed.Checked
                Facade.RoleFacade.Save(MainRole)

                Dim pecMailBoxesToUpdate As New Dictionary(Of Short, Boolean)()
                If PecEnabled Then
                    For Each item As KeyValuePair(Of Short, Boolean) In GetSelectedMailboxes(grdMailboxesNew)
                        pecMailBoxesToUpdate.Add(item.Key, item.Value)
                    Next
                End If

                trProtocolBoxNew.Visible = False
                If ProtocolBoxEnabled Then
                    trProtocolBoxNew.Visible = True
                    For Each item As KeyValuePair(Of Short, Boolean) In GetSelectedMailboxes(grdProtocolBoxesNew)
                        pecMailBoxesToUpdate.Add(item.Key, item.Value)
                    Next
                End If

                UpdatePecMailboxList(Facade.RoleFacade.GetById(MainRole.Id), pecMailBoxesToUpdate)

                If ProtocolEnv.SecurityGroupAutogenerateEnabled Then
                    CreateRoleSecurityGroups()
                End If

                If ProtocolEnv.InnerContactRoot.HasValue AndAlso ProtocolEnv.ContactRoleGenerateEnabled Then
                    GenerateRoleContact()
                End If

            Case "rename"
                If String.IsNullOrEmpty(txtNewNome.Text) Then
                    AjaxAlert("Campo nome obbligatorio")
                    Exit Sub
                End If

                If Facade.RoleFacade.AlreadyExists(txtNewNome.Text, CurrentTenant.TenantAOO.UniqueId) AndAlso Not txtOldName.Text.Eq(txtNewNome.Text) Then
                    AjaxAlert("Esiste già un settore con lo stesso nome")
                    Exit Sub
                End If

                MainRole.Name = txtNewNome.Text
                MainRole.EMailAddress = txtNewMail.Text
                MainRole.ServiceCode = txtNewServ.Text
                MainRole.UriSharepoint = txtOldSharepoint.Text
                MainRole.Collapsed = cbCollapsed.Checked
                Facade.RoleFacade.Update(MainRole)
                MainRole.IdTenantAOO = CurrentTenant.TenantAOO.UniqueId
                Facade.RoleFacade.SendUpdateRoleCommand(MainRole)
                Dim pecMailBoxesToUpdate As New Dictionary(Of Short, Boolean)()
                If PecEnabled Then
                    For Each item As KeyValuePair(Of Short, Boolean) In GetSelectedMailboxes(grdMailboxes)
                        pecMailBoxesToUpdate.Add(item.Key, item.Value)
                    Next
                End If

                If ProtocolBoxEnabled Then
                    For Each item As KeyValuePair(Of Short, Boolean) In GetSelectedMailboxes(grdProtocolBoxes)
                        pecMailBoxesToUpdate.Add(item.Key, item.Value)
                    Next
                End If

                UpdatePecMailboxList(Facade.RoleFacade.GetById(MainRole.Id), pecMailBoxesToUpdate)

                Dim associatedContact As Contact = Facade.ContactFacade.GetByIdRole(MainRole.Id)
                If ProtocolEnv.ContactRoleGenerateEnabled AndAlso ProtocolEnv.InnerContactRoot.HasValue AndAlso associatedContact IsNot Nothing AndAlso Facade.ContactFacade.IsChildContact(ProtocolEnv.InnerContactRoot.Value, associatedContact.Id) Then
                    associatedContact.Description = MainRole.Name
                    Facade.ContactFacade.Update(associatedContact)
                End If

            Case "delete"
                If PecEnabled OrElse ProtocolBoxEnabled Then
                    UpdatePecMailboxList(Facade.RoleFacade.GetById(MainRole.Id), Nothing)
                End If
                Dim disableAllChildren As Boolean = DirectCast(sender, RadButton).CommandArgument.Eq("disableAllChildren")
                Facade.RoleFacade.DisableRole(MainRole, disableAllChildren)

            Case "recovery"
                Dim activateAllChildren As Boolean = DirectCast(sender, RadButton).CommandArgument.Eq("activateAllChildren")
                Facade.RoleFacade.ActivateRole(MainRole, activateAllChildren)

            Case "clone"
                Facade.RoleFacade.Clone(MainRole, txtName.Text)
        End Select

        Dim nodeType As String
        If (MainRole.Father Is Nothing) Then
            nodeType = "Role"
        Else
            nodeType = "SubRole"
        End If

        AjaxManager.ResponseScripts.Add(String.Format("CloseWindow('{0}','{1}','{2}');", Action, MainRole.Id, nodeType))

    End Sub

#End Region

#Region " Methods "
    Private Sub CreateRoleSecurityGroups()
        Dim roleNameFragmentsWithoutSpaces As IEnumerable(Of String) = MainRole.Name.Split(" "c).Select(Function(roleFragment) roleFragment.Trim())
        Dim roleNameWithoutSpaces As String = String.Join("", roleNameFragmentsWithoutSpaces)

        For Each securityGroupNameTemplate As KeyValuePair(Of SecurityGroupType, String) In securityGroupTypes
            Dim securityGroupName As String = String.Format(securityGroupNameTemplate.Value, roleNameWithoutSpaces)
            Dim roleGroupName As String = String.Format(securityGroupNameTemplate.Value, roleNameWithoutSpaces)

            Dim securityGroup As SecurityGroups = FacadeFactory.Instance.SecurityGroupsFacade.GetGroupByName(securityGroupName)

            If securityGroup Is Nothing Then
                securityGroup = New SecurityGroups With {.GroupName = securityGroupName}
                FacadeFactory.Instance.SecurityGroupsFacade.Save(securityGroup)
            End If

            Dim roleGroup As RoleGroup = FacadeFactory.Instance.RoleGroupFacade.GetByRole(MainRole).FirstOrDefault(Function(x) x.Name = roleGroupName)

            If roleGroup Is Nothing Then
                roleGroup = New RoleGroup()
                roleGroup.Name = roleGroupName
                roleGroup.Role = MainRole
                roleGroup.SecurityGroup = securityGroup

                SetRoleGroupDefaultRights(roleGroup)
                SetRoleGroupRightsBasedOnSecurityGroupType(securityGroupNameTemplate.Key, roleGroup)

                FacadeFactory.Instance.RoleGroupFacade.Save(roleGroup)
            End If
        Next
    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma, MasterDocSuite.AjaxFlatLoadingPanel)
    End Sub

    Private Sub InitializePage()
        InitializeTreeView()
        Select Case Action.ToLower()
            Case "add"
                Title = "Settori - Aggiungi"
                pnlRinomina.Visible = False
                pnlInserimento.Visible = True

                If PecEnabled Then
                    trNewPEC.Visible = True
                    BindMailboxeControl(grdMailboxesNew, GetMailboxesToSelect(False), False)
                Else
                    trNewPEC.Visible = False
                End If

                If ProtocolBoxEnabled AndAlso ProtocolEnv.RoleGroupProcotolMailBoxRightEnabled Then
                    trProtocolBoxNew.Visible = True
                    BindMailboxeControl(grdProtocolBoxesNew, GetMailboxesToSelect(True), True)
                Else
                    trProtocolBoxNew.Visible = False
                End If

                txtName.Focus()

            Case "rename"

                Title = "Settori - Modifica"
                pnlRinomina.Visible = True
                pnlInserimento.Visible = False
                lblOldName.Text = "Nome:"
                txtOldName.Text = MainRole.Name
                txtOldName.ReadOnly = True
                lblOldMail.Text = "Email:"
                txtOldEmail.Text = MainRole.EMailAddress
                txtOldEmail.ReadOnly = True
                lblNewNome.Text = "Nuovo nome:"
                txtNewNome.Text = MainRole.Name
                lblNewMail.Text = "Nuova Email:"
                txtNewMail.Text = MainRole.EMailAddress
                txtOldServ.Text = MainRole.ServiceCode
                lblNewServ.Text = "Nuovo codice di servizio:"
                txtNewServ.Text = MainRole.ServiceCode
                cbCollapsed.Checked = MainRole.Collapsed = True
                txtOldSharepoint.Text = MainRole.UriSharepoint


                If PecEnabled Then
                    trPEC.Visible = True
                    BindMailboxeControl(grdMailboxes, GetMailboxesToSelect(False), False)
                Else
                    trPEC.Visible = False
                End If

                If ProtocolBoxEnabled Then
                    trProtocolBox.Visible = True
                    BindMailboxeControl(grdProtocolBoxes, GetMailboxesToSelect(True), True)
                Else
                    trProtocolBox.Visible = False
                End If

                txtNewNome.Focus()

            Case "delete", "recovery"
                If Action.Eq("delete") Then
                    Title = "Settori - Elimina"
                Else
                    Title = "Settori - Recupera"
                End If
                pnlRinomina.Visible = False
                pnlInserimento.Visible = True
                txtName.Text = MainRole.Name
                txtName.ReadOnly = True
                txtEmail.Text = MainRole.EMailAddress
                txtEmail.ReadOnly = True

                If PecEnabled Then
                    trNewPEC.Visible = True
                    BindMailboxeControl(grdMailboxesNew, GetMailboxesToSelect(False), False)
                Else
                    trNewPEC.Visible = False
                End If

                'Verifico se posso cancellare il settore selezionato
                If Action.Eq("delete") AndAlso Not Facade.RoleFacade.CanDelete(MainRole) Then
                    ErrLabel.Text =
                        "il settore non può essere cancellato, ci sono Collaborazioni che fanno riferimento ad esso"
                    ErrLabel.Visible = True
                    btnConferma.Enabled = False
                End If

            Case "clone"
                Title = "Settori - Clona"
                pnlRinomina.Visible = False
                pnlInserimento.Visible = True
                trNewPEC.Visible = False
                trProtocolBoxNew.Visible = False
                trCollapseNew.Visible = False
                trEmailNew.Visible = False
                trServiceCodeNew.Visible = False
                trSharepointNew.Visible = False
                txtName.Text = MainRole.Name
        End Select
    End Sub

    Private Sub InitializeTreeView()
        Dim node As New RadTreeNode()
        If (MainRole.Name <> "") Then
            node.Text = MainRole.Name
            If MainRole.Father Is Nothing Then
                node.ImageUrl = ImagePath.SmallRole
            Else
                node.ImageUrl = ImagePath.SmallSubRole
            End If
        Else
            node.ImageUrl = ImagePath.SmallRole
            node.Text = "Settore"
        End If

        RadTreeViewSelectedRole.Nodes.Add(node)
    End Sub

    Private Shared Function GetSelectedMailboxes(ByVal grid As RadGrid) As IDictionary(Of Short, Boolean)
        Dim tmp As New Dictionary(Of Short, Boolean)

        For Each item As GridDataItem In grid.Items
            If item.Selected Then
                Dim radio As RadioButton = CType(item.FindControl("rbtDefaultMailBox"), RadioButton)
                tmp.Add(CShort(item.GetDataKeyValue("Id")), radio.Checked)
            End If
        Next

        Return tmp
    End Function

    Private Function GetMailboxesToSelect(ByVal onlyProtocolBox As Boolean) As IDictionary(Of Short, Boolean)
        Dim tmp As New Dictionary(Of Short, Boolean)

        If MainRole.Mailboxes IsNot Nothing Then
            For Each pecMailBox As PECMailBox In MainRole.Mailboxes.Where(Function(pmb) (pmb.IsProtocolBox.HasValue AndAlso pmb.IsProtocolBox.Value) = onlyProtocolBox).ToList()
                Dim currentPecMailBox As PECMailBox = pecMailBox
                For Each pecMailBoxRole As PECMailBoxRole In From pecMailBoxRole1 In currentPecMailBox.MailBoxRoles Where pecMailBoxRole1.Id.RoleId = MainRole.Id
                    tmp.Add(pecMailBox.Id, pecMailBoxRole.Priority)
                Next
            Next
        End If

        Return tmp
    End Function

    Private Sub BindMailboxeControl(ByVal grid As RadGrid, ByVal mailboxIds As IDictionary(Of Short, Boolean), ByVal onlyProtocolBox As Boolean)
        grid.DataSource = Facade.PECMailboxFacade.GetMoveMailBoxes().Where(Function(pmb) (pmb.IsProtocolBox.HasValue AndAlso pmb.IsProtocolBox.Value) = onlyProtocolBox).ToList()
        grid.DataBind()

        If mailboxIds IsNot Nothing Then
            Dim i As Integer = 0
            For Each item As GridDataItem In grid.Items
                item.Selected = mailboxIds.ContainsKey(CType(item.GetDataKeyValue("Id"), Short))
                Dim radio As RadioButton = CType(item.FindControl("rbtDefaultMailBox"), RadioButton)
                radio.Checked = item.Selected AndAlso mailboxIds(CShort(item.GetDataKeyValue("Id")))
                radio.Attributes.Add("onclick", String.Format("javascript:MutuallyExclusive({0}, this, {1})", grid.ClientID, i))
                i = i + 1
            Next
        End If
    End Sub

    Private Sub UpdatePecMailboxList(ByVal roleToUpdate As Role, ByVal mailboxIdToAdd As IDictionary(Of Short, Boolean))
        If roleToUpdate.Mailboxes Is Nothing Then
            Exit Sub
        End If

        '' Se il comando è l'eliminazione si ferma qui
        If Action.Eq("delete") Then
            roleToUpdate.Mailboxes.Clear()
            Facade.RoleFacade.Update(roleToUpdate, "ProtDB")
            Exit Sub
        End If

        Dim existingMailBoxes As List(Of PECMailBox) = roleToUpdate.Mailboxes.ToList()
        For Each mailBox As PECMailBox In From item In existingMailBoxes Where Not mailboxIdToAdd.ContainsKey(item.Id)
            roleToUpdate.Mailboxes.Remove(mailBox)
        Next

        For Each itemToAdd As KeyValuePair(Of Short, Boolean) In mailboxIdToAdd
            Dim pecMail As PECMailBox = Facade.PECMailboxFacade.GetById(itemToAdd.Key)
            'Se non esiste tra quelle già presenti in DB la aggiungo
            If Not roleToUpdate.Mailboxes.Contains(pecMail) Then
                roleToUpdate.Mailboxes.Add(pecMail)
            End If
        Next

        'Aggiorno la struttura dati
        Facade.RoleFacade.Update(roleToUpdate, "ProtDB")

        '' Aggiorna la priorità
        For Each mbId As KeyValuePair(Of Short, Boolean) In mailboxIdToAdd
            Dim pecmailBoxRole As PECMailBoxRole = Facade.PECMailboxRoleFacade.GetById(New PecMailBoxRoleCompositeKey(roleToUpdate, Facade.PECMailboxFacade.GetById(mbId.Key)))
            pecmailBoxRole.Priority = mbId.Value
            Facade.PECMailboxRoleFacade.Update(pecmailBoxRole)
        Next
    End Sub

    Private Sub SetRoleGroupDefaultRights(roleGroup As RoleGroup)
        roleGroup.ResolutionRights = GroupRights.EmptyRights
        roleGroup.DocumentRights = GroupRights.EmptyRights
        roleGroup.DocumentSeriesRights = GroupRights.EmptyRights
        roleGroup.ProtocolRights = New RoleProtocolRights(GroupRights.EmptyRights)
    End Sub

    Private Sub SetRoleGroupRightsBasedOnSecurityGroupType(securityGroupType As SecurityGroupType, roleGroup As RoleGroup)
        Select Case securityGroupType
            Case SecurityGroupType.Dossier
                CommonUtil.GetInstance().SetGroupRight(roleGroup.DocumentRights, RoleDocumentRightType.RoleEnabled, True)
            Case SecurityGroupType.Protocol
                roleGroup.ProtocolRights = New RoleProtocolRights(GroupRights.EmptyRights) With {.IsRoleEnabled = True}
            Case SecurityGroupType.ProtocolMananger
                Dim roleProtocolRights As RoleProtocolRights = New RoleProtocolRights(GroupRights.EmptyRights) With {.IsRoleEnabled = True, .IsRoleManager = True}
                roleGroup.ProtocolRights = roleProtocolRights
            Case SecurityGroupType.Archives
                CommonUtil.GetInstance().SetGroupRight(roleGroup.DocumentSeriesRights, DocumentSeriesRoleRightPositions.Enabled, True)

        End Select
    End Sub

    Private Sub GenerateRoleContact()
        Dim associatedContact As Contact
        Dim associatedContactParent As Contact = Nothing
        If MainRole.Level > 0 Then
            associatedContactParent = Facade.ContactFacade.GetByIdRole(MainRole.Father.Id)
        End If
        If MainRole.Level = 0 OrElse associatedContactParent Is Nothing Then
            associatedContact = CreateNewContactByRole(_contactRoot, MainRole)
            Facade.ContactFacade.Save(associatedContact)
        Else
            associatedContact = CreateNewContactByRole(associatedContactParent, MainRole)
            Facade.ContactFacade.Save(associatedContact)
        End If
    End Sub

    Private Function CreateNewContactByRole(ByVal contactParent As Contact, ByVal role As Role) As Contact
        Return New Contact() With {
                .Parent = contactParent,
                .Role = role,
                .Description = role.Name,
                .ContactType = New ContactType() With {
                    .Id = "S"c
                },
                .IsActive = True
            }
    End Function
#End Region

End Class



