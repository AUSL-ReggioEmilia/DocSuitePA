Imports System.Collections.Concurrent
Imports System.Collections.Specialized
Imports System.IO
Imports System.Linq
Imports System.Runtime.Caching
Imports System.Text
Imports System.Web
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Logging

Public Class CommonShared

#Region " Fields "

    Private Shared _workflowLocation As Location = Nothing
    Public Const THIN_CLIENT_X_SMARTCLIENT_VERSION As String = "40"

    Protected Const USER_CONNECTED_GROUPS_FIELD As String = "UserConnectedGroups"
    Protected Const USER_CONNECTED_GROUPS_SG_FIELD As String = "UserConnectedGroupsSG"
    Protected Const USER_CONNECTED_GROUPS_NAME_SG_FIELD As String = "UserConnectedGroupsNameSG"
    Protected Const USER_FULLNAME_FIELD As String = "UserFullName"
    Protected Const USER_SESSIONID_FIELD As String = "SessionId"
    Protected Const USER_DESCRIPTION_FIELD As String = "UserDescription"
    Protected Const USER_MAIL_FIELD As String = "UserMail"
    Protected Const USER_COMPUTER_FIELD As String = "UserComputer"
    Protected Const USER_GROUP_PROTOCOL_MANAGER_SELECTED As String = "GroupProtocolManagerSelected"
    Protected Const USER_GROUP_PROTOCOL_NOT_MANAGER_SELECTED As String = "GroupProtocolNotManagerSelected"
    Protected Const USER_GROUP_PAPERWORK_SELECTED As String = "GroupPaperworkSelected"
    Protected Const USER_GROUP_RESOLUTION_SELECTED As String = "GroupResolutionSelected"


    Protected Const USER_CONTAINER_RIGHT_DICTIONARY As String = "User.ContainerRightDictionary"
    Protected Const USER_ROLE_RIGHT_DICTIONARY As String = "User.RoleRightDictionary"
    Protected Const USER_DOCUMENTSERIES_CONTAINER_RIGHT_DICTIONARY As String = "User.DocumentSeriesContainerRightDictionary"
    Protected Const USER_PROTOCOL_CONTAINER_RIGHT_DICTIONARY As String = "User.ProtocolContainerRightDictionary"
    Protected Const USER_UDS_CONTAINER_RIGHT_DICTIONARY As String = "User.UDSContainerRightDictionary"
    Protected Const USER_DOCUMENT_CONTAINER_RIGHT_DICTIONARY As String = "User.DocumentContainerRightDictionary"
    Protected Const USER_RESOLUTION_CONTAINER_RIGHT_DICTIONARY As String = "User.ResolutionRightDictionary"

    Private Const ZebraSessionKeyName As String = "ZebraProtocolKeysToPrint"
    Private Const SharedFileSessionKeyName As String = "SharedFile"
    Private Const SharedSelectedMailBox As String = "PEC_SelectedPecMailBox"

    ''' <summary> Dizionario generale che mantiene tutti i valori del contesto in mancanza di sessione. </summary>
    Private Shared _processContext As ConcurrentDictionary(Of String, Object) = New ConcurrentDictionary(Of String, Object)

#End Region

#Region " Properties "

    Public Shared ReadOnly Property EffectiveOChart As OChart
        Get
            Return FacadeFactory.Instance.OChartFacade.GetEffective()
        End Get
    End Property

    Public Shared Sub ClearRightDictionaries()
        SetContextValue(USER_CONTAINER_RIGHT_DICTIONARY, Nothing)
        SetContextValue(USER_ROLE_RIGHT_DICTIONARY, Nothing)
        SetContextValue(USER_DOCUMENTSERIES_CONTAINER_RIGHT_DICTIONARY, Nothing)
        SetContextValue(USER_PROTOCOL_CONTAINER_RIGHT_DICTIONARY, Nothing)
        SetContextValue(USER_UDS_CONTAINER_RIGHT_DICTIONARY, Nothing)
        SetContextValue(USER_DOCUMENT_CONTAINER_RIGHT_DICTIONARY, Nothing)
        SetContextValue(USER_RESOLUTION_CONTAINER_RIGHT_DICTIONARY, Nothing)
    End Sub

    Public Shared ReadOnly Property UserContainerRightDictionary() As Dictionary(Of ProtocolContainerRightPositions, IList(Of Integer))
        Get
            If GetContextValue(USER_CONTAINER_RIGHT_DICTIONARY) Is Nothing Then
                Dim dic As Dictionary(Of ProtocolContainerRightPositions, IList(Of Integer)) = (New ContainerFacade).GetCurrentUserContainers(DSWEnvironment.Protocol)
                SetContextValue(USER_CONTAINER_RIGHT_DICTIONARY, dic)
            End If
            Return CType(GetContextValue(USER_CONTAINER_RIGHT_DICTIONARY), Dictionary(Of ProtocolContainerRightPositions, IList(Of Integer)))
        End Get
    End Property

    Public Shared ReadOnly Property UserRoleRightDictionary() As Dictionary(Of ProtocolRoleRightPositions, IList(Of Integer))
        Get
            If GetContextValue(USER_ROLE_RIGHT_DICTIONARY) Is Nothing Then
                Dim dic As Dictionary(Of ProtocolRoleRightPositions, IList(Of Integer)) = (New RoleFacade).GetCurrentUserRoles(DSWEnvironment.Protocol)
                SetContextValue(USER_ROLE_RIGHT_DICTIONARY, dic)
            End If
            Return CType(GetContextValue(USER_ROLE_RIGHT_DICTIONARY), Dictionary(Of ProtocolRoleRightPositions, IList(Of Integer)))
        End Get
    End Property

    ''' <summary>Ricerca i gruppi appartenenti ad un utente</summary>
    ''' <remarks>User Environment: Users Groups</remarks>
    Public Shared ReadOnly Property UserConnectedGroups() As String
        Get
            Return String.Empty
        End Get
    End Property

    Shared Property UserFullName() As String
        Get
            ' TODO: spostare in DocSuiteUser
            Dim user As String = GetContextValue(USER_FULLNAME_FIELD)

            If String.IsNullOrEmpty(user) Then
                user = HttpContext.Current.User.Identity.Name
                SetContextValue(USER_FULLNAME_FIELD, user)
            End If

            Return user
        End Get
        Set(ByVal value As String)
            SetContextValue(USER_FULLNAME_FIELD, value)
        End Set
    End Property


    Shared Property UserSessionId() As String
        Get
            Return GetContextValue(USER_SESSIONID_FIELD)
        End Get
        Set(ByVal value As String)
            SetContextValue(USER_SESSIONID_FIELD, value)
        End Set
    End Property

    Shared Property DSUserDescription() As String
        Get
            Return GetContextValue(USER_DESCRIPTION_FIELD)
        End Get
        Set(ByVal value As String)
            SetContextValue(USER_DESCRIPTION_FIELD, value)
        End Set
    End Property

    Shared Property DsUserMail() As String
        Get
            Return CType(GetContextValue(USER_MAIL_FIELD), String)
        End Get
        Set(ByVal value As String)
            SetContextValue(USER_MAIL_FIELD, value)
        End Set
    End Property

    ''' <summary> Computer del client. </summary>
    Shared Property DSUserComputer() As String
        Get
            Return CType(GetContextValue(USER_COMPUTER_FIELD), String)
        End Get
        Set(ByVal value As String)
            SetContextValue(USER_COMPUTER_FIELD, value)
        End Set
    End Property

    Public Shared Property AdvancedViewer() As Integer
        Get
            If Not String.IsNullOrEmpty(DocSuiteContext.Current.ProtocolEnv.ForceViewer) Then
                Return Integer.Parse(DocSuiteContext.Current.ProtocolEnv.ForceViewer)
            End If
            Dim facade As New ComputerLogFacade()
            Dim computerLog As ComputerLog = facade.GetCurrent()
            If computerLog IsNot Nothing Then
                Return computerLog.AdvancedViewer

            End If

            Dim finder As NHibernateUserLogFinder = New NHibernateUserLogFinder()
            finder.SystemUser = DocSuiteContext.Current.User.FullUserName
            Dim logs As IList(Of UserLog) = finder.DoSearch()
            If Not logs Is Nothing And logs.Count = 0 Then
                Return 0
            End If
            Return logs(0).AdvancedViewer
        End Get
        Set(ByVal value As Integer)
            Dim facade As New ComputerLogFacade()
            Dim item As ComputerLog = facade.GetCurrent()
            item.AdvancedViewer = value
            Dim computerLogFacade As ComputerLogFacade = New ComputerLogFacade()
            computerLogFacade.Update(item)
            Dim _finder As NHibernateUserLogFinder = New NHibernateUserLogFinder()
            _finder.SystemUser = DocSuiteContext.Current.User.FullUserName
            Dim logs As IList(Of UserLog) = _finder.DoSearch()
            If logs Is Nothing OrElse logs.Count <> 0 Then
                ' Salvo il valore impostato
                Dim userLog As UserLog = logs.First()
                userLog.AdvancedViewer = value
                Dim userLogFacade As UserLogFacade = New UserLogFacade()
                userLogFacade.Update(userLog)
            End If
        End Set
    End Property

    Public Shared Property GroupProtocolManagerSelected As String
        Get
            Return CType(GetContextValue(USER_GROUP_PROTOCOL_MANAGER_SELECTED, True), String)
        End Get
        Set(value As String)
            SetContextValue(USER_GROUP_PROTOCOL_MANAGER_SELECTED, value, True, True)
        End Set
    End Property

    Public Shared Property GroupProtocolNotManagerSelected As String
        Get
            Return CType(GetContextValue(USER_GROUP_PROTOCOL_NOT_MANAGER_SELECTED, True), String)
        End Get
        Set(value As String)
            SetContextValue(USER_GROUP_PROTOCOL_NOT_MANAGER_SELECTED, value, True, True)
        End Set
    End Property

    Public Shared Property GroupPaperworkSelected As String
        Get
            Return CType(GetContextValue(USER_GROUP_PAPERWORK_SELECTED, True), String)
        End Get
        Set(value As String)
            SetContextValue(USER_GROUP_PAPERWORK_SELECTED, value, True, True)
        End Set
    End Property

    Public Shared Property GroupResolutionSelected As String
        Get
            Return CType(GetContextValue(USER_GROUP_RESOLUTION_SELECTED, True), String)
        End Get
        Set(value As String)
            SetContextValue(USER_GROUP_RESOLUTION_SELECTED, value, True, True)
        End Set
    End Property

    ''' <summary>Nome del computer server</summary>
    Shared ReadOnly Property MachineName() As String
        Get
            If HttpContext.Current IsNot Nothing Then
                Return HttpContext.Current.Server.MachineName
            Else
                Return String.Empty
            End If
        End Get
    End Property

    Public Shared ReadOnly Property UserDomain() As String
        Get
            Return DocSuiteContext.Current.User.Domain
        End Get
    End Property

    Public Shared ReadOnly Property UserDocumentName() As String
        Get
            Return String.Concat(DocSuiteContext.Current.User.UserName, "-", UserSessionId)
        End Get
    End Property

    Public Shared ReadOnly Property HasGroupStatisticsRight As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.EnvGroupStatistics)
        End Get
    End Property

    Public Shared ReadOnly Property HasGroupStatisticsReslolutionRight As Boolean
        Get
            Return DocSuiteContext.Current.IsResolutionEnabled AndAlso UserConnectedBelongsTo(DocSuiteContext.Current.ResolutionEnv.EnvGroupStatistics)
        End Get
    End Property

    Public Shared ReadOnly Property HasGroupSuspendRight As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.EnvGroupSuspend)
        End Get
    End Property

    ''' <summary> Indica se l'utente corrente appartiene al gruppo che può editare i contatti. </summary>
    Public Shared ReadOnly Property HasGroupTblContactRight As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.EnvGroupTblContact)
        End Get
    End Property

    Public Shared ReadOnly Property HasGroupTblCategoryRight As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.EnvGroupTblCategory)
        End Get
    End Property

    Public Shared ReadOnly Property HasGroupTblContainerRight As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.EnvGroupTblContainer)
        End Get
    End Property

    Public Shared ReadOnly Property HasGroupTblRoleRight As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.EnvGroupTblRole)
        End Get
    End Property

    Public Shared ReadOnly Property HasGroupProposerFullRight As Boolean
        Get
            Return DocSuiteContext.Current.IsResolutionEnabled AndAlso UserConnectedBelongsTo(DocSuiteContext.Current.ResolutionEnv.EnvGroupProposerFull)
        End Get
    End Property

    Public Shared ReadOnly Property HasGroupLogViewRight As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.EnvGroupLogView)
        End Get
    End Property

    Public Shared ReadOnly Property HasGroupConcourseRight As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.EnvGroupConcourse)
        End Get
    End Property

    Shared ReadOnly Property HasWordGroupImportRight As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.WordGroupImport)
        End Get
    End Property

    Shared ReadOnly Property HasExcelGroupImportRight As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.ExcelGroupImport)
        End Get
    End Property

    Public Shared ReadOnly Property HasGroupEditCategoryRight As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.GroupEditCategory)
        End Get
    End Property

    Public Shared ReadOnly Property HasGroupAdministratorRight As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.EnvGroupAdministrator)
        End Get
    End Property

    Public Shared ReadOnly Property HasGroupDigitalLastPageRight As Boolean
        Get
            Return DocSuiteContext.Current.IsResolutionEnabled AndAlso DocSuiteContext.Current.ResolutionEnv.DigitalLastPage AndAlso UserConnectedBelongsTo(DocSuiteContext.Current.ResolutionEnv.DigitalLastPageGroup)
        End Get
    End Property

    ''' <summary> Verifica se l'operatore corrente è abilitato a vedere le funzionalità di Registro Atti. </summary>
    ''' <returns>True se appartiene al gruppo ResolutionEnv.ResolutionJournalGroup</returns>
    Public Shared ReadOnly Property ResolutionJournalEnabled As Boolean
        Get
            Return DocSuiteContext.Current.IsResolutionEnabled AndAlso UserConnectedBelongsTo(DocSuiteContext.Current.ResolutionEnv.ResolutionJournalGroup)
        End Get
    End Property

    Public Shared ReadOnly Property ResolutionPublicationJournalEnabled As Boolean
        Get
            Return DocSuiteContext.Current.IsResolutionEnabled AndAlso UserConnectedBelongsTo(DocSuiteContext.Current.ResolutionEnv.ReslPubJournalGroup)
        End Get
    End Property

    ''' <summary> Verifica se l'operatore corrente è abilitato a vedere le funzionalità di firma del ritiro degli Atti. </summary>
    ''' <returns>True se appartiene al gruppo ResolutionEnv.RitiraAttiGroups</returns>
    Public Shared ReadOnly Property RitiraAttiEnabled As Boolean
        Get
            Return DocSuiteContext.Current.IsResolutionEnabled AndAlso UserConnectedBelongsTo(DocSuiteContext.Current.ResolutionEnv.RitiraAttiGroups)
        End Get
    End Property

    ''' <summary> Verifica se è abilitata la funzionalità di PEC agli Organi di Controllo. </summary>
    ''' <remarks>
    ''' Funzionalità pensata per spedire PEC al Collegio Sindacale del cliente AUSL Torino
    ''' pec abilitata
    ''' utente nel gruppo
    ''' mailbox presente
    ''' </remarks>
    Public Shared ReadOnly Property ResolutionPECOCEnabled As Boolean
        Get
            Return DocSuiteContext.Current.ProtocolEnv.IsPECEnabled AndAlso
                DocSuiteContext.Current.ResolutionEnv.MailBoxCollegioSindacale IsNot Nothing AndAlso
                 UserConnectedBelongsTo(DocSuiteContext.Current.ResolutionEnv.GruppiCollegioSindacale)
        End Get
    End Property

    Shared ReadOnly Property HasInvoiceGroupImportRight As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.InvoiceGroupImport)
        End Get
    End Property

    Shared ReadOnly Property HasGroupExtractionRight As Boolean
        Get
            Return DocSuiteContext.Current.IsResolutionEnabled AndAlso UserConnectedBelongsTo(DocSuiteContext.Current.ResolutionEnv.EnvGroupExtraction)
        End Get
    End Property

    Shared ReadOnly Property HasGroupJournalRight As Boolean
        Get
            If (DocSuiteContext.Current.ProtocolEnv.IsJournalEnabled) Then
                Dim gruppi As String = DocSuiteContext.Current.ProtocolEnv.JournalEnabledGroups
                ''Dev'essere attivo il registro giornaliero e (o i gruppi vuoti oppure l'utente attivo presente nel gruppo)
                Return DocSuiteContext.Current.ProtocolEnv.IsJournalEnabled AndAlso UserConnectedBelongsTo(gruppi, True)
            End If
            Return False
        End Get
    End Property

    Shared ReadOnly Property HasGroupPECFixedRight As Boolean
        Get
            Dim gruppi As String = DocSuiteContext.Current.ProtocolEnv.PECFixedGroups
            Return UserConnectedBelongsTo(gruppi)
        End Get
    End Property


    Shared ReadOnly Property HasGroupPecMailLogViewRight As Boolean
        Get
            Dim gruppi As String = DocSuiteContext.Current.ProtocolEnv.PecMailLogViewVisibleGroups
            Return UserConnectedBelongsTo(gruppi, True)
        End Get
    End Property

    ''' <summary> Indica se l'operatore corrente può correggere protocolli </summary>
    Public Shared ReadOnly Property HasGroupProtocolCorrectionRight As Boolean
        Get
            Return Not String.IsNullOrWhiteSpace(DocSuiteContext.Current.ProtocolEnv.ProtCorrectionGroups) AndAlso UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.ProtCorrectionGroups)
        End Get
    End Property

    Shared ReadOnly Property HasGroupTblStampeRight As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.EnvGroupTblStampe)
        End Get
    End Property

    Shared ReadOnly Property HasGroupTblStampeSecurityRight As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.EnvGroupTblStampeSecurity)
        End Get
    End Property

    Shared ReadOnly Property HasGroupTblRoleAdminRight As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.EnvGroupTblRoleAdmin)
        End Get
    End Property

    Shared ReadOnly Property HasGroupTblContainerAdminRight As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.EnvGroupTblContainerAdmin)
        End Get
    End Property

    Shared ReadOnly Property HasGroupOriginalEmlRight As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.EnvGroupOriginalEml)
        End Get
    End Property

    Shared ReadOnly Property PosteWebEnabled As Boolean
        Get
            Return DocSuiteContext.Current.ProtocolEnv.IsPosteWebEnabled
        End Get
    End Property

    Shared ReadOnly Property HasPosteWebReportRight As Boolean
        Get
            Return PosteWebEnabled AndAlso UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.GroupPosteWebReport)
        End Get
    End Property

    ''' <summary> Protocolli da stampare in Zebra </summary>
    ''' <remarks> Serializzati ad cazzum </remarks>
    Public Shared Property ZebraPrintData As ICollection(Of Guid)
        Get
            Dim datas As ICollection(Of Guid) = TryCast(GetContextValue(ZebraSessionKeyName), ICollection(Of Guid))
            Return datas
        End Get
        Set(ByVal value As ICollection(Of Guid))
            SetContextValue(ZebraSessionKeyName, value)
        End Set
    End Property

    Public Shared Property SelectedSharedFile() As FileInfo
        Get
            Return CType(GetContextValue(SharedFileSessionKeyName), FileInfo)
        End Get
        Set(value As FileInfo)
            SetContextValue(SharedFileSessionKeyName, value)
        End Set
    End Property

    Public Shared Property SelectedPecMailBoxId() As Short?
        Get
            Return DirectCast(GetContextValue(SharedSelectedMailBox), Short?)
        End Get
        Set(value As Short?)
            SetContextValue(SharedSelectedMailBox, value)
        End Set
    End Property

    Shared ReadOnly Property HasGroupTblRoleTypeResolutionRight As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ResolutionEnv.EnvGroupTblRoleTypeResl)
        End Get
    End Property
    Public Shared ReadOnly Property HasRefusedProtocolGroupsRight As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.RefusedProtocolsGroups)
        End Get
    End Property
    Public Shared ReadOnly Property HasGroupsWithSearchProtocolRoleRestrictionNoneRight As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.GroupsWithSearchProtocolRoleRestrictionNone)
        End Get
    End Property
    Public Shared ReadOnly Property HasSecurityGroupAdminRight As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.SecurityGroupAdmin)
        End Get
    End Property
    Public Shared ReadOnly Property HasSecurityGroupPowerUserRight As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.SecurityGroupPowerUser)
        End Get
    End Property

    Public Shared ReadOnly Property HasHiddenSecurityGroupForNotAdminsRight As List(Of String)
        Get
            Return DocSuiteContext.Current.ProtocolEnv.HiddenSecurityGroupForNotAdmins.Split({"|"c, ","c}, StringSplitOptions.RemoveEmptyEntries).ToList()
        End Get
    End Property

    Public Shared ReadOnly Property HasGroupsWithPosteWebAccountRestrictionNoneRight() As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.GroupsWithPosteWebAccountRestrictionNone)
        End Get
    End Property

    Shared ReadOnly Property HasGroupTblPrivacyLevelManagerGroupRight As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.PrivacyManagerGroups)
        End Get
    End Property

    Private Shared ReadOnly Property UserDocumentSeriesActiveContainerRightDictionary() As Dictionary(Of DocumentSeriesContainerRightPositions, IList(Of Integer))
        Get
            If GetContextValue(USER_DOCUMENTSERIES_CONTAINER_RIGHT_DICTIONARY) Is Nothing Then
                Dim dic As Dictionary(Of DocumentSeriesContainerRightPositions, IList(Of Integer)) = (New ContainerFacade).GetCurrentUserDocumentSeriesContainers(DSWEnvironment.DocumentSeries, True)
                SetContextValue(USER_DOCUMENTSERIES_CONTAINER_RIGHT_DICTIONARY, dic)
            End If
            Return CType(GetContextValue(USER_DOCUMENTSERIES_CONTAINER_RIGHT_DICTIONARY), Dictionary(Of DocumentSeriesContainerRightPositions, IList(Of Integer)))
        End Get
    End Property

    Private Shared ReadOnly Property UserProtocolActiveContainerRightDictionary() As Dictionary(Of ProtocolContainerRightPositions, IList(Of Integer))
        Get
            If GetContextValue(USER_PROTOCOL_CONTAINER_RIGHT_DICTIONARY) Is Nothing Then
                Dim dic As Dictionary(Of ProtocolContainerRightPositions, IList(Of Integer)) = (New ContainerFacade).GetCurrentUserProtocolContainers(DSWEnvironment.Protocol, True)
                SetContextValue(USER_PROTOCOL_CONTAINER_RIGHT_DICTIONARY, dic)
            End If
            Return CType(GetContextValue(USER_PROTOCOL_CONTAINER_RIGHT_DICTIONARY), Dictionary(Of ProtocolContainerRightPositions, IList(Of Integer)))
        End Get
    End Property

    Private Shared ReadOnly Property UserUDSActiveContainerRightDictionary() As Dictionary(Of ProtocolContainerRightPositions, IList(Of Integer))
        Get
            If GetContextValue(USER_UDS_CONTAINER_RIGHT_DICTIONARY) Is Nothing Then
                Dim dic As Dictionary(Of ProtocolContainerRightPositions, IList(Of Integer)) = (New ContainerFacade).GetCurrentUserProtocolContainers(DSWEnvironment.UDS, True)
                SetContextValue(USER_UDS_CONTAINER_RIGHT_DICTIONARY, dic)
            End If
            Return CType(GetContextValue(USER_UDS_CONTAINER_RIGHT_DICTIONARY), Dictionary(Of ProtocolContainerRightPositions, IList(Of Integer)))
        End Get
    End Property


    Private Shared ReadOnly Property UserDocumentActiveContainerRightDictionary() As Dictionary(Of DocumentContainerRightPositions, IList(Of Integer))
        Get
            If GetContextValue(USER_DOCUMENT_CONTAINER_RIGHT_DICTIONARY) Is Nothing Then
                Dim dic As Dictionary(Of DocumentContainerRightPositions, IList(Of Integer)) = (New ContainerFacade).GetCurrentUserDocumentContainers(DSWEnvironment.Document, True)
                SetContextValue(USER_DOCUMENT_CONTAINER_RIGHT_DICTIONARY, dic)
            End If
            Return CType(GetContextValue(USER_DOCUMENT_CONTAINER_RIGHT_DICTIONARY), Dictionary(Of DocumentContainerRightPositions, IList(Of Integer)))
        End Get
    End Property

    Private Shared ReadOnly Property UserResolutionActiveContainerRightDictionary() As Dictionary(Of ResolutionRightPositions, IList(Of Integer))
        Get
            If GetContextValue(USER_RESOLUTION_CONTAINER_RIGHT_DICTIONARY) Is Nothing Then
                Dim dic As Dictionary(Of ResolutionRightPositions, IList(Of Integer)) = (New ContainerFacade).GetCurrentUserReolutionContainers(DSWEnvironment.Resolution, True)
                SetContextValue(USER_RESOLUTION_CONTAINER_RIGHT_DICTIONARY, dic)
            End If
            Return CType(GetContextValue(USER_RESOLUTION_CONTAINER_RIGHT_DICTIONARY), Dictionary(Of ResolutionRightPositions, IList(Of Integer)))
        End Get
    End Property

    Public Shared ReadOnly Property HasGroupTransparentManagerRight As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.TransparentManagerGroups)
        End Get
    End Property
    Public Shared ReadOnly Property HasGroupProcessesViewsReadableRight As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.ProcessesViewsReadableGroups)
        End Get
    End Property
    Public Shared ReadOnly Property HasGroupProcessesViewsManageableRight As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.ProcessesViewsManageableGroups)
        End Get
    End Property

    Public Shared ReadOnly Property HasGroupInvoiceSDIRight As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.InvoiceSDIGroup)
        End Get
    End Property

    Public Shared ReadOnly Property HasGroupPECFisicalDeleteRight As Boolean
        Get
            Return UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.PECFisicalDeleteGroups)
        End Get
    End Property

    Public Shared ReadOnly Property CurrentWorkflowLocation As Location
        Get
            If _workflowLocation Is Nothing AndAlso DocSuiteContext.Current.ProtocolEnv.WorkflowLocation.HasValue Then
                _workflowLocation = FacadeFactory.Instance.LocationFacade.GetById(DocSuiteContext.Current.ProtocolEnv.WorkflowLocation.Value)
            End If
            Return _workflowLocation
        End Get
    End Property
#End Region

#Region " Methods "

    Private Shared Function GetChkByQueryString(ByVal queryString As String) As Integer
        Dim sessionId As String
        Try
            sessionId = HttpContext.Current.Session.SessionID.ToString()
        Catch ex As Exception
            sessionId = String.Empty
        End Try
        Dim bb As Byte() = New ASCIIEncoding().GetBytes(String.Concat(queryString, sessionId))
        ''Dim hashed As Byte() = New SHA1CryptoServiceProvider().ComputeHash(bb)
        Dim chk As Integer
        For Each item As Byte In bb
            chk += item
        Next
        Return chk
    End Function

    Public Shared Function AppendSecurityCheck(ByVal queryString As String) As String
        Return String.Format("{0}&Chk={1}", queryString, GetChkByQueryString(queryString))
    End Function

    Public Shared Function VerifyChkQueryString(qs As NameValueCollection, promptException As Boolean) As Boolean
        Dim chk As Integer? = qs.GetValueOrDefault(Of Integer?)("Chk", Nothing)
        If Not chk.HasValue Then
            Throw New DocSuiteException("Verifica sicurezza", "Impossibile validare il link, parametro mancante o non valido.")
        End If

        Dim temp As String = qs.ToString()
        temp = temp.Substring(0, temp.LastIndexOf("Chk="))
        If temp.EndsWith("&") Then
            temp = temp.Substring(0, temp.Length - 1)
        End If
        If chk = GetChkByQueryString(temp) Then
            Return True
        End If
        If promptException Then
            ' Aggiungo un log aggiuntivo data la gravità
            ' TODO: mettere su un log che manda la mail?
            FileLogger.Fatal(LogName.FileLog, String.Format("ATTENZIONE, falso check di sicurezza! utente [{0}].", DocSuiteContext.Current.User.FullUserName))
            Throw New DocSuiteException("Verifica sicurezza", "Impossibile validare il link.")
        End If
        Return False
    End Function

    ''' <summary>
    ''' Verifica se almeno uno dei gruppi nell'elenco fornito esiste nell'elenco dei SecurityGroups connessi.
    ''' </summary>
    ''' <param name="connectedGroups">Lista di SecurityGroups</param>
    ''' <param name="groupString">Elenco di gruppi da verificare nel formato "1|2|..."</param>
    Public Shared Function SecurityGroupExistsIn(connectedGroups As IList(Of SecurityGroups), groupString As String) As Boolean

        If connectedGroups IsNot Nothing Then
            For Each groupId As String In groupString.Split({"|"c, ","c}, StringSplitOptions.RemoveEmptyEntries)
                Dim id As Integer
                If Not Integer.TryParse(groupId, id) Then
                    Dim message As String = String.Format("Il valore {0} non è un identificativo SecurityGroups valido.", groupId)
                    Throw New InvalidCastException(message)
                End If
                If connectedGroups.Any(Function(sg) sg.Id = id) Then
                    Return True
                End If
            Next
        End If
        Return False
    End Function

    ''' <summary>
    ''' Verifica se l'operatore corrente appartiene almeno ad uno dei gruppi nell'elenco fornito.
    ''' Di default restituisce False se la lista è vuota.
    ''' Utilizzare l'overload a 2 parametri per gestire il valore di default su lista vuota.
    ''' </summary>
    ''' <param name="groupList">
    ''' Elenco di gruppi da verificare.
    ''' Se IsSecurityGroupEnabled nel formato "1|2|..."
    ''' altrimenti "Group1|Group2|..."
    ''' </param>
    Public Shared Function UserConnectedBelongsTo(groupList As String) As Boolean
        Return UserConnectedBelongsTo(groupList, False)
    End Function

    ''' <summary> Verifica se l'operatore corrente appartiene almeno ad uno dei gruppi nell'elenco fornito. </summary>
    ''' <param name="groupList">
    ''' Elenco di gruppi da verificare.
    ''' Se IsSecurityGroupEnabled nel formato "1|2|..."
    ''' altrimenti "Group1|Group2|..."
    ''' </param>
    ''' <param name="defaultOnEmptyGroupList">
    ''' Definisce il valore Booleano da restituire in caso di lista di gruppi vuota
    ''' </param>
    Public Shared Function UserConnectedBelongsTo(groupList As String, defaultOnEmptyGroupList As Boolean) As Boolean
        If String.IsNullOrEmpty(groupList) Then
            Return defaultOnEmptyGroupList
        End If

        Dim results As IList(Of SecurityGroups) = Nothing
        If Not CacheSingleton.Instance.SecurityGroupsByUsers.ContainsKey(DocSuiteContext.Current.User.FullUserName) OrElse Not CacheSingleton.Instance.SecurityGroupsByUsers.TryGetValue(DocSuiteContext.Current.User.FullUserName, results) Then
            results = FacadeFactory.Instance.SecurityUsersFacade.GetGroupsByAccount(DocSuiteContext.Current.User.UserName)
            CacheSingleton.Instance.SecurityGroupsByUsers.TryAdd(DocSuiteContext.Current.User.FullUserName, results)
        End If
        Return SecurityGroupExistsIn(results, groupList)
    End Function

    Public Shared Function UserBelongsTo(domain As String, securityGroups As IList(Of SecurityGroups), containerGroupList As IList(Of ContainerGroup)) As Boolean
        If containerGroupList Is Nothing OrElse containerGroupList.Count = 0 Then
            Return False
        End If

        Try
            Return SecurityGroupExistsIn(securityGroups, String.Join("|", containerGroupList.Select(Function(cgl) cgl.SecurityGroup.Id).ToArray()))
        Catch ex As Exception
            FileLogger.Error(LogName.FileLog, "UserBelongsTo: Errore in fase di verifica dell'appartenenza di un utente ad un gruppo.", ex)
            Return False
        End Try
    End Function

    ''' <summary>Dato il nome di un file o l'estensione ritorna l'icona corrispondente</summary>
    ''' <param name="objectType">
    ''' <list type="table">
    ''' <listheader>
    ''' <term>Tipi di oggetti:</term>
    ''' </listheader>
    ''' <item>
    ''' <term>FL</term>
    ''' <description>File generico, esamina l'estensione</description>
    ''' </item>
    ''' <item>
    ''' <term>LP</term>
    ''' <description>Protocollo</description>
    ''' </item>
    ''' <item>
    ''' <term>LF</term>
    ''' <description>Fascicolo</description>
    ''' </item>
    ''' <item>
    ''' <term>LR</term>
    ''' <description>Atti</description>
    ''' </item>
    ''' </list>
    ''' </param>
    ''' <param name="extension">Nome del file o estensione.</param>
    Public Shared Function GetIconName(ByVal objectType As String, ByVal extension As String) As String
        ' Se il tipo di oggetto non è presente lascio il nome vuoto
        If String.IsNullOrWhiteSpace(objectType) Then
            Return String.Empty
        End If

        Dim iconName As String = ""
        Select Case objectType.ToUpper()
            Case "FL"
                ' lascio notepad, se viene visualizzata questa icona c'è un'errore
                iconName = "NotePad"
                FileLogger.Warn(LogName.FileLog, "Questa riga di codice è stata sostituita con ImagePath.FromFile()")
            Case "LP"
                iconName = "Protocollo"
            Case "LF"
                iconName = "Fascicolo"
            Case "LR"
                iconName = "Atti"
            Case "A"
                iconName = "Remove"
        End Select

        Return iconName
    End Function

    Public Shared Function GetArrayUserFromString() As String()
        Return FacadeFactory.Instance.SecurityUsersFacade.GetGroupsByAccount(DocSuiteContext.Current.User.FullUserName).Select(Function(sg) sg.GroupName).ToArray()
    End Function

    ''' <summary> Preleva un valore dalla sessione o dal dizionario. </summary>
    ''' <param name="fieldName">Nome della variabile in sessione</param>
    Public Shared Function GetContextValue(ByVal fieldName As String, Optional ByVal appendUserNameKeyName As Boolean = False) As Object
        If HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Session IsNot Nothing Then
            Return HttpContext.Current.Session(fieldName)
        End If
        If appendUserNameKeyName Then
            fieldName = String.Concat(DocSuiteContext.Current.User.FullUserName, fieldName)
        End If
        Dim returnValue As Object = Nothing
        If _processContext IsNot Nothing AndAlso _processContext.ContainsKey(fieldName) AndAlso _processContext.TryGetValue(fieldName, returnValue) Then
            Return returnValue
        End If
        Return String.Empty
    End Function

    ''' <summary> Imposta un valore in sessione o nel dizionario. </summary>
    ''' <param name="fieldName">Nome della variabile in sessione</param>
    Public Shared Sub SetContextValue(ByVal fieldName As String, ByVal value As Object, Optional ByVal needProcessContext As Boolean = False, Optional ByVal appendUserNameKeyName As Boolean = False)
        Dim sessionSaved As Boolean = False
        If (HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Session IsNot Nothing) Then
            HttpContext.Current.Session.Add(fieldName, value)
            sessionSaved = True
        End If
        If needProcessContext OrElse Not sessionSaved Then
            If appendUserNameKeyName Then
                fieldName = String.Concat(DocSuiteContext.Current.User.FullUserName, fieldName)
            End If
            _processContext.AddOrUpdate(fieldName, value, Function(k, v) value)
        End If
    End Sub

    Public Shared Sub SaveThreadContext()
        _processContext.TryAdd(USER_CONNECTED_GROUPS_FIELD, UserConnectedGroups)
        _processContext.TryAdd(USER_FULLNAME_FIELD, UserFullName)
        _processContext.TryAdd(USER_SESSIONID_FIELD, UserSessionId)
        _processContext.TryAdd(USER_DESCRIPTION_FIELD, DSUserComputer)
        _processContext.TryAdd(USER_MAIL_FIELD, DsUserMail)
        _processContext.TryAdd(USER_COMPUTER_FIELD, DSUserComputer)
    End Sub


    ' TODO: ELIMINARE
    Public Shared Function ConvData(ByVal Data As String) As Date
        If Data = "" Then Exit Function
        If InStr(Data, " ") = 0 Then   'se space date + time
            Data = Replace(Data, ".", "/")
        End If
        Return CDate(Data)
    End Function

    Public Shared Sub ClearEffectiveOChart()
        MemoryCache.Default.Remove("CommonShared_EffectiveOChart")
    End Sub

    Public Shared Function UserDocumentSeriesCheckRight(containerRight As DocumentSeriesContainerRightPositions) As Boolean
        If UserDocumentSeriesActiveContainerRightDictionary.ContainsKey(containerRight) Then
            If UserDocumentSeriesActiveContainerRightDictionary(containerRight).Count > 0 Then
                Return True
            End If
        End If
        Return False
    End Function
    Public Shared Function UserProtocolCheckRight(containerRight As ProtocolContainerRightPositions) As Boolean
        If UserProtocolActiveContainerRightDictionary.ContainsKey(containerRight) Then
            If UserProtocolActiveContainerRightDictionary(containerRight).Count > 0 Then
                Return True
            End If
        End If
        Return False
    End Function

    Public Shared Function UserUDSCheckRight(containerRight As ProtocolContainerRightPositions) As Boolean
        If UserUDSActiveContainerRightDictionary.ContainsKey(containerRight) Then
            If UserUDSActiveContainerRightDictionary(containerRight).Count > 0 Then
                Return True
            End If
        End If
        Return False
    End Function

    Public Shared Function UserDocumentCheckRight(containerRight As DocumentContainerRightPositions) As Boolean
        If UserDocumentActiveContainerRightDictionary.ContainsKey(containerRight) Then
            If UserDocumentActiveContainerRightDictionary(containerRight).Count > 0 Then
                Return True
            End If
        End If
        Return False
    End Function
    Public Shared Function UserResolutionCheckRight(containerRight As ResolutionRightPositions) As Boolean
        If UserResolutionActiveContainerRightDictionary.ContainsKey(containerRight) Then
            If UserResolutionActiveContainerRightDictionary(containerRight).Count > 0 Then
                Return True
            End If
        End If
        Return False
    End Function
#End Region

End Class
