Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Desks
Imports VecompSoftware.DocSuiteWeb.DTO.Desks
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.ExtensionMethods

Public Class uscInvitationUser
    Inherits DocSuite2008BaseControl

    Public Event UserAdded(ByVal sender As Object, ByVal e As DeskRoleUserEventArgs)
    Public Event UserDeleted(ByVal sender As Object, ByVal e As DeskRoleUserEventArgs)
    Public Event UserChangeRole(ByVal sender As Object, ByVal e As DeskRoleUserEventArgs)
#Region "Fields"
    Private Const COLUMN_INVITATION_MODE As String = "invitationType"
    Private Const COLUMN_INVITATION_TYPE_MODE As String = "invitationTypeMod"
    Private Const COMMON_SEL_CONTACT_DOMAIN_PATH As String = "../UserControl/CommonSelContactDomain.aspx"
    Private Const OPEN_WINDOW_SCRIPT As String = "return {0}_OpenWindow('{1}', '{2}', '{3}');"
    Public Const TYPE_MODIFY_NAME As String = "Modify"
    Private _buttonUserDeleteEnabled As Boolean? = False
#End Region

#Region "Properties"
    Public Property BindAsyncEnable As Boolean

    Public Property BtnAddUserVisible As Boolean
        Get
            Return ToolBar.Items.FindItemByValue("btnAddUser").Visible
        End Get
        Set(ByVal value As Boolean)
            ToolBar.Items.FindItemByValue("btnAddUser").Visible = value
        End Set
    End Property

    Public Property BtnRemoveUserVisible As Boolean
        Get
            Return ToolBar.Items.FindItemByValue("btnRemoveUser").Visible
        End Get
        Set(ByVal value As Boolean)
            ToolBar.Items.FindItemByValue("btnRemoveUser").Visible = value
        End Set
    End Property

    Public Property IsReadOnly As Boolean
        Get
            Return CType(ViewState("IsReadOnly"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            ViewState("IsReadOnly") = value
        End Set
    End Property

    Public Property ColumnInvitationTypeModVisible As Boolean
        Get
            Return dgvInvitatedUser.Columns.FindByUniqueName(COLUMN_INVITATION_TYPE_MODE).Visible
        End Get
        Set(ByVal value As Boolean)
            dgvInvitatedUser.Columns.FindByUniqueName(COLUMN_INVITATION_TYPE_MODE).Visible = value
        End Set
    End Property

    Public Property JsonContactAdded() As String
        Get
            Return CType(ViewState("_jsonContact"), String)
        End Get
        Set(ByVal value As String)
            ViewState("_jsonContact") = value
        End Set
    End Property

    Public ReadOnly Property JavascriptClosingFunction As String
        Get
            Return ID & "_CloseWindow"
        End Get
    End Property

    Private Property InvitatedUserDataSource As IList(Of DeskRoleUserResult)
        Get
            Return TryCast(Session("invitatedUserDataSource"), IList(Of DeskRoleUserResult))
        End Get
        Set(value As IList(Of DeskRoleUserResult))
            Session("invitatedUserDataSource") = value
        End Set
    End Property

    Public Property ColumnInvitationTypeVisible As Boolean
        Get
            Return dgvInvitatedUser.Columns.FindByUniqueName(COLUMN_INVITATION_MODE).Visible
        End Get
        Set(ByVal value As Boolean)
            dgvInvitatedUser.Columns.FindByUniqueName(COLUMN_INVITATION_MODE).Visible = value
        End Set
    End Property

    Private ReadOnly Property CurrentUserKeySelected As String
        Get
            Dim contactKey As String = String.Empty
            If dgvInvitatedUser.SelectedValue IsNot Nothing Then
                contactKey = dgvInvitatedUser.SelectedValue.ToString()
            End If
            Return contactKey
        End Get
    End Property

    Public Property RemovedUser As IList(Of DeskRoleUserResult)
        Get
            Dim removedUsers As IList(Of DeskRoleUserResult) = TryCast(ViewState("DeskRemovedUser"), IList(Of DeskRoleUserResult))
            If removedUsers Is Nothing Then
                ViewState("DeskRemovedUser") = New List(Of DeskRoleUserResult)
                Return DirectCast(ViewState("DeskRemovedUser"), IList(Of DeskRoleUserResult))
            End If
            Return removedUsers
        End Get
        Set(value As IList(Of DeskRoleUserResult))
            ViewState("DeskRemovedUser") = value
        End Set
    End Property

    Public Property AdminUser As DeskRoleUserResult
        Get
            Return DirectCast(ViewState("AdminUser"), DeskRoleUserResult)
        End Get
        Set(value As DeskRoleUserResult)
            ViewState("AdminUser") = value
        End Set
    End Property

    Public Property ButtonUserDeleteEnabled As Boolean
        Get
            _buttonUserDeleteEnabled = CType(ViewState("ButtonUserDeleteEnabled"), Boolean)
            Return If(_buttonUserDeleteEnabled.HasValue, _buttonUserDeleteEnabled.Value, False)
        End Get
        Set(value As Boolean)
            ViewState("ButtonUserDeleteEnabled") = value
        End Set

    End Property
#End Region

#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        ' SET NO CACHE
        Page.Response.Cache.SetAllowResponseInBrowserHistory(False)
        Page.Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Page.Response.Cache.SetNoStore()
        Page.Response.Cache.SetValidUntilExpires(True)

        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub
    Protected Sub ToolBar_ButtonClick(sender As Object, e As RadToolBarEventArgs) Handles ToolBar.ButtonClick
        Dim sourceControl As RadToolBarButton = DirectCast(e.Item, RadToolBarButton)
        Select Case sourceControl.CommandName
            Case "btnAddUser"
                windowSelContact.OnClientClose = JavascriptClosingFunction
                windowSelContact.Width = ProtocolEnv.ModalWidth
                windowSelContact.Height = ProtocolEnv.ModalHeight

                Dim queryString As String = String.Format("Type={0}&ParentID={1}&ExceptDeskUser={2}", ProtBasePage.DefaultType, ID, DocSuiteContext.Current.User.FullUserName)

                AjaxManager.ResponseScripts.Add(String.Format(OPEN_WINDOW_SCRIPT, ID, COMMON_SEL_CONTACT_DOMAIN_PATH, windowSelContact.ClientID, queryString))
            Case "btnRemoveUser"
                If String.IsNullOrEmpty(CurrentUserKeySelected) Then
                    BasePage.AjaxAlert("Nessun utente selezionato per la cancellazione")
                    Exit Sub
                End If

                Dim userDto As DeskRoleUserResult = InvitatedUserDataSource.FirstOrDefault(Function(x) x.SerializeKey.Eq(CurrentUserKeySelected))
                If userDto IsNot Nothing Then
                    If userDto.UserName.Eq(DocSuiteContext.Current.User.FullUserName) Then
                        BasePage.AjaxAlert("Non è possibile eliminare il proprio utente dal tavolo")
                        Exit Sub
                    End If
                    InvitatedUserDataSource.Remove(userDto)
                    'Aggiungo alla lista solo elementi che effettivamente erano salvati precedentemente
                    If userDto.IsSavedOnDesk Then
                        RemovedUser.Add(userDto)
                    End If
                End If

                dgvInvitatedUser.DataSource = InvitatedUserDataSource
                dgvInvitatedUser.DataBind()

                BtnRemoveUserVisible = False

                RaiseEvent UserDeleted(Me, New DeskRoleUserEventArgs() With {.DeskRoleUser = userDto})
        End Select
    End Sub

    Private Sub UscInvitationUser_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arguments As String() = Split(e.Argument, "|", 3)
        If Not arguments(0).Eq(ClientID) Then
            Exit Sub
        End If

        Select Case arguments(1)
            Case "AddDomain"
                Dim localArg As String = HttpUtility.HtmlDecode(arguments(2))
                JsonContactAdded = localArg
                Dim contact As Contact = JsonConvert.DeserializeObject(Of Contact)(JsonContactAdded)
                If AdminUser IsNot Nothing AndAlso AdminUser.UserName.Eq(contact.Code) Then
                    BasePage.AjaxAlert("L'utente selezionato è già proprietario del tavolo")
                    Exit Sub
                Else
                    SetUser(contact, localArg, False, DeskPermissionType.Reader, True)
                End If

                dgvInvitatedUser.DataSource = InvitatedUserDataSource
                dgvInvitatedUser.DataBind()
        End Select
    End Sub

    Protected Sub dgvInvitatedUser_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles dgvInvitatedUser.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim dto As DeskRoleUserResult = DirectCast(e.Item.DataItem, DeskRoleUserResult)

        DirectCast(e.Item.FindControl("lblUserName"), Label).Text = dto.Description

        With DirectCast(e.Item.FindControl("lblInvitationType"), Label)
            .Text = DeskPermissionType.Reader.GetDescription()
            If dto.PermissionType.HasValue Then
                .Text = dto.PermissionType.Value.GetDescription()
            End If
        End With

        Dim invitationTypeModControl As RadComboBox = DirectCast(e.Item.FindControl("ddlInvitationType"), RadComboBox)
        InitializeInvitationType(invitationTypeModControl, DeskPermissionType.Manage)
        InitializeInvitationType(invitationTypeModControl, DeskPermissionType.Reader)
        InitializeInvitationType(invitationTypeModControl, DeskPermissionType.Contributor)
        InitializeInvitationType(invitationTypeModControl, DeskPermissionType.Approval)

        invitationTypeModControl.SelectedValue = Convert.ToInt32(DeskPermissionType.Reader).ToString()
        If dto.PermissionType.HasValue Then
            invitationTypeModControl.SelectedValue = Convert.ToInt32(dto.PermissionType.Value).ToString()
        End If

        If IsReadOnly Then
            invitationTypeModControl.Enabled = False
        End If
    End Sub

    Protected Sub dgvInvitatedUser_ItemCommand(ByVal sender As Object, ByVal e As GridCommandEventArgs) Handles dgvInvitatedUser.ItemCommand
        Select Case e.CommandName
            Case "RowClick"
                BtnRemoveUserVisible = False
                If Not IsReadOnly Then
                    If InvitatedUserDataSource.Any(Function(x) x.SerializeKey.Eq(CurrentUserKeySelected)) Then
                        BtnRemoveUserVisible = True
                    End If
                End If
        End Select
    End Sub

    Protected Sub DdlInvitationType_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim selectedDdl As RadComboBox = DirectCast(sender, RadComboBox)
        Dim dataItem As GridDataItem = DirectCast(selectedDdl.NamingContainer, GridDataItem)

        Dim keySelected As String = dataItem.GetDataKeyValue("SerializeKey").ToString()
        Dim dto As DeskRoleUserResult = InvitatedUserDataSource.Single(Function(x) x.SerializeKey.Eq(keySelected))
        dto.PermissionType = CType([Enum].Parse(GetType(DeskPermissionType), selectedDdl.SelectedValue), DeskPermissionType)
        RaiseEvent UserChangeRole(Me, New DeskRoleUserEventArgs() With {.DeskRoleUser = dto, .FromAddDomain = False})
    End Sub
#End Region

#Region "Methods"
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf UscInvitationUser_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, dgvInvitatedUser, BasePage.MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, ToolBar)

        AjaxManager.AjaxSettings.AddAjaxSetting(dgvInvitatedUser, dgvInvitatedUser)
        AjaxManager.AjaxSettings.AddAjaxSetting(ToolBar, windowSelContact)
        AjaxManager.AjaxSettings.AddAjaxSetting(ToolBar, dgvInvitatedUser)
        AjaxManager.AjaxSettings.AddAjaxSetting(dgvInvitatedUser, ToolBar)
    End Sub


    Public Sub Refresh()
        Initialize()
        InitializeUserControlSource()
        'Resetto lo stato dei viewstate
        RemovedUser.Clear()
    End Sub

    Private Sub Initialize()
        BtnAddUserVisible = True
        BtnRemoveUserVisible = True
        If IsReadOnly Then
            BtnAddUserVisible = False
            BtnRemoveUserVisible = False
        End If

        InitializeColumns()
    End Sub

    ''' <summary>
    ''' Inizializza i dati dello user control per la corretta visualizzazione.
    ''' Da richiamare nell'inizializzazione della pagina.
    ''' </summary>
    Public Sub InitializeUserControlSource()
        InvitatedUserDataSource = Nothing

        AdminUser = Nothing
        If (BindAsyncEnable) Then
            dgvInvitatedUser.DataSource = String.Empty
            dgvInvitatedUser.DataBind()
        End If
    End Sub

    Private Sub InitializeInvitationType(ByRef radComboBox As RadComboBox, permissionType As DeskPermissionType)
        Dim item As RadComboBoxItem = New RadComboBoxItem()
        item.Text = permissionType.GetDescription()
        item.Value = Convert.ToInt32(permissionType).ToString()
        radComboBox.Items.Add(item)
    End Sub

    Private Sub InitializeColumns()
        Select Case Type
            Case "Modify"
                ColumnInvitationTypeModVisible = True
                ColumnInvitationTypeVisible = False
            Case Else
                ColumnInvitationTypeVisible = True
                ColumnInvitationTypeModVisible = False
        End Select
    End Sub

    Public Sub BindUser(roleUsers As IEnumerable(Of DeskRoleUser), isSavedOnDesk As Boolean)
        Dim contact As Contact
        Dim key As String
        For Each roleUser As DeskRoleUser In roleUsers
            contact = New Contact With {.ContactType = New ContactType(ContactType.Aoo)}
            contact.Code = roleUser.AccountName
            contact.Description = CommonAD.GetDisplayName(roleUser.AccountName)
            key = HttpUtility.HtmlEncode(JsonConvert.SerializeObject(contact))
            SetUser(contact, key, isSavedOnDesk, roleUser.PermissionType)
        Next

        dgvInvitatedUser.DataSource = InvitatedUserDataSource
        dgvInvitatedUser.DataBind()
    End Sub

    Private Sub SetUser(contact As Contact, key As String, isSavedOnDesk As Boolean, permission As DeskPermissionType, Optional ByVal fromAddDomain As Boolean? = False)
        Dim dto As DeskRoleUserResult = New DeskRoleUserResult(contact, key)
        dto.IsSavedOnDesk = isSavedOnDesk
        dto.PermissionType = permission
        SetUser(dto, fromAddDomain)
    End Sub

    Private Sub SetUser(userDto As DeskRoleUserResult, Optional ByVal fromAddDomain As Boolean? = False)
        If InvitatedUserDataSource Is Nothing Then
            InvitatedUserDataSource = New List(Of DeskRoleUserResult)
        End If

        If InvitatedUserDataSource.Any(Function(x) x.UserName.Eq(userDto.UserName)) Then
            BasePage.AjaxAlert("L'utente selezionato è già presente")
            Exit Sub
        End If

        'Verifico se l'utente è stato cancellato in precedenza
        If RemovedUser.Any(Function(x) x.UserName.Eq(userDto.UserName)) Then
            Dim toRemove As DeskRoleUserResult = RemovedUser.First(Function(x) x.UserName.Eq(userDto.UserName))
            userDto.IsSavedOnDesk = toRemove.IsSavedOnDesk
            userDto.IsChanged = True
            RemovedUser.Remove(toRemove)
        End If

        If (Not userDto.PermissionType.Equals(DeskPermissionType.Admin)) Then
            InvitatedUserDataSource.Add(userDto)
        Else
            AdminUser = userDto
        End If

        BtnRemoveUserVisible = False

        RaiseEvent UserAdded(Me, New DeskRoleUserEventArgs() With {.DeskRoleUser = userDto, .fromAddDomain = fromAddDomain})
    End Sub

    Public Function GetUsers() As IList(Of DeskRoleUserResult)
        If InvitatedUserDataSource Is Nothing Then
            Return New List(Of DeskRoleUserResult)()
        End If

        Dim users As IList(Of DeskRoleUserResult) = New List(Of DeskRoleUserResult)
        For Each item As GridDataItem In dgvInvitatedUser.Items
            Dim serializeKey As String = item.GetDataKeyValue("SerializeKey").ToString()
            Dim user As DeskRoleUserResult = InvitatedUserDataSource.SingleOrDefault(Function(s) s.SerializeKey.Eq(serializeKey))
            If user IsNot Nothing Then
                Dim invitationTypeModControl As RadComboBox = DirectCast(item.FindControl("ddlInvitationType"), RadComboBox)
                user.PermissionType = DirectCast([Enum].Parse(GetType(DeskPermissionType), invitationTypeModControl.SelectedValue), DeskPermissionType)
            End If
            users.Add(user)
        Next

        Return users
    End Function

    Public Function GetAddedUsers() As IList(Of DeskRoleUserResult)
        Return GetUsers().Where(Function(x) Not x.IsSavedOnDesk).ToList()
    End Function

    Public Function GetChangedUsers() As IList(Of DeskRoleUserResult)
        Return GetUsers().Where(Function(x) x.IsSavedOnDesk AndAlso x.IsChanged).ToList()
    End Function
#End Region
End Class