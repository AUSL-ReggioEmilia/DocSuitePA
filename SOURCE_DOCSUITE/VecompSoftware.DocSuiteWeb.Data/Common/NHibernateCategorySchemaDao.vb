Imports System.Collections.Generic
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateCategorySchemaDao
    Inherits BaseNHibernateDao(Of CategorySchema)

#Region "Fields"

#End Region

#Region "Properties"

#End Region

#Region "Constructor"
    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub
#End Region

#Region "Methods"
    Public Function GetActiveCategorySchema([date] As DateTimeOffset) As CategorySchema
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Le("StartDate", [date]))
        criteria.Add(Restrictions.Or(Restrictions.IsNull("EndDate"),
                                     Restrictions.Gt("EndDate", [date])))

        Return criteria.UniqueResult(Of CategorySchema)
    End Function

    Public Function GetManageableCategorySchemas([date] As DateTimeOffset) As ICollection(Of CategorySchema)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Lt("StartDate", [date]))
        criteria.AddOrder(New [Order]("Version", True))

        Return criteria.List(Of CategorySchema)
    End Function

    Public Function GetMaxVersion() As Short
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.IsNotNull("EndDate"))
        criteria.SetProjection(Projections.Max("Version"))
        Return criteria.UniqueResult(Of Short)
    End Function

    Public Function GetCategorySchemaByVersion(version As Short) As CategorySchema
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Version", version))
        Return criteria.UniqueResult(Of CategorySchema)
    End Function
#End Region

End Class
