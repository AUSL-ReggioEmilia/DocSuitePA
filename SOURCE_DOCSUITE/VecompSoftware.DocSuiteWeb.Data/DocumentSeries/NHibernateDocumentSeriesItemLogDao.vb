Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateDocumentSeriesItemLogDao
    Inherits BaseNHibernateDao(Of DocumentSeriesItemLog)


    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetByItem(item As DocumentSeriesItem) As IList(Of DocumentSeriesItemLog)
        criteria = NHibernateSession.CreateCriteria(Of DocumentSeriesItemLog)("DSIL")
        criteria.Add(Restrictions.Eq("DSIL.DocumentSeriesItem", item))
        Return criteria.List(Of DocumentSeriesItemLog)()
    End Function

    Public Function GetByItemAndLogType(item As DocumentSeriesItem, t As DocumentSeriesItemLogType) As IList(Of DocumentSeriesItemLog)
        criteria = NHibernateSession.CreateCriteria(Of DocumentSeriesItemLog)("DSIL")
        criteria.Add(Restrictions.Eq("DSIL.DocumentSeriesItem", item))
        criteria.Add(Restrictions.Eq("DSIL.LogType", t))
        criteria.AddOrder(New Order("DSIL.LogDate", True))
        Return criteria.List(Of DocumentSeriesItemLog)()
    End Function

End Class
