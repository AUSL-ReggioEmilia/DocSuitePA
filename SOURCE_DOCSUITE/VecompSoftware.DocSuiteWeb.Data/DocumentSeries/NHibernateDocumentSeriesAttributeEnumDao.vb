Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate
Imports VecompSoftware.NHibernateManager
Imports NHibernate.Criterion
Imports NHibernate.Transform

Public Class NHibernateDocumentSeriesAttributeEnumDao
    Inherits BaseNHibernateDao(Of DocumentSeriesAttributeEnum)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetByDocumentSeries(idDocumentSeries As Integer) As IList(Of DocumentSeriesAttributeEnum)

        Using session As ISession = NHibernateSessionManager.Instance.OpenSession(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
            Dim criteria As ICriteria = session.CreateCriteria(Of DocumentSeriesAttributeEnum)("AE")
            criteria.SetFetchMode("EnumValues", FetchMode.Join)
            criteria.Add(Restrictions.Eq("AE.DocumentSeries.Id", idDocumentSeries))
            criteria.SetResultTransformer(Transformers.DistinctRootEntity)


            Return criteria.List(Of DocumentSeriesAttributeEnum)()
        End Using

    End Function

    Public Function GetValueDescription(idDocumentSeries As Integer, attributeName As String, attributeValue As Integer) As String
        Using session As ISession = NHibernateSessionManager.Instance.OpenSession(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
            Dim criteria As ICriteria = session.CreateCriteria(Of DocumentSeriesAttributeEnumValue)("AEV")
            criteria.CreateAlias("AEV.Attribute", "AE")
            criteria.SetFetchMode("AEV.Attribute", FetchMode.Eager)

            criteria.Add(Restrictions.Eq("AE.DocumentSeries.Id", idDocumentSeries))
            criteria.Add(Restrictions.Eq("AE.AttributeName", attributeName))
            criteria.Add(Restrictions.Eq("AEV.AttributeValue", attributeValue))

            criteria.SetProjection(Projections.Property("Description"))
            criteria.SetResultTransformer(Transformers.DistinctRootEntity)
            criteria.SetMaxResults(1)
            Return criteria.UniqueResult(Of String)()
        End Using
    End Function

End Class
