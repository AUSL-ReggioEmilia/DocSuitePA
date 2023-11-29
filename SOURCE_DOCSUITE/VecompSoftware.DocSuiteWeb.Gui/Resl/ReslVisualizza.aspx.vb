Imports System.Collections.Generic
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Conservations
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Model.DocumentGenerator
Imports VecompSoftware.DocSuiteWeb.Model.DocumentGenerator.Parameters
Imports VecompSoftware.Helpers.ExtensionMethods
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

        SetResolutionGeneratorMetadata()
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

        AjaxManager.AjaxSettings.AddAjaxSetting(uscResolution.ResolutionWorkflow, resolutionBottomBar.TableDocuments)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscResolution.ResolutionWorkflow, uscResolution.AuslPcWebPublicationPanel)

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

        If ProtocolEnv.ConservationEnabled AndAlso uscResolution.CurrentConservation IsNot Nothing Then
            AddIcon(cell, ConservationHelper.StatusBigIcon(uscResolution.CurrentConservation.Status), ConservationHelper.StatusDescription(uscResolution.CurrentConservation.Status))
        End If

        'Inizializza il controllo
        Controller.Initialize()

        'Registro script di stampa
        Const script As String = "var hgt=document.all('divContent').style.height;" &
                                 "document.all('divContent').style.height='100%';" &
                                 "var wdt=document.all('divContent').style.width;" &
                                 "document.all('divContent').style.width='100%';" &
                                 "window.print();" &
                                 "document.all('divContent').style.height=hgt;" &
                                 "document.all('divContent').style.width=wdt;" &
                                 "return false;"
        Controller.RegisterPrintScript(script)

        'Mostra i pulsanti
        Controller.ShowButtons()
        'Mostro le informazioni
        Controller.Show()

        If ResolutionEnv.EnableFlushAnnexed AndAlso resolutionBottomBar.ButtonFlushAnnexed.Visible Then
            AddHandler resolutionBottomBar.ButtonFlushAnnexed.Click, AddressOf ButtonFlushAnnexed_ClickDelegate
        End If

    End Sub

    Private Function VerifyResolutionRights() As Boolean

        If CurrentResolution Is Nothing Then
            Throw New DocSuiteException("Errore verifica registrazione", String.Format("Registrazione [{0:0000000}] inesistente.", IdResolution))
        End If

        If Not CurrentResolutionRight.HasCurrentStepVisibilityRights Then
            Throw New DocSuiteException("Errore verifica registrazione",
                String.Format("Non è possibile visualizzare la registrazione [{0:0000000}]. Verificare se si dispone di sufficienti autorizzazioni.", IdResolution))
        End If

        If CurrentResolutionRight.IsExecutiveViewable Then
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

    Public Shared Sub AddIcon(ByVal c As TableCell, ByVal imageUrl As String, ByVal toolTip As String)
        Dim a As New Image
        a.ImageUrl = imageUrl
        a.ToolTip = toolTip
        c.Controls.Add(a)
        c.Controls.Add(WebHelper.BrControl)
    End Sub

    Private Sub SetResolutionGeneratorMetadata()
        Dim metadata As List(Of BaseDocumentGeneratorParameter) = New List(Of BaseDocumentGeneratorParameter)()
        AddDocumentGeneratorParameters(metadata, CurrentResolution)
        Dim script As String = String.Format("SetMetadataSessionStorage('{0}');", HttpUtility.JavaScriptStringEncode(JsonConvert.SerializeObject(metadata, DocSuiteContext.DefaultWebAPIJsonSerializerSettings)))
        AjaxManager.ResponseScripts.Add(script)
    End Sub

    Private Sub AddDocumentGeneratorParameters(metadata As List(Of BaseDocumentGeneratorParameter), resolution As Resolution)
        If resolution.Year.HasValue Then
            metadata.Add(New IntParameter("Year", resolution.Year.Value))
            metadata.Add(New StringParameter("Number", resolution.InclusiveNumber))
        End If

        If resolution.AdoptionDate.HasValue Then
            metadata.Add(New DateTimeParameter("AdoptionDate", resolution.AdoptionDate.Value))
        End If

        If resolution.ConfirmDate.HasValue Then
            metadata.Add(New DateTimeParameter("ConfirmDate", resolution.ConfirmDate.Value))
        End If

        If resolution.EffectivenessDate.HasValue Then
            metadata.Add(New DateTimeParameter("EffectivenessDate", resolution.EffectivenessDate.Value))
        End If

        If resolution.ProposeDate.HasValue Then
            metadata.Add(New DateTimeParameter("ProposeDate", resolution.ProposeDate.Value))
        End If

        If resolution.PublishingDate.HasValue Then
            metadata.Add(New DateTimeParameter("PublishingDate", resolution.PublishingDate.Value))
        End If

        If resolution.UltimaPaginaDate.HasValue Then
            metadata.Add(New DateTimeParameter("UltimaPaginaDate", resolution.UltimaPaginaDate.Value.DateTime))
        End If

        If resolution.WebPublicationDate.HasValue Then
            metadata.Add(New DateTimeParameter("WebPublicationDate", resolution.WebPublicationDate.Value))
        End If

        metadata.Add(New StringParameter("Object", Server.HtmlEncode(resolution.ResolutionObject)))
        metadata.Add(New StringParameter("Note", Server.HtmlEncode(resolution.Note)))
        metadata.Add(New BooleanParameter("Documents", True))
    End Sub
#End Region

End Class