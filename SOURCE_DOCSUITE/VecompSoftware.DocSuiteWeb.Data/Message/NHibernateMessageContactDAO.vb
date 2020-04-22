Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate.Criterion

Public Class NHibernateMessageContactDAO
    Inherits BaseNHibernateDao(Of MessageContact)
    Implements IMessageContactDao

    Public Function GetByMessage(message As DSWMessage) As IList(Of MessageContact) Implements IMessageContactDao.GetByMessage
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Message", message))
        Return criteria.List(Of MessageContact)()
    End Function

    Public Function GetByMessage(message As DSWMessage, position As MessageContact.ContactPositionEnum) As IList(Of MessageContact) Implements IMessageContactDao.GetByMessage
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Message", message))
        criteria.Add(Restrictions.Eq("ContactPosition", position))
        Return criteria.List(Of MessageContact)()
    End Function
End Class
