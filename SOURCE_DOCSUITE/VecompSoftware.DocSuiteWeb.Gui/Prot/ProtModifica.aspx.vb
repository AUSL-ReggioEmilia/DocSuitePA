Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Formatter
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging

Public Class ProtModifica
    Inherits ProtBasePage

#Region " Properties "
    Public ReadOnly Property ProtocolStatusConfirm As String
        Get
            Return JsonConvert.SerializeObject(ProtocolEnv.ProtocolStatusConfirmRequest)
        End Get
    End Property

#End Region
#Region " ContainerControl "

    Private Overloads ReadOnly Property CurrentContainerControl As ContainerControl
        Get
            Return New ContainerControl(cmbContainer, rcbContainer)
        End Get
    End Property

    Private Sub RcbContainerItemsRequested(sender As RadComboBox, e As RadComboBoxItemsRequestedEventArgs) Handles rcbContainer.ItemsRequested
        CurrentContainerControl().ClearItems()
        Dim availableContainers As IList(Of Container) = Facade.ContainerFacade.GetManageableContainers(e.Text, CurrentProtocol.Container)
        For Each c As Container In availableContainers
            CurrentContainerControl().AddItem(c.Name, c.Id.ToString())
        Next
        If availableContainers.Count = 1 Then
            ' Se è disponibile un singolo contenitore lo seleziono di default.
            CurrentContainerControl().SelectedValue = availableContainers.Item(0).Id.ToString()
        End If
    End Sub

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()
        ' Blocco la modifica a chi non è autorizzato
        If Not CurrentProtocolRights.CanAddDocuments Then '  CommonUtil.UserCanAddAnnexed Then
            uscAnnexes.ReadOnly = True
        Else
            uscAnnexes.SignButtonEnabled = ProtocolEnv.IsFDQEnabled
            ' Copia da protocollo
            uscAnnexes.ButtonCopyProtocol.Visible = ProtocolEnv.CopyProtocolDocumentsEnabled
            uscAnnexes.ButtonCopyUDS.Visible = ProtocolEnv.UDSEnabled
            ' Copia da atto
            If (DocSuiteContext.Current.IsResolutionEnabled) Then
                uscAnnexes.ButtonCopyResl.Visible = ResolutionEnv.CopyReslDocumentsEnabled
            End If
        End If

        InitializeTab()
        uscProtocollo.CurrentProtocol = CurrentProtocol
        rfvContainer.ControlToValidate = CurrentContainerControl.ActiveControl.ID.ToString()
        If Not IsPostBack Then
            Initialize()
            InitializeDocumentControls()
        End If
    End Sub

    Private Sub InitializeDocumentControls()
        Dim uploaddocumentsLabels As New List(Of Tuple(Of uscDocumentUpload, Model.Entities.DocumentUnits.ChainType))
        uploaddocumentsLabels.Add(New Tuple(Of uscDocumentUpload, Model.Entities.DocumentUnits.ChainType)(uscDocumento, Model.Entities.DocumentUnits.ChainType.MainChain))
        uploaddocumentsLabels.Add(New Tuple(Of uscDocumentUpload, Model.Entities.DocumentUnits.ChainType)(uscAllegati, Model.Entities.DocumentUnits.ChainType.AttachmentsChain))
        uploaddocumentsLabels.Add(New Tuple(Of uscDocumentUpload, Model.Entities.DocumentUnits.ChainType)(uscAnnexes, Model.Entities.DocumentUnits.ChainType.AnnexedChain))
        InitializeDocumentLabels(uploaddocumentsLabels)
    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscDocumento)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscAllegati)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscAnnexes)
    End Sub

    Private Sub BtnConfermaClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnConferma.Click
        If CurrentProtocol Is Nothing Then
            Exit Sub
        End If

        ' Verifico che l'utente abbia i permessi di modifica
        If CurrentProtocolRights.IsEditable OrElse (Action.Eq("Repair") AndAlso CurrentProtocolRights.IsErrorEditable) Then
            If ProtocolEnv.AuthorizContainer <> 0 Then
                If (Convert.ToInt32(CurrentContainerControl.SelectedValue) = ProtocolEnv.AuthorizContainer) Then
                    ' FG20111123
                    ' il contenitore è Autorizzazioni
                    If Not cmbProtocolStatus.SelectedValue.Eq("A") Then
                        ' errore se diverso da Assegnato
                        AjaxAlert("Impossibile salvare. Modificare il contenitore o lo stato del protocollo ad Assegnato")
                        Exit Sub
                    End If
                Else
                    ' il contenitore è diverso da Autorizzazioni
                    If cmbProtocolStatus.SelectedValue.Eq("A") Then
                        ' errore se Assegnato
                        AjaxAlert("Impossibile salvare. Modificare il contenitore o lo stato del protocollo ad un valore diverso da Assegnato")
                        Exit Sub
                    End If
                End If
            End If

            If tblEditClassificazione.Visible AndAlso CurrentFascicleDocumentUnits.Any(Function(f) f.ReferenceType = ReferenceType.Fascicle) Then
                Dim categorySelected As Category = uscClassificatore.SelectedCategories.FirstOrDefault()
                Dim subCategorySelected As Category = uscSottoClassificatore.SelectedCategories.FirstOrDefault()
                If subCategorySelected IsNot Nothing Then
                    categorySelected = subCategorySelected
                End If
                If categorySelected IsNot Nothing AndAlso Not categorySelected.Id.Equals(CurrentProtocol.Category.Id) Then
                    AjaxAlert("Non è possibile modificare il classificatore del documento in quanto già fascicolato.")
                    Exit Sub
                End If
            End If

            LogInsert()

            'Contatti
            If ProtocolEnv.ContactModify.CheckYearModify(CurrentProtocol.Year) Then
                CurrentProtocol.Contacts.Clear()
                CurrentProtocol.ManualContacts.Clear()
                Facade.ProtocolFacade.Update(CurrentProtocol)
                CurrentProtocol.AppendSenders(uscProtocollo.ControlSenders.GetContacts(False))
                CurrentProtocol.AppendRecipients(uscProtocollo.ControlRecipients.GetContacts(False))
            End If

            'Contenitore
            CurrentProtocol.Container = Facade.ContainerFacade.GetById(CType(CurrentContainerControl.SelectedValue, Integer), False, "ProtDB")
            ' Se non ci sono documenti imposto anche la location
            If CurrentProtocol.IdDocument.GetValueOrDefault(0) = 0 AndAlso CurrentProtocol.IdAttachments.GetValueOrDefault(0) = 0 Then
                CurrentProtocol.Location = CurrentProtocol.Container.ProtLocation
            End If

            If CommonShared.HasGroupEditCategoryRight Then
                Dim selectedCategory As Category = uscClassificatore.SelectedCategories.FirstOrDefault()
                If selectedCategory IsNot Nothing Then
                    CurrentProtocol.Category = selectedCategory
                Else
                    FileLogger.Debug(LoggerName, String.Format("Modifica protocollo {0}. Nessun classificatore selezionato in fase di modifica.", CurrentProtocol.FullNumber))
                End If
            Else
                Dim selectedCategroy As Category = uscSottoClassificatore.SelectedCategories.FirstOrDefault()
                'Sotto Categoria
                If tblEditClassificazione.Visible AndAlso selectedCategroy IsNot Nothing Then
                    CurrentProtocol.Category = selectedCategroy
                End If
            End If

            'Pubblicazione su Internet
            If ProtocolEnv.IsPublicationEnabled Then
                CurrentProtocol.CheckPublication = ckbPublication.Checked
            End If

            'Categoria
            CurrentProtocol.ServiceCategory = SelServiceCategory.CategoryText
            'Note
            CurrentProtocol.Note = txtNote.Text
            'Proponente
            CurrentProtocol.Subject = uscSubject.GetContactText()

            'Inserimento Facicoli
            If ProtocolEnv.IsIssueEnabled Then
                'Rimuovo tutti i fascicoli
                Facade.ProtocolFacade.RemoveProtocolContactIssue(CurrentProtocol)
                'Aggiunge i nuovi fascicoli
                For Each contact As ContactDTO In uscFascicoli.GetContacts(False)
                    Facade.ProtocolFacade.AddProtocolContactIssue(CurrentProtocol, contact.Contact)
                Next
            End If

            If DocSuiteContext.Current.ProtocolEnv.IsClaimEnabled Then
                CurrentProtocol.IsClaim = uscProtocollo.IsClaim
            End If

            'record
            If DocSuiteContext.Current.ProtocolEnv.IsStatusEnabled Then
                If Not String.IsNullOrEmpty(cmbProtocolStatus.SelectedValue) Then
                    CurrentProtocol.Status = Facade.ProtocolStatusFacade.GetById(cmbProtocolStatus.SelectedValue)
                Else
                    CurrentProtocol.Status = Nothing
                End If
            End If

            'aggiorno oggetto
            Try
                If (ProtocolEnv.IsChangeObjectEnable OrElse Action.Eq("Repair")) AndAlso (Not String.IsNullOrEmpty(uscObject.Text) AndAlso Not CurrentProtocol.ProtocolObject.Eq(uscObject.Text)) Then
                    Facade.ProtocolFacade.UpdateProtocolObject(CurrentProtocol, CurrentProtocol.ProtocolObject, uscObject.Text, CurrentProtocol.ObjectChangeReason, uscObject.ObjectChangeReason)
                End If
            Catch ex As Exception
                Throw New DocSuiteException("Modifica Protocollo", "Errore in fase di modifica Oggetto", ex)
            End Try

            Dim info As New ProtocolSignatureInfo(uscAllegati.DocumentInfosAdded.Count)
            UpdateDocuments(True, info)

        ElseIf CurrentProtocolRights.IsEditableAttachment Then
            ' Qualora il gruppo abbinato all'utente abbia solo permessi di lettura gestisco il salvataggio del solo allegato.
            UpdateDocuments(False)
        End If

        If Action.Eq("Repair") OrElse (CurrentProtocol.IdDocument.GetValueOrDefault(0) > 0 AndAlso CurrentProtocol.IdStatus = ProtocolStatusId.Incompleto) Then
            Facade.ProtocolFacade.Activation(CurrentProtocol)
        End If

        Facade.ProtocolFacade.Update(CurrentProtocol)
        Facade.ProtocolFacade.RaiseAfterEdit(CurrentProtocol, uscAllegati.DocumentInfosAdded)
        Facade.ProtocolFacade.SendUpdateProtocolCommand(CurrentProtocol)
        Response.Redirect($"../Prot/ProtVisualizza.aspx?{CommonShared.AppendSecurityCheck($"UniqueId={CurrentProtocol.Id}&Type=Prot")}")
    End Sub

    Protected Sub uscClassificatore_CategoryAdding(sender As Object, args As EventArgs) Handles uscClassificatore.CategoryAdding
        uscClassificatore.FromDate = CurrentProtocol.RegistrationDate.Date
    End Sub
#End Region

#Region " Methods "

    Private Sub Initialize()
        ' Visualizza dati protocollo  
        uscProtocollo.VisibleProtocollo = True
        uscProtocollo.VisibleOggetto = True
        uscProtocollo.VisibleTipoDocumento = True
        uscProtocollo.VisibleClassificazione = False
        uscProtocollo.VisibleMittentiDestinatari = False
        uscProtocollo.VisibleProponente = False
        uscProtocollo.VisibleStatoProtocollo = False
        uscProtocollo.VisibleScatolone = False
        uscProtocollo.VisibleHandler = False

        If CurrentProtocolRights.IsEditable OrElse (Action.Eq("Repair") AndAlso CurrentProtocolRights.IsErrorEditable) Then
            uscProtocollo.ContactModifyEnable = ProtocolEnv.ContactModify.CheckYearModify(CurrentProtocol.Year)
            uscProtocollo.ClaimModifyEnable = ProtocolEnv.IsClaimEnabled

            'Recupero Documenti da Biblos
            SetDocumenti()

            'Classificazione
            Dim categories As List(Of Category) = New List(Of Category)
            Dim category As Category = CurrentProtocol.Category
            If Not CommonShared.HasGroupEditCategoryRight Then
                Do While category.Parent IsNot Nothing AndAlso category.Parent.Code > 0
                    category = category.Parent
                Loop
            End If
            categories.Add(category)
            uscClassificatore.DataSource = categories
            uscClassificatore.DataBind()

            'SottoClassificazione
            If CurrentFascicleDocumentUnits.Count > 0 Then
                tblEditClassificazione.Visible = False
            Else
                If CurrentProtocol.Category.Parent IsNot Nothing AndAlso CurrentProtocol.Category.Parent.Code > 0 Then
                    uscSottoClassificatore.SubCategory = CurrentProtocol.Category
                End If
                ' Verifico se l'operatore è abilitato a modificare la Category
                If CommonShared.HasGroupEditCategoryRight Then
                    ' Nascondo lo UC di subcategory
                    tblEditClassificazione.Visible = False
                    uscClassificatore.ReadOnly = False
                Else
                    uscClassificatore.ReadOnly = True
                    uscSottoClassificatore.CategoryID = category.Id
                End If
            End If

            'Oggetto
            If ProtocolEnv.IsChangeObjectEnable Then
                tblEditObject.Visible = True
                uscObject.Text = CurrentProtocol.ProtocolObject
                uscObject.Year = CurrentProtocol.Year.ToString()
                uscObject.Number = CurrentProtocol.Number.ToString()
            End If

            CurrentContainerControl.Enabled = ProtocolEnv.ProtocolContainerEditable
            'Contenitori con diritto di Inserimento
            If CurrentProtocol.Container.IsInvoiceEnable OrElse CurrentProtocolRights.IsRejected OrElse ProtocolEnv.ProtocolRejectionEnabled Then
                ' Con gestione fatture o in caso di rigetto il contenitore è solo una label
                CurrentContainerControl.AddItem(CurrentProtocol.Container.Name, CurrentProtocol.Container.Id.ToString())
                CurrentContainerControl.Enabled = False
            Else
                ' Senza gestione fatture aggiungo tutti i contenitori
                CurrentContainerControl.ClearItems()
                Dim availableContainers As IList(Of Container) = Facade.ContainerFacade.GetManageableContainers("", CurrentProtocol.Container)
                For Each c As Container In availableContainers
                    CurrentContainerControl.AddItem(c.Name, c.Id.ToString())
                Next
            End If
            CurrentContainerControl.SelectedValue = CurrentProtocol.Container.Id.ToString()

            ' Protocol status
            If ProtocolEnv.IsStatusEnabled Then
                tblEditProtocolStatus.Visible = True
                If CurrentProtocol.Status IsNot Nothing Then
                    If CurrentProtocol.Status.Id.Eq("P") Then
                        cmbProtocolStatus.DataSource = Facade.ProtocolStatusFacade.GetAll()
                    Else
                        cmbProtocolStatus.DataSource = Facade.ProtocolStatusFacade.GetByProtocolStatusExclusion("P")
                    End If
                    cmbProtocolStatus.DataBind()
                    cmbProtocolStatus.SelectedValue = CurrentProtocol.Status.Id
                Else
                    cmbProtocolStatus.DataSource = Facade.ProtocolStatusFacade.GetByProtocolStatusExclusion("P")
                    cmbProtocolStatus.DataBind()
                    cmbProtocolStatus.Items.Insert(0, New ListItem("", ""))
                    cmbProtocolStatus.SelectedIndex = 0
                End If
            Else
                tblEditProtocolStatus.Visible = False
            End If

            'fascicolo
            If ProtocolEnv.IsIssueEnabled Then
                tblFascicoli.Visible = True
                Dim contacts As New List(Of ContactDTO)
                For Each contactIssue As ProtocolContactIssue In CurrentProtocol.ContactIssues
                    Dim vContactDto As New ContactDTO
                    vContactDto.Contact = contactIssue.Contact
                    vContactDto.Type = ContactDTO.ContactType.Address
                    If contactIssue IsNot Nothing Then
                        contacts.Add(vContactDto)
                    End If
                Next
                uscFascicoli.DataSource = contacts
                uscFascicoli.DataBind()
            Else
                tblFascicoli.Visible = False
            End If

            If CurrentProtocol.Type.Id = -1 Then
                lblSubject.Text = "Assegnatario: "
            ElseIf CurrentProtocol.Type.Id = 1 Then
                lblSubject.Text = "Proponente: "
            End If
            uscSubject.DataSource = CurrentProtocol.Subject
            SelServiceCategory.CategoryText = CurrentProtocol.ServiceCategory

            uscProtocollo.VisibleNote = False
            txtNote.Text = CurrentProtocol.Note

            'Pubblicazione su Internet
            If DocSuiteContext.Current.ProtocolEnv.IsPublicationEnabled Then
                rowPubblica.Visible = True
                ckbPublication.Checked = CurrentProtocol.CheckPublication
            End If

            If DocSuiteContext.Current.ProtocolEnv.IsClaimEnabled Then
                uscProtocollo.VisibleClaim = True
                uscProtocollo.IsClaim = CurrentProtocol.IsClaim.HasValue AndAlso CurrentProtocol.IsClaim.Value
            Else
                uscProtocollo.VisibleClaim = False
            End If

            If Action.Eq("Repair") Then
                uscDocumento.ReadOnly = False
                uscDocumento.IsDocumentRequired = True
                uscAllegati.ReadOnly = False
                tblEditObject.Visible = True
                uscObject.Text = CurrentProtocol.ProtocolObject
                uscObject.Year = CurrentProtocol.Year.ToString()
                uscObject.Number = CurrentProtocol.Number.ToString()
                uscProtocollo.VisibleOggetto = False
                uscClassificatore.ReadOnly = False
            End If

        ElseIf CurrentProtocolRights.IsEditableAttachment Then
            ' Qualora l'utente abbia solo permessi di lettura permetto l'aggiunta di allegati.
            uscProtocollo.ContactModifyEnable = False ' Disabilito la modifica dei contatti.
            uscProtocollo.ClaimModifyEnable = uscProtocollo.ClaimModifyEnable

            SetDocumenti() ' Recupero Documenti da Biblos

            'Classificazione
            Dim categories As New List(Of Category)
            Dim category As Category = CurrentProtocol.Category
            If Not CurrentFascicleDocumentUnits.Any(Function(f) f.ReferenceType = ReferenceType.Fascicle) Then
                Do While category.Parent IsNot Nothing AndAlso category.Parent.Code > 0
                    category = category.Parent
                Loop
            End If
            categories.Add(category)
            uscClassificatore.DataSource = categories
            uscClassificatore.DataBind()

            'SottoClassificazione
            If CurrentFascicleDocumentUnits.Any(Function(f) f.ReferenceType = ReferenceType.Fascicle) Then
                tblEditClassificazione.Visible = False
                uscClassificatore.CategoryID = CurrentProtocol.Category.Id
            Else
                uscSottoClassificatore.CategoryID = CurrentProtocol.Category.Id
                If CurrentProtocol.Category.Parent IsNot Nothing AndAlso CurrentProtocol.Category.Parent.Code > 0 Then
                    uscSottoClassificatore.SubCategory = CurrentProtocol.Category
                End If
            End If
            tblEditClassificazione.Visible = False ' Disabilito la modifica della classificazione.
            tblEditContenitore.Visible = False ' Disabilito la selezione del contenitore.
            tblEditProtocolStatus.Visible = False ' Disabilito la modifica dello stato protocollo.
            tblFascicoli.Visible = False ' Disabilito la modifica dei fascicoli.
            rowPubblica.Visible = False
            tblOther.Visible = False
            uscProtocollo.FindControl("tblPosteWeb").Visible = False
        End If
    End Sub

    Private Sub InitializeTab()
        If ProtocolEnv.IsStatusEnabled Then
            CurrentContainerControl.AddAjaxSettings(AjaxManager, cmbProtocolStatus)
        End If

        ' Verifica Protocollo
        If CurrentProtocol Is Nothing Then
            Throw New DocSuiteException($"Protocollo ID {CurrentProtocol.Id}", "Protocollo inesistente")
        End If

        ' Verifica diritti
        If Action.Eq("Repair") Then
            If Not CurrentProtocolRightsStatusCancel.IsErrorEditable.GetValueOrDefault(False) Then
                Throw New DocSuiteException($"Protocollo n. {CurrentProtocol.FullNumber}", "Mancano diritti di autorizzazione")
            End If
        Else
            If Not CurrentProtocolRightsStatusCancel.IsEditable AndAlso Not CurrentProtocolRightsStatusCancel.IsRejected AndAlso Not CurrentProtocolRightsStatusCancel.IsEditableAttachment.GetValueOrDefault(False) Then
                Throw New DocSuiteException($"Protocollo n. {CurrentProtocol.FullNumber}", "Mancano diritti di autorizzazione")
            End If
        End If

        MasterDocSuite.HistoryTitle = String.Format("{0} {1}", Title, CurrentProtocol.FullNumber)
    End Sub

    ''' <summary> Carica documenti e allegati nei rispettivi usercontrol. </summary>
    Private Sub SetDocumenti()
        ' Recupero Documento principale
        Dim doc As BiblosDocumentInfo = ProtocolFacade.GetDocument(CurrentProtocol)
        If doc IsNot Nothing Then
            uscDocumento.LoadDocumentInfo(doc)
        Else
            If CurrentProtocol.IdStatus = ProtocolStatusId.Incompleto AndAlso ProtocolEnv.ProtParzialeEnabled Then
                uscDocumento.ReadOnly = False
                uscAllegati.ReadOnly = False
            End If
        End If

        ' Recupero Allegati
        Dim attachs() As BiblosDocumentInfo = ProtocolFacade.GetAttachments(CurrentProtocol)
        If attachs.Length > 0 Then
            uscAllegati.LoadDocumentInfo(attachs.Cast(Of DocumentInfo).ToList())
        End If

        uscAnnexes.DocumentDeletable = ProtocolEnv.EnableFlushAnnexed
        uscAnnexes.ButtonRemoveEnabled = ProtocolEnv.EnableFlushAnnexed

        ' Recupero "Allegati non parte integrante (Annessi)"
        Dim annexes() As BiblosDocumentInfo = ProtocolFacade.GetAnnexes(CurrentProtocol)
        If annexes.Length > 0 Then
            uscAnnexes.LoadDocumentInfo(annexes.Cast(Of DocumentInfo).ToList())
        End If
    End Sub

    Private Sub LogInsert()

        'Log: Modifica campo Note
        CreateFieldChangeLog("Note", CurrentProtocol.Note, txtNote.Text)

        'Log: Modifica Assegnatario/Proponente
        CreateFieldChangeLog("Assegn/Propon.", CurrentProtocol.Subject, uscSubject.GetContactText())

        'Log: Modifica Sottocategoria
        Dim newCategory As Category = Nothing
        Dim newSubCategory As String = ""
        If uscSottoClassificatore.HasSelectedCategories Then
            newCategory = uscSottoClassificatore.SelectedCategories.First()
            newSubCategory = String.Format("{0} ({1})", newCategory.Id, newCategory.GetFullName())
        End If

        Dim oldSubCategory As String = ""
        If newCategory IsNot Nothing AndAlso CurrentProtocol.Category.Id <> newCategory.Id Then
            oldSubCategory = String.Format("{0} ({1})", CurrentProtocol.Category.Id, CurrentProtocol.Category.GetFullName())
        End If
        CreateFieldChangeLog("Stoto classificazione", oldSubCategory, newSubCategory)

        'Log: Modifica Categoria di servizio
        CreateFieldChangeLog("Categ. Serv", CurrentProtocol.ServiceCategory, SelServiceCategory.CategoryText)

        'Log: Modifica Pubblicazione
        If ProtocolEnv.IsPublicationEnabled Then
            If CurrentProtocol.CheckPublication Then
                CreateFieldChangeLog("Pubblicazione Internet", If(CurrentProtocol.CheckPublication, "1", "0"), If(ckbPublication.Checked, "1", "0"))
            Else
                CreateFieldChangeLog("Pubblicazione Internet", "0", If(ckbPublication.Checked, "1", "0"))
            End If
        End If

        'Log: Modifica Stato Protocollo
        If DocSuiteContext.Current.ProtocolEnv.IsStatusEnabled Then
            If CurrentProtocol.Status IsNot Nothing Then
                CreateFieldChangeLog("Stato del protocollo", CurrentProtocol.Status.Id, cmbProtocolStatus.SelectedValue)
            Else
                CreateFieldChangeLog("Stato del protocollo", String.Empty, cmbProtocolStatus.SelectedValue)
            End If
        End If

        'Log: Modifica Contenitore
        If CurrentProtocol.Container IsNot Nothing AndAlso CurrentContainerControl.Visible Then
            Dim currentContainer As String = String.Format("{0} ({1})", CurrentProtocol.Container.Id, CurrentProtocol.Container.Name)
            Dim newContainer As String = String.Format("{0} ({1})", CurrentContainerControl.SelectedValue, CurrentContainerControl.SelectedText)

            CreateFieldChangeLog("Contenitore", currentContainer, newContainer)
        Else
            If Not String.IsNullOrEmpty(CurrentContainerControl.SelectedValue) Then
                Dim newContainer As String = String.Format("{0} ({1})", CurrentContainerControl.SelectedValue, CurrentContainerControl.SelectedText)
                CreateFieldChangeLog("Contenitore", String.Empty, newContainer)
            End If
        End If

        'Log: Modifica contatti
        If ProtocolEnv.ContactModify.CheckYearModify(CurrentProtocol.Year) Then
            'la lista degli usercontrol è quella corretta...da li recupero i contatti aggiunti e rimossi
            Dim removedSenders As New List(Of ContactDTO)
            Dim removedRecipients As New List(Of ContactDTO)
            CurrentProtocol.GetContacts(removedSenders, removedRecipients)
            Dim manualRemovedSenders As New List(Of ContactDTO)
            Dim manualRemovedRecipients As New List(Of ContactDTO)
            CurrentProtocol.GetManualContacts(manualRemovedSenders, manualRemovedRecipients)

            Dim addedRecipients As IList(Of ContactDTO) = uscProtocollo.ControlRecipients.GetAddressContacts(False)
            Dim addedManualRecipients As IList(Of ContactDTO) = uscProtocollo.ControlRecipients.GetManualContacts()
            Dim addedSenders As IList(Of ContactDTO) = uscProtocollo.ControlSenders.GetAddressContacts(False)
            Dim addedManualSenders As IList(Of ContactDTO) = uscProtocollo.ControlSenders.GetManualContacts()

            'Recupera tutti i contatti Mittenti da rubrica aggiunti e cancellati
            Facade.ContactFacade.DeleteSameDTO(addedSenders, removedSenders)
            'Recupera tutti i contatti Mittenti manuali aggiunti e cancellati
            Facade.ContactFacade.DeleteSameDTO(addedManualSenders, manualRemovedSenders)
            'Recupera tutti i contatti Destinatari da rubrica aggiunti e cancellati
            Facade.ContactFacade.DeleteSameDTO(addedRecipients, removedRecipients)
            'Recupera tutti i contatti Destinatari manuali aggiunti e cancellati
            Facade.ContactFacade.DeleteSameDTO(addedManualRecipients, manualRemovedRecipients)

            'Log Mittenti Eliminati
            SbLogInsertContact(removedSenders, "Mittenti {rem}: ")
            'Log Mittenti Inseriti
            SbLogInsertContact(addedSenders, "Mittenti {new}: ")
            'Log Mittenti Manuali Eliminati
            SbLogInsertContact(manualRemovedSenders, "Mittenti Manuali {rem}: ")
            'Log Mittenti Manuali Inseriti
            SbLogInsertContact(addedManualSenders, "Mittenti Manuali {new}: ")

            'Log Destinatari Eliminati
            SbLogInsertContact(removedRecipients, "Destinatari {rem}: ")
            'Log Destinatari Inseriti
            SbLogInsertContact(addedRecipients, "Destinatari {new}: ")
            'Log Destinatari Manuali Eliminati
            SbLogInsertContact(manualRemovedRecipients, "Destinatari Manuali {rem}: ")
            'Log Destinatari Manuali Inseriti
            SbLogInsertContact(addedManualRecipients, "Destinatari Manuali {new}: ")
        End If
    End Sub

    Private Sub SbLogInsertContact(ByVal contactList As IList(Of ContactDTO), ByVal logParamMessage As String)
        For i As Integer = 0 To contactList.Count - 1
            Select Case contactList(i).Type
                Case ContactDTO.ContactType.Address
                    Facade.ProtocolLogFacade.Insert(CurrentProtocol, ProtocolLogEvent.PM, logParamMessage & contactList(i).Contact.Id)
                Case ContactDTO.ContactType.Manual
                    Facade.ProtocolLogFacade.Insert(CurrentProtocol, ProtocolLogEvent.PM, logParamMessage & contactList(i).Contact.Description)
            End Select
        Next
    End Sub

    Private Sub CreateFieldChangeLog(ByVal message As String, ByVal oldValue As String, ByVal newValue As String)
        If oldValue.Eq(newValue) Then
            Exit Sub
        End If

        Facade.ProtocolLogFacade.Insert(CurrentProtocol, ProtocolLogEvent.PM, message & " (old): " & oldValue)
        Facade.ProtocolLogFacade.Insert(CurrentProtocol, ProtocolLogEvent.PM, message & " (new): " & newValue)
    End Sub

    Private Overloads Sub UpdateDocuments(ByVal updateMainDocuments As Boolean)
        UpdateDocuments(updateMainDocuments, New ProtocolSignatureInfo())
    End Sub

    Private Overloads Sub UpdateDocuments(ByVal updateMainDocuments As Boolean, ByVal signatureInfo As ProtocolSignatureInfo)
        Try
            If updateMainDocuments Then
                If uscDocumento.DocumentInfosAdded.Count > 0 Then
                    CurrentProtocol.DocumentCode = uscDocumento.DocumentInfosAdded(0).Name
                    Dim attributes As Dictionary(Of String, String) = Facade.ProtocolFacade.GetDocumentAttributes(CurrentProtocol)
                    Facade.ProtocolFacade.AddDocument(CurrentProtocol, uscDocumento.DocumentInfosAdded(0), signatureInfo, attributes)
                End If
            End If

            'verifico se ci sono allegati aggiunti
            If uscAllegati.DocumentsAddedCount > 0 Then
                If DocSuiteContext.Current.ProtocolEnv.IsConservationEnabled Then
                    CurrentProtocol.ConservationStatus = "M"c
                End If
                Facade.ProtocolFacade.AddAttachments(CurrentProtocol, uscAllegati.DocumentInfosAdded, signatureInfo)
            End If

            'verifico se ci sono "Allegati non parte integrante (Annessi)" aggiunti
            If ProtocolEnv.EnableFlushAnnexed Then
                If uscAnnexes.DocumentsToDelete.Count > 0 Then
                    Dim annexes As ICollection(Of BiblosDocumentInfo) = ProtocolFacade.GetAnnexes(CurrentProtocol)
                    For Each idDocument As Guid In uscAnnexes.DocumentsToDelete
                        Dim annexed As BiblosDocumentInfo = annexes.FirstOrDefault(Function(x) x.DocumentId = idDocument)
                        If annexed IsNot Nothing Then
                            Service.DetachDocument(idDocument)
                            FacadeFactory.Instance.ProtocolLogFacade.Insert(CurrentProtocol, ProtocolLogEvent.PM, String.Format("Eliminato Annesso {0} con ID {1}", annexed.Name, idDocument))
                        End If
                    Next
                End If

                If Not CurrentProtocol.IdAnnexed.Equals(Guid.Empty) AndAlso uscAnnexes.DocumentInfos.Count = 0 Then
                    Service.DetachDocument(CurrentProtocol.IdAnnexed)
                    CurrentProtocol.IdAnnexed = Guid.Empty
                    FacadeFactory.Instance.ProtocolLogFacade.Insert(CurrentProtocol, ProtocolLogEvent.PM, "Catena annessi svuotata")
                End If
            End If

            If uscAnnexes.DocumentInfosAdded.Count > 0 Then
                If DocSuiteContext.Current.ProtocolEnv.IsConservationEnabled Then
                    CurrentProtocol.ConservationStatus = "M"c
                End If
                Facade.ProtocolFacade.AddAnnexes(CurrentProtocol, uscAnnexes.DocumentInfosAdded, signatureInfo)
            End If

        Catch ex As Exception
            Throw New DocSuiteException("Modifica Protocollo", "Errore in fase di salvataggio documenti", ex)
        End Try
    End Sub

#End Region

End Class

