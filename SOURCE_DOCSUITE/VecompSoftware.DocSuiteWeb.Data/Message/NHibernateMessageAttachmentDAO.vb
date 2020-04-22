Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate.Criterion

Public Class NHibernateMessageAttachmentDAO
    Inherits BaseNHibernateDao(Of MessageAttachment)
    Implements IMessageAttachmentDao

    Public Function GetByMessage(message As DSWMessage) As IList(Of MessageAttachment) Implements IMessageAttachmentDao.GetByMessage
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Message", message))
        Return criteria.List(Of MessageAttachment)()
    End Function

End Class
