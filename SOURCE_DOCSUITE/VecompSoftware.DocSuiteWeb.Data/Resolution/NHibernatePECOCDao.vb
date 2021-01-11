Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernatePECOCDao
    Inherits BaseNHibernateDao(Of PECOC)

    ''' <summary> Ritira tutti i <see cref="PECOC"/> in un determinato stato </summary>
    Public Function GetByStatus(status As PECOCStatus) As IList(Of PECOC)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("IsActive", 1S))
        criteria.Add(Restrictions.Eq("Status", status))

        Return criteria.List(Of PECOC)()
    End Function
End Class
