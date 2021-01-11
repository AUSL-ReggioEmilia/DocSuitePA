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
            Dim selectedUDS As UDSRepository = CurrentUDSRepositoryFacade.GetById(SelectedUDSId.Value)
            If selectedUDS IsNot Nothing AndAlso Not String.IsNullOrEmpty(selectedUDS.ModuleXML) Then
                Dim model As UDSModel = UDSModel.LoadXml(selectedUDS.ModuleXML)
                If model.Model.Documents IsNot Nothing Then
                    If dto.VersioningDocumentGroup.Eq(VersioningDocumentGroup.MainDocument) OrElse dto.VersioningDocumentGroup.Eq(VersioningDocumentGroup.MainDocumentOmissis) Then
                        'Documento principale
                        If model.Model.Documents.Document IsNot Nothing Then
                            documentTypeModControl.Visible = True
                            documentTypeModControl.Items.Add(New DropDownListItem(model.Model.Documents.Document.Label, SERIES_MAINDOCUMENT_CODE))
                        Else
                            selectable = False
                            documentTypeNotFoundLabelControl.Visible = True
                            documentTypeNotFoundLabelControl.Text = "Nessun controllo definito nell'archivio per il documento principale"
                        End If
                    End If

                    If dto.VersioningDocumentGroup.Eq(VersioningDocumentGroup.Attachment) OrElse dto.VersioningDocumentGroup.Eq(VersioningDocumentGroup.AttachmentOmissis) Then
                        'Allegati
                        If model.Model.Documents.DocumentAttachment IsNot Nothing Then
                            documentTypeModControl.Visible = True
                            documentTypeModControl.Items.Add(New DropDownListItem(model.Model.Documents.DocumentAttachment.Label, SERIES_ATTACHMENT_CODE))
                        Else
                            selectable = False
                            documentTypeNotFoundLabelControl.Visible = True
                            documentTypeNotFoundLabelControl.Text = "Nessun controllo definito nell'archivio per gli allegati"
                        End If
                    End If

                    If dto.VersioningDocumentGroup.Eq(VersioningDocumentGroup.Annexed) Then
                        'Annesso
                        If model.Model.Documents.DocumentAnnexed IsNot Nothing Then
                            documentTypeModControl.Visible = True
                            documentTypeModControl.Items.Add(New DropDownListItem(model.Model.Documents.DocumentAnnexed.Label, SERIES_ANNEXED_CODE))
                        Else
                            selectable = False
                            documentTypeNotFoundLabelControl.Visible = True
                            documentTypeNotFoundLabelControl.Text = "Nessun controllo definito nell'archivio per gli annessi"
                        End If
                    End If
                End If
            End If
        End If



        e.Item.Selected = selectable
        If Not selectable Then
            e.Item.SelectableMode = GridItemSelectableMode.None
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

        btnConfirm.PostBackUrl = String.Concat("../UDS/UDSInsert.aspx?Type=UDS&Action=Insert&IdCollaboration=", CurrentCollaborationId)

        LoadAvailableUDS()
        BindPageFromCollaboration()
    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlUDSs, ddlUDSs, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlUDSs, pnlCollaborationDocuments, MasterDocSuite.AjaxDefaultLoadingPanel)
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
#End Region

End Class