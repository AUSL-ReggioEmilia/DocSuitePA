Imports System.Text
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI

Partial Public Class uscOggetto
    Inherits DocSuite2008BaseControl

    Public Enum ObjectMode
        [Object] = 0
        Contract = 1
    End Enum

#Region " Fields "

    Private _objMode As ObjectMode = ObjectMode.Object
    Private _contractDocType As String = ""

#End Region

#Region " Properties "

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
            Return txtObject.Text
        End Get
        Set(ByVal value As String)
            txtObject.Text = value
        End Set
    End Property

    Public Property EditMode() As Boolean
        Get
            If ViewState("_editmode") Is Nothing Then
                ViewState("_editmode") = True
            End If
            Return DirectCast(ViewState("_editmode"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            ViewState("_editmode") = value
        End Set
    End Property

    Public Property MultiLine() As Boolean
        Get
            Return (txtObject.TextMode = InputMode.MultiLine)
        End Get
        Set(ByVal value As Boolean)
            txtObject.TextMode = If(value, InputMode.MultiLine, InputMode.SingleLine)
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

    Public Property Enabled() As Boolean
        Get
            Return txtObject.Enabled
        End Get
        Set(ByVal value As Boolean)
            txtObject.Enabled = value
            txtObject.ShowButton = value
        End Set
    End Property

    Public Property Mode() As ObjectMode
        Get
            Return _objMode
        End Get
        Set(ByVal value As ObjectMode)
            _objMode = value
        End Set
    End Property

    Public Property ContractDocType() As String
        Get
            Return _contractDocType
        End Get
        Set(ByVal value As String)
            _contractDocType = value
        End Set
    End Property

    Public ReadOnly Property TextBoxControl() As Control
        Get
            ' TODO: sopprimere sta proprietà
            Return txtObject
        End Get
    End Property

    Public Property TextBoxWidth() As Unit
        Get
            Return txtObject.Width
        End Get
        Set(ByVal value As Unit)
            txtObject.Width = value
        End Set
    End Property

    Public ReadOnly Property PanelControl As Control
        Get
            Return pnlId
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()

        If Not Page.IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub AjaxRequestHandler(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        If String.IsNullOrEmpty(e.Argument) OrElse Not e.Argument.Split("|"c)(0).Eq(ID) Then
            Exit Sub
        End If
        txtObject.Text = e.Argument.Split("|"c)(1)
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf AjaxRequestHandler

        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, txtObject)
    End Sub

    Private Sub Initialize()
        If Not EditMode Then
            txtObject.ReadOnly = True
            txtObject.ShowButton = False
            Exit Sub
        End If

        txtObject.ShowButton = True
        txtObject.ClientEvents.OnButtonClick = ID & "_OpenObjectWindow"
    End Sub

    Public Function GetUrl() As String
        Dim url As New StringBuilder()
        If Mode = ObjectMode.Contract Then
            url.Append("../Cont/ContTableContract.aspx?Action=Sel")
        ElseIf Mode = ObjectMode.Object Then
            url.Append("../UserControl/CommonSelOggetto.aspx?")
        Else
            Throw New DocSuiteException("Inizializzazione controllo", String.Format("Modalità di oggetto errata. {0}", ProtocolEnv.DefaultErrorMessage))
        End If
        If Not String.IsNullOrEmpty(Type) Then
            url.AppendFormat("&Type={0}", Type)
        End If
        If Not String.IsNullOrEmpty(ContractDocType) Then
            url.AppendFormat("&DocType={0}", ContractDocType)
        End If
        Return url.ToString()
    End Function

#End Region

End Class