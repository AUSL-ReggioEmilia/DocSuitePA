Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateProtocolRejectedRoleDao
    Inherits BaseNHibernateDao(Of ProtocolRejectedRole)

#Region " Constructors "

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

    Public Function GetByRole(idRole As Integer, year As Short, number As Integer) As ProtocolRejectedRole
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of ProtocolRejectedRole)()
        criteria.Add(Restrictions.Eq("Role.Id", idRole))
        criteria.Add(Restrictions.Eq("Protocol.Year", year))
        criteria.Add(Restrictions.Eq("Protocol.Number", number))
        Return criteria.UniqueResult(Of ProtocolRejectedRole)()
    End Function

End Class