Imports VecompSoftware.NHibernateManager.Dao

Public Interface IMessageAttachmentDao
    Inherits INHibernateDao(Of MessageAttachment)

    Function GetByMessage(message As DSWMessage) As IList(Of MessageAttachment)
End Interface
