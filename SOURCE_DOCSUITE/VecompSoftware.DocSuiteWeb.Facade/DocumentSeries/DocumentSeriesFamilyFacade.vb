Imports VecompSoftware.DocSuiteWeb.Data

Public Class DocumentSeriesFamilyFacade
    Inherits BaseProtocolFacade(Of DocumentSeriesFamily, Integer, NHibernateDocumentSeriesFamilyDao)

    Public Function GetFamiliesByArchive(idArchive As Integer) As IList(Of DocumentSeriesFamily)
        Return _dao.GetFamiliesByArchive(idArchive)
    End Function
End Class
