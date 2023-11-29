Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.ExtensionMethods
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models

Public Class AggregateToProtocol
    Inherits ProtBasePage
    Implements IProtocolInitializer


#Region " Fields "
    Private _whiteList() As String
    Private _blackList() As String
    Private _allDocuments As List(Of DocumentInfo)
    Private _protocolsKeys As List(Of Guid) = Nothing
    Private _protocols As List(Of Protocol) = Nothing
    Private _toSendProtocolType As SendDocumentUnitType?
#End Region

#Region " Properties "
    Private ReadOnly Property ProtocolsKeys As List(Of Guid)
        Get
            If _protocolsKeys.IsNullOrEmpty() Then
                If ViewState("keys") Is Nothing Then
                    _protocolsKeys = HttpContext.Current.Request.QueryString.GetValue(Of List(Of Guid))("keys")
                    ViewState("keys") = _protocolsKeys
                Else
                    _protocolsKeys = CType(ViewState("keys"), List(Of Guid))
                End If
            End If
            Return _protocolsKeys
        End Get
    End Property
    Private ReadOnly Property ProtocolList As List(Of Protocol)
        Get
            If Not ProtocolsKeys.IsNullOrEmpty() AndAlso _protocols Is Nothing Then
                _protocols = ProtocolsKeys.Select(Function(f) FacadeFactory.Instance.ProtocolFacade.GetById(f)).ToList()
            End If
            If _protocols Is Nothing Then
                _protocols = New List(Of Protocol)()
            End If
            Return _protocols
        End Get
    End Property
    Public ReadOnly Property WhiteList As String()
        Get
            If _whiteList Is Nothing Then
                If Not String.IsNullOrEmpty(ProtocolEnv.FileExtensionWhiteList) Then
                    _whiteList = ProtocolEnv.FileExtensionWhiteList.Split("|"c)
                Else
                    _whiteList = New String() {}
                End If
            End If
            Return _whiteList
        End Get
    End Property
    Public ReadOnly Property BlackList As String()
        Get
            If _blackList Is Nothing Then
                If Not String.IsNullOrEmpty(ProtocolEnv.FileExtensionBlackList) Then
                    _blackList = ProtocolEnv.FileExtensionBlackList.Split("|"c)
                Else
                    _blackList = New String() {}
                End If
            End If
            Return _blackList
        End Get
    End Property
    Public ReadOnly Property AllAttachmentDocuments As List(Of DocumentInfo)
        Get
            _allDocuments = New List(Of DocumentInfo)
            For Each selectedItem As GridDataItem In DocumentListGrid.MasterTableView.GetSelectedItems()
                Dim radio As RadioButton = CType(selectedItem.FindControl("rbtMainDocument"), RadioButton)
                If radio Is Nothing OrElse Not radio.Checked Then
                    Dim documentInfo As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(CType(selectedItem.GetDataKeyValue("Serialized"), String)))
                    Dim fileName As String = DirectCast(selectedItem.FindControl("txtNewFileName"), TextBox).Text
                    If Not String.IsNullOrEmpty(fileName) Then
                        Dim newTempDoc As FileInfo = documentInfo.SaveUniqueToTemp()
                        documentInfo = New TempFileDocumentInfo(fileName, newTempDoc)
                    End If
                    documentInfo.Name = FileHelper.ReplaceUnicode(documentInfo.Name)
                    _allDocuments.Add(documentInfo)
                End If
            Next
            Return _allDocuments
        End Get
    End Property
    Public ReadOnly Property SelectedDocument As DocumentInfo
        Get
            For Each item As GridDataItem In DocumentListGrid.Items
                Dim radio As RadioButton = CType(item.FindControl("rbtMainDocument"), RadioButton)
                If radio IsNot Nothing AndAlso radio.Checked Then
                    Dim key As String = item.GetDataKeyValue("Serialized").ToString()
                    Dim temp As NameValueCollection = HttpUtility.ParseQueryString(key)
                    Dim documentInfo As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(temp)
                    Dim fileName As String = DirectCast(item.FindControl("txtNewFileName"), TextBox).Text
                    If Not String.IsNullOrEmpty(fileName) Then
                        Dim newTempDoc As FileInfo = documentInfo.SaveUniqueToTemp()
                        documentInfo = New TempFileDocumentInfo(fileName, newTempDoc)
                    End If
                    documentInfo.Name = FileHelper.ReplaceUnicode(documentInfo.Name)
                    Return documentInfo
                End If
            Next
            Return Nothing
        End Get
    End Property
    Public ReadOnly Property DocumentsSource As List(Of DocumentInfo)
        Get
            Dim source As New List(Of DocumentInfo)
            For Each selectedItem As GridDataItem In DocumentListGrid.MasterTableView.Items
                Dim documentInfo As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(CType(selectedItem.GetDataKeyValue("Serialized"), String)))
                Dim fileName As String = DirectCast(selectedItem.FindControl("txtNewFileName"), TextBox).Text
                If Not String.IsNullOrEmpty(fileName) Then
                    Dim newTempDoc As FileInfo = documentInfo.SaveUniqueToTemp()
                    documentInfo = New TempFileDocumentInfo(fileName, newTempDoc)
                End If
                documentInfo.Name = FileHelper.ReplaceUnicode(documentInfo.Name)
                source.Add(documentInfo)
            Next
            Return source
        End Get
    End Property
    Private ReadOnly Property ToSendProtocolType As SendDocumentUnitType?
        Get
            If Not _toSendProtocolType.HasValue Then
                _toSendProtocolType = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of SendDocumentUnitType?)("ToSendProtocolType", Nothing)
            End If
            Return _toSendProtocolType
        End Get
    End Property

    Private ReadOnly Property FromViewer As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("FromViewer", False)
        End Get
    End Property
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            lblWarning.Visible = Not DocSuiteContext.Current.ProtocolEnv.EnableDictionDocumentSign AndAlso Not DocSuiteContext.Current.ProtocolEnv.CheckSignedEvaluateStream
            InitializeAggregateToProtocol()
        End If
    End Sub
    Private Sub DocumentListGridItemDataBound(sender As Object, e As GridItemEventArgs) Handles DocumentListGrid.ItemDataBound
        If e.Item.ItemType = GridItemType.Item OrElse e.Item.ItemType = GridItemType.AlternatingItem Then
            Dim bound As DocumentInfo = DirectCast(e.Item.DataItem, DocumentInfo)
            Dim radio As RadioButton = CType(e.Item.FindControl("rbtMainDocument"), RadioButton)
            Dim chk As Boolean? = CheckDocument(bound.Name)
            If chk.HasValue Then
                If chk.Value = True Then
                    e.Item.Selected = True
                Else
                    e.Item.SelectableMode = GridItemSelectableMode.None
                    radio.Enabled = False
                End If
            Else
                e.Item.Selected = False
            End If
            If FileHelper.MatchFileName(bound.Name, FileHelper.BUSTA_PDF) OrElse FileHelper.MatchExtension(bound.Name, FileHelper.XML) Then
                e.Item.Selected = False
            End If
            radio.Attributes.Add("onclick", String.Format("javascript:MutuallyExclusive(this, {0})", e.Item.RowIndex))
        End If
    End Sub
#End Region

#Region " Methods "
    Private Sub InitializeAggregateToProtocol()
        panelProt.Visible = True
        cmdProtInit.Visible = True
        cmdProtInit.Enabled = True

        Dim params As New StringBuilder()
        params.AppendFormat("Action={0}&ToSendProtocolType={1}&keys={2}", "ToSendProtocol", ToSendProtocolType, JsonConvert.SerializeObject(ProtocolsKeys))
        cmdProtInit.PostBackUrl = String.Format("~/Prot/ProtInserimento.aspx?{0}", CommonShared.AppendSecurityCheck(params.ToString()))

        If ToSendProtocolType.Equals(SendDocumentUnitType.ToMail) Then
            Title = "Gestisci Protocolli - Invia Mail"
        Else
            Title = "Gestisci Protocolli - Invia Pec"
        End If

        Dim documents As IList(Of DocumentInfo) = InizializeDocuments()
        DocumentListGrid.Visible = True
        DocumentListGrid.DataSource = BonificaDocumenti(documents)
        DocumentListGrid.DataBind()

        If DocumentListGrid.Items.Count > 0 Then
            Dim defaultMainDoc As GridDataItem = DocumentListGrid.Items.Item(0)
            Dim radio As RadioButton = CType(defaultMainDoc.FindControl("rbtMainDocument"), RadioButton)
            radio.Checked = True
            defaultMainDoc.Selected = True
        End If

        Dim recipients As IList(Of ContactDTO) = New List(Of ContactDTO)
        For Each prot As Protocol In ProtocolList
            recipients = GetContacts(prot)
            If recipients.Count > 0 Then
                For Each contact As ContactDTO In recipients
                    uscMittenti.DataSource.Add(contact)
                Next
                uscMittenti.DataBind()
            End If
        Next
        For Each prot As Protocol In ProtocolList
            Dim node As New RadTreeNode()
            node.Text = String.Format("{0} del {1:dd/MM/yyyy} ({2})", prot.FullNumber, prot.RegistrationDate.ToLocalTime(), prot.ProtocolObject)
            node.Font.Bold = True
            node.ImageUrl = "~/Comm/Images/DocSuite/Protocollo16.png"
            rtvProtocol.Nodes.Add(node)
        Next
    End Sub
    Private Function GetContacts(protocol As Protocol) As IList(Of ContactDTO)
        Dim recipients As IList(Of ContactDTO) = New List(Of ContactDTO)
        For Each item As ProtocolContact In protocol.Contacts
            If item.ComunicationType.Eq(ProtocolContactCommunicationType.Sender) Then
                recipients.Add(New ContactDTO(item.Contact, ContactDTO.ContactType.Address))
            End If
        Next

        For Each item As ProtocolContactManual In protocol.ManualContacts
            If item.ComunicationType.Eq(ProtocolContactCommunicationType.Sender) Then
                recipients.Add(New ContactDTO(item.Contact, item.Id))
            End If
        Next
        Return recipients
    End Function

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(DocumentListGrid, DocumentListGrid)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, panelProt, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlButtons, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscMittenti, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub
    Private Function CheckDocument(name As String) As Boolean?
        If WhiteList.Count = 0 AndAlso BlackList.Count = 0 Then
            Return True ' Nessun controllo richiesto
        End If
        If FileHelper.MatchExtension(name, BlackList) Then
            Return False ' File in Black List 
        End If
        If FileHelper.MatchExtension(name, WhiteList) Then
            Return True ' File in White List
        End If
        If ProtocolEnv.EnableGrayList Then
            Return Nothing ' Gray List
        End If
        Return False ' File non in WhiteList e GrayList disabilitata
    End Function

    Public Function GetProtocolInitializer() As ProtocolInitializer Implements IProtocolInitializer.GetProtocolInitializer
        Dim tor As New ProtocolInitializer()
        Dim allSelectedDocument As List(Of DocumentInfo) = AllAttachmentDocuments
        tor.ProtocolType = 1
        'Se il documento principale non è stato valorizzato significa che non si tratta di caricamento automatico da organigramma
        If tor.MainDocument Is Nothing Then
            Dim attachments As List(Of DocumentInfo) = allSelectedDocument
            ' Solamente un elemento può essere MAIN
            tor.MainDocument = SelectedDocument
            tor.Attachments = New List(Of DocumentInfo)()
            For Each attachment As DocumentInfo In attachments
                If CheckDocument(attachment.Name).GetValueOrDefault(True) Then
                    tor.Attachments.Add(attachment)
                End If
            Next
        End If

        Dim contacts As IList(Of ContactDTO)
        If ProtocolEnv.SelectSenderFromTreeviewEnabled Then
            contacts = uscMittenti.GetSelectedContacts()
        Else
            contacts = uscMittenti.GetContacts(False)
        End If
        If Not contacts.IsNullOrEmpty() Then
            tor.Recipients = contacts.ToList()
        End If
        Return tor
    End Function

    Private Function InizializeDocuments() As IList(Of DocumentInfo)
        Dim documents As IList(Of DocumentInfo) = New List(Of DocumentInfo)()
        If (Not PreviousPage Is Nothing AndAlso TypeOf PreviousPage Is ISendMail AndAlso PreviousPage.IsCrossPagePostBack) Then
            Dim docs As List(Of DocumentInfo) = CType(CType(PreviousPage, ISendMail).Documents, List(Of DocumentInfo))
            If docs.Any() Then
                Return docs
            End If
        End If
        Return documents
    End Function

    Private Function BonificaDocumenti(ByRef documents As IList(Of DocumentInfo)) As List(Of DocumentInfo)
        Dim bonificati As List(Of DocumentInfo) = New List(Of DocumentInfo)
        For Each doc As DocumentInfo In documents
            If FileHelper.MatchExtension(doc.Name, FileHelper.EML) Then
                Dim pdfDoc As New BiblosPdfDocumentInfo(doc)
                bonificati.Add(pdfDoc)
            Else
                bonificati.Add(doc)
            End If
        Next
        Return bonificati
    End Function
#End Region

End Class