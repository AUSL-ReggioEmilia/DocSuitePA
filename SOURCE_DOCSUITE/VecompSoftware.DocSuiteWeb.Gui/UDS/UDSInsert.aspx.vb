Imports System.Collections.Generic
Imports System.Web
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.BusinessRule.Rules.Rights.UDS
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Workflows
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.UDS
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Workflows
Imports VecompSoftware.DocSuiteWeb.Facade.WebAPI.Fascicles
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.UDS
Imports VecompSoftware.Helpers.Workflow
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Logging

Public Class UDSInsert
    Inherits UDSBasePage

#Region "Fields"
    Private _udsSource As UDSDto
    Private _signerHelper As Helpers.PDF.SignTools
    Private _workflowSignedDocRequired As IDictionary(Of String, Boolean)
    Private _fromPec As Boolean?
    Private Const PAGE_INSERT_TITLE As String = "Inserimento nuovo archivio"
    Private Const PAGE_EDIT_TITLE As String = "Modifica archivio"
    Private Const BTN_SAVE_INSERT_TITLE As String = "Conferma inserimento"
    Private Const BTN_SAVE_EDIT_TITLE As String = "Modifica archivio"
    Private Const ON_ERROR_FUNCTION As String = "onError('{0}')"
    Private Const UDS_SUMMARY_PATH As String = "~/UDS/UDSView.aspx?Type=UDS&IdUDS={0}&IdUDSRepository={1}"
    Private _currentFascicleDocumentUnitFacade As FascicleDocumentUnitFacade
    Private _fromCollaboration As Boolean?
    Private Const ODATA_EQUAL_UDSID As String = "$filter=UDSId eq {0}"
    Private _currentRepositoryRigths As UDSRepositoryRightsUtil
    Private _fromProtocol? As Boolean
    Private _fromWorkFlow? As Boolean
#End Region

#Region "Error Messages"
    Private Const NOT_SIGNED_DOCUMENT_MESSAGE As String = "Alcuni documenti del controllo {0} non sono stati firmati."
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

    Private Property WorkflowSignedDocRequired As IDictionary(Of String, Boolean)
        Get
            If _workflowSignedDocRequired Is Nothing Then
                _workflowSignedDocRequired = New Dictionary(Of String, Boolean)
            End If
            Return _workflowSignedDocRequired
        End Get
        Set(value As IDictionary(Of String, Boolean))
            _workflowSignedDocRequired = value
        End Set
    End Property

    Public ReadOnly Property IdPECMail As Integer?
        Get
            Return GetKeyValue(Of Integer?)("IdPECMail")
        End Get
    End Property

    Private ReadOnly Property FromPec As Boolean
        Get
            If Not _fromPec.HasValue Then
                _fromPec = IdPECMail.HasValue
            End If
            Return _fromPec.Value
        End Get
    End Property

    Private ReadOnly Property CurrentFascicleDocumentUnitFacade As FascicleDocumentUnitFacade
        Get
            If _currentFascicleDocumentUnitFacade Is Nothing Then
                _currentFascicleDocumentUnitFacade = New FascicleDocumentUnitFacade(DocSuiteContext.Current.Tenants)
            End If
            Return _currentFascicleDocumentUnitFacade
        End Get
    End Property
    Public ReadOnly Property IdCollaboration As Integer?
        Get
            Return GetKeyValue(Of Integer?)("IdCollaboration")
        End Get
    End Property

    Private ReadOnly Property FromCollaboration As Boolean
        Get
            If Not _fromCollaboration.HasValue Then
                _fromCollaboration = IdCollaboration.HasValue
            End If
            Return _fromCollaboration.Value
        End Get
    End Property

    Private ReadOnly Property CurrentRepositoryRigths As UDSRepositoryRightsUtil
        Get
            If _currentRepositoryRigths Is Nothing Then
                _currentRepositoryRigths = New UDSRepositoryRightsUtil(CurrentUDSRepository, DocSuiteContext.Current.User.FullUserName, UDSSource)
            End If
            Return _currentRepositoryRigths
        End Get
    End Property

    Public ReadOnly Property IdProtocol As Guid?
        Get
            Return GetKeyValue(Of Guid?)("IdProtocol")
        End Get
    End Property

    Private ReadOnly Property FromProtocol As Boolean
        Get
            If Not _fromProtocol.HasValue Then
                _fromProtocol = IdProtocol.HasValue
            End If
            Return _fromProtocol.Value
        End Get
    End Property

    Public ReadOnly Property IdWorkflowActivity As Guid?
        Get
            Return GetKeyValue(Of Guid?)("IdWorkflowActivity")
        End Get
    End Property

    Private ReadOnly Property FromWorkFlow As Boolean
        Get
            If Not _fromWorkFlow.HasValue Then
                _fromWorkFlow = IdWorkflowActivity.HasValue
            End If
            Return _fromWorkFlow.Value
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
        uscUDS.UDSSubject = UDSSource.Subject
        uscUDS.UDSId = UDSSource.Id
        uscUDS.RepositoryBind()
    End Sub

    Protected Sub BtnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Try
            Dim correlationId As Guid = Guid.Empty
            If Not Guid.TryParse(HFcorrelatedCommandId.Value, correlationId) Then
                AjaxManager.ResponseScripts.Add(String.Format(ON_ERROR_FUNCTION, "Errore generale, contattare assistenza : CorrelationId is not Valid."))
                Return
            End If

            Dim model As UDSModel = uscUDS.GetUDSModel()
            Dim sendedCommandId As Guid
            CheckAllDocumentSigned(model.Model.Documents)

            Select Case Action
                Case uscUDS.ACTION_TYPE_INSERT, uscUDS.ACTION_TYPE_DUPLICATE
                    If IdPECMail.HasValue Then
                        Dim pecMail As PECMail = Facade.PECMailFacade.GetById(IdPECMail.Value)
                        model.FillPECMails(New List(Of ReferenceModel) From {New ReferenceModel() With {.EntityId = IdPECMail.Value, .UniqueId = pecMail.UniqueId}})
                    End If
                    If IdCollaboration.HasValue Then
                        Dim collaboration As Collaboration = Facade.CollaborationFacade.GetById(IdCollaboration.Value)
                        model.FillCollaborations(New List(Of ReferenceModel) From {New ReferenceModel() With {.EntityId = IdCollaboration.Value, .UniqueId = collaboration.UniqueId, .TemplateName = collaboration.TemplateName}})
                        If ProtocolEnv.SecureDocumentEnabled Then
                            Facade.CollaborationFacade.FinalizeSecureDocument(collaboration)
                        End If
                    End If
                    If IdProtocol.HasValue Then
                        model.FillProtocols(New List(Of ReferenceModel) From {New ReferenceModel() With {.UniqueId = IdProtocol.Value}})
                    End If

                    If Action.Equals("Duplicate") Then
                        uscUDS.CurrentUDSRepositoryId = CurrentUDSRepositoryFacade.GetMaxVersionByName(CurrentUDSRepository.Name).Id
                    End If
                    sendedCommandId = CurrentUDSRepositoryFacade.SendCommandInsertData(uscUDS.CurrentUDSRepositoryId.Value, correlationId, model)


                Case uscUDS.ACTION_TYPE_EDIT
                    If Not model.Model.Category.IdCategory.Eq(UDSSource.UDSModel.Model.Category.IdCategory) Then
                        If CurrentFascicleDocumentUnitFacade.GetFascicolatedIdDocumentUnit(UDSSource.Id) IsNot Nothing Then
                            Throw New Exception("Non è possibile modificare il classificatore del documento in quanto già Fascicolato.")
                        End If
                    End If

                    RemoveDocuments(model)

                    model.Model.Authorizations = UDSSource.UDSModel.Model.Authorizations
                    model.Model.Protocols = UDSSource.UDSModel.Model.Protocols
                    model.Model.Messages = UDSSource.UDSModel.Model.Messages
                    model.Model.PECMails = UDSSource.UDSModel.Model.PECMails
                    model.Model.Collaborations = UDSSource.UDSModel.Model.Collaborations
                    model.Model.Resolutions = UDSSource.UDSModel.Model.Resolutions

                    sendedCommandId = CurrentUDSRepositoryFacade.SendCommandUpdateData(uscUDS.CurrentUDSRepositoryId.Value, CurrentIdUDS.Value, correlationId, model)
            End Select

            FileLogger.Info(LoggerName, $"Command sended with Id {sendedCommandId} and CorrelationId {correlationId}")
            btnSave.Enabled = False
        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            Dim exceptionMessage As String = String.Format("Errore nella fase di invio: {0}", ProtocolEnv.DefaultErrorMessage)
            AjaxManager.ResponseScripts.Add(String.Format(ON_ERROR_FUNCTION, HttpUtility.JavaScriptStringEncode(exceptionMessage)))
        End Try
    End Sub

    Protected Sub UDSIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs) Handles uscUDS.UDSIndexChanged
        btnSave.Enabled = Not String.IsNullOrEmpty(e.Value)
    End Sub

    Protected Sub UDSInsert_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arg As String = e.Argument.Replace("~", "'")
        Dim arguments As Object() = arg.Split("|"c)
        If arguments.Length = 0 Then
            Exit Sub
        End If

        Dim argumentName As String = arguments(0).ToString()

        Select Case argumentName
            Case "udsInsertCallback"
                Dim udsId As Guid = Guid.Parse(arguments(1).ToString())
                Dim udsRepositoryId As Guid = Guid.Parse(arguments(2).ToString())
                CallbackFromServiceBus(udsId, udsRepositoryId)
        End Select
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub
#End Region

#Region "Methods"
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf UDSInsert_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(uscUDS, pnlActionButtons)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSave, pnlActionButtons)
    End Sub

    Private Sub CallbackFromServiceBus(UDSId As Guid, UDSRepositoryId As Guid)
        If UDSId.Equals(Guid.Empty) Then
            Exit Sub
        End If
        If Not String.IsNullOrEmpty(Callback) Then
            Response.Redirect(Callback)
        End If
        Dim url As String = UDS_SUMMARY_PATH
        If IsWorkflowOperation Then
            AddWorkflowProperties(UDSRepositoryId, UDSId)
            url = String.Concat(url, String.Format("&IsWorkflowOperation=True&IdWorkflowActivity={0}", CurrentIdWorkflowActivity))
        End If
        Response.Redirect(String.Format(url, UDSId, UDSRepositoryId))
    End Sub

    Private Sub InitializeWorkflowWizard()
        Dim insertUDSStep As RadWizardStep = New RadWizardStep()
        insertUDSStep.ID = "InsertUDS"
        insertUDSStep.Title = "Inserisci un nuovo Archivio"
        insertUDSStep.ToolTip = "Inserisci un nuovo Archivio"
        insertUDSStep.Active = True
        MasterDocSuite.WorkflowWizardControl.WizardSteps.Add(insertUDSStep)

        Dim sendCompleteStep As RadWizardStep = New RadWizardStep()
        sendCompleteStep.ID = "SendComplete"
        sendCompleteStep.Title = "Concludi attività"
        sendCompleteStep.ToolTip = "Concludi attività"
        sendCompleteStep.Enabled = False
        MasterDocSuite.WorkflowWizardControl.WizardSteps.Add(sendCompleteStep)
    End Sub

    Private Sub Initialize()
        If String.IsNullOrEmpty(Action) OrElse Not (Action.Eq(uscUDS.ACTION_TYPE_INSERT) OrElse Action.Eq(uscUDS.ACTION_TYPE_EDIT) OrElse Action.Eq(uscUDS.ACTION_TYPE_DUPLICATE)) Then
            Throw New DocSuiteException("Action type non corretto")
        End If

        If Action.Eq(uscUDS.ACTION_TYPE_EDIT) AndAlso Not CurrentIdUDS.HasValue Then
            Throw New ArgumentNullException("CurrentIdUDS")
        End If

        If Action.Eq(uscUDS.ACTION_TYPE_EDIT) AndAlso Not CurrentRepositoryRigths.IsEditable Then
            Throw New DocSuiteException("Utente senza diritti di modifica")
        End If

        'Duplicazione Protocollo

        If Action.Eq(uscUDS.ACTION_TYPE_DUPLICATE) Then
            DuplicateUDS()
        End If

        If IsWorkflowOperation Then
            MasterDocSuite.WorkflowWizardRow.Visible = True
            InitializeWorkflowWizard()
            InitializeFromWorkflow()
        End If

        If FromPec OrElse FromCollaboration OrElse FromProtocol OrElse FromWorkflow Then
            InitializeFromUDSInitializer()
        End If

        Title = If(Action.Eq(uscUDS.ACTION_TYPE_INSERT) OrElse Action.Eq(uscUDS.ACTION_TYPE_DUPLICATE), PAGE_INSERT_TITLE, PAGE_EDIT_TITLE)
        btnSave.Enabled = Action.Eq(uscUDS.ACTION_TYPE_EDIT)
        btnSave.Text = If(Action.Eq(uscUDS.ACTION_TYPE_INSERT) OrElse Action.Eq(uscUDS.ACTION_TYPE_DUPLICATE), BTN_SAVE_INSERT_TITLE, BTN_SAVE_EDIT_TITLE)
        uscUDS.ActionType = Action
    End Sub

    Private Function GetCheck(ByVal field As String, ByVal right As Integer) As Boolean
        If Mid$(field, right, 1) = "1" Then
            Return True
        End If
        Return False
    End Function

    Private Sub DuplicateUDS()

        InitializeFromUDSInitializer()

    End Sub

    Private Sub InitializeFromUDSInitializer()
        If PreviousPage IsNot Nothing AndAlso TypeOf PreviousPage Is IUDSInitializer Then
            Dim dtoInitializer As UDSDto = CType(PreviousPage, IUDSInitializer).GetUDSInitializer()
            Dim Check As String = Request.QueryString("Check")
            uscUDS.CurrentUDSRepositoryId = dtoInitializer.UDSRepository.UniqueId

            If Action.Eq(uscUDS.ACTION_TYPE_DUPLICATE) Then
                dtoInitializer.UDSModel.Model.Documents = Nothing
                If GetCheck(Check, UDSCopyOption.Object) Then
                    uscUDS.UDSSubject = dtoInitializer.Subject
                Else
                    dtoInitializer.UDSModel.Model.Subject.Value = String.Empty
                    dtoInitializer.UDSModel.Model.Subject.DefaultValue = String.Empty
                End If
                If GetCheck(Check, UDSCopyOption.Category) Then
                    uscUDS.UDSCategory = dtoInitializer.Category
                Else
                    dtoInitializer.UDSModel.Model.Category = Nothing
                End If

                If Not GetCheck(Check, UDSCopyOption.Metadata) Then
                    Dim modelField As UDSModelField = Nothing
                    For Each item As Section In dtoInitializer.UDSModel.Model.Metadata
                        If item.Items IsNot Nothing Then
                            For Each element As FieldBaseType In item.Items
                                modelField = New UDSModelField(element)
                                modelField.Value = Nothing
                            Next
                        End If
                    Next
                End If

                If Not GetCheck(Check, UDSCopyOption.Roles) Then
                    dtoInitializer.UDSModel.Model.Authorizations = Nothing
                End If

                If Not GetCheck(Check, UDSCopyOption.Contacts) Then
                    dtoInitializer.UDSModel.Model.Contacts = Nothing
                End If
            End If

            uscUDS.UDSItemSource = dtoInitializer.UDSModel
        End If
    End Sub

    Private Sub InitializeFromWorkflow()
        Dim udsNameWorkflowProperty As WorkflowProperty = CurrentWorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(CurrentIdWorkflowActivity, WorkflowPropertyHelper.DSW_FIELD_UDS_NAME)
        If udsNameWorkflowProperty Is Nothing Then
            Throw New DocSuiteException(String.Format("Nessuna workflow property presente con nome {0}", WorkflowPropertyHelper.DSW_FIELD_UDS_NAME))
        End If
        Dim currentRepository As UDSRepository = CurrentUDSRepositoryFacade.GetMaxVersionByName(udsNameWorkflowProperty.ValueString)
        If currentRepository Is Nothing Then
            Exit Sub
        End If


        Dim signedDocRequiredWorkflowProperty As WorkflowProperty = CurrentWorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(CurrentIdWorkflowActivity, WorkflowPropertyHelper.DSW_PROPERTY_SIGNED_DOC_REQUIRED)
        If signedDocRequiredWorkflowProperty IsNot Nothing Then
            'TODO: Dovrebbe essere un dizionario con Nome Controllo: valore booleano
            'WorkflowSignedDocRequired = JsonConvert.DeserializeObject(Of Dictionary(Of string, bool))(signedDocRequiredWorkflowProperty.ValueString, DocSuiteContext.DefaultWebAPIJsonSerializerSettings)
            Dim model As UDSModel = UDSModel.LoadXml(currentRepository.ModuleXML)
            If model.Model.Documents IsNot Nothing AndAlso model.Model.Documents.Document IsNot Nothing Then
                WorkflowSignedDocRequired.Add(model.Model.Documents.Document.Label, signedDocRequiredWorkflowProperty.ValueBoolean.Value)
            End If
        End If

        uscUDS.WorkflowSignedDocRequired = WorkflowSignedDocRequired

        uscUDS.CurrentUDSRepositoryId = currentRepository.Id
        Dim udsModelWorkflowProperty As WorkflowProperty = CurrentWorkflowPropertyFacade.GetWorkflowPropertyByActivityAndName(CurrentIdWorkflowActivity, WorkflowPropertyHelper.DSW_PROPERTY_MODEL)
        If udsModelWorkflowProperty IsNot Nothing Then
            Dim dto As UDSDto = CurrentUDSFacade.ReadUDSWorkflowJson(udsModelWorkflowProperty.ValueString, currentRepository)
            uscUDS.UDSItemSource = dto.UDSModel
        End If
    End Sub

    Private Function CheckDocumentInstancesSigned(docInstances As DocumentInstance()) As Boolean
        If docInstances Is Nothing Then
            Return True
        End If

        For Each instance As DocumentInstance In docInstances

            If Action.Eq(uscUDS.ACTION_TYPE_EDIT) AndAlso String.IsNullOrEmpty(instance.DocumentContent) Then
                Return True
            End If
            If Not Helpers.PDF.SignTools.CheckSigned(Convert.FromBase64String(instance.DocumentContent)) Then
                Return False
            End If
        Next
        Return True
    End Function

    Private Function IsSignRequired(doc As Helpers.UDS.Document) As Boolean
        If doc Is Nothing Then
            Return False
        End If

        If uscUDS.WorkflowSignedDocRequired.ContainsKey(doc.Label) Then
            Return uscUDS.WorkflowSignedDocRequired(doc.Label)
        End If

        Return doc.SignRequired
    End Function

    ''' <summary>
    ''' Verifica, in base alla definizione fatta da Designer o proveniente da workflow, se tutti i documenti
    ''' di una specifico controllo sono firmati.    
    ''' </summary>
    ''' <remarks>Se uno dei documenti non è stato firmato verrà sollevata una eccezione.</remarks>
    Private Sub CheckAllDocumentSigned(documents As Documents)
        If documents Is Nothing Then
            Exit Sub
        End If

        If documents.Document IsNot Nothing AndAlso IsSignRequired(documents.Document) Then
            If Not CheckDocumentInstancesSigned(documents.Document.Instances) Then
                Throw New Exception(String.Format(NOT_SIGNED_DOCUMENT_MESSAGE, documents.Document.Label))
            End If
        End If

        If documents.DocumentAttachment IsNot Nothing AndAlso IsSignRequired(documents.DocumentAttachment) Then
            If Not CheckDocumentInstancesSigned(documents.DocumentAttachment.Instances) Then
                Throw New Exception(String.Format(NOT_SIGNED_DOCUMENT_MESSAGE, documents.DocumentAttachment.Label))
            End If
        End If

        If documents.DocumentAnnexed IsNot Nothing AndAlso IsSignRequired(documents.DocumentAnnexed) Then
            If Not CheckDocumentInstancesSigned(documents.DocumentAnnexed.Instances) Then
                Throw New Exception(String.Format(NOT_SIGNED_DOCUMENT_MESSAGE, documents.DocumentAnnexed.Label))
            End If
        End If
    End Sub

    Private Sub AddWorkflowProperties(UDSRepositoryId As Guid, UDSId As Guid)
        If IsWorkflowOperation Then
            Dim workflowActivityFacade As WorkflowActivityFacade = New WorkflowActivityFacade(DocSuiteContext.Current.User.FullUserName)
            Dim activity As WorkflowActivity = workflowActivityFacade.GetById(CurrentIdWorkflowActivity)
            Dim source As UDSDto = GetSource(UDSRepositoryId, UDSId)

            Dim propertyId As WorkflowProperty = New WorkflowProperty(DocSuiteContext.Current.User.FullUserName) With {
                .Name = WorkflowPropertyHelper.DSW_FIELD_UDS_ID,
                .PropertyType = WorkflowPropertyType.PropertyGuid,
                .WorkflowType = WorkflowType.Activity,
                .ValueGuid = UDSId
            }

            Dim propertyYear As WorkflowProperty = New WorkflowProperty(DocSuiteContext.Current.User.FullUserName) With {
                .Name = WorkflowPropertyHelper.DSW_FIELD_UDS_YEAR,
                .PropertyType = WorkflowPropertyType.PropertyInt,
                .WorkflowType = WorkflowType.Activity,
                .ValueInt = source.Year
            }

            Dim propertyNumber As WorkflowProperty = New WorkflowProperty(DocSuiteContext.Current.User.FullUserName) With {
                .Name = WorkflowPropertyHelper.DSW_FIELD_UDS_NUMBER,
                .PropertyType = WorkflowPropertyType.PropertyInt,
                .WorkflowType = WorkflowType.Activity,
                .ValueInt = source.Number
            }

            Dim propertyRepositoryId As WorkflowProperty = New WorkflowProperty(DocSuiteContext.Current.User.FullUserName) With {
                .Name = WorkflowPropertyHelper.DSW_FIELD_UDS_REPOSITORY_ID,
                .PropertyType = WorkflowPropertyType.PropertyGuid,
                .WorkflowType = WorkflowType.Activity,
                .ValueGuid = UDSRepositoryId
            }

            activity.WorkflowProperties.Add(propertyId)
            activity.WorkflowProperties.Add(propertyYear)
            activity.WorkflowProperties.Add(propertyNumber)
            activity.WorkflowProperties.Add(propertyRepositoryId)
            activity.Status = WorkflowStatus.Progress
            workflowActivityFacade.Save(activity)
        End If
    End Sub

    Private Sub RemoveDocuments(model As UDSModel)
        If model.Model.Documents Is Nothing Then
            Exit Sub
        End If

        Dim container As Data.Container = Facade.ContainerFacade.GetById(Integer.Parse(model.Model.Container.IdContainer))
        Dim toDetach As List(Of Guid) = New List(Of Guid)
        If model.Model.Documents.Document IsNot Nothing AndAlso model.Model.Documents.Document.Deletable Then
            toDetach.AddRange(uscUDS.GetDeletedDocuments(model.Model.Documents.Document))
        End If

        If model.Model.Documents.DocumentAttachment IsNot Nothing AndAlso model.Model.Documents.DocumentAttachment.Deletable Then
            toDetach.AddRange(uscUDS.GetDeletedDocuments(model.Model.Documents.DocumentAttachment))
        End If

        If model.Model.Documents.DocumentAnnexed IsNot Nothing AndAlso model.Model.Documents.DocumentAnnexed.Deletable Then
            toDetach.AddRange(uscUDS.GetDeletedDocuments(model.Model.Documents.DocumentAnnexed))
        End If

        If toDetach.Count > 0 Then
            For Each idDocument As Guid In toDetach
                FileLogger.Debug(LoggerName, String.Format("DetachDocuments -> UDS Id {0} - Detach document with Id: {1}", CurrentIdUDS, idDocument))
                Service.DetachDocument(container.UDSLocation.DocumentServer, idDocument)
            Next
        End If
    End Sub
#End Region

End Class