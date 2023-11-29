Imports System.Text
Imports System.Linq
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data.PEC.Finder
Imports VecompSoftware.DocSuiteWeb.Facade.PEC
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.Web
Imports System.Web
Imports VecompSoftware.Services.Biblos.Models

Partial Public Class PECOutgoingMails
    Inherits PECBasePage
    Implements IHavePecMail

#Region " Fields "

    Private _mailBoxes As List(Of PECMailBox)
    Private _integratedMail As Boolean = False
    Private _selectedMailBoxId As Nullable(Of Short)
    Private _unmanagedMailBoxes As List(Of PECMailBox)
    Private _managedMailBoxes As List(Of PECMailBox)
    ''' <summary> Numero di giorni da sottrarre alla data odierna nella preimpostazione del filtro. </summary>
    Private Const DaysToAddInitialized As Double = -15
    Private Const VALID_MAILBOX_IMGURL As String = "../App_Themes/DocSuite2008/images/green-dot-document.png"
    Private Const INTEROP_IMGURL As String = "../App_Themes/DocSuite2008/imgset16/user.png"

#End Region

#Region " Properties "

    Public ReadOnly Property CurrentMailBox As PECMailBox
        Get
            If Not String.IsNullOrEmpty(ddlMailbox.SelectedValue) AndAlso Not ddlMailbox.SelectedValue.Eq("ALL") Then
                Return Facade.PECMailboxFacade.GetById(Short.Parse(ddlMailbox.SelectedValue))
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ShowPECMailBoxIncludedFilter As Boolean
        Get
            Return "ALL".Eq(ddlMailbox.SelectedValue)
        End Get
    End Property

    Private ReadOnly Property IntegratedMail As Boolean
        Get
            If Not _integratedMail Then
                Boolean.TryParse(Request.QueryString("IntegratedMail"), _integratedMail)
            End If

            Return _integratedMail
        End Get
    End Property

    Public Property SelectedMailBoxId As Short?
        Get
            If Not _selectedMailBoxId.HasValue Then
                _selectedMailBoxId = Request.QueryString.GetValueOrDefault(Of Short?)("SelectedMailboxId", Nothing)
                'Provo a caricare la casella PEC dalla sessione
                If Not _selectedMailBoxId.HasValue Then
                    _selectedMailBoxId = CommonShared.SelectedPecMailBoxId
                End If
            End If
            Return _selectedMailBoxId
        End Get
        Set(value As Short?)
            _selectedMailBoxId = value
        End Set
    End Property

    Public ReadOnly Property MailBoxes As List(Of PECMailBox)
        Get
            If _mailBoxes Is Nothing Then
                _mailBoxes = Facade.PECMailboxFacade.GetVisibleMailBoxes(IntegratedMail)
            End If
            Return _mailBoxes
        End Get
    End Property

    Private ReadOnly Property SessionFinderType As SessionSearchController.SessionFinderType
        Get
            Return Request.QueryString.GetValueOrDefault(Of SessionSearchController.SessionFinderType)("CustomFinderType", SessionSearchController.SessionFinderType.PecOutFinderType)
        End Get
    End Property

    Private ReadOnly Property CurrentFinder As NHibernatePECMailFinder
        Get
            Dim finderMy As NHibernatePECMailFinder = CType(SessionSearchController.LoadSessionFinder(SessionFinderType), NHibernatePECMailFinder)
            Dim finderOther As NHibernatePECMailFinder = CType(SessionSearchController.LoadSessionFinder(SessionSearchController.SessionFinderType.PecInFinderType), NHibernatePECMailFinder)

            ' Fastpecsender non prende filtri dagli altri finder
            If SessionFinderType = SessionSearchController.SessionFinderType.FastPecSender Then
                Return finderMy
            End If

            ' Se non ho già fatto alcun filtro, eredito:
            ' -> mailbox
            ' -> date
            ' -> paginazione
            If finderMy Is Nothing Then
                If Not finderOther Is Nothing Then
                    Return CreateFinderFromPecIncoming(finderOther)
                End If
                Return Nothing
            End If

            Dim registrationMy As DateTime
            DateTime.TryParse(CType(Session("DSW_PECOUTFinderType"), String), registrationMy)
            Dim registrationOther As DateTime
            DateTime.TryParse(CType(Session("DSW_PECINFinderType"), String), registrationOther)

            ' Verifico quale Finder sia più recente
            If registrationMy > registrationOther Then
                ' Quello corrente è più recente
                Return finderMy
            End If

            ' L'altro è più recente. Se ha una casella sola la adotto come mia
            If finderOther.MailboxIds.Length <> 1 Then
                ' cancello il finder e ne faccio uno nuovo
                Return Nothing
            End If

            ' Sono sulla stessa casella, posso tenere il mio vecchio Finder
            If finderMy.MailboxIds.Length = 1 AndAlso finderMy.MailboxIds(0) = finderOther.MailboxIds(0) Then
                Return finderMy
            End If

            ' Sono su caselle diverse, ritorno nuovo Finder
            Return CreateFinderFromPecIncoming(finderOther)
        End Get
    End Property

    Public ReadOnly Property PecMails() As IEnumerable(Of PECMail) Implements IHavePecMail.PecMails
        Get
            Return GetSelectedMails()
        End Get
    End Property

    Private Property TaskHeaderIdIn() As IEnumerable(Of Integer)
        Get
            Return CType(ViewState("TaskHeaderIdIn"), IEnumerable(Of Integer))
        End Get
        Set(value As IEnumerable(Of Integer))
            ViewState("TaskHeaderIdIn") = value
        End Set
    End Property

    ''' <summary>
    ''' Indica se predisporre la maschera per visualizzare i soli elementi legati ad invii da FastProtocolSender.
    ''' </summary>
    Private ReadOnly Property IsMassive As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean)("IsMassive", False)
        End Get
    End Property

    Private ReadOnly Property UnmanagedMailBoxes As ICollection(Of PECMailBox)
        Get
            If _unmanagedMailBoxes Is Nothing Then
                _unmanagedMailBoxes = MailBoxes.Where(Function(x) String.IsNullOrWhiteSpace(x.OutgoingServerName)).ToList()
            End If
            Return _unmanagedMailBoxes
        End Get
    End Property

    Private ReadOnly Property ManagedMailBoxes As ICollection(Of PECMailBox)
        Get
            If _managedMailBoxes Is Nothing Then
                _managedMailBoxes = MailBoxes.Where(Function(x) Not String.IsNullOrWhiteSpace(x.OutgoingServerName)).ToList()
            End If
            Return _managedMailBoxes
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()

        dgMail.MasterTableView.NoMasterRecordsText = String.Format("Nessuna {0} disponibile", PecLabel)
        If Not IsPostBack Then
            ValidateFinder()
            InitializePECMailBoxIncluded()
            Initialize()
            SetColumnVisibility("PECOutgoingMails", ProtocolEnv.PECInOutColumnsVisibility, dgMail)
            DataBindMailBoxes(ddlMailbox)
            SetFinderInForm()
            InitializeCollapsedSection()
            UpdatePECMailBoxInputColor()
        End If

    End Sub

    Private Sub UpdatePECMailBoxInputColor()
        Dim selectedItem As RadComboBoxItem = ddlMailbox.SelectedItem

        If selectedItem.ForeColor = Drawing.Color.Red Then
            ddlMailbox.InputCssClass = "text-red"
        End If
    End Sub

    Private Sub chkShowRecorded_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles chkShowRecorded.CheckedChanged
        ToggleShowRecorded(chkShowRecorded.Checked)
    End Sub

    Protected Sub ddlPECMailBoxIncluded_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlPECMailBoxIncluded.SelectedIndexChanged
        dgMail.DiscardFinder()
        DataBindMailGrid()
    End Sub

    Private Sub dgMail_Init(sender As Object, e As EventArgs) Handles dgMail.Init
        For Each column As GridColumn In dgMail.Columns
            Select Case column.UniqueName
                Case "cProtocol"
                    DirectCast(column, CompositeTemplateExportableColumn).CustomExportDelegate = New CompositeTemplateExportableColumn.ExportExpressionDelegate(AddressOf dgMail_LinkProtocolCustomExportDelegate)
            End Select
        Next
    End Sub

    Private Sub dgMail_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles dgMail.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim item As PecMailHeader = DirectCast(e.Item.DataItem, PecMailHeader)
        Dim entityItem As PECMail = Facade.PECMailFacade.GetById(item.Id.Value)
        Dim row As GridDataItem = DirectCast(e.Item, GridDataItem)

        EvaluatePecStatus(item, row)

        With DirectCast(e.Item.FindControl("cmdViewSummary"), RadButton)
            .CommandArgument = item.Id.ToString()
            .Image.ImageUrl = If(item.HasRead.GetValueOrDefault(False), ImagePath.SmallEmailOpen, ImagePath.SmallEmail)
        End With

        With DirectCast(e.Item.FindControl("cmdViewDocs"), RadButton)
            .CommandArgument = item.Id.ToString()
            If ProtocolEnv.PECGridCheckActiveDocumentsEnabled Then
                If entityItem.ProcessStatus <> PECMailProcessStatus.StoredInDocumentManager Then
                    Dim chain As BiblosChainInfo = New BiblosChainInfo(entityItem.IDAttachments)
                    .Enabled = chain.HasActiveDocuments()
                Else
                    .Enabled = True
                End If
            End If
            If item.HasAttachments Then
                .Image.ImageUrl = ImagePath.SmallEmailDocumentsAttach
                .ToolTip = String.Format("Allegati: {0}", item.AttachmentsCount)
            Else
                .Image.ImageUrl = ImagePath.SmallEmailDocuments
            End If
        End With

        With DirectCast(e.Item.FindControl("imgPriority"), Image)
            If item.MailPriority.HasValue AndAlso item.MailPriority.Value <> 0 Then
                .Visible = True
                If item.MailPriority.Value > 0 Then
                    .ImageUrl = "../Comm/Images/Mails/highimportance.gif"
                    .AlternateText = "Priorità alta"
                Else
                    .ImageUrl = "../Comm/Images/Mails/lowimportance.gif"
                    .AlternateText = "Priorità bassa"
                End If
            Else
                .Visible = False
                row.Item("cPriority").Text = WebHelper.Space
            End If
        End With

        With DirectCast(e.Item.FindControl("imgInterop"), Image)
            If Not String.IsNullOrEmpty(item.Segnatura) Then
                .Visible = True
                .ImageUrl = If(item.IsValidForInterop.GetValueOrDefault(False), ImagePath.SmallCodeGreen, ImagePath.SmallCodeGray)
            Else
                .Visible = False
                row.Item("cInterop").Text = WebHelper.Space
            End If
        End With

        With DirectCast(e.Item.FindControl("cmdProtocol"), RadButton)
            If item.Year.HasValue AndAlso item.Number.HasValue AndAlso item.DocumentUnitType.HasValue Then
                .Visible = True
                .Icon.PrimaryIconHeight = Unit.Pixel(16)
                .Icon.PrimaryIconWidth = Unit.Pixel(16)
                .Text = String.Format("{0}/{1:0000000}", item.Year, item.Number)
                If ProtocolEnv.VisualizzaLibricinoOutgoing Then
                    .Icon.PrimaryIconUrl = GetPrimaryIconUrl(item.DocumentUnitType.Value)
                End If
                .CommandName = GetCommandName(item.DocumentUnitType.Value)
                .CommandArgument = GetCommandArgument(item)
                .ToolTip = GetToolTip(item)
            Else
                .Visible = False
                row.Item("cProtocol").Text = WebHelper.Space
            End If
        End With

        With DirectCast(e.Item.FindControl("imgIsPEC"), Image)
            If item.XTrasporto.Eq("posta-certificata") Then
                .Visible = True
                .ImageUrl = ImagePath.SmallDocumentSignature
            Else
                .Visible = False
                row.Item("cIsPEC").Text = WebHelper.Space
            End If
        End With

        With DirectCast(e.Item.FindControl("lblStatus"), Label)
            Dim status As String = Facade.PECMailFacade.GetMailStatusByXRiferimentoMessageId(item.XRiferimentoMessageID)
            If Not String.IsNullOrEmpty(status) Then
                .Visible = True
                .Text = status
            Else
                .Visible = False
                row.Item("colCurrentMailStatus").Text = WebHelper.Space
            End If
        End With

        ' Oggetto
        With DirectCast(e.Item.FindControl("lblSubject"), Label)
            .Font.Bold = Not item.HasRead.GetValueOrDefault(False)
            ' Fix dimensione massima caratteri
            If DocSuiteContext.Current.ProtocolEnv.MaxNumberOfCharsObjectInGridPec > 0 AndAlso Not String.IsNullOrEmpty(item.MailSubject) AndAlso item.MailSubject.Length > DocSuiteContext.Current.ProtocolEnv.MaxNumberOfCharsObjectInGridPec Then
                .Text = item.MailSubject.Remove(DocSuiteContext.Current.ProtocolEnv.MaxNumberOfCharsObjectInGridPec) & "  [...]"
                .ToolTip = item.MailSubject
            Else
                .Text = item.MailSubject
            End If
        End With

        ' Destinatari
        Dim lblRecipient As Label = TryCast(e.Item.FindControl("lblRecipient"), Label)
        If lblRecipient IsNot Nothing Then
            Dim recipients As String() = item.MailRecipients.Split(";"c)
            If recipients.Any() Then
                Dim recipientFormatted As New List(Of String)
                For Each recipient As String In recipients
                    If String.IsNullOrEmpty(RegexHelper.MatchName(recipient)) Then
                        recipientFormatted.Add(String.Format("<{0}>", RegexHelper.MatchEmail(recipient).Trim()))
                    Else
                        recipientFormatted.Add(String.Format("'{0}' <{1}>", RegexHelper.MatchName(recipient).Trim(), RegexHelper.MatchEmail(recipient).Trim()))
                    End If
                Next

                Dim mailRecipients As String = String.Join("; ", recipientFormatted)
                ' Fix dimensione massima caratteri
                If DocSuiteContext.Current.ProtocolEnv.MaxNumberOfCharsRecipientInGridPec > 0 AndAlso Not String.IsNullOrEmpty(mailRecipients) AndAlso mailRecipients.Length > DocSuiteContext.Current.ProtocolEnv.MaxNumberOfCharsRecipientInGridPec Then
                    lblRecipient.Text = HttpUtility.HtmlEncode(mailRecipients).Remove(DocSuiteContext.Current.ProtocolEnv.MaxNumberOfCharsRecipientInGridPec) & "  [...]"
                    lblRecipient.ToolTip = mailRecipients
                Else
                    lblRecipient.Text = HttpUtility.HtmlEncode(mailRecipients)
                End If
            End If
        End If

        ' Destinatari Cc
        Dim lblRecipientCc As Label = TryCast(e.Item.FindControl("lblRecipientCc"), Label)
        If lblRecipientCc IsNot Nothing Then
            Dim recipientsCc As String() = item.MailRecipientsCc.Split(";"c)
            If recipientsCc.Any() Then
                Dim recipientFormattedCc As New List(Of String)
                For Each recipientCc As String In recipientsCc
                    If String.IsNullOrEmpty(RegexHelper.MatchName(recipientCc)) Then
                        recipientFormattedCc.Add(String.Format("<{0}>", RegexHelper.MatchEmail(recipientCc).Trim()))
                    Else
                        recipientFormattedCc.Add(String.Format("'{0}' <{1}>", RegexHelper.MatchName(recipientCc).Trim(), RegexHelper.MatchEmail(recipientCc).Trim()))
                    End If
                Next

                Dim mailRecipientsCc As String = String.Join("; ", recipientFormattedCc)
                ' Fix dimensione massima caratteri
                If DocSuiteContext.Current.ProtocolEnv.MaxNumberOfCharsRecipientInGridPec > 0 AndAlso Not String.IsNullOrEmpty(mailRecipientsCc) AndAlso mailRecipientsCc.Length > DocSuiteContext.Current.ProtocolEnv.MaxNumberOfCharsRecipientInGridPec Then
                    lblRecipientCc.Text = HttpUtility.HtmlEncode(mailRecipientsCc).Remove(DocSuiteContext.Current.ProtocolEnv.MaxNumberOfCharsRecipientInGridPec) & "  [...]"
                    lblRecipientCc.ToolTip = mailRecipientsCc
                Else
                    lblRecipientCc.Text = HttpUtility.HtmlEncode(mailRecipientsCc)
                End If
            End If
        End If

        ' Dimensione
        With DirectCast(e.Item.FindControl("lblPecSize"), Label)
            If item.Size.HasValue Then
                .Visible = True
                .Text = item.Size.Value.ToByteFormattedString(1)
            Else
                .Visible = False
                row.Item("PecSize").Text = WebHelper.Space
            End If
        End With

    End Sub

    Private Sub RefreshGrid_Events(ByVal sender As Object, ByVal e As EventArgs) Handles ddlMailbox.SelectedIndexChanged, cmdRefreshGrid.Click
        If CurrentMailBox IsNot Nothing AndAlso CurrentMailBox.LoginError Then
            AjaxAlert("La casella PEC ha un problema di configurazione. Avvisare il responsabile per la corretta configurazione")
        End If

        ddlMailbox.InputCssClass = If(CurrentMailBox IsNot Nothing AndAlso CurrentMailBox.LoginError, "text-red", "text-black")

        Dim selectedId As Short
        If Short.TryParse(ddlMailbox.SelectedValue, selectedId) Then
            CommonShared.SelectedPecMailBoxId = selectedId
        Else
            CommonShared.SelectedPecMailBoxId = Nothing
        End If

        ddlPECMailBoxIncluded.Visible = ShowPECMailBoxIncludedFilter
        ddlPECMailBoxIncluded.SelectedIndex = 1

        dgMail.DiscardFinder()
        DataBindMailGrid()
    End Sub

    Private Sub dgMail_ItemCommand(ByVal source As Object, ByVal e As GridCommandEventArgs) Handles dgMail.ItemCommand
        If e.Item.ItemType = GridItemType.AlternatingItem OrElse e.Item.ItemType = GridItemType.Item Then
            Select Case e.CommandName
                Case SHOW_PROT_COMMAND_NAME
                    Dim arguments As String() = e.CommandArgument.ToString.Split("|"c)
                    Dim s As String = $"UniqueId={arguments(0)}&Type=Prot"

                    Response.RedirectLocation = "parent"
                    Response.Redirect($"~/Prot/ProtVisualizza.aspx?{CommonShared.AppendSecurityCheck(s)}")
                Case SHOW_UDS_COMMAND_NAME
                    Dim arguments As String() = e.CommandArgument.ToString().Split({"|"c}, 2)
                    Response.RedirectLocation = "parent"
                    Response.Redirect(String.Format("../UDS/UDSView.aspx?Type=UDS&IdUDS={0}&IdUDSRepository={1}", arguments(0), arguments(1)))
                Case "ViewDocs"
                    Response.RedirectLocation = "parent"
                    Response.Redirect("../PEC/PECView.aspx?" & CommonShared.AppendSecurityCheck(String.Format("Type=Pec&PECId={0}", e.CommandArgument)))
                Case "ViewSummary", "ViewReplay", "ViewForward"
                    Response.RedirectLocation = "parent"
                    Response.Redirect("../PEC/PECSummary.aspx?" & CommonShared.AppendSecurityCheck(String.Format("Type=Pec&PECId={0}", e.CommandArgument)))
            End Select
        End If
    End Sub

    Protected Sub CmdViewLogClick(ByVal sender As Object, ByVal e As EventArgs) Handles cmdViewLog.Click

        Dim selectedPecMails As IList(Of PECMail)
        If Not GetSelectedPecMail(Of PECMail)(Function(f) GetSelectedPecMails(f), dgMail, selectedPecMails) Then
            Exit Sub
        End If
        CurrentPecMailIdList = GetSelectedPecMailIds(dgMail)
        Server.Execute("../PEC/PECViewLog.aspx")
    End Sub

    Private Sub CmdNewMailClick(ByVal sender As Object, ByVal e As EventArgs) Handles cmdNewMail.Click
        Dim url As New StringBuilder()
        url.AppendFormat("PECInsert.aspx?Type=Pec&SimpleMode={0}", ProtocolEnv.PECSimpleMode.ToString())
        If (ddlMailbox IsNot Nothing) AndAlso (ddlMailbox.SelectedItem IsNot Nothing) AndAlso (Not String.IsNullOrEmpty(ddlMailbox.SelectedItem.Value)) AndAlso (Not ddlMailbox.SelectedItem.Value.Eq("ALL")) Then
            url.AppendFormat("&SelectedMailboxId={0}", ddlMailbox.SelectedItem.Value)
        End If
        Response.Redirect(url.ToString())
    End Sub

    Private Sub PageAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        If e.Argument.Eq("refresh") Then
            DataBindMailGrid()
        End If
    End Sub

    Private Sub CmdClearFiltersClick(sender As Object, e As EventArgs) Handles cmdClearFilters.Click
        InitializeFilters()
        dgMail.DiscardFinder()
        '  AddExpressionRegistrationDateOrder()
        DataBindMailGrid()
    End Sub

    Private Sub cmdForward_Click(sender As Object, e As EventArgs) Handles cmdForward.Click
        FordwardMail(CurrentMailBox, dgMail)
    End Sub

    Protected Sub cmdResend_Click(sender As Object, e As EventArgs) Handles cmdResend.Click
        Dim selectedPecMails As IList(Of PECMail)
        If Not GetSelectedPecMail(Of PECMail)(Function(f) GetSelectedPecMails(f), dgMail, selectedPecMails) Then
            Exit Sub
        End If

        Try
            selectedPecMails.ToList().ForEach(Sub(m) Me.IsPecResendable(m))
            selectedPecMails.ToList().ForEach(Sub(m) Facade.PECMailFacade.Resend(m))
        Catch ex As DocSuiteException
            FileLogger.Error(LoggerName, ex.Message, ex)
            Me.AjaxAlert(ex.Message)
            Return
        End Try

        CmdClearFiltersClick(sender, e)
        RefreshGrid_Events(sender, e)
        AjaxAlert("Il reinvio delle PEC selezionate è avvenuto correttamente")
    End Sub

    Private Sub btnExport_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnExport.Click
        Dim selectedPecMails As IList(Of PECMail) = New List(Of PECMail)
        If Not GetSelectedPecMail(Of PECMail)(Function(f) GetSelectedPecMails(f), dgMail, selectedPecMails) Then
            Return
        End If

        Dim listID As IList(Of Guid) = selectedPecMails.Select(Function(f) f.DocumentUnit.Id).ToList()

        Session("ExportStartDate") = DateTime.Now
        Dim resultsErrorString As String = CommExport.InitializeExportTask(listID)

        If Not String.IsNullOrEmpty(resultsErrorString) Then
            AjaxAlert(resultsErrorString, False)
            Return
        End If

        SetupPageWithTaskRunning()

    End Sub

    Public Function dgMail_LinkProtocolCustomExportDelegate(dataItem As GridDataItem) As String
        Dim btn As RadButton = DirectCast(dataItem.FindControl("cmdProtocol"), RadButton)
        If btn IsNot Nothing Then
            Return btn.ToolTip
        End If
        Return String.Empty
    End Function
#End Region

#Region " Methods "
    Private Sub SetupPageWithTaskRunning()
        Dim url As String = "~\Prot\ProtImportProgress.aspx?" & CommonShared.AppendSecurityCheck("Type=Prot&Title=Esportazione Documenti&TaskType=E")
        WindowBuilder.LoadWindow("wndProgress", url, "onTaskCompleted", Unit.Pixel(500), Unit.Pixel(250))
    End Sub

    Protected Function WindowWidth() As Integer
        Return DocSuiteContext.Current.ProtocolEnv.PECWindowWidth
    End Function

    Protected Function WindowHeight() As Integer
        Return DocSuiteContext.Current.ProtocolEnv.PECWindowHeight
    End Function

    Protected Function WindowBehaviors() As String
        Return DocSuiteContext.Current.ProtocolEnv.PECWindowBehaviors
    End Function

    Protected Function WindowPosition() As String
        Return DocSuiteContext.Current.ProtocolEnv.PECWindowPosition
    End Function

    Private Sub InitializeFilters()
        dtpShowRecordedFrom.SelectedDate = Today.AddDays(DaysToAddInitialized)
        dtpShowRecordedTo.SelectedDate = Today

        ToggleShowRecorded(ProtocolEnv.PECOutShowRecordedDefault)

        rdbFilterPECResult.SelectedValue = "-1"
        If ProtocolEnv.PECWithErrorFilterEnabled Then
            rdbFilterPECResult.SelectedValue = "2"
        End If
        rdbSenderFilter.SelectedValue = "-1"
        cbIncludeMultiSended.Checked = False
        txtFinderSubject.Text = ""
        txtFilterRecipient.Text = ""
        trIncludeMultiSended.Visible = ProtocolEnv.FastProtocolSenderEnabled
        trMessageMultiSended.Visible = False
        If Me.IsMassive Then
            trIncludeMultiSended.Visible = False
            trMessageMultiSended.Visible = True
            cbIncludeMultiSended.Checked = True
        End If
    End Sub

    Private Sub Initialize()
        Title = "PEC - POSTA INVIATA"
        InitializeFilters()

        tblFilterEsito.Visible = DocSuiteContext.Current.ProtocolEnv.ShowMailReceiptFilters
        tblFilterVisualizza.Visible = DocSuiteContext.Current.ProtocolEnv.PECOutShowRecordedFilter
        btnOpenModalRaccomandata.Visible = DocSuiteContext.Current.ProtocolEnv.IsRaccomandataEnabled AndAlso Me.IsMassive
        btnExport.Visible = DocSuiteContext.Current.ProtocolEnv.FastProtocolSenderEnabled AndAlso Me.IsMassive
        hrFilterEsito.Visible = tblFilterEsito.Visible AndAlso tblFilterVisualizza.Visible
        cmdResend.Visible = DocSuiteContext.Current.ProtocolEnv.ResendAndPecClone AndAlso Me.IsMassive
        cmdResend.Text = String.Format("Reinvia {0}", PecLabel)
        cmdViewLog.Visible = CommonShared.HasGroupPecMailLogViewRight
        cmdDelete.Visible = ProtocolEnv.PECDeleteButtonEnabled
        If ProtocolEnv.PECNewMailEnabled Then
            cmdNewMail.Style.Remove("display")
            'Verifico se effettivamente può spedire
            cmdNewMail.Enabled = UserCanSend()
            If (Not cmdNewMail.Enabled) Then
                cmdNewMail.ToolTip = "Non esistono account PEC abilitati all'invio per l'utente corrente."
            End If
        Else
            cmdNewMail.Style.Add("display", "none")
        End If

        If Not cmdNewMail.Enabled Then
            cmdNewMail.ToolTip = "Non esistono account PEC abilitati all'invio per l'utente corrente."
        End If
    End Sub

    Private Sub InitializeCollapsedSection()
        If ProtocolEnv.CollapsePECHeaderEnabled Then
            collapseSection.Visible = True
        End If
    End Sub

    Private Sub InitializePECMailBoxIncluded()
        ddlPECMailBoxIncluded.Items.Clear()

        ddlPECMailBoxIncluded.Items.Add(New RadComboBoxItem(EnumHelper.GetDescription(IncludedPECTypes.Unmanaged), CType(IncludedPECTypes.Unmanaged, Short).ToString()))
        ddlPECMailBoxIncluded.Items.Add(New RadComboBoxItem(EnumHelper.GetDescription(IncludedPECTypes.Managed), CType(IncludedPECTypes.Managed, Short).ToString()) With {.Selected = True})
        ddlPECMailBoxIncluded.Items.Add(New RadComboBoxItem(EnumHelper.GetDescription(IncludedPECTypes.Both), CType(IncludedPECTypes.Both, Short).ToString()))
        ddlPECMailBoxIncluded.Visible = False
    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(dgMail, dgMail, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlMailbox, dgMail, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlMailbox, pnlFilterLeft)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlPECMailBoxIncluded, dgMail, MasterDocSuite.AjaxDefaultLoadingPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(chkShowRecorded, dtpShowRecordedFrom)
        AjaxManager.AjaxSettings.AddAjaxSetting(chkShowRecorded, dtpShowRecordedTo)
        AjaxManager.AjaxSettings.AddAjaxSetting(chkShowRecorded, cmdRefreshGrid)

        AjaxManager.AjaxSettings.AddAjaxSetting(cmdRefreshGrid, dgMail, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdRefreshGrid, dtpShowSentFrom)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdRefreshGrid, dtpShowSentTo)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdRefreshGrid, pnlFilterRight)

        AjaxManager.AjaxSettings.AddAjaxSetting(cmdResend, dgMail, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdResend, pnlFilterRight)

        AjaxManager.AjaxSettings.AddAjaxSetting(cmdClearFilters, dgMail)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdClearFilters, ddlMailbox)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdClearFilters, dtpShowSentFrom)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdClearFilters, dtpShowSentTo)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdClearFilters, txtFilterRecipient)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdClearFilters, txtFinderSubject)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdClearFilters, rdbSenderFilter)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdClearFilters, pnlFilterRight)
        If Not "true".Eq(Request.QueryString("IsMassive")) AndAlso trIncludeMultiSended.Visible Then
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdClearFilters, cbIncludeMultiSended)
        End If

        AjaxManager.AjaxSettings.AddAjaxSetting(cmdNewMail, dgMail, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, dgMail, MasterDocSuite.AjaxDefaultLoadingPanel)

        WindowBuilder.RegisterWindowManager(RadWindowManager)
        WindowBuilder.RegisterOpenerElement(btnExport)
        WindowBuilder.RegisterOpenerElement(AjaxManager)

        AddHandler AjaxManager.AjaxRequest, AddressOf PageAjaxRequest
    End Sub

    Private Overloads Sub ToggleShowRecorded(ByVal pEnabled As Boolean)
        chkShowRecorded.Checked = pEnabled
        dtpShowRecordedFrom.Enabled = chkShowRecorded.Checked
        dtpShowRecordedTo.Enabled = chkShowRecorded.Checked
    End Sub


    Protected Function GetSelectedMails() As IList(Of PECMail)
        Return Facade.PECMailFacade.GetListByIds(GetSelectedPecMailIds(dgMail))
    End Function

    Private Sub SetFinderInForm()
        Dim finder As NHibernatePECMailFinder = CurrentFinder

        '' Il finder potrebbe essere ancora nothing
        If finder Is Nothing Then
            ' Imposto le date standard
            dtpShowSentFrom.SelectedDate = Today.AddDays(DaysToAddInitialized)
            dtpShowSentTo.SelectedDate = Today

            Dim defaultMailBox As PECMailBox = Facade.PECMailboxFacade.GetDefault(MailBoxes)
            If defaultMailBox IsNot Nothing Then
                ddlMailbox.SelectedValue = defaultMailBox.Id.ToString()
            End If
            Exit Sub
        End If

        TaskHeaderIdIn = finder.TaskHeaderIdIn
        If Not TaskHeaderIdIn Is Nothing Then
            chkAnomalies.Checked = False
        End If

        ' Filtro per casella di posta
        If Not finder.MailboxIds.IsNullOrEmpty() AndAlso finder.MailboxIds.Length = 1 Then
            ' una sola casella selezionata
            ddlMailbox.SelectedValue = finder.MailboxIds(0).ToString()
            ddlPECMailBoxIncluded.Visible = False
        ElseIf Not SelectedMailBoxId Is Nothing AndAlso finder.MailboxIds.Length > 1 Then
            ' Selected value Pagina Precedente
            ddlMailbox.SelectedValue = SelectedMailBoxId
            ddlPECMailBoxIncluded.Visible = False
        Else
            ' ALL
            ddlMailbox.SelectedValue = "ALL"
            ddlPECMailBoxIncluded.Visible = True
            ddlPECMailBoxIncluded.SelectedIndex = 1
        End If

        If CurrentMailBox IsNot Nothing AndAlso CurrentMailBox.LoginError Then
            AjaxAlert("La casella PEC ha un problema di configurazione. Avvisare il responsabile per la corretta configurazione")
        End If

        ' Filtri per data spedizione (da - a e nulla)
        ' Imposto solo se nel finder sono già presenti i valori (su richiesta di alejandro)
        If finder.MailDateFrom.HasValue Then
            dtpShowSentFrom.SelectedDate = finder.MailDateFrom.Value
        End If
        If finder.MailDateTo.HasValue Then
            dtpShowSentTo.SelectedDate = finder.MailDateTo.Value
        End If

        ' Filtri per data protocollazione (da - a)
        If finder.RecordedDateFrom.HasValue OrElse finder.RecordedDateTo.HasValue Then
            chkShowRecorded.Checked = True
            If finder.RecordedDateFrom.HasValue Then
                dtpShowRecordedFrom.SelectedDate = finder.RecordedDateFrom.Value
            End If
            If finder.RecordedDateTo.HasValue Then
                dtpShowRecordedTo.SelectedDate = finder.RecordedDateTo.Value
            End If
        Else
            chkShowRecorded.Checked = False
            dtpShowRecordedFrom.SelectedDate = Nothing
            dtpShowRecordedTo.SelectedDate = Nothing
        End If

        ' Filtro per Oggetto
        txtFinderSubject.Text = finder.MailSubject
        ' Filtro per Destinatario
        txtFilterRecipient.Text = finder.Recipient

        rdbFilterPECResult.SelectedValue = "-1"
        If ProtocolEnv.PECWithErrorFilterEnabled Then
            rdbFilterPECResult.SelectedValue = "2"
        End If
        rdbSenderFilter.SelectedValue = "-1"
        If finder.ReceiptCriteria.HasValue Then
            rdbFilterPECResult.SelectedValue = finder.ReceiptCriteria.Value.ToString("D")
        End If
        If finder.RegistrationUserCriteria.HasValue Then
            rdbSenderFilter.SelectedValue = finder.RegistrationUserCriteria.Value.ToString("D")
        End If
        If finder.TaskHeaderCriteria.HasValue Then
            cbIncludeMultiSended.Checked = finder.TaskHeaderCriteria.Value = PECTaskHeaderCriteria.WithTaskHeader
        End If

    End Sub

    Private Sub DataBindMailBoxes(ByVal ddlMBoxes As RadComboBox)
        ddlMBoxes.Items.Clear()
        Dim realMailBoxes As IList(Of PECMailBox) = Facade.PECMailboxFacade.FillRealPecMailBoxes(MailBoxes)
        For Each mailbox As PECMailBox In realMailBoxes
            Dim comboboxItem As RadComboBoxItem = New RadComboBoxItem()
            comboboxItem.Text = mailbox.MailBoxName
            comboboxItem.Value = mailbox.Id.ToString()
            comboboxItem.ImageUrl = If(mailbox.IsForInterop, INTEROP_IMGURL, VALID_MAILBOX_IMGURL)
            If mailbox.LoginError Then
                comboboxItem.ForeColor = Drawing.Color.Red
            End If
            ddlMailbox.Items.Add(comboboxItem)
        Next
        ddlMBoxes.Items.Add(New RadComboBoxItem("Tutte", "ALL") With {.Selected = CommonShared.HasGroupAdministratorRight})

        ddlPECMailBoxIncluded.Visible = CommonShared.HasGroupAdministratorRight
    End Sub

    Private Function GetFinderByForm() As NHibernatePECMailFinder
        Dim finder As New NHibernatePECMailFinder()
        finder.TaskHeaderIdIn = TaskHeaderIdIn

        ' Filtro per sola posta in uscita.
        finder.Direction = PECMailDirection.Outgoing
        finder.Actives = True
        finder.Anomalies = chkAnomalies.Checked

        ' Filtro caselle di posta
        Dim mailBoxIds As New List(Of Short)
        If Not ddlMailbox.SelectedValue.Eq("ALL") Then
            mailBoxIds.Add(Short.Parse(ddlMailbox.SelectedValue))
        Else
            Dim includedPECMailBoxes As ICollection(Of PECMailBox) = MailBoxes
            Dim pecMailBoxIncluded As Short
            If Not String.IsNullOrEmpty(ddlPECMailBoxIncluded.SelectedValue) AndAlso Short.TryParse(ddlPECMailBoxIncluded.SelectedValue, pecMailBoxIncluded) Then
                FileLogger.Debug(LoggerName, $"ddlPECMailBoxIncluded has been setted with {ddlPECMailBoxIncluded.SelectedValue} and value {pecMailBoxIncluded}")
                Select Case CType(pecMailBoxIncluded, IncludedPECTypes)
                    Case IncludedPECTypes.Unmanaged
                        FileLogger.Debug(LoggerName, $"ddlPECMailBoxIncluded force UnmanagedMailBoxes")
                        includedPECMailBoxes = UnmanagedMailBoxes
                    Case IncludedPECTypes.Managed
                        FileLogger.Debug(LoggerName, $"ddlPECMailBoxIncluded force ManagedMailBoxes")
                        includedPECMailBoxes = ManagedMailBoxes
                End Select
            End If
            For Each item As RadComboBoxItem In ddlMailbox.Items
                If Not item.Value.Eq("ALL") AndAlso includedPECMailBoxes.Any(Function(x) x.Id.Equals(Short.Parse(item.Value))) Then
                    mailBoxIds.Add(Short.Parse(item.Value))
                End If
            Next
        End If
        finder.MailboxIds = mailBoxIds.ToArray()
        If mailBoxIds.Count > 2100 Then
            FileLogger.Warn(LoggerName, $"mailBoxIds has been limited to 2100 elements to prevent timeout issue")
            finder.MailboxIds = mailBoxIds.Take(2100).ToArray()
        End If

        ' Filtri per data spedizione (da - a e nulla).
        finder.MailDateFrom = dtpShowSentFrom.SelectedDate
        finder.MailDateTo = dtpShowSentTo.SelectedDate

        finder.UnsentMails = True

        ' Filtri per data protocollazione (da - a).
        If chkShowRecorded.Checked Then
            finder.RecordedDateFrom = dtpShowRecordedFrom.SelectedDate
            finder.RecordedDateTo = dtpShowRecordedTo.SelectedDate
        End If

        Dim receiptValue As PECReceiptCriteria
        If [Enum].TryParse(rdbFilterPECResult.SelectedValue, receiptValue) Then
            finder.ReceiptCriteria = receiptValue
        End If
        finder.TaskHeaderCriteria = If(cbIncludeMultiSended.Checked, PECTaskHeaderCriteria.All, PECTaskHeaderCriteria.ExcludeTaskHeader)

        If rdbSenderFilter.SelectedValue = "0" Then
            finder.RegistrationUserCriteria = PECRegistrationUserCriteria.MyRegistrationUser
            finder.RegistrationUser = DocSuiteContext.Current.User.FullUserName
        ElseIf rdbSenderFilter.SelectedValue = "1" Then
            finder.RegistrationUserCriteria = PECRegistrationUserCriteria.MySectors
            Dim myRoles As ICollection(Of Role) = Facade.RoleFacade.GetUserRoles(DSWEnvironment.Protocol, DossierRoleRightPositions.Enabled, True, CurrentTenant.TenantAOO.UniqueId)
            Dim pecs As ICollection(Of PECMailBox) = MailBoxes _
                                                     .Where(Function(mb) Not String.IsNullOrEmpty(mb.IncomingServerName) AndAlso Not String.IsNullOrEmpty(mb.OutgoingServerName)) _
                                                     .ToList()
            Dim selectedPecMail As Short
            If Not String.IsNullOrEmpty(ddlMailBox.SelectedValue) AndAlso Not ddlMailBox.SelectedValue.Eq("ALL") AndAlso Short.TryParse(ddlMailBox.SelectedValue, selectedPecMail) Then
                pecs = pecs.Where(Function(p) p.Id.Equals(selectedPecMail)).ToList()
            End If

            Dim colleagueGroupName As ICollection(Of String) = Facade.RoleGroupFacade.GetByPecMailBoxes(pecs)
            Dim users As ICollection(Of String) = New List(Of String)()
            For Each groupName As String In colleagueGroupName
                For Each adUser As AccountModel In CommonAD.GetADUsersFromGroup(groupName, CommonShared.UserDomain)
                    If Not users.Contains(adUser.Account) Then
                        users.Add(String.Format("{0}\{1}", adUser.Domain, adUser.Account))
                    End If
                Next
            Next
            finder.RegistrationUsers = users.ToArray()
        End If

        ' Filtro per Oggetto
        finder.MailSubject = txtFinderSubject.Text.Trim()
        ' Filtro per Destinatario
        finder.Recipient = txtFilterRecipient.Text.Trim()
        finder.SortExpressions.Add("RegistrationDate", "DESC")
        Return finder
    End Function

    Private Sub DataBindMailGrid()
        If (ddlMailBox.Items Is Nothing) OrElse ddlMailBox.Items.Count <= 1 OrElse String.IsNullOrEmpty(ddlMailBox.SelectedValue) OrElse ddlMailBox.SelectedValue.Eq("--") Then
            Exit Sub
        End If

        If dgMail.Finder Is Nothing Then
            Dim pecMailFinder As NHibernatePECMailFinder = GetFinderByForm()

            dgMail.Finder = pecMailFinder

            ' Paginazione griglia.
            pecMailFinder.EnablePaging = dgMail.AllowPaging
            dgMail.CurrentPageIndex = 0
            dgMail.CustomPageIndex = 0


        End If

        dgMail.PageSize = dgMail.Finder.PageSize


        ' Salvo il Finder in Sessione (gestione BACK)
        SessionSearchController.SaveSessionFinder(dgMail.Finder, SessionFinderType)
        ' Salvo in sessione l'ora di registrazione
        If SessionFinderType = SessionSearchController.SessionFinderType.PecOutFinderType Then
            Session.Add("DSW_PECOUTFinderType", DateTime.Now)
        End If

        ' DataBind del finder.
        dgMail.DataBindFinder()
    End Sub

    Protected Function GetIsPecIcon(ByVal xTrasporto As String) As String
        If xTrasporto.Eq("posta-certificata") Then
            Return "../Comm/Images/Mails/PEC.gif"
        End If

        Return ImagePath.SpacerImage
    End Function

    Protected Function GetAttachmentIcon(hasAttachments As Boolean) As String
        If hasAttachments Then
            Return "../Comm/Images/Mails/mailattach.gif"
        End If
        Return ImagePath.SpacerImage
    End Function

    Protected Function GetProtocolFullNumber(year As Short, number As Integer) As String
        If year > 0 AndAlso number > 0 Then
            Return Facade.ProtocolFacade.GetProtocolNumber(year, number)
        End If
        Return String.Empty
    End Function

    Protected Function GetSegnaturaIcon(ByVal segnatura As String, ByVal isValidForInterop As Boolean) As String
        If String.IsNullOrEmpty(segnatura) Then
            Return ImagePath.SpacerImage
        End If

        If isValidForInterop Then
            Return "../Comm/Images/segn_valid16.gif"
        Else
            Return "../Comm/Images/segn16.gif"
        End If
    End Function

    Protected Function GetForwardedIcon(ByVal isForwarded As Boolean) As String
        If isForwarded Then
            Return "../Comm/Images/Mails/forwardmail.gif"
        End If

        Return ImagePath.SpacerImage
    End Function

    Protected Function GetMovedIcon(ByVal idMail As Integer) As String
        Dim mail As PECMail = Facade.PECMailFacade.GetById(idMail)
        If mail Is Nothing Then
            Return ImagePath.SpacerImage
        End If

        Dim logs As IList(Of PECMailLog) = mail.LogEntries
        If (logs Is Nothing) OrElse logs.Count <= 0 Then
            Return ImagePath.SpacerImage
        End If

        If logs.Any(Function(log) log.Type.Eq(PECMailLogType.Move.ToString())) Then
            Return "../Comm/Images/Mails/mail_moved.gif"
        End If

        Return ImagePath.SpacerImage
    End Function

    Protected Function GetIsDestinatedIcon(ByVal isDestinated As Boolean) As String
        If isDestinated Then
            Return "../Comm/Images/Mails/forwardmail.gif"
        End If

        Return ImagePath.SpacerImage
    End Function

    Protected Function ProtVisualizzaUrl() As String
        Return ResolveUrl("../Prot/ProtVisualizza.aspx")
    End Function

    Protected Function PecMailViewerUrl() As String
        Return ResolveUrl("~/PEC/PECView.aspx")
    End Function

    Private Function UserCanSend() As Boolean
        Dim visibleMSendingailBoxes As IList(Of PECMailBox) = Facade.PECMailboxFacade.GetVisibleSendingMailBoxes()
        Dim realMailBoxes As IList(Of PECMailBox) = Facade.PECMailboxFacade.FillRealPecMailBoxes(visibleMSendingailBoxes)
        Return realMailBoxes.Any(Function(x) Not x.IsForInterop)
    End Function

    Private Function CreateFinderFromPecIncoming(ByVal pecIncomingFinder As NHibernatePECMailFinder) As NHibernatePECMailFinder
        Dim finder As NHibernatePECMailFinder = GetFinderByForm()
        finder.MailboxIds = pecIncomingFinder.MailboxIds

        If pecIncomingFinder.MailDateFrom.HasValue Then
            finder.MailDateFrom = pecIncomingFinder.MailDateFrom.Value
        Else
            finder.MailDateFrom = Today.AddDays(DaysToAddInitialized)
        End If

        If pecIncomingFinder.MailDateTo.HasValue Then
            finder.MailDateTo = pecIncomingFinder.MailDateTo.Value
        Else
            finder.MailDateTo = Today
        End If

        ' Eredito il paging
        finder.EnablePaging = pecIncomingFinder.EnablePaging
        finder.PageSize = pecIncomingFinder.PageSize

        Return finder
    End Function

    Private Sub ValidateFinder()
        '''rimuovo il finder della griglia per ricaricare i dati in maniera asincrona
        '''così da non bloccare l'utente in apertura della pagina
        If dgMail.Finder IsNot Nothing Then
            dgMail.DiscardFinder()
        End If
    End Sub

#End Region

End Class