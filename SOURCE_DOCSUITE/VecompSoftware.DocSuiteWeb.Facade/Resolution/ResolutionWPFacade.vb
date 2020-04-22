Imports System
Imports System.Xml
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports VecompSoftware.Helpers.PDF
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Text
Imports VecompSoftware.Helpers
Imports VecompSoftware.Services.Sharepoint
Imports VecompSoftware.Services.Biblos.Models

''' <summary>
''' Classe per la pubblicazione Internet delle delibere/atti
''' Metodi per la pubblicazione tramite servizio SharePoint
''' </summary>
''' <remarks></remarks>
Partial Public Class ResolutionWPFacade
    Inherits BaseResolutionFacade(Of Resolution, Integer, NHibernateResolutionDao)

    Public Sub New()
        MyBase.New()
    End Sub

    ''' <summary>
    ''' Metodo per popolare con informazioni aggiuntive la pubblicazione in SP
    ''' </summary>
    ''' <param name="currentResolution">Resolution</param>
    ''' <returns>Stringa XML con i valori da aggiornare</returns>
    ''' <remarks>Metodo da sistemare per rendere dinamica le informazioni passate</remarks>
    Public Function GetXmlOther(ByVal currentResolution As Resolution) As String
        ' Creo XML con Metadati
        Dim xmlMeta As New XmlDocument()
        xmlMeta.AppendChild(xmlMeta.CreateXmlDeclaration("1.0", "utf-8", String.Empty))
        xmlMeta.AppendChild(xmlMeta.CreateElement("Metas"))
        Dim metadata As XmlElement = xmlMeta.CreateElement("Metadata")
        metadata.Attributes.Append(xmlMeta.CreateAttribute("name")).Value = "Prop1"
        'Il parametro ForceSharePointPublication è specifico per la pubblicazione su Sharepoint di AUSL-RE
        If DocSuiteContext.Current.ResolutionEnv.ForceSharePointPublication Then
            metadata.Attributes.Append(xmlMeta.CreateAttribute("value")).Value = currentResolution.ServiceNumber
        Else
            metadata.Attributes.Append(xmlMeta.CreateAttribute("value")).Value = currentResolution.InclusiveNumber
        End If
        xmlMeta.DocumentElement.AppendChild(metadata)
        Return xmlMeta.InnerXml
    End Function

    Public Function GetXmlSp(ByVal files As IList(Of DocumentInfo), ByVal signature As String, ByVal idResolution As Integer) As XmlDocument
        Return GetXmlSp(files, signature, idResolution, String.Empty)
    End Function

    Public Function GetXmlSp(ByVal files As IList(Of DocumentInfo), ByVal signature As String, ByVal idResolution As Integer, ByVal watermark As String) As XmlDocument
        Dim xmlDoc As XmlDocument = New XmlDocument()
        xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "utf-8", String.Empty))
        xmlDoc.AppendChild(xmlDoc.CreateElement("Documents"))

        For Each file As DocumentInfo In files
            Dim elemF As XmlElement = xmlDoc.CreateElement("Frontespizio")
            elemF.SetAttribute("nome", signature.Replace("/", "-").Replace("\", "-").Replace(":", "").Replace(" ", "_") + "_" + idResolution.ToString)
            elemF.SetAttribute("ext", "pdf")

            Dim output As Byte() = If(watermark.Equals(String.Empty), file.GetPdfStream(signature), file.GetPdfLocked(signature, watermark))
            elemF.InnerXml = "<Data><![CDATA[" + Convert.ToBase64String(output) + "]]></Data>"

            xmlDoc.DocumentElement.AppendChild(elemF)
        Next
        Return xmlDoc
    End Function

    ''' <summary> Crea il file XML per un documento di tipo frontespizio di pubblicazione o ritiro </summary>
    ''' <param name="fi">File Frontespizio</param>
    Public Function GetXMLSPFrontespizio(ByVal fi As FileInfo, ByVal signature As String, ByVal res As Resolution) As String
        Try
            Dim retval As String = String.Empty
            Dim XmlDoc As XmlDocument = New XmlDocument()
            Dim xml As StringBuilder = New StringBuilder("<?xml version=""1.0"" encoding=""utf-8"" ?>")

            xml.Append("<Documents>")
            xml.Append("</Documents>")
            XmlDoc.LoadXml(xml.ToString())
            Dim elemF As XmlElement = XmlDoc.CreateElement("Frontespizio")
            elemF.SetAttribute("nome", signature.Replace("/", "-").Replace("\", "-").Replace(":", "").Replace(" ", "_") + "_" + res.Id.ToString)
            elemF.SetAttribute("ext", "pdf")

            Dim file As New FileDocumentInfo(fi)
            Dim sFrontespizio As Byte() = file.GetPdfStream(signature)

            Dim stream() As Byte = Merge(res, sFrontespizio, signature)

            elemF.InnerXml = "<Data><![CDATA[" + Convert.ToBase64String(stream) + "]]></Data>"
            XmlDoc.DocumentElement.AppendChild(elemF)

            retval = XmlDoc.InnerXml
            Return retval
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetXMLSPFrontespizio(ByVal trwF As Dictionary(Of String, String), ByVal res As Resolution, ByVal tw As TabWorkflow,
                ByVal fileRes As FileResolution, ByVal description As String, ByVal signature As String) As String
        Try

            Dim xmlDoc As XmlDocument = New XmlDocument()
            Dim xml As StringBuilder = New StringBuilder("<?xml version=""1.0"" encoding=""utf-8"" ?>")
            Dim sFrontespizio As Byte()

            xml.Append("<Documents>")
            xml.Append("</Documents>")
            xmlDoc.LoadXml(xml.ToString())
            For Each tn As KeyValuePair(Of String, String) In trwF
                Dim elemF As XmlElement = xmlDoc.CreateElement("Frontespizio")
                elemF.SetAttribute("nome", signature.Replace("/", "-").Replace("\", "-").Replace(":", "").Replace(" ", "_") + "_" + res.Id.ToString)
                elemF.SetAttribute("ext", "pdf")

                If (tn.Key = "" Or IsNothing(tn.Key)) Then
                    sFrontespizio = Renderizza(res, tw, fileRes, signature)
                Else
                    Dim FileName As String = If(True, CommonUtil.GetInstance().AppTempPath, "") & tn.Key
                    Dim fi As New FileInfo(FileName)
                    Dim file As New FileDocumentInfo(fi)
                    sFrontespizio = file.GetPdfStream(signature)
                End If

                Dim stream() As Byte = Merge(res, sFrontespizio, signature)

                elemF.InnerXml = "<Data><![CDATA[" + Convert.ToBase64String(stream) + "]]></Data>"
                If xmlDoc.DocumentElement IsNot Nothing Then
                    xmlDoc.DocumentElement.AppendChild(elemF)
                End If
            Next

            Return xmlDoc.InnerXml

        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Private Shared Function Merge(ByVal res As Resolution, ByVal sFrontespizio As Byte(), ByVal signature As String) As Byte()
        Dim docAssumedProposal As New BiblosDocumentInfo(res.Location.DocumentServer, res.Location.ReslBiblosDSDB, res.File.IdAssumedProposal.Value)
        Dim streamAssumedProposal As Byte() = docAssumedProposal.GetPdfStream(signature)

        Dim pdf As New PDF.PdfMerge()
        pdf.AddDocument(sFrontespizio)
        pdf.AddDocument(streamAssumedProposal)


        If Not res.File.IdAttachements Is Nothing And Not IsDBNull(res.File.IdAttachements) Then
            Dim attachments As List(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(res.Location.DocumentServer, res.Location.ReslBiblosDSDB, res.File.IdAttachements)
            For Each attachment As BiblosDocumentInfo In attachments
                pdf.AddDocument(attachment.GetPdfStream(String.Concat(signature, " (Allegato)")))
            Next
        End If

        If DocSuiteContext.Current.ResolutionEnv.IncludeAnnexesInPubblication Then
            If Not res.File.IdAnnexes.Equals(Guid.Empty) Then
                Dim annexes As IList(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(res.Location.DocumentServer, res.File.IdAnnexes)
                For Each annexed As BiblosDocumentInfo In annexes
                    pdf.AddDocument(annexed.GetPdfStream(String.Concat(signature, " (Annesso)")))
                Next
            End If
        End If

        Dim stream As New MemoryStream()
        pdf.Merge(stream)
        Dim watermark As String = String.Empty
        If Not String.IsNullOrEmpty(DocSuiteContext.Current.ResolutionEnv.WebPublishWatermark) Then
            watermark = DocSuiteContext.Current.ResolutionEnv.WebPublishWatermark
        End If
        Dim output As Byte() = Services.StampaConforme.Service.ConvertToPdf(stream.ToArray(), FileHelper.PDF, String.Empty, watermark)
        Return output

    End Function

    Private Shared Function Renderizza(ByVal res As Resolution, ByVal tw As TabWorkflow, ByVal fileRes As FileResolution, ByVal signature As String) As Byte()
        Dim idChain As String = CType(ReflectionHelper.GetPropertyCase(fileRes, tw.FieldDocument), String)
        Dim doc As New BiblosDocumentInfo(res.Location.DocumentServer, res.Location.ReslBiblosDSDB, idChain)
        Return doc.GetPdfStream(signature)
    End Function

    Public Function GetTemplate(ByVal resl As Resolution, ByVal stepDesc As String) As String
        Dim TabWork As New TabWorkflowFacade

        Dim ResolutionType As Integer = resl.Type.Id
        Dim items As List(Of TabWorkflow) = TabWork.GetByResolutionType(ResolutionType)

        For Each s As TabWorkflow In items
            If s.Description.ToLower() = stepDesc.ToLower() Then
                ' Trovato
                Return s.Template
            End If
        Next

        ' Step non trovato
        Return String.Empty
    End Function

    Public Function GetWorkflow(ByVal resl As Resolution, ByVal stepDesc As String) As TabWorkflow
        Dim tabWork As New TabWorkflowFacade

        Dim ResolutionType As Integer = resl.Type.Id
        Dim items As List(Of TabWorkflow) = tabWork.GetByResolutionType(ResolutionType)

        For Each s As TabWorkflow In items
            If s.Description.ToLower() = stepDesc.ToLower() Then
                ' Trovato
                Return s
            End If
        Next

        ' Step non trovato
        Return Nothing
    End Function

    ''' <summary>
    ''' Genera il frontalino di ritiro
    ''' </summary>
    ''' <param name="resl">Atto da ritirare</param>
    ''' <param name="dataRitiro">Data del ritiro</param>
    ''' <param name="tempPath">Directory temporanea dove mettere il file, se vuota usa <see>CommonUtil.AppTempPath</see></param>
    ''' <param name="fileName">Nome del file</param>
    ''' <returns>Nome e percorso del file</returns>
    Public Function GenerateRetireDocument(ByVal resl As Resolution, ByVal dataRitiro As DateTime, ByVal tempPath As String, ByVal fileName As String) As DocumentInfo
        Return GenerateRetireDocument(resl, dataRitiro, tempPath, fileName, Nothing, DocSuiteContext.Current.User.FullUserName)
    End Function

    ''' <summary>
    ''' Genera il frontalino di ritiro
    ''' </summary>
    ''' <param name="resl">Atto da ritirare</param>
    ''' <param name="dataRitiro">Data del ritiro</param>
    ''' <param name="tempPath">Directory temporanea dove mettere il file, se vuota usa <see>CommonUtil.AppTempPath</see></param>
    ''' <param name="fileName">Nome del file</param>
    ''' <returns>Nome e percorso del file</returns>
    Public Function GenerateRetireDocument(ByVal resl As Resolution, ByVal dataRitiro As DateTime, ByVal tempPath As String, ByVal fileName As String, ByVal ApplicationPath As String, ByVal UserName As String) As DocumentInfo
        ' Creo documento di ritiro
        Const data As String = "Frontespizio[LeaveDate]"
        Const stepDesc As String = "Ritiro Pubblicazione"
        Dim template As String = GetTemplate(resl, stepDesc)

        Dim info As FileInfo = ResolutionUtil.GeneraStampaODG(resl, data, dataRitiro, resl.Type.Id, stepDesc, template, tempPath, fileName, applicationPath:=ApplicationPath)

        Return New FileDocumentInfo(info)
    End Function

    ''' <summary> Ritira il documento tramite sharepoint e aggiorna l'atto </summary>
    Public Sub RetireResolution(ByVal file As DocumentInfo, ByRef resl As Resolution, ByVal dataRitiro As DateTime)
        ' Calcolo la signature
        Dim resolutionFacade As New ResolutionFacade
        Dim sign As String = String.Format("{0}: Ritiro Albo {1:dd/MM/yyyy}", resolutionFacade.SqlResolutionGetNumber(resl.Id, , , , True), dataRitiro)
        ' Creo XML con dati dei documenti
        Dim xmlDocuments As XmlDocument = GetXmlSp(New List(Of DocumentInfo) From {file}, sign, resl.Id)
        Dim docs As String = xmlDocuments.InnerXml

        ' Creo XML con Metadati
        Dim others As String = GetXmlOther(resl)
        Try
            ' Eseguo il ritiro su Web
            Dim retVal As ReturnValue = ServiceSHP.InsertInRetireArea(resl.Container.Name, resl.Year, resl.Number.ToString(),
                                        resl.AdoptionDate.Value, resl.ResolutionObject, DateTime.Now,
                                        resl.WebPublicationDate.Value, dataRitiro, resl.Type.Description,
                                        others, docs, resl.WebSPGuid)

            ' Imposto i valori della pubblicazione
            resl.WebSPGuid = retVal.Guid
            resl.WebState = Resolution.WebStateEnum.Revoked
            resl.WebRevokeDate = dataRitiro
            Factory.ResolutionFacade.Update(resl)

            ' Aggiorno la tabella WebPublication
            Dim webPublicationFacade As WebPublicationFacade = New WebPublicationFacade()
            Dim webPublicationList As IList(Of WebPublication) = webPublicationFacade.GetByResolution(resl)
            If (webPublicationList.Count > 0) Then
                webPublicationList(0).Status = 3
                webPublicationFacade.Update(webPublicationList(0))
            End If

            Factory.ResolutionLogFacade.Log(resl, ResolutionLogType.WP, "File ritirato correttamente")
        Catch ex As Exception
            Throw New DocSuiteException("Errore Pubblicazione Ritiro internet.", ex)
        End Try
    End Sub

    Public Function CheckArea(resl As Resolution, area As String) As Boolean
        Return ServiceSHP.CheckArea(resl.WebSPGuid, area)
    End Function

    ''' <summary>
    ''' Genera il documento PDF che deve essere pubblicato su SharePoint
    ''' Corrisponde probabilmente al documento principale ma può essere personalizzato tramite TabWorkflow
    ''' </summary>
    ''' <param name="currentResolution">L'atto da pubblicare</param>
    ''' <param name="idCatena">idCatena del documento generato e inserito in Biblos</param>
    ''' <param name="segnatura">segnatura da applicare al documento</param>
    ''' <param name="privacyPreview">booleano per definire se il documento generato è definitivo da inserire oppure se è una preview da stampare</param>
    ''' <returns>Ritorna il percorso completo al file nella Temp</returns>
    ''' <remarks></remarks>
    Public Function GetMergedPublicationDocument(ByRef currentResolution As Resolution, ByRef idCatena As Nullable(Of Long), ByVal segnatura As String, ByVal privacyPreview As Boolean) As String
        '' Carico le tipologie di documento che devono essere inserite
        If String.IsNullOrEmpty(segnatura) Then segnatura = "*"
        '' Carico il WorkStep di Pubblicazione
        Dim tabWorkflowFacade As New TabWorkflowFacade()
        Dim workStep As TabWorkflow = tabWorkflowFacade.GetByDescription("Pubblicazione", currentResolution.WorkflowType)
        Dim fileResolutionDocTypesToPublish As IList(Of ResolutionFacade.DocType) = If(privacyPreview, DocTypesToPrivacyPublishPreview(workStep), DocTypesToPublish(currentResolution, workStep))
        Const fileName As String = "publicatedDoc.pdf"

        If currentResolution Is Nothing OrElse String.IsNullOrEmpty(fileName) OrElse fileResolutionDocTypesToPublish Is Nothing OrElse fileResolutionDocTypesToPublish.Count = 0 Then Throw New Exception("Impossibile generare il documento di pubblicazione. Fornire quali documenti dell'atto includere!")
        '' Condizioni di esistenza soddisfatte, caricamento variabili di ambiente
        Dim documentServer As String = currentResolution.Location.DocumentServer
        Dim documentArchive As String = currentResolution.Location.ReslBiblosDSDB
        Dim fileResolutionFacade As New FileResolutionFacade()
        Dim fileResolution As FileResolution = fileResolutionFacade.GetByResolution(currentResolution)(0)

        ''Attivo il manager per il merge dei PDF
        Dim managerPdf As PdfMerge = New PdfMerge()

        ''Estrazione dei documenti
        For Each reslDocType As ResolutionFacade.DocType In fileResolutionDocTypesToPublish
            Dim currentBiblosId As Integer
            Select Case reslDocType
                Case ResolutionFacade.DocType.Adozione
                    currentBiblosId = CType(fileResolution.IdAssumedProposal, Integer)
                Case ResolutionFacade.DocType.Allegati
                    currentBiblosId = CType(fileResolution.IdAttachements, Integer)
                Case ResolutionFacade.DocType.Disposizione
                    currentBiblosId = CType(fileResolution.IdResolutionFile, Integer)
                Case ResolutionFacade.DocType.Frontespizio
                    currentBiblosId = CType(fileResolution.IdFrontespizio, Integer)
                Case ResolutionFacade.DocType.OrganoControllo
                    currentBiblosId = CType(fileResolution.IdControllerFile, Integer)
                Case ResolutionFacade.DocType.Proposta
                    currentBiblosId = CType(fileResolution.IdProposalFile, Integer)
                Case ResolutionFacade.DocType.UltimaPagina
                    currentBiblosId = CType(fileResolution.IdUltimaPagina, Integer)
                Case ResolutionFacade.DocType.AllegatiRiservati
                    currentBiblosId = CType(fileResolution.IdPrivacyAttachments, Integer)
                Case ResolutionFacade.DocType.FrontespizioRitiro
                    currentBiblosId = CType(fileResolution.IdFrontalinoRitiro, Integer)
                Case ResolutionFacade.DocType.PrivacyPublicationDocument
                    currentBiblosId = CType(fileResolution.IdPrivacyPublicationDocument, Integer)
                Case Else
                    Throw New Exception(String.Format("Il tipo di elemento denominato {0} non è riconosciuto come tipo valido per gli atti", reslDocType.ToString()))
            End Select

            ''Estraggo il documento specifico da Biblos
            If currentBiblosId > 0 Then
                ''Carico la lista completa dei documenti
                Dim documentsDocumentList As List(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(documentServer, documentArchive, currentBiblosId)
                For Each doc As BiblosDocumentInfo In documentsDocumentList
                    managerPdf.AddDocument(doc.SavePdfNoSignature(New DirectoryInfo(CommonUtil.GetInstance().AppTempPath), FileHelper.UniqueFileNameFormat(doc.Name, DocSuiteContext.Current.User.UserName)).FullName)
                Next
            End If
        Next

        ''A questo punto il managerPDF contiene tutti i documenti da esportare
        ' Salvo il file unito
        Dim tempFileName As String = FileHelper.UniqueFileNameFormat(fileName, DocSuiteContext.Current.User.UserName)
        Dim tempFileFullPath As String = CommonUtil.GetInstance().AppTempPath & tempFileName
        managerPdf.Merge(tempFileFullPath)

        ''Carico su Biblos il documento creato se necessario (ovvero idChain è collegato realmente ad un oggetto)
        If idCatena.HasValue Then
            Dim fileDocumentInfo As New FileDocumentInfo(New FileInfo(tempFileFullPath))
            fileDocumentInfo.Signature = segnatura
            fileDocumentInfo.AddAttribute(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE, 0)
            Dim docBiblos As BiblosDocumentInfo = fileDocumentInfo.ArchiveInBiblos(currentResolution.Location.DocumentServer, currentResolution.Location.ReslBiblosDSDB)
            idCatena = docBiblos.BiblosChainId
            If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                Factory.ResolutionLogFacade.Insert(currentResolution, ResolutionLogType.LP, String.Format("Associato livello privacy {0} al {1} {2} [{3}]", 0, "documento", docBiblos.Name, docBiblos.DocumentId))
            End If
        End If

        ''Restituisco il percorso sulla Temp via HTTP
        Return Path.Combine(If(privacyPreview, CommonUtil.GetInstance.AppTempHttp, CommonUtil.GetInstance.AppTempPath), tempFileName)
    End Function

    ''' <summary>
    ''' Metodo che ritorna le tipologie di documento che devono essere pubblicate
    ''' </summary>
    Private Shared Function DocTypesToPublish(ByRef currentResolution As Resolution, workstep As TabWorkflow) As IList(Of ResolutionFacade.DocType)
        Dim docTypes As New List(Of ResolutionFacade.DocType)()
        Dim fileResolutionFacade As New FileResolutionFacade
        '' Se è stato caricato un documento privacy mando quello (come convenzione lo trovo in IdControllerFile)
        Dim fileResolution As IList(Of FileResolution) = fileResolutionFacade.GetByResolution(currentResolution)
        If fileResolution.Count > 0 Then
            If fileResolution(0).IdPrivacyPublicationDocument > 0 Then
                docTypes.Add(ResolutionFacade.DocType.Frontespizio)
                docTypes.Add(ResolutionFacade.DocType.PrivacyPublicationDocument)
            Else
                Dim docTypesString As String() = TabWorkflowFacade.GetPipedParametersFromWorkflowManagedData(workstep.ManagedWorkflowData, "DocTypesToPublish")
                docTypes.AddRange(From docType In docTypesString Select DirectCast([Enum].Parse(GetType(ResolutionFacade.DocType), docType), ResolutionFacade.DocType))
            End If
        End If
        Return docTypes
    End Function

    ''' <summary>
    ''' Metodo che ritorna le tipologie di documento che devono essere mostrate prima della pubblicazione
    ''' </summary>
    Private Shared Function DocTypesToPrivacyPublishPreview(workstep As TabWorkflow) As IList(Of ResolutionFacade.DocType)
        Dim docTypesString As String() = TabWorkflowFacade.GetPipedParametersFromWorkflowManagedData(workstep.ManagedWorkflowData, "DocTypesToPrivacyPublishPreview")
        Return (From docType In docTypesString Select DirectCast([Enum].Parse(GetType(ResolutionFacade.DocType), docType), ResolutionFacade.DocType)).ToList()
    End Function

    ''' <summary>
    ''' Metodo che genera un frontalino di pubblicazione per una specifica Resolution a partire dai dati di pubblicazione
    ''' </summary>
    Public Shared Function InserisciFrontalinoPubblicazione(currentResolution As Resolution, publicationDate As Date, stepId As Short) As FileDocumentInfo
        Dim resolutionFacade As New ResolutionFacade
        Dim fileDocumentInfo As New FileDocumentInfo(ResolutionUtil.GeneraFrontalino(publicationDate, currentResolution, "Pubblicazione", stepId, String.Empty))
        fileDocumentInfo.Signature = resolutionFacade.ResolutionSignature(currentResolution, ResolutionType.UploadDocumentType.Frontespizio)
        fileDocumentInfo.Name = "Frontalino di pubblicazione.pdf"

        fileDocumentInfo.AddAttribute(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE, 0)

        Dim docBiblos As BiblosDocumentInfo = fileDocumentInfo.ArchiveInBiblos(currentResolution.Location.DocumentServer, currentResolution.Location.ReslBiblosDSDB)
        Dim idDocumentoFrontalinoPubblicazione As Integer = docBiblos.BiblosChainId

        If DocSuiteContext.Current.PrivacyLevelsEnabled Then
            FacadeFactory.Instance.ResolutionLogFacade.Insert(currentResolution, ResolutionLogType.LP, String.Format("Associato livello privacy {0} al {1} {2} [{3}]", 0, "documento", docBiblos.Name, docBiblos.DocumentId))
        End If

        resolutionFacade.SqlResolutionDocumentUpdate(currentResolution.Id, idDocumentoFrontalinoPubblicazione, ResolutionFacade.DocType.Frontespizio)
        Return fileDocumentInfo
    End Function

    Public Function GetSharepointPublicationXml(currentResolution As Resolution, watermark As [String], ByRef idCatena As Integer) As String
        ''Documento finale da pubblicare
        Dim signature As String = Factory.ResolutionFacade.ResolutionSignature(currentResolution, ResolutionType.UploadDocumentType.Pubblicazione)
        Dim xmlDoc As XmlDocument = GetXmlSp(New List(Of DocumentInfo) From {New FileDocumentInfo(New FileInfo(GetMergedPublicationDocument(currentResolution, Nothing, signature, False)))}, signature, currentResolution.Id, watermark)

        '' Se passo idCatena al metodo GetMergedDocument ottengo la memorizzazione su Biblos del documento senza Watermark --> e posso evitare il codice seguente
        '' Se invece passo Nothing allora devo estrarre successivamente dall'xml il documento con Watermark

        'Memorizzo su Biblos // devo farlo qui perchè il GetMergedDocument alternativamente produce il file senza watermark
        If idCatena = 0 Then
            Dim stream As Byte() = Convert.FromBase64String(xmlDoc.SelectNodes("//Frontespizio/Data")(0).InnerText)
            Dim outputMemoryDocumentInfo As New MemoryDocumentInfo(stream, "Documento pubblicato.pdf")
            outputMemoryDocumentInfo.Signature = signature

            If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                outputMemoryDocumentInfo.AddAttribute(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE, 0)
            End If

            Dim docBiblos As BiblosDocumentInfo = outputMemoryDocumentInfo.ArchiveInBiblos(currentResolution.Location.DocumentServer, currentResolution.Location.ReslBiblosDSDB)
            idCatena = docBiblos.BiblosChainId

            If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                Factory.ResolutionLogFacade.Insert(currentResolution, ResolutionLogType.LP, String.Format("Associato livello privacy {0} al {1} {2} [{3}]", 0, "documento", docBiblos.Name, docBiblos.DocumentId))
            End If
        End If

        Return xmlDoc.InnerXml.ToString()
    End Function

End Class
