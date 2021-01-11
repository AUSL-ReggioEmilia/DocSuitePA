Imports System.Data.OleDb
Imports System.IO
Imports System.Threading
Imports System.Globalization
Imports System.Collections.Generic
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Xml
Imports VecompSoftware.Services.Biblos.Models

''' <summary>
''' Classe per l'importazione delle lettere nei protocolli
''' DP 12-05 seconda release e aggiunta commenti
''' </summary>
Public Class CommImportLettera
    Inherits BaseCommImport

    Protected _errorFileName As String
    Protected _excelConnection As OleDbConnection
    Protected _sheetName As String
    Protected _sOggetto As String = String.Empty
    Protected _sStatus As String = String.Empty
    Protected _fileDocInfo As FileInfo
    Protected _tempFileDoc As String = String.Empty
    Protected _fileXmlInfo As FileInfo
    Protected _tempFileXml As String = String.Empty
    Protected _dataRow As DataRow
    Protected _statusEnable As Boolean = False
    Protected _userName As String = String.Empty
    Protected _appTempPath As String = String.Empty

    Protected _xlsContactData() As String

    '**REMOVE**
    Protected _idPrenotazione As String
    Protected _contactDescr As String

    ''' <summary>
    ''' Costruttore di default
    ''' </summary>
    Public Sub New(ByVal comm As CommonUtil, Optional ByVal type As String = "Lettera")

        MyBase.New(, type)
        _statusEnable = DocSuiteContext.Current.ProtocolEnv.IsStatusEnabled
        _userName = DocSuiteContext.Current.User.UserName
        _appTempPath = comm.AppTempPath
    End Sub

    ''' <summary>
    ''' Controlla i file nella dir di input
    ''' per trovare i legami fra xml e doc
    ''' </summary>
    ''' <returns>la tabella con i file xml e doc e il relativo status</returns>
    Public Overrides Function CheckFiles() As DataTable

        Dim xmlDoc As New XmlDocument
        Dim resultTable As New DataTable
        Dim sFile As String = String.Empty
        Dim myRows() As DataRow = Nothing

        Dim dirInfo As New DirectoryInfo(_dirInput)
        Dim fileInfo As FileInfo

        resultTable.Columns.Add("FILEXML", Type.GetType("System.String"))
        resultTable.Columns.Add("FILEDOC", Type.GetType("System.String"))
        resultTable.Columns.Add("STATUS", Type.GetType("System.Boolean"))

        'Ciclo su tutta la directory
        For Each fileInfo In dirInfo.GetFiles()
            _dataRow = resultTable.NewRow
            If fileInfo.Extension.ToUpper = ".XML" Then
                Try
                    xmlDoc.Load(fileInfo.FullName)
                    Dim xmlNode As XmlNode = xmlDoc.SelectSingleNode("//Documento")
                    Dim sFileName As String = xmlNode.InnerText
                    _dataRow("FILEXML") = fileInfo.Name.ToUpper()
                    _dataRow("FILEDOC") = sFileName
                    _dataRow("STATUS") = 1
                Catch ex As Exception
                    _dataRow("FILEXML") = fileInfo.Name.ToUpper()
                    _dataRow("FILEDOC") = ""
                    _dataRow("STATUS") = 1
                End Try
            Else
                _dataRow("FILEXML") = ""
                _dataRow("FILEDOC") = fileInfo.Name.ToUpper()
                _dataRow("STATUS") = 1
            End If

            resultTable.Rows.Add(_dataRow)
        Next


        For Each _dataRow In resultTable.Rows
            If CStr(_dataRow("FILEXML")).Length > 0 Then
                'file xml verifico che il relativo file sia presente
                If CStr(_dataRow("FILEDOC")).Length > 0 Then
                    sFile = dirInfo.FullName + "\" + CStr(_dataRow("FILEDOC"))
                    fileInfo = New FileInfo(sFile)
                    If Not fileInfo.Exists Then _dataRow("STATUS") = 0
                Else
                    _dataRow("STATUS") = 0
                End If
            Else
                'File documento devo verificare che sia referenziato
                sFile = CStr(_dataRow("FILEDOC"))
                myRows = resultTable.Select("FILEXML <> '' AND FILEDOC = '" + sFile + "' ")
                If myRows.Length = 0 Then _dataRow("STATUS") = 0
            End If
        Next

        'Elimino i file DOC OK in quanto già associati all'xml
        'Quindi nella datatable si avranno: -XML e DOC OK XML e DOC KO '' e DOC KO
        myRows = resultTable.Select("STATUS = 1 AND  FILEXML = '' ")

        For Each _dataRow In myRows
            resultTable.Rows.Remove(_dataRow)
        Next

        Return resultTable

    End Function

    ''' <summary>
    ''' Metodo che prepara l'importazione settando le proprietà
    ''' e lanciando il thread di importazione
    ''' </summary>
    ''' <param name="Protocollo">Contenitore con le proprietà comuni dei protocolli</param>
    ''' <returns>Booleano di errore</returns>
    Public Overrides Function InserimentoProtocollo(ByVal Protocollo As Protocol, ByVal all As Boolean) As Boolean

        _protocol = Protocollo

        'Tabella risultati
        _resultTable = New DataTable("Risultati")
        ResultTable.Columns.Add("FILEXML", Type.GetType("System.String"))
        ResultTable.Columns.Add("FILEDOC", Type.GetType("System.String"))
        ResultTable.Columns.Add("ERROR", Type.GetType("System.String"))
        ResultTable.Columns.Add("RESULT", Type.GetType("System.String"))

        'Tabella errori
        _errorsTable = New DataTable("Errori")
        ErrorsTable.Columns.Add("FILEXML", Type.GetType("System.String"))
        ErrorsTable.Columns.Add("FILEDOC", Type.GetType("System.String"))
        ErrorsTable.Columns.Add("ERROR", Type.GetType("System.String"))
        ErrorsTable.Columns.Add("RESULT", Type.GetType("System.String"))

        'Init
        Try
            _inputDirInfo = New DirectoryInfo(_dirInput)
            _outputDirOutInfo = New DirectoryInfo(_dirOutput)
            _outputDirErrorsInfo = New DirectoryInfo(_dirErrors)

            _errorFileName = _dirErrors + "\" + _userName + "-" + _
                             String.Format("{0:yyyy-MM-dd-HHmmss}", Date.Now) + "-Import.log"

            If File.Exists(_errorFileName) Then
                File.Delete(_errorFileName)
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

        'Ciclo su tutti i files nella directory input
        _inputFileNames = Directory.GetFiles(_dirInput, "*.xml")

        'Connessione ad Excel
        If _inputFileNames.Length > 0 Then

            Dim xlsFile As String = Directory.GetFiles(_dirInput, "*.xls")(0)
            Dim sConnString As String = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=[ExcelFilePath];Extended Properties='Excel 8.0;HDR=Yes;'"
            sConnString = sConnString.Replace("[ExcelFilePath]", xlsFile)
            _excelConnection = New OleDbConnection(sConnString)
            _excelConnection.Open()

            Dim schemaTable As DataTable = _excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, New Object() {Nothing, Nothing, Nothing, "TABLE"})
            If (schemaTable.Rows.Count > 0) Then
                _sheetName = "[" & schemaTable.Rows(0)(2) & "]"
            End If

            _imported = 0
            _errors = 0
            _total = 0

            Dim task As MultiStepLongRunningTask = GlobalAsax.LongRunningTask
            With task
                .TaskUser = _userName
                _total = _inputFileNames.Length
                .StepsCount = _inputFileNames.Length
                .SetCurrentFileName = New MultiStepLongRunningTask.SetCurrentFileNameDelegate(AddressOf SetCurrentFilename)
                .TaskToExecute = New MultiStepLongRunningTask.TaskToExec(AddressOf ImportProtocol)
                .RunTask()
            End With

            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Metodo che prepara l'importazione settando le proprietà
    ''' e lanciando il thread di importazione
    ''' </summary>
    ''' <param name="Protocollo">Contenitore con le proprietà comuni dei protocolli</param>
    ''' <returns>Booleano di errore</returns>
    Public Function InserimentoProtocolloExcel(ByVal Protocollo As Protocol) As Boolean

        _protocol = Protocollo

        'Tabella risultati
        _resultTable = New DataTable("Risultati")
        ResultTable.Columns.Add("FILEXML", Type.GetType("System.String"))
        ResultTable.Columns.Add("FILEDOC", Type.GetType("System.String"))
        ResultTable.Columns.Add("ERROR", Type.GetType("System.String"))
        ResultTable.Columns.Add("RESULT", Type.GetType("System.String"))

        'Tabella errori
        _errorsTable = New DataTable("Errori")
        ErrorsTable.Columns.Add("FILEXML", Type.GetType("System.String"))
        ErrorsTable.Columns.Add("FILEDOC", Type.GetType("System.String"))
        ErrorsTable.Columns.Add("ERROR", Type.GetType("System.String"))
        ErrorsTable.Columns.Add("RESULT", Type.GetType("System.String"))

        'Init
        Try
            _inputDirInfo = New DirectoryInfo(_dirInput)
            _outputDirOutInfo = New DirectoryInfo(_dirOutput)
            _outputDirErrorsInfo = New DirectoryInfo(_dirErrors)

            _errorFileName = _dirErrors + "\" + _userName + "-" + _
                             String.Format("{0:yyyy-MM-dd-HHmmss}", Date.Now) + "-Import.log"

            If File.Exists(_errorFileName) Then
                File.Delete(_errorFileName)
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

        Dim xlsFile As String = Directory.GetFiles(_dirInput, "*.xls")(0)
        Dim sConnString As String = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=[ExcelFilePath];Extended Properties='Excel 8.0;HDR=Yes;'"
        sConnString = sConnString.Replace("[ExcelFilePath]", xlsFile)
        _excelConnection = New OleDbConnection(sConnString)
        _excelConnection.Open()

        'Recupero lo schema del foglio excel
        Dim schemaTable As DataTable = _excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, New Object() {Nothing, Nothing, Nothing, "TABLE"})
        If (schemaTable.Rows.Count > 0) Then
            _sheetName = "[" & schemaTable.Rows(0)(2) & "]"
        End If

        'Gestione del file excel
        Dim command As IDbCommand = _excelConnection.CreateCommand()
        command.CommandText = "SELECT * FROM  " + _sheetName + " WHERE NUM_PROT_LETTERA_INSOLUTO is null AND IDPRENOTAZIONE is not null"
        command.CommandType = CommandType.Text

        Dim dr As IDataReader = command.ExecuteReader()
        If Not dr.Read() Then
            dr.Close()
            Return False
        End If

        Dim filenames As New List(Of String)
        Dim contactData As New List(Of String)
        Do
            Dim contact As String = dr.Item("COGNOME") & If(Not String.IsNullOrEmpty(dr.Item("NOME").ToString()), "§" & dr.Item("NOME").ToString(), "")
            filenames.Add(contact.Replace("§", " "))
            contact &= "|" & dr.Item("INDIRIZZO") '1
            contact &= "|" & dr.Item("LOCALITA") '2
            contact &= "|" & dr.Item("DATA_NASCITA") '3
            contact &= "|" & dr.Item("CODICE_FISCALE") '4
            contact &= "|" & dr.Item("IDPRENOTAZIONE") '5
            contact &= "|" & dr.Item("MASTER_ID") '6
            contact &= "|" & dr.Item("IMPORTO") '7
            contact &= "-" & dr.Item("DATA_PRESTAZIONE")
            contact &= "-" & dr.Item("ANNO")
            contact &= "-" & dr.Item("N_PRONTO_SOCCORSO")
            contact &= "-" & dr.Item("NUMERO_PROTOCOLLO")
            contact &= "-" & dr.Item("DATA_PROTOCOLLO")
            contactData.Add(contact)
            If Not dr.Read() Then
                Exit Do
            End If
        Loop
        _inputFileNames = filenames.ToArray()
        _xlsContactData = contactData.ToArray()
        'fine gestione del file excel

        _imported = 0
        _errors = 0
        _total = 0

        Dim task As MultiStepLongRunningTask = GlobalAsax.LongRunningTask
        With task
            .TaskUser = _userName
            _total = _inputFileNames.Length
            .StepsCount = _inputFileNames.Length
            .SetCurrentFileName = New MultiStepLongRunningTask.SetCurrentFileNameDelegate(AddressOf SetCurrentFilename)
            .TaskToExecute = New MultiStepLongRunningTask.TaskToExec(AddressOf ImportProtocolExcel)
            .RunTask()
        End With

        Return True
    End Function

    ''' <summary>
    ''' Subroutine di importazione dei dati
    ''' </summary>
    ''' <param name="currentStep">Numero del passaggio corrente</param>
    Private Sub ImportProtocol(ByVal currentStep As Int32)

        Dim xmlDoc As New XmlDocument
        Dim fileName As String = String.Empty
        Dim localProtocol As Protocol
        Dim matricola As String = String.Empty
        Dim progressivo As String = String.Empty
        Dim codice As String = String.Empty

        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("it-IT")

        _fileXmlInfo = New FileInfo(_inputFileNames(currentStep))
        Try
            ' verifico che non sia necessario riaprire la connessione a fronte di un errore
            If _excelConnection.State = ConnectionState.Closed Then
                _excelConnection.Open()
            End If
            'Carico il documento xml
            xmlDoc.Load(_fileXmlInfo.FullName)

            'Leggo il nome del documento
            _fileDocInfo = New FileInfo(_dirInput & GetNodeValue(xmlDoc, "//Documento"))
            '_fileDocInfo = New FileInfo(_dirInput & "\" & GetNodeValue(xmlDoc, "//Documento"))

            If Not _fileXmlInfo.Exists Then
                Throw New Exception("Documento '" & _fileDocInfo.Name & "' non trovato per file metadati '" & _fileXmlInfo.Name & "'")
            End If

            'prendo i dati dal file XML
            matricola = GetNodeValue(xmlDoc, "//Matricola")
            progressivo = GetNodeValue(xmlDoc, "//Progressivo")
            codice = GetNodeValue(xmlDoc, "//Codice")


            Dim command As IDbCommand = _excelConnection.CreateCommand()
            command.CommandText = "SELECT * FROM  " + _sheetName + " WHERE NumeroProtocollo is null AND Codice = '" & _
                                  codice & "' AND Progressivo=" + progressivo + " AND  Matricola ='" + matricola + "'"
            command.CommandType = CommandType.Text

            Dim dr As IDataReader = command.ExecuteReader()
            If Not dr.Read() Then
                dr.Close()
                Throw New Exception("Record non presente nel file Excel. Controllare che il file Excel sia corretto.")
            End If
            dr.Close()

            Dim protFacade As New ProtocolFacade()

            NHibernateSessionManager.Instance.InvalidateSessionFrom("ProtDB")

            localProtocol = protFacade.CreateProtocol(CurrentTenant.TenantAOO.UniqueId)

            With localProtocol
                .IdDocument = _protocol.IdDocument
                .IdStatus = _protocol.IdStatus
                .Type = _protocol.Type
                .Container = _protocol.Container
                .Location = _protocol.Location
                .Note = _protocol.Note
                .Subject = _protocol.Subject
                .Status = _protocol.Status
                .Category = _protocol.Category
                .ProtocolObject = _protocol.ProtocolObject
                .DocumentCode = _fileDocInfo.Name
                .DocumentType = _protocol.DocumentType
                If _statusEnable Then
                    If _protocol.Status IsNot Nothing Then
                        .Status = _protocol.Status
                    Else
                        .IdStatus = ProtocolStatusId.Attivo
                        .Status = (New ProtocolStatusFacade().GetById(ProtocolStatusId.Attivo))
                    End If
                End If
            End With

            Dim contFacade As New ContactFacade()
            Dim contacts As IList(Of Contact) = contFacade.GetContactBySearchCode(matricola, -1)

            If contacts.Count = 0 Then
                Throw New Exception("I dati relativi alla matricola " + matricola + " non sono presenti in Contatti")
            End If

            If contacts.Count > 1 Then
                Throw New Exception("I dati relativi alla matricola " + matricola + " non sono Univoci in DocSuite")
            End If

            If contacts(0) IsNot Nothing Then
                localProtocol.AddRecipient(contacts(0), False)
            End If

            'copio il file xml ed il documento ( se arrivato fin qui entrambi esistenti)nella cartella temporanea
            'copia doc
            '_tempFileDoc = _appTempPath & "\" & _userName & "-Insert-" & String.Format("{0:HHmmss}", Now()) & "-" & _fileDocInfo.Name
            _tempFileDoc = _appTempPath & _userName & "-Insert-" & String.Format("{0:HHmmss}", Now()) & "-" & _fileDocInfo.Name
            File.Copy(_fileDocInfo.FullName, _tempFileDoc)

            'copia xml
            _tempFileXml = _appTempPath & "\" & _userName & "-Insert-" & String.Format("{0:HHmmss}", Now()) & "-" & _
                           _fileXmlInfo.Name
            File.Copy(_fileXmlInfo.FullName, _tempFileXml)


            'Carico il file Doc
            Dim document As New FileDocumentInfo(New FileInfo(_tempFileDoc))
            document.Name = _fileDocInfo.Name

            'Carico il file Xml come allegato
            Dim attachs As New List(Of FileDocumentInfo)
            Dim attach As New FileDocumentInfo(New FileInfo(_tempFileXml))
            attach.Name = _fileXmlInfo.Name
            attachs.Add(attach)

            'Salvataggio del protocollo
            protFacade.Save(localProtocol)
            protFacade.AddDocument(localProtocol, document)
            protFacade.AddAttachments(localProtocol, attachs)
            protFacade.Activation(localProtocol)


            'Inserimento in tabella OK
            _dataRow = ResultTable.NewRow()
            _dataRow("FILEXML") = _fileXmlInfo.FullName
            _dataRow("FILEDOC") = _fileDocInfo.Name
            _dataRow("ERROR") = "Protocollato correttamente alle: " & String.Format("{0:hh:mm:ss}", Date.Now) & " del " & _
                              String.Format("{0:dd/MM/yyyy}", Date.Now)
            _dataRow("RESULT") = localProtocol.FullNumber
            ResultTable.Rows.Add(_dataRow)

            'Sposto doc e xml in una cartella di OUTPUT
            If Not Directory.Exists(_dirOutput) Then Directory.CreateDirectory(_dirOutput)

            _imported += 1
            Dim newFileName As String = String.Format("{0:yyyy-MM-dd-HHmmss}", Date.Now) + "-" + Convert.ToString(_imported) + "-"
            'Xml
            File.Move(_fileXmlInfo.FullName, _dirOutput + "\" + newFileName + _fileXmlInfo.Name)
            'doc
            File.Move(_fileDocInfo.FullName, _dirOutput + "\" + newFileName + _fileDocInfo.Name)


            command = _excelConnection.CreateCommand()

            command.CommandText = "UPDATE  " + _sheetName + " SET NumeroProtocollo = '" +
                            localProtocol.FullNumber + "', DataProtocollo = '" + localProtocol.RegistrationDate.ToLocalTime().DateTime.ToShortDateString() + "' WHERE Codice = '" & codice &
                            "' AND Progressivo=" + progressivo + " AND  Matricola='" + matricola + "'"

            command.ExecuteNonQuery()

            If (_inputFileNames.Length = currentStep + 1) Then
                _excelConnection.Close()
            End If

        Catch ex As Exception

            WriteLog(_sheetName + vbTab + _inputFileNames(currentStep) + vbTab + ex.Message)
            _errors += 1

            'Creo la riga d'errore nella datatable KO
            _dataRow = ErrorsTable.NewRow()
            _dataRow("FILEXML") = _fileXmlInfo.FullName
            _dataRow("FILEDOC") = _fileDocInfo.Name
            _dataRow("ERROR") = ex.Message
            _dataRow("RESULT") = "-1"
            ErrorsTable.Rows.Add(_dataRow)

            ' Chiudo la connessione per evitare che resti appesa
            If _excelConnection.State = ConnectionState.Open Then
                _excelConnection.Close()
            End If

        Finally
            'rimuove i documenti da importare dalla cartella temporanea
            If Not String.IsNullOrEmpty(_tempFileDoc) Then File.Delete(_tempFileDoc)
            If Not String.IsNullOrEmpty(_tempFileXml) Then File.Delete(_tempFileXml)
        End Try

    End Sub

    ''' <summary>
    ''' Subroutine di importazione dei dati da foglio Excel
    ''' </summary>
    ''' <param name="currentStep">Numero del passaggio corrente</param>
    Private Sub ImportProtocolExcel(ByVal currentStep As Int32)

        Dim xmlDoc As New XmlDocument
        Dim fileName As String = String.Empty
        Dim localProtocol As Protocol
        Dim matricola As String = String.Empty
        Dim progressivo As String = String.Empty
        Dim codice As String = String.Empty

        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("it-IT")

        Try

            Dim protFacade As New ProtocolFacade()

            localProtocol = protFacade.CreateProtocol(CurrentTenant.TenantAOO.UniqueId)
            Dim cont() As String = _xlsContactData(currentStep).Split("|"c)
            Dim stemp As String

            With localProtocol
                .IdDocument = _protocol.IdDocument
                .IdStatus = ProtocolStatusId.Attivo
                .Type = _protocol.Type
                .Container = _protocol.Container
                .Location = _protocol.Location
                stemp = If(_protocol.Note.Length > 0, _protocol.Note & " ", "") & cont(7).TrimEnd("-"c) 'stringa coi dettagli dell'importazione
                .Note = If(stemp.Length > 255, Left(stemp, 255), stemp)
                .Subject = _protocol.Subject
                .Status = _protocol.Status
                .Category = _protocol.Category
                stemp = String.Concat(_protocol.ProtocolObject, " IDPRENOTAZIONE:", cont(5))
                .ProtocolObject = If(stemp.Length > 255, Left(stemp, 255), stemp)
                .DocumentType = _protocol.DocumentType
            End With

            'Creazione del contatto manuale coi dati del xls
            Dim contact As New Contact()
            contact.ContactType = New ContactType(ContactType.Person)

            'Mi salvo i dati per il report
            _idPrenotazione = cont(5)
            _contactDescr = cont(0).Replace("§", " ")
            Dim contactName As String = cont(0).Replace("§", "|")

            contact.Description = If(contactName.Length > 60, Left(contactName, 60), contactName) 'Descriprion (Cognome Nome)
            contact.Address = New Address
            contact.Address.Address = If(cont(1).Length > 60, Left(cont(1), 60), cont(1)) 'Indirizzo (completo)
            contact.Address.City = If(cont(2).Length > 50, Left(cont(2), 50), cont(2)) 'Città (e provincia)
            If Not String.IsNullOrEmpty(cont(3)) Then
                contact.BirthDate = Convert.ToDateTime(cont(3))
            End If
            If cont(4).Length = 16 Then
                contact.FiscalCode = cont(4)
            Else
                WriteLog(_sheetName + vbTab + _inputFileNames(currentStep) + vbTab + "Codice Fiscale non corretto")
            End If
            contact.Note = cont(6)

            If contact IsNot Nothing Then
                localProtocol.AddRecipientManual(contact, False)
            End If

            'Salvataggio del protocollo
            protFacade.Save(localProtocol)

            'Log del salvataggio
            Dim protLog As New ProtocolLogFacade()
            protLog.Insert(localProtocol, ProtocolLogEvent.PI, "")

            'Inserimento in tabella OK
            _dataRow = ResultTable.NewRow()
            _dataRow("FILEXML") = _idPrenotazione
            _dataRow("FILEDOC") = _contactDescr
            _dataRow("ERROR") = "Protocollato correttamente alle: " & String.Format("{0:hh:mm:ss}", Date.Now) & " del " & _
                              String.Format("{0:dd/MM/yyyy}", Date.Now)
            _dataRow("RESULT") = localProtocol.FullNumber
            ResultTable.Rows.Add(_dataRow)
            _imported += 1

            Dim command As IDbCommand = _excelConnection.CreateCommand()
            command = _excelConnection.CreateCommand()
            command.CommandText = "UPDATE  " + _sheetName + " SET NUM_PROT_LETTERA_INSOLUTO = '" + _
                            localProtocol.FullNumber + "', DATA_PROT_LETTERA_INSOLUTO = '" + _
            localProtocol.RegistrationDate.DateTime.ToShortDateString() + "' WHERE IDPRENOTAZIONE = " & cont(5)
            command.ExecuteNonQuery()

        Catch ex As Exception

            WriteLog(_sheetName + vbTab + _inputFileNames(currentStep) + vbTab + ex.Message)
            _errors += 1

            'Creo la riga d'errore nella datatable KO
            _dataRow = ErrorsTable.NewRow()
            _dataRow("FILEXML") = _idPrenotazione
            _dataRow("FILEDOC") = _contactDescr
            _dataRow("ERROR") = ex.Message
            _dataRow("RESULT") = "-1"
            ErrorsTable.Rows.Add(_dataRow)
        Finally
            If (_inputFileNames.Length = currentStep + 1) Then
                _excelConnection.Close()
            End If
        End Try

    End Sub


    ''' <summary>
    ''' Routine che recupera il nome del file
    ''' dato il suo numero nella coda
    ''' </summary>
    ''' <param name="currentStep">Numero nella coda</param>
    ''' <returns>Nome del file</returns>
    Private Function SetCurrentFilename(ByVal currentStep As Integer) As String

        If (currentStep < 0 Or currentStep > _inputFileNames.Length - 1) Then
            Return String.Empty
        End If

        Return _inputFileNames(currentStep)

    End Function

    ''' <summary>
    ''' Utility per il recupero dei valori nei nodi di un file XML
    ''' </summary>
    ''' <param name="XmlDoc">Documento XML</param>
    ''' <param name="NodeName">Nome del nodo</param>
    ''' <returns>Valore del nodo</returns>
    ''' <remarks>Lancia un'eccezione se il nodo non viene trovato</remarks>
    Protected Function GetNodeValue(ByVal XmlDoc As XmlDocument, ByVal NodeName As String) As String

        Dim nodo As XmlNode = XmlDoc.SelectSingleNode(NodeName)
        If IsNothing(nodo) Then
            Throw New Exception("Campo " + NodeName + " non trovato all'interno del file XML. Verificare Maiuscole e minuscole.")
        Else
            Return nodo.InnerText
        End If

    End Function

    Protected Sub WriteLog(ByVal message As String)
        Dim stream As FileStream
        Dim writer As StreamWriter

        'Copio i file nella directory d'errore
        If Not Directory.Exists(_dirErrors) Then Directory.CreateDirectory(_dirErrors)

        If File.Exists(_errorFileName) Then
            stream = New FileStream(_errorFileName, FileMode.Append)
        Else
            stream = New FileStream(_errorFileName, FileMode.Create) 'va in overwrite!!! 
        End If

        writer = New StreamWriter(stream)

        writer.WriteLine(message)
        writer.Close()
    End Sub
End Class

