Imports Telerik.Web.UI

Partial Public Class uscProtocolObjectChanger
    Inherits WindowControl

#Region "Fields"
    Private _year As String
    Private _number As String
#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeButtons()
        txtObject.MaxLength = ProtocolEnv.ObjectMaxLength()
        txtObjectChanged.MaxLength = ProtocolEnv.ObjectMaxLength()
    End Sub

    ''' <summary> Apertura finestra cambio oggetto </summary>
    Private Sub btnOpenObject_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs) Handles btnOpenObject.Click
        Dim URL As String = "../UserControl/CommonChangeOggetto.aspx?Year=" & Me.Year & "&Number=" & Me.Number
        WindowBuilder.LoadWindow("windowChangeOggetto", URL, Me.ID & "_OnClose", Unit.Pixel(650), Unit.Pixel(500))
    End Sub

#End Region

#Region " Methods "
    Private Sub InitializeButtons()
        Me.RegisterWindowManager(RadWindowManagerOggetto)
        WindowBuilder.RegisterOpenerElement(btnOpenObject)
        RadWindowManagerOggetto.Windows(0).OnClientClose = Me.ID & "_OnClose"
    End Sub
#End Region

#Region "Properties"
    Public Property MaxLength() As Integer
        Get
            Return txtObject.MaxLength
        End Get
        Set(ByVal value As Integer)
            txtObject.MaxLength = value
        End Set
    End Property

    Public Property Text() As String
        Get
            Return txtObjectChanged.Text
        End Get
        Set(ByVal value As String)
            txtObjectChanged.Text = value
            txtObject.Text = value
        End Set
    End Property

    Public Property MultiLine() As Boolean
        Get
            Return (txtObject.TextMode = TextBoxMode.MultiLine)
        End Get
        Set(ByVal value As Boolean)
            If value = True Then
                txtObject.TextMode = TextBoxMode.MultiLine
            Else
                txtObject.TextMode = TextBoxMode.SingleLine
            End If

        End Set
    End Property

    Public Property Required() As Boolean
        Get
            Return rfvObject.Enabled
        End Get
        Set(ByVal value As Boolean)
            rfvObject.Enabled = value
        End Set
    End Property

    Public Property RequiredMessage() As String
        Get
            Return rfvObject.ErrorMessage
        End Get
        Set(ByVal value As String)
            rfvObject.ErrorMessage = value
        End Set
    End Property

    Public Property Year() As String
        Get
            Return GetPropertyValue(Of String)("_year", 0)
        End Get
        Set(ByVal value As String)
            SetPropertyValue(Of String)("_year", value)
        End Set
    End Property

    Public Property Number() As String
        Get
            Return GetPropertyValue(Of String)("_number", 0)
        End Get
        Set(ByVal value As String)
            SetPropertyValue(Of String)("_number", value)
        End Set
    End Property

    Public ReadOnly Property ObjectChangeReason() As String
        Get
            Return txtChangeReason.Text
        End Get
    End Property
#End Region

#Region "Properties: Controls"
    Public ReadOnly Property TextBoxControl() As RadTextBox
        Get
            Return txtObject
        End Get
    End Property
#End Region

End Class