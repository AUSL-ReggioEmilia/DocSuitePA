Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Model.Parameters

Public Class ProtocolEnv
    Inherits BaseEnvironment

#Region " Fields "

    Private _conservationExtensions() As String
    Private Const DefaultConnectionStringName As String = "ProtConnection"

    Private Const DefaultProtocolMailData As String = "<b>DocSuite - Gestione Documentale</b><br /><br />Allego il {0}<br />Oggetto: {1}<br /><br /><a href='{2}?Tipo=Prot&Azione=Apri&Anno={3}&Numero={4}'>{0}</a><br />"
    Private Const DefaultProtocolMailDataWithBody As String = "<b>DocSuite - Gestione Documentale</b><br /><br /><a href='{0}?Tipo=Prot&Azione=Apri&Anno={1}&Numero={2}'>{3}</a><br />"

#End Region

#Region " Constructors "

    Public Sub New(ByRef pContext As DocSuiteContext)
        MyBase.New(DefaultConnectionStringName, pContext)
    End Sub

    Public Sub New(ByRef pContext As DocSuiteContext, ByRef pParameters As IEnumerable(Of ParameterEnv))
        MyBase.New(DefaultConnectionStringName, pContext, pParameters)
    End Sub

#End Region

#Region " Properties "

    Public ReadOnly Property MulticlassificationEnabled() As Boolean
        Get
            Return GetBoolean("MulticlassificationEnabled", False)
        End Get
    End Property

    Public ReadOnly Property EnvGroupStatistics() As String
        Get
            Return GetString("GroupStatistics", String.Empty)
        End Get
    End Property

    Public ReadOnly Property SignaturePrintExt() As String
        Get
            Return GetString("SignaturePrintExt", String.Empty)
        End Get
    End Property


    Public ReadOnly Property EnableFederationAD() As Boolean
        Get
            Return GetBoolean("EnableFederationAD", False)
        End Get
    End Property

    Public ReadOnly Property EnableContactAndDistributionGroup() As Boolean
        Get
            Return GetBoolean("EnableContactAndDistributionGroup", True)
        End Get
    End Property

    Public ReadOnly Property CheckSignedEvaluateStream() As Boolean
        Get
            Return GetBoolean("CheckSignedEvaluateStream", True)
        End Get
    End Property

    Public ReadOnly Property DispositionNotification() As Boolean
        Get
            Return GetBoolean("DispositionNotification")
        End Get
    End Property
    Public ReadOnly Property ShowDispositionNotification() As Boolean
        Get
            Return GetBoolean("ShowDispositionNotification")
        End Get
    End Property
    ' Definisce la visibilitࠤella sezione "Invio per conto di un settore" nell'invio della mail.
    Public ReadOnly Property ShowSendBySectorNotification() As Boolean
        Get
            Return GetBoolean("ShowSendBySectorNotification", False)
        End Get
    End Property

    ' Definisce lo String.Format della firma in calce "Invio per conto di un settore"
    Public ReadOnly Property SendBySectorFormat() As String
        Get
            Return GetString("SendBySectorFormat", "Inviato da {0} per conto del settore: {1}")
        End Get
    End Property

    Public ReadOnly Property ProtRegistrationDateFormat() As String
        Get
            Return GetString("ProtRegistrationDateFormat", "{0:dd/MM/yyyy}")
        End Get
    End Property

    Public ReadOnly Property EnvServiceLogEnabled() As Boolean
        Get
            Return GetBoolean("ServiceLogEnabled")
        End Get
    End Property

    Public ReadOnly Property ProtParzialeEnabled() As Boolean
        Get
            Return GetBoolean("ProtParzialeEnabled")
        End Get
    End Property

    Public ReadOnly Property ProtParzialeRecoveryEnabled() As Boolean
        Get
            Return GetBoolean("ProtParzialeRecoveryEnabled")
        End Get
    End Property

    Public ReadOnly Property PreProtocollazioneEnabled() As Boolean
        Get
            Return GetBoolean("PreProtocollazioneEnabled")
        End Get
    End Property

    Public ReadOnly Property EnvTableDocTypeRequired() As Boolean
        Get
            Return GetBoolean("TableDocTypeRequired")
        End Get
    End Property

    Public ReadOnly Property EnvObjectMinLength() As Integer
        Get
            Return GetInteger("ObjectMinLength", 1)
        End Get
    End Property

    Public ReadOnly Property IsProtocolAttachLocationEnabled() As Boolean
        Get
            Return GetBoolean("ProtocolAttachLocationEnabled")
        End Get
    End Property

    Public ReadOnly Property IsClaimEnabled() As Boolean
        Get
            Return GetBoolean("ClaimEnabled")
        End Get
    End Property

    Public ReadOnly Property EnvTableLogEnabled() As Boolean
        Get
            Return GetBoolean("TableLogEnabled")
        End Get
    End Property

    Public ReadOnly Property EnvUtltRenderingEnabled() As Boolean
        Get
            Return GetBoolean("UtltRenderingEnabled")
        End Get
    End Property

    Public ReadOnly Property IsUserErrorEnabled() As Boolean
        Get
            Return GetBoolean("UserErrorEnabled")
        End Get
    End Property

    ''' <summary> Definisce il gruppo per la gestione dei contatti </summary>
    Public ReadOnly Property EnvGroupTblContact() As String
        Get
            Return GetString("GroupTblContact", String.Empty)
        End Get
    End Property

    Public ReadOnly Property EnvWordEnabled() As Boolean
        Get
            Return GetBoolean("WordEnabled")
        End Get
    End Property

    ''' <summary> Attiva e gestisce il modulo Importazione Lettere Excel. </summary>
    ''' <remarks> Usata anche in ExcelPathImport e ExcelGroupImport. </remarks>
    Public ReadOnly Property EnvExcelEnabled() As Boolean
        Get
            Dim s As String = GetPipedString("ExcelEnabled")
            Return Not String.IsNullOrEmpty(s) AndAlso s.Eq("1")
        End Get
    End Property

    Public ReadOnly Property IsJournalEnabled() As Boolean
        Get
            Return GetBoolean("JournalEnabled")
        End Get
    End Property

    Public ReadOnly Property IsCheckSignEnabled() As Boolean
        Get
            Return GetBoolean("CheckSignEnabled")
        End Get
    End Property

    ''' <summary> Indica se la PEC 蠡bilitata in protocollo </summary>
    Public ReadOnly Property IsPECEnabled() As Boolean
        Get
            Return GetBoolean("PECEnabled")
        End Get
    End Property

    Public ReadOnly Property PECRoleRightDefaultValue As Boolean
        Get
            Return IsPECEnabled AndAlso RoleGroupPECRightEnabled AndAlso GetBoolean("PECRoleRightDefaultValue")
        End Get
    End Property

    Public ReadOnly Property IsPECDestinationEnabled() As Boolean
        Get
            Return GetBoolean("PECDestinationEnabled")
        End Get
    End Property

    Public ReadOnly Property PECContactTypeEnabled() As String
        Get
            Return GetString("PECContactTypeEnabled", "M,A,U,P")
        End Get
    End Property

    Public ReadOnly Property PECDraftMailBoxId() As Short
        Get
            Return GetShort("PECDraftMailBoxId", -1)
        End Get
    End Property

    ''' <summary> Visualizzazione di default delle PEC in Arrivo per criterio di protocollazione </summary>
    Public ReadOnly Property PECDefaultInitialView() As Integer
        Get
            Return GetInteger("PECDefaultInitialView", 0)
        End Get
    End Property

    Public ReadOnly Property SegnaturaClearRegex() As String
        Get
            Return GetString("SegnaturaClearRegex", "[]") ' ATTENZIONE! dentro le quadre c'蠩l carattere "doppio punto esclamativo".
        End Get
    End Property

    Public ReadOnly Property EnvGroupTblCategory() As String
        Get
            Return GetString("GroupTblCategory", String.Empty)
        End Get
    End Property

    Public ReadOnly Property EnvGroupTblContainer() As String
        Get
            Return GetString("GroupTblContainer", String.Empty)
        End Get
    End Property

    Public ReadOnly Property EnvGroupTblRole() As String
        Get
            Return GetString("GroupTblRole", String.Empty)
        End Get
    End Property

    Public ReadOnly Property EnvGroupLogView() As String
        Get
            Return GetString("GroupLogView", String.Empty)
        End Get
    End Property

    Public ReadOnly Property EnvGroupConcourse() As String
        Get
            Return GetString("GroupConcourse", String.Empty)
        End Get
    End Property

    Public ReadOnly Property EnvGroupSuspend() As String
        Get
            Return GetString("GroupSuspend", String.Empty)
        End Get
    End Property

    Public ReadOnly Property EnvGroupAdministrator() As String
        Get
            Return GetString("GroupAdministrator", String.Empty)
        End Get
    End Property

    ''' <summary> Mittente delle PEC di notifica. </summary>
    Public ReadOnly Property ProtPecSendSender() As String
        Get
            Return GetString("ProtPecSendSender", String.Empty)
        End Get
    End Property

    Public ReadOnly Property IsDesktopEnabled() As Boolean
        Get
            Return GetBoolean("DesktopEnabled")
        End Get
    End Property

    ''' <summary> Abilita visualizzazione e modifica delle autorizzazioni full sui settori di protocollo. </summary>
    Public ReadOnly Property IsAuthorizFullEnabled() As Boolean
        Get
            Return GetBoolean("AuthorizFullEnabled")
        End Get
    End Property

    Public ReadOnly Property IsAuthorizInsertEnabled() As Boolean
        Get
            Return GetBoolean("AuthorizInsert")
        End Get
    End Property

    Public ReadOnly Property IsAuthorizInsertRolesEnabled() As Boolean
        Get
            Return GetBoolean("AuthorizInsertRoles")
        End Get
    End Property

    Public ReadOnly Property AuthorizMailBody() As String
        Get
            Return GetString("AuthorizMailBody", String.Empty)
        End Get
    End Property

    ''' <summary> Template lungo dei link ai protocolli generati nelle mail di docsuite. </summary>
    ''' <remarks> {0} Nome completo protocollo. {1} Oggetto del protocollo. {2} Link autogenerato alla docsuite. {3} Anno protocollo. {4} Numero protocollo. </remarks>
    Public ReadOnly Property ProtocolMailData() As String
        Get
            Return GetString("ProtocolMailData", DefaultProtocolMailData)
        End Get
    End Property

    ''' <summary> Template corto dei link ai protocolli generati nelle mail di docsuite. </summary>
    ''' <remarks> {0} Link autogenerato dalla docsuite. {1} Anno protocollo. {2} Numero protocollo. {3} Nome completo protocollo. </remarks>
    Public ReadOnly Property ProtocolMailDataWithBody() As String
        Get
            Return GetString("ProtocolMailDataWithBody", DefaultProtocolMailDataWithBody)
        End Get
    End Property
    ''' <summary>
    ''' Template body mail per autorizzazione  External Viewer
    ''' </summary>
    ''' <returns>{0} Archivio. {1} User. {2} Anno/Numero protocollo. {3} Link External Viewer.</returns>
    Public ReadOnly Property ProtocolMailAuthExternalViewerBody() As String
        Get
            Return GetString("ProtocolMailAuthExternalViewerBody", "Azienda unit࠳anitaria locale di Reggio Emilia<br/>Archivio DocSuite: {0} <br/>Autorizzazione di {1} alla lettura del seguente documento {2} <br/><br/>Per accedere utilizzare il seguente indizzo:{3}")
        End Get
    End Property


    Public ReadOnly Property IsInvoiceEnabled() As Boolean
        Get
            Return GetBoolean("InvoiceEnabled", False)
        End Get
    End Property

    Public ReadOnly Property IsDistributionEnabled() As Boolean
        Get
            Return GetBoolean("DistributionEnabled", False)
        End Get
    End Property

    Public ReadOnly Property IsInterOfficeEnabled() As Boolean
        Get
            Return GetBoolean("InterOfficeEnabled", False)
        End Get
    End Property

    Public ReadOnly Property IsSearchContactEnabled() As Boolean
        Get
            Return GetBoolean("SearchContactEnabled", False)
        End Get
    End Property

    Public ReadOnly Property CopyProtocolDocumentsEnabled() As Boolean
        Get
            Return GetBoolean("CopyProtocolDocumentsEnabled", False)
        End Get
    End Property

    Public ReadOnly Property IsFDQEnabled() As Boolean
        Get
            Return GetBoolean("FDQEnabled")
        End Get
    End Property

    Public ReadOnly Property AuthorizedSecurity() As Integer
        Get
            Dim tmp As Integer = GetInteger("AuthorizSecurity", 2)
            If tmp = 0 Then
                tmp = 2
            End If
            Return tmp
        End Get
    End Property

    ''' <summary> Stringa usata in alto per la signatura dei documenti </summary>
    Public ReadOnly Property SignatureString() As String
        Get
            Return GetString("SignatureString", String.Empty)
        End Get
    End Property

    Public ReadOnly Property HierarchicalCollaboration() As Boolean
        Get
            Return GetBoolean("HierarchicalCollaboration")
        End Get
    End Property

    Public ReadOnly Property CheckCollaborationSigns() As Boolean
        Get
            Return GetBoolean("CheckCollaborationSigns")
        End Get
    End Property

    Public ReadOnly Property ForceProsecutable As Boolean
        Get
            Return GetBoolean("ForceProsecutable", False)
        End Get
    End Property

    Public ReadOnly Property ZebraPrinterEnabled() As Boolean
        Get
            Return IsComputerLogEnabled() AndAlso GetBoolean("ZebraPrinterEnabled")
        End Get
    End Property

    Public ReadOnly Property FDQMultipleShare() As String
        Get
            Dim fdqShare As String = GetString("FDQMultipleShare")
#If DEBUG Then
            'fdqShare = "C:/temp"
#End If
            If Not String.IsNullOrEmpty(fdqShare) AndAlso (fdqShare.Chars(fdqShare.Length - 1) <> "\") Then
                fdqShare += "\"
            End If
            Return fdqShare
        End Get
    End Property

    Public ReadOnly Property CorporateAcronym() As String
        Get
            Return GetString("CorporateAcronym", String.Empty)
        End Get
    End Property

    Public ReadOnly Property CorporateName() As String
        Get
            Return GetString("CorporateName", String.Empty)
        End Get
    End Property

    ''' <summary> Tipo di firma da apporre ai protocolli archiviati su biblos </summary>
    Public ReadOnly Property SignatureType() As Integer
        Get
            Return GetInteger("SignatureType", 0)
        End Get
    End Property

    Public ReadOnly Property CollaborationSignatureType() As Integer
        Get
            Return GetInteger("CollaborationSignatureType")
        End Get
    End Property

    Public ReadOnly Property CollaborationMail() As Boolean
        Get
            Return GetBoolean("CollaborationMail")
        End Get
    End Property

    Public ReadOnly Property CollaborationMultiDocument() As Boolean
        Get
            Return GetBoolean("CollaborationMultiDocument")
        End Get
    End Property

    Public ReadOnly Property CollaborationDocumentEditable() As Boolean
        Get
            Return GetBoolean("CollDocumentEditable")
        End Get
    End Property

    Public ReadOnly Property CollaborationAttachmentEditable() As Boolean
        Get
            Return GetBoolean("CollAttachmentEditable")
        End Get
    End Property

    Public ReadOnly Property NomeDirigentiCollaborazione() As String
        Get
            Return GetString("NomeDirigentiCollaborazione", "Dirigenti")
        End Get
    End Property

    Public ReadOnly Property NomeViceCollaborazione() As String
        Get
            Return GetString("NomeViceCollaborazione", "Vice")
        End Get
    End Property

    Public ReadOnly Property NomeSegreteria() As String
        Get
            Return GetString("NomeSegreteria", "Segreteria")
        End Get
    End Property

    Public ReadOnly Property HideSegreteriaOnDirigSelect() As Boolean
        Get
            Return GetBoolean("HideSegreteriaOnDirigSelect")
        End Get
    End Property

    ''' <summary>Controlla la paginazione nelle firme multiple della collaborazione</summary>
    Public ReadOnly Property PagingOnMultipleFDQ() As Boolean
        Get
            Return GetBoolean("PagingOnMultipleFDQ", True)
        End Get
    End Property

    Public ReadOnly Property SearchMaxRecords() As Integer
        Get
            Return GetInteger("SearchMaxRecords")
        End Get
    End Property

    Public ReadOnly Property ForceViewer() As String
        Get
            Return GetString("ForceViewer", String.Empty)
        End Get
    End Property

    ''' <summary> Numero massimo di record da ritirare. </summary>
    Public ReadOnly Property DesktopDayDiff() As Integer
        Get
            Return GetInteger("DesktopDayDiff", 30)
        End Get
    End Property

    ''' <summary> Numero di giorni da considerare a partire dall'attuale nelle viste per la scrivania. </summary>
    Public ReadOnly Property DesktopMaxRecords() As Integer
        Get
            Return GetInteger("DesktopMaxRecords", 300)
        End Get
    End Property

    Public ReadOnly Property IsIssueEnabled() As Boolean
        Get
            Return GetBoolean("IssueEnabled")
        End Get
    End Property

    Public ReadOnly Property IsInteropEnabled() As Boolean
        Get
            Return GetBoolean("InteropEnabled")
        End Get
    End Property

    Public ReadOnly Property PECInteropContactSearch() As Integer
        Get
            Return GetInteger("PECInteropContactSearch", 0)
        End Get
    End Property

    Public ReadOnly Property InteropOggettoXPath() As String
        Get
            Return GetString("InteropOggettoXPath", "//Descrizione/Documento/Oggetto")
        End Get
    End Property


    Public ReadOnly Property IsLogEnabled() As Boolean
        Get
            Return GetBoolean("LogEnabled")
        End Get
    End Property

    Public ReadOnly Property IsComputerLogEnabled() As Boolean
        Get
            Return IsLogEnabled() AndAlso GetBoolean("ComputerLogEnabled")
        End Get
    End Property

    Public ReadOnly Property LogSecurity() As Integer
        Get
            Return GetInteger("LogSecurity")
        End Get
    End Property

    Public ReadOnly Property FascicleEnabled() As Boolean
        Get
            Return GetBoolean("FascicleEnabled", False)
        End Get
    End Property

    Public ReadOnly Property IsLogStatusEnabled() As Boolean
        Get
            Return GetBoolean("LogStatusEnabled", False)
        End Get
    End Property

    Public ReadOnly Property IsStatusEnabled As Boolean
        Get
            Return GetBoolean("StatusEnabled", False)
        End Get
    End Property

    Public ReadOnly Property IsProtSearchTitleEnabled() As Boolean
        Get
            Return GetBoolean("ProtSearchTitleEnabled")
        End Get
    End Property

    ''' <summary> Abilita la gestione del tipo documento </summary>
    Public ReadOnly Property IsTableDocTypeEnabled() As Boolean
        Get
            Return GetBoolean("TableDocTypeEnabled")
        End Get
    End Property

    Public ReadOnly Property IsTableLogEnabled() As Boolean
        Get
            Return GetBoolean("TableLogEnabled")
        End Get
    End Property

    Public ReadOnly Property IsSecurityEnabled() As Boolean
        Get
            Return GetBoolean("Security")
        End Get
    End Property

    Public ReadOnly Property IsSecurityGroupEnabled() As Boolean
        Get
            Return GetBoolean("SecurityGroupEnabled", False)
        End Get
    End Property

    Public ReadOnly Property IsPackageEnabled() As Boolean
        Get
            Return GetBoolean("PackageEnabled")
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
            Return GetInteger("ObjectMaxLength", 255)
        End Get
    End Property

    Public ReadOnly Property IsHelpEnabled() As Boolean
        Get
            Return GetBoolean("HelpEnabled")
        End Get
    End Property

    Public ReadOnly Property IsEnvSearchDefaultEnabled() As Boolean
        Get
            Return GetBoolean("CommSearchDefault")
        End Get
    End Property

    Public ReadOnly Property IsCollaborationEnabled() As Boolean
        Get
            Return GetBoolean("CollaborationEnabled")
        End Get
    End Property

    Public ReadOnly Property IsCollaborationGroupEnabled() As Boolean
        Get
            Return GetBoolean("CollaborationGroupEnabled")
        End Get
    End Property

    Public ReadOnly Property IsDiaryEnabled() As Boolean
        Get
            Return GetBoolean("DiaryEnabled")
        End Get
    End Property

    Public ReadOnly Property HasImportPregresso() As Boolean
        Get
            Return GetBoolean("ImportPregresso")
        End Get
    End Property

    Public ReadOnly Property IsUserCollOfflineEnabled() As Boolean
        Get
            Return GetBoolean("UserCollOfflineEnabled")
        End Get
    End Property

    Public ReadOnly Property WordPathImport() As String
        Get
            Return GetString("WordPathImport")
        End Get
    End Property

    Public ReadOnly Property ExcelPathImport() As String
        Get
            Return GetPipedString("ExcelEnabled", 2)
        End Get
    End Property

    ''' <summary>Cartella condivisa di rete usata per checkin/checkout</summary>
    Public ReadOnly Property VersioningShare() As String
        Get
            Return GetString("VersioningShare", String.Empty)
        End Get
    End Property

    Public ReadOnly Property VersioningShareDocSeriesImporter() As String
        Get
            Return GetString("VersioningShareDocSeriesImporter", String.Concat(VersioningShare, "\\", "DocSeriesImporter"))
        End Get
    End Property

    Public ReadOnly Property VersioningShareCheckOutEnabled As Boolean
        Get
            Return Not String.IsNullOrWhiteSpace(VersioningShare) AndAlso GetBoolean("VersioningShareCheckOutEnabled")
        End Get
    End Property

    ''' <summary> Percorso dal quale prendere i file per l'importazione. </summary>
    Public ReadOnly Property InvoicePathImport() As String
        Get
            Return GetString("InvoicePathImport", "C:\\Temp\\FastInput\\")
        End Get
    End Property

    Public ReadOnly Property AuthorizMailSmtpEnabled() As String
        Get
            Return GetString("AuthorizMailSmtpEnabled", String.Empty)
        End Get
    End Property

    Public ReadOnly Property AuthorizMailFrom() As String
        Get
            Return GetString("AuthorizMailFrom", String.Empty)
        End Get
    End Property

    Public ReadOnly Property MailSmtpServer() As String
        Get
            Return GetString("MailSmtpServer", String.Empty)
        End Get
    End Property

    Public ReadOnly Property UserErrorMailFrom() As String
        Get
            Return GetString("UserErrorMailFrom", String.Empty)
        End Get
    End Property

    Public ReadOnly Property UserErrorMailTo() As String
        Get
            Return GetString("UserErrorMailTo", String.Empty)
        End Get
    End Property

    ''' <summary> Posizione del BIT sui permessi di contenitore che permette di creare link tra i protocolli. </summary>
    Public ReadOnly Property LinkSecurity() As Integer
        Get
            Return GetInteger("LinkSecurity", 2)
        End Get
    End Property

    Public ReadOnly Property ScannerConfigurationEnabled() As Boolean
        Get
            Return IsComputerLogEnabled() AndAlso GetBoolean("ScannerConfigurationEnabled")
        End Get
    End Property

    ''' <summary> Import documenti da cartella di rete </summary>
    Public ReadOnly Property ImportSharedFolder() As String
        Get
            Dim sFolder As String = GetString("ImportSharedFolder")
            If sFolder = "" Then
                sFolder = "-"
            Else
                If sFolder.Chars(sFolder.Length - 1) <> "\" Then
                    sFolder += "\"
                End If
            End If
            Return sFolder
        End Get
    End Property

    Public ReadOnly Property WordGroupImport() As String
        Get
            Return GetString("WordGroupImport", String.Empty)
        End Get
    End Property

    Public ReadOnly Property ExcelGroupImport() As String
        Get
            Return GetPipedString("ExcelEnabled", 1)
        End Get
    End Property

    ''' <summary> Abilita l'importazione protocollo per i gruppi. </summary>
    Public ReadOnly Property InvoiceGroupImport() As String
        Get
            Return GetString("InvoiceGroupImport", String.Empty)
        End Get
    End Property

    Public ReadOnly Property EnvAuthorizFullEnabled() As Boolean
        Get
            Return GetBoolean("AuthorizFullEnabled")
        End Get
    End Property

    Public ReadOnly Property UserLogEnabled() As String
        Get
            Return GetString("UserLogEnabled", String.Empty)
        End Get
    End Property

    Public ReadOnly Property OfficeRedirect() As String
        Get
            Return GetString("OfficeRedirect", String.Empty)
        End Get
    End Property

    Public ReadOnly Property ThinClientxDefaultButton() As String
        Get
            Return GetString("ThinClientxDefaultButton", String.Empty)
        End Get
    End Property

    Public ReadOnly Property AuthorizContainer() As Integer
        Get
            Return GetInteger("AuthorizContainer")
        End Get
    End Property

    ''' <summary>
    ''' Quando <see>True</see>, per autorizzare un protocollo ad altri settori basta avere i diritti di visualizzazione agli altri settori.
    ''' </summary>
    Public ReadOnly Property AuthorizableIfRoleAutorized() As Boolean
        Get
            Return GetBoolean("AuthorizableIfRoleAutorized")
        End Get
    End Property

    ''' <summary> Contenitore a cui vengono associati i protocolli sospesi. </summary>
    Public ReadOnly Property SuspendContainer() As Integer
        Get
            Return GetInteger("SuspendContainer", -32768)
        End Get
    End Property

    ''' <summary>
    ''' Indica se nella maschera di ricerca e visualizzazione protocolli sospesi deve essere visibile l'area di filtraggio risultati.
    ''' </summary>
    Public ReadOnly Property SuspendFilterMaskEnabled As Boolean
        Get
            Return GetBoolean("SuspendFilterMaskEnabled")
        End Get
    End Property

    ''' <summary> Categoria che viene impostata ai protocolli sospesi. </summary>
    Public ReadOnly Property SuspendCategory() As Integer
        Get
            Return GetInteger("SuspendCategory", -32768)
        End Get
    End Property

    Public ReadOnly Property ThinClientxMultiChain() As Boolean
        Get
            Return GetBoolean("ThinClientxMultiChain")
        End Get
    End Property

    Public ReadOnly Property SmartClientMultiChain() As Boolean
        Get
            Return GetBoolean("SmartClientMultiChain")
        End Get
    End Property

    ''' <summary> Numero massimo di contatti nelle liste </summary>
    Public ReadOnly Property ContactMaxItems() As Integer
        Get
            Return GetInteger("ContactMaxItems", 50)
        End Get
    End Property

    Public ReadOnly Property EnvChangeObject() As ChangeObjectParameter
        Get
            Dim param As String = GetString("ProtChangeObject")
            If String.IsNullOrEmpty(param) Then
                Return Nothing
            End If
            Return New ChangeObjectParameter(param)
        End Get
    End Property

    Public ReadOnly Property ModificaOggettiDisable() As Boolean
        Get
            Return GetBoolean("ModificaOggettiDisable", True)
        End Get
    End Property

    Public ReadOnly Property IsChangeObjectEnable() As Boolean
        Get
            Return GetBoolean("ObjectChangeEnabled")
        End Get
    End Property

    ''' <summary> Per accedere all'indice della pubblica amministrazione (ldap internet) </summary>
    Public ReadOnly Property LdapIndicePa() As String
        Get
            Return GetString("LDAPIndicePA", String.Empty)
        End Get
    End Property

    Public ReadOnly Property IsConservationEnabled() As Boolean
        Get
            Return GetBoolean("ConservationEnabled")
        End Get
    End Property

    Public ReadOnly Property ConservationMaxItems() As Integer
        Get
            Return GetInteger("ConservationMaxItems")
        End Get
    End Property

    Public ReadOnly Property EnvConservationDateFormat() As String
        Get
            Return GetString("ConservationDateFormat", "{0:dd/MM/yyyy}")
        End Get
    End Property

    Public ReadOnly Property ConservationExtensions() As String()
        Get
            Dim tmp As String = GetString("ConservationExtensions")
            If _conservationExtensions Is Nothing AndAlso Not String.IsNullOrEmpty(tmp) Then
                _conservationExtensions = tmp.Split("|".ToCharArray())
            Else
                _conservationExtensions = New String() {FileHelper.P7M, FileHelper.M7M}
            End If
            Return _conservationExtensions
        End Get
    End Property

    Public ReadOnly Property ConservationMaxResults As Integer
        Get
            Return GetInteger("ConservationMaxResults")
        End Get
    End Property

    Public ReadOnly Property ConservationFieldsMaxLength() As Integer
        Get
            Return GetInteger("ConservationFieldsMaxLength", 511)
        End Get
    End Property

    Public ReadOnly Property FastInputImportPath() As String
        Get
            Return GetString("FastInputImportPath", String.Empty)
        End Get
    End Property

    Public ReadOnly Property IsImportContactEnabled() As Boolean
        Get
            Return GetBoolean("ImportContact")
        End Get
    End Property

    Public ReadOnly Property IsImportContactManualEnabled() As Boolean
        Get
            Return GetBoolean("ImportContactManual")
        End Get
    End Property

    Public ReadOnly Property DiaryFullEnabled() As Boolean
        Get
            Return GetBoolean("DiaryFullEnabled", True)
        End Get
    End Property

    Public ReadOnly Property UploadZipManaged() As Boolean
        Get
            Return GetBoolean("UploadZipManaged")
        End Get
    End Property

    Public ReadOnly Property ContactModify() As ContactModifyParameter
        Get
            Return New ContactModifyParameter(GetString("ContactModify"))
        End Get
    End Property

    Public ReadOnly Property ExportPath() As String
        Get
            Return GetString("ExportPath", String.Empty)
        End Get
    End Property

    Public ReadOnly Property CategoryView() As Integer
        Get
            Return GetInteger("CategoryView")
        End Get
    End Property

    Public ReadOnly Property MailDisclaimer() As String
        Get
            Return GetString("MailDisclaimer", String.Empty)
        End Get
    End Property

    Public ReadOnly Property SmartClientMailBody() As String
        Get
            Return GetString("SmartClientMailBody", String.Empty)
        End Get
    End Property

    Public ReadOnly Property PdfPrint() As Boolean
        Get
            Return GetBoolean("PdfPrint")
        End Get
    End Property

    Public ReadOnly Property ProtocolImportClass() As String
        Get
            Return GetString("ProtocolImportClass", String.Empty)
        End Get
    End Property

    Public ReadOnly Property ProtocolImportLimit() As Integer
        Get
            Return GetInteger("ProtocolImportLimit")
        End Get
    End Property

    Public ReadOnly Property RubricaCheckDuplicati() As Boolean
        Get
            Return GetBoolean("RubricaCheckDuplicati")
        End Get
    End Property

    Public ReadOnly Property EditableAttachment() As Boolean
        Get
            Return GetBoolean("EditableAttachment")
        End Get
    End Property

    Public ReadOnly Property EnvGroupModifyAttachmentProt() As String
        Get
            Return GetString("EnvGroupModifyAttachmentProt", String.Empty)
        End Get
    End Property


    Public ReadOnly Property WSTopMediaReadCount() As Integer
        Get
            Return GetInteger("WSTopMediaReadCount", 0)
        End Get
    End Property

    Public ReadOnly Property WSTopMediaContainers As String
        Get
            Return GetString("WSTopMediaContainers", String.Empty)
        End Get
    End Property

    Public ReadOnly Property InvoiceProtocolContainers As String
        Get
            Return GetString("InvoiceProtocolContainers", String.Empty)
        End Get
    End Property

    Public ReadOnly Property WSTopMediaParams As String
        Get
            Return GetString("WSTopMediaParams", String.Empty)
        End Get
    End Property

    Public ReadOnly Property WSTopMediaRoles As String
        Get
            Return GetString("WSTopMediaRoles", String.Empty)
        End Get
    End Property

    Public ReadOnly Property TopMediaParameters As TopMediaParameters
        Get
            Return New TopMediaParameters()
        End Get
    End Property

    Public ReadOnly Property IsRaccomandataEnabled() As Boolean
        Get
            Return GetBoolean("RaccomandataEnabled")
        End Get
    End Property

    Public ReadOnly Property IsLetteraEnabled() As Boolean
        Get
            Return GetBoolean("LetteraEnabled")
        End Get
    End Property

    Public ReadOnly Property TNoticeEnabled() As Boolean
        Get
            Return GetBoolean("TNoticeEnabled", False)
        End Get
    End Property

    Public ReadOnly Property IsTelgrammaEnabled() As Boolean
        Get
            Return GetBoolean("TelegrammaEnabled")
        End Get
    End Property

    Public ReadOnly Property SelRoleUserStartCollapsed() As Boolean
        Get
            Return GetBoolean("SelRoleUserStartCollapsed")
        End Get
    End Property

    Public ReadOnly Property RedirectToRootSite() As Boolean
        Get
            Return GetBoolean("RedirectToRootSite")
        End Get
    End Property

    ''' <summary> Specifica se deve essere DocumentSource.aspx a chiamare il servizio di Stampa Conforme. </summary>
    Public ReadOnly Property DocumentSourceConversion() As Boolean
        Get
            Return GetBoolean("DocumentSourceConversion")
        End Get
    End Property

    Public ReadOnly Property DocumentSourceScriptTimeout() As Integer
        Get
            Return GetInteger("DocumentSourceScriptTimeout")
        End Get
    End Property

    Public ReadOnly Property IsRecoverEnabled() As Boolean
        Get
            Return GetBoolean("RecoverEnabled")
        End Get
    End Property

    Public ReadOnly Property IsRedoEnabled() As Boolean
        Get
            Return GetBoolean("RedoEnabled")
        End Get
    End Property

    Public ReadOnly Property IsRepairEnabled() As Boolean
        Get
            Return GetBoolean("RepairEnabled")
        End Get
    End Property

    ''' <summary> Attiva la Rubrica di Settore </summary>
    Public ReadOnly Property RoleContactEnabled() As Boolean
        Get
            Return GetBoolean("RoleContactEnabled")
        End Get
    End Property

    Public ReadOnly Property CheckRecoverToProtocol() As Boolean
        Get
            Return GetBoolean("CheckRecoverToProtocol", False)
        End Get
    End Property

    Public ReadOnly Property ProtocolRecoverHandleEnabled() As Boolean
        Get
            Return GetBoolean("ProtocolRecoverHandleEnabled", True)
        End Get
    End Property

    Public ReadOnly Property VisioneFirmaChecked() As Boolean
        Get
            Return GetBoolean("VisioneFirmaChecked")
        End Get
    End Property

    ''' <summary> Gruppo di utenti che sono abilitati a modifcare la Category di un protocollo. </summary>
    Public ReadOnly Property GroupEditCategory As String
        Get
            Return GetString("GroupEditCategory", String.Empty)
        End Get
    End Property

    ''' <summary> Regex di validazione stringhe. </summary>
    Public ReadOnly Property RegexOnPasteString As String
        Get
            Return GetString("RegexOnPasteString", String.Empty)
        End Get
    End Property

    ''' <summary> Indica se la pagina finale della collaborazione deve essere aperta nell'iFrame principale. </summary>
    Public ReadOnly Property CollaborationOpenOnMain As Boolean
        Get
            Return GetBoolean("CollaborationOpenOnMain")
        End Get
    End Property

    Public ReadOnly Property PECLogHideIpAddress As Boolean
        Get
            Return GetBoolean("PECLogHideIpAddress")
        End Get
    End Property

    Public ReadOnly Property PECLogShowMoveDefaultValue As Boolean
        Get
            Return GetBoolean("PECLogShowMoveDefaultValue")
        End Get
    End Property

    Public ReadOnly Property PECMoveNotificationEnabled As Boolean
        Get
            Return GetBoolean("PECMoveNotificationEnabled")
        End Get
    End Property

    ''' <summary> Abilita il trasferimento protocollo. </summary>
    Public ReadOnly Property ProtocolTransfertEnabled As Boolean
        Get
            Return GetBoolean("ProtocolTransfertEnabled")
        End Get
    End Property

    ''' <summary> Indica se 蠡ttiva la ricerca "light" per NHibernateProtocolFinder. </summary>
    Public ReadOnly Property CreateCriteriaLight() As Boolean
        Get
            Return GetBoolean("CreateCriteriaLight")
        End Get
    End Property

    ''' <summary> Valore di default per il campo AdvancedViewer dei nuovi record di ComputerLog. </summary>
    Public ReadOnly Property DefaultAdvancedViewer() As Integer
        Get
            Return GetInteger("DefaultAdvancedViewer")
        End Get
    End Property

    ''' <summary> Valore di default per il campo AdvancedViewer dei nuovi record di ComputerLog. </summary>
    Public ReadOnly Property DefaultAdvancedScanner() As Integer
        Get
            Return GetInteger("DefaultAdvancedScanner")
        End Get
    End Property

    ''' <summary> Firma con funzioni vicariali - parametro di default per FVicario di VecVBSignerManager.dll. </summary>
    Public ReadOnly Property DefaultFVicario As String
        Get
            Return GetString("DefaultFVicario", String.Empty)
        End Get
    End Property

    ''' <summary> Abilita la gestione della rubrica Dominio AD. </summary>
    Public ReadOnly Property AbilitazioneRubricaDomain As Boolean
        Get
            Return GetBoolean("AbilitazioneRubricaDomain")
        End Get
    End Property

    Public ReadOnly Property AbilitazioneRubricaOChart As Boolean
        Get
            Return APIEnabled AndAlso GetBoolean("AbilitazioneRubricaOChart")
        End Get
    End Property

    Public ReadOnly Property RubricaDomainExistingOnly As Boolean
        Get
            Return IsSecurityGroupEnabled AndAlso AbilitazioneRubricaDomain AndAlso GetBoolean("RubricaDomainExistingOnly")
        End Get
    End Property

    Public ReadOnly Property IsPECDestinationOptional() As Boolean
        Get
            Return GetBoolean("PECDestinationOptional")
        End Get
    End Property

    ''' <summary> Indica se se proporre o meno la data documento da protocollazione da pec. </summary>
    Public ReadOnly Property EnableDocumentDateFromPEC() As Boolean
        Get
            Return GetBoolean("EnableDocumentDateFromPEC")
        End Get
    End Property


    ''' <summary> Casella di default per lo sposta. </summary>
    Public ReadOnly Property PECUnmanagedDefaultMove As Integer
        Get
            Return GetInteger("PECUnmanagedDefaultMove")
        End Get
    End Property

    ''' <summary> ContattiDefaultContiene: Valore di default del flag Contiene nella selezione di mittenti/destinatari. </summary>
    Public ReadOnly Property ContattiDefaultContiene As Boolean
        Get
            Return GetBoolean("ContattiDefaultContiene")
        End Get
    End Property

    ''' <summary> ConsentiDuplicaProtDaPEC: Se = 1 il bottone DUPLICA viene abilitato anche per i protocolli derivati da PEC. </summary>
    Public ReadOnly Property ConsentiDuplicaProtDaPEC As Boolean
        Get
            Return GetBoolean("ConsentiDuplicaProtDaPEC")
        End Get
    End Property

    ''' <summary> GruppoPECEmptyRecycleBin: restituisce il gruppo degli utenti con diritto di svuotamento del cestino PEC. </summary>
    ''' <returns>string: nome del gruppo abilitato allo svuotamento cestino PEC</returns>
    Public ReadOnly Property GruppoPECEmptyRecycleBin As String
        Get
            Return GetString("GruppoPECEmptyRecycleBin", String.Empty)
        End Get
    End Property

    ''' <summary> Abilita la ricerca custom per mittente/destinatario. </summary>
    Public ReadOnly Property SearchRecipientCustom As Boolean
        Get
            Return GetBoolean("SearchRecipientCustom")
        End Get
    End Property

    ''' <summary> Abilita la ricerca per mittente/destinatario in tutti i contatti figli del contatto effettivamente ricercato. </summary>
    Public ReadOnly Property SearchProtocolRecipientChildren As Boolean
        Get
            Return GetBoolean("SearchProtocolRecipientChildren", False)
        End Get
    End Property

    Public ReadOnly Property EditLoadCurrentContainer As Boolean
        Get
            Return GetBoolean("EditLoadCurrentContainer")
        End Get
    End Property

    Public ReadOnly Property AutocompleteContainer As Boolean
        Get
            Return GetBoolean("AutocompleteContainer")
        End Get
    End Property

    Public ReadOnly Property SecurityProtocolObject As String
        Get
            Return GetString("SecurityProtocolObject", "Protocollo riservato con sicurezza aziendale applicata")
        End Get
    End Property

    Public ReadOnly Property CollaborationNoMultipleSign() As Boolean
        Get
            Return GetBoolean("CollaborationNoMultipleSign")
        End Get
    End Property

    Public ReadOnly Property SuperAdmin() As String
        Get
            Return GetString("SuperAdmin", String.Empty)
        End Get
    End Property

    Public ReadOnly Property HideManualRecipient() As Boolean
        Get
            Return GetBoolean("HideManualRecipient")
        End Get
    End Property

    ''' <summary> Indica se il menù iniziale debba rimanere chiuso all'avvio di DSW. </summary>
    Public ReadOnly Property MainMenuCollapsed() As Boolean
        Get
            Return GetBoolean("MainMenuCollapsed")
        End Get
    End Property

    ''' <summary> Indica se i settori disabilitati continuano a dare diritti agli operatori. </summary>
    Public ReadOnly Property DisabledRolesRights() As Boolean
        Get
            Return GetBoolean("DisabledRolesRights")
        End Get
    End Property

    ''' <summary> Indica se il PARER 蠡ttivo per il DB di Protocollo. </summary>
    Public ReadOnly Property ParerEnabled() As Boolean
        Get
            Return GetBoolean("ParerEnabled")
        End Get
    End Property

    Public ReadOnly Property PECHandlerTimeout() As Integer
        Get
            Return GetInteger("PECHandlerTimeout")
        End Get
    End Property

    Public ReadOnly Property PECWindowWidth() As Integer
        Get
            Return GetInteger("PECWindowWidth", 800)
        End Get
    End Property

    Public ReadOnly Property PECWindowHeight() As Integer
        Get
            Return GetInteger("PECWindowHeight", 600)
        End Get
    End Property

    Public ReadOnly Property ModalWidth() As Integer
        Get
            Return GetInteger("ModalWidth", 700)
        End Get
    End Property


    Public ReadOnly Property ModalHeight() As Integer
        Get
            Return GetInteger("ModalHeight", 500)
        End Get
    End Property

    Public ReadOnly Property PECWindowBehaviors() As String
        Get
            Return GetString("PECWindowBehaviors", "Telerik.Web.UI.WindowBehaviors.Close")
        End Get
    End Property

    Public ReadOnly Property PECWindowPosition() As String
        Get
            Return GetString("PECWindowPosition", "wnd.center();")
        End Get
    End Property

    Public ReadOnly Property PECSimpleMode() As Boolean
        Get
            Return GetBoolean("PECSimpleMode")
        End Get
    End Property

    Public ReadOnly Property PECSimpleModeHideButtons() As String
        Get
            Return GetString("PECSimpleModeHideButtons", String.Empty)
        End Get
    End Property

    Public ReadOnly Property PECSimpleModeWidth() As Integer
        Get
            Return GetInteger("PECSimpleModeWidth", 675)
        End Get
    End Property

    Public ReadOnly Property PECSimpleModeHeight() As Integer
        Get
            Return GetInteger("PECSimpleModeHeight", 200)
        End Get
    End Property

    Public ReadOnly Property MultiDomainEnabled() As Boolean
        Get
            Return GetBoolean("MultiDomainEnabled")
        End Get
    End Property

    Public ReadOnly Property PECHandlerEnabled() As Boolean
        Get
            Return GetBoolean("PECHandlerEnabled", False)
        End Get
    End Property

    ''' <summary> Template dell'oggetto della mail di notifica di presa in carico PEC. </summary>
    Public ReadOnly Property PECHandlerNotificationTemplate() As String
        Get
            Return GetString("PECHandlerNotificationTemplate", "L'utente %USER% (%USEREMAIL%) ha preso in carico la PEC %ID%  del %DATE% con oggetto: %SUBJECT%")
        End Get
    End Property



    ''' <summary> Abilita la notifica via mail della presa in carico di una PEC al precedente gestore. </summary>
    Public ReadOnly Property PECHandlerNotificationEnabled As Boolean
        Get
            Return GetBoolean("PECHandlerNotificationEnabled")
        End Get
    End Property

    Public ReadOnly Property ProtHandlerEnabled() As Boolean
        Get
            Return GetBoolean("ProtHandlerEnabled")
        End Get
    End Property

    ''' <summary> Template dell'oggetto della mail di notifica di presa in carico Protocollo. </summary>
    Public ReadOnly Property ProtHandlerNotificationTemplate() As String
        Get
            Return GetString("ProtHandlerNotificationTemplate", "L'utente %USER% (%USEREMAIL%) ha preso in carico il Protocollo con %ID% del %DATE% con oggetto: %SUBJECT%")
        End Get
    End Property

    ''' <summary> Abilita la notifica via mail della presa in carico di un Protocollo al precedente gestore. </summary>
    Public ReadOnly Property ProtHandlerNotificationEnabled As Boolean
        Get
            Return GetBoolean("ProtHandlerNotificationEnabled")
        End Get
    End Property

    ''' <summary> Tipo di invio mail presa in carico. </summary>
    Public ReadOnly Property NotificationHandlerType() As NotificationHandlerType
        Get
            Dim notificationType As NotificationHandlerType = NotificationHandlerType.Smtp
            Dim id As Integer = GetInteger("NotificationHandlerType", 0)
            If [Enum].IsDefined(GetType(NotificationHandlerType), id) Then
                notificationType = DirectCast(id, NotificationHandlerType)
            End If
            Return notificationType
        End Get
    End Property

    Private _invoiceProtocolContainerIdentifiers As IList(Of Integer)
    Public ReadOnly Property InvoiceProtocolContainerIdentifiers As IList(Of Integer)
        Get
            If (_invoiceProtocolContainerIdentifiers Is Nothing) Then
                Dim splitted As String() = DocSuiteContext.Current.ProtocolEnv.InvoiceProtocolContainers.Split("|"c)
                Dim parsed As Integer = 0
                Dim valid As IEnumerable(Of String) = splitted.Where(Function(c) Integer.TryParse(c, parsed))
                _invoiceProtocolContainerIdentifiers = valid.Select(Function(c) Integer.Parse(c)).ToList()
            End If
            Return _invoiceProtocolContainerIdentifiers
        End Get
    End Property

    Public ReadOnly Property PECUnhandleOnMove() As Boolean
        Get
            Return GetBoolean("PECUnhandleOnMove")
        End Get
    End Property

    Public ReadOnly Property PECUnhandleOnMoveBehaviour() As Integer
        Get
            Return GetInteger("PECUnhandleOnMoveBehaviour")
        End Get
    End Property

    Public ReadOnly Property PECMoveEnabled() As Boolean
        Get
            Return GetBoolean("PECMoveEnabled")
        End Get
    End Property

    Public ReadOnly Property PECNewMailEnabled() As Boolean
        Get
            Return GetBoolean("PECNewMailEnabled")
        End Get
    End Property

    Public ReadOnly Property PECReplyEnabled() As Boolean
        Get
            Return GetBoolean("PECReplyEnabled")
        End Get
    End Property

    Public ReadOnly Property PECOutShowRecordedFilter() As Boolean
        Get
            Return GetBoolean("PECOutShowRecordedFilter")
        End Get
    End Property

    Public ReadOnly Property PECOutShowRecordedDefault() As Boolean
        Get
            Return GetBoolean("PECOutShowRecordedDefault")
        End Get
    End Property

    Public ReadOnly Property ShowMailReceiptFilters() As Boolean
        Get
            Return GetBoolean("ShowMailReceiptFilters", True)
        End Get
    End Property

    Public ReadOnly Property ShowRecordedDates() As Boolean
        Get
            Return GetBoolean("ShowRecordedDates")
        End Get
    End Property

    Public ReadOnly Property PECExternalTrashbin() As Boolean
        Get
            Return GetBoolean("PECExternalTrashbin", False)
        End Get
    End Property

    Public ReadOnly Property PECForwardSuccessfulEnabled() As Boolean
        Get
            Return GetBoolean("PECForwardSuccessfulEnabled")
        End Get
    End Property

    ''' <summary> Indica se spostare il passaggio di parametri dalla querystring alla session. </summary>
    ''' <value>Default false</value>
    ''' <remarks>Nata per la MailFacade.</remarks>
    Public ReadOnly Property QuerystringToSession() As Boolean
        Get
            Return GetBoolean("QuerystringToSession", True)
        End Get
    End Property

    Public ReadOnly Property UserConsoleEnabled() As Boolean
        Get
            Return GetBoolean("UserConsoleEnabled")
        End Get
    End Property

    Public ReadOnly Property UserLogEmail() As Boolean
        Get
            Return GetBoolean("UserLogEmail")
        End Get
    End Property

    Public ReadOnly Property VisualizzaLibricinoIngoing() As Boolean
        Get
            Return GetBoolean("VisualizzaLibricinoIngoing", True)
        End Get
    End Property

    Public ReadOnly Property VisualizzaLibricinoOutgoing() As Boolean
        Get
            Return GetBoolean("VisualizzaLibricinoOutgoing", True)
        End Get
    End Property

    Public ReadOnly Property PropageCollaborationEmailAdd() As Boolean
        Get
            Return GetBoolean("PropageCollaborationEmailAdd")
        End Get
    End Property

    Public ReadOnly Property PECYearCanStartReport() As Integer
        Get
            Return GetInteger("PECYearCanStartReport", 2010)
        End Get
    End Property

    Public ReadOnly Property ScannerBufferMaximum() As Integer
        Get
            ''Se il valore rilevato 蠴ra 1 e 1024 viene impostato
            ''Se non 蠲ilevato vale 100
            ''Se 蠴roppo grande vale 1024 che 蠩l massimo
            Dim val As Integer = GetInteger("ScannerBufferMaximum", 100)
            Return Math.Min(val, 1024)
        End Get
    End Property

    Public ReadOnly Property PECConfirmAllegaAProtocollo() As Boolean
        Get
            Return GetBoolean("PECConfirmAllegaAProtocollo")
        End Get
    End Property

    Public ReadOnly Property PECEnableDeleteIfNotRecorded() As Boolean
        Get
            Return GetBoolean("PECEnableDeleteIfNotRecorded")
        End Get
    End Property

    Public ReadOnly Property ProtNewPecMailEnabled() As Boolean
        Get
            Return GetBoolean("ProtNewPecMailEnabled")
        End Get
    End Property

    Public ReadOnly Property AllowZeroAuthorizations() As Boolean
        Get
            Return GetBoolean("AllowZeroAuthorizations")
        End Get
    End Property

    Public ReadOnly Property EnableMultiChainView() As Boolean
        Get
            Return GetBoolean("EnableMultiChainView")
        End Get
    End Property

    Public ReadOnly Property CheckSession() As Boolean
        Get
            Return GetBoolean("CheckSession", True)
        End Get
    End Property

    Public ReadOnly Property ObserversAssemblyName() As String
        Get
            Return GetString("ObserversAssemblyName", String.Empty)
        End Get
    End Property

    Public ReadOnly Property ProtocolFacadeObserverName() As String
        Get
            Return GetString("ProtocolFacadeObserverName", "VecompSoftware.DocSuiteWeb.Facade.Observers.ProtocolFacadeObserver")
        End Get
    End Property

    Public ReadOnly Property MessageLocation() As Integer
        Get
            Return GetInteger("MessageLocation", -1)
        End Get
    End Property

    Public ReadOnly Property PecIntDefaultContactSenderId() As Integer
        Get
            Return GetInteger("PecIntDefaultContactSenderId", -1)
        End Get
    End Property

    Public ReadOnly Property PecHandlerMessageEnabled As Boolean
        Get
            Return GetBoolean("PecHandlerMessageEnabled", False)
        End Get
    End Property

    ''' <summary> Abilita il bottone che permette di reinviare le PEC con errore </summary>
    Public ReadOnly Property ResendAndPecClone As Boolean
        Get
            Return GetBoolean("ResendAndPecClone", False)
        End Get
    End Property


    ''' <summary> Abilita il bottone della duplicazione della PEC nella posta in arrivo </summary>
    Public ReadOnly Property PECClone As Boolean
        Get
            Return GetBoolean("PecClone", False)
        End Get
    End Property

    Public ReadOnly Property MassivePECCloneEnabled As Boolean
        Get
            Return GetBoolean("MassivePECCloneEnabled", False)
        End Get
    End Property


    ''' <summary> Abilita il bottone di spostamento delle PEC nella posta in arrivo </summary>
    Public ReadOnly Property MassivePecMove As Boolean
        Get
            Return GetBoolean("MassivePecMove", False)
        End Get
    End Property

    Public ReadOnly Property DocumentPreviewHeight() As Integer
        Get
            Return GetInteger("DocumentPreviewHeight", 500)
        End Get
    End Property

    Public ReadOnly Property DocumentPreviewWidth() As Integer
        Get
            Return GetInteger("DocumentPreviewWidth", 700)
        End Get
    End Property

    Public ReadOnly Property AllowPreviousYear() As Boolean
        Get
            Return GetBoolean("AllowPreviousYear")
        End Get
    End Property

    Public ReadOnly Property FileExtensionBlackList() As String
        Get
            Return GetString("FileExtensionBlackList", String.Empty)
        End Get
    End Property

    Public ReadOnly Property FileExtensionWhiteList() As String
        Get
            Return GetString("FileExtensionWhiteList", String.Empty)
        End Get
    End Property

    Public ReadOnly Property EnableGrayList() As Boolean
        Get
            Return GetBoolean("EnableGrayList")
        End Get
    End Property

    Public ReadOnly Property UploadSilverlightEnabled() As Boolean
        Get
            Return GetBoolean("UploadSilverlightEnabled", True)
        End Get
    End Property

    Public ReadOnly Property JournalEnabledGroups() As String
        Get
            Return GetString("JournalEnabledGroups", String.Empty)
        End Get
    End Property

    Public ReadOnly Property PECFixedGroups() As String
        Get
            Return GetString("PECFixedGroups", String.Empty)
        End Get
    End Property

    Public ReadOnly Property PecMailLogViewVisibleGroups() As String
        Get
            Return GetString("PecMailLogViewVisibleGroups", String.Empty)
        End Get
    End Property

    Public ReadOnly Property PecProtocolDocTypeDefault() As String
        Get
            Return GetString("PecProtocolDocTypeDefault", String.Empty)
        End Get
    End Property

    Public ReadOnly Property ProtocolBoxProtocolDocTypeDefault() As String
        Get
            Return GetString("ProtocolBoxProtocolDocTypeDefault", String.Empty)
        End Get
    End Property

    Public ReadOnly Property StampaConformeExtensions() As String
        Get
            Return GetString("StampaConformeExtensions", ".pdf|.doc|.docx|.docm|.odt|.ott|.xls|.xlsx|.xlsm|.ods|.ots|.p7m|.tsd|.p7x|.m7m|.txt|.jpeg|.tif|.tiff|.bmp|.odg")
        End Get
    End Property

    Public ReadOnly Property DefaultProtocolSortExpression() As Dictionary(Of String, String)
        Get
            Dim temp As String = GetString("DefaultProtocolSortExpression", "Year ASC|Number ASC")
            Dim t() As String = temp.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
            Return t.ToDictionary(Function(s) s.Split(" "c)(0), Function(s) s.Split(" "c)(1))
        End Get
    End Property

    Public ReadOnly Property PECFromFile() As Boolean
        Get
            Return GetBoolean("PECFromFile")
        End Get
    End Property

    Public ReadOnly Property TimeIntervalForCleaningTemp() As Integer
        Get
            Return GetInteger("TimeIntervalForCleaningTemp", 0)
        End Get
    End Property

    ''' <summary> Indica se 蠯bbligatorio indicare il messaggio di motivazione di spostamento della pec. </summary>
    Public ReadOnly Property PecRequiredMoveMessage() As Boolean
        Get
            Return GetBoolean("PecRequiredMoveMessage")
        End Get
    End Property

    ''' <summary> Indica se 蠯bbligatorio indicare il messaggio di motivazione di cancellazione della pec. </summary>
    Public ReadOnly Property PecRequiredDeleteMessage() As Boolean
        Get
            Return GetBoolean("PecRequiredDeleteMessage")
        End Get
    End Property

    ''' <summary> Definisce se attivare la funzionalitࠤi Casella di protocollazione. </summary>
    Public ReadOnly Property ProtocolBoxEnabled As Boolean
        Get
            Return GetBoolean("ProtocolBoxEnabled", False)
        End Get
    End Property

    Public ReadOnly Property PecUnboundMode() As Integer
        Get
            Return GetInteger("PecUnboundMode", -1)
        End Get
    End Property

    Public ReadOnly Property DocumentSeriesEnabled() As Boolean
        Get
            Return GetBoolean("DocumentSeriesEnabled")
        End Get
    End Property

    Public ReadOnly Property DocumentSeriesLogSummaryEnabled As Boolean
        Get
            Return DocumentSeriesEnabled AndAlso GetBoolean("DocumentSeriesLogSummaryEnabled")
        End Get
    End Property


    Public ReadOnly Property SeriesRetireAuto() As Integer
        Get
            Return GetInteger("SeriesRetireAuto", 0)
        End Get
    End Property

    ''' <summary> Se attivato sposta il menu collaborazione dalla scrivania al menu principale. </summary>
    Public ReadOnly Property MoveCollaborationMenu() As Boolean
        Get
            Return GetBoolean("MoveCollaborationMenu")
        End Get
    End Property

    Public ReadOnly Property CopyFromSeries() As Boolean
        Get
            Return GetBoolean("CopyFromSeries")
        End Get
    End Property

    ''' <summary> Indica se le notifiche sono abilitate. </summary>
    Public ReadOnly Property NotificationEnabled() As Boolean
        Get
            Return NotificationTypes.Count > 0
        End Get
    End Property

    ''' <summary> Numero in millisecondi del tempo di refresh delle notifiche nella scrivania. </summary>
    Public ReadOnly Property NotificationTimer() As Integer
        Get
            Return GetInteger("NotificationTimer", 0)
        End Get
    End Property

    ''' <summary> Indica quali notifiche sono abilitate. </summary>
    Public ReadOnly Property NotificationTypes() As IList(Of NotificationType)
        Get
            ' Elimino duplicati
            Dim stringArray As New HashSet(Of String)(GetString("NotificationTypes", String.Empty).Split(","c))

            Dim list As New List(Of NotificationType)
            For Each stringId As String In stringArray
                Dim value As NotificationType
                If [Enum].TryParse(Of NotificationType)(stringId, value) AndAlso [Enum].IsDefined(GetType(NotificationType), value) Then
                    list.Add(value)
                End If
            Next
            Return list
        End Get
    End Property

    Public ReadOnly Property PecMailViewDefault As Integer
        Get
            Return GetInteger("PecMailViewDefault", -1)
        End Get
    End Property

    ''' <summary> Nome dell'applicazione. </summary>
    Public ReadOnly Property ApplicationName() As String
        Get
            Return GetString("ApplicationName", "DocSuite - Gestione Documentale")
        End Get
    End Property

    ''' <summary> Indica se considerare tutti i contatti inseriti nel controllo, o solo i figli. </summary>
    ''' <remarks> True: Calcola e salva tutti i nodi. False: Considera solo i nodi foglia. </remarks>
    Public ReadOnly Property ContactCountGetAllSelected As Boolean
        Get
            Return GetBoolean("ContactCountGetAllSelected", True)
        End Get
    End Property


    Public ReadOnly Property BehaviourRegex As String
        Get
            Return GetString("BehaviourRegex", "(?<target>\<(?<fieldtype>\@|\#|\%|§|\$)(?<field>[a-zA-Z]+[^:>\[\]])(\[(?<value>(-|\+)?\d*)\])?:?(?<format>[^.-[>]]*)\>)")
        End Get
    End Property

    Public ReadOnly Property DocumentSeriesPageSize() As Integer
        Get
            Return GetInteger("DocumentSeriesPageSize", 10)
        End Get
    End Property

    Public ReadOnly Property DuplicateDefaults() As IList(Of String)
        Get
            Dim temp As String = GetString("DuplicateDefaults", "OwnerRoles, Subject, DynamicData, Category, Documents")
            Return Array.ConvertAll(temp.Split(","c), Function(p) p.Trim())
        End Get
    End Property


    Public ReadOnly Property DuplicateDefaultsInSession() As Boolean
        Get
            Return GetBoolean("DuplicateDefaultsInSession", False)
        End Get
    End Property


    Public ReadOnly Property OwnerRolesInSession() As Boolean
        Get
            Return GetBoolean("OwnerRolesInSession", False)
        End Get
    End Property

    ''' <summary> Se attivato sposta il menu protocollo/pratiche/atti dalla scrivania al menu principale. </summary>
    Public ReadOnly Property MoveScrivaniaMenu() As Boolean
        Get
            Return GetBoolean("MoveScrivaniaMenu")
        End Get
    End Property

    ''' <summary> Abilita il rigetto di protocollo. </summary>
    ''' <remarks> Funzione nata per ENAV. </remarks>
    Public ReadOnly Property ProtocolRejectionEnabled() As Boolean
        Get
            Return GetBoolean("ProtocolRejection", False)
        End Get
    End Property

    ''' <summary> Contenitore per i protocolli rigettati </summary>
    ''' <remarks> Funzione nata per ENAV </remarks>
    Public ReadOnly Property ProtocolRejectionContainerId As Integer
        Get
            Return GetInteger("ProtocolRejectionContainer")
        End Get
    End Property

    Public ReadOnly Property PECAddressDestinationCheck() As Boolean
        Get
            Return GetBoolean("PECAddressDestinationCheck", False)
        End Get
    End Property

    Public Enum RoleOwnerDefaultModeEnum
        None = 0
        All = 1
        OnlyIfSingle = 2
    End Enum

    Public ReadOnly Property RoleOwnerDefaultMode() As RoleOwnerDefaultModeEnum
        Get
            Return CType(GetInteger("RoleOwnerDefaultMode", 2), RoleOwnerDefaultModeEnum)
        End Get
    End Property

    Public ReadOnly Property EnableValidatorSummary() As Boolean
        Get
            Return GetBoolean("EnableValidatorSummary", False)
        End Get
    End Property

    ''' <summary> Identificativo del nodo che identifica la rubrica interna </summary>
    Public ReadOnly Property InnerContactRoot() As Integer?
        Get
            Return GetNullableInt("InnerContactRoot")
        End Get
    End Property

    Public ReadOnly Property ExcludeInnerContact() As Boolean
        Get
            Return InnerContactRoot.HasValue AndAlso GetBoolean("ExcludeInnerContact")
        End Get
    End Property


    ''' <summary> Indica il vincolo su classificatore. </summary>
    ''' <value>0: nessuno, 1: uguale, 2: gerarchico</value>
    Public ReadOnly Property FascicleProtocolConstraint() As Integer
        Get
            Return GetInteger("FascicleProtocolConstraint", 1)
        End Get
    End Property

    Public ReadOnly Property ReportLibraryEnabled() As Boolean
        Get
            Return GetBoolean("ReportLibraryEnabled", False)
        End Get
    End Property

    Public ReadOnly Property ReportLibraryPath() As String
        Get
            Return GetString("ReportLibraryPath", "~/Report/Protocol/")
        End Get
    End Property

    Public ReadOnly Property ReportRowCountLimit() As Integer
        Get
            Return GetInteger("ReportRowCountLimit", 1000)
        End Get
    End Property

    ''' <summary> Abilita la firma singola in PAdES. </summary>
    Public ReadOnly Property EnableSinglePades() As Boolean
        Get
            Return GetBoolean("EnableSinglePades", False)
        End Get
    End Property

    ''' <summary> Abilita la firma multipla in PAdES. </summary>
    Public ReadOnly Property EnableMultiPades() As Boolean
        Get
            Return EnableMultiSign AndAlso GetBoolean("EnableMultiPades", False)
        End Get
    End Property

    ''' <summary> Abilita la firma multipla </summary>
    ''' <remarks> Sovrascrive anche EnableMultiPades </remarks>
    Public ReadOnly Property EnableMultiSign() As Boolean
        Get
            Return GetBoolean("EnableMultiSign", False)
        End Get
    End Property

    Public ReadOnly Property ChekPDFSigned() As Boolean
        Get
            Return GetBoolean("ChekPDFSigned", True)
        End Get
    End Property

    Public ReadOnly Property ProtocolLogReadTypes As List(Of String)
        Get
            Dim mandatory As New List(Of String)() From {"P1", "PS"}
            Dim value As String = GetString("ProtocolLogReadTypes")

            If String.IsNullOrWhiteSpace(value) Then
                Return mandatory
            End If
            Dim splitted As IEnumerable(Of String) = value.Split(","c).Where(Function(s) Not String.IsNullOrWhiteSpace(s)).Select(Function(s) s.Trim())

            mandatory.AddRange(splitted)
            Return mandatory.Distinct().ToList()
        End Get
    End Property

    Public ReadOnly Property ProtocolLogEditTypes As List(Of String)
        Get
            Dim mandatory As New List(Of String)() From {"PI"}
            Dim value As String = GetString("ProtocolLogEditTypes")

            If String.IsNullOrWhiteSpace(value) Then
                Return mandatory
            End If

            Dim splitted As IEnumerable(Of String) = value.Split(","c).Where(Function(s) Not String.IsNullOrWhiteSpace(s)).Select(Function(s) s.Trim())

            mandatory.AddRange(splitted)
            Return mandatory.Distinct().ToList()
        End Get
    End Property

    Public ReadOnly Property ProtocolLogEditDaysThreshold As Integer
        Get
            Return GetInteger("ProtocolLogEditDaysThreshold")
        End Get
    End Property

    Public ReadOnly Property DisableProtocolLogHasBeenRead As Boolean
        Get
            Return GetBoolean("DisableProtocolLogHasBeenRead")
        End Get
    End Property

    Public ReadOnly Property ADDisplayProperty() As String
        Get
            Return GetString("ADDisplayProperty", "displayname")
        End Get
    End Property

    Public ReadOnly Property SegnatureDefaultUseCheck() As Boolean
        Get
            Return GetBoolean("SegnatureDefaultUseCheck", True)
        End Get
    End Property

    Public ReadOnly Property SegnatureOptional() As Boolean
        Get
            Return GetBoolean("SegnatureOptional", False)
        End Get
    End Property

    Public ReadOnly Property EnableUserProfile As Boolean
        Get
            Return GetBoolean("EnableUserProfile", False)
        End Get
    End Property

    Public ReadOnly Property ForceUserLogEmail As Boolean
        Get
            Return GetBoolean("ForceUserLogEmail", False)
        End Get
    End Property

    Public ReadOnly Property AutoLoadRoles As Boolean
        Get
            Return GetBoolean("AutoLoadRoles", False)
        End Get
    End Property

    Public ReadOnly Property ReplyFromOriginalBox As Boolean
        Get
            Return GetBoolean("ReplyFromOriginalBox", True)
        End Get
    End Property

    Public ReadOnly Property ShowContactCount As Boolean
        Get
            Return GetBoolean("ShowContactCount", False)
        End Get
    End Property

    ''' <summary> Indica se visualizzare in collaborazione l'inserimento da contatto manuale. </summary>
    Public ReadOnly Property CollManualContactEnabled As Boolean
        Get
            Return GetBoolean("CollManualContactEnabled", True)
        End Get
    End Property

    Public ReadOnly Property ProtMinimumPecToShow As Integer
        Get
            Return GetInteger("ProtMinimumPecToShow", 0)
        End Get
    End Property


    Public ReadOnly Property SeriesImportGroups() As String
        Get
            Return GetString("SeriesImportGroups", String.Empty)
        End Get
    End Property

    Public ReadOnly Property UseConcourseWithGrid() As Boolean
        Get
            Return GetBoolean("UseConcourseWithGrid", False)
        End Get
    End Property

    Public ReadOnly Property TextBtnSearchContacts As String
        Get
            Return GetString("TextBtnSearchContacts", "Controlla nomi")
        End Get
    End Property

    Public ReadOnly Property TextSearchComplexityThreshold As Integer
        Get
            Return GetInteger("TextSearchComplexityThreshold", 4)
        End Get
    End Property


    ''' <summary> Tolleranza dell'automatismo di autorizzazione per l'estrazione dei settori. </summary>
    Public ReadOnly Property CollSecretaryRoleAllowance() As CollaborationSecretaryRoleAllowance
        Get
            Dim enumPosition As String = GetString("CollSecretaryRoleAllowance", "2")
            Dim toReturn As CollaborationSecretaryRoleAllowance = CollaborationSecretaryRoleAllowance.UserDestinationFirstRoles
            CollaborationSecretaryRoleAllowance.TryParse(enumPosition, toReturn)
            Return toReturn
        End Get
    End Property

    Public ReadOnly Property PECDefaultMainDocument() As String
        Get
            Return GetString("PECDefaultMainDocument", "postacert.eml")
        End Get
    End Property

    ''' <summary>Abilita le API di DocSuiteWeb.</summary>
    Public ReadOnly Property APIEnabled As Boolean
        Get
            Return GetBoolean("APIEnabled", False)
        End Get
    End Property

    ''' <summary>Indirizzo dell'APIProvider di default.</summary>
    Public ReadOnly Property APIDefaultProvider As String
        Get
            Return GetString("APIDefaultProvider", "http://localhost/dswapi/")
        End Get
    End Property

    ''' <summary>Numero di ore per cui verranno conservate le risposte in cache.</summary>
    Public ReadOnly Property APICacheExpiration As Integer
        Get
            Return GetInteger("APICacheExpiration", 10)
        End Get
    End Property

    ''' <summary> Identificativo serie documentale default per AVCP. </summary>
    Public ReadOnly Property AvcpDocumentSeriesId As Integer?
        Get
            Dim id As String = GetString("AvcpDocumentSeriesId")
            Dim documentSeriesId As Integer
            If Integer.TryParse(id, documentSeriesId) Then
                Return documentSeriesId
            End If
            Return Nothing
        End Get
    End Property

    ''' <summary>Mostra la voce Utente.</summary>
    Public ReadOnly Property ShowUser As Boolean
        Get
            Return GetBoolean("ShowUser", True)
        End Get
    End Property

    ''' <summary>Mostra la voce Protocollo.</summary>
    Public ReadOnly Property ShowProtocol As Boolean
        Get
            Return GetBoolean("ShowProtocol", True)
        End Get
    End Property

    ''' <summary> Nome del modulo degli Archivi </summary>
    Public ReadOnly Property DocumentSeriesName As String
        Get
            Return GetString("DocumentSeriesName", "Serie Documentali")
        End Get
    End Property

    ''' <summary> Id di default della PecMailView da utilizzare per visualizzare i documenti. </summary>
    Public ReadOnly Property ProtocolBoxPecMailViewDefault As Integer
        Get
            Return GetInteger("ProtocolBoxPecMailViewDefault", 0)
        End Get
    End Property

    Public ReadOnly Property OChartCommunicationDataName As String
        Get
            Return GetString("OChartCommunicationDataName", "OChartCommunicationData.xml")
        End Get
    End Property


    Public ReadOnly Property OChartEnabled() As Boolean
        Get
            Return GetBoolean("OChartEnabled", False)
        End Get
    End Property

    ''' <summary> Abilita la precompilazione in con i dati specificati nell'organigramma </summary>
    Public ReadOnly Property OChartProtocolPreloadingEnabled As Boolean
        Get
            Return OChartEnabled AndAlso GetBoolean("OChartProtocolPreloadingEnabled", False)
        End Get
    End Property

    Public ReadOnly Property OChartAdminGroups() As String
        Get
            Return GetString("OChartAdminGroups", String.Empty)
        End Get
    End Property

    Public ReadOnly Property OChartContactRoot() As Integer?
        Get
            Return GetNullableInt("OChartContactRoot")
        End Get
    End Property

    ''' <summary> Formato della segnatura per marcare i documenti dei protocolli </summary>
    Public ReadOnly Property ProtocolSignatureFormat As String
        Get
            Return GetString("ProtocolSignatureFormat", "{0:Short}/{1:Number:0000000}/{1:Date:dd/MM/yyyy}/{1:Direction:Short}/{1:Container:Name}&lt;{2:DocumentType:Short}/AA.{2:AttachmentsCount}&gt;")
        End Get
    End Property
    ''' <summary> Formato della segnatura per marcare gli allegati dei protocolli </summary>
    Public ReadOnly Property AttachmentSignatureFormat As String
        Get
            Return GetString("AttachmentSignatureFormat", "{0:Short}/{1:Number:0000000}/{1:Date:dd/MM/yyyy}/{1:Direction:Short}/{1:Container:Name}&lt;{2:DocumentType:Short}.{2:DocumentNumber}&gt;")
        End Get
    End Property
    ''' <summary> Formato della segnatura per marcare gli annessi dei protocolli </summary>
    Public ReadOnly Property AnnexedSignatureFormat As String
        Get
            Return GetString("AnnexedSignatureFormat", "{0:Short}/{1:Number:0000000}/{1:Date:dd/MM/yyyy}/{1:Direction:Short}/{1:Container:Name}")
        End Get
    End Property

    Public ReadOnly Property ReportRicevutaHeaderDescription As String
        Get
            Return GetString("ReportRicevutaHeaderDescription", String.Empty)
        End Get
    End Property

    Public ReadOnly Property PecToProtContact() As Integer
        Get
            Return GetInteger("PecToProtContact", 0)
        End Get
    End Property

    Public ReadOnly Property ProtocolBoxToProtContact() As Integer
        Get
            Return GetInteger("ProtocolBoxToProtContact", 0)
        End Get
    End Property

    Public ReadOnly Property AVCPDatasetUrlMask() As String
        Get
            Return GetString("AVCPDatasetUrlMask", String.Empty)
        End Get
    End Property

    Public ReadOnly Property AVCPInclusiveNumberMask() As String
        Get
            Return GetString("AVCPInclusiveNumberMask", "{0:0000}/{2:0000000}")
        End Get
    End Property

    Public ReadOnly Property ProtCorrectionGroups() As String
        Get
            Return GetString("ProtCorrectionGroups", String.Empty)
        End Get
    End Property

    Public ReadOnly Property AVCPConfigFolder() As String
        Get
            Return GetString("AVCPConfigFolder", "./Config/AVCP/")
        End Get
    End Property

    Public ReadOnly Property AVCPImporterGroup() As String
        Get
            Return GetString("AVCPImporterGroup", String.Empty)
        End Get
    End Property

    Public ReadOnly Property AVCPResolutionType() As Integer
        Get
            Return GetInteger("AVCPResolutionType", 0)
        End Get
    End Property

    Public ReadOnly Property AVCPDefaultCategoryId() As Integer
        Get
            Return GetInteger("AVCPDefaultCategoryId")
        End Get
    End Property

    Public ReadOnly Property AVCPLinkToResolution() As Boolean
        Get
            Return GetBoolean("AVCPLinkToResolution", False)
        End Get
    End Property

    Public ReadOnly Property AVCPEntePubblicatore() As String
        Get
            Return GetString("AVCPEntePubblicatore", String.Empty)
        End Get
    End Property

    Public ReadOnly Property AVCPLicenza() As String
        Get
            Return GetString("AVCPLicenza", String.Empty)
        End Get
    End Property

    Public ReadOnly Property AVCPCfEnteAppaltante() As String
        Get
            Return GetString("AVCPCfEnteAppaltante", String.Empty)
        End Get
    End Property

    Public ReadOnly Property ConcourseExcelExportDynamic As Boolean
        Get
            Return GetBoolean("ConcourseExcelExportDynamic", False)
        End Get
    End Property

    Public ReadOnly Property ContainerBehaviourEnabled As Boolean
        Get
            Return GetBoolean("ContainerBehaviourEnabled", False)
        End Get
    End Property

    Public ReadOnly Property TaskTempFolder() As String
        Get
            Return GetString("TaskTempFolder", String.Empty)
        End Get
    End Property

    Public ReadOnly Property CheckExistRecipientsAndRepair() As Boolean
        Get
            Return GetBoolean("CheckExistRecipientsAndRepair", True)
        End Get
    End Property

    Public ReadOnly Property TaskHeaderGroups() As String
        Get
            Return GetString("TaskHeaderGroups", String.Empty)
        End Get
    End Property

    Public ReadOnly Property TaskTypeEnabled() As String
        Get
            Return GetString("TaskTypeEnabled", String.Empty)
        End Get
    End Property

    Public ReadOnly Property WsCollCheckUserRole As Boolean
        Get
            Return GetBoolean("WsCollCheckUserRole", True)
        End Get
    End Property

    ''' <summary> Indica se abilitare la decompressione degli zip nel viewerlight </summary>
    Public ReadOnly Property ViewerUnzip As Boolean
        Get
            Return GetBoolean("ViewerUnzip", False)
        End Get
    End Property

    Public ReadOnly Property EnvGroupTblStampe As String
        Get
            Return GetString("GroupTblStampe", String.Empty)
        End Get
    End Property

    Public ReadOnly Property EnvGroupTblStampeSecurity As String
        Get
            Return GetString("GroupTblStampeSecurity", String.Empty)
        End Get
    End Property

    Public ReadOnly Property EnvGroupTblRoleAdmin As String
        Get
            Return GetString("GroupTblRoleAdmin", String.Empty)
        End Get
    End Property

    Public ReadOnly Property EnvGroupTblContainerAdmin As String
        Get
            Return GetString("GroupTblContainerAdmin", String.Empty)
        End Get
    End Property

    Public ReadOnly Property GroupPecAutoHandle As String
        Get
            Return GetString("GroupPecAutoHandle", String.Empty)
        End Get
    End Property

    ''' <summary> Mostra il corpo del messaggio della PEC in visualizzazione </summary>
    Public ReadOnly Property PecShowBody() As Boolean
        Get
            Return GetBoolean("PecShowBody", False)
        End Get
    End Property

    Public ReadOnly Property InserimentoAllaVisioneFirmaEnabled As Boolean
        Get
            Return GetBoolean("InserimentoAllaVisioneFirmaEnabled", True)
        End Get
    End Property

    Public ReadOnly Property InserimentoAlProtocolloSegreteriaEnabled As Boolean
        Get
            Return GetBoolean("InserimentoAlProtocolloSegreteriaEnabled", True)
        End Get
    End Property

    Public ReadOnly Property CheckEmptyPecAddressFromSummary As Boolean
        Get
            Return GetBoolean("CheckEmptyPecAddressFromSummary", True)
        End Get
    End Property

    ''' <summary> Visibilitࠤella colonna Assegnatari/Proponenti nella griglia dei Protocolli da leggere </summary>
    Public ReadOnly Property DaLeggereSubject As Boolean
        Get
            Return GetBoolean("DaLeggereSubject", False)
        End Get
    End Property

    Public ReadOnly Property CollaborationRefreshButtonVisible As Boolean
        Get
            Return GetBoolean("CollaborationRefreshButtonVisible", True)
        End Get
    End Property

    ''' <summary> Imposta la firma predefinita, dove disponibile </summary>
    Public ReadOnly Property DefaultSignType() As String
        Get
            Return GetString("DefaultSignType", "cades")
        End Get
    End Property

    Public ReadOnly Property EnvGroupOriginalEml As String
        Get
            Return GetString("GroupOriginalEml", String.Empty)
        End Get
    End Property

    Public ReadOnly Property EnableCc As Boolean
        Get
            Return GetBoolean("EnableCc", False)
        End Get
    End Property

    Public ReadOnly Property ProtocolLogShowOnlyCurrentIfNotAdmin As Boolean
        Get
            Return GetBoolean("ProtocolLogShowOnlyCurrentIfNotAdmin", False)
        End Get
    End Property

    ''' <summary> Indica il tipo di firma multipla da rendere visibile all'utente </summary>
    Public ReadOnly Property MultipleSignType() As Integer
        Get
            Return GetInteger("MultipleSignType", 0)
        End Get
    End Property

    Public ReadOnly Property PECHandleContainerRight As Integer
        Get
            Return GetInteger("PECHandleContainerRight", 1)
        End Get
    End Property

    Public ReadOnly Property PECToProtocolFixedDirection As Boolean
        Get
            Return GetBoolean("PECToProtocolFixedDirection", False)
        End Get
    End Property

    Public ReadOnly Property ProtocollableOutgoingPEC As Boolean
        Get
            Return GetBoolean("ProtocollableOutgoingPEC", False)
        End Get
    End Property

    Public ReadOnly Property ProtocolDocumentHandlerStatusCancel As Boolean
        Get
            Return GetBoolean("ProtocolDocumentHandlerStatusCancel", False)
        End Get
    End Property

    Public ReadOnly Property RoleGroupPECRightEnabled As Boolean
        Get
            Return GetBoolean("RoleGroupPECRightEnabled")
        End Get
    End Property
    Public ReadOnly Property RoleGroupProcotolMailBoxRightEnabled As Boolean
        Get
            Return GetBoolean("RoleGroupProcotolMailBoxRightEnabled")
        End Get
    End Property

    Public ReadOnly Property PecSendMaximumSizeMargin As String
        Get
            Return GetString("PecSendMaximumSizeMargin", "37%")
        End Get
    End Property

    Public ReadOnly Property CategoryFullCodeFormatType As Integer
        Get
            Return GetInteger("CategoryFullCodeFormatType", 1)
        End Get
    End Property

    Public ReadOnly Property AllowZeroBytesUpload As Boolean
        Get
            Return GetBoolean("AllowZeroBytesUpload", False)
        End Get
    End Property

    Public ReadOnly Property CollaborationSourceProtocolEnabled As Boolean
        Get
            Return IsCollaborationEnabled AndAlso GetBoolean("CollaborationSourceProtocolEnabled")
        End Get
    End Property

    Public ReadOnly Property DisableRemoveAttachments As Boolean
        Get
            Return GetBoolean("DisableRemoveAttachments", False)
        End Get
    End Property

    Public ReadOnly Property SizeThresholdRemoveAttachments As Integer
        Get
            Return GetInteger("SizeThresholdRemoveAttachments", 1024 * 1024)
        End Get
    End Property

    Public ReadOnly Property EnablePecAttachmentListFromProtocol As Boolean
        Get
            Return GetBoolean("EnablePecAttachmentListFromProtocol", True)
        End Get
    End Property

    ''' <summary> Legge il valore kb da parameterEnv e lo restituisce in byte </summary>
    Public ReadOnly Property WarningUploadThreshold As Integer
        Get
            Return GetInteger("WarningUploadThreshold", 35000) * 1024
        End Get
    End Property

    ''' <summary>
    ''' Definisce la modalitࠤi notifica del warning. Valori accettati:
    ''' "ErrorHolder" --> pannello su upload
    ''' "Alert" --> alert javascript in seguito alla conferma
    ''' </summary>
    Public ReadOnly Property WarningUploadThresholdType As String
        Get
            Return GetString("WarningUploadThresholdTyep", "ErrorHolder")
        End Get
    End Property

    ''' <summary> Legge il valore kb da parameterEnv e lo restituisce in byte </summary>
    Public ReadOnly Property MaxUploadThreshold As Integer
        Get
            Return GetInteger("MaxUploadThreshold", 50000) * 1024
        End Get
    End Property

    Public ReadOnly Property StandardUploadHeight As Integer
        Get
            Return GetInteger("StandardUploadHeight", 300)
        End Get
    End Property

    Public ReadOnly Property MultipleUploadHeight As Integer
        Get
            Return GetInteger("MultipleUploadHeight", 300)
        End Get
    End Property

    Public ReadOnly Property StandardUploadWidth As Integer
        Get
            Return GetInteger("StandardUploadWidth", 600)
        End Get
    End Property

    Public ReadOnly Property MultipleUploadWidth As Integer
        Get
            Return GetInteger("MultipleUploadWidth", 600)
        End Get
    End Property

    ''' <summary> Abilita la scelta dei singoli allegati di origine di una PEC </summary>
    Public ReadOnly Property EnablePecAttachmentListFromPec As Boolean
        Get
            Return GetBoolean("EnablePecAttachmentListFromPec", True)
        End Get
    End Property

    Public ReadOnly Property CollaborationManagementExpired As Boolean
        Get
            Return GetBoolean("CollaborationManagementExpired", False)
        End Get
    End Property

    Public ReadOnly Property FastProtocolSenderEnabled As Boolean
        Get
            Return GetBoolean("FastProtocolSenderEnabled", False)
        End Get
    End Property

    Public ReadOnly Property MaxNumberOfCharsObjectInGridPec As Integer
        Get
            Return GetInteger("MaxNumberOfCharsObjectInGridPec", 0)
        End Get
    End Property

    Public ReadOnly Property MaxNumberOfCharsRecipientInGridPec As Integer
        Get
            Return GetInteger("MaxNumberOfCharsRecipientInGridPec", 0)
        End Get
    End Property

    Public ReadOnly Property PECSourceProtocolEnabled As Boolean
        Get
            Return FastProtocolSenderEnabled AndAlso GetBoolean("PECSourceProtocolEnabled", False)
        End Get
    End Property

    Public ReadOnly Property PECSourceProtocolPattern As String
        Get
            Return GetString("PECSourceProtocolPattern", "\[[1,2]{1}[0,9]{1}[0-9]{9}\]")
        End Get
    End Property

    ''' <summary> Abilita la visualizzazione delle tabelle messaggi </summary>
    Public ReadOnly Property EnableMessageView As Boolean
        Get
            Return GetBoolean("EnableMessageView")
        End Get
    End Property

    ''' <summary> Nome della visualizzazione dei messaggi </summary>
    Public ReadOnly Property MessageViewName As String
        Get
            Return GetString("MessageViewName", "Diario posta inviata")
        End Get
    End Property

    Public ReadOnly Property PECSourceProtocolIdContainer As Integer
        Get
            Return GetInteger("PECSourceProtocolIdContainer")
        End Get
    End Property

    Public ReadOnly Property PECSourceProtocolIdRoles As String
        Get
            Return GetString("PECSourceProtocolIdRoles", String.Empty)
        End Get
    End Property

    ''' <summary> Definisce se visualizzare o meno il pannello di ordinamento nella visualizzazione "Casella di protocollazione" </summary>
    Public ReadOnly Property ProtocolBoxShowOrdinamento As Boolean
        Get
            Return GetBoolean("ProtocolBoxShowOrdinamento", False)
        End Get
    End Property

    Public ReadOnly Property ProtocolStatusWcfDefault As String
        Get
            Return GetString("ProtocolStatusWcfDefault", String.Empty)
        End Get
    End Property

    Public ReadOnly Property DocumentSeriesIsMineRightEnabled As Boolean
        Get
            Return GetBoolean("DocumentSeriesIsMineRightEnabled", True)
        End Get
    End Property

    Public ReadOnly Property DisableProtRisultatiReportButtons As Boolean
        Get
            Return GetBoolean("DisableProtRisultatiReportButtons")
        End Get
    End Property

    Public ReadOnly Property ShowConfirmMessageOnSendMail As Boolean
        Get
            Return GetBoolean("ShowConfirmMessageOnSendMail", True)
        End Get
    End Property

    Public ReadOnly Property PecMailViewDefaultToSend As Integer
        Get
            Return GetInteger("PecMailViewDefaultToSend", -1)
        End Get
    End Property

    Public ReadOnly Property DocumentSeriesLogGroups As String
        Get
            Return GetString("DocumentSeriesLogGroups", String.Empty)
        End Get
    End Property

    Public ReadOnly Property PosteWebDefaultSender As String
        Get
            Return GetString("PosteWebDefaultSender", String.Empty)
        End Get
    End Property

    Public ReadOnly Property IsAuthorizeInsertRequired() As Boolean
        Get
            Return GetBoolean("AuthorizeInsertRequired")
        End Get
    End Property

    Public ReadOnly Property IsFilenameAutomaticRenameEnabled() As Boolean
        Get
            Return GetBoolean("FilenameAutomaticRenameEnabled")
        End Get
    End Property

    Public ReadOnly Property GroupPosteWebReport() As String
        Get
            Return GetString("GroupPosteWebReport", String.Empty)
        End Get
    End Property

    Public ReadOnly Property DefaultTextMatchMode As String
        Get
            Return GetString("DefaultTextMatchMode", TextSearchBehaviour.TextMatchMode.Contains.ToString())
        End Get
    End Property

    Public ReadOnly Property MieiCheckOutMenuLabel As String
        Get
            Return GetString("MieiCheckOutMenuLabel", "Miei Check Out")
        End Get
    End Property

    Public ReadOnly Property CollaborationWhiteListEnabled As Boolean
        Get
            Return GetBoolean("CollaborationWhiteListEnabled")
        End Get
    End Property

    Public ReadOnly Property CollaborationWhiteList As String
        Get
            Return GetString("CollaborationWhiteList", ".txt|.doc|.docx|.xls|.xlsx")
        End Get
    End Property

    Public ReadOnly Property CustomErrorFrontPageDocument As String
        Get
            Return GetString("CustomErrorFrontPageDocument", String.Empty)
        End Get
    End Property

    Public ReadOnly Property CollaborationRightsEnabled As Boolean
        Get
            Return GetBoolean("CollaborationRightsEnabled", False)
        End Get
    End Property

    Public ReadOnly Property EnableViewerLightFileRename As Boolean
        Get
            Return GetBoolean("EnableViewerLightFileRename", True)
        End Get
    End Property

    Public ReadOnly Property CustomPECMailSubjectEnabled As Boolean
        Get
            Return GetBoolean("CustomPECMailSubjectEnabled")
        End Get
    End Property
    ''' <summary>
    ''' Oggetto mail autorizzazione alla visione di  External Viewer
    ''' </summary>
    ''' <returns>
    ''' 0 . numero protocollo
    ''' </returns>
    Public ReadOnly Property ProtocolMailAuthExternalViewerSubject As String
        Get
            Return GetString("ProtocolMailAuthExternalViewerSubject", "Autorizzazione al protocollo aziendale n.{0}")
        End Get
    End Property

    Public ReadOnly Property CustomPECMailSubjectFormat As String
        Get
            Return GetString("CustomPECMailSubjectFormat", "DocSuite Protocollo n. {0}/{1:0000000} del {2:dd/MM/yyyy} - {3}")
        End Get
    End Property
    Public ReadOnly Property BasicPersonSearcherKey As String
        Get
            Return GetString("BasicPersonSearcherKey", "(&(&(objectCategory=person)(objectClass=user)(!(userAccountControl:1.2.840.113556.1.4.803:=2)))(|(cn=*{0}*)(sAMAccountName={0})))")
        End Get
    End Property

    Public ReadOnly Property BasicContactSearcherKey As String
        Get
            Return GetString("BasicContactSearcherKey", "(&(&(&(objectCategory=person)(objectClass=contact))(mail=*))(displayName=*{0}*))")
        End Get
    End Property

    Public ReadOnly Property BasicDistributionSearcherKey As String
        Get
            Return GetString("BasicDistributionSearcherKey", "(&(&(&(objectClass=group)(proxyAddresses=*))(mail=*))(displayName=*{0}*))")
        End Get
    End Property

    Public ReadOnly Property DefaultErrorMessage As String
        Get
            Return GetString("DefaultErrorMessage", "Se l anomalia si presenta nuovamente, contattare l assistenza informatica.")
        End Get
    End Property
    Public ReadOnly Property PECMoveRedirectOnConfirm As Integer
        Get
            Return GetInteger("PECMoveRedirectOnConfirm")
        End Get
    End Property
    Public ReadOnly Property EnableMessageViewCcColumn As Boolean
        Get
            Return GetBoolean("EnableMessageViewCcColumn", False)
        End Get
    End Property

    Public ReadOnly Property ProtocolSearchLocationEnabled As Boolean
        Get
            Return GetBoolean("ProtocolSearchLocationEnabled", True)
        End Get
    End Property

    Public ReadOnly Property SelectableProtocolThreshold As Integer
        Get
            Return GetInteger("SelectableProtocolThreshold", 20)
        End Get
    End Property

    Public ReadOnly Property MessageObjectMaxLength As Integer
        Get
            Return GetInteger("MessageObjectMaxLength", 500)
        End Get
    End Property

    Public ReadOnly Property EnableLinkToProtocolInNotificationCollaboration As Boolean
        Get
            Return GetBoolean("EnableLinkToProtocolInNotificationCollaboration", False)
        End Get
    End Property

    Public ReadOnly Property DeleteCollaborationsIfNoDocuments As Boolean
        Get
            Return GetBoolean("DeleteCollaborationsIfNoDocuments", True)
        End Get
    End Property

    Public ReadOnly Property ProtocolKindEnabled As Boolean
        Get
            Return GetBoolean("ProtocolKindEnabled", False)
        End Get
    End Property

    Public ReadOnly Property InvoicePAEnabled As Boolean
        Get
            Return GetBoolean("InvoicePAEnabled", False) AndAlso Not InvoiceSDIEnabled
        End Get
    End Property

    Public ReadOnly Property InvoiceSDIEnabled As Boolean
        Get
            Return GetBoolean("InvoiceSDIEnabled", False)
        End Get
    End Property

    Public ReadOnly Property InvoiceSDIB2BKind As Integer
        Get
            Return GetInteger("InvoiceSDIB2BKind", 0)
        End Get
    End Property

    Public ReadOnly Property InvoiceSDIPAKind As Integer
        Get
            Return GetInteger("InvoiceSDIPAKind", 0)
        End Get
    End Property

    Public ReadOnly Property TemplateProtocolEnable As Boolean
        Get
            Return GetBoolean("TemplateProtocolEnable", False)
        End Get
    End Property

    Public ReadOnly Property TemplateGroups() As String
        Get
            Return GetString("TemplateGroups", String.Empty)
        End Get
    End Property

    Public ReadOnly Property PECWarningMessage() As String
        Get
            Return GetString("PECWarningMessage", "ATTENZIONE: sono stati riscontrati problemi nella ricezione, eventualmente verificare il Log")
        End Get
    End Property

    Public ReadOnly Property EnableNextAfterMultiSign As Boolean
        Get
            Return GetBoolean("EnableNextAfterMultiSign", False)
        End Get
    End Property


    Public ReadOnly Property PECHandlerMoveNotificationTemplate() As String
        Get
            Return GetString("PECHandlerMoveNotificationTemplate", "L'utente %USER% (%USEREMAIL%) ha spostato la PEC %ID%  del %DATE% con oggetto: %SUBJECT%")
        End Get
    End Property

    Public ReadOnly Property EnableSignaturePECAccepted As Boolean
        Get
            Return GetBoolean("EnableSignaturePECAccepted", False)
        End Get
    End Property

    Public ReadOnly Property SignaturePECAccepted As String
        Get
            Return GetString("SignaturePECAccepted", "{0} il: {1}")
        End Get
    End Property

    Public ReadOnly Property EnableSignaturePECReceived As Boolean
        Get
            Return GetBoolean("EnableSignaturePECReceived", False)
        End Get
    End Property

    Public ReadOnly Property SignaturePECReceived As String
        Get
            Return GetString("SignaturePECReceived", "{0} il: {1}")
        End Get
    End Property

    Public ReadOnly Property EnableSignatureMailReceived As Boolean
        Get
            Return GetBoolean("EnableSignatureMailReceived", False)
        End Get
    End Property

    Public ReadOnly Property SignatureMailReceived As String
        Get
            Return GetString("SignatureMailReceived", "{0} il: {1}")
        End Get
    End Property

    Public ReadOnly Property EnableSignatureMailSend As Boolean
        Get
            Return GetBoolean("EnableSignatureMailSend", False)
        End Get
    End Property

    Public ReadOnly Property SignatureMailSend As String
        Get
            Return GetString("SignatureMailSend", "{0} il: {1}")
        End Get
    End Property

    Public ReadOnly Property EnableLinkToProtocol As Boolean
        Get
            Return GetBoolean("EnableLinkToProtocol", False)
        End Get
    End Property
    Public ReadOnly Property PECMoveNotificationType As Integer
        Get
            Return GetInteger("PECMoveNotificationType", 1)
        End Get
    End Property

    Public ReadOnly Property EnableUnifiedDiary As Boolean
        Get
            Return GetBoolean("EnableUnifiedDiary", False)
        End Get
    End Property

    Public ReadOnly Property MaxDaySearchUnifiedDiary As Integer
        Get
            Return GetInteger("MaxDaySearchUnifiedDiary", 30)
        End Get
    End Property

    Public ReadOnly Property CheckOutCollaborationOnlyForSigners As Boolean
        Get
            Return GetBoolean("CheckOutCollaborationOnlyForSigners", False)
        End Get
    End Property

    Public ReadOnly Property EnableFlushAnnexed As Boolean
        Get
            Return GetBoolean("EnableFlushAnnexed", False)
        End Get
    End Property
    ''' <summary>
    ''' Abilita la sezione "Frontalini e privacy" nella sezione Tabelle>Contenitori>Modifica
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property EnableEditingHeadingFrontalini As Boolean
        Get
            Return GetBoolean("EnableEditingHeadingFrontalini", False)
        End Get
    End Property

    Public ReadOnly Property UploadDocumentSeriesRenameMaxLength As Integer
        Get
            Return GetInteger("UploadDocumentSeriesRenameMaxLength", 0)
        End Get
    End Property

    Public ReadOnly Property EnableButtonAllega As Boolean
        Get
            Return GetBoolean("EnableButtonAllega", True)
        End Get
    End Property
    Public ReadOnly Property EnableButtonLinkProtocolSend As Boolean
        Get
            Return GetBoolean("EnableButtonLinkProtocolSend", True)
        End Get
    End Property
    Public ReadOnly Property EnableButtonLinkProtocolPrint As Boolean
        Get
            Return GetBoolean("EnableButtonLinkProtocolPrint", True)
        End Get
    End Property

    Public ReadOnly Property EnableDictionDocumentSign As Boolean
        Get
            'Return GetBoolean("EnableDictionDocumentSigns", True)
            Return True
        End Get
    End Property
    Public ReadOnly Property PECDeleteButtonEnabled As Boolean
        Get
            Return GetBoolean("PECDeleteButtonEnabled", False)
        End Get
    End Property

    Public ReadOnly Property DefaultContactParentForInteropSender As Integer
        Get
            Return GetInteger("DefaultContactParentForInteropSender", 0)
        End Get
    End Property

    Public ReadOnly Property ProtocolContactTextSearchMode As Integer
        Get
            Return GetInteger("ProtocolContactTextSearchMode", 0)
        End Get
    End Property

    Public ReadOnly Property MessageMaxSize As Integer
        Get
            Return GetInteger("MessageMaxSize", 10485760)
        End Get
    End Property

    Public ReadOnly Property MessageMaxSizeAlertMessage As String
        Get
            Return GetString("MessageMaxSizeAlertMessage", "La email supera i limiti aziendali delle dimensioni massime per la posta elettronica. Utilizzare i servizi di large email")
        End Get
    End Property

    Public ReadOnly Property DeskEnable As Boolean
        Get
            Return GetBoolean("DeskEnable", False)
        End Get
    End Property

    ''' <summary>
    ''' Contiene il percorso del versionamento dei documenti in checkout di Tavoli (Desk)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property DeskShare As String
        Get
            Return GetString("DeskShare", String.Empty)
        End Get
    End Property

    ''' <summary>
    ''' Abilita il checkout dei documenti di tavoli, solo se 蠶alorizzato un percorso in "DeskShare"
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property DeskShareCheckOutEnabled As Boolean
        Get
            Return Not String.IsNullOrWhiteSpace(DeskShare) AndAlso GetBoolean("DeskShareCheckOutEnabled", False)
        End Get
    End Property

    ''' <summary>
    ''' Abilita la pagina di amministrazione delle serie documentali in anni diversi.
    ''' Abilita la possibilita di inserire serie documentali in anni diversi da quello corrente 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ChangeYearDocumentSeriesEnabled As Boolean
        Get
            Return GetBoolean("ChangeYearDocumentSeriesEnabled", False)
        End Get
    End Property


    ''' <summary>
    ''' Abilita la visione del buttone per l'inserimento di una firma in una messaggio pec
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property InsertDefaultBodyButtonEnabled As Boolean
        Get
            Return GetBoolean("InsertDefaultBodyButtonEnabled", False)
        End Get
    End Property

    Public ReadOnly Property DefaultPECBodyContent As String
        Get
            Return GetString("DefaultPECBodyContent", String.Empty)
        End Get
    End Property

    Public ReadOnly Property GenericStoryBoardMaxDay As Integer
        Get
            Return GetInteger("GenericStoryBoardMaxDay", 10)
        End Get
    End Property

    Public ReadOnly Property SeriesFromResolutionKindObjectFormat As String
        Get
            Return GetString("SeriesFromResolutionKindObjectFormat", String.Empty)
        End Get
    End Property

    Public ReadOnly Property AVCPIdBusinessContact As Integer
        Get
            Return GetInteger("AVCPIdBusinessContact", 0)
        End Get
    End Property

    Public ReadOnly Property SenderToProtocolGridVisible As Boolean
        Get
            Return GetBoolean("SenderToProtocolGridVisible", False)
        End Get
    End Property

    Public ReadOnly Property ContactToProtocolGridVisible As Boolean
        Get
            Return GetBoolean("ContactToProtocolGridVisible", False)
        End Get
    End Property

    Public ReadOnly Property CategoryToProtocolGridVisible As Boolean
        Get
            Return GetBoolean("CategoryToProtocolGridVisible", True)
        End Get
    End Property

    Public ReadOnly Property LastUserProtocolUpdateMode() As Integer
        Get
            Return GetInteger("LastUserProtocolUpdateMode", 0)
        End Get
    End Property

    Public ReadOnly Property SMSPecNotificationEnabled As Boolean
        Get
            Return GetBoolean("SMSPecNotificationEnabled", False)
        End Get
    End Property

    Public ReadOnly Property SMSConfigurationGroups As String
        Get
            Return GetString("SMSConfigurationGroups", String.Empty)
        End Get
    End Property

    Public ReadOnly Property BandiGaraDocumentSeriesId As Integer?
        Get
            Return GetNullableInt("BandiGaraDocumentSeriesId")
        End Get
    End Property

    Public ReadOnly Property RenameAttachmentGroups As String
        Get
            Return GetString("RenameAttachmentGroups", String.Empty)
        End Get
    End Property

    Public ReadOnly Property SMSUserProfileReadOnly As Boolean
        Get
            Return GetBoolean("SMSUserProfileReadOnly", False)
        End Get
    End Property
    Public ReadOnly Property MailBodyProtocolLinksEnabled As Boolean
        Get
            Return GetBoolean("MailBodyProtocolLinksEnabled", True)
        End Get
    End Property

    Public ReadOnly Property ViewLightSelectAllEnabled As Boolean
        Get
            Return GetBoolean("ViewLightSelectAllEnabled", True)
        End Get
    End Property

    Public ReadOnly Property ViewLightAlwaysOpenPages() As ICollection(Of String)
        Get
            Return GetStrings("ViewLightAlwaysOpenPages")
        End Get
    End Property

    Public ReadOnly Property StampaConformeMessageError As String
        Get
            Return GetString("StampaConformeMessageError", "Errore in stampa conforme. Viene restituito il pdf originale {0}.")
        End Get
    End Property

    Public ReadOnly Property ProtocolBoxAllButtonEnabled As Boolean
        Get
            Return GetBoolean("ProtocolBoxAllButtonEnabled", False)
        End Get
    End Property

    Public ReadOnly Property CollaborationVisionSignatureDataEditable As Boolean
        Get
            Return GetBoolean("CollaborationVisionSignatureDataEditable", False)
        End Get
    End Property

    Public ReadOnly Property ResizeSignWindowEnabled As Boolean
        Get
            Return GetBoolean("ResizeSignWindowEnabled", False)
        End Get
    End Property

    Public ReadOnly Property CopyReslAdoptFromCollaborationEnable As Boolean
        Get
            Return GetBoolean("CopyReslAdoptFromCollaborationEnabled", False)
        End Get
    End Property

    Public ReadOnly Property SelectSenderFromTreeviewEnabled As Boolean
        Get
            Return GetBoolean("SelectSenderFromTreeviewEnabled", False)
        End Get
    End Property

    Public ReadOnly Property UserChangePasswordEnabled As Boolean
        Get
            Return GetBoolean("UserChangePasswordEnabled", False)
        End Get
    End Property

    Public ReadOnly Property CoccardaProtocolEnabled As Boolean
        Get
            Return GetBoolean("CoccardaProtocolEnabled", False)
        End Get
    End Property
    Public ReadOnly Property JeepServiceModuleWarningDaysThreshold As Integer
        Get
            Return GetInteger("JeepServiceModuleWarningDaysThreshold", 0)
        End Get
    End Property

    Public ReadOnly Property RoleContactHistoricizing As Boolean
        Get
            Return GetBoolean("RoleContactHistoricizing", False)
        End Get
    End Property

    Public ReadOnly Property DeleteMultipleMailRecipientPages() As ICollection(Of String)
        Get
            Return GetStrings("DeleteMultipleMailRecipientPages")
        End Get
    End Property

    Public ReadOnly Property SignDeskDocumentEnabled As Boolean
        Get
            Return GetBoolean("SignDeskDocumentEnabled", False)
        End Get
    End Property

    Public ReadOnly Property CollapsePECHeaderEnabled As Boolean
        Get
            Return GetBoolean("CollapsePECHeaderEnabled", False)
        End Get
    End Property

    Public ReadOnly Property ProtocolTypeSenderAuthorizationInsert As ICollection(Of Integer)
        Get
            Return GetIntegers("ProtocolTypeSenderAuthorizationInsert", "-1|0|1")
        End Get
    End Property

    Public ReadOnly Property CollaborationTipologiaDefault As String
        Get
            Return GetString("CollaborationTipologiaDefault", String.Empty)
        End Get
    End Property

    Public ReadOnly Property CollaborationMenuAlwaysVisible As Boolean
        Get
            Return GetBoolean("CollaborationMenuAlwaysVisible", False)
        End Get
    End Property

    Public ReadOnly Property DeskCollaborationLinkVisible As Boolean
        Get
            Return GetBoolean("DeskCollaborationLinkVisible", True)
        End Get
    End Property

    Public ReadOnly Property ToReadProtocolTypeFinderEnabled As Boolean
        Get
            Return GetBoolean("ToReadProtocolTypeFinderEnabled", False)
        End Get
    End Property


    Public ReadOnly Property ImportDocumentSeriesRecoveryFolder As String
        Get
            Return GetString("ImportDocumentSeriesRecoveryFolder", String.Empty)
        End Get
    End Property

    Public ReadOnly Property UploadSharepointDocumentLibrary As Boolean
        Get
            Return GetBoolean("UploadSharepointDocumentLibrary", False)
        End Get
    End Property
    '
    Public ReadOnly Property CollaborationAggregateEnabled As Boolean
        Get
            Return GetBoolean("CollaborationAggregateEnabled", False)
        End Get
    End Property
    Public ReadOnly Property CollaborationRoleUoia As Integer
        Get
            Return GetInteger("CollaborationRoleUoia", 0)
        End Get
    End Property

    Public ReadOnly Property ShowAVCPFiscalCodeCompany As Boolean
        Get
            Return GetBoolean("ShowAVCPFiscalCodeCompany", False)
        End Get
    End Property

    Public ReadOnly Property AVCPAddSelContactEnabled As Boolean
        Get
            Return GetBoolean("AVCPAddSelContactEnabled", True)
        End Get
    End Property
    ''' <summary> Rende obbligatorio l'inserimento del mittente </summary>
    Public ReadOnly Property IsProtSenderRequired As Boolean
        Get
            Return GetBoolean("IsProtSenderRequired", False)
        End Get
    End Property

    Public ReadOnly Property ZipUploadEnabled As Boolean
        Get
            Return GetBoolean("ZipUploadEnabled", False)
        End Get
    End Property

    ''' <summary> Permette di aggiungere destinatari in Rispondi PEC </summary>
    Public ReadOnly Property AddPECRecipientsEnabled As Boolean
        Get
            Return GetBoolean("AddPECRecipientsEnabled", False)
        End Get
    End Property

    ''' <summary> Permette di visualizzare nelle PEC in entrata il pulsante "Rispondi a tutti"  </summary>
    Public ReadOnly Property PECReplyAllEnabled As Boolean
        Get
            Return GetBoolean("PECReplyAllEnabled", False)
        End Get
    End Property

    ''' <summary> Governa la visibilitࠤel filtro delle collaborazioni da visualizzare nelle griglie  </summary>
    Public ReadOnly Property CollaborationFilterEnabled As Boolean
        Get
            Return GetBoolean("CollaborationFilterEnabled", True)
        End Get
    End Property

    Public ReadOnly Property CollaborationColumnsVisibility As List(Of GridViewModel)
        Get
            Return GetJson(Of List(Of GridViewModel))("CollaborationColumnsVisibility", My.Resources.CollaborationGridColumns)
        End Get
    End Property

    Public ReadOnly Property DSWEnable As String
        Get
            Return GetString("DSWEnable", String.Empty)
        End Get
    End Property

    Public ReadOnly Property CollaborationConfirmButtonEnabled As Boolean
        Get
            Return GetBoolean("CollaborationConfirmButtonEnabled", True)
        End Get
    End Property

    Public ReadOnly Property TenantModels As List(Of TenantModel)
        Get
            Return GetJson(Of List(Of TenantModel))("TenantModel", String.Empty)
        End Get
    End Property

    Public ReadOnly Property TenantAuthorizationEnabled As Boolean
        Get
            Return GetBoolean("TenantAuthorizationEnabled", False)
        End Get
    End Property

    Public ReadOnly Property StrictManagerChange As Boolean
        Get
            Return GetBoolean("StrictManagerChange", False)
        End Get
    End Property

    Public ReadOnly Property FascicleContactId As Integer
        Get
            Return GetInteger("FascicleContactId", 0)
        End Get
    End Property

    Public ReadOnly Property PECInOutColumnsVisibility As List(Of GridViewModel)
        Get
            Return GetJson(Of List(Of GridViewModel))("PECInOutColumnsVisibility", My.Resources.PECInOutColumns)
        End Get
    End Property

    Public ReadOnly Property RolesUserProfileEnabled() As Boolean
        Get
            Return GetBoolean("RolesUserProfileEnabled", False)
        End Get
    End Property

    Public ReadOnly Property VirtualPECMailBoxProtocolableEnabled() As Boolean
        Get
            Return GetBoolean("VirtualPECMailBoxProtocolableEnabled", True)
        End Get
    End Property


    Public ReadOnly Property FascicolableThreshold As Integer
        Get
            Return GetInteger("FascicolableThreshold", 30)
        End Get
    End Property


    Public ReadOnly Property FascicolableThresholdDate As String
        Get
            Return GetString("FascicolableThresholdDate", "30/08/2016")
        End Get
    End Property

    Public ReadOnly Property DistributionHierarchicalEnabled() As Boolean
        Get
            Return GetBoolean("DistributionHierarchicalEnabled", True)
        End Get
    End Property

    Public ReadOnly Property RegistrationUserToProtocolGridVisible As Boolean
        Get
            Return GetBoolean("RegistrationUserToProtocolGridVisible", False)
        End Get
    End Property

    Public ReadOnly Property ProtocolHighlightEnabled As Boolean
        Get
            Return GetBoolean("ProtocolHighlightEnabled", False)
        End Get
    End Property

    Public ReadOnly Property UDSEnabled As Boolean
        Get
            Return GetBoolean("UDSEnabled", False)
        End Get
    End Property

    Public ReadOnly Property ProtocolContainerEditable As Boolean
        Get
            Return GetBoolean("ProtocolContainerEditable", True)
        End Get
    End Property

    Public ReadOnly Property CollaborationLocation As Integer
        Get
            Return GetInteger("CollaborationLocation", 99)
        End Get
    End Property
    Public ReadOnly Property SignedIconRenderingModality As SignedIconRenderingModality
        Get
            Return GetJson(Of SignedIconRenderingModality)("SignedIconRenderingModality", My.Resources.SignedIconRenderingModality)
        End Get
    End Property
    Public ReadOnly Property SeriesConfigurationOrders As List(Of DocumentSeriesConfigurationOrder)
        Get
            Return GetJson(Of List(Of DocumentSeriesConfigurationOrder))("AmministrazioneTrasparenteConfigurationOrders", My.Resources.DocumentSeriesConfigurationOrder)
        End Get
    End Property

    Public ReadOnly Property CollDocSignedNotEditable As Boolean
        Get
            Return GetBoolean("CollDocSignedNotEditable", False)
        End Get
    End Property

    Public ReadOnly Property PECMainDocumentConservationDownload As Boolean
        Get
            Return GetBoolean("PECMainDocumentConservationDownload", False)
        End Get
    End Property

    Public ReadOnly Property ProtocolHighlightSecurityEnabled As Boolean
        Get
            Return GetBoolean("ProtocolHighlightSecurityEnabled", False)
        End Get
    End Property

    Public ReadOnly Property HighlightProtocolGroups As HighlightProtocolGroup
        Get
            Return GetJson(Of HighlightProtocolGroup)("HighlightProtocolGroups", My.Resources.HighlightProtocolGroups)
        End Get
    End Property
    Public ReadOnly Property SelectCheckRecipientEnabled As Boolean
        Get
            Return GetBoolean("SelectCheckRecipientEnabled", False)
        End Get
    End Property
    Public ReadOnly Property ShowOnlySignAndNextEnabled As Boolean
        Get
            Return GetBoolean("ShowOnlySignAndNextEnabled", False)
        End Get
    End Property
    Public ReadOnly Property AVCPCIGUniqueValidationEnabled As Boolean
        Get
            Return GetBoolean("AVCPCIGUniqueValidationEnabled", True)
        End Get
    End Property

    Public ReadOnly Property DetectProtocolInDuplicateMetadatas As Boolean
        Get
            Return GetBoolean("DetectProtocolInDuplicateMetadatas", False)
        End Get
    End Property

    Public ReadOnly Property CheckSecurityUsersDomainEnabled As Boolean
        Get
            Return GetBoolean("CheckSecurityUsersDomainEnabled", False)
        End Get
    End Property
    Public ReadOnly Property IP4DEnabled As Boolean
        Get
            Return GetBoolean("IP4DEnabled", False)
        End Get
    End Property

    Public ReadOnly Property ExternalViewerProtocolLink As String
        Get
            Return GetString("ExternalViewerProtocolLink")
        End Get
    End Property
    Public ReadOnly Property ExternalViewerMyDocuments As String
        Get
            Return GetString("ExternalViewerMyDocuments")
        End Get
    End Property

    Public ReadOnly Property IP4DGroups As IList(Of String)
        Get
            Return GetJson(Of IList(Of String))("IP4DGroups", String.Empty)
        End Get
    End Property

    Public ReadOnly Property WorkflowManagerEnabled As Boolean
        Get
            Return GetBoolean("WorkflowManagerEnabled", False)
        End Get
    End Property

    Public ReadOnly Property MassivePropageCollaborationEnabled As Boolean
        Get
            Return GetBoolean("MassivePropageCollaborationEnabled", False)
        End Get
    End Property

    Public ReadOnly Property ContactNationalityEnabled As Boolean
        Get
            Return GetBoolean("ContactNationalityEnabled", False)
        End Get
    End Property

    Public ReadOnly Property RefusedProtocolAuthorizationEnabled As Boolean
        Get
            Return GetBoolean("RefusedProtocolAuthorizationEnabled", False)
        End Get
    End Property

    Public ReadOnly Property RefusedProtocolsGroups As String
        Get
            If IsSecurityGroupEnabled AndAlso RefusedProtocolAuthorizationEnabled Then
                Return GetString("RefusedProtocolsGroups", String.Empty)
            Else
                Return String.Empty
            End If
        End Get
    End Property
    Public ReadOnly Property SendPECDocumentEnabled As Boolean
        Get
            Return GetBoolean("SendPECDocumentEnabled", True)
        End Get
    End Property
    Public ReadOnly Property AssignProtocolEnabled As Boolean
        Get
            Return GetBoolean("AssignProtocolEnabled", False)
        End Get
    End Property
    Public ReadOnly Property PECWithErrorFilterEnabled As Boolean
        Get
            Return GetBoolean("PECWithErrorFilterEnabled", False)
        End Get
    End Property
    Public ReadOnly Property GroupsWithSearchProtocolRoleRestrictionNone As String
        Get
            Return GetString("GroupsWithSearchProtocolRoleRestrictionNone", String.Empty)
        End Get
    End Property
    Public ReadOnly Property ADUserEmailRestrictionEnabled As Boolean
        Get
            Return GetBoolean("ADUserEmailRestrictionEnabled", True)
        End Get
    End Property

    Public ReadOnly Property PosteWEBAttachmentsChecked As Boolean
        Get
            Return GetBoolean("PosteWEBAttachmentsChecked", False)
        End Get
    End Property
    Public ReadOnly Property SecurityGroupAdmin As String
        Get
            Return GetString("SecurityGroupAdmin", String.Empty)
        End Get
    End Property
    Public ReadOnly Property SecurityGroupPowerUser As String
        Get
            Return GetString("SecurityGroupPowerUser", String.Empty)
        End Get
    End Property
    Public ReadOnly Property SendProtocolMessageFromViewerEnabled As Boolean
        Get
            Return GetBoolean("SendProtocolMessageFromViewerEnabled", False)
        End Get
    End Property

    Public ReadOnly Property SimplifiedProtocolGridResultEnabled As Boolean
        Get
            Return GetBoolean("SimplifiedProtocolGridResultEnabled", False)
        End Get
    End Property

    Public ReadOnly Property HiddenSecurityGroupForNotAdmins As String
        Get
            Return GetString("HiddenSecurityGroupForNotAdmins", String.Empty)
        End Get
    End Property

    Public ReadOnly Property CostiPosteWebAccountEnabled As Boolean
        Get
            Return GetBoolean("CostiPosteWebAccountEnabled", False)
        End Get
    End Property

    Public ReadOnly Property ContactListsEnabled As Boolean
        Get
            Return GetBoolean("ContactListsEnabled", False)
        End Get
    End Property

    Public ReadOnly Property SelectedRoleNodeEnabled As Boolean
        Get
            Return GetBoolean("SelectedRoleNodeEnabled", True)
        End Get
    End Property

    Public ReadOnly Property ProtocolSearchAdaptiveEnabled As Boolean
        Get
            Return GetBoolean("ProtocolSearchAdaptiveEnabled", False)
        End Get
    End Property

    Public ReadOnly Property ProtocolNoteReservedRoleEnabled As Boolean
        Get
            Return GetBoolean("ProtocolNoteReservedRoleEnabled", False)
        End Get
    End Property

    Public ReadOnly Property UnifiedCollFunctionDesignerEnabled As Boolean
        Get
            Return GetBoolean("UnifiedCollFunctionDesignerEnabled", False)
        End Get
    End Property
    Public ReadOnly Property PECMailboxSelectEnabled As Boolean
        Get
            Return GetBoolean("PECMailboxSelectEnabled", False)
        End Get
    End Property
    Public ReadOnly Property ProtocolDefaultAdaptiveSearch As AdaptiveSearchModel
        Get
            Return GetJson(Of AdaptiveSearchModel)("ProtocolDefaultAdaptiveSearch", My.Resources.ProtocolDefaultAdaptiveSearch)
        End Get
    End Property

    Public ReadOnly Property IncludeInnerContactRootToRecipients As Boolean
        Get
            Return GetBoolean("IncludeInnerContactRootToRecipients", False)
        End Get
    End Property

    Public ReadOnly Property GroupsWithPosteWebAccountRestrictionNone As String
        Get
            Return GetString("GroupsWithPosteWebAccountRestrictionNone", String.Empty)
        End Get
    End Property

    Public ReadOnly Property ShowPECManualMultiVisibleButton() As Boolean
        Get
            Return GetBoolean("ShowPECManualMultiVisibleButton", False)
        End Get
    End Property

    Public ReadOnly Property OmniBusApplicationKey As String
        Get
            Return GetString("OmniBusApplicationKey", String.Empty)
        End Get
    End Property

    Public ReadOnly Property DefaultTenantEnabled As Boolean
        Get
            Return GetBoolean("DefaultTenantEnabled", True)
        End Get
    End Property

    Public ReadOnly Property FascicleMiscellaneaLocation As Integer
        Get
            Return GetInteger("FascicleMiscellaneaLocation", -1)
        End Get
    End Property

    Public ReadOnly Property AbsentManagersCertificates As AbsentManagerCertificateModel
        Get
            Return GetJson(Of AbsentManagerCertificateModel)("AbsentManagersCertificates", My.Resources.AbsentManagersCertificates)
        End Get
    End Property

    Public ReadOnly Property TemplateCollaborationGroups As String
        Get
            Return GetString("TemplateCollaborationGroups", String.Empty)
        End Get
    End Property

    Public ReadOnly Property TemplateDocumentRepositoryLocation As Integer
        Get
            Return GetInteger("TemplateDocumentRepositoryLocation", -1)
        End Get
    End Property

    Public ReadOnly Property PecColorProtocolLogEnabled As Boolean
        Get
            Return GetBoolean("PecColorProtocolLogEnabled", False)
        End Get
    End Property

    ''' <summary> Indica se fare la preselezione dei destinatari prima dell'invio mail. </summary>
    Public ReadOnly Property MailRecipientsSelectionEnabled As Boolean
        Get
            Return GetBoolean("MailRecipientsSelectionEnabled", False)
        End Get
    End Property

    Public ReadOnly Property TemplatesAuthorizations As String
        Get

            Return GetString("TemplatesAuthorizations", "[]")
        End Get
    End Property

    Public ReadOnly Property PosteWebValidateCivicNumber As Boolean
        Get
            Return GetBoolean("PosteWebValidateCivicNumber", True)
        End Get
    End Property

    Public ReadOnly Property TemplateDocumentGroups As String
        Get
            Return GetString("TemplateDocumentGroups", String.Empty)
        End Get
    End Property

    Public ReadOnly Property MetadataRepositoryGroups As String
        Get
            Return GetString("MetadataRepositoryGroups", String.Empty)
        End Get
    End Property

    Public ReadOnly Property TemplateDocumentVisibilities As ICollection(Of TemplateDocumentVisibilityConfiguration)
        Get
            Return GetJson(Of ICollection(Of TemplateDocumentVisibilityConfiguration))("TemplateDocumentVisibilities", My.Resources.TemplateDocumentVisibilities)
        End Get
    End Property
    Public ReadOnly Property PraticheEnabled As Boolean
        Get
            Return GetBoolean("PraticheEnabled", True) AndAlso DocSuiteContext.Current.IsDocumentEnabled
        End Get
    End Property

    Public ReadOnly Property DossierEnabled As Boolean
        Get
            Return GetBoolean("DossierEnabled", False)
        End Get
    End Property

    Public ReadOnly Property DocumentSeriesDocumentsLabel As IDictionary(Of Model.Entities.DocumentUnits.ChainType, String)
        Get
            Return GetJson(Of IDictionary(Of Model.Entities.DocumentUnits.ChainType, String))("DocumentSeriesDocumentsLabel", My.Resources.DocumentUnitDocumentsLabel)
        End Get
    End Property

    Public ReadOnly Property DocumentSeriesReorderDocumentEnabled As Boolean
        Get
            Return GetBoolean("DocumentSeriesReorderDocumentEnabled", False)
        End Get
    End Property

    Public ReadOnly Property DocumentSeriesRenameDocumentEnabled As Boolean
        Get
            Return GetBoolean("DocumentSeriesRenameDocumentEnabled", False)
        End Get
    End Property

    Public ReadOnly Property DossierMiscellaneaLocation As Integer
        Get
            Return GetInteger("DossierMiscellaneaLocation", -1)
        End Get
    End Property

    Public ReadOnly Property MoveWorkflowDeskToCollaboration As Boolean
        Get
            Return GetBoolean("MoveWorkflowDeskToCollaboration", False)
        End Get
    End Property

    Public ReadOnly Property FasciclesPanelVisibilities As IDictionary(Of String, Boolean)
        Get
            Return GetJson(Of IDictionary(Of String, Boolean))("FasciclesPanelVisibilities", My.Resources.FasciclesPanelVisibilities)
        End Get
    End Property

    Public ReadOnly Property SpidEnabled As Boolean
        Get
            Return GetBoolean("SpidEnabled", False)
        End Get
    End Property

    ''' <summary> Abilita la Ricerca delle PEC in carico nella rubrica Dominio AD. </summary>
    Public ReadOnly Property DomainLookUpEnabled As Boolean
        Get
            Return GetBoolean("DomainLookUpEnabled", True)
        End Get
    End Property

    Public ReadOnly Property FascicleAuthorizedRoleCaption As String
        Get
            Return GetString("FascicleAuthorizedRoleCaption", String.Empty)
        End Get
    End Property

    ''' <summary> Abilita la visibilitࠤei Fascicoli di Attivit஠</summary>
    Public ReadOnly Property ActivityFascicleEnabled As Boolean
        Get
            Return GetBoolean("ActivityFascicleEnabled", True)
        End Get
    End Property

    Public ReadOnly Property ForceDescendingOrderElements As Boolean
        Get
            Return GetBoolean("ForceDescendingOrderElements", False)
        End Get
    End Property

    Public ReadOnly Property DematerialisationEnabled As Boolean
        Get
            Return GetBoolean("DematerialisationEnabled", False) AndAlso Not IsProtocolAttachLocationEnabled
        End Get
    End Property

    Public ReadOnly Property UDSLocation As Integer
        Get
            Return GetInteger("UDSLocation", -1)
        End Get
    End Property

    Public ReadOnly Property SecurityFascicleObject As String
        Get
            Return GetString("SecurityFascicleObject", "Fascicolo riservato con sicurezza aziendale applicata")
        End Get
    End Property

    Public ReadOnly Property IsInvoiceDataResultEnabled As Boolean
        Get
            Return GetBoolean("InvoiceDataResultEnabled", False)
        End Get
    End Property

    Public ReadOnly Property MaxNumberDropdownElements As Integer
        Get
            Return GetInteger("MaxNumberDropdownElements", 10)
        End Get
    End Property

    Public ReadOnly Property ProtocolDefaultAllStatusSearchEnabled As Boolean
        Get
            Return GetBoolean("ProtocolDefaultAllStatusSearchEnabled", False)
        End Get
    End Property


    Public ReadOnly Property MetadataRepositoryEnabled As Boolean
        Get
            Return GetBoolean("MetadataRepositoryEnabled", False)
        End Get
    End Property

    Public ReadOnly Property SecureDocumentEnabled As Boolean
        Get
            Return GetBoolean("SecureDocumentEnabled", False)
        End Get
    End Property

    Public ReadOnly Property PasswordEncryptionKey As String
        Get
            Return GetString("PasswordEncryptionKey", "")
        End Get
    End Property

    Public ReadOnly Property PrivacyManagerGroups As String
        Get
            Return GetString("PrivacyManagerGroups", String.Empty)
        End Get
    End Property

    Public ReadOnly Property ArchiveSecurityGroupsGenerationEnabled As Boolean
        Get
            Return GetBoolean("ArchiveSecurityGroupsGenerationEnabled", True)
        End Get
    End Property

    Public ReadOnly Property SecureDocumentGroups As String
        Get
            Return GetString("SecureDocumentGroups", String.Empty)
        End Get
    End Property

    Public ReadOnly Property SecureDocumentSignatureEnabled As Boolean
        Get
            Return GetBoolean("SecureDocumentSignatureEnabled", False)
        End Get
    End Property

    Public ReadOnly Property MailUploadProtRecipientsEnabled As Boolean
        Get
            Return GetBoolean("MailUploadProtRecipientsEnabled", False)
        End Get
    End Property

    Public ReadOnly Property SecureDocumentVisibilities As ICollection(Of SecureDocumentVisibilityConfiguration)
        Get
            Return GetJson(Of ICollection(Of SecureDocumentVisibilityConfiguration))("SecureDocumentVisibilities", My.Resources.SecureDocumentVisibilities)
        End Get
    End Property

    Public ReadOnly Property PECInsertDocumentSizeEvaluationEnabled As Boolean
        Get
            Return GetBoolean("PECInsertDocumentSizeEvaluationEnabled", True)
        End Get
    End Property


    Public ReadOnly Property PosteWebRequestLocation As Integer
        Get
            Return GetInteger("PosteWebRequestLocation", -1)
        End Get
    End Property

    Public ReadOnly Property FastProtocolSenderIPAContactEnabled As Boolean
        Get
            Return GetBoolean("FastProtocolSenderIPAContactEnabled", False)
        End Get
    End Property

    Public ReadOnly Property ValidationCityCodeEnabled As Boolean
        Get
            Return GetBoolean("ValidationCityCodeEnabled", True)
        End Get
    End Property
    Public ReadOnly Property PrivacyRoleManagerCanEditEnabled As Boolean
        Get
            Return GetBoolean("PrivacyRoleManagerCanEditEnabled", False)
        End Get
    End Property

    Public ReadOnly Property SeriesTitle As String
        Get
            Return GetString("SeriesTitle", "Amministrazione Aperta")
        End Get
    End Property

    Public ReadOnly Property ButtonSeriesTitle As String
        Get
            Return GetString("ButtonSeriesTitle", "Amministrazione aperta")
        End Get
    End Property

    Public ReadOnly Property SecureDocumentWorkflowStatementVisibility As SecureDocumentWorkflowStatementVisibilityConfiguration
        Get
            Return GetJson(Of SecureDocumentWorkflowStatementVisibilityConfiguration)("SecureDocumentWorkflowStatementVisibility", My.Resources.SecureDocumentWorkflowStatementVisibility)
        End Get
    End Property

    Public ReadOnly Property SecurePaperServiceId As Integer
        Get
            Return GetInteger("SecurePaperServiceId", -1)
        End Get
    End Property

    Public ReadOnly Property SecurePaperCertificateThumbprint As String
        Get
            Return GetString("SecurePaperCertificateThumbprint", String.Empty)
        End Get
    End Property

    Public ReadOnly Property SecurePaperServiceUrl As String
        Get
            Return GetString("SecurePaperServiceUrl", String.Empty)
        End Get
    End Property

    Public ReadOnly Property ProtocolDocumentSeriesButtonEnable As Boolean
        Get
            Return GetBoolean("ProtocolDocumentSeriesButtonEnable", False)
        End Get
    End Property

    Public ReadOnly Property PrivacyTypology As PrivacyType?
        Get
            Dim value As Integer? = GetInteger("PrivacyTypology", Nothing)
            If Not value.HasValue Then
                Return Nothing
            End If

            If [Enum].IsDefined(GetType(PrivacyType), value) Then
                Return DirectCast(value.Value, PrivacyType)
            End If
        End Get
    End Property

    Public ReadOnly Property SearchOnlyAuthorizedFasciclesEnabled As Boolean
        Get
            Return GetBoolean("SearchOnlyAuthorizedFasciclesEnabled", False)
        End Get
    End Property

    Public ReadOnly Property PECAttachmentsZipEnabled As Boolean
        Get
            Return GetBoolean("PECAttachmentsZipEnabled", False)
        End Get
    End Property

    Public ReadOnly Property LogConservationLocation As Integer
        Get
            Return GetInteger("LogConservationLocation", -1)
        End Get
    End Property

    Public ReadOnly Property AmministrazioneTrasparenteProtocolEnabled As Boolean
        Get
            Return GetBoolean("AmministrazioneTrasparenteProtocolEnabled", False)
        End Get
    End Property

    Public ReadOnly Property ScannerLightRestEnabled As Boolean
        Get
            Return GetBoolean("ScannerLightRestEnabled", False)
        End Get
    End Property

    Public ReadOnly Property ShowFiscalCodeInFascicleSummary As Boolean
        Get
            Return GetBoolean("ShowFiscalCodeInFascicleSummary", True)
        End Get
    End Property

    Public ReadOnly Property UDSLogShowOnlyCurrentIfNotAdmin As Boolean
        Get
            Return GetBoolean("UDSLogShowOnlyCurrentIfNotAdmin", False)
        End Get
    End Property

    Public ReadOnly Property MonitoringTransparentRatings As List(Of String)
        Get
            Return GetJson(Of List(Of String))("MonitoringTransparentRatings", My.Resources.MonitoringTransparentRatings)
        End Get
    End Property

    Public ReadOnly Property TransparentManagerGroups As String
        Get
            Return GetString("TransparentManagerGroups", String.Empty)
        End Get
    End Property

    Public ReadOnly Property PECPasswordZipEnabled As Boolean
        Get
            Return GetBoolean("PECPasswordZipEnabled", False)
        End Get
    End Property
    Public ReadOnly Property EmailCompressEnabled As Boolean
        Get
            Return GetBoolean("EmailCompressEnabled", True)
        End Get
    End Property

    Public ReadOnly Property ProtocolDistributionTypologies As List(Of Integer)
        Get
            Return GetJson(Of List(Of Integer))("ProtocolDistributionTypologies", My.Resources.ProtocolDistributionTypologies)
        End Get
    End Property

    Public ReadOnly Property ShowVisibilityTypeInFascicleSearch As Boolean
        Get
            Return GetBoolean("ShowVisibilityTypeInFascicleSearch", True)
        End Get
    End Property
    Public ReadOnly Property TransparentMonitoringEnabled As Boolean
        Get
            'Return DocumentSeriesEnabled AndAlso GetBoolean("TransparentMonitoringEnabled", False)
            Return True
        End Get
    End Property

    Public ReadOnly Property ManageDisableItemsEnabled As Boolean
        Get
            Return GetBoolean("ManageDisableItemsEnabled", False)
        End Get
    End Property

    Public ReadOnly Property SeriesItemOwnerRoleRequired As Boolean
        Get
            Return GetBoolean("SeriesItemOwnerRoleRequired", False)
        End Get
    End Property

    Public ReadOnly Property BrowseDocumentHistoryEnabled As Boolean
        Get
            Return GetBoolean("BrowseDocumentHistoryEnabled", False)
        End Get
    End Property
    Public ReadOnly Property PrimoPianoDefaultEnabled As Boolean
        Get
            Return GetBoolean("PrimoPianoDefaultEnabled", False)
        End Get
    End Property

    Public ReadOnly Property SeriesPublishingDateFromResolutionEnabled As Boolean
        Get
            Return GetBoolean("SeriesPublishingDateFromResolutionEnabled", False)
        End Get
    End Property

    Public ReadOnly Property UDSInvoiceTypology As String
        Get
            Return GetString("UDSInvoiceTypology", String.Empty)
        End Get
    End Property

    Public ReadOnly Property InvoiceSDIGroup As String
        Get
            Return GetString("InvoiceSDIGroup", String.Empty)
        End Get
    End Property

    Public ReadOnly Property MultiTenantEnabled As Boolean
        Get
            Return GetBoolean("MultiTenantEnabled", False)
        End Get
    End Property

    Public ReadOnly Property ShowProtocolCategoryFullCode As Boolean
        Get
            Return GetBoolean("ShowProtocolCategoryFullCode", True)
        End Get
    End Property
    Public ReadOnly Property ShowUDSChainsInProtocolViewer As Boolean
        Get
            Return GetBoolean("ShowUDSChainsInProtocolViewer", False)
        End Get
    End Property

    Public ReadOnly Property ValidateEmailAdressesPattern As String
        Get
            Return GetString("ValidateEmailAdressesPattern", "^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$")
        End Get
    End Property

    Public ReadOnly Property FascicleContainerEnabled As Boolean
        Get
            Return FascicleEnabled AndAlso GetBoolean("FascicleContainerEnabled", False)
        End Get
    End Property

    Public ReadOnly Property PECGridCheckActiveDocumentsEnabled() As Boolean
        Get
            Return GetBoolean("PECGridCheckActiveDocumentsEnabled", False)
        End Get
    End Property

    Public ReadOnly Property WorkflowLocation As Integer
        Get
            Return GetInteger("WorkflowLocation", -1)
        End Get
    End Property

    Public ReadOnly Property RemoteSignEnabled As Boolean
        Get
            Return GetBoolean("RemoteSignEnabled", False)
        End Get
    End Property

    Public ReadOnly Property ProtocolGridOrderColumns As IDictionary(Of String, Integer)
        Get
            Return GetJson(Of IDictionary(Of String, Integer))("ProtocolGridOrderColumns", String.Empty)
        End Get
    End Property

    Public ReadOnly Property DefaultStatusUserWorkflowFilter As Integer
        Get
            Return GetInteger("DefaultStatusUserWorkflowFilter", 1)
        End Get
    End Property


    Public ReadOnly Property ZenDeskUrl As String
        Get
            Return GetString("ZenDeskUrl", "https://ged.zendesk.com")
        End Get
    End Property

    Public ReadOnly Property ZenDeskEmail As String
        Get
            Return GetString("ZenDeskEmail", "fabrizio.lazzarotto@vecompsoftware.it")
        End Get
    End Property

    Public ReadOnly Property ZenDeskToken As String
        Get
            Return GetString("ZenDeskToken", "EyPRJhOMk2QE1TAZvmMQ8xia3bEvePVRmUE6XUnl")
        End Get
    End Property

    Public ReadOnly Property CurrentUserEMailSenderEnabled As Boolean
        Get
            Return GetBoolean("CurrentUserEMailSenderEnabled", True)
        End Get
    End Property

    Public ReadOnly Property ProtocolDistributionViewDefaultFilter As Integer
        Get
            Return GetInteger("ProtocolDistributionViewDefaultFilter", 1)
        End Get
    End Property

    Public ReadOnly Property WorkflowActivityImages As IDictionary(Of String, String)
        Get
            Return GetJson(Of IDictionary(Of String, String))("WorkflowActivityImages", My.Resources.WorkflowActivityImages)
        End Get
    End Property


    Public ReadOnly Property SectorNoteDistributionEnabled As Boolean
        Get
            Return GetBoolean("SectorNoteDistributionEnabled", False)
        End Get
    End Property
    Public ReadOnly Property DocumentUnitLabels As IDictionary(Of Model.Entities.DocumentUnits.ChainType, String)
        Get
            Return GetJson(Of IDictionary(Of Model.Entities.DocumentUnits.ChainType, String))("DocumentUnitDocumentsLabel", My.Resources.DocumentUnitDocumentsLabel)
        End Get
    End Property

    Public ReadOnly Property ContactSmartEnabled As Boolean
        Get
            Return GetBoolean("ContactSmartEnabled", False)
        End Get
    End Property

    Public ReadOnly Property FascicleRoleLabels As IDictionary(Of String, String)
        Get
            Return GetJson(Of IDictionary(Of String, String))("FascicleRoleLabels", My.Resources.FascicleRoleLabels)
        End Get
    End Property

    Public ReadOnly Property FascicleRoleRPLabel As String
        Get
            Return If(FascicleRoleLabels.ContainsKey("ProdecureRoleName"), FascicleRoleLabels("ProdecureRoleName"), "Responsabile di procedimento")
        End Get
    End Property

    Public ReadOnly Property FascicleRoleSPLabel As String
        Get
            Return If(FascicleRoleLabels.ContainsKey("SecretaryRoleName"), FascicleRoleLabels("SecretaryRoleName"), "Segreteria di procedimento")
        End Get
    End Property

    Public ReadOnly Property RoleGroupPECRightOutgoingEnabled As Boolean
        Get
            Return GetBoolean("RoleGroupPECRightOutgoingEnabled", True)
        End Get
    End Property

    Public ReadOnly Property ProtocolStatusConfirmRequest As IList(Of String)
        Get
            Return GetJson(Of IList(Of String))("ProtocolStatusConfirmRequest", "[]")
        End Get
    End Property

    Public ReadOnly Property ProcessContainer As Integer
        Get
            Return GetInteger("ProcessContainer", -1)
        End Get
    End Property

    Public ReadOnly Property ProcessRole As Integer
        Get
            Return GetInteger("ProcessRole", -1)
        End Get
    End Property

    Public ReadOnly Property ProcessEnabled As Boolean
        Get
            Return GetBoolean("ProcessEnabled", False)
        End Get
    End Property

    Public ReadOnly Property ShowCollaborationSignDate As Boolean
        Get
            Return GetBoolean("ShowCollaborationSignDate", False)
        End Get
    End Property

    Public ReadOnly Property DescendingCollaborationOrder As Boolean
        Get
            Return GetBoolean("DescendingCollaborationOrder", False)
        End Get
    End Property

    Public ReadOnly Property ForceDeleteCollaborationEnabled As Boolean
        Get
            Return GetBoolean("ForceDeleteCollaborationEnabled", False)
        End Get
    End Property
    Public ReadOnly Property CheckResolutionCollaborationOriginEnabled As Boolean
        Get
            Return GetBoolean("CheckResolutionCollaborationOriginEnabled", False)
        End Get
    End Property

    Public ReadOnly Property EnabledEmailAttachmentPages As IDictionary(Of String, Boolean)
        Get
            Return GetJson(Of IDictionary(Of String, Boolean))("EnabledEmailAttachmentPages", String.Empty)
        End Get
    End Property
    Public ReadOnly Property SearchProtocolRblDefaultValue As IDictionary(Of String, String)
        Get
            Return GetJson(Of IDictionary(Of String, String))("SearchProtocolRblDefaultValue", My.Resources.SearchProtocolRblDefaultValue)
        End Get
    End Property
#End Region

#Region " Methods "

    Function IsPosteWebEnabled() As Boolean
        Return IsRaccomandataEnabled() OrElse IsLetteraEnabled() OrElse IsTelgrammaEnabled()
    End Function

    Function IsProtocolRecoverEnabled() As Boolean
        Return IsRecoverEnabled() OrElse IsRedoEnabled() OrElse IsRepairEnabled()
    End Function

    Function IsInvoiceDataGridResultEnabled() As Boolean
        Return IsInvoiceEnabled AndAlso IsInvoiceDataResultEnabled
    End Function
    Function PECMailBoxPasswordEncriptionEnabled() As Boolean
        Return Not String.IsNullOrEmpty(PasswordEncryptionKey) AndAlso PasswordEncryptionKey.Length = 32
    End Function
#End Region

End Class