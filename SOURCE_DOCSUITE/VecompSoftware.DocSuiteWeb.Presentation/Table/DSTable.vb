Imports System.Collections.Generic
Imports System.Web.UI.WebControls
Imports System.IO
Imports System.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade

' TODO: NON USARLA PER NUOVE IMPLEMENTAZIONI
''' <summary> Classe per la costruzione di una tabella Html. </summary>
''' <remarks> Classe oscena da dimettere il più velocemente possibile. </remarks>
<Serializable()>
Public Class DSTable
    Inherits DSHtmlControl
    Implements IDisposable

#Region "Fields"
    Private _table As Table
    Private _tableRowCollection As IList(Of DSTableRow)
    Private _cssClassRow As String
    Private _cssClassAlternativeRow As String

    'TODO: Gestione di scrittura su file
    Private _bufferFilePath As String = String.Empty
    Private _rowCount As Integer
    Private _writer As StreamWriter
    Private _htmlWriter As HtmlTextWriter

#End Region

#Region " Constructors "

    ' TODO: NON USARLA PER NUOVE IMPLEMENTAZIONI
    Public Sub New(Optional ByVal useDefaultStyles As Boolean = True)
        _table = New Table()
        InitRowCollection(_table)

        If useDefaultStyles Then
            _table.CellPadding = 1
        End If
    End Sub

#End Region

#Region "DSTableRow Collection"

    ''' <summary>
    ''' Inizializza la collezione di DSTableRow con le righe della tabella passata come parametro
    ''' </summary>
    Private Sub InitRowCollection(ByRef table As Table)
        _tableRowCollection = New List(Of DSTableRow)
        For Each row As TableRow In table.Rows
            _tableRowCollection.Add(New DSTableRow(row))
        Next
    End Sub

    ''' <summary> Aggiunge una riga alla tabella. </summary>
    ''' <param name="tableRow">Riga da aggiungere</param>
    Public Sub AddRow(ByRef tableRow As DSTableRow)
        If String.IsNullOrEmpty(tableRow.CSSClass) Then
            If (_table.Rows.Count Mod 2 = 0) AndAlso Not String.IsNullOrEmpty(CSSClassAlternativeRow) Then
                tableRow.CSSClass = CSSClassAlternativeRow
            Else
                tableRow.CSSClass = CSSClassRow
            End If
        End If
        _tableRowCollection.Add(tableRow)
        _table.Rows.Add(tableRow.TableRow)
        _rowCount += 1
    End Sub

    ''' <summary>
    ''' Rimuove una riga dalla tabella
    ''' </summary>
    ''' <param name="tableRow">Riga da rimuovere</param>
    ''' <remarks></remarks>
    Public Sub RemoveRow(ByRef tableRow As DSTableRow)
        _tableRowCollection.Remove(tableRow)
        _table.Rows.Remove(tableRow.TableRow)
    End Sub

    ''' <summary>
    ''' Aggiunge una singola cella alla tabella inserendo una riga
    ''' </summary>
    ''' <param name="tableCell">Cella da aggiungere</param>
    ''' <remarks></remarks>
    Public Sub AddSingleCell(ByRef tableCell As DSTableCell, Optional ByVal useDefaultStyles As Boolean = True)
        Dim row As New DSTableRow(tableCell)
        If useDefaultStyles Then
            row.Width = Unit.Percentage(100)
        End If
        AddRow(row)
    End Sub

    ''' <summary>
    ''' Aggiunge una cella all'ultima riga della tabella
    ''' </summary>
    ''' <param name="tableCell">Cella da aggiungere</param>
    ''' <remarks></remarks>
    Public Sub AppendCell(ByRef tableCell As DSTableCell)
        Dim row As DSTableRow = Rows(_tableRowCollection.Count - 1)
        row.AddCell(tableCell)
    End Sub

    ''' <summary>
    ''' Crea una nuova riga senza celle nella tabella
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub CreateEmptyRow(Optional ByVal useDefaultStyles As Boolean = True)
        Dim row As New DSTableRow
        If useDefaultStyles Then
            row.Width = Unit.Percentage(100)
        End If
        AddRow(row)
    End Sub

    ''' <summary> Crea una nuova riga senza celle applicando uno stile </summary>
    Public Sub CreateEmptyRow(ByVal cssClass As String)
        Dim row As New DSTableRow
        row.CSSClass = cssClass
        AddRow(row)
    End Sub
#End Region

#Region "Properties"
    ''' <summary> Larghezza della tabella </summary>
    Public Property Width() As Unit
        Get
            Return _table.Width
        End Get
        Set(ByVal value As Unit)
            _table.Width = value
        End Set
    End Property

    ''' <summary> Imposta una classe CSS per tutte le righe della tabella </summary>
    Public Property CSSClassRow() As String
        Get
            Return _cssClassRow
        End Get
        Set(ByVal value As String)
            _cssClassRow = value
        End Set
    End Property
    ''' <summary>
    ''' Imposta una classe CSS per tutte le righe alternate della tabella
    ''' </summary>
    ''' <value>Stringa che identifica la classe CSS</value>
    ''' <returns>Stringa che identifica la classe CSS</returns>
    ''' <remarks></remarks>
    Public Property CSSClassAlternativeRow() As String
        Get
            Return _cssClassAlternativeRow
        End Get
        Set(ByVal value As String)
            _cssClassAlternativeRow = value
        End Set
    End Property
    ''' <summary>
    ''' Imposta una classe CSS per la tabella
    ''' </summary>
    ''' <value>Stringa che identifica la classe CSS</value>
    ''' <returns>Stringa che identifica la classe CSS</returns>
    ''' <remarks></remarks>
    Public Property CSSClass() As String
        Get
            Return _table.CssClass
        End Get
        Set(ByVal value As String)
            _table.CssClass = value
        End Set
    End Property
    ''' <summary>
    ''' Restituisce l'ultima riga della tabella
    ''' </summary>
    ''' <remarks></remarks>
    Public ReadOnly Property CurrentRow() As DSTableRow
        Get
            Return Rows(Rows.Count - 1)
        End Get
    End Property
    ''' <summary>
    ''' Restituisce la collezione di righe della tabella
    ''' </summary>
    ''' <value>Lista di DSTableRow</value>
    ''' <returns>Lista di DSTableRow</returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Rows() As IList(Of DSTableRow)
        Get
            Return _tableRowCollection
        End Get
    End Property
    ''' <summary>
    ''' Restituisce il controllo Table usato per costruire la tabella
    ''' </summary>
    ''' <value>Controllo Table</value>
    ''' <returns>Controllo Table</returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Table() As Table
        Get
            Return _table
        End Get
    End Property
#End Region

#Region "Gestione BUFFER"

#Region "Properties"
    ''' <summary>
    ''' Indica se la classe utilizza il Buffer per la generazione della stampa
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property UseBuffer() As Boolean
        Get
            Return Not String.IsNullOrEmpty(_bufferFilePath)
        End Get
    End Property

    ''' <summary>
    ''' Conteggio delle righe della stampa
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property RowCount() As Integer
        Get
            If UseBuffer Then
                Return _rowCount
            Else
                Return Rows.Count
            End If
        End Get
    End Property

    ''' <summary>
    ''' Stringa del file temporaneo del Buffer
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property BufferFilePath() As String
        Get
            Return _bufferFilePath
        End Get
    End Property
#End Region

#Region "Public Methods"
    ''' <summary>
    ''' Inizializza il buffer su file
    ''' </summary>
    ''' <param name="PrintType"></param>
    ''' <remarks></remarks>
    Public Sub InitializeBuffer(ByVal PrintType As String)
        If Not String.IsNullOrEmpty(PrintType) Then

            'Cancello tutti i file temporanei per questa stampa di questo utente
            Dim CommInstance As CommonUtil = CommonUtil.GetInstance
            Dim searchPattern As String = String.Format("{0}*{1}*.html", DocSuiteContext.Current.User.UserName, PrintType)
            For Each fileName As String In Directory.GetFiles(CommInstance.AppTempPath, searchPattern, SearchOption.TopDirectoryOnly)
                Try
                    File.Delete(fileName)
                Catch ex As IOException
                    ' Il file precedente è bloccato da codice fatto male
                End Try
            Next

            'Setto il nome temporaneo
            Dim tempFileName As String = String.Format("{0}-{1}-{2:HHmmss}.html", CommonUtil.UserDocumentName, PrintType, Now())
            _bufferFilePath = CommInstance.AppTempPath & tempFileName

            'Setto i writer
            _writer = New StreamWriter(_bufferFilePath)
            _htmlWriter = New HtmlTextWriter(_writer)
        Else
            CloseBuffer()
        End If
    End Sub

    ''' <summary>
    ''' Scrive l'ultima Row inserita nel buffer, eliminandola dalla memoria
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub FlushBuffer()
        If UseBuffer And _tableRowCollection.Count > 0 Then
            'recupero l'ultima Row
            Using _row As DSTableRow = Rows(_tableRowCollection.Count - 1)
                'scrivo su file
                _row.TableRow.RenderControl(_htmlWriter)
                _htmlWriter.Flush()

                'Rimuovo la row dala memoria
                RemoveRow(_row)
            End Using
        End If
    End Sub

    ''' <summary>
    ''' Chiude il buffer e relativi writer
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub CloseBuffer()
        'chiudo i writer
        If _htmlWriter IsNot Nothing Then
            _htmlWriter.Close()
            _htmlWriter.Dispose()
        End If
        If _writer IsNot Nothing Then
            _writer.Close()
            _writer.Dispose()
        End If
    End Sub
#End Region

#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                CloseBuffer()
            End If
        End If
        disposedValue = True
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>This code added by Visual Basic to correctly implement the disposable pattern</remarks>
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class