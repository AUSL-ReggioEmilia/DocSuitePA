Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao
Imports System.Linq

Public Class NHibernateResolutionActivityDao
    Inherits BaseNHibernateDao(Of ResolutionActivity)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function CountErrorActivities() As Long
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Status", ResolutionActivityStatus.ProcessedWithErrors))
        criteria.SetProjection(Projections.RowCountInt64())
        Return criteria.UniqueResult(Of Long)()
    End Function

    Public Function GetToBeProcessedByType(type As ResolutionActivityType) As IList(Of ResolutionActivity)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        Dim statusFilters As IList(Of ResolutionActivityStatus) = New List(Of ResolutionActivityStatus)()
        statusFilters.Add(ResolutionActivityStatus.ProcessedWithErrors)
        statusFilters.Add(ResolutionActivityStatus.ToBeProcessed)
        criteria.Add(Restrictions.Le("ActivityDate", New DateTimeOffset(DateTime.Today)))
        criteria.Add(Restrictions.Eq("ActivityType", type))
        criteria.Add(Restrictions.In("Status", statusFilters.ToArray()))
        Return criteria.List(Of ResolutionActivity)()
    End Function

    Public Function CheckPublicationActivityDate(resolution As Resolution, selectedDate As DateTime) As Boolean
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("ActivityType", ResolutionActivityType.Publication))
        criteria.Add(Restrictions.Eq("UniqueIdResolution", resolution.UniqueId))
        criteria.Add(Restrictions.Ge("ActivityDate", New DateTimeOffset(selectedDate)))
        criteria.SetProjection(Projections.RowCount())
        Return criteria.UniqueResult(Of Integer) > 0
    End Function

End Class
