Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods

<ComponentModel.DataObject()>
Public Class ContainerFacade
    Inherits CommonFacade(Of Container, Integer, NHibernateContainerDao)

#Region " Constructors "

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal dbName As String)
        MyBase.New(dbName)
    End Sub

#End Region

#Region " Methods "

    Public Function AlreadyExists(name As String) As Boolean
        Return _dao.AlreadyExists(name)
    End Function

    Public Overrides Sub Save(ByRef container As Container)
        'Recupero ID dalla tabella parameter
        Dim pf As New ParameterFacade(_dbName)
        Dim parameter As Parameter = pf.GetAll()(0)
        container.Id = parameter.LastUsedIdContainer + 1
        container.IsActive = 1
        container.RegistrationDate = DateTimeOffset.UtcNow
        container.RegistrationUser = DocSuiteContext.Current.User.FullUserName
        If (pf.UpdateReplicateLastIdContainer(parameter.LastUsedIdContainer + 1, parameter.LastUsedIdContainer)) Then
            For Each containerGroup As ContainerGroup In container.ContainerGroups
                containerGroup.Container = container
            Next
            FacadeFactory.Instance.TableLogFacade.Insert("Container", LogEvent.INS, String.Concat("Inserito il contenitore ", container.Name), container.UniqueId)
            If container.PrivacyEnabled Then
                FacadeFactory.Instance.TableLogFacade.Insert("Container", LogEvent.PR, String.Format("Inserito il contenitore {0} con privacy abilitata con livello privacy {1}", container.Id, container.PrivacyLevel), container.UniqueId)
            End If
            MyBase.Save(container)
        End If
    End Sub

    Public Overrides Sub Update(ByRef container As Container)
        container.LastChangedDate = DateTimeOffset.UtcNow
        container.LastChangedUser = DocSuiteContext.Current.User.FullUserName
        FacadeFactory.Instance.TableLogFacade.Insert("Container", LogEvent.UP, String.Concat("Modificato il contenitore ", container.Name), container.UniqueId)
        MyBase.Update(container)
    End Sub

    Public Overrides Sub UpdateOnly(ByRef container As Container)
        container.LastChangedDate = DateTimeOffset.UtcNow
        container.LastChangedUser = DocSuiteContext.Current.User.FullUserName
        FacadeFactory.Instance.TableLogFacade.Insert("Container", LogEvent.UP, String.Concat("Modificato il contenitore ", container.Name), container.UniqueId)
        MyBase.UpdateOnly(container)
    End Sub

    Public Overrides Function IsUsed(ByRef obj As Container) As Boolean
        If _dao.ContainerUsedProtocol(obj) Then
            Return True
        End If
        If DocSuiteContext.Current.IsDocumentEnabled Then
            If _dao.ContainerUsedDocument(obj) Then
                Return True
            End If
        End If
        If DocSuiteContext.Current.IsResolutionEnabled Then
            If _dao.ContainerUsedResolution(obj) Then
                Return True
            End If
        End If

        Return False
    End Function

    Public Function GetContainerRigths(ByVal type As String, ByVal groups As String, ByVal isActive As Short) As IList(Of Container)
        Return _dao.GetContainerRigths(type, groups, isActive, Nothing, Nothing)
    End Function

    Public Function GetContainerByDocumentRigths(ByVal groups As String, ByVal isActive As Short, ByVal rights As DocumentContainerRightPositions, ByVal idContainer As Integer?) As IList(Of Container)
        Return _dao.GetContainerRigths("DOCM", groups, isActive, rights, idContainer)
    End Function

    Public Function FilterContainers(ByVal items As IList(Of Container), filter As String) As IList(Of Container)
        If String.IsNullOrEmpty(filter) Then
            Return items
        End If

        Dim filtered As New List(Of Container)
        For Each container As Container In From container1 In items Where container1.Name.ContainsIgnoreCase(filter) AndAlso Not filtered.Contains(container1)
            filtered.Add(container)
        Next

        Return filtered
    End Function

    Public Overloads Function GetSecurityGroupsContainerRights(ByVal type As String, ByVal securityGroups As IList(Of SecurityGroups), ByVal isActive As Short, ByVal rightPosition As DocumentContainerRightPositions, ByVal idContainer As Integer?) As IList(Of Container)
        Return _dao.GetSecurityGroupsContainerRights(type, securityGroups, isActive, rightPosition, idContainer)
    End Function

    Public Function GetContainerExtensionRigths(ByVal type As String, ByVal groups As String, ByVal active As Short, ByVal keyType As String, ByVal rights As String) As IList(Of Container)
        ' TODO: verificare chi la usa
        Return _dao.GetContainerExtensionRigths(type, groups, active, keyType, rights)
    End Function

    Public Function GetAllRights(ByVal type As String, ByVal isActive As Short?) As IList(Of ContainerRightsDto)
        Return _dao.GetAllRights(type, isActive)
    End Function

    Public Function GetContainerByName(ByVal description As String) As IList(Of Container)
        Return _dao.GetContainerByName(description, Nothing)
    End Function

    Public Function GetContainerByName(ByVal description As String, ByVal isActive As Boolean) As IList(Of Container)
        Dim status As Short = Convert.ToInt16(isActive)
        Return _dao.GetContainerByName(description, status)
    End Function

    Public Function IsAlmostOneConservationEnabled() As Boolean
        Return _dao.IsAlmostOneConservationEnabled()
    End Function

    Public Function GetAllRightsDistinct(ByVal type As String, ByVal isActive As Short?) As IList(Of Container)
        Return _dao.GetAllRightsDistinct(type, isActive)
    End Function

    ''' <summary> Verifica se si han diritti di inserimento o gestione. </summary>
    Public Function HasInsertOrProposalRights() As Boolean
        Return CommonShared.UserProtocolCheckRight(ProtocolContainerRightPositions.Insert) OrElse CommonShared.UserResolutionCheckRight(ResolutionRightPositions.Insert)
    End Function

    Public Function GetContainers(env As DSWEnvironment, rightPosition As Integer?, active As Boolean?, Optional idProtocolType As Nullable(Of Short) = Nothing) As IList(Of Container)
        Dim rightPositions As List(Of Integer) = Nothing
        If rightPosition.HasValue Then
            rightPositions = New List(Of Integer)({rightPosition.Value})
        End If
        Return GetContainers(env, rightPositions, active, idProtocolType)
    End Function

    Public Function GetContainers(env As DSWEnvironment, rightPositions As List(Of Integer), active As Boolean?, Optional idProtocolType As Nullable(Of Short) = Nothing) As IList(Of Container)
        Dim groups As IList(Of SecurityGroups) = FacadeFactory.Instance.SecurityUsersFacade.GetGroupsByAccount(DocSuiteContext.Current.User.UserName)
        If groups.IsNullOrEmpty() Then
            Return Nothing
        End If

        Dim identifiers As List(Of Integer) = groups.Select(Function(g) g.Id).ToList()
        Return _dao.GetContainersBySG(identifiers, env, rightPositions, active, idProtocolType)
    End Function

    Public Function GetContainers(domain As String, userName As String, env As DSWEnvironment, rightPosition As Integer?, active As Boolean?) As IList(Of Container)
        Dim groups As IList(Of SecurityGroups) = Factory.SecurityUsersFacade.GetGroupsByAccount(userName)
        If groups.IsNullOrEmpty() Then
            Return Nothing
        End If
        Return _dao.GetContainersBySG(groups.Select(Function(g) g.Id).ToList(), env, rightPosition, active)
    End Function

    Public Function GetContainersCount(env As DSWEnvironment, rightPosition As Integer?, active As Boolean?) As Integer
        Dim groups As IList(Of SecurityGroups) = Factory.SecurityUsersFacade.GetGroupsByAccount(DocSuiteContext.Current.User.UserName)
        If groups.IsNullOrEmpty() Then
            Return Nothing
        End If
        Return _dao.GetContainersCountBySG(groups.Select(Function(g) g.Id).ToList(), env, rightPosition, active)
    End Function

    Public Function CheckContainerRight(idContainer As Integer, env As DSWEnvironment, rightPosition As Integer?, active As Boolean?) As Boolean
        Dim groups As IList(Of SecurityGroups) = Factory.SecurityUsersFacade.GetGroupsByAccount(DocSuiteContext.Current.User.UserName)
        If groups.IsNullOrEmpty() Then
            Return Nothing
        End If
        Return _dao.CheckContainerRightBySG(idContainer, groups.Select(Function(g) g.Id).ToList(), env, rightPosition, active)
    End Function

    Public Function CheckContainerRight(idContainer As Integer, domain As String, userName As String, env As DSWEnvironment, rightPosition As Integer?, active As Boolean?) As Boolean
        Dim groups As IList(Of SecurityGroups) = Factory.SecurityUsersFacade.GetGroupsByAccount(userName)
        If groups.IsNullOrEmpty() Then
            Return Nothing
        End If
        Return _dao.CheckContainerRightBySG(idContainer, groups.Select(Function(g) g.Id).ToList(), env, rightPosition, active)
    End Function

    Public Function GetCurrentUserContainers(env As DSWEnvironment) As Dictionary(Of ProtocolContainerRightPositions, IList(Of Integer))
        Return GetCurrentUserContainers(env, Nothing)
    End Function

    Public Function GetCurrentUserContainers(env As DSWEnvironment, active As Boolean?) As Dictionary(Of ProtocolContainerRightPositions, IList(Of Integer))
        Dim tor As New Dictionary(Of ProtocolContainerRightPositions, IList(Of Integer))
        Dim temp As IList(Of Integer)
        For Each val As ProtocolContainerRightPositions In [Enum].GetValues(GetType(ProtocolContainerRightPositions))
            temp = GetContainers(env, val, active).Select(Function(c) c.Id).ToList()
            If Not temp.IsNullOrEmpty() Then
                tor.Add(val, temp)
            End If
        Next
        Return tor
    End Function

    ''' <summary> Elenco filtrato dei contenitori in cui posso spostare un protocollo. </summary>
    ''' <param name="filter">Filtro per nome contenitore. </param>
    ''' <param name="currentContainer">Contenitore da aggiungere alla lista dei risultati.</param>
    ''' <remarks>
    ''' Nell'elenco non ci saranno mai contenitori per fatture in quanto in tali contenitori non si possono inserire protocolli da esterno.
    ''' </remarks>
    Public Function GetManageableContainers(filter As String, currentContainer As Container) As IList(Of Container)

        ' Recupero l'elenco dei contenitori su cui ho diritto di inserimento.
        Dim availableContainers As IList(Of Container) = GetContainers(DSWEnvironment.Protocol, ProtocolContainerRightPositions.Insert, True)

        ' Recupero solo quelli ce non sono INVOICE.
        Dim retval As IList(Of Container) = (From c In availableContainers Where Not c.IsInvoiceEnable).ToList()

        ' Se mancante aggiungo anche il contenitore corrente: è il caso di modifiche su protocolli in contenitori su cui non ho diritto di inserimento, ma solo di modifica.
        If DocSuiteContext.Current.ProtocolEnv.EditLoadCurrentContainer AndAlso Not retval.Contains(currentContainer) Then
            retval.Add(currentContainer)
        End If

        If Not String.IsNullOrEmpty(filter) Then
            retval = FilterContainers(retval, filter)
        End If

        Return retval
    End Function


    Public Function GetArchiveNames(idArchive As Integer) As IList(Of String)
        Return _dao.GetArchiveNames(idArchive)
    End Function

    Public Function GetContainersBySecurity(securityGroups As IList(Of SecurityGroups)) As IList(Of Container)
        Return _dao.GetContainersBySecurity(securityGroups)
    End Function

    Public Function GetCurrentUserDocumentSeriesContainers(env As DSWEnvironment, active As Boolean?) As Dictionary(Of DocumentSeriesContainerRightPositions, IList(Of Integer))
        Dim tor As New Dictionary(Of DocumentSeriesContainerRightPositions, IList(Of Integer))
        Dim temp As IList(Of Integer)
        For Each val As DocumentSeriesContainerRightPositions In [Enum].GetValues(GetType(DocumentSeriesContainerRightPositions))
            temp = GetContainers(env, val, active).Select(Function(c) c.Id).ToList()
            If Not temp.IsNullOrEmpty() Then
                tor.Add(val, temp)
            End If
        Next
        Return tor
    End Function

    Public Function GetCurrentUserProtocolContainers(env As DSWEnvironment, active As Boolean?) As Dictionary(Of ProtocolContainerRightPositions, IList(Of Integer))
        Dim tor As New Dictionary(Of ProtocolContainerRightPositions, IList(Of Integer))
        Dim temp As IList(Of Integer)
        For Each val As ProtocolContainerRightPositions In [Enum].GetValues(GetType(ProtocolContainerRightPositions))
            temp = GetContainers(env, val, active).Select(Function(c) c.Id).ToList()
            If Not temp.IsNullOrEmpty() Then
                tor.Add(val, temp)
            End If
        Next
        Return tor
    End Function
    Public Function GetCurrentUserDocumentContainers(env As DSWEnvironment, active As Boolean?) As Dictionary(Of DocumentContainerRightPositions, IList(Of Integer))
        Dim tor As New Dictionary(Of DocumentContainerRightPositions, IList(Of Integer))
        Dim temp As IList(Of Integer)
        For Each val As DocumentContainerRightPositions In [Enum].GetValues(GetType(DocumentContainerRightPositions))
            temp = GetContainers(env, val, active).Select(Function(c) c.Id).ToList()
            If Not temp.IsNullOrEmpty() Then
                tor.Add(val, temp)
            End If
        Next
        Return tor
    End Function

    Public Function GetCurrentUserReolutionContainers(env As DSWEnvironment, active As Boolean?) As Dictionary(Of ResolutionRightPositions, IList(Of Integer))
        Dim tor As New Dictionary(Of ResolutionRightPositions, IList(Of Integer))
        Dim temp As IList(Of Integer)
        For Each val As ResolutionRightPositions In [Enum].GetValues(GetType(ResolutionRightPositions))
            temp = GetContainers(env, val, active).Select(Function(c) c.Id).ToList()
            If Not temp.IsNullOrEmpty() Then
                tor.Add(val, temp)
            End If
        Next
        Return tor
    End Function
#End Region

End Class