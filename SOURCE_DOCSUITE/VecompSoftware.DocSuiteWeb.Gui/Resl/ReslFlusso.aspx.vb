Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Facade.ExtensionMethods
Imports VecompSoftware.Helpers.PDF
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers
Imports VecompSoftware.Services.Sharepoint
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.DocSuiteWeb.DTO.Resolutions
Imports System.Globalization
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.NHibernateManager

Partial Public Class ReslFlusso
    Inherits ReslBasePage

#Region " Fields "
    Private Const FakeNodeValue As String = "meta"

    Private _reslType As Short
    Private _step As Short

    Private _roles As IList(Of Role)
    Private _fileResolution As FileResolution

    Private _pnlDocumentiOmissisVisible As Boolean = True
    Private _pnlAttachVisible As Boolean = True
    Private _pnlAttachOmissisVisible As Boolean = True
    Private _pnlAnnexesVisible As Boolean = True
    Private _pnlDocumentoVisible As Boolean = True

    Private _pnlPrivacyVisible As Boolean

    Private _numeroServizio As String
    Private _currentTypeDescription As String
    Private _resolutionDocumentSeriesItemFacade As Facade.NHibernate.Resolutions.ResolutionDocumentSeriesItemFacade
    Private _currentResolutionWPFacade As ResolutionWPFacade

#End Region

#Region " Properties "

    Private ReadOnly Property ReslType As Short
        Get
            Return CType(Request.QueryString("ReslType"), Short)
        End Get
    End Property

    Private ReadOnly Property [Step] As Short
        Get
            Return CType(Request.QueryString("Step"), Short)
        End Get
    End Property

    Private Property StepDescription As String
        Get
            Return CType(ViewState("StepDesc"), String)
        End Get
        Set(value As String)
            ViewState("StepDesc") = value
        End Set
    End Property

    Private ReadOnly Property NumeroServizio As String
        Get
            If String.IsNullOrEmpty(_numeroServizio) Then
                If txtNumeroServizio.Visible Then
                    ' Compongo il codice servizio + numero
                    Dim numero As String = txtNumeroServizio.Text.PadLeft(4, "0"c)
                    If ddlServizio.Visible Then
                        If Not ddlServizio.SelectedItem Is Nothing Then
                            Dim serviceCode As String = Facade.RoleFacade.GetById(CType(ddlServizio.SelectedValue, Integer)).ServiceCode.ToString()
                            _numeroServizio = String.Format("{0}/{1}", serviceCode, numero)
                        End If
                    Else
                        _numeroServizio = numero
                    End If
                Else
                    _numeroServizio = "N"
                End If
            End If
            Return _numeroServizio
        End Get
    End Property

    Private Property PnlDocumentiOmissisVisible() As Boolean
        Get
            Return _pnlDocumentiOmissisVisible
        End Get
        Set(ByVal value As Boolean)
            _pnlDocumentiOmissisVisible = value
            If value Then
                pnlDocumentiOmissis.Attributes.Remove("style")
            Else
                WebUtils.ObjAttDisplayNone(pnlDocumentiOmissis)
            End If
        End Set
    End Property

    Private Property PnlAttachVisible() As Boolean
        Get
            Return _pnlAttachVisible
        End Get
        Set(ByVal value As Boolean)
            _pnlAttachVisible = value
            If value Then
                pnlAttach.Attributes.Remove("style")
            Else
                WebUtils.ObjAttDisplayNone(pnlAttach)
            End If
        End Set
    End Property

    Private Property PnlAttachOmissisVisible() As Boolean
        Get
            Return _pnlAttachOmissisVisible
        End Get
        Set(ByVal value As Boolean)
            _pnlAttachOmissisVisible = value
            If value Then
                pnlAttachOmissis.Attributes.Remove("style")
            Else
                WebUtils.ObjAttDisplayNone(pnlAttachOmissis)
            End If
        End Set
    End Property

    Private Property PnlAnnexesVisible() As Boolean
        Get
            Return _pnlAnnexesVisible
        End Get
        Set(ByVal value As Boolean)
            _pnlAnnexesVisible = value
            If value Then
                pnlAnnexes.Attributes.Remove("style")
            Else
                WebUtils.ObjAttDisplayNone(pnlAnnexes)
            End If
        End Set
    End Property

    Private Property PnlDocumentoVisible() As Boolean
        Get
            Return _pnlDocumentoVisible
        End Get
        Set(ByVal value As Boolean)
            _pnlDocumentoVisible = value
            If value Then
                pnlDocumento.Attributes.Remove("style")
            Else
                WebUtils.ObjAttDisplayNone(pnlDocumento)
            End If
        End Set
    End Property

    Private Property PnlPrivacyAttachmentVisible() As Boolean
        Get
            Return pnlPrivacyAttachment.Visible
        End Get
        Set(ByVal value As Boolean)
            pnlPrivacyAttachment.Visible = value
        End Set
    End Property

    Private Property PnlPrivacyVisible() As Boolean
        Get
            Return _pnlPrivacyVisible
        End Get
        Set(ByVal value As Boolean)
            _pnlPrivacyVisible = value
            If value Then
                pnlPrivacy.Attributes.Remove("style")
            Else
                WebUtils.ObjAttDisplayNone(pnlPrivacy)
            End If
        End Set
    End Property

    Public ReadOnly Property CurrentTypeDescription As String
        Get
            If String.IsNullOrEmpty(_currentTypeDescription) Then
                _currentTypeDescription = Facade.ResolutionTypeFacade.GetDescription(CurrentResolution.Type.Id)
            End If
            Return _currentTypeDescription
        End Get
    End Property

    Public ReadOnly Property CurrentResolutionDocumentSeriesItemFacade As Facade.NHibernate.Resolutions.ResolutionDocumentSeriesItemFacade
        Get
            If _resolutionDocumentSeriesItemFacade Is Nothing Then
                _resolutionDocumentSeriesItemFacade = New Facade.NHibernate.Resolutions.ResolutionDocumentSeriesItemFacade()
            End If
            Return _resolutionDocumentSeriesItemFacade
        End Get
    End Property

    Private ReadOnly Property CurrentResolutionWPFacade As ResolutionWPFacade
        Get
            If _currentResolutionWPFacade Is Nothing Then
                _currentResolutionWPFacade = New ResolutionWPFacade()
            End If
            Return _currentResolutionWPFacade
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        MasterDocSuite.TitleVisible = False

        _reslType = ReslType
        _step = [Step]

        WebUtils.ObjAttDisplayNone(txtLastWorkflowDate)
        WebUtils.ObjAttDisplayNone(txtIdLocation)

        PnlDocumentiOmissisVisible = Not (pnlDocumentiOmissis.Attributes("style") = "display:none;")
        PnlAttachOmissisVisible = Not (pnlAttachOmissis.Attributes("style") = "display:none;")
        PnlAttachVisible = Not (pnlAttach.Attributes("style") = "display:none;")
        PnlAnnexesVisible = Not (pnlAnnexes.Attributes("style") = "display:none;")
        PnlDocumentoVisible = Not (pnlDocumento.Attributes("style") = "display:none;")
        SetOptionsVisibility(Not (pnlOptions.Attributes("style") = "display:none;"))
        PnlPrivacyVisible = Not (pnlPrivacy.Attributes("style") = "display:none;")

        InitializeAjax()

        'Configuro l'handler del pulsante conferma
        Select Case ResolutionEnv.Configuration
            Case ConfTo
                AddHandler btnConferma.Click, AddressOf BtnConfermaToClick
            Case Else
                AddHandler btnConferma.Click, AddressOf BtnConfermaClick
        End Select

        If Not Page.IsPostBack Then
            If Not Page.IsCallback AndAlso Not CommonUtil.VerifyChkQueryString(Request.QueryString, True) Then
                Exit Sub
            End If

            Initialize()
            InitializeDocumentControls()
        End If

        If DocSuiteContext.Current.PrivacyLevelsEnabled Then
            InitDocumentsPrivacyLevels()
        End If

        If Facade.ResolutionFacade.IsManagedProperty("ServiceNumber", CurrentResolution.Type.Id, "AUTOINC") Then
            AddHandler ddlServizio.SelectedIndexChanged, AddressOf DdlServizioSelectedIndexChanged
        End If
    End Sub

    Private Sub InitializeDocumentControls()
        uscUploadDocumenti.Caption = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.MainChain)
        uscUploadDocumenti.TreeViewCaption = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.MainChain)

        uscUploadDocumentiOmissis.Caption = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.MainOmissisChain)
        uscUploadDocumentiOmissis.TreeViewCaption = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.MainOmissisChain)

        uscUploadAllegati.Caption = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.AttachmentsChain)
        uscUploadAllegati.TreeViewCaption = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.AttachmentsChain)

        uscUploadAllegatiOmissis.Caption = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.AttachmentOmissisChain)
        uscUploadAllegatiOmissis.TreeViewCaption = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.AttachmentOmissisChain)

        uscUploadPrivacyAttachment.Caption = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.PrivacyAttachmentChain)
        uscUploadPrivacyAttachment.TreeViewCaption = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.PrivacyAttachmentChain)

        uscUploadAnnexes.Caption = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.AnnexedChain)
        uscUploadAnnexes.TreeViewCaption = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.AnnexedChain)

        documentUploader.Caption = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.MainChain)
        documentUploader.TreeViewCaption = ProtocolEnv.DocumentUnitLabels(Model.Entities.DocumentUnits.ChainType.MainChain)
    End Sub

    Private Sub uscUploadDocumenti_ButtonFrontespizioClick(ByVal sender As Object, ByVal e As EventArgs) Handles uscUploadDocumenti.ButtonFrontespizioClick
        ' Verifico che la data sia impostata.
        If Not txtData.SelectedDate.HasValue Then
            rfvData.Validate()
            Exit Sub
        End If
        Dim fi As FileInfo = ResolutionUtil.GeneraFrontalino(txtData.SelectedDate.Value, CurrentResolution, StepDescription, _step, String.Empty)
        Dim doc As New TempFileDocumentInfo(fi)
        Dim tmp As String() = fi.Name.Split("-"c)
        Dim fileName As String = tmp(tmp.Length - 1)
        doc.Name = fileName
        doc.AddAttribute(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE, 0)

        uscUploadDocumenti.LoadDocumentInfo(doc, False, False, True, True)
        ' Una volta generato il documento deve essere bloccato il campo data
        txtData.Enabled = Not ResolutionEnv.Configuration.Eq(ConfAuslPc)
        If ResolutionEnv.Configuration.Eq(ConfAuslPc) Then
            uscUploadDocumenti.ButtonFrontespizioEnabled = False
            uscUploadDocumenti.ButtonFileEnabled = False
            uscUploadDocumenti.ButtonRemoveEnabled = False
            uscUploadDocumenti.ButtonScannerEnabled = False
            uscUploadDocumenti.ButtonLibrarySharepointEnabled = False
        End If
    End Sub

    'todo: rivedere tutta la logica, non devono esistere metodi da 500 righe
    Private Sub BtnConfermaClick(ByVal sender As Object, ByVal e As EventArgs)
        If CurrentResolution Is Nothing Then
            Exit Sub
        End If

        _fileResolution = Facade.FileResolutionFacade.GetByResolution(CurrentResolution)(0)

        Dim idDocumento As Integer = -1
        Dim idDocumentiOmissis As Guid = Guid.Empty
        Dim idAllegati As Integer = -1
        Dim idAllegatiOmissis As Guid = Guid.Empty
        Dim idAnnexes As Guid = Guid.Empty
        Dim idPrivacyAttachment As Integer = -1

        Dim [date] As Date = Date.Now

        If txtData.SelectedDate.HasValue Then
            [date] = txtData.SelectedDate.Value
        End If

        ' Gestione numero di servizio
        If txtNumeroServizio.Visible Then

            Try
                If Not Facade.ResolutionFacade.CheckServiceNumber(CType([date].Year, Short), NumeroServizio, CurrentResolution.Id, CurrentResolution.Type.Id) Then
                    Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RX, String.Format("ERR.ATTI.STEP: Numero di Servizio {0} già utilizzato", NumeroServizio))
                    AjaxAlert("Attenzione! Il Numero di servizio è già stato utilizzato")
                    Exit Sub
                End If

                Dim sNum As Integer = Facade.ResolutionFacade.CheckPrevServiceNumberSequence(CurrentResolution.Id, CType([date].Year, Short), NumeroServizio, [date], CurrentResolution.Type.Id)

                If sNum <> -1 Then
                    Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RX, String.Format("ERR.ATTI.STEP: Numero di Servizio {0} minore di {1}", NumeroServizio, sNum))
                    AjaxAlert("Attenzione! Il Numero di servizio deve essere maggiore di {0}", sNum)
                    Exit Sub
                End If

                sNum = Facade.ResolutionFacade.CheckFollowingServiceNumberSequence(CurrentResolution.Id, CType([date].Year, Short), NumeroServizio, [date], CurrentResolution.Type.Id)

                If sNum <> -1 Then
                    Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RX, String.Format("ERR.ATTI.STEP: Numero di Servizio {0} maggiore di {1}", NumeroServizio, sNum))
                    AjaxAlert("Attenzione! Il Numero di servizio deve essere minore di {0}", sNum)
                    Exit Sub
                End If

            Catch ex As Exception
                FileLogger.Error(LoggerName, ex.Message, ex)
                Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RX, String.Format("ERR.ATTI.STEP: Errore generico nel controllo del Numero di Servizio: {0}", ex.Message))
                AjaxAlert(String.Format("Si è verificato un errore nel controllo del Numero di Servizio: {0}", ex.Message))
            End Try
        End If

        Dim b As Boolean
        ' Selezione Workflow
        Dim workStep As TabWorkflow = Nothing
        Select Case Action
            Case "Modify", "Delete"
                b = Facade.TabWorkflowFacade.GetByStep(CurrentResolution.WorkflowType, _step, workStep)
            Case "Next"
                b = Facade.TabWorkflowFacade.GetByStep(CurrentResolution.WorkflowType, _step + 1S, workStep)
                'EF 20120208 Pulitura del campo declineNote in quanto il messaggio è stato superato
                If b AndAlso ResolutionEnv.Configuration.Eq(ConfAuslPc) AndAlso CurrentResolution.DeclineNote IsNot Nothing Then
                    CurrentResolution.DeclineNote = Nothing
                End If
                'Lo recupero ora altrimenti viene cancellato se abilitato il parametro EnableFlushAnnexed
                idAnnexes = ReflectionHelper.GetPropertyCase(CurrentResolution.File, workStep.FieldAnnexed)
        End Select

        ' Segnalo che nello step di adozione i documenti devono essere tutti firmati
        If ResolutionEnv.MustResolutionAdoptionDocumentIsSigned AndAlso workStep.Description = WorkflowStep.ADOZIONE Then
            If DocumentMustSignAlert(uscUploadDocumenti.DocumentInfos()) Then
                Exit Sub
            End If
        End If

        If Not Action.Eq("Delete") AndAlso ddlServizio.Visible Then
            SetAdoptionRole(CurrentResolution, ddlServizio.SelectedValue)
        End If

        CheckAdoptionDate(Action, workStep, txtData)
        If CurrentResolution.Type.Id.Equals(ResolutionType.IdentifierDetermina) AndAlso NumeroServizio <> "N" Then
            If Not ValidateAdoptionDate(NumeroServizio, workStep.ManagedWorkflowData) Then
                FileLogger.Error(LoggerName, String.Format("E' stata inserita una data di Adozione antecedente alla data di Adozione dell'ultima {0} presente.", CurrentTypeDescription))
                AjaxAlert(String.Format("E' stata inserita una data di Adozione antecedente alla data di Adozione dell'ultima {0} presente.", CurrentTypeDescription))
                Exit Sub
            End If
        End If

        ' Resolution e ResolutionFile
        If Not b Then
            Exit Sub
        End If

        Dim howManyNextSteps As Integer = 0
        If Not Action.Eq("DELETE") Then
            Dim numServ As String = If(pnlNumeroServizio.Visible, NumeroServizio, "N")
            Dim dataFlusso As String = If(pnlData.Visible, txtData.SelectedDate.DefaultString(), "N")

            ' PER AUSLPC DEVO SALVARLO PRIMA DI CALCOLARE LA SIGNATURE E SALVARE IL FILE
            If ResolutionEnv.Configuration.Eq("AUSL-PC") Then
                b = Facade.ResolutionFacade.SqlResolutionUpdateWorkflowData(IdResolution, _reslType, Not CurrentResolution.Number.HasValue, workStep, dataFlusso, numServ, idAllegati, idDocumento, idAnnexes, "N", True, DocSuiteContext.Current.User.FullUserName)
            End If

            Dim append As Boolean = False
            If Not String.IsNullOrEmpty(workStep.BiblosFileProperty) Then
                append = StringHelper.InStrTest(workStep.BiblosFileProperty, "APP")
            End If

            If PnlDocumentoVisible And b Then
                idDocumento = CType((0 & ReflectionHelper.GetPropertyCase(CurrentResolution.File, workStep.FieldDocument)), Integer)

                ' se sono in un finto nodo riaggiungo per la modifica i documenti
                If uscUploadDocumenti.HasDocuments AndAlso uscUploadDocumenti.GetNodeValue(0).Eq(FakeNodeValue) Then
                    Dim app As Boolean = False
                    Dim errorType As ResolutionLogType = ResolutionLogType.RX
                    If workStep.Description.Eq(WorkflowStep.ADOZIONE) Then
                        app = True
                    End If

                    Try
                        DuplicaDocumento(idDocumento, uscUploadDocumenti, CurrentResolution, app, False, String.Empty)
                    Catch ex As Exception
                        Facade.ResolutionLogFacade.Log(CurrentResolution, errorType, String.Format("ERR.ATTI.STEP.{1}: Errore in fase di duplicazione: {0}", ex.Message, workStep.Description.ToUpper()))
                        AjaxAlert("ATTENZIONE!!{0} {1}", Environment.NewLine, ex.Message)
                        Exit Sub
                    End Try
                End If


                If Not Action.Eq("MODIFY") AndAlso (
                    workStep.Description.Eq(WorkflowStep.PUBBLICAZIONE) OrElse workStep.Description.Eq(WorkflowStep.RITIRO) OrElse (
                        ResolutionEnv.Configuration.Eq(ConfAuslPc) AndAlso (workStep.Description.Eq(WorkflowStep.ESECUTIVA) OrElse workStep.Description.Eq(WorkflowStep.FrontalinoEsecutiva))
                        )) AndAlso ResolutionEnv.CheckPublishDocumentSigned Then
                    If uscUploadDocumenti.HasDocuments AndAlso _fileResolution.IdProposalFile.HasValue Then
                        Dim fileProtoposta As New BiblosDocumentInfo(CurrentResolution.Location.ReslBiblosDSDB, _fileResolution.IdProposalFile.Value)
                        If fileProtoposta.IsSigned Then
                            Dim resolutionWorkflow As TabWorkflow = Facade.TabWorkflowFacade.GetByResolution(CurrentResolution.Id)
                            Facade.TabWorkflowFacade.GetByStep(
                                resolutionWorkflow.Id.WorkflowType,
                                resolutionWorkflow.Id.ResStep + 1S,
                                resolutionWorkflow)
                            Dim fieldDocumentProperty As Integer? = CType(ReflectionHelper.GetPropertyCase(_fileResolution, resolutionWorkflow.FieldDocument), Integer?)

                            Dim fileEffettivo As DocumentInfo
                            If fieldDocumentProperty IsNot Nothing Then
                                'Verifico se esista un file che è già stato caricato per il field document
                                fileEffettivo = New BiblosDocumentInfo(CurrentResolution.Location.ReslBiblosDSDB, fieldDocumentProperty.Value)
                            Else
                                'Se non c'è documento caricato effettuo il controllo legacy
                                fileEffettivo = uscUploadDocumenti.DocumentInfosAdded().First()
                            End If

                            If Not fileEffettivo.IsSigned Then
                                AjaxAlert("Il documento deve essere firmato")
                                Exit Sub
                            End If
                        End If
                    End If
                End If

                If (ResolutionEnv.WebPublishEnabled AndAlso (Not ResolutionEnv.Configuration.Eq(ConfTo)) AndAlso ResolutionEnv.WebAutoPublish) OrElse
                    (ResolutionEnv.ForceSharePointPublication AndAlso Not ResolutionEnv.PublishToOnlineRegisterEnabled) Then
                    ' Verifico che non sia già stato pubblicato
                    Try
                        ' Teoricamente per ASMN, ma avendo l'esecutività automatica, sharepoint viene chiamato direttamente dalla pagina di inserimento.
                        If workStep.Description.Eq(WorkflowStep.PUBBLICAZIONE) Then
                            b = PublishToSharePoint()
                        ElseIf workStep.Description.Eq(WorkflowStep.RITIRO) Then
                            b = RetireFromSharePoint()
                        End If
                    Catch ex As Exception
                        FileLogger.Error(LoggerName, "Errore Pubblicazione\Ritiro internet.", ex)
                        AjaxAlert("Errore Pubblicazione\Ritiro internet: " + ex.Message)
                        Exit Sub
                    End Try
                End If

                'Pubblicazione sul nuovo albo online
                If ResolutionEnv.PublishToOnlineRegisterEnabled AndAlso workStep.Description.Eq(WorkflowStep.PUBBLICAZIONE) Then
                    b = PublishToOnlineRegister()
                End If

                If Not b Then
                    Exit Sub
                End If

                idDocumento = InsertFileToBiblos(uscUploadDocumenti.DocumentInfosAdded, idDocumento, "D", append, workStep.Description, "")
                b = True
            End If

            ' Allegati
            If PnlAttachVisible Then
                idAllegati = 0 & ReflectionHelper.GetPropertyCase(CurrentResolution.File, workStep.FieldAttachment)

                ' se sono in un finto nodo riaggiungo per la modifica i documenti
                If uscUploadAllegati.HasDocuments AndAlso uscUploadAllegati.GetNodeValue(0).Eq(FakeNodeValue) Then
                    Dim app As Boolean = False
                    Dim errorType As ResolutionLogType = ResolutionLogType.RX
                    If workStep.Description.Eq(WorkflowStep.ADOZIONE) Then
                        app = True
                    End If

                    Try
                        DuplicaDocumento(idAllegati, uscUploadAllegati, CurrentResolution, app, False, String.Empty)
                        append = True
                    Catch ex As Exception
                        Facade.ResolutionLogFacade.Log(CurrentResolution, errorType, String.Format("ERR.ATTI.STEP.{1}: Errore in fase di duplicazione Allegati: {0}", ex.Message, workStep.Description.ToUpper()))
                        AjaxAlert("ATTENZIONE!!{0} {1}", Environment.NewLine, ex.Message)
                        Exit Sub
                    End Try
                End If

                idAllegati = InsertFileToBiblos(uscUploadAllegati.DocumentInfosAdded, idAllegati, "A", append, workStep.Description, String.Empty)
                b = True
            Else
                If workStep.Description = WorkflowStep.ADOZIONE AndAlso CurrentResolution.File.IdAttachements.HasValue Then
                    Dim attachments As IEnumerable(Of DocumentInfo) = BiblosDocumentInfo.GetDocuments(CurrentResolution.Location.ReslBiblosDSDB, CurrentResolution.File.IdAttachements.Value)
                    idAllegati = InsertFileToBiblos(attachments.ToList(), CurrentResolution.File.IdAttachements.Value, "A", append, workStep.Description, String.Empty)
                    b = True
                End If
            End If

            ' Annessi
            If PnlAnnexesVisible Then
                idAnnexes = ReflectionHelper.GetPropertyCase(CurrentResolution.File, workStep.FieldAnnexed)
                ' se sono in un finto nodo riaggiungo per la modifica i documenti
                If uscUploadAnnexes.HasDocuments AndAlso uscUploadAnnexes.GetNodeValue(0).Eq(FakeNodeValue) Then
                    Dim errorType As ResolutionLogType = ResolutionLogType.RX

                    Try
                        DuplicaDocumento(idAnnexes, uscUploadAnnexes, CurrentResolution, True, False, String.Empty)
                        append = True
                    Catch ex As Exception
                        Facade.ResolutionLogFacade.Log(CurrentResolution, errorType, String.Format("ERR.ATTI.STEP.{1}: Errore in fase di duplicazione Annessi: {0}", ex.Message, workStep.Description.ToUpper()))
                        AjaxAlert("ATTENZIONE!!{0} {1}", Environment.NewLine, ex.Message)
                        Exit Sub
                    End Try
                End If

                idAnnexes = InsertFileToBiblosWithGuid(uscUploadAnnexes.DocumentInfosAdded, idAnnexes, "AN", append, workStep.Description, "")
                b = True
            Else
                If workStep.Description = WorkflowStep.ADOZIONE AndAlso Not CurrentResolution.File.IdAnnexes.Equals(Guid.Empty) Then
                    Dim annexed As IEnumerable(Of DocumentInfo) = BiblosDocumentInfo.GetDocuments(CurrentResolution.File.IdAnnexes)
                    idAnnexes = InsertFileToBiblosWithGuid(annexed.ToList(), CurrentResolution.File.IdAnnexes, "AN", append, workStep.Description, String.Empty)
                    b = True
                End If
            End If

            If PnlPrivacyAttachmentVisible Then
                idPrivacyAttachment = 0 & ReflectionHelper.GetPropertyCase(CurrentResolution.File, workStep.FieldPrivacyAttachment)
                If workStep.Description.Eq(WorkflowStep.ADOZIONE) AndAlso uscUploadPrivacyAttachment.HasDocuments Then
                    ' se sono in un finto nodo riaggiungo per la modifica i documenti
                    If uscUploadPrivacyAttachment.GetNodeValue(0).Eq(FakeNodeValue) Then
                        Try
                            DuplicaDocumento(idPrivacyAttachment, uscUploadPrivacyAttachment, CurrentResolution, True, False, String.Empty)
                            append = True
                        Catch ex As Exception
                            Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RX, String.Format("ERR.ATTI.STEP.ADOZIONE: Errore in fase di duplicazione: {0}", ex.Message))
                            AjaxAlert("ATTENZIONE!!{0} {1}", Environment.NewLine, ex.Message)
                            Exit Sub
                        End Try
                    End If
                End If

                idPrivacyAttachment = InsertFileToBiblos(uscUploadPrivacyAttachment.DocumentInfosAdded, idPrivacyAttachment, "AR", append, workStep.Description, "")
                Facade.ResolutionFacade.SqlResolutionDocumentUpdate(CurrentResolution.Id, idPrivacyAttachment, ResolutionFacade.DocType.AllegatiRiservati)
                b = True
            End If

            If Action.Eq("NEXT") Then
                'Creazione automatica del frontalino per AUSL-PC
                If ResolutionEnv.GenerateFrontalinoInAdoptionState AndAlso ResolutionEnv.Configuration.Eq("AUSL-PC") AndAlso CurrentResolution.WorkflowType = Facade.TabMasterFacade.GetFieldValue("WorkflowType", "AUSL-PC", _reslType) AndAlso workStep.Description.Eq(WorkflowStep.ADOZIONE) Then
                    Dim number As String = String.Empty
                    If Not String.IsNullOrEmpty(txtNumeroServizio.Text) Then
                        number = txtNumeroServizio.Text
                    End If
                    If (Facade.ResolutionFacade.IsManagedProperty("ServiceNumber", CurrentResolution.Type.Id)) Then
                        number = String.Empty
                    End If

                    Dim locationFacade As New LocationFacade("ReslDB")
                    Dim location As Location = locationFacade.GetById(CInt(txtIdLocation.Text))
                    Dim idCatenaFrontalino As Integer
                    Dim presentSigners As IList(Of String) = Nothing
                    Dim collaborationResl As Collaboration = FacadeFactory.Instance.CollaborationFacade.GetByResolution(CurrentResolution)
                    If Not collaborationResl Is Nothing AndAlso ResolutionEnv.DelibereSignsReportEnabled Then
                        presentSigners = Facade.ResolutionFacade.GetPresentDocumentSigners(CurrentResolution, workStep, idDocumento)
                    End If
                    ResolutionUtil.GetInstance().InserisciFrontalino(CType(txtData.SelectedDate, Date), IdResolution, _reslType, workStep.Description, location, idCatenaFrontalino, number, _step, ddlServizio.SelectedValue, presentSigners)
                End If

                ''Se il documento principale non è stato caricato, allora lo carico
                If (_fileResolution.IdResolutionFile Is Nothing AndAlso DocSuiteContext.Current.ResolutionEnv.ForceSharePointPublication AndAlso workStep.Description.Eq(WorkflowStep.ADOZIONE)) Then
                    Dim idResolutionFileDocumentInfo As DocumentInfo = New BiblosDocumentInfo(CurrentResolution.Location.ReslBiblosDSDB, _fileResolution.IdProposalFile.Value)
                    idResolutionFileDocumentInfo.Signature = GetSignature("D", workStep.Description)
                    Dim biblosDoc As BiblosDocumentInfo = idResolutionFileDocumentInfo.ArchiveInBiblos(CurrentResolution.Location.ReslBiblosDSDB)
                    idResolutionFileDocumentInfo = biblosDoc

                    If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                        Facade.ResolutionFacade.ResolutionInsertedDocumentPrivacyLevel(CurrentResolution, biblosDoc, "D")
                    End If

                    Facade.ResolutionFacade.SqlResolutionDocumentUpdate(CurrentResolution.Id, CType(idResolutionFileDocumentInfo, BiblosDocumentInfo).BiblosChainId, ResolutionFacade.DocType.Disposizione)
                End If

                'Accodamento
                If b AndAlso StringHelper.InStrTest(workStep.ManagedWorkflowData, "ComposeDoc") Then
                    Dim workStepCopy As TabWorkflow = CType(workStep.Clone(), TabWorkflow)
                    Dim fieldDocumentBackup As String = workStepCopy.FieldDocument
                    If ResolutionEnv.Configuration.Eq(ConfAuslPc) Then
                        '' Inserisce il frontespizio corrente
                        b = Facade.ResolutionFacade.SqlResolutionUpdateWorkflowData(IdResolution, _reslType, Not CurrentResolution.Number.HasValue, workStepCopy, dataFlusso, numServ, idAllegati, idDocumento, idAnnexes, "N", True, DocSuiteContext.Current.User.FullUserName)
                        'EF 20120216 Workaround per forzare il caricamento della catena di documenti in idResolutionFile e gli altri frontalini dove previsto (per AUSL-PC)
                        ' Salvo i vecchi campi
                        fieldDocumentBackup = workStepCopy.FieldDocument
                        workStepCopy.FieldDocument = "idResolutionFile"
                    End If
                    Dim signature As String = Facade.ResolutionFacade.SqlResolutionGetNumber(IdResolution, , , , True)
                    ResolutionFacade.ComposeDocument(workStepCopy.ManagedWorkflowData, _fileResolution, idDocumento, CurrentResolution.Location, signature)
                    ' Ripristino i campi originali
                    workStepCopy.FieldDocument = fieldDocumentBackup
                    If Not b Then
                        AjaxAlert("ATTENZIONE{0}L'accodamento del documento non è andata a buon fine.{0}Ripetere l'operazione.", Environment.NewLine)
                    End If
                End If
            End If

            ' Aggiorno il workflow resolution
            If b Then
                b = Facade.ResolutionFacade.SqlResolutionUpdateWorkflowData(IdResolution, _reslType, Not CurrentResolution.Number.HasValue, workStep, dataFlusso, numServ, idAllegati, idDocumento, idAnnexes, "N", True, DocSuiteContext.Current.User.FullUserName)
            End If

            If Action.Eq("NEXT") Then
                ' Verifico se devo salvare anche lo step successivo
                ' Bisognerebbe rifare la stessa identica procedura dall'inizio
                ' Siccome è solo un caso (ASMN - Pubblicazione -> Esecutività) per ora va bene così... alla grande!!!
                Dim lastWorkStep As TabWorkflow = workStep
                Dim nextStepId As Short = _step + 2S
                While StringHelper.InStrTest(lastWorkStep.ManagedWorkflowData, "NextStep") And b
                    ' Aggiunto per compatibilità tra AUSL-PC e ASMN, in questo modo il valore di default è preservato per ASMN, mentre per AUSL-PC è possibile ottenere il caricamento del file.
                    idDocumento = -1
                    howManyNextSteps = howManyNextSteps + 1
                    Dim workNextStep As TabWorkflow = Nothing
                    ' Recupero il nuovo step
                    b = Facade.TabWorkflowFacade.GetByStep(CurrentResolution.WorkflowType, nextStepId, workNextStep)
                    If b Then
                        Dim data As DateTime = ReflectionHelper.GetProperty(CurrentResolution, lastWorkStep.FieldDate)
                        Dim temp As String = If(pnlData.Visible, String.Format("{0:dd/MM/yyyy}", data), "N")
                        Select Case ResolutionEnv.Configuration
                            Case ConfAuslPc
                                If StringHelper.InStrTest(workNextStep.ManagedWorkflowData, "ComposeDoc") And b Then
                                    Dim signature As String = Facade.ResolutionFacade.SqlResolutionGetNumber(IdResolution, , , , True)
                                    ResolutionFacade.ComposeDocument(workNextStep.ManagedWorkflowData, _fileResolution, idDocumento, CurrentResolution.Location, signature)
                                End If
                        End Select

                        b = Facade.ResolutionFacade.SqlResolutionUpdateWorkflowData(IdResolution, _reslType, Not CurrentResolution.Number.HasValue, workNextStep, temp, numServ, -1, idDocumento, idAnnexes, "N", True, DocSuiteContext.Current.User.FullUserName)
                    End If
                    lastWorkStep = workNextStep
                    nextStepId = nextStepId + 1S
                End While
            End If
        Else
            If b AndAlso (ResolutionEnv.Configuration.Eq(ConfAuslPc) AndAlso workStep.Description.Eq(WorkflowStep.PUBBLICAZIONE)) Then
                ' Rimozione frontalino di pubblicazione vecchio
                Facade.ResolutionFacade.SqlResolutionFileUpdate(IdResolution, 0, workStep.FieldDocument)
            End If
            If b Then
                b = Facade.ResolutionFacade.SqlResolutionDeleteStep(IdResolution, workStep)
            End If
        End If

        ' Salvataggio ResolutionWorkflow
        If Not b Then
            Exit Sub
        End If
        ' Allegati e Documento:
        ' Se non ho salvato su ResolutionFile (se id = -1)
        ' Rileggo il valore originale se esiste

        'Documento Principale
        If idDocumento.Equals(-1) Then
            If Not String.IsNullOrEmpty(workStep.FieldDocument) Then
                idDocumento = 0 & ReflectionHelper.GetPropertyCase(CurrentResolution, workStep.FieldDocument)
            Else
                idDocumento = 0
            End If
        End If

        ' Documenti Omissis
        If idDocumentiOmissis.Equals(Guid.Empty) Then
            If Not String.IsNullOrEmpty(workStep.FieldAnnexed) Then
                idDocumentiOmissis = ReflectionHelper.GetPropertyCase(CurrentResolution, workStep.FieldDocumentsOmissis)
            Else
                idDocumentiOmissis = Guid.Empty
            End If
        End If

        'Allegati
        If idAllegati.Equals(-1) Then
            If Not String.IsNullOrEmpty(workStep.FieldAttachment) Then
                idAllegati = 0 & ReflectionHelper.GetPropertyCase(CurrentResolution, workStep.FieldAttachment)
            Else
                idAllegati = 0
            End If
        End If

        ' Documenti Omissis
        If idAllegatiOmissis.Equals(Guid.Empty) Then
            If Not String.IsNullOrEmpty(workStep.FieldAnnexed) Then
                idAllegatiOmissis = ReflectionHelper.GetPropertyCase(CurrentResolution, workStep.FieldAttachmentsOmissis)
            Else
                idAllegatiOmissis = Guid.Empty
            End If
        End If

        ' Annessi
        If idAnnexes.Equals(Guid.Empty) Then
            If Not String.IsNullOrEmpty(workStep.FieldAnnexed) Then
                idAnnexes = ReflectionHelper.GetPropertyCase(CurrentResolution, workStep.FieldAnnexed)
            Else
                idAnnexes = Guid.Empty
            End If
        End If

        ' Allegati Riservati
        If idPrivacyAttachment.Equals(-1) Then
            If Not String.IsNullOrEmpty(workStep.FieldPrivacyAttachment) Then
                idPrivacyAttachment = 0 & ReflectionHelper.GetPropertyCase(CurrentResolution, workStep.FieldPrivacyAttachment)
            Else
                idPrivacyAttachment = 0
            End If
        End If

        ' Settori
        If workStep.ExistWorkflowData(TabWorkflow.WorkflowField.Role) Then
            Dim roleId As Integer = workStep.ExtractoWorkflowData(Of Integer)(TabWorkflow.WorkflowField.Role)
            Facade.ResolutionRoleFacade.AddRole(CurrentResolution, roleId, DocSuiteContext.Current.ResolutionEnv.DefaultResolutionRoleType)
        End If

        Select Case Action
            Case "Modify" 'Salvataggio in ResolutionWorkflow
                b = Facade.ResolutionWorkflowFacade.SqlResolutionWorkflowUpdate(IdResolution, idDocumento, idAllegati, idPrivacyAttachment, idAnnexes, idDocumentiOmissis, idAllegatiOmissis)
            Case "Next" 'Inserimento in ResolutionWorkflow
                b = Facade.ResolutionWorkflowFacade.InsertNextStep(IdResolution, _step, idDocumento, idAllegati, idPrivacyAttachment, idAnnexes, idDocumentiOmissis, idAllegatiOmissis, DocSuiteContext.Current.User.FullUserName)

                For index As Short = 1 To howManyNextSteps
                    b = Facade.ResolutionWorkflowFacade.InsertNextStep(IdResolution, _step + index, idDocumento, idAllegati, idPrivacyAttachment, idAnnexes, idDocumentiOmissis, idAllegatiOmissis, DocSuiteContext.Current.User.FullUserName)
                Next

            Case "Delete"
                b = Facade.ResolutionWorkflowFacade.EnablePreviousStep(IdResolution, _step)

                If ResolutionEnv.WebPublishEnabled OrElse ResolutionEnv.ForceSharePointPublication Then

                    If workStep.Description.Eq(WorkflowStep.PUBBLICAZIONE) Then
                        DeleteFromSharePoint(True)
                    ElseIf workStep.Description.Eq(WorkflowStep.RITIRO) Then
                        DeleteFromSharePoint(False)
                    End If

                End If
                If (ResolutionEnv.Configuration = ConfAuslPc) Then 'EF 20120208 Nel caso di AUSL-PC salvo la motivazione di ritorno in DeclineNote Motivazione|Step
                    Dim dataFlusso As String = String.Format("{0:dd/MM/yyyy}", txtData.SelectedDate)
                    ''DeclineNote è formato da:
                    ''0 Messaggio
                    ''1 Numero dello step che viene arretrato
                    ''2 Nome dello step che viene arretrato
                    ''3 Data di arretramento
                    CurrentResolution.DeclineNote = String.Format("{0}§{1}§{2}§{3}", txtMotivazione.Text, _step, workStep.CustomDescription, dataFlusso)
                End If
                Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RU, String.Format("Annullamento {0}. Motivazione: {1}", workStep.CustomDescription, txtMotivazione.Text.Split("§"c)(0)))
        End Select

        If Not b Then
            Exit Sub
        End If

        'Invio comando di creazione/aggiornamento Resolution alle WebApi
        If Not Action.Eq("DELETE") AndAlso Not workStep.Description.Eq(WorkflowStep.PROPOSTA) Then
            If Action.Eq("NEXT") AndAlso workStep.Description.Eq(WorkflowStep.ADOZIONE) Then
                Facade.ResolutionFacade.SendCreateResolutionCommand(CurrentResolution)
            Else
                Facade.ResolutionFacade.SendUpdateResolutionCommand(CurrentResolution)
            End If
        End If

        ' Se sto andando nello stato di pubblicazione, devo automaticamente Confermare le serie documentali senza documento richiesto
        If Action.Eq("NEXT") AndAlso workStep.Description.Eq(WorkflowStep.PUBBLICAZIONE) Then
            CurrentResolutionDocumentSeriesItemFacade.ConfirmAndPublishSeries(CurrentResolution)
        End If

        AjaxManager.ResponseScripts.Add("return CloseWindow();")
    End Sub

    ''' <summary> Archivia i documenti principali e qualora sia prevista la gestione digitale gli eventuali frontalini. </summary>
    ''' <param name="workStep"> Step di avanzamento del workflow </param>
    ''' <remarks> FG20140707: Questo è un primo tentativo di scorporare e semplificare alcune funzionalità contenute in BtnConfermaToClick </remarks>
    Private Sub ArchiveDocumentAndGetFrontespizi(workStep As TabWorkflow, ByRef idDocumento As Integer?, ByRef frontespizi As List(Of BiblosDocumentInfo))
        If Not PnlDocumentoVisible Then
            Return
        End If

        Dim gestioneDigitale As Boolean = DocSuiteContext.Current.ResolutionEnv.GestioneDigitale(Me.CurrentResolution)
        Dim addedDocuments As IList(Of DocumentInfo) = Me.uscUploadDocumenti.DocumentInfosAdded

        If gestioneDigitale AndAlso workStep.Description.Eq("ADOZIONE") _
            AndAlso Not addedDocuments.IsNullOrEmpty() _
            AndAlso _fileResolution IsNot Nothing AndAlso _fileResolution.IdFrontespizio.HasValue Then
            frontespizi = BiblosDocumentInfo.GetDocuments(Me.CurrentResolution.Location.ReslBiblosDSDB, Me._fileResolution.IdFrontespizio.Value)

            addedDocuments.Insert(0, frontespizi.First())
            FacadeFactory.Instance.ResolutionLogFacade.Log(Me.CurrentResolution, ResolutionLogType.RP, "Frontalino aggiunto automaticamente.")
        End If

        idDocumento = CType(ReflectionHelper.GetPropertyCase(_fileResolution, workStep.FieldDocument), Integer?)

        If Not addedDocuments.IsNullOrEmpty() OrElse Not Me.uscUploadDocumenti.GetNodeValue(0).Eq(Me.FakeNodeValue) Then
            idDocumento = Me.InsertFileToBiblos(addedDocuments, idDocumento.GetValueOrDefault(0), "D", uscUploadDocumenti.MultipleDocuments, workStep.Description, String.Empty)
        End If
    End Sub

    Private Sub BtnConfermaToClick(ByVal sender As Object, ByVal e As System.EventArgs)
        If CurrentResolution Is Nothing Then
            Exit Sub
        End If

        If DocSuiteContext.Current.ResolutionEnv.AutomaticActivityStepEnabled AndAlso pnlAutomatismi.Visible = True Then
            Dim nodes As RadTreeNodeCollection = rtvListDocument.Nodes(0).Nodes
            If Not IsOneDocumentChecked(nodes) Then
                AjaxAlert("Selezionare almeno un documento.")
                Exit Sub
            End If
        End If

        _fileResolution = FacadeFactory.Instance.FileResolutionFacade.GetByResolution(CurrentResolution)(0)

        Dim archive As Boolean = radioFrontalino.SelectedValue.Eq("frontalinodigitale") OrElse (_fileResolution IsNot Nothing AndAlso _fileResolution.IdFrontespizio.HasValue)

        Dim stepExists As Boolean
        Dim workStep As TabWorkflow = Nothing

        'Selezione Workflow
        Select Case Action
            Case "Modify", "Delete"
                stepExists = FacadeFactory.Instance.TabWorkflowFacade.GetByStep(CurrentResolution.WorkflowType, _step, workStep)
            Case "Next"
                stepExists = FacadeFactory.Instance.TabWorkflowFacade.GetByStep(CurrentResolution.WorkflowType, _step + 1S, workStep)
                If CurrentResolution.DeclineNote IsNot Nothing Then
                    CurrentResolution.DeclineNote = Nothing
                End If
        End Select

        ' Segnalo che nello step di adozione i documenti devono essere tutti firmati
        If ResolutionEnv.MustResolutionAdoptionDocumentIsSigned AndAlso workStep.Description = WorkflowStep.ADOZIONE Then
            If DocumentMustSignAlert(uscUploadDocumenti.DocumentInfos()) Then
                Exit Sub
            End If
        End If

        CheckAdoptionDate(Action, workStep, txtData)
        If CurrentResolution.Type.Id.Equals(ResolutionType.IdentifierDetermina) Then
            If Not ValidateAdoptionDate(String.Empty, workStep.ManagedWorkflowData) Then
                FileLogger.Error(LoggerName, String.Format("E' stata inserita una data di Adozione antecedente alla data di Adozione dell'ultima {0} presente.", CurrentTypeDescription))
                AjaxAlert(String.Format("E' stata inserita una data di Adozione antecedente alla data di Adozione dell'ultima {0} presente.", CurrentTypeDescription))
                Exit Sub
            End If
        End If

        ' nothing = non viene toccato
        ' 0 o Guid.Empty = va cancellato
        ' altro = aggiornare
        Dim idDocumento As Integer? = Nothing
        Dim idDocumentiOmissis As Guid? = Nothing
        Dim idAllegati As Integer? = Nothing
        Dim idAllegatiOmissis As Guid? = Nothing
        Dim idAnnessi As Guid? = Nothing
        Dim idPrivacyAttachment As Integer? = Nothing
        Dim frontalini As ICollection(Of ResolutionFrontispiece) = New List(Of ResolutionFrontispiece)

        'Resolution e ResolutionFile
        If stepExists Then
            If Not Action.Eq("DELETE") Then
                Dim numServ As String = If(pnlNumeroServizio.Visible, txtNumeroServizio.Text, "N")
                Dim dataFlusso As String = If(pnlData.Visible, txtData.SelectedDate.DefaultString, "N")
                ' Inibisco l'aggiornamento di utente e data del passo se la data viene mostrata bloccata e mi trovo in uno step di modifica
                If Not txtData.Enabled AndAlso Action.Eq("MODIFY") Then
                    dataFlusso = "N"
                End If

                ' Se sono in fase di adozione predispongo il frontalino
                If ResolutionEnv.GestioneDigitale(CurrentResolution) AndAlso (workStep.Description.Eq(WorkflowStep.ADOZIONE)) Then
                    ' Genero il frontalino e lo archivio se selezionato per l'archiviazione
                    Dim printer As New ReslFrontalinoPrintPdfTO()
                    Dim adoptionDate As DateTime = DateTime.ParseExact(dataFlusso, "dd/MM/yyyy", CultureInfo.CurrentCulture)

                    'NB: la generazione del frontalino di adozione è stata spostata prima che avvenga l'update dell'atto con anno, numero, data adozione ecc,
                    '    in modo che se si blocca o va in errore la generazione del frontalino, l'atto non vada in adozione.
                    '    A questo punto anno e numero dell'atto non sono ancora stati generati, ma servono per calcolare la segnatura del frontalino.
                    Try
                        Dim year As Short = 0
                        Dim number As Integer = 0
                        If CurrentResolution.Number.HasValue AndAlso CurrentResolution.Year.HasValue Then
                            year = CurrentResolution.Year.Value
                            number = CurrentResolution.Number.Value
                        Else
                            Dim yearAndnumber As Tuple(Of Short, Integer) = Facade.ResolutionFacade.CalculateYearAndNumber(CurrentResolution.ProposeDate.Value, adoptionDate, _reslType)
                            year = yearAndnumber.Item1
                            number = yearAndnumber.Item2
                        End If

                        Dim frontispieceResolution As Resolution = DirectCast(CurrentResolution.Clone(), Resolution)
                        frontispieceResolution.Year = year
                        frontispieceResolution.Number = number
                        frontispieceResolution.AdoptionDate = adoptionDate
                        frontispieceResolution.Container = DirectCast(CurrentResolution.Container.Clone(), Container)
                        Dim reslContacts As IList(Of ResolutionContact) = New List(Of ResolutionContact)()
                        For Each resolutionContactProposer As ResolutionContact In CurrentResolution.ResolutionContactProposers
                            reslContacts.Add(New ResolutionContact() With {.Contact = DirectCast(resolutionContactProposer.Contact.Clone(), Contact)})
                        Next
                        frontispieceResolution.ResolutionContactProposers = reslContacts
                        If Not numServ.Eq("N") AndAlso Not String.IsNullOrEmpty(numServ) Then
                            frontispieceResolution.ServiceNumber = numServ
                        End If
                        Dim signature As String = GetSignature("F", workStep.Description, frontispieceResolution)

                        frontalini = printer.GeneraFrontalini(frontispieceResolution)
                        'Salvataggio dei frontalini in biblos e update di fileresolution
                        If archive Then
                            printer.SaveBiblosFrontispieces(frontalini, CurrentResolution, signature)
                        End If
                    Catch ex As Exception
                        FileLogger.Error(LoggerName, String.Format("Errore nella generazione dei file di frontespizio da Sommario: {0}", ex.Message), ex)
                        Facade.ResolutionLogFacade.Insert(CurrentResolution, ResolutionLogType.RF, "Errore nella generazione dei file di frontespizio da Sommario.")
                        AjaxAlert(String.Format("Errore nella generazione dei file di frontespizio. L'atto è ancora in stato di proposta, riprovare l'avanzamento di step."))
                        Exit Sub
                    End Try

                    If archive Then
                        Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RP, "Frontalino predisposto per gestione digitale")
                    Else
                        Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RP, "Frontalino creato per stampa")
                    End If
                End If

                ' Salvo le modifiche all'Atto, nessuna modifica ai documenti
                stepExists = Facade.ResolutionFacade.SqlResolutionUpdateWorkflowData(IdResolution, _reslType, Not CurrentResolution.Number.HasValue, workStep, dataFlusso, numServ, -1, -1, Guid.Empty, "N", False, DocSuiteContext.Current.User.FullUserName)

                'Se i valori sono cambiati significa che l'atto è stato appena adottato
                Dim hadJustBeenAdopted As Boolean = _fileResolution IsNot Nothing AndAlso _fileResolution.IdFrontespizio.HasValue

                '' TORINO: dopo l'adozione il frontalino non è più necessario
                If ResolutionEnv.GestioneDigitale(CurrentResolution) AndAlso (workStep.Description.Eq(WorkflowStep.PUBBLICAZIONE) OrElse workStep.Description.Eq(WorkflowStep.ESECUTIVA)) Then
                    If _fileResolution IsNot Nothing AndAlso _fileResolution.IdFrontespizio.HasValue Then
                        _fileResolution.IdFrontespizio = Nothing
                        Facade.FileResolutionFacade.Save(_fileResolution)
                        Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RP, "Frontalino rimosso dopo Adozione.")
                    End If
                End If

                ' Documenti
                Dim listaFrontalini As List(Of BiblosDocumentInfo) = Nothing
                If stepExists Then
                    Me.ArchiveDocumentAndGetFrontespizi(workStep, idDocumento, listaFrontalini)
                End If

                ' Documenti Omissis
                If PnlDocumentiOmissisVisible Then
                    idDocumentiOmissis = CType(ReflectionHelper.GetPropertyCase(_fileResolution, workStep.FieldDocumentsOmissis), Guid)

                    Dim addedDocuments As IList(Of DocumentInfo) = uscUploadDocumentiOmissis.DocumentInfosAdded
                    ' Se la lista ha un valore significa che è stata inizializzata
                    ' e che pertanto deve essere utilizzata
                    If listaFrontalini IsNot Nothing AndAlso CurrentResolution.Container.Privacy Then
                        'Devo aggiungere alla catena degli omissis il frontalino privacy
                        'Solo nel contesto privacy
                        If listaFrontalini.Count > 1 Then
                            ' se posso inserire il secondo frontalino [si.. purtroppo il secondo frontalino è quello privacy]
                            addedDocuments.Insert(0, listaFrontalini(1))
                        Else
                            ' altrimenti lancio un errore [e scrivo nel log]
                            FileLogger.Error(LoggerName, String.Format("Impossibile accodare il frontalino privacy nella resolution [id {0}] in quanto, nonostante sia abilitata la privacy sul contenitore, non è arrivato il secondo frontalino.", CurrentResolution.Id))
                            AjaxAlert(String.Format("ATTENZIONE{0}{0}Impossibile accodare il Frontalino.{0}{0}Frontalino Privacy non trovato.", Environment.NewLine))
                        End If
                    End If

                    If Not addedDocuments.IsNullOrEmpty OrElse Not uscUploadDocumentiOmissis.GetNodeValue(0).Eq(FakeNodeValue) Then
                        idDocumentiOmissis = InsertFileToBiblosWithGuid(addedDocuments, Guid.Empty, "DO", True, workStep.Description, "")
                    End If

                    stepExists = True
                End If

                ' Allegati
                If PnlAttachVisible Then
                    idAllegati = CType(ReflectionHelper.GetPropertyCase(_fileResolution, workStep.FieldAttachment), Integer)

                    Dim addedDocuments As IList(Of DocumentInfo) = uscUploadAllegati.DocumentInfosAdded
                    If Not addedDocuments.IsNullOrEmpty OrElse Not uscUploadAllegati.GetNodeValue(0).Eq(FakeNodeValue) Then
                        idAllegati = InsertFileToBiblos(addedDocuments, 0, "A", False, workStep.Description, "")
                    End If

                    stepExists = True
                End If

                ' Allegati Omissis
                If PnlAttachOmissisVisible Then
                    idAllegatiOmissis = CType(ReflectionHelper.GetPropertyCase(_fileResolution, workStep.FieldAttachmentsOmissis), Guid)

                    Dim addedDocuments As IList(Of DocumentInfo) = uscUploadAllegatiOmissis.DocumentInfosAdded
                    If Not addedDocuments.IsNullOrEmpty OrElse Not uscUploadAllegatiOmissis.GetNodeValue(0).Eq(FakeNodeValue) Then
                        idAllegatiOmissis = InsertFileToBiblosWithGuid(addedDocuments, Guid.Empty, "AO", True, workStep.Description, "")
                    End If

                    stepExists = True
                End If

                'Annessi
                If PnlAnnexesVisible Then
                    ' TODO: capire perchè non possono essere cancellati sarebbe bello
                    idAnnessi = CType(ReflectionHelper.GetPropertyCase(_fileResolution, workStep.FieldAnnexed), Guid)
                    Dim oldIdAnnessi As Guid = idAnnessi.Value
                    Dim addedDocuments As IList(Of DocumentInfo) = uscUploadAnnexes.DocumentInfosAdded
                    If ResolutionEnv.GestioneDigitale(CurrentResolution) Then
                        If CurrentResolution.Container.Privacy.Value = 1 Then
                            'Se ho il frontalino
                            If _fileResolution IsNot Nothing AndAlso _fileResolution.IdFrontespizio.HasValue Then
                                ' Se ci sono documenti (anche fittizi)
                                If uscUploadAnnexes.DocumentsCount > 0 Then
                                    ' Se ha solo nuovi documenti
                                    If HasOnlyRealDocuments(uscUploadAnnexes.TreeViewControl) OrElse hadJustBeenAdopted Then
                                        '' Se il primo nodo non ha value vuol dire che gli annessi ci sono già, quindi devo fare il merge
                                        If (uscUploadAnnexes.TreeViewControl.Nodes(0).Nodes(0).Value = "") Then
                                            oldIdAnnessi = Guid.Empty

                                            ' Recupero il Frontalino Privacy
                                            Dim doc As New BiblosDocumentInfo(CurrentResolution.Location.ReslBiblosDSDB, _fileResolution.IdFrontespizio.Value)
                                            addedDocuments.Insert(0, doc)
                                        End If
                                    End If
                                Else
                                    ' Se non ci sono nodi vuol dire che voglio buttare via il backup (ammesso che ci sia)
                                    oldIdAnnessi = Guid.Empty
                                End If
                            End If
                        End If
                    End If

                    If Not addedDocuments.IsNullOrEmpty OrElse Not uscUploadAnnexes.GetNodeValue(0).Eq(FakeNodeValue) Then
                        Dim signature As String = ""
                        idAnnessi = InsertFileToBiblosWithGuid(addedDocuments, Guid.Empty, "AN", True, workStep.Description, signature)
                        '' Se è stata creata una nuova catena, aggiungo in coda 
                        If (oldIdAnnessi <> Guid.Empty AndAlso idAnnessi.Value <> oldIdAnnessi) Then
                            Service.CopyDocuments(oldIdAnnessi, CurrentResolution.Location.ReslBiblosDSDB, idAnnessi.Value, signature)
                        End If
                    End If

                    stepExists = True
                End If

                ' Allegati privacy
                If PnlPrivacyAttachmentVisible Then
                    idPrivacyAttachment = CType(ReflectionHelper.GetPropertyCase(_fileResolution, workStep.FieldPrivacyAttachment), Integer)

                    Dim addedDocuments As IList(Of DocumentInfo) = uscUploadPrivacyAttachment.DocumentInfosAdded
                    If Not addedDocuments.IsNullOrEmpty OrElse Not uscUploadPrivacyAttachment.GetNodeValue(0).Eq(FakeNodeValue) Then
                        idPrivacyAttachment = InsertFileToBiblos(addedDocuments, 0, "AR", True, workStep.Description, "")
                    End If

                    stepExists = True
                End If

                If Action.Eq("NEXT") Then
                    'Accodamento
                    If StringHelper.InStrTest(workStep.ManagedWorkflowData, "ComposeDoc") And stepExists Then
                        Try
                            ComposeDocument("" & workStep.ManagedWorkflowData, _fileResolution, idDocumento.Value)
                        Catch ex As Exception
                            FileLogger.Warn(LoggerName, ex.Message, ex)
                            AjaxAlert("ATTENZIONE\n\nL'accodamento del documento non è andato a buon fine.\n\nRipetere l'operazione.")
                        End Try
                    End If
                End If

                btnConferma.Enabled = False

                ' Salvo le modifiche ai documenti
                ' Documento Principale
                If idDocumento.HasValue AndAlso Not String.IsNullOrEmpty(workStep.FieldDocument) Then
                    If stepExists Then
                        stepExists = Facade.ResolutionFacade.SqlResolutionFileUpdate(IdResolution, idDocumento, workStep.FieldDocument)
                        If stepExists Then
                            Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RD, String.Format("Documenti {0} {1}.", workStep.Description, If(idDocumento.Value = 0, "rimossi", "aggiunti in catena " & idDocumento.Value)))
                        End If
                    End If
                End If

                ' Documenti Omissis
                If idDocumentiOmissis.HasValue AndAlso Not String.IsNullOrEmpty(workStep.FieldDocumentsOmissis) Then
                    If stepExists Then
                        stepExists = Facade.ResolutionFacade.SqlResolutionFileUpdate(IdResolution, idDocumentiOmissis.Value, workStep.FieldDocumentsOmissis)
                        If stepExists Then
                            Facade.ResolutionLogFacade.Insert(CurrentResolution, ResolutionLogType.RD, String.Format("Documenti Omissis {0} {1}.", workStep.Description, If(idDocumentiOmissis.Value = Guid.Empty, "rimossi", "aggiunti in catena " & idDocumentiOmissis.Value.ToString())))
                        End If
                    End If
                End If

                ' Allegati
                If idAllegati.HasValue AndAlso Not String.IsNullOrEmpty(workStep.FieldAttachment) Then
                    If stepExists Then
                        stepExists = Facade.ResolutionFacade.SqlResolutionFileUpdate(IdResolution, idAllegati.Value, workStep.FieldAttachment)
                        If stepExists Then
                            Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RD, String.Format("Allegati {0} {1}.", workStep.Description, If(idAllegati.Value = 0, "rimossi", "aggiunti in catena " & idAllegati.Value.ToString())))
                        End If
                    End If
                End If

                ' Allegati Omissis
                If idAllegatiOmissis.HasValue AndAlso Not String.IsNullOrEmpty(workStep.FieldAttachmentsOmissis) Then
                    If stepExists Then
                        stepExists = Facade.ResolutionFacade.SqlResolutionFileUpdate(IdResolution, idAllegatiOmissis.Value, workStep.FieldAttachmentsOmissis)
                        If stepExists Then
                            Facade.ResolutionLogFacade.Insert(CurrentResolution, ResolutionLogType.RD, String.Format("Allegati Omissis {0} {1}.", workStep.Description, If(idAllegatiOmissis.Value = Guid.Empty, "rimossi", "aggiunti in catena " & idAllegatiOmissis.Value.ToString())))
                        End If
                    End If
                End If

                ' Annessi
                If idAnnessi.HasValue AndAlso Not String.IsNullOrEmpty(workStep.FieldAnnexed) Then
                    If stepExists Then
                        stepExists = Facade.ResolutionFacade.SqlResolutionFileUpdate(IdResolution, idAnnessi.Value, workStep.FieldAnnexed)
                        If stepExists Then
                            Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RD, String.Format("Annessi {0} {1}.", workStep.Description, If(idAnnessi.Value = Guid.Empty, "rimossi", "aggiunti in catena " & idAnnessi.Value.ToString())))
                        End If
                    End If
                End If

                ' Allegati Riservati
                If idPrivacyAttachment.HasValue AndAlso Not String.IsNullOrEmpty(workStep.FieldPrivacyAttachment) Then
                    If stepExists Then
                        stepExists = Facade.ResolutionFacade.SqlResolutionFileUpdate(IdResolution, idPrivacyAttachment, workStep.FieldPrivacyAttachment)
                        If stepExists Then
                            Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RD, String.Format("Allegati Riservati {0} {1}.", workStep.Description, If(idPrivacyAttachment.Value = 0, "rimossi", "aggiunti in catena " & idPrivacyAttachment.Value.ToString())))
                        End If
                    End If
                End If

                ' Se tutto è andato a buon fine salvo le modifiche
                If stepExists Then
                    Facade.ResolutionFacade.Save(CurrentResolution)
                End If
            Else
                stepExists = Facade.ResolutionFacade.SqlResolutionDeleteStep(IdResolution, workStep)
                Dim dataFlusso As String = String.Format("{0:dd/MM/yyyy}", txtData.SelectedDate)
                CurrentResolution.DeclineNote = String.Format("{0}§{1}§{2}§{3}", txtMotivazione.Text, _step, workStep.CustomDescription, dataFlusso)
                Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RU, String.Format("RETROSTEP DA {0}. Motivazione: {1}", workStep.CustomDescription, txtMotivazione.Text.Split("§"c)(0)))
            End If
        End If
        '--Salvataggio ResolutionWorkflow
        If stepExists Then
            ' Allegati e Documento:
            ' Se non ho salvato su ResolutionFile (nothing) rileggo il valore originale se esiste e imposto un default se non c'è

            ' Documento Principale
            If Not idDocumento.HasValue Then
                If Not String.IsNullOrEmpty(workStep.FieldDocument) Then
                    idDocumento = CType(ReflectionHelper.GetPropertyCase(CurrentResolution, workStep.FieldDocument), Integer)
                Else
                    idDocumento = 0
                End If
            End If

            ' Documenti Omissis
            If Not idDocumentiOmissis.HasValue Then
                If Not String.IsNullOrEmpty(workStep.FieldDocumentsOmissis) Then
                    idDocumentiOmissis = CType(ReflectionHelper.GetPropertyCase(CurrentResolution, workStep.FieldDocumentsOmissis), Guid)
                Else
                    idDocumentiOmissis = Guid.Empty
                End If
            End If

            'Allegati
            If Not idAllegati.HasValue Then
                If Not String.IsNullOrEmpty(workStep.FieldAttachment) Then
                    idAllegati = CType(ReflectionHelper.GetPropertyCase(CurrentResolution, workStep.FieldAttachment), Integer)
                Else
                    idAllegati = 0
                End If
            End If

            ' Allegati Omissis
            If Not idAllegatiOmissis.HasValue Then
                If Not String.IsNullOrEmpty(workStep.FieldAttachmentsOmissis) Then
                    idAllegatiOmissis = CType(ReflectionHelper.GetPropertyCase(CurrentResolution, workStep.FieldAttachmentsOmissis), Guid)
                Else
                    idAllegatiOmissis = Guid.Empty
                End If
            End If

            ' Annessi
            If Not idAnnessi.HasValue Then
                If Not String.IsNullOrEmpty(workStep.FieldAnnexed) Then
                    idAnnessi = CType(ReflectionHelper.GetPropertyCase(CurrentResolution, workStep.FieldAnnexed), Guid)
                Else
                    idAnnessi = Guid.Empty
                End If
            End If

            ' Allegati Riservati
            If Not idPrivacyAttachment.HasValue Then
                If Not String.IsNullOrEmpty(workStep.FieldPrivacyAttachment) Then
                    idPrivacyAttachment = CType(ReflectionHelper.GetPropertyCase(CurrentResolution, workStep.FieldPrivacyAttachment), Integer)
                Else
                    idPrivacyAttachment = 0
                End If
            End If

            Select Case Action
                Case "Modify"
                    ' Catene a nothing; 0 e guid.empty = nothing
                    Facade.ResolutionWorkflowFacade.SqlResolutionWorkflowUpdate(IdResolution, idDocumento.Value, idAllegati.Value, idPrivacyAttachment.Value, idAnnessi.Value, idDocumentiOmissis.Value, idAllegatiOmissis.Value)
                Case "Next" 'Inserimento in ResolutionWorkflow
                    If _step = 2 Then
                        ' Adozione -> Pubblicazione
                        CurrentResolution.CheckPublication = True
                        Facade.ResolutionFacade.Save(CurrentResolution)
                    End If
                    ' Catene a nothing = nothing; guid.empty e 0 non aggiorna
                    Facade.ResolutionWorkflowFacade.InsertNextStep(IdResolution, _step, idDocumento.Value, idAllegati.Value, idPrivacyAttachment.Value, idAnnessi.Value, idDocumentiOmissis.Value, idAllegatiOmissis.Value, DocSuiteContext.Current.User.FullUserName)
                Case "Delete"
                    Facade.ResolutionWorkflowFacade.EnablePreviousStep(IdResolution, _step)
            End Select
        End If

        ' Stampo il promemoria di stampa frontalino solo se la gestione è cartacea
        If Not String.IsNullOrEmpty(ResolutionEnv.PromemoriaAdozione) AndAlso workStep.Description.Eq(WorkflowStep.ADOZIONE) Then
            If ResolutionEnv.GestioneDigitale(CurrentResolution) AndAlso (Not archive) AndAlso Action.Eq("Next") Then
                AjaxAlert(ResolutionEnv.PromemoriaAdozione)
            End If
        End If

        If workStep.Description.Eq(WorkflowStep.ADOZIONE) Then
            Dim fileTemp As String = String.Empty
            ' Verifico se devo visualizzare a video i frontalini: lo visualizzo solo se selezionato cartaceo e non aggiunto documento
            If idDocumento <= 0 And Not archive Then
                fileTemp = String.Format("{0}Fontalini_{1}.pdf", CommonUtil.UserDocumentName, Guid.NewGuid().ToString("N"))
                Dim fDestination As String = CommonInstance.AppTempPath & fileTemp
                Dim managerPdf As New PdfMerge()
                For Each doc As ResolutionFrontispiece In frontalini
                    managerPdf.AddDocument(doc.Path)
                Next
                managerPdf.Merge(fDestination)
            End If

            If Not String.IsNullOrEmpty(fileTemp) Then
                Session.Add("DocumentToOpen", CommonInstance.AppTempHttp & fileTemp)
            End If
        End If

        ' Se sto andando nello stato di pubblicazione, devo automaticamente Confermare le serie documentali senza documento richiesto
        If Action.Eq("NEXT") AndAlso workStep.Description.Eq(WorkflowStep.PUBBLICAZIONE) Then
            CurrentResolutionDocumentSeriesItemFacade.ConfirmAndPublishSeries(CurrentResolution)
        End If

        'Invio comando di creazione/aggiornamento Resolution alle WebApi
        If Not Action.Eq("DELETE") AndAlso Not workStep.Description.Eq(WorkflowStep.PROPOSTA) Then
            If Action.Eq("NEXT") AndAlso workStep.Description.Eq(WorkflowStep.ADOZIONE) Then
                Facade.ResolutionFacade.SendCreateResolutionCommand(CurrentResolution)
            Else
                Facade.ResolutionFacade.SendUpdateResolutionCommand(CurrentResolution)
            End If
        End If

        'Se il parametro degli avanzamenti di step automatici è attivo, creo un attività del JeepService per ogni atto selezionato. 
        If DocSuiteContext.Current.ResolutionEnv.AutomaticActivityStepEnabled AndAlso pnlAutomatismi.Visible = True Then

            Dim activityDocument As ResolutionActivityDocumentModel = New ResolutionActivityDocumentModel()
            Dim documentId As Guid

            For Each node As RadTreeNode In rtvListDocument.Nodes(0).Nodes
                If node.Checked AndAlso Guid.TryParse(node.Value, documentId) Then
                    activityDocument.Ids.Add(documentId)
                End If
            Next
            activityDocument.IsPrivacy = False
            If CurrentResolution.Container.Privacy.HasValue AndAlso CurrentResolution.Container.Privacy.HasValue Then
                activityDocument.IsPrivacy = True
            End If
            Facade.ResolutionActivityFacade.CreatePublicationActivity(CurrentResolution, activityDocument)
            Dim effectivenessDate As DateTimeOffset = DateAdd(DateInterval.Day, 10, CurrentResolution.PublishingDate.Value)
            Facade.ResolutionActivityFacade.CreateExecutiveActivity(CurrentResolution, effectivenessDate)
        End If

        AjaxManager.ResponseScripts.Add("return CloseWindow('');")
    End Sub

    Private Sub CvForzaAvanzamentoServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles cvForzaAvanzamento.ServerValidate
        args.IsValid = chkForzaAvanzamento.Checked
    End Sub

    Protected Sub ChkTuttiICodiciCheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles chkTuttiICodici.CheckedChanged
        ' non serve perchè viene già fatto nell'inizializzazione
        ' DoDataBindDdlServizio()

        If CurrentResolution IsNot Nothing And Not String.IsNullOrEmpty(CurrentResolution.ServiceNumber) Then
            InitializeDdlServizio(CurrentResolution)
        Else
            InitializeDdlServizioByServiceNumber(txtNumeroServizio.Text)
        End If
    End Sub

    Protected Sub DdlServizioSelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        ServiceNumberCheck()
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        uscUploadDocumenti.SignButtonEnabled = ResolutionEnv.IsFDQEnabled

        Dim workflowData As String = String.Empty
        If Facade.TabWorkflowFacade.SqlTabWorkflowManagedWData(IdResolution, CurrentResolution.WorkflowType, _step + 1S, workflowData) AndAlso
            StringHelper.InStrTest(workflowData, "ServiceNumber") Then
            DoDataBindDdlServizio()
        End If

        CommonInstance.UserDeleteTemp(TempType.I)
        SetOptionsVisibility(False)
        PnlPrivacyVisible = False

        If CurrentResolution Is Nothing Then
            Exit Sub
        End If
        _fileResolution = Facade.FileResolutionFacade.GetByResolution(CurrentResolution)(0)
        txtIdLocation.Text = CurrentResolution.Location.Id.ToString()

        Dim workflow As TabWorkflow = Nothing 'drW
        Dim workNext As TabWorkflow = Nothing 'drWNext
        Dim workActive As TabWorkflow = Nothing 'drWActive

        If Not Facade.TabWorkflowFacade.GetByStep(CurrentResolution.WorkflowType, _step, workActive) Then
            Exit Sub
        End If

        'Inizializzo il pannello di upload allegati privacy a Visibile = False
        PnlPrivacyAttachmentVisible = False

        Select Case Action
            Case "Modify", "Delete"
                CheckAdoptionDate(Action, workActive, txtData)

                Dim resWorkflow As ResolutionWorkflow 'drRW
                resWorkflow = Facade.ResolutionWorkflowFacade.SqlResolutionWorkflowSearch(IdResolution, _step)
                If resWorkflow Is Nothing Then Exit Sub
                InitializePanel(0)
                ' Descrizioni
                lblTitolo.Text = workActive.CustomDescription & If(Action.Eq("Modify"), " - Modifica", " - Elimina")
                lblDocumento.Text = "" & workActive.DocumentDescription
                If lblDocumento.Text = "" Then 'Prendo la descrizione dello step successivo
                    Dim workflow2 As TabWorkflow = Nothing 'drW2
                    If Facade.TabWorkflowFacade.GetByStep(CurrentResolution.WorkflowType, _step + 1S, workflow2) Then
                        lblDocumento.Text = "" & workflow2.DocumentDescription
                    End If
                End If

                ' Documento
                If PnlDocumentoVisible Then
                    InitializeDocument(workActive)
                    uscUploadDocumenti.IsDocumentRequired = False
                End If

                ' Documenti Omissis
                If PnlDocumentiOmissisVisible Then
                    InitializeDocumentiOmissis(workActive)
                End If

                ' Allegati
                If PnlAttachVisible Then
                    InitializeAttachment(workActive)
                End If

                ' Allegati Omisisis
                If PnlAttachOmissisVisible Then
                    InitializeAttachmentOmissis(workActive)
                End If

                ' Annessi
                If PnlAnnexesVisible Then
                    InitializeAnnexes(workActive)
                End If

                ' Allegati Riservati
                If PnlPrivacyAttachmentVisible Then
                    InitializePrivacyAttachment(workActive)
                End If

                ' Data
                If pnlData.Visible Then
                    txtData.SelectedDate = CType(ReflectionHelper.GetProperty(CurrentResolution, workActive.FieldDate), Date?)
                End If

                ' N. Servizio
                If pnlNumeroServizio.Visible AndAlso (CurrentResolution.ServiceNumber IsNot Nothing) Then
                    InitializeDdlServizio(CurrentResolution)
                End If

                ' Controllo Data StepPrecedente
                If Action.Eq("Modify") Then
                    cvCompareData.Visible = TabWorkflowFacade.TestManagedWorkflowDataProperty(workActive.ManagedWorkflowData, "Date", "MOD", "Tested")
                Else
                    cvCompareData.Visible = TabWorkflowFacade.TestManagedWorkflowDataProperty(workActive.ManagedWorkflowData, "Date", "DEL", "Tested")
                End If

                If _step > 1 Then
                    Dim workPre As TabWorkflow = Nothing 'drWPre
                    If Not Facade.TabWorkflowFacade.GetByStep(CurrentResolution.WorkflowType, _step - 1S, workPre) Then
                        Exit Sub
                    End If
                    txtLastWorkflowDate.Text = CType(ReflectionHelper.GetProperty(CurrentResolution, workPre.FieldDate), Date?).DefaultString()
                    cvCompareData.ErrorMessage &= txtLastWorkflowDate.Text
                End If

            Case "Next"
                Dim mainChainToDuplicate As Integer = -1
                Dim attachmentChainToDuplicate As Integer = -1
                Dim omissisMainChainToDuplicate As Guid = Guid.Empty
                Dim omissisAttachmentChainToDuplicate As Guid = Guid.Empty
                If Not Facade.TabWorkflowFacade.GetByStep(CurrentResolution.WorkflowType, _step + 1S, workNext) Then
                    Exit Sub
                End If

                ' Attivo il controllo sulla data per il passo di worklow successivo
                CheckAdoptionDate(Action, workNext, txtData)

                ' ALEJANDRO: riporto documento firmato
                ' Se il documento dello step precedente è firmato (P7M) allora lo propongo come default nello step successivo
                If (ResolutionEnv.Configuration.Eq(ConfTo) OrElse ResolutionEnv.CopyDocumentsToAdoption) AndAlso (Facade.TabWorkflowFacade.GetByStep(CurrentResolution.WorkflowType, _step, workflow)) Then
                    Dim fileRes As FileResolution = Facade.FileResolutionFacade.GetByResolution(CurrentResolution)(0)

                    ''Calcolo il documento effettivo solo se provengo dalla proposta
                    If workflow.Description.Eq(WorkflowStep.PROPOSTA) OrElse workflow.Description.Eq(WorkflowStep.ATTO_CONFORME) Then
                        ''Documento principale
                        mainChainToDuplicate = CType(ReflectionHelper.GetPropertyCase(fileRes, workflow.FieldDocument), Integer)
                        omissisMainChainToDuplicate = CType(ReflectionHelper.GetPropertyCase(fileRes, workActive.FieldDocumentsOmissis), Guid)
                    End If

                    ''Allegati
                    attachmentChainToDuplicate = CType(ReflectionHelper.GetPropertyCase(fileRes, workflow.FieldAttachment), Integer)
                    omissisAttachmentChainToDuplicate = CType(ReflectionHelper.GetPropertyCase(fileRes, workActive.FieldAttachmentsOmissis), Guid)
                End If
                ' FINE ALEJANDRO: riporto documento firmato
                Dim s As String = String.Empty
                If ResolutionEnv.Configuration.Eq(ConfTo) Then ' Configurazione ASL3-TO
                    lblTitolo.Text = workNext.CustomDescription

                Else ' Configurazione BASE
                    s = workNext.CustomDescription

                    If StringHelper.InStrTest(workNext.ManagedWorkflowData, "NextStep") Then
                        Dim workNext2 As TabWorkflow = Nothing 'drWNext2
                        If Facade.TabWorkflowFacade.GetByStep(CurrentResolution.WorkflowType, _step + 2S, workNext2) Then
                            If workNext2.CustomDescription.Substring(0, 1).Eq("E") Then
                                s &= " ed "
                            Else
                                s &= " e "
                            End If
                            's &= workNext2.Description
                            s &= workNext2.CustomDescription
                        End If
                    End If

                    lblTitolo.Text = s & " - Inserimento"
                End If

                lblDocumento.Text = workNext.DocumentDescription
                If String.IsNullOrEmpty(lblDocumento.Text) Then
                    ' Prendo la descrizione dello step successivo
                    Dim work2 As TabWorkflow = Nothing 'drW2
                    If Facade.TabWorkflowFacade.GetByStep(CurrentResolution.WorkflowType, _step + 2S, work2) Then
                        lblDocumento.Text = work2.DocumentDescription
                    End If
                End If

                ' Workaround per gestire il nome del pannello di generazione del frontalino di esecutività
                If (ResolutionEnv.Configuration.Eq(ConfAuslPc) AndAlso lblDocumento.Text.Eq("Documento") AndAlso workNext.Description.Eq(WorkflowStep.ESECUTIVA)) Then
                    lblDocumento.Text = "Rel. Esecutività"
                End If

                InitializePanel(_step + 1S)
                ' Step description
                StepDescription = workNext.Description
                lblActualStep.Text = workActive.CustomDescription
                lblNextStep.Text = s
                If ResolutionEnv.Configuration = ConfTo Then ' Configurazione ASL3-TO
                    lblNextStep.Text = workNext.CustomDescription
                End If
                ' Documento
                If PnlDocumentoVisible Then
                    InitializeDocument(workNext)
                    If mainChainToDuplicate > 0 Then
                        '' Rimuovo l'eventuale fakenode che fa confusione nella casistica di duplicazione documenti verso l'adozione
                        '' se voglio il fakenode allora devo disabilitare la duplicazione dei documenti
                        uscUploadDocumenti.ClearNodes()
                        DuplicaDocumento(mainChainToDuplicate, uscUploadDocumenti, CurrentResolution, True, True, GetSignature(String.Empty, workActive.Description))
                    End If
                    ' In fase di adozione, viene inserito il documento aggiunto in "proposta" dell' atto se e solo se non è già stato caricato
                    If (uscUploadDocumenti.DocumentsAddedCount = 0) Then
                        InitializeAdoptionDocument()
                    End If
                    If (uscUploadDocumenti.DocumentsAddedCount = 0) Then
                        InitializePublicationDocument()
                    End If
                End If

                ' Documenti Omissis
                If PnlDocumentiOmissisVisible Then
                    'Rimosso volontariamente in modo da riportare nello step successivo solamente i documenti firmati
                    'InitializeDocumentiOmissis(workNext)
                    If omissisMainChainToDuplicate <> Guid.Empty Then
                        DuplicaDocumento(omissisMainChainToDuplicate, uscUploadDocumentiOmissis, CurrentResolution, True, True, GetSignature(String.Empty, workActive.Description))
                    End If
                End If

                ' Allegati
                If PnlAttachVisible Then
                    'Rimosso volontariamente in modo da riportare nello step successivo solamente i documenti firmati
                    'InitializeAttachment(workNext)
                    If attachmentChainToDuplicate > 0 Then
                        DuplicaDocumento(attachmentChainToDuplicate, uscUploadAllegati, CurrentResolution, True, ResolutionEnv.Configuration.Eq(ConfTo) OrElse ResolutionEnv.CopyOnlySignedDocumentsToAdoption, GetSignature(String.Empty, workActive.Description))
                    End If
                End If

                ' Allegati Omissis
                If PnlAttachOmissisVisible Then
                    'Rimosso volontariamente in modo da riportare nello step successivo solamente i documenti firmati
                    'InitializeAttachmentOmissis(workNext)
                    If omissisAttachmentChainToDuplicate <> Guid.Empty Then
                        DuplicaDocumento(omissisAttachmentChainToDuplicate, uscUploadAllegatiOmissis, CurrentResolution, True, ResolutionEnv.Configuration.Eq(ConfTo) OrElse ResolutionEnv.CopyOnlySignedDocumentsToAdoption, GetSignature(String.Empty, workActive.Description))
                    End If
                End If

                ' Annessi
                If PnlAnnexesVisible Then
                    InitializeAnnexes(workNext)
                End If

                ' Allegati Riservati
                If PnlPrivacyAttachmentVisible Then
                    InitializePrivacyAttachment(workNext)
                End If

                ' N. Servizio
                If pnlNumeroServizio.Visible AndAlso (CurrentResolution.ServiceNumber IsNot Nothing) Then
                    InitializeDdlServizio(CurrentResolution)
                End If

                ' Data stepAttivo
                Select Case ResolutionEnv.Configuration
                    Case ConfTo
                        Dim sData As String = CType(ReflectionHelper.GetPropertyCase(CurrentResolution, workActive.FieldDate), Date?).DefaultString()
                        txtLastWorkflowDate.Text = "" & sData
                        cvCompareData.ErrorMessage &= "" & sData
                        cvCompareData.Visible = TabWorkflowFacade.TestManagedWorkflowDataProperty(workNext.ManagedWorkflowData, "Date", "INS", "Tested")
                        '-- Setto la data
                        Select Case workNext.Description.Trim
                            Case WorkflowStep.ADOZIONE
                                pnlFrontalino.Visible = True
                                Dim isDefaultFrontalinoDigitale As Boolean = TabWorkflowFacade.TestManagedWorkflowDataProperty(workNext.ManagedWorkflowData, "Frontespizio", "SEL", "Digitale")
                                radioFrontalino.SelectedValue = If(isDefaultFrontalinoDigitale, "frontalinodigitale", "frontalinocartaceo")
                            Case WorkflowStep.PUBBLICAZIONE
                                'Invio CS + 5 gg + 1 gg (solo TO) 
                                If CurrentResolution.SupervisoryBoardWarningDate.HasValue Then
                                    txtData.Enabled = False
                                    txtData.SelectedDate = DateAdd(DateInterval.Day, 6, CurrentResolution.SupervisoryBoardWarningDate.Value)
                                    If DocSuiteContext.Current.ResolutionEnv.AutomaticActivityStepEnabled Then
                                        pnlAutomatismi.Visible = True
                                        Dim node As RadTreeNode
                                        For Each doc As WebDoc In GetPubblicationDocuments(CurrentResolution, String.Empty)
                                            node = New RadTreeNode
                                            node.Text = doc.Name
                                            node.Value = doc.DocumentGuid.ToString()
                                            rtvListDocument.Nodes(0).Nodes.Add(node)
                                        Next
                                    End If
                                Else
                                    'Manca la data per il calcolo
                                    lblInfo.Text = "Manca la data di Invio al Collegio Sindacale. Impossibile Pubblicare la Delibera."
                                    lblInfo.Visible = True
                                    btnConferma.Enabled = False
                                End If
                            Case WorkflowStep.ESECUTIVA
                                If CurrentResolution.OCRegion.GetValueOrDefault(False) Then 'Va in Regione
                                    'Ricezione + 40 gg
                                    If CurrentResolution.ConfirmDate.HasValue Then
                                        txtData.SelectedDate = DateAdd(DateInterval.Day, 40, CurrentResolution.ConfirmDate.Value)
                                    Else
                                        'Manca la data per il calcolo
                                        lblInfo.Text = "Manca la data di Ricezione in Regione. Impossibile rendere Esecutiva la Delibera."
                                        lblInfo.Visible = True
                                        btnConferma.Enabled = False
                                    End If
                                Else
                                    'Pubblicazione + 10gg
                                    txtData.SelectedDate = DateAdd(DateInterval.Day, 10, CurrentResolution.PublishingDate.Value)
                                End If
                        End Select

                    Case ConfAuslPc
                        Select Case workNext.Description.Trim
                            Case "Pubblicazione"
                                If ResolutionEnv.CheckOCValidations AndAlso CurrentResolution.OCSupervisoryBoard AndAlso Not CurrentResolution.SupervisoryBoardWarningDate.HasValue Then
                                    ' Manca la data dell'invio al collegio sindacale
                                    AjaxManager.ResponseScripts.Add("return CloseWindow('');")
                                    AjaxAlert(StringHelper.ReplaceAlert("Manca la data di Invio al Collegio Sindacale. Impossibile pubblicare!"))
                                    Exit Sub
                                ElseIf ResolutionEnv.CheckOCValidations AndAlso CurrentResolution.OCManagement AndAlso Not CurrentResolution.ManagementWarningDate.HasValue Then
                                    ' Manca la data dell'invio alla conferenza dei sindaci
                                    AjaxManager.ResponseScripts.Add("return CloseWindow('');")
                                    AjaxAlert(StringHelper.ReplaceAlert("Manca la data di Invio alla Conferenza dei Sindaci. Impossibile pubblicare!"))
                                    Exit Sub
                                ElseIf ResolutionEnv.CheckOCValidations AndAlso CurrentResolution.OCRegion AndAlso Not CurrentResolution.WarningDate.HasValue Then
                                    ' Manca la data dell'invio alla regione
                                    AjaxManager.ResponseScripts.Add("return CloseWindow('');")
                                    AjaxAlert(StringHelper.ReplaceAlert("Manca la data di Invio alla Regione. Impossibile pubblicare!"))
                                    Exit Sub
                                End If

                                ' Pannelli per la pubblicazione Web
                                If ResolutionEnv.CheckOCValidations Then
                                    SetOptionsVisibility(True)
                                    PnlPrivacyVisible = selectPrivacy.SelectedItem IsNot Nothing AndAlso Boolean.Parse(selectPrivacy.SelectedValue)
                                    rfvSelectPrivacy.Enabled = True

                                End If


                        End Select

                        Dim comparisonDate As String = workActive.FieldDate
                        If CurrentResolution.Type.Id.Equals(ResolutionType.IdentifierDelibera) AndAlso Not String.IsNullOrEmpty(ResolutionEnv.AdoptionComparisonDate) Then
                            comparisonDate = ResolutionEnv.AdoptionComparisonDate
                        End If
                        txtLastWorkflowDate.Text = CType(ReflectionHelper.GetProperty(CurrentResolution, comparisonDate), Date?).DefaultString()
                        cvCompareData.ErrorMessage &= txtLastWorkflowDate.Text
                        cvCompareData.Visible = TabWorkflowFacade.TestManagedWorkflowDataProperty(workNext.ManagedWorkflowData, "Date", "INS", "Tested")

                    Case Else
                        txtLastWorkflowDate.Text = CType(ReflectionHelper.GetProperty(CurrentResolution, workActive.FieldDate), Date?).DefaultString()
                        cvCompareData.ErrorMessage &= txtLastWorkflowDate.Text
                        cvCompareData.Visible = TabWorkflowFacade.TestManagedWorkflowDataProperty(workNext.ManagedWorkflowData, "Date", "INS", "Tested")

                End Select
        End Select

        MasterDocSuite.HistoryTitle = lblTitolo.Text

        If (Facade.ResolutionFacade.IsManagedProperty("ServiceNumber", CurrentResolution.Type.Id, "AUTOINC")) Then
            ServiceNumberCheck()
        End If

        If ResolutionEnv.CheckPublicationOrder AndAlso StepDescription.Eq(WorkflowStep.PUBBLICAZIONE) Then
            Dim serviceNumber As String = CurrentResolution.ServiceNumber
            If CurrentResolution.Type.Id = ResolutionType.IdentifierDetermina Then
                serviceNumber = String.Concat(serviceNumber.Split("/"c).First(), "/")
            End If
            Dim resolution As Resolution = Facade.ResolutionFacade.GetActualToPublicate(serviceNumber, CurrentResolution.Type.Id, CurrentResolution.WorkflowType)
            If resolution IsNot Nothing AndAlso Not resolution.Id = CurrentResolution.Id Then
                btnConferma.Enabled = False
                AjaxAlert("E' obbligatorio pubblicare gli atti nella loro sequenza numerica")
            End If
        End If
    End Sub

    ''' <summary>
    ''' Carica il controllo di visualizzazione dei codici settori derivando il codice dall'atto corrente
    ''' </summary>
    ''' <param name="numServizio">ServiceNumber dell'atto corrente</param>
    ''' <remarks></remarks>
    Private Sub InitializeDdlServizioByServiceNumber(ByVal numServizio As String)
        '' Carico nel modo standard
        DoDataBindDdlServizio()

        '' Carico il codice corrente se ho un numero di servizio valido
        If Not String.IsNullOrEmpty(numServizio) Then
            Dim values As String() = numServizio.Split("/"c)

            If values.GetLength(0) = 2 Then
                '' Derivo il primo codice settore
                For Each item As ListItem In From item1 As ListItem In ddlServizio.Items Where item1.Text.Contains(values(0))
                    item.Selected = True
                Next

                ' Se non lo posso modificare scrivo tutto sul campo
                If txtNumeroServizio.Enabled Then
                    If values.Length > 1 Then
                        txtNumeroServizio.Text = values(1)
                    Else
                        txtNumeroServizio.Text = ""
                    End If
                Else
                    txtNumeroServizio.Text = numServizio
                End If
            Else
                If values.Length > 0 Then
                    txtNumeroServizio.Text = values(0)
                Else
                    txtNumeroServizio.Text = ""
                End If
            End If
        Else
            txtNumeroServizio.Text = ""
        End If
    End Sub

    Private Function InitializeDdlServizioByRole(ByVal idRole As Short) As Boolean
        '' Carico l'elenco
        DoDataBindDdlServizio()

        If ddlServizio.Items.FindByValue(idRole.ToString) IsNot Nothing Then
            '' seleziono l'elemento
            ddlServizio.SelectedValue = idRole.ToString
            Return True
        End If
        Return False
    End Function

    Private Sub InitializeDdlServizio(ByRef resl As Resolution)
        '' Se è presente l'idRole allora lo uso
        If resl.AdoptionRoleId.HasValue AndAlso InitializeDdlServizioByRole(resl.AdoptionRoleId.Value) Then
            '' Ho un idRole e l'ho correttamente impostato
            '' devo solo completare il numero di servizio se presente
            If Not String.IsNullOrEmpty(resl.ServiceNumber) AndAlso resl.ServiceNumber.Split("/"c).Count > 1 Then
                If txtNumeroServizio.Enabled Then
                    txtNumeroServizio.Text = resl.ServiceNumber.Split("/"c)(1)
                Else
                    txtNumeroServizio.Text = resl.ServiceNumber
                End If
            End If
        Else
            '' Allora calcolo l'id nel vecchio modo
            InitializeDdlServizioByServiceNumber(resl.ServiceNumber)
        End If
    End Sub

    ''' <summary>
    ''' Leggo settori per popolare la combo dei codici di servizio
    ''' </summary>
    Private Sub DoDataBindDdlServizio()
        ddlServizio.Items.Clear()

        If Not chkTuttiICodici.Checked Then
            _roles = Facade.RoleFacade.GetUserRoles(DSWEnvironment.Resolution, 1, True)
        Else
            _roles = Facade.RoleFacade.GetRoles(DSWEnvironment.Resolution, 1, True, "", False, Nothing)
        End If

        Dim ddlServizioDictionary As Dictionary(Of String, String) = (From role In _roles Where Not String.IsNullOrEmpty(role.ServiceCode)).ToDictionary(Function(role) String.Format("{0} ({1})", role.ServiceCode, role.Name), Function(role) role.Id.ToString())
        ddlServizio.DataSource = From item In ddlServizioDictionary Order By item.Key
        ddlServizio.DataValueField = "Value"
        ddlServizio.DataTextField = "Key"
        ddlServizio.DataBind()

        'Aggiungo il valore iniziale
        If ddlServizio.Items.Count > 1 Then
            ddlServizio.Items.Insert(0, New ListItem("Seleziona Servizio", String.Empty))
        End If

    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlServizio, txtNumeroServizio)
        AjaxManager.AjaxSettings.AddAjaxSetting(selectPrivacy, pnlPrivacy)
        AjaxManager.AjaxSettings.AddAjaxSetting(selectPrivacy, pnlOptions, MasterDocSuite.AjaxDefaultLoadingPanel)
        'AjaxManager.AjaxSettings.AddAjaxSetting(selectPrivacy, uscUploadDocumenti.ButtonsPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscUploadDocumenti)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscUploadDocumentiOmissis)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscUploadAllegati)


        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscUploadAllegatiOmissis)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscUploadAnnexes)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlPrivacyAttachment)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscUploadPrivacyAttachment)

        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlPrivacyAttachment)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma, MasterDocSuite.AjaxFlatLoadingPanel)

        If PnlPrivacyVisible Then
            'AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlPrivacyAttachment)
        End If
    End Sub

    ''' <summary>
    ''' Inizializza i pannelli della pagina impostati da parametri del Database (TabWorkflow) in base allo step passato come parametro.
    ''' </summary>
    ''' <param name="reslStep">numero step</param>
    Private Sub InitializePanel(ByVal reslStep As Short)
        Dim workflowData As String = String.Empty
        pnlNextStep.Visible = Action.Eq("Next")
        pnMotivazione.Visible = Action.Eq("Delete")

        If Facade.TabWorkflowFacade.SqlTabWorkflowManagedWData(IdResolution, CurrentResolution.WorkflowType, reslStep, workflowData) Then
            Dim sProp As String = String.Empty
            Select Case Action
                Case "Next"
                    sProp = "INS"
                Case "Modify"
                    sProp = "MOD"
                Case "Delete"
                    sProp = ""
            End Select

            Dim workActive As TabWorkflow = Nothing 'WorkflowAttuale
            If Not Facade.TabWorkflowFacade.GetByStep(CurrentResolution.WorkflowType, _step, workActive) Then
                Exit Sub
            End If

            Dim workNext As TabWorkflow = Nothing 'WorkflowSuccessivo
            If Not Facade.TabWorkflowFacade.GetByStep(CurrentResolution.WorkflowType, _step + 1S, workNext) Then
                Exit Sub
            End If

            'parametro pannello data visibile
            pnlData.Visible = StringHelper.InStrTest(workflowData, "Date")
            'parametro data abilitata
            txtData.Enabled = Not String.IsNullOrEmpty(sProp) AndAlso TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "Date", sProp, "")
            If TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "Date", "TODAY", "") Then
                txtData.SelectedDate = Date.Today
                If (ResolutionEnv.Configuration = ConfAuslPc) Then
                    If (TabWorkflowFacade.TestManagedWorkflowDataProperty(workNext.ManagedWorkflowData, "Date", "INS", "Tested")) Then
                        Dim precedentDate As Date? = CType(ReflectionHelper.GetProperty(CurrentResolution, workActive.FieldDate), Date?)
                        If (txtData.SelectedDate < precedentDate) Then
                            txtData.SelectedDate = precedentDate
                        End If
                    End If
                End If
            End If

            If TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "Date", "AUTO", "") Then
                txtData.Enabled = False
                If (ResolutionEnv.Configuration = ConfAuslPc) Then
                    txtData.SelectedDate = CType(ReflectionHelper.GetProperty(CurrentResolution, workActive.FieldDate), Date?)
                End If
            End If
            'parametro data obbligatoria
            rfvData.Visible = TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "Date", "OBB", "")



            'parametro inserimento numero atto obbligatorio
            rfvNumero.Visible = TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "ServiceNumber", "OBB", "")
            'parametro inserimento numero atto abilitato
            txtNumeroServizio.Enabled = Not String.IsNullOrEmpty(sProp) AndAlso TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "ServiceNumber", sProp, "")

            ' Imposto l'etichetta del campo
            If CurrentResolution.Type.Id = 0 AndAlso Not ResolutionEnv.IncrementalNumberEnabled Then ' ATTO
                lblNumeroServizio.Text = "Codice/Numero"
            Else
                lblNumeroServizio.Text = "Numero"
            End If

            ' Se posso modificare il campo visualizzo anche gli altri elementi, altrimenti solo il textbox
            If StringHelper.InStrTest(workflowData, "ServiceNumber") Then
                pnlNumeroServizio.Visible = True
                ' Verifico tipo atto
                If txtNumeroServizio.Visible Then
                    If CurrentResolution.Type.Id = 0 AndAlso Not ResolutionEnv.IncrementalNumberEnabled Then ' ATTO
                        ddlServizio.Visible = True
                        ddlServizioSeparator.Visible = True
                        chkTuttiICodici.Visible = Not ResolutionEnv.DisableCheckTuttiICodici
                    End If
                Else
                    revNumero.Visible = False ' Disabilito il controllo numerico
                End If
            Else
                pnlNumeroServizio.Visible = False
            End If

            'DOCUMENTO
            'parametro pannello documento visibile
            PnlDocumentoVisible = StringHelper.InStrTest(workflowData, "Document")
            'parametro documento in sola lettura
            uscUploadDocumenti.ReadOnly = Not (Not String.IsNullOrEmpty(sProp) AndAlso TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "Document", sProp, ""))
            'parametro documento obbligatorio
            uscUploadDocumenti.IsDocumentRequired = TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "Document", "OBB", "")
            'parametro documenti multipli
            uscUploadDocumenti.MultipleDocuments = TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "Document", sProp, "N")

            'DOCUMENTI OMISSIS
            'parametro pannello allegati visibile
            PnlDocumentiOmissisVisible = StringHelper.InStrTest(workflowData, "DocumentsOmissis")
            'parametro allegati in sola lettura
            uscUploadDocumentiOmissis.ReadOnly = Not (Not String.IsNullOrEmpty(sProp) AndAlso TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "DocumentsOmissis", sProp, ""))
            'parametro allegati obbligatori
            uscUploadDocumentiOmissis.IsDocumentRequired = TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "DocumentsOmissis", "OBB", "")
            uscUploadDocumentiOmissis.ButtonCopyProtocol.Visible = DocSuiteContext.Current.ProtocolEnv.CopyProtocolDocumentsEnabled
            uscUploadDocumentiOmissis.ButtonCopyUDS.Visible = DocSuiteContext.Current.ProtocolEnv.UDSEnabled
            'parametro firma allegato consentita: si abilita in TabWorkflow con INS-SIGN (per l'inserimento) o MOD-SIGN per la modifica
            uscUploadDocumentiOmissis.SignButtonEnabled = ResolutionEnv.IsFDQEnabled AndAlso TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "DocumentsOmissis", sProp, "SIGN")

            'ALLEGATI
            'parametro pannello allegati visibile
            PnlAttachVisible = StringHelper.InStrTest(workflowData, "Attachment")
            'parametro allegati in sola lettura
            uscUploadAllegati.ReadOnly = Not (Not String.IsNullOrEmpty(sProp) AndAlso TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "Attachment", sProp, ""))
            'parametro allegati obbligatori
            uscUploadAllegati.IsDocumentRequired = TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "Attachment", "OBB", "")
            uscUploadAllegati.ButtonCopyProtocol.Visible = DocSuiteContext.Current.ProtocolEnv.CopyProtocolDocumentsEnabled
            uscUploadAllegati.ButtonCopyUDS.Visible = DocSuiteContext.Current.ProtocolEnv.UDSEnabled
            'parametro firma allegato consentita: si abilita in TabWorkflow con INS-SIGN (per l'inserimento) o MOD-SIGN per la modifica
            uscUploadAllegati.SignButtonEnabled = ResolutionEnv.IsFDQEnabled AndAlso TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "Attachment", sProp, "SIGN")
            If ResolutionEnv.HideDocumentButtons AndAlso workNext.Description.Eq("Adozione") Then
                uscUploadAllegati.ButtonPreviewEnabled = True
                uscUploadAllegati.ButtonRemoveEnabled = StringHelper.InStrTest(workNext.BiblosFileProperty, ".DEL.")
            End If


            'ALLEGATI OMISSIS
            'parametro pannello allegati visibile
            PnlAttachOmissisVisible = StringHelper.InStrTest(workflowData, "AttachmentOmissis")
            'parametro allegati in sola lettura
            uscUploadAllegatiOmissis.ReadOnly = Not (Not String.IsNullOrEmpty(sProp) AndAlso TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "AttachmentOmissis", sProp, ""))
            'parametro allegati obbligatori
            uscUploadAllegatiOmissis.IsDocumentRequired = TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "AttachmentOmissis", "OBB", "")
            uscUploadAllegatiOmissis.ButtonCopyProtocol.Visible = DocSuiteContext.Current.ProtocolEnv.CopyProtocolDocumentsEnabled
            uscUploadAllegatiOmissis.ButtonCopyUDS.Visible = DocSuiteContext.Current.ProtocolEnv.UDSEnabled
            'parametro firma allegato consentita: si abilita in TabWorkflow con INS-SIGN (per l'inserimento) o MOD-SIGN per la modifica
            uscUploadAllegatiOmissis.SignButtonEnabled = ResolutionEnv.IsFDQEnabled AndAlso TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "AttachmentOmissis", sProp, "SIGN")

            'ANNESSI
            'parametro pannello annessi visibile
            PnlAnnexesVisible = StringHelper.InStrTest(workflowData, "Annexed")
            'parametro annessi in sola lettura
            uscUploadAnnexes.ReadOnly = Not (Not String.IsNullOrEmpty(sProp) AndAlso TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "Annexed", sProp, ""))
            'parametro annessi obbligatori
            uscUploadAnnexes.IsDocumentRequired = TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "Annexed", "OBB", "")
            uscUploadAnnexes.ButtonCopyProtocol.Visible = DocSuiteContext.Current.ProtocolEnv.CopyProtocolDocumentsEnabled
            uscUploadAnnexes.ButtonCopyUDS.Visible = DocSuiteContext.Current.ProtocolEnv.UDSEnabled
            uscUploadAnnexes.SignButtonEnabled = ResolutionEnv.IsFDQEnabled AndAlso TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "Annexed", sProp, "SIGN")
            If ResolutionEnv.HideDocumentButtons AndAlso workNext.Description.Eq("Adozione") Then
                uscUploadAnnexes.ButtonPreviewEnabled = True
                uscUploadAnnexes.ButtonRemoveEnabled = StringHelper.InStrTest(workNext.BiblosFileProperty, ".DEL.")
            End If

            'ALLEGATI PRIVACY
            'parametro pannello allegati visibile
            Dim currentResolutionRights As New ResolutionRights(CurrentResolution)
            PnlPrivacyAttachmentVisible = False
            If currentResolutionRights.IsPrivacyAttachmentAllowed AndAlso StringHelper.InStrTest(workflowData, "PrivacyAttachment") Then
                PnlPrivacyAttachmentVisible = True
            End If
            'parametro allegati riservati in sola lettura
            uscUploadPrivacyAttachment.ReadOnly = Not (Not String.IsNullOrEmpty(sProp) AndAlso TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "PrivacyAttachment", sProp, ""))
            'parametro allegati riservati obbligatori
            uscUploadPrivacyAttachment.IsDocumentRequired = TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "PrivacyAttachment", "OBB", "")
            'parametro firma allegato riservato consentita: si abilita in TabWorkflow con INS-SIGN (per l'inserimento) o MOD-SIGN per la modifica
            uscUploadPrivacyAttachment.SignButtonEnabled = ResolutionEnv.IsFDQEnabled AndAlso TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "PrivacyAttachment", sProp, "SIGN")


            '11-02-2008: Gestione pagine documenti
            'pannello numero di pagine documento visibile solo se AUSL-PC
            pnlDocPages.Visible = False ' FG 20120405 - nascosto perchè non ancora implementato...
        End If
    End Sub

    Private Sub InitializeDocument(ByVal workflow As TabWorkflow)
        Dim fieldAttachment As Integer? = CType(ReflectionHelper.GetPropertyCase(_fileResolution, workflow.FieldDocument), Integer?)
        If fieldAttachment.HasValue AndAlso fieldAttachment.Value <> 0 Then
            If Not StepDescription.Eq("Ritiro Pubblicazione") Then
                uscUploadDocumenti.AddNode(lblDocumento.Text, "../Resl/Images/" & workflow.DocumentImageFile, FakeNodeValue, False, False)
            End If
            uscUploadDocumenti.ButtonRemoveEnabled = StringHelper.InStrTest(workflow.BiblosFileProperty, ".DEL.")
        End If
        uscUploadDocumenti.ButtonFrontespizioEnabled = StringHelper.InStrTest(workflow.BiblosFileProperty, ".GEN.")

        'EF 20120321 Per consentire a Piacenza di caricare anche un eventuale frontalino cartaceo da scanner o da file esterno
        If ResolutionEnv.HideDocumentButtons Then
            If StepDescription.Eq("Adozione") OrElse StepDescription.Eq("Pubblicazione") OrElse
                StepDescription.Eq("Ritiro Pubblicazione") OrElse StepDescription.Eq("Esecutività") Then
                If _fileResolution.IdProposalFile.HasValue AndAlso _fileResolution.IdProposalFile.Value <> 0 Then
                    uscUploadDocumenti.ButtonFileEnabled = False
                    uscUploadDocumenti.ButtonScannerEnabled = False
                    uscUploadDocumenti.ButtonLibrarySharepointEnabled = False
                    uscUploadDocumenti.SignButtonEnabled = False
                    uscUploadDocumenti.ButtonRemoveEnabled = StringHelper.InStrTest(workflow.BiblosFileProperty, ".DEL.")
                    uscUploadDocumenti.ButtonPreviewEnabled = True
                End If

                If StepDescription.Eq("Esecutività") AndAlso (CurrentResolution.Type.Id.Equals(ResolutionType.IdentifierDelibera) AndAlso
                         CurrentResolution.IsChecked.HasValue AndAlso CurrentResolution.IsChecked.Value) Then
                    uscUploadDocumenti.ButtonFileEnabled = True
                    uscUploadDocumenti.ButtonScannerEnabled = True
                End If
            End If
        End If
    End Sub

    Private Function DocumentMustSignAlert(docs As IList(Of DocumentInfo)) As Boolean
        ' Segnalo che nello step di adozione i documenti devono essere tutti firmati
        If ResolutionEnv.MustResolutionAdoptionDocumentIsSigned AndAlso StepDescription.Eq(WorkflowStep.ADOZIONE) Then
            For Each doc As DocumentInfo In docs
                If Not doc.IsSigned Then
                    AjaxAlert("Il passaggio dei documenti in adozione è possibile solo se firmati digitalmente")
                    Return True
                End If
            Next
        End If
        Return False
    End Function

    Private Sub InitializeAdoptionDocument()
        ' Carica documenti di default in fase di adozione
        If ResolutionEnv.LoadProposalDocumentInAdoptionState AndAlso StepDescription.Eq(WorkflowStep.ADOZIONE) Then
            If _fileResolution.IdProposalFile.HasValue AndAlso _fileResolution.IdProposalFile.Value <> 0 Then
                Dim proposal As New BiblosDocumentInfo(CurrentResolution.Location.ReslBiblosDSDB, _fileResolution.IdProposalFile.Value)
                uscUploadDocumenti.LoadDocumentInfo(proposal, proposal.IsSigned, True, True, True)
            End If
        End If
    End Sub
    Private Sub InitializePublicationDocument()
        ' Carica documenti di default in fase di pubblicazione
        If ResolutionEnv.LoadAdoptionDocumentInPublicationState AndAlso StepDescription.Eq(WorkflowStep.PUBBLICAZIONE) Then
            If _fileResolution.IdAssumedProposal.HasValue AndAlso _fileResolution.IdAssumedProposal.Value <> 0 Then
                Dim adoption As New BiblosDocumentInfo(CurrentResolution.Location.ReslBiblosDSDB, _fileResolution.IdAssumedProposal.Value)
                uscUploadDocumenti.LoadDocumentInfo(adoption, adoption.IsSigned, True, True, True)
            End If
        End If
    End Sub

    Private Sub InitializeDocumentiOmissis(ByVal workflow As TabWorkflow)
        Dim fieldDocumentsOmissis As Guid = CType(ReflectionHelper.GetPropertyCase(_fileResolution, workflow.FieldDocumentsOmissis), Guid)
        If Not fieldDocumentsOmissis.Equals(Guid.Empty) Then
            uscUploadDocumentiOmissis.AddNode(lblDocumentiOmissis.Text, "../Comm/Images/File/Allegati16.gif", FakeNodeValue, False, False)
        End If
    End Sub

    Private Sub InitializeAttachment(ByVal workflow As TabWorkflow)
        Dim fieldAttachment As Integer? = CType(ReflectionHelper.GetPropertyCase(_fileResolution, workflow.FieldAttachment), Integer?)
        If fieldAttachment.HasValue AndAlso fieldAttachment.Value <> 0 Then
            uscUploadAllegati.AddNode(lblAllegati.Text, "../Comm/Images/File/Allegati16.gif", FakeNodeValue, False, False)
        End If
    End Sub

    Private Sub InitializeAttachmentOmissis(ByVal workflow As TabWorkflow)
        Dim fieldAttachmentsOmissis As Guid = CType(ReflectionHelper.GetPropertyCase(_fileResolution, workflow.FieldAttachmentsOmissis), Guid)
        If Not fieldAttachmentsOmissis.Equals(Guid.Empty) Then
            uscUploadAllegatiOmissis.AddNode(lblAllegatiOmissis.Text, "../Comm/Images/File/Allegati16.gif", FakeNodeValue, False, False)
        End If
    End Sub

    Private Sub InitializeAnnexes(ByVal workflow As TabWorkflow)
        Dim fieldAnnexed As Guid = CType(ReflectionHelper.GetPropertyCase(_fileResolution, workflow.FieldAnnexed), Guid)
        If Not fieldAnnexed.Equals(Guid.Empty) Then
            uscUploadAnnexes.AddNode(lblAnnexes.Text, "../Comm/Images/File/Allegati16.gif", FakeNodeValue, True, False)
        End If
    End Sub

    Private Sub InitializePrivacyAttachment(ByVal workflow As TabWorkflow)
        Dim fieldAttachment As Integer? = CType(ReflectionHelper.GetPropertyCase(_fileResolution, workflow.FieldPrivacyAttachment), Integer?)
        If fieldAttachment.HasValue AndAlso fieldAttachment.Value <> 0 Then
            uscUploadPrivacyAttachment.AddNode(lblPrivacyAttachment.Text, "../Comm/Images/File/Allegati16.gif", FakeNodeValue, False, False)
        End If
    End Sub

    Private Shared Sub DuplicaDocumento(ByRef idCatena As Integer, documentControl As uscDocumentUpload, ByVal resl As Resolution, ByVal append As Boolean, ByVal onlySigned As Boolean, ByVal signature As String)
        If Not idCatena > 0 Then
            Throw New DocSuiteException("IdCatena errato")
        End If

        Dim documents As List(Of BiblosDocumentInfo) = (From biblosDocumentInfo1 In BiblosDocumentInfo.GetDocuments(resl.Location.ReslBiblosDSDB, idCatena) Where Not onlySigned OrElse biblosDocumentInfo1.IsSigned).ToList()
        AddDocumentInfos(documents, documentControl, append, signature)
        idCatena = 0
    End Sub

    Private Shared Sub DuplicaDocumento(ByRef guidCatena As Guid, documentControl As uscDocumentUpload, ByVal resl As Resolution, ByVal append As Boolean, ByVal onlySigned As Boolean, ByVal signature As String)
        If guidCatena = Guid.Empty Then
            Throw New DocSuiteException("Guid Catena non valorizzato")
        End If

        Dim documents As List(Of BiblosDocumentInfo) = (From biblosDocumentInfo1 In BiblosDocumentInfo.GetDocuments(guidCatena) Where Not onlySigned OrElse biblosDocumentInfo1.IsSigned).ToList()
        AddDocumentInfos(documents, documentControl, append, signature)

        guidCatena = Guid.Empty
    End Sub

    ''' <summary> Aggiunge nodi inerenti i DocumentInfo al nodo padre indicato </summary>
    ''' <remarks> Usa un modo custom per la creazione del nodo, occhio! Rimuovere se possibile la necessità di creare un file temporaneo </remarks>
    Private Shared Sub AddDocumentInfos(documentList As List(Of BiblosDocumentInfo), documentControl As uscDocumentUpload, append As Boolean, ByVal signature As String)
        Dim index As Integer = 0
        For Each document As BiblosDocumentInfo In documentList
            ' Creo la copia del documento preesistente
            Dim tempDocument As New TempFileDocumentInfo(document.SaveUniqueToTemp())
            ' Imposto il nome del documento originale come nome da salvare in biblos
            tempDocument.Name = document.Name
            'Imposto la signature se è stata passata
            If Not String.IsNullOrEmpty(signature) Then tempDocument.Signature = signature
            If document.Attributes.Any(Function(f) f.Key.Equals(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE)) Then
                tempDocument.Attributes.Add(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE, document.Attributes.Item(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE))
            End If

            documentControl.LoadDocumentInfoByIndex(tempDocument, False, False, append, True, False, index)
            If Not append Then
                index += 1
            End If
        Next
    End Sub

    Private Function GetSignature(ByVal signatureType As String, ByVal stato As String, Optional resolution As Resolution = Nothing) As String
        Dim signature As String = "*"
        Dim anno As String = ""
        Dim number As String = ""

        '' Gestione altri clienti.. si potrebbe fare meglio...
        If stato.Eq("Adozione") Then
            If txtData.SelectedDate.HasValue Then
                anno = Year(txtData.SelectedDate.Value).ToString
            End If
            If txtNumeroServizio.Text <> "" Then
                number = txtNumeroServizio.Text
            End If
            'EF 20120127 Workaround per avere una generazione corretta della signature in caso di Determina di AUSL-PC
            If (ResolutionEnv.Configuration.Eq("AUSL-PC") And Facade.ResolutionFacade.IsManagedProperty("ServiceNumber", CurrentResolution.Type.Id)) Then
                number = ""
            End If
            signature = Facade.ResolutionFacade.SqlResolutionGetNumber(IdResolution, anno, number, txtData.SelectedDate.DefaultString(), True, resolution)
            AddSignatureType(signature, signatureType)

        ElseIf stato.Eq("Documento Adozione") Then
            signature = Facade.ResolutionFacade.SqlResolutionGetNumber(IdResolution, , , , True)
            AddSignatureType(signature, signatureType)

        ElseIf stato.Eq("Pubblicazione") AndAlso uscUploadDocumenti.ButtonFrontespizioEnabled = True Then
            signature = Facade.ResolutionFacade.SqlResolutionGetNumber(IdResolution, , , , True)
            Select Case signatureType
                Case "A"
                    signature &= " (Allegato)"
                Case "AR"
                    signature &= " (Allegato Riservato)"
                Case "AN"
                    signature &= " (Annesso)"
                Case "DO"
                    signature &= " (Documento Omissis)"
                Case "AO"
                    signature &= " (Allegato Omissis)"
                Case Else
                    signature &= String.Format(": Inserimento Albo {0:dd/MM/yyyy}", txtData.SelectedDate)
            End Select

        ElseIf (stato.Eq("FrontalinoEsecutività") OrElse stato.Eq("Esecutività")) AndAlso ResolutionEnv.Configuration.Eq("AUSL-PC") Then
            signature = Facade.ResolutionFacade.SqlResolutionGetNumber(IdResolution, , , , True)
            AddSignatureType(signature, signatureType)

        ElseIf stato.Eq("Pubblicazione") OrElse stato.Eq("Esecutività") Then
            signature = Facade.ResolutionFacade.SqlResolutionGetNumber(IdResolution, , , , True)
            '**** DG - 2011-09-06 - Per ASL3 TO va tolto l'inserimento della data di scadenza
            Dim azienda As String = DocSuiteContext.Current.ResolutionEnv.Configuration
            If ResolutionEnv.ExpirationDateSignatureEnabled AndAlso CurrentResolution.Type.Description.Eq("Delibera") AndAlso stato.Eq("Pubblicazione") Then
                signature &= " Data Scad. " & txtData.SelectedDate.Value.AddDays(ResolutionEnv.PublicationEndDays).ToString("dd/MM/yyyy")
            End If
            AddSignatureType(signature, signatureType)

        ElseIf stato = "Ritiro Pubblicazione" Then
            signature = Facade.ResolutionFacade.SqlResolutionGetNumber(IdResolution, , , , True)
            AddSignatureType(signature, signatureType)
            If uscUploadDocumenti.ButtonFrontespizioEnabled And signatureType = "D" Then
                signature += ": Ritiro Albo " + String.Format("{0:dd/MM/yyyy}", txtData.SelectedDate)
            End If
        End If

        If Not signature.Contains("*") Then
            ' Inserimento acronimo aziendale per la signature
            Select Case ResolutionEnv.SignatureType
                Case 0
                Case 1
                    signature = ResolutionEnv.CorporateAcronym & " " & signature
            End Select
        End If

        Return signature
    End Function

    Private Shared Sub AddSignatureType(ByRef signature As String, ByVal signatureType As String)
        Select Case signatureType
            Case "A"
                signature &= " (Allegato)"
            Case "AR"
                signature &= " (Allegato Riservato)"
            Case "AN"
                signature &= " (Annesso)"
            Case "DO"
                signature &= " (Documento Omissis)"
            Case "AO"
                signature &= " (Allegato Omissis)"
        End Select
    End Sub

    ''' <summary> Inserisce i documenti in biblos </summary>
    ''' <returns> 0 se non vengono passati documenti, stessa catena in ingresso se viene chiesto di appendere, nuova catena se non si appende </returns>
    Private Function InsertFileToBiblos(docs As IList(Of DocumentInfo), ByVal idCatena As Integer, ByRef signatureType As String, ByVal append As Boolean, ByVal stato As String, ByRef signature As String) As Integer
        If docs.Count <= 0 Then
            Return 0
        End If

        ' controllo se sono in aggiunta della catena vecchia o se devo creare una catena nuova
        Dim newCatena As Integer
        Dim biblosAction As String
        If Not append Then
            newCatena = 0
            biblosAction = "creata"
        Else
            newCatena = idCatena
            biblosAction = "modificata"
        End If

        ' Ottengo la Signature
        signature = GetSignature(signatureType, stato)

        Dim locationFacade As New LocationFacade("ReslDB")
        Dim location As Location = locationFacade.GetById(CInt(txtIdLocation.Text))

        Dim savedDocument As BiblosDocumentInfo
        For Each doc As DocumentInfo In docs
            doc.Signature = signature
            savedDocument = doc.ArchiveInBiblos(location.ReslBiblosDSDB, newCatena)
            newCatena = savedDocument.BiblosChainId
            If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                Facade.ResolutionFacade.ResolutionInsertedDocumentPrivacyLevel(CurrentResolution, savedDocument, signatureType)
            End If
        Next

        Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RF, String.Format("Catena [{0}] {1} con successo.", newCatena, biblosAction))

        Return newCatena
    End Function

    ''' <summary> Inserisce i documenti in biblos </summary>
    ''' <returns> Guid.Empty se non vengono passati documenti, stessa catena in ingresso se viene chiesto di appendere, nuova catena se non si appende </returns>
    Private Function InsertFileToBiblosWithGuid(docs As IList(Of DocumentInfo), ByVal idCatena As Guid, ByRef signatureType As String, ByVal append As Boolean, ByVal stato As String, ByRef signature As String) As Guid
        If docs.Count <= 0 Then
            Return Guid.Empty
        End If

        ' controllo se sono in aggiunta della catena vecchia o se devo creare una catena nuova
        Dim newCatena As Guid
        Dim biblosAction As String
        If Not append Then
            newCatena = Guid.Empty
            biblosAction = "creata"
        Else
            newCatena = idCatena
            biblosAction = "modificata"
        End If

        ' Ottengo la Signature
        signature = GetSignature(signatureType, stato)

        Dim locationFacade As New LocationFacade("ReslDB")
        Dim location As Location = locationFacade.GetById(CInt(txtIdLocation.Text))

        Dim savedDocument As BiblosDocumentInfo
        For Each doc As DocumentInfo In docs
            doc.Signature = signature
            savedDocument = doc.ArchiveInBiblos(location.ReslBiblosDSDB, newCatena)
            newCatena = savedDocument.ChainId

            If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                Facade.ResolutionFacade.ResolutionInsertedDocumentPrivacyLevel(CurrentResolution, savedDocument, signatureType)
            End If
        Next

        Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RF, String.Format("Catena [{0}] {1} con successo", newCatena, biblosAction))

        Return newCatena
    End Function

    Public Function PublishToSharePoint() As Boolean
        If Not CurrentResolution.WebPublicationDate.HasValue Then
            Dim signature As String = String.Format("{0} {1}: Inserimento Albo {2:dd/MM/yyyy}", ResolutionEnv.CorporateAcronym, Facade.ResolutionFacade.SqlResolutionGetNumber(IdResolution, complete:=True), txtData.SelectedDate)

            Try
                Dim tw As TabWorkflow = Facade.TabWorkflowFacade.GetByResolution(CurrentResolution.Id)
                Dim fileRes As FileResolution = Facade.FileResolutionFacade.GetByResolution(CurrentResolution)(0)
                Dim docChain As Integer = Convert.ToInt32(ReflectionHelper.GetPropertyCase(fileRes, tw.FieldDocument))
                Dim description As String = Service.GetDocumentName(CurrentResolution.Location.ReslBiblosDSDB, docChain, 0)

                Dim strXmlDoc As String = CurrentResolutionWPFacade.GetXMLSPFrontespizio(
                uscUploadDocumenti.DocumentInfos.ToDictionary(Function(d) DirectCast(d, TempFileDocumentInfo).FileInfo.Name, Function(d) d.Serialized),
                CurrentResolution,
                tw,
                fileRes,
                description,
                signature)

                Dim strXmlOther As String = CurrentResolutionWPFacade.GetXmlOther(CurrentResolution)

                Common.SharePointFacade.Publish(CurrentResolution, txtData.SelectedDate.Value, txtData.SelectedDate.Value.AddDays(15), signature, strXmlDoc, strXmlOther)
            Catch ex As Exception
                FileLogger.Warn(LoggerName, "Errore Pubblicazione internet.", ex)
                AjaxAlert("Errore Pubblicazione internet: " + ex.Message())
                Return False
            End Try
        End If
        Return True
    End Function

    Public Function RetireFromSharePoint() As Boolean
        If Not CurrentResolution.WebRevokeDate.HasValue Then
            Try
                Dim signature As String = String.Format("{0}: Ritiro Albo {1:dd/MM/yyyy}", Facade.ResolutionFacade.SqlResolutionGetNumber(IdResolution, complete:=True), txtData.SelectedDate)
                Dim strXmlDoc As String = CurrentResolutionWPFacade.GetXmlSp(uscUploadDocumenti.DocumentInfos, signature, CurrentResolution.Id).InnerXml
                Dim strXmlOther As String = Facade.ResolutionWPFacade.GetXmlOther(CurrentResolution)

                Common.SharePointFacade.Retire(CurrentResolution, txtData.SelectedDate.Value, signature, strXmlDoc, strXmlOther)
            Catch ex As Exception
                FileLogger.Warn(LoggerName, "Errore Pubblicazione Ritiro internet: " & ex.Message, ex)
                AjaxAlert("Errore Pubblicazione Ritiro internet: " & ex.Message)
                Return False
            End Try
        End If
        Return True
    End Function

    Public Sub DeleteFromSharePoint(ByVal isPublish As Boolean)
        Try
            If isPublish Then
                If Not String.IsNullOrEmpty(CurrentResolution.WebSPGuid) Then
                    ServiceSHP.DeleteFileInPubblicationArea(CurrentResolution.WebSPGuid)
                End If

                CurrentResolution.WebState = Resolution.WebStateEnum.None
                CurrentResolution.WebRevokeDate = Nothing
                CurrentResolution.WebPublicationDate = Nothing
                CurrentResolution.WebSPGuid = Nothing
            Else
                If Not String.IsNullOrEmpty(CurrentResolution.WebSPGuid) Then
                    ServiceSHP.DeleteFileInRetireArea(CurrentResolution.WebSPGuid)
                End If

                ' Imposto i valori della pubblicazione
                CurrentResolution.WebState = Resolution.WebStateEnum.Published
                CurrentResolution.WebRevokeDate = Nothing
                CurrentResolution.WebSPGuid = Nothing

            End If

            Try
                Facade.ResolutionFacade.Update(CurrentResolution)
                Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.WP, "Ritiro Internet avvenuto correttamente")
            Catch ex As Exception
                FileLogger.Warn(LoggerName, ex.Message, ex)
                AjaxAlert("Errore Pubblicazione\Ritiro internet: Impossibile aggiornare i dati sul database")
                Exit Sub
            End Try

        Catch ex As Exception
            FileLogger.Warn(LoggerName, ex.Message, ex)
            AjaxAlert("Errore Pubblicazione\Ritiro internet: " + ex.Message)
            Exit Sub
        End Try
    End Sub

    Private Shared Function HasDocument(ByVal tvw As RadTreeView, ByVal checkOnlyDimension As Boolean) As Boolean
        If tvw.Nodes(0).Nodes.Count <= 0 Then Return False

        ' Con false il metodo di verifica è retrocompatibile.
        ' Con il parametro a true, verifica solo l'effettiva presenza di voci.
        If checkOnlyDimension Then Return True

        Return tvw.Nodes(0).Nodes.Cast(Of RadTreeNode)().Any(Function(tn) String.IsNullOrEmpty(tn.Value))
    End Function

    Private Function HasOnlyRealDocuments(ByVal tvw As RadTreeView) As Boolean
        If tvw.Nodes(0).Nodes.Count <= 0 Then
            Return False
        End If

        For Each tn As RadTreeNode In tvw.Nodes(0).Nodes
            'Se trovo un Value "" significa che ho già una catena con frontalino, quindi non devo rigenerarlo
            If String.IsNullOrEmpty(tn.Value) Then
                Return False
            End If
        Next tn
        'Se tutti gli allegati sono <> da "" vuol dire che sono allegati veri da soli e non è mai stato creato frontalino
        Return True
    End Function

    Private Sub SetOptionsVisibility(ByVal value As Boolean)
        If value Then
            pnlOptions.Attributes.Remove("style")
        Else
            WebUtils.ObjAttDisplayNone(pnlOptions)
        End If
    End Sub

    Private Sub ServiceNumberCheck()
        If pnlNumeroServizio.Visible Then
            Dim annoCorrente As Integer = Year(Date.Now)
            If txtData.SelectedDate.HasValue Then
                annoCorrente = Year(txtData.SelectedDate.Value)
            End If

            If CurrentResolution.Type.Id.Equals(ResolutionType.IdentifierDelibera) OrElse (CurrentResolution.Type.Id.Equals(ResolutionType.IdentifierDetermina) AndAlso ResolutionEnv.IncrementalNumberEnabled) Then
                txtNumeroServizio.Text = Facade.ResolutionFacade.GetNextFreeServiceNumber(IdResolution, annoCorrente, CurrentResolution.Type.Id).ToString()
            Else
                '' Calcolo il numero di servizio successivo sse non ho il valore già impostato
                '' (se il campo è bloccato infatti conterrà, in fase di modifica, tutto il codice completo)
                If Not String.IsNullOrEmpty(ddlServizio.SelectedValue) AndAlso ddlServizio.Items.Count > 0 Then
                    Dim serviceCode As String = Facade.RoleFacade.GetById(CType(ddlServizio.SelectedValue, Integer)).ServiceCode.ToString()
                    txtNumeroServizio.Text = Facade.ResolutionFacade.GetNextFreeServiceNumber(IdResolution, annoCorrente, serviceCode, Nothing).ToString()
                End If
            End If
        Else
            txtNumeroServizio.Text = String.Empty
        End If
    End Sub

    ''' <summary> Dizionario dei file nella collezione, in chiave il nome del file nella temp (univoco), in valore il nome del file a video. </summary>
    ''' <returns> Dictionary dei file nella TreeView </returns>
    Public Shared Function GetDictionaryFromTreeView(nodes As RadTreeNodeCollection) As Dictionary(Of String, String)
        Dim items As IDictionary(Of String, String) = New Dictionary(Of String, String)
        For Each n As RadTreeNode In nodes
            If Not String.IsNullOrEmpty(n.Value) Then
                items.Add(n.Value, n.Text)
            End If
        Next
        Return items
    End Function

    Private Sub ComposeDocument(ByVal changeFields As String, ByVal fileRes As FileResolution, ByRef idCatena As Integer)
        Dim s As String = Mid(changeFields, InStr(UCase(changeFields), UCase("ComposeDoc")))
        s = Mid(s, InStr(s, "[") + 1)
        s = Mid(s, 1, InStr(s, "]") - 1)

        Dim sFile() As String = Split(s, "+")
        Dim idCatenaTo As Integer = CType((0 & ReflectionHelper.GetPropertyCase(fileRes, sFile(0))), Integer)
        For i As Integer = 1 To UBound(sFile)
            Dim idCatenaFrom As Integer = CType((0 & ReflectionHelper.GetPropertyCase(fileRes, sFile(i))), Integer)
            If (idCatenaFrom <> 0) And (idCatenaTo <> 0) Then
                AddToEndDocument(idCatenaFrom, idCatenaTo)
            End If
        Next i
        idCatena = idCatenaTo
    End Sub

    ''' <summary> Accoda i documenti presenti nella catena lIdCatenaFrom alla catena lIdCatenaTo </summary>
    ''' <param name="idCatenaFrom">Catena di lettura</param>
    ''' <param name="idCatenaTo">Catena di scrittura</param>
    Private Sub AddToEndDocument(ByVal idCatenaFrom As Integer, ByRef idCatenaTo As Integer)
        Dim location As Location = Facade.LocationFacade.GetById(Integer.Parse(txtIdLocation.Text))

        Dim loc As New UIDLocation() With {.Archive = location.ReslBiblosDSDB}

        Dim sources As List(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(New UIDChain(loc, idCatenaFrom))

        For i As Integer = 0 To sources.Count - 1
            Dim doc As BiblosDocumentInfo = sources(i)
            doc.ArchiveInBiblos(location.ReslBiblosDSDB, idCatenaTo)
        Next
    End Sub

    Private Shared Sub SetAdoptionRole(ByRef resl As Resolution, ByVal role As String)
        Dim idRole As Short
        If resl.AdoptionRoleId Is Nothing AndAlso Not String.IsNullOrEmpty(role) AndAlso Short.TryParse(role, idRole) AndAlso idRole <> 0 Then
            resl.AdoptionRoleId = idRole
            FacadeFactory.Instance.ResolutionFacade.UpdateNoLastChange(resl)
        End If
    End Sub

    Private Shared Sub CheckAdoptionDate(ByVal action As String, ByVal workStep As TabWorkflow, ByRef radDatePicker As RadDatePicker)
        If Not workStep.Description.Eq(WorkflowStep.ADOZIONE) OrElse action.Eq("delete") Then
            Exit Sub
        End If

        For Each checkAdoptionDateCommand As String In DocSuiteContext.Current.ResolutionEnv.CheckAdoptionDateCommands
            If checkAdoptionDateCommand.Eq("ForbidFutureDates") Then
                radDatePicker.MaxDate = Now.Date
            End If
        Next
    End Sub

    Private Function ValidateAdoptionDate(ByVal serviceCode As String, ByVal workflowManagedData As String) As Boolean
        Dim compareEnable As Boolean = TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowManagedData, "Date", "INS", "Compare")
        If Not compareEnable Then
            Return True
        End If

        Dim selectedDate As Date = Date.MinValue
        If txtData.SelectedDate.HasValue Then
            selectedDate = txtData.SelectedDate.Value
        End If

        Dim checkAdoptionDate As Boolean = Facade.ResolutionFacade.CheckPreviousResolutionAdoptionDateMinus(CurrentResolution, selectedDate, serviceCode)
        If Not checkAdoptionDate Then
            Return False
        End If
        Return True
    End Function

    Private Function PublishToOnlineRegister() As Boolean
        Try
            CurrentResolution.WebState = Resolution.WebStateEnum.Published
            CurrentResolution.WebPublicationDate = txtData.SelectedDate.Value
            Facade.ResolutionFacade.Update(CurrentResolution)

            Return True
        Catch ex As Exception

        End Try

    End Function

    Private Function GetPubblicationDocuments(ByVal resl As Resolution, ByVal forcedName As String) As IList(Of WebDoc)
        'WebDocument contiene i documenti che verranno restituiti per l'atto
        Dim webDocuments As New List(Of WebDoc)
        If resl Is Nothing Then
            Return webDocuments
        End If

        'Definisco la variabile che discrimina tra contenitore standard e contenitore privacy
        Dim isContainerPrivacy As Boolean = Convert.ToBoolean(resl.Container.Privacy.GetValueOrDefault(0))
        Dim tabWorkflow As TabWorkflow = Facade.TabWorkflowFacade.GetTabWorkflow(resl, DocSuiteContext.Current.ResolutionEnv.WebDocumentSourceWorkflowStep)
        If Not isContainerPrivacy Then
            'Situazione normale
            'Carico il documento principale standard
            webDocuments.AddRange(Facade.ResolutionFacade.GetPubblicationDocuments(resl.Location, resl.File, tabWorkflow.FieldDocument, forcedName, True))

            'Carico gli allegati standard
            webDocuments.AddRange(Facade.ResolutionFacade.GetPubblicationDocuments(resl.Location, resl.File, tabWorkflow.FieldAttachment, forcedName, True))
        Else
            'Situazione privacy
            'Carico il documento principale Omissis
            webDocuments.AddRange(Facade.ResolutionFacade.GetPubblicationDocuments(resl.Location, resl.File, tabWorkflow.FieldDocumentsOmissis, forcedName, True))

            'Carico gli allegati Omissis
            webDocuments.AddRange(Facade.ResolutionFacade.GetPubblicationDocuments(resl.Location, resl.File, tabWorkflow.FieldAttachmentsOmissis, forcedName, False))
        End If

        Return webDocuments
    End Function

    Private Function IsOneDocumentChecked(ByVal nodes As RadTreeNodeCollection) As Boolean
        For Each node As RadTreeNode In nodes
            If node.Checked Then
                Return True
            End If
        Next
        Return False
    End Function

    Private Sub InitDocumentsPrivacyLevels()
        Dim minLevel As Integer = 0
        Dim visibility As Boolean = False
        If CurrentResolution IsNot Nothing AndAlso CurrentResolution.Container IsNot Nothing AndAlso CurrentResolution.Container.PrivacyEnabled Then
            minLevel = CurrentResolution.Container.PrivacyLevel
            visibility = True
        End If

        Select Case StepDescription
            Case WorkflowStep.PUBBLICAZIONE, WorkflowStep.ESECUTIVA
                minLevel = 0
                Exit Select
        End Select

        If PnlDocumentoVisible Then
            uscUploadDocumenti.MinPrivacyLevel = minLevel
            uscUploadDocumenti.ButtonPrivacyLevelVisible = visibility
        End If
        If PnlAttachVisible Then
            uscUploadAllegati.MinPrivacyLevel = minLevel
            uscUploadAllegati.ButtonPrivacyLevelVisible = visibility
        End If
        If PnlAnnexesVisible Then
            uscUploadAnnexes.MinPrivacyLevel = minLevel
            uscUploadAnnexes.ButtonPrivacyLevelVisible = visibility
        End If
    End Sub
#End Region

End Class