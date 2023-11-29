Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports VecompSoftware.Helpers.ExtensionMethods
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports Newtonsoft.Json
Imports System.Web
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Commons
Imports APICommon = VecompSoftware.DocSuiteWeb.Entity.Commons
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.WebAPI
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Templates
Imports VecompSoftware.DocSuiteWeb.Entity.Templates
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI

Partial Public Class uscCollRoles
    Inherits DocSuite2008BaseControl

#Region " Fields "

    Private _currentRole As Role
    Private _currentRoleUserFacade As Facade.WebAPI.Commons.RoleUserFacade
    Private _currentTemplateCollaborationFinder As TemplateCollaborationFinder

#End Region

#Region " Properties "
    Private ReadOnly Property CurrentTemplateCollaborationFinder As TemplateCollaborationFinder
        Get
            If _currentTemplateCollaborationFinder Is Nothing Then
                _currentTemplateCollaborationFinder = New TemplateCollaborationFinder(DocSuiteContext.Current.Tenants)
                _currentTemplateCollaborationFinder.ResetDecoration()
                _currentTemplateCollaborationFinder.EnablePaging = False
            End If
            Return _currentTemplateCollaborationFinder
        End Get
    End Property
    Public Property RoleId As Integer
        Get
            Return CType(Me.ViewState("_role"), Integer)
        End Get
        Set(ByVal value As Integer)
            ViewState("_role") = value
            _currentRole = Nothing
        End Set
    End Property

    Public Property EditMode As Boolean
        Get
            Return CType(Me.ViewState("_editMode"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            ViewState("_editMode") = value
        End Set
    End Property

    Public ReadOnly Property CurrentRole As Role
        Get
            If _currentRole Is Nothing Then
                _currentRole = FacadeFactory.Instance.RoleFacade.GetById(Me.RoleId)
            End If
            Return _currentRole
        End Get
    End Property

    Public Property FromCollaboration As Boolean
        Get
            If ViewState(String.Format("{0}_FromCollaboration", ID)) IsNot Nothing Then
                Return DirectCast(ViewState(String.Format("{0}_FromCollaboration", ID)), Boolean)
            End If
            Return False
        End Get
        Set(value As Boolean)
            ViewState(String.Format("{0}_FromCollaboration", ID)) = value
        End Set
    End Property

    Public Property Environment As CollaborationDocumentType?
        Get
            If ViewState(String.Format("{0}_Environment", ID)) IsNot Nothing Then
                Return DirectCast(ViewState(String.Format("{0}_Environment", ID)), CollaborationDocumentType)
            End If
            Return Nothing
        End Get
        Set(value As CollaborationDocumentType?)
            ViewState(String.Format("{0}_Environment", ID)) = value
        End Set
    End Property

    Protected ReadOnly Property CurrentRoleUserFacade As Facade.WebAPI.Commons.RoleUserFacade
        Get
            If _currentRoleUserFacade Is Nothing Then
                _currentRoleUserFacade = New Facade.WebAPI.Commons.RoleUserFacade(DocSuiteContext.Current.Tenants, BasePage.CurrentTenant)
            End If
            Return _currentRoleUserFacade
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjaxSettings()
        pnlButtonsGes.Visible = EditMode
        chkCheckPropagate.Visible = DocSuiteContext.Current.ProtocolEnv.HierarchicalCollaboration
        chkEnableLocation.Visible = DocSuiteContext.Current.ProtocolEnv.CollaborationRightsEnabled

        If ProtocolEnv.RemoteSignEnabled AndAlso CommonShared.HasGroupTblRoleRight Then
            btnEditSignProfile.Visible = True
            btnEditSignProfile.Enabled = False
        End If

        If Not IsPostBack Then
            chkEnableLocation.Checked = False
            ddlDSWEnvironment.Enabled = False
            InitializeCollaborationRights()
        End If

    End Sub

    Protected Sub AddUserAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arguments As String() = Split(e.Argument, "|", 2)
        If arguments.First().Eq("INSERT") AndAlso rtvRoles.SelectedNode IsNot Nothing Then
            Dim users As IList(Of DomainUser) = JsonConvert.DeserializeObject(Of List(Of DomainUser))(HttpUtility.HtmlDecode(arguments.Last()))
            For Each userInfo As DomainUser In users
                InsertUser(userInfo)
            Next
        End If
    End Sub

    Private Sub RtvRolesNodeClick(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs) Handles rtvRoles.NodeClick
        e.Node.Expanded = True
        InitializeButtons(e.Node)
    End Sub

    Protected Sub chkEnableLocation_CheckedChanged(sender As Object, e As EventArgs)
        ddlDSWEnvironment.SelectedValue = DSWEnvironment.Protocol.ToString()
        ddlDSWEnvironment.Enabled = chkEnableLocation.Checked
        LoadUserRoles()
    End Sub

    Private Sub Delete(account As String, userType As String, Optional riseArgumentException As Boolean = True)
        Dim finder As New NHRoleUserFinder()
        finder.RoleIdIn.Add(Me.RoleId)
        finder.AccountIn.Add(account)

        If Not chkCheckPropagate.Checked Then
            finder.TypeIn.Add(userType)
        End If

        If DocSuiteContext.Current.ProtocolEnv.CollaborationRightsEnabled Then
            If Not chkEnableLocation.Checked AndAlso DocSuiteContext.Current.ProtocolEnv.CollaborationRightsEnabled Then
                finder.DSWEnvironmentIn.AddRange(Me.GetAvailableDSWEnvironments())
            Else
                Dim selected As DSWEnvironment = GetSelectedDSWEnvironment()
                finder.DSWEnvironmentIn.Add(selected)
            End If
        End If

        Dim found As IList(Of RoleUser) = finder.List()
        If found.IsNullOrEmpty() Then
            If (riseArgumentException) Then
                Dim message As String = "Impossibile trovare l'utente {0}."
                message = String.Format(message, account)
                Throw New ArgumentException(message)
            End If
            Return
        End If

        If String.Equals(userType, RoleUserType.D.ToString) OrElse String.Equals(userType, RoleUserType.V.ToString) Then
            Dim mapper As MapperRoleUserEntity = New MapperRoleUserEntity()
            Dim apiRoleUser As APICommon.RoleUser = Nothing
            Dim templates As ICollection(Of WebAPIDto(Of TemplateCollaboration)) = Nothing
            Dim accountStr As String()
            Try
                For Each roleUser As RoleUser In found
                    apiRoleUser = mapper.MappingDTO(roleUser)
                    If roleUser.Account.Contains("\") AndAlso templates Is Nothing Then
                        accountStr = roleUser.Account.Split("\"c)
                        templates = WebAPIImpersonatorFacade.ImpersonateFinder(CurrentTemplateCollaborationFinder,
                            Function(impersonationType, wfinder)
                                wfinder.ResetDecoration()
                                wfinder.UserName = accountStr.Last()
                                wfinder.Domain = accountStr.First()
                                wfinder.IdRole = roleUser.Role.Id
                                Return wfinder.DoSearch()
                            End Function)
                    End If
                    CurrentRoleUserFacade.Update(apiRoleUser, UpdateActionType.RoleUserTemplateCollaborationInvalid.ToString())
                Next

                If templates IsNot Nothing AndAlso templates.Any() Then
                    BasePage.AjaxAlert("Attenzione, eliminando l'utente selezionato, si è invalidato almeno un Template di Collaborazione. Verificare i template per apportare le dovute modifiche ai firmatari rimossi.")
                End If

            Catch ex As Exception
                FileLogger.Error(LoggerName, ex.Message, ex)
                Throw ex
            End Try
        End If

        For Each item As RoleUser In found
            FacadeFactory.Instance.RoleUserFacade.Delete(item)
        Next
    End Sub

    Private Sub BtnDeleteClick(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnDelete.Click

        Dim selectedNode As RadTreeNode = rtvRoles.SelectedNode
        If selectedNode Is Nothing Then
            BasePage.AjaxAlert("Selezionare un account", False)
            Exit Sub
        End If

        Dim prevNode As RadTreeNode = selectedNode.Prev
        If prevNode Is Nothing Then
            prevNode = selectedNode.ParentNode
        End If

        If String.IsNullOrEmpty(selectedNode.Value) Then
            Exit Sub
        End If
        Try
            Dim account As String = selectedNode.Value
            Dim roleUserType As String = selectedNode.ParentNode.Attributes("Type")
            Delete(account, roleUserType)

            If ProtocolEnv.FascicleEnabled AndAlso ProtocolEnv.UnifiedCollFunctionDesignerEnabled AndAlso roleUserType.Eq(Data.RoleUserType.S.ToString()) Then
                Delete(account, Data.RoleUserType.SP.ToString(), riseArgumentException:=False)
            End If

            If chkCheckPropagate.Checked Then
                ' Eseguo un refresh dell'albero
                LoadUserRoles(False)
            Else
                selectedNode.Remove()
                prevNode.Selected = True
                InitializeButtons(prevNode)
            End If
        Catch ex As Exception
            FileLogger.Warn(LoggerName, ex.Message, ex)
            BasePage.AjaxAlert("Errore eliminazione Account", False)
        End Try
    End Sub

    Private Sub btnControlla_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnControlla.Click
        ' verifica su utente loggato
        FileLogger.Debug(LoggerName, String.Concat("Controlla utenti in disegno di collaborazione - ", DocSuiteContext.Current.User.FullUserName))

        If Not FacadeFactory.Instance.SecurityUsersFacade.ExistsUser(DocSuiteContext.Current.User.UserName, DocSuiteContext.Current.User.Domain) Then
            BasePage.AjaxAlert("Nessun gruppo legato all'utente corrente.")
            Exit Sub
        End If

        ' lista utenti ammessi
        Dim users As List(Of AccountModel) = New List(Of AccountModel)()
        Dim role As Role = FacadeFactory.Instance.RoleFacade.GetById(Me.RoleId)

        If role IsNot Nothing Then
            For Each securityUser As SecurityUsers In role.RoleGroups.Where(Function(f) f.SecurityGroup IsNot Nothing).SelectMany(Function(f) f.SecurityGroup.SecurityUsers)
                users.Add(New AccountModel(securityUser.Account, securityUser.Description, domain:=securityUser.UserDomain))
            Next
        End If

        ' verifica su utenti memorizzati
        Dim roleUsersToDel As List(Of RoleUser) = New List(Of RoleUser)
        Dim roleUsers As IList(Of RoleUser) = Facade.RoleUserFacade.GetByRoleId(RoleId)

        roleUsersToDel.AddRange(roleUsers.Where(Function(f) Not users.Any(Function(u) u.GetFullUserName().Eq(f.Account))))
        If Not roleUsersToDel.Any() Then
            BasePage.AjaxAlert("Nessuna operazione da eseguire.")
            Exit Sub
        End If

        For Each roleUserToDel As RoleUser In roleUsersToDel
            Try
                Facade.RoleUserFacade.Delete(roleUserToDel)
                Dim node As RadTreeNode = rtvRoles.FindNodeByAttribute("account", roleUserToDel.Account)
                If chkCheckPropagate.Checked Then
                    ' Verifico se lo stesso utente si trova anche in altri ruoli e li cancello
                    For Each item As RoleUser In Facade.RoleUserFacade.GetByRoleIdAndAccount(RoleId, roleUserToDel.Account, String.Empty)
                        Facade.RoleUserFacade.Delete(item)
                    Next
                End If
                If node IsNot Nothing Then
                    node.Remove()
                End If

            Catch ex As Exception
                FileLogger.Warn(LoggerName, "Errore eliminazione Account", ex)
                BasePage.AjaxAlert("Errore eliminazione Account", False)
                Exit Sub
            End Try
        Next
        If chkCheckPropagate.Checked Then
            LoadUserRoles(False)
        End If

        Dim msg As String = String.Format("Eliminati [{0}] account.", roleUsersToDel.Count)
        FileLogger.Info(LoggerName, msg)
        BasePage.AjaxAlert(msg)
        rtvRoles.Nodes(0).Selected = True
    End Sub

    Private Sub BtnSetMainRoleClick(sender As Object, e As EventArgs) Handles btnSetMainRole.Click
        SetIsMainRole(True)
    End Sub

    Private Sub BtnRemoveMainRoleClick(sender As Object, e As EventArgs) Handles btnRemoveMainRole.Click
        SetIsMainRole(False)
    End Sub

    Private Sub ddlDSWEnvironment_SelectedIndexChanged(sender As Object, e As DropDownListEventArgs) Handles ddlDSWEnvironment.SelectedIndexChanged
        LoadUserRoles()
    End Sub

    Private Sub BtnModifyContactRPClick(sender As Object, e As EventArgs) Handles btnModifyContactRP.Click
        Dim url As String = GetCommonContactGesUrl()
        If String.IsNullOrEmpty(url) Then
            Exit Sub
        End If
        Dim script As String = String.Format("return {0}_OpenWindow('{1}', '');", ID, url)
        AjaxManager.ResponseScripts.Add(script)
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnDelete, rtvRoles)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnControlla, rtvRoles)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSetMainRole, rtvRoles)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRemoveMainRole, rtvRoles)
        AjaxManager.AjaxSettings.AddAjaxSetting(chkEnableLocation, ddlDSWEnvironment)

        If DocSuiteContext.Current.ProtocolEnv.CollaborationRightsEnabled Then
            AjaxManager.AjaxSettings.AddAjaxSetting(Me.ddlDSWEnvironment, rtvRoles)
        End If

        If EditMode Then
            AddHandler AjaxManager.AjaxRequest, AddressOf AddUserAjaxRequest
            AjaxManager.AjaxSettings.AddAjaxSetting(rtvRoles, pnlButtonsGes)
        End If
    End Sub

    Private Function GetSelUsersGroupUrl() As String
        Dim sb As New StringBuilder()
        Dim roleUserType As String = rtvRoles.SelectedNode.Attributes("Type")
        sb.AppendFormat("Type=Comm&IdRole={0}&RoleUserType={1}", RoleId, roleUserType)
        If DocSuiteContext.Current.ProtocolEnv.CollaborationRightsEnabled Then
            Dim selected As DSWEnvironment = GetSelectedDSWEnvironment()
            sb.AppendFormat("&DSWEnvironment={0}", selected)
        End If

        Dim qs As String = CommonShared.AppendSecurityCheck(sb.ToString())
        Return String.Concat("../Comm/SelUsersGroup.aspx?", qs)
    End Function
    Private Function GetCommonContactGesUrl() As String
        Dim sb As New StringBuilder()
        sb.Append("Type=Comm&Action=Rename&ActionType=P")

        Dim account As String = rtvRoles.SelectedNode.Attributes("account")
        Dim contacts As IList(Of Contact) = Facade.ContactFacade.GetContactByRole(account, True, parentId:=ProtocolEnv.FascicleContactId, idRole:=RoleId)

        If contacts Is Nothing OrElse contacts.Count = 0 OrElse contacts.Count > 1 Then
            BasePage.AjaxAlert("Non è stato possibile caricare l'utente. Contatto non trovato")
            Return String.Empty
        End If
        sb.AppendFormat("&idContact={0}", contacts(0).Id)
        Dim qs As String = CommonShared.AppendSecurityCheck(sb.ToString())
        Return String.Concat("../UserControl/CommonContactGes.aspx?", qs)
    End Function
    Private Function GetUserRoleProfileUrl(roleUser As String) As String
        Return String.Concat("../Utlt/UserRoleProfile.aspx?Type=Comm&RoleUser=", roleUser, "&MakeReload=", False, "&SignSaveModalityDisabled=", True)
    End Function

    Private Sub InitializeButtons(ByVal node As RadTreeNode)
        btnModifyContactRP.Visible = False
        btnModifyContactRP.OnClientClick = String.Empty

        If node.Value = String.Empty Then
            btnAggiungi.Enabled = True
            btnDelete.Enabled = False

            Dim url As String = GetSelUsersGroupUrl()
            btnAggiungi.OnClientClick = String.Format("return {0}_OpenWindow('{1}', {0}_closeUsers);", ID, url)

            ''Imposta i bottoni per la gestione del settore principale
            btnSetMainRole.Enabled = False
            btnSetMainRole.ToolTip = String.Empty
            btnRemoveMainRole.Enabled = False
            btnRemoveMainRole.ToolTip = String.Empty
        Else
            btnAggiungi.Enabled = False
            btnDelete.Enabled = True

            If node.Attributes("Type").Eq(RoleUserType.RP.ToString()) Then
                btnModifyContactRP.Visible = True
            End If

            btnAggiungi.OnClientClick = ""

            ''Imposta i bottoni per la gestione del settore principale
            ''Se è impostato già come MainRole allora propongo il bottone per rimuovere il Main
            Dim isMainRole As Boolean
            If Boolean.TryParse(node.Attributes("IsMainRole"), isMainRole) Then
                If isMainRole Then
                    btnRemoveMainRole.Visible = True
                    btnRemoveMainRole.Enabled = True
                    btnRemoveMainRole.ToolTip = String.Format("Rimuove dall'utente {0}, come appartenente al gruppo ""{1}"" l'impostazione di settore principale per il settore ""{2}""", node.Text, rtvRoles.SelectedNode.ParentNode.Text, CurrentRole.Name)
                    btnSetMainRole.Visible = False
                Else
                    btnSetMainRole.Visible = True
                    btnSetMainRole.Enabled = True
                    btnSetMainRole.ToolTip = String.Format("Memorizza il settore ""{0}"" come principale per l'utente {1} come appartenente al gruppo ""{2}""", CurrentRole.Name, node.Text, rtvRoles.SelectedNode.ParentNode.Text)
                    btnRemoveMainRole.Visible = False
                End If
            End If

            If node.ParentNode IsNot Nothing AndAlso (node.ParentNode.Text = DocSuiteContext.Current.ProtocolEnv.NomeDirigentiCollaborazione OrElse node.ParentNode.Text = DocSuiteContext.Current.ProtocolEnv.NomeViceCollaborazione) Then
                btnEditSignProfile.Enabled = True
                Dim url As String = GetUserRoleProfileUrl(node.Value.Replace("\", "\\"))
                btnEditSignProfile.OnClientClick = String.Format("return {0}_OpenWindow('{1}', {0}_closeUsers, {2}, {3});", ID, url, 700, 570)
            End If
        End If
    End Sub

    Private Function CreateRootNode(ByVal txt As String, ByVal value As RoleUserType) As RadTreeNode
        Dim tn As New RadTreeNode
        tn.Text = txt
        tn.Attributes("Type") = value.ToString()
        tn.Font.Bold = True
        tn.Expanded = True
        tn.Checkable = False
        tn.ImageUrl = "../Comm/images/Interop/Ruolo.gif"

        Return tn
    End Function

    Private Function CreateUserNode(ByVal roleUser As RoleUser, ByVal check As Boolean) As RadTreeNode
        Dim tn As RadTreeNode = New RadTreeNode
        tn.Text = String.Format("{0} ({1}){2}", roleUser.Description, roleUser.Email, If(roleUser.IsMainRole, " [Settore principale]", String.Empty))
        tn.Value = roleUser.Account
        tn.AddAttribute("account", roleUser.Account)
        tn.Expanded = True
        tn.ImageUrl = "../App_Themes/DocSuite2008/imgset16/user.png"
        If roleUser.Type.Eq(RoleUserType.S.ToString()) Then
            tn.Checkable = check
            tn.Checked = roleUser.Enabled.GetValueOrDefault(False)
        Else
            tn.Checkable = False
        End If
        ''Aggiungo l'attributo MainRole nel nodo
        tn.Attributes.Add("IsMainRole", (roleUser.IsMainRole.HasValue AndAlso roleUser.IsMainRole.Value).ToString)
        tn.Attributes("Type") = roleUser.Type
        Return tn
    End Function

    Public Sub LoadUserRoles(ByVal check As Boolean)
        rtvRoles.Nodes.Clear()

        Dim roleUsers As IList(Of RoleUser)
        If DocSuiteContext.Current.ProtocolEnv.CollaborationRightsEnabled Then
            Dim selected As DSWEnvironment = GetSelectedDSWEnvironment()
            Dim finder As New NHRoleUserFinder()
            finder.RoleIdIn.Add(Me.RoleId)
            finder.DSWEnvironmentIn.Add(selected)
            roleUsers = finder.List()
        Else
            roleUsers = FacadeFactory.Instance.RoleUserFacade.GetByRoleId(Me.RoleId)
            roleUsers = RemoveDuplicates(roleUsers)
        End If

        If ProtocolEnv.UnifiedCollFunctionDesignerEnabled Then
            LoadUnifiedRoleFunctions(roleUsers, check)
        Else
            LoadRoleFunctions(roleUsers, check)
        End If
    End Sub

    ''' <summary>
    ''' Rimuovo i RoleUser duplicati. Mantengo sempre quelli con Id più alto.
    ''' </summary>
    ''' <param name="users"></param>
    ''' <returns></returns>
    Private Function RemoveDuplicates(ByRef users As IList(Of RoleUser)) As List(Of RoleUser)
        Dim distinctRoles As List(Of RoleUser) = New List(Of RoleUser)()
        For Each u As RoleUser In users
            If Not distinctRoles.Where(Function(f) f.Account.Eq(u.Account) AndAlso f.Type = u.Type).Any() Then
                distinctRoles.Add(u)
            Else
                '' Seleziono il roleUser con ID maggiore per tenere una coerenza in fase di visualizzazione
                Dim ruPresent As RoleUser = distinctRoles.Where(Function(f) f.Account.Eq(u.Account) AndAlso f.Type = u.Type).FirstOrDefault()
                If ruPresent.Id < u.Id Then
                    distinctRoles.Remove(ruPresent)
                    distinctRoles.Add(u)
                End If
            End If
        Next
        Return distinctRoles
    End Function

    Public Sub LoadUserRoles()
        LoadUserRoles(False)
        If EditMode Then
            rtvRoles.Nodes(0).Selected = True
            InitializeButtons(rtvRoles.SelectedNode)
        End If
    End Sub

    Private Function ParseRoleUserType(value As String) As RoleUserType
        Dim result As RoleUserType = RoleUserType.X
        If [Enum].TryParse(value, result) Then
            Return result
        End If

        Dim message As String = "Nessun RoleUserType definito per il valore ""{0}""."
        message = String.Format(message, value)
        Throw New ArgumentException(message)
    End Function

    Public Function UpdateCheckedRoles() As Boolean
        Try
            For Each rootNode As RadTreeNode In rtvRoles.Nodes
                For Each node As RadTreeNode In rootNode.Nodes
                    For Each childrenNode As RadTreeNode In node.Nodes
                        If childrenNode.Checkable Then
                            Dim accountName As String = childrenNode.Value
                            Dim roleUserType As RoleUserType = ParseRoleUserType(node.Attributes("Type"))
                            Dim roleUser As IList(Of RoleUser) = GetRoleUsers(accountName, roleUserType)
                            If roleUser.Count = 1 Then
                                Dim first As RoleUser = roleUser.First()
                                first.Enabled = node.Checkable
                                FacadeFactory.Instance.RoleUserFacade.Update(first)
                            End If
                        End If
                    Next
                Next
            Next
        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore controllo ruoli", ex)
            Return False
        End Try
        Return True
    End Function

    Private Function GetRoleUsers(accountName As String, roleUserType As RoleUserType?, dswEnvironment As DSWEnvironment) As IList(Of RoleUser)
        Dim roleUserTypes As IList(Of RoleUserType) = New List(Of RoleUserType)
        If roleUserType.HasValue Then
            roleUserTypes.Add(roleUserType.Value)
        End If
        Return GetRoleUsers(accountName, roleUserTypes, dswEnvironment)
    End Function

    Private Function GetRoleUsers(accountName As String, roleUserTypes As IList(Of RoleUserType), dswEnvironment As DSWEnvironment) As IList(Of RoleUser)
        If DocSuiteContext.Current.ProtocolEnv.CollaborationRightsEnabled Then
            Dim finder As New NHRoleUserFinder()
            finder.RoleIdIn.Add(Me.RoleId)
            finder.AccountIn.Add(accountName)
            finder.DSWEnvironmentIn.Add(dswEnvironment)
            If Not roleUserTypes.IsNullOrEmpty() AndAlso roleUserTypes.Count > 0 Then
                finder.TypeIn.AddRange(roleUserTypes.Select(Function(s) s.ToString()))
            End If

            Return finder.List()
        End If

        If Not roleUserTypes.IsNullOrEmpty() AndAlso roleUserTypes.Count > 0 Then
            Dim roleUsers As List(Of RoleUser) = New List(Of RoleUser)
            For Each roleUserType As RoleUserType In roleUserTypes
                roleUsers.AddRange(FacadeFactory.Instance.RoleUserFacade.GetByRoleIdAndAccount(Me.RoleId, accountName, roleUserType))
            Next
            Return roleUsers
        End If
        Return FacadeFactory.Instance.RoleUserFacade.GetByRoleIdAndAccount(Me.RoleId, accountName, String.Empty)
    End Function

    Private Function GetRoleUsers(accountName As String, roleUserType As RoleUserType?) As IList(Of RoleUser)
        Dim selected As DSWEnvironment = GetSelectedDSWEnvironment()
        Return GetRoleUsers(accountName, roleUserType, selected)
    End Function

    Private Function GetRoleUsers(accountName As String) As IList(Of RoleUser)
        Return GetRoleUsers(accountName, Nothing)
    End Function

    Private Sub InsertUser(userInfo As DomainUser)
        Try
            Dim selectedNode As RadTreeNode = rtvRoles.SelectedNode
            Dim selectedNodeType As RoleUserType = DirectCast([Enum].Parse(GetType(RoleUserType), selectedNode.Attributes("Type")), RoleUserType)
            Dim user As RoleUser = Nothing
            If ProtocolEnv.UnifiedCollFunctionDesignerEnabled AndAlso selectedNodeType.Equals(RoleUserType.S) Then
                If (ProtocolEnv.FascicleEnabled AndAlso Not FromCollaboration) Then
                    user = InsertUser(userInfo, RoleUserType.SP)
                End If
                If (DocSuiteContext.Current.ProtocolEnv.IsCollaborationEnabled AndAlso DocSuiteContext.Current.ProtocolEnv.IsCollaborationGroupEnabled) Then
                    user = InsertUser(userInfo, RoleUserType.S)
                End If
                selectedNode.Nodes.Add(CreateUserNode(user, False))
            Else
                user = InsertUser(userInfo, selectedNodeType)
                selectedNode.Nodes.Add(CreateUserNode(user, False))
            End If

        Catch ex As Exception
            FileLogger.Error(LoggerName, "Errore inserimento utente in settore", ex)
            BasePage.AjaxAlert(ex.Message)
        End Try
    End Sub

    Private Function InsertUser(userInfo As DomainUser, type As RoleUserType) As RoleUser
        Try
            Dim accountDescription As String = userInfo.Name
            Dim accountName As String = userInfo.DisplayName
            Dim isMainRole As Boolean = userInfo.MainRole

            'Recupero tutti gli abbinamenti settore/utente/roleusertype.
            Dim availableRoleUsers As IList(Of RoleUser) = GetRoleUsersByType(type, accountName, GetSelectedDSWEnvironment())

            If Not DocSuiteContext.Current.ProtocolEnv.HierarchicalCollaboration AndAlso availableRoleUsers.Count > 0 AndAlso IsSelectedNodeCollaborationType() Then
                Throw New DocSuiteException("Inserimento Utente in Settore", "Utente già inserito nella gerarchia.")
            End If

            Dim emailAddress As String = String.Empty

            ' Recupero l'indirizzo email dell'utente dalla tabella RoleUser.
            Dim hasEmailAddress As Boolean = availableRoleUsers.Any(Function(f) Not String.IsNullOrEmpty(f.Email))
            ' Se è attiva la propagazione di anche l'indirizzo email, riporto l'indirizzo trovato
            ' Alternativamente valorizza il campo email di solo il primo nodo utente aggiunto al disegno dell'ufficio.
            If DocSuiteContext.Current.ProtocolEnv.PropageCollaborationEmailAdd AndAlso hasEmailAddress Then
                emailAddress = availableRoleUsers.Where(Function(f) Not String.IsNullOrEmpty(f.Email)).Select(Function(s) s.Email).First()
            End If

            ' Qualora non sia presente, provo a recuperarlo da AD.
            If Not hasEmailAddress Then
                emailAddress = FacadeFactory.Instance.UserLogFacade.EmailOfUser(accountName, True)
            End If

            ' Qualora l'utente corrente non abbia un indirizzo email configurato, notifico a video.
            If Not hasEmailAddress AndAlso String.IsNullOrEmpty(emailAddress) Then
                Throw New DocSuiteException("Errore inserimento utente in settore", String.Format("L'utente [{0}] non dispone di email.", accountDescription))
            End If

            Dim user As RoleUser = CreateRoleUser(RoleId, type, accountDescription, accountName, emailAddress, isMainRole, True)
            FacadeFactory.Instance.RoleUserFacade.Save(user)

            If user.Type.Eq(RoleUserType.RP.ToString()) Then
                Dim userEmail As String = FacadeFactory.Instance.UserLogFacade.EmailOfUser(accountName, True)
                Dim contacts As IList(Of Contact) = Facade.ContactFacade.GetContactByRole(accountName, True, parentId:=ProtocolEnv.FascicleContactId, idRole:=RoleId)

                If contacts Is Nothing OrElse Not contacts.Count() > 0 Then
                    Dim adUser As AccountModel = CommonAD.GetAccount(user.Account)
                    Dim newContact As Contact = New Contact With {
                        .IsActive = True,
                        .Description = String.Concat(adUser.LastName, "|", adUser.FirstName),
                        .ContactType = New ContactType(ContactType.Person),
                        .EmailAddress = userEmail,
                        .SearchCode = accountName,
                        .Parent = Facade.ContactFacade.GetById(ProtocolEnv.FascicleContactId)
                    }
                    Facade.ContactFacade.Save(newContact)

                    BasePage.AjaxAlert($"Inserito {DocSuiteContext.Current.ProtocolEnv.FascicleRoleRPLabel} [{newContact.Description}] senza codice fiscale.")
                End If
            End If

            If (Not user.Type.Eq(RoleUserType.M.ToString()) AndAlso Not user.Type.Eq(RoleUserType.U.ToString())) Then
                AddPropagate(user)
                If Not chkEnableLocation.Checked AndAlso DocSuiteContext.Current.ProtocolEnv.CollaborationRightsEnabled Then
                    For Each dswEnv As DSWEnvironment In GetAvailableDSWEnvironments()
                        CheckPropagate(user, dswEnv)
                    Next
                Else
                    CheckPropagate(user)
                End If
            End If
            Return user
        Catch ex As Exception
            FileLogger.Error(LoggerName, "Errore inserimento utente in settore", ex)
            BasePage.AjaxAlert(ex.Message)
        End Try
        Return Nothing
    End Function

    ''' <summary>
    ''' Recupera la lista di RoleUser per singolo contesto dato un RoleUserType.
    ''' </summary>
    ''' <remarks>
    ''' Es. Se RoleUserType = RP (Responsabile di procedimento), saranno recuperati i RoleUser con type RP e SP perchè sono Type di Fascicolo.
    ''' </remarks>
    ''' <param name="userType"></param>
    ''' <param name="accountName"></param>
    ''' <param name="environment"></param>
    ''' <returns></returns>
    Private Function GetRoleUsersByType(userType As RoleUserType, accountName As String, environment As DSWEnvironment) As IList(Of RoleUser)
        Select Case userType
            Case RoleUserType.RP
            Case RoleUserType.SP
                'Tutti i roleuser per fascicolo
                Return GetRoleUsers(accountName, New List(Of RoleUserType) From {RoleUserType.RP, RoleUserType.SP}, environment)
            Case RoleUserType.M
            Case RoleUserType.U
                'Tutti i roleuser per protocollo
                Return GetRoleUsers(accountName, New List(Of RoleUserType) From {RoleUserType.M, RoleUserType.U}, environment)
            Case RoleUserType.MP
                'Tutti i roleuser per protocollo
                Return GetRoleUsers(accountName, New List(Of RoleUserType) From {RoleUserType.MP}, environment)
            Case Else
                Return GetRoleUsers(accountName, New List(Of RoleUserType) From {RoleUserType.D, RoleUserType.S, RoleUserType.V, RoleUserType.X}, environment)
        End Select
        Return New List(Of RoleUser)
    End Function

    Private Sub AddPropagate(roleUser As RoleUser)
        If Not chkEnableLocation.Checked AndAlso DocSuiteContext.Current.ProtocolEnv.CollaborationRightsEnabled Then
            Dim allPropagate As Boolean = True
            For Each dswEnv As DSWEnvironment In GetAvailableDSWEnvironments().Where(Function(x) Not x = roleUser.DSWEnvironment)
                If GetRoleUsers(roleUser.Account, New List(Of RoleUserType), dswEnv).Count > 0 AndAlso Not DocSuiteContext.Current.ProtocolEnv.HierarchicalCollaboration Then
                    allPropagate = False
                    Continue For
                End If

                Dim user As RoleUser = DirectCast(roleUser.Clone(), RoleUser)
                user.DSWEnvironment = dswEnv
                FacadeFactory.Instance.RoleUserFacade.Save(user)
            Next

            If Not allPropagate Then
                BasePage.AjaxAlert("Non è stato possibile propagare l'utente in tutte le location in quanto già presente")
            End If
        End If
    End Sub

    Private Sub CheckPropagate(roleUser As RoleUser)
        CheckPropagate(roleUser, Nothing)
    End Sub

    Private Sub CheckPropagate(roleUser As RoleUser, dswEnv As DSWEnvironment?)
        If chkCheckPropagate.Checked Then
            Select Case roleUser.Type
                Case RoleUserType.D.ToString()
                    Dim vice As RoleUser = DirectCast(roleUser.Clone(), RoleUser)
                    vice.Email = String.Empty
                    vice.IsMainRole = False
                    vice.Type = RoleUserType.V.ToString()
                    If dswEnv.HasValue Then
                        vice.DSWEnvironment = dswEnv.Value
                    End If
                    FacadeFactory.Instance.RoleUserFacade.Save(vice)

                    Dim segreteria As RoleUser = DirectCast(roleUser.Clone(), RoleUser)
                    segreteria.Email = String.Empty
                    segreteria.IsMainRole = False
                    segreteria.Type = RoleUserType.S.ToString()
                    If dswEnv.HasValue Then
                        segreteria.DSWEnvironment = dswEnv.Value
                    End If
                    FacadeFactory.Instance.RoleUserFacade.Save(segreteria)

                Case RoleUserType.V.ToString()
                    Dim segreteria As RoleUser = DirectCast(roleUser.Clone(), RoleUser)
                    segreteria.Email = String.Empty
                    segreteria.IsMainRole = False
                    segreteria.Type = RoleUserType.S.ToString()
                    If dswEnv.HasValue Then
                        segreteria.DSWEnvironment = dswEnv.Value
                    End If
                    FacadeFactory.Instance.RoleUserFacade.Save(segreteria)
            End Select

            ' Eseguo un refresh dell'albero
            LoadUserRoles(False)
        End If
    End Sub

    Private Function CreateRoleUser(ByVal idRole As Integer, ByVal roleUserType As String, ByVal description As String, ByVal account As String, ByVal email As String, ByVal isMainRole As Boolean, ByVal enabled As Boolean) As RoleUser
        Return CreateRoleUser(idRole, DirectCast([Enum].Parse(GetType(RoleUserType), roleUserType), RoleUserType), description, account, email, isMainRole, enabled)
    End Function

    Private Function CreateRoleUser(ByVal idRole As Integer, ByVal roleUserType As RoleUserType, ByVal description As String, ByVal account As String, ByVal email As String, ByVal isMainRole As Boolean, ByVal enabled As Boolean) As RoleUser
        Dim user As RoleUser = New RoleUser()
        user.Role = FacadeFactory.Instance.RoleFacade.GetById(idRole)
        user.Type = roleUserType.ToString
        user.Description = description
        user.Account = account
        user.Email = email
        user.Enabled = enabled
        user.IsMainRole = isMainRole

        DecorateForCollaborationRights(user)

        Return user
    End Function

    Private Function GetAvailableDSWEnvironments() As List(Of DSWEnvironment)
        Dim result As New List(Of DSWEnvironment)
        If DocSuiteContext.Current.IsProtocolEnabled Then
            result.Add(DSWEnvironment.Protocol)
        End If
        If DocSuiteContext.Current.IsResolutionEnabled Then
            result.Add(DSWEnvironment.Resolution)
        End If
        If DocSuiteContext.Current.ProtocolEnv.DocumentSeriesEnabled Then
            result.Add(DSWEnvironment.DocumentSeries)
        End If
        Return result
    End Function

    Private Sub InitializeCollaborationRights()
        If Not DocSuiteContext.Current.ProtocolEnv.CollaborationRightsEnabled OrElse FromCollaboration Then
            Return
        End If

        pnlCollaborationRights.Visible = True
    End Sub

    Public Sub BindDdlDSWEnvironment()
        If Not DocSuiteContext.Current.ProtocolEnv.CollaborationRightsEnabled Then
            Return
        End If

        ddlDSWEnvironment.Items.Clear()
        Dim available As List(Of DSWEnvironment) = GetAvailableDSWEnvironments()
        If available.IsNullOrEmpty() Then
            Return
        End If

        For Each env As DSWEnvironment In available
            Dim item As New DropDownListItem()
            item.Value = env.ToString()
            Select Case env
                Case DSWEnvironment.Protocol
                    item.Text = "Protocollo"
                    item.ImageUrl = "~/Comm/Images/DocSuite/Protocollo16.png"
                Case DSWEnvironment.Resolution
                    item.Text = FacadeFactory.Instance.TabMasterFacade.TreeViewCaption
                    item.ImageUrl = "~/Comm/Images/DocSuite/Atti16.png"
                Case DSWEnvironment.DocumentSeries
                    item.Text = DocSuiteContext.Current.ProtocolEnv.DocumentSeriesName
                    item.ImageUrl = ImagePath.SmallDocumentSeries
                Case Else
                    item.Text = item.Value
            End Select

            ddlDSWEnvironment.Items.Add(item)
        Next

        If ddlDSWEnvironment.Items.Count > 0 Then
            ddlDSWEnvironment.Items.First().Selected = True
        End If
    End Sub

    Private Function GetSelectedDSWEnvironment() As DSWEnvironment
        If String.IsNullOrWhiteSpace(Me.ddlDSWEnvironment.SelectedValue) Then
            Return GetCollaborationEnvironment()
        End If

        Dim selectedValue As String = ddlDSWEnvironment.SelectedValue
        Dim result As DSWEnvironment = DSWEnvironment.Any
        If [Enum].TryParse(selectedValue, result) Then
            Return result
        End If

        Dim message As String = "Nessun DSWEnvironment definito per il valore ""{0}""."
        message = String.Format(message, selectedValue)
        Throw New ArgumentException(message)
    End Function

    Private Function GetCollaborationEnvironment() As DSWEnvironment
        If Not Environment.HasValue Then
            Return DSWEnvironment.Any
        End If

        Select Case Environment.Value
            Case CollaborationDocumentType.A,
                CollaborationDocumentType.D,
                CollaborationDocumentType.O
                Return DSWEnvironment.Resolution
            Case CollaborationDocumentType.P,
                CollaborationDocumentType.U
                Return DSWEnvironment.Protocol
            Case CollaborationDocumentType.S
                Return DSWEnvironment.DocumentSeries
        End Select
    End Function

    Private Sub DecorateForCollaborationRights(ByRef user As RoleUser)
        If Not DocSuiteContext.Current.ProtocolEnv.CollaborationRightsEnabled Then
            Return
        End If

        Dim selected As DSWEnvironment = GetSelectedDSWEnvironment()
        If Not selected.Equals(DSWEnvironment.Any) Then
            user.DSWEnvironment = selected
        End If
    End Sub

    Private Sub SetIsMainRole(isMainRole As Boolean)
        Dim selectedNode As RadTreeNode = rtvRoles.SelectedNode
        If selectedNode Is Nothing Then
            BasePage.AjaxAlert("Selezionare un account", False)
            Exit Sub
        End If
        Dim accountName As String = selectedNode.Value
        Dim roleUserType As RoleUserType = ParseRoleUserType(selectedNode.ParentNode.Attributes("Type"))
        If Not chkEnableLocation.Checked AndAlso DocSuiteContext.Current.ProtocolEnv.CollaborationRightsEnabled Then
            For Each dswEnv As DSWEnvironment In GetAvailableDSWEnvironments()
                UpdateRoleUserMainRole(isMainRole, accountName, roleUserType, dswEnv)
                If ProtocolEnv.FascicleEnabled AndAlso ProtocolEnv.UnifiedCollFunctionDesignerEnabled AndAlso roleUserType.Equals(RoleUserType.S) Then
                    UpdateRoleUserMainRole(isMainRole, accountName, RoleUserType.SP, dswEnv)
                End If
            Next
        Else
            UpdateRoleUserMainRole(isMainRole, accountName, roleUserType)
            If ProtocolEnv.FascicleEnabled AndAlso ProtocolEnv.UnifiedCollFunctionDesignerEnabled AndAlso roleUserType.Equals(RoleUserType.S) Then
                UpdateRoleUserMainRole(isMainRole, accountName, RoleUserType.SP)
            End If
        End If
        LoadUserRoles(False)
    End Sub

    Private Function IsSelectedNodeCollaborationType() As Boolean
        Dim selectedNode As RadTreeNode = rtvRoles.SelectedNode
        Dim selectedNodeType As RoleUserType = DirectCast([Enum].Parse(GetType(RoleUserType), selectedNode.Attributes("Type")), RoleUserType)

        Select Case selectedNodeType
            Case RoleUserType.D
            Case RoleUserType.S
            Case RoleUserType.V
            Case RoleUserType.X
                Return True
        End Select

        Return False
    End Function

    Private Sub LoadUnifiedRoleFunctions(roleUsers As IList(Of RoleUser), ByVal check As Boolean)
        Dim rootFunc As RadTreeNode = Nothing
        rootFunc = New RadTreeNode("Funzioni", "FunzioniRoot") With {.Expanded = True, .Checkable = False}
        rtvRoles.Nodes.Add(rootFunc)

        If DocSuiteContext.Current.ProtocolEnv.IsCollaborationEnabled AndAlso
                               DocSuiteContext.Current.ProtocolEnv.IsCollaborationGroupEnabled Then
            Dim rootDirettori As RadTreeNode = CreateRootNode(DocSuiteContext.Current.ProtocolEnv.NomeDirigentiCollaborazione, RoleUserType.D)
            rootFunc.Nodes.Add(rootDirettori)
            Dim rootViceDirettori As RadTreeNode = CreateRootNode(DocSuiteContext.Current.ProtocolEnv.NomeViceCollaborazione, RoleUserType.V)
            rootFunc.Nodes.Add(rootViceDirettori)
        End If

        If (DocSuiteContext.Current.ProtocolEnv.IsCollaborationEnabled AndAlso
                           DocSuiteContext.Current.ProtocolEnv.IsCollaborationGroupEnabled) Or
                           (ProtocolEnv.FascicleEnabled AndAlso Not FromCollaboration) Then
            Dim rootSegreteria As RadTreeNode = CreateRootNode(DocSuiteContext.Current.ProtocolEnv.NomeSegreteria, RoleUserType.S)
            rootFunc.Nodes.Add(rootSegreteria)
        End If

        If ProtocolEnv.FascicleEnabled AndAlso Not FromCollaboration Then
            Dim rootResponsabiliProcedimento As RadTreeNode = CreateRootNode(DocSuiteContext.Current.ProtocolEnv.FascicleRoleRPLabel, RoleUserType.RP)
            rootFunc.Nodes.Add(rootResponsabiliProcedimento)
        End If

        If DocSuiteContext.Current.PrivacyEnabled AndAlso Not FromCollaboration Then
            Dim rootPrivacyManager As RadTreeNode = CreateRootNode(CommonBasePage.PRIVACY_LABEL, RoleUserType.MP)
            rootFunc.Nodes.Add(rootPrivacyManager)
        End If

        For Each item As RoleUser In roleUsers
            If rootFunc IsNot Nothing Then
                Dim type As String = item.Type
                'Poiché ho sia il contatto di segreteria di collaborazione sia quello di segreteria fascicoli,
                'allora posso omettere di inserire quel contatto, perché risulterebbe duplicato
                Select Case type
                    Case RoleUserType.SP.ToString()
                        Continue For
                End Select
                Dim childNode As RadTreeNode = rootFunc.Nodes.FindNodeByAttribute("Type", item.Type)
                If childNode IsNot Nothing Then
                    Dim roleUserNode As RadTreeNode = CreateUserNode(item, check)
                    childNode.Nodes.Add(roleUserNode)
                End If
            End If
        Next
    End Sub

    Private Sub LoadRoleFunctions(roleUsers As IList(Of RoleUser), ByVal check As Boolean)
        Dim rootFasc As RadTreeNode = Nothing
        Dim rootColl As RadTreeNode = Nothing
        Dim rootProt As RadTreeNode = Nothing
        If ProtocolEnv.FascicleEnabled AndAlso Not FromCollaboration Then
            rootFasc = New RadTreeNode("Fascicoli", "FascicoliRoot") With {.Expanded = True, .Checkable = False}
            rtvRoles.Nodes.Add(rootFasc)

            Dim rootResponsabiliProcedimento As RadTreeNode = CreateRootNode(DocSuiteContext.Current.ProtocolEnv.FascicleRoleRPLabel, RoleUserType.RP)
            rootFasc.Nodes.Add(rootResponsabiliProcedimento)
            Dim rootSegreteriaProcedimento As RadTreeNode = CreateRootNode(DocSuiteContext.Current.ProtocolEnv.FascicleRoleSPLabel, RoleUserType.SP)
            rootFasc.Nodes.Add(rootSegreteriaProcedimento)
        End If

        If (DocSuiteContext.Current.PrivacyEnabled OrElse ProtocolEnv.IsDistributionEnabled) AndAlso Not FromCollaboration Then
            rootProt = New RadTreeNode("Protocollo", "ProtocolloRoot") With {.Expanded = True, .Checkable = False}
            rtvRoles.Nodes.Add(rootProt)

            'If ProtocolEnv.IsDistributionEnabled Then
            '    Dim rootManager As RadTreeNode = CreateRootNode("Manager", RoleUserType.M)
            '    rootProt.Nodes.Add(rootManager)
            '    Dim rootUtenti As RadTreeNode = CreateRootNode("Utenti", RoleUserType.U)
            '    rootProt.Nodes.Add(rootUtenti)
            'End If

            If DocSuiteContext.Current.PrivacyEnabled Then
                Dim rootPrivacyManager As RadTreeNode = CreateRootNode(CommonBasePage.PRIVACY_LABEL, RoleUserType.MP)
                rootProt.Nodes.Add(rootPrivacyManager)
            End If

        End If

        If DocSuiteContext.Current.ProtocolEnv.IsCollaborationEnabled AndAlso DocSuiteContext.Current.ProtocolEnv.IsCollaborationGroupEnabled Then
            rootColl = New RadTreeNode("Collaborazione", "CollaborazioneRoot") With {.Expanded = True, .Checkable = False}
            rtvRoles.Nodes.Add(rootColl)
            Dim rootDirettori As RadTreeNode = CreateRootNode(DocSuiteContext.Current.ProtocolEnv.NomeDirigentiCollaborazione, RoleUserType.D)
            rootColl.Nodes.Add(rootDirettori)
            Dim rootViceDirettori As RadTreeNode = CreateRootNode(DocSuiteContext.Current.ProtocolEnv.NomeViceCollaborazione, RoleUserType.V)
            rootColl.Nodes.Add(rootViceDirettori)
            Dim rootSegreteria As RadTreeNode = CreateRootNode(DocSuiteContext.Current.ProtocolEnv.NomeSegreteria, RoleUserType.S)
            rootColl.Nodes.Add(rootSegreteria)
        End If

        For Each item As RoleUser In roleUsers
            Dim rootNode As RadTreeNode = Nothing
            Select Case item.Type
                Case RoleUserType.RP.ToString(),
                    RoleUserType.SP.ToString()
                    If rootFasc IsNot Nothing Then
                        rootNode = rootFasc
                    End If
                Case RoleUserType.M.ToString(),
                     RoleUserType.U.ToString(),
                     RoleUserType.MP.ToString()
                    If rootProt IsNot Nothing Then
                        rootNode = rootProt
                    End If
                Case Else
                    If rootColl IsNot Nothing Then
                        rootNode = rootColl
                    End If
            End Select
            If rootNode IsNot Nothing Then
                Dim childNode As RadTreeNode = rootNode.Nodes.FindNodeByAttribute("Type", item.Type)
                If childNode IsNot Nothing Then
                    Dim roleUserNode As RadTreeNode = CreateUserNode(item, check)
                    childNode.Nodes.Add(roleUserNode)
                End If
            End If
        Next
    End Sub

    Private Sub UpdateRoleUserMainRole(isMainRole As Boolean, accountName As String, type As Data.RoleUserType, Optional dswEnv As DSWEnvironment = Nothing)
        Dim roleUser As RoleUser
        If Not dswEnv = Nothing Then
            roleUser = GetRoleUsers(accountName, type, dswEnv).FirstOrDefault()
        Else
            roleUser = GetRoleUsers(accountName, type).First()
        End If
        If roleUser IsNot Nothing Then
            roleUser.IsMainRole = isMainRole
            FacadeFactory.Instance.RoleUserFacade.Update(roleUser)
        End If

        btnSetMainRole.Visible = Not isMainRole
        btnRemoveMainRole.Visible = isMainRole
    End Sub
#End Region

End Class