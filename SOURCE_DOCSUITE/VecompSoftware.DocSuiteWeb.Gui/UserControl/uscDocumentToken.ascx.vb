Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class uscDocumentToken
    Inherits DocSuite2008BaseControl

#Region " Fields "

    Private _documentTabTokenFacade As DocumentTabTokenFacade

#End Region

#Region " Properties "

    Public Property PanelTipologiaRichiestaVisible() As Boolean
        Get
            Return tblTipologiaRichiesta.Visible
        End Get
        Set(ByVal value As Boolean)
            tblTipologiaRichiesta.Visible = value
        End Set
    End Property

    Public Property StepVisible() As Boolean
        Get
            Return tblRowStep.Visible
        End Get
        Set(ByVal value As Boolean)
            tblRowStep.Visible = value
        End Set
    End Property

    Public Property TextStep() As String
        Get
            Return txtStep.Text
        End Get
        Set(ByVal value As String)
            txtStep.Text = value
        End Set
    End Property

    Public Property TipologiaVisible() As Boolean
        Get
            Return tblRowType.Visible
        End Get
        Set(ByVal value As Boolean)
            tblRowStep.Visible = value
        End Set
    End Property

    Public Property TextTokenType() As String
        Get
            Return txtTokenType.Text
        End Get
        Set(ByVal value As String)
            txtTokenType.Text = value
        End Set
    End Property

    Public Property TextTokenName() As String
        Get
            Return txtTokenName.Text
        End Get
        Set(ByVal value As String)
            txtTokenName.Text = value
        End Set
    End Property

    Public Property RestituzioneVisible() As Boolean
        Get
            Return tblRowReturn.Visible
        End Get
        Set(ByVal value As Boolean)
            tblRowReturn.Visible = value
        End Set
    End Property

    Public Property PanelMovimentazioneRichiestaVisible() As Boolean
        Get
            Return tblMovRichiesta.Visible
        End Get
        Set(ByVal value As Boolean)
            tblMovRichiesta.Visible = value
        End Set
    End Property

    Public ReadOnly Property ControlSettoreDestinatario() As uscSettori
        Get
            Return uscDestinatarioMovimentazione
        End Get
    End Property

    Public ReadOnly Property ControlSettoreMittente() As uscSettori
        Get
            Return uscMittenteMovimentazione
        End Get
    End Property

    Public Property PanelSettoriCCVisible() As Boolean
        Get
            Return tblSettoriCC.Visible
        End Get
        Set(ByVal value As Boolean)
            tblSettoriCC.Visible = value
        End Set
    End Property

    Public ReadOnly Property ControlSettoreCC() As uscSettori
        Get
            Return uscSettoriCC
        End Get
    End Property

    Public Property PanelDatiRichiestaVisible() As Boolean
        Get
            Return uscRequestDocument.Visible
        End Get
        Set(ByVal value As Boolean)
            uscRequestDocument.Visible = value
        End Set
    End Property

    Public ReadOnly Property ControlDatiRichiesta() As uscDocumentDati
        Get
            Return uscRequestDocument
        End Get
    End Property

    Private ReadOnly Property DocumentTabTokenFacade() As DocumentTabTokenFacade
        Get
            If _documentTabTokenFacade Is Nothing Then
                _documentTabTokenFacade = New DocumentTabTokenFacade()
            End If
            Return _documentTabTokenFacade
        End Get
    End Property

#End Region
    
#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjaxSettings()
    End Sub

    Private Sub rblRestituzione_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles rblRestituzione.SelectedIndexChanged
        SetRestituzione()
    End Sub

    Private Sub uscDestinatarioMovimentazione_RoleRemoved(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs) Handles uscDestinatarioMovimentazione.RoleRemoved
        SetRblRestituzione()
    End Sub

    Private Sub uscDestinatarioMovimentazione_RolesAdded(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs) Handles uscDestinatarioMovimentazione.RolesAdded
        SetRblRestituzione()
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjaxSettings()
        If (RestituzioneVisible) Then
            If (TipologiaVisible) Then
                AjaxManager.AjaxSettings.AddAjaxSetting(rblRestituzione, txtTokenType)
                AjaxManager.AjaxSettings.AddAjaxSetting(uscDestinatarioMovimentazione, txtTokenType)
                AjaxManager.AjaxSettings.AddAjaxSetting(rblRestituzione, txtTokenName)
                AjaxManager.AjaxSettings.AddAjaxSetting(uscDestinatarioMovimentazione, txtTokenName)
            End If
            AjaxManager.AjaxSettings.AddAjaxSetting(uscDestinatarioMovimentazione, rblRestituzione)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, rblRestituzione)
        End If
    End Sub

    ''' <summary> Imposta le etichette di Restituzione per la Tipologia richiesta </summary>
    Private Sub SetRestituzione()
        Select Case rblRestituzione.SelectedValue
            Case "Si" : txtTokenType.Text = "RR"
            Case "No" : txtTokenType.Text = "RC"
        End Select
        Dim _docTabToken As DocumentTabToken = DocumentTabTokenFacade.GetById(txtTokenType.Text)
        If _docTabToken IsNot Nothing Then
            txtTokenName.Text = _docTabToken.Description
        End If
    End Sub

    ''' <summary> Imposta le etichette di Restituzione per la Tipologia richiesta in base al numero di Settori aggiunti </summary>
    Private Sub SetRblRestituzione()
        If uscDestinatarioMovimentazione.Count > 1 Then
            rblRestituzione.SelectedValue = "Si"
            rblRestituzione.Enabled = False
        Else
            rblRestituzione.SelectedValue = "No"
            rblRestituzione.Enabled = True
        End If
        SetRestituzione()
    End Sub

#End Region
    
End Class