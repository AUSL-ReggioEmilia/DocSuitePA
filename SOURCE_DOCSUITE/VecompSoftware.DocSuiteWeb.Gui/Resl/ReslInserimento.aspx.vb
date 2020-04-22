Imports System.Collections.Generic
Imports System.Linq
Imports System.Collections.ObjectModel
Imports VecompSoftware.DocSuiteWeb.DTO.Resolutions
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Resolutions
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Web
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.DocSuiteWeb.Model.Workflow
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Commons
Imports VecompSoftware.DocSuiteWeb.Model.Integrations.GenericProcesses

Partial Public Class ReslInserimento
    Inherits ReslBasePage

#Region " Fields "

    Private _currColl As Collaboration
    Private _selectedContainer As Container
    Private _currentResolution As Resolution
    Private Const PROPOSER_FIELD_DATA_NAME As String = "Proposer"
    Private Const ROLE_PROPOSER_PROPERTY_NAME As String = "ROLEPROPOSER"
    Private Const OPEN_WINDOW_SCRIPT As String = "<script language=""javascript"">function f(){OpenSelRoleProposerWindow(); Sys.Application.remove_load(f);}; Sys.Application.add_load(f);</script>"
#End Region

#Region " Properties "

    Private Property PrevSelectedIdContainer As Integer
        Get
            Dim idx As Integer = -1
            If Session("ReslInsContId") IsNot Nothing AndAlso Integer.TryParse(Session("ReslInsContId").ToString(), idx) Then
                Return idx
            End If
            Return -1
        End Get
        Set(value As Integer)
            Session("ReslInsContId") = value
        End Set
    End Property

    Private ReadOnly Property SelectedContainer() As Container
        Get
            If _selectedContainer Is Nothing Then
                If Not String.IsNullOrEmpty(idContainer.SelectedValue) Then
                    Dim temp As Integer
                    If Not Integer.TryParse(idContainer.SelectedValue, temp) Then
                        Throw New DocSuiteException("Caricamento Contenitore", String.Format("Errore in fase di conversione identificativo di contenitore: {0}", idContainer.SelectedValue))
                    End If
                    _selectedContainer = Facade.ContainerFacade.GetById(temp)
                    ' TODO: Da Implementare Controllo diritti su contenitore - disabilitare checkbox delibere o determine in base ai diritti
                End If
            End If
            Return _selectedContainer
        End Get
    End Property

    Private Property PnlAllegatiVisible() As Boolean
        Get
            Return (pnlAttach.Attributes("style") Is Nothing)
        End Get
        Set(ByVal value As Boolean)
            If value Then
                pnlAttach.Attributes.Remove("style")
            Else
                pnlAttach.Attributes.Add("style", "display:none")
            End If
        End Set
    End Property

    Private ReadOnly Property CurrentResolutionRights As ResolutionRights
        Get
            Dim tmpResolution As New Resolution
            Dim tmpResolutionType As ResolutionType = Facade.ResolutionTypeFacade.GetById(CType(rblProposta.SelectedValue, Short))

            tmpResolution.Type = tmpResolutionType
            If Not String.IsNullOrEmpty(idContainer.SelectedValue) Then
                tmpResolution.Container = SelectedContainer
            End If

            Return New ResolutionRights(tmpResolution)
        End Get
    End Property

    Private ReadOnly Property CurrentResolutionType As Short
        Get
            Dim t As Short = Short.Parse(rblProposta.SelectedValue)
            If t = 1S AndAlso chkAUSLREOCSoggetta.Checked Then
                t = 2S
            End If
            Return t
        End Get
    End Property

    Private ReadOnly Property CurrentCollaboration As Collaboration
        Get
            If _currColl Is Nothing Then
                Dim idColl As String = Request.QueryString("IdCollaboration")
                _currColl = Facade.CollaborationFacade.GetById(CType(idColl, Integer))
                If _currColl Is Nothing Then
                    Throw New DocSuiteException("Errore recupero parametro", String.Format("Impossibile recuperare la collaborazione [{0}]", idColl))
                End If
            End If

            Return _currColl
        End Get
    End Property

    Private Property CurrentResolution As Resolution
        Get
            If _currentResolution IsNot Nothing Then Return _currentResolution
            If ViewState("idResolution") IsNot Nothing Then _currentResolution = Facade.ResolutionFacade.GetById(CType(ViewState("idResolution"), Integer))
            If _currentResolution IsNot Nothing Then
                ''Salvo anche negli UserControl per non perdere il valore
                uscPrivacyPanel.CurrentResolution = _currentResolution
            End If
            Return _currentResolution
        End Get
        Set(value As Resolution)
            _currentResolution = value
            ''Salvo anche negli UserControl per non perdere il valore
            uscPrivacyPanel.CurrentResolution = value
            ''Salvo l'id in viewState
            If (value IsNot Nothing) Then ViewState("idResolution") = value.Id
        End Set
    End Property

    Private ReadOnly Property CurrentWorkflowType As String
        Get
            Dim resolutionType As ResolutionType = Facade.ResolutionTypeFacade.GetById(CType(rblProposta.SelectedValue, Short))
            Return Facade.TabMasterFacade.GetFieldValue("WorkflowType", DocSuiteContext.Current.ResolutionEnv.Configuration, resolutionType.Id)
        End Get
    End Property

    Private ReadOnly Property CurrentSelectedResolutionKind As ResolutionKind
        Get
            If String.IsNullOrEmpty(ddlResolutionKind.SelectedValue) Then
                Return Nothing
            End If

            Return CurrentResolutionKindFacade.GetById(Guid.Parse(ddlResolutionKind.SelectedValue))
        End Get
    End Property

    Private Property CurrentResolutionModel As ResolutionInsertModel
        Get
            If Session("CurrentResolutionModel") IsNot Nothing Then
                Return DirectCast(Session("CurrentResolutionModel"), ResolutionInsertModel)
            End If
            Return Nothing
        End Get
        Set(value As ResolutionInsertModel)
            If value Is Nothing Then
                Session.Remove("CurrentResolutionModel")
            Else
                Session("CurrentResolutionModel") = value
            End If
        End Set
    End Property

    'Contiene la serie documentale associata alla proposta di Atto che si sta inserendo.
    'Utilizzato con parametro ResolutionKindEnabled
    Public Property DraftSeriesItemAdded As IList(Of ResolutionSeriesDraftInsert)
        Get
            If Session("DraftSeriesItemAdded") IsNot Nothing Then
                Return DirectCast(Session("DraftSeriesItemAdded"), IList(Of ResolutionSeriesDraftInsert))
            End If
            Return Nothing
        End Get
        Set(value As IList(Of ResolutionSeriesDraftInsert))
            If value Is Nothing Then
                Session.Remove("DraftSeriesItemAdded")
            Else
                Session("DraftSeriesItemAdded") = value
            End If
        End Set
    End Property

    Public ReadOnly Property RoleProposerEnabled As Boolean
        Get
            Return Facade.ResolutionFacade.IsManagedProperty(PROPOSER_FIELD_DATA_NAME, CurrentResolutionType, ROLE_PROPOSER_PROPERTY_NAME)
        End Get
    End Property
#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

        InitializeControls()

        InitializeAjax()

        If Not Page.IsPostBack Then
            Initialize()
            InitializeDocumentControls()
        End If
        InitizializePrivacy()
    End Sub

    Protected Sub ReslInserimentoAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arguments As String() = Split(e.Argument, "|")
        Select Case arguments(0)
            Case "blockinsert"
                BlockInsert()
                AjaxManager.ResponseScripts.Add("ExecuteAjaxRequest('privacygenerate');")

            Case "privacygenerate"
                '' Genero un inserimento
                CurrentResolution = InsertResolution()

                '' genero il file dove oscurare gli omissis a mano
                uscPrivacyPanel.GeneratePrivacyDocumentToPrint(CurrentWorkflowType)

                'Mostro i dati dell'atto in inizio della pagina
                lblNumberLabel.Text = String.Format("{0}: ", Facade.ResolutionTypeFacade.GetDescription(CurrentResolutionType))
                lblNumber.Text = CurrentResolution.InclusiveNumber
                tblResolutionNumber.Visible = True

                btnInserimento.Text = "COMPLETA INSERIMENTO"

            Case "privacyreset"
                uscPrivacyPanel.ResetSelector()

            Case "checkdatatoinsert"
                'Verifico prima la presenza dei dati di base dell'atto e poi di quelli specifici per la privacy
                If CheckDataForInsert() AndAlso CheckPrivacy() Then
                    uscPrivacyPanel.CheckPrivacy()
                Else
                    uscPrivacyPanel.ResetSelector()
                End If

            Case "createNewDraftSeries"
                NewDraftSeriesAction(Convert.ToInt32(arguments(1)))

            Case "goToDraftSeries"
                BindModelFromPage()
                Dim parameters As String = String.Format("IdDocumentSeriesItem={0}&Action={1}&PreviousPage={2}&Type=Series", arguments(1), DocumentSeriesAction.View, HttpUtility.UrlEncode(Page.Request.Url.AbsoluteUri))
                Response.Redirect(String.Format("~/Series/Item.aspx?{0}", CommonShared.AppendSecurityCheck(parameters)))

            Case "draftSeriesConnect"
                If DraftSeriesItemAdded Is Nothing Then
                    DraftSeriesItemAdded = New List(Of ResolutionSeriesDraftInsert)
                End If

                If DraftSeriesItemAdded.Any(Function(x) x.IdSeriesItem.Equals(Convert.ToInt32(arguments(1)))) Then
                    AjaxAlert("L'elemento selezionato è già stato aggiunto.")
                    Exit Sub
                End If
                DraftSeriesItemAdded.Add(New ResolutionSeriesDraftInsert() With {.IdSeries = Convert.ToInt32(arguments(2)), .IdSeriesItem = Convert.ToInt32(arguments(1))})

                Dim kindSeriesSource As ICollection(Of ResolutionKindDocumentSeries) = New Collection(Of ResolutionKindDocumentSeries)
                If CurrentSelectedResolutionKind IsNot Nothing Then
                    kindSeriesSource = CurrentSelectedResolutionKind.ResolutionKindDocumentSeries
                End If
                dgvResolutionKindDocumentSeries.DataSource = kindSeriesSource
                dgvResolutionKindDocumentSeries.DataBind()

            Case "removeDraftLink"
                Dim itemToRemove As ResolutionSeriesDraftInsert = DraftSeriesItemAdded.Single(Function(x) x.IdSeriesItem.Equals(Convert.ToInt32(arguments(1))))
                DraftSeriesItemAdded.Remove(itemToRemove)

                Dim kindSeriesSource As ICollection(Of ResolutionKindDocumentSeries) = New Collection(Of ResolutionKindDocumentSeries)
                If CurrentSelectedResolutionKind IsNot Nothing Then
                    kindSeriesSource = CurrentSelectedResolutionKind.ResolutionKindDocumentSeries
                End If
                dgvResolutionKindDocumentSeries.DataSource = kindSeriesSource
                dgvResolutionKindDocumentSeries.DataBind()
                AjaxManager.ResponseScripts.Add("ResponseEnd();")
            Case "selectedRoleProposer"
                'validazione dato
                If arguments(1).Eq("root") Then
                    AjaxAlert("Selezionare almeno un settore")
                    Exit Sub
                End If

                Dim idRole As Integer = Integer.Parse(arguments(1))
                Dim role As Role = Facade.RoleFacade.GetById(idRole)
                SetRoleProposerSelected(role)

        End Select
    End Sub

    Private Sub btnInserimento_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnInserimento.Click
        If proposerGeneralValidator.Enabled Then
            Page.Validate("ProposerGeneralValidationGroup")
            If Not Page.IsValid Then
                AjaxAlert(proposerGeneralValidator.ErrorMessage)
                Exit Sub
            End If
        End If

        If ResolutionEnv.ResolutionKindEnabled Then
            If CurrentSelectedResolutionKind Is Nothing Then
                AjaxAlert("E' avvenuto un problema nel caricamento della Tipologia Atto.")
                Exit Sub
            End If

            If Not CurrentSelectedResolutionKind.ResolutionKindDocumentSeries.IsNullOrEmpty() Then
                Dim missingDraftSeries As IEnumerable(Of ResolutionKindDocumentSeries) = CurrentSelectedResolutionKind.ResolutionKindDocumentSeries.Where(Function(x) Not DraftSeriesItemAdded.Any(Function(xx) xx.IdSeries.Equals(x.DocumentSeries.Id)))
                If DraftSeriesItemAdded Is Nothing OrElse missingDraftSeries.Any() Then
                    AjaxAlert("Alcune Bozze non sono state inserite.")
                    Exit Sub
                End If
            End If
        End If

        If CurrentResolution Is Nothing Then
            If Not CheckDataForInsert() Then
                Exit Sub
            End If
            CurrentResolution = InsertResolution()
        End If

        If CurrentResolution Is Nothing Then
            AjaxAlert("Errore non previsto. Contattare l'assistenza.")
            Exit Sub
        End If

        If ResolutionEnv.ResolutionKindEnabled AndAlso DraftSeriesItemAdded IsNot Nothing Then
            For Each seriesDraftInsert As ResolutionSeriesDraftInsert In DraftSeriesItemAdded
                Dim resolutionSeries As ResolutionDocumentSeriesItem = New ResolutionDocumentSeriesItem()
                resolutionSeries.Resolution = CurrentResolution
                resolutionSeries.UniqueIdResolution = CurrentResolution.UniqueId
                resolutionSeries.IdDocumentSeriesItem = seriesDraftInsert.IdSeriesItem
                resolutionSeries.UniqueIdDocumentSeriesItem = FacadeFactory.Instance.DocumentSeriesItemFacade.GetById(seriesDraftInsert.IdSeriesItem).UniqueId
                Facade.ResolutionDocumentSeriesItemFacade.Save(resolutionSeries)
            Next
        End If

        '' Verifico se devo caricare il documento privacy
        If uscPrivacyPanel.PrivacyDocumentVisible AndAlso DocSuiteContext.Current.ResolutionEnv.UseSharepointPublication Then
            uscPrivacyPanel.SavePrivacyDocument(ResolutionFacade.DocType.PrivacyPublicationDocument)

            ''Richiamo la finalizzazione dell'inserimento per effettuare l'inserimento automatico degli step di adozione/pubblicazione/esecutività
            ProtocolFinalizeSharePointPublication(CurrentResolution)
        End If

        Dim s As String = "Type=Resl&idResolution=" & CurrentResolution.Id
        If ResolutionEnv.IsInsertPreViewEnabled Then
            s &= "&Action=Insert"
        End If
        Response.Redirect(String.Format("{0}?{1}", GetViewPageName(CurrentResolution.Id), CommonShared.AppendSecurityCheck(s)))
    End Sub

    Private Sub ProtocolFinalizeSharePointPublication(res As Resolution)
        Dim resolutionType As Integer = res.Type.Id
        Const idCatena As Integer = 0
        Const idCatenaAllegati As Integer = 0
        Const idCatenaAllegatiRiservati As Integer = 0

        Dim fileResolution As FileResolution = Facade.FileResolutionFacade.GetByResolution(res)(0)

        If resolutionType = Data.ResolutionType.IdentifierDelibera Then
            ' Mando avanti fino alla fine del Workflow
            Dim tabWorkflowPubblicazione As TabWorkflow = Facade.TabWorkflowFacade.GetByDescription("Pubblicazione", res.WorkflowType)
            Dim tabWorkflowPrecedingPubblicazione As TabWorkflow = Nothing
            Facade.TabWorkflowFacade.GetByStep(res.WorkflowType, tabWorkflowPubblicazione.Id.ResStep - 1S, tabWorkflowPrecedingPubblicazione)
            If StringHelper.InStrTest(tabWorkflowPrecedingPubblicazione.ManagedWorkflowData, "NextStep") Then
                ' Pubblicazione

                If fileResolution.IdFrontespizio Is Nothing Then
                    ''Genero e inserisco il frontalino
                    ResolutionWPFacade.InserisciFrontalinoPubblicazione(res, res.AdoptionDate.Value.AddDays(1), tabWorkflowPrecedingPubblicazione.Id.ResStep)
                End If

                Facade.ResolutionWorkflowFacade.InsertNextStep(res.Id, 2, idCatena, idCatenaAllegati, idCatenaAllegatiRiservati, Guid.Empty, Guid.Empty, Guid.Empty, DocSuiteContext.Current.User.FullUserName)
                res.PublishingDate = res.AdoptionDate.Value.AddDays(1)
                res.PublishingUser = res.AdoptionUser

                PublicateResolutionOnSharePoint(res, res.AdoptionDate.Value.AddDays(1), True)
            End If

            Dim tabWorkflowEsecutivita As TabWorkflow = Facade.TabWorkflowFacade.GetByDescription("Esecutività", res.WorkflowType)
            Dim tabWorkflowPrecedingEsecutivita As TabWorkflow = Nothing
            Facade.TabWorkflowFacade.GetByStep(res.WorkflowType, tabWorkflowEsecutivita.Id.ResStep - 1S, tabWorkflowPrecedingEsecutivita)
            If StringHelper.InStrTest(tabWorkflowPrecedingEsecutivita.ManagedWorkflowData, "NextStep") Then
                ' Esecutività
                Facade.ResolutionWorkflowFacade.InsertNextStep(res.Id, 3, idCatena, idCatenaAllegati, idCatenaAllegatiRiservati, Guid.Empty, Guid.Empty, Guid.Empty, DocSuiteContext.Current.User.FullUserName)
                res.EffectivenessDate = res.PublishingDate.Value.AddDays(15)
                res.EffectivenessUser = res.PublishingUser
            End If
            ' Salvo le modifiche
            Facade.ResolutionFacade.UpdateOnly(res)

            Facade.ResolutionFacade.SendUpdateResolutionCommand(res)

        ElseIf resolutionType = Data.ResolutionType.IdentifierDetermina Then
            'Verifico se devo salvare anche lo step successivo
            Const myStep As Short = 2
            Dim workStep As TabWorkflow = Facade.TabWorkflowFacade.GetByResolution(res.Id)
            Dim lastWorkStep As TabWorkflow = workStep
            Dim nextStepId As Short = myStep + 2

            '' La variabile serve a costringere l'ingresso nel caso ci sia almeno 1 documento
            Dim forceNext As Boolean = uscUploadDocumenti.DocumentsCount > 0
            Dim howManyNextSteps As Short = 0

            While StringHelper.InStrTest(lastWorkStep.ManagedWorkflowData, "NextStep") OrElse forceNext
                '' Disattivo la variabile per evitare un ciclo infinito
                forceNext = False

                howManyNextSteps = howManyNextSteps + 1S
                Dim workNextStep As TabWorkflow = Nothing
                ' Recupero il nuovo step
                Facade.TabWorkflowFacade.GetByStep(res.WorkflowType, nextStepId, workNextStep)

                Dim data As Date = CType(ReflectionHelper.GetProperty(res, lastWorkStep.FieldDate), Date)
                Select Case workNextStep.Description
                    Case "Pubblicazione"
                        data = data.AddDays(1)

                        If fileResolution.IdFrontespizio Is Nothing Then
                            '' Genero e inserisco il frontalino
                            ' Recupero lo step precedente a quello di pubblicazione
                            Facade.TabWorkflowFacade.GetByStep(res.WorkflowType, CType((workNextStep.Id.ResStep - 1), Short), workStep)
                            ResolutionWPFacade.InserisciFrontalinoPubblicazione(res, data, CType((workNextStep.Id.ResStep - 1), Short))
                        End If
                        PublicateResolutionOnSharePoint(res, data, True)
                    Case "Esecutività"
                        data = data.AddDays(15)
                End Select

                Facade.ResolutionFacade.SqlResolutionUpdateWorkflowData(res.Id, CurrentResolutionType, Not res.Number.HasValue, workNextStep, String.Format("{0:dd/MM/yyyy}", data), "N", -1, -1, Guid.Empty, "N", True, DocSuiteContext.Current.User.FullUserName)

                lastWorkStep = workNextStep
                nextStepId = nextStepId + 1S
            End While

            ''Inserisco lo step di Documento Adozione nel caso in cui abbia già il documento
            If uscUploadDocumenti.DocumentsCount > 0 Then
                Facade.ResolutionWorkflowFacade.InsertNextStep(res.Id, myStep, 0, 0, 0, Guid.Empty, Guid.Empty, Guid.Empty, DocSuiteContext.Current.User.FullUserName)
            End If

            For index As Short = 1 To howManyNextSteps
                Facade.ResolutionWorkflowFacade.InsertNextStep(res.Id, myStep + index, 0, 0, 0, Guid.Empty, Guid.Empty, Guid.Empty, DocSuiteContext.Current.User.FullUserName)
            Next
            Facade.ResolutionFacade.SendUpdateResolutionCommand(res)
        End If
    End Sub

    Private Sub rblProposta_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles rblProposta.SelectedIndexChanged
        ContainerDdlFiller()
        InitializeVisibleProperty()
        UpdateCategory()
        UpdateContainer()
        SelectedContainerChanged()
        InitizializePrivacy()
    End Sub

    Private Sub chbAdoption_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
        If chbAdoption.Checked Then
            pnlAdozioneData.Visible = True
            rdpDataAdozione.SelectedDate = DateTime.Now
        Else
            pnlAdozioneData.Visible = False
            rdpDataAdozione.SelectedDate = Nothing
        End If
    End Sub

    Private Sub IdContainerSelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles idContainer.SelectedIndexChanged
        SelectedContainerChanged()
    End Sub

    Private Sub UscContactRoleUpdate(ByVal sender As Object, ByVal e As EventArgs)
        Dim contactList As New List(Of ContactDTO)
        contactList.AddRange(uscDestinatari.GetContacts(False))
        uscSettori.LoadRoleContacts(contactList, False)
    End Sub

    Private Sub ChkAuslreocSoggettaCheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles chkAUSLREOCSoggetta.CheckedChanged, chkAUSLREOCNonSoggetta.CheckedChanged
        Dim check As CheckBox = DirectCast(sender, CheckBox)
        If Not check.Checked Then
            Exit Sub
        End If

        UpdateCategory()
        UpdateContainer()

        If CurrentResolutionType = 2S Then
            chkImmediatelyExecutive.Checked = False
            chkImmediatelyExecutive.Visible = False
        Else
            chkImmediatelyExecutive.Visible = True
        End If
    End Sub

    Private Sub CvControllaAuslreocServerValidate(ByVal source As Object, ByVal args As ServerValidateEventArgs) Handles cvControllaAUSLREOC.ServerValidate
        args.IsValid = chkAUSLREOCNonSoggetta.Checked OrElse chkAUSLREOCSoggetta.Checked
    End Sub

    Private Sub UscUploadDocumentiDocumentRemoved(sender As Object, e As DocumentEventArgs) Handles uscUploadDocumenti.DocumentRemoved
        ''Se rimuovo un documento e ho attiva la pubblicazione sharepoint di ASMN e l'atto non è una delibera e non ho documenti, allora tolgo il pannello
        If ResolutionEnv.UseSharepointPublication Then
            uscPrivacyPanel.PrivacyTypeVisible = Not (rblProposta.SelectedValue <> "1" AndAlso uscUploadDocumenti.DocumentsCount = 0)
            uscPrivacyPanel.ValidatorEnabled = uscPrivacyPanel.PrivacyTypeVisible
        End If
    End Sub

    Private Sub UscUploadDocumentiDocumentUploaded(sender As Object, e As DocumentEventArgs) Handles uscUploadDocumenti.DocumentUploaded
        '' Se l'atto è di tipo disposizione e sto aggiungendo documenti, allora devo mostrare e rendere obbligatoria la scelta della privacy
        If ResolutionEnv.UseSharepointPublication Then
            uscPrivacyPanel.PrivacyTypeVisible = uscUploadDocumenti.DocumentsCount > 0
            uscPrivacyPanel.ValidatorEnabled = uscPrivacyPanel.PrivacyTypeVisible
        End If
    End Sub

    Protected Sub DdlResolutionKind_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlResolutionKind.SelectedIndexChanged
        If CurrentResolutionModel Is Nothing OrElse (CurrentResolutionModel.ResolutionKind.HasValue AndAlso Not CurrentResolutionModel.ResolutionKind.Equals(Guid.Parse(ddlResolutionKind.SelectedValue))) Then
            DraftSeriesItemAdded = Nothing
        End If

        pnlAmmTrasp.Visible = False
        If CurrentSelectedResolutionKind IsNot Nothing AndAlso Not CurrentSelectedResolutionKind.ResolutionKindDocumentSeries.IsNullOrEmpty() Then
            pnlAmmTrasp.Visible = True
            dgvResolutionKindDocumentSeries.DataSource = CurrentSelectedResolutionKind.ResolutionKindDocumentSeries
            dgvResolutionKindDocumentSeries.DataBind()
        End If
    End Sub

    Protected Sub DgvResolutionKindDocumentSeries_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles dgvResolutionKindDocumentSeries.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim entity As ResolutionKindDocumentSeries = DirectCast(e.Item.DataItem, ResolutionKindDocumentSeries)
        DirectCast(e.Item.FindControl("lblDocumentSeriesName"), Label).Text = entity.DocumentSeries.Name

        Dim btnDraft As Button = DirectCast(e.Item.FindControl("btnDocumentSeriesDraft"), Button)
        Dim btnConnect As Button = DirectCast(e.Item.FindControl("btnDocumentSeriesConnect"), Button)
        Dim seriesLink As HyperLink = DirectCast(e.Item.FindControl("documentSeriesLink"), HyperLink)
        Dim btnRemoveLink As ImageButton = DirectCast(e.Item.FindControl("btnRemoveLink"), ImageButton)

        btnDraft.Visible = False
        btnConnect.Visible = False
        seriesLink.Visible = False
        btnRemoveLink.Visible = False
        If DraftSeriesItemAdded IsNot Nothing AndAlso DraftSeriesItemAdded.Any(Function(x) x.IdSeries.Equals(entity.DocumentSeries.Id)) Then
            seriesLink.Visible = True
            btnRemoveLink.Visible = True
            Dim idSeriesItem As Integer = DraftSeriesItemAdded.Single(Function(x) x.IdSeries.Equals(entity.DocumentSeries.Id)).IdSeriesItem
            Dim seriesItem As DocumentSeriesItem = Facade.DocumentSeriesItemFacade.GetById(idSeriesItem)
            seriesLink.Text = String.Format("N. {0} del {1}", seriesItem.Id, seriesItem.RegistrationDate.ToLocalTime().DateTime.ToString("dd/MM/yyyy"))
            seriesLink.Attributes.Add("onclick", String.Format("GoToDraftSeries('{0}')", seriesItem.Id))

            btnRemoveLink.OnClientClick = String.Format("return RemoveDraftLink('{0}');", seriesItem.Id)
        Else
            btnDraft.Visible = True
            btnDraft.OnClientClick = String.Format("return CreateNewDraftSeries('{0}');", entity.DocumentSeries.Id)

            btnConnect.Visible = True
            btnConnect.OnClientClick = String.Format("return {1}_OpenDraftSeriesConnectWindow('{0}');", entity.DocumentSeries.Id, ID)
        End If
    End Sub

    Protected Sub GeneralProposerValidation(sender As Object, e As ServerValidateEventArgs) Handles proposerGeneralValidator.ServerValidate
        'Verifica che i valori non siano vuoti
        e.IsValid = SelProponente.GetContacts(False).Any() OrElse Not String.IsNullOrEmpty(SelProponenteAlt.GetContactText())
    End Sub

    Private Sub UscProposerRoleUpdate(ByVal sender As Object, ByVal e As EventArgs)
        uscSettori.LoadRoleContacts(SelProponente.GetContacts(False), False)
    End Sub
#End Region

#Region " Methods "

    Private Function CheckDataForInsert() As Boolean
        'verifica collaborazione se già protocollato
        If Action.Eq("FromCollaboration") OrElse (CurrentResolutionModel IsNot Nothing AndAlso CurrentResolutionModel.IdCollaboration.HasValue) Then
            If (CurrentResolutionModel IsNot Nothing AndAlso CurrentResolutionModel.IdCollaboration.HasValue) Then
                _currColl = FacadeFactory.Instance.CollaborationFacade.GetById(CurrentResolutionModel.IdCollaboration.Value)
            End If

            If Not String.IsNullOrEmpty(CurrentCollaboration.ProtocolString) Then
                AjaxAlert("Il documento della Collaborazione è già stato Protocollato.{0}Numero di Protocollo {1}.", Environment.NewLine, CurrentCollaboration.ProtocolString)
                Return False
            End If

            If CurrentCollaboration.Resolution IsNot Nothing Then
                AjaxAlert("Il documento della Collaborazione è già presente in un Atto.{0}Numero di Atto {1}.", Environment.NewLine, CurrentCollaboration.Resolution.Id)
                Return False
            End If
        End If

        'Verifica oggetto
        If SelOggetto.Text.Length > SelOggetto.MaxLength Then
            AjaxAlert("Impossibile salvare.{0}Il campo Oggetto ha superato i caratteri disponibili.{0}(Caratteri {1} Disponibili {2})", Environment.NewLine, SelOggetto.Text.Length, SelOggetto.MaxLength)
            Return False
        End If
        If ResolutionEnv.ObjectMinLength <> 1 Then
            If SelOggetto.Text.Length < ResolutionEnv.ObjectMinLength Then
                AjaxAlert("Impossibile salvare.{0}Il campo Oggetto non ha raggiunto i caratteri minimi ({1} caratteri).", Environment.NewLine, ResolutionEnv.ObjectMinLength)
                Return False
            End If
        End If
        Dim PartialObject As String = StringHelper.Clean(SelOggetto.Text, DocSuiteContext.Current.ProtocolEnv.RegexOnPasteString)
        If Not (String.Equals(SelOggetto.Text, PartialObject)) Then
            SelOggetto.Text = PartialObject
            AjaxAlert("L'oggetto è stato ripulito in automatico dai caratteri speciali. Verificare che l'oggetto risultante sia valido")
            Return False
        End If

        'Verifica documento
        If Not CommonInstance.BiblosExistFile(uscUploadDocumenti.DocumentInfosAdded) Then
            uscUploadDocumenti.ClearNodes()
            AjaxAlert("Il Documento sul Server non è valido.{0}Reinserire il Documento", Environment.NewLine)
            Return False
        End If

        Select Case ResolutionEnv.Configuration
            Case ConfTo
                'Verifica Note
                If Len(txtNote.Text) > txtNote.MaxLength Then
                    AjaxAlert("Impossibile salvare.{0}Il campo Note ha superato i caratteri disponibili.", Environment.NewLine)
                    Return False
                End If

            Case Else
                'Verifica(allegati)
                If Not CommonInstance.BiblosExistFile(uscUploadAttach.DocumentInfosAdded) Then
                    uscUploadAttach.ClearNodes()
                    AjaxAlert("Il Documento Allegato sul Server non è valido.{0}Reinserire il Documento Allegato", Environment.NewLine)
                    Return False
                End If

        End Select

        Return True
    End Function

    Private Function CheckPrivacy() As Boolean
        Try
            '' Eccezioni al corretto funzionamento della generazione automatica del documento da scansionare
            If uscUploadDocumenti.DocumentInfos.Count = 0 Then
                Throw New ArgumentException("E' necessario inserire un documento nell'atto.")
            End If
            If String.IsNullOrEmpty(idContainer.SelectedValue) Then
                Throw New ArgumentException("E' necessario selezionare un contenitore per l'atto")
            End If
            If Not rdpDataAdozione.SelectedDate.HasValue Then
                Throw New ArgumentException("E' necessario inserire la data di adozione dell'atto")
            End If
            If pnlAdozioneNumero.Enabled AndAlso String.IsNullOrEmpty(txtServiceNumber.Text) Then
                Throw New ArgumentException("E' necessario inserire il numero di adozione dell'atto")
            End If
            If String.IsNullOrEmpty(SelOggetto.Text) Then
                Throw New ArgumentException("E' necessario specificare l'oggetto dell'atto")
            End If
        Catch ex As Exception
            uscPrivacyPanel.ResetSelector()
            AjaxAlert("Impossibile attivare il pannello Privacy.{0}{1}", Environment.NewLine, ex.Message)
        End Try
        Return True
    End Function

    Private Function InsertResolution() As Resolution
        PrevSelectedIdContainer = Integer.Parse(idContainer.SelectedValue)
        'Tipologia
        Dim resolutionTypeId As Short = Short.Parse(rblProposta.SelectedValue)

        '--Numero progressivo
        IdResolution = Facade.ParameterFacade.GetIdresolution()
        If IdResolution = -1 Then
            Throw New DocSuiteException("Inserimento Resolution", "Il Server non ha assegnato correttamente il numero di Documento progressivo.")
        End If

        '--Registrazione
        Dim res As Resolution = New Resolution
        res.Id = IdResolution
        res.Status.Id = ResolutionStatusId.Temporaneo
        res.ProposeDate = DateTime.Today
        res.ProposeUser = DocSuiteContext.Current.User.FullUserName
        res.Type.Id = resolutionTypeId
        'Container
        res.Container = SelectedContainer
        res.Location = SelectedContainer.ReslLocation

        'Comunication
        'Proponente
        If Not RoleProposerEnabled Then
            For Each contact As ContactDTO In SelProponente.GetContacts(False)
                res.AddProposer(contact.Contact)
            Next
            'Proponente Alternativo
            If Not String.IsNullOrEmpty(SelProponenteAlt.GetContactText()) Then
                res.AlternativeProposer = SelProponenteAlt.GetContactText()
            End If
        Else
            Dim roles As IList(Of Role) = uscRoleProposer.GetRoles()
            If roles.Any() Then
                res.AlternativeProposer = roles.First().Name
                res.RoleProposer = roles.First()
            End If
        End If

        'Responsabile
        For Each contact As ContactDTO In SelResponsabile.GetContacts(False)
            res.AddManager(contact.Contact)
        Next
        'Responsabile Alternativo
        If Not String.IsNullOrEmpty(SelResponsabileAlt.GetContactText()) Then
            res.AlternativeManager = SelResponsabileAlt.GetContactText()
        End If

        'Assegnatario
        For Each contact As ContactDTO In SelAssegnatario.GetContacts(False)
            res.AddAssignee(contact.Contact)
        Next
        'Assegnatario Alternativo
        If Not String.IsNullOrEmpty(SelAssegnatarioAlt.GetContactText()) Then
            res.AlternativeAssignee = SelAssegnatarioAlt.GetContactText()
        End If

        'Destinatari
        ' Aggiungo i Destinatari all'Atto e verifico se questi hanno un Settore associato da Autorizzare
        'Dim roles As New List(Of Role)
        For Each contact As ContactDTO In uscDestinatari.GetContacts(False)
            res.AddRecipient(contact.Contact)
            ''AG 20100421: magari fare una verifichina per evitare problemi di chiave duplicata... avrei risparmiato una mezza giornata in ASMN... argh!
            'If Not contact.Contact.Role Is Nothing AndAlso Not roles.Contains(contact.Contact.Role) Then
            '    roles.Add(contact.Contact.Role)
            'End If
        Next
        'Destinatario Alternativo
        If Not String.IsNullOrEmpty(uscDestinatariAlt.GetContactText()) Then
            res.AlternativeRecipient = uscDestinatariAlt.GetContactText()
        End If

        ' Oggetto e note
        If Not String.IsNullOrEmpty(SelOggetto.Text) Then
            res.ResolutionObject = StringHelper.Clean(SelOggetto.Text, DocSuiteContext.Current.ProtocolEnv.RegexOnPasteString)
        End If
        If Not String.IsNullOrEmpty(txtNote.Text) Then
            res.Note = txtNote.Text
        End If

        'Tipologia Atto
        If ResolutionEnv.ResolutionKindEnabled Then
            res.ResolutionKind = CurrentSelectedResolutionKind
        Else
            res.ResolutionKind = CurrentResolutionKindFacade.GetAll().First()
        End If

        'classificatore
        If pnlCategory.Visible AndAlso uscSelCategory.HasSelectedCategories Then
            Dim selectedCategory As Category = uscSelCategory.SelectedCategories.First()
            Dim root As Category = selectedCategory.Root
            If selectedCategory.Equals(root) Then
                res.Category = selectedCategory
            Else
                res.Category = root
                res.SubCategory = selectedCategory
            End If
        End If

        ' Se la configurazione è AUSL devo verificare lo stato dei Check per decidere quale WorkFlow devono seguire
        If pnlAUSLREOC.Visible AndAlso rblProposta.SelectedValue = "1" Then
            If chkAUSLREOCNonSoggetta.Checked Then
                res.IsChecked = False
            End If
            If chkAUSLREOCSoggetta.Checked Then
                res.IsChecked = True
                ' Imposto il tipo atto a 2 in modo che prenda il record corretto di WORKFLOW
                resolutionTypeId = 2
            End If
        End If

        res.WorkflowType = Facade.TabMasterFacade.GetFieldValue("WorkflowType", DocSuiteContext.Current.ResolutionEnv.Configuration, resolutionTypeId)

        ' FileResolution
        Dim fr As New FileResolution
        fr.Id = IdResolution
        fr.Resolution = res
        res.File = fr
        fr.UniqueIdResolution = res.UniqueId

        'Configurazione ASL-TO2
        If pnlImmediatelyExecutive.Visible Then
            res.ImmediatelyExecutive = chkImmediatelyExecutive.Checked
        End If
        If pnlObjectPrivacy.Visible Then
            res.ResolutionObjectPrivacy = StringHelper.Clean(SelOggettoPrivacy.Text, DocSuiteContext.Current.ProtocolEnv.RegexOnPasteString)
        End If

        If pnlOC.Visible Then
            res.OCSupervisoryBoard = chkOCSupervisoryBoard.Checked
            res.OCRegion = chkOCRegion.Checked
            res.OCCorteConti = chkOCCorteConti.Checked
            res.OCOther = chkOCOther.Checked
            If chkOCOther.Checked Then
                res.OtherDescription = txtOCOtherDescription.Text
            End If
        End If

        'EF 20120119 Workaround per non perdere il check senza creare una nuova colonna lato DB. Da verificare e adattare
        If ResolutionEnv.Configuration.Eq(ConfAuslPc) Then
            res.OCManagement = chkOCConfSindaci.Checked
        End If

        If (Action.Eq("FromCollaboration") OrElse (CurrentResolutionModel IsNot Nothing AndAlso CurrentResolutionModel.IdCollaboration.HasValue)) AndAlso ProtocolEnv.SecureDocumentEnabled Then
            Facade.CollaborationFacade.FinalizeSecureDocument(CurrentCollaboration)
        End If

        Try
            Facade.ResolutionFacade.Save(res)
        Catch ex As Exception
            Facade.ResolutionLogFacade.Log(res, ResolutionLogType.RE, ex.Message)
            FileLogger.Error(LoggerName, "Errore in inserimento Resolution", ex)
            AjaxAlert("Errore in fase di inserimento, contattare l'assistenza.")
            Return Nothing
        End Try

        ' Inserimento autorizzazioni 
        For Each r As Role In uscSettori.GetRoles
            Facade.ResolutionRoleFacade.AddRole(res, r.Id, ResolutionEnv.AuthorizInsertType)
        Next

        If Not String.IsNullOrEmpty(ResolutionEnv.CollResolutionRole) Then
            For Each r As Role In collaborationAuthorizedRoles.GetRoles
                For Each idResolutionRoleType As String In ResolutionEnv.CollResolutionRole.Split({"|,"}, StringSplitOptions.RemoveEmptyEntries)
                    Facade.ResolutionRoleFacade.AddRole(res, r.Id, CType(idResolutionRoleType, Integer))
                Next
            Next
        End If

        ''--biblos
        'Inserimento documento, allegato in biblos
        Dim year As Short = 0
        Dim number As Integer = 0
        Dim signature As String = "*"
        Dim idCatena As Integer = 0
        Dim idCatenaNome As String = String.Empty
        Dim idCatenaDocumentiOmissis As Guid = Guid.Empty
        Dim idCatenaAllegati As Integer = 0
        Dim idCatenaAllegatiOmissis As Guid = Guid.Empty
        Dim idCatenaAllegatiRiservati As Integer = 0
        Dim idCatenaAnnessi As Guid = Guid.Empty

        Try
            Dim dematerialisationRequestModel As DocumentManagementRequestModel = New DocumentManagementRequestModel()

            If uscUploadDocumenti.DocumentInfos.Count > 0 Then
                ' Segnatura su adozione automatica
                If ResolutionEnv.IsInsertAdoption Then
                    Dim param As Parameter = Facade.ParameterFacade.GetCurrentAndRefresh()
                    year = param.LastUsedResolutionYear
                    If resolutionTypeId = ResolutionType.IdentifierDelibera Then
                        param.LastUsedResolutionNumber += 1S
                        number = param.LastUsedResolutionNumber
                    Else
                        number = param.LastUsedBillNumber
                        param.LastUsedBillNumber += 1S
                    End If
                    Facade.ParameterFacade.Update(param)
                    signature = Facade.ResolutionTypeFacade.GetDescription(res.Type)
                    signature &= String.Format(" {0}/{1:0000000} del {2}", year, number, rdpDataAdozione.SelectedDate.DefaultString)

                End If

                Dim documentBiblosInfo As List(Of BiblosDocumentInfo) = ResolutionFacade.SaveBiblosDocuments(res, uscUploadDocumenti.DocumentInfosAdded, signature, Guid.Empty, idCatena)
                If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                    ResolutionInsertedDocumentPrivacyLevel(res, documentBiblosInfo)
                End If

                If DocSuiteContext.Current.ProtocolEnv.DematerialisationEnabled AndAlso uscUploadDocumenti.DocumentInfosDematerialisationAdded IsNot Nothing AndAlso uscUploadDocumenti.DocumentInfosDematerialisationAdded.Count > 0 _
                    AndAlso Not String.IsNullOrEmpty(uscUploadDocumenti.DocumentInfosAdded(0).Name) AndAlso documentBiblosInfo IsNot Nothing AndAlso documentBiblosInfo.FirstOrDefault() IsNot Nothing _
                   AndAlso Not documentBiblosInfo.FirstOrDefault().DocumentId.IsEmpty() AndAlso Not documentBiblosInfo.FirstOrDefault().ChainId.IsEmpty() Then

                    Dim workflowReferenceBiblosModel As WorkflowReferenceBiblosModel = New WorkflowReferenceBiblosModel()
                    workflowReferenceBiblosModel.DocumentName = uscUploadDocumenti.DocumentInfosAdded(0).Name
                    workflowReferenceBiblosModel.ChainType = Model.Entities.DocumentUnits.ChainType.MainChain
                    workflowReferenceBiblosModel.ArchiveChainId = documentBiblosInfo.FirstOrDefault().ChainId
                    workflowReferenceBiblosModel.ArchiveDocumentId = documentBiblosInfo.FirstOrDefault().DocumentId
                    dematerialisationRequestModel.Documents.Add(workflowReferenceBiblosModel)
                End If

                Select Case True
                    Case Facade.ResolutionFacade.IsManagedProperty("idProposalFile", resolutionTypeId)
                        Facade.ResolutionFacade.SqlResolutionDocumentUpdate(IdResolution, idCatena, ResolutionFacade.DocType.Proposta)
                    Case Facade.ResolutionFacade.IsManagedProperty("idResolutionFile", resolutionTypeId)
                        Facade.ResolutionFacade.SqlResolutionDocumentUpdate(IdResolution, idCatena, ResolutionFacade.DocType.Disposizione)
                End Select

                If uscUploadDocumenti.DocumentInfos(0).IsSigned Then
                    Facade.ResolutionFacade.SqlResolutionDocumentUpdate(IdResolution, idCatena, ResolutionFacade.DocType.Adozione)
                End If
            End If

            ' Documento Nome
            If uscUploadDocumenti.DocumentInfos.Count = 1 Then
                idCatenaNome = uscUploadDocumenti.DocumentInfos.Select(Function(o) o.DownloadFileName).FirstOrDefault()
            End If


            ' Documenti Omissis
            If uscUploadDocumentiOmissis.DocumentInfos.Count > 0 Then
                Dim docsBiblos As List(Of BiblosDocumentInfo) = ResolutionFacade.SaveBiblosDocuments(res, uscUploadDocumentiOmissis.DocumentInfosAdded, signature, idCatenaDocumentiOmissis)
                If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                    ResolutionInsertedDocumentPrivacyLevel(res, docsBiblos, ResolutionFacade.DocType.DocumentoPrincipaleOmissis.ToString())
                End If
                If ResolutionEnv.MainDocumentOmissisEnable Then
                    Facade.ResolutionFacade.SqlResolutionDocumentUpdate(IdResolution, idCatenaDocumentiOmissis, ResolutionFacade.DocType.DocumentoPrincipaleOmissis)
                End If
            End If

            ' Allegati
            If uscUploadAttach.DocumentInfos.Count > 0 Then
                Dim attachmentsBiblosInfo As List(Of BiblosDocumentInfo) = ResolutionFacade.SaveBiblosDocuments(res, uscUploadAttach.DocumentInfosAdded, signature, Guid.Empty, idCatenaAllegati)

                If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                    ResolutionInsertedDocumentPrivacyLevel(res, attachmentsBiblosInfo, ResolutionFacade.DocType.Allegati.ToString())
                End If

                If DocSuiteContext.Current.ProtocolEnv.DematerialisationEnabled AndAlso uscUploadAttach.DocumentInfosDematerialisationAdded IsNot Nothing AndAlso uscUploadAttach.DocumentInfosDematerialisationAdded.Count > 0 Then

                    Dim attachmentWorkflowReferenceBiblosModel As WorkflowReferenceBiblosModel
                    Dim attachmentId As Guid
                    Dim attachmentChainId As Guid

                    For Each attachment As DocumentInfo In uscUploadAttach.DocumentInfosDematerialisationAdded
                        attachment = uscUploadAttach.DocumentInfosAdded.Where(Function(x) x.Serialized = attachment.Serialized).SingleOrDefault()

                        attachmentWorkflowReferenceBiblosModel = New WorkflowReferenceBiblosModel()
                        attachmentId = Nothing
                        attachmentChainId = Nothing

                        If attachment IsNot Nothing AndAlso Not String.IsNullOrEmpty(attachment.Name) AndAlso attachment.Attributes.Keys.Contains("documentId") AndAlso Guid.TryParse(attachment.Attributes("documentId"), attachmentId) AndAlso Not attachmentId = Guid.Empty _
                            AndAlso attachment.Attributes.Keys.Contains("chainId") AndAlso Guid.TryParse(attachment.Attributes("chainId"), attachmentChainId) AndAlso attachment.Attributes.Keys.Contains("archiveName") AndAlso Not String.IsNullOrEmpty(attachment.Attributes("archiveName")) AndAlso Not attachmentChainId = Guid.Empty Then
                            attachmentWorkflowReferenceBiblosModel.DocumentName = attachment.Name
                            attachmentWorkflowReferenceBiblosModel.ChainType = Model.Entities.DocumentUnits.ChainType.AttachmentsChain
                            attachmentWorkflowReferenceBiblosModel.ArchiveChainId = attachmentChainId
                            attachmentWorkflowReferenceBiblosModel.ArchiveDocumentId = attachmentId
                            attachmentWorkflowReferenceBiblosModel.ArchiveName = attachment.Attributes("archiveName")
                            dematerialisationRequestModel.Documents.Add(attachmentWorkflowReferenceBiblosModel)
                        End If
                    Next
                End If
                If PnlAllegatiVisible Then
                    Facade.ResolutionFacade.SqlResolutionDocumentUpdate(IdResolution, idCatenaAllegati, ResolutionFacade.DocType.Allegati)
                End If
            End If

            'Log di Serializzazione comando 'SB' per attestazione conformità
            If DocSuiteContext.Current.ProtocolEnv.DematerialisationEnabled AndAlso dematerialisationRequestModel.Documents IsNot Nothing AndAlso dematerialisationRequestModel.Documents.Count > 0 Then
                Dim documentUnit As New WorkflowReferenceModel()
                documentUnit.ReferenceId = res.UniqueId
                documentUnit.ReferenceType = DSWEnvironmentType.Resolution
                dematerialisationRequestModel.DocumentUnit = documentUnit
                dematerialisationRequestModel.RegistrationUser = DocSuiteContext.Current.User.FullUserName
                Facade.ResolutionLogFacade.Log(res, ResolutionLogType.SB, JsonConvert.SerializeObject(dematerialisationRequestModel))
            End If

            ' Allegati Omissis
            If uscUploadAttachOmissis.DocumentInfos.Count > 0 Then
                Dim docsBiblos As List(Of BiblosDocumentInfo) = ResolutionFacade.SaveBiblosDocuments(res, uscUploadAttachOmissis.DocumentInfosAdded, signature, idCatenaAllegatiOmissis)
                If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                    ResolutionInsertedDocumentPrivacyLevel(res, docsBiblos, ResolutionFacade.DocType.AllegatiOmissis.ToString())
                End If
                If ResolutionEnv.AttachmentOmissisEnable Then
                    Facade.ResolutionFacade.SqlResolutionDocumentUpdate(IdResolution, idCatenaAllegatiOmissis, ResolutionFacade.DocType.AllegatiOmissis)
                End If
            End If

            ' Allegati Privacy
            If uscUploadPrivacyAttachment.DocumentInfos.Count > 0 Then
                Dim docsBiblos As List(Of BiblosDocumentInfo) = ResolutionFacade.SaveBiblosDocuments(res, uscUploadPrivacyAttachment.DocumentInfosAdded, signature, Guid.Empty, idCatenaAllegatiRiservati)
                If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                    ResolutionInsertedDocumentPrivacyLevel(res, docsBiblos, ResolutionFacade.DocType.AllegatiRiservati.ToString())
                End If
                If pnlPrivacyAttachment.Visible Then
                    Facade.ResolutionFacade.SqlResolutionDocumentUpdate(IdResolution, idCatenaAllegatiRiservati, ResolutionFacade.DocType.AllegatiRiservati)
                End If
            End If

            ' Annessi
            If uscUploadAnnexed.DocumentInfos.Count > 0 Then
                Dim docsBiblos As List(Of BiblosDocumentInfo) = ResolutionFacade.SaveBiblosDocuments(res, uscUploadAnnexed.DocumentInfosAdded, signature, idCatenaAnnessi)
                If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                    ResolutionInsertedDocumentPrivacyLevel(res, docsBiblos, ResolutionFacade.DocType.Annessi.ToString())
                End If
                Facade.ResolutionFacade.SqlResolutionDocumentUpdate(IdResolution, idCatenaAnnessi, ResolutionFacade.DocType.Annessi)
            End If

        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore in inserimento atto", ex)
            AjaxAlert("Errore in inserimento atto")
            Return Nothing
        End Try

        'status
        res.Status = Facade.ResolutionStatusFacade.GetById(ResolutionStatusId.Attivo)
        Facade.ResolutionFacade.UpdateOnly(res)

        'Workflow
        Facade.ResolutionWorkflowFacade.InsertNextStep(IdResolution, 0, idCatena, idCatenaAllegati, idCatenaAllegatiRiservati, idCatenaAnnessi, idCatenaDocumentiOmissis, idCatenaAllegatiOmissis, DocSuiteContext.Current.User.FullUserName, idCatenaNome)

        ' adozione con numerazione automatica
        If ResolutionEnv.IsInsertAdoption AndAlso rdpDataAdozione.SelectedDate.HasValue Then
            If number <= 0 Then
                Dim param As Parameter = Facade.ParameterFacade.GetCurrentAndRefresh()
                year = param.LastUsedResolutionYear
                If resolutionTypeId = ResolutionType.IdentifierDelibera Then
                    param.LastUsedResolutionNumber = param.LastUsedResolutionNumber + 1S
                    number = param.LastUsedResolutionNumber
                Else
                    number = param.LastUsedBillNumber
                    param.LastUsedBillNumber = param.LastUsedBillNumber + 1S
                End If
                Facade.ParameterFacade.Update(param)
            End If
            res.Year = year
            res.Number = number
            res.AdoptionUser = DocSuiteContext.Current.User.FullUserName
            res.AdoptionDate = rdpDataAdozione.SelectedDate.Value
            If resolutionTypeId = ResolutionType.IdentifierDelibera Then
                res.ProposeDate = res.AdoptionDate
            End If

            Facade.ResolutionFacade.Update(res)

            ' Aggiungo lo step di Adozione
            Facade.ResolutionWorkflowFacade.InsertNextStep(IdResolution, 1, idCatena, idCatenaAllegati, idCatenaAllegatiRiservati, idCatenaAnnessi, idCatenaDocumentiOmissis, idCatenaAllegatiOmissis, DocSuiteContext.Current.User.FullUserName)

            'Invio comando di creazione Resolution alle WebApi
            Facade.ResolutionFacade.SendCreateResolutionCommand(CurrentResolution)

        End If

        'Collaborazione 
        If CurrentResolutionModel IsNot Nothing AndAlso CurrentResolutionModel.IdCollaboration.HasValue Then
            _currColl = FacadeFactory.Instance.CollaborationFacade.GetById(CurrentResolutionModel.IdCollaboration.Value)
        End If

        If Action.Eq("FromCollaboration") OrElse (CurrentResolutionModel IsNot Nothing AndAlso CurrentResolutionModel.IdCollaboration.HasValue) Then
            Try
                Facade.CollaborationFacade.Update(CurrentCollaboration, "", Nothing, "", Nothing, CollaborationStatusType.PT, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, res.Id, "", 0, False)
                Facade.CollaborationFacade.SendMail(CurrentCollaboration, CollaborationMainAction.ProtocollatiGestiti)
            Catch ex As Exception
                Facade.ResolutionLogFacade.Log(res, ResolutionLogType.RE, ex.Message)
                FileLogger.Warn(LoggerName, "Problema inserimento da collaborazione", ex)
                AjaxAlert("Errore in Aggiornamento dati registrazione della Collaborazione: " & ex.Message)
            End Try
        End If

        'Inserimento Log
        Facade.ResolutionLogFacade.Log(res, ResolutionLogType.RI, "")
        Return res
    End Function

    Private Sub ClearIdResolution()
        ViewState("IdResolution") = Nothing
    End Sub

    Private Sub Duplicate()
        Dim check As String = Request.QueryString("Check")
        Dim resolution As Resolution = Facade.ResolutionFacade.Duplicate(IdResolution,
            GetCheck(check, ResolutionDuplicaOption.Type),
            GetCheck(check, ResolutionDuplicaOption.Recipients),
            GetCheck(check, ResolutionDuplicaOption.Proposer),
            GetCheck(check, ResolutionDuplicaOption.Object),
            GetCheck(check, ResolutionDuplicaOption.Note),
            GetCheck(check, ResolutionDuplicaOption.Manager),
            GetCheck(check, ResolutionDuplicaOption.Container),
            GetCheck(check, ResolutionDuplicaOption.Category),
            GetCheck(check, ResolutionDuplicaOption.Assignee))

        ' Ho terminato di prepopolare i dati dalla resolution di origine,
        ' predispongo la maschera per un nuovo inserimento.
        ClearIdResolution()

        'Type
        rblProposta.SelectedValue = CType(resolution.Type.Id, String)
        rblProposta_SelectedIndexChanged(rblProposta, New EventArgs())
        'Recipients
        BindContacts(resolution.ResolutionContactsRecipients, uscDestinatari)
        'Alternative Recipients
        uscDestinatariAlt.DataSource = resolution.AlternativeRecipient
        'Proposer
        If Not RoleProposerEnabled Then
            BindContacts(resolution.ResolutionContactProposers, SelProponente)
        Else
            If resolution.RoleProposer IsNot Nothing Then
                uscRoleProposer.SourceRoles = New List(Of Role) From {resolution.RoleProposer}
                uscRoleProposer.DataBind()
            End If
        End If
        'Alternative Proposer
        SelProponenteAlt.DataSource = resolution.AlternativeProposer
        'Manager
        BindContacts(resolution.ResolutionContactsManagers, SelResponsabile)
        'Alternative Managers
        SelResponsabileAlt.DataSource = resolution.AlternativeManager
        'Assignee
        BindContacts(resolution.ResolutionContactsAssignees, SelAssegnatario)
        'Alternative Assignees
        SelAssegnatarioAlt.DataSource = resolution.AlternativeAssignee
        'Object
        SelOggetto.Text = resolution.ResolutionObject
        'Note
        txtNote.Text = resolution.Note
        'Container
        If resolution.Container IsNot Nothing Then
            idContainer.SelectedValue = CType(resolution.Container.Id, String)
        End If
        'classificatore

        Dim categoryToDuplicate As Category = If(resolution.SubCategory IsNot Nothing, resolution.SubCategory, resolution.Category)
        If Facade.CategoryFacade.IsCategoryActive(categoryToDuplicate) Then
            uscSelCategory.DataSource.Add(categoryToDuplicate)
        End If
        uscSelCategory.DataBind()
    End Sub

    Protected Sub BindContacts(ByRef resolutionContacts As IList(Of ResolutionContact), ByRef uscContact As uscContattiSel)
        For Each reslContact As ResolutionContact In resolutionContacts
            Dim contactDto As New ContactDTO()
            contactDto.Contact = reslContact.Contact
            contactDto.Type = ContactDTO.ContactType.Address
            uscContact.DataSource.Add(contactDto)
        Next
        uscContact.DataBind()
    End Sub

    Private Shared Function GetCheck(ByVal field As String, ByVal right As Integer) As Boolean
        Dim b As Boolean
        Select Case Mid$(field, right, 1)
            Case "1"
                b = True
            Case Else
                b = False
        End Select
        Return b
    End Function

    Private Sub DisableComponentFromCollaboration()
        uscUploadDocumenti.ButtonFileEnabled = False
        uscUploadDocumenti.ButtonLibrarySharepointEnabled = False
        uscUploadDocumenti.ButtonScannerEnabled = False
        uscUploadDocumenti.SignButtonEnabled = False
        uscUploadDocumenti.ButtonRemoveEnabled = False

        'disabilita controllo upload Documenti Omissis
        uscUploadDocumentiOmissis.ButtonFileEnabled = False
        uscUploadDocumentiOmissis.ButtonLibrarySharepointEnabled = False
        uscUploadDocumentiOmissis.ButtonScannerEnabled = False
        uscUploadDocumentiOmissis.SignButtonEnabled = False
        uscUploadDocumentiOmissis.ButtonRemoveEnabled = False

        'disabilita controllo upload Allegati
        uscUploadAttach.ButtonFileEnabled = False
        uscUploadAttach.ButtonLibrarySharepointEnabled = False
        uscUploadAttach.ButtonScannerEnabled = False
        uscUploadAttach.SignButtonEnabled = False
        uscUploadAttach.ButtonRemoveEnabled = False

        'disabilita controllo upload Allegati Omissis
        uscUploadAttachOmissis.ButtonFileEnabled = False
        uscUploadAttachOmissis.ButtonLibrarySharepointEnabled = False
        uscUploadAttachOmissis.ButtonScannerEnabled = False
        uscUploadAttachOmissis.SignButtonEnabled = False
        uscUploadAttachOmissis.ButtonRemoveEnabled = False

        'disabilita controllo upload Allegati Privacy
        uscUploadPrivacyAttachment.ButtonFileEnabled = False
        uscUploadPrivacyAttachment.ButtonLibrarySharepointEnabled = False
        uscUploadPrivacyAttachment.ButtonScannerEnabled = False
        uscUploadPrivacyAttachment.SignButtonEnabled = False
        uscUploadPrivacyAttachment.ButtonRemoveEnabled = False

        'disabilita controllo upload Annessi
        uscUploadAnnexed.ButtonFileEnabled = False
        uscUploadAnnexed.ButtonLibrarySharepointEnabled = False
        uscUploadAnnexed.ButtonScannerEnabled = False
        uscUploadAnnexed.SignButtonEnabled = False
        uscUploadAnnexed.ButtonRemoveEnabled = False
    End Sub

    Private Sub SetAuthorizationFromCollaboration()
        ' Autorizzazioni da collaborazione
        If Not String.IsNullOrEmpty(ResolutionEnv.CollResolutionRole) Then
            Dim roles As IList(Of Role) = Facade.CollaborationFacade.GetSecretaryRoles(CurrentCollaboration, DocSuiteContext.Current.User.FullUserName)
            If Not roles.IsNullOrEmpty() Then
                tblCollaborationAuthorize.Visible = True
                collaborationAuthorizedRoles.SourceRoles = roles.ToList()
                collaborationAuthorizedRoles.DataBind(False, False)
            End If
        End If
    End Sub
    Private Sub SetCollaboration()
        DisableComponentFromCollaboration()

        SelOggetto.Text = CurrentCollaboration.CollaborationObject
        txtNote.Text = CurrentCollaboration.Note
        Select Case CurrentCollaboration.DocumentType
            Case CollaborationDocumentType.D.ToString()
                rblProposta.SelectedValue = "1"
            Case CollaborationDocumentType.A.ToString()
                rblProposta.SelectedValue = "0"
        End Select
        rblProposta_SelectedIndexChanged(rblProposta, New EventArgs())

        'Area Documentale: Documenti
        If (CurrentCollaboration.GetFirstDocumentVersioning() <> 0) Then
            Try
                Dim doc As New BiblosDocumentInfo(CurrentCollaboration.Location.DocumentServer, CurrentCollaboration.Location.ProtBiblosDSDB, CurrentCollaboration.GetFirstDocumentVersioning(), 0)
                uscUploadDocumenti.LoadDocumentInfo(doc, True)
                If DocSuiteContext.Current.PrivacyLevelsEnabled AndAlso doc IsNot Nothing Then
                    uscUploadDocumenti.FromCollaborationPrivacyLevelEnabled = True
                End If
            Catch ex As Exception
                FileLogger.Warn(LoggerName, "Errore estrazione Documento: " & ex.Message, ex)
                AjaxAlert("Errore estrazione Documento: " & ex.Message)
            End Try
        Else
            AjaxAlert("Errore in Estrazione del Documento: nessun documento trovato")
        End If
        'Area Documentale: Documenti omissis
        Dim documentsOmissisDictionary As IDictionary(Of Guid, BiblosDocumentInfo) = Facade.CollaborationVersioningFacade.GetLastVersionDocuments(CurrentCollaboration, VersioningDocumentGroup.MainDocumentOmissis)
        If documentsOmissisDictionary IsNot Nothing Then
            Dim documentsOmissis As New List(Of DocumentInfo)(documentsOmissisDictionary.Values)
            If documentsOmissis IsNot Nothing AndAlso documentsOmissis.Count > 0 Then
                uscUploadDocumentiOmissis.LoadDocumentInfo(documentsOmissis, True, True, True, True)
            End If
        End If

        'Area Documentale: Allegati
        Dim attachmentsDictionary As IDictionary(Of Guid, BiblosDocumentInfo) = Facade.CollaborationVersioningFacade.GetLastVersionDocuments(CurrentCollaboration, VersioningDocumentGroup.Attachment)
        If attachmentsDictionary IsNot Nothing Then
            Dim attachments As New List(Of DocumentInfo)(attachmentsDictionary.Values)
            If attachments IsNot Nothing AndAlso attachments.Count > 0 Then
                uscUploadAttach.LoadDocumentInfo(attachments, True, True, True, True)
            End If
        End If

        'Area Documentale: Allegati omissis
        Dim attachmentsOmissisDictionary As IDictionary(Of Guid, BiblosDocumentInfo) = Facade.CollaborationVersioningFacade.GetLastVersionDocuments(CurrentCollaboration, VersioningDocumentGroup.AttachmentOmissis)
        If attachmentsOmissisDictionary IsNot Nothing Then
            Dim attachmentsOmissis As New List(Of DocumentInfo)(attachmentsOmissisDictionary.Values)
            If attachmentsOmissis IsNot Nothing AndAlso attachmentsOmissis.Count > 0 Then
                uscUploadAttachOmissis.LoadDocumentInfo(attachmentsOmissis, True, True, True, True)
            End If
        End If

        'Area Documentale: Annessi
        Dim annexedDictionary As IDictionary(Of Guid, BiblosDocumentInfo) = Facade.CollaborationVersioningFacade.GetLastVersionDocuments(CurrentCollaboration, VersioningDocumentGroup.Annexed)
        If annexedDictionary IsNot Nothing Then
            Dim annexed As New List(Of DocumentInfo)(annexedDictionary.Values)
            If annexed IsNot Nothing AndAlso annexed.Count > 0 Then
                uscUploadAnnexed.LoadDocumentInfo(annexed, True, True, True, True)
            End If
        End If

        SetAuthorizationFromCollaboration()

        'Setto il proponente di default che è il medesimo settore della segreteria che ha generato l'atto
        If RoleProposerEnabled Then
            Dim proposerCollRoles As IList(Of Role) = Facade.CollaborationFacade.GetSecretaryRoles(CurrentCollaboration, DocSuiteContext.Current.User.FullUserName)
            If proposerCollRoles.Any() Then
                Dim distinctRole As List(Of Role) = proposerCollRoles.Distinct().ToList()
                If distinctRole.Count = 1 Then
                    uscRoleProposer.SourceRoles = distinctRole
                    uscRoleProposer.DataBind()
                Else
                    'apro una nuova modale per la selezione del proponente da salvare
                    SelectRoleProposerFromCollaboration(distinctRole)
                End If
            End If
        End If
    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnInserimento, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, rblProposta)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblProposta, uscUploadDocumenti)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscUploadAnnexed)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnInserimento, SelOggetto)

        If DocSuiteContext.Current.PrivacyLevelsEnabled Then
            AjaxManager.AjaxSettings.AddAjaxSetting(idContainer, uscUploadDocumenti)
            AjaxManager.AjaxSettings.AddAjaxSetting(uscUploadDocumenti, uscUploadDocumenti)
            AjaxManager.AjaxSettings.AddAjaxSetting(idContainer, uscUploadAnnexed)
            If PnlAllegatiVisible Then
                AjaxManager.AjaxSettings.AddAjaxSetting(idContainer, uscUploadAttach)
            End If
            If ResolutionEnv.AttachmentOmissisEnable Then
                AjaxManager.AjaxSettings.AddAjaxSetting(idContainer, uscUploadAttachOmissis)
            End If
            If ResolutionEnv.MainDocumentOmissisEnable Then
                AjaxManager.AjaxSettings.AddAjaxSetting(idContainer, uscUploadDocumentiOmissis)
            End If
        End If

        Select Case ResolutionEnv.Configuration
            Case ConfTo
                AjaxManager.AjaxSettings.AddAjaxSetting(idContainer, pnlObjectPrivacy)
            Case ConfAuslPc
                If ResolutionEnv.CheckOCValidations Then
                    AjaxManager.AjaxSettings.AddAjaxSetting(idContainer, pnlObjectPrivacy)
                    AjaxManager.AjaxSettings.AddAjaxSetting(rblProposta, pnlOCSupervisoryBoard)
                    AjaxManager.AjaxSettings.AddAjaxSetting(rblProposta, pnlOCConfSindaci)
                    AjaxManager.AjaxSettings.AddAjaxSetting(rblProposta, pnlOCRegion)
                    AjaxManager.AjaxSettings.AddAjaxSetting(rblProposta, pnlOCManagement)
                    AjaxManager.AjaxSettings.AddAjaxSetting(rblProposta, pnlOCCorteConti)
                    AjaxManager.AjaxSettings.AddAjaxSetting(rblProposta, pnlOCOther)
                Else
                    AjaxManager.AjaxSettings.AddAjaxSetting(rblProposta, pnlAUSLREOC)
                    AjaxManager.AjaxSettings.AddAjaxSetting(rblProposta, pnlAttach)
                    AjaxManager.AjaxSettings.AddAjaxSetting(chkAUSLREOCSoggetta, pnlCategory)
                    AjaxManager.AjaxSettings.AddAjaxSetting(chkAUSLREOCNonSoggetta, pnlCategory)
                    AjaxManager.AjaxSettings.AddAjaxSetting(chkAUSLREOCSoggetta, pnlImmediatelyExecutive)
                    AjaxManager.AjaxSettings.AddAjaxSetting(chkAUSLREOCNonSoggetta, pnlImmediatelyExecutive)
                    AjaxManager.AjaxSettings.AddAjaxSetting(chkAUSLREOCSoggetta, idContainer)
                    AjaxManager.AjaxSettings.AddAjaxSetting(chkAUSLREOCNonSoggetta, idContainer)
                End If

            Case Else
                AjaxManager.AjaxSettings.AddAjaxSetting(rblProposta, pnlAttach)
        End Select

        If ResolutionEnv.ResolutionKindEnabled Then
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlResolutionKind, pnlAmmTrasp)
        End If

        AjaxManager.AjaxSettings.AddAjaxSetting(rblProposta, pnlCategory)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblProposta, idContainer)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblProposta, pnlObjectPrivacy)

        ' Controlli che aggiornano il pannello allegati riservati.
        AjaxManager.AjaxSettings.AddAjaxSetting(rblProposta, pnlPrivacyAttachment)

        AjaxManager.AjaxSettings.AddAjaxSetting(idContainer, pnlPrivacyAttachment)

        If chbAdoption.Visible Then
            AjaxManager.AjaxSettings.AddAjaxSetting(rblProposta, chbAdoption)
        End If
        If pnlAdozioneData.Visible Then
            AjaxManager.AjaxSettings.AddAjaxSetting(rblProposta, pnlAdozioneData)
        End If
        If pnlAdozioneNumero.Visible Then
            AjaxManager.AjaxSettings.AddAjaxSetting(rblProposta, pnlAdozioneNumero)
        End If

        'Chiamate Ajax pannelli autorizzazioni
        If ResolutionEnv.AuthorizInsert <> 0 Then
            AjaxManager.AjaxSettings.AddAjaxSetting(uscDestinatari, uscSettori)
            AjaxManager.AjaxSettings.AddAjaxSetting(uscSettori, uscSettori)
        End If

        If ResolutionEnv.AuthorizInsertProposerRolesEnabled Then
            AjaxManager.AjaxSettings.AddAjaxSetting(SelProponente, uscSettori)
            AjaxManager.AjaxSettings.AddAjaxSetting(uscSettori, uscSettori)
        End If

        'Pannello privacy per ASMN in inserimento
        If DocSuiteContext.Current.ResolutionEnv.UseSharepointPublication Then
            AjaxManager.AjaxSettings.AddAjaxSetting(uscUploadDocumenti, uscPrivacyPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscPrivacyPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(rblProposta, uscPrivacyPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, tblResolutionNumber)

            'Comandi necessari per bloccare i controlli in fase di inserimento parziale
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, idContainer)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, rdpDataAdozione)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, txtServiceNumber)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, SelOggetto)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, txtNote)
        End If

        'AjaxManager.AjaxSettings.AddAjaxSetting(rblProposta, SelProponenteAlt)
        'AjaxManager.AjaxSettings.AddAjaxSetting(rblProposta, SelProponente)
        'AjaxManager.AjaxSettings.AddAjaxSetting(rblProposta, uscRoleProposer)
        'AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, SelProponenteAlt)
        'AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, SelProponente)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblProposta, pnlProposer)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlProposer)
        'AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscRoleProposer)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSelectRoleProposer, uscRoleProposer)
        AjaxManager.AjaxSettings.AddAjaxSetting(rtvRoles, rtvRoles)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSelectRoleProposer, btnSelectRoleProposer)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnInserimento, proposerGeneralValidator)

        If TblDestinatari.Visible Then
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscDestinatariAlt)
        End If
        If pnlAssegnatario.Visible Then
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, SelAssegnatarioAlt)
        End If
        If pnlResponsabile.Visible Then
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, SelResponsabileAlt)
        End If

        If ResolutionEnv.ResolutionKindEnabled Then
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlAmmTrasp)
            AjaxManager.AjaxSettings.AddAjaxSetting(dgvResolutionKindDocumentSeries, dgvResolutionKindDocumentSeries)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlResolutionKind, pnlAmmTrasp)
            AjaxManager.AjaxSettings.AddAjaxSetting(windowSelDraftSeries, windowSelDraftSeries)
        End If

        AddHandler AjaxManager.AjaxRequest, AddressOf ReslInserimentoAjaxRequest
    End Sub

    Private Sub InitializeVisibleProperty()

        Dim proposta As Short = Short.Parse(rblProposta.SelectedValue)

        ' Assegnatario 
        ' Se è contenuto IN è visibile il pannello contenitore 
        pnlAssegnatario.Visible = Facade.ResolutionFacade.IsManagedProperty("Assignee", proposta, "IN")
        ' Se è contenuto CONTACT.AD è visibile la ricerca per rubrica aziendale
        pnlAsseInterop.Visible = Facade.ResolutionFacade.IsManagedProperty("Assignee", proposta, "CONTACT.AD")
        SelAssegnatario.Visible = Facade.ResolutionFacade.IsManagedProperty("Assignee", proposta, "CONTACT.AD")
        SelAssegnatario.ButtonSelectVisible = Not Facade.ResolutionFacade.IsManagedProperty("Assignee", proposta, "CONTACT.AD")
        SelAssegnatario.ButtonSelectDomainVisible = Facade.ResolutionFacade.IsManagedProperty("Assignee", proposta, "CONTACT.AD")
        ' Se è contenuto CONTACT è visibile la ricerca per Testo 
        SelAssegnatarioAlt.Visible = Not pnlAsseInterop.Visible

        If Facade.ResolutionFacade.IsManagedProperty("Assignee", proposta, "LDAP") Then
            pnlAsseInterop.Visible = False
            SelAssegnatarioAlt.Visible = True
            SelAssegnatarioAlt.ButtonADContactVisible = True
            SelAssegnatarioAlt.ButtonSelContactVisible = False
        End If


        ' Responsabile 
        ' Se è contenuto IN è visibile il pannello contenitore 
        pnlResponsabile.Visible = Facade.ResolutionFacade.IsManagedProperty("Manager", proposta, "IN")

        pnlRespInterop.Visible = Facade.ResolutionFacade.IsManagedProperty("Manager", proposta, "CONTACT.AD")
        SelResponsabile.Visible = Facade.ResolutionFacade.IsManagedProperty("Manager", proposta, "CONTACT.AD")
        SelResponsabile.ButtonSelectVisible = Not Facade.ResolutionFacade.IsManagedProperty("Manager", proposta, "CONTACT.AD")
        SelResponsabile.ButtonSelectDomainVisible = Facade.ResolutionFacade.IsManagedProperty("Manager", proposta, "CONTACT.AD")
        ' Se è contenuto CONTACT è visibile la ricerca per Testo 
        SelResponsabileAlt.Visible = Not pnlRespInterop.Visible

        If Facade.ResolutionFacade.IsManagedProperty("Manager", proposta, "LDAP") Then
            pnlRespInterop.Visible = False
            SelResponsabileAlt.Visible = True
            SelResponsabileAlt.ButtonADContactVisible = True
            SelResponsabileAlt.ButtonSelContactVisible = False
        End If


        ' Proponente 
        ' UserControl Se ruolo del proponente
        ' Se è contenuto IN è visibile il pannello contenitore 
        pnlProposer.SetDisplay(Facade.ResolutionFacade.IsManagedProperty("Proposer", proposta, "IN"))
        ' Se è contenuto CONTACT.AD è visibile la ricerca per rubrica aziendale
        SelProponente.ButtonSelectVisible = Not Facade.ResolutionFacade.IsManagedProperty("Proposer", proposta, "CONTACT.AD")
        SelProponente.ButtonSelectDomainVisible = Facade.ResolutionFacade.IsManagedProperty("Proposer", proposta, "CONTACT.AD")
        ' Se è contenuto CONTACT è visibile la ricerca per Testo
        pnlProponenteInterop.Visible = Facade.ResolutionFacade.IsManagedProperty("Proposer", proposta, "CONTACT")
        SelProponenteAlt.Visible = Not ResolutionEnv.HideAlternativeProposer
        uscRoleProposer.Visible = RoleProposerEnabled

        If Facade.ResolutionFacade.IsManagedProperty("Proposer", proposta, "LDAP") Then
            pnlProponenteInterop.Visible = False
            SelProponenteAlt.Visible = True
            SelProponenteAlt.ButtonADContactVisible = True
            SelProponenteAlt.ButtonSelContactVisible = False
        End If

        If RoleProposerEnabled Then
            uscRoleProposer.Required = Facade.ResolutionFacade.IsManagedProperty("Proposer", proposta, "OB")
            uscRoleProposer.RequiredMessage = "Campo Proponente Obbligatorio"
        Else
            SelProponente.IsRequired = Facade.ResolutionFacade.IsManagedProperty("Proposer", proposta, "OB")
        End If


        ' Destinatario 
        ' Se è contenuto IN è visibile il pannello contenitore 
        ' Se è contenuto CONTACT.AD è visibile la ricerca per rubrica aziendale
        uscDestinatari.ButtonSelectVisible = Not Facade.ResolutionFacade.IsManagedProperty("Recipient", proposta, "CONTACT.AD")
        uscDestinatari.ButtonSelectDomainVisible = Facade.ResolutionFacade.IsManagedProperty("Recipient", proposta, "CONTACT.AD")
        ' Se è attivo il parametro, non si vede il pannello di destinatario alternativo
        uscDestinatariAlt.Visible = Not ResolutionEnv.HideAlternativeRecipient

        pnlCategory.Visible = Facade.ResolutionFacade.IsManagedProperty("Category", proposta)
        uscUploadDocumenti.IsDocumentRequired = IsDocumentoObbligatorio()

        ' Verifico abilitazione e permessi della gestione allegati riservati.
        pnlPrivacyAttachment.SetDisplay(CurrentResolutionRights.IsPrivacyAttachmentAllowed)

        Select Case ResolutionEnv.Configuration
            Case ConfTo
                PnlAllegatiVisible = True
                pnlAdozione.Visible = False
                pnlAutorizzazioni.Visible = True
                pnlImmediatelyExecutive.Visible = True
                pnlObjectPrivacy.Visible = False
                pnlOC.Visible = True
                pnlOCManagement.Visible = False
                uscDestinatari.ButtonSelectVisible = True
                uscDestinatari.ButtonSelectDomainVisible = False

         'EF 20120111 Separati i branch per poter eseguire singole modifiche su Piacenza
            Case ConfAuslPc
                PnlAllegatiVisible = True 'EF 20120111 Attivati gli allegati normali
                pnlAdozione.Visible = False
                pnlAutorizzazioni.Visible = False
                pnlImmediatelyExecutive.Visible = False 'EF 20120111 Disattivato il flag *Immediatamente esecutiva"
                pnlObjectPrivacy.Visible = False

                If ResolutionEnv.CheckOCValidations Then
                    'EF 20120117 Gestione degli organi di Controllo
                    TblDestinatari.Visible = False 'EF 20120111 Disattivata la tabella "Destinatari"
                    pnlOC.Visible = True
                    chkOCSupervisoryBoard.Checked = True
                    chkOCSupervisoryBoard.Enabled = False
                    pnlOCManagement.Visible = False
                    pnlOCCorteConti.Visible = False
                    pnlOCOther.Visible = False

                    'EF 20120117 Abilita il controllo della regione e della conferenza dei sindaci solo se è una delibera
                    pnlOCRegion.Visible = (proposta = 1)
                    pnlOCConfSindaci.Visible = (proposta = 1)
                Else
                    pnlAUSLREOC.Visible = (proposta = 1)

                    If pnlAUSLREOC.Visible Then
                        chkAUSLREOCSoggetta.Attributes.Add("onclick", "javascript:if (this.checked) {document.getElementById('" & chkAUSLREOCNonSoggetta.ClientID & "').checked = false}")
                        chkAUSLREOCNonSoggetta.Attributes.Add("onclick", "javascript:if (this.checked) {document.getElementById('" & chkAUSLREOCSoggetta.ClientID & "').checked = false}")
                    End If
                End If
            Case Else
                PnlAllegatiVisible = Facade.ResolutionFacade.IsManagedProperty("idAttachements", proposta, "")

        End Select


        'Condizioni per abilitare il nuovo validatore generale se visibili entrambi i controlli del proponente
        proposerGeneralValidator.Enabled = False
        If ((SelProponente.Visible AndAlso SelProponenteAlt.Visible) _
            OrElse (Facade.ResolutionFacade.IsManagedProperty("Proposer", proposta, "LDAP") _
            AndAlso Facade.ResolutionFacade.IsManagedProperty("Proposer", proposta, "OB"))) _
            AndAlso Not Facade.ResolutionFacade.IsManagedProperty("Proposer", proposta, "ONLYFIRST") Then
            proposerGeneralValidator.Enabled = SelProponente.IsRequired
            SelProponente.IsRequired = Not proposerGeneralValidator.Enabled
        End If
    End Sub

    Private Sub Initialize()
        Title = String.Format("{0} - Inserimento", Facade.TabMasterFacade.TreeViewCaption)
        ' Lunghezza oggetto
        SelOggetto.MaxLength = DocSuiteContext.Current.ResolutionEnv.ObjectMaxLength

        Dim s As String = String.Empty
        If Facade.ResolutionFacade.IsManagedProperty("idProposalFile", ResolutionType.IdentifierDelibera) Then
            s = "Proposta di "
        End If
        rblProposta.Items.Add(s & Facade.ResolutionTypeFacade.DeliberaCaption)
        rblProposta.Items(0).Value = ResolutionType.IdentifierDelibera.ToString()
        s = String.Empty
        If Facade.ResolutionFacade.IsManagedProperty("idProposalFile", ResolutionType.IdentifierDetermina) Then
            s = "Proposta di "
        End If
        rblProposta.Items.Add(s & Facade.ResolutionTypeFacade.DeterminaCaption)
        rblProposta.Items(1).Value = ResolutionType.IdentifierDetermina.ToString()
        lblObjectPrivacy.Text = ResolutionEnv.ResolutionObjectPrivacyLabel
        SelOggettoPrivacy.RequiredMessage = String.Format("Campo {0} Obbligatorio", ResolutionEnv.ResolutionObjectPrivacyLabel)

        Select Case ResolutionEnv.Configuration
            Case ConfTo
                rblProposta.Items(0).Selected = True
            Case ConfAuslPc
                rblProposta.Items(1).Selected = True
                If ResolutionEnv.CheckOCValidations Then
                    rblProposta.Items(1).Selected = False
                    rblProposta.Items(0).Selected = True
                End If
            Case Else
                rblProposta.Items(1).Selected = True
        End Select
        If Not RoleProposerEnabled AndAlso ResolutionEnv.ProposerContact.HasValue Then
            SelProponente.ContactRoot = ResolutionEnv.ProposerContact.Value
        End If
        InitializeVisibleProperty()

        ContainerDdlFiller()

        'Sezione Documenti: Abilitazione Firma
        uscUploadDocumenti.SignButtonEnabled = ResolutionEnv.IsFDQEnabled
        uscUploadAttach.SignButtonEnabled = ResolutionEnv.IsFDQEnabled
        uscUploadPrivacyAttachment.SignButtonEnabled = ResolutionEnv.IsFDQEnabled
        uscUploadAnnexed.SignButtonEnabled = ResolutionEnv.IsFDQEnabled

        'Inizializzazione pannelli di Tipologia Atto
        pnlAmmTrasp.Visible = False
        pnlResolutionKind.Visible = False
        ddlResolutionKindValidator.Enabled = False
        If ResolutionEnv.ResolutionKindEnabled Then
            pnlResolutionKind.Visible = True
            ddlResolutionKindValidator.Enabled = True
            BindResolutionKind()
        End If

        Select Case Action
            Case "Duplicate"
                Duplicate()
            Case "FromCollaboration"
                SetCollaboration()
            Case "FromSeries"
                If CurrentResolutionModel IsNot Nothing Then
                    If CurrentResolutionModel.IdCollaboration.HasValue Then
                        _currColl = FacadeFactory.Instance.CollaborationFacade.GetById(CurrentResolutionModel.IdCollaboration.Value)
                        DisableComponentFromCollaboration()
                        SetAuthorizationFromCollaboration()
                    End If
                    BindPageFromModel()
                End If
        End Select

        'Pulisco eventuali sessioni attive        
        If Not Action = "FromSeries" Then
            CurrentResolutionModel = Nothing
            DraftSeriesItemAdded = Nothing
            dgvResolutionKindDocumentSeries.DataBind()
        End If

        If ResolutionEnv.ManageDefaultCategory AndAlso Not Action = "FromSeries" Then
            If Not Action.Eq("Duplicate") OrElse (Action.Eq("Duplicate") AndAlso uscSelCategory.DataSource.IsNullOrEmpty()) Then
                UpdateCategory()
            End If
        End If

    End Sub

    Private Sub InitializeDocumentControls()
        lblDocumentCaption.Text = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.ProposalChain)
        uscUploadDocumenti.TreeViewCaption = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.ProposalChain)

        lblDocumentOmissisCaption.Text = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.MainOmissisChain)
        uscUploadDocumentiOmissis.TreeViewCaption = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.MainOmissisChain)

        lblAttachmentsCaption.Text = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.AttachmentsChain)
        uscUploadAttach.TreeViewCaption = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.AttachmentsChain)

        lblAttachmentsOmissisCaption.Text = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.AttachmentOmissisChain)
        uscUploadAttachOmissis.TreeViewCaption = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.AttachmentOmissisChain)

        lblPrivacyAttachmentCaption.Text = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.PrivacyAttachmentChain)
        uscUploadPrivacyAttachment.TreeViewCaption = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.PrivacyAttachmentChain)

        lblAnnexedCaption.Text = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.AnnexedChain)
        uscUploadAnnexed.TreeViewCaption = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.AnnexedChain)
    End Sub

    ''' <summary> Verifica che nella ListItemCollection ricevuta vi sia almeno un elemento avente come valore quello passato come parametro. </summary>
    ''' <param name="pListItemCollection">ListItemCollection da verificare.</param>
    ''' <param name="value">Valore da cercare.</param>
    Private Shared Function ListItemCollectionContains(pListItemCollection As ListItemCollection, value As String) As Boolean
        Return pListItemCollection.Cast(Of ListItem)().Any(Function(i) i.Value.Eq(value))

    End Function

    Private Sub SelectedContainerChanged()
        pnlObjectPrivacy.Visible = False
        If SelectedContainer IsNot Nothing AndAlso SelectedContainer.Privacy.HasValue Then
            pnlObjectPrivacy.Visible = CType(SelectedContainer.Privacy.Value, Boolean)
        End If
        ' Verifico abilitazione e permessi della gestione allegati riservati.
        pnlPrivacyAttachment.SetDisplay(CurrentResolutionRights.IsPrivacyAttachmentAllowed)

        If DocSuiteContext.Current.PrivacyLevelsEnabled Then
            If Action.Eq("FromCollaboration") Then
                uscUploadDocumenti.FromCollaborationPrivacyLevelEnabled = True
                uscUploadAttach.FromCollaborationPrivacyLevelEnabled = True
                uscUploadAnnexed.FromCollaborationPrivacyLevelEnabled = True
            End If
            InitDocumentsPrivacyLevels(True)
        End If

    End Sub

    Private Sub InitializeControls()

        pnlAdozione.Visible = ResolutionEnv.IsInsertAdoption

        If ResolutionEnv.MainDocumentOmissisEnable Then
            uscUploadDocumentiOmissis.ButtonPrivacyLevelVisible = False
        End If

        If ResolutionEnv.AttachmentOmissisEnable Then
            uscUploadAttachOmissis.ButtonPrivacyLevelVisible = False
        End If

        If PnlAllegatiVisible Then
            uscUploadAttach.ButtonPrivacyLevelVisible = DocSuiteContext.Current.PrivacyLevelsEnabled
        End If
        uscUploadPrivacyAttachment.ButtonPrivacyLevelVisible = False
        uscUploadDocumenti.ButtonPrivacyLevelVisible = DocSuiteContext.Current.PrivacyLevelsEnabled
        uscUploadAnnexed.ButtonPrivacyLevelVisible = DocSuiteContext.Current.PrivacyLevelsEnabled
        If DocSuiteContext.Current.PrivacyLevelsEnabled Then
            InitDocumentsPrivacyLevels(False)
        End If

        ''Gestione catene documentali aggiuntive Omissis
        If ResolutionEnv.MainDocumentOmissisEnable Then
            mainDocumentOmissisTr.Style.Remove("display")
        End If

        If ResolutionEnv.AttachmentOmissisEnable Then
            attachmentOmissisTr.Style.Remove("display")
        End If

        uscUploadAttach.ButtonCopyResl.Visible = ResolutionEnv.CopyReslDocumentsEnabled
        uscUploadAttach.ButtonCopyProtocol.Visible = ResolutionEnv.CopyProtocolDocumentsEnabled
        uscUploadAnnexed.ButtonCopyResl.Visible = ResolutionEnv.CopyReslDocumentsEnabled
        uscUploadAnnexed.ButtonCopyProtocol.Visible = ResolutionEnv.CopyProtocolDocumentsEnabled
        If ProtocolEnv.DematerialisationEnabled Then
            uscUploadDocumenti.CheckDematerialisationCompliance = True
            uscUploadAttach.CheckDematerialisationCompliance = True
            uscUploadAnnexed.CheckDematerialisationCompliance = False
        End If

        Select Case ResolutionEnv.Configuration
            Case ConfTo
                idContainer.AutoPostBack = True
                uscUploadDocumenti.MultipleDocuments = True
                uscSelCategory.CategoryID = ResolutionEnv.CategoryRoot
                AddHandler idContainer.SelectedIndexChanged, AddressOf IdContainerSelectedIndexChanged
            Case Else
                AddHandler chbAdoption.CheckedChanged, AddressOf chbAdoption_CheckedChanged
        End Select

        ' Abilita Autorizzazioni
        Select Case ResolutionEnv.AuthorizInsert
            Case 0
                pnlAutorizzazioni.Visible = False
            Case 1, 2
                pnlAutorizzazioni.Visible = True
                If ProtocolEnv.MultiDomainEnabled AndAlso ProtocolEnv.TenantAuthorizationEnabled Then
                    uscSettori.TenantEnabled = True
                End If
                uscSettori.Required = (ResolutionEnv.AuthorizInsert = 2)
                AddHandler uscDestinatari.ContactRemoved, AddressOf UscContactRoleUpdate
                AddHandler uscDestinatari.ContactAdded, AddressOf UscContactRoleUpdate
            Case Else
                Throw New DocSuiteException("Inserimento Resolution", "Valore AuthorizInsert non previsto: " & ResolutionEnv.AuthorizInsert)
        End Select

        If ResolutionEnv.AuthorizInsertProposerRolesEnabled Then
            pnlAutorizzazioni.Visible = True
            AddHandler SelProponente.ContactAdded, AddressOf UscProposerRoleUpdate
            AddHandler SelProponente.ContactRemoved, AddressOf UscProposerRoleUpdate
        End If

    End Sub

    Private Function IsDocumentoObbligatorio() As Boolean
        Dim proposta As Short = Short.Parse(rblProposta.SelectedValue)

        If Facade.ResolutionFacade.IsManagedProperty("idProposalFile", proposta) Then
            Return Facade.ResolutionFacade.IsManagedProperty("idProposalFile", proposta, "IN-OB")
        End If

        If Facade.ResolutionFacade.IsManagedProperty("idResolutionFile", proposta) Then
            Return Facade.ResolutionFacade.IsManagedProperty("idResolutionFile", proposta, "IN-OB")
        End If

        Return False
    End Function

    Private Sub InitizializePrivacy()
        '' Pannello controllo privacy
        '' Se il tipo di atto è delibera, allora attivo il controllo privacy
        uscPrivacyPanel.PrivacyTypeVisible = ResolutionEnv.UseSharepointPublication AndAlso (rblProposta.SelectedValue = "1" OrElse uscUploadDocumenti.DocumentsCount > 0)
        uscPrivacyPanel.ValidatorEnabled = uscPrivacyPanel.PrivacyTypeVisible
        uscPrivacyPanel.ResolutionTypeId = CurrentResolutionType
        '' Se è invece disposizione, attivo il controllo solo se ho inserito documenti e lo faccio dal callback di documento caricato
    End Sub

    ''' <summary> Riempie la dropdown dei contenitori. </summary>
    Private Sub ContainerDdlFiller()

        idContainer.Items.Clear()
        idContainer.Items.Add(New ListItem("", ""))

        If ResolutionEnv.UseContainerResolutionType Then
            ' Creo la lista in base all'associazione contenuta sulla tabella ContainerResolutionType
            Dim typeId As Short = Short.Parse(rblProposta.SelectedValue)
            Dim crtf As New ContainerResolutionTypeFacade
            Dim listcontainer As IList(Of ContainerResolutionType) = crtf.GetAllowedContainers(typeId, 1, ResolutionRightPositions.Insert)

            ' Rimuovere il controllo, diritti di inserimento, solo se è attivo il parametro UseContainerResolutionType
            If listcontainer.Count <= 0 AndAlso Not ResolutionEnv.UseContainerResolutionType Then
                Throw New DocSuiteException(Facade.TabMasterFacade.TreeViewCaption & " Inserimento", "Utente senza diritti di Inserimento")
            End If

            ' TODO: verificare da chi viene impostata questa variabile di sessione
            Dim mContainer As String = CType(Session("ReslContainer"), String)
            If ResolutionEnv.ManageDefaultContainer Then
                Dim container As Container = ResolutionEnv.GetDefaultContainer(CurrentResolutionType)
                If Not container Is Nothing Then
                    mContainer = container.Id.ToString()
                End If
            End If

            Dim tempId As Integer
            For Each container As ContainerResolutionType In listcontainer
                If tempId = container.Id.idContainer Then
                    Continue For
                End If

                WebUtils.ObjDropDownListAdd(idContainer, container.container.Name, container.Id.idContainer.ToString())
                tempId = container.Id.idContainer
                If idContainer.Items(idContainer.Items.Count - 1).Value = mContainer Then
                    idContainer.SelectedIndex = idContainer.Items.Count - 1
                End If

            Next container

            ' Qualora la combo dei contenitori non abbia nessun elemento selezionato,
            '  la proprietà PrevSelectedIdContainer sia valorizzata con l'id contenitore precedentemente selezionato
            '  e la combo contenga un elemento avente come valore quello della proprietà PrevSelectedIdContainer,
            '  reimposto la selezione della combo.
            If idContainer.SelectedIndex < 1 AndAlso PrevSelectedIdContainer <> -1 AndAlso ListItemCollectionContains(idContainer.Items, CType(PrevSelectedIdContainer, String)) Then
                idContainer.SelectedValue = CType(PrevSelectedIdContainer, String)
            End If
        Else
            Dim listcontainer As IList(Of Container) = Facade.ContainerFacade.GetContainers(DSWEnvironment.Resolution, ResolutionRightPositions.Insert, True)
            If listcontainer.Count <= 0 Then
                Throw New DocSuiteException(Facade.TabMasterFacade.TreeViewCaption & " Inserimento", "Utente senza diritti di Inserimento")
            End If

            ' TODO: verificare da chi viene impostata questa variabile di sessione
            Dim mContainer As String = CType(Session("ReslContainer"), String)
            If ResolutionEnv.ManageDefaultContainer Then
                Dim container As Container = ResolutionEnv.GetDefaultContainer(CurrentResolutionType)
                If Not container Is Nothing Then
                    mContainer = container.Id.ToString()
                End If
            End If

            Dim tempId As Integer
            For Each container As Container In listcontainer
                If tempId = container.Id Then
                    Continue For
                End If

                WebUtils.ObjDropDownListAdd(idContainer, container.Name, container.Id.ToString())
                tempId = container.Id
                If idContainer.Items(idContainer.Items.Count - 1).Value = mContainer Then
                    idContainer.SelectedIndex = idContainer.Items.Count - 1
                End If
            Next container

            ' Se non impostato si verifica con il valore precedente
            If idContainer.SelectedIndex <= 0 And PrevSelectedIdContainer <> -1 Then
                idContainer.SelectedValue = CType(PrevSelectedIdContainer, String)
            End If

        End If

        SelectedContainerChanged()

    End Sub

    Private Sub UpdateCategory()
        If Not ResolutionEnv.ManageDefaultCategory Then
            Exit Sub
        End If

        Dim c As Category = ResolutionEnv.GetDefaultCategory(CurrentResolutionType)
        If Not c Is Nothing Then
            Dim list As New List(Of Category)
            list.Add(c)
            uscSelCategory.DataSource = list
        Else
            uscSelCategory.Clear()
            uscSelCategory.DataSource = New List(Of Category)
        End If
        uscSelCategory.DataBind()
    End Sub

    Private Sub UpdateContainer()
        If Not ResolutionEnv.ManageDefaultContainer Then
            Exit Sub
        End If

        Dim c As Container = ResolutionEnv.GetDefaultContainer(CurrentResolutionType)

        If Not c Is Nothing AndAlso idContainer.Items.Contains(New ListItem(c.Name, c.Id.ToString())) Then
            idContainer.SelectedValue = c.Id.ToString()
            Exit Sub
        End If

        ' Se non impostato si verifica con il valore precedente
        If (PrevSelectedIdContainer <> -1) AndAlso (idContainer.Items.FindByValue(CType(PrevSelectedIdContainer, String)) IsNot Nothing) Then
            idContainer.SelectedValue = CType(PrevSelectedIdContainer, String)
        Else
            idContainer.SelectedIndex = -1
        End If

    End Sub

    Private Sub BlockInsert()
        ' Provvedo a bloccare i campi dai quali devo copiare i dati
        uscPrivacyPanel.PrivacySelectorEnabled = False

        uscUploadDocumenti.ReadOnly = True
        idContainer.Enabled = False
        If rdpDataAdozione.Visible Then
            rdpDataAdozione.Enabled = False
        End If
        txtServiceNumber.Enabled = False
        SelOggetto.Enabled = False
        rblProposta.Enabled = False

        ''Altri campi da bloccare
        uscDestinatariAlt.ReadOnly = True
        uscDestinatari.ReadOnly = True
        'Proponente
        If RoleProposerEnabled Then
            uscRoleProposer.ReadOnly = True
        Else
            SelProponente.ReadOnly = True
        End If

        SelProponenteAlt.ReadOnly = True
        txtNote.Enabled = False
        uscUploadDocumenti.ReadOnly = True
        uscUploadAttach.ReadOnly = True
        uscUploadPrivacyAttachment.ReadOnly = True
        uscUploadAnnexed.ReadOnly = True
    End Sub

    Private Sub BindResolutionKind()
        Dim kinds As ICollection(Of ResolutionKind) = New ResolutionKindFacade(DocSuiteContext.Current.User.FullUserName).GetActiveResolutionKind()

        If kinds.Count > 1 Then
            ddlResolutionKind.Items.Add(New ListItem(String.Empty, String.Empty))
        End If

        For Each resolutionKind As ResolutionKind In kinds.OrderBy(Function(o) o.Name)
            Dim item As ListItem = New ListItem()
            item.Value = resolutionKind.Id.ToString()
            item.Text = resolutionKind.Name

            ddlResolutionKind.Items.Add(item)
        Next

        If kinds.Count.Equals(1) Then
            ddlResolutionKind.SelectedIndex = 0
            ddlResolutionKind.Enabled = False
            Call DdlResolutionKind_SelectedIndexChanged(Me.Page, Nothing)
        End If
    End Sub

    Private Sub BindModelFromPage()
        Dim model As ResolutionInsertModel = New ResolutionInsertModel()
        'Tipologia atto
        model.ProposalType = Convert.ToInt32(rblProposta.SelectedValue)
        If (Request.QueryString("IdCollaboration") IsNot Nothing) Then
            model.IdCollaboration = Request.QueryString("IdCollaboration").GetValueOrDefault(Of Integer)(Nothing)
        End If

        If (CurrentResolutionModel IsNot Nothing AndAlso CurrentResolutionModel.IdCollaboration IsNot Nothing AndAlso model.IdCollaboration Is Nothing) Then
            model.IdCollaboration = CurrentResolutionModel.IdCollaboration
        End If

        'Documenti principali
        For Each mainDocument As DocumentInfo In uscUploadDocumenti.DocumentInfos
            model.MainDocuments.Add(mainDocument.Serialized)
        Next

        'Documenti omissis
        For Each mainDocumentOmissis As DocumentInfo In uscUploadDocumentiOmissis.DocumentInfos
            model.MainDocumentOmissis.Add(mainDocumentOmissis.Serialized)
        Next

        'Allegati
        For Each attachment As DocumentInfo In uscUploadAttach.DocumentInfos
            model.Attachments.Add(attachment.Serialized)
        Next

        'Allegati Omissis
        For Each attachmentOmissis As DocumentInfo In uscUploadAttachOmissis.DocumentInfos
            model.AttachmentOmissis.Add(attachmentOmissis.Serialized)
        Next

        'Annessi
        For Each annexed As DocumentInfo In uscUploadAnnexed.DocumentInfos
            model.Annexes.Add(annexed.Serialized)
        Next

        'Contenitore
        If Not String.IsNullOrEmpty(idContainer.SelectedValue) Then
            model.Container = Convert.ToInt32(idContainer.SelectedValue)
        End If

        'Tipologia Atto
        If ResolutionEnv.ResolutionKindEnabled Then
            model.ResolutionKind = Guid.Parse(ddlResolutionKind.SelectedValue)
        End If

        'Dati Adozione
        model.AutomaticAdoption = chbAdoption.Checked
        model.ImmediatelyExecutive = chkImmediatelyExecutive.Checked
        model.AdoptionDate = rdpDataAdozione.SelectedDate
        model.AdoptionNumber = txtServiceNumber.Text

        'Contatti
        model.Recipients = uscDestinatari.GetContacts(True)
        model.AlternativeRecipient = uscDestinatariAlt.GetContactText()

        If RoleProposerEnabled Then
            Dim roles As IList(Of Role) = uscRoleProposer.GetRoles()
            If roles.Any() Then
                model.RoleProposer = roles.First()
            End If
        Else
            model.Proposers = SelProponente.GetContacts(True)
        End If
        model.AlternativeProposer = SelProponenteAlt.GetContactText()

        model.Assignee = SelAssegnatario.GetContacts(True).FirstOrDefault()
        model.AlternativeAssignee = SelAssegnatarioAlt.GetContactText()
        model.Responsible = SelResponsabile.GetContacts(True).FirstOrDefault()
        model.AlternativeResponsible = SelResponsabileAlt.GetContactText()

        'Autorizzazioni
        model.CollaborationAuthorizations = collaborationAuthorizedRoles.GetRoles()
        model.Authorizations = uscSettori.GetRoles()

        'Dati Generici
        model.PrivacySubject = SelOggettoPrivacy.Text
        model.Subject = SelOggetto.Text
        model.Note = txtNote.Text
        If uscSelCategory.SelectedCategories.Any() Then
            model.Category = uscSelCategory.SelectedCategories.Select(Function(s) s.Id).ToList()
        End If

        'OC
        If pnlOC.Visible Then
            model.OcSupervisoryBoard = chkOCSupervisoryBoard.Checked
            model.OcRegion = chkOCRegion.Checked
            model.OcManagement = chkOCManagement.Checked
            model.OcCorteConti = chkOCCorteConti.Checked
            model.OcOther = chkOCOther.Checked
            model.OcOtherDescription = txtOCOtherDescription.Text
        End If

        If pnlAUSLREOC.Visible Then
            model.DelSoggetta = chkAUSLREOCSoggetta.Checked
            model.DelNonSoggetta = chkAUSLREOCNonSoggetta.Checked
        End If

        'Salvo lo stato degli oggetti in sessione
        CurrentResolutionModel = model
    End Sub

    Private Sub BindPageFromModel()
        'Tipologia atto
        rblProposta.SelectedValue = CurrentResolutionModel.ProposalType.ToString()
        Call rblProposta_SelectedIndexChanged(rblProposta, New EventArgs())

        'Documenti principali
        For Each mainDocument As String In CurrentResolutionModel.MainDocuments
            Dim docInfo As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(mainDocument))
            uscUploadDocumenti.LoadDocumentInfo(docInfo, False, True, True, True, False)
        Next

        'Documenti omissis
        For Each mainDocumentOmissis As String In CurrentResolutionModel.MainDocumentOmissis
            Dim docInfo As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(mainDocumentOmissis))
            uscUploadDocumentiOmissis.LoadDocumentInfo(docInfo, False, True, True, True, False)
        Next

        'Allegati
        For Each attachment As String In CurrentResolutionModel.Attachments
            Dim docInfo As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(attachment))
            uscUploadAttach.LoadDocumentInfo(docInfo, False, True, True, True, False)
        Next

        'Allegati Omissis
        For Each attachmentOmissis As String In CurrentResolutionModel.AttachmentOmissis
            Dim docInfo As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(attachmentOmissis))
            uscUploadAttachOmissis.LoadDocumentInfo(docInfo, False, True, True, True, False)
        Next

        'Annessi
        For Each annexed As String In CurrentResolutionModel.Annexes
            Dim docInfo As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(annexed))
            uscUploadAnnexed.LoadDocumentInfo(docInfo, False, True, True, True, False)
        Next

        'Contenitore
        If CurrentResolutionModel.Container.HasValue Then
            idContainer.SelectedValue = CurrentResolutionModel.Container.ToString()
            _selectedContainer = Nothing
            IdContainerSelectedIndexChanged(Me, New EventArgs())
        End If

        'Tipologia Atto
        pnlAmmTrasp.Visible = False
        If ResolutionEnv.ResolutionKindEnabled AndAlso CurrentResolutionModel.ResolutionKind.HasValue Then
            pnlAmmTrasp.Visible = True
            ddlResolutionKind.SelectedValue = CurrentResolutionModel.ResolutionKind.Value.ToString()
            dgvResolutionKindDocumentSeries.DataSource = CurrentSelectedResolutionKind.ResolutionKindDocumentSeries
            dgvResolutionKindDocumentSeries.DataBind()
        End If

        'Dati Adozione
        If CurrentResolutionModel.AutomaticAdoption.HasValue Then
            chbAdoption.Checked = CurrentResolutionModel.AutomaticAdoption.Value
        End If
        If CurrentResolutionModel.ImmediatelyExecutive.HasValue Then
            chkImmediatelyExecutive.Checked = CurrentResolutionModel.ImmediatelyExecutive.Value
        End If


        If CurrentResolutionModel.AdoptionDate.HasValue Then
            rdpDataAdozione.SelectedDate = CurrentResolutionModel.AdoptionDate.Value
        End If

        txtServiceNumber.Text = CurrentResolutionModel.AdoptionNumber

        'Contatti
        uscDestinatari.DataSource = CurrentResolutionModel.Recipients
        uscDestinatari.DataBind()
        uscDestinatariAlt.DataSource = CurrentResolutionModel.AlternativeRecipient

        If RoleProposerEnabled Then
            If CurrentResolutionModel.RoleProposer IsNot Nothing Then
                uscRoleProposer.SourceRoles = New List(Of Role)() From {CurrentResolutionModel.RoleProposer}
                uscRoleProposer.DataBind()
            End If
        Else
            SelProponente.DataSource = CurrentResolutionModel.Proposers
            SelProponente.DataBind()
        End If
        SelProponenteAlt.DataSource = CurrentResolutionModel.AlternativeProposer

        If CurrentResolutionModel.Assignee IsNot Nothing Then
            SelAssegnatario.DataSource = New List(Of ContactDTO) From {CurrentResolutionModel.Assignee}
            SelAssegnatario.DataBind()
        End If
        SelAssegnatarioAlt.DataSource = CurrentResolutionModel.AlternativeAssignee

        If CurrentResolutionModel.Responsible IsNot Nothing Then
            SelResponsabile.DataSource = New List(Of ContactDTO) From {CurrentResolutionModel.Responsible}
            SelResponsabile.DataBind()
        End If
        SelResponsabileAlt.DataSource = CurrentResolutionModel.AlternativeResponsible

        'Autorizzazioni
        For Each collRole As Role In CurrentResolutionModel.CollaborationAuthorizations
            collaborationAuthorizedRoles.SourceRoles.Add(Facade.RoleFacade.GetById(collRole.Id))
        Next
        collaborationAuthorizedRoles.DataBind(False, False)

        For Each role As Role In CurrentResolutionModel.Authorizations
            uscSettori.SourceRoles.Add(Facade.RoleFacade.GetById(role.Id))
        Next
        uscSettori.DataBind(False, False)

        'Dati Generici
        SelOggettoPrivacy.Text = CurrentResolutionModel.PrivacySubject
        SelOggetto.Text = CurrentResolutionModel.Subject
        txtNote.Text = CurrentResolutionModel.Note
        For Each idCategory As Integer In CurrentResolutionModel.Category
            uscSelCategory.DataSource.Add(Facade.CategoryFacade.GetById(idCategory))
        Next
        uscSelCategory.DataBind()

        'OC
        If pnlOC.Visible Then
            If CurrentResolutionModel.OcSupervisoryBoard.HasValue Then
                chkOCSupervisoryBoard.Checked = CurrentResolutionModel.OcSupervisoryBoard.Value
            End If

            If CurrentResolutionModel.OcRegion.HasValue Then
                chkOCRegion.Checked = CurrentResolutionModel.OcRegion.Value
            End If

            If CurrentResolutionModel.OcManagement.HasValue Then
                chkOCManagement.Checked = CurrentResolutionModel.OcManagement.Value
            End If

            If CurrentResolutionModel.OcCorteConti.HasValue Then
                chkOCCorteConti.Checked = CurrentResolutionModel.OcCorteConti.Value
            End If

            If CurrentResolutionModel.OcOther.HasValue Then
                chkOCOther.Checked = CurrentResolutionModel.OcOther.Value
            End If

            txtOCOtherDescription.Text = CurrentResolutionModel.OcOtherDescription
        End If

        If pnlAUSLREOC.Visible Then
            If CurrentResolutionModel.DelSoggetta.HasValue Then
                chkAUSLREOCSoggetta.Checked = CurrentResolutionModel.DelSoggetta.Value
            End If

            If CurrentResolutionModel.DelNonSoggetta.HasValue Then
                chkAUSLREOCNonSoggetta.Checked = CurrentResolutionModel.DelNonSoggetta.Value
            End If
        End If
    End Sub

    Private Sub NewDraftSeriesAction(idSeries As Integer)
        If String.IsNullOrEmpty(SelOggetto.Text) Then
            AjaxAlert("Il campo Oggetto è obbligatorio")
            AjaxManager.ResponseScripts.Add("ResponseEnd();")
            Exit Sub
        End If

        If Not uscSelCategory.SelectedCategories.Any() Then
            AjaxAlert("E' necessario selezionare almeno un classificatore")
            AjaxManager.ResponseScripts.Add("ResponseEnd();")
            Exit Sub
        End If

        Dim currentSeries As DocumentSeries = Facade.DocumentSeriesFacade.GetById(idSeries)
        If currentSeries IsNot Nothing Then
            If Not Facade.ContainerFacade.CheckContainerRight(currentSeries.Container.Id, DSWEnvironment.DocumentSeries, DocumentSeriesContainerRightPositions.Draft, True) Then
                AjaxAlert("Non si hanno diritti di inserimento per la voce selezionata")
                AjaxManager.ResponseScripts.Add("ResponseEnd();")
                Exit Sub
            End If
        End If

        BindModelFromPage()
        Response.Redirect(String.Format("../Series/Item.aspx?{0}", CommonShared.AppendSecurityCheck(String.Format("Type=Series&Action={0}&IdSeries={1}", DocumentSeriesAction.FromResolutionKind.ToString(), idSeries))))
    End Sub

    Public Sub SelectRoleProposerFromCollaboration(roles As IList(Of Role))
        For Each role As Role In roles
            Dim node As RadTreeNode = New RadTreeNode()
            node.Text = role.Name
            node.Value = role.Id.ToString()
            node.Attributes.Add("ID", role.Id.ToString())
            node.ImageUrl = ImagePath.SmallRole

            rtvRoles.Nodes(0).Nodes.Add(node)
        Next
        rtvRoles.Nodes(0).Nodes(0).Selected = True

        ClientScript.RegisterStartupScript(Me.GetType(), "SelRoleProposer", OPEN_WINDOW_SCRIPT)
    End Sub

    Private Sub SetRoleProposerSelected(role As Role)
        uscRoleProposer.SourceRoles = New List(Of Role)() From {role}
        uscRoleProposer.DataBind()

        AjaxManager.ResponseScripts.Add("CloseWindowRoleProposer();")
    End Sub

    Private Sub InitDocumentsPrivacyLevels(showAlert As Boolean)
        Dim minLevel As Integer = 0
        Dim forceValue As Integer? = Nothing
        If Not String.IsNullOrEmpty(idContainer.SelectedValue) AndAlso DocSuiteContext.Current.PrivacyLevelsEnabled Then
            Dim container As Container = Facade.ContainerFacade.GetById(CInt(idContainer.SelectedValue))
            If (container IsNot Nothing) Then
                uscUploadDocumenti.ButtonPrivacyLevelVisible = container.PrivacyEnabled
                uscUploadAnnexed.ButtonPrivacyLevelVisible = container.PrivacyEnabled
                If PnlAllegatiVisible Then
                    uscUploadAttach.ButtonPrivacyLevelVisible = container.PrivacyEnabled
                End If
                forceValue = container.PrivacyLevel
                If container.PrivacyEnabled Then
                    forceValue = Nothing
                    Dim docs As List(Of DocumentInfo) = New List(Of DocumentInfo)(uscUploadDocumenti.DocumentInfosAdded)
                    docs.AddRange(uscUploadAnnexed.DocumentInfosAdded)
                    If PnlAllegatiVisible Then
                        docs.AddRange(uscUploadAttach.DocumentInfosAdded)
                    End If
                    If CurrentResolutionRights.IsPrivacyAttachmentAllowed Then
                        docs.AddRange(uscUploadPrivacyAttachment.DocumentInfosAdded)
                    End If
                    If Facade.DocumentFacade.CheckPrivacyLevel(docs, container) Then
                        forceValue = container.PrivacyLevel
                        If showAlert Then
                            AjaxAlert(String.Concat("Attenzione! Il livello di ", PRIVACY_LABEL, " del contenitore scelto è maggiore dei livelli attribuiti ai documenti. Ai documenti con livello di ", PRIVACY_LABEL, " minore, viene attribuito il livello del contenitore."))
                        End If
                    End If
                End If
            End If
            If container IsNot Nothing Then
                minLevel = container.PrivacyLevel
            End If

            uscUploadDocumenti.MinPrivacyLevel = minLevel
            uscUploadAnnexed.MinPrivacyLevel = minLevel
            uscUploadDocumenti.RefreshPrivacyLevelAttributes(minLevel, forceValue)
            uscUploadAnnexed.RefreshPrivacyLevelAttributes(minLevel, forceValue)
            If PnlAllegatiVisible Then
                uscUploadAttach.MinPrivacyLevel = minLevel
                uscUploadAttach.RefreshPrivacyLevelAttributes(minLevel, forceValue)
            End If
        End If
    End Sub

    Private Sub ResolutionInsertedDocumentPrivacyLevel(ByRef resl As Resolution, ByRef docs As List(Of BiblosDocumentInfo), Optional ByRef chainType As String = Nothing)
        Dim docType As String = "documento"
        If Not String.IsNullOrEmpty(chainType) Then
            Select Case chainType
                Case ResolutionFacade.DocType.Allegati.ToString()
                    docType = "allegato"
                    Exit Select
                Case ResolutionFacade.DocType.Annessi.ToString()
                    docType = "annesso"
                    Exit Select
                Case ResolutionFacade.DocType.AllegatiOmissis.ToString()
                    docType = "allegato (omesso)"
                    Exit Select
                Case ResolutionFacade.DocType.AllegatiRiservati.ToString()
                    docType = "allegato (riservato)"
                    Exit Select
                Case ResolutionFacade.DocType.DocumentoPrincipaleOmissis.ToString()
                    docType = "documento (omissis)"
                    Exit Select
            End Select
        End If
        For Each doc As BiblosDocumentInfo In docs
            If doc.Attributes.Any(Function(f) f.Key.Equals(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE)) Then
                FacadeFactory.Instance.ResolutionLogFacade.Insert(resl, ResolutionLogType.LP, String.Format("Associato livello privacy {0} al {1} {2} [{3}]", doc.Attributes.Item(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE), docType, doc.Name, doc.DocumentId))
            End If
        Next
    End Sub
#End Region

End Class