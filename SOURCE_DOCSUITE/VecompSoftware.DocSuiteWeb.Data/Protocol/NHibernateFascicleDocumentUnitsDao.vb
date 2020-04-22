Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateFascicleDocumentUnitsDao
    Inherits BaseNHibernateDao(Of FascicleDocumentUnit)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Function GetByProtocol(ByVal protocol As Protocol) As IList(Of FascicleDocumentUnit)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("IdDocumentUnit", protocol.UniqueId))
        Return criteria.List(Of FascicleDocumentUnit)()
    End Function

End Class
