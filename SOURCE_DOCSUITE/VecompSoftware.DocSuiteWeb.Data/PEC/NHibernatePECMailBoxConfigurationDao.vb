Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate
Imports NHibernate.Criterion

Public Class NHibernatePECMailBoxConfigurationDao
    Inherits BaseNHibernateDao(Of PECMailBoxConfiguration)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

End Class
