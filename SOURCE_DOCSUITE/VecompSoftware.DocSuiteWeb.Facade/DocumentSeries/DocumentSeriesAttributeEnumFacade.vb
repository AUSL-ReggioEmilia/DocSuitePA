Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data

Public Class DocumentSeriesAttributeEnumFacade
    Inherits BaseProtocolFacade(Of DocumentSeriesAttributeEnum, Integer, NHibernateDocumentSeriesAttributeEnumDao)

    Public Function GetByDocumentSeries(idDocumentSeries As Integer) As IList(Of DocumentSeriesAttributeEnum)
        Return _dao.GetByDocumentSeries(idDocumentSeries)
    End Function

    Public Function GetValueDescription(idDocumentSeries As Integer, attributeName As String, attributeValue As Integer) As String
        Return _dao.GetValueDescription(idDocumentSeries, attributeName, attributeValue)
    End Function
End Class
