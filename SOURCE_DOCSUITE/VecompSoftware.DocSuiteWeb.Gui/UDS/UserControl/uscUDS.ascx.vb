Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.BusinessRule.Rules.Rights.PEC
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Tenants
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.UDS
Imports VecompSoftware.DocSuiteWeb.Model.DocumentGenerator
Imports VecompSoftware.DocSuiteWeb.Model.DocumentGenerator.Parameters
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.UDS
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports WebAPIUDS = VecompSoftware.DocSuiteWeb.Entity.UDS

Public Class uscUDS
    Inherits DocSuite2008BaseControl

#Region "Fields"
    Private _udsRepositoryFacade As UDSRepositoryFacade
    Private _udsUserFinder As UDSUserFinder
    Private authorizedUsers As ICollection(Of WebAPIUDS.UDSUser)
    Private _currentTenantFinder As TenantFinder
    Public Const ACTION_TYPE_INSERT As String = "Insert"
    Public Const ACTION_TYPE_EDIT As String = "Edit"
    Public Const ACTION_TYPE_VIEW As String = "View"
    Public Const ACTION_TYPE_SEARCH As String = "Search"
    Public Const ACTION_TYPE_AUTHORIZE As String = "Authorize"
    Public Const ACTION_TYPE_DUPLICATE As String = "Duplicate"
    Public Const YEAR_LABEL_FORMAT As String = "Anno: {0}"
    Public Const NUMBER_LABEL_FORMAT As String = "Numero: {0:0000000}"
    Public Const REGDATE_LABEL_FORMAT As String = "Data registrazione: {0:dd/MM/yyyy}"
    Public Const PAGE_PROTOCOL_SUMMARY As String = "~/Prot/ProtVisualizza.aspx?Year={0}&Number={1}"
#End Region

#Region "Odata query"
    Private Const ODATA_DOCUMENTUNIT_FILTER As String = "DocumentUnit/DocumentUnitRoles/any(r:r/{0} in ({1}))"
    Private Const ODATA_CONTACT_FILTER As String = "Contacts/any(contacts:contacts/{0} eq {1} and contacts/ContactLabel eq '{2}')"
    Private Const ODATA_CONTACT_MANUAL_FILTER As String = "(Contacts/any(c:contains(c/{0},'{1}') and c/ContactLabel eq '{2}') or Contacts/any(c1:contains(c1/Contact/Description,'{1}') and c1/ContactLabel eq '{2}'))"
#End Region

#Region "Properties"
    Public Event NeedRepositorySource(ByVal sender As Object, ByVal e As EventArgs)
    Public Event UDSIndexChanged(ByVal sender As Object, ByVal e As RadComboBoxSelectedIndexChangedEventArgs)

    Public ReadOnly Property UDSRepositoryFacade As UDSRepositoryFacade
        Get
            If _udsRepositoryFacade Is Nothing Then
                _udsRepositoryFacade = New UDSRepositoryFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _udsRepositoryFacade
        End Get
    End Property

    Public ReadOnly Property CurrentStaticDataControl As IUDSStaticData
        Get
            If ActionType.Eq(ACTION_TYPE_SEARCH) Then
                Return udsDataFinder
            End If
            Return DirectCast(udsDataInsert, IUDSStaticData)
        End Get
    End Property

    Public ReadOnly Property UDSUserFinder As UDSUserFinder
        Get
            If _udsUserFinder Is Nothing Then
                _udsUserFinder = New UDSUserFinder(DocSuiteContext.Current.Tenants)
            End If
            Return _udsUserFinder
        End Get
    End Property

    Public ReadOnly Property AuhtorizedUsers As ICollection(Of WebAPIUDS.UDSUser)
        Get
            If authorizedUsers Is Nothing Then
                authorizedUsers = New List(Of WebAPIUDS.UDSUser)
                UDSUserFinder.ResetDecoration()
                UDSUserFinder.IdUDS = UDSId.Value
                UDSUserFinder.EnablePaging = False
                Dim result As ICollection(Of WebAPIDto(Of WebAPIUDS.UDSUser)) = UDSUserFinder.DoSearch()
                If result IsNot Nothing AndAlso result.Count() > 0 Then
                    authorizedUsers = result.Select(Function(x) x.Entity).ToList()
                End If
            End If
            Return authorizedUsers
        End Get
    End Property

    ''' <summary>
    ''' Recupera il modello del repository corrente partendo dal relativo XML
    ''' </summary>                            
    ''' <remarks>Contiene solamente lo schema dei dati da visualizzare/gestire</remarks>
    Public Property UDSItemSource As UDSModel
        Get
            If ViewState(String.Format("{0}_UDSItemSource", ID)) Is Nothing Then
                Return Nothing
            End If
            Dim jsonUdsXml As String = DirectCast(ViewState(String.Format("{0}_UDSItemSource", ID)), String)
            Return UDSModel.LoadXml(jsonUdsXml)
        End Get
        Set(value As UDSModel)
            ViewState(String.Format("{0}_UDSItemSource", ID)) = value.SerializeToXml()
        End Set
    End Property

    Public Property CurrentUDSRepositoryId As Guid?
        Get
            If ViewState(String.Format("{0}_CurrentUDSRepositoryId", ID)) Is Nothing Then
                Return Nothing
            End If
            Return DirectCast(ViewState(String.Format("{0}_CurrentUDSRepositoryId", ID)), Guid)
        End Get
        Set(value As Guid?)
            ViewState(String.Format("{0}_CurrentUDSRepositoryId", ID)) = value
        End Set
    End Property

    Public Property ActionType As String
        Get
            If ViewState(String.Format("{0}_ActionType", ID)) Is Nothing Then
                Return Nothing
            End If
            Return DirectCast(ViewState(String.Format("{0}_ActionType", ID)), String)
        End Get
        Set(value As String)
            ViewState(String.Format("{0}_ActionType", ID)) = value
        End Set
    End Property

    Public Property UDSId As Guid?
        Get
            If ViewState(String.Format("{0}_UDSId", ID)) Is Nothing Then
                Return Nothing
            End If
            Return Guid.Parse(ViewState(String.Format("{0}_UDSId", ID)).ToString())
        End Get
        Set(value As Guid?)
            ViewState(String.Format("{0}_UDSId", ID)) = value
        End Set
    End Property


    Public Property UDSYear As Integer?
        Get
            If ViewState(String.Format("{0}_UDSYear", ID)) Is Nothing Then
                Return Nothing
            End If
            Return Integer.Parse(ViewState(String.Format("{0}_UDSYear", ID)).ToString())
        End Get
        Set(value As Integer?)
            ViewState(String.Format("{0}_UDSYear", ID)) = value
        End Set
    End Property

    Public Property UDSNumber As Integer?
        Get
            If ViewState(String.Format("{0}_UDSNumber", ID)) Is Nothing Then
                Return Nothing
            End If
            Return Integer.Parse(ViewState(String.Format("{0}_UDSNumber", ID)).ToString())
        End Get
        Set(value As Integer?)
            ViewState(String.Format("{0}_UDSNumber", ID)) = value
        End Set
    End Property

    Public Property UDSRegistrationDate As DateTimeOffset?
        Get
            If ViewState(String.Format("{0}_UDSRegistrationDate", ID)) Is Nothing Then
                Return Nothing
            End If
            Return DateTimeOffset.Parse(ViewState(String.Format("{0}_UDSRegistrationDate", ID)).ToString())
        End Get
        Set(value As DateTimeOffset?)
            ViewState(String.Format("{0}_UDSRegistrationDate", ID)) = value
        End Set
    End Property

    Public Property UDSRegistrationUser As String
        Get
            If ViewState(String.Format("{0}_UDSRegistrationUser", ID)) Is Nothing Then
                Return String.Empty
            End If
            Return ViewState(String.Format("{0}_UDSRegistrationUser", ID)).ToString()
        End Get
        Set(value As String)
            ViewState(String.Format("{0}_UDSRegistrationUser", ID)) = value
        End Set
    End Property

    Public Property UDSLastChangedDate As DateTimeOffset?
        Get
            If ViewState(String.Format("{0}_UDSLastChangedDate", ID)) Is Nothing Then
                Return Nothing
            End If
            Return DateTimeOffset.Parse(ViewState(String.Format("{0}_UDSLastChangedDate", ID)).ToString())
        End Get
        Set(value As DateTimeOffset?)
            ViewState(String.Format("{0}_UDSLastChangedDate", ID)) = value
        End Set
    End Property

    Public Property UDSLastChangedUser As String
        Get
            If ViewState(String.Format("{0}_UDSLastChangedUser", ID)) Is Nothing Then
                Return String.Empty
            End If
            Return ViewState(String.Format("{0}_UDSLastChangedUser", ID)).ToString()
        End Get
        Set(value As String)
            ViewState(String.Format("{0}_UDSLastChangedUser", ID)) = value
        End Set
    End Property
    Public Property UDSSubject As String
        Get
            If ViewState(String.Format("{0}_UDSSubject", ID)) Is Nothing Then
                Return String.Empty
            End If
            Return ViewState(String.Format("{0}_UDSSubject", ID)).ToString()
        End Get
        Set(value As String)
            ViewState(String.Format("{0}_UDSSubject", ID)) = value
        End Set
    End Property
    Public Property UDSStatus As Short?
        Get
            If ViewState(String.Format("{0}_UDSStatus", ID)) Is Nothing Then
                Return Nothing
            End If
            Return Short.Parse(ViewState(String.Format("{0}_UDSStatus", ID)).ToString())
        End Get
        Set(value As Short?)
            ViewState(String.Format("{0}_UDSStatus", ID)) = value
        End Set
    End Property
    Public Property UDSCancelMotivation As String
        Get
            If ViewState(String.Format("{0}_UDSCancelMotivation", ID)) Is Nothing Then
                Return String.Empty
            End If
            Return ViewState(String.Format("{0}_UDSCancelMotivation", ID)).ToString()
        End Get
        Set(value As String)
            ViewState(String.Format("{0}_UDSCancelMotivation", ID)) = value
        End Set
    End Property
    Public Property UDSCategory As UDSEntityCategoryDto
        Get
            If ViewState(String.Format("{0}_UDSCategory", ID)) Is Nothing Then
                Return Nothing
            End If
            Return DirectCast(ViewState(String.Format("{0}_UDSCategory", ID)), UDSEntityCategoryDto)
        End Get
        Set(value As UDSEntityCategoryDto)
            ViewState(String.Format("{0}_UDSCategory", ID)) = value
        End Set
    End Property

    Public Property UDSContainer As UDSEntityContainerDto
        Get
            If ViewState(String.Format("{0}_UDSContainer", ID)) Is Nothing Then
                Return Nothing
            End If
            Return DirectCast(ViewState(String.Format("{0}_UDSContainer", ID)), UDSEntityContainerDto)
        End Get
        Set(value As UDSEntityContainerDto)
            ViewState(String.Format("{0}_UDSContainer", ID)) = value
        End Set
    End Property
    Public Property UDSAuthorizations As IList(Of UDSEntityRoleDto)
        Get
            If ViewState(String.Format("{0}_UDSAuthorizations", ID)) Is Nothing Then
                Return New List(Of UDSEntityRoleDto)
            End If
            Return DirectCast(ViewState(String.Format("{0}_UDSAuthorizations", ID)), IList(Of UDSEntityRoleDto))
        End Get
        Set(value As IList(Of UDSEntityRoleDto))
            ViewState(String.Format("{0}_UDSAuthorizations", ID)) = value
        End Set
    End Property

    Public Property UDSMessages As IList(Of UDSEntityMessageDto)
        Get
            If ViewState(String.Format("{0}_UDSMessages", ID)) Is Nothing Then
                Return New List(Of UDSEntityMessageDto)
            End If
            Return DirectCast(ViewState(String.Format("{0}_UDSMessages", ID)), IList(Of UDSEntityMessageDto))
        End Get
        Set(value As IList(Of UDSEntityMessageDto))
            ViewState(String.Format("{0}_UDSMessages", ID)) = value
        End Set
    End Property

    Public Property UDSPecMails As IList(Of UDSEntityPECMailDto)
        Get
            If ViewState(String.Format("{0}_UDSPecMails", ID)) Is Nothing Then
                Return New List(Of UDSEntityPECMailDto)
            End If
            Return DirectCast(ViewState(String.Format("{0}_UDSPecMails", ID)), IList(Of UDSEntityPECMailDto))
        End Get
        Set(value As IList(Of UDSEntityPECMailDto))
            ViewState(String.Format("{0}_UDSPecMails", ID)) = value
        End Set
    End Property

    Public Property WorkflowSignedDocRequired As IDictionary(Of String, Boolean)
        Get
            If ViewState(String.Format("{0}_WorkflowSignedDocRequired", ID)) IsNot Nothing Then
                Return DirectCast(ViewState(String.Format("{0}_WorkflowSignedDocRequired", ID)), IDictionary(Of String, Boolean))
            End If
            Return New Dictionary(Of String, Boolean)
        End Get
        Set(ByVal value As IDictionary(Of String, Boolean))
            ViewState(String.Format("{0}_WorkflowSignedDocRequired", ID)) = value
        End Set
    End Property

    Public ReadOnly Property FromUDSLink() As Boolean
        Get
            Return Request.QueryString("fromUDSLink") IsNot Nothing
        End Get
    End Property

    Public Property CopyToPEC As Boolean?
        Get
            If ViewState(String.Format("{0}_CopyToPEC", ID)) IsNot Nothing Then
                Return DirectCast(ViewState(String.Format("{0}_CopyToPEC", ID)), Boolean)
            End If
            Return Nothing
        End Get
        Set(ByVal value As Boolean?)
            ViewState(String.Format("{0}_CopyToPEC", ID)) = value
        End Set
    End Property


    'TODO: Disabilito di default la visibilità del pannello parer in quanto non ancora gestito
    Public Property HideParerPanel As Boolean = True

    Public Property CurrentUDSTypologyId As Guid?

    Public ReadOnly Property uscDataFinder As uscUDSStaticDataFinder
        Get
            Return udsDataFinder
        End Get
    End Property

    Public ReadOnly Property DropDownListUDS As RadComboBox
        Get
            Return ddlUds
        End Get
    End Property

    Public ReadOnly Property DropDownListTypology As RadComboBox
        Get
            Return ddlTypology
        End Get
    End Property

    Public Property SessionIsEmpty As Boolean
#End Region

#Region "Events"

    Protected Sub ddlUds_ItemsChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs) Handles ddlUds.SelectedIndexChanged
        RaiseEvent UDSIndexChanged(Me, e)
        If String.IsNullOrEmpty(e.Value) Then
            If ActionType.Eq(ACTION_TYPE_SEARCH) Then
                rowDynamicData.SetDisplay(False)
                ToggleFinderBorderStyle(False)
            Else
                SetRowControlVisibile(False)
            End If
            Exit Sub
        End If

        CurrentUDSRepositoryId = Guid.Parse(e.Value)
        Dim repository As UDSRepository = UDSRepositoryFacade.GetById(CurrentUDSRepositoryId.Value)
        Dim schemaRepository As UDSModel = UDSModel.LoadXml(repository.ModuleXML)
        SetRowControlVisibile(True)
        rowUDSInfo.SetDisplay(False)

        If ActionType.Eq(ACTION_TYPE_SEARCH) Then
            ToggleFinderBorderStyle(True)
        End If

        If ActionType.Eq(ACTION_TYPE_INSERT) Then
            CurrentStaticDataControl.SetData(schemaRepository)
        End If

        udsDataFinder.SetUDSBehaviour(schemaRepository)


        udsDynamicControls.ResetState()
        udsDynamicControls.LoadDynamicControls(schemaRepository.Model, True)
        udsDynamicControls.InitializeDynamicFilters()
        udsDynamicControls.LoadDefaultData()

        If (FromUDSLink) Then
            AjaxManager.ResponseScripts.Add(String.Concat("UDSRepositoryOnChange('", CurrentUDSRepositoryId, "');"))
        End If
    End Sub

    Protected Sub ddlUds_ItemsRequested(sender As Object, e As RadComboBoxItemsRequestedEventArgs) Handles ddlUds.ItemsRequested
        Dim repositories As IList(Of WebAPIUDS.UDSRepository)
        Dim finder As UDSRepositoryFinder = New UDSRepositoryFinder(DocSuiteContext.Current.Tenants)
        If ActionType.Eq(ACTION_TYPE_SEARCH) Then
            finder.ActionType = UDSRepositoryFinderActionType.Search
        Else
            finder.ActionType = UDSRepositoryFinderActionType.Insert
        End If
        finder.PECAnnexedEnabled = CopyToPEC.HasValue AndAlso CopyToPEC.Value
        finder.UserName = DocSuiteContext.Current.User.UserName
        finder.Domain = DocSuiteContext.Current.User.Domain
        finder.SortExpressions.Add("Entity.Name", "ASC")
        If rowTypology.Visible AndAlso Not String.IsNullOrEmpty(e.Message) Then
            finder.IdUDSTypology = Guid.Parse(e.Message)
        End If
        finder.EnablePaging = False
        Dim results As ICollection(Of WebAPIDto(Of WebAPIUDS.UDSRepository)) = finder.DoSearch()

        If results IsNot Nothing Then
            repositories = results.Select(Function(r) r.Entity).ToList()
            If ProtocolEnv.MultiTenantEnabled Then
                If BasePage.CurrentTenant IsNot Nothing AndAlso BasePage.CurrentTenant.Containers IsNot Nothing Then
                    Dim fullFinder As UDSRepositoryFinder
                    Dim fullRepository As WebAPIUDS.UDSRepository
                    For Each repository As WebAPIUDS.UDSRepository In repositories.ToList()
                        fullFinder = New UDSRepositoryFinder(DocSuiteContext.Current.Tenants) With {
                            .EnablePaging = False,
                            .ActionType = UDSRepositoryFinderActionType.FindElement,
                            .UniqueId = repository.UniqueId,
                            .ExpandProperties = True
                        }
                        fullRepository = fullFinder.DoSearch().Select(Function(s) s.Entity).First()
                        If (Not BasePage.CurrentTenant.Containers.Any(Function(x) x.EntityShortId = fullRepository.Container.EntityShortId)) Then
                            repositories.Remove(repository)
                        End If
                    Next
                Else
                    repositories = New List(Of WebAPIUDS.UDSRepository)
                End If
            End If

            ddlUds.Items.Clear()
            ddlUds.Items.Add(New RadComboBoxItem(String.Empty, String.Empty))
            For Each repository As WebAPIUDS.UDSRepository In repositories
                ddlUds.Items.Add(New RadComboBoxItem(repository.Name, repository.UniqueId.ToString()))
            Next

            InitializeCombos()

            If ddlUds.Items.Count = 2 Then
                ddlUds.SelectedIndex = 1
                Call ddlUds_ItemsChanged(sender, New RadComboBoxSelectedIndexChangedEventArgs(ddlUds.SelectedItem.Text, String.Empty, ddlUds.SelectedItem.Value, String.Empty))
            End If
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub rptUDSMessage_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptUDSMessage.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim item As MessageEmail = DirectCast(e.Item.DataItem, MessageEmail)

            With DirectCast(e.Item.FindControl("lblUDSMessageUser"), Label)
                .Text = CommonAD.GetDisplayName(item.Message.RegistrationUser)
            End With

            With DirectCast(e.Item.FindControl("lblUDSMessageDate"), Label)
                .Text = If(item.SentDate.HasValue, item.SentDate.Value.ToString(), String.Empty)
            End With

            With DirectCast(e.Item.FindControl("lblUDSMessageSender"), Label)
                .Text = Server.HtmlEncode(item.GetSender().ToString())
            End With

            With DirectCast(e.Item.FindControl("lblUDSMessageRecipient"), Label)
                .Text = Server.HtmlEncode(String.Join("; ", item.GetRecipients))
            End With

            With DirectCast(e.Item.FindControl("lblUDSMessageSubject"), HtmlAnchor)
                .InnerText = item.Subject
                If (item.EmlDocumentId.HasValue OrElse item.Message.Attachments.Count > 0) Then
                    .HRef = String.Format("{0}?Type=Prot&MessageEmailId={1}", ResolveUrl("~/Prot/MessageEmailView.aspx"), item.Id)
                Else
                    .Attributes.Add("disabled", "disabled")
                    .Attributes.Remove("onclick")
                End If
            End With

            With DirectCast(e.Item.FindControl("imgUDSMessageStatus"), Image)
                .AlternateText = Server.HtmlEncode(item.Message.Status.GetDescription())
                .ImageUrl = ImagePath.GetMessageStatusIconPath(item.Message.Status)
            End With

            With DirectCast(e.Item.FindControl("lblUDSMessageStatus"), Label)
                .Text = Server.HtmlEncode(item.Message.Status.GetDescription())
            End With

        End If
    End Sub

    Protected Sub rptOutgoingPEC_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptOutgoingPEC.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim item As PECMail = DirectCast(e.Item.DataItem, PECMail)

            Dim HtmlAncorColor As String = String.Empty
            Dim Title As String = String.Empty

            If (item.IsActive = 3) Then
                HtmlAncorColor = "orange"
                Title = "PEC in elaborazione"
            End If
            If (item.IsActive = 255) Then
                HtmlAncorColor = "red"
                Title = "PEC in errore"
            End If

            With DirectCast(e.Item.FindControl("lblPECOutgoingDate"), Label)
                .Text = If(item.MailDate.HasValue, item.MailDate.Value.ToString(), "")
            End With

            With DirectCast(e.Item.FindControl("lblPECOutgoingMittente"), Label)
                .Text = Server.HtmlEncode(item.MailSenders)
            End With

            With DirectCast(e.Item.FindControl("lblPECOutgoingSender"), Label)
                .Text = Server.HtmlEncode(item.MailRecipients)
            End With

            With DirectCast(e.Item.FindControl("aOutgoing"), HtmlAnchor)
                .InnerText = If(String.IsNullOrEmpty(item.MailSubject), "<senza oggetto>", item.MailSubject)
                .HRef = String.Format("{0}?Type=Pec&PECId={1}", ResolveUrl("~/PEC/PECSummary.aspx"), item.Id)
                .Style.Add("color", HtmlAncorColor)
                .Title = Title
            End With

            With DirectCast(e.Item.FindControl("uscPecHistory"), uscPecHistory)
                .PecHistory = Facade.PECMailFacade.GetOutgoingMailHistory(item.Id)
                .BindData()
            End With
        End If
    End Sub

    Protected Sub rptIngoingPEC_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptIngoingPEC.ItemDataBound
        If e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim item As PECMail = DirectCast(e.Item.DataItem, PECMail)

        With DirectCast(e.Item.FindControl("lblPECIngoingDate"), Label)
            .Text = item.MailDate.ToString()
        End With

        With DirectCast(e.Item.FindControl("lblPECIngoingSender"), Label)
            .Text = item.MailSenders
        End With

        Dim pecRights As PECMailRightsUtil = New PECMailRightsUtil(item, DocSuiteContext.Current.User.FullUserName)
        With DirectCast(e.Item.FindControl("aIncoming"), HtmlAnchor)
            .InnerText = If(String.IsNullOrEmpty(item.MailSubject), "<senza oggetto>", item.MailSubject)
            .HRef = String.Format("{0}?Type=Pec&PECId={1}&ProtocolBox={2}", ResolveUrl("~/PEC/PECSummary.aspx"), item.Id, item.MailBox.IsProtocolBox.HasValue AndAlso item.MailBox.IsProtocolBox.Value)
        End With
    End Sub

    Private Sub btnExpandIngoingPec_Click(sender As Object, e As ImageClickEventArgs) Handles btnExpandIngoingPec.Click
        RepeaterExpander(btnExpandIngoingPec, rptIngoingPEC)
    End Sub

    Private Sub btnExpandUDSMessage_Click(sender As Object, e As ImageClickEventArgs) Handles btnExpandUDSMessage.Click
        RepeaterExpander(btnExpandUDSMessage, rptUDSMessage)
    End Sub

    Private Sub btnExpandOutgoingPec_Click(sender As Object, e As ImageClickEventArgs) Handles btnExpandOutgoingPec.Click
        RepeaterExpander(btnExpandOutgoingPec, rptOutgoingPEC)
    End Sub

    Protected Sub chkShowOtherStatusPec_changed(ByVal sender As Object, e As EventArgs) Handles chkShowOtherStatusPec.CheckedChanged
        InitializePecPanel()
    End Sub

    Protected Sub ddlTypology_ItemsChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs) Handles ddlTypology.SelectedIndexChanged
        Call ddlUds_ItemsChanged(Me, New RadComboBoxSelectedIndexChangedEventArgs(Nothing, Nothing, Nothing, Nothing))
        RaiseEvent UDSIndexChanged(Me, New RadComboBoxSelectedIndexChangedEventArgs(Nothing, Nothing, Nothing, Nothing))

        Dim selectedTypologyEvent As RadComboBoxItemsRequestedEventArgs = New RadComboBoxItemsRequestedEventArgs()
        selectedTypologyEvent.Message = e.Value
        Call ddlUds_ItemsRequested(Me, selectedTypologyEvent)
    End Sub

#End Region

#Region "Methods"

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlUds, uscContent, BasePage.MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlUds, udsDataFinder)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlUds, udsDataInsert)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlUds, rowDynamicData)
        AjaxManager.AjaxSettings.AddAjaxSetting(chkShowOtherStatusPec, tblOutgoingPEC)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnExpandOutgoingPec, tblOutgoingPEC)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnExpandUDSMessage, tblUDSMessage)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnExpandIngoingPec, tblUDSMessage)
    End Sub

    Private Sub Initialize()
        udsDynamicControls.IsReadOnly = False
        rowTypology.Visible = False
        Select Case ActionType
            Case ACTION_TYPE_INSERT, ACTION_TYPE_DUPLICATE
                SetRowControlVisibile(False)
                InitializeInsertAction()

            Case ACTION_TYPE_EDIT
                SetRowControlVisibile(True)
                InitializeEditAction()

            Case ACTION_TYPE_VIEW
                SetRowControlVisibile(True)
                InitializeViewAction()

            Case ACTION_TYPE_SEARCH
                SetRowControlVisibile(True)
                InitializeSearchAction()

            Case ACTION_TYPE_AUTHORIZE
                SetRowControlVisibile(False)
                InitializeAuthorizeAction()
        End Select
    End Sub

    Public Sub SetRowControlVisibile(visible As Boolean)
        rowData.SetDisplay(visible)
        rowUDSInfo.SetDisplay(visible)
        rowDynamicData.SetDisplay(visible)
    End Sub

    Public Sub SetDeletedState(cancelMotivation As String)
        lblCancelMotivation.Text = cancelMotivation
        tblAnnullamento.Visible = True
    End Sub

    Private Sub InitializeAction()
        lblDddlUds.Visible = False
        ddlUds.Visible = False
        udsDataFinder.Visible = False
        udsDataInsert.Visible = False
    End Sub

    Public Sub InitializeInsertAction()
        InitializeAction()

        rowTypology.Visible = True
        InitializeUDSTypology()

        Call ddlUds_ItemsRequested(Me, New RadComboBoxItemsRequestedEventArgs())

        rowUDSArchive.SetDisplay(True)
        udsDataInsert.Visible = True
        udsDataInsert.ActionType = ACTION_TYPE_INSERT
        If ActionType.Eq(ACTION_TYPE_DUPLICATE) Then
            udsDataInsert.ActionType = ACTION_TYPE_DUPLICATE
        End If
        CurrentStaticDataControl.ResetControls()

        ddlUds.Visible = True
        CurrentStaticDataControl.InitializeControls()

        udsDynamicControls.ActionType = ACTION_TYPE_INSERT
        If ActionType.Eq(ACTION_TYPE_DUPLICATE) Then
            udsDynamicControls.ActionType = ACTION_TYPE_DUPLICATE
        End If
        udsDynamicControls.WorkflowSignedDocRequired = WorkflowSignedDocRequired

        If (Request.QueryString("ArchiveTypeId") IsNot Nothing) Then
            CurrentUDSRepositoryId = Guid.Parse(Request.QueryString("ArchiveTypeId").ToString())
        End If

        If CurrentUDSRepositoryId IsNot Nothing Then
            ddlUds.SelectedValue = CurrentUDSRepositoryId.ToString()
            ddlUds.Enabled = False
            Call ddlUds_ItemsChanged(Me, New RadComboBoxSelectedIndexChangedEventArgs(String.Empty, String.Empty, CurrentUDSRepositoryId.ToString(), String.Empty))
        End If

        If UDSItemSource IsNot Nothing Then
            RepositoryBind(False)
        End If
    End Sub

    Public Sub InitializeEditAction()
        InitializeAction()

        udsDataInsert.Visible = True
        udsDataInsert.ActionType = ACTION_TYPE_EDIT
        CurrentStaticDataControl.ResetControls()

        lblDddlUds.Visible = True
        CurrentStaticDataControl.InitializeControls()
        rowUDSArchive.SetDisplay(False)
        rowUDSInfo.SetDisplay(True)

        udsDynamicControls.ActionType = ACTION_TYPE_EDIT
        udsDynamicControls.WorkflowSignedDocRequired = WorkflowSignedDocRequired
        udsDynamicControls.ViewAuthorizations = False

        If UDSItemSource Is Nothing Then
            RaiseEvent NeedRepositorySource(Me, New EventArgs())
            Exit Sub
        End If

        RepositoryBind()
    End Sub

    Public Sub InitializeSearchAction()
        InitializeAction()

        rowTypology.Visible = True
        InitializeUDSTypology()
        Call ddlUds_ItemsRequested(Me, New RadComboBoxItemsRequestedEventArgs())

        udsDynamicControls.ActionType = ACTION_TYPE_SEARCH
        udsDynamicControls.ViewDocuments = False

        ddlUds.Visible = True

        udsDataFinder.Visible = True
        rowUDSArchive.SetDisplay(True)
        rowUDSInfo.SetDisplay(False)
        rowDynamicData.SetDisplay(ddlUds.Items.Count = 2)

        If CurrentUDSTypologyId.HasValue AndAlso CurrentUDSTypologyId.Value <> Guid.Empty Then
            ddlTypology.SelectedValue = CurrentUDSTypologyId.Value.ToString()
            ddlTypology.Enabled = False
        End If

        If CurrentUDSRepositoryId.HasValue AndAlso CurrentUDSRepositoryId.Value <> Guid.Empty Then
            ddlUds.SelectedValue = CurrentUDSRepositoryId.Value.ToString()
            Call ddlUds_ItemsChanged(Me, New RadComboBoxSelectedIndexChangedEventArgs(String.Empty, String.Empty, CurrentUDSRepositoryId.ToString(), String.Empty))
        End If

    End Sub

    Public Sub InitializeAuthorizeAction()
        InitializeAction()

        rowUDSArchive.SetDisplay(False)
        rowUDSInfo.SetDisplay(True)
        tblAnnullamento.Visible = False
        tblParer.Visible = ProtocolEnv.ParerEnabled AndAlso Not HideParerPanel
        lblDddlUds.Visible = True
        tblSubject.Visible = False
        udsDynamicContactsControl.Visible = False
        RepositoryBind(False)

    End Sub

    Public Sub InitializeViewAction()
        InitializeAction()

        rowUDSArchive.SetDisplay(False)
        rowUDSContacts.Visible = True
        tblClassificazione.Visible = True
        tblParer.Visible = ProtocolEnv.ParerEnabled AndAlso Not HideParerPanel
        udsDataInsert.Visible = True
        udsDataInsert.ActionType = ACTION_TYPE_VIEW
        CurrentStaticDataControl.ResetControls()

        udsDynamicContactsControl.ActionType = ACTION_TYPE_VIEW
        udsDynamicContactsControl.IsReadOnly = True
        udsDynamicContactsControl.ViewMetadata = False
        udsDynamicContactsControl.ViewAuthorizations = False
        udsDynamicContactsControl.ViewDocuments = False

        udsDynamicAuthorizationsControl.ActionType = ACTION_TYPE_VIEW
        udsDynamicAuthorizationsControl.IsReadOnly = True
        udsDynamicAuthorizationsControl.ViewMetadata = False
        udsDynamicAuthorizationsControl.ViewContacts = False
        udsDynamicAuthorizationsControl.ViewDocuments = False

        udsDynamicControls.ActionType = ACTION_TYPE_VIEW
        udsDynamicControls.IsReadOnly = True
        udsDynamicControls.ViewContacts = False
        udsDynamicControls.ViewAuthorizations = False

        lblDddlUds.Visible = True
        CurrentStaticDataControl.InitializeControls()
        DirectCast(CurrentStaticDataControl, IUDSInsertStaticData).RegistrationDate = UDSRegistrationDate

        If UDSItemSource Is Nothing Then
            RaiseEvent NeedRepositorySource(Me, New EventArgs())
            If UDSItemSource IsNot Nothing AndAlso UDSItemSource.Model IsNot Nothing AndAlso UDSItemSource.Model.WorkflowEnabled Then
                Dim metadata As List(Of BaseDocumentGeneratorParameter) = New List(Of BaseDocumentGeneratorParameter)()
                For Each item As FieldBaseType In UDSItemSource.Model.Metadata.SelectMany(Function(f) f.Items)
                    If TypeOf item Is TextField Then
                        'str = New StringParameter(item.ColumnName, DirectCast(item, TextField).Value)
                        'str.IsHtmlValue = DirectCast(item, TextField).HTMLEnable
                        metadata.Add(New StringParameter(item.ColumnName, Server.HtmlEncode(DirectCast(item, TextField).Value)))
                    End If
                    If TypeOf item Is DateField Then
                        'str.IsHtmlValue = DirectCast(item, TextField).HTMLEnable
                        metadata.Add(New DateTimeParameter(item.ColumnName, DirectCast(item, DateField).Value))
                    End If
                    If TypeOf item Is EnumField AndAlso Not String.IsNullOrEmpty(DirectCast(item, EnumField).Value) Then
                        metadata.Add(New StringParameter(item.ColumnName, Server.HtmlEncode(String.Join(", ", JsonConvert.DeserializeObject(Of List(Of String))(DirectCast(item, EnumField).Value)).ToArray())))
                    End If
                    If TypeOf item Is LookupField AndAlso Not String.IsNullOrEmpty(DirectCast(item, EnumField).Value) Then
                        metadata.Add(New StringParameter(item.ColumnName, Server.HtmlEncode(String.Join(", ", JsonConvert.DeserializeObject(Of List(Of String))(DirectCast(item, LookupField).Value)).ToArray())))
                    End If
                    If TypeOf item Is BoolField Then
                        metadata.Add(New StringParameter(item.ColumnName, If(DirectCast(item, BoolField).Value, "vero", "falso")))
                    End If
                    If TypeOf item Is NumberField Then
                        Dim res As NumberField = DirectCast(item, NumberField)
                        metadata.Add(New StringParameter(item.ColumnName, res.Value.ToString(res.Format)))
                    End If
                Next
                metadata.Add(New StringParameter("_subject", Server.HtmlEncode(UDSItemSource.Model.Subject.Value)))
                metadata.Add(New StringParameter("_filename", $"{UDSItemSource.Model.Alias}_{UDSYear.Value}_{UDSNumber.Value:0000000}"))

                RadScriptManager.RegisterStartupScript(Page, Page.GetType(), "InitializePageFromWorkflowUI", String.Concat("$(document).ready(function() {SetMetadataSessionStorage('", JsonConvert.SerializeObject(metadata, DocSuiteContext.DefaultWebAPIJsonSerializerSettings), "','", Server.HtmlEncode(UDSItemSource.SerializeToXml()), "')});"), True)
            End If
            Exit Sub
        End If


        If UDSStatus.HasValue AndAlso UDSStatus.Value.Equals(0) Then
            SetDeletedState(UDSCancelMotivation)
        End If
        RepositoryBind()
    End Sub

    ''' <summary>
    ''' Abilita la sezione elementi collegati e mostra il link verso il link di protocollo
    ''' </summary>
    Public Sub InitializeElementiCollegati()
        uscDocumentUnitReferences.Visible = True
        If UDSId.HasValue Then
            uscDocumentUnitReferences.IdDocumentUnit = UDSId.Value.ToString()
            uscDocumentUnitReferences.DocumentUnitYear = UDSYear?.ToString()
            uscDocumentUnitReferences.DocumentUnitNumber = UDSNumber?.ToString()
        End If
    End Sub

    Public Sub InitializeMulticlassification()
        uscMulticlassificationRest.Visible = ProtocolEnv.MulticlassificationEnabled
        uscMulticlassificationRest.IdDocumentUnit = UDSId.Value.ToString()
    End Sub

    Public Sub RepositoryBind(Optional changeVisibility As Boolean = True)
        If UDSItemSource Is Nothing Then
            Exit Sub
        End If

        If Not CurrentUDSRepositoryId.HasValue Then
            Throw New ArgumentNullException("CurrentUDSRepositoryId non impostato")
        End If

        If changeVisibility Then
            SetRowControlVisibile(True)
        End If
        Dim repository As UDSRepository = UDSRepositoryFacade.GetById(CurrentUDSRepositoryId.Value)
        Dim schemaRepositoryModel As UDSModel = UDSModel.LoadXml(repository.ModuleXML)

        lblDddlUds.Text = repository.Name

        If repository.Container IsNot Nothing Then
            lblContainer.Text = repository.Container.Name
        End If

        tblUDSinformation.Visible = Not schemaRepositoryModel.Model.HideRegistrationIdentifier
        If tblUDSinformation.Visible Then
            If UDSYear.HasValue Then
                lblYear.Text = String.Format(YEAR_LABEL_FORMAT, UDSYear.Value)
            End If
            If UDSNumber.HasValue Then
                lblNumer.Text = String.Format(NUMBER_LABEL_FORMAT, UDSNumber.Value)
            End If
        End If

        If UDSRegistrationDate.HasValue Then
            lblRegistrationDate.Text = String.Format(REGDATE_LABEL_FORMAT, UDSRegistrationDate.Value)
        End If

        If rowUDSInfo.Visible AndAlso UDSRegistrationDate.HasValue Then
            lblRegistrationUser.Text = String.Format("{0} {1:dd/MM/yyyy}", CommonAD.GetDisplayName(UDSRegistrationUser), UDSRegistrationDate.Value.ToLocalTime)
            lblLastChangedUser.Text = CommonAD.GetDisplayName(UDSLastChangedUser)

            If Not UDSLastChangedUser.IsNullOrEmpty() Then
                lblLastChangedUser.Text = String.Format("{0} {1:dd/MM/yyyy}", CommonAD.GetDisplayName(UDSLastChangedUser), UDSLastChangedDate.Value.ToLocalTime())
            End If

            If AuhtorizedUsers.Count() > 0 Then
                lblAuthorized.Visible = True
                Dim userLabel As String = ""
                For Each user As WebAPIUDS.UDSUser In AuhtorizedUsers
                    userLabel = CommonAD.GetDisplayName(user.Account)
                    If String.IsNullOrEmpty(userLabel) Then
                        userLabel = user.Account
                    End If
                Next
                lblAuthorized.Text = userLabel
            End If
        End If

        'Oggetto
        lblSubject.Text = UDSSubject

        ' Classificazione
        If tblClassificazione.Visible Then
            lblCode.Text = UDSCategory.GetCodeDotted()
            lblDescription.Text = FacadeFactory.Instance.CategoryFacade.GetFullIncrementalName(FacadeFactory.Instance.CategoryFacade.GetById(UDSCategory.EntityShortId))
        End If

        CurrentStaticDataControl.SetData(UDSItemSource)

        udsDynamicControls.LoadDynamicControls(schemaRepositoryModel.Model, True)
        udsDynamicControls.InitializeDynamicFilters()
        udsDynamicControls.LoadDefaultData()

        ' Contatti
        If rowUDSContacts.Visible Then
            udsDynamicContactsControl.LoadDynamicControls(schemaRepositoryModel.Model, True)
            udsDynamicContactsControl.InitializeDynamicFilters()
            udsDynamicContactsControl.LoadDefaultData()
        End If

        ' Autorizzazioni
        If ActionType.Eq(ACTION_TYPE_VIEW) AndAlso UDSAuthorizations.Count > 0 Then
            rowUDSAuthorizations.Visible = True
        End If

        If rowUDSAuthorizations.Visible Then
            udsDynamicAuthorizationsControl.LoadDynamicControls(schemaRepositoryModel.Model, True)
            udsDynamicAuthorizationsControl.InitializeDynamicFilters()
            udsDynamicAuthorizationsControl.LoadDefaultData()
        End If

        'Mail
        If ActionType.Equals(ACTION_TYPE_VIEW) AndAlso UDSMessages.Count > 0 Then
            tblUDSMessage.Visible = True
            Dim messages As IList(Of DSWMessage) = New List(Of DSWMessage)
            For Each message As UDSEntityMessageDto In UDSMessages
                If message.IdMessage.HasValue Then
                    Dim item As DSWMessage = Facade.MessageFacade.GetById(message.IdMessage.Value)
                    If item IsNot Nothing Then
                        messages.Add(item)
                    End If
                End If
            Next

            Dim totalCountMessage As Integer = messages.Sum(Function(p) p.Emails.Count())
            UDSMessageTitle.Text = String.Format("Messaggi Mail inviati ({0})", totalCountMessage)
            btnExpandUDSMessage.ImageUrl = ImagePath.SmallExpand

            Dim messageEmails As New List(Of MessageEmail)
            For Each item As DSWMessage In messages
                messageEmails.AddRange(item.Emails)
            Next
            messageEmails = messageEmails.OrderByDescending(Function(x) Not x.SentDate.HasValue).ThenByDescending(Function(x) x.SentDate).ThenByDescending(Function(x) x.Id).ToList()

            rptUDSMessage.DataSource = messageEmails
            rptUDSMessage.DataBind()

            If totalCountMessage <= ProtocolEnv.ProtMinimumPecToShow Then
                RepeaterExpander(btnExpandUDSMessage, rptUDSMessage)
            End If
        End If

        'PECMail
        If ProtocolEnv.IsPECEnabled AndAlso ActionType.Equals(ACTION_TYPE_VIEW) AndAlso UDSPecMails.Count() > 0 Then
            InitializePecPanel()
        End If

        'PARER
        If tblParer.Visible Then
            InitializeParerPanel()
        End If
    End Sub

    Public Sub RefreshDynamicControls()
        If UDSItemSource Is Nothing Then
            Exit Sub
        End If

        udsDynamicControls.SetUDSValues(UDSItemSource.Model)
        udsDynamicContactsControl.SetUDSValues(UDSItemSource.Model)
        udsDynamicAuthorizationsControl.SetUDSValues(UDSItemSource.Model)
    End Sub

    Public Sub udsDynamicControl_OnNeedDynamicsSource(source As Object, e As EventArgs) Handles udsDynamicControls.OnNeedDynamicsSource
        If UDSItemSource IsNot Nothing Then
            udsDynamicControls.SetUDSValues(UDSItemSource.Model)
        End If
    End Sub

    Public Sub udsDynamicContactsControl_OnNeedDynamicsSource(source As Object, e As EventArgs) Handles udsDynamicContactsControl.OnNeedDynamicsSource
        If UDSItemSource IsNot Nothing Then
            udsDynamicContactsControl.SetUDSValues(UDSItemSource.Model)
        End If
    End Sub
    Public Sub udsDynamicAuthorizationsControl_OnNeedDynamicsSource(source As Object, e As EventArgs) Handles udsDynamicAuthorizationsControl.OnNeedDynamicsSource
        If UDSItemSource IsNot Nothing Then
            udsDynamicAuthorizationsControl.SetUDSValues(UDSItemSource.Model)
        End If
    End Sub

    Public Function GetUDSModel() As UDSModel
        Dim dynamicModel As UnitaDocumentariaSpecifica = udsDynamicControls.GetUDSValues()
        If dynamicModel Is Nothing Then
            dynamicModel = New UnitaDocumentariaSpecifica()
            dynamicModel.Category = New Helpers.UDS.Category()
        End If

        Dim repository As UDSRepository = UDSRepositoryFacade.GetById(CurrentUDSRepositoryId.Value)
        dynamicModel.Title = repository.Name
        dynamicModel.Subject.Value = CurrentStaticDataControl.Subject
        dynamicModel.Category.IdCategory = CurrentStaticDataControl.IdCategory.ToString()

        Dim model As UDSModel = New UDSModel()
        model.Model = dynamicModel

        Return model
    End Function

    Public Function GetDatesBetween(fieldToGet As String) As Control
        Return udsDynamicControls.GetDatesBetween(fieldToGet)
    End Function

    Private Sub ToggleFinderBorderStyle(setBorder As Boolean)
        If setBorder Then
            AjaxManager.ResponseScripts.Add("setBorderBottom();")
        Else
            AjaxManager.ResponseScripts.Add("removeBorderBottom();")
        End If
    End Sub

    Public Shared Sub RepeaterExpander(button As ImageButton, repeater As Repeater)
        If repeater.Visible Then
            button.ImageUrl = ImagePath.SmallExpand
            button.ToolTip = "Espandi"
            repeater.Visible = False
        Else
            button.ImageUrl = ImagePath.SmallShrink
            button.ToolTip = "Comprimi"
            repeater.Visible = True
        End If
    End Sub

    Public Sub InitializeParerPanel()
        'TODO: Gestire verifica stato di conservazione
    End Sub

    Public Sub InitializePecPanel()
        Dim outgoingPecMails As IList(Of PECMail) = New List(Of PECMail)
        Dim ingoingPecMails As IList(Of PECMail) = New List(Of PECMail)
        For Each pecMail As UDSEntityPECMailDto In UDSPecMails
            If pecMail.IdPECMail.HasValue Then
                Dim item As PECMail = Facade.PECMailFacade.GetById(pecMail.IdPECMail.Value)
                If item IsNot Nothing AndAlso (item.IsActive.Equals(ActiveType.PECMailActiveType.Active) OrElse
                item.IsActive.Equals(ActiveType.PECMailActiveType.Processing) OrElse item.IsActive.Equals(ActiveType.PECMailActiveType.Error)) Then
                    If item.Direction = PECMailDirection.Ingoing Then
                        ingoingPecMails.Add(item)
                    Else
                        outgoingPecMails.Add(item)
                    End If
                End If
            End If
        Next

        If ingoingPecMails.Count > 0 Then
            tblIngoingPEC.Visible = True
            ingoingPecTitle.Text = String.Format("Messaggi d'origine ({0})", ingoingPecMails.Count)

            rptIngoingPEC.DataSource = ingoingPecMails.OrderByDescending(Function(x) x.MailDate)
            rptIngoingPEC.DataBind()

            If ingoingPecMails.Count <= ProtocolEnv.ProtMinimumPecToShow Then
                RepeaterExpander(btnExpandIngoingPec, rptIngoingPEC)
            End If
        End If

        chkShowOtherStatusPec.Visible = outgoingPecMails.Any(Function(p) p.IsActive.Equals(ActiveType.PECMailActiveType.Processing) OrElse
                                                         p.IsActive.Equals(ActiveType.PECMailActiveType.Error))

        If Not chkShowOtherStatusPec.Visible OrElse (chkShowOtherStatusPec.Visible AndAlso Not chkShowOtherStatusPec.Checked) Then
            outgoingPecMails = outgoingPecMails.Where(Function(p) p.IsActive.Equals(ActiveType.PECMailActiveType.Active)).ToList()
        End If

        If ProtocolEnv.PECDraftMailBoxId <> -1 Then
            outgoingPecMails = outgoingPecMails.Where(Function(p) p.MailBox.Id <> DocSuiteContext.Current.ProtocolEnv.PECDraftMailBoxId).ToList()
        End If

        If outgoingPecMails.Count() > 0 Then
            tblOutgoingPEC.Visible = True
            Dim multiplePecs As IList(Of PECMail) = outgoingPecMails.Where(Function(p) p.Multiple).ToList()
            outgoingPecTitle.Text = String.Format("Messaggi PEC inviati ({0}){1}", outgoingPecMails.Count(), If(multiplePecs.Count() > 0, " - Elaborazione in corso PEC multiple", String.Empty))
            btnExpandOutgoingPec.ImageUrl = ImagePath.SmallExpand
            outgoingPecMails = outgoingPecMails.Except(multiplePecs).OrderByDescending(Function(x) x.MailDate).ToList()
            rptOutgoingPEC.DataSource = outgoingPecMails.OrderByDescending(Function(x) Not x.MailDate.HasValue).ThenByDescending(Function(x) x.MailDate)
            rptOutgoingPEC.DataBind()

            If outgoingPecMails.Count() <= ProtocolEnv.ProtMinimumPecToShow Then
                RepeaterExpander(btnExpandOutgoingPec, rptOutgoingPEC)
            End If
        End If
    End Sub

    Public Function GetDeletedDocuments(document As Helpers.UDS.Document) As IList(Of Guid)
        Return udsDynamicControls.GetDeletedDocuments(document)
    End Function

    Private Sub InitializeUDSTypology()
        Dim typologyFinder As UDSTypologyFinder = New UDSTypologyFinder(DocSuiteContext.Current.Tenants)
        typologyFinder.Status = WebAPIUDS.UDSTypologyStatus.Active
        typologyFinder.EnablePaging = False
        Dim results As ICollection(Of WebAPIDto(Of WebAPIUDS.UDSTypology)) = typologyFinder.DoSearch()
        Dim typologies As ICollection(Of WebAPIUDS.UDSTypology)
        If results IsNot Nothing Then
            typologies = results.Select(Function(r) r.Entity).ToList()
            ddlTypology.Items.Add(New RadComboBoxItem(String.Empty, String.Empty))
            For Each item As WebAPIUDS.UDSTypology In typologies
                ddlTypology.Items.Add(New RadComboBoxItem(item.Name, item.UniqueId.ToString()))
            Next
            ddlTypology.SelectedIndex = 0
        End If
    End Sub

    Public Sub InitializeFilters(DetailsSearchModel As UDSRepositoryDetailsSearchModel)
        udsDataFinder.InitializeFilters(DetailsSearchModel)
    End Sub

    Public Sub InitializeCombos()
        SessionIsEmpty = True
        If Session("TempUDSRepositorySearchFilters") IsNot Nothing Then
            SessionIsEmpty = False
            Dim SearchModel As UDSRepositorySearchModel = CType(Session("TempUDSRepositorySearchFilters"), UDSRepositorySearchModel)
            Session("TempUDSRepositorySearchFilters") = Nothing
            If SearchModel.TipologyId <> String.Empty Then
                CurrentUDSTypologyId = Guid.Parse(SearchModel.TipologyId)
            End If
            If SearchModel.UDSRepositoryId <> String.Empty Then
                CurrentUDSRepositoryId = Guid.Parse(SearchModel.UDSRepositoryId)
            End If
        End If
    End Sub

    Public Sub SaveDynamicFiltersToSession()
        udsDynamicControls.SaveDynamicFiltersToSession()
    End Sub

    Public Function GetFinderModel() As UDSFinderDto
        Dim finderModel As UDSFinderDto = New UDSFinderDto()
        Dim model As UDSModel = GetUDSModel()

        If CurrentUDSRepositoryId.HasValue Then
            finderModel.IdRepository = CurrentUDSRepositoryId.Value
        End If

        Dim finderControl As IUDSFinderStaticData = DirectCast(CurrentStaticDataControl, IUDSFinderStaticData)

        If finderControl.Year.HasValue Then
            finderModel.Filters.Add(New UDSFinderExpressionDto() With {.FieldName = "_year", .Expression = String.Format("_year eq {0}", finderControl.Year.Value)})
        End If

        If finderControl.Number.HasValue Then
            Dim numberFormat As String = finderControl.Number.Value.ToString()
            Dim decimalSeparator As String = Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator
            numberFormat = numberFormat.Replace(decimalSeparator, ".")
            finderModel.Filters.Add(New UDSFinderExpressionDto() With {.FieldName = "_number", .Expression = String.Format("_number eq {0}", numberFormat)})
        End If

        If finderControl.RegistrationDateFrom.HasValue Then
            finderModel.Filters.Add(New UDSFinderExpressionDto() With {.FieldName = "RegistrationDateFrom",
                                    .Expression = String.Format("RegistrationDate ge cast({0}Z, Edm.DateTimeOffset)", finderControl.RegistrationDateFrom.Value.ToString("s"))})
        End If

        If finderControl.RegistrationDateTo.HasValue Then
            finderModel.Filters.Add(New UDSFinderExpressionDto() With {.FieldName = "RegistrationDateTo",
                                    .Expression = String.Format("RegistrationDate lt cast({0}Z, Edm.DateTimeOffset)", finderControl.RegistrationDateTo.Value.AddDays(1.0).ToString("s"))})
        End If

        finderModel.Filters.Add(New UDSFinderExpressionDto() With {.FieldName = "_status", .Expression = String.Format("_status eq {0}", Convert.ToInt32(Not finderControl.ViewDeletedUDS))})

        If model IsNot Nothing Then
            If Not String.IsNullOrEmpty(model.Model.Subject.Value) Then
                finderModel.Filters.Add(New UDSFinderExpressionDto() With {.FieldName = "_subject", .Expression = String.Format("contains(_subject, '{0}')", model.Model.Subject.Value)})
            End If

            If model.Model.Category IsNot Nothing AndAlso model.Model.Category.Searchable AndAlso Not String.IsNullOrEmpty(model.Model.Category.IdCategory) Then
                finderModel.Filters.Add(New UDSFinderExpressionDto() With {.FieldName = "IdCategory", .Expression = String.Format("IdCategory eq {0}", model.Model.Category.IdCategory)})
            End If

            If model.Model.Contacts IsNot Nothing Then
                For Each contact As Contacts In model.Model.Contacts
                    If Not contact.ContactInstances.IsNullOrEmpty() AndAlso contact.Searchable Then
                        For Each contactInstance As ContactInstance In contact.ContactInstances
                            finderModel.Filters.Add(New UDSFinderExpressionDto() With {
                                                                                .FieldName = String.Concat("IdContact", Guid.NewGuid()),
                                                                                .Expression = String.Format(ODATA_CONTACT_FILTER, "IdContact",
                                                                                                            contactInstance.IdContact, contact.Label)})
                        Next
                    End If

                    If Not contact.ContactManualInstances.IsNullOrEmpty() Then
                        For Each manualInstance As ContactManualInstance In contact.ContactManualInstances
                            finderModel.Filters.Add(New UDSFinderExpressionDto() With {
                                                                                .FieldName = String.Concat("ContactManual", Guid.NewGuid()),
                                                                                .Expression = String.Format(ODATA_CONTACT_MANUAL_FILTER, "ContactManual",
                                                                                                            JsonConvert.DeserializeObject(Of Data.ContactDTO)(manualInstance.ContactDescription).Contact.DescriptionFormatByContactType,
                                                                                                            contact.Label)})
                        Next
                    End If
                Next
            End If

            If model.Model.Authorizations IsNot Nothing AndAlso model.Model.Authorizations.Searchable AndAlso Not model.Model.Authorizations.Instances.IsNullOrEmpty() Then
                Dim UniqueIds As String = String.Join(",", model.Model.Authorizations.Instances.Select(Function(t) t.UniqueId))
                finderModel.Filters.Add(New UDSFinderExpressionDto() With {
                                                                              .FieldName = "UniqueIdRole",
                                                                              .Expression = String.Format(ODATA_DOCUMENTUNIT_FILTER, "UniqueIdRole", UniqueIds)})
            End If

            If model.Model.Metadata IsNot Nothing Then
                For Each section As Section In model.Model.Metadata
                    If section.Items IsNot Nothing Then
                        For Each item As Object In section.Items
                            Dim elementName As String = item.GetType().Name
                            Select Case elementName
                                Case uscUDSDynamics.CTL_ENUM
                                    Dim field As EnumField = DirectCast(item, EnumField)
                                    If Not field.Searchable OrElse String.IsNullOrEmpty(field.Value) Then
                                        Continue For
                                    End If

                                    If field.MultipleValues Then
                                        Dim selectedItems As ICollection(Of String) = JsonConvert.DeserializeObject(Of ICollection(Of String))(field.Value) _
                                        .Where(Function(x) Not String.IsNullOrEmpty(x)).ToList()
                                        Dim expression As String = String.Join(" or ", selectedItems.Select(Function(s) $"contains({field.ColumnName}, '""{s}""')"))
                                        If Not String.IsNullOrEmpty(expression) Then
                                            finderModel.Filters.Add(New UDSFinderExpressionDto() With {.FieldName = field.ColumnName, .Expression = $"({expression})"})
                                        End If
                                    Else
                                        Dim selectedItems As ICollection(Of String) = JsonConvert.DeserializeObject(Of ICollection(Of String))(field.Value)
                                        If selectedItems IsNot Nothing AndAlso Not String.IsNullOrEmpty(selectedItems(0)) Then
                                            finderModel.Filters.Add(New UDSFinderExpressionDto() With {.FieldName = field.ColumnName, .Expression = $"contains({field.ColumnName}, '""{selectedItems(0)}""')"})
                                        End If
                                    End If

                                Case uscUDSDynamics.CTL_LOOKUP
                                    Dim field As LookupField = DirectCast(item, LookupField)
                                    If Not field.Searchable OrElse String.IsNullOrEmpty(field.Value) Then
                                        Continue For
                                    End If

                                    If field.MultipleValues Then
                                        Dim selectedItems As ICollection(Of String) = JsonConvert.DeserializeObject(Of ICollection(Of String))(field.Value)
                                        Dim expression As String = String.Join(" or ", selectedItems.Select(Function(s) $"contains({field.ColumnName}, '""{s}""')"))
                                        finderModel.Filters.Add(New UDSFinderExpressionDto() With {.FieldName = field.ColumnName, .Expression = $"({expression})"})
                                    Else
                                        Dim selectedItems As ICollection(Of String) = JsonConvert.DeserializeObject(Of ICollection(Of String))(field.Value)
                                        finderModel.Filters.Add(New UDSFinderExpressionDto() With {.FieldName = field.ColumnName, .Expression = $"contains({field.ColumnName}, '""{selectedItems(0)}""')"})
                                    End If

                                Case uscUDSDynamics.CTL_STATUS
                                    Dim field As StatusField = DirectCast(item, StatusField)
                                    If Not field.Searchable OrElse String.IsNullOrEmpty(field.Value) Then
                                        Continue For
                                    End If

                                    finderModel.Filters.Add(New UDSFinderExpressionDto() With {.FieldName = field.ColumnName, .Expression = $"contains({field.ColumnName}, '{field.Value}')"})

                                Case uscUDSDynamics.CTL_TEXT
                                    Dim field As TextField = DirectCast(item, TextField)
                                    If Not field.Searchable OrElse String.IsNullOrEmpty(field.Value) Then
                                        Continue For
                                    End If

                                    finderModel.Filters.Add(New UDSFinderExpressionDto() With {.FieldName = field.ColumnName, .Expression = String.Format("contains({0}, '{1}')", field.ColumnName, field.Value)})
                                Case uscUDSDynamics.CTL_NUMBER
                                    Dim field As NumberField = DirectCast(item, NumberField)
                                    Dim numberFormat As String = field.Value.ToString()
                                    Dim decimalSeparator As String = Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator
                                    numberFormat = numberFormat.Replace(decimalSeparator, ".")


                                    If Not field.Searchable OrElse Not field.ValueSpecified Then
                                        Continue For
                                    End If

                                    finderModel.Filters.Add(New UDSFinderExpressionDto() With {.FieldName = field.ColumnName, .Expression = String.Format("{0} eq {1}", field.ColumnName, numberFormat)})
                                Case uscUDSDynamics.CTL_DATE
                                    Dim field As DateField = DirectCast(item, DateField)
                                    If Not field.Searchable Then
                                        Continue For
                                    End If

                                    Dim fromDate As RadDatePicker = CType(GetDatesBetween(CType("field_" + item.ColumnName + "FromDate", String)), RadDatePicker)
                                    Dim toDate As RadDatePicker = CType(GetDatesBetween(CType("field_" + item.ColumnName + "ToDate", String)), RadDatePicker)
                                    Dim startDate As String
                                    Dim endDate As String

                                    If fromDate.SelectedDate.HasValue AndAlso Not toDate.SelectedDate.HasValue Then
                                        startDate = fromDate.SelectedDate.Value.ToString("s")
                                        finderModel.Filters.Add(New UDSFinderExpressionDto() With {.FieldName = CType(item.ColumnName, String), .Expression = String.Format("{0} eq cast({1}Z, Edm.DateTimeOffset)", CType(item.ColumnName, String), startDate)})
                                    ElseIf Not fromDate.SelectedDate.HasValue AndAlso toDate.SelectedDate.HasValue Then
                                        endDate = toDate.SelectedDate.Value.ToString("s")
                                        finderModel.Filters.Add(New UDSFinderExpressionDto() With {.FieldName = CType(item.ColumnName, String), .Expression = String.Format("{0} eq cast({1}Z, Edm.DateTimeOffset)", CType(item.ColumnName, String), endDate)})
                                    ElseIf fromDate.SelectedDate.HasValue AndAlso toDate.SelectedDate.HasValue Then
                                        startDate = fromDate.SelectedDate.Value.ToString("s")
                                        endDate = toDate.SelectedDate.Value.ToString("s")
                                        finderModel.Filters.Add(New UDSFinderExpressionDto() With {.FieldName = CType(item.ColumnName, String), .Expression = String.Format("({0} ge cast({1}Z, Edm.DateTimeOffset)) and ({0} le cast({2}Z, Edm.DateTimeOffset))", CType(item.ColumnName, String), startDate, endDate)})
                                    Else
                                        Continue For
                                    End If

                                Case uscUDSDynamics.CTL_CHECKBOX
                                    Dim field As BoolField = DirectCast(item, BoolField)
                                    If Not field.Searchable OrElse Not field.ValueSpecified Then
                                        Continue For
                                    End If

                                    finderModel.Filters.Add(New UDSFinderExpressionDto() With {.FieldName = field.ColumnName, .Expression = String.Format("{0} eq {1}", field.ColumnName, field.Value.ToString().ToLower())})

                            End Select
                        Next
                    End If
                Next
            End If
        End If
        Return finderModel
    End Function
#End Region

End Class