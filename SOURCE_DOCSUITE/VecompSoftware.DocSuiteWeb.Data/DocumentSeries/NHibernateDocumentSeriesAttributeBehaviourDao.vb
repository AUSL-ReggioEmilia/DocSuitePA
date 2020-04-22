Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateDocumentSeriesAttributeBehaviourDao
    Inherits BaseNHibernateDao(Of DocumentSeriesAttributeBehaviour)


    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub
    
    Public Function GetAttributeBehaviourGroups(series As DocumentSeries, action As DocumentSeriesAction) As IList(Of String)
        criteria = NHibernateSession.CreateCriteria(Of DocumentSeriesAttributeBehaviour)("DSAB")

        criteria.Add(Restrictions.Eq("DSAB.DocumentSeries", series))
        criteria.Add(Restrictions.Eq("DSAB.Visible", True))
        criteria.Add(Restrictions.Eq("DSAB.Action", action))
        criteria.SetProjection(Projections.Distinct(Projections.Property("DSAB.AttributeGroup")))
        criteria.AddOrder(Order.Asc("DSAB.AttributeGroup"))
        Return criteria.List(Of String)()

    End Function

    Public Function GetAttributeBehaviours(series As DocumentSeries, action As DocumentSeriesAction, group As String, attributeName As String) As IList(Of DocumentSeriesAttributeBehaviour)
        criteria = NHibernateSession.CreateCriteria(Of DocumentSeriesAttributeBehaviour)("DSAB")

        criteria.Add(Restrictions.Eq("DSAB.DocumentSeries", series))
        criteria.Add(Restrictions.Eq("DSAB.Action", action))
        If Not String.IsNullOrEmpty(group) Then
            criteria.Add(Restrictions.Eq("DSAB.AttributeGroup", group))
        End If
        If Not String.IsNullOrEmpty(attributeName) Then
            criteria.Add(Restrictions.Eq("DSAB.AttributeName", attributeName))
        End If
        criteria.AddOrder(Order.Asc("DSAB.Id"))
        Return criteria.List(Of DocumentSeriesAttributeBehaviour)()

    End Function
    
    Public Function GetAttributeBehaviours(series As DocumentSeries, action As DocumentSeriesAction, group As String) As IList(Of DocumentSeriesAttributeBehaviour)
        Return GetAttributeBehaviours(series, action, group, String.Empty)
    End Function

    Public Function GetAttributeBehaviours(series As DocumentSeries, action As DocumentSeriesAction) As IList(Of DocumentSeriesAttributeBehaviour)
        Return GetAttributeBehaviours(series, action, String.Empty, String.Empty)
    End Function
End Class
