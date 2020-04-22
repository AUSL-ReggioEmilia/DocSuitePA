
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class PECOCLogFacade
    Inherits BaseResolutionFacade(Of PECOCLog, Integer, NHibernatePECOCLogDao)

    ''' <summary>
    ''' Data la richiesta, genera il log corrispondente allo stato attuale della stessa.
    ''' </summary>
    Public Sub InsertLog(ByVal pecOc As PECOC)
        InsertLog(pecOc, String.Empty)
    End Sub

    ''' <summary>
    ''' Data la richiesta, genera il log corrispondente allo stato attuale della stessa.
    ''' </summary>
    Public Sub InsertLog(ByVal pecOc As PECOC, ByVal message As String)
        _dao.InsertLog(pecOc, message)

    End Sub

End Class