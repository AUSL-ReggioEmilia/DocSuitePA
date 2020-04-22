Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Web

Partial Public Class ReslVisualizzaStorico
    Inherits ReslBasePage

#Region " Fields "

    Dim _documentLinkCount As Integer = 0

#End Region

#Region " Properties "

    Public Property CurrentDocumentLinkCount() As Integer
        Get
            Return _documentLinkCount
        End Get
        Set(ByVal value As Integer)
            _documentLinkCount = value
        End Set
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Pratica.Attributes.Add("onclick", "return OpenWindowDocument();")
        WebUtils.ObjAttDisplayNone(SelPratica)
        WebUtils.ObjAttDisplayNone(btnSelPratica)

        If Not Page.IsPostBack Then
            If Not Page.IsCallback AndAlso Not CommonUtil.VerifyChkQueryString(Request.QueryString, True) Then
                Exit Sub
            End If
            Initialize()
        End If
    End Sub

    Private Sub CmdDocumentoClick(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDocumento.Click
        If CurrentResolution Is Nothing Then
            Exit Sub
        End If

        Dim idChain As Integer? = Nothing
        Select Case cmdDocumento.Text
            Case "Documento", "Doc. Esecutivo"
                idChain = CurrentResolution.File.IdResolutionFile
            Case "Doc. Pubblicato", "Doc. Adottato"
                idChain = CurrentResolution.File.IdAssumedProposal
        End Select

        Dim url As String = GetFileViewerScript(cmdDocumento.Text, "idResolutionFile")
        Response.Redirect(url)
    End Sub

    Private Sub cmdProposta_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdProposta.Click
        If CurrentResolution Is Nothing Then
            Exit Sub
        End If

        Dim url As String = GetFileViewerScript("Doc. Proposta", "IdProposalFile")
        Response.Redirect(url)
    End Sub

    Private Sub cmdAllegati_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdAllegati.Click
        If CurrentResolution Is Nothing Then
            Exit Sub
        End If

        Dim url As String = GetFileViewerScript("Allegati", "IdAttachements")
        Response.Redirect(url)
    End Sub

    Private Sub cmdFrontespizio_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdFrontespizio.Click
        If CurrentResolution Is Nothing Then
            Exit Sub
        End If

        Dim url As String = GetFileViewerScript("Frontespizio", "IdResolutionFile")
        Response.Redirect(url)
    End Sub

    Private Sub cmdOrganoControllo_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdOrganoControllo.Click
        If CurrentResolution Is Nothing Then
            Exit Sub
        End If

        Dim url As String = GetFileViewerScript("Risposta Organo Controllo", "IdControllerFile")
        Response.Redirect(url)
    End Sub

    Private Function GetFileViewerScript(ByVal description As String, ByVal field As String) As String
        Dim viewerPage As String = String.Format("{0}/viewers/FileResolutionViewer.aspx?", DocSuiteContext.Current.CurrentTenant.DSWUrl)
        Dim querystring As String = String.Format("idResolution={0}&field={2}&description={1}", CurrentResolution.Id, Server.UrlEncode(description), field)
        Dim url As String = String.Concat(viewerPage, CommonShared.AppendSecurityCheck(querystring))
        Return url
    End Function

    Private Sub btnSelPratica_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSelPratica.Click
        Dim s As String
        s = SelPratica.Text
        If Left(s, 5) = "Docm:" Then
            s = Mid$(s, 6)
            Dim v As Array = Split(s, "/")
            s = "../Docm/DocmVisualizza.aspx?" & CommonShared.AppendSecurityCheck("Year=" & v(0) & "&Number=" & v(1))
            SelPratica.Text = ""
            Response.Redirect(s)
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        'recupera l'atto
        If CurrentResolution IsNot Nothing Then
            'numero pratiche collegate all'atto
            CurrentDocumentLinkCount = Facade.ResolutionFacade.GetDocumentLinkedCount(CurrentResolution.Id)
        End If

        'verifica i diritti di visualizzazione
        If Not VerifyResolutionRights() Then
            Exit Sub
        End If

        'Titolo Pagina
        Title = Facade.ResolutionTypeFacade.GetDescription(CurrentResolution.Type) & " - Visualizzazione"

        'Inizializza lo userControl di visualizzazione
        uscResolution.CurrentResolution = CurrentResolution
        uscResolution.IsStorico = True

        uscResolution.VisibleNumber = True
        uscResolution.VisibleStatus = True
        uscResolution.VisibleObject = True
        uscResolution.VisibleComunication = True
        uscResolution.VisibleComunicationAssMgr = True
        uscResolution.VisibleComunicationAssMgrAlternative = True
        uscResolution.VisibleComunicationDestProp = True
        uscResolution.VisibleComunicationDestPropAlternative = True
        uscResolution.VisibleRoles = True
        uscResolution.VisibleOther = True

        'Scrittura Log
        Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RS)

        'Inizializza i Pulsanti
        SetDocumento()

        'Inizializza Pannello Icone
        InitializeIcons()

        'Mostro il controllo
        uscResolution.Show()
    End Sub

    Private Sub InitializeIcons()
        Dim cell As TableCell = TblButtons.Rows(0).Cells(0)

        'Icona tipologia Atto
        ReslVisualizza.AddIcon(cell, GetResolutionIcon(CurrentResolution), "")

        'Icona Pratica
        If CurrentDocumentLinkCount > 0 Then
            Select Case CurrentDocumentLinkCount
                Case 1
                    ReslVisualizza.AddIcon(cell, "../Comm/images/docsuite/pratica32.gif", "")
                Case Else
                    ReslVisualizza.AddIcon(cell, "../Comm/images/docsuite/pratiche32.gif", "")
            End Select
        Else
            Pratica.Visible = False
        End If
    End Sub

    Private Sub SetDocumento()
        cmdFrontespizio.Visible = False

        'Invio Mail
        MailFacade.RegisterOpenerMailWindow(cmdMail, MailFacade.CreateResolutionMailParameters(IdResolution, cmdMail.ID))
        'Imposta pulsanti
        Dim visibleRight As Boolean = CurrentResolutionRight.IsViewable
                Dim proposalMode As Boolean = CurrentResolutionRight.CanInsertInContainer
                '--Visualizzazione
                cmdProposta.Visible = proposalMode Or visibleRight
                cmdDocumento.Visible = visibleRight AndAlso CurrentResolution.File.IdResolutionFile.HasValue
                cmdAllegati.Visible = visibleRight
                cmdFrontespizio.Visible = visibleRight
                cmdOrganoControllo.Visible = visibleRight
                cmdMail.Visible = visibleRight
                '--Abilitazione
                cmdProposta.Enabled = CurrentResolution.File.IdProposalFile.HasValue
                Select Case True
                    Case CurrentResolution.File.IdResolutionFile.HasValue
                        Select Case CurrentResolution.EffectivenessDate.HasValue
                            Case True
                                cmdDocumento.Text = "Doc. Esecutivo"
                                cmdFrontespizio.Visible = False
                                cmdAllegati.Visible = False
                            Case False
                                cmdDocumento.Text = "Doc. Pubblicato"
                        End Select

                    Case CurrentResolution.File.IdAssumedProposal.HasValue
                        cmdDocumento.Text = "Doc. Adottato"
                    Case Else
                        cmdDocumento.Enabled = False
                End Select

        If visibleRight Then
            cmdFrontespizio.Enabled = CurrentResolution.File.IdResolutionFile.HasValue
            cmdAllegati.Enabled = CurrentResolution.File.IdAttachements.HasValue
            cmdOrganoControllo.Enabled = CurrentResolution.File.IdControllerFile.HasValue
            cmdMail.Visible = True
        End If
    End Sub

    Private Function VerifyResolutionRights() As Boolean
        If CurrentResolution Is Nothing Then
            AjaxAlert("Registrazione n. " & String.Format("{0:0000000}", IdResolution) & " Registrazione Inesistente")
            Return False
        End If
        If Not CurrentResolutionRight.IsViewable Then
            Dim err As String = "Non è possibile visualizzare il Documento richiesto. Verificare se si dispone di sufficienti autorizzazioni."
            AjaxAlert("Registrazione n. " & String.Format("{0:0000000}", IdResolution), err)
            Return False
        End If
        Return True
    End Function

    Private Function GetResolutionIcon(ByVal resolution As Resolution) As String
        If resolution.AdoptionDate.HasValue Then
            If resolution.PublishingDate.HasValue Then
                Return DefineIcon(resolution.Type, resolution.Status.Id, True)
            Else    'Tra l'adozione e la pubblicazione
                Return DefineIcon(resolution.Type, -10, True)
            End If
        Else
            Return DefineIcon(resolution.Type, resolution.Status.Id, True)
        End If
    End Function

    Public Function GetWindowDocumentPage() As String
        Return String.Format("../Docm/DocmSelezione.aspx?Titolo=Selezione Pratica&NomeCampoID=SelPratica&AddButton=btnSelPratica&Type=LR&Link={0}|", IdResolution)
    End Function

#End Region

End Class