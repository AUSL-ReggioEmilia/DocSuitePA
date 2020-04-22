Imports System.Collections.Generic
Imports NHibernate
Imports NHibernate.Criterion
Imports NHibernate.SqlCommand
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernatePosteOnlineAccountDao
    Inherits BaseNHibernateDao(Of POLAccount)

#Region " Constructor "

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

    Public Function GetByRoles(ByRef roles As IList(Of Role)) As IList(Of POLAccount)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Roles", "R", JoinType.LeftOuterJoin)

        Dim rolesId(roles.Count) As Integer
        For i As Integer = 0 To roles.Count - 1
            rolesId(i) = roles(i).Id
        Next
        criteria.Add(Expression.In("R.Id", rolesId))
        criteria.Add(Restrictions.Eq("R.IsActive", 1S))

        Return criteria.List(Of POLAccount)()
    End Function

End Class
