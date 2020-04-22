Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Workflows
Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder
Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.Workflows
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.Workflows
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Workflow
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.UDS
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.UDS
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Workflows
Imports VecompSoftware.DocSuiteWeb.Model.Workflow
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.ExtensionMethods.EnumEx
Imports VecompSoftware.Helpers.Workflow
Imports VecompSoftware.Services.Logging

Public Class UserWorkflow
    Inherits WorkflowBasePage

    Dim _finder As New WorkflowActivityFinder(New MapperWorkflowActivity(), DocSuiteContext.Current.User.FullUserName)
    Private Const PROTOCOL_VISUALIZZA_PATH As String = "~/Prot/ProtVisualizza.aspx?Year={0}&Number={1}&Type=Prot"
    Private Const UDS_SUMMARY_PATH As String = "~/UDS/UDSView.aspx?Type=UDS&IdUDS={0}&IdUDSRepository={1}"
    Private Const COLLABORATION_TO_PROTOCOL_UDS_PATH As String = "~/Prot/ProtInserimento.aspx?Type=Prot&Action=FromCollaboration&IdCollaboration={0}&IdUDS={1}&IdUDSRepository={2}"
    Private Const FASCICLE_VISUALIZZA_PATH As String = "~/Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle={0}"
    Private Const DOSSIER_VISUALIZZA_PATH As String = "~/Dossiers/DossierVisualizza.aspx?Type=Dossier&IdDossier={0}&DossierTitle={1}"
    Private Const DESK_VISUALIZZA_PATH As String = "~/Workflows/WorkflowActivitySummary.aspx?"
    Public Const UDS_ADDRESS_NAME As String = "API-UDSAddress"
    Private Const ODATA_FILTER As String = "$filter=_year eq {0} and _number eq {1}"
    Private _currentUDSFacade As UDSFacade
    Private _currentUDSRepositoryFacade As UDSRepositoryFacade

#Region "Property"

    Public ReadOnly Property WorkflowPropertyFacade() As WorkflowPropertyFacade
        Get
            If _workflowPropertyFacade Is Nothing Then
                _workflowPropertyFacade = New WorkflowPropertyFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _workflowPropertyFacade
        End Get
    End Property

    Public ReadOnly Property CurrentWorkflowAuthorizationFacade As WorkflowAuthorizationFacade
        Get
            If _workflowAuthorizationFacade Is Nothing Then
                _workflowAuthorizationFacade = New WorkflowAuthorizationFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _workflowAuthorizationFacade
        End Get
    End Property

    Public ReadOnly Property CurrentUDSFacade As UDSFacade
        Get
            If _currentUDSFacade Is Nothing Then
                _currentUDSFacade = New UDSFacade()
            End If
            Return _currentUDSFacade
        End Get
    End Property

    Public ReadOnly Property CurrentUDSRepositoryFacade As UDSRepositoryFacade
        Get
            If _currentUDSRepositoryFacade Is Nothing Then
                _currentUDSRepositoryFacade = New UDSRepositoryFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _currentUDSRepositoryFacade
        End Get
    End Property

#End Region

#Region " Fields"

    Private _workflowPropertyFacade As WorkflowPropertyFacade
    Private _workflowAuthorizationFacade As WorkflowAuthorizationFacade
    Private _workflowImgPath As IDictionary(Of String, String)
#End Region

#Region " Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()
        InitializeImgPath()

        If Not IsPostBack Then
            InitializeFilters()
            SetFinder()
            wfGrid.CurrentPageIndex = 0
            wfGrid.PageSize = _finder.PageSize
            wfGrid.DataSource = _finder.DoSearchHeader()
            wfGrid.DataBind()
        End If
    End Sub

    Protected Sub InitializeImgPath()
        If DocSuiteContext.Current.ProtocolEnv.WorkflowActivityImages IsNot Nothing Then
            _workflowImgPath = ProtocolEnv.WorkflowActivityImages
        End If
    End Sub

    ''' <summary>
    ''' Esegue la ricerca all'interno di Workflow
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearch.Click
        If Not Page.IsValid Then
            Return
        End If
        Try
            SetFinder()
            wfGrid.DataSource = _finder.DoSearchHeader()
            wfGrid.DataBind()
        Catch ex As DocSuiteException
            AjaxAlert(ex)
        End Try
    End Sub

    Private Sub btnCreate_Click(sender As Object, e As EventArgs) Handles cmdCreate.Click
        Response.Redirect("../Workflows/WorkflowActivityInsert.aspx?Type=Series")
    End Sub

    ''' <summary>
    ''' Decoro i dati nella griglia
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub wfGrid_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles wfGrid.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim workflowActivityResult As WorkflowActivityResult = DirectCast(e.Item.DataItem, WorkflowActivityResult)
        If workflowActivityResult IsNot Nothing Then

            With DirectCast(e.Item.FindControl("lblWorkflowActivityStatus"), Label)
                .Text = workflowActivityResult.WorkflowActivityStatus.GetDescription()
            End With

            With DirectCast(e.Item.FindControl("lblWorkflowSubject"), Label)
                .Text = workflowActivityResult.WorkflowSubject
            End With

            Dim lnkWorkflowActivityName As HyperLink = DirectCast(e.Item.FindControl("lnkWorkflowActivityName"), HyperLink)
            lnkWorkflowActivityName.NavigateUrl = GetActionUrl(workflowActivityResult)
            lnkWorkflowActivityName.Text = workflowActivityResult.WorkflowActivityName
            If String.IsNullOrEmpty(lnkWorkflowActivityName.NavigateUrl) Then
                lnkWorkflowActivityName.Text = String.Concat("< attività con anomalie - ", workflowActivityResult.WorkflowActivityId, " - contattare assistenza > ")
                lnkWorkflowActivityName.Enabled = False
            End If

            Dim role As Model.Workflow.WorkflowRole = Nothing
            Dim jsonWorkflowRole As WorkflowProperty = WorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(workflowActivityResult.WorkflowActivityId, WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_ROLE)
            If jsonWorkflowRole IsNot Nothing Then
                role = JsonConvert.DeserializeObject(Of Model.Workflow.WorkflowRole)(jsonWorkflowRole.ValueString, DocSuiteContext.DefaultWebAPIJsonSerializerSettings)
            End If
            If role IsNot Nothing AndAlso role.Name IsNot Nothing Then
                With DirectCast(e.Item.FindControl("lblWorkflowProposerRoleName"), Label)
                    .Text = role.Name
                End With
            End If

            Dim roleReceiver As Role = Nothing
            Dim jsonWorkflowRoles As WorkflowProperty = WorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(workflowActivityResult.WorkflowActivityId, WorkflowPropertyHelper.DSW_PROPERTY_ROLES)
            If jsonWorkflowRoles IsNot Nothing Then
                Dim roles As IList(Of Model.Workflow.WorkflowMapping) = JsonConvert.DeserializeObject(Of IList(Of Model.Workflow.WorkflowMapping))(jsonWorkflowRoles.ValueString, DocSuiteContext.DefaultWebAPIJsonSerializerSettings)
                Dim dest As Model.Workflow.WorkflowMapping = roles.FirstOrDefault()
                If dest IsNot Nothing AndAlso dest.Role IsNot Nothing Then
                    roleReceiver = Facade.RoleFacade.GetById(dest.Role.IdRole)
                End If
            End If
            If roleReceiver IsNot Nothing AndAlso roleReceiver.Name IsNot Nothing Then
                Dim lblWorkflowReceiverRoleName As Label = DirectCast(e.Item.FindControl("lblWorkflowReceiverRoleName"), Label)
                lblWorkflowReceiverRoleName.Text = roleReceiver.Name
            End If
            If workflowActivityResult.WorkflowActivityType = ActivityType.GenericActivity Then
                With DirectCast(e.Item.FindControl("lblWorkflowProposerRoleName"), Label)
                    .Text = workflowActivityResult.WorkflowActivityRequestorUser
                End With
                Dim jsonWorkflowAccounts As WorkflowProperty = WorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(workflowActivityResult.WorkflowActivityId, WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS)
                Dim workflowAccountPosition As WorkflowProperty = WorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(workflowActivityResult.WorkflowActivityId, WorkflowPropertyHelper.DSW_FIELD_RECIPIENT_POSITION)
                Dim accounts As IList(Of Model.Workflow.WorkflowAccount) = JsonConvert.DeserializeObject(Of IList(Of Model.Workflow.WorkflowAccount))(jsonWorkflowAccounts.ValueString, DocSuiteContext.DefaultWebAPIJsonSerializerSettings)
                Dim lblWorkflowReceiverRoleName As Label = DirectCast(e.Item.FindControl("lblWorkflowReceiverRoleName"), Label)
                Dim position As Integer = 0
                If (workflowAccountPosition IsNot Nothing AndAlso workflowAccountPosition.ValueInt.HasValue) Then
                    position = Convert.ToInt32(workflowAccountPosition.ValueInt.Value)
                End If

                lblWorkflowReceiverRoleName.Text = accounts.ElementAtOrDefault(position)?.AccountName
            End If

            If rdbWfStatus.SelectedValue = "2" Then
                Dim handler As WorkflowAuthorization = CurrentWorkflowAuthorizationFacade.GetWorkflowAuthorizationByActivity(workflowActivityResult.WorkflowActivityId)
                If (handler IsNot Nothing AndAlso Not handler.Account.IsNullOrEmpty) Then
                    Dim lblWorkflowIsHandler As Label = DirectCast(e.Item.FindControl("lblWorkflowIsHandler"), Label)
                    lblWorkflowIsHandler.Text = CommonAD.GetDisplayName(handler.Account)
                End If
            End If

            Dim jsonWorkflowStartFrom As WorkflowProperty = WorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(workflowActivityResult.WorkflowActivityId, WorkflowPropertyHelper.DSW_FIELD_PRODUCT_NAME)
            If jsonWorkflowStartFrom IsNot Nothing Then
                Dim imgWorkflowActivityStart As Image = DirectCast(e.Item.FindControl("imgWorkflowActivityStart"), Image)
                If imgWorkflowActivityStart IsNot Nothing Then
                    imgWorkflowActivityStart.Visible = False
                    Dim jsonWorkflowStartFromValue As String = String.Empty
                    If Not _workflowImgPath Is Nothing AndAlso _workflowImgPath.Keys.Any(Function(f) f = jsonWorkflowStartFrom.ValueString) Then
                        imgWorkflowActivityStart.Visible = True
                        imgWorkflowActivityStart.ImageUrl = _workflowImgPath(jsonWorkflowStartFrom.ValueString)
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub cmdClearFilters_Click(sender As Object, e As EventArgs) Handles btnClearFilters.Click
        Me.ClearFilters(Me.searchTable.Controls)
        InitializeFilters()
    End Sub

    Protected Sub wfGrid_DetailTableDataBind(source As Object, e As Telerik.Web.UI.GridDetailTableDataBindEventArgs) Handles wfGrid.DetailTableDataBind
        Dim dataItem As GridDataItem = DirectCast(e.DetailTableView.ParentItem, GridDataItem)
        FillDataTable(e.DetailTableView, dataItem)
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        Me.AjaxManager.AjaxSettings.AddAjaxSetting(Me.btnClearFilters, Me.searchTable)
        AjaxManager.AjaxSettings.AddAjaxSetting(wfGrid, wfGrid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, wfGrid, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    ''' <summary>
    ''' Funzione per il recupero dei dati necessari per la crezione dell'url 
    ''' </summary>
    ''' <param name="row">Riga della grid popolata dalle entità</param>
    ''' <remarks></remarks>
    Private Function GetActionUrl(ByVal row As WorkflowActivityResult) As String

        Dim urlAction As String = String.Empty
        Dim JsonWorkflowOperation As WorkflowProperty = WorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(row.WorkflowActivityId, WorkflowPropertyHelper.DSW_PROPERTY_OPERATION)
        If JsonWorkflowOperation IsNot Nothing Then
            Dim op As WorkflowActivityOperation = JsonConvert.DeserializeObject(Of WorkflowActivityOperation)(JsonWorkflowOperation.ValueString, DocSuiteContext.DefaultWebAPIJsonSerializerSettings)
            Dim WorkflowArea As WorkflowActivityArea = op.Area
            Dim WorkflowAction As WorkflowActivityAction = op.Action
            urlAction = CreateUrl(row, op.Area, op.Action)
        End If
        Return urlAction
    End Function

    ''' <summary>
    ''' Funzione per la creazione dell'url 
    ''' </summary>
    ''' <param name="row">Riga della grid popolata dalle entità</param>
    ''' <param name="WorkflowAction">La WorkflowActivityAction viene recuperata dal json che è persisito sul db. Action è la azione compiuta dall'oggetto nell'area. Vedi WorkflowOperationConfig.json</param>
    ''' <param name="WorkflowArea">La WorkflowActivityArea viene recuperata dal json che è persisito sul db. Area è la area di azione della action. Vedi WorkflowOperationConfig.json</param>
    ''' <remarks></remarks>
    Private Function CreateUrl(ByVal row As WorkflowActivityResult, ByVal workflowArea As WorkflowActivityArea, ByVal workflowAction As WorkflowActivityAction) As String
        Dim securityCheck As String = String.Empty
        Dim protocolNumber As String = String.Empty
        Dim protocolYear As String = String.Empty
        Dim url As String = String.Empty
        Dim wfQueryString As String = String.Format("&IsWorkflowOperation=True&IdWorkflowActivity={0}", row.WorkflowActivityId)

        Select Case workflowArea
            Case WorkflowActivityArea.Protocol
                url = GetProtocolUrlByAction(workflowAction, row.WorkflowActivityId)
            Case WorkflowActivityArea.UDS
                url = GetUDSUrlByAction(workflowAction, row.WorkflowActivityId)
            Case WorkflowActivityArea.Collaboration
                url = GetCollaborationUrlByAction(workflowAction, row.WorkflowActivityId)
            Case WorkflowActivityArea.Fascicle
                url = GetFascicleUrlByAction(workflowAction, row.WorkflowActivityId)
            Case WorkflowActivityArea.Dossier
                url = GetDossierUrlByAction(workflowAction, row.WorkflowActivityId)
            Case WorkflowActivityArea.Desk
                url = GetDeskUrlByAction(workflowAction, row.WorkflowActivityId)
            Case Else
                Throw New DocSuiteException(String.Format("Nessuna Area configurata per il nome passato: {0}", workflowArea.GetDescription()))
        End Select

        If String.IsNullOrEmpty(url) Then
            FileLogger.Warn(LogName.WebAPIClientLog, String.Concat(row.WorkflowActivityId, " - ", row.WorkflowActivityName, " has empty url"), Nothing)
            Return String.Empty
        End If

        'for workflowarea = desk, not exist workflowaction=create => actionoptions=null'
        Dim actionOptions As IDictionary(Of String, String) = CommonInstance.GetWorkflowActionOptions(workflowArea, workflowAction)
        If actionOptions.Count <> 0 AndAlso actionOptions("SecurityCheck").Eq(Boolean.TrueString) Then
            securityCheck = GetSecurityCheck()
            url = String.Format("{0}&{1}", url, securityCheck)
        End If

        url = String.Concat(url, wfQueryString)
        Return url
    End Function

    ''' <summary>
    ''' Funzione per il calcolo del security check
    ''' </summary>
    ''' <remarks></remarks>
    Private Function GetSecurityCheck() As String
        Return CommonShared.AppendSecurityCheck("Action=Insert")
    End Function


    ''' <summary>
    ''' Imposto i valori di default dei filtri
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeFilters()
        trType.Visible = False
        trEnvironment.Visible = False
        'Dim sorted As Dictionary(Of Integer, String) = [Enum].GetValues(GetType(ActivityType)).OfType(Of ActivityType)() _
        '            .Where(Function(x) Not x.Equals(ActivityType.All)) _
        '            .Select(Function(s) New With {.Key = Integer.Parse(s), .Value = EnumHelper.GetDescription(s)}) _
        '            .OrderBy(Function(x) x.Value).ToDictionary(Function(d) d.Key, Function(d) d.Value)

        'For Each type As KeyValuePair(Of Integer, String) In sorted
        '    ddlType.Items.Add(New ListItem(type.Value, type.Key.ToString()))
        'Next
        'ddlType.Items.Insert(0, New ListItem(EnumHelper.GetDescription(ActivityType.All), Integer.Parse(ActivityType.All.ToString()).ToString()))
        'ddlType.SelectedIndex = ActivityType.All
        'Dim repositories As Dictionary(Of Integer, String) = Me.GetAvailableDSWEnvironment()
        'For Each type As KeyValuePair(Of Integer, String) In repositories
        '    ddlEnvironment.Items.Add(New ListItem(type.Value.ToString(), type.Key.ToString()))
        'Next
        'ddlEnvironment.Items.Insert(0, New ListItem(EnumHelper.GetDescription(DSWEnvironment.Any), DirectCast(DSWEnvironment.Any, Integer).ToString()))
        'ddlEnvironment.SelectedIndex = DSWEnvironment.Any

        rdbWfStatus.SelectedIndex = ProtocolEnv.DefaultStatusUserWorkflowFilter
        txtWfNameActivity.Text = String.Empty
        txtWfInstanceName.Text = String.Empty
        txtWfSubject.Text = String.Empty

    End Sub

    Private Function GetAvailableDSWEnvironment() As Dictionary(Of Integer, String)
        Dim results As Dictionary(Of Integer, String) = New Dictionary(Of Integer, String)
        If DocSuiteContext.Current.IsProtocolEnabled Then
            results.Add(DirectCast(DSWEnvironment.Protocol, Integer), EnumHelper.GetDescription(DSWEnvironment.Protocol))
        End If
        If DocSuiteContext.Current.IsResolutionEnabled Then
            results.Add(DirectCast(DSWEnvironment.Resolution, Integer), FacadeFactory.Instance.TabMasterFacade.TreeViewCaption)
        End If
        If DocSuiteContext.Current.ProtocolEnv.DocumentSeriesEnabled Then
            results.Add(DirectCast(DSWEnvironment.DocumentSeries, Integer), DocSuiteContext.Current.ProtocolEnv.DocumentSeriesName)
        End If
        If DocSuiteContext.Current.ProtocolEnv.UDSEnabled Then
            results.Add(100, "Archivi")
        End If
        results.Add(DirectCast(DSWEnvironment.Fascicle, Integer), EnumHelper.GetDescription(DSWEnvironment.Fascicle))
        Return results
    End Function


    ''' <summary>
    ''' Imposto i filtri per il finder
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetFinder()
        _finder.EnablePaging = True
        _finder.PageSize = DocSuiteContext.Current.ProtocolEnv.SearchMaxRecords
        _finder.SortExpressions.Add(New SortExpression(Of WorkflowActivity)() With {.Direction = SortDirection.Descending, .Expression = Function(x) x.RegistrationDate})
        If rdbWfStatus.SelectedItem IsNot Nothing AndAlso Not rdbWfStatus.SelectedValue.Eq("0") Then
            _finder.WorkflowActivityStatus = New List(Of WorkflowStatus)()
            _finder.WorkflowActivityStatus.Add(DirectCast(Short.Parse(rdbWfStatus.SelectedValue), WorkflowStatus))
        End If
        If rdbWfStatus.SelectedValue.Eq("0") Then
            _finder.RequestorUser = DocSuiteContext.Current.User.FullUserName
        End If
        If Not String.IsNullOrEmpty(txtWfNameActivity.Text) Then
            _finder.WorkflowActivityName = txtWfNameActivity.Text
        End If

        If Not String.IsNullOrEmpty(txtWfInstanceName.Text) Then
            _finder.WorkflowInstanceName = txtWfInstanceName.Text
        End If

        If Not String.IsNullOrEmpty(txtWfSubject.Text) Then
            _finder.WorkflowSubject = txtWfSubject.Text
        End If
    End Sub

    Private Function GetProtocolUrlByAction(workflowAction As WorkflowActivityAction, workflowActivityId As Guid) As String
        Select Case workflowAction
            Case WorkflowActivityAction.Create
                Dim wfPropertyYear As WorkflowProperty = WorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(workflowActivityId, WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_YEAR)
                Dim wfPropertyNumber As WorkflowProperty = WorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(workflowActivityId, WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_NUMBER)
                If wfPropertyYear IsNot Nothing AndAlso wfPropertyNumber IsNot Nothing Then
                    Return String.Format(PROTOCOL_VISUALIZZA_PATH, wfPropertyYear.ValueInt, wfPropertyNumber.ValueInt)
                End If
                Return CommonInstance.GetWorkflowActionPage(WorkflowActivityArea.Protocol, workflowAction)
                Exit Select

            Case WorkflowActivityAction.ToPEC
                Dim wfPropertyYear As WorkflowProperty = WorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(workflowActivityId, WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_YEAR)
                Dim wfPropertyNumber As WorkflowProperty = WorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(workflowActivityId, WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_NUMBER)
                If wfPropertyYear IsNot Nothing AndAlso wfPropertyNumber IsNot Nothing Then
                    Dim wfPropertyPecId As WorkflowProperty = WorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(workflowActivityId, WorkflowPropertyHelper.DSW_FIELD_PEC_ID)
                    If wfPropertyPecId IsNot Nothing Then
                        Dim tempUrl As String = String.Concat(PROTOCOL_VISUALIZZA_PATH, "&Action=FromPEC")
                        Return String.Format(tempUrl, wfPropertyYear.ValueInt, wfPropertyNumber.ValueInt)
                    Else
                        Dim actionPageFormat As String = CommonInstance.GetWorkflowActionPage(WorkflowActivityArea.Protocol, workflowAction)
                        Return String.Format(actionPageFormat, wfPropertyYear.ValueInt, wfPropertyNumber.ValueInt)
                    End If
                End If
                Return String.Empty
                Exit Select

            Case Else
                Return String.Empty
        End Select
    End Function

    Private Function GetFascicleUrlByAction(workflowAction As WorkflowActivityAction, workflowActivityId As Guid) As String
        Select Case workflowAction
            Case WorkflowActivityAction.ToAssignment
                Dim dsw_p_ReferenceModel As WorkflowProperty = WorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(workflowActivityId, WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL)
                If dsw_p_ReferenceModel IsNot Nothing Then
                    Dim workflowReferenceModel As WorkflowReferenceModel = JsonConvert.DeserializeObject(Of WorkflowReferenceModel)(dsw_p_ReferenceModel.ValueString)
                    If workflowReferenceModel IsNot Nothing AndAlso workflowReferenceModel.ReferenceId <> Guid.Empty Then
                        Return String.Format(FASCICLE_VISUALIZZA_PATH, workflowReferenceModel.ReferenceId)
                    Else
                        Return String.Empty
                    End If
                Else
                    Return String.Empty
                End If
            Case Else
                Return String.Empty
        End Select
    End Function

    Private Function GetDossierUrlByAction(workflowAction As WorkflowActivityAction, workflowactivityId As Guid) As String
        Select Case workflowAction
            Case WorkflowActivityAction.ToAssignment
                Dim dsw_p_ReferenceModel As WorkflowProperty = WorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(workflowactivityId, WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL)
                If dsw_p_ReferenceModel IsNot Nothing Then
                    Dim workflowReferenceModel As WorkflowReferenceModel = JsonConvert.DeserializeObject(Of WorkflowReferenceModel)(dsw_p_ReferenceModel.ValueString)
                    If workflowReferenceModel IsNot Nothing AndAlso workflowReferenceModel.ReferenceId <> Guid.Empty Then
                        Return String.Format(DOSSIER_VISUALIZZA_PATH, workflowReferenceModel.ReferenceId, workflowReferenceModel.Title)
                    Else
                        Return String.Empty
                    End If
                Else
                    Return String.Empty
                End If
            Case Else
                Return String.Empty
        End Select
    End Function

    Private Function GetDeskUrlByAction(workflowAction As WorkflowActivityAction, workflowactivityId As Guid) As String
        Select Case workflowAction
            Case WorkflowActivityAction.Create
                Return DESK_VISUALIZZA_PATH
            Case Else
                Return String.Empty
        End Select
    End Function

    Private Function GetCollaborationUrlByAction(workflowAction As WorkflowActivityAction, workflowActivityId As Guid) As String
        Select Case workflowAction
            Case WorkflowActivityAction.ToSign
                Dim wfPropertyCollaborationId As WorkflowProperty = WorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(workflowActivityId, WorkflowPropertyHelper.DSW_FIELD_COLLABORATION_ID)
                If wfPropertyCollaborationId Is Nothing OrElse Not wfPropertyCollaborationId.ValueInt.HasValue Then
                    Return String.Empty
                End If
                Dim actionPageFormat As String = CommonInstance.GetWorkflowActionPage(WorkflowActivityArea.Collaboration, workflowAction)
                Return String.Format(actionPageFormat, wfPropertyCollaborationId.ValueInt)
            Case WorkflowActivityAction.ToProtocol
                Dim wfPropertyCollaborationId As WorkflowProperty = WorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(workflowActivityId, WorkflowPropertyHelper.DSW_FIELD_COLLABORATION_ID)
                Dim wfPropertyUDSId As WorkflowProperty = WorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(workflowActivityId, WorkflowPropertyHelper.DSW_FIELD_UDS_ID)
                Dim wfPropertyUDSRepositoryId As WorkflowProperty = WorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(workflowActivityId, WorkflowPropertyHelper.DSW_FIELD_UDS_REPOSITORY_ID)
                If wfPropertyCollaborationId Is Nothing OrElse Not wfPropertyCollaborationId.ValueInt.HasValue Then
                    Return String.Empty
                End If

                If wfPropertyUDSId IsNot Nothing AndAlso wfPropertyUDSRepositoryId IsNot Nothing Then
                    Return String.Format(COLLABORATION_TO_PROTOCOL_UDS_PATH, wfPropertyCollaborationId.ValueInt, wfPropertyUDSId.ValueGuid, wfPropertyUDSRepositoryId.ValueGuid)
                End If

                Dim actionPageFormat As String = CommonInstance.GetWorkflowActionPage(WorkflowActivityArea.Collaboration, workflowAction)
                Return String.Format(actionPageFormat, wfPropertyCollaborationId.ValueInt)
            Case Else
                Return String.Empty
        End Select
    End Function

    Private Function GetUDSUrlByAction(workflowAction As WorkflowActivityAction, workflowActivityId As Guid) As String
        Select Case workflowAction
            Case WorkflowActivityAction.Create
                Dim wfPropertyUdsId As WorkflowProperty = WorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(workflowActivityId, WorkflowPropertyHelper.DSW_FIELD_UDS_ID)
                Dim wfPropertyUdsRepositoryId As WorkflowProperty = WorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(workflowActivityId, WorkflowPropertyHelper.DSW_FIELD_UDS_REPOSITORY_ID)
                If wfPropertyUdsId IsNot Nothing AndAlso wfPropertyUdsRepositoryId IsNot Nothing Then
                    Return String.Format(UDS_SUMMARY_PATH, wfPropertyUdsId.ValueGuid, wfPropertyUdsRepositoryId.ValueGuid)
                End If
                Return CommonInstance.GetWorkflowActionPage(WorkflowActivityArea.UDS, workflowAction)
                Exit Select

            Case WorkflowActivityAction.ToProtocol
                Dim wfPropertyUDSRepositoryName As WorkflowProperty = WorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(workflowActivityId, WorkflowPropertyHelper.DSW_FIELD_UDS_NAME)
                Dim wfPropertyUDSYear As WorkflowProperty = WorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(workflowActivityId, WorkflowPropertyHelper.DSW_FIELD_UDS_YEAR)
                Dim wfPropertyUDSNumber As WorkflowProperty = WorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(workflowActivityId, WorkflowPropertyHelper.DSW_FIELD_UDS_NUMBER)
                If wfPropertyUDSRepositoryName IsNot Nothing Then
                    Dim udsRepository As UDSRepository = CurrentUDSRepositoryFacade.GetMaxVersionByName(wfPropertyUDSRepositoryName.ValueString)
                    If udsRepository Is Nothing Then
                        Return String.Empty
                    End If
                    If wfPropertyUDSYear IsNot Nothing AndAlso wfPropertyUDSNumber IsNot Nothing Then
                        Dim protocolExist As Boolean = False
                        Dim dto As UDSDto = CurrentUDSFacade.GetUDSSource(udsRepository, String.Format(ODATA_FILTER, wfPropertyUDSYear.ValueInt.Value, wfPropertyUDSNumber.ValueInt.Value))
                        protocolExist = dto IsNot Nothing AndAlso dto.HasProtocol()
                        If protocolExist Then
                            Dim wfPropertyProtocolYear As WorkflowProperty = WorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(workflowActivityId, WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_YEAR)
                            Dim wfPropertyProtocolNumber As WorkflowProperty = WorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(workflowActivityId, WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_NUMBER)
                            If wfPropertyProtocolYear IsNot Nothing AndAlso wfPropertyProtocolNumber IsNot Nothing Then
                                Return String.Format(PROTOCOL_VISUALIZZA_PATH, wfPropertyProtocolYear.ValueInt.Value, wfPropertyProtocolNumber.ValueInt.Value)
                            End If
                        Else
                            Dim actionPageFormat As String = CommonInstance.GetWorkflowActionPage(WorkflowActivityArea.UDS, workflowAction)
                            Return String.Format(actionPageFormat, dto.Id, udsRepository.Id)
                        End If
                    End If
                End If
                Return String.Empty
                Exit Select

            Case WorkflowActivityAction.ToPEC
                Dim wfPropertyYear As WorkflowProperty = WorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(workflowActivityId, WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_YEAR)
                Dim wfPropertyNumber As WorkflowProperty = WorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(workflowActivityId, WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_NUMBER)
                If wfPropertyYear IsNot Nothing AndAlso wfPropertyNumber IsNot Nothing Then
                    Dim wfPropertyPecId As WorkflowProperty = WorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(workflowActivityId, WorkflowPropertyHelper.DSW_FIELD_PEC_ID)
                    If wfPropertyPecId IsNot Nothing Then
                        Dim tempUrl As String = String.Concat(PROTOCOL_VISUALIZZA_PATH, "&Action=FromPEC")
                        Return String.Format(tempUrl, wfPropertyYear.ValueInt, wfPropertyNumber.ValueInt)
                    Else
                        Dim actionPageFormat As String = CommonInstance.GetWorkflowActionPage(WorkflowActivityArea.UDS, workflowAction)
                        Return String.Format(actionPageFormat, wfPropertyYear.ValueInt, wfPropertyNumber.ValueInt)
                    End If
                End If
                Return String.Empty
                Exit Select

            Case Else
                Return String.Empty
        End Select

    End Function

    Private Sub ClearFilters(controls As ControlCollection)
        For Each item As Control In controls
            If item.Controls IsNot Nothing AndAlso item.Controls.Count > 0 Then
                Me.ClearFilters(item.Controls)
            End If

            Select Case True
                Case TypeOf item Is TextBox
                    Dim casted As TextBox = DirectCast(item, TextBox)
                    If casted.Enabled AndAlso casted.Visible Then
                        casted.Text = String.Empty
                    End If
                    Continue For

                Case TypeOf item Is CheckBox
                    Dim casted As CheckBox = DirectCast(item, CheckBox)
                    If casted.Enabled AndAlso casted.Visible Then
                        casted.Checked = False
                    End If
                    Continue For

                Case TypeOf item Is DropDownList
                    Dim casted As DropDownList = DirectCast(item, DropDownList)
                    If casted.Enabled AndAlso casted.Visible Then
                        casted.ClearSelection()
                    End If
                    Continue For

                Case TypeOf item Is RadioButtonList
                    Dim casted As RadioButtonList = DirectCast(item, RadioButtonList)
                    If casted.Enabled AndAlso casted.Visible Then
                        casted.ClearSelection()
                    End If
                    Continue For

                Case TypeOf item Is RadNumericTextBox
                    Dim casted As RadNumericTextBox = DirectCast(item, RadNumericTextBox)
                    If casted.Enabled AndAlso casted.Visible Then
                        casted.Text = String.Empty
                    End If
                    Continue For

                Case TypeOf item Is RadDropDownList
                    Dim casted As RadDropDownList = DirectCast(item, RadDropDownList)
                    If casted.Enabled AndAlso casted.Visible Then
                        casted.ClearSelection()
                    End If
                    Continue For

                Case TypeOf item Is uscContattiSel
                    Dim casted As uscContattiSel = DirectCast(item, uscContattiSel)
                    If Not casted.ReadOnly AndAlso casted.Visible Then
                        casted.DataSource.Clear()
                        casted.DataBind()
                    End If
                    Continue For

                Case TypeOf item Is uscClassificatore
                    Dim casted As uscClassificatore = DirectCast(item, uscClassificatore)
                    If Not casted.ReadOnly AndAlso casted.Visible Then
                        casted.Clear()
                        casted.DataBind()
                    End If
                    Continue For

                Case TypeOf item Is RadDatePicker
                    Dim casted As RadDatePicker = DirectCast(item, RadDatePicker)
                    If casted.Enabled AndAlso casted.Visible Then
                        casted.Clear()
                    End If
                    Continue For

                Case TypeOf item Is RadComboBox
                    Dim casted As RadComboBox = DirectCast(item, RadComboBox)
                    If casted.Enabled AndAlso casted.Visible Then
                        casted.ClearSelection()
                    End If
                    Continue For

                Case Else
            End Select
        Next
    End Sub

    Private Sub FillDataTable(detailTableView As GridTableView, dataItem As GridDataItem)
        Select Case detailTableView.Name
            Case "NestedWorkflowActivity"
                Dim workflowInstanceId As String = dataItem.GetDataKeyValue("WorkflowInstanceId").ToString()
                Dim workflowActivityId As String = dataItem.GetDataKeyValue("WorkflowActivityId").ToString()
                If Not String.IsNullOrEmpty(workflowInstanceId) AndAlso Not String.IsNullOrEmpty(workflowActivityId) Then
                    LoadNestedWorkflowActivities(Guid.Parse(workflowInstanceId), Guid.Parse(workflowActivityId), detailTableView)
                End If

                Exit Select
        End Select
    End Sub

    Private Sub LoadNestedWorkflowActivities(workflowInstanceId As Guid, workflowActivityId As Guid, detailTableView As GridTableView)
        _finder.ExcludeWorkflowActivityId = workflowActivityId
        _finder.WorkflowInstanceId = workflowInstanceId
        _finder.RequestorUser = DocSuiteContext.Current.User.FullUserName
        _finder.EnablePaging = False
        detailTableView.DataSource = _finder.DoSearchHeader()
    End Sub
#End Region

End Class

