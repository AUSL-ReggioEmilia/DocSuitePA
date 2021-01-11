Imports System.Linq
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports System.Web.Services.Protocols
Imports System.IO
Imports Telerik.Web.UI
Imports iTextSharp.text.exceptions
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Biblos.Models

Partial Public Class AuslPc_ReslPubblicaRevoca
    Inherits ReslBasePage

#Region " Fields "
    Private Const FakeNodeValue As String = "meta"

    Private ViewReslType As Short
    Private Action As String
    Private MyStep As Short

    Private _fileResolution As FileResolution
    Private _pnlDocumentoVisible As Boolean = True
    Private _pnlAttachVisible As Boolean = True
    Private _pnlPrivacyAttachmentVisible As Boolean = True
    Private _pnlAnnexesVisible As Boolean = True
    Private _pnlOptionsVisible As Boolean
    Private _pnlPrivacyVisible As Boolean

    Private _newPublicationNumber As Integer = -1
    Private _resolutionDocumentSeriesItemFacade As Facade.NHibernate.Resolutions.ResolutionDocumentSeriesItemFacade
#End Region

#Region " Properties "

    Private Property StepDescription As String
        Get
            Return CType(ViewState("StepDesc"), String)
        End Get
        Set(value As String)
            ViewState("StepDesc") = value
        End Set
    End Property

    Private Property FileResolution As FileResolution
        Get
            If (_fileResolution Is Nothing) Then _fileResolution = Facade.FileResolutionFacade.GetByResolution(CurrentResolution)(0)
            Return _fileResolution
        End Get
        Set(ByVal value As FileResolution)
            _fileResolution = value
        End Set
    End Property

    Private Property PnlOptionsVisible() As Boolean
        Get
            Return _pnlOptionsVisible
        End Get
        Set(ByVal value As Boolean)
            _pnlOptionsVisible = value
            If value Then
                pnlOptions.Attributes.Remove("style")
            Else
                WebUtils.ObjAttDisplayNone(pnlOptions)
            End If
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

    Private Property PnlPrivacyAttachmentVisible() As Boolean
        Get
            Return _pnlPrivacyAttachmentVisible
        End Get
        Set(ByVal value As Boolean)
            _pnlPrivacyAttachmentVisible = value
            If value Then
                pnlPrivacyAttachment.Attributes.Remove("style")
            Else
                WebUtils.ObjAttDisplayNone(pnlPrivacyAttachment)
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
    Public ReadOnly Property CurrentResolutionDocumentSeriesItemFacade As Facade.NHibernate.Resolutions.ResolutionDocumentSeriesItemFacade
        Get
            If _resolutionDocumentSeriesItemFacade Is Nothing Then
                _resolutionDocumentSeriesItemFacade = New Facade.NHibernate.Resolutions.ResolutionDocumentSeriesItemFacade()
            End If
            Return _resolutionDocumentSeriesItemFacade
        End Get
    End Property
#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False

        ViewReslType = Request.QueryString("ReslType")
        Action = UCase(Request.QueryString("Action"))
        MyStep = Request.QueryString("Step")
        WebUtils.ObjAttDisplayNone(txtLastWorkflowDate)
        WebUtils.ObjAttDisplayNone(txtIdLocation)

        InitializePrivacy()

        PnlDocumentoVisible = Not (pnlDocumento.Attributes("style") = "display:none;")
        PnlOptionsVisible = Not (pnlOptions.Attributes("style") = "display:none;")
        PnlPrivacyVisible = Not (pnlPrivacy.Attributes("style") = "display:none;")
        PnlAttachVisible = Not (pnlAttach.Attributes("style") = "display:none;")
        PnlPrivacyAttachmentVisible = Not (pnlPrivacyAttachment.Attributes("style") = "display:none;")
        PnlAnnexesVisible = Not (pnlAnnexes.Attributes("style") = "display:none;")

        InitializeAjax()

        If DocSuiteContext.Current.PrivacyLevelsEnabled Then
            InitDocumentsPrivacyLevels()
        End If

        If Not Page.IsPostBack Then
            If Not Page.IsCallback AndAlso Not CommonShared.VerifyChkQueryString(Request.QueryString, True) Then
                Exit Sub
            End If
            uscUploadDocumenti.SignButtonEnabled = ResolutionEnv.IsFDQEnabled
            Initialize()
        End If
    End Sub

    Private Sub uscUploadDocumenti_ButtonFrontespizioClick(ByVal sender As Object, ByVal e As EventArgs) Handles uscUploadDocumenti.ButtonFrontespizioClick
        If Action.Eq("REVOKE") Then
            Dim adozioneWorkStep As TabWorkflow = Facade.TabWorkflowFacade.GetByDescription("Adozione", CurrentResolution.WorkflowType)
            MyStep = adozioneWorkStep.Id.ResStep
            Try
                _newPublicationNumber = revoke()
            Catch ex As Exception
                AjaxAlert("Errore in fase di revoca della pubblicazione: " & ex.Message)
            End Try
        End If
        Try
            ' Verifico che la data sia impostata.
            If txtData.SelectedDate.HasValue Then
                Dim fi As FileInfo = ResolutionUtil.GeneraFrontalino(txtData.SelectedDate.Value, CurrentResolution, StepDescription, MyStep, String.Empty)
                Dim doc As New TempFileDocumentInfo(fi)
                doc.Name = "Relata di Pubblicazione.pdf"
                doc.AddAttribute(BiblosFacade.DOCUMENT_POSITION_ATTRIBUTE, 0)

                uscUploadDocumenti.LoadDocumentInfo(doc, False, False, False, True)

                'Una volta generato il documento deve essere bloccato il campo data
                txtData.Enabled = False
                uscUploadDocumenti.ButtonFrontespizioEnabled = False
                uscUploadDocumenti.ButtonFileEnabled = False
                uscUploadDocumenti.ButtonRemoveEnabled = False
                uscUploadDocumenti.ButtonScannerEnabled = False
                uscUploadDocumenti.ButtonLibrarySharepointEnabled = False
                If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                    uscUploadDocumenti.ButtonPrivacyLevelVisible = false
                End If
                'Attivo il bottone per aprire il pannello privacy
                If (_newPublicationNumber > 0) Then
                    PnlOptionsVisible = True
                    selectPrivacy.Items.FindByText("No").Enabled = False
                End If
            Else
                rfvData.Validate()
            End If
        Catch ex As Exception
            FileLogger.Error(LoggerName, "Errore in fase di generazione frontalino", ex)
            AjaxAlert("Errore in fase di generazione del frontalino: " & ex.Message)
        End Try
    End Sub

    Protected Sub btnConferma_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConferma.Click
        If CurrentResolution IsNot Nothing Then
            Dim b As Boolean
            Dim howManyNextSteps As Integer = 0
            Dim workStep As TabWorkflow
            Dim idDocumento As Integer = -1
            Dim idAllegati As Integer = -1
            Dim idPrivacyAttachment As Integer = -1
            Dim operationStep As Integer
            Dim idAnnexes As Guid = Guid.Empty

            ' Selezione Workflow
            Select Case Action
                Case "REVOKE"
                    workStep = Facade.TabWorkflowFacade.GetByDescription("Pubblicazione", CurrentResolution.WorkflowType)
                    b = True
                    operationStep = workStep.Id.ResStep
                Case "NEXT"
                    b = Facade.TabWorkflowFacade.GetByStep(CurrentResolution.WorkflowType, MyStep + 1S, workStep)
                    operationStep = MyStep + 1
            End Select

            ''Se il documento principale non è stato caricato, allora lo carico
            If (FileResolution.IdResolutionFile Is Nothing AndAlso DocSuiteContext.Current.ResolutionEnv.UseSharepointPublication) Then
                Dim idResolutionFileDocumentInfo As DocumentInfo = uscUploadDocumenti.DocumentInfosAdded(0)
                idResolutionFileDocumentInfo.Signature = String.Format(" {0} del {1}", CurrentResolution.InclusiveNumber, String.Format("{0:dd/MM/yyyy}", Now))
                idResolutionFileDocumentInfo = idResolutionFileDocumentInfo.ArchiveInBiblos(CurrentResolution.Location.ReslBiblosDSDB)
                Facade.ResolutionFacade.SqlResolutionDocumentUpdate(CurrentResolution.Id, CType(idResolutionFileDocumentInfo, BiblosDocumentInfo).BiblosChainId, ResolutionFacade.DocType.Disposizione)
            End If

            ''Se ci sono documenti privacy da caricare allora li carico
            If uscPrivacyPanel.PrivacyDocumentVisible AndAlso DocSuiteContext.Current.ResolutionEnv.UseSharepointPublication Then
                uscPrivacyPanel.SavePrivacyDocument(ResolutionFacade.DocType.PrivacyPublicationDocument)
            End If

            ' Pubblicazione Piacenza
            Try
                VerifyFrontalino()
                If b AndAlso ((workStep.Description.Eq(WorkflowStep.PUBBLICAZIONE) AndAlso Not Action.Eq("DELETE")) OrElse Action.Eq("REVOKE")) Then
                    Dim currentPublicationNumber As Long = Facade.ResolutionFacade.GetLastOrNewPublicationNumber(CurrentResolution)
                    Try
                        If Action.Eq("REVOKE") Then Facade.ResolutionFacade.EndWebRevoca(CurrentResolution, currentPublicationNumber)
                        b = Publicate()
                    Catch ex As Exception
                        AjaxAlert("Sono occorsi alcuni problemi in fase di revoca/pubblicazione: " & ex.Message)
                    End Try
                End If
            Catch ex As Exception
                b = False
                AjaxAlert(ex.Message)
            End Try

            ' Resolution e ResolutionFile
            If b Then
                If Not Action.Eq("DELETE") Then
                    Dim DataFlusso As String = If(pnlData.Visible, String.Format("{0:dd/MM/yyyy}", txtData.SelectedDate), "N")

                    b = Facade.ResolutionFacade.SqlResolutionUpdateWorkflowData(IdResolution, ViewReslType, Not CurrentResolution.Number.HasValue, workStep, DataFlusso, "N", idAllegati, idDocumento, Guid.Empty, "N", True, DocSuiteContext.Current.User.FullUserName)
                    Dim append As Boolean = False

                    'Verifica se il documento è firmato
                    If ResolutionEnv.CheckPublishDocumentSigned AndAlso PnlDocumentoVisible And b AndAlso Not Action.Eq("MODIFY") Then
                        If uscUploadDocumenti.HasDocuments() Then
                            If FileResolution.IdProposalFile.HasValue Then
                                Dim proposta As New BiblosDocumentInfo(CurrentResolution.Location.ReslBiblosDSDB, FileResolution.IdProposalFile.Value)
                                If proposta.IsSigned() Then
                                    Dim resolutionWorkflow As TabWorkflow = Facade.TabWorkflowFacade.GetByResolution(CurrentResolution.Id)
                                    'Carico sullo stesso oggetto lo step successivo
                                    Facade.TabWorkflowFacade.GetByStep(resolutionWorkflow.Id.WorkflowType,
                                                                       resolutionWorkflow.Id.ResStep + 1S,
                                                                       resolutionWorkflow)

                                    Dim fileEffettivo As DocumentInfo = uscUploadDocumenti.DocumentInfos.First()
                                    If Not fileEffettivo.IsSigned() Then
                                        b = False
                                        AjaxAlert("Il documento deve essere firmato")
                                        Exit Sub
                                    End If
                                End If
                            End If
                        End If
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
                                DuplicaDocumento(idAllegati, uscUploadAllegati, CurrentResolution, app)
                                append = True
                            Catch ex As Exception
                                Facade.ResolutionLogFacade.Log(CurrentResolution, errorType, String.Format("ERR.ATTI.STEP.{1}: Errore in fase di duplicazione Allegati: {0}.", ex.Message, workStep.Description.ToUpper()))
                                AjaxAlert("ATTENZIONE!!{0} {1}", Environment.NewLine, ex.Message)
                                Exit Sub
                            End Try
                        End If

                        InsertFileToBiblos(uscUploadAllegati.DocumentInfosAdded, idAllegati, "A", append, workStep.Description)
                        b = True
                    End If

                    ' Annessi
                    If PnlAnnexesVisible Then
                        idAnnexes = ReflectionHelper.GetPropertyCase(CurrentResolution.File, workStep.FieldAnnexed)
                        ' se sono in un finto nodo riaggiungo per la modifica i documenti
                        If uscUploadAnnexes.HasDocuments AndAlso uscUploadAnnexes.GetNodeValue(0).Eq(FakeNodeValue) Then
                            Dim app As Boolean = False
                            Dim errorType As ResolutionLogType = ResolutionLogType.RX
                            If workStep.Description.Eq(WorkflowStep.ADOZIONE) Then
                                app = True
                            End If

                            Try
                                DuplicaDocumento(idAnnexes, uscUploadAnnexes, CurrentResolution, True)
                                append = True
                            Catch ex As Exception
                                Facade.ResolutionLogFacade.Log(CurrentResolution, errorType, String.Format("ERR.ATTI.STEP.{1}: Errore in fase di duplicazione Annessi: {0}", ex.Message, workStep.Description.ToUpper()))
                                AjaxAlert("ATTENZIONE!!{0} {1}", Environment.NewLine, ex.Message)
                                Exit Sub
                            End Try
                        End If

                        InsertFileToBiblosWithGuid(uscUploadAnnexes.DocumentInfosAdded, idAnnexes, "AN", append, workStep.Description)
                        b = True
                    End If

                    If pnlPrivacyAttachment.Visible Then
                        idPrivacyAttachment = 0 & ReflectionHelper.GetPropertyCase(CurrentResolution.File, workStep.FieldPrivacyAttachment)
                        If workStep.Description.Eq(WorkflowStep.ADOZIONE) AndAlso uscUploadPrivacyAttachment.HasDocuments Then
                            ' se sono in un finto nodo riaggiungo per la modifica i documenti
                            If uscUploadPrivacyAttachment.GetNodeValue(0).Eq(FakeNodeValue) Then
                                Try
                                    DuplicaDocumento(idPrivacyAttachment, uscUploadPrivacyAttachment, CurrentResolution, True)
                                    append = True
                                Catch ex As Exception
                                    Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RX, String.Format("ERR.ATTI.STEP.ADOZIONE: Errore in fase di duplicazione: {0}", ex.Message))
                                    AjaxAlert("ATTENZIONE!!{0} {1}", Environment.NewLine, ex.Message)
                                    Exit Sub
                                End Try
                            End If
                        End If

                        InsertFileToBiblos(uscUploadPrivacyAttachment.DocumentInfosAdded, idPrivacyAttachment, "AR", append, workStep.Description)
                        Facade.ResolutionFacade.SqlResolutionDocumentUpdate(CurrentResolution.Id, idPrivacyAttachment, ResolutionFacade.DocType.AllegatiRiservati)
                        b = True
                    End If

                    If Action = "NEXT" Then
                        'Accodamento
                        If StringHelper.InStrTest(workStep.ManagedWorkflowData, "ComposeDoc") And b Then
                            Dim workStepCopy As TabWorkflow = workStep.Clone()
                            Dim fieldDocumentBackup As String = workStepCopy.FieldDocument
                            '' Inserisce il frontespizio corrente
                            b = Facade.ResolutionFacade.SqlResolutionUpdateWorkflowData(IdResolution, ViewReslType, Not CurrentResolution.Number.HasValue, workStepCopy, DataFlusso, "N", idAllegati, idDocumento, idAnnexes, "N", True, DocSuiteContext.Current.User.FullUserName)

                            'EF 20120216 Workaround per forzare il caricamento della catena di documenti in idResolutionFile e gli altri frontalini dove previsto (per AUSL-PC)
                            ' Salvo i vecchi campi
                            fieldDocumentBackup = workStepCopy.FieldDocument
                            workStepCopy.FieldDocument = "idResolutionFile"
                            Dim signature As String = Facade.ResolutionFacade.SqlResolutionGetNumber(IdResolution, , , , True)
                            ResolutionFacade.ComposeDocument(workStepCopy.ManagedWorkflowData, FileResolution, idDocumento, CurrentResolution.Location, signature)
                            ' Ripristino i campi originali
                            workStepCopy.FieldDocument = fieldDocumentBackup
                        End If

                        If b Then
                            b = Facade.ResolutionFacade.SqlResolutionUpdateWorkflowData(IdResolution, ViewReslType, Not CurrentResolution.Number.HasValue, workStep, DataFlusso, "N", idAllegati, idDocumento, Guid.Empty, "N", True, DocSuiteContext.Current.User.FullUserName)
                        End If

                        'Verifico se devo salvare anche lo step successivo
                        Dim lastWorkStep As TabWorkflow = workStep
                        Dim nextStepId As Integer = MyStep + 2
                        While StringHelper.InStrTest(lastWorkStep.ManagedWorkflowData, "NextStep") And b
                            'EF 20120216 Aggiunto per compatibilità tra AUSL-PC e ASMN, in questo modo il valore di default è preservato per ASMN, mentre per AUSL-PC è possibile ottenere il caricamento del file.
                            idDocumento = -1
                            howManyNextSteps = howManyNextSteps + 1
                            Dim workNextStep As TabWorkflow = Nothing
                            ' Recupero il nuovo step
                            b = Facade.TabWorkflowFacade.GetByStep(CurrentResolution.WorkflowType, nextStepId, workNextStep)
                            If b Then
                                Dim data As DateTime = ReflectionHelper.GetProperty(CurrentResolution, lastWorkStep.FieldDate)

                                '' Modifico le date per pubblicazione ed esecutività ASMN
                                If ResolutionEnv.UseSharepointPublication Then
                                    If workNextStep.Description.Trim() = "Pubblicazione" Then
                                        data = data.AddDays(1)
                                    ElseIf workNextStep.Description.Trim() = "Esecutività" Then
                                        data = data.AddDays(15)
                                    End If
                                End If

                                Dim temp As String = If(pnlData.Visible, String.Format("{0:dd/MM/yyyy}", data), "N")
                                If StringHelper.InStrTest(workNextStep.ManagedWorkflowData, "ComposeDoc") And b Then
                                    Dim signature As String = Facade.ResolutionFacade.SqlResolutionGetNumber(IdResolution, , , , True)
                                    ResolutionFacade.ComposeDocument(workNextStep.ManagedWorkflowData, FileResolution, idDocumento, CurrentResolution.Location, signature)
                                End If

                                b = Facade.ResolutionFacade.SqlResolutionUpdateWorkflowData(IdResolution, ViewReslType, Not CurrentResolution.Number.HasValue, workNextStep, temp, "N", -1, idDocumento, Guid.Empty, "N", True, DocSuiteContext.Current.User.FullUserName)

                                If workNextStep.Description = "Pubblicazione" AndAlso ResolutionEnv.UseSharepointPublication Then
                                    ' Pubblicazione WEB
                                    ' Carico lo step precedente alla pubblicazione perchè contiene il template per il frontalino
                                    Facade.TabWorkflowFacade.GetByStep(CurrentResolution.WorkflowType, CType((workNextStep.Id.ResStep - 1), Short), workStep)
                                    PublicateOnSharePoint(CurrentResolution, workStep, True)
                                End If
                            End If
                            lastWorkStep = workNextStep
                            nextStepId = nextStepId + 1
                        End While
                    End If
                End If
            End If

            ' Salvataggio ResolutionWorkflow
            If b Then
                ' Allegati e Documento:
                ' Se non ho salvato su ResolutionFile (se id = -1)
                ' Rileggo il valore originale se esiste
                If (idAllegati = -1) Then
                    If Not String.IsNullOrEmpty(workStep.FieldAttachment) Then
                        idAllegati = 0 & ReflectionHelper.GetPropertyCase(CurrentResolution, workStep.FieldAttachment)
                    Else
                        idAllegati = 0
                    End If
                End If
                If idPrivacyAttachment.Equals(-1) Then
                    If Not String.IsNullOrEmpty(workStep.FieldPrivacyAttachment) Then
                        idPrivacyAttachment = 0 & ReflectionHelper.GetPropertyCase(CurrentResolution, workStep.FieldPrivacyAttachment)
                    Else
                        idPrivacyAttachment = 0
                    End If
                End If
                ' Annessi
                If (idAnnexes = Guid.Empty) Then
                    If Not String.IsNullOrEmpty(workStep.FieldAnnexed) Then
                        idAnnexes = ReflectionHelper.GetPropertyCase(CurrentResolution, workStep.FieldAnnexed)
                    Else
                        idAnnexes = Guid.Empty
                    End If
                End If
                ' Settori
                If workStep.ExistWorkflowData(TabWorkflow.WorkflowField.Role) Then
                    Dim roleId As Integer = workStep.ExtractoWorkflowData(Of Integer)(TabWorkflow.WorkflowField.Role)
                    Facade.ResolutionRoleFacade.AddRole(CurrentResolution, roleId, 1)
                End If
                If idDocumento = -1 Then
                    If Not String.IsNullOrEmpty(workStep.FieldDocument) Then
                        idDocumento = 0 & ReflectionHelper.GetPropertyCase(CurrentResolution, workStep.FieldDocument)
                    Else
                        idDocumento = 0
                    End If
                End If
                ' Inserimento in ResolutionWorkflow
                If Action.Eq("NEXT") Then
                    b = Facade.ResolutionWorkflowFacade.InsertNextStep(IdResolution, MyStep, idDocumento, idAllegati, idPrivacyAttachment, idAnnexes, Guid.Empty, Guid.Empty, DocSuiteContext.Current.User.FullUserName)

                    For index As Short = 1 To howManyNextSteps
                        b = Facade.ResolutionWorkflowFacade.InsertNextStep(IdResolution, MyStep + index, idDocumento, idAllegati, idPrivacyAttachment, idAnnexes, Guid.Empty, Guid.Empty, DocSuiteContext.Current.User.FullUserName)
                    Next
                End If

                'Invio comando di creazione/aggiornamento Resolution alle WebApi
                If Not Action.Eq("DELETE") AndAlso Not workStep.Description.Eq(WorkflowStep.PROPOSTA) Then
                    If Action.Eq("NEXT") AndAlso workStep.Description.Eq(WorkflowStep.ADOZIONE) Then
                        Facade.ResolutionFacade.SendCreateResolutionCommand(CurrentResolution)
                    Else
                        Facade.ResolutionFacade.SendUpdateResolutionCommand(CurrentResolution)
                    End If
                End If
            End If

            If b Then
                CurrentResolutionDocumentSeriesItemFacade.ConfirmAndPublishSeries(CurrentResolution)
                AjaxManager.ResponseScripts.Add("return CloseWindow();")
            End If
        End If
    End Sub

    Protected Sub selectPrivacy_SelectedIndexChanged(sender As Object, e As EventArgs) Handles selectPrivacy.SelectedIndexChanged
        SetPrivacy()
    End Sub

    Protected Sub AuslPcReslPubblicaRevocaAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arguments As String() = Split(e.Argument, "|")
        Select Case arguments(0)
            Case "blockinsert"
                txtData.Enabled = False
                uscUploadDocumenti.ReadOnly = True
                AjaxManager.ResponseScripts.Add("ExecuteAjaxRequest('privacygenerate');")
            Case "privacygenerate"
                ''Verifico che il documento principale sia stato caricato
                If (FileResolution.IdResolutionFile Is Nothing) Then
                    Dim idResolutionFileDocumentInfo As DocumentInfo = uscUploadDocumenti.DocumentInfosAdded(0)
                    idResolutionFileDocumentInfo.Signature = String.Format(" {0} del {1}", CurrentResolution.InclusiveNumber, String.Format("{0:dd/MM/yyyy}", Now))
                    idResolutionFileDocumentInfo = idResolutionFileDocumentInfo.ArchiveInBiblos(CurrentResolution.Location.ReslBiblosDSDB)
                    Facade.ResolutionFacade.SqlResolutionDocumentUpdate(CurrentResolution.Id, CType(idResolutionFileDocumentInfo, BiblosDocumentInfo).BiblosChainId, ResolutionFacade.DocType.Disposizione)
                End If
                uscPrivacyPanel.GeneratePrivacyDocumentToPrint(CurrentResolution.WorkflowType)
                btnConferma.Text = "COMPLETA INSERIMENTO"
            Case "privacyreset"
                uscPrivacyPanel.ResetSelector()
            Case "checkdatatoinsert"
                'Verifico prima la presenza dei dati di base dell'atto e poi di quelli specifici per la privacy
                If uscUploadDocumenti.HasDocuments Then
                    uscPrivacyPanel.CheckPrivacy()
                Else
                    AjaxAlert(String.Format("Impossibile attivare il pannello Privacy.{0}E' necessario caricare 1 documento principale.{0}", Environment.NewLine))
                    uscPrivacyPanel.ResetSelector()
                End If
        End Select
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        CommonInstance.UserDeleteTemp(TempType.I)
        PnlOptionsVisible = False
        PnlPrivacyVisible = False
        rfvPrivacyObject.ErrorMessage = String.Format("{0} obbligatorio", ResolutionEnv.ResolutionObjectPrivacyLabel)

        If CurrentResolution IsNot Nothing Then
            txtIdLocation.Text = CurrentResolution.Location.Id

            Dim workflow As TabWorkflow = Nothing 'drW
            Dim workNext As TabWorkflow = Nothing 'drWNext
            Dim workActive As TabWorkflow = Nothing 'drWActive

            If Not Facade.TabWorkflowFacade.GetByStep(CurrentResolution.WorkflowType, MyStep, workActive) Then
                Exit Sub
            End If


            Select Case Action
                Case "REVOKE"
                    workNext = Facade.TabWorkflowFacade.GetByDescription("Pubblicazione", CurrentResolution.WorkflowType)
                    InitializePanel(workNext.Id.ResStep)
                    StepDescription = workNext.Description

                    '--Descrizioni
                    lblTitolo.Text = "Sost. della Pubblicazione"
                    lblDocumento.Text = "Sost. della Pubblicazione"

                    '--Documento
                    If PnlDocumentoVisible Then
                        InitializeDocument(CurrentResolution, workNext, False)
                    End If
                    uscUploadDocumenti.ButtonFrontespizioEnabled = False

                    '--Data
                    pnlData.Visible = True
                    txtData.SelectedDate = Date.Now

                    ' Pannelli per la pubblicazione Web
                    ' Rimuovo la scelta, si può solo ricaricare da scanner
                    PnlOptionsVisible = False
                    PnlPrivacyVisible = False
                    rfvSelectPrivacy.Enabled = False
                    txtLastWorkflowDate.Text = ReflectionHelper.GetProperty(CurrentResolution, workActive.FieldDate)
                    cvCompareData.ErrorMessage &= txtLastWorkflowDate.Text
                    cvCompareData.Visible = TabWorkflowFacade.TestManagedWorkflowDataProperty(workNext.ManagedWorkflowData, "Date", "INS", "Tested")
                    workflow = workActive
                Case "NEXT"
                    Dim DocumentToDuplicate As Integer = -1
                    If Not Facade.TabWorkflowFacade.GetByStep(CurrentResolution.WorkflowType, MyStep + 1S, workNext) Then Exit Sub

                    StepDescription = workNext.Description

                    Dim s As String = String.Empty
                    s = workNext.CustomDescription

                    If StringHelper.InStrTest(workNext.ManagedWorkflowData, "NextStep") Then
                        Dim workNext2 As TabWorkflow 'drWNext2
                        If Facade.TabWorkflowFacade.GetByStep(CurrentResolution.WorkflowType, MyStep + 2S, workNext2) Then
                            'If workNext2.Description.Substring(0, 1).ToUpper = "E" Then
                            If workNext2.CustomDescription.Substring(0, 1).ToUpper = "E" Then
                                s &= " ed "
                            Else
                                s &= " e "
                            End If
                            's &= workNext2.Description
                            s &= workNext2.CustomDescription
                        End If
                    End If

                    lblTitolo.Text = s & " - Inserimento"

                    lblDocumento.Text = workNext.DocumentDescription
                    If lblDocumento.Text = "" Then 'Prendo la descrizione dello step successivo
                        Dim work2 As TabWorkflow 'drW2
                        If Facade.TabWorkflowFacade.GetByStep(CurrentResolution.WorkflowType, MyStep + 2S, work2) Then
                            lblDocumento.Text = work2.DocumentDescription
                        End If
                    End If

                    'EF 20120229 Workaround per gestire il nome del pannello di generazione del frontalino di esecutività
                    If (lblDocumento.Text = "Documento" And workNext.Description = "Esecutività") Then lblDocumento.Text = "Rel. Esecutività"

                    InitializePanel(MyStep + 1S)

                    '--Documento
                    If PnlDocumentoVisible Then
                        InitializeDocument(CurrentResolution, workNext, True)
                        If DocumentToDuplicate > 0 Then
                            DuplicaDocumento(DocumentToDuplicate, uscUploadDocumenti.TreeViewControl, CurrentResolution)
                        End If
                    End If

                    lblAllegati.Text = "Allegati (parte integrante)"
                    lblAnnexes.Text = "Annessi (non parte integrante)"
                    lblPrivacyAttachment.Text = "Allegati Riservati"

                    '--Allegati
                    If PnlAttachVisible Then
                        InitializeAttachment(CurrentResolution, workActive)
                    End If

                    ' Allegati Riservati
                    If PnlPrivacyAttachmentVisible Then
                        InitializePrivacyAttachment(workActive)
                    End If

                    ' Annessi
                    If PnlAnnexesVisible Then
                        InitializeAnnexes(workActive)
                    End If

                    '--Data stepAttivo
                    Select Case workNext.Description.Trim
                        Case "Pubblicazione"
                            If CurrentResolution.OCSupervisoryBoard AndAlso Not CurrentResolution.SupervisoryBoardWarningDate.HasValue Then
                                ' Manca la data dell'invio al collegio sindacale
                                AjaxManager.ResponseScripts.Add("return CloseWindow('');")
                                AjaxAlert("Manca la data di Invio al Collegio Sindacale. Impossibile pubblicare!")
                                Exit Sub
                            ElseIf CurrentResolution.OCManagement AndAlso Not CurrentResolution.ManagementWarningDate.HasValue Then
                                ' Manca la data dell'invio alla conferenza dei sindaci
                                AjaxManager.ResponseScripts.Add("return CloseWindow('');")
                                AjaxAlert("Manca la data di Invio alla Conferenza dei Sindaci. Impossibile pubblicare!")
                                Exit Sub
                            ElseIf CurrentResolution.OCRegion AndAlso Not CurrentResolution.WarningDate.HasValue Then
                                ' Manca la data dell'invio alla regione
                                AjaxManager.ResponseScripts.Add("return CloseWindow('');")
                                AjaxAlert("Manca la data di Invio alla Regione. Impossibile pubblicare!")
                                Exit Sub
                            End If

                            ' Pannelli per la pubblicazione Web
                            PnlOptionsVisible = True
                            PnlPrivacyVisible = selectPrivacy.SelectedItem IsNot Nothing AndAlso Boolean.Parse(selectPrivacy.SelectedValue)
                            rfvSelectPrivacy.Enabled = True
                            txtLastWorkflowDate.Text = ReflectionHelper.GetProperty(CurrentResolution, workActive.FieldDate)
                            cvCompareData.ErrorMessage &= txtLastWorkflowDate.Text
                            cvCompareData.Visible = TabWorkflowFacade.TestManagedWorkflowDataProperty(workNext.ManagedWorkflowData, "Date", "INS", "Tested")
                    End Select
                    workflow = workNext
            End Select

            MasterDocSuite.HistoryTitle = lblTitolo.Text
        End If
    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(selectPrivacy, pnlPrivacy)
        AjaxManager.AjaxSettings.AddAjaxSetting(selectPrivacy, pnlOptions, MasterDocSuite.AjaxDefaultLoadingPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, txtData)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscPrivacyPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, btnConferma)

        If PnlPrivacyVisible Then
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, documentUploader)
        End If
        If PnlDocumentoVisible Then
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscUploadDocumenti)
        End If

        If PnlAttachVisible Then
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscUploadAllegati)
        End If
        If PnlPrivacyAttachmentVisible Then
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscUploadPrivacyAttachment)
        End If
        If PnlAnnexesVisible Then
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscUploadAnnexes)
        End If

        AddHandler AjaxManager.AjaxRequest, AddressOf AuslPcReslPubblicaRevocaAjaxRequest
    End Sub

    Private Sub InitializePrivacy()
        '' Pannello controllo privacy
        uscPrivacyPanel.UseResolutionNumberDisplay = False
        uscPrivacyPanel.CurrentResolution = CurrentResolution
    End Sub

    ''' <summary>
    ''' Inizializza i pannelli della pagina impostati da parametri del Database (TabWorkflow) in base allo step passato come parametro.
    ''' </summary>
    ''' <param name="ReslStep">numero step</param>
    Private Sub InitializePanel(ByVal reslStep As Short)
        Dim workflowData As String = String.Empty

        If Facade.TabWorkflowFacade.SqlTabWorkflowManagedWData(IdResolution, CurrentResolution.WorkflowType, reslStep, workflowData) Then
            Dim sProp As String = String.Empty
            Select Case Action
                Case "NEXT", "REVOKE" : sProp = "INS"
                Case "MODIFY" : sProp = "MOD"
                Case "DELETE" : sProp = ""
            End Select

            'parametro pannello data visibile
            pnlData.Visible = True
            'parametro data abilitata
            txtData.Enabled = (Action <> "REVOKE")
            'parametro data obbligatoria
            rfvData.Visible = True

            Dim workActive As TabWorkflow = Nothing
            If (MyStep < 5 AndAlso Not Facade.TabWorkflowFacade.GetByStep(CurrentResolution.WorkflowType, MyStep, workActive)) Then Exit Sub

            If TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "Date", "TODAY", "") Then
                txtData.SelectedDate = Date.Today
                Dim workNext As TabWorkflow = Nothing 'WorkflowSuccessivo
                If Facade.TabWorkflowFacade.GetByStep(CurrentResolution.WorkflowType, MyStep + 1, workNext) Then
                    If (TabWorkflowFacade.TestManagedWorkflowDataProperty(workNext.ManagedWorkflowData, "Date", "INS", "Tested")) Then
                        Dim precedentDate As String = ReflectionHelper.GetProperty(CurrentResolution, workActive.FieldDate)
                        If (txtData.SelectedDate < precedentDate) Then txtData.SelectedDate = precedentDate
                    End If
                End If
            End If

            If TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "Date", "AUTO", "") Then
                txtData.SelectedDate = ReflectionHelper.GetProperty(CurrentResolution, workActive.FieldDate)
            End If

            'parametro pannello documento visibile
            PnlDocumentoVisible = StringHelper.InStrTest(workflowData, "Document")
            'parametro documento in sola lettura
            uscUploadDocumenti.ReadOnly = Not If(String.IsNullOrEmpty(sProp), False, TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "Document", sProp, ""))
            'parametro documento obbligatorio
            uscUploadDocumenti.IsDocumentRequired = TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "Document", "OBB", "")
            'parametro documenti multipli
            uscUploadDocumenti.MultipleDocuments = TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "Document", sProp, "N")

            'parametro pannello allegati visibile
            PnlAttachVisible = StringHelper.InStrTest(workflowData, "Attachment")
            'parametro allegati in sola lettura
            uscUploadAllegati.ReadOnly = Not If(sProp = "", False, TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "Attachment", sProp, ""))
            'parametro allegati obbligatori
            uscUploadAllegati.IsDocumentRequired = TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "Attachment", "OBB", "")
            uscUploadAllegati.ButtonCopyProtocol.Visible = DocSuiteContext.Current.ProtocolEnv.CopyProtocolDocumentsEnabled
            'parametro firma allegato consentita: si abilita in TabWorkflow con INS-SIGN (per l'inserimento) o MOD-SIGN per la modifica
            uscUploadAllegati.SignButtonEnabled = ResolutionEnv.IsFDQEnabled AndAlso TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "Attachment", sProp, "SIGN")

            'parametro pannello allegati visibile
            Dim currentResolutionRights As New ResolutionRights(CurrentResolution)
            PnlPrivacyAttachmentVisible = currentResolutionRights.IsPrivacyAttachmentAllowed AndAlso StringHelper.InStrTest(workflowData, "PrivacyAttachment")
            'parametro allegati riservati in sola lettura
            uscUploadPrivacyAttachment.ReadOnly = Not If(sProp.Equals(String.Empty), False, TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "PrivacyAttachment", sProp, ""))
            'parametro allegati riservati obbligatori
            uscUploadPrivacyAttachment.IsDocumentRequired = TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "PrivacyAttachment", "OBB", "")
            'parametro firma allegato riservato consentita: si abilita in TabWorkflow con INS-SIGN (per l'inserimento) o MOD-SIGN per la modifica
            uscUploadPrivacyAttachment.SignButtonEnabled = ResolutionEnv.IsFDQEnabled AndAlso TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "PrivacyAttachment", sProp, "SIGN")

            'ANNESSI
            'parametro pannello annessi visibile
            PnlAnnexesVisible = StringHelper.InStrTest(workflowData, "Annexed")
            'parametro annessi in sola lettura
            uscUploadAnnexes.ReadOnly = Not If(String.IsNullOrEmpty(sProp), False, TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "Annexed", sProp, ""))
            'parametro annessi obbligatori
            uscUploadAnnexes.IsDocumentRequired = TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "Annexed", "OBB", "")
            uscUploadAnnexes.ButtonCopyProtocol.Visible = DocSuiteContext.Current.ProtocolEnv.CopyProtocolDocumentsEnabled
            'parametro firma allegato consentita: si abilita in TabWorkflow con INS-SIGN (per l'inserimento) o MOD-SIGN per la modifica
            uscUploadAnnexes.SignButtonEnabled = ResolutionEnv.IsFDQEnabled AndAlso TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "Annexed", sProp, "SIGN")

            'Verifico se è presente il valore SharePointPrivacy in ManagedWorkflowData
            uscPrivacyPanel.PrivacyTypeVisible = TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowData, "SharePointPrivacy", sProp, "")
            uscPrivacyPanel.ValidatorEnabled = uscPrivacyPanel.PrivacyTypeVisible
        End If
    End Sub

    Private Sub InitializeDocument(ByVal resl As Resolution, ByVal workflow As TabWorkflow, ByVal loadDocument As Boolean)
        Dim s As String = ReflectionHelper.GetPropertyCase(FileResolution, workflow.FieldDocument)
        If (Not String.IsNullOrEmpty(s) AndAlso loadDocument) Then
            uscUploadDocumenti.AddNode(lblDocumento.Text, "../Resl/Images/" & workflow.DocumentImageFile, "", False, False)
            uscUploadDocumenti.ButtonRemoveEnabled = StringHelper.InStrTest(workflow.BiblosFileProperty, ".DEL.")
            uscUploadDocumenti.ButtonFrontespizioEnabled = StringHelper.InStrTest(workflow.BiblosFileProperty, ".GEN.")
        End If
        uscUploadDocumenti.ButtonFrontespizioEnabled = StringHelper.InStrTest(workflow.BiblosFileProperty, ".GEN.")
    End Sub

    Private Sub InitializeAttachment(ByVal resl As Resolution, ByVal workflow As TabWorkflow)
        Dim fieldAttachment As Integer? = ReflectionHelper.GetPropertyCase(_fileResolution, workflow.FieldAttachment)
        If fieldAttachment.HasValue Then
            uscUploadAllegati.AddNode(lblAllegati.Text, "../Comm/Images/File/Allegati16.gif", FakeNodeValue, False, False)
        End If
    End Sub

    Private Sub InitializeAnnexes(ByVal workflow As TabWorkflow)
        Dim fieldAnnexed As Guid = ReflectionHelper.GetPropertyCase(_fileResolution, workflow.FieldAnnexed)
        If fieldAnnexed <> Guid.Empty Then
            uscUploadAnnexes.AddNode(lblAnnexes.Text, "../Comm/Images/File/Allegati16.gif", FakeNodeValue, True, False)
        End If
    End Sub

    Private Sub InitializePrivacyAttachment(ByVal p_workflow As TabWorkflow)
        Dim fieldAttachment As Integer? = ReflectionHelper.GetPropertyCase(_fileResolution, p_workflow.FieldPrivacyAttachment)
        If fieldAttachment.HasValue AndAlso fieldAttachment.Value <> 0 Then
            uscUploadPrivacyAttachment.AddNode(lblPrivacyAttachment.Text, "../Comm/Images/File/Allegati16.gif", FakeNodeValue, False, False)
        End If
    End Sub

    Private Sub DuplicaDocumento(ByRef idCatena As Integer, documentControl As uscDocumentUpload, ByVal resl As Resolution, ByVal append As Boolean)
        Dim documents As List(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(resl.Location.ReslBiblosDSDB, idCatena)
        AddDocumentInfos(documents, documentControl, append)

        idCatena = 0
    End Sub

    Private Sub DuplicaDocumento(ByRef guidCatena As Guid, documentControl As uscDocumentUpload, ByVal resl As Resolution, ByVal append As Boolean)
        If guidCatena = Guid.Empty Then
            Throw New DocSuiteException("Attenzione Guid Catena non valorizzato")
        End If

        Dim documents As List(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(guidCatena)
        AddDocumentInfos(documents, documentControl, append)

        guidCatena = Guid.Empty
    End Sub

    Private Sub DuplicaDocumento(ByRef idCatena As Integer, ByRef tv As RadTreeView, ByVal resl As Resolution, Optional ByVal append As Boolean = True)
        Dim docs As List(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(resl.Location.ReslBiblosDSDB, idCatena)

        Dim index As Integer = 0
        For Each doc As BiblosDocumentInfo In docs
            Dim file As FileInfo = BiblosFacade.SaveUniqueToTemp(doc)
            Dim tn As New RadTreeNode
            tn.Text = doc.Name
            tn.Value = file.Name
            tn.ImageUrl = ImagePath.FromFile(file.FullName)
            If append Then
                tv.Nodes(0).Nodes.Add(tn)
            Else
                tv.Nodes(0).Nodes.Insert(index, tn)
                index += 1
            End If
        Next
        idCatena = 0

    End Sub

    ''' <summary> Aggiunge nodi inerenti i DocumentInfo al nodo padre indicato </summary>
    ''' <remarks> Usa un modo custom per la creazione del nodo, occhio! Rimuovere se possibile la necessità di creare un file temporaneo </remarks>
    Private Shared Sub AddDocumentInfos(documentList As List(Of BiblosDocumentInfo), documentControl As uscDocumentUpload, append As Boolean)

        Dim index As Integer = 0
        For Each document As BiblosDocumentInfo In documentList
            ' Creo la copia del documento preesistente
            Dim tempDocument As New TempFileDocumentInfo(BiblosFacade.SaveUniqueToTemp(document))
            ' Imposto il nome del documento originale come nome da salvare in biblos
            tempDocument.Name = document.Name


            If document.Attributes.Any(Function(f) f.Key.Equals(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE)) Then
                tempDocument.Attributes.Add(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE, document.Attributes.Item(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE))
            End If

            documentControl.LoadDocumentInfoByIndex(tempDocument, False, False, append, True, False, index)
            If Not append Then
                index += 1
            End If
        Next


    End Sub

    Private Sub SetPrivacy()
        Dim success As Boolean
        Try
            If selectPrivacy.SelectedItem IsNot Nothing Then
                If uscUploadDocumenti.DocumentInfos.Count = 0 Then
                    Throw New ArgumentException("Relata di pubblicazione mancante, rigenerarla o aggiungerla da file.")
                ElseIf ResolutionEnv.CheckPublishDocumentSigned AndAlso Not uscUploadDocumenti.DocumentInfos(0).IsSigned() Then
                    Throw New Exception("La relata di pubblicazione non è firmata, impossibile proseguire.")
                End If

                'Blocco i comandi della generazione del frontalino
                uscUploadDocumenti.SignButtonEnabled = False

                If Boolean.Parse(selectPrivacy.SelectedValue) Then
                    ' genero il file dove oscurare gli omissis
                    Dim publicationDocument As FileInfo = Facade.ResolutionFacade.GetPublicationDocument(CurrentResolution, uscUploadDocumenti.DocumentInfos(0), True)
                    privacyImageToPrint.ImageUrl = ImagePath.SmallPdf
                    privacyImageToPrint.NavigateUrl = CommonUtil.GetInstance().HomeDirectory & "/Temp/" & publicationDocument.Name
                    privacyTextToPrint.NavigateUrl = CommonUtil.GetInstance().HomeDirectory & "/Temp/" & publicationDocument.Name
                    AjaxAlert("Stampare il documento privacy e caricarlo.")

                    'Carico l'oggetto privacy a partire dall'oggetto attualmente definito (solo se ho scelto la modalità privacy)
                    txtPrivacyObject.Text = If(CurrentResolution.ResolutionObjectPrivacy IsNot Nothing, CurrentResolution.ResolutionObjectPrivacy, CurrentResolution.ResolutionObject)
                Else
                    txtPrivacyObject.Text = String.Empty
                End If
            End If
            success = True
        Catch ex As InvalidPdfException
            selectPrivacy.SelectedIndex = -1
            AjaxAlert(String.Format("Generazione del documento di pubblicazione non riuscita. {0}\n\n {1}", ProtocolEnv.DefaultErrorMessage, ex.Message))
        Catch ex As Exception
            selectPrivacy.SelectedIndex = -1
            AjaxAlert(ex.Message)
        End Try
        PnlPrivacyVisible = selectPrivacy.SelectedItem IsNot Nothing AndAlso Boolean.Parse(selectPrivacy.SelectedValue)
        documentUploader.IsDocumentRequired = PnlPrivacyVisible
        rfvPrivacyObject.Enabled = PnlPrivacyVisible
        Return
    End Sub

    Private Function Publicate() As Boolean
        Dim success As Boolean
        Try
            'Carico l'oggetto Privacy solo se non è vuoto
            If Not String.IsNullOrEmpty(txtPrivacyObject.Text) Then CurrentResolution.ResolutionObjectPrivacy = txtPrivacyObject.Text

            ' Controllo se il frontalino deve essere firmato
            If uscUploadDocumenti.DocumentInfos.Count = 0 Then
                Throw New DocSuiteException("La relata di pubblicazione non è stata generata correttamente, riprovare.")
            End If
            If ResolutionEnv.CheckPublishDocumentSigned AndAlso Not uscUploadDocumenti.DocumentInfos(0).IsSigned Then
                Throw New DocSuiteException("La relata di pubblicazione deve essere firmata.")
            End If

            Dim idCatenaFrontalino As Integer = 0

            If Not Action.Eq("REVOKE") Then
                If selectPrivacy.SelectedItem Is Nothing Then
                    Throw New DocSuiteException("Selezionare l'opzione privacy.")

                ElseIf Boolean.Parse(selectPrivacy.SelectedValue) Then
                    ' documento con omissis caricato dall'utente
                    If documentUploader.DocumentInfosAdded.Count <> 1 Then
                        Throw New DocSuiteException("Documento privacy mancante.")
                    End If
                    If ResolutionEnv.CheckPublishDocumentSigned AndAlso Not documentUploader.DocumentInfosAdded(0).IsSigned Then
                        Throw New DocSuiteException("Il documento privacy deve essere firmato per proseguire.")
                    End If

                    'Carico il frontalino
                    Dim docFrontalino As DocumentInfo = uscUploadDocumenti.DocumentInfosAdded(0)
                    docFrontalino.Signature = Facade.ResolutionFacade.ComposeWebPublicationSignature(CurrentResolution)
                    idCatenaFrontalino = docFrontalino.ArchiveInBiblos(CurrentResolution.Location.ReslBiblosDSDB).BiblosChainId

                    'Carico il documento privacy
                    Dim doc As DocumentInfo = documentUploader.DocumentInfosAdded(0)
                    doc.Signature = Facade.ResolutionFacade.ComposeWebPublicationSignature(CurrentResolution)

                    FileResolution.IdPrivacyPublicationDocument = doc.ArchiveInBiblos(CurrentResolution.Location.ReslBiblosDSDB).BiblosChainId
                    Facade.FileResolutionFacade.Save(FileResolution)

                    Facade.ResolutionFacade.DoWebPublication(CurrentResolution, True, doc, True)
                End If
            End If

            If idCatenaFrontalino = 0 Then
                'Carico il frontalino
                Dim doc As DocumentInfo = uscUploadDocumenti.DocumentInfos(0)

                doc.Signature = Facade.ResolutionFacade.ComposeWebPublicationSignature(CurrentResolution)
                idCatenaFrontalino = doc.ArchiveInBiblos(CurrentResolution.Location.ReslBiblosDSDB).BiblosChainId
                ' documento della resolution
                Facade.ResolutionFacade.DoWebPublication(CurrentResolution, doc, Action.Eq("REVOKE"))

            End If

            'Aggiorno la data di WebPublication
            CurrentResolution.WebPublicationDate = Now
            Facade.ResolutionFacade.Update(CurrentResolution)

            'Salvo il frontalino generato nella fileResolution
            If (idCatenaFrontalino <> 0) Then
                FileResolution.IdFrontespizio = idCatenaFrontalino
                Facade.FileResolutionFacade.Save(FileResolution)
            End If

            success = True

            Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.WP, "File pubblicato correttamente")
        Catch ex As SoapHeaderException
            AjaxAlert("Errore nella connessione al webservice")
            FileLogger.Error(LoggerName, "Errore nella connessione al webservice", ex)
        Catch ex As SoapException
            AjaxAlert("Errore del webservice: " & ex.Message)
            FileLogger.Error(LoggerName, "Errore del webservice", ex)
        Catch ex As Exception
            AjaxAlert(ex.Message)
            FileLogger.Error(LoggerName, "Errore generico", ex)
        End Try

        Return success
    End Function

    ''' <summary> Funzione di revoca di Pubblicazione per Piacenza </summary>
    Private Function revoke() As Integer
        Dim newPublicationNumber As Integer = -1
        Try
            newPublicationNumber = Facade.ResolutionFacade.StartWebRevoca(CurrentResolution)
        Catch ex As SoapHeaderException
            AjaxAlert("Errore nella connessione al webservice")
        Catch ex As SoapException
            AjaxAlert("Errore del webservice: " & ex.Message)
        Catch ex As Exception
            AjaxAlert(ex.Message)
        End Try
        Return newPublicationNumber
    End Function

    Private Sub PublicateOnSharePoint(resolution As Resolution, tabWorkflowPrePubblicazione As TabWorkflow, addWatermark As Boolean)
        If FileResolution.IdFrontespizio Is Nothing Then
            ' Genero e inserisco il frontalino
            ResolutionWPFacade.InserisciFrontalinoPubblicazione(resolution, resolution.AdoptionDate.Value.AddDays(1), tabWorkflowPrePubblicazione.Id.ResStep)
        End If

        ' Effettuo la pubblicazione effettiva
        PublicateResolutionOnSharePoint(resolution, resolution.AdoptionDate.Value.AddDays(1), addWatermark)
    End Sub

    Private Sub VerifyFrontalino()
        '' Se si tratta di uno step frontalino 
        If DocSuiteContext.Current.ResolutionEnv.Configuration = "AUSL-PC" Then
            ' Controllo se il frontalino è stato firmato
            If uscUploadDocumenti.DocumentInfosAdded.Count > 0 Then
                If String.IsNullOrEmpty(uscUploadDocumenti.DocumentInfosAdded(0).Name) Then
                    selectPrivacy.SelectedIndex = -1
                    Throw New DocSuiteException("La relata di pubblicazione non è stata generata correttamente, riprovare.")
                ElseIf ResolutionEnv.CheckPublishDocumentSigned AndAlso Not uscUploadDocumenti.DocumentInfosAdded(0).IsSigned() Then
                    selectPrivacy.SelectedIndex = -1
                    Throw New DocSuiteException("La relata di pubblicazione deve essere firmata.")
                End If
            End If

            ''Verifico la firma del documento privacy
            If Not Action.Eq("REVOKE") Then
                If selectPrivacy.SelectedItem Is Nothing Then
                    Throw New DocSuiteException("Selezionare l'opzione privacy.")

                ElseIf Boolean.Parse(selectPrivacy.SelectedValue) Then
                    ' documento con omissis caricato dall'utente
                    If documentUploader.DocumentInfosAdded.Count = 1 Then
                        If ResolutionEnv.CheckPublishDocumentSigned AndAlso Not (documentUploader.DocumentInfosAdded(0).IsSigned()) Then
                            Throw New DocSuiteException("Il documento privacy deve essere firmato per proseguire.")
                        End If
                    Else
                        Throw New DocSuiteException("Documento privacy mancante.")
                    End If
                End If
            End If
        End If
    End Sub

    ''' <summary> Permette di inserire nuovi documenti in biblos. </summary>
    ''' <remarks> Eliminato il bool di ritorno perchè era sempre true</remarks>
    <Obsolete("Da togliere in favore di InsertFileToBiblosWithGuid")>
    Private Sub InsertFileToBiblos(docs As List(Of DocumentInfo), ByRef idCatena As Integer, ByRef type As String, ByVal append As Boolean, ByVal stato As String)
        InsertFileToBiblos(docs, idCatena, type, append, stato, String.Empty)
    End Sub

    ''' <summary> Permette di inserire nuovi documenti in biblos. </summary>
    ''' <remarks> Eliminato il bool di ritorno perchè era sempre true</remarks>
    <Obsolete("Da togliere in favore di InsertFileToBiblosWithGuid")>
    Private Sub InsertFileToBiblos(docs As List(Of DocumentInfo), ByRef idCatena As Integer, ByRef type As String, ByVal append As Boolean, ByVal stato As String, ByRef signature As String)
        '--controllo se sono in aggiunta della catena vecchia o se devo creare una catena nuova
        If docs.Count <= 0 Then
            idCatena = 0
            Exit Sub
        End If

        If Not append Then
            idCatena = 0
        End If

        ' Ottengo la Signature
        signature = GetSignature(type, stato, signature)

        Dim locationFacade As New LocationFacade("ReslDB")
        Dim location As Location = locationFacade.GetById(CInt(txtIdLocation.Text))

        Dim savedDocument As BiblosDocumentInfo
        For Each doc As DocumentInfo In docs
            doc.Signature = signature
            savedDocument = doc.ArchiveInBiblos(location.ReslBiblosDSDB, idCatena)
            idCatena = savedDocument.BiblosChainId

            If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                Facade.ResolutionFacade.ResolutionInsertedDocumentPrivacyLevel(CurrentResolution, savedDocument, type)
            End If
        Next

        Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RD, String.Format("ATTI.BIBLOS: Catena {0} inserita con successo", idCatena))
    End Sub

    ''' <summary> Permette di inserire nuovi documenti in biblos. </summary>
    ''' <remarks> Eliminato il bool di ritorno perchè era sempre true</remarks>
    Private Sub InsertFileToBiblosWithGuid(docs As List(Of DocumentInfo), ByRef idCatena As Guid, ByRef type As String, ByVal append As Boolean, ByVal stato As String)
        InsertFileToBiblosWithGuid(docs, idCatena, type, append, stato, String.Empty)
    End Sub

    ''' <summary> Permette di inserire nuovi documenti in biblos. </summary>
    ''' <remarks> Eliminato il bool di ritorno perchè era sempre true</remarks>
    Private Sub InsertFileToBiblosWithGuid(docs As List(Of DocumentInfo), ByRef idCatena As Guid, ByRef type As String, ByVal append As Boolean, ByVal stato As String, ByRef signature As String)
        If docs.Count <= 0 Then
            idCatena = Guid.Empty
            Exit Sub
        End If

        If Not append Then
            idCatena = Guid.Empty
        End If

        ' Ottengo la Signature
        signature = GetSignature(type, stato, signature)

        Dim locationFacade As New LocationFacade("ReslDB")
        Dim location As Location = locationFacade.GetById(CInt(txtIdLocation.Text))

        Dim savedDocument As BiblosDocumentInfo
        For Each doc As DocumentInfo In docs
            doc.Signature = signature
            savedDocument = doc.ArchiveInBiblos(location.ReslBiblosDSDB, idCatena)
            idCatena = savedDocument.ChainId

            If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                Facade.ResolutionFacade.ResolutionInsertedDocumentPrivacyLevel(CurrentResolution, savedDocument, type)
            End If
        Next

        Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RD, String.Format("ATTI.BIBLOS: Catena {0} inserita con successo", idCatena))
    End Sub

    Private Function GetSignature(ByVal type As String, ByVal stato As String, ByVal signature As String) As String
        signature = "*"

        If stato.Eq("Documento Adozione") Then
            '--                
            signature = Facade.ResolutionFacade.SqlResolutionGetNumber(IdResolution, , , , True)
            Select Case type
                Case "A"
                    signature &= " (Allegato)"
                Case "AR"
                    signature &= " (Allegato Riservato)"
                Case "AN"
                    signature &= " (Annesso)"
                Case Else
            End Select
        ElseIf stato.Eq("Pubblicazione") AndAlso uscUploadDocumenti.ButtonFrontespizioEnabled = True Then
            signature = Facade.ResolutionFacade.SqlResolutionGetNumber(IdResolution, , , , True)
            Select Case type
                Case "A"
                    signature &= " (Allegato)"
                Case "AR"
                    signature &= " (Allegato Riservato)"
                Case "AN"
                    signature &= " (Annesso)"
                Case Else
                    signature &= String.Format(": Inserimento Albo {0:dd/MM/yyyy}", txtData.SelectedDate)
            End Select
        ElseIf (stato.Eq("FrontalinoEsecutività") OrElse stato.Eq("Esecutività")) AndAlso ResolutionEnv.Configuration.Eq("AUSL-PC") Then
            signature = Facade.ResolutionFacade.SqlResolutionGetNumber(IdResolution, , , , True)
            Select Case type
                Case "A"
                    signature &= " (Allegato)"
                Case "AR"
                    signature &= " (Allegato Riservato)"
                Case "AN"
                    signature &= " (Annesso)"
                Case Else
            End Select
        ElseIf stato.Eq("Pubblicazione") OrElse stato.Eq("Esecutività") Then
            signature = Facade.ResolutionFacade.SqlResolutionGetNumber(IdResolution, , , , True)
            '**** DG - 2011-09-06 - Per ASL3 TO va tolto l'inserimento della data di scadenza
            Dim azienda As String = DocSuiteContext.Current.ResolutionEnv.Configuration
            If ResolutionEnv.ExpirationDateSignatureEnabled AndAlso CurrentResolution.Type.Description.Eq("Delibera") Then
                signature &= " Data Scad. " & txtData.SelectedDate.Value.AddDays(15).ToString("dd/MM/yyyy")
            End If
            Select Case type
                Case "A"
                    signature &= " (Allegato)"
                Case "AR"
                    signature &= " (Allegato Riservato)"
                Case "AN"
                    signature &= " (Annesso)"
                Case Else
            End Select
        ElseIf stato = "Ritiro Pubblicazione" Then
            signature = Facade.ResolutionFacade.SqlResolutionGetNumber(IdResolution, , , , True)
            Select Case type
                Case "A"
                    signature &= " (Allegato)"
                Case "AR"
                    signature &= " (Allegato Riservato)"
                Case "AN"
                    signature &= " (Annesso)"
                Case Else
            End Select
            If uscUploadDocumenti.ButtonFrontespizioEnabled And type = "D" Then
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