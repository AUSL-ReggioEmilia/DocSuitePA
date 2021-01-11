Imports System.IO
Imports System.Xml
Imports System.Data.OleDb
Imports System.Linq
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data.Formatter
Imports VecompSoftware.Services.Biblos.Models

Public Class CommImport
    Inherits BaseCommImport

#Region " Fields "

    Protected ErrorFileName As String
    Protected ExcelConnection As OleDbConnection
    Protected SheetName As String
    Protected Oggetto As String = String.Empty
    Protected Status As String = String.Empty
    Protected FileDocInfo As FileInfo
    Protected TempFileDoc As String = String.Empty
    Protected FileXmlInfo As FileInfo
    Protected TempFileXml As String = String.Empty
    Protected DataRow As DataRow
    Protected OptionalMetadata As DataTable
    Protected AdUser As String = String.Empty
    Protected AdPassword As String = String.Empty

#End Region

#Region " Properties "

    ''' <summary> Tabella dei metadati </summary>
    Public Property TabellaMetadati() As DataTable
        Get
            Return _metadata
        End Get
        Set(ByVal value As DataTable)
            _metadata = value
        End Set
    End Property

#End Region

#Region " Constructors "

    ''' <summary> Costruttore di default </summary>
    Public Sub New()
        MyBase.New()
        LoadImpersonatorInfo()
    End Sub

    Public Sub New(ByVal container As String, ByVal type As String)
        MyBase.New(container, type)
        LoadImpersonatorInfo()
    End Sub

#End Region

#Region " Methods "

    Private Sub LoadImpersonatorInfo()
        AdUser = DocSuiteContext.Current.CurrentTenant.DomainUser
        AdPassword = DocSuiteContext.Current.CurrentTenant.DomainPassword
    End Sub

    ''' <summary> Controlla i file nella dir di input per trovare i legami fra xml e doc </summary>
    ''' <returns>la tabella con i file xml e doc e il relativo status</returns>
    Public Overrides Function CheckFiles() As DataTable
        Return CheckFiles(False)
    End Function

    Public Overloads Function CheckFiles(ByVal onlyPairs As Boolean) As DataTable

        Dim xmlDoc As New XmlDocument

        Dim dirInfo As New DirectoryInfo(_dirInput)
        Dim fileInfo As FileInfo

        Dim resTable As New DataTable
        resTable.Columns.Add("FILEXML", Type.GetType("System.String"))
        resTable.Columns.Add("FILEDOC", Type.GetType("System.String"))
        resTable.Columns.Add("STATUS", Type.GetType("System.Boolean"))

        'Ciclo su tutta la directory
        For Each fileInfo In dirInfo.GetFiles()
            DataRow = resTable.NewRow
            Dim addrow As Boolean = False
            If fileInfo.Extension.ToUpper = ".XML" Then
                Try
                    xmlDoc.Load(fileInfo.FullName)
                    Dim xmlNode As XmlNode = xmlDoc.SelectSingleNode("//Document")
                    Dim sFileName As String = xmlNode.Attributes("DocumentFileName").InnerText()
                    DataRow("FILEXML") = fileInfo.Name.ToUpper()
                    DataRow("FILEDOC") = sFileName
                    DataRow("STATUS") = 1
                    addrow = True
                Catch ex As Exception
                    DataRow("FILEXML") = fileInfo.Name.ToUpper()
                    DataRow("FILEDOC") = ""
                    DataRow("STATUS") = 1
                End Try
            Else
                DataRow("FILEXML") = ""
                DataRow("FILEDOC") = fileInfo.Name.ToUpper()
                DataRow("STATUS") = 1
            End If

            If Not onlyPairs OrElse addrow Then
                resTable.Rows.Add(DataRow)
            End If
        Next

        Dim sFile As String
        Dim myRows() As DataRow
        For Each DataRow In resTable.Rows
            If CStr(DataRow("FILEXML")).Length > 0 Then
                'file xml verifico che il relativo file sia presente
                If CStr(DataRow("FILEDOC")).Length > 0 Then
                    sFile = dirInfo.FullName + "\" + CStr(DataRow("FILEDOC"))
                    fileInfo = New FileInfo(sFile)
                    If Not fileInfo.Exists Then DataRow("STATUS") = 0
                Else
                    DataRow("STATUS") = 0
                End If
            Else
                'File documento devo verificare che sia referenziato
                sFile = CStr(DataRow("FILEDOC"))
                myRows = resTable.Select("FILEXML <> '' AND FILEDOC = '" + sFile + "' ")
                If myRows.Length = 0 Then DataRow("STATUS") = 0
            End If
        Next

        'Elimino i file DOC OK in quanto già associati all'xml
        'Quindi nella datatable si avranno: -XML e DOC OK XML e DOC KO '' e DOC KO
        myRows = resTable.Select("STATUS = 1 AND  FILEXML = '' ")

        For Each DataRow In myRows
            resTable.Rows.Remove(DataRow)
        Next

        Return resTable

    End Function

    ''' <summary>
    ''' Metodo che prepara l'importazione settando le proprietà
    ''' e lanciando il thread di importazione
    ''' </summary>
    ''' <param name="Protocollo">Contenitore con le proprietà comuni dei protocolli</param>
    ''' <returns>Booleano di errore</returns>
    Public Overrides Function InserimentoProtocollo(ByVal protocollo As Protocol, ByVal all As Boolean) As Boolean

        _protocol = protocollo

        _errorsTable = New DataTable("Errori")
        _errorsTable.Columns.Add("FILEXML", Type.GetType("System.String"))
        _errorsTable.Columns.Add("FILEDOC", Type.GetType("System.String"))
        _errorsTable.Columns.Add("ERROR", Type.GetType("System.String"))

        'Init
        Try
            Dim _importSubFolder As String = Now.ToString("yyyyMMdd_HHmmss")
            _outputDirErrorsInfo = New DirectoryInfo(_dirErrors)
            _outputDirOutInfo = New DirectoryInfo(_dirOutput + "\" + _importSubFolder)
            ErrorFileName = _importSubFolder
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

        If all Then
            'Ciclo su tutti i files nella directory input
            _inputFileNames = Directory.GetFiles(_dirInput, "*.xml")
        Else
            Dim dtFile As DataTable = CheckFiles() ' oImport.CheckFiles()
            Dim rows As DataRow() = dtFile.Select("STATUS=True")

            Dim top As Integer = If(DocSuiteContext.Current.ProtocolEnv.ProtocolImportLimit = 0, rows.Length, Math.Min(rows.Length, DocSuiteContext.Current.ProtocolEnv.ProtocolImportLimit))

            ReDim _inputFileNames(top - 1)

            For i As Integer = 0 To top - 1
                _inputFileNames(i) = _dirInput & "\" & rows(i)(0).ToString()
            Next
        End If



        If _inputFileNames.Length > 0 Then
            _imported = 0
            _errors = 0
            Dim task As MultiStepLongRunningTask = GlobalAsax.LongRunningTask
            task.TaskUser = DocSuiteContext.Current.User.UserName
            task.StepsCount = _inputFileNames.Length
            task.TaskToExecute = New MultiStepLongRunningTask.TaskToExec(AddressOf ImportProtocol)
            task.RunTask()

            Return True
        Else
            Return False
        End If


    End Function

    ''' <summary>
    ''' Subroutine di importazione dei dati
    ''' </summary>
    ''' <param name="currentStep">Numero del passaggio corrente</param>
    Private Sub ImportProtocol(ByVal currentStep As Int32)

        Dim xmldocument As New XmlDocument
        xmldocument.Load(_inputFileNames(currentStep))

        FileXmlInfo = New FileInfo(_inputFileNames(currentStep))

        Dim pdfFileInfo As New FileInfo(_dirInput & "/" & xmldocument.SelectSingleNode("//Document").Attributes("DocumentFileName").Value)

        Try
            Dim importer As IProtocolImporter
            If Not String.IsNullOrEmpty(DocSuiteContext.Current.ProtocolEnv.ProtocolImportClass) Then
                Try
                    importer = ProtocolImportFactory.GetProtocolImporter(DocSuiteContext.Current.ProtocolEnv.ProtocolImportClass)
                Catch ex As Exception
                    Throw New Exception(String.Format("Impossibile caricare l'importatore corretto: {0}.", ex.Message), ex)
                End Try

                Dim template As New ProtocolTemplate
                template.Category = _protocol.Category
                template.Container = _protocol.Container
                template.DocumentType = _protocol.DocumentType
                template.Location = _protocol.Location
                If DocSuiteContext.Current.ProtocolEnv.IsProtocolAttachLocationEnabled Then
                    template.AttachLocation = _protocol.AttachLocation
                End If
                template.Note = _protocol.Note
                template.Status = _protocol.Status
                template.Type = _protocol.Type

                Dim prot As Protocol = importer.Import(xmldocument, pdfFileInfo, template)

                'Tutto OK 
                If Not Directory.Exists(_dirOutput) Then
                    Directory.CreateDirectory(_dirOutput)
                End If

                Dim sFileOut As String = _dirOutput + "\IMPORT.LOG"
                Dim oFileInfoO As New FileInfo(sFileOut)

                Dim fs As FileStream
                If oFileInfoO.Exists() Then
                    fs = New FileStream(sFileOut, FileMode.Append)
                Else
                    fs = New FileStream(sFileOut, FileMode.Create)
                End If      'va in overwrite!!! 

                Dim sw As New StreamWriter(fs)

                'Sposto i file nella directory di uscita
                Dim _xmlDoc As New FileInfo(_inputFileNames(currentStep))
                If _xmlDoc.Exists Then
                    File.Copy(_xmlDoc.FullName, _dirOutput + "\" + _xmlDoc.Name, True)
                    _xmlDoc.Delete()
                End If
                If pdfFileInfo.Exists Then
                    File.Copy(pdfFileInfo.FullName, _dirOutput + "\" + pdfFileInfo.Name, True)
                    pdfFileInfo.Delete()
                End If

                sw.WriteLine(String.Concat(_xmlDoc.Name, vbTab, pdfFileInfo.Name, vbTab, prot.Year.ToString(), " ", prot.Number.ToString()))
                sw.Close()

                _imported += 1
                Return
            End If

        Catch ex As Exception

            'Copio i file nella directory d'errore
            If Not Directory.Exists(_dirErrors) Then
                Directory.CreateDirectory(_dirErrors)
            End If

            Dim sFileErr As String = String.Format("{0}\{1}.LOG", _dirErrors, ErrorFileName)
            Dim oFileInfoE As New FileInfo(sFileErr)
            Dim fs As FileStream
            If oFileInfoE.Exists() Then
                fs = New FileStream(sFileErr, FileMode.Append)
            Else
                fs = New FileStream(sFileErr, FileMode.Create)    'va in overwrite!!! 

            End If
            Dim sw As New StreamWriter(fs)
            sw.WriteLine(xmldocument.Name + vbTab + pdfFileInfo.Name + vbTab + ex.Message)

            _errors += 1
            'Creo la riga d'errore nella datatable
            DataRow = _errorsTable.NewRow

            DataRow("FILEXML") = FileXmlInfo.Name ' _xmldocument.Name
            DataRow("FILEDOC") = pdfFileInfo.Name
            If ex.InnerException IsNot Nothing Then
                DataRow("ERROR") = String.Concat(ex.Message, Environment.NewLine, ex.InnerException.Message)
            Else
                DataRow("ERROR") = ex.Message
            End If
            _errorsTable.Rows.Add(DataRow)
            sw.Close()
            Return
        End Try


        'Da qui in poi è la versione vecchia
        Dim _filexmlinfo As New FileInfo(_inputFileNames(currentStep))

        Dim xmlDoc As New XmlDocument
        Dim fileStream As FileStream
        Dim streamWriter As StreamWriter = Nothing
        Dim contact As Contact = Nothing
        Dim contactManual As Contact = Nothing
        Dim matricola As String
        Dim iType As Short = 0
        Dim fiscalCode As String
        Dim description As String = String.Empty
        Dim nome As String = String.Empty
        Dim cognome As String = String.Empty
        Dim aHlp() As String

        Dim protFacade As New ProtocolFacade()
        Dim protLogFacade As New ProtocolLogFacade()

        Try
            'ripulisci lista metadati
            _metadata.Rows.Clear()

            'Aggiungo le informazioni relative al nome file 
            xmlDoc.Load(_inputFileNames(currentStep))
            InsertMetadataRow("Filename", xmlDoc.SelectSingleNode("//Document").Attributes("DocumentFileName").Value)

            FileDocInfo = New FileInfo(_dirInput & "/" & xmlDoc.SelectSingleNode("//Document").Attributes("DocumentFileName").Value)


            Select Case DocSuiteContext.Current.CurrentDomainName
                Case "COOPELLEUNO"
                    If Not FileDocInfo.Exists Then
                        Return ' In ELLEUNO i documenti XML sono sempre in sovrannumero rispetto ai PDF da FastInput
                    End If
            End Select

            Select Case DocSuiteContext.Current.CurrentDomainName
                Case "COOPELLEUNO"
                    'Aggiungo i metadati dall'xml
                    ElleUnoBuildMetadataFromXml(xmlDoc)
                Case Else
                    'Aggiungo i metadati dall'xml
                    BuildMetadataFromXml(xmlDoc)
            End Select

            'Aggiungo i metadati aggiuntivi
            If Not OptionalMetadata Is Nothing Then
                Dim i As Integer
                For i = 0 To OptionalMetadata.Rows.Count - 1
                    _metadata.ImportRow(OptionalMetadata.Rows(i))
                Next i
            End If

            Dim type As Char?
            Dim myRows() As DataRow
            Select Case DocSuiteContext.Current.CurrentDomainName
                Case "COOPELLEUNO"
                    ' SEGNALAZIONE 1020: tutti i contatti sono da intendere di tipo Azienda e devono riportare PIva o CF
                    myRows = _metadata.Select("KEY ='Denominazione'")
                    If myRows.Length > 0 Then
                        description = CStr(myRows(0)("VALUE")).Trim()
                        If description.Length = 0 Then
                            Throw New Exception("Denominazione non valida nei Metadati su File")
                        End If
                        type = ContactType.Aoo
                        cognome = ""
                        nome = ""
                        iType = 2
                    Else
                        Throw New Exception("Denominazione non presente nei Metadati su File")
                    End If
                Case Else
                    'protocollo
                    '-1 Recupero in base - FiscalCode  in tabella contatti 
                    '-2 Recupero il destinatario dal record con KEY = 'DENOMINAZIONE' dei metadati
                    '-3 Recupero il destinatario dal record con KEY = 'Cognome' e KEY = 'Nome' dei metadati
                    '-2 e 3 FiscalCode nei metadati

                    myRows = _metadata.Select("KEY ='CodiceRicerca'")

                    If myRows.Length > 0 Then
                        matricola = CStr(myRows(0)("VALUE")).Trim()

                        Dim contFacade As New ContactFacade()
                        Dim contacts As IList(Of Contact) = contFacade.GetContactBySearchCode(matricola, -1)

                        If contacts.Count = 0 Then
                            Throw New Exception(String.Format("I Metadati Associati al Codice di Ricerca {0} non sono presenti in Contatti", matricola))
                        End If

                        If contacts.Count > 1 Then
                            Throw New Exception(String.Format("I Metadati Associati al Codice di Ricerca {0} non sono Univoci in DocSuite", matricola))
                        End If

                        contact = contacts(0)

                        fiscalCode = contact.FiscalCode

                        type = contact.ContactType.Id
                        Select Case type
                            Case ContactType.Aoo
                                description = contact.Description
                                nome = ""
                                cognome = ""

                                InsertMetadataRow("Denominazione", description)
                                InsertMetadataRow("PartitaIVA", fiscalCode)

                            Case ContactType.Person
                                aHlp = contact.Description.Split("|"c)
                                cognome = aHlp(0)
                                InsertMetadataRow("Cognome", cognome)

                                If aHlp.Length > 1 Then
                                    nome = aHlp(1)
                                    InsertMetadataRow("Nome", nome)
                                Else
                                    nome = ""
                                End If

                                InsertMetadataRow("CodiceFiscale", fiscalCode)

                            Case Else
                                Throw New Exception("La Tipologia Contatto " & type.Value & " Associata al Codice di Ricerca " & matricola & " non è Valida")
                        End Select

                        iType = 1

                        myRows(0).Delete()
                    End If

                    If iType = 0 Then
                        'Ho trovato la denominazione quindi ho un'azienda!!!!
                        myRows = _metadata.Select("KEY ='Denominazione'")
                        If myRows.Length > 0 Then
                            description = CStr(myRows(0)("VALUE")).Trim()
                            If description.Length = 0 Then
                                Throw New Exception("Denominazione non valida nei Metadati su File")
                            End If
                            type = ContactType.Aoo
                            cognome = ""
                            nome = ""
                            iType = 2
                        End If
                    End If

                    If iType = 0 Then
                        'Non ho trovato la denominazione il contatto deve essere una persona fisica!!

                        myRows = _metadata.Select("KEY ='Cognome'")
                        If myRows.Length > 0 Then cognome = CStr(myRows(0)("VALUE")).Trim()

                        myRows = _metadata.Select("KEY ='Nome'")
                        If myRows.Length > 0 Then nome = CStr(myRows(0)("VALUE")).Trim()

                        If nome.Length = 0 And cognome.Length = 0 Then
                            Throw New Exception("Cognome\Nome non validi nei metadati su File")
                        End If

                        type = ContactType.Person
                        description = cognome & "|" & nome
                        iType = 3
                    End If

            End Select


            Select Case iType
                Case 0
                    Throw New Exception("Impossibile Recuperare i Metadati sia da File che da Contatti")
                Case 1
                    contactManual = Nothing
                Case 2, 3
                    myRows = _metadata.Select("KEY ='PartitaIVA'")
                    If myRows.Length = 0 Then
                        myRows = _metadata.Select("KEY ='CodiceFiscale'")
                    End If

                    If myRows.Length > 0 Then
                        fiscalCode = CStr(myRows(0)("VALUE")).Trim()
                    Else
                        fiscalCode = ""
                    End If

                    contactManual = New Contact()
                    With contactManual
                        .ContactType = New ContactType(type.Value)
                        .Description = description
                        .FiscalCode = fiscalCode

                    End With
                    contact = Nothing
            End Select

            'DATAFATTURA
            Dim sDtFattura As String = ""
            myRows = _metadata.Select("KEY ='DataFattura'")
            If myRows.Length > 0 Then
                sDtFattura = CStr(myRows(0)("VALUE"))
            End If

            'NUMEROFATTURA
            Dim nFattura As Integer
            myRows = _metadata.Select("KEY ='NumeroFattura'")
            If myRows.Length > 0 Then
                nFattura = CInt(myRows(0)("VALUE"))
            End If

            'REGISTROIVA
            Dim sRegistroIVA As String = ""
            myRows = _metadata.Select("KEY ='RegistroIVA'")
            If myRows.Length > 0 Then
                sRegistroIVA = CStr(myRows(0)("VALUE"))
            End If

            'ANNOIVA
            Dim nAnnoIVA As Integer
            myRows = _metadata.Select("KEY ='AnnoIVA'")
            If myRows.Length > 0 Then
                nAnnoIVA = CInt(myRows(0)("VALUE"))
            End If

            'PROTOCOLLOIVA
            Dim nProtocolloIVA As Integer
            myRows = _metadata.Select("KEY ='ProtocolloIVA'")
            If myRows.Length > 0 Then
                nProtocolloIVA = CInt(myRows(0)("VALUE"))
            End If

            'LP20070319 Modifiche STANDARD PRO ITW
            'Campo note -> Se esistente lo appendo alle note del protocollo
            'Campo TipoDocumento->Se esiste verifico che sia una tipologia valida di documento
            'Campo ImportoFattura->Va inserito in campo ad Hoc se esistente
            Dim sNote As String = ""
            myRows = _metadata.Select("KEY ='Note'")
            If myRows.Length > 0 Then
                sNote = "|" + CStr(myRows(0)("VALUE"))
            End If

            Dim ptF As New ProtocolTypeFacade()
            myRows = _metadata.Select("KEY ='TipoDocumento'")
            ' **REMOVE**: Modifica per ELLEUNO. In Tipo documento da XML arriva una stringa che mettiamo nell'oggetto e che passiamo nei metadati
            '           Il codice si aspetta un identificativo di ProtocolType che ad ogni modo usa solo per scrivere la relativa descrizione nell'oggetto.

            Dim docType As String = String.Empty

            If myRows.Length > 0 Then
                docType = CType(myRows(0)("VALUE"), String)

                Select Case DocSuiteContext.Current.CurrentDomainName
                    Case "COOPELLEUNO"
                        ' NOOP
                    Case Else
                        If (ptF.GetById(docType) Is Nothing) Then
                            Throw New Exception(String.Format("Tipologia Documento {0} non presente in DocSuite", docType))
                        End If
                        _idDocType = ptF.GetById(docType).Id
                        docType = ptF.GetById(_idDocType).ShortDescription
                End Select

            End If

            myRows = _metadata.Select("KEY ='ImportoFattura'")
            If myRows.Length > 0 Then
                ' Dim sDesc As String = myRows(0)("VALUE")
            End If



            'LP 20070525 x Gestione oBJDescription
            'Documento <Tipo Documento> N.<NumeroFattura> del <DataFattura> <RagioneSociale>
            Dim tipoDocumento As String = ""

            If docType.Length > 0 Then
                tipoDocumento = "Tipo Documento : " & docType
            End If
            'If Not (ptF.GetById(_idDocType) Is Nothing) Then
            '    tipoDocumento = "Tipo Documento : " & ptF.GetById(_idDocType).ShortDescription
            'End If


            tipoDocumento &= String.Format(" N. {0} del {1} ", nFattura, sDtFattura)
            If description.Length > 0 Then
                tipoDocumento &= description
            Else
                tipoDocumento &= cognome & If(nome.Length > 0, " " & nome, "")
            End If

            'VERIFICA DATI DOPPI
            Dim finder As NHibernateProtocolFinder = New NHibernateProtocolFinder("ProtDB")
            Select Case _protocol.Container.Id
                Case -32711 'ATTIVE
                    ' Cercare la Fattura per numero fattura e anno
                    finder.IdStatus = ProtocolStatusId.Attivo
                    finder.Year = _protocol.Year
                    finder.InvoiceDateFrom = _protocol.InvoiceDate
                    finder.InvoiceDateTo = _protocol.InvoiceDate
                    finder.InvoiceNumber = _protocol.InvoiceNumber
                    finder.LoadFetchModeFascicleEnabled = False
                    finder.LoadFetchModeProtocolLogs = False
                    Dim items As IList(Of Protocol) = finder.DoSearch()
                    If items.Count > 0 Then
                        Throw New Exception(String.Format("Fattura Attiva già inserita in DataBase: {0}/{1}", items(0).Year, items(0).Number))
                    End If
                Case -32710 ' PASSIVE
                    ' Posso cercare per DocumentCode/DocumentName che dovrebbe contenere il nome del file pdf di origine
                    finder.DocumentName = FileDocInfo.Name
                    finder.IdStatus = ProtocolStatusId.Attivo
                    Dim items As IList(Of Protocol) = finder.DoSearch()
                    If items.Count > 0 Then
                        Throw New Exception(String.Format("Fattura Passiva già inserita in DataBase: {0}/{1}", items(0).Year, items(0).Number))
                    End If
            End Select

            'creazione lista allegati protocollo
            Dim lista As New List(Of TempFileDocumentInfo)
            lista.Add(New TempFileDocumentInfo(_filexmlinfo) With {.Name = _filexmlinfo.Name})

            ' CREO IL PROTOCOLLO
            Dim protocol As Protocol

            protocol = protFacade.CreateProtocol(CurrentTenant.TenantAOO.UniqueId)
            If protocol Is Nothing Then
                Exit Sub
            End If

            protocol.IdStatus = ProtocolStatusId.Errato
            ' Valori da Template
            protocol.Container = _protocol.Container
            protocol.Location = _protocol.Location
            protocol.AttachLocation = _protocol.AttachLocation
            protocol.DocumentType = _protocol.DocumentType

            ' POPOLO LE PROPERTIES del protocollo
            With protocol
                ' Valori da Template
                .Container = _protocol.Container
                .Location = _protocol.Location
                .AttachLocation = _protocol.AttachLocation
                .DocumentType = _protocol.DocumentType
                .Category = _protocol.Category

                Select Case DocSuiteContext.Current.CurrentDomainName
                    Case "COOPELLEUNO"
                        Select Case _protocol.Container.Id
                            Case -32710
                                .Type = (New ProtocolTypeFacade).GetById(-1)
                            Case Else
                                .Type = _protocol.Type
                        End Select
                    Case Else
                        .Type = _protocol.Type
                End Select

                If DocSuiteContext.Current.ProtocolEnv.IsStatusEnabled AndAlso _protocol.Status IsNot Nothing Then
                    .Status = _protocol.Status
                Else
                    .IdStatus = ProtocolStatusId.Errato
                End If

                .Note = _protocol.Note + sNote
                .ProtocolObject = tipoDocumento
                .DocumentCode = FileDocInfo.Name
                .InvoiceNumber = CStr(nFattura)
                .InvoiceDate = sDtFattura
                .AccountingSectional = sRegistroIVA
                .AccountingYear = nAnnoIVA
                .AccountingNumber = nProtocolloIVA
            End With
            ' Salvo il protocollo in -5
            protFacade.Save(protocol)

            'Contacts
            If contact IsNot Nothing Then
                protocol.AddRecipient(contact, False)
            ElseIf contactManual IsNot Nothing Then
                protocol.AddRecipientManual(contactManual, False)
            End If

            ' AGGIORNO IL PROTOCOLLO
            protFacade.Update(protocol)

            DataRow = _metadata.NewRow
            DataRow("KEY") = "Signature"
            DataRow("VALUE") = protFacade.GenerateSignature(protocol, protocol.RegistrationDate.ToLocalTime().DateTime, New ProtocolSignatureInfo())
            _metadata.Rows.Add(DataRow)

            ''Inserimento in BIBLOS
            protFacade.AddDocument(protocol, New FileDocumentInfo(FileDocInfo), New ProtocolSignatureInfo(0), GetAttributes(_metadata))

            ''Inserimento Log
            protLogFacade.Insert(_protocol, ProtocolLogEvent.PI, "Protocollo importato")

            'Tutto OK 
            If Not Directory.Exists(_dirOutput) Then
                Directory.CreateDirectory(_dirOutput)
            End If

            Dim sFileOut As String = _dirOutput + "\IMPORT.LOG"
            Dim oFileInfoO As New FileInfo(sFileOut)

            If oFileInfoO.Exists() Then
                fileStream = New FileStream(sFileOut, FileMode.Append)
            Else
                fileStream = New FileStream(sFileOut, FileMode.Create)
            End If      'va in overwrite!!! 

            streamWriter = New StreamWriter(fileStream)

            'Sposto i file nella directory di uscita
            If _filexmlinfo.Exists Then
                File.Copy(_filexmlinfo.FullName, _dirOutput + "\" + _filexmlinfo.Name, True)
                _filexmlinfo.Delete()
            End If
            If FileDocInfo.Exists Then
                File.Copy(FileDocInfo.FullName, _dirOutput + "\" + FileDocInfo.Name, True)
                FileDocInfo.Delete()
            End If

            streamWriter.WriteLine(_filexmlinfo.Name + vbTab + FileDocInfo.Name + vbTab + CStr(protocol.Year) + " " + CStr(protocol.Number))
            _imported += 1

        Catch ex As Exception
            'Copio i file nella directory d'errore
            If Not Directory.Exists(_dirErrors) Then
                Directory.CreateDirectory(_dirErrors)
            End If

            Dim sFileErr As String = String.Format("{0}\{1}.LOG", _dirErrors, ErrorFileName)
            Dim oFileInfoE As New FileInfo(sFileErr)
            If oFileInfoE.Exists() Then
                fileStream = New FileStream(sFileErr, FileMode.Append)
            Else
                fileStream = New FileStream(sFileErr, FileMode.Create)    'va in overwrite!!! 
            End If
            streamWriter = New StreamWriter(fileStream)

            streamWriter.WriteLine("{0}{1}{2}{1}{3}", _filexmlinfo.Name, vbTab, FileDocInfo.Name, ex.Message)
            _errors += 1
            'Creo la riga d'errore nella datatable
            DataRow = _errorsTable.NewRow
            DataRow("FILEXML") = _filexmlinfo.Name
            DataRow("FILEDOC") = FileDocInfo.Name
            If ex.InnerException IsNot Nothing Then
                DataRow("ERROR") = String.Concat(ex.Message, "\n", ex.InnerException.Message)
            Else
                DataRow("ERROR") = ex.Message
            End If
            _errorsTable.Rows.Add(DataRow)
        Finally
            If Not streamWriter Is Nothing Then
                streamWriter.Close()
            End If
        End Try
    End Sub

    ''' <summary>
    ''' Popola la tabella dei metadati partendo da un documento XML
    ''' </summary>
    ''' <param name="XmlDoc">Dcoumento XML di origine</param>
    Protected Sub BuildMetadataFromXml(ByVal xmlDoc As XmlDocument, Optional ByVal sType As String = "Fattura")

        Dim xmlDoc2 As XmlDocument
        Dim xmlNode As XmlNode = xmlDoc.SelectSingleNode("/Document/Page[@Type=""" & sType & """]")
        If xmlNode Is Nothing Then xmlNode = xmlDoc.SelectSingleNode("/Document/Page[@Type=""" & sType.ToUpper() & """]")

        If xmlNode Is Nothing Then Return

        'Ritrovo tutti gli attributi presenti nell'XML come figli di fattura
        Dim childNode As XmlNode
        Dim childAttribute As XmlAttribute

        For Each childNode In xmlNode.ChildNodes
            If childNode.Name = "Zone" Then
                childAttribute = childNode.Attributes("Name")
                If Not childAttribute Is Nothing Then
                    xmlDoc2 = New XmlDocument()
                    xmlDoc2.LoadXml(childNode.OuterXml)
                    xmlNode = xmlDoc2.SelectSingleNode("//Row[@Y=""0""]")
                    If Not xmlNode Is Nothing AndAlso Not String.IsNullOrEmpty(xmlNode.InnerText) Then
                        InsertMetadataRow(childAttribute.Value, xmlNode.InnerText)
                    End If
                End If
            End If
        Next
    End Sub

    ''' <summary>
    ''' Popola la tabella dei metadati partendo da un documento XML
    ''' </summary>
    ''' <param name="XmlDoc">Dcoumento XML di origine</param>
    Protected Sub ElleUnoBuildMetadataFromXml(ByVal xmlDoc As XmlDocument)

        Dim xmlDoc2 As XmlDocument
        Dim xmlNode As XmlNode = xmlDoc.SelectSingleNode("/Document/Page[contains(@DocumentFileName,'.pdf')]")
        If xmlNode Is Nothing Then xmlNode = xmlDoc.SelectSingleNode("/Document/Page[contains(@DocumentFileName,'.PDF')]")
        If xmlNode Is Nothing Then xmlNode = xmlDoc.SelectSingleNode("/Document/Page[contains(@DocumentFileName,'.Pdf')]")

        If xmlNode Is Nothing Then Return

        'Ritrovo tutti gli attributi presenti nell'XML come figli di fattura
        Dim childNode As XmlNode
        Dim childAttribute As XmlAttribute

        For Each childNode In xmlNode.ChildNodes
            If childNode.Name = "Zone" Then
                childAttribute = childNode.Attributes("Name")
                If Not childAttribute Is Nothing Then
                    xmlDoc2 = New XmlDocument()
                    xmlDoc2.LoadXml(childNode.OuterXml)
                    xmlNode = xmlDoc2.SelectSingleNode("//Row[@Y=""0""]")
                    If Not xmlNode Is Nothing AndAlso Not String.IsNullOrEmpty(xmlNode.InnerText) Then
                        InsertMetadataRow(childAttribute.Value, xmlNode.InnerText)
                    End If
                End If
            End If
        Next

    End Sub

    ''' <summary> Funzione di utilità che popola una riga dei metadati </summary>
    Protected Sub InsertMetadataRow(ByVal chiave As String, ByVal valore As String)

        DataRow = _metadata.NewRow
        DataRow("KEY") = chiave
        DataRow("VALUE") = valore
        _metadata.Rows.Add(DataRow)

    End Sub

    Protected Function GetAttributes(ByVal dtMetadati As DataTable) As Dictionary(Of String, String)
        Return dtMetadati.Rows.Cast(Of DataRow)().ToDictionary(Of String, String)(Function(row) row("KEY"), Function(row) row("VALUE"))
    End Function

    ''' <summary>
    ''' Crea il documento XML da passare a Biblos a partire dalla tabella dei metadati
    ''' </summary>
    ''' <param name="dtMetadati">Tabella di origine</param>
    ''' <returns>Rapprensentazione in fomrato stringa del documento XML</returns>
    Protected Function BuildBiblosXml(ByVal dtMetadati As DataTable) As Dictionary(Of String, String)

        Dim tor As Dictionary(Of String, String) = dtMetadati.Rows.Cast(Of DataRow)().ToDictionary(Of String, String)(Function(row) row("KEY"), Function(row) row("VALUE"))
        Return tor

    End Function

    ''' <summary> Metodo che prepara l'importazione settando le proprietà e lanciando il thread di importazione e portando i metadati </summary>
    ''' <param name="userName">Utente</param>
    ''' <param name="MetadatiOpzionali">Contenitore con le proprietà opzionali dei protocolli (metadati)</param>
    Public Overloads Function InserimentoMassiva(ByVal userName As String, ByVal MetadatiOpzionali As DataTable) As Boolean
        OptionalMetadata = MetadatiOpzionali
        InserimentoMassiva(userName)
    End Function

    Public Overloads Function InserimentoMassiva(ByVal userName As String) As Boolean

        _errorsTable = New DataTable("Errori")
        _errorsTable.Columns.Add("FILEXML", Type.GetType("System.String"))
        _errorsTable.Columns.Add("FILEDOC", Type.GetType("System.String"))
        _errorsTable.Columns.Add("ERROR", Type.GetType("System.String"))

        'Init
        Try
            _sImportSubFolder = Now.ToString("yyyyMMdd_HHmmss")
            _outputDirErrorsInfo = New DirectoryInfo(_dirErrors)
            _outputDirOutInfo = New DirectoryInfo(_dirOutput + "\" + _sImportSubFolder)

        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

        'Ciclo su tutti i files nella directory input
        _inputFileNames = Directory.GetFiles(_dirInput, "*.xml")

        If _inputFileNames.Length > 0 Then
            _imported = 0
            _errors = 0
            Dim task As MultiStepLongRunningTask = GlobalAsax.LongRunningTask
            task.TaskUser = userName
            task.StepsCount = _inputFileNames.Length
            task.TaskToExecute = New MultiStepLongRunningTask.TaskToExec(AddressOf ImportMassiva)
            task.RunTask()

            Return True
        Else
            Return False
        End If
    End Function

    Private Sub ImportMassiva(ByVal currentStep As Integer)
        Dim oFileI As FileInfo
        Dim xmlDoc As New XmlDocument
        Dim sFileName As String = String.Empty
        Dim myRow As DataRow
        Dim oFileO As FileInfo
        Dim oFsO As FileStream
        Dim oFsE As FileStream
        Dim oSwO As StreamWriter = Nothing
        Dim oSwE As StreamWriter = Nothing
        Dim originalFileName As String
        Dim drHlp As DataRow
        Dim sBarcode As String

        oFileI = New FileInfo(_inputFileNames(currentStep))

        Dim impersonator As Impersonator = CommonAD.ImpersonateSuperUser()

        Dim protFacade As New ProtocolFacade()
        Dim protLogFacade As New ProtocolLogFacade()
        Try
            _metadata.Rows.Clear()

            'Aggiungo le informazioni relative al nome file 
            xmlDoc.Load(_inputFileNames(currentStep))
            sFileName = xmlDoc.SelectSingleNode("//Document").Attributes("DocumentFileName").Value
            originalFileName = sFileName
            InsertMetadataRow("Filename", sFileName)

            FileDocInfo = New FileInfo(xmlDoc.SelectSingleNode("//Document").Attributes("DocumentFileName").Value)

            'Aggiungo i metadati dall'xml
            BuildMetadataFromXml(xmlDoc, "FASTINPUT")

            'Aggiungo i metadati aggiuntivi
            If Not OptionalMetadata Is Nothing Then
                Dim i As Integer
                For i = 0 To OptionalMetadata.Rows.Count - 1
                    _metadata.ImportRow(OptionalMetadata.Rows(i))
                Next i
            End If


            '1) formato valido <yyyy>0|1<0000000n>
            '2) Verifico protocollo esistente -> se non esiste = errore
            '3) Se protocollo esistente -> se flag 0 -> Documento principale NON deve esistere già documento associato
            '4)                         -> se flag = 1 -> allegato vado in append


            'protocollo
            '-1 Recupero in base - FiscalCode  in tabella contatti 
            '-2 Recupero il destinatario dal record con KEY = 'DENOMINAZIONE' dei metadati
            '-3 Recupero il destinatario dal record con KEY = 'Cognome' e KEY = 'Nome' dei metadati
            '-2 e 3 FiscalCode nei metadati
            Dim nAnno As Integer
            Dim nNumero As Integer
            Dim bAllegato As Boolean
            Dim myRows() As DataRow = _metadata.Select("KEY ='BARCODE'")
            If myRows.Length = 0 Then
                Throw New Exception("Nodo BARCODE non presente nell XML")
            End If
            sBarcode = CStr(myRows(0)("VALUE")).Trim()

            If sBarcode.Length <> 12 Then
                Throw New Exception(String.Format("Formato Codice Barcorde {0} non valido", sBarcode))
            End If

            Dim sHlp As String

            sHlp = sBarcode.Substring(0, 4)
            If Not IsNumeric(sHlp) Then
                Throw New Exception(String.Format("Anno {0} del Barcode {1} non valido ", sBarcode, sHlp))
            End If
            nAnno = Integer.Parse(sHlp)

            sHlp = sBarcode.Substring(4, 1)
            If sHlp <> "0" And sHlp <> "1" Then
                Throw New Exception(String.Format("Tipologia Documento\Allegato {0} del Barcode {1} non valido ", sBarcode, sHlp))
            End If
            bAllegato = (sHlp = "1")

            sHlp = sBarcode.Substring(5)
            If Not IsNumeric(sHlp) Then
                Throw New Exception(String.Format("Numero Protocollo {0} del Barcode {1} non valido ", sBarcode, sHlp))
            End If
            nNumero = Integer.Parse(sHlp)

            Dim listID As New List(Of YearNumberCompositeKey)
            listID.Add(New YearNumberCompositeKey(nAnno, nNumero))
            Dim protocols As IList(Of Protocol) = protFacade.GetProtocols(listID)
            If protocols.Count = 0 Then
                Throw New Exception(String.Format("Il protocollo {0}/{1:00000000} associato al barcode {2} non esiste.", nAnno.ToString(), nNumero.ToString(), sBarcode))
            End If

            If protocols.Count > 1 Then
                Throw New Exception(String.Format("Il protocollo {0}/{1:00000000} associato al barcode {2} non è univoco.", nAnno.ToString(), nNumero.ToString(), sBarcode))
            End If

            Dim idBiblos As Integer = 0
            Dim idAttachments As Integer = 0
            Dim type As ProtocolType = Nothing
            Dim container As Container = Nothing
            Dim location As Location = Nothing

            If protocols(0).IdDocument.HasValue Then idBiblos = protocols(0).IdDocument.Value
            If protocols(0).IdAttachments.HasValue Then idAttachments = protocols(0).IdAttachments.Value
            If protocols(0).Type IsNot Nothing Then type = protocols(0).Type
            If protocols(0).Container IsNot Nothing Then container = protocols(0).Container
            If protocols(0).Location IsNot Nothing Then location = protocols(0).Location

            If Not bAllegato And idBiblos > 0 Then
                Throw New Exception(String.Format("Il protocollo {0}/{1:00000000} associato al barcode {2} ha già un documento associato.", nAnno.ToString(), nNumero.ToString(), sBarcode))
            End If

            ' Biblos Documento
            myRow = _metadata.NewRow
            myRow("KEY") = "Signature"
            myRow("VALUE") = protFacade.GenerateSignature(protocols(0), DateTime.Now, New ProtocolSignatureInfo())
            _metadata.Rows.Add(myRow)

            sFileName = _dirInput + "\" + sFileName

            Dim fi As New FileInfo(sFileName)

            'Creo l'XML di passaggio parametri a biblos
            For i As Integer = _metadata.Rows.Count - 1 To 0 Step -1
                If _metadata.Rows(i)("KEY") <> "Filename" And _metadata.Rows(i)("KEY") <> "Signature" Then
                    _metadata.Rows.RemoveAt(i)
                End If
            Next
            'Inserimento in biblos
            Dim loc As New UIDLocation() With {.Archive = location.ProtBiblosDSDB}

            If Not bAllegato Then
                idBiblos = Service.AddFile(loc, fi, BuildBiblosXml(_metadata)).Chain.Id
            Else
                idBiblos = Service.AddFile(loc, idAttachments, fi, BuildBiblosXml(_metadata)).Chain.Id
            End If

            If idBiblos <= 0 Then
                Throw New Exception("Errore non definito nel Web Service BIBLOS")
            End If

            If Not bAllegato Then
                protFacade.UpdateDocument(protocols(0), idBiblos, 0, originalFileName)
            Else
                protFacade.UpdateAttachments(protocols(0), If(idAttachments = 0, idBiblos, idAttachments))
            End If

            protLogFacade.Insert(protocols(0), ProtocolLogEvent.PM, "")
            'Tutto OK 
            If Not _outputDirOutInfo.Exists Then
                _outputDirOutInfo.Create()
            End If

            Dim sFileOut As String = _outputDirOutInfo.FullName + "\IMPORTMASSIVA.LOG"
            Dim oFileInfoO As New FileInfo(sFileOut)
            If oFileInfoO.Exists() Then
                oFsO = New FileStream(sFileOut, FileMode.Append)
            Else
                oFsO = New FileStream(sFileOut, FileMode.Create)
            End If      'va in overwrite!!! 
            oSwO = New StreamWriter(oFsO)


            'Copio i file nella directory di uscita
            oFileO = New FileInfo(oFileI.FullName)
            If oFileO.Exists Then
                oFileO.CopyTo(_outputDirOutInfo.FullName + "\" + oFileO.Name, True)
                oFileO.Delete()
            End If

            If Len(sFileName) > 0 Then
                oFileO = New FileInfo(sFileName)
                If oFileO.Exists Then
                    oFileO.CopyTo(_outputDirOutInfo.FullName + "\" + oFileO.Name, True)
                    oFileO.Delete()
                End If
            End If

            oSwO.WriteLine(oFileI.FullName + vbTab + sFileName + vbTab + CStr(nAnno) + " " + CStr(nNumero))
            sFileName = ""
            _imported += 1

        Catch ex As Exception
            'Copio i file nella directory d'errore
            If Not _outputDirErrorsInfo.Exists Then
                _outputDirErrorsInfo.Create()
            End If

            Dim sFileErr As String = _outputDirErrorsInfo.FullName + "\" + _sImportSubFolder + ".LOG"
            Dim oFileInfoE As New FileInfo(sFileErr)
            If oFileInfoE.Exists() Then
                oFsE = New FileStream(sFileErr, FileMode.Append)
            Else
                oFsE = New FileStream(sFileErr, FileMode.Create)    'va in overwrite!!! 
            End If
            oSwE = New StreamWriter(oFsE)

            oSwE.WriteLine(oFileI.FullName + vbTab + sFileName + vbTab + ex.Message + ex.StackTrace)
            _errors += 1
            'Creo la riga d'errore nella datatable
            drHlp = _errorsTable.NewRow
            drHlp("FILEXML") = oFileI.FullName
            drHlp("FILEDOC") = sFileName
            drHlp("ERROR") = ex.Message
            _errorsTable.Rows.Add(drHlp)
        Finally
            If oSwO IsNot Nothing Then
                oSwO.Close()
            End If
            If oSwE IsNot Nothing Then
                oSwE.Close()
            End If
            impersonator.ImpersonationUndo()
        End Try
    End Sub

#End Region

End Class