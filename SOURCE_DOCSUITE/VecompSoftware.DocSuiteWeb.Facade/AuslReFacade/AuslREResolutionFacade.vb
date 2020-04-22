
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.IO
Imports System.Xml
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.PDF
Imports Biblos = VecompSoftware.Services.Biblos
Imports StampaConforme = VecompSoftware.Services.StampaConforme
Imports System.Web
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Biblos

Public NotInheritable Class AuslREResolutionFacade

    Private Sub New()
    End Sub
    Public Shared Sub ForwardWorkflow(resl As Resolution, stepNumber As Short, stepCount As Short)
        If stepCount <= 0 Then
            Throw New DocSuiteException("Errore WorkFlow", "Il numero di step da eseguire deve essere maggiore di 0")
        End If
    End Sub

    Public Shared Function GetResolutionType(resl As Resolution) As Short
        If resl.IsChecked.HasValue AndAlso resl.IsChecked.Value Then
            Return 2
        End If
        Return resl.Type.Id
    End Function

    Public Shared Function GeneraFrontalino(resl As Resolution, tw As TabWorkflow, stepDate As DateTime) As [Byte]()
        Try
            Dim info As FileInfo = ResolutionUtil.GeneraStampaODG(resl, tw.ManagedWorkflowData, stepDate, resl.Type.Id, tw.Description, tw.Template)
            Dim doc As New FileDocumentInfo(info)
            Return doc.GetPdfStream()
        Catch ex As Exception
            Throw New DocSuiteException("Genera Frontalino", "Errore nella generazione del frontalino in PDF", ex)
        End Try
    End Function

    Public Shared Sub ForwardWorkflow(resl As Resolution, tw As TabWorkflow, stepServiceNumber As String, user As String)
        Dim stepDate As DateTime = DateTime.Now
        Dim stepDocuments As IList(Of DocumentInfo) = New List(Of DocumentInfo)()

        Try
            Select Case tw.Description
                Case WorkflowStep.ADOZIONE
                    stepDate = DateTime.Now
                    Exit Select
                Case WorkflowStep.PUBBLICAZIONE
                    stepDate = resl.AdoptionDate.Value
                    '.AddDays(1);

                    'DEVO CREARE IL FRONTALINO
                    Dim bytes As Byte() = GeneraFrontalino(resl, tw, stepDate)
                    Dim label As String = If(tw.Description.Eq("Ritiro Pubblicazione"), "Frontespizio Ritirato.pdf", "Frontespizio.pdf")
                    stepDocuments.Add(New MemoryDocumentInfo(bytes, label))
                    Exit Select
                Case WorkflowStep.ESECUTIVA
                    stepDate = DateTime.Now
                    Exit Select
                Case WorkflowStep.RITIRO
                    stepDate = resl.PublishingDate.Value.AddDays(15)
                    Exit Select
            End Select
        Catch ex As Exception
            Throw New DocSuiteException("Passo del WorkFlow", "Impossibile calcolare la data dello step", ex)
        End Try


        ForwardWorkflow(resl, tw, stepDate, "N", stepDocuments, Nothing, Nothing, user)
    End Sub

    Public Shared Sub ForwardWorkflow(resl As Resolution, tw As TabWorkflow, stepDate As DateTime, stepServiceNumber As String, stepDocuments As IList(Of DocumentInfo), stepAttachments As IList(Of DocumentInfo), stepAnnexes As IList(Of DocumentInfo), user As String, Optional tempPath As String = "")
        Dim idCatenaDocumento As Integer = -1
        Dim idAllegati As Integer = -1
        Dim idAnnexed As Guid = Guid.Empty

        If stepDocuments Is Nothing Then
            stepDocuments = New List(Of DocumentInfo)()
        End If
        If stepAttachments Is Nothing Then
            stepAttachments = New List(Of DocumentInfo)()
        End If
        If stepAnnexes Is Nothing Then
            stepAnnexes = New List(Of DocumentInfo)()
        End If

        Try
            Dim appendDoc As Boolean = False
            Dim appendAttach As Boolean = False
            Dim appendAnnexed As Boolean = False
            If Not [String].IsNullOrEmpty(tw.BiblosFileProperty) Then
                appendDoc = StringHelper.InStrTest(tw.BiblosFileProperty, "APP")
                appendAttach = StringHelper.InStrTest(tw.BiblosFileProperty, "APP")
                appendAnnexed = StringHelper.InStrTest(tw.BiblosFileProperty, "APP")
            End If


            Dim idCatenaDocumentoTemp As Object = ReflectionHelper.GetPropertyCase(resl.File, tw.FieldDocument)
            If idCatenaDocumentoTemp IsNot Nothing Then
                idCatenaDocumento = CInt(idCatenaDocumentoTemp)
            End If

            '#Region "Collaboration"

            'SE ARRIVO DA COLLABORATION O COMUNQUE IL DOCUMENTO DI PROPOSTA
            'E' UN P7M QUESTO DIVIENE DI DEFAULT IL DOCUMENTO DI ADOZIONE 
            '****VALE PER TUTTI I CLIENTI CHE HANNO LA COLLABORATION ( AUSL E ASMN )
            If tw.Description.Eq(WorkflowStep.ADOZIONE) Then
                'SE HO CARICATO DEI DOCUMENTI DA INTERFACCIA NON UTILIZZO QUELLO DI PROPOSTA. 
                'IN INSERIMENTO LA CATENA E' SALVATA SIA IN IDPROPOSAL (PROPOSTA) CHE IN IDASSUMEDPROPOSAL
                If stepDocuments.Count = 0 Then
                    If idCatenaDocumento = -1 Then
                        Throw New DocSuiteException("Passo del WorkFlow", String.Format("Impossibile recuperare il documento di proposta per resolution [{0}] ", resl.Id))
                    End If

                    Dim doc As New BiblosDocumentInfo(resl.Location.DocumentServer, resl.Location.ReslBiblosDSDB, idCatenaDocumento, 0)

                    'OLDDocumentInfo di = bf.GetChainSingleDocumentInfo(resl.Location.DocumentServer, resl.Location.ReslBiblosDSDB, idCatenaDocumento, 0);

                    stepDocuments = New DocumentInfo() {doc}

                    'PER IL DOCUMENTO DI ADOZIONE SI CREA COMUNQUE UNA SUA CATENA DIVERSA DAL DOCUMENTO DI PROPOSTA 
                    'ALLA FINE AVRO' IN IDPROPOSAL LA CATENA CON SOLO LA PROPOSTA E IN IDASSUMEDPROPOSAL LA NUOVA CATENA
                    'CON IL DOCUMENTO ADOTTATO 
                    appendDoc = False
                End If
                'IN PUBLICAZIONE E RITIRO CONTROLLO CHE SE IL DOCUMENTO DI PROPOSTA E' UN P7M ( ARRIVA DA COLLABORAZIONE )
                'ANCHE I FRONTALINI DI PUBBLICAZIONE E RITIRO SIANO FIRMATI                
                '****VALE PER TUTTI I CLIENTI CHE HANNO LA COLLABORATION ( AUSL E ASMN )
            ElseIf tw.Description = WorkflowStep.PUBBLICAZIONE OrElse tw.Description = WorkflowStep.RITIRO Then
                If stepDocuments.Count = 0 AndAlso resl.File.IdProposalFile.HasValue Then
                    Dim filename As String = Service.GetDocumentName(New UIDDocument(resl.Location.DocumentServer, resl.Location.ReslBiblosDSDB, resl.File.IdProposalFile.Value))

                    If String.IsNullOrEmpty(filename) Then
                        Throw New DocSuiteException("Passo del WorkFlow", [String].Format("Impossibile recuperare il nome del file archiviato nella catena [{0}]", resl.File.IdProposalFile.Value))
                    End If

                    If FileHelper.MatchExtension(filename, FileHelper.P7M) AndAlso Not FileHelper.MatchExtension(stepDocuments(0).Name, FileHelper.P7M) Then
                        Throw New DocSuiteException("Passo del WorkFlow", String.Format("Il documento  di proposta [{0}] deve essere firmato", resl.File.IdProposalFile.Value))
                    End If
                End If
            End If
            '#End Region

            '#Region "Documento Principale"

            Dim docSignature As String = GetSignatureDocument(resl, tw.Description, stepDate, stepServiceNumber)

            For Each di As DocumentInfo In stepDocuments
                di.Signature = docSignature
            Next

            Dim reslType As Short = GetResolutionType(resl)
            'PER LE DELIBERE SOGGETTE (TYPE==2) CREO IL CATENONE IN RITIRO
            If (tw.Description.Eq(WorkflowStep.ESECUTIVA) AndAlso reslType <> 2S) OrElse (tw.Description.Eq(WorkflowStep.RITIRO) AndAlso reslType = 2S) Then
                'ACCODO FRONTALINO, DOC. ADOTTATO E ALLEGATI
                'NON USO IDFRONTESPIZIO PERCHE' NON E' SEMPRE STATO VALORIZZATO
                If Not resl.File.IdResolutionFile.HasValue Then
                    Throw New DocSuiteException("Passo del WorkFlow", String.Format("ResolutionFile non trovato per resolution [{0}]", resl.Id))
                End If
                If Not resl.File.IdAssumedProposal.HasValue Then
                    Throw New DocSuiteException("Passo del WorkFlow", String.Format("AssumedProposal non trovato per resolution [{0}]", resl.Id))
                End If

                Dim frontalino As New BiblosDocumentInfo(resl.Location.DocumentServer, resl.Location.ReslBiblosDSDB, resl.File.IdResolutionFile.Value, 0)
                Dim docAdottato As New BiblosDocumentInfo(resl.Location.DocumentServer, resl.Location.ReslBiblosDSDB, resl.File.IdAssumedProposal.Value, 0)

                Dim newStepDocs As IList(Of DocumentInfo) = New List(Of DocumentInfo)()
                ' NELLE SOGGETTE FA GIA' PARTE DELLA CATENA
                If reslType <> 2 Then
                    newStepDocs.Add(frontalino)
                End If
                newStepDocs.Add(docAdottato)

                If resl.File.IdAttachements.HasValue AndAlso resl.File.IdAttachements.Value <> 0 Then
                    Dim attachs As List(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(resl.Location.DocumentServer, resl.Location.ReslBiblosDSDB, resl.File.IdAttachements.Value)

                    For Each di As BiblosDocumentInfo In attachs
                        newStepDocs.Add(di)
                    Next
                End If

                'PER COME E' CONFIGURATO IL WORKFLOW SOLO PER LE SOGGETTE
                For Each di As DocumentInfo In stepDocuments
                    newStepDocs.Add(di)
                Next

                stepDocuments = newStepDocs
            End If

            'L'HO FATTO ANDARE.. MI ERO ROTTO DI GESTIRE APPEND
            If Not (reslType = 2 AndAlso tw.Description.Eq(WorkflowStep.ESECUTIVA)) AndAlso Not appendDoc Then
                idCatenaDocumento = 0
            End If

            If stepDocuments.Count > 0 Then
                idCatenaDocumento = DocumentInfoFactory.ArchiveDocumentsInBiblos(stepDocuments, resl.Location.DocumentServer, resl.Location.ReslBiblosDSDB, idCatenaDocumento)

                If idCatenaDocumento = 0 Then
                    Throw New DocSuiteException("Passo del WorkFlow", String.Format("Errore nel salvataggio del documento su Biblos per resolution [{0}] ", resl.Id))
                End If
            End If

            'SALVO IL FRONTESPIZIO IN QUESTO CAMPO PER POTERLO EVENTUALMENTE RECUPERARE IN SEGUITO ( NEL CASO 
            'VENISSE ABILITATO IL BACK OPPURE IL BUON VITTORIO DEVE SISTEMARE QUALCHE ATTO) . IL WORKFLOW E' CONFIGURATO PER SALVARE LA CATENA DEL DOCUMENTO 
            'NELLA COLONNA IDRESOLUTION MA DAL PASSO SUCCESSIVO QUESTA CATENA CONTERRA' TUTTI DI DOCUMENTI DELL'ATTO
            If tw.Description.Eq(WorkflowStep.PUBBLICAZIONE) Then
                resl.File.IdFrontespizio = idCatenaDocumento
            End If

            '#End Region

            '#Region "Allegati"

            Dim attachSignature As String = GetSignatureAttachment(resl, tw.Description, stepDate, stepServiceNumber, "Allegato")

            For Each di As DocumentInfo In stepAttachments
                di.Signature = attachSignature
            Next

            If Not String.IsNullOrEmpty(tw.FieldAttachment) Then
                ' Abilito l'append degli allegati solo se il parametro è spento
                ' --> ovvero come era l'applicazione prima dell'introduzione della duplicazione degli allegati
                ' altrimenti sdoppia ad ogni passaggio
                If Not DocSuiteContext.Current.ResolutionEnv.CopyDocumentsToAdoption Then
                    Dim idAllegatiStep As Object = ReflectionHelper.GetPropertyCase(resl.File, tw.FieldAttachment)
                    If idAllegatiStep IsNot Nothing Then
                        idAllegati = CInt(idAllegatiStep)
                    End If
                End If
                appendAttach = True
                ' GLI ALLEGATI VANNO SEMPRE IN APPEND TRANNE IN ADOZIONE
                'GLI ALLEGATI SAREBBERO DA DUPLICARE OGNI VOLTA CHE E' ATTIVA LA POSSIBILITA'
                'DI TORNARE INDIETRO IN MODO DA POTER RIPRISTINARE LO STATO DELLO STEP PRECEDENTE
                'PER ORA MANTENGO LA LOGICA ATTUALE CHE GESTISCE A CODICE QUANDO DUPLICARE LA CATENA OPPURE NO
                If tw.Description.Eq(WorkflowStep.ADOZIONE) AndAlso idAllegati > 0 Then
                    Dim attachs As List(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(resl.Location.DocumentServer, resl.Location.ReslBiblosDSDB, idAllegati)

                    For Each di As BiblosDocumentInfo In attachs
                        di.Signature = attachSignature
                    Next

                    If attachs IsNot Nothing AndAlso attachs.Count > 0 Then
                        Dim temp As New List(Of DocumentInfo)(attachs)
                        For Each di As DocumentInfo In stepAttachments
                            di.Signature = attachSignature
                            temp.Add(di)
                        Next
                        stepAttachments = temp
                    End If

                    appendAttach = False
                End If

                If Not appendAttach Then
                    idAllegati = 0
                End If


                If stepAttachments.Count > 0 Then
                    idAllegati = DocumentInfoFactory.ArchiveDocumentsInBiblos(stepAttachments, resl.Location.DocumentServer, resl.Location.ReslBiblosDSDB, idAllegati)
                    If idAllegati = 0 Then
                        Throw New DocSuiteException("Passo del WorkFlow", String.Format("Errore nel salvataggio in Biblos degli allegati su resolution [{0}].", resl.Id))
                    End If
                End If
            End If

            '#End Region

            '#Region "Annessi"

            Dim annexedSignature As String = GetSignatureAttachment(resl, tw.Description, stepDate, stepServiceNumber, "Annesso")

            For Each di As DocumentInfo In stepAnnexes
                di.Signature = annexedSignature
            Next

            If Not String.IsNullOrEmpty(tw.FieldAnnexed) Then
                Dim idAnnessiStep As Object = ReflectionHelper.GetPropertyCase(resl.File, tw.FieldAnnexed)
                ' GLI ALLEGATI VANNO SEMPRE IN APPEND TRANNE IN ADOZIONE
                If idAnnessiStep IsNot Nothing Then
                    idAnnexed = DirectCast(idAnnessiStep, Guid)
                    appendAnnexed = True
                End If

                'GLI ALLEGATI SAREBBERO DA DUPLICARE OGNI VOLTA CHE E' ATTIVA LA POSSIBILITA'
                'DI TORNARE INDIETRO IN MODO DA POTER RIPRISTINARE LO STATO DELLO STEP PRECEDENTE
                'PER ORA MANTENGO LA LOGICA ATTUALE CHE GESTISCE A CODICE QUANDO DUPLICARE LA CATENA OPPURE NO
                If tw.Description.Eq(WorkflowStep.ADOZIONE) AndAlso idAnnexed <> Guid.Empty Then
                    Dim annexes As List(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(resl.Location.DocumentServer, idAnnexed)

                    For Each di As BiblosDocumentInfo In annexes
                        di.Signature = annexedSignature
                    Next

                    If annexes IsNot Nothing AndAlso annexes.Count > 0 Then
                        Dim temp As New List(Of DocumentInfo)(annexes)
                        For Each di As DocumentInfo In stepAnnexes
                            di.Signature = annexedSignature
                            temp.Add(di)
                        Next
                        stepAnnexes = temp
                    End If

                    appendAnnexed = False
                End If

                If Not appendAnnexed Then
                    idAnnexed = Guid.Empty
                End If


                If stepAnnexes.Count > 0 Then
                    idAnnexed = DocumentInfoFactory.ArchiveDocumentsInBiblos(stepAnnexes, resl.Location.DocumentServer, resl.Location.ReslBiblosDSDB, idAnnexed)
                    If idAnnexed = Guid.Empty Then
                        Throw New DocSuiteException("Passo del WorkFlow", String.Format("Errore nel salvataggio in Biblos degli allegati su resolution [{0}].", resl.Id))
                    End If
                End If
            End If

            '#End Region

            '#Region "SharePoint"

            ' PUBBLICAZIONE SU SHAREPOINT
            ' QUESTA LOGICA VALE SOLO PER AUSL-RE
            If DocSuiteContext.Current.ResolutionEnv.WebAutoPublish Then

                Dim strXmlOther As String = "<?xml version=""1.0"" encoding=""utf-8"" ?>"
                strXmlOther &= "<Metas><Metadata name=""Prop1"" value="""
                strXmlOther &= resl.ServiceNumber
                strXmlOther &= """></Metadata></Metas>"

                Dim strXmlDoc As String
                If (reslType = 2S AndAlso tw.Description.Eq(WorkflowStep.PUBBLICAZIONE)) OrElse (reslType <> 2S AndAlso tw.Description.Eq(WorkflowStep.ESECUTIVA)) Then

                    'PER LE SOGGETTE IL FRONTALINO E' STATO UPLOADATO NEL PASSO CORRENTE
                    Dim frontalino As DocumentInfo
                    If reslType = 2S Then
                        frontalino = stepDocuments(0)
                    Else
                        frontalino = New BiblosDocumentInfo(resl.Location.DocumentServer, resl.Location.ReslBiblosDSDB, resl.File.IdResolutionFile.Value, 0)
                    End If

                    Dim docAdottato As DocumentInfo = New BiblosDocumentInfo(resl.Location.DocumentServer, resl.Location.ReslBiblosDSDB, resl.File.IdAssumedProposal.Value, 0)

                    Dim docs As IList(Of DocumentInfo) = New List(Of DocumentInfo)()
                    docs.Add(frontalino)
                    docs.Add(docAdottato)

                    Dim atchs As List(Of DocumentInfo) = Nothing
                    If resl.File.IdAttachements.HasValue Then
                        atchs = BiblosDocumentInfo.GetDocuments(resl.Location.DocumentServer, resl.Location.ReslBiblosDSDB, resl.File.IdAttachements.Value).Cast(Of DocumentInfo)().ToList()
                    End If

                    Dim annexes As List(Of DocumentInfo) = Nothing
                    If resl.File.IdAnnexes <> Guid.Empty Then
                        annexes = BiblosDocumentInfo.GetDocuments(resl.Location.DocumentServer, resl.File.IdAnnexes).Cast(Of DocumentInfo)().ToList()
                    End If

                    strXmlDoc = GetSharepointFrontespizioXML(resl, docs, atchs, annexes, frontalino.Signature, attachSignature, tempPath)

                    SharePointFacade.Publish(resl, stepDate, stepDate.AddDays(15), frontalino.Signature, strXmlDoc, strXmlOther)
                ElseIf tw.Description.Eq(WorkflowStep.RITIRO) Then
                    strXmlDoc = GetSharepointFrontespizioXML(resl, stepDocuments, Nothing, Nothing, docSignature, attachSignature, tempPath)

                    SharePointFacade.Retire(resl, stepDate, docSignature, strXmlDoc, strXmlOther)
                End If
            End If
            '#End Region

            'SALVATAGGIO FINALE DATI WORKFLOW 
            If idAllegati = -1 Then
                If Not String.IsNullOrEmpty(tw.FieldAttachment) Then
                    Dim idAllegatiStep As Object = ReflectionHelper.GetPropertyCase(resl.File, tw.FieldAttachment)
                    If idAllegatiStep IsNot Nothing Then
                        idAllegati = CInt(idAllegatiStep)
                    Else
                        idAllegati = 0
                    End If
                Else
                    idAllegati = 0
                End If
            End If

            If idCatenaDocumento = -1 Then
                idCatenaDocumento = If(idCatenaDocumentoTemp IsNot Nothing, CInt(idCatenaDocumentoTemp), 0)
            End If

            Dim nomeCatenaDocumento As String = String.Empty
            Try
                If stepDocuments.Count = 1 Then
                    nomeCatenaDocumento = stepDocuments _
                                      .Select(Function(o) o.DownloadFileName) _
                                      .FirstOrDefault()
                End If
            Catch ex As Exception
                Throw New DocSuiteException("Passo del WorkFlow", String.Format("Nessun documento principale trovato per resolution [{0}]", resl.Id))
            End Try

            Dim flag As Boolean = FacadeFactory.Instance.ResolutionFacade.SqlResolutionUpdateWorkflowData(resl.Id, resl.Type.Id, Not resl.Number.HasValue, tw, [String].Format("{0:dd/MM/yyyy}", stepDate), stepServiceNumber, idAllegati, idCatenaDocumento, idAnnexed, "N", True, user)

            If Not flag Then
                Throw New DocSuiteException("Passo del WorkFlow", String.Format("Impossibile salvare i dati nella tabella ResolutionWorkFlow per resolution [{0}]", resl.Id))
            End If

            'Invio comando di creazione/aggiornamento Resolution alle WebApi
            If tw.Description.Eq(WorkflowStep.ADOZIONE) Then
                FacadeFactory.Instance.ResolutionFacade.SendCreateResolutionCommand(resl)
            Else
                FacadeFactory.Instance.ResolutionFacade.SendUpdateResolutionCommand(resl)
            End If

            flag = FacadeFactory.Instance.ResolutionWorkflowFacade.InsertNextStep(resl.Id, tw.Id.ResStep - 1S, idCatenaDocumento, idAllegati, 0, idAnnexed, Guid.Empty, Guid.Empty, user, nomeCatenaDocumento)
            If Not flag Then
                Throw New DocSuiteException("Passo del WorkFlow", String.Format("Impossibile salvare i dati nella tabella ResolutionWorkFlow per resolution [{0}]", resl.Id))
            End If

            If tw.Description.Eq(WorkflowStep.ADOZIONE) AndAlso resl.ImmediatelyExecutive.HasValue AndAlso resl.ImmediatelyExecutive.Value Then
                If FacadeFactory.Instance.TabWorkflowFacade.GetByStep(resl.WorkflowType, tw.Id.ResStep + 1S, tw) Then
                    ForwardWorkflow(resl, tw, stepServiceNumber, user)
                End If

            End If
        Catch ex As DocSuiteException
            Throw
        Catch ex As Exception
            Throw New DocSuiteException("Passo del WorkFlow", "Errore non previsto", ex)
        End Try

    End Sub

    'DOVREBBE DIVENTARE UN METODO CHE DATA UNA RESL ED UNO STEP TI TORNA LA FIRMA CORRETTA PER QUELLO STEP, 
    'RECUPERANDO DATA E SERVICE NUMBER DALLA RESOLUTION.. PER ORA PERO' QUANDO LO RICHIAMO LA RESOLUTION NON E' AGGIORNATA E QUINDI 
    'PER IL MOMENTO RIPORTO LA VECCHIA LOGICA....
    'TODO: CREARE UN OVERLOAD DEL METODO CHE DATO SOLO LA RESL RITORNI LA FIRMA PER LO STEP CORRENTE..        
    Public Shared Function GetSignatureDocument(resl As Resolution, currentStep As [String], stepDate As DateTime, serviceNumber As String) As String
        Dim signature As String = "*"

        Select Case currentStep
            Case WorkflowStep.ADOZIONE
                signature = FacadeFactory.Instance.ResolutionFacade.SqlResolutionGetNumber(resl.Id, stepDate.Year.ToString(), serviceNumber, [String].Format("{0:dd/MM/yyyy}", stepDate), True)
                Exit Select
            Case WorkflowStep.DOCUMENTO_ADOZIONE
                signature = FacadeFactory.Instance.ResolutionFacade.SqlResolutionGetNumber(resl.Id, String.Empty, String.Empty, String.Empty, True)
                Exit Select
            Case WorkflowStep.PUBBLICAZIONE
                signature = String.Format("{0}: Inserimento Albo {1:dd/MM/yyyy}", FacadeFactory.Instance.ResolutionFacade.SqlResolutionGetNumber(resl.Id, String.Empty, String.Empty, String.Empty, True), stepDate)
                Exit Select
            Case WorkflowStep.ESECUTIVA
                signature = FacadeFactory.Instance.ResolutionFacade.SqlResolutionGetNumber(resl.Id, String.Empty, String.Empty, String.Empty, True)
                ' NELLA VECCHIA VERSIIONE DELLA DOCSUITE NON HO TROVATO QUESTO PEZZO DI CODICE
                ' QUINDI PROBABILMENTE ERA UNA MODIFICA CHE NON E' STATA RIPORTATA SULLA NUOVA..
                'if (resl.Type.Description == "Delibera")
                '   signature +=  String.Format(" Data Scad. {0:dd/MM/yyyy}", resl.EffectivenessDate.Value.AddDays(15));
                Exit Select
            Case WorkflowStep.RITIRO
                signature = [String].Format("{0}: Ritiro Albo {1:dd/MM/yyyy}", FacadeFactory.Instance.ResolutionFacade.SqlResolutionGetNumber(resl.Id, String.Empty, String.Empty, String.Empty, True), stepDate)
                Exit Select
        End Select

        If Not signature.Contains("*") Then
            If DocSuiteContext.Current.ResolutionEnv.SignatureType = 1 Then
                signature = [String].Format("{0} {1}", DocSuiteContext.Current.ResolutionEnv.CorporateAcronym, signature)
            End If
        End If

        Return signature
    End Function

    ''' <summary> Calcola la signature per la catena allegati o annessi. </summary>
    ''' <param name="resl"></param>
    ''' <param name="currentStep"></param>
    ''' <param name="stepDate"></param>
    ''' <param name="serviceNumber"></param>
    ''' <param name="typeChain">Allegato o Annesso per indicare la catena</param>
    Public Shared Function GetSignatureAttachment(resl As Resolution, currentStep As [String], stepDate As DateTime, serviceNumber As String, typeChain As String) As String
        Dim signature As String = "*"

        Select Case currentStep
            Case WorkflowStep.ADOZIONE, WorkflowStep.DOCUMENTO_ADOZIONE, WorkflowStep.ESECUTIVA
                signature = [String].Format("{0} ({1})", GetSignatureDocument(resl, currentStep, stepDate, serviceNumber), typeChain)
                Exit Select
            Case WorkflowStep.PUBBLICAZIONE
                signature = String.Format("{0} ({1})", FacadeFactory.Instance.ResolutionFacade.SqlResolutionGetNumber(resl.Id, String.Empty, String.Empty, String.Empty, True), typeChain, stepDate)
                Exit Select
            Case WorkflowStep.RITIRO
                signature = [String].Format("{0} ({1})", FacadeFactory.Instance.ResolutionFacade.SqlResolutionGetNumber(resl.Id, String.Empty, String.Empty, String.Empty, True), typeChain, stepDate)
                Exit Select
        End Select

        Return signature
    End Function

    Public Shared Function GetSharepointFrontespizioXML(resl As Resolution, documents As IList(Of DocumentInfo), attachments As IList(Of DocumentInfo), annexes As IList(Of DocumentInfo), docSignature As [String], atchSignature As [String], Optional tempPath As String = "") As String
        Dim exportedFile As String = [String].Empty
        ' Butto via il file esportato in quanto non è richiesto tramite questa chiamata
        Return GetSharepointFrontespizioXML(resl, documents, attachments, annexes, docSignature, atchSignature,
            String.Empty, exportedFile, tempPath)
    End Function

    Public Shared Function GetSharepointFrontespizioXML(resl As Resolution, documents As IList(Of DocumentInfo), attachments As IList(Of DocumentInfo), annexes As IList(Of DocumentInfo), docSignature As [String], atchSignature As [String],
        watermark As [String], ByRef exportedFile As String, Optional tempPath As String = "") As String
        Return GetSharepointFrontespizioXML(resl, documents, attachments, annexes, docSignature, atchSignature,
            watermark, exportedFile, [String].Empty, tempPath)
    End Function

    Public Shared Function GetSharepointFrontespizioXML(resl As Resolution, documents As IList(Of DocumentInfo), attachments As IList(Of DocumentInfo), annexes As IList(Of DocumentInfo), docSignature As [String], atchSignature As [String],
        watermark As [String], ByRef exportedFile As String, finalSignature As [String], Optional tempPath As String = "") As String

        If String.IsNullOrEmpty(tempPath) Then
            tempPath = CommonUtil.GetInstance().AppTempPath
        End If

        Dim xmlDoc As New XmlDocument()
        Dim xml As New StringBuilder("<?xml version=""1.0"" encoding=""utf-8"" ?>")

        xml.Append("<Documents>")
        xml.Append("</Documents>")
        xmlDoc.LoadXml(xml.ToString())

        Dim pdfMerge As New PdfMerge()

        Dim elemF As XmlElement = xmlDoc.CreateElement("Frontespizio")
        elemF.SetAttribute("nome", docSignature.Replace("/", "-").Replace("\", "-").Replace(":", "").Replace(" ", "_") & "_" & resl.Id)
        elemF.SetAttribute("ext", "pdf")

        If documents IsNot Nothing Then
            For Each di As DocumentInfo In documents
                Dim converted As Byte() = di.GetPdfStream(di.Signature)
                pdfMerge.AddDocument(converted)
            Next
        End If

        If attachments IsNot Nothing Then
            For Each di As DocumentInfo In attachments
                Dim converted As Byte() = di.GetPdfStream(di.Signature)
                pdfMerge.AddDocument(converted)
            Next
        End If

        If annexes IsNot Nothing Then
            For Each di As DocumentInfo In annexes
                Dim converted As Byte() = di.GetPdfStream(di.Signature)
                pdfMerge.AddDocument(converted)
            Next
        End If

        Dim sFilename As String = FileHelper.UniqueFileNameFormat("FrontespizioSharepoint.pdf", DocSuiteContext.Current.User.UserName)
        Dim destination As String = Path.Combine(tempPath, sFilename)
        pdfMerge.Merge(destination)

        Dim stream As Byte() = File.ReadAllBytes(destination)

        Dim output As Byte() = If(watermark.Equals(String.Empty), StampaConforme.Service.ConvertToPdf(stream, FileHelper.PDF, finalSignature, String.Empty), StampaConforme.Service.LockedPdf(stream, FileHelper.PDF, finalSignature, watermark))
        ' Salvo lo stream su Temp
        Dim memoryDocumentInfo As New MemoryDocumentInfo(output, sFilename)
        ' Restituisco il valore
        Dim uniquename As String = FileHelper.UniqueFileNameFormat(memoryDocumentInfo.PDFName, DocSuiteContext.Current.User.UserName)
        exportedFile = memoryDocumentInfo.SavePdf(New DirectoryInfo(tempPath), uniquename, String.Empty).FullName

        elemF.InnerXml = "<Data><![CDATA[" & Convert.ToBase64String(output) & "]]></Data>"
        If xmlDoc.DocumentElement IsNot Nothing Then
            xmlDoc.DocumentElement.AppendChild(elemF)
        End If

        Return xmlDoc.InnerXml
    End Function

End Class