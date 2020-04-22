Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate.Criterion

Public Class NHibernateMessageEmailDAO
    Inherits BaseNHibernateDao(Of MessageEmail)
    Implements IMessageEmailDao

    Public Function GetByMessage(message As DSWMessage) As MessageEmail Implements IMessageEmailDao.GetByMessage
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Message", message))
        Return criteria.UniqueResult(Of MessageEmail)()
    End Function

End Class
