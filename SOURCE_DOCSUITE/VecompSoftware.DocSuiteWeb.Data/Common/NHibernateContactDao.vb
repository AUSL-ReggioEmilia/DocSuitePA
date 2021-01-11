Imports System
Imports System.Linq
Imports System.Collections.Generic
Imports NHibernate
Imports NHibernate.Criterion
Imports NHibernate.Transform
Imports NHibernate.Linq
Imports System.Text
Imports VecompSoftware.NHibernateManager.Dao
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Entity.Tenants

Public Class NHibernateContactDao
    Inherits BaseNHibernateDao(Of Contact)

    Public Enum DescriptionSearchType
        Equal = 1
        Contains = 2
    End Enum

#Region " Constructors "

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

    Public Overrides Sub Save(ByRef entity As Contact)
        MyBase.Save(entity)
        NHibernateSession.SessionFactory.Evict(GetType(Contact))
    End Sub

    Public Overrides Sub Update(ByRef entity As Contact)
        MyBase.Update(entity)
        NHibernateSession.SessionFactory.Evict(GetType(Contact))
    End Sub

    Public Overrides Sub UpdateNoLastChange(ByRef entity As Contact)
        MyBase.UpdateNoLastChange(entity)
        NHibernateSession.SessionFactory.Evict(GetType(Contact))
    End Sub

    Public Overrides Sub UpdateOnly(ByRef entity As Contact)
        MyBase.UpdateOnly(entity)
        NHibernateSession.SessionFactory.Evict(GetType(Contact))
    End Sub

    Public Overrides Sub Delete(ByRef entity As Contact)
        MyBase.Delete(entity)
        NHibernateSession.SessionFactory.Evict(GetType(Contact))
    End Sub

    Public Function GetRootContact(ByVal searchAll As Boolean,
                                   Optional categoryFascicleRightRoles As IList(Of Integer) = Nothing,
                                   Optional excludeParentId As List(Of Integer) = Nothing,
                                   Optional onlyParentId As Integer? = Nothing,
                                   Optional procedureType As String = Nothing, Optional idRole As Integer? = Nothing,
                                   Optional currentTenant As Tenant = Nothing) As IList(Of Contact)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        If String.IsNullOrEmpty(procedureType) Then
            criteria.Add(Restrictions.IsNull("Parent"))
        End If

        If DocSuiteContext.Current.ProtocolEnv.RoleContactEnabled Then
            criteria.Add(Restrictions.IsNull("RoleRootContact"))
        End If

        If Not searchAll Then
            criteria.Add(Restrictions.Eq("IsActive", 1S))
        End If
        If excludeParentId IsNot Nothing AndAlso excludeParentId.Count() > 0 Then
            criteria.Add(Restrictions.Not(Restrictions.In("Id", excludeParentId)))
            For Each i As Integer In excludeParentId
                criteria.Add(Restrictions.Not(Restrictions.Like("FullIncrementalPath", String.Format("{0}|", i.ToString()), MatchMode.Start)))
            Next
        End If

        If onlyParentId.HasValue Then
            criteria.Add(Restrictions.Or(Restrictions.Like("FullIncrementalPath", String.Format("{0}|", onlyParentId.Value.ToString()), MatchMode.Start),
            Restrictions.Like("FullIncrementalPath", onlyParentId.Value.ToString(), MatchMode.Exact)))
        End If


        'Se si è selezionato il classificatore, allora si cercano i contatti definiti sui settori con diritti sul classificatore con tipologia procedureType
        If categoryFascicleRightRoles IsNot Nothing OrElse Not String.IsNullOrEmpty(procedureType) Then
            criteria.Add(GetSearchContactInCategoryIntersectionRoleCriteriaDisjunction(categoryFascicleRightRoles, procedureType))
        End If

        If currentTenant IsNot Nothing Then
            If Not String.IsNullOrEmpty(currentTenant.UniqueId.ToString()) Then
                criteria.CreateAlias("TenantContacts", "TC", SqlCommand.JoinType.InnerJoin)
                criteria.Add(Restrictions.Eq("TC.IdTenant", currentTenant.UniqueId))
            End If
        End If

        criteria.AddOrder(Order.Asc("Description"))
        Return criteria.List(Of Contact)()
    End Function

    Public Function GetRoleRootContact(ByVal searchAll As Boolean, Optional excludeParentId As List(Of Integer) = Nothing, Optional onlyParentId As Integer? = Nothing, Optional currentTenant As Tenant = Nothing) As IList(Of Contact)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.IsNull("Parent"))

        criteria.Add(Restrictions.IsNotNull("RoleRootContact"))

        If Not searchAll Then
            criteria.Add(Restrictions.Eq("IsActive", 1S))
        End If

        If excludeParentId IsNot Nothing AndAlso excludeParentId.Count() > 0 Then
            criteria.Add(Restrictions.Not(Restrictions.In("Id", excludeParentId)))
            For Each i As Integer In excludeParentId
                criteria.Add(Restrictions.Not(Restrictions.Like("FullIncrementalPath", String.Format("{0}|", i.ToString()), MatchMode.Start)))
            Next
        End If
        If onlyParentId.HasValue Then
            criteria.Add(Restrictions.Like("FullIncrementalPath", onlyParentId.Value.ToString(), MatchMode.Start))

        End If

        If currentTenant IsNot Nothing Then
            If Not String.IsNullOrEmpty(currentTenant.UniqueId.ToString()) Then
                criteria.CreateAlias("TenantContacts", "TC", SqlCommand.JoinType.InnerJoin)
                criteria.Add(Restrictions.Eq("TC.IdTenant", currentTenant.UniqueId))
            End If
        End If

        criteria.AddOrder(Order.Asc("Description"))

        Return criteria.List(Of Contact)()
    End Function


    Public Function GetRoleRootContact(ByVal sRole As ICollection, ByVal searchAll As Boolean, Optional excludeParentIds As List(Of Integer) = Nothing, Optional onlyParentId As Integer? = Nothing, Optional currentTenant As Tenant = Nothing) As IList(Of Contact)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.IsNull("Parent"))
        criteria.Add(Restrictions.In("RoleRootContact.Id", sRole))

        If Not searchAll Then
            criteria.Add(Restrictions.Eq("IsActive", 1S))
        End If

        If excludeParentIds IsNot Nothing AndAlso excludeParentIds.Count() > 0 Then
            criteria.Add(Restrictions.Not(Restrictions.In("Id", excludeParentIds)))
            For Each i As Integer In excludeParentIds
                criteria.Add(Restrictions.Not(Restrictions.Like("FullIncrementalPath", String.Format("{0}|", i.ToString()), MatchMode.Start)))
            Next
        End If

        If onlyParentId.HasValue Then
            criteria.Add(Restrictions.Like("FullIncrementalPath", onlyParentId.Value.ToString(), MatchMode.Start))
        End If

        If currentTenant IsNot Nothing Then
            If Not String.IsNullOrEmpty(currentTenant.UniqueId.ToString()) Then
                criteria.CreateAlias("TenantContacts", "TC", SqlCommand.JoinType.InnerJoin)
                criteria.Add(Restrictions.Eq("TC.IdTenant", currentTenant.UniqueId))
            End If
        End If

        criteria.AddOrder(Order.Asc("Description"))

        Return criteria.List(Of Contact)()
    End Function

    Public Function GetContactByParentId(ByVal parentId As Integer, ByVal searchAll As Boolean,
                                         Optional categoryFascicleRightRoles As IList(Of Integer) = Nothing,
                                         Optional excludeParentIds As List(Of Integer) = Nothing,
                                         Optional onlyParentId As Integer? = Nothing,
                                         Optional contactListId As Guid? = Nothing,
                                         Optional procedureType As String = Nothing,
                                         Optional idRole As Integer? = Nothing,
                                         Optional roleType As String = Nothing,
                                         Optional currentTenant As Tenant = Nothing) As IList(Of Contact)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.CreateAlias("Parent", "p", SqlCommand.JoinType.LeftOuterJoin)
        criteria.Add(Restrictions.Eq("p.Id", parentId))

        If Not searchAll Then criteria.Add(Restrictions.Eq("IsActive", 1S))

        If excludeParentIds IsNot Nothing AndAlso excludeParentIds.Count() > 0 Then
            criteria.Add(Restrictions.Not(Restrictions.In("Id", excludeParentIds)))
            For Each i As Integer In excludeParentIds
                criteria.Add(Restrictions.Not(Restrictions.Like("FullIncrementalPath", String.Format("{0}|", i.ToString()), MatchMode.Start)))
            Next
        End If

        If onlyParentId.HasValue Then
            criteria.Add(Restrictions.Like("FullIncrementalPath", String.Format("{0}|", onlyParentId.Value.ToString()), MatchMode.Start))
        End If

        If DocSuiteContext.Current.ProtocolEnv.ContactListsEnabled AndAlso contactListId IsNot Nothing AndAlso Not contactListId.Equals(Guid.Empty) Then
            criteria.CreateAlias("ContactLists", "CL")
            criteria.Add(Restrictions.Eq("CL.Id", contactListId))
        End If

        If categoryFascicleRightRoles IsNot Nothing OrElse Not String.IsNullOrEmpty(procedureType) Then
            criteria.Add(GetSearchContactInCategoryIntersectionRoleCriteriaDisjunction(categoryFascicleRightRoles, procedureType))
        End If

        If roleType IsNot Nothing AndAlso roleType.Equals(RoleUserType.RP.ToString()) Then
            criteria.Add(Subqueries.PropertyIn("SearchCode", GetByRoleUserRP(idRole, roleType)))
        End If

        If currentTenant IsNot Nothing Then
            If Not String.IsNullOrEmpty(currentTenant.UniqueId.ToString()) Then
                criteria.CreateAlias("TenantContacts", "TC", SqlCommand.JoinType.InnerJoin)
                criteria.Add(Restrictions.Eq("TC.IdTenant", currentTenant.UniqueId))
            End If
        End If

        criteria.AddOrder(Order.Asc("Description"))

        criteria.SetFetchMode("Children", FetchMode.Eager)
        criteria.SetResultTransformer(Transformers.DistinctRootEntity)

        Return criteria.List(Of Contact)()
    End Function

    Public Function GetContactWithId(ByVal idContactList As Integer()) As IList(Of Contact)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.In("Id", idContactList))
        Return criteria.List(Of Contact)()
    End Function

    Public Function GetContactWithEmail(ByVal eMailList As String()) As IList(Of Contact)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.In("EmailAddress", eMailList))

        Return criteria.List(Of Contact)()
    End Function

    Public Function GetContactWithCertifiedEmail(ByVal eMailList As String()) As IList(Of Contact)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.In("CertifiedMail", eMailList))

        Return criteria.List(Of Contact)()
    End Function

    Public Function GetContactWithCertifiedAndClassicEmail(ByVal eMailList As String()) As IList(Of Contact)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        Dim disj As Disjunction = New Disjunction()
        disj.Add(Restrictions.In("CertifiedMail", eMailList))
        disj.Add(Restrictions.In("EmailAddress", eMailList))
        criteria.Add(disj)
        criteria.Add(Restrictions.Eq("IsActive", 1S))

        Return criteria.List(Of Contact)()
    End Function

    Public Function GetContactByDescription(ByVal description As String, ByVal searchType As DescriptionSearchType, ByVal searchAll As Boolean,
                                            ByVal contactRootRoles As List(Of Integer),
                                            Optional categoryFascicleRightRoles As IList(Of Integer) = Nothing,
                                            Optional ByVal rootFullIncrementalPath As String = "",
                                            Optional exludeParentId As List(Of Integer) = Nothing,
                                            Optional onlyParentId As Integer? = Nothing,
                                            Optional contactListId As Guid? = Nothing,
                                            Optional procedureType As String = Nothing,
                                            Optional idRole As Integer? = Nothing,
                                            Optional roleType As String = Nothing,
                                            Optional currentTenant As Tenant = Nothing) As IList(Of Contact)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        'descrizione da cercare
        If Not String.IsNullOrEmpty(description) Then
            Dim words As String() = description.Split(" "c)
            Dim conju As Conjunction = Restrictions.Conjunction()
            For i As Integer = 0 To words.Length - 1
                If (i = 0) Then
                    Select Case searchType
                        Case DescriptionSearchType.Equal
                            conju.Add(Restrictions.Like("Description", words(i), MatchMode.Start))
                        Case DescriptionSearchType.Contains
                            conju.Add(Restrictions.Like("Description", words(i), MatchMode.Anywhere))
                    End Select
                Else
                    conju.Add(Restrictions.Like("Description", words(i), MatchMode.Anywhere))
                End If
            Next
            criteria.Add(conju)
        End If

        'sicurezza rubriche di settore in ricerca per descrizione
        If contactRootRoles IsNot Nothing Then
            Dim disjunction As Disjunction = Restrictions.Disjunction()
            disjunction.Add(Restrictions.IsNull("RoleRootContact.Id"))
            disjunction.Add(Restrictions.In("RoleRootContact.Id", contactRootRoles.ToArray()))
            criteria.Add(disjunction)
        End If

        If exludeParentId IsNot Nothing AndAlso exludeParentId.Count() > 0 Then
            criteria.Add(Restrictions.Not(Restrictions.In("Id", exludeParentId)))
            For Each i As Integer In exludeParentId
                criteria.Add(Restrictions.Not(Restrictions.Like("FullIncrementalPath", String.Format("{0}|", i.ToString()), MatchMode.Start)))
            Next
        End If

        If onlyParentId.HasValue Then
            criteria.Add(Restrictions.Like("FullIncrementalPath", String.Format("{0}|", onlyParentId.Value.ToString()), MatchMode.Start))
        End If

        'Contatto di default
        If Not String.IsNullOrEmpty(rootFullIncrementalPath) Then
            criteria.Add(Restrictions.Like("FullIncrementalPath", rootFullIncrementalPath, MatchMode.Start))
        End If

        If Not searchAll Then
            criteria.Add(Restrictions.Eq("IsActive", 1S))
        End If

        If DocSuiteContext.Current.ProtocolEnv.ContactListsEnabled AndAlso contactListId IsNot Nothing AndAlso Not contactListId.Equals(Guid.Empty) Then
            criteria.CreateAlias("ContactLists", "CL")
            criteria.Add(Restrictions.Eq("CL.Id", contactListId))
        End If

        If categoryFascicleRightRoles IsNot Nothing OrElse Not String.IsNullOrEmpty(procedureType) Then
            criteria.Add(GetSearchContactInCategoryIntersectionRoleCriteriaDisjunction(categoryFascicleRightRoles, procedureType))
        End If

        If roleType IsNot Nothing AndAlso roleType.Equals(RoleUserType.RP.ToString()) Then
            criteria.Add(Subqueries.PropertyIn("SearchCode", GetByRoleUserRP(idRole, roleType)))
        End If

        If currentTenant IsNot Nothing Then
            If Not String.IsNullOrEmpty(currentTenant.UniqueId.ToString()) Then
                criteria.CreateAlias("TenantContacts", "TC", SqlCommand.JoinType.InnerJoin)
                criteria.Add(Restrictions.Eq("TC.IdTenant", currentTenant.UniqueId))
            End If
        End If

        criteria.AddOrder(Order.Asc("Description"))

        Return criteria.List(Of Contact)()
    End Function

    Public Function GetContactByFiscalCode(ByVal fiscalCode As String) As IList(Of Contact)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("FiscalCode", fiscalCode))

        Return criteria.List(Of Contact)()
    End Function

    Public Function GetContactByDescriptionAndContactType(ByVal description As String, ByVal contactType As Char) As IList(Of Contact)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("Description", description))
        criteria.Add(Restrictions.Eq("ContactType.Id", contactType))

        Return criteria.List(Of Contact)()
    End Function

    Public Function GetByDescription(description As String, contactType As Char, parentId As Integer?) As IList(Of Contact)
        Dim criteria As ICriteria = Me.NHibernateSession.CreateCriteria(Of Contact)()
        criteria.Add(Restrictions.Eq("Description", description))
        criteria.Add(Restrictions.Eq("ContactType.Id", contactType))
        criteria.Add(Restrictions.Eq("IsActive", 1S))

        If parentId.HasValue Then
            If parentId.Value.Equals(0) Then
                Return Nothing
            End If
            criteria.Add(Restrictions.Eq("Parent.Id", parentId.Value))
        End If

        Return criteria.List(Of Contact)()
    End Function

    Public Function GetByFiscalCodes(fiscalCodes As ICollection(Of String), contactType As Char, parentId As Integer?) As IList(Of Contact)
        Dim criteria As ICriteria = Me.NHibernateSession.CreateCriteria(Of Contact)()
        criteria.Add(Restrictions.In("FiscalCode", fiscalCodes.ToArray()))
        criteria.Add(Restrictions.Eq("ContactType.Id", contactType))

        If parentId.HasValue Then
            If parentId.Value.Equals(0) Then
                Return Nothing
            End If
            criteria.Add(Restrictions.Eq("Parent.Id", parentId.Value))
        End If

        Return criteria.List(Of Contact)()
    End Function

    Public Function GetLikeDescription(description As String, contactType As Char, parentId As Integer?) As IList(Of Contact)
        Dim criteria As ICriteria = Me.NHibernateSession.CreateCriteria(Of Contact)()
        criteria.Add(Restrictions.Like("Description", description, MatchMode.Anywhere))
        criteria.Add(Restrictions.Eq("ContactType.Id", contactType))
        criteria.Add(Restrictions.IsNotNull("FiscalCode"))

        If parentId.HasValue Then
            If parentId.Value.Equals(0) Then
                Return Nothing
            End If
            criteria.Add(Restrictions.Eq("Parent.Id", parentId.Value))
        End If

        Return criteria.List(Of Contact)()
    End Function
    Public Function GetContactBySearchCode(ByVal searchCode As String, ByVal isActive As Short,
                                           Optional categoryFascicleRightRoles As IList(Of Integer) = Nothing,
                                           Optional excludeParentId As List(Of Integer) = Nothing,
                                           Optional onlyParentId As Integer? = Nothing,
                                           Optional contactListId As Guid? = Nothing,
                                           Optional procedureType As String = Nothing,
                                           Optional idRole As Integer? = Nothing,
                                           Optional roleType As String = Nothing,
                                           Optional currentTenantId? As Guid = Nothing) As IList(Of Contact)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("SearchCode", searchCode))

        If isActive <> -1 Then
            criteria.Add(Restrictions.Eq("IsActive", isActive))
        End If

        If excludeParentId IsNot Nothing AndAlso excludeParentId.Count() > 0 Then
            criteria.Add(Restrictions.Not(Restrictions.In("Id", excludeParentId)))
            For Each i As Integer In excludeParentId
                criteria.Add(Restrictions.Not(Restrictions.Like("FullIncrementalPath", String.Format("{0}|", i.ToString()), MatchMode.Start)))
            Next
        End If

        If onlyParentId.HasValue Then
            criteria.Add(Restrictions.Like("FullIncrementalPath", onlyParentId.Value.ToString(), MatchMode.Start))
        End If

        If categoryFascicleRightRoles IsNot Nothing OrElse Not String.IsNullOrEmpty(procedureType) Then
            criteria.Add(GetSearchContactInCategoryIntersectionRoleCriteriaDisjunction(categoryFascicleRightRoles, procedureType))
        End If

        If roleType IsNot Nothing AndAlso roleType.Equals(RoleUserType.RP.ToString()) Then
            criteria.Add(Subqueries.PropertyIn("SearchCode", GetByRoleUserRP(idRole, roleType)))
        End If

        If DocSuiteContext.Current.ProtocolEnv.ContactListsEnabled AndAlso contactListId IsNot Nothing AndAlso Not contactListId.Equals(Guid.Empty) Then
            criteria.CreateAlias("ContactLists", "CL")
            criteria.Add(Restrictions.Eq("CL.Id", contactListId))
        End If

        If currentTenantId IsNot Nothing Then
            If Not String.IsNullOrEmpty(currentTenantId.ToString()) Then
                criteria.CreateAlias("TenantContacts", "TC", SqlCommand.JoinType.InnerJoin)
                criteria.Add(Restrictions.Eq("TC.IdTenant", currentTenantId))
            End If
        End If

        criteria.AddOrder(Order.Asc("Description"))

        Return criteria.List(Of Contact)()
    End Function

    Public Function GetContactByIncrementalFather(ByVal incrementalFather As Integer, Optional ByVal isActive As Boolean = False) As IList(Of Contact)
        Dim sqlQuery As String = "SELECT {c.*} from Contact {c}" &
                                 " WHERE ( c.FullIncrementalPath LIKE (" &
                                 "  (SELECT c.FullIncrementalPath" &
                                 "      FROM Contact {c} " &
                                 "      WHERE ( c.Incremental = :IncrementalFather )) + '%'))" &
                                 " AND c.IsActive = " & If(isActive, "1", "0") &
                                 " ORDER BY c.FullIncrementalPath"
        Dim query As ISQLQuery = NHibernateSession.CreateSQLQuery(sqlQuery).AddEntity("c", persitentType)
        query.SetInt32("IncrementalFather", incrementalFather)

        Return query.List(Of Contact)()
    End Function

    Public Function GetMaxId() As Integer
        Const query As String = "SELECT MAX(C.Id) FROM Contact AS C"
        Try
            Return NHibernateSession.CreateQuery(query).UniqueResult(Of Integer)
        Catch ex As Exception
            Return 0
        End Try
    End Function

    ''' <summary> Costruisce il FullIncrementalPath di una role. </summary>
    ''' <param name="contact">Contact</param>
    ''' <param name="sFull">variabile che conterrà il FullIncrementalPath</param>
    Public Sub GetFullIncrementalPath(ByRef contact As Contact, ByRef sFull As String)
        If contact Is Nothing Then
            sFull = "" & sFull
        Else
            If sFull <> "" Then
                sFull = "|" & sFull
            End If
            sFull = contact.Id.ToString() & sFull

            Dim father As Contact = contact.Parent
            If father IsNot Nothing Then
                GetFullIncrementalPath(father, sFull)
            End If
        End If
    End Sub

    Public Function GetContactByGroups(ByVal groups As IList(Of String)) As IList(Of Contact)
        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.CreateAlias("Role", "r")
        criteria.CreateAlias("r.RoleGroups", "rg")
        criteria.Add(Restrictions.In("rg.Name", groups.ToArray()))
        criteria.AddOrder(Order.Asc("Description"))

        Return criteria.List(Of Contact)()
    End Function

    Public Function GetContactByFullPath(ByVal incrementalPath As String) As IList(Of Contact)
        criteria = NHibernateSession.CreateCriteria(persitentType)

        Dim incrementals As ICollection(Of Integer) = New List(Of Integer)
        If Not BaseEnvironment.TryGetIntegers(incrementalPath, incrementals) Then
            FileLogger.Warn(LogName.FileLog, String.Concat("Attenzione incrementalPath errata_ ", incrementalPath))
            Return New List(Of Contact)
        End If
        criteria.Add(Restrictions.In("Id", incrementals.ToArray()))
        Return criteria.List(Of Contact)()
    End Function

    Public Function GetContactsForContactList(ByVal contactList As Integer()) As IList(Of Contact)
        'recupera tutti i padri dei contatti passati in lista
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.In("Id", contactList))
        criteria.Add(Restrictions.Eq("IsActive", 1S))
        Dim disj As New Disjunction()
        disj.Add(Restrictions.And(Restrictions.IsNull("ActiveFrom"), Restrictions.IsNull("ActiveTo")))
        disj.Add(Restrictions.And(Restrictions.Ge("ActiveTo", Date.Now), Restrictions.Le("ActiveFrom", DateTime.Now)))
        criteria.Add(disj)

        Return criteria.List(Of Contact)()
    End Function

    Public Function GetContacts(ByVal code As String, ByVal contactType As Char?, ByVal isActive As Short?) As IList(Of Contact)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        If Not String.IsNullOrEmpty(code) Then
            criteria.Add(Restrictions.Eq("Code", code))
        End If

        If isActive.HasValue Then
            If isActive.Value <> -1 Then
                criteria.Add(Restrictions.Eq("IsActive", isActive.Value))
            End If
        End If

        If contactType.HasValue Then
            criteria.Add(Restrictions.Eq("ContactType.Id", contactType.Value))
        End If

        criteria.AddOrder(Order.Asc("Description"))

        Return criteria.List(Of Contact)()
    End Function

    Public Function GetCountByContactTitle(ByVal contactTitle As ContactTitle) As Integer
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("StudyTitle.Id", contactTitle.Id))
        Return criteria.List(Of Contact)().Count
    End Function

    Public Function GetByMail(mailAddress As String) As IList(Of Contact)
        Dim criteria As ICriteria = Me.NHibernateSession.CreateCriteria(Of Contact)()
        Dim disj As New Disjunction()
        disj.Add(Restrictions.Eq("CertifiedMail", mailAddress))
        disj.Add(Restrictions.Eq("EmailAddress", mailAddress))
        criteria.Add(disj)
        Return criteria.List(Of Contact)()
    End Function

    Public Function GetByList(contactListId As Guid, showAll As Boolean, roleContactIds As List(Of Integer), excludeParentalIds As List(Of Integer), onlyParentId As Integer?) As IList(Of Contact)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("ContactLists", "CL")
        criteria.Add(Restrictions.Eq("CL.Id", contactListId))

        If roleContactIds IsNot Nothing Then
            Dim disjunction As Disjunction = Restrictions.Disjunction()
            disjunction.Add(Restrictions.IsNull("RoleRootContact.Id"))
            disjunction.Add(Restrictions.In("RoleRootContact.Id", roleContactIds.ToArray()))
            criteria.Add(disjunction)
        End If

        If excludeParentalIds IsNot Nothing AndAlso excludeParentalIds.Count() > 0 Then
            criteria.Add(Restrictions.Not(Restrictions.In("Id", excludeParentalIds)))
            For Each i As Integer In excludeParentalIds
                criteria.Add(Restrictions.Not(Restrictions.Like("FullIncrementalPath", String.Format("{0}|", i.ToString()), MatchMode.Start)))
            Next
        End If

        If onlyParentId.HasValue Then
            criteria.Add(Restrictions.Like("FullIncrementalPath", String.Format("{0}|", onlyParentId.Value.ToString()), MatchMode.Start))
        End If

        If Not showAll Then
            criteria.Add(Restrictions.Eq("IsActive", 1S))
        End If

        Return criteria.List(Of Contact)()
    End Function

    Private Function GetByCategoryIdAndProcedureType(categoryFascicleRightRoles As IList(Of Integer), type As String) As DetachedCriteria
        Dim detachActiveRoleCriteria As DetachedCriteria = DetachedCriteria.For(Of Role)("R")
        detachActiveRoleCriteria.Add(Restrictions.Eq("R.IsActive", Convert.ToInt16(True)))
        If categoryFascicleRightRoles IsNot Nothing AndAlso categoryFascicleRightRoles.Any() Then
            detachActiveRoleCriteria.Add(Restrictions.In("R.Id", categoryFascicleRightRoles.ToArray()))
        End If
        detachActiveRoleCriteria.SetProjection(Projections.Id)

        Dim detachRoleUserCriteria As DetachedCriteria = DetachedCriteria.For(Of RoleUser)("RU")
        detachRoleUserCriteria.CreateAlias("RU.Role", "Role", SqlCommand.JoinType.InnerJoin)
        detachRoleUserCriteria.CreateAlias("Role.RoleGroups", "RoleGroups", SqlCommand.JoinType.InnerJoin)
        If Not String.IsNullOrEmpty(type) Then
            detachRoleUserCriteria.Add(Restrictions.Eq("RU.Type", type))
        End If
        detachRoleUserCriteria.Add(Subqueries.PropertyIn("Role.Id", detachActiveRoleCriteria))
        detachRoleUserCriteria.Add(Restrictions.Not(Restrictions.Eq("RU.Account", String.Empty)))
        detachRoleUserCriteria.SetProjection(Projections.Distinct(Projections.Property("RU.Account")))

        Return detachRoleUserCriteria
    End Function

    Private Function GetSearchContactInCategoryIntersectionRoleCriteriaDisjunction(categoryFascicleRightRoles As IList(Of Integer), procedureType As String) As Disjunction
        Dim disjExistProcedureTypeInRootOrChildren As Disjunction = New Disjunction()

        Dim existsChildren As DetachedCriteria = DetachedCriteria.For(Of Contact)("CC")
        existsChildren.SetProjection(Projections.Property("CC.Parent.Id"))

        Dim mailDisjuction As Disjunction = New Disjunction()
        mailDisjuction.Add(Subqueries.PropertyIn("SearchCode", GetByCategoryIdAndProcedureType(categoryFascicleRightRoles, procedureType)))
        existsChildren.Add(mailDisjuction)
        disjExistProcedureTypeInRootOrChildren.Add(mailDisjuction)

        Return disjExistProcedureTypeInRootOrChildren
    End Function

    Private Function GetByRoleUserRP(idRole As Integer?, roleType As String) As DetachedCriteria
        Dim detachRoleUserCriteria As DetachedCriteria = DetachedCriteria.For(Of RoleUser)("RU")
        detachRoleUserCriteria.CreateAlias("RU.Role", "Role", SqlCommand.JoinType.InnerJoin)
        detachRoleUserCriteria.CreateAlias("Role.RoleGroups", "RoleGroups", SqlCommand.JoinType.InnerJoin)
        detachRoleUserCriteria.Add(Restrictions.Eq("Role.IsActive", Convert.ToInt16(True)))

        If idRole IsNot Nothing Then
            detachRoleUserCriteria.Add(Restrictions.Eq("Role.Id", idRole.Value))
        End If

        If Not String.IsNullOrEmpty(roleType) Then
            detachRoleUserCriteria.Add(Restrictions.Eq("RU.Type", roleType))
        End If

        Return detachRoleUserCriteria.SetProjection(Projections.Property("RU.Account"))
    End Function

    Public Function GetContactByRole(ByVal searchCode As String, ByVal isActive As Short, Optional parentId As Integer? = Nothing, Optional idRole As Integer? = Nothing) As IList(Of Contact)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        If isActive <> -1 Then
            criteria.Add(Restrictions.Eq("IsActive", isActive))
        End If

        If parentId.HasValue AndAlso parentId <> 0 Then
            criteria.Add(Restrictions.Like("FullIncrementalPath", String.Format("{0}|", parentId.Value.ToString()), MatchMode.Start))
        End If

        Dim detachRoleUserCriteria As DetachedCriteria = DetachedCriteria.For(Of RoleUser)("RU")
        detachRoleUserCriteria.CreateAlias("RU.Role", "Role", SqlCommand.JoinType.InnerJoin)
        detachRoleUserCriteria.CreateAlias("Role.RoleGroups", "RoleGroups", SqlCommand.JoinType.InnerJoin)
        detachRoleUserCriteria.Add(Restrictions.Eq("Role.IsActive", Convert.ToInt16(True)))

        If idRole IsNot Nothing Then
            detachRoleUserCriteria.Add(Restrictions.Eq("Role.Id", idRole.Value))
        End If

        If Not String.IsNullOrEmpty(searchCode) Then
            criteria.Add(Restrictions.Eq("SearchCode", searchCode))
            detachRoleUserCriteria.Add(Restrictions.Eq("Account", searchCode))
        End If

        detachRoleUserCriteria.SetProjection(Projections.Property("RU.Account"))

        criteria.Add(Subqueries.PropertyIn("SearchCode", detachRoleUserCriteria))

        criteria.AddOrder(Order.Asc("Description"))

        Return criteria.List(Of Contact)()
    End Function

    Public Function GetByIdRole(ByVal idRole As Integer) As Contact
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Role.Id", idRole))
        Return criteria.List(Of Contact)().FirstOrDefault()
    End Function

    Public Sub BatchChangeContactActiveState(contact As Contact, isActive As Boolean, Optional recursiveChildren As Boolean = False)
        NHibernateSession.Query(Of Contact)() _
                        .Where(Function(x) x.Id = contact.Id) _
                        .UpdateBuilder() _
                        .Set(Function(p) p.IsActive, Convert.ToInt16(isActive)) _
                        .Update()

        If recursiveChildren Then
            NHibernateSession.Query(Of Contact)() _
                        .Where(Function(x) x.FullIncrementalPath.StartsWith(String.Concat(contact.FullIncrementalPath, "|"))) _
                        .UpdateBuilder() _
                        .Set(Function(p) p.IsActive, Convert.ToInt16(isActive)) _
                        .Update()
        End If
    End Sub

    Public Function GetContactByIncrementalFatherAndSearchCode(ByVal incrementalFather As Integer, ByVal searchCode As String, ByVal isActive As Boolean) As Contact
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("IsActive", If(isActive, 1S, 0S)))
        If Not String.IsNullOrEmpty(searchCode) Then
            criteria.Add(Restrictions.Eq("SearchCode", searchCode))
        End If
        Dim detachedContactCriteria As DetachedCriteria = DetachedCriteria.For(Of Contact)("C")
        detachedContactCriteria.Add(Restrictions.Eq("C.SearchCode", searchCode).IgnoreCase())
        detachedContactCriteria.SetProjection(Projections.Property("C.FullIncrementalPath"))
        criteria.Add(Subqueries.PropertyIn("FullIncrementalPath", detachedContactCriteria))
        criteria.AddOrder(Order.Asc("FullIncrementalPath"))

        Return criteria.List(Of Contact)().FirstOrDefault()
    End Function
End Class