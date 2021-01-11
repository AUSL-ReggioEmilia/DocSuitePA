Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Entity.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.OData
Imports VecompSoftware.DocSuiteWeb.Facade.Common.WebAPI
Imports VecompSoftware.DocSuiteWeb.Model.DocumentGenerator
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.Compress
Imports VecompSoftware.Helpers.EInvoice.EInvoice1_2
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.PDF
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers.XML.Converters.Factory
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging
Imports WebAPICommons = VecompSoftware.DocSuiteWeb.Entity.Commons

Namespace Viewers

    Public Class ViewerLight
        Inherits DocSuite2008BaseControl

#Region " Fields "
        Private _counter As Integer = 0
        Public Const HIDE_ACTIVEX_SCRIPT As String = "HidePDFActivex();"
        Public Const SHOW_ACTIVEX_SCRIPT As String = "ShowPDFActivex();"
        Private Const DEFAULT_OPEN_WINDOW_SCRIPT As String = "return {0}_OpenWindow('{1}', '{2}' ,'{3}', '{4}');"

        'Identifica i nodi di tipo Gruppo
        Public Const NODE_GROUP_IDENTIFIER As String = "GROUP"
        'Identifica i nodi di tipo Document
        Public Const NODE_DOCUMENT_IDENTIFIER As String = "DOCUMENT"
        'Identifica i nodi di tipo Folder
        Public Const NODE_FOLDER_IDENTIFIER As String = "FOLDER"

        Public Const BIBLOS_ATTRIBUTE_UniqueId As String = "UniqueId"
        Public Const BIBLOS_ATTRIBUTE_Year As String = "Year"
        Public Const BIBLOS_ATTRIBUTE_Number As String = "Number"
        Public Const BIBLOS_ATTRIBUTE_Environment As String = "Environment"
        Public Const BIBLOS_ATTRIBUTE_IsPublic As String = "IsPublic"
        Public Const BIBLOS_ATTRIBUTE_Miscellanea As String = "Miscellanea"
        Public Const BIBLOS_ATTRIBUTE_UserVisibilityAuthorized As String = "UserVisibilityAuthorized"
        Public Const BIBLOS_ATTRIBUTE_InvoiceKind As String = "InvoiceKind"
        Public Const BIBLOS_ATTRIBUTE_IsInvoice As String = "IsInvoice"
        Public Const BIBLOS_ATTRIBUTE_Disabled As String = "Disabled"

        Public Const BIBLOS_ATTRIBUTE_BiblosChainId As String = "BiblosChainId"
        Public Const BIBLOS_ATTRIBUTE_BiblosDocumentName As String = "BiblosDocumentName"
        Public Const BIBLOS_ATTRIBUTE_BiblosDocumentId As String = "BiblosDocumentId"


        Private _prefixFileName As String
        Private _collapseOnSingleDocument As Boolean?
        Private _leftPaneStartWidth As Integer?
        Private _viewOriginal As Boolean
        Private _treeViewKeys As ICollection(Of String)
        Private _currentODataFacade As New ODataFacade()
        Private _currentXMLFactory As XmlFactory

        Private ReadOnly _invoiceStyleIcon As IDictionary(Of String, String) = New Dictionary(Of String, String) From
        {
            {InvoiceStylesheetNames.ASSO_SOFTWARE, "~/App_Themes/DocSuite2008/imgset16/assosoftware_favicon.ico"},
            {InvoiceStylesheetNames.AGID, "~/App_Themes/DocSuite2008/imgset16/agid_favicon.ico"}
        }
#End Region

#Region " Properties "
        Public Property AlwaysDocumentTreeOpen As Boolean = False

        Public Property PrefixFileName As String
            Get
                Return If(Not ProtocolEnv.EnableViewerLightFileRename OrElse String.IsNullOrEmpty(_prefixFileName), "Documents", _prefixFileName)
            End Get
            Set(value As String)
                _prefixFileName = value
            End Set
        End Property

        Public ReadOnly Property FileName As String
            Get
                Return String.Concat(PrefixFileName, ".zip")
            End Get
        End Property

        Public ReadOnly Property Initialized As Boolean
            Get
                Return rtvListDocument.Nodes.Count > 0
            End Get
        End Property

        Public Property EnabledCheckIsSignedButton As Boolean

        ''' <summary>
        ''' Insieme strutturato dei nodi da visualizzare.
        ''' </summary>
        ''' <value>Lista di gruppi di documenti. Tipicamente un gruppo per ogni elemento da visualizzare.</value>
        ''' <returns>Lista di gruppi di documenti.</returns>
        Public Property DataSource As IList(Of DocumentInfo)

        ''' <summary>
        ''' Pagina da utilizzare per recuperare i documenti.
        ''' </summary>
        ''' <value>Nome, senza estensione, dell'handler che gestisce il recupero dei documenti.</value>
        ''' <returns>Nome dell'hadler</returns>
        ''' <remarks>Se non valorizzato assume il valore di default "DocumentSource"</remarks>
        Public Property DocumentSourcePage As String
            Get
                If ViewState("DocumentSourcePage") Is Nothing Then
                    ViewState("DocumentSourcePage") = "DocumentInfoHandler" ' Pagina di default
                End If
                Return ViewState("DocumentSourcePage").ToString()
            End Get
            Set(value As String)
                ViewState("DocumentSourcePage") = value
            End Set
        End Property

        Public Property DefaultDocument As String

        ''' <summary> Quando attivo visualizza una Check-box vicino ad ogni nodo </summary>
        Public Property CheckBoxes As Boolean
            Get
                If ViewState("checkBoxes") Is Nothing Then
                    Return rtvListDocument.CheckBoxes
                End If
                Return CType(ViewState("checkBoxes"), Boolean)
            End Get
            Set(value As Boolean)
                ViewState("checkBoxes") = value

                If Not MultiViewEnable.HasValue OrElse Not MultiViewEnable.Value Then
                    rtvListDocument.CheckBoxes = value
                End If
            End Set
        End Property

        Public ReadOnly Property CheckedDocuments As List(Of DocumentInfo)
            Get
                Return CurrentTreeView.CheckedNodes.Where(Function(node) node.Value.Eq(NODE_DOCUMENT_IDENTIFIER) OrElse node.Value.Eq(NODE_GROUP_IDENTIFIER)) _
                    .Select(Function(node) ExtractDocumentInfo(node)) _
                    .ToList()
            End Get
        End Property

        Public ReadOnly Property CurrentTreeView As RadTreeView
            Get
                If MultiViewEnable.HasValue AndAlso MultiViewEnable.Value Then
                    Dim treView As RadTreeView = CType(rtvListDocumentContainer.PageViews.Item(rtvListDocumentContainer.SelectedIndex).Controls.Item(0), RadTreeView)
                    Return treView
                End If
                Return rtvListDocument
            End Get
        End Property

        Public ReadOnly Property AllDocuments() As IList(Of DocumentInfo)
            Get
                Return CurrentTreeView.GetAllNodes().Where(Function(node) node.Value.Eq(NODE_DOCUMENT_IDENTIFIER)) _
                    .Select(Function(node) ExtractDocumentInfo(node)) _
                    .ToList()
            End Get
        End Property

        Public ReadOnly Property SelectedDocument() As DocumentInfo
            Get
                Dim node As RadTreeNode = Nothing
                Dim ret As DocumentInfo = Nothing
                If CurrentTreeView.SelectedNode IsNot Nothing AndAlso CurrentTreeView.SelectedNode.Attributes("key") IsNot Nothing Then
                    node = CurrentTreeView.SelectedNode
                End If
                If (node Is Nothing AndAlso CurrentTreeView IsNot Nothing) Then
                    node = CurrentTreeView.GetAllNodes().FirstOrDefault(Function(n) n.Value.Eq(NODE_DOCUMENT_IDENTIFIER))
                End If
                If (node IsNot Nothing) Then
                    ret = ExtractDocumentInfo(node)
                    ret.DownloadFileName = If(node.Attributes("ResFileName") IsNot Nothing, node.Attributes("ResFileName"), String.Empty)
                End If
                Return ret
            End Get
        End Property

        Public Property ToolBarVisible As Boolean
            Get
                Return ToolBar.Visible
            End Get
            Set(value As Boolean)
                ToolBar.Visible = value
                paneToolbar.Collapsed = Not value
            End Set
        End Property

        Public Property MultiViewDefaultId As Integer?
            Get
                Return CType(ViewState("multiViewDefaultId"), Integer?)
            End Get
            Set(value As Integer?)
                ViewState("multiViewDefaultId") = value
            End Set
        End Property

        Public Property MultiViewEnable As Nullable(Of Boolean)
            Get
                Return CType(ViewState("multiViewEnable"), Boolean?)
            End Get
            Set(value As Boolean?)
                ViewState("multiViewEnable") = value
            End Set
        End Property

        Public Property CollapseOnSingleDocument As Boolean
            Get
                Return _collapseOnSingleDocument.GetValueOrDefault(True)
            End Get
            Set(value As Boolean)
                _collapseOnSingleDocument = value
            End Set
        End Property

        Public Property LeftPaneStartWidth As Integer
            Get
                Return _leftPaneStartWidth.GetValueOrDefault(300)
            End Get
            Set(value As Integer)
                _leftPaneStartWidth = Math.Max(LeftPane.MinWidth, value)
            End Set
        End Property

        Public Property LeftPaneVisible As Boolean
            Get
                Return LeftPane.Visible
            End Get
            Set(value As Boolean)
                LeftPane.Visible = value
            End Set
        End Property

        Public Property ViewOriginal As Boolean
            Get
                Return _viewOriginal
            End Get
            Set(value As Boolean)
                _viewOriginal = value
            End Set
        End Property

        ''' <summary> Utilizzare la proprietà per inserire del testo CUSTOM nella signature dei documenti da visualizzare.
        ''' Utilizzata o per un oggetto mail o per un oggetto PEC </summary>
        Public Property AddSignature As String = String.Empty

        Public Property TreeViewKeys As ICollection(Of String)
            Get
                If _treeViewKeys IsNot Nothing Then
                    Return _treeViewKeys
                End If
                _treeViewKeys = New List(Of String)
                If ViewState("TreeViewKeys") IsNot Nothing Then
                    _treeViewKeys = DirectCast(ViewState("TreeViewKeys"), ICollection(Of String))
                End If
                Return _treeViewKeys
            End Get
            Set(value As ICollection(Of String))
                ViewState("TreeViewKeys") = value
            End Set
        End Property

        Public Property ModifyPrivacyEnabled As Boolean

        Public ReadOnly Property CurrentODataFacade As ODataFacade
            Get
                If _currentODataFacade Is Nothing Then
                    _currentODataFacade = New ODataFacade()
                    Return _currentODataFacade
                End If
                Return _currentODataFacade
            End Get
        End Property

        Public Property CurrentDocumentUnitID As Guid?

        Public Property CurrentLocationId As Integer?

        'Questa proprietà viene popolato solo nelle pagine di atti solo se l'atto non è ancora adottato e quindi non esiste nella document unit, oppure se l'atto è già esecutivo e quindi pubblico
        'Per il momento non vengono considerate le autorizzazioni privacy
        Public Property IdContainer As Integer?

        'Questa proprietà viene settata a true solo nei viewer dove il controllo dei diritti di visualizzazione documenti (a livello di handler) avviene tramite la sql function delle document unit
        'Ad oggi: ResolutionViewer, FileResolutionViewer e FascicleViewer
        Public Property CheckViewableRight As Boolean

        Public Property FromFascicle As Boolean

        Private ReadOnly Property CurrentXMLFactory As XmlFactory
            Get
                If _currentXMLFactory Is Nothing Then
                    _currentXMLFactory = New XmlFactory()
                End If
                Return _currentXMLFactory
            End Get
        End Property

        Public ReadOnly Property CurrentSelectedNode As RadTreeNode
            Get
                Dim currentNode As RadTreeNode = CurrentTreeView.SelectedNode
                If currentNode Is Nothing Then
                    currentNode = CurrentTreeView.GetAllNodes().FirstOrDefault(Function(n) n.Value.Eq(NODE_DOCUMENT_IDENTIFIER))
                End If
                Return currentNode
            End Get
        End Property

        Public Property Button_StartWorklow As String

        Public ReadOnly Property StartWorkflow_Window As String
            Get
                Return windowStartWorkflow.ClientID
            End Get
        End Property

        Private ReadOnly Property CopiaConforme_Btn As RadToolBarButton
            Get
                Dim downloadButtonToolbarBtn As RadToolBarButton = DirectCast(ToolBar.FindItemByValue("ViewerLight_Download"), RadToolBarButton)
                Return downloadButtonToolbarBtn
            End Get
        End Property

        Public Property StampaConformeEnabled As Boolean = True
        Public Property DocumentsPreviewEnabled As Boolean = True

#End Region

#Region " Events "
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            InitializeAjax()
            InitializePageElements()

            For Each key As String In TreeViewKeys
                InitializeTreeView(key)
            Next
            If Not Page.IsPostBack Then
                RadScriptManager.GetCurrent(Page).AsyncPostBackTimeout = 120
                AjaxManager.DefaultLoadingPanelID = "RadAjaxLoadingPanel1"

                If Not EnabledCheckIsSignedButton Then
                    ToolBar.Items.Remove(ToolBar.Items.FindItemByValue("ViewerLight_CheckIsSigned"))
                End If

                If ModifyPrivacyEnabled Then
                    windowModifyPrivacyLevel.Title = String.Concat("Modifica livelli di ", CommonBasePage.PRIVACY_LABEL)
                    Dim privacyButton As RadToolBarItem = ToolBar.Items.FindItemByValue("ViewerLight_ModifyPrivacy")
                    If privacyButton IsNot Nothing Then
                        privacyButton.Text = String.Concat("Modifica ", CommonBasePage.PRIVACY_LABEL)
                        privacyButton.ToolTip = String.Concat("Modifica ", CommonBasePage.PRIVACY_LABEL)
                    End If
                Else
                    ToolBar.Items.Remove(ToolBar.Items.FindItemByValue("ViewerLight_ModifyPrivacy"))
                End If

                If Not ProtocolEnv.BrowseDocumentHistoryEnabled Then
                    ToolBar.Items.Remove(ToolBar.Items.FindItemByValue("ViewerLight_Version"))
                End If

                Try
                    DataBind()
                Catch ex As ExtractException
                    AjaxManager.Alert(CommonBasePage.EXTRACT_COMPRESSFILE_ERROR)
                End Try
            End If

        End Sub

        ''' <summary>
        ''' ad ogni tick del timer verifico se in sessione ho il nome di un file PDF andato in errore.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Protected Sub timerMessage_Elapsed(ByVal sender As Object, ByVal e As System.EventArgs) Handles timerMessage.Tick
            If Session("DocumentHandlerError") IsNot Nothing Then
                messageBox.Visible = False
                messageText.Text = String.Empty
                If Not String.IsNullOrEmpty(Session("DocumentHandlerError")) Then
                    ' Espongo l'errore della stampa di un file PDF e mostro il popup contenente il nome del file.
                    messageBox.Visible = True
                    messageBox.Attributes("class") = "error"
                    messageText.Text = String.Format(DocumentEnv.Context.ProtocolEnv.StampaConformeMessageError, Session("DocumentHandlerError").ToString())
                End If

                Session.Remove("DocumentHandlerError")
            End If
        End Sub

        ''' <summary> Gestisce le chiamate Ajax dello UserControl </summary>
        Protected Sub RadAjaxManagerAjaxRequest(sender As Object, e As AjaxRequestEventArgs)

            If Not e.Argument.StartsWith("ViewerLight_") Then
                Exit Sub 'Non si tratta di una richiesta del Viewer
            End If

            Dim documents As List(Of DocumentInfo) = If(CheckedDocuments, New List(Of DocumentInfo)())

            Select Case e.Argument
                Case "ViewerLight_Version"
                    Dim ii As Integer = 0
                    For Each checkedDocument As BiblosDocumentInfo In documents.OfType(Of BiblosDocumentInfo)
                        Dim node As RadTreeNode = CurrentTreeView.CheckedNodes.SingleOrDefault(Function(n) (n.Value.Eq(NODE_DOCUMENT_IDENTIFIER) OrElse n.Value.Eq(NODE_GROUP_IDENTIFIER)) AndAlso n.Attributes("key").Eq(checkedDocument.Serialized))
                        Dim nodehasdoc As Boolean = False
                        If checkedDocument.DocumentParentId.HasValue AndAlso (Not node Is Nothing) Then
                            Dim BiblosDocInfos As ICollection(Of BiblosDocumentInfo) = Service.GetAllDocumentVersions(checkedDocument.DocumentParentId.Value)
                            Dim DocInfos As IList(Of DocumentInfo) = New List(Of DocumentInfo)
                            node.Selected = True
                            Dim count As Integer = node.Nodes.Count
                            For i As Integer = 0 To count - 1
                                node.Nodes.RemoveAt(0)
                            Next
                            For Each bibloDocument As BiblosDocumentInfo In BiblosDocInfos.OrderByDescending(Function(f) f.Version)
                                If bibloDocument.Version <> checkedDocument.Version Then
                                    Dim idbliblos As Guid = bibloDocument.DocumentId
                                    bibloDocument.Name = String.Concat(Path.GetFileNameWithoutExtension(bibloDocument.Name), "_", FormatNumber(bibloDocument.Version, 2).ToString().Replace(",", "."), bibloDocument.Extension)
                                    AddDocumentInfo(rtvListDocument, node, bibloDocument, True)
                                    nodehasdoc = True
                                End If
                            Next
                            If nodehasdoc Then node.Expanded = True
                        End If
                    Next

                Case "ViewerLight_Print"
                    If CheckedDocuments IsNot Nothing AndAlso CheckedDocuments.Count > 0 Then
                        documents = FilterDocuments(CheckedDocuments)
                    Else
                        documents = FilterDocuments(New List(Of DocumentInfo)({SelectedDocument}))
                    End If

                    If documents.IsNullOrEmpty Then
                        AjaxManager.ResponseScripts().Add("PrintDocument();")
                    Else
                        ' Stampa tutti i documenti in catena
                        ' Eseguo un Merge di tutti i pdf e lo visualizzo nell'ActiveX, successivamente lancio la stampa del documento

                        Dim file As FileInfo = GetDocumentToPrint(documents)
                        Dim temp As New TempFileDocumentInfo(file)
                        temp.Name = "Documents.pdf"

                        Dim link As String = GetGenericLink(temp.ToQueryString(), "Documents.pdf")
                        Dim jj As String = String.Format("PrintDocument('{0}');", link)
                        AjaxManager.ResponseScripts().Add(jj)
                    End If

                Case "ViewerLight_Download"

                    If Not StampaConformeEnabled Then
                        AjaxManager.Alert("La stampa conforme è disabilitata")
                        Exit Sub
                    End If

                    If documents.Count > 0 Then
                        Dim file As FileInfo = ZipPdfDocuments(documents)
                        DownloadFile(file)
                    Else
                        DownloadDocument(SelectedDocument, True)
                    End If
                    ' Preparo lo zip da scaricare, lo salvo in temp
                Case "ViewerLight_Original"

                    If documents.Count > 0 Then
                        ' Preparo lo zip da scaricare, lo salvo in temp
                        Dim file As FileInfo = ZipOriginalDocuments(documents)
                        DownloadFile(file)
                    Else
                        Dim doc As DocumentInfo = SelectedDocument
                        doc.DownloadFileName = String.Empty
                        DownloadDocument(doc, False)
                    End If
                Case "ViewerLight_CheckIsSigned"
                    Dim doc As DocumentInfo = SelectedDocument
                    doc.IsSigned = SignTools.CheckSigned(doc.Stream)
                    CurrentTreeView.SelectedNode.ImageUrl = ImagePath.FromDocumentInfo(doc)
                Case "ViewerLight_ModifyPrivacy"

                    If Not Me.CheckedDocuments.Any() Then
                        AjaxManager.Alert("Selezionare almeno un documento")
                        Exit Sub
                    End If

                    Dim selectedModels As List(Of ReferenceModel) = New List(Of ReferenceModel)
                    Dim selected As ReferenceModel
                    For Each checkedDocument As BiblosDocumentInfo In Me.CheckedDocuments
                        selected = New ReferenceModel()
                        selected.UniqueId = checkedDocument.DocumentId.ToString()
                        selected.IdArchiveChain = checkedDocument.ChainId.ToString()
                        selectedModels.Add(selected)
                    Next

                    AjaxManager.ResponseScripts.Add(String.Format(DEFAULT_OPEN_WINDOW_SCRIPT, ID, "windowModifyPrivacyLevel", CurrentDocumentUnitID, CurrentLocationId, JsonConvert.SerializeObject(selectedModels)))

                Case "ViewerLight_LoadInvoiceStylesheets"
                    If ProtocolEnv.InvoiceSDIEnabled Then
                        ResetToolbarButtons()
                        LoadInvoiceStyles()
                    End If
            End Select
        End Sub

#End Region

#Region " Methods "
        Private Sub InitializeAjax()
            AddHandler AjaxManager.AjaxRequest, AddressOf RadAjaxManagerAjaxRequest
            AjaxManager.AjaxSettings.AddAjaxSetting(ToolBar, ToolBar)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, RadPageSplitter)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, ToolBar, BasePage.MasterDocSuite.AjaxFlatLoadingPanel)
            AjaxManager.ClientEvents.OnResponseEnd = "requestEnd"
        End Sub

        Private Sub InitializePageElements()
            CopiaConforme_Btn.Enabled = StampaConformeEnabled
            PDFPane.Visible = DocumentsPreviewEnabled
            SplitBar.Visible = DocumentsPreviewEnabled

            If LeftPaneStartWidth <> 0 Then
                LeftPaneStartWidth = If(DocumentsPreviewEnabled, 30, 100)
            End If

            If DocumentsPreviewEnabled Then
                PDFPane.Width = Unit.Percentage(70)
            End If

            ToolBar.Items(0).Visible = ProtocolEnv.ViewLightSelectAllEnabled
            LeftPane.Width = Unit.Percentage(LeftPaneStartWidth)
        End Sub

        Private Shared Function ExtractDocumentInfo(node As RadTreeNode) As DocumentInfo
            Dim documentInfo As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(node.Attributes("key")))

            If node.Attributes.Keys.OfType(Of String).Any(Function(f) f = BIBLOS_ATTRIBUTE_UniqueId) Then
                documentInfo.AddAttribute(BIBLOS_ATTRIBUTE_UniqueId, node.Attributes(BIBLOS_ATTRIBUTE_UniqueId))
            End If
            If node.Attributes.Keys.OfType(Of String).Any(Function(f) f = BIBLOS_ATTRIBUTE_Environment) Then
                documentInfo.AddAttribute(BIBLOS_ATTRIBUTE_Environment, node.Attributes(BIBLOS_ATTRIBUTE_Environment))
            End If
            If node.Attributes.Keys.OfType(Of String).Any(Function(f) f = BIBLOS_ATTRIBUTE_IsPublic) Then
                documentInfo.AddAttribute(BIBLOS_ATTRIBUTE_IsPublic, node.Attributes(BIBLOS_ATTRIBUTE_IsPublic))
            End If
            If node.Attributes.Keys.OfType(Of String).Any(Function(f) f = BIBLOS_ATTRIBUTE_Miscellanea) Then
                documentInfo.AddAttribute(BIBLOS_ATTRIBUTE_Miscellanea, node.Attributes(BIBLOS_ATTRIBUTE_Miscellanea))
            End If
            If node.Attributes.Keys.OfType(Of String).Any(Function(f) f = BIBLOS_ATTRIBUTE_UserVisibilityAuthorized) Then
                documentInfo.AddAttribute(BIBLOS_ATTRIBUTE_UserVisibilityAuthorized, node.Attributes(BIBLOS_ATTRIBUTE_UserVisibilityAuthorized))
            End If
            If node.Attributes.Keys.OfType(Of String).Any(Function(f) f = BIBLOS_ATTRIBUTE_IsInvoice) Then
                documentInfo.AddAttribute(BIBLOS_ATTRIBUTE_IsInvoice, node.Attributes(BIBLOS_ATTRIBUTE_IsInvoice))
            End If
            If node.Attributes.Keys.OfType(Of String).Any(Function(f) f = BIBLOS_ATTRIBUTE_InvoiceKind) Then
                documentInfo.AddAttribute(BIBLOS_ATTRIBUTE_InvoiceKind, node.Attributes(BIBLOS_ATTRIBUTE_InvoiceKind))
            End If
            If node.Attributes.Keys.OfType(Of String).Any(Function(f) f = BIBLOS_ATTRIBUTE_Disabled) Then
                documentInfo.AddAttribute(BIBLOS_ATTRIBUTE_Disabled, node.Attributes(BIBLOS_ATTRIBUTE_Disabled))
            End If

            Return documentInfo
        End Function

        Public Sub Refresh()
            PdfSRC.Attributes.Add("value", String.Empty)
            DataBind()
        End Sub

        ''' <summary> Eseguo il Bind degli oggetti. </summary>
        Public Overloads Sub DataBind()
            ' Gestione multi viste
            If MultiViewEnable Then
                ' Nascondo il controllo classico
                rtvListDocument.Visible = False
                ' Attivo i controlli personalizzati
                multiPages.Visible = True
                LeftPane.Scrolling = SplitterPaneScrolling.None

                Dim ids As New List(Of String)

                Dim i As Integer = 0
                For Each rootDataSource As FolderInfo In DataSource
                    ids.Add(rootDataSource.ID)
                    '' Carico la dataSource di root per distribuirla su più tab
                    '' Costruisco il tab
                    tabStripViews.Tabs.Add(New RadTab(rootDataSource.Name))

                    Dim rtvListDocumentPerView As RadTreeView = InitializeTreeView(rootDataSource.ID)

                    ''Verifico il default
                    Dim isDefaultTreeView As Boolean = False
                    If rootDataSource.ID = MultiViewDefaultId Then
                        '' Imposto la pagina
                        rtvListDocumentContainer.SelectedIndex = i
                        '' Imposto il tab
                        tabStripViews.SelectedIndex = i
                        '' Imposto nella TreeViewCorrente il primo nodo da aprire
                        isDefaultTreeView = True
                    End If

                    '' Carico la treeView
                    LoadDocumentTree(rtvListDocumentPerView, rootDataSource.Children, isDefaultTreeView)
                    i += 1
                Next

                ViewState("TreeViewKeys") = ids
            Else
                LoadDocumentTree(rtvListDocument, DataSource, True)
            End If

            LeftPane.Collapsed = (Not AlwaysDocumentTreeOpen) AndAlso CollapseOnSingleDocument AndAlso CountDocuments() = 1
            If ProtocolEnv.InvoiceSDIEnabled Then
                ResetToolbarButtons()
                LoadInvoiceStyles()
            End If
        End Sub

        Private Function InitializeTreeView(key As String) As RadTreeView
            Dim rtvListDocumentPageView As RadPageView = rtvListDocumentContainer.FindPageViewByID(String.Format("rtvListDocumentPageView{0}", key))
            If rtvListDocumentPageView Is Nothing Then
                rtvListDocumentPageView = New RadPageView()
                rtvListDocumentPageView.ID = String.Format("rtvListDocumentPageView{0}", key)
                rtvListDocumentContainer.PageViews.Add(rtvListDocumentPageView)
            End If

            '' Carico la pagina
            '' Creo la treeView
            Dim rtvListDocumentPerView As New RadTreeView With {
                .ID = String.Format("rtvListDocumentPerView{0}", key),
                .CssClass = "TreeViewFullHeight DocumentTreeContainer",
                .OnClientNodeChecked = "ClientNodeChecked",
                .OnClientNodeClicked = "ClientNodeClicked",
                .CheckBoxes = CheckBoxes
                }
            rtvListDocumentPageView.Controls.Add(rtvListDocumentPerView)
            Return rtvListDocumentPerView
        End Function


        Private Sub LoadDocumentTree(rtvControl As RadTreeView, documentInfoList As IList(Of DocumentInfo), isDefaultTreeView As Boolean)
            'Caricamento classico
            rtvControl.Nodes.Clear()
            If documentInfoList.IsNullOrEmpty() Then
                Return
            End If
            _counter = 0
            For Each doc As DocumentInfo In documentInfoList
                AddDocumentInfo(rtvControl, Nothing, doc, isDefaultTreeView)
            Next
        End Sub

        Private Sub AddDocumentInfo(rtvControl As RadTreeView, parentNode As RadTreeNode, doc As DocumentInfo, isDefaultTreeView As Boolean)
            Dim node As RadTreeNode = RadTreeNodeFactory(doc, isDefaultTreeView)
            If IsNothing(parentNode) Then
                rtvControl.Nodes.Add(node)
            Else
                parentNode.Nodes.Add(node)
            End If
            ' Creazione del nodo
            If doc.Children.Count > 0 Then
                For Each child As DocumentInfo In doc.Children
                    AddDocumentInfo(rtvControl, node, child, isDefaultTreeView)
                Next
            ElseIf (FileHelper.MatchExtension(doc.Name, FileHelper.ZIP) OrElse FileHelper.MatchExtension(doc.Name, FileHelper.RAR)) AndAlso ProtocolEnv.ViewerUnzip Then
                node.Expanded = True

                Try
                    Dim compressManager As ICompress = New ZipCompress()
                    If FileHelper.MatchExtension(doc.Name, FileHelper.RAR) Then
                        compressManager = New RarCompress()
                    End If
                    Using memoryStream As New MemoryStream(doc.Stream)
                        For Each item As CompressItem In compressManager.InMemoryExtract(memoryStream)
                            Dim memoryInfo As New MemoryDocumentInfo(item.Data, item.Filename)
                            Dim file As New TempFileDocumentInfo(BiblosFacade.SaveUniqueToTemp(memoryInfo))
                            file.Name = item.Filename
                            file.Parent = doc
                            AddDocumentInfo(rtvControl, node, file, isDefaultTreeView)
                        Next
                    End Using
                Catch ex As ExtractException
                    FileLogger.Error(LogName.FileLog, ex.Message, ex)
                End Try
            End If
        End Sub

        Private Function GetParentID(doc As DocumentInfo) As String
            Dim fi As FolderInfo = TryCast(doc, FolderInfo)
            If fi IsNot Nothing Then
                If Not String.IsNullOrEmpty(fi.ID) Then
                    Return fi.ID
                End If
            End If

            If doc.Parent IsNot Nothing Then
                Return GetParentID(doc.Parent)
            End If

            Return String.Empty
        End Function

        Private Function GetParentID(node As RadTreeNode) As String
            If node Is Nothing OrElse String.IsNullOrEmpty(node.Attributes("key")) Then
                Return String.Empty
            End If
            Dim doc As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(node.Attributes("key")))
            Dim fi As FolderInfo = TryCast(doc, FolderInfo)
            If fi IsNot Nothing Then
                If Not String.IsNullOrEmpty(fi.ID) Then
                    Return fi.ID
                End If
            End If

            If node.ParentNode IsNot Nothing Then
                Return GetParentID(node.ParentNode)
            End If

            Return String.Empty
        End Function

        Private Function RadTreeNodeFactory(doc As DocumentInfo, isDefaultTreeView As Boolean) As RadTreeNode
            Dim items As NameValueCollection = doc.ToQueryString()
            Dim key As String = items.AsEncodedQueryString()

            Dim treeNode As RadTreeNode = New RadTreeNode
            Dim containDisabledAttribute As Boolean = False
            treeNode.Attributes.Add("key", key)
            If doc.Attributes IsNot Nothing Then
                Dim val As String
                If doc.Attributes.ContainsKey(BIBLOS_ATTRIBUTE_UniqueId) Then
                    val = doc.Attributes(BIBLOS_ATTRIBUTE_UniqueId)
                    treeNode.Attributes.Add(BIBLOS_ATTRIBUTE_UniqueId, val)
                    items.Add(BIBLOS_ATTRIBUTE_UniqueId, val)
                End If
                If doc.Attributes.ContainsKey(BIBLOS_ATTRIBUTE_Environment) Then
                    val = doc.Attributes(BIBLOS_ATTRIBUTE_Environment)
                    treeNode.Attributes.Add(BIBLOS_ATTRIBUTE_Environment, val)
                    items.Add(BIBLOS_ATTRIBUTE_Environment, val)
                End If
                If doc.Attributes.ContainsKey(BIBLOS_ATTRIBUTE_IsPublic) Then
                    val = doc.Attributes(BIBLOS_ATTRIBUTE_IsPublic)
                    treeNode.Attributes.Add(BIBLOS_ATTRIBUTE_IsPublic, val)
                    items.Add(BIBLOS_ATTRIBUTE_IsPublic, val)
                End If
                If doc.Attributes.ContainsKey(BIBLOS_ATTRIBUTE_Miscellanea) Then
                    val = doc.Attributes(BIBLOS_ATTRIBUTE_Miscellanea)
                    treeNode.Attributes.Add(BIBLOS_ATTRIBUTE_Miscellanea, val)
                    items.Add(BIBLOS_ATTRIBUTE_Miscellanea, val)
                End If
                If doc.Attributes.ContainsKey(BIBLOS_ATTRIBUTE_UserVisibilityAuthorized) Then
                    val = doc.Attributes(BIBLOS_ATTRIBUTE_UserVisibilityAuthorized)
                    treeNode.Attributes.Add(BIBLOS_ATTRIBUTE_UserVisibilityAuthorized, val)
                    items.Add(BIBLOS_ATTRIBUTE_UserVisibilityAuthorized, val)
                End If
                If doc.Attributes.ContainsKey(BIBLOS_ATTRIBUTE_Disabled) Then
                    val = doc.Attributes(BIBLOS_ATTRIBUTE_Disabled)
                    treeNode.Attributes.Add(BIBLOS_ATTRIBUTE_Disabled, val)
                    items.Add(BIBLOS_ATTRIBUTE_Disabled, val)
                    containDisabledAttribute = If(val = Boolean.TrueString, True, False)
                End If
                If doc.Attributes.ContainsKey(BIBLOS_ATTRIBUTE_Year) Then
                    val = doc.Attributes(BIBLOS_ATTRIBUTE_Year)
                    treeNode.Attributes.Add(BIBLOS_ATTRIBUTE_Year, val)
                    items.Add(BIBLOS_ATTRIBUTE_Year, val)
                End If
                If doc.Attributes.ContainsKey(BIBLOS_ATTRIBUTE_Number) Then
                    val = doc.Attributes(BIBLOS_ATTRIBUTE_Number)
                    treeNode.Attributes.Add(BIBLOS_ATTRIBUTE_Number, val)
                    items.Add(BIBLOS_ATTRIBUTE_Number, val)
                End If
            End If

            If TypeOf doc Is FolderInfo Then
                Dim fi As FolderInfo = DirectCast(doc, FolderInfo)
                ' Devo costruire un documento di tipo Folder
                treeNode.Text = fi.Caption
                treeNode.ToolTip = fi.Name
                treeNode.ImageUrl = ImagePath.SmallFolder
                treeNode.ExpandMode = TreeNodeExpandMode.ClientSide
                treeNode.Expanded = True
                treeNode.Value = NODE_FOLDER_IDENTIFIER
                treeNode.Checkable = True
            Else

                If ProtocolEnv.InvoiceSDIEnabled Then
                    If CheckIsInvoice(doc) Then
                        treeNode.Attributes.Add(BIBLOS_ATTRIBUTE_IsInvoice, True.ToString())
                        items.Add(BIBLOS_ATTRIBUTE_IsInvoice, True.ToString())

                        Dim invoiceModel As XMLConverterModel = GetInvoiceModel(doc)
                        treeNode.Attributes.Add(BIBLOS_ATTRIBUTE_InvoiceKind, invoiceModel.ModelKind.ToString())
                        items.Add(BIBLOS_ATTRIBUTE_InvoiceKind, invoiceModel.ModelKind.ToString())
                    End If
                End If

                _counter += 1
                ' DocumentInfo normale
                treeNode.Text = doc.Caption

                If DocumentsPreviewEnabled Then
                    treeNode.ToolTip = $"Visualizza {doc.Name} come PDF"
                End If

                If DocSuiteContext.Current.PrivacyLevelsEnabled AndAlso doc.Attributes.ContainsKey(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE) Then
                    Dim privacyLevel As WebAPICommons.PrivacyLevel = DocSuiteContext.Current.CurrentPrivacyLevels.SingleOrDefault(Function(x) x.Level = Convert.ToInt32(doc.Attributes(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE)))
                    If privacyLevel IsNot Nothing Then
                        If Not String.IsNullOrEmpty(privacyLevel.Colour) AndAlso Not privacyLevel.Colour.ToLower().Eq("#ffffff") Then
                            treeNode.Style.Add("color", privacyLevel.Colour)
                        End If
                        treeNode.SelectedCssClass = "selectedLevel"
                        treeNode.ToolTip = String.Concat(treeNode.ToolTip, " - livello di ", CommonBasePage.PRIVACY_LABEL, ": ", privacyLevel.Description)
                    End If
                End If
                treeNode.ImageUrl = ImagePath.FromDocumentInfo(doc)
                treeNode.Value = If((FileHelper.MatchExtension(doc.Name, FileHelper.ZIP) _
                    OrElse FileHelper.MatchExtension(doc.Name, FileHelper.RAR)), NODE_GROUP_IDENTIFIER, NODE_DOCUMENT_IDENTIFIER)

                items.Add("parent", GetParentID(doc))
                Dim resFileName As String = doc.PDFName
                treeNode.NavigateUrl = GetViewerLink(items, resFileName)
                treeNode.Attributes.Add("onclick", "return false;")

                If ProtocolEnv.EnableViewerLightFileRename AndAlso Not String.IsNullOrEmpty(_prefixFileName) Then
                    resFileName = String.Concat(_prefixFileName, "_Doc", _counter.ToString("000"), FileHelper.PDF)
                End If

                treeNode.Attributes.Add("ResFileName", resFileName)

                Dim viewItems As NameValueCollection = items
                If ViewOriginal Then
                    viewItems.Add("Original", "True")
                End If

                If DocumentsPreviewEnabled AndAlso Not containDisabledAttribute Then
                    treeNode.Attributes.Add("ViewLink", GetViewerLink(viewItems, resFileName))
                End If

                treeNode.Attributes.Add("Extension", doc.Extension)

                If TypeOf doc Is BiblosDocumentInfo Then
                    Dim fi As BiblosDocumentInfo = DirectCast(doc, BiblosDocumentInfo)
                    If fi.Version > 1 Then
                        treeNode.Attributes.Add("MoreVersion", "true")
                    Else
                        treeNode.Attributes.Add("MoreVersion", "false")
                    End If
                End If

                If StampaConformeEnabled AndAlso Not containDisabledAttribute Then
                    items.Add("Download", "true")
                    treeNode.Attributes.Add("DownloadLink", GetViewerLink(items, resFileName))
                End If

                items.Add("Original", "true")
                treeNode.Attributes.Add("DownloadOriginalLink", GetViewerLink(items, resFileName))

                treeNode.Checkable = True
                If FileExtensionBlackList IsNot Nothing AndAlso FileExtensionBlackList.Contains(doc.Extension) Then
                    treeNode.Attributes.Clear()
                    treeNode.Checkable = False
                    treeNode.Enabled = False
                    treeNode.NavigateUrl = String.Empty
                    treeNode.ToolTip = "Estensione del file configurata come non sicura o non valida. L'anteprima e il download del file è inibito dalle policy di sicurezza aziendali."
                End If

                If DocumentsPreviewEnabled AndAlso Not containDisabledAttribute Then
                    If treeNode.Enabled AndAlso isDefaultTreeView AndAlso String.IsNullOrEmpty(DefaultDocument) OrElse key.Eq(DefaultDocument) Then
                        SetFirst(treeNode)
                    End If
                End If

                If TypeOf doc Is BiblosDocumentInfo Then
                    Dim fi As BiblosDocumentInfo = DirectCast(doc, BiblosDocumentInfo)
                    treeNode.Attributes.Add(BIBLOS_ATTRIBUTE_BiblosChainId, fi.ChainId.ToString())
                    treeNode.Attributes.Add(BIBLOS_ATTRIBUTE_BiblosDocumentId, fi.DocumentId.ToString())
                    treeNode.Attributes.Add(BIBLOS_ATTRIBUTE_BiblosDocumentName, fi.Name)
                    treeNode.Attributes.Add(BIBLOS_ATTRIBUTE_Environment, Convert.ToInt32(DSWEnvironment.Document).ToString())
                End If
            End If

            If containDisabledAttribute Then
                treeNode.ToolTip = "Il seguente file non può essere convertito in un PDF in quanto la funzionalità è stata disattivata dall'amministratore del sistema. E' disponibile il solo download in originale"
                treeNode.NavigateUrl = String.Empty
            Else
                treeNode.Attributes.Add("HasDownload", "true")
            End If

            Return treeNode
        End Function

        ''' <summary>
        ''' Restituisce la stringa rappresentate il link di visualizzazione del documento.
        ''' </summary>
        ''' <param name="items">Insieme di parametri per la querystring</param>
        ''' <param name="name">Nome del documento</param>
        Private Function GetViewerLink(items As NameValueCollection, name As String) As String
            Return String.Format("{0}/Viewers/Handlers/{1}.ashx/{2}?{3}{4}",
                                             DocSuiteContext.Current.CurrentTenant.DSWUrl,
                                             DocumentSourcePage,
                                             FileHelper.FileNameToUrl(name),
                                             IIf(Me.AddSignature = String.Empty, String.Empty, String.Format("Signature={0}&", HttpUtility.HtmlEncode(Me.AddSignature))),
                                             CommonShared.AppendSecurityCheck(items.AsEncodedQueryString()))
        End Function

        Private Function GetGenericLink(items As NameValueCollection, name As String) As String

            Return String.Format("{0}/Viewers/Handlers/DocumentInfoHandler.ashx/{1}?{2}",
                                             DocSuiteContext.Current.CurrentTenant.DSWUrl,
                                             FileHelper.FileNameToUrl(name),
                                             CommonShared.AppendSecurityCheck(items.AsEncodedQueryString()))
        End Function

        ''' <summary> Imposta il nodo selezionato. </summary>
        ''' <param name="node">Nodo da visualizzare</param>
        ''' <remarks>Il primo nodo che chiama questo metodo viene registrato come quello aperto di default</remarks>
        Private Sub SetFirst(node As RadTreeNode)
            If String.IsNullOrEmpty(PdfSRC.Attributes.Item("value")) Then
                PdfSRC.Attributes.Add("value", node.Attributes.Item("ViewLink"))
                node.Selected = True
            End If
        End Sub

        ''' <summary> Esegue il download del documento. </summary>
        ''' <param name="file">File da scaricare</param>
        Private Sub DownloadFile(file As FileInfo)

            Dim temp As New TempFileDocumentInfo(file)
            temp.Name = FileName
            Dim items As NameValueCollection = temp.ToQueryString()
            items.Add("Download", "True")
            items.Add("Original", "True")
            Dim link As String = GetGenericLink(items, temp.Name)

            AjaxManager.ResponseScripts.Add(String.Format("StartDownload('{0}');", link))

        End Sub

        Private Sub DownloadDocument(doc As DocumentInfo, pdf As Boolean)
            If doc Is Nothing Then
                Return
            End If
            Dim items As NameValueCollection = doc.ToQueryString()
            If (doc.Attributes IsNot Nothing) Then
                If doc.Attributes.ContainsKey(BIBLOS_ATTRIBUTE_UniqueId) Then
                    items.Add(BIBLOS_ATTRIBUTE_UniqueId, doc.Attributes(BIBLOS_ATTRIBUTE_UniqueId))
                End If
                If doc.Attributes.ContainsKey(BIBLOS_ATTRIBUTE_Environment) Then
                    items.Add(BIBLOS_ATTRIBUTE_Environment, doc.Attributes(BIBLOS_ATTRIBUTE_Environment))
                End If
                If doc.Attributes.ContainsKey(BIBLOS_ATTRIBUTE_IsPublic) Then
                    items.Add(BIBLOS_ATTRIBUTE_IsPublic, doc.Attributes(BIBLOS_ATTRIBUTE_IsPublic))
                End If
                If doc.Attributes.ContainsKey(BIBLOS_ATTRIBUTE_Miscellanea) Then
                    items.Add(BIBLOS_ATTRIBUTE_Miscellanea, doc.Attributes(BIBLOS_ATTRIBUTE_Miscellanea))
                End If
                If doc.Attributes.ContainsKey(BIBLOS_ATTRIBUTE_UserVisibilityAuthorized) Then
                    items.Add(BIBLOS_ATTRIBUTE_UserVisibilityAuthorized, doc.Attributes(BIBLOS_ATTRIBUTE_UserVisibilityAuthorized))
                End If
                If doc.Attributes.ContainsKey(BIBLOS_ATTRIBUTE_Disabled) Then
                    items.Add(BIBLOS_ATTRIBUTE_Disabled, doc.Attributes(BIBLOS_ATTRIBUTE_Disabled))
                End If
            End If

            items.Add("parent", GetParentID(CurrentTreeView.SelectedNode))
            items.Add("Download", "true")
            Dim link As String

            If Not pdf Then
                items.Add("Original", "true")
                link = Me.GetViewerLink(items, doc.Name)
            Else
                'caso Copia conforme

                link = Me.GetViewerLink(items, If(String.IsNullOrEmpty(doc.DownloadFileName), doc.PDFName, doc.DownloadFileName))
            End If

            AjaxManager.ResponseScripts.Add(String.Format("StartDownload('{0}');", link))

        End Sub

        ''' <summary> Esegue il merge di tutti i documenti in un unico PDF </summary>
        Private Function GetDocumentToPrint(documents As List(Of DocumentInfo)) As FileInfo
            Dim merger As New PdfMerge()
            For Each document As DocumentInfo In documents
                Dim ext As String = Path.GetExtension(document.Name)
                If StringHelper.ExistsIn(DocSuiteContext.Current.ProtocolEnv.StampaConformeExtensions, ext, "|"c) Then
                    Try
                        merger.AddDocument(document.GetPdfStream())
                    Catch ex As Exception
                        Dim stream As New MemoryStream()
                        If String.Compare(document.Extension, FileHelper.PDF, True) = 0 Then
                            stream = New MemoryStream(document.Stream)
                        Else
                            FacadeFactory.Instance.BiblosFacade.GetExceptionReadingDocument(ex, stream)
                        End If
                        merger.AddDocument(stream.ToArray())
                    End Try
                Else
                    ' Aggiungo pagina di conversione non permessa
                    Dim stream As New MemoryStream()
                    Facade.BiblosFacade.GetConversionNotAllowedPdf(document, stream)
                    merger.AddDocument(stream.ToArray())
                End If
            Next

            'Caso in cui non si abbia diritti su nessun documento
            If merger.Documents.Count = 0 Then
                Return Nothing
            End If

            Dim tempFileName As String = FileHelper.UniqueFileNameFormat("Merged.pdf", DocSuiteContext.Current.User.UserName)
            Dim destination As String = Path.Combine(CommonInstance.AppTempPath, tempFileName)

            merger.Merge(destination)

            Return New FileInfo(destination)
        End Function

        ''' <summary> Genera uno ZIP a partire dai documenti passati per parametro. </summary>
        ''' <param name="files">Insieme dei documenti da zippare</param>
        ''' <returns>Riferimento allo Zip creato</returns>
        Private Function ZipBuilder(content As IList(Of KeyValuePair(Of String, Byte()))) As FileInfo
            Dim tempFileName As String = FileHelper.UniqueFileNameFormat(FileName, DocSuiteContext.Current.User.UserName)
            Dim destination As String = Path.Combine(CommonInstance.AppTempPath, tempFileName)
            Dim compressManager As ICompress = New ZipCompress()
            compressManager.Compress(content, destination)
            Return New FileInfo(destination)
        End Function

        ''' <summary> Genera lo zip dei documenti convertiti in Pdf. </summary>
        Private Function ZipPdfDocuments(documents As List(Of DocumentInfo)) As FileInfo
            Dim files As IList(Of KeyValuePair(Of String, Byte())) = New List(Of KeyValuePair(Of String, Byte()))()
            Dim allowedDocuments As List(Of DocumentInfo) = New List(Of DocumentInfo)()
            allowedDocuments = FilterDocuments(documents)
            For Each document As DocumentInfo In allowedDocuments
                Dim ext As String = Path.GetExtension(document.Name)
                If StringHelper.ExistsIn(DocSuiteContext.Current.ProtocolEnv.StampaConformeExtensions, ext, "|"c, "") Then
                    Try
                        files.Add(New KeyValuePair(Of String, Byte())(document.PDFName, document.GetPdfStream()))
                    Catch ex As Exception
                        Dim stream As New MemoryStream()
                        FacadeFactory.Instance.BiblosFacade.GetExceptionReadingDocument(ex, stream)
                        files.Add(New KeyValuePair(Of String, Byte())("Error.pdf", stream.ToArray()))
                    End Try
                Else
                    ' Aggiungo pagina di conversione non permessa
                    Dim stream As New MemoryStream()
                    Facade.BiblosFacade.GetConversionNotAllowedPdf(document, stream)
                    files.Add(New KeyValuePair(Of String, Byte())("Conversione non possibile.pdf", stream.ToArray()))
                End If
            Next

            Return ZipBuilder(files)
        End Function

        ''' <summary> Genera lo zip dei documenti originali. </summary>
        Private Function ZipOriginalDocuments(documents As List(Of DocumentInfo)) As FileInfo
            Dim files As IList(Of KeyValuePair(Of String, Byte())) = New List(Of KeyValuePair(Of String, Byte()))()
            Dim allowedDocuments As List(Of DocumentInfo) = New List(Of DocumentInfo)()
            allowedDocuments = FilterDocuments(documents)
            For Each document As DocumentInfo In allowedDocuments
                Try
                    files.Add(New KeyValuePair(Of String, Byte())(document.Name, document.Stream))
                Catch ex As Exception
                    Dim stream As New MemoryStream()
                    FacadeFactory.Instance.BiblosFacade.GetExceptionReadingDocument(ex, stream)
                    files.Add(New KeyValuePair(Of String, Byte())("Error.pdf", stream.ToArray()))
                End Try
            Next
            Return ZipBuilder(files)
        End Function

        Public Function FilterDocuments(documents As List(Of DocumentInfo)) As List(Of DocumentInfo)
            Dim filteredDocuments As List(Of DocumentInfo) = New List(Of DocumentInfo)()
            Dim uniqueId As String = String.Empty
            Dim environment As String = String.Empty
            Dim isPublic As Boolean = False
            Dim id As Guid
            Dim viewRight As Boolean
            Dim documentUnit As Entity.DocumentUnits.DocumentUnit
            Dim validTypes As Integer()
            Dim udRight As Boolean
            Dim disabled As Boolean = False
            For Each doc As DocumentInfo In documents.Where(Function(x) Not x.IsRemoved)
                disabled = False
                If doc.Attributes.Any(Function(a) a.Key.Eq(BIBLOS_ATTRIBUTE_UniqueId)) Then
                    uniqueId = doc.Attributes(BIBLOS_ATTRIBUTE_UniqueId)
                End If
                If doc.Attributes.Any(Function(a) a.Key.Eq(BIBLOS_ATTRIBUTE_Environment)) Then
                    environment = doc.Attributes(BIBLOS_ATTRIBUTE_Environment)
                End If

                If doc.Attributes.Any(Function(a) a.Key.Eq(BIBLOS_ATTRIBUTE_Disabled)) Then
                    disabled = Boolean.Parse(doc.Attributes(BIBLOS_ATTRIBUTE_Disabled))
                End If
                If disabled Then
                    Continue For
                End If

                If doc.Attributes.Any(Function(a) a.Key.Eq(BIBLOS_ATTRIBUTE_IsPublic)) Then
                    isPublic = Convert.ToBoolean(doc.Attributes(BIBLOS_ATTRIBUTE_IsPublic))
                End If
                validTypes = New Integer() {DSWEnvironment.Protocol, DSWEnvironment.DocumentSeries, DSWEnvironment.UDS, DSWEnvironment.Resolution}
                If isPublic OrElse String.IsNullOrEmpty(environment) OrElse Not validTypes.Contains(Convert.ToInt32(environment)) Then
                    filteredDocuments.Add(doc)
                    Continue For
                End If

                'Considero il caso degli atti in cui l'atto non è ancora adottato (non c'è nella document unit) e va controllata la privacy del documento
                If IdContainer.HasValue AndAlso Convert.ToInt32(environment) = DSWEnvironment.Resolution AndAlso Facade.DocumentFacade.CheckPrivacy(doc, IdContainer.Value, Nothing, Nothing, environment, False) Then
                    filteredDocuments.Add(doc)
                    Continue For
                End If

                If Not String.IsNullOrEmpty(uniqueId) Then
                    id = Guid.Parse(uniqueId)
                    If CheckViewableRight Then
                        viewRight = CurrentODataFacade.HasViewableRight(id, DocSuiteContext.Current.User.UserName, DocSuiteContext.Current.User.Domain)
                    End If

                    If (Not CheckViewableRight OrElse viewRight) AndAlso (Not DocSuiteContext.Current.PrivacyEnabled OrElse FromFascicle) Then
                        documentUnit = WebAPIImpersonatorFacade.ImpersonateFinder(New DocumentUnitFinder(DocSuiteContext.Current.Tenants),
                            Function(impersonationType, wfinder)
                                wfinder.ResetDecoration()
                                wfinder.EnablePaging = False
                                wfinder.IdDocumentUnit = id
                                wfinder.ExpandContainer = True
                                wfinder.ExpandRoles = True
                                wfinder.ExpandUsers = True
                                Return wfinder.DoSearch().Select(Function(f) f.Entity).SingleOrDefault()
                            End Function)

                        If documentUnit IsNot Nothing Then
                            udRight = Facade.DocumentFacade.CheckPrivacy(doc, documentUnit.Container.EntityShortId,
                                                               documentUnit.DocumentUnitRoles.Where(Function(d) Not d.AuthorizationRoleType = RoleAuthorizationType.Responsible).Select(Function(r) r.UniqueIdRole).ToArray(),
                                                               documentUnit.DocumentUnitRoles.Where(Function(d) d.AuthorizationRoleType = RoleAuthorizationType.Responsible).Select(Function(r) r.UniqueIdRole).ToArray(),
                                                               documentUnit.Environment, documentUnit.DocumentUnitUsers.Any(Function(d) d.AuthorizationType = RoleAuthorizationType.Accounted AndAlso d.Account.Equals(DocSuiteContext.Current.User.FullUserName)))
                            If udRight Then
                                filteredDocuments.Add(doc)
                            End If
                        Else
                            FileLogger.Warn(LogName.FileLog, String.Concat("Unità documentaria con id ", uniqueId, " non trovata"))
                        End If
                    End If
                End If
            Next
            Return filteredDocuments
        End Function

        Public Sub ReloadViewer(arguments As String(), externalDataSource As List(Of DocumentInfo))

            If arguments.Length >= 1 AndAlso arguments(0).Eq("PrivacyWindowClose") Then
                AjaxManager.ResponseScripts.Add(SHOW_ACTIVEX_SCRIPT)
                If arguments.Length >= 2 AndAlso arguments(1).Eq("true") Then
                    DataSource = externalDataSource
                    DataBind()
                End If
            End If
        End Sub

        Private Function GetInvoiceModel(document As DocumentInfo) As XMLConverterModel
            Dim invoiceModel As XMLConverterModel = New XMLConverterModel() With {.ModelKind = XMLModelKind.Invalid}
            If (Not document.Extension.Eq(FileHelper.XML) AndAlso Not document.Extension.Eq(FileHelper.P7M)) Then
                Return invoiceModel
            End If

            If document.Extension.Eq(FileHelper.P7M) AndAlso
                Not Path.GetFileNameWithoutExtension(document.Name).EndsWith(FileHelper.XML, StringComparison.InvariantCultureIgnoreCase) Then
                Return invoiceModel
            End If

            Dim xmlContent As String = String.Empty
            If (document.Extension.Eq(FileHelper.P7M)) Then
                xmlContent = EInvoiceHelper.TryGetInvoiceSignedContent(document.Stream, Sub(f, ex) FileLogger.Warn(LogName.FileLog, ex.Message, ex))
            Else
                xmlContent = Encoding.Default.GetString(document.Stream)
            End If

            If String.IsNullOrEmpty(xmlContent) Then
                Return invoiceModel
            End If

            If (Not xmlContent.StartsWith("<")) Then
                Dim index As Integer = xmlContent.IndexOf("<")
                xmlContent = xmlContent.Substring(index, xmlContent.Length - index)
            End If

            invoiceModel = CurrentXMLFactory.BuildXmlModel(xmlContent)
            Return invoiceModel
        End Function

        Private Function CheckIsInvoice(document As DocumentInfo) As Boolean
            Dim invoiceModel As XMLConverterModel = GetInvoiceModel(document)
            Return (invoiceModel.ModelKind = XMLModelKind.InvoicePA_V12 OrElse invoiceModel.ModelKind = XMLModelKind.InvoicePR_V12 OrElse
                            invoiceModel.ModelKind = XMLModelKind.InvoicePA_V10 OrElse invoiceModel.ModelKind = XMLModelKind.InvoicePA_V11)
        End Function

        Private Sub ResetToolbarButtons()
            Dim styleButtons As ICollection(Of RadToolBarButton) = ToolBar.GetGroupButtons("InvoiceStyle")
            If Not styleButtons.IsNullOrEmpty() Then
                For Each styleButton As RadToolBarButton In styleButtons.ToList()
                    ToolBar.Items.Remove(styleButton)
                Next
            End If
        End Sub

        Private Sub LoadInvoiceStyles()
            Dim currentNode As RadTreeNode = CurrentSelectedNode
            Dim invoiceKind As XMLModelKind
            Dim isInvoice As Boolean
            If (currentNode Is Nothing OrElse currentNode.Attributes(BIBLOS_ATTRIBUTE_IsInvoice) Is Nothing OrElse currentNode.Attributes(BIBLOS_ATTRIBUTE_InvoiceKind) Is Nothing) Then
                Return
            End If

            If [Enum].TryParse(currentNode.Attributes(BIBLOS_ATTRIBUTE_InvoiceKind), invoiceKind) AndAlso
                (Boolean.TryParse(currentNode.Attributes(BIBLOS_ATTRIBUTE_IsInvoice), isInvoice) AndAlso isInvoice) Then
                Dim invoiceResources As InvoiceResources = DocSuiteContext.InvoiceResources.FirstOrDefault(Function(x) x.InvoiceKind = invoiceKind)
                If invoiceResources Is Nothing Then
                    Return
                End If

                Dim separator As RadToolBarButton = New RadToolBarButton(String.Empty, False, "InvoiceStyle") With
                {
                    .IsSeparator = True
                }
                separator.AddAttribute("IsInvoiceStyle", True.ToString())
                ToolBar.Items.Add(separator)

                Dim toCreateButton As RadToolBarButton
                Dim documentAttributes As NameValueCollection
                Dim currentSelectedDocument As DocumentInfo = SelectedDocument
                For Each invoiceStyle As KeyValuePair(Of String, String) In invoiceResources.Stylesheets
                    toCreateButton = New RadToolBarButton(invoiceStyle.Key, False, "InvoiceStyle") With
                    {
                        .CheckOnClick = True,
                        .Value = "ViewerLight_LoadInvoiceStylesheets",
                        .ImageUrl = _invoiceStyleIcon.Where(Function(x) x.Key = invoiceStyle.Key).Select(Function(s) s.Value).FirstOrDefault()
                    }

                    documentAttributes = currentSelectedDocument.ToQueryString()
                    If (currentSelectedDocument.Attributes IsNot Nothing) Then
                        If currentSelectedDocument.Attributes.ContainsKey(BIBLOS_ATTRIBUTE_UniqueId) Then
                            documentAttributes.Add(BIBLOS_ATTRIBUTE_UniqueId, currentSelectedDocument.Attributes(BIBLOS_ATTRIBUTE_UniqueId))
                        End If
                        If currentSelectedDocument.Attributes.ContainsKey(BIBLOS_ATTRIBUTE_Environment) Then
                            documentAttributes.Add(BIBLOS_ATTRIBUTE_Environment, currentSelectedDocument.Attributes(BIBLOS_ATTRIBUTE_Environment))
                        End If
                        If currentSelectedDocument.Attributes.ContainsKey(BIBLOS_ATTRIBUTE_IsPublic) Then
                            documentAttributes.Add(BIBLOS_ATTRIBUTE_IsPublic, currentSelectedDocument.Attributes(BIBLOS_ATTRIBUTE_IsPublic))
                        End If
                        If currentSelectedDocument.Attributes.ContainsKey(BIBLOS_ATTRIBUTE_Miscellanea) Then
                            documentAttributes.Add(BIBLOS_ATTRIBUTE_Miscellanea, currentSelectedDocument.Attributes(BIBLOS_ATTRIBUTE_Miscellanea))
                        End If
                        If currentSelectedDocument.Attributes.ContainsKey(BIBLOS_ATTRIBUTE_UserVisibilityAuthorized) Then
                            documentAttributes.Add(BIBLOS_ATTRIBUTE_UserVisibilityAuthorized, currentSelectedDocument.Attributes(BIBLOS_ATTRIBUTE_UserVisibilityAuthorized))
                        End If
                        If currentSelectedDocument.Attributes.ContainsKey(BIBLOS_ATTRIBUTE_IsInvoice) Then
                            documentAttributes.Add(BIBLOS_ATTRIBUTE_IsInvoice, currentSelectedDocument.Attributes(BIBLOS_ATTRIBUTE_IsInvoice))
                        End If
                        If currentSelectedDocument.Attributes.ContainsKey(BIBLOS_ATTRIBUTE_InvoiceKind) Then
                            documentAttributes.Add(BIBLOS_ATTRIBUTE_InvoiceKind, currentSelectedDocument.Attributes(BIBLOS_ATTRIBUTE_InvoiceKind))
                        End If
                        If currentSelectedDocument.Attributes.ContainsKey(BIBLOS_ATTRIBUTE_Disabled) Then
                            documentAttributes.Add(BIBLOS_ATTRIBUTE_Disabled, currentSelectedDocument.Attributes(BIBLOS_ATTRIBUTE_Disabled))
                        End If
                    End If

                    documentAttributes.Add("parent", GetParentID(currentNode))
                    documentAttributes.Add("InvoiceStylePosition", invoiceResources.Stylesheets.Values.ToList().IndexOf(invoiceStyle.Value).ToString())

                    toCreateButton.AddAttribute("ViewerLink", GetViewerLink(documentAttributes, currentNode.Attributes("ResFileName")))
                    toCreateButton.AddAttribute("IsInvoiceStyle", True.ToString())
                    ToolBar.Items.Add(toCreateButton)
                Next
            End If
        End Sub

        Private Function CountDocuments() As Integer
            Return CurrentTreeView.GetAllNodes().Where(Function(node) node.Value.Eq(NODE_DOCUMENT_IDENTIFIER)).Count()
        End Function
#End Region

    End Class

End Namespace