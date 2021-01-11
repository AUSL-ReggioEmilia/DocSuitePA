Imports System
Imports System.Collections.Generic
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateUserErrorDao
    Inherits BaseNHibernateDao(Of UserError)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetMaxId(ByVal DBType As String) As Integer
        ConnectionName = DBType

        Dim query As String = "SELECT MAX(UE.Id) FROM UserError AS UE"

        Try
            Return NHibernateSession.CreateQuery(query).UniqueResult(Of Integer)()
        Catch ex As Exception
            Return 0
        End Try

    End Function

    Public Function SearchByModule(ByVal User As String, ByVal ModuleName As String, ByVal DateFrom As DateTime) As IList(Of UserError)
        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("SystemUser", User))
        criteria.Add(Restrictions.Eq("ModuleName", ModuleName))
        criteria.Add(Restrictions.Ge("ErrorDate", DateFrom))

        criteria.AddOrder(Order.Desc("Id"))

        Return criteria.List(Of UserError)()
    End Function

End Class
