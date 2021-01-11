Imports System.Text
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports Newtonsoft.Json
Imports System.Web
Imports System.Collections.Generic
Imports System.Linq

Partial Public Class uscContattiSelText
    Inherits DocSuite2008BaseControl

#Region " Fields "

    Private _isMittDest As Boolean
    Private _forceAddressBook As Boolean
    Private _maxLunghezzaTesto As Integer?
    Private Const SEL_MITTENTE_URL_FORMAT As String = "../UserControl/CommonSelMittDest.aspx?{0}"
    Private Const SEL_CONTACT_RUBRICA_FORMAT As String = "../UserControl/CommonSelContactRubrica.aspx?{0}"
    Private Const SEL_CONTACT_DOMAIN_FORMAT As String = "../UserControl/CommonSelContactDomain.aspx?Type={0}&ConfermaNuovoVisible=False&ParentID={1}"
    Private Const OPEN_WINDOW As String = "return {0}_OpenWindow('{1}', 'windowSelContact', {0}{2});"

#End Region

#Region " Properties "

    ''' <summary> Contatto con cui inizializzare il controllo. </summary>
    Public Property DataSource() As String
        Get
            Return txtContact.Text
        End Get
        Set(ByVal value As String)
            txtContact.Text = value
        End Set
    End Property

    ''' <summary> Indica se sono visibili i pulsanti sul controllo. </summary>
    Public Property ButtonsVisible() As Boolean
        Get
            Return pnlButtons.Visible
        End Get
        Set(ByVal value As Boolean)
            pnlButtons.Visible = value
        End Set
    End Property

    ''' <summary> Indica se il controllo deve gestire la ricerca dei Mittenti/Destinatari. </summary>
    Public Property IsMittDest() As Boolean
        Get
            Return _isMittDest
        End Get
        Set(ByVal value As Boolean)
            _isMittDest = value
        End Set
    End Property

    Public Property TextBoxWidth() As Unit
        Get
            Return txtContact.Width
        End Get
        Set(ByVal value As Unit)
            txtContact.Width = value
            If value.Type = UnitType.Pixel Then
                tblContactText.Width = Unit.Pixel(CType(value.Value + 50, Integer)).ToString()
            End If
        End Set
    End Property

    ''' <summary> Forza lo usercontrol ad utilizzare la rubrica anche se attivo ADAM o AD. </summary>
    Public Property ForceAddressBook() As Boolean
        Get
            Return _forceAddressBook
        End Get
        Set(ByVal value As Boolean)
            _forceAddressBook = value
        End Set
    End Property

    ''' <summary> Id del contatto padre aperto di default. </summary>
    Public Property ContactRoot() As String
        Get
            Return CType(ViewState("_contactRoot"), String)
        End Get
        Set(ByVal value As String)
            ViewState("_contactRoot") = value
        End Set
    End Property

    Public Property MaxLunghezzaTesto As Integer?
        Get
            Return _maxLunghezzaTesto
        End Get
        Set(ByVal value As Integer?)
            _maxLunghezzaTesto = value
        End Set
    End Property

    Public Property [ReadOnly] As Boolean
        Get
            Return Not txtContact.Enabled
        End Get
        Set(value As Boolean)
            txtContact.Enabled = Not value
            btnSelContact.Enabled = Not value
            btnDelContact.Enabled = Not value
        End Set
    End Property

    ''' <summary> Indica se è visibile la selezione dei contatti da ActiveDirectory. </summary>
    Public Property ButtonADContactVisible() As Boolean
        Get
            Return btnADContact.Visible
        End Get
        Set(ByVal value As Boolean)
            btnADContact.Visible = value AndAlso DocSuiteContext.Current.ProtocolEnv.AbilitazioneRubricaDomain
        End Set
    End Property

    ''' <summary> Indica se è visibile la selezione dei contatti da Rubrica. </summary>
    Public Property ButtonSelContactVisible() As Boolean
        Get
            Return btnSelContact.Visible
        End Get
        Set(ByVal value As Boolean)
            btnSelContact.Visible = value
        End Set
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not Page.IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub Initialize()
        ' Massima lunghezza contatto
        If MaxLunghezzaTesto.HasValue Then
            txtContact.MaxLength = MaxLunghezzaTesto.Value
        End If

        Dim url As String = String.Empty
        Dim closeFunction As String = String.Empty
        If IsMittDest Then
            url = String.Format(SEL_MITTENTE_URL_FORMAT, GetWindowParameters())
            closeFunction = "_CloseMittDest"
        Else
            url = String.Format(SEL_CONTACT_RUBRICA_FORMAT, GetWindowParameters())
            closeFunction = "_OnClose"
        End If
        btnSelContact.OnClientClick = String.Format(OPEN_WINDOW, ID, url, closeFunction)

        Dim selAdContactUrl As String = String.Format(SEL_CONTACT_DOMAIN_FORMAT, BasePage.Type, ID)
        btnADContact.OnClientClick = String.Format(OPEN_WINDOW, ID, selAdContactUrl, "_CloseDomain")
    End Sub

    Private Sub btnDelContact_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs) Handles btnDelContact.Click
        txtContact.Text = ""
    End Sub

    Protected Sub uscContattiSelText_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arguments As String() = e.Argument.Split("|"c)
        If Not arguments(0).Eq(ClientID) Then
            Exit Sub
        End If

        Dim contact As Contact
        Select Case arguments(1)
            Case "ADAM"
                contact = JsonConvert.DeserializeObject(Of Contact)(arguments(2))
            Case "ContactAD"
                Dim localArg As String = HttpUtility.HtmlDecode(arguments(2))
                contact = JsonConvert.DeserializeObject(Of Contact)(localArg)
            Case Else
                Dim idContact As Integer = Integer.Parse(arguments(2))
                contact = Facade.ContactFacade.GetById(idContact)
        End Select
        If contact IsNot Nothing Then
            txtContact.Text = Replace(contact.Description, "|", " ")
        Else
            BasePage.AjaxAlert("Contatto non trovato. Contattare l'assistenza.")
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        If Not Visible OrElse [ReadOnly] Then
            Exit Sub
        End If

        AddHandler AjaxManager.AjaxRequest, AddressOf uscContattiSelText_AjaxRequest

        AjaxManager.AjaxSettings.AddAjaxSetting(btnSelContact, txtContact)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnDelContact, txtContact)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnADContact, txtContact)

        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, txtContact)
    End Sub

    Private Function GetWindowParameters() As String
        Dim parameters As New StringBuilder
        If Not String.IsNullOrEmpty(Type) Then
            parameters.AppendFormat("&Type={0}", Type)
        End If
        If Not String.IsNullOrEmpty(ContactRoot) Then
            parameters.AppendFormat("&ContactRoot={0}", ContactRoot)
        End If
        parameters.AppendFormat("&ParentID={0}", ID)

        If parameters.Length = 0 Then
            Return String.Empty
        End If

        Return parameters.Remove(0, 1).ToString()
    End Function

    ''' <summary> Restituisce il contatto selezionato, Nothing altrimenti. </summary>
    Public Function GetContactText() As String
        Return txtContact.Text
    End Function

#End Region

End Class