Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate

Public Class NHibernateDocumentSeriesItemRoleDao
    Inherits BaseNHibernateDao(Of DocumentSeriesItemRole)


    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetRoles(item As DocumentSeriesItem, linkType As DocumentSeriesItemRoleLinkType) As IList(Of Role)

        criteria = NHibernateSession.CreateCriteria(Of DocumentSeriesItemRole)("DSIR")
        criteria.Add(Restrictions.Eq("DSIR.Item", item))
        criteria.Add(Restrictions.Eq("DSIR.LinkType", linkType))
        criteria.SetProjection(Projections.Property("Role"))
        Return criteria.List(Of Role)()

    End Function

    Public Function GetItemRoles(item As DocumentSeriesItem, role As Role, linkType As DocumentSeriesItemRoleLinkType?) As IList(Of DocumentSeriesItemRole)

        If item Is Nothing Then
            Throw New ArgumentNullException("DocumentSeriesItem non valorizzata.", "item")
        End If

        criteria = NHibernateSession.CreateCriteria(Of DocumentSeriesItemRole)("DSIR")
        criteria.Add(Restrictions.Eq("DSIR.Item", item))
        If role IsNot Nothing Then
            criteria.Add(Restrictions.Eq("DSIR.Role", role))
        End If
        If linkType.HasValue Then
            criteria.Add(Restrictions.Eq("DSIR.LinkType", linkType.Value))
        End If

        Return criteria.List(Of DocumentSeriesItemRole)()
    End Function

    Public Function GetOwnersByItems(dsiIdentifiers As List(Of Integer)) As IList(Of DocumentSeriesItemRole)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of DocumentSeriesItemRole)("DSIR")
        criteria.Add(Restrictions.In("DSIR.Item.Id", dsiIdentifiers.ToArray()))
        criteria.Add(Restrictions.Eq("DSIR.LinkType", DocumentSeriesItemRoleLinkType.Owner))
        criteria.SetFetchMode("DSIR.Role", FetchMode.Eager)
        Return criteria.List(Of DocumentSeriesItemRole)()
    End Function

End Class
