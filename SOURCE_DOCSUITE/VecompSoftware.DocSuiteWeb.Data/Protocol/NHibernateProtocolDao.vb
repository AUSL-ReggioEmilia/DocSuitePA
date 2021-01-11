Imports System
Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports NHibernate.Transform
Imports VecompSoftware.NHibernateManager.Transformer
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.Helpers
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao
Imports VecompSoftware.Helpers.NHibernate
Imports System.Linq

Public Class NHibernateProtocolDao
    Inherits BaseNHibernateDao(Of Protocol)

    Public Enum ConcourseOrder
        ProtocolNumber
        Alphabetic
    End Enum

    Public Enum FetchingStrategy
        Common
        BasicDataAndPermissions
        BasicDataAndLocation
    End Enum

#Region " Constructors "

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

    Public Function GetCountByContainer(ByVal container As Container) As Long
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        crit.Add(Restrictions.Eq("Container", container))
        crit.SetProjection(Projections.RowCountInt64())
        Return crit.UniqueResult(Of Long)()
    End Function

    Public Function GetCountByCategory(ByVal category As Category) As Long
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        crit.Add(Restrictions.Eq("Category", category))
        crit.SetProjection(Projections.RowCountInt64())
        Return crit.UniqueResult(Of Long)()
    End Function

#Region " GetProtocols "

    Private Sub SetFetchingStrategy(ByRef crit As ICriteria, strategy As FetchingStrategy)
        Select Case strategy
            Case FetchingStrategy.Common
            Case FetchingStrategy.BasicDataAndPermissions
                With crit
                    .SetFetchMode("Category", FetchMode.Lazy)

                    .SetFetchMode("PecMails", FetchMode.Lazy)
                    .SetFetchMode("DocumentType", FetchMode.Lazy)
                    If DocSuiteContext.Current.ProtocolEnv.ParerEnabled Then
                        .SetFetchMode("ProtocolParer", FetchMode.Lazy)
                    End If

                    .SetFetchMode("Container", FetchMode.Lazy)
                    .SetFetchMode("Container.ContainerDocTypes", FetchMode.Lazy)
                    .SetFetchMode("Container.ContainerGroups", FetchMode.Lazy)

                    .SetFetchMode("Location", FetchMode.Lazy)
                    .SetFetchMode("Roles", FetchMode.Eager)
                    .SetFetchMode("Roles.Role", FetchMode.Lazy)
                    .SetFetchMode("Roles.Role.RoleGroups", FetchMode.Lazy)
                End With

            Case FetchingStrategy.BasicDataAndLocation
                With crit
                    .SetFetchMode("Category", FetchMode.Lazy)

                    .SetFetchMode("PecMails", FetchMode.Lazy)
                    .SetFetchMode("DocumentType", FetchMode.Lazy)
                    If DocSuiteContext.Current.ProtocolEnv.ParerEnabled Then
                        .SetFetchMode("ProtocolParer", FetchMode.Lazy)
                    End If

                    .SetFetchMode("Container", FetchMode.Lazy)
                    .SetFetchMode("Container.ContainerDocTypes", FetchMode.Lazy)
                    .SetFetchMode("Container.ContainerGroups", FetchMode.Lazy)

                    .SetFetchMode("Location", FetchMode.Eager)
                    .SetFetchMode("Roles", FetchMode.Lazy)
                    .SetFetchMode("Roles.Role", FetchMode.Lazy)
                    .SetFetchMode("Roles.Role.RoleGroups", FetchMode.Lazy)
                End With

            Case Else
                SetFetchingStrategy(crit, FetchingStrategy.Common)
        End Select
    End Sub

    Public Function FinderProtocolByMetadati(year As Short, accountingSectional As String, container As String, vatRegistrationNumber As Integer) As Protocol
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        crit.CreateAlias("AdvanceProtocol", "AP", SqlCommand.JoinType.InnerJoin)
        crit.CreateAlias("Container", "C", SqlCommand.JoinType.InnerJoin)

        crit.Add(Restrictions.Eq("Year", year))
        crit.Add(Restrictions.Eq("AP.AccountingSectional", accountingSectional))
        crit.Add(Restrictions.Eq("C.Name", container))
        crit.Add(Restrictions.Eq("AP.AccountingNumber", vatRegistrationNumber))
        crit.Add(Restrictions.Eq("IdStatus", Convert.ToInt32(ProtocolStatusId.Attivo)))
        Return crit.UniqueResult(Of Protocol)()
    End Function

    Private Function GetProtocols(ByRef crit As ICriteria, keys As IList(Of YearNumberCompositeKey), strategy As FetchingStrategy) As IList(Of Protocol)
        SetFetchingStrategy(crit, strategy)
        Return GetProtocols(crit, keys)
    End Function

    Private Function GetProtocols(ByRef crit As ICriteria, ByVal keys As IList(Of YearNumberCompositeKey)) As IList(Of Protocol)
        If keys IsNot Nothing AndAlso keys.Count > 0 Then
            Dim aggregated As IDictionary(Of Short, IList(Of Integer)) = AggregateProtocolKeys(keys)
            With crit
                Dim disj As New Disjunction()
                For Each item As KeyValuePair(Of Short, IList(Of Integer)) In aggregated
                    Dim conj As New Conjunction
                    conj.Add(Restrictions.Eq("P.Year", item.Key))
                    Dim numbers As New List(Of Integer)(item.Value)
                    conj.Add(Restrictions.In("P.Number", numbers))
                    disj.Add(conj)
                Next
                .Add(disj)
                .AddOrder(Order.Asc("P.Year"))
                .AddOrder(Order.Asc("P.Number"))
                Return .List(Of Protocol)()
            End With
        End If
        Return Nothing
    End Function

    Public Function GetProtocols(keys As IList(Of YearNumberCompositeKey), strategy As FetchingStrategy) As IList(Of Protocol)
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(Of Protocol)("P")
        Return GetProtocols(crit, keys, strategy)
    End Function

    Public Function GetProtocols(ids As ICollection(Of Guid), strategy As FetchingStrategy) As IList(Of Protocol)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of Protocol)("P")
        SetFetchingStrategy(criteria, strategy)
        criteria.Add(Restrictions.InG("Id", ids))
        criteria.AddOrder(Order.Asc("P.Year"))
        criteria.AddOrder(Order.Asc("P.Number"))
        Return criteria.List(Of Protocol)
    End Function

#End Region

    ''' <summary> Elenco dei protocolli modificati nel range specificato  </summary>
    ''' <remarks> Query specifica per la scrivania </remarks>
    Public Function GetUserProtocolDiary(ByVal fromDate As DateTime, ByVal toDate As DateTime, currentTenantAOOId As Guid) As ICollection(Of UserDiary)
        Dim qQuery As IQuery = NHibernateSession.GetNamedQuery("ProtUserDiary")

        qQuery.SetResultTransformer(Transformers.AliasToBean(New UserDiary().GetType()))
        qQuery = qQuery.SetParameter("SystemUser", DocSuiteContext.Current.User.FullUserName)
        qQuery = qQuery.SetParameter("LogDateFrom", fromDate.BeginOfTheDay().ToVecompSoftwareString())
        qQuery = qQuery.SetParameter("LogDateTo", toDate.EndOfTheDay().ToVecompSoftwareString())
        qQuery = qQuery.SetParameter("IdTenantAOO", currentTenantAOOId)

        Return qQuery.List(Of UserDiary)()
    End Function

    Public Function IsUsedDocType(ByVal doctype As DocumentType) As IList(Of Protocol)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType, "P")

        criteria.CreateAlias("P.DocumentType", "DocumentType")
        criteria.Add(Restrictions.Eq("DocumentType.Id", doctype.Id))
        criteria.SetMaxResults(1)
        Return criteria.List(Of Protocol)()

    End Function

    Public Function GetByYearNumber(year As Short, number As Integer) As Protocol
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Year", year))
        criteria.Add(Restrictions.Eq("Number", number))
        Return criteria.UniqueResult(Of Protocol)()
    End Function

    Public Function GetByUniqueId(uniqueId As Guid) As Protocol
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Id", uniqueId))
        Return criteria.UniqueResult(Of Protocol)()
    End Function

    Public Function GetMaxProtocolNumber(ByVal year As Short) As Integer
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        crit.Add(Restrictions.Eq("Year", year))
        crit.SetProjection(Projections.Max("Number"))
        Return crit.UniqueResult(Of Integer)()
    End Function


    ''' <summary>
    ''' Recupera una lista di Protocolli che hanno uno degli status cercati
    ''' </summary>
    Public Function GetProtocolsByStatuses(ByVal statuses As List(Of Integer)) As IList(Of Protocol)
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(persitentType, "P")
        crit.Add(Expression.In("P.IdStatus", statuses))
        Return crit.List(Of Protocol)()
    End Function

    Public Function HasProtSuspended() As Boolean
        Return HasProtSuspended(Nothing)
    End Function

    Public Function HasProtSuspended(year As Short?) As Boolean
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType, "P")
        criteria.Add(Restrictions.Eq("P.IdStatus", Convert.ToInt32(ProtocolStatusId.Sospeso)))
        If year.HasValue Then
            criteria.Add(Restrictions.Eq("P.Year", year.Value))
        End If
        criteria.SetProjection(Projections.RowCount())
        Return criteria.UniqueResult(Of Integer) > 0
    End Function

    Public Function CountProtSuspended(year As Short, from As DateTime, [to] As DateTime) As Integer
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType, "P")
        criteria.Add(Restrictions.Eq("P.IdStatus", Convert.ToInt32(ProtocolStatusId.Sospeso)))
        criteria.Add(Restrictions.Eq("P.Year", year))
        criteria.Add(Restrictions.Between("P.RegistrationDate", New DateTimeOffset(from), New DateTimeOffset([to])))
        criteria.SetProjection(Projections.RowCount())
        Return criteria.UniqueResult(Of Integer)
    End Function
    Public Function GetFirstProtocolSuspended(year As Short, from As DateTime, [to] As DateTime) As Protocol
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType, "P")
        criteria.Add(Restrictions.Eq("P.IdStatus", Convert.ToInt32(ProtocolStatusId.Sospeso)))
        criteria.Add(Restrictions.Eq("P.Year", year))
        criteria.Add(Restrictions.Between("P.RegistrationDate", New DateTimeOffset(from), New DateTimeOffset([to])))
        criteria.AddOrder(Order.Asc("P.Number"))
        criteria.SetMaxResults(1)
        Return criteria.UniqueResult(Of Protocol)
    End Function

    ''' <summary> Esegue la sospensione di protocolli. </summary>
    ''' <param name="suspendNumber">Numero di protocolli da sospendere.</param>
    ''' <param name="suspendDate">Data Registrazione dei protocolli che verranno sospesi.</param>
    Public Function Suspend(ByVal suspendNumber As Integer, ByVal suspendDate As Date, suspendYear As Short?, currentTenantAOOId As Guid) As List(Of String)
        Dim vParameter As New Parameter
        Dim session As ISession = NHibernateSessionManager.Instance.GetSessionFrom("ProtDB")
        Dim sqlQuery As IQuery = session.CreateSQLQuery($"select * from Parameter with (xlock, rowlock) where IdTenantAOO='{currentTenantAOOId}'").AddEntity(vParameter.GetType).SetMaxResults(1)
        Dim tx As ITransaction = session.BeginTransaction(IsolationLevel.Serializable)

        Dim retval As New List(Of String)
        Try
            Dim currentProtocolYear As Short
            Dim firstAvailableProtocolNumber As Integer
            Dim lastUsedNumber As Integer
            vParameter = sqlQuery.UniqueResult(Of Parameter)()
            session.Refresh(vParameter)
            Dim useParameterData As Boolean = Not suspendYear.HasValue OrElse (vParameter.LastUsedYear = suspendYear.Value)
            If useParameterData Then
                currentProtocolYear = vParameter.LastUsedYear
                firstAvailableProtocolNumber = vParameter.LastUsedNumber
                vParameter.LastUsedNumber += suspendNumber
                lastUsedNumber = vParameter.LastUsedNumber
            Else
                currentProtocolYear = suspendYear.Value
                Dim lastProtocolNumber As Integer = GetMaxProtocolNumber(suspendYear.Value)
                firstAvailableProtocolNumber = lastProtocolNumber + 1
                lastUsedNumber = firstAvailableProtocolNumber + suspendNumber
            End If

            Dim currentLastChangedDate As Date = DateTime.Today
            Dim categoryDao As New NHibernateCategoryDao("ProtDB")
            Dim currentSuspendCategory As Category = categoryDao.GetById(DocSuiteContext.Current.ProtocolEnv.SuspendCategory, False)
            Dim containerDao As New NHibernateContainerDao("ProtDB")
            Dim currentSuspendContainer As Container = containerDao.GetById(DocSuiteContext.Current.ProtocolEnv.SuspendContainer, False)
            Dim protocolTypeDao As New NHibernateProtocolTypeDao("ProtDB")
            Dim currentSuspendProtocolType As ProtocolType = protocolTypeDao.GetById(-1, False)
            Dim locationDao As New NHibernateLocationDao("ProtDB")
            Dim currentSuspendLocation As Location = locationDao.GetById(0, False)

            Dim currentProtocol As Protocol
            For currentProtocolNumber As Integer = firstAvailableProtocolNumber To lastUsedNumber - 1
                currentProtocol = New Protocol

                currentProtocol.Year = currentProtocolYear
                currentProtocol.Number = currentProtocolNumber
                currentProtocol.LastChangedUser = DocSuiteContext.Current.User.FullUserName
                currentProtocol.LastChangedDate = currentLastChangedDate
                currentProtocol.Category = currentSuspendCategory
                currentProtocol.IdStatus = ProtocolStatusId.Sospeso
                currentProtocol.Type = currentSuspendProtocolType
                currentProtocol.ProtocolObject = "DUMMY"
                currentProtocol.Container = currentSuspendContainer
                currentProtocol.Location = currentSuspendLocation
                currentProtocol.RegistrationDate = suspendDate
                currentProtocol.RegistrationUser = DocSuiteContext.Current.User.FullUserName
                currentProtocol.AdvanceProtocol.RegistrationDate = suspendDate
                currentProtocol.AdvanceProtocol.RegistrationUser = DocSuiteContext.Current.User.FullUserName



                retval.Add(String.Format("{0}/{1:0000000}", suspendDate.Year, currentProtocol.Number))
                session.Save(currentProtocol)
            Next

            If useParameterData Then
                session.Update(vParameter)
            End If

            tx.Commit()
        Catch ex As Exception
            tx.Rollback()
            Throw
        Finally
            session.Flush()
        End Try

        Return retval
    End Function

    Function GetLastProtocolInDuplicateMetadatas(ByVal documentCode As String, ByVal [date] As Date?, ByVal subject As String) As Protocol
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        crit.Add(Restrictions.Eq("IdStatus", Convert.ToInt32(ProtocolStatusId.Attivo)))
        crit.Add(Restrictions.Eq("DocumentProtocol", documentCode))

        If ([date].HasValue) Then
            crit.Add(Restrictions.Eq("DocumentDate", [date]))
        Else
            crit.Add(Restrictions.IsNull("DocumentDate"))
        End If

        If Not String.IsNullOrEmpty(subject) Then
            crit.Add(Restrictions.Eq("ProtocolObject", subject))
        End If

        crit.AddOrder(Order.Desc("RegistrationDate"))
        crit.SetMaxResults(1)

        Return crit.UniqueResult(Of Protocol)()
    End Function

    ''' <summary> Metodo che ritira protocolli e anagrafiche dei concorsi nel contenitore richiesto. </summary>
    ''' <param name="dateFrom">Data di inizio ricerca</param>
    ''' <param name="dateto">Data di fine ricerca</param>
    ''' <param name="categoryPath">Contenitore</param>
    ''' <remarks>Metodo per la stampa dei concorsi</remarks>
    Function GetProtocolForConcourse(ByVal dateFrom As Date?, ByVal dateto As Date?, ByVal categoryPath As String) As IList(Of Protocol)
        criteria = NHibernateSession.CreateCriteria(persitentType, "P")

        NHibernateSession.EnableFilter("ContactComType").SetParameter("Type", "M")
        NHibernateSession.EnableFilter("ManualContactComType").SetParameter("Type", "M")

        'contatti
        criteria.CreateAlias("P.Contacts", "Contacts", SqlCommand.JoinType.LeftOuterJoin)
        criteria.CreateCriteria("Contacts.Contact", "Contact", SqlCommand.JoinType.LeftOuterJoin)
        criteria.SetFetchMode("Contact", FetchMode.Join)
        criteria.SetFetchMode("Contacts", FetchMode.Join)
        'contatti manuali
        criteria.CreateAlias("P.ManualContacts", "ManualContacts", SqlCommand.JoinType.LeftOuterJoin)
        criteria.SetFetchMode("ManualContacts", FetchMode.Join)


        'filtri
        criteria.Add(Restrictions.Ge("P.IdStatus", Convert.ToInt32(ProtocolStatusId.Attivo)))
        If dateFrom.HasValue Then
            criteria.Add(Restrictions.Ge("P.RegistrationDate", New DateTimeOffset(dateFrom.Value)))
        End If
        If dateto.HasValue Then
            criteria.Add(Restrictions.Le("P.RegistrationDate", New DateTimeOffset(dateto.Value)))
        End If
        If Not String.IsNullOrEmpty(categoryPath) Then
            criteria.CreateAlias("P.Category", "Category", SqlCommand.JoinType.LeftOuterJoin)
            criteria.Add(Restrictions.Eq("Category.FullIncrementalPath", categoryPath))
        End If

        GetProtocolForCouncourseProjections(criteria)

        Return criteria.List(Of Protocol)()
    End Function

    ''' <summary> Imposta le proiezioni del criterio a partire da un dizionario di alias. </summary>
    ''' <param name="crit">Criterio di cui impostare le proiezioni.</param>
    ''' <param name="aliases">Dizionario degli alias.</param>
    ''' <remarks>Questo metodo era originariamente nel dao di base, spostato qui in ottica di refactoring/eliminazione.</remarks>
    Private Sub CreateProjections(ByRef crit As ICriteria, aliases As IDictionary(Of String, String))
        If aliases IsNot Nothing AndAlso aliases.Count > 0 Then
            Try
                Dim proj As ProjectionList = Projections.ProjectionList()
                For Each item As KeyValuePair(Of String, String) In aliases
                    proj.Add(Projections.Property(item.Key), item.Value)
                Next
                crit.SetProjection(proj)
            Catch ex As Exception
                Throw New Exception("Errore nell'impostazione proiezioni", ex)
            End Try
        End If
    End Sub

    Private Sub GetProtocolForCouncourseProjections(ByRef crit As ICriteria)
        Dim aliases As New Dictionary(Of String, String)
        aliases.Add("Id", "Id")
        aliases.Add("Year", "Year")
        aliases.Add("Number", "Number")
        aliases.Add("RegistrationDate", "RegistrationDate")
        aliases.Add("ProtocolObject", "ProtocolObject")
        aliases.Add("IdDocument", "IdDocument")
        aliases.Add("DocumentCode", "DocumentCode")
        aliases.Add("Type", "Type")
        aliases.Add("Category", "Category")

        ' Tutte queste proprietà che confluiscono tutte nella stessa... - FG
        aliases.Add("Contact.Description", "Contacts")
        aliases.Add("Contact.BirthDate", "Contacts")
        aliases.Add("Contact.Address.PlaceName", "Contacts")
        aliases.Add("Contact.Address.Address", "Contacts")
        aliases.Add("Contact.Address.CivicNumber", "Contacts")
        aliases.Add("Contact.Address.ZipCode", "Contacts")
        aliases.Add("Contact.Address.City", "Contacts")
        aliases.Add("Contact.Address.CityCode", "Contacts")

        CreateProjections(crit, aliases)
        crit.SetResultTransformer(New TupleToPropertyResultTransformer(GetType(Protocol), aliases, False))
    End Sub

    Public Function GetFirstContact(uniqueIdProtocol As Guid, ByVal type As String) As ProtocolContactDTO
        Dim protContacts As IList(Of ProtocolContactDTO)
        Dim protManaualContacts As IList(Of ProtocolContactDTO)

        Dim protContactQuery As ICriteria = NHibernateSession.CreateCriteria(Of ProtocolContact)("PC")
        protContactQuery.CreateAlias("PC.Contact", "C", SqlCommand.JoinType.InnerJoin)
        protContactQuery.CreateAlias("PC.Protocol", "P")
        protContactQuery.Add(Restrictions.Eq("PC.ComunicationType", type))
        protContactQuery.Add(Restrictions.Eq("P.Id", uniqueIdProtocol))

        Dim protContactProj As ProjectionList = Projections.ProjectionList()
        protContactProj.Add(Projections.Property("P.Year"), "Year")
        protContactProj.Add(Projections.Property("P.Number"), "Number")
        protContactProj.Add(Projections.Property("C.Description"), "Description")
        protContactProj.Add(Projections.Property("C.ContactType.Id"), "Type")
        protContactProj.Add(Projections.Property("C.SearchCode"), "SearchCode")
        protContactQuery.SetProjection(Projections.Distinct(protContactProj))
        protContactQuery.SetResultTransformer(New AliasToBeanResultTransformer(GetType(ProtocolContactDTO)))
        protContacts = protContactQuery.List(Of ProtocolContactDTO)


        Dim protContactManualQuery As ICriteria = NHibernateSession.CreateCriteria(Of ProtocolContactManual)("PCM")
        protContactManualQuery.CreateAlias("PCM.Protocol", "P")
        protContactManualQuery.Add(Restrictions.Eq("PCM.ComunicationType", type))
        protContactManualQuery.Add(Restrictions.Eq("P.Id", uniqueIdProtocol))

        Dim protContactManualProj As ProjectionList = Projections.ProjectionList()
        protContactManualProj.Add(Projections.Property("P.Year"), "Year")
        protContactManualProj.Add(Projections.Property("P.Number"), "Number")
        protContactManualProj.Add(Projections.Property("PCM.Incremental"), "Incremental")
        protContactManualProj.Add(Projections.Property("PCM.Contact.Description"), "Description")
        protContactProj.Add(Projections.Property("PCM.Contact.ContactType.Id"), "Type")
        protContactManualQuery.SetProjection(Projections.Distinct(protContactManualProj))
        protContactManualQuery.SetResultTransformer(New AliasToBeanResultTransformer(GetType(ProtocolContactDTO)))
        protManaualContacts = protContactManualQuery.List(Of ProtocolContactDTO)

        Return protContacts.Concat(protManaualContacts).OrderBy(Function(x) x.Incremental).FirstOrDefault()
    End Function

    Private Function AggregateProtocolKeys(keys As IList(Of YearNumberCompositeKey)) As IDictionary(Of Short, IList(Of Integer))
        If (keys Is Nothing) OrElse keys.Count <= 0 Then
            Return Nothing
        End If

        Dim aggregated As New Dictionary(Of Short, IList(Of Integer))
        For Each ynck As YearNumberCompositeKey In keys
            If aggregated.ContainsKey(ynck.Year.Value) AndAlso Not aggregated.Item(ynck.Year.Value).Contains(ynck.Number.Value) Then
                aggregated.Item(ynck.Year.Value).Add(ynck.Number.Value)
                Continue For
            End If
            Dim numbers As New List(Of Integer)
            numbers.Add(ynck.Number.Value)
            aggregated.Add(ynck.Year.Value, numbers)
        Next
        Return aggregated
    End Function

    Private Function GetMainProtocolContacts(aggregated As ICollection(Of Guid)) As IList(Of ProtocolContact)
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(Of ProtocolContact)("PC")
        With crit
            .SetFetchMode("Contact", FetchMode.Eager)
            .SetFetchMode("Contact.Address.PlaceName", FetchMode.Eager)
            .CreateAlias("PC.Protocol", "P")
            ' Se il protocollo è in uscita recupero solo i destinatari, altrimenti recupero i mittenti.
            Dim dcComunicationType As DetachedCriteria = DetachedCriteriaComunicationType("P.Id")
            .Add(Restrictions.EqProperty("PC.ComunicationType", Projections.SubQuery(dcComunicationType)))

            Dim disj As Disjunction = New Disjunction()
            Dim conj As Conjunction = New Conjunction()
            conj.Add(Restrictions.In("P.Id", aggregated.ToArray()))
            disj.Add(conj)

            .Add(disj)

            .AddOrder(Order.Asc("P.Year"))
            .AddOrder(Order.Asc("P.Number"))
            .AddOrder(Order.Asc("PC.Contact.Id"))

            Return .List(Of ProtocolContact)()
        End With
    End Function

    Private Function GetMainProtocolContactManuals(aggregated As ICollection(Of Guid)) As IList(Of ProtocolContactManual)
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(Of ProtocolContactManual)("PCM")
        With crit
            .SetFetchMode("Contact", FetchMode.Eager)
            .SetFetchMode("Contact.Address.PlaceName", FetchMode.Eager)
            .CreateAlias("PCM.Protocol", "P")

            ' Se il protocollo è in uscita recupero solo i destinatari, altrimenti recupero i mittenti.
            Dim dcComunicationType As DetachedCriteria = DetachedCriteriaComunicationType("P.Id")
            .Add(Restrictions.EqProperty("PCM.ComunicationType", Projections.SubQuery(dcComunicationType)))

            Dim disj As New Disjunction()
            Dim conj As Conjunction = New Conjunction()
            conj.Add(Restrictions.In("P.Id", aggregated.ToArray()))
            disj.Add(conj)

            .Add(disj)

            .AddOrder(Order.Asc("P.Year"))
            .AddOrder(Order.Asc("P.Number"))
            .AddOrder(Order.Asc("PCM.Incremental"))

            Return .List(Of ProtocolContactManual)()
        End With
    End Function

    Private Function DetachedCriteriaComunicationType(uniqueIdProtocolPropertyDescriptor As String) As DetachedCriteria
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of Protocol)("Protocol")
        dc.Add(Restrictions.EqProperty(uniqueIdProtocolPropertyDescriptor, "Protocol.Id"))
        dc.SetProjection(Projections.Conditional(Restrictions.Eq("Protocol.Type.Id", 1),
                                                 Projections.Constant("D"), Projections.Constant("M")))
        Return dc
    End Function

    Public Function GetMainContacts(keys As IList(Of Guid)) As IDictionary(Of Guid, Object)
        If (keys Is Nothing) OrElse keys.Count <= 0 Then
            Return Nothing
        End If

        Dim protocolContacts As IList(Of ProtocolContact) = GetMainProtocolContacts(keys)
        Dim protocolContactManuals As IList(Of ProtocolContactManual) = GetMainProtocolContactManuals(keys)

        If ((protocolContacts Is Nothing) OrElse protocolContacts.Count <= 0) AndAlso ((protocolContactManuals Is Nothing) OrElse protocolContactManuals.Count <= 0) Then
            Return Nothing
        End If

        Dim merged As New Dictionary(Of Guid, Object)
        For Each pc As ProtocolContact In protocolContacts
            If Not merged.ContainsKey(pc.Protocol.Id) Then
                merged.Add(pc.Protocol.Id, pc)
            End If
        Next
        For Each pcm As ProtocolContactManual In protocolContactManuals
            If Not merged.ContainsKey(pcm.Protocol.Id) Then
                merged.Add(pcm.Protocol.Id, pcm)
            End If
        Next
        Return merged
    End Function

    Public Function GetJournalPrint(ByVal idContainers As String, ByVal dateFrom As Date?, ByVal dateTo As Date?, ByVal idStatus As Integer?) As IList(Of Protocol)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Category", "C", SqlCommand.JoinType.InnerJoin)

        If dateFrom.HasValue Then
            criteria.Add(Expression.Sql(NHibernateHelper.GreaterThanOrEqualToDateIsoFormat("RegistrationDate", dateFrom.Value)))
        End If

        If dateTo.HasValue Then
            criteria.Add(Expression.Sql(NHibernateHelper.LessThanOrEqualToDateIsoFormat("RegistrationDate", dateTo.Value)))
        End If

        If idStatus.HasValue Then
            criteria.Add(Restrictions.Eq("IdStatus", idStatus.Value))
        End If

        criteria.Add(Expression.In("Container.Id", StringHelper.ConvertStringToList(Of Integer)(idContainers, ","c)))

        criteria.AddOrder(Order.Asc("Year"))
        criteria.AddOrder(Order.Asc("Number"))

        Return criteria.List(Of Protocol)()
    End Function

    ''' <summary> Ritorna il numero di pratiche collegate a un determinato protocollo. </summary>
    ''' <param name="year">Anno del protocollo</param>
    ''' <param name="number">Numero del protocollo</param>
    Public Function GetLinkedDocumentCount(ByVal year As Short, ByVal number As Integer) As Integer
        criteria = NHibernateSessionManager.Instance.GetSessionFrom("DocmDB").CreateCriteria(GetType(DocumentObject))
        criteria.Add(Restrictions.Eq("idObjectType", "LP"))

        Dim protocolToken As String = String.Concat(year, "|", String.Format("{0:0000000}", number))
        criteria.Add(Expression.Like("Link", protocolToken, MatchMode.Start))

        Return criteria.List.Count
    End Function

    Public Function GetLastProtocolByYear(year As Short) As Protocol
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Year", year))
        criteria.AddOrder(Order.Desc("Number"))
        criteria.SetMaxResults(1)
        Return criteria.UniqueResult(Of Protocol)
    End Function
End Class