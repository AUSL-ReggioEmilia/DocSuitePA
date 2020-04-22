Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports Newtonsoft.Json
Imports System.Collections.Generic
Imports APICommons = VecompSoftware.DocSuiteWeb.Entity.Commons
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Commons
Imports DSW = VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Model.Workflow
Imports System.Diagnostics
Imports System.Reflection

Partial Public Class uscStartWorkflow
    Inherits DocSuite2008BaseControl

#Region " Fields "
    Private Const ENVIRONMENT_CHANGED As String = "EnvironmentChanged"
    Private Const SET_RECIPIENT_PROPERTIES As String = "SetRecipientProperties"
    Private Const LOAD_EXTERNAL_DATA As String = "LoadExternalData"
    Private Const UPDATE_CALLBACK As String = "uscStartWorkflow.updateCallback()"
    Private Const USC_PROPOSER_ACCOUNT As String = "usc_proposer_account"
    Private Const GET_PROPOSER_CONTACT As String = "Get_Proposer_Contact"
    Private Const USC_PROPOSER_ROLE As String = "usc_proposer_role"
    Private Const USC_RECIPIENT_ROLE As String = "usc_recipient_role"
    Private Const USC_RECIPIENT_ACCOUNT As String = "usc_recipient_account"
    Private Const GET_RECIPIENT_CONTACT As String = "Get_Recipient_Contact"
    Private Const START_WORKFLOW As String = "uscStartWorkflow.startWorkflow({0})"

    Private _environment As DSWEnvironment?
#End Region

#Region " Properties "
    Protected ReadOnly Property DSWVersion As String
        Get
            Return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion
        End Get
    End Property

    Public Property Environment As DSWEnvironment
        Get
            If _environment Is Nothing Then
                _environment = DSWEnvironment.Any
            End If
            Return CType(_environment, DSWEnvironment)
        End Get
        Set(value As DSWEnvironment)
            _environment = value
        End Set
    End Property

    Public ReadOnly Property PageContent As Control
        Get
            Return pnlWorkflowStart
        End Get
    End Property

    Public Property TenantName As String
    Public Property TenantId As String
    Public Property ShowOnlyNoInstanceWorkflows As Boolean
#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Dim env As DSWEnvironment = Environment
            env = SetEnvironmentRoles(env)
            uscRecipientRole.Environment = env
            rgvDocumentLists.DataSource = New List(Of WorkflowReferenceBiblosModel)
        End If
    End Sub

    Protected Sub uscStartWorkflow_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)

        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch
            Exit Sub
        End Try

        If ajaxModel Is Nothing Then
            Return
        End If

        If String.Equals(ajaxModel.ActionName, SET_RECIPIENT_PROPERTIES) Then
            If ajaxModel.Value.Count > 0 AndAlso CBool(ajaxModel.Value(0)) Then
                Session.Add("MultipleContacts", CBool(ajaxModel.Value(0)))
            Else
                Session.Remove("MultipleContacts")
            End If
        End If

        If String.Equals(ajaxModel.ActionName, ENVIRONMENT_CHANGED) AndAlso ajaxModel.Value IsNot Nothing AndAlso ajaxModel.Value.Count > 0 Then

            If ajaxModel.Value.Count > 0 Then
                Dim NewEnvironment As DSWEnvironment = CType(ajaxModel.Value(0), DSWEnvironment)
                If NewEnvironment <> Environment Then
                    Environment = NewEnvironment
                    'i diritti dei settori delle uds sono salvati in DocumentSeriesRights
                    NewEnvironment = SetEnvironmentRoles(NewEnvironment)
                    Dim roles As IList(Of Role) = New List(Of Role)()
                    For Each r As Role In uscRecipientRole.GetRoles()
                        roles.Add(r)
                    Next
                    If roles.Count > 0 Then
                        uscRecipientRole.RemoveRoles(roles)
                    End If
                    uscRecipientRole.Environment = NewEnvironment
                End If
            End If

            If ajaxModel.Value.Count > 1 Then
                AjaxManager.ResponseScripts.Add(ajaxModel.Value(1))
                Exit Sub
            End If
        End If

        If String.Equals(ajaxModel.ActionName, LOAD_EXTERNAL_DATA) AndAlso ajaxModel.Value IsNot Nothing AndAlso ajaxModel.Value.Count > 2 Then
            Dim controlReadOnly As Boolean = CType(ajaxModel.Value(0), Boolean)
            Dim controlType As String = CType(ajaxModel.Value(1), String)

            Select Case controlType
                Case USC_PROPOSER_ROLE
                    'caricamento settore proponente                
                    uscProposerRole.ReadOnly = controlReadOnly
                    uscProposerRole.Initialize()
                    Dim roles As List(Of APICommons.Role) = JsonConvert.DeserializeObject(Of List(Of APICommons.Role))(ajaxModel.Value(2))
                    LoadRoles(roles, uscProposerRole)
                    Exit Select
                Case USC_RECIPIENT_ROLE
                    'caricamento settore destinatario default
                    uscRecipientRole.ReadOnly = controlReadOnly
                    uscRecipientRole.Initialize()
                    Dim roles As List(Of APICommons.Role) = JsonConvert.DeserializeObject(Of List(Of APICommons.Role))(ajaxModel.Value(2))
                    LoadRoles(roles, uscRecipientRole)
                Case USC_RECIPIENT_ACCOUNT
                    uscRecipientContact.ReadOnly = controlReadOnly
                    Dim contacts As List(Of WorkflowAccount) = JsonConvert.DeserializeObject(Of List(Of WorkflowAccount))(ajaxModel.Value(2))
                    LoadContacts(contacts, uscRecipientContact)
                Case USC_PROPOSER_ACCOUNT
                    uscProposerContact.ReadOnly = controlReadOnly
                    Dim contacts As List(Of WorkflowAccount) = JsonConvert.DeserializeObject(Of List(Of WorkflowAccount))(ajaxModel.Value(2))
                    LoadContacts(contacts, uscProposerContact)
            End Select
            If ajaxModel.Value.Count > 3 Then
                AjaxManager.ResponseScripts.Add(ajaxModel.Value(3))
                Exit Sub
            End If
        End If

        If String.Equals(ajaxModel.ActionName, GET_PROPOSER_CONTACT) Then
            Dim contacts As IList(Of ContactDTO) = uscProposerContact.GetContacts(False)
            Dim accounts As List(Of WorkflowAccount) = New List(Of WorkflowAccount)()
            Dim account As WorkflowAccount
            For Each contact As ContactDTO In contacts
                account = New WorkflowAccount()
                account.AccountName = contact.Contact.Code
                account.DisplayName = contact.Contact.Description
                account.EmailAddress = contact.Contact.EmailAddress

                accounts.Add(account)
            Next
            AjaxManager.ResponseScripts.Add(String.Format(START_WORKFLOW, JsonConvert.SerializeObject(accounts)))
            Exit Sub
        End If
        AjaxManager.ResponseScripts.Add(UPDATE_CALLBACK)
    End Sub

#End Region

#Region " Methods "
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf uscStartWorkflow_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscRecipientRole)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscRecipientRole, uscRecipientRole)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlWorkflowRepository, ddlWorkflowRepository)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscProposerRole)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscProposerRole, uscProposerRole)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscRecipientContact)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscProposerContact)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscUploadDocumentRest)
    End Sub

    Private Function SetEnvironmentRoles(env As DSWEnvironment) As DSWEnvironment
        Select Case env
            Case DSWEnvironment.Fascicle
                Return DSWEnvironment.Any
            Case DSWEnvironment.UDS
                Return DSWEnvironment.DocumentSeries
        End Select
    End Function

    Private Sub LoadRoles(Roles As List(Of APICommons.Role), rolesControl As uscSettori)

        Dim mapper As MapperRoleDSWEntity = New MapperRoleDSWEntity()
        Dim dswRoles As IList(Of DSW.Role) = New List(Of DSW.Role)
        For Each role As APICommons.Role In Roles
            dswRoles.Add(mapper.MappingDTO(role))
        Next

        rolesControl.SourceRoles.Clear()
        rolesControl.SourceRoles.AddRange(CType(dswRoles, IList(Of Data.Role)))
        rolesControl.DataBind(False, False)
    End Sub

    Private Sub LoadContacts(accounts As List(Of WorkflowAccount), uscContact As IBindingUserControl(Of ContactDTO))
        Dim references As IList(Of ContactDTO) = New List(Of ContactDTO)

        Dim contactDto As ContactDTO
        Dim contact As Contact
        For Each account As WorkflowAccount In accounts
            contact = New Contact()
            contact.EmailAddress = account.EmailAddress
            contact.Description = account.DisplayName
            contact.Code = account.AccountName
            contact.ContactType = New ContactType(ContactType.Person)
            contactDto = New ContactDTO(contact, ContactDTO.ContactType.Manual)
            references.Add(contactDto)
        Next
        uscContact.DataSource = references
        uscContact.DataBind()
    End Sub
#End Region

End Class
