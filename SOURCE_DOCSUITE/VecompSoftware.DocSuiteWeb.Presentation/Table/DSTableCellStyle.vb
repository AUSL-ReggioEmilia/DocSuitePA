Imports System.Web.UI.WebControls
''' <summary>
''' Classe per definire uno stile da applicare ad una cella
''' </summary>
''' <remarks></remarks>
Public Class DSTableCellStyle

#Region "Fields"
    Private _width As Unit
    Private _font As FontInfo
    Private _halignment As HorizontalAlign
    Private _valignment As VerticalAlign
    Private _columnSpan As Integer
    Private _lineBox As Boolean
    Private _wrap As Boolean
    Private _cssClass As String
#End Region

    Public Sub New()
        Dim style As New Style
        _font = style.Font
    End Sub


#Region "Properties"
    ''' <summary>
    ''' Larghezza della cella
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Width() As Unit
        Get
            Return _width
        End Get
        Set(ByVal value As Unit)
            _width = value
        End Set
    End Property
    ''' <summary>
    ''' Font del testo contenuto nella cella
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Font() As FontInfo
        Get
            Return _font
        End Get
        Set(ByVal value As FontInfo)
            _font = value
        End Set
    End Property
    ''' <summary>
    ''' Allineamento orizzontale della cella
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property HorizontalAlignment() As HorizontalAlign
        Get
            Return _halignment
        End Get
        Set(ByVal value As HorizontalAlign)
            _halignment = value
        End Set
    End Property
    ''' <summary>
    ''' Allineamento verticale della cella
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property VerticalAlignment() As VerticalAlign
        Get
            Return _valignment
        End Get
        Set(ByVal value As VerticalAlign)
            _valignment = value
        End Set
    End Property
    ''' <summary>
    ''' ColumnSpan della cella
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ColumnSpan() As Integer
        Get
            Return _columnSpan
        End Get
        Set(ByVal value As Integer)
            _columnSpan = value
        End Set
    End Property
    ''' <summary>
    ''' Se true imposta un bordo intorno alla cella
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LineBox() As Boolean
        Get
            Return _lineBox
        End Get
        Set(ByVal value As Boolean)
            _lineBox = value
        End Set
    End Property
    ''' <summary>
    ''' Se true imposta un bordo intorno alla cella
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Wrap() As Boolean
        Get
            Return _wrap
        End Get
        Set(ByVal value As Boolean)
            _wrap = value
        End Set
    End Property
    ''' <summary>
    ''' Classe CSS da applicare alla cella
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CSSClass() As String
        Get
            Return _cssClass
        End Get
        Set(ByVal value As String)
            _cssClass = value
        End Set
    End Property
#End Region
End Class
