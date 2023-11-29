Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Commons
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade.PEC
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.DTO.WorkflowsElsa
Imports VecompSoftware.DocSuiteWeb.Facade.WebAPI

Partial Public Class PECIncomingMails
    Inherits PECBasePage
    Implements IHavePecMail

#Region " Fields "

    Private _mailboxes As IList(Of PECMailBox)

    ''' <summary> Numero di giorni da sottrarre alla data odierna nella preimpostazione del filtro </summary>
    Const DaysToAddInitialized As Double = -15
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

    ''' <summary> Presa in carico abilitata </summary>
    Public ReadOnly Property IsHandleEnabled As Boolean
        Get
            ' La casella corrente è abilitata
            If CurrentMailBox IsNot Nothing Then
                Return CurrentMailBox.PecHandleEnabled()
            End If

            ' Almeno una casella abilitata
            Return MailBoxes.Any(Function(pecMailBox) pecMailBox.PecHandleEnabled() = True)
        End Get
    End Property

    ''' <summary>
    ''' Definisce se la configurazione corrente contiene almeno 1 Casella di protocollazione esplicita (=mittente non bloccato)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsProtocolBoxExplicit As Boolean
        Get
            '' Il controllo ha senso solo in modalità "Casella di protocollazione"
            If Not ProtocolBoxEnabled Then
                Return False
            End If

            If CurrentMailBox IsNot Nothing Then
                Return CurrentMailBox.IsProtocolBoxExplicit.HasValue AndAlso CurrentMailBox.IsProtocolBoxExplicit.Value
            End If

            Return MailBoxes.Any(Function(pecMailBox) pecMailBox.IsProtocolBoxExplicit.HasValue AndAlso pecMailBox.IsProtocolBoxExplicit.Value)
        End Get
    End Property

    Public ReadOnly Property AutoSender As Boolean
        Get
            '' Il controllo ha senso solo in modalità "Casella di protocollazione"
            If Not ProtocolBoxEnabled Then
                Return False
            End If

            If CurrentMailBox IsNot Nothing Then
                Return Not CurrentMailBox.IsProtocolBoxExplicit.HasValue OrElse Not CurrentMailBox.IsProtocolBoxExplicit.Value
            End If

            Return False
        End Get
    End Property

    ''' <summary> Elenco delle mailbox visibili all'utente </summary>
    Public ReadOnly Property MailBoxes As IList(Of PECMailBox)
        Get
            If _mailboxes Is Nothing Then
                _mailboxes = New List(Of PECMailBox)
                If ProtocolBoxEnabled Then
                    _mailboxes = Facade.PECMailboxFacade.GetVisibleProtocolMailBoxes()
                Else
                    _mailboxes = Facade.PECMailboxFacade.GetVisibleMailBoxes()
                End If
            End If
            Return _mailboxes
        End Get
    End Property

    Private ReadOnly Property CurrentFinder As NHibernatePECMailFinder
        Get
            Try
                Dim finderIn As NHibernatePECMailFinder = CType(SessionSearchController.LoadSessionFinder(SessionSearchController.SessionFinderType.PecInFinderType), NHibernatePECMailFinder)
                Dim finderProtocolBox As NHibernatePECMailFinder = CType(SessionSearchController.LoadSessionFinder(SessionSearchController.SessionFinderType.ProtocolBoxFinderType), NHibernatePECMailFinder)
                Dim finderOut As NHibernatePECMailFinder = CType(SessionSearchController.LoadSessionFinder(SessionSearchController.SessionFinderType.PecOutFinderType), NHibernatePECMailFinder)

                '' Gestione del Finder tra Casella di protocollazione e Posta in arrivo per evitare collisioni
                '' Se ho già un finder in griglia
                If dgMail.Finder IsNot Nothing Then
                    If ProtocolBoxEnabled Then
                        '' se sono in modalità ProtocolBox ma il finder caricato è diverso
                        dgMail.ReplaceFinder(finderProtocolBox)
                        '' Visto che imposto il finder di Casella di protocollazione ritorno subito
                        Return finderProtocolBox
                    Else
                        'End If
                        'If Not ProtocolBoxEnabled AndAlso Not dgMail.Finder.Equals(finderIn) Then
                        '' se sono in modalità standard ma il finder caricato è diverso
                        dgMail.ReplaceFinder(finderIn)
                        '' Visto che imposto il finderIn lascio i controlli successivi
                    End If
                End If

                If finderIn Is Nothing Then
                    If Not finderOut Is Nothing Then
                        Dim item As New NHibernatePECMailFinder()
                        item.MailboxIds = finderOut.MailboxIds
                        Return item
                    End If
                    Return Nothing
                End If

                Dim registrationIn As DateTime
                DateTime.TryParse(CType(Session("DSW_PECINFinderType"), String), registrationIn)
                Dim registrationOut As DateTime
                DateTime.TryParse(CType(Session("DSW_PECOUTFinderType"), String), registrationOut)

                ' Verifico quale Finder sia più recente
                If registrationIn > registrationOut Then
                    ' Quello corrente è più recente
                    Return finderIn
                Else
                    ' L'altro è più recente. Se ha una casella sola la adotto come mia

                    If finderOut.MailboxIds.Length = 1 Then
                        If finderIn.MailboxIds.Length = 1 AndAlso finderIn.MailboxIds(0) = finderOut.MailboxIds(0) Then
                            ' Sono sulla stessa casella, posso tenere il mio vecchio Finder
                            Return finderIn
                        Else
                            ' Sono su caselle diverse, ritorno nuovo Finder
                            Dim item As New NHibernatePECMailFinder()
                            item.MailboxIds = finderOut.MailboxIds
                            Return item
                        End If
                    Else
                        ' cancello il finder e ne faccio uno nuovo
                        Return Nothing
                    End If
                End If
            Catch ex As Exception
                FileLogger.Warn(LoggerName, "Errore in caricamento Finder di Sessione", ex)
                AjaxAlert("Errore in caricamento Finder di Sessione: " + ex.Message)
                Return Nothing
            End Try


        End Get
    End Property

    Private ReadOnly Property ExternalTrashbin As Boolean
        Get
            Dim temp As Boolean = False
            Boolean.TryParse(Request.QueryString.Item("ExternalTrashbin"), temp)
            Return ProtocolEnv.PECExternalTrashbin AndAlso temp
        End Get
    End Property

    Public ReadOnly Property PecMails As IEnumerable(Of PECMail) Implements IHavePecMail.PecMails
        Get
            Return GetSelectedPecMails(dgMail)
        End Get
    End Property

    Private Property CheckBoxesTypeState As Dictionary(Of String, Boolean)
        Get
            If ViewState("CheckBoxesTypeState") IsNot Nothing Then
                Return DirectCast(ViewState("CheckBoxesTypeState"), Dictionary(Of String, Boolean))
            End If
            Return Nothing
        End Get
        Set(value As Dictionary(Of String, Boolean))
            ViewState("CheckBoxesTypeState") = value
        End Set
    End Property

#End Region

#Region " Events "

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        RadScriptManager.GetCurrent(Me).EnableHistory = True
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()

        dgMail.MasterTableView.NoMasterRecordsText = String.Format("Nessuna {0} disponibile", PecLabel)
        If Not IsPostBack Then
            ValidateFinder()

            InitializeExternalTrashbin()
            SetColumnVisibility("PECIncomingMails", ProtocolEnv.PECInOutColumnsVisibility, dgMail)
            InitializePECMailBoxIncluded()
            InitializeMailboxes()
            Initialize()
            SetFinderInForm()
            InitializeCurrentMailBox()
            InitializeDestination()
            SetCestinoButtons()
            InitializeCollapsedSection()
            InitializeHandling()
            UpdatePECMailBoxInputColor()

            If ProtocolEnv.PECMailInsertAuthorizationEnabled AndAlso Not Facade.DomainUserFacade.HasCurrentRight(CurrentDomainUser, DSWEnvironmentType.Protocol, DomainUserFacade.HasPECSendableRight) Then
                cmdNewMail.Visible = False
                cmdForward.Visible = False
            End If
        End If
    End Sub

    Protected Sub ddlMailbox_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlMailbox.SelectedIndexChanged

        If CurrentMailBox IsNot Nothing AndAlso CurrentMailBox.LoginError Then
            AjaxAlert("La casella PEC ha un problema di configurazione. Avvisare il responsabile per la corretta configurazione")
        End If

        Dim selectedId As Short
        If Short.TryParse(ddlMailbox.SelectedValue, selectedId) Then
            CommonShared.SelectedPecMailBoxId = selectedId
        Else
            CommonShared.SelectedPecMailBoxId = Nothing
        End If

        chkDaDestinare.Style.Add("display", "none")
        chkDaDestinare.Checked = False
        chkDestinati.Style.Add("display", "none")
        chkDestinati.Checked = False
        ddlMailbox.InputCssClass = If(CurrentMailBox IsNot Nothing AndAlso CurrentMailBox.LoginError, "text-red", "text-black")
        ddlPECMailBoxIncluded.Visible = "ALL".Eq(ddlMailbox.SelectedValue)
        InitializeCurrentMailBox()
        InitializeHandling()
        InitializeDestination()

        dgMail.DiscardFinder()
        DataBindMailGrid()
    End Sub

    Private Sub UpdatePECMailBoxInputColor()
        Dim selectedItem As RadComboBoxItem = ddlMailbox.SelectedItem

        If selectedItem.ForeColor = Drawing.Color.Red Then
            ddlMailbox.InputCssClass = "text-red"
        End If
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

    Private Sub dgMail_ItemCommand(ByVal source As Object, ByVal e As GridCommandEventArgs) Handles dgMail.ItemCommand
        If e.Item.ItemType <> GridItemType.AlternatingItem AndAlso e.Item.ItemType <> GridItemType.Item Then
            Exit Sub
        End If

        'Dim idMail As Integer = Convert.ToInt32(dgMail.Items(e.Item.ItemIndex).Item("IdMail").Text)
        'Il caricamento del numero fatto sopra non funziona. Tuttavia il codice seguente che utilizza il valore idMail è un codice apparentemente morto.
        Const idMail As Integer = -1
        Dim mail As PECMail
        Dim s As String

        Select Case e.CommandName
            Case "Forward"
                Try
                    mail = Facade.PECMailFacade.GetById(idMail)
                    If mail IsNot Nothing Then
                        mail.IsToForward = True
                        Facade.PECMailFacade.Update(mail)
                        AjaxAlert("Messaggio PEC marcato per l'inoltro.")
                        DataBindMailGrid()
                    Else
                        AjaxAlert("Non è stato possibile marcare per l'inoltro il messaggio PEC, il messaggio non è presente nella casella.")
                    End If
                Catch ex As Exception
                    FileLogger.Warn(LoggerName, "Non è stato possibile marcare per l'inoltro il messaggio PEC, riprovare in un secondo momento: " & ex.Message, ex)
                    AjaxAlert("Non è stato possibile marcare per l'inoltro il messaggio PEC, riprovare in un secondo momento.")
                End Try
            Case "Delete"
                Try
                    mail = Facade.PECMailFacade.GetById(idMail)
                    If mail IsNot Nothing Then
                        Facade.PECMailFacade.Delete(mail)
                        AjaxAlert("Messaggio PEC eliminato.")
                        DataBindMailGrid()
                    Else
                        AjaxAlert("Non è stato possibile marcare per l'inoltro il messaggio PEC, il messaggio non è presente nella casella.")
                    End If
                Catch ex As Exception
                    FileLogger.Warn(LoggerName, "Non è stato possibile marcare per l'inoltro il messaggio PEC, riprovare in un secondo momento: " & ex.Message, ex)
                    AjaxAlert("Non è stato possibile marcare per l'inoltro il messaggio PEC, riprovare in un secondo momento.")
                End Try
            Case "Move"
                Dim ddlMoveToMailbox As DropDownList
                Dim mailbox As PECMailBox
                Dim ctrl As Control = e.Item.FindControl("ddlMoveToMailbox")
                Dim dropDownList As DropDownList = TryCast(ctrl, DropDownList)
                If (dropDownList IsNot Nothing) Then
                    ddlMoveToMailbox = dropDownList
                    Try
                        mailbox = Facade.PECMailboxFacade.GetById(Convert.ToInt16(ddlMoveToMailbox.SelectedValue))
                        mail = Facade.PECMailFacade.GetById(idMail)
                        If mail IsNot Nothing Then
                            mail.MailBox = mailbox
                            mailbox.Mails.Add(mail)

                            ' in concorrenza pessimistica: nessun controllo che la mail sia ancora legata alla stessa mailbox e ralativo feedback
                            Facade.PECMailFacade.Update(mail)

                            AjaxAlert("Messaggio PEC spostato nella nuova casella.")
                            DataBindMailGrid()
                        Else
                            AjaxAlert("Non è stato possibile spostare il messaggio PEC, il messaggio non è presente nella casella.")
                        End If
                    Catch ex As Exception
                        FileLogger.Warn(LoggerName, "Non è stato possibile spostare il messaggio PEC, riprovare in un secondo momento: " & ex.Message, ex)
                        AjaxAlert("Non è stato possibile spostare il messaggio PEC, riprovare in un secondo momento.")
                    End Try
                End If

            Case SHOW_PROT_COMMAND_NAME
                Dim arguments As String() = e.CommandArgument.ToString().Split({"|"c}, 3)
                s = $"UniqueId={arguments(0)}"
                Response.RedirectLocation = "parent"
                Response.Redirect($"~/Prot/ProtVisualizza.aspx?{CommonShared.AppendSecurityCheck(s)}&Type=Prot")
            Case SHOW_UDS_COMMAND_NAME
                Dim arguments As String() = e.CommandArgument.ToString().Split({"|"c}, 2)
                Response.RedirectLocation = "parent"
                Response.Redirect(String.Format("../UDS/UDSView.aspx?Type=UDS&IdUDS={0}&IdUDSRepository={1}", arguments(0), arguments(1)))
            Case "ViewDocs"
                ''Forzo la presa in carico automatica (se prevista)
                CurrentPecMailId = Integer.Parse(e.CommandArgument.ToString)
                PecUnhandled = False
                Response.RedirectLocation = "parent"
                Response.Redirect("../PEC/PECView.aspx?" & CommonShared.AppendSecurityCheck(String.Format("Type=Pec&PECId={0}{1}", e.CommandArgument, If(ProtocolBoxEnabled, "&ProtocolBox=True", String.Empty))))
            Case "ViewSummary", "ViewReplay", "ViewForward"
                ''Forzo la presa in carico automatica (se prevista)
                CurrentPecMailId = Integer.Parse(e.CommandArgument.ToString)
                PecUnhandled = False
                Response.RedirectLocation = "parent"
                Response.Redirect("../PEC/PECSummary.aspx?" & CommonShared.AppendSecurityCheck(String.Format("Type=Pec&PECId={0}{1}", e.CommandArgument, If(ProtocolBoxEnabled, "&ProtocolBox=True", String.Empty))))
        End Select
    End Sub

    Protected Sub dgMail_ItemDataBound(ByVal sender As System.Object, ByVal e As GridItemEventArgs) Handles dgMail.ItemDataBound
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

        With DirectCast(e.Item.FindControl("imgDirection"), Image)
            If item.Direction.HasValue Then
                .Visible = True
                If item.Direction.Value = PECMailDirection.Ingoing Then
                    .ImageUrl = "../App_Themes/DocSuite2008/imgset16/receiveMail.png"
                    .AlternateText = "Mail in ingresso"
                ElseIf item.Direction.Value = PECMailDirection.Outgoing Then
                    .ImageUrl = "../App_Themes/DocSuite2008/imgset16/sendEmail.png"
                    .AlternateText = "Mail in uscita"
                End If
            Else
                .Visible = False
                row.Item("cDirection").Text = WebHelper.Space
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
                If ProtocolEnv.VisualizzaLibricinoIngoing Then
                    .Icon.PrimaryIconUrl = GetPrimaryIconUrl(item.DocumentUnitType.Value)
                End If

                If ProtocolEnv.PecColorProtocolLogEnabled Then
                    If item.IsSendRolesLog Then
                        .BackColor = Drawing.Color.LawnGreen
                        .Style.Add(HtmlTextWriterStyle.BackgroundImage, "none")
                    Else
                        .BackColor = Drawing.Color.Red
                        .Style.Add(HtmlTextWriterStyle.BackgroundImage, "none")
                    End If
                End If

                .CommandName = GetCommandName(item.DocumentUnitType.Value)
                .CommandArgument = GetCommandArgument(item)
                .ToolTip = GetToolTip(item)
            Else
                .Visible = False
                row.Item("cProtocol").Text = WebHelper.Space
            End If
        End With

        ' Aggiungo le icone delle coccarde
        With DirectCast(e.Item.FindControl("imgIsPEC"), Image)
            Dim kvp As KeyValuePair(Of String, String) = CoccardaManager.GetImage(item, ProtocolEnv.CoccardaProtocolEnabled)
            If Not String.IsNullOrEmpty(kvp.Key) Then
                .Visible = True
                .ImageUrl = kvp.Key
                .ToolTip = kvp.Value
            Else
                .Visible = False
                row.Item("cIsPEC").Text = WebHelper.Space
            End If
        End With

        With DirectCast(e.Item.FindControl("imgMoved"), Image)
            If item.HasMove.GetValueOrDefault(False) Then
                .Visible = True
                .ImageUrl = ImagePath.SmallMoveToFolder
            Else
                .Visible = False
                row.Item("cMoved").Text = WebHelper.Space
            End If
        End With

        actionIfColumnVisibile("Handler", Sub()
                                              Dim lblHandler As Label = DirectCast(e.Item.FindControl("lblHandler"), Label)
                                              lblHandler.Text = item.Handler
                                              If ProtocolEnv.DomainLookUpEnabled AndAlso lblHandler IsNot Nothing AndAlso Not String.IsNullOrEmpty(item.Handler) Then
                                                  lblHandler.Text = CommonAD.GetDisplayName(item.Handler)
                                              End If
                                          End Sub)

        With DirectCast(e.Item.FindControl("imgViewReplay"), RadButton)
            If item.LastReplyMailId.HasValue AndAlso item.LastReplyMailId > 0 Then
                .Visible = True
                .CommandArgument = item.LastReplyMailId.ToString()
                .Image.ImageUrl = ImagePath.SmallEmailReply
            Else
                .Visible = False
                row.Item("Reply").Text = WebHelper.Space
            End If
        End With

        With DirectCast(e.Item.FindControl("imgViewForward"), RadButton)
            If item.LastForwardMailId.HasValue AndAlso item.LastForwardMailId > 0 Then
                .Visible = True
                .CommandArgument = item.LastForwardMailId.ToString()
                .Image.ImageUrl = ImagePath.SmallEmailForward
            Else
                .Visible = False
                row.Item("Forward").Text = WebHelper.Space
            End If
        End With

        With DirectCast(e.Item.FindControl("imgCc"), Image)
            If Not String.IsNullOrEmpty(item.MailRecipientsCc) AndAlso item.ReceivedAsCc.HasValue AndAlso item.ReceivedAsCc.Value Then
                .Visible = True
                .ImageUrl = ImagePath.SmallMailIsCc
            Else
                .Visible = False
                row.Item("cIsCc").Text = WebHelper.Space
            End If
        End With

        Dim name As String = RegexHelper.MatchName(item.MailSenders)
        Dim address As String = RegexHelper.MatchEmail(item.MailSenders)
        actionIfColumnVisibile("Da", Sub()
                                         If String.IsNullOrEmpty(name) OrElse name.Eq(address) Then
                                             With DirectCast(e.Item.FindControl("lblAddress"), Label)
                                                 .Text = HttpUtility.HtmlEncode(String.Format("<{0}>", address))
                                             End With
                                         Else
                                             With DirectCast(e.Item.FindControl("lblAddress"), Label)
                                                 .Text = HttpUtility.HtmlEncode(String.Format("'{0}' <{1}>", name, address))
                                             End With
                                         End If
                                     End Sub)

        actionIfColumnVisibile("MailSubject", Sub()
                                                  Dim lblSubject As Label = TryCast(e.Item.FindControl("lblSubject"), Label)
                                                  lblSubject.Font.Bold = Not item.HasRead.GetValueOrDefault(False)
                                                  ' Fix dimensione massima caratteri
                                                  If DocSuiteContext.Current.ProtocolEnv.MaxNumberOfCharsObjectInGridPec > 0 AndAlso Not String.IsNullOrEmpty(item.MailSubject) AndAlso item.MailSubject.Length > DocSuiteContext.Current.ProtocolEnv.MaxNumberOfCharsObjectInGridPec Then
                                                      lblSubject.Text = item.MailSubject.Remove(DocSuiteContext.Current.ProtocolEnv.MaxNumberOfCharsObjectInGridPec) & "  [...]"
                                                      lblSubject.ToolTip = item.MailSubject
                                                  Else
                                                      lblSubject.Text = item.MailSubject
                                                  End If
                                              End Sub)
        ' Dimensione
        actionIfColumnVisibile("PecSize", Sub()
                                              Dim lblPecSize As Label = TryCast(e.Item.FindControl("lblPecSize"), Label)
                                              If item.Size.HasValue Then
                                                  lblPecSize.Visible = True
                                                  lblPecSize.Text = item.Size.Value.ToByteFormattedString(1)
                                              Else
                                                  lblPecSize.Visible = False
                                                  row.Item("PecSize").Text = WebHelper.Space
                                              End If
                                          End Sub)

        ' Destinatari
        Dim lblMailBoxName As Label = TryCast(e.Item.FindControl("lblMailBoxName"), Label)
        If lblMailBoxName IsNot Nothing Then
            lblMailBoxName.Text = HttpUtility.HtmlEncode(item.MailBoxName)
        End If

        actionIfColumnVisibile("colMailBoxName", Sub()
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
                                                                 lblRecipient.Text = String.Concat(HttpUtility.HtmlEncode(mailRecipients).Remove(DocSuiteContext.Current.ProtocolEnv.MaxNumberOfCharsRecipientInGridPec), "  [...]")
                                                                 lblRecipient.ToolTip = mailRecipients
                                                             Else
                                                                 lblRecipient.Text = HttpUtility.HtmlEncode(mailRecipients)
                                                             End If
                                                         End If
                                                     End If
                                                 End Sub)
    End Sub

    Private Sub actionIfColumnVisibile(columnUniqueId As String, action As Action)
        Dim col As GridColumn
        If Not String.IsNullOrEmpty(columnUniqueId) Then
            col = dgMail.Columns.FindByUniqueName(columnUniqueId)
            If col IsNot Nothing AndAlso col.Visible Then
                action()
            End If
        End If
    End Sub

    Private Sub cmdAggiornaDgMail_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdAggiornaDgMail.Click
        Dim selectedId As Short
        If Short.TryParse(ddlMailbox.SelectedValue, selectedId) Then
            CommonShared.SelectedPecMailBoxId = selectedId
        Else
            CommonShared.SelectedPecMailBoxId = Nothing
        End If

        dgMail.DiscardFinder()
        DataBindMailGrid()
    End Sub

    Private Sub CmdHandleClick(sender As Object, e As EventArgs) Handles cmdHandle.Click
        Dim err As Boolean = False
        For Each mail As PECMail In GetSelectedPecMails(dgMail)
            If String.IsNullOrEmpty(mail.Handler) OrElse mail.Handler.Eq(DocSuiteContext.Current.User.FullUserName) Then
                mail.Handler = DocSuiteContext.Current.User.FullUserName
                Facade.PECMailFacade.Update(mail)
            Else
                err = True
            End If
        Next
        If err Then
            AjaxAlert("Non tutte le PEC sono state prese in carico in quanto già assegnate ad altro operatore")
        End If
        DataBindMailGrid()
    End Sub

    Private Sub cmdClearFilters_Click(sender As Object, e As EventArgs) Handles cmdClearFilters.Click
        txtFilterSender.Text = String.Empty
        txtFilterSubject.Text = String.Empty

        chkShowHandled.Checked = True

        chkTypePEC.Checked = False
        chkTypeAnomalia.Checked = False
        chkTypeNotify.Checked = False

        rdbShowRecorded.SelectedIndex = ProtocolEnv.PECDefaultInitialView
        dtpShowRecordedFrom.SelectedDate = Nothing
        dtpShowRecordedTo.SelectedDate = Nothing
        dtpMailDateFrom.SelectedDate = Today.AddDays(DaysToAddInitialized)
        dtpMailDateTo.SelectedDate = Today

        InitializeDestination()

        dgMail.DiscardFinder()
        DataBindMailGrid()
    End Sub

    Private Sub cmdNewMail_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdNewMail.Click
        Dim url As New StringBuilder()
        url.AppendFormat("PECInsert.aspx?Type=Pec&SimpleMode={0}", ProtocolEnv.PECSimpleMode.ToString())
        If (ddlMailbox IsNot Nothing) AndAlso (ddlMailbox.SelectedItem IsNot Nothing) AndAlso (Not String.IsNullOrEmpty(ddlMailbox.SelectedItem.Value)) AndAlso (Not ddlMailbox.SelectedItem.Value.Eq("ALL")) Then
            url.AppendFormat("&SelectedMailboxId={0}", ddlMailbox.SelectedItem.Value)
        End If
        Response.Redirect(url.ToString())
    End Sub


    Protected Sub cmdDocuments_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDocuments.Click
        If Not GetSelectedPecMail(Of Integer)(Function(f) GetSelectedPecMailIds(f), dgMail, CurrentPecMailIdList) Then
            Exit Sub
        End If

        Dim serialized As String = JsonConvert.SerializeObject(CurrentPecMailIdList)
        Dim encoded As String = HttpUtility.UrlEncode(serialized)
        Dim redirectUrl As String = "{0}/viewers/PecMailViewer.aspx?PECIds={1}"
        redirectUrl = String.Format(redirectUrl, DocSuiteContext.Current.CurrentTenant.DSWUrl, encoded)
        Response.Redirect(redirectUrl)
    End Sub

    Protected Sub cmdMove_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdMove.Click
        Dim selectedPecMails As IList(Of PECMail)
        If Not GetSelectedPecMail(Of PECMail)(Function(f) GetSelectedPecMails(f), dgMail, selectedPecMails) Then
            Exit Sub
        End If

        Dim ids As New List(Of Integer)
        For Each mail As PECMail In selectedPecMails
            If Not String.IsNullOrEmpty(mail.Handler) AndAlso Not mail.Handler.Eq(DocSuiteContext.Current.User.FullUserName) Then
                AjaxAlert("Non tutte le PEC selezionate sono in carico all'operatore corrente.")
                Exit Sub
            End If

            ids.Add(mail.Id)
        Next
        Response.Redirect(String.Concat("PECMoveMails.aspx?Type=PEC&ProtocolBox=", ProtocolBoxEnabled, "&PECIds=", JsonConvert.SerializeObject(ids)))
    End Sub

    Protected Sub cmdClonePec_Click(sender As Object, e As EventArgs) Handles cmdClonePec.Click
        Dim selectedPecMails As IList(Of PECMail)
        If Not GetSelectedPecMail(Of PECMail)(Function(f) GetSelectedPecMails(f), dgMail, selectedPecMails) OrElse
            Not CheckAllSelectedPecMailIsActive(selectedPecMails) Then
            Exit Sub
        End If

        For Each pec As PECMail In selectedPecMails.Where(Function(f) Not f.Year.HasValue)
            Facade.PECMailFacade.Duplicate(pec)
        Next

        cmdAggiornaDgMail_Click(sender, e)
        For Each pec As PECMail In selectedPecMails
            For Each gridPecItem As GridDataItem In dgMail.Items
                If DirectCast(gridPecItem.GetDataKeyValue("Id"), Integer) = pec.Id Then
                    gridPecItem.Selected = True
                End If
            Next
        Next
    End Sub

    Private Sub cmdForward_Click(sender As Object, e As EventArgs) Handles cmdForward.Click
        FordwardMail(CurrentMailBox, dgMail)
    End Sub

    Private Sub chkVisCestino_CheckedChanged(sender As Object, e As EventArgs) Handles chkVisCestino.CheckedChanged
        SetCestinoButtons()
        dgMail.DiscardFinder()
        DataBindMailGrid()
    End Sub

    Private Sub cmdRestore_Click(sender As Object, e As EventArgs) Handles cmdRestore.Click
        Dim toRestoreMails As IList(Of PECMail) = GetSelectedPecMails(dgMail)
        If toRestoreMails.IsNullOrEmpty() Then
            AjaxAlert("Nessuna {0} da ripristinare.", PecLabel)
            DataBindMailGrid()
            Exit Sub
        End If

        For Each mail As PECMail In toRestoreMails
            mail.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Active)
            Facade.PECMailFacade.Update(mail)
            Facade.PECMailLogFacade.Restored(mail)
        Next

        AjaxAlert("Gli elementi selezionati sono stati ripristinati.")
        DataBindMailGrid()
    End Sub

    Private Sub cmdSvuotaCestino_Click(sender As Object, e As EventArgs) Handles cmdSvuotaCestino.Click
        Dim finder As NHibernatePECMailFinder = CType(dgMail.Finder, NHibernatePECMailFinder)
        finder.EnablePaging = False
        Dim toDeleteMails As IList(Of PECMail) = finder.DoSearch()
        If toDeleteMails.IsNullOrEmpty() Then
            AjaxAlert("Nessuna {0} da eliminare.", PecLabel)
            DataBindMailGrid()
            Exit Sub
        End If

        Facade.PECMailFacade.LogicDelete(toDeleteMails.Where(Function(m) m.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Delete)).ToList())

        'For Each mail As PECMail In toDeleteMails.Where(Function(m) m.IsActive = 0)
        '    mail.IsActive = 2
        '    Facade.PECMailFacade.Update(mail)
        '    Facade.PECMailLogFacade.InsertLog(mail, "isActive ---> 2 per svuotamento cestino", PECMailLogType.Delete)
        'Next

        AjaxAlert("Il cestino è stato svuotato.")
        DataBindMailGrid()
    End Sub

    Private Sub cmdAttach_Click(sender As Object, e As EventArgs) Handles cmdAttach.Click
        CurrentPecMailIdList = GetSelectedPecMailIds(dgMail)
    End Sub

    Protected Sub cmdViewLog_Click(sender As Object, e As EventArgs) Handles cmdViewLog.Click
        Dim selectedPecMails As IList(Of PECMail)
        If Not GetSelectedPecMail(Of PECMail)(Function(f) GetSelectedPecMails(f), dgMail, selectedPecMails) Then
            Exit Sub
        End If
        CurrentPecMailIdList = GetSelectedPecMailIds(dgMail)
        Server.Execute("../PEC/PECViewLog.aspx")
    End Sub

    Protected Sub chkReceiptNotLinked_CheckedChanged(sender As Object, e As EventArgs) Handles chkReceiptNotLinked.CheckedChanged
        If chkReceiptNotLinked.Checked Then
            Dim chbTS As Dictionary(Of String, Boolean) = New Dictionary(Of String, Boolean)
            chbTS.Add("chkTypePEC", chkTypePEC.Checked)
            chbTS.Add("chkTypeAnomalia", chkTypeAnomalia.Checked)
            chbTS.Add("chkTypeNotify", chkTypeNotify.Checked)

            chkTypePEC.Checked = True
            chkTypePEC.Enabled = False
            chkTypeAnomalia.Enabled = False
            chkTypeAnomalia.Checked = False
            chkTypeNotify.Enabled = False
            chkTypeNotify.Checked = False
            CheckBoxesTypeState = chbTS
        Else
            chkTypePEC.Checked = CheckBoxesTypeState("chkTypePEC")
            chkTypeAnomalia.Checked = CheckBoxesTypeState("chkTypeAnomalia")
            chkTypeNotify.Checked = CheckBoxesTypeState("chkTypeNotify")

            chkTypePEC.Enabled = True
            chkTypeAnomalia.Enabled = True
            chkTypeNotify.Enabled = True
        End If

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

    Private Sub InitializeAjax()
        AddHandler MasterDocSuite.AjaxManager.AjaxRequest, AddressOf PECIncomingMails_AjaxRequest

        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, dgMail, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, tblFilterDestinati)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlFilterLeft)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlMailbox, pnlFilterRight)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlPECMailBoxIncluded, dgMail, MasterDocSuite.AjaxDefaultLoadingPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(dgMail, dgMail, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlMailbox, dgMail, MasterDocSuite.AjaxDefaultLoadingPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(cmdHandle, dgMail, MasterDocSuite.AjaxDefaultLoadingPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAggiornaDgMail, dgMail, MasterDocSuite.AjaxDefaultLoadingPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(cmdNewMail, dgMail, MasterDocSuite.AjaxDefaultLoadingPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(cmdClearFilters, dgMail, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdClearFilters, pnlFilterLeft)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdClearFilters, pnlFilterRight)

        AjaxManager.AjaxSettings.AddAjaxSetting(chkVisCestino, dgMail, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(chkVisCestino, cmdRestore)
        AjaxManager.AjaxSettings.AddAjaxSetting(chkVisCestino, cmdSvuotaCestino)
        AjaxManager.AjaxSettings.AddAjaxSetting(chkVisCestino, cmdDelete)

        AjaxManager.AjaxSettings.AddAjaxSetting(cmdRestore, dgMail, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdRestore, cmdRestore)

        AjaxManager.AjaxSettings.AddAjaxSetting(cmdSvuotaCestino, dgMail, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdSvuotaCestino, cmdSvuotaCestino)

        'AjaxManager.AjaxSettings.AddAjaxSetting(collapseSection, dgMail)
        'AjaxManager.AjaxSettings.AddAjaxSetting(headerSection, dgMail)
    End Sub

    Private Sub PECIncomingMails_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        If e.Argument.Eq("InitialPageLoad") Then
            InitializeDestination()
            DataBindMailGrid()
        End If
    End Sub

    Private Sub Initialize()
        chkShowHandled.Checked = True
        cmdMove.Visible = ProtocolEnv.MassivePecMove
        cmdClonePec.Visible = ProtocolEnv.MassivePECCloneEnabled
        cmdViewLog.Visible = CommonShared.HasGroupPecMailLogViewRight
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

        ' Nascondo le date di protocollazione per la ricerca
        If ProtocolEnv.ShowRecordedDates Then
            tblFilterVisualizzaDate.Style.Remove("dispaly")
        Else
            tblFilterVisualizzaDate.Style.Add("display", "none")
        End If

        ' Naming dei bottoni
        cmdNewMail.Text = String.Format("Nuova {0}", PecLabel)
        cmdClonePec.Text = String.Format("Duplica {0}", PecLabel)
        cmdForward.Text = String.Format("Inoltra {0}", PecLabel)

    End Sub

    Private Sub InitializePECMailBoxIncluded()
        ddlPECMailBoxIncluded.Items.Clear()

        ddlPECMailBoxIncluded.Items.Add(New RadComboBoxItem(EnumHelper.GetDescription(IncludedPECTypes.Unmanaged), CType(IncludedPECTypes.Unmanaged, Short).ToString()))
        ddlPECMailBoxIncluded.Items.Add(New RadComboBoxItem(EnumHelper.GetDescription(IncludedPECTypes.Managed), CType(IncludedPECTypes.Managed, Short).ToString()) With {.Selected = True})
        ddlPECMailBoxIncluded.Items.Add(New RadComboBoxItem(EnumHelper.GetDescription(IncludedPECTypes.Both), CType(IncludedPECTypes.Both, Short).ToString()))
        ddlPECMailBoxIncluded.Visible = False
    End Sub
    Private Sub InitializeCollapsedSection()
        If ProtocolEnv.CollapsePECHeaderEnabled Then
            collapseSection.Visible = True
        End If
    End Sub

    Private Sub InitializeMailboxes()
        ddlMailbox.Items.Clear()
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
        ddlMailbox.Items.Add(New RadComboBoxItem("Tutte", "ALL") With {.Selected = CommonShared.HasGroupAdministratorRight})
        ddlPECMailBoxIncluded.Visible = CommonShared.HasGroupAdministratorRight

        'Definisco il titolo in modo personalizzato
        ddlMailBoxLabel.InnerText = String.Format("Casella {0}", If(ProtocolBoxEnabled, "e-mail:", "PEC:"))
    End Sub

    Private Sub InitializeExternalTrashbin()
        If ProtocolBoxEnabled Then
            Title = "Casella di protocollazione"
        Else
            Title = "PEC - POSTA IN ARRIVO"
        End If

        If ProtocolEnv.PECExternalTrashbin Then
            cmdDelete.Visible = False
            chkVisCestino.Checked = ExternalTrashbin
            chkVisCestino.Style.Add("display", "none")
            If ExternalTrashbin Then
                tblFilterVisualizzaDate.Style.Add("display", "none")
            Else
                tblFilterVisualizzaDate.Style.Remove("display")
            End If
            trFilterSender.Style.Remove("display")
            If ExternalTrashbin Then
                trFilterSender.Style.Add("display", "none")
                If ProtocolBoxEnabled Then
                    Title = "Casella di protocollazione - POSTA ELIMINATA"
                Else
                    Title = "PEC - POSTA ELIMINATA"
                End If
            End If
            dgMail.Columns.FindByUniqueName("cDirection").Visible = ExternalTrashbin
            Exit Sub
        End If

        trFilterSender.Style.Remove("display")
        chkVisCestino.Style.Remove("display")
    End Sub

    Private Function GetSelectedValueOrDefault(dropdown As RadComboBox) As String
        If dropdown.Items.Count = 0 _
            OrElse String.IsNullOrEmpty(dropdown.SelectedValue) _
            OrElse dropdown.SelectedValue.Eq("--") Then
            Return Nothing
        End If

        Return dropdown.SelectedValue.Trim()
    End Function

    Private Function GetFilterSender() As String
        Dim selectedMailBox As String = GetSelectedValueOrDefault(ddlMailbox)
        If String.IsNullOrEmpty(selectedMailBox) Then
            Return Nothing
        End If

        If ddlMailbox.Items.Count = 1 Then
            Return Nothing
        End If

        '' Imposto il mittente solo in modalità Casella di protocollazione con almeno una casella con ProtocolBoxExplicit a False
        If AutoSender Then
            txtFilterSender.Text = UserMail()
        End If

        Return txtFilterSender.Text.Trim()
    End Function

    Private Function GetFilterDestinated() As Boolean?
        If Not DocSuiteContext.Current.ProtocolEnv.IsPECDestinationEnabled OrElse Not CurrentMailBox.IsDestinationEnabled.GetValueOrDefault(False) Then
            Return Nothing
        End If

        If Facade.PECMailboxFacade.IsRoleUserManager(CurrentMailBox) Then
            Return chkDestinati.Checked
        End If

        If chkDaDestinare.Checked Then
            Return Nothing
        End If

        Return True
    End Function

    Private Sub SetFinderInForm()

        Dim finder As NHibernatePECMailFinder = CurrentFinder
        If finder IsNot Nothing Then
            ' Carico i dati sull'interfaccia
            If finder.MailboxIds.Length = 1 Then
                ' una sola casella selezionata
                ddlMailbox.SelectedValue = finder.MailboxIds(0).ToString()
                ddlPECMailBoxIncluded.Visible = False
            Else
                ' ALL
                ddlMailbox.SelectedValue = "ALL"
                ddlPECMailBoxIncluded.Visible = True
            End If

            If finder.Actives IsNot Nothing Then
                If Not DocSuiteContext.Current.ProtocolEnv.PECExternalTrashbin Then
                    chkVisCestino.Checked = Not finder.Actives.GetValueOrDefault(False)
                End If
                chkShowHandled.Checked = (finder.Handler = String.Empty)

                If finder.MailDateFrom.HasValue Then
                    dtpMailDateFrom.SelectedDate = finder.MailDateFrom.Value
                End If

                If finder.MailDateTo.HasValue Then
                    dtpMailDateTo.SelectedDate = finder.MailDateTo.Value
                End If

                txtFilterSubject.Text = finder.MailSubject
                txtFilterSender.Text = finder.Sender

                If Not finder.PecMailTypeIncluded.IsNullOrEmpty() Then
                    chkTypePEC.Checked = finder.PecMailTypeIncluded.Contains(PECMailType.PEC)
                    chkTypeAnomalia.Checked = finder.PecMailTypeIncluded.Contains(PECMailType.Anomalia)
                    chkTypeNotify.Checked = finder.PecMailTypeIncluded.Contains(PECMailType.Notifica)
                End If

                If Not finder.OnlyNotRecorded.GetValueOrDefault(False) Then
                    'chkShowRecorded.Checked = True impostare a Checked il corrispettivo radiobutton
                    If finder.RecordedDateFrom.HasValue Then
                        dtpShowRecordedFrom.SelectedDate = finder.RecordedDateFrom.Value
                    End If
                    If finder.RecordedDateTo.HasValue Then
                        dtpShowRecordedTo.SelectedDate = finder.RecordedDateTo.Value
                    End If
                Else
                    'chkShowRecorded.Checked = False Togliere il checked dal rispettivo button
                    dtpShowRecordedFrom.SelectedDate = Nothing
                    dtpShowRecordedTo.SelectedDate = Nothing
                End If
                chkInverted.Checked = finder.InvertedSort

                'Verifico se era stata fatta una richiesta specifica di visualizzazione
                If (finder.RecordedInDocSuite = 1) Then
                    'Solo protocollati
                    rdbShowRecorded.SelectedIndex = 2
                ElseIf (finder.OnlyNotRecorded = False) Then
                    'Tutti
                    rdbShowRecorded.SelectedIndex = 0
                Else
                    'Solo da protocollare
                    rdbShowRecorded.SelectedIndex = 1
                End If
            Else
                ' Imposto le date standard
                dtpMailDateFrom.SelectedDate = Today.AddDays(DaysToAddInitialized)
                dtpMailDateTo.SelectedDate = Today
            End If

        Else
            ' Imposto le date standard
            dtpMailDateFrom.SelectedDate = Today.AddDays(DaysToAddInitialized)
            dtpMailDateTo.SelectedDate = Today

            Dim defaultMailBox As PECMailBox = Facade.PECMailboxFacade.GetDefault(MailBoxes)
            If defaultMailBox IsNot Nothing Then
                ddlMailbox.SelectedValue = defaultMailBox.Id.ToString()
            End If

        End If

        If CurrentMailBox IsNot Nothing AndAlso CurrentMailBox.LoginError Then
            AjaxAlert("La casella PEC ha un problema di configurazione. Avvisare il responsabile per la corretta configurazione")
        End If

        'Se ancora non c'è un valore di default significa che non è stata mai fatta una ricerca
        If (rdbShowRecorded.SelectedIndex = -1) Then
            rdbShowRecorded.SelectedIndex = ProtocolEnv.PECDefaultInitialView
        End If

    End Sub

    Private Function GetFinderByForm() As NHibernatePECMailFinder
        Dim finder As New NHibernatePECMailFinder()

        Dim filterSender As String = GetFilterSender()
        If filterSender Is Nothing Then
            Return Nothing
        End If

        finder.Sender = filterSender

        finder.Direction = PECMailDirection.Ingoing ' SOLO IN ENTRATA

        ' Se è attiva la gestione del cestino esterno, visualizzo anche la posta inviata.
        If ExternalTrashbin Then
            finder.Direction = Nothing
        End If

        finder.Actives = Not chkVisCestino.Checked
        finder.Anomalies = Not chkVisCestino.Checked AndAlso chkAnomalies.Checked

        finder.Handler = String.Empty
        If Not chkShowHandled.Checked Then
            finder.Handler = DocSuiteContext.Current.User.FullUserName
        End If

        finder.Destinated = Nothing

        '' Imposta i filtri sulle caselle in modo coerente con le caselle di protocollazione
        If ddlMailbox.SelectedValue.Eq("ALL") Then
            Dim query As IEnumerable(Of PECMailBox) = MailBoxes.Where(Function(b) Facade.PECMailboxFacade.IsRealPecMailBox(b))
            If ddlPECMailBoxIncluded.SelectedIndex = 1 Then
                query = query.Where(Function(b) Not String.IsNullOrWhiteSpace(b.IncomingServerName))
            End If
            If ddlPECMailBoxIncluded.SelectedIndex = 0 Then
                query = query.Where(Function(b) String.IsNullOrWhiteSpace(b.IncomingServerName))
            End If
            If rdbShowRecorded.SelectedIndex = 1 AndAlso Not DocSuiteContext.Current.ProtocolEnv.VirtualPECMailBoxProtocolableEnabled Then
                query = query.Where(Function(x) Not String.IsNullOrEmpty(x.IncomingServerName))
            End If
            finder.MailboxIds = query.Select(Function(b) b.Id).ToArray()
            If ProtocolBoxEnabled Then
                finder.MailBoxIdsBySender = New KeyValuePair(Of String, Short())(
                    UserMail,
                    MailBoxes.Where(Function(b) Facade.PECMailboxFacade.IsRealPecMailBox(b) AndAlso (Not b.IsProtocolBoxExplicit.HasValue OrElse Not b.IsProtocolBoxExplicit.Value)).Select(Function(b) b.Id).ToArray())
            End If
        Else
            Dim mailbox As PECMailBox = CurrentMailBox
            finder.MailboxIds = New Short() {mailbox.Id}
            If ProtocolBoxEnabled AndAlso (Not mailbox.IsProtocolBoxExplicit.HasValue OrElse Not mailbox.IsProtocolBoxExplicit.Value) Then
                finder.MailBoxIdsBySender = New KeyValuePair(Of String, Short())(
                    UserMail(),
                    New Short() {mailbox.Id})
            End If
        End If

        If Not ddlMailbox.SelectedValue.Eq("ALL") Then
            finder.Destinated = GetFilterDestinated()
        End If

        finder.MailDateFrom = dtpMailDateFrom.SelectedDate
        finder.MailDateTo = dtpMailDateTo.SelectedDate

        finder.MailSubject = txtFilterSubject.Text.Trim()

        finder.XTrasporto = String.Empty

        finder.PecMailTypeIncluded = New List(Of PECMailType)
        If chkTypePEC.Checked Then
            finder.PecMailTypeIncluded.Add(PECMailType.PEC)
        End If

        If chkTypeAnomalia.Checked Then
            finder.PecMailTypeIncluded.Add(PECMailType.Anomalia)
        End If

        If chkTypeNotify.Checked Then
            finder.PecMailTypeIncluded.Add(PECMailType.Notifica)
        End If

        ' Visualizza anche Mail protocollate.
        Select Case rdbShowRecorded.SelectedIndex
            Case 0
                finder.OnlyNotRecorded = False
                ' Filtro per data protocollazione.
                If dtpShowRecordedFrom.SelectedDate.HasValue Then
                    finder.RecordedDateFrom = dtpShowRecordedFrom.SelectedDate
                End If
                If dtpShowRecordedTo.SelectedDate.HasValue Then
                    finder.RecordedDateTo = dtpShowRecordedTo.SelectedDate
                End If
            Case 1
                finder.OnlyNotRecorded = True
                finder.RecordedDateFrom = Nothing
                finder.RecordedDateTo = Nothing
            Case 2
                finder.RecordedInDocSuite = 1
                ' Filtro per data protocollazione.
                If dtpShowRecordedFrom.SelectedDate.HasValue Then
                    finder.RecordedDateFrom = dtpShowRecordedFrom.SelectedDate
                End If
                If dtpShowRecordedTo.SelectedDate.HasValue Then
                    finder.RecordedDateTo = dtpShowRecordedTo.SelectedDate
                End If
        End Select

        ' Ordino per data
        finder.InvertedSort = chkInverted.Checked

        ' Filtro per PEC scompagnate. Sono tutte le PEC di ricevuta che sono legate a una mail inviata NON inviata da DocSuite
        If chkReceiptNotLinked.Checked Then
            If Not finder.PecMailTypeIncluded.Contains(PECMailType.PEC) Then
                finder.PecMailTypeIncluded.Add(PECMailType.PEC)
            End If
            finder.ReceiptNotLinked = True
        End If

        finder.EnablePaging = dgMail.AllowPaging
        finder.CustomPageIndex = 0
        Return finder
    End Function

    Private Sub DataBindMailGrid()
        If dgMail.Finder Is Nothing Then
            Dim finder As NHibernatePECMailFinder = GetFinderByForm()
            If finder Is Nothing Then
                Return
            End If

            dgMail.Finder = finder
            dgMail.CurrentPageIndex = 0
            dgMail.CustomPageIndex = 0
        End If

        dgMail.PageSize = dgMail.Finder.PageSize

        ' Salvo il Finder in Sessione (gestione BACK)
        If ProtocolBoxEnabled Then
            SessionSearchController.SaveSessionFinder(dgMail.Finder, SessionSearchController.SessionFinderType.ProtocolBoxFinderType)
        Else
            SessionSearchController.SaveSessionFinder(dgMail.Finder, SessionSearchController.SessionFinderType.PecInFinderType)
        End If

        ' Salvo in sessione l'ora di registrazione
        Session.Add("DSW_PECINFinderType", DateTime.Now)

        dgMail.DataBindFinder()
    End Sub

    Protected Function PecMailContentUrl() As String
        Return ResolveUrl("~/PEC/PECView.aspx")
    End Function

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

    Private Sub InitializeHandling()
        '' Attivo la colonna Handler solo se il parametro è impostato
        dgMail.Columns.FindByUniqueName("Handler").Visible = IsHandleEnabled

        If IsHandleEnabled Then
            tblFilterHandler.Style.Remove("display")
        Else
            tblFilterHandler.Style.Add("display", "none")
        End If
    End Sub

    Private Function UserCanSend() As Boolean
        Dim visibleMSendingailBoxes As IList(Of PECMailBox) = Facade.PECMailboxFacade.GetVisibleSendingMailBoxes()
        Dim realMailBoxes As IList(Of PECMailBox) = Facade.PECMailboxFacade.FillRealPecMailBoxes(visibleMSendingailBoxes)
        Return realMailBoxes.Count > 0
    End Function

    Private Sub SetCestinoButtons()
        If chkVisCestino.Checked Then
            cmdDelete.Visible = False
            cmdRestore.Visible = True
            If CommonShared.UserConnectedBelongsTo(ProtocolEnv.GruppoPECEmptyRecycleBin) Then
                cmdSvuotaCestino.Visible = True
            End If

        Else
            cmdDelete.Visible = ProtocolEnv.PECDeleteButtonEnabled
            cmdRestore.Visible = False
            cmdSvuotaCestino.Visible = False
        End If
        If Not IsPostBack Then
            cmdDelete.PostBackUrl = String.Format("~/PEC/PECDelete.aspx?Type=PEC&ProtocolBox={0}", ProtocolBoxEnabled)
        End If
    End Sub

    Private Sub ValidateFinder()
        '''rimuovo il finder della griglia per ricaricare i dati in maniera asincrona
        '''così da non bloccare l'utente in apertura della pagina
        If dgMail.Finder IsNot Nothing Then
            dgMail.DiscardFinder()
        End If
    End Sub

    Private Sub InitializeCurrentMailBox()
        If ProtocolBoxEnabled Then
            '' Nascondo il controllo sul mittente solo se:
            '' 1. è selezionata una specifica casella (e non tutte) --> se tutte mostro il controllo
            '' 2. la casella selezionata non è di tipo esplicito --> se esplicita mostro il controllo
            txtFilterSender.Enabled = IsProtocolBoxExplicit

            '' Disattivo il pannello di tipologia in quanto gli elementi presenti saranno necessariamente solo mail normali
            trFilterType.Style.Add("display", "none")

            '' Disattivo il pannello di ordinamento se non esplicitamente richiesto
            If Not ProtocolEnv.ProtocolBoxShowOrdinamento Then
                trFilterOrder.Style.Add("display", "none")
            End If

            cmdNewMail.Visible = False
            cmdMove.Visible = ProtocolEnv.ProtocolBoxAllButtonEnabled
            cmdForward.Visible = ProtocolEnv.ProtocolBoxAllButtonEnabled
        End If
    End Sub

    ''' <summary> Gestione Distribuzione Protocollo </summary>
    Private Sub InitializeDestination()
        Dim mailbox As PECMailBox = CurrentMailBox
        If ProtocolEnv.IsPECDestinationEnabled AndAlso mailbox IsNot Nothing AndAlso mailbox.IsDestinationEnabled Then

            tblFilterDestinati.Style.Remove("display")

            If Facade.PECMailboxFacade.IsRoleUserManager(mailbox) Then
                ' Sono direttore
                chkDestinati.Style.Remove("display")
                chkDaDestinare.Style.Add("display", "none")
                chkDaDestinare.Checked = False
            Else
                ' Sono segretario
                chkDaDestinare.Style.Remove("display")
                chkDaDestinare.Checked = True
                chkDestinati.Style.Add("display", "none")
            End If
        Else
            tblFilterDestinati.Style.Add("display", "none")
        End If
    End Sub

    Private Function UserMail() As String
        Dim email As String = Facade.UserLogFacade.EmailOfUser(DocSuiteContext.Current.User.UserName, DocSuiteContext.Current.User.Domain, True)

        If String.IsNullOrEmpty(email) Then
            Throw New InvalidOperationException("Operatore sprovvisto di indirizzo email, funzionalità non attiva.")
        End If

        Return email
    End Function

#End Region

End Class