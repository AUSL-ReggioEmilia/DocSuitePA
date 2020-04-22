Imports VecompSoftware.DocSuiteWeb.Data

<Serializable> _
Public Class DocumentSeriesSubsectionFacade
    Inherits BaseProtocolFacade(Of DocumentSeriesSubsection, Integer, NHibernateDocumentSeriesSubsectionDao)


    Public Function GetByDocumentSeries(series As DocumentSeries) As IList(Of DocumentSeriesSubsection)
        Return _dao.GetByDocumentSeries(series)
    End Function

End Class
