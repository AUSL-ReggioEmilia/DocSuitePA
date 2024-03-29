﻿Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods

Public Class ResolutionChangeControllerTO
    Inherits ResolutionChangeController

#Region " Fields "
    Private Const OCDATA_FIELD_NAME As String = "OCData"
    Private Const OBJECT_FIELD_NAME As String = "Object"
    Private Const STATUSDATA_FIELD_NAME As String = "StatusData"

    Private Const OC_CS_PROPERTY_NAME As String = "CS"
    Private Const OC_REG_PROPERTY_NAME As String = "REG"
    Private Const OC_GEST_PROPERTY_NAME As String = "GEST"
    Private Const OC_ALTRO_PROPERTY_NAME As String = "ALTRO"
    Private Const OC_CC_PROPERTY_NAME As String = "CC"
    Private Const STATUSDATA_NOTEXECUTIVE_PROPERTY_NAME As String = "NOTEXECUTIVE"

    Private Const OC_CS_FILED_VALUE As String = ".CS."
    Private Const OC_REG_FILED_VALUE As String = ".REG."
    Private Const OC_GEST_FILED_VALUE As String = ".GEST."
    Private Const OC_ALTRO_FILED_VALUE As String = ".ALTRO."
    Private Const OC_CC_FILED_VALUE As String = ".CC."
    Private Const STATUSDATA_NOTEXECUTIVE_FIELD_VALUE As String = ".NOTEXECUTIVE."
#End Region

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

        _uscReslChange.VisibleOC = False

        'Proponente di default
        _uscReslChange.ControlProposerInterop.ContactRoot = DocSuiteContext.Current.ResolutionEnv.ProposerContact

        _uscReslChange.VisibleImmediatelyExecutive = DocSuiteContext.Current.ResolutionEnv.ImmediatelyExecutiveEnabled

        Dim changeableData As String = String.Empty

        If Facade.TabWorkflowFacade.GetChangeableData(_uscReslChange.CurrentResolution.Id, _uscReslChange.CurrentResolution.WorkflowType, 0, changeableData) Then

            'Recovery OC Rights
            Dim cheCkOCRights As Boolean = ResolutionRights.CheckIsExecutive(_uscReslChange.CurrentResolution)
            'OC Data
            _uscReslChange.VisibleOCList = ManagedDataTest(OCDATA_FIELD_NAME, , changeableData, OCDATA_FIELD_NAME) AndAlso cheCkOCRights
            'OC SupervisoryBoard
            _uscReslChange.VisibleOCSupervisoryBoard = ManagedDataTest(OCDATA_FIELD_NAME, OC_CS_PROPERTY_NAME, changeableData, OC_CS_FILED_VALUE) AndAlso cheCkOCRights
            _uscReslChange.VisibleOCSupervisoryBoardExtra = _uscReslChange.CurrentResolution.OCSupervisoryBoard.GetValueOrDefault(False)
            'OC Region
            _uscReslChange.VisibleOCRegion = ManagedDataTest(OCDATA_FIELD_NAME, OC_REG_PROPERTY_NAME, changeableData, OC_REG_FILED_VALUE) AndAlso cheCkOCRights AndAlso _uscReslChange.CurrentResolution.OCRegion.GetValueOrDefault(False)
            'OC Management
            _uscReslChange.VisibleOCManagement = ManagedDataTest(OCDATA_FIELD_NAME, OC_GEST_PROPERTY_NAME, changeableData, OC_GEST_FILED_VALUE) AndAlso cheCkOCRights AndAlso _uscReslChange.CurrentResolution.OCManagement.GetValueOrDefault(False)
            'OC Other
            _uscReslChange.VisibleOCOther = ManagedDataTest(OCDATA_FIELD_NAME, OC_ALTRO_PROPERTY_NAME, changeableData, OC_ALTRO_FILED_VALUE) AndAlso cheCkOCRights AndAlso _uscReslChange.CurrentResolution.OCOther.GetValueOrDefault(False)

            'Object Privacy
            _uscReslChange.VisibleObjectPrivacy = ManagedDataTest(OBJECT_FIELD_NAME, , changeableData, OBJECT_FIELD_NAME) AndAlso cheCkOCRights

            ' CorteDeiConti
            _uscReslChange.VisibleCorteDeiConti = ManagedDataTest(OCDATA_FIELD_NAME, OC_CC_PROPERTY_NAME, changeableData, OC_CC_FILED_VALUE) AndAlso cheCkOCRights AndAlso _uscReslChange.CurrentResolution.OCCorteConti.GetValueOrDefault(False)
        End If

        If _uscReslChange.CurrentResolution.EffectivenessDate.HasValue AndAlso Not String.IsNullOrEmpty(_uscReslChange.CurrentResolution.EffectivenessUser) AndAlso
            CommonShared.UserConnectedBelongsTo(ResolutionEnv.OCExecutiveModifyGroups) AndAlso DocSuiteContext.Current.ResolutionEnv.CheckOCValidations Then
            _uscReslChange.SetOCControlsVisible()
        End If

        'Visibilità ed etichette dell'organo di controllo
        Dim ocOptions As OCOptionsModel = DocSuiteContext.Current.ResolutionEnv.OCOptions
        If ocOptions IsNot Nothing Then
            _uscReslChange.VisibleOCSupervisoryBoardCheckbox = ocOptions.SupervisoryBoard.Visible
            _uscReslChange.LabelOCSupervisoryBoardCheckbox = ocOptions.SupervisoryBoard.Label
            _uscReslChange.VisibleOCConfSindaciCheckbox = ocOptions.ConfSindaci.Visible
            _uscReslChange.LabelOCConfSindaciCheckbox = ocOptions.ConfSindaci.Label
            _uscReslChange.VisibleOCRegionCheckbox = ocOptions.Region.Visible
            _uscReslChange.LabelOCRegionCheckbox = ocOptions.Region.Label
            _uscReslChange.VisibleOCManagementCheckbox = ocOptions.Management.Visible
            _uscReslChange.LabelOCManagementCheckbox = ocOptions.Management.Label
            _uscReslChange.VisibleOCCorteContiCheckbox = ocOptions.CorteConti.Visible
            _uscReslChange.LabelOCCorteContiCheckbox = ocOptions.CorteConti.Label
            _uscReslChange.VisibleOCOtherCheckbox = ocOptions.Other.Visible
            _uscReslChange.LabelOCOtherCheckbox = ocOptions.Other.Label
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

        'Service
        If _uscReslChange.VisibleProposerProtocolLink Then
            domainObject.ProposerProtocolLink = _objChangedData.ProposerProtocolLink
        End If

        'OC List
        If _uscReslChange.VisibleOCList Then
            domainObject.OCCorteConti = _objChangedData.OCCorteConti
            domainObject.OCManagement = _objChangedData.OCManagement
            domainObject.OCOther = _objChangedData.OCOther
            domainObject.OCRegion = _objChangedData.OCRegion
            domainObject.OCSupervisoryBoard = _objChangedData.OCSupervisoryBoard
        End If

        'OC SupervisoryBoard
        If _uscReslChange.VisibleOCSupervisoryBoard Then
            If Not domainObject.SupervisoryBoardWarningDate.Equals(_objChangedData.SupervisoryBoardWarningDate) Then
                domainObject.SupervisoryBoardWarningDate = _objChangedData.SupervisoryBoardWarningDate
                Facade.ResolutionLogFacade.Insert(domainObject, ResolutionLogType.RM, "Modificata data invio a Collegio Sindacale")
            End If
            If Not domainObject.SupervisoryBoardProtocolLink.Eq(_objChangedData.SupervisoryBoardProtocolLink) Then
                domainObject.SupervisoryBoardProtocolLink = _objChangedData.SupervisoryBoardProtocolLink
                Facade.ResolutionLogFacade.Insert(domainObject, ResolutionLogType.RM, "Modificato collegamento protocollo a Collegio Sindacale")
            End If
            domainObject.File.IdSupervisoryBoardFile = _objChangedData.File.IdSupervisoryBoardFile
            domainObject.SupervisoryBoardOpinion = _objChangedData.SupervisoryBoardOpinion
        End If

        ' OC CorteDeiConti
        If _uscReslChange.VisibleCorteDeiConti Then
            If Not domainObject.CorteDeiContiWarningDate.Equals(_objChangedData.CorteDeiContiWarningDate) Then
                domainObject.CorteDeiContiWarningDate = _objChangedData.CorteDeiContiWarningDate
                Facade.ResolutionLogFacade.Insert(domainObject, ResolutionLogType.RM, "Modificata data invio a Corte dei Conti")
            End If
            If Not domainObject.CorteDeiContiProtocolLink.Eq(_objChangedData.CorteDeiContiProtocolLink) Then
                domainObject.CorteDeiContiProtocolLink = _objChangedData.CorteDeiContiProtocolLink
                Facade.ResolutionLogFacade.Insert(domainObject, ResolutionLogType.RM, "Modificato collegamento protocollo a Corte dei Conti")
            End If
        End If

        'OC Management
        If _uscReslChange.VisibleOCManagement Then
            If Not domainObject.ManagementWarningDate.Equals(_objChangedData.ManagementWarningDate) Then
                domainObject.ManagementWarningDate = _objChangedData.ManagementWarningDate
                Facade.ResolutionLogFacade.Insert(domainObject, ResolutionLogType.RM, "Modificata data invio al Controllo di Gestione")
            End If
            If Not domainObject.ManagementProtocolLink.Eq(_objChangedData.ManagementProtocolLink) Then
                domainObject.ManagementProtocolLink = _objChangedData.ManagementProtocolLink
                Facade.ResolutionLogFacade.Insert(domainObject, ResolutionLogType.RM, "Modificato collegamento protocollo al Controllo di Gestione")
            End If
        End If

        'OC Other
        If _uscReslChange.VisibleOCOther Then
            domainObject.OtherDescription = _objChangedData.OtherDescription
        End If

        'OC Region
        If _uscReslChange.VisibleOCRegion Then
            If Not domainObject.WarningDate.Equals(_objChangedData.WarningDate) Then
                domainObject.WarningDate = _objChangedData.WarningDate
                Facade.ResolutionLogFacade.Insert(domainObject, ResolutionLogType.RM, "Modificata data invio alla Regione")
            End If
            If Not domainObject.RegionProtocolLink.Eq(_objChangedData.RegionProtocolLink) Then
                domainObject.RegionProtocolLink = _objChangedData.RegionProtocolLink
                Facade.ResolutionLogFacade.Insert(domainObject, ResolutionLogType.RM, "Modificato collegamento protocollo alla Regione")
            End If
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

        If _uscReslChange.VisibleImmediatelyExecutive Then
            domainObject.ImmediatelyExecutive = _objChangedData.ImmediatelyExecutive
        End If
    End Sub

    Protected Overrides Sub InitializeDelegates()
        'Status delegate
        Dim changeableData As String = String.Empty
        Facade.TabWorkflowFacade.GetChangeableData(_uscReslChange.CurrentResolution.Id, _uscReslChange.CurrentResolution.WorkflowType, 0, changeableData)

        If Not String.IsNullOrWhiteSpace(changeableData) AndAlso Not _uscReslChange.CurrentResolution.SupervisoryBoardWarningDate.HasValue _
            AndAlso ManagedDataTest(STATUSDATA_FIELD_NAME, changeableData:=changeableData, FieldTest:=STATUSDATA_NOTEXECUTIVE_FIELD_VALUE) Then
            GetStatusList = New GetStatusListDelegate(AddressOf Facade.ResolutionStatusFacade.GetStatusNotExecutive)
        Else
            GetStatusList = New GetStatusListDelegate(AddressOf Facade.ResolutionStatusFacade.GetStatusList)
        End If

    End Sub

    Protected Overrides Sub AttachEvents()
        _uscReslChange.AttachEvent(uscResolutionChange.ResolutionChangeEventType.StatusSelectedChangedEvent)
        _uscReslChange.AttachEvent(uscResolutionChange.ResolutionChangeEventType.ConfirmDateRegionSelectedChangedEvent)
        _uscReslChange.AttachEvent(uscResolutionChange.ResolutionChangeEventType.OCListSelectedChangedEvent)
        _uscReslChange.AttachEvent(uscResolutionChange.ResolutionChangeEventType.ContainerSelectedChangedEvent)
    End Sub

#End Region

End Class
