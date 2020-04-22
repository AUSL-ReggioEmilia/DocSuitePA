Imports System.Collections.Generic
Imports System.ComponentModel
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Web
Imports System.Linq
Imports VecompSoftware.Services.Biblos.Models

Namespace Resl

    Public Class ToSeries
        Inherits ReslBasePage

        Public Enum ChainType
            <Description("Documenti")>
            Document
            <Description("Annessi")>
            Annexed
            <Description("Annessi da non pubblicare")>
            UnpublishedAnnexed
        End Enum

#Region " Fields "

        Private _containerChainTypes As List(Of ChainType) = Nothing

        Private _allDocuments As List(Of DocumentInfo)

        Private _currentDocumentSeriesId As String
        Private Const DocumentSeriesSessionToken As String = "CurrentDocumentSeriesId"

#End Region

#Region " Properties "

        ''' <summary> Incrementale desiderato. </summary>
        ''' <remarks> Aggiunto come property casomai fosse necessario in futuro metterlo in querystring. </remarks>
        Private ReadOnly Property CurrentIncremental() As Short
            Get
                Return Facade.ResolutionWorkflowFacade.GetActiveIncremental(IdResolution, 1)
            End Get
        End Property

        Private ReadOnly Property AllDocuments() As List(Of DocumentInfo)
            Get
                If _allDocuments Is Nothing Then
                    _allDocuments = New List(Of DocumentInfo)
                    _allDocuments.AddRange(GetWithDummyParent("Documenti", Facade.ResolutionFacade.GetDocuments(CurrentResolution, CurrentIncremental)))

                    ''Gestione documenti adottati

                    Dim tabAdozione As TabWorkflow = Facade.TabWorkflowFacade.GetByDescription("Adozione", CurrentResolution.WorkflowType)
                    Dim tabPubblicazione As TabWorkflow = Facade.TabWorkflowFacade.GetByDescription("Pubblicazione", CurrentResolution.WorkflowType)
                    If tabAdozione IsNot Nothing AndAlso tabPubblicazione IsNot Nothing AndAlso CurrentIncremental = tabPubblicazione.Id.ResStep Then
                        Dim docs As BiblosDocumentInfo() = Facade.ResolutionFacade.GetDocuments(CurrentResolution, tabAdozione.Id.ResStep)
                        If Not docs.IsNullOrEmpty() Then
                            _allDocuments.AddRange(GetWithDummyParent("Documenti", docs))
                        End If

                    End If

                    '' Gestione Documenti Omissis
                    If ResolutionEnv.MainDocumentOmissisEnable Then
                        _allDocuments.AddRange(GetWithDummyParent("Documenti Omissis", Facade.ResolutionFacade.GetDocumentsOmissis(CurrentResolution, CurrentIncremental, False)))
                    End If
                    _allDocuments.AddRange(GetWithDummyParent("Allegati", Facade.ResolutionFacade.GetAttachments(CurrentResolution, CurrentIncremental, False)))
                    '' Gestione Allegati Omissis
                    If ResolutionEnv.AttachmentOmissisEnable Then
                        _allDocuments.AddRange(GetWithDummyParent("Allegati Omissis", Facade.ResolutionFacade.GetAttachmentsOmissis(CurrentResolution, CurrentIncremental, False)))
                    End If
                    _allDocuments.AddRange(GetWithDummyParent("Annessi", Facade.ResolutionFacade.GetAnnexes(CurrentResolution, CurrentIncremental)))
                End If

                Return _allDocuments
            End Get
        End Property

        Public ReadOnly Property SelectedDocuments() As Dictionary(Of DocumentInfo, ChainType)
            Get
                Dim selected As New Dictionary(Of DocumentInfo, ChainType)
                For Each item As GridDataItem In DocumentListGrid.MasterTableView.GetSelectedItems()
                    Dim documentInfo As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(CType(item.GetDataKeyValue("Serialized"), String)))
                    If DirectCast(item.FindControl("pdf"), RadButton).Checked AndAlso TypeOf documentInfo Is BiblosDocumentInfo Then
                        documentInfo = New BiblosPdfDocumentInfo(DirectCast(documentInfo, BiblosDocumentInfo))
                    Else
                        documentInfo.Signature = String.Empty
                    End If
                    Dim chainType As ChainType = DirectCast([Enum].Parse(GetType(ChainType), DirectCast(item.FindControl("chainTypes"), DropDownList).SelectedValue), ChainType)
                    selected.Add(documentInfo, chainType)
                Next
                Return selected
            End Get
        End Property

        Public Property SelectedDocumentSeriesId As String
            Get
                If String.IsNullOrEmpty(_currentDocumentSeriesId) Then
                    If Session(DocumentSeriesSessionToken) IsNot Nothing Then
                        _currentDocumentSeriesId = CType(Session(DocumentSeriesSessionToken), String)
                    Else
                        _currentDocumentSeriesId = ddlDocumentSeries.SelectedValue
                        Session(DocumentSeriesSessionToken) = _currentDocumentSeriesId
                    End If
                End If
                Return _currentDocumentSeriesId
            End Get
            Set(ByVal value As String)
                _currentDocumentSeriesId = ddlDocumentSeries.SelectedValue
                Session(DocumentSeriesSessionToken) = _currentDocumentSeriesId
            End Set
        End Property

        ''' <summary> Tipi di catene nelle quali inserire i documenti per il container selezionato </summary>
        Private ReadOnly Property ContainerChainTypes As List(Of ChainType)
            Get
                If _containerChainTypes Is Nothing AndAlso Not String.IsNullOrEmpty(SelectedDocumentSeriesId) Then
                    _containerChainTypes = New List(Of ChainType)()
                    _containerChainTypes.Add(ChainType.Document)
                    Dim container As Data.Container = Facade.ContainerFacade.GetById(Integer.Parse(SelectedDocumentSeriesId))
                    If container.DocumentSeriesAnnexedLocation IsNot Nothing Then
                        _containerChainTypes.Add(ChainType.Annexed)
                    End If
                    If container.DocumentSeriesUnpublishedAnnexedLocation IsNot Nothing Then
                        _containerChainTypes.Add(ChainType.UnpublishedAnnexed)
                    End If
                End If

                Return _containerChainTypes
            End Get
        End Property

#End Region

#Region " Events "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            SetResponseNoCache()
            btnConfirm.PostBackUrl = "../Series/Item.aspx?" & CommonShared.AppendSecurityCheck(String.Format("Type=Series&Action={0}", DocumentSeriesAction.FromResolution.ToString()))
            InitializeAjax()
            If Not Page.IsPostBack Then
                DocumentSeries.Text = ProtocolEnv.DocumentSeriesName & ":"
                rfvDocumentSeries.Text = "Selezionare un elemento dall'elenco"
                ' informazioni atto
                Name.Text = String.Format("{0}:", Facade.ResolutionFacade.GetResolutionLabel(CurrentResolution))
                Number.Text = Facade.ResolutionFacade.GetResolutionNumber(CurrentResolution)
                ' caricamento 
                ' Contenitori su cui l'operatore ha diritti di inserimento
                Dim availableContainers As IList(Of Data.Container) = Facade.ContainerFacade.GetContainers(DSWEnvironment.DocumentSeries, DocumentSeriesContainerRightPositions.Insert, True)

                If DocSuiteContext.Current.ResolutionEnv.ResolutionKindEnabled Then
                    Dim documentSeriesItems As IList(Of DocumentSeriesItem) = Facade.ResolutionDocumentSeriesItemFacade.GetDocumentSeriesItems(CurrentResolution)
                    availableContainers = availableContainers.Where(Function(f) Not documentSeriesItems.Any(Function(r) r.DocumentSeries.Container.Id = f.Id)).ToList()
                End If

                ddlDocumentSeries.DataValueField = "Id"
                ddlDocumentSeries.DataTextField = "Name"
                ddlDocumentSeries.DataSource = availableContainers
                ddlDocumentSeries.DataBind()
                ' Rendo necessaria la selezione di un atto
                ddlDocumentSeries.Items.Insert(0, New ListItem("Scegliere un archivio...", ""))
                ddlDocumentSeries.SelectedIndex = 0
                ddlDocumentSeries.SelectedValue = SelectedDocumentSeriesId

                BindDocuments()
            End If
        End Sub

        Private Sub ddlDocumentSeries_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlDocumentSeries.SelectedIndexChanged
            SelectedDocumentSeriesId = ddlDocumentSeries.SelectedValue
            BindDocuments()
        End Sub

        Protected Sub DocumentListGrid_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles DocumentListGrid.ItemDataBound
            If e.Item.ItemType <> GridItemType.Item And e.Item.ItemType <> GridItemType.AlternatingItem Then
                Exit Sub
            End If

            Dim item As DocumentInfo = DirectCast(e.Item.DataItem, DocumentInfo)

            With DirectCast(e.Item.FindControl("fileImage"), RadButton)
                .Image.ImageUrl = ImagePath.FromDocumentInfo(item)
                .OnClientClicked = "OpenGenericWindow"
                .CommandArgument = String.Format("../Viewers/DocumentInfoViewer.aspx?{0}", item.ToQueryString().AsEncodedQueryString())
            End With

            ' Carico tutti i tipi di catena documentale disponibili nel contenitore
            With DirectCast(e.Item.FindControl("chainTypes"), DropDownList)
                .Visible = False
                .Items.Clear()
                If ContainerChainTypes IsNot Nothing Then
                    .Visible = True
                    For Each chainType As ChainType In ContainerChainTypes
                        .Items.Add(New ListItem(GetSeriesChainDescription(chainType), chainType.ToString("D")))
                    Next
                End If
            End With

            Dim fileName As Label = DirectCast(e.Item.FindControl("fileName"), Label)
            fileName.Text = item.Name
            Dim fileType As Label = DirectCast(e.Item.FindControl("fileType"), Label)
            fileType.Text = item.Parent.Name

        End Sub

#End Region

#Region " Methods "

        Private Sub InitializeAjax()
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlDocumentSeries, DocumentListGrid, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnConfirm, DocumentListGrid, MasterDocSuite.AjaxDefaultLoadingPanel)
        End Sub

        ''' <summary> caricamento documenti </summary>
        Private Sub BindDocuments()
            DocumentListGrid.DataSource = AllDocuments
            DocumentListGrid.DataBind()
        End Sub

        Public Shared Function GetWithDummyParent(ByVal parentName As String, ByVal documents As IList(Of DocumentInfo)) As List(Of DocumentInfo)
            Dim parent As New FolderInfo(Guid.NewGuid.ToString(), parentName)

            Dim list As New List(Of DocumentInfo)(documents.Count)
            For Each item As DocumentInfo In documents
                item.Parent = parent
                list.Add(item)
            Next
            Return list
        End Function

        Public Function GetSeriesChainDescription(chain As ChainType) As String
            Dim chainDescription As String = String.Empty
            Select Case chain
                Case ChainType.Document
                    If (ProtocolEnv.DocumentSeriesDocumentsLabel.ContainsKey(Model.Entities.DocumentUnits.ChainType.MainChain)) Then
                        chainDescription = ProtocolEnv.DocumentSeriesDocumentsLabel(Model.Entities.DocumentUnits.ChainType.MainChain)
                    End If

                Case ChainType.Annexed
                    If (ProtocolEnv.DocumentSeriesDocumentsLabel.ContainsKey(Model.Entities.DocumentUnits.ChainType.AnnexedChain)) Then
                        chainDescription = ProtocolEnv.DocumentSeriesDocumentsLabel(Model.Entities.DocumentUnits.ChainType.AnnexedChain)
                    End If

                Case ChainType.UnpublishedAnnexed
                    If (ProtocolEnv.DocumentSeriesDocumentsLabel.ContainsKey(Model.Entities.DocumentUnits.ChainType.UnpublishedAnnexedChain)) Then
                        chainDescription = ProtocolEnv.DocumentSeriesDocumentsLabel(Model.Entities.DocumentUnits.ChainType.UnpublishedAnnexedChain)
                    End If

            End Select
            Return chainDescription
        End Function
#End Region

    End Class

End Namespace