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


    ''' <summary> Funzione di utilità che popola una riga dei metadati </summary>
    Protected Sub InsertMetadataRow(ByVal chiave As String, ByVal valore As String)

        DataRow = _metadata.NewRow
        DataRow("KEY") = chiave
        DataRow("VALUE") = valore
        _metadata.Rows.Add(DataRow)

    End Sub

    ''' <summary>
    ''' Crea il documento XML da passare a Biblos a partire dalla tabella dei metadati
    ''' </summary>
    ''' <param name="dtMetadati">Tabella di origine</param>
    ''' <returns>Rapprensentazione in fomrato stringa del documento XML</returns>
    Protected Function BuildBiblosXml(ByVal dtMetadati As DataTable) As Dictionary(Of String, String)

        Dim tor As Dictionary(Of String, String) = dtMetadati.Rows.Cast(Of DataRow)().ToDictionary(Of String, String)(Function(row) row("KEY"), Function(row) row("VALUE"))
        Return tor

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