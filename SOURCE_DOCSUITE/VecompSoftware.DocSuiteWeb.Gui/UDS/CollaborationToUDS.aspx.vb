Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.Linq
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.BusinessRule.Rules.Rights.UDS
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.Collaborations
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.UDS
Imports VecompSoftware.DocSuiteWeb.Model.Entities.UDS
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.UDS
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models

Public Class CollaborationToUDS
    Inherits UDSBasePage
    Implements IUDSInitializer

#Region "Fields"
    Private _currentCollaborationId As Integer?
    Private _currentCollaboration As Collaboration
    Private _currentUDSRepositoryId As Guid?
    Private Const OPEN_WINDOW_SCRIPT As String = "return {0}_OpenWindow('{1}', '{2}', '{3}');"
    Private Const SERIES_MAINDOCUMENT_CODE As String = "MAINDOC"
    Private Const SERIES_ANNEXED_CODE As String = "ANX"
    Private Const SERIES_ATTACHMENT_CODE As String = "ATT"
    Private _currentUDSRepositoryFacade As UDSRepositoryFacade
    Private _udsEnvironment As Integer?
    Private _currentUDSRepository As UDSRepository
#End Region

#Region "Properties"
    Public ReadOnly Property CurrentCollaborationId As Integer?
        Get
            If Not _currentCollaborationId.HasValue Then
                _currentCollaborationId = GetKeyValue(Of Integer?, CollaborationToUDS)("IdCollaboration")
            End If
            Return _currentCollaborationId
        End Get
    End Property

    Public ReadOnly Property UDSEnvironment As Integer?
        Get
            If Not _udsEnvironment.HasValue Then
                _udsEnvironment = GetKeyValue(Of Integer?, CollaborationToUDS)("UDSEnvironment")
            End If
            Return _udsEnvironment
        End Get
    End Property

    Public ReadOnly Property CurrentCollaboration As Collaboration
        Get
            If _currentCollaboration Is Nothing AndAlso CurrentCollaborationId.HasValue Then
                _currentCollaboration = Facade.CollaborationFacade.GetById(CurrentCollaborationId.Value)
            End If
            Return _currentCollaboration
        End Get
    End Property

    Public ReadOnly Property SelectedUDSId As Guid?
        Get
            If Not _currentUDSRepositoryId.HasValue AndAlso Not String.IsNullOrEmpty(ddlUDSs.SelectedValue) Then
                _currentUDSRepositoryId = Guid.Parse(ddlUDSs.SelectedValue)
            End If
            Return _currentUDSRepositoryId
        End Get
    End Property

    Private ReadOnly Property SelectedUDS As UDSRepository
        Get
            If _currentUDSRepository Is Nothing AndAlso SelectedUDSId.HasValue Then
                _currentUDSRepository = CurrentUDSRepositoryFacade.GetById(SelectedUDSId.Value)
            End If
            Return _currentUDSRepository
        End Get
    End Property

    Public ReadOnly Property SelectedMainDocument As IList(Of DocumentInfo)
        Get
            Dim mainDocuments As IList(Of DocumentInfo) = New List(Of DocumentInfo)
            Dim documentType As RadDropDownList
            Dim key As String
            Dim temp As NameValueCollection
            Dim documentInfo As DocumentInfo
            For Each item As GridDataItem In dgvDocuments.MasterTableView.GetSelectedItems()
                documentType = CType(item.FindControl("ddlDocumentType"), RadDropDownList)
                If documentType IsNot Nothing AndAlso documentType.SelectedValue.Eq(SERIES_MAINDOCUMENT_CODE) Then
                    key = item.GetDataKeyValue("BiblosSerializeKey").ToString()
                    temp = HttpUtility.ParseQueryString(key)
                    documentInfo = DocumentInfoFactory.BuildDocumentInfo(temp)
                    documentInfo.Name = FileHelper.ReplaceUnicode(documentInfo.Name)
                    mainDocuments.Add(documentInfo)
                End If
            Next
            Return mainDocuments
        End Get
    End Property

    Public ReadOnly Property SelectedAttachments As IList(Of DocumentInfo)
        Get
            Dim unpublishedAnnexed As IList(Of DocumentInfo) = New List(Of DocumentInfo)
            For Each item As GridDataItem In dgvDocuments.MasterTableView.GetSelectedItems()
                Dim documentType As RadDropDownList = CType(item.FindControl("ddlDocumentType"), RadDropDownList)
                If documentType IsNot Nothing AndAlso documentType.SelectedValue.Eq(SERIES_ATTACHMENT_CODE) Then
                    Dim key As String = item.GetDataKeyValue("BiblosSerializeKey").ToString()
                    Dim temp As NameValueCollection = HttpUtility.ParseQueryString(key)
                    Dim documentInfo As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(temp)
                    documentInfo.Name = FileHelper.ReplaceUnicode(documentInfo.Name)
                    unpublishedAnnexed.Add(documentInfo)
                End If
            Next
            Return unpublishedAnnexed
        End Get
    End Property

    Public ReadOnly Property SelectedAnnexed As IList(Of DocumentInfo)
        Get
            Dim annexed As IList(Of DocumentInfo) = New List(Of DocumentInfo)
            For Each item As GridDataItem In dgvDocuments.MasterTableView.GetSelectedItems()
                Dim documentType As RadDropDownList = CType(item.FindControl("ddlDocumentType"), RadDropDownList)
                If documentType IsNot Nothing AndAlso documentType.SelectedValue.Eq(SERIES_ANNEXED_CODE) Then
                    Dim key As String = item.GetDataKeyValue("BiblosSerializeKey").ToString()
                    Dim temp As NameValueCollection = HttpUtility.ParseQueryString(key)
                    Dim documentInfo As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(temp)
                    documentInfo.Name = FileHelper.ReplaceUnicode(documentInfo.Name)
                    annexed.Add(documentInfo)
                End If
            Next
            Return annexed
        End Get
    End Property

    Public Property IdFascicle As Guid?
        Get
            If ViewState(String.Format("{0}_IdFascicle", ID)) Is Nothing Then
                Return Nothing
            End If
            Return Guid.Parse(ViewState(String.Format("{0}_IdFascicle", ID)).ToString())
        End Get
        Set(value As Guid?)
            ViewState(String.Format("{0}_IdFascicle", ID)) = value
        End Set
    End Property
#End Region

#Region "Events"
    Protected Sub DgvDocuments_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles dgvDocuments.ItemDataBound
        If e.Item.ItemType = GridItemType.GroupHeader Then
            Dim item As GridGroupHeaderItem = DirectCast(e.Item, GridGroupHeaderItem)
            item.DataCell.Text = GetHeaderGroupLabel(item.DataCell.Text)
        End If

        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        ' imgDocumentExtensionType
        Dim dto As CollaborationDocument = DirectCast(e.Item.DataItem, CollaborationDocument)
        With DirectCast(e.Item.FindControl("imgDocumentExtensionType"), ImageButton)
            .ImageUrl = ImagePath.FromFile(dto.DocumentName, True)

            Dim url As String = ResolveUrl("~/Viewers/DocumentInfoViewer.aspx?" & CommonShared.AppendSecurityCheck(dto.BiblosSerializeKey))
            .OnClientClick = String.Format(OPEN_WINDOW_SCRIPT, ID, url, windowPreviewDocument.ClientID, "")
        End With

        With DirectCast(e.Item.FindControl("lblDocumentName"), Label)
            .Text = dto.DocumentName
        End With

        Dim documentTypeNotFoundLabelControl As Label = DirectCast(e.Item.FindControl("lblDocumentTypeNotFound"), Label)
        Dim documentTypeModControl As RadDropDownList = DirectCast(e.Item.FindControl("ddlDocumentType"), RadDropDownList)
        documentTypeModControl.Items.Clear()
        documentTypeModControl.Visible = False
        documentTypeNotFoundLabelControl.Visible = False
        Dim selectable As Boolean = True

        If (SelectedUDSId.HasValue) Then
            If SelectedUDS IsNot Nothing AndAlso Not String.IsNullOrEmpty(SelectedUDS.ModuleXML) Then
                Dim model As UDSModel = UDSModel.LoadXml(SelectedUDS.ModuleXML)
                If model.Model.Documents IsNot Nothing Then
                    selectable = SetSelectableDocumentTypes(e, dto, model)
                Else
                    selectable = False
                    documentTypeNotFoundLabelControl.Visible = True
                    documentTypeNotFoundLabelControl.Text = "Nessun controllo definito nell'archivio per il documento principale"
                End If
            End If
        End If

        e.Item.Selected = selectable
        If Not selectable Then
            e.Item.SelectableMode = GridItemSelectableMode.None
        End If
    End Sub

    Protected Sub DdlDocumentType_SelectedIndexChanged(ByVal sender As Object, ByVal e As DropDownListEventArgs)
        If String.IsNullOrEmpty(e.Value) OrElse SelectedUDS Is Nothing Then
            Return
        End If

        Dim model As UDSModel = UDSModel.LoadXml(SelectedUDS.ModuleXML)
        Dim currentItem As RadDropDownList = DirectCast(sender, RadDropDownList)
        If Not CheckDocumentTypeMultiFileAllowed(e.Value, model) Then
            ResetDocumentTypeSelectionExcept(e.Value, currentItem)
        End If
    End Sub

    Protected Sub DdlSeries_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlUDSs.SelectedIndexChanged
        BindCollaborationDocumentsGrid()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub
#End Region

#Region "Methods"
    Private Sub Initialize()
        If CurrentCollaboration Is Nothing Then
            Throw New DocSuiteException(String.Format("Collaborazione {0}", CurrentCollaborationId), "Nessuna collaborazione trovata per l'ID passato")
        End If

        windowPreviewDocument.Height = Unit.Pixel(ProtocolEnv.DocumentPreviewHeight)
        windowPreviewDocument.Width = Unit.Pixel(ProtocolEnv.DocumentPreviewWidth)

        LoadAvailableUDS()
        BindPageFromCollaboration()

        btnConfirm.PostBackUrl = String.Format("../UDS/UDSInsert.aspx?Type=UDS&Action=Insert&IdCollaboration={0}&ArchiveTypeId={1}", CurrentCollaborationId, ddlUDSs.SelectedValue)
    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlUDSs, ddlUDSs, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlUDSs, pnlCollaborationDocuments, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(dgvDocuments, dgvDocuments, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub BindPageFromCollaboration()
        lblCollaborationSubject.Text = CurrentCollaboration.CollaborationObject
        lblNote.Text = CurrentCollaboration.Note

        BindCollaborationDocumentsGrid()
    End Sub

    Private Sub LoadAvailableUDS()
        Dim results As ICollection(Of UDSRepository) = CurrentUDSRepositoryFacade.GetFascicolable()
        If results.IsNullOrEmpty() Then
            Exit Sub
        End If

        Dim authorizedRepositories As ICollection(Of UDSRepository) = New List(Of UDSRepository)()
        Dim currentRepositoryRigths As UDSRepositoryRightsUtil = Nothing
        Dim dto As UDSDto = Nothing
        For Each repository As UDSRepository In results
            dto = New UDSDto() With {.UDSModel = UDSModel.LoadXml(repository.ModuleXML)}
            currentRepositoryRigths = New UDSRepositoryRightsUtil(repository, DocSuiteContext.Current.User.FullUserName, dto)
            If currentRepositoryRigths.IsInsertable Then
                authorizedRepositories.Add(repository)
            End If
        Next

        ddlUDSs.DataValueField = "Id"
        ddlUDSs.DataTextField = "Name"
        ddlUDSs.DataSource = authorizedRepositories
        ddlUDSs.DataBind()

        If UDSEnvironment.HasValue Then
            Dim repository As UDSRepository = authorizedRepositories.FirstOrDefault(Function(x) x.DSWEnvironment = UDSEnvironment.Value)
            If repository IsNot Nothing Then
                ddlUDSs.SelectedValue = repository.Id.ToString()
                DdlSeries_SelectedIndexChanged(Me.Page, Nothing)
                ddlUDSs.Enabled = False
                Exit Sub
            Else
                AjaxAlert(String.Concat("Attenzione! Non è stato trovato un Archivio utilizzabile con ID", UDSEnvironment, ", oppure l'archivio non è abilitato alla Fascicolazione"))
        End If
        End If

        If (authorizedRepositories.Count = 1) Then
            ddlUDSs.SelectedIndex = 0
            DdlSeries_SelectedIndexChanged(Me.Page, Nothing)
        Else
            ddlUDSs.Items.Insert(0, New DropDownListItem("", ""))
        End If
    End Sub

    Private Sub BindCollaborationDocumentsGrid()
        Dim collaborationDocuments As List(Of CollaborationDocument) = New List(Of CollaborationDocument)

        collaborationDocuments.AddRange(Facade.CollaborationVersioningFacade.GetLastVersionDocumentDtos(CurrentCollaboration, VersioningDocumentGroup.MainDocument))
        collaborationDocuments.AddRange(Facade.CollaborationVersioningFacade.GetLastVersionDocumentDtos(CurrentCollaboration, VersioningDocumentGroup.MainDocumentOmissis))
        collaborationDocuments.AddRange(Facade.CollaborationVersioningFacade.GetLastVersionDocumentDtos(CurrentCollaboration, VersioningDocumentGroup.Attachment))
        collaborationDocuments.AddRange(Facade.CollaborationVersioningFacade.GetLastVersionDocumentDtos(CurrentCollaboration, VersioningDocumentGroup.AttachmentOmissis))
        collaborationDocuments.AddRange(Facade.CollaborationVersioningFacade.GetLastVersionDocumentDtos(CurrentCollaboration, VersioningDocumentGroup.Annexed))

        dgvDocuments.DataSource = collaborationDocuments
        dgvDocuments.DataBind()
    End Sub

    Private Shared Function GetHeaderGroupLabel(source As String) As String
        Dim splitted As String() = source.Split(":"c)
        Select Case splitted(1).Trim().ToString()
            Case VersioningDocumentGroup.MainDocument
                Return "Documento Principale"
            Case VersioningDocumentGroup.MainDocumentOmissis
                Return "Documento Principale Omissis"
            Case VersioningDocumentGroup.Attachment
                Return "Allegato"
            Case VersioningDocumentGroup.AttachmentOmissis
                Return "Allegato Omissis"
            Case VersioningDocumentGroup.Annexed
                Return "Annesso"
            Case Else
                Return String.Empty
        End Select
    End Function

    Public Function GetUDSInitializer() As UDSDto Implements IUDSInitializer.GetUDSInitializer
        If (Not SelectedUDSId.HasValue) Then
            Throw New DocSuiteException("Impossibile proseguire con la gestione dell'archivio per un problema applicativo interno. Contattare l'assistenza.")
        End If

        Dim dto As UDSDto = New UDSDto()
        dto.Id = SelectedUDSId.Value

        Dim selectedUDS As UDSRepository = CurrentUDSRepositoryFacade.GetById(dto.Id)
        dto.UDSRepository = New UDSEntityRepositoryDto() With {.UniqueId = selectedUDS.Id, .Name = selectedUDS.Name}
        Dim model As UnitaDocumentariaSpecifica = UDSModel.LoadXml(selectedUDS.ModuleXML).Model
        model.Subject.Value = CurrentCollaboration.CollaborationObject

        Dim collaborationDraft As CollaborationDraft = Facade.CollaborationDraftFacade.GetFromCollaboration(CurrentCollaboration)

        If collaborationDraft IsNot Nothing AndAlso String.IsNullOrEmpty(collaborationDraft.Data) = False Then
            Dim udsWorkflowModel As UDSWorkflowModel = JsonConvert.DeserializeObject(Of UDSWorkflowModel)(collaborationDraft.Data)

            If udsWorkflowModel.Fascicle IsNot Nothing Then
                IdFascicle = udsWorkflowModel.Fascicle.UniqueId
            End If

            If model.Metadata IsNot Nothing AndAlso model.Metadata.Any() Then
                Dim metadata As Section = model.Metadata.First()
                For Each dynamicData As KeyValuePair(Of String, String) In udsWorkflowModel.DynamicDatas
                    Dim metadataField As FieldBaseType = metadata.Items.FirstOrDefault(Function(x) x.ColumnName = dynamicData.Key)
                    If metadataField Is Nothing OrElse String.IsNullOrEmpty(dynamicData.Value) Then
                        Continue For
                    End If

                    If TypeOf metadataField Is TextField Then
                        DirectCast(metadataField, TextField).Value = dynamicData.Value
                    ElseIf TypeOf metadataField Is DateField Then
                        DirectCast(metadataField, DateField).ValueSpecified = True
                        DirectCast(metadataField, DateField).Value = Date.Parse(dynamicData.Value)
                    End If
                Next
            End If

            If model.Contacts.Any() AndAlso udsWorkflowModel.Contact IsNot Nothing AndAlso udsWorkflowModel.Contact.EntityId.HasValue Then
                model.Contacts.First().ContactInstances = New List(Of ContactInstance) From {
                    New ContactInstance With {
                        .IdContact = udsWorkflowModel.Contact.EntityId.Value,
                        .UniqueId = udsWorkflowModel.Contact.UniqueId.ToString()
                    }
                }.ToArray()
            End If
        End If

        'Se il documento principale non è stato valorizzato significa che non si tratta di caricamento automatico da organigramma
        If model.Documents Is Nothing OrElse model.Documents.Document Is Nothing OrElse model.Documents.Document.Instances Is Nothing Then

            If model.Documents IsNot Nothing Then

                If model.Documents.Document IsNot Nothing Then
                    Dim mainDocuments As IList(Of DocumentInfo) = SelectedMainDocument
                    If mainDocuments IsNot Nothing Then
                        model.Documents.Document.Instances = GetDocumentInstances(mainDocuments)
                    End If
                End If

                If model.Documents.DocumentAttachment IsNot Nothing Then
                    Dim attachments As IList(Of DocumentInfo) = SelectedAttachments
                    If attachments IsNot Nothing Then
                        model.Documents.DocumentAttachment.Instances = GetDocumentInstances(attachments)
                    End If
                End If

                If model.Documents.DocumentAnnexed IsNot Nothing Then
                    Dim annexed As IList(Of DocumentInfo) = SelectedAnnexed
                    If annexed IsNot Nothing Then
                        model.Documents.DocumentAnnexed.Instances = GetDocumentInstances(annexed)
                    End If
                End If

            End If
        End If

        dto.UDSModel = New UDSModel(model)
        Return dto
    End Function

    Private Function GetIdFascicle() As Guid? Implements IUDSInitializer.GetIdFascicle
        Return IdFascicle
    End Function

    Private Function GetDocumentInstances(documents As IList(Of DocumentInfo)) As DocumentInstance()
        Dim documentInstances As IList(Of DocumentInstance) = New List(Of DocumentInstance)
        If documents.Any() Then
            Dim documentStored As BiblosDocumentInfo = Nothing
            For Each document As DocumentInfo In documents
                If TypeOf document Is BiblosPdfDocumentInfo Then
                    documentStored = document.ArchiveInBiblos(CommonShared.CurrentWorkflowLocation.ProtBiblosDSDB, Guid.Empty)
                    documentInstances.Add(New DocumentInstance() With {.IdDocumentToStore = documentStored.DocumentId.ToString(), .DocumentName = document.Name})
                Else
                    documentInstances.Add(New DocumentInstance() With {.StoredChainId = DirectCast(document, BiblosDocumentInfo).DocumentId.ToString()})
                End If
            Next
        End If
        Return documentInstances.ToArray()
    End Function

    Public Function SetSelectableDocumentTypes(e As GridItemEventArgs, dto As CollaborationDocument, model As UDSModel) As Boolean
        If model.Model.DocumentTypeCoherencyInArchivingCollaborationDisabledSpecified AndAlso model.Model.DocumentTypeCoherencyInArchivingCollaborationDisabled Then
            Return SetSelectableDocumentTypesByDesigner(e, dto, model)
        Else
            Return SetSelectableDocumentTypesByCoherency(e, dto, model)
        End If
    End Function

    Private Function SetSelectableDocumentTypesByCoherency(e As GridItemEventArgs, dto As CollaborationDocument, model As UDSModel) As Boolean
        Dim selectable As Boolean = True
        Dim documentTypeNotFoundLabelControl As Label = DirectCast(e.Item.FindControl("lblDocumentTypeNotFound"), Label)
        Dim documentTypeModControl As RadDropDownList = DirectCast(e.Item.FindControl("ddlDocumentType"), RadDropDownList)
        Select Case dto.VersioningDocumentGroup
            Case VersioningDocumentGroup.MainDocument,
                VersioningDocumentGroup.MainDocumentOmissis
                If model.Model.Documents.Document IsNot Nothing Then
                    documentTypeModControl.Visible = True
                    documentTypeModControl.Items.Add(New DropDownListItem(model.Model.Documents.Document.Label, SERIES_MAINDOCUMENT_CODE))
                Else
                    selectable = False
                    documentTypeNotFoundLabelControl.Visible = True
                    documentTypeNotFoundLabelControl.Text = "Nessun controllo definito nell'archivio per il documento principale"
                End If

            Case VersioningDocumentGroup.Attachment,
                VersioningDocumentGroup.AttachmentOmissis
                If model.Model.Documents.DocumentAttachment IsNot Nothing Then
                    documentTypeModControl.Visible = True
                    documentTypeModControl.Items.Add(New DropDownListItem(model.Model.Documents.DocumentAttachment.Label, SERIES_ATTACHMENT_CODE))
                Else
                    selectable = False
                    documentTypeNotFoundLabelControl.Visible = True
                    documentTypeNotFoundLabelControl.Text = "Nessun controllo definito nell'archivio per gli allegati"
                End If

            Case VersioningDocumentGroup.Annexed
                If model.Model.Documents.DocumentAnnexed IsNot Nothing Then
                    documentTypeModControl.Visible = True
                    documentTypeModControl.Items.Add(New DropDownListItem(model.Model.Documents.DocumentAnnexed.Label, SERIES_ANNEXED_CODE))
                Else
                    selectable = False
                    documentTypeNotFoundLabelControl.Visible = True
                    documentTypeNotFoundLabelControl.Text = "Nessun controllo definito nell'archivio per gli annessi"
                End If

        End Select

        Return selectable
    End Function

    Private Function SetSelectableDocumentTypesByDesigner(e As GridItemEventArgs, dto As CollaborationDocument, model As UDSModel) As Boolean
        Dim documentTypeModControl As RadDropDownList = DirectCast(e.Item.FindControl("ddlDocumentType"), RadDropDownList)

        documentTypeModControl.Visible = True
        documentTypeModControl.Items.Add(New DropDownListItem(String.Empty, String.Empty))
        If model.Model.Documents.Document IsNot Nothing Then
            documentTypeModControl.Items.Add(New DropDownListItem(model.Model.Documents.Document.Label, SERIES_MAINDOCUMENT_CODE))
        End If

        If model.Model.Documents.DocumentAttachment IsNot Nothing Then
            documentTypeModControl.Items.Add(New DropDownListItem(model.Model.Documents.DocumentAttachment.Label, SERIES_ATTACHMENT_CODE))
        End If

        If model.Model.Documents.DocumentAnnexed IsNot Nothing Then
            documentTypeModControl.Items.Add(New DropDownListItem(model.Model.Documents.DocumentAnnexed.Label, SERIES_ANNEXED_CODE))
        End If
        documentTypeModControl.AutoPostBack = True

        Select Case dto.VersioningDocumentGroup
            Case VersioningDocumentGroup.MainDocument,
                VersioningDocumentGroup.MainDocumentOmissis
                documentTypeModControl.SelectedValue = SERIES_MAINDOCUMENT_CODE

            Case VersioningDocumentGroup.Attachment,
                VersioningDocumentGroup.AttachmentOmissis
                documentTypeModControl.SelectedValue = SERIES_ATTACHMENT_CODE

            Case VersioningDocumentGroup.Annexed
                documentTypeModControl.SelectedValue = SERIES_ANNEXED_CODE

        End Select

        If Not CheckDocumentTypeMultiFileAllowed(documentTypeModControl.SelectedValue, model) AndAlso
            dgvDocuments.MasterTableView.Items.Cast(Of GridDataItem) _
                .Any(Function(x) CType(x.FindControl("ddlDocumentType"), RadDropDownList).SelectedValue.Eq(documentTypeModControl.SelectedValue)) Then

            documentTypeModControl.SelectedValue = String.Empty
        End If

        DdlDocumentType_SelectedIndexChanged(documentTypeModControl, New DropDownListEventArgs(documentTypeModControl.SelectedIndex, documentTypeModControl.SelectedText, documentTypeModControl.SelectedValue))

        Return True
    End Function

    Private Function CheckDocumentTypeMultiFileAllowed(documentType As String, model As UDSModel) As Boolean
        If model.Model.Documents IsNot Nothing Then
            Select Case documentType
                Case SERIES_MAINDOCUMENT_CODE
                    Return model.Model.Documents.Document IsNot Nothing AndAlso model.Model.Documents.Document.AllowMultiFile
                Case SERIES_ATTACHMENT_CODE
                    Return model.Model.Documents.DocumentAttachment IsNot Nothing AndAlso model.Model.Documents.DocumentAttachment.AllowMultiFile
                Case SERIES_ANNEXED_CODE
                    Return model.Model.Documents.DocumentAnnexed IsNot Nothing AndAlso model.Model.Documents.DocumentAnnexed.AllowMultiFile
            End Select
        End If
        Return False
    End Function

    Private Sub ResetDocumentTypeSelectionExcept(documentType As String, exceptItem As RadDropDownList)
        Dim ddlItem As RadDropDownList
        For Each item As GridDataItem In dgvDocuments.MasterTableView.Items
            ddlItem = CType(item.FindControl("ddlDocumentType"), RadDropDownList)
            If ddlItem IsNot Nothing AndAlso ddlItem.SelectedValue.Eq(documentType) AndAlso Not ddlItem.ClientID.Eq(exceptItem.ClientID) Then
                ddlItem.ClearSelection()
            End If
        Next
    End Sub
#End Region

End Class