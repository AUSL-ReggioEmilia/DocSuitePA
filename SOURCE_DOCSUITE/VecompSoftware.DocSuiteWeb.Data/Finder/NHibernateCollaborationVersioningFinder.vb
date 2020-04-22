Imports NHibernate
Imports NHibernate.Criterion
Imports System.ComponentModel
Imports VecompSoftware.NHibernateManager
Imports NHibernate.Impl



<Serializable(), DataObject()> _
Public Class NHibernateCollaborationVersioningFinder
    Inherits NHibernateBaseFinder(Of CollaborationVersioning, CollaborationVersioning)

    Public Sub New(idCollaboration As Integer)
        SessionFactoryName = System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB)
        currentIdCollaboration = idCollaboration
    End Sub

#Region " Properties "

    Private Property currentIdCollaboration As Integer?

    Protected ReadOnly Property NHibernateSession As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property

#End Region

    Protected Overrides Function CreateCriteria() As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of CollaborationVersioning)("CV")
        If currentIdCollaboration.HasValue Then
            criteria.Add(Restrictions.Eq("CV.Collaboration.Id", currentIdCollaboration))
        End If

        ' Non ho idea nello specifico di cosa possa servire fare questo giro della morte qua...
        ' Ma dato che c'era anche nella versione con NHibernateDomainObjectFinder la replico. - FG
        Dim dc As DetachedCriteria = CreateDetachFromCriteria(criteria, GetType(CollaborationVersioning))
        criteria = dc.GetExecutableCriteria(NHibernateSession)
        MyBase.AttachFilterExpressions(criteria)

        Return criteria
    End Function
    Public Overrides Function DoSearch() As IList(Of CollaborationVersioning)
        Dim criteria As ICriteria = CreateCriteria()

        If SortExpressions.Count > 0 Then
            Dim orderList As IList = DirectCast(criteria, CriteriaImpl).IterateOrderings()
            orderList.Clear()
            MyBase.AttachSortExpressions(criteria)
        End If
        criteria.SetFirstResult(PageIndex)
        criteria.SetMaxResults(PageSize)

        Return criteria.List(Of CollaborationVersioning)()
    End Function

End Class