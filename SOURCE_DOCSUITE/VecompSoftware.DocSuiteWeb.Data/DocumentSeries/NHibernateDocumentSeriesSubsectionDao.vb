Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateDocumentSeriesSubsectionDao
    Inherits BaseNHibernateDao(Of DocumentSeriesSubsection)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetByDocumentSeries(series As DocumentSeries) As IList(Of DocumentSeriesSubsection)
        criteria = NHibernateSession.CreateCriteria(Of DocumentSeriesSubsection)("DSS")
        criteria.Add(Restrictions.Eq("DSS.DocumentSeries", series))
        criteria.AddOrder(Order.Asc("DSS.SortOrder"))
        Return criteria.List(Of DocumentSeriesSubsection)()
    End Function



End Class