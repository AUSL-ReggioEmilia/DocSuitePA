Imports System.Collections.Generic
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateProtocolTypeDao
    Inherits BaseNHibernateDao(Of ProtocolType)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetByDescription(ByVal Description As String) As IList(Of ProtocolType)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        'Description
        criteria.Add(Restrictions.Eq("Description", Description))

        Return criteria.List(Of ProtocolType)()
    End Function

End Class
