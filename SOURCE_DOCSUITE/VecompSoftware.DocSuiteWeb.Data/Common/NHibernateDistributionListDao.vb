Imports System.Collections.Generic
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateDistributionListDao
    Inherits BaseNHibernateDao(Of DistributionList)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetAllOrderedByName() As IList(Of DistributionList)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.AddOrder(Order.Asc("Name"))
        Return criteria.List(Of DistributionList)()
    End Function
End Class
