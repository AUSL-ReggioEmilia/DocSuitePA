Imports VecompSoftware.NHibernateManager.Dao

Public Interface IMessageDao
    Inherits INHibernateDao(Of DSWMessage)

    Function GetActiveMessages(type As DSWMessage.MessageTypeEnum, status As DSWMessage.MessageStatusEnum) As IList(Of DSWMessage)
End Interface
