Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate
Imports NHibernate.Criterion

Public Class NHibernateTenderLotDao
    Inherits BaseNHibernateDao(Of TenderLot)


    Public Function GetByCIG(cig As String) As TenderLot

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of TenderLot)()

        criteria.Add(Restrictions.Eq("CIG", cig))

        criteria.SetMaxResults(1)

        Return criteria.UniqueResult(Of TenderLot)()

    End Function

End Class
