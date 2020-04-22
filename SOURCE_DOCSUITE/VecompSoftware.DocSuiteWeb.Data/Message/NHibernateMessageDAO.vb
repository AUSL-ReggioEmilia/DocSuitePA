Imports NHibernate.Transform
Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate.Criterion

Public Class NHibernateMessageDAO
    Inherits BaseNHibernateDao(Of DSWMessage)
    Implements IMessageDao

    Public Function GetActiveMessages(type As DSWMessage.MessageTypeEnum, status As DSWMessage.MessageStatusEnum) As IList(Of DSWMessage) Implements IMessageDao.GetActiveMessages
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("MessageType", type))
        criteria.Add(Restrictions.Eq("Status", status))
        criteria.SetResultTransformer(Transformers.DistinctRootEntity)
        Return criteria.List(Of DSWMessage)()
    End Function
End Class
