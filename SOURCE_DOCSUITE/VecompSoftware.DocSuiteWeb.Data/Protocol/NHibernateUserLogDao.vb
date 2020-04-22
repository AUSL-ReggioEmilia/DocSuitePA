Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateUserLogDao
    Inherits BaseNHibernateDao(Of UserLog)


    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetByUser(user As String, domain As String) As UserLog
        Dim username As String = $"{domain}\{user}"
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Id", username))
        Return criteria.UniqueResult(Of UserLog)()
    End Function
End Class
