Imports System.Text

Partial Public Class uscServiceCategory
    Inherits WindowControl

#Region " Fields "

    Private _headertitle As String = ""
    Private _editmode As Boolean = True

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeButtons()
        If Not Page.IsPostBack Then
            InitializeObject()
        End If
    End Sub

    ''' <summary> Apertura finestra selezione classificatore </summary>
    Private Sub SelO_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs) Handles SelO.Click
        Dim url As String = "../UserControl/CommonSelServiceCategory.aspx?" & GetWindowParameters()
        WindowBuilder.LoadWindow("windowSelServiceCategory", url, ID & "_OnClose", Unit.Pixel(600), Unit.Pixel(460))
    End Sub

#End Region

#Region "Properties"

    Public Property HeaderTitle() As String
        Get
            Return _headertitle
        End Get
        Set(ByVal value As String)
            _headertitle = value
        End Set
    End Property

    Public Property MaxLength() As Integer
        Get
            Return txtServiceCategory.MaxLength
        End Get
        Set(ByVal value As Integer)
            txtServiceCategory.MaxLength = value
        End Set
    End Property

    Public Property Text() As String
        Get
            Return txtServiceCategory.Text
        End Get
        Set(ByVal value As String)
            txtServiceCategory.Text = value
        End Set
    End Property

    Public Property EditMode() As Boolean
        Get
            Return _editmode
        End Get
        Set(ByVal value As Boolean)
            _editmode = value
        End Set
    End Property

    Public Property MultiLine() As Boolean
        Get
            Return (txtServiceCategory.TextMode = TextBoxMode.MultiLine)
        End Get
        Set(ByVal value As Boolean)
            If value = True Then
                txtServiceCategory.TextMode = TextBoxMode.MultiLine
            Else
                txtServiceCategory.TextMode = TextBoxMode.SingleLine
            End If
        End Set
    End Property

    Public Property Required() As Boolean
        Get
            Return rfvServiceCategory.Enabled
        End Get
        Set(ByVal value As Boolean)
            rfvServiceCategory.Enabled = value
        End Set
    End Property

    Public Property Enabled() As Boolean
        Get
            Return txtServiceCategory.Enabled
        End Get
        Set(ByVal value As Boolean)
            txtServiceCategory.Enabled = value
            SelO.Enabled = value
        End Set
    End Property

    Public Property CategoryText() As String
        Get
            Return txtServiceCategory.Text
        End Get
        Set(value As String)
            txtServiceCategory.Text = value
        End Set
    End Property

    Public Property TextBoxWidth() As String
        Get
            Return txtServiceCategory.Width.ToString()
        End Get
        Set(ByVal value As String)
            txtServiceCategory.Width = Unit.Parse(value)
            tdServiceCategoryText.Width = value
        End Set

    End Property
#End Region

#Region " Methods "

    Private Sub InitializeButtons()
        RegisterWindowManager(RadWindowManagerServiceCategory)
        WindowBuilder.RegisterOpenerElement(SelO)
    End Sub

    Private Sub InitializeObject()
        If EditMode = False Then
            txtServiceCategory.ReadOnly = True
        End If
        SelO.Visible = EditMode
    End Sub

    Private Function GetWindowParameters() As String
        Dim parameters As New StringBuilder

        If Not String.IsNullOrEmpty(BasePage.Type) Then
            parameters.AppendFormat("&Type={0}", BasePage.Type)
        End If

        Return parameters.ToString()
    End Function

#End Region

End Class