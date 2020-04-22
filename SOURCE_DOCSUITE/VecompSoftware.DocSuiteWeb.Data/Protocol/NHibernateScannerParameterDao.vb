Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateScannerParameterDao
    Inherits BaseNHibernateDao(Of ScannerParameter)

    Public Sub New(ByVal p_sessionFactoryName As String)
        MyBase.New(p_sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

End Class
