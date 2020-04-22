Imports VecompSoftware.NHibernateManager.Dao

Public Interface IMessageContactDao
    Inherits INHibernateDao(Of MessageContact)

    Function GetByMessage(message As DSWMessage) As IList(Of MessageContact)
    Function GetByMessage(message As DSWMessage, position As MessageContact.ContactPositionEnum) As IList(Of MessageContact)
End Interface
