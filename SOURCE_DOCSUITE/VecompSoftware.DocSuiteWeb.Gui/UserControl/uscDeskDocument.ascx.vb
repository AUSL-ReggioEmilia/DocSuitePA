Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.BusinessRule.Rules.Desks
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Desks
Imports VecompSoftware.DocSuiteWeb.DTO.Collaborations
Imports VecompSoftware.DocSuiteWeb.DTO.Desks
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Desks
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging

Public Class uscDeskDocument
    Inherits DocSuite2008BaseControl

    Public Event DocumentUploaded(ByVal sender As Object, ByVal e As DocumentEventArgs)
    Public Event DocumentCheckIn(ByVal sender As Object, ByVal e As DeskDocumentResultEventArgs)
    Public Event DocumentCheckOut(ByVal sender As Object, ByVal e As DeskDocumentResultEventArgs)
    Public Event DocumentUndoCheckOut(ByVal sender As Object, ByVal e As DeskDocumentResultEventArgs)
    Public Event DocumentSigned(sender As Object, e As DeskDocumentResultEventArgs)
    Public Event DocumentBeforeSingleSign(ByVal sender As Object, ByVal e As DocumentBeforeSignEventArgs)
    Public Event DocumentReloaded(ByVal sender As Object, ByVal e As DocumentEventArgs)
#Region "Fields"
    Private _wndHeight As Integer?
    Private _wndWidth As Integer?
    Private Const COMMON_UPLOAD_DOCUMENT_PATH As String = "../UserControl/CommonUploadDocument.aspx"
    Private Const STORYBOARD_PATH_FORMAT As String = "~/Desks/DeskViewStoryBoard.aspx?Type=Desk&DeskId={0}&DeskDocumentId={1}"
    Private Const TEMPLATE_DOCUMENT_PATH As String = "../UserControl/CommonSelTemplateDocumentRepository.aspx"
    Private Const OPEN_WINDOW_SCRIPT As String = "return {0}_OpenWindow('{1}', '{2}', '{3}');"
    Private Const OPEN_WINDOW_CONTROL As String = "return {0}_OpenWindowBase('{1}');"
    Private Const OPEN_WINDOW_SIGN_SCRIPT As String = "return {0}_OpenWindowSign('{1}', '{2}', '{3}');"
    Private Const COLUMN_LAST_COMMENT As String = "LastComment"
    Private Const COLUMN_STORY_BOARD As String = "ViewStoryBoard"
    Private Const COLUMN_CHECKINOUT As String = "CheckInOut"
    Private Const COLUMN_DELETE_DOCUMENT As String = "DeleteDocument"
    Private Const COLUMN_SELECT_DOCUMENT As String = "DocumentSelect"
    Private Const BIBLOS_KEY As String = "BiblosKey"
    Private Const NEW_DOCUMENT_ATTRIBUTES As String = "ToPersistDocumentInfos"
    Public Const TYPE_INSERT_NAME As String = "Insert"
    Public Const TYPE_MODIFY_NAME As String = "Modify"


    Private _deskFacade As DeskFacade
    Private _allowedExtensions As List(Of String)
    Private Const OPEN_WINDOW_CHECKIN_SCRIPT As String = "return {0}_OpenCheckInWindow('{1}', '{2}', '{3}', '{4}');"
    Private Property _deskRoleUser As DeskRoleUser
    Private Property _deskDocumentVersionfacade As DeskDocumentVersionFacade
    Private Property _deskDocumentFacade As DeskDocumentFacade
    Private _currentDeskRigths As DeskRightsUtil
    Private _CurrentDeskDocumentDto As DeskDocumentResult
    Private _currentDesk As Desk
#End Region

#Region "Properties"

    Public Property BindAsyncEnable As Boolean

    Private ReadOnly Property DeskDocumentSelected As IList(Of DeskDocumentResult)
        Get
            Dim keys As IList(Of DeskDocumentResult) = New List(Of DeskDocumentResult)()
            If dgvDeskDocument.SelectedItems.Count > 0 Then
                For Each item As GridDataItem In dgvDeskDocument.SelectedItems
                    Dim doc As DeskDocumentResult = DeskDocumentDataSource.FirstOrDefault(Function(x) x.BiblosSerializeKey.Eq(item.GetDataKeyValue("BiblosSerializeKey").ToString()))
                    keys.Add(doc)
                Next
            End If
            Return keys
        End Get
    End Property


    Public ReadOnly Property JavascriptClosingFunction As String
        Get
            Return ID & "_CloseDocument"
        End Get
    End Property

    Public ReadOnly Property JavascriptClosingCheckInFunction As String
        Get
            Return ID & "_CloseCheckInDocument"
        End Get
    End Property

    Public ReadOnly Property JavascriptClosingSelectTemplateFunction As String
        Get
            Return ID & "_CloseSelectTemplateWindow"
        End Get
    End Property

    Private Property DeskDocumentDataSource As IList(Of DeskDocumentResult)
        Get
            Return TryCast(Session("deskDocumentDataSource"), IList(Of DeskDocumentResult))
        End Get
        Set(value As IList(Of DeskDocumentResult))
            Session("deskDocumentDataSource") = value
        End Set
    End Property

    Public Property ColumnLastCommentVisible As Boolean
        Get
            Return dgvDeskDocument.Columns.FindByUniqueName(COLUMN_LAST_COMMENT).Visible
        End Get
        Set(ByVal value As Boolean)
            dgvDeskDocument.Columns.FindByUniqueName(COLUMN_LAST_COMMENT).Visible = value
        End Set
    End Property

    Public Property ColumnSelectVisible As Boolean
        Get
            Return dgvDeskDocument.Columns.FindByUniqueName(COLUMN_SELECT_DOCUMENT).Visible
        End Get
        Set(ByVal value As Boolean)
            dgvDeskDocument.Columns.FindByUniqueName(COLUMN_SELECT_DOCUMENT).Visible = value
        End Set
    End Property

    Public Property ButtonStoryBoardVisible As Boolean
        Get
            If ViewState("ButtonStoryBoardVisible") IsNot Nothing Then
                Return DirectCast(ViewState("ButtonStoryBoardVisible"), Boolean)
            End If
            Return False
        End Get
        Set(ByVal value As Boolean)
            ViewState("ButtonStoryBoardVisible") = value
        End Set
    End Property

    ''' <summary>
    ''' Visibilità della colonna di cancellazione del documento dal tavolo
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ButtonDeleteEnable As Boolean
        Get
            If ViewState("ButtonDeleteEnable") IsNot Nothing Then
                Return DirectCast(ViewState("ButtonDeleteEnable"), Boolean)
            End If
            Return False
        End Get
        Set(ByVal value As Boolean)
            ViewState("ButtonDeleteEnable") = value
        End Set
    End Property

    Public Property BtnUploadDocumentVisible As Boolean
        Get
            Return ToolBar.Items.FindItemByValue("btnUploadDocument").Visible
        End Get
        Set(ByVal value As Boolean)
            ToolBar.Items.FindItemByValue("btnUploadDocument").Visible = value
        End Set
    End Property

    Public Property BtnScannerDocumentVisible As Boolean
        Get
            Return ToolBar.Items.FindItemByValue("btnScannerDocument").Visible
        End Get
        Set(ByVal value As Boolean)
            ToolBar.Items.FindItemByValue("btnScannerDocument").Visible = value
        End Set
    End Property
    Public Property BtnScannerDocumentImageUrl As String
        Get
            Return ToolBar.Items.FindItemByValue("btnScannerDocument").ImageUrl
        End Get
        Set(ByVal value As String)
            ToolBar.Items.FindItemByValue("btnScannerDocument").ImageUrl = value
        End Set
    End Property

    Public Property BtnSignDocumentVisible As Boolean
        Get
            Return ToolBar.Items.FindItemByValue("btnSignDocument").Visible
        End Get
        Set(ByVal value As Boolean)
            ToolBar.Items.FindItemByValue("btnSignDocument").Visible = value
        End Set
    End Property

    Public Property BtnSelectTemplateVisible As Boolean
        Get
            Return ToolBar.Items.FindItemByValue("btnSelectTemplate").Visible
        End Get
        Set(ByVal value As Boolean)
            ToolBar.Items.FindItemByValue("btnSelectTemplate").Visible = value
        End Set
    End Property

    Protected ReadOnly Property BtnSignDocument As RadToolBarButton
        Get
            Return DirectCast(ToolBar.Items.FindItemByValue("btnSignDocument"), RadToolBarButton)
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

    Public Property IsReadOnly As Boolean
        Get
            Return CType(ViewState("IsReadOnly"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            ViewState("IsReadOnly") = value
        End Set
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

    Private Property DeskDocumentFacade As DeskDocumentFacade
        Get
            Return _deskDocumentFacade
        End Get
        Set(value As DeskDocumentFacade)
            _deskDocumentFacade = value
        End Set
    End Property

    Public ReadOnly Property CurrentDeskRoleUser As DeskRoleUser
        Get
            If _deskRoleUser Is Nothing Then
                _deskRoleUser = CurrentDesk.DeskRoleUsers.Where(Function(t) t.AccountName.Eq(DocSuiteContext.Current.User.FullUserName)).FirstOrDefault()
            End If
            Return _deskRoleUser
        End Get
    End Property

    Protected ReadOnly Property CurrentDeskDocumentVersionFacade As DeskDocumentVersionFacade
        Get
            If _deskDocumentVersionfacade Is Nothing Then
                _deskDocumentVersionfacade = New DeskDocumentVersionFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _deskDocumentVersionfacade
        End Get
    End Property

    Protected ReadOnly Property CurrentDeskDocumentFacade As DeskDocumentFacade
        Get
            If _deskDocumentFacade Is Nothing Then
                _deskDocumentFacade = New DeskDocumentFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _deskDocumentFacade
        End Get
    End Property

    Protected Overloads ReadOnly Property CurrentDeskRights As DeskRightsUtil
        Get
            If _currentDeskRigths Is Nothing AndAlso CurrentDesk IsNot Nothing Then
                _currentDeskRigths = New DeskRightsUtil(CurrentDesk, DocSuiteContext.Current.User.FullUserName)
            End If
            Return _currentDeskRigths
        End Get
    End Property

    Protected ReadOnly Property CurrentDesk As Desk
        Get
            If Not DeskId.HasValue Then Return Nothing
            _currentDesk = CurrentDeskFacade.GetById(DeskId.Value)
            Return _currentDesk
        End Get
    End Property

    Public Property DeskId As Guid?
        Get
            Return CType(ViewState("DeskId"), Guid)
        End Get
        Set(value As Guid?)
            ViewState("DeskId") = value
        End Set
    End Property

    Protected Overridable ReadOnly Property CurrentDeskFacade As DeskFacade
        Get
            If _deskFacade Is Nothing Then
                _deskFacade = New DeskFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _deskFacade
        End Get
    End Property

    Public Property CurrentDeskDocumentDto As DeskDocumentResult
        Get
            Return _CurrentDeskDocumentDto
        End Get
        Set(ByVal value As DeskDocumentResult)
            _CurrentDeskDocumentDto = value
        End Set
    End Property

    Public ReadOnly Property DocumentsToSign As IList(Of MultiSignDocumentInfo)
        Get
            Dim list As New List(Of MultiSignDocumentInfo)
            For Each dto As DeskDocumentResult In DeskDocumentSelected
                ' Creazione oggetto per pagina di firma multipla
                Dim documentInfo As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(dto.BiblosSerializeKey))
                Dim msdi As New MultiSignDocumentInfo(documentInfo)
                msdi.GroupCode = dto.IdDeskDocument.ToString()
                msdi.Mandatory = True
                msdi.DocType = "Documento"
                msdi.Description = CurrentDesk.Description
                msdi.IdOwner = dto.IdDeskDocument.ToString()

                list.Add(msdi)
            Next
            Return list
        End Get
    End Property

    Public Property ParentPageUrl As String
        Get
            Return CType(ViewState("ParentPageUrl"), String)
        End Get
        Set(value As String)
            ViewState("ParentPageUrl") = value
        End Set
    End Property

    ''' <summary> Indica se la pagina è tornata dalla firma multipla senza operazioni </summary>
    Private ReadOnly Property BackFromMultiSign As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(MultipleSign.MultiSignUndoQuerystring, False)
        End Get
    End Property

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

    Protected Property CurrentDeskDocumentDtoSingleSign As DeskDocumentResult
        Get
            Return CType(Session("CurrentDeskDocumentDtoSingleSign"), DeskDocumentResult)
        End Get
        Set(value As DeskDocumentResult)
            If value Is Nothing Then
                Session.Remove("CurrentDeskDocumentDtoSingleSign")
            Else
                Session("CurrentDeskDocumentDtoSingleSign") = value
            End If
        End Set
    End Property

    Protected Property CurrentDocumentInfoSigleSign As BiblosDocumentInfo
        Get
            Return CType(Session("CurrentDocumentInfoSigleSign"), BiblosDocumentInfo)
        End Get
        Set(value As BiblosDocumentInfo)
            If value Is Nothing Then
                Session.Remove("CurrentDocumentInfoSigleSign")
            Else
                Session("CurrentDocumentInfoSigleSign") = value
            End If
        End Set
    End Property

#End Region

#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        ' SET NO CACHE
        Page.Response.Cache.SetAllowResponseInBrowserHistory(False)
        Page.Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Page.Response.Cache.SetNoStore()
        Page.Response.Cache.SetValidUntilExpires(True)
        DeskDocumentFacade = New DeskDocumentFacade(DocSuiteContext.Current.User.FullUserName)

        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub ToolBar_ButtonClick(sender As Object, e As RadToolBarEventArgs) Handles ToolBar.ButtonClick
        Dim sourceControl As RadToolBarButton = DirectCast(e.Item, RadToolBarButton)
        Select Case sourceControl.CommandName
            Case "btnUploadDocument"
                windowUploadDocument.OnClientClose = JavascriptClosingFunction
                windowUploadDocument.Width = Unit.Pixel(WindowWidth)
                windowUploadDocument.Height = Unit.Pixel(WindowHeight)

                Dim queryString As String = String.Format("Type={0}&MultiDoc=true", Type)

                AjaxManager.ResponseScripts.Add(String.Format(OPEN_WINDOW_SCRIPT, ID, COMMON_UPLOAD_DOCUMENT_PATH, windowUploadDocument.ClientID, queryString))
            Case "btnScannerDocument"
                'ScannerLight
                Dim params As StringBuilder = New StringBuilder()
                windowScannerDocument.OnClientClose = JavascriptClosingFunction
                windowScannerDocument.Width = Unit.Pixel(700)
                windowScannerDocument.Height = Unit.Pixel(480)
                Dim scannerLightPath As String = SCANNER_LIGHT_PATH
                If ProtocolEnv.ScannerLightRestEnabled Then
                    scannerLightPath = SCANNER_LIGHT_PATH_REST
                    params.AppendFormat("&multipleEnabled={0}", MultipleDocuments)
                End If
                AjaxManager.ResponseScripts.Add(String.Format(OPEN_WINDOW_SCRIPT, ID, scannerLightPath, windowScannerDocument.ClientID, params.ToString()))

            Case "btnSelectTemplate"
                wndSelectTemplate.OnClientClose = JavascriptClosingSelectTemplateFunction
                wndSelectTemplate.Width = Unit.Pixel(WindowWidth)
                wndSelectTemplate.Height = Unit.Pixel(500)

                Dim queryString As String = String.Concat("Type=", Type)
                AjaxManager.ResponseScripts.Add(String.Format(OPEN_WINDOW_SCRIPT, ID, TEMPLATE_DOCUMENT_PATH, wndSelectTemplate.ClientID, queryString))

        End Select
    End Sub

    Protected Sub UscDeskDocument_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arguments As String() = Split(e.Argument, "|", 5)
        If Not arguments(0).Eq(ClientID) Then
            Exit Sub
        End If

        If arguments.Count >= 3 AndAlso Not String.IsNullOrEmpty(arguments(1)) Then
            Select Case arguments(1)
                Case "UPLOAD"
                    Dim deserialized As Dictionary(Of String, String) = JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(arguments(2))
                    For Each item As KeyValuePair(Of String, String) In deserialized
                        Dim name As String = item.Value
                        Dim doc As New TempFileDocumentInfo(name, New FileInfo(Path.Combine(CommonUtil.GetInstance().AppTempPath, item.Key)))

                        If doc.Size > 0 OrElse ProtocolEnv.AllowZeroBytesUpload Then
                            LoadDocumentInfo(doc, True, Date.Today)
                        End If
                    Next
                    RaiseEvent DocumentUploaded(Me, New DocumentEventArgs())

                Case "SETCHECKINVERSION"
                    Dim deserialized As Dictionary(Of String, String) = JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(arguments(2))
                    Dim biblosSerializedKey As String = DirectCast(arguments(3), String)
                    Dim item As KeyValuePair(Of String, String) = deserialized.FirstOrDefault()
                    Dim name As String = item.Value
                    Dim doc As New TempFileDocumentInfo(name, New FileInfo(Path.Combine(CommonUtil.GetInstance().AppTempPath, item.Key)))
                    If doc.Size > 0 OrElse ProtocolEnv.AllowZeroBytesUpload Then
                        Dim dto As DeskDocumentResult = DeskDocumentDataSource.Where(Function(t) t.BiblosSerializeKey.Eq(biblosSerializedKey)).FirstOrDefault()
                        OpenChangeVersionWindow(dto.LastVersion, biblosSerializedKey, deserialized)
                    End If


                Case "UPLOADCHECKIN"
                    Dim deserialized As Dictionary(Of String, String) = JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(arguments(2))
                    Dim BiblosSerializedKey As String = DirectCast(arguments(3), String)
                    Dim newVersion As Decimal = Decimal.Parse(arguments(4))

                    ' Intercetto un unico file
                    Dim item As KeyValuePair(Of String, String) = deserialized.FirstOrDefault()
                    Dim name As String = item.Value
                    Dim doc As New TempFileDocumentInfo(name, New FileInfo(Path.Combine(CommonUtil.GetInstance().AppTempPath, item.Key)))
                    If doc.Size > 0 OrElse ProtocolEnv.AllowZeroBytesUpload Then
                        If Not String.IsNullOrEmpty(BiblosSerializedKey) Or BiblosSerializedKey.Eq("undefined") Then
                            Dim dto As DeskDocumentResult = DeskDocumentDataSource.Where(Function(t) t.BiblosSerializeKey.Eq(BiblosSerializedKey)).FirstOrDefault()
                            Dim newDocument As BiblosDocumentInfo = DeskDocumentFacade.CheckIn(CurrentDesk, CurrentDeskRoleUser, dto, doc.Stream, DocSuiteContext.Current.User.FullUserName, DocumentsService.ContentFormat.Binary, newVersion)
                            RaiseEvent DocumentCheckIn(Me, New DeskDocumentResultEventArgs() With {.DeskDocumentResult = dto})
                        End If
                    End If
                Case "SIGN"
                    Try
                        If Not String.IsNullOrEmpty(arguments(2)) Then
                            Dim signedFile As TempFileDocumentInfo = CType(DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(arguments(2))), TempFileDocumentInfo)
                            If Not signedFile.FileInfo.Exists Then
                                BasePage.AjaxAlert(String.Format("Documento firmato non è valido, reinserire il documento. {0}", ProtocolEnv.DefaultErrorMessage), False)
                                Exit Sub
                            End If
                            CurrentDocumentInfoSigleSign.Stream = signedFile.Stream

                            If FileHelper.MatchExtension(signedFile.FileInfo.Name, FileHelper.P7M) Then
                                CurrentDocumentInfoSigleSign.Name += FileHelper.P7M
                            End If

                            FileLogger.Info(LoggerName, String.Format("[{0}.SIGN] fileName: {1}", ClientID, CurrentDocumentInfoSigleSign.Name))
                            CurrentDeskDocumentFacade.UpdateSignedDocument(CurrentDesk, CurrentDeskRoleUser, CurrentDeskDocumentDtoSingleSign, CurrentDocumentInfoSigleSign, DocSuiteContext.Current.User.FullUserName)
                            CurrentDeskDocumentDtoSingleSign.IsSigned = True
                            FileLogger.Info(LoggerName, String.Format("[{0}.SIGN] RaiseEvent DocumentSigned", ClientID))
                            RaiseEvent DocumentSigned(Me, New DeskDocumentResultEventArgs() With {.DeskDocumentResult = CurrentDeskDocumentDtoSingleSign})
                        End If
                    Catch ex As Exception
                        FileLogger.Info(LoggerName, ex.Message)
                    Finally
                        CurrentDeskDocumentDtoSingleSign = Nothing
                    End Try
                Case "TEMPLATEDOCUMENT"
                    Dim idArchiveChain As Guid = Nothing
                    If Guid.TryParse(arguments(2), idArchiveChain) Then
                        Dim docs As ICollection(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocumentsLatestVersion(String.Empty, idArchiveChain)
                        Dim newName As String = String.Empty
                        Dim tmpFile As TempFileDocumentInfo = Nothing
                        For Each doc As BiblosDocumentInfo In docs
                            newName = doc.Name.Replace(Path.GetFileNameWithoutExtension(doc.Name), arguments(3))
                            tmpFile = New TempFileDocumentInfo(newName, BiblosFacade.SaveUniqueToTemp(doc))

                            LoadDocumentInfo(tmpFile, True, Date.Today)
                            RaiseEvent DocumentUploaded(Me, New DocumentEventArgs() With {.Document = tmpFile})
                        Next
                    End If
                Case "RELOADDOCUMENT"
                    RaiseEvent DocumentReloaded(Me, New DocumentEventArgs())

                Case "SAVENEWVERSION"
                    NewVersionDocument()
            End Select
        End If
    End Sub

    Protected Sub dgvDeskDocument_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles dgvDeskDocument.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim dto As DeskDocumentResult = DirectCast(e.Item.DataItem, DeskDocumentResult)

        ' imgDocumentExtensionType
        With DirectCast(e.Item.FindControl("imgDocumentExtensionType"), ImageButton)
            If dto.IsSigned AndAlso FileHelper.MatchExtension(dto.Name, FileHelper.PDF) Then
                .ImageUrl = ImagePath.FormatImageName("file_extension_pdf_signed", True)
            Else
                .ImageUrl = ImagePath.FromFile(dto.Name, True)
            End If

            Dim url As String = ResolveUrl("~/Viewers/DocumentInfoViewer.aspx?" & CommonShared.AppendSecurityCheck(dto.BiblosSerializeKey))
            .OnClientClick = String.Format(OPEN_WINDOW_SCRIPT, ID, url, windowPreviewDocument.ClientID, "")
        End With

        ' lblDocumentName
        DirectCast(e.Item.FindControl("lblDocumentName"), Label).Text = dto.Name

        ' lblLastDate
        With DirectCast(e.Item.FindControl("lblLastDate"), Label)
            If dto.RegistrationDate.HasValue Then
                .Text = dto.RegistrationDate.Value.ToString("dd/MM/yyyy")
            End If
        End With

        With DirectCast(e.Item.FindControl("lblDocVersion"), Label)
            If dto.LastVersion.HasValue Then
                .Text = dto.LastVersion.Value.ToString("0.00").Replace(",", ".")
            End If
        End With


        ' lblLastComment
        If ColumnLastCommentVisible Then
            DirectCast(e.Item.FindControl("lblLastComment"), Label).Text = dto.LastComment
        End If

        ' btnViewStoryBoard
        If ButtonStoryBoardVisible AndAlso dto.IsSavedToBiblos Then
            With DirectCast(e.Item.FindControl("btnViewStoryBoard"), RadButton)
                .Visible = True
            End With
        End If

        Dim deskDocumentRightsUtil As DeskDocumentRightsUtil = New DeskDocumentRightsUtil(dto, CurrentDeskRights, DocSuiteContext.Current.User.FullUserName)
        With DirectCast(e.Item.FindControl("btnCheckInDocument"), RadButton)
            .Visible = deskDocumentRightsUtil.IsCheckInButtonVisible
            .Enabled = deskDocumentRightsUtil.IsCheckInButtonEnable
            If (Not deskDocumentRightsUtil.IsCheckInButtonEnable) Then
                .Image.ImageUrl = "../Comm/Images/CheckInOpacity.png"
                .ToolTip = String.Format("Estratto dall'utente: {0}", deskDocumentRightsUtil.CurrentUserDocumentCheckOut)
            Else
                .Image.ImageUrl = "../Comm/Images/CheckIn.png"
                .ToolTip = String.Format("Archivia documento")
            End If
        End With

        CType(CType(e.Item, GridDataItem)("DocumentSelect").Controls(0), CheckBox).Enabled = Not deskDocumentRightsUtil.IsDocumentCheckOut

        With DirectCast(e.Item.FindControl("btnCheckOutDocument"), RadButton)
            If CurrentDeskRights IsNot Nothing Then
                .Visible = deskDocumentRightsUtil.IsCheckOutButtonVisible
            Else
                .Visible = False
            End If
        End With

        With DirectCast(e.Item.FindControl("btnUndoCheckOutDocument"), RadButton)
            If CurrentDeskRights IsNot Nothing Then
                .Visible = deskDocumentRightsUtil.IsUndoCheckOutButtonVisible
            Else
                .Visible = False
            End If
        End With

        With DirectCast(e.Item.FindControl("btnReopenCheckOutDocument"), RadButton)
            If CurrentDeskRights IsNot Nothing Then
                .Visible = deskDocumentRightsUtil.IsUndoCheckOutButtonVisible
            Else
                .Visible = False
            End If
        End With

        With DirectCast(e.Item.FindControl("btnSignSingleDocument"), RadButton)
            If dto.IsSigned Then
                .Visible = True
                .Enabled = False
            Else
                If CurrentDeskRights IsNot Nothing Then
                    .Visible = deskDocumentRightsUtil.IsCheckOutButtonVisible
                    .Enabled = True
                Else
                    .Visible = False
                    .Enabled = False
                End If
            End If
        End With

        With DirectCast(e.Item.FindControl("btnDeleteDocument"), RadButton)
            ' In fase di inserimento del tavolo la funzione di cancellazione è abilitata sempre.
            Select Case Type
                Case TYPE_INSERT_NAME
                    .Visible = True
                Case TYPE_MODIFY_NAME
                    .Visible = CurrentDeskRights.IsDocumentButtonDeleteVisible AndAlso deskDocumentRightsUtil.CanDeleteDocument
                Case Else
                    .Visible = False
            End Select
        End With

        With DirectCast(e.Item.FindControl("btnRenameDocument"), RadButton)
            If CurrentDeskRights IsNot Nothing Then
                .Visible = True
                .Enabled = deskDocumentRightsUtil.IsRanameButtonEnable
            Else
                .Visible = False
                .Enabled = False
            End If
        End With
    End Sub

    Protected Sub OpenChangeVersionWindow(lastversion As Decimal, biblokey As String, newDocumentAttributes As Dictionary(Of String, String))
        Dim newversion As Decimal = lastversion + 0.01D
        Dim version As String() = Split(newversion.ToString("#,##0.00"), ",")
        txtVersionDocMax.Text = version(0)
        txtVersionDocMin.Text = version(1)
        lblActualVersion.Text = FormatNumber(lastversion, 2).ToString().Replace(",", ".")


        windowChangeVersionDocument.AddAttribute(BIBLOS_KEY, biblokey)
        windowChangeVersionDocument.AddAttribute(NEW_DOCUMENT_ATTRIBUTES, JsonConvert.SerializeObject(newDocumentAttributes))
        windowChangeVersionDocument.Height = Unit.Pixel(150)
        AjaxManager.ResponseScripts.Add(String.Format(OPEN_WINDOW_CONTROL, ID, windowChangeVersionDocument.ClientID))
    End Sub

    Protected Sub dgvDeskDocument_ItemCommand(ByVal sender As Object, ByVal e As GridCommandEventArgs) Handles dgvDeskDocument.ItemCommand
        Dim rowItem As GridDataItem = DirectCast(e.Item, GridDataItem)
        Dim biblosSerializeKey As String = rowItem.GetDataKeyValue("BiblosSerializeKey").ToString()
        Dim dto As DeskDocumentResult = DeskDocumentDataSource.SingleOrDefault(Function(x) x.BiblosSerializeKey.Eq(biblosSerializeKey))
        Select Case e.CommandName
            Case "CheckIn"


                If DocSuiteContext.Current.ProtocolEnv.DeskShareCheckOutEnabled Then
                    ' Check in da directory condivisa
                    Dim deskVersioning As DeskDocumentVersion = CurrentDeskDocumentVersionFacade.GetLastVersionByIdDeskDocument(dto.IdDeskDocument.Value)
                    Dim documentInfoToCheckIn As DocumentInfo = DeskDocumentFacade.GetLastDocumentVersion(dto)
                    Dim fileName As String = String.Format("{0}_{1}{2}", documentInfoToCheckIn.Name, deskVersioning.Id, Path.GetExtension(documentInfoToCheckIn.Name))

                    Dim doc As New FileDocumentInfo(New FileInfo(Path.Combine(DocSuiteContext.Current.ProtocolEnv.DeskShare, fileName)))
                    Dim newDocument As BiblosDocumentInfo = DeskDocumentFacade.CheckIn(CurrentDesk, CurrentDeskRoleUser, dto, doc.Stream, DocSuiteContext.Current.User.FullUserName, DocumentsService.ContentFormat.Binary, Nothing)
                    DeleteLocalFile(fileName)
                    RaiseEvent DocumentCheckIn(Me, New DeskDocumentResultEventArgs() With {.DeskDocumentResult = dto})
                Else
                    ' Check in da upload
                    windowUploadDocument.OnClientClose = JavascriptClosingCheckInFunction
                    windowUploadDocument.Width = Unit.Pixel(WindowWidth)
                    windowUploadDocument.Height = Unit.Pixel(WindowHeight)
                    Dim params As New StringBuilder()
                    params.AppendFormat("Type={0}", Type)
                    If MultipleDocuments Then
                        params.Append("&MultiDoc=false")
                    End If
                    If AllowedExtensions IsNot Nothing AndAlso AllowedExtensions.Count > 0 Then
                        params.AppendFormat("&allowedextensions={0}", String.Join(",", AllowedExtensions))
                    End If
                    AjaxManager.ResponseScripts.Add(String.Format(OPEN_WINDOW_CHECKIN_SCRIPT, ID, COMMON_UPLOAD_DOCUMENT_PATH, windowUploadDocument.ClientID, params.ToString(), dto.BiblosSerializeKey.ToString()))
                End If


            Case "ReopenCheckOut"
                Try
                    If DocSuiteContext.Current.ProtocolEnv.DeskShareCheckOutEnabled Then
                        Dim deskVersioning As DeskDocumentVersion = CurrentDeskDocumentVersionFacade.GetLastVersionByIdDeskDocument(dto.IdDeskDocument.Value)
                        Dim checkOutDoc As DocumentInfo = DeskDocumentFacade.GetLastDocumentVersion(dto)
                        checkOutDoc.Name = String.Format("{0}_{1}{2}", checkOutDoc.Name, deskVersioning.Id, Path.GetExtension(checkOutDoc.Name))
                        OpenLocalFile(checkOutDoc)
                    End If
                Catch ex As Exception
                    FileLogger.Error(LoggerName, ex.Message, ex)
                    BasePage.AjaxAlert(ex.Message)
                End Try
            Case "CheckOut"
                Dim checkOutDoc As BiblosDocumentInfo
                Dim deskVersioning As DeskDocumentVersion = CurrentDeskDocumentVersionFacade.GetLastVersionByIdDeskDocument(dto.IdDeskDocument.Value)
                Try
                    checkOutDoc = DeskDocumentFacade.CheckOut(CurrentDesk, CurrentDeskRoleUser, deskVersioning, dto, DocSuiteWeb.Data.DocSuiteContext.Current.User.FullUserName, DocumentsService.ContentFormat.Binary, False)

                    If DocSuiteContext.Current.ProtocolEnv.DeskShareCheckOutEnabled Then
                        checkOutDoc.Name = String.Format("{0}_{1}{2}", checkOutDoc.Name, deskVersioning.Id, Path.GetExtension(checkOutDoc.Name))
                        OpenLocalFile(checkOutDoc)
                    Else
                        StartDownload(checkOutDoc)
                    End If
                Catch ex As Exception
                    FileLogger.Error(LoggerName, ex.Message, ex)
                    BasePage.AjaxAlert(ex.Message)
                End Try

                RaiseEvent DocumentCheckOut(Me, New DeskDocumentResultEventArgs() With {.DeskDocumentResult = dto})
            Case "CheckUndoOut"
                Dim deskVersioning As DeskDocumentVersion = CurrentDeskDocumentVersionFacade.GetByIdAndVersion(dto.IdDeskDocument.Value, dto.LastVersion.Value)
                Try
                    If DocSuiteContext.Current.ProtocolEnv.DeskShareCheckOutEnabled Then
                        ' Riestraggo il documento per avere il nome.
                        Dim documentInfoToCheckIn As DocumentInfo = DeskDocumentFacade.GetLastDocumentVersion(dto)
                        Dim fileName As String = String.Format("{0}_{1}{2}", documentInfoToCheckIn.Name, deskVersioning.Id, Path.GetExtension(documentInfoToCheckIn.Name))
                        DeleteLocalFile(fileName)
                    End If
                    DeskDocumentFacade.UndoCheckOut(CurrentDesk, CurrentDeskRoleUser, deskVersioning, dto, DocSuiteContext.Current.User.FullUserName)
                Catch ex As Exception
                    FileLogger.Error(LoggerName, ex.Message, ex)
                    BasePage.AjaxAlert(ex.Message)
                End Try
                RaiseEvent DocumentUndoCheckOut(Me, New DeskDocumentResultEventArgs() With {.DeskDocumentResult = dto})
            Case "RenameDocument"
                txtNewName.Text = Path.GetFileNameWithoutExtension(dto.Name)
                windowRenameDocument.AddAttribute(BIBLOS_KEY, dto.BiblosSerializeKey)
                windowRenameDocument.Height = Unit.Pixel(150)
                AjaxManager.ResponseScripts.Add(String.Format(OPEN_WINDOW_CONTROL, ID, windowRenameDocument.ClientID))
            Case "SignSingleDocument"
                Dim deskVersioning As DeskDocumentVersion = CurrentDeskDocumentVersionFacade.GetByIdAndVersion(dto.IdDeskDocument.Value, dto.LastVersion.Value)
                CurrentDeskDocumentDtoSingleSign = dto
                Try
                    If Not DeskDocumentFacade.IsCheckOut(CurrentDeskDocumentDtoSingleSign) Then
                        ' Leggo il documento da biblos senza estrarlo.
                        CurrentDocumentInfoSigleSign = DirectCast(DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(CurrentDeskDocumentDtoSingleSign.BiblosSerializeKey)), BiblosDocumentInfo)

                        Dim args As New DocumentBeforeSignEventArgs()
                        args.SourceDocument = CurrentDocumentInfoSigleSign
                        RaiseEvent DocumentBeforeSingleSign(Me, args)

                        If args.Cancel Then
                            Exit Sub
                        End If

                        signWindow.OnClientClose = ID & "_CloseSignWindow"
                        AjaxManager.ResponseScripts.Add(String.Format(If(ProtocolEnv.ResizeSignWindowEnabled, OPEN_WINDOW_SIGN_SCRIPT, OPEN_WINDOW_SCRIPT), ID, "../Comm/SingleSign.aspx", signWindow.ClientID, CurrentDocumentInfoSigleSign.ToQueryString().AsEncodedQueryString()))
                    End If
                Catch ex As Exception
                    FileLogger.Error(LoggerName, ex.Message, ex)

                    BasePage.AjaxAlert(ex.Message)
                End Try
            Case "DeleteDocument"
                If dto IsNot Nothing Then
                    If Not dto.IdDeskDocument Is Nothing AndAlso dto.IdDeskDocument <> Guid.Empty Then
                        CurrentDeskDocumentFacade.Delete(dto.IdDeskDocument)
                    End If

                    DeskDocumentDataSource.Remove(dto)
                End If

                dgvDeskDocument.DataSource = DeskDocumentDataSource
                dgvDeskDocument.Rebind()
            Case "GoToStoryBoard"
                Response.Redirect(String.Format(STORYBOARD_PATH_FORMAT, dto.IdDesk, dto.IdDeskDocument))
        End Select
    End Sub
#End Region

#Region "Methods"
    Public Overloads Function GetKeyValue(Of T)(key As String) As T
        Return GetKeyValue(Of T, CommBasePage)(key)
    End Function

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf UscDeskDocument_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, dgvDeskDocument, BasePage.MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(dgvDeskDocument, dgvDeskDocument, BasePage.MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ToolBar, windowUploadDocument)
        AjaxManager.AjaxSettings.AddAjaxSetting(ToolBar, windowScannerDocument)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRenameDocument, btnRenameDocument, BasePage.MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSaveNewVersion, pnlChangeVersion)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlChangeVersion)
    End Sub

    Private Sub InitializeTypeMode()
        'Inizializzo le colonne in base al Type in query string
        Select Case Type
            Case TYPE_INSERT_NAME
                ColumnLastCommentVisible = False
                ButtonStoryBoardVisible = False
                ColumnSelectVisible = False
                BtnSignDocumentVisible = False
                dgvDeskDocument.AllowMultiRowSelection = False
            Case TYPE_MODIFY_NAME
                ColumnLastCommentVisible = True
                ButtonStoryBoardVisible = True
                ColumnSelectVisible = ProtocolEnv.SignDeskDocumentEnabled AndAlso CurrentDeskRights.CanSignDocument AndAlso IsReadOnly
                BtnSignDocumentVisible = ProtocolEnv.SignDeskDocumentEnabled AndAlso CurrentDeskRights.CanSignDocument AndAlso IsReadOnly
                dgvDeskDocument.AllowMultiRowSelection = ProtocolEnv.SignDeskDocumentEnabled AndAlso CurrentDeskRights.CanSignDocument AndAlso IsReadOnly
        End Select
    End Sub

    ''' <summary>
    ''' Esegue nuovamente l'inizializzazione del controllo per l'aggiornamento dei dati
    ''' e delle regole di visualizzazione
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Refresh()
        Initialize()
        InitializeUserControlSource()
    End Sub

    Private Sub Initialize()
        windowPreviewDocument.Height = Unit.Pixel(ProtocolEnv.DocumentPreviewHeight)
        windowPreviewDocument.Width = Unit.Pixel(ProtocolEnv.DocumentPreviewWidth)

        BtnSignDocument.PostBackUrl = Page.ResolveUrl(MultipleSignBasePage.GetMultipleSignUrl())

        If ProtocolEnv.ScannerLightRestEnabled Then
            BtnScannerDocumentImageUrl = "~/App_Themes/DocSuite2008/imgset16/scanner.png"
        End If

        'Inizializzo i pulsanti dell user control
        BtnScannerDocumentVisible = True
        BtnUploadDocumentVisible = True
        If IsReadOnly Then
            BtnScannerDocumentVisible = False
            BtnUploadDocumentVisible = False
        End If

        ' Clean Sign
        CurrentDeskDocumentDtoSingleSign = Nothing

        InitializeTypeMode()
        If Not BackFromMultiSign Then
            SaveSignedDocuments()
        End If

    End Sub

    ''' <summary>
    ''' Inizializza i dati dello user control per la corretta visualizzazione.
    ''' Da richiamare nell'inizializzazione della pagina.
    ''' </summary>
    Public Sub InitializeUserControlSource()
        DeskDocumentDataSource = Nothing
        If (BindAsyncEnable) Then
            dgvDeskDocument.DataSource = String.Empty
            dgvDeskDocument.DataBind()
        End If
    End Sub

    Private Sub LoadDocumentInfo(doc As DocumentInfo)
        LoadDocumentInfo(doc, False, Nothing)
    End Sub

    Private Sub LoadDocumentInfo(doc As DocumentInfo, append As Boolean, docDate As Date?)
        Dim docs As IList(Of DocumentInfo) = New List(Of DocumentInfo) From {doc}
        LoadDocumentInfo(docs, append, docDate)
    End Sub

    Private Sub LoadDocumentInfo(docs As IList(Of DocumentInfo), append As Boolean, docDate As Date?)
        Dim documents As IList(Of DeskDocumentResult) = New List(Of DeskDocumentResult)

        For Each doc As DocumentInfo In docs
            documents.Add(New DeskDocumentResult() With {.Name = doc.Name, .IsSavedToBiblos = False, .BiblosSerializeKey = doc.ToQueryString().AsEncodedQueryString(), .RegistrationDate = docDate, .Size = doc.Size, .LastComment = String.Empty})
        Next

        BindDeskDocuments(documents, append)
    End Sub

    Public Sub BindDeskDocuments(deskDocuments As IList(Of DeskDocumentResult), append As Boolean)
        If Not append Then
            DeskDocumentDataSource = Nothing
        End If

        Dim clonedDataSourceToExt As IList(Of DeskDocumentResult) = DeskDocumentDataSource
        If clonedDataSourceToExt Is Nothing Then
            clonedDataSourceToExt = New List(Of DeskDocumentResult)
        End If

        For Each document As DeskDocumentResult In deskDocuments
            clonedDataSourceToExt.Add(document)
        Next

        dgvDeskDocument.DataSource = clonedDataSourceToExt
        dgvDeskDocument.DataBind()

        DeskDocumentDataSource = clonedDataSourceToExt
    End Sub

    ''' <summary>
    ''' Metodo che ritorna la lista di documenti presenti nel controllo
    ''' </summary>
    Public Function GetDocuments() As IList(Of DocumentInfo)
        If DeskDocumentDataSource Is Nothing Then
            Return New List(Of DocumentInfo)
        End If

        Dim documents As IList(Of DocumentInfo) = New List(Of DocumentInfo)()
        For Each documentDto As DeskDocumentResult In DeskDocumentDataSource
            documents.Add(DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(documentDto.BiblosSerializeKey)))
        Next

        Return documents
    End Function

    ''' <summary>
    ''' Metodo che ritorna la lista di documenti aggiunti nella sessione corrente
    ''' </summary>
    Public Function GetAddedDocuments() As IList(Of DocumentInfo)
        If DeskDocumentDataSource Is Nothing Then
            Return New List(Of DocumentInfo)()
        End If

        Dim documents As IList(Of DocumentInfo) = New List(Of DocumentInfo)()
        For Each documentDto As DeskDocumentResult In DeskDocumentDataSource.Where(Function(x) Not x.IsSavedToBiblos)
            documents.Add(DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(documentDto.BiblosSerializeKey)))
        Next

        Return documents
    End Function

    Private Shared Function GetHandlerUrl(items As NameValueCollection, name As String) As String
        Return String.Format("{0}/Viewers/Handlers/DocumentInfoHandler.ashx/{1}?{2}",
                              DocSuiteContext.Current.CurrentTenant.DSWUrl,
                             FileHelper.FileNameToUrl(name),
                             CommonShared.AppendSecurityCheck(items.AsEncodedQueryString()))
    End Function

    Private Sub StartDownload(content As DocumentInfo)
        Dim itemCollection As NameValueCollection = content.ToQueryString()
        itemCollection.Add("Download", "True")
        itemCollection.Add("Original", "True")
        AjaxManager.ResponseScripts.Add(String.Format("StartDownload('{0}');", GetHandlerUrl(itemCollection, content.Name)))
    End Sub

    Private Const excel_ext As String = "|.xlsx|.xlsm|.xlsb|.xltx|.xltm|.xlt|.xls|.xlam|.xla|.xlw|.xlr|.csv|.dif|.slk|"
    Private Const word_ext As String = "|.doc|.docm|.docx|.docx|.dot|.dotm|.dotx|.htm|.html|.mht|.mhtml|.odt|.pdf|.rtf|.txt|.wps|.xml|.xps"

    ''' <summary>
    ''' Downlaod file checkout in share di rete
    ''' </summary>
    ''' <param name="content"></param>
    ''' <remarks></remarks>
    Private Sub OpenLocalFile(content As DocumentInfo)
        Dim destination As String = Path.Combine(DocSuiteContext.Current.ProtocolEnv.DeskShare, content.Name)
        If Not File.Exists(destination) Then
            File.WriteAllBytes(destination, content.Stream)
        End If
        Dim fileInfo As FileInfo = New FileInfo(destination)

        Dim functionName As String = "OpenAlert"
        If excel_ext.Contains(String.Concat("|", fileInfo.Extension, "|")) Then
            functionName = "OpenExcel"
        End If
        If word_ext.Contains(String.Concat("|", fileInfo.Extension, "|")) Then
            functionName = "OpenWord"
        End If

        If functionName.Eq("OpenAlert") Then
            destination = fileInfo.Extension
        End If

        Dim escaped As String = HttpUtility.JavaScriptStringEncode(destination)
        Dim jsOpenLocalFile As String = String.Format("{0}('{1}');", functionName, escaped)
        AjaxManager.ResponseScripts.Add(jsOpenLocalFile)
    End Sub

    Private Sub SaveSignedDocuments()
        Dim sourcePage As MultipleSignBasePage = TryCast(BasePage.PreviousPage, MultipleSignBasePage)
        If sourcePage IsNot Nothing Then
            For Each document As MultiSignDocumentInfo In sourcePage.SignedDocuments
                Dim file As FileDocumentInfo = DirectCast(document.DocumentInfo, FileDocumentInfo)
                Dim dto As DeskDocumentResult = DeskDocumentDataSource.FirstOrDefault(Function(x) x.IdDeskDocument.Equals(Guid.Parse(document.IdOwner)))

                'Eseguo update del documento
                Dim biblosDoc As BiblosDocumentInfo = DirectCast(DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(dto.BiblosSerializeKey)), BiblosDocumentInfo)
                biblosDoc.Stream = file.Stream
                If FileHelper.MatchExtension(file.Name, FileHelper.P7M) Then
                    biblosDoc.Name += FileHelper.P7M
                End If
                CurrentDeskDocumentFacade.UpdateSignedDocument(CurrentDesk, CurrentDeskRoleUser, dto, biblosDoc, DocSuiteContext.Current.User.FullUserName)
                RaiseEvent DocumentSigned(Me, New DeskDocumentResultEventArgs() With {.DeskDocumentResult = dto})
            Next
            ' Ricarico pagina per passare all'operazione successiva
            Response.Redirect(ParentPageUrl, True)
            Exit Sub
        End If
    End Sub

    Private Sub DeleteLocalFile(content As FileDocumentInfo)
        Dim destination As String = Path.Combine(DocSuiteContext.Current.ProtocolEnv.DeskShare, content.FileInfo.FullName)
        If Not File.Exists(destination) Then
            Return
        End If

        Try
            File.Delete(destination)
        Catch ex As Exception
            Dim message As String = String.Format("Non è stato possibile cancellare il file dal percorso: {0}", destination)
            FileLogger.Warn(LoggerName, message, ex)
        End Try
    End Sub

    Private Sub DeleteLocalFile(fileName As String)
        Dim destination As String = Path.Combine(DocSuiteContext.Current.ProtocolEnv.DeskShare, fileName)
        If Not File.Exists(destination) Then
            Return
        End If

        Try
            File.Delete(destination)
        Catch ex As Exception
            Dim message As String = String.Format("Non è stato possibile cancellare il file dal percorso: {0}", destination)
            FileLogger.Warn(LoggerName, message, ex)
        End Try
    End Sub

    Protected Sub RenameDocument(sender As Object, e As EventArgs) Handles btnRenameDocument.Click
        Dim newname As String = txtNewName.Text
        Dim biblosSerializeKey As String = windowRenameDocument.Attributes.Item(BIBLOS_KEY)
        Dim dto As DeskDocumentResult = DeskDocumentDataSource.SingleOrDefault(Function(x) x.BiblosSerializeKey.Eq(biblosSerializeKey))
        Try
            Dim toUpdate As BiblosDocumentInfo = BiblosDocumentInfo.GetDocumentInfo(dto.DocumentServer, dto.IdDocumentBiblos, Nothing, True).FirstOrDefault()
            toUpdate.Name = String.Concat(Path.GetFileNameWithoutExtension(newname), toUpdate.Extension)
            Dim deskVersioning As DeskDocumentVersion = CurrentDeskDocumentVersionFacade.GetLastVersionByIdDeskDocument(dto.IdDeskDocument.Value)
            DeskDocumentFacade.RenameDoc(CurrentDesk, CurrentDeskRoleUser, deskVersioning, toUpdate, DocSuiteContext.Current.User.FullUserName)

        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            BasePage.AjaxAlert(ex.Message)
        End Try

        AjaxManager.ResponseScripts.Add(String.Format("ClosewindowRenameDocument();"))
    End Sub

    Private Sub NewVersionDocument()
        Dim newStringVersion As String = String.Concat(txtVersionDocMax.Text, ",", txtVersionDocMin.Text)
        Dim biblosSerializeKey As String = windowChangeVersionDocument.Attributes.Item(BIBLOS_KEY)
        Dim dto As DeskDocumentResult = DeskDocumentDataSource.Where(Function(t) t.BiblosSerializeKey.Eq(biblosSerializeKey)).FirstOrDefault()
        Dim lastVersion As Decimal = If(dto.LastVersion, 0D)
        Dim newVersion As Decimal = Convert.ToDecimal(newStringVersion)
        Dim documentAttributes As String = windowChangeVersionDocument.Attributes.Item(NEW_DOCUMENT_ATTRIBUTES)
        If newVersion <= lastVersion Then
            BasePage.AjaxAlert("La versione inserita è minore o uguale alla versione attuale. Inserire una versione di valore maggiore")
        Else
            AjaxManager.ResponseScripts.Add(String.Format("{0}_ExecuteCheckInDocument('{1}','{2}','{3}');", ID, documentAttributes, biblosSerializeKey, newVersion))
        End If
    End Sub



#End Region

End Class