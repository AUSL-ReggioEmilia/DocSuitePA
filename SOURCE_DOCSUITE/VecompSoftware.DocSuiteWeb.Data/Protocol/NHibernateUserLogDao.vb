Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateUserLogDao
    Inherits BaseNHibernateDao(Of UserLog)

    Protected Overrides ReadOnly Property NHibernateSession As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName, False)
        End Get
    End Property

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetByUser(username As String) As UserLog
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Id", username))
        Return criteria.UniqueResult(Of UserLog)()
    End Function

    Public Function GetUnconfiguredUsers() As IList(Of UserLog)
        'Dim crit As ICriteria = NHibernateSession.CreateCriteria(persitentType, "UL")
        'crit.CreateAlias("UL.Id", "UL.SystemUser", SqlCommand.JoinType.LeftOuterJoin)

        'Dim ownCriteria As DetachedCriteria = DetachedCriteria.For(Of SecurityUsers)("SU")
        'Dim securityQuery As String = String.Concat("SU.UserDomain", "+'\'+", "SU.Account")
        'ownCriteria.Add(Restrictions.EqProperty("UL.SystemUser", securityQuery))
        'ownCriteria.Add(Restrictions.IsNotNull("SU.idUser"))
        'ownCriteria.SetProjection(Projections.Id)
        'crit.Add(Subqueries.Exists(ownCriteria))

        Dim sqlStatement As String = "SELECT * FROM UserLog u LEFT JOIN SecurityUsers su ON u.SystemUser = (su.UserDomain+'\'+su.Account) WHERE su.iduser IS  NULL"
        Dim qry As ISQLQuery = NHibernateSession.CreateSQLQuery(sqlStatement)
        qry.AddEntity("u", New UserLog().GetType())

        Return qry.List(Of UserLog)
    End Function
End Class
