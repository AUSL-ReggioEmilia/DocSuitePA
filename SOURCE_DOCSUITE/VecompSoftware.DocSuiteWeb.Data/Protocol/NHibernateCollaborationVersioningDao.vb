Imports NHibernate.Criterion
Imports System.Collections.Generic
Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate

Public Class NHibernateCollaborationVersioningDao
    Inherits BaseNHibernateDao(Of
        CollaborationVersioning)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

#Region " Methods "

    Public Function GetByCollaboration(ByVal idCollaboration As Integer) As IList(Of CollaborationVersioning)
        criteria = NHibernateSession.CreateCriteria(persitentType, "CV")
        criteria.Add(Restrictions.Eq("CV.Collaboration.Id", idCollaboration))
        criteria.AddOrder(Order.Asc("CV.CollaborationIncremental"))
        criteria.AddOrder(Order.Asc("CV.Incremental"))
        Return criteria.List(Of CollaborationVersioning)()
    End Function

    Public Function GetLastVersioningsByUser(ByVal idCollaborations As Integer(), ByVal user As String) As IList(Of CollaborationVersioning)
        Dim detachedMaxIncremental As DetachedCriteria = DetachedCriteria.For(GetType(CollaborationVersioning), "CVS")
        detachedMaxIncremental.Add(Restrictions.EqProperty("CVS.Collaboration.Id", "CV.Collaboration.Id"))
        detachedMaxIncremental.Add(Restrictions.EqProperty("CVS.CollaborationIncremental", "CV.CollaborationIncremental"))
        detachedMaxIncremental.Add(Restrictions.Eq("CVS.IsActive", 1S))
        detachedMaxIncremental.SetProjection(Projections.Max("CVS.Incremental"))

        criteria = NHibernateSession.CreateCriteria(persitentType, "CV")
        criteria.Add(Restrictions.In("CV.Collaboration.Id", idCollaborations))
        criteria.Add(Subqueries.PropertyIn("CV.Incremental", detachedMaxIncremental))
        criteria.Add(Restrictions.Eq("CV.RegistrationUser", user))

        criteria.AddOrder(Order.Asc("CV.Collaboration.Id"))
        criteria.AddOrder(Order.Asc("CV.CollaborationIncremental"))
        criteria.AddOrder(Order.Asc("CV.Incremental"))
        Return criteria.List(Of CollaborationVersioning)()
    End Function

    Public Function GetLastCollaborationIncremental(idCollaboration As Integer) As Short?
        criteria = NHibernateSession.CreateCriteria(persitentType, "CV")
        criteria.Add(Restrictions.Eq("CV.Collaboration.Id", idCollaboration))
        criteria.SetProjection(Projections.Max("CV.CollaborationIncremental"))
        Return criteria.UniqueResult(Of Short?)()
    End Function
    Public Function GetLastIncremental(idCollaboration As Integer, collaborationIncremental As Short) As Short?
        criteria = NHibernateSession.CreateCriteria(persitentType, "CV")
        criteria.Add(Restrictions.Eq("CV.Collaboration.Id", idCollaboration))
        criteria.Add(Restrictions.Eq("CV.CollaborationIncremental", collaborationIncremental))
        criteria.SetProjection(Projections.Max("CV.Incremental"))
        Return criteria.UniqueResult(Of Short?)()
    End Function

    Public Function GetLastVersioningByIncremental(idCollaboration As Integer, collaborationIncremental As Short) As CollaborationVersioning
        criteria = NHibernateSession.CreateCriteria(persitentType, "CV")
        criteria.Add(Restrictions.Eq("CV.Collaboration.Id", idCollaboration))
        criteria.Add(Restrictions.Eq("CV.CollaborationIncremental", collaborationIncremental))
        criteria.AddOrder(Order.Desc("CV.Incremental"))
        criteria.SetMaxResults(1)
        Return criteria.UniqueResult(Of CollaborationVersioning)
    End Function

    Public Function GetLastVersionings(idCollaboration As Integer) As IList(Of CollaborationVersioning)
        Dim detachedMaxIncremental As DetachedCriteria = DetachedCriteria.For(GetType(CollaborationVersioning), "CVS")
        detachedMaxIncremental.Add(Restrictions.EqProperty("CVS.Collaboration.Id", "CV.Collaboration.Id"))
        detachedMaxIncremental.Add(Restrictions.EqProperty("CVS.CollaborationIncremental", "CV.CollaborationIncremental"))
        detachedMaxIncremental.Add(Restrictions.Eq("CVS.IsActive", 1S))
        detachedMaxIncremental.SetProjection(Projections.Max("CVS.Incremental"))

        criteria = NHibernateSession.CreateCriteria(persitentType, "CV")
        criteria.Add(Restrictions.Eq("CV.Collaboration.Id", idCollaboration))
        criteria.Add(Subqueries.PropertyIn("CV.Incremental", detachedMaxIncremental))
        criteria.AddOrder(Order.Asc("CV.CollaborationIncremental"))
        Return criteria.List(Of CollaborationVersioning)()
    End Function

    Public Function HasCheckOut(idCollaboration As Integer) As Boolean
        Dim detachedMaxIncremental As DetachedCriteria = DetachedCriteria.For(GetType(CollaborationVersioning), "CVS")
        detachedMaxIncremental.Add(Restrictions.EqProperty("CVS.Collaboration.Id", "CV.Collaboration.Id"))
        detachedMaxIncremental.Add(Restrictions.EqProperty("CVS.CollaborationIncremental", "CV.CollaborationIncremental"))
        detachedMaxIncremental.Add(Restrictions.Eq("CVS.IsActive", 1S))
        detachedMaxIncremental.SetProjection(Projections.Max("CVS.Incremental"))

        criteria = NHibernateSession.CreateCriteria(persitentType, "CV")
        criteria.Add(Restrictions.Eq("CV.Collaboration.Id", idCollaboration))
        criteria.Add(Subqueries.PropertyIn("CV.Incremental", detachedMaxIncremental))
        criteria.Add(Restrictions.Eq("CV.CheckedOut", True))
        criteria.SetProjection(Projections.RowCount())
        Return criteria.UniqueResult(Of Integer)() > 0
    End Function
    Public Function GetDocumentInCheckout(idCollaboration As Integer) As IList(Of CollaborationVersioning)
        Dim detachedMaxIncremental As DetachedCriteria = DetachedCriteria.For(GetType(CollaborationVersioning), "CVS")
        detachedMaxIncremental.Add(Restrictions.EqProperty("CVS.Collaboration.Id", "CV.Collaboration.Id"))
        detachedMaxIncremental.Add(Restrictions.EqProperty("CVS.CollaborationIncremental", "CV.CollaborationIncremental"))
        detachedMaxIncremental.Add(Restrictions.Eq("CVS.IsActive", 1S))
        detachedMaxIncremental.SetProjection(Projections.Max("CVS.Incremental"))

        criteria = NHibernateSession.CreateCriteria(persitentType, "CV")
        criteria.Add(Restrictions.Eq("CV.Collaboration.Id", idCollaboration))
        criteria.Add(Subqueries.PropertyIn("CV.Incremental", detachedMaxIncremental))
        criteria.Add(Restrictions.Eq("CV.CheckedOut", True))

        Return criteria.List(Of CollaborationVersioning)()
    End Function
    Public Function GetLastCheckout(idCollaboration As Integer) As CollaborationVersioning
        Dim detachedMaxIncremental As DetachedCriteria = DetachedCriteria.For(GetType(CollaborationVersioning), "CVS")
        detachedMaxIncremental.Add(Restrictions.EqProperty("CVS.Collaboration.Id", "CV.Collaboration.Id"))
        detachedMaxIncremental.Add(Restrictions.EqProperty("CVS.CollaborationIncremental", "CV.CollaborationIncremental"))
        detachedMaxIncremental.Add(Restrictions.Eq("CVS.IsActive", 1S))
        detachedMaxIncremental.SetProjection(Projections.Max("CVS.Incremental"))

        criteria = NHibernateSession.CreateCriteria(persitentType, "CV")
        criteria.Add(Restrictions.Eq("CV.Collaboration.Id", idCollaboration))
        criteria.Add(Subqueries.PropertyIn("CV.Incremental", detachedMaxIncremental))
        criteria.Add(Restrictions.Eq("CV.CheckedOut", True))

        Return criteria.UniqueResult(Of CollaborationVersioning)()
    End Function

    Public Function GetVersioningByCheckOutSessionId(sessionId As String) As CollaborationVersioning
        criteria = NHibernateSession.CreateCriteria(persitentType, "CV")
        criteria.Add(Restrictions.Eq("CV.IsActive", 1S))
        criteria.Add(Restrictions.Eq("CheckOutSessionId", sessionId))
        Return criteria.UniqueResult(Of CollaborationVersioning)()
    End Function

    Public Function GetByAccount(username As String) As IList(Of CollaborationVersioning)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Collaboration", "C", SqlCommand.JoinType.InnerJoin)
        criteria.Add(Restrictions.Eq("CheckedOut", True))
        criteria.Add(Restrictions.Like("CheckOutUser", String.Format("\{0}", username), MatchMode.End))
        criteria.Add(Restrictions.Not(Restrictions.Eq("C.IdStatus", CollaborationStatusType.PT.ToString())))

        Return criteria.List(Of CollaborationVersioning)()
    End Function
#End Region

End Class