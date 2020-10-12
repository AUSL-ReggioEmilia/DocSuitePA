Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Commons
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Commons
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging

Partial Public Class uscDocumentUpload
    Inherits DocSuite2008BaseControl

    Public Event DocumentUploaded(ByVal sender As Object, ByVal e As DocumentEventArgs)
    Public Event DocumentSigned(ByVal sender As Object, ByVal e As DocumentSignedEventArgs)
    Public Event DocumentBeforeSign(ByVal sender As Object, ByVal e As DocumentBeforeSignEventArgs)
    Public Event DocumentRemoved(ByVal sender As Object, ByVal e As DocumentEventArgs)
    Public Event DocumentSelected(ByVal sender As Object, ByVal e As DocumentEventArgs)
    Public Event ButtonFrontespizioClick(ByVal sender As Object, ByVal e As EventArgs)

#Region " Fields "

    Private Const LoggerName As String = LogName.FileLog
    Private Const NodeAttributeLocked As String = "Locked"
    Private Const OpenWindowScript As String = "return {0}_OpenWindow('{1}', '{2}', '{3}');"
    Private Const OpenWindowScannerScript As String = "return {0}_OpenWindowScanner('{1}', '{2}', '{3}');"
    Private Const OpenWindowSignScript As String = "return {0}_OpenWindowSign('{1}', '{2}', '{3}');"
    Private Const PrefixPadding As String = "000"
    Private Const TREEVIEW_NODE_DROPPING_FUNCTION As String = "{0}_onNodeDropping"
    Private Const EDIT_DOCUMENT_SCRIPT As String = "return {0}_editDocumentName();"
    Private Const TREENODE_EDITED_SCRIPT As String = "{0}_treeDocumentEdited"
    Private Const TREENODE_EDITED_END_SCRIPT As String = "{0}_responseEnd();"
    Private Const FakeNodeValue As String = "meta"

    Private _wndHeight As Integer?
    Private _wndWidth As Integer?
    Private _fileInputsCount As Integer
    Private _sharedFolder As DirectoryInfo
    Private _isAttachment As Boolean = False
    Private _checkableDocuments As Nullable(Of Boolean)
    Private _sendSourceDocument As Nullable(Of Boolean)
    Private _btnVersioning As Button
    Private _privacyLevelFacade As PrivacyLevelFacade

    Private _allowedExtensions As List(Of String)

    Private _prefix As String
    Private _prefixKey As String
    Private _originalTreeViewCaption As String
    Private _filenameAutomaticRenameEnabled As Boolean = False

    Private _documentInfos As IList(Of DocumentInfo)
    Private _documentInfosAdded As IList(Of DocumentInfo)
    Private _allowZipDocument As Boolean = False
    Private _checkedDocumentInfos As IList(Of DocumentInfo)
    Private _minPrivacyLevel As Integer?
    Private _maxPrivacyLevel As Integer?
    Private Shared _defaultPrivacyLevel As Integer?

#End Region

#Region " Properties "

    Public ReadOnly Property JavascriptClosingFunction As String
        Get
            Return ID & "_CloseDocument"
        End Get
    End Property
    Public ReadOnly Property JavascriptClosingScannerRestFunction As String
        Get
            Return ID & "_CloseScannerRestDocument"
        End Get
    End Property

    Public ReadOnly Property JavascriptCloseCopyFunction As String
        Get
            Return ID & "_CloseCopyDocument"
        End Get
    End Property

    Public ReadOnly Property JavascriptCloseProtFunction As String
        Get
            Return ID & "_CloseUploadDocumentProt"
        End Get
    End Property

    Public ReadOnly Property JavascriptCloseReslFunction As String
        Get
            Return ID & "_CloseCopyDocumentResl"
        End Get
    End Property

    Public ReadOnly Property JavascriptCloseSeriesFunction As String
        Get
            Return ID & "_CloseCopyDocumentSeries"
        End Get
    End Property

    Public ReadOnly Property JavascriptCloseUDSFunction As String
        Get
            Return ID & "_CloseCopyDocumentUDS"
        End Get
    End Property

    Public ReadOnly Property JavascriptCloseSelectTemplateFunction As String
        Get
            Return ID & "_CloseSelectTemplateWindow"
        End Get
    End Property

    Public ReadOnly Property EncodedSelectedNodeValue As String
        Get
            Dim node As RadTreeNode = SelectedNode
            If node IsNot Nothing Then
                Return Page.Server.UrlEncode(node.Value)
            End If

            Return String.Empty
        End Get
    End Property

    Public ReadOnly Property DocumentsCount As Integer
        Get
            Return RadTreeViewDocument.Nodes(0).Nodes.Count
        End Get
    End Property

    Public ReadOnly Property HasDocuments As Boolean
        Get
            Return DocumentsCount > 0
        End Get
    End Property

    Public ReadOnly Property DocumentsAddedCount As Integer
        Get
            Dim count As Integer = 0
            For Each node As RadTreeNode In RadTreeViewDocument.Nodes(0).Nodes
                If String.Compare(node.Attributes("Added"), "True", StringComparison.OrdinalIgnoreCase) = 0 Then
                    count += 1
                End If
            Next
            Return count
        End Get
    End Property

    Public Property MultipleDocuments As Boolean
        Get
            Return CType(ViewState("MultipleDocuments"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            ViewState("MultipleDocuments") = value
        End Set
    End Property

    Public Property Caption As String
        Get
            Return lblCaption.Text
        End Get
        Set(ByVal value As String)
            lblCaption.Text = value
        End Set
    End Property

    Public Property IsDocumentRequired As Boolean
        Get
            Return rfvDocument.Enabled
        End Get
        Set(ByVal value As Boolean)
            rfvDocument.Enabled = value
            rfvDocument.Visible = value
        End Set
    End Property

    Public Property TreeViewCaption As String
        Get
            Return RadTreeViewDocument.Nodes(0).Text
        End Get
        Set(ByVal value As String)
            '' Salvo la TreeViewCaption originale (se già non è stato fatto)
            If String.IsNullOrEmpty(_originalTreeViewCaption) Then
                _originalTreeViewCaption = value
            End If
            RadTreeViewDocument.Nodes(0).Text = value
        End Set
    End Property

    Public Property SharedFolder As DirectoryInfo
        Get
            Return _sharedFolder
        End Get
        Set(ByVal value As DirectoryInfo)
            _sharedFolder = value
        End Set
    End Property

    Public Property IsAttachment As Boolean
        Get
            Return _isAttachment
        End Get
        Set(ByVal value As Boolean)
            _isAttachment = value
        End Set
    End Property

    Public Property BtnVersioning As Button
        Get
            Return _btnVersioning
        End Get
        Set(ByVal value As Button)
            _btnVersioning = value
        End Set
    End Property

    ''' <summary> Abilita il pulsante di firma. </summary>
    Public Property SignButtonEnabled As Boolean
        Get
            Return btnSignDocument.Visible
        End Get
        Set(ByVal value As Boolean)
            btnSignDocument.Visible = value
        End Set
    End Property

    Public Property ButtonPreviewEnabled As Boolean
        Get
            Return btnPreviewDoc.Visible
        End Get
        Set(ByVal value As Boolean)
            btnPreviewDoc.Visible = value
        End Set
    End Property

    Public Property ButtonScannerEnabled As Boolean
        Get
            Return btnAddDocumentScanner.Visible
        End Get
        Set(ByVal value As Boolean)
            btnAddDocumentScanner.Visible = value
        End Set
    End Property

    Public Property FilenameAutomaticRenameEnabled As Boolean
        Get
            Return _filenameAutomaticRenameEnabled
        End Get
        Set(ByVal value As Boolean)
            _filenameAutomaticRenameEnabled = value
        End Set
    End Property

    Public Property ButtonFileEnabled() As Boolean
        Get
            Return btnAddDocument.Visible
        End Get
        Set(ByVal value As Boolean)
            btnAddDocument.Visible = value
        End Set
    End Property
    Public Property ButtonLibrarySharepointEnabled() As Boolean
        Get
            Return btnUploadSharepoint.Visible
        End Get
        Set(ByVal value As Boolean)
            btnUploadSharepoint.Visible = value
        End Set
    End Property

    Public Property ButtonRemoveEnabled As Boolean
        Get
            Return btnRemoveDocument.Visible
        End Get
        Set(ByVal value As Boolean)
            btnRemoveDocument.Visible = value
        End Set
    End Property

    Public Property ButtonSharedFolederEnabled As Boolean
        Get
            Return btnImportSharedFolder.Visible
        End Get
        Set(ByVal value As Boolean)
            btnImportSharedFolder.Visible = value
        End Set
    End Property

    Public Property ButtonFrontespizioEnabled As Boolean
        Get
            Return tblFrontespizio.Visible
        End Get
        Set(ByVal value As Boolean)
            tblFrontespizio.Visible = value
        End Set
    End Property

    Public Property ButtonSelectTemplateEnabled As Boolean
        Get
            Return btnSelectTemplate.Visible
        End Get
        Set(ByVal value As Boolean)
            btnSelectTemplate.Visible = value
        End Set
    End Property

    Public Property ButtonPrivacyLevelVisible As Boolean
        Get
            Return btnPrivacyLevel.Visible
        End Get
        Set(ByVal value As Boolean)
            btnPrivacyLevel.Visible = value
        End Set
    End Property

    Public ReadOnly Property ButtonCopyProtocol As ImageButton
        Get
            Return btnCopyProtocol
        End Get
    End Property

    Public ReadOnly Property ButtonCopyResl As ImageButton
        Get
            Return btnCopyResl
        End Get
    End Property

    Public ReadOnly Property ButtonCopySeries As ImageButton
        Get
            Return btnCopySeries
        End Get
    End Property

    Public ReadOnly Property ButtonCopyUDS As ImageButton
        Get
            Return btnCopyUDS
        End Get
    End Property

    Public ReadOnly Property ButtonAddDocument As ImageButton
        Get
            Return btnAddDocument
        End Get
    End Property

    Public ReadOnly Property ButtonLibrarySharepoint As ImageButton
        Get
            Return btnUploadSharepoint
        End Get
    End Property

    Public Property [ReadOnly] As Boolean
        Get
            Return Not (tblButtons.Visible)
        End Get
        Set(ByVal value As Boolean)
            tblButtons.Visible = Not value
            rfvDocument.Enabled = Not value
        End Set
    End Property

    Private ReadOnly Property PrivacyLevelFacade As PrivacyLevelFacade
        Get
            If _privacyLevelFacade Is Nothing Then
                _privacyLevelFacade = New PrivacyLevelFacade()
            End If
            Return _privacyLevelFacade
        End Get
    End Property


    ''' <summary> Nodo selezionato. </summary>
    ''' <remarks> Esclude il nodo root, in presenza di un solo documento torna quello. </remarks>
    Public ReadOnly Property SelectedNode As RadTreeNode
        Get
            If DocumentsCount = 1 Then
                Return RadTreeViewDocument.Nodes(0).Nodes(0)
            ElseIf DocumentsCount > 1 AndAlso RadTreeViewDocument.SelectedNode IsNot Nothing AndAlso Not RadTreeViewDocument.SelectedNode.Value.Eq("root") Then
                Return RadTreeViewDocument.SelectedNode
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property SelectedDocumentInfo As DocumentInfo
        Get
            Dim tmpDocInfo As DocumentInfo = GetDocumentInfoByNode(SelectedNode)
            If tmpDocInfo IsNot Nothing Then
                tmpDocInfo.AddAttribute(BiblosFacade.DOCUMENT_POSITION_ATTRIBUTE, SelectedNode.Index.ToString())
            End If
            Return tmpDocInfo
        End Get
    End Property

    Public ReadOnly Property DocumentInfos As IList(Of DocumentInfo)
        Get
            If _documentInfos Is Nothing Then
                _documentInfos = New List(Of DocumentInfo)
                Dim privacyLvl As Integer = 0
                For Each node As RadTreeNode In RadTreeViewDocument.Nodes(0).Nodes
                    If Not node.Attributes("key") Is Nothing Then
                        Dim tmpDocInfo As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(node.Attributes("key")))
                        tmpDocInfo.AddAttribute(BiblosFacade.DOCUMENT_POSITION_ATTRIBUTE, node.Index.ToString())
                        privacyLvl = If(DocSuiteContext.Current.PrivacyLevelsEnabled, DefaultPrivacyLevel, 0)
                        If node.Attributes(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE) IsNot Nothing Then
                            privacyLvl = CInt(node.Attributes(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE))
                        End If
                        If DocSuiteContext.Current.PrivacyLevelsEnabled AndAlso PrivacyLevelVisible Then
                            Dim ddlPrivacyLevels As RadDropDownList = DirectCast(node.FindControl("ddlPrivacyLevels"), RadDropDownList)
                            If ddlPrivacyLevels IsNot Nothing Then
                                privacyLvl = CInt(ddlPrivacyLevels.SelectedValue)
                            End If
                        End If
                        tmpDocInfo.AddAttribute(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE, privacyLvl.ToString())
                        _documentInfos.Add(tmpDocInfo)
                    End If
                Next
            End If
            Return _documentInfos
        End Get
    End Property

    Public ReadOnly Property DocumentInfosAdded As IList(Of DocumentInfo)
        Get
            If _documentInfosAdded Is Nothing Then
                _documentInfosAdded = New List(Of DocumentInfo)
                Dim tmpDocInfo As DocumentInfo
                Dim privacyLvl As Integer = 0
                For Each node As RadTreeNode In RadTreeViewDocument.Nodes(0).Nodes
                    If Not node.Attributes("key") Is Nothing AndAlso node.Attributes("Added") = "True" Then
                        tmpDocInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(node.Attributes("key")))
                        tmpDocInfo.AddAttribute(BiblosFacade.DOCUMENT_POSITION_ATTRIBUTE, node.Index.ToString())
                        privacyLvl = If(DocSuiteContext.Current.PrivacyLevelsEnabled, DefaultPrivacyLevel, 0)
                        If node.Attributes(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE) IsNot Nothing Then
                            privacyLvl = CInt(node.Attributes(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE))
                        End If
                        If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                            If PrivacyLevelVisible Then
                                Dim ddlPrivacyLevels As RadDropDownList = DirectCast(node.FindControl("ddlPrivacyLevels"), RadDropDownList)
                                If ddlPrivacyLevels IsNot Nothing AndAlso Not String.IsNullOrEmpty(ddlPrivacyLevels.SelectedValue) Then
                                    privacyLvl = CInt(ddlPrivacyLevels.SelectedValue)
                                End If
                            End If
                            If node.Attributes("PrivacyLevel") IsNot Nothing Then
                                privacyLvl = Convert.ToInt32(node.Attributes("PrivacyLevel"))
                            End If
                        End If
                        tmpDocInfo.AddAttribute(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE, privacyLvl.ToString())
                        _documentInfosAdded.Add(tmpDocInfo)
                    End If
                Next
            End If
            Return _documentInfosAdded
        End Get
    End Property

    Public ReadOnly Property CheckedDocumentInfos As IList(Of DocumentInfo)
        Get
            If _checkedDocumentInfos Is Nothing Then
                _checkedDocumentInfos = New List(Of DocumentInfo)
                For Each node As RadTreeNode In RadTreeViewDocument.CheckedNodes
                    If Not node.Attributes("key") Is Nothing Then
                        Dim tmpDocInfo As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(node.Attributes("key")))
                        tmpDocInfo.AddAttribute(BiblosFacade.DOCUMENT_POSITION_ATTRIBUTE, node.Index.ToString())
                        _checkedDocumentInfos.Add(tmpDocInfo)
                    End If
                Next
            End If
            Return _checkedDocumentInfos
        End Get
    End Property

    Public Property HeaderVisible As Boolean
        Get
            Return tblHeader.Visible
        End Get
        Set(ByVal value As Boolean)
            tblHeader.Visible = value
        End Set
    End Property

    Public Property FileInputsCount As Integer
        Get
            Return _fileInputsCount
        End Get
        Set(ByVal value As Integer)
            _fileInputsCount = value
        End Set
    End Property

    Public ReadOnly Property TreeViewControl As RadTreeView
        Get
            Return RadTreeViewDocument
        End Get
    End Property

    Public Property AllowZipDocument As Boolean
        Get
            Return _allowZipDocument
        End Get
        Set(ByVal value As Boolean)
            _allowZipDocument = value
        End Set
    End Property
    Public Property AllowUnlimitFileSize As Boolean
    Public ReadOnly Property MyBaseControl As DocSuite2008BaseControl
        Get
            Return DirectCast(Me, DocSuite2008BaseControl)
        End Get
    End Property

    Public ReadOnly Property WindowHeight As Integer
        Get
            If Not _wndHeight.HasValue Then
                If MultipleDocuments Then
                    _wndHeight = ProtocolEnv.MultipleUploadHeight
                Else
                    _wndHeight = ProtocolEnv.StandardUploadHeight
                End If
            End If
            Return _wndHeight.Value
        End Get
    End Property

    Public ReadOnly Property WindowWidth As Integer
        Get
            If Not _wndWidth.HasValue Then
                If MultipleDocuments Then
                    _wndWidth = ProtocolEnv.MultipleUploadWidth
                Else
                    _wndWidth = ProtocolEnv.StandardUploadWidth
                End If
            End If
            Return _wndWidth.Value
        End Get
    End Property

    Public Property CheckSelectedNode As Boolean
        Get
            Return CType(ViewState("CheckSelectedNode"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            ViewState("CheckSelectedNode") = value
        End Set
    End Property

    Public Property CheckableDocuments As Boolean?
        Get
            If _checkableDocuments Is Nothing Then _checkableDocuments = False
            Return _checkableDocuments
        End Get
        Set(ByVal value As Boolean?)
            _checkableDocuments = value
        End Set
    End Property

    Public Property EnableCheckedDocumentSelection As Boolean
        Get
            Return CType(ViewState("EnableCheckedDocumentSelection"), Boolean)
        End Get
        Set(value As Boolean)
            ViewState("EnableCheckedDocumentSelection") = value
        End Set
    End Property

    Public Property SendSourceDocument As Nullable(Of Boolean)
        Get
            If _sendSourceDocument Is Nothing Then
                _sendSourceDocument = False
                CheckableDocuments = Nothing
                lblSendSourceDocument.Style.Add("display", "none")
            End If
            Return _sendSourceDocument
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _sendSourceDocument = value
            CheckableDocuments = value
            If value.HasValue AndAlso value Then
                lblSendSourceDocument.Style.Remove("display")
            Else
                lblSendSourceDocument.Style.Add("display", "none")
            End If
        End Set
    End Property

    Public Property CollFileDateTime As String
        Get
            Return CType(ViewState("_collFileDateTime"), String)
        End Get
        Set(ByVal value As String)
            ViewState("_collFileDateTime") = value
        End Set
    End Property

    Public Property CollFileName As String
        Get
            Return CType(ViewState("_collFileName"), String)
        End Get
        Set(ByVal value As String)
            ViewState("_collFileName") = value
        End Set
    End Property

    ''' <summary> Abilita l'importazione dei contatti manuali </summary>
    ''' <remarks> xls caricati negli annessi </remarks>
    Public Property EnableImportContactManual As Boolean

    ''' <summary> Imposta quali estensioni sono permesse in fase di caricamento. </summary>
    <TypeConverter(GetType(StringListTypeConverter))>
    Public Property AllowedExtensions As List(Of String)
        Get
            If _allowedExtensions Is Nothing Then
                Dim viewStateValue As String = CType(ViewState(ClientID & "_allowedExtensions"), String)
                If Not String.IsNullOrEmpty(viewStateValue) Then
                    _allowedExtensions = JsonConvert.DeserializeObject(Of List(Of String))(viewStateValue)
                End If
            End If
            Return _allowedExtensions
        End Get
        Set(value As List(Of String))
            Dim serialized As String = JsonConvert.SerializeObject(value)
            ViewState.Item(ClientID & "_allowedExtensions") = serialized
            _allowedExtensions = value
        End Set
    End Property

    ''' <summary> Prefisso alfanumerico aggiunto a tutti i file caricati sullo UserControl. </summary>
    ''' <remarks> Prefix conterrà la parte statica che verrà anteposta e seguita da un numero su 3 cifre, identificativo ordinale del singolo file. </remarks>
    Public Property Prefix As String
        Get
            Return _prefix
        End Get
        Set(value As String)
            _prefix = value
        End Set
    End Property

    Private Property LastPrefixId As Integer
        Get
            Dim lastSessionId As Integer

            If ViewState(_prefixKey) Is Nothing Then
                Try
                    lastSessionId = Request.QueryString.GetValue(Of Integer)(_prefixKey)
                Catch ex As Exception
                    'Se la chiave non è già presente, allora verifico se ci sono documenti da sfruttare
                    If DocumentsCount > 0 Then
                        'Se ci sono documenti calcolo l'ultimo prefisso inserito
                        Dim lastUsedIdString As String = DocumentInfos.Last().Name.Substring(Prefix.Length, PrefixPadding.Length)
                        If Not Integer.TryParse(lastUsedIdString, lastSessionId) Then
                            'Se ho un errore, prendo per buono il numero totale di documenti inseriti
                            lastSessionId = DocumentsCount
                        End If
                    End If
                End Try
            Else
                lastSessionId = Convert.ToInt32(ViewState(_prefixKey))
            End If
            ViewState(_prefixKey) = lastSessionId
            Return lastSessionId
        End Get
        Set(value As Integer)
            ViewState(_prefixKey) = value
        End Set
    End Property

    ''' <summary>
    ''' Calcola la grandezza totale dello UserControl
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property TotalSize As Long
        Get
            Return DocumentInfos.Sum(Function(documentInfo) documentInfo.Size)
        End Get
    End Property

    ''' <summary>
    ''' Definisce se deve essere mostrata la grandezza di ogni allegato
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ShowDocumentsSize As Boolean

    ''' <summary>
    ''' Definisce se deve essere mostrata la grandezza totale degli allegati
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ShowTotalSize As Boolean

    Private ReadOnly Property SessionProtocolNumber As String
        Get
            If Not Session.Item("ProtocolNumberAttach") Is Nothing Then
                Return Session.Item("ProtocolNumberAttach").ToString()
            Else
                Return String.Empty
            End If
        End Get
    End Property

    Private ReadOnly Property SessionResolutionNumber As String
        Get
            If Not Session.Item("ResolutionNumberAttach") Is Nothing Then
                Return Session.Item("ResolutionNumberAttach").ToString()
            Else
                Return String.Empty
            End If
        End Get
    End Property

    Private ReadOnly Property SessionSeriesNumber As String
        Get
            If Not Session.Item("SeriesNumberAttach") Is Nothing Then
                Return Session.Item("SeriesNumberAttach").ToString()
            Else
                Return String.Empty
            End If
        End Get
    End Property

    Private ReadOnly Property SessionUDSNumber As String
        Get
            If Not Session.Item("UDSNumberAttach") Is Nothing Then
                Return Session.Item("UDSNumberAttach").ToString()
            Else
                Return String.Empty
            End If
        End Get
    End Property

    Public Property DocumentDeletable As Boolean
        Get
            Return CType(ViewState("DocumentDeletable"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            ViewState("DocumentDeletable") = value
        End Set
    End Property

    Public Property DocumentsToDelete As IList(Of Guid)
        Get
            If ViewState("DocumentsToDelete") Is Nothing Then
                ViewState("DocumentsToDelete") = New List(Of Guid)
            End If
            Return DirectCast(ViewState("DocumentsToDelete"), IList(Of Guid))
        End Get
        Set(ByVal value As IList(Of Guid))
            ViewState("DocumentsToDelete") = value
        End Set
    End Property

    Public Property CloseAfterSignPdfConversion As Boolean
        Get
            If ViewState("CloseAfterSignPdfConversion") Is Nothing Then
                ViewState("CloseAfterSignPdfConversion") = False
            End If
            Return DirectCast(ViewState("CloseAfterSignPdfConversion"), Boolean)
        End Get
        Set(value As Boolean)
            ViewState("CloseAfterSignPdfConversion") = value
        End Set
    End Property

    Public Property DocumentsDragAndDropEnabled As Boolean

    Public Property DocumentsRenameEnabled As Boolean

    Public Property MinPrivacyLevel As Integer
        Get
            If _minPrivacyLevel.HasValue Then
                Return _minPrivacyLevel.Value
            End If
            Return 0
        End Get
        Set(value As Integer)
            _minPrivacyLevel = value
        End Set
    End Property

    Public Property MaxPrivacyLevel As Integer?
        Get
            If _maxPrivacyLevel.HasValue Then
                Return _maxPrivacyLevel.Value
            End If
            Return Nothing
        End Get
        Set(value As Integer?)
            _maxPrivacyLevel = value
        End Set
    End Property

    Public Shared ReadOnly Property DefaultPrivacyLevel As Integer
        Get
            If _defaultPrivacyLevel.HasValue Then
                Return _defaultPrivacyLevel.Value
            End If
            'TODO: livello privacy utente
            Return 0
        End Get
    End Property

    Public Property PrivacyLevelVisible As Boolean

    Public Property ModifiyPrivacyLevelEnabled As Boolean

    Public Property FromCollaborationPrivacyLevelEnabled As Boolean

    Public Property DelegatedUser As String
        Get
            Return ViewState("DelegatedUser")
        End Get
        Set(value As String)
            ViewState("DelegatedUser") = value
        End Set
    End Property
#End Region

#Region " Events "

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        _prefixKey = String.Format("{0}_LastPrefixId", ID)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        ' SET NO CACHE
        Page.Response.Cache.SetAllowResponseInBrowserHistory(False)
        Page.Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Page.Response.Cache.SetNoStore()
        Page.Response.Cache.SetValidUntilExpires(True)


        InitializeAjax()
        If Not Page.IsPostBack Then
            Initialize()
            RadTreeViewDocument.DataBind()
        End If

        WebUtils.ExpandOnClientNodeAttachEvent(RadTreeViewDocument)
        'Nascondi i controlli per la gestione del documento uploadato
        WebUtils.ObjAttDisplayNone(txtDocumentOK)
        WebUtils.ObjAttDisplayNone(btnAddDocumentFileFDQ)
        WebUtils.ObjAttDisplayNone(btnAddDocumentFile)
        WebUtils.ObjAttDisplayNone(txtFileName)
        WebUtils.ObjAttDisplayNone(txtFileDateTime)

        InitializeButtons()
    End Sub

    Protected Sub UscDocumentAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        'argument(0) = ID controllo che ha effettuato chiamata ajax
        Dim arguments As String() = Split(e.Argument, "|", 4)
        If Not arguments(0).Eq(ClientID) Then
            Exit Sub
        End If

        'argument(1) = azione
        Select Case arguments(1)
            Case "COPY"
                Dim deserialized As List(Of String) = JsonConvert.DeserializeObject(Of List(Of String))(arguments(2))
                Dim type As String = arguments(3)
                Dim initialCode As String = String.Empty

                If FilenameAutomaticRenameEnabled Then
                    Select Case type
                        Case "PROT"
                            initialCode = SessionProtocolNumber
                        Case "RESL"
                            initialCode = SessionResolutionNumber
                        Case "SERIES"
                            initialCode = SessionSeriesNumber
                        Case "UDS"
                            initialCode = SessionUDSNumber
                    End Select
                End If
                For Each item As String In deserialized
                    Dim doc As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(HttpUtility.UrlDecode(item)))
                    If FilenameAutomaticRenameEnabled Then
                        doc.Name = WebHelper.UploadDocumentSetFilename(initialCode, doc.Extension, DocumentInfos.Count + 1)
                        doc.Extension = doc.Extension
                    End If
                    'todo inserire logica di progressivo se e solo se è abilitato il parametro 'FilenameAutomaticRenameEnabled'

                    LoadDocumentInfo(doc, False, True, MultipleDocuments, True, False)
                    RaiseEvent DocumentUploaded(Me, New DocumentEventArgs() With {.Document = doc})

                    AddNextPrefix()
                Next
                UpdateTotalSize()
            Case "UPLOADSCANNERREST"
                Dim documents As List(Of DocumentModel) = JsonConvert.DeserializeObject(Of List(Of DocumentModel))(arguments(2))
                Dim tempDoc As TempFileDocumentInfo
                Dim pathInfo As String
                For Each document As DocumentModel In documents
                    pathInfo = Path.Combine(CommonUtil.GetInstance().TempDirectory.FullName, FileHelper.UniqueFileNameFormat(document.FileName, DocSuiteContext.Current.User.UserName))
                    File.WriteAllBytes(pathInfo, document.ContentStream)
                    tempDoc = New TempFileDocumentInfo(document.FileName, New FileInfo(pathInfo))
                    If tempDoc.Size > 0 OrElse ProtocolEnv.AllowZeroBytesUpload Then
                        LoadDocumentInfo(tempDoc, False, True, MultipleDocuments, True, False)
                        RaiseEvent DocumentUploaded(Me, New DocumentEventArgs() With {.document = tempDoc})

                        AddNextPrefix()
                    End If
                Next
                UpdateTotalSize()

            Case "UPLOAD"
                Dim deserialized As Dictionary(Of String, String) = JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(arguments(2))
                Dim type As String = If(arguments.Length > 3, arguments(3), String.Empty)
                For Each item As KeyValuePair(Of String, String) In deserialized
                    Dim name As String = If(Not String.IsNullOrEmpty(Prefix), String.Format("{0}{1}-{2}", Prefix, GetNextPrefix, item.Value), item.Value).ToString()
                    If FilenameAutomaticRenameEnabled AndAlso "PROT".Eq(type) Then
                        name = WebHelper.UploadDocumentSetFilename(SessionProtocolNumber, Path.GetExtension(name), DocumentInfos.Count + 1)
                    End If

                    Dim doc As New TempFileDocumentInfo(name, New FileInfo(Path.Combine(CommonUtil.GetInstance().AppTempPath, item.Key)))

                    '' Procedo solo se la grandezza maggiore di 0 oppure se sono accettate da Biblos anche
                    '' grandezze nulle [con parametro apposito] --> per evitare errori in fase di memorizzazione
                    If doc.Size > 0 OrElse ProtocolEnv.AllowZeroBytesUpload Then
                        LoadDocumentInfo(doc, False, True, MultipleDocuments, True, False)
                        RaiseEvent DocumentUploaded(Me, New DocumentEventArgs() With {.Document = doc})

                        AddNextPrefix()
                    End If
                Next
                UpdateTotalSize()

            Case "SIGN"
                Dim signed As TempFileDocumentInfo = CType(DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(arguments(2))), TempFileDocumentInfo)
                Dim args As New DocumentSignedEventArgs()
                args.DestinationDocument = signed
                args.SourceDocument = SelectedDocumentInfo

                FileLogger.Info(LoggerName, String.Format("[{0}.SIGN] fileName: {1}", ClientID, signed.Name))

                If Not signed.FileInfo.Exists Then
                    BasePage.AjaxAlert(String.Format("Documento firmato non è valido, reinserire il documento. {0}", ProtocolEnv.DefaultErrorMessage), False)
                    Exit Sub
                End If

                Dim node As RadTreeNode = SelectedNode
                node.Text = signed.Name
                If ShowDocumentsSize Then
                    node.Text &= String.Format(" ({0})", signed.Size.ToByteFormattedString(0))
                End If
                node.ImageUrl = ImagePath.FromDocumentInfo(signed, True) ' ImagePath.FromFile(signed.Name)
                node.Value = signed.FileInfo.Name
                node.Attributes("key") = arguments(2)
                node.Attributes("EncodeFileName") = signed.FileInfo.Name

                FileLogger.Info(LoggerName, String.Format("[{0}.SIGN] RaiseEvent DocumentSigned", ClientID))
                RaiseEvent DocumentSigned(Me, args)
                UpdateTotalSize()

            Case "REMOVE"
                RemoveNode(SelectedNode)
                If (FilenameAutomaticRenameEnabled) Then
                    Dim count As Int16 = 0
                    For Each doc As RadTreeNode In RadTreeViewDocument.Nodes(0).Nodes
                        Dim res As String() = WebHelper.SplitUploadDocumentRename(doc.Text)
                        If (res IsNot Nothing AndAlso res.Length = 4) Then
                            count += 1
                            doc.Text = WebHelper.UploadDocumentSetFilename(WebHelper.UploadDocumentRename(res(0), Int16.Parse(res(1)), Int32.Parse(res(2))), res(3), count)
                        End If
                    Next
                End If

            Case "TEMPLATEDOCUMENT"
                Dim idArchiveChain As Guid = Nothing
                If Guid.TryParse(arguments(2), idArchiveChain) Then
                    Dim docs As ICollection(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(idArchiveChain)
                    For Each doc As BiblosDocumentInfo In docs
                        Dim newName As String = doc.Name.Replace(Path.GetFileNameWithoutExtension(doc.Name), arguments(3))
                        Dim tmpFile As TempFileDocumentInfo = New TempFileDocumentInfo(newName, BiblosFacade.SaveUniqueToTemp(doc))
                        If FilenameAutomaticRenameEnabled Then
                            doc.Name = WebHelper.UploadDocumentSetFilename(SessionProtocolNumber, doc.Extension, DocumentInfos.Count + 1)
                            doc.Extension = doc.Extension
                        End If
                        'todo inserire logica di progressivo se e solo se è abilitato il parametro 'FilenameAutomaticRenameEnabled'

                        LoadDocumentInfo(tmpFile, False, True, MultipleDocuments, True, False)
                        RaiseEvent DocumentUploaded(Me, New DocumentEventArgs() With {.Document = tmpFile})

                        AddNextPrefix()
                    Next
                    UpdateTotalSize()
                End If

            Case "PRIVACYLEVELSET"
                If DocSuiteContext.Current.PrivacyLevelsEnabled AndAlso PrivacyLevelVisible AndAlso arguments(2) IsNot Nothing AndAlso arguments(3) IsNot Nothing Then
                    Dim selectedNodeIndex As Integer = CInt(arguments(2))
                    Dim selectedValue As Integer = CInt(arguments(3))

                    Dim node As RadTreeNode = RadTreeViewDocument.Nodes(0).Nodes(selectedNodeIndex)
                    node.Attributes.Item(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE) = selectedValue.ToString()
                    Dim ddlPrivacyLevels As RadDropDownList = DirectCast(node.FindControl("ddlPrivacyLevels"), RadDropDownList)
                    ddlPrivacyLevels.SelectedValue = selectedValue.ToString()
                End If
            Case "PRIVACY"
                If DocSuiteContext.Current.PrivacyLevelsEnabled AndAlso arguments.Count > 1 Then
                    Dim node As RadTreeNode = SelectedNode
                    node.AddAttribute("PrivacyLevel", arguments(2))
                End If
        End Select

    End Sub

    Protected Sub btnFrontespizio_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnFrontespizio.Click
        RaiseEvent ButtonFrontespizioClick(sender, e)
    End Sub

    Private Sub btnAddDocument_Click(sender As Object, e As ImageClickEventArgs) Handles btnAddDocument.Click
        windowUploadDocument.OnClientClose = JavascriptClosingFunction
        windowUploadDocument.Width = Unit.Pixel(WindowWidth)
        windowUploadDocument.Height = Unit.Pixel(WindowHeight)

        Dim params As New StringBuilder()
        params.AppendFormat("Type={0}", Type)
        If MultipleDocuments Then
            params.Append("&MultiDoc=true")
        End If
        If AllowedExtensions IsNot Nothing AndAlso AllowedExtensions.Count > 0 Then
            params.AppendFormat("&allowedextensions={0}", String.Join(",", AllowedExtensions))
        End If
        params.AppendFormat("&allowzipdoc={0}", AllowZipDocument)
        params.AppendFormat("&allowunlimitfilesize={0}", AllowUnlimitFileSize)
        AjaxManager.ResponseScripts.Add(String.Format(OpenWindowScript, ID, "../UserControl/CommonUploadDocument.aspx", windowUploadDocument.ClientID, params.ToString()))
    End Sub

    Protected Sub btnSelectTemplate_Click(sender As Object, e As ImageClickEventArgs) Handles btnSelectTemplate.Click
        wndSelectTemplate.OnClientClose = JavascriptCloseSelectTemplateFunction
        wndSelectTemplate.Width = Unit.Pixel(WindowWidth)
        wndSelectTemplate.Height = Unit.Pixel(500)

        AjaxManager.ResponseScripts.Add(String.Format(OpenWindowScript, ID, "../UserControl/CommonSelTemplateDocumentRepository.aspx", wndSelectTemplate.ClientID, String.Concat("Type=", Type)))
    End Sub

    Private Sub btnUploadSharepoint_Click(sender As Object, e As ImageClickEventArgs) Handles btnUploadSharepoint.Click
        windowUploadDocument.OnClientClose = JavascriptClosingFunction
        windowUploadDocument.Width = Unit.Pixel(650)
        windowUploadDocument.Height = Unit.Pixel(400)
        windowUploadDocument.Behaviors = WindowBehaviors.Maximize Or WindowBehaviors.Resize Or WindowBehaviors.Close Or WindowBehaviors.Minimize

        Dim params As New StringBuilder()
        params.AppendFormat("Type={0}", Type)

        AjaxManager.ResponseScripts.Add(String.Format(OpenWindowScript, ID, "../UserControl/CommonSharepointDocument.aspx", windowUploadDocument.ClientID, params.ToString()))
    End Sub

    Private Sub btnPrivacyLevel_Click(sender As Object, e As ImageClickEventArgs) Handles btnPrivacyLevel.Click
        If SelectedNode Is Nothing Then
            BasePage.AjaxAlert(String.Concat("Selezionare un documento per procedere con i livelli di ", CommonBasePage.PRIVACY_LABEL, "."))
            Exit Sub
        End If

        windowUploadDocument.OnClientClose = String.Concat(ID, "_ClosePrivacyWindow")
        windowUploadDocument.Width = Unit.Pixel(800)
        windowUploadDocument.Height = Unit.Pixel(400)
        windowUploadDocument.Behaviors = WindowBehaviors.Maximize Or WindowBehaviors.Resize Or WindowBehaviors.Close Or WindowBehaviors.Minimize

        Dim parameters As String = String.Empty
        If Not SelectedNode.Attributes("key") Is Nothing Then
            Dim values As NameValueCollection = HttpUtility.ParseQueryString(SelectedNode.Attributes("key"))
            Dim dic As IDictionary(Of String, String) = values.Cast(Of String).ToDictionary(Function(x) x, Function(y) values(y).Replace("\", "\\"))
            parameters = String.Concat("Doc=", JsonConvert.SerializeObject(dic), "&Caption=", Caption)
            If SelectedNode.Attributes("PrivacyLevel") IsNot Nothing Then
                parameters = String.Concat(parameters, "&Level=", SelectedNode.Attributes("PrivacyLevel"))
            End If
            If SelectedNode.Attributes("MinPrivacy") IsNot Nothing AndAlso Not Convert.ToInt32(SelectedNode.Attributes("MinPrivacy")) = 0 Then
                parameters = String.Concat(parameters, "&Min=", Convert.ToInt32(SelectedNode.Attributes("MinPrivacy")))
            End If
        End If
        AjaxManager.ResponseScripts.Add(String.Format(OpenWindowScript, ID, "../Comm/PrivacyLevels.aspx", windowUploadDocument.ClientID, parameters))
    End Sub

    Private Sub btnCopyProtocol_Click(sender As Object, e As ImageClickEventArgs) Handles btnCopyProtocol.Click
        wndCopyProtocol.Height = Unit.Pixel(ProtocolEnv.ModalHeight)
        wndCopyProtocol.Width = Unit.Pixel(ProtocolEnv.ModalWidth)
        wndCopyProtocol.OnClientClose = If(FilenameAutomaticRenameEnabled, JavascriptCloseProtFunction, JavascriptClosingFunction)
        AjaxManager.ResponseScripts.Add(String.Format(OpenWindowScript, ID, "../Prot/ProtAllega.aspx", wndCopyProtocol.ClientID, "Titolo=Selezione Protocollo&Action=CopyProtocolDocuments"))
    End Sub

    Private Sub btnCopyResl_Click(sender As Object, e As ImageClickEventArgs) Handles btnCopyResl.Click
        wndCopyResl.Width = Unit.Pixel(ProtocolEnv.ModalWidth)
        wndCopyResl.Height = Unit.Pixel(ProtocolEnv.ModalHeight)
        wndCopyResl.OnClientClose = JavascriptCloseReslFunction
        AjaxManager.ResponseScripts.Add(String.Format(OpenWindowScript, ID, "../Resl/ReslAllega.aspx", wndCopyResl.ClientID, "Titolo=Selezione Protocollo&Action=CopyProtocolDocuments&Type=Resl"))
    End Sub

    Private Sub btnCopySeries_Click(sender As Object, e As ImageClickEventArgs) Handles btnCopySeries.Click
        wndCopySeries.Height = Unit.Pixel(ProtocolEnv.ModalHeight)
        wndCopySeries.Width = Unit.Pixel(ProtocolEnv.ModalWidth)
        wndCopySeries.OnClientClose = JavascriptCloseSeriesFunction
        wndCopySeries.Title = "Copia da " & ProtocolEnv.DocumentSeriesName
        AjaxManager.ResponseScripts.Add(String.Format(OpenWindowScript, ID, "../Series/AttachDocuments.aspx", wndCopySeries.ClientID, "Titolo=Selezione Protocollo&Action=CopyProtocolDocuments&Type=Series"))
    End Sub

    Private Sub btnCopyUDS_Click(sender As Object, e As ImageClickEventArgs) Handles btnCopyUDS.Click
        wndCopyUDS.Height = Unit.Pixel(ProtocolEnv.ModalHeight)
        wndCopyUDS.Width = Unit.Pixel(ProtocolEnv.ModalWidth)
        wndCopyUDS.OnClientClose = JavascriptCloseUDSFunction
        AjaxManager.ResponseScripts.Add(String.Format(OpenWindowScript, ID, "../UDS/AttachDocuments.aspx", wndCopyUDS.ClientID, "Titolo=Selezione Protocollo&Action=CopyProtocolDocuments&Type=UDS"))
    End Sub

    Private Sub btnSignDocument_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs)
        If SelectedDocumentInfo Is Nothing Then
            BasePage.AjaxAlert("Selezionare un documento per procedere con la firma.")
            Exit Sub
        End If

        Dim args As New DocumentBeforeSignEventArgs()
        args.SourceDocument = SelectedDocumentInfo
        RaiseEvent DocumentBeforeSign(Me, args)

        If args.Cancel Then
            Exit Sub
        End If

        signWindow.OnClientClose = ID & "_CloseSignWindow"
        Dim parameters As String = SelectedDocumentInfo.ToQueryString().AsEncodedQueryString()
        If CloseAfterSignPdfConversion Then
            parameters = String.Concat(parameters, "&CloseAfterPdfConversion=True")
        End If
        If Not String.IsNullOrEmpty(DelegatedUser) Then
            parameters = $"{parameters}&DelegatedUser={HttpUtility.UrlEncode(DelegatedUser)}"
        End If
        AjaxManager.ResponseScripts.Add(String.Format(If(ProtocolEnv.ResizeSignWindowEnabled, OpenWindowSignScript, OpenWindowScript), ID, "../Comm/SingleSign.aspx", signWindow.ClientID, parameters))
    End Sub

    ''' <summary> Apertura finestra Scanner. </summary>
    Private Sub btnAddDocumentScanner_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs)
        Dim params As StringBuilder = New StringBuilder()
        windowScannerDocument.OnClientClose = JavascriptClosingFunction
        Dim scannerLightPath As String = SCANNER_LIGHT_PATH
        If ProtocolEnv.ScannerLightRestEnabled Then
            scannerLightPath = SCANNER_LIGHT_PATH_REST
            params.AppendFormat("&multipleEnabled={0}", MultipleDocuments)
            windowScannerDocument.Width = Unit.Pixel(700)
            windowScannerDocument.Height = Unit.Pixel(480)
            windowScannerDocument.OnClientClose = JavascriptClosingScannerRestFunction
        End If
        AjaxManager.ResponseScripts.Add(String.Format(OpenWindowScannerScript, ID, scannerLightPath, windowScannerDocument.ClientID, params.ToString(), ""))
    End Sub

    ''' <summary> Apertura finesta anteprima. </summary>
    Private Sub btnPreviewDoc_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs)

        Dim node As RadTreeNode = SelectedNode
        If node Is Nothing Then
            BasePage.AjaxAlert("Selezionare un documento per procedere con l'anteprima.")
            Exit Sub
        End If

        If node.Attributes("Previewable") IsNot Nothing AndAlso node.Attributes("Previewable").Eq(False.ToString()) Then
            BasePage.AjaxAlert("Non si possiedono diritti per visualizzare l'anteprima del documento selezionato.")
            Exit Sub
        End If

        Dim url As String = String.Empty
        If Not node.Attributes("key") Is Nothing Then
            url = ResolveUrl("~/Viewers/DocumentInfoViewer.aspx?" & CommonShared.AppendSecurityCheck(node.Attributes("key")))
        ElseIf Not node.Attributes("EncodeFileName") Is Nothing Then
            url = "../Viewers/TempFileViewer.aspx?DownloadFile=" & node.Attributes("EncodeFileName")
        End If

        If String.IsNullOrEmpty(url) Then
            BasePage.AjaxAlert("Il documento selezionato non è disponibile per l'anteprima.")
            Exit Sub
        End If

        windowPreviewDocument.Height = Unit.Pixel(DocSuiteContext.Current.ProtocolEnv.DocumentPreviewHeight)
        windowPreviewDocument.Width = Unit.Pixel(DocSuiteContext.Current.ProtocolEnv.DocumentPreviewWidth)

        AjaxManager.ResponseScripts.Add(String.Format(OpenWindowScript, ID, url, windowPreviewDocument.ClientID, ""))
    End Sub

    ''' <summary> Apertura finestra importa da cartella condivisa. </summary>
    Protected Sub btnImportSharedFolder_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs)
        If SharedFolder Is Nothing Then
            BasePage.AjaxAlert("Cartella Condivisa non configurata")
            Exit Sub
        End If

        Dim impersonator As Impersonator = Nothing
        Try
            impersonator = CommonAD.ImpersonateSuperUser()
            Dim files As List(Of FileInfo) = SharedFolder.GetFiles().Where(Function(file) ((file.Attributes And FileAttributes.Hidden) <> FileAttributes.Hidden) AndAlso (file.Attributes And FileAttributes.System) <> FileAttributes.System).ToList()
            If files.Count = 0 Then
                BasePage.AjaxAlert("Nessun file nella Cartella Condivisa")
                Exit Sub
            End If

            If files.Count() = 1 Then
                ' Selezione automatica documento singolo
                Dim sharedFile As FileInfo = files(0)
                CommonShared.SelectedSharedFile = sharedFile

                Dim uniquename As String = FileHelper.UniqueFileNameFormat(sharedFile.Name, DocSuiteContext.Current.User.UserName)
                Dim doc As New TempFileDocumentInfo(sharedFile.CopyTo(CommonInstance.AppTempPath & uniquename))
                doc.Name = sharedFile.Name

                LoadDocumentInfo(doc, False, True, MultipleDocuments, True, True)
                RaiseEvent DocumentUploaded(Me, New DocumentEventArgs() With {.Document = doc})

                Exit Sub
            End If
        Catch ex As Exception
            FileLogger.Error(LoggerName, "Errore in selezione file su Cartella Condivisa", ex)
            BasePage.AjaxAlert("Errore selezione file, contattare l'assistenza.")
            Exit Sub
        Finally
            If impersonator IsNot Nothing Then
                impersonator.ImpersonationUndo()
            End If

        End Try

        ' Maschera di selezione documenti
        windowSharedFolder.Width = Unit.Pixel(700)
        windowSharedFolder.Height = Unit.Pixel(500)
        windowSharedFolder.OnClientClose = JavascriptClosingFunction
        AjaxManager.ResponseScripts.Add(String.Format(OpenWindowScript, ID, "../UserControl/CommonSelSharedFolder.aspx", windowSharedFolder.ClientID, "Type=" & BasePage.Type))
    End Sub

    Private Sub btnImportContactManual_Click(sender As Object, e As ImageClickEventArgs) Handles btnImportContactManual.Click
        windowUploadDocument.Width = Unit.Pixel(WindowWidth)
        windowUploadDocument.Height = Unit.Pixel(WindowHeight)
        windowUploadDocument.OnClientClose = JavascriptClosingFunction

        Dim params As New StringBuilder("Excel=true")
        params.AppendFormat("&Type={0}", Type)
        If MultipleDocuments Then
            params.Append("&MultiDoc=true")
        End If
        params.AppendFormat("&allowedextensions={0}", Server.UrlDecode(".xls,.xlsx"))
        btnImportContactManual.OnClientClick = String.Format(OpenWindowScript, ID, "../UserControl/CommonUploadDocument.aspx", windowUploadDocument.ClientID, params)
    End Sub

    Private Sub RadTreeViewDocument_NodeClick(sender As Object, e As RadTreeNodeEventArgs) Handles RadTreeViewDocument.NodeClick
        If SelectedDocumentInfo IsNot Nothing AndAlso DocumentSelectedEvent IsNot Nothing Then
            RaiseEvent DocumentSelected(Me, New DocumentEventArgs() With {.Document = SelectedDocumentInfo})
        End If
    End Sub

    Protected Sub RadTreeViewDocument_NodeEdit(ByVal sender As Object, ByVal e As RadTreeNodeEditEventArgs) Handles RadTreeViewDocument.NodeEdit
        Dim nodeEdited As RadTreeNode = e.Node
        Dim newText As String = e.Text
        If (String.IsNullOrEmpty(newText) OrElse Not FileHelper.IsValidFileName(newText) OrElse Not Path.HasExtension(newText)) Then
            BasePage.AjaxAlert("E' richiesto un nome valido per il documento")
            AjaxManager.ResponseScripts.Add(String.Format(TREENODE_EDITED_END_SCRIPT, ID))
            Exit Sub
        End If

        If Not Path.GetExtension(nodeEdited.Text).Eq(Path.GetExtension(newText)) Then
            BasePage.AjaxAlert("Non è possibile modificare il tipo del documento")
            AjaxManager.ResponseScripts.Add(String.Format(TREENODE_EDITED_END_SCRIPT, ID))
            Exit Sub
        End If

        nodeEdited.Text = newText
        Dim selectedDocInfo As DocumentInfo = SelectedDocumentInfo
        selectedDocInfo.Name = newText
        SelectedNode.AddAttribute("key", selectedDocInfo.ToQueryString().AsEncodedQueryString())
        AjaxManager.ResponseScripts.Add(String.Format(TREENODE_EDITED_END_SCRIPT, ID))
    End Sub

    Protected Sub RadTreeViewDocument_OnNodeDataBound(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs) Handles RadTreeViewDocument.NodeCreated
        Dim node As RadTreeNode = e.Node
        Dim ddlPrivacyLevels As RadDropDownList = DirectCast(node.FindControl("ddlPrivacyLevels"), RadDropDownList)
        Dim lblPrivacy As Label = DirectCast(node.FindControl("lblPrivacy"), Label)
        If ddlPrivacyLevels IsNot Nothing Then
            node.Width = New Unit("90%")
            ddlPrivacyLevels.SetDisplay(False)
            lblPrivacy.SetDisplay(False)
            If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                If Not MinPrivacyLevel = 0 Then
                    node.AddAttribute("MinPrivacy", MinPrivacyLevel.ToString())
                End If
                If PrivacyLevelVisible AndAlso
                (ModifiyPrivacyLevelEnabled OrElse ButtonFileEnabled OrElse ButtonScannerEnabled OrElse ButtonLibrarySharepointEnabled OrElse ButtonCopyProtocol.Visible OrElse ButtonCopyResl.Visible OrElse ButtonCopySeries.Visible OrElse ButtonCopyUDS.Visible OrElse btnImportContactManual.Visible OrElse ButtonSelectTemplateEnabled) AndAlso Not node.Value.Equals(FakeNodeValue) Then
                    ddlPrivacyLevels.SetDisplay(True)
                    lblPrivacy.SetDisplay(True)
                    lblPrivacy.Text = String.Concat(" - ", CommonBasePage.PRIVACY_LABEL)
                    ddlPrivacyLevels.OnClientSelectedIndexChanged = String.Concat(ID, "_ddlPrivacyLevels_SelectedIndexChanged")
                    ddlPrivacyLevels.Attributes.Add("nodeIndex", node.Index.ToString())

                    ddlPrivacyLevels.DataSource = PrivacyLevelFacade.GetAllowedPrivacyLevels(MinPrivacyLevel, MaxPrivacyLevel).OrderBy(Function(s) s.Level)
                    ddlPrivacyLevels.DataTextField = "Description"
                    ddlPrivacyLevels.DataValueField = "Level"

                    ddlPrivacyLevels.SelectedValue = node.Attributes.Item(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE)
                    ddlPrivacyLevels.DataBind()
                End If
            End If
        End If
    End Sub
#End Region

#Region " Methods "

    Public Sub Initialize()
        ButtonLibrarySharepointEnabled = ProtocolEnv.UploadSharepointDocumentLibrary AndAlso (ButtonFileEnabled OrElse ButtonScannerEnabled)
        RadTreeViewDocument.CheckBoxes = CheckableDocuments.Value OrElse EnableCheckedDocumentSelection
        If RadTreeViewDocument.CheckBoxes Then
            RadTreeViewDocument.Nodes(0).Checkable = False
        End If
        btnCopySeries.ToolTip = String.Concat("Copia da ", ProtocolEnv.DocumentSeriesName)

        If DocumentDeletable Then
            btnRemoveDocument.OnClientClick = String.Concat(Me.ID, "_ConfirmRemoveDocument(); return false;")
        Else
            btnRemoveDocument.OnClientClick = String.Concat(Me.ID, "_ConfirmRemoveDocumentCallback(true); return false;")
        End If

        If DocumentsRenameEnabled Then
            btnEditName.Visible = True
            btnEditName.OnClientClick = String.Format(EDIT_DOCUMENT_SCRIPT, ID)
            RadTreeViewDocument.OnClientNodeEdited = String.Format(TREENODE_EDITED_SCRIPT, ID)
        End If

        If DocumentsDragAndDropEnabled Then
            RadTreeViewDocument.EnableDragAndDrop = True
            RadTreeViewDocument.OnClientNodeDropping = String.Format(TREEVIEW_NODE_DROPPING_FUNCTION, ID)
            RadTreeViewDocument.EnableDragAndDropBetweenNodes = True
        End If

        If ButtonPrivacyLevelVisible Then
            btnPrivacyLevel.ToolTip = String.Concat("Assegna livelli di ", CommonBasePage.PRIVACY_LABEL)
        End If
    End Sub

    Private Function isLocked(ByVal node As RadTreeNode) As Boolean
        Dim locked As String = node.Attributes(NodeAttributeLocked)
        If Not String.IsNullOrEmpty(locked) AndAlso locked.Eq("True") Then
            Return True
        End If
        Return False
    End Function

    Private Shared Sub SetLocked(ByRef node As RadTreeNode, value As Boolean)
        If value Then
            node.Attributes(NodeAttributeLocked) = "True"
            Return
        End If
        node.Attributes(NodeAttributeLocked) = "False"
    End Sub

    Public Sub LoadDocumentInfo(doc As DocumentInfo)
        LoadDocumentInfo(doc, False)
    End Sub

    Public Sub LoadDocumentInfo(doc As DocumentInfo, isNew As Boolean)
        LoadDocumentInfo(doc, False, False, False, isNew, False)
    End Sub

    Public Sub LoadDocumentInfo(doc As DocumentInfo, signature As Boolean, deletable As Boolean, append As Boolean, isNew As Boolean)
        LoadDocumentInfo(doc, signature, deletable, append, isNew, False)
    End Sub

    Public Sub LoadDocumentInfo(doc As DocumentInfo, signature As Boolean, deletable As Boolean, append As Boolean, isNew As Boolean, updateSizeCount As Boolean)
        Dim documents As New List(Of DocumentInfo)({doc})
        LoadDocumentInfo(documents, signature, deletable, append, isNew, updateSizeCount)
    End Sub

    Public Sub LoadDocumentInfo(docs As IList(Of DocumentInfo))
        LoadDocumentInfo(docs, False, False, False, False, False)
    End Sub

    Public Sub LoadDocumentInfo(docs As IList(Of DocumentInfo), signature As Boolean, deletable As Boolean, append As Boolean, isNew As Boolean)
        LoadDocumentInfo(docs, signature, deletable, append, isNew, False)
    End Sub

    Public Sub LoadDocumentInfo(docs As IList(Of DocumentInfo), signature As Boolean, deletable As Boolean, append As Boolean, isNew As Boolean, updateSizeCount As Boolean, Optional previewable As Boolean = True)
        If Not append Then
            ClearNodes()
        End If

        For Each doc As DocumentInfo In docs
            '' metto da parte il doc originale per avere cache
            DocumentInfos.Add(doc)

            Dim node As RadTreeNode = CreateNodeFromDocumentInfo(doc, signature, deletable, isNew, ShowDocumentsSize, previewable)
            RadTreeViewDocument.Nodes(0).Nodes.Add(node)
        Next
        If HasDocuments Then
            ' se non devo validare imposto il campo come validato
            AjaxManager.ResponseScripts.Add(ID & "_SetTextValidator();")
        End If

        If updateSizeCount Then
            UpdateTotalSize()
        End If
        RadTreeViewDocument.DataBind()
    End Sub

    ''' <summary> Crea un nodo compatibile con i metodi del controllo </summary>
    ''' <remarks> Metodo per ora necessario per gestire gestioni custom dei nodi. Eliminarlo o renderlo private. </remarks>
    Public Shared Function CreateNodeFromDocumentInfo(doc As DocumentInfo, signature As Boolean, deletable As Boolean, isNew As Boolean, showSize As Boolean, Optional previewable As Boolean = True) As RadTreeNode
        Dim node As New RadTreeNode()
        If signature AndAlso Not String.IsNullOrEmpty(doc.Signature) Then
            node.Text = doc.Signature
        Else
            node.Text = doc.Name
        End If

        '' Carico subito la size (se richiesta) in modo tale che venga valorizzata e quindi inserita in querystring
        If showSize Then
            node.Text &= String.Format(" ({0})", doc.Size.ToByteFormattedString(0))
        End If

        node.ImageUrl = ImagePath.FromDocumentInfo(doc, True) ' ImagePath.FromFile(doc.Name)

        Dim items As NameValueCollection = doc.ToQueryString()
        Dim key As String = items.AsEncodedQueryString()
        node.Attributes.Add("key", key)
        If (isNew) Then
            node.Attributes.Add("Added", "True")
        Else
            node.Attributes.Add("Added", "False")
        End If
        node.Attributes.Add("Previewable", previewable.ToString())
        Dim level As Integer = If(DocSuiteContext.Current.PrivacyLevelsEnabled, DefaultPrivacyLevel, 0)
        If DocSuiteContext.Current.PrivacyLevelsEnabled AndAlso doc.Attributes IsNot Nothing AndAlso doc.Attributes.Any(Function(s) s.Key.Equals(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE)) AndAlso Not String.IsNullOrEmpty(doc.Attributes.Item(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE)) Then
            level = CInt(doc.Attributes.Item(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE))
        End If
        node.Attributes.Add(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE, level.ToString())
        node.Expanded = True
        If Not deletable Then
            SetLocked(node, True)
        End If
        Return node
    End Function

    Public Sub RemoveDocumentInfo(doc As DocumentInfo)
        '' Rimuovo il documento prima dalla lista dei documenti
        DocumentInfos.Remove(doc)
        '' Rimuovo il nodo dalla struttura
        RemoveNode(GetNodeByDocumentInfo(doc))
    End Sub

    Public Shared Function GetDocumentInfoByNode(node As RadTreeNode) As DocumentInfo
        If node Is Nothing OrElse node.Attributes Is Nothing OrElse String.IsNullOrEmpty(node.Attributes("key")) Then
            Return Nothing
        End If

        Return DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(node.Attributes("key")))
    End Function

    Public Function GetNodeByDocumentInfo(document As DocumentInfo) As RadTreeNode
        Dim expected As String = document.ToQueryString().AsEncodedQueryString()
        Return RadTreeViewDocument.FindNodeByAttribute("key", expected)
    End Function

    Public Sub SetDocumentInfoName(document As DocumentInfo, name As String)
        Dim var As RadTreeNode = GetNodeByDocumentInfo(document)
        Dim docName As String = DirectCast(var.FindControl("lbl"), Label).Text
        If Not String.IsNullOrEmpty(docName) Then
            RadTreeViewDocument.FindNodeByText(docName).Text += String.Format(" {0}", name)
        End If

        RadTreeViewDocument.DataBind()
    End Sub

    Private Sub InitializeAjax()
        If Visible Then
            AddHandler AjaxManager.AjaxRequest, AddressOf UscDocumentAjaxRequest
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, RadTreeViewDocument)
            AjaxManager.AjaxSettings.AddAjaxSetting(RadTreeViewDocument, RadTreeViewDocument)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, txtDocumentOK)

            AjaxManager.AjaxSettings.AddAjaxSetting(btnAddDocument, windowUploadDocument)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnUploadSharepoint, windowUploadDocument)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnCopyProtocol, wndCopyProtocol)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnCopySeries, wndCopySeries)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnCopyResl, wndCopyResl)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnImportContactManual, windowUploadDocument)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnSelectTemplate, wndSelectTemplate)

            If ButtonPreviewEnabled Then
                AddHandler btnPreviewDoc.Click, AddressOf btnPreviewDoc_Click
                AjaxManager.AjaxSettings.AddAjaxSetting(btnPreviewDoc, windowPreviewDocument)
            End If

            If SignButtonEnabled Then
                AddHandler btnSignDocument.Click, AddressOf btnSignDocument_Click
                AjaxManager.AjaxSettings.AddAjaxSetting(btnSignDocument, signWindow)
            End If

            If ButtonScannerEnabled Then
                AddHandler btnAddDocumentScanner.Click, AddressOf btnAddDocumentScanner_Click
                AjaxManager.AjaxSettings.AddAjaxSetting(btnAddDocumentScanner, windowScannerDocument)
            End If

            If ButtonSharedFolederEnabled Then
                AjaxManager.AjaxSettings.AddAjaxSetting(btnImportSharedFolder, windowSharedFolder)
            End If
        End If
    End Sub

    Private Sub InitializeButtons()
        If ProtocolEnv.ScannerLightRestEnabled Then
            btnAddDocumentScanner.ImageUrl = "~/App_Themes/DocSuite2008/imgset16/scanner.png"
        End If
        ' Upload da Shared Folder
        If SharedFolder Is Nothing Then
            ButtonSharedFolederEnabled = False
        Else
            ' forse qui c'è qualcosa che non va...
            If SharedFolder.Exists Then

                Dim impersonator As Impersonator = CommonAD.ImpersonateSuperUser()
                Dim sharedFiles As FileInfo() = SharedFolder.GetFiles()
                impersonator.ImpersonationUndo()

                Select Case sharedFiles.Length
                    Case 0
                        ButtonSharedFolederEnabled = False

                    Case Else
                        ButtonSharedFolederEnabled = True
                        AddHandler btnImportSharedFolder.Click, AddressOf btnImportSharedFolder_Click

                End Select
            End If
        End If

        btnImportContactManual.Visible = EnableImportContactManual
    End Sub

    ''' <summary> Imposta i documenti come nodi aggiunti. </summary>
    ''' <param name="alreadyValidated">Indica se deve validare il controllo.</param>
    Public Sub InitializeNodesAsAdded(alreadyValidated As Boolean)
        For Each node As RadTreeNode In RadTreeViewDocument.Nodes(0).Nodes
            node.Attributes.Add("Added", "True")
        Next

        If Not alreadyValidated Then
            ' se non devo validare imposto il campo come validato
            AjaxManager.ResponseScripts.Add(ID & "_SetTextValidator();")
        End If
    End Sub

    Public Sub LoadBiblosDocuments(archive As String, idchain As Integer, signature As Boolean, blockDelete As Boolean)
        ClearNodes()
        Dim docs As List(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(archive, idchain)

        Dim index As Integer = 0
        For Each doc As BiblosDocumentInfo In docs
            Dim node As New RadTreeNode(doc.Name)
            node.ImageUrl = ImagePath.FromDocumentInfo(doc, True)
            If SendSourceDocument Then
                node.Value = String.Format("{0}|{1}|{2}", doc.ArchiveName, doc.BiblosChainId, index)
                node.Attributes.Add("Filename", doc.Name)
            End If

            If signature AndAlso Not String.IsNullOrEmpty(doc.Signature) Then
                node.Text = doc.Signature
            End If

            node.Expanded = True
            If blockDelete Then
                SetLocked(node, True)
            End If

            RadTreeViewDocument.Nodes(0).Nodes.Add(node)
            index += 1
        Next
    End Sub

    Public Sub ClearNodes()
        RadTreeViewDocument.Nodes(0).Nodes.Clear()
    End Sub

    Public Function GetNodeValue(ByVal index As Integer) As String
        Dim sRet As String = String.Empty

        If RadTreeViewDocument.Nodes.Count > 0 AndAlso DocumentsCount > index Then
            sRet = RadTreeViewDocument.Nodes(0).Nodes(index).Value
        End If

        Return sRet
    End Function

    Public Function GetNode(ByVal index As Integer) As RadTreeNode
        Dim sRet As RadTreeNode = Nothing

        If RadTreeViewDocument.Nodes.Count > 0 AndAlso DocumentsCount > index Then
            sRet = RadTreeViewDocument.Nodes(0).Nodes(index)
        End If

        Return sRet
    End Function

    <Obsolete("Questo grado di controllo sui nodi non dovrebbe essere usato.")>
    Public Function AddNode(ByVal text As String, ByVal imageUrl As String, ByVal value As String, ByVal clear As Boolean, ByVal bold As Boolean) As RadTreeNode

        Dim node As New RadTreeNode
        node.Text = text
        node.ImageUrl = imageUrl
        node.Value = value
        node.Selected = True

        If bold Then
            node.Style.Add("font-weight", "bold")
        End If

        Dim father As RadTreeNode = RadTreeViewDocument.Nodes(0)
        If clear Then
            father.Nodes.Clear()
        End If
        father.Nodes.Add(node)
        father.Expanded = True

        ' Il controllo risulta già validato
        AjaxManager.ResponseScripts.Add(ID & "_SetTextValidator();")

        Return node
    End Function

    Private Sub Validate()
        If DocumentsCount = 0 Then
            AjaxManager.ResponseScripts.Add(ID & "_ClearTextValidator();")
        End If
    End Sub

    Public Function GetNextPrefix() As String
        If Not String.IsNullOrEmpty(Prefix) Then
            Return Right(PrefixPadding & LastPrefixId + 1, PrefixPadding.Length)
        End If
        Return String.Empty
    End Function

    Public Sub AddNextPrefix()
        If Not String.IsNullOrEmpty(Prefix) Then
            LastPrefixId += 1
        End If
    End Sub

    Private Sub UpdateTotalSize()
        '' Aggiorno il label se richiesto
        If ShowTotalSize Then
            Dim size As Long = TotalSize
            If size > 0 Then
                '' Aggiorno la root dei nodi con la grandezza aggiornata
                TreeViewCaption = String.Format("{0} ({1})", _originalTreeViewCaption, size.ToByteFormattedString(0))
            Else
                '' Se la grandezza è 0 ripristino la scritta originale
                TreeViewCaption = _originalTreeViewCaption
            End If
        End If
    End Sub

    Private Sub RemoveNode(node As RadTreeNode, Optional [raiseEvent] As Boolean = True)
        If node IsNot Nothing Then
            If isLocked(node) Then
                If DocumentDeletable Then
                    Dim doc As BiblosDocumentInfo = TryCast(GetDocumentInfoByNode(node), BiblosDocumentInfo)
                    If doc IsNot Nothing Then
                        DocumentsToDelete.Add(doc.DocumentId)
                    End If
                    node.Remove()
                Else
                    BasePage.AjaxAlert("Non è possibile eliminare un documento già memorizzato", False)
                    ''Ritorno altrimenti poi viene rimosso il documento dal versioning
                    Return
                End If
            Else
                If Not String.IsNullOrEmpty(node.Value) Then
                    File.Delete(CommonInstance.AppTempPath & node.Value)
                End If
                node.Remove()
            End If
        End If

        'Gestione prefisso: Se non ci sono documenti memorizzo 0 come valore iniziale in modo che il count riparta nuovamente
        If Not String.IsNullOrEmpty(Prefix) AndAlso DocumentsAddedCount = 0 Then
            ViewState.Remove(_prefixKey)
        End If

        RadTreeViewDocument.Nodes(0).Expanded = True
        Validate()
        UpdateTotalSize()
        If node IsNot Nothing AndAlso DocumentRemovedEvent IsNot Nothing AndAlso [raiseEvent] Then
            RaiseEvent DocumentRemoved(Me, New DocumentEventArgs() With {.Document = GetDocumentInfoByNode(node)})
        End If
    End Sub

    Public Sub LoadDocumentInfoByIndex(doc As DocumentInfo, signature As Boolean, deletable As Boolean, append As Boolean, isNew As Boolean, updateSizeCount As Boolean, index As Integer)

        '' metto da parte il doc originale per avere cache
        DocumentInfos.Add(doc)

        Dim node As RadTreeNode = CreateNodeFromDocumentInfo(doc, signature, deletable, isNew, ShowDocumentsSize)
        If append Then
            RadTreeViewDocument.Nodes(0).Nodes.Add(node)
        Else
            RadTreeViewDocument.Nodes(0).Nodes.Insert(index, node)
        End If

        If HasDocuments Then
            ' se non devo validare imposto il campo come validato
            AjaxManager.ResponseScripts.Add(ID & "_SetTextValidator();")
        End If

        If updateSizeCount Then
            UpdateTotalSize()
        End If
        RadTreeViewDocument.DataBind()
    End Sub

    Public Sub RefreshDdlPrivacyLevel(visibility As Boolean)
        PrivacyLevelVisible = visibility AndAlso (ModifiyPrivacyLevelEnabled OrElse ButtonFileEnabled OrElse ButtonScannerEnabled OrElse ButtonLibrarySharepointEnabled OrElse ButtonCopyProtocol.Visible OrElse ButtonCopyResl.Visible OrElse ButtonCopySeries.Visible OrElse ButtonCopyUDS.Visible OrElse btnImportContactManual.Visible OrElse ButtonSelectTemplateEnabled OrElse FromCollaborationPrivacyLevelEnabled)
        For Each node As RadTreeNode In RadTreeViewDocument.Nodes(0).Nodes
            Dim ddlPrivacyLevels As RadDropDownList = DirectCast(node.FindControl("ddlPrivacyLevels"), RadDropDownList)
            Dim lblPrivacy As Label = DirectCast(node.FindControl("lblPrivacy"), Label)
            If ddlPrivacyLevels IsNot Nothing Then
                node.Width = New Unit("90%")
                ddlPrivacyLevels.SetDisplay(PrivacyLevelVisible AndAlso Not node.Value.Equals(FakeNodeValue))
                lblPrivacy.SetDisplay(PrivacyLevelVisible AndAlso Not node.Value.Equals(FakeNodeValue))
                If DocSuiteContext.Current.PrivacyLevelsEnabled AndAlso PrivacyLevelVisible AndAlso Not node.Value.Equals(FakeNodeValue) Then
                    ddlPrivacyLevels.OnClientSelectedIndexChanged = String.Concat(ID, "_ddlPrivacyLevels_SelectedIndexChanged")
                    ddlPrivacyLevels.Attributes.Add("nodeIndex", node.Index.ToString())
                    ddlPrivacyLevels.DataSource = CurrentPrivacyLevels.Where(Function(s) s.Level >= MinPrivacyLevel).OrderBy(Function(s) s.Level)
                    ddlPrivacyLevels.DataTextField = "Description"
                    ddlPrivacyLevels.DataValueField = "Level"
                    lblPrivacy.Text = String.Concat(" - ", CommonBasePage.PRIVACY_LABEL)
                    ddlPrivacyLevels.SelectedValue = node.Attributes.Item(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE)
                    ddlPrivacyLevels.DataBind()
                End If
            End If
        Next
        RadTreeViewDocument.DataBind()
    End Sub

    Public Sub RefreshPrivacyLevelAttributes(minValue As Integer, forceValue As Integer?)
        If RadTreeViewDocument.Nodes(0).Nodes IsNot Nothing AndAlso (Not minValue = 0 OrElse forceValue.HasValue) Then
            For Each node As RadTreeNode In RadTreeViewDocument.Nodes(0).Nodes
                If Not minValue = 0 Then
                    node.AddAttribute("MinPrivacy", minValue.ToString())
                End If
                If forceValue.HasValue Then
                    node.AddAttribute("PrivacyLevel", forceValue.Value.ToString())
                End If
            Next
        End If
        RadTreeViewDocument.DataBind()
    End Sub
#End Region

End Class

