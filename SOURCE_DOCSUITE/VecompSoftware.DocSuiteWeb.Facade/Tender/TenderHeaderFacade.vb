Imports VecompSoftware.DocSuiteWeb.Data

Public Class TenderHeaderFacade
    Inherits FacadeNHibernateBase(Of TenderHeader, Guid, NHibernateTenderHeaderDao)

    Public Function GetByCIG(cig As String) As TenderHeader
        Return _dao.GetByCIG(cig)
    End Function

    Public Function GetByResolution(resolution As Resolution) As TenderHeader
        Return _dao.GetByResolution(resolution)
    End Function

    Public Function GetByDocumentSeriesItem(idItem As Integer) As TenderHeader
        Return _dao.GetByDocumentSeriesItem(idItem)
    End Function

    Public Function SetPayment(header As TenderHeader, cig As String, key As String, importo As Decimal) As TenderHeader
        Return _dao.SetPayment(header, cig, key, importo)
    End Function

    Public Function GetChangedHeaders(dateFrom As DateTime) As IList(Of TenderHeader)
        Return _dao.GetChangedHeaders(dateFrom)
    End Function
End Class
