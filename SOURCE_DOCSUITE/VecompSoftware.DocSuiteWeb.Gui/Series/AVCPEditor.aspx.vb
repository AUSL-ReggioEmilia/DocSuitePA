Imports System.Collections.Generic
Imports System.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.AVCP
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.DTO.Resolutions
Imports VecompSoftware.DocSuiteWeb.DTO.DocumentSeries
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Resolutions
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.DocSuiteWeb.BusinessRule.Relations

Namespace Series

    Public Class AvcpEditor
        Inherits CommonBasePage

#Region "Fields"
        Private _currentResolutionId As Integer?
        Private _currentDocumentSeriesItemId As Integer?
        Private _currentBandiGaraDocumentSeriesItem As DocumentSeriesItem
        Private _currentDocumentSeriesItemFacade As DocumentSeriesItemFacade
        Private _currentAVCPDocumentSeriesItem As DocumentSeriesItem
        Private _currentSubCategory As Category
        Private _contactFacade As ContactFacade
        Private _previousPage As String
        Private _currentCategory As Category
        Private _currentAVCPFacade As AVCPFacade
        Private _availableContainers As IEnumerable(Of Container)
        Private _resolutionKindDocumentSeries As ResolutionKindDocumentSeriesFacade
        Private _action As DocumentSeriesAction
#End Region

#Region " Properties "

        Public ReadOnly Property CurrentResolutionId As Integer
            Get
                If Not _currentResolutionId.HasValue Then
                    _currentResolutionId = Request.QueryString.GetValueOrDefault(Of Integer)("IdResolution", -1)
                End If
                Return _currentResolutionId
            End Get
        End Property

        Public Property CurrentPublication() As pubblicazione
            Get
                Return DirectCast(ViewState("currentPublication"), pubblicazione)
            End Get
            Set(value As pubblicazione)
                ViewState("currentPublication") = value
            End Set
        End Property

        Public ReadOnly Property CurrentDocumentSeriesItemId As Integer
            Get
                If Not _currentDocumentSeriesItemId.HasValue Then
                    _currentDocumentSeriesItemId = Request.QueryString.GetValueOrDefault(Of Integer)("IdDocumentSeriesItem", -1)
                End If
                Return _currentDocumentSeriesItemId.Value
            End Get
        End Property

        Public ReadOnly Property CurrentDocumentSeriesItemFacade As DocumentSeriesItemFacade
            Get
                If _currentDocumentSeriesItemFacade Is Nothing Then
                    _currentDocumentSeriesItemFacade = New DocumentSeriesItemFacade()
                End If
                Return _currentDocumentSeriesItemFacade
            End Get
        End Property

        Public ReadOnly Property CurrentBandiGaraDocumentSeriesItem As DocumentSeriesItem
            Get
                If _currentBandiGaraDocumentSeriesItem Is Nothing Then
                    _currentBandiGaraDocumentSeriesItem = CurrentDocumentSeriesItemFacade.GetById(CurrentDocumentSeriesItemId)
                End If
                Return _currentBandiGaraDocumentSeriesItem
            End Get
        End Property

        Public ReadOnly Property CurrentAVCPDocumentSeriesItem As DocumentSeriesItem
            Get
                If _currentAVCPDocumentSeriesItem Is Nothing Then
                    If CurrentResolutionId <> -1 Then
                        Dim resolution As Resolution = FacadeFactory.Instance.ResolutionFacade.GetById(CurrentResolutionId)
                        _currentAVCPDocumentSeriesItem = New AVCPFacade().GetAVCPDocumentSeriesItem(resolution)
                    Else
                        If CurrentDocumentSeriesItemId <> -1 Then
                            _currentAVCPDocumentSeriesItem = CurrentDocumentSeriesItemFacade.GetById(CurrentDocumentSeriesItemId)
                        End If
                    End If
                    If _currentAVCPDocumentSeriesItem Is Nothing Then
                        _currentAVCPDocumentSeriesItem = New DocumentSeriesItem()
                    End If
                End If
                Return _currentAVCPDocumentSeriesItem
            End Get
        End Property

        ''' <summary>
        ''' Ente appaltante proposto da interfaccia grafica
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property EnteAppaltante As String
            Get
                Return ProtocolEnv.AVCPEntePubblicatore
            End Get
        End Property

        ''' <summary>
        ''' CF Struttura Appaltante
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property CfStrutturaAppaltante As String
            Get
                Return ProtocolEnv.AVCPCfEnteAppaltante
            End Get
        End Property

        Public ReadOnly Property ContactFacade() As ContactFacade
            Get
                If (_contactFacade Is Nothing) Then
                    _contactFacade = New ContactFacade()
                End If
                Return _contactFacade
            End Get
        End Property

        Private Property CurrentAziendeBandoGara As IList(Of Contact)
            Get
                If (Not ViewState("currentAziendeBandoGara") Is Nothing) Then
                    Return DirectCast(ViewState("currentAziendeBandoGara"), List(Of Contact))
                End If
                Return New List(Of Contact)
            End Get
            Set(value As IList(Of Contact))
                ViewState("currentAziendeBandoGara") = value
            End Set
        End Property

        Private Property CurrentStrutturaProponente As String
            Get
                If (Not ViewState("strutturaProponente") Is Nothing) Then
                    Return DirectCast(ViewState("strutturaProponente"), String)
                End If
                Return String.Empty
            End Get
            Set(value As String)
                ViewState("strutturaProponente") = value
            End Set
        End Property

        Private Enum TypeSeries
            AVCP
            BandiDiGara
        End Enum
        Private ReadOnly Property CurrentTypeSeries As TypeSeries
            Get
                Dim _typeSeries As TypeSeries = Request.QueryString.GetValueOrDefault(Of TypeSeries)("TypeSeries", TypeSeries.AVCP)
                Return _typeSeries
            End Get
        End Property

        Private ReadOnly Property CurrentCategory As Category
            Get
                If _currentCategory Is Nothing Then
                    Dim categoryId As Integer = Request.QueryString.GetValueOrDefault(Of Integer)("Category", -1)
                    _currentCategory = New CategoryFacade().GetById(categoryId)
                End If
                Return _currentCategory
            End Get
        End Property

        Private ReadOnly Property CurrentSubCategory As Category
            Get
                If _currentSubCategory Is Nothing Then
                    Dim subCategoryId As Integer = Request.QueryString.GetValueOrDefault(Of Integer)("SubCategory", -1)
                    _currentSubCategory = New CategoryFacade().GetById(subCategoryId)
                End If
                Return _currentSubCategory
            End Get
        End Property

        Private ReadOnly Property CurrentPreviousPage As String
            Get
                If _previousPage Is Nothing Then
                    _previousPage = Request.QueryString.GetValueOrDefault(Of String)("PreviousPage", String.Empty)
                End If
                Return String.Format("{0}&DocumentSeriesAVCPId={1}", _previousPage, IIf(CurrentAVCPDocumentSeriesItem Is Nothing, -1, CurrentAVCPDocumentSeriesItem.Id))
            End Get
        End Property

        Private ReadOnly Property CurrentAVCPFacade As AVCPFacade
            Get
                If _currentAVCPFacade Is Nothing Then
                    _currentAVCPFacade = New AVCPFacade()
                End If
                Return _currentAVCPFacade
            End Get
        End Property

        Private ReadOnly Property AvailableContainer() As IEnumerable(Of Container)
            Get
                If _availableContainers Is Nothing Then
                    _availableContainers = Facade.ContainerFacade.GetContainers(DSWEnvironment.DocumentSeries, New List(Of Integer)({DocumentSeriesContainerRightPositions.Insert, DocumentSeriesContainerRightPositions.Draft}), True)
                End If
                Return _availableContainers
            End Get
        End Property

        Private Property CurrentNodeLotSelected As String
            Get
                If Not ViewState.Item("CurrentNodeLotSelected") Is Nothing Then
                    Return DirectCast(ViewState.Item("CurrentNodeLotSelected"), String)
                End If
                Return Nothing
            End Get
            Set(value As String)
                ViewState("CurrentNodeLotSelected") = value
            End Set
        End Property

        Public Property DraftSeriesItemAdded As IList(Of ResolutionSeriesDraftInsert)
            Get
                If Session("DraftSeriesItemAdded") IsNot Nothing Then
                    Return DirectCast(Session("DraftSeriesItemAdded"), IList(Of ResolutionSeriesDraftInsert))
                End If
                Return Nothing
            End Get
            Set(value As IList(Of ResolutionSeriesDraftInsert))
                If value Is Nothing Then
                    Session.Remove("DraftSeriesItemAdded")
                Else
                    Session("DraftSeriesItemAdded") = value
                End If
            End Set
        End Property

        Private ReadOnly Property CurrentDocumentSeriesModel As DocumentSeriesInsertModel
            Get
                If Session("CurrentDocumentSeriesModel") IsNot Nothing Then
                    Return DirectCast(Session("CurrentDocumentSeriesModel"), DocumentSeriesInsertModel)
                End If
                Return Nothing
            End Get
        End Property

        Private Property CurrentResolutionModel As ResolutionInsertModel
            Get
                If Session("CurrentResolutionModel") IsNot Nothing Then
                    Return DirectCast(Session("CurrentResolutionModel"), ResolutionInsertModel)
                End If
                Return Nothing
            End Get
            Set(value As ResolutionInsertModel)
                If value Is Nothing Then
                    Session.Remove("CurrentResolutionModel")
                Else
                    Session("CurrentResolutionModel") = value
                End If
            End Set
        End Property

        Protected Overridable ReadOnly Property ReslKindDocumentSeriesFacade As ResolutionKindDocumentSeriesFacade
            Get
                If _resolutionKindDocumentSeries Is Nothing Then
                    _resolutionKindDocumentSeries = New ResolutionKindDocumentSeriesFacade(DocSuiteContext.Current.User.FullUserName)
                End If
                Return _resolutionKindDocumentSeries
            End Get
        End Property

        Private ReadOnly Property Action() As DocumentSeriesAction
            Get
                Dim temp As String = Request.QueryString.GetValueOrDefault(Of String)("Action", DocumentSeriesAction.Insert.ToString())
                _action = CType([Enum].Parse(GetType(DocumentSeriesAction), temp), DocumentSeriesAction)

                If _action = DocumentSeriesAction.FromResolution AndAlso Not DocSuiteContext.Current.IsResolutionEnabled Then
                    Throw New InvalidOperationException("Modulo Resolution non abilitato.")
                End If
                Return _action
            End Get
        End Property

        Private ReadOnly Property IsUpdateOrView() As Boolean
            Get
                If Action.Equals(DocumentSeriesAction.FromResolutionView) OrElse Action.Equals(DocumentSeriesAction.FromResolutionKindUpdate) Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

#End Region

#Region " Events "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            InitializeAjax()
            SetResponseNoCache()
            If Not Page.IsPostBack Then

                lbCFStrutturaAppaltante.Text = CfStrutturaAppaltante
                lbEnteAppaltante.Text = EnteAppaltante
                lblSumAziendeInvitate.Text = "0"
                txtYear.Text = DateTime.Now.Year.ToString()
                publisher.Text = EnteAppaltante

                If CurrentDocumentSeriesModel IsNot Nothing Then
                    txtTitle.Text = CurrentDocumentSeriesModel.Object
                End If

                If CurrentTypeSeries = TypeSeries.AVCP OrElse IsUpdateOrView Then
                    lblTitle.Text = CurrentAVCPDocumentSeriesItem.Subject

                    Try
                        CurrentPublication = CurrentAVCPFacade.GetAVCPStructure(CurrentAVCPDocumentSeriesItem)
                    Catch ex As Exception
                        ' Significa che non esiste nessun documento di AVCP salvato e creo una nuova struttura
                        CurrentPublication = CreateEmptyPubblication()
                    End Try
                End If

                If CurrentTypeSeries = TypeSeries.BandiDiGara AndAlso Not IsUpdateOrView Then
                    panelInsertInAVCP.Visible = True
                    chbInsertAVCP.Checked = True
                    CurrentPublication = CreateEmptyPubblication()
                End If

                CurrentPublication = SetDefaultParameter(CurrentPublication)
                DirectCast(toolBar.FindItemByValue("remove"), RadToolBarButton).ImageUrl = ImagePath.SmallRemove
                PublicationLoad(CurrentPublication)
                LoadAziendePartecipanti()
            End If

            If (CurrentDocumentSeriesModel IsNot Nothing) Then
                lot.DefaultSubject = CurrentDocumentSeriesModel.Object
            End If
            lot.DefaultStructName = EnteAppaltante
        End Sub
        Private Sub btnElencoAziendeInvitate_Click(sender As Object, e As EventArgs) Handles btnElencoAziendeInvitate.Click
            ShowWindows(windowsBandoGara)
        End Sub

        Private Function NodeSelected() As Boolean
            If lotsTree.SelectedNode Is Nothing OrElse String.IsNullOrEmpty(lotsTree.SelectedNode.Category) Then
                lot.Visible = False
                AjaxAlert("Selezionare un nodo valido")
                Return False
            End If
            Return True
        End Function

        Private Sub toolBar_ButtonClick(sender As Object, e As RadToolBarEventArgs) Handles toolBar.ButtonClick

            Select Case e.Item.Value
                Case "save"
                    If Not NodeSelected() Then
                        Return
                    End If
                    Dim publication As pubblicazione = CurrentPublication

                    Dim lotto As pubblicazioneLotto = lot.GetLotto()

                    For index As Integer = 0 To publication.data.Length - 1
                        Dim item As pubblicazioneLotto = publication.data(index)
                        If item.cig.Eq(lotto.cig) AndAlso Not item.cig.Eq(lotsTree.SelectedNode.Value) Then
                            txtMessageWindow.Text = "CIG già presente"
                            Me.ShowWindows(RadWindow1)
                            Exit Sub
                        End If
                    Next

                    If lotsTree.SelectedNode.Category = "lot" Then
                        ' Verifico che il cig CORRENTE non sia già presente a sistema db
                        If ProtocolEnv.AVCPCIGUniqueValidationEnabled Then
                            If CheckCIGExists(lotto.cig) Then
                                txtMessageWindow.Text = "CIG già presente nel sistema"
                                Me.ShowWindows(RadWindow1)
                                Exit Sub
                            End If
                        End If


                        ' Salvo i dati
                        For index As Integer = 0 To publication.data.Length - 1
                                Dim item As pubblicazioneLotto = publication.data(index)
                                If item.cig.Eq(lotsTree.SelectedNode.Value) Then
                                    publication.data(index) = lotto
                                    CurrentPublication = publication
                                    Exit For
                                End If
                            Next
                        End If

                        PublicationLoad(publication)
                    Exit Sub

                Case "add"
                    If lotsTree.Nodes.Count = 0 Then
                        ' Se non nessun nodo (lotto) allora ne creo uno vuoto.
                        CurrentPublication = CreateEmptyPubblication()
                    End If

                    Dim newData As List(Of pubblicazioneLotto) = CurrentPublication.data.ToList()
                    newData.Add(CreateEmptyLotto())

                    Dim publication As pubblicazione = CurrentPublication
                    publication.data = newData.ToArray()
                    CurrentPublication = publication
                    PublicationLoad(publication)
                    lotsTree.Nodes(lotsTree.Nodes.Count - 1).Selected = True
                    Dim evtargs As New RadTreeNodeEventArgs(lotsTree.Nodes(lotsTree.Nodes.Count - 1))
                    lotsTree_NodeClick(Nothing, evtargs)
                    Exit Sub
                Case "remove"
                    If Not NodeSelected() Then
                        Return
                    End If
                    Dim publication As pubblicazione = CurrentPublication
                    If lotsTree.SelectedNode.Category = "lot" Then
                        Dim inx As Integer = lotsTree.SelectedNode.Index
                        Dim toKeep As List(Of pubblicazioneLotto) = CurrentPublication.data.ToList()
                        toKeep.RemoveAt(inx)
                        publication.data = toKeep.ToArray()
                        CurrentPublication = publication
                    End If
                    lotsTree.Nodes.Remove(lotsTree.SelectedNode)

                    PublicationLoad(publication)
                    Exit Sub
                Case Else
                    AjaxAlert("Caso non previsto")
            End Select

        End Sub

        Private Sub lotsTree_NodeClick(sender As Object, e As RadTreeNodeEventArgs) Handles lotsTree.NodeClick
            If lotsTree.SelectedNode Is Nothing OrElse String.IsNullOrEmpty(e.Node.Category) Then
                lot.Visible = False
                Exit Sub
            End If

            Select Case e.Node.Category
                Case "lot"
                    ' memorizzo l'ultimo lotto selezionato
                    CurrentNodeLotSelected = e.Node.Value
                    ' carico lo user control per la modifica dei dati AVCP
                    LoadLot(e.Node.Value)
            End Select

        End Sub

        Private Sub LoadLot(ByVal cig As String)

            For Each item As pubblicazioneLotto In CurrentPublication.data
                If item.cig.Eq(cig) Then
                    lot.Visible = True
                    lot.SetLotto(item)
                    lot.SetAziendeBando(CurrentAziendeBandoGara)
                    Exit Sub
                End If
            Next

        End Sub

        Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
            Dim publication As pubblicazione = CurrentPublication
            publication.metadata = New pubblicazioneMetadata

            Dim year As Integer
            If Integer.TryParse(txtYear.Text, year) Then
                publication.metadata.annoRiferimento = year
            End If
            publication.metadata.titolo = txtTitle.Text
            publication.metadata.abstract = txtAbstract.Text
            If publishingDate.SelectedDate.HasValue Then
                publication.metadata.dataPubbicazioneDataset = publishingDate.SelectedDate.Value
            End If
            publication.metadata.entePubblicatore = publisher.Text
            If lastUpdateDate.SelectedDate.HasValue Then
                publication.metadata.dataUltimoAggiornamentoDataset = lastUpdateDate.SelectedDate.Value
            End If
            publication.metadata.urlFile = url.Text
            If Not String.IsNullOrEmpty(licence.Text) Then
                publication.metadata.licenza = licence.Text
            End If

            If CurrentTypeSeries = TypeSeries.AVCP OrElse IsUpdateOrView Then
                Dim resolution As Resolution = FacadeFactory.Instance.ResolutionFacade.GetById(CurrentResolutionId)
                Dim AVCPDocumenSeriesItem As DocumentSeriesItem = CurrentAVCPDocumentSeriesItem
                CurrentAVCPFacade.SetDataSetPub(publication, AVCPDocumenSeriesItem, DocSuiteContext.Current.User.FullUserName, True)
                ' aggiornamento campi dinamici serie bandi di gara
                UpdateBandiDiGaraSeries(publication, AVCPDocumenSeriesItem)
                If IsUpdateOrView Then
                    Session("CurrentAVCP") = CurrentPublication
                End If
            End If

            If CurrentTypeSeries = TypeSeries.BandiDiGara AndAlso Not IsUpdateOrView Then
                ' Inserisci una NUOVA serie documentale di AVCP 
                If chbInsertAVCP.Checked = True Then
                    CreateNewDocumentSeriesItemAVCP(publication)
                End If
                ' Redirect alla pagina di inserimento della bozza passando i campi precompilati.
                Session("CurrentAVCP") = CurrentPublication
            End If
            Response.Redirect(CurrentPreviousPage, False)
        End Sub

        ''' <summary>
        ''' Rimpiazzo tutte le strutture proponente, se vuote, con il codice fiscale presente nella parameterEnv e con la denominazione del settore di appartenenza.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Protected Sub uscProponente_RolesAdded(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs) Handles uscProponente.RolesAdded
            If (uscProponente.RoleListAdded.Count = 1) Then
                UpdateRole()
            End If
        End Sub

        Protected Sub uscProponente_RolesRemoved(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs) Handles uscProponente.RoleRemoved
            Dim _oldStrutturaProponente As String = CurrentStrutturaProponente
            CurrentStrutturaProponente = String.Empty
            Dim newLotto As List(Of pubblicazioneLotto) = New List(Of pubblicazioneLotto)
            For Each lotto As pubblicazioneLotto In CurrentPublication.data
                If lotto.strutturaProponente.denominazione.Eq(_oldStrutturaProponente) Then
                    lotto.strutturaProponente = New pubblicazioneLottoStrutturaProponente With {.denominazione = CurrentStrutturaProponente}
                    newLotto.Add(lotto)
                End If
            Next
            CurrentPublication.data = newLotto.ToArray()
            ' ricarico i dati della pubblicazione corrente
            PublicationLoad(CurrentPublication)
        End Sub

        Private Sub uscAziendeBando_CompleteSelection(sender As Object, e As ContactEventArgs) Handles uscAziendeBando.CompleteSelectionAziende
            CurrentAziendeBandoGara = e.ContactTarget
            lblSumAziendeInvitate.Text = CurrentAziendeBandoGara.Count
            HideWindows(windowsBandoGara)
        End Sub

        Private Sub uscAziendeBando_NeedFinder(sender As Object, e As ContactEventArgs) Handles uscAziendeBando.NeedFinder

            Dim contattiRubrica As List(Of Contact) = ContactFacade.GetLikeDescription(e.Description, ContactType.Aoo, ProtocolEnv.AVCPIdBusinessContact).ToList()
            Dim contattiTarget As List(Of Contact) = uscAziendeBando.GetAziendeTarget()
            contattiRubrica.AddRange(contattiTarget.Where(Function(f) Not contattiRubrica.Any(Function(c) c.Id = f.Id)).ToList())
            uscAziendeBando.ForceLoadingSource(contattiRubrica, contattiTarget)
        End Sub

#End Region

#Region " Methods "

        Private Sub ShowWindows(window As RadWindow)
            Dim script As String = String.Concat("function f(){$find(""", window.ClientID, """).show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);")
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, True)
        End Sub

        Private Sub HideWindows(window As RadWindow)
            Dim script As String = String.Concat("function f(){$find(""", window.ClientID, """).close(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);")
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, True)
        End Sub

        Private Sub InitializeAjax()
            AjaxManager.AjaxSettings.AddAjaxSetting(lotsTree, panelAVCPLotto, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(uscProponente, panelIntestazione, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnElencoAziendeInvitate, windowsBandoGara, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(uscProponente, uscProponente)
            AjaxManager.AjaxSettings.AddAjaxSetting(uscProponente, panelAVCPLotto, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnSave, RadWindow1, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnSave, btnSave, MasterDocSuite.AjaxFlatLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnSave, panelAVCPLotto, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscAziendeBando, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(uscAziendeBando, uscAziendeBando, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(uscAziendeBando, panelIntestazione, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(lot, panelIntestazione, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(toolBar, panelAVCPLotto, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(toolBar, RadWindow1, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(lot, panelAVCPLotto)
        End Sub

        Private Sub PublicationLoad(ByVal publication As pubblicazione)
            If (publication Is Nothing) Then
                Return
            End If
            If (publication.metadata IsNot Nothing) Then

                txtAbstract.Text = publication.metadata.abstract

                If Not String.IsNullOrEmpty(publication.metadata.entePubblicatore) Then
                    publisher.Text = publication.metadata.entePubblicatore
                End If

                If Not String.IsNullOrEmpty(publication.metadata.titolo) Then
                    txtTitle.Text = publication.metadata.titolo
                End If

                If publication.metadata.annoRiferimento > 0 Then
                    txtYear.Text = publication.metadata.annoRiferimento.ToString()
                End If

                If publication.metadata.dataPubbicazioneDataset <> DateTime.MinValue Then
                    publishingDate.SelectedDate = publication.metadata.dataPubbicazioneDataset
                End If

                If publication.metadata.dataUltimoAggiornamentoDataset <> DateTime.MinValue Then
                    lastUpdateDate.SelectedDate = publication.metadata.dataUltimoAggiornamentoDataset
                End If
                url.Text = publication.metadata.urlFile
                If publication.metadata.licenza IsNot Nothing Then
                    licence.Text = publication.metadata.licenza.ToString()
                End If

            End If

            Dim lastIndex As Integer = -1
            If lotsTree.SelectedNode IsNot Nothing Then
                lastIndex = lotsTree.SelectedNode.Index
            End If

            lotsTree.Nodes.Clear()
            If publication.data IsNot Nothing Then
                For index As Integer = 0 To publication.data.Length - 1
                    lotsTree.Nodes.Add(CreateLotNode(publication.data(index)))
                Next
            End If
            lotsTree.ExpandAllNodes()
            lot.Visible = False
            If lotsTree.Nodes.Count > 0 AndAlso lastIndex >= 0 Then
                lotsTree.Nodes(lastIndex).Selected = True
                Dim evtargs As New RadTreeNodeEventArgs(lotsTree.Nodes(lastIndex))
                lotsTree_NodeClick(Nothing, evtargs)
            End If
        End Sub

        Private Function CreateLotNode(ByVal publicationLot As pubblicazioneLotto) As RadTreeNode
            Dim node As New RadTreeNode
            node.Text = String.Format("Lotto [{0}]", publicationLot.cig)
            node.ToolTip = "Codice Identificativo Gara"
            node.Category = "lot"
            node.Value = publicationLot.cig
            node.Nodes.Add(New RadTreeNode("Importo aggiudicazione: " & publicationLot.importoAggiudicazione.ToString()))
            node.Nodes.Add(New RadTreeNode("Importo somme liquidate: " & publicationLot.importoSommeLiquidate.ToString()))
            node.Nodes.Add(New RadTreeNode("Oggetto: " & publicationLot.oggetto))
            node.Nodes.Add(New RadTreeNode("Scelta contraente: " & publicationLot.sceltaContraente.GetXmlName()))
            If publicationLot.strutturaProponente IsNot Nothing Then
                node.Nodes.Add(New RadTreeNode("Codice fiscale Proponente: " & publicationLot.strutturaProponente.codiceFiscaleProp))
                node.Nodes.Add(New RadTreeNode("Denominazione Proponente: " & publicationLot.strutturaProponente.denominazione))
            End If
            If publicationLot.tempiCompletamento IsNot Nothing Then
                node.Nodes.Add(New RadTreeNode("Data Inizio: " & publicationLot.tempiCompletamento.dataInizio.ToShortDateString()))
                node.Nodes.Add(New RadTreeNode("Data Ultimazione: " & publicationLot.tempiCompletamento.dataUltimazione.ToShortDateString()))
            End If
            If publicationLot.partecipanti IsNot Nothing Then
                node.Nodes.Add(CreatePartecipantsNode(publicationLot.partecipanti))
            End If
            If publicationLot.aggiudicatari IsNot Nothing Then
                node.Nodes.Add(CreateContractorsNode(publicationLot.aggiudicatari))
            End If
            Return node
        End Function

        Private Function CreatePartecipantsNode(partecipants As pubblicazioneLottoPartecipanti) As RadTreeNode
            Dim node As New RadTreeNode
            node.Text = "Partecipanti"
            If partecipants.partecipante IsNot Nothing Then
                For index As Integer = 0 To partecipants.partecipante.Length - 1
                    node.Nodes.Add(CreateSingoloTypeNode(partecipants.partecipante(index)))
                Next
            End If
            If partecipants.raggruppamento IsNot Nothing Then
                Dim nodeRaggruppamento As New RadTreeNode
                nodeRaggruppamento.Text = "Raggruppamento"
                For index As Integer = 0 To partecipants.raggruppamento.Length - 1
                    nodeRaggruppamento.Nodes.AddRange(CreatePartecipantsRaggruppamentoNode(partecipants.raggruppamento(index)))
                Next
                node.Nodes.Add(nodeRaggruppamento)
            End If
            Return node
        End Function

        Private Function CreateSingoloTypeNode(singoloType As singoloType) As RadTreeNode
            Dim node As New RadTreeNode
            node.Text = singoloType.ragioneSociale
            If Not String.IsNullOrEmpty(singoloType.Item) Then
                node.Nodes.Add(New RadTreeNode(If(singoloType.ItemElementName = ItemChoiceType1.codiceFiscale, "Codice Fiscale", "Identificativo Fiscale Estero") & ": " & singoloType.Item))
            End If
            Return node
        End Function

        Private Function CreatePartecipantsRaggruppamentoNode(raggruppamento As pubblicazioneLottoPartecipantiRaggruppamento) As IList(Of RadTreeNode)
            Dim raggruppamentoNodes As New List(Of RadTreeNode)(raggruppamento.membro.Length)
            For index As Integer = 0 To raggruppamento.membro.Length - 1
                Dim node As New RadTreeNode
                node.Text = raggruppamento.membro(index).ragioneSociale
                node.Nodes.Add(New RadTreeNode(If(raggruppamento.membro(index).ItemElementName = ItemChoiceType.codiceFiscale, "Codice Fiscale", "Identificativo Fiscale Estero") & ": " & raggruppamento.membro(index).Item))
                node.Nodes.Add(New RadTreeNode(raggruppamento.membro(index).ruolo.GetXmlName()))
                raggruppamentoNodes.Add(node)
            Next
            Return raggruppamentoNodes
        End Function

        Private Function CreateContractorsNode(contractors As pubblicazioneLottoAggiudicatari) As RadTreeNode
            Dim node As New RadTreeNode
            node.Text = "Aggiudicatari"
            node.Category = "contractors"
            If contractors.aggiudicatario IsNot Nothing Then
                For index As Integer = 0 To contractors.aggiudicatario.Length - 1
                    node.Nodes.Add(CreateSingoloTypeNode(contractors.aggiudicatario(index)))
                Next
            End If
            If contractors.aggiudicatarioRaggruppamento IsNot Nothing Then
                Dim nodeRaggruppamento As New RadTreeNode
                nodeRaggruppamento.Text = "Raggruppamento"
                For index As Integer = 0 To contractors.aggiudicatarioRaggruppamento.Length - 1
                    nodeRaggruppamento.Nodes.AddRange(CreateContractorsRaggruppamentoNode(contractors.aggiudicatarioRaggruppamento(index)))
                Next
                node.Nodes.Add(nodeRaggruppamento)
            End If
            Return node
        End Function

        Private Function CreateContractorsRaggruppamentoNode(raggruppamento As pubblicazioneLottoAggiudicatariAggiudicatarioRaggruppamento) As IList(Of RadTreeNode)
            Dim raggruppamentoNodes As New List(Of RadTreeNode)(raggruppamento.membro.Length)
            For index As Integer = 0 To raggruppamento.membro.Length - 1
                Dim node As New RadTreeNode
                node.Text = raggruppamento.membro(index).ragioneSociale
                node.Nodes.Add(New RadTreeNode(If(raggruppamento.membro(index).ItemElementName = ItemChoiceType.codiceFiscale, "Codice Fiscale", "Identificativo Fiscale Estero") & ": " & raggruppamento.membro(index).Item))
                node.Nodes.Add(New RadTreeNode(raggruppamento.membro(index).ruolo.GetXmlName()))
                raggruppamentoNodes.Add(node)
            Next
            Return raggruppamentoNodes
        End Function

        Private Function CreateSimplyPublication() As pubblicazione
            ' tenere per test
            Return New pubblicazione With {
                .metadata = New pubblicazioneMetadata With {
                    .dataPubbicazioneDataset = DateTime.Now,
                    .dataUltimoAggiornamentoDataset = DateTime.Now,
                    .licenza = ""
                    },
                .data = New pubblicazioneLotto() {
                    New pubblicazioneLotto With {
                        .strutturaProponente = New pubblicazioneLottoStrutturaProponente(),
                        .tempiCompletamento = New pubblicazioneLottoTempiCompletamento With {
                            .dataInizio = DateTime.Now.Date,
                            .dataUltimazione = DateTime.Now.AddYears(1).Date
                            },
                        .sceltaContraente = sceltaContraenteType.Item01PROCEDURAAPERTA
                        }
                    }
                }
        End Function

        Private Sub SetProponente()
            Dim newLotto As List(Of pubblicazioneLotto) = New List(Of pubblicazioneLotto)
            For Each lotto As pubblicazioneLotto In CurrentPublication.data
                If (lotto.strutturaProponente.codiceFiscaleProp = String.Empty AndAlso lotto.strutturaProponente.denominazione = String.Empty) Then
                    lotto.strutturaProponente = New pubblicazioneLottoStrutturaProponente With
                                                {.denominazione = CurrentStrutturaProponente,
                                                    .codiceFiscaleProp = CfStrutturaAppaltante}
                    newLotto.Add(lotto)
                End If
            Next
            CurrentPublication.data = newLotto.ToArray()
            ' ricarico i dati della pubblicazione corrente
            PublicationLoad(CurrentPublication)
        End Sub

        Private Sub UpdateRole(Optional ByVal selectedSecton As Role = Nothing)
            Dim strutturaProponente As Role
            If Not selectedSecton Is Nothing Then
                strutturaProponente = selectedSecton
            Else
                strutturaProponente = Facade.RoleFacade.GetById(uscProponente.RoleListAdded(0))
            End If
            CurrentStrutturaProponente = strutturaProponente.Name
            Dim newLotto As List(Of pubblicazioneLotto) = New List(Of pubblicazioneLotto)
            For Each lotto As pubblicazioneLotto In CurrentPublication.data
                lotto.strutturaProponente = New pubblicazioneLottoStrutturaProponente With
                                            {.denominazione = CurrentStrutturaProponente, .codiceFiscaleProp = CfStrutturaAppaltante}
                newLotto.Add(lotto)
            Next
            CurrentPublication.data = newLotto.ToArray()
            ' ricarico i dati della pubblicazione corrente
            PublicationLoad(CurrentPublication)
            ' Ricarico le informazioni dell'ultimo lotto selezionato
            If Not CurrentNodeLotSelected Is Nothing Then
                LoadLot(CurrentNodeLotSelected)
            End If
        End Sub

        ''' <summary>
        ''' Setto i parametri di default da impostare su interfaccia grafica o sull'XML AVCP all'avvio
        ''' </summary>
        ''' <remarks></remarks>

        Private Function SetDefaultParameter(ByVal publication As pubblicazione) As pubblicazione
            ' Imposto il titolo se non è presente
            If (publication IsNot Nothing) Then
                If String.IsNullOrEmpty(publication.metadata.titolo) Then
                    If (CurrentDocumentSeriesModel IsNot Nothing) Then
                        publication.metadata.titolo = CurrentDocumentSeriesModel.Object
                    End If
                Else
                    lot.DefaultSubject = publication.metadata.titolo
                End If
                ' Imposto il nome dell'ente appaltante se non presente nell'XML di AVCP
                If String.IsNullOrEmpty(publication.metadata.entePubblicatore) Then
                    publication.metadata.entePubblicatore = EnteAppaltante
                End If
            End If

            ' Se provengo dalla pagina di bandi di gara, associo i "Settori di appartenenza" al "Proponente"
            If Not CurrentDocumentSeriesModel Is Nothing _
                AndAlso Not CurrentDocumentSeriesModel.SectorMembershipAuthorizations Is Nothing _
                AndAlso uscProponente.SourceRoles Is Nothing Then

                uscProponente.SourceRoles = CurrentDocumentSeriesModel.SectorMembershipAuthorizations
                UpdateRole(CurrentDocumentSeriesModel.SectorMembershipAuthorizations)
            End If
            Return publication
        End Function

        ''' <summary> 
        ''' Carica le aziende partecipanti all'intero BANDO
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub LoadAziendePartecipanti()
            If (ProtocolEnv.AVCPIdBusinessContact > 0) Then

                ' caricamento aziende TARGET dall'oggetto pubblicazione (tutti i lotti). Ricercare le aziende per CF tra i partecipanti e gli aggiudicatari
                Dim contattiTarget As List(Of String) = New List(Of String)
                If (CurrentPublication IsNot Nothing AndAlso CurrentPublication.data IsNot Nothing) Then
                    contattiTarget.AddRange(CurrentPublication.data.Where(Function(f) f.partecipanti IsNot Nothing AndAlso f.partecipanti.partecipante IsNot Nothing).SelectMany(Function(p) p.partecipanti.partecipante.Select(Function(t) t.Item)).ToList())
                End If

                'caricare contact
                Dim contattiRubrica As ICollection(Of Contact) = ContactFacade.GetByFiscalCodes(contattiTarget, ContactType.Aoo, ProtocolEnv.AVCPIdBusinessContact)
                ' Carico i dati all'avvio 
                uscAziendeBando.ForceLoadingSource(contattiRubrica, contattiRubrica)
                ' Reperisco le aziende target già selezionate
                CurrentAziendeBandoGara = uscAziendeBando.GetAziendeTarget()
                lblSumAziendeInvitate.Text = CurrentAziendeBandoGara.Count().ToString()
            End If
        End Sub

        ''' <summary>
        ''' Verifica se il CIG esiste nel DB
        ''' </summary>
        ''' <param name="cig"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CheckCIGExists(ByVal cig As String) As Boolean
            Return CurrentAVCPFacade.CheckCIGExists(cig)
        End Function

        ''' <summary>
        ''' Crea una pubblicazione vuota
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CreateEmptyPubblication() As pubblicazione
            Return New pubblicazione() With {.data = New List(Of pubblicazioneLotto)().ToArray(), .metadata = New pubblicazioneMetadata()}
        End Function

        ''' <summary>
        ''' Crea lotto vuota
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CreateEmptyLotto() As pubblicazioneLotto
            Dim data As pubblicazioneLotto = New pubblicazioneLotto With {.cig = String.Empty,
                                                                                 .importoAggiudicazione = New Decimal(),
                                                                                 .importoSommeLiquidate = New Decimal(),
                                                                                 .oggetto = String.Empty,
                                                                                 .sceltaContraente = sceltaContraenteType.Item01PROCEDURAAPERTA,
                                                                                 .strutturaProponente = New pubblicazioneLottoStrutturaProponente() With {
                                                                                       .denominazione = IIf(CurrentStrutturaProponente.Eq(String.Empty), Nothing, CurrentStrutturaProponente),
                                                                                       .codiceFiscaleProp = CfStrutturaAppaltante},
                                                                                 .tempiCompletamento = New pubblicazioneLottoTempiCompletamento With {
                                                                                       .dataInizio = DateTime.Now.Date,
                                                                                       .dataUltimazione = DateTime.Now.AddYears(1).Date}
                                                                                }
            Return data
        End Function

        Private Sub CreateNewDocumentSeriesItemAVCP(publication As pubblicazione)
            Dim CurrentDocumentSeriesFacade As DocumentSeriesItemFacade = New DocumentSeriesItemFacade()
            Dim status As DocumentSeriesItemStatus = DocumentSeriesItemStatus.Draft
            Dim annoRiferimento As Integer = 0
            If publishingDate.DateInput.SelectedDate.HasValue Then
                annoRiferimento = publishingDate.DateInput.SelectedDate.Value.Year
            End If
            Dim chain As New BiblosChainInfo()

            CurrentAVCPDocumentSeriesItem.Category = CurrentCategory
            CurrentAVCPDocumentSeriesItem.SubCategory = CurrentSubCategory
            CurrentAVCPDocumentSeriesItem.DocumentSeries = Facade.DocumentSeriesFacade.GetById(DocSuiteContext.Current.ProtocolEnv.AvcpDocumentSeriesId.Value)
            If (CurrentDocumentSeriesModel IsNot Nothing) Then
                CurrentAVCPDocumentSeriesItem.Subject = CurrentDocumentSeriesModel.Object
            End If

            ' salvo serie documentale e compilo i dati dinamici
            Dim AVCPDocumenSeriesItem As DocumentSeriesItem = CurrentDocumentSeriesFacade.SaveDocumentSeriesItem(CurrentAVCPDocumentSeriesItem, annoRiferimento, chain, Nothing, Nothing, status, String.Empty)
            ' setto il file XML di pubblicazione
            CurrentAVCPFacade.SetDataSetPub(publication, AVCPDocumenSeriesItem, DocSuiteContext.Current.User.FullUserName, True)
            ' Memorizzo la serie documentale in sessione. Viene utilizzata per collegare la serie di bandi di gara ad AVCP nella pagina di Item.
            If DraftSeriesItemAdded Is Nothing Then
                DraftSeriesItemAdded = New List(Of ResolutionSeriesDraftInsert)
            End If

            Dim IdResolutionKindDocumentSeries As Guid? = Nothing
            If CurrentResolutionModel.ResolutionKind.HasValue Then
                IdResolutionKindDocumentSeries = ReslKindDocumentSeriesFacade.GetResolutionAndSeriesByReslAndSeries(CurrentResolutionModel.ResolutionKind.Value, AVCPDocumenSeriesItem.DocumentSeries.Id).Id
            End If
            DraftSeriesItemAdded.Add(New ResolutionSeriesDraftInsert() With {
                .IdSeries = AVCPDocumenSeriesItem.DocumentSeries.Id,
                .IdSeriesItem = AVCPDocumenSeriesItem.Id})
        End Sub

        Private Sub UpdateBandiDiGaraSeries(publication As pubblicazione, documentSeriesItem As DocumentSeriesItem)
            ' Cerco l'atto associato alla serie documentale di AVCP
            If (publication Is Nothing OrElse documentSeriesItem Is Nothing OrElse Not DocSuiteContext.Current.ProtocolEnv.BandiGaraDocumentSeriesId.HasValue) Then
                Return
                FileLogger.Debug(LogName.FileLog, String.Format("UpdateBandiDiGaraSeries {0} - {1} : non aggiornata", publication, If(documentSeriesItem IsNot Nothing, documentSeriesItem.Id, -1)))
            End If

            Dim resolution As Resolution = Facade.ResolutionDocumentSeriesItemFacade.GetResolutions(documentSeriesItem).FirstOrDefault()
            If resolution IsNot Nothing Then
                Dim docSeriesLikedToResl As ICollection(Of DocumentSeriesItem) = Facade.ResolutionFacade.GetSeriesToAVCP(resolution)
                Dim bandiGaraDocumentSeriesItem As DocumentSeriesItem = docSeriesLikedToResl _
                        .Where(Function(x) x.DocumentSeries.Id = DocSuiteContext.Current.ProtocolEnv.BandiGaraDocumentSeriesId.Value).FirstOrDefault()
                If (bandiGaraDocumentSeriesItem Is Nothing) Then
                    FileLogger.Debug(LogName.FileLog, String.Format("UpdateBandiDiGaraSeries {0} - {1} : bandiGaraDocumentSeriesItem {2} non aggiornata", publication,
                                                                    If(documentSeriesItem IsNot Nothing, documentSeriesItem.Id, -1),
                                                                    String.Join(",", docSeriesLikedToResl.Select(Function(f) documentSeriesItem.Id).ToArray())))
                    Return
                End If

                ' Verifico la relation tra due serie documentali
                Dim dsir As DocumentSeriesItemRelations = New DocumentSeriesItemRelations(documentSeriesItem, bandiGaraDocumentSeriesItem)
                ' Se ho una serie documentale di bandi di gara, collegata ad AVCP, tramite l'atto; eseguo l'aggiornamento dei metadati.
                If dsir.CanUpdateMetadata() Then
                    Dim bandiGaraArchiveInfo As ArchiveInfo = DocumentSeriesFacade.GetArchiveInfo(bandiGaraDocumentSeriesItem.DocumentSeries)
                    Dim chain As BiblosChainInfo = Facade.DocumentSeriesItemFacade.GetMainChainInfo(bandiGaraDocumentSeriesItem)
                    ' update campi dinamici DocumentSeriesItem
                    chain = CurrentAVCPFacade.UpdateAttributeBandiDiGara(bandiGaraDocumentSeriesItem, publication, bandiGaraArchiveInfo, chain)
                    ' salvo il DocumentSeriesItem e aggiorno gli attributi della serie documentale
                    Facade.DocumentSeriesItemFacade.UpdateDocumentSeriesItem(bandiGaraDocumentSeriesItem, chain, $"Aggiornamento metadati serie bandi di gara e contratti {bandiGaraDocumentSeriesItem.Year:0000}/{bandiGaraDocumentSeriesItem.Number:0000000} da pubblicazione AVCP")
                    If bandiGaraDocumentSeriesItem.Status = DocumentSeriesItemStatus.Active Then
                        Facade.DocumentSeriesItemFacade.SendUpdateDocumentSeriesItemCommand(bandiGaraDocumentSeriesItem)
                    End If
                End If
            End If
        End Sub
#End Region
    End Class
End Namespace