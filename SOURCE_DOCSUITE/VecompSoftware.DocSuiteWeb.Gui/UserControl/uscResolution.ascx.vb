Imports System.Collections.Generic
Imports System.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Gui.UserControl
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Logging
Imports System.Web
Imports VecompSoftware.DocSuiteWeb.DTO.Resolutions
Imports System.Collections.ObjectModel

Partial Public Class uscResolution
    Inherits DocSuite2008BaseControl

#Region " Fields "

    Private _btnDoc4 As Button
    Private _isStorico As Boolean

    Private _currentResolution As Resolution
    Private _fullNumber As String = String.Empty
    Private _roleProposerEnabled As Boolean?
    Private Const PROPOSER_FIELD_DATA_NAME As String = "Proposer"
    Private Const ROLE_PROPOSER_PROPERTY_NAME As String = "ROLEPROPOSER"
    Private _currentParer As ResolutionParer
    Dim _currentTabWorkflow As TabWorkflow
#End Region

#Region " Properties "

    Private ReadOnly Property ResolutionEnv() As ResolutionEnv
        Get
            Return DocSuiteContext.Current.ResolutionEnv
        End Get
    End Property

    Protected Property ProvNumber As String

    Protected Property FullNumber() As String
        Get
            Return _fullNumber
        End Get
        Set(ByVal value As String)
            _fullNumber = value
        End Set
    End Property

    Public Property CurrentResolution() As Resolution
        Get
            Return _currentResolution
        End Get
        Set(ByVal value As Resolution)
            _currentResolution = value
            uscWorkflow.CurrentResolution = value

            uscOC.CurrentResolution = value
        End Set
    End Property
    Private _currentResolutionRight As ResolutionRights
    Public ReadOnly Property CurrentResolutionRight As ResolutionRights
        Get
            If _currentResolutionRight Is Nothing AndAlso CurrentResolution IsNot Nothing Then
                _currentResolutionRight = New ResolutionRights(CurrentResolution)
            End If
            Return _currentResolutionRight
        End Get
    End Property

    Public Property ResolutionWorkflow() As UscResWorkflow
        Get
            Return uscWorkflow
        End Get
        Set(ByVal value As UscResWorkflow)
            uscWorkflow = value
        End Set
    End Property

    Public ReadOnly Property WebStateLabel() As Label
        Get
            Return _webStateLabel
        End Get
    End Property

    Public ReadOnly Property CheckPublication() As CheckBox
        Get
            Return chkPublication
        End Get
    End Property

    Public ReadOnly Property WebPublicationDateLabel() As Label
        Get
            Return _webPublicationDateLabel
        End Get
    End Property

    Public ReadOnly Property WebRevokeDateLabel() As Label
        Get
            Return _webRevokeDateLabel
        End Get
    End Property

    Public ReadOnly Property AuslPcWebPublicationPanel As Panel
        Get
            Return AuslPcWebPublication
        End Get
    End Property

    Public Property ResolutionOC() As uscResolutionOC
        Get
            Return uscOC
        End Get
        Set(ByVal value As uscResolutionOC)
            uscOC = value
        End Set
    End Property

    Public Property ButtonDoc4() As Button
        Get
            Return _btnDoc4
        End Get
        Set(ByVal value As Button)
            _btnDoc4 = value
        End Set
    End Property

    Public Property IsStorico() As Boolean
        Get
            Return _isStorico
        End Get
        Set(ByVal value As Boolean)
            _isStorico = value
        End Set
    End Property

    Public ReadOnly Property ParerDetailUrl() As String
        Get
            Return ResolveUrl("~/PARER/ParerDetail.aspx")
        End Get
    End Property

    ''' <summary> Mostra/Nasconde pannello con la numerazione. </summary>
    Public Property VisibleNumber() As Boolean
        Get
            Return tblResolution.Visible
        End Get
        Set(ByVal value As Boolean)
            tblResolution.Visible = value
        End Set
    End Property

    ''' <summary> Mostra/Nasconde pannello con i dati degli ordini di controllo. </summary>
    Public Property VisibleODC() As Boolean
        Get
            Return tblODC.Visible
        End Get
        Set(ByVal value As Boolean)
            tblODC.Visible = value
        End Set
    End Property

    ''' <summary> Mostra/Nasconde pannello con la motivazione dell'annullamento. </summary>
    Public Property VisibleMotivazione() As Boolean
        Get
            Return tblMotivazione.Visible
        End Get
        Set(ByVal value As Boolean)
            tblMotivazione.Visible = value
        End Set
    End Property

    ''' <summary> Mostra/Nasconde pannello con l'imgButton del documento OC. </summary>
    Public Property VisibleOCDocumento() As Boolean
        Get
            Return imgDocumento.Visible
        End Get
        Set(ByVal value As Boolean)
            imgDocumento.Visible = value
            lblDocumento.Visible = value
        End Set
    End Property

    ''' <summary> Mostra/Nasconde pannello con i dati sullo Stato. </summary>
    Public Property VisibleStatus() As Boolean
        Get
            Return tblStatus.Visible
        End Get
        Set(ByVal value As Boolean)
            tblStatus.Visible = value
        End Set
    End Property

    ''' <summary> Mostra/Nasconde pannello con i dati riguardanti Oggetto e Note. </summary>
    Public Property VisibleObject() As Boolean
        Get
            Return tblObject.Visible
        End Get
        Set(ByVal value As Boolean)
            tblObject.Visible = value
        End Set
    End Property

    ''' <summary> Mostra/Nasconde pannello con i dati economici. </summary>
    Public Property VisibleEconomyData() As Boolean
        Get
            Return tblEconomicData.Visible
        End Get
        Set(ByVal value As Boolean)
            tblEconomicData.Visible = value
        End Set
    End Property

    ''' <summary> Mostra/Nasconde pannello con i dati riguardanti i contatti. </summary>
    Public Property VisibleComunication() As Boolean
        Get
            Return tblComunication.Visible
        End Get
        Set(ByVal value As Boolean)
            tblComunication.Visible = value
        End Set
    End Property

    ''' <summary> Mostra/Nasconde pannello con i dati riguardanti i contatti Destinatario e Proponente. </summary>
    Public Property VisibleComunicationDestProp() As Boolean
        Get
            Return trDestProp.Visible
        End Get
        Set(ByVal value As Boolean)
            trDestProp.Visible = value
        End Set
    End Property

    ''' <summary> Mostra/Nasconde pannello con i dati riguardanti i contatti Assegnatario e Responsabile. </summary>
    Public Property VisibleComunicationAssMgr() As Boolean
        Get
            Return trAssMgr.Visible
        End Get
        Set(ByVal value As Boolean)
            trAssMgr.Visible = value
        End Set
    End Property

    ''' <summary> Mostra/Nasconde pannello con i dati riguardanti i contatti Destinatario e Proponente Alternativi. </summary>
    Public Property VisibleComunicationDestPropAlternative() As Boolean
        Get
            Return trDestPropAlternative.Visible
        End Get
        Set(ByVal value As Boolean)
            trDestPropAlternative.Visible = value
        End Set
    End Property

    ''' <summary> Mostra/Nasconde pannello con i dati riguardanti i contatti Assegnatario e Responsabile Alternativi. </summary>
    Public Property VisibleComunicationAssMgrAlternative() As Boolean
        Get
            Return trAssMgrAlternative.Visible
        End Get
        Set(ByVal value As Boolean)
            trAssMgrAlternative.Visible = value
        End Set
    End Property

    ''' <summary> Mostra/Nasconde pannello con i dati riguardanti le autorizzazioni. </summary>
    Public Property VisibleRoles As Boolean
        Get
            Return uscSettori.Visible
        End Get
        Set(ByVal value As Boolean)
            uscSettori.Visible = value
        End Set
    End Property

    ''' <summary> Mostra/Nasconde pannello con i dati riguardanti la classificazione. </summary>
    Public Property VisibleCategory() As Boolean
        Get
            Return tblCategory.Visible
        End Get
        Set(ByVal value As Boolean)
            tblCategory.Visible = value
        End Set
    End Property

    ''' <summary> Mostra/Nasconde pannello con i dati di utilità. </summary>
    Public Property VisibleOther() As Boolean
        Get
            Return tblOther.Visible
        End Get
        Set(ByVal value As Boolean)
            tblOther.Visible = value
        End Set
    End Property

    ''' <summary> Mostra/Nasconde pannello con i dati per le autorizzazioni. </summary>
    Public Property VisibleDataRole() As Boolean
        Get
            Return tblDataRole.Visible
        End Get
        Set(ByVal value As Boolean)
            tblDataRole.Visible = value
        End Set
    End Property

    Public Property VisibleCheckWebPublish() As Boolean
        Get
            Return chkPublication.Visible
        End Get
        Set(ByVal value As Boolean)
            chkPublication.Visible = value
            lblPubblication.Visible = value
            tbPublication.Visible = value
            tbPublicationLabel.Visible = value
        End Set
    End Property

    Public Property VisibleWebRetire() As Boolean
        Get
            Return tblWebRetireData.Visible
        End Get
        Set(ByVal value As Boolean)
            tblWebRetireData.Visible = value
        End Set
    End Property

    ''' <summary> Mostra/Nasconde pannello con la check che indica se è immediatamente esecutiva. </summary>
    ''' <remarks> ASL-TO2 </remarks>
    Public Property VisibleImmediatelyExecutive() As Boolean
        Get
            Return trResolutionImmediatelyExecutive.Visible
        End Get
        Set(ByVal value As Boolean)
            trResolutionImmediatelyExecutive.Visible = value
        End Set
    End Property

    ''' <summary> Mostra/Nasconde pannello con l'indicazione del protocollo per la trasmissione ai Servizi. </summary>
    ''' <remarks> ASL-TO2 </remarks>
    Public Property VisibleProposerProtocolLink() As Boolean
        Get
            Return trResolutionProposerProtocolLink.Visible
        End Get
        Set(ByVal value As Boolean)
            trResolutionProposerProtocolLink.Visible = value
        End Set
    End Property

    ''' <summary> Mostra/Nasconde pannello con i dati riguardanti Oggetto Privacy. </summary>
    ''' <remarks> ASL-TO2 </remarks>
    Public Property VisibleObjectPrivacy() As Boolean
        Get
            Return tblObjectPrivacy.Visible
        End Get
        Set(ByVal value As Boolean)
            tblObjectPrivacy.Visible = value
        End Set
    End Property
    Public Property VisibleAmmTraspMonitorLog As Boolean
        Get
            Return uscAmmTraspMonitorLog.Visible
        End Get
        Set(value As Boolean)
            uscAmmTraspMonitorLog.Visible = value
        End Set
    End Property

    Public ReadOnly Property RoleProposerEnabled As Boolean
        Get
            If Not _roleProposerEnabled.HasValue AndAlso CurrentResolution IsNot Nothing Then
                _roleProposerEnabled = Facade.ResolutionFacade.IsManagedProperty(PROPOSER_FIELD_DATA_NAME, CurrentResolution.Type.Id, ROLE_PROPOSER_PROPERTY_NAME)
            End If
            Return _roleProposerEnabled.Value
        End Get
    End Property

    Public ReadOnly Property CurrentParer As ResolutionParer
        Get
            If _currentParer Is Nothing Then
                _currentParer = Facade.ResolutionParerFacade.GetByResolution(CurrentResolution)
            End If
            Return _currentParer
        End Get
    End Property

    Public ReadOnly Property AjaxDefaultLoadingPanel() As RadAjaxLoadingPanel
        Get
            Return DefaultLoadingPanel
        End Get
    End Property

    Private Property CurrentResolutionModel As ResolutionInsertModel
        Get
            If Session("CurrentResolutionModel") IsNot Nothing Then
                Return DirectCast(Session("CurrentResolutionModel"), ResolutionInsertModel)
            End If
            Return Nothing
        End Get
        Set(value As ResolutionInsertModel)
            If value Is Nothing Then
                Session.Remove("CurrentResolutionModel")
            Else
                Session("CurrentResolutionModel") = value
            End If
        End Set
    End Property

    Public ReadOnly Property WindowAmmTraspMonitorLog As uscAmmTraspMonitorLog
        Get
            Return uscAmmTraspMonitorLog
        End Get
    End Property
    Public ReadOnly Property CurrentActiveTabWorkflow() As TabWorkflow
        Get
            If _currentTabWorkflow Is Nothing AndAlso CurrentResolution IsNot Nothing Then
                _currentTabWorkflow = Facade.TabWorkflowFacade.GetActive(CurrentResolution)
            End If
            Return _currentTabWorkflow
        End Get
    End Property
#End Region

#Region " Events "

    Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
        CurrentResolution = New Resolution()
        VisibleCategory = False
        VisibleComunication = False
        VisibleComunicationAssMgr = False
        VisibleComunicationAssMgrAlternative = False
        VisibleComunicationDestProp = False
        VisibleComunicationDestPropAlternative = False
        VisibleEconomyData = False
        VisibleNumber = False
        VisibleObject = False
        VisibleODC = False
        VisibleOther = False
        VisibleRoles = False
        VisibleStatus = False
        VisibleDataRole = False
        tblWorkflow.Visible = False
        tblComunicationStorico.Visible = False
        lblPubblication.Visible = ResolutionEnv.IsPublicationEnabled
        lblResolutionDeleteReason.Text = ResolutionEnv.ResolutionDeclineReasonLabel
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        uscDocumentUnitReferences.IdDocumentUnit = CurrentResolution.UniqueId.ToString()

        'Calcola FullNumber dell'atto
        Facade.ResolutionFacade.ReslFullNumber(CurrentResolution, CurrentResolution.Type.Id, ProvNumber, FullNumber)
        lblObjectPrivacy.Text = ResolutionEnv.ResolutionObjectPrivacyLabel
        lblObjectPrivacyDetail.Text = String.Concat(ResolutionEnv.ResolutionObjectPrivacyLabel, ":")
        uscMulticlassificationRest.IdDocumentUnit = CurrentResolution.UniqueId.ToString()
        uscMulticlassificationRest.Visible = ProtocolEnv.MulticlassificationEnabled
    End Sub
    Protected Sub DgvResolutionKindDocumentSeries_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles dgvResolutionKindDocumentSeries.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim entity As ResolutionDocumentSeriesItem = DirectCast(e.Item.DataItem, ResolutionDocumentSeriesItem)
        Dim seriesItem As DocumentSeriesItem = Facade.DocumentSeriesItemFacade.GetById(entity.IdDocumentSeriesItem)
        DirectCast(e.Item.FindControl("lblDocumentSeriesName"), Label).Text = seriesItem.DocumentSeries.Name
        Dim seriesLink As HyperLink = DirectCast(e.Item.FindControl("documentSeriesLink"), HyperLink)
        Dim btnCorrect As ImageButton = DirectCast(e.Item.FindControl("btnCorrectSeries"), ImageButton)
        seriesLink.Text = String.Format("N. {0} del {1}", seriesItem.Id, seriesItem.RegistrationDate.ToString("dd/MM/yyyy"))
        seriesLink.Attributes.Add("onclick", String.Format("return GoToDraftSeries('{0}');", seriesItem.Id))
        btnCorrect.OnClientClick = String.Format("return CorrectDraftLink('{0}');", seriesItem.Id)
    End Sub
    Private Sub uscWorkflow_AjaxRefresh(ByVal sender As Object, ByVal e As EventArgs) Handles uscWorkflow.AjaxRefresh
        'Calcola FullNumber dell'atto
        Facade.ResolutionFacade.ReslFullNumber(CurrentResolution, CurrentResolution.Type.Id, ProvNumber, FullNumber)
    End Sub

    Protected Sub ReslolutionAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arguments As String() = Split(e.Argument, "|")
        Dim draftSeriesItems As IList(Of ResolutionDocumentSeriesItem) = Facade.ResolutionDocumentSeriesItemFacade.GetByResolution(CurrentResolution.Id)
        Select Case arguments(0)
            Case "goToDraftSeries"
                Dim parameters As String = String.Format("IdDocumentSeriesItem={0}&Action={1}&PreviousPage={2}&Type=Series", arguments(1), DocumentSeriesAction.View, HttpUtility.UrlEncode(Page.Request.Url.AbsoluteUri))
                Response.Redirect(String.Format("~/Series/Item.aspx?{0}", CommonShared.AppendSecurityCheck(parameters)))
            Case "correctDraftLink"
                Dim documentSeries As DocumentSeriesItem = Facade.DocumentSeriesItemFacade.GetById(Convert.ToInt32(arguments(1)))
                If documentSeries.DocumentSeries.Id.Equals(DocSuiteContext.Current.ProtocolEnv.BandiGaraDocumentSeriesId) Then
                    BindModelFromPage()
                    Response.Redirect(String.Format("~/Series/Item.aspx?Type=Series&Action={0}&IdDocumentSeriesItem={1}&IdSeries={2}&IdResolution={3}", DocumentSeriesAction.FromResolutionView, arguments(1), documentSeries.DocumentSeries.Id, CurrentResolution.Id))
                End If
                Response.Redirect(String.Format("~/Series/Item.aspx?Type=Series&Action={0}&IdDocumentSeriesItem={1}", DocumentSeriesAction.Edit, arguments(1)))
        End Select
    End Sub
#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(uscWorkflow, tblResolution)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscWorkflow, tblOther)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscWorkflow, tblMotivazione)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscWorkflow, uscSettori)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscWorkflow, tblObjectPrivacy)

        If ResolutionEnv.ResolutionKindEnabled Then
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlAmmTrasp)
            AjaxManager.AjaxSettings.AddAjaxSetting(dgvResolutionKindDocumentSeries, dgvResolutionKindDocumentSeries)
        End If

        AddHandler AjaxManager.AjaxRequest, AddressOf ReslolutionAjaxRequest
    End Sub

    ''' <summary> Configura lo usercontrol per la visualizzazione storica. </summary>
    Public Sub LoadHistoryMode()
        uscWorkflow.Visible = Not _isStorico
        tblWorkflow.Visible = _isStorico
        tblComunicationStorico.Visible = _isStorico

        VisibleRoles = VisibleRoles AndAlso Not _isStorico

        If _isStorico Then
            WebUtils.ObjAttDisplayNone(tblComunication)
            chkPublication.Visible = False
            lblPubblication.Visible = False
        End If

        tblSettoriAutoriz.Visible = _isStorico
    End Sub

    Public Sub Show()
        VisibleMotivazione = ResolutionEnv.ResolutionDeclineTextEnabled AndAlso CurrentResolution.DeclineNote IsNot Nothing
        'Visualizzo il btnDoc4
        If _btnDoc4 IsNot Nothing Then
            _btnDoc4.Text = "Doc4"
            _btnDoc4.Attributes.Clear()
        End If

        'Visualizzo OC
        If Facade.ResolutionFacade.IsManagedProperty("OCData", CurrentResolution.Type.Id) Then
            InitializeOC()
            If Not ResolutionEnv.PanelOCDEnabled Or Not CurrentResolution.IsChecked Then
                VisibleODC = False
            End If
        Else
            VisibleEconomyData = False
        End If

        If (_btnDoc4 IsNot Nothing) AndAlso _btnDoc4.Text.Eq("Doc4") Then
            WebUtils.ObjAttDisplayNone(_btnDoc4)
            _btnDoc4.Style.Add("position", "absolute !important")
        End If

        VisibleEconomyData = Facade.ResolutionFacade.IsManagedProperty("EconomicData", CurrentResolution.Type.Id)

        If Facade.ResolutionFacade.IsManagedProperty("Proposer", CurrentResolution.Type.Id, "CONTACT") OrElse Facade.ResolutionFacade.IsManagedProperty("Recipent", CurrentResolution.Type.Id, "CONTACT") Then
            LoadProposerReceipientContacts()
        Else
            VisibleComunicationDestProp = False
        End If

        If Facade.ResolutionFacade.IsManagedProperty("Assegnee", CurrentResolution.Type.Id, "CONTACT") OrElse Facade.ResolutionFacade.IsManagedProperty("Manager", CurrentResolution.Type.Id, "CONTACT") Then
            LoadAssigneeManagerContacts()
        Else
            VisibleComunicationAssMgr = False
        End If

        VisibleCategory = Facade.ResolutionFacade.IsManagedProperty("Category", CurrentResolution.Type.Id)

        'Pubblicazione
        If ResolutionEnv.IsPublicationEnabled Then
            lblPubblication.Style.Add("display", "block")
        End If

        If ResolutionEnv.WebPublishPanel AndAlso (ResolutionEnv.WebPublishEnabled OrElse ResolutionEnv.IsPublicationEnabled) Then
            lbStatoWeb.Style.Add("display", "block")
            lbDataPubbWeb.Style.Add("display", "block")
            lbDataRetireWeb.Style.Add("display", "block")

            Dim state As String = ""
            If CurrentResolution.WebState.HasValue Then
                Select Case CurrentResolution.WebState.Value
                    Case Resolution.WebStateEnum.None
                        state = "Non pubblicato"
                    Case Resolution.WebStateEnum.Published
                        state = "Pubblicato"
                End Select
            End If
            _webStateLabel.Text = state

            If CurrentResolution.WebPublicationDate.HasValue Then
                _webPublicationDateLabel.Text = CurrentResolution.WebPublicationDate.Value.ToString("dd/MM/yyyy")
            Else
                _webPublicationDateLabel.Text = ""
            End If
            If CurrentResolution.WebRevokeDate.HasValue Then
                _webRevokeDateLabel.Text = CurrentResolution.WebRevokeDate.Value.ToString("dd/MM/yyyy")
            Else
                _webRevokeDateLabel.Text = ""
            End If
        Else
            tblWebPubblicationState.Style.Add("display", "none")
            tblWebPublicationData.Style.Add("display", "none")
            tblWebRetireData.Style.Add("display", "none")
        End If

        LoadHistoryMode()

        'LoadRoles()

        LoadParer()

        If ResolutionEnv.WebPublicationPrint Then
            Try
                Dim items As IList(Of WebPublication) = Facade.WebPublicationFacade.GetByResolution(CurrentResolution)
                If items.Count > 0 Then
                    ' Visualizzo i dati di pubblicazione
                    tblWebPublication.Visible = True
                    WebPublicationRepeater.DataSource = items
                    WebPublicationRepeater.DataBind()
                End If
            Catch ex As Exception
                FileLogger.Warn(LoggerName, ex.Message, ex)
            End Try
        End If

        '  Collaborazione
        If ProtocolEnv.IsCollaborationEnabled AndAlso ResolutionEnv.ShowCollaboration Then
            Dim collaboration As Collaboration = Facade.CollaborationFacade.GetByResolution(CurrentResolution)

            If collaboration IsNot Nothing AndAlso (CurrentResolutionRight.IsDocumentViewable(CurrentActiveTabWorkflow) OrElse (CurrentResolution.EffectivenessDate.HasValue AndAlso ResolutionEnv.ShowExecutiveDocumentEnabled)) AndAlso Facade.ResolutionFacade.HasDocuments(CurrentResolution, ResolutionEnv.QuickDocumentsCheckAllChains) Then
                collaborationPanel.Visible = True
                cmdCollaboration.Text = collaboration.CollaborationObject
                cmdCollaboration.NavigateUrl = String.Format("~/User/UserCollGestione.aspx?Type={0}&Titolo=Visualizzazione&Action={1}&idCollaboration={2}&Action2={3}&Title2=Visualizzazione",
                                                             CollaborationFacade.GetPageTypeFromDocumentType(collaboration.DocumentType), CollaborationSubAction.ProtocollatiGestiti, collaboration.Id, CollaborationMainAction.ProtocollatiGestiti)
            End If
        End If
    End Sub

    Protected Function GetProvNumber() As String
        If Not String.IsNullOrEmpty(ProvNumber) Then
            Return ProvNumber
        Else
            Return FullNumber
        End If
    End Function

    Protected Function GetProvNumberLabel() As String
        If Not String.IsNullOrEmpty(ProvNumber) Then
            Return "Numero prov.:"
        Else
            If ResolutionEnv.Configuration.Eq("AUSL-PC") AndAlso Not ResolutionEnv.CheckOCValidations AndAlso CurrentResolution.Type.Id = ResolutionType.IdentifierDelibera AndAlso CurrentResolution.IsChecked.HasValue Then
                Return "Delibera " & If(CurrentResolution.IsChecked, "soggetta a controllo", "non soggetta a controllo") & ":"
            Else
                Return Facade.ResolutionTypeFacade.GetDescription(CurrentResolution.Type) & ":"
            End If
        End If
    End Function

    Protected Function GetFullNumber() As String
        If Not String.IsNullOrEmpty(ProvNumber) Then
            Return FullNumber
        Else
            Return String.Empty
        End If
    End Function

    Protected Function GetFullNumberLabel() As String
        If Not String.IsNullOrEmpty(ProvNumber) Then
            Return String.Format("{0}:", Facade.ResolutionFacade.GetResolutionLabel(CurrentResolution))
        Else
            Return String.Empty
        End If
    End Function

    Protected Function GetControllerDescripton() As String
        If CurrentResolution.ControllerStatus IsNot Nothing Then
            Return CurrentResolution.ControllerStatus.Description
        Else
            Return String.Empty
        End If
    End Function

    Public Sub InitializeOC()
        ' VisibleODC = True ' AJG: il pannello deve essere reso visibile nel displaycontroller attivo, non qui
        If (CurrentResolution.File Is Nothing) OrElse Not CurrentResolution.File.IdControllerFile.HasValue Then
            VisibleOCDocumento = False
            Exit Sub
        End If
        Dim viewRights As Boolean = CurrentResolutionRight.IsViewable
        Dim viewerScript As String = String.Empty
        If viewRights Then
            Dim queryString As String = String.Format("idResolution={0}&field={1}&description={2}", CurrentResolution.Id, "idControllerFile", Server.UrlEncode("Risposta OC"))
            viewerScript = String.Format("window.location.href='{0}/viewers/FileResolutionViewer.aspx?{1}'; return false;", DocSuiteContext.Current.CurrentTenant.DSWUrl, CommonShared.AppendSecurityCheck(queryString))

            imgDocumento.Attributes.Add("onclick", viewerScript)
        End If
        If ButtonDoc4 Is Nothing Then
            Exit Sub
        End If

        ButtonDoc4.Text = "Risposta OC"
        If viewRights Then
            ButtonDoc4.Attributes.Add("onclick", viewerScript)
        Else
            ButtonDoc4.Enabled = False
        End If
    End Sub

    Protected Function GetStatusDescription() As String
        If CurrentResolution.Status IsNot Nothing Then
            Return CurrentResolution.Status.Description
        Else
            Return String.Empty
        End If
    End Function

    Private Function GetValidityDateFrom() As String
        If CurrentResolution.ValidityDateFrom.HasValue Then
            Return "Dal " & CurrentResolution.ValidityDateFromFormat("{0:dd/MM/yyyy}")
        Else
            Return String.Empty
        End If
    End Function

    Private Function GetValidityDateTo() As String
        If CurrentResolution.ValidityDateTo.HasValue Then
            Return " al " & CurrentResolution.ValidityDateToFormat("{0:dd/MM/yyyy}")
        Else
            Return String.Empty
        End If
    End Function

    Protected Function GetValidityContractDate() As String
        Return GetValidityDateFrom() & GetValidityDateTo()
    End Function

    Protected Function GetBidTypeDescription() As String
        If CurrentResolution.BidType IsNot Nothing Then
            Return CurrentResolution.BidType.Acronym & " - " & CurrentResolution.BidType.Description
        Else
            Return String.Empty
        End If
    End Function

    Protected Function GetAlternativeRecipient() As String
        Dim alternativeRecipient As String = String.Empty
        For Each resRecipient As ResolutionRecipient In CurrentResolution.ResolutionRecipients
            If resRecipient.Recipient IsNot Nothing Then
                alternativeRecipient &= resRecipient.Recipient.FullName & "<BR>"
            End If
        Next
        alternativeRecipient &= CurrentResolution.AlternativeRecipient

        Return alternativeRecipient
    End Function

    Protected Function GetAlternativeProposer() As String
        Dim alternativeProposer As String = String.Empty

        alternativeProposer &= CurrentResolution.AlternativeProposer

        Return alternativeProposer
    End Function

    Protected Function GetAlternativeAssignee() As String
        Dim alternativeAssignee As String = String.Empty

        alternativeAssignee &= CurrentResolution.AlternativeAssignee

        Return alternativeAssignee
    End Function

    Protected Function GetAlternativeManager() As String
        Dim alternativeManager As String = String.Empty

        alternativeManager &= CurrentResolution.AlternativeManager

        Return alternativeManager
    End Function

    Protected Sub BindContactRecipients()
        For Each reslContact As ResolutionContact In CurrentResolution.ResolutionContactsRecipients
            Dim contact As New ContactDTO()
            contact.Contact = reslContact.Contact
            contact.Type = ContactDTO.ContactType.Address
            uscContactRecipient.DataSource.Add(contact)
        Next
        uscContactRecipient.DataBind()
    End Sub

    Protected Sub BindContactProposers()
        For Each reslContact As ResolutionContact In CurrentResolution.ResolutionContactProposers
            Dim contact As New ContactDTO()
            contact.Contact = reslContact.Contact
            contact.Type = ContactDTO.ContactType.Address
            uscContactProposer.DataSource.Add(contact)
        Next
        uscContactProposer.DataBind()
    End Sub

    Protected Sub BindContactAssignees()
        For Each reslContact As ResolutionContact In CurrentResolution.ResolutionContactsAssignees
            Dim contact As New ContactDTO()
            contact.Contact = reslContact.Contact
            contact.Type = ContactDTO.ContactType.Address
            uscContactAssignee.DataSource.Add(contact)
        Next
        uscContactAssignee.DataBind()
    End Sub

    Protected Sub BindContactManagers()
        For Each reslContact As ResolutionContact In CurrentResolution.ResolutionContactsManagers
            Dim contact As New ContactDTO()
            contact.Contact = reslContact.Contact
            contact.Type = ContactDTO.ContactType.Address
            uscContactManager.DataSource.Add(contact)
        Next
        uscContactManager.DataBind()
    End Sub

    Public Sub LoadProposerReceipientContacts()
        BindContactRecipients()
        BindContactProposers()

        If Not RoleProposerEnabled AndAlso (uscContactProposer.DataSource.Any() OrElse uscContactRecipient.DataSource.Any()) Then
            trDestProp.Visible = True
            uscContactProposer.Visible = uscContactProposer.DataSource.Any()
            uscContactRecipient.Visible = uscContactRecipient.DataSource.Any()
        Else
            trDestProp.Visible = False
        End If
    End Sub

    Public Sub LoadAssigneeManagerContacts()
        BindContactAssignees()
        BindContactManagers()

        If (uscContactAssignee.DataSource.Count + uscContactManager.DataSource.Count > 0) Then
            trAssMgr.Visible = True
            If uscContactAssignee.DataSource.Count = 0 Then
                uscContactAssignee.Visible = False
            End If
            If uscContactManager.DataSource.Count = 0 Then
                uscContactManager.Visible = False
            End If
        Else
            trAssMgr.Visible = False
        End If
    End Sub

    Public Sub LoadContacts()
        LoadProposerReceipientContacts()
        LoadAssigneeManagerContacts()
    End Sub

    ''' <summary> Carica Autorizzazioni. </summary>
    Public Sub LoadRoles()
        VisibleRoles = CurrentResolution.ResolutionRoles.Count > 0
        uscSettori.Visible = VisibleRoles

        If VisibleRoles Then
            uscSettori.Caption = "Settori con Autorizzazione"
            uscSettori.SourceRoles = CurrentResolution.ResolutionRoles.Select(Function(f) f.Role).ToList()
            uscSettori.DataBind()
        End If
    End Sub

    Public Sub LoadImmediatelyExecutive()
        chkExecutive.Checked = CurrentResolution.ImmediatelyExecutive.GetValueOrDefault(False)
    End Sub

    Public Sub LoadPubblicationInternet()
        chkPublication.Checked = CurrentResolution.CheckPublication
    End Sub

    Protected Function GetMotivazioneDescription() As String
        If CurrentResolution.DeclineNote IsNot Nothing Then
            Return CurrentResolution.DeclineNote.Split("§"c)(0)
        End If
        Return String.Empty
    End Function

    ''' <summary> Carico dati PARER. </summary>
    Public Sub LoadParer()
        If Not ResolutionEnv.ParerEnabled Then
            tblParer.Visible = False
            If CurrentResolution.UltimaPaginaDate.HasValue Then
                lbDataDematerializzazione.Text = CurrentResolution.UltimaPaginaDate.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
                tblParer.Visible = True
                linkUriVersamento.Text = String.Empty
            End If
            Exit Sub
        End If

        tblParer.Visible = True

        If Facade.ResolutionParerFacade.Exists(CurrentResolution) Then
            ' Sezione Atto Dematerializzato
            If CurrentParer.ArchivedDate.HasValue Then
                lbDataVersamento.Text = CurrentParer.ArchivedDate.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
            End If
            linkUriVersamento.NavigateUrl = CurrentParer.ParerUri
            If CurrentResolution.UltimaPaginaDate.HasValue Then
                lbDataDematerializzazione.Text = CurrentResolution.UltimaPaginaDate.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
            End If

            parerInfo.ImageUrl = "../Comm/Images/info.png"
            parerInfo.OnClientClick = String.Format("return OpenParerDetail({0});", CurrentResolution.Id)

            Select Case Facade.ResolutionParerFacade.GetConservationStatus(CurrentResolution)
                Case ResolutionParerFacade.ResolutionParerConservationStatus.Correct
                    parerIcon.ImageUrl = "../Comm/images/parer/green.png"
                    parerLabel.Text = "Conservazione corretta."
                Case ResolutionParerFacade.ResolutionParerConservationStatus.Warning
                    parerIcon.ImageUrl = "../Comm/images/parer/yellow.png"
                    parerLabel.Text = "Conservazione con avviso."
                Case ResolutionParerFacade.ResolutionParerConservationStatus.Error
                    parerIcon.ImageUrl = "../Comm/images/parer/red.png"
                    parerLabel.Text = "Conservazione con errori."
                Case ResolutionParerFacade.ResolutionParerConservationStatus.Undefined
                    parerIcon.ImageUrl = "../Comm/images/parer/lightgray.png"
                    parerLabel.Text = "Stato conservazione non definito."
            End Select
        Else
            ' Non soggetto alla conservazione sostitutiva
            parerInfo.Visible = False
            parerIcon.ImageUrl = "../Comm/images/parer/lightgray.png"
            parerLabel.Text = "Non soggetto alla conservazione anticipata."
        End If
    End Sub

    Public Function GetWebPublicationDescription(pubblicationId As Integer, isPrivacy As Boolean) As String
        Dim item As WebPublication = Facade.WebPublicationFacade.GetById(pubblicationId, False)
        Dim status As String = ""
        Select Case item.Status
            Case 0
                status = ""
            Case 1
                status = "ATTIVA"
            Case 2
                status = "REVOCATA"
            Case 3
                status = "RITIRATA"
        End Select
        Dim retval As String
        If (Not String.IsNullOrEmpty(status)) Then
            retval = String.Format("{0} - Pubblicazione {1}{2} ({3})", item.RegistrationDate.ToString("dd/MM/yyyy HH:mm:ss"), If(isPrivacy, "con omissis ", String.Empty), status, item.ExternalKey)
        Else
            retval = String.Format("{0} - Prenotato numero di pubblicazione ({1})", item.RegistrationDate.ToString("dd/MM/yyyy HH:mm:ss"), item.ExternalKey)
        End If
        If (ResolutionEnv.WebPublicationPrintBaseAddress IsNot Nothing AndAlso item.Status = 1) Then
            retval = String.Format("<a target=""_blank"" href=""{0}"">{1}</a>", ResolutionEnv.WebPublicationPrintBaseAddress & item.ExternalKey, retval)
        End If
        Return retval
    End Function
    Public Function GetAdoptionTrasmissionLabel() As String
        If CurrentResolution.Type.Id.Equals(ResolutionType.IdentifierDetermina) Then
            Return "Trasm. Adozione"
        Else
            Return String.Empty
        End If
    End Function

    Private Sub BindModelFromPage()
        Dim model As ResolutionInsertModel = New ResolutionInsertModel()
        'Tipologia di atto
        model.ResolutionKind = CurrentResolution.ResolutionKind.Id
        'Categoria
        model.Category = New List(Of Integer)({CurrentResolution.Category.Id})
        'Contenitore
        model.Container = CurrentResolution.Container.Id
        'Dati Adozione
        model.AdoptionDate = CurrentResolution.AdoptionDate
        'Salvo lo stato degli oggetti in sessione
        CurrentResolutionModel = model
    End Sub
    Protected Function GetLastResolutioLog() As String
        trLastConfrimView.Visible = False
        Dim resolutionLog As ResolutionLog = Facade.ResolutionLogFacade.GetlastResolutionLog(CurrentResolution.Id, ResolutionLogType.CV)
        If Not resolutionLog Is Nothing Then
            If Not resolutionLog Is Nothing Then
                Dim user As AccountModel = CommonAD.GetAccount(resolutionLog.SystemUser)
                If user IsNot Nothing Then
                    trLastConfrimView.Visible = True
                    Return String.Format("{0} il {1}", user.DisplayName, resolutionLog.LogDate)
                End If
            End If
        End If
        Return String.Empty
    End Function
#End Region

End Class