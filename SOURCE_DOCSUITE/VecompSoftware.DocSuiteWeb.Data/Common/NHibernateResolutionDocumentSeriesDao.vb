Imports System.Collections.Generic
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateResolutionDocumentSeriesItemDao
    Inherits BaseNHibernateDao(Of ResolutionDocumentSeriesItem)

#Region " Constructors "

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub
    Public Sub New()
        MyBase.New()
    End Sub

#End Region

    Public Function GetByIdResolution(idResolution As Integer) As IList(Of ResolutionDocumentSeriesItem)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of ResolutionDocumentSeriesItem)("RDSI")
        criteria.Add(Restrictions.Eq("RDSI.Resolution.Id", idResolution))
        criteria.Add(Restrictions.IsNotNull("RDSI.IdDocumentSeriesItem"))
        criteria.AddOrder(Order.Asc("RDSI.IdDocumentSeriesItem"))
        Return criteria.List(Of ResolutionDocumentSeriesItem)()
    End Function

    Public Function GetByIdDocumentSeriesItem(idDocumentSeriesItem As Integer) As IList(Of ResolutionDocumentSeriesItem)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of ResolutionDocumentSeriesItem)("RDSI")
        criteria.Add(Restrictions.Eq("RDSI.IdDocumentSeriesItem", idDocumentSeriesItem))
        criteria.Add(Restrictions.IsNotNull("RDSI.Resolution.Id"))
        criteria.AddOrder(Order.Asc("RDSI.IdDocumentSeries"))
        Return criteria.List(Of ResolutionDocumentSeriesItem)()
    End Function

    Public Function GetResolutions(idDocumentSeriesItem As Integer) As IList(Of Resolution)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of ResolutionDocumentSeriesItem)("RDSI")
        criteria.Add(Restrictions.Eq("RDSI.IdDocumentSeriesItem", idDocumentSeriesItem))
        criteria.Add(Restrictions.IsNotNull("RDSI.Resolution.Id"))
        criteria.SetProjection(Projections.Property("RDSI.Resolution"))
        criteria.AddOrder(Order.Asc("RDSI.Resolution.Id"))
        Return criteria.List(Of Resolution)()
    End Function

    Public Function GetResolutionDocumentSeries() As IList(Of ResolutionDocumentSeriesItem)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of ResolutionDocumentSeriesItem)("RDSI")
        Return criteria.List(Of ResolutionDocumentSeriesItem)()
    End Function

End Class
