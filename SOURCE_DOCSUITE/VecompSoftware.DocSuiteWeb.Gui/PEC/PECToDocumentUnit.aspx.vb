Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Collections.Specialized
Imports System.IO
Imports VecompSoftware.DocSuiteWeb.Facade.ExtensionMethods
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports VecompSoftware.Services.Biblos
Imports System.Web
Imports VecompSoftware.DocSuiteWeb.Gui.Viewers
Imports VecompSoftware.Helpers.Web
Imports VecompSoftware.DocSuiteWeb.Data.OrganizationChart.xml
Imports VecompSoftware.DocSuiteWeb.Facade.PEC
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.UDS
Imports VecompSoftware.Helpers.UDS
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.Helpers.Compress
Imports VecompSoftware.DocSuiteWeb.BusinessRule.Rules.Rights.UDS
Imports VecompSoftware.DocSuiteWeb.Entity.Fascicles
Imports VecompSoftware.DocSuiteWeb.DTO.Commons

Public Class PECToDocumentUnit
    Inherits PECBasePage
    Implements IProtocolInitializer
    Implements IUDSInitializer

#Region " Fields "

    Private _whiteList() As String
    Private _blackList() As String
    Private _allDocuments As List(Of DocumentInfo)
    Private _oChartItemData As OChartProtocolXml
    Private _segnaturaReader As SegnaturaReader
    Private _mailboxes As IList(Of PECMailBox)
    Private _currentUDSRepositoryFacade As UDSRepositoryFacade
    Private _currentFascicleFolderFinder As Data.WebAPI.Finder.Fascicles.FascicleFolderFinder

    Private Const LOG_WORKFLOW_ACTIVITY As String = "pecToDocumentUnit.insertWorkflowActivity({0},{1},{2});"
    Private Const CONFIRM_CALLBACK As String = "pecToDocumentUnit.confirmCallback('{0}','{1}',{2},'{3}');"
    Private Const INSERT_MISCELLANEA As String = "InsertMiscellanea"
    Private Const RADIO_ON_CHANGE As String = "radioButton_OnClick();"
#End Region

#Region " Properties "
    Public Property SessionPECMail As PECMail
        Get
            If Session("PECToDocumentUnit_SessionPECMail") IsNot Nothing Then
                Return DirectCast(Session("PECToDocumentUnit_SessionPECMail"), PECMail)
            End If
            Return Nothing
        End Get
        Set(value As PECMail)
            Session("PECToDocumentUnit_SessionPECMail") = value
        End Set
    End Property
    Public ReadOnly Property CurrentMailBox As PECMailBox
        Get
            If Not String.IsNullOrEmpty(ddlMailbox.SelectedValue) AndAlso Not ddlMailbox.SelectedValue.Eq("ALL") Then
                Return Facade.PECMailboxFacade.GetById(Short.Parse(ddlMailbox.SelectedValue))
            End If
            Return Nothing
        End Get
    End Property

    ''' <summary> Elenco delle mailbox visibili all'utente </summary>
    Public ReadOnly Property MailBoxes As IList(Of PECMailBox)
        Get
            If _mailboxes Is Nothing Then
                _mailboxes = New List(Of PECMailBox)
            End If

            If _mailboxes.Count = 0 Then
                _mailboxes = Facade.PECMailboxFacade.GetHumanManageable().GetVisibleProtocolMailBoxes()
            End If

            Return _mailboxes
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

    Public ReadOnly Property OChartItemData As OChartProtocolXml
        Get
            If _oChartItemData Is Nothing Then
                _oChartItemData = Facade.OChartFacade.GetOChartItem(CurrentPecMailWrapper)
            End If
            Return _oChartItemData
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

    Public ReadOnly Property UseInterop() As Boolean
        Get
            Return Not String.IsNullOrEmpty(CurrentPecMail.Segnatura) AndAlso chkInterop.Checked AndAlso Not SegnaturaReader.HasErrors
        End Get
    End Property

    Public ReadOnly Property UseSenderInterop() As Boolean
        Get
            Return UseInterop AndAlso chkInteropMittente.Checked
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


    Public ReadOnly Property hasValidInterop() As Boolean
        Get
            Return Not String.IsNullOrEmpty(CurrentPecMail.Segnatura) AndAlso Not SegnaturaReader.HasErrors
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


    Public ReadOnly Property CurrentFascicleFolderFinder() As Data.WebAPI.Finder.Fascicles.FascicleFolderFinder
        Get
            If _currentFascicleFolderFinder Is Nothing Then
                _currentFascicleFolderFinder = New Data.WebAPI.Finder.Fascicles.FascicleFolderFinder(DocSuiteContext.Current.CurrentTenant)
                _currentFascicleFolderFinder.EnablePaging = False
            End If
            Return _currentFascicleFolderFinder
        End Get
    End Property
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjaxHandlers()

        uscPECInfo.PecLabel = PecLabel.ToLower()
        lblWarning.Visible = Not DocSuiteContext.Current.ProtocolEnv.EnableDictionDocumentSign AndAlso Not DocSuiteContext.Current.ProtocolEnv.CheckSignedEvaluateStream

        InitializeAjax()

        If Not IsPostBack Then
            SessionPECMail = Nothing
            Title = String.Format("{0} - Gestisci", PecLabel)
            If CurrentPecMail Is Nothing AndAlso Request.QueryString.HasKeys AndAlso "True".Eq(Request.QueryString("PecFromFile")) Then
                InitializeMailboxes()
                Form.Attributes("class") = String.Concat(Form.Attributes("class"), " protocolBox")
                panelPECFromFile.Visible = True
                panelPEC.Visible = False
                cmdInit.Enabled = False
                cmdInitAndClone.Enabled = False
                cmdAnnulla.Enabled = False
                Return
            End If
            If (PreviousPage Is Nothing) OrElse Not (TypeOf PreviousPage Is PECBasePage) OrElse Not PreviousPage.IsCrossPagePostBack Then
                Throw New DocSuiteException("Pec to Protocol", "Pagina di provenienza mancante o non corretta.")
            End If

            InitializePecToProtocol()
        End If
    End Sub
    Private Sub InitializeAjaxHandlers()
        AddHandler AjaxManager.AjaxRequest, AddressOf pecToDocumentUnit_AjaxRequest
    End Sub

    Private Sub DocumentListGridCommandDataBound(sender As Object, e As GridCommandEventArgs) Handles DocumentListGrid.ItemCommand
        If e.CommandName.Eq("unzip") AndAlso e.CommandArgument IsNot Nothing AndAlso Not String.IsNullOrEmpty(e.CommandArgument.ToString()) Then
            Dim documentId As Guid = Guid.Parse(e.CommandArgument.ToString())
            Dim radio As RadioButton = CType(e.Item.FindControl("rbtMainDocument"), RadioButton)
            Dim txtPassword As HiddenField = CType(e.Item.FindControl("txtPassword"), HiddenField)
            Dim btnUnzip As Button = CType(e.Item.FindControl("btnUnzip"), Button)
            If radio IsNot Nothing Then
                Dim key As String = DocumentListGrid.Items(e.Item.ItemIndex).GetDataKeyValue("Serialized").ToString()
                Dim temp As NameValueCollection = HttpUtility.ParseQueryString(key, Encoding.UTF8)
                Dim zipDoc As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(temp)
                If DocumentsSource Is Nothing Then
                    AjaxAlert("Errore nella gestione degli allegati.")
                    Exit Sub
                End If
                Dim documenti As List(Of DocumentInfo) = DocumentsSource
                Try
                    Dim compressManager As ICompress = New ZipCompress()
                    If FileHelper.MatchExtension(zipDoc.Name, FileHelper.RAR) Then
                        compressManager = New RarCompress()
                    End If

                    Dim extracted As List(Of CompressItem) = compressManager.InMemoryExtract(New MemoryStream(zipDoc.Stream), password:=txtPassword.Value)
                    For Each item As CompressItem In extracted
                        Dim memoryInfo As New MemoryDocumentInfo(item.Data, item.Filename)
                        Dim file As New TempFileDocumentInfo(BiblosFacade.SaveUniqueToTemp(memoryInfo))
                        file.Name = item.Filename
                        documenti.Add(file)
                    Next

                    Dim results As IList(Of DocumentInfo) = BonificaDocumenti(documenti)
                    Dim zip As DocumentInfo = results.FirstOrDefault(Function(f) (TypeOf f Is BiblosDocumentInfo) AndAlso DirectCast(f, BiblosDocumentInfo).DocumentId = documentId)
                    If zip IsNot Nothing Then
                        results.Remove(zip)
                    End If
                    DocumentListGrid.DataSource = results.ToList()
                    DocumentListGrid.DataBind()
                Catch ex As BadPasswordException
                    AjaxAlert("Password errata. Specificare una nuova password.")
                Catch ex As ExtractException
                    AjaxAlert(EXTRACT_COMPRESSFILE_ERROR)
                End Try
            End If
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

    Protected Sub ChangeTypeContact(sender As Object, e As EventArgs) Handles rblTypeSender.SelectedIndexChanged
        If (SessionPECMail IsNot Nothing) Then
            CurrentPecMail.MailRecipients = SessionPECMail.MailRecipients
            CurrentPecMail.MailRecipientsCc = SessionPECMail.MailRecipientsCc
            CurrentPecMail.MailSenders = SessionPECMail.MailSenders
        End If
        UpdateContact()
    End Sub

    Private Sub ChkUseSegnatureCheckedChanged(sender As Object, e As EventArgs) Handles chkInteropMittente.CheckedChanged
        chbEMailCertified.Enabled = Not chkInteropMittente.Checked
        UpdateContact()
    End Sub

    Private Sub ChbEMailCertifiedCheckedChanged(sender As Object, e As EventArgs) Handles chbEMailCertified.CheckedChanged
        UpdateContact()
    End Sub

    Private Sub ChkUseOChartCheckedChanged(sender As Object, e As EventArgs) Handles chkUseOChart.CheckedChanged
    End Sub

    Private Sub UscUploadDocumentiDocumentUploaded(sender As Object, e As DocumentEventArgs) Handles uscUploadDocumenti.DocumentUploaded
        If Not FileHelper.MatchExtension(e.Document.Name, FileHelper.EML) AndAlso Not FileHelper.MatchExtension(e.Document.Name, FileHelper.MSG) Then
            AjaxAlert("E' possibile protocollare PEC provenienti da file con le sole estensioni EML o MSG")
            uscUploadDocumenti.RemoveDocumentInfo(e.Document)
            Return
        End If

        If ddlMailbox.SelectedItem IsNot Nothing AndAlso Not "invalid".Eq(ddlMailbox.SelectedItem.Value) Then
            CreatePecMailFromFile(e.Document)
        End If
    End Sub
    Protected Sub ddlMailbox_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlMailbox.SelectedIndexChanged

        If ddlMailbox.SelectedItem IsNot Nothing AndAlso Not "invalid".Eq(ddlMailbox.SelectedItem.Value) AndAlso uscUploadDocumenti.SelectedDocumentInfo IsNot Nothing Then
            CreatePecMailFromFile(uscUploadDocumenti.SelectedDocumentInfo)
        End If
    End Sub
    Private Sub chkInterop_CheckedChanged(sender As Object, e As EventArgs) Handles chkInterop.CheckedChanged
        chkInteropMittente.Enabled = chkInterop.Checked

        If Not chkInteropMittente.Enabled Then
            chkInteropMittente.Checked = False
        End If

        chbEMailCertified.Enabled = Not chkInteropMittente.Checked

        UpdateContact()
    End Sub

    Protected Sub ddlTemplateProtocol_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlTemplateProtocol.SelectedIndexChanged
        Dim protocolParams As New StringBuilder()
        protocolParams.Append("Type=Prot&Action=Interop")
        protocolParams.AppendFormat("&IdPECMail={0}", CurrentPecMail.Id)
        protocolParams.AppendFormat("&ProtocolBox={0}", ProtocolBoxEnabled)

        If Not String.IsNullOrEmpty(ddlTemplateProtocol.SelectedValue) Then
            protocolParams.AppendFormat("&IdTemplateProtocol={0}", ddlTemplateProtocol.SelectedValue)
        End If

        Dim protUrl As String = String.Format("../Prot/ProtInserimento.aspx?{0}", CommonShared.AppendSecurityCheck(protocolParams.ToString()))
        Dim protCloneUrl As String = String.Format("../Prot/ProtInserimento.aspx?{0}&needClone=1", CommonShared.AppendSecurityCheck(protocolParams.ToString()))
        cmdInit.PostBackUrl = protUrl
        cmdInitAndClone.PostBackUrl = protCloneUrl
        AjaxManager.ResponseScripts.Add(String.Format(RADIO_ON_CHANGE))
    End Sub

    Protected Sub rblDocumentUnit_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblDocumentUnit.SelectedIndexChanged
        Dim selectedEnv As DSWEnvironment = DirectCast([Enum].Parse(GetType(DSWEnvironment), rblDocumentUnit.SelectedValue), DSWEnvironment)
        Dim causesValidation As Boolean = False
        Dim onClientClickAction As String = "ShowLoadingPagePanel();"

        Select Case selectedEnv
            Case DSWEnvironment.Protocol
                If (ProtocolEnv.PECClone) Then
                    cmdInitAndClone.Style.Add("display", "inline")
                Else
                    cmdInitAndClone.Style.Add("display", "none")
                End If
                ddlTemplateProtocol_SelectedIndexChanged(Me, New EventArgs())

            Case DSWEnvironment.UDS
                If (ProtocolEnv.PECClone) Then
                    cmdInitAndClone.Style.Add("display", "inline")
                Else
                    cmdInitAndClone.Style.Add("display", "none")
                End If
                causesValidation = True
                onClientClickAction = "onArchiveClick();"
                ddlUDSArchives_SelectedIndexChanged(Me, New RadComboBoxSelectedIndexChangedEventArgs(String.Empty, String.Empty, String.Empty, String.Empty))

            Case DSWEnvironment.Fascicle
                cmdInit.PostBackUrl = Nothing
                cmdInitAndClone.PostBackUrl = Nothing
                cmdInitAndClone.Style.Add("display", "none")
                onClientClickAction = "return onFascicleMiscellaneaClick();"
                AjaxManager.ResponseScripts.Add(String.Format(RADIO_ON_CHANGE))

        End Select

        cmdInit.CausesValidation = causesValidation
        cmdInit.OnClientClick = onClientClickAction
        cmdInitAndClone.CausesValidation = causesValidation
        cmdInitAndClone.OnClientClick = onClientClickAction
    End Sub

    Private Sub cmdAnnullaClick(sender As Object, e As EventArgs) Handles cmdAnnulla.Click
        If Not String.IsNullOrEmpty(PreviousPageUrl) Then
            Response.Redirect(PreviousPageUrl)
        Else
            cmdAnnulla.Enabled = False
        End If

    End Sub

    Private Function SaveBiblosDocument(document As DocumentInfo, chainId As Guid) As BiblosDocumentInfo
        Dim fascMiscellaneaLocation As Location = Facade.LocationFacade.GetById(ProtocolEnv.FascicleMiscellaneaLocation)
        If fascMiscellaneaLocation.DocumentServer Is Nothing Then
            AjaxAlert("Nessun location presente con questo ID. Errore di configurazione, contattatare Assistenza.")
            Exit Function
        End If
        If String.IsNullOrEmpty(fascMiscellaneaLocation.ProtBiblosDSDB) Then
            AjaxAlert("Nessun archivio defininito. Errore di configurazione, contattatare Assistenza.")
            Exit Function
        End If
        Dim storedBiblosDocumentInfo As BiblosDocumentInfo = document.ArchiveInBiblos(fascMiscellaneaLocation.DocumentServer, fascMiscellaneaLocation.ProtBiblosDSDB, chainId)
        Return storedBiblosDocumentInfo
    End Function
    Protected Sub pecToDocumentUnit_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch
            Exit Sub
        End Try

        If ajaxModel Is Nothing Then
            Return
        End If

        If String.Equals(ajaxModel.ActionName, INSERT_MISCELLANEA) AndAlso ajaxModel.Value IsNot Nothing Then
            Dim idFascicle As Guid = Guid.Parse(ajaxModel.Value(0))
            InsertFascicleMiscellanea(idFascicle)
        End If
    End Sub

    Protected Sub ddlUDSArchives_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs) Handles ddlUDSArchives.SelectedIndexChanged
        LoadDdlUDSContactTypes()

        Dim udsParams As New StringBuilder()
        udsParams.Append("Type=UDS&Action=Insert")
        udsParams.AppendFormat("&IdPECMail={0}", CurrentPecMail.Id)
        udsParams.AppendFormat("&ArchiveTypeId={0}", ddlUDSArchives.SelectedValue)

        Dim udsUrl As String = String.Format("../UDS/UDSInsert.aspx?{0}", CommonShared.AppendSecurityCheck(udsParams.ToString()))
        Dim udsCloneUrl As String = String.Format("../UDS/UDSInsert.aspx?{0}&needClone=1", CommonShared.AppendSecurityCheck(udsParams.ToString()))
        cmdInit.PostBackUrl = udsUrl
        cmdInitAndClone.PostBackUrl = udsCloneUrl
        AjaxManager.ResponseScripts.Add(String.Format(RADIO_ON_CHANGE))
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeMailboxes()
        ddlMailbox.Items.Clear()
        ddlMailbox.Items.Add(New ListItem("Selezione una Casella di protocollazione", "invalid"))
        For Each mailbox As PECMailBox In MailBoxes
            If Facade.PECMailboxFacade.IsRealPecMailBox(mailbox) Then
                ddlMailbox.Items.Add(New ListItem(Facade.PECMailboxFacade.MailBoxRecipientLabel(mailbox), mailbox.Id.ToString()))
            End If
        Next

        'Definisco il titolo in modo personalizzato
        ddlMailBoxLabel.InnerText = String.Format("Casella {0}", If(ProtocolBoxEnabled, "e-mail:", "PEC:"))
    End Sub

    Private Sub UpdateContact()
        uscMittenti.SimpleMode = True
        Select Case rblTypeSender.SelectedValue
            Case "0" ' Non riportare mittente
                pnlUDSContact.Visible = False
            Case "1" ' Aggiungere contatto manuale
                uscMittenti.EnableCheck = False
                If ProtocolEnv.SelectSenderFromTreeviewEnabled AndAlso UseSenderInterop Then
                    uscMittenti.EnableCheck = True
                End If
                uscMittenti.DataSource = GetManualContacts()
                uscMittenti.SimpleMode = False
                pnlUDSContact.Visible = DirectCast([Enum].Parse(GetType(DSWEnvironment), rblDocumentUnit.SelectedValue), DSWEnvironment) = DSWEnvironment.UDS
            Case "2" ' Contatto da rubrica
                ' Verifico se esiste un contatto in rubrica
                uscMittenti.EnableCheck = False
                If ProtocolEnv.SelectSenderFromTreeviewEnabled AndAlso (UseSenderInterop OrElse UseContactType) Then
                    uscMittenti.EnableCheck = True
                End If
                uscMittenti.DataSource = GetAddressContacts()
                pnlUDSContact.Visible = DirectCast([Enum].Parse(GetType(DSWEnvironment), rblDocumentUnit.SelectedValue), DSWEnvironment) = DSWEnvironment.UDS
        End Select
        uscMittenti.DataBind()
    End Sub

    Public Function GetManualContacts() As List(Of ContactDTO)
        If Me.UseInterop AndAlso chkInteropMittente.Checked Then
            Return Me.SegnaturaReader.GetMittenteForManual()
        End If
        Dim pecMail As PECMail = CurrentPecMail
        If SessionPECMail IsNot Nothing Then
            pecMail = SessionPECMail
        End If
        Dim emailAddress As String = RegexHelper.MatchEmail(pecMail.MailSenders)
        Dim description As String = RegexHelper.MatchName(pecMail.MailSenders)
        If String.IsNullOrWhiteSpace(description) Then
            description = emailAddress
        End If

        Return MailFacade.CreateManualContact(description, emailAddress, Data.ContactType.Aoo, True, chbEMailCertified.Checked).AsList()
    End Function

    Public Function GetAddressContacts() As List(Of ContactDTO)
        If Me.UseInterop AndAlso chkInteropMittente.Checked Then
            'TODO: Gestire il file segnegnature per proporre all'utente diversi opzioni di MITTENTE.
            '      Generare un modale di proposta. Alla scelta di UN UNICO MITTENTE confermare e salvare i dati.
            '      Verificare i campi SQL per standardizzare la lunghezza del campo Description e EmailAddress.
            Return SegnaturaReader.GetMittenteForAddressBook(True)
        End If

        Dim pecMail As PECMail = CurrentPecMail
        If SessionPECMail IsNot Nothing Then
            pecMail = SessionPECMail
        End If

        Dim mailList As List(Of String) = RegexHelper.MatchEmail(pecMail.MailSenders).AsList()
        Dim contacts As IList(Of Contact) = FacadeFactory.Instance.ContactFacade.GetContactWithCertifiedAndClassicEmail(mailList.ToArray())
        Return contacts.Select(Function(c) New ContactDTO(c, ContactDTO.ContactType.Address)).ToList()
    End Function

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(DocumentListGrid, WarningPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(DocumentListGrid, DocumentListGrid)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlMailbox, panelPECFromFile)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlMailbox, panelPEC)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscUploadDocumenti)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, panelPECFromFile, MasterDocSuite.AjaxDefaultLoadingPanel)

        If ProtocolEnv.TemplateProtocolEnable Then
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlTemplateProtocol, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        End If

        AjaxManager.AjaxSettings.AddAjaxSetting(ddlUDSArchives, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblDocumentUnit, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblTypeSender, radAjaxPnlSender, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(chkInterop, radAjaxPnlSender, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(chkInteropMittente, radAjaxPnlSender, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(chbEMailCertified, radAjaxPnlSender, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(chkInteropMittente, pnlInterop, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(chkInterop, pnlInterop, MasterDocSuite.AjaxDefaultLoadingPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(rblTypeSender, pnlUDSContact)

        AjaxManager.ClientEvents.OnResponseEnd = "responseEnd"
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

    Private Sub CheckDocuments()
        ' AREA LOG
        Dim warn As New StringBuilder()

        If DocumentListGrid.Items.Count = 0 Then
            ' Nessun documento selezionato
            If warn.Length > 0 Then
                warn.Append(WebHelper.Br)
            End If
            warn.Append("ATTENZIONE: Nessun documento selezionato.")
        ElseIf String.IsNullOrEmpty(CType(DocumentListGrid.SelectedValue, String)) Then
            ' Nessun documento selezionato
            If warn.Length > 0 Then
                warn.Append(WebHelper.Br)
            End If
            warn.Append("ATTENZIONE: Selezionare il documento principale.")
        End If

        If warn.Length > 0 Then
            WarningLabel.Text = warn.ToString()
            WarningPanel.CssClass = "warningArea"
        Else
            WarningLabel.Text = String.Empty
            WarningPanel.CssClass = "hiddenField"
        End If

    End Sub

    Protected Function GetNotes(name As String) As String
        Dim v As Boolean? = CheckDocument(name)
        If v.HasValue Then
            If v.Value Then
                Return "OK"
            Else
                Return "Impossibile protocollare questo tipo di documento."
            End If
        Else
            Return "Documento non previsto"
        End If
    End Function

    Protected Sub ToggleRowSelection(ByVal sender As Object, ByVal e As EventArgs)
        CType(CType(sender, CheckBox).NamingContainer, GridItem).Selected = CType(sender, CheckBox).Checked
        CheckDocuments()
    End Sub

    Public Function GetProtocolInitializer() As ProtocolInitializer Implements IProtocolInitializer.GetProtocolInitializer
        Dim tor As New ProtocolInitializer()

        Dim allSelectedDocument As List(Of DocumentInfo) = AllAttachmentDocuments
        Select Case True
            Case Me.chkUseOChart.Checked AndAlso Me.OChartItemData.OChartItem IsNot Nothing
                '' Oggetto
                tor.Subject = OChartItemData.ProtocolXmlData.ProtocolObject

                '' Note del protocollo
                tor.Notes = OChartItemData.ProtocolXmlData.Notes

                '' Classificazione (del mittente) del settore [match su fullCode]
                tor.Category = Facade.OChartFacade.GetCategory(OChartItemData)

                '' Dati del protocollo mittente
                '' FullNumber
                tor.SenderProtocolNumber = String.Format("{0}/{1:0000000}", OChartItemData.ProtocolXmlData.Year, OChartItemData.ProtocolXmlData.Number)
                '' Data di registrazione
                tor.SenderProtocolDate = OChartItemData.ProtocolXmlData.ProtocolDate
                '' Tipo di documento
                tor.DocumentTypeLabel = OChartItemData.ProtocolXmlData.DocumentType.Description

                '' Gestione documenti
                '' Documento principale                
                tor.MainDocument = Facade.OChartFacade.GetMainDocuments(OChartItemData, allSelectedDocument).FirstOrDefault()
                '' Allegati
                tor.Attachments = Facade.OChartFacade.GetAttachments(OChartItemData, allSelectedDocument)
                '' Annessi
                tor.Annexed = Facade.OChartFacade.GetAnnexed(OChartItemData, allSelectedDocument)

                '' Dati legati all'OChart
                '' Contenitori legati all'OChart
                tor.Containers = OChartItemData.OChartItem.Containers.Select(Function(e) e.Container).ToList()
                '' Destinatari legati all'OChart
                tor.Recipients = OChartItemData.OChartItem.Contacts.Select(Function(e) New ContactDTO(e.Contact, ContactDTO.ContactType.Address)).ToList()
                '' Settori legati all'OChart
                tor.Roles = OChartItemData.OChartItem.Roles.Select(Function(e) e.Role).ToList()



            Case Me.UseInterop
                tor.Subject = CheckInteropSubject(SegnaturaReader)
                tor.SenderProtocolNumber = SegnaturaReader.GetNumeroRegistrazione()
                tor.SenderProtocolDate = SegnaturaReader.GetDataRegistrazione()
            Case Else
                tor.Subject = CheckPecSubject()
                If ProtocolEnv.EnableDocumentDateFromPEC Then
                    tor.SenderProtocolDate = If(SessionPECMail IsNot Nothing, SessionPECMail.MailDate, CurrentPecMail.MailDate)
                End If
        End Select

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
        If ProtocolEnv.SelectSenderFromTreeviewEnabled AndAlso (UseSenderInterop OrElse UseContactType) Then
            contacts = uscMittenti.GetSelectedContacts()
        Else
            contacts = uscMittenti.GetContacts(False)
        End If
        If Not contacts.IsNullOrEmpty() Then
            tor.Senders = contacts.ToList()
        End If

        Return tor
    End Function

    Private Function CheckInteropSubject(ByVal signatureReader As SegnaturaReader) As String
        Dim subject As String = signatureReader.GetOggetto()
        If (String.IsNullOrEmpty(subject)) Then
            Return CheckPecSubject()
        Else
            Return subject
        End If
    End Function

    Private Function CheckPecSubject() As String
        If SessionPECMail IsNot Nothing Then
            Return SessionPECMail.MailSubject
        Else
            Return CurrentPecMail.MailSubject
        End If
    End Function

    Private Sub CreatePecMailFromFile(document As DocumentInfo)
        Dim pec As PECMail = Tools.PecMailFactory(document)
        pec.PECType = PECMailType.FromFile
        pec.Receipts = New List(Of PECMailReceipt)
        pec.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Disabled)
        pec.MailBox = CurrentMailBox
        If (pec.MailBox IsNot Nothing) Then
            pec.Location = pec.MailBox.Location
        End If
        Facade.PECMailFacade.Save(pec)
        Dim attachments As List(Of DocumentInfo) = Tools.GetAttachments(document)
        Dim errors As New List(Of String)
        For Each docInfo As DocumentInfo In attachments
            Try
                Facade.PECMailFacade.ArchiveAttachment(pec, docInfo, True)
            Catch ex As Exception
                errors.Add(String.Format("{0} -> [{1}]", docInfo.Name, ex.Message))
                FileLogger.Warn(LoggerName, String.Format("Errore in fase di caricamento allegato {0}", docInfo.ToString()))
            End Try
        Next
        Facade.PECMailFacade.UpdateNoLastChange(pec)
        CurrentPecMailId = pec.Id
        InitializePecToProtocol()
    End Sub

    Private Function BonificaDocumenti(documents As IList(Of DocumentInfo)) As List(Of DocumentInfo)
        Dim bonificati As List(Of DocumentInfo) = New List(Of DocumentInfo)
        For Each doc As DocumentInfo In documents
            If FileHelper.MatchExtension(doc.Name, FileHelper.EML) AndAlso TypeOf doc Is BiblosDocumentInfo Then
                bonificati.Add(New BiblosPdfDocumentInfo(CType(doc, BiblosDocumentInfo)))
            Else
                bonificati.Add(doc)
            End If
        Next
        Return bonificati
    End Function

    Private Sub InitializePecToProtocol()
        panelPECFromFile.Visible = False
        panelPEC.Visible = True
        cmdAnnulla.Visible = True
        cmdInit.Enabled = True
        cmdAnnulla.Enabled = True

        Dim params As New StringBuilder()
        params.Append("Type=Prot&Action=Interop")
        params.AppendFormat("&IdPECMail={0}", CurrentPecMail.Id)
        params.AppendFormat("&ProtocolBox={0}", ProtocolBoxEnabled)
        If ProtocolEnv.TemplateProtocolEnable Then
            If Not String.IsNullOrEmpty(ddlTemplateProtocol.SelectedValue) Then
                params.AppendFormat("&IdTemplateProtocol={0}", ddlTemplateProtocol.SelectedValue)
            End If
        End If

        cmdInit.PostBackUrl = String.Format("../Prot/ProtInserimento.aspx?{0}", CommonShared.AppendSecurityCheck(params.ToString()))
        If (ProtocolEnv.PECClone) Then
            Dim protCloneUrl As String = String.Format("../Prot/ProtInserimento.aspx?{0}&needClone=1", CommonShared.AppendSecurityCheck(params.ToString()))
            cmdInitAndClone.PostBackUrl = protCloneUrl
            cmdInitAndClone.Style.Add("display", "inline")
        Else
            cmdInitAndClone.Style.Add("display", "none")
        End If

        Dim availableContainers As IList(Of Data.Container) = Facade.ContainerFacade.GetContainers(DSWEnvironment.UDS, UDSRightPositions.PECIngoing, True)
        pnlDocumentUnitSelect.Visible = ProtocolEnv.UDSEnabled AndAlso availableContainers.Count > 0
        pnlUDSContact.Visible = False

        If CommonShared.UserConnectedBelongsTo(ProtocolEnv.RenameAttachmentGroups) Then
            DocumentListGrid.MasterTableView.EditMode = GridEditMode.Batch
            DocumentListGrid.MasterTableView.GetColumn("Description").HeaderStyle.Width = New Unit(90, UnitType.Percentage)
            pnlRenameAttahmentWarning.CssClass = "successAreaLow"
        End If

        If hasValidInterop Then
            uscInteropInfo.Visible = True
            uscInteropInfo.Source = CurrentPecMail.Segnatura
        Else
            If Not String.IsNullOrEmpty(CurrentPecMail.Segnatura) Then
                WarningInteropLabel.Text = "La segnatura non è valida. Interoperabilità disabilitata."
                WarningInteropPanel.CssClass = "warningArea"
            Else
                WarningInteropLabel.Text = String.Empty
                WarningInteropPanel.CssClass = "hiddenField"
            End If
        End If

        Dim documents As IList(Of DocumentInfo) = InizializeDocuments()

        uscPECInfo.PECMail = CurrentPecMail

        If ProtocolBoxEnabled AndAlso documents IsNot Nothing AndAlso documents.Any() Then
            InitializeForwarderPecMail(documents)
        End If

        DocumentListGrid.DataSource = BonificaDocumenti(documents)

        ' Nome del documento principale
        Dim mainDocName As String
        If Not OChartItemData Is Nothing AndAlso Not OChartItemData.ProtocolXmlData Is Nothing AndAlso Not OChartItemData.ProtocolXmlData.MainDocuments.IsNullOrEmpty Then
            mainDocName = OChartItemData.ProtocolXmlData.MainDocuments.FirstOrDefault().Caption
        ElseIf UseInterop Then
            mainDocName = SegnaturaReader.GetNomeDocumentoPrincipale()
        Else
            mainDocName = ProtocolEnv.PECDefaultMainDocument
        End If

        DocumentListGrid.DataBind()
        ' Ciclo alla ricerca dell'eventuale documento principale
        For Each item As GridDataItem In DocumentListGrid.Items
            Dim key As String = item.GetDataKeyValue("Serialized").ToString()
            Dim temp As NameValueCollection = HttpUtility.ParseQueryString(key) 'qui non encoda gli escape
            Dim doc As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(temp)

            If String.IsNullOrEmpty(mainDocName) OrElse doc.Name.Eq(mainDocName) Then
                Dim radio As RadioButton = CType(item.FindControl("rbtMainDocument"), RadioButton)
                radio.Checked = True
                item.Selected = True
                Exit For
            End If
        Next

        ' Visualizzo la scelta da Segnatura solo se presente
        pnlInterop.Visible = hasValidInterop
        chkInterop.Checked = ProtocolEnv.SegnatureDefaultUseCheck
        chkInterop.Enabled = ProtocolEnv.SegnatureOptional
        chkInteropMittente.Checked = ProtocolEnv.SegnatureDefaultUseCheck
        chkInteropMittente.Enabled = ProtocolEnv.SegnatureOptional

        chbEMailCertified.Enabled = Not chkInteropMittente.Checked
        chbEMailCertified.Checked = (CurrentPecMail.PECType.GetValueOrDefault(PECMailType.Anomalia) = PECMailType.PEC)

        'Visualizzo la scelta da OChart solo se presente
        chkUseOChart.Visible = OChartItemData IsNot Nothing
        chkUseOChart.Checked = chkUseOChart.Visible

        CheckDocuments()

        If ProtocolBoxEnabled Then
            rblTypeSender.SelectedIndex = ProtocolEnv.ProtocolBoxToProtContact
        Else
            rblTypeSender.SelectedIndex = ProtocolEnv.PecToProtContact
        End If
        UpdateContact()

        pnlTemplateProtocol.Style.Add("display", "none")
        If ProtocolEnv.TemplateProtocolEnable Then
            Dim templates As IList(Of TemplateProtocol) = Facade.TemplateProtocolFacade.GetTemplates()
            If templates.Any() Then
                pnlTemplateProtocol.Style.Add("display", "block")
                ddlTemplateProtocol.Items.Add(New ListItem(String.Empty, String.Empty))
                For Each template As TemplateProtocol In templates.OrderBy(Function(o) o.TemplateName)
                    Dim item As ListItem = New ListItem(template.TemplateName, template.Id.ToString())
                    ddlTemplateProtocol.Items.Add(item)
                Next
            End If
        End If

        If ProtocolEnv.IsPECDestinationEnabled AndAlso CurrentPecMail.MailBox.IsDestinationEnabled.GetValueOrDefault(False) AndAlso Not String.IsNullOrEmpty(CurrentPecMail.DestinationNote) Then
            lblDestination.Text = String.Concat("NOTE DI DESTINAZIONE: ", CurrentPecMail.DestinationNote)
            pnlDestination.CssClass = "warningArea"
        End If

        If ProtocolEnv.UDSEnabled Then
            Dim archives As IList(Of UDSRepository) = New UDSRepositoryFacade(DocSuiteContext.Current.User.FullUserName).GetByPecEnabled(String.Empty)

            Dim autorizedArchives As IList(Of UDSRepository) = New List(Of UDSRepository)
            Dim currentRepositoryRigths As UDSRepositoryRightsUtil = Nothing
            Dim dto As UDSDto = Nothing
            For Each archive As UDSRepository In archives
                dto = New UDSDto() With {.UDSModel = UDSModel.LoadXml(archive.ModuleXML)}
                currentRepositoryRigths = New UDSRepositoryRightsUtil(archive, DocSuiteContext.Current.User.FullUserName, dto)
                If currentRepositoryRigths.IsArchiveableByPEC Then
                    autorizedArchives.Add(archive)
                End If
            Next

            ddlUDSArchives.DataSource = autorizedArchives
            ddlUDSArchives.DataBind()

            If autorizedArchives.Any() AndAlso autorizedArchives.Count() = 1 Then
                ddlUDSArchives.Items(0).Selected = True
                LoadDdlUDSContactTypes()
            Else
                ddlUDSArchives.Items.Insert(0, New RadComboBoxItem(String.Empty, String.Empty))
            End If
        End If
        ddlUDSArchives.Visible = True
    End Sub

    Private Function InizializeDocuments() As IList(Of DocumentInfo)
        Dim documents As IList(Of DocumentInfo) = New List(Of DocumentInfo)()

        If (Not PreviousPage Is Nothing AndAlso TypeOf PreviousPage Is IHaveViewerLight AndAlso PreviousPage.IsCrossPagePostBack) Then
            Dim docs As List(Of DocumentInfo) = CType(PreviousPage, IHaveViewerLight).CheckedDocuments
            If Not docs.Any() Then
                Return docs
            End If
            documents = Facade.PECMailFacade.GetDocumentList(docs)
        Else
            ' Nel caso non ci siano documenti sulla PreviousPage carico tutti i documenti della PEC
            documents = Facade.PECMailFacade.GetDocumentList(CurrentPecMailWrapper)
        End If
        Return documents
    End Function

    Private Sub InitializeForwarderPecMail(ByRef documents As IList(Of DocumentInfo))
        Dim forwarderMail As DocumentInfo = documents _
            .Where(Function(d) FileHelper.MatchFileName(d.Name, FileHelper.BUSTA)) _
            .FirstOrDefault()

        If (forwarderMail IsNot Nothing AndAlso forwarderMail.Parent IsNot Nothing) Then
            Dim docToRemove As DocumentInfo = documents.SingleOrDefault(Function(d) FileHelper.MatchFileName(d.Name, FileHelper.CORPO_DEL_MESSAGGIO))
            If (docToRemove IsNot Nothing) Then
                documents.Remove(docToRemove)
            End If
            forwarderMail.Name = FileHelper.CORPO_DEL_MESSAGGIO
            documents = forwarderMail.Parent.Children
            uscPECInfo.PECMail = Nothing
            uscPECInfo.SessionPECMail = Tools.PecMailFactory(forwarderMail)
            uscPECInfo.SessionPECMail.MailBox = CurrentPecMail.MailBox
            SessionPECMail = uscPECInfo.SessionPECMail
        End If
    End Sub


    Public Function GetUDSInitializer() As UDSDto Implements IUDSInitializer.GetUDSInitializer
        Dim udsSelected As UDSRepository = CurrentUDSRepositoryFacade.GetById(Guid.Parse(ddlUDSArchives.SelectedValue))

        If udsSelected Is Nothing Then
            Throw New Exception("Errore in ricerca archivio selezionato")
        End If

        Dim dto As UDSDto = New UDSDto()
        dto.Id = udsSelected.Id
        dto.UDSRepository = New UDSEntityRepositoryDto() With {.UniqueId = udsSelected.Id, .Name = udsSelected.Name}

        Dim model As UnitaDocumentariaSpecifica = UDSModel.LoadXml(udsSelected.ModuleXML).Model

        Dim allSelectedDocument As List(Of DocumentInfo) = AllAttachmentDocuments
        Select Case True
            Case Me.UseInterop
                model.Subject.Value = CheckInteropSubject(SegnaturaReader)
            Case Else
                model.Subject.Value = CheckPecSubject()
        End Select

        'Se il documento principale non è stato valorizzato significa che non si tratta di caricamento automatico da organigramma
        If model.Documents Is Nothing OrElse model.Documents.Document Is Nothing OrElse model.Documents.Document.Instances Is Nothing Then
            If model.Documents IsNot Nothing Then
                If model.Documents.Document IsNot Nothing Then
                    Dim mainDocument As DocumentInfo = SelectedDocument
                    If mainDocument IsNot Nothing Then
                        Dim documents As IList(Of DocumentInstance) = New List(Of DocumentInstance)
                        model.Documents.Document.Instances = GetDocumentInstances(New List(Of DocumentInfo) From {mainDocument})
                    End If
                End If

                If model.Documents.DocumentAttachment IsNot Nothing Then
                    Dim attachments As IList(Of DocumentInfo) = allSelectedDocument
                    If attachments IsNot Nothing Then
                        Dim documents As IList(Of DocumentInstance) = New List(Of DocumentInstance)
                        model.Documents.DocumentAttachment.Instances = GetDocumentInstances(attachments.Where(Function(x) CheckDocument(x.Name).GetValueOrDefault(True)).ToList())
                    End If
                End If
            End If
        End If

        If model.Contacts IsNot Nothing Then
            Dim contacts As IList(Of ContactDTO)
            If ProtocolEnv.SelectSenderFromTreeviewEnabled AndAlso (UseSenderInterop OrElse UseContactType) Then
                contacts = uscMittenti.GetSelectedContacts()
            Else
                contacts = uscMittenti.GetContacts(False)
            End If

            If ddlContactFields.SelectedValue IsNot String.Empty Then
                Dim contactField As Contacts = model.Contacts.Where(Function(x) x.Label.ToString() = ddlContactFields.SelectedValue).SingleOrDefault()
                contactField.ContactInstances = GetContactInstances(contacts)
                contactField.ContactManualInstances = GetManualContactInstances(contacts)
            End If
        End If
        dto.UDSModel = New UDSModel(model)
        Return dto
    End Function

    Private Function GetContactInstances(contacts As IList(Of ContactDTO)) As ContactInstance()
        Dim contactInstances As IList(Of ContactInstance) = New List(Of ContactInstance)
        Dim addressContacts As IList(Of Data.ContactDTO) = contacts.Where(Function(x) x.Type = ContactDTO.ContactType.Address).ToList()
        For Each addressContact As Data.ContactDTO In addressContacts
            Dim instance As ContactInstance = New ContactInstance()
            instance.IdContact = addressContact.Contact.Id.ToString()
            contactInstances.Add(instance)
        Next

        Return contactInstances.ToArray()
    End Function

    Private Function GetManualContactInstances(contacts As IList(Of ContactDTO)) As ContactManualInstance()
        Dim contactInstances As IList(Of ContactManualInstance) = New List(Of ContactManualInstance)

        Dim manualContacts As IList(Of Data.ContactDTO) = contacts.Where(Function(x) x.Type = ContactDTO.ContactType.Manual).ToList()
        For Each manualContact As Data.ContactDTO In manualContacts
            Dim instance As ContactManualInstance = New ContactManualInstance()
            manualContact.Contact.UniqueId = Guid.Empty
            instance.ContactDescription = JsonConvert.SerializeObject(manualContact, New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Serialize})
            contactInstances.Add(instance)
        Next

        Return contactInstances.ToArray()
    End Function

    Private Function GetDocumentInstances(documents As IList(Of DocumentInfo)) As DocumentInstance()
        Dim documentInstances As IList(Of DocumentInstance) = New List(Of DocumentInstance)
        If documents.Any() Then
            For Each document As DocumentInfo In documents
                If TypeOf document Is BiblosPdfDocumentInfo Then
                    documentInstances.Add(New DocumentInstance() With {.DocumentContent = Convert.ToBase64String(document.Stream), .DocumentName = document.Name})
                Else
                    documentInstances.Add(New DocumentInstance() With {.IdDocument = DirectCast(document, BiblosDocumentInfo).DocumentId.ToString()})
                End If
            Next
        End If
        Return documentInstances.ToArray()
    End Function

    Private Sub LoadDdlUDSContactTypes()
        ddlContactFields.Items.Clear()
        If String.IsNullOrEmpty(ddlUDSArchives.SelectedValue) Then
            AjaxManager.ResponseScripts.Add(String.Format(RADIO_ON_CHANGE))
            Exit Sub
        End If

        Dim archive As UDSRepository = New UDSRepositoryFacade(DocSuiteContext.Current.User.FullUserName).GetById(Guid.Parse(ddlUDSArchives.SelectedValue))
        Dim model As UDSModel = UDSModel.LoadXml(archive.ModuleXML)
        If model.Model.Contacts IsNot Nothing Then
            ddlContactFields.DataSource = model.Model.Contacts
        End If
        ddlContactFields.DataBind()
        AjaxManager.ResponseScripts.Add(String.Format(RADIO_ON_CHANGE))
    End Sub

    Private Sub InsertFascicleMiscellanea(idFascicle As Guid)
        If idFascicle.Equals(Guid.Empty) Then
            Throw New ArgumentNullException("Nessun fascicolo definito per l'inserimento")
        End If

        Dim allSelectedDocument As List(Of DocumentInfo) = AllAttachmentDocuments
        If allSelectedDocument.Count = 0 Then
            AjaxManager.ResponseScripts.Add(String.Format(CONFIRM_CALLBACK, Guid.Empty, idFascicle, False.ToString().ToLower(), "Nessun documento selezionato per la fascicolazione"))
            Exit Sub
        End If

        CurrentFascicleFolderFinder.ResetDecoration()
        CurrentFascicleFolderFinder.ExpandProperties = True
        CurrentFascicleFolderFinder.IdFascicle = idFascicle
        CurrentFascicleFolderFinder.ReadDefaultFolder = True

        Dim currentFascicleFolder As FascicleFolder = CurrentFascicleFolderFinder.DoSearch().SingleOrDefault().Entity
        Dim chainId As Guid = Guid.Empty
        If currentFascicleFolder.FascicleDocuments.Count > 0 Then
            chainId = currentFascicleFolder.FascicleDocuments.Where(Function(x) x.ChainType = Entity.DocumentUnits.ChainType.Miscellanea).Select(Function(s) s.IdArchiveChain).SingleOrDefault()
        End If
        Dim isNewIdArchiveChain As Boolean = chainId.Equals(Guid.Empty)

        For Each addedDocument As DocumentInfo In allSelectedDocument
            addedDocument.AddAttribute(BiblosFacade.REGISTRATION_USER_ATTRIBUTE, DocSuiteContext.Current.User.FullUserName)
            Dim biblosFunc As Func(Of DocumentInfo, BiblosDocumentInfo) = Function(d As DocumentInfo) SaveBiblosDocument(d, chainId)
            Dim savedtemplate As BiblosDocumentInfo = biblosFunc(addedDocument)
            chainId = savedtemplate.ChainId
        Next
        AjaxManager.ResponseScripts.Add(String.Format(CONFIRM_CALLBACK, chainId, idFascicle, isNewIdArchiveChain.ToString().ToLower(), String.Empty))
    End Sub
#End Region

End Class