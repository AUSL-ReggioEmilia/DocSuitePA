Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods

Public Class ResolutionChangeController
    Inherits BaseResolutionController
    Implements IChangerController(Of Resolution)

#Region "Fields"
    Protected _uscReslChange As uscResolutionChange
    Protected _objChangedData As Resolution
#End Region

#Region "Constructor"
    Public Sub New(ByRef uscControl As uscResolutionChange)
        _uscReslChange = uscControl
    End Sub
#End Region

#Region "Utils"
    Protected Function ManagedDataTest(ByVal FieldName As String, Optional ByVal FieldProperty As String = "", Optional ByVal changeableData As String = "", Optional ByVal FieldTest As String = "") As Boolean
        Return Facade.ResolutionFacade.ManagedDataTest(_uscReslChange.CurrentResolution, FieldName, FieldProperty, changeableData, FieldTest)
    End Function
    Private Function CurrentContainerHasAccountingEnabled() As Boolean
        If Not ResolutionEnv.ResolutionAccountingEnabled Then
            Return True
        End If
        Dim accountingProperty As ContainerProperty = _uscReslChange.CurrentResolution.Container.ContainerProperties.FirstOrDefault(Function(x) x.Name.Equals(ContainerPropertiesName.ResolutionAccountingEnabled))
        If accountingProperty Is Nothing Then
            Return False
        End If
        Return accountingProperty.ValueBoolean.Value
    End Function
#End Region

#Region "Initialize"
    Protected Overridable Sub InitializeNonStandardPanels()
        'ObjectPrivacy
        _uscReslChange.VisibleObjectPrivacy = False
        'ProtocolLink
        _uscReslChange.VisibleProposerProtocolLink = False
        'OCList
        _uscReslChange.VisibleOCList = False
        'OCSupervisoryBoard
        _uscReslChange.VisibleOCSupervisoryBoard = False
        'OCConfSindaci
        _uscReslChange.VisibleOCConfSindaci = False
        'OCRegion
        _uscReslChange.VisibleOCRegion = False
        'OCManagement
        _uscReslChange.VisibleOCManagement = False
        'OCOther
        _uscReslChange.VisibleOCOther = False

        'CorteDeiConti
        _uscReslChange.VisibleCorteDeiConti = False
    End Sub

    Public Overridable Sub Initialize() Implements IChangerController(Of Resolution).Initialize
        'Nasconde i pannelli che non fanno parte della configurazione base
        InitializeNonStandardPanels()

        Dim changeableData As String = String.Empty
        If _uscReslChange.CurrentResolution IsNot Nothing Then
            If Facade.TabWorkflowFacade.GetChangeableData(_uscReslChange.CurrentResolution.Id, _uscReslChange.CurrentResolution.WorkflowType, 0, changeableData) Then
                'Alternative Contacts
                _uscReslChange.VisibleContactRecipient = ManagedDataTest("Recipient", , changeableData, "Recipient")
                _uscReslChange.VisibleContactProposer = ManagedDataTest("Proposer", , changeableData, "Proposer")
                'Inizializzo la visibilità dei controlli per il proponente
                _uscReslChange.RoleProposerVisible = _uscReslChange.RoleProposerEnabled
                _uscReslChange.VisibleContactProposerAddress = False
                _uscReslChange.VisibleContactAlternativeProposer = False

                _uscReslChange.VisibleContactAssignee = ManagedDataTest("Assignee", , changeableData, "Assignee")
                _uscReslChange.VisibleContactManager = ManagedDataTest("Manager", , changeableData, "Manager")

                'Contacts
                If _uscReslChange.VisibleContactRecipient Then
                    _uscReslChange.VisibleContactRecipientAddress = ManagedDataTest("Recipient", "CONTACT")
                End If
                If Not _uscReslChange.RoleProposerEnabled AndAlso _uscReslChange.VisibleContactProposer Then
                    _uscReslChange.VisibleContactProposerAddress = ManagedDataTest("Proposer", "CONTACT")
                    _uscReslChange.ContactProposerMultiSelect = ManagedDataTest("Proposer", "MULTISELECT")
                    _uscReslChange.VisibleContactAlternativeProposer = Not (ResolutionEnv.HideAlternativeProposer)
                End If
                If _uscReslChange.VisibleContactAssignee Then
                    _uscReslChange.VisibleContactAssigneeAddress = ManagedDataTest("Assignee", "CONTACT")
                End If
                If _uscReslChange.VisibleContactManager Then
                    _uscReslChange.VisibleContactManagerAddress = ManagedDataTest("Manager", "CONTACT")
                End If
                'Category
                _uscReslChange.VisibleCategory = ManagedDataTest("Category", , changeableData, "Category")
                'EconomicData
                _uscReslChange.VisibleEconomicData = ManagedDataTest("EconomicData", , changeableData, "EconomicData") AndAlso CurrentContainerHasAccountingEnabled()

                If _uscReslChange.VisibleEconomicData AndAlso ResolutionEnv.ResolutionAccountingEnabled Then
                    _uscReslChange.VisibleEconomicDataContratto = False
                    _uscReslChange.VisibleEconomicDataFornitore = False
                    _uscReslChange.VisibleEconomicDataPosizione = False
                    _uscReslChange.LabelEconomicDataBidType = "Stato contabilità:"
                    _uscReslChange.LabelEconomicDataTitle = "Contabilità:"
                End If

                'TODO: gestione gruppi NON AD

                'OC
                'Per mostrare il pannello dell'organo di controllo, la pproprietà OcData deve essere presente in TabMaster e in TabWorkflow allo step specifico
                'Attenzione: per AUSL-RE ci sono 3 tipologie di Atti ed in TabMaster sono identificate con i valori 0,1,2 della colonna ResolutionType. 
                '            Tuttavia non c'è una corrispondenza nella tabella ResolutionType dove sono presenti solo due tipi: 0 e 1, ovvero la classica distinzione tra determine e delibere.
                _uscReslChange.VisibleOC = ManagedDataTest("OCData", , changeableData, "OcData") And _uscReslChange.CurrentResolutionRight.IsExecutive
                _uscReslChange.VisibleStatus = ManagedDataTest("StatusData", , changeableData, "StatusData") And _uscReslChange.CurrentResolutionRight.IsAdministrable
                _uscReslChange.VisibleContainer = ManagedDataTest("Container", , changeableData, "Container")

                ' Campi dentro il box Oggetto
                'Object
                _uscReslChange.VisibleObject = ManagedDataTest("Object", , changeableData, "Object")
                'Note
                _uscReslChange.VisibleNote = ManagedDataTest("Note", , changeableData, "Note")
                'ImmediatelyExecutive
                _uscReslChange.VisibleImmediatelyExecutive = ManagedDataTest("IExec", , changeableData, "IExec")



                'Publication
                _uscReslChange.VisiblePublication = ManagedDataTest("Publication", , changeableData, "InternetPublication") And ResolutionEnv.IsPublicationEnabled
                If _uscReslChange.VisiblePublication Then
                    ' Verifico che ci sia almeno un documento caricato in adozione
                    Dim _fileResolution As FileResolution = Facade.FileResolutionFacade.GetByResolution(_uscReslChange.CurrentResolution)(0)
                    Dim workActive As TabWorkflow 'drWActive
                    Dim MyStep As Short = 2 'STEP ADDOZIONE
                    If Not Facade.TabWorkflowFacade.GetByStep(_uscReslChange.CurrentResolution.WorkflowType, MyStep, workActive) Then
                        _uscReslChange.VisiblePublication = False
                    Else
                        'Dim fieldDocument As Integer? = ReflectionHelper.GetPropertyCase(_fileResolution, workActive.FieldDocument)
                        _uscReslChange.VisiblePublication = _fileResolution.IdResolutionFile.HasValue ' fieldDocument.HasValue
                    End If

                End If
                'Type
                _uscReslChange.VisibleType = ManagedDataTest("Type", , changeableData, "Type")

                'PubblicationLetter
                _uscReslChange.VisiblePublicationLetterProtocolLink = ManagedDataTest("PublicationLetterProtocolLink", , changeableData, "PublicationLetterProtocolLink")

            End If
        End If
    End Sub
#End Region

#Region "DataBind"
    Public Overridable Sub DataBind() Implements IChangerController(Of Resolution).DataBind
        'Initialize delegates for bind Data
        InitializeDelegates()

        'category
        If _uscReslChange.VisibleCategory Then
            _uscReslChange.BindCategory()
        End If
        'contact recipient
        If _uscReslChange.VisibleContactRecipient Then
            If _uscReslChange.VisibleContactRecipientAddress Then
                _uscReslChange.BindContactRecipients()
            End If
            If _uscReslChange.VisibleContactAlternativeRecipient Then
                _uscReslChange.BindContactAlternativeRecipients()
            End If
        End If
        'contact proposer
        If _uscReslChange.VisibleContactProposer Then
            If Not _uscReslChange.RoleProposerEnabled AndAlso _uscReslChange.VisibleContactProposerAddress Then
                _uscReslChange.BindContactProposer()
            End If

            If _uscReslChange.VisibleContactAlternativeProposer Then
                _uscReslChange.BindContactAlternativeProposer()
            End If
        End If

        'eseguo il bind del settore proponente
        If _uscReslChange.RoleProposerEnabled Then
            _uscReslChange.BindRoleProposer()
        End If

        'contact assignee
        If _uscReslChange.VisibleContactAssignee Then
            If _uscReslChange.VisibleContactAssigneeAddress Then
                _uscReslChange.BindContactAssignee()
            End If
            If _uscReslChange.VisibleContactAlternativeAssignee Then
                _uscReslChange.BindContactAlternativeAssignee()
            End If
        End If
        'contact manager
        If _uscReslChange.VisibleContactManager Then
            If _uscReslChange.VisibleContactManagerAddress Then
                _uscReslChange.BindContactManager()
            End If
            If _uscReslChange.VisibleContactAlternativeManager Then
                _uscReslChange.BindContactAlternativeManager()
            End If
        End If
        'container
        If _uscReslChange.VisibleContainer Then
            _uscReslChange.BindContainer()
        End If
        'economic data
        If _uscReslChange.VisibleEconomicData Then
            If ResolutionEnv.ResolutionAccountingEnabled Then
                _uscReslChange.BindEconomicDataAccounting()
            Else
                _uscReslChange.BindEconomicData()
            End If
        End If
        'note
        If _uscReslChange.VisibleNote Then
            _uscReslChange.BindNote()
        End If
        'oggetto
        If _uscReslChange.VisibleObject Then
            _uscReslChange.BindObject()
        End If
        'OC
        If _uscReslChange.VisibleOC Then
            _uscReslChange.BindOCData()
        End If
        'pubblication
        If _uscReslChange.VisiblePublication And ResolutionEnv.IsPublicationEnabled Then
            _uscReslChange.BindPubblication()
        End If
        'status
        If _uscReslChange.VisibleStatus Then
            _uscReslChange.BindStatus(GetStatusList.Invoke())
        End If
        '
        If _uscReslChange.VisibleType Then
            _uscReslChange.BindType()
        End If

        'oggetto privacy
        If _uscReslChange.VisibleObjectPrivacy() Then
            _uscReslChange.BindObjectPrivacy()
        End If
        'invio servizi
        If _uscReslChange.VisibleProposerProtocolLink Then
            _uscReslChange.BindProposerProtocolLink()
        End If

        ' Protocollo lettera pubblicazione
        If _uscReslChange.VisiblePublicationLetterProtocolLink Then
            _uscReslChange.BindPublicationLetterProtocolLink()
        End If

        'OC lista
        If _uscReslChange.VisibleOCList Then
            _uscReslChange.BindOCList()
        End If
        'OC collegio sindacale
        If _uscReslChange.VisibleOCSupervisoryBoard Then
            _uscReslChange.BindOCSupervisoryBoard()
        End If
        If _uscReslChange.VisibleOCConfSindaci Then
            _uscReslChange.BindOCConfSindaci()
        End If
        'OC regione
        'If _uscReslChange.VisibleOCRegion Then
        _uscReslChange.BindOCRegion()
        'End If
        'OC gestione
        'If _uscReslChange.VisibleOCManagement Then
        _uscReslChange.BindOCManagement()
        'End If
        'OC altri
        'If _uscReslChange.VisibleOCOther Then
        _uscReslChange.BindOCOther()
        'End If

        'ImmediatelyExecutive
        If _uscReslChange.VisibleImmediatelyExecutive Then
            _uscReslChange.BindImmediatelyExecutive()
        End If

        'CorteDeiConti
        If _uscReslChange.VisibleCorteDeiConti Then _uscReslChange.BindCorteDeiConti()
    End Sub
#End Region

#Region "Validate Data"
    Public Overridable Function ValidateData(ByRef errorMessage As String) As Boolean Implements IChangerController(Of Resolution).ValidateData
        Return _uscReslChange.ValidateObject(errorMessage)
    End Function
#End Region

#Region "BindDataToObject"
    Protected Overridable Function GetChangedObjectData() As Resolution
        Return _uscReslChange.GetChangedObjectData()
    End Function

    Public Overridable Sub BindDataToObject(ByRef domainObject As Resolution) Implements IInsertController(Of Resolution).BindDataToObject
        _objChangedData = GetChangedObjectData()

        'category
        If _uscReslChange.VisibleCategory Then
            domainObject.SubCategory = _objChangedData.SubCategory
        End If

        'container
        If _uscReslChange.VisibleContainer Then
            domainObject.Container = _objChangedData.Container
        End If
        'economic data
        If _uscReslChange.VisibleEconomicData Then
            domainObject.Position = _objChangedData.Position
            domainObject.ValidityDateFrom = _objChangedData.ValidityDateFrom
            domainObject.ValidityDateTo = _objChangedData.ValidityDateTo
            domainObject.BidType = _objChangedData.BidType
            domainObject.SupplierCode = _objChangedData.SupplierCode
            domainObject.SupplierDescription = _objChangedData.SupplierDescription
        End If
        'note
        If _uscReslChange.VisibleNote Then
            domainObject.Note = _objChangedData.Note
        End If
        'oggetto
        If _uscReslChange.VisibleObject Then
            domainObject.ResolutionObject = _objChangedData.ResolutionObject
        End If
        'OC
        If _uscReslChange.VisibleOC Then
            domainObject.WarningDate = _objChangedData.WarningDate
            domainObject.WarningProtocol = _objChangedData.WarningProtocol
            domainObject.ConfirmDate = _objChangedData.ConfirmDate
            domainObject.ConfirmProtocol = _objChangedData.ConfirmProtocol
            domainObject.WaitDate = _objChangedData.WaitDate
            domainObject.ResponseDate = _objChangedData.ResponseDate
            domainObject.ResponseProtocol = _objChangedData.ResponseProtocol
            domainObject.ControllerStatus = _objChangedData.ControllerStatus
            domainObject.ControllerOpinion = _objChangedData.ControllerOpinion
            domainObject.File.IdControllerFile = _objChangedData.File.IdControllerFile
        End If

        If _uscReslChange.VisiblePublicationLetterProtocolLink Then
            domainObject.PublishingProtocolLink = _objChangedData.PublishingProtocolLink
        End If
        'pubblication
        If _uscReslChange.VisiblePublication And ResolutionEnv.IsPublicationEnabled Then
            domainObject.CheckPublication = _objChangedData.CheckPublication
        End If
        'status
        If _uscReslChange.VisibleStatus Then
            domainObject.Status = _objChangedData.Status
            domainObject.LastChangedDate = _objChangedData.LastChangedDate
            domainObject.LastChangedReason = _objChangedData.LastChangedReason
        End If
        'type
        If _uscReslChange.VisibleType Then
            domainObject.Type = _objChangedData.Type
        End If

        'contact recipient
        If _uscReslChange.VisibleContactRecipient Then
            If _uscReslChange.VisibleContactRecipientAddress Then
                ChangeContactList(domainObject, _objChangedData.ResolutionContactsRecipients, "D")
            End If
            If _uscReslChange.VisibleContactAlternativeRecipient Then
                domainObject.AlternativeRecipient = _objChangedData.AlternativeRecipient
            End If
        End If

        'role proposer
        If _uscReslChange.RoleProposerEnabled Then
            domainObject.AlternativeProposer = _objChangedData.AlternativeProposer
            If _objChangedData.RoleProposer IsNot Nothing Then
                domainObject.RoleProposer = _objChangedData.RoleProposer
            End If
        End If

        'contact proposer
        If Not _uscReslChange.RoleProposerEnabled AndAlso _uscReslChange.VisibleContactProposer Then
            If _uscReslChange.VisibleContactProposerAddress Then
                ChangeContactList(domainObject, _objChangedData.ResolutionContactProposers, "P")
            End If
            If _uscReslChange.VisibleContactAlternativeProposer Then
                domainObject.AlternativeProposer = _objChangedData.AlternativeProposer
            End If
        End If

        'contact assignee
        If _uscReslChange.VisibleContactAssignee Then
            If _uscReslChange.VisibleContactAssigneeAddress Then
                ChangeContactList(domainObject, _objChangedData.ResolutionContactsAssignees, "A")
            End If
            If _uscReslChange.VisibleContactAlternativeAssignee Then
                domainObject.AlternativeAssignee = _objChangedData.AlternativeAssignee
            End If
        End If
        'contact manager
        If _uscReslChange.VisibleContactManager Then
            If _uscReslChange.VisibleContactManagerAddress Then
                ChangeContactList(domainObject, _objChangedData.ResolutionContactsManagers, "R")
            End If
            If _uscReslChange.VisibleContactAlternativeManager Then
                domainObject.AlternativeManager = _objChangedData.AlternativeManager
            End If
        End If

        'CorteDeiConti
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
    End Sub

    Private Sub ChangeContactList(ByRef resolution As Resolution, ByVal newContactList As IList(Of ResolutionContact), ByVal comunicationType As String)

        Dim toremove As New List(Of ResolutionContact)
        For i As Integer = 0 To resolution.ResolutionContacts.Count - 1
            Dim contact As ResolutionContact = resolution.ResolutionContacts.ElementAt(i)
            Dim remove As Boolean = True
            If contact.ComunicationType.Equals(comunicationType) Then
                For Each newC As ResolutionContact In newContactList
                    If newC.Id.ComunicationType.Equals(contact.Id.ComunicationType) And newC.Id.IdContact.Equals(contact.Id.IdContact) Then
                        remove = False
                        Exit For
                    End If
                Next
                If remove Then
                    toremove.Add(contact)
                    ' AJG 20101215: ma come si fa a rimuovere elementi dalla collection su cui si sta eseguendo il ciclo?!
                    'resolution.ResolutionContacts.Remove(contact)
                End If
            End If
        Next
        For Each item As ResolutionContact In toremove
            resolution.ResolutionContacts.Remove(item)
        Next

        For Each contact As ResolutionContact In newContactList
            Select Case comunicationType
                Case "D"
                    resolution.AddRecipient(contact.Contact)
                Case "P"
                    resolution.AddProposer(contact.Contact)
                Case "A"
                    resolution.AddAssignee(contact.Contact)
                Case "R"
                    resolution.AddManager(contact.Contact)
            End Select
        Next
    End Sub
#End Region

#Region "Delegates"
    Protected Delegate Function GetStatusListDelegate() As IList(Of ResolutionStatus)
    Protected GetStatusList As GetStatusListDelegate

    Protected Overridable Sub InitializeDelegates() Implements IInsertController(Of Resolution).InitializeDelegates
        GetStatusList = New GetStatusListDelegate(AddressOf Facade.ResolutionStatusFacade.GetStatusList)
    End Sub
#End Region

#Region "AttachEvents"
    Protected Overridable Sub AttachEvents() Implements IInsertController(Of Resolution).AttachEvents
        _uscReslChange.AttachEvent(uscResolutionChange.ResolutionChangeEventType.StatusSelectedChangedEvent)
        _uscReslChange.AttachEvent(uscResolutionChange.ResolutionChangeEventType.ConfirmDateSelectedChangedEvent)
    End Sub

#End Region

End Class
