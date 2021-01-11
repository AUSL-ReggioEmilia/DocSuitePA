Imports System.Collections.Generic
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateDocumentFolderDao
    Inherits BaseNHibernateDao(Of DocumentFolder)

#Region " Constructors "

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

    Public Function GetMaxId(ByVal year As Short, ByVal number As Integer) As Short
        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("Id.Year", year))
        criteria.Add(Restrictions.Eq("Id.Number", number))

        criteria.SetProjection(Projections.ProjectionList.Add(Projections.Max("Id.Incremental")))

        Return criteria.UniqueResult(Of Short)() + 1S
    End Function

    Function GetByYearAndNumber(ByVal year As Short, ByVal number As Integer, ByVal incrementalFather As Short?) As IList(Of DocumentFolder)
        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.CreateAlias("Role", "R", SqlCommand.JoinType.LeftOuterJoin)

        criteria.Add(Restrictions.Eq("Id.Year", year))
        criteria.Add(Restrictions.Eq("Id.Number", number))
        criteria.Add(Restrictions.Eq("IsActive", 1S))

        If Not incrementalFather.HasValue Then
            criteria.Add(Restrictions.IsNull("IncrementalFather"))
        Else
            criteria.Add(Restrictions.Eq("IncrementalFather", incrementalFather.Value))
        End If
        
        criteria.AddOrder(Order.Asc("Id.Incremental"))

        Return criteria.List(Of DocumentFolder)()
    End Function


    Public Function GetByRole(ByVal year As Short, ByVal number As Integer, ByVal role As Role) As IList(Of DocumentFolder)
        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("Id.Year", year))
        criteria.Add(Restrictions.Eq("Id.Number", number))
        criteria.Add(Restrictions.Eq("IsActive", 1S))
        criteria.Add(Restrictions.Eq("Role.Id", role.Id))

        Return criteria.List(Of DocumentFolder)()
    End Function

    Public Function GetRoot(ByVal year As Short, ByVal number As Integer, ByVal idroleincremental As Short) As IList(Of DocumentFolder)
        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("Id.Year", year))
        criteria.Add(Restrictions.Eq("Id.Number", number))
        criteria.Add(Restrictions.Eq("IsActive", 1S))
        If idroleincremental <> 0 Then
            criteria.Add(Restrictions.Eq("IncrementalFather", idroleincremental))
        Else
            criteria.Add(Restrictions.IsNull("IncrementalFather"))
        End If


        Return criteria.List(Of DocumentFolder)()
    End Function

End Class
