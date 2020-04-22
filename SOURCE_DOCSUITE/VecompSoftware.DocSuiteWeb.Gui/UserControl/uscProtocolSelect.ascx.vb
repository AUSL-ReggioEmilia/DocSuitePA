Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class uscProtocolSelect
    Inherits DocSuite2008BaseControl

    Public Delegate Sub OnSelectedEventHandler(ByVal sender As Object, ByVal e As EventArgs)

    ''' <summary> Scatenato alla pressione del pulsante seleziona </summary>
    Public Event ProtocolSelected As OnSelectedEventHandler

#Region " Fields "

    Private _selectedProtocol As Protocol = Nothing

#End Region

#Region " Properties "

    ''' <summary> Restituisce il protocollo selezionato </summary>
    Public ReadOnly Property SelectedProtocol() As Protocol
        Get
            If _selectedProtocol Is Nothing Then
                Dim year As Short
                Dim number As Integer
                If Short.TryParse(txtYear.Text, year) AndAlso Integer.TryParse(txtNumber.Text, number) Then
                    _selectedProtocol = Facade.ProtocolFacade.GetById(year, number, False)
                End If
            End If

            Return _selectedProtocol
        End Get
    End Property

    ''' <summary> Restituisce o imposta il testo contenuto nella textBox Anno </summary>
    Public Property TextYear() As String
        Get
            Return txtYear.Text
        End Get
        Set(ByVal value As String)
            txtYear.Text = value
        End Set
    End Property

    ''' <summary> Restituisce o imposta il testo contenuto nella textBox Numero </summary>
    Public Property TextNumber() As String
        Get
            Return txtNumber.Text
        End Get
        Set(ByVal value As String)
            txtNumber.Text = value
        End Set
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not Page.IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSeleziona, uscProtocolPreview)
    End Sub

    Private Sub btnSeleziona_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSeleziona.Click
        CheckAndSelect()
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        btnCerca.OnClientClick = String.Format("return {0}_OpenSearchWindow();", ID)
    End Sub

    ''' <summary> Visualizza anteprima Protocollo </summary>
    Public Sub ShowPreviewMode()
        tblSearch.Visible = False
        CheckAndSelect()
    End Sub

    Private Sub CheckAndSelect()
        uscProtocolPreview.Visible = False
        If SelectedProtocol IsNot Nothing Then
            uscProtocolPreview.Visible = True
            uscProtocolPreview.CurrentProtocol = SelectedProtocol
            uscProtocolPreview.Initialize()
            txtYear.Text = SelectedProtocol.Year.ToString()
            txtNumber.Text = SelectedProtocol.Number.ToString()
        End If

        RaiseEvent ProtocolSelected(Me, New EventArgs())
    End Sub

#End Region

End Class

