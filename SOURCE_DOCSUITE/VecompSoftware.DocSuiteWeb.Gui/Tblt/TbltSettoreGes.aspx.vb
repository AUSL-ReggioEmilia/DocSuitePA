Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports System.Collections.Generic

Partial Class TbltSettoreGes
    Inherits CommonBasePage

#Region "Fields"
    Private _parentRole As Role = Nothing
    Private _role As Role = Nothing
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
        MasterDocSuite.TitleVisible = False

        If Not CommonShared.HasGroupAdministratorRight AndAlso Not CommonShared.HasGroupTblRoleAdminRight Then
            AjaxAlert("Sono necessari diritti amministrativi per vedere la pagina.")
            AjaxManager.ResponseScripts.Add("CloseWindow();")
            Exit Sub
        End If

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

                If Facade.RoleFacade.AlreadyExists(txtName.Text) Then
                    AjaxAlert("Esiste già un settore con lo stesso nome")
                    Exit Sub
                End If

                MainRole.Name = txtName.Text
                MainRole.EMailAddress = txtEmail.Text
                MainRole.ServiceCode = txtServiceCod.Text
                MainRole.UriSharepoint = txtUriSharepoint.Text


                'Se ha una role padre allora la imposto
                MainRole.Father = ParentRole
                MainRole.TenantId = DocSuiteContext.Current.CurrentTenant.TenantId
                MainRole.IdRoleTenant = MainRole.Id

                If cbNewCollapsed.Checked Then
                    MainRole.Collapsed = 1
                Else
                    MainRole.Collapsed = 0
                End If

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

            Case "rename"
                If String.IsNullOrEmpty(txtNewNome.Text) Then
                    AjaxAlert("Campo nome obbligatorio")
                    Exit Sub
                End If

                If Facade.RoleFacade.AlreadyExists(txtNewNome.Text) AndAlso Not txtOldName.Text.Eq(txtNewNome.Text) Then
                    AjaxAlert("Esiste già un settore con lo stesso nome")
                    Exit Sub
                End If

                ' Salvo il ruolo prima di eseguire il rename
                Dim roleToHistoricize As Role = Nothing
                If ProtocolEnv.RoleContactHistoricizing Then
                    roleToHistoricize = CType(MainRole.Clone(), Role)
                End If

                MainRole.Name = txtNewNome.Text
                MainRole.EMailAddress = txtNewMail.Text
                MainRole.ServiceCode = txtNewServ.Text
                MainRole.UriSharepoint = txtOldSharepoint.Text


                If cbCollapsed.Checked Then
                    MainRole.Collapsed = 1
                Else
                    MainRole.Collapsed = 0
                End If

                'Cerco in RoleNames se esiste già un contatto con lo stesso idRole. Se lo trovo aggiorno il campo ToDate
                'AltrimentiInserisco prima di modificarlo il contatto vecchio nella RoleNames
                ''
                If ProtocolEnv.RoleContactHistoricizing Then

                    Dim sd As Date? = txtHistoryDate.SelectedDate
                    Dim roleToSplit As RoleName = Nothing
                    MainRole.IsChanged = 1S

                    If sd.HasValue Then
                        ' cerco il ruolo in cui la data selezionata è compresa (cerco anche nei ruoli validi attualmente -> ToDate IS NULL)
                        roleToSplit = Facade.RoleNameFacade.GetRoleNamesHistoryByValidDate(MainRole.Id, sd.Value)
                        If roleToSplit IsNot Nothing Then
                            '' Se trovo un ruolo nella data selezionata, devo spaccarlo in 2 nuovi ruolo storicizzati.
                            HistoryLogic(sd.Value, roleToSplit, MainRole)
                        Else
                            Dim roleOlder As RoleName = Facade.RoleNameFacade.GetRoleNamesOlder(MainRole.Id)
                            If roleOlder Is Nothing Then
                                '' Se non è mai stato storicizzato un ruolo allora eseguo la prima storicizzazione
                                AddFirstOldHistory(sd, MainRole, roleToHistoricize)
                            Else
                                '' Se non trovo un ruolo nella data selezionata, devo inserirlo prima del contatto più vecchio e devo impostarli la data di fine.
                                AddNewBeforeHistory(roleOlder.FromDate, MainRole)
                            End If
                        End If
                    End If

                    ' Salvo nella tabella ROLE il dato attualmente valido oppure salvo i dati appena inseriti
                    If ProtocolEnv.RoleContactHistoricizing Then
                        ' Solo se esiste un ROLE storicizzato, Aggiorno la description con l'ultimo contatto valido.
                        Dim roleToUpdate As RoleName = Facade.RoleNameFacade.GetRoleNameByIdRole(MainRole.Id)
                        If roleToUpdate IsNot Nothing Then
                            MainRole.Name = roleToUpdate.Name
                        End If
                    End If

                End If

                Facade.RoleFacade.Update(MainRole)

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

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma, MasterDocSuite.AjaxFlatLoadingPanel)
    End Sub

    Private Sub EditOldRole(ByVal MainRole As Role, ByVal isChanged As Short)

        MainRole.Name = txtNewNome.Text
        MainRole.EMailAddress = txtNewMail.Text
        MainRole.ServiceCode = txtNewServ.Text
        MainRole.IsChanged = isChanged


        If cbCollapsed.Checked Then
            MainRole.Collapsed = 1
        Else
            MainRole.Collapsed = 0
        End If
        Facade.RoleFacade.UpdateOnly(MainRole)

    End Sub


    Private Sub AddNewBeforeHistory(ByVal ToDate As Date, role As Role)

        Dim roleHistorical As New RoleName With
        {
            .Name = txtNewNome.Text,
            .Role = role,
            .FromDate = txtHistoryDate.SelectedDate.Value,
            .ToDate = ToDate.AddSeconds(-1)
        }

        Facade.RoleNameFacade.Save(roleHistorical)

    End Sub

    ''' <summary>
    ''' Esegue la prima storicizzazione del ruolo.
    ''' 
    ''' Crea un nuovo ruolo
    ''' FROM: RegistrationDate
    ''' TO:   NULL
    ''' 
    ''' Crea un nuovo ruolo storicizzato
    ''' FROM: DataInserita
    ''' TO:   RegistrationDate
    ''' </summary>
    ''' <param name="fromDate"></param>
    Private Sub AddFirstOldHistory(fromDate As Date?, role As Role, roleToHistoricize As Role)
        Dim contactHistorical As RoleName
        Dim contactActual As RoleName

        If DateTime.Compare(role.RegistrationDate.DateTime.Date, fromDate.Value) < 0 Then
            contactHistorical = New RoleName With
            {
                .Name = role.Name,
                .Role = role,
                .FromDate = role.RegistrationDate.DateTime.Date,
                .ToDate = fromDate.Value.AddMilliseconds(-1)
            }

            contactActual = New RoleName With
            {
                .Name = roleToHistoricize.Name,
                .Role = roleToHistoricize,
                .FromDate = fromDate.Value,
                .ToDate = Nothing
            }
        Else
            contactHistorical = New RoleName With
            {
                .Name = role.Name,
                .Role = role,
                .FromDate = fromDate.Value,
                .ToDate = role.RegistrationDate.DateTime.Date.AddMilliseconds(-1)
            }

            contactActual = New RoleName With
            {
                .Name = roleToHistoricize.Name,
                .Role = roleToHistoricize,
                .FromDate = role.RegistrationDate.DateTime.Date,
                .ToDate = Nothing
            }
        End If
        Facade.RoleNameFacade.Save(contactHistorical)
        Facade.RoleNameFacade.Save(contactActual)
    End Sub

    Private Sub AddNewBetweenHistory(fromDate As DateTime, toDate As DateTime)

        Dim roleNew As New RoleName
        roleNew.Name = txtNewNome.Text
        roleNew.Role = MainRole
        roleNew.FromDate = fromDate
        roleNew.ToDate = toDate
        Facade.RoleNameFacade.Save(roleNew)

    End Sub

    Private Sub HistoryLogic(sd As DateTime, roleToSplit As RoleName, role As Role)
        ' salvo il vecchio to date 
        Dim dateToNewRole As Date? = roleToSplit.ToDate

        roleToSplit.ToDate = sd.AddSeconds(-1)
        roleToSplit.Role = role
        Facade.RoleNameFacade.UpdateOnly(roleToSplit)

        'aggiungo sul db il ruolo con le date corrette
        Dim roleToCreate As New RoleName With
        {
            .Name = role.Name,
            .Role = role,
            .FromDate = sd.Date,
            .ToDate = dateToNewRole
        }
        Facade.RoleNameFacade.Save(roleToCreate)

    End Sub




    Private Sub InitializePage()
        InitializeTreeView()

        txtHistoryDate.Visible = ProtocolEnv.RoleContactHistoricizing
        txtHistoryDate.Enabled = ProtocolEnv.RoleContactHistoricizing
        'TODO: pagina da rivedere, troppi doppioni per medesime funzionalità
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
                cbCollapsed.Checked = MainRole.Collapsed = 1
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
        grid.DataSource = Facade.PECMailboxFacade.GetHumanManageable().GetMoveMailBoxes().Where(Function(pmb) (pmb.IsProtocolBox.HasValue AndAlso pmb.IsProtocolBox.Value) = onlyProtocolBox).ToList()
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

#End Region

End Class



