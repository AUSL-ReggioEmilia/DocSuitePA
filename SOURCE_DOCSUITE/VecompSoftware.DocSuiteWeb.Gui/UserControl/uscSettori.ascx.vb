Imports System.Collections.Generic
Imports System.Text
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Model.Parameters
Imports System.Drawing
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports Newtonsoft.Json
Imports System.Web

Partial Public Class uscSettori
    Inherits DocSuite2008BaseControl

    Public Delegate Sub OnRoleRemovedEventHandler(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs)
    Public Delegate Sub OnRoleRemovingEventHandler(ByVal sender As Object, ByVal args As RoleEventArgs)
    Public Delegate Sub OnRolesAddedEventHandler(ByVal sender As Object, ByVal e As EventArgs)
    Public Delegate Sub OnRoleAddedEventHandler(ByVal sender As Object, ByVal e As RoleEventArgs)
    Public Delegate Sub OnRoleAddingEventHandler(ByVal sender As Object, ByVal e As RoleEventArgs)

    Public Delegate Sub RoleSelectedHandler(ByVal sender As Object, ByVal e As RoleEventArgs)

    Public Delegate Sub RoleUserViewModeChanged(ByVal sender As Object, ByVal e As EventArgs)

    ''' <summary> Scatenato prima dell'aggiunta di un nuovo settore. </summary>
    Public Event RoleAdding As OnRoleAddingEventHandler
    ''' <summary> Scatenato quando vengono aggiunti uno o più nodi alla treeview. </summary>
    Public Event RolesAdded As OnRolesAddedEventHandler
    ''' <summary> Scatenato all'aggiunta di un nuovo settore. </summary>
    Public Event RoleAdded As OnRoleAddedEventHandler
    ''' <summary> Scatenato quando viene rimosso un nodo dalla treeview. </summary>
    Public Event RoleRemoved As OnRoleRemovedEventHandler

    Public Event RoleRemoving As OnRoleRemovingEventHandler

    ''' <summary> Scatenato quando viene selezionato un nodo dalla treeview. </summary>
    Public Event RoleSelected As RoleSelectedHandler

    Public Event OnRoleUserViewModeChanged As RoleUserViewModeChanged

    Public Event OnRoleUserViewManagersChanged As RoleUserViewModeChanged

    Private Enum ProtocolRoleUserColumns As Byte
        IdRole = 0
        GroupName = 1
        UserName = 2
        Account = 3
    End Enum

    Public Enum RoleUserViewMode As Byte
        Roles = 0
        RoleGroups = 1
        RoleUsers = 2
    End Enum

    Private Enum TypeAttributeValue
        Role
        SubRole
        User
    End Enum

    Public Enum NodeTypeAttributeValue
        Role
        SubRole
        Group
        RoleUser
        RoleGroup
        FullIncrementalPath
    End Enum

#Region " Fields "

    Private Const NodeTextFormat As String = "{0} ( {1} )"
    Private Const TypeAttribute As String = "Type"
    Private Const NodeTypeAttribute As String = "NodeType"
    Private Const UserAccountAttribute As String = "UserAccount"
    Private Const SelectedRoleAttribute As String = "SELECTED"
    Private Const PrivacyRoleAttribute As String = "Privacy"
    Private Const UserAuthorizationAttribute As String = "UserAuthorization"
    Private Const SelectedFullIncrementalPathAttribute As String = "FullIncrementalPath"
    Private Const RemovedRoleAttribute As String = "REMOVED"

    Private Const TrueAttributeValue As String = "TRUE"
    Private Const FalseAttributeValue As String = "FALSE"

    Private Const DELETE_ROLE_CALLBACK As String = "{0}_uscSettoriTS.deleteCallback('{1}','{2}')"

    Private _multiSelect As String = String.Empty
    Private _location As String = String.Empty
    Private _roleList As List(Of Role)
    Private _usersList As IDictionary(Of String, String)
    Private _currentProtocolRights As ProtocolRights
    Private _currentRoleUserViewMode As Nullable(Of Byte)
    Private _manageableRolesSessionName As String
    Private _manageableRoles As String
    Private _roleFacade As RoleFacade
    Private _roleNameFacade As RoleNameFacade
    Private _toolbarVisible As Boolean = True
#End Region

#Region " Properties "

    Public ReadOnly Property RoleFacade As RoleFacade
        Get
            If _roleFacade Is Nothing Then
                _roleFacade = New RoleFacade
            End If
            Return _roleFacade
        End Get
    End Property

    Public ReadOnly Property RoleNameFacade As RoleNameFacade
        Get
            If _roleNameFacade Is Nothing Then
                _roleNameFacade = New RoleNameFacade
            End If
            Return _roleNameFacade
        End Get
    End Property

    Private ReadOnly Property RoleUserViewModeSessionName() As String
        Get
            Return ClientID & "_CurrentRoleUserViewMode"
        End Get
    End Property

    Public Property Environment As DSWEnvironment
        Get
            If ViewState("DSWEnvironment") Is Nothing Then
                ViewState("DSWEnvironment") = DSWEnvironment.Any
            End If
            Return CType(ViewState("DSWEnvironment"), DSWEnvironment)
        End Get
        Set(value As DSWEnvironment)
            ViewState("DSWEnvironment") = value
        End Set
    End Property

    Public Property CurrentProtocol As Protocol

    Public Overloads ReadOnly Property CurrentProtocolRights As ProtocolRights
        Get
            If _currentProtocolRights Is Nothing Then
                _currentProtocolRights = New ProtocolRights(CurrentProtocol)
            End If
            Return _currentProtocolRights
        End Get
    End Property

    Public Property AjaxReturnModelEnabled As Boolean = False
    Public Property AjaxModelActionName As String = String.Empty

    Public Property ProtocolRoleUsersOnly As Boolean

    Public Property CopiaConoscenzaEnabled As Boolean

    Public Property CurrentRoleUserViewMode As Byte?
        Get
            If Not _currentRoleUserViewMode.HasValue AndAlso Session(RoleUserViewModeSessionName) IsNot Nothing Then
                _currentRoleUserViewMode = CType(Session(RoleUserViewModeSessionName), Byte?)
            End If
            Return _currentRoleUserViewMode
        End Get
        Set(ByVal value As Byte?)
            Session(RoleUserViewModeSessionName) = value
            _currentRoleUserViewMode = value
        End Set
    End Property

    ''' <summary> Nome della variabile di sessione che contiene ManageableRoles. </summary>
    ''' <value>In formato 0|1|2|...</value>
    Public ReadOnly Property GetManageableRolesSessionName As String
        Get
            If String.IsNullOrEmpty(_manageableRolesSessionName) Then
                _manageableRolesSessionName = ClientID & "_ManageableRoles"
            End If
            Return _manageableRolesSessionName
        End Get
    End Property

    ''' <summary> Elenco dei settori di cui sono esplicitamente manager </summary>
    Public Property ManageableRoles As String
        Get
            If String.IsNullOrEmpty(_manageableRoles) AndAlso Session(GetManageableRolesSessionName) IsNot Nothing Then
                _manageableRoles = CType(Session(GetManageableRolesSessionName), String)
            End If
            Return _manageableRoles
        End Get
        Set(ByVal value As String)
            If String.IsNullOrEmpty(CType(Session(GetManageableRolesSessionName), String)) Then
                Session(GetManageableRolesSessionName) = value
                _manageableRoles = value
            End If
        End Set
    End Property

    ''' <summary> True permette l'inserimento di più di un ruolo, False un solo ruolo. </summary>
    Public Property MultipleRoles() As Boolean
        Get
            Return CType(ViewState("MultipleRoles"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            ViewState("MultipleRoles") = value
        End Set
    End Property

    ''' <summary> Restituisce in numero di settori selezionati </summary>
    Public ReadOnly Property Count() As Integer
        Get
            Return GetRoles().Count
        End Get
    End Property

    ''' <summary>
    ''' Imposta o restituisce la sicurezza sugli utenti: se attivo verranno considerati i gruppi di appartenenza dell'utente
    ''' </summary>
    ''' <value>True considerati i gruppi, False altrimenti</value>
    ''' <returns>True considerati i gruppi, False altrimenti</returns>
    Public Property RoleRestictions() As RoleRestrictions
        Get
            If ViewState("RoleRestictions") Is Nothing Then
                ViewState("RoleRestictions") = RoleRestrictions.None
            End If
            Return CType(ViewState("RoleRestictions"), RoleRestrictions)
        End Get
        Set(ByVal value As RoleRestrictions)
            ViewState("RoleRestictions") = value
        End Set
    End Property

    ''' <summary> Imposta o restituisce se è attiva la selezione multipla dei ruoli </summary>
    Public Property MultiSelect() As Boolean
        Get
            Return _multiSelect.Eq(TrueAttributeValue)
        End Get
        Set(ByVal value As Boolean)
            _multiSelect = If(value = True, TrueAttributeValue, FalseAttributeValue)
        End Set
    End Property

    ''' <summary> Imposta o restituisce il testo di intestazione del controllo </summary>
    Public Property Caption() As String
        Get
            Return lblCaption.Text
        End Get
        Set(ByVal value As String)
            lblCaption.Text = value
        End Set
    End Property

    ''' <summary> Lista di ruoli con la quale si vuole inizializzare il controllo </summary>
    ''' <value>IList(Of Role) che verranno visualizzati nell'albero</value>
    Public Property SourceRoles() As List(Of Role)
        Get
            If _roleList Is Nothing Then
                _roleList = New List(Of Role)
            End If
            Return _roleList
        End Get
        Set(ByVal value As List(Of Role))
            _roleList = value
        End Set
    End Property

    ''' <summary> Lista di utenti con la quale si vuole inizializzare il controllo </summary>
    ''' <value>IDictionary(Of String, String) che verranno visualizzati nell'albero</value>
    Public Property SourceUsers As IDictionary(Of String, String)
        Get
            If _usersList Is Nothing Then
                _usersList = New Dictionary(Of String, String)
            End If
            Return _usersList
        End Get
        Set(ByVal value As IDictionary(Of String, String))
            _usersList = value
        End Set
    End Property

    ''' <summary> Imposta o restituisce se il validatore di campo obbligatorio è abilitato o meno. </summary>
    Public Property Required() As Boolean
        Get
            Return AnyNodeCheck.Enabled
        End Get
        Set(ByVal value As Boolean)
            AnyNodeCheck.Enabled = value
        End Set
    End Property

    ''' <summary> Messaggio visualizzato quando scatta il validatore di campo obbligatorio. </summary>
    Public Property RequiredMessage() As String
        Get
            Return AnyNodeCheck.ErrorMessage
        End Get
        Set(ByVal value As String)
            AnyNodeCheck.ErrorMessage = value
        End Set
    End Property

    ''' <summary>
    ''' Imposta o restituisce i diritti che i ruoli visualizzati nella PopUp devono possedere (Stringa)
    ''' </summary>
    ''' <value>Stringa che identifica la posizione dei diritti</value>
    Public Property Rights As String

    ''' <summary> Indica se visualizzare anche i non attivi. </summary>
    Public Property ShowActive As Boolean
        Get
            If ViewState("ShowActive") Is Nothing Then
                ViewState("ShowActive") = True
            End If
            Return CType(ViewState("ShowActive"), Boolean)
        End Get
        Set(value As Boolean)
            ViewState("ShowActive") = value
        End Set
    End Property

    ''' <summary> Indica la visibilità della toolbar di edit </summary>
    ''' <value> Per edit dei check guardare <see cref="Checkable"/> e <see cref="EditableCheck"/> </value>
    Public Property [ReadOnly]() As Boolean
        Get
            Return CType(ViewState("ReadOnly"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            ViewState("ReadOnly") = value
        End Set
    End Property

    Public Property HideDeleteButton As Boolean
        Get
            Return CType(ViewState("HideDeleteButton"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            ViewState("HideDeleteButton") = value
        End Set
    End Property

    Public Property HideAddButton As Boolean
        Get
            Return CType(ViewState("HideAddButton"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            ViewState("HideAddButton") = value
        End Set
    End Property

    ''' <summary> Visualizza l'intestazione del controllo. </summary>
    ''' <value>True intestazione visibile, False altrimenti</value>
    Public Property HeaderVisible() As Boolean
        Get
            Return tblHeader.Visible
        End Get
        Set(ByVal value As Boolean)
            tblHeader.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Imposta o restituisce se il controllo visualizza anche gli utenti per ogni gruppo appartenente ai ruoli selezionati
    ''' </summary>
    ''' <value>True utenti visualizzati, False altrimenti</value>
    Public ReadOnly Property UserShown() As Boolean
        Get
            If ViewState("ShowUser") IsNot Nothing Then
                Return CType(ViewState("ShowUser"), Boolean)
            Else
                Return False
            End If
        End Get
    End Property

    Public Property LoadUsers() As Boolean

    ''' <summary> Restituisce il settore selezionato nella treeview. </summary>
    Public ReadOnly Property SelectedRole() As Role
        Get
            Dim node As RadTreeNode = RadTreeSettori.SelectedNode
            Dim tenantNode As RadTreeNode = RadTreeRoleTenant.SelectedNode
            If (node Is Nothing OrElse String.IsNullOrEmpty(node.Value)) AndAlso (tenantNode Is Nothing OrElse String.IsNullOrEmpty(tenantNode.Value)) Then
                Return Nothing
            End If

            If node IsNot Nothing AndAlso Not String.IsNullOrEmpty(node.Value) AndAlso node.Attributes(UserAuthorizationAttribute) Is Nothing AndAlso IsNumeric(node.Value) Then
                Return Facade.RoleFacade.GetByIdAndTenant(Integer.Parse(node.Value), DocSuiteContext.Current.CurrentTenant.TenantId)
            ElseIf tenantNode IsNot Nothing AndAlso Not String.IsNullOrEmpty(tenantNode.Value) AndAlso node.Attributes(UserAuthorizationAttribute) Is Nothing AndAlso IsNumeric(tenantNode.Value) Then
                Return Facade.RoleFacade.GetByIdAndTenant(Integer.Parse(tenantNode.Value), DocSuiteContext.Current.Tenants.FirstOrDefault(Function(x) Not x.CurrentTenant).TenantId)
            End If
            Return Nothing
        End Get
    End Property

    ''' <summary> Restituisce il controllo Treeview. </summary>
    Public ReadOnly Property TreeViewControl() As RadTreeView
        Get
            Return RadTreeSettori
        End Get
    End Property

    Public ReadOnly Property TableContentControl() As HtmlTable
        Get
            Return tblSettori
        End Get
    End Property

    ''' <summary> Lista con gli id dei ruoli da aggiungere. </summary>
    Public ReadOnly Property RoleListAdded() As ICollection(Of Integer)
        Get

            If (CType(ViewState("RolesAdded"), ICollection(Of Integer))) Is Nothing Then
                ViewState("RolesAdded") = New List(Of Integer)()
            End If
            Return CType(ViewState("RolesAdded"), ICollection(Of Integer))
        End Get
    End Property
    ''' <summary> Restituisce la lista con gli id dei ruoli da eliminare </summary>
    ''' <value>Lista contenuta nel viewstate</value>
    Public ReadOnly Property RoleListRemoved() As ICollection(Of Integer)
        Get
            If (CType(ViewState("RolesRemoved"), ICollection(Of Integer))) Is Nothing Then
                ViewState("RolesRemoved") = New List(Of Integer)()
            End If
            Return CType(ViewState("RolesRemoved"), ICollection(Of Integer))
        End Get
    End Property

    ''' <summary> Imposta se i nodi devono avere un Check di selezione </summary>
    ''' <value>True: i nodi possiedono un check di selezione</value>
    Public Property Checkable() As Boolean
        Get
            Return CType(ViewState("Checkable"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            ViewState("Checkable") = value
        End Set
    End Property

    ''' <summary> Indica se i check (se visibili) sono editabili </summary>
    ''' <value> Default true </value>
    Public Property EditableCheck() As Boolean
        Get
            If ViewState("EditableCheck") Is Nothing Then
                ViewState("EditableCheck") = True
            End If
            Return CType(ViewState("EditableCheck"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            ViewState("EditableCheck") = value
            ' Accrocchio
            For Each node As RadTreeNode In RadTreeSettori.GetAllNodes()
                If node.Checkable Then
                    node.Enabled = value
                End If
            Next

        End Set
    End Property

    Public Property SelectedRoles() As String
        Get
            Return CType(ViewState("SelectedRoles"), String)
        End Get
        Set(ByVal value As String)
            ViewState.Add("SelectedRoles", value)
        End Set
    End Property

    Public Property CausesValidation As Boolean
        Get
            Return RadTreeSettori.CausesValidation
        End Get
        Set(value As Boolean)
            RadTreeSettori.CausesValidation = value
        End Set
    End Property

    Public Property TenantEnabled As Boolean

    Public Property SearchByUserEnabled As Boolean

    Public Property SelectedRoleUserAccount As String

    Public Property ToolBarRenderMode() As RenderMode
        Get
            Return ToolBar.RenderMode
        End Get
        Set(ByVal value As RenderMode)
            ToolBar.RenderMode = value
        End Set
    End Property

    Public Property ToolBarVisible() As Boolean
        Get
            Return _toolbarVisible
        End Get
        Set(ByVal value As Boolean)
            _toolbarVisible = value
        End Set
    End Property

    ''' <summary> Indica se forzare la ricerca nell'environment DSWEnvironment.Any Da settare solo se non si passa la rightPosition </summary>
    Public Property AnyRightInAnyEnvironment As Boolean
        Get
            If ViewState("AnyRightInAnyEnvironment") Is Nothing Then
                ViewState("AnyRightInAnyEnvironment") = False
            End If
            Return CType(ViewState("AnyRightInAnyEnvironment"), Boolean)
        End Get
        Set(value As Boolean)
            ViewState("AnyRightInAnyEnvironment") = value
        End Set
    End Property
    Protected Shared ReadOnly Property CurrentTenantId As Guid
        Get
            Return DocSuiteContext.Current.CurrentTenant.TenantId
        End Get
    End Property

    Public Property FascicleVisibilityType As VisibilityType
        Get
            If (DirectCast(ToolBar.FindButtonByCommandName("checkFascicleVisibilityType"), RadToolBarButton).Checked) Then
                Return VisibilityType.Accessible
            Else
                Return VisibilityType.Confidential
            End If
        End Get
        Set(value As VisibilityType)
            DirectCast(ToolBar.FindButtonByCommandName("checkFascicleVisibilityType"), RadToolBarButton).Checked = (value = VisibilityType.Accessible)
        End Set
    End Property

    Public Property FascicleVisibilityTypeEnabled As Boolean

    Public ReadOnly Property HasSelectedRole() As Boolean
        Get
            Return RadTreeSettori.Nodes.Count > 0
        End Get
    End Property

    Public Property IdCategorySelected() As Integer?

    Public Property PropagateAuthorizationsEnabled As Boolean

    Public Property PrivacyAuthorizationButtonVisible As Boolean

    Public Property UserAuthorizationEnabled As Boolean

    Public Property ViewDistributableManager As Boolean

    Public Property GetAllRolesEnabled As Boolean


    Public Property HidePanelByControlId As String

    Public Property SelectedTenantId As Guid
#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        ToolBar.OnClientButtonClicking = String.Format("{0}_ToolBarButtonClicking", ClientID)
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub ToolBar_ButtonClick(sender As Object, e As RadToolBarEventArgs) Handles ToolBar.ButtonClick
        Dim sourceControl As RadToolBarButton = DirectCast(e.Item, RadToolBarButton)
        Select Case sourceControl.CommandName
            Case "add"
                OpenAddSettori()
            Case "delete"
                If RadTreeSettori.SelectedNode Is Nothing Then
                    BasePage.AjaxAlert("Attenzione: selezionare almeno un'autorizzazione.")
                    AjaxManager.ResponseScripts.Add(Me.ClientID + "_ClearSessionStorage()")
                    Exit Sub
                End If
                Dim role As Role = SelectedRole
                If (role IsNot Nothing) Then
                    DeleteSettore(role)
                    Exit Sub
                End If
                If DocSuiteContext.Current.SimplifiedPrivacyEnabled AndAlso RadTreeSettori.SelectedNode.Attributes(UserAuthorizationAttribute) IsNot Nothing AndAlso
                    RadTreeSettori.SelectedNode.Attributes(UserAuthorizationAttribute) = TrueAttributeValue Then
                    RadTreeSettori.SelectedNode.Remove()
                End If
            Case "viewMode"
                RaiseEvent OnRoleUserViewModeChanged(Me, New EventArgs)
            Case "viewManagers"
                RaiseEvent OnRoleUserViewManagersChanged(Me, New EventArgs)
            Case "checkPrivacy"
                If DocSuiteContext.Current.SimplifiedPrivacyEnabled AndAlso SelectedRole IsNot Nothing Then
                    Dim role As Role = SelectedRole
                    Dim treeView As RadTreeView = RadTreeSettori
                    If DocSuiteContext.Current.ProtocolEnv.MultiDomainEnabled AndAlso Not role.TenantId.Equals(Guid.Empty) AndAlso Not role.TenantId.Equals(DocSuiteContext.Current.CurrentTenant.TenantId) Then
                        treeView = RadTreeRoleTenant
                    End If
                    Dim selectedNode As RadTreeNode = treeView.FindNodeByValue(role.IdRoleTenant.ToString())
                    Dim privacyRoleAttributeValue As String = TrueAttributeValue

                    If selectedNode.Attributes.Item(PrivacyRoleAttribute) IsNot Nothing Then
                        privacyRoleAttributeValue = selectedNode.Attributes.Item(PrivacyRoleAttribute)
                        selectedNode.Attributes.Item(PrivacyRoleAttribute) = If(privacyRoleAttributeValue.Eq(FalseAttributeValue), TrueAttributeValue, FalseAttributeValue)

                        If role.Father Is Nothing Then
                            selectedNode.ImageUrl = ImagePath.SmallRole
                        Else
                            selectedNode.ImageUrl = ImagePath.SmallSubRole
                        End If
                        selectedNode.ToolTip = ""
                        If privacyRoleAttributeValue.Eq(FalseAttributeValue) Then
                            selectedNode.ImageUrl = "../App_Themes/DocSuite2008/imgset16/lock.png"
                            selectedNode.ToolTip = "Autorizzazione riservata/privacy"
                        End If
                    Else
                        selectedNode.AddAttribute(PrivacyRoleAttribute, TrueAttributeValue)
                        selectedNode.ImageUrl = "../App_Themes/DocSuite2008/imgset16/lock.png"
                        selectedNode.ToolTip = "Autorizzazione riservata/privacy"
                    End If

                End If
            Case "user"
                Dim parameters As String = String.Concat("Type=", BasePage.Type, "&ParentID=", ClientID)
                AjaxManager.ResponseScripts.Add(String.Format("return {0}_OpenUserWindow({1},{2},'{3}');", ClientID, ProtocolEnv.DocumentPreviewWidth, ProtocolEnv.DocumentPreviewHeight, parameters))
        End Select
    End Sub

    Private Sub uscSettori_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)

        Dim ajaxModel As AjaxModel = If(CanDeserialize(Of AjaxModel)(e.Argument), JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument), Nothing)

        If ajaxModel IsNot Nothing AndAlso ajaxModel.ActionName.Eq("GetByTenantId") Then
            MultiTenantDataBind(Guid.Parse(ajaxModel.Value(0)))
        End If

        Dim arguments As String() = e.Argument.Split("|"c)
        If Not arguments(0).Eq(ClientID) OrElse String.IsNullOrEmpty(arguments(1)) Then
            Exit Sub
        End If

        'Autorizzazione utente
        If DocSuiteContext.Current.SimplifiedPrivacyEnabled AndAlso arguments.Length > 2 AndAlso arguments(1).Eq("User") Then
            Dim localArg As String = HttpUtility.HtmlDecode(arguments(2))
            Dim contact As Contact = JsonConvert.DeserializeObject(Of Contact)(localArg)
            AddUserAuthorization(contact.FullDescription(False), contact.Code, localArg, False, Nothing, ProtocolUserType.Authorization, String.Empty)
            Exit Sub
        End If

        Dim tenantId As Guid = DocSuiteContext.Current.CurrentTenant.TenantId
        Dim account As String = String.Empty
        Dim currentTreeView As RadTreeView = RadTreeSettori

        If arguments.Count > 2 Then
            If (arguments.Any(Function(f) f.StartsWith("tenantId="))) Then
                tenantId = Guid.Parse(arguments.Single(Function(f) f.StartsWith("tenantId=")).Split("="c)(1))

                If Not tenantId = DocSuiteContext.Current.CurrentTenant.TenantId Then
                    currentTreeView = RadTreeRoleTenant
                End If
            End If

            If (arguments.Any(Function(f) f.StartsWith("userName="))) Then
                account = arguments.Single(Function(f) f.StartsWith("userName=")).Split("="c)(1)
            End If

        End If

        AddSettori(arguments(1), tenantId, currentTreeView, account)
        If RadTreeSettori.Nodes.Count > 0 Then
            fldCurrentTenant.SetDisplay(True)
        End If
    End Sub

    Protected Sub Page_Unload(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Unload
        ViewState.Remove("RolesAdded")
        ViewState.Remove("RolesRemoved")
    End Sub

    Protected Sub RadTreeSettori_NodeClick(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs) Handles RadTreeSettori.NodeClick
        If RoleSelectedEvent Is Nothing AndAlso RadTreeSettori.SelectedNode Is Nothing AndAlso RadTreeRoleTenant.SelectedNode Is Nothing Then
            Exit Sub
        End If

        Dim otherTenantNode As RadTreeNode = RadTreeRoleTenant.SelectedNode
        If RadTreeRoleTenant.SelectedNode IsNot Nothing Then
            otherTenantNode.Selected = False
        End If
        Dim node As RadTreeNode = e.Node
        Dim nodeValue As Integer = 0
        If node IsNot Nothing Then
            If DocSuiteContext.Current.SimplifiedPrivacyEnabled Then
                Dim privacy As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("checkPrivacy"), RadToolBarButton)
                If privacy.Visible Then
                    privacy.Enabled = RadTreeSettori.Nodes.Count > 0 AndAlso (node.Attributes(UserAuthorizationAttribute) Is Nothing OrElse Not node.Attributes(UserAuthorizationAttribute).Equals(TrueAttributeValue))
                End If
            End If

            If Integer.TryParse(node.Value, nodeValue) Then
                Dim role As Role = Facade.RoleFacade.GetByIdAndTenant(nodeValue, DocSuiteContext.Current.CurrentTenant.TenantId)

                If ProtocolEnv.RefusedProtocolAuthorizationEnabled AndAlso CurrentProtocol IsNot Nothing AndAlso CurrentProtocol.Roles.Any(Function(r) r.Role.Id.Equals(role.Id)) Then
                    Dim protRole As ProtocolRole = CurrentProtocol.Roles.Single(Function(r) r.Role.Id.Equals(role.Id))
                    If protRole.Status = ProtocolRoleStatus.Accepted Then
                        node.CssClass = "selected-node-accepted-role"
                    End If
                End If
                RaiseEvent RoleSelected(sender, New RoleEventArgs(role))
            End If
        End If
    End Sub

    Protected Sub RadTreeRoleTenant_NodeClick(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs) Handles RadTreeRoleTenant.NodeClick
        If DocSuiteContext.Current.SimplifiedPrivacyEnabled Then
            Dim privacy As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("checkPrivacy"), RadToolBarButton)
            If privacy.Visible Then
                privacy.Enabled = RadTreeSettori.Nodes.Count > 0 OrElse RadTreeRoleTenant.Nodes.Count > 0
            End If
        End If
        If RoleSelectedEvent Is Nothing AndAlso RadTreeSettori.SelectedNode Is Nothing Then
            Exit Sub
        End If

        Dim currentTenantNode As RadTreeNode = RadTreeSettori.SelectedNode
        If RadTreeSettori.SelectedNode IsNot Nothing Then
            currentTenantNode.Selected = False
        End If
        Dim node As RadTreeNode = e.Node
        Dim nodeValue As Integer = 0
        If node IsNot Nothing AndAlso Integer.TryParse(node.Value, nodeValue) Then
            Dim role As Role = Facade.RoleFacade.GetByIdAndTenant(nodeValue, DocSuiteContext.Current.Tenants.SingleOrDefault(Function(x) Not x.CurrentTenant).TenantId)
            RaiseEvent RoleSelected(sender, New RoleEventArgs(role))
        End If
    End Sub

    Protected Sub RadTreeRoleTenant_NodeEvents(sender As Object, e As EventArgs) Handles RadTreeRoleTenant.NodeDataBound, RadTreeRoleTenant.NodeCreated
        If DocSuiteContext.Current.ProtocolEnv.MultiDomainEnabled AndAlso DocSuiteContext.Current.ProtocolEnv.TenantAuthorizationEnabled AndAlso RadTreeRoleTenant.Nodes.Count > 0 Then
            fldOtherTenant.SetDisplay(True)
        End If
    End Sub

    Protected Sub RadTreeSettori_NodeEvents(sender As Object, e As EventArgs) Handles RadTreeSettori.NodeDataBound, RadTreeSettori.NodeCreated
        If RadTreeSettori.Nodes.Count > 0 Then
            fldCurrentTenant.SetDisplay(True)
        End If
    End Sub

#End Region

#Region " Methods "

    Public Sub Initialize(Optional needClearSessionStorage As Boolean = True)
        RadTreeSettori.CheckBoxes = Checkable
        lblCurrentTenant.Text = DocSuiteContext.Current.CurrentTenant.TenantName
        If ProtocolEnv.TenantAuthorizationEnabled Then
            Dim otherTenant As TenantModel = DocSuiteContext.Current.Tenants.FirstOrDefault(Function(x) Not x.CurrentTenant)
            If otherTenant IsNot Nothing Then
                lblOtherTenant.Text = otherTenant.TenantName
            End If
        End If
        InitializeControls()

        If Visible AndAlso UseSessionStorage AndAlso needClearSessionStorage Then
            AjaxManager.ResponseScripts.Add(Me.ClientID + "_ClearSessionStorage()")
        End If
    End Sub

    Public Sub SetButtonsVisibility(value As Boolean)
        Dim delete As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("delete"), RadToolBarButton)
        delete.Visible = value
    End Sub

    Private Sub InitializeAjax()
        If Visible Then
            AddHandler AjaxManager.AjaxRequest, AddressOf uscSettori_AjaxRequest
            AjaxManager.AjaxSettings.AddAjaxSetting(RadTreeSettori, ToolBar)
            AjaxManager.AjaxSettings.AddAjaxSetting(ToolBar, RadTreeSettori)
            AjaxManager.AjaxSettings.AddAjaxSetting(ToolBar, RadTreeRoleTenant)
            'AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, RadTreeSettori)
            'AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, RadTreeRoleTenant)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, fldCurrentTenant)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, fldOtherTenant)
            AjaxManager.AjaxSettings.AddAjaxSetting(ToolBar, fldOtherTenant, BasePage.MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(ToolBar, fldCurrentTenant, BasePage.MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(RadTreeSettori, RadTreeRoleTenant)
            AjaxManager.AjaxSettings.AddAjaxSetting(RadTreeRoleTenant, RadTreeSettori)
            'AjaxManager.AjaxSettings.AddAjaxSetting(RadTreeSettori, RadTreeSettori)
        End If
    End Sub

    Private Sub InitializeControls()

        ToolBar.Visible = (Not [ReadOnly] OrElse CopiaConoscenzaEnabled OrElse CurrentRoleUserViewMode.HasValue OrElse FascicleVisibilityTypeEnabled OrElse PropagateAuthorizationsEnabled) AndAlso ToolBarVisible

        Dim add As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("add"), RadToolBarButton)
        add.Visible = Not [ReadOnly] AndAlso Not HideAddButton

        Dim delete As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("delete"), RadToolBarButton)
        delete.Visible = Not [ReadOnly] AndAlso Not HideDeleteButton

        Dim copiaConoscenza As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("copiaConoscenza"), RadToolBarButton)
        copiaConoscenza.Visible = CopiaConoscenzaEnabled

        Dim viewMode As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("viewMode"), RadToolBarButton)
        If CurrentRoleUserViewMode.HasValue Then
            viewMode.Visible = True
            If CurrentRoleUserViewMode.Value = RoleUserViewMode.Roles Then
                viewMode.Text = "Passa a Visualizzazione Completa"
            Else
                viewMode.Text = "Passa a Visualizzazione Settori"
            End If
        Else
            viewMode.Visible = False
        End If

        Dim viewManagers As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("viewManagers"), RadToolBarButton)
        viewManagers.Visible = CurrentRoleUserViewMode.HasValue AndAlso ViewDistributableManager

        Dim checkFascicleVisibilityType As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("checkFascicleVisibilityType"), RadToolBarButton)
        checkFascicleVisibilityType.Visible = FascicleVisibilityTypeEnabled

        Dim checkPropagateAuthorizations As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("checkPropagateAuthorizations"), RadToolBarButton)
        checkPropagateAuthorizations.Visible = PropagateAuthorizationsEnabled

        InitializePrivacyAuthorization()
        InitializeUserAuthorization()
    End Sub

    Public Sub InitializeUserAuthorization()
        btnDelete.ImageUrl = "../App_Themes/DocSuite2008/imgset16/brick_delete.png"
        btnDelete.ToolTip = "Elimina settore"
        btnADUser.Visible = False
        If DocSuiteContext.Current.SimplifiedPrivacyEnabled AndAlso UserAuthorizationEnabled Then
            btnADUser.Visible = Not [ReadOnly]
            btnDelete.ImageUrl = "../App_Themes/DocSuite2008/imgset16/delete.png"
            btnDelete.ToolTip = "Elimina autorizzazione"
        End If
    End Sub

    Public Sub InitializePrivacyAuthorization()
        Dim privacy As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("checkPrivacy"), RadToolBarButton)
        privacy.Visible = DocSuiteContext.Current.SimplifiedPrivacyEnabled AndAlso PrivacyAuthorizationButtonVisible AndAlso Not [ReadOnly]
        If privacy.Visible Then
            privacy.Text = CommonBasePage.PRIVACY_LABEL
            privacy.ToolTip = CommonBasePage.PRIVACY_LABEL
            privacy.Enabled = RadTreeSettori.Nodes.Count > 0 OrElse RadTreeRoleTenant.Nodes.Count > 0
        End If
    End Sub

    Protected Sub OpenAddSettori()
        Dim selRoles As New StringBuilder()
        selRoles.Append(
            String.Join("|", RadTreeSettori.GetAllNodes().Where(Function(x) x.Attributes(SelectedRoleAttribute).Eq(TrueAttributeValue)).Select(Function(x) x.Value)).ToArray()
            )

        If Not String.IsNullOrEmpty(SelectedRoles) Then
            selRoles.AppendFormat("|{0}", SelectedRoles)
        End If

        Dim parameters As String = GetWindowParameters()
        If selRoles.Length <> 0 Then
            parameters = String.Format("{0}&Selected={1}&TenantSelected={2}", parameters, selRoles.ToString(), String.Empty)
        End If

        AjaxManager.ResponseScripts.Add(String.Format("return {0}_OpenWindow({1},{2},'{3}');", ClientID, ProtocolEnv.DocumentPreviewWidth, ProtocolEnv.DocumentPreviewHeight, parameters))
    End Sub

    Protected Sub AddSettori(ByVal settoriList As String, tenantId As Guid, ByRef treeView As RadTreeView, account As String)
        If Not String.IsNullOrEmpty(settoriList) Then
            Dim v As String() = settoriList.Split(","c)

            Dim idRoleList As New List(Of Integer)()
            For Each idRole As String In v
                idRoleList.Add(Integer.Parse(idRole))
            Next

            Dim roles As IList(Of Role) = Facade.RoleFacade.GetByIds(idRoleList, tenantId).OrderBy(Function(x) x.Name).ToList()
            If MultipleRoles = False Then
                For Each node As RadTreeNode In treeView.GetAllNodes()
                    Dim idChild As Integer = CType(node.Value, Integer)
                    SetRolesRemovedList(Facade.RoleFacade.GetByIdAndTenant(idChild, tenantId), treeView)
                Next
                treeView.Nodes.Clear()
            End If
            If roles.Count > 0 Then
                AddRoles(roles, True, False, False)
            End If
        End If
        SelectedRoleUserAccount = account
        RaiseEvent RolesAdded(ToolBar, New RadTreeNodeEventArgs(New RadTreeNode()))
    End Sub

    'Delete Settore: If selected role don't have children remove node
    '                If selected role have children and font.bold=true then set font.bold=false
    '                If selected role have any children with font.bold=true, don't remove node

    Protected Sub DeleteSettore(role As Role)
        Dim treeView As RadTreeView = RadTreeSettori
        If DocSuiteContext.Current.ProtocolEnv.MultiDomainEnabled AndAlso Not role.TenantId.Equals(Guid.Empty) AndAlso Not role.TenantId.Equals(DocSuiteContext.Current.CurrentTenant.TenantId) Then
            treeView = RadTreeRoleTenant
        End If

        Dim args As New RoleEventArgs(role)
        args.IsPersistent = Not RoleListAdded.Contains(role.IdRoleTenant)

        RaiseEvent RoleRemoving(ToolBar, args)

        ' Azione annullata dall'esterno
        If args.Cancel Then
            Exit Sub
        End If
        Dim selectedNode As RadTreeNode = treeView.FindNodeByValue(role.IdRoleTenant.ToString())
        If Not String.IsNullOrEmpty(ManageableRoles) Then
            ' Verifico che il settore corrente sia figlio di uno di quelli in cui l'utente corrente è manager.
            Dim manageables As String() = ManageableRoles.Split("|"c)

            Dim isDeletable As Boolean = False
            For Each mr As String In manageables
                ' ATTENZIONE! Rendo non deautorizzabili i settori di cui l'utente corrente è esplicitamente manager.
                If role.FullIncrementalPath.Contains(mr) AndAlso Not role.IdRoleTenant.ToString.Equals(mr) Then
                    isDeletable = True ' Se si ne permetto la cancellazione.
                    Exit For
                End If
            Next

            If Not isDeletable Then
                ' Altrimenti avviso che l'utente corrente non ne ha i permessi.
                BasePage.AjaxAlert("Settore [{0}] non modificabile dall'utente corrente.", role.Name)

                RaiseEvent RoleRemoved(ToolBar, New RadTreeNodeEventArgs(selectedNode)) ' Sollevo comunque l'evento per reimpostare le checkbox nell'albero.
                If UseSessionStorage Then
                    AjaxManager.ResponseScripts.Add(String.Format(DELETE_ROLE_CALLBACK, Me.ClientID, role.Id.ToString(), role.TenantId.ToString()))
                End If
                Exit Sub
            End If
        End If

        If selectedNode IsNot Nothing AndAlso selectedNode.Parent IsNot Nothing Then
            RemoveNodeRecursive(selectedNode, treeView, role.TenantId.ToString())
            If RadTreeSettori.Nodes.Count = 0 Then
                fldCurrentTenant.SetDisplay(False)
            End If

            If RadTreeRoleTenant.Nodes.Count = 0 Then
                fldOtherTenant.SetDisplay(False)
            End If
            RaiseEvent RoleRemoved(ToolBar, New RadTreeNodeEventArgs(selectedNode))
            If UseSessionStorage Then
                AjaxManager.ResponseScripts.Add(String.Format(DELETE_ROLE_CALLBACK, Me.ClientID, role.Id.ToString(), role.TenantId.ToString()))
            End If
        End If
    End Sub

    Private Sub RemoveNodeRecursive(ByVal delNode As RadTreeNode, ByRef treeView As RadTreeView, Optional tenantId As String = Nothing)
        If String.IsNullOrEmpty(tenantId) Then
            tenantId = DocSuiteContext.Current.CurrentTenant.TenantId.ToString()
        End If
        Dim role As Role = Facade.RoleFacade.GetByIdAndTenant(Integer.Parse(delNode.Value), Guid.Parse(tenantId))
        Dim node As RadTreeNode = SeekAndImplementNode(Nothing, role, treeView, tenantId)
        node.Attributes.Add(RemovedRoleAttribute, FalseAttributeValue)
        node.Font.Bold = False

        'se aggiunto in precedenza annulla addizione
        SetRolesRemovedList(role, treeView)

        If delNode.Parent IsNot Nothing Then
            'Se il nodo da eliminare non ha un fratello rimuovo il padre. Verifico che l'unico nodo figlio sia il delNode.
            If delNode.ParentNode IsNot Nothing AndAlso delNode.ParentNode.Nodes.Count = 1 AndAlso delNode.ParentNode.Nodes.FindNodeByValue(delNode.Value) IsNot Nothing AndAlso
                Not GetRoles().Any(Function(x) x.IdRoleTenant.ToString().Eq(delNode.ParentNode.Value) AndAlso x.TenantId.ToString().Eq(tenantId)) Then
                RemoveNodeRecursive(delNode.ParentNode, treeView, tenantId)
            End If
        End If
        delNode.Remove()
    End Sub

    Public Sub RemoveRoleNode(role As Role)

        Dim existingNode As RadTreeNode = RadTreeSettori.FindNodeByValue(role.IdRoleTenant.ToString())
        If existingNode Is Nothing Then Throw New NullReferenceException()

        RemoveNodeRecursive(existingNode, RadTreeSettori)
    End Sub

    ''' <summary> Cerca, crea ricorsivamente dal basso e aggancia il nodo del settore e il relativo padre all'albero  </summary>
    ''' <param name="node"> Eventuale nodo figlio </param>
    ''' <param name="role"> Settore inerente a questo macello </param>
    ''' <remarks> Ci facciamo un baffo del Single responsibility principle </remarks>
    Private Function SeekAndImplementNode(ByRef node As RadTreeNode, ByVal role As Role, ByRef treeView As RadTreeView, tenantId As String, Optional dateToHistoricize As DateTime? = Nothing) As RadTreeNode
        Dim existingNode As RadTreeNode = treeView.FindNodeByValue(role.IdRoleTenant.ToString())
        If existingNode IsNot Nothing Then
            Return existingNode
        End If

        Dim nodeToAdd As New RadTreeNode()
        nodeToAdd.Checkable = False
        If role IsNot Nothing Then

            nodeToAdd.Text = role.Name
            ' Se è attiva la storicizzazione e la data è stata impostata, vado a recuperare il nome "storico".
            If ProtocolEnv.RoleContactHistoricizing AndAlso dateToHistoricize.HasValue Then
                Dim RoleAggiornato As RoleName = RoleNameFacade.GetRoleNamesByValidDate(role.IdRoleTenant, dateToHistoricize.Value, tenantId)
                If RoleAggiornato IsNot Nothing Then
                    nodeToAdd.Text = RoleAggiornato.Name
                End If
            End If

            Dim currentProtocolRole As ProtocolRole = GetProtocolRoleByRoleId(role.IdRoleTenant)
            If currentProtocolRole IsNot Nothing AndAlso currentProtocolRole.Type IsNot Nothing AndAlso currentProtocolRole.Type.Eq(ProtocolRoleTypes.CarbonCopy) Then
                nodeToAdd.Text = String.Concat(nodeToAdd.Text, " (CC)")
            End If

            nodeToAdd.Value = role.IdRoleTenant.ToString()

            If role.IsActive <> 1 OrElse Not role.IsActiveRange() Then
                nodeToAdd.CssClass = "notActive"
                nodeToAdd.ToolTip = "il settore non è più attivo"
            End If

            If role.Father Is Nothing Then 'Primo Livello
                nodeToAdd.ImageUrl = ImagePath.SmallRole
                nodeToAdd.Attributes.Add(TypeAttribute, TypeAttributeValue.Role.ToString())
                AddUsersOfGroupsNode(nodeToAdd, role)
                treeView.Nodes.Add(nodeToAdd)
            Else
                nodeToAdd.ImageUrl = ImagePath.SmallSubRole
                nodeToAdd.Attributes.Add(TypeAttribute, TypeAttributeValue.SubRole.ToString())
                Dim newNode As RadTreeNode = treeView.FindNodeByValue(role.Father.IdRoleTenant.ToString())
                If (newNode Is Nothing) Then
                    SeekAndImplementNode(nodeToAdd, role.Father, treeView, tenantId, dateToHistoricize)
                Else
                    AddUsersOfGroupsNode(nodeToAdd, role)
                    newNode.Nodes.Add(nodeToAdd)
                End If

            End If
            nodeToAdd.Expanded = True
        End If
        If node IsNot Nothing Then
            nodeToAdd.Nodes.Add(node)
        End If

        If RoleListRemoved.Contains(role.IdRoleTenant) Then
            RoleListRemoved.Remove(role.IdRoleTenant)
        End If
        Return nodeToAdd
    End Function

    Public Sub AddRoles(ByVal roles As IList(Of Role), ByVal persistState As Boolean, ByVal recursive As Boolean, ByVal onlyWithPecAddress As Boolean, Optional multitenantEnabled As Boolean = False, Optional tenantId As Guid? = Nothing)
        Dim treeView As RadTreeView = RadTreeSettori

        For Each role As Role In roles
            If DocSuiteContext.Current.ProtocolEnv.MultiDomainEnabled AndAlso Not role.TenantId.Equals(Guid.Empty) AndAlso Not role.TenantId.Equals(DocSuiteContext.Current.CurrentTenant.TenantId) Then
                treeView = RadTreeRoleTenant
            End If
            AddRole(role, persistState, recursive, onlyWithPecAddress, True, treeView, multitenantEnabled:=multitenantEnabled, selectedTenantId:=tenantId)
        Next
    End Sub

    Public Sub AddRole(ByVal role As Role, ByVal persistState As Boolean, ByVal recursive As Boolean, ByVal onlyWithPecAddress As Boolean, ByVal checked As Boolean)
        Dim treeView As RadTreeView = RadTreeSettori
        If DocSuiteContext.Current.ProtocolEnv.MultiDomainEnabled AndAlso Not role.TenantId.Equals(Guid.Empty) AndAlso Not role.TenantId.Equals(DocSuiteContext.Current.CurrentTenant.TenantId) Then
            treeView = RadTreeRoleTenant
        End If
        AddRole(role, persistState, recursive, onlyWithPecAddress, checked, treeView)
    End Sub

    Public Sub AddRole(ByVal role As Role, ByVal persistState As Boolean, ByVal recursive As Boolean, ByVal onlyWithPecAddress As Boolean, ByVal checked As Boolean, treeView As RadTreeView, Optional multitenantEnabled As Boolean = False, Optional selectedTenantId As Guid? = Nothing)
        Dim cancelableEvent As RoleEventArgs = New RoleEventArgs(role)
        RaiseEvent RoleAdding(ToolBar, cancelableEvent)
        If cancelableEvent.Cancel Then
            Return
        End If

        If recursive Then
            AddRecursiveNode(Nothing, role, onlyWithPecAddress, treeView, If(multitenantEnabled, selectedTenantId.Value, role.TenantId), multitenantEnabled:=multitenantEnabled)
        Else
            Dim node As RadTreeNode = SeekAndImplementNode(Nothing, role, treeView, role.TenantId.ToString())
            node.Attributes.Add(SelectedRoleAttribute, TrueAttributeValue)
            node.Attributes.Add(SelectedFullIncrementalPathAttribute, role.FullIncrementalPath)
            node.Font.Bold = True
            'node.Selected = True
            If persistState Then
                'se eliminato in precedenza annulla eliminazione
                SetRolesAddedList(role)
            End If
            If Checkable Then
                node.Checkable = True
                node.Checked = checked
                node.Enabled = EditableCheck
            End If
        End If

        If UseSessionStorage Then
            Dim script As String = GetTSSessionStorage(role, ClientID, AjaxModelActionName)
            AjaxManager.ResponseScripts.Add(script)
        End If
        RaiseEvent RoleAdded(ToolBar, New RoleEventArgs(role))
    End Sub

    Public Shared Function GetTSSessionStorage(role As Role, clientId As String, actionName As String) As String
        Dim ajaxResult As AjaxModel = New AjaxModel()
        ajaxResult.ActionName = actionName
        ajaxResult.Value = New List(Of String)()
        ajaxResult.Value.Add(JsonConvert.SerializeObject(String.Concat(clientId, "|", role.Id, "|tenantId=", role.TenantId)))
        Dim script As String = String.Format("{0}_uscSettoriTS.setRoles({1},{2});", clientId, True.ToString().ToLower(), JsonConvert.SerializeObject(ajaxResult))
        Return script
    End Function

    Private Sub AddUsersOfGroupsNode(ByRef node As RadTreeNode, ByRef role As Role)
        If Not LoadUsers OrElse role.RoleGroups Is Nothing Then
            Exit Sub
        End If

        For Each roleGroup As RoleGroup In role.RoleGroups
            AddNodeUser(node, roleGroup, roleGroup.ProtocolRights.IsRoleEnabled)
        Next
    End Sub

    Private Sub AddNodeUser(ByRef node As RadTreeNode, ByRef group As RoleGroup, ByVal administrator As Boolean)
        For Each user As AccountModel In CommonAD.GetADUsersFromGroup(group.Name, CommonShared.UserDomain)
            Dim userNode As New RadTreeNode
            userNode.Value = group.Role.IdRoleTenant.ToString()
            userNode.Text = user.Account
            userNode.Attributes.Add(TypeAttribute, TypeAttributeValue.User.ToString())
            userNode.Attributes.Add("UserType", "UA")
            userNode.Attributes.Add("GroupId", group.Role.IdRoleTenant.ToString())
            userNode.Attributes.Add("GroupName", group.Name)
            userNode.Attributes.Add("UserName", user.Account)
            userNode.Attributes.Add(UserAccountAttribute, user.GetFullUserName())
            userNode.Attributes.Add("Administrator", administrator.ToString())
            userNode.ImageUrl = "../Comm/images/User16M.gif"
            userNode.Style.Add("color", "gray")
            userNode.Font.Bold = True
            userNode.Visible = False

            node.Nodes.Add(userNode)

        Next user
    End Sub

    Private Sub AddUserAuthorization(fullDescription As String, fullAccount As String, manualContact As String, showRegistrationDate As Boolean,
                                     auhtorizationDate As DateTimeOffset?, userType As ProtocolUserType, authorizerDescription As String)
        Dim foundNode As RadTreeNode = RadTreeSettori.Nodes.FindNodeByValue(fullAccount)
        If foundNode IsNot Nothing Then
            Exit Sub
        End If

        Dim node As RadTreeNode = New RadTreeNode()
        node.Text = fullDescription

        If showRegistrationDate AndAlso auhtorizationDate.HasValue Then
            If userType.Equals(ProtocolUserType.Highlight) Then
                node.Text = $"{node.Text} - messo in evidenza da {authorizerDescription} il {auhtorizationDate:dd/MM/yyyy}"
            Else
                node.Text = String.Concat(node.Text, " - autorizzato il ", String.Format("{0:dd/MM/yyyy}", auhtorizationDate.Value))
            End If
        End If

        node.Value = fullAccount
        node.Attributes.Add("ManualContact", manualContact)
        node.Attributes.Add(UserAuthorizationAttribute, TrueAttributeValue)
        node.Expanded = True
        node.Font.Bold = True
        node.Attributes.Add("Selected", "1")
        node.ImageUrl = Page.ResolveClientUrl("~/App_Themes/DocSuite2008/imgset16/user.png")
        TreeViewUtils.ChangeNodesForeColor(node, Color.Empty)
        RadTreeSettori.Nodes.Add(node)
    End Sub

    Sub RemoveRole(role As Role)
        If SourceRoles.Contains(role) Then
            Dim node As RadTreeNode = RadTreeSettori.FindNodeByValue(role.IdRoleTenant.ToString)
            If node IsNot Nothing Then
                RemoveNodeRecursive(node, RadTreeSettori)
            End If
            SourceRoles.Remove(role)
        End If
    End Sub

    Protected Function GetWindowParameters() As String
        Dim parameters As New StringBuilder

        If Environment <> DSWEnvironment.Any OrElse AnyRightInAnyEnvironment Then
            parameters.AppendFormat("&DSWEnvironment={0}", Environment)
        End If

        If Not String.IsNullOrEmpty(Type) Then
            parameters.AppendFormat("&Type={0}", Type)
        End If
        parameters.AppendFormat("&MultiSelect={0}", MultiSelect)
        parameters.AppendFormat("&RoleRestiction={0}", RoleRestictions)
        parameters.AppendFormat("&Rights={0}", Rights)
        parameters.AppendFormat("&isActive={0}", ShowActive)
        parameters.AppendFormat("&TenantEnabled={0}", TenantEnabled)
        parameters.AppendFormat("&SearchByUserEnabled={0}", SearchByUserEnabled)

        If IdCategorySelected IsNot Nothing Then
            parameters.AppendFormat("&idCategorySelected={0}", IdCategorySelected)
        End If

        If Not String.IsNullOrEmpty(ManageableRoles) Then
            parameters.AppendFormat("&ManageableRoles={0}", ManageableRoles)
        End If

        If GetAllRolesEnabled Then
            parameters.AppendFormat("&GetAllRolesEnabled={0}", GetAllRolesEnabled)
        End If

        parameters.AppendFormat("&HidePanelByControlId={0}", HidePanelByControlId)

        Dim returnValue As String = parameters.ToString()
        Return returnValue.Remove(returnValue.IndexOf("&"), 1)
    End Function

    Public Sub DataBindProtocolRoles(ByVal protRoleList As IList(Of ProtocolRole), ByVal showRegistrationDate As Boolean, Optional users As IList(Of ProtocolUser) = Nothing)
        Dim node As RadTreeNode
        Dim treeView As RadTreeView
        Dim roleDate As DateTimeOffset
        For Each protRole As ProtocolRole In protRoleList
            treeView = RadTreeSettori
            If DocSuiteContext.Current.ProtocolEnv.MultiDomainEnabled AndAlso Not protRole.Role.TenantId.Equals(Guid.Empty) AndAlso Not protRole.Role.TenantId.Equals(DocSuiteContext.Current.CurrentTenant.TenantId) Then
                treeView = RadTreeRoleTenant
            End If

            node = SeekAndImplementNode(Nothing, protRole.Role, treeView, protRole.Role.TenantId.ToString(), protRole.RegistrationDate.DateTime)
            node.Attributes.Add(SelectedRoleAttribute, TrueAttributeValue)
            node.Attributes.Add(SelectedFullIncrementalPathAttribute, protRole.Role.FullIncrementalPath)

            node.Font.Bold = True

            If ProtocolEnv.RefusedProtocolAuthorizationEnabled AndAlso protRole.Status = ProtocolRoleStatus.Accepted Then
                node.CssClass = "node-accepted-role"
            End If

            node.Selected = ProtocolEnv.SelectedRoleNodeEnabled
            If showRegistrationDate Then
                roleDate = protRole.RegistrationDate
                If protRole.LastChangedDate.HasValue Then
                    roleDate = protRole.LastChangedDate.Value
                End If
                node.Text = String.Concat(node.Text, " - autorizzato il ", String.Format("{0:dd/MM/yyyy}", roleDate))
            End If

            Dim roleRights As IList(Of Role) = Facade.RoleFacade.GetUserRoles(DSWEnvironment.Protocol, 1, True)

            If (Not String.IsNullOrEmpty(protRole.Note)) AndAlso
               (((Not protRole.NoteType = ProtocolRoleNoteType.Reserved) AndAlso (ProtocolEnv.ProtocolNoteReservedRoleEnabled)) OrElse ((protRole.NoteType = ProtocolRoleNoteType.Reserved) AndAlso (ProtocolEnv.ProtocolNoteReservedRoleEnabled) AndAlso (roleRights.Contains(protRole.Role))) OrElse (Not ProtocolEnv.ProtocolNoteReservedRoleEnabled)) Then
                Dim nodeOriginalText As String = node.Text
                node.Text = String.Format(NodeTextFormat, nodeOriginalText, protRole.Note)
                If protRole.Note.Length > 100 Then
                    node.ToolTip = String.Format(NodeTextFormat, nodeOriginalText, protRole.Note)
                    node.Text = String.Format(NodeTextFormat, nodeOriginalText, String.Format("{0} [...]", protRole.Note.Remove(100)))
                End If
            End If

            If DocSuiteContext.Current.SimplifiedPrivacyEnabled AndAlso protRole.Type.Eq(ProtocolRoleTypes.Privacy) Then
                node.ImageUrl = "../App_Themes/DocSuite2008/imgset16/lock.png"
                node.ToolTip = "Autorizzazione riservata/privacy"
            End If

            SourceRoles.Add(protRole.Role)
            'se eliminato in precedenza annulla eliminazione
            SetRolesAddedList(protRole.Role)
            'SetRoleAs(RoleAction.Added, protRole.Role.Id)
        Next

        If (DocSuiteContext.Current.SimplifiedPrivacyEnabled OrElse ProtocolEnv.ProtocolHighlightEnabled) AndAlso users IsNot Nothing Then
            For Each user As ProtocolUser In users.OrderByDescending(Function(f) f.RegistrationDate)
                AddUserAuthorization(GetUserDescription(user.Account), user.Account, Nothing, True, user.RegistrationDate, user.Type, GetUserDescription(user.RegistrationUser))
            Next
        End If

        If RadTreeSettori.Nodes.Count > 0 Then
            fldCurrentTenant.SetDisplay(True)
        End If

        If RadTreeRoleTenant.Nodes.Count > 0 Then
            fldOtherTenant.SetDisplay(True)
        End If
    End Sub

    Private Function GetUserDescription(account As String) As String
        Dim adUser As AccountModel = CommonAD.GetAccount(account)
        If adUser Is Nothing Then
            Return account
        End If
        Return String.Concat(adUser.DisplayName, " (", adUser.Account, ")")
    End Function

    Public Sub SetProtocolSourceUsers(protocolUsers As IEnumerable(Of ProtocolUser))
        If DocSuiteContext.Current.SimplifiedPrivacyEnabled AndAlso protocolUsers IsNot Nothing Then
            For Each user As ProtocolUser In CurrentProtocol.Users.Where(Function(u) u.Type = ProtocolUserType.Authorization)
                If Not SourceUsers.ContainsKey(user.Account) Then
                    SourceUsers.Add(user.Account, GetUserDescription(user.Account))
                End If
            Next
        End If
    End Sub

    ''' <summary> Popola la treeview con i ruoli impostati nel datasource. </summary>
    Public Shadows Sub DataBind(Optional ByVal recursive As Boolean = False, Optional ByVal onlyWithPecAddress As Boolean = False)
        RadTreeSettori.Nodes.Clear()
        RadTreeRoleTenant.Nodes.Clear()

        Dim currentTenantRoleList As IList(Of Role) = New List(Of Role)
        Dim otherTenantRoleList As IList(Of Role) = New List(Of Role)

        If SourceRoles IsNot Nothing AndAlso SourceRoles.Count > 0 Then
            For Each role As Role In SourceRoles.Where(Function(x) x.TenantId.Equals(DocSuiteContext.Current.CurrentTenant.TenantId))
                currentTenantRoleList.Add(role)
            Next

            For Each role As Role In SourceRoles.Where(Function(x) Not x.TenantId.Equals(DocSuiteContext.Current.CurrentTenant.TenantId))
                otherTenantRoleList.Add(role)
            Next
            AddRoles(currentTenantRoleList, False, recursive, onlyWithPecAddress)
            AddRoles(otherTenantRoleList, False, recursive, onlyWithPecAddress)

        End If

        If DocSuiteContext.Current.SimplifiedPrivacyEnabled AndAlso SourceUsers IsNot Nothing AndAlso SourceUsers.Count > 0 Then
            For Each user As KeyValuePair(Of String, String) In SourceUsers
                AddUserAuthorization(user.Value, user.Key, String.Empty, False, Nothing, ProtocolUserType.Authorization, String.Empty)
            Next
        End If

        If RadTreeSettori.Nodes.Count > 0 Then
            fldCurrentTenant.SetDisplay(True)
        End If

        If RadTreeRoleTenant.Nodes.Count > 0 Then
            fldOtherTenant.SetDisplay(True)
        End If
    End Sub

    ''' <summary> Lista di ruoli presenti. </summary>
    Public Function GetRoles() As IList(Of Role)
        Dim values As String() = RadTreeSettori.GetAllNodes().Where(Function(x) x.Attributes(SelectedRoleAttribute).Eq(TrueAttributeValue)).Select(Function(x) x.Value).ToArray()
        Dim tenantValues As String() = RadTreeRoleTenant.GetAllNodes().Where(Function(x) x.Attributes(SelectedRoleAttribute).Eq(TrueAttributeValue)).Select(Function(x) x.Value).ToArray()

        SourceRoles = New List(Of Role)
        For Each value As String In values
            SourceRoles.Add(Facade.RoleFacade.GetById(Integer.Parse(value)))
        Next
        If tenantValues IsNot Nothing Then
            For Each value As String In tenantValues
                SourceRoles.Add(Facade.RoleFacade.GetByIdAndTenant(Integer.Parse(value), DocSuiteContext.Current.Tenants.FirstOrDefault(Function(x) Not x.CurrentTenant).TenantId))
            Next
        End If
        Return SourceRoles
    End Function

    Private Function GetCheckedNodes() As IList(Of RadTreeNode)
        Dim checkedNodes As IList(Of RadTreeNode) = RadTreeSettori.GetAllNodes().Where(Function(x) x.Attributes(SelectedRoleAttribute).Eq(TrueAttributeValue) AndAlso x.Checked).ToList()
        For Each item As RadTreeNode In RadTreeRoleTenant.GetAllNodes().Where(Function(x) x.Attributes(SelectedRoleAttribute).Eq(TrueAttributeValue) AndAlso x.Checked).ToList()
            checkedNodes.Add(item)
        Next
        Return checkedNodes
    End Function

    Public Function HasCheckedRoles() As Boolean
        Return GetCheckedNodes().Count > 0
    End Function

    ''' <summary> Lista di ruoli selezionati. </summary>
    Public Function GetCheckedRoles() As IList(Of Role)
        Dim ids As New List(Of Integer)(GetCheckedNodes().Select(Function(x) Integer.Parse(x.Value)).ToList())
        Return Facade.RoleFacade.GetByIds(ids)
    End Function

    ''' <summary> Lista di id dei ruoli selezionati. </summary>
    Public Function GetCheckedRoleIds() As IList(Of Integer)
        Dim ids As New List(Of Integer)(GetCheckedNodes().Select(Function(x) Integer.Parse(x.Value)).ToList())
        Return ids
    End Function

    ''' <summary> Lista di ruoli non selezionati. </summary>
    Public Function GetUncheckedRoles() As IList(Of Role)
        Dim values As New List(Of Integer)(RadTreeSettori.GetAllNodes().Where(Function(x) x.Attributes(SelectedRoleAttribute).Eq(TrueAttributeValue) AndAlso Not x.Checked).Select(Function(x) Integer.Parse(x.Value)).ToArray())
        For Each item As Integer In RadTreeRoleTenant.GetAllNodes().Where(Function(x) x.Attributes(SelectedRoleAttribute).Eq(TrueAttributeValue) AndAlso Not x.Checked).Select(Function(x) Integer.Parse(x.Value)).ToArray()
            values.Add(item)
        Next
        Return Facade.RoleFacade.GetByIds(values)
    End Function

    ''' <summary> Visualizza i nodi utente che appartengono ai ruoli selezionati. </summary>
    ''' <param name="onlyAdministrator">True visualizzati solo gli utenti appartenenti a gruppi di amministrazione, false altrimenti</param>
    Public Sub ShowUsers(ByVal onlyAdministrator As Boolean)
        If ViewState("ShowUser") IsNot Nothing Then
            Exit Sub
        End If

        For Each node As RadTreeNode In RadTreeSettori.GetAllNodes()
            If Not node.Attributes(TypeAttribute).Eq(TypeAttributeValue.User.ToString()) Then
                Continue For
            End If

            If Not onlyAdministrator Then
                node.Visible = True
                Continue For
            End If

            node.Visible = Boolean.Parse(node.Attributes("Administrator"))
        Next
        ViewState("ShowUser") = True
    End Sub

    ''' <summary> Nasconde i nodi utente che appartengono ai ruoli selezionati </summary>
    Public Sub HideUsers()
        If ViewState("ShowUser") Is Nothing Then
            Exit Sub
        End If

        For Each node As RadTreeNode In RadTreeSettori.GetAllNodes()
            If node.Attributes(TypeAttribute).Eq(TypeAttributeValue.User.ToString()) Then
                node.Visible = False
            End If
        Next
        ViewState.Remove("ShowUser")
    End Sub

    ''' <summary> Carica i settori dei contatti all'interno dello usercontrol passato come parametro </summary>
    Public Sub LoadRoleContacts(ByVal contactList As IList(Of ContactDTO), clearRoles As Boolean)
        If clearRoles Then
            SourceRoles.Clear()
        Else
            SourceRoles.AddRange(GetRoles())
            If DocSuiteContext.Current.SimplifiedPrivacyEnabled Then
                For Each user As KeyValuePair(Of String, String) In GetUsers()
                    If Not SourceUsers.ContainsKey(user.Key) Then
                        SourceUsers.Add(user.Key, user.Value)
                    End If
                Next
            End If
        End If
        If (contactList IsNot Nothing) AndAlso (contactList.Count > 0) Then
            SourceRoles.AddRange(contactList.Where(Function(cdto) cdto.Contact IsNot Nothing AndAlso cdto.Contact.Role IsNot Nothing).Select(Function(c) c.Contact.Role))
        End If
        If SourceRoles.Count > 0 Then
            DataBind()
        End If
    End Sub

    Private Sub SetGroupNode(ByRef node As RadTreeNode, ByVal roleGroup As RoleGroup)
        node.Text = roleGroup.Name
        node.Value = roleGroup.Role.IdRoleTenant.ToString()
        node.ImageUrl = "../App_Themes/DocSuite2008/imgset16/GroupMembers.png"
        node.Attributes.Add(NodeTypeAttribute, NodeTypeAttributeValue.Group.ToString())
        node.Target = "InternoPrincipale"
    End Sub

    Private Sub AddRecursiveNode(ByRef parentNode As RadTreeNode, ByRef role As Role, ByVal onlyWithPecAddress As Boolean, ByRef treeView As RadTreeView, tenantId As Guid, Optional multitenantEnabled As Boolean = False)
        Dim nodeToAdd As RadTreeNode = CreateNode(role, False, onlyWithPecAddress)
        If Not onlyWithPecAddress Then
            AddRoleGroups(nodeToAdd, role)
        End If
        If parentNode Is Nothing Then
            treeView.Nodes.Add(nodeToAdd)
        Else
            parentNode.Nodes.Add(nodeToAdd)
        End If
        Dim children As IList(Of Role) = Facade.RoleFacade.GetItemsByParentId(role.IdRoleTenant, onlyWithPecAddress, tenantId, multitenantEnabled:=multitenantEnabled)
        If children IsNot Nothing AndAlso children.Count > 0 Then
            For Each child As Role In children
                AddRecursiveNode(nodeToAdd, child, onlyWithPecAddress, treeView, tenantId, multitenantEnabled:=multitenantEnabled)
            Next
        End If
    End Sub

    Private Function CreateNode(ByVal role As Role, ByVal expandCallBack As Boolean, ByVal onlyWithPecAddress As Boolean) As RadTreeNode
        Dim node As New RadTreeNode()
        node.Text = role.Name
        node.Value = role.IdRoleTenant.ToString()
        node.Attributes.Add("IdRoleTenant", role.IdRoleTenant.ToString())
        If (role.Father Is Nothing) Then
            If (expandCallBack) Then
                node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack
            End If
            node.ImageUrl = ImagePath.SmallRole
            node.Attributes.Add(NodeTypeAttribute, NodeTypeAttributeValue.Role.ToString())
        Else
            node.ImageUrl = ImagePath.SmallSubRole
            node.Attributes.Add(NodeTypeAttribute, NodeTypeAttributeValue.SubRole.ToString())
        End If
        If (role.IsActive <> 1) Then
            node.Style.Add("color", "gray")
            node.Attributes.Add("Recovery", "true")
        Else
            node.Style.Remove("color")
            node.Attributes.Add("Recovery", "false")
        End If

        If onlyWithPecAddress Then
            ' vengono evidenziati solo i nodi con caselle PEC associate (i RoleGroups non sono visualizzati)
            node.Font.Bold = (role.Mailboxes IsNot Nothing AndAlso role.Mailboxes.Count > 0)
        Else
            node.Font.Bold = True
        End If
        node.Expanded = True
        Return node
    End Function

    Private Sub AddRoleGroups(ByRef node As RadTreeNode, ByVal role As Role)
        Dim groupNode As RadTreeNode
        If Not (role.RoleGroups Is Nothing) Then
            For Each roleGroup As RoleGroup In role.RoleGroups
                groupNode = New RadTreeNode()
                SetGroupNode(groupNode, roleGroup)
                node.Nodes.Add(groupNode)
                node.Attributes.Add(SelectedFullIncrementalPathAttribute, roleGroup.Role.FullIncrementalPath)

                AddNodeUser(groupNode, roleGroup, False)
            Next
        End If
    End Sub

    Private Sub SetRolesAddedList(ByVal role As Role)
        ' Rimuovo dalla lista dei REMOVED
        If RoleListRemoved.Contains(role.Id) Then
            RoleListRemoved.Remove(role.Id)
        End If
        ' VERIFICO SE PRESENTE
        If Not RoleListAdded.Contains(role.Id) Then
            ' VERIFICO se era già in datasource
            For Each roleds As Role In SourceRoles
                If roleds.IdRoleTenant = role.IdRoleTenant Then
                    Exit Sub
                End If
            Next
            RoleListAdded.Add(role.Id)
        End If

    End Sub

    Private Sub SetRolesRemovedList(ByVal role As Role, ByRef treeView As RadTreeView)
        'Verifica eventuali figli
        Dim node As RadTreeNode = treeView.FindNodeByValue(CType(role.IdRoleTenant, String))

        If RoleListAdded.Contains(role.Id) Then
            RoleListAdded.Remove(role.Id)
        Else
            If (node.Attributes.Item(SelectedRoleAttribute).Eq(TrueAttributeValue)) Then
                RoleListRemoved.Add(role.Id)
            End If
        End If

        If node IsNot Nothing Then
            For Each child As RadTreeNode In node.Nodes
                If (Not RoleListRemoved.Contains(child.Value)) Then
                    Dim idChild As Integer = CType(child.Value, Integer)
                    SetRolesRemovedList(Facade.RoleFacade.GetByIdAndTenant(idChild, role.TenantId), treeView)
                End If
            Next
        End If


    End Sub

    ''' <summary> Seleziona il settore </summary>
    Public Sub CheckRole(ByVal role As Role)
        Dim nodeToCheck As RadTreeNode = RadTreeSettori.FindNodeByValue(role.IdRoleTenant.ToString())
        If nodeToCheck IsNot Nothing AndAlso nodeToCheck.Checkable Then
            nodeToCheck.Checked = True
        End If
    End Sub

    Public Sub CheckAll()
        CheckNodes(RadTreeSettori.GetAllNodes(), True)
    End Sub

    Public Sub UnCheckAll()
        CheckNodes(RadTreeSettori.GetAllNodes(), False)
    End Sub

    Private Sub CheckNodes(ByRef nodes As IList(Of RadTreeNode), ByVal check As Boolean)
        If nodes.IsNullOrEmpty() Then
            Exit Sub
        End If
        For Each node As RadTreeNode In nodes
            If node.Checkable Then
                node.Checked = check
            End If
        Next
    End Sub

    Public Function GetRoleValues(selected As Boolean, nodeType As NodeTypeAttributeValue) As List(Of String)
        Dim selectedNodes As List(Of String) = Nothing
        For Each node As RadTreeNode In RadTreeSettori.Nodes
            GetSelectedNodes(node, selectedNodes, nodeType, selected)
        Next

        For Each node As RadTreeNode In RadTreeRoleTenant.Nodes
            GetSelectedNodes(node, selectedNodes, nodeType, selected)
        Next
        Return selectedNodes
    End Function

    Public Function GetPrivacyRoles() As IList(Of String)
        Dim privacyNodes As List(Of String) = New List(Of String)
        For Each node As RadTreeNode In RadTreeSettori.GetAllNodes()
            If node.Attributes.Item(PrivacyRoleAttribute) IsNot Nothing AndAlso node.Attributes.Item(PrivacyRoleAttribute).Eq(TrueAttributeValue) Then
                privacyNodes.Add(node.Value)
            End If
        Next

        For Each node As RadTreeNode In RadTreeRoleTenant.GetAllNodes()
            If node.Attributes.Item(PrivacyRoleAttribute) IsNot Nothing AndAlso node.Attributes.Item(PrivacyRoleAttribute).Eq(TrueAttributeValue) Then
                privacyNodes.Add(node.Value)
            End If
        Next
        Return privacyNodes
    End Function

    Public Function GetUsers() As IDictionary(Of String, String)
        Dim userNodes As IDictionary(Of String, String) = New Dictionary(Of String, String)
        For Each node As RadTreeNode In RadTreeSettori.Nodes
            If node.Attributes.Item(UserAuthorizationAttribute) IsNot Nothing AndAlso node.Attributes.Item(UserAuthorizationAttribute).Eq(TrueAttributeValue) Then
                userNodes.Add(New KeyValuePair(Of String, String)(node.Value, node.Text))
            End If
        Next
        Return userNodes
    End Function

    ''' <summary> Popola una lista con i valori dei nodi selezionati del tipo specificato. </summary>
    ''' <param name="node">Nodo da popolare</param>
    ''' <param name="selectedNodes">Lista da popolare</param>
    ''' <param name="nodeType">Tipo nodo da considerare</param>
    ''' <param name="checked">Indica se cerco i nodi selezionati o meno</param>
    Private Overloads Sub GetSelectedNodes(ByVal node As RadTreeNode, ByRef selectedNodes As List(Of String), ByVal nodeType As NodeTypeAttributeValue?, checked As Boolean)

        If selectedNodes Is Nothing Then
            selectedNodes = New List(Of String)
        End If

        If node Is Nothing Then
            Exit Sub
        End If

        If node.Checkable AndAlso node.Checked = checked Then
            If nodeType.HasValue Then
                If node.Attributes.Item(NodeTypeAttribute).Eq(nodeType.ToString()) Then
                    selectedNodes.Add(node.Value)
                End If
            Else
                selectedNodes.Add(node.Value)
            End If
        End If

        For Each childNode As RadTreeNode In node.Nodes
            GetSelectedNodes(childNode, selectedNodes, nodeType, checked)
        Next
    End Sub

    Public Function GetFullIncrementalPathAttribute(selected As Boolean, nodeType As NodeTypeAttributeValue) As IList(Of KeyValuePair(Of String, String))
        Dim selectedNodes As IList(Of KeyValuePair(Of String, String)) = Nothing
        For Each node As RadTreeNode In RadTreeSettori.Nodes
            GetSelectedFullIncrementalPathNodes(node, selectedNodes, nodeType, selected)
        Next

        For Each node As RadTreeNode In RadTreeRoleTenant.Nodes
            GetSelectedFullIncrementalPathNodes(node, selectedNodes, nodeType, selected)
        Next
        Return selectedNodes
    End Function

    ''' <summary> Popola una lista con il FullIncrementalPath dei nodi selezionati del tipo specificato. </summary>
    ''' <param name="node">Nodo da popolare</param>
    ''' <param name="selectedNodes">Lista da popolare</param>
    ''' <param name="nodeType">Tipo nodo da considerare</param>
    ''' <param name="checked">Indica se cerco i nodi selezionati o meno</param>
    Private Overloads Sub GetSelectedFullIncrementalPathNodes(ByVal node As RadTreeNode, ByRef selectedNodes As IList(Of KeyValuePair(Of String, String)), ByVal nodeType As NodeTypeAttributeValue?, checked As Boolean)
        If selectedNodes Is Nothing Then
            selectedNodes = New List(Of KeyValuePair(Of String, String))
        End If

        If node Is Nothing Then
            Exit Sub
        End If

        If node.Checkable AndAlso node.Checked = checked Then
            If nodeType.HasValue Then
                If node.Attributes.Item(NodeTypeAttribute).Eq(nodeType.ToString()) Then
                    selectedNodes.Add(New KeyValuePair(Of String, String)(node.Value, node.Attributes.Item(NodeTypeAttributeValue.FullIncrementalPath.ToString())))
                End If
            Else
                selectedNodes.Add(New KeyValuePair(Of String, String)(node.Value, node.Attributes.Item(NodeTypeAttributeValue.FullIncrementalPath.ToString())))
            End If
        End If

        For Each childNode As RadTreeNode In node.Nodes
            GetSelectedFullIncrementalPathNodes(childNode, selectedNodes, nodeType, checked)
        Next
    End Sub

    ''' <summary> Popola una lista con i valori dei nodi Role in Copia Conoscenza. </summary>
    ''' <param name="node">Nodo radice</param>
    Public Shared Sub GetCcRoleNodes(ByVal node As RadTreeNode, ByRef foundNodes As IList(Of String))
        If node Is Nothing Then
            Exit Sub
        End If
        ' ATTENZIONE! Se il nodo di settore è checkabile, la Copia Conoscenza deve essere recuperata dal checkbox e non dall'attributo NodeCC!
        If Not node.Checkable _
            AndAlso node.Attributes.Item(NodeTypeAttribute) IsNot Nothing AndAlso node.Attributes.Item(NodeTypeAttribute).Eq(NodeTypeAttributeValue.Role.ToString()) _
            AndAlso node.Attributes.Item("NodeCC") IsNot Nothing AndAlso node.Attributes.Item("NodeCC").Eq("cc") Then
            foundNodes.Add(node.Value)
        End If
        For Each childNode As RadTreeNode In node.Nodes
            GetCcRoleNodes(childNode, foundNodes)
        Next
    End Sub

    ''' <summary> Popola una lista con i valori dei nodi del tipo specificato. </summary>
    ''' <param name="node">Nodo radice</param>
    ''' <param name="foundNodes">Lista da popolare</param>
    ''' <param name="nodeType">Tipo nodo da considerare</param>
    Private Overloads Shared Sub GetNodesByNodeType(ByVal node As RadTreeNode, ByRef foundNodes As List(Of String), ByVal nodeType As NodeTypeAttributeValue?)
        If node Is Nothing OrElse Not nodeType.HasValue Then
            Exit Sub
        End If
        If node.Attributes.Item(NodeTypeAttribute) IsNot Nothing AndAlso node.Attributes.Item(NodeTypeAttribute).Eq(nodeType.ToString()) Then
            foundNodes.Add(node.Value)
        End If
        For Each childNode As RadTreeNode In node.Nodes
            GetNodesByNodeType(childNode, foundNodes, nodeType)
        Next
    End Sub

    ''' <summary> Popola una lista con i valori dei nodi Role (vecchio popolamento). </summary>
    Public Function GetOldValues() As String()
        Dim values As String() = RadTreeSettori.GetAllNodes() _
            .Where(Function(node) node.Attributes(TypeAttribute).Eq(TypeAttributeValue.Role.ToString()) OrElse node.Attributes(TypeAttribute).Eq(TypeAttributeValue.SubRole.ToString())) _
            .Select(Function(node) node.Value).ToArray()

        Return values
    End Function

    ''' <summary> Recupera un ProtocolRole da un Id Role. </summary>
    Private Function GetProtocolRoleByRoleId(ByVal roleId As Integer) As ProtocolRole
        If (CurrentProtocol Is Nothing) OrElse (CurrentProtocol.Roles Is Nothing) OrElse CurrentProtocol.Roles.Count <= 0 Then
            Return Nothing
        End If

        For Each pr As ProtocolRole In CurrentProtocol.Roles
            If pr.Role.Id.Equals(roleId) Then
                Return pr
            End If
        Next
        Return Nothing
    End Function

    ''' <summary> Lista di IdRole presenti nell'albero nei quali risulto essere manager. </summary>
    Private Function GetManageableRoles() As IList(Of Integer)
        Dim foundCurrentTenantRoles As New List(Of String)
        Dim foundOtherTenantRoles As New List(Of String)
        For Each node As RadTreeNode In RadTreeSettori.Nodes
            GetNodesByNodeType(node, foundCurrentTenantRoles, NodeTypeAttributeValue.Role)
        Next

        For Each node As RadTreeNode In RadTreeRoleTenant.Nodes
            GetNodesByNodeType(node, foundOtherTenantRoles, NodeTypeAttributeValue.Role)
        Next

        Dim manageables As New List(Of Integer)
        For Each temp As String In foundCurrentTenantRoles
            Dim roleId As Integer = Integer.Parse(temp)
            If Not manageables.Contains(roleId) AndAlso Facade.RoleFacade.IsRoleDistributionManager(roleId, DocSuiteContext.Current.CurrentTenant.TenantId) Then
                manageables.Add(roleId)
            End If
        Next

        For Each temp As String In foundOtherTenantRoles
            Dim roleId As Integer = Integer.Parse(temp)
            If Not manageables.Contains(roleId) AndAlso Facade.RoleFacade.IsRoleDistributionManager(roleId, DocSuiteContext.Current.Tenants.SingleOrDefault(Function(x) Not x.CurrentTenant).TenantId) Then
                manageables.Add(roleId)
            End If
        Next

        Return manageables
    End Function

    ''' <summary> Insieme dei ruoli dove sono manager in formato querystring. </summary>
    Public Function GetManageableRolesParameter() As String
        Dim manageables As IList(Of Integer) = GetManageableRoles()

        If manageables.IsNullOrEmpty() Then
            Return String.Empty
        End If

        Return String.Join("|", manageables)
    End Function

    Public Sub ResetManageableRoles()
        Session(GetManageableRolesSessionName) = Nothing
        _manageableRoles = Nothing
    End Sub

    ''' <summary> Verifica se un nodo RoleUser esiste in ProtocolRoleUser. </summary>
    ''' <param name="roleUserNode">Nodo di tipo RoleUser</param>
    Private Function ExistsInProtocolRoleUser(ByVal roleUserNode As RadTreeNode) As Boolean
        If (CurrentProtocol Is Nothing) OrElse (CurrentProtocol.RoleUsers Is Nothing) OrElse CurrentProtocol.RoleUsers.Count <= 0 Then
            Return False
        End If

        Dim roleUserNodeValue As String() = roleUserNode.Value.Split("|"c)
        Dim idRole As Integer = CType(roleUserNodeValue.GetValue(ProtocolRoleUserColumns.IdRole), Integer)
        Dim groupName As String = CType(roleUserNodeValue.GetValue(ProtocolRoleUserColumns.GroupName), String)
        Dim userName As String = CType(roleUserNodeValue.GetValue(ProtocolRoleUserColumns.UserName), String)
        Return CurrentProtocol.RoleUsers.Any(Function(x) x.Role.Id = idRole AndAlso x.GroupName.Eq(groupName) AndAlso x.UserName.Eq(userName))
    End Function

    ''' <summary> Verifica se il RoleGroup ha almeno un ProtocolRoleUser. </summary>
    ''' <param name="roleGroup">Abbinamento settore/gruppo</param>
    Private Function ExistsGroupInProtocolRoleUser(ByVal roleGroup As RoleGroup) As Boolean
        If (CurrentProtocol Is Nothing) OrElse CurrentProtocol.RoleUsers.IsNullOrEmpty() Then
            Return False
        End If

        For Each pru As ProtocolRoleUser In CurrentProtocol.RoleUsers
            If pru.Role.Id = roleGroup.Role.IdRoleTenant AndAlso pru.GroupName.Eq(roleGroup.Name) Then
                Return True
            End If
        Next
        Return False
    End Function

    ''' <summary> Recupera il valore di un nodo RoleUser. </summary>
    ''' <param name="idRole">Id del settore</param>
    ''' <param name="groupName">Nome del gruppo</param>
    ''' <param name="userName">Nome utente</param>
    ''' <param name="userAccount">Account utente</param>
    Private Overloads Function GetRoleUserNodeValue(ByVal idRole As String, ByVal groupName As String, ByVal userName As String, ByVal userAccount As String) As String
        Return String.Format("{0}|{1}|{2}|{3}", idRole, groupName, userName, userAccount)
    End Function

    ''' <summary> Recupera il valore di un nodo RoleUser. </summary>
    ''' <param name="roleGroup">Abbinamento settore/gruppo</param>
    ''' <param name="userName">Nome utente</param>
    ''' <param name="userAccount">Account utente</param>
    Private Overloads Function GetRoleUserNodeValue(ByVal roleGroup As RoleGroup, ByVal userName As String, ByVal userAccount As String) As String
        Return GetRoleUserNodeValue(roleGroup.Role.IdRoleTenant.ToString(), roleGroup.Name, userName, userAccount)
    End Function

    ''' <summary> Imposta un nodo di tipo RoleUser. </summary>
    ''' <param name="roleGroup">Abbinamento settore/gruppo</param>
    ''' <param name="userName">Nome utente</param>
    ''' <param name="userAccount">Account Utente</param>
    Private Function SetRoleUserNodeForRoleUser(ByVal roleGroup As RoleGroup, user As AccountModel, ByVal asRoleDistributionManager As Boolean, ByVal asCopiaConoscenza As Boolean) As RadTreeNode
        Dim retval As RadTreeNode = New RadTreeNode()

        retval.Text = user.Name
        If asCopiaConoscenza Then
            retval.Text = String.Concat(retval.Text, " (CC)")
        End If
        retval.Attributes.Add(NodeTypeAttribute, NodeTypeAttributeValue.RoleUser.ToString())
        If roleGroup.ProtocolRights.IsRoleManager Then
            retval.Checkable = False
            retval.Style.Item("color") = "gray"
            retval.ImageUrl = "../Comm/images/User16M.gif"
        Else
            retval.Value = GetRoleUserNodeValue(roleGroup, user.Name, user.GetFullUserName())
            retval.Checkable = Checkable AndAlso asRoleDistributionManager
            retval.Style.Item("color") = "black"
            retval.ImageUrl = "../Comm/images/User16U.gif"
        End If
        'Memorizzo lo UserAccount per verificare che non esista già il nodo
        retval.Attributes.Add(UserAccountAttribute, user.GetFullUserName())
        retval.ExpandMode = TreeNodeExpandMode.ClientSide
        If (SelectedRoleUserAccount.Eq(user.GetFullUserName()) AndAlso retval.Checkable) Then
            retval.Checked = True
        End If
        Return retval
    End Function

    ''' <summary> Aggiunge i nodi RoleUser all'albero. </summary>
    ''' <param name="roleGroup">Abbinamento settore/gruppo</param>
    Private Sub AddRoleUsersForRoleUser(ByVal roleGroup As RoleGroup, ByVal asRoleDistributionManager As Boolean, ByVal asCopiaConoscenza As Boolean)
        Dim fatherNode As RadTreeNode = RadTreeSettori.FindNodeByValue(CreateRoleGroupNodeValue(roleGroup))
        Dim users As IEnumerable(Of AccountModel)
        If (roleGroup.SecurityGroup Is Nothing) Then
            Throw New DocSuiteException(String.Concat("Il gruppo ", roleGroup.Name, "(", roleGroup.Id, ") non è configurato correttamente nella SecurityGroup"))
        End If
        users = Facade.SecurityUsersFacade.GetUsersByGroup(roleGroup.SecurityGroup.Id).Select(Function(f) New AccountModel(f.Account, f.Description, domain:=f.UserDomain) With {.DisplayName = f.Description})
        For Each user As AccountModel In users.OrderBy(Function(f) f.Name)
            Dim currentNode As RadTreeNode = SetRoleUserNodeForRoleUser(roleGroup, user, asRoleDistributionManager, asCopiaConoscenza)

            If fatherNode.Nodes.FindNodeByAttribute(UserAccountAttribute, currentNode.Attributes(UserAccountAttribute)) Is Nothing Then
                If ProtocolRoleUsersOnly Then
                    If roleGroup.ProtocolRights.IsRoleManager OrElse ExistsInProtocolRoleUser(currentNode) Then
                        fatherNode.Nodes.Add(currentNode)
                    End If
                Else
                    If roleGroup.ProtocolRights.IsRoleManager OrElse ExistsInProtocolRoleUser(currentNode) OrElse asRoleDistributionManager Then
                        fatherNode.Nodes.Add(currentNode)
                    End If
                End If
            End If
        Next
    End Sub

    Private Function CreateRoleGroupNodeValue(ByVal roleGroup As RoleGroup) As String
        Return String.Format("{0}|{1}", roleGroup.Role.IdRoleTenant.ToString(), If(roleGroup.ProtocolRights.IsRoleManager, "R", "U"))
    End Function

    ''' <summary> Imposta un nodo di tipo RoleGroup. </summary>
    ''' <param name="roleGroup">Abbinamento settore/gruppo</param>
    Private Function SetRoleGroupNodeForRoleUser(ByVal roleGroup As RoleGroup) As RadTreeNode
        Dim retval As New RadTreeNode

        If roleGroup.ProtocolRights.IsRoleManager Then
            retval.Text = "RESPONSABILI"
            retval.Expanded = False
        Else
            retval.Text = "UTENTI"
            retval.Expanded = CurrentRoleUserViewMode.HasValue AndAlso CurrentRoleUserViewMode.Value > RoleUserViewMode.Roles
        End If
        retval.Value = CreateRoleGroupNodeValue(roleGroup)
        retval.Attributes.Add(NodeTypeAttribute, NodeTypeAttributeValue.RoleGroup.ToString())
        retval.Checkable = False
        retval.ImageUrl = "../App_Themes/DocSuite2008/imgset16/GroupMembers.png"
        retval.Style.Add("color", "darkblue")
        retval.ExpandMode = TreeNodeExpandMode.ClientSide

        Return retval
    End Function

    ''' <summary> Aggiunge i nodi RoleGroup all'albero. </summary>
    ''' <param name="role">Settore</param>
    Private Sub AddRoleGroupsForRoleUser(ByVal role As Role, ByVal asRoleDistributionManager As Boolean, ByVal asCopiaConoscenza As Boolean)
        Dim viewManagers As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("viewManagers"), RadToolBarButton)
        Dim fatherNode As RadTreeNode = RadTreeSettori.FindNodeByValue(role.IdRoleTenant.ToString())
        If DocSuiteContext.Current.ProtocolEnv.MultiDomainEnabled AndAlso Not role.TenantId.Equals(Guid.Empty) AndAlso Not role.TenantId.Equals(DocSuiteContext.Current.CurrentTenant.TenantId) Then
            fatherNode = RadTreeRoleTenant.FindNodeByValue(role.IdRoleTenant.ToString())
        End If

        For Each rg As RoleGroup In role.RoleGroups
            If Not rg.ProtocolRights.IsRoleEnabled Then
                Continue For
            End If
            If Not viewManagers.Checked AndAlso rg.ProtocolRights.IsRoleManager Then
                Continue For
            End If

            Dim currentNode As RadTreeNode = fatherNode.Nodes.FindNodeByValue(CreateRoleGroupNodeValue(rg))
            If currentNode Is Nothing Then
                currentNode = SetRoleGroupNodeForRoleUser(rg)
                fatherNode.Nodes.Add(currentNode)
            End If

            If ProtocolRoleUsersOnly Then
                If rg.ProtocolRights.IsRoleManager OrElse ExistsGroupInProtocolRoleUser(rg) Then
                    AddRoleUsersForRoleUser(rg, asRoleDistributionManager, asCopiaConoscenza) ' Aggiungo i nodi RoleUser.
                End If
            Else
                AddRoleUsersForRoleUser(rg, asRoleDistributionManager, asCopiaConoscenza)
            End If

            If Not currentNode.Nodes.Count > 0 Then
                currentNode.Remove()
            End If
        Next
    End Sub


    ''' <summary> Imposta un nodo di tipo Role. </summary>
    ''' <param name="role">Settore</param>
    Private Function SetRoleNodeForRoleUser(ByVal role As Role, ByVal protocolRole As ProtocolRole, ByVal asRoleDistributionManager As Boolean) As RadTreeNode
        Dim retval As New RadTreeNode

        retval.Text = role.Name
        retval.Value = role.IdRoleTenant.ToString()
        retval.Attributes.Add(NodeTypeAttribute, NodeTypeAttributeValue.Role.ToString())
        ' La possibilità di Copia Conoscenza è concessa quando:
        ' l'usercontrol è configurato per essere checkabile
        ' - E - l'utente ha i permessi di distribuzione a livello di contenitore
        ' - OPPURE - l'utente è manager di distribuzione in almeno un settore padre - MA NON - di quello corrente.
        retval.Checkable = Checkable AndAlso (CopiaConoscenzaEnabled OrElse (asRoleDistributionManager AndAlso Not Facade.RoleFacade.IsRoleDistributionManager(role.IdRoleTenant, role.TenantId)))
        retval.Expanded = True
        retval.ImageUrl = ImagePath.SmallRole
        retval.Font.Bold = True

        If asRoleDistributionManager Then
            retval.Attributes.Add("NodeManager", "M")
        End If

        ' Imposto le proprietà del nodo per la Copia Conoscenza.
        Dim isCopiaConoscenza As Boolean = protocolRole IsNot Nothing AndAlso protocolRole.Type IsNot Nothing AndAlso protocolRole.Type.Eq(ProtocolRoleTypes.CarbonCopy)
        If isCopiaConoscenza Then
            If retval.Checkable Then
                retval.Checked = True
            End If
            retval.Text = retval.Text & " (CC)"
            retval.Attributes.Add("NodeCC", "CC")
        End If

        ' Imposto le proprietà del nodo per l'autorizzazione esplicita/implicita.
        Dim isExplicitAuthorization As Boolean = protocolRole IsNot Nothing AndAlso protocolRole.DistributionType IsNot Nothing AndAlso protocolRole.DistributionType.Eq(ProtocolDistributionType.Explicit)
        retval.Font.Bold = isExplicitAuthorization
        retval.Font.Italic = Not isExplicitAuthorization
        retval.ExpandMode = TreeNodeExpandMode.ClientSide

        Return retval
    End Function

    Private Overloads Sub AddRoleForRoleUser(ByVal role As Role, ByVal idProtocolType As Integer, ByVal isDistributable As Boolean)
        Dim treeView As RadTreeView = RadTreeSettori
        If DocSuiteContext.Current.ProtocolEnv.MultiDomainEnabled AndAlso Not role.TenantId.Equals(Guid.Empty) AndAlso Not role.TenantId.Equals(DocSuiteContext.Current.CurrentTenant.TenantId) Then
            treeView = RadTreeRoleTenant
        End If
        AddRoleForRoleUser(role, idProtocolType, isDistributable, treeView)
    End Sub

    ''' <summary> Aggiunge un nodo Role all'albero. </summary>
    Private Overloads Sub AddRoleForRoleUser(ByVal role As Role, ByVal idProtocolType As Integer, ByVal isDistributable As Boolean, treeView As RadTreeView)
        Dim currentNode As RadTreeNode = treeView.FindNodeByValue(role.IdRoleTenant.ToString())
        If currentNode IsNot Nothing Then
            Exit Sub
        End If

        Dim asRoleDistributionManager As Boolean
        Dim fatherNode As RadTreeNode

        If role.Father Is Nothing Then
            ' Essendo una nuova radice verifico di essere manager del settore corrente.
            fatherNode = Nothing
            asRoleDistributionManager = Facade.RoleFacade.IsRoleDistributionManager(role.IdRoleTenant, role.TenantId)
        Else
            AddRoleForRoleUser(role.Father, idProtocolType, isDistributable, treeView)

            fatherNode = treeView.FindNodeByValue(role.Father.IdRoleTenant.ToString())
            ' Verifico di essere manager del settore padre - OPPURE - di esserlo per il settore corrente.
            asRoleDistributionManager = (fatherNode.Attributes("NodeManager") IsNot Nothing AndAlso fatherNode.Attributes("NodeManager").Eq("M")) OrElse Facade.RoleFacade.IsRoleDistributionManager(role.IdRoleTenant, role.TenantId)
        End If
        If Not DocSuiteContext.Current.ProtocolEnv.DistributionHierarchicalEnabled Then
            asRoleDistributionManager = True
        End If
        Dim currentProtocolRole As ProtocolRole = GetProtocolRoleByRoleId(role.IdRoleTenant)
        currentNode = SetRoleNodeForRoleUser(role, currentProtocolRole, asRoleDistributionManager)

        ' Aggiungo il FullIncrementalPath su tutti i nodi
        currentNode.Attributes.Add(SelectedFullIncrementalPathAttribute, role.FullIncrementalPath)

        If fatherNode IsNot Nothing Then
            fatherNode.Nodes.Add(currentNode)
        Else
            treeView.Nodes.Add(currentNode)
        End If

        Dim asCopiaConoscenza As Boolean = currentProtocolRole IsNot Nothing AndAlso currentProtocolRole.Type IsNot Nothing AndAlso currentProtocolRole.Type.Eq(ProtocolRoleTypes.CarbonCopy)
        If ProtocolEnv.ProtocolDistributionTypologies.Contains(idProtocolType) AndAlso isDistributable Then
            AddRoleGroupsForRoleUser(role, asRoleDistributionManager, asCopiaConoscenza) ' Aggiungo i nodi RoleGroup.
        End If
    End Sub

    ''' <summary> Popola l'albero per la gestione di RoleUser. </summary>
    Public Sub DataBindForRoleUser(ByVal idProtocolType As Integer?, ByVal isDistributable As Boolean?, Optional ByVal forceisDistributableToTrue As Boolean = False, Optional RadTreeSettoriOnClientNodeClick As Boolean = False)
        RadTreeSettori.Nodes.Clear()
        RadTreeRoleTenant.Nodes.Clear()
        If Not RadTreeSettoriOnClientNodeClick Then
            RadTreeSettori.OnClientNodeClicking = "CanceledClientNodeClicking"
        End If
        RadTreeRoleTenant.OnClientNodeClicking = "CanceledClientNodeClicking"

        idProtocolType = If(idProtocolType, CurrentProtocol.Type.Id)
        isDistributable = If(isDistributable, CurrentProtocolRights.IsDistributable) OrElse forceisDistributableToTrue

        For Each r As Role In SourceRoles
            AddRoleForRoleUser(r, idProtocolType.Value, isDistributable.Value)
        Next

        If (RadTreeSettori.Nodes.Count > 0 OrElse RadTreeRoleTenant.Nodes.Count > 0) AndAlso Checkable AndAlso CurrentProtocol IsNot Nothing AndAlso Not CurrentProtocol.RoleUsers.IsNullOrEmpty() Then
            ' Seleziona i nodi RoleUser presenti in ProtocolRoleUser
            For Each pru As ProtocolRoleUser In CurrentProtocol.RoleUsers
                Dim seek As String = GetRoleUserNodeValue(pru.Role.Id.ToString(), pru.GroupName, pru.UserName, pru.Account)
                Dim currentNode As RadTreeNode = Nothing
                If DocSuiteContext.Current.ProtocolEnv.MultiDomainEnabled AndAlso Not pru.Role.TenantId.Equals(Guid.Empty) AndAlso Not pru.Role.TenantId.Equals(DocSuiteContext.Current.CurrentTenant.TenantId) Then
                    currentNode = RadTreeRoleTenant.FindNodeByValue(seek)
                Else
                    currentNode = RadTreeSettori.FindNodeByValue(seek)
                End If
                If currentNode IsNot Nothing AndAlso currentNode.Checkable Then
                    currentNode.Checked = True
                End If
            Next
        End If
        If RadTreeSettori.Nodes.Count > 0 Then
            fldCurrentTenant.SetDisplay(True)
        End If
        InitializeControls()
    End Sub

    Public Sub RemoveRoles(ByVal roles As IList(Of Role))
        For Each role As Role In roles
            DeleteSettore(role)
        Next
    End Sub

    ''' <summary>
    ''' metodo per cancellare tutti i settori dello usercontrol
    ''' </summary>
    Public Sub RemoveAllRoles()
        RadTreeSettori.Nodes.Clear()
        RadTreeRoleTenant.Nodes.Clear()
    End Sub

    Public Sub SetPrivacyAuthorizationNodes(roleIds As Integer())
        For Each id As Integer In roleIds
            Dim node As RadTreeNode = RadTreeSettori.FindNodeByValue(id.ToString())
            If node Is Nothing Then
                node = RadTreeRoleTenant.FindNodeByValue(id.ToString())
            End If
            node.ImageUrl = "../App_Themes/DocSuite2008/imgset16/lock.png"
            node.ToolTip = "Autorizzazione riservata/privacy"
            node.AddAttribute(PrivacyRoleAttribute, TrueAttributeValue)
        Next
    End Sub
    Private Function CanDeserialize(Of T)(json As String) As Boolean
        Try
            Dim jsonObject As T = JsonConvert.DeserializeObject(Of T)(json)
            Return jsonObject IsNot Nothing
        Catch ex As Exception
            Return False
        End Try
    End Function

#Region " MultiTenant "
    Private Sub MultiTenantDataBind(tenantId As Guid)
        Dim roles As List(Of Role) = Facade.RoleFacade.GetRootItems(True, False, tenantId, True).ToList()
        RadTreeSettori.Nodes.Clear()
        AddRoles(roles, False, True, False, multitenantEnabled:=ProtocolEnv.MultiTenantEnabled, tenantId:=tenantId)
    End Sub

    Private Sub RadTreeSettori_Load(sender As Object, e As EventArgs) Handles RadTreeSettori.Load

    End Sub
#End Region


#End Region

End Class
