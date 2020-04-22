Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports NHibernate.Transform
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateCollaborationAggregateDao
    Inherits BaseNHibernateDao(Of CollaborationAggregate)
#Region " Constructors "

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "

    Public Function GetCollaborationAggregateById(ByVal collaborationFatherId As Integer) As IList(Of CollaborationAggregate)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("CollaborationFather.Id", collaborationFatherId))


        Return criteria.List(Of CollaborationAggregate)()
    End Function

#End Region

End Class
