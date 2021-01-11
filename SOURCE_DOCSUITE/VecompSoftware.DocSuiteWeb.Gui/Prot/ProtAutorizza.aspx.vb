Imports System.Collections.Generic
Imports System.Net.Mail
Imports System.Text
Imports System.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.DTO.Protocols

Public Class ProtAutorizza
    Inherits ProtBasePage

    Private Enum ProtocolRoleUserColumns
        IdRole = 0
        GroupName = 1
        UserName = 2
        Account = 3
    End Enum

#Region " Fields "


#End Region

#Region " Properties "
    Public Property Dirty() As Boolean
        Get
            Return CType(ViewState("Dirty"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            ViewState("Dirty") = value
        End Set
    End Property
    Public Property RoleNoteChanged As IList(Of ProtocolRoleNoteModel)
        Get
            If ViewState("RoleNoteChanged") Is Nothing Then
                ViewState("RoleNoteChanged") = New List(Of ProtocolRoleNoteModel)
            End If
            Return DirectCast(ViewState("RoleNoteChanged"), IList(Of ProtocolRoleNoteModel))
        End Get
        Set(value As IList(Of ProtocolRoleNoteModel))
            ViewState("RoleNoteChanged") = value
        End Set
    End Property
    Private Property PreviousNodeSelected As ProtocolRole
        Get
            Return TryCast(ViewState("PreviousNodeSelected"), ProtocolRole)
        End Get
        Set(value As ProtocolRole)
            ViewState("PreviousNodeSelected") = value
        End Set
    End Property
#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()

        If ProtocolEnv.MultiDomainEnabled AndAlso ProtocolEnv.TenantAuthorizationEnabled Then
            uscAutorizza.TenantEnabled = True
        End If

        If Not IsPostBack Then
            InitializeTab()
            ' Predispongo la nuova valorizzazione dei settori autorizzabili dai manager di distribuzione.
            uscAutorizza.ResetManageableRoles()

            Dirty = False
            Initialize()
        End If
    End Sub

    Private Sub btnConferma_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConferma.Click
        If CurrentProtocol Is Nothing Then
            Exit Sub
        End If

        If Not ProtocolEnv.AllowZeroAuthorizations AndAlso uscAutorizza.GetRoles.IsNullOrEmpty() AndAlso uscAutorizza.GetUsers().IsNullOrEmpty() Then
            AjaxAlert("E' necessario autorizzare almeno un settore.")
            Exit Sub
        End If

        ' Solo per protocolli in entrata o tra uffici, rimuovo gli Users
        If CurrentProtocolRights.IsProtocolTypeDistributable Then
            Facade.ProtocolFacade.RemoveRoleUserAuthorizations(CurrentProtocol, uscProtocolRoleUser.GetRoleValues(False, uscSettori.NodeTypeAttributeValue.RoleUser))
        End If

        ' Aggiungo i settori
        Facade.ProtocolFacade.AddRoleAuthorizations(CurrentProtocol, uscAutorizza.RoleListAdded(), Explicit)

        If CanDeleteRole(CurrentProtocol.GetRoles()) Then
            ' Rimuovo i settori
            Facade.ProtocolFacade.RemoveRoleAuthorizations(CurrentProtocol, uscAutorizza.RoleListRemoved())
        End If

        'Aggiorno le autorizzazioni privacy
        If DocSuiteContext.Current.SimplifiedPrivacyEnabled AndAlso CurrentProtocol.Roles IsNot Nothing AndAlso CurrentProtocol.Roles.Count > 0 Then
            Dim privacyRoles As IList(Of String) = uscAutorizza.GetPrivacyRoles()
            For Each item As ProtocolRole In CurrentProtocol.Roles
                item.Type = Nothing
                If privacyRoles.Contains(item.Role.Id.ToString()) Then
                    item.Type = ProtocolRoleTypes.Privacy
                End If
            Next
        End If

        ' Aggiorno i CC
        If ProtocolEnv.IsDistributionEnabled Then
            ' Log OK
            ' Prima devono essere gestiti i CC non selezionati, poi quelli selezionati per po
            Facade.ProtocolFacade.UpdateRoleAuthorization(CurrentProtocol, uscProtocolRoleUser.GetFullIncrementalPathAttribute(False, uscSettori.NodeTypeAttributeValue.Role), False)
            Facade.ProtocolFacade.UpdateRoleAuthorization(CurrentProtocol, uscProtocolRoleUser.GetFullIncrementalPathAttribute(True, uscSettori.NodeTypeAttributeValue.Role), True)
        End If

        ' Solo per protocolli in entrata o tra uffici, aggiungo gli Users
        If CurrentProtocolRights.IsProtocolTypeDistributable Then
            Facade.ProtocolFacade.AddRoleUserAuthorizations(CurrentProtocol, uscProtocolRoleUser.GetRoleValues(True, uscSettori.NodeTypeAttributeValue.RoleUser))
        End If

        'TODO: modificare salvataggio 
        If DocSuiteContext.Current.SimplifiedPrivacyEnabled Then
            Dim currentUsers As IDictionary(Of String, String) = uscAutorizza.GetUsers()
            Dim addedUsers As IDictionary(Of String, String) = New Dictionary(Of String, String)
            Dim deletedUsers As IList(Of String) = New List(Of String)
            For Each user As KeyValuePair(Of String, String) In currentUsers
                If Not CurrentProtocol.Users.Any(Function(x) x.Account.Eq(user.Key) AndAlso x.Type = ProtocolUserType.Authorization) Then
                    CurrentProtocol.AddUser(user)
                    addedUsers.Add(user)
                End If
            Next
            For Each user As ProtocolUser In CurrentProtocol.Users.Where(Function(x) x.Type = ProtocolUserType.Authorization).ToList()
                If Not currentUsers.Any(Function(x) x.Key.Eq(user.Account)) Then
                    CurrentProtocol.Users.Remove(user)
                    deletedUsers.Add(user.Account)
                End If
            Next
            Facade.ProtocolFacade.Update(CurrentProtocol)
            For Each item As KeyValuePair(Of String, String) In addedUsers

                Facade.ProtocolLogFacade.AddUserAuthorization(CurrentProtocol, item.Value)
            Next
            For Each item As String In deletedUsers
                Facade.ProtocolLogFacade.DelUserAuthorization(CurrentProtocol, item)
            Next
        End If

        Facade.ProtocolFacade.UpdateOnly(CurrentProtocol)

        'Invio comando di modifica alle web api
        Facade.ProtocolFacade.SendUpdateProtocolCommand(CurrentProtocol)

        If ProtocolEnv.RefusedProtocolAuthorizationEnabled AndAlso CommonShared.HasRefusedProtocolGroupsRight AndAlso CurrentProtocol.RejectedRoles.Any() Then
            Facade.ProtocolRejectedRoleFacade.FixRejectedRoles(CurrentProtocol.RejectedRoles)
            Facade.ProtocolLogFacade.Insert(CurrentProtocol, ProtocolLogEvent.PZ, String.Format("Prot. {0} - Corrette autorizzazioni", CurrentProtocol.FullNumber))
        End If


        Dim redirect As Boolean = True
        If ProtocolEnv.AuthorizMailSmtpEnabled.Eq("1"c) Then
            Dim mittenti As New ArrayList
            Dim destinatari As New ArrayList
            Dim authorizMailFrom As String = ProtocolEnv.AuthorizMailFrom
            Dim authorizMailTo As String = RoleFacade.GetEmailAddresses(CurrentProtocol.GetRoles())
            If Not String.IsNullOrEmpty(authorizMailFrom) Then
                mittenti.Add(authorizMailFrom)
            End If
            If Not String.IsNullOrEmpty(authorizMailTo) Then
                Dim mails As Array = authorizMailTo.Split(";"c)
                For Each mail As String In mails
                    destinatari.Add(mail)
                Next
            End If
            redirect = ProtSendMail(CurrentProtocol, destinatari, mittenti)
        End If

        If Not redirect Then
            Exit Sub
        End If

        Dim s As New StringBuilder
        s.AppendFormat("UniqueId={0}", CurrentProtocol.Id)
        If Dirty Then
            s.Append("&Action=MailSettori")
        Else
            s.AppendFormat("&Action={0}", Action)
        End If

        '*****note di settore
        Try
            Dim roleIdsToRemove As IList(Of Integer) = New List(Of Integer)
            If Not RoleNoteChanged Is Nothing Then
                Dim protocolRoleNote As ProtocolRoleNoteModel
                For Each protRole As ProtocolRole In CurrentProtocol.Roles.Where(Function(dc) RoleNoteChanged.Any(Function(x) x.IdRole = dc.Role.Id))
                    protocolRoleNote = RoleNoteChanged.Single(Function(rn) rn.IdRole = protRole.Role.Id)
                    protRole.Note = protocolRoleNote.Note
                    protRole.LastChangedDate = DateTimeOffset.UtcNow
                    protRole.LastChangedUser = DocSuiteContext.Current.User.FullUserName
                    If ProtocolEnv.ProtocolNoteReservedRoleEnabled Then
                        protRole.NoteType = CType(protocolRoleNote.NoteType, ProtocolRoleNoteType)
                    End If
                    CreateFieldChangeLog($"Note settore {protRole.Role.Id} ({protRole.Role.Name})", protRole.Note, protocolRoleNote.Note)
                Next
            End If
            'Modifico eventuali valori non settati in view state
            If uscProtocolRoleUser.SelectedRole IsNot Nothing AndAlso Not RoleNoteChanged.Any(Function(r) r.IdRole.Equals(uscProtocolRoleUser.SelectedRole.Id)) Then
                Dim protRole As ProtocolRole = CurrentProtocol.Roles.SingleOrDefault(Function(g) g.Role.Id.Equals(uscProtocolRoleUser.SelectedRole.Id))
                If protRole IsNot Nothing Then
                    protRole.Note = txtNote.Text
                    protRole.LastChangedDate = DateTimeOffset.UtcNow
                    protRole.LastChangedUser = DocSuiteContext.Current.User.FullUserName
                End If
            End If

            If ProtocolEnv.RefusedProtocolAuthorizationEnabled AndAlso roleIdsToRemove.Count() > 0 Then
                Facade.ProtocolRejectedRoleFacade.RejectProtocols(CurrentProtocol, roleIdsToRemove)
            Else
                Facade.ProtocolFacade.Update(CurrentProtocol)
            End If
        Catch ex As Exception
            FileLogger.Error(LoggerName, "Errore in fase di salvataggio Note di settore di Protocollo.", ex)
            AjaxAlert("Errore in fase di salvataggio Note di settore di Protocollo.")
            Exit Sub
        End Try

        Response.Redirect($"~/Prot/ProtVisualizza.aspx?{CommonShared.AppendSecurityCheck(s.ToString())}")
    End Sub

    Private Sub CreateFieldChangeLog(ByVal message As String, ByVal oldValue As String, ByVal newValue As String)
        If oldValue.Eq(newValue) Then
            Exit Sub
        End If

        Facade.ProtocolLogFacade.Insert(CurrentProtocol, ProtocolLogEvent.PM, message & " (old): " & oldValue)
        Facade.ProtocolLogFacade.Insert(CurrentProtocol, ProtocolLogEvent.PM, message & " (new): " & newValue)
    End Sub

    Private Sub uscAutorizza_RoleEdited(ByVal sender As Object, ByVal e As EventArgs) Handles uscAutorizza.RolesAdded, uscAutorizza.RoleRemoved
        BindProtocolRoleUsers()
    End Sub

    Private Sub uscAutorizza_RoleRemoving(sender As Object, args As RoleEventArgs) Handles uscAutorizza.RoleRemoving
        If args.Role IsNot Nothing AndAlso CurrentProtocol.Roles.Any(Function(f) f.Type = ProtocolRoleTypes.Privacy AndAlso f.Role.Id = args.Role.Id) Then
            If Not Facade.ContainerGroupFacade.HasContainerRight(CurrentProtocol.Container.Id, DocSuiteContext.Current.User.Domain, DocSuiteContext.Current.User.UserName, ProtocolContainerRightPositions.Privacy, DSWEnvironment.Protocol) Then
                args.Cancel = True
                AjaxAlert($"Non è possibile eliminare il settore {PRIVACY_LABEL} selezionato.")
            End If
        End If
    End Sub

    Private Sub uscProtocolRoleUser_OnRoleUserViewModeChanged(ByVal sender As Object, ByVal e As EventArgs) Handles uscProtocolRoleUser.OnRoleUserViewModeChanged
        If uscProtocolRoleUser.CurrentRoleUserViewMode.Value = uscSettori.RoleUserViewMode.RoleUsers Then
            BindProtocolRoleUsers(uscSettori.RoleUserViewMode.Roles)
        Else
            BindProtocolRoleUsers(uscSettori.RoleUserViewMode.RoleUsers)
        End If
    End Sub

    Private Sub uscProtocolRoleUser_OnRoleUserViewManagersChanged(sender As Object, e As EventArgs) Handles uscProtocolRoleUser.OnRoleUserViewManagersChanged
        BindProtocolRoleUsers()
    End Sub

    Protected Sub UscAutorizza_RoleSelected(ByVal sender As Object, ByVal e As RoleEventArgs) Handles uscAutorizza.RoleSelected
        uscAutorizza.SetButtonsVisibility(CanDeleteRole(e.Role))
    End Sub

    Private Sub ProtAutorizza_AjaxRequest(sender As Object, e As AjaxRequestEventArgs)
        Dim arguments As String() = Split(e.Argument, "|")

        Select Case arguments(0)
            Case "DeactivateNode"
                Dim guid As Guid = Guid.Parse(arguments(1))
                Dim protRole As ProtocolRejectedRole = CurrentProtocol.RejectedRoles.SingleOrDefault(Function(s) s.Id.Equals(guid))
                Facade.ProtocolRejectedRoleFacade.DeactivatedRejectedRole(protRole)
                Facade.ProtocolLogFacade.Insert(CurrentProtocol, ProtocolLogEvent.PZ, String.Format("Prot. {0} - Disattivata autorizzazione {1} rigettata", CurrentProtocol.FullNumber, protRole.Role.Name))
                ReloadCurrentProtocolState()
                InitializeTab()
        End Select
    End Sub

    Private Sub rtvContainers_NodeDataBound(sender As Object, e As RadTreeNodeEventArgs) Handles TreeViewRefused.NodeDataBound
        e.Node.ImageUrl = ImagePath.SmallSubRole
        Dim nodeData As RejectedRoleModel = DirectCast(e.Node.DataItem, RejectedRoleModel)
        DirectCast(e.Node.FindControl("RoleName"), Label).Text = nodeData.Name
        DirectCast(e.Node.FindControl("DeactivateRejectedButton"), ImageButton).OnClientClick = String.Concat("return SendDeactivate('", nodeData.UniqueId, "');")
    End Sub
#End Region

#Region " Methods "

    Private Sub Initialize()
        tblAutorizzazioneCompleta.Visible = ProtocolEnv.EnvAuthorizFullEnabled
        uscAutorizza.Visible = True
        uscAutorizzaFull.Visible = True

        If DocSuiteContext.Current.SimplifiedPrivacyEnabled Then
            uscAutorizza.PrivacyAuthorizationButtonVisible = Facade.ContainerGroupFacade.HasContainerRight(CurrentProtocol.Container.Id, DocSuiteContext.Current.User.Domain, DocSuiteContext.Current.User.UserName, ProtocolContainerRightPositions.Privacy, DSWEnvironment.Protocol)
            uscAutorizzaFull.PrivacyAuthorizationButtonVisible = uscAutorizza.PrivacyAuthorizationButtonVisible
            uscAutorizza.InitializePrivacyAuthorization()
            uscAutorizzaFull.InitializePrivacyAuthorization()
            uscAutorizza.UserAuthorizationEnabled = True
            uscAutorizza.InitializeUserAuthorization()
            uscAutorizzaFull.UserAuthorizationEnabled = True
            uscAutorizzaFull.InitializeUserAuthorization()
        End If
        Dim userContainers As Dictionary(Of ProtocolContainerRightPositions, IList(Of Integer)) = Facade.ContainerFacade.GetCurrentUserContainers(DSWEnvironment.Protocol, True)
        Dim idContainer As Integer = CurrentProtocol.Container.Id

        uscAutorizza.HideDeleteButton = Not CanDeleteRole(CurrentProtocol.GetRoles())
        uscAutorizza.SearchByUserEnabled = DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled AndAlso Not DocSuiteContext.Current.ProtocolEnv.DistributionHierarchicalEnabled

        Dim roles As New List(Of Role)
        Dim rolesFull As New List(Of Role)

        If ProtocolEnv.MultiDomainEnabled AndAlso ProtocolEnv.TenantAuthorizationEnabled Then
            uscAutorizza.TenantEnabled = True
            uscAutorizzaFull.TenantEnabled = True
        End If

        uscAutorizza.CurrentProtocol = CurrentProtocol
        If CurrentProtocol.Roles.Count > 0 OrElse (DocSuiteContext.Current.SimplifiedPrivacyEnabled AndAlso CurrentProtocol.Users.Where(Function(u) u.Type = ProtocolUserType.Authorization).Count > 0) Then
            'recupera le autorizzazioni 
            For Each protocolRole As ProtocolRole In CurrentProtocol.Roles
                If (String.IsNullOrEmpty(protocolRole.Rights)) Then
                    roles.Add(protocolRole.Role)
                Else
                    rolesFull.Add(protocolRole.Role)
                End If
            Next

            uscAutorizza.SourceRoles = roles
            If DocSuiteContext.Current.SimplifiedPrivacyEnabled Then
                uscAutorizza.SetProtocolSourceUsers(CurrentProtocol.Users.Where(Function(u) u.Type = ProtocolUserType.Authorization))
            End If
            uscAutorizza.DataBind()
        End If

        If ProtocolEnv.EnvAuthorizFullEnabled Then
            uscAutorizzaFull.SourceRoles = rolesFull
            If DocSuiteContext.Current.SimplifiedPrivacyEnabled Then
                uscAutorizzaFull.SetProtocolSourceUsers(CurrentProtocol.Users.Where(Function(u) u.Type = ProtocolUserType.Authorization))
            End If
            uscAutorizzaFull.DataBind()
        End If

        If DocSuiteContext.Current.SimplifiedPrivacyEnabled Then
            uscAutorizza.SetPrivacyAuthorizationNodes(CurrentProtocol.Roles.Where(Function(r) Not String.IsNullOrEmpty(r.Type) AndAlso r.Type = ProtocolRoleTypes.Privacy AndAlso roles.Contains(r.Role)).Select(Function(x) x.Role.Id).ToArray())
            uscAutorizzaFull.SetPrivacyAuthorizationNodes(CurrentProtocol.Roles.Where(Function(r) Not String.IsNullOrEmpty(r.Type) AndAlso r.Type = ProtocolRoleTypes.Privacy AndAlso rolesFull.Contains(r.Role)).Select(Function(x) x.Role.Id).ToArray())
        End If

        If CurrentProtocolRights.IsContainerDistributable OrElse CurrentProtocol.Type.Id = 1 Then
            BindProtocolRoleUsers(roles, uscSettori.RoleUserViewMode.Roles)
        Else
            BindProtocolRoleUsers(roles, uscSettori.RoleUserViewMode.RoleUsers)
        End If

        tableNote.Visible = ProtocolEnv.SectorNoteDistributionEnabled
    End Sub

    Private Sub InitializeAjax()
        AddHandler MasterDocSuite.AjaxManager.AjaxRequest, AddressOf ProtAutorizza_AjaxRequest
        AddHandler uscProtocolRoleUser.TreeViewControl.NodeClick, AddressOf SelectedRoleChange
        AjaxManager.AjaxSettings.AddAjaxSetting(uscProtocolRoleUser, txtNote, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, tbltRefusedAuthorizations)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscAutorizza, uscAutorizza)
        If ProtocolEnv.IsDistributionEnabled AndAlso CurrentProtocolRights.IsDistributable Then
            AjaxManager.AjaxSettings.AddAjaxSetting(uscAutorizza, uscProtocolRoleUser, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(uscProtocolRoleUser, uscProtocolRoleUser)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, uscProtocolRoleUser)
        End If
    End Sub

    Private Sub InitializeTab()
        'verifica Protocollo
        If CurrentProtocol Is Nothing Then
            Throw New DocSuiteException(
                $"Protocollo ID {CurrentProtocolId}",
                "Protocollo Inesistente",
                Request.Url.ToString(),
                DocSuiteContext.Current.User.FullUserName)
        End If

        uscProtocollo.CurrentProtocol = CurrentProtocol
        uscProtocollo.VisibleProtocollo = True
        uscProtocollo.VisibleAltri = True
        uscProtocollo.VisibleClassificazione = False
        uscProtocollo.VisibleFascicolo = False
        uscProtocollo.VisibleMittentiDestinatari = False
        uscProtocollo.VisibleOggetto = False
        uscProtocollo.VisibleStatoProtocollo = False
        uscProtocollo.VisibleTipoDocumento = False
        uscProtocollo.VisiblePosteWeb = False
        uscProtocollo.VisibleHandler = False

        uscProtocollo.Show()

        If ProtocolEnv.RefusedProtocolAuthorizationEnabled AndAlso CommonShared.HasRefusedProtocolGroupsRight Then
            tbltRefusedAuthorizations.Visible = False
            Dim results As List(Of RejectedRoleModel) = New List(Of RejectedRoleModel)
            If CurrentProtocol.RejectedRoles.Any(Function(r) r.Status = ProtocolRoleStatus.Refused) Then
                tbltRefusedAuthorizations.Visible = True
                results = CurrentProtocol.RejectedRoles.Where(Function(r) r.Status = ProtocolRoleStatus.Refused) _
                .Select(Function(rejectedRole) New RejectedRoleModel() With {.Name = String.Format("{0} - {1} ( {2} )", rejectedRole.Role.Name,
                                                                                     String.Format(DocSuiteContext.Current.ProtocolEnv.ProtRegistrationDateFormat, rejectedRole.RegistrationDate.ToLocalTime.DateTime), rejectedRole.Note),
                                                                             .UniqueId = rejectedRole.Id.ToString()}).ToList()
            End If

            TreeViewRefused.DataFieldID = "UniqueId"
            TreeViewRefused.DataValueField = "UniqueId"
            TreeViewRefused.DataSource = results
            TreeViewRefused.DataBind()

        End If
    End Sub

    Private Overloads Sub BindProtocolRoleUsers(ByVal roleUserViewMode As uscSettori.RoleUserViewMode)
        BindProtocolRoleUsers(uscAutorizza.GetRoles, roleUserViewMode)
    End Sub

    Private Overloads Sub BindProtocolRoleUsers()
        BindProtocolRoleUsers(uscAutorizza.GetRoles, uscProtocolRoleUser.CurrentRoleUserViewMode)
    End Sub

    ''' <summary> Predispone l'usercontrol relativo alle autorizzazioni di distribuzione di protocollo. </summary>
    ''' <param name="roles">Settori autorizzati</param>
    ''' <param name="roleUserViewMode">Modalità di visualizzazione</param>
    Private Overloads Sub BindProtocolRoleUsers(ByVal roles As IList(Of Role), ByVal roleUserViewMode As uscSettori.RoleUserViewMode?)
        If Not ProtocolEnv.IsDistributionEnabled OrElse Not CurrentProtocolRights.IsDistributable Then
            uscProtocolRoleUser.Visible = False
            tblProtocolRoleUser.Visible = False
            Exit Sub
        End If

        uscProtocolRoleUser.Visible = True
        uscProtocolRoleUser.Required = False
        uscProtocolRoleUser.Caption = "Autorizzazioni Responsabile Settore"
        uscProtocolRoleUser.Checkable = True
        uscProtocolRoleUser.TreeViewControl.CheckBoxes = True
        uscProtocolRoleUser.CopiaConoscenzaEnabled = CurrentProtocolRights.IsContainerDistributable
        uscProtocolRoleUser.CurrentRoleUserViewMode = roleUserViewMode

        uscProtocolRoleUser.CurrentProtocol = CurrentProtocol
        uscProtocolRoleUser.SourceRoles = roles.ToList()
        uscProtocolRoleUser.SelectedRoleUserAccount = uscAutorizza.SelectedRoleUserAccount
        uscProtocolRoleUser.ViewDistributableManager = CurrentProtocolRights.IsDistributable AndAlso ProtocolEnv.ProtocolDistributionTypologies.Contains(CurrentProtocol.Type.Id)

        uscProtocolRoleUser.DataBindForRoleUser(Nothing, Nothing, False, ProtocolEnv.SectorNoteDistributionEnabled)
        uscAutorizza.ReadOnly = Not CurrentProtocolRights.IsDistributable

        If Not CurrentProtocolRights.IsContainerDistributable AndAlso CurrentProtocolRights.IsCurrentUserDistributionManager AndAlso DocSuiteContext.Current.ProtocolEnv.DistributionHierarchicalEnabled Then
            ' Se l'utente corrente è solamente manager di distribuzione gli permetto di autorizzare solo sottosettori di quelli correntemente autorizzati.
            uscAutorizza.ManageableRoles = uscProtocolRoleUser.GetManageableRolesParameter()
        End If
    End Sub

    Public Function ProtSendMail(ByVal protocol As Protocol, ByVal mittenti As ArrayList, ByVal destinatari As ArrayList) As Boolean

        If mittenti.Count = 0 Then
            AjaxAlert("Non sono stati specificati i Mittenti")
            Return False
        End If

        If destinatari.Count = 0 Then
            AjaxAlert("Non sono stati specificati i Destinatari")
            Return False
        End If

        If protocol Is Nothing Then
            AjaxAlert("Errore in ricerca Protocollo")
            Return False
        End If
        Try
            Facade.ProtocolFacade.SendMail(protocol)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function CanDeleteRole(role As Role) As Boolean
        Return CanDeleteRole(New List(Of Role) From {role})
    End Function

    Private Function CanDeleteRole(roles As IList(Of Role)) As Boolean
        If DocSuiteContext.Current.ProtocolEnv.IsDistributionEnabled Then
            Return True
        End If
        Dim userContainers As Dictionary(Of ProtocolContainerRightPositions, IList(Of Integer)) = Facade.ContainerFacade.GetCurrentUserContainers(DSWEnvironment.Protocol, True)
        If Not userContainers.Any(Function(f) f.Key = ProtocolContainerRightPositions.Modify) Then
            Return False
        End If
        Dim idContainer As Integer = CurrentProtocol.Container.Id
        Return userContainers.Item(ProtocolContainerRightPositions.Modify).Contains(idContainer)
    End Function

    Private Sub SavePreviousNodeOnViewState()
        If PreviousNodeSelected IsNot Nothing Then
            Dim roleToUpdate As ProtocolRole = CurrentProtocol.Roles.SingleOrDefault(Function(x) x.Id.Equals(PreviousNodeSelected.Id))
            If roleToUpdate IsNot Nothing Then
                If roleToUpdate.Note.GetValueOrEmpty().Equals(txtNote.Text) Then
                    If RoleNoteChanged.Any(Function(r) r.IdRole.Equals(roleToUpdate.Role.Id)) Then
                        Dim itemToRemove As ProtocolRoleNoteModel = RoleNoteChanged.First(Function(r) r.IdRole.Equals(roleToUpdate.Role.Id))
                        RoleNoteChanged.Remove(itemToRemove)
                    End If
                    Dim node As RadTreeNode = uscProtocolRoleUser.TreeViewControl.FindNodeByValue(roleToUpdate.Role.Id.ToString())
                    node.Text = roleToUpdate.Role.Name
                Else
                    If RoleNoteChanged.Any(Function(r) r.IdRole.Equals(roleToUpdate.Role.Id)) Then
                        RoleNoteChanged.First(Function(r) r.IdRole.Equals(roleToUpdate.Role.Id)).Note = txtNote.Text
                    Else
                        Dim itemToAdd As ProtocolRoleNoteModel = New ProtocolRoleNoteModel()
                        itemToAdd.IdRole = roleToUpdate.Role.Id
                        itemToAdd.Note = txtNote.Text
                        RoleNoteChanged.Add(itemToAdd)
                        Dim node As RadTreeNode = uscProtocolRoleUser.TreeViewControl.FindNodeByValue(roleToUpdate.Role.Id.ToString())
                        node.Text = $"{node.Text} *"
                    End If
                End If
            End If
        End If
    End Sub

    Public Sub SelectedRoleChange(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs)
        SavePreviousNodeOnViewState()
        txtNote.Visible = False
        If uscProtocolRoleUser.SelectedRole Is Nothing Then Exit Sub
        txtNote.Visible = True

        Dim protRole As ProtocolRole = CurrentProtocol.Roles.SingleOrDefault(Function(s) s.Role.Id.Equals(uscProtocolRoleUser.SelectedRole.Id))
        If protRole Is Nothing Then Exit Sub

        txtNote.Text = protRole.Note
        PreviousNodeSelected = protRole
        If RoleNoteChanged.Any(Function(r) r.IdRole.Equals(protRole.Role.Id)) Then
            Dim roleChanged As ProtocolRoleNoteModel = RoleNoteChanged.First(Function(r) r.IdRole.Equals(protRole.Role.Id))
            txtNote.Text = roleChanged.Note
        End If
    End Sub
#End Region

End Class
