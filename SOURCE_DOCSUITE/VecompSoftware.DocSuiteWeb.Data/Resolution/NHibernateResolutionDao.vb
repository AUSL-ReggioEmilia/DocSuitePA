Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports NHibernate.Transform
Imports VecompSoftware.Helpers
Imports NHibernate
Imports VecompSoftware.Helpers.NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao
Imports VecompSoftware.Helpers.ExtensionMethods
Imports NHibernate.SqlCommand
Imports System.Data.Common

Public Class NHibernateResolutionDao
    Inherits BaseNHibernateDao(Of Resolution)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetCountByContainer(ByVal Container As Container) As Long
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Container", Container))
        criteria.SetProjection(Projections.RowCountInt64())
        Return criteria.UniqueResult(Of Long)()
    End Function

    Public Function GetByYearAndNumber(year As Short, number As Integer) As Resolution
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Year", year))
        criteria.Add(Restrictions.Eq("Number", number))
        Return criteria.UniqueResult(Of Resolution)()
    End Function

    Public Function GetByUniqueId(uniqueId As Guid) As Resolution
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("UniqueId", uniqueId))
        Return criteria.UniqueResult(Of Resolution)()
    End Function

    Public Function GetByYearNumberType(year As Short, number As Integer, type As Short) As Resolution
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Year", year))
        criteria.Add(Restrictions.Eq("Number", number))
        criteria.CreateAlias("Type", "T")
        criteria.Add(Restrictions.Eq("T.Id", type))
        Return criteria.UniqueResult(Of Resolution)()
    End Function

    Public Function GetCountByCategory(ByVal Category As Category) As Long
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Category", Category))
        criteria.SetProjection(Projections.RowCountInt64())
        Return criteria.UniqueResult(Of Long)()
    End Function

    Public Function GetCountBySubCategory(ByVal Category As Category) As Long
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("SubCategory", Category))
        criteria.SetProjection(Projections.RowCountInt64())
        Return criteria.UniqueResult(Of Long)()
    End Function

    Public Function CheckPrevServiceNumberSequence(ByVal IdResl As Integer, ByVal Year As Short, ByVal ServiceNumber As String, ByVal AdoptionDate As DateTime, type As Short) As Integer

        Dim Tokens As String() = ServiceNumber.Split("/"c)
        Dim codice As String = String.Empty
        Dim numero As Integer = 0
        Dim query As IQuery

        If Tokens.Length = 1 Then
            ' Senza codice servizio
            If Not Integer.TryParse(Tokens(0), numero) Then
                Return 0
            End If

            ' Ricerca solo per le Delibere 
            query = NHibernateSession.CreateQuery("select cast(r.ServiceNumber as int) " &
            "from Resolution r " &
            "where r.Year = :Year " &
            "and r.Id <> :IdResl " &
            "and r.AdoptionDate < :AdoptionDate " &
            "and isnumeric(r.ServiceNumber) = 1 " &
            "and cast(r.ServiceNumber as int) > :Number " &
            "and r.Status = 0 " &
            "and r.Type = 0" & type &
            "order by cast(r.ServiceNumber as int) desc")

        Else
            ' Con codice servizio
            codice = Tokens(0)
            If Not Integer.TryParse(Tokens(1), numero) Then
                Return 0
            End If
            query = NHibernateSession.CreateQuery("select cast(substring(r.ServiceNumber, " & codice.Length + 2 & ", 255) as int) " &
            "from Resolution r " &
            "where  r.ServiceNumber like :Service " &
            "and r.Year = :Year " &
            "and r.Id <> :IdResl " &
            "and r.AdoptionDate < :AdoptionDate and cast(substring(r.ServiceNumber, " & codice.Length + 2 & ", 255) as int) > :Number  " &
            "and r.Status = 0 " &
            "order by cast(substring(r.ServiceNumber,  " & codice.Length + 2 & ", 255) as int) desc")
        End If

        query.SetParameter("AdoptionDate", AdoptionDate)
        query.SetParameter("IdResl", IdResl)
        If Not String.IsNullOrEmpty(codice) Then
            query.SetParameter("Service", codice & "/%")
            query.SetParameter("Number", numero)
        Else
            query.SetParameter("Number", numero.ToString())
        End If
        query.SetParameter("Year", Year)
        query.SetMaxResults(1)

        Dim results As IList(Of Integer) = query.List(Of Integer)()
        If results.Count > 0 Then
            Return results.First()
        Else
            Return -1
        End If

    End Function

    Public Function GetNextFreeServiceNumber(IdResl As Integer, Year As Short, ByVal RoleServiceNumber As String, type As Short?) As Integer

        If String.IsNullOrEmpty(RoleServiceNumber) Then
            criteria = NHibernateSession.CreateCriteria(persitentType)
            criteria.Add(Restrictions.Not(Restrictions.Eq("Id", IdResl)))
            criteria.Add(Restrictions.Eq("Year", Year))

            If DocSuiteContext.Current.ResolutionEnv.IncrementalNumberEnabled AndAlso type.HasValue Then
                criteria.Add(Restrictions.Eq("Type.Id", type.Value))
            Else
                criteria.Add(Restrictions.Eq("Type.Id", ResolutionType.IdentifierDelibera))
            End If

            criteria.SetProjection(Projections.Max("ServiceNumber"))
            Dim result As String = criteria.UniqueResult(Of String)
            If String.IsNullOrEmpty(result) Then
                Return 1
            Else
                Return Convert.ToInt32(result) + 1
            End If

        Else
            Dim query As IQuery
            query = NHibernateSession.CreateQuery("select r.ServiceNumber " &
                "from Resolution r " &
                "where r.ServiceNumber is not NULL " &
                "and r.Year = :Year " &
                "and r.Id <> :IdResl " &
                "and r.Status = 0 " &
                "and r.ServiceNumber like :RoleServiceNumber " &
                "order by r.ServiceNumber desc")
            query.SetParameter("Year", Year)
            query.SetParameter("IdResl", IdResl)
            query.SetParameter("RoleServiceNumber", RoleServiceNumber & "/%")
            query.SetMaxResults(1)
            Dim results As IList = query.List()
            If results.Count > 0 Then
                Return Convert.ToInt32(results(0).ToString.Split("/"c)(1)) + 1
            Else
                Return 1
            End If
        End If
    End Function

    Public Function CheckFollowingServiceNumberSequence(ByVal IdResl As Integer, ByVal Year As Short, ByVal ServiceNumber As String, ByVal AdoptionDate As Date, type As Short) As Integer

        Dim Tokens As String() = ServiceNumber.Split("/"c)

        Dim codice As String = String.Empty
        Dim numero As Integer = 0
        Dim query As IQuery
        If Tokens.Length = 1 Then
            ' Senza codice servizio
            If Not Integer.TryParse(Tokens(0), numero) Then
                Return 0
            End If

            ' Ricerca solo per le Delibere 
            query = NHibernateSession.CreateQuery("select r.ServiceNumber " &
            "from Resolution r " &
            "where r.Year = :Year " &
            "and r.Id <> :IdResl " &
            "and r.AdoptionDate > :AdoptionDate  " &
            "and isnumeric(r.ServiceNumber) = 1 " &
            "and cast(r.ServiceNumber as int) < :Number " &
            "and r.Status = 0 " &
            "and r.Type = 0" & type &
            "order by cast(r.ServiceNumber as int) desc")

        Else
            ' Con codice servizio
            codice = Tokens(0)
            If Not Integer.TryParse(Tokens(1), numero) Then
                Return 0
            End If
            query = NHibernateSession.CreateQuery("select cast(substring(r.ServiceNumber, " & codice.Length + 2 & ", 255) as int) " &
            "from Resolution r " &
            "where  r.ServiceNumber like :Service " &
            "and r.Year = :Year " &
            "and r.Id <> :IdResl " &
            "and r.AdoptionDate > :AdoptionDate and cast(substring(r.ServiceNumber, " & codice.Length + 2 & ", 255) as int) < :Number " &
            "and r.Status = 0 " &
             "order by cast(substring(r.ServiceNumber,  " & codice.Length + 2 & ", 255) as int) asc")
        End If

        query.SetParameter("AdoptionDate", AdoptionDate)
        query.SetParameter("IdResl", IdResl)
        If Not String.IsNullOrEmpty(codice) Then
            query.SetParameter("Service", codice & "/%")
            query.SetParameter("Number", numero)
        Else
            query.SetParameter("Number", numero.ToString())
        End If
        query.SetParameter("Year", Year)
        query.SetMaxResults(1)

        Dim results As IList(Of Integer) = query.List(Of Integer)()
        If results.Count > 0 Then
            Return results.First()
        Else
            Return -1
        End If

    End Function

    Public Function CheckServiceNumber(ByVal year As Short, ByVal serviceNumber As String, ByVal id As Integer, type As Short) As Boolean
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Not(Restrictions.Eq("Id", id)))
        criteria.Add(Restrictions.Eq("Year", year))
        criteria.Add(Restrictions.Eq("ServiceNumber", serviceNumber))
        criteria.Add(Restrictions.Eq("Type.Id", type))
        criteria.Add(Restrictions.Eq("Status.Id", CType(ResolutionStatusId.Attivo, Short)))
        criteria.SetProjection(Projections.Count("Id"))
        Return criteria.UniqueResult(Of Integer)() = 0
    End Function

    ''' <summary>
    ''' Restituisce un oggetto Resolution cercato per IdResolution oppure ServiceNumber oppure per Year e Number
    ''' </summary>
    Public Function GetByIdOrAdoptionData(ByVal type As Nullable(Of Short), ByVal Id As Integer?, ByVal inclusiveNumber As String, ByVal Year As Short?) As Resolution
        criteria = NHibernateSession.CreateCriteria(persitentType)

        If type.HasValue Then
            criteria.Add(Restrictions.Eq("Type.Id", type))
        End If

        Select Case True
            Case Id.HasValue
                criteria.Add(Restrictions.Eq("Id", Id.Value))
            Case (Not String.IsNullOrEmpty(inclusiveNumber))
                criteria.Add(Restrictions.Eq("InclusiveNumber", inclusiveNumber))
        End Select

        Dim result As IList(Of Resolution) = criteria.List(Of Resolution)()
        If result.Count > 0 Then
            Return result(0)
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' Recupera tutte le informazioni dai LOG per visualizzare le informazioni di diario di un utente
    ''' </summary>
    ''' <param name="pDateFrom">Data da cui reperire le informazioni di log</param>
    ''' <param name="pDateTo">Data fino a cui reperire le informazioni di log</param>
    Function GetUserResolutionDiary(ByVal pDateFrom As Date, ByVal pDateTo As Date) As ICollection(Of UserDiary)

        Dim qQuery As IQuery = NHibernateSession.GetNamedQuery("ReslUserDiary")

        qQuery.SetResultTransformer(Transformers.AliasToBean(New UserDiary().GetType()))
        qQuery = qQuery.SetParameter("SystemUser", DocSuiteContext.Current.User.FullUserName)
        qQuery = qQuery.SetParameter("LogDateFrom", pDateFrom.BeginOfTheDay().ToVecompSoftwareString())
        qQuery = qQuery.SetParameter("LogDateTo", pDateTo.EndOfTheDay().ToVecompSoftwareString())

        Return qQuery.List(Of UserDiary)()

    End Function

    ''' <summary>
    ''' Metodo che ritorna una lista di resolution partendo da una lista di id
    ''' </summary>
    Function GetResolutions(ByVal keyList As IList(Of Integer)) As IList(Of Resolution)
        Return GetResolutions(keyList, String.Empty, True)
    End Function

    Function GetResolutions(ByVal keyList As IList(Of Integer), ByVal orderfield As String, ByVal asc As Boolean) As IList(Of Resolution)
        criteria = NHibernateSession.CreateCriteria(persitentType, "R")

        criteria.Add(Restrictions.In("R.Id", keyList.ToArray()))
        If Not String.IsNullOrEmpty(orderfield) Then
            If asc Then
                criteria.AddOrder(Order.Asc(orderfield))
            Else
                criteria.AddOrder(Order.Desc(orderfield))
            End If
        End If

        Return criteria.List(Of Resolution)()
    End Function

    ''' <summary>
    ''' Metodo che ritorna una lista di resolution partendo da una lista di id
    ''' </summary>
    ''' <param name="keyList">Lista di id da trovare</param>
    ''' <param name="GroupBy">Properties da mettere in GROUP BY (separate da virgola, senza alias)</param>
    ''' <param name="OrderByAsc">Properties da mettere in ORDER BY [ASC only] (separate da virgola, senza alias)</param>
    Function GetResolutionsLetter(ByVal keyList As IList(Of Integer), Optional ByVal GroupBy As String = "", Optional ByVal GroupByAliases As String = "", Optional ByVal OrderByAsc As String = "", Optional ByVal OnlyGestione As Boolean = False) As IList(Of ResolutionLetter)
        criteria = NHibernateSession.CreateCriteria(persitentType, "R")

        Dim disju As Disjunction = Expression.Disjunction()
        For Each key As Integer In keyList
            disju.Add(Restrictions.Eq("R.Id", key))
        Next
        criteria.Add(disju)

        If OnlyGestione Then
            criteria.Add(Restrictions.Eq("R.OCManagement", True))
        End If

        'Aggiungo gli ordinamenti
        If Not String.IsNullOrEmpty(OrderByAsc) Then
            Dim orders As String() = OrderByAsc.Split(","c)
            For Each o As String In orders
                If Not String.IsNullOrEmpty(o.Trim()) Then
                    criteria.AddOrder(Order.Asc("R." & o.Trim()))
                End If
            Next
        End If

        criteria.CreateAlias("ResolutionContactProposers", "PC")
        criteria.CreateAlias("PC.Contact", "C")
        criteria.CreateAlias("Container", "Cont")

        'Aggiungo i raggruppamenti
        Dim projList As ProjectionList = Projections.ProjectionList()
        If Not String.IsNullOrEmpty(GroupBy) Then
            Dim groups As String() = GroupBy.Split(","c)
            Dim aliases As String() = GroupByAliases.Split(","c)
            For i As Integer = 0 To groups.Length - 1
                If Not String.IsNullOrEmpty(groups(i).Trim()) Then
                    projList.Add(Projections.Property(groups(i).Trim()), aliases(i).Trim())
                End If
            Next
        Else
            projList.Add(Projections.Property("Id"), "Id")
            projList.Add(Projections.Property("Year"), "Year")
            projList.Add(Projections.Property("Number"), "Number")
            projList.Add(Projections.Property("ServiceNumber"), "ServiceNumber")
            projList.Add(Projections.Property("Container.Id"), "ContainerId")
            projList.Add(Projections.Property("AdoptionDate"), "AdoptionDate")
            projList.Add(Projections.Property("PublishingDate"), "PublishingDate")
            projList.Add(Projections.Property("EffectivenessDate"), "EffectivenessDate")
            projList.Add(Projections.Property("ResolutionObject"), "ResolutionObject")
            projList.Add(Projections.Property("C.Code"), "ProposerCode")
            projList.Add(Projections.Property("Cont.HeadingLetter"), "HeadingLetter")
            projList.Add(Projections.Property("OCSupervisoryBoard"), "OCSupervisoryBoard")
            projList.Add(Projections.Property("OCRegion"), "OCRegion")
            projList.Add(Projections.Property("OCManagement"), "OCManagement")
            projList.Add(Projections.Property("OCOther"), "OCOther")
            projList.Add(Projections.Property("OtherDescription"), "OtherDescription")
        End If

        criteria.SetProjection(Projections.Distinct(projList))
        criteria.SetResultTransformer(New NHibernate.Transform.AliasToBeanResultTransformer(GetType(ResolutionLetter)))

        Return criteria.List(Of ResolutionLetter)()
    End Function

    Function GetMinMaxWorkflowDate(ByVal keyList As IList(Of Integer), ByVal [property] As String, Optional ByVal minmax As String = "MAX") As Date?
        criteria = NHibernateSession.CreateCriteria(persitentType, "R")

        Dim disju As Disjunction = Restrictions.Disjunction()
        For Each key As Integer In keyList
            disju.Add(Restrictions.Eq("R.Id", key))
        Next
        criteria.Add(disju)

        Select Case minmax.ToUpper()
            Case "MAX"
                criteria.SetProjection(Projections.Max("R." & [property]))
            Case "MIN"
                criteria.SetProjection(Projections.Min("R." & [property]))
        End Select

        Return criteria.UniqueResult(Of Date?)()
    End Function

    Function GetResolutionsOrderProposerCode(ByVal keyList As IList(Of Integer)) As IList(Of Resolution)
        criteria = NHibernateSession.CreateCriteria(persitentType, "R")

        criteria.CreateAlias("R.ResolutionContacts", "ResContacts", SqlCommand.JoinType.LeftOuterJoin)
        criteria.Add(Restrictions.Eq("ResContacts.Id.ComunicationType", "P"))
        criteria.CreateAlias("ResContacts.Contact", "RContact", SqlCommand.JoinType.LeftOuterJoin)

        Dim disju As Disjunction = Expression.Disjunction()
        For Each key As Integer In keyList
            disju.Add(Restrictions.Eq("R.Id", key))
        Next
        criteria.Add(disju)

        criteria.AddOrder(Order.Asc("RContact.Code"))
        criteria.AddOrder(Order.Asc("R.Id"))
        criteria.SetResultTransformer(New DistinctRootEntityResultTransformer())

        Return criteria.List(Of Resolution)()
    End Function

    ''' <summary> Recupera le delibere da pubblicare. </summary>
    Public Function GetResolutionToPublicate() As IList(Of Resolution)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Type.Id", Convert.ToInt16(1)))
        criteria.Add(Restrictions.IsNull("PublishingDate"))
        criteria.Add(Expression.Sql("{alias}.SupervisoryBoardWarningDate <= GetDate() - 5"))
        criteria.AddOrder(Order.Asc("SupervisoryBoardWarningDate"))

        Return criteria.List(Of Resolution)()
    End Function

    ''' <summary> Ritorna atti per il collegio sindacale </summary>
    Public Function GetBySupervisoryBoardDate([from] As DateTime, [to] As DateTime?, type As ResolutionType) As IList(Of Resolution)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Type", type))
        criteria.Add(Restrictions.Ge("SupervisoryBoardWarningDate", [from]))
        If [to].HasValue Then
            criteria.Add(Restrictions.Le("SupervisoryBoardWarningDate", [to].Value))
        End If

        Return criteria.List(Of Resolution)()
    End Function

    Public Function GetBySupervisoryBoardCollaborationId(collaborationId As Integer) As ICollection(Of Resolution)
        Dim query As IQueryOver(Of Resolution, Resolution) = NHibernateSession.QueryOver(Of Resolution)()
        query = query.Where(Function(f) f.SupervisoryBoardProtocolCollaboration IsNot Nothing) _
                     .And(Function(f) f.SupervisoryBoardProtocolCollaboration.Value = collaborationId)

        Return query.List(Of Resolution)()
    End Function

    Public Function GetPublicated(ByVal [from] As DateTime?, ByVal [to] As DateTime, ByVal [adoptionFrom] As DateTime?, ByVal [adoptionTo] As DateTime?, ByVal containerId As Integer?, ByVal reslType As Short?) As IList(Of Resolution)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        'criteria.Add(Expression("_webState", Convert.ToInt32(1)))
        criteria.Add(Restrictions.IsNull("WebRevokeDate"))
        criteria.Add(Restrictions.IsNull("UltimaPaginaDate"))
        criteria.Add(Restrictions.IsNotNull("PublishingDate"))
        criteria.Add(Restrictions.IsNotNull("EffectivenessDate"))

        If [from].HasValue Then
            criteria.Add(Restrictions.Ge("PublishingDate", [from]))
        End If
        criteria.Add(Restrictions.Le("PublishingDate", [to]))

        If [adoptionFrom].HasValue Then
            criteria.Add(Restrictions.Ge("AdoptionDate", [adoptionFrom]))
        End If

        If [adoptionTo].HasValue Then
            criteria.Add(Restrictions.Le("AdoptionDate", adoptionTo))
        End If

        If (Not containerId Is Nothing) Then
            criteria.Add(Restrictions.Eq("Container.Id", containerId))
        End If

        If (Not reslType Is Nothing) Then
            criteria.Add(Restrictions.Eq("Type.Id", reslType))
        End If

        criteria.CreateAlias("File", "F")
        criteria.Add(Restrictions.IsNotNull("F.IdUltimaPagina"))
        criteria.AddOrder(Order.Asc("Number"))
        Return criteria.List(Of Resolution)()
    End Function

    ''' <summary>
    ''' Recupera le delibere da rendere esecutive
    ''' </summary>
    ''' <param name="region">True: inviate alla region, False: non inviate alla regione</param>
    ''' <returns>Elenco di delibere da rendere esecutive</returns>
    ''' <remarks></remarks>
    Public Function GetResolutionToExecutive(ByVal region As Boolean) As IList(Of Resolution)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Type.Id", Convert.ToInt16(1)))
        criteria.Add(Restrictions.IsNull("EffectivenessDate"))
        Select Case region
            Case True
                criteria.Add(Expression.Sql("{alias}.ConfirmDate <= GetDate()- 40"))
                criteria.Add(Expression.IsNotNull("WarningDate"))
                criteria.AddOrder(Order.Asc("ConfirmDate"))
            Case False
                criteria.Add(Expression.Sql("{alias}.PublishingDate <= GetDate() - 10"))
                criteria.Add(Restrictions.IsNull("WarningDate"))
                criteria.AddOrder(Order.Asc("PublishingDate"))
        End Select

        Return criteria.List(Of Resolution)()
    End Function

    Public Function SqlIncremental(ByVal Table As String, ByVal idResolution As Integer, ByRef id As Integer) As Boolean
        Dim query As String = "SELECT MAX(Incremental) AS Progressivo FROM " & Table & " " &
              "WHERE idResolution=" & idResolution
        Try
            id = NHibernateSession.CreateQuery(query).UniqueResult(Of Integer)()
            Return True
        Catch ex As Exception
            id = 0
            Return False
        End Try
    End Function

    Public Function SqlActiveIncremental(ByVal Table As String, ByVal idResolution As Integer, ByRef idFather As Integer) As Boolean
        Dim query As String = "SELECT (Incremental) AS Progressivo FROM " & Table & " " &
              "WHERE idResolution=" & idResolution & " " &
              "AND IsActive=1"
        Try
            idFather = NHibernateSession.CreateQuery(query).UniqueResult(Of Integer)()
            Return True
        Catch ex As Exception
            idFather = 0
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Metodo che aggiorna i link di una lista di resolution
    ''' </summary>
    ''' <param name="ProtocolLinkType">Il nome del campo da aggiornare</param>
    ''' <param name="IdResolutionList">Stringa con gli id delle resolution (separate da virgole)</param>
    ''' <param name="Protocol">Valore da aggiornare</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateProtocolLink(ByVal ProtocolLinkType As String, ByVal IdResolutionList As String, ByVal Protocol As String) As Boolean
        Dim transaction As ITransaction = Nothing

        Dim bRet As Boolean = False

        Try
            Dim sqlStatement As String = String.Format("UPDATE Resolution SET {0}='{1}' WHERE idResolution IN ({2}) ", ProtocolLinkType, Protocol, IdResolutionList)

            SessionFactoryName = "ReslDB"
            transaction = NHibernateSession.BeginTransaction(IsolationLevel.ReadCommitted)
            NHibernateHelper.ExecuteNonQuery(sqlStatement, NHibernateSession.Connection, transaction)
            bRet = True
        Catch
            bRet = False
        End Try

        Return bRet
    End Function

    Public Function UpdateProposerWarningDate(ByVal IdResolutionList As String, ByVal Data As Date?, ByVal UserConnected As String) As Boolean
        Dim transaction As ITransaction = Nothing

        Dim bRet As Boolean
        Dim sql As String = String.Empty
        Dim sqlStatement As String

        Try
            '-- Spedizione Collegio Sindacale
            If Data.HasValue Then
                sql = "ProposerWarningDate = '" & String.Format("{0:yyyyMMdd}", Data) & "',"
                sql &= "ProposerWarningUser='" & UserConnected & "'"
            Else
                sql = "ProposerWarningDate=NULL,"
                sql &= "ProposerWarningUser=NULL"
            End If

            If Not String.IsNullOrEmpty(sql) Then
                sqlStatement = "UPDATE Resolution SET " & sql &
                      " WHERE idResolution IN (" & IdResolutionList & ") "
                SessionFactoryName = "ReslDB"
                transaction = NHibernateSession.BeginTransaction(IsolationLevel.ReadCommitted)
                NHibernateHelper.ExecuteNonQuery(sqlStatement, NHibernateSession.Connection, transaction)
            Else
                bRet = False
            End If
            bRet = True
        Catch ex As Exception
            bRet = False
        End Try

        Return bRet
    End Function

    Public Function UpdateEffectivenessDate(ByVal IdResolutionList As String, ByVal DataEsecutivita As Date?, ByVal UserConnected As String) As Boolean

        Dim transaction As ITransaction = Nothing

        Dim bRet As Boolean = False
        Dim sql As String = String.Empty
        Dim sqlStatement As String
        Dim i As Integer

        Try
            SessionFactoryName = "ReslDB"
            transaction = NHibernateSession.BeginTransaction(IsolationLevel.ReadCommitted)

            '-- Esecutività
            Dim command As IDbCommand = New SqlClient.SqlCommand()
            command.Connection = NHibernateSession.Connection
            transaction.Enlist(command)

            If DataEsecutivita.HasValue Then
                sql = "EffectivenessDate = '" & String.Format("{0:yyyyMMdd}", DataEsecutivita) & "',"
                sql &= "EffectivenessUser='" & UserConnected & "'"
            Else
                sql = "EffectivenessDate=NULL,"
                sql &= "EffectivenessUser=NULL"
            End If

            If sql <> "" Then
                sqlStatement = "UPDATE Resolution SET " & sql &
                      " WHERE idResolution IN (" & IdResolutionList & ") " &
                      " AND OCRegion=0 "

                command.CommandText = sqlStatement
                i = command.ExecuteNonQuery()
            End If

            '-- Aggiornamento ResolutionWorkflow x Pubblicazione
            Dim inc As Short
            Dim incFather As Short
            Dim nextstep As Short
            Dim keylist As String() = IdResolutionList.Split(","c)

            For Each key As String In keylist
                Dim idResolution As Integer = Integer.Parse(key)

                Dim daoWork As New NHibernateResolutionWorkflowDao("ReslDB")
                incFather = daoWork.GetActiveIncremental(idResolution, 1S)
                inc = daoWork.GetMaxIncremental(idResolution) + 1S
                nextstep = daoWork.GetActiveStep(idResolution) + 1S
                If incFather <> 0 Then
                    command = New SqlClient.SqlCommand()
                    command.Connection = NHibernateSession.Connection
                    transaction.Enlist(command)

                    sql = "LastChangedDate = '" & String.Format("{0:yyyyMMdd}", GetServerDate()) & "',"
                    sql &= "LastChangedUser='" & UserConnected & "',"

                    sqlStatement = "UPDATE ResolutionWorkflow " &
                          " SET " & sql & " IsActive=0 " &
                          " WHERE idResolution=" & idResolution & " AND Incremental=" & incFather & " "

                    command.CommandText = sqlStatement
                    i = command.ExecuteNonQuery()
                    bRet = i <> 0
                End If

                command = New SqlClient.SqlCommand()
                command.Connection = NHibernateSession.Connection
                transaction.Enlist(command)

                sql = idResolution & ","
                sql &= inc & ","
                sql &= If(incFather <> 0, incFather, "NULL") & ","
                sql &= nextstep & ","
                sql &= "1,"
                sql &= "'" & UserConnected & "',"
                sql &= "'" & String.Format("{0:yyyyMMdd}", GetServerDate()) & "'"

                sqlStatement = "INSERT INTO ResolutionWorkflow " &
                      " (idResolution,Incremental,IncrementalFather, " &
                      " Step,IsActive,RegistrationUser,RegistrationDate ) " &
                      " VALUES (" & sql & ")"

                command.CommandText = sqlStatement
                i = command.ExecuteNonQuery()
            Next

            transaction.Commit()
            bRet = True

        Catch ex As Exception
            transaction.Rollback()
        End Try
        Return bRet
    End Function

    Public Function UpdatePublishingDate(ByVal IdResolutionList As String, ByVal DataPubblicazione As Date?, ByVal UserConnected As String) As Boolean
        Dim transaction As ITransaction = Nothing

        Dim bRet As Boolean = False
        Dim sql As String = String.Empty
        Dim sqlStatement As String
        Dim i As Integer

        Try
            SessionFactoryName = "ReslDB"
            transaction = NHibernateSession.BeginTransaction(IsolationLevel.ReadCommitted)

            '-- Pubblicazione
            Dim command As IDbCommand = New SqlClient.SqlCommand()
            command.Connection = NHibernateSession.Connection
            transaction.Enlist(command)

            If DataPubblicazione.HasValue Then
                sql = "PublishingDate = '" & String.Format("{0:yyyyMMdd}", DataPubblicazione) & "',"
                sql &= "PublishingUser='" & UserConnected & "'"
                If DocSuiteContext.Current.ResolutionEnv.WebPublishEnabled Then
                    sql &= ",CheckPublication='1'"
                End If
            Else
                sql = "PublishingDate=NULL,"
                sql &= "PublishingUser=NULL"
                If DocSuiteContext.Current.ResolutionEnv.WebPublishEnabled Then
                    sql &= ",CheckPublication='0'"
                End If
            End If

            If sql <> "" Then
                sqlStatement = String.Format("UPDATE Resolution SET {0} WHERE idResolution IN ({1}) ", sql, IdResolutionList)

                command.CommandText = sqlStatement
                i = command.ExecuteNonQuery()
            End If

            '-- Aggiornamento ResolutionWorkflow x Pubblicazione
            Dim inc As Short
            Dim incFather As Short
            Dim nextstep As Short
            Dim keylist As String() = IdResolutionList.Split(","c)

            For Each key As String In keylist
                Dim idResolution As Integer = Integer.Parse(key)

                Dim daoWork As New NHibernateResolutionWorkflowDao("ReslDB")
                incFather = daoWork.GetActiveIncremental(idResolution, 1S)
                inc = daoWork.GetMaxIncremental(idResolution) + 1S
                nextstep = daoWork.GetActiveStep(idResolution) + 1S
                If incFather <> 0 Then
                    command = New SqlClient.SqlCommand()
                    command.Connection = NHibernateSession.Connection
                    transaction.Enlist(command)

                    sql = "LastChangedDate = '" & String.Format("{0:yyyyMMdd}", GetServerDate()) & "',"
                    sql &= "LastChangedUser='" & UserConnected & "',"

                    sqlStatement = String.Format("UPDATE ResolutionWorkflow SET {0} IsActive=0 WHERE idResolution={1} AND Incremental={2} ", sql, idResolution, incFather)

                    command.CommandText = sqlStatement
                    i = command.ExecuteNonQuery()
                    bRet = i <> 0
                End If

                command = New SqlClient.SqlCommand()
                command.Connection = NHibernateSession.Connection
                transaction.Enlist(command)

                sql = idResolution & ","
                sql &= inc & ","
                sql &= If(incFather <> 0, incFather, "NULL") & ","
                sql &= nextstep & ","
                sql &= "1,"
                sql &= "'" & UserConnected & "',"
                sql &= "'" & String.Format("{0:yyyyMMdd}", GetServerDate()) & "'"

                sqlStatement = String.Format("INSERT INTO ResolutionWorkflow (idResolution,Incremental,IncrementalFather, Step,IsActive,RegistrationUser,RegistrationDate ) VALUES ({0})", sql)

                command.CommandText = sqlStatement
                i = command.ExecuteNonQuery()
            Next

            transaction.Commit()
            bRet = True

        Catch ex As Exception
            transaction.Rollback()
        End Try
        Return bRet
    End Function

    Public Function GetCalculatedLink(prot As Protocol) As String
        Return String.Format("{0}|{1:0000000}|{2}|{3:dd/MM/yyyy}", prot.Year, prot.Number, prot.Location.Id, prot.RegistrationDate)
    End Function

    Public Function UpdateOC(ByVal IdResolutionList As String,
        ByVal DataSpedCollegio As Date?, ByVal ProtCollegio As Protocol,
        ByVal DataSpedRegione As Date?, ByVal ProtRegione As Protocol,
        ByVal DataSpedGestione As Date?, ByVal ProtGestione As Protocol,
        ByVal UserConnected As String) As Boolean

        Dim transaction As ITransaction = Nothing

        Dim bRet As Boolean = False
        Dim sql As String = String.Empty
        Dim sqlStatement As String
        Dim i As Integer

        Try
            SessionFactoryName = "ReslDB"
            transaction = NHibernateSession.BeginTransaction(IsolationLevel.ReadCommitted)

            '-- Spedizione Collegio Sindacale
            Dim command As DbCommand = New SqlClient.SqlCommand()
            command.Connection = NHibernateSession.Connection
            transaction.Enlist(command)

            If DataSpedCollegio.HasValue Then
                sql = "SupervisoryBoardWarningDate = '" & String.Format("{0:yyyyMMdd}", DataSpedCollegio) & "',"
                sql &= "SupervisoryBoardWarningUser='" & UserConnected & "'"
            Else
                sql = "SupervisoryBoardWarningDate=NULL,"
                sql &= "SupervisoryBoardWarningUser=NULL"
            End If

            If sql <> "" Then
                sqlStatement = "UPDATE Resolution SET " & sql &
                      " WHERE idResolution IN (" & IdResolutionList & ") "

                command.CommandText = sqlStatement
                i = command.ExecuteNonQuery()
            End If

            'Link Protocollo lettera trasmissione adozione Collegio
            If ProtCollegio IsNot Nothing Then
                command = New SqlClient.SqlCommand()
                command.Connection = NHibernateSession.Connection
                transaction.Enlist(command)

                sqlStatement = String.Format("UPDATE Resolution SET SupervisoryBoardProtocolLink='{0}' WHERE idResolution IN ({1}) ", GetCalculatedLink(ProtCollegio), IdResolutionList)

                command.CommandText = sqlStatement
                i = command.ExecuteNonQuery()
            End If

            '-- Spedizione Regione
            command = New SqlClient.SqlCommand()
            command.Connection = NHibernateSession.Connection
            transaction.Enlist(command)

            If DataSpedRegione.HasValue Then
                sql = "WarningDate = '" & String.Format("{0:yyyyMMdd}", DataSpedRegione) & "',"
                sql &= "WarningUser='" & UserConnected & "'"
            Else
                sql = "WarningDate=NULL,"
                sql &= "WarningUser=NULL"
            End If

            If sql <> "" Then
                sqlStatement = "UPDATE Resolution SET " & sql &
                      " WHERE idResolution IN (" & IdResolutionList & ") AND " &
                  "OCRegion=1"

                command.CommandText = sqlStatement
                i = command.ExecuteNonQuery()
            End If

            'Link Protocollo lettera trasmissione adozione Regione
            If ProtRegione IsNot Nothing Then
                command = New SqlClient.SqlCommand()
                command.Connection = NHibernateSession.Connection
                transaction.Enlist(command)

                sqlStatement = String.Format("UPDATE Resolution SET RegionProtocolLink='{0}' WHERE idResolution IN ({1}) AND  OCRegion=1", GetCalculatedLink(ProtRegione), IdResolutionList)

                command.CommandText = sqlStatement
                i = command.ExecuteNonQuery()
            End If

            '-- Controllo di Gestione
            command = New SqlClient.SqlCommand()
            command.Connection = NHibernateSession.Connection
            transaction.Enlist(command)

            If DataSpedGestione.HasValue Then
                sql = "ManagementWarningDate = '" & String.Format("{0:yyyyMMdd}", DataSpedGestione) & "',"
                sql &= "ManagementWarningUser='" & UserConnected & "'"
            Else
                sql = "ManagementWarningDate=NULL,"
                sql &= "ManagementWarningUser=NULL"
            End If

            If sql <> "" Then
                sqlStatement = "UPDATE Resolution SET " & sql &
                      " WHERE idResolution IN (" & IdResolutionList & ") AND " &
                  " OCManagement=1"

                command.CommandText = sqlStatement
                i = command.ExecuteNonQuery()
            End If

            'Link Protocollo lettera trasmissione adozione Gestione
            If ProtGestione IsNot Nothing Then
                command = New SqlClient.SqlCommand()
                command.Connection = NHibernateSession.Connection
                transaction.Enlist(command)

                sqlStatement = String.Format("UPDATE Resolution SET ManagementProtocolLink='{0}'  WHERE idResolution IN ({1}) AND OCManagement=1", GetCalculatedLink(ProtGestione), IdResolutionList)

                command.CommandText = sqlStatement
                i = command.ExecuteNonQuery()
            End If

            transaction.Commit()
            bRet = True

        Catch ex As Exception
            transaction.Rollback()
        End Try
        Return bRet
    End Function

    ''' <summary> Recupera l'ultimo numero atto inserito per un anno antecedente all'anno correntemente in gestione. </summary>
    ''' <param name="year">Anno di cui recuperare l'ultimo numero inserito</param>
    ''' <param name="type">Tipo dell'atto</param>
    Public Function GetMaxResolutionNumber(year As Short, type As Int16) As Integer
        Dim parameterDao As New NHibernateParameterDao("ReslDB")
        Dim lastUsedYear As Short = parameterDao.GetLastUsedResolutionYear()
        If Not year < lastUsedYear Then
            ' ATTENZIONE! La seguente verifica è stata introdotta onde evitare generazioni di chiavi duplicate e conseguente BLOCCO dell'applicazione.
            Throw New ArgumentException("Non è possibile utilizzare la funzione getMaxResolutionNumber per l'anno correntemente in gestione o successivi.")
        End If

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of Resolution)()
        criteria.Add(Restrictions.Eq("Year", year))
        criteria.Add(Restrictions.Eq("Type.Id", type))
        criteria.SetProjection(Projections.Max("Number"))

        Return criteria.UniqueResult(Of Integer)()
    End Function

    Public Function GetResolutionRoles(resl As Resolution) As IList(Of ResolutionRole)

        criteria = NHibernateSession.CreateCriteria(Of ResolutionRole)("RR")
        criteria.CreateAlias("RR.ResolutionRoleType", "RRT", JoinType.InnerJoin)

        criteria.Add(Restrictions.Eq("RR.Id.IdResolution", resl.Id))

        criteria.AddOrder(Order.Asc("RRT.SortOrder"))

        Return criteria.List(Of ResolutionRole)()
    End Function

    Public Function GetPreviousSequenceAdoptionDate(ByVal idResolution As Integer, ByVal roleServiceNumber As String, ByVal resolutionType As Short) As Date
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(Of Resolution)("R")
        crit.Add(Expression.Lt("R.Id", idResolution))
        crit.Add(Restrictions.Eq("R.Type.Id", resolutionType))

        If Not String.IsNullOrEmpty(roleServiceNumber) Then
            crit.Add(Expression.Like("R.ServiceNumber", roleServiceNumber))
        End If
        crit.SetProjection(Projections.Max("R.AdoptionDate"))
        Return crit.UniqueResult(Of Date)()
    End Function

    Public Function GetResolutionLocation(idResolution As Integer) As Location
        Dim reslLocation As Location = Nothing
        Dim reslContainer As Container = Nothing
        Dim locationResult As Location = Nothing

        Dim query As IQueryOver(Of Resolution, Resolution) = NHibernateSession.QueryOver(Of Resolution)()

        Return query.Where(Function(x) x.Id = idResolution) _
            .JoinAlias(Function(x) x.Container, Function() reslContainer) _
            .JoinAlias(Function(x) reslContainer.ReslLocation, Function() reslLocation) _
            .SelectList(Function(slist) slist.[Select](Function(s) reslLocation.Id).WithAlias(Function() locationResult.Id) _
                            .[Select](Function(s) reslLocation.ConsBiblosDSDB).WithAlias(Function() locationResult.ConsBiblosDSDB) _
                            .[Select](Function(s) reslLocation.DocmBiblosDSDB).WithAlias(Function() locationResult.DocmBiblosDSDB) _
                            .[Select](Function(s) reslLocation.Name).WithAlias(Function() locationResult.Name) _
                            .[Select](Function(s) reslLocation.ProtBiblosDSDB).WithAlias(Function() locationResult.ProtBiblosDSDB) _
                            .[Select](Function(s) reslLocation.ReslBiblosDSDB).WithAlias(Function() locationResult.ReslBiblosDSDB)) _
            .TransformUsing(Transformers.AliasToBean(Of Location)()) _
            .SingleOrDefault(Of Location)()

    End Function

    Public Function GetActualToPublicate(serviceNumber As String, type As Short, workflowType As String) As Resolution
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.IsNull("PublishingDate"))
        criteria.Add(Restrictions.IsNotNull("AdoptionDate"))
        If type = ResolutionType.IdentifierDetermina Then
            criteria.Add(Restrictions.Like("ServiceNumber", serviceNumber, MatchMode.Start))
        End If
        criteria.Add(Restrictions.Eq("Status.Id", CType(ResolutionStatusId.Attivo, Short)))
        criteria.Add(Restrictions.Eq("Type.Id", type))
        criteria.Add(Restrictions.Eq("WorkflowType", workflowType))
        criteria.AddOrder(Order.Asc("ServiceNumber"))
        criteria.SetMaxResults(1)
        Return criteria.UniqueResult(Of Resolution)
    End Function

    Public Function CountResolutionKind(ReslKind As ResolutionKind) As Integer
        criteria = NHibernateSession.CreateCriteria(Of Resolution)
        criteria.Add(Restrictions.Eq("ResolutionKind.Id", ReslKind.Id))
        criteria.SetProjection(Projections.Count("ResolutionKind.Id"))
        Return criteria.UniqueResult(Of Integer)
    End Function

    'Public Function CountResolutionKind(ReslKind As ResolutionKind) As Integer
    '    criteria = NHibernateSession.CreateCriteria(Of Resolution)
    '    criteria.Add(Restrictions.Eq("ResolutionKind.Id", ReslKind.Id))
    '    criteria.SetProjection(Projections.Count("ResolutionKind.Id"))
    '    Return criteria.UniqueResult(Of Integer)
    'End Function

End Class
