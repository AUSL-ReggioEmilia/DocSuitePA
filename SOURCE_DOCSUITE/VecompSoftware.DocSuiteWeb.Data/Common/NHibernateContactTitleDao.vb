Imports System.Collections.Generic
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateContactTitleDao
    Inherits BaseNHibernateDao(Of ContactTitle)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Function GetAll() As IList(Of ContactTitle)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.AddOrder(Order.Asc("Description"))
        Return (criteria.List(Of ContactTitle)())
    End Function

    Public Function GetMaxId() As Integer
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.SetProjection(Projections.ProjectionList.Add(Projections.Max("Id")))
        Return criteria.UniqueResult(Of Integer)()
    End Function

    Public Function ContactTitleUsedProtocol(ByVal contactTitle As ContactTitle) As Boolean
        Dim cntDao As New NHibernateContactDao(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
        Dim cntManualDao As New NHibernateProtocolContactManualDao(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
        Return (cntDao.GetCountByContactTitle(contactTitle) > 0 OrElse cntManualDao.GetCountByContactTitle(contactTitle) > 0)
    End Function

    Public Function ContactTitleUsedDocument(ByVal ContactTitle As ContactTitle) As Boolean
        Dim cntDao As New NHibernateContactDao("DocmDB")
        Return cntDao.GetCountByContactTitle(ContactTitle) > 0
    End Function

    Public Function ContactTitleUsedResolution(ByVal ContactTitle As ContactTitle) As Boolean
        Dim cntDao As New NHibernateContactDao("ReslDB")
        Return cntDao.GetCountByContactTitle(ContactTitle) > 0
    End Function

    Public Function GetByDescription(ByVal Description As String) As IList(Of ContactTitle)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Description", Description))
        Return criteria.List(Of ContactTitle)()
    End Function

End Class
