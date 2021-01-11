Imports VecompSoftware.NHibernateManager.Dao

Public Interface IMessageContactEmailDao
    Inherits INHibernateDao(Of MessageContactEmail)

    Function GetByContact(message As MessageContact) As MessageContactEmail
End Interface
