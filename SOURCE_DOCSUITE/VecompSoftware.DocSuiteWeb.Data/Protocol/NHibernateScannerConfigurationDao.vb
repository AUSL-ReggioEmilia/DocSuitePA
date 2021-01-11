Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateScannerConfigurationDao
    Inherits BaseNHibernateDao(Of ScannerConfiguration)

    Public Sub New(ByVal p_sessionFactoryName As String)
        MyBase.New(p_sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub SetDefaultConfiguration(ByVal p_entity As ScannerConfiguration)
        For Each sc As ScannerConfiguration In GetAll()
            sc.IsDefault = False
            Update(sc)
        Next
        p_entity.IsDefault = True
        Update(p_entity)
    End Sub

    Public Function GetDefaultConfiguration() As ScannerConfiguration
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.SetMaxResults(1)
        criteria.Add(Restrictions.Eq("IsDefault", True))
        Return criteria.UniqueResult(Of ScannerConfiguration)()
    End Function

End Class
