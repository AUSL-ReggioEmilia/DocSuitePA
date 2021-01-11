Imports System.Collections.Generic
Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate
Imports NHibernate.Criterion

Public Class NHibernateContactPlaceNameDao
    Inherits BaseNHibernateDao(Of ContactPlaceName)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetByDescription(ByVal description As String) As IList(Of ContactPlaceName)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Description", description))
        Return criteria.List(Of ContactPlaceName)()
    End Function

End Class
