Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateLocationDao
    Inherits BaseNHibernateDao(Of Location)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetDefaultDocumentServer() As String
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of Location)()
        criteria.Add(Restrictions.IsNotNull("DocumentServer"))
        criteria.SetProjection(Projections.Property("DocumentServer"))
        criteria.SetMaxResults(1)
        Return criteria.UniqueResult(Of String)()
    End Function
End Class
