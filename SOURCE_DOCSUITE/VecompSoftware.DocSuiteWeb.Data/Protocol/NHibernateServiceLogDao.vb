Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateServiceLogDao
    Inherits BaseNHibernateDao(Of ServiceLog)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

End Class
