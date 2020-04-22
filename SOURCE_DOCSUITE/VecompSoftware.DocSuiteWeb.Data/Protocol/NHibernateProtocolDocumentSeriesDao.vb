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

    Public Function GetByProtocol(year As Short, number As Integer) As IList(Of ProtocolDocumentSeriesItem)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of ProtocolDocumentSeriesItem)("PDSI")
        criteria.Add(Restrictions.Eq("PDSI.Protocol.Id.Year", year))
        criteria.Add(Restrictions.Eq("PDSI.Protocol.Id.Number", number))
        criteria.Add(Restrictions.IsNotNull("PDSI.DocumentSeriesItem.Id"))
        criteria.AddOrder(Order.Asc("PDSI.DocumentSeriesItem.Id"))
        Return criteria.List(Of ProtocolDocumentSeriesItem)()
    End Function

    Public Function GetProtocols(idDocumentSeriesItem As Integer) As IList(Of Protocol)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of ProtocolDocumentSeriesItem)("PDSI")
        criteria.Add(Restrictions.Eq("PDSI.DocumentSeriesItem.Id", idDocumentSeriesItem))
        criteria.Add(Restrictions.IsNotNull("PDSI.Protocol.Id.Year"))
        criteria.Add(Restrictions.IsNotNull("PDSI.Protocol.Id.Number"))
        criteria.SetProjection(Projections.Property("PDSI.Protocol"))
        criteria.AddOrder(Order.Asc("PDSI.Protocol.Id"))
        Return criteria.List(Of Protocol)()
    End Function

End Class
