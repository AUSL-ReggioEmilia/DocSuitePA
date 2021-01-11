Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateProtocolRoleUserDao
    Inherits BaseNHibernateDao(Of ProtocolRoleUser)

#Region " Constructors "

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "

    Public Function GetByProtocolIdAndAccount(uniqueIdProtocol As Guid, account As String) As IList(Of ProtocolRoleUser)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Protocol.Id", uniqueIdProtocol))
        criteria.Add(Restrictions.Eq("Account", account.ToLower()))
        Return criteria.List(Of ProtocolRoleUser)()
    End Function

#End Region

End Class
