Imports System
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateTableLogDao
    Inherits BaseNHibernateDao(Of TableLog)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Sub Save(ByRef entity As TableLog)
        MyBase.Save(entity)
    End Sub
End Class
