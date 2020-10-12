Imports System.Linq
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Xml
Imports System.Collections.Generic
Imports System.Net
Imports Telerik.Web.UI
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Services.Biblos.Models

Partial Class ProtInterop
    Inherits ProtBasePage

#Region " Fields "
    Private _segnatureFilePath As String
    Private _segnatureFileVirtualPath As String
    Private _destinationMailBox As PECMailBox
    Private _draftMailBox As PECMailBox
#End Region

#Region " Properties "

    Public Property InteropMessage() As Boolean
        Get
            If Me.ViewState("InteropMessage") IsNot Nothing Then
                Return Convert.ToBoolean(Me.ViewState("InteropMessage"))
            End If

            Return False
        End Get
        Set(ByVal value As Boolean)
            Me.ViewState("InteropMessage") = value
        End Set
    End Property

    Public Property HasSenders() As Boolean
        Get
            If Me.ViewState("HasSenders") IsNot Nothing Then
                Return Convert.ToBoolean(Me.ViewState("HasSenders"))
            End If

            Return False
        End Get
        Set(ByVal value As Boolean)
            Me.ViewState("HasSenders") = value
        End Set
    End Property

    Public Property CurrentSignature() As String
        Get
            If ViewState("Segnatura") IsNot Nothing Then
                Return ViewState("Segnatura").ToString()
            End If

            Return String.Empty
        End Get
        Set(ByVal value As String)
            ViewState("Segnatura") = value
        End Set
    End Property

    Public Property ValidationResult() As SegnaturaValidationResult
        Get
            If Me.ViewState("ValidationResult") IsNot Nothing Then
                Return CType(Me.ViewState("ValidationResult"), SegnaturaValidationResult)
            End If

            Return New SegnaturaValidationResult()
        End Get
        Set(ByVal value As SegnaturaValidationResult)
            Me.ViewState("ValidationResult") = value
        End Set
    End Property

    Protected ReadOnly Property PecEMailInsertUrl() As String
        Get
            Return Request.FilePath.Replace("ProtInterop.aspx", "ProtPecEmailAddress.aspx")
        End Get
    End Property

    Private ReadOnly Property DestinationMailBox As PECMailBox
        Get
            If _destinationMailBox Is Nothing Then
                Dim mailBoxId As Short
                If Not Short.TryParse(ddlPECAddress.SelectedValue, mailBoxId) Then
                    Throw New InvalidCastException("DestinationMailBox non valido.")
                End If
                _destinationMailBox = Facade.PECMailboxFacade.GetById(mailBoxId)
            End If
            If _destinationMailBox Is Nothing Then
                Throw New KeyNotFoundException("DestinationMailBox mancante.")
            End If

            Return _destinationMailBox
        End Get
    End Property

    Private ReadOnly Property DraftMailBox As PECMailBox
        Get
            If _draftMailBox Is Nothing Then
                If DocSuiteContext.Current.ProtocolEnv.PECDraftMailBoxId = -1 Then
                    Throw New InvalidCastException("PECDraftMailBoxId non valido in ParameterEnv di protocollo.")
                End If
                _draftMailBox = Facade.PECMailboxFacade.GetById(DocSuiteContext.Current.ProtocolEnv.PECDraftMailBoxId)
            End If
            If _draftMailBox Is Nothing Then
                Throw New KeyNotFoundException("DraftMailBox mancante.")
            End If
            Return _draftMailBox
        End Get
    End Property

    Protected Function WindowWidth() As Integer
        Return ProtocolEnv.PECWindowWidth
    End Function

    Protected Function WindowHeight() As Integer
        Return ProtocolEnv.PECWindowHeight
    End Function

    Protected Function WindowBehaviors() As String
        Return ProtocolEnv.PECWindowBehaviors
    End Function

    Protected Function WindowPosition() As String
        Return ProtocolEnv.PECWindowPosition
    End Function

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim partialName As String = CommonUtil.UserDocumentName & "-Interop-Segnatura.xml"
        _segnatureFilePath = CommonInstance.AppTempPath & partialName
        _segnatureFileVirtualPath = System.Web.HttpContext.Current.Request.Url.Scheme & System.Uri.SchemeDelimiter & System.Web.HttpContext.Current.Request.Url.Authority & System.Web.HttpContext.Current.Request.ApplicationPath() & "Temp/" & partialName

        ' Check-boxes sempre visibili
        uscProtocollo.ControlRecipients.EnableCheck = True

        ' Abilito invio documenti originali.
        uscProtDocument.SendSourceDocument = True
        uscProtAttachs.SendSourceDocument = uscProtDocument.SendSourceDocument

        If Not IsPostBack Then
            Initialize()
            InitializeProtocolControl()
        End If

        If InteropMessage Then
            btnSegnatura.Visible = True
            Page.Title = "Protocollo - Interoperabilità"
        Else : Page.Title = "Protocollo - PEC"
        End If

        If Not Me.IsPostBack Then
            uscProtocollo.ControlRecipients.CheckAllNodes()
            initializeRoleControl()
        End If
    End Sub

    Private Sub btnSegnatura_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSegnatura.Click
        Dim xDoc As XmlDocument = (New Segnatura(CurrentSignature, DocSuiteContext.PecSegnature)).GetXmlSignature()
        xDoc.RemoveChild(xDoc.DocumentType)
        xDoc.Save(_segnatureFilePath)
        Response.Redirect(_segnatureFileVirtualPath)
    End Sub

    Protected Sub uscProtocollo_MittenteAdded(ByVal sender As Object, ByVal e As EventArgs) Handles uscProtocollo.MittenteAdded
        If uscProtocollo.ControlSenders.GetContacts(False).Count = 1 Then
            ' salva mittenti
            If Not HasSenders Then
                For Each contactDto As ContactDTO In uscProtocollo.ControlSenders.GetContacts(False)
                    Select Case contactDto.Type
                        Case contactDto.ContactType.Address
                            CurrentProtocol.AddSender(contactDto.Contact, contactDto.IsCopiaConoscenza)
                        Case contactDto.ContactType.Manual
                            CurrentProtocol.AddSenderManual(contactDto.Contact, contactDto.IsCopiaConoscenza)
                    End Select
                Next

                Try
                    Facade.ProtocolFacade.UpdateOnly(CurrentProtocol)
                Catch ex As Exception
                    FileLogger.Warn(LoggerName, "Errore durante il salvataggio del mittente.", ex)
                    AjaxAlert(String.Format("Errore durante il salvataggio del mittente: {0}", ex.Message))
                End Try
            End If

            If InteropMessage AndAlso String.IsNullOrEmpty(CurrentSignature) Then
                GenerateSegnatura()
            End If
            updForm.Update()

        End If
    End Sub

    Protected Sub uscSettori_RoleSelected(ByVal sender As System.Object, ByVal e As RoleEventArgs) Handles uscSettori.RoleSelected
        If e.Role IsNot Nothing Then Me.BindMailboxes(e.Role)
        Me.updForm.Update()
    End Sub

    Protected Sub ddlPECAddress_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ddlPECAddress.SelectedIndexChanged
        If Me.ddlPECAddress.SelectedValue <> "--" Then
            btnMail.Enabled = (InteropMessage AndAlso ValidationResult.IsValid) OrElse Not InteropMessage
            cmdCreateMail.Enabled = (InteropMessage AndAlso ValidationResult.IsValid) OrElse Not InteropMessage
        Else
            btnMail.Enabled = False
            cmdCreateMail.Enabled = False
        End If
        Me.updForm.Update()
    End Sub

    Protected Sub manager_AjaxRequest(ByVal sender As Object, ByVal e As Telerik.Web.UI.AjaxRequestEventArgs)
        Me.updForm.Update()
    End Sub

    Private Sub cmdCreateMail_Click(sender As Object, e As System.EventArgs) Handles cmdCreateMail.Click
        uscProtocollo.CurrentProtocol = CurrentProtocol
        If CheckContacts() Then
            Dim draft As PECMail = CreateMail()
            Dim jsScript As String = String.Format("OpenPECMailViewer({0}, {1});", draft.Id, DestinationMailBox.Id)
            AjaxManager.ResponseScripts.Add(jsScript)
        End If
    End Sub

    Private Sub uscProtocollo_RefreshContact(ByVal sender As Object, ByVal e As EventArgs) Handles uscProtocollo.RefreshContact
        uscProtocollo.CurrentProtocol = CurrentProtocol
        uscProtocollo.LoadContacts(False)
    End Sub

    Private Sub btnMail_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnMail.Click
        If CheckContacts() Then
            Try
                InsertMailInDB(CurrentProtocol, Facade.PECMailboxFacade.GetById(Short.Parse(ddlPECAddress.SelectedValue)), GetSignatura(), txtNote.Text)
            Catch ex As WebException
                FileLogger.Warn(LoggerName, ex.Message, ex)
                AjaxAlert(String.Format("Servizio di invio mail non disponibile: {0}.", ex.Message))
            Catch ex As Exception
                FileLogger.Warn(LoggerName, ex.Message, ex)
                AjaxAlert(String.Format("Errore durante l'inserimento nella coda di invio dei messaggi: {0}.", ex.Message))
            End Try
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        uscProtocollo.CurrentProtocol = CurrentProtocol

        ' Se configurata una PECMailBox per la gestione delle bozze nascondo la modalità di invio diretto.
        btnMail.Style.Add("display", "none")
        cmdCreateMail.Style.Add("display", "none")
        If DocSuiteContext.Current.ProtocolEnv.PECDraftMailBoxId = -1 Then
            btnMail.Style.Remove("display")
        Else
            cmdCreateMail.Style.Remove("display")
        End If

        If Not CommonUtil.HasGroupAdministratorRight Then
            btnSegnatura.Style.Add("display", "none")
        Else : btnSegnatura.Style.Remove("display")
        End If

        ' Messaggio Interoperabile
        Dim interopInt As Integer = 0
        Integer.TryParse(Me.Request.QueryString("im"), interopInt)
        Me.InteropMessage = Convert.ToBoolean(interopInt)

        InitializeAjax()
        SetDocumenti()
    End Sub

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf manager_AjaxRequest

        AjaxManager.AjaxSettings.AddAjaxSetting(btnMail, updForm, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdCreateMail, updForm, MasterDocSuite.AjaxDefaultLoadingPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscProtDocument)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscProtAttachs)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscSettori, uscSettori)
    End Sub

    Private Sub InitializeProtocolControl()
        Dim s As String = String.Empty
        'verifica Protocollo
        If CurrentProtocol Is Nothing Then
            Throw New DocSuiteException($"Protocollo ID {CurrentProtocolId}", "Protocollo Inesistente", Request.Url.ToString(), DocSuiteContext.Current.User.FullUserName)
        End If

        ' i protocolli in ingresso non sono spedibili via interoperabilità
        If CurrentProtocol.Type.Id = -1 Then
            Throw New DocSuiteException($"Protocollo n. {CurrentProtocol.FullNumber}", "I protocolli in ingresso non possono essere inviati attraverso l'interoperabilità", Request.Url.ToString(), DocSuiteContext.Current.User.FullUserName)
        End If

        'If Not RolesCheckUserRight() Then
        If Not CurrentProtocolRights.IsPecSendable Then
            Throw New DocSuiteException($"Protocollo n. {CurrentProtocol.FullNumber}", "Mancano diritti di Autorizzazione sul protocollo", Request.Url.ToString(), DocSuiteContext.Current.User.FullUserName)
        End If

        If Not DocSuiteContext.Current.ProtocolEnv.IsInteropEnabled Then
            Throw New DocSuiteException($"Protocollo n. {CurrentProtocol.FullNumber}", "Interoperabilità non abilitata", Request.Url.ToString(), DocSuiteContext.Current.User.FullUserName)
        End If

        Dim senders As New List(Of ContactDTO)
        CurrentProtocol.GetContacts(senders, New List(Of ContactDTO))
        CurrentProtocol.GetManualContacts(senders, New List(Of ContactDTO))

        If senders.Count > 1 Then
            Throw New DocSuiteException($"Protocollo n. {CurrentProtocol.FullNumber}", "Il mittente dev'essere univoco", Request.Url.ToString(), DocSuiteContext.Current.User.FullUserName)
        End If

        Me.HasSenders = (senders.Count > 0)

        uscProtocollo.VisibleProtocollo = True
        uscProtocollo.VisibleMittentiDestinatari = (DocSuiteContext.Current.ProtocolEnv.IsInteropEnabled)

        If Me.HasSenders Then
            uscProtocollo.ContactMittenteModifyEnable = False
            If Me.InteropMessage Then Me.GenerateSegnatura()
        Else
            uscProtocollo.ContactMittenteModifyEnable = Me.InteropMessage
        End If

        uscProtocollo.VisibleOggetto = True
        uscProtocollo.VisibleProtocolloMittente = False
        uscProtocollo.VisibleAltri = False
        uscProtocollo.VisibleClassificazione = False
        uscProtocollo.VisibleFascicolo = False
        uscProtocollo.VisibleStatoProtocollo = False
        uscProtocollo.VisibleTipoDocumento = False
        uscProtocollo.VisibleFatturazione = False
        uscProtocollo.VisibleStatoProtocollo = False
        uscProtocollo.VisibleAssegnatario = False
        uscProtocollo.VisibleScatolone = False

    End Sub

    Private Sub initializeRoleControl()
        ' Ritiro tutti i miei ruoli
        Dim myRoles As IList(Of Role) = Facade.RoleFacade.GetUserRoles(DSWEnvironment.Protocol, Nothing, True)
        'Facade.RoleFacade.GetRoleRigths("PROT", CommonShared.UserConnectedGroups, 1, "", Nothing, False)

        ' Prendo tutti i ruoli senza padre
        Dim roleFacade As New RoleFacade()
        Dim tmpRoleList As IList(Of Role) = roleFacade.GetRootItems(withPECMailbox:=True)

        ' modifica temporanea
        Dim roleList As New List(Of Role)
        If tmpRoleList IsNot Nothing Then
            roleList = (From roleToCheck In tmpRoleList Where myRoles.Contains(roleToCheck)).ToList()
        End If

        ' Mostra tutte le foglie con pec associate
        uscSettori.ReadOnly = True
        uscSettori.SourceRoles = roleList
        uscSettori.DataBind(True, True)

        ' Seleziono il settore se è uno solo
        Dim roleTree As RadTreeView = uscSettori.FindControl("RadTreeSettori")
        If roleTree.Nodes.Count.Equals(0) Then
            Exit Sub
        End If

        If roleTree.Nodes(0).Nodes.Count.Equals(1) Then
            Dim singleNode As RadTreeNode = roleTree.Nodes(0).Nodes(0)
            Dim a As New RoleEventArgs(Facade.RoleFacade.GetById(CType(singleNode.Value, Integer)))
            singleNode.Selected = True
            uscSettori_RoleSelected(roleTree, a)
        End If
    End Sub

    ''' <summary> Mostra tutte le email relative al settore, e abilita l'invio della mail </summary>
    Private Sub BindMailboxes(ByVal role As Role)
        ddlPECAddress.Items.Clear()

        Dim mailBoxes As IList(Of PECMailBox) = role.GetSendEnabledMailBoxes()
        If mailBoxes IsNot Nothing AndAlso mailBoxes.Count > 0 Then

            For Each mailbox As PECMailBox In mailBoxes
                If (CurrentProtocolRights.IsInteroperable AndAlso InteropMessage AndAlso mailbox.IsForInterop) _
                    OrElse CurrentProtocolRights.IsPECSendable AndAlso Not InteropMessage AndAlso Not mailbox.IsForInterop Then
                    ddlPECAddress.Items.Add(New ListItem(mailbox.MailBoxName, mailbox.Id.ToString))
                End If
            Next

            If ddlPECAddress.Items.Count = 1 Then
                ddlPECAddress.Items(0).Selected = True
                btnMail.Enabled = (InteropMessage AndAlso ValidationResult.IsValid) OrElse Not InteropMessage
                cmdCreateMail.Enabled = (InteropMessage AndAlso ValidationResult.IsValid) OrElse Not InteropMessage
            ElseIf ddlPECAddress.Items.Count > 1 Then
                Dim fakeItem As New ListItem("- Selezionare una casella di posta -", "--")
                ddlPECAddress.Items.Insert(0, fakeItem)
                fakeItem.Selected = True
                btnMail.Enabled = False
                cmdCreateMail.Enabled = False
            End If

            'Se non carica mailBox certamente non caricherà ddlPECAddress
            ddlPECAddress.Enabled = ddlPECAddress.Items.Count > 0
        End If
    End Sub

    Private Sub SetDocumenti()

        uscProtDocument.LoadBiblosDocuments(CurrentProtocol.Location.ProtBiblosDSDB, CurrentProtocol.IdDocument.Value, False, True)
        If uscProtDocument.SendSourceDocument Then
            uscProtDocument.TreeViewControl.Nodes(0).Checkable = False
        End If

        If CurrentProtocol.IdAttachments.HasValue AndAlso CurrentProtocol.IdAttachments > 0 Then
            uscProtAttachs.Visible = True
            uscProtAttachs.LoadBiblosDocuments(CurrentProtocol.Location.ProtBiblosDSDB, CurrentProtocol.IdAttachments.Value, True, True)
            If uscProtAttachs.SendSourceDocument Then
                uscProtAttachs.TreeViewControl.Nodes(0).Checkable = False
            End If
        End If

    End Sub

    Private Sub GenerateSegnatura()
        If CurrentSignature.Length = 0 Then
            Try
                ' creazione segnatura
                Dim sendersDto As New List(Of ContactDTO)
                Dim recipientsDto As New List(Of ContactDTO)
                Dim segnatura As New Segnatura(CurrentProtocol, DocSuiteContext.PecSegnature)

                sendersDto.AddRange(uscProtocollo.ControlSenders.GetContacts(False))
                recipientsDto.AddRange(uscProtocollo.ControlRecipients.GetContacts(False))

                If sendersDto.Count > 0 AndAlso recipientsDto.Count > 0 Then
                    segnatura.LoadContactList(sendersDto, ContactDirection.Sender)
                    segnatura.LoadContactList(recipientsDto, ContactDirection.Recipient)

                    Dim segnaturaXml As XmlDocument = segnatura.GetXmlSignature()

                    ' validazione segnatura
                    Me.ValidationResult = segnatura.IsInteropSignatureValid()
                    If Me.ValidationResult.IsValid Then
                        btnSegnatura.Enabled = True
                        Me.CurrentSignature = segnaturaXml.OuterXml
                    Else
                        Dim validationException As String = ""

                        If segnatura.ValidationException Is Nothing Then
                            btnSegnatura.Enabled = True
                            Me.CurrentSignature = segnaturaXml.OuterXml
                        Else
                            validationException = segnatura.ValidationException.Message
                            btnSegnatura.Enabled = False
                        End If
                        AjaxAlert(String.Format("{0} - {1}", Me.ValidationResult.NotValidMessage, validationException))
                    End If
                End If
            Catch ex As Exception
                btnSegnatura.Enabled = False
                AjaxAlert(ex.Message)
            End Try
        End If
    End Sub

    Private Function AddNotesToSegnature(ByRef xmldoc As XmlDocument, ByRef descrizione As XmlElement, ByVal note As String) As Boolean
        If Not (note Is Nothing) AndAlso (note.Length > 0) Then
            Dim xmlNote As XmlElement = xmldoc.CreateElement("Note")
            xmlNote.InnerText = note
            descrizione.AppendChild(xmlNote)
        End If
    End Function

    Private Overloads Function PopulateAttachments(archive As String, idchain As Integer?, ByVal main As Boolean) As List(Of PECMailAttachment)
        Dim retAttachs As New List(Of PECMailAttachment)

        Dim docs As List(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(archive, idchain.Value)
        For Each doc As BiblosDocumentInfo In docs

            Dim attach As New PECMailAttachment() With {
                .AttachmentName = doc.PDFName,
                .IsMain = main}

            retAttachs.Add(attach)
        Next

        Return retAttachs
    End Function

    ''' <summary>
    ''' Popola un elenco PECMailAttachments a partire dai nodi documento selezionati nell'albero.
    ''' </summary>
    ''' <remarks>Il valore dei nodi deve essere nel formato "Server|Database|ChainId|Enum".</remarks>
    Private Overloads Shared Function PopulateAttachments(ByVal p_nodes As IList(Of Telerik.Web.UI.RadTreeNode)) As IList(Of PECMailAttachment)
        Dim retval As IList(Of PECMailAttachment) = New List(Of PECMailAttachment)
        Dim currentFileRef As String()

        For Each n As Telerik.Web.UI.RadTreeNode In p_nodes
            currentFileRef = n.Value.Split("|"c)
            If currentFileRef.Length.Equals(3) Then
                Dim doc As New BiblosDocumentInfo(currentFileRef(0), Integer.Parse(currentFileRef(1)), Integer.Parse(currentFileRef(2)))

                Dim item As New PECMailAttachment
                item.IsMain = False
                item.AttachmentName = doc.Name

                retval.Add(item)
            End If
        Next
        Return retval
    End Function

    ''' <summary> Accodamento in DB </summary>
    Private Function InsertMailInDB(ByVal protocol As Protocol, ByVal mailbox As PECMailBox, ByVal segnatura As String, ByVal mailBody As String) As PECMail

        Dim pecMail As PECMail = New PECMail()
        pecMail.Year = protocol.Year
        pecMail.Number = protocol.Number
        pecMail.DocumentUnit = Facade.DocumentUnitFacade.GetById(protocol.Id)
        pecMail.Direction = PECMailDirection.Outgoing
        pecMail.MailSubject = protocol.ProtocolObject
        pecMail.MailType = PECMailTypes.Invio
        pecMail.Segnatura = segnatura
        pecMail.MailPriority = Convert.ToInt16(ddlPriority.SelectedValue)
        pecMail.MailBody = mailBody
        pecMail.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Active)
        pecMail.IsValidForInterop = ValidationResult.IsValid
        pecMail.Attachments = New List(Of PECMailAttachment)

        pecMail.MailSenders = mailbox.MailBoxName

        ' destinatari
        Dim recipientAddresses As New List(Of String)
        For Each contactDto As ContactDTO In uscProtocollo.ControlRecipients.GetContacts(False)
            ' Aggiungo ai destinatari della mail solo quelli spuntati dall'utente
            If contactDto.IsCopiaConoscenza Then
                If String.IsNullOrEmpty(contactDto.Contact.CertifiedMail) Then
                    recipientAddresses.Add(contactDto.Contact.EmailAddress)
                Else
                    recipientAddresses.Add(contactDto.Contact.CertifiedMail)
                End If
            End If
        Next
        If recipientAddresses.Count.Equals(0) Then
            Throw New Exception("E' necessario selezionare almeno un destinatario")
        End If
        pecMail.MailRecipients = String.Join(",", recipientAddresses.ToArray())

        ' mailbox
        pecMail.MailBox = mailbox
        mailbox.Mails.Add(pecMail)

        ' allegati
        Dim attachments As New List(Of PECMailAttachment)
        attachments.AddRange(PopulateAttachments(protocol.Location.ProtBiblosDSDB, protocol.IdDocument, True))
        attachments.AddRange(PopulateAttachments(protocol.Location.ProtBiblosDSDB, protocol.IdAttachments, False))
        ' Aggiungo gli originali qualora selezionati.
        If uscProtDocument.SendSourceDocument Then
            attachments.AddRange(PopulateAttachments(uscProtDocument.TreeViewControl.CheckedNodes))
        End If
        If uscProtAttachs.SendSourceDocument Then
            attachments.AddRange(PopulateAttachments(uscProtAttachs.TreeViewControl.CheckedNodes))
        End If

        For Each attachment As PECMailAttachment In attachments
            attachment.Mail = pecMail
            pecMail.Attachments.Add(attachment)
        Next

        Try
            pecMail.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Delete)
            Facade.PECMailFacade.Save(pecMail)

            If Not pecMail.Year.HasValue Then
                pecMail.Year = protocol.Year
                Facade.PECMailFacade.Update(pecMail)
            End If

            ' Salvo gli allegati prima di convalidare l'email
            For Each attachment As PECMailAttachment In pecMail.Attachments
                Facade.PECMailAttachmentFacade.Save(attachment)
            Next

            AjaxAlert("Mail inserita correttamente nella coda di invio.")

            btnMail.Enabled = False
            cmdCreateMail.Enabled = False

            ' Solo alla fine rendo attiva l'email creata
            pecMail.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Active)
            Facade.PECMailFacade.Save(pecMail)

            Return pecMail
        Catch ex As Exception
            FileLogger.Warn(LoggerName, String.Format("Errore durante l'inserimento nella coda di invio del messaggio: {0}.", ex.Message), ex)
            AjaxAlert(String.Format("Errore durante l'inserimento nella coda di invio del messaggio: {0}.", ex.Message))
        End Try

        Return pecMail
    End Function

    Protected Function PECMailViewerUrl() As String
        Return ResolveUrl("~/PEC/PECView.aspx")
    End Function

    ''' <summary> Crea una PECMail temporanea per l'anteprima di invio. </summary>
    Private Function CreateMail() As PECMail
        Dim retval As New PECMail()

        retval.Year = CurrentProtocol.Year
        retval.Number = CurrentProtocol.Number
        retval.DocumentUnit = Facade.DocumentUnitFacade.GetById(CurrentProtocol.Id)
        retval.MailSubject = CurrentProtocol.ProtocolObject
        retval.Direction = PECMailDirection.Outgoing
        retval.MailType = PECMailTypes.Invio
        retval.Segnatura = GetSignatura()
        retval.MailPriority = CShort(ddlPriority.SelectedValue) ' MailPriority è Short e gli viene passato in conversione un Int16
        retval.MailBody = txtNote.Text
        retval.IsValidForInterop = ValidationResult.IsValid

        ' Creo la bozza nell'apposita mailbox.
        retval.MailBox = DraftMailBox

        ' Imposto come mittenti i destinatari della mailbox di destinazione.
        retval.MailSenders = DestinationMailBox.MailBoxName

        ' Destinatari
        Dim recipients As New List(Of String)
        For Each contact As ContactDTO In uscProtocollo.ControlRecipients.GetContacts(False)
            ' Aggiungo ai destinatari della mail solo quelli selezionati dall'utente.
            If contact.IsCopiaConoscenza Then
                If String.IsNullOrEmpty(contact.Contact.CertifiedMail) Then
                    recipients.Add(contact.Contact.EmailAddress.Trim())
                Else
                    recipients.Add(contact.Contact.CertifiedMail.Trim())
                End If
            End If
        Next
        If recipients.Count < 1 Then
            Throw New Exception("Selezionare almeno un destinatario.")
        End If
        retval.MailRecipients = String.Join("; ", recipients.ToArray())

        ' Popolo lista allegati.
        Dim attachments As New List(Of PECMailAttachment)
        attachments.AddRange(PopulateAttachments(CurrentProtocol.Location.ProtBiblosDSDB, CurrentProtocol.IdDocument, True))
        attachments.AddRange(PopulateAttachments(CurrentProtocol.Location.ProtBiblosDSDB, CurrentProtocol.IdAttachments, False))

        ' Aggiungo gli originali qualora selezionati.
        If uscProtDocument.SendSourceDocument Then
            attachments.AddRange(PopulateAttachments(uscProtDocument.TreeViewControl.CheckedNodes))
        End If
        If uscProtAttachs.SendSourceDocument Then
            attachments.AddRange(PopulateAttachments(uscProtAttachs.TreeViewControl.CheckedNodes))
        End If

        If attachments.Count > 0 Then
            retval.Attachments = New List(Of PECMailAttachment)
            For Each attachment As PECMailAttachment In attachments
                attachment.Mail = retval
                retval.Attachments.Add(attachment)
            Next
        End If

        Try
            retval.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Delete)
            Facade.PECMailFacade.Save(retval)

            ' Salvo gli allegati prima di convalidare l'email.
            If retval.Attachments IsNot Nothing AndAlso retval.Attachments.Count > 0 Then
                For Each attachment As PECMailAttachment In retval.Attachments
                    Facade.PECMailAttachmentFacade.Save(attachment)
                Next
            End If
            cmdCreateMail.Enabled = False
            Facade.PECMailFacade.Update(retval)
            Facade.PECMailLogFacade.Drafted(retval, DestinationMailBox.Id)
            Return retval
        Catch ex As Exception
            Dim message As String = String.Format("Errore durante la creazione del messaggio: {0}.", ex.Message)
            FileLogger.Warn(LoggerName, message, ex)
            AjaxAlert(message)
        End Try

        Return retval
    End Function

    Private Function CheckContacts() As Boolean

        If Me.ddlPECAddress.SelectedValue = "--" Then
            Return False 'Nessuna casella di posta selezionata
        End If

        If Not Me.InteropMessage Then

            Dim noMailContactIds As String = ""
            Dim noMailContacts As String = ""

            For Each contactDto As ContactDTO In uscProtocollo.ControlRecipients.GetContacts(False)
                If contactDto.IsCopiaConoscenza AndAlso String.IsNullOrEmpty(contactDto.Contact.EmailAddress) AndAlso String.IsNullOrEmpty(contactDto.Contact.CertifiedMail) Then
                    If contactDto.Type = 0 Then
                        noMailContactIds &= contactDto.IdManualContact.ToString() & "$"
                    Else
                        noMailContactIds &= contactDto.Contact.Id.ToString() & "$"
                    End If
                    noMailContacts &= Replace(contactDto.Contact.Description, "|", " ") & "$"
                End If
            Next

            If noMailContacts.Length > 0 Then
                noMailContactIds = noMailContactIds.TrimEnd("$"c)
                noMailContacts = noMailContacts.TrimEnd("$"c)

                'Me.lblErrore.Text = String.Format("Impossibile inviare il messaggio PEC, alcuni destinatari non possiedono l' indirizzo e-mail: {0}.", noMailContacts.Replace("$", ", ").Replace("|"c, " "c))
                Dim script As String = String.Format("OpenPecMailAddressWindow('{0}', '', '{1}', '{2}');", Server.UrlEncode(noMailContactIds), Server.UrlEncode(noMailContacts.Replace("'", String.Empty)), "Impossibile inviare il messaggio PEC, alcuni destinatari non possiedono un indirizzo e-mail.")
                ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "InsertEmails", script, True)

                Return False
            End If
        End If
        Return True
    End Function

    Private Function GetSignatura() As String
        Dim retval As String = String.Empty

        If InteropMessage Then
            If ValidationResult.IsValid Then
                Try
                    Dim sign As New Segnatura(CurrentSignature, DocSuiteContext.PecSegnature)
                    Dim xmlDoc As XmlDocument = sign.GetXmlSignature()
                    If Not String.IsNullOrEmpty(txtNote.Text) Then
                        AddNotesToSegnature(xmlDoc, xmlDoc.SelectSingleNode("//Descrizione"), txtNote.Text)
                    End If

                    retval = xmlDoc.OuterXml
                Catch ex As Exception
                    Dim message As String = String.Format("Errore in fase di generazione della Signature: {0}.", ValidationResult.NotValidMessage)
                    Throw New Exception(message)
                End Try
            Else
                Dim message As String = String.Format("Signature non valida: {0}.", ValidationResult.NotValidMessage)
                Throw New Exception(message)
            End If
        End If

        Return retval
    End Function

#End Region

End Class