Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateRecipientDao
    Inherits BaseNHibernateDao(Of Recipient)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

End Class
