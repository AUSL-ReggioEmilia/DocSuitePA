Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls

''' <summary>
''' Classe per la costruzione di una tabella Html
''' </summary>
Public Class DSTableCell
    Inherits DSHtmlControl
    Implements IDisposable

#Region "Fields"
    Dim _tableCell As TableCell
    Dim _lineBox As Boolean
    Dim _properties As DSTableCellStyle
#End Region

#Region "New"
    Public Sub New(Optional ByVal useDefaultStyles As Boolean = True)
        _tableCell = New TableCell()
        If useDefaultStyles Then
            _tableCell.Style.Add("padding", "1px 1px 1px 1px")
        End If
        _properties = New DSTableCellStyle()
    End Sub

    Public Sub New(ByVal text As String)
        Me.New()
        Me.Text = text
    End Sub

    Public Sub New(ByVal tableCell As TableCell)
        _tableCell = tableCell
    End Sub
#End Region

#Region "Public Functions"
    ''' <summary>
    ''' Aggiunge un controllo alla cella
    ''' </summary>
    ''' <param name="control">Controllo da aggiungere</param>
    ''' <remarks></remarks>
    Public Sub AddCellControl(ByVal control As System.Web.UI.Control)
        _tableCell.Controls.Add(control)
    End Sub
    ''' <summary>
    ''' Rimuove un controllo dalla cella
    ''' </summary>
    ''' <param name="control">Controllo da rimuovere</param>
    ''' <remarks></remarks>
    Public Sub RemoveCellControl(ByVal control As System.Web.UI.Control)
        _tableCell.Controls.Remove(control)
    End Sub
    ''' <summary>
    ''' Aggiunge un Image Control alla cella
    ''' </summary>
    ''' <param name="imagePath">Path completo dell'immagine</param>
    ''' <remarks></remarks>
    Public Sub AddImageCellControl(ByVal imagePath As String)
        Dim image As New Image()
        image.ImageUrl = imagePath
        _tableCell.Controls.Add(image)
    End Sub
    ''' <summary>
    ''' Aggiunge un Image Control alla cella specificandogli tooltip ed evento javascript sull'onclick
    ''' </summary>
    ''' <param name="imagePath">Path completo dell'immagine</param>
    ''' <param name="toolTip">Test Tooltip</param>
    ''' <param name="onClickScript">Script javascript scatenato sull'evento onclick</param>
    ''' <remarks></remarks>
    Public Sub AddImageCellControl(ByVal imagePath As String, ByVal toolTip As String, ByVal onClickScript As String)
        Dim image As New Image()
        image.ImageUrl = imagePath
        image.ToolTip = toolTip
        If Not String.IsNullOrEmpty(onClickScript) Then
            image.Attributes.Add("onclick", onClickScript & ";return false;")
            image.Attributes.Add("style", "cursor:hand;")
        End If
        _tableCell.Controls.Add(image)
    End Sub
    ''' <summary>
    ''' Applica una stile alla cella
    ''' </summary>
    ''' <param name="style">Oggetto DSTableCellStyle che contiene le proprietà grafiche che deve avere la cella</param>
    ''' <remarks></remarks>
    Public Sub ApplyStyle(ByVal style As DSTableCellStyle)
        _tableCell.Width = style.Width
        _tableCell.Font.CopyFrom(style.Font)
        _tableCell.HorizontalAlign = style.HorizontalAlignment
        _tableCell.VerticalAlign = style.VerticalAlignment
        _tableCell.ColumnSpan = style.ColumnSpan
        _lineBox = style.LineBox
        If style.LineBox Then
            _tableCell.BorderStyle = BorderStyle.Solid
            _tableCell.BorderWidth = Unit.Pixel(1)
        Else
            _tableCell.BorderStyle = BorderStyle.None
            _tableCell.BorderWidth = Unit.Pixel(0)
        End If
        _tableCell.Wrap = style.Wrap
        _tableCell.CssClass = style.CSSClass
    End Sub

    Public Sub ApplyStyle(ByVal width As Unit, ByVal bold As Boolean, ByVal hAlign As HorizontalAlign, ByVal lineBox As Boolean, ByVal colSpan As Integer)
        Dim style As New DSTableCellStyle()
        style.Width = width
        style.Font.Bold = bold
        style.HorizontalAlignment = hAlign
        style.LineBox = lineBox
        style.ColumnSpan = colSpan
        style.Wrap = True
        ApplyStyle(style)
    End Sub

    Public Sub AddDividingLineControl(Optional ByVal color As String = "#000000", Optional ByVal size As String = "1")
        Dim hrControl As HtmlGenericControl = New HtmlGenericControl("hr")
        hrControl.Attributes.Add("color", color)
        hrControl.Attributes.Add("size", size)
        hrControl.Attributes.Add("noshade", "")
        AddCellControl(hrControl)
    End Sub

#End Region

#Region "Properties"
    ''' <summary>
    ''' Restituisce lo stile da applicare alla cella
    ''' </summary>
    ''' <value>Oggetto DSTableCellStyle</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Style() As DSTableCellStyle
        Get
            Return _properties
        End Get
    End Property
    ''' <summary>
    ''' Restituisce/imposta il testo che deve contenere la cella
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Text() As String
        Get
            Return _tableCell.Text
        End Get
        Set(ByVal value As String)
            _tableCell.Text = value
        End Set
    End Property
    ''' <summary>
    ''' Restituisce/imposta la larghezza della cella
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Width() As Unit
        Get
            Return _tableCell.Width
        End Get
        Set(ByVal value As Unit)
            _tableCell.Width = value
        End Set
    End Property
    ''' <summary>
    ''' Restituisce/imposta il Font del testo della cella
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Font() As FontInfo
        Get
            Return _tableCell.Font
        End Get
    End Property
    ''' <summary>
    ''' Restituisce/imposta l'allineamento orizzontale dela cella
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property HorizontalAlignment() As HorizontalAlign
        Get
            Return _tableCell.HorizontalAlign
        End Get
        Set(ByVal value As HorizontalAlign)
            _tableCell.HorizontalAlign = value
        End Set
    End Property
    ''' <summary>
    ''' Restituisce/imposta l'allineamento verticale dela cella
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property VerticalAlignment() As VerticalAlign
        Get
            Return _tableCell.VerticalAlign
        End Get
        Set(ByVal value As VerticalAlign)
            _tableCell.VerticalAlign = value
        End Set
    End Property
    ''' <summary>
    ''' Restituisce/imposta il columnspan dela cella
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ColumnSpan() As Integer
        Get
            Return _tableCell.ColumnSpan
        End Get
        Set(ByVal value As Integer)
            _tableCell.ColumnSpan = value
        End Set
    End Property
    ''' <summary>
    ''' Se True imposta un bordo intorno alla cella
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LineBox() As Boolean
        Get
            Return _lineBox
        End Get
        Set(ByVal value As Boolean)
            If value = True Then
                _tableCell.BorderStyle = BorderStyle.Solid
                _tableCell.BorderWidth = Unit.Pixel(1)
            Else
                _tableCell.BorderStyle = BorderStyle.None
                _tableCell.BorderWidth = Unit.Pixel(0)
            End If
            _lineBox = value
        End Set
    End Property
    ''' <summary>
    ''' Restituisce/imposta la classe CSS da applicare alla cella
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CSSClass() As String
        Get
            Return _tableCell.CssClass
        End Get
        Set(ByVal value As String)
            _tableCell.CssClass = value
        End Set
    End Property
    ''' <summary>
    ''' Restituisce l'oggetto TableCell
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property TableCell() As TableCell
        Get
            Return _tableCell
        End Get
    End Property
#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                _tableCell.Dispose()
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
