Imports VecompSoftware.DocSuiteWeb.Facade
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class uscResolutionSelect
    Inherits DocSuite2008BaseControl

    Public Delegate Sub OnSelectedEventHandler(ByVal sender As Object, ByVal e As EventArgs)
    ''' <summary> Scatenato alla pressione del pulsante seleziona </summary>
    Public Event ResolutionSelected As OnSelectedEventHandler

#Region " Fields "

    Private _selResolution As Resolution = Nothing

#End Region

#Region " Properties "

    Public ReadOnly Property InclusiveNumber As String
        Get
            Return String.Concat(txtYear.Text, "/", If(VisibleServiceNumberPanel, _
                                 If(String.IsNullOrEmpty(txtServiceNumber.Text), String.Empty, txtServiceNumber.Text.Trim()), _
                                 If(String.IsNullOrEmpty(txtNumber.Text), String.Empty, txtNumber.Text.Trim())))
        End Get
    End Property

    ''' <summary> Restituisce l'atto selezionato </summary>
    Public Property SelectedResolution() As Resolution
        Get
            If _selResolution Is Nothing Then
                Dim incompleteData As Boolean
                If CheckInputData(incompleteData) Then
                    _selResolution = Facade.ResolutionFacade.GetByIdOrAdoptionData(rblTipo.SelectedValue, txtIdResolution.Text, InclusiveNumber, txtYear.Text)
                End If
            End If
            Return _selResolution
        End Get
        Private Set(value As Resolution)
            _selResolution = value
        End Set
    End Property

    ''' <summary> Restituisce la tipologia di atto selezionata </summary>
    Public ReadOnly Property SelectedResolutionType() As String
        Get
            Return rblTipo.SelectedItem.Text
        End Get
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

    ''' <summary> Restituisce il controllo TextBox che identifica il numero provvisorio </summary>
    Public ReadOnly Property TextBoxIdResolution() As TextBox
        Get
            Return txtIdResolution
        End Get
    End Property

    ''' <summary> Mostra/Nasconde il pannello Anno/Numero di Adozione </summary>
    Public Property VisibleYearNumberPanel() As Boolean
        Get
            Return (tdAdoptionNumber.Visible And tdAdoptionYear.Visible)
        End Get
        Set(ByVal value As Boolean)
            tdAdoptionNumber.Visible = value
            tdAdoptionYear.Visible = value
            tdIdResolution.Visible = value
        End Set
    End Property

    ''' <summary> Mostra/Nasconde il pannello Numero Provvisorio (IdResolution) </summary>
    Public Property VisibleIdResolutionPanel() As Boolean
        Get
            Return tdIdResolution.Visible
        End Get
        Set(ByVal value As Boolean)
            tdIdResolution.Visible = value
        End Set
    End Property


    ''' <summary> Mostra/Nasconde il pannello ServiceNumeber </summary>
    Public Property VisibleServiceNumberPanel() As Boolean
        Get
            Return tdServiceNumber.Visible
        End Get
        Set(ByVal value As Boolean)
            tdServiceNumber.Visible = value
            tdAdoptionYear.Visible = value
        End Set
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        RequiredFieldValidator1.IsValid = True
        InitializeAjaxSettings()
        Initialize()
    End Sub

    Protected Sub uscResolutionSelect_AjaxRequest(ByVal sender As Object, ByVal e As Telerik.Web.UI.AjaxRequestEventArgs)
        Dim arguments As String() = Split(e.Argument, "|")
        If arguments(0) = Me.ClientID Then
            Dim reslParam As ResolutionParam = JsonConvert.DeserializeObject(Of ResolutionParam)(arguments(1))
            txtIdResolution.Text = reslParam.Id.ToString()
            If reslParam.Year.HasValue Then
                txtYear.Text = reslParam.Year.Value.ToString()
            End If
            If reslParam.Number.HasValue Then
                txtNumber.Text = reslParam.Number.Value.ToString()
            End If
            txtServiceNumber.Text = reslParam.ServiceNumber
            rblTipo.SelectedValue = reslParam.IdType.ToString()
            AjaxManager.ResponseScripts.Add("document.getElementById('" & btnSeleziona.ClientID & "').click();")
        End If
    End Sub

    ''' <summary>
    ''' Evento scatenato al click del pulsante "Seleziona": nel caso esista un protocollo con tale anno e numero valorizza
    ''' la proprietà SelectedProtocol
    ''' </summary>
    Private Sub btnSeleziona_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSeleziona.Click
        If VisibleYearNumberPanel AndAlso String.IsNullOrEmpty(txtNumber.Text) AndAlso String.IsNullOrEmpty(txtIdResolution.Text) Then
            RequiredFieldValidator1.IsValid = False
            Return
        End If
        If VisibleServiceNumberPanel Then
            If String.IsNullOrEmpty(txtServiceNumber.Text) Then
                RequiredFieldValidator1.IsValid = False
                Return
            End If
            txtIdResolution.Text = String.Empty

        End If
        BindResolution(rblTipo.SelectedValue)
        RaiseEvent ResolutionSelected(sender, e)
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSeleziona, uscResolutionPreview)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnCerca, ajaxPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSeleziona, ajaxPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, ajaxPanel)
        AddHandler AjaxManager.AjaxRequest, AddressOf uscResolutionSelect_AjaxRequest
    End Sub

    Private Sub Initialize()
        btnCerca.OnClientClick = String.Format("return {0}_OpenSearchWindow();", ID)
        If Not IsPostBack Then
            InitResolutionTypes()
        End If
    End Sub

    Public Sub InitResolutionTypes()
        rblTipo.Items.Clear()
        rblTipo.Items.Add(Facade.ResolutionTypeFacade.DeliberaCaption())
        rblTipo.Items(0).Value = ResolutionType.IdentifierDelibera
        rblTipo.Items(0).Selected = True
        rblTipo.Items.Add(Facade.ResolutionTypeFacade.DeterminaCaption())
        rblTipo.Items(1).Value = ResolutionType.IdentifierDetermina
    End Sub

    Public Function GetResolutionDescription(ByVal incompleteData As Boolean) As String
        Select Case True
            Case VisibleYearNumberPanel
                If Not String.IsNullOrEmpty(txtNumber.Text) Then
                    Return String.Format("{0} n. {1} ", rblTipo.SelectedItem.Text, ResolutionFacade.ReslFull(txtNumber.Text))
                Else
                    Return String.Format("{0} n. {1} ", rblTipo.SelectedItem.Text, ResolutionFacade.ReslFull(txtIdResolution.Text))
                End If
            Case VisibleServiceNumberPanel
                If incompleteData Then
                    Return String.Format("{0} n. {1} ", rblTipo.SelectedItem.Text, txtIdResolution.Text)
                Else
                    Return String.Format("{0} n. {1} ", rblTipo.SelectedItem.Text, txtServiceNumber.Text)
                End If
        End Select

        Return String.Empty
    End Function

    <Obsolete("Cosa è questa cosa oscena?")>
    Public Function CheckInputData(ByRef incompleteData As Boolean) As Boolean
        incompleteData = False
        'Verifica Dati per la ricerca
        If VisibleYearNumberPanel Then
            'verifico Anno e Numero
            If String.IsNullOrEmpty(txtYear.Text) Or String.IsNullOrEmpty(txtNumber.Text) Then
                'verifico IdResolution
                If String.IsNullOrEmpty(txtIdResolution.Text) Then
                    Return False
                End If
            End If
        Else
            'verifico ServiceNumber
            incompleteData = String.IsNullOrEmpty(txtServiceNumber.Text)
            If incompleteData And String.IsNullOrEmpty(txtIdResolution.Text) Then
                Return False
            End If
        End If

        Return True
    End Function

    ''' <summary> Visualizza anteprima Protocollo </summary>
    Public Sub ShowPreviewMode()
        tblSearch.Visible = False
        BindResolution(String.Empty)
    End Sub

    Private Sub BindResolution(ByVal type As String)

        SelectedResolution = Facade.ResolutionFacade.GetByIdOrAdoptionData(type, txtIdResolution.Text, InclusiveNumber, txtYear.Text)

        If SelectedResolution IsNot Nothing Then
            uscResolutionPreview.Visible = True
            uscResolutionPreview.CurrentResolution = SelectedResolution
            uscResolutionPreview.Show()

            txtYear.Text = SelectedResolution.Year.ToString()
            txtNumber.Text = SelectedResolution.Number.ToString()
            txtIdResolution.Text = SelectedResolution.Id.ToString()
            txtServiceNumber.Text = SelectedResolution.ServiceNumber
        Else
            uscResolutionPreview.Visible = False
        End If
    End Sub

#End Region

End Class