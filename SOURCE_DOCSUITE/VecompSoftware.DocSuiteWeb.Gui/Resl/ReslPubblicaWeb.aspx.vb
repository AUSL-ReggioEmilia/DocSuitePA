Imports System.Collections.Generic
Imports VecompSoftware.Helpers.PDF
Imports VecompSoftware.Helpers
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.IO
Imports VecompSoftware.DocSuiteWeb.Services.CMVGroup
Imports VecompSoftware.Services.Biblos.Models

Partial Public Class ReslPubblicaWeb
    Inherits ReslBasePage

    Private _gridBarController As ResolutionGridBarController
    Protected Property GridBarController() As ResolutionGridBarController
        Get
            Return _gridBarController
        End Get
        Set(ByVal value As ResolutionGridBarController)
            _gridBarController = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Inizializzazioni
        Initialize()
        InitializeAjaxSettings()
        InitializeController()

        If Not Me.IsPostBack Then
            DoSearch(False)
        End If
    End Sub

    Private Sub Initialize()
        Me.MasterDocSuite.TitleVisible = False
        AddHandler uscReslGrid.Grid.DataBound, AddressOf DataSourceChanged
        uscReslGrid.DisableColumn(uscReslGrid.COLUMN_DOCUMENT_SIGN)
        'uscReslGrid.DisableColumn(uscReslGrid.COLUMN_ADOPTIONDATE)
        uscReslGrid.DisableColumn(uscReslGrid.COLUMN_CATEGORY)
        uscReslGrid.DisableColumn(uscReslGrid.COLUMN_STATUS)
        uscReslGrid.DisableColumn(uscReslGrid.COLUMN_TIPOC)
        uscReslGrid.DisableColumn(uscReslGrid.COLUMN_PROPOSER_CODE)
    End Sub

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(uscReslGrid.Grid, uscReslGrid.Grid, MasterDocSuite.AjaxLoadingPanelSearch)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscReslGrid.Grid, lblHeader)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlType, uscReslGrid.Grid)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlType, lblHeader)
    End Sub

    Private Sub InitializeGridBar()
        uscReslGridBar.AjaxEnabled = True
        uscReslGridBar.AjaxLoadingPanel = Me.MasterDocSuite.AjaxLoadingPanelSearch
        uscReslGridBar.PubblicaWebButton.Visible = True
    End Sub

    Private Sub InitializeController()
        'GridBar
        InitializeGridBar()
        GridBarController = New ResolutionGridBarController(uscReslGridBar)
        GridBarController.LoadConfiguration(ResolutionEnv.Configuration)
        GridBarController.EnableMiddle = False ' Nascondo la parte destra
        GridBarController.BindGrid(uscReslGrid.Grid)

        AddHandler uscReslGridBar.PubblicaWebButton.Click, AddressOf WebPublishClick

        GridBarController.Show()

        uscReslGridBar.PrintButton.Visible = False
        uscReslGridBar.DocumentsButton.Visible = False
    End Sub

    Private Sub WebPublishClick(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim result As String = String.Empty
        Try
            Dim selectedDocuments As Dictionary(Of Integer, uscReslGridBar.WebDocument) = uscReslGridBar.GetSelectedDocuments()

            If selectedDocuments.Count > 0 Then
                ' Scrivo su Log il numero di elementi selezionati
                FileLogger.Info(LoggerName, "WebPublishClick - Elementi selezionati: " & selectedDocuments.Count.ToString)

                ' Recupero la cartella di destinazione
                Dim folder As String = ResolutionEnv.WebPublishFolder & DateTime.Now.ToString("yyyyMMddmmss") & "\"
                Try
                    If Directory.Exists(folder) Then Directory.Delete(folder, True)
                Catch ex As Exception
                    Throw New Exception("Errore rilevato in fase di creazione cartella di pubblicazione: " & folder, ex)
                End Try

                If (Not Directory.CreateDirectory(folder).Exists) Then
                    Throw New Exception("Errore in creazione cartella di pubblicazione: " & folder)
                End If

                Dim filenameroot As String = ResolutionEnv.WebPublishHTMLFile ' "Documento_"
                Dim filecountAtto As Integer = 0
                Dim filecountDelibera As Integer = 0

                Dim errorString As String = String.Empty
                Dim countOk As Integer = 0
                Dim countKo As Integer = 0
                ' Eseguo un ciclo sui documentio selezionati nella griglia
                Dim counter As Integer = 0
                For Each kv As KeyValuePair(Of Integer, uscReslGridBar.WebDocument) In selectedDocuments
                    counter = counter + 1
                    'Next
                    'For y As Integer = 0 To _selectedDocuments.Count - 1
                    ' Recupero il documento dall'elenco
                    Dim document As uscReslGridBar.WebDocument = kv.Value ' _selectedDocuments.Values(y)
                    Try
                        ' Cartella di destinazione del documento finale da pubblicare
                        Dim myFolder As String = folder & document.Resolution.Type.Description & "/"
                        If Not Directory.Exists(myFolder) Then
                            If (Not Directory.CreateDirectory(myFolder).Exists) Then
                                Throw New Exception("Errore in creazione cartella di pubblicazione: " & myFolder)
                            End If
                        End If

                        Dim ext As String

                        If Not document.DocumentsIds Is Nothing AndAlso document.DocumentsIds.Count > 0 Then 'Verifico i componenti del documento da pubblicare
                            Dim files As New List(Of String)
                            ' Eseguo un ciclo sui documenti selezionati dalla catena
                            For Each docGuid As Guid In document.DocumentsIds
                                Dim doc As New BiblosDocumentInfo(docGuid)
                                Dim file As FileInfo = BiblosFacade.SaveUniquePdfToTempNoSignature(doc, "WebPubTemp.pdf")

                                ' Salvo il nome del documento temporaneo
                                files.Add(file.FullName)
                            Next

                            ' A questo punto ho un elenco di documento pdf da fondere e convertire per la pubblicazione
                            ' Devo fondere tutti i pdf, spedire alla stampa conforme per la signature e per la gestione dei diritti
                            ext = "pdf"
                            ' Imposto il nome del file di destinazione 
                            Dim fileName As String = String.Empty
                            Select Case document.Resolution.Type.Id
                                Case 0 'Atto
                                    fileName = String.Concat(filenameroot, filecountAtto, ".", ext)
                                Case 1 'Delibera
                                    fileName = String.Concat(filenameroot, filecountDelibera, ".", ext)
                            End Select

                            ' Eseguo la fusione dei documenti
                            Dim tempFile2 As String = Path.Combine(CommonUtil.GetInstance().AppTempPath, FileHelper.UniqueFileNameFormat(fileName, DocSuiteContext.Current.User.UserName))
                            Try
                                If files.Count > 1 Then
                                    FileLogger.Info(LoggerName, "Inizio fusione documenti.")
                                    Dim managerPdf As New PdfMerge()
                                    For Each doc As String In files
                                        managerPdf.AddDocument(doc)
                                    Next
                                    managerPdf.Merge(tempFile2)
                                    'PdfManager.PdfMerge.MergeFiles(tempFile2, files.ToArray())
                                    FileLogger.Info(LoggerName, "Documenti fusi in " & tempFile2)
                                Else
                                    FileLogger.Info(LoggerName, "Fusione non necessaria, singolo documento.")
                                    File.Copy(files.Item(0), tempFile2)
                                End If
                            Catch ex As Exception
                                FileLogger.Warn(LoggerName, "[WebPublishClick] Si è verificato un errore in fase di fusione documenti.", ex)
                            End Try

                            ' Conversione del documento per stampe conforme
                            Dim lbl As String = ParseString(ResolutionEnv.WebPublishSign, document, String.Empty, counter, document.DocumentsIds.Count)
                            Dim label As String = String.Format(ResolutionEnv.WebPublishSignTag, lbl)

                            Dim info As New FileInfo(tempFile2)
                            Dim numBytes As Long = info.Length
                            Dim myStream As New FileStream(tempFile2, FileMode.Open, FileAccess.Read)
                            Dim br As New BinaryReader(myStream)
                            Dim content As Byte() = br.ReadBytes(CInt(numBytes))
                            br.Close()
                            myStream.Close()

                            Try
                                FileLogger.Info(LoggerName, "Conversione documento per pubblicazione Web")
                                content = VecompSoftware.Services.StampaConforme.Service.ConvertToPdf(content, ext, label)
                            Catch ex As Exception
                                FileLogger.Error(LoggerName, "Errore in conversione documento: " & ex.Message)
                                Throw New Exception("Errore in fase di conversione documento.", ex)
                            End Try

                            Try
                                Dim tempDocument As New MemoryDocumentInfo(content, fileName)
                                Dim cmvGroup As New CmvGroup()
                                If Not cmvGroup.Publish(document.Resolution, tempDocument, result) Then
                                    Throw New InvalidOperationException(result)
                                End If
                            Catch ex As Exception
                                Dim message As String = "Errore in fase di invio documento al portale: {0} - {1}"
                                message = String.Format(message, fileName, result)
                                Throw New Exception(message, ex)
                            End Try

                            ' Salvo i dati su DB
                            document.Resolution.WebState = Resolution.WebStateEnum.Published
                            document.Resolution.WebPublicationDate = DateTime.Today
                            document.Resolution.WebPublicatedDocuments = If(document.Privacy, "1", "0")
                            'For Each item As Guid In document.DocumentsIds
                            '    document.Resolution.WebPublicatedDocuments = document.Resolution.WebPublicatedDocuments & String.Format("|{0}", item)
                            'Next
                            'TODO Verificare se è necessario memorizzare tutti i nomi
                            document.Resolution.WebPublicatedDocuments = document.Resolution.WebPublicatedDocuments & "|"
                            Facade.ResolutionFacade.Save(document.Resolution)
                            ' -- FINE PUBBLICAZIONE ATTO, registro sul log
                            Facade.ResolutionLogFacade.Insert(document.Resolution, ResolutionLogType.RP, "Pubblicazione avvenuta con successo: " & result)
                            countOk = countOk + 1
                        End If
                    Catch ex As Exception
                        ' Registro l'errore
                        Facade.ResolutionLogFacade.Log(document.Resolution, ResolutionLogType.RE, "Errore in fase di pubblicazione: " & ex.Message)
                        If Not String.IsNullOrEmpty(errorString) Then
                            errorString = errorString & ", "
                        End If
                        errorString = errorString & document.Resolution.Number
                        countKo = countKo + 1
                        FileLogger.Error(LoggerName, ex.Message, ex)
                    End Try
                Next

                DoSearch(True)

                If countKo > 0 Then
                    ' si sono verificati degli errori
                    AjaxAlert(String.Format("{0} pubblicazion{2} avvenut{3} con successo. {1} error{4} rilevat{5}.", countOk, countKo, If(countOk = 1, "e", "i"), If(countOk = 1, "a", "e"), If(countKo = 1, "e", "i"), If(countKo = 1, "o", "i")))
                Else
                    AjaxAlert(String.Format("Pubblicazione atti avvenuta con successo. {0} document{1} pubblicat{1}.", countOk, If(countOk = 1, "o", "i")))
                End If
            Else
                AjaxAlert("Nessun atto selezionato. Selezionare almeno 1 atto da pubblicare.")
            End If
        Catch ex As Exception
            FileLogger.Warn(LoggerName, ex.Message, ex)
            AjaxAlert(ex.Message)
        End Try
    End Sub

    ''' <summary> Crea la firma per la pubblicazione. </summary>
    ''' <remarks>
    ''' Copiata in ResolutionFacade.ComposeWebPubblicationSignature, valutare come unire i due codici
    ''' </remarks>
    <Obsolete("Che ci fa in pagina? Va inserita nella business logic, leggi commento xml")>
    Private Function ParseString(ByVal str As String, ByVal doc As uscReslGridBar.WebDocument, ByVal filename As String, ByVal counter As Integer, ByVal total As Integer) As String

        str = str.Replace("{filename}", filename)
        str = str.Replace("{year}", doc.Resolution.Year)
        str = str.Replace("{number}", doc.Resolution.Number.ToString().PadLeft(5, "0"c))
        str = str.Replace("{fullnumber}", Facade.ResolutionFacade.CalculateFullNumber(doc.Resolution, doc.Resolution.Type.Id, False))
        str = str.Replace("{type}", doc.Resolution.Type.Description)

        If total > 1 Then
            str = str.Replace("{counter}", "(Documento " & counter & ")")
        Else
            str = str.Replace("{counter}", String.Empty)
        End If

        If doc.Resolution.AdoptionDate.HasValue Then
            str = str.Replace("{AdoptionDate}", doc.Resolution.AdoptionDate.Value.ToString("dd/MM/yyyy"))
        Else
            str = str.Replace("{AdoptionDate}", String.Empty)
        End If
        If doc.Resolution.PublishingDate.HasValue Then
            str = str.Replace("{PublishingDate}", doc.Resolution.PublishingDate.Value.ToString("dd/MM/yyyy"))
        Else
            str = str.Replace("{PublishingDate}", String.Empty)
        End If
        If doc.Resolution.EffectivenessDate.HasValue Then
            str = str.Replace("{EffectivenessDate}", doc.Resolution.EffectivenessDate.Value.ToString("dd/MM/yyyy"))
        Else
            str = str.Replace("{EffectivenessDate}", String.Empty)
        End If
        str = str.Replace("{object}", doc.Resolution.ResolutionObject) ' If(doc.Privacy, doc.Resolution.ResolutionObjectPrivacy, doc.Resolution.ResolutionObject))

        Return str

    End Function


#Region "Grid Events"
    Protected Sub DataSourceChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim gvResolutions As BindGrid = uscReslGrid.Grid
        If gvResolutions.DataSource IsNot Nothing Then
            lblHeader.Text = Facade.TabMasterFacade.TreeViewCaption & " - Risultati (" & gvResolutions.DataSource.Count & "/" & gvResolutions.VirtualItemCount & ")"
        Else
            lblHeader.Text = Facade.TabMasterFacade.TreeViewCaption & " - Nessun Risultato"
        End If
        MasterDocSuite.HistoryTitle = lblHeader.Text
    End Sub
#End Region

#Region "Search"
    Private Sub DoSearch(ByVal reloading As Boolean)
        Dim gvResolutions As BindGrid = uscReslGrid.Grid

        Dim finder As NHibernateResolutionFinder = New NHibernateResolutionFinder("ReslDB") ' SessionSearchController.LoadSessionFinder(SessionSearchController.SessionFinderType.ReslFinderType)
        finder.WebState = Resolution.WebStateEnum.None
        finder.CheckPublication = True
        finder.SortExpressions.Add("Year", "ASC")
        finder.SortExpressions.Add("Number", "ASC")
        ' Filtro solo per delibere e determine attive.
        finder.IdStatus = 0
        If (ddlType.SelectedValue.Equals("0")) Then
            finder.Pubblicata = True
            finder.DateFrom = Date.Today.AddDays(-10)
        ElseIf (ddlType.SelectedValue.Equals("1")) Then
            finder.Pubblicata = True
            finder.DateTo = Date.Today.AddDays(-10)
        End If

        gvResolutions.PageSize = finder.PageSize
        gvResolutions.Finder = finder
        gvResolutions.DataBindFinder()
        gvResolutions.Visible = True

        If (Not reloading) And finder.Count() = 0 Then
            AjaxAlert("Non ci sono documenti da pubblicare.")
        End If
    End Sub
#End Region

    Private Sub ddlType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlType.SelectedIndexChanged
        DoSearch(True)
    End Sub

End Class