Imports System.Linq
Imports NHibernate
Imports NHibernate.Criterion
Imports NHibernate.Transform
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateSecurityUsersDao
    Inherits BaseNHibernateDao(Of SecurityUsers)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    ''' <summary> Restituisce l'Id di valore massimo. </summary>
    Public Function GetMaxId() As Integer
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.SetProjection(Projections.Max("Id"))

        Return criteria.UniqueResult(Of Integer)
    End Function

    ''' <summary> Elenco di SecurityGroups abbinati ad un SecurityUsers. </summary>
    Public Function GetGroupsByAccount(userDomain As String, defaultDomain As String, account As String, Optional groups As Integer() = Nothing) As IList(Of SecurityGroups)
        Dim criteria As ICriteria = CreateGroupsByAccountCriteria(userDomain, defaultDomain, account, groups)
        Return criteria.List(Of SecurityGroups)()
    End Function

    ''' <summary> Count di SecurityGroups abbinati ad un SecurityUsers. </summary>
    Public Function CountGroupsByAccount(userDomain As String, defaultDomain As String, account As String, Optional groups As Integer() = Nothing) As Integer
        Dim criteria As ICriteria = CreateGroupsByAccountCriteria(userDomain, defaultDomain, account, groups)
        criteria.SetProjection(Projections.RowCount())
        Return criteria.UniqueResult(Of Integer)()
    End Function

    ''' <summary> Elenco di SecurityUsers di uno SecurityGroup. </summary>
    Public Function GetUsersByGroup(idGroup As Integer) As IList(Of SecurityUsers)
        criteria = NHibernateSession.CreateCriteria(Of SecurityUsers)()
        criteria.Add(Restrictions.Eq("Group.Id", idGroup))

        Return criteria.List(Of SecurityUsers)()
    End Function

    Public Function GetUsersByAccount(account As String, excludeDomain As String) As IList(Of SecurityUsers)
        criteria = NHibernateSession.CreateCriteria(Of SecurityUsers)()
        criteria.Add(Restrictions.Eq("Account", account))
        criteria.Add(Restrictions.Not(Restrictions.Eq("UserDomain", excludeDomain)))

        Return criteria.List(Of SecurityUsers)
    End Function

    Public Function GetUsersByAccountOrDescription(searchText As String, domain As String) As IList(Of SecurityUsers)
        criteria = NHibernateSession.CreateCriteria(Of SecurityUsers)()
        criteria.Add(Restrictions.Or(Restrictions.Like("Account", searchText, MatchMode.Anywhere), Restrictions.Like("Description", searchText, MatchMode.Anywhere)))
        If Not String.IsNullOrEmpty(domain) Then
            criteria.Add(Restrictions.Eq("UserDomain", domain))
        End If

        Dim userProjection As ProjectionList = Projections.ProjectionList()
        userProjection.Add(Projections.Property("Account"), "Account")
        userProjection.Add(Projections.Property("Description"), "Description")
        userProjection.Add(Projections.Property("UserDomain"), "UserDomain")
        criteria.SetProjection(Projections.Distinct(userProjection))
        criteria.SetResultTransformer(Transformers.AliasToBean(Of SecurityUsers))

        Return criteria.List(Of SecurityUsers)
    End Function

    Public Function GetUsersByAccountAndGroups(account As String, domain As String, idGroups As Integer()) As IList(Of SecurityUsers)
        criteria = NHibernateSession.CreateCriteria(Of SecurityUsers)()
        criteria.Add(Restrictions.Eq("Account", account))
        criteria.Add(Restrictions.Eq("UserDomain", domain))
        criteria.CreateAlias("Group", "G")
        criteria.Add(Restrictions.In("G.Id", idGroups))
        Return criteria.List(Of SecurityUsers)
    End Function

    Public Function GetUsersByDescription(description As String, domain As String) As IList(Of SecurityUsers)
        criteria = NHibernateSession.CreateCriteria(Of SecurityUsers)()
        If Not String.IsNullOrEmpty(description) Then
            criteria.Add(Restrictions.Like("Description", description, MatchMode.Anywhere))
        End If
        If Not String.IsNullOrEmpty(domain) Then
            criteria.Add(Restrictions.Eq("UserDomain", domain))
        End If

        Dim userProjection As ProjectionList = Projections.ProjectionList()
        userProjection.Add(Projections.Property("Account"), "Account")
        userProjection.Add(Projections.Property("Description"), "Description")
        userProjection.Add(Projections.Property("UserDomain"), "UserDomain")
        criteria.SetProjection(Projections.Distinct(userProjection))
        criteria.SetResultTransformer(Transformers.AliasToBean(Of SecurityUsers))
        Return criteria.List(Of SecurityUsers)
    End Function

    Public Function GetSecurityUsersCount(account As String, domain As String) As Long
        criteria = NHibernateSession.CreateCriteria(Of SecurityUsers)()
        criteria.Add(Restrictions.Eq("Account", account))
        criteria.Add(Restrictions.Eq("UserDomain", domain))
        criteria.SetProjection(Projections.RowCountInt64())

        Return criteria.UniqueResult(Of Long)()
    End Function

    ''' <summary> Cerca uno specifico account utente in un gruppo di tipo security group. </summary>
    Public Function GetUsersByGroupAndAccount(idGroup As Integer, account As String) As IList(Of SecurityUsers)
        criteria = NHibernateSession.CreateCriteria(Of SecurityUsers)()
        criteria.Add(Restrictions.Eq("Group.Id", idGroup))
        criteria.Add(Restrictions.Eq("Account", account))

        Return criteria.List(Of SecurityUsers)()
    End Function
    Public Function GetUsersByGroupAndAccount(idGroup As Integer, userDomain As String, defaultDomain As String, account As String) As IList(Of SecurityUsers)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of SecurityUsers)()
        criteria.Add(Restrictions.Eq("Group.Id", idGroup))
        criteria.Add(Restrictions.Eq("Account", account))

        If String.IsNullOrEmpty(userDomain) Then
            ' Qualora non venga specificato un dominio per retrocompatibilità considero nullo o il dominio di default.
            Dim disj As New Disjunction()
            disj.Add(Restrictions.Eq("UserDomain", defaultDomain))
            disj.Add(Restrictions.IsNull("UserDomain"))
            disj.Add(Expression.Sql("1=0"))
            criteria.Add(disj)
        Else
            criteria.Add(Restrictions.Eq("UserDomain", userDomain))
        End If

        Return criteria.List(Of SecurityUsers)()
    End Function

    Public Function ExistsUser(account As String, domain As String) As Boolean
        criteria = NHibernateSession.CreateCriteria(Of SecurityUsers)()
        criteria.Add(Restrictions.Eq("Account", account))
        criteria.Add(Restrictions.Eq("UserDomain", domain))
        criteria.SetProjection(Projections.RowCountInt64())
        Return criteria.UniqueResult(Of Long)() > 0
    End Function

    Public Function UserBelongsAtLeastOneRole(ByVal rolesId As IList(Of Integer), userDomain As String, account As String) As Boolean
        criteria = NHibernateSession.CreateCriteria(Of SecurityUsers)("SU")

        criteria.CreateAlias("SU.Group", "G", SqlCommand.JoinType.InnerJoin)
        criteria.CreateAlias("G.RoleGroup", "RG", SqlCommand.JoinType.InnerJoin)

        criteria.Add(Restrictions.Eq("SU.Account", account))
        criteria.Add(Restrictions.Eq("SU.UserDomain", userDomain))

        criteria.Add(Restrictions.In("RG.Role.Id", rolesId.ToArray()))

        criteria.SetProjection(Projections.Distinct(Projections.RowCountInt64()))
        Return criteria.UniqueResult(Of Long)() > 0

    End Function

    Public Function ExistUser(userDomain As String, account As String) As Boolean
        criteria = NHibernateSession.CreateCriteria(Of SecurityUsers)("SU")

        criteria.Add(Restrictions.Eq("SU.Account", account))
        criteria.Add(Restrictions.Eq("SU.UserDomain", userDomain))

        criteria.SetProjection(Projections.Distinct(Projections.RowCountInt64()))
        Return criteria.UniqueResult(Of Long)() > 0

    End Function

    ''' <summary> Restituisce i criteri nhibernate per la ricerca dei SecurityGroups abbinati ad un SecurityUsers. </summary>
    Private Function CreateGroupsByAccountCriteria(userDomain As String, defaultDomain As String, account As String, groups() As Integer) As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of SecurityGroups)("SG")

        Dim groupConj As Conjunction = New Conjunction()
        groupConj.Add(Restrictions.Eq("SG.HasAllUsers", True))

        Dim userDetachCriteria As DetachedCriteria = DetachedCriteria.For(GetType(SecurityUsers), "SU")
        'userDetachCriteria.CreateAlias("SU.Group", "G", SqlCommand.JoinType.InnerJoin)
        userDetachCriteria.Add(Restrictions.EqProperty("SG.Id", "SU.Group.Id"))
        userDetachCriteria.Add(Restrictions.Eq("SU.Account", account))

        If groups IsNot Nothing Then
            criteria.Add(Restrictions.In("SG.Id", groups))
        End If

        If String.IsNullOrEmpty(userDomain) OrElse userDomain.Equals(defaultDomain, StringComparison.InvariantCultureIgnoreCase) Then
            ' Qualora non venga specificato un dominio per retrocompatibilità considero nullo o il dominio di default.
            Dim userDisj As Disjunction = New Disjunction()
            userDisj.Add(Restrictions.Eq("SU.UserDomain", defaultDomain))
            userDisj.Add(Restrictions.IsNull("SU.UserDomain"))
            userDisj.Add(Expression.Sql("1=0"))
            userDetachCriteria.Add(userDisj)
        Else
            userDetachCriteria.Add(Restrictions.Eq("SU.UserDomain", userDomain))
        End If
        userDetachCriteria.SetProjection(Projections.Id())

        criteria.Add(Restrictions.Or(groupConj, Subqueries.Exists(userDetachCriteria)))
        Return criteria
    End Function
End Class
