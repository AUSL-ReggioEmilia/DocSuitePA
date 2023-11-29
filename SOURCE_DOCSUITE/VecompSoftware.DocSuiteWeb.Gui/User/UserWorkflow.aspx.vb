Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder
Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.Workflows
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.Workflows
Imports VecompSoftware.DocSuiteWeb.Entity.Workflows
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Workflow
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.UDS
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Commons
Imports VecompSoftware.DocSuiteWeb.Model.Workflow
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.ExtensionMethods.EnumEx
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers.Workflow
Imports VecompSoftware.Services.Logging
Imports WorkflowActivityAction = VecompSoftware.DocSuiteWeb.Model.Workflow.WorkflowActivityAction
Imports WorkflowActivityArea = VecompSoftware.DocSuiteWeb.Model.Workflow.WorkflowActivityArea
Imports WorkflowActivityType = VecompSoftware.DocSuiteWeb.Entity.Workflows.WorkflowActivityType

Public Class UserWorkflow
    Inherits WorkflowBasePage

    Dim _finder As New WorkflowActivityFinder(New MapperWorkflowActivity(), DocSuiteContext.Current.User.FullUserName)
    Private Const PROTOCOL_VISUALIZZA_PATH As String = "~/Prot/ProtVisualizza.aspx?UniqueId={0}&Type=Prot"
    Private Const UDS_SUMMARY_PATH As String = "~/UDS/UDSView.aspx?Type=UDS&IdUDS={0}&IdUDSRepository={1}"
    Private Const COLLABORATION_TO_PROTOCOL_UDS_PATH As String = "~/Prot/ProtInserimento.aspx?Type=Prot&Action=FromCollaboration&IdCollaboration={0}&IdUDS={1}&IdUDSRepository={2}"
    Private Const FASCICLE_VISUALIZZA_PATH As String = "~/Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle={0}"
    Private Const DOSSIER_VISUALIZZA_PATH As String = "~/Dossiers/DossierVisualizza.aspx?Type=Dossier&IdDossier={0}&DossierTitle={1}"
    Private Const DESK_VISUALIZZA_PATH As String = "~/Workflows/WorkflowActivitySummary.aspx?"
    Public Const UDS_ADDRESS_NAME As String = "API-UDSAddress"
    Private Const ODATA_FILTER As String = "$filter=_year eq {0} and _number eq {1}"
    Private _currentUDSFacade As UDSFacade
    Private _currentWorkflowPropertyFacade As Facade.WebAPI.Workflows.WorkflowPropertyFacade
    Private _currentWorkflowRepositoryFacade As Facade.WebAPI.Workflows.WorkflowRepositoryFacade
#Region "Property"

    Public ReadOnly Property CurrentUDSFacade As UDSFacade
        Get
            If _currentUDSFacade Is Nothing Then
                _currentUDSFacade = New UDSFacade()
            End If
            Return _currentUDSFacade
        End Get
    End Property

    Public ReadOnly Property CurrentWorkflowPropertyFacade As Facade.WebAPI.Workflows.WorkflowPropertyFacade
        Get
            If _currentWorkflowPropertyFacade Is Nothing Then
                _currentWorkflowPropertyFacade = New WebAPI.Workflows.WorkflowPropertyFacade(DocSuiteContext.Current.Tenants, CurrentTenant)
            End If
            Return _currentWorkflowPropertyFacade
        End Get
    End Property
    Public ReadOnly Property CurrentWorkflowRepositoryFacade As Facade.WebAPI.Workflows.WorkflowRepositoryFacade
        Get
            If _currentWorkflowRepositoryFacade Is Nothing Then
                _currentWorkflowRepositoryFacade = New WebAPI.Workflows.WorkflowRepositoryFacade(DocSuiteContext.Current.Tenants, CurrentTenant)
            End If
            Return _currentWorkflowRepositoryFacade
        End Get
    End Property

    Private ReadOnly Property RepositoryName As String
        Get
            Return Request.QueryString.GetValueOrDefault("RepositoryName", String.Empty)
        End Get
    End Property

#End Region

#Region " Fields"

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

        If Not rdpDateFilterTo.SelectedDate.HasValue Then
            rdpDateFilterTo.SelectedDate = rdpDateFilterFrom.SelectedDate
        End If

        If Not Validations() Then
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

            Dim workflowProperty As WorkflowProperty = CurrentWorkflowPropertyFacade.FindPropertyByActivityIdAndName(workflowActivityResult.WorkflowActivityId, WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_END_MOTIVATION)
            If workflowProperty IsNot Nothing AndAlso String.IsNullOrEmpty(workflowProperty.ValueString) = False Then
                workflowActivityResult.WorkflowSubject = workflowProperty.ValueString
            End If

            With DirectCast(e.Item.FindControl("lblWorkflowSubject"), Label)
                .Text = workflowActivityResult.WorkflowSubject
            End With

            Dim lnkWorkflowActivityName As HyperLink = DirectCast(e.Item.FindControl("lnkWorkflowActivityName"), HyperLink)
            lnkWorkflowActivityName.NavigateUrl = GetActionUrl(workflowActivityResult)
            lnkWorkflowActivityName.Text = workflowActivityResult.WorkflowActivityName
            If String.IsNullOrEmpty(lnkWorkflowActivityName.NavigateUrl) Then
                lnkWorkflowActivityName.Text = $"< attività con anomalie - {workflowActivityResult.WorkflowActivityId} - contattare assistenza >"
                lnkWorkflowActivityName.Enabled = False
            End If

            Dim role As Model.Workflow.WorkflowRole = Nothing
            Dim dsw_p_ProposerRole As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_ROLE, workflowActivityResult.WorkflowActivityId)
            If dsw_p_ProposerRole IsNot Nothing AndAlso Not String.IsNullOrEmpty(dsw_p_ProposerRole.ValueString) Then
                role = JsonConvert.DeserializeObject(Of Model.Workflow.WorkflowRole)(dsw_p_ProposerRole.ValueString, DocSuiteContext.DefaultWebAPIJsonSerializerSettings)
            End If
            If role IsNot Nothing AndAlso role.Name IsNot Nothing Then
                With DirectCast(e.Item.FindControl("lblWorkflowProposerRoleName"), Label)
                    .Text = role.Name
                End With
            End If

            Dim roleReceiver As Role = Nothing
            Dim dsw_p_Roles As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_PROPERTY_ROLES, workflowActivityResult.WorkflowActivityId)
            If dsw_p_Roles IsNot Nothing AndAlso Not String.IsNullOrEmpty(dsw_p_Roles.ValueString) Then
                Dim roles As IList(Of Model.Workflow.WorkflowMapping) = JsonConvert.DeserializeObject(Of IList(Of Model.Workflow.WorkflowMapping))(dsw_p_Roles.ValueString, DocSuiteContext.DefaultWebAPIJsonSerializerSettings)
                Dim dest As Model.Workflow.WorkflowMapping = roles.FirstOrDefault()
                If dest IsNot Nothing AndAlso dest.Role IsNot Nothing Then
                    roleReceiver = Facade.RoleFacade.GetById(dest.Role.IdRole)
                End If
            End If
            If roleReceiver IsNot Nothing AndAlso roleReceiver.Name IsNot Nothing Then
                Dim lblWorkflowReceiverRoleName As Label = DirectCast(e.Item.FindControl("lblWorkflowReceiverRoleName"), Label)
                lblWorkflowReceiverRoleName.Text = roleReceiver.Name
            End If
            If workflowActivityResult.WorkflowActivityType = WorkflowActivityType.GenericActivity Then
                With DirectCast(e.Item.FindControl("lblWorkflowProposerRoleName"), Label)
                    .Text = workflowActivityResult.WorkflowActivityRequestorUser
                End With
                Dim dsw_p_Accounts As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS, workflowActivityResult.WorkflowActivityId)
                Dim dsw_e_RecipientPosition As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_FIELD_RECIPIENT_POSITION, workflowActivityResult.WorkflowActivityId)
                Dim accounts As IList(Of WorkflowAccount) = JsonConvert.DeserializeObject(Of IList(Of Model.Workflow.WorkflowAccount))(dsw_p_Accounts.ValueString, DocSuiteContext.DefaultWebAPIJsonSerializerSettings)
                Dim lblWorkflowReceiverRoleName As Label = DirectCast(e.Item.FindControl("lblWorkflowReceiverRoleName"), Label)
                Dim position As Integer = 0
                If (dsw_e_RecipientPosition IsNot Nothing AndAlso dsw_e_RecipientPosition.ValueInt.HasValue) Then
                    position = Convert.ToInt32(dsw_e_RecipientPosition.ValueInt.Value)
                End If

                lblWorkflowReceiverRoleName.Text = accounts.ElementAtOrDefault(position)?.AccountName
            End If

            If rdbWfStatus.SelectedValue = "2" Then
                Dim handler As WorkflowAuthorization = GetWorkflowAuthorizationHandler(workflowActivityResult.WorkflowActivityId)
                If handler IsNot Nothing AndAlso Not handler.Account.IsNullOrEmpty() Then
                    Dim lblWorkflowIsHandler As Label = DirectCast(e.Item.FindControl("lblWorkflowIsHandler"), Label)
                    lblWorkflowIsHandler.Text = CommonAD.GetDisplayName(handler.Account)
                End If
            End If

            Dim dsw_e_ProductName As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_FIELD_PRODUCT_NAME, workflowActivityResult.WorkflowActivityId)
            If dsw_e_ProductName IsNot Nothing Then
                Dim imgWorkflowActivityStart As Image = DirectCast(e.Item.FindControl("imgWorkflowActivityStart"), Image)
                If imgWorkflowActivityStart IsNot Nothing Then
                    imgWorkflowActivityStart.Visible = False
                    Dim jsonWorkflowStartFromValue As String = String.Empty
                    If Not _workflowImgPath Is Nothing AndAlso _workflowImgPath.Keys.Any(Function(f) f = dsw_e_ProductName.ValueString) Then
                        imgWorkflowActivityStart.Visible = True
                        imgWorkflowActivityStart.ImageUrl = _workflowImgPath(dsw_e_ProductName.ValueString)
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub cmdClearFilters_Click(sender As Object, e As EventArgs) Handles btnClearFilters.Click
        Me.ClearFilters(Me.searchTable.Controls)
        InitializeFilters()
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(Me.btnClearFilters, Me.searchTable)
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
        Dim dsw_p_Operation As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_PROPERTY_OPERATION, row.WorkflowActivityId)
        If dsw_p_Operation IsNot Nothing Then
            Dim op As WorkflowActivityOperation = JsonConvert.DeserializeObject(Of WorkflowActivityOperation)(dsw_p_Operation.ValueString, DocSuiteContext.DefaultWebAPIJsonSerializerSettings)
            Dim WorkflowArea As WorkflowActivityArea = op.Area
            Dim WorkflowAction As WorkflowActivityAction = op.Action
            urlAction = CreateUrl(row, op.Area, op.Action)
        End If
        Return urlAction
    End Function

    Private Function Validations() As Boolean
        If Not rdpDateFilterFrom.SelectedDate.HasValue Then
            AjaxAlert("Manca data Inizio")
            Return False
        End If

        If Not rdpDateFilterTo.SelectedDate.HasValue Then
            AjaxAlert("Manca data Fine")
            Return False
        End If
        If rdpDateFilterFrom.SelectedDate.Value > rdpDateFilterTo.SelectedDate.Value Then
            AjaxAlert("Il range di date non è valido. Verificare se la data di fine è superiore della data di inizio.")
            Return False
        End If
        Return True
    End Function


    ''' <summary>
    ''' Funzione per la creazione dell'url 
    ''' </summary>
    ''' <param name="row">Riga della grid popolata dalle entità</param>
    ''' <param name="WorkflowAction">La WorkflowActivityAction viene recuperata dal json che è persisito sul db. Action è la azione compiuta dall'oggetto nell'area. Vedi WorkflowOperationConfig.json</param>
    ''' <param name="WorkflowArea">La WorkflowActivityArea viene recuperata dal json che è persisito sul db. Area è la area di azione della action. Vedi WorkflowOperationConfig.json</param>
    ''' <remarks></remarks>
    Private Function CreateUrl(ByVal row As WorkflowActivityResult, ByVal workflowArea As WorkflowActivityArea, ByVal workflowAction As WorkflowActivityAction) As String
        Dim wfQueryString As String = $"&IsWorkflowOperation=True&IdWorkflowActivity={row.WorkflowActivityId}"
        FileLogger.Debug(LogName.FileLog, $"Workflow.CreateUrl {row.WorkflowActivityId} {workflowArea} {workflowAction}")
        Dim url As String
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
            Case WorkflowActivityArea.DocSuiteNext
                url = GetDocsuiteNextByAction(workflowAction, row.WorkflowActivityId)
            Case Else
                Throw New DocSuiteException(String.Format("Nessuna Area configurata per il nome passato: {0}", workflowArea.GetDescription()))
        End Select

        If String.IsNullOrEmpty(url) Then
            FileLogger.Warn(LogName.FileLog, String.Concat(row.WorkflowActivityId, " - ", row.WorkflowActivityName, " has empty url"), Nothing)
            Return String.Empty
        End If
        FileLogger.Debug(LogName.FileLog, $"url is {url}")
        'for workflowarea = desk, not exist workflowaction=create => actionoptions=null'
        Dim actionOptions As IDictionary(Of String, String) = CommonInstance.GetWorkflowActionOptions(workflowArea, workflowAction)
        If actionOptions.Count <> 0 AndAlso actionOptions("SecurityCheck").Eq(Boolean.TrueString) Then
            Dim securityCheck As String = GetSecurityCheck()
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
        trType.Visible = True
        trEnvironment.Visible = False

        rdpDateFilterFrom.SelectedDate = DateTime.Today.AddDays(-ProtocolEnv.DesktopDayDiff).Date
        rdpDateFilterTo.SelectedDate = DateTime.Today.Date.AddDays(1).AddMilliseconds(-1)

        rdbWfStatus.SelectedIndex = ProtocolEnv.DefaultStatusUserWorkflowFilter
        txtWfNameActivity.Text = String.Empty
        txtWfInstanceName.Text = String.Empty
        txtWfSubject.Text = String.Empty

        ddlType.Enabled = True

        LoadWorkflowRepositories()
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


    Private Sub LoadWorkflowRepositories()
        ddlType.Items.Clear()
        ddlType.Items.Add(New RadComboBoxItem(String.Empty, String.Empty))
        Dim selectedItem As RadComboBoxItem = Nothing
        Dim workflowRepositories As ICollection(Of WorkflowRepository)
        Try
            workflowRepositories = CurrentWorkflowRepositoryFacade.GetAllUserWorkflowRepository()
        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            AjaxAlert("Errore in caricamento lista attività.")
            Exit Sub
        End Try

        For Each repository As WorkflowRepository In workflowRepositories
            Dim item As New RadComboBoxItem(repository.Name, repository.UniqueId.ToString())
            ddlType.Items.Add(item)

            If repository.Name = RepositoryName AndAlso selectedItem Is Nothing Then
                selectedItem = item
            End If
        Next

        If selectedItem IsNot Nothing Then
            ddlType.SelectedValue = selectedItem.Value
        End If
    End Sub

    ''' <summary>
    ''' Imposto i filtri per il finder
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetFinder()
        _finder.EnablePaging = True
        _finder.PageSize = DocSuiteContext.Current.ProtocolEnv.SearchMaxRecords
        _finder.SortExpressions.Add(New SortExpression(Of Data.Entity.Workflows.WorkflowActivity)() With {.Direction = SortDirection.Descending, .Expression = Function(x) x.RegistrationDate})
        _finder.IdTenant = CurrentTenant.UniqueId
        If rdbWfStatus.SelectedItem IsNot Nothing AndAlso Not rdbWfStatus.SelectedValue.Eq("0") Then
            _finder.WorkflowActivityStatus = New List(Of Data.Entity.Workflows.WorkflowStatus)()
            _finder.WorkflowActivityStatus.Add(DirectCast(Short.Parse(rdbWfStatus.SelectedValue), Data.Entity.Workflows.WorkflowStatus))
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

        If rdpDateFilterFrom.SelectedDate.HasValue Then
            _finder.WorkflowDateFrom = rdpDateFilterFrom.SelectedDate.Value.Date
        End If

        If rdpDateFilterTo.SelectedDate.HasValue Then
            _finder.WorkflowDateTo = rdpDateFilterTo.SelectedDate.Value.Date.AddDays(1)
        End If

        If ddlType.SelectedItem IsNot Nothing AndAlso Not String.IsNullOrEmpty(ddlType.SelectedItem.Value) Then
            _finder.WorkflowRepositoryUniqueId = Guid.Parse(ddlType.SelectedItem.Value)
        End If

        _finder.IsVisible = True
        wfGrid.MasterTableView.GroupByExpressions.Clear()
        'If rdbViewer.SelectedValue = "1" Then
        '    Dim expression As GridGroupByExpression = New GridGroupByExpression()
        '    Dim gridGroupByField As GridGroupByField = New GridGroupByField()
        '    gridGroupByField.FieldName = "WorkflowInstanceId"
        '    gridGroupByField.HeaderText = "Raggruppa"
        '    expression.GroupByFields.Add(gridGroupByField)
        '    wfGrid.MasterTableView.GroupByExpressions.Add(expression)
        'End If
    End Sub

    Private Function GetProtocolUrlByAction(workflowAction As WorkflowActivityAction, workflowActivityId As Guid) As String
        Select Case workflowAction
            Case WorkflowActivityAction.Create
                Dim dsw_e_ProtocolUniqueId As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_UNIQUEID, workflowActivityId)
                If dsw_e_ProtocolUniqueId IsNot Nothing AndAlso dsw_e_ProtocolUniqueId.ValueGuid.HasValue Then
                    FileLogger.Debug(LogName.FileLog, $"Workflow.GetProtocolUrlByAction {dsw_e_ProtocolUniqueId.ValueGuid}")
                    Return String.Format(PROTOCOL_VISUALIZZA_PATH, dsw_e_ProtocolUniqueId.ValueGuid.Value)
                End If
                FileLogger.Debug(LogName.FileLog, $"Workflow.GetProtocolUrlByAction {workflowAction}")
                Return CommonInstance.GetWorkflowActionPage(WorkflowActivityArea.Protocol, workflowAction)
                Exit Select

            Case WorkflowActivityAction.ToPEC
                Dim dsw_e_ProtocolUniqueId As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_UNIQUEID, workflowActivityId)
                If dsw_e_ProtocolUniqueId IsNot Nothing AndAlso dsw_e_ProtocolUniqueId.ValueGuid.HasValue Then
                    Dim dsw_e_PECId As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_FIELD_PEC_ID, workflowActivityId)
                    If dsw_e_PECId IsNot Nothing Then
                        Dim tempUrl As String = String.Concat(PROTOCOL_VISUALIZZA_PATH, "&Action=FromPEC")
                        FileLogger.Debug(LogName.FileLog, $"Workflow.GetProtocolUrlByAction {dsw_e_ProtocolUniqueId.ValueGuid}")
                        Return String.Format(tempUrl, dsw_e_ProtocolUniqueId.ValueGuid.Value)
                    Else
                        Dim actionPageFormat As String = CommonInstance.GetWorkflowActionPage(WorkflowActivityArea.Protocol, workflowAction)
                        FileLogger.Debug(LogName.FileLog, $"Workflow.GetProtocolUrlByAction {workflowAction}")
                        Return String.Format(actionPageFormat, dsw_e_ProtocolUniqueId.ValueGuid.Value)
                    End If
                End If
                Return String.Empty
                Exit Select

            Case WorkflowActivityAction.ToProtocol
                Dim dsw_p_ReferenceModel As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, workflowActivityId)
                If dsw_p_ReferenceModel IsNot Nothing Then
                    Dim workflowReferenceModel As WorkflowReferenceModel = JsonConvert.DeserializeObject(Of WorkflowReferenceModel)(dsw_p_ReferenceModel.ValueString)
                    If workflowReferenceModel IsNot Nothing AndAlso workflowReferenceModel.ReferenceId <> Guid.Empty Then
                        Return String.Format(PROTOCOL_VISUALIZZA_PATH, workflowReferenceModel.ReferenceId)
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

    Private Function GetFascicleUrlByAction(workflowAction As WorkflowActivityAction, workflowActivityId As Guid) As String
        Select Case workflowAction
            Case WorkflowActivityAction.Authorize
                Return DESK_VISUALIZZA_PATH
            Case WorkflowActivityAction.ToAssignment
                Dim dsw_p_ReferenceModel As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, workflowActivityId)
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
                Dim dsw_p_ReferenceModel As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, workflowactivityId)
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

    Private Function GetDocsuiteNextByAction(workflowAction As WorkflowActivityAction, workflowactivityId As Guid) As String
        Select Case workflowAction
            Case WorkflowActivityAction.ToProtocol
                Dim dsw_a_RedirectToPECToDocumentUnit As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_ACTION_REDIRECT_PEC_DOCUMENTUNIT, workflowactivityId)
                If dsw_a_RedirectToPECToDocumentUnit Is Nothing OrElse Not dsw_a_RedirectToPECToDocumentUnit.ValueBoolean.HasValue Then
                    Return String.Empty
                End If

                Dim dsw_p_ReferenceModel As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, workflowactivityId)
                If dsw_p_ReferenceModel Is Nothing OrElse String.IsNullOrEmpty(dsw_p_ReferenceModel.ValueString) Then
                    Return String.Empty
                End If

                Dim workflowReferenceModel As WorkflowReferenceModel = JsonConvert.DeserializeObject(Of WorkflowReferenceModel)(dsw_p_ReferenceModel.ValueString)
                If workflowReferenceModel Is Nothing OrElse workflowReferenceModel.ReferenceType <> DSWEnvironmentType.PECMail Then
                    Return String.Empty
                End If

                Dim pecMail As Entity.PECMails.PECMail = JsonConvert.DeserializeObject(Of Entity.PECMails.PECMail)(workflowReferenceModel.ReferenceModel)
                If pecMail.ProcessStatus <> PECMailProcessStatus.ArchivedInDocSuiteNext Then
                    Throw New DocSuiteException(String.Format("PECMail (id:{0}) ProcessStatus non compatibile con ActivityArea DocSuiteNext e ActivityAction ToProtocol. (atteso {1}, ricevuto {2})", pecMail.UniqueId, PECMailProcessStatus.ArchivedInDocSuiteNext, pecMail.ProcessStatus))
                End If

                Return String.Format("~/Pec/PECToDocumentUnit.aspx?{0}", CommonShared.AppendSecurityCheck($"isInWindow=true&Type=Pec&PECId={pecMail.EntityId}"))
            Case Else
                Return String.Empty
        End Select
    End Function

    Private Function GetCollaborationUrlByAction(workflowAction As WorkflowActivityAction, workflowActivityId As Guid) As String
        Select Case workflowAction
            Case WorkflowActivityAction.ToSign
                Dim dsw_e_CollaborationId As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_FIELD_COLLABORATION_ID, workflowActivityId)
                If dsw_e_CollaborationId Is Nothing OrElse Not dsw_e_CollaborationId.ValueInt.HasValue Then
                    Return String.Empty
                End If
                Dim actionPageFormat As String = CommonInstance.GetWorkflowActionPage(WorkflowActivityArea.Collaboration, workflowAction)
                FileLogger.Debug(LogName.FileLog, $"Workflow.GetCollaborationUrlByAction {dsw_e_CollaborationId.ValueInt}")
                Return String.Format(actionPageFormat, dsw_e_CollaborationId.ValueInt)
            Case WorkflowActivityAction.ToProtocol
                Dim dsw_e_CollaborationId As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_FIELD_COLLABORATION_ID, workflowActivityId)
                If dsw_e_CollaborationId Is Nothing OrElse Not dsw_e_CollaborationId.ValueInt.HasValue Then
                    Return String.Empty
                End If

                Dim dsw_e_ProtocolUniqueId As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_UNIQUEID, workflowActivityId)
                If dsw_e_ProtocolUniqueId IsNot Nothing AndAlso dsw_e_ProtocolUniqueId.ValueGuid.HasValue Then
                    FileLogger.Debug(LogName.FileLog, $"Workflow.GetCollaborationUrlByAction {dsw_e_ProtocolUniqueId.ValueGuid}")
                    Return String.Format(PROTOCOL_VISUALIZZA_PATH, dsw_e_ProtocolUniqueId.ValueGuid.Value)
                End If

                Dim dsw_e_UDSId As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_FIELD_UDS_ID, workflowActivityId)
                Dim dsw_e_UDSRepositoryId As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_FIELD_UDS_REPOSITORY_ID, workflowActivityId)
                If dsw_e_UDSId IsNot Nothing AndAlso dsw_e_UDSRepositoryId IsNot Nothing Then
                    FileLogger.Debug(LogName.FileLog, $"Workflow.GetCollaborationUrlByAction {dsw_e_CollaborationId.ValueInt} {dsw_e_UDSId.ValueGuid} {dsw_e_UDSRepositoryId.ValueGuid}")
                    Return String.Format(COLLABORATION_TO_PROTOCOL_UDS_PATH, dsw_e_CollaborationId.ValueInt, dsw_e_UDSId.ValueGuid, dsw_e_UDSRepositoryId.ValueGuid)
                End If

                FileLogger.Debug(LogName.FileLog, $"Workflow.GetCollaborationUrlByAction {dsw_e_CollaborationId.ValueInt}")
                Dim actionPageFormat As String = CommonInstance.GetWorkflowActionPage(WorkflowActivityArea.Collaboration, workflowAction)
                Return String.Format(actionPageFormat, dsw_e_CollaborationId.ValueInt)
            Case Else
                Return String.Empty
        End Select
    End Function

    Private Function GetUDSUrlByAction(workflowAction As WorkflowActivityAction, workflowActivityId As Guid) As String
        Select Case workflowAction
            Case WorkflowActivityAction.Create
                Dim dsw_e_UDSId As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_FIELD_UDS_ID, workflowActivityId)
                Dim dsw_e_UDSRepositoryId As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_FIELD_UDS_REPOSITORY_ID, workflowActivityId)
                If dsw_e_UDSId IsNot Nothing AndAlso dsw_e_UDSRepositoryId IsNot Nothing Then
                    FileLogger.Debug(LogName.FileLog, $"Workflow.GetUDSUrlByAction {dsw_e_UDSId.ValueGuid} {dsw_e_UDSRepositoryId.ValueGuid}")
                    Return String.Format(UDS_SUMMARY_PATH, dsw_e_UDSId.ValueGuid, dsw_e_UDSRepositoryId.ValueGuid)
                End If
                FileLogger.Debug(LogName.FileLog, $"Workflow.GetUDSUrlByAction {workflowAction}")
                Return CommonInstance.GetWorkflowActionPage(WorkflowActivityArea.UDS, workflowAction)
                Exit Select

            Case WorkflowActivityAction.ToProtocol
                Dim dsw_e_UDSName As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_FIELD_UDS_NAME, workflowActivityId)
                Dim dsw_e_UDSYear As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_FIELD_UDS_YEAR, workflowActivityId)
                Dim dsw_e_UDSNumber As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_FIELD_UDS_NUMBER, workflowActivityId)
                If dsw_e_UDSName IsNot Nothing Then
                    Dim udsRepository As UDSRepository = CurrentUDSRepositoryFacade.GetMaxVersionByName(dsw_e_UDSName.ValueString)
                    If udsRepository Is Nothing Then
                        Return String.Empty
                    End If
                    If dsw_e_UDSYear IsNot Nothing AndAlso dsw_e_UDSNumber IsNot Nothing Then
                        Dim protocolExist As Boolean = False
                        Dim dto As UDSDto = CurrentUDSFacade.GetUDSSource(udsRepository, String.Format(ODATA_FILTER, dsw_e_UDSYear.ValueInt.Value, dsw_e_UDSNumber.ValueInt.Value))
                        protocolExist = dto IsNot Nothing AndAlso dto.HasProtocol()
                        If protocolExist Then
                            Dim dsw_e_ProtocolUniqueId As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_UNIQUEID, workflowActivityId)
                            If dsw_e_ProtocolUniqueId IsNot Nothing AndAlso dsw_e_ProtocolUniqueId.ValueGuid.HasValue Then
                                FileLogger.Debug(LogName.FileLog, $"Workflow.GetUDSUrlByAction {dsw_e_ProtocolUniqueId.ValueGuid}")
                                Return String.Format(PROTOCOL_VISUALIZZA_PATH, dsw_e_ProtocolUniqueId.ValueGuid.Value)
                            End If
                        Else
                            Dim actionPageFormat As String = CommonInstance.GetWorkflowActionPage(WorkflowActivityArea.UDS, workflowAction)
                            FileLogger.Debug(LogName.FileLog, $"Workflow.GetUDSUrlByAction {workflowAction} {dto.Id} {udsRepository.Id}")
                            Return String.Format(actionPageFormat, dto.Id, udsRepository.Id)
                        End If
                    End If
                End If
                Return String.Empty
                Exit Select

            Case WorkflowActivityAction.ToPEC
                Dim dsw_e_ProtocolUniqueId As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_FIELD_PROTOCOL_UNIQUEID, workflowActivityId)
                If dsw_e_ProtocolUniqueId IsNot Nothing AndAlso dsw_e_ProtocolUniqueId.ValueGuid.HasValue Then
                    Dim dsw_e_PECId As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_FIELD_PEC_ID, workflowActivityId)
                    If dsw_e_PECId IsNot Nothing Then
                        Dim tempUrl As String = String.Concat(PROTOCOL_VISUALIZZA_PATH, "&Action=FromPEC")
                        FileLogger.Debug(LogName.FileLog, $"Workflow.GetUDSUrlByAction {dsw_e_ProtocolUniqueId.ValueGuid}")
                        Return String.Format(tempUrl, dsw_e_ProtocolUniqueId.ValueGuid.Value)
                    Else
                        Dim actionPageFormat As String = CommonInstance.GetWorkflowActionPage(WorkflowActivityArea.UDS, workflowAction)
                        FileLogger.Debug(LogName.FileLog, $"Workflow.GetUDSUrlByAction UDS {dsw_e_ProtocolUniqueId.ValueGuid}")
                        Return String.Format(actionPageFormat, dsw_e_ProtocolUniqueId.ValueGuid.Value)
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

#End Region

End Class

