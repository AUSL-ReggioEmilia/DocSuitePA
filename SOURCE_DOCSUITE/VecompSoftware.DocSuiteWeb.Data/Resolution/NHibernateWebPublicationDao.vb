Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao
Imports VecompSoftware.Helpers.NHibernate

Public Class NHibernateWebPublicationDao
    Inherits BaseNHibernateDao(Of WebPublication)

#Region " Constructors "
    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub
#End Region

#Region " Methods "

    ''' <summary>Torna tutte le WebPublication relative all'Atto, escludendo gli stati richiesti</summary>
    Public Function GetByResolution(resolution As Resolution) As IList(Of WebPublication)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Resolution", resolution))
        criteria.AddOrder(New Order("LastChangedDate", False))
        Return criteria.List(Of WebPublication)()
    End Function

    ''' <summary>Torna tutte le WebPublication relative all'Atto, escludendo gli stati richiesti</summary>
    Public Function GetByResolution(resolution As Resolution, statusToIgnore As List(Of Integer)) As IList(Of WebPublication)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Resolution", resolution))
        criteria.Add(Expression.Not(Expression.In("Status", statusToIgnore)))
        criteria.AddOrder(New Order("LastChangedDate", False))
        Return criteria.List(Of WebPublication)()
    End Function

    ''' <summary>Controlla l'esistenza di una webPublication, escludendo gli stati richiesti</summary>
    Public Function Exists(resolution As Resolution, statusToIgnore As List(Of Integer)) As Boolean
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Resolution", resolution))
        criteria.Add(Expression.Not(Expression.In("Status", statusToIgnore)))
        Return criteria.SetProjection(Projections.RowCountInt64()).UniqueResult(Of Long)() > 0
    End Function

    ''' <summary>Ritorna l'ultimo numero di pubblicazione revocato relativamente all'atto</summary>
    Public Function GetLastRevokedNumber(resolution As Resolution) As String
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.SetMaxResults(1)
        criteria.Add(Restrictions.Eq("Resolution", resolution))
        criteria.Add(Restrictions.Eq("Status", 1))

        criteria.SetProjection(Projections.Property("ExternalKey"))
        criteria.AddOrder(New Order("LastChangedDate", False))
        Return criteria.UniqueResult(Of String)()
    End Function

    Public Function GetPublishedResolutionsID(startDate As DateTime, endDate As DateTime) As IList(Of Integer)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.SetResultTransformer(Transform.Transformers.DistinctRootEntity)
        criteria.SetProjection(Projections.Property("Resolution.Id"))
        criteria.Add(Expression.Sql(NHibernateHelper.GreaterThanOrEqualToDateIsoFormat("RegistrationDate", startDate)))
        criteria.Add(Expression.Sql(NHibernateHelper.LessThanOrEqualToDateIsoFormat("RegistrationDate", endDate)))
        Return criteria.List(Of Integer)()
    End Function

    Public Function GetFirstPublishedResolutionID(startDate As DateTime, endDate As DateTime) As Integer
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.SetResultTransformer(Transform.Transformers.DistinctRootEntity)
        criteria.SetMaxResults(1)
        criteria.SetProjection(Projections.Cast(NHibernateUtil.Int32, Projections.Property("ExternalKey")))
        criteria.Add(Expression.Sql(NHibernateHelper.GreaterThanOrEqualToDateIsoFormat("RegistrationDate", startDate)))
        criteria.Add(Expression.Sql(NHibernateHelper.LessThanOrEqualToDateIsoFormat("RegistrationDate", endDate)))
        criteria.AddOrder(Order.Asc(Projections.Cast(NHibernateUtil.Int32, Projections.Property("ExternalKey"))))
        Return criteria.UniqueResult(Of Integer)()
    End Function

    Public Function GetLastPublishedResolutionID(startDate As DateTime, endDate As DateTime) As Integer
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.SetResultTransformer(Transform.Transformers.DistinctRootEntity)
        criteria.SetMaxResults(1)
        criteria.SetProjection(Projections.Cast(NHibernateUtil.Int32, Projections.Property("ExternalKey")))
        criteria.Add(Expression.Sql(NHibernateHelper.GreaterThanOrEqualToDateIsoFormat("RegistrationDate", startDate)))
        criteria.Add(Expression.Sql(NHibernateHelper.LessThanOrEqualToDateIsoFormat("RegistrationDate", endDate)))
        criteria.AddOrder(Order.Desc(Projections.Cast(NHibernateUtil.Int32, Projections.Property("ExternalKey"))))
        Return criteria.UniqueResult(Of Integer)()
    End Function

    Public Function HasResolutionPublications(resolution As Resolution) As Boolean
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Resolution", resolution))
        Return criteria.SetProjection(Projections.RowCountInt64()).UniqueResult(Of Long)() > 0
    End Function

    Public Function HasResolutionPrivacyPublications(resolution As Resolution) As Boolean
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Resolution", resolution))
        criteria.Add(Restrictions.Eq("IsPrivacy", True))
        Return criteria.SetProjection(Projections.RowCountInt64()).UniqueResult(Of Long)() > 0
    End Function


#End Region

End Class
