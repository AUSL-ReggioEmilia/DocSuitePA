Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Web
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.OrganizationChart.xml
Imports VecompSoftware.DocSuiteWeb.DTO.PECMails
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.ExtensionMethods
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.Compress
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models
Imports System.Net.Http
Imports Limilabs.Mail
Imports VecompSoftware.Core.Command.CQRS.Events.Models.PECMails
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.PECMails
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Extensions
Imports VecompSoftware.DocSuiteWeb.Model.ServiceBus

Public Class EventPECStreamError
    Inherits PECBasePage
    Implements IProtocolInitializer

#Region " Fields "

    Private _mail As PECMail
    Private _sessionMail As PECMail
    Private _pecLabel As String = "PEC"
    Private _allDocuments As List(Of DocumentInfo)
    Private _whiteList() As String
    Private _blackList() As String
    Private _oChartItemData As OChartProtocolXml
    Private _segnaturaReader As SegnaturaReader

#End Region

#Region " Properties  "

    Public Property SessionPECMail As PECMail
        Get
            Return _sessionMail
        End Get
        Set(value As PECMail)
            _sessionMail = value
        End Set
    End Property

    Public Property PECMailId As Integer?

    Public Overloads Property PecLabel As String
        Get
            If ViewState("pecLabel") IsNot Nothing Then
                _pecLabel = ViewState("pecLabel").ToString()
            Else
                ViewState("pecLabel") = _pecLabel
            End If
            Return _pecLabel
        End Get
        Set(value As String)
            ViewState("pecLabel") = value
        End Set
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

    Public ReadOnly Property UseSenderInterop() As Boolean
        Get
            Return UseInterop AndAlso chkInteropMittente.Checked
        End Get
    End Property

    Public ReadOnly Property SegnaturaReader() As SegnaturaReader
        Get
            If _segnaturaReader Is Nothing Then
                Dim defaultIdContactParent As Integer = Nothing
                If ProtocolEnv.DefaultContactParentForInteropSender > 0 Then
                    defaultIdContactParent = ProtocolEnv.DefaultContactParentForInteropSender
                End If

                Dim pecMail As PECMail = CurrentPecMail
                If SessionPECMail IsNot Nothing Then
                    pecMail = SessionPECMail
                End If

                _segnaturaReader = New SegnaturaReader(pecMail.Segnatura, defaultIdContactParent)
                _segnaturaReader.LoadDocument()
            End If

            Return _segnaturaReader
        End Get
    End Property

    Public ReadOnly Property UseContactType() As Boolean
        Get
            If (rblTypeSender.SelectedIndex = 2) Then
                Return True
            End If
            Return False
        End Get
    End Property

    Public ReadOnly Property UseInterop() As Boolean
        Get
            Return Not chkInterop.Checked
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        uscPECInfo.PecLabel = PecLabel.ToLower()
        lblWarning.Visible = Not DocSuiteContext.Current.ProtocolEnv.EnableDictionDocumentSign AndAlso Not DocSuiteContext.Current.ProtocolEnv.CheckSignedEvaluateStream

        InitializeAjax()

        If Not IsPostBack Then
            Dim email As IMail = GetPecMailError("entity_event", "PECMailErrorStream")

            'Populate details
            SetPECStreamErrorDetails(email)

            'Populate attachment grid
            SetPECStreamErrorAttachments(email)
        End If
    End Sub

    Private Sub DocumentListGridItemDataBound(sender As Object, e As GridItemEventArgs) Handles DocumentListGrid.ItemDataBound
        If e.Item.ItemType = GridItemType.Item OrElse e.Item.ItemType = GridItemType.AlternatingItem Then
            Dim bound As DocumentInfo = DirectCast(e.Item.DataItem, DocumentInfo)
            Dim radio As RadioButton = CType(e.Item.FindControl("rbtMainDocument"), RadioButton)
            Dim txtPassword As HiddenField = CType(e.Item.FindControl("txtPassword"), HiddenField)
            Dim imgDecrypt As ImageButton = CType(e.Item.FindControl("imgDecrypt"), ImageButton)
            Dim btnUnzip As Button = CType(e.Item.FindControl("btnUnzip"), Button)

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
            If FileHelper.MatchExtension(bound.Name, FileHelper.ZIP) OrElse FileHelper.MatchExtension(bound.Name, FileHelper.RAR) Then
                Try
                    Dim compressManager As ICompress = New ZipCompress()
                    If FileHelper.MatchExtension(bound.Name, FileHelper.RAR) Then
                        compressManager = New RarCompress()
                    End If
                    If compressManager.HasPassword(bound.Stream) Then
                        imgDecrypt.Visible = True
                        imgDecrypt.OnClientClick = String.Format("showDialogInitially('{0}','{1}');return false;", txtPassword.ClientID, btnUnzip.ClientID)
                        btnUnzip.Visible = True
                        If (TypeOf bound Is BiblosDocumentInfo) Then
                            btnUnzip.CommandArgument = DirectCast(bound, BiblosDocumentInfo).DocumentId.ToString()
                        End If
                    End If
                Catch ex As ExtractException
                    AjaxAlert(EXTRACT_COMPRESSFILE_ERROR)
                End Try
            End If
            radio.Attributes.Add("onclick", String.Format("javascript:MutuallyExclusive(this, {0})", e.Item.RowIndex))
        End If

        If e.Item.ItemType = GridItemType.Header Then
            CType(CType(e.Item, GridHeaderItem)("Select").Controls(0), CheckBox).ToolTip = "Selezione documenti da Protocollare."
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(DocumentListGrid, DocumentListGrid)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, panelPEC, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlButtons, MasterDocSuite.AjaxDefaultLoadingPanel)

        If ProtocolEnv.TemplateProtocolEnable Then
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, pnlButtons)
        End If

        AjaxManager.AjaxSettings.AddAjaxSetting(rblDocumentUnit, pnlButtons)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblTypeSender, radAjaxPnlSender, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(chkInterop, radAjaxPnlSender, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(chkInteropMittente, radAjaxPnlSender, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(chbEMailCertified, radAjaxPnlSender, MasterDocSuite.AjaxDefaultLoadingPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(chkInteropMittente, pnlInterop, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(chkInterop, pnlInterop, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblDocumentUnit, pnlTemplateProtocol, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblDocumentUnit, pnlUDSSelect, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.ClientEvents.OnResponseEnd = "responseEnd"
    End Sub

    Private Function GetPecMailError(topicName As String, subscriptionName As String) As IMail
        Dim builder As MailBuilder = New MailBuilder()
        Dim email As IMail = builder.Create()

        Dim finder As EventPECStreamErrorFinder = New EventPECStreamErrorFinder(DocSuiteContext.Current.Tenants) With {
            .topicName = topicName,
            .subscriptionName = subscriptionName,
            .CorrelationId = Request.QueryString("CorrelatedId"),
            .PageSize = 50
        }
        Dim result As ICollection(Of WebAPIDto(Of ServiceBusTopicMessage)) = finder.DoSearchHeader()
        Dim eventErrorStreamPecMail As EventErrorStreamPECMail = JsonConvert.DeserializeObject(Of EventErrorStreamPECMail)(result(0).Entity.Content, DocSuiteContext.DefaultWebAPIJsonSerializerSettings)

        email = builder.CreateFromEml(eventErrorStreamPecMail.ContentType.ContentTypeValue.Stream)
        Dim test As IMail = builder.Create()
        Return email
    End Function

    Private Sub SetPECStreamErrorDetails(email As IMail)
        Dim lblFrom As Label = CType(uscPECInfo.FindControl("lblFrom"), Label)
        Dim addresses As New StringBuilder()
        For i As Integer = 0 To email.From.Count - 1
            addresses.Append(email.From(i).Address).
                Append(", ")
        Next
        addresses.Remove(addresses.Length - 2, 2)
        lblFrom.Text = addresses.ToString()
        Dim lblTo As Label = CType(uscPECInfo.FindControl("lblTo"), Label)
        addresses = New StringBuilder()
        For i As Integer = 0 To email.To.Count - 1
            For j As Integer = 0 To email.To(i).GetMailboxes().Count - 1
                addresses.Append(email.To(i).GetMailboxes()(j).Address).
                    Append(", ")
            Next
        Next
        addresses.Remove(addresses.Length - 2, 2)
        lblTo.Text = addresses.ToString()
        Dim lblMailBox As Label = CType(uscPECInfo.FindControl("lblMailBox"), Label)
        lblMailBox.Text = lblTo.Text
        Dim lblDate As Label = CType(uscPECInfo.FindControl("lblDate"), Label)
        lblDate.Text = email.Date.ToString()
        Dim lblSubject As Label = CType(uscPECInfo.FindControl("lblSubject"), Label)
        lblSubject.Text = email.Subject
    End Sub

    Private Sub SetPECStreamErrorAttachments(email As IMail)
        Dim documents As IList(Of DocumentInfo) = New List(Of DocumentInfo)()
        Dim doc As MemoryDocumentInfo
        For i As Integer = 0 To email.Attachments.Count - 1
            doc = New MemoryDocumentInfo(email.Attachments(i).Data, email.Attachments(i).FileName)
            documents.Add(doc)
        Next
        DocumentListGrid.DataSource = documents
        DocumentListGrid.DataBind()
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

        tor.Subject = CheckPecSubject()

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
        If ProtocolEnv.SelectSenderFromTreeviewEnabled AndAlso (UseContactType) Then
            contacts = uscMittenti.GetSelectedContacts()
        Else
            contacts = uscMittenti.GetContacts(False)
        End If
        If Not contacts.IsNullOrEmpty() Then
            tor.Senders = contacts.ToList()
        End If

        Return tor
    End Function

    Private Function CheckPecSubject() As String
        If (SessionPECMail IsNot Nothing) Then
            Return SessionPECMail.MailSubject
        Else
            Return CurrentPecMail.MailSubject
        End If
    End Function

    Protected Sub ChangeTypeContact(sender As Object, e As EventArgs) Handles rblTypeSender.SelectedIndexChanged
        Dim email As IMail = GetPecMailError("entity_event", "PECMailErrorStream")
        UpdateContact(email)
    End Sub

    Private Sub UpdateContact(email As IMail)
        uscMittenti.SimpleMode = True
        Select Case rblTypeSender.SelectedValue
            Case "1" ' Aggiungere contatto manuale
                uscMittenti.EnableCheck = False
                If ProtocolEnv.SelectSenderFromTreeviewEnabled AndAlso UseSenderInterop Then
                    uscMittenti.EnableCheck = True
                End If
                uscMittenti.DataSource = GetManualContacts(email)
                uscMittenti.SimpleMode = False
            Case "2" ' Contatto da rubrica
                ' Verifico se esiste un contatto in rubrica
                uscMittenti.EnableCheck = False
                If ProtocolEnv.SelectSenderFromTreeviewEnabled AndAlso (UseSenderInterop OrElse UseContactType) Then
                    uscMittenti.EnableCheck = True
                End If
                uscMittenti.DataSource = GetAddressContacts(email)
        End Select
        uscMittenti.DataBind()
    End Sub

    Public Function GetAddressContacts(email As IMail) As List(Of ContactDTO)
        If Me.UseInterop AndAlso chkInteropMittente.Checked Then
            'TODO: Gestire il file segnegnature per proporre all'utente diversi opzioni di MITTENTE.
            '      Generare un modale di proposta. Alla scelta di UN UNICO MITTENTE confermare e salvare i dati.
            '      Verificare i campi SQL per standardizzare la lunghezza del campo Description e EmailAddress.
            Return SegnaturaReader.GetMittenteForAddressBook(True)
        End If

        Dim mailSenders As New StringBuilder()
        For i As Integer = 0 To email.From.Count - 1
            mailSenders.Append(email.From(i).Address).
                Append(", ")
        Next
        mailSenders.Remove(mailSenders.Length - 2, 2)

        Dim mailList As List(Of String) = RegexHelper.MatchEmail(mailSenders.ToString()).AsList()
        Dim contacts As IList(Of Contact) = FacadeFactory.Instance.ContactFacade.GetContactWithCertifiedAndClassicEmail(mailList.ToArray())
        Return contacts.Select(Function(c) New ContactDTO(c, ContactDTO.ContactType.Address)).ToList()
    End Function

    Public Function GetManualContacts(email As IMail) As List(Of ContactDTO)
        If Me.UseInterop AndAlso chkInteropMittente.Checked Then
            Return Me.SegnaturaReader.GetMittenteForManual()
        End If

        Dim mailSenders As New StringBuilder()
        For i As Integer = 0 To email.From.Count - 1
            mailSenders.Append(email.From(i).Address).
                Append(", ")
        Next
        mailSenders.Remove(mailSenders.Length - 2, 2)

        Dim emailAddress As String = RegexHelper.MatchEmail(mailSenders.ToString())
        Dim description As String = RegexHelper.MatchName(mailSenders.ToString())
        If String.IsNullOrWhiteSpace(description) Then
            description = emailAddress
        End If

        Return MailFacade.CreateManualContact(description, emailAddress, Data.ContactType.Aoo, True, chbEMailCertified.Checked).AsList()
    End Function

    Private Sub btnProtocol_Click(sender As Object, e As EventArgs) Handles btnProtocol.Click
        Dim params As New StringBuilder()
        params.Append("Type=Prot&Action=PECError")
        params.AppendFormat("&ProtocolBox={0}", ProtocolBoxEnabled)
        If ProtocolEnv.TemplateProtocolEnable Then
            If Not String.IsNullOrEmpty(ddlTemplateProtocol.SelectedValue) Then
                params.AppendFormat("&IdTemplateProtocol={0}", ddlTemplateProtocol.SelectedValue)
            End If
        End If
        Response.Redirect(String.Format("../Prot/ProtInserimento.aspx?{0}", CommonShared.AppendSecurityCheck(params.ToString())))
    End Sub
#End Region
End Class