Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateServiceCodeDescriptorDao
    Inherits BaseNHibernateDao(Of ServiceCodeDescriptor)


    Public Function GetDescriptorsByDate(reference As DateTime) As IList(Of ServiceCodeDescriptor)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of ServiceCodeDescriptor)("SCD")
        criteria.Add(Restrictions.Le("SCD.RegistrationDate", reference))
        criteria.Add(Restrictions.Ge("SCD.DismissalDate", reference))
        criteria.AddOrder(Order.Asc("SCD.SortIndex"))
        Return criteria.List(Of ServiceCodeDescriptor)()
    End Function

End Class
