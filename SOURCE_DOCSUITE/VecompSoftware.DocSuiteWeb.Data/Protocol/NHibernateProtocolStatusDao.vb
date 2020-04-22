Imports System.Collections.Generic
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateProtocolStatusDao
    Inherits BaseNHibernateDao(Of ProtocolStatus)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetByIncremental(ByVal incremental As Integer) As IList(Of ProtocolStatus)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Incremental", Incremental))
        Return criteria.List(Of ProtocolStatus)()
    End Function

    Public Function GetByDescription(ByVal description As String) As IList(Of ProtocolStatus)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Description", description))
        Return criteria.List(Of ProtocolStatus)()
    End Function

    Public Function GetByProtocolStatusExclusion(ByVal statusExclusion As String) As IList(Of ProtocolStatus)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Not(Restrictions.In("Id", statusExclusion.Split(","c))))
        Return criteria.List(Of ProtocolStatus)()
    End Function

End Class
