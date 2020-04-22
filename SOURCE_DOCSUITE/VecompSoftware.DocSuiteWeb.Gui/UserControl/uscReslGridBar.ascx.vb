Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers
Imports Newtonsoft.Json
Imports System.Web

Partial Public Class uscReslGridBar
    Inherits BaseGridBar

    Public Structure WebDocument
        Public Resolution As Resolution
        Public Description As String
        Public Privacy As Boolean
        Public DocumentsIds As List(Of Guid)
    End Structure

    Public Class WebPublishEventError
        Inherits EventArgs
        Public Exception As Exception

        Public Sub New(ByVal e As Exception)
            Exception = e
        End Sub
    End Class

#Region " Fields "

    Private Const ADOTTA As String = "Adotta"
    Private Const AVVENUTA_ADOZIONE As String = "Trasmissione Avvenuta Adozione"
    Private Const TRASMISSIONE_OC As String = "Trasm. OC"
    Private Const PUBBLICAZIONE As String = "Pubblicazione"
    Private Const ESECUTIVITA As String = "Esecutività"
    Private Const TRASMISSIONE_ESECUTIVITA As String = "Trasmissione Esecutività"
    Private Const TRASMISSIONE_CS_FIRMA_DIGITALE As String = "Trasm. Collegio Sindacale Firma Digitale"
    Private Const STAMPA_ULTIMA_PAGINA As String = "Stampa Ultima Pagina"
    Private Const DA_AFFARI_GENERALI As String = "Avvia raccolta firme"
#End Region

#Region " Properties "

    Public ReadOnly Property AddButton() As PromptClickOnceButton
        Get
            Return btnAdd
        End Get
    End Property

    Public ReadOnly Property RemoveButton() As PromptClickOnceButton
        Get
            Return btnRemove
        End Get
    End Property

    Public ReadOnly Property ValidaButton() As PromptClickOnceButton
        Get
            Return btnValida
        End Get
    End Property


    Public Property VisibleWorkFlowPanel() As Boolean
        Get
            Return pnlGridBarWorkflow.Visible
        End Get
        Set(ByVal value As Boolean)
            pnlGridBarWorkflow.Visible = value
        End Set
    End Property

    Public Property VisibleRegionPanel() As Boolean
        Get
            Return pnlGridBarRegion.Visible
        End Get
        Set(ByVal value As Boolean)
            pnlGridBarRegion.Visible = value
        End Set
    End Property

    Public Property VisibleDefaultPanel() As Boolean
        Get
            Return pnlGridBar.Visible
        End Get
        Set(ByVal value As Boolean)
            pnlGridBar.Visible = value
        End Set
    End Property

    Private ReadOnly Property MyStep() As ResolutionGridBarController.BarWorkflowStep
        Get
            Return Session("CurrentStep")
        End Get
    End Property

    Private ReadOnly Property Tipologia() As Short
        Get
            Return Session("CurrentTipo")
        End Get
    End Property

    Private ReadOnly Property PublishingDate() As Date?
        Get
            Return Session("PublishingDate")
        End Get
    End Property

    Private ReadOnly Property CollegioWarningDate() As Date?
        Get
            Return Session("CollegioWarningDate")
        End Get
    End Property

    Private ReadOnly Property TextProtocollo() As String
        Get
            Return Session("TextProtocollo")
        End Get
    End Property

    Public ReadOnly Property PubblicaWebButton() As Button
        Get
            Return btnPubblicaWeb
        End Get
    End Property

    Private ReadOnly Property CheckOmissis As Boolean
        Get
            Dim checked As Boolean? = DirectCast(Session("CheckOmissis"), Boolean)
            If checked.HasValue Then
                Return checked.Value
            End If
            Return False
        End Get
    End Property

    Public Property CurrentResolutionDocuments As IList(Of PublicationResolutionDocumentModel)
        Get
            Return TryCast(Session("ResolutionDocuments"), IList(Of PublicationResolutionDocumentModel))
        End Get
        Set(value As IList(Of PublicationResolutionDocumentModel))
            If value Is Nothing Then
                Session.Remove("ResolutionDocuments")
            Else
                Session("ResolutionDocuments") = value
            End If
        End Set
    End Property

#End Region

#Region "BaseGrid Implementation: Properties"

    Public Overrides ReadOnly Property DeselectButton() As System.Web.UI.WebControls.Button
        Get
            Return btnDeselectAll
        End Get
    End Property

    Public Overrides ReadOnly Property PrintButton() As System.Web.UI.WebControls.Button
        Get
            Return btnStampa
        End Get
    End Property

    Public Overrides ReadOnly Property DocumentsButton() As Button
        Get
            Return btnDocuments
        End Get
    End Property

    Public Overrides ReadOnly Property SelectButton() As System.Web.UI.WebControls.Button
        Get
            Return btnSelectAll
        End Get
    End Property

    Public Overrides ReadOnly Property SetReadButton() As System.Web.UI.WebControls.Button
        Get
            Return btnSetRead
        End Get
    End Property

    Public Overrides ReadOnly Property LeftPanel() As System.Web.UI.WebControls.Panel
        Get
            Return pnlGridBarWorkflow
        End Get
    End Property

    Public Overrides ReadOnly Property MiddlePanel() As System.Web.UI.WebControls.Panel
        Get
            Return pnlGridBarRegion
        End Get
    End Property

    Public Overrides ReadOnly Property RightPanel() As System.Web.UI.WebControls.Panel
        Get
            Return pnlGridBar
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Initialize()
    End Sub

    Private Sub btnPubblicaWeb_Click(ByVal sender As Object, ByVal e As EventArgs)

    End Sub

    Private Sub btnShowRegion_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim values As New Dictionary(Of String, Object)
        For Each item As GridDataItem In _grid.MasterTableView.Items
            If item.ItemType = GridItemType.Item Or item.ItemType = GridItemType.AlternatingItem Then
                item.ExtractValues(values)
                If values("OCRegion") IsNot Nothing Then
                    item.Visible = CType(values("OCRegion"), Boolean) = True
                End If
            End If
        Next
    End Sub

    Private Sub btnShowAll_Click(ByVal sender As Object, ByVal e As EventArgs)
        For Each item As GridDataItem In _grid.MasterTableView.Items
            If item.ItemType = GridItemType.Item Or item.ItemType = GridItemType.AlternatingItem Then
                item.Visible = True
            End If
        Next
    End Sub
    Private Sub BtnDocumentsClick(ByVal sender As Object, ByVal e As EventArgs)
        Dim selection As New List(Of Integer)
        Dim currentResolution As Resolution
        Dim currentActiveTabWorkflow As TabWorkflow
        Dim currentResolutionRights As ResolutionRights
        ' Registro il log di visualizzazione dei documenti
        For Each idResolution As Integer In GetSelectedItems()
            currentResolution = Facade.ResolutionFacade.GetById(idResolution)
            currentActiveTabWorkflow = Facade.TabWorkflowFacade.GetActive(currentResolution)
            currentResolutionRights = New ResolutionRights(currentResolution)
            If currentResolutionRights.IsDocumentViewable(currentActiveTabWorkflow) OrElse (ResolutionEnv.ShowExecutiveDocumentEnabled AndAlso currentResolution.EffectivenessDate.HasValue) Then
                Facade.ResolutionLogFacade.Insert(currentResolution, ResolutionLogType.RD, "Visualizzazione documento da MultiCatena")
                selection.Add(idResolution)
            End If
        Next

        If selection.IsNullOrEmpty() Then
            BasePage.AjaxAlert("Nessun atto selezionato", False)
            Return
        End If

        Dim serialized As String = JsonConvert.SerializeObject(selection)
        Dim encoded As String = HttpUtility.UrlEncode(serialized)
        Dim redirectUrl As String = "~/viewers/ResolutionViewer.aspx?multiple=true&keys={0}&documents=true&attachments=true&annexes=true&documentsomissis=true&attachmentsomissis=true&previous=conditional"
        redirectUrl = String.Format(redirectUrl, encoded)
        Response.Redirect(redirectUrl)
    End Sub

    Private Sub BtnWorkflowClick(ByVal sender As Object, ByVal e As EventArgs)
        Dim sIDs As String = ListToCSV(GetSelectedItems())
        Dim sUrl As String = "../Resl/ReslFlussoElenco.aspx?"
        Dim sParteComune As String = "Type=Resl&Selezione=" & sIDs & "&Tipologia=" & Tipologia
        Dim s As String
        Dim script As String
        Dim list As New ArrayList

        If String.IsNullOrEmpty(sIDs) Then
            BasePage.AjaxAlert("Nessun elemento selezionato", False)
            Exit Sub
        End If

        Select Case MyStep
            Case ResolutionGridBarController.BarWorkflowStep.Adotta
                s = CommonShared.AppendSecurityCheck("Action=Adozione&" & sParteComune)
                script = "return " & ID & "_OpenWindow('windowFlussoElenco','Adozione','" & sUrl + s & "',700,500);"

                If (CheckRegioneSelected(list)) Then
                    BasePage.AjaxAlertConfirm("Selezionati atti con richiesta di approvazione dalla regione con atti senza tale richiesta, Continuare?", script, Nothing)
                Else
                    AjaxManager.ResponseScripts.Add(script)
                    'Page.ClientScript.RegisterStartupScript(Page.GetType, "Adozione", "<SCRIPT language='javascript'>" & script & "</SCRIPT>")
                End If

            Case ResolutionGridBarController.BarWorkflowStep.AvvenutaAdozione
                s = CommonShared.AppendSecurityCheck("Action=TrasmAvvenutaAdozione&" & sParteComune)
                script = "return " & ID & "_OpenWindow('windowFlussoElenco','AvvenutaAdozione','" & sUrl + s & "',700,500);"

                If (CheckRegioneSelected(list)) Then
                    BasePage.AjaxAlertConfirm("Selezionati atti con richiesta di approvazione dalla regione con atti senza tale richiesta, Continuare?", script, Nothing)
                Else
                    AjaxManager.ResponseScripts.Add(script)
                End If

            Case ResolutionGridBarController.BarWorkflowStep.Esecutiva
                If Tipologia = ResolutionType.IdentifierDetermina Then
                    If String.IsNullOrEmpty(sIDs) Then
                        BasePage.AjaxAlert("Nessun elemento selezionato.", False)
                        Exit Sub
                    End If
                End If

                s = CommonShared.AppendSecurityCheck("Action=Esecutivita&" & sParteComune & "&PublishingDate=" & String.Format("{0:yyyyMMdd}", PublishingDate.Value))
                script = "return " & ID & "_OpenWindow('windowFlussoElenco','Esecutività','" & sUrl + s & "',700,500);"

                If (CheckRegioneSelected(list)) Then
                    BasePage.AjaxAlertConfirm("Selezionati atti con richiesta di approvazione dalla regione con atti senza tale richiesta, Continuare?", script, Nothing)
                Else
                    AjaxManager.ResponseScripts.Add(script)
                End If

            Case ResolutionGridBarController.BarWorkflowStep.Pubblicazione
                Dim collegioDate As String = String.Format("{0:yyyyMMdd}", If(CollegioWarningDate.HasValue, CollegioWarningDate.GetValueOrDefault(), ""))
                s = CommonShared.AppendSecurityCheck("Action=Pubblicazione&" & sParteComune & "&Protocollo=" & TextProtocollo & "&CollegioWarningDate=" & collegioDate)
                script = "return " & ID & "_OpenWindow('windowFlussoElenco','Invio pubblicazione con firma digitale','" & sUrl + s & "',700,500);"

                Dim selSporco As Boolean = CheckRegioneSelected(list)

                If list.Count > 0 AndAlso Tipologia = ResolutionType.IdentifierDelibera Then
                    Dim strLista As String = String.Empty
                    For i As Integer = 0 To list.Count - 1
                        strLista &= "\n" & list(i)
                    Next i
                    script = "alert('IMPORTANTE: Ricordarsi di rendere esecutiva la delibera in esame con la data della D.G.R.\n\nDelibere da controllare: " & strLista & "'); " & script
                End If

                If selSporco Then
                    BasePage.AjaxAlertConfirm("Selezionati atti con richiesta di approvazione dalla regione con atti senza tale richiesta, Continuare?", script, Nothing)
                Else
                    AjaxManager.ResponseScripts.Add(script)
                End If

                If String.IsNullOrEmpty(sIDs) Then
                    BasePage.AjaxAlert("Nessun elemento selezionato.", False)
                    Exit Sub
                End If

                If (CheckRegioneSelected(list)) Then
                    BasePage.AjaxAlertConfirm("Selezionati atti con richiesta di approvazione dalla regione con atti senza tale richiesta, Continuare?", script, Nothing)
                Else
                    AjaxManager.ResponseScripts.Add(script)
                End If

                'Se il parametro è attivo, salvo in sessione i documenti selezionati in griglia, per ogni atto, da pubblicare sull'albo online
                If ResolutionEnv.AutomaticActivityStepEnabled Then
                    Dim selection As IList(Of PublicationResolutionDocumentModel) = GetSelectedResolutions()
                    CurrentResolutionDocuments = selection
                End If

            Case ResolutionGridBarController.BarWorkflowStep.TrasmissioneCollegioSindacaleFirmaDigitale
                If String.IsNullOrEmpty(sIDs) Then
                    BasePage.AjaxAlert("Nessun elemento selezionato.", False)
                    Exit Sub
                End If

                s = CommonShared.AppendSecurityCheck("Action=TrasmAdozioneCollegioSindacaleFirmaDigitale&" & sParteComune)
                script = "return " & ID & "_OpenWindow('windowFlussoElenco','Trasmissione a Collegio Sindacale con Firma Digitale','" & sUrl + s & "',700,500);"

                If (CheckRegioneSelected(list)) Then
                    BasePage.AjaxAlertConfirm("Selezionati atti con richiesta di approvazione dalla regione con atti senza tale richiesta, Continuare?", script, Nothing)
                Else
                    AjaxManager.ResponseScripts.Add(script)
                End If

            Case ResolutionGridBarController.BarWorkflowStep.TrasmissioneOc
                If String.IsNullOrEmpty(sIDs) Then
                    BasePage.AjaxAlert("Nessun elemento selezionato.", False)
                    Exit Sub
                End If

                s = CommonShared.AppendSecurityCheck("Action=TrasmAdozioneOrganoControllo&" & sParteComune)
                script = "return " & ID & "_OpenWindow('windowFlussoElenco','Trasmissione OC e Pubblicazione','" & sUrl + s & "',700,500);"

                If (CheckRegioneSelected(list)) Then
                    BasePage.AjaxAlertConfirm("Selezionati atti con richiesta di approvazione dalla regione con atti senza tale richiesta, Continuare?", script, Nothing)
                Else
                    AjaxManager.ResponseScripts.Add(script)
                End If

            Case ResolutionGridBarController.BarWorkflowStep.StampaUltimaPagina
                If String.IsNullOrEmpty(sIDs) Then
                    BasePage.AjaxAlert("Nessun elemento selezionato.", False)
                    Exit Sub
                End If

                ' nuova gestione con report viewer
                Dim allPdfs As New List(Of String)
                Dim resolutionIds As IList(Of Integer) = GetSelectedItems()
                For Each idResolution As Integer In resolutionIds
                    Dim resolution As Resolution = Facade.ResolutionFacade.GetById(idResolution)
                    Dim ultimaPaginaPrintPdf As New ReslUltimaPaginaPrintPdf(CheckOmissis)
                    Dim path As String = ultimaPaginaPrintPdf.GeneraUltimaPagina(resolution, True)
                    allPdfs.Add(path)
                    ' LOG
                    Facade.ResolutionLogFacade.Log(resolution, ResolutionLogType.RP, "Ultima pagina creata per stampa")
                Next
                If ResolutionEnv.MailMassiveLastPageCreatedEnabled Then
                    'INVIA MAIL AI FIRMATARI DEFINITI NEL GRUPPO
                    LastPageSendMail(allPdfs.Count)
                End If
                BasePage.AjaxAlert(String.Format("Creazione di {0} document{1} di 'Ultima Pagina' avvenuta con successo.", allPdfs.Count, If(allPdfs.Count = 1, "o", "i")))
            Case ResolutionGridBarController.BarWorkflowStep.DaAffariGenerali
                If String.IsNullOrEmpty(sIDs) Then
                    BasePage.AjaxAlert("Nessun elemento selezionato.", False)
                    Exit Sub
                End If
                s = CommonShared.AppendSecurityCheck("Action=TrasmDaAffariGenerali&" & sParteComune)
                script = String.Format("return {0}_OpenWindow('windowFlussoElenco','Crea collaborazione da Affari Generali','{1}{2}',700,500);", ID, sUrl, s)

                If (CheckRegioneSelected(list)) Then
                    BasePage.AjaxAlertConfirm("Selezionati atti con richiesta di approvazione dalla regione con atti senza tale richiesta, Continuare?", script, Nothing)
                Else
                    AjaxManager.ResponseScripts.Add(script)
                End If

        End Select
    End Sub

    Private Sub AjaxManager_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)

        Select Case Left(e.Argument.ToLower, 4)
            Case "true"
                btnWorkflow.Enabled = True
            Case "fals"
                btnWorkflow.Enabled = False
            Case "wnd|"
                Session.Add("DocumentToOpen", e.Argument.Split("|"c)(1))
                AjaxManager.ResponseScripts.Add("RefreshSearch();")
            Case "clea"
                CurrentResolutionDocuments = Nothing
        End Select

    End Sub

#End Region

#Region " Methods "

    Protected Overrides Sub InitializeAjaxSettings()
        MyBase.InitializeAjaxSettings()
        If AjaxEnabled Then
            AjaxManager.AjaxSettings.AddAjaxSetting(btnDocuments, Grid)
            If RightPanel.Visible Then
                AjaxManager.AjaxSettings.AddAjaxSetting(btnShowRegion, Grid, AjaxLoadingPanel)
                AjaxManager.AjaxSettings.AddAjaxSetting(btnShowAll, Grid, AjaxLoadingPanel)
            End If
            If LeftPanel.Visible Then
                AjaxManager.AjaxSettings.AddAjaxSetting(btnWorkflow, Grid, AjaxLoadingPanel)
                AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, btnWorkflow)
            End If
            AddHandler AjaxManager.AjaxRequest, AddressOf AjaxManager_AjaxRequest
        End If
    End Sub

    Protected Overrides Sub AttachEvents()
        MyBase.AttachEvents()
        AddHandler btnDocuments.Click, AddressOf BtnDocumentsClick
        If LeftPanel.Visible Then
            AddHandler btnWorkflow.Click, AddressOf BtnWorkflowClick
        End If
        If RightPanel.Visible Then
            AddHandler btnShowRegion.Click, AddressOf btnShowRegion_Click
            AddHandler btnShowAll.Click, AddressOf btnShowAll_Click
        End If

        AddHandler btnPubblicaWeb.Click, AddressOf btnPubblicaWeb_Click
    End Sub

    Protected Overrides Sub Print()
        Dim reslprint As ResolutionsPrint

        'Controllo se devo avviare la stampa del workflow
        If HasWorkflow Then
            reslprint = New ReslWorkflowPrint
        Else
            If DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("ASL3-TO") Then
                reslprint = New ResolutionsPrintTO
            Else
                reslprint = New ResolutionsPrint
            End If
        End If

        reslprint.ListId = GetSelectedItems()
        Session.Add("Printer", reslprint)
        Response.Redirect("..\Comm\CommPrint.aspx?Type=Prot&PrintName=Resolution")
    End Sub

    Public Overrides Function GetSelectedItems() As IList
        Dim sId As New List(Of Integer)
        For Each item As GridDataItem In _grid.Items
            Dim cb As CheckBox = DirectCast(item.FindControl("cbSelect"), CheckBox)
            If cb.Checked Then
                Dim lb As LinkButton = DirectCast(item.FindControl("lnkResolution"), LinkButton)
                If lb.CommandName.Eq("ShowResl") Then
                    sId.Add(Integer.Parse(lb.CommandArgument))
                End If
            End If
        Next
        Return sId
    End Function

    Protected Overrides Sub ConfigureSetReadProperties()
    End Sub


    Public Function GetSelectedResolutions() As IList(Of PublicationResolutionDocumentModel)
        Dim selection As IList(Of PublicationResolutionDocumentModel) = New List(Of PublicationResolutionDocumentModel)()
        Dim resolutionDocument As PublicationResolutionDocumentModel
        Dim idResolution As Integer
        Dim idDocument As Guid
        Dim documents As IList(Of Guid)

        For Each item As GridDataItem In _grid.Items
            resolutionDocument = New PublicationResolutionDocumentModel()
            documents = New List(Of Guid)

            Dim cb As CheckBox = DirectCast(item.FindControl("cbSelect"), CheckBox)
            If Not cb.Checked Then
                Continue For
            End If

            Dim lb As LinkButton = DirectCast(item.FindControl("lnkResolution"), LinkButton)

            If lb.CommandName.Eq("ShowResl") AndAlso Integer.TryParse(lb.CommandArgument, idResolution) Then
                resolutionDocument.IdResolution = idResolution
                Dim cbl As CheckBoxList = DirectCast(item.FindControl("DocumentiCheckList"), CheckBoxList)
                For Each li As ListItem In cbl.Items
                    If li.Selected Then
                        idDocument = Guid.Parse(li.Value.Split("|"c)(0))
                        documents.Add(idDocument)
                    End If
                Next
                resolutionDocument.IdDocuments = documents
                selection.Add(resolutionDocument)
            End If
        Next

        Return selection
    End Function


    'TODO: da togliere quando verrà eliminata la pagina Pubblicazione Web
    Public Function GetSelectedDocuments() As Dictionary(Of Integer, WebDocument)
        Dim list As New Dictionary(Of Integer, WebDocument)

        For Each item As GridDataItem In _grid.Items
            Dim cb As CheckBox = DirectCast(item.FindControl("cbSelect"), CheckBox)
            If Not cb.Checked Then
                Continue For
            End If

            ' Elemento selezionato
            Dim webdoc As New WebDocument

            Dim lb As LinkButton = DirectCast(item.FindControl("lnkResolution"), LinkButton)
            If lb.CommandName.Eq("ShowResl") Then
                ' Recupero la Resolution
                webdoc.Resolution = Facade.ResolutionFacade.GetById(Integer.Parse(lb.CommandArgument), False, "ReslDB")

                Dim privacy As Boolean = False
                If webdoc.Resolution.Container.Privacy.HasValue Then
                    If webdoc.Resolution.Container.Privacy.Value Then
                        privacy = True
                    End If
                End If

                'Dim fileResolution As FileResolution = Facade.FileResolutionFacade.GetByResolution(webdoc.Resolution)(0)
                Dim workActive As TabWorkflow = Nothing 'drWActive
                If Not Facade.TabWorkflowFacade.GetByStep(webdoc.Resolution.WorkflowType, ResolutionFacade.DocType.Adozione, workActive) Then
                    Return Nothing
                End If

                Dim cbl As CheckBoxList = DirectCast(item.FindControl("DocumentiCheckList"), CheckBoxList)
                If cbl.Items.Count = 0 Then ' rbl.Items.Count = 0 Then
                    ' seleziono il documento della resolution
                    'Dim fieldDoc As Integer? = ReflectionHelper.GetPropertyCase(fileResolution, workActive.FieldDocument)
                    'If Not fieldDoc.HasValue Then
                    '    webdoc.IdChain = -1 ' non c'è nessun documento allegato
                    'Else
                    '    webdoc.IdChain = fieldDoc.Value
                    'End If
                    'webdoc.ChainEnum = 0
                    webdoc.Privacy = False
                Else
                    ' L'enumeratore della catena allegati
                    'webdoc.IdChain = -1
                    If privacy Then
                        'Dim fieldAttachment As Integer? = ReflectionHelper.GetPropertyCase(fileResolution, workActive.FieldAttachment)
                        'If fieldAttachment.HasValue Then
                        '    'webdoc.IdChain = fieldAttachment.Value
                        'End If
                        webdoc.Privacy = True
                    Else
                        'Dim fieldDocument As Integer? = ReflectionHelper.GetPropertyCase(fileResolution, workActive.FieldDocument)
                        'If fieldDocument.HasValue Then
                        '    'webdoc.IdChain = fieldDocument.Value
                        'End If
                        webdoc.Privacy = False
                    End If
                    webdoc.DocumentsIds = New List(Of Guid)
                    For Each li As ListItem In cbl.Items
                        If Not li.Selected Then Continue For
                        webdoc.DocumentsIds.Add(New Guid(li.Value.Split("|"c)(0)))
                    Next
                    webdoc.Privacy = privacy
                End If
            End If
            If list.ContainsKey(webdoc.Resolution.Id) Then
                FileLogger.Info(LoggerName, "WebPublication: elemento già presente in lista: " & webdoc.Resolution.Id)
                Continue For
            Else
                list.Add(webdoc.Resolution.Id, webdoc)
            End If
        Next
        Return list
    End Function

    ''' <summary> Metodo che cotrolla se sono selezionate sia righe Regione che non </summary>
    ''' <param name="list">Lista che viene popolata con il Number/ServiceNumber delle Regioni</param>
    Public Function CheckRegioneSelected(ByRef list As ArrayList) As Boolean
        Dim region As Boolean = False
        Dim normal As Boolean = False

        For Each item As Telerik.Web.UI.GridDataItem In _grid.Items
            Dim cbSelect As CheckBox = DirectCast(item.FindControl("cbSelect"), CheckBox)
            If Not cbSelect.Checked Then
                Continue For
            End If

            Dim chkRegion As CheckBox = DirectCast(item("Regione").Controls(0), CheckBox)
            If chkRegion.Checked Then
                Dim lnkRegion As LinkButton = DirectCast(item.FindControl("lnkResolution"), LinkButton)
                list.Add(lnkRegion.Text)
                region = True
            Else
                normal = True
            End If

        Next item

        Return (region And normal)
    End Function

    Private Function ListToCSV(ByVal list As IList(Of Integer)) As String
        Dim sRet As String = String.Empty

        For i As Integer = 0 To list.Count - 1
            sRet &= list(i) & "-"
        Next

        Return sRet.TrimEnd("-"c)
    End Function

    Public Sub SetButtonLabel()
        Select Case MyStep
            Case ResolutionGridBarController.BarWorkflowStep.Adotta
                btnWorkflow.Text = ADOTTA
                btnWorkflow.Width = Unit.Pixel(243)
            Case ResolutionGridBarController.BarWorkflowStep.AvvenutaAdozione
                btnWorkflow.Text = AVVENUTA_ADOZIONE
                btnWorkflow.Width = Unit.Pixel(243)
            Case ResolutionGridBarController.BarWorkflowStep.TrasmissioneOc
                btnWorkflow.Text = TRASMISSIONE_OC
                btnWorkflow.Width = Unit.Pixel(243)
            Case ResolutionGridBarController.BarWorkflowStep.Esecutiva
                btnWorkflow.Text = ESECUTIVITA
                btnWorkflow.Width = Unit.Pixel(243)
            Case ResolutionGridBarController.BarWorkflowStep.TrasmissioneCollegioSindacaleFirmaDigitale
                btnWorkflow.Text = TRASMISSIONE_CS_FIRMA_DIGITALE
                btnWorkflow.Width = Unit.Pixel(243)
            Case ResolutionGridBarController.BarWorkflowStep.Pubblicazione
                btnWorkflow.Text = PUBBLICAZIONE
                btnWorkflow.Width = Unit.Pixel(243)
            Case ResolutionGridBarController.BarWorkflowStep.StampaUltimaPagina
                btnWorkflow.Text = STAMPA_ULTIMA_PAGINA
                btnWorkflow.Width = Unit.Pixel(243)
            Case ResolutionGridBarController.BarWorkflowStep.DaAffariGenerali
                btnWorkflow.Text = DA_AFFARI_GENERALI
                btnWorkflow.Width = Unit.Pixel(243)
        End Select
    End Sub

    Private Sub LastPageSendMail(pages As Integer)
        Try
            Dim contacts As New List(Of MessageContactEmail)
            contacts.Add(Facade.MessageContactEmailFacade.CreateEmailContact(CommonInstance.UserDescription, DocSuiteContext.Current.User.FullUserName, Facade.UserLogFacade.EmailOfUser(DocSuiteContext.Current.User.UserName, DocSuiteContext.Current.User.Domain, True), MessageContact.ContactPositionEnum.Sender))
            Dim recipients As New List(Of SecurityUsers)

            For Each groupId As String In DocSuiteContext.Current.ResolutionEnv.DigitalLastPageGroup.Split({"|"c, ","c}, StringSplitOptions.RemoveEmptyEntries)
                Dim id As Integer
                If Not Integer.TryParse(groupId, id) Then
                    Dim message As String = String.Format("Il valore {0} non è un identificativo SecurityGroups valido.", groupId)
                    Throw New InvalidCastException(message)
                End If
                recipients = recipients.Union(Facade.SecurityUsersFacade.GetUsersByGroup(id)).ToList()
            Next

            For Each receiver As SecurityUsers In recipients
                Dim user As UserLog = Facade.UserLogFacade.GetByUser(receiver.Account, String.Empty)
                Dim userMail As String = ValidateMailContact(receiver.Account, user.UserMail)
                If Not String.IsNullOrEmpty(userMail) Then
                    contacts.Add(Facade.MessageContactEmailFacade.CreateEmailContact(receiver.Description, DocSuiteContext.Current.User.FullUserName, userMail, MessageContact.ContactPositionEnum.Recipient))
                    Continue For
                End If
                Dim adUser As AccountModel = CommonAD.GetAccount(receiver.Account)
                Dim contactMail As String = ValidateMailContact(receiver.Account, adUser.Email)
                contacts.Add(Facade.MessageContactEmailFacade.CreateEmailContact(receiver.Description, DocSuiteContext.Current.User.FullUserName, contactMail, MessageContact.ContactPositionEnum.Recipient))
            Next

            Dim body As String = String.Empty
            Dim link As String = "<a href='{0}?Tipo=Resl&Azione=firmaUltimaPagina'>{1}</a>"

            link = String.Format(link, DocSuiteContext.Current.CurrentTenant.DSWUrl, "firma ultima pagina")

            body = String.Format("Sono disponibili {0} nuove ultime pagine da firmare digitalmente. Premere qui {1} per accedere alla funzionalità", pages, link)
            Dim email As MessageEmail
            email = Facade.MessageEmailFacade.CreateEmailMessage(contacts, "Generazione ultime pagine", body, False)
            Facade.MessageEmailFacade.SendEmailMessage(email)

        Catch dswx As DocSuiteException
            BasePage.AjaxAlert("Impossibile recapitare l'email: {0}.", dswx.Message)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, ex.Message, ex)
            BasePage.AjaxAlert("Errore impossibile inviare Email. Contattare l'assistenza")
        End Try
    End Sub
    Private Function ValidateMailContact(userName As String, mail As String) As String
        If Not String.IsNullOrEmpty(mail) Then
            Dim contactMail As String = RegexHelper.MatchEmail(mail)
            If String.IsNullOrEmpty(contactMail) Then
                Throw New DocSuiteException("Errore invio", String.Format("Il destinatario ({0}) non ha un indirizzo email.", userName))
            End If
            Return contactMail
        End If
        Return String.Empty
    End Function
#End Region

End Class

