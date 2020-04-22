Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Entity.Templates
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Extensions
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.UDS
Imports VecompSoftware.DocSuiteWeb.Model.WebAPI.Client
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Logging

Public Class TemplateUserCollGestione
    Inherits UserBasePage

#Region "Fields"
    Private Const INSERT_ACTION As String = "Insert"
    Private Const EDIT_ACTION As String = "Edit"
    Public Const CONFIRM_CALLBACK As String = "templateUserCollGestione.confirmCallback({0}, {1})"
    Public Const LOAD_CALLBACK As String = "templateUserCollGestione.loadFromEntityCallback()"
    Public Const SET_BUTTONS_STATE As String = "templateUserCollGestione.setButtonsState({0})"
    Private _currentUDSRepositoryFacade As UDSRepositoryFacade
#End Region

#Region "Properties"
    Public ReadOnly Property CurrentUDSRepositoryFacade As UDSRepositoryFacade
        Get
            If _currentUDSRepositoryFacade Is Nothing Then
                _currentUDSRepositoryFacade = New UDSRepositoryFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _currentUDSRepositoryFacade
        End Get
    End Property

    Private Property RoleContacts As IDictionary(Of Role, List(Of Contact))
        Get
            If ViewState("roleContacts") IsNot Nothing Then
                Return DirectCast(ViewState("roleContacts"), Dictionary(Of Role, List(Of Contact)))
            End If
            ViewState("roleContacts") = New Dictionary(Of Role, List(Of Contact))
            Return DirectCast(ViewState("roleContacts"), Dictionary(Of Role, List(Of Contact)))
        End Get
        Set(value As IDictionary(Of Role, List(Of Contact)))
            ViewState("roleContacts") = value
        End Set
    End Property

    Private Property SecretaryRoles As IDictionary(Of Guid, Integer)
        Get
            If ViewState("secretaryRoles") IsNot Nothing Then
                Return DirectCast(ViewState("secretaryRoles"), IDictionary(Of Guid, Integer))
            End If
            ViewState("secretaryRoles") = New Dictionary(Of Guid, Integer)
            Return DirectCast(ViewState("secretaryRoles"), IDictionary(Of Guid, Integer))
        End Get
        Set(value As IDictionary(Of Guid, Integer))
            ViewState("secretaryRoles") = value
        End Set
    End Property

    Public ReadOnly Property TemplateId As Guid?
        Get
            If Not String.IsNullOrEmpty(Request.QueryString("TemplateId")) Then
                Return Guid.Parse(Request.QueryString("TemplateId"))
            End If
            Return Nothing
        End Get
    End Property

#End Region

#Region "Events"
    Private Sub uscVisioneFirma_ContactAdded(ByVal sender As Object, ByVal e As EventArgs) Handles uscVisioneFirma.RoleUserContactAdded, uscVisioneFirma.ManualContactAdded
        Dim contact As Contact = JsonConvert.DeserializeObject(Of Contact)(uscVisioneFirma.JsonContactAdded)
        InitializeSegreterie(contact)
    End Sub

    Protected Sub btnActions_OnClick(sender As Object, e As EventArgs) Handles btnConfirm.Click, btnPublish.Click
        Try
            Dim senderControl As RadButton = DirectCast(sender, RadButton)
            Dim entity As TemplateCollaboration = GetTemplateCollaboration()
            AjaxManager.ResponseScripts.Add(String.Format(CONFIRM_CALLBACK, JsonConvert.SerializeObject(entity), JsonConvert.SerializeObject(senderControl.ID.Eq(btnPublish.ID))))
        Catch ex As Exception
            FileLogger.Error(LoggerName, "Errore in generazione template", ex)
            AjaxManager.ResponseScripts.Add(String.Format(SET_BUTTONS_STATE, JsonConvert.SerializeObject(True)))
            AjaxManager.ResponseScripts.Add(LOAD_CALLBACK)
            AjaxAlert("Errore in generazione template")
        End Try
    End Sub

    Protected Sub TemplateUserCollGestione_AjaxRequest(sender As Object, e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch
        End Try
        If ajaxModel Is Nothing Then
            Return
        End If

        Select Case ajaxModel.ActionName
            Case "DocumentTypeChanged"
                uscVisioneFirma.CollaborationType = ajaxModel.Value(0)
                uscVisioneFirma.EnvironmentType = ajaxModel.Value(0)
                uscVisioneFirma.UpdateRoleUserType()

                uscAuthorizedRoles.Environment = GetEnvironmentFromDocumentType(ajaxModel.Value(0))
                lblDestinatari.Text = DocumentTypeCaption(ddlDocumentType.SelectedValue)

            Case "LoadFromEntity"
                Try
                    FillPageFromEntity(JsonConvert.DeserializeObject(Of TemplateCollaboration)(ajaxModel.Value(0)))
                    AjaxManager.ResponseScripts.Add(LOAD_CALLBACK)
                Catch ex As Exception
                    FileLogger.Error(LoggerName, ex.Message, ex)
                        AjaxManager.ResponseScripts.Add(String.Format(SET_BUTTONS_STATE, JsonConvert.SerializeObject(False)))
                        AjaxManager.ResponseScripts.Add(LOAD_CALLBACK)
                    AjaxAlert("Errore in caricamento utenti del template")
                End Try
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
        AddHandler MasterDocSuite.AjaxManager.AjaxRequest, AddressOf TemplateUserCollGestione_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConfirm, pnlMainPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnPublish, pnlMainPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlDocumentType, uscVisioneFirma)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscVisioneFirma, uscSettoriSegreterie)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscSettoriSegreterie, uscSettoriSegreterie)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscAuthorizedRoles, uscAuthorizedRoles)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, lblDestinatari)
    End Sub

    Private Sub Initialize()
        If Not CommonShared.UserConnectedBelongsTo(ProtocolEnv.TemplateCollaborationGroups) AndAlso Not CommonShared.HasGroupAdministratorRight Then
            Throw New DocSuiteException("Utente non abilitato alla gestione del template di Collaborazione")
        End If

        If String.IsNullOrEmpty(Action) OrElse Not (Action.Eq(INSERT_ACTION) OrElse Action.Eq(EDIT_ACTION)) Then
            Throw New DocSuiteException("Action type non corretto")
        End If

        BindDdlCollaborationType()
        Dim collaborationTypeName As String = CollaborationFacade.GetPageTypeFromDocumentType(ddlDocumentType.SelectedValue)
        uscVisioneFirma.CollaborationType = collaborationTypeName
        uscVisioneFirma.EnvironmentType = collaborationTypeName
    End Sub

    Private Function GetTemplateCollaboration() As TemplateCollaboration
        Dim entity As TemplateCollaboration = If(Action.Eq(INSERT_ACTION), New TemplateCollaboration(), New TemplateCollaboration(TemplateId.Value))
        If txtName.Text IsNot Nothing Then
            entity.Name = txtName.Text
        End If
        Dim signers As RadTreeNodeCollection = uscVisioneFirma.TreeViewControl.Nodes(0).Nodes
        If signers.Count > 0 Then
            Dim incremental As Integer = 0
            For Each signer As RadTreeNode In signers
                Dim templateUser As TemplateCollaborationUser
                If Action.Eq(EDIT_ACTION) AndAlso Not String.IsNullOrEmpty(signer.Attributes("TemplateCollaborationUserId")) Then
                    templateUser = New TemplateCollaborationUser(Guid.Parse(signer.Attributes("TemplateCollaborationUserId")))
                    templateUser.IsValid = Boolean.Parse(signer.Attributes("IsValid"))
                Else
                    templateUser = New TemplateCollaborationUser()
                    templateUser.IsValid = True
                End If

                Dim signerContact As Contact = JsonConvert.DeserializeObject(Of Contact)(signer.Attributes(uscContattiSel.ManualContactAttribute))
                templateUser.Account = signerContact.Code
                incremental += 1
                templateUser.Incremental = incremental
                templateUser.IsRequired = signer.Checked
                templateUser.UserType = TemplateCollaborationUserType.Signer

                Dim role As Role = RoleContacts.Where(Function(x) x.Value.Any(Function(xx) xx.FullDescription.Eq(signerContact.FullDescription))).Select(Function(s) s.Key).FirstOrDefault()
                If role IsNot Nothing Then
                    templateUser.Role = New Entity.Commons.Role() With {.EntityShortId = Convert.ToInt16(role.Id)}
                End If

                entity.TemplateCollaborationUsers.Add(templateUser)
            Next
        End If

        Dim secretaries As IList(Of Role) = uscSettoriSegreterie.GetRoles()
        If secretaries.Count > 0 Then
            Dim incremental As Integer = 0
            For Each secretary As Role In secretaries
                Dim templateUser As TemplateCollaborationUser
                If SecretaryRoles.Any(Function(x) x.Value.Equals(secretary.Id)) Then
                    templateUser = New TemplateCollaborationUser(SecretaryRoles.First(Function(x) x.Value.Equals(secretary.Id)).Key)
                Else
                    templateUser = New TemplateCollaborationUser()
                End If

                incremental += 1
                templateUser.Incremental = incremental
                templateUser.UserType = TemplateCollaborationUserType.Secretary
                templateUser.Role = New Entity.Commons.Role() With {.EntityShortId = Convert.ToInt16(secretary.Id)}
                templateUser.IsRequired = uscSettoriSegreterie.TreeViewControl.FindNodeByValue(secretary.Id.ToString()).Checked
                templateUser.IsValid = True
                entity.TemplateCollaborationUsers.Add(templateUser)
            Next
        End If

        Dim restitutions As RadTreeNodeCollection = uscRestituzioni.TreeViewControl.Nodes(0).Nodes
        If restitutions.Count > 0 Then
            Dim incremental As Integer = 0
            For Each restitution As RadTreeNode In restitutions
                Dim templateRestitutionUser As TemplateCollaborationUser
                If Action.Eq(EDIT_ACTION) AndAlso Not String.IsNullOrEmpty(restitution.Attributes("TemplateCollaborationUserId")) Then
                    templateRestitutionUser = New TemplateCollaborationUser(Guid.Parse(restitution.Attributes("TemplateCollaborationUserId")))
                Else
                    templateRestitutionUser = New TemplateCollaborationUser()
                End If

                Dim restitutionContact As Contact = JsonConvert.DeserializeObject(Of Contact)(restitution.Attributes(uscContattiSel.ManualContactAttribute))
                incremental += 1
                templateRestitutionUser.Incremental = incremental
                templateRestitutionUser.UserType = TemplateCollaborationUserType.Person
                templateRestitutionUser.Account = restitutionContact.Code
                templateRestitutionUser.IsRequired = restitution.Checked
                templateRestitutionUser.IsValid = True
                entity.TemplateCollaborationUsers.Add(templateRestitutionUser)
            Next
        End If

        Dim proposerRoles As IList(Of Role) = uscAuthorizedRoles.GetRoles()
        If proposerRoles.Count > 0 Then
            For Each proposerRole As Role In proposerRoles
                entity.Roles.Add(New Entity.Commons.Role() With {.EntityShortId = Convert.ToInt16(proposerRole.Id)})
            Next
        End If

        Return entity
    End Function

    Private Function GetListItem(documentType As CollaborationDocumentType) As DropDownListItem
        Dim listItem As New DropDownListItem(documentType.ToString(), documentType.ToString())
        Select Case documentType
            Case CollaborationDocumentType.P
                listItem.Text = "Protocollo"
            Case CollaborationDocumentType.U
                listItem.Text = "Uoia"
            Case CollaborationDocumentType.D
                listItem.Text = Facade.ResolutionTypeFacade.DeliberaCaption
            Case CollaborationDocumentType.A
                listItem.Text = Facade.ResolutionTypeFacade.DeterminaCaption
            Case CollaborationDocumentType.S
                listItem.Text = ProtocolEnv.DocumentSeriesName
            Case CollaborationDocumentType.UDS
                listItem.Text = "Archivi"
        End Select

        Return listItem
    End Function

    Public Sub BindDdlCollaborationType()
        Me.ddlDocumentType.Items.Clear()
        Dim documentTypes As IList(Of CollaborationDocumentType) = Facade.CollaborationFacade.GetAvailableDocumentTypes()

        Dim tmpTypes As List(Of DropDownListItem) = documentTypes.Where(Function(t) Not t.Equals(CollaborationDocumentType.O) AndAlso Not t.Equals(CollaborationDocumentType.W)) _
                     .Select(Function(t) Me.GetListItem(t)).OrderBy(Function(o) o.Text).ToList()
        tmpTypes.ForEach(Sub(t) ddlDocumentType.Items.Add(t))

        ddlSpecificDocumentType.Items.Clear()
        ddlSpecificDocumentType.Items.Add(New DropDownListItem("", "") With {.Selected = True})
        CurrentUDSRepositoryFacade.GetFascicolable().ToList().ForEach(Sub(s) ddlSpecificDocumentType.Items.Add(New DropDownListItem(s.Name, Convert.ToInt32(s.DSWEnvironment).ToString())))
    End Sub

    Private Sub InitializeSegreterie(ByVal contact As Contact)
        Dim roles As IList(Of Role) = New List(Of Role)
        If Not String.IsNullOrEmpty(contact.RoleUserIdRole) Then
            roles.Add(Facade.RoleFacade.GetById(Integer.Parse(contact.RoleUserIdRole)))
        End If

        For Each role As Role In roles
            If RoleContacts.ContainsKey(role) Then
                RoleContacts(role).Add(contact)
            Else
                RoleContacts.Add(role, New List(Of Contact)({contact}))
                uscSettoriSegreterie.AddRole(role, True, False, False, True)
            End If
        Next
    End Sub

    Private Sub FillSigners(templateUsers As ICollection(Of TemplateCollaborationUser))
        uscVisioneFirma.DataSource = Nothing
        uscVisioneFirma.DataBind()
        For Each contactSigner As TemplateCollaborationUser In templateUsers.Where(Function(x) x.UserType.Equals(TemplateCollaborationUserType.Signer) AndAlso Not String.IsNullOrEmpty(x.Account)).OrderBy(Function(o) o.Incremental)
            Dim user As AccountModel = CommonAD.GetAccount(contactSigner.Account)
            Dim roleUserId As String = String.Empty
            Dim description As String = String.Empty
            Dim email As String = String.Empty
            If user IsNot Nothing Then
                description = user.DisplayName
                email = user.Email
            End If

            If contactSigner.Role IsNot Nothing Then
                roleUserId = contactSigner.Role.EntityShortId.ToString()
                Dim roleUser As RoleUser = Facade.RoleUserFacade.GetByRoleIdAndAccount(contactSigner.Role.EntityShortId, contactSigner.Account, True).FirstOrDefault()
                If roleUser IsNot Nothing Then
                    description = roleUser.Description
                    If Not String.IsNullOrEmpty(roleUser.Email) Then
                        email = roleUser.Email
                    End If
                End If
            End If

            If String.IsNullOrEmpty(description) AndAlso String.IsNullOrEmpty(email) Then
                FileLogger.Warn(LoggerName, String.Concat("Non sono stati trovati riferimenti per l'account ", contactSigner.Account))
                description = contactSigner.Account
            End If

            Dim node As RadTreeNode = uscVisioneFirma.AddCollaborationContact(ContactType.AdAmPerson, description, email, contactSigner.Account, String.Empty, roleUserId, True, contactSigner.IsRequired)
            node.Attributes.Add("TemplateCollaborationUserId", contactSigner.UniqueId.ToString())
            node.Attributes.Add("IsValid", contactSigner.IsValid.ToString())
            node.Attributes.Add("Persisted", "True")
        Next
    End Sub

    Private Sub FillSecretaries(templateUsers As ICollection(Of TemplateCollaborationUser))
        SecretaryRoles = New Dictionary(Of Guid, Integer)
        For Each secretary As TemplateCollaborationUser In templateUsers.Where(Function(x) x.UserType.Equals(TemplateCollaborationUserType.Secretary) AndAlso x.Role IsNot Nothing).OrderBy(Function(o) o.Incremental)
            uscSettoriSegreterie.AddRole(Facade.RoleFacade.GetById(secretary.Role.EntityShortId), True, False, False, secretary.IsRequired)
            SecretaryRoles.Add(secretary.UniqueId, secretary.Role.EntityShortId)
        Next
    End Sub

    Private Sub FillAuthorizedProposers(roles As ICollection(Of Entity.Commons.Role))
        For Each role As Entity.Commons.Role In roles
            uscAuthorizedRoles.AddRole(Facade.RoleFacade.GetById(role.EntityShortId), True, False, False, False)
        Next
    End Sub

    Private Sub FillRestitutions(templateUsers As ICollection(Of TemplateCollaborationUser))
        uscRestituzioni.DataSource = Nothing
        uscRestituzioni.DataBind()
        For Each contactRestitution As TemplateCollaborationUser In templateUsers.Where(Function(x) x.UserType.Equals(TemplateCollaborationUserType.Person) AndAlso Not String.IsNullOrEmpty(x.Account)).OrderBy(Function(o) o.Incremental)
            Dim user As AccountModel = CommonAD.GetAccount(contactRestitution.Account)
            Dim description As String = String.Empty
            Dim email As String = String.Empty
            If user IsNot Nothing Then
                description = user.DisplayName
                email = user.Email
            End If

            If String.IsNullOrEmpty(description) AndAlso String.IsNullOrEmpty(email) Then
                FileLogger.Warn(LoggerName, String.Concat("Non sono stati trovati riferimenti per l'account ", contactRestitution.Account))
                description = contactRestitution.Account
            End If

            Dim node As RadTreeNode = uscRestituzioni.AddCollaborationContact(ContactType.AdAmPerson, description, email, contactRestitution.Account, "G", String.Empty, True, contactRestitution.IsRequired)
            node.Attributes.Add("TemplateCollaborationUserId", contactRestitution.UniqueId.ToString())
            node.Attributes.Add("Persisted", "True")
        Next
    End Sub

    Private Sub FillPageFromEntity(entity As TemplateCollaboration)
        Dim collaborationTypeName As String = CollaborationFacade.GetPageTypeFromDocumentType(entity.DocumentType)
        uscVisioneFirma.CollaborationType = collaborationTypeName
        uscVisioneFirma.EnvironmentType = collaborationTypeName
        uscVisioneFirma.UpdateRoleUserType()

        uscAuthorizedRoles.Environment = GetEnvironmentFromDocumentType(entity.DocumentType)

        If entity.TemplateCollaborationUsers.Count > 0 Then
            FillSigners(entity.TemplateCollaborationUsers)
            FillSecretaries(entity.TemplateCollaborationUsers)
            FillRestitutions(entity.TemplateCollaborationUsers)
        End If
        FillAuthorizedProposers(entity.Roles)
        lblDestinatari.Text = DocumentTypeCaption(ddlDocumentType.SelectedValue)
    End Sub

    Private Function GetEnvironmentFromDocumentType(documentType As String) As DSWEnvironment
        If Integer.TryParse(documentType, 0) Then
            Return DSWEnvironment.UDS
        End If

        Select Case documentType
            Case CollaborationDocumentType.P.ToString(),
                 CollaborationDocumentType.U.ToString()
                Return DSWEnvironment.Protocol

            Case CollaborationDocumentType.A.ToString(),
                     CollaborationDocumentType.D.ToString()
                Return DSWEnvironment.Resolution

            Case CollaborationDocumentType.S.ToString()
                Return DSWEnvironment.DocumentSeries

            Case CollaborationDocumentType.UDS.ToString()
                Return DSWEnvironment.UDS
        End Select
    End Function

    Private Function DocumentTypeCaption(ByVal documentType As String) As String
        Select Case documentType
            Case CollaborationDocumentType.D.ToString()
                Return Facade.ResolutionTypeFacade.DeliberaCaption
            Case CollaborationDocumentType.A.ToString()
                Return Facade.ResolutionTypeFacade.DeterminaCaption
            Case CollaborationDocumentType.S.ToString()
                Return ProtocolEnv.DocumentSeriesName
            Case CollaborationDocumentType.UDS.ToString()
                Return "Archivi"
            Case Else
                Return "Protocollazione"
        End Select
    End Function
#End Region

End Class