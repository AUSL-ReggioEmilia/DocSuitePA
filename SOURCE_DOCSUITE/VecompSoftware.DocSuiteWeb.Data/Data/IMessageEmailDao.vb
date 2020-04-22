Imports VecompSoftware.NHibernateManager.Dao

Public Interface IMessageEmailDao
    Inherits INHibernateDao(Of MessageEmail)

    Function GetByMessage(message As DSWMessage) As MessageEmail
End Interface
