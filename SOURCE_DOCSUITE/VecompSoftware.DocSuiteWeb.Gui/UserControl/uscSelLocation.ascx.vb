Imports System.Text
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class uscSelLocation
    Inherits WindowControl

#Region " Fields "

    Private _location As Location
    Private _tipo As String

#End Region

#Region " Properties "

    Public Property Caption() As String
        Get
            Return lblCaption.Text
        End Get
        Set(ByVal value As String)
            lblCaption.Text = value
        End Set
    End Property

    Public Property [ReadOnly]() As Boolean
        Get
            Return Not (btnDelLocation.Visible And btnSelLocation.Visible)
        End Get
        Set(ByVal value As Boolean)
            btnDelLocation.Visible = (Not value)
            btnSelLocation.Visible = (Not value)
        End Set
    End Property

    Public Property Location() As Location
        Get
            If _location Is Nothing Then
                Dim idLocation As Integer
                If Integer.TryParse(txtIdLocation.Text, idLocation) Then
                    _location = New LocationFacade(DbName).GetById(idLocation, False)
                End If
            End If
            Return _location
        End Get
        Set(ByVal value As Location)
            _location = value
            If _location IsNot Nothing Then
                txtIdLocation.Text = _location.Id.ToString()
                txtLocation.Text = String.Format("{0} {1}", _location.Id.ToString(), _location.Name)
            End If
        End Set
    End Property

    Public Property TextReadOnly() As Boolean
        Get
            Return txtLocation.ReadOnly
        End Get
        Set(ByVal value As Boolean)
            txtLocation.ReadOnly = value
        End Set
    End Property

    Public Property Tipo() As String
        Get
            Return _tipo
        End Get
        Set(value As String)
            _tipo = value
        End Set
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        InitializeButtons()
    End Sub

    Private Sub btnDelLocation_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs) Handles btnDelLocation.Click
        txtLocation.Text = String.Empty
        txtIdLocation.Text = String.Empty
    End Sub

    Private Sub btnSelLocation_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs) Handles btnSelLocation.Click
        Dim url As String = "../UserControl/CommonSelLocation.aspx?" & GetWindowParameters()
        WindowBuilder.LoadWindow("windowSelLocazioni", url, ID & "_OnClose", Unit.Pixel(550), Unit.Pixel(450))
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnDelLocation, txtLocation)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnDelLocation, txtIdLocation)
    End Sub

    Private Sub InitializeButtons()
        RegisterWindowManager(RadWindowManagerLocazioni)
        WindowBuilder.RegisterOpenerElement(btnSelLocation)
    End Sub

    Private Function GetWindowParameters() As String
        Dim parameters As New StringBuilder

        If Not String.IsNullOrEmpty(Tipo) Then
            parameters.Append("&Type=" & Tipo)
        ElseIf Not String.IsNullOrEmpty(BasePage.Type) Then
            parameters.Append("&Type=" & BasePage.Type)
        End If

        Dim returnValue As String = parameters.ToString()
        Return returnValue.Remove(returnValue.IndexOf("&"), 1)
    End Function

#End Region
    
End Class