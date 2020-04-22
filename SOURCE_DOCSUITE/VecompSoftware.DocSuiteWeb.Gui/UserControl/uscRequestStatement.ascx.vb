Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports Newtonsoft.Json
Imports System.Collections.Generic
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.DocSuiteWeb.Model.Workflow
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Model.Integrations.GenericProcesses
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Commons
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Dematerialisation
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.Entity.DocumentUnits
Imports System.Collections.ObjectModel
Imports VecompSoftware.DocSuiteWeb.Facade.Common.SecureDocument
Imports VecompSoftware.DocSuiteWeb.Facade.Common.SecurePaper
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Partial Public Class uscRequestStatement
    Inherits DocSuite2008BaseControl

#Region " Fields "
    Private _documentUnitFinder As DocumentUnitFinder
    Private _roleContact As Dictionary(Of Role, List(Of Contact))
    Private _dematerialisationFacade As DematerialisationFacade
    Private _secureDocumentFacade As SecureDocumentFacade
    Private _securePaperFacade As SecurePaperFacade
    Public Const DEMATERIALISATION_STATEMENT As String = "DematerialisationStatement"
    Public Const SECURE_DOCUMENT_STATEMENT As String = "SecureDocumentStatement"
    Public Const SECURE_PAPER_STATEMENT As String = "SecurePaperStatement"
    Private Const REFERENCE_SECUREDOCUMENT_ATTRIBUTE_NAME As String = "SecureDocumentId"
    Private Const REFERENCE_SECUREPAPER_ATTRIBUTE_NAME As String = "SecurePaperArchiveKey"
#End Region

#Region " Properties "
    Private ReadOnly Property CurrentDocumentUnitFinder As DocumentUnitFinder
        Get
            If _documentUnitFinder Is Nothing Then
                _documentUnitFinder = New DocumentUnitFinder(DocSuiteContext.Current.CurrentTenant)
            End If
            Return _documentUnitFinder
        End Get
    End Property

    Private ReadOnly Property CurrentDematerialisationFacade As DematerialisationFacade
        Get
            If _dematerialisationFacade Is Nothing Then
                _dematerialisationFacade = New DematerialisationFacade()
            End If
            Return _dematerialisationFacade
        End Get
    End Property

    Private ReadOnly Property CurrentSecureDocumentFacade As SecureDocumentFacade
        Get
            If _secureDocumentFacade Is Nothing Then
                _secureDocumentFacade = New SecureDocumentFacade()
            End If
            Return _secureDocumentFacade
        End Get
    End Property

    Private ReadOnly Property CurrentSecurePaperFacade As SecurePaperFacade
        Get
            If _securePaperFacade Is Nothing Then
                _securePaperFacade = New SecurePaperFacade()
            End If
            Return _securePaperFacade
        End Get
    End Property

    Private Property RoleContacts As Dictionary(Of Role, List(Of Contact))
        Get
            If _roleContact Is Nothing AndAlso ViewState("roleContacts") Is Nothing Then
                _roleContact = New Dictionary(Of Role, List(Of Contact))
                ViewState("roleContacts") = _roleContact
            ElseIf _roleContact Is Nothing AndAlso ViewState("roleContacts") IsNot Nothing Then
                _roleContact = DirectCast(ViewState("roleContacts"), Dictionary(Of Role, List(Of Contact)))
            End If
            Return _roleContact
        End Get
        Set(value As Dictionary(Of Role, List(Of Contact)))
            _roleContact = value
            ViewState("roleContacts") = _roleContact
        End Set
    End Property

    Public Property FromPageMultiple As Boolean
    Public Property TenantName As String
    Public Property TenantId As String
    Public Property TypeId As String
    Public Property IdChain As Guid?
    Public Property IdAttachmentsChain As Guid?
    Public Property ArchiveName As String
    Public Property DocumentUnitsIds As String
    Public Property RequestStatementType As String

    Public ReadOnly Property UDType As DSWEnvironmentType
        Get
            Select Case Type.ToUpperInvariant()
                Case "RESL"
                    Return DSWEnvironmentType.Resolution
                Case "SERIES"
                    Return DSWEnvironmentType.DocumentSeries
                Case Else
                    Return DSWEnvironmentType.Protocol
            End Select
        End Get
    End Property
#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub


    Private Sub UscSignersContactRemoved(ByVal sender As Object, ByVal e As EventArgs) Handles uscSigners.ContactRemoved
        Dim tnRemoved As RadTreeNode = uscSigners.LastRemovedNode
        If tnRemoved Is Nothing Then
            RoleContacts.Clear()
            uscSigners.TreeViewControl.Nodes(0).Nodes.Clear()
            uscSettoriSegreterie.SourceRoles.Clear()
            uscSettoriSegreterie.DataBind()
            Exit Sub
        End If
        Dim serializedContact As String = tnRemoved.Attributes(uscContattiSel.ManualContactAttribute)
        If String.IsNullOrEmpty(serializedContact) Then
            Exit Sub
        End If

        Dim contact As Contact = JsonConvert.DeserializeObject(Of Contact)(serializedContact)
        ' Controllo se la rimozione del contatto porta alla rimozione di ruoli
        Dim roles As List(Of Role) = uscSettoriSegreterie.GetRoles().ToList()
        For Each role As Role In roles
            If Not RoleContacts.ContainsKey(role) OrElse Not RoleContacts(role).Contains(contact) Then
                Continue For
            End If
            RoleContacts(role).RemoveAll(Function(c) c.Code.Eq(contact.Code) AndAlso c.Description.Eq(contact.Description))
            If RoleContacts(role).IsNullOrEmpty() Then
                RoleContacts.Remove(role)
                uscSettoriSegreterie.RemoveRole(role)
            End If
        Next
    End Sub

    Private Sub DocumentListGrid_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles DocumentListGrid.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim boundHeader As DTO.Workflows.RequestStatementResult = DirectCast(e.Item.DataItem, DTO.Workflows.RequestStatementResult)
        e.Item.SelectableMode = GridItemSelectableMode.ServerAndClientSide
        If (Not boundHeader.IsSeletable) Then
            e.Item.SelectableMode = GridItemSelectableMode.None
            e.Item.ToolTip = "Documento con securizzazione già richiesta"
        End If
    End Sub

#End Region

#Region " Methods "
    Public Sub Initialize()

        DocumentListGrid.Visible = Not FromPageMultiple
        documentPanel.Visible = Not FromPageMultiple

        uscSigners.UpdateRoleUserType()
        uscSigners.EnvironmentType = Type
        uscSigners.ReadOnly = False
        rowSigners.SetDisplay(True)
        rowSignerTitle.SetDisplay(True)
        rowSecretaryTitle.SetDisplay(True)
        rowSecretaries.SetDisplay(True)
        uscSettoriSegreterie.ReadOnly = False
        uscSettoriSegreterie.EditableCheck = True

        If (String.IsNullOrEmpty(RequestStatementType)) Then
            RequestStatementType = DEMATERIALISATION_STATEMENT
        End If

        If (RequestStatementType.Eq(SECURE_PAPER_STATEMENT)) Then
            rowSignerTitle.SetDisplay(False)
            rowSigners.SetDisplay(False)
            rowSecretaryTitle.SetDisplay(False)
            rowSecretaries.SetDisplay(False)
        End If

        Dim documentsTobind As List(Of DTO.Workflows.RequestStatementResult) = New List(Of DTO.Workflows.RequestStatementResult)
        Dim guidList As List(Of Guid) = JsonConvert.DeserializeObject(Of List(Of Guid))(DocumentUnitsIds)

        If IdChain.HasValue AndAlso Not IdChain.Equals(Guid.Empty) AndAlso Not FromPageMultiple Then
            For Each biblosDocument As BiblosDocumentInfo In BiblosDocumentInfo.GetDocumentsLatestVersion(String.Empty, IdChain.Value)
                Dim isLocked As Boolean = False
                Select Case RequestStatementType
                    Case SECURE_DOCUMENT_STATEMENT
                        isLocked = Facade.ProtocolLogFacade.SearchSecureDocumentLockLog(guidList.First(), String.Empty, biblosDocument.DocumentId).Count > 0
                    Case SECURE_PAPER_STATEMENT
                        isLocked = Facade.ProtocolLogFacade.SearchSecurePaperLockLog(guidList.First(), String.Empty, biblosDocument.DocumentId).Count > 0
                End Select
                documentsTobind.Add(DocumentMapper(biblosDocument, True, isLocked))
            Next
        End If

        If IdAttachmentsChain.HasValue AndAlso Not IdAttachmentsChain.Equals(Guid.Empty) AndAlso Not FromPageMultiple Then
            For Each biblosDocument As BiblosDocumentInfo In BiblosDocumentInfo.GetDocumentsLatestVersion(String.Empty, IdAttachmentsChain.Value)
                Dim isLocked As Boolean = False
                Select Case RequestStatementType
                    Case SECURE_DOCUMENT_STATEMENT
                        isLocked = Facade.ProtocolLogFacade.SearchSecureDocumentLockLog(guidList.First(), String.Empty, biblosDocument.DocumentId).Count > 0
                    Case SECURE_PAPER_STATEMENT
                        isLocked = Facade.ProtocolLogFacade.SearchSecurePaperLockLog(guidList.First(), String.Empty, biblosDocument.DocumentId).Count > 0
                End Select
                documentsTobind.Add(DocumentMapper(biblosDocument, False, isLocked))
            Next
        End If

        DocumentListGrid.DataSource = documentsTobind
        DocumentListGrid.DataBind()

    End Sub
    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(uscSigners, uscSettoriSegreterie)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscSettoriSegreterie, uscSettoriSegreterie)
        AjaxManager.AjaxSettings.AddAjaxSetting(DocumentListGrid, DocumentListGrid)
    End Sub

    Private Sub UscSignersContactAdded(ByVal sender As Object, ByVal e As EventArgs) Handles uscSigners.RoleUserContactAdded, uscSigners.ManualContactAdded
        Dim contact As Contact = JsonConvert.DeserializeObject(Of Contact)(uscSigners.JsonContactAdded)
        AddSigner(contact)
    End Sub

    Private Sub UscSigner_ContactRemover(ByVal sender As Object, ByVal e As EventArgs) Handles uscSigners.ContactRemoved
        uscSigners.ButtonSelectDomainVisible = True
        uscSigners.ButtonRoleVisible = True
        uscSigners.ButtonSelectVisible = True
    End Sub

    Private Sub InitializeSegreterie(ByVal contact As Contact)
        Dim roles As New List(Of Role)
        If Not String.IsNullOrEmpty(contact.RoleUserIdRole) Then
            roles.Add(Facade.RoleFacade.GetById(Integer.Parse(contact.RoleUserIdRole)))
        Else
            roles.AddRange(Facade.RoleUserFacade.GetSecretaryRoles(contact.Code, True))
        End If

        If roles.IsNullOrEmpty() Then
            Exit Sub
        End If

        For Each role As Role In roles
            If RoleContacts.ContainsKey(role) Then
                RoleContacts(role).Add(contact)
            Else
                RoleContacts.Add(role, New List(Of Contact)({contact}))
                Dim checked As Boolean = True
                uscSettoriSegreterie.AddRole(role, True, False, False, checked)
            End If
        Next
    End Sub

    Public Function SendCommand() As Boolean
        If (DocumentListGrid.SelectedItems.Count < 1 OrElse DocumentListGrid.SelectedItems.Count.Equals(0)) AndAlso Not FromPageMultiple Then
            AjaxManager.Alert("Selezionare almeno un Documento.")
            Return False
        End If

        If Not RequestStatementType.Eq(SECURE_PAPER_STATEMENT) Then
            Dim selectedContacts As IList(Of ContactDTO) = uscSigners.GetContacts(False)

            If (selectedContacts Is Nothing OrElse selectedContacts.Count() > 1 OrElse selectedContacts.Count() < 1) Then
                AjaxManager.Alert("Selezionare almeno uno e non più di un firmatario.")
                Return False
            End If

            Dim singer As ContactDTO = selectedContacts.SingleOrDefault()
            If (singer Is Nothing OrElse singer.Contact Is Nothing) Then
                AjaxManager.Alert("Il firmatario selezionato non è valido. Manca l'attributo account")
                Return False
            End If

            Dim selectedRoles As IList(Of Role) = uscSettoriSegreterie.GetCheckedRoles()

            If (selectedRoles Is Nothing OrElse selectedRoles.Count() < 1) Then
                AjaxManager.Alert("Selezionare almeno una segreteria.")
                Return False
            End If
        End If

        Try
            Dim guidList As List(Of Guid) = JsonConvert.DeserializeObject(Of List(Of Guid))(DocumentUnitsIds)
            Dim statementRequests As List(Of DocumentManagementRequestModel) = New List(Of DocumentManagementRequestModel)
            Dim statementModel As DocumentManagementRequestModel
            Dim log As ProtocolLog
            For Each uniqueId As Guid In guidList
                log = Nothing
                If RequestStatementType.Eq(DEMATERIALISATION_STATEMENT) Then
                    log = Facade.ProtocolLogFacade.SearchLogByProtocolUniqueId(uniqueId, String.Empty, ProtocolLogEvent.SB).SingleOrDefault()
                End If
                If log IsNot Nothing Then
                    statementModel = JsonConvert.DeserializeObject(Of DocumentManagementRequestModel)(log.LogDescription)
                Else
                    CurrentDocumentUnitFinder.ResetDecoration()
                    CurrentDocumentUnitFinder.EnablePaging = False
                    CurrentDocumentUnitFinder.IdDocumentUnit = uniqueId
                    Dim documentUnit As DocumentUnit = CurrentDocumentUnitFinder.DoSearch().Select(Function(f) f.Entity).SingleOrDefault()
                    If (documentUnit Is Nothing) Then
                        AjaxManager.Alert("L'unità documentaria selezionata non esiste. Contattare assistenza per chiedere le verifiche di sincronizzazione delle tabelle.")
                        Return False
                    End If
                    statementModel = PrepareRequest(documentUnit)
                End If

                If Not RequestStatementType.Eq(SECURE_PAPER_STATEMENT) Then
                    statementModel.Roles = GetWorkflowRoles()
                    statementModel.Signers = GetSigners()
                End If

                If (RequestStatementType.Eq(DEMATERIALISATION_STATEMENT)) Then
                    CurrentDematerialisationFacade.SendDematerialisationRequest(statementModel)
                    Select Case UDType
                        Case DSWEnvironmentType.Protocol
                            FacadeFactory.Instance.ProtocolLogFacade.InsertDematerialisationSuccesfullLog(uniqueId)
                            If (log IsNot Nothing) Then
                                Facade.ProtocolLogFacade.Delete(log)
                            End If
                        Case Else
                            Throw New NotImplementedException()
                    End Select
                End If

                If (RequestStatementType.Eq(SECURE_DOCUMENT_STATEMENT)) Then
                    CurrentSecureDocumentFacade.SendSecureDocumentRequest(statementModel)
                    For Each document As WorkflowReferenceBiblosModel In statementModel.Documents
                        FacadeFactory.Instance.ProtocolLogFacade.InsertSecureDocumentSuccesfullLog(uniqueId, document.ArchiveDocumentId.Value)
                    Next
                End If

                If (RequestStatementType.Eq(SECURE_PAPER_STATEMENT)) Then
                    CurrentSecurePaperFacade.SendSecurePaperRequest(statementModel)
                    For Each document As WorkflowReferenceBiblosModel In statementModel.Documents
                        FacadeFactory.Instance.ProtocolLogFacade.InsertSecurePaperSuccesfullLog(uniqueId, document.ArchiveDocumentId.Value)
                    Next
                End If
            Next
            Return True
        Catch ex As Exception
            FileLogger.Error(LogName.FileLog, "invio del comando di dematerializazione non effettuato", ex)
            AjaxManager.Alert("Errore durante l'invio riprovare.")
            Return False
        End Try
    End Function

    Private Function GetWorkflowRoles() As List(Of WorkflowRole)
        Dim selectedRoles As IList(Of Role) = uscSettoriSegreterie.GetCheckedRoles()
        Dim workflowRoles As List(Of WorkflowRole) = New List(Of WorkflowRole)
        Dim workflowRole As WorkflowRole
        For Each role As Role In selectedRoles
            workflowRole = New WorkflowRole With {
                .IdRole = Convert.ToInt16(role.Id),
                .Name = role.Name,
                .EmailAddress = role.EMailAddress
            }
            workflowRoles.Add(workflowRole)
        Next
        Return workflowRoles
    End Function

    Private Function GetSigners() As ICollection(Of WorkflowMapping)
        Dim signers As ICollection(Of WorkflowMapping) = New Collection(Of WorkflowMapping)
        Dim contact As ContactDTO = uscSigners.GetContacts(False).SingleOrDefault()
        Dim signer As WorkflowMapping = New WorkflowMapping()

        signer.Account = New WorkflowAccount() With {
        .AccountName = contact.Contact.Code,
        .DisplayName = contact.Contact.Description,
        .EmailAddress = contact.Contact.EmailAddress,
        .Required = True}

        signers.Add(signer)

        Return signers
    End Function

    Private Function DocumentMapper(biblosDocument As BiblosDocumentInfo, isMainDocument As Boolean, isLocked As Boolean) As DTO.Workflows.RequestStatementResult
        Dim viewModel As DTO.Workflows.RequestStatementResult = New DTO.Workflows.RequestStatementResult()
        Dim documentModel As DTO.Commons.DocumentModel = New DTO.Commons.DocumentModel()
        documentModel.Name = biblosDocument.Name
        documentModel.IdChain = biblosDocument.ChainId
        documentModel.IdDocument = biblosDocument.DocumentId
        documentModel.IsMainDocument = isMainDocument
        viewModel.Document = documentModel
        viewModel.HasSecureDocumentReference = RequestStatementType.Eq(uscRequestStatement.SECURE_DOCUMENT_STATEMENT) AndAlso biblosDocument.GetAttributeValue(REFERENCE_SECUREDOCUMENT_ATTRIBUTE_NAME) IsNot Nothing
        viewModel.HasSecurePaperReference = RequestStatementType.Eq(uscRequestStatement.SECURE_PAPER_STATEMENT) AndAlso biblosDocument.GetAttributeValue(REFERENCE_SECUREPAPER_ATTRIBUTE_NAME) IsNot Nothing
        viewModel.IsLocked = isLocked
        Return viewModel
    End Function

    Private Function PrepareRequest(documentUnit As DocumentUnit) As DocumentManagementRequestModel
        Dim statementRequest As DocumentManagementRequestModel = New DocumentManagementRequestModel()
        statementRequest.RegistrationUser = CurrentUser

        Dim workflowBiblosDocument As WorkflowReferenceBiblosModel
        For Each gridItem As GridDataItem In DocumentListGrid.SelectedItems
            workflowBiblosDocument = New WorkflowReferenceBiblosModel()
            workflowBiblosDocument.ArchiveChainId = Guid.Parse(gridItem("IdChain").Text)
            workflowBiblosDocument.ArchiveDocumentId = Guid.Parse(gridItem("IdDocument").Text)
            workflowBiblosDocument.DocumentName = gridItem("Name").Text
            workflowBiblosDocument.ArchiveName = ArchiveName
            statementRequest.Documents.Add(workflowBiblosDocument)
        Next

        statementRequest.DocumentUnit = New WorkflowReferenceModel()
        statementRequest.DocumentUnit.ReferenceId = documentUnit.UniqueId
        statementRequest.DocumentUnit.ReferenceType = UDType
        statementRequest.DocumentUnit.ReferenceModel = JsonConvert.SerializeObject(documentUnit)
        Return statementRequest
    End Function

    Public Sub LoadADSigner(defaultSignerAccount As String)
        Dim signers As IList(Of ContactDTO) = uscSigners.GetContacts(False)
        If signers Is Nothing OrElse signers.Count < 1 Then
            Dim signer As AccountModel = CommonAD.GetAccount(defaultSignerAccount)
            If signer IsNot Nothing Then
                Dim contact As Contact = New Contact()
                contact.Code = defaultSignerAccount
                contact.Description = signer.DisplayName
                contact.SearchCode = signer.Account
                contact.EmailAddress = signer.Email
                contact.ContactType = New Data.ContactType(Data.ContactType.Aoo)

                Dim serialized As String = JsonConvert.SerializeObject(contact)
                uscSigners.JsonContactAdded = serialized
                uscSigners.DataSource = New List(Of ContactDTO) From {New ContactDTO(contact, ContactDTO.ContactType.Manual)}
                uscSigners.DataBind()
                AddSigner(contact)
            End If
        End If
    End Sub

    Private Sub AddSigner(contact As Contact)
        InitializeSegreterie(contact)
        Dim lastAddedNode As RadTreeNode = uscSigners.TreeViewControl.Nodes(0).Nodes.FindNodeByText(contact.FullDescription(uscSigners.SimpleMode))
        If lastAddedNode IsNot Nothing Then
            lastAddedNode.Checked = True
        End If
        uscSigners.ButtonDeleteVisible = True
        uscSigners.ButtonSelectDomainVisible = False
        uscSigners.ButtonRoleVisible = False
        uscSigners.ButtonSelectVisible = False
    End Sub
#End Region

End Class
