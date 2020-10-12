Imports System.Collections.Generic
Imports System.Text
Imports Limilabs.Mail.Appointments
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Sharepoint
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Helpers.Web
Imports VecompSoftware.Services.Biblos.Models

Partial Public Class ReslVisualizza
    Inherits ReslBasePage
    Implements ISendMail

#Region " Fields "

    Dim _documentLinkCount As Integer = 0

    Private _reslController As IDisplayController

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

    Private Property Controller() As IDisplayController
        Get
            Return _reslController
        End Get
        Set(ByVal value As IDisplayController)
            _reslController = value
        End Set
    End Property

    Public ReadOnly Property SenderDescription() As String Implements ISendMail.SenderDescription
        Get
            Return CommonInstance.UserDescription
        End Get
    End Property

    Public ReadOnly Property SenderEmail() As String Implements ISendMail.SenderEmail
        Get
            Return CommonInstance.UserMail
        End Get
    End Property

    Public ReadOnly Property Recipients() As IList(Of ContactDTO) Implements ISendMail.Recipients
        Get
            Return New List(Of ContactDTO)()
        End Get
    End Property

    Public ReadOnly Property Documents() As IList(Of DocumentInfo) Implements ISendMail.Documents
        Get
            ' Nessun documento
            Return New List(Of DocumentInfo)()
        End Get
    End Property

    Public ReadOnly Property Subject() As String Implements ISendMail.Subject
        Get
            Return MailFacade.GetResolutionSubject(CurrentResolution)
        End Get
    End Property

    Public ReadOnly Property Body() As String Implements ISendMail.Body
        Get
            Return MailFacade.GetResolutionBody(CurrentResolution)
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        SetResponseNoCache()

        InitializeControls()
        InitializeAjax()

        If Not IsPostBack Then
            Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RS)
        End If

        uscResolution.Show()

        ' Verifico se devo aprire un file
        If Session("DocumentToOpen") IsNot Nothing Then
            If Not String.IsNullOrEmpty(Session("DocumentToOpen").ToString()) Then AjaxManager.ResponseScripts.Add(String.Format("window.open('{0}');", Session("DocumentToOpen")))
            Session.Remove("DocumentToOpen")
        End If
    End Sub

    Private Sub ReslVisualizza_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arguments As String() = Split(e.Argument, "|", 2)
        Select Case arguments(0).ToUpper()
            Case "DUPLICATE"
                Duplicate(arguments(1))
            Case "DELETE"
                Delete()
            Case "SELDOC"
                SelectDocument(arguments(1))
            Case "CANCEL"
                Response.Redirect("~/Resl/ReslRicerca.aspx?Type=Resl")
            Case "LASTPAGE"
                Controller.ShowButtons()
        End Select
    End Sub

    Protected Sub Workflow_Refresh(ByVal sender As Object, ByVal e As EventArgs)
        Controller.ShowButtons()
    End Sub

    Private Sub ButtonFlushAnnexed_ClickDelegate(ByVal sender As Object, ByVal e As EventArgs)
        Facade.ResolutionFacade.FlushAnnexed(CurrentResolution)
        resolutionBottomBar.ButtonFlushAnnexed.Visible = False
        MyBase.ResetCurrentResolutionState()
        AjaxAlert("Catena annessi svuotata correttamente")
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf ReslVisualizza_AjaxRequest
        AddHandler uscResolution.ResolutionWorkflow.AjaxRefresh, AddressOf Workflow_Refresh

        AjaxManager.AjaxSettings.AddAjaxSetting(resolutionBottomBar.ButtonPublishWeb, uscResolution.WebStateLabel)
        AjaxManager.AjaxSettings.AddAjaxSetting(resolutionBottomBar.ButtonPublishWeb, uscResolution.WebPublicationDateLabel)

        AjaxManager.AjaxSettings.AddAjaxSetting(resolutionBottomBar.ButtonRevokeWeb, uscResolution.WebStateLabel)
        AjaxManager.AjaxSettings.AddAjaxSetting(resolutionBottomBar.ButtonRevokeWeb, uscResolution.WebRevokeDateLabel)

        AjaxManager.AjaxSettings.AddAjaxSetting(uscResolution.ResolutionWorkflow, resolutionBottomBar.TableDocuments)

        AjaxManager.AjaxSettings.AddAjaxSetting(uscResolution.ResolutionWorkflow, uscResolution.AuslPcWebPublicationPanel)

        ' AjaxManager.AjaxSettings.AddAjaxSetting(resolutionBottomBar, resolutionBottomBar)

        If ResolutionEnv.EnableFlushAnnexed Then
            AjaxManager.AjaxSettings.AddAjaxSetting(resolutionBottomBar.ButtonFlushAnnexed, uscResolution.ResolutionWorkflow, MasterDocSuite.AjaxDefaultLoadingPanel)
        End If
    End Sub

    Private Sub InitializeControls()
        resolutionBottomBar.ReslId = IdResolution

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
        Title = String.Format("{0} - Visualizzazione", Facade.ResolutionTypeFacade.GetDescription(CurrentResolution.Type))
        'Inizializza Pannello Icone
        Dim cell As TableCell = TblButtons.Rows(0).Cells(0)

        'Icona tipologia Atto
        Dim status As Short
        If CurrentResolution.AdoptionDate.HasValue Then
            If CurrentResolution.PublishingDate.HasValue Then
                status = CurrentResolution.Status.Id
            Else
                'Tra l'adozione e la pubblicazione
                status = -10
            End If
        Else
            status = CurrentResolution.Status.Id
        End If
        AddIcon(cell, DefineIcon(CurrentResolution.Type, status, True), String.Format("{0} - {1}", Facade.ResolutionTypeFacade.GetDescription(CurrentResolution.Type), CurrentResolution.Status.Description))

        'Icona Pratica
        If CurrentDocumentLinkCount > 0 Then
            Select Case CurrentDocumentLinkCount
                Case 1
                    AddIcon(cell, "../Comm/images/docsuite/pratica32.gif", String.Empty)
                Case Else
                    AddIcon(cell, "../Comm/images/docsuite/pratiche32.gif", String.Empty)
            End Select
        End If

        If ProtocolEnv.DocumentSeriesEnabled AndAlso Not Facade.ResolutionDocumentSeriesItemFacade.GetDocumentSeriesItems(CurrentResolution).IsNullOrEmpty() Then
            AddIcon(cell, ImagePath.BigDocumentSeries, "Amministrazione Aperta")
        End If

        ' Icona Collaborazione
        If ProtocolEnv.IsCollaborationEnabled AndAlso ResolutionEnv.ShowCollaboration Then
            Dim collaboration As Collaboration = Facade.CollaborationFacade.GetByResolution(CurrentResolution)
            If collaboration IsNot Nothing Then
                AddIcon(cell, "../Comm/Images/DocSuite/Fascicoli32.gif", "Proviene da collaborazione")
            End If
        End If

        ' Icona Pubblicazione
        If ResolutionEnv.WebPublicationPrint AndAlso (CurrentResolution.WebPublicationDate.HasValue OrElse CurrentResolution.PublishingDate.HasValue) Then
            If Facade.WebPublicationFacade.HasResolutionPrivacyPublications(CurrentResolution) Then
                AddIcon(cell, ImagePath.BigWorldKey, "Atto pubblicato su Albo on-line con modalità Privacy")
            ElseIf Facade.WebPublicationFacade.HasResolutionPublications(CurrentResolution) Then
                AddIcon(cell, ImagePath.BigWorld, "Atto pubblicato su Albo on-line")
            End If
        End If

        'Inizializza lo userControl di visualizzazione
        uscResolution.CurrentResolution = CurrentResolution
        resolutionBottomBar.CurrentResolution = CurrentResolution
        Controller = ControllerFactory.CreateResolutionDisplayController(uscResolution, resolutionBottomBar)

        'Inizializza il controllo
        Controller.Initialize()

        'Registro script di stampa
        Const script As String = "var hgt=document.all('divContent').style.height;" & _
                                 "document.all('divContent').style.height='100%';" & _
                                 "var wdt=document.all('divContent').style.width;" & _
                                 "document.all('divContent').style.width='100%';" & _
                                 "window.print();" & _
                                 "document.all('divContent').style.height=hgt;" & _
                                 "document.all('divContent').style.width=wdt;" & _
                                 "return false;"
        Controller.RegisterPrintScript(script)

        'Mostra i pulsanti
        Controller.ShowButtons()

        'Mostro le informazioni
        Controller.Show()

        'Associo il click ai pulsanti di Pubblicazione
        If ResolutionEnv.WebPublishEnabled Then
            AddHandler resolutionBottomBar.ButtonPublishWeb.Click, AddressOf ButtonPublishWebClickDelegate
            AddHandler resolutionBottomBar.ButtonRevokeWeb.Click, AddressOf ButtonRevokeWebClickDelegate
        End If

        If ResolutionEnv.EnableFlushAnnexed AndAlso resolutionBottomBar.ButtonFlushAnnexed.Visible Then
            AddHandler resolutionBottomBar.ButtonFlushAnnexed.Click, AddressOf ButtonFlushAnnexed_ClickDelegate
        End If

    End Sub

    Private Function VerifyResolutionRights() As Boolean

        If CurrentResolution Is Nothing Then
            Throw New DocSuiteException("Errore verifica registrazione", String.Format("Registrazione [{0:0000000}] inesistente.", IdResolution))
        End If

        If CurrentResolutionRight.IsResolutionExecutive Then
            Return True
        End If

        If Not CurrentResolutionRight.IsPreviewable Then
            ' Controllo per visibilità post pubblicazione
            If DocSuiteContext.Current.ResolutionEnv.ShowAfterPubblication AndAlso CurrentResolution.PublishingDate.HasValue AndAlso CurrentResolution.PublishingDate <= DateTime.Now Then
                Return True
            End If

            Throw New DocSuiteException("Errore verifica registrazione",
                String.Format("Non è possibile visualizzare la registrazione [{0:0000000}]. Verificare se si dispone di sufficienti autorizzazioni.", IdResolution))
        End If

        'TODO: Togliere questo controllo
        ' AJG20091130: Verifica Settori utente:
        ' Con il seguente controllo verifico se i Settori di appartenenza dell'utente collegato gli permettono di vedere le Resolutions in stato "Proposta"
        ' L'utente può vedere la Resolution solo se almento un Settore a cui è assegnato è in comune con quelli dell'autore della proposta
        ' Il controllo può essere disabilitatò mettendo a zero la variabile "VerifyRoles"
        ' Rocca20110427: Verifica parametro GroupPropostaAtto, se verificato non esegue il controllo
        ' AJG20110523: Centralizzo il controllo per Proposta attiva
        If Not CurrentResolutionRight.IsProposalViewable() Then
            Throw New DocSuiteException("Errore verifica registrazione",
                String.Format("Non è possibile visualizzare la registrazione [{0:0000000}] in proposta. Verificare se si dispone di sufficienti autorizzazioni.", IdResolution))
        End If

        Return True
    End Function

    Private Sub Duplicate(ByVal checkSel As String)
        Dim s As String = "Type=Resl&Action=Duplicate&IdResolution=" & IdResolution & "&Check=" & checkSel
        'TODO: AUSL-Piacenza
        'If Resl.SqlParameterEnvConfiguration = "AUSL-PC" Then
        '    Response.Redirect("../Resl/PCReslInserimento.aspx?" & Comm.ChkGenera(Me, s))
        'End If
        Response.Redirect("../Resl/ReslInserimento.aspx?" & CommonShared.AppendSecurityCheck(s))
    End Sub

    Private Sub Delete()
        If Not Facade.ResolutionFacade.Delete(CurrentResolution) Then
            Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RC, 1)
            Throw New DocSuiteException("Elimina Proposta", "Non è possibile eliminare la proposta: " & CurrentResolution.IdFull)
        Else
            Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RC)
            Response.Redirect("../Resl/ReslRicerca.aspx?Type=Resl")
        End If
    End Sub

    Private Sub SelectDocument(ByVal selDocument As String)
        Dim s As String = selDocument
        If Left(s, 5) = "Docm:" Then
            s = Mid$(s, 6)
            Dim v As Array = Split(s, "/")
            s = "../Docm/DocmVisualizza.aspx?" & CommonShared.AppendSecurityCheck(String.Format("Type=Docm&Year={0}&Number={1}", v(0), v(1)))
            Response.Redirect(s)
        End If
    End Sub

    Public Function GetWindowDeletePage() As String
        Return "../Resl/ReslElimina.aspx?Titolo=Elimina Proposta&" & CommonShared.AppendSecurityCheck("Type=Resl&Action=Ann&IdResolution=" & IdResolution)
    End Function

    Public Function GetWindowSelDocument() As String
        Return String.Format("../Docm/DocmSelezione.aspx?Type=LR&Link={0}|", IdResolution)
    End Function

    ''' <summary> Metodo per la pubblicazione del frontespizio. </summary>
    Protected Overridable Sub ButtonPublishWebClickDelegate(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim strXmlDoc As String = String.Empty
            Dim ResWPFacade As ResolutionWPFacade = New ResolutionWPFacade()
            Dim WebPublDate As DateTime = DateTime.Today

            'Allo stato attuale questo bottone può essere richiamato solo in configurazione ASMN
            'quindi non ha senso passare l'XML del documento
            'strXmlDoc = ResWPFacade.GetXMLSPFrontespizio(byteData, sign, CurrentResolution.Id)

            Dim retVal As ReturnValue
            Try
                retVal = ServiceSHP.InsertInPublicationArea(CurrentResolution.Container.Name, CurrentResolution.Year, CurrentResolution.Number.Value.ToString(), CurrentResolution.AdoptionDate.Value, CurrentResolution.ResolutionObject, WebPublDate, WebPublDate, WebPublDate.AddDays(15), CurrentResolution.Type.Description, ResWPFacade.GetXmlOther(CurrentResolution), strXmlDoc)
            Catch ex As Exception
                Throw New DocSuiteException("Errore pubblicazione", "Impossibile pubblicare con Sharepoint.", ex)
            End Try

            ' Imposto i valori della pubblicazione
            CurrentResolution.WebSPGuid = retVal.Guid
            CurrentResolution.WebState = Resolution.WebStateEnum.Published
            CurrentResolution.WebPublicationDate = WebPublDate
            Facade.ResolutionFacade.Save(CurrentResolution)
            Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.WP, "File pubblicato correttamente")

            'Mostra i pulsanti
            Controller.ShowButtons()
            uscResolution.Show()

            AjaxAlert(CurrentResolution.Type.Description + " pubblicato")

        Catch ex As Exception
            FileLogger.Error(LoggerName, "Errore pubblicazione Web.", ex)
            AjaxAlert(ex.Message)
            Controller.ShowButtons()
            uscResolution.Show()
        End Try

    End Sub

    Protected Overridable Sub ButtonRevokeWebClickDelegate(ByVal sender As System.Object, ByVal e As EventArgs)
        Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.WP, "Inizio Pubblicazione Internet")
        Try
            Dim intestazione As New StringBuilder("<Label>")
            intestazione.Append("<Text>")
            intestazione.AppendFormat("{0}: Ritiro Albo {1:dd/MM/yyyy}", Facade.ResolutionFacade.SqlResolutionGetNumber(CurrentResolution.Id, , , , True), CurrentResolution.PublishingDate)
            intestazione.Append("</Text>")
            intestazione.Append("<Footer></Footer>")
            intestazione.Append("<Font Face=""Arial"" Size=""12"" Style=""Bold,Italic"" />")
            intestazione.Append("</Label>")

            Dim strXmlDoc As String = String.Empty
            Dim resWpFacade As ResolutionWPFacade = New ResolutionWPFacade()
            Dim webRewkDate As DateTime = DateTime.Now

            Try
                ServiceSHP.InsertInRetireArea(CurrentResolution.Container.Name, CurrentResolution.Year, CurrentResolution.Number.Value.ToString(), CurrentResolution.AdoptionDate.Value, CurrentResolution.ResolutionObject, DateTime.Now, CurrentResolution.WebPublicationDate.Value, webRewkDate, CurrentResolution.Type.Description, resWpFacade.GetXmlOther(CurrentResolution), strXmlDoc, CurrentResolution.WebSPGuid)
            Catch ex As Exception
                Throw New DocSuiteException("Errore revoca", "Impossibile inserire su Sharepoint.", ex)
            End Try

            ' Imposto i valori della pubblicazione
            CurrentResolution.WebState = Resolution.WebStateEnum.Revoked
            CurrentResolution.WebRevokeDate = webRewkDate

            Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.WR, "File ritirato correttamente")

            AjaxAlert(CurrentResolution.Type.Description & " ritirato")
        Catch ex As Exception
            FileLogger.Error(LoggerName, "Errore revoca web.", ex)
            AjaxAlert(ex.Message)
        End Try

        Controller.ShowButtons()
        uscResolution.Show()
    End Sub

    Public Shared Sub AddIcon(ByVal c As TableCell, ByVal imageUrl As String, ByVal toolTip As String)
        Dim a As New Image
        a.ImageUrl = imageUrl
        a.ToolTip = toolTip
        c.Controls.Add(a)
        c.Controls.Add(WebHelper.BrControl)
    End Sub

#End Region

End Class