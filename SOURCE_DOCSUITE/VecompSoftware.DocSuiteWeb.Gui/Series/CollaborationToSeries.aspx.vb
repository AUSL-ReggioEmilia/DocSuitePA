Imports System.Collections.Generic
Imports System.Linq
Imports System.Collections.Specialized
Imports VecompSoftware.Helpers
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Helpers.ExtensionMethods
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.DTO.Collaborations
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Web
Imports VecompSoftware.Services.Biblos.Models

Public Class CollaborationToSeries
    Inherits CommonBasePage

#Region "Fields"
    Private _currentCollaborationId As Integer?
    Private _currentCollaboration As Collaboration
    Private _availableContainers As ICollection(Of Container)
    Private Const OPEN_WINDOW_SCRIPT As String = "return {0}_OpenWindow('{1}', '{2}', '{3}');"
    Private Const SERIES_MAINDOCUMENT_CODE As String = "MAINDOC"
    Private Const SERIES_ANNEXED_CODE As String = "ANX"
    Private Const SERIES_UNPUBLISHED_ANNEXED_CODE As String = "UANX"
#End Region

#Region "Properties"
    Public ReadOnly Property CurrentCollaborationId As Integer?
        Get
            If Not _currentCollaborationId.HasValue Then
                _currentCollaborationId = GetKeyValue(Of Integer?, CollaborationToSeries)("IdCollaboration")
            End If
            Return _currentCollaborationId
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

    Private ReadOnly Property AvailableContainer() As ICollection(Of Container)
        Get
            If _availableContainers Is Nothing Then
                _availableContainers = Facade.ContainerFacade.GetContainers(DSWEnvironment.DocumentSeries, New List(Of Integer)({DocumentSeriesContainerRightPositions.Insert, DocumentSeriesContainerRightPositions.Draft}), True)
            End If
            Return _availableContainers
        End Get
    End Property

    Public Property CurrentDocumentSeries As DocumentSeries
        Get
            Return DirectCast(ViewState("CurrentDocumentSeries"), DocumentSeries)
        End Get
        Set(value As DocumentSeries)
            ViewState("CurrentDocumentSeries") = value
        End Set
    End Property

    Public ReadOnly Property SelectedMainDocument As IList(Of DocumentInfo)
        Get
            Dim mainDocuments As IList(Of DocumentInfo) = New List(Of DocumentInfo)
            For Each item As GridDataItem In dgvDocuments.MasterTableView.GetSelectedItems()
                Dim documentType As RadDropDownList = CType(item.FindControl("ddlDocumentType"), RadDropDownList)
                If documentType IsNot Nothing AndAlso documentType.SelectedValue.Eq(SERIES_MAINDOCUMENT_CODE) Then
                    Dim key As String = item.GetDataKeyValue("BiblosSerializeKey").ToString()
                    Dim temp As NameValueCollection = HttpUtility.ParseQueryString(key)
                    Dim documentInfo As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(temp)
                    documentInfo.Name = FileHelper.ReplaceUnicode(documentInfo.Name)
                    mainDocuments.Add(documentInfo)
                End If
            Next
            Return mainDocuments
        End Get
    End Property

    Public ReadOnly Property SelectedUnpublishedAnnexed As IList(Of DocumentInfo)
        Get
            Dim unpublishedAnnexed As IList(Of DocumentInfo) = New List(Of DocumentInfo)
            For Each item As GridDataItem In dgvDocuments.MasterTableView.GetSelectedItems()
                Dim documentType As RadDropDownList = CType(item.FindControl("ddlDocumentType"), RadDropDownList)
                If documentType IsNot Nothing AndAlso documentType.SelectedValue.Eq(SERIES_UNPUBLISHED_ANNEXED_CODE) Then
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

        Dim documentTypeModControl As RadDropDownList = DirectCast(e.Item.FindControl("ddlDocumentType"), RadDropDownList)
        documentTypeModControl.Items.Clear()
        documentTypeModControl.Visible = False

        If CurrentDocumentSeries IsNot Nothing AndAlso CurrentDocumentSeries.Container IsNot Nothing Then
            'Documento principale
            If CurrentDocumentSeries.Container.DocumentSeriesLocation IsNot Nothing Then
                documentTypeModControl.Visible = True
                Dim mainCaption As String = String.Empty
                If (ProtocolEnv.DocumentSeriesDocumentsLabel.ContainsKey(Model.Entities.DocumentUnits.ChainType.MainChain)) Then
                    mainCaption = ProtocolEnv.DocumentSeriesDocumentsLabel(Model.Entities.DocumentUnits.ChainType.MainChain)
                End If
                documentTypeModControl.Items.Add(New DropDownListItem(mainCaption, SERIES_MAINDOCUMENT_CODE))
            End If

            'Annesso
            If CurrentDocumentSeries.Container.DocumentSeriesAnnexedLocation IsNot Nothing Then
                documentTypeModControl.Visible = True
                Dim annexedCaption As String = String.Empty
                If (ProtocolEnv.DocumentSeriesDocumentsLabel.ContainsKey(Model.Entities.DocumentUnits.ChainType.AnnexedChain)) Then
                    annexedCaption = ProtocolEnv.DocumentSeriesDocumentsLabel(Model.Entities.DocumentUnits.ChainType.AnnexedChain)
                End If
                documentTypeModControl.Items.Add(New DropDownListItem(annexedCaption, SERIES_ANNEXED_CODE))
            End If

            'Annesso da non pubblicare
            If CurrentDocumentSeries.Container.DocumentSeriesAnnexedLocation IsNot Nothing Then
                documentTypeModControl.Visible = True
                Dim unpublishedAnnexedCaption As String = String.Empty
                If (ProtocolEnv.DocumentSeriesDocumentsLabel.ContainsKey(Model.Entities.DocumentUnits.ChainType.UnpublishedAnnexedChain)) Then
                    unpublishedAnnexedCaption = ProtocolEnv.DocumentSeriesDocumentsLabel(Model.Entities.DocumentUnits.ChainType.UnpublishedAnnexedChain)
                End If
                documentTypeModControl.Items.Add(New DropDownListItem(unpublishedAnnexedCaption, SERIES_UNPUBLISHED_ANNEXED_CODE))
            End If
        End If

        e.Item.Selected = True
    End Sub

    Protected Sub DdlContainers_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlContainers.SelectedIndexChanged
        CurrentDocumentSeries = Nothing
        LoadAvailableDocumentSeries()
        BindCollaborationDocumentsGrid()
    End Sub

    Protected Sub DdlSeries_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlSeries.SelectedIndexChanged
        CurrentDocumentSeries = Facade.DocumentSeriesFacade.GetDocumentSeries(Integer.Parse(ddlSeries.SelectedValue))
        If CurrentDocumentSeries.SubsectionEnabled.GetValueOrDefault(False) Then
            pnlSubSection.Visible = True
            ' Carico le Subsections 
            DataBindAvailableSubsection()
        Else
            pnlSubSection.Visible = False
            subSectionValidation.Enabled = False
        End If

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

        Title = String.Format("Collaborazione - inserimento in {0}", ProtocolEnv.DocumentSeriesName)
        lblSeriesSelectionTitle.Text = ProtocolEnv.DocumentSeriesName
        lblDocumentSeriesTitle.Text = ProtocolEnv.DocumentSeriesName
        ddlSeries.DefaultMessage = String.Format("Seleziona {0}", ProtocolEnv.DocumentSeriesName)

        windowPreviewDocument.Height = Unit.Pixel(ProtocolEnv.DocumentPreviewHeight)
        windowPreviewDocument.Width = Unit.Pixel(ProtocolEnv.DocumentPreviewWidth)

        btnConfirm.PostBackUrl = String.Format("../Series/Item.aspx?{0}", CommonShared.AppendSecurityCheck(String.Format("Type=Series&Action={0}", DocumentSeriesAction.FromCollaboration.ToString())))

        pnlSubSection.Visible = False
        subSectionValidation.Enabled = False

        InitializeSeries()
        BindPageFromCollaboration()
    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlContainers, ddlContainers, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlContainers, ddlSeries)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlContainers, pnlSubSection)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlContainers, ddlSubsection)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlContainers, subSectionValidation)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlContainers, pnlCollaborationDocuments, MasterDocSuite.AjaxDefaultLoadingPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(ddlSeries, ddlSeries, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlSeries, pnlSubSection)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlSeries, ddlSubsection)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlSeries, pnlCollaborationDocuments, MasterDocSuite.AjaxDefaultLoadingPanel)

        AjaxManager.ClientEvents.OnResponseEnd = "responseEnd"
    End Sub

    Private Sub BindPageFromCollaboration()
        lblCollaborationSubject.Text = CurrentCollaboration.CollaborationObject
        lblNote.Text = CurrentCollaboration.Note

        BindCollaborationDocumentsGrid()
    End Sub

    Private Sub InitializeSeries()
        LoadContainers()
        LoadAvailableDocumentSeries()
    End Sub

    Private Sub LoadContainers()
        Dim results As IEnumerable(Of ContainerArchive) = AvailableContainer.Where(Function(c) c.Archive IsNot Nothing).Select(Function(c) c.Archive).Distinct()
        ddlContainers.DataValueField = "Id"
        ddlContainers.DataTextField = "Name"
        ddlContainers.DataSource = results
        ddlContainers.DataBind()
        If Not results.IsNullOrEmpty() AndAlso results.Count = 1 Then
            ddlContainers.SelectedIndex = 0
        Else
            ddlContainers.Items.Insert(0, New DropDownListItem("", ""))
        End If
    End Sub

    Private Sub LoadAvailableDocumentSeries()
        Dim results As IEnumerable(Of Container) = AvailableContainer.Where(Function(container) String.IsNullOrEmpty(ddlContainers.SelectedValue) OrElse (container.Archive IsNot Nothing AndAlso container.Archive.Id = Integer.Parse(ddlContainers.SelectedValue))).ToList()

        ddlSeries.DataValueField = "Id"
        ddlSeries.DataTextField = "Name"
        ddlSeries.DataSource = results
        ddlSeries.DataBind()
        If (Not results.IsNullOrEmpty AndAlso results.Count() = 1) Then
            ddlSeries.SelectedIndex = 0
            CurrentDocumentSeries = Facade.DocumentSeriesFacade.GetDocumentSeries(Integer.Parse(ddlSeries.SelectedValue))
            DdlSeries_SelectedIndexChanged(Me.Page, Nothing)
        Else
            ddlSeries.Items.Insert(0, New DropDownListItem("", ""))
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

    Private Sub DataBindAvailableSubsection()
        ' Gestisco le sotto-sezioni
        pnlSubSection.Visible = False
        subSectionValidation.Enabled = False
        If CurrentDocumentSeries IsNot Nothing AndAlso CurrentDocumentSeries.SubsectionEnabled Then
            pnlSubSection.Visible = True
            subSectionValidation.Enabled = True
            Dim subsections As IList(Of DocumentSeriesSubsection) = Facade.DocumentSeriesSubsectionFacade.GetByDocumentSeries(CurrentDocumentSeries)
            ddlSubsection.DataSource = subsections
            ddlSubsection.DataBind()
            ddlSubsection.Items.Insert(0, New DropDownListItem(String.Empty, String.Empty))
            ddlSubsection.SelectedIndex = 0
        End If
    End Sub
#End Region

End Class