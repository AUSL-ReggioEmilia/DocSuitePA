Imports System.Collections.Generic
Imports System.Linq
Imports System.IO
Imports VecompSoftware.Helpers.ExtensionMethods
Imports Telerik.Web.UI.Calendar
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Resolutions
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.Web
Imports System.Collections.ObjectModel
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Resolutions
Imports System.Web
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Resolutions
Imports VecompSoftware.Services.Biblos.Models

Partial Public Class uscResolutionChange
    Inherits DocSuite2008BaseControl

    Public Enum ResolutionChangeEventType
        StatusSelectedChangedEvent = 0
        ConfirmDateSelectedChangedEvent = 1
        ConfirmDateRegionSelectedChangedEvent = 2
        OCListSelectedChangedEvent = 3
        ContainerSelectedChangedEvent = 4
    End Enum

#Region " Fields "

    Private _resl As Resolution
    Private _currentResolutionRight As ResolutionRights
    Private _roleProposerEnabled As Boolean?
    Private Const PROPOSER_FIELD_DATA_NAME As String = "Proposer"
    Private Const ROLE_PROPOSER_PROPERTY_NAME As String = "ROLEPROPOSER"
    Private _currentResolutionsDocumentSeriesItem As IList(Of ResolutionDocumentSeriesItem)
    Private _resolutionKindDocumentSeries As ResolutionKindDocumentSeriesFacade
    Private _resolutionKindFacade As ResolutionKindFacade
    Private _currentResolutionStatus As String

#End Region

#Region " Properties "

    Public Property CurrentResolution() As Resolution
        Get
            Return _resl
        End Get
        Set(ByVal value As Resolution)
            _resl = value
        End Set
    End Property

    Public Property CurrentResolutionStatus As String
        Get
            If String.IsNullOrEmpty(_currentResolutionStatus) Then
                _currentResolutionStatus = Facade.TabWorkflowFacade.GetDescriptionByResolution(CurrentResolution.Id)
            End If
            Return _currentResolutionStatus
        End Get
        Set(ByVal value As String)
            _currentResolutionStatus = value
        End Set
    End Property

    Public ReadOnly Property CurrentResolutionRight As ResolutionRights
        Get
            If _currentResolutionRight Is Nothing AndAlso CurrentResolution IsNot Nothing Then
                _currentResolutionRight = New ResolutionRights(CurrentResolution)
            End If
            Return _currentResolutionRight
        End Get
    End Property

    ''' <summary> Visualizza/Nasconde Pannello Tipologia </summary>
    Public Property VisibleType() As Boolean
        Get
            Return tblType.Visible
        End Get
        Set(ByVal value As Boolean)
            tblType.Visible = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde Pannello Classificatore </summary>
    Public Property VisibleCategory() As Boolean
        Get
            Return tblCategory.Visible
        End Get
        Set(ByVal value As Boolean)
            tblCategory.Visible = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde Pannello SottoClassificatore </summary>
    Public Property VisibleSubCategory() As Boolean
        Get
            Return trSubCategory.Visible
        End Get
        Set(ByVal value As Boolean)
            trSubCategory.Visible = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde Pannello Oggetto </summary>
    Public Property VisibleObject() As Boolean
        Get
            Return trObject.Visible
        End Get
        Set(ByVal value As Boolean)
            trObject.Visible = value
            CheckObjectBox()
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde Pannello Note </summary>
    Public Property VisibleNote() As Boolean
        Get
            Return trNote.Visible
        End Get
        Set(ByVal value As Boolean)
            trNote.Visible = value
            CheckObjectBox()
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde Pannello Immediatamente Esecutiva </summary>
    Public Property VisibleImmediatelyExecutive() As Boolean
        Get
            Return trIE.Visible
        End Get
        Set(ByVal value As Boolean)
            trIE.Visible = value
            CheckObjectBox()
        End Set
    End Property

    Public Sub CheckObjectBox()
        If trIE.Visible Or trNote.Visible Or trObject.Visible Then
            tblObject.Visible = True
        Else
            tblObject.Visible = False

        End If
    End Sub

#Region "Economic Data"
    ''' <summary>
    ''' Visualizza/Nasconde Pannello Dati Economici
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property VisibleEconomicData() As Boolean
        Get
            Return tblEconomicData.Visible
        End Get
        Set(ByVal value As Boolean)
            tblEconomicData.Visible = value
        End Set
    End Property
#End Region

#Region "OC"
    ''' <summary>
    ''' Visualizza/Nasconde Pannello Organo Di Controllo
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property VisibleOC() As Boolean
        Get
            Return tblOC.Visible
        End Get
        Set(ByVal value As Boolean)
            tblOC.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde Pannello Organo Di Controllo - Commento
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property VisibleOCComment() As Boolean
        Get
            Return trOCComment.Visible
        End Get
        Set(ByVal value As Boolean)
            trOCComment.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde Pannello Regione - Commento
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property VisibleRegionComment() As Boolean
        Get
            Return trRegionComment.Visible
        End Get
        Set(ByVal value As Boolean)
            trRegionComment.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde Pannello Organo Di Controllo - Documento
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property VisibleOCUploadDocument() As Boolean
        Get
            Return trOCDocument.Visible
        End Get
        Set(ByVal value As Boolean)
            trOCDocument.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde Pannello Organo Di Controllo - Opinione
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property VisibleOCOpinion() As Boolean
        Get
            Return trOCOpinion.Visible
        End Get
        Set(ByVal value As Boolean)
            trOCOpinion.Visible = value
        End Set
    End Property
#End Region

    ''' <summary> Visualizza/Nasconde Pannello Comunicazione </summary>
    Public Property VisibleComunication() As Boolean
        Get
            Return tblComunication.Visible
        End Get
        Set(ByVal value As Boolean)
            tblComunication.Visible = value
        End Set
    End Property

#Region "Recipients"
    ''' <summary>
    ''' Visualizza/Nasconde Pannello Destinatario
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property VisibleContactRecipient() As Boolean
        Get
            Return trContactDest.Visible
        End Get
        Set(ByVal value As Boolean)
            trContactDest.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde Pannello Destinatario da Rubrica
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property VisibleContactRecipientAddress() As Boolean
        Get
            Return trDest.Visible
        End Get
        Set(ByVal value As Boolean)
            trDest.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde Pannello Destinatario Alternativo
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property VisibleContactAlternativeRecipient() As Boolean
        Get
            Return trAlternativeDest.Visible
        End Get
        Set(ByVal value As Boolean)
            trAlternativeDest.Visible = value
        End Set
    End Property
#End Region

#Region "Proposer"
    ''' <summary>
    ''' Visualizza/Nasconde Pannello Proponente
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property VisibleContactProposer() As Boolean
        Get
            Return trContactProp.Visible
        End Get
        Set(ByVal value As Boolean)
            trContactProp.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde Proponente da Rubrica
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property VisibleContactProposerAddress() As Boolean
        Get
            Return uscContactProp.Visible
        End Get
        Set(ByVal value As Boolean)
            uscContactProp.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde Proponente Alternativo
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property VisibleContactAlternativeProposer() As Boolean
        Get
            Return txtAlternativeProp.Visible
        End Get
        Set(ByVal value As Boolean)
            txtAlternativeProp.Visible = value
        End Set
    End Property

    Public Property RoleProposerVisible As Boolean
        Get
            Return uscRoleProposer.Visible
        End Get
        Set(value As Boolean)
            uscRoleProposer.Visible = value
        End Set
    End Property

    Public Property ContactProposerMultiSelect As Boolean
        Get
            Return uscContactProp.MultiSelect
        End Get
        Set(value As Boolean)
            uscContactProp.MultiSelect = value
        End Set
    End Property
#End Region

#Region "Assegnatario"
    ''' <summary>
    ''' Visualizza/Nasconde Pannello Assegnatario
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property VisibleContactAssignee() As Boolean
        Get
            Return trContactAss.Visible
        End Get
        Set(ByVal value As Boolean)
            trContactAss.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde Pannello Assegnatario da Rubrica
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property VisibleContactAssigneeAddress() As Boolean
        Get
            Return trAss.Visible
        End Get
        Set(ByVal value As Boolean)
            trAss.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde Pannello Assegnatario Alternativo
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property VisibleContactAlternativeAssignee() As Boolean
        Get
            Return trAlternativeAss.Visible
        End Get
        Set(ByVal value As Boolean)
            trAlternativeAss.Visible = value
        End Set
    End Property
#End Region

#Region "Manager"
    ''' <summary>
    ''' Visualizza/Nasconde Pannello Responsabile
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property VisibleContactManager() As Boolean
        Get
            Return trContactMgr.Visible
        End Get
        Set(ByVal value As Boolean)
            trContactMgr.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde Pannello Responsabile da Rubrica
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property VisibleContactManagerAddress() As Boolean
        Get
            Return trMgr.Visible
        End Get
        Set(ByVal value As Boolean)
            trMgr.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde Pannello Responsabile Alternativo
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property VisibleContactAlternativeManager() As Boolean
        Get
            Return trAlternativeMgr.Visible
        End Get
        Set(ByVal value As Boolean)
            trAlternativeMgr.Visible = value
        End Set
    End Property
#End Region

    ''' <summary> Visualizza/Nasconde Pannello Stato </summary>
    Public Property VisibleStatus() As Boolean
        Get
            Return tblStatus.Visible
        End Get
        Set(ByVal value As Boolean)
            tblStatus.Visible = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde Pannello Altri </summary>
    Public Property VisibleOther() As Boolean
        Get
            Return tblOther.Visible
        End Get
        Set(ByVal value As Boolean)
            tblOther.Visible = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde Pannello Contenitore </summary>
    Public Property VisibleContainer() As Boolean
        Get
            Return trContainer.Visible
        End Get
        Set(ByVal value As Boolean)
            trContainer.Visible = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde Pannello Pubblicazione su Internet. Default: non visibile </summary>
    Public Property VisiblePublication() As Boolean
        Get
            Return trPublication.Visible
        End Get
        Set(ByVal value As Boolean)
            trPublication.Visible = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde Pannello Oggetto Privacy </summary>
    Public Property VisibleObjectPrivacy() As Boolean
        Get
            Return tblObjectPrivacy.Visible
        End Get
        Set(ByVal value As Boolean)
            tblObjectPrivacy.Visible = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde Pannello Service </summary>
    Public Property VisibleProposerProtocolLink() As Boolean
        Get
            Return tblService.Visible
        End Get
        Set(ByVal value As Boolean)
            tblService.Visible = value
            uscProposerProtocolLink.SetVisible = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde Pannello Protocollo lettera di avvenuta pubblicazione </summary>
    Public Property VisiblePublicationLetterProtocolLink As Boolean
        Get
            Return tblPublicationLetterProtocolLink.Visible
        End Get
        Set(ByVal value As Boolean)
            tblPublicationLetterProtocolLink.Visible = value
            uscPublicationLetterProtocolLink.SetVisible = value
        End Set
    End Property

    ''' <summary> Visualizza in sola lettura Pannello Protocollo lettera di avvenuta pubblicazione </summary>
    Public Property ReadOnlyPublicationLetterProtocolLink As Boolean
        Get
            Return uscPublicationLetterProtocolLink.ReadOnly
        End Get
        Set(ByVal value As Boolean)
            If value Then tblPublicationLetterProtocolLink.Visible = True
            uscPublicationLetterProtocolLink.ReadOnly = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde Pannello Lista Organi di Controllo </summary>
    Public Property VisibleOCList() As Boolean
        Get
            Return tblOCList.Visible
        End Get
        Set(ByVal value As Boolean)
            tblOCList.Visible = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde Pannello Organo di Controllo - Collegio Sindacale </summary>
    Public Property VisibleOCSupervisoryBoard() As Boolean
        Get
            Return tblOCSupervisoryBoard.Visible
        End Get
        Set(ByVal value As Boolean)
            tblOCSupervisoryBoard.Visible = value
        End Set
    End Property


    ''' <summary> Visualizza/Nasconde Pannello Organo di Controllo - Corte dei Conti </summary>
    Public Property VisibleCorteDeiConti As Boolean
        Get
            Return tblCorteDeiConti.Visible
        End Get
        Set(ByVal value As Boolean)
            tblCorteDeiConti.Visible = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde Pannello Organo di Controllo - Collegio Sindacale Extra </summary>
    Public Property VisibleOCSupervisoryBoardExtra() As Boolean
        Get
            Return (trOCSupervisoryBoardFile.Visible And trOCSupervisoryBoardOpinion.Visible)
        End Get
        Set(ByVal value As Boolean)
            trOCSupervisoryBoardFile.Visible = value
            trOCSupervisoryBoardOpinion.Visible = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde Pannello Organo di Controllo - Regione </summary>
    ''' <remarks>Gestisco tramite visibilità di stile per permettere la creazione degli script ajax corretti</remarks>
    Public Property VisibleOCRegion() As Boolean
        Get
            ' Vero se è visibile
            Return tblOCRegion.Style.Item("display") Is Nothing
        End Get
        Set(ByVal value As Boolean)
            If (value) Then
                tblOCRegion.Style.Remove("display")
            Else
                tblOCRegion.Style.Add("display", "none")
            End If
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde Pannello Organo di Controllo - Gestione
    ''' </summary>
    Public Property VisibleOCManagement() As Boolean
        Get
            Return tblOCManagement.Visible
        End Get
        Set(ByVal value As Boolean)
            tblOCManagement.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde Pannello Organo di Controllo - Altri
    ''' </summary>
    Public Property VisibleOCOther() As Boolean
        Get
            Return tblOCOther.Visible
        End Get
        Set(ByVal value As Boolean)
            tblOCOther.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde Pannello Organo di Controllo - Conferenza dei Sindaci
    ''' </summary>
    Public Property VisibleOCConfSindaci() As Boolean
        Get
            Return tblOCConfSindaci.Visible AndAlso (tblOCConfSindaci.Style.Item("display") <> "none")
        End Get
        Set(ByVal value As Boolean)
            tblOCConfSindaci.Visible = value
            If (value) Then
                tblOCConfSindaci.Style.Remove("display")
            Else
                tblOCConfSindaci.Style.Add("display", "none")
            End If
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde Pannello Organo di Controllo - Collegio sindacale (Checkbox)
    ''' </summary>
    Public Property VisibleOCSupervisoryBoardCheckbox() As Boolean
        Get
            Return pnlOCSupervisoryBoard.Visible
        End Get
        Set(ByVal value As Boolean)
            pnlOCSupervisoryBoard.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde Pannello Organo di Controllo - Conferenza dei Sindaci (Checkbox)
    ''' </summary>
    Public Property VisibleOCConfSindaciCheckbox() As Boolean
        Get
            Return pnlOCConfSindaci.Visible
        End Get
        Set(ByVal value As Boolean)
            pnlOCConfSindaci.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde Pannello Organo di Controllo - Conferenza dei Sindaci (Checkbox)
    ''' </summary>
    Public Property VisibleOCRegionCheckbox() As Boolean
        Get
            Return pnlOCRegion.Visible
        End Get
        Set(ByVal value As Boolean)
            pnlOCRegion.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde Pannello Organo di Controllo - Controllo di Gestione (Checkbox)
    ''' </summary>
    Public Property VisibleOCManagementCheckbox() As Boolean
        Get
            Return pnlOCManagement.Visible
        End Get
        Set(ByVal value As Boolean)
            pnlOCManagement.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde Pannello Organo di Controllo - Corte dei Conti (Checkbox)
    ''' </summary>
    Public Property VisibleOCCorteContiCheckbox() As Boolean
        Get
            Return pnlOCCorteConti.Visible
        End Get
        Set(ByVal value As Boolean)
            pnlOCCorteConti.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde Pannello Organo di Controllo - Altro (Checkbox)
    ''' </summary>
    Public Property VisibleOCOtherCheckbox() As Boolean
        Get
            Return pnlOCOther.Visible
        End Get
        Set(ByVal value As Boolean)
            pnlOCOther.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde Riga del documento della Regione
    ''' </summary>
    Public Property VisibleOCRegionDocument() As Boolean
        Get
            Return trRegionDocument.Visible
        End Get
        Set(ByVal value As Boolean)
            trRegionDocument.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde la sezione DGR della Regione
    ''' </summary>
    Public Property VisibleOCRegionDGR() As Boolean
        Get
            Return tdDGRLabel.Visible AndAlso tdDGRtext.Visible
        End Get
        Set(ByVal value As Boolean)
            tdDGRLabel.Visible = value
            tdDGRtext.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde la sezione di Spedizione alla Regione
    ''' </summary>
    Public Property VisibleOCRegionSpedizione() As Boolean
        Get
            Return trOCRegionSpedizione.Visible
        End Get
        Set(ByVal value As Boolean)
            trOCRegionSpedizione.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde la sezione di Spedizione alla Regione
    ''' </summary>
    Public Property VisibleOCRegionRicezioneEScadenza() As Boolean
        Get
            Return trOCRegionRicezioneEScadenza.Visible
        End Get
        Set(ByVal value As Boolean)
            trOCRegionRicezioneEScadenza.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde la sezione di Spedizione alla Regione
    ''' </summary>
    Public Property VisibleOCRegionRispostaEDGR() As Boolean
        Get
            Return trOCRegionRispostaEDGR.Visible
        End Get
        Set(ByVal value As Boolean)
            trOCRegionRispostaEDGR.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde la sezione di Spedizione alla Regione
    ''' </summary>
    Public Property VisibleOCRegionInvioChiarimenti() As Boolean
        Get
            Return trOCREgionInvioChiarimenti.Visible
        End Get
        Set(ByVal value As Boolean)
            trOCREgionInvioChiarimenti.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde la sezione Note Approvazione della Regione
    ''' </summary>
    Public Property VisibleOCRegionNoteApprovazione() As Boolean
        Get
            Return trOCRegionNoteApprovazione.Visible
        End Get
        Set(ByVal value As Boolean)
            trOCRegionNoteApprovazione.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde la sezione Note Decadimento della Regione
    ''' </summary>
    Public Property VisibleOCRegionNoteDecadimento() As Boolean
        Get
            Return trOCRegionNoteDecadimento.Visible
        End Get
        Set(ByVal value As Boolean)
            trOCRegionNoteDecadimento.Visible = value
        End Set
    End Property


    ''' <summary>
    ''' Interagisce con l'etichetta della data di risposta della Regione
    ''' </summary>
    Public Property RegionResponseDateLabel() As String
        Get
            Return tdRegionResponseDateLabel.InnerText
        End Get
        Set(ByVal value As String)
            tdRegionResponseDateLabel.InnerText = value
        End Set
    End Property

    ''' <summary>
    ''' Interagisce con l'etichetta della data di invio opinione della Regione
    ''' </summary>
    Public Property RegionOpinionText() As String
        Get
            Return tdRegionOpinionText.InnerText
        End Get
        Set(ByVal value As String)
            tdRegionOpinionText.InnerText = value
        End Set
    End Property


    ''' <summary>
    ''' Visualizza/Nasconde Il pannello del protocollo di risposta
    ''' </summary>
    Public Property VisibleResponseProtocol() As Boolean
        Get
            Return tdResponseProtocolLabel.Visible And tdResponseProtocolContent.Visible
        End Get
        Set(ByVal value As Boolean)
            tdResponseProtocolLabel.Visible = value
            tdResponseProtocolContent.Visible = value
        End Set
    End Property

    Public ReadOnly Property ControlRecipientInterop() As uscContattiSel
        Get
            Return uscContactDest
        End Get
    End Property

    Public ReadOnly Property ControlProposerInterop() As uscContattiSel
        Get
            Return uscContactProp
        End Get
    End Property

    Public ReadOnly Property ControlManagerInterop() As uscContattiSel
        Get
            Return uscContactMgr
        End Get
    End Property

    Public ReadOnly Property ControlAssigneeInterop() As uscContattiSel
        Get
            Return uscContactAss
        End Get
    End Property

    Public ReadOnly Property ControlCategory() As uscClassificatore
        Get
            Return uscCategory
        End Get
    End Property

    Public ReadOnly Property RoleProposerEnabled As Boolean
        Get
            If Not _roleProposerEnabled.HasValue AndAlso CurrentResolution IsNot Nothing Then
                _roleProposerEnabled = Facade.ResolutionFacade.IsManagedProperty(PROPOSER_FIELD_DATA_NAME, CurrentResolution.Type.Id, ROLE_PROPOSER_PROPERTY_NAME)
            End If
            Return _roleProposerEnabled.Value
        End Get
    End Property

    Public ReadOnly Property CurrentResolutionsDocumentSeriesItem As IList(Of ResolutionDocumentSeriesItem)
        Get
            If _currentResolutionsDocumentSeriesItem Is Nothing Then
                _currentResolutionsDocumentSeriesItem = Facade.ResolutionDocumentSeriesItemFacade.GetByResolution(CurrentResolution.Id)
            End If
            Return _currentResolutionsDocumentSeriesItem
        End Get
    End Property

    Public ReadOnly Property Action() As String
        Get
            Dim val As String
            If ViewState("Action") Is Nothing Then
                val = HttpContext.Current.Request.QueryString("Action")
                ViewState("Action") = val
            Else
                val = ViewState("Action").ToString()
            End If
            Return val
        End Get
    End Property

    'Contiene la serie documentale associata alla proposta di Atto che si sta inserendo.
    'Utilizzato con parametro ResolutionKindEnabled
    Public Property DraftSeriesItemAdded As IList(Of ResolutionSeriesDraftInsert)
        Get
            If Session("DraftSeriesItemAdded") IsNot Nothing Then
                Return DirectCast(Session("DraftSeriesItemAdded"), IList(Of ResolutionSeriesDraftInsert))
            End If
            Return Nothing
        End Get
        Set(value As IList(Of ResolutionSeriesDraftInsert))
            If value Is Nothing Then
                Session.Remove("DraftSeriesItemAdded")
            Else
                Session("DraftSeriesItemAdded") = value
            End If
        End Set
    End Property

    Public Property CurrentResolutionDocumentSeriesDTO As IList(Of ResolutionChangeDSI)
        Get
            If Session("CurrentResolutionDocumentSeriesDTO") IsNot Nothing Then
                Return DirectCast(Session("CurrentResolutionDocumentSeriesDTO"), IList(Of ResolutionChangeDSI))
            End If
            Return Nothing
        End Get
        Set(value As IList(Of ResolutionChangeDSI))
            If value Is Nothing Then
                Session.Remove("CurrentResolutionDocumentSeriesDTO")
            Else
                Session("CurrentResolutionDocumentSeriesDTO") = value
            End If
        End Set
    End Property

    Public Property DocumentSeriesToRemove As IList(Of ResolutionChangeDSI)
        Get
            If Session("DocumentSeriesToRemove") IsNot Nothing Then
                Return DirectCast(Session("DocumentSeriesToRemove"), IList(Of ResolutionChangeDSI))
            End If
            Return New List(Of ResolutionChangeDSI)
        End Get
        Set(value As IList(Of ResolutionChangeDSI))
            If value Is Nothing Then
                Session.Remove("DocumentSeriesToRemove")
            Else
                Session("DocumentSeriesToRemove") = value
            End If
        End Set
    End Property


    Public ReadOnly Property CurrentSelectedResolutionKind As ResolutionKind
        Get
            Dim kindId As Guid
            If String.IsNullOrEmpty(ddlResolutionKind.SelectedValue) OrElse Not Guid.TryParse(ddlResolutionKind.SelectedValue, kindId) Then
                Return Nothing
            End If
            Return CurrentResolutionKindFacade.GetById(kindId)
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

    Protected Overridable ReadOnly Property CurrentResolutionKindFacade As ResolutionKindFacade
        Get
            If _resolutionKindFacade Is Nothing Then
                _resolutionKindFacade = New ResolutionKindFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _resolutionKindFacade
        End Get
    End Property

    Protected Overridable ReadOnly Property CurrentReslKindDocumentSeriesFacade As ResolutionKindDocumentSeriesFacade
        Get
            If _resolutionKindDocumentSeries Is Nothing Then
                _resolutionKindDocumentSeries = New ResolutionKindDocumentSeriesFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _resolutionKindDocumentSeries
        End Get
    End Property

    Public ReadOnly Property IsAllDocumentSeriesLink As Boolean
        Get
            If CurrentSelectedResolutionKind IsNot Nothing AndAlso Not CurrentResolutionDocumentSeriesDTO.IsNullOrEmpty() Then
                Dim temp As List(Of ResolutionChangeDSI) = CurrentResolutionDocumentSeriesDTO.ToList()
                If DocumentSeriesToRemove IsNot Nothing AndAlso DocumentSeriesToRemove.Count() > 0 Then
                    temp = temp.Where(Function(x) Not DocumentSeriesToRemove.Any(Function(y) y.DocumentSeriesId = x.DocumentSeriesId)).ToList()
                End If
                Return CurrentSelectedResolutionKind.ResolutionKindDocumentSeries.All(Function(x) temp.Any(Function(y) y.DocumentSeriesId = x.DocumentSeries.Id AndAlso y.DSItemId.HasValue))
            End If
            Return True
        End Get
    End Property

    Public ReadOnly Property IsNullContainer As Boolean
        Get
            If String.IsNullOrEmpty(ddlIdContainer.SelectedValue) Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    Public ReadOnly Property IsVisibleContainer As Boolean
        Get
            Return ddlIdContainer.Visible
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not Page.IsPostBack Then

            lblObjectPrivacy.Text = ResolutionEnv.ResolutionObjectPrivacyLabel
            lblObjectPrivacyDetail.Text = String.Concat(ResolutionEnv.ResolutionObjectPrivacyLabel, ":")
            If ResolutionEnv.ResolutionKindEnabled AndAlso
                CurrentResolutionStatus <> WorkflowStep.PUBBLICAZIONE AndAlso
                CurrentResolutionStatus <> WorkflowStep.RITIRO AndAlso
                CurrentResolutionStatus <> WorkflowStep.ESECUTIVA Then

                CurrentResolutionDocumentSeriesDTO = Nothing
                pnlResolutionKind.Visible = True
                If Action = "FromSeries" AndAlso CurrentResolutionModel IsNot Nothing Then
                    BindPageFromModel()
                Else
                    LoadAmministrazioneTrasparenteJustRecorder()
                End If
                LoadAmministrazioneTrasparente()
            Else
                pnlResolutionKind.Visible = False
            End If

        End If
    End Sub

    Protected Sub ResolutionAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arguments As String() = Split(e.Argument, "|")
        Select Case arguments(0)
            Case "createNewDraftSeries"
                NewDraftSeriesAction(Convert.ToInt32(arguments(1)))
            Case "goToDraftSeries"
                Dim parameters As String = String.Format("IdDocumentSeriesItem={0}&Action={1}&PreviousPage={2}&Type=Series", arguments(1), DocumentSeriesAction.View, HttpUtility.UrlEncode(Page.Request.Url.AbsoluteUri))
                Response.Redirect(String.Format("~/Series/Item.aspx?{0}", CommonShared.AppendSecurityCheck(parameters)))
            Case "draftSeriesConnect"
                If DraftSeriesItemAdded Is Nothing Then
                    DraftSeriesItemAdded = New List(Of ResolutionSeriesDraftInsert)
                End If

                If DraftSeriesItemAdded.Any(Function(x) x.IdSeriesItem.Equals(Convert.ToInt32(arguments(1)))) Then
                    AjaxAlert("L'elemento selezionato è già stato aggiunto.")
                    Exit Sub
                End If
                DraftSeriesItemAdded.Add(New ResolutionSeriesDraftInsert() With {.IdSeries = Convert.ToInt32(arguments(2)), .IdSeriesItem = Convert.ToInt32(arguments(1))})

                dgvResolutionKindDocumentSeries.DataSource = EvaluateKindDocumentSeries(DraftSeriesItemAdded)
                dgvResolutionKindDocumentSeries.DataBind()

            Case "removeDraftLink"
                If arguments(1) IsNot Nothing Then
                    Dim documentSeriesItemToRemove As IList(Of ResolutionChangeDSI) = DocumentSeriesToRemove
                    Dim itemToRemove As ResolutionSeriesDraftInsert = DraftSeriesItemAdded.Single(Function(x) x.IdSeriesItem.Equals(Convert.ToInt32(arguments(1))))
                    DraftSeriesItemAdded.Remove(itemToRemove)
                    Dim temp As ResolutionChangeDSI = CurrentResolutionDocumentSeriesDTO.SingleOrDefault(Function(x) x.ResolutionDSItemId.HasValue AndAlso x.DSItemId.HasValue AndAlso x.DSItemId.Value = itemToRemove.IdSeriesItem)
                    If temp IsNot Nothing AndAlso Not documentSeriesItemToRemove.Any(Function(x) x.DocumentSeriesId = temp.DocumentSeriesId) Then
                        documentSeriesItemToRemove.Add(temp)
                    End If
                    DocumentSeriesToRemove = documentSeriesItemToRemove
                End If
                dgvResolutionKindDocumentSeries.DataSource = EvaluateKindDocumentSeries(DraftSeriesItemAdded)
                dgvResolutionKindDocumentSeries.DataBind()
                AjaxManager.ResponseScripts.Add("ResponseEnd();")
            Case "correctDraftLink"
                Dim documentSeries As DocumentSeriesItem = Facade.DocumentSeriesItemFacade.GetById(Convert.ToInt32(arguments(1)))
                If documentSeries.DocumentSeries.Id.Equals(DocSuiteContext.Current.ProtocolEnv.BandiGaraDocumentSeriesId) Then
                    BindModelFromPage()
                    Response.Redirect(String.Format("~/Series/Item.aspx?Type=Series&Action={0}&IdDocumentSeriesItem={1}&IdSeries={2}&IdResolution={3}", DocumentSeriesAction.FromResolutionKindUpdate, arguments(1), documentSeries.DocumentSeries.Id, CurrentResolution.Id))
                End If
                Response.Redirect(String.Format("~/Series/Item.aspx?Type=Series&Action={0}&IdDocumentSeriesItem={1}", DocumentSeriesAction.Edit, arguments(1)))
        End Select
    End Sub

    Private Sub LoadAmministrazioneTrasparenteJustRecorder()
        If ResolutionEnv.ResolutionKindEnabled Then
            pnlResolutionKind.Visible = True
            ddlResolutionKindValidator.Enabled = True
            DraftSeriesItemAdded = LoadDraftItem()
            CurrentResolutionModel = LoadResolutionModel()
        End If
    End Sub
    Private Sub LoadAmministrazioneTrasparente()
        If ResolutionEnv.ResolutionKindEnabled Then
            BindResolutionKind()

            If CurrentResolutionModel.ResolutionKind.HasValue Then
                ddlResolutionKind.SelectedValue = CurrentResolutionModel.ResolutionKind.Value.ToString()

                If CurrentSelectedResolutionKind IsNot Nothing AndAlso (Not CurrentSelectedResolutionKind.ResolutionKindDocumentSeries.IsNullOrEmpty() OrElse Not CurrentResolutionsDocumentSeriesItem.IsNullOrEmpty) Then
                    Call DdlResolutionKind_SelectedIndexChanged(ddlResolutionKind, New EventArgs())
                End If
            End If
        End If
    End Sub
    Private Sub ddlStatus_SelectedIndexChangedEvent(ByVal sender As Object, ByVal e As EventArgs)
        txtChangedReason.Enabled = (ddlStatus.SelectedValue <> "0")
        If ddlStatus.SelectedValue = "0" Then
            txtChangedReason.Text = String.Empty
        End If
    End Sub

    Private Sub rdpConfirmDate_SelectedDateChangedEvent(ByVal sender As Object, ByVal e As SelectedDateChangedEventArgs)
        If rdpConfirmDate.SelectedDate.HasValue Then
            rdpWaitDate.SelectedDate = DateAdd(DateInterval.Day, 39, rdpConfirmDate.SelectedDate.Value)
        End If
    End Sub

    Private Sub rdpRegionConfirmDate_SelectedDateChangedEvent(ByVal sender As Object, ByVal e As SelectedDateChangedEventArgs)
        If rdpRegionConfirmDate.SelectedDate.HasValue Then
            rdpRegionWaitDate.SelectedDate = DateAdd(DateInterval.Day, 39, rdpRegionConfirmDate.SelectedDate.Value)
        End If
    End Sub

    Private Sub ddlIdContainer_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlIdContainer.SelectedIndexChanged
        VisibleObjectPrivacy = False
        If String.IsNullOrEmpty(ddlIdContainer.SelectedValue()) Then
            Exit Sub
        End If

        Dim container As Container = Facade("ReslDB").ContainerFacade.GetById(Integer.Parse(ddlIdContainer.SelectedValue), False, "ReslDB")
        If (container IsNot Nothing) AndAlso container.Privacy.HasValue AndAlso container.Privacy.Value = 1 Then
            VisibleObjectPrivacy = True
            txtObjectPrivacy.Text = txtNote.Text
        End If
    End Sub

    Private Sub chkOCRegion_CheckedChangedEvent(ByVal sender As Object, ByVal e As EventArgs)
        If Not chkOCRegion.Checked Then
            VisibleOCRegion = False
            Exit Sub
        End If
        Dim changeableData As String = String.Empty
        If Facade.TabWorkflowFacade.GetChangeableData(CurrentResolution.Id, CurrentResolution.WorkflowType, 0, changeableData) Then
            VisibleOCRegion = Facade.ResolutionFacade.ManagedDataTest(CurrentResolution, "OCData", "REG", changeableData, ".REG.") AndAlso CurrentResolutionRight.IsExecutive
        End If
    End Sub

    Private Sub chkOCSupervisoryBoard_CheckedChangedEvent(ByVal sender As Object, ByVal e As EventArgs)
        If Not DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("AUSL-PC") Then
            VisibleOCSupervisoryBoardExtra = chkOCSupervisoryBoard.Checked
            Exit Sub
        End If
        'EF 20120120 Disattiva tutto il blocco se viene Deselezionato
        If Not chkOCSupervisoryBoard.Checked Then
            VisibleOCSupervisoryBoard = False
            Exit Sub
        End If

        Dim changeableData As String = String.Empty
        If Facade.TabWorkflowFacade.GetChangeableData(CurrentResolution.Id, CurrentResolution.WorkflowType, 0, changeableData) Then
            VisibleOCSupervisoryBoard = Facade.ResolutionFacade.ManagedDataTest(CurrentResolution, "OCData", "CS", changeableData, ".CS.") AndAlso CurrentResolutionRight.IsExecutive
        End If

    End Sub

    Private Sub chkOCOther_CheckedChangedEvent(ByVal sender As Object, ByVal e As EventArgs)
        If Not chkOCOther.Checked Then
            VisibleOCOther = False
            Exit Sub
        End If
        Dim changeableData As String = String.Empty
        If Facade.TabWorkflowFacade.GetChangeableData(CurrentResolution.Id, CurrentResolution.WorkflowType, 0, changeableData) Then
            VisibleOCOther = Facade.ResolutionFacade.ManagedDataTest(CurrentResolution, "OCData", "ALTRO", changeableData, ".ALTRO.") AndAlso CurrentResolutionRight.IsExecutive
        End If
    End Sub

    Private Sub chkOCManagement_CheckedChangedEvent(ByVal sender As Object, ByVal e As EventArgs)
        If Not chkOCManagement.Checked Then
            VisibleOCManagement = False
            Exit Sub
        End If
        Dim changeableData As String = String.Empty
        If Facade.TabWorkflowFacade.GetChangeableData(CurrentResolution.Id, CurrentResolution.WorkflowType, 0, changeableData) Then
            VisibleOCManagement = Facade.ResolutionFacade.ManagedDataTest(CurrentResolution, "OCData", "GEST", changeableData, ".GEST.") AndAlso CurrentResolutionRight.IsExecutive
        End If
    End Sub

    Private Sub chkOCConfSindaci_CheckedChangedEvent(ByVal sender As Object, ByVal e As EventArgs)
        If Not chkOCConfSindaci.Checked Then
            VisibleOCConfSindaci = False
            Exit Sub
        End If
        Dim changeableData As String = String.Empty
        If Facade.TabWorkflowFacade.GetChangeableData(CurrentResolution.Id, CurrentResolution.WorkflowType, 0, changeableData) Then
            VisibleOCConfSindaci = Facade.ResolutionFacade.ManagedDataTest(CurrentResolution, "OCData", "CONFSIND", changeableData, ".CONFSIND.") AndAlso CurrentResolutionRight.IsExecutive
        End If
    End Sub

    Private Sub chkOCCorteConti_CheckedChangedEvent(ByVal sender As Object, ByVal e As EventArgs)
        If Not chkOCCorteConti.Checked Then
            VisibleCorteDeiConti = False
            Exit Sub
        End If
        Dim changeableData As String = String.Empty
        If Facade.TabWorkflowFacade.GetChangeableData(CurrentResolution.Id, CurrentResolution.WorkflowType, 0, changeableData) Then
            VisibleCorteDeiConti = Facade.ResolutionFacade.ManagedDataTest(CurrentResolution, "OCData", "CC", changeableData, ".CC.") AndAlso CurrentResolutionRight.IsExecutive
        End If
    End Sub

    Protected Sub DgvResolutionKindDocumentSeries_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles dgvResolutionKindDocumentSeries.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim dto As ResolutionChangeDSI = DirectCast(e.Item.DataItem, ResolutionChangeDSI)
        DirectCast(e.Item.FindControl("lblDocumentSeriesName"), Label).Text = dto.DocumentSeriesName

        Dim btnDraft As Button = DirectCast(e.Item.FindControl("btnDocumentSeriesDraft"), Button)
        Dim btnConnect As Button = DirectCast(e.Item.FindControl("btnDocumentSeriesConnect"), Button)
        Dim btnRemoveLink As ImageButton = DirectCast(e.Item.FindControl("btnRemoveLink"), ImageButton)
        Dim seriesLink As HyperLink = DirectCast(e.Item.FindControl("documentSeriesLink"), HyperLink)
        Dim btnCorrect As ImageButton = DirectCast(e.Item.FindControl("btnCorrectSeries"), ImageButton)
        btnCorrect.Visible = False
        btnDraft.Visible = False
        btnConnect.Visible = False
        seriesLink.Visible = False
        btnRemoveLink.Visible = False

        If DraftSeriesItemAdded IsNot Nothing AndAlso DraftSeriesItemAdded.Any(Function(x) x.IdSeries.Equals(dto.DocumentSeriesId)) Then
            btnRemoveLink.Visible = True
            seriesLink.Visible = True
            btnCorrect.Visible = True
            Dim idSeriesItem As Integer = DraftSeriesItemAdded.Single(Function(x) x.IdSeries.Equals(dto.DocumentSeriesId)).IdSeriesItem
            Dim seriesItem As DocumentSeriesItem = Facade.DocumentSeriesItemFacade.GetById(idSeriesItem)
            seriesLink.Text = String.Format("N. {0} del {1}", seriesItem.Id, seriesItem.RegistrationDate.ToLocalTime().DateTime.ToString("dd/MM/yyyy"))
            seriesLink.Attributes.Add("onclick", String.Format("GoToDraftSeries('{0}')", seriesItem.Id))

            btnRemoveLink.OnClientClick = String.Format("return RemoveDraftLink('{0}');", seriesItem.Id)
            btnCorrect.OnClientClick = String.Format("return CorrectDraftLink('{0}');", seriesItem.Id)
        Else
            btnDraft.Visible = True
            btnDraft.OnClientClick = String.Format("return CreateNewDraftSeries('{0}');", dto.DocumentSeriesId)

            btnConnect.Visible = True
            btnConnect.OnClientClick = String.Format("return {1}_OpenDraftSeriesConnectWindow('{0}','{2}');", dto.DocumentSeriesId, ID, dto.DSItemId)
        End If
    End Sub

    Protected Sub DdlResolutionKind_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlResolutionKind.SelectedIndexChanged

        pnlAmmTrasp.Visible = False
        If CurrentSelectedResolutionKind IsNot Nothing AndAlso (Not CurrentSelectedResolutionKind.ResolutionKindDocumentSeries.IsNullOrEmpty() OrElse Not CurrentResolutionsDocumentSeriesItem.IsNullOrEmpty) Then
            pnlAmmTrasp.Visible = True
            Dim results As ICollection(Of ResolutionChangeDSI) = EvaluateKindDocumentSeries()
            If results IsNot Nothing Then
                dgvResolutionKindDocumentSeries.DataSource = results
                dgvResolutionKindDocumentSeries.DataBind()
            End If
        End If
    End Sub

    Protected Sub uscSubCategory_CategoryAdding(sender As Object, args As EventArgs) Handles uscSubCategory.CategoryAdding
        Select Case CurrentResolutionStatus
            Case WorkflowStep.PROPOSTA
                uscSubCategory.FromDate = CurrentResolution.ProposeDate
            Case WorkflowStep.ADOZIONE,
                 WorkflowStep.DOCUMENTO_ADOZIONE
                uscSubCategory.FromDate = CurrentResolution.AdoptionDate
            Case WorkflowStep.PUBBLICAZIONE
                uscSubCategory.FromDate = CurrentResolution.PublishingDate
            Case WorkflowStep.RITIRO
                uscSubCategory.FromDate = CurrentResolution.WebRevokeDate
            Case WorkflowStep.ESECUTIVA,
                 WorkflowStep.FrontalinoEsecutiva
                uscSubCategory.FromDate = CurrentResolution.EffectivenessDate
        End Select
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(chkOCSupervisoryBoard, pnlOrganoControllo)
        AjaxManager.AjaxSettings.AddAjaxSetting(chkOCRegion, pnlOrganoControllo)
        AjaxManager.AjaxSettings.AddAjaxSetting(chkOCManagement, pnlOrganoControllo)
        AjaxManager.AjaxSettings.AddAjaxSetting(chkOCOther, pnlOrganoControllo)
        AjaxManager.AjaxSettings.AddAjaxSetting(chkOCConfSindaci, pnlOrganoControllo)
        AjaxManager.AjaxSettings.AddAjaxSetting(chkOCCorteConti, pnlOrganoControllo)

        If VisibleOC Then
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscOCDocument.TreeViewControl)
        End If
        If pnlProponente.Visible = True Then
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, txtAlternativeProp)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscContactProp)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscRoleProposer)
        End If

        If ResolutionEnv.ResolutionKindEnabled Then
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlAmmTrasp)
            AjaxManager.AjaxSettings.AddAjaxSetting(dgvResolutionKindDocumentSeries, dgvResolutionKindDocumentSeries)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlResolutionKind, pnlAmmTrasp)
            AjaxManager.AjaxSettings.AddAjaxSetting(windowSelDraftSeries, windowSelDraftSeries)
        End If
        AddHandler AjaxManager.AjaxRequest, AddressOf ResolutionAjaxRequest
    End Sub

    Public Sub BindType()
        rblType.Items.Add(Facade.ResolutionTypeFacade.DeliberaCaption())
        rblType.Items(0).Value = ResolutionType.IdentifierDelibera
        rblType.Items.Add(Facade.ResolutionTypeFacade.DeterminaCaption())
        rblType.Items(1).Value = ResolutionType.IdentifierDetermina
        Select Case CurrentResolution.Type.Id
            Case ResolutionType.IdentifierDetermina
                rblType.Items(0).Selected = True
            Case ResolutionType.IdentifierDelibera
                rblType.Items(1).Selected = True
        End Select
    End Sub

    Public Sub BindCategory()
        If CurrentResolution.Category IsNot Nothing Then
            uscSubCategory.CategoryID = CurrentResolution.Category.Id
            If CurrentResolution.SubCategory IsNot Nothing Then
                uscSubCategory.SubCategory = CurrentResolution.SubCategory
                uscCategory.DataSource.Add(CurrentResolution.SubCategory)
            Else
                uscCategory.DataSource.Add(CurrentResolution.Category)
            End If
            uscCategory.DataBind()
        End If
    End Sub

    Public Sub BindObject()
        txtObject.Text = CurrentResolution.ResolutionObject
    End Sub

    Public Sub BindNote()
        txtNote.Text = CurrentResolution.Note
    End Sub

    Public Sub BindImmediatelyExecutive()
        chkExecutive.Checked = CurrentResolution.ImmediatelyExecutive.HasValue AndAlso CurrentResolution.ImmediatelyExecutive.Value
    End Sub

    Public Sub BindEconomicData()
        txtPosition.Text = CurrentResolution.Position
        rdpValidityDateFrom.SelectedDate = CurrentResolution.ValidityDateFrom
        rdpValidityDateTo.SelectedDate = CurrentResolution.ValidityDateTo

        Dim bidTypeList As IList(Of BidType) = Facade.BidTypeFacade.GetAll()
        WebUtils.ObjDropDownListAdd(ddlBidType, "", "")
        For Each bidType As BidType In bidTypeList
            ddlBidType.Items.Add(New ListItem(String.Format("{0} - {1}", bidType.Acronym, bidType.Description), bidType.Id.ToString()))
            If bidType.Equals(CurrentResolution.BidType) Then
                ddlBidType.SelectedIndex = ddlBidType.Items.Count - 1
            End If
        Next
        txtSupplierCode.Text = CurrentResolution.SupplierCode
        txtSupplierDescription.Text = CurrentResolution.SupplierDescription
    End Sub

    Public Sub BindOCData()
        rdpWarningDate.SelectedDate = CurrentResolution.WarningDate
        txtWarningProt.Text = CurrentResolution.WarningProtocol
        rdpConfirmDate.SelectedDate = CurrentResolution.ConfirmDate
        txtConfirmProt.Text = CurrentResolution.ConfirmProtocol
        rdpWaitDate.SelectedDate = CurrentResolution.WaitDate
        rdpResponseDate.SelectedDate = CurrentResolution.ResponseDate
        txtResponseProt.Text = CurrentResolution.ResponseProtocol

        Dim controllerStatusList As IList(Of ControllerStatusResolution) = Facade.ControllerStatusResolutionFacade.GetAll()
        ddlOCComment.Items.Add(New ListItem("", ""))
        For Each controller As ControllerStatusResolution In controllerStatusList
            ddlOCComment.Items.Add(New ListItem(String.Format("{0} - {1}", controller.Acronym, controller.Description), controller.Id.ToString()))
            If controller.Equals(CurrentResolution.ControllerStatus) Then
                ddlOCComment.SelectedIndex = ddlOCComment.Items.Count - 1
            End If
        Next

        If CurrentResolution.File.IdControllerFile.HasValue Then
            Dim node As New RadTreeNode("Risposta Organo Controllo", CurrentResolution.File.IdControllerFile.Value.ToString())
            node.ImageUrl = "../Resl/Images/FileOC.gif"
            uscOCDocument.TreeViewControl.Nodes(0).Nodes.Add(node)
        End If
        txtOCOpinion.Text = CurrentResolution.ControllerOpinion
    End Sub

    Public Sub BindContactRecipients()
        For Each contact As ResolutionContact In CurrentResolution.ResolutionContactsRecipients
            uscContactDest.DataSource.Add(New ContactDTO(contact.Contact, ContactDTO.ContactType.Address))
        Next
        uscContactDest.DataBind()
    End Sub

    Public Sub BindContactAlternativeRecipients()
        txtAlternativeDest.Text = CurrentResolution.AlternativeRecipient
    End Sub

    Public Sub BindContactProposer()
        For Each contact As ResolutionContact In CurrentResolution.ResolutionContactProposers
            uscContactProp.DataSource.Add(New ContactDTO(contact.Contact, ContactDTO.ContactType.Address))
        Next
        uscContactProp.DataBind()
    End Sub

    Public Sub BindRoleProposer()
        If CurrentResolution.RoleProposer Is Nothing Then
            Exit Sub
        End If

        uscRoleProposer.SourceRoles = New List(Of Role) From {CurrentResolution.RoleProposer}
        uscRoleProposer.DataBind()
    End Sub

    Public Sub BindContactAlternativeProposer()
        txtAlternativeProp.Text = CurrentResolution.AlternativeProposer
    End Sub

    Public Sub BindContactAssignee()
        For Each contact As ResolutionContact In CurrentResolution.ResolutionContactsAssignees
            uscContactAss.DataSource.Add(New ContactDTO(contact.Contact, ContactDTO.ContactType.Address))
        Next
        uscContactAss.DataBind()
    End Sub

    Public Sub BindContactAlternativeAssignee()
        txtAlternativeAss.Text = CurrentResolution.AlternativeAssignee
    End Sub

    Public Sub BindContactManager()
        For Each contact As ResolutionContact In CurrentResolution.ResolutionContactsManagers
            uscContactMgr.DataSource.Add(New ContactDTO(contact.Contact, ContactDTO.ContactType.Address))
        Next
        uscContactMgr.DataBind()
    End Sub

    Public Sub BindContactAlternativeManager()
        txtAlternativeMgr.Text = CurrentResolution.AlternativeManager
    End Sub

    Public Sub BindStatus(ByRef statusList As IList(Of ResolutionStatus))
        ddlStatus.Enabled = (CurrentResolution.Status.Id = ResolutionStatusId.Attivo)
        txtChangedReason.Enabled = (CurrentResolution.Status.Id = ResolutionStatusId.Attivo)
        txtChangedReason.Text = CurrentResolution.LastChangedReason

        For Each status As ResolutionStatus In statusList
            ddlStatus.Items.Add(New ListItem(status.Description, status.Id.ToString()))
            If status.Equals(CurrentResolution.Status) Then
                ddlStatus.SelectedIndex = ddlStatus.Items.Count - 1
            End If
        Next
        ddlStatus_SelectedIndexChangedEvent(ddlStatus, New EventArgs())
    End Sub

    ''' <summary>  </summary>
    ''' <remarks> DG - 2011-10-11 - 1082: contenitore unico per delibere </remarks>
    Public Sub BindContainer()
        ddlIdContainer.Items.Clear()

        Dim useContainerResolutionType As Boolean = DocSuiteContext.Current.ResolutionEnv.UseContainerResolutionType

        If useContainerResolutionType Then
            Dim crtf As New ContainerResolutionTypeFacade
            Dim containerList As IList(Of ContainerResolutionType) = crtf.GetAllowedContainers(_resl.Type.Id, 1, ResolutionRightPositions.Insert)

            Dim containerId As Integer
            Dim container As ContainerResolutionType

            If containerList.Count > 0 Then
                WebUtils.ObjDropDownListAdd(ddlIdContainer, "", "")
                For Each container In containerList
                    If containerId = container.Id.idContainer Then
                        Continue For
                    End If

                    WebUtils.ObjDropDownListAdd(ddlIdContainer, container.container.Name, container.Id.idContainer.ToString())
                    containerId = container.Id.idContainer
                    If container.container.Equals(CurrentResolution.Container) Then
                        ddlIdContainer.SelectedIndex = ddlIdContainer.Items.Count - 1
                    End If

                Next
            End If

            If ddlIdContainer.Items.FindByValue(_resl.Container.Id.ToString()) Is Nothing Then
                WebUtils.ObjDropDownListAdd(ddlIdContainer, _resl.Container.Name, _resl.Container.Id.ToString())
                ddlIdContainer.SelectedIndex = ddlIdContainer.Items.Count - 1
            End If
        Else
            Dim containerList As IList(Of Container) = Facade.ContainerFacade.GetContainers(DSWEnvironment.Resolution, ResolutionRightPositions.Insert, True)

            For Each container As Container In containerList
                ddlIdContainer.Items.Add(New ListItem(container.Name, container.Id.ToString()))
                If container.Equals(CurrentResolution.Container) Then
                    ddlIdContainer.SelectedIndex = ddlIdContainer.Items.Count - 1
                End If
            Next
        End If

        ddlIdContainer_SelectedIndexChanged(ddlIdContainer, New EventArgs())
    End Sub

    Public Sub BindPubblication()
        ckbPublication.Checked = CurrentResolution.CheckPublication
    End Sub

    Public Sub BindObjectPrivacy()
        txtObjectPrivacy.Text = CurrentResolution.ResolutionObjectPrivacy
    End Sub

    Public Sub BindProposerProtocolLink()
        uscProposerProtocolLink.AddProtocolAsLink(CurrentResolution.ProposerProtocolLink)
    End Sub

    Public Sub BindPublicationLetterProtocolLink()
        uscPublicationLetterProtocolLink.AddProtocolAsLink(CurrentResolution.PublishingProtocolLink)
    End Sub

    Public Sub BindOCList()
        chkOCSupervisoryBoard.Checked = CurrentResolution.OCSupervisoryBoard.GetValueOrDefault(False)
        chkOCSupervisoryBoard_CheckedChangedEvent(chkOCSupervisoryBoard, New EventArgs())
        chkOCRegion.Checked = CurrentResolution.OCRegion.GetValueOrDefault(False)
        chkOCRegion_CheckedChangedEvent(chkOCRegion, New EventArgs())
        chkOCManagement.Checked = CurrentResolution.OCManagement.GetValueOrDefault(False)
        chkOCManagement_CheckedChangedEvent(chkOCManagement, New EventArgs())
        chkOCOther.Checked = CurrentResolution.OCOther.GetValueOrDefault(False)
        chkOCOther_CheckedChangedEvent(chkOCOther, New EventArgs())
        chkOCCorteConti.Checked = CurrentResolution.OCCorteConti.GetValueOrDefault(False)
        chkOCCorteConti_CheckedChangedEvent(chkOCCorteConti, New EventArgs())
        chkOCConfSindaci.Checked = CurrentResolution.OCManagement.GetValueOrDefault(False)
        chkOCConfSindaci_CheckedChangedEvent(chkOCConfSindaci, New EventArgs())
        If DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("AUSL-PC") Then
            chkOCSupervisoryBoard.Enabled = False
        End If
    End Sub

    Public Sub BindOCSupervisoryBoard()
        rdpSupervisoryBoardWarningDate.SelectedDate = CurrentResolution.SupervisoryBoardWarningDate
        uscSupervisoryBoardProtocolLink.AddProtocolAsLink(CurrentResolution.SupervisoryBoardProtocolLink)
        If CurrentResolution.File.IdSupervisoryBoardFile.HasValue Then
            Dim node As New RadTreeNode("Risposta Organo Controllo", CurrentResolution.File.IdSupervisoryBoardFile.Value.ToString())
            node.ImageUrl = "../Resl/Images/FileOC.gif"
            uscOCSupervisoryBoardDocument.TreeViewControl.Nodes(0).Nodes.Add(node)
        End If
        txtOCSupervisoryBoardOpinion.Text = CurrentResolution.SupervisoryBoardOpinion
    End Sub

    Public Sub BindCorteDeiConti()
        rdpCorteDeiContiWarningDate.SelectedDate = CurrentResolution.CorteDeiContiWarningDate
        uscCorteDeiContiProtocolLink.AddProtocolAsLink(CurrentResolution.CorteDeiContiProtocolLink)
    End Sub

    Public Sub BindOCConfSindaci()
        rdpConfSindaciWarningDate.SelectedDate = CurrentResolution.ManagementWarningDate
        uscConfSindaciProtocolLink.AddProtocolAsLink(CurrentResolution.ManagementProtocolLink)
    End Sub

    Public Sub BindOCRegion()
        rdpRegionWarningDate.SelectedDate = CurrentResolution.WarningDate
        rdpRegionConfirmDate.SelectedDate = CurrentResolution.ConfirmDate
        rdpRegionWaitDate.SelectedDate = CurrentResolution.WaitDate
        rdpRegionResponseDate.SelectedDate = CurrentResolution.ResponseDate
        uscRegionProtocolLink.AddProtocolAsLink(CurrentResolution.RegionProtocolLink)

        Dim controllerStatusList As IList(Of ControllerStatusResolution) = Facade.ControllerStatusResolutionFacade.GetAll()
        For Each controller As ControllerStatusResolution In controllerStatusList
            ddlRegionComment.Items.Add(New ListItem(String.Format("{0} - {1}", controller.Acronym, controller.Description), controller.Id.ToString()))
            If controller.Equals(CurrentResolution.ControllerStatus) Then
                ' ddlRegionComment.SelectedIndex = ddlOCComment.Items.Count - 1
                ' AJG 20101215: credo sia meglio questa riga...
                ddlRegionComment.SelectedIndex = ddlRegionComment.Items.Count - 1
            End If
        Next
        ddlRegionComment.Items.Insert(0, "")

        If CurrentResolution.File.IdControllerFile.HasValue Then
            Dim node As New RadTreeNode("Risposta Organo Controllo", CurrentResolution.File.IdControllerFile.Value.ToString())
            node.ImageUrl = "../Resl/Images/FileOC.gif"
            uscOCRegionDocument.TreeViewControl.Nodes(0).Nodes.Add(node)
        End If

        txtRegionOpinion.Text = CurrentResolution.ControllerOpinion
        txtDeclineNote.Text = CurrentResolution.DeclineNote
        txtApprovalNote.Text = CurrentResolution.ApprovalNote
        txtDGR.Text = CurrentResolution.DGR

        'Piacenza
        If (tdResponseProtocolContent.Visible) Then uscRegionResponseProtocol.AddProtocolAsLink(CurrentResolution.ResponseProtocol)
        If (VisibleOCRegionInvioChiarimenti) Then
            rdpRegionInvioChiarimenti.SelectedDate = CurrentResolution.ConfirmDate
            uscRegionInvioChiarimentiProtocolLink.AddProtocolAsLink(CurrentResolution.ConfirmProtocol)
        End If
    End Sub

    Public Sub BindOCManagement()
        rdpManagementWarningDate.SelectedDate = CurrentResolution.ManagementWarningDate
        uscManagementProtocolLink.AddProtocolAsLink(CurrentResolution.ManagementProtocolLink)
    End Sub

    Public Sub BindOCOther()
        txtOCOtherDescription.Text = CurrentResolution.OtherDescription
    End Sub

    ''' <summary> Verifica se il campo Oggetto è valido per la modifica </summary>
    ''' <param name="errorMessage">Messaggio di errore restituito</param>
    Public Function ValidateObject(ByRef errorMessage As String) As Boolean
        If Len(txtObject.Text) > txtObject.MaxLength Then
            errorMessage = "Impossibile salvare.\n\nIl campo Oggetto ha superato i caratteri disponibili."
            Return False
        End If
        Return True
    End Function

    ''' <summary> Verifica se il campo Oggetto Privacy è valido per la modifica </summary>
    ''' <param name="errorMessage">Messaggio di errore restituito</param>
    Public Function ValidateObjectPrivacy(ByRef errorMessage As String) As Boolean
        If Len(txtObjectPrivacy.Text) > txtObjectPrivacy.MaxLength Then
            errorMessage = String.Format("Impossibile salvare.\n\nIl campo {0} ha superato i caratteri disponibili.", ResolutionEnv.ResolutionObjectPrivacyLabel)
            Return False
        End If
        Return True
    End Function

    ''' <summary> Verifica se il campo Note è valido per la modifica </summary>
    ''' <param name="errorMessage">Messaggio di errore restituito</param>
    Public Function ValidateNote(ByRef errorMessage As String) As Boolean
        If Len(txtObject.Text) > txtNote.MaxLength Then
            errorMessage = "Impossibile salvare.\n\nIl campo Note ha superato i caratteri disponibili."
            Return False
        End If
        Return True
    End Function

    Public Function GetChangedObjectData(Optional ByVal regionOc As Boolean = False) As Resolution
        Dim resl As New Resolution()

        'Type
        If Not String.IsNullOrEmpty(rblType.SelectedValue) Then
            resl.Type = Facade.ResolutionTypeFacade.GetById(Short.Parse(rblType.SelectedValue))
        End If

        'SubCategory
        resl.SubCategory = uscSubCategory.SelectedCategories.FirstOrDefault()

        'Object
        resl.ResolutionObject = txtObject.Text

        'Note
        resl.Note = txtNote.Text

        'ImmediatelyExecutive
        resl.ImmediatelyExecutive = chkExecutive.Checked

        'EconomicData
        resl.Position = txtPosition.Text
        resl.ValidityDateFrom = rdpValidityDateFrom.SelectedDate
        resl.ValidityDateTo = rdpValidityDateTo.SelectedDate
        If Not String.IsNullOrEmpty(ddlBidType.SelectedValue) Then
            resl.BidType = Facade.BidTypeFacade.GetById(Integer.Parse(ddlBidType.SelectedValue))
        End If
        resl.SupplierCode = txtSupplierCode.Text
        resl.SupplierDescription = txtSupplierDescription.Text

        'Contact: Recipients
        For Each contact As ContactDTO In uscContactDest.GetContacts(False)
            resl.AddRecipient(contact.Contact)
        Next
        resl.AlternativeRecipient = txtAlternativeDest.Text

        'Proposer
        If Not RoleProposerEnabled Then
            'Contact: Porposer
            For Each contact As ContactDTO In uscContactProp.GetContacts(False)
                resl.AddProposer(contact.Contact)
            Next
            resl.AlternativeProposer = txtAlternativeProp.Text
        Else
            Dim roles As IList(Of Role) = uscRoleProposer.GetRoles()
            If roles.Any() Then
                resl.AlternativeProposer = roles.First().Name
                resl.RoleProposer = roles.First()
            End If
        End If

        'Contact: Assignee
        For Each contact As ContactDTO In uscContactAss.GetContacts(False)
            resl.AddAssignee(contact.Contact)
        Next
        resl.AlternativeAssignee = txtAlternativeAss.Text

        'Contact: Manager
        For Each contact As ContactDTO In uscContactMgr.GetContacts(False)
            resl.AddManager(contact.Contact)
        Next
        resl.AlternativeManager = txtAlternativeMgr.Text

        'Status
        If Not String.IsNullOrEmpty(ddlStatus.SelectedValue) Then
            resl.Status = Facade.ResolutionStatusFacade.GetById(Short.Parse(ddlStatus.SelectedValue))
        End If
        resl.LastChangedDate = DateTimeOffset.UtcNow
        resl.LastChangedReason = txtChangedReason.Text

        'Container
        If Not String.IsNullOrEmpty(ddlIdContainer.SelectedValue) Then
            resl.Container = Facade.ContainerFacade.GetById(Integer.Parse(ddlIdContainer.SelectedValue), False, "ReslDB")
        End If

        'Publication
        resl.CheckPublication = ckbPublication.Checked

        'Service
        resl.ProposerProtocolLink = uscProposerProtocolLink.GetFirstProtocolLink()

        'Protocollo lettera di pubblicazione
        If uscPublicationLetterProtocolLink.Visible Then
            resl.PublishingProtocolLink = uscPublicationLetterProtocolLink.GetFirstProtocolLink()
        End If

        'Object Privacy
        resl.ResolutionObjectPrivacy = txtObjectPrivacy.Text

        ' Conferenza dei Sindaci 'EF 20120120
        If VisibleOCConfSindaci Then
            resl.ManagementWarningDate = rdpConfSindaciWarningDate.SelectedDate
            resl.ManagementProtocolLink = uscConfSindaciProtocolLink.GetFirstProtocolLink
        End If

        ' CorteDeiConti
        If VisibleCorteDeiConti Then
            resl.CorteDeiContiWarningDate = rdpCorteDeiContiWarningDate.SelectedDate
            resl.CorteDeiContiProtocolLink = uscCorteDeiContiProtocolLink.GetFirstProtocolLink
        End If

        'OCData
        SetOCObjectData(resl, regionOc)

        Return resl
    End Function

    Private Sub SetOCObjectData(ByRef resl As Resolution, ByVal regionOC As Boolean)
        'OC List
        resl.OCCorteConti = chkOCCorteConti.Checked
        resl.OCManagement = chkOCManagement.Checked
        resl.OCOther = chkOCOther.Checked
        resl.OCRegion = chkOCRegion.Checked
        resl.OCSupervisoryBoard = chkOCSupervisoryBoard.Checked
        ' EF 20120119 Se cambia la conferenza dei Sindaci aggiorno su controllo di gestione. 
        ' Non entra in conflitto per chi ha la conferenza dei sindaci non ha il campo other lato grafica
        If DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("AUSL-PC") Then
            resl.OCManagement = chkOCConfSindaci.Checked
        End If

        'OC SupervisoryBoard
        resl.SupervisoryBoardWarningDate = rdpSupervisoryBoardWarningDate.SelectedDate
        resl.SupervisoryBoardProtocolLink = uscSupervisoryBoardProtocolLink.GetFirstProtocolLink()
        If uscOCSupervisoryBoardDocument.DocumentInfos.Count > 0 Then
            ' TODO: usare metodi nuovi
            Dim tempFileDocumentInfo As TempFileDocumentInfo = DirectCast(uscOCSupervisoryBoardDocument.DocumentInfos(0), TempFileDocumentInfo)
            Dim location As New UIDLocation(resl.Container.ReslLocation.ReslBiblosDSDB)
            Dim uid As UIDDocument = Service.AddFile(location, tempFileDocumentInfo.FileInfo, Service.GetBaseAttributes(tempFileDocumentInfo.Name, String.Empty))
            resl.File.IdSupervisoryBoardFile = uid.Chain.Id

        End If
        resl.SupervisoryBoardOpinion = txtOCSupervisoryBoardOpinion.Text

        'OC Management
        resl.ManagementWarningDate = rdpManagementWarningDate.SelectedDate
        resl.ManagementProtocolLink = uscManagementProtocolLink.GetFirstProtocolLink()

        If DocSuiteContext.Current.IsResolutionEnabled AndAlso DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("AUSL-PC") Then
            'OC Conferenza dei Sindaci
            resl.ManagementWarningDate = rdpConfSindaciWarningDate.SelectedDate
            resl.ManagementProtocolLink = uscConfSindaciProtocolLink.GetFirstProtocolLink()
        End If

        'OC Other
        resl.OtherDescription = txtOCOtherDescription.Text

        'OC Region
        Select Case regionOC
            Case True
                'OC Region
                resl.WarningDate = rdpRegionWarningDate.SelectedDate
                resl.ConfirmDate = rdpRegionConfirmDate.SelectedDate
                'resl.ConfirmProtocol = txtRegionConfirmProt.Text
                resl.WaitDate = rdpRegionWaitDate.SelectedDate
                resl.ResponseDate = rdpRegionResponseDate.SelectedDate
                If (tdResponseProtocolContent.Visible) Then resl.ResponseProtocol = uscRegionResponseProtocol.GetFirstProtocolLink()
                resl.RegionProtocolLink = uscRegionProtocolLink.GetFirstProtocolLink()
                If Not String.IsNullOrEmpty(ddlRegionComment.SelectedValue) Then
                    resl.ControllerStatus = Facade.ControllerStatusResolutionFacade.GetById(ddlRegionComment.SelectedValue)
                End If
                resl.ControllerOpinion = txtRegionOpinion.Text
                resl.DeclineNote = txtDeclineNote.Text
                resl.ApprovalNote = txtApprovalNote.Text
                If uscOCRegionDocument.DocumentsAddedCount > 0 AndAlso Not String.IsNullOrEmpty(uscOCRegionDocument.GetNodeValue(0)) Then
                    ' TODO: forse c'è da controllare qualcosa qui
                    Dim fileInfo As New FileInfo(Server.MapPath("~/Temp/" & uscOCRegionDocument.GetNodeValue(0)))
                    Dim location As New UIDLocation(resl.Container.ReslLocation.ReslBiblosDSDB)
                    Dim uid As UIDDocument = Service.AddFile(location, fileInfo, Service.GetBaseAttributes(uscOCRegionDocument.GetNodeValue(0), ""))
                    resl.File.IdControllerFile = uid.Chain.Id

                ElseIf uscOCRegionDocument.DocumentInfos.Count > 0 Then
                    ' TODO: usare metodi nuovi
                    Dim tempFileDocumentInfo As TempFileDocumentInfo = DirectCast(uscOCRegionDocument.DocumentInfos(0), TempFileDocumentInfo)
                    Dim location As New UIDLocation(resl.Container.ReslLocation.ReslBiblosDSDB)
                    Dim uid As UIDDocument = Service.AddFile(location, tempFileDocumentInfo.FileInfo, Service.GetBaseAttributes(tempFileDocumentInfo.Name, String.Empty))
                    resl.File.IdControllerFile = uid.Chain.Id

                Else
                    ' nessun doc
                    resl.File.IdControllerFile = Nothing

                End If

                'AJG 20101215: magari è il caso di tirar giù dalla pagina anche questi altri campi.
                resl.DGR = txtDGR.Text

                'EF 20120320 Modifiche per Piacenza
                If (VisibleOCRegionInvioChiarimenti) Then
                    resl.ConfirmDate = rdpRegionInvioChiarimenti.SelectedDate
                    resl.ConfirmProtocol = uscRegionInvioChiarimentiProtocolLink.GetFirstProtocolLink()
                End If
            Case False
                'OCData Standard
                resl.WarningDate = rdpWarningDate.SelectedDate
                resl.WarningProtocol = txtWarningProt.Text
                resl.ConfirmDate = rdpConfirmDate.SelectedDate
                resl.ConfirmProtocol = txtConfirmProt.Text
                resl.WaitDate = rdpWaitDate.SelectedDate
                resl.ResponseDate = rdpResponseDate.SelectedDate
                resl.ResponseProtocol = txtResponseProt.Text
                If Not String.IsNullOrEmpty(ddlOCComment.SelectedValue) Then
                    resl.ControllerStatus = Facade.ControllerStatusResolutionFacade.GetById(Convert.ToInt16(ddlOCComment.SelectedValue))
                End If
                resl.ControllerOpinion = txtOCOpinion.Text
                If uscOCDocument.DocumentsAddedCount > 0 AndAlso uscOCDocument.DocumentInfos IsNot Nothing AndAlso uscOCDocument.DocumentInfos.Item(0) IsNot Nothing Then

                    Dim location As New UIDLocation(resl.Container.ReslLocation.ReslBiblosDSDB)
                    Dim documentUploaded As BiblosDocumentInfo = uscOCDocument.DocumentInfos.Item(0).ArchiveInBiblos(location.Archive)
                    resl.File.IdControllerFile = documentUploaded.BiblosChainId

                End If
        End Select
    End Sub

    Public Sub AttachEvent(ByVal eventType As ResolutionChangeEventType)
        Select Case eventType
            Case ResolutionChangeEventType.StatusSelectedChangedEvent
                AddHandler ddlStatus.SelectedIndexChanged, AddressOf ddlStatus_SelectedIndexChangedEvent
                AjaxManager.AjaxSettings.AddAjaxSetting(ddlStatus, txtChangedReason)

            Case ResolutionChangeEventType.ConfirmDateSelectedChangedEvent
                AddHandler rdpConfirmDate.SelectedDateChanged, AddressOf rdpConfirmDate_SelectedDateChangedEvent
                AjaxManager.AjaxSettings.AddAjaxSetting(rdpConfirmDate, rdpWaitDate)

            Case ResolutionChangeEventType.ConfirmDateRegionSelectedChangedEvent
                AddHandler rdpRegionConfirmDate.SelectedDateChanged, AddressOf rdpRegionConfirmDate_SelectedDateChangedEvent
                AjaxManager.AjaxSettings.AddAjaxSetting(rdpRegionConfirmDate, rdpRegionWaitDate)

            Case ResolutionChangeEventType.OCListSelectedChangedEvent
                AddHandler chkOCSupervisoryBoard.CheckedChanged, AddressOf chkOCSupervisoryBoard_CheckedChangedEvent
                AddHandler chkOCRegion.CheckedChanged, AddressOf chkOCRegion_CheckedChangedEvent
                AddHandler chkOCManagement.CheckedChanged, AddressOf chkOCManagement_CheckedChangedEvent
                AddHandler chkOCOther.CheckedChanged, AddressOf chkOCOther_CheckedChangedEvent
                AddHandler chkOCConfSindaci.CheckedChanged, AddressOf chkOCConfSindaci_CheckedChangedEvent
                AddHandler chkOCCorteConti.CheckedChanged, AddressOf chkOCCorteConti_CheckedChangedEvent

            Case ResolutionChangeEventType.ContainerSelectedChangedEvent
                ddlIdContainer.AutoPostBack = True
                AddHandler ddlIdContainer.SelectedIndexChanged, AddressOf ddlIdContainer_SelectedIndexChanged
                AjaxManager.AjaxSettings.AddAjaxSetting(ddlIdContainer, tblObjectPrivacy)
        End Select
    End Sub

    Private Sub BindResolutionKind()
        Dim kinds As ICollection(Of ResolutionKind) = New ResolutionKindFacade(DocSuiteContext.Current.User.FullUserName).GetActiveResolutionKind()

        If kinds.Count > 1 Then
            ddlResolutionKind.Items.Add(New ListItem("", ""))
        End If

        For Each resolutionKind As ResolutionKind In kinds.OrderBy(Function(o) o.Name)
            Dim item As ListItem = New ListItem()
            item.Value = resolutionKind.Id.ToString()
            item.Text = resolutionKind.Name

            ddlResolutionKind.Items.Add(item)
        Next

        If kinds.Count.Equals(1) Then
            ddlResolutionKind.SelectedIndex = 0
            ddlResolutionKind.Enabled = False
            Call DdlResolutionKind_SelectedIndexChanged(Me.Page, Nothing)
        End If
    End Sub

    Private Sub NewDraftSeriesAction(idSeries As Integer)
        Dim currentSeries As DocumentSeries = Facade.DocumentSeriesFacade.GetById(idSeries)
        If currentSeries IsNot Nothing Then
            If Not Facade.ContainerFacade.CheckContainerRight(currentSeries.Container.Id, DSWEnvironment.DocumentSeries, DocumentSeriesContainerRightPositions.Insert, True) Then
                AjaxAlert("Non si hanno diritti di inserimento per la voce selezionata")
                AjaxManager.ResponseScripts.Add("ResponseEnd();")
                Exit Sub
            End If
        End If

        BindModelFromPage()
        Response.Redirect(String.Format("../Series/Item.aspx?{0}", CommonShared.AppendSecurityCheck(String.Format("Type=Series&Action={0}&IdSeries={1}&IdResolution={2}", DocumentSeriesAction.FromResolutionKindUpdate.ToString(), idSeries, CurrentResolution.Id))))
    End Sub

    Private Function LoadDraftItem() As IList(Of ResolutionSeriesDraftInsert)
        Dim _draftSeriesItemAdded As IList(Of ResolutionSeriesDraftInsert) = New List(Of ResolutionSeriesDraftInsert)
        For Each rdsi As ResolutionDocumentSeriesItem In CurrentResolutionsDocumentSeriesItem
            Dim dsi As DocumentSeriesItem = Facade.DocumentSeriesItemFacade.GetById(rdsi.IdDocumentSeriesItem.Value)

            _draftSeriesItemAdded.Add(New ResolutionSeriesDraftInsert() With {
                                      .IdSeries = dsi.DocumentSeries.Id,
                                      .IdSeriesItem = dsi.Id
                                      })
        Next
        Return _draftSeriesItemAdded
    End Function

    Private Function GetCurrentDocumentSeriesItems() As ICollection(Of ResolutionChangeDSI)
        Dim results As IList(Of ResolutionDocumentSeriesItem) = Facade.ResolutionDocumentSeriesItemFacade.GetByResolution(CurrentResolution)
        Dim resolutionChangeDSI As ICollection(Of ResolutionChangeDSI) = New List(Of ResolutionChangeDSI)
        If results IsNot Nothing Then
            For Each item As ResolutionDocumentSeriesItem In results
                Dim dsi As DocumentSeriesItem = Facade.DocumentSeriesItemFacade.GetById(item.IdDocumentSeriesItem.Value)
                If dsi IsNot Nothing Then
                    resolutionChangeDSI.Add(New ResolutionChangeDSI() With {
                        .DocumentSeriesId = dsi.DocumentSeries.Id,
                        .DocumentSeriesName = dsi.DocumentSeries.Name,
                        .DSIRegistrationDate = dsi.RegistrationDate,
                        .DSItemId = item.IdDocumentSeriesItem,
                        .ResolutionDSItemId = item.Id
                        })
                End If
            Next
        End If
        Return resolutionChangeDSI
    End Function

    Private Function GetKindDocumentSerie() As ICollection(Of ResolutionChangeDSI)
        Dim resolutionChangeDSI As ICollection(Of ResolutionChangeDSI) = New List(Of ResolutionChangeDSI)
        If CurrentSelectedResolutionKind IsNot Nothing Then
            For Each item As ResolutionKindDocumentSeries In CurrentSelectedResolutionKind.ResolutionKindDocumentSeries
                resolutionChangeDSI.Add(New ResolutionChangeDSI() With {
                    .DocumentSeriesId = item.DocumentSeries.Id,
                    .DocumentSeriesName = item.DocumentSeries.Name
                    })
            Next
        End If
        Return resolutionChangeDSI
    End Function

    Private Function EvaluateKindDocumentSeries(Optional externalList As IList(Of ResolutionSeriesDraftInsert) = Nothing) As ICollection(Of ResolutionChangeDSI)
        Dim results As List(Of ResolutionChangeDSI) = New List(Of ResolutionChangeDSI)
        Dim rdsi As ICollection(Of ResolutionChangeDSI) = GetCurrentDocumentSeriesItems()
        Dim kindRDI As ICollection(Of ResolutionChangeDSI) = GetKindDocumentSerie()
        If rdsi IsNot Nothing AndAlso kindRDI IsNot Nothing Then
            For Each item As ResolutionChangeDSI In kindRDI
                Dim dto As ResolutionChangeDSI = rdsi.SingleOrDefault(Function(x) x.DocumentSeriesId = item.DocumentSeriesId)
                If dto IsNot Nothing Then
                    item.DocumentSeriesId = dto.DocumentSeriesId
                    item.DocumentSeriesName = dto.DocumentSeriesName
                    item.DSIRegistrationDate = dto.DSIRegistrationDate
                    item.DSItemId = dto.DSItemId
                    item.ResolutionDSItemId = dto.ResolutionDSItemId
                    rdsi.Remove(dto)
                End If
            Next
            results.AddRange(kindRDI)
            results.AddRange(rdsi)
        End If
        If externalList IsNot Nothing Then
            For Each result As ResolutionChangeDSI In results
                Dim dto As ResolutionSeriesDraftInsert = externalList.SingleOrDefault(Function(x) x.IdSeries = result.DocumentSeriesId)
                If dto IsNot Nothing Then
                    result.DSItemId = dto.IdSeriesItem
                End If
            Next
        End If
        If DocumentSeriesToRemove IsNot Nothing Then
            For Each temp As ResolutionChangeDSI In DocumentSeriesToRemove
                If CurrentSelectedResolutionKind.ResolutionKindDocumentSeries.Any(Function(x) Not x.DocumentSeries.Id = temp.DocumentSeriesId) Then
                    Dim itemToRemove As ResolutionChangeDSI = results.SingleOrDefault(Function(x) x.DSItemId.HasValue AndAlso x.DSItemId.Value = temp.DSItemId.Value)
                    results.Remove(itemToRemove)
                End If
            Next
        End If

        If (DraftSeriesItemAdded IsNot Nothing AndAlso DraftSeriesItemAdded.Count() > 0 AndAlso results.Any(Function(f) Not f.DSItemId.HasValue)) Then
            For Each item As ResolutionChangeDSI In results.Where(Function(f) Not f.DSItemId.HasValue)
                Dim draft As ResolutionSeriesDraftInsert = DraftSeriesItemAdded.SingleOrDefault(Function(f) f.IdSeries = item.DocumentSeriesId)
                If draft IsNot Nothing Then
                    item.DSItemId = draft.IdSeriesItem
                End If
            Next
        End If

        results = results.OrderBy(Function(x) x.DSItemId).ToList()
        CurrentResolutionDocumentSeriesDTO = results

        Return results
    End Function

    Private Function LoadResolutionModel() As ResolutionInsertModel
        Dim rm As ResolutionInsertModel = New ResolutionInsertModel()
        rm.ResolutionKind = CurrentResolution.ResolutionKind.Id
        Return rm
    End Function

    Private Sub BindModelFromPage()
        Dim model As ResolutionInsertModel = New ResolutionInsertModel()
        'Tipologia atto
        If Not String.IsNullOrEmpty(rblType.SelectedValue) Then
            model.ProposalType = Convert.ToInt32(rblType.SelectedValue)
        End If
        If (Request.QueryString("IdCollaboration") IsNot Nothing) Then
            model.IdCollaboration = Request.QueryString("IdCollaboration").GetValueOrDefault(Of Integer)(Nothing)
        End If

        'Tipologia Atto
        If Not String.IsNullOrEmpty(ddlResolutionKind.SelectedValue) Then
            model.ResolutionKind = Guid.Parse(ddlResolutionKind.SelectedValue)
        Else
            model.ResolutionKind = CurrentResolution.ResolutionKind.Id
        End If

        'Categoria
        model.Category = New List(Of Integer)({CurrentResolution.Category.Id})
        'Contenitore
        model.Container = CurrentResolution.Container.Id

        'Dati Adozione
        model.AdoptionDate = CurrentResolution.AdoptionDate

        'Contatti
        model.Recipients = uscContactDest.GetContacts(True)

        If RoleProposerEnabled Then
            Dim roles As IList(Of Role) = uscRoleProposer.GetRoles()
            If roles.Any() Then
                model.RoleProposer = roles.First()
            End If
        Else
            model.Proposers = uscContactProp.GetContacts(True)
        End If
        model.AlternativeProposer = txtAlternativeProp.Text

        model.Assignee = uscContactAss.GetContacts(True).FirstOrDefault()
        model.Responsible = uscContactMgr.GetContacts(True).FirstOrDefault()

        'Dati Generici
        model.PrivacySubject = txtObjectPrivacy.Text
        model.Subject = txtObject.Text
        model.Note = txtNote.Text
        If uscSubCategory.SelectedCategories.Any() Then
            model.Category = uscSubCategory.SelectedCategories.Select(Function(s) s.Id).ToList()
        End If

        'OC
        If pnlOrganoControllo.Visible Then
            model.OcSupervisoryBoard = chkOCSupervisoryBoard.Checked
            model.OcRegion = chkOCRegion.Checked
            model.OcManagement = chkOCManagement.Checked
            model.OcCorteConti = chkOCCorteConti.Checked
            model.OcOther = chkOCOther.Checked
            model.OcOtherDescription = txtOCOtherDescription.Text
        End If

        'Salvo lo stato degli oggetti in sessione
        CurrentResolutionModel = model
    End Sub

    Private Sub BindPageFromModel()
        ' Inserisco nel viewState l'idResolution che ritorno dalla quueryString delle serieDocumentali
        ViewState("IdResolution") = Request.QueryString.GetValueOrDefault(Of Integer)("IdResolution", -1)

        If CurrentResolutionModel.ResolutionKind.HasValue Then
            ddlResolutionKind.SelectedValue = CurrentResolutionModel.ResolutionKind.Value.ToString()
        End If

        'Tipologia atto
        If Not String.IsNullOrEmpty(CurrentResolutionModel.ProposalType.ToString()) Then
            rblType.SelectedValue = CurrentResolutionModel.ProposalType.ToString()
        End If

        'Contatti
        uscContactDest.DataSource = CurrentResolutionModel.Recipients
        uscContactDest.DataBind()

        If RoleProposerEnabled Then
            If CurrentResolutionModel.RoleProposer IsNot Nothing Then
                uscRoleProposer.SourceRoles = New List(Of Role)() From {CurrentResolutionModel.RoleProposer}
                uscRoleProposer.DataBind()
            End If
        Else
            uscContactProp.DataSource = CurrentResolutionModel.Proposers
            uscContactProp.DataBind()
        End If
        txtAlternativeProp.Text = CurrentResolutionModel.AlternativeProposer

        If CurrentResolutionModel.Assignee IsNot Nothing Then
            uscContactAss.DataSource = New List(Of ContactDTO) From {CurrentResolutionModel.Assignee}
            uscContactAss.DataBind()
        End If

        If CurrentResolutionModel.Responsible IsNot Nothing Then
            uscContactMgr.DataSource = New List(Of ContactDTO) From {CurrentResolutionModel.Responsible}
            uscContactMgr.DataBind()
        End If

        'Dati Generici
        txtObjectPrivacy.Text = CurrentResolutionModel.PrivacySubject
        txtObject.Text = CurrentResolutionModel.Subject
        txtNote.Text = CurrentResolutionModel.Note
        For Each idCategory As Integer In CurrentResolutionModel.Category
            uscSubCategory.DataSource.Add(Facade.CategoryFacade.GetById(idCategory))
        Next
        uscSubCategory.DataBind()

        'OC
        If pnlOrganoControllo.Visible Then
            If CurrentResolutionModel.OcSupervisoryBoard.HasValue Then
                chkOCSupervisoryBoard.Checked = CurrentResolutionModel.OcSupervisoryBoard.Value
            End If

            If CurrentResolutionModel.OcRegion.HasValue Then
                chkOCRegion.Checked = CurrentResolutionModel.OcRegion.Value
            End If

            If CurrentResolutionModel.OcManagement.HasValue Then
                chkOCManagement.Checked = CurrentResolutionModel.OcManagement.Value
            End If

            If CurrentResolutionModel.OcCorteConti.HasValue Then
                chkOCCorteConti.Checked = CurrentResolutionModel.OcCorteConti.Value
            End If

            If CurrentResolutionModel.OcOther.HasValue Then
                chkOCOther.Checked = CurrentResolutionModel.OcOther.Value
            End If

            txtOCOtherDescription.Text = CurrentResolutionModel.OcOtherDescription
        End If

    End Sub




    Private Function ConvertSeriesDraftToDocumentSeriesItem(ResolutionSeries As ResolutionSeriesDraftInsert) As ResolutionDocumentSeriesItem
        Return Facade.ResolutionDocumentSeriesItemFacade.GetByDocumentSeriesItem(ResolutionSeries.IdSeriesItem)
    End Function


    Public Sub SetOCControlsVisible()
        For Each item As Control In Container.Controls
            item.Visible = False
        Next

        pnlOrganoControllo.Visible = True
        VisibleOCSupervisoryBoard = True
        VisibleOCSupervisoryBoardExtra = CurrentResolution.OCSupervisoryBoard.GetValueOrDefault(False)
        VisibleOCRegion = CurrentResolution.OCRegion.GetValueOrDefault(False)
        VisibleOCManagement = CurrentResolution.OCManagement.GetValueOrDefault(False)
        VisibleOCOther = CurrentResolution.OCOther.GetValueOrDefault(False)
        VisibleCorteDeiConti = CurrentResolution.OCCorteConti.GetValueOrDefault(False)

    End Sub

#End Region


#Region " Ajax Alert "

    ''' <summary>Visualizza messaggio di errore popup in javascript</summary>
    ''' <param name="message">Messaggio d'errore composto</param>
    ''' <param name="args">Array di <see>Object</see> contenente zero o più argomenti da formattare</param>
    Public Sub AjaxAlert(ByVal message As String, ByVal ParamArray args() As Object)
        AjaxAlert(String.Format(message, args), True)
    End Sub

    ''' <summary> Visualizza messaggio di errore gestito in javascript. </summary>
    ''' <param name="exception"> Viene preso solo il messaggio. </param>
    ''' <remarks> Tipicamente siamo sicuri di avere messaggi leggibili solo delle DocSuiteException, non usare tutte le eccezioni. </remarks>
    Public Sub AjaxAlert(ByVal exception As DocSuiteException)
        FileLogger.Error(LoggerName, exception.Message, exception)
        AjaxAlert(exception.Message, True)
    End Sub

    ''' <summary>Visualizza messaggio di errore popup in javascript</summary>
    ''' <param name="message">Messaggio d'errore</param>
    Public Sub AjaxAlert(ByVal message As String)
        AjaxAlert(message, True)
    End Sub

    ''' <summary>Metodo che esegue l'alert</summary>
    ''' <param name="message">messaggio da mandare</param>
    ''' <param name="checkJavascript">Indica se filtrare il messaggio per evitare caratteri che invalidano il javascript</param>
    Private Sub AjaxAlert(ByVal message As String, ByVal checkJavascript As Boolean)
        If checkJavascript Then
            message = StringHelper.ReplaceAlert(message)
        End If

        If AjaxManager IsNot Nothing Then
            AjaxManager.Alert(message)
        End If
    End Sub

    Public Sub AjaxAlertConfirm(ByVal message As String, ByVal scriptOnConfirm As String, ByVal scriptBeforeConfirm As String)
        AjaxAlertConfirm(message, scriptOnConfirm, scriptBeforeConfirm, False)
    End Sub

    Public Sub AjaxAlertConfirm(ByVal message As String, ByVal scriptOnConfirm As String, ByVal scriptBeforeConfirm As String, ByVal checkJavascript As Boolean)
        AjaxAlertConfirmAndDeny(message, scriptOnConfirm, String.Empty, scriptBeforeConfirm, checkJavascript)
    End Sub

    Public Sub AjaxAlertConfirmAndDeny(ByVal message As String, ByVal scriptOnConfirm As String, ByVal scriptOnDeny As String, ByVal scriptBeforeConfirm As String, ByVal checkJavascript As Boolean)
        ' TODO: Non si riesce a creare un'overload compatibile con la gestione principale degli alert fatta con le rad?

        If checkJavascript Then
            message = HttpUtility.JavaScriptStringEncode(message)
        End If

        Dim script As String = String.Format("function AlertConfirm() {{{0} if (confirm('{1}')){{{2}}} else {{{3}}} return true; }} AlertConfirm();", scriptBeforeConfirm, message, scriptOnConfirm, scriptOnDeny)
        AjaxManager.ResponseScripts.Add(script)
    End Sub

#End Region

End Class