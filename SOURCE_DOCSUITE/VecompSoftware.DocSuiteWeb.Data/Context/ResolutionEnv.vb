Imports System.Linq

Public Class ResolutionEnv
    Inherits BaseEnvironment

#Region " Fields "

    Private Const DefaultConnectionStringName As String = "ReslConnection"
    ''' <summary> Testo di default del body di una mail di ODG </summary>
    Const DefaultOdgMailData As String = "<b>%ProductName% - Gestione Documentale</b><br /><br />Allego:<br /><br /><a href='%MailAppPath%?Tipo=Resl&Azione=OdG&Identificativo=%IdODG%'>%Intestazione%</a>"

#End Region

#Region " Constructor "

    Public Sub New(ByRef pContext As DocSuiteContext)
        MyBase.New(DefaultConnectionStringName, pContext)
    End Sub

    Public Sub New(ByRef pContext As DocSuiteContext, ByRef pParameters As IEnumerable(Of ParameterEnv))
        MyBase.New(DefaultConnectionStringName, pContext, pParameters)
    End Sub

#End Region

    Public ReadOnly Property IsInteropEnabled() As Boolean
        Get
            Return GetBoolean("InteropEnabled")
        End Get

    End Property

    Public ReadOnly Property Configuration() As String
        Get
            Return GetString("Configuration", "")
        End Get
    End Property

    Public ReadOnly Property IsLogEnabled() As Boolean
        Get
            Return GetBoolean("LogEnabled")
        End Get
    End Property

    Public ReadOnly Property ResolutionDomain() As String
        Get
            Return GetString("Domain", String.Empty)
        End Get
    End Property

    Public ReadOnly Property InsertDisclaimer() As String
        Get
            Return GetString("InsertDisclaimer", String.Empty)
        End Get
    End Property

    Public ReadOnly Property IsFDQEnabled() As Boolean
        Get
            Return GetBoolean("FDQEnabled")
        End Get
    End Property

    Public ReadOnly Property EnvGroupExtraction() As String
        Get
            Return GetString("GroupExtraction", String.Empty)
        End Get
    End Property

    Public ReadOnly Property EnvGroupStatistics() As String
        Get
            Return GetString("GroupStatistics", String.Empty)
        End Get
    End Property

    Public ReadOnly Property EnvGroupProposerFull() As String
        Get
            Return GetString("GroupProposerFull", String.Empty)
        End Get
    End Property

    Public ReadOnly Property EnvGroupPropostaAtto() As String
        Get
            Return GetString("GroupPropostaAtto", String.Empty)
        End Get
    End Property

    ''' <summary> Numero di giorni da considerare a partire dall'attuale nelle viste per la scrivania. </summary>
    Public ReadOnly Property DesktopDayDiff() As Integer
        Get
            Return GetInteger("DesktopDayDiff", 30)
        End Get
    End Property

    ''' <summary> Numero massimo di record da ritirare. </summary>
    Public ReadOnly Property DesktopMaxRecords() As Integer
        Get
            Return GetInteger("DesktopMaxRecords", 300)
        End Get
    End Property

    Public ReadOnly Property CorporateAcronym() As String
        Get
            Return GetString("CorporateAcronym", String.Empty)
        End Get
    End Property

    Public ReadOnly Property SignatureType() As Integer
        Get
            Return GetInteger("SignatureType")
        End Get
    End Property

    ReadOnly Property IsInsertAdoption() As Boolean
        Get
            Return GetBoolean("InsertAdoption")
        End Get
    End Property

    Public ReadOnly Property AuthorizInsert() As Integer
        Get
            Return GetInteger("AuthorizInsert")
        End Get
    End Property

    Public ReadOnly Property WebPublishPanel() As Boolean
        Get
            Return GetBoolean("WebPublishPanel")
        End Get
    End Property

    Public ReadOnly Property SearchMaxRecords() As Integer
        Get
            Return GetInteger("SearchMaxRecords", 100)
        End Get
    End Property

    Public ReadOnly Property Security() As Boolean
        Get
            Return GetBoolean("Security")
        End Get
    End Property

    ''' <summary> Indica se controllare il diritto di adozione sul contenitore in fase di stampa </summary>
    Public ReadOnly Property SecurityPrint() As Boolean
        Get
            Return GetBoolean("SecurityPrint", True)
        End Get
    End Property

    Public ReadOnly Property IsUserErrorEnabled() As Boolean
        Get
            Return GetBoolean("UserErrorEnabled")
        End Get
    End Property

    Public ReadOnly Property IsInsertPreViewEnabled() As Boolean
        Get
            Return GetBoolean("InsertPreViewEnabled")
        End Get
    End Property

    Public ReadOnly Property IsPreViewDocEnabled() As Boolean
        Get
            Return GetBoolean("PreViewDocEnabled")
        End Get
    End Property

    Public ReadOnly Property IsPublicationEnabled() As Boolean
        Get
            Return GetBoolean("PublicationEnabled")
        End Get
    End Property

    ReadOnly Property ObjectMinLength() As Integer
        Get
            Return GetInteger("ObjectMinLength")
        End Get
    End Property


    ReadOnly Property ObjectMaxLength() As Integer
        Get
            Return GetInteger("ObjectMaxLength", 511)
        End Get
    End Property

    Public ReadOnly Property IsInsertDuplicateEnabled() As Boolean
        Get
            Return GetBoolean("InsertDuplicate")
        End Get
    End Property

    Public ReadOnly Property IsSendMailEnabled() As Boolean
        Get
            Return GetBoolean("SendMailEnabled")
        End Get
    End Property

    Public ReadOnly Property MailSmtpServer() As String
        Get
            Return GetString("MailSmtpServer")
        End Get
    End Property

    Public ReadOnly Property SendMailFrom() As String
        Get
            Return GetString("SendMailFrom")
        End Get
    End Property

    Public ReadOnly Property OCDetermMailTo() As String
        Get
            Return GetString("OCDetermMailTo")
        End Get
    End Property

    Public ReadOnly Property SupervisoryMailTo() As String
        Get
            Return GetString("SupervisoryMailTo")
        End Get
    End Property

    Public ReadOnly Property ManagementMailTo() As String
        Get
            Return GetString("ManagementMailTo")
        End Get
    End Property

    ''' <summary> Contatto rubrica dal quale far partire le ricerche contatti per proponenti. </summary>
    Public ReadOnly Property ProposerContact() As Integer?
        Get
            Return GetNullableInt("ProposerContact")
        End Get
    End Property

    Public ReadOnly Property CategoryRoot() As Integer
        Get
            Return GetInteger("CategoryRoot")
        End Get
    End Property

    Public ReadOnly Property GroupOmissis() As String
        Get
            Return GetString("GroupOmissis")
        End Get
    End Property

    Public ReadOnly Property WebPublishEnabled() As Boolean
        Get
            Return GetBoolean("WebPublishEnabled")
        End Get
    End Property

    Public ReadOnly Property WebPublishFolder() As String
        Get
            Return GetString("WebPublishFolder")
        End Get
    End Property

    Public ReadOnly Property WebPublishHTMLFile() As String
        Get
            Return GetString("WebPublishHTMLFile")
        End Get
    End Property
    Public ReadOnly Property WebPublishSign() As String
        Get
            Return GetString("WebPublishSign")
        End Get
    End Property

    Public ReadOnly Property WebPublishSignTag() As String
        Get
            Return GetString("WebPublishSignTag")
        End Get
    End Property

    Public ReadOnly Property WebPublishWatermark() As String
        Get
            Return GetString("WebPublishWatermark")
        End Get
    End Property

    Public ReadOnly Property GestioneDigitaleDetermine() As Boolean
        Get
            Return GetBoolean("GestioneDigitaleDetermine")
        End Get
    End Property

    Public ReadOnly Property GestioneDigitaleDelibere() As Boolean
        Get
            Return GetBoolean("GestioneDigitaleDelibere")
        End Get
    End Property

    ' TODO: spostare da qui
    Public Function GestioneDigitale(ByVal resl As Resolution) As Boolean
        Select Case resl.Type.Id
            Case 0
                Return DocSuiteContext.Current.ProtocolEnv.PdfPrint And GestioneDigitaleDetermine
            Case 1
                Return DocSuiteContext.Current.ProtocolEnv.PdfPrint And GestioneDigitaleDelibere
        End Select
    End Function

    Public ReadOnly Property PromemoriaAdozione() As String
        Get
            Return GetString("PromemoriaAdozione", "Usare stampa frontalino per stamparlo.")
        End Get
    End Property

    Public ReadOnly Property CMVGroupEnabled() As Boolean
        Get
            Return GetBoolean("CMVGroupEnabled", False)
        End Get
    End Property

    Public ReadOnly Property DromedianWebParameters() As String
        Get
            Return GetString("DromedianWebParameters")
        End Get
    End Property

    Public ReadOnly Property CmvGroupParameters() As String
        Get
            Return GetString("CmvGroupParameters")
        End Get
    End Property

    ''' <summary> Inibisce l'inserimento di proponente alternativo in inserimento atti. </summary>
    Public ReadOnly Property HideAlternativeProposer() As Boolean
        Get
            Return GetBoolean("HideAlternativeProposer")
        End Get
    End Property

    Public ReadOnly Property CustomWorkflowXml() As String
        Get
            Return GetString("CustomWorkflowXml")
        End Get
    End Property

    Public ReadOnly Property DigitalLastPage() As Boolean
        Get
            Return GetBoolean("DigitalLastPage")
        End Get
    End Property

    ' TODO: spostare da qui
    Public ReadOnly Property DefaultAuthorizationRoles() As IList(Of Role)
        Get
            Dim ids As String = GetString("DefaultAuthorizationRoles")
            If Not String.IsNullOrEmpty(ids) Then
                Dim dao As New NHibernateRoleDao("ReslDB")
                Dim list As IList(Of Int32) = New List(Of Int32)
                For Each id As String In ids.Split(",|;".ToCharArray())
                    list.Add(Integer.Parse(id))
                Next
                Return dao.GetByIds(list)
            End If

            Return New List(Of Role)
        End Get

    End Property

    ' TODO: spostare da qui
    Public Function GetDefaultCategory(ByVal resolutionType As Integer) As Category
        Dim idCategory As Integer = GetInteger("DefaultCategory" + resolutionType.ToString())
        If idCategory <> 0 Then
            Dim dao As New NHibernateCategoryDao("ReslDB")
            Return dao.GetById(idCategory, False)
        End If
        Return Nothing
    End Function

    Public ReadOnly Property ManageDefaultCategory As Boolean
        Get
            For i As Integer = 0 To 2
                If GetInteger("DefaultCategory" & i.ToString()) <> 0 Then
                    Return True
                End If
            Next
            Return False
        End Get
    End Property

    ' TODO: spostare da qui
    Public Function GetDefaultContainer(ByVal resolutionType As Short) As Container
        Dim idCategory As Integer = GetInteger("DefaultContainer" + resolutionType.ToString())
        If idCategory <> 0 Then
            Dim dao As New NHibernateContainerDao("ReslDB")
            Return dao.GetById(idCategory, False)
        Else
            Return Nothing
        End If
    End Function

    Public ReadOnly Property ManageDefaultContainer As Boolean
        Get
            For i As Integer = 0 To 2
                If GetInteger("DefaultContainer" + i.ToString()) <> 0 Then
                    Return True
                End If
            Next
            Return False
        End Get
    End Property

    Public ReadOnly Property CopyProtocolDocumentsEnabled() As Boolean
        Get
            Return GetBoolean("CopyProtocolDocumentsEnabled")
        End Get
    End Property

    Public ReadOnly Property CopyReslDocumentsEnabled() As Boolean
        Get
            Return GetBoolean("CopyReslDocumentsEnabled")
        End Get
    End Property

    Public ReadOnly Property UseContainerResolutionType() As Boolean
        Get
            Return GetBoolean("UseContainerResolutionType")
        End Get
    End Property

    Public ReadOnly Property ConservationFieldDocument() As String
        Get
            Return GetString("ConservationFieldDocument", String.Empty)
        End Get
    End Property

    Public ReadOnly Property ConservationFieldAttachment() As String
        Get
            Return GetString("ConservationFieldAttachment", String.Empty)
        End Get
    End Property

    Public ReadOnly Property DigitalLastPageGroup() As String
        Get
            Return GetString("DigitalLastPageGroup", String.Empty)
        End Get
    End Property
    Public ReadOnly Property ResolutionJournalGroup() As String
        Get
            Return GetString("ResolutionJournalGroup", String.Empty)
        End Get
    End Property

    Public ReadOnly Property ReslPubJournalGroup() As String
        Get
            Return GetString("ReslPubJournalGroup", String.Empty)
        End Get
    End Property

    Public ReadOnly Property ResolutionJournalSigners() As String
        Get
            Return GetString("ResolutionJournalSigners", String.Empty)
        End Get
    End Property

    ''' <summary> Insieme di email alle quali verrà mandata la PEC con le delibere adottate. </summary>
    Public ReadOnly Property EmailCollegioSindacale() As String
        Get
            Return GetString("EmailCollegioSindacale", String.Empty)
        End Get
    End Property

    ''' <summary> Oggetto della PEC con le delibere adottate. </summary>
    Public ReadOnly Property OggettoCollegioSindacale() As String
        Get
            Return GetString("OggettoCollegioSindacale", String.Empty)
        End Get
    End Property

    ''' <summary> Testo della PEC con le delibere adottate. </summary>
    Public ReadOnly Property TestoCollegioSindacale() As String
        Get
            Return GetString("TestoCollegioSindacale", String.Empty)
        End Get
    End Property

    ' TODO: spostare da qui
    ''' <summary> MailBox dove risiedono le PEC inviate al collegio sindacale. </summary>
    ''' <returns> Deve essere abilitata la PEC e il database dei protocolli. </returns>
    Public ReadOnly Property MailBoxCollegioSindacale() As PECMailBox
        Get
            Dim mailBox As PECMailBox = Nothing
            If DocSuiteContext.Current.IsProtocolEnabled Then

                Dim dao As New NHibernatePECMailBoxDao(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
                mailBox = dao.GetById(GetShort("MailBoxCollegioSindacale"), False)
            End If
            Return mailBox
        End Get
    End Property

    ' TODO: spostare da qui
    ''' <summary> PECMailBox da usare per parcheggiare una PEC in attesa di essere spedita. </summary>
    ''' <remarks>Deve essere abilitata la PEC e il database dei protocolli. Creata per la funzione del collegio sindacale torino, può essere usata per parcheggiare qualsiasi PEC relativa agli atti.</remarks>
    Public ReadOnly Property MailBoxBozze() As PECMailBox
        Get
            Dim mailBox As PECMailBox = Nothing
            If DocSuiteContext.Current.IsProtocolEnabled Then
                Dim dao As New NHibernatePECMailBoxDao(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
                mailBox = dao.GetById(GetShort("MailBoxBozze"), False)
            End If
            Return mailBox
        End Get
    End Property

    ''' <summary> Gruppi abilitati alla gestione delle PEC con le delibere adottate. </summary>
    Public ReadOnly Property GruppiCollegioSindacale() As String
        Get
            Return GetString("GruppiCollegioSindacale", String.Empty)
        End Get
    End Property

    ''' <summary> Abilita la numerazione degli atti per gli anni antecedenti quello correntemente in gestione. </summary>
    Public ReadOnly Property PastYearsEnumerationEnabled() As Boolean
        Get
            Return GetBoolean("PastYearsEnumerationEnabled")
        End Get
    End Property

    ''' <summary> Gruppi che possono firmare il ritiro degli Atti. </summary>
    Public ReadOnly Property RitiraAttiGroups() As String
        Get
            Return GetString("RitiraAttiGroups", String.Empty)
        End Get
    End Property

    Public ReadOnly Property WebPublicationPrint() As Boolean
        Get
            Return GetBoolean("WebPublicationPrint")
        End Get
    End Property

    Public ReadOnly Property WebPublicationPrintBaseAddress() As String
        Get
            Return GetString("WebPublicationPrintBaseAddress", Nothing)
        End Get
    End Property

    Public ReadOnly Property ResolutionJournalShowOnlyActive() As Boolean
        Get
            Return GetBoolean("ReslJournalOnlyActive")
        End Get
    End Property

    ''' <summary> Indica di quale step recuperare le catene da allegare alla gestione WebDocumentSource. (Pubblicazione Torino?). </summary>
    Public ReadOnly Property WebDocumentSourceWorkflowStep As Short
        Get
            Return GetShort("WebDocumentSourceWorkflowStep", 2)
        End Get
    End Property

    Public ReadOnly Property WindowWidthRegistri() As Integer
        Get
            Return GetInteger("WindowWidthRegistri", 800)
        End Get
    End Property

    Public ReadOnly Property WindowHeightRegistri() As Integer
        Get
            Return GetInteger("WindowHeighhRegistri", 500)
        End Get
    End Property

    Public ReadOnly Property UseWindowPreview() As Boolean
        Get
            Return GetBoolean("UseWindowPreview", False)
        End Get
    End Property

    ''' <summary> Definisce il valore di default (a livello aziendale) da selezionare per la copia da atto </summary>
    Public ReadOnly Property ReslAllegaDefaultType() As Integer
        Get
            Return GetInteger("ReslAllegaDefaultType", 0)
        End Get
    End Property

    Public ReadOnly Property ButtonSeriesTitle As String
        Get
            Return GetString("ButtonSeriesTitle", "Aggiungi Amministrazione Trasparente")
        End Get
    End Property

    Public ReadOnly Property RegistroProvvedimentiAdottatiLastSignOnly As Boolean
        Get
            Return GetBoolean("RegistroProvvedimentiAdottatiLastSignOnly", False)
        End Get
    End Property

    Public ReadOnly Property PubblicazioneOnLineSignature() As String
        Get
            Return GetString("PubblicazioneOnLineSignature", "(Pubblicazione Albo on-line)")
        End Get
    End Property

    ''' <summary> Definisce se visualizzare negli atti il collegamento alla collaborazione. </summary>
    Public ReadOnly Property ShowCollaboration As Boolean
        Get
            Return GetBoolean("ShowCollaboration", False)
        End Get
    End Property

    ''' <summary> Indica se mostrare a tutti un atto successivamente alla pubblicazione. </summary>
    Public ReadOnly Property ShowAfterPubblication() As Boolean
        Get
            Return GetBoolean("ShowAfterPubblication", False)
        End Get
    End Property

    Public ReadOnly Property ProposalViewable() As Boolean
        Get
            Return GetBoolean("ProposalViewable", True)
        End Get
    End Property

    Public ReadOnly Property CollResolutionRole() As String
        Get
            Return GetString("CollResolutionRole", String.Empty)
        End Get
    End Property

    Public ReadOnly Property DisableCheckTuttiICodici() As Boolean
        Get
            Return GetBoolean("DisableCheckTuttiICodici", False)
        End Get
    End Property

    Public ReadOnly Property AuthorizInsertType() As Integer
        Get
            Return GetInteger("AuthorizInsertType", 1)
        End Get
    End Property

    Public ReadOnly Property DefaultResolutionRoleType() As Integer
        Get
            Return GetInteger("DefaultResolutionRoleType", 1)
        End Get
    End Property

    Public ReadOnly Property MainDocumentOmissisEnable As Boolean
        Get
            Return GetBoolean("MainDocumentOmissisEnable", False)
        End Get
    End Property

    Public ReadOnly Property AttachmentOmissisEnable As Boolean
        Get
            Return GetBoolean("AttachmentOmissisEnable", False)
        End Get
    End Property

    Public ReadOnly Property QuickDocumentsCheckAllChains As Boolean
        Get
            Return GetBoolean("QuickDocumentsCheckAllChains", True)
        End Get
    End Property

    Public ReadOnly Property ReslProtocolStartYear As Short
        Get
            Return GetShort("ReslProtocolStartYear", 2010)
        End Get
    End Property

    Public ReadOnly Property ReslProtocolMaxResults As Integer
        Get
            Return GetInteger("ReslProtocolMaxResults", 10)
        End Get
    End Property

    Public ReadOnly Property CopyDocumentsToAdoption As Boolean
        Get
            Return GetBoolean("CopyDocumentsToAdoption", False)
        End Get
    End Property
    Public ReadOnly Property CopyOnlySignedDocumentsToAdoption As Boolean
        Get
            Return GetBoolean("CopyOnlySignedDocumentsToAdoption", False)
        End Get
    End Property

    Public ReadOnly Property CheckAdoptionDateCommands As String()
        Get
            Dim commands As String = GetString("CheckAdoptionDateCommands", String.Empty)
            If Not String.IsNullOrEmpty(commands) Then
                Return commands.Split("|"c).ToArray()
            End If

            Return New String() {}
        End Get
    End Property

    Public ReadOnly Property CopyFromResolutionSearchByServiceNumberAsLike As Boolean
        Get
            Return GetBoolean("CopyFromResolutionSearchByServiceNumberAsLike", False)
        End Get
    End Property

    Public ReadOnly Property DisableButtonFrontPubblicazione As Boolean
        Get
            Return GetBoolean("DisableButtonFrontPubblicazione", False)
        End Get
    End Property

    Public ReadOnly Property EnableFlushAnnexed As Boolean
        Get
            Return GetBoolean("EnableFlushAnnexed", False)
        End Get
    End Property

    Public ReadOnly Property ResolutionKindEnabled As Boolean
        Get
            Return GetBoolean("ResolutionKindEnabled", False)
        End Get
    End Property

    Public ReadOnly Property ViewResolutionProposedByRoleEnabled As Boolean
        Get
            Return GetBoolean("ViewResolutionProposedByRoleEnabled", False)
        End Get
    End Property

    Public ReadOnly Property EnvGroupTblRoleTypeResl() As String
        Get
            Return GetString("EnvGroupTblRoleTypeResl", String.Empty)
        End Get
    End Property

    Public ReadOnly Property AutomaticConfirmAvcpSeries() As Boolean
        Get
            Return GetBoolean("AutomaticConfirmAvcpSeries", True)
        End Get
    End Property

    Public ReadOnly Property DeclineNoteViewIndex As String
        Get
            Return GetString("DeclineNoteViewIndex", String.Empty)
        End Get
    End Property

    Public ReadOnly Property MustResolutionAdoptionDocumentIsSigned As Boolean
        Get
            Return GetBoolean("MustResolutionAdoptionDocumentIsSigned", False)
        End Get
    End Property

    Public ReadOnly Property LoadProposalDocumentInAdoptionState As Boolean
        Get
            Return GetBoolean("LoadProposalDocumentInAdoptionState", False)
        End Get
    End Property

    Public ReadOnly Property LoadAdoptionDocumentInPublicationState As Boolean
        Get
            Return GetBoolean("LoadAdoptionDocumentInPublicationState", False)
        End Get
    End Property

    Public ReadOnly Property ResolutionObjectPrivacyLabel As String
        Get
            Return GetString("ResolutionObjectPrivacyLabel", "Oggetto Privacy")
        End Get
    End Property

    Public ReadOnly Property GenerateFrontalinoInAdoptionState As Boolean
        Get
            Return GetBoolean("GenerateFrontalinoInAdoptionState", False)
        End Get
    End Property

    Public ReadOnly Property CheckOCValidations As Boolean
        Get
            Return GetBoolean("CheckOCValidations", False)
        End Get
    End Property

    Public ReadOnly Property HideDocumentButtons As Boolean
        Get
            Return GetBoolean("HideDocumentButtons", True)
        End Get
    End Property

    Public ReadOnly Property ControlCheckResolutionEnabled As Boolean
        Get
            Return GetBoolean("ControlCheckResolutionEnabled", False)
        End Get
    End Property

    Public ReadOnly Property CustomFrontespiecePrintEnabled As Boolean
        Get
            Return GetBoolean("CustomFrontespiecePrintEnabled", False)
        End Get
    End Property

    Public ReadOnly Property ForceSharePointPublication As Boolean
        Get
            Return GetBoolean("ForceSharePointPublication", False)
        End Get
    End Property

    Public ReadOnly Property CheckPublishDocumentSigned As Boolean
        Get
            Return GetBoolean("CheckPublishDocumentSigned", True)
        End Get
    End Property

    Public ReadOnly Property ApplyGeneralistUserDeskRight As Boolean
        Get
            Return GetBoolean("ApplyGeneralistUserDeskRight", True)
        End Get
    End Property

    Public ReadOnly Property CheckPublicationOrder As Boolean
        Get
            Return GetBoolean("CheckPublicationOrder", False)
        End Get
    End Property
    Public ReadOnly Property CustomControllerFactoryEnabled As Boolean
        Get
            Return GetBoolean("CustomControllerFactoryEnabled", False)
        End Get
    End Property


    ''' <summary> Inibisce l'inserimento di destinatario alternativo in inserimento atti. </summary>
    Public ReadOnly Property HideAlternativeRecipient() As Boolean
        Get
            Return GetBoolean("HideAlternativeRecipient")
        End Get
    End Property

    Public ReadOnly Property PublicationEndDays As Integer
        Get
            Return GetInteger("PublicationEndDays", 15)
        End Get
    End Property

    Public ReadOnly Property AdoptionComparisonDate As String
        Get
            Return GetString("AdoptionComparisonDate", String.Empty)
        End Get
    End Property

    Public ReadOnly Property CheckRoleButtonRightsEnabled As Boolean
        Get
            Return GetBoolean("CheckRoleButtonRightsEnabled", False)
        End Get
    End Property

    Public ReadOnly Property IncrementalNumberEnabled As Boolean
        Get
            Return GetBoolean("IncrementalNumberEnabled", False)
        End Get
    End Property

    Public ReadOnly Property ViewAllExecutiveEnabled As Boolean
        Get
            Return GetBoolean("ViewAllExecutiveEnabled", False)
        End Get
    End Property

    Public ReadOnly Property InclusiveNumberWithProposerCodeEnabled As Boolean
        Get
            Return GetBoolean("InclusiveNumberWithProposerCodeEnabled", False)
        End Get
    End Property

    Public ReadOnly Property AuthorizInsertProposerRolesEnabled() As Boolean
        Get
            Return GetBoolean("AuthorizInsertProposerRolesEnabled")
        End Get
    End Property
    Public ReadOnly Property ModifyExecutiveResolutionEnabled As Boolean
        Get
            Return GetBoolean("ModifyExecutiveResolutionEnabled", False)
        End Get
    End Property

    Public ReadOnly Property PublishToOnlineRegisterEnabled As Boolean
        Get
            Return GetBoolean("PublishToOnlineRegisterEnabled", False)
        End Get
    End Property

    Public ReadOnly Property ShowMassiveResolutionSearchPageEnabled As Boolean
        Get
            Return GetBoolean("ShowMassiveResolutionSearchPageEnabled", False)
        End Get
    End Property

    Public ReadOnly Property ShowResolutionStatisticsEnabled As Boolean
        Get
            Return GetBoolean("ShowResolutionStatisticsEnabled", False)
        End Get
    End Property

    Public ReadOnly Property ShowExecutiveDocumentEnabled As Boolean
        Get
            Return GetBoolean("ShowExecutiveDocumentEnabled", False)
        End Get
    End Property

    Public ReadOnly Property PrintTemplatePath As String
        Get
            Return GetString("PrintTemplatePath", String.Empty)
        End Get
    End Property

    Public ReadOnly Property UniqueLastPageGenerationEnabled As Boolean
        Get
            Return GetBoolean("UniqueLastPageGenerationEnabled", False)
        End Get
    End Property

    Public ReadOnly Property ExpirationDateSignatureEnabled As Boolean
        Get
            Return GetBoolean("ExpirationDateSignatureEnabled", False)
        End Get
    End Property

    Public ReadOnly Property PanelOCDEnabled() As Boolean
        Get
            Return GetBoolean("PanelOCDEnabled", False)
        End Get
    End Property

    Public ReadOnly Property CompleteTransparencyExecutiveStepEnabled As Boolean
        Get
            Return GetBoolean("CompleteTransparencyExecutiveStepEnabled", False)
        End Get
    End Property
    Public ReadOnly Property MailMassiveLastPageCreatedEnabled As Boolean
        Get
            Return GetBoolean("MailMassiveLastPageCreatedEnabled", False)
        End Get
    End Property

    ''' <summary> Numero in millisecondi del tempo massimo di generazione frontalino. </summary>
    Public ReadOnly Property ReportPrintTimer As Integer
        Get
            Return GetInteger("ReportPrintTimer", 30000)
        End Get
    End Property

    Public ReadOnly Property DelibereSignsReportEnabled As Boolean
        Get
            Return GetBoolean("DelibereSignsReportEnabled", False)
        End Get
    End Property

    Public ReadOnly Property OCExecutiveModifyGroups As String
        Get
            Return GetString("OCExecutiveModifyGroups", String.Empty)
        End Get
    End Property

    Public ReadOnly Property ResolutionJournalEffectivenessDateEnabled As Boolean
        Get
            Return GetBoolean("ResolutionJournalEffectivenessDateEnabled", False)
        End Get
    End Property
    Public ReadOnly Property ResolutionJournalEndPublishingDateEnabled As Boolean
        Get
            Return GetBoolean("ResolutionJournalEndPublishingDateEnabled", False)
        End Get
    End Property
    Public ReadOnly Property DefaulLoadLastPageEnabled As Boolean
        Get
            Return GetBoolean("DefaulLoadLastPageEnabled", False)
        End Get
    End Property

    Public ReadOnly Property AutomaticActivityStepEnabled As Boolean
        Get
            Return GetBoolean("AutomaticActivityStepEnabled", False)
        End Get
    End Property
    Public ReadOnly Property ErrorAutomaticActivitiesGroup As Integer
        Get
            Return GetInteger("ErrorAutomaticActivitiesGroup", 0)
        End Get
    End Property

    Public ReadOnly Property UncomplianceRevokeResolutionEnabled As Boolean
        Get
            Return GetBoolean("UncomplianceRevokeResolutionEnabled", False)
        End Get
    End Property
    Public ReadOnly Property CheckResolutionStep As String
        Get
            Return GetString("CheckResolutionStep", String.Empty)
        End Get
    End Property
    Public ReadOnly Property ResolutionDeclineReasonLabel As String
        Get
            Return GetString("ResolutionDeclineReasonLabel", "Motivazione di annullamento")
        End Get
    End Property
    Public ReadOnly Property ResolutionDeclineTextEnabled As Boolean
        Get
            Return GetBoolean("ResolutionDeclineTextEnabled", False)
        End Get
    End Property
    Public ReadOnly Property ResolutionShowAlwaysAttachments As Boolean
        Get
            Return GetBoolean("ResolutionShowAlwaysAttachments", False)
        End Get
    End Property
    Public ReadOnly Property UltimaPaginaReportSignEnabled As Boolean
        Get
            Return GetBoolean("UltimaPaginaReportSignEnabled", True)
        End Get
    End Property
    Public ReadOnly Property UltimaPaginaRemoteSignInformation As String
        Get
            Return GetString("UltimaPaginaRemoteSignInformation", String.Empty)
        End Get
    End Property
    Public ReadOnly Property ResolutionAccountingEnabled As Boolean
        Get
            Return GetBoolean("ResolutionAccountingEnabled", False)
        End Get
    End Property

    Public ReadOnly Property ImmediatelyExecutiveEnabled As Boolean
        Get
            Return GetBoolean("ImmediatelyExecutiveEnabled", True)
        End Get
    End Property

    Public ReadOnly Property OCOptions As OCOptionsModel
        Get
            Return GetJson(Of OCOptionsModel)("OCOptions", String.Empty)
        End Get
    End Property

    Public ReadOnly Property ResolutionSearchableSteps As List(Of ResolutionStep)
        Get
            Return GetJson(Of List(Of ResolutionStep))("ResolutionSearchableSteps", String.Empty)
        End Get
    End Property

    Public ReadOnly Property ResolutionConfirmViewingRequiredEnabled As Boolean
        Get
            Return GetBoolean("ResolutionConfirmViewingRequiredEnabled", False)
        End Get
    End Property

    Public ReadOnly Property ActiveStepFilterSelectedEnabled As Boolean
        Get
            Return GetBoolean("ActiveStepFilterSelectedEnabled", False)
        End Get
    End Property
End Class