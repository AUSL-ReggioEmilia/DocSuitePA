Imports System.Collections.Generic
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateFileResolutionDao
    Inherits BaseNHibernateDao(Of FileResolution)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetByResolution(ByVal resl As Resolution) As IList(Of FileResolution)

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Id", resl.Id))
        Return criteria.List(Of FileResolution)()

    End Function

End Class
