Imports System.Collections.Generic
Imports System.Linq
Imports System.Collections.ObjectModel
Imports VecompSoftware.DocSuiteWeb.EntityMapper.PECMails
Imports VecompSoftware.DocSuiteWeb.DTO.PECMails
Imports VecompSoftware.DocSuiteWeb.API
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.ComponentModel

<DataObject()>
Public Class PECMailBoxFacade
    Inherits BaseProtocolFacade(Of PECMailBox, Short, NHibernatePECMailBoxDao)

#Region "Fields"
    Private ReadOnly _mapperPECMailBox As MapperPECMailBox
#End Region

#Region " Constructors "

    Public Sub New()
        MyBase.New()

        _mapperPECMailBox = New MapperPECMailBox()
    End Sub

#End Region

#Region " Methods "

    Public Function GetByRecipient(ByVal address As String) As PECMailBox
        Return _dao.GetByRecipient(address)
    End Function

    Public Function GetByRoles(roles As IList(Of Role)) As IList(Of PECMailBox)
        Return _dao.GetByRoles(roles)
    End Function

    Public Function IsCurrentUserEnabled(mailbox As PECMailBox) As Boolean
        Dim roleGroups As IEnumerable(Of RoleGroup) = mailbox.Roles.SelectMany(Function(sm) sm.RoleGroups).AsEnumerable()

        'Livello minimo per la visualizzazione della casella PEC
        roleGroups = roleGroups.Where(Function(w) w.ProtocolRights.IsRoleEnabled)
        If DocSuiteContext.Current.ProtocolEnv.RoleGroupPECRightEnabled Then
            roleGroups = roleGroups.Where(Function(w) w.ProtocolRights.IsRolePEC)
        End If

        Dim availableRoles As IList(Of Role) = New List(Of Role)()
        availableRoles = roleGroups.Select(Function(s) s.Role).ToList()

        If DocSuiteContext.Current.ProtocolEnv.RoleGroupPECRightEnabled Then
            Return FacadeFactory.Instance.PECMailboxRoleFacade.CurrentUserBelongsPecRole(availableRoles)
        Else
            Return FacadeFactory.Instance.RoleFacade.CurrentUserBelongsToRoles(DSWEnvironment.Protocol, availableRoles)

        End If

        Return False
    End Function

    Public Function IsCurrentUserProtocolMailBoxEnabled(mailbox As PECMailBox) As Boolean
        Dim roleGroups As IEnumerable(Of RoleGroup) = mailbox.Roles.SelectMany(Function(sm) sm.RoleGroups).AsEnumerable()

        'Livello minimo per la visualizzazione della casella PEC
        roleGroups = roleGroups.Where(Function(w) w.ProtocolRights.IsRoleEnabled)

        If DocSuiteContext.Current.ProtocolEnv.RoleGroupProcotolMailBoxRightEnabled Then
            roleGroups = roleGroups.Where(Function(w) w.ProtocolRights.IsRoleProtocolMail)
        End If
        Dim availableRoles As IList(Of Role) = New List(Of Role)()
        availableRoles = roleGroups.Select(Function(s) s.Role).ToList()

        If DocSuiteContext.Current.ProtocolEnv.RoleGroupProcotolMailBoxRightEnabled Then

            Return FacadeFactory.Instance.PECMailboxRoleFacade.CurrentUserBelongsPecRole(availableRoles)

        Else
            Return FacadeFactory.Instance.RoleFacade.CurrentUserBelongsToRoles(DSWEnvironment.Protocol, availableRoles)
        End If

        Return False
    End Function

    Private Function CheckIfAuthorizationOnPecMailBox(ByVal mailboxes As IList(Of PECMailBox), ByVal managed As Boolean) As List(Of PECMailBox)
        Dim available As IEnumerable(Of PECMailBox) = mailboxes.GroupBy(Function(m) m.Id).Select(Function(g) g.First())
        If managed Then
            available = available.Where(Function(m) m.Managed)
        Else
            available = available.Where(Function(m) m.Unmanaged)
        End If

        If CommonShared.HasGroupAdministratorRight Then
            ' Gli amministratori hanno visibilità su tutte le mailbox.
            Return available.ToList()
        End If

        Return available.Where(Function(m) IsCurrentUserEnabled(m)).ToList()
    End Function

    Private Function CheckIfAuthorizationOnProtocolMailBox(ByVal mailboxes As IList(Of PECMailBox), ByVal managed As Boolean) As List(Of PECMailBox)
        Dim available As IEnumerable(Of PECMailBox) = mailboxes.GroupBy(Function(m) m.Id).Select(Function(g) g.First())

        If CommonShared.HasGroupAdministratorRight Then
            ' Gli amministratori hanno visibilità su tutte le mailbox.
            Return available.ToList()
        End If

        Return available.Where(Function(m) IsCurrentUserProtocolMailBoxEnabled(m)).ToList()
    End Function

    Public Function GetIfIsInterop(ByVal isInterop As Boolean) As IList(Of PECMailBox)
        Return _dao.GetIfIsInterop(isInterop)
    End Function

    Public Function GetIncomingMailBoxByIdHost(idHost As Guid, isDefault As Boolean) As IList(Of PECMailBox)
        Return _dao.GetIncomingMailBoxByIdHost(idHost, isDefault)
    End Function

    Public Function GetOutgoingMailBoxByIdHost(idHost As Guid, isDefault As Boolean) As IList(Of PECMailBox)
        Return _dao.GetOutgoingMailBoxByIdHost(idHost, isDefault)
    End Function

    Public Function MailBoxRecipientLabel(ByRef box As PECMailBox) As String
        Return If(box.IsForInterop, "(I) ", "(-) ") + box.MailBoxName
    End Function

    Public Function FillRealPecMailBoxes(mailBoxes As ICollection(Of PECMailBox)) As IList(Of PECMailBox)
        Dim draftMailBox As PECMailBox = DocSuiteContext.Current.ResolutionEnv.MailBoxBozze
        Dim realMailBoxes As IList(Of PECMailBox) = mailBoxes.Where(Function(x) IsRealPecMailBox(x, draftMailBox)).ToList()
        Return realMailBoxes
    End Function

    Public Function IsRealPecMailBox(item As PECMailBox) As Boolean
        Return IsRealPecMailBox(item, DocSuiteContext.Current.ResolutionEnv.MailBoxBozze)
    End Function

    Public Function IsRealPecMailBox(item As PECMailBox, draftMailBox As PECMailBox) As Boolean
        '' Per metodi automatici che potrebbero passare un valore null
        If item Is Nothing Then
            Return False
        End If

        If item.Id = DocSuiteContext.Current.ProtocolEnv.PECDraftMailBoxId Then
            Return False
        End If

        If DocSuiteContext.Current.IsResolutionEnabled AndAlso draftMailBox IsNot Nothing AndAlso item.Id = draftMailBox.Id Then
            Return False
        End If
        Return True
    End Function

    Public Function GetIfIsSendingEnabled(ByVal protocolBoxOnly As Boolean) As IList(Of PECMailBox)
        Return _dao.GetIfIsSendingEnabled(protocolBoxOnly)
    End Function

    Public Function GetProtocolBoxes(ByVal protocolBoxOnly As Boolean) As IList(Of PECMailBox)
        Return _dao.GetProtocolBoxes(protocolBoxOnly)
    End Function

    Public Function CountManyPECMailsReceived(pecMailBoxId As Integer, fromDate As DateTime) As Integer
        Return _dao.CountManyPECMailsReceived(pecMailBoxId, fromDate)
    End Function

    Public Function CountPECMails(pecMailBoxId As Integer) As Integer
        Return _dao.CountPECMails(pecMailBoxId)
    End Function

    Public Function GetVisibleProtocolMailBoxes() As IList(Of PECMailBox)
        Return GetVisibleMailBoxes(False, False, True)
    End Function

    ''' <summary> Ritorna tutte le mailbox sulle quali l'utente ha autorizzazioni. </summary>
    Public Function GetVisibleMailBoxes() As List(Of PECMailBox)
        Return GetVisibleMailBoxes(False)
    End Function

    ''' <summary> Ritorna tutte le mailbox reali (non i folders) sulle quali l'utente ha autorizzazioni. </summary>
    Public Function GetVisibleOnlyPecMail() As List(Of PECMailBox)
        Return GetVisibleMailBoxes(False)
    End Function

    ''' <summary> Ritorna tutte le mailbox sulle quali l'utente ha autorizzazioni, facendo le verifiche sulle mail integrate. </summary>
    ''' <remarks>Tutta la parte inerente le pec integrate viene tenuta per retrocompatibilità, è possibile cancellarla.</remarks>
    Public Function GetVisibleMailBoxes(ByVal integratedMail As Boolean) As List(Of PECMailBox)
        Return GetVisibleMailBoxes(integratedMail, False)
    End Function

    ''' <summary> Ricerca tutte le mailbox sulle quali l'utente ha autorizzazioni e con le quali può effettivamente spedire. </summary>
    Public Function GetVisibleSendingMailBoxes() As List(Of PECMailBox)
        Return GetVisibleMailBoxes(False, True)
    End Function

    Public Function GetVisibleMailBoxes(ByVal integratedMail As Boolean, ByVal sendingEnabledOnly As Boolean) As List(Of PECMailBox)
        Return GetVisibleMailBoxes(integratedMail, sendingEnabledOnly, False)
    End Function

    ''' <summary> Ritorna tutte le mailbox sulle quali l'utente ha autorizzazioni e può effettivamente utilizzare, facendo le verifiche sulle mail integrate. </summary>
    Public Function GetVisibleMailBoxes(ByVal integratedMail As Boolean, ByVal sendingEnabledOnly As Boolean, ByVal protocolBoxOnly As Boolean) As List(Of PECMailBox)
        Dim finder As NHibernatePECMailBoxFinder = New NHibernatePECMailBoxFinder()
        finder.HasOutgoingServer = sendingEnabledOnly
        finder.IntegratedEnabled = integratedMail
        finder.CheckUserRights = Not CommonShared.HasGroupAdministratorRight
        finder.RoleGroupPECRight = DocSuiteContext.Current.ProtocolEnv.RoleGroupPECRightEnabled
        If sendingEnabledOnly AndAlso DocSuiteContext.Current.ProtocolEnv.RoleGroupPECRightEnabled AndAlso Not DocSuiteContext.Current.ProtocolEnv.RoleGroupPECRightOutgoingEnabled Then
            finder.RoleGroupPECRight = Nothing
        End If
        finder.RoleGroupProtocolMailRight = DocSuiteContext.Current.ProtocolEnv.RoleGroupProcotolMailBoxRightEnabled
        finder.OnlyProtocolBox = protocolBoxOnly
        finder.UDSEnabled = DocSuiteContext.Current.ProtocolEnv.UDSEnabled
        finder.GroupIds = FacadeFactory.Instance.SecurityUsersFacade.GetGroupsByAccount(DocSuiteContext.Current.User.UserName).Select(Function(s) s.Id).ToList()
        Dim authorized As List(Of PECMailBox) = finder.DoSearch()
        authorized.Sort(New PecMailBoxComparer())
        Return authorized
    End Function

    ''' <summary>
    ''' Torna tutte le mail per la funzione di sposta PEC
    ''' </summary>
    ''' <param name="mails">Se tutte le mail sono nella stessa casella di posta evito di mostrarla</param>
    Public Function GetMoveMailBoxes(ByRef mails As IList(Of PECMail)) As List(Of PECMailBox)
        Dim mailboxList As List(Of PECMailBox) = GetAll()
        mailboxList.RemoveAll(Function(mb) mb.Unmanaged = False)

        If mails IsNot Nothing AndAlso mails.Count > 0 Then
            ' Rimuovo la mailbox di partenza quando è una sola o quando sono tutte sulla stessa
            Dim mailboxToRemove As PECMailBox = mails(0).MailBox
            If mails.Any(Function(mail) mailboxToRemove IsNot Nothing AndAlso Not mailboxToRemove.Equals(mail.MailBox)) Then
                mailboxToRemove = Nothing
            End If
            If mailboxToRemove IsNot Nothing Then
                mailboxList.Remove(mailboxToRemove)
            End If
        End If

        mailboxList.Sort(New PecMailBoxComparer())
        Return mailboxList
    End Function

    ''' <summary> Torna tutte le mail per la funzione di sposta PEC. </summary>
    Public Function GetMoveMailBoxes() As List(Of PECMailBox)
        Return GetMoveMailBoxes(Nothing)
    End Function

    ''' <summary> Verifica se l'utente corrente è manager della mailbox specificata. </summary>
    Public Function IsRoleUserManager(mailBox As PECMailBox) As Boolean
        Return IsRoleUserManager(mailBox, DocSuiteContext.Current.User.FullUserName)
    End Function

    ''' <summary> Verifica se l'account ricevuto come parametro è manager della mailbox specificata. </summary>
    Private Function IsRoleUserManager(mailBox As PECMailBox, account As String) As Boolean
        For Each ru As RoleUser In GetRoleUserType(mailBox, account)
            If ru.Type.Eq(RoleUserType.D.ToString()) OrElse ru.Type.Eq(RoleUserType.V.ToString()) Then
                Return True
            End If
        Next
        Return False
    End Function

    ''' <summary> Recupera le abilitazioni per settore dell'account ricevuto come parametro. </summary>
    Private Function GetRoleUserType(mailBox As PECMailBox, account As String) As IList(Of RoleUser)
        Dim retval As New List(Of RoleUser)
        For Each r As Role In mailBox.Roles
            retval.AddRange(Factory.RoleUserFacade.GetByRoleIdAndAccount(r.Id, account, String.Empty))
        Next
        Return retval
    End Function

    ''' <summary> Verifica se l'utente corrente ha permessi di visualizzazione per la mailbox specificata. </summary>
    Public Function IsVisibleMailBox(mailBox As PECMailBox) As Boolean
        Return CommonShared.HasGroupAdministratorRight OrElse IsCurrentUserEnabled(mailBox)
    End Function

    ''' <summary> Mailbox da visualizzare di default. </summary>
    ''' <param name="mailboxes">mailbox tra le quali cercare</param>
    ''' <returns> <see>Nothing</see> se non è presente, altrimenti la prima casella definita come prioritaria. </returns>
    Public Function GetDefault(ByRef mailboxes As IList(Of PECMailBox)) As PECMailBox
        If mailboxes.IsNullOrEmpty() Then
            Return Nothing
        End If

        Dim userRoles As IList(Of Role) = New List(Of Role)
        If DocSuiteContext.Current.ProtocolEnv.RoleGroupPECRightEnabled Then
            userRoles = FacadeFactory.Instance.RoleFacade.GetUserRoles(DSWEnvironment.Protocol, 3, True)
        Else
            userRoles = FacadeFactory.Instance.RoleFacade.GetUserRoles(DSWEnvironment.Protocol, 1, True)
        End If

        Dim defaultBoxes As ICollection(Of PECMailBox) = mailboxes.Where(Function(x) x.MailBoxRoles.Any(Function(xx) xx.Priority AndAlso userRoles.Any(Function(xr) xr.Id = xx.Role.Id))).ToList()
        If Not defaultBoxes.IsNullOrEmpty() Then
            Return defaultBoxes.First()
        End If
        Return Nothing
    End Function

    Private Function CalculateEffectivePecSendMaximumSize(ByVal maxSize As Double) As Double
        Dim tor As Double = maxSize
        Try
            Dim margine As String = DocSuiteContext.Current.ProtocolEnv.PecSendMaximumSizeMargin
            If margine.EndsWith("%") Then
                '' E' un valore percentuale
                Dim factor As Double = Double.Parse(margine.Split("%"c)(0))
                factor = (factor / 100) + 1
                tor = maxSize / factor
            ElseIf margine.EndsWith("bytes") Then
                '' E' un valore da sottrarre al massimo
                Dim marginBytes As Double = Double.Parse(margine.Substring(0, margine.IndexOf("bytes", StringComparison.Ordinal)))
                tor = maxSize - marginBytes
            End If
        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Impossibile calcolare la dimensione massima consentita per l'invio di una PEC", ex)
        End Try
        Return tor
    End Function

    Public Function GetMaxReceiveSize(ByRef mailBox As PECMailBox) As Long
        Return mailBox.Configuration.MaxReceiveByteSize
    End Function

    Public Function GetMaxSendSize(ByRef mailBox As PECMailBox) As Long
        Return Convert.ToInt64(CalculateEffectivePecSendMaximumSize(mailBox.Configuration.MaxSendByteSize))
    End Function

    ''' <summary>
    ''' Restituisce tutte le caselle abilitate alla ricezione PEC
    ''' </summary>
    ''' <remarks>Non verifica i diritti dell'utente su tali caselle</remarks>
    Public Function GetAllIncomingMailBox() As ICollection(Of PECMailBoxDto)
        Dim mailBoxes As New List(Of PECMailBox)()
        mailBoxes.AddRange(Factory.PECMailboxFacade.GetIfIsInterop(True).Where(Function(box) Not String.IsNullOrEmpty(box.IncomingServerName) AndAlso Not box.IsProtocolBox.GetValueOrDefault(False)))
        mailBoxes.AddRange(Factory.PECMailboxFacade.GetIfIsInterop(False).Where(Function(box) Not String.IsNullOrEmpty(box.IncomingServerName) AndAlso Not box.IsProtocolBox.GetValueOrDefault(False)))

        Dim dtos As ICollection(Of PECMailBoxDto)
        dtos = _mapperPECMailBox.MappingDTO(mailBoxes)
        Return dtos
    End Function

    Public Function GetHumanManageable() As PECMailBoxFacade
        _dao.SetHumanManageableFilter()
        Return Me
    End Function
#End Region

End Class

