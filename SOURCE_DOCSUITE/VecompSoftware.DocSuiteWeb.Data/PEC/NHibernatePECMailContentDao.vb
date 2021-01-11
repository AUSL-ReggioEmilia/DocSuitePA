Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernatePECMailContentDao
    Inherits BaseNHibernateDao(Of PECMailContent)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetByMail(ByVal pec As PECMail) As PECMailContent
        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("IdPECMail", pec.Id))
        'Imposto il TimeOut della query a 5 minuti visto che i dati da spostare sono molti e il server SQL potrebbe essere lento
        criteria.SetTimeout(300)

        Return criteria.UniqueResult(Of PECMailContent)()
    End Function

End Class
