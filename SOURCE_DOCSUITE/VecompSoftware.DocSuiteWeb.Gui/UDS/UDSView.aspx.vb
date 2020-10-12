Imports System.Collections.Generic
Imports System.Text
Imports System.Web
Imports System.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.BusinessRule.Rules.Rights.UDS
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.UDS
Imports VecompSoftware.DocSuiteWeb.Model.Workflow
Imports VecompSoftware.Helpers.UDS
Imports VecompSoftware.Helpers.WebAPI
Imports VecompSoftware.Helpers.Workflow
Imports VecompSoftware.Services.Logging
Imports WebApientity = VecompSoftware.DocSuiteWeb.Entity.UDS
Imports WebApiFacade = VecompSoftware.DocSuiteWeb.Facade.WebAPI.UDS
Imports VecompSoftware.DocSuiteWeb.Entity.Workflows

Public Class UDSView
    Inherits UDSBasePage
    Implements IProtocolInitializer
    Implements IUDSInitializer
#Region "Fields"
    Private _udsSource As UDSDto
    Private _currentRepositoryRigths As UDSRepositoryRightsUtil
    Public Const EDIT_PAGE_URL As String = "~/UDS/UDSInsert.aspx?Type=UDS&Action=Edit&IdUDS={0}&IdUDSRepository={1}&Callback={2}"
    Public Const VIEWER_PAGE_URL As String = "~/Viewers/UDSViewer.aspx?IdUDS={0}&IdUDSRepository={1}"
    Private Const START_WORKFLOW_WINDOW_SCRIPT As String = "startWorkflowWindow"
    Private Const CLOSE_WORKFLOW_WINDOW_SCRIPT As String = "closeStartWorkflowWindow();"
    Private Const ON_ERROR_FUNCTION As String = "onError('{0}')"
    Private Const ON_DONE_FUNCTION As String = "onDone('{0}')"
    Public Const NOTIFICATION_SUCCESS_ICON As String = "ok"
    Public Const CANCEL_COMMAND_ARGUMENT As String = "Attenzione! Confermi la procedura di annullamento dell'archivio corrente?"
    Private _udsLogFacade As WebApiFacade.UDSLogFacade
    Private ReadOnly _udsUserFinder As UDSUserFinder
    Private ReadOnly _isCurrentUserAuthorized As Boolean?
    Private ReadOnly _authorizedUsers As ICollection(Of WebApientity.UDSUser)
#End Region

#Region "Properties"

    Public ReadOnly Property UDSSource As UDSDto
        Get
            If _udsSource Is Nothing Then
                _udsSource = GetSource()
            End If
            Return _udsSource
        End Get
    End Property


    Private ReadOnly Property CurrentRepositoryRights As UDSRepositoryRightsUtil
        Get
            If _currentRepositoryRigths Is Nothing Then
                _currentRepositoryRigths = New UDSRepositoryRightsUtil(CurrentUDSRepository, DocSuiteContext.Current.User.FullUserName, UDSSource)
            End If
            Return _currentRepositoryRigths
        End Get
    End Property

    Public ReadOnly Property IsWorkflowEnabled As Boolean
        Get
            Return CurrentRepositoryRights.IsWorkflowEnabled AndAlso CurrentRepositoryRights.IsDocumentsViewable
        End Get
    End Property

    Public ReadOnly Property CurrentUserWorkflowActivityId As Guid?
        Get
            Return If(ProtocolEnv.WorkflowManagerEnabled, CurrentRepositoryRights.CurrentUserWorkflowActivity, Nothing)
        End Get
    End Property

    Public ReadOnly Property UDSLogFacade As WebApiFacade.UDSLogFacade
        Get
            If _udsLogFacade Is Nothing Then
                _udsLogFacade = New WebApiFacade.UDSLogFacade(DocSuiteContext.Current.Tenants.ToList(), CurrentTenant)
            End If
            Return _udsLogFacade
        End Get
    End Property
#End Region

#Region "Events"

    Protected Sub UscUDS_NeedRepositorySource(sender As Object, e As EventArgs) Handles uscUDS.NeedRepositorySource
        uscUDS.CurrentUDSRepositoryId = CurrentIdUDSRepository
        uscUDS.UDSItemSource = UDSSource.UDSModel
        uscUDS.UDSYear = UDSSource.Year
        uscUDS.UDSNumber = UDSSource.Number
        uscUDS.UDSRegistrationDate = UDSSource.RegistrationDate
        uscUDS.UDSRegistrationUser = UDSSource.RegistrationUser
        uscUDS.UDSLastChangedDate = UDSSource.LastChangedDate
        uscUDS.UDSLastChangedUser = UDSSource.LastChangedUser
        uscUDS.UDSCategory = UDSSource.Category
        uscUDS.UDSSubject = UDSSource.Subject
        uscUDS.UDSAuthorizations = UDSSource.Authorizations
        uscUDS.UDSMessages = UDSSource.Messages
        uscUDS.UDSId = UDSSource.Id
        uscUDS.RepositoryBind()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        uscUDS.HasEditableRight = CurrentRepositoryRights.IsEditable
        uscUDS.HasActiveWorkflowActivity = CurrentRepositoryRights.CurrentUserWorkflowActivity.HasValue

        AddHandler MasterDocSuite.OnWorkflowConfirmed, AddressOf WorkflowConfirmed
        If Not IsPostBack Then
            Initialize()
        End If

        If btnCancel.Visible AndAlso Not CurrentRepositoryRights.CancelMotivationRequired Then
            btnCancel.CommandArgument = CANCEL_COMMAND_ARGUMENT
            btnCancel.OnClientClicking = "RadConfirm"
            btnCancel.OnClientClicked = "cancelConfirm"
            rfvCancelReason.Enabled = False
        End If
    End Sub

    Protected Sub WorkflowConfirmed(sender As Object, e As EventArgs)
        If CurrentWorkflowActivity IsNot Nothing AndAlso (CurrentWorkflowActivity.Status = WorkflowStatus.Todo OrElse CurrentWorkflowActivity.Status = WorkflowStatus.Progress) Then
            Dim workflowNotify As WorkflowNotify = New WorkflowNotify(CurrentWorkflowActivity.UniqueId) With {
                .WorkflowName = CurrentWorkflowActivity?.WorkflowInstance?.WorkflowRepository?.Name
            }
            workflowNotify.OutputArguments.Add(WorkflowPropertyHelper.DSW_FIELD_UDS_NUMBER, New WorkflowArgument() With {
                                      .Name = WorkflowPropertyHelper.DSW_FIELD_UDS_NUMBER,
                                      .PropertyType = ArgumentType.PropertyInt,
                                      .ValueInt = UDSSource.Number})
            workflowNotify.OutputArguments.Add(WorkflowPropertyHelper.DSW_FIELD_UDS_YEAR, New WorkflowArgument() With {
                                      .Name = WorkflowPropertyHelper.DSW_FIELD_UDS_YEAR,
                                      .PropertyType = ArgumentType.PropertyInt,
                                      .ValueInt = UDSSource.Year})
            workflowNotify.OutputArguments.Add(WorkflowPropertyHelper.DSW_FIELD_UDS_ID, New WorkflowArgument() With {
                                      .Name = WorkflowPropertyHelper.DSW_FIELD_UDS_ID,
                                      .PropertyType = ArgumentType.PropertyGuid,
                                      .ValueGuid = CurrentIdUDS})
            workflowNotify.OutputArguments.Add(WorkflowPropertyHelper.DSW_ACTION_ACTIVITY_MANUAL_COMPLETE, New WorkflowArgument() With {
                                      .Name = WorkflowPropertyHelper.DSW_ACTION_ACTIVITY_MANUAL_COMPLETE,
                                      .PropertyType = ArgumentType.PropertyBoolean,
                                      .ValueBoolean = True})
            Dim webApiHelper As WebAPIHelper = New WebAPIHelper()
            If Not WebAPIImpersonatorFacade.ImpersonateSendRequest(webApiHelper, workflowNotify, DocSuiteContext.Current.CurrentTenant.WebApiClientConfig, DocSuiteContext.Current.CurrentTenant.OriginalConfiguration) Then
                FileLogger.Warn(LoggerName, "UDSWorkflowConfirmed is not correctly evaluated from WebAPI. See specific error in WebAPI logger")
            End If
        End If
        Response.Redirect("~/User/UserWorkflow.aspx?Type=Comm")
    End Sub

    Protected Sub UDSView_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arg As String = e.Argument.Replace("~", "'")
        Dim arguments As Object() = arg.Split("|"c)
        If arguments.Length = 0 Then
            Exit Sub
        End If

        Dim argumentName As String = arguments(0).ToString()

        Select Case argumentName
            Case "udsDeleteCallback"
                Dim cancelMotivation As String = If(CurrentRepositoryRights.CancelMotivationRequired, txtCancelReason.Text, String.Empty)
                uscUDS.SetDeletedState(cancelMotivation)
                AjaxManager.ResponseScripts.Add(String.Format(ON_DONE_FUNCTION, "Annullamento eseguito correttamente"))

            Case "toDuplicate"
                ' trucco orribile per fare cross-page ed avere la PreviousPage
                toDuplicate.PostBackUrl = String.Concat("~/UDS/UDSInsert.aspx?Type=UDS&Action=Duplicate&IdUDSRepository=", arguments(2), "&IdUDS=", arguments(1), "&Check=", arguments(3))
                AjaxManager.ResponseScripts.Add("goToDuplicateInsert()")
        End Select
    End Sub

    Protected Sub btnCancelUDS_OnClick(sender As Object, e As EventArgs) Handles btnCancelUDS.Click, btnCancel.Click
        Try
            Dim correlationId As Guid = Guid.Empty
            Dim sendedCommandId As Guid = Guid.Empty
            If (Not Guid.TryParse(HFcorrelatedCommandId.Value, correlationId)) Then
                AjaxManager.ResponseScripts.Add(String.Format(ON_ERROR_FUNCTION, "Errore generale, contattare assistenza : CorrealtionId is not Valid."))
                Return
            End If

            Dim model As UDSModel = uscUDS.GetUDSModel()
            Dim cancelMotivation As String = If(CurrentRepositoryRights.CancelMotivationRequired, txtCancelReason.Text, String.Empty)
            sendedCommandId = CurrentUDSRepositoryFacade.SendCommandDeleteData(CurrentIdUDSRepository.Value, CurrentIdUDS.Value, correlationId, model, cancelMotivation)
            FileLogger.Info(LoggerName, String.Format("Command sended with Id {0} and CorrelationId {0}", sendedCommandId, correlationId))

            btnCancel.Enabled = False
        Catch ex As Exception
            Dim exceptionMessage As String = String.Format("Errore nella fase di invio: {0}", ProtocolEnv.DefaultErrorMessage)
            AjaxManager.ResponseScripts.Add(String.Format(ON_ERROR_FUNCTION, HttpUtility.JavaScriptStringEncode(exceptionMessage)))
        End Try
    End Sub

    Private Sub BtnFascicle_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnFascicle.Click
        Dim params As String = CommonShared.AppendSecurityCheck(String.Format("UniqueId={0}&UDSRepositoryName={1}&CategoryId={2}&UDType={3}&CategoryFullIncrementalPath={4}&FascicleObject={5}&FolderSelectionEnabled=True&Type=Fasc",
            UDSSource.Id, UDSSource.UDSRepository.Name, UDSSource.Category.EntityShortId, Convert.ToInt32(DSWEnvironment.UDS), UDSSource.Category.FullIncrementalPath, UDSSource.Subject))
        If CurrentIdUDSRepository.HasValue Then
            params = String.Concat(params, "&IdUDSRepository=", CurrentIdUDSRepository.Value)
        End If
        Response.Redirect(String.Concat("~/Fasc/FascUDManager.aspx?", params))
    End Sub

    Private Sub BtnLog_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnLog.Click
        Dim params As String = CommonShared.AppendSecurityCheck(String.Format("IdUDS={0}&Type=UDS", UDSSource.Id))
        If CurrentIdUDSRepository.HasValue Then
            params = String.Concat(params, "&IdUDSRepository=", CurrentIdUDSRepository.Value)
        End If
        Response.Redirect(String.Concat("~/UDS/UDSLog.aspx?", params))
    End Sub

#End Region

#Region "Methods"
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf UDSView_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(btnCancel, btnCancel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnCancelUDS, btnCancelUDS)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscUDS)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, toDuplicate)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnFascicle, btnFascicle)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnLog, btnLog)
    End Sub

    Private Sub Initialize()
        If Not CurrentIdUDS.HasValue Then
            Throw New ArgumentNullException("CurrentIdUDS")
        End If

        If Not CurrentIdUDSRepository.HasValue Then
            Throw New ArgumentNullException("CurrentIdUDSRepository")
        End If

        If Not CurrentRepositoryRights.IsSummaryViewable AndAlso Not CurrentRepositoryRights.IsCurrentUserAuthorized Then
            Throw New DocSuiteException("Utente senza diritti di visualizzazione")
        End If

        If IsWorkflowOperation Then
            MasterDocSuite.WorkflowWizardRow.Visible = True
            MasterDocSuite.WizardActionColumn.Visible = True

            InitializeWorkflowWizard()
        End If

        uscUDS.UDSStatus = UDSSource.Status
        uscUDS.UDSCancelMotivation = UDSSource.CancelMotivation
        uscUDS.UDSId = CurrentIdUDS
        uscUDS.UDSYear = UDSSource.Year
        uscUDS.UDSNumber = UDSSource.Number
        uscUDS.InitializeUscDocumentUnitReferences()
        uscUDS.InitializeMulticlassification()
        uscUDS.VisibleParer = ProtocolEnv.ParerEnabled AndAlso UDSSource.UDSModel.Model.ConservationEnabled

        btnEdit.PostBackUrl = String.Format(EDIT_PAGE_URL, CurrentIdUDS, CurrentIdUDSRepository, String.Empty)

        btnSendPec.Visible = False
        If DocSuiteContext.Current.ProtocolEnv.IsPECEnabled Then
            btnSendPec.PostBackUrl = String.Format("../PEC/PECInsert.aspx?Type=Pec&SimpleMode={0}&Year={1}&Number={2}&IdUDSRepository={3}&IdUDS={4}",
                                               DocSuiteContext.Current.ProtocolEnv.PECSimpleMode.ToString(), UDSSource.Year, UDSSource.Number, CurrentIdUDSRepository, CurrentIdUDS)
            btnSendPec.Visible = CurrentRepositoryRights.IsPECSendable
        End If

        btnMail.Visible = CurrentRepositoryRights.IsMailSendable
        btnMailRoles.Visible = CurrentRepositoryRights.IsMailRoleSendable
        btnMail.PostBackUrl = String.Format("~/MailSenders/UDSMailSender.aspx?recipients=false&IdUDSRepository={0}&IdUDS={1}&Type=UDS", CurrentIdUDSRepository, CurrentIdUDS)
        btnMailRoles.PostBackUrl = String.Format("~/MailSenders/UDSMailSender.aspx?IdUDSRepository={0}&IdUDS={1}&Type=UDS", CurrentIdUDSRepository, CurrentIdUDS)

        btnToProtocol.Visible = False
        If CurrentRepositoryRights.IsProtocollable AndAlso Not UDSSource.HasProtocol() Then
            Dim params As New StringBuilder()
            params.Append("Type=Prot&Action=FromUDS")
            params.AppendFormat("&IdUDS={0}", CurrentIdUDS)
            params.AppendFormat("&IdUDSRepository={0}", CurrentIdUDSRepository)

            btnToProtocol.PostBackUrl = $"../Prot/ProtInserimento.aspx?{CommonShared.AppendSecurityCheck(params.ToString())}"
            btnToProtocol.Visible = True
        End If

        btnViewDocuments.PostBackUrl = String.Format(VIEWER_PAGE_URL, CurrentIdUDS, CurrentIdUDSRepository)
        btnViewDocuments.Visible = CurrentRepositoryRights.IsDocumentsViewable OrElse CurrentRepositoryRights.IsCurrentUserAuthorized
        btnCancel.Visible = CurrentRepositoryRights.IsDeletable
        btnEdit.Visible = CurrentRepositoryRights.IsEditable
        btnDuplica.Visible = CurrentRepositoryRights.IsClonable
        btnLink.Visible = UDSSource.UDSModel.Model.LinkButtonEnabled AndAlso CurrentRepositoryRights.IsEditable

        btnLog.Visible = False
        If CurrentRepositoryRights.IsActive Then
            btnLog.Visible = CurrentRepositoryRights.EnableViewLog
        End If

        btnAuthorize.PostBackUrl = $"~/UDS/UDSAutorizza.aspx?Type=UDS&IdUDS={CurrentIdUDS}&IdUDSRepository={CurrentIdUDSRepository}"
        btnAuthorize.Visible = CurrentRepositoryRights.IsActive AndAlso UDSSource.UDSModel.Model.Authorizations IsNot Nothing AndAlso (UDSSource.UDSModel.Model.Authorizations.ModifyEnabled OrElse UDSSource.UDSModel.Model.Authorizations.Instances Is Nothing) AndAlso CurrentRepositoryRights.IsAuthorizable
        btnFascicle.Enabled = False

        btnFascicle.Visible = CurrentRepositoryRights.IsActive AndAlso DocSuiteContext.Current.ProtocolEnv.FascicleEnabled AndAlso CurrentRepositoryRights.IsDocumentsViewable AndAlso Not CurrentRepositoryRights.HasWorkflowInProgress
        btnFascicle.Enabled = btnFascicle.Visible

        uscUDS.ActionType = uscUDS.ACTION_TYPE_VIEW

        Title = $"{UDSSource.UDSModel.Model.Title} - Visualizzazione"

        UDSLogFacade.InsertLog(UDSSource.Id, UDSSource.UDSRepository.UniqueId, UDSSource.UDSRepository.DSWEnvironment, String.Concat("Visualizzazione ", UDSSource.UDSModel.Model.Title), WebApientity.UDSLogType.SummaryView)
    End Sub
    Public Function GetUDSInitializer() As UDSDto Implements IUDSInitializer.GetUDSInitializer
        Return UDSSource
    End Function

    Private Sub InitializeWorkflowWizard()

        Dim insertUDSStep As RadWizardStep = New RadWizardStep()
        insertUDSStep.ID = "InsertUDS"
        insertUDSStep.Title = "Inserisci un nuovo Archivio"
        insertUDSStep.ToolTip = "Inserisci un nuovo Archivio"
        insertUDSStep.Enabled = False
        MasterDocSuite.WorkflowWizardControl.WizardSteps.Add(insertUDSStep)

        Dim sendCompleteStep As RadWizardStep = New RadWizardStep()
        sendCompleteStep.ID = "SendComplete"
        sendCompleteStep.Title = "Concludi attività"
        sendCompleteStep.ToolTip = "Concludi attività"
        sendCompleteStep.Active = True
        MasterDocSuite.WorkflowWizardControl.WizardSteps.Add(sendCompleteStep)

        MasterDocSuite.CompleteWorkflowActivityButton.Enabled = False
        If CurrentWorkflowActivity IsNot Nothing AndAlso (CurrentWorkflowActivity.Status = WorkflowStatus.Todo OrElse CurrentWorkflowActivity.Status = WorkflowStatus.Progress) Then
            MasterDocSuite.CompleteWorkflowActivityButton.Enabled = True
        End If
    End Sub

    Public Function GetProtocolInitializer() As ProtocolInitializer Implements IProtocolInitializer.GetProtocolInitializer
        Return CurrentUDSRepositoryFacade.GetProtocolInitializer(UDSSource)
    End Function

#End Region

End Class