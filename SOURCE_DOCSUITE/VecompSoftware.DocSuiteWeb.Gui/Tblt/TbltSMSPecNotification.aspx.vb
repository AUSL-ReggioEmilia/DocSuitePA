Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Helpers
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.PECMails
Imports VecompSoftware.Helpers.ExtensionMethods
Imports Limilabs.Mail.Appointments
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.PECMails
Imports VecompSoftware.DocSuiteWeb.Data.Entity.PECMails
Imports VecompSoftware.DocSuiteWeb.Gui.WebComponent.Grid
Imports VecompSoftware.DocSuiteWeb.DTO.PECMails
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Web
Imports VecompSoftware.DocSuiteWeb.EntityMapper.PECMails

Public Class TbltSMSPecNotification
    Inherits ProtBasePage

#Region "Fields"
    Private Const PAGE_TITLE As String = "Tabella Notifiche SMS ricezione PEC"
    Private Const COMMON_SEL_CONTACT_DOMAIN_PATH As String = "../Comm/SelUsers.aspx"
    Private Const OPEN_WINDOW_SCRIPT As String = "return {0}_OpenWindow('{1}', '{2}', '{3}');"
    Private Const OPEN_SIMPLE_WINDOW_SCRIPT As String = "return {0}_SimpleOpenWindow('{1}');"
    Private _pecMailBoxUserFacade As PECMailBoxUserFacade
#End Region

#Region "Properties"
    Public ReadOnly Property JavascriptClosingFunction As String
        Get
            Return ID & "_CloseWindow"
        End Get
    End Property

    Public ReadOnly Property CurrentPECMailBoxUserFacade As PECMailBoxUserFacade
        Get
            If _pecMailBoxUserFacade Is Nothing Then
                _pecMailBoxUserFacade = New PECMailBoxUserFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _pecMailBoxUserFacade
        End Get
    End Property

    Public Property CurrentMailBoxSelected As Integer?
        Get
            If ViewState("IdMailBoxSelected") IsNot Nothing Then
                Return DirectCast(ViewState("IdMailBoxSelected"), Integer)
            End If
            Return Nothing
        End Get
        Set(value As Integer?)
            ViewState("IdMailBoxSelected") = value
        End Set
    End Property

    Private Property SetUserAfterUpdate As Boolean
        Get
            If ViewState("SetUserAfterChange") IsNot Nothing Then
                Return DirectCast(ViewState("SetUserAfterChange"), Boolean)
            End If
            Return False
        End Get
        Set(value As Boolean)
            ViewState("SetUserAfterChange") = value
        End Set
    End Property

    Private Property CurrentContactToUpdate As AccountModel
        Get
            If ViewState("CurrentContactToUpdate") IsNot Nothing Then
                Return DirectCast(ViewState("CurrentContactToUpdate"), AccountModel)
            End If
            Return Nothing
        End Get
        Set(value As AccountModel)
            ViewState("CurrentContactToUpdate") = value
        End Set
    End Property
#End Region

#Region "Events"
    Protected Sub ToolBar_ButtonClick(sender As Object, e As RadToolBarEventArgs) Handles ToolBar.ButtonClick
        Dim sourceControl As RadToolBarButton = DirectCast(e.Item, RadToolBarButton)
        Select Case sourceControl.CommandName
            Case "btnAddUser"
                windowSelContact.OnClientClose = JavascriptClosingFunction
                windowSelContact.Width = ProtocolEnv.ModalWidth
                windowSelContact.Height = ProtocolEnv.ModalHeight

                Dim queryString As String = String.Format("Type={0}&ParentID={1}&CheckEmailAddress=false", Type, ID)

                AjaxManager.ResponseScripts.Add(String.Format(OPEN_WINDOW_SCRIPT, ID, COMMON_SEL_CONTACT_DOMAIN_PATH, windowSelContact.ClientID, queryString))
        End Select
    End Sub


    Private Sub TbltSMSPecNotification_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arguments As String() = Split(e.Argument, "|", 3)
        If Not arguments(0).Eq(ClientID) Then
            Exit Sub
        End If

        Select Case arguments(1)
            Case "AddUser"
                If Not CurrentMailBoxSelected.HasValue Then
                    AjaxAlert("Nessuna casella PEC selezionata")
                    Exit Sub
                End If

                Dim localArg As String = HttpUtility.HtmlDecode(arguments(2))
                Dim contact As AccountModel = JsonConvert.DeserializeObject(Of AccountModel)(localArg)
                SetUser(contact)
        End Select
    End Sub

    Protected Sub dgvStoryBoard_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles dgvUserConfiguration.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim dto As PECMailBoxUserDto = DirectCast(e.Item.DataItem, PECMailBoxUserDto)

        With DirectCast(e.Item.FindControl("lblAccountName"), Label)
            If Not String.IsNullOrEmpty(dto.Account) Then
                .Text = CommonAD.GetDisplayName(dto.Account)
            Else
                .Text = CommonAD.GetDisplayName(dto.SecurityAccount)
            End If
        End With

        With DirectCast(e.Item.FindControl("lblMobilePhone"), Label)
            If Not String.IsNullOrEmpty(dto.Account) Then
                .Text = Facade.UserLogFacade.MobilePhoneOfUser(dto.Account, String.Empty)
            Else
                .Text = Facade.UserLogFacade.MobilePhoneOfUser(dto.SecurityAccount, String.Empty)
            End If
        End With

    End Sub
    Protected Sub PecRepeater_OnItemCommand(ByVal source As Object, ByVal e As RepeaterCommandEventArgs) Handles pecRepeater.ItemCommand
        Dim idMailBox As Integer = CType(e.CommandArgument, Integer)
        CurrentMailBoxSelected = idMailBox

        For Each item As RepeaterItem In pecRepeater.Items
            Dim linkBtn As LinkButton = TryCast(item.FindControl("pecLink"), LinkButton)
            If linkBtn IsNot Nothing Then
                linkBtn.CssClass = linkBtn.CssClass.Replace("selected", "").TrimEnd()
            End If
        Next

        Dim btn As LinkButton = DirectCast(e.CommandSource, LinkButton)
        btn.CssClass = String.Format("{0} selected", btn.CssClass)

        dgvUserConfiguration.Visible = True
        ToolBar.Visible = True

        BindPecUserData(idMailBox)
        AjaxManager.ResponseScripts.Add("responseEnd();")
    End Sub

    Protected Sub DgvUserConfiguration_ItemCommand(ByVal sender As Object, ByVal e As GridCommandEventArgs) Handles dgvUserConfiguration.ItemCommand
        Dim rowItem As GridDataItem = DirectCast(e.Item, GridDataItem)
        Dim itemId As Guid = DirectCast(rowItem.GetDataKeyValue("Id"), Guid)
        Dim mailBoxUser As PECMailBoxUser = CurrentPECMailBoxUserFacade.GetById(itemId)
        Select Case e.CommandName
            Case "DeleteUser"
                If mailBoxUser IsNot Nothing Then
                    CurrentPECMailBoxUserFacade.Delete(mailBoxUser)
                End If

                BindPecUserData(CurrentMailBoxSelected.Value)
                AjaxManager.ResponseScripts.Add("responseEnd();")
            Case "UpdateUser"
                WarningPanel.Visible = False
                Dim dto As New AccountModel()
                dto.Account = mailBoxUser.AccountName
                CurrentContactToUpdate = dto
                SetUserAfterUpdate = False
                txtMobilePhone.Text = String.Empty
                Dim userLog As UserLog = Facade.UserLogFacade.GetByUser(mailBoxUser.AccountName, String.Empty)
                If userLog IsNot Nothing Then
                    txtMobilePhone.Text = userLog.MobilePhone
                End If
                AjaxManager.ResponseScripts.Add(String.Format(OPEN_SIMPLE_WINDOW_SCRIPT, ID, windowAddMobilePhone.ClientID))
                AjaxManager.ResponseScripts.Add("responseEnd();")

        End Select
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        'Inizializzo delegati per la griglia
        dgvUserConfiguration = DelegateForGrid(Of PECMailBoxUser, PECMailBoxUserDto).Delegate(dgvUserConfiguration)
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub BtnSaveMobilePhone_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveMobilePhone.Click
        If Not txtMobilePhone.Text.All(Function(s) Char.IsDigit(s)) OrElse (txtMobilePhone.Text.Length < 6 OrElse txtMobilePhone.Text.Length > 15) Then
            AjaxAlert("Inserire un numero di cellulare valido")
            Exit Sub
        End If
        UpdateUserMobilePhone(CurrentContactToUpdate)

        If SetUserAfterUpdate Then
            SetUser(CurrentContactToUpdate)
        Else
            BindPecUserData(CurrentMailBoxSelected.Value)
        End If        
        Dim jsScript As String = String.Format("{0}_SimpleWindowClose()", ID)
        AjaxManager.ResponseScripts.Add(jsScript)
    End Sub
#End Region

#Region "Methods"
    Private Sub Initialize()
        If Not CommonShared.UserConnectedBelongsTo(ProtocolEnv.SMSConfigurationGroups) Then
            Throw New DocSuiteException("Utente non abilitato alla gestione delle notifiche SMS ricezione PEC")
        End If

        Title = PAGE_TITLE
        BindPecRepeater()

        dgvUserConfiguration.Visible = False
        ToolBar.Visible = False
    End Sub

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf TbltSMSPecNotification_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(ToolBar, windowSelContact)
        AjaxManager.AjaxSettings.AddAjaxSetting(pecRepeater, dgvUserConfiguration)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, dgvUserConfiguration)
        AjaxManager.AjaxSettings.AddAjaxSetting(pecRepeater, ToolBar)
        AjaxManager.AjaxSettings.AddAjaxSetting(pecRepeater, pecRepeater)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSaveMobilePhone, pnlEditorWindow, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSaveMobilePhone, dgvUserConfiguration)
        AjaxManager.AjaxSettings.AddAjaxSetting(dgvUserConfiguration, WarningPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(dgvUserConfiguration, txtMobilePhone)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, WarningPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, txtMobilePhone)

        AjaxManager.ClientEvents.OnResponseEnd = "responseEnd"
    End Sub

    Private Sub BindPecRepeater()
        Dim incomingPecBoxes As ICollection(Of PECMailBoxDto) = Facade.PECMailboxFacade.GetAllIncomingMailBox()
        pecRepeater.DataSource = incomingPecBoxes
        pecRepeater.DataBind()
    End Sub

    Private Sub BindPecUserData(idMailBox As Integer)
        Dim finder As PECMailBoxUserFinder = New PECMailBoxUserFinder(New MapperPECMailBoxUser())
        finder.IdPECMailBox = idMailBox
        dgvUserConfiguration.Finder = finder
        dgvUserConfiguration.CurrentPageIndex = 0
        dgvUserConfiguration.CustomPageIndex = 0

        dgvUserConfiguration.DataBindFinder(Of PECMailBoxUser, PECMailBoxUserDto)()
    End Sub

    Private Sub SetUser(contact As AccountModel)
        Dim userAlreadyExist As Boolean = CurrentPECMailBoxUserFacade.UserExist(contact.Account, CurrentMailBoxSelected.Value)
        If userAlreadyExist Then
            AjaxAlert("L'utente selezionato è già stato inserito")
            Exit Sub
        End If

        Dim mobilePhone As String = Facade.UserLogFacade.MobilePhoneOfUser(contact.Account, contact.Domain)
        If String.IsNullOrEmpty(mobilePhone) Then
            WarningPanel.Visible = True
            CurrentContactToUpdate = contact
            SetUserAfterUpdate = True
            txtMobilePhone.Text = String.Empty
            AjaxManager.ResponseScripts.Add(String.Format(OPEN_SIMPLE_WINDOW_SCRIPT, ID, windowAddMobilePhone.ClientID))
            Exit Sub
        End If

        Dim mailBox As PECMailBox = Facade.PECMailboxFacade.GetById(Convert.ToInt16(CurrentMailBoxSelected.Value))
        'todo: da gestire il salvataggio delle informazioni per utenti security user, attualmente viene registrato il solo account
        Dim boxUser As PECMailBoxUser = New PECMailBoxUser(DocSuiteContext.Current.User.FullUserName) With {.AccountName = contact.Account, .PECMailBox = mailBox}
        CurrentPECMailBoxUserFacade.Save(boxUser)


        BindPecUserData(CurrentMailBoxSelected.Value)
    End Sub

    Private Sub UpdateUserMobilePhone(contact As AccountModel)
        Dim item As UserLog = Facade.UserLogFacade.GetByUser(contact.Account, contact.Domain)
        'Se il contatto non è presente in UserLog in quanto non ha mai fatto un accesso in DSW
        'lo creo in modalità base (solo account name)
        If item Is Nothing Then
            item = New UserLog() With {.Id = contact.GetFullUserName()}
            Facade.UserLogFacade.Save(item)
        End If

        item.MobilePhone = txtMobilePhone.Text
        Facade.UserLogFacade.UpdateOnly(item)
    End Sub
#End Region

End Class