Imports System.Text
Imports System.Linq
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class uscFascicleSelect
    Inherits DocSuite2008BaseControl

    Public Delegate Sub OnSelectedEventHandler(ByVal sender As Object, ByVal e As EventArgs)

    ''' <summary> Scatenato alla pressione del pulsante seleziona </summary>
    Public Event FascicleSelected As OnSelectedEventHandler

#Region " Fields "

    Private _selFascicle As Fascicle = Nothing

#End Region

#Region " Properties "

    ''' <summary> fascicolo selezionato </summary>
    Public ReadOnly Property SelectedFascicle() As Fascicle
        Get
            If _selFascicle Is Nothing Then
                _selFascicle = Facade.FascicleFacade.GetByYearNumberCategory(Short.Parse(txtYear.Text), SelectedCategory.Id, Integer.Parse(txtNumber.Text))
            End If
            Return _selFascicle
        End Get
    End Property

    ''' <summary> classificatore </summary>
    Public Property SelectedCategory() As Category
        Get
            Return ViewState("_selCategory")
        End Get
        Set(ByVal value As Category)
            ViewState("_selCategory") = value
        End Set
    End Property

    ''' <summary> Restituisce il controllo TextBox che identifica l'anno </summary>
    Public ReadOnly Property TextBoxYear() As TextBox
        Get
            Return txtYear
        End Get
    End Property

    ''' <summary> Restituisce il controllo TextBox che identifica il numero </summary>
    Public ReadOnly Property TextBoxNumber() As TextBox
        Get
            Return txtNumber
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjaxSettings()
        Initialize()
    End Sub

    Protected Sub uscResolutionSelect_AjaxRequest(ByVal sender As Object, ByVal e As Telerik.Web.UI.AjaxRequestEventArgs)
        Dim arguments As String() = Split(e.Argument, "|")
        If arguments(0) = Me.ClientID Then
            Dim fascParam As FascicleParam = JsonConvert.DeserializeObject(Of FascicleParam)(arguments(1))
            txtYear.Text = fascParam.Year.ToString()
            txtNumber.Text = fascParam.Incremental.ToString()
            SelectedCategory = Facade.CategoryFacade.GetById(fascParam.IdCategory)
            uscCategory.DataSource.Add(SelectedCategory)
            uscCategory.DataBind()
            'btnSeleziona_Click(sender, New EventArgs())
            AjaxManager.ResponseScripts.Add("document.getElementById('" & btnSeleziona.ClientID & "').click();")
        End If
    End Sub

    ''' <summary> Evento scatenato al click del pulsante "Seleziona": nel caso esista un protocollo con tale anno e numero valorizza la proprietà SelectedProtocol </summary>
    Private Sub btnSeleziona_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSeleziona.Click
        If CheckData() Then
            BindFascicle()
            RaiseEvent FascicleSelected(sender, e)
        End If
    End Sub

    Private Sub uscCategory_CategoryAdded(ByVal sender As Object, ByVal e As EventArgs) Handles uscCategory.CategoryAdded
        If uscCategory.HasSelectedCategories Then
            SelectedCategory = uscCategory.SelectedCategories.First()
        Else
            SelectedCategory = Nothing
        End If
    End Sub

    Private Sub uscCategory_CategoryRemoved(ByVal sender As Object, ByVal e As EventArgs) Handles uscCategory.CategoryRemoved
        If uscCategory.HasSelectedCategories Then
            SelectedCategory = uscCategory.SelectedCategories.First()
        Else
            SelectedCategory = Nothing
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSeleziona, uscFascilePreview)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, ajaxPanel)
        AddHandler AjaxManager.AjaxRequest, AddressOf uscResolutionSelect_AjaxRequest
    End Sub

    Private Sub Initialize()
        btnCerca.OnClientClick = "return " & Me.ID & "_OpenSearchWindow();"
    End Sub

    ''' <summary> Visualizza anteprima Protocollo </summary>
    Public Sub ShowPreviewMode()
        tblSearch.Visible = False
        BindFascicle()
    End Sub

    Private Sub BindFascicle()
        If SelectedFascicle IsNot Nothing Then
            uscFascilePreview.Visible = True
            uscFascilePreview.CurrentFascicle = _selFascicle
            uscFascilePreview.Show()
            txtYear.Text = SelectedFascicle.Year.ToString()
            txtNumber.Text = SelectedFascicle.Number.ToString()
        Else
            uscFascilePreview.Visible = False
        End If
    End Sub

    Private Function CheckData() As Boolean
        Dim s As New StringBuilder
        If String.IsNullOrEmpty(txtNumber.Text) Then
            s.AppendLine("Numero Obbligatorio")
        End If
        If String.IsNullOrEmpty(txtYear.Text) Then
            s.AppendLine("Anno Obbligatorio")
        End If
        If Not uscCategory.HasSelectedCategories Then
            s.AppendLine("Classificatore Obbligatorio")
        End If
        If s.Length > 0 Then
            BasePage.AjaxAlert(s.ToString())
            Return False
        End If

        Return True
    End Function

#End Region

End Class