
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class PECOCFacade
    Inherits BaseResolutionFacade(Of PECOC, Integer, NHibernatePECOCDao)

    ''' <summary>
    ''' Ritira tutti i <see>PECOC</see> in un determinato stato
    ''' </summary>
    Public Function GetByStatus(ByVal status As PECOCStatus) As IList(Of PECOC)
        Return _dao.GetByStatus(status)
    End Function

End Class
