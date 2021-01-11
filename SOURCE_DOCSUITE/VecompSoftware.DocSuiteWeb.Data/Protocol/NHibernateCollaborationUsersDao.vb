Imports System.Collections.Generic
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateCollaborationUsersDao
    Inherits BaseNHibernateDao(Of CollaborationUser)

#Region " Constructors "

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "
    Public Function GetByAccount(username As String) As IList(Of CollaborationUser)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Collaboration", "C", SqlCommand.JoinType.InnerJoin)
        criteria.Add(Restrictions.Like("Account", String.Format("\{0}", username), MatchMode.End))
        criteria.Add(Restrictions.Not(Restrictions.Eq("C.IdStatus", CollaborationStatusType.PT.ToString())))

        Return criteria.List(Of CollaborationUser)()
    End Function

    Public Function GetByCollaboration(ByVal collaborationId As Integer, ByVal destinationFirst As Boolean?, ByVal destinationType As DestinatonType?) As IList(Of CollaborationUser)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("IdCollaboration", collaborationId))
        If destinationFirst.HasValue Then
            criteria.Add(Restrictions.Eq("DestinationFirst", destinationFirst.Value))
        End If
        If destinationType.HasValue Then
            criteria.Add(Restrictions.Eq("DestinationType", destinationType.Value.ToString()))
        End If

        Return criteria.List(Of CollaborationUser)()
    End Function

#End Region

End Class
