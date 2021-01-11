Imports System.Collections.Generic
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateProtocolDocumentSeriesItemDao
    Inherits BaseNHibernateDao(Of ProtocolDocumentSeriesItem)

#Region " Constructors "

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub
    Public Sub New()
        MyBase.New()
    End Sub

#End Region

    Public Function GetByProtocol(idProtocol As Guid) As IList(Of ProtocolDocumentSeriesItem)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of ProtocolDocumentSeriesItem)("PDSI")
        criteria.CreateAlias("PDSI.Protocol", "P")
        criteria.Add(Restrictions.Eq("P.Id", idProtocol))
        criteria.Add(Restrictions.IsNotNull("PDSI.DocumentSeriesItem.Id"))
        criteria.AddOrder(Order.Asc("PDSI.DocumentSeriesItem.Id"))
        Return criteria.List(Of ProtocolDocumentSeriesItem)()
    End Function

    Public Function GetProtocols(idDocumentSeriesItem As Integer) As IList(Of Protocol)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of ProtocolDocumentSeriesItem)("PDSI")
        criteria.CreateAlias("PDSI.Protocol", "P")
        criteria.Add(Restrictions.Eq("PDSI.DocumentSeriesItem.Id", idDocumentSeriesItem))
        criteria.Add(Restrictions.IsNotNull("P.Id"))
        criteria.SetProjection(Projections.Property("PDSI.Protocol"))
        criteria.AddOrder(Order.Asc("P.Year"))
        criteria.AddOrder(Order.Asc("P.Number"))
        Return criteria.List(Of Protocol)()
    End Function

End Class
