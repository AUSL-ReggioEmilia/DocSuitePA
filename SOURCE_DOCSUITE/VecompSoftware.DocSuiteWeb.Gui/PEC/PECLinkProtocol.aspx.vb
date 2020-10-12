Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI

Public Class PECLinkProtocol
    Inherits PECBasePage

#Region "Properties"
    Public ReadOnly Property CurrentProtocol As Protocol
        Get
            Return protocolPreview.CurrentProtocol
        End Get
    End Property
#End Region

#Region "Events"
    Protected Sub PECLinkProtocol_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arg As String = e.Argument
        Select Case arg
            Case "selectProtocol"
                BtnSeleziona_Click(sender, e)
        End Select
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub BtnSeleziona_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim protocolYear As Short
        Dim protocolNumber As Integer
        If Not Short.TryParse(txtYear.Text, protocolYear) OrElse Not Integer.TryParse(txtNumber.Text, protocolNumber) Then
            AjaxAlert(String.Format("Protocollo [{0}/{1}] errore in validazione dei dati inseriti.", txtYear.Text, txtNumber.Text))
            Exit Sub
        End If

        Dim selectedProtocol As Protocol = Facade.ProtocolFacade.GetById(protocolYear, protocolNumber, False)
        If selectedProtocol Is Nothing Then
            AjaxAlert(String.Format("Protocollo [{0}/{1}] non trovato.", protocolYear, protocolNumber))
            Exit Sub
        End If

        ' L'utente non ha i diritti di modifica sul protocollo selezionato
        If Not New ProtocolRights(selectedProtocol).IsEditable Then
            AjaxAlert(String.Format("Protocollo [{0}]{1}Mancano i diritti necessari", ProtocolFacade.ProtocolFullNumber(protocolYear, protocolNumber), Environment.NewLine))
            Exit Sub
        End If

        protocolPreview.CurrentProtocol = selectedProtocol
        protocolPreview.Visible = True
        protocolPreview.Initialize()
        txtYear.Text = selectedProtocol.Year.ToString()
        txtNumber.Text = selectedProtocol.Number.ToString()
    End Sub

    Private Sub BtnConfirm_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConfirm.Click
        If CurrentProtocol Is Nothing Then
            AjaxAlert("Nessun protocollo selezionato per il collegamento")
            Exit Sub
        End If

        Facade.PECMailFacade.LinkToProtocol(CurrentPecMail, CurrentProtocol)

        Dim parameters As String = String.Format("Type=Prot&UniqueId={0}", CurrentProtocol.Id)
        Response.Redirect(String.Concat("~/Prot/ProtVisualizza.aspx?", CommonShared.AppendSecurityCheck(parameters)), True)
    End Sub

#End Region

#Region "Methods"
    Private Sub Initialize()
        Title = String.Format("{0} - Collega", PecLabel)        
        pecInfo.PecLabel = PecLabel.ToLower()

        btnCancel.PostBackUrl = PreviousPageUrl

        If CurrentPecMail Is Nothing Then
            Throw New DocSuiteException("Errore in collegamento PEC", "Nessuna PEC da gestire.")
        End If
        pecInfo.PECMail = CurrentPecMail
        pecInfo.BindData(CurrentPecMail)

        protocolPreview.Visible = False
    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlPageContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSeleziona, protocolPreview, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConfirm, pnlPageContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        AddHandler AjaxManager.AjaxRequest, AddressOf PECLinkProtocol_AjaxRequest
    End Sub
#End Region

End Class