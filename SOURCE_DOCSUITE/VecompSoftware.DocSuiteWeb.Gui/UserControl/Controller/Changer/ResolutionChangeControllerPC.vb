Imports VecompSoftware.DocSuiteWeb.Data

Public Class ResolutionChangeControllerPC
    Inherits ResolutionChangeController
    
#Region " Constructors "

    Public Sub New(ByRef uscControl As uscResolutionChange)
        MyBase.New(uscControl)
    End Sub

#End Region

#Region " Methods "

    Protected Overrides Sub InitializeNonStandardPanels()
        _uscReslChange.VisibleType = False
        _uscReslChange.VisibleOC = False
    End Sub

    Public Overrides Sub Initialize()
        MyBase.Initialize()

        'Proponente di default
        _uscReslChange.ControlProposerInterop.ContactRoot = DocSuiteContext.Current.ResolutionEnv.ProposerContact

        _uscReslChange.VisibleImmediatelyExecutive = DocSuiteContext.Current.ResolutionEnv.ImmediatelyExecutiveEnabled
        _uscReslChange.VisibleComunication = False 'EF 20120119 Disattivazione del pannello di comunicazione

        Dim changeableData As String = String.Empty
        If Facade.TabWorkflowFacade.GetChangeableData(_uscReslChange.CurrentResolution.Id, _uscReslChange.CurrentResolution.WorkflowType, 0, changeableData) Then
            'Recovery OC Rights
            Dim cheCkOCRights As Boolean = _uscReslChange.CurrentResolutionRight.IsExecutive
            'OC Data
            _uscReslChange.VisibleOCList = ManagedDataTest("OCData", , changeableData, "OCData") AndAlso cheCkOCRights
            'OC SupervisoryBoard
            _uscReslChange.VisibleOCSupervisoryBoard = ManagedDataTest("OCData", "CS", changeableData, ".CS.") AndAlso cheCkOCRights AndAlso _uscReslChange.CurrentResolution.OCSupervisoryBoard.GetValueOrDefault(False)
            _uscReslChange.VisibleOCSupervisoryBoardExtra = False 'EF 20120120 Disabilitata la board doc+rilievo per AUSL-PC
            'OC Conferenza dei Sindaci
            _uscReslChange.VisibleOCConfSindaci = ManagedDataTest("OCData", "CONFSIND", changeableData, ".CONFSIND.") AndAlso cheCkOCRights AndAlso _uscReslChange.CurrentResolution.OCManagement.GetValueOrDefault(False)
            'OC Region
            _uscReslChange.VisibleOCRegion = ManagedDataTest("OCData", "REG", changeableData, ".REG.") AndAlso cheCkOCRights AndAlso _uscReslChange.CurrentResolution.OCRegion.GetValueOrDefault(False)
            'OC Other
            _uscReslChange.VisibleOCOther = ManagedDataTest("OCData", "ALTRO", changeableData, ".ALTRO.") AndAlso cheCkOCRights AndAlso _uscReslChange.CurrentResolution.OCOther.GetValueOrDefault(False)
            'Object Privacy
            _uscReslChange.VisibleObjectPrivacy = ManagedDataTest("Object", , changeableData, "Object") AndAlso cheCkOCRights
            ' CorteDeiConti
            _uscReslChange.VisibleCorteDeiConti = ManagedDataTest("OCData", "CC", changeableData, ".CC.") AndAlso cheCkOCRights AndAlso _uscReslChange.CurrentResolution.OCCorteConti.GetValueOrDefault(False)
            'Disattivazione del pannello generico degli organi di controllo
            _uscReslChange.VisibleOC = False
            'Disattivazione del pannello "invio servizi"
            _uscReslChange.VisibleProposerProtocolLink = False
            'Rimozione del campo di commento vincolato
            _uscReslChange.VisibleRegionComment = False
            'Modifica dello status possibile solo a chi possiede il diritto di cancellazione e contemporaneamente di ufficio dirigenziale
            _uscReslChange.VisibleStatus = _uscReslChange.CurrentResolutionRight.IsCancelable AndAlso _uscReslChange.CurrentResolutionRight.IsExecutive

            'Visualizza tutto il pannello del Collegio Sindacale solo se è previsto per la resolution ed è attivo il diritto di adozione
            _uscReslChange.VisibleOCSupervisoryBoardCheckbox = Facade.ResolutionFacade.ManagedDataTest(_uscReslChange.CurrentResolution, "OCData", "CS", changeableData, ".CS.") And _
                _uscReslChange.CurrentResolutionRight.IsExecutive
            'Visualizza il pannello della Conferenza dei Sindaci solo se è previsto su TabWorkflow
            _uscReslChange.VisibleOCConfSindaciCheckbox = Facade.ResolutionFacade.ManagedDataTest(_uscReslChange.CurrentResolution, "OCData", "CONFSIND", changeableData, ".CONFSIND.") And _uscReslChange.CurrentResolutionRight.IsExecutive
            'Visualizza il pannello della Regione solo se è previsto su TabWorkflow
            _uscReslChange.VisibleOCRegionCheckbox = Facade.ResolutionFacade.ManagedDataTest(_uscReslChange.CurrentResolution, "OCData", "REG", changeableData, ".REG.") And _uscReslChange.CurrentResolutionRight.IsExecutive
            'Disattivo il checkbox del controllo di Gestione
            _uscReslChange.VisibleOCManagementCheckbox = False
            'Disattivo il checkbox del controllo della Corte dei Conti
            _uscReslChange.VisibleOCCorteContiCheckbox = False
            'Disattivo il checkbox del controllo "Altro"
            _uscReslChange.VisibleOCOtherCheckbox = False
            'Disattivo il documento per la regione
            _uscReslChange.VisibleOCRegionDocument = False
            'Disattivo il campo DGR della regione
            _uscReslChange.VisibleOCRegionDGR = False
            'Attivo il pannello per l'inserimento di data e protocollo dei chiarimenti inviati alla Regione
            _uscReslChange.VisibleOCRegionInvioChiarimenti = True
            'Disattivo il pannello di ricezione e scadenza dei dati di Regione
            _uscReslChange.VisibleOCRegionRicezioneEScadenza = False
            'Disattivo il pannello note di approvazione della regione
            _uscReslChange.VisibleOCRegionNoteApprovazione = False
            'Disattivo il pannello note decadimento della regione
            _uscReslChange.VisibleOCRegionNoteDecadimento = False
            'Visualizza il pannello del protocollo di Risposta della Regione
            _uscReslChange.VisibleResponseProtocol = True

            _uscReslChange.RegionResponseDateLabel = "Data risposta della Regione:"
            _uscReslChange.RegionOpinionText = "Risposta finale della Regione:"
        End If
    End Sub

    Public Overrides Function ValidateData(ByRef errorMessage As String) As Boolean
        If Not _uscReslChange.ValidateObject(errorMessage) Then
            Return False
        End If
        If Not _uscReslChange.ValidateObjectPrivacy(errorMessage) Then
            Return False
        End If
        If Not _uscReslChange.ValidateNote(errorMessage) Then
            Return False
        End If

        Return True
    End Function

    Protected Overrides Function GetChangedObjectData() As Resolution
        Return _uscReslChange.GetChangedObjectData(True)
    End Function

    Public Overrides Sub BindDataToObject(ByRef domainObject As Resolution)
        MyBase.BindDataToObject(domainObject)

        'Object Privacy
        If _uscReslChange.VisibleObjectPrivacy Then
            domainObject.ResolutionObjectPrivacy = _objChangedData.ResolutionObjectPrivacy
        End If

        'Note
        If _uscReslChange.VisibleNote Then
            domainObject.Note = _objChangedData.Note
        End If

        'OC List
        If _uscReslChange.VisibleOCList Then
            domainObject.OCOther = _objChangedData.OCOther
            domainObject.OCRegion = _objChangedData.OCRegion
            domainObject.OCSupervisoryBoard = _objChangedData.OCSupervisoryBoard
            domainObject.OCManagement = _objChangedData.OCManagement
        End If

        'OC SupervisoryBoard
        If _uscReslChange.VisibleOCSupervisoryBoard Then
            domainObject.SupervisoryBoardWarningDate = _objChangedData.SupervisoryBoardWarningDate
            domainObject.SupervisoryBoardProtocolLink = _objChangedData.SupervisoryBoardProtocolLink
            domainObject.File.IdSupervisoryBoardFile = _objChangedData.File.IdSupervisoryBoardFile
            domainObject.SupervisoryBoardOpinion = _objChangedData.SupervisoryBoardOpinion
        End If

        'Conferenza dei Sindaci
        If _uscReslChange.VisibleOCConfSindaci Then
            domainObject.ManagementWarningDate = _objChangedData.ManagementWarningDate
            domainObject.ManagementProtocolLink = _objChangedData.ManagementProtocolLink
        End If

        'OC Region
        If _uscReslChange.VisibleOCRegion Then
            domainObject.WarningDate = _objChangedData.WarningDate
            domainObject.ConfirmDate = _objChangedData.ConfirmDate
            domainObject.ConfirmProtocol = _objChangedData.ConfirmProtocol
            domainObject.WaitDate = _objChangedData.WaitDate
            domainObject.ResponseDate = _objChangedData.ResponseDate
            domainObject.ResponseProtocol = _objChangedData.ResponseProtocol
            domainObject.ControllerStatus = _objChangedData.ControllerStatus
            domainObject.ControllerOpinion = _objChangedData.ControllerOpinion
            domainObject.File.IdControllerFile = _objChangedData.File.IdControllerFile
            domainObject.RegionProtocolLink = _objChangedData.RegionProtocolLink
            domainObject.DGR = _objChangedData.DGR
            domainObject.ApprovalNote = _objChangedData.ApprovalNote
            domainObject.DeclineNote = _objChangedData.DeclineNote
        End If

    End Sub

    Protected Overrides Sub InitializeDelegates()
        'Status delegate
        GetStatusList = New GetStatusListDelegate(AddressOf Facade.ResolutionStatusFacade.GetStatusList)
    End Sub

    Protected Overrides Sub AttachEvents()
        _uscReslChange.AttachEvent(uscResolutionChange.ResolutionChangeEventType.StatusSelectedChangedEvent)
        _uscReslChange.AttachEvent(uscResolutionChange.ResolutionChangeEventType.ConfirmDateRegionSelectedChangedEvent)
        _uscReslChange.AttachEvent(uscResolutionChange.ResolutionChangeEventType.OCListSelectedChangedEvent)
        _uscReslChange.AttachEvent(uscResolutionChange.ResolutionChangeEventType.ContainerSelectedChangedEvent)
    End Sub

#End Region

End Class
