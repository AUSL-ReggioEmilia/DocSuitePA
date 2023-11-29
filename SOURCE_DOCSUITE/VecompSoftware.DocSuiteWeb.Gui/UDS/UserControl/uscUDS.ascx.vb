Imports System.Collections.Generic
Imports System.Linq
Imports System.Text.RegularExpressions
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.BusinessRule.Rules.Rights.PEC
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Conservations
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Tenants
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Entity.Conservations
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
    Private _currentTenantFinder As TenantFinder
    Private _currentConservation As Conservation
    Public Const ACTION_TYPE_INSERT As String = "Insert"
    Public Const ACTION_TYPE_EDIT As String = "Edit"
    Public Const ACTION_TYPE_VIEW As String = "View"
    Public Const ACTION_TYPE_SEARCH As String = "Search"
    Public Const ACTION_TYPE_AUTHORIZE As String = "Authorize"
    Public Const ACTION_TYPE_DUPLICATE As String = "Duplicate"
    Public Const YEAR_LABEL_FORMAT As String = "Anno: {0}"
    Public Const NUMBER_LABEL_FORMAT As String = "Numero: {0:0000000}"
    Public Const REGDATE_LABEL_FORMAT As String = "Data registrazione: {0:dd/MM/yyyy}"
    Public Const PAGE_PROTOCOL_SUMMARY As String = "~/Prot/ProtVisualizza.aspx?UniqueId={0}&Type=Prot"
#End Region

#Region "Odata query"
    Private Const ODATA_DOCUMENTUNIT_FILTER As String = "DocumentUnit/DocumentUnitRoles/any(r:r/{0} in ({1}))"
    Private Const ODATA_CONTACT_FILTER As String = "DocumentUnit/UDSContacts/any(contacts:contacts/Relation/EntityId eq {0} and contacts/ContactLabel eq '{1}')"
    Private Const ODATA_CONTACT_MANUAL_FILTER As String = "(DocumentUnit/UDSContacts/any(c:contains(c/ContactManual,'{0}') and c/ContactLabel eq '{1}') or DocumentUnit/UDSContacts/any(c1:contains(c1/Relation/Description,'{0}') and c1/ContactLabel eq '{1}'))"
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

    Public Property VisibleConservation As Boolean
        Get
            If ViewState(String.Format("{0}_VisibleParer", ID)) IsNot Nothing Then
                Return DirectCast(ViewState(String.Format("{0}_VisibleParer", ID)), Boolean)
            End If
            Return False
        End Get
        Set(ByVal value As Boolean)
            ViewState(String.Format("{0}_VisibleParer", ID)) = value
        End Set
    End Property

    Public Property HasEditableRight As Boolean
    Public Property HasActiveWorkflowActivity As Boolean = False
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

    Public ReadOnly Property CurrentConservation As Conservation
        Get
            If _currentConservation Is Nothing AndAlso UDSId.HasValue Then
                Dim conservation As Conservation = WebAPIImpersonatorFacade.ImpersonateFinder(New ConservationFinder(DocSuiteContext.Current.CurrentTenant),
                    Function(impersonationType, finder)
                        finder.UniqueId = UDSId.Value
                        finder.EnablePaging = False
                        Return finder.DoSearch().Select(Function(x) x.Entity).FirstOrDefault()
                    End Function)

                _currentConservation = conservation
            End If
            Return _currentConservation
        End Get
    End Property
#End Region

#Region "Events"

    Protected Sub ddlUds_ItemsChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs) Handles ddlUds.SelectedIndexChanged
        RaiseEvent UDSIndexChanged(Me, e)

        Web.HttpContext.Current.Session.Add("Archive.Search.ArchiveType", ddlUds.SelectedItem.Text)
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
        udsDynamicControls.IdUDSRepository = CurrentUDSRepositoryId
        udsDynamicControls.MyAuthorizedRolesEnabled = True
        udsDynamicControls.LoadDynamicControls(schemaRepository.Model, True)
        udsDynamicControls.InitializeDynamicFilters()
        udsDynamicControls.LoadDefaultData()

        If FromUDSLink Then
            AjaxManager.ResponseScripts.Add(String.Concat("UDSRepositoryOnChange('", CurrentUDSRepositoryId, "');"))
        End If

        If ActionType.Eq(ACTION_TYPE_INSERT) OrElse ActionType.Eq(ACTION_TYPE_EDIT) Then
            AjaxManager.ResponseScripts.Add("RemovePostbackSessionState();")
        End If
    End Sub

    Protected Sub ddlUds_ItemsRequested(sender As Object, e As RadComboBoxItemsRequestedEventArgs) Handles ddlUds.ItemsRequested
        Dim repositories As IList(Of WebAPIUDS.UDSRepository)

        Dim results As ICollection(Of WebAPIDto(Of WebAPIUDS.UDSRepository)) = WebAPIImpersonatorFacade.ImpersonateFinder(New UDSRepositoryFinder(DocSuiteContext.Current.Tenants),
                    Function(impersonationType, finder)
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
                        Return finder.DoSearch()
                    End Function)

        If results IsNot Nothing Then
            repositories = results.Select(Function(r) r.Entity).ToList()
            If BasePage.CurrentTenant IsNot Nothing AndAlso BasePage.CurrentTenant.Containers IsNot Nothing Then
                Dim fullRepository As WebAPIUDS.UDSRepository
                For Each repository As WebAPIUDS.UDSRepository In repositories.ToList()
                    fullRepository = WebAPIImpersonatorFacade.ImpersonateFinder(New UDSRepositoryFinder(DocSuiteContext.Current.Tenants),
                        Function(impersonationType, finder)
                            finder.ActionType = UDSRepositoryFinderActionType.FindElement
                            finder.UniqueId = repository.UniqueId
                            finder.ExpandProperties = True
                            finder.EnablePaging = False
                            Return finder.DoSearch().Select(Function(s) s.Entity).First()
                        End Function)
                    If (Not BasePage.CurrentTenant.Containers.Any(Function(x) x.EntityShortId = fullRepository.Container.EntityShortId)) Then
                        repositories.Remove(repository)
                    End If
                Next
            Else
                repositories = New List(Of WebAPIUDS.UDSRepository)
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
        uscDocumentUnitReferences.ShowRemoveUDSLinksButton = HasEditableRight

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

    Private Sub btnExpandUDSMessage_Click(sender As Object, e As ImageClickEventArgs) Handles btnExpandUDSMessage.Click
        RepeaterExpander(btnExpandUDSMessage, rptUDSMessage)
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
        AjaxManager.AjaxSettings.AddAjaxSetting(btnExpandUDSMessage, tblUDSMessage)
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
                InitializeEditAction(HasActiveWorkflowActivity)

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
        udsDynamicControls.DocumentsReadonly = False
        If ActionType.Eq(ACTION_TYPE_DUPLICATE) Then
            udsDynamicControls.ActionType = ACTION_TYPE_DUPLICATE
        End If
        udsDynamicControls.WorkflowSignedDocRequired = WorkflowSignedDocRequired

        If Not String.IsNullOrEmpty(Request.QueryString("ArchiveTypeId")) Then
            CurrentUDSRepositoryId = Guid.Parse(Request.QueryString("ArchiveTypeId").ToString())
        End If

        If CurrentUDSRepositoryId IsNot Nothing Then
            ddlUds.SelectedValue = CurrentUDSRepositoryId.ToString()
            Call ddlUds_ItemsChanged(Me, New RadComboBoxSelectedIndexChangedEventArgs(String.Empty, String.Empty, CurrentUDSRepositoryId.ToString(), String.Empty))
        End If

        If UDSItemSource IsNot Nothing Then
            RepositoryBind(False)
        End If
    End Sub

    Public Sub InitializeEditAction(documentsReadonly As Boolean)
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
        udsDynamicControls.ViewDocuments = True
        udsDynamicControls.DocumentsReadonly = documentsReadonly

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
            Call ddlTypology_ItemsChanged(ddlTypology, New RadComboBoxSelectedIndexChangedEventArgs(Nothing, Nothing, CurrentUDSTypologyId.Value.ToString(), Nothing))
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
        rowUDSContacts.SetDisplay(False)
        tblAnnullamento.Visible = False
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
        LoadConservation()

        If UDSItemSource Is Nothing Then
            RaiseEvent NeedRepositorySource(Me, New EventArgs())
            If UDSItemSource IsNot Nothing AndAlso UDSItemSource.Model IsNot Nothing AndAlso UDSItemSource.Model.WorkflowEnabled Then
                Dim metadata As List(Of BaseDocumentGeneratorParameter) = New List(Of BaseDocumentGeneratorParameter)()
                Dim str As StringParameter
                For Each item As FieldBaseType In UDSItemSource.Model.Metadata.SelectMany(Function(f) f.Items)
                    If TypeOf item Is TextField Then
                        str = New StringParameter(item.ColumnName, Server.HtmlEncode(DirectCast(item, TextField).Value))
                        str.HasHtmlValue = DirectCast(item, TextField).HTMLEnable
                        metadata.Add(str)
                    End If
                    If TypeOf item Is DateField Then
                        metadata.Add(New DateTimeParameter(item.ColumnName, DirectCast(item, DateField).Value))
                    End If
                    If TypeOf item Is EnumField AndAlso Not String.IsNullOrEmpty(DirectCast(item, EnumField).Value) Then
                        str = New StringParameter(item.ColumnName, Server.HtmlEncode(String.Join(", ", JsonConvert.DeserializeObject(Of List(Of String))(DirectCast(item, EnumField).Value)).ToArray()))
                        metadata.Add(str)
                    End If
                    If TypeOf item Is LookupField AndAlso Not String.IsNullOrEmpty(DirectCast(item, LookupField).Value) Then
                        str = New StringParameter(item.ColumnName, Server.HtmlEncode(String.Join(", ", JsonConvert.DeserializeObject(Of List(Of String))(DirectCast(item, LookupField).Value)).ToArray()))
                        metadata.Add(str)
                    End If
                    If TypeOf item Is BoolField Then
                        str = New StringParameter(item.ColumnName, If(DirectCast(item, BoolField).Value, "vero", "falso"))
                        metadata.Add(str)
                    End If
                    If TypeOf item Is NumberField Then
                        Dim res As NumberField = DirectCast(item, NumberField)
                        str = New StringParameter(item.ColumnName, res.Value.ToString(res.Format))
                        metadata.Add(str)
                    End If
                Next
                metadata.Add(New StringParameter("_subject", Server.HtmlEncode(UDSItemSource.Model.Subject.Value)))
                metadata.Add(New StringParameter("_filename", $"{UDSItemSource.Model.Alias}_{UDSYear.Value}_{UDSNumber.Value:0000000}"))
                metadata.Add(New BooleanParameter("Documents", True))

                Dim udsHtmlEncode As String = Server.HtmlEncode(UDSItemSource.SerializeToXml())
                udsHtmlEncode = udsHtmlEncode.Replace("\", "%5C")

                RadScriptManager.RegisterStartupScript(Page, Page.GetType(), "InitializePageFromWorkflowUI", String.Concat("$(document).ready(function() {SetMetadataSessionStorage('", JsonConvert.SerializeObject(metadata, DocSuiteContext.DefaultWebAPIJsonSerializerSettings), "','", udsHtmlEncode, "')});"), True)
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
    Public Sub InitializeUscDocumentUnitReferences()
        uscDocumentUnitReferences.Visible = True
        uscDocumentUnitReferences.ShowWorkflowActivities = ProtocolEnv.WorkflowManagerEnabled AndAlso ProtocolEnv.WorkflowStateSummaryEnabled
        uscDocumentUnitReferences.ShowFascicleLinks = Not ProtocolEnv.ProcessEnabled AndAlso ProtocolEnv.FascicleEnabled
        uscDocumentUnitReferences.ShowDocumentUnitFascicleLinks = ProtocolEnv.ProcessEnabled
        If UDSId.HasValue Then
            uscDocumentUnitReferences.ReferenceUniqueId = UDSId.Value.ToString()
            uscDocumentUnitReferences.DocumentUnitYear = UDSYear?.ToString()
            uscDocumentUnitReferences.DocumentUnitNumber = UDSNumber?.ToString()
        End If

        If ProtocolEnv.IsPECEnabled Then
            If ProtocolEnv.UnifiedPECLinksPanelEnabled Then
                uscDocumentUnitReferences.ShowPECUnified = True
                uscDocumentUnitReferences.ShowPECIncoming = False
                uscDocumentUnitReferences.ShowPECOutgoing = False
            Else
                uscDocumentUnitReferences.ShowPECUnified = False
                uscDocumentUnitReferences.ShowPECIncoming = True
                uscDocumentUnitReferences.ShowPECOutgoing = True
            End If
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
        End If

        'Oggetto
        lblSubject.Text = UDSSubject

        ' Classificazione
        If tblClassificazione.Visible Then
            lblCode.Text = UDSCategory.GetCodeDotted()
            lblDescription.Text = FacadeFactory.Instance.CategoryFacade.GetFullIncrementalName(FacadeFactory.Instance.CategoryFacade.GetById(UDSCategory.EntityShortId))
        End If

        CurrentStaticDataControl.SetData(UDSItemSource)

        If UDSItemSource.Model.Metadata IsNot Nothing _
            AndAlso UDSItemSource.Model.Metadata.Any() _
            AndAlso UDSItemSource.Model.Metadata.First().Items IsNot Nothing _
            AndAlso UDSItemSource.Model.Metadata.First().Items.Any(Function(x) x.GetType().Name = uscUDSDynamics.CTL_TREE_LIST_FIELD) Then

            Dim udsFieldLists As List(Of TreeListField) = DirectCast(UDSItemSource.Model.Metadata.First().Items _
                .Where(Function(x) x.GetType().Name = uscUDSDynamics.CTL_TREE_LIST_FIELD) _
                .Select(Function(x) DirectCast(x, TreeListField)).ToList(), List(Of TreeListField))
            Dim udsFieldListChildren As New List(Of KeyValuePair(Of String, Guid))
            For Each udsFieldList As TreeListField In udsFieldLists
                If Not String.IsNullOrEmpty(udsFieldList.Value) AndAlso Guid.Parse(udsFieldList.Value) <> Guid.Empty Then
                    udsFieldListChildren.Add(New KeyValuePair(Of String, Guid)(udsFieldList.ColumnName, Guid.Parse(udsFieldList.Value)))
                End If
            Next
            udsDynamicControls.UDSFieldListChildren = udsFieldListChildren
        End If
        udsDynamicControls.LoadDynamicControls(schemaRepositoryModel.Model, True)
        udsDynamicControls.InitializeDynamicFilters()
        udsDynamicControls.IdUDSRepository = CurrentUDSRepositoryId
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

        Dim metadatas As FieldBaseType() = UDSItemSource.Model.Metadata(0).Items
        AjaxManager.ResponseScripts.Add($"SetCurrentMetadataSessionStorage({JsonConvert.SerializeObject(metadatas, DocSuiteContext.DefaultWebAPIJsonSerializerSettings)});")
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

    Public Function GetControlsBetween(fieldToGet As String) As Control
        Return udsDynamicControls.GetControlsBetween(fieldToGet)
    End Function

    Private Sub LoadConservation()
        trConservationStatus.Visible = False

        If VisibleConservation AndAlso CurrentConservation IsNot Nothing Then
            trConservationStatus.Visible = True
            imgConservationIcon.ImageUrl = ConservationHelper.StatusSmallIcon(CurrentConservation.Status)
            lblConservationStatus.Text = ConservationHelper.StatusDescription(CurrentConservation.Status)

            If String.IsNullOrEmpty(CurrentConservation.Uri) Then
                Exit Sub
            End If

            If Not Regex.IsMatch(CurrentConservation.Uri, ProtocolEnv.ConservationURIValidationRegex) Then
                lblConservationUri.Visible = True
                conservationUriLabel.Visible = True

                lblConservationStatus.ToolTip = "Url non compatibile con il portale ingestor"
                lblConservationUri.Text = CurrentConservation.Uri
                Exit Sub
            End If
            If Not String.IsNullOrWhiteSpace(ProtocolEnv.IngestorBaseURL) Then
                lblConservationStatus.NavigateUrl = $"{ProtocolEnv.IngestorBaseURL}/{CurrentConservation.Uri}"
            End If
        End If
    End Sub

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

    Public Function GetDeletedDocuments(document As Helpers.UDS.Document) As IList(Of Guid)
        Return udsDynamicControls.GetDeletedDocuments(document)
    End Function

    Private Sub InitializeUDSTypology()
        Dim results As ICollection(Of WebAPIDto(Of WebAPIUDS.UDSTypology)) = WebAPIImpersonatorFacade.ImpersonateFinder(New UDSTypologyFinder(DocSuiteContext.Current.Tenants),
                        Function(impersonationType, finder)
                            finder.Status = WebAPIUDS.UDSTypologyStatus.Active
                            finder.EnablePaging = False
                            Return finder.DoSearch()
                        End Function)

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

        If Not finderControl.DocumentName.IsNullOrEmpty() Then
            finderModel.Filters.Add(New UDSFinderExpressionDto() With {.FieldName = "DocumentName",
                                    .Expression = String.Format("Documents/any(d:contains(d/DocumentName,'{0}') and d/DocumentType eq 1)", finderControl.DocumentName)})
        End If

        If finderControl.GenericDocument = "1" Then
            finderModel.Filters.Add(New UDSFinderExpressionDto() With {.FieldName = "",
                                    .Expression = "not Documents/any()"})
        End If
        If finderControl.GenericDocument = "2" Then
            finderModel.Filters.Add(New UDSFinderExpressionDto() With {.FieldName = "",
                                .Expression = "Documents/any()"})
        End If

        If finderControl.ViewDeletedUDS = "1" Then
            finderModel.Filters.Add(New UDSFinderExpressionDto() With {.FieldName = "_status",
                                    .Expression = "_status eq 0"})
        End If
        If finderControl.ViewDeletedUDS = "2" Then
            finderModel.Filters.Add(New UDSFinderExpressionDto() With {.FieldName = "_status",
                                .Expression = "_status eq 1"})
        End If

        If model IsNot Nothing Then
            If Not String.IsNullOrEmpty(model.Model.Subject.Value) Then
                finderModel.Filters.Add(New UDSFinderExpressionDto() With {.FieldName = "_subject", .Expression = String.Format("contains(_subject, '{0}')", model.Model.Subject.Value.Replace("'", "''"))})
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
                                                                                .Expression = String.Format(ODATA_CONTACT_FILTER, contactInstance.IdContact, contact.Label)})
                        Next
                    End If

                    If Not contact.ContactManualInstances.IsNullOrEmpty() Then
                        For Each manualInstance As ContactManualInstance In contact.ContactManualInstances
                            finderModel.Filters.Add(New UDSFinderExpressionDto() With {
                                                                                .FieldName = String.Concat("ContactManual", Guid.NewGuid()),
                                                                                .Expression = String.Format(ODATA_CONTACT_MANUAL_FILTER,
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
                                    If Not field.Searchable Then
                                        Continue For
                                    End If

                                    Dim fromNumber As RadNumericTextBox = CType(GetControlsBetween(CType($"field_{item.ColumnName}FromNumber", String)), RadNumericTextBox)
                                    Dim toNumber As RadNumericTextBox = CType(GetControlsBetween(CType($"field_{item.ColumnName}ToNumber", String)), RadNumericTextBox)

                                    Dim decimalSeparator As String = Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator
                                    Dim startNumberFormat As String = If(fromNumber.Value.HasValue,
                                        fromNumber.Value.Value.ToString().Replace(decimalSeparator, "."),
                                        Nothing)
                                    Dim endNumberFormat As String = If(toNumber.Value.HasValue,
                                        toNumber.Value.Value.ToString().Replace(decimalSeparator, "."),
                                        Nothing)

                                    If startNumberFormat IsNot Nothing AndAlso endNumberFormat Is Nothing Then
                                        finderModel.Filters.Add(New UDSFinderExpressionDto() With
                                                                {
                                                                .FieldName = CType(item.ColumnName, String),
                                                                .Expression = String.Format("{0} eq {1}",
                                                                                            CType(item.ColumnName, String),
                                                                                            startNumberFormat)
                                                                })
                                    ElseIf startNumberFormat Is Nothing AndAlso endNumberFormat IsNot Nothing Then
                                        finderModel.Filters.Add(New UDSFinderExpressionDto() With
                                                                {
                                                                .FieldName = CType(item.ColumnName, String),
                                                                .Expression = String.Format("{0} eq {1}",
                                                                                            CType(item.ColumnName, String),
                                                                                            endNumberFormat)
                                                                })
                                    ElseIf startNumberFormat IsNot Nothing AndAlso endNumberFormat IsNot Nothing Then
                                        finderModel.Filters.Add(New UDSFinderExpressionDto() With
                                                                {
                                                                .FieldName = CType(item.ColumnName, String),
                                                                .Expression = String.Format("({0} ge {1}) and ({0} le {2})",
                                                                                            CType(item.ColumnName, String),
                                                                                            startNumberFormat,
                                                                                            endNumberFormat)
                                                                })
                                    Else
                                        Continue For
                                    End If
                                Case uscUDSDynamics.CTL_DATE
                                    Dim field As DateField = DirectCast(item, DateField)
                                    If Not field.Searchable Then
                                        Continue For
                                    End If

                                    Dim fromDate As RadDatePicker = CType(GetControlsBetween(CType("field_" + item.ColumnName + "FromDate", String)), RadDatePicker)
                                    Dim toDate As RadDatePicker = CType(GetControlsBetween(CType("field_" + item.ColumnName + "ToDate", String)), RadDatePicker)
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

                                Case uscUDSDynamics.CTL_TREE_LIST_FIELD
                                    Dim field As TreeListField = DirectCast(item, TreeListField)
                                    If Not field.Searchable OrElse String.IsNullOrEmpty(field.Value) Then
                                        Continue For
                                    End If
                                    Dim selectedItem As Guid = Guid.Parse(field.Value)
                                    finderModel.Filters.Add(New UDSFinderExpressionDto() With {.FieldName = field.ColumnName, .Expression = $"{field.ColumnName} eq {selectedItem}"})

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