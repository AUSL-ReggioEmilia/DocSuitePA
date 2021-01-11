Imports System.Web.UI.WebControls
Imports System.Collections.Generic

''' <summary>
''' Classe per la costruzione di una riga di una tabella html
''' </summary>
Public Class DSTableRow
    Inherits DSHtmlControl
    Implements IDisposable

#Region "Fields"

    Private _tableCellCollection As IList(Of DSTableCell)
    Private _tableRow As TableRow
    Private _cssClassCell As String

#End Region

#Region "Properties"

    ''' <summary> Larghezza della riga. </summary>
    Public Property Width() As Unit
        Get
            Return _tableRow.Width
        End Get
        Set(ByVal value As Unit)
            _tableRow.Width = value
        End Set
    End Property

    ''' <summary> Imposta una classe CSS per la riga. </summary>
    Public Property CSSClass() As String
        Get
            Return _tableRow.CssClass
        End Get
        Set(ByVal value As String)
            _tableRow.CssClass = value
        End Set
    End Property

    ''' <summary> Imposta una classe CSS per tutte le celle della riga. </summary>
    Public Property CSSClassCell() As String
        Get
            Return _cssClassCell
        End Get
        Set(ByVal value As String)
            _cssClassCell = value
        End Set
    End Property

    ''' <summary> Restituisce l'ultima cella della riga. </summary>
    Public ReadOnly Property CurrentCell() As DSTableCell
        Get
            Return Cells(Cells.Count - 1)
        End Get
    End Property

    ''' <summary> Restituisce la collezione di celle della riga. </summary>
    Public ReadOnly Property Cells() As IList(Of DSTableCell)
        Get
            Return _tableCellCollection
        End Get
    End Property

    ''' <summary> Restituisce il controllo TableRow usato per costruire la riga. </summary>
    Public ReadOnly Property TableRow() As TableRow
        Get
            Return _tableRow
        End Get
    End Property

#End Region

#Region " Constructor "

    ''' <summary> Inizializza una riga per tabella html. </summary>
    Public Sub New()
        _tableCellCollection = New List(Of DSTableCell)
        _tableRow = New TableRow()
    End Sub

    ''' <summary> Inizializza una riga per tabella html. </summary>
    Public Sub New(ByVal cell As DSTableCell)
        Me.New()
        AddCell(cell)
    End Sub

    ''' <summary> Inizializza una riga per tabella html. </summary>
    Public Sub New(ByVal CSSClass As String)
        Me.New()
        Me.CSSClass = CSSClass
    End Sub

    ''' <summary> Inizializza una riga per tabella html. </summary>
    Public Sub New(ByVal cell As DSTableCell, ByVal CSSClass As String)
        Me.New(cell)
        Me.CSSClass = CSSClass
    End Sub

    ''' <summary> Inizializza una riga per tabella html. </summary>
    Public Sub New(ByVal tableRow As TableRow)
        _tableRow = tableRow
        InitCellCollection(_tableRow)
    End Sub

#End Region

#Region " Methods "

    ''' <summary> Inizializza la collezione di DSTableCell con le celle della riga passate come parametro. </summary>
    Private Sub InitCellCollection(ByRef tableRow As TableRow)
        _tableCellCollection = New List(Of DSTableCell)
        For Each cell As TableCell In tableRow.Cells
            _tableCellCollection.Add(New DSTableCell(cell))
        Next
    End Sub

    ''' <summary> Aggiunge una cella alla collezione di celle. </summary>
    Public Sub AddCell(ByRef tableCell As DSTableCell)
        If String.IsNullOrEmpty(tableCell.CSSClass) Then
            If String.IsNullOrEmpty(CSSClassCell) Then
                tableCell.CSSClass = _tableRow.CssClass
            Else
                tableCell.CSSClass = CSSClassCell
            End If
        End If
        _tableCellCollection.Add(tableCell)
        _tableRow.Cells.Add(tableCell.TableCell)
    End Sub

    ''' <summary> Rimuove una cella dalla collezione di cell. </summary>
    Public Sub RemoveCell(ByRef tableCell As DSTableCell)
        _tableCellCollection.Remove(tableCell)
        _tableRow.Cells.Remove(tableCell.TableCell)
    End Sub

    ''' <summary> Appende una cella vuota alla lista di celle della riga. </summary>
    Public Sub CreateEmpytCell(Optional ByVal useDefaultStyles As Boolean = True)
        Dim cell As New DSTableCell(useDefaultStyles)
        If useDefaultStyles Then
            cell.Width = Unit.Percentage(100)
        End If
        Me.AddCell(cell)
    End Sub

#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                _tableRow.Dispose()
            End If
        End If
        Me.disposedValue = True
    End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
